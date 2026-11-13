#!/usr/bin/env python3
"""Coverage of T.EXE: generates the porting tables in STATUS.md from the original, findings, code markers and tests.

Sources (none of them is entered into the tables by hand):
  - gamedata/dos/T.EXE           segments and far entry points (same method as tools/segmap.py); gamedata/ is the link
                                 created by tools/claude/fetch-reference.sh, or set TRADERS_GAMEDATA
  - docs/routines/SSSS_OOOO_*.md findings; covered are the addresses SSSS:OOOO in the title line
  - src/**/*.cs                  markers "[ORIGINAL] SSSS:OOOO" at the ported code
  - tests/**/*.cs                explicit tags "[DIFFERENTIAL] SSSS:OOOO" in differential tests against the RefEmu;
                                 plain addresses in comments do not count

Every [ORIGINAL] in src/ and every [DIFFERENTIAL] in tests/ must be followed by a valid address (4+4 upper-case hex
digits); anything else is an error, so a marker in another spelling cannot be ignored silently.

Collective markers and tags count with every address of their list: the first address after the tag, then further
entries separated by "/" or ",", each a full address SSSS:OOOO or a bare offset OOOO in the segment of the entry before
(e.g. "[ORIGINAL] 192D:0698 / 06C2 / 0702" or "[DIFFERENTIAL] 11A3:007D, 11A3:0180"). The list ends at the first other
text, so addresses later in a sentence ("… then 0609:1A61") do not count. The parser checks itself on every run.

Usage:
  python3 tools/abdeckung.py --write   renew the tables between <!-- abdeckung:anfang --> and <!-- abdeckung:ende -->
  python3 tools/abdeckung.py --check   exit 1 if STATUS.md is stale, a marker has no finding, a ported finding has no
                                       differential test or a marker/tag has no valid address (part of tools/pruefen.sh)
"""
import collections
import os
import pathlib
import re
import struct
import sys

ROOT = pathlib.Path(__file__).resolve().parents[1]
STATUS = ROOT / "STATUS.md"
BEGIN, END = "<!-- abdeckung:anfang -->", "<!-- abdeckung:ende -->"
ADDR = re.compile(r"\b([0-9A-F]{4}):([0-9A-F]{4})\b")
TAG_ADDR = r"\s+([0-9A-F]{4}):([0-9A-F]{4})\b"
MARKER = re.compile(r"\[ORIGINAL\]" + TAG_ADDR)
DIFF_TAG = re.compile(r"\[DIFFERENTIAL\]" + TAG_ADDR)
# further entry of a collective list: separator, then SSSS:OOOO or a bare OOOO (not followed by another word character)
LIST_NEXT = re.compile(r"\s*[/,]\s*(?:([0-9A-F]{4}):)?([0-9A-F]{4})\b(?!:)")
DGROUP = 0x28DC


def segments(exe: pathlib.Path):
    """Code segments (paragraph -> size) and far entry points per segment, as in tools/segmap.py."""
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
    """Findings with an address: address -> file name (relative to docs/routines)."""
    result = {}
    for f in sorted((ROOT / "docs/routines").glob("*.md")):
        if f.name.startswith("_"):
            continue
        title = f.read_text(encoding="utf-8").splitlines()[0]
        for s, o in ADDR.findall(title):
            result[(int(s, 16), int(o, 16))] = f.name
    return result


def scan(tag: str, pattern: re.Pattern, files, errors):
    """
    Addresses after `tag` (address -> files) and, for the further entries of collective lists, their list head
    (address -> head); a `tag` without a valid address is reported in `errors`.
    """
    hits = collections.defaultdict(set)
    heads = {}
    for f in files:
        if "bin" in f.parts or "obj" in f.parts:
            continue
        rel = f.relative_to(ROOT).as_posix()
        for n, line in enumerate(f.read_text(encoding="utf-8", errors="replace").splitlines(), 1):
            valid = list(pattern.finditer(line))
            for m in valid:
                head, *rest = collective(line, m)
                hits[head].add(rel)
                for addr in rest:
                    hits[addr].add(rel)
                    heads.setdefault(addr, head)
            if line.count(tag) > len(valid):
                errors.append(f"{tag} ohne gültige Adresse SSSS:OOOO (Großbuchstaben) in {rel}:{n}: {line.strip()}")
    return hits, heads


def collective(line: str, first: re.Match):
    """All addresses of the list that starts with the tag match `first` (first address, then LIST_NEXT entries)."""
    seg = int(first.group(1), 16)
    result = [(seg, int(first.group(2), 16))]
    pos = first.end()
    while (m := LIST_NEXT.match(line, pos)) is not None:
        if m.group(1):
            seg = int(m.group(1), 16)
        result.append((seg, int(m.group(2), 16)))
        pos = m.end()
    return result


def parser_errors():
    """Self-check of the marker parser (counter-check: taking only the first address of a list fails here)."""
    cases = [
        ("// [ORIGINAL] 192D:0698 / 06C2 / 06E2 – one option", [(0x192D, 0x0698), (0x192D, 0x06C2), (0x192D, 0x06E2)]),
        ("/// [ORIGINAL] 192D:0D10 / 0D42 (delta) – columns", [(0x192D, 0x0D10), (0x192D, 0x0D42)]),
        ("// [ORIGINAL] 11A3:007D, 135D:000E, 0100", [(0x11A3, 0x007D), (0x135D, 0x000E), (0x135D, 0x0100)]),
        ("/// [ORIGINAL] 0609:1854(x, y): emblem, then 0609:1A61", [(0x0609, 0x1854)]),
        ("// [ORIGINAL] 21FD:0687 – random(0); 186C:0002([08DA])", [(0x21FD, 0x0687)]),
        ("// [ORIGINAL] 22A3:0750 / 0d42 / 12345", [(0x22A3, 0x0750)]),
        ("// [ORIGINAL] 1777:01F8 / 02DC:x", [(0x1777, 0x01F8)]),
        ("// [ORIGINAL] 0E31:0003 and [ORIGINAL] 0E31:0465 / 0149", [(0x0E31, 0x0003), (0x0E31, 0x0465), (0x0E31, 0x0149)]),
    ]
    errors = []
    for line, expected in cases:
        got = [a for m in MARKER.finditer(line) for a in collective(line, m)]
        if got != expected:
            hexed = lambda xs: ", ".join(f"{a:04X}:{b:04X}" for a, b in xs)
            errors.append(f"Selbsttest Sammelmarker: {line!r} ergibt [{hexed(got)}], erwartet [{hexed(expected)}]")
    return errors


def exe_path() -> pathlib.Path:
    base = os.environ.get("TRADERS_GAMEDATA")
    return (pathlib.Path(base) if base else ROOT / "gamedata") / "dos" / "T.EXE"


def build():
    size, far = segments(exe_path())
    found = findings()
    errors = parser_errors()
    markers, marker_heads = scan("[ORIGINAL]", MARKER, sorted((ROOT / "src").rglob("*.cs")), errors)
    tests, _ = scan("[DIFFERENTIAL]", DIFF_TAG, sorted((ROOT / "tests").rglob("*.cs")), errors)
    # Further entries of a collective marker are covered by the finding and the differential test of the list head unless
    # they have their own (shown as "über <head>" in the table).
    via = {}
    for addr, head in marker_heads.items():
        if addr not in found and head in found:
            found[addr] = found[head]
            via[addr] = head
        if addr not in tests and head in tests:
            tests[addr] = tests[head]
            via[addr] = head
    errors += [f"Marker [ORIGINAL] {s:04X}:{o:04X} ohne Befund in docs/routines/ ({', '.join(sorted(markers[(s, o)]))})"
              for (s, o) in sorted(markers) if (s, o) not in found]
    errors += [f"{s:04X}:{o:04X} ist portiert (Marker), aber ohne Differenztest ([DIFFERENTIAL] in tests/**/*.cs)"
               for (s, o) in sorted(markers) if (s, o) in found and (s, o) not in tests]

    def seg_of(addr):
        return max((s for s in size if s <= addr[0]), default=addr[0])

    lines = [BEGIN, "<!-- Erzeugt von tools/abdeckung.py --write; nicht von Hand ändern. -->", "",
             "### Routinen mit Befund", "",
             "| Adresse | Befund | C# (`[ORIGINAL]`) | Differenztest | Status |", "|---|---|---|---|---|"]
    for addr in sorted(found):
        m, t = sorted(markers.get(addr, ())), sorted(tests.get(addr, ()))
        status = "✅" if m and t else ("🟢" if m else "🔍")
        if addr in via:
            status += f" über `{via[addr][0]:04X}:{via[addr][1]:04X}`"
        lines.append(f"| `{addr[0]:04X}:{addr[1]:04X}` | [{found[addr]}](docs/routines/{found[addr]}) | "
                     f"{', '.join(f'`{x}`' for x in m) or '–'} | {', '.join(f'`{pathlib.PurePosixPath(x).name}`' for x in t) or '–'} | {status} |")
    lines += ["", "Status: ✅ portiert mit Differenztest (`[DIFFERENTIAL]`) · 🟢 portiert, Differenztest fehlt · 🔍 nur Befund.", "",
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
    if not exe_path().exists():
        # Without the original files the segment table cannot be computed; report it instead of passing silently.
        print(f"Abdeckung nicht geprüft: {exe_path()} fehlt (tools/claude/fetch-reference.sh original)")
        return 4
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
