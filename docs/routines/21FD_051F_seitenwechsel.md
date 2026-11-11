# 21FD:051F – Seitenwechsel des Spiels

- **Ladebild-Adresse:** `21FD:051F` (Dateioffset `0x25AEF`).
- **Aufgabe:** Zeichen- und Anzeigeseite des Spiels tauschen.
- **Aufgerufen von:** Bildschleifen (`21FD:0004`, `060D`, `0634`, `0665`) und Szenen.
- **Parameter / Rückgabe:** keine.

## Ablauf (eigene Worte)
1. `DS:082E` ^= 1 (16 Bit); `DS:0830` = 1, wenn `082E` jetzt 0 ist, sonst 0.
2. `DS:4516` = Seitennummer `DS:40DE[082E]` (long, Index · 4 in 16 Bit).
3. `0B67:0238(4516, 40DE[0830])` (fünftes Argument FFFFh wird nicht gelesen): Anzeige und Zeichenseite wählen.
4. `DS:3D95` = 0 → `0B67:028F` (Rücklauf abwarten); `DS:3D91` ≠ 0 → `21FD:0185(4)`.

## Zustand
| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `082E` | W | – | gezeichnete Seite (Index) | g/s |
| `0830` | W | – | angezeigte Seite (Index) | s |
| `40DE` | D[2] | – | Seitennummern | g |
| `4516` | D | – | aktuelle Zeichenseitennummer | s |
| `3D95`, `3D91` | B | – | Wartesteuerung | g |
| `0238`, `023A` | W | – | über `0B67:0238` | g/s |

## Seiteneffekte in Reihenfolge
CRTC-Start und Rücklaufwarten (`0B67:0196/01B5`), Zeichenseite, weitere Rücklaufwarten. Kein Zufall, keine Uhr.

## C#-Umsetzung
`src/Traders.Vga/Layers/Pages.cs`, `GraphicsLayers.FlipPages`. Nicht in Spur 2: `21FD:0590` (Zeichenseite = Anzeigeseite) und
die Bildschleifen `060D/0620/0634/0665` (außerhalb der Dateigrenzen; sie verbinden `0A96:06A8` und `23CD:06E9/071C`).

## Prüfung
`tests/Traders.Tests/Vga/PageLayerTests.cs`: `FlipPages_21FD_051F_matches_original` (120 Wechsel; `082E` auch ≠ 0/1,
Seitennummern 1/2 und abweichende 0, 3, 10001h; `3D95`/`3D91` zufällig), `FlipPages_alternates_shown_and_draw_page`.
