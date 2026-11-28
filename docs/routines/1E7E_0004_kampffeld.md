# 1E7E:0004 / 1E7E:0DB3 / 1E7E:083F / 1E7E:0870 / 1E7E:088D / 1E7E:08F1 – Kampffeld und Feld-Hilfen (Segment `1E7E`)

- **Ladebild-Adresse:** Segment `1E7E` (Dateioffset `0x21DE0`, Größe `0xE50`); `0004` bei `0x21DE4`, `0DB3` bei `0x22B93`.
- **Aufgabe:** Kampffeld der Roboterphase aufbauen und zeichnen (`0004`), Zellen setzen und zeichnen (Hilfen), in R11 die
  Zellen des Kampfes (`0BCA`, `0C87`, `08B5`, `08D3`, `09FD`, `0E21`).
- **Aufgerufen von / Phase:** `1C68:000B` (Aufruf 18) ruft `0004`; die Aufstellung `0535:041F` ruft `083F`/`0870`/`088D`; in R11
  der Kampf `0EDE:11DA` (`083F`), `0EDE:136C` (`08F1`), `0EDE:189C` (`088D`).
- **Parameter / Rückgabe:** je Routine unten; keine Rückgabe (AX ungenutzt).

## Raster

`DS:48F4` ist im Kampf ein Feld **26 × 26 Byte**: Zelle (x, y) bei `48F4 + 26·x + y`, x, y = 0…25 (Beleg: `0004` läuft x und y
je 0…19h, `0DB3` schreibt `[48F4 + 1Ah·x + y]`). Das Feld endet bei `4B97`, unmittelbar vor `ShopOil` `4B98`. Der Plan
(Schnittstelle „9 × 26“) übernimmt die Größe des Produktionsrasters (`22A3`), das denselben Puffer nur bis `48F4 + 9·26`
nutzt; für das Kampffeld gilt 26 × 26. Bildschirm: Zelle (x, y) liegt bei (8·x + 38h, 8·y − 4), die Reihen y = 0 und 19h sind
halbe Reihen (4 Zeilen), sodass 26 Reihen genau 200 Zeilen füllen. Zellwert / 8 ist die Art (`0004` und `049C` werten
`Wert / 8 = 0` als „frei“).

## Ablauf `0004` (R10, Teil 2)

1. `0A96:001B`, `0A96:0267(28h, 10h, 18h, 18h, 0, 0, 140h, C8h)` (Kacheln über den ganzen Schirm), `0A96:06A8`.
2. Für x = 0…25 (außen): Liste leeren, für y = 0…25: innen (0 < x, y < 19h) `07E8(x, y)`, sonst Rand `07AA(x, y)`; je x `06A8`.
3. `0132` (Gelände in einem 8×8-Plan `DS:4478`, Verwerfungsschleife), `049C` (Übergänge über `06A7`, je x eine Liste).
4. n = `random(50) + 10`; Liste leeren; n-mal x = `random(23) + 1`, y = `random(23) + 1`, ist die Zelle frei (Wert / 8 = 0):
   `0812(x, y)`. Dann `06A8`.
5. Liste leeren; für `DS:37AA` = 0…3: `08F1(p, 0)`, `09C4`; `06A8`; `0A96:0C3F([082E])`. `37AA` bleibt 4.

Zufall (17 Aufrufstellen in `0004` und Callees): `0004` dreimal, `0132` sechs, `06A7` vier, `07AA` zwei, `07E8` und `0812` je einer.
Reihenfolge und Abbruchbedingungen dokumentiert Teil 2 mit dem Port.

## Übrige Routinen des Segments (kurz)

| Routine | Zweck (vorläufig) | Zufall | Runde |
|---|---|---|---|
| `0132` | 8×8-Plan `4478` mit 1 füllen, `random(4)` = 0 → 0 Löcher, sonst `random(30h)`; Löcher über `random(7)`, `random(7)` mit Verwerfung (beide Koordinaten in {0, 1, 6}, oder Zelle schon 0; endet, da höchstens 47 < 48 zulässige Zellen); danach je Zelle Nachbarmaske → Kacheln über `07AA` | 6 | R10 |
| `049C` / `06A7` | Übergänge: je freier Zelle Nachbarn (`48DA`/`490E` = ±1 x) prüfen, Kachel nach Maske über `06A7(x, y, Typ)` (Typ ≤ 7 Tabellen `0F62`/`0F6A`, 8/9 zufällig) | 4 | R10 |
| `07AA` / `07E8` / `0812` | Randkachel (`random(2)`, `random(2)`, Wert 30h) / Boden (`random(3)`, Wert 0) / Hindernis (`random(2)`, Wert 11h) | je 2/1/1 | R10 |
| `09C4` | Zelle des Spielers `37AA` an (`[0844+p]`, `[0848+p]`), Kachel (8·p, 30h), Wert 8 | – | R10 |
| `08B5` / `08D3` | Zelle mit Wert 21h, Kachel (40h, 30h) / Wert 0, Kachel (8, 28h) | – | R11 |
| `09FD`, `0BCA`, `0C87`, `0E21` | Kampfzellen (Treffer, Spuren); `0BCA` ruft `0C87` sechsmal, `0E21` zieht `random(1)` | `0E21`: 1 | R11 |

## Feld-Hilfen (Teil 1, portiert)

- **`0DB3(sx, sy, x, y, v)`:** Höhe h = 8; ty = 8·y − 4 (16 Bit), bei ty < 0 (vorzeichenbehaftet) 0; y = 0 → sy + 4, h = 4;
  sonst y = 19h → h = 4. `0A96:0311(sx, sy, h, 8·x + 38h, ty)` (Typ 7), danach `[48F4 + 26·x + y]` = Low-Byte von v
  (16-Bit-Adresse, `imul`).
- **`083F(x, y, k)`:** `0DB3(8·k + 50h, 0, x, y, ((k + 7)·8 + 3) als Byte)` – die Byte-Rechnung läuft in AL (AH beliebig, `0DB3`
  liest nur das Byte). `0535` übergibt k = 0…5 (`[bp−3]`).
- **`0870(x, y)`:** `0DB3(20h, 0, x, y, 1Ah)`. **`088D(x, y)`:** `0DB3(8·(int8)[37AA] + 20h, 30h, x, y, 23h)`.
- **`08F1(p, f)`:** p als Wort, f als Byte. Zuerst `l = (int8)[2F01 + p]`; dann für j = 0…1 (außen), i = 0…1 (innen):
  `0DB3(78h·f + 8·i, 8·j + 8, [084C + p] + i, [0850 + p] + j, [0F86 + f])` (die Tabellen werden je Durchgang neu gelesen). Ist
  l ≠ −1 und f ≠ 1: `0A96:0525(8·[084C + p] + 3Eh, 8·[0850 + p] + 6, Far-Zeiger [0F72 + 4·l], [0F82 + p], −1)` (Typ 12, ohne
  Schatten). Zuletzt `[4BB6 + p] = f`.

## Zustand
| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `48F4` | B[26][26] | – | Kampffeld, Zelle `26·x + y` | s – `0DB3` |
| `084C` / `0850` | B[4] | u | Feld-x / Feld-y des Roboters je Spieler | g – `08F1` |
| `2F01` | B[4] | s | Beschriftung je Spieler (Index in `0F72`, −1 = keine) | g – `08F1` |
| `0F72` | Far[] | – | Beschriftungstexte (Ladebild: vier Zeiger auf `DS:0FF4`…`0FFA`) | g – `08F1` |
| `0F82` | B[4] | u | Farbe der Beschriftung (Ladebild 8, 9, 0Dh, 0Ch) | g – `08F1` |
| `0F86` | B[] | u | Zellwert je Roboterbild f (Ladebild f = 0: 29h, 1 und 2: 0) | g – `08F1` |
| `4BB6` | B[4] | u | zuletzt gezeichnetes Roboterbild je Spieler | s – `08F1` |
| `37AA` | B | s | Spieler (Kachel von `088D`; Schleifenvariable in `0004`) | g – `088D`; s – `0004` |
| `15BA`/`15BE`/`15C0` | – | – | Befehlsliste (`0A96_06A8_befehlsliste.md`) | s – über `0311`/`0525` |

Indizierte Schreibziele der Hilfen laut Disassembly (der Aufrufgraph zählt nur direkte): `[bx+48F4]` (`0DB3`), `[si+4BB6]` (`08F1`);
sonst keine. `tools/callgraph.py` bestätigt die Aufrufe (`0DB3` → `0A96:0311`; `08F1` → `0DB3`, `0A96:0525`) und keinen Zufall.

## Seiteneffekte in Reihenfolge
Je Zelle erst der Blit (Eintrag Typ 7 in der Befehlsliste), dann das Rasterbyte; in `08F1` vier Zellen, ggf. der Text (Typ 12),
dann `4BB6`. Kein Zufall, keine Uhr, keine Eingabe, kein Ton. Gezeichnet wird erst beim Ausführen der Liste durch den Aufrufer.

## C#-Umsetzung
`src/Traders.Core/Robots/Field/FieldCells.cs` (`Cell`, `PlaceKind`, `PlaceBlock`, `PlaceMarker`, `DrawRobot`), Adressen in
`BattleFieldLayout.cs`; die Zeichenanteile als `FieldCommand`-Liste (`FieldCommand.cs`: `Blit8` = `0A96:0311`, `Text` =
`0A96:0525`, Argumente in Aufrufreihenfolge), die der Aufrufer in die Befehlsliste der Ausgabeschicht gibt. Namen von
`083F`/`0870`/`088D` sind vorläufig (Bedeutung der Werte 3Bh…63h, 1Ah, 23h klärt Spur 4 bzw. R11).

## Prüfung
`tests/Traders.Tests/Robots/Field/FieldCellsDifferentialTests.cs` (RefEmu `LoadOnly`, Direktaufrufe nach `0A96:001B`, die
Befehle des Ports über `GraphicsLayers.AddBlit8`/`AddText` in eine Kopie des DS; danach ganzes 64-KB-DS gleich, also Raster,
Listeneinträge, Zeiger, Zähler, `4BB6` und `DS:10D2`):
- `Cell_matches_the_original_including_half_rows_and_signs` – 400 Fälle (`new Random(0DB3h + n)`), x, y um die Ränder (−3…27),
  y = 0/19h, y < 0 und 8·y mit Überlauf (0FFFh, 1000h, 2000h), sy nahe FFFFh.
- `Placement_cells_match_the_original` – 600 Fälle `083F` (k = 0…5, 6, 18h, 19h, FFh, 100h, 1FFFh, FFFFh), `0870`, `088D`
  (`37AA` 0…3 und beliebige Bytes).
- `Robot_matches_the_original_with_and_without_label` – 400 Fälle, p = 0…4, FFFFh, f = 0, 1, 2, 3, 80h, FFh, Beschriftung −1, 0…3 und
  ungewöhnliche Indizes; beide Zweige gezählt.
- `Robot_of_player_two_writes_its_four_cells_and_label_in_order` – Einzelfall mit Erwartungen aus dem Disassembly (halbe Reihe y = 0).

Gegenprobe (lokal, nicht eingecheckt): `y == 0x19` → `0x18` und Text auch bei f = 1 → drei der vier Tests rot.
