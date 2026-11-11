# 23CD:31A6 / 23CD:373B / 23CD:3764 – Rechteck füllen, Pixel setzen, Pixel lesen (VGA, Mode X)

- **Ladebild-Adresse:** Vektorziele des VGA-Teils (Vektortabelle `23CD:2FF6`, nach `CS:0004` kopiert von `23CD:053C`):
  `31A6` (Vektor 10, Sprungleiste `23CD:069B`), `373B` (Vektor 30, `06D9`), `3764` (Vektor 34, `06DE`).
  Hilfen: Seitenadresse `3162` (y·80 + x/4), Ausrichtung `312D` mit den Tabellen `315A` (Startversatz je Ebene) und
  `315E` (Zusatzbyte je Ebene nach `w & 3`, um `x & 3` rotiert).
- **Aufgabe:** gefülltes Rechteck in der aktuellen Farbe (`CS:0588`, nur Low-Byte); ein Pixel setzen; ein Pixel der
  Zeichenseite lesen.
- **Aufgerufen von:** `31A6` über die Befehlsliste (`0A96`, Typ 11) und `0B67`; `373B` über `0A96` (Typ 15); `3764` direkt
  aus `0EDE`, `111D`, `1B03`, `20B3` (8 Stellen).
- **Parameter / Rückgabe:** `31A6(x, y, w, h)`; `373B(x, y, Farbe)`; `3764(x, y)` → `ax` = Pixel (`ah` = 0).

## Zustand
| Ort | Breite | Bedeutung | gelesen/geschrieben |
|---|---|---|---|
| `CS:0002` | Wort | Segment der Zeichenseite (`311A`) | r |
| `CS:0588` | Wort | Farbe (`05C3`) | r (`31A6`) |
| Sequencer 2 | Byte | Map Mask | w |
| Graphics Controller 4 | Byte | Read Map | w (`3764`) |

## Ablauf (eigene Worte)
- `31A6`: für jede Ebene 1, 2, 4, 8 (Map Mask) wird der Startoffset um ein Byte erhöht, wenn die Ebene links vor `x`
  liegt, und die Bytezahl `w/4` um eins, wenn die Ebene ein Restpixel trägt; dann `h` Zeilen zu je 80 Byte.
  Am Ende Map Mask = 0Fh. Zeilenzähler läuft bis 0 herunter: `h` = 0 bedeutet 65536 Zeilen (Grenzfall, Teil 3).
- `373B`: Map Mask = `1 << (x & 3)`, ein Byte schreiben, danach Map Mask = FFh (nicht 0Fh).
- `3764`: Read Map = `x & 3`, ein Byte lesen, danach direkter Aufruf von `30D1` (Read Map 0).

## Seiteneffekte in Reihenfolge
Nur VGA-Register und Video-RAM; kein Zufall, keine Uhr, kein DS-Zugriff.

## C#-Umsetzung
`src/Traders.Vga/ModeXDriver.cs`: `FillRect`, `SetPixel`, `GetPixel`; VRAM-Modell `ModeXMemory`.

## Prüfung
`tests/Traders.Tests/Vga/DriverDifferentialTests.cs` (RefEmu, `DriverHarness`: zufälliges VRAM, Seite A000h/A400h zufällig):
`FillRect_31A6_matches_original` (40 Aufrufe, Farbe, x, y, w ≤ 90, h ≤ 50 zufällig),
`SetPixel_373B_and_GetPixel_3764_match_original` (200 gemischte Aufrufe, Rückgabe `al` verglichen), `Drawing_goes_to_the_selected_page`.
Nach jedem Aufruf ganzes VRAM (4 × 64 KB), Map Mask und Read Map gleich. Grenzfälle (`h` = 0, Ränder) offen → Teil 3.
