#!/usr/bin/env python3
"""Counts the markers [ORIGINAL] and [APPROX] per project under src/ and prints a Markdown table.

[ORIGINAL] = backed by T.EXE (with location segment:offset), [APPROX] = not backed (with reason), see AGENTS.md.
Usage: markerbericht.py [root folder]
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
