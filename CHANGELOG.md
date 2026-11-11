# Zwischenstände

## Runde 2 – Altbestand übernehmen (11.11.2026) · `main` `b55ce3a`

- ⓪ Bestandsaufnahme (PR #33, Fable): Karte aller 45 Segmente von `T.EXE`, Spiegel gegen Port und Altbranches, RefEmu-Proben P1–P6
  (`docs/analyse/BESTAND.md`); Nachzug in `DATENLAYOUT.md`, `STATUS.md`, Plan (PR #40); Entscheidungen: nur VGA, Zwischenziel
  Gesamtpartie ohne Roboterkampf, F3 = A (Zufall je Bild als Kern-Ereignis).
- ① RefEmu (PR #46): Tastendruck mit Haltezeit, FXDRIVER als TSR, Code-Haken, Aufrufspur, OPL-Spur; Wochenlauf als Orakel
  (`StartGame`, `RunToWeekStart`, `RunToMarker`) mit festen Zufallszuständen aus P1/P5.
- ② VGA-Treiber `23CD` (PR #43): Mode-X-Speicher, RAM-Atlas, 29 Treiberadressen mit Sammelbefunden und Differenztests.
- ③ Phasen (PR #44): `tools/callgraph.py` (Ausgabe nur lokal), Befunde `1777:000C`/`01F8`, Test der Markenfolge über zwei Wochen.
- Prozess: Zugriff der Läufe auf das Code-Repo (PR #32), `proben.sh` nach Übernahme (PR #45). 445 Tests grün mit Originaldaten.

## Runde 1 – Umstellung auf den Ablauf des Eishockey-Managers (11.11.2026) · `main` `1aead2b`

- ① Werkzeuge (PR #4): SDK-Release `claude-werkzeuge`, `tools/pruefen.sh`, schlanke `build.yml` statt `ci.yml`.
- ② Prozessdoku (PR #5): Rollen, Runden, Kurzaufrufe, Review-Kette, Merge-Prüfung; Labels.
- ③ Abdeckung (PR #6): `tools/abdeckung.py` erzeugt die Routinen-/Segmenttabellen in `STATUS.md`; Marker `[ORIGINAL]`,
  `[DIFFERENTIAL]`; Kommentare im Quelltext Englisch.
- ④ Referenz-Repo (PR #19, traders-referenz#1): Originale, Disassemblierung, RefEmu-Bilder und Videobilder nur noch im privaten
  Repo `kaaasi/traders-referenz` (`tools/claude/fetch-reference.sh`); Tests mit Originaldaten `[GameDataFact]`; drei Prüfläufe;
  CI mit Originaldaten über Secret `TRADERS_REFERENZ_TOKEN`; Git-Historie bereinigt.
- Workflow „Branches aufräumen“ (PR #18).
- Kein neuer Portierungsmeilenstein; 396 Tests grün mit Originaldaten (`tools/pruefen.sh`, CI).

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
