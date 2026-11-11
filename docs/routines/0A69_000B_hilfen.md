# 0A69:000B / 0A69:002C / 0A69:004D / 0A69:0090 / 0A69:00D3 / 0A69:011B / 0A69:0164 / 0A69:01AD / 0A69:0243 / 0A69:0288 – Zeichenhilfen

- **Ladebild-Adresse:** Segment `0A69` (Dateioffset `0xDC90`), `000B` bei `0xDC9B`.
- **Aufgabe:** Linien, Schattentexte, zentrierte Texte, Zahlen mit Schatten und Schreibmaschinentext über die Befehlsliste `0A96`.
- **Aufgerufen von:** Szenen (Menüs, Berichte, Meldungen).

## Ablauf (eigene Worte)
- `000B(x, y, w, F)`: Linie x … x + w − 1; `002C(x, y, h, F)`: Linie y … y + h − 1 (`0433`).
- `004D(x, y, Text, F, S, ox, oy)`: Text bei (x + ox, y + oy) in S, dann bei (x, y) in F, beide ohne Schattenrechteck.
- `0090(x, y, w, Text, F, Schatten)`: x' = x + 1 + (w − strlen·[3CA8]) ≫ 1 (vorzeichenlos); `00D3` dasselbe mit `004D`.
- `011B`/`0164(x, y, Zahl, Stellen, F, S, ox, oy)`: `057A` bzw. `05FB` zweimal wie `004D`.
- `01AD(x, y, Text, F, Warten, Schluss)`: je Zeichen i: `DS:0140` = Zeichen, Liste leeren, Text `DS:0140` bei x + i·[3CA8],
  `21FD:060D` (Liste, Seitenwechsel, Liste), außer bei Leerzeichen `21A7:0054(1, 16h)` (Tastenklick), `21FD:0245` (wartet nur bei
  `DS:3D60` ≠ 0 auf den Zeitgeber), `0185(Warten)`. Nullzeiger überspringt die Zeichen. Danach `0185(Schluss)`, wenn > 0.
- `0243`: `01AD` zentriert wie `0090` (Nullzeiger: gar nichts). `0288(x, y, w, far Zeilen, n, F, Warten)`: Zeile i mit `0243` bei
  y + i·([34A4] + [34A4]/5), Schluss −1.

## Seiteneffekte in Reihenfolge
DS (Liste, `0140`); bei `01AD` zusätzlich Bild, Seitenwechsel, Ton und Zeitgeber-Warten wie beschrieben. Kein Zufall.

## C#-Umsetzung
`src/Traders.Vga/Layers/DrawHelpers.cs`. Tastenklick und Zeitgeber-Warten gehen als Ereignisse an `ITypewriterEffects`
(Ton: Spur `host`/R10; Zeitgeber: Kern, R4); `21FD:060D` ist im Port inline (Liste, `FlipPages`, Liste).

## Prüfung
`tests/Traders.Tests/Vga/CommandListTests.cs`: `Helpers_0A69_match_original` (fünf Runden, Schrittweite [3CA8] auch FFF0h,
Nullzeiger), `Typewriter_0A69_matches_original` (Ton aus, `DS:3D60` = 0; Klicks am Einsprung `21A7:0054` gezählt).
