# Arbeitspakete

Die Portierung läuft in **Runden**; je Runde arbeiten mehrere **Spuren** parallel, jede auf einem Branch `paket/<spur>` mit
einem PR, eigenen Dateien und klarer Abnahme. Regeln: [AGENTS.md](../AGENTS.md), Ablauf:
[prozess/zusammenarbeit.md](prozess/zusammenarbeit.md). Den Status pflegt jede Spur nur in ihrem eigenen Abschnitt; Plan-
abschnitte ändert die Leitstelle. Abgeschlossene Runden stehen in [archiv/](archiv/) (Paketphase AP1–AP5: [archiv/runde-0.md](archiv/runde-0.md), Runde 1: [archiv/runde-1.md](archiv/runde-1.md)).

## Aktuelle Runde: 3 – Phasenmaschine, Wochenregeln, Grafikschichten, Host (Plan Leitstelle 11.11.2026, Go nötig)

**Ziel:** Kern und Ausgabe bekommen ihr Gerüst, bevor die erste Szene entsteht (R4): Phasenmaschine mit Takt und
Zufallsereignissen je Bild (F3 = A), die Regeln der Woche ohne Bildschirm als belegte Kernroutinen, die Grafikschichten über dem
Treiber `23CD` und ein Host, der Framebuffer zeigt und Eingaben als Scancodes liefert. Ergebnis: M3 abgeschlossen (Phasenmaschine,
Wochenregeln mit Mehrrundenprobe), Grundlage für M4 (Menüs, R4).

**Leitstelle R3:** Claude (Sitzung vom 11.11.2026). **Modell:** alle Spuren und Erstreviews Claude Opus (Spielkern, Disassembly,
RefEmu, Differenztests; `zusammenarbeit.md` §5 „Modellwahl“). **Base:** `main` `aba13bf`. Namen der Läufe: `Traders R3 <spur>`.

**Gemeinsame Vorgaben für alle vier Spuren:**

- Branch `paket/<spur>` von aktuellem `main`, ein Draft-PR, Teile als Aufgabenliste (`spur-start.md` „Teile und Fortsetzung“).
- Je portierter Originalroutine Befund in `docs/routines/` (Sammelbefund mit mehreren Adressen erlaubt), `[ORIGINAL]`-Marker und
  `[DIFFERENTIAL]`-Test gegen den RefEmu mit Grenzfällen und Zufallszustand danach; Erwartungswerte nie aus dem Port.
- Regeln und Phasen arbeiten auf `Dgroup` (Byteabbild) mit Offsets aus `DgroupLayout.cs` (nur additiv, im PR begründet);
  `GameState`/`GameStateMapper.cs` nur, wenn ein Feld modelliert werden muss – dann `partial` ergänzen (wer zuerst braucht) und
  den Mapper nur additiv erweitern (R3P-4). Neue
  DGROUP-Adressen mit Beleg in `docs/analyse/DATENLAYOUT.md` (je Spur eigener Unterabschnitt, kein Umschreiben fremder Zeilen).
- Kein eigener Zufall, keine Host-Uhr im Kern (`AGENTS.md` §4); Wartezeiten und Bildtakt sind explizite Eingaben.
- `STATUS.md`: nur der erzeugte Block per `tools/abdeckung.py --write` (bei Konflikt nach Rebase erneut ausführen, R2P-1).

## 0 `takt` – Phasenmaschine, Takt, Zufall je Bild, Maschinenabzug

- **Teile:** (1) **Maschinenabzug im RefEmu** (offen aus R2, BESTAND §3.1 Punkt 3): Zustand der Maschine an einer Marke
  `21FD:0977(n)` sichern und wiederaufsetzen (CPU, Speicher, Zeitgeber, Tastatur, VGA, Ton-TSR); Nachweis: Lauf mit Wiederaufsetzen
  = durchgehender Lauf, bitgleich in Datensegment, Zufallszustand `DS:10D2`, Zeit und Bild an der nächsten Marke. Abzüge entstehen
  zur Testlaufzeit in einem Zwischenordner und nie im Repo (abgeleitet aus `T.EXE`, `AGENTS.md` §2.1).
  (2) **Phasenmaschine und Takt im Kern** (`src/Traders.Core/Phases/`): Schrittfolge der Woche nach
  `docs/routines/1777_01F8_wochenschleife.md` (30 Zeilen einschließlich der sieben Marken; Schrittnummern in diesem Plan nach
  `BESTAND.md` §1.4, 24 Schritte ohne Marken; beide Abbruchwege) als Daten, Takt als explizite
  Tick-Eingabe, Spielerhandlungen als Befehle, Zufallsverbrauch je Bild als Kern-Ereignis, das die Szene genau einmal je Bild auslöst
  (F3 = A); noch nicht portierte Schritte als ausdrücklich „ausstehend“ markierte Schritte, kein stilles Überspringen. Dazu die
  Präzisierung von `AGENTS.md` §4 (Zeile „Szenen und Rendering lesen nur“) und, wo prüfbar, eine Regel in `tools/check-architecture.py`.
  (3) **Anbindung und Mehrrundenprobe** (nach Merge von 1 `regeln`): die bildschirmlosen Schritte nach `BESTAND.md` §1.4 – 1 `1777:02DC`, 2 `0825:07C4`,
  5 und 16 `0609:1178`, 10 `21FD:021F`, 11 `22A3:0003`, 12 `22A3:026B`, 21 `08CF:11D7` – rufen die Routinen aus Spur 1; Test über vier Wochen mit vier Computerspielern: an jedem dieser Schritte Datensegment aus dem RefEmu
  übernehmen, Schritt im Port ausführen, Ergebnis gegen den RefEmu beim Rücksprung vergleichen (Datensegment, `DS:10D2`), dazu
  Markenfolge und Wochenzähler; Abbruchweg B (Rückgabe aus `20B3:0005`) per gesetztem Zustand prüfen. P3 #52 mitnehmen.
- **Dateigrenzen:** `src/Traders.Core/Phases/**`, `tests/Traders.Tests/Phases/**`, `src/Traders.RefEmu/**` (nur Maschinenabzug,
  additiv), `tests/Traders.Tests/RefEmu/**` (nur neue Dateien), `docs/routines/1777_*` außer `1777_02DC*`, `docs/analyse/REFEMU.md`,
  `AGENTS.md` §4 (eine Zeile), `tools/check-architecture.py` (ohne die Regel für `Traders.App`, s. 3). Gemeinsame Datei `src/Traders.RefEmu/OriginalProgram.cs` gehört in R3
  dieser Spur.
- **Abnahme:** alle drei Läufe von `tools/pruefen.sh` grün; Wiederaufsetzen bitgleich (Test, Gegenprobe: ein ausgelassenes
  Zustandsteil macht ihn rot); Mehrrundenprobe ≥ 4 Wochen nach `AGENTS.md` §5, Gegenprobe im Review (vertauschter Schritt, falscher
  Rundungsweg → rot); `--schnell` wird um höchstens 3 Minuten länger, sonst eigenes Filtermerkmal außerhalb von `--schnell` und im
  PR nennen; `check-architecture.py` grün.
- **Abhängigkeiten:** Teile 1–2 sofort; Teil 3 nach Merge von 1 (sonst `spur-wartet`, „wartet auf 1“).
- **Status:** Teil 1 in Arbeit (Claude, Draft-PR `paket/takt`, Base `7a645ff`).

## 1 `regeln` – Wochenregeln ohne Bildschirm

- **Teile** (je Routine Befund, Port in `src/Traders.Core/Rules/`, Differenztest mit gesetztem Zustand über `LoadOnly`/`Call` wie
  Proben P2/P3, Grenzfälle einschließlich 16-Bit-Überlauf über `Int16Math`):
  (1) ohne Zufall: Wochenpreise `1777:02DC` (BESTAND B1: keine Uhr), Obergrenzen `0609:1178`, Schwund `22A3:026B` – mit P3 #39 R1
  (Überlauf ab 4096, Zähler `DS:37AA`/`DS:45D0` als Seiteneffekt);
  (2) Vermögen/Rang/Verlauf `0825:0813`, `0825:06CB`, `0825:07C4`, Produktion `22A3:0003` (Raster `DS:48F4`, BESTAND B5);
  (3) mit Zufall: Wetter `21FD:021F`, Edelsteinkurs `08CF:11D7`, Warenpreise `0B9F:06A0`/`078A` (Zufallszustand vorher/nachher und
  Zahl der Ziehungen im Test).
- **Nicht in 1:** neue Partie `0E31:0003` (→ R4 mit Menü und Spielstart; abweichend von der Vorschau, um die Spur in drei Teilen zu
  halten), Endbedingung `20B3:0005` (→ R10), Anbindung an die Phasenmaschine (→ 0 Teil 3).
- **Dateigrenzen:** `src/Traders.Core/Rules/**`, `tests/Traders.Tests/Rules/**`, `docs/routines/{1777_02DC,0609_1178,22A3_*,0825_*,21FD_021F,08CF_11D7,0B9F_*}*`,
  eigener Unterabschnitt in `docs/analyse/DATENLAYOUT.md`.
- **Abnahme:** alle drei Läufe grün; `abdeckung.py --check` grün; je Differenztest Gegenprobe im Erstreview; die Mehrrundenprobe
  dieser Regeln liefert 0 Teil 3 (`AGENTS.md` §5), Spur 1 nennt dafür im PR die Eingangs- und Ausgabefelder je Routine.
- **Abhängigkeiten:** keine.

## 2 `schichten` – Grafikschichten über dem Treiber

- **Teile:** (1) Seiten und Warten `0B67`, Paletten und Blenden `21FD:026C…04DA` (Basis-, geladene, Arbeitspalette, Blendfarbe
  `DS:10D8`), Seitenwechsel `21FD:051F` (sichtbare/verdeckte Seite `DS:082E/0830`), Farbregister `2295:006D/00C5`; Wartezeiten als Schrittzahl bzw. Tick-Eingabe,
  keine eigene Uhr. (2) Befehlsliste `0A96` (17 Befehlstypen, Ausführung `06A8`, Liste `DS:15BA…`) und Hilfen `0A69`.
  (3) Bewegte Objekte `21AF` (8 Sätze ab `DS:330C`) über `23CD:06E9/071C`; P3 #60 klären (erreicht das Spiel den Fall?).
- **Grenze zum Kern:** Die Schichten ändern keinen Spielzustand. Zieht eine Routine Zufall (z. B. Warteschleife `21FD:0004` im
  Fat-Mike-Modus, BESTAND §1.4), wird nur die Darstellung portiert und die Ziehung im Befund als Kern-Ereignis für 0 notiert.
- **Dateigrenzen:** `src/Traders.Vga/**`, `tests/Traders.Tests/Vga/**`, `docs/routines/{0B67,0A96,0A69,21AF,2295}_*`,
  `docs/routines/21FD_*` nur für Adressen aus `026C…04DA` (Paletten) und `051F`, `docs/analyse/GRAFIK.md`. Kein Eingriff in `src/Traders.RefEmu/`.
- **Abnahme:** alle drei Läufe grün; VRAM- und Palettenvergleich gegen den `VgaAdapter` des RefEmu nach `Call` der Originalroutine
  (`Frame()`), Gegenprobe je Test im Review; Befehlsliste mit allen 17 Typen mindestens einmal.
- **Abhängigkeiten:** keine.

## 3 `host` – App-Host mit Eingabe, Ton-Analyse

- **Teile:** (1) Eingabe und Zeitgeber des Originals analysieren: `27C6` (Tastatur- und Zeitgeber-Interrupt, Tastenzustand
  `DS:3D47–3DC6`, Tickzähler `DS:117E/1180`, Zeitgeberfrequenz), `0E9E` (Steuerungsarten), Befunde in `docs/routines/`; Host-Ersatz
  als `[APPROX]`, wo kein Differenztest möglich ist, mit Begründung. (2) `src/Traders.App` (MonoGame DesktopGL, Paket wie
  `Traders.Viewer`): Fenster mit Skalierung 320 × 200, fester Tick-Takt nach Teil 1, Tastatur → Scancode-Ereignisse (Make/Break) im
  Format des Tastenzustands, Anzeige eines Framebuffers aus `Traders.Vga`; noch ohne Szenen (R4). Test ohne Fenster: Abbildung
  Taste → Scancode und Tick-Zählung bei unterschiedlicher Bildrate. (3) Ton-Analyse: `FXDRIVER.EXE` (Schnittstelle `int 64h`),
  `218B`, `21A7:0054` (drei Wege nach `DS:0910`, nur der Treiberweg wird portiert) → `docs/analyse/TON.md`; P3 #39 R2 (`2850`).
- **Dateigrenzen:** `src/Traders.App/**`, `tests/Traders.Tests/App/**`, `docs/routines/{27C6,0E9E}_*`, `docs/analyse/TON.md`;
  gemeinsame Datei `TradersRedone.slnx` (eine Zeile, neues Projekt) gehört in R3 dieser Spur, ebenso
  `tests/Traders.Tests/Traders.Tests.csproj` (eine `ProjectReference`), `tests/Traders.Tests/packages.lock.json` und in
  `tools/check-architecture.py` die Regel für `Traders.App` (keine Abhängigkeit auf `Traders.RefEmu`, R3P-3). Lockdatei des neuen Projekts
  mitcommitten; fehlt ein Paket im Offline-Spiegel, im PR melden (Leitstelle stößt `werkzeuge.yml` an).
- **Abnahme:** alle drei Läufe grün; `check-architecture.py` grün mit der neuen Regel (App hängt nicht vom RefEmu ab, Kern nicht von App; Gegenprobe im Review); Befunde mit
  RefEmu-Probe (z. B. Tickzähler nach *n* emulierten Sekunden, Tastenzustand nach `Press`); Windows-Start nur per Hand (Nutzer).
- **Abhängigkeiten:** keine. Die Anbindung an die Phasenmaschine folgt in R4.

## Zuständigkeit gemeinsamer Dateien in R3

`OriginalProgram.cs`: 0. `TradersRedone.slnx`, `tests/Traders.Tests/Traders.Tests.csproj`, `tests/Traders.Tests/packages.lock.json`: 3.
`tools/check-architecture.py`: 0, die Regel für `Traders.App`: 3. `AGENTS.md` §4 (eine Zeile): 0. `DgroupLayout.cs`, `GameState.cs`
(`partial`), `GameStateMapper.cs`: nur additiv (0, 1), bei Konflikt rebased die später gemergte Spur. `docs/analyse/DATENLAYOUT.md`: je Spur eigener Unterabschnitt. `STATUS.md`:
nur der erzeugte Block (alle). Übriges `STATUS.md`, `CHANGELOG.md`, dieser Plan, `docs/prozess/**`: Leitstelle.
`Directory.Build.props`, `global.json`: niemand.

## Integrationsfolge

0 (Teile 1–2), 1, 2, 3 parallel; dateigetrennt bis auf die Zeilen oben. Merge 1 → 2 → 3, 0 nach seinem Teil 3 zuletzt (braucht 1).
Konflikte nur dann über `integration/runde-3`.

## Offene Fragen an den Nutzer

- **G1 Go** für Runde 3 mit vier Spuren wie oben, alle Opus.
- **G2 Zugriff:** Diese Leitstellensitzung hat kein Schreib-/Leserecht auf `kaaasi/traders-referenz` (`add_repo` von der Freigabe
  abgelehnt, `git ls-remote` scheitert); ohne das legt sie nach `anhang.md` F keine Läufe an. Entweder Zugriff freigeben oder die
  vier Läufe selbst anlegen (beide Repos, Code-Repo mit Schreibzugriff).
- Übrige P3 aus R2: #36 mit diesem PR behoben; #39 R1 → 1, R2 → 3, R3 offen (nur Präzisierung, Leitstelle bei Gelegenheit);
  #49 offen (nächster Lauf an `experiments/bestand/`); #52 → 0; #60 → 2.

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
