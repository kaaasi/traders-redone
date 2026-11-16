# Runde 6 – Katastrophen, Wochenereignisse, Stationen, Ernte (Plan Leitstelle 15.11.2026, F1 = B, Go Nutzer 15.11.2026)

**Ziel:** Die Wochenschritte 4, 9, 11 und 16 laufen im Port statt im RefEmu: Katastrophen `111D:0009` mit Sternvorführung
(`V-BERGE`, Aufruf 4, ab Woche 3 bei eingeschaltetem Schalter `DS:2F30`; der Wächter ist seit R5 portiert), Wochenereignisse
`17CB:0005` (Aufruf 9), Stationen, Raketenflug und Piraten `1B03:0009` (Aufruf 11, nach F1 = B aus R7 vorgezogen) und Ernte
zeigen und einsammeln `22A3:02D3` (Aufruf 16). Wetter `21FD:021F` (12), Produktion `22A3:0003` (13) und Schwund `22A3:026B` (14)
liegen seit R3 vor. Danach spielt der Host die Woche bis einschließlich Ernte und hält an der Roboterphase `1C68` (Aufruf 18).
Ergebnis: M6 (Produktion & Wetter) ✅, M9 (Rakete & Stationen / Piraten) ✅ und M10 (Ereignisse & Katastrophen) ✅. Dazu die Pflichten aus R5: Leseregel über
Methodenparameter (#139), Sammelmarker ohne Listenform (#140), schneller Test `ReportPictureStep` (#135), Nachholung im Host
(#113); nebenbei #76 P3-1 (Architekturregel Rückrichtung) und #131 (Befund `0DFD_000D`).

**Leitstelle R6:** Claude (Sitzung vom 15.11.2026). **Modell:** alle Spuren und Erstreviews Claude Opus (Spielkern, Zufall,
Disassembly, Differenztests; `zusammenarbeit.md` §5 „Modellwahl“). **Base:** `main` `15fd3bd`. Namen der Läufe: `Traders R6 <spur>`.
**Starts zeitversetzt** (Nutzungslimit): 1 und 2 sofort (1 legt in Teil 1 zuerst die Regelseite an), 3 und 0 eine Stunde später.

**Gemeinsame Vorgaben für alle vier Spuren** (wie R5):

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
- **Regelrelevante Pixel – Regelseite des Kerns** (R6P-1): Liest das Original Pixel der Zeichenseite für eine Entscheidung
  (`23CD:06DE` = Sprung auf den portierten Pixel-Leser `23CD:3764`; gerufen aus `111D:0112` und `1B03:08ED`/`0BE7`/`1178`; davon
  hängt z. B. die Zahl der Ziehungen in `0112` ab), führt der Kern eine eigene **Regelseite** (`src/Traders.Core/Pixels/**`,
  indizierte 320 × 200-Seite mit `Get(x, y)`): Startbild aus den Bildpixeln (`V-BERGE`, `V-RAKETE`, `V-ANBAU`), die der Kern als
  explizite Eingabe bekommt (`Func<string, IndexedImage>` o. ä., gespeist von Host und Tests aus `Traders.Assets`, wie
  `Func<int, ImageFileLengths>`), plus genau die Zeichenschritte, die vor einem Lesen die gelesenen Pixel ändern (Sterne,
  Verschiebung, Objekte; im Befund belegt). `Traders.Core` bekommt **keine** Projektreferenz. `23CD:06DE`/`3764` werden nicht neu
  portiert. **Besitz:** Spur 1 legt die Regelseite in Teil 1 als Erstes an und committet sie sofort; Spur 3 nutzt sie (Ergänzungen
  nur additiv in eigener Datei `Pixels/Stations*`). Differenztest: Regelseite an jeder Lesestelle gleich dem VRAM der Zeichenseite
  im RefEmu (an den gelesenen Stellen).
- Schritt-Handler liefern die Spuren 1, 2 und 3 als Klassen in ihren Dateien; **eingebunden in die Phasenmaschine werden sie nur von
  Spur 0 Teil 3** (`Phases/**` gehört 0). Bis dahin laufen ihre Differenztests über `LoadOnly`/`Call` bzw. das Wochenorakel.
- Nur VGA; Regeln auf `Dgroup` mit Offsets aus `DgroupLayout.cs` (nur additiv); `GameState`/`GameStateMapper.cs` nur additiv
  (`partial`). Neue DGROUP-Adressen mit Beleg in `docs/analyse/DATENLAYOUT.md` (je Spur eigener Unterabschnitt).
- `STATUS.md`: nur der erzeugte Block per `tools/abdeckung.py --write`; nach jedem Merge Abgleichlauf der nächsten Spur (Merge
  von `main`, `--write`, Label `leitstelle-delta`).

**Noch offene Routinen laut Aufrufgraph** (Leitstelle, `tools/callgraph.py` auf `T.EXE`, transitiv ab dem Einsprung bis zu
portierten Routinen): `111D`: `0112`, `0264`, `03B7`, `0519`, `0757`, `07E2` (`23CD:06DE` ist der portierte Pixel-Leser `3764`, R6P-1) · `17CB`: `001E`, `02BD`,
`039D`, `047E`, `04D7`, `0736`, `0986`, `09E0` · `22A3`: `0340`, `0426`, `04B1`, `04DA`, `04F7`, `062E`, `07C2`, `0806`, `082F`,
`0874`, `08C1` · `1B03`: `0009`, `0079`, `00D4`, `0101`, `0153`, `01C3`, `0201`, `02C0`, `0413`, `04D4`, `0548`, `05E7`, `0632`,
`0705`, `0740`, `0763`, `07C4`, `0849`, `08ED`, `0A62`, `0B7A`, `0BE7`, `0C89`, `0CAC`, `0CCF`, `0D75`, `0DC0`, `0E11`, `0EA9`,
`10EA`, `1178`, `1257`, `128D`, `12D7`, `131B`, `1345`, `139D`, `13C9`, `13EE`, `141D`, `1454`, `148B`, `15D7`, `1617` (Pixel-Leser
`23CD:06DE` über die Regelseite). Schon portiert und gerufen u. a.: `0609:10D2` (Feldart), `0609:1BE5`/`13C5`/`1F2D`/`11E7`/`1AC0`/`1FCC`/`21BA`,
`0825:06CB`, `0E31:0361`/`023D`, `21AF:00CA`/`022B`/`0412`/`045E`, `0B67:01D4`/`0206`, `0E9E:000E`, `22A3:0750`, `2295:0001`/`006D`, `21FD:0004`/`0185`/`05DB`, `0E9E:037A`, Grafikschichten `0A96`/`0A69`.

## 0 `ernte` – Ernte zeigen und einsammeln `22A3:02D3`, Einbindung der Wochenschritte

- **Teile:** (1) **Ernte im Kern** (`src/Traders.Core/Harvest/**`): Befund `22A3:02D3` mit `0340`, `0426`, `04B1`, `04DA`, `04F7`,
  `062E` und den Zeichenhelfern `07C2`, `0806`, `082F`, `0874`, `08C1` (Kern- und Zeichenteil je Routine belegen und trennen):
  Ablauf je Spieler und Feld, Einsammeln in die Vorräte, Raster `DS:48F4` (Ergebnis von `22A3:0003`), `DS:3CE6`, `DS:45D0`,
  Schleifenvariablen `2EB0`/`2F00`/`37AA`, Eingabe über `0E9E:037A` (Abbruch/Weiter), jedes Bild als Kern-Ereignis. Die
  vorhandenen `0609`-Routinen aus R5 (`1BE5` mit `13C5`, `1F2D`, `11E7`, `1AC0`, `1FCC`, `21BA`) werden gerufen; `13C5` erreicht die
  Ernte nur über `1BE5` wie der Anbau (R6P-4): Nachtrag „Aufruf über `1BE5` aus `22A3:02D3`“ in `0609_0004_anbau.md`. Differenztests über `LoadOnly`/`Call(22A3:02D3)` mit
  Wochenzuständen aus echten Partien (Wochen 0–3, Mensch und Computer, volle und leere Felder, Vorräte an der Obergrenze).
  (2) **Szene** `src/Traders.Presentation/Harvest/`: Zeichenteil aller Ernte-Routinen, pixelgenau gegen den RefEmu je Bild über
  eine Ernte mit Mensch, auch Befehlsliste (`0A96`/`0A69`). (3) **Einbindung** (nach Merge von 1, 2 und 3): Handler für Aufruf 4
  (Rest hinter dem Wächter: `CatastropheGuardStep(rest)` mit dem Handler der Spur 1), 9 (Spur 2), 11 (Spur 3) und 16 in
  `WeekSteps`/`PhaseMachine`; **Mehrrundenprobe** über ≥ 5 Wochen (damit Aufruf 4 ab Woche 3 läuft) mit einem Menschen und drei
  Computerspielern, Katastrophen eingeschaltet, in der die Aufrufe 3, 4, 6, 8, 9, 11, 16, 21 im Port laufen (Aufruf 18 `1C68`, 19
  und die Handelsschritte weiter aus dem Orakel; Vergleich Datensegment und `DS:10D2` je Aufrufstelle, Bild für Bild für 4, 9,
  11, 16); Host nach F1 = B: spielt die Woche von der Bilanz bis zur Ernte, Halt mit Hinweis an Aufruf 18. P3 #135 (schneller Test
  `ReportPictureStep`); #113 geht an den Fixlauf `fix/anbau-figuren`.
- **Dateigrenzen:** `src/Traders.Core/Harvest/**`, `tests/Traders.Tests/Harvest/**`, `src/Traders.Presentation/Harvest/**`,
  `tests/Traders.Tests/Presentation/Harvest*`, `docs/routines/22A3_*` außer `22A3_0003*`/`22A3_026B*`, `docs/routines/0609_0004*`
  (nur Nachtrag `13C5`), ab Teil 3 `src/Traders.Core/Phases/**`, `tests/Traders.Tests/Phases/**`, `src/Traders.App/**`,
  `tests/Traders.Tests/App/**`, `src/Traders.RefEmu/OriginalProgram.cs` (additiv, Hilfen für Bildfolgen).
- **Abnahme:** alle drei Läufe grün; Ernte im Gleichschritt mit dem RefEmu (Datensegment, `DS:10D2`, Bildzähler) für Woche 0 und
  eine spätere Woche, je mit Mensch und Computer; Szene pixelgenau; Gegenprobe im Review (Ertrag eines Feldes doppelt eingesammelt,
  Vorratsgrenze ausgelassen, ein Bild doppelt verarbeitet → rot); Mehrrundenprobe ≥ 5 Wochen, in der nachweislich (Assert) mindestens ein Lauf von `111D:0112` mit einer Katastrophenart im Port
  verglichen wird (R6P-2: im R5-Tastenmuster Katastrophe in Woche 4; Woche bzw. Tastenmuster im Test festlegen); `check-architecture.py` grün.
- **Abhängigkeiten:** Teile 1–2 sofort; Teil 3 nach Merge von 1, 2 und 3 (sonst `spur-wartet`, „wartet auf 1/2/3“).
- **Status:** Alle Teile fertig (PR #152, Claude): Teil 1 Kern `22A3:02D3` mit Differenztests, Teil 2 Szene je Bild pixelgenau; Teil 3 Einbindung der Aufrufe 4, 9, 11, 16 (`Phases/WeekScreens.cs`), Mehrrundenprobe Wochen 0–4 mit Mensch und Katastrophe (Woche 4) Bild für Bild, Host bis zur Ernte mit Halt an Aufruf 18; #135, #164, #170 mitgenommen (Fortsetzung 1, 16.11.2026). Fixlauf R6E-1/R6E-2 (#175, #176): schneller Test `WeekScreens.Incidents` mit Betrag `2295:0001` in Woche 2, Kommentare „4, 9, 11“ (16.11.2026). **Gemergt 16.11.2026** (Review `reviews/2026-11-16-r6-ernte.md` „Nein“ → Fixlauf R6E-1/-2, Delta-Prüfung Leitstelle).

## 1 `katastrophen` – Katastrophen und Sternvorführung `111D:0009`

- **Teile:** (1) **Befund und Kern** (`src/Traders.Core/Disasters/**`): Befund `111D:0009` (Nachtrag zum Wächter-Befund
  `111D_0009_katastrophen.md`) mit `0112` (Vorführung, Ziehungen 319/150), den drei Katastrophenarten `0264`, `03B7`, `0519`
  (Wahl über `random(10)`/`random(2)` in `0009`), Meldefenster `0757`, `07E2`; zuerst die **Regelseite** (gemeinsame Vorgaben) als **eigener kleiner PR** `paket/regelseite` (Teil 1a, R6N-1: Seitenmodell,
  Bildeingabe, Differenztest an den Lesestellen von `0112`; sofort Ready for review, die Leitstelle legt das Erstreview an und
  merged vor allen anderen), danach auf `paket/katastrophen`: Auswahl, Wirkung auf Felder
  (`0609:10D2`, Bodengüte, Feldart, Rücksetzen über `0E31:0361`), betroffene Spieler, Zufall in Reihenfolge, Bildfolge mit
  Seitenwechseln `21FD:051F`/`0185`/`0245` und Warteschleife `21FD:0004(3)`. Handler als Rest hinter `CatastropheGuardStep`
  (Vertrag `IWeekStepHandler`, im Befund festhalten). Differenztests über `LoadOnly`/`Call(111D:0009)` mit Zuständen, in denen
  **jede** Katastrophenart und der Fall „keine“ eintritt (Zufallszustand gezielt setzen), Wochen 3–5, Mensch und Computer.
  (2) **Szene** `src/Traders.Presentation/Disasters/`: Bildteil (`V-ANBAU`/`V-BERGE`, Paletten, Fenster, Text), pixelgenau gegen
  den RefEmu je Bild für jede Katastrophenart, auch Befehlsliste. (3) Kleinigkeiten: #140 (Sammelmarker in `MainMenu.cs` in
  Listenform, `192D:1177` im erzeugten Block) und #131 (Befund `0DFD_000D_programmstart.md` nachziehen).
- **Dateigrenzen:** `src/Traders.Core/Disasters/**`, `tests/Traders.Tests/Disasters/**`, `src/Traders.Presentation/Disasters/**`,
  `tests/Traders.Tests/Presentation/Disasters*`, `docs/routines/111D_*`, `src/Traders.Core/Pixels/**` (außer `Pixels/Stations*`),
  `tests/Traders.Tests/Pixels/**`; für #140
  `src/Traders.Core/Menus/MainMenu.cs` (nur Marker), für #131 `docs/routines/0DFD_000D*`.
- **Abnahme:** alle drei Läufe grün; jede Katastrophenart im Gleichschritt mit dem RefEmu (Datensegment, `DS:10D2`, Bildzähler),
  Szene pixelgenau; Gegenprobe im Review (Katastrophenart vertauscht, eine Ziehung der Vorführung ausgelassen, falsches Feld
  getroffen → rot); `abdeckung.py --check` grün, `192D:1177` gezählt.
- **Abhängigkeiten:** keine (Spur 0 bindet ein).
- **Status:** Alle Teile fertig, #153 Ready for review: Teil 1a (Regelseite, #149) gemergt; Teil 1 Kern mit Sternvorführung auf der Regelseite, getippten Zeilen und Bildfolge im Gleichschritt mit dem RefEmu in laufenden Partien (Wochen 3–5, Computer und Mensch, keine/Art 0/1/2); Teil 2 Szene `DisasterScene` pixelgenau je Bild (VRAM, DAC, CRTC, Befehlsliste); Teil 3 fertig (Claude, Fortsetzung 2, 15.11.2026). Mitgenommen: #155, #156. **Gemergt 15.11.2026** (Review `reviews/2026-11-15-r6-katastrophen.md` „Ja“, Fixlauf nur Merge von `main`); offen P3 #165, #166.

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
- **Status:** PR #146 (Claude), alle Teile fertig, Review offen. Teil 1 ✅ (Leseregel #139, Rückrichtung #76 P3-1 mit Gegenproben), Teil 2 ✅ (Kern `17CB:0005`, 22 Differenzszenarien, Mehrrundenprobe), Teil 3 ✅ (Szene `IncidentScene` pixelgenau in 7 Szenarien; Schreibmaschine `0A69:0288` mit Takt und Betrag `2295:0001` im Kern). **Gemergt 15.11.2026** (Review `reviews/2026-11-15-r6-ereignisse.md` „Ja“); offen P3 #162–#164 (#164 mit #152 erledigt).

## 3 `stationen` – Stationswahl, Raketenflug, Piraten, Stationsertrag `1B03:0009` (F1 = B)

- **Teile:** (1) **Befund und Kern I** (`src/Traders.Core/Stations/**`): Befund `1B03:0009` mit allen 44 Routinen (Liste oben):
  Aufbau (`V-RAKETE`), Stationswahl je Spieler (Mensch als Befehl über `0E9E`, Computer), Reihenfolge der Spieler, Zufall in
  Reihenfolge (u. a. `0079` 11/2/3, `0101` 6, `01C3` 2, `0849` 3/299, `0A62` 10, `0B7A` 5000, `0EA9` 20/30, `1178` 319/199/2),
  Kern- und Zeichenteil je Routine belegen und trennen; Stationswahl und -ertrag (Buchungen auf Geld, Vorräte, Felder über
  `0609:10D2`) im Kern mit Differenztests über `LoadOnly`/`Call`. (2) **Kern II:** Raketenflug und Piraten Bild für Bild als
  Kern-Ereignisse – Bewegung, Eingabe, Kollision gegen `V-RAKETE` (Regelseite des Kerns `Pixels/`), Sichtbarkeit, Piratenangriff mit
  Folgen; Gleichschritt mit dem RefEmu (Datensegment, `DS:10D2`, Bildzähler) für Woche 0 und eine spätere Woche, je mit Mensch und
  Computer, Grenzfälle (kein Geld, Rakete getroffen, Zeitablauf). (3) **Szene** `src/Traders.Presentation/Stations/`:
  Zeichenteil aller Routinen (Objekte `21AF`, Seiten `0B67`, Schichten `0A96`/`0A69`), pixelgenau gegen den RefEmu je Bild über
  eine Phase mit Mensch, auch Befehlsliste. Handler für Aufruf 11 (ohne Einbindung).
- **Dateigrenzen:** `src/Traders.Core/Stations/**`, `src/Traders.Core/Pixels/Stations*`, `tests/Traders.Tests/Stations/**`, `src/Traders.Presentation/Stations/**`,
  `tests/Traders.Tests/Presentation/Stations*`, `docs/routines/1B03_*`.
- **Abnahme:** alle drei Läufe grün; Phase im Gleichschritt mit dem RefEmu, Szene pixelgenau; Gegenprobe im Review (Station eines
  Spielers vertauscht, Kollision um ein Pixel verschoben, eine Ziehung ausgelassen, Ertrag doppelt gebucht → rot);
  `check-architecture.py` grün.
- **Abhängigkeiten:** Regelseite (Spur 1 Teil 1a, eigener PR) – Teil 1 braucht sie schon für `1178` (R6N-1): bis zu deren
  Merge zuerst die pixelfreien Routinen, nie Dateien aus `Pixels/**` außer `Pixels/Stations*` übernehmen; ist sonst nichts mehr
  zu tun, `spur-wartet` mit „wartet auf Regelseite“.
  Größte Spur der Runde (0x1650 Byte); bis zu zwei Fortsetzungen über das Limit von 3 (F2 = ja).
- **Status:** Teile 1–3 fertig (PR #151): Befund `1B03_0009_stationen.md` (44 Routinen), Kern aller Routinen; `1178`, `08ED`, `0BE7` über die Regelseite (`Pixels/StationsPage.cs`), ganze Phase `0009` im Bildgleichschritt mit dem RefEmu (16 Zufallszustände, Standardpartie Woche 0–2); Szene `Presentation/Stations/StationScene.cs` je Rücklauf pixelgenau (VRAM, DAC, Start, Befehlsliste; 12 Zustände mit Menschen, Woche 0), Handler `StationsStep` für Aufruf 11 (nicht eingebunden), tickt je Bild des Originals (Fixlauf R6S-1). **Gemergt 16.11.2026** (Review „Nein“ R6S-1 P1 → Fixlauf → Nachprüfung `reviews/2026-11-16-r6-stationen-nachpruefung.md` „Ja“).

## Fixlauf `fix/anbau-figuren` – #147 (P2) aus dem Windows-Handtest R5, mit #113

- **Auftrag:** Im Host zeigt die Anbauphase die bewegten Spielfiguren (Objekte `21AF`) als schmale senkrechte Farbstreifen
  (#147, `main` `95d6e8d`); `PlantingFrameHarness` ist Bild für Bild gleich, der Host-Test vergleicht nur an Aufrufstelle 9.
  Erst reproduzieren: roter Regressionstest auf dem Host-Pfad (`ProgramHost` Bild für Bild durch den Anbau von Woche 0 gegen den
  RefEmu: VRAM, DAC, CRTC-Start je Rücklauf, mit Mensch), dann beheben, Testlücke schließen. Dazu P3 #113 (Nachholgrenze 17–18
  im Test, doppelte Nachholschleife in `HostGame.Update`).
- **Dateigrenzen:** `src/Traders.App/**`, `tests/Traders.Tests/App/**`, `src/Traders.Presentation/Field/**`,
  `tests/Traders.Tests/Presentation/Field*`, `src/Traders.Core/Field/**` und `tests/Traders.Tests/Field/**` (nur wenn der Fehler
  dort belegt ist). Überschneidung mit R6: `Traders.App` gehört Spur 0 erst ab Teil 3 – der Fixlauf wird vorher gemergt, sonst
  rebased Spur 0; `Field/**` ändert keine R6-Spur.
- **Modell:** Claude Opus. **Abnahme:** neuer Test vor dem Fix rot, danach grün; alle drei Läufe grün (Testinfrastruktur);
  Gegenprobe im Review (Fehler wieder eingebaut → rot). **Status:** PR #150 (Claude), **gemergt 15.11.2026** (Review
  `reviews/2026-11-15-fix-anbau-figuren.md` „Ja“): Ursache – Heap-Zeiger von Atlas und Masken (`DS:45D6`/`4BC6`) im Host nie gesetzt;
  Host-Test Bild für Bild durch den Anbau (vor dem Fix rot in Bild 136); #113 erledigt. Offen P3 #157, #158.

## Zuständigkeit gemeinsamer Dateien in R6

`tools/check-architecture.py`: 2. `tools/abdeckung.py`: niemand (Sammelmarker in Listenform zählen schon). `src/Traders.Core/Phases/**`,
`src/Traders.App/**`, `OriginalProgram.cs` (additiv): 0; braucht eine andere Spur eine Hilfe in `OriginalProgram.cs`, legt sie sie
additiv in eine eigene `partial`-Datei; fehlt das Schlüsselwort `partial` (`OriginalProgram`, `GameState`), setzt es die erste
Spur, die es braucht, als einzige Änderung an der Datei (R6P-3). `src/Traders.Core/`: `Harvest/` 0, `Disasters/` 1, `Pixels/` 1 (außer `Pixels/Stations*`: 3), `Events/` 2, `Stations/` 3,
`Menus/MainMenu.cs` 1 (nur Marker). `src/Traders.Presentation/`: `Harvest/` 0, `Disasters/` 1, `Events/` 2, `Stations/` 3. Vorhandene `Field/`-, `Week/`- und `Rules/`-Klassen
werden gerufen, nicht geändert; braucht eine Spur dort eine Änderung, meldet sie `spur-wartet` mit Begründung. `DgroupLayout.cs`,
`GameState.cs` (`partial`), `GameStateMapper.cs`: nur additiv (alle), bei Konflikt rebased die später gemergte Spur.
`docs/analyse/DATENLAYOUT.md`: je Spur eigener Unterabschnitt. `STATUS.md`: nur der erzeugte Block (alle). Übriges `STATUS.md`,
`CHANGELOG.md`, dieser Plan, `docs/prozess/**`, `docs/vorlagen/**`: Leitstelle. `TradersRedone.slnx`, `Directory.Build.props`,
`global.json`: niemand.

## Integrationsfolge R6

Regelseite (1a) → 2 → 1 → 3 → 0 (1a so früh wie möglich; Teil 3 von 0 erst nach Merge von 1, 2 und 3). Gesamtreview am Rundenende (ChatGPT/Codex), danach Abschluss
M6/M9/M10.

## Entscheidungen des Nutzers (R6)

- **G1 = ja** (Nutzer 15.11.2026): Go für Runde 6 mit diesem Plan (vier Spuren, alle Opus).
- **F1 = B** (Nutzer 15.11.2026): Stationen `1B03` aus R7 vorgezogen (Spur 3), Host spielt bis zur Ernte.
- **F2 = ja** (Nutzer 15.11.2026): Spur 3 `stationen` darf bis zu zwei Fortsetzungen über das Limit von 3 hinaus bekommen.

Offen und nicht in R6: P3 #49, #75, #76 (P3-2/-3). #121 und #142 behebt der Plan-PR.

## Abschluss (16.11.2026)

- Alle Spuren gemergt: Regelseite #149 (Spur 1 Teil 1a), 2 `ereignisse` #146, 1 `katastrophen` #153, 3 `stationen` #151,
  0 `ernte` #152 (Folge Regelseite → 2 → 1 → 3 → 0 wie geplant). Fix #147 (Spielfiguren im Host, #150), Plan #143/#148,
  CI-Zeitlimit #172 (#167), Status #177. `main` am Ende der Spuren `ebaddee`.
- Reviews: Plan `reviews/2026-11-15-r6-plan.md` (Nein → R6P-1–4) und `…-plan-nachpruefung.md` („Ja“, R6N-1/-2 eingearbeitet);
  Erstreviews `…-r6-regelseite.md` („Ja“), `…-fix-anbau-figuren.md` („Ja“), `…-r6-ereignisse.md` („Ja“), `…-r6-katastrophen.md`
  („Ja“), `…-r6-stationen.md` (Nein → R6S-1 P1, Nachprüfung `2026-11-16-r6-stationen-nachpruefung.md` „Ja“),
  `2026-11-16-r6-ernte.md` (Nein → R6E-1 P2, Delta-Prüfung Leitstelle), `…-ci-timeout.md` („Ja“).
- Gesamtreview Codex [reviews/2026-11-16-r6-gesamt.md](../reviews/2026-11-16-r6-gesamt.md) (Zwischenstand) und
  [Teil 2](../reviews/2026-11-16-r6-gesamt-teil2.md) „Ja“: keine Fehler; Testlücke R6G-T1 (#180). M6, M9, M10 abgeschlossen.
- Entscheidungen: G1 Go, F1 = B (Stationen `1B03` aus R7 vorgezogen, Host bis zur Ernte), F2 = ja (Spur 3 bis 5 Fortsetzungen,
  verbraucht: 2). Fixlauf #147 aus dem Windows-Handtest R5 übernommen.
- Geschlossen: #113, #121, #131, #135, #139, #140, #142, #147, #155, #156, #164, #167, #169, #170, #175, #176. Offen als P3:
  #49, #75, #76 (P3-2/-3), #157, #158, #162, #163, #165, #166, #180.
- Erfahrungen: Regelrelevante Pixel brauchen eine Regelseite im Kern (R6P-1) – früh als eigener kleiner PR (R6N-1) hat sich
  bewährt. Handler müssen je Bild takten; die Abnahme „Bildzähler“ im Review gezielt prüfen (R6S-1). `--force-with-lease` wurde
  in manchen Läufen abgelehnt, Merge von `main` war der Ausweg. CI-Läufe näherten sich dem Zeitlimit (#167).

