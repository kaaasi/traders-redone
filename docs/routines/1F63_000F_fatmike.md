# 1F63:000F / 1F63:01B2 / 1F63:0305 / 1F63:0390 / 1F63:04D8 / 1F63:064A / 1F63:0711 / 1F63:079A / 1F63:08DA / 1F63:0A8A / 1F63:0C82 / 1F63:0D24 / 1F63:0E5C / 1F63:0E8F / 1F63:0EC7 / 1F63:0F46 / 1F63:0F84 / 1F63:0FDF / 1F63:1185 / 1F63:1247 / 1F63:13FE / 1F63:143E / 1F63:147B / 1F63:104B – Fat Mike (Schutzgeld)

- **Ladebild-Adresse:** Segment `1F63` (Dateioffset `0x22C30`, Größe `0x1500`); `000F` bei `0x22C3F`. Die ersten 15 Bytes sind der
  Schluss der Routine davor. `147B` läuft über das Segmentende hinaus: `pop si`/`mov sp,bp`/`pop bp`/`retf` liegen bei `20B3:0000`.
- **Aufgabe:** Aufruf 26 der Wochenschleife `1777:01F8` (jede Woche). Fat Mike bietet in bis zu drei Runden seinen „Schutz“ zu je
  einem Preis an; bezahlt wird mit Vorrat der Ware 3 (`DS:3CFA`, Schritt 8). Menschen zahlen mit Feuer, solange das Angebot läuft,
  Computer entscheiden in einem festen Bild. Wer am Ende nicht gezahlt hat, verliert auf dem Spielfeld Ernte (Felder werden leer)
  oder – im Wiederholungsfall oder wenn niemand gezahlt hat – Felder. Der Merker `DS:4BC2` hält „hat zuletzt nicht gezahlt“.
- **Aufgerufen von / Phase:** `000F` nur von `1777:01F8` (Aufruf 26). `01B2`, `0C82`, `0D24`, `0E5C`, `0EC7`, `0F46` ruft auch der
  Wettbewerb `1F63:0B03` (Aufruf 30, Teil 2 der Spur); alle übrigen nur aus `000F` und untereinander (`tools/callgraph.py`).
- **Parameter / Rückgabe:** `01B2` und `0C82` → AX = größter Vorrat 3; `0A8A` → AX = Zahl der betroffenen Felder;
  `0E5C(p, i)`, `0E8F(w)`, `0F84(n)`, `0FDF(i)` (Byte), `147B(Preis)`, `104B(Maske, Zustand)`.

## Ablauf (eigene Worte)

- **`000F`:** Bild `186C:0002([08D4])`, `1247`, `21FD:037D(0)`, `0D24`, `54B1` = 0, `21FD:026C(DS:0FFC)`, `13FE`, `21FD:0329`,
  `01B2`, Schreibmaschine `0A69:0288(B1h, 0Ah, 8Fh, DS:3B64, 2, 8, 3)`, `0185(50)`; `random(6)` = 0 → `0305`; `23CD:071C`, `0390`,
  `3CE6` = 4, `54BA[p]` = 0. Runde `54CC` = 0 … 2, solange `3CE6` ≠ 0: `54B8` = `54C2[Runde]`, `147B(54B8)`, Runde 2 → `0F84(1)`,
  Ton (2, 21h/22h/23h), `04D8`. Danach Ton (2, 0), `54C8` = (`3CE6` = 4), `0F84(3CE6 = 0 ? 0 : 2)`, `0185(100)`, `0711`,
  `21AF:0472`, `079A`.
- **`01B2`:** `max` = `0C82`; `54B2` = max/120 + 1 (Einheit der Leiter). Liste leer; für si = 30, 60, 90, 120 zwei Linien
  `0A69:002C` und die Zahl `0A69:011B(si + 0Dh, 0Fh, 54B2·si (imul, cwd), 4, 8, 0, −1, 1)`; je Spieler `0EC7` oder – im
  Wettbewerb (`54B1` = 1) mit `4BC2` = 1 – `0F46`; `21FD:060D`, Ton (0, 24h). Dann für Pixel di = 1 … 120, solange max/54B2 ≥ di:
  Liste leer, je teilnehmendem Spieler mit Vorrat/54B2 ≥ di `0E5C(p, di)`, `21FD:060D`. Ergebnis max.
- **`0305`:** n = `random(3)` (0 … 3); n + 1 mal: v = `random(1)` + 1 → `08ED[i]`; `104B(v, 0)`, Liste, Seitenwechsel, `0185(10)`,
  Seitenwechsel, `104B(v, 1)`, Liste, `0185(20)`.
- **`0390`:** `54B4` = Σ Vorrat 3 / 24 (16-bittig), höchstens 30. Je Runde (`54CC` als Schleifenvariable, danach 3): ab Woche 4
  (`4B9A` ≥ 4) die Art si, die von Runde zu Runde **bleibt**: `random(6)` = 0 → 0, `random(15)` = 0 → 1, `random(20)` = 0 → 2,
  sonst bei `2F05` < 4 (Byte mit Vorzeichen) `random(5)` = 0 → 2. Preis = (`random(5)` + 1)·`54B4`, Art 1 (`random(5)` + 3)·`54B4`,
  Art 2 999; mindestens 10; minus Rest der Division durch `54B2` (idiv). `54D2` = erste Runde mit kleinstem Preis.
- **`04D8`:** Bildzähler si = 0 … 31·`B[1020 + IQ]` (IQ `DS:2E8E`, Werte 6/5/4/2), solange `3CE6` ≠ 0: Liste leer; Runde ≤ 1 →
  Anzeige `113E` (`random(200)`); `0E9E:037A`, `21FD:0245`; je Spieler mit `54BA` ≠ 1: Computer im Bild 31·`B[1020+IQ]` − 20·p →
  `064A`; Aktion 0Ah und Vorrat 3 ≥ Preis (mit Vorzeichen) → Vorrat − Preis, Text `0A69:004D(93h, 21h·p + 2Dh, DS:103C „OK“,
  Farbe B[101C + p], 0, −1, 1)`, Ton (0, 25h), `0EC7`, `0E8F(120 − Vorrat/54B2)`, `54BA[p]` = 1, `3CE6` − 1. Am Bildende
  `15BE` > 0 (Liste hat Einträge) → `1185`, sonst `0185(2)`.
- **`064A`:** Aktion FFh; Spieler der Woche (`4B9A` mod 7 = p) → 0Ah nur in der billigsten Runde (`54CC` = `54D2`); sonst 0Ah bei
  `4BC2` = 1, bei Vorrat 3 ≥ 4·Preis (16-bittiges Schieben), bei `random(5)` = 0, in Runde 0 bei 2·`54B4` ≥ Preis, Runde 1 bei
  4·`54B4` ≥ Preis, Runde 2 immer. Ergebnis in `3308[p]`.
- **`0711`:** gezahlt → `54CE` = 0, `4BC2` = 0; sonst `54CE` = 2 bei `4BC2` = 1 oder `54C8` = 1, sonst 1; `4BC2` = 1; `0A8A` ≤ 0 →
  `54CE` = 0.
- **`0A8A`:** di = `B[4BA8 + p]` − 1 (vorzeichenlos geladen); `54C8` = 1 → di/4; `54CE` = 1 → Σ `2EE7[4p + k]` (k = `45D0` = 0 … 3,
  danach 4) / 4; sonst (di + 1)/5.
- **`079A`:** keiner mit `54CE` ≠ 0 → Ende. Sonst `186C:0002([08CE])`, `21FD:037D(5)`, `21AF:000D`, `21FD:026C(DS:00A8)`,
  `0609:11E7`; bei `54C8` = 1 Hinweisbild (Hintergrund, Rahmen (28h, 4Bh, F0h, 32h), `060D`, `0329`, `0288(28h, 63h, F0h, DS:3B74,
  2, 5, 5)`, `0185(100)`, `037D(5)`); `0609:1BE5`, `0329`, `21FD:05DB(1)`, bei `54C8` = 1 `0288(10h, 13h, E0h, DS:3B94, 3, 5, 5)`;
  `08DA` je Spieler mit `54CE` ≠ 0; `05DB(0)`, `21FD:0004(0)`.
- **`08DA`:** n = `0A8A`; ≤ 0 → Ende. Liste leer, Emblem `0609:1879(F0h, 6, 3)`, `060D`; bei `54C8` = 0 Tafel `0A96:0267(F0h, B0h,
  20h, 16h, 10h, 6, E0h, 2Ch)`, `060D` und `0288(10h, 13h, E0h, DS:37CC + 4·(ECh bzw. EFh), 3, 5, 5)` – ECh bei `54CE` = 2.
  Solange n > 0: `2EB0` = `random(Spalten − 1)`, `2F00` = `random(Zeilen − 1)` (beide Grenzen eingeschlossen); nur eigene
  Felder ohne HQ (Art 4); `54CE` = 2 → `0609:10D2(x, y, −1, −2)`, Ton (0, 27h); sonst nur Felder ≠ leer (−2) →
  `0609:10D2(x, y, p, −2)`, Ton (0, 26h); n − 1, `0609:1CB1(x, y)`, `060D`, `0185(25)`. Ohne passendes Feld zieht das Original
  endlos weiter; mit stimmigen Zählern (`0609:10D2`) kommt das nicht vor.
- **`0C82`:** `4890[p]` = 0 und max über die Teilnehmer (Wettbewerb: ohne `4BC2` = 1), Start 0; bei max > 0 `4890[p]` = 1 für
  Vorrat = max und `4BC2` = 0.
- **Zeichnen:** `0D24` (32 × `0A96:0089`, Hintergrund `0030`, `00E2`, zwei `0267`, 5 + 3 × `00E2`, Liste, `0C3F`), `0E5C`
  (`0030`), `0E8F` (`0267`), `0EC7` (`00E2`, `0A69:011B`), `0F46` (`0A69:004D` mit `DS:3BA0`), `0F84(n)` (`104B(3, 1)` bzw.
  `104B(3, 2)`, bei n = 2 zusätzlich `0FDF(1)`, dann `1185`), `0FDF(i)` (zwei `0030` aus `B[1024+i]`/`B[1026+i]` und
  `W[1028+2i]`/`B[102C+i]`), `1185` (Liste, `23CD:06E9`, `21FD:051F`, `23CD:071C`, Liste).
- **Objekte:** `1247` Ebene (+2) 82h für Objekt 0, 1Eh für 1 … 6, je `21AF:022B` und `21AF:00CA`; `13FE` `54CA` = 94h, `143E`,
  `21AF:0412(i, 0)` für 0 … 6, `06E9`, `051F`, `06E9`; `143E` x = `54CA` + `B[102E+i]`, y = `B[1035+i]`; `147B(Preis)` Ziel =
  Preis/`54B2` + 20, höchstens 100h; je Bild Schritt = Abstand/8, mindestens ±1, höchstens ±4; `143E`, `06E9`, `051F`, `071C`.

## Zustand

| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `3CFA + 8p` | W | s | Vorrat Ware 3 (Zahlungsmittel) | L alle; W `04D8` |
| `4BC2` | B[4] | u | 1 = hat zuletzt nicht gezahlt | L `064A`, `0711`, `0C82`, `01B2`; W `0711` |
| `54B1` | B | u | 0 Fat Mike, 1 Wettbewerb | W `000F` (0), `0B03` (1) |
| `54B2` / `54B4` / `54B8` | W | s | Leitereinheit / Grundpreis / laufender Preis | W `01B2` / `0390` / `0390`, `000F` |
| `54BA` | W[4] | s | 1 = hat gezahlt | W `000F`, `04D8`; L `0711` |
| `54C2` / `54D2` / `54CC` | W[3] / W / W | s | Preise / billigste Runde / Runde (Schleife) | W `0390`, `000F` |
| `54C8` | B | u | 1 = niemand hat gezahlt | W `000F`; L `0711`, `0A8A`, `079A`, `08DA` |
| `54CA` | W | s | x der Objektreihe | W `13FE`, `147B` |
| `54CE` | B[4] | u | Urteil 0 nichts, 1 Ernte weg, 2 Felder weg | W `0711`; L `079A`, `08DA`, `0A8A` |
| `08ED` | B[4] | u | Masken der Blinkfolge `0305` | W `0305` |
| `4890` | B[4] | u | 1 = größter Vorrat 3 | W `0C82` |
| `3CE6` | B | u | Spieler, die noch zahlen können | W `000F`, `04D8` |
| `3308` | B[4] | s | Aktion (0Ah Feuer) | L `04D8`; W `064A` |
| `1020` / `101C` / `103C` | B[IQ] / B[4] / Text | – | Angebotsdauer (6/5/4/2), Textfarbe (0Ch…0Fh), „OK“ | L |
| `102E` / `1035`, `1024`–`102C` | B[7] / B[7], Tabellen | – | Objektversatz, Atlaslagen der Lampen | L `143E`, `0FDF` |
| `330E + 1Ah·i`, `3320`/`3322 + 1Ah·i` | W | s | Objektebene, x, y | W `1247`, `143E` |
| `2EB0` / `2F00`, `45D0`, `37AA` | B / W / B | s | Schleifenvariablen | W |
| `3B64`, `3B74`, `3B94`, `3BA0`, `37CC` | Fernzeiger | – | Texte der Tabelle | L |
| `15BE` | W | s | Zahl der Listeneinträge (`0A96`) | L `04D8` |

## Seiteneffekte in Reihenfolge

Zufall: `000F` 1 × `random(6)`; `0305` 1 × `random(3)` und je Durchlauf `random(1)`; `0390` je Runde ab Woche 4 `random(6)`,
`random(15)`, `random(20)` und ggf. `random(5)`, dann `random(5)` und bei Art 1 noch einmal `random(5)`; `04D8` je Bild in den Runden
0 und 1 die Anzeige `113E` (`random(200)` nur bei abgelaufenem Zähler `54B0`), je Computerentscheid ggf. `random(5)`; `08DA` je
Versuch `random(Spalten − 1)`, `random(Zeilen − 1)`; `079A` die Randziehungen von `0609:13C5` in `1BE5`. Kein Geld (`3CD2`) wird
gebucht. Jedes Warten (Seitenwechsel, `0185`, Blenden, Schreibmaschine, Warteschleife) ist ein Bild.

## C#-Umsetzung

`src/Traders.Core/FatMike/FatMikePhase.cs` (alle 23 Routinen), `FatMikeEvents.cs` (Ereignisse, `IFatMikeHost`), `FatMikeHost.cs`
(Steuerung, Pause, Warteschleife Modus 0, Texte), `FatMikeStep.cs` (Handler für Aufruf 26; eingebunden von Spur `woche`). Gerufen
statt nachgebaut: `1F63:113E` (`Week.WaitLoop.Indicator`, dafür öffentlich), `0609:10D2` (`FieldGrid.Set`), `0609:11E7`/`1BE5`/
`13C5`/`1FCC`-Kern (`Planting`), Schreibmaschine `0A69:0288` (`Harvest.TypeLines`), Blenden, Seitenwechsel, `0185`.
`15BE` > 0 in `04D8`: der Kern zählt die Listeneinträge, die er seit `0A96:001B` angefordert hat (jede Zeichenanforderung legt im
Original mindestens einen Eintrag an); `15BE` selbst gehört der Liste von `Traders.Vga`. `104B`, `1879`, `0A69:002C/004D/011B` und
die `0A96`-Befehle sind Zeichenereignisse für die Szene.

**Szene (Teil 3):** `src/Traders.Presentation/FatMike/FatMikeScene.cs` spielt die Ereignisse über `Traders.Vga` ab; gemeinsame
Anforderungen (Bild, Objekte, Paletten, Blenden, Seiten, Liste, Wartebilder, Spielfeld der Strafe) über eine `FieldScene` auf
denselben Ebenen, eigene als Einträge der Befehlsliste `0A96` (`0089`, `00E2`, `0267`, `0030`, `0A69:002C/004D/011B`, `0525`),
Schrift `21FD:05DB`, Schreibmaschine `0A69:01AD` (Lage aus `0288`/`0243`), `21AF:0472`. Neu belegt aus dem Disassembly:
**`1F63:104B(Maske, Zustand)`** – je Bit 0 (x `D0h`, Atlas-x `38h`) bzw. Bit 1 (x `F4h`, Atlas-x `58h`) ein `0A96:0030`; Zustand
(Low-Byte) 1: Atlas (x, `6Ah`) `20h`·`28h` nach y `32h`, 2: Atlas (Atlas-x, `43h`) `20h`·`28h` nach y `32h`, sonst Atlas (Atlas-x,
`6Bh`) `20h`·`1Fh` nach y `3Bh`. Liest und schreibt kein `DS`; keine neuen `[APPROX]`.

## Prüfung

`tests/Traders.Tests/FatMike/FatMikeDifferentialTests.cs` gegen den RefEmu (Harness `FatMikeHarness.cs`: Zeichnen, Ton, Steuerung,
Pause und Warteschleife ersetzt und mitgeschrieben, die Listenzahl `15BE` wie im Original geführt): `0390` (Woche < 4 und ≥ 4,
Mindestpreis 10, 999, Einheit bis 400), `0C82` (Gleichstand, Wettbewerb), `01B2` mit `0E5C`/`0EC7`/`0F46`, `064A`, `0711` mit
`0A8A` (alle drei Urteile), `04D8` mit Skript (Menschen und Computer, IQ 0–3, Zahlung bei Vorrat knapp über/unter dem Preis),
`0305`, `0D24`, `0F84`, `1247`, `13FE`, `147B` (auch negative und große Preise), `08DA`/`079A` auf stimmigen Feldern, `000F` ganz aus
LoadOnly-Zuständen und aus echten Partien in den Wochen 0–3 (Menschen per Skript, `4BC2` und Vorräte verändert). Verglichen:
Datensegment, `DS:10D2`, Zahl der `random()`-Aufrufe, Aufruffolge mit Argumenten.

Bildgenau und Bildzähler: `tests/Traders.Tests/FatMike/FatMikeFrameDifferentialTests.cs` (Harness `FatMikeFrameHarness.cs`):
`FatMikeStep` (Aufruf 26) in der Phasenmaschine im Gleichschritt mit dem Original einer echten Partie (Schnappschuss am Einsprung,
lebende Grafik, Tasten als Interrupts) – je Rücklauf des Originals ein Bild des Ports; dort Datensegment, `DS:10D2`, Zahl der
`random()`-Aufrufe, Rücklaufwarten der Szene und Bild (vier VRAM-Ebenen, DAC, CRTC-Start), bei der Rückkehr erneut. Fälle: Woche 0
vier Computer; Woche 1 zahlende Menschen mit Blinken `0305` (Startwert gewählt); Woche 3 ein Mensch bestraft (Ernte vernichtet
bzw. Felder verloren, Brett, Wappen, Tafel, Textzeilen, Warteschleife Modus 0); Woche 3 niemand zahlt (Warnbild `3B74`, Hinweis
`3B94`, vier Strafen). Machbarkeitsprüfung der Strafe im Testaufbau (`08DA` endet nur bei erreichbarer Zahl). Gegenproben (rot):
Anzeige `104B` ausgelassen, ein getipptes Zeichen ausgelassen, Vorrat ohne Schatten, ein Bild doppelt verarbeitet.
