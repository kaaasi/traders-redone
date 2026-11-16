# 1DEF:000C / 1DEF:00DB / 1DEF:0105 / 1DEF:0173 / 1DEF:01AF / 1DEF:020E / 1DEF:0227 / 1DEF:0308 / 1DEF:03CD / 1DEF:04A9 / 1DEF:0651 / 1DEF:06F8 / 1DEF:0751 / 1DEF:07AD – Zusatzphase „Geschenk von Fat Mike“

- **Ladebild-Adresse:** Segment `1DEF` (Dateioffset `0x214F0`, Größe `0x8F0`); `000C` bei `0x214FC`. Die ersten zwölf Bytes
  des Segments sind der Schluss der Routine davor. `07AD` läuft über das Segmentende hinaus: `mov sp,bp`/`pop bp`/`retf`
  liegen bei `1F63:0000`. Hinter `04A9` steht bei `0647` die Sprungtabelle (fünf Wörter `0612`, `056C`, `0533`, `05A4`, `05DB`).
- **Aufgabe:** Aufruf 24 der Wochenschleife `1777:01F8` (nach dem Handel). Fällt die Phase, zeigt ein Hinweis (Textzeilen 227/228
  der Texttabelle, 0-basiert: Fat Mike verschenkt Waren), dann liegen 25 Warenstücke auf einem Brett aus 6 × 7 Zellen. Alle vier
  Spieler laufen gleichzeitig – Menschen mit der Steuerung, Computer zum nächsten freien Stück – und nehmen auf, was auf ihrer
  Zelle liegt: je Stück 2·`DS:2F26` Einheiten der Ware. Sind alle 25 aufgenommen, läuft die Warteschleife und die Gewinne kommen
  in die Vorräte.
- **Aufgerufen von / Phase:** `000C` nur von `1777:01F8`; alle übrigen nur aus `000C` und untereinander (`tools/callgraph.py`).
- **Parameter / Rückgabe:** keine, außer `0308(Zelle)` und `00DB` (AL = 1: Phase fällt).

## Ablauf (eigene Worte)

- **`000C`:** Rahmen `sub sp,0D2h`; Fernzeiger auf die Stapelfelder: `54A6` → `[bp−54h]` (Boden, W[42]), `547C` → `[bp−0A8h]`
  (Ware, W[42]), `54A2` → `[bp−0D2h]` (Ziel, B[42]), Segment jeweils SS. `00DB` = 0 → Ende. Sonst `186C:0002([08CE])`,
  `0609:1F2D`, `21FD:037D(0)`, `21FD:026C(DS:00A8)`, `0609:11E7`, `0105`, `21FD:037D(0)`, `0173`, `01AF`, `020E`, `0227`, `03CD`,
  `21FD:0329`, `21FD:0185(14h)`, `5480` = 0, `0A96:001B`, `04A9`, `21FD:0004(0)`, `0609:21BA` je Spieler (`37AA` = 0 … 3), `0751`.
- **`00DB`:** `random(20)` = 0 → AL = 1; sonst AL = (2·`57F2` < `57F0`) (16-Bit-Schieben, Vergleich mit Vorzeichen). Die Ziehung
  geschieht immer. `random(n)` liefert 0 … n, also 1 zu 21; Woche 0 der Standardpartie fällt über die Produktion (`57F0` = 1).
- **`0105`:** `0A96:001B`, `0609:1ED9`, `0609:1256(28h, 4Bh, F0h, 32h)`, `0A96:06A8`, `0A96:0C3F([082E])`, `21FD:0329`,
  `0A69:0288(28h, 64h, F0h, DS:3B5C, 2, 5, 5)` (Schreibmaschine wie in der Ernte), `21FD:0185(14h)`.
- **`0173`:** Zelle 0 … 29h: `random(6)`, > 3 (mit Vorzeichen) → 0; Bodenkachel 0 … 3 (0 also mit 4/7).
- **`01AF`:** alle Zellen −1; 25-mal (`si` = 0 … 18h): `random(41)` bis die Zelle leer ist, dann Ware `random(3)` (0 … 3).
- **`020E`:** alle Ziele FFh.
- **`0227`:** `0A96:001B`, `0609:1ED9`, `0609:1256(0, 4, 50h, C0h)`, `0609:13C5(70h, 17h, C0h, 9Ah)` (zwölf `random(1)`), `0308`
  für jede Zelle, `0A96:06A8`, `0A96:001B`; je Spieler `0A96:0030(C0h, B0h, 30h, 2Ch, 10h, 2Ch·p + 0Ch)` und für `45D0` = 0 … 3
  `0609:1912(k, 10h, 2Ch·p + 8k + 13h)`; `0A96:06A8`, `0A96:0C3F([082E])`.
- **`0308(c)`:** Spalte c mod 6, Zeile c / 6 (`idiv`); `0A96:0030([0F22 + 2·Boden], [0F2A + 2·Boden], 20h, 16h, 20h·s + 70h,
  16h·z + 17h)`; liegt Ware w ≠ −1: `0A96:013B(130h, 7w + B0h, 80h, 7w + 28h, 10h, 7, 20h·s + 78h, 16h·z + 26h)`.
- **`03CD`:** je Spieler `4BD2` = 0, `3022` = 0, Spalte `304E` = `random(5)`, Zeile `305A` = `random(6)`, Objekt-x
  `3320 + 1Ah·p` = 20h·s + 78h, Objekt-y = 16h·z + 23h, `0609:1FCC`, Gewinne `5482[p][0…3]` = 0.
- **`04A9`:** solange `5480` < 19h (Wort, mit Vorzeichen; Prüfung bei `0636` vor jedem Durchlauf): `21FD:051F`,
  `23CD:071C([082E])`, `0A96:06A8`, `0A96:001B`, `21FD:0185(3)`, `0E9E:037A`, `21FD:0245`; je Spieler: `3022[p]` ≠ 0 →
  `0609:1FE0` (Schritt der laufenden Bewegung); sonst `0651`, Computer (`0840` = 1) → `07AD`, dann Sprungtabelle über
  `cbw(3308[p]) + 1` (vorzeichenlos ≤ 4): −1 → `0609:215E` (Blinzeln, `random(200)`); 0 rechts, 1 links (Spalte ≠ 5 bzw. 0;
  `4BCA` = Richtung, `3022` = 11h), 2 hoch, 3 runter (Zeile ≠ 0 bzw. 6; `3022` = 0Ch), Spalte bzw. Zeile ± 1 (Byte); andere
  Aktionen (Feuer 0Ah) nichts. Danach `0A96:06A8`, `23CD:06E9([082E])`. Nach der Schleife `21FD:0665`.
- **`0651`:** Zelle = 6·Zeile + Spalte (cbw, 16 Bit); `45D0` = Ware; −1 → Ende. Sonst `5480` + 1, `5482[8p + 2·45D0]` +
  2·`cbw(2F26)`, `06F8`, Ware = −1, `0308(Zelle)` (nah), Ton (0, 31h).
- **`06F8`:** `0A96:057A(22h, 2Ch·p + 8·45D0 + 18h, Gewinn als Doppelwort (cwd), 3, [C8h + 2p], 5)`.
- **`07AD`:** bester Abstand 100, Ziel 0 (Bytes); für Zelle 0 … 29h mit Ware ≠ −1 und Ziel FFh oder = `37AA`: Abstand =
  |s − Spalte| + |z − Zeile| in 8 Bit, Vergleich mit Vorzeichen, bei ≤ wird ersetzt (bei Gleichstand gewinnt die letzte Zelle).
  Dann `3308[p]`: Zielspalte < Spalte → 1, > → 0, sonst Zielzeile < Zeile → 2, sonst 3. Ziel[Zelle] = `37AA`. Ohne Treffer
  bleibt Zelle 0 das Ziel (und wird beansprucht).
- **`0751`:** `3CF4[8p + 2k]` += `5482[8p + 2k]` für alle p, k = 0 … 3 – 16 Bit **ohne Obergrenze** (anders als die Ernte mit 999).

## Zustand

| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `54A6`/`54A8`, `547C`/`547E`, `54A2`/`54A4` | Fernzeiger | – | Stapelfelder Boden, Ware, Ziel | W `000C`; L alle |
| `5480` | W | s | aufgenommene Stücke (Ende bei 25) | W `000C`, `0651`; L `04A9` |
| `5482` | W[4][4] | s | Gewinn je Spieler und Ware (Schritt 8) | W `03CD`, `0651`; L `06F8`, `0751` |
| `57F0` / `57F2` | W | s | Felder ohne / mit Ertrag (aus `22A3:0003`) | L `00DB` |
| `2F26` | B | s | Einheiten je Stück: 2·`2F26` | L `0651` |
| `0F22` / `0F2A` | W[4] | – | Atlaskoordinaten der Bodenkacheln | L `0308` |
| `3B5C` | Fernzeiger[2] | – | Hinweiszeilen 227/228 | L `0105` |
| `304E`/`305A`, `3308`, `3022`, `4BCA`, `4BD2`, `3320`/`3322 + 1Ah·p` | wie Anbau | | Cursor 0…5/0…6, Aktion, Bewegung, Objekt | L/W |
| `37AA`, `45D0` | B / W | s | Schleifenvariablen; danach 4 / 4 | W |
| `3CF4` | W[4][4] | s | Vorräte | W `0751` |

## Seiteneffekte in Reihenfolge

Zufall: `00DB` 1 × `random(20)`; fällt die Phase: `0173` 42 × `random(6)`, `01AF` mindestens 25 × `random(41)` und 25 ×
`random(3)`, `0227` 12 × `random(1)` (Rahmen `0609:13C5`), `03CD` je Spieler `random(5)`, `random(6)`; im Lauf `random(200)` je
untätigem Spieler ohne laufendes Blinzeln. Grafik und Ton wie oben; Eingabe `0E9E:037A` einmal je Durchlauf, Pause `21FD:0245`.
Jedes Warten (Seitenwechsel, `0185`, Blenden, Schreibmaschine, Warteschleife) ist ein Bild.

## Stapelrahmen

SS ist nicht DGROUP: in der laufenden Partie ist SS = DGROUP + 58Ah (direkt hinter den `58A0h` Bytes von DGROUP), SP beim
Eintritt aus der Wochenschleife 0FDAh, BP also 0FD8h (RefEmu-Probe in den Wochen 0–3, Test
`Phase_in_real_games_matches_the_original`). Der Port hält die drei Felder in `ExtraBoard` und schreibt nur die Fernzeiger mit
diesem Rahmen (`ExtraStep.GameFrame`, im Test der Rahmen des Originals).

## C#-Umsetzung

`src/Traders.Core/Extra/ExtraPhase.cs` (alle 14 Routinen), `ExtraEvents.cs` (Ereignisse, `IExtraHost`, `ExtraBoard`,
`ExtraFrame`), `ExtraHost.cs` (Steuerung, Pause, Warteschleife `Week.WaitLoop` im Modus 0, Texte), `ExtraStep.cs` (Handler für
Aufruf 24; eingebunden von Spur `ende`). Gerufen statt nachgebaut: `0609:1F2D`/`11E7`/`13C5`/`1FCC`/`1FE0`/`21BA`
(`Planting`, `FieldCursor`), `0609:215E` (`FieldCursor.Idle`), Schreibmaschine `0A69:0288` (`Harvest.TypeLines`), Blenden,
Seitenwechsel und `0185` (`Planting.Fade`/`Flip`/`Retraces`), `21FD:0004` (`Week.WaitLoop`).

## Prüfung

`tests/Traders.Tests/Extra/ExtraDifferentialTests.cs` gegen den RefEmu (Zeichnen, Ton, Steuerung, Pause und Warteschleife
ersetzt und mitgeschrieben): `00DB` (Zufall 0, Grenze 2·aktiv = untätig ± 1), `0173`/`01AF`/`020E`, `0308`, `0227`/`0105`/`03CD`,
`0651`/`06F8`, `07AD` (auch ohne freies Stück), `0751` (Summen > 999 und Überlauf über 7FFFh), `04A9` Durchlauf für Durchlauf
mit Datensegment und Stapelfeldern, `000C` ganz aus LoadOnly-Zuständen (fällt / fällt nicht) und aus echten Partien in den
Wochen 0–3 (Menschen und Computer). Verglichen: Datensegment, Stapelfelder, `DS:10D2`, Zahl der `random()`-Aufrufe, Aufruffolge.
Gegenproben (je rot): Auslösung invertiert, eine Ziehung in `0173` ausgelassen, Vorrat bei 999 gekappt, Gleichstand der KI
andersherum, Rand unten bei Zeile 5.
