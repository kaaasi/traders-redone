#!/usr/bin/env python3
"""Function map of T.EXE and its coverage by the port (findings in code and docs per function).

Adopted from kaaasi/eishockey-manager-redone (tools/reverse-engineering/funktionskarte.py, em.exe with Borland VROOMM
overlays) and adapted to T.EXE (Turbo C++ 1990, plain MZ image without overlays, addresses SSSS:OOOO relative to the
load image as in AGENTS.md 2.8).

Usage:
    python3 tools/funktionskarte.py <path/to/T.EXE> [--repo .] [--csv out.csv] [--vorlagenregel]

Prints a Markdown summary (status shares per segment, largest gaps) to stdout; with --csv every function is written
as one row (outside the repo: the column `texte` holds short strings of the original). Needs `ndisasm` (nasm). Reads
T.EXE only; it writes nothing into the repo.

Steps:
 1. Layout: MZ header, code segments from the relocation table (same method as tools/segmap.py and tools/abdeckung.py);
    DGROUP = 28DC. A function is addressed as SSSS:OOOO (segment = paragraph in the load image) and by its file offset.
 2. Function starts: segment starts, prologs (push bp / mov bp,sp), far call targets (9A with relocated segment),
    far pointers in the data segment (relocated segment word with its offset word before it), near call targets
    (with a prolog or at least two callers). A function ends where the next one starts (same segment).
 3. Coverage: addresses SSSS:OOOO in src/**/*.cs comment blocks and docs/**/*.md (except docs/archiv/, docs/reviews/ and
    the inventories in NOT_EVIDENCE_DOCS) are mapped onto the functions; a list "SSSS:OOOO / OOOO, OOOO" and a range
    "SSSS:OOOO..OOOO" (also "…", or "–" without blanks; end in the same segment) count with every entry. In code the marker of a finding is the nearest
    [ORIGINAL]/[APPROX] before it in the same comment block (otherwise the next one after it):
      - [ORIGINAL]: only the addresses of the marker's own list (directly after the tag, as in tools/abdeckung.py) count
        as "umgesetzt"; other addresses after an [ORIGINAL] are mentions ("dokumentiert"). --vorlagenregel counts every
        address after an [ORIGINAL] as "umgesetzt" (rule of the template) for comparison.
      - [APPROX]: "genähert"; the line of the [APPROX] is kept (column `approx`) so that the class of the rest list
        (tools/markerliste.py, docs/analyse/restliste-klassen.csv) can be looked up.
      - no marker: "dokumentiert". Findings in docs are always "dokumentiert".
 4. Hints: plain DGROUP strings, image numbers passed to the image loader 186C:0002, globals, callers.
"""

from __future__ import annotations

import argparse
import bisect
import csv
import re
import struct
import subprocess
import sys
import tempfile
from collections import defaultdict
from dataclasses import dataclass, field
from pathlib import Path

DGROUP = 0x28DC
IMAGE_LOADER = (0x186C, 0x0002)  # load picture i (docs/routines/186C_ressourcenlader.md)
STATUS_ORDER = ["umgesetzt", "genähert", "dokumentiert", "unbekannt"]
# inventories that list addresses without being evidence (generated or copied lists)
NOT_EVIDENCE_DOCS = {"docs/analyse/segments.md", "docs/analyse/restliste.md"}
NOT_EVIDENCE_DIRS = ("docs/archiv/", "docs/reviews/", "docs/analyse/abdeckung-grafik/")

# Group of each segment (docs/analyse/BESTAND.md §1.3).
GROUPS = {
    "Phasen/Regeln": "0609 0825 08CF 0B9F 0C29 0DFD 0E31 111D 1777 17CB 192D 1B03 1DEF 1F63 20B3 22A3",
    "Roboterkampf": "0535 0EDE 1C68 1E7E",
    "Computerspieler": "11A3 12C8 135D 141D 150F 16D6",
    "Grafik": "0A69 0A96 0B67 21AF 23CD",
    "System/Hilfen": "0D73 0E9E 186C 21FD 2295 2336 27C6 2821",
    "Ton": "218B 21A7 2850",
    "Laufzeit": "0000 28DA 28DB",
}
GROUP_OF = {int(s, 16): g for g, ss in GROUPS.items() for s in ss.split()}


@dataclass
class Region:
    seg: int
    start: int  # offset in the load image (= seg * 16)
    size: int
    header: int  # file offset of the load image

    @property
    def end(self) -> int:
        return self.start + self.size

    @property
    def name(self) -> str:
        return f"{self.seg:04X}"

    @property
    def group(self) -> str:
        return GROUP_OF.get(self.seg, "?")


@dataclass
class Function:
    start: int
    end: int
    region: Region
    reasons: set[str] = field(default_factory=set)
    refs: dict[str, list[str]] = field(default_factory=lambda: defaultdict(list))
    approx: set[str] = field(default_factory=set)  # "src/…:line" of the [APPROX] lines its "genähert" findings belong to
    strings: list[str] = field(default_factory=list)
    images: set[int] = field(default_factory=set)
    globals_: set[int] = field(default_factory=set)
    callers: set[int] = field(default_factory=set)
    callees: set[int] = field(default_factory=set)

    @property
    def size(self) -> int:
        return self.end - self.start

    @property
    def status(self) -> str:
        for s in STATUS_ORDER[:3]:
            if self.refs.get(s):
                return s
        return "unbekannt"

    @property
    def offset(self) -> int:
        return self.start - self.region.start

    def address(self) -> str:
        return f"{self.region.seg:04X}:{self.offset:04X}"

    def file_offset(self) -> int:
        return self.region.header + self.start


# ---------------------------------------------------------------- layout

def read_layout(exe: bytes):
    hdr = struct.unpack_from("<H", exe, 8)[0] * 16
    nrel = struct.unpack_from("<H", exe, 6)[0]
    relo = struct.unpack_from("<H", exe, 0x18)[0]
    img = exe[hdr:]
    sites = []
    segrefs = set()
    for i in range(nrel):
        off, seg = struct.unpack_from("<HH", exe, relo + i * 4)
        lin = seg * 16 + off
        sites.append(lin)
        segrefs.add(struct.unpack_from("<H", img, lin)[0])
    code_end = DGROUP * 16
    segs = sorted(s for s in segrefs if s * 16 < code_end)
    regions = []
    for i, s in enumerate(segs):
        end = segs[i + 1] * 16 if i + 1 < len(segs) else code_end
        regions.append(Region(s, s * 16, end - s * 16, hdr))
    return hdr, img, sorted(sites), regions


# ---------------------------------------------------------------- disassembly

INSN = re.compile(r"^([0-9A-F]{8})\s+([0-9A-F]+)\s+(.*)$")
NEAR_CALL = re.compile(r"^call 0x([0-9a-f]+)$")
IMM = re.compile(r"^(push|mov) (?:word )?(?:[a-z]{2}|word \[[^\]]+\]|\[[^\]]+\])?,?(?:word )?0x([0-9a-f]+)$")
MEM = re.compile(r"\[(?:(?:[a-z]{2})(?:\+[a-z]{2})?)?([+-])?0x([0-9a-f]+)\]")


def disassemble(img: bytes, region: Region, sync: list[int]) -> list[tuple[int, bytes, str]]:
    """Linear sweep with ndisasm; `sync` = region offsets where decoding must restart (known function starts)."""
    with tempfile.NamedTemporaryFile(suffix=".bin") as tmp:
        tmp.write(img[region.start:region.end])
        tmp.flush()
        cmd = ["ndisasm", "-b16", "-o0"]
        for off in sorted(set(sync)):
            cmd += ["-s", str(off)]
        out = subprocess.run(cmd + [tmp.name], capture_output=True, text=True, check=True).stdout
    result = []
    for line in out.splitlines():
        m = INSN.match(line)
        if m:
            result.append((int(m.group(1), 16), bytes.fromhex(m.group(2)), m.group(3)))
    return result


class Analyzer:
    def __init__(self, exe: bytes):
        self.exe = exe
        self.header, self.img, self.sites, self.regions = read_layout(exe)
        self.by_seg = {r.seg: r for r in self.regions}
        self.site_set = set(self.sites)
        self.code: dict[int, list[tuple[int, bytes, str]]] = {}
        self.functions: list[Function] = []
        self.starts: list[int] = []
        self.dgroup = DGROUP * 16
        self.dgroup_size = len(self.img) - self.dgroup

    def region_of(self, lin: int) -> Region | None:
        for r in self.regions:
            if r.start <= lin < r.end:
                return r
        return None

    def lin(self, seg: int, off: int) -> int | None:
        r = self.by_seg.get(seg)
        if r is None or off >= r.size:
            return None
        return r.start + off

    def run(self) -> None:
        starts: dict[int, set[str]] = defaultdict(set)
        for r in self.regions:
            starts[r.start].add("Segmentanfang")
            blob = self.img[r.start:r.end]
            for m in re.finditer(rb"\x55\x8b\xec", blob):  # push bp; mov bp,sp
                starts[r.start + m.start()].add("Prolog")
        far_calls: list[tuple[int, int]] = []
        for lin in self.sites:
            seg = struct.unpack_from("<H", self.img, lin)[0]
            off = struct.unpack_from("<H", self.img, lin - 2)[0] if lin >= 2 else 0
            tgt = self.lin(seg, off)
            if tgt is None:
                continue
            if lin < self.dgroup and lin >= 3 and self.img[lin - 3] == 0x9A:
                starts[tgt].add("Aufruf")
                far_calls.append((lin - 3, tgt))
            elif lin >= self.dgroup:
                starts[tgt].add("Zeiger")  # far pointer in the data segment (e.g. table of drawing commands)
        calls: list[tuple[int, int]] = []
        for _ in range(4):
            before = len(starts)
            calls = list(far_calls)
            near_targets: dict[int, int] = defaultdict(int)
            for r in self.regions:
                sync = [a - r.start for a in starts if r.start <= a < r.end]
                code = disassemble(self.img, r, sync)
                self.code[r.seg] = code
                for off, raw, text in code:
                    m = NEAR_CALL.match(text)
                    if m and raw[0] == 0xE8:
                        tgt = int(m.group(1), 16)
                        if tgt < r.size:
                            near_targets[r.start + tgt] += 1
                            calls.append((r.start + off, r.start + tgt))
            # near calls may come from mis-decoded data: accept a target with a prolog or at least two callers
            for tgt, n in near_targets.items():
                if tgt in starts or n >= 2:
                    starts[tgt].add("Aufruf")
            if len(starts) == before:
                break
        calls = [(site, tgt) for site, tgt in calls if tgt in starts]
        ordered = sorted(a for a in starts if self.region_of(a))
        funcs = []
        for idx, a in enumerate(ordered):
            reg = self.region_of(a)
            nxt = ordered[idx + 1] if idx + 1 < len(ordered) else reg.end
            funcs.append(Function(a, min(nxt, reg.end), reg, reasons=starts[a]))
        self.functions = funcs
        self.starts = [f.start for f in funcs]
        for site, tgt in calls:
            caller, callee = self.func_at(site), self.func_at(tgt)
            if caller and callee and caller is not callee:
                caller.callees.add(callee.start)
                callee.callers.add(caller.start)
        self._collect_operands()

    def func_at(self, lin: int) -> Function | None:
        i = bisect.bisect_right(self.starts, lin) - 1
        if i >= 0 and self.functions[i].start <= lin < self.functions[i].end:
            return self.functions[i]
        return None

    def dgroup_string(self, off: int) -> str | None:
        if off < 0x40 or off >= self.dgroup_size:  # small immediates are counters, not string offsets
            return None
        base = self.dgroup + off
        if self.img[base - 1] != 0:
            return None
        end = self.img.find(b"\0", base, base + 80)
        if end < 0:
            return None
        raw = self.img[base:end]
        if len(raw) >= 3 and all(0x20 <= c < 0x7F for c in raw) and sum(c >= 0x41 for c in raw) >= 2:
            return raw.decode("latin-1")
        return None

    def _collect_operands(self) -> None:
        loader = self.lin(*IMAGE_LOADER)
        for r in self.regions:
            code = self.code[r.seg]
            for i, (off, raw, text) in enumerate(code):
                lin = r.start + off
                f = self.func_at(lin)
                if f is None:
                    continue
                m = IMM.match(text)
                if m:
                    s = self.dgroup_string(int(m.group(2), 16))
                    if s and s not in f.strings:
                        f.strings.append(s)
                for sign, num in MEM.findall(text):
                    v = int(num, 16)
                    if sign == "-":
                        v = (-v) & 0xFFFF
                    if v >= 0x100 and "bp" not in text.split("[", 1)[1].split("]")[0]:
                        f.globals_.add(v)
                # image number: last immediate pushed before a far call into the loader
                if raw[0] == 0x9A and lin + 3 in self.site_set and loader is not None:
                    seg = struct.unpack_from("<H", self.img, lin + 3)[0]
                    tgt = self.lin(seg, struct.unpack_from("<H", self.img, lin + 1)[0])
                    if tgt == loader:
                        for j in range(i - 1, max(i - 6, -1), -1):
                            mm = re.match(r"^push (?:byte \+|word )?0x([0-9a-f]+)$", code[j][2])
                            if mm:
                                v = int(mm.group(1), 16)
                                if 0 <= v < 300:
                                    f.images.add(v)
                                break


# ---------------------------------------------------------------- references

ADDR = r"([0-9A-F]{4}):([0-9A-F]{4})"
# a colon after the address is fine ("0A96:0433: type 9"), unless a hex digit follows (another seg:off)
NOT_AFTER = r"(?![0-9A-Za-z]|:[0-9A-Fa-f])"
HEAD = re.compile(r"(?<![0-9A-Za-z:])" + ADDR + NOT_AFTER)
# ranges: "SSSS:OOOO..OOOO", "SSSS:OOOO…OOOO", "SSSS:OOOO–OOOO" (dash without blanks: " – " separates text);
# the end is a bare offset or an address in the same segment
RANGE_TAIL = re.compile(r"(?:\s*(?:\.\.|…)\s*|–)(?:([0-9A-F]{4}):)?([0-9A-F]{4})" + NOT_AFTER)
LIST_NEXT = re.compile(r"\s*[/,]\s*(?:([0-9A-F]{4}):)?([0-9A-F]{4})" + NOT_AFTER)
MARKER = re.compile(r"\[(ORIGINAL|APPROX)\]")
OWN_LIST = re.compile(r"\[ORIGINAL\]\s+(?=[0-9A-F]{4}:[0-9A-F]{4})")


class RefCollector:
    def __init__(self, an: Analyzer, template_rule: bool = False):
        self.an = an
        self.hits = 0
        self.template_rule = template_rule

    def addresses(self, text: str) -> list[tuple[int, int, int]]:
        """(position, first, last) load-image offsets of the addresses in `text` (lists and ranges expanded)."""
        out = []
        pos = 0
        while True:
            m = HEAD.search(text, pos)
            if not m:
                break
            seg, off = int(m.group(1), 16), int(m.group(2), 16)
            pos = m.end()
            a = self.an.lin(seg, off)
            if a is None:
                continue
            r = RANGE_TAIL.match(text, pos)
            if r:
                seg2 = int(r.group(1), 16) if r.group(1) else seg
                b = self.an.lin(seg, int(r.group(2), 16)) if seg2 == seg else None
                if b is not None and a < b:
                    out.append((m.start(), a, b))
                    pos = r.end()
                    continue
            out.append((m.start(), a, a))
            cur = seg
            while True:
                n = LIST_NEXT.match(text, pos)
                if not n:
                    break
                if n.group(1):
                    cur = int(n.group(1), 16)
                b = self.an.lin(cur, int(n.group(2), 16))
                if b is None:
                    break
                out.append((n.start(2), b, b))
                pos = n.end()
        return out

    def mark(self, first: int, last: int, status: str, where: str, approx_line: str | None = None) -> None:
        an = self.an
        i = max(bisect.bisect_right(an.starts, first) - 1, 0)
        while i < len(an.functions) and an.functions[i].start <= last:
            f = an.functions[i]
            if f.end > first:
                if where not in f.refs[status]:
                    f.refs[status].append(where)
                    self.hits += 1
                if approx_line:
                    f.approx.add(approx_line)
            i += 1

    def own_list_positions(self, text: str) -> set[int]:
        """Positions of the addresses that form the list directly after an [ORIGINAL] tag."""
        pos = set()
        for m in OWN_LIST.finditer(text):
            p = m.end()
            h = HEAD.match(text, p)
            if not h:
                continue
            pos.add(h.start())
            q = h.end()
            r = RANGE_TAIL.match(text, q)
            if r:
                continue
            while True:
                n = LIST_NEXT.match(text, q)
                if not n:
                    break
                pos.add(n.start(2))
                q = n.end()
        return pos

    def scan_code(self, root: Path) -> None:
        for path in sorted((root / "src").rglob("*.cs")):
            rel = path.relative_to(root)
            if any(p in ("bin", "obj") for p in rel.parts):
                continue
            lines = path.read_text(encoding="utf-8", errors="replace").splitlines()
            blocks: list[list[tuple[int, str]]] = []
            cur: list[tuple[int, str]] = []
            for no, line in enumerate(lines, 1):
                s = line.strip()
                if s.startswith("//") or s.startswith("*") or s.startswith("/*") or "//" in s:
                    cur.append((no, line))
                else:
                    if cur:
                        blocks.append(cur)
                    cur = []
            if cur:
                blocks.append(cur)
            for block in blocks:
                text = "\n".join(l for _, l in block)
                markers = [(m.start(), m.group(1)) for m in MARKER.finditer(text)]
                own = self.own_list_positions(text)
                for pos, a, b in self.addresses(text):
                    prev = [(p, k) for p, k in markers if p <= pos]
                    nxt = [(p, k) for p, k in markers if p > pos]
                    mk = prev[-1] if prev else (nxt[0] if nxt else None)
                    line_no = block[0][0] + text[:pos].count("\n")
                    where = f"{rel.as_posix()}:{line_no}"
                    if mk is None:
                        self.mark(a, b, "dokumentiert", where)
                    elif mk[1] == "APPROX":
                        approx_line = f"{rel.as_posix()}:{block[0][0] + text[:mk[0]].count(chr(10))}"
                        self.mark(a, b, "genähert", where, approx_line)
                    elif pos in own or (self.template_rule and prev):
                        self.mark(a, b, "umgesetzt", where)
                    else:
                        self.mark(a, b, "dokumentiert", where)

    def scan_docs(self, root: Path) -> None:
        for path in sorted((root / "docs").rglob("*.md")):
            rel = path.relative_to(root).as_posix()
            if rel in NOT_EVIDENCE_DOCS or rel.startswith(NOT_EVIDENCE_DIRS):
                continue
            for no, line in enumerate(path.read_text(encoding="utf-8", errors="replace").splitlines(), 1):
                for _, a, b in self.addresses(line):
                    self.mark(a, b, "dokumentiert", f"{rel}:{no}")


# ---------------------------------------------------------------- report

def pct(part: int, whole: int) -> str:
    return f"{100 * part / whole:.0f} %" if whole else "–"


def hint(f: Function, an: Analyzer, with_texts: bool = True) -> str:
    bits = []
    if f.images:
        bits.append("Bild " + ",".join(str(i) for i in sorted(f.images)[:6]))
    if with_texts and f.strings:
        bits.append("„" + "“, „".join(s[:28] for s in f.strings[:3]) + "“")
    if not bits and f.callers:
        c = an.func_at(min(f.callers))
        if c:
            bits.append(f"gerufen von {c.address()}" + (f" (+{len(f.callers) - 1})" if len(f.callers) > 1 else ""))
    if not bits and f.globals_:
        bits.append("Globale " + ", ".join(f"DS:{g:04X}" for g in sorted(f.globals_)[:4]))
    return "; ".join(bits).replace("|", "/") or "–"


def render(an: Analyzer, rc: RefCollector) -> str:
    out = []
    w = out.append
    funcs = an.functions
    tot = sum(f.size for f in funcs)
    w(f"Erzeugt aus `T.EXE` ({len(an.exe)} Byte): {len(funcs)} Funktionen in {len(an.regions)} Segmenten, {tot} Byte Code; "
      f"{rc.hits} Zuordnungen von Fundstellen zu Funktionen" + (" (Vorlagenregel)." if rc.template_rule else "."))
    w("")
    w("| Status | Funktionen | Bytes | Anteil |")
    w("|---|---:|---:|---:|")
    for st in STATUS_ORDER:
        fs = [f for f in funcs if f.status == st]
        w(f"| {st} | {len(fs)} | {sum(f.size for f in fs)} | {pct(sum(f.size for f in fs), tot)} |")
    w(f"| **Summe** | {len(funcs)} | {tot} | 100 % |")
    w("")
    w("| Segment | Gruppe | Größe | Funktionen | " + " | ".join(STATUS_ORDER) + " |")
    w("|---|---|---:|---:|" + "---:|" * len(STATUS_ORDER))
    for r in an.regions:
        fs = [f for f in funcs if f.region is r]
        size = sum(f.size for f in fs)
        w(f"| `{r.name}` | {r.group} | 0x{r.size:X} | {len(fs)} | "
          + " | ".join(pct(sum(f.size for f in fs if f.status == st), size) for st in STATUS_ORDER) + " |")
    w("")
    w("Die 20 größten Funktionen „unbekannt“ oder „genähert“ (ohne Laufzeit `0000`):")
    w("")
    w("| Adresse | Datei | Größe | Status | Aufrufer | Hinweis |")
    w("|---|---|---:|---|---:|---|")
    gaps = sorted((f for f in funcs if f.status in ("unbekannt", "genähert") and f.region.group != "Laufzeit"),
                  key=lambda f: -f.size)[:20]
    for f in gaps:
        w(f"| `{f.address()}` | 0x{f.file_offset():05X} | {f.size} | {f.status} | {len(f.callers)} | {hint(f, an, False)} |")
    return "\n".join(out)


CSV_HEADER = ["adresse", "datei", "segment", "gruppe", "offset", "groesse", "status", "erkannt_durch", "aufrufer",
              "aufgerufene", "bilder", "texte", "globale", "approx", "fundstellen"]


def write_csv(an: Analyzer, path: Path) -> None:
    with path.open("w", newline="", encoding="utf-8") as fh:
        wr = csv.writer(fh)
        wr.writerow(CSV_HEADER)
        for f in an.functions:
            refs = "; ".join(f"{s}: {', '.join(v)}" for s, v in f.refs.items() if v)
            wr.writerow([f.address(), f"0x{f.file_offset():05X}", f.region.name, f.region.group, f"0x{f.offset:04X}",
                         f.size, f.status, "+".join(sorted(f.reasons)),
                         " ".join(an.func_at(c).address() for c in sorted(f.callers)),
                         " ".join(an.func_at(c).address() for c in sorted(f.callees)),
                         " ".join(str(i) for i in sorted(f.images)), " / ".join(f.strings[:8]),
                         " ".join(f"{g:04X}" for g in sorted(f.globals_)), " ".join(sorted(f.approx)), refs])


def main() -> int:
    ap = argparse.ArgumentParser(description=__doc__.split("\n")[0])
    ap.add_argument("exe", type=Path)
    ap.add_argument("--repo", type=Path, default=Path(__file__).resolve().parents[1])
    ap.add_argument("--csv", type=Path, help="write all functions as CSV (outside the repo)")
    ap.add_argument("--vorlagenregel", action="store_true",
                    help="count every address after an [ORIGINAL] as implemented (rule of the template)")
    args = ap.parse_args()
    an = Analyzer(args.exe.read_bytes())
    an.run()
    rc = RefCollector(an, args.vorlagenregel)
    rc.scan_code(args.repo)
    rc.scan_docs(args.repo)
    if args.csv:
        write_csv(an, args.csv)
    print(render(an, rc))
    return 0


if __name__ == "__main__":
    sys.exit(main())
