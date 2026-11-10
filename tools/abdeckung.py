#!/usr/bin/env python3
"""Abdeckung von T.EXE: erzeugt die Portierungstabellen in STATUS.md aus Original, Befunden, Code-Markern und Tests.

Quellen (nichts davon wird von Hand in die Tabellen eingetragen):
  - original/dos/T.EXE           Segmente und Far-Einsprünge (gleiche Ermittlung wie tools/segmap.py)
  - docs/routines/SSSS_OOOO_*.md Befunde; abgedeckt sind die Adressen SSSS:OOOO in der Titelzeile
  - src/**/*.cs                  Marker „[ORIGINAL] SSSS:OOOO“ an der portierten Stelle
  - tests/**/*Differential*.cs   Differenztests gegen den RefEmu; zählen für jede darin genannte Adresse SSSS:OOOO

Aufruf:
  python3 tools/abdeckung.py --write   Tabellen zwischen <!-- abdeckung:anfang --> und <!-- abdeckung:ende --> erneuern
  python3 tools/abdeckung.py --check   Exit 1, wenn STATUS.md veraltet ist oder ein Marker ohne Befund bzw. ein
                                       portierter Befund ohne Differenztest auftaucht (Teil von tools/pruefen.sh)
"""
import collections
import pathlib
import re
import struct
import sys

ROOT = pathlib.Path(__file__).resolve().parents[1]
STATUS = ROOT / "STATUS.md"
BEGIN, END = "<!-- abdeckung:anfang -->", "<!-- abdeckung:ende -->"
ADDR = re.compile(r"\b([0-9A-F]{4}):([0-9A-F]{4})\b")
MARKER = re.compile(r"\[ORIGINAL\]\s+([0-9A-F]{4}):([0-9A-F]{4})")
DGROUP = 0x28DC


def segments(exe: pathlib.Path):
    """Codesegmente (Paragraph → Größe) und Far-Einsprünge je Segment, wie tools/segmap.py."""
    d = exe.read_bytes()
    hdr_para, nrel, relo = struct.unpack_from("<H", d, 8)[0], struct.unpack_from("<H", d, 6)[0], struct.unpack_from("<H", d, 0x18)[0]
    img = d[hdr_para * 16:]
    sites, refs = set(), collections.Counter()
    for i in range(nrel):
        off, seg = struct.unpack_from("<HH", d, relo + i * 4)
        lin = seg * 16 + off
        sites.add(lin)
        refs[struct.unpack_from("<H", img, lin)[0]] += 1
    code_end = DGROUP * 16
    segs = sorted(s for s in refs if s * 16 < code_end)
    size = {s: ((segs[i + 1] * 16) if i + 1 < len(segs) else code_end) - s * 16 for i, s in enumerate(segs)}
    far = collections.defaultdict(set)
    for lin in sites:
        if 3 <= lin < code_end and img[lin - 3] == 0x9A:
            far[struct.unpack_from("<H", img, lin)[0]].add(struct.unpack_from("<H", img, lin - 2)[0])
    return size, far


def findings():
    """Befunde mit Adresse: Adresse → Datei (relativ zu docs/routines)."""
    result = {}
    for f in sorted((ROOT / "docs/routines").glob("*.md")):
        if f.name.startswith("_"):
            continue
        title = f.read_text(encoding="utf-8").splitlines()[0]
        for s, o in ADDR.findall(title):
            result[(int(s, 16), int(o, 16))] = f.name
    return result


def scan(pattern: re.Pattern, files):
    hits = collections.defaultdict(set)
    for f in files:
        if "bin" in f.parts or "obj" in f.parts:
            continue
        for s, o in pattern.findall(f.read_text(encoding="utf-8", errors="replace")):
            hits[(int(s, 16), int(o, 16))].add(f.relative_to(ROOT).as_posix())
    return hits


def build():
    size, far = segments(ROOT / "original/dos/T.EXE")
    found = findings()
    markers = scan(MARKER, (ROOT / "src").rglob("*.cs"))
    tests = scan(ADDR, [p for p in (ROOT / "tests").rglob("*.cs") if "Differential" in p.name])
    errors = [f"Marker [ORIGINAL] {s:04X}:{o:04X} ohne Befund in docs/routines/ ({', '.join(sorted(markers[(s, o)]))})"
              for (s, o) in sorted(markers) if (s, o) not in found]
    errors += [f"{s:04X}:{o:04X} ist portiert (Marker), aber ohne Differenztest (tests/**/*Differential*.cs)"
               for (s, o) in sorted(markers) if (s, o) in found and (s, o) not in tests]

    def seg_of(addr):
        return max((s for s in size if s <= addr[0]), default=addr[0])

    lines = [BEGIN, "<!-- Erzeugt von tools/abdeckung.py --write; nicht von Hand ändern. -->", "",
             "### Routinen mit Befund", "",
             "| Adresse | Befund | C# (`[ORIGINAL]`) | Differenztest | Status |", "|---|---|---|---|---|"]
    for addr in sorted(found):
        m, t = sorted(markers.get(addr, ())), sorted(tests.get(addr, ()))
        status = "✅" if m and t else ("🟢" if m else "🔍")
        lines.append(f"| `{addr[0]:04X}:{addr[1]:04X}` | [{found[addr]}](docs/routines/{found[addr]}) | "
                     f"{', '.join(f'`{x}`' for x in m) or '–'} | {', '.join(f'`{pathlib.PurePosixPath(x).name}`' for x in t) or '–'} | {status} |")
    lines += ["", "Status: ✅ portiert mit Differenztest · 🟢 portiert, Differenztest fehlt · 🔍 nur Befund.", "",
              "### Abdeckung je Segment", "",
              "| Segment | Größe | Far-Einsprünge | mit Befund | portiert | mit Differenztest |", "|---|---:|---:|---:|---:|---:|"]
    total = collections.Counter()
    for s in sorted(size):
        f = [a for a in found if seg_of(a) == s]
        p = [a for a in f if a in markers]
        t = [a for a in p if a in tests]
        row = (len(far[s]), len(f), len(p), len(t))
        total.update(dict(zip("ebpt", row)))
        total["g"] += size[s]
        lines.append(f"| `{s:04X}` | 0x{size[s]:X} | {row[0]} | {row[1]} | {row[2]} | {row[3]} |")
    lines.append(f"| **Summe** | **0x{total['g']:X}** | **{total['e']}** | **{total['b']}** | **{total['p']}** | **{total['t']}** |")
    lines += ["", END]
    return "\n".join(lines), errors


def main():
    mode = sys.argv[1] if len(sys.argv) > 1 else "--check"
    block, errors = build()
    text = STATUS.read_text(encoding="utf-8")
    if BEGIN not in text or END not in text:
        print(f"STATUS.md: Markierungen {BEGIN} … {END} fehlen")
        return 1
    new = text[:text.index(BEGIN)] + block + text[text.index(END) + len(END):]
    if mode == "--write":
        STATUS.write_text(new, encoding="utf-8")
        print("STATUS.md erneuert.")
    elif new != text:
        errors.append("STATUS.md ist veraltet – python3 tools/abdeckung.py --write ausführen und mitcommitten")
    for e in errors:
        print(f"FEHLER: {e}")
    if not errors and mode != "--write":
        print("Abdeckung aktuell.")
    return 1 if errors else 0


if __name__ == "__main__":
    sys.exit(main())
