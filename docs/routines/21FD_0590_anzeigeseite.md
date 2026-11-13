# 21FD:0590 – Auf der Anzeigeseite zeichnen (Spur `bilanz`, R5)

- **Ladebild-Adresse:** `21FD:0590` (Dateioffset `0x25B60`)
- **Aufgabe:** die Zeichenseite der Spielseiten wird die gerade gezeigte Seite, damit ein Wischer direkt sichtbar zeichnet.
- **Aufgerufen von / Phase:** `0825:09C9` (Öffnungswischer der Wochenbilanz), `0B9F:0813`, `111D:053F`, `20B3:0C0C` (je ein Aufruf).
- **Parameter / Rückgabe:** keine.

## Ablauf (eigene Worte)
`DS:082E` (Index der Zeichenseite) = `DS:0830` (Index der gezeigten Seite); `DS:4516/4518` = Seitennummer `DS:40DE[082E]`;
`0B67:0238(DS:4516, Seitennummer von 0830)` – zeigt die Seite (mit dem Rücklauf von `0B67:0196/01B5`) und zeichnet auf ihr, weil
beide Nummern gleich sind; danach Rücklauf `0B67:028F`. Anders als `21FD:051F` keine Tastenabfrage (`DS:3D91/3D95`).

## Zustand
| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `082E`, `0830` | W, W | u | Index Zeichenseite, Index gezeigte Seite | s, g |
| `40DE` | DW[2] | u | Seitennummern | g |
| `4516` | DW | u | Nummer der Zeichenseite | s |
| `0238`, `023A` | W, W | u | gezeigte Seite, auf versteckter Seite zeichnen (`0B67`) | s |

## Seiteneffekte in Reihenfolge
Grafik (Bildstart, Zeichenseite des Treibers), zwei Rückläufe. Kein Zufall, keine Uhr, keine Eingabe, kein Ton.

## C#-Umsetzung
Nur Grafikzustand: `ReportScene.DrawOnShownPage` in `src/Traders.Presentation/Week/ReportScene.cs` über die Schichten
(`GraphicsLayers.SelectPages`, `WaitForRetrace`). Der Kern meldet den Aufruf als Ereignis `DrawOnShownPage` und wartet zwei Bilder.

## Prüfung
`tests/Traders.Tests/Presentation/WeekReportSceneDifferentialTests.cs` (`[DIFFERENTIAL] 21FD:0590`, durch `0825:09C9`): Bildstart,
VRAM und Rücklaufzahl am Eingang von `0590` und am folgenden Eingang von `03EB` gleich dem Original. Die übrigen Aufrufer sind nicht
portiert.
