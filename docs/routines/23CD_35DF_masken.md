# 23CD:35DF / 23CD:3CE8 – maskierter Blit und Maske einfärben (VGA, Mode X)

- **Ladebild-Adresse:** `35DF` (Vektor 2C, Sprungleiste `23CD:06A5`), `3CE8` (Vektor 48, `06E3`); Maskenadresse `23CD:054B`
  (y·40 + x/8 im Maskenatlas `CS:057E`, gesetzt von `058A`).
- **Aufgabe:** Atlas-Rechteck nur dort auf die Zeichenseite kopieren, wo die Maske ein gesetztes Bit hat (`35DF`), bzw. die
  Maskenform in einer Farbe füllen (`3CE8`).
- **Aufgerufen von:** Befehlsliste `0A96` (Typ 3 bzw. 4).
- **Parameter / Rückgabe:** `35DF(sx, sy, w, h, dx, dy, mx, my)`; `3CE8(mx, my, w, h, dx, dy, Farbe)`. Keine Rückgabe.

## Ablauf (eigene Worte)
- Masken: 1 Bit je Pixel, 40 Byte je Zeile, höchstwertiges Bit links, Wörter big-endian gelesen. Die unteren drei Bit von
  `mx` gehen verloren (Masken liegen immer auf 8er-Grenzen).
- Vier Durchgänge wie `3449`/`351E` (Ebene des Ziels und der Quelle rücken je Durchgang weiter); im Durchgang i wird das
  Maskenwort um i nach links geschoben, je Zielbyte wird eines der Bits 15, 11, 7, 3 geprüft, dann das nächste Wort.
- Nach jeder Zeile: Masken-Zeiger um die gelesenen Wörter zurück und um 40 vor; Quelle um 320, Ziel um 80.
- Ein Durchgang mit 0 Byte (w < 4) läuft 65536 Byte je Zeile (Zähler läuft unter 0); Tests meiden das.

## Seiteneffekte in Reihenfolge
Map Mask und Video-RAM; Ende Map Mask 0Fh. Kein Zufall, keine Uhr, kein DS-Zugriff.

## C#-Umsetzung
`src/Traders.Vga/ModeXDriver.cs`: `MaskedBlit`, `MaskFill`, `MaskAddr`, `MaskWord`.

## Prüfung
`DriverDifferentialTests.MaskedBlit_35DF_matches_original`, `MaskFill_3CE8_matches_original`: je 40 zufällige Aufrufe
(Masken- und Atlasinhalt zufällig, w ≥ 4), VRAM und Register gleich. Rand: `Rectangles_across_the_border_wrap_like_the_original` (`tests/Traders.Tests/Vga/DriverEdgeCaseTests.cs`). Nicht geprüft: w < 4 (65536 Byte je Zeile).
