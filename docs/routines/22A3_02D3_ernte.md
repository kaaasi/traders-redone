# 22A3:02D3 / 22A3:0340 / 22A3:0426 / 22A3:04B1 / 22A3:04DA / 22A3:04F7 / 22A3:062E / 22A3:07C2 / 22A3:0806 / 22A3:082F / 22A3:0874 / 22A3:08C1 – Ernte zeigen und einsammeln

- **Ladebild-Adresse:** Segment `22A3` (Dateioffset `0x26030`); `02D3` bei `0x26303`. `08C1` läuft über das Segmentende
  hinaus: die letzten Bytes (`add sp,8`, `21FD:060D`, Rücksprung) liegen bei `2336:0000` (Disassemblierung `2336.asm`).
- **Aufgabe:** Ernte der Woche (Aufruf 16 der Wochenschleife `1777:01F8`): Spielfeld wie im Anbau aufbauen, Wetterhinweis
  tippen, je Spieler die Anzeige (Rang, Zeitleiste, vier Waren mit Vorrat), dann läuft der Cursor jedes Spielers Feld für Feld
  (Spalte innen, Zeile außen) zu den eigenen Feldern mit Ertrag (`DS:48F4` aus `22A3:0003`), ein Balken wächst Bild für Bild, der
  laufende Vorrat zählt mit, am Ende wird der ganze Ertrag in den Vorrat gebucht (höchstens 999).
- **Aufgerufen von / Phase:** `02D3` nur von `1777:01F8` (Aufruf 16); alle übrigen nur aus `02D3` und untereinander.
- **Parameter / Rückgabe:** keine, außer `082F(Wert)`.

## Ablauf (eigene Worte)

- **`02D3`:** `186C:0002([08CE])`, `0609:1F2D`, `21FD:037D(7)`, `0609:1BE5`, `3CE6` = 4, `21FD:026C(DS:00A8)`, `0609:11E7`,
  `21FD:0329`, `08C1`, `0340`, `0A96:001B`, Ton (0, 17h), `0426`, Ton (0, 0).
- **`08C1`:** `21FD:05DB(1)`, `0A69:0288(0, 14h, 140h, DS:3BA4 + 12·Wetter, 3, 9, 5)` (Wetter Byte mit Vorzeichen, 16 Bit),
  `21FD:05DB(0)`, `21FD:0185(50h)`, `0A96:001B`, `0609:1F04(10h, 6, 120h, 2Bh)`, `21FD:060D`.
- **`0A69:0288`/`0243`/`01AD`** (Bildfolge; Zeichnen in `Traders.Vga.Layers.DrawHelpers`): Zeile i = 0 … Anzahl − 1 (mit
  Vorzeichen) aus dem Fernzeigerfeld; Nullzeiger überspringen; je Zeichen (Länge per `strlen`, vorzeichenlos) `[0140]` = Zeichen,
  `0A96:001B`, `0A96:0525` (Text), `21FD:060D`, Ton (1, 16h) außer bei Leerzeichen, `21FD:0245`, `21FD:0185(Warten)` (`cwd`);
  Schlusswarten −1 (keins). Die Zeilen sind Textzeilen F5h + 3·Wetter (0-basiert wie `tools/callgraph.py`, Puffer `DS:4884`).
- **`0340`:** je Spieler (`37AA`): `0A96:001B`, `0609:1854(p·40h + 20h, 6)`, `0609:1AC0`, `0609:1FCC`, `3786[p]` (Doppelwort) = 0,
  `0609:18E5`; für `45D0` = 0 … 3 (Wort, mit Vorzeichen) `0609:1912(k, p·40h + 20h, 8k + 0Dh)` und `0874`; dann Cursor x = −1,
  y = 0, `04F7`, `57E8[p]` = 0, `57F4[p]` = (`0840[p]` = 1), `21FD:060D`. `45D0` bleibt 4.
- **`0426`:** solange `3CE6` ≠ 0: `21FD:051F`, `21FD:0185(1)`, `23CD:071C([082E])`, `0A96:06A8`, `0A96:001B`, `0E9E:037A`,
  `21FD:0245`; je Spieler nach Modus `43B2` (Byte vorzeichenlos) 7 → `04B1`, 8 → `062E`; `0A96:06A8`, `23CD:06E9([082E])`.
  Danach `21FD:0665`.
- **`04B1`:** Aktion `3308[p]` = 0 (rechts) oder `57F4[p]` = 1 → `04F7`; dann `04DA`.
- **`04DA`:** Aktion = 0Ah (Feuer) → `57F4[p]` = 1 (ab dann ohne Taste).
- **`04F7`:** **einmal** `0806`; dann Schleife: Cursor x + 1 (Byte); > Spalten − 1 (cbw, 16 Bit mit Vorzeichen) → x = 0, y + 1;
  > Zeilen − 1 → `0609:21BA`, `3CE6` − 1, Modus 6, Ende. Sonst `2EB0`/`2F00` = Cursor; Besitzer `4442[x·6 + y]` ≠ `37AA`
  (Byte) oder Ertrag `48F4[x·1Ah + y]` = 0 → weiter; sonst Objekt-x `3320 + 1Ah·p` = (x << 5) + `451E` + 8, Objekt-y `3322` =
  y · 16h + `4528` + 0Ch, `3786[p]` = 7D0h (Doppelwort), Modus 8, Ende. Der Sprung zurück geht hinter den Aufruf von `0806`.
- **`062E`:** `04DA`; `2EB0`/`2F00` = Cursor; `45D0` = Art `2EB1[x·6 + y]` (cbw); `57E8[p]` + 1; ist es ≤ Ertrag / 2 (Ertrag
  vorzeichenlos, `idiv`; Vergleich mit Vorzeichen) → `07C2`, `082F(Vorrat[p][45D0] + 2·57E8[p])`; sonst Vorrat
  `3CF4[8p + 2·45D0]` + Ertrag, > 999 (mit Vorzeichen) → 999, `0874`, `57E8[p]` = 0, Modus 7.
- **Zeichenhelfer:** `07C2` = `0A96:0433(p·40h + 46h, 2Ah − 57E8[p], x + 3, gleich, Wort [C8h + 2p])` (Linie); `0806` =
  `0A96:04D7(p·40h + 46h, 0Ch, 4, 1Eh, 5)` (Rechteck); `082F(v)` = `0A96:057A(p·40h + 32h, 8·45D0 + 12h, min(v, 999) als
  Doppelwort, 3, 2, 5)`; `0874` = dasselbe mit dem Vorrat und Farbe 0Dh.

## Kern und Zeichnen

| Routine | Kern (Zustand, Zufall) | Zeichnen/Ausgabe |
|---|---|---|
| `02D3`, `0340`, `0426`, `04B1`, `04DA`, `04F7`, `062E` | ganz | Aufrufe als Ereignisse |
| `08C1` | Bildfolge (`0185(50h)`, `060D`) | `05DB`, `1F04`, Liste |
| `0A69:0288`/`0243`/`01AD` | Bildfolge je Zeichen (Länge, Leerzeichen → kein Ton, Pause, Warten) | Lage (`3CA8`, `34A4`), Zeichen `DS:0140`, Text `0525` |
| `07C2`, `0806`, `082F`, `0874` | – (nur Lesen; `082F` begrenzt nur die Anzeige) | ganz |
| `0609:1F2D`, `1BE5` mit `13C5`, `11E7`, `1FCC`, `21BA` | wie im Anbau (R5, [0609_0004_anbau.md](0609_0004_anbau.md)) | wie im Anbau |

## Zustand

| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `57E8` | W[4] | s | Bilder der laufenden Sammlung (Balkenhöhe) | r/w |
| `57F4` | B[4] | u | 1 = ohne Taste weiter (Computer oder nach Feuer) | r/w |
| `48F4` | B[9·26] | u | Ertrag je Feld (`22A3:0003`) | r |
| `3CF4` | W[4][4] | s | Vorrat | r/w |
| `45D0` | W | s | Ware/Art (Schleife in `0340`, Art in `062E`) | w |
| `43B2` | B[4] | u | Modus: 7 warten, 8 sammeln, 6 fertig | r/w |
| `304E`/`305A` | B[4] | s | Cursor x/y | r/w |
| `3CE6` | B | u | Spieler noch auf dem Feld | r/w |
| `2EB0`/`2F00` | B | s | Feld unter dem Cursor (globale Schleifenvariablen) | w |
| `3786` | D[4] | – | Bewegungsverzögerung der Steuerung (`0E9E`) | w |
| `330C` (+0, +14h, +16h, +18h) | Sätze 1Ah | – | Cursorobjekte (Kernteil wie im Anbau) | w |
| `3BA4` | Fernzeiger[6][3] | – | Wetterhinweis (Textzeilen) | r |
| `00C8` | W[4] | – | Farbe des Spielers (Balken) | r |
| `0840`, `3308`, `4442`, `2EB1`, `08FC`/`08FE`, `451E`/`4528`, `40E6`, `08CE` | | | wie im Anbau | r |

Neue Adressen in [DATENLAYOUT.md](../analyse/DATENLAYOUT.md) §15.

## Seiteneffekte in Reihenfolge

- **Zufall:** nur `13C5` beim Aufbau (2 · max(0, Spalten) `random(1)`), sonst keiner.
- **Grafik/Ton:** alle Aufrufe als `FieldEvent` (gemeinsam mit dem Anbau) bzw. `HarvestEvent` (`HarvestEventKind`: `SetFont`,
  `TypeLines`, `TypeCharacter`, `TileArea`, `DrawEmptyPanel`, `DrawGoods`, `Number`, `Line`, `FillRect`) in Originalfolge.
  `DS:0140` (getipptes Zeichen) und die Schriftmaße von `05DB` gehören der Ausgabe.
- **Eingabe:** `0E9E:037A` und `21FD:0245` über `IHarvestHost`; Texte des Hinweises als explizite Eingabe
  (`IHarvestHost.FarString`, im Spiel aus dem Textpuffer des Programmstarts).
- **Zeit:** keine Uhr; Bilder nur an Rückläufen (`051F`, `0185`, Blenden) und Abfragen der Pause.

## Bildfolge (VGA `DS:090E` = 1)

| Stelle | Rückläufe |
|---|---|
| `21FD:037D(7)` | 8 × (1 + 4) = 40 |
| `0609:1BE5` | bei VGA keiner |
| `21FD:0329` | 8 × (1 + 3) = 32 |
| `08C1` | je Zeichen `060D` (`051F`) + Pause + `0185(5)`; dann `0185(50h)`, `060D` |
| `0340` | je Spieler `060D` |
| `0426` je Durchgang | `051F`, `0185(1)`, Pause |
| `21FD:0665` | `051F` |

`21FD:051F`, `0185` und `0245` wie im Anbau ([0609_0004_anbau.md](0609_0004_anbau.md) „Bildfolge“).

## Transitiv gerufen, noch nicht portiert (`tools/callgraph.py`, Stand `e04742b`, bis zu portierten Routinen)

`22A3`: `0340 0426 04B1 04DA 04F7 062E 07C2 0806 082F 0874 08C1` (diese Spur) · Ton `21A7:0054`/`000D`, `218B:013A`/`0153`,
`27C6:041B`, `2850:00CC` (Ereignis bis R10). Gerufen und schon portiert: `0609:1F2D`, `1BE5`, `11E7`, `1854`, `18E5`, `1912`,
`1AC0`, `1F04`, `1FCC`, `21BA`, `0A69:0288`, `0A96:001B`/`0433`/`04D7`/`057A`/`06A8`, `0E9E:037A`, `186C:0002`, `21FD:0185`/`0245`/
`026C`/`0329`/`037D`/`051F`/`05DB`/`060D`/`0665`, `23CD:06E9`/`071C`.

## C#-Umsetzung

`src/Traders.Core/Harvest/Harvest.cs`: `Run` = `02D3` als Bildfolge, `NoticeFrames` = `08C1`, `TypeLines` = Bildfolge von
`0A69:0288`/`0243`/`01AD`, `PanelsFrames` = `0340`, `CollectFrames` = `0426`, `PassFrames`/`Pass` = ein Durchgang, `Advance` =
`04B1`, `CheckAutomatic` = `04DA`, `NextField` = `04F7`, `Collect` = `062E`, `Bar` = `07C2`, `ClearBar` = `0806`,
`ShowRunningStock` = `082F`, `ShowStock` = `0874`. `HarvestEvents.cs`: Ereignisse und `IHarvestHost`. Aufbau, Blenden,
Seitenwechsel, Cursorobjekte über `Planting` (R5). `HarvestStep` (Aufruf 16 als `IWeekStepHandler`, wie `PlantingStep`) wird
in Teil 3 in die Phasenmaschine eingebunden.
`HarvestHost.cs`: Spielfassung des Wirts (Steuerung `ControlReader.ReadActions`, Pause `Planting.Pause`, Texte über einen
Lesezugriff auf den Textpuffer).

**Szene (Teil 2):** `src/Traders.Presentation/Harvest/HarvestScene.cs` spielt die gemeinsamen Ereignisse über eine `FieldScene`
auf denselben Schichten (Aufbau, Rang, Zeitleiste, Objekte, Blenden, Seiten, Warten) und die eigenen: `SetFont` = `21FD:05DB`
(wie `MenuScene`), `TypeLines`/`TypeCharacter` = Zeichnen von `0A69:0288`/`0243`/`01AD` (Zeile i bei y + i·(`34A4` + `34A4`/5),
mittig x + 1 + (w − Länge·`3CA8`)/2 vorzeichenlos geschoben, Zeichen k bei + k·`3CA8`, `DS:0140` = Zeichen, `0A96:0525`),
`TileArea` = `0609:1F04`, `DrawEmptyPanel` = `0609:18E5`, `DrawGoods` = `0609:1912`, `Number`/`Line`/`FillRect` =
`0A96:057A`/`0433`/`04D7`.

## Prüfung

`tests/Traders.Tests/Harvest/HarvestDifferentialTests.cs` (RefEmu `LoadOnly` bzw. Schnappschuss einer echten Partie am
Einsprung von `02D3`, direkte Aufrufe; Zeichen-, Ton- und Ausgaberoutinen durch `retf` ersetzt und mit Argumenten
aufgezeichnet, `0609`-Routinen, `21AF:0412` und die Schreibmaschine `0A69` laufen im Original). Verglichen: ganzes DS (ohne
Ausgabebytes), `10D2`, Zahl der `random`-Aufrufe, Aufruffolge. `04F7` 3000 Fälle (fertig/sammeln je > 100), `062E` 4000 (Balken,
Buchung, Grenze 999 je > 100), `04B1`/`04DA` je 2000, `0340` und `08C1` je 600 (Nullzeiger, leere Zeilen, Leerzeichen, Wetter
−2…7), `0426` Durchgang für Durchgang an `22A3:04A3` in 150 Läufen mit Tastenskript, `02D3` ganz 120 Läufe, dazu echte Partien:
Wochen 0–3 je sechs Läufe (Computer wie gespielt, Spieler zu Menschen gemacht, Vorräte nahe 999, Felder und Erträge verändert).

**Gleichschritt Bild für Bild** (`tests/Traders.Tests/Harvest/HarvestFrameDifferentialTests.cs`, Harness
`HarvestFrameHarness`, Muster `PlantingFrameHarness`): Original ab Schnappschuss am Einsprung von `02D3` (Woche 0 bzw. 2, vier
Computer) mit echter Grafik und Tastatur-Interrupts; Port `HarvestStep` in der Phasenmaschine ab Aufruf 16 mit `HarvestHost` und
`HarvestScene`. An jedem Rücklauf des Originals: DS (ohne Ausgabebytes), `10D2`, Zahl der Ziehungen, VRAM-Ebenen, DAC,
CRTC-Start; ebenso am Rücksprung (Maschine danach blockiert an Aufruf 18, 17 ist Marker 3). Läufe: vier Computer Woche 0,
Mensch mit „rechts“ und Feuer und Vorräten bei 990 (Woche 0), zwei Menschen in Woche 2. Gegenproben (je rot): Balkenlinie
ausgelassen, ein getipptes Zeichen ausgelassen (beide am Bild), ein Bild doppelt verarbeitet.

## Einbindung in die Woche (Teil 3, R6)

`src/Traders.Core/Phases/WeekScreens.cs` bindet die Bildschirmschritte der Runde 6 in `WeekSteps`: Aufruf 4 hinter dem Wächter
(`CatastropheGuardStep(rest)` mit `CatastropheStep`), 9 (`WeekIncidentsStep`), 11 (`StationsStep`), 16 (`HarvestStep`); erster
ausstehender Aufruf ist danach 18 (`1C68:000B`). Die Schritte entstehen je Aufruf neu (`DeferredStep`), weil ihre Eingaben vom
Zeitpunkt abhängen: Regelseite auf der Zeichenseite `DS:40DE[DS:082E]`, Bild `DS:08D0`, Schrift des Treibers, Textpuffer
`DS:4884` (eine gemeinsame Kopie; `2295:0001` schreibt die Beträge der Ereignisse im Kern, die Zahlen der Stationen als
Ausgabeaufruf über die Schichten hinein). Seitenzustand `DS:082E/0830`, `40DE…40E5`, `4516` gehört den Ausgabeschichten; der
Host gibt ihn zu Beginn jedes Aufrufs als Eingabe (`WeekScreenInputs.OutputPages`, `[APPROX]`).

**Mehrrundenprobe** `tests/Traders.Tests/Phases/MultiWeekRound6DifferentialTests.cs`: Standardpartie, Spieler 0 Mensch
(Steuerung 4, R5-Tastenmuster), Katastrophen an, Wochen 0–4 und Aufruf 1 von Woche 5. Aufrufe 3, 4, 8, 9, 11, 16 Bild für Bild
(je Rücklauf DS ohne Ausgabebytes des Schritts, `10D2`; das Original rechnet sein Bild zuerst, damit ein `time()`-Wert für
`random(0)` bekannt ist), 6, 21 und Regelschritte am Rücksprung, 18, 19, 23–30 im RefEmu. Textpuffer nach 4, 9, 11, 16 gleich
(nach RefEmu-Schritten übernommen). Belegt (Lauf auf diesem Stand): Katastrophe Art 0 mit Sternvorführung `0112` in Woche 4
(1402 Bilder im Port), Wochenereignis in Woche 3 (1096 Bilder), Stationen und Ernte jede Woche mit Handlungen des Menschen.
Gegenprobe (lokal): ein Bild von Aufruf 4 doppelt verarbeitet → rot in Woche 4 (`DS:09C8`, Rücklauf 727).

**Host** (`src/Traders.App/ProgramHost.cs`): spielt Woche 0 bis zur Ernte mit allen Szenen und hält an Aufruf 18;
`HostPlantingFrameDifferentialTests` Bild für Bild gegen den RefEmu (VRAM, DAC, CRTC-Start, `10D2` je Rücklauf; Bericht und
Ereignisse nur an ihren Darstellungsroutinen, siehe deren Harness), `ProgramHostTests` DS und Bild an der Aufrufstelle von 18.
Gegenprobe (lokal): `HarvestScene.NextFrame` im Host ausgelassen → rot (DAC in der Ernte). Schneller Test für den Bildeintrag
von Aufruf 3: `ReportPictureStepTests.cs` (#135).
