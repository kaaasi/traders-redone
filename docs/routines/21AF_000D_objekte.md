# 21AF:000D / 21AF:00CA / 21AF:022B / 21AF:0412 / 21AF:045E / 21AF:0472 / 21AF:04A2 / 21AF:04CD / 0000:5101 – bewegte Objekte verwalten

- **Ladebild-Adresse:** Segment `21AF` (Dateioffset `0x250F0`), `000D` bei `0x250FD`.
- **Aufgabe:** die acht Objektsätze `DS:330C` (Belegung `GRAFIK.md` §4) einrichten, Bild-, Masken- und Sicherungszeiger setzen,
  Objekte zeigen und verbergen, Rückschreibsätze ungültig machen. Gezeichnet wird über `23CD:06E9/071C`.
- **Aufgerufen von:** `00CA` 20-mal, `022B` 15-mal, `0412`/`045E` je 9-mal, `000D` 6-mal, `04CD` 5-mal (u. a. `21FD:037D/03B4`),
  `0472` 3-mal in `0535`, `0609`, `0825`, `08CF`, `0DFD`, `0EDE`, `186C`, `1B03`, `1C68`, `1F63`, `20B3`, `21FD`. `04A2` nur aus `04CD`;
  `21AF:0489` (setzt `DS:54D8/54D9 + 2·i` = 1) wird nirgends aufgerufen.

## Ablauf (eigene Worte)
- `000D`: je Objekt i Zeiger +04 = `DS:34A6 + 54h·i` (21 Bildzeiger), +08 = `DS:3068 + 54h·i` (21 Maskenzeiger), +0C =
  `DS:3438 + 8·i` (zwei Sicherungszeiger), +10 = `DS:33E4 + 8·i` (zwei Rückschreibsätze à 4 Byte, direkt, keine Zeiger);
  Sätze genullt, inaktiv.
- `00CA(i, Bild, x, y, mx, my)`: Bildzeiger = Atlaszeiger `DS:45D6` + Versatz; VGA y·320 + x/4 + 4 als long, mit der
  Laufzeitroutine `0000:5101` (huge-Zeiger + long, Ergebnis normalisiert auf Offset 0…15) addiert; EGA y·160 + x/8 + 4,
  Tandy y·160 + x/2 + 4, nur auf den Offset. Maskenzeiger = `DS:4BC6` + 2·(my·20 + mx/16). Divisionen vorzeichenbehaftet.
- `022B(i, x1, y1, x2, y2)`: Sicherungspuffer für Seite 0/1 im RAM-Atlas wie `00CA`, bei VGA x vorher auf ein Vielfaches von 4
  aufgerundet. Unbekannter Modus: uninitialisierte Versätze (nicht portiert).
- `0412(i, Bild)`: +18 = Bild, Rückschreibsätze genullt, aktiv = 1. `045E(i)`: aktiv = 0; `0472`: alle. `04A2(i)`: beide
  Rückschreibsätze von i nullen; `04CD`: alle.

## Zustand
| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `330C` | 1Ah·8 | – | Objektsätze | g/s |
| `34A6`, `3068` | Far[21]·8 | – | Bild-, Maskenzeiger | s |
| `3438` | Far[2]·8 | – | Sicherungspuffer | s |
| `33E4` | 4 B·2·8 | – | Rückschreibsätze (Seitenoffset, x&3, gültig) | s |
| `45D6`, `4BC6` | Far | – | RAM-Atlas, Maskenatlas | g |
| `090E` | W | – | Anzeigemodus | g |

## Seiteneffekte in Reihenfolge
Nur DS. Kein Zufall, keine Uhr.

## C#-Umsetzung
`src/Traders.Vga/Layers/Objects.cs` (`InitializeObjects`, `SetObjectImage`, `SetObjectSaveBuffers`, `ShowObject`, `HideObject`,
`HideAllObjects`, `DrawObjects`/`RestoreObjects` als Brücke von den DS-Sätzen zu `ModeXDriver.DrawObjects/RestoreObjects`),
`Palettes.cs` (`InvalidateRestoreRecords`, `InvalidateAllRestoreRecords`). Far-Zeiger in RAM-Atlas, Maskenatlas oder DS werden auf
die Puffer des Ports abgebildet. `0000:5101` nur für nicht negative Versätze.

## Prüfung
`tests/Traders.Tests/Vga/ObjectLayerTests.cs`: 24 Runden Zurückschreiben/Bewegen/Zeichnen auf wechselnden Seiten mit acht Objekten
(Bilder über `00CA`, Puffer über `022B`, `0412`/`045E`/`0472`), Vergleich DS, VRAM, Zeichenseite, RAM-Atlas; Zeiger für EGA/Tandy;
Normalisierung mit Atlaszeigern `4FFF:0010`, `4F00:FFF8`, `4000:8000`. `04A2`/`04CD`: `PaletteLayerTests`.
