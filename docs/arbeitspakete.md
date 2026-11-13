# Arbeitspakete

Die Portierung läuft in **Runden**; je Runde arbeiten mehrere **Spuren** parallel, jede auf einem Branch `paket/<spur>` mit
einem PR, eigenen Dateien und klarer Abnahme. Regeln: [AGENTS.md](../AGENTS.md), Ablauf:
[prozess/zusammenarbeit.md](prozess/zusammenarbeit.md). Den Status pflegt jede Spur nur in ihrem eigenen Abschnitt; Plan-
abschnitte ändert die Leitstelle. Abgeschlossene Runden stehen in [archiv/](archiv/) (Paketphase AP1–AP5: [archiv/runde-0.md](archiv/runde-0.md), Runde 1: [archiv/runde-1.md](archiv/runde-1.md), Runde 2: [archiv/runde-2.md](archiv/runde-2.md), Runde 3: [archiv/runde-3.md](archiv/runde-3.md), Runde 4: [archiv/runde-4.md](archiv/runde-4.md)).

## Aktuelle Runde: 5 – Wochenbilanz, Wochenplanung, Anbau mit Anbau-KI (Plan Leitstelle und Go Nutzer 13.11.2026)

**Ziel:** Die ersten Bildschirmschritte der Woche laufen im Port statt im RefEmu: Wochenbilanz `0825:000A` (Aufruf 3, schon in
Woche 0), Wochenplanung der Computerspieler `11A3:0000` (Aufrufe 6 und 21), Anbau `0609:0004` (Aufruf 8) mit Spielfeldschleife
`0609:03E0`, `DrawSelection` `0609:0C73` und dem Computerzug `135D:000E`. Die Mehrrundenprobe übernimmt diese Schritte nicht mehr
aus dem RefEmu. Ergebnis: M5 (Anbau) ✅, Teil von M11 (Wochenbilanz). Dazu die Pflichten aus R4: Leseregel der Szenen (#101,
#103, #105), `DS:330C` mit einem Besitzer (#103 P3-4), Sammelmarker in `abdeckung.py` (#109).

**Leitstelle R5:** Claude (Sitzung vom 13.11.2026). **Modell:** alle Spuren und Erstreviews Claude Opus (Spielkern, KI,
Disassembly, Differenztests; `zusammenarbeit.md` §5 „Modellwahl“). **Base:** `main` `542d0d4`. Namen der Läufe: `Traders R5 <spur>`.
**Starts zeitversetzt** (Nutzungslimit): 0 und 2 sofort, 1 und 3 eine Stunde später.

**Gemeinsame Vorgaben für alle vier Spuren:**

- Branch `paket/<spur>` von aktuellem `main`, ein Draft-PR, Teile als Aufgabenliste (`spur-start.md` „Teile und Fortsetzung“).
- Je portierter Originalroutine Befund in `docs/routines/` (Sammelbefund erlaubt; **je Adresse ein eigener `[ORIGINAL]`-Marker**,
  bis #109 gemergt ist), `[DIFFERENTIAL]`-Test gegen den RefEmu mit Grenzfällen und Zufallszustand `DS:10D2` danach;
  Erwartungswerte nie aus dem Port. Teil 1 jeder Spur stellt die transitiv gerufenen, noch nicht portierten Routinen fest
  (`tools/segmap.py`, `tools/callgraph.py`) und schreibt die Liste in den Befund; was einer anderen Spur gehört, wird gerufen,
  nicht nachgebaut.
- **Kern und Szene getrennt** (`AGENTS.md` §4): Ablauf, Zustandsänderungen, Käufe, Zufall und KI im Kern als Schritte bzw.
  Kern-Ereignisse der Phasenmaschine; Szenen in `src/Traders.Presentation/` lesen nur. Spielerhandlungen sind Befehle
  (`Submit(IPlayerCommand)`), Eingaben kommen über die Befehlsquelle `0E9E` aus R4.
- Schritt-Handler liefern die Spuren 1–3 als Klassen in ihren Dateien; **eingebunden in die Phasenmaschine werden sie nur von
  Spur 0 Teil 3** (`Phases/**` gehört 0). Bis dahin laufen ihre Differenztests über `LoadOnly`/`Call` bzw. das Wochenorakel.
- Nur VGA; Regeln auf `Dgroup` mit Offsets aus `DgroupLayout.cs` (nur additiv); `GameState`/`GameStateMapper.cs` nur additiv
  (`partial`). Neue DGROUP-Adressen mit Beleg in `docs/analyse/DATENLAYOUT.md` (je Spur eigener Unterabschnitt).
- `STATUS.md`: nur der erzeugte Block per `tools/abdeckung.py --write`; nach jedem Merge Abgleichlauf der nächsten Spur (Merge
  von `main`, `--write`, Label `leitstelle-delta`).

## 0 `anbau` – Anbauphase `0609:0004`, Spielfeldschleife, Einbindung der Wochenschritte

- **Teile:** (1) **Anbau im Kern** (`src/Traders.Core/Field/Planting*`): Befund `0609:0004` mit Spielfeldschleife `0609:03E0`
  und allen von ihnen gerufenen, noch offenen `0609`-Routinen mit Zustand (Kandidaten laut Aufrufgraph, Plan-Review R5P-4:
  `00D6`, `0160`, `032A`, `064D`, `069F`, `07D6`, `096A`, `0AEF`, `0FA9`, `11E7`, `1540`, `1A03`, `1A97`, `1AC0`, `1BE5`, `1C83`,
  `1CB1`, `1CF4`, `1EA3`, `1F2D`, `1FCC`, `1FE0`; Kern- und Zeichenteil je Routine im Befund belegen und trennen; `0080` gehört
  zu `1F63`, nicht zum Anbau): Spielerfolge, HQ/Feld kaufen, Buchungen, Abbruch- und Zeitgrenzen, jeder Zufallsverbrauch als
  Kern-Ereignis je Bild – auch die Ziehung in `0609:13C5` (zugleich Zeichnen, geteilt mit `08CF`, `1DEF`, `20B3`, `22A3`: Ziehung
  im Kern, Zeichnen in Teil 2). Gerufen, nicht nachgebaut: Warteschleife `21FD:0004` mit Helfern (Spur 3), `0609:074D`/`100F`
  (Spur 1); bis zu deren Merge über das Gleichschritt-Orakel. Menschlicher Zug als Befehle; der Computerzug läuft über den
  vorhandenen Rückruf `IFieldOutput.ComputerTurn` (`Field/FieldCursor.cs`), an den Spur 1 andockt – bis zum Merge von 1 im
  Orakel aus dem RefEmu. **`DS:330C` bekommt einen Besitzer** (#103 P3-4): Festlegung im Befund, welche Seite `21AF:045E`
  spiegelt; Leseprüfung `AssertReadOnly` deckt `DS:330C` ab (#103 P3-2). (2) **Szene** `src/Traders.Presentation/Field/`:
  `DrawSelection` `0609:0C73` mit `0D02`, `0DF9`, dazu `0204`, `18E5`, `1912`, `193C`, `1978` und der Zeichenteil von `13C5`
  (nur Befehlsliste `0A96`/Schichten `0A69`, kein Zustand; R5P-3), pixelgenau gegen den RefEmu je Bild über eine Anbaurunde mit
  Mensch; Szenentests vergleichen auch die Befehlsliste (#103 P3-3). (3) **Einbindung** (nach Merge von 1, 2, 3): Handler für
  Aufrufe 3 (Spur 3), 6 und 21 (Spur 2) und 8 in `WeekRuleSteps`/`PhaseMachine`; Aufruf 4 `111D:0009`: nur der Wächter
  (`111D:000D–001B`: Woche `DS:4B9A` ≤ 2 oder `DS:2F30` = 0 → Leerlauf) als belegter Schritt, sonst ausstehend (R5P-6).
  **Mehrrundenprobe** über ≥ 4 Wochen mit einem Menschen und drei Computerspielern, in der die Aufrufe 3, 6, 8, 21 im Port laufen
  (Aufruf 4 ab Woche 3 aus dem Orakel; Vergleich Datensegment und `DS:10D2` je Aufrufstelle wie R3/R4); Host nach F1 = A
  (Bilanz und Anbau spielbar, Halt mit Hinweis am ersten ausstehenden Schritt – Aufruf 9 in Woche 0–2, sonst ggf. Aufruf 4);
  P3 #113.
- **Dateigrenzen:** `src/Traders.Core/Field/**` außer `Field/PlantingAiHelpers*`, `tests/Traders.Tests/Field/**`,
  `src/Traders.Presentation/Field/**`, `tests/Traders.Tests/Presentation/Field*`, `docs/routines/0609_*` außer `0609_1178*`,
  `0609_074D*`, `0609_100F*`, `docs/routines/111D_0009*` (nur Wächter), `src/Traders.Core/Phases/**` und
  `tests/Traders.Tests/Phases/**` (ab Teil 3), `src/Traders.App/**`, `tests/Traders.Tests/App/**` (Teil 3),
  `src/Traders.RefEmu/OriginalProgram.cs` (additiv, Hilfen für Bildfolgen und Spielerzüge), `src/Traders.Core/Objects/**` bzw.
  der Kernteil von `21AF:045E` (nur #103 P3-4).
- **Abnahme:** alle drei Läufe grün; Anbau im Gleichschritt mit dem RefEmu (Datensegment, `DS:10D2`, Bildzähler) für Woche 0
  und eine spätere Woche, je mit Menschen- und Computerzug, Grenzfälle (kein Geld, alles belegt, Zeitablauf); Gegenprobe im
  Review (ein Kauf doppelt gebucht, ein Bild doppelt verarbeitet, Ziehung in `13C5` ausgelassen → rot); Szene pixelgenau;
  Mehrrundenprobe ≥ 4 Wochen (`AGENTS.md` §5); `check-architecture.py` grün.
- **Abhängigkeiten:** Teile 1–2 sofort; Teil 3 nach Merge von 1, 2 und 3 (sonst `spur-wartet`, „wartet auf 1/2/3“).
- **Status:** Teil 1 fertig (Claude, PR #122, Fortsetzung 1): Kern `Field/Planting.cs` mit Befund `routines/0609_0004_anbau.md` und Differenztests je Routine; Bildfolge (`Planting.Run`) und Schritt-Handler `Field/PlantingStep.cs` für Aufruf 8; Gleichschritt mit dem RefEmu im Spiel je Rücklauf (`Field/PlantingFrameDifferentialTests.cs`), Orakel für `074D`/`100F`/`135D` (Spur 1) und `21FD:0004` (Spur 3) bis zu deren Merge. `DS:330C`-Besitzer festgelegt, #103 P3-2 behoben. Teil 2 zum Großteil (Fortsetzung 2): alle Zeichenroutinen der Anbauphase in `Presentation/Field/FieldScene.cs` (Befund `routines/0609_0C73_anbauszene.md`), Pixel- und Befehlslistentests je Routine und `1BE5` ganz (`Field/PlantingSceneDifferentialTests.cs`, #103 P3-3); Korrektur: `random(1)` in `13C5` wählt das Randstück (0/1), `DrawBorder` trägt die Ergebnisse. Teil 2 fertig (Fortsetzung 3): Ausgabe der Anbauphase in `Presentation/Field/FieldScene.Output.cs` (Bild, Objekte mit Spiegel `DS:330C`, Paletten und Blenden, Seiten, Liste, `0185`, `0665`, `0204` ganz) Bild für Bild; in allen sechs Gleichschrittabläufen (auch mit Mensch) VRAM, DAC und CRTC-Start an jedem Rücklauf gleich (Warteschleife `21FD:0004` im Orakel, Ausgabe dort übernommen), vier Gegenproben rot. Offen: Teil 3 (Einbindung, nach 1/2/3).

## 1 `anbau-ki` – Computerzug im Anbau `135D`

- **Teile:** (1) Befund `135D:000E` (Computerzug, aufgerufen aus `0609`) mit den weiteren Einsprüngen `0935` (aus `0609:07D6`),
  `0AB3` (aus `0609:069F`) und allen internen Routinen (`00AC`, `0240`, `0263`, `0284`, `02BD`, `0480`, `04F5`, `05AF`, `06A1`,
  `0851`, `0A59`) sowie den zwei `0609`-Helfern **`0609:074D`** (aus `135D:00AC`) und **`0609:100F`** (aus `05AF`, `08B7`), die
  diese Spur portiert (R5P-2): Feldbewertung, Feldwahl, Cursorziel, Reihenfolge der Feldprüfung (`135D` zieht keinen Zufall,
  R5P-5). Vertrag für Spur 0 im Befund: Andocken an den vorhandenen Rückruf `IFieldOutput.ComputerTurn(Dgroup, x, y)`
  (`Field/FieldCursor.cs`, Befund `0609_047B_cursor.md`) und Einsprünge `0935`/`0AB3`; sofort als erstes Ergebnis committen.
  (2) Port in `src/Traders.Core/Ai/Planting*` und `src/Traders.Core/Field/PlantingAiHelpers*` (reine Logik auf `Dgroup`) mit
  Differenztests über `LoadOnly`/`Call` für jeden Einsprung und beide Helfer: Rückgabe, Datensegment und `DS:10D2`
  (unverändert) gleich, für alle vier Stufen, volle und leere Felder, Geldgrenzen. (3) Grenzfälle und Wochenzustände aus echten
  Partien (RefEmu bis Aufruf 8 der Wochen 0–3, je Computerspieler ein Zug).
- **Dateigrenzen:** `src/Traders.Core/Ai/Planting*`, `src/Traders.Core/Field/PlantingAiHelpers*`, `tests/Traders.Tests/Ai/Planting*`,
  `docs/routines/135D_*`, `docs/routines/0609_074D*`, `docs/routines/0609_100F*`.
- **Abnahme:** alle drei Läufe grün; jeder Einsprung und beide Helfer bitgleich mit dem RefEmu; Gegenprobe im Review (Bewertung
  eines Feldes verändert, Reihenfolge der Feldprüfung bzw. Cursorziel vertauscht → rot); `abdeckung.py --check` grün.
- **Abhängigkeiten:** keine (Spur 0 bindet ein).
- **Status:** –

## 2 `planung` – Wochenplanung der Computerspieler `11A3`

- **Teile:** (1) Befund `11A3:0000` mit beiden Durchgängen (Argument 0 vor dem Anbau, 1 vor dem Handel) und allen 15 Routinen
  (`007D`, `0180`, `043F`, `0599`, `061A`, `067D`, `078A`, `0AA5`, `0B3B`, `0D35`, `0E01`, `0EB8`, `0F2E`, `104B`): was je
  Durchgang gelesen und geschrieben wird (u. a. Raster `DS:48F4–49DD`), Zufall in Reihenfolge, Abhängigkeit von Stufe und Woche.
  (2) Port `src/Traders.Core/Ai/WeekPlanning*` als Handler für die Aufrufe 6 und 21 (ohne Bildschirm) mit Differenztests über
  `Call(11A3:0000)` für beide Argumente: Datensegment und `DS:10D2` gleich, alle Stufen, Wochenzustände 0–3 aus echten Partien,
  gemischte Mensch/Computer-Belegung. (3) **Sammelmarker** in `tools/abdeckung.py` (#109: alle Adressen eines Markers zählen,
  Test mit Gegenprobe) und P3 #99 R1/R3 (`0E31:0465` Kommentar, Test `Swapped_draw_order_is_detected` über `NewGame.Start`).
- **Dateigrenzen:** `src/Traders.Core/Ai/WeekPlanning*`, `tests/Traders.Tests/Ai/WeekPlanning*`, `docs/routines/11A3_*`,
  `tools/abdeckung.py` mit Tests (#109), für #99 `src/Traders.Core/Rules/NewGame*`, `tests/Traders.Tests/Rules/NewGame*`,
  `docs/routines/0E31_*`.
- **Abnahme:** alle drei Läufe grün; beide Durchgänge bitgleich mit dem RefEmu; Gegenprobe im Review (Durchgang vertauscht, eine
  Ziehung vertauscht → rot); `abdeckung.py --check` grün, Sammelmarker belegt (z. B. `192D`-Einsprünge nicht mehr 🔍).
- **Abhängigkeiten:** keine.
- **Status:** Teile 1–3 fertig (Claude, PR #123, Base `b80c07d`); Review R5: R5PL-1 (P2) und R5PL-2 (P3) im Fixlauf behoben, Delta-Prüfung Leitstelle offen. Erbe nur noch für Far-Einsprünge des Kopfsegments, Differenztest nie geerbt: 11 Einträge `192D` jetzt 🟢 bis zu eigenem `[DIFFERENTIAL]` (Liste in `MainMenuDifferentialTests.cs`, Spur/Leitstelle `menue`). Restlücke #109: Sammelmarker in anderer Form (`// 1177`, „then 0609:1A61“) zählen weiter nicht – Marker in Listenform bringen (Dateien anderer Spuren).

## 3 `bilanz` – Wochenbilanz `0825:000A` und Leseregel der Szenen

- **Teile:** (1) **Leseregel** in `tools/check-architecture.py` vor den neuen Szenen: schreibende Kernaufrufe aus Szenen auch
  statisch (`KeyTable.Write`, `MainMenu.ApplyLevel`), mit Rückgabewert (`Wealth.Wares`) und als Instanz (`new MainMenu(…).Run()`)
  sowie `FieldCursor.`/`FieldGrid.Set`/`ControlReader.` erkennen (#101 R4M-2, #103 P3-1, #105 P3-1), Rohoffsetregel über
  Hex-Literale hinaus (#105 P3-2); je Regel Gegenprobe; sofort committen, damit 0 sie übernimmt. (2) **Warteschleife und Bilanz
  im Kern** (R5P-1, R5P-3): zuerst die gemeinsame Warteschleife **`21FD:0004`** (gerufen aus `0825:000A` und `0609:00D6`; schreibt
  `DS:2701`, Zufall über `1F63:113E`) mit `0825:0686`, `1F63:113E`/`104B`, `21FD:060D`, `21FD:0665`, `21FD:0590` in
  `src/Traders.Core/Week/WaitLoop*` – sofort committen, Spur 0 ruft sie; `21A7:0054` (Ton) und `0609:1FCC` gerufen bzw. bis R10
  `[APPROX]`. Danach Befund `0825:000A` (Argument, Aufrufer `1777:0208` und der zweite Aufrufer) mit `02D7` (`DS:1584`) in
  `src/Traders.Core/Week/Report*`: Ablauf, Wartezeit, Tasten, jeder Zufall (nur über `21FD:0004`) als Kern-Ereignis je Bild; die
  vorhandenen `Wealth`/`Ranking` (`0813`, `06CB`, `07C4`) werden gerufen. Abbruchweg A (`DS:3D55`) bleibt in der Wochenschleife.
  (3) **Szene** `src/Traders.Presentation/Week/`: die Zeichenroutinen `0825:0072`, `0178`, `0364`, `047D`, `0560`, `09C9` (nur
  Befehlsliste/Schichten, kein Zustand; Laufvariable `DS:37AA` im Befund behandeln), Bilanzbildschirm pixelgenau gegen den RefEmu
  je Bild für Woche 0 und eine spätere Woche, auch Befehlsliste; P3 #101 R4M-1 (`MenuTexts` TAB/Grenze 1194h).
- **Dateigrenzen:** `tools/check-architecture.py` (ganz), `src/Traders.Core/Week/**` (neu), `tests/Traders.Tests/Week/**`,
  `src/Traders.Presentation/Week/**`, `tests/Traders.Tests/Presentation/Week*`, `docs/routines/0825_*` außer `0825_0813*`,
  `docs/routines/{21FD_0004,21FD_060D,21FD_0665,21FD_0590,1F63_113E,1F63_104B}*`, `src/Traders.Core/Rules/Wealth.cs` nur additiv;
  für #101 R4M-1 `src/Traders.Core/Menus/MenuTexts.cs` und `tests/Traders.Tests/Menus/**`.
- **Abnahme:** alle drei Läufe grün; Warteschleife und Bilanz im Gleichschritt mit dem RefEmu (Datensegment, `DS:10D2`,
  Bildzähler), Bild pixelgenau; Gegenprobe im Review (Rangfolge vertauscht, Ziehung in `21FD:0004` ausgelassen → rot; je neue
  Architekturregel eine Verletzung → rot).
- **Abhängigkeiten:** keine; Teil 1 zuerst.
- **Status:** –

## Zuständigkeit gemeinsamer Dateien in R5

`tools/check-architecture.py`: 3. `tools/abdeckung.py`: 2. `src/Traders.Core/Phases/**`, `src/Traders.App/**`,
`OriginalProgram.cs` (additiv): 0; braucht eine andere Spur eine Hilfe in `OriginalProgram.cs`, legt sie sie additiv in eine
eigene `partial`-Datei. `src/Traders.Core/Ai/`: 1 (`Planting*`) und 2 (`WeekPlanning*`). `src/Traders.Core/Field/`: 0, außer `PlantingAiHelpers*`
(`0609:074D`/`100F`): 1. Warteschleife `21FD:0004` mit Helfern (`src/Traders.Core/Week/WaitLoop*`): 3. `src/Traders.Presentation/`: `Field/` 0,
`Week/` 3. `DgroupLayout.cs`, `GameState.cs` (`partial`), `GameStateMapper.cs`: nur additiv (alle), bei Konflikt rebased die
später gemergte Spur. `docs/analyse/DATENLAYOUT.md`: je Spur eigener Unterabschnitt. `STATUS.md`: nur der erzeugte Block (alle).
Übriges `STATUS.md`, `CHANGELOG.md`, dieser Plan, `docs/prozess/**`: Leitstelle. `TradersRedone.slnx`, `Directory.Build.props`,
`global.json`: niemand (kein neues Projekt nötig).

## Integrationsfolge

0 (Teile 1–2), 1, 2, 3 parallel, dateigetrennt. Merge 3 → 2 → 1 → 0 (Leseregel zuerst, damit die Szene von 0 sie erfüllt; 0 nach
Teil 3). Konflikte nur dann über `integration/runde-5`.

## Offene Fragen an den Nutzer

- **G1 Go – erteilt (Nutzer 13.11.2026)** für vier Spuren wie oben, alle Opus, zeitversetzte Starts durch die Leitstelle;
  Plan-Review `reviews/2026-11-13-r5-plan.md` (R5P-1 bis R5P-6 eingearbeitet).
- **F1 Host in R5 – A (Nutzer 13.11.2026):** `Traders.App` zeigt nach dem Menü Wochenbilanz und Anbau spielbar (Tastatur) und
  hält am ersten noch ausstehenden Schritt (Aufruf 9, Wochenereignisse) mit einem Hinweis an (Spur 0 Teil 3); Windows-Handtest
  durch den Nutzer.
- Übrige P3: #99 R1/R3 → 2, R2 mit diesem PR; #101 → 3; #103 P3-1 → 3, P3-2/3/4 → 0, P3-5 mit diesem PR; #105 → 3; #109 → 2;
  #113 → 0; #116 schon auf `main` behoben (`../routines/`), mit diesem PR geschlossen; #75 offen (nächster Lauf an
  `src/Traders.Vga/`), #76 P3-2 mit dem Ton (R10), #49 offen (nächster Lauf an `experiments/bestand/`).

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
