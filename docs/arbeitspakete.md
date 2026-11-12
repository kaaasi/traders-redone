# Arbeitspakete

Die Portierung läuft in **Runden**; je Runde arbeiten mehrere **Spuren** parallel, jede auf einem Branch `paket/<spur>` mit
einem PR, eigenen Dateien und klarer Abnahme. Regeln: [AGENTS.md](../AGENTS.md), Ablauf:
[prozess/zusammenarbeit.md](prozess/zusammenarbeit.md). Den Status pflegt jede Spur nur in ihrem eigenen Abschnitt; Plan-
abschnitte ändert die Leitstelle. Abgeschlossene Runden stehen in [archiv/](archiv/) (Paketphase AP1–AP5: [archiv/runde-0.md](archiv/runde-0.md), Runde 1: [archiv/runde-1.md](archiv/runde-1.md), Runde 2: [archiv/runde-2.md](archiv/runde-2.md), Runde 3: [archiv/runde-3.md](archiv/runde-3.md)).

## Aktuelle Runde: 4 – Programmablauf, Menüs, neue Partie, Spielfeldschicht (Plan Leitstelle und Go Nutzer 12.11.2026)

**Ziel:** Der Port läuft vom Programmstart bis in die erste Woche: Start und Code-Abfrage, Hauptmenü mit allen Untermenüs,
neue Partie mit belegten Startwerten, danach die Wochenschleife aus R3. Die erste Szene entsteht (Hauptmenü) und wird bild- und
zufallsgenau gegen den RefEmu geprüft (#86); Eingaben kommen als Befehle aus einer Befehlsquelle im Kern (`0E9E`), auch für
Menschen (#81). Ergebnis: M4 (Menü & Setup), Grundlage für den Anbau (R5) durch die Spielfeldschicht aus `0609`.

**Leitstelle R4:** Claude (Sitzung vom 12.11.2026). **Modell:** alle Spuren und Erstreviews Claude Opus (Spielkern, Disassembly,
RefEmu, Differenztests; `zusammenarbeit.md` §5 „Modellwahl“). **Base:** `main` `0c33796`. Namen der Läufe: `Traders R4 <spur>`.
**Starts zeitversetzt** (Erfahrung R3, Nutzungslimit): 2 und 1 sofort, 3 und 0 eine Stunde später.

**Gemeinsame Vorgaben für alle vier Spuren:**

- Branch `paket/<spur>` von aktuellem `main`, ein Draft-PR, Teile als Aufgabenliste (`spur-start.md` „Teile und Fortsetzung“).
- Je portierter Originalroutine Befund in `docs/routines/` (Sammelbefund mit mehreren Adressen erlaubt), `[ORIGINAL]`-Marker und
  `[DIFFERENTIAL]`-Test gegen den RefEmu mit Grenzfällen und Zufallszustand danach; Erwartungswerte nie aus dem Port.
- **Kern und Szene getrennt** (`AGENTS.md` §4): Menü- und Programmlogik, Zustandsänderungen und jeder Zufallsverbrauch – auch
  der je Bild im Menü (P5; P4: `192D:0234`, `192D:14B4`) – liegen im Kern als Schritte bzw. Kern-Ereignisse der Phasenmaschine;
  Szenen in `src/Traders.Presentation/` lesen nur und zeichnen über `Traders.Vga`. Uhrwerte (z. B. `time()` für `random(0)`)
  und Eingaben sind explizite Eingaben.
- Nur VGA; EGA/Tandy-Zweige und PC-Lautsprecher bleiben außen vor (im Befund nennen, nicht portieren).
- Regeln auf `Dgroup` mit Offsets aus `DgroupLayout.cs` (nur additiv); `GameState`/`GameStateMapper.cs` nur additiv (`partial`).
  Neue DGROUP-Adressen mit Beleg in `docs/analyse/DATENLAYOUT.md` (je Spur eigener Unterabschnitt).
- `STATUS.md`: nur der erzeugte Block per `tools/abdeckung.py --write`; nach jedem Merge Abgleichlauf der nächsten Spur (Merge von
  `main`, `--write`, Label `leitstelle-delta`).

## 0 `programm` – Programmablauf vor der Woche, Start, Code-Abfrage, Host-Anbindung

- **Teile:** (1) **Programmphasen im Kern** (`src/Traders.Core/Phases/`): Ablauf von `main` `1777:000C` als Daten wie die
  Wochenschleife – Initialisierung (nur die spielrelevanten Teile von `1777:04E4`), Start `0DFD:000D`, dann je Durchlauf
  (`1777:013B`): Tonaufrufe `218B:013A`, ggf. `21A7:0054(0,1)`/`218B:0094(1,2)` (R10, als „ausstehend“ bzw. `[APPROX]`, im
  Datensegmentvergleich ausdrücklich behandelt) → Hauptmenü `192D:0007` (`AL` = 0 → Programmende) → nur bei `DS:3CCC` = 1 neue
  Partie `0E31:0003` → in jedem Fall `DS:08FA` = 1 (auch bei geladenem Spielstand) → Wochenschleife `1777:01F8`; Rückgabe 0
  zurück zum Schleifenanfang, Rückgabe 1 Programmende (R4P-3). Menü und neue Partie als ausdrücklich „ausstehende“ Schritte,
  bis 1 und 2 gemergt sind.
  Kommandozeile, Systemschicht `0D73` und Abbau ersetzt der Host (`[APPROX]` mit Begründung). (2) **Start und Code-Abfrage:**
  `0DFD:000D` (Ladefolge: Schrift, Masken, Ladebild, sechs vorgeladene Bilder, Texte; welche Zustandsfelder entstehen) mit dem
  **Ressourcenlader `186C`** (`0002`, `01A9`, `02C6`, `0AB1`, `0B0A`; Zustand `DS:09C8`, Tabelle `DS:09CE–0A8B`, `DS:4C00…`):
  Zustandsmodell im Kern (`Core/Startup`), Datei-, Entpack- und Atlasteil über die vorhandenen `Traders.Assets`/`Traders.Vga`
  (additiv, R4P-2); und
  `21FD:0687` mit `081D`, `08D3` (Neusaat `random(0)` aus explizitem Uhrwert, Ziehung und Ziffernformel; F1 = B: ohne
  Anzeige und Eingabe, `0863` nur im Befund; Befund [21FD_0687_codeabfrage.md](routines/21FD_0687_codeabfrage.md) ergänzen);
  Differenztest: gleicher Uhrwert → gleicher Zufallszustand und gleiches Datensegment wie das Original nach beliebiger Eingabe,
  bis zum Merge von 1 ohne den Textzeigerblock `DS:37CC…3C8F` (`192D:18B6` gehört 1); der Vergleich über das ganze Datensegment
  folgt in Teil 3. (3) **Anbindung** (nach Merge von 1 und 2): `Traders.App` treibt die
  Programmphasen mit `PhaseMachine.Advance`, zeigt Ladebild und Menüszene aus `Traders.Presentation`, Tasten als Befehle;
  **Mehrrundenprobe mit Mensch** (#81 B3): Start → Menü mit einem menschlichen und drei Computerspielern → neue Partie → vier Wochen,
  Bildschirmschritte weiter aus dem RefEmu übernommen, Vergleich wie R3; Architekturregel gegen Rohoffsets wie `d.U32(0x10D2)`
  außerhalb von Core/RefEmu (#81 B4); P3 #76 P3-3 (`27C6:00AC`).
- **Dateigrenzen:** `src/Traders.Core/Phases/**`, `tests/Traders.Tests/Phases/**`, `src/Traders.Core/Startup/**` (neu),
  `src/Traders.Assets/Loading/**` (neu, additiv), `docs/routines/186C_*`,
  `tests/Traders.Tests/Startup/**`, `src/Traders.App/**`, `tests/Traders.Tests/App/**`,
  `src/Traders.Presentation/Startup/**` (Ladebild; ab Teil 3), `docs/routines/{1777_000C,1777_04E4,0DFD,21FD_0687}*`,
  `docs/routines/27C6_*` (nur #76 P3-3), `tools/check-architecture.py` (ohne die Regeln für `Traders.Presentation`, s. 1;
  die Abhängigkeit `Traders.App → Traders.Presentation` in Teil 3 nach Rebase auf 1).
- **Abnahme:** alle drei Läufe grün; Programmablauf im Gleichschritt mit dem RefEmu bis `1777:02DC` der Woche 0 (Markenfolge,
  Datensegment, `DS:10D2`), Gegenprobe im Review (vertauschte Reihenfolge Start/Code, fehlendes `DS:08FA` → rot); Mehrrundenprobe
  mit Mensch ≥ 4 Wochen (`AGENTS.md` §5); `check-architecture.py` grün mit Gegenprobe der neuen Regel; Windows-Start per Hand (Nutzer).
- **Abhängigkeiten:** Teile 1–2 sofort; Teil 3 nach Merge von 1 und 2 (sonst `spur-wartet`, „wartet auf 1/2“).

## 1 `menue` – Menüs `192D` und erste Szene

- **Teile:** (1) **Menülogik im Kern** (`src/Traders.Core/Menus/`): Hauptmenü `192D:0007`, Stufenmenü `0592` mit Stufentabelle
  `DS:0B90…0BAF`, Einstellungen (Optionsfelder `DS:08E4…0902`), Steuerungsmenü (`DS:083C…0843`), Diskettenmenü (Laden/Speichern
  über `2336`/`SaveGame`, Dateizugriff als Host-Schnittstelle, s. F2), Ergebnis `DS:3CCC`; jede Auswahl als Befehl;
  Zufallsverbrauch als Kern-Ereignis an allen sieben Quellen `192D:0234`, `0495`, `12A6`, `138F`, `14B4`, `14FB`, `1A04`
  (17 Aufrufstellen von `21FD:01BA`, R4P-4); das Laden des Menübilds (`186C:0002`) über die vorhandenen Assets, im
  Datensegmentvergleich bis zum Merge von 0 ohne den Laderzustand `DS:09C8…0A8B`, `DS:4C00…`. Texttabelle `192D:18B6` gegen `TextTable` belegen
  (Differenztest auf `DS:37CC…3C8F` als Indexfolge, Zeigerwerte nur relativ). (2) **Szene:** neues Projekt
  `src/Traders.Presentation` (Abhängigkeiten Core, Assets, Vga) mit Menüszene über die Schichten aus R3; pixelgenauer Vergleich
  von VRAM und Palette gegen den RefEmu-Framebuffer für jedes Menü; dazu die Architekturregeln für `Traders.Presentation`
  (liest nur, kein `OriginalRandom`, hängt nicht vom RefEmu ab) und die Rückrichtung für alle Projekte (#76 P3-1).
  (3) **Bild- und Zufallsfolge (#86):** Hauptmenü über *n* Bilder mit Verweildauer und Tastenfolgen (Mensch) im Gleichschritt
  mit dem RefEmu – Bildzähler, `DS:10D2` und Framebuffer je Bild gleich; Gegenprobe im Review (ein Bild doppelt verarbeitet → rot).
- **Dateigrenzen:** `src/Traders.Core/Menus/**`, `tests/Traders.Tests/Menus/**`, `src/Traders.Presentation/**` außer `Startup/**`
  und `Field/**`, `tests/Traders.Tests/Presentation/**`, `docs/routines/192D_*`, `src/Traders.Vga/**` nur additiv (P3 #75 darf
  mit), `tests/Traders.Tests/Vga/**` und `docs/analyse/GRAFIK.md` §8 (nur #75, R4P-7); gemeinsame Dateien `TradersRedone.slnx`, `tests/Traders.Tests/Traders.Tests.csproj`,
  `tests/Traders.Tests/packages.lock.json`, `src/Traders.RefEmu/OriginalProgram.cs` (Hilfen für Bildfolgen, additiv), `AGENTS.md` §4
  (eine Zeile zu `Traders.Presentation`), in `tools/check-architecture.py` die Regeln für `Traders.Presentation` und die Rückrichtung.
- **Abnahme:** alle drei Läufe grün; alle Menüs pixelgenau gegen den RefEmu; Bild- und Zufallsfolge im Gleichschritt (#86);
  Optionsfelder nach jeder Menüfolge gleich dem RefEmu; `check-architecture.py` grün mit Gegenproben.
- **Abhängigkeiten:** keine.

## 2 `partie` – neue Partie `0E31`

- **Teile:** (1) Befund für `0E31:0003`, `0149`, `023D`, `0361`/`0379`, `0465` (Land zufällig oder fest, Stationen, Startwerte
  je Spieler; 14 statische Aufrufstellen von `21FD:01BA` in Schleifen und das Wetter `21FD:021F` – die dynamische Zahl und
  Reihenfolge der Ziehungen im Befund; Schluss von `0465` liegt jenseits `0x06D0`, R4P-5). `023D` (auch aus `1B03:0139`) und
  `0361` (auch aus `111D:02E8`) als einzeln aufrufbare Kernroutinen. (2) Port in `src/Traders.Core/Rules/NewGame*` mit
  Differenztest über `LoadOnly`/`Call`: gesamter Startzustand (Datensegment) und `DS:10D2` danach gleich, für alle vier Stufen,
  festes und zufälliges Land, gemischte Mensch/Computer-Belegung und mehrere Zufallszustände. (3) Grenzfälle (alle Spieler
  Computer, Optionen an den Rändern der Stufentabelle), P3 #74 (Edelsteinkurs-Überlauf, `0825:07C4` bei stark negativer Woche).
- **Dateigrenzen:** `src/Traders.Core/Rules/NewGame*`, `tests/Traders.Tests/Rules/NewGame*`, `docs/routines/0E31_*`; für #74 die
  betroffenen Dateien unter `src/Traders.Core/Rules/` und `tests/Traders.Tests/Rules/`.
- **Abnahme:** alle drei Läufe grün; Startzustand bitgleich für jede Kombination, Gegenprobe im Review (eine Ziehung vertauscht →
  rot); `abdeckung.py --check` grün.
- **Abhängigkeiten:** keine.

## 3 `spielfeld` – Befehlsquelle `0E9E` und Spielfeldschicht `0609`

- **Teile:** (1) **Befehlsquelle im Kern:** `0E9E:000E/0035/0264/0312/037A` (Befund
  [0E9E_000E_steuerung.md](routines/0E9E_000E_steuerung.md)) – Tastenzustand bzw. Gerätewerte als Eingabe, Bits `DS:37C3+i`,
  Einzelmerker und Aktionscode als Ergebnis; alle sechs Steuerungsarten im Kern, der Host liefert vorerst nur Tastatur (s. F3).
  (2) **Spielfeld im Kern:** Feldart setzen `0609:10D2` (schreibt `DS:4442+`, `2EB1+`, zählt `4BA8+`, `2EE7+`) und, was es ruft,
  dazu die Cursorlogik. (3) **Spielfeld als Szene** (nach Merge von 1): die Zeichenroutinen `0609:1256`, `1879`, `1ED9` (nur
  Befehlsliste `0A96:0030/0267`, kein Zustand; R4P-1) für Raster, Cursor und Kopfzeile in `src/Traders.Presentation/Field/`,
  pixelgenau gegen den RefEmu an einem Wochenzustand.
- **Dateigrenzen:** `src/Traders.Core/Input/**`, `src/Traders.Core/Field/**`, `tests/Traders.Tests/{Input,Field}/**`,
  `src/Traders.Presentation/Field/**` (ab Teil 3), `docs/routines/{0E9E,0609}_*` außer `0609_1178*`,
  `src/Traders.RefEmu/Machine/PcMachine*` nur additiv für Gerätewerte von Joystick (Port 201h), Maus (`int 33h`) und LPT (R4P-6).
- **Abnahme:** alle drei Läufe grün; je Steuerungsart Differenztest (Bits und Aktionscode gleich dem RefEmu, Joystick/Maus/LPT
  über gesetzte Gerätewerte im RefEmu, sonst `[APPROX]` mit Begründung); Feldroutinen mit Grenzfällen; Spielfeldbild pixelgenau.
- **Abhängigkeiten:** Teile 1–2 sofort; Teil 3 nach Merge von 1 (sonst `spur-wartet`, „wartet auf 1“).
- **Status (Spur, PR offen):** Teil 1 in Arbeit.

## Zuständigkeit gemeinsamer Dateien in R4

`TradersRedone.slnx`, `tests/Traders.Tests/Traders.Tests.csproj`, `tests/Traders.Tests/packages.lock.json`, `OriginalProgram.cs`,
`AGENTS.md` §4 (eine Zeile): 1. `tools/check-architecture.py`: 0, die Regeln für `Traders.Presentation` und die Rückrichtung: 1.
`src/Traders.Presentation/`: 1 legt an; `Startup/` gehört 0, `Field/` gehört 3. `src/Traders.RefEmu/Machine/PcMachine*`: 3 (nur
additiv, Gerätewerte). Ressourcenlader `186C` mit `src/Traders.Assets/Loading/`: 0. `DgroupLayout.cs`, `GameState.cs` (`partial`),
`GameStateMapper.cs`: nur additiv (alle), bei Konflikt rebased die später gemergte Spur. `docs/analyse/DATENLAYOUT.md`: je Spur
eigener Unterabschnitt. `STATUS.md`: nur der erzeugte Block (alle). Übriges `STATUS.md`, `CHANGELOG.md`, dieser Plan,
`docs/prozess/**`: Leitstelle. `Directory.Build.props`, `global.json`: niemand.

## Integrationsfolge

2, 1, 0 (Teile 1–2), 3 (Teile 1–2) parallel, dateigetrennt bis auf die Zeilen oben. Merge 2 → 1 → 3 → 0 (0 und 3 nach ihrem
Teil 3). Konflikte nur dann über `integration/runde-4`.

## Offene Fragen an den Nutzer

- **G1 Go – erteilt (Nutzer 12.11.2026)** für vier Spuren wie oben, alle Opus, zeitversetzte Starts durch die Leitstelle;
  Plan-Review `reviews/2026-11-12-r4-plan.md` (R4P-1 bis R4P-7 eingearbeitet).
- **F1 Code-Abfrage – B (Nutzer 12.11.2026):** Der Port zeigt „INSERT COIN!“ nicht; die Zufallsneusaat `random(0)` aus dem
  Uhrwert und alle übrigen Zustandsänderungen von `21FD:0687` bleiben (Differenztest), Bild und Tasteneingabe entfallen als
  `[APPROX]` mit Begründung (Original nimmt jede Eingabe an).
- **F2 Diskettenmenü – A (Nutzer 12.11.2026):** Menü originalgetreu, Spielstand als Datei über eine Host-Schnittstelle,
  Diskettenwechsel-Meldungen entfallen als `[APPROX]`.
- **F3 Steuerungsarten – A (Nutzer 12.11.2026):** alle sechs Arten im Kern mit Differenztest, der Host liefert vorerst nur
  Tastatur (Arten 3/4); Joystick/Maus/LPT folgen bei Bedarf.
- **G2 Zugriff – bestätigt (Nutzer 12.11.2026):** Jede Leitstelle, auch künftige, bindet `kaaasi/traders-referenz` mit `push` ein.
- Übrige P3: #39 R3 und #89 mit diesem PR behoben; #74 → 2; #75 → 1; #76 P3-1 → 1, P3-3 → 0, P3-2 (`TON.md`) mit dem Ton (R10);
  #81 → 0; #86 → 1; #49 offen (nächster Lauf an `experiments/bestand/`).

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
