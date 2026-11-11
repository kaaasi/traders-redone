"""Static counts behind docs/analyse/BESTAND.md (sections 1.1-1.3, 1.5, 2.4).

Usage: python3 -I experiments/bestand/statik.py <T.EXE> <disasm dir> <out dir> [repo root]
  <disasm dir>  per-segment listings SSSS.asm as written by tools/segmap.py (or fetch-reference.sh disasm)
Writes <out dir>/graph.json (functions with callees, random() sites, DS accesses) and prints the summary.
Nothing derived from the original is written into the repository.
"""
import bisect, collections, json, os, re, struct, sys

exe, dis, out = sys.argv[1:4]
root = sys.argv[4] if len(sys.argv) > 4 else os.path.join(os.path.dirname(os.path.abspath(__file__)), '..', '..')
d = open(exe, 'rb').read()
names = 'sig lastpage pages nrel hdrpara minalloc maxalloc ss sp csum ip cs relo ovl'.split()
h = dict(zip(names, struct.unpack_from('<14H', d, 0)))
img = d[h['hdrpara'] * 16:]
DG = 0x28DC
code_end = DG * 16
RANDOM = (0x21FD, 0x01BA)
GFX = (0x0A96, 0x0A69, 0x23CD)

# --- header and relocations -------------------------------------------------------------------
sites = {}
for i in range(h['nrel']):
    off, seg = struct.unpack_from('<HH', d, h['relo'] + i * 4)
    sites[seg * 16 + off] = struct.unpack_from('<H', img, seg * 16 + off)[0]
data_targets = collections.Counter(v for lin, v in sites.items() if lin >= code_end)
dgroup = img[code_end:code_end + (h['ss'] - DG) * 16]
print(f"file {len(d)} header {h['hdrpara']*16:#x} image {len(img):#x} minalloc {h['minalloc']} entry {h['cs']:04X}:{h['ip']:04X}")
print(f"DGROUP {DG:04X} size {len(dgroup):#x} stack {h['ss']:04X}:{h['sp']:04X} last non-zero data byte DS:{max(i for i, b in enumerate(dgroup) if b):04X}")
print(f"relocations {h['nrel']}: code {sum(1 for l in sites if l < code_end)}, data {sum(data_targets.values())} "
      f"({', '.join(f'{k:04X}x{v}' for k, v in sorted(data_targets.items()))})")

segs = sorted({v for v in sites.values() if v * 16 < code_end})
base = [s * 16 for s in segs]
bounds = {s: (segs[i + 1] * 16 if i + 1 < len(segs) else code_end) for i, s in enumerate(segs)}
seg_of = lambda lin: segs[bisect.bisect_right(base, lin) - 1]

# --- call graph ---------------------------------------------------------------------------------
farcalls = []
for lin, v in sorted(sites.items()):
    if lin < code_end and img[lin - 3] in (0x9A, 0xEA):
        farcalls.append((seg_of(lin), v, struct.unpack_from('<H', img, lin - 2)[0]))
starts = collections.defaultdict(set)
for _, ts, to in farcalls:
    starts[ts].add(to)
for s in segs:
    b, e = s * 16, bounds[s]
    for p in range(b, e - 3):
        if img[p] == 0xE8:
            t = (p + 3 - b + struct.unpack_from('<h', img, p + 1)[0]) & 0xFFFF
            if b + t < e - 3 and img[b + t] == 0x55 and img[b + t + 1:b + t + 3] == b'\x8b\xec':
                starts[s].add(t)

funcs = {}
for s in segs:
    listing = []
    for line in open(os.path.join(dis, f'{s:04X}.asm')):
        m = re.match(r'^([0-9A-F]{8})  ([0-9A-F]+)\s+(.*)$', line)
        if m:
            listing.append((int(m.group(1), 16), m.group(3).strip()))
    offs = [o for o, _ in listing]
    st = sorted(starts[s])
    for i, f in enumerate(st):
        e = st[i + 1] if i + 1 < len(st) else bounds[s] - s * 16
        F = dict(seg=s, off=f, end=e, far=[], near=[], ints={}, ports=0, ds={}, rng=0)
        for o, t in listing[bisect.bisect_left(offs, f):bisect.bisect_left(offs, e)]:
            m = re.match(r'call 0x([0-9a-f]+):0x([0-9a-f]+)', t)
            if m:
                tgt = (int(m.group(1), 16), int(m.group(2), 16))
                F['far'].append(tgt)
                F['rng'] += tgt == RANDOM
            m = re.match(r'call 0x([0-9a-f]+)$', t)
            if m:
                F['near'].append(int(m.group(1), 16))
                F['rng'] += s == RANDOM[0] and int(m.group(1), 16) == RANDOM[1]   # push cs / call near
            m = re.match(r'int (0x[0-9a-f]+)', t)
            if m:
                F['ints'][m.group(1)] = F['ints'].get(m.group(1), 0) + 1
            F['ports'] += bool(re.match(r'(in|out) ', t))
            if 'cs:' not in t and 'es:' not in t:
                for mm in re.finditer(r'\[(?:(?:bx|si|di)\+)?(0x[0-9a-f]+)\]', t):
                    a = int(mm.group(1), 16)
                    F['ds'][a] = F['ds'].get(a, 0) + 1
        funcs[f'{s:04X}:{f:04X}'] = F
os.makedirs(out, exist_ok=True)
json.dump(dict(segs=segs, sizes={f'{s:04X}': bounds[s] - s * 16 for s in segs}, funcs=funcs), open(os.path.join(out, 'graph.json'), 'w'))

callers = collections.defaultdict(collections.Counter)
for f in funcs.values():
    for ts, _ in f['far']:
        if ts != f['seg']:
            callers[ts][f['seg']] += 1
rng = collections.Counter(); gfx = collections.Counter(); nf = collections.Counter()
for f in funcs.values():
    nf[f['seg']] += 1; rng[f['seg']] += f['rng']
    gfx[f['seg']] += sum(1 for ts, _ in f['far'] if ts in GFX)
print(f"\n{len(segs)} segments, {sum(len(v) for v in starts.values())} called functions, "
      f"random() sites {sum(rng.values())} in {sum(1 for v in rng.values() if v)} segments")
print('seg   size  funcs rng gfx  called from (calls x segments)')
for s in segs:
    print(f"{s:04X} {bounds[s]-s*16:#6x} {nf[s]:5d} {rng[s]:3d} {gfx[s]:3d}  {sum(callers[s].values())} x {len(callers[s])}")

# --- cross-check against docs/analyse/segments.md ---------------------------------------------
doc = {int(m.group(1), 16): int(m.group(2), 16) for m in re.finditer(
    r'^\| `([0-9A-F]{4})` \| 0x([0-9A-F]+) \|', open(os.path.join(root, 'docs/analyse/segments.md')).read(), re.M)}
same = sorted(doc) == segs and all(doc[s] == bounds[s] - s * 16 for s in segs)
print(f"\nsegments.md: {len(doc)} segments, list and sizes {'identical' if same else 'DIFFERENT'}")

# --- DGROUP coverage of DATENLAYOUT.md --------------------------------------------------------
acc = collections.defaultdict(set)
for f in funcs.values():
    for a in f['ds']:
        if 0x100 <= a < len(dgroup):
            acc[a].add(f['seg'])
named = {int(x, 16) for x in re.findall(r'`([0-9A-F]{4})`', open(os.path.join(root, 'docs/analyse/DATENLAYOUT.md')).read())}
game = [a for a in acc if acc[a] - {0x0000, 0x23CD, 0x27C6, 0x2850}]
print(f"DS addresses accessed with a literal address (from DS:0100): {len(acc)}, from game code {len(game)}, "
      f"named in DATENLAYOUT.md {len([a for a in game if a in named])}")

# --- computer player segments: no graphics, sound or input reachable ---------------------------
def reach(k, seen):
    if k in seen or k not in funcs:
        return
    seen.add(k)
    for ts, to in funcs[k]['far']:
        reach(f'{ts:04X}:{to:04X}', seen)
    for to in funcs[k]['near']:
        reach(f"{funcs[k]['seg']:04X}:{to:04X}", seen)
io = {0x0A96, 0x0A69, 0x23CD, 0x21A7, 0x218B, 0x0E9E, 0x27C6, 0x0B67, 0x21AF, 0x186C}
for s in (0x11A3, 0x12C8, 0x135D, 0x141D, 0x150F, 0x16D6):
    seen = set()
    for k, f in funcs.items():
        if f['seg'] == s:
            reach(k, seen)
    hit = sorted(k for k in seen if int(k[:4], 16) in io)
    print(f"{s:04X}: reaches {sorted({k[:4] for k in seen} - {f'{s:04X}'})}, graphics/sound/input/loader: {hit or 'none'}")
