"""Call graph of T.EXE (Turbo C++ 1990) with texts, random calls, images and phase markers.

Usage: python3 -I tools/callgraph.py gamedata/dos/T.EXE <output dir>

The output contains original texts and stays local (AGENTS.md 2.1, decision F1): use a directory outside the
repository or docs/analyse/callgraph/, which is ignored by git. A second run on the same T.EXE gives identical files.

Writes into the output directory:
  callgraph.json   all functions with callees (incl. immediate arguments), callers,
                   referenced texts, DS globals written, random()/image/sound calls
  functions.md     one section per game function (segment 0000 = runtime is summarised)
  tree.md          call tree below main() (1777:000C), depth-limited, game code only
  phases.dot       Graphviz graph: week loop 1777:01F8 and the phase modules below it

Method (independent of docs/analyse/disasm, which splits functions at segment borders):
  * the whole code area (load image 0 .. DGROUP*16) is disassembled in one linear sweep
    with ndisasm, so functions that spill over a segment start stay intact;
  * function starts = far-call targets (relocated `9A off seg`), near-call targets
    (E8 rel16, relative to the calling function's CS) and `push bp / mov bp,sp` prologues;
  * texts: ENGLISCH.TXT line k (0-based, same as DATENLAYOUT.md) is the far pointer at
    DS:37D0+4k; every direct access to [0x37D0+4k] / [0x37D2+4k] counts as a use of text k;
  * call arguments: immediates pushed since the previous call (push imm, or mov ax/al,imm
    + push ax, xor ax,ax + push ax); first element = first C argument.
Requires ndisasm. Output is generated - do not edit by hand.
"""
import collections, json, os, re, struct, subprocess, sys, tempfile

DGROUP = 0x28DC
TEXT_BASE = 0x37D0          # text k: far pointer at DS:37D0 + 4k (0-based line number)
TEXT_COUNT = 304
RANDOM = (0x21FD, 0x01BA)
CHECKPOINT = (0x21FD, 0x0977)   # empty function, called with phase number 0..6 in the week loop
LOAD_IMAGE = (0x186C, 0x0002)
MAIN = (0x1777, 0x000C)
WEEK = (0x1777, 0x01F8)
IMAGES = ['V-ANBAU', 'V-RAKETE', 'V-BERGE', 'V-FATMIK', 'V-AHANDE', 'V-BHANDE', 'V-MENU',
          'V-ENDE', 'V-KAMPF', 'V-ABEND']

# Names established by analysis (AP3, AP5, STATUS.md). Everything else is listed by address.
KNOWN = {
    '1777:000C': 'main(argc, argv)',
    '1777:01F8': 'play_game: week loop',
    '1777:02DC': 'weekly_prices',
    '1777:04E4': 'init_program',
    '192D:0007': 'menu: main menu (returns 0 = quit)',
    '0E31:0003': 'new_game',
    '21FD:01BA': 'random(n)',
    '21FD:0977': 'checkpoint(n) (empty)',
    '21FD:021F': 'roll_weather',
    '186C:0002': 'load_image(i)',
    '2336:000A': 'save_game',
    '2336:0539': 'load_game',
    '0825:07C4': 'record_wealth_history',
    '0825:06CB': 'compute_ranks',
    '0825:0813': 'compute_wealth',
}


def run_ndisasm(code):
    with tempfile.NamedTemporaryFile(suffix='.bin', delete=False) as f:
        f.write(code)
        name = f.name
    try:
        out = subprocess.run(['ndisasm', '-b16', '-o0', name], capture_output=True, text=True,
                             check=True).stdout
    finally:
        os.remove(name)
    ins = []
    for line in out.splitlines():
        m = re.match(r'([0-9A-F]{8})\s+([0-9A-F]+)\s+(.*)$', line)
        if m:
            ins.append((int(m.group(1), 16), bytes.fromhex(m.group(2)), m.group(3).strip()))
        elif ins and line.strip().startswith('-'):
            # continuation line of a long instruction: '         -XXXX'
            extra = line.strip()[1:]
            a, b, t = ins[-1]
            ins[-1] = (a, b + bytes.fromhex(extra), t)
    return ins


def load(exe):
    d = open(exe, 'rb').read()
    hdr_para = struct.unpack_from('<H', d, 8)[0]
    nrel = struct.unpack_from('<H', d, 6)[0]
    relo = struct.unpack_from('<H', d, 0x18)[0]
    img = d[hdr_para * 16:]
    reloc = set()
    segs = set()
    for i in range(nrel):
        off, seg = struct.unpack_from('<HH', d, relo + i * 4)
        lin = seg * 16 + off
        reloc.add(lin)
        v = struct.unpack_from('<H', img, lin)[0]
        if v * 16 < DGROUP * 16:
            segs.add(v)
    return img, reloc, sorted(segs)


def main():
    exe, out = sys.argv[1], sys.argv[2]
    img, reloc, segs = load(exe)
    code_end = DGROUP * 16
    code = img[:code_end]
    ins = run_ndisasm(code)
    texts = [l.rstrip('\r') for l in open(os.path.join(os.path.dirname(exe), 'ENGLISCH.TXT'),
                                          encoding='latin-1').read().split('\n')]

    def canon(lin):
        s = max(x for x in segs if x * 16 <= lin)
        return s, lin - s * 16

    def name(lin):
        return '%04X:%04X' % canon(lin)

    # ---- function starts with their CS ----
    starts = {}  # linear -> cs
    for a, b, t in ins:
        if b[:1] == b'\x9a' and len(b) == 5 and a + 3 in reloc:
            off, seg = struct.unpack_from('<HH', b, 1)
            starts.setdefault(seg * 16 + off, seg)
    idx = {a: i for i, (a, b, t) in enumerate(ins)}
    for i, (a, b, t) in enumerate(ins[:-1]):
        if b == b'\x55' and ins[i + 1][1] in (b'\x8b\xec', b'\x89\xe5'):
            starts.setdefault(a, canon(a)[0])
    # near calls: iterate until stable (CS of the caller decides the target)
    for _ in range(3):
        order = sorted(starts)
        cur = None
        k = 0
        for a, b, t in ins:
            while k < len(order) and order[k] <= a:
                cur = order[k]
                k += 1
            if cur is None or b[:1] != b'\xe8' or len(b) != 3:
                continue
            cs = starts[cur]
            rel = struct.unpack_from('<h', b, 1)[0]
            tgt = cs * 16 + ((a - cs * 16 + 3 + rel) & 0xFFFF)
            if 0 <= tgt < code_end and tgt not in starts:
                starts[tgt] = cs

    order = sorted(starts)
    funcs = {}
    for i, s in enumerate(order):
        funcs[s] = dict(id=name(s), cs='%04X' % starts[s], lin=s, end=order[i + 1] if i + 1 < len(order) else code_end,
                        calls=[], indirect=[], texts=[], text_dynamic=False, writes=collections.Counter(),
                        reads=collections.Counter(), callers=set())

    # ---- per-instruction analysis ----
    k = 0
    cur = None
    pushes = []
    ax = None
    MEMW = re.compile(r'^(mov|inc|dec|add|sub|and|or|xor|adc|sbb|shl|shr|not|neg|pop)\s+(?:byte |word |dword )?\[(0x[0-9a-f]+)\]')
    MEMR = re.compile(r'\[(0x[0-9a-f]+)\]')
    for a, b, t in ins:
        while k < len(order) and order[k] <= a:
            cur = order[k]
            k += 1
            pushes, ax = [], None
        if cur is None:
            continue
        f = funcs[cur]
        cs = starts[cur]
        op = t.split()[0] if t else ''
        # texts
        for m in re.finditer(r'\[(?:[a-z]{2}\+)?(0x[0-9a-f]+)\]', t):
            v = int(m.group(1), 16)
            if TEXT_BASE <= v < TEXT_BASE + 4 * TEXT_COUNT and ('es:' not in t and 'cs:' not in t and 'ss:' not in t):
                if '+' in t.split('[', 1)[1].split(']')[0]:
                    f['text_dynamic'] = True
                else:
                    kk = (v - TEXT_BASE) // 4
                    if kk not in f['texts']:
                        f['texts'].append(kk)
        if re.search(r'0x37cc\b|0x37ce\b', t):
            f['text_dynamic'] = True
        # address of a table entry passed on (push ds / mov ax,&texts[k] / push ax)
        mi = re.match(r'(?:mov\s+(?:[abcd]x|si|di),|push\s+(?:word )?)(0x[0-9a-f]+)$', t)
        if mi:
            v = int(mi.group(1), 16)
            if TEXT_BASE <= v < TEXT_BASE + 4 * TEXT_COUNT and (v - TEXT_BASE) % 4 == 0:
                kk = (v - TEXT_BASE) // 4
                if kk not in f['texts']:
                    f['texts'].append(kk)
        # DS globals (direct addressing only, no segment override)
        if not re.search(r'\b(es|cs|ss):', t):
            mw = MEMW.match(t)
            if mw:
                f['writes'][int(mw.group(2), 16)] += 1
            elif op not in ('call', 'jmp', 'lea'):
                for mr in MEMR.finditer(t):
                    f['reads'][int(mr.group(1), 16)] += 1
        # argument tracking
        if t.startswith('mov ax,0x') and ',' in t and '[' not in t:
            ax = int(t.split(',')[1], 16)
        elif t.startswith('mov al,0x') and '[' not in t:
            ax = int(t.split(',')[1], 16)
        elif t in ('xor ax,ax', 'sub ax,ax'):
            ax = 0
        elif t == 'push ax':
            pushes.append(ax)
        elif op == 'push' and re.fullmatch(r'push word \[0x8(c[ef]|d[0-9a-f]|e0)\]', t):
            # image handles DS:08CE+2i hold i (V-ANBAU = 0 ...), see load_image
            pushes.append('img:' + IMAGES[(int(t[-6:-1], 16) - 0x8CE) // 2])
        elif op == 'push':
            arg = t[5:]
            if re.fullmatch(r'(byte |word )?(\+|-)?0x[0-9a-f]+', arg):
                v = int(arg.split()[-1].replace('+', ''), 16) if not arg.split()[-1].startswith('-') else -int(arg.split()[-1][1:], 16)
                pushes.append(v)
            else:
                pushes.append(None)
        elif re.match(r'(mov|xor|add|sub|or|and|cbw|cwd|in|lodsw|lodsb|pop|imul|mul|div|idiv|shl|shr|sar|inc|dec|neg|not|xchg|lea|les|lds|cmp|test)\b', t):
            if re.match(r'(cbw)$', t):
                pass
            elif re.match(r'\w+\s+(ax|al|ah)\b', t) or op in ('mul', 'imul', 'div', 'idiv', 'lodsw', 'lodsb', 'cwd') or 'xchg' in t:
                if not op in ('cmp', 'test', 'cwd'):
                    ax = None
        if op == 'call':
            tgt = None
            kind = None
            if b[:1] == b'\x9a' and len(b) == 5 and a + 3 in reloc:
                off, seg = struct.unpack_from('<HH', b, 1)
                tgt, kind = seg * 16 + off, 'far'
            elif b[:1] == b'\xe8' and len(b) == 3:
                rel = struct.unpack_from('<h', b, 1)[0]
                tgt, kind = cs * 16 + ((a - cs * 16 + 3 + rel) & 0xFFFF), 'near'
            if tgt is not None and tgt in funcs:
                args = list(reversed(pushes))
                f['calls'].append(dict(at=name(a), target=funcs[tgt]['id'], kind=kind, args=args, _i=idx[a]))
                funcs[tgt]['callers'].add(f['id'])
            else:
                f['indirect'].append(dict(at=name(a), text=t))
            pushes, ax = [], None
        elif t.startswith('add sp,') or t.startswith('pop cx'):
            pass

    # trim arguments to the stack cleanup after the call (pop cx / add sp,N)
    for f in funcs.values():
        for c in f['calls']:
            i = c.pop('_i') + 1
            n = 0
            while i < len(ins) and ins[i][2] in ('pop cx', 'pop dx', 'pop bx') and n < 2:
                n += 1
                i += 1
            if n == 0 and i < len(ins):
                m = re.match(r'add sp,(?:byte )?\+?(0x[0-9a-f]+)$', ins[i][2])
                if m:
                    n = int(m.group(1), 16) // 2
            c['args'] = c['args'][:n] if n else []

    byid = {f['id']: f for f in funcs.values()}

    def label(fid):
        return f'{fid} {KNOWN[fid]}' if fid in KNOWN else fid

    # summary fields
    rnd = '%04X:%04X' % RANDOM
    for f in funcs.values():
        f['random'] = [c['args'][0] if c['args'] else None for c in f['calls'] if c['target'] == rnd]
        f['images'] = [c['args'][0][4:] if c['args'] and isinstance(c['args'][0], str) else '?'
                       for c in f['calls'] if c['target'] == '%04X:%04X' % LOAD_IMAGE]
        f['checkpoints'] = [c['args'][0] if c['args'] else None for c in f['calls'] if c['target'] == '%04X:%04X' % CHECKPOINT]

    # ---- JSON ----
    os.makedirs(out, exist_ok=True)
    js = []
    for f in sorted(funcs.values(), key=lambda x: x['lin']):
        js.append(dict(id=f['id'], name=KNOWN.get(f['id']), cs=f['cs'], size=f['end'] - f['lin'],
                       callers=sorted(f['callers']), calls=f['calls'], indirect=f['indirect'],
                       texts=f['texts'], text_dynamic=f['text_dynamic'], random=f['random'],
                       images=f['images'], checkpoints=f['checkpoints'],
                       writes=['%04X' % w for w in sorted(f['writes'])],
                       reads=['%04X' % r for r in sorted(f['reads'])]))
    with open(os.path.join(out, 'callgraph.json'), 'w') as o:  # one function per line (diff-friendly)
        o.write('[\n' + ',\n'.join(json.dumps(x, separators=(',', ':')) for x in js) + '\n]\n')

    def short_text(kk):
        s = texts[kk].strip() if kk < len(texts) else '?'
        return (s[:40] + '…') if len(s) > 40 else s

    # ---- functions.md ----
    def fmt_args(args):
        return '(' + ', '.join('?' if v is None else v if isinstance(v, str) else (str(v) if -10 < v < 10 else hex(v)) for v in args) + ')'

    with open(os.path.join(out, 'functions.md'), 'w') as o:
        o.write('# Funktionen von T.EXE (Spielcode)\n\nErzeugt von `tools/callgraph.py` – nicht von Hand bearbeiten. '
                'Text *k* = Zeile *k* (0-basiert) von `ENGLISCH.TXT` = Zeiger `DS:37D0+4k`. '
                'Segment `0000` (Turbo-C-Laufzeit) ist ausgelassen.\n\n')
        cur_seg = None
        for f in sorted(funcs.values(), key=lambda x: x['lin']):
            seg = f['id'][:4]
            if seg == '0000':
                continue
            if seg != cur_seg:
                o.write(f'\n## Segment {seg}\n\n')
                cur_seg = seg
            o.write(f"### {label(f['id'])}\n\n")
            o.write(f"Größe 0x{f['end'] - f['lin']:X}"
                    + (f", CS `{f['cs']}`" if f['cs'] != seg else '') + '. ')
            if f['callers']:
                o.write('Aufrufer: ' + ', '.join(f'`{c}`' for c in sorted(f['callers'])) + '.\n')
            else:
                o.write('Keine direkten Aufrufer.\n')
            calls = [c for c in f['calls'] if not c['target'].startswith('0000')]
            if calls:
                o.write('\n- Ruft: ' + ', '.join(f"`{c['target']}`{fmt_args(c['args']) if c['args'] else ''}" for c in calls) + '\n')
            rt = [c['target'] for c in f['calls'] if c['target'].startswith('0000')]
            if rt:
                o.write('- Laufzeit: ' + ', '.join(f'`{x}`' for x in sorted(set(rt))) + '\n')
            if f['indirect']:
                o.write('- Indirekt: ' + ', '.join(f"`{x['text']}`" for x in f['indirect']) + '\n')
            if f['texts']:
                o.write('- Texte: ' + '; '.join(f'{kk} „{short_text(kk)}“' for kk in f['texts']) + '\n')
            if f['text_dynamic']:
                o.write('- Texte mit berechnetem Index\n')
            if f['random']:
                o.write('- random: ' + ', '.join('?' if v is None else str(v) for v in f['random']) + '\n')
            if f['images']:
                o.write('- Bilder: ' + ', '.join(f['images']) + '\n')
            o.write('\n')

    # ---- tree.md ----
    with open(os.path.join(out, 'tree.md'), 'w') as o:
        o.write('# Aufrufbaum unter main()\n\nErzeugt von `tools/callgraph.py`. Nur Spielcode (ohne Laufzeit `0000`, '
                'Grafiktreiber `0A96`/`0A69`/`23CD`, Hilfen `21FD`), Tiefe ≤ 4; '
                'bereits gezeigte Teilbäume werden mit „↑“ abgekürzt. '
                'Argumente = unmittelbare Werte (? = berechnet).\n\n```\n')
        skip = ('0000', '0A96', '0A69', '23CD', '21FD', '2295')
        shown = set()

        def walk(fid, depth, args=None, prefix=''):
            f = byid[fid]
            extra = []
            if f['texts']:
                extra.append('T ' + ','.join(str(x) for x in f['texts'][:6]) + ('…' if len(f['texts']) > 6 else ''))
            if f['images']:
                extra.append('img ' + ','.join(f['images']))
            if f['random']:
                extra.append(f"rnd×{len(f['random'])}")
            line = prefix + label(fid) + (fmt_args(args) if args else '') + ('  [' + '; '.join(extra) + ']' if extra else '')
            if fid in shown and f['calls']:
                o.write(line + '  ↑\n')
                return
            o.write(line + '\n')
            shown.add(fid)
            if depth >= 4:
                return
            for c in f['calls']:
                t = c['target']
                if t[:4] in skip and t not in ('%04X:%04X' % CHECKPOINT, '21FD:021F'):
                    continue
                walk(t, depth + 1, c['args'], prefix + '  ')
        walk('%04X:%04X' % MAIN, 0)
        o.write('```\n')

    # ---- phases.dot ----
    with open(os.path.join(out, 'phases.dot'), 'w') as o:
        o.write('digraph week {\n  rankdir=LR; node [shape=box, fontname="Helvetica"];\n')
        w = byid['%04X:%04X' % WEEK]
        prev = None
        for i, c in enumerate(w['calls']):
            node = f'n{i}'
            lab = label(c['target']) + (fmt_args(c['args']) if c['args'] else '')
            o.write(f'  {node} [label="{lab}"];\n')
            if prev:
                o.write(f'  {prev} -> {node};\n')
            prev = node
        o.write('}\n')

    print(f'{len(funcs)} functions, {sum(len(f["calls"]) for f in funcs.values())} calls')


if __name__ == '__main__':
    main()
