# 23CD:32D8 / 23CD:3A9A / 23CD:3386 / 23CD:3449 / 23CD:351E / 23CD:328E – Blits und Kopien (VGA, Mode X)

- **Ladebild-Adresse:** Vektorziele des VGA-Teils: `32D8` (Vektor 14, Sprungleiste `23CD:06A0`), `3A9A` (Vektor 20, `06D4`),
  `3386` (Vektor 38, `06AA`), `3449` (Vektor 3C, `06AF`), `351E` (Vektor 40, `06B4`), `328E` (Vektor 18, `06B9`).
  Hilfe: Atlasadresse `3181` (Paragraph y·20, Offset x/4 + 4 für den Größenkopf).
- **Aufgabe:** Rechtecke zwischen RAM-Atlas (`CS:0582`, ebenengetrennte Zeilen zu 320 Byte) und Zeichenseite kopieren.
- **Aufgerufen von:** Befehlsliste `0A96` (Typen 0, 1, 2, 5, 7/8/16, Kachelung 6); `3A9A` zusätzlich aus `0C29`; `328E` aus `0B67`.
- **Parameter / Rückgabe:** `32D8`, `3386`, `3449`, `351E`: `(sx, sy, w, h, dx, dy)`; `3A9A(sx, sy, n, h, dx, dy)` mit Breite
  n·8; `328E(Quelloffset, Quellsegment, Zieloffset, Zielsegment, w, h)` in Bytes (je Byte 4 Pixel). Keine Rückgabe.

## Ablauf (eigene Worte)
- `32D8`: je Zielebene k (Map Mask 1, 2, 4, 8) ergibt sich die Quellebene aus `(k − dx) & 3 + (sx & 3)`; ein Übertrag über 3
  rückt die Quelle um ein Byte weiter. Startversatz und Restbyte je Ebene wie bei `31A6`, Ausrichtung nach `dx`.
- `3A9A`: schiebt das Breitenargument auf dem Stapel um 3 Bit nach links (·8) und springt nach `32D8`; das Argument des Aufrufers ist danach verändert (Borland-Aufrufer räumen ab, ohne Wirkung).
- `3386`: wie `32D8`, liest aber zuvor das Ziel über Read Map = k und schreibt Ziel ODER Quelle. Innere Schleife mit `LOOP`:
  trägt eine Ebene 0 Byte (w < 4), läuft sie 65536-mal je Zeile (wie das Original; Tests meiden das).
- `3449`: Kopie im RAM-Atlas, vier Durchgänge; der erste trägt `w/4` + 1, solange Restpixel übrig sind; Quell- und Zielebene
  rücken je Durchgang um eins weiter, beim Überlauf auf Ebene 0 rückt der Byteoffset um eins.
- `351E`: dieselbe Zählweise innerhalb der Zeichenseite, je Durchgang Read Map = Quellebene, Map Mask = Zielebene; Ende Map Mask 0Fh.
- `328E`: Latch-Kopie mit allen Ebenen; je Byte Lesen (lädt die Latches) und Schreiben mit Bitmaske 0; nach jeder Zeile
  beide Zeiger um `80 − w`. Danach direkter Aufruf von `30D1`.

## Seiteneffekte in Reihenfolge
VGA-Register (Map Mask, Read Map, bei `328E` Bitmaske) und Video-RAM bzw. RAM-Atlas; kein Zufall, keine Uhr, kein DS-Zugriff.

## C#-Umsetzung
`src/Traders.Vga/ModeXDriver.cs`: `BlitAtlasToPage`, `BlitAtlasToPageFixed`, `OrAtlasToPage`, `CopyAtlasToAtlas`,
`CopyPageToPage`, `LatchCopy`; Atlas `PlanarAtlas`, VRAM mit Latches `ModeXMemory`.

## Prüfung
`DriverDifferentialTests`: je Routine 40 zufällige Aufrufe (`…_32D8_…`, `…_3A9A_…`, `…_3386_…` mit w ≥ 4, `…_3449_…` mit
Atlasvergleich und unverändertem VRAM, `…_351E_…`, `LatchCopy_328E_…` zwischen A000h/A400h); nach jedem Aufruf VRAM bzw.
Atlas und Register gleich. Zusätzlich Bildprobe `Full_screen_blit_of_a_converted_picture_shows_the_picture` (V-MENU, ohne RefEmu).
Grenzfälle (Seitenende, w < 4, h = 0) offen → Teil 3.
