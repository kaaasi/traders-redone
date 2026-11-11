# 21FD:026C / 21FD:02C9 / 21FD:0329 / 21FD:0353 / 21FD:037D / 21FD:03B4 / 21FD:03EB / 21FD:04DA / 21FD:0185 – Paletten, Blenden, Warten

- **Ladebild-Adresse:** `21FD:026C` (Dateioffset `0x2583C`), `02C9`, `0329`, `0353`, `037D`, `03B4`, `03EB`, `04DA`; Warten `0185`.
- **Aufgabe:** Basispalette aus der geladenen Palette übernehmen oder farbweise setzen, Arbeitspalette als Blende der Basis zu
  einer Farbe berechnen und an den DAC geben, Ein- und Ausblenden in acht Stufen.
- **Aufgerufen von:** Szenenwechsel (Einblenden nach Bildaufbau, Ausblenden davor).
- **Parameter / Rückgabe:** `02C9(Index, rgb12)`, `03EB(rgb12, Stufe)`, `037D/03B4(rgb12)`, `04DA(rgb12)`, `0185(Anzahl: long)`.

## Ablauf (eigene Worte)
- 12-Bit-Farbe → drei Byte: R = (rgb ≫ 8) als Byte · 8 (Bits über dem zwölften wirken mit), G = ((rgb ∧ F0h) / 16) · 8,
  B = (rgb ∧ 0Fh) · 8; ergibt 0…120, der DAC behält die unteren sechs Bit.
- `026C`: 768 Byte `DS:5017` → `DS:3DCB`. `02C9`: Basis[Index·3 …] (Adresse 16 Bit gebildet, keine Bereichsprüfung).
- `03EB`: Schritt = (Stufe ≪ 3) als Byte. Je Farbe 0…255 und Kanal: Basis > Ziel (Byte vorzeichenbehaftet) → Basis − Schritt,
  nicht unter Ziel; sonst Basis + Schritt, nicht über Ziel; Ergebnis nach `DS:54E8`. Bei VGA dann `2295:00C5(DS:54E8)`.
  Folge der Vorzeichen: Schritt 80h (Stufe 16) oder Basisbytes ≥ 80h laufen über das Ziel hinaus bzw. werden nicht begrenzt.
- `0329`/`0353`: Stufe 7…0 mit Blendfarbe `DS:10D8`, danach `0185(3)` bzw. `0185(1)`.
- `037D`/`03B4`(Farbe): Stufe 0…7, danach `0185(4)` bzw. `0185(2)`; dann `04DA(Farbe)` und `21AF:04CD`.
- `04DA`: `DS:10D8` = Farbe. `0185(n)`: n-mal `0D73:00D3` (32-Bit-Zähler; n ≤ 0 wartet nicht).

## Zustand
| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `3DCB` | B[768] | s (Vergleich) | Basispalette | g/s |
| `5017` | B[768] | – | geladene Palette | g |
| `54E8` | B[768] | – | Arbeitspalette | s |
| `10D8` | W | – | Blendfarbe | g/s |
| `090E` | W | – | Anzeigemodus | g |
| `33E4` | 64 B | – | über `21AF:04CD` genullt | s |

## Seiteneffekte in Reihenfolge
DAC (nur VGA), je Blendstufe ein Rücklaufwarten in `2295:00C5`, dann die Wartezeit der Stufe. Kein Zufall, keine Uhr.

## C#-Umsetzung
`src/Traders.Vga/Layers/Palettes.cs` (`CopyLoadedToBase`, `SetBaseColor`, `Blend`, `FadeIn`, `FadeInFast`, `FadeOut`,
`FadeOutFast`, `SetBlendColor`), `GraphicsLayers.WaitRetraces` (`0185`).

## Prüfung
`tests/Traders.Tests/Vga/PaletteLayerTests.cs`: Basispalette mit zufälligen Bytes 0…255, Farben auch über 12 Bit, Stufen
0, 7, 16, 20h, FFFFh und zufällig, Modus 1 und 0; Indizes von `02C9` mit 16-Bit-Umlauf (5555h, 5556h, AAABh); vier Ein- und vier
Ausblendungen. Verglichen: ganzes DS (mit `DS:10D2`), DAC, Zahl der Rücklaufwarten. Begrenzung und Überlauf bei Stufe 16:
`Blend_stops_at_the_target_but_byte_step_80h_passes_unclamped`.
