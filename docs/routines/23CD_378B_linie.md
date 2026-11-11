# 23CD:378B / 23CD:38F2 / 23CD:06BE – Linie durchgezogen, Linie mit Muster, Auswahl (VGA, Mode X)

- **Ladebild-Adresse:** `378B` (Vektor 1C, Sprungleiste `23CD:06CA`), `38F2` (Vektor 4C, `06CF`), Auswahl `06BE`
  (kartenunabhängig: Muster `CS:04F4` = FFFFh → Vektor 1C, sonst Vektor 4C). Seitenadresse über `3162`.
- **Aufgabe:** Linie von (x1, y1) nach (x2, y2) einschließlich beider Endpunkte in der Farbe `CS:0588` auf die Zeichenseite.
- **Aufgerufen von:** `06BE` aus der Befehlsliste `0A96` (5 Stellen: Typ 9 Linie, Typ 10 Rahmen aus vier Linien).
- **Parameter / Rückgabe:** `(x1, y1, x2, y2)`; keine Rückgabe. Die Routinen tauschen die Argumente auf dem Stapel (ohne
  Wirkung für Borland-Aufrufer).

## Ablauf (eigene Worte)
Drei Zweige, in beiden Routinen gleich gebaut:
1. **x1 = x2 (senkrecht, auch für einen einzelnen Punkt):** Map Mask einmal auf `1 << (x & 3)`; Länge `y2 − y1` mit
   vorzeichenlosem Vergleich (bei y2 < y1 negiert, Schritt −80); |dy| + 1 Bytes.
2. **y1 = y2 (waagrecht):** Start beim kleineren x (vorzeichenloser Vergleich); Ebenenbits 11h um `x & 3` rotiert, je Pixel
   eins weiter, beim Übertrag aus Bit 7 ein Byte weiter.
3. **sonst (Bresenham):** Differenzen **vorzeichenbehaftet** (`jns`); ist x2 − x1 negativ, werden die Endpunkte getauscht;
   y-Schritt ±80. Die längere Achse (dy > dx vorzeichenbehaftet, sonst x) ist die Führungsachse; Fehlerterm
   2·kurz − lang, Zuschläge 2·kurz bzw. 2·kurz − 2·lang, Test auf negativ. lang + 1 Pixel.

Muster (`38F2`): Kopie von `CS:04F4` je Aufruf; vor jedem Pixel um 1 nach links rotiert, das herausgeschobene Bit (vorher
Bit 15) entscheidet über das Zeichnen. Map Mask wird bei waagrechten und flachen Linien nur für gezeichnete Pixel gesetzt.
Am Ende Map Mask 0Fh.

## Seiteneffekte in Reihenfolge
Map Mask und Video-RAM. Kein Zufall, keine Uhr, kein DS-Zugriff.

## C#-Umsetzung
`src/Traders.Vga/ModeXLines.cs`: `DrawLine` (`06BE`), `DrawLineSolid`, `DrawLinePatterned` mit gemeinsamem Rumpf.

## Prüfung
`tests/Traders.Tests/Vga/DriverLineAndObjectTests.cs`: `DrawLineSolid_378B_…`, `DrawLinePatterned_38F2_…` (Muster
zufällig, jedes siebte FFFFh), `DrawLine_06BE_…` (je 60 Linien: jede fünfte senkrecht, jede fünfte waagrecht, jede fünfte steil,
Rest beliebig; Seite und Farbe zufällig). VRAM und Register gleich. Gegenproben (lokal): Fehlertest `<= 0` statt `< 0`, Musterbit
invertiert → rot; Führungsachse `dy >= dx` bleibt grün (gleichwertig: bei dx = dy liefern beide Zweige dieselben Pixel).
Grenzfälle außerhalb des Bildes → Teil 3.
