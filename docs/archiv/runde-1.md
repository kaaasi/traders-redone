# Runde 1 – Umstellung auf den Ablauf des Eishockey-Managers (Go Nutzer 10.11.2026)

Grundlage: Umstellungsplan im claude.ai-Projekt „Traders Redone“ (`claude/UMSTELLUNGSPLAN.md`), Entscheidungen D1–D6 vom
10.11.2026: Originale bleiben im Repo (D1, **ersetzt 11.11.2026:** Originale und Videobilder ins private Repo
`kaaasi/traders-referenz`, damit gilt D2 Token-Secret für die CI), Rollen wie Eishockey (D3), Windows-Build nur per Hand (D4), Kommentare
Deutsch (D5, ersetzt 11.11.2026: Kommentare Englisch, `AGENTS.md` §1), R1 setzt die Leitstelle selbst um und eine frische Sitzung reviewt (D6).

**Leitstelle R1:** Claude (Sitzung vom 10.11.2026). Folge: ① → ② → ③; ① und ② sind dateigetrennt und dürfen parallel
reviewt werden, gemergt wird in dieser Folge.

## ① `prozess/werkzeuge` – Bauen und Testen in der Claude-Sitzung (PR #4)

- **Inhalt:** `werkzeuge.yml` (Release `claude-werkzeuge`), `tools/claude/`, `tools/pruefen.sh`, `tools/markerbericht.py`,
  `build.yml` statt `ci.yml`, ci-results-Mechanik entfernt, `global.json` 10.0.402.
- **Abnahme:** `tools/pruefen.sh` in einer frischen Claude-Sitzung grün (396 Tests), CI grün auf dem Head.
- **Status:** gemergt 11.11.2026 (`096dcbe`), Review R1① „Ja“ (#14); P3 #7–#9 mit PR #6 behoben.

**Review:** ein geplanter Lauf (Claude Opus, frische Sitzung) prüft #4, #5, #6 je mit eigener Datei in `docs/reviews/`.

## ② `prozess/regeln` – Prozessdoku

- **Inhalt:** `AGENTS.md`, `CLAUDE.md`, `docs/prozess/*`, `docs/vorlagen/*`, `docs/handoffs/VORLAGE.md`, PR-/Issue-Vorlage, diese
  Datei; `CONTRIBUTING.md`, `docs/UMGEBUNGEN.md`, `docs/ARBEITSPAKETE.md`, `docs/uebergabe/` aufgelöst (Archiv). Labels angelegt.
- **Abnahme:** keine Verweise auf entfernte Werkzeuge oder Dateien; Regeln des alten `AGENTS.md`/`CONTRIBUTING.md` sind
  übernommen oder bewusst ersetzt (Gegenliste im PR).
- **Status:** gemergt 11.11.2026 (`a138d26`), Review R1② „Ja“ (#15); P3 #10 mit PR #6 behoben.

## ③ `prozess/abdeckung` – Status-Übersicht per Skript (nach ①)

- **Inhalt:** `tools/abdeckung.py` (auf `tools/segmap.py`): Segmente und Einsprünge aus `T.EXE`, `[ORIGINAL] <seg:off>`-Marker im
  Code, Befunde in `docs/routines/`, Differenztests → erzeugte Tabellen in `STATUS.md` zwischen Markierungskommentaren;
  `--check` in `tools/pruefen.sh`. Vorhandene Routinen (`21FD:01BA`, `2336:000A/0539`) mit Markern versehen; `Int16Math` ist
  keine `T.EXE`-Routine (IDIV-Semantik) und verweist ohne Adressmarker auf `docs/routines/Int16Math_IDIV.md`.
- **Abnahme:** zweiter Lauf ohne Diff; `--check` wird rot, wenn ein Marker ohne Befund, ein portierter Befund ohne
  `[DIFFERENTIAL]`-Test oder ein Marker ohne gültige Adresse auftaucht.
- **Status:** gemergt 11.11.2026 (`18e6502`) nach Review R1③ „Nein“ (#16), Fixlauf und Delta-Prüfung der Leitstelle (nur
  P2/P3); Issues #7–#13 geschlossen.

## ④ `prozess/referenz` – Originale ins Referenz-Repo (Nutzer 11.11.2026)

- **Inhalt:** Referenz-Repo `kaaasi/traders-referenz` (PR traders-referenz#1: `original/`, `analyse/disasm/`, `refemu/`,
  `videos/`); hier entfernt: `original/`, `reference/`, `docs/analyse/disasm/`, RefEmu-PNGs. Neu: `tools/claude/fetch-reference.sh`,
  Prüfsummen unter `tools/originale/`, `[GameDataFact]`/`[GameDataTheory]` (`tests/Traders.Tests/Support/TestGameData.cs`),
  `tools/pruefen.sh` mit drei Läufen, CI holt die Daten über Secret `TRADERS_REFERENZ_TOKEN`, Doku (AGENTS §2.1, §3, §5,
  CLAUDE, Vorlagen, Anhang).
- **Abnahme:** mit Daten 396 Tests grün; ohne Daten (versteckt) alle übrigen grün, Daten-Tests sichtbar übersprungen; Pflichtlauf
  ohne Daten scheitert; keine Originaldatei mehr im Baum.
- **Nutzer:** Secret `TRADERS_REFERENZ_TOKEN` anlegen (fein granuliertes Token, nur `kaaasi/traders-referenz`, Contents: Read);
  traders-referenz#1 mergen. In der Git-Historie dieses Repos bleiben die Dateien (privat; Umschreiben nur auf Wunsch).
- **Status:** gemergt 11.11.2026 (`1aead2b`) nach Review R1④ „Nein“ (#24, nur R19-1 P2), Fixlauf und Delta-Prüfung der
  Leitstelle; Issues #20–#23 geschlossen. traders-referenz#1 gemergt, Secret angelegt (CI mit Originaldaten). Git-Historie am
  11.11.2026 bereinigt (Nutzerauftrag); alte PR-Refs hält nur noch GitHub (Support-Anfrage optional, Nutzer).

## Zuständigkeit gemeinsamer Dateien in R1

`.github/workflows/*`, `tools/*`, `global.json`: ①. Doku-Dateien außer `STATUS.md`: ②. `STATUS.md` und Marker in `src/`: ③.

## Danach (Leitstelle nach dem Merge von ①–③)

Alte Branches aufgeräumt (11.11.2026, Nutzer per Hand bzw. Workflow „Branches aufräumen“, PR #18); `dev/ap1-refemu`, `dev/ap2-vga`, `dev/ap5-phasen` bleiben bis zur Übernahme in Runde 2.

## Abschluss (11.11.2026)

- Gesamtreview ChatGPT: [reviews/2026-11-11-r1-gesamt.md](../reviews/2026-11-11-r1-gesamt.md) – „Ja“, kein P0–P2; P3 #26 mit dem
  Abschluss-PR behoben. Status-PR #17 gemergt (`c4ea5d8`).
- Reviews der Runde: `reviews/2026-11-10-r1-werkzeuge.md`, `…-r1-regeln.md`, `…-r1-abdeckung.md`, `reviews/2026-11-11-r1-referenz.md`.
- Offen für später (Nutzer): Anfrage beim GitHub-Support, die alten PR-Refs (`refs/pull/*`, enthalten noch Originaldateien) zu
  entfernen.

