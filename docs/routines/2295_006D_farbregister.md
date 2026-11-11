# 2295:006D / 2295:00C5 – Farbregister

- **Ladebild-Adresse:** `2295:006D` (Dateioffset `0x25FBD`), `2295:00C5`; DAC-Ausgabe `27C6:042E`.
- **Aufgabe:** ein DAC-Register aus einer 12-Bit-Farbe setzen bzw. alle 256 Register aus einer Palette.
- **Aufgerufen von:** `006D` aus Szenen (einzelne Farben); `00C5` aus `21FD:03EB`.
- **Parameter / Rückgabe:** `006D(Index, rgb12)`, `00C5(far Palette)`.

## Ablauf (eigene Worte)
- `006D`: Farbe wie `21FD:02C9` in drei Byte; nur bei `DS:090E` = 1 `27C6:042E` mit bx = Index, cx = 1 (Port 3C8h erhält bl).
  Kein Rücklaufwarten.
- `00C5`: zuerst `0D73:00D3` (auch auf anderen Karten), dann bei VGA `27C6:042E` mit bx = 0, cx = 100h.
- `27C6:042E`: je Eintrag drei Byte lesen, unter `cli` Index nach 3C8h, R, G, B nach 3C9h; bx und Zähler weiter.

## Seiteneffekte in Reihenfolge
DAC (VGA hält 6 Bit je Kanal), Rücklaufwarten (`00C5`). Kein DS-Schreibzugriff, kein Zufall.

## C#-Umsetzung
`src/Traders.Vga/Layers/Palettes.cs` (`SetColorRegister`, `SetAllColorRegisters`), DAC-Modell `VgaDac`
(`src/Traders.Vga/Layers/GraphicsLayers.cs`).

## Prüfung
`tests/Traders.Tests/Vga/PaletteLayerTests.cs`: `006D` mit Index 0, 255, 256, FFFFh und zufällig, Farben bis FFFFh, Modus 1, 0, 2;
`00C5` mit vier zufälligen Paletten in DS, Modus 1 und 0. Verglichen: DS, DAC, Rücklaufwarten.
