#!/usr/bin/env python3
"""Zählt die Marker [ORIGINAL] und [APPROX] je Projekt unter src/ und gibt eine Markdown-Tabelle aus.

[ORIGINAL] = aus T.EXE belegt (mit Fundstelle Segment:Offset), [APPROX] = nicht belegt (mit Begründung), siehe AGENTS.md.
Aufruf: markerbericht.py [wurzelordner]
"""
import pathlib
import sys

root = pathlib.Path(sys.argv[1] if len(sys.argv) > 1 else pathlib.Path(__file__).resolve().parents[1])
rows = []
for project in sorted(p for p in (root / "src").iterdir() if p.is_dir()):
    original = approx = 0
    for f in project.rglob("*.cs"):
        if "bin" in f.parts or "obj" in f.parts:
            continue
        text = f.read_text(encoding="utf-8", errors="replace")
        original += text.count("[ORIGINAL]")
        approx += text.count("[APPROX]")
    rows.append((project.name, original, approx))

print("### Markerbericht\n")
print("| Projekt | [ORIGINAL] | [APPROX] |")
print("|---|---:|---:|")
for name, o, a in rows:
    print(f"| {name} | {o} | {a} |")
print(f"| **Summe** | **{sum(r[1] for r in rows)}** | **{sum(r[2] for r in rows)}** |")
