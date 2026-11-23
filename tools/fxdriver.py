#!/usr/bin/env python3
"""Unpack FXDRIVER.EXE (LZEXE 0.91) and write the image and a disassembly for the analysis of spur musik (R9).

The output contains original code and must stay local: by default it goes to out/fxdriver/ (git-ignored, AGENTS.md 2.1).
Marker offsets [ORIGINAL-FX] OOOO are byte offsets in the unpacked load image (offset 0 = first byte after the
MZ header of the unpacked program, i.e. the load segment = PSP + 10h); see docs/analyse/FXDRIVER.md.

    python3 -I tools/fxdriver.py gamedata/dos/FXDRIVER.EXE [out/fxdriver]

Writes FXDRIVER.UNP.EXE (unpacked MZ program), image.bin (load image without relocation), relocs.txt, and,
if ndisasm is available, code.asm (16-bit disassembly of the code part with image offsets as addresses).
"""
import hashlib
import os
import shutil
import struct
import subprocess
import sys



def unpack(data: bytes):
    """Return (header fields of the unpacked program, image bytes, relocation list of (segment, offset))."""
    if data[:2] != b"MZ":
        raise ValueError("not an MZ program")
    h = struct.unpack_from("<14H", data, 0)
    header_bytes = h[4] * 16
    cs, ip = h[11], h[10]
    base = header_bytes + cs * 16  # LZEXE header at CS:0 of the packed program
    ip0, cs0, sp0, ss0, packed_paras, inc_paras, dec_size, _check = struct.unpack_from("<8H", data, base)
    code = data[base + ip : base + ip + 0x30]
    if not code.startswith(bytes.fromhex("060e1f8b0e0c008bf14e89f78cdb031e0a008ec3fdf3a4")):
        raise ValueError("decompressor not recognised as LZEXE 0.91")

    # Relocation table of version 0.91 at CS:0158 of the packed program.
    relocs = []
    pos = base + 0x158
    seg = off = 0
    while True:
        span = data[pos]
        pos += 1
        if span == 0:
            span = struct.unpack_from("<H", data, pos)[0]
            pos += 2
            if span == 0:
                seg += 0x0FFF
                continue
            if span == 1:
                break
        off += span
        seg += (off & ~0x0F) >> 4
        off &= 0x0F
        relocs.append((seg & 0xFFFF, off))

    # Compressed load module: starts at the first byte after the header (CS - packed_paras = 0 here).
    src = header_bytes + (cs - packed_paras) * 16
    out = bytearray()
    state = {"pos": src, "buf": 0, "cnt": 0}

    def word():
        v = struct.unpack_from("<H", data, state["pos"])[0]
        state["pos"] += 2
        return v

    def byte():
        v = data[state["pos"]]
        state["pos"] += 1
        return v

    state["buf"] = word()
    state["cnt"] = 16

    def bit():
        b = state["buf"] & 1
        state["buf"] >>= 1
        state["cnt"] -= 1
        if state["cnt"] == 0:
            state["buf"] = word()
            state["cnt"] = 16
        return b

    while True:
        if bit():
            out.append(byte())
            continue
        if not bit():
            length = (bit() << 1 | bit()) + 2
            span = byte() - 0x100
        else:
            lo = byte()
            hi = byte()
            span = (lo | ((hi & 0xF8) << 5) | 0xE000) - 0x10000
            length = (hi & 7) + 2
            if length == 2:
                length = byte()
                if length == 0:
                    break  # end of data
                if length == 1:
                    continue  # segment change of the decompressor, no output
                length += 1
        for _ in range(length):
            out.append(out[span])
    fields = {"ip": ip0, "cs": cs0, "sp": sp0, "ss": ss0, "inc_paras": inc_paras, "min_alloc": h[5], "max_alloc": h[6]}
    return fields, bytes(out), relocs


def build_exe(fields, image, relocs) -> bytes:
    header_paras = (0x1C + 4 * len(relocs) + 15) // 16
    total = header_paras * 16 + len(image)
    hdr = bytearray(header_paras * 16)
    # extra memory: keep what the packed program asked for, plus the increase of the load module
    struct.pack_into("<14H", hdr, 0, 0x5A4D, total % 512, (total + 511) // 512, len(relocs), header_paras,
                     fields["min_alloc"] + fields["inc_paras"], fields["max_alloc"], fields["ss"], fields["sp"], 0,
                     fields["ip"], fields["cs"], 0x1C, 0)
    for i, (s, o) in enumerate(relocs):
        struct.pack_into("<HH", hdr, 0x1C + 4 * i, o, s)
    return bytes(hdr) + image


def main(argv):
    if len(argv) < 2:
        print(__doc__)
        return 2
    data = open(argv[1], "rb").read()
    out_dir = argv[2] if len(argv) > 2 else os.path.join("out", "fxdriver")
    fields, image, relocs = unpack(data)
    os.makedirs(out_dir, exist_ok=True)
    open(os.path.join(out_dir, "image.bin"), "wb").write(image)
    open(os.path.join(out_dir, "FXDRIVER.UNP.EXE"), "wb").write(build_exe(fields, image, relocs))
    with open(os.path.join(out_dir, "relocs.txt"), "w") as f:
        for s, o in relocs:
            f.write(f"{s:04X}:{o:04X}  image offset {s * 16 + o:04X}\n")
    print(f"Abbild {len(image)} Bytes (sha256 {hashlib.sha256(image).hexdigest()[:16]}), {len(relocs)} Relokationen, "
          f"Einsprung {fields['cs']:04X}:{fields['ip']:04X}, Stapel {fields['ss']:04X}:{fields['sp']:04X}")
    if shutil.which("ndisasm"):
        with open(os.path.join(out_dir, "code.asm"), "w") as f:
            subprocess.run(["ndisasm", "-b16", os.path.join(out_dir, "image.bin")], stdout=f, check=True)
    print(f"Ausgabe in {out_dir} (nur lokal, nie committen)")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))
