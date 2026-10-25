# Zwischenstände

Jeder Meilenstein-Stand wird hier mit Commit festgehalten. Git-Tags (`v0.x`) können
nachträglich auf den genannten Commit gesetzt werden.

## v0.2 – Ressourcen M1 (25.10.2026) · Commit `502f9a1`

- Schriften (groß 16×12, klein 16×5), Masken-Atlas, Musik-Kopf „A.H.“
- Ton-Treiberschnittstelle `int 64h` dokumentiert
- Asset-Viewer (MonoGame) und erweiterter PNG-Export
- Formatdoku `docs/RESSOURCENFORMATE.md`

## v0.1 – Fundament (25.10.2026) · Commit `a6cc3fe`

- Portierungsplan M0–M15, Erweiterungen E1–E7
- Originaldateien (DOS, Amiga) mit SHA-256-Prüfung in CI
- Ressourcen: PowerPacker PP20, VGA-Atlas + Palette, Texttabelle, PNG-Export
- Analyse: Segmentkarte (45 Module, ~890 Funktionen), Disassemblierung je Modul
- Kern: Original-Zufallsgenerator `21FD:01BA`, 16-Bit-Arithmetik-Helfer
