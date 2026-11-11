# Arbeitspakete

Die Portierung läuft in **Runden**; je Runde arbeiten mehrere **Spuren** parallel, jede auf einem Branch `paket/<spur>` mit
einem PR, eigenen Dateien und klarer Abnahme. Regeln: [AGENTS.md](../AGENTS.md), Ablauf:
[prozess/zusammenarbeit.md](prozess/zusammenarbeit.md). Den Status pflegt jede Spur nur in ihrem eigenen Abschnitt; Plan-
abschnitte ändert die Leitstelle. Abgeschlossene Runden stehen in [archiv/](archiv/) (Paketphase AP1–AP5: [archiv/runde-0.md](archiv/runde-0.md)).

## Aktuelle Runde: Runde 1 – Umstellung auf den Ablauf des Eishockey-Managers (Go Nutzer 10.11.2026)

Grundlage: Umstellungsplan im claude.ai-Projekt „Traders Redone“ (`claude/UMSTELLUNGSPLAN.md`), Entscheidungen D1–D6 vom
10.11.2026: Originale bleiben im Repo (D1, D2 entfällt), Rollen wie Eishockey (D3), Windows-Build nur per Hand (D4), Kommentare
Deutsch (D5, ersetzt 11.11.2026: Kommentare Englisch, `AGENTS.md` §1), R1 setzt die Leitstelle selbst um und eine frische Sitzung reviewt (D6).

**Leitstelle R1:** Claude (Sitzung vom 10.11.2026). Folge: ① → ② → ③; ① und ② sind dateigetrennt und dürfen parallel
reviewt werden, gemergt wird in dieser Folge.

### ① `prozess/werkzeuge` – Bauen und Testen in der Claude-Sitzung (PR #4)

- **Inhalt:** `werkzeuge.yml` (Release `claude-werkzeuge`), `tools/claude/`, `tools/pruefen.sh`, `tools/markerbericht.py`,
  `build.yml` statt `ci.yml`, ci-results-Mechanik entfernt, `global.json` 10.0.402.
- **Abnahme:** `tools/pruefen.sh` in einer frischen Claude-Sitzung grün (396 Tests), CI grün auf dem Head.
- **Status:** gemergt 11.11.2026 (`096dcbe`), Review R1① „Ja“ (#14); P3 #7–#9 mit PR #6 behoben.

**Review:** ein geplanter Lauf (Claude Opus, frische Sitzung) prüft #4, #5, #6 je mit eigener Datei in `docs/reviews/`.

### ② `prozess/regeln` – Prozessdoku

- **Inhalt:** `AGENTS.md`, `CLAUDE.md`, `docs/prozess/*`, `docs/vorlagen/*`, `docs/handoffs/VORLAGE.md`, PR-/Issue-Vorlage, diese
  Datei; `CONTRIBUTING.md`, `docs/UMGEBUNGEN.md`, `docs/ARBEITSPAKETE.md`, `docs/uebergabe/` aufgelöst (Archiv). Labels angelegt.
- **Abnahme:** keine Verweise auf entfernte Werkzeuge oder Dateien; Regeln des alten `AGENTS.md`/`CONTRIBUTING.md` sind
  übernommen oder bewusst ersetzt (Gegenliste im PR).
- **Status:** gemergt 11.11.2026 (`a138d26`), Review R1② „Ja“ (#15); P3 #10 mit PR #6 behoben.

### ③ `prozess/abdeckung` – Status-Übersicht per Skript (nach ①)

- **Inhalt:** `tools/abdeckung.py` (auf `tools/segmap.py`): Segmente und Einsprünge aus `T.EXE`, `[ORIGINAL] <seg:off>`-Marker im
  Code, Befunde in `docs/routines/`, Differenztests → erzeugte Tabellen in `STATUS.md` zwischen Markierungskommentaren;
  `--check` in `tools/pruefen.sh`. Vorhandene Routinen (`21FD:01BA`, `2336:000A/0539`) mit Markern versehen; `Int16Math` ist
  keine `T.EXE`-Routine (IDIV-Semantik) und verweist ohne Adressmarker auf `docs/routines/Int16Math_IDIV.md`.
- **Abnahme:** zweiter Lauf ohne Diff; `--check` wird rot, wenn ein Marker ohne Befund, ein portierter Befund ohne
  `[DIFFERENTIAL]`-Test oder ein Marker ohne gültige Adresse auftaucht.
- **Status:** gemergt 11.11.2026 (`18e6502`) nach Review R1③ „Nein“ (#16), Fixlauf und Delta-Prüfung der Leitstelle (nur
  P2/P3); Issues #7–#13 geschlossen.

### Zuständigkeit gemeinsamer Dateien in R1

`.github/workflows/*`, `tools/*`, `global.json`: ①. Doku-Dateien außer `STATUS.md`: ②. `STATUS.md` und Marker in `src/`: ③.

### Danach (Leitstelle nach dem Merge von ①–③)

Alte Branches aufgeräumt (11.11.2026): `ci-results`, `dev/ci-probe`, `dev/integration-*`, `dev/repo-*`, `dev/ap3-state` und die
gemergten `prozess/*`/`review/*` gelöscht; `dev/ap1-refemu`, `dev/ap2-vga`, `dev/ap5-phasen` bleiben bis zur Übernahme in Runde 2.
Offen: Gesamtreview Runde 1 (ChatGPT), danach Archiv `docs/archiv/runde-1.md` und Plan Runde 2.

## Vorschau: Runde 2 – Altbestand übernehmen (Plan folgt, Go nötig)

| Spur | Quelle | Inhalt |
|---|---|---|
| `refemu` | `dev/ap1-refemu` (`2f03e46`) | Tastatur mit Haltezeit, FXDRIVER als TSR, Code-Haken, Aufrufspur Grafiktreiber, OPL-Spur; auf neuen `main`, Übergabe nachholen |
| `grafik` | `dev/ap2-vga` (`3fc6033`) | Mode-X-VRAM, RAM-Atlas, Treiberroutinen `23CD`, Grafikhilfen `0A96`/`0A69`, Differenztests |
| `phasen` | `dev/ap5-phasen` (`92edea9`) | Aufrufgraph, `main()`, Hauptschleife, `docs/analyse/PHASEN.md` (Phasenfolge, Bedingungen, Video-Zeitstempel) |

Fahrplan danach (je Runde 2–4 Spuren): R3 Phasenmaschine + App-Host + Ton-Analyse · R4 Menü & Setup · R5 Anbau · R6 Produktion,
Fat Mike · R7 Handel · R8 Rakete & Piraten · R9 Ereignisse, Bilanz/Einkauf · R10 Roboterkampf · R11 Ende, Ton · R12 Gesamtpartie
(v1.0) · danach Erweiterungen ([PORTIERUNGSPLAN.md](PORTIERUNGSPLAN.md)).
