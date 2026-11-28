#!/usr/bin/env python3
"""Marker list of all [APPROX] lines in src/ with stable IDs and hand-kept classes (rest list).

Adopted from kaaasi/eishockey-manager-redone (tools/reverse-engineering/markerliste.py) and adapted to this repo:
paths are relative to src/ (e.g. Traders.Core/Trade/TradeRules.cs), there is no seeding from an older inventory and
no function rows (F-IDs); IDs start at A001.

Usage:
    python3 tools/markerliste.py [--repo .] [--write] [--abweichungen] [--csv-ids]

Without --write the generated Markdown block is printed to stdout. With --write the block between the markers in
docs/analyse/restliste.md is replaced (hand-written text outside the markers stays). --abweichungen prints the
deviation list (every entry except class X, by area) instead of the block. --csv-ids prints "ID,path:line,key" for
every marker line (helper for filling in the class file).

Exit code: 0 = every marker line has exactly one valid class, 1 = unclassified lines or invalid classes (listed on
stderr).

IDs and matching:
 - Every line under src/ (bin/ and obj/ skipped) that contains "[APPROX]" is one entry, exactly like
   `grep -rc '\\[APPROX\\]' src/`.
 - The key of a line is its file and its wording: SHA-1 of the normalised text (comment tokens and surrounding
   whitespace removed, inner whitespace collapsed), first 10 hex digits. Line numbers are never part of the key, so a
   moved line keeps its ID; a reworded line gets a new ID and the old ID is listed as "entfallen".
 - Memory: the previous generated block in restliste.md (ID, file, key). Lines are paired with previous rows of the same
   file and key in line order; then with a unique key in another file (file moved).
 - Unmatched lines get new IDs (max known ID + 1), in order of file and line.

Classification is hand-maintained in docs/analyse/restliste-klassen.csv (key: ID; columns ID, Klasse, Bereich,
Funktion/Bildschirm, Abweichung, Beleg, Verdacht, Aufwand). Classes: V, D, E, K, X (see CLASSES). tools/funktionskarte.py
reads the class of the [APPROX] a finding belongs to (colours of the coverage overview).
"""

from __future__ import annotations

import argparse
import csv
import hashlib
import re
import sys
from collections import defaultdict
from dataclasses import dataclass
from pathlib import Path

DOC = Path("docs/analyse/restliste.md")
CSV_FILE = Path("docs/analyse/restliste-klassen.csv")
CSV_COLUMNS = ["ID", "Klasse", "Bereich", "Funktion/Bildschirm", "Abweichung", "Beleg", "Verdacht", "Aufwand"]
CLASSES = {
    "V": "v1.0-Blocker",
    "D": "nur DOSBox/Video",
    "E": "bewusst",
    "K": "kosmetisch",
    "X": "Marker veraltet",
}
AUFWAND = {"", "S", "M", "L"}
FIRST_NEW_ID = 1
BEGIN = "<!-- BEGIN generiert: tools/markerliste.py – nicht von Hand ändern -->"
END = "<!-- END generiert -->"
MARK = "[APPROX]"


def normalise(line: str) -> str:
    s = line.strip()
    s = re.sub(r"^(///|//|/\*\*|/\*|\*/|\*|#)\s?", "", s)
    s = re.sub(r"\s*\*/$", "", s)
    return re.sub(r"\s+", " ", s).strip()


def key_of(line: str) -> str:
    return hashlib.sha1(normalise(line).encode("utf-8")).hexdigest()[:10]


def id_num(ident: str) -> int:
    return int(ident[1:])


@dataclass
class Marker:
    path: str  # relative to src/, e.g. Traders.Core/Trade/TradeRules.cs
    line: int
    text: str
    key: str
    ident: str = ""
    origin: str = ""


@dataclass
class Gone:
    ident: str
    last: str  # last known place
    key: str
    note: str


@dataclass
class PrevRow:
    ident: str
    path: str
    key: str
    origin: str = ""
    stelle: str = ""


def scan(repo: Path) -> list[Marker]:
    src = repo / "src"
    out: list[Marker] = []
    for p in sorted(src.rglob("*"), key=lambda q: q.as_posix()):
        if not p.is_file():
            continue
        rel = p.relative_to(src)
        if any(part in ("bin", "obj") for part in rel.parts[:-1]):
            continue
        data = p.read_bytes()
        if MARK.encode() not in data:
            continue
        text = data.decode("utf-8", errors="replace")
        for i, line in enumerate(text.split("\n"), 1):
            if MARK in line:
                out.append(Marker(rel.as_posix(), i, line, key_of(line)))
    return out


def md_cell(s: str, limit: int = 0) -> str:
    s = re.sub(r"\s+", " ", s).strip()
    if limit and len(s) > limit:
        s = s[: limit - 1].rstrip() + "…"
    return s.replace("\\", "\\\\").replace("|", "\\|").replace("<", "&lt;").replace(">", "&gt;")


def md_uncell(s: str) -> str:
    return s.strip().replace("&gt;", ">").replace("&lt;", "<").replace("\\|", "|").replace("\\\\", "\\")


# ---------------------------------------------------------------- previous block

def split_row(line: str) -> list[str]:
    cells, cur, i = [], "", 1
    body = line.strip()
    while i < len(body):
        c = body[i]
        if c == "\\" and i + 1 < len(body):
            cur += body[i: i + 2]
            i += 2
            continue
        if c == "|":
            cells.append(cur)
            cur = ""
        else:
            cur += c
        i += 1
    return [md_uncell(x) for x in cells]


def read_previous(doc: Path) -> tuple[list[PrevRow], list[Gone]]:
    if not doc.exists():
        return [], []
    text = doc.read_text(encoding="utf-8")
    m = re.search(re.escape(BEGIN) + r"(.*?)" + re.escape(END), text, flags=re.S)
    if not m:
        return [], []
    rows: list[PrevRow] = []
    gone: list[Gone] = []
    section = ""
    for line in m.group(1).split("\n"):
        if line.startswith("### "):
            section = line
            continue
        if not line.startswith("| A"):
            continue
        c = split_row(line)
        if section.startswith("### Marker"):
            # | ID | Stelle | Schlüssel | Wortlaut | Herkunft | ...
            stelle = c[1].strip("`")
            rows.append(PrevRow(c[0], stelle.rsplit(":", 1)[0], c[2].strip("`"), c[4], stelle))
        elif section.startswith("### Entfallen"):
            # | ID | zuletzt | Schlüssel | Hinweis | Klasse |
            gone.append(Gone(c[0], c[1].strip("`"), c[2].strip("`"), c[3]))
    return rows, gone


# ---------------------------------------------------------------- matching

def pair(markers: list[Marker], candidates: list[tuple[str, str, str]], assign) -> list[tuple[str, str, str]]:
    """candidates: (ident, path, key) in preferred order. Pairs same file + key in line order, then a key
    that is unique among the remaining markers and candidates (moved file). Returns the unpaired ones."""
    free: dict[tuple[str, str], list[Marker]] = defaultdict(list)
    for m in markers:
        if not m.ident:
            free[(m.path, m.key)].append(m)
    rest = []
    for c in candidates:
        lst = free.get((c[1], c[2]))
        if lst:
            assign(lst.pop(0), c)
        else:
            rest.append(c)
    by_key: dict[str, list[Marker]] = defaultdict(list)
    for m in markers:
        if not m.ident:
            by_key[m.key].append(m)
    cand_keys: dict[str, int] = defaultdict(int)
    for c in rest:
        cand_keys[c[2]] += 1
    still = []
    for c in rest:
        lst = by_key.get(c[2])
        if lst and len(lst) == 1 and cand_keys[c[2]] == 1:
            assign(lst.pop(), c)
        else:
            still.append(c)
    return still


def build(repo: Path):
    markers = scan(repo)
    prev_rows, prev_gone = read_previous(repo / DOC)
    known = {r.ident for r in prev_rows} | {g.ident for g in prev_gone}
    prev_by_id = {r.ident: r for r in prev_rows}

    def assign_prev(m: Marker, c):
        r = prev_by_id[c[0]]
        m.ident, m.origin = r.ident, r.origin

    # 1) previous block (active rows first, then IDs that were gone and whose wording came back)
    unpaired_prev = pair(markers, [(r.ident, r.path, r.key) for r in prev_rows], assign_prev)
    gone_by_id = {g.ident: g for g in prev_gone}

    def assign_gone(m: Marker, c):
        g = gone_by_id.pop(c[0])
        m.ident, m.origin = g.ident, "wieder da"

    pair(markers, [(g.ident, re.match(r"[^\s:]*", g.last.strip("`")).group(0), g.key) for g in prev_gone if g.key],
         assign_gone)
    gone: list[Gone] = list(gone_by_id.values())
    for c in unpaired_prev:
        r = prev_by_id[c[0]]
        gone.append(Gone(r.ident, f"{r.stelle} (zuletzt im Block)", r.key, "Wortlaut geändert oder entfernt"))

    # 2) new IDs
    all_ids = [id_num(x) for x in known] + [id_num(m.ident) for m in markers if m.ident]
    nxt = max([FIRST_NEW_ID - 1] + all_ids) + 1
    for m in markers:
        if m.ident:
            continue
        m.ident = f"A{nxt:03d}"
        m.origin = "neu"
        nxt += 1
    gone.sort(key=lambda g: id_num(g.ident))
    return markers, gone


# ---------------------------------------------------------------- classes

def read_classes(repo: Path) -> dict[str, dict[str, str]]:
    p = repo / CSV_FILE
    if not p.exists():
        return {}
    rows = {}
    with p.open(encoding="utf-8", newline="") as f:
        for row in csv.DictReader(f):
            ident = (row.get("ID") or "").strip()
            if ident:
                rows[ident] = {k: (row.get(k) or "").strip() for k in CSV_COLUMNS}
    return rows


def marker_classes(repo: Path) -> dict[tuple[str, int], str]:
    """(path relative to the repo, line) -> class of that [APPROX] line; used by tools/funktionskarte.py."""
    markers, _ = build(repo)
    classes = read_classes(repo)
    return {(f"src/{m.path}", m.line): classes.get(m.ident, {}).get("Klasse", "") for m in markers}


def render(markers: list[Marker], gone: list[Gone], classes: dict, problems: list[str]) -> str:
    out = []
    w = out.append
    files = len({m.path for m in markers})
    counts: dict[str, int] = defaultdict(int)
    for m in markers:
        k = classes.get(m.ident, {}).get("Klasse", "")
        counts[k if k in CLASSES else ("?" if k else "")] += 1
    w(BEGIN)
    w("")
    w(f"**Zählung:** {files} Dateien, {len(markers)} Markerzeilen (= `grep -rc '\\[APPROX\\]' src/ | "
      f"awk -F: '$2>0{{n++;s+=$2}}END{{print n,s}}'`); {len(gone)} IDs entfallen.")
    w("")
    w("| Klasse | Bedeutung | Stellen |")
    w("|---|---|---:|")
    for k, v in CLASSES.items():
        w(f"| {k} | {v} | {counts[k]} |")
    w(f"| – | unklassifiziert | {counts['']} |")
    if counts["?"]:
        w(f"| ? | ungültige Klasse | {counts['?']} |")
    w("")
    w("### Marker")
    w("")
    w("Stelle relativ zu `src/`; Schlüssel = Datei + Wortlaut (`tools/markerliste.py`, Docstring). Herkunft: „neu“ bei "
      "der ersten Vergabe, „wieder da“ für eine zurückgekehrte ID.")
    w("")
    w("| ID | Stelle | Schlüssel | Wortlaut | Herkunft | " + " | ".join(CSV_COLUMNS[1:]) + " |")
    w("|" + "---|" * (5 + len(CSV_COLUMNS) - 1))
    for m in sorted(markers, key=lambda x: (x.path, x.line)):
        c = classes.get(m.ident, {})
        cells = [m.ident, f"`{m.path}:{m.line}`", f"`{m.key}`", md_cell(normalise(m.text), 110),
                 md_cell(m.origin)] + [md_cell(c.get(k, "")) for k in CSV_COLUMNS[1:]]
        w("| " + " | ".join(cells) + " |")
    w("")
    w("### Entfallen")
    w("")
    w("IDs ohne Markerzeile (aus dem vorigen Block).")
    w("")
    w("| ID | zuletzt | Schlüssel | Hinweis | Klasse |")
    w("|---|---|---|---|---|")
    for g in gone:
        w(f"| {g.ident} | `{md_cell(g.last)}` | {f'`{g.key}`' if g.key else ''} | {md_cell(g.note, 120)} | "
          f"{md_cell(classes.get(g.ident, {}).get('Klasse', ''))} |")
    if problems:
        w("")
        w("### Meldungen")
        w("")
        for p in problems:
            w(f"- {md_cell(p)}")
    w("")
    w(END)
    return "\n".join(out)


def render_deviations(markers: list[Marker], classes: dict) -> str:
    """Every entry except class X, by area."""
    by_area: dict[str, list[tuple[str, str, str, str]]] = defaultdict(list)
    for m in markers:
        c = classes.get(m.ident, {})
        k = c.get("Klasse", "")
        if k in ("", "X"):
            continue
        by_area[c.get("Bereich") or "(ohne Bereich)"].append(
            (c.get("Funktion/Bildschirm") or m.path, c.get("Abweichung", ""), k, m.ident))
    out = ["# Abweichungen vom Original", ""]
    for area in sorted(by_area):
        out.append(f"## {area}")
        out.append("")
        out.append("| Funktion bzw. Bildschirm | Abweichung | Klasse | ID |")
        out.append("|---|---|---|---|")
        for fn, dev, k, ident in sorted(by_area[area], key=lambda t: ("VDEK".find(t[2]), t[0], id_num(t[3]))):
            out.append(f"| {md_cell(fn)} | {md_cell(dev)} | {k} | {ident} |")
        out.append("")
    return "\n".join(out)


def main() -> int:
    ap = argparse.ArgumentParser(description=__doc__.split("\n")[0])
    ap.add_argument("--repo", type=Path, default=Path(__file__).resolve().parents[1])
    ap.add_argument("--write", action="store_true", help=f"update {DOC} between the markers")
    ap.add_argument("--abweichungen", action="store_true", help="print the deviation list instead")
    ap.add_argument("--csv-ids", action="store_true", help="print ID, place and key of every marker line")
    args = ap.parse_args()
    repo = args.repo.resolve()
    markers, gone = build(repo)
    classes = read_classes(repo)
    if args.csv_ids:
        for m in sorted(markers, key=lambda x: (x.path, x.line)):
            print(f"{m.ident},{m.path}:{m.line},{m.key}")
        return 0
    active = {m.ident for m in markers}
    problems = []
    unclassified = [m for m in markers if not classes.get(m.ident, {}).get("Klasse")]
    for i, c in sorted(classes.items(), key=lambda t: id_num(t[0])):
        if c["Klasse"] and c["Klasse"] not in CLASSES:
            problems.append(f"{i}: ungültige Klasse „{c['Klasse']}“")
        if c["Aufwand"] not in AUFWAND:
            problems.append(f"{i}: ungültiger Aufwand „{c['Aufwand']}“")
        if i not in active:
            problems.append(f"{i}: Klasse ohne Marker (entfallen)")
    if args.abweichungen:
        print(render_deviations(markers, classes))
    else:
        text = render(markers, gone, classes, problems)
        if args.write:
            doc = repo / DOC
            old = doc.read_text(encoding="utf-8") if doc.exists() else ""
            if BEGIN in old:
                new = re.sub(re.escape(BEGIN) + r".*?" + re.escape(END), lambda _: text, old, flags=re.S)
            else:
                new = old.rstrip() + ("\n\n" if old.strip() else "") + text + "\n"
            doc.write_text(new, encoding="utf-8")
        else:
            print(text)
    for p in problems:
        print(f"markerliste: {p}", file=sys.stderr)
    if unclassified:
        print(f"markerliste: {len(unclassified)} unklassifiziert (erste: "
              f"{', '.join(m.ident for m in unclassified[:10])})", file=sys.stderr)
    bad = [p for p in problems if "ungültig" in p]
    return 1 if unclassified or bad else 0


if __name__ == "__main__":
    sys.exit(main())
