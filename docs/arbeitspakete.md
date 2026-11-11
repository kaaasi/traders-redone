# Arbeitspakete

Die Portierung läuft in **Runden**; je Runde arbeiten mehrere **Spuren** parallel, jede auf einem Branch `paket/<spur>` mit
einem PR, eigenen Dateien und klarer Abnahme. Regeln: [AGENTS.md](../AGENTS.md), Ablauf:
[prozess/zusammenarbeit.md](prozess/zusammenarbeit.md). Den Status pflegt jede Spur nur in ihrem eigenen Abschnitt; Plan-
abschnitte ändert die Leitstelle. Abgeschlossene Runden stehen in [archiv/](archiv/) (Paketphase AP1–AP5: [archiv/runde-0.md](archiv/runde-0.md)).

## Aktuelle Runde: Runde 2 – Altbestand übernehmen (Plan 11.11.2026, Go Nutzer 11.11.2026)

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
- **Umfang:** ein Lauf, höchstens 3 Teile (Fable ist teuer). **Abhängigkeiten:** keine. **Status:** gemergt 11.11.2026 (`5d71fb7`), Review „Ja“ (#38); P3 R1–R3 als #39. B1–B7 und die Entscheidungen §3.3 zieht
  die Leitstelle nach (PR `prozess/bestand-nachzug`), B8 → F3.

### ① `refemu` – Referenz-Emulator nach dem Hauptmenü (Quelle `dev/ap1-refemu`, `a12822d`, 2 Commits)

- **Teile:** (1) Übernahme: Tastendruck mit Haltezeit, Scancode-Tabelle, FXDRIVER als TSR (`MzLoader`, `PcMachine.Dos`),
  Code-Haken in `PcMachine.RunFor`, `CallTracer` (Grafiktreiber), `OplTrace`; Tests nach `[GameDataFact]`, Kommentare Englisch.
  (2) Spiellauf nach dem Hauptmenü (`GameRunTests`, Hauptmenü-Hash mit FXDRIVER) und Experiment `experiments/refemu/`;
  `docs/analyse/REFEMU.md` ergänzen; P3 #31 (Überschrift `RefEmu/Data/README.md`) mitnehmen.
- **Dateigrenzen:** `src/Traders.RefEmu/**`, `tests/Traders.Tests/RefEmu/**`, `docs/analyse/REFEMU.md`, `experiments/refemu/`.
  Gemeinsame Datei `src/Traders.RefEmu/OriginalProgram.cs` gehört in R2 dieser Spur.
- **Abnahme:** `tools/pruefen.sh` alle drei Läufe grün; zwei Läufe bitgleich (Determinismus-Test bleibt grün); Hash des Hauptmenüs
  mit FXDRIVER als Test; Spiellauf erreicht nachweislich einen Zustand nach dem Hauptmenü (Test mit Prüfkriterium, nicht nur „läuft“).
- **Abhängigkeiten:** keine; läuft parallel zu ⓪. **Status:** Teile 1–2 fertig, PR #37, Erstreview läuft. Wochenlauf als Orakel
  (`StartGame`, `RunToWeekStart`, `RunToMarker`) nach BESTAND §3.1. Offen für R3: Maschinenabzug zum Wiederaufsetzen an Marken
  (BESTAND §3.1 Punkt 3).

### ② `grafik` – VGA-Treiber `23CD` (Quelle `dev/ap2-vga`, `5a9f178`; zugeschnitten nach BESTAND §3.1)

- **Teile:** (1) Übernahme per Cherry-pick von `0e95549`, `5a9f178`; `f4eb83e` und `ff31159` (Commit/Revert-Paar, auf `main`
  anders gelöst) sowie `dab5889` (nur `AP2.md` und `tools/exeimg.py`) auslassen (N2). `ModeXMemory`, RAM-Atlas, `GRAFIK.md` (eigene Worte), Tests auf `[GameDataFact]`/
  `TestGameData.Dos` (`DriverHarness`), `[ORIGINAL] 23CD:<off>`-Marker, Befunde als **Sammelbefunde** in `docs/routines/23CD_*.md`
  (ein Befund darf mehrere Adressen tragen): Rechteck/Pixel, Blits, Masken, Text, Seiten, Atlas-Umwandlung `166A/3079`;
  `[DIFFERENTIAL] 23CD:<off>` je portierter Routine. (2) Fehlende Treiberteile: Linie (2 Vektoren), Objekt zeichnen/zurückschreiben
  `06E9`/`071C` (2 Vektoren), je mit Differenztest. (3) Grenzfälle (Clipping, Seitenwechsel), Kartenwahl `23CD:04F6` nur VGA;
  `tools/exeimg.py` (aus `dab5889` per Pfad-Checkout) nur, wenn
  `tools/segmap.py` es nicht abdeckt.
- **Nicht in ②:** EGA-, Tandy-Teil (entfällt, nur VGA), `0A96`, `0A69`, `0B67`, `21AF`, Paletten/Blenden, Farbregister → R3
  (Grafikschichten).
- **Dateigrenzen:** `src/Traders.Vga/**`, `tests/Traders.Tests/Vga/**`, `docs/analyse/GRAFIK.md`, `docs/routines/23CD_*`, ggf.
  `tools/exeimg.py`; Block in `STATUS.md` per `abdeckung.py --write`. Kein Eingriff in `src/Traders.RefEmu/`.
- **Abnahme:** alle drei Läufe grün; `abdeckung.py --check` grün; je Differenztest Gegenprobe im Erstreview (Mutation → rot);
  VRAM-Vergleich gegen `VgaAdapter` des RefEmu, Erwartungswerte nie aus dem Port. Die Treiberroutinen liegen hinter der
  Vektortabelle und sind keine Far-Einsprünge: Lehnt `abdeckung.py` solche Adressen ab, im PR melden (Werkzeug gehört nicht zu ②).
- **Abhängigkeiten:** keine (⓪ gemergt). **Status:** Teile 1–3 fertig, PR #43 bereit zum Erstreview (11.11.2026); `tools/exeimg.py`
  nicht übernommen (`segmap.py` deckt die Disassemblierung ab).

### ③ `phasen` – Wochenschleife und Aufrufgraph-Werkzeug (Quelle `dev/ap5-phasen`, `8ed0d34`; zugeschnitten nach BESTAND §3.1)

- **Teile:** (1) `tools/callgraph.py` übernehmen, Ausgabe nur lokal (F1, `.gitignore`-Zeile); Experiment nach `experiments/phasen/`.
  (2) Befunde in `docs/routines/`: `1777_01F8_wochenschleife.md` (Folge der 30 Aufrufe nach BESTAND §1.4, Marken 0–6, beide
  Abbruchwege: Scancode 0Eh nach der Bilanz, Rückgabe aus `20B3:0005`; Wochenzähler `DS:4B9A`) und `1777_000C_main.md`. Kein Port.
  (3) Test im RefEmu, der Markenfolge und Wochenzähler über mindestens zwei Wochen gegen den Befund prüft (nutzt `RunToMarker` aus
  ①); ist ① beim Start von Teil 3 nicht gemergt: Teil blockiert (`spur-wartet`, „wartet auf ①“).
- **Entfällt gegenüber dem Plan:** `docs/analyse/PHASEN.md` (Inhalt steht in BESTAND §1.4 und den Befunden), Video-Zeitstempel.
- **Dateigrenzen:** `tools/callgraph.py`, `experiments/phasen/`, `docs/routines/1777_*`, `tests/Traders.Tests/Phases/**`,
  `.gitignore` (eine Zeile); Block in `STATUS.md` per `abdeckung.py --write` (N1).
- **Abnahme:** alle drei Läufe grün (neue Tests); `callgraph.py` reproduzierbar (zweiter Lauf ohne Diff); Befunde mit Adressen und
  Probe; Test aus Teil 3 wird rot, wenn eine Marke vertauscht oder ausgelassen wird (Gegenprobe im Review); Laufzeit: der Test
  verlängert `pruefen.sh --schnell` um höchstens 3 Minuten, sonst als eigenes Filtermerkmal außerhalb von `--schnell` führen und
  im PR nennen (N3); kein Originaltext im Repo.
- **Abhängigkeiten:** Teile 1–2 sofort; Teil 3 nach Merge von ①. **Status:** offen, Start durch den Nutzer.

### Zuständigkeit gemeinsamer Dateien in R2

`docs/analyse/BESTAND.md`: ⓪ (P3 #39 bei Bedarf Leitstelle). `OriginalProgram.cs`: ①. `STATUS.md`: nur der erzeugte Block zwischen `abdeckung:anfang/ende`
per `tools/abdeckung.py --write` durch die Spur, die Marker oder Befunde hinzufügt (②, ③, ggf. ①); bei Konflikt im Block nach Rebase `--write`
erneut ausführen (R2P-1). `.gitignore`: ③ (eine Zeile). Übriges `STATUS.md`, `CHANGELOG.md`, dieser Plan: Leitstelle. `GameState.cs`,
`TradersRedone.slnx`, `Directory.Build.props`, `global.json`: niemand (bei Bedarf im PR begründen, Leitstelle entscheidet).

### Integrationsfolge

⓪ und ① parallel (⓪ gemergt); ② und ③ nach dem Zuschnitt dieses Plans (Leitstelle 11.11.2026) parallel, ③ Teil 3 nach ①.
Merge ① → ② → ③. Spuren sind dateigetrennt; Konflikte nur
dann über `integration/runde-2`.

### Offene Fragen an den Nutzer

- **F1 Aufrufgraph-Ausgabe – entschieden (Nutzer 11.11.2026):** `docs/analyse/callgraph/` aus `dev/ap5-phasen` (erzeugt aus
  `T.EXE`, mit Originaltexten) kommt nicht ins Code-Repo; Spuren erzeugen sie lokal, die Leitstelle legt bei Bedarf einen Stand im
  Referenz-Repo unter `analyse/callgraph/` ab (PR `daten/callgraph`).
- **F2 Go – erteilt (Nutzer 11.11.2026):** Runde 2 mit ⓪ (Fable) und ①–③ (Opus) wie oben; Spuren startet der Nutzer selbst.
- **Entschieden (Nutzer 11.11.2026, BESTAND §3.3):** nur VGA; Zwischenziel „Gesamtpartie ohne Roboterkampf“ vor dem Roboterkampf
  (`PORTIERUNGSPLAN.md` §5, Fahrplan unten).
- **F3 Zufall beim Bildaufbau (BESTAND B8) – entschieden: A (Nutzer 11.11.2026).** Präzisierung von `AGENTS.md` §4 mit der
  Phasenmaschinen-Spur in R3. Hintergrund: Das Original zieht in Bildschleifen Zufall (Proben P4, P5); der
  Zufallszustand bei Partiebeginn hängt von der Verweildauer im Menü ab. `AGENTS.md` §4 verlangt „Szenen … verbrauchen keinen
  Zufall“ und „was das Original beim Bildaufbau ändert, ist ein Ereignis im Kern“. **A (gewählt):** Regel präzisieren – jeder
  Zufallsverbrauch je Bild ist ein Kern-Ereignis im Takt der Phasenmaschine, die Szene löst es genau einmal je Bild aus; Entwurf in
  der Phasenmaschinen-Spur von R3. **B:** Zufall der Darstellung abweichend vom Original aus einer eigenen Quelle (`[APPROX]`) –
  bricht die Zufallsfolge der Regeln und damit Differenz- und Mehrrundenproben.

## Fahrplan

Nach Runde 2, je Runde 2–4 Spuren; Vorschlag nach BESTAND §3.2 (Leitstelle 11.11.2026), jede Runde mit eigenem Plan und Go:

| Runde | Themen | Segmente |
|---|---|---|
| R3 | Phasenmaschine mit Takt und Zufallsereignissen (F3 = A) · Wochenregeln ohne Bildschirm · Grafikschichten · App-Host mit Eingabe · Ton-Analyse | `1777`, Teile von `0825`/`0609`/`22A3`/`08CF`/`0B9F`; `0A96`, `0A69`, `0B67`, `21AF`, `21FD`-Paletten; `0E9E`/`27C6` |
| R4 | Menüs · neue Partie · Start/Code-Abfrage · Spielfeldschicht | `192D`, `0E31`, `0DFD`, `21FD:0687`, Teile von `0609` |
| R5 | Anbau · Anbau-KI und Wochenplanung · Wochenbilanz | `0609`, `135D`, `11A3`, `0825` |
| R6 | Produktion/Ernte/Wetter · Katastrophen · Wochenereignisse | `22A3`, `111D`, `17CB` |
| R7 | Stationen/Rakete/Piraten | `1B03` |
| R8 | Handel · Handels-KI · Zusatzphase | `0B9F`, `0C29`, `141D`, `1DEF` |
| R9 | Fat Mike und Wettbewerb · Einkauf | `1F63`, `08CF` |
| R10 | Ende/Abend · Ton · **Gesamtpartie ohne Roboterkampf** (Zwischenziel) | `20B3`, `218B`, `21A7` |
| R11–R12 | Roboterkauf, Aufstellung, Kampffeld, Kampf, je mit KI | `1C68`, `16D6`, `0535`, `12C8`, `1E7E`, `0EDE`, `150F` |
| R13 | Gesamtpartie v1.0 | – |

Danach Erweiterungen ([PORTIERUNGSPLAN.md](PORTIERUNGSPLAN.md)).
