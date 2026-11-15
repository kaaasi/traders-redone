# Arbeitspakete

Die Portierung läuft in **Runden**; je Runde arbeiten mehrere **Spuren** parallel, jede auf einem Branch `paket/<spur>` mit
einem PR, eigenen Dateien und klarer Abnahme. Regeln: [AGENTS.md](../AGENTS.md), Ablauf:
[prozess/zusammenarbeit.md](prozess/zusammenarbeit.md). Den Status pflegt jede Spur nur in ihrem eigenen Abschnitt; Plan-
abschnitte ändert die Leitstelle. Abgeschlossene Runden stehen in [archiv/](archiv/) (Paketphase AP1–AP5: [archiv/runde-0.md](archiv/runde-0.md), Runde 1: [archiv/runde-1.md](archiv/runde-1.md), Runde 2: [archiv/runde-2.md](archiv/runde-2.md), Runde 3: [archiv/runde-3.md](archiv/runde-3.md), Runde 4: [archiv/runde-4.md](archiv/runde-4.md), Runde 5: [archiv/runde-5.md](archiv/runde-5.md)).

## Aktuelle Runde: 6 – Katastrophen, Wochenereignisse, Ernte (Plan Leitstelle 15.11.2026, Go nötig)

**Ziel:** Die Wochenschritte 4, 9 und 16 laufen im Port statt im RefEmu: Katastrophen `111D:0009` mit Sternvorführung
(`V-BERGE`, Aufruf 4, ab Woche 3 bei eingeschaltetem Schalter `DS:2F30`; der Wächter ist seit R5 portiert), Wochenereignisse
`17CB:0005` (Aufruf 9) und Ernte zeigen und einsammeln `22A3:02D3` (Aufruf 16). Wetter `21FD:021F` (12), Produktion `22A3:0003`
(13) und Schwund `22A3:026B` (14) liegen seit R3 vor. Danach hält der Host erst an den Stationen `1B03` (Aufruf 11, R7).
Ergebnis: M6 (Produktion & Wetter) ✅ und M10 (Ereignisse & Katastrophen) ✅. Dazu die Pflichten aus R5: Leseregel über
Methodenparameter (#139), Sammelmarker ohne Listenform (#140), schneller Test `ReportPictureStep` (#135), Nachholung im Host
(#113); nebenbei #76 P3-1 (Architekturregel Rückrichtung) und #131 (Befund `0DFD_000D`).

**Leitstelle R6:** Claude (Sitzung vom 15.11.2026). **Modell:** alle Spuren und Erstreviews Claude Opus (Spielkern, Zufall,
Disassembly, Differenztests; `zusammenarbeit.md` §5 „Modellwahl“). **Base:** `main` `15fd3bd`. Namen der Läufe: `Traders R6 <spur>`.
**Starts zeitversetzt** (Nutzungslimit): 1 und 2 sofort, 0 eine Stunde später.

**Gemeinsame Vorgaben für alle drei Spuren** (wie R5):

- Branch `paket/<spur>` von aktuellem `main`, ein Draft-PR, Teile als Aufgabenliste (`spur-start.md` „Teile und Fortsetzung“).
- Je portierter Originalroutine Befund in `docs/routines/` (Sammelbefund erlaubt; Sammelmarker nur in Listenform
  `[ORIGINAL] XXXX:YYYY, XXXX:YYYY`, sonst je Adresse ein Marker), `[DIFFERENTIAL]`-Test gegen den RefEmu mit Grenzfällen und
  Zufallszustand `DS:10D2` danach; Erwartungswerte nie aus dem Port. Teil 1 jeder Spur stellt die transitiv gerufenen, noch nicht
  portierten Routinen fest (`tools/callgraph.py`, Liste der Leitstelle unten nachprüfen) und schreibt sie in den Befund; was einer
  anderen Spur gehört, wird gerufen, nicht nachgebaut.
- **Kern und Szene getrennt** (`AGENTS.md` §4): Ablauf, Zustandsänderungen, Buchungen, Zufall (auch jede Ziehung, die nur ein Bild
  wählt, z. B. Sternpositionen) im Kern als Kern-Ereignis je Bild; Szenen in `src/Traders.Presentation/` lesen nur und spielen die
  Zeichenereignisse ab (Muster `FieldEvent`/`WeekEvent` aus R5). Eingaben über die Befehlsquelle `0E9E`; Spielerhandlungen als
  Befehle. Warteschleife `21FD:0004` (R5) wird gerufen, nicht nachgebaut.
- **Ton** `21A7:0054`/`000D` (mit `218B:013A`/`0153`, `27C6:041B`, `2850:00CC`): nur als Ereignis bzw. `[APPROX]` bis R10, wie R5.
- **Regelrelevante Pixel:** Prüft eine Routine Pixel von `V-BERGE` oder `V-ANBAU` für eine Entscheidung, nutzt der Kern den
  kanonischen Original-Atlas (`AGENTS.md` §4); im Befund belegen, ob das der Fall ist.
- Schritt-Handler liefern die Spuren 1 und 2 als Klassen in ihren Dateien; **eingebunden in die Phasenmaschine werden sie nur von
  Spur 0 Teil 3** (`Phases/**` gehört 0). Bis dahin laufen ihre Differenztests über `LoadOnly`/`Call` bzw. das Wochenorakel.
- Nur VGA; Regeln auf `Dgroup` mit Offsets aus `DgroupLayout.cs` (nur additiv); `GameState`/`GameStateMapper.cs` nur additiv
  (`partial`). Neue DGROUP-Adressen mit Beleg in `docs/analyse/DATENLAYOUT.md` (je Spur eigener Unterabschnitt).
- `STATUS.md`: nur der erzeugte Block per `tools/abdeckung.py --write`; nach jedem Merge Abgleichlauf der nächsten Spur (Merge
  von `main`, `--write`, Label `leitstelle-delta`).

**Noch offene Routinen laut Aufrufgraph** (Leitstelle, `tools/callgraph.py` auf `T.EXE`, transitiv ab dem Einsprung bis zu
portierten Routinen): `111D`: `0112`, `0264`, `03B7`, `0519`, `0757`, `07E2` und `23CD:06DE` (aus `0112`) · `17CB`: `001E`, `02BD`,
`039D`, `047E`, `04D7`, `0736`, `0986`, `09E0` · `22A3`: `0340`, `0426`, `04B1`, `04DA`, `04F7`, `062E`, `07C2`, `0806`, `082F`,
`0874`, `08C1`. Schon portiert und gerufen u. a.: `0609:10D2` (Feldart), `0609:1BE5`/`13C5`/`1F2D`/`11E7`/`1AC0`/`1FCC`/`21BA`,
`0825:06CB`, `0E31:0361`, `22A3:0750`, `2295:0001`/`006D`, `21FD:0004`/`0185`/`05DB`, `0E9E:037A`, Grafikschichten `0A96`/`0A69`.

## 0 `ernte` – Ernte zeigen und einsammeln `22A3:02D3`, Einbindung der Wochenschritte

- **Teile:** (1) **Ernte im Kern** (`src/Traders.Core/Harvest/**`): Befund `22A3:02D3` mit `0340`, `0426`, `04B1`, `04DA`, `04F7`,
  `062E` und den Zeichenhelfern `07C2`, `0806`, `082F`, `0874`, `08C1` (Kern- und Zeichenteil je Routine belegen und trennen):
  Ablauf je Spieler und Feld, Einsammeln in die Vorräte, Raster `DS:48F4` (Ergebnis von `22A3:0003`), `DS:3CE6`, `DS:45D0`,
  Schleifenvariablen `2EB0`/`2F00`/`37AA`, Eingabe über `0E9E:037A` (Abbruch/Weiter), jedes Bild als Kern-Ereignis. Die
  vorhandenen `0609`-Routinen aus R5 (`1BE5` mit `13C5`, `1F2D`, `11E7`, `1AC0`, `1FCC`, `21BA`) werden gerufen; der „Ernte-Teil“
  von `13C5` (Aufrufer außerhalb des Anbaus) wird hier im Befund geklärt. Differenztests über `LoadOnly`/`Call(22A3:02D3)` mit
  Wochenzuständen aus echten Partien (Wochen 0–3, Mensch und Computer, volle und leere Felder, Vorräte an der Obergrenze).
  (2) **Szene** `src/Traders.Presentation/Harvest/`: Zeichenteil aller Ernte-Routinen, pixelgenau gegen den RefEmu je Bild über
  eine Ernte mit Mensch, auch Befehlsliste (`0A96`/`0A69`). (3) **Einbindung** (nach Merge von 1 und 2): Handler für Aufruf 4
  (Rest hinter dem Wächter: `CatastropheGuardStep(rest)` mit dem Handler der Spur 1), 9 (Spur 2) und 16 in
  `WeekSteps`/`PhaseMachine`; **Mehrrundenprobe** über ≥ 5 Wochen (damit Aufruf 4 ab Woche 3 läuft) mit einem Menschen und drei
  Computerspielern, Katastrophen eingeschaltet, in der die Aufrufe 3, 4, 6, 8, 9, 16, 21 im Port laufen (Aufruf 11 `1B03`, 18 und
  die Handelsschritte weiter aus dem Orakel; Vergleich Datensegment und `DS:10D2` je Aufrufstelle, Bild für Bild für 4, 9, 16);
  Host nach F1: spielt Bilanz, Katastrophen, Anbau und Wochenereignisse, Halt mit Hinweis an Aufruf 11. P3 #135 (schneller Test
  `ReportPictureStep`) und #113 (Nachholgrenze 17–18 im Test, doppelte Nachholschleife in `HostGame.Update`).
- **Dateigrenzen:** `src/Traders.Core/Harvest/**`, `tests/Traders.Tests/Harvest/**`, `src/Traders.Presentation/Harvest/**`,
  `tests/Traders.Tests/Presentation/Harvest*`, `docs/routines/22A3_*` außer `22A3_0003*`/`22A3_026B*`, `docs/routines/0609_0C73*`
  (nur Nachtrag `13C5`), ab Teil 3 `src/Traders.Core/Phases/**`, `tests/Traders.Tests/Phases/**`, `src/Traders.App/**`,
  `tests/Traders.Tests/App/**`, `src/Traders.RefEmu/OriginalProgram.cs` (additiv, Hilfen für Bildfolgen).
- **Abnahme:** alle drei Läufe grün; Ernte im Gleichschritt mit dem RefEmu (Datensegment, `DS:10D2`, Bildzähler) für Woche 0 und
  eine spätere Woche, je mit Mensch und Computer; Szene pixelgenau; Gegenprobe im Review (Ertrag eines Feldes doppelt eingesammelt,
  Vorratsgrenze ausgelassen, ein Bild doppelt verarbeitet → rot); Mehrrundenprobe ≥ 5 Wochen; `check-architecture.py` grün.
- **Abhängigkeiten:** Teile 1–2 sofort; Teil 3 nach Merge von 1 und 2 (sonst `spur-wartet`, „wartet auf 1/2“).
- **Status:** –

## 1 `katastrophen` – Katastrophen und Sternvorführung `111D:0009`

- **Teile:** (1) **Befund und Kern** (`src/Traders.Core/Disasters/**`): Befund `111D:0009` (Nachtrag zum Wächter-Befund
  `111D_0009_katastrophen.md`) mit `0112` (Vorführung, Ziehungen 319/150), den drei Katastrophenarten `0264`, `03B7`, `0519`
  (Wahl über `random(10)`/`random(2)` in `0009`), Meldefenster `0757`, `07E2` und `23CD:06DE`: Auswahl, Wirkung auf Felder
  (`0609:10D2`, Bodengüte, Feldart, Rücksetzen über `0E31:0361`), betroffene Spieler, Zufall in Reihenfolge, Bildfolge mit
  Seitenwechseln `21FD:051F`/`0185`/`0245` und Warteschleife `21FD:0004(3)`. Handler als Rest hinter `CatastropheGuardStep`
  (Vertrag `IWeekStepHandler`, im Befund festhalten). Differenztests über `LoadOnly`/`Call(111D:0009)` mit Zuständen, in denen
  **jede** Katastrophenart und der Fall „keine“ eintritt (Zufallszustand gezielt setzen), Wochen 3–5, Mensch und Computer.
  (2) **Szene** `src/Traders.Presentation/Disasters/`: Bildteil (`V-ANBAU`/`V-BERGE`, Paletten, Fenster, Text), pixelgenau gegen
  den RefEmu je Bild für jede Katastrophenart, auch Befehlsliste. (3) Kleinigkeiten: #140 (Sammelmarker in `MainMenu.cs` in
  Listenform, `192D:1177` im erzeugten Block) und #131 (Befund `0DFD_000D_programmstart.md` nachziehen).
- **Dateigrenzen:** `src/Traders.Core/Disasters/**`, `tests/Traders.Tests/Disasters/**`, `src/Traders.Presentation/Disasters/**`,
  `tests/Traders.Tests/Presentation/Disasters*`, `docs/routines/111D_*`, `docs/routines/23CD_06DE*`; für #140
  `src/Traders.Core/Menus/MainMenu.cs` (nur Marker), für #131 `docs/routines/0DFD_000D*`.
- **Abnahme:** alle drei Läufe grün; jede Katastrophenart im Gleichschritt mit dem RefEmu (Datensegment, `DS:10D2`, Bildzähler),
  Szene pixelgenau; Gegenprobe im Review (Katastrophenart vertauscht, eine Ziehung der Vorführung ausgelassen, falsches Feld
  getroffen → rot); `abdeckung.py --check` grün, `192D:1177` gezählt.
- **Abhängigkeiten:** keine (Spur 0 bindet ein).
- **Status:** –

## 2 `ereignisse` – Wochenereignisse `17CB:0005` und Leseregel

- **Teile:** (1) **Leseregel** in `tools/check-architecture.py` vor den neuen Szenen: schreibende Kernaufrufe über
  Methodenparameter erkennen (#139, Probe aus dem Gesamtreview R5 als Test) und Rückrichtung für alle Projekte (#76 P3-1); je
  Regel Gegenprobe; sofort committen. (2) **Befund und Kern** (`src/Traders.Core/Events/**`): Befund `17CB:0005` mit `001E`
  (Geldereignisse, Ziehungen 3/9/…/100), `04D7` und `0736` (Feldereignisse, `0609:10D2`, `22A3:0750`), `02BD`/`039D` (Gewinn/
  Verlust), `047E`, Fenster `0986`, `09E0`; Reihenfolge der drei Ereignisgruppen, betroffene Spieler (Rang über `0825:06CB`),
  Buchungen auf Bargeld und Felder, Zufall in Reihenfolge, Warteschleife `21FD:0004(0)` und `21FD:05DB`. Handler für Aufruf 9 (ohne
  Einbindung). Differenztests über `LoadOnly`/`Call(17CB:0005)` mit Zuständen, in denen jede Ereignisart und „kein Ereignis“
  eintritt, Wochen 0–3, Mensch und Computer, Geldgrenzen (16-Bit, kein Geld). (3) **Szene** `src/Traders.Presentation/Events/`:
  Fenster und Texte, pixelgenau gegen den RefEmu je Bild, auch Befehlsliste.
- **Dateigrenzen:** `tools/check-architecture.py` (ganz) mit Tests, `src/Traders.Core/Events/**`, `tests/Traders.Tests/Events/**`,
  `src/Traders.Presentation/Events/**`, `tests/Traders.Tests/Presentation/Events*`, `docs/routines/17CB_*`.
- **Abnahme:** alle drei Läufe grün; jede Ereignisart im Gleichschritt mit dem RefEmu (Datensegment, `DS:10D2`, Bildzähler), Szene
  pixelgenau; Gegenprobe im Review (Ereignisgruppen vertauscht, Betrag mit Vorzeichenfehler, eine Ziehung ausgelassen → rot; je
  neue Architekturregel eine Verletzung → rot).
- **Abhängigkeiten:** keine; Teil 1 zuerst.
- **Status:** –

## Zuständigkeit gemeinsamer Dateien in R6

`tools/check-architecture.py`: 2. `tools/abdeckung.py`: niemand (Sammelmarker in Listenform zählen schon). `src/Traders.Core/Phases/**`,
`src/Traders.App/**`, `OriginalProgram.cs` (additiv): 0; braucht eine andere Spur eine Hilfe in `OriginalProgram.cs`, legt sie sie
additiv in eine eigene `partial`-Datei. `src/Traders.Core/`: `Harvest/` 0, `Disasters/` 1, `Events/` 2, `Menus/MainMenu.cs` 1 (nur
Marker). `src/Traders.Presentation/`: `Harvest/` 0, `Disasters/` 1, `Events/` 2. Vorhandene `Field/`-, `Week/`- und `Rules/`-Klassen
werden gerufen, nicht geändert; braucht eine Spur dort eine Änderung, meldet sie `spur-wartet` mit Begründung. `DgroupLayout.cs`,
`GameState.cs` (`partial`), `GameStateMapper.cs`: nur additiv (alle), bei Konflikt rebased die später gemergte Spur.
`docs/analyse/DATENLAYOUT.md`: je Spur eigener Unterabschnitt. `STATUS.md`: nur der erzeugte Block (alle). Übriges `STATUS.md`,
`CHANGELOG.md`, dieser Plan, `docs/prozess/**`, `docs/vorlagen/**`: Leitstelle. `TradersRedone.slnx`, `Directory.Build.props`,
`global.json`: niemand.

## Integrationsfolge R6

2 → 1 → 0 (Teil 3 von 0 erst nach Merge von 1 und 2). Gesamtreview am Rundenende (ChatGPT/Codex), danach Abschluss M6/M10.

## Offene Fragen an den Nutzer (R6)

- **G1:** Go für Runde 6 mit diesem Plan (drei Spuren, alle Opus)?
- **F1:** Die Ernte (Aufruf 16) liegt hinter den Stationen (Aufruf 11, R7). **A (Vorschlag):** Ernte läuft in R6 nur in Tests und
  der Mehrrundenprobe (Aufruf 11 aus dem Orakel); der Host hält an Aufruf 11 und zeigt die Ernte erst ab R7. **B:** Stationen
  `1B03` (0x1650 Byte, 44 Routinen) aus R7 als vierte Spur vorziehen, damit der Host in R6 bis zur Ernte spielt; R6 wird deutlich
  größer und Spur 0 Teil 3 wartet zusätzlich auf diese Spur.

Offen und nicht in R6: P3 #49, #75, #76 (P3-2/-3). #121 und #142 behebt der Plan-PR.

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
