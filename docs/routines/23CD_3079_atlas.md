# 23CD:3079 – Bild in den RAM-Atlas umwandeln (VGA)

- **Ladebild-Adresse:** `23CD:3079` (far, nicht über die Vektortabelle). Gegenstück für andere Karten `23CD:166A` – nicht
  portiert (nur VGA, `BESTAND.md` §3.3).
- **Aufgabe:** ein entpacktes `V-*.PP`-Bild zeilenweise in ebenengetrennte Zeilen umordnen (Ebene k = Pixel 4j + k ab k·80).
- **Aufgerufen von:** Ressourcenlader `186C` (eine Stelle) nach dem Entpacken.
- **Parameter / Rückgabe:** `3079(far Puffer, Zeilenbytes, Zeilen)`; Aufruf mit Puffer + 4 (Größenkopf übersprungen), 320, 256.

## Zustand
| DS-Offset | Breite | Bedeutung | gelesen/geschrieben |
|---|---|---|---|
| `2BAC` | far | Zwischenpuffer für eine Zeile | r |

## Ablauf (eigene Worte)
Je Zeile: Zeile wortweise in den Zwischenpuffer kopieren (Zeilenbytes/2 Wörter), dann viermal je Zeilenbytes/4 Byte mit
Schritt 4 zurückschreiben (Start 0, 1, 2, 3). Nächste Zeile über das Datensegment: + Zeilenbytes/16 Paragraphen.

## Seiteneffekte in Reihenfolge
Nur Puffer und Zwischenpuffer; kein Zufall, keine Uhr, keine Ausgabe.

## C#-Umsetzung
`src/Traders.Vga/ModeXMemory.cs`: `PlanarAtlas.ConvertToPlanarRows`, `PlanarAtlas.FromLinear`.

## Prüfung
`DriverDifferentialTests.Planar_row_conversion_3079_matches_original`: zufälliger Puffer in voller Atlasgröße, Original im
RefEmu (Zwischenpuffer über `DS:2BAC`) und Port, ganzer Atlas bytegleich.
