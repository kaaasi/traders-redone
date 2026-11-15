# 17CB:0005 / 17CB:001E / 17CB:02BD / 17CB:039D / 17CB:047E / 17CB:04D7 / 17CB:0736 / 17CB:0986 / 17CB:09E0 – Wochenereignisse (Spur `ereignisse`, R6)

- **Ladebild-Adresse:** `17CB:0005` (Segment `17CB`, Dateioffset `0x1B2B0`, Größe `0xA10`); Unterroutinen `001E`, `02BD`,
  `039D`, `047E`, `04D7`, `0736`, `0986`, `09E0`.
- **Aufgabe:** Ereignisse der Woche: Feldübernahme, Geldgewinn oder -verlust, Ernteausfall – jeweils mit eigenem Fenster und
  Warteschleife.
- **Aufgerufen von / Phase:** Wochenschleife `1777:023B` (Aufruf 9), Rücksprung `1777:0240`.
- **Parameter / Rückgabe:** keine (alle Routinen ohne Parameter außer `02BD`/`039D(Betrag: long)`, `047E(Betrag: long)` → AL,
  `09E0(far Zeiger auf Texttabelleneintrag, Zeilen, Farbe)`).

## Ablauf (eigene Worte)

`0005`: Rangfolge `0825:06CB`, dann **`04D7` → `001E` → `0736`** (feste Reihenfolge, jede Gruppe höchstens ein Ereignis).

`random(n)` liefert 0…n (`21FD:01BA`), `random(0)` setzt den Seed aus der Uhr.

**`001E` Geld:** p = `random(3)` → `DS:37AA`. Ist `RankUnique[p]` ≥ 2 (vorzeichenlos):
- si = erster Spieler mit `RankUnique` = `RankUnique[p]` − 1 (Wortvergleich; ohne Treffer 4).
- Gewinn = LDIV(LXMUL(3, `2F06[si]` − `2F06[p]`), 4); ist 2·`2F06[p]` > `2F06[si]` (32 Bit vorzeichenbehaftet), muss
  `random(9)` = 0 sein. Nur Gewinn > 0: Fenster `0986`, `02BD(Gewinn)`, `21FD:0004(0)`, `21FD:05DB(0)`.

Sonst (Rang 0 oder 1) Verlust gegen den Nächstschlechteren si (`RankUnique` + 1):
- Ist 2·`2F06[si]` > `2F06[p]`, muss `random(Rank[p]·5 + 8)` = 0 sein.
- Lücke = `2F06[p]` − `2F06[si]`; `Rank[p]` = 0: Verlust = ¾ der Lücke (LXMUL/LDIV), sonst ⅓; über 3EDh ersetzt durch
  3EDh − `random(100)` (Wort, High = 0).
- Höchstens LDIV(`3CD2[p]`, 2) (halbes Bargeld). Computer (`0840` ≠ 0) bei `2E8E` ≥ 3 (MEAN) verlieren nichts. Nur Verlust > 0:
  Fenster, `039D(Verlust)`, Warteschleife, Schrift 0.

**`02BD`/`039D` Buchung:** Klasse k = `047E(Betrag)`: < 200 → 0, < 500 → 1, < 1000 → 2, < 2000 → 3, sonst 4 (vorzeichenbehaftet).
Text t = `09AF[k]` bzw. `09BE[k]`; Ton `21A7:0054(0, 4Eh/4Fh)`, `0185(50)`, Liste leeren, Wappen `0609:1879(F0h, 1Bh, 0/3)` des
Spielers `37AA`, `21FD:060D`, `0185(30)`; Betrag mit Breite 4 in Text t + `09AA[k]` bzw. `09B9[k]` ab Zeichen 3 (`2295:0001`);
`09E0(Text t, Zeilen 09B4[k]/09C3[k], Farbe 4/5)`; zuletzt `3CD2[37AA]` ± Betrag (32 Bit).

**`04D7` Feldübernahme:** Feldrang c[p] = 3 − Zahl der anderen mit nicht mehr Feldern (`4BA8`, vorzeichenlos, Gleichstand
zählt; Schleife über `37AA`, endet bei 4). v = `random(3)`:
- nur weiter bei c[v] ≤ 1, `Rank[v]` = 0 und `4BA8[v]` ≥ 7.
- w = letzter Spieler mit c = 3; Abbruch bei w = −1 oder `Rank[w]` = 0.
- Ist (`4BA8[w]`·15)/10 (16 Bit) > `4BA8[v]`, muss `random(8)` = 0 sein.

Dann Fenster, Text 5Dh (`DS:3940`, 7 Zeilen, Farbe 5); `37AA` = v: Ton 4Fh, `0185(50)`, Wappen (20h, 1Bh, 3), `060D`,
`0185(30)`; `37AA` = w: Ton 4Eh, `0185(50)`, Wappen (F0h, 1Bh, 0), `060D`. Feldwahl: `2EB0` = `random(Spalten − 1)`,
`2F00` = `random(Zeilen − 1)`, bis Besitzer = v und Art ≠ 4 (HQ); endlos, falls es kein solches Feld gibt (bei ≥ 7 Feldern
nicht möglich). `0609:10D2(x, y, w, Art)`; Warteschleife, Schrift 0.

**`0736` Ernteausfall:** Anteile `22A3:0750`; di = `random(3)`; `Rank[di]` > 1 → nichts; Rang 1 braucht `random(6)` = 0,
Rang 0 `random(3)` = 0. Erste Art si 0…2 mit `342C[di·3 + si]` > 60 (vorzeichenlos):
- Fenster; Wappen der Spieler mit Anteil > 0 an si (Ton 4Fh, `0185(50)`, Wappen (19h + 3Ah·n, 1Bh, 3), `060D`, `0185(20)`),
  dann der ohne (Ton 4Eh, x F8h − 3Ah·n, Abzeichen 0).
- Text 64h + 4·si (`DS:395C` + 10h·si, 4 Zeilen, Farbe 5).
- Felder Zeilen außen (`2F00`), Spalten innen (`2EB0`, beide bis einschließlich Anzahl − 1): Art = si, Besitzer o
  (vorzeichenbehaftet) → bei `random(3 − Rank[o])` ≠ 0 `0609:10D2(x, y, o, −2)`. Bei `Rank[o]` = 3 ist das `random(0)`
  (Seed aus der Uhr, ergibt 0, Feld bleibt).
- Warteschleife, Schrift 0, Ende (nur eine Art).

**`0986` Fenster:** `21FD:037D(4)`, `0A96:001B`, `0609:1ED9`, `0609:1256(0, 14h, 140h, A0h)`, `0A96:06A8`, `0A96:0C3F(DS:082E)`,
`21FD:05DB(1)`, `21FD:026C(DS:00A8)`, `21FD:0329`.

**`09E0` Text:** `0A69:0288(0, 86h − 7·Zeilen, 140h, Zeiger, Zeilen, Farbe, 5)`.

**Schreibmaschine `0A69:0288` → `0243` → `01AD`:** je Zeile i (0 … Zeilen − 1, vorzeichenbehaftet; Nullzeiger überspringen) je
Zeichen k, solange strlen > k: `DS:0140` = Zeichen, `0A96:001B`, `0A96:0525(x, y, DS:0140, Farbe, −1)` mit
x = 1 + (140h − strlen·`3CA8`)/2 (vorzeichenlos) + k·`3CA8`, y = 86h − 7·Zeilen + i·(`34A4` + `34A4`/5); `21FD:060D`, Klick
`21A7:0054(1, 16h)` außer bei Leerzeichen, Pause-Taste `21FD:0245`, `21FD:0185(5)`; Endwartezeit −1 (keine).
**`2295:0001`** schreibt den Betrag (Breite 4, führende Leerzeichen/Nullen → `DS:1126`) ohne Endnull in den Text; der Text ist
danach im Textpuffer `DS:4884` geändert.

## Zustand
| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `37AA` | B | s | Spieler / Schleifenvariable | g/s |
| `302A`, `3D42`, `2F06` | B[4], B[4], D[4] | u, u, s | Ränge und Vermögen (`0825:06CB`) | s/g |
| `3CD2` | D[4] | s | Bargeld | g/s |
| `4BA8`, `2EE7`, `2F05`, `4442`, `2EB1` | | | Felder und Zähler (`0609:10D2`) | g/s |
| `342C` | B[4][3] | u | Anteile (`22A3:0750`) | s/g |
| `2EB0`, `2F00` | B | s | Feldschleife | s |
| `0840`, `2E8E` | B[4], W | u, s | Computer, Stufe | g |
| `08FC`, `08FE` | W | s | Spalten, Zeilen | g |
| `09AA…09C7` | B[5]×6 | s | Texttabellen der Buchung | g |
| `082E`, `090E` | W | – | Seitenkopie, Anzeigemodus (Wartezeiten der Blenden) | g |
| `10D2` | D | u | Zufall | g/s |

## Seiteneffekte in Reihenfolge
Zufall in Reihenfolge (oben); Uhr nur über `random(0)` in `0736`. Textpuffer: Betrag (`2295:0001`); `DS:0140` je Zeichen.
Grafik, Palette, Schrift (`05DB` schreibt `3CA8`/`34A4`), Ton: Ausgabe (Szene). Wartezeiten: `037D` acht Stufen à `0185(4)`, `0329` à `0185(3)`, je plus ein Bildwechsel in
`2295:00C5` bei `090E` = 1; `0185(n)` n Bildwechsel; Seitenwechsel `051F` wie im Menü; Schreibmaschine je Zeichen `060D` (Seitenwechsel), `0245`
(P-Taste) und `0185(5)`; Eingabe sonst nur in `21FD:0004(0)`.

Nur als Ereignis: `21A7:0054` (Ton, R10). Zeichenteil in der Szene: `0609:1879`, `0609:1ED9`, `0609:1256`, Zeichen von
`0A69:01AD`, Fenster, Blenden, Seiten. Im Kern: Takt und Speicher von `0A69:0288`/`0243`/`01AD`, `2295:0001`. Gerufen statt
nachgebaut: `0825:06CB`, `22A3:0750`, `0609:10D2`, `21FD:0004`, `21FD:0245` (`Planting.Pause`), `21FD:0185`/`037D`/`0329`
(Wartezeiten über `Planting.Retraces`/`Fade`); Szene: `FieldScene.Background`/`Frame`, Blenden und Seitenwechsel der Schichten.

## C#-Umsetzung
`src/Traders.Core/Events/WeekIncidents.cs` (`WeekIncidents`, `IncidentLayout`, Schritt `WeekIncidentsStep`),
`src/Traders.Core/Events/IncidentEvents.cs` (ein Ereignis je Aufruf des Originals), `src/Traders.Core/Events/IncidentTexts.cs`
(`ITextMemory`, `TextBufferMemory` über `ProgramStart.TextBuffer`, `2295:0001`). Szene
`src/Traders.Presentation/Events/IncidentScene.cs` (spielt die Ereignisse ab, liest Kern und Textpuffer).

## Prüfung
- `IncidentsDifferentialTests.Week_incidents_17CB_0005_matches_original` (22 Szenarien, Schichten gestubbt): ganzes
  Datensegment, Bildzahl, `random()` je Bild, alle Schichtaufrufe mit Bild und Argumenten. Fälle: kein Ereignis, Übernahme
  (auch mit `random(8)`, Menschen mit Feuer und Plus/Minus), Verlust (3EDh-Kappung, halbes Bargeld, MEAN-Computer immun, Mensch
  nicht immun, `random(Rang·5 + 8)`, ohne Bargeld), Gewinn (Klassen 0–4, über 16 Bit, negatives Vermögen, `random(9)`, Pause und
  Abbruch), Ernteausfall (Rang 0 und 1, `random(0)`, kleines Feld 5 × 4); Wochen 0–3.
- `Scenarios_reach_every_branch_in_the_original`: jeder Verzweigungspunkt, alle fünf Klassen, `random(0)` und Abbruch im
  Original erreicht.
- Schreibmaschine im Differenztest: `0A69:01AD`/`0243`/`0288` und `2295:0001` laufen im Original (übrige Schichten gestubbt),
  hinter den Textmarken liegen die Zeilen aus `ENGLISCH.TXT`; verglichen werden zusätzlich `0A96:0525` (Zeichen, Farbe), der
  Klick, alle Bildwechsel und der Textspeicher.
- Szene `EventsSceneDifferentialTests` (`EventsSceneHarness`, laufendes Standardspiel, alle Schichten live): VRAM aller Seiten,
  DAC, CRTC-Start, Retraces und Bildnummer bei jeder Blende, jedem Seitenwechsel (auch je Zeichen und in der Warteschleife) und
  am Ende; kein Ereignis (Woche 0), Ereignis der Woche 2, Übernahme (auch Menschen mit Keypad und Feuer), Verlust,
  Gewinn mit P-Pause, Ernteausfall mit Gewinn. Gegenproben: Zeichen eine Spalte versetzt (6 von 7 rot), Wartezeit 4 statt 5
  (25 von 30 rot).
- Mehrrundenprobe `IncidentsMultiWeekTests` (vergleicht auch den Textpuffer) (Standardspiel, Wochen 0–3, ungestubbter RefEmu, Ereignis in Woche 2).
- Gegenproben (gezeigt): Gruppen vertauscht, Vorzeichen der Buchung, ausgelassene `random(9)`, umgekehrte Ernteziehung → rot.
