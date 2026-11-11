# 21AF:04A2 / 21AF:04CD – Zeigerpaare der bewegten Objekte löschen

- **Ladebild-Adresse:** `21AF:04A2` (Dateioffset `0x25592`), `21AF:04CD`.
- **Aufgabe:** `04A2(i)` nullt die beiden Far-Zeiger von Satz i ab `DS:33E4` (8 Byte je Satz); `04CD` tut das für i = 0…7.
- **Aufgerufen von:** `04CD` am Ende von `21FD:037D/03B4` (Ausblenden).
- **Parameter / Rückgabe:** `04A2(i)`, Adresse 33E4h + i · 8 in 16 Bit; keine Rückgabe.

## Seiteneffekte in Reihenfolge
Nur DS. Bedeutung der Zeiger: Teil 3 der Spur (Objekte `21AF`).

## C#-Umsetzung
`src/Traders.Vga/Layers/Palettes.cs` (`ClearObjectPointerPair`, `ClearObjectPointerPairs`).

## Prüfung
`PaletteLayerTests.ClearObjectPointerPair_21AF_04A2_matches_original` (i = 0…9), `04CD` in `FadeOut_21FD_037D_and_03B4_match_original`.
