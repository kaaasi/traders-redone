"""Builds the compact 80286 real-mode CPU test subset used by the RefEmu tests.

Source: SingleStepTests/80286 (MIT, Daniel Balsom), directory v1_real_mode (MOO format).
Usage:  python3 -I make_cputests.py <dir with *.MOO.gz + metadata.json> <revocation_list.txt> <out.bin.gz> [per_file]

Output format (little endian, gzip):
  "CPT1" u32 groupCount
  group: u8 nameLen, name (e.g. "D3.4"), u16 flagsMask, u32 testCount, tests...
  test:  u8 nbytes, bytes, u16 init[14], u16 final[14]   (order ax bx cx dx cs ss ds es sp bp si di ip flags,
                                                          final is complete, not a diff)
         u8 exception (0xFF = none), u32 flagAddress,
         u16 nInit, (u32 addr, u8 value)*, u16 nFinal, (u32 addr, u8 value)*
"""
import gzip, json, os, struct, sys

REGS = ['ax', 'bx', 'cx', 'dx', 'cs', 'ss', 'ds', 'es', 'sp', 'bp', 'si', 'di', 'ip', 'flags']


def regs(mv, off):
    mask = struct.unpack_from('<H', mv, off)[0]; off += 2
    r = {}
    for i, n in enumerate(REGS):
        if mask & (1 << i):
            r[n] = struct.unpack_from('<H', mv, off)[0]; off += 2
    return r


def state(mv, off, length):
    end, st = off + length, {'regs': {}, 'ram': []}
    while off < end:
        tag = bytes(mv[off:off + 4]); sl = struct.unpack_from('<I', mv, off + 4)[0]; off += 8
        if tag == b'REGS':
            st['regs'] = regs(mv, off)
        elif tag == b'RAM ':
            cnt = struct.unpack_from('<I', mv, off)[0]
            st['ram'] = [struct.unpack_from('<IB', mv, off + 4 + 5 * i) for i in range(cnt)]
        off += sl
    return st


def parse(data):
    mv = memoryview(data)
    assert data[:4] == b'MOO '
    off = 8 + struct.unpack_from('<I', mv, 4)[0]
    tests = []
    while off < len(data):
        tag = bytes(mv[off:off + 4]); ln = struct.unpack_from('<I', mv, off + 4)[0]; off += 8
        if tag == b'TEST':
            p, t = off + 4, {}
            while p < off + ln:
                st = bytes(mv[p:p + 4]); sl = struct.unpack_from('<I', mv, p + 4)[0]; p += 8
                if st == b'BYTS':
                    c = struct.unpack_from('<I', mv, p)[0]; t['bytes'] = bytes(mv[p + 4:p + 4 + c])
                elif st == b'INIT':
                    t['init'] = state(mv, p, sl)
                elif st == b'FINA':
                    t['final'] = state(mv, p, sl)
                elif st == b'HASH':
                    t['hash'] = mv[p:p + sl].hex()
                elif st == b'EXCP':
                    t['exc'] = struct.unpack_from('<BI', mv, p)
                p += sl
            tests.append(t)
        off += ln
    return tests


def flags_mask(meta, name):
    parts = name.split('.')
    m = meta['opcodes'][parts[0]]
    if len(parts) > 1:
        m = m['reg'][parts[1]]
    return m.get('flags-mask', 0xFFFF), m.get('status', 'normal')


def main():
    src, revf, out = sys.argv[1:4]
    per = int(sys.argv[4]) if len(sys.argv) > 4 else 40
    meta = json.load(open(os.path.join(src, 'metadata.json')))
    revoked = {l.strip().lower() for l in open(revf) if l.strip()}
    names = sorted(f[:-7] for f in os.listdir(src) if f.endswith('.MOO.gz'))
    blob = bytearray(b'CPT1' + struct.pack('<I', 0))
    groups = total = 0
    for name in names:
        mask, status = flags_mask(meta, name)
        tests = [t for t in parse(gzip.open(os.path.join(src, name + '.MOO.gz')).read())
                 if t.get('hash', '').lower() not in revoked]
        step = max(1, len(tests) // per)
        pick = tests[::step][:per]
        nb = name.encode()
        blob += struct.pack('<B', len(nb)) + nb + struct.pack('<HI', mask, len(pick))
        for t in pick:
            ir = t['init']['regs']; fr = dict(ir); fr.update(t['final']['regs'])
            blob += struct.pack('<B', len(t['bytes'])) + t['bytes']
            blob += struct.pack('<14H', *[ir[r] for r in REGS]) + struct.pack('<14H', *[fr[r] for r in REGS])
            exc = t.get('exc')
            blob += struct.pack('<BI', exc[0] if exc else 0xFF, exc[1] if exc else 0)
            for ram in (t['init']['ram'], t['final']['ram']):
                blob += struct.pack('<H', len(ram)) + b''.join(struct.pack('<IB', a, v) for a, v in ram)
        groups += 1; total += len(pick)
    struct.pack_into('<I', blob, 4, groups)
    with gzip.GzipFile(out, 'wb', mtime=0) as f:
        f.write(bytes(blob))
    print(f'{groups} groups, {total} tests, {os.path.getsize(out)} bytes')


main()
