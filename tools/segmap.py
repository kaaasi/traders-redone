"""Segment map, far-call targets and per-segment disassembly for T.EXE (Turbo C++ 1990).

Usage: python3 -I tools/segmap.py gamedata/dos/T.EXE out_dir
Writes out_dir/segments.md and out_dir/disasm/SSSS.asm (ndisasm required).
"""
import struct, sys, os, collections, subprocess

exe, out = sys.argv[1], sys.argv[2]
d = open(exe, 'rb').read()
hdr_para = struct.unpack_from('<H', d, 8)[0]
nrel = struct.unpack_from('<H', d, 6)[0]
relo = struct.unpack_from('<H', d, 0x18)[0]
img = d[hdr_para * 16:]
DGROUP = 0x28DC

reloc_sites = set()
seg_refs = collections.Counter()
for i in range(nrel):
    off, seg = struct.unpack_from('<HH', d, relo + i * 4)
    lin = seg * 16 + off
    reloc_sites.add(lin)
    seg_refs[struct.unpack_from('<H', img, lin)[0]] += 1

code_end = DGROUP * 16
segs = sorted(s for s in seg_refs if s * 16 < code_end)
bounds = {s: ((segs[i + 1] * 16) if i + 1 < len(segs) else code_end) for i, s in enumerate(segs)}

# far calls: 9A off seg where seg word is a relocation site
far_targets = collections.defaultdict(lambda: collections.Counter())
for lin in reloc_sites:
    if lin >= 3 and lin < code_end and img[lin - 3] == 0x9A:
        off = struct.unpack_from('<H', img, lin - 2)[0]
        seg = struct.unpack_from('<H', img, lin)[0]
        caller = max(s for s in segs if s * 16 <= lin)
        far_targets[seg][off] += 1

# near calls inside each segment (E8 rel16) - heuristic, refined later by recursive descent
near_targets = collections.defaultdict(collections.Counter)
for s in segs:
    base, end = s * 16, bounds[s]
    for p in range(base, end - 2):
        if img[p] == 0xE8:
            rel = struct.unpack_from('<h', img, p + 1)[0]
            t = (p + 3 - base + rel) & 0xFFFF
            # accept only targets that look like a Turbo C prologue (push bp / mov bp,sp)
            tl = base + t
            if tl < end - 3 and img[tl] == 0x55 and img[tl + 1:tl + 3] in (b'\x8b\xec', b'\x89\xe5'):
                near_targets[s][t] += 1

os.makedirs(os.path.join(out, 'disasm'), exist_ok=True)
with open(os.path.join(out, 'segments.md'), 'w') as f:
    f.write('# Segmentkarte T.EXE\n\n')
    f.write('Automatisch erzeugt von `tools/segmap.py`. Segmente sind Paragraphen relativ zum Ladebild '
            '(Ladesegment = 0). DGROUP = `0x28DC`.\n\n')
    f.write('| Segment | Größe | Relok.-Refs | Far-Einsprünge | Near-Funktionen |\n|---|---:|---:|---:|---:|\n')
    for s in segs:
        f.write(f'| `{s:04X}` | 0x{bounds[s]-s*16:X} | {seg_refs[s]} | {len(far_targets[s])} | {len(near_targets[s])} |\n')
    f.write('\n## Far-Einsprünge (Funktionen, die aus anderen Segmenten gerufen werden)\n\n')
    for s in segs:
        if far_targets[s]:
            items = ', '.join(f'`{s:04X}:{o:04X}`×{n}' for o, n in sorted(far_targets[s].items()))
            f.write(f'- **{s:04X}**: {items}\n')

for s in segs:
    base, end = s * 16, bounds[s]
    blob = os.path.join(out, 'disasm', f'{s:04X}.bin')
    open(blob, 'wb').write(img[base:end])
    labels = set(far_targets[s]) | set(near_targets[s])
    asm = subprocess.run(['ndisasm', '-b16', '-o0', blob], capture_output=True, text=True).stdout
    with open(os.path.join(out, 'disasm', f'{s:04X}.asm'), 'w') as f:
        f.write(f'; segment {s:04X}  file offset 0x{hdr_para*16+base:X}  size 0x{end-base:X}\n')
        for line in asm.splitlines():
            try:
                off = int(line[:8], 16)
            except ValueError:
                f.write(line + '\n'); continue
            if off in labels:
                f.write(f'\n; ---- {s:04X}:{off:04X} ----\n')
            f.write(line + '\n')
    os.remove(blob)
print(len(segs), 'segments,', sum(len(v) for v in far_targets.values()), 'far entry points,',
      sum(len(v) for v in near_targets.values()), 'near functions')
