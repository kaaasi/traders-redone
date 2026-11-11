# Zwischenstände

## Runde 1 – Umstellung auf den Ablauf des Eishockey-Managers (11.11.2026) · `main` `18e6502`

- ① Werkzeuge (PR #4): SDK-Release `claude-werkzeuge`, `tools/pruefen.sh`, schlanke `build.yml` statt `ci.yml`.
- ② Prozessdoku (PR #5): Rollen, Runden, Kurzaufrufe, Review-Kette, Merge-Prüfung; Labels.
- ③ Abdeckung (PR #6): `tools/abdeckung.py` erzeugt die Routinen-/Segmenttabellen in `STATUS.md`; Marker `[ORIGINAL]`,
  `[DIFFERENTIAL]`; Kommentare im Quelltext Englisch.
- Kein neuer Portierungsmeilenstein; 396 Tests grün (`tools/pruefen.sh`, CI).

## Repositorymodernisierung (27.10.2026) · Integration `76df32b`

- Anbieterunabhängige AGENTS.md, Übergaben und Worktree-/PR-Ablauf.
- Integrationsprüfung mit JSON-Nachweisen und Fehlerfalltests.
- Lesende Build-CI, separater vertrauenswürdiger Publisher, manuelle Diagnosen.
- IDIV-Grenzfall korrigiert und gegen RefEmu abgesichert; gemeinsame Testpfade.
- Kein neuer Portierungsmeilenstein; Paketstände bleiben separat zu integrieren.


Jeder Meilenstein-Stand wird hier mit Commit festgehalten (keine Tags).

## v0.3 – Integration AP1 + AP3 (25.10.2026) · Commit `2c02a38`

- AP1: Referenz-Emulator (286, DOS/BIOS/VGA), T.EXE läuft bis ins Hauptmenü, `random()` gegen Original geprüft ✅
- AP3: Datenlayout (52 Spielstandfelder), C#-Spielzustand, Spielstand lesen/schreiben
- Leitstelle: VGA-Bildkopf korrigiert (Pixel ab Byte 772), Emulator-Startlauf in jedem CI-Lauf
- 388 Tests grün (Linux + Windows)

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


## AP3 Stand 2 – Savegame-Differenztests

- Stand `f45f778` mit bewahrter Branch-Historie integriert.
- Original-Speichern/Laden, Speicher-Nebenwirkungen und Kurzdatei-Fehlerfall gegen RefEmu geprüft.
