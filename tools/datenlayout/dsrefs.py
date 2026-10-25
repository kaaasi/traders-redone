"""Collect every DGROUP (DS) memory access in T.EXE from the ndisasm listings.

Usage: python3 -I tools/datenlayout/dsrefs.py docs/analyse/disasm out.json [out.md]

Per access: segment:offset, function (nearest preceding '; ---- SSSS:OOOO' label),
mnemonic, width (1/2/4 bytes, 0 = unknown), mode (r / w / rw / addr), index registers,
displacement. Accesses through BP (stack), ES/SS/CS overrides and pure register pointers
are skipped. `push ds` + `mov ax,imm` + `push ax` is recorded as mode 'addr' (far address
of a DS object taken, e.g. passed to a function).

The JSON is the raw material for docs/analyse/DATENLAYOUT.md; the optional Markdown file
is a per-offset summary (generated, not edited by hand).
"""
import json, os, re, sys, collections

MEM = re.compile(r'(?:(byte|word|dword|far|near) )?\[(?:(cs|ds|es|ss):)?([^\]]+)\]')
REG8 = {'al', 'ah', 'bl', 'bh', 'cl', 'ch', 'dl', 'dh'}
REG16 = {'ax', 'bx', 'cx', 'dx', 'si', 'di', 'bp', 'sp', 'es', 'ds', 'ss', 'cs'}
WIDTH = {'byte': 1, 'word': 2, 'dword': 4, 'far': 4}
READ_ONLY = {'cmp', 'test', 'push', 'call', 'jmp', 'les', 'lds'}
WRITE_ONLY = {'mov', 'pop', 'lea'}
MIN_INDEXED_DISP = 0x100  # below that, [reg+disp] is almost certainly a pointer + field offset


def parse_line(line):
    m = re.match(r'([0-9A-F]{8})\s+([0-9A-F]+)\s+(.*)$', line)
    if not m:
        return None
    return int(m.group(1), 16), m.group(2), m.group(3).strip()


def main():
    src, out_json = sys.argv[1], sys.argv[2]
    out_md = sys.argv[3] if len(sys.argv) > 3 else None
    refs = []
    for fn in sorted(os.listdir(src)):
        if not fn.endswith('.asm'):
            continue
        seg = fn[:-4]
        func = f'{seg}:0000'
        hist = []  # last instructions, for the push ds / mov ax,imm / push ax pattern
        for line in open(os.path.join(src, fn)):
            if line.startswith('; ----'):
                func = line.split()[2]
                hist.clear()
                continue
            p = parse_line(line)
            if not p:
                continue
            off, _hex, ins = p
            hist.append(ins)
            if len(hist) > 3:
                hist.pop(0)
            if len(hist) == 3 and hist[0] == 'push ds' and hist[2] == 'push ax':
                mm = re.match(r'mov ax,0x([0-9a-f]+)$', hist[1])
                if mm:
                    refs.append(dict(at=f'{seg}:{off:04X}', func=func, op='push', width=0, mode='addr',
                                     index='', disp=int(mm.group(1), 16), ins=hist[1]))
            parts = ins.split(None, 1)
            mnem = parts[0]
            if mnem in ('rep', 'repe', 'repne', 'lock') or len(parts) < 2:
                continue
            ops = [o.strip() for o in parts[1].split(',')]
            # re-join operands broken by the split inside brackets (no commas there in ndisasm)
            for i, o in enumerate(ops):
                m = MEM.search(o)
                if not m:
                    continue
                kw, override, expr = m.groups()
                if override in ('es', 'ss', 'cs'):
                    continue
                terms = [t for t in re.split(r'\+|(?=-)', expr) if t]
                regs = [t for t in terms if t.strip('-') in REG16]
                if 'bp' in regs or 'sp' in regs:
                    continue
                consts = [t for t in terms if t not in regs]
                if not all(re.fullmatch(r'-?0x[0-9a-f]+', c) for c in consts):
                    continue  # 32-bit / fs/gs forms: data misread as code by the linear sweep
                disp = 0
                for c in consts:
                    disp += int(c, 16) if not c.startswith('-') else -int(c[1:], 16)
                disp &= 0xFFFF
                if not consts:
                    continue
                if regs and disp < MIN_INDEXED_DISP:
                    continue
                width = WIDTH.get(kw, 0)
                if not width:
                    other = [x for j, x in enumerate(ops) if j != i]
                    if any(x in REG8 for x in other):
                        width = 1
                    elif any(x in REG16 for x in other):
                        width = 2
                    elif mnem in ('les', 'lds'):
                        width = 4
                if mnem in ('push', 'pop') or mnem.startswith('inc') or mnem.startswith('dec'):
                    width = width or 2
                if mnem in READ_ONLY:
                    mode = 'r'
                elif i == 0 and mnem in WRITE_ONLY:
                    mode = 'w'
                elif i == 0:
                    mode = 'rw'
                else:
                    mode = 'r'
                if mnem in ('les', 'lds'):
                    width = 4
                refs.append(dict(at=f'{seg}:{off:04X}', func=func, op=mnem, width=width, mode=mode,
                                 index='+'.join(regs), disp=disp, ins=ins))
    json.dump(refs, open(out_json, 'w'), indent=0)
    print(len(refs), 'DS accesses')
    if out_md:
        write_md(refs, out_md)


def write_md(refs, path):
    by = collections.defaultdict(list)
    for r in refs:
        by[r['disp']].append(r)
    with open(path, 'w') as f:
        f.write('# DS-Zugriffe in T.EXE\n\nAutomatisch erzeugt von `tools/datenlayout/dsrefs.py` – nicht von Hand '
                'ändern. Ausgewertet und benannt in [DATENLAYOUT.md](DATENLAYOUT.md).\n\n'
                'Spalten: Breiten (B=Byte, W=Wort, D=Doppelwort, ?=unbekannt), Lesen/Schreiben, '
                'indiziert (Registerindex), Adresse genommen, Segmente.\n\n'
                '| Offset | Breiten | r | w | idx | addr | Segmente |\n|---|---|---:|---:|---:|---:|---|\n')
        wn = {0: '?', 1: 'B', 2: 'W', 4: 'D'}
        for d in sorted(by):
            rs = by[d]
            widths = ''.join(sorted({wn[r['width']] for r in rs if r['mode'] != 'addr'}))
            nr = sum(1 for r in rs if 'r' in r['mode'])
            nw = sum(1 for r in rs if 'w' in r['mode'])
            ni = sum(1 for r in rs if r['index'])
            na = sum(1 for r in rs if r['mode'] == 'addr')
            segs = ' '.join(sorted({r['at'][:4] for r in rs}))
            f.write(f'| `{d:04X}` | {widths} | {nr} | {nw} | {ni} | {na} | {segs} |\n')


if __name__ == '__main__':
    main()
