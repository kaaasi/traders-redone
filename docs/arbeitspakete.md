# Arbeitspakete

Die Portierung läuft in **Runden**; je Runde arbeiten mehrere **Spuren** parallel, jede auf einem Branch `paket/<spur>` mit
einem PR, eigenen Dateien und klarer Abnahme. Regeln: [AGENTS.md](../AGENTS.md), Ablauf:
[prozess/zusammenarbeit.md](prozess/zusammenarbeit.md). Den Status pflegt jede Spur nur in ihrem eigenen Abschnitt; Plan-
abschnitte ändert die Leitstelle. Abgeschlossene Runden stehen in [archiv/](archiv/) (Paketphase AP1–AP5: [archiv/runde-0.md](archiv/runde-0.md)).

## Aktuelle Runde: Runde 2 – Altbestand übernehmen (Plan 11.11.2026, **Go Nutzer offen**)

**Ziel:** Die drei Altbranches der Paketphase (Runde 0) nach den Regeln von Runde 1 auf `main` bringen: Originalcode-Marker,
Befunde in `docs/routines/`, `[DIFFERENTIAL]`-Tests, Tests mit Originaldaten nur über `[GameDataFact]`/`TestGameData.Dos`,
Kommentare Englisch, nichts aus den Originalen Abgeleitetes im Repo, `tools/abdeckung.py --check` grün. Ergebnis: M2 (RefEmu)
vollständig für das Spiel nach dem Hauptmenü, Grafiktreiber `23CD` portiert (Vorarbeit M3/M4), Phasenfolge belegt (Grundlage R3).

**Leitstelle R2:** Claude (Sitzung vom 11.11.2026). **Modell:** ⓪ Claude Fable (Nutzer 11.11.2026), ①–③ und alle Erstreviews
Claude Opus (RefEmu, Disassembly, Differenztests). **Quellen** (Stand nach der Historienbereinigung vom 11.11.2026; die SHAs der Vorschau sind damit überholt):
alle drei zweigen bei `b6d9057` ab, `main` ist 45 Commits weiter.

**Gemeinsame Vorgaben für alle drei Spuren:**

- Branch `paket/<spur>` von aktuellem `main`; den Altbestand per `git cherry-pick` übernehmen, nicht den Altbranch rebasen;
  Pfad-Checkout (`git checkout origin/dev/<ap> -- <pfad>`) nur für Dateien, die `main` seit `b6d9057` nicht geändert hat
  (`git diff --stat b6d9057 origin/main -- <pfad>` leer; R2P-2). `dev/*` bleibt unverändert, bis die Leitstelle ihn nach dem
  Merge löscht.
- `docs/uebergabe/APn.md` nicht übernehmen (Verzeichnis aufgelöst); Brauchbares in die PR-Beschreibung bzw. `docs/analyse/`.
  `ci/experiment.sh` wird `experiments/<spur>/<name>.sh` (`experiments/README.md`).
- Nicht übernommen wird, was gegen `AGENTS.md` §2.1 verstößt (Originaltexte, Disassemblierungsauszüge, Bildabzüge); fehlt der
  Spur dadurch eine Ablage im Referenz-Repo, im PR melden – ablegen nur Leitstelle/Nutzer.
- Altbestand ist **nicht** geprüft: jede übernommene Originalroutine braucht Befund + Differenztest, sonst `[APPROX]` oder raus.

### ⓪ `bestand` – Bestandsaufnahme `T.EXE` gegen den bisherigen Stand (Nutzer 11.11.2026, vor ② und ③)

- **Ziel:** unabhängiger Abgleich, bevor ungeprüfter Altbestand auf `main` kommt; reine Analyse, kein Port.
- **Teile:** (1) `T.EXE` kartieren: Segmente, Einsprünge, Rolle jedes Moduls, Datenbereiche der DGROUP, genutzte
  Ressourcenformate (Grundlage `tools/segmap.py`, `docs/analyse/segments.md`, `DATENLAYOUT.md`, `fetch-reference.sh disasm`).
  (2) Spiegeln gegen `main`, `dev/ap1-refemu`, `dev/ap2-vga`, `dev/ap5-phasen`, `docs/analyse/*`, `docs/routines/*`: je Bereich
  „portiert und belegt / vorhanden, unbelegt / fehlt“, Widersprüche zum Altbestand mit ausführbarem Beleg (`zusammenarbeit.md` §5
  Punkt 7). (3) Empfehlung zum Zuschnitt der Teile von ①–③ und der Runden R3 ff.
- **Ergebnis:** `docs/analyse/BESTAND.md` (Tabellen, Adressen, Befunde in eigenen Worten; keine Disassemblierungsauszüge, keine
  Originaltexte). Lange Listen nur lokal; braucht es eine Ablage, im PR melden (Leitstelle legt im Referenz-Repo ab).
- **Dateigrenzen:** `docs/analyse/BESTAND.md`; Korrekturen an `docs/analyse/segments.md`/`DATENLAYOUT.md` nur als Befund im PR.
- **Abnahme:** jeder Bereich von `T.EXE` erfasst (Segmentliste vollständig gegen `segmap.py`); jede Behauptung „fehlt“ mit
  erfolgloser Codesuche, jeder Widerspruch mit Probe; `pruefen.sh --schnell` grün. Erstreview (Opus) prüft Stichproben gegen `T.EXE`.
- **Umfang:** ein Lauf, höchstens 3 Teile (Fable ist teuer). **Abhängigkeiten:** keine. **Status:** offen.

### ① `refemu` – Referenz-Emulator nach dem Hauptmenü (Quelle `dev/ap1-refemu`, `a12822d`, 2 Commits)

- **Teile:** (1) Übernahme: Tastendruck mit Haltezeit, Scancode-Tabelle, FXDRIVER als TSR (`MzLoader`, `PcMachine.Dos`),
  Code-Haken in `PcMachine.RunFor`, `CallTracer` (Grafiktreiber), `OplTrace`; Tests nach `[GameDataFact]`, Kommentare Englisch.
  (2) Spiellauf nach dem Hauptmenü (`GameRunTests`, Hauptmenü-Hash mit FXDRIVER) und Experiment `experiments/refemu/`;
  `docs/analyse/REFEMU.md` ergänzen; P3 #31 (Überschrift `RefEmu/Data/README.md`) mitnehmen.
- **Dateigrenzen:** `src/Traders.RefEmu/**`, `tests/Traders.Tests/RefEmu/**`, `docs/analyse/REFEMU.md`, `experiments/refemu/`.
  Gemeinsame Datei `src/Traders.RefEmu/OriginalProgram.cs` gehört in R2 dieser Spur.
- **Abnahme:** `tools/pruefen.sh` alle drei Läufe grün; zwei Läufe bitgleich (Determinismus-Test bleibt grün); Hash des Hauptmenüs
  mit FXDRIVER als Test; Spiellauf erreicht nachweislich einen Zustand nach dem Hauptmenü (Test mit Prüfkriterium, nicht nur „läuft“).
- **Abhängigkeiten:** keine; läuft parallel zu ⓪. **Status:** offen.

### ② `grafik` – Mode X und Grafiktreiber `23CD` (Quelle `dev/ap2-vga`, `5a9f178`, 7 Commits)

- **Teile:** (1) `ModeXMemory` (4 Ebenen, Map-Mask, Read-Map, Latches) und RAM-Atlas; `docs/analyse/GRAFIK.md` (in eigenen
  Worten, ohne Disassemblierungsauszüge). (2) `ModeXDriver`: Treiberroutinen `23CD` mit `[ORIGINAL]`-Markern, je Routine Befund
  `docs/routines/23CD_<off>_<name>.md` und `[DIFFERENTIAL] 23CD:<off>` in `DriverDifferentialTests` (Testumgebung `DriverHarness`
  auf `TestGameData.Dos` umstellen). (3) Rest-Routinen und Grenzfälle (Clipping, Seitenwechsel), `tools/exeimg.py` nur, wenn
  `tools/segmap.py` es nicht abdeckt.
- **Dateigrenzen:** `src/Traders.Vga/**`, `tests/Traders.Tests/Vga/**`, `docs/analyse/GRAFIK.md`, `docs/routines/23CD_*`,
  `docs/routines/0A96_*`, `docs/routines/0A69_*`, ggf. `tools/exeimg.py`. Kein Eingriff in `src/Traders.RefEmu/`.
- **Abnahme:** alle drei Läufe grün; `abdeckung.py --check` grün mit jedem neuen `[ORIGINAL]`; je Differenztest Gegenprobe im
  Erstreview (Mutation → rot); VRAM-Vergleich gegen `VgaAdapter` des RefEmu, Erwartungswerte nie aus dem Port.
- **Abhängigkeiten:** Start nach Merge von ⓪; die Leitstelle passt die Teile an dessen Empfehlung an (nutzt den RefEmu-Stand von `main`). `0A96`/`0A69` (Befehlsliste, Texthilfen) nur Analyse, Port in R3/R4.
  **Status:** offen.

### ③ `phasen` – Aufrufgraph und Phasenfolge (Quelle `dev/ap5-phasen`, `8ed0d34`, 1 Commit)

- **Teile:** (1) `tools/callgraph.py` übernehmen; Ausgabe wird **erzeugt, nicht committet** (F1, entschieden). (2) `docs/analyse/PHASEN.md`:
  `main()`, Hauptschleife, Phasenfolge und Bedingungen mit Adressen, in eigenen Worten; Video-Zeitstempel je Phase (Videos über
  `fetch-reference.sh videos`). (3) Phasen-Spur im RefEmu als `experiments/phasen/` – braucht die Code-Haken aus ①; ist ① beim
  Start von Teil 3 nicht gemergt: Teil blockiert (`spur-wartet`, „wartet auf ①“).
- **Dateigrenzen:** `tools/callgraph.py`, `docs/analyse/PHASEN.md`, `experiments/phasen/`, `.gitignore` (nur Zeile für die
  Callgraph-Ausgabe).
- **Abnahme:** `pruefen.sh --schnell` grün; `callgraph.py` läuft reproduzierbar auf `T.EXE` (zweiter Lauf ohne Diff); jede Phase in
  `PHASEN.md` mit Adresse und mindestens einer Probe (RefEmu-Spur oder Videobild); kein Originaltext im Repo.
- **Abhängigkeiten:** Start nach Merge von ⓪ (Teile ggf. angepasst); Teil 3 nach Merge von ①. **Status:** offen.

### Zuständigkeit gemeinsamer Dateien in R2

`docs/analyse/BESTAND.md`: ⓪. `OriginalProgram.cs`: ①. `STATUS.md`: nur der erzeugte Block zwischen `abdeckung:anfang/ende`
per `tools/abdeckung.py --write` durch die Spur, die Marker hinzufügt (② und ggf. ①); bei Konflikt im Block nach Rebase `--write`
erneut ausführen (R2P-1). `.gitignore`: ③ (eine Zeile). Übriges `STATUS.md`, `CHANGELOG.md`, dieser Plan: Leitstelle. `GameState.cs`,
`TradersRedone.slnx`, `Directory.Build.props`, `global.json`: niemand (bei Bedarf im PR begründen, Leitstelle entscheidet).

### Integrationsfolge

⓪ und ① parallel starten; nach Merge von ⓪ passt die Leitstelle ② und ③ an (Plan-PR, Hinweis an den Nutzer nur bei geänderter
Abnahme) und startet beide. Merge ⓪ → ① → ② → ③ (① darf vor ⓪, wenn früher fertig). Spuren sind dateigetrennt; Konflikte nur
dann über `integration/runde-2`.

### Offene Fragen an den Nutzer

- **F1 Aufrufgraph-Ausgabe – entschieden (Nutzer 11.11.2026):** `docs/analyse/callgraph/` aus `dev/ap5-phasen` (erzeugt aus
  `T.EXE`, mit Originaltexten) kommt nicht ins Code-Repo; Spuren erzeugen sie lokal, die Leitstelle legt bei Bedarf einen Stand im
  Referenz-Repo unter `analyse/callgraph/` ab (PR `daten/callgraph`).
- **F2 Go** für Runde 2 mit ⓪ (Fable) und ①–③ (Opus) wie oben.

## Fahrplan

Nach Runde 2 (je Runde 2–4 Spuren): R3 Phasenmaschine + App-Host + Ton-Analyse · R4 Menü & Setup · R5 Anbau · R6 Produktion,
Fat Mike · R7 Handel · R8 Rakete & Piraten · R9 Ereignisse, Bilanz/Einkauf · R10 Roboterkampf · R11 Ende, Ton · R12 Gesamtpartie
(v1.0) · danach Erweiterungen ([PORTIERUNGSPLAN.md](PORTIERUNGSPLAN.md)).
