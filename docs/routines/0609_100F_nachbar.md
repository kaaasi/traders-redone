# 0609:100F – Grenzt ein Feld an eigenes Land? (Helfer des Computerzugs)

- **Ladebild-Adresse:** Segment `0609` (Dateioffset `0x9690`); `100F` bei `0xA69F`.
- **Aufgabe:** 1, wenn mindestens eines der vier orthogonalen Nachbarfelder dem Spieler am Zug (`DS:37AA`) gehört. Reine
  Prüfung über `0609:1072` (Feld im Raster und Besitzer = `37AA`, Befund [0609_10D2_feldart.md](0609_10D2_feldart.md)).
- **Aufgerufen von / Phase:** `135D:05AF` (Feldwahl der KI, Modi 3/5), `135D:08B7` (toter Zweig, siehe
  [135D_000E_computerzug.md](135D_000E_computerzug.md)) und `0609:08E7` (menschlicher Kauf; ruft nur). Diese Spur portiert die
  Routine (R5P-2).
- **Parameter / Rückgabe:** `100F(x, y)` (Wörter, `x` bei `bp+6`, `y` bei `bp+8`); `AL` = 1/0, `AH` unbestimmt. Kein Zustand
  geschrieben.

## Ablauf (eigene Worte)

- Prüft nacheinander `1072(x−1, y)`, `1072(x+1, y)`, `1072(x, y−1)`, `1072(x, y+1)`; die erste 1 ergibt `AL` = 1 (Rückkehr),
  sonst `AL` = 0. `1072(a, b)` = 1, wenn (a, b) im Raster liegt (`0609:10A7`) und der Besitzer `4442[a·6 + b]` = `DS:37AA`.
- Reihenfolge: links, rechts, oben, unten.

## Zustand

| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen |
|---|---|---|---|---|
| `4442` | B[9·6] | s | Besitzer je Feld | L (über `1072`) |
| `08FC` / `08FE` | W | s | Spalten / Zeilen (`10A7`) | L |
| `37AA` | B | s | Spieler am Zug | L |

## Seiteneffekte

Keine.

## C#-Umsetzung

`src/Traders.Core/Field/PlantingAiHelpers.cs`: `PlantingAiHelpers.HasOwnNeighbour(Dgroup d, short x, short y)` = `100F`, ruft
das vorhandene `FieldGrid.OwnedByCurrent` (= `0609:1072`).

## Prüfung

`tests/Traders.Tests/Ai/PlantingAiHelpersDifferentialTests.cs` (RefEmu `LoadOnly`, direkter `Call(0609:100F)`): ganzes DS und
`AL` gleich; Fälle mit eigenem/fremdem/freiem Nachbarn in jeder Richtung, Feld am Rand, Koordinaten außerhalb. Gegenprobe:
eine Nachbarrichtung weggelassen → rot.
