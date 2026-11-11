# 23CD:05DD / 23CD:31FD / 23CD:05B6 / 23CD:05C3 – Textausgabe, Zeichen, Schrift und Farbe setzen (VGA)

- **Ladebild-Adresse:** `05DD` (Text, kartenunabhängig), `31FD` (Vektor 0C, ein Zeichen, VGA), `05B6` (Schrift-Nr. nach
  `CS:0586`), `05C3` (Farbe nach `CS:0588`). Schrifttabelle `CS:057A`, gesetzt von `058A` (aus `0DFD`).
- **Aufgabe:** nullterminierte Zeichenkette mit der aktuellen Schrift in der aktuellen Farbe zeichnen.
- **Aufgerufen von:** `05DD` aus `0A96:0C68` (Befehlsliste Typ 12/13) und `0C29`; `05B6` aus `0DFD`, `21FD`; `05C3` aus `0A96`,
  `0B67`, `0C29`.
- **Parameter / Rückgabe:** `05DD(x, y, far Zeichenkette)`, `y` = unterste Zeile der Zeichen; `05B6(n)`, `05C3(Farbe)`.

## Ablauf (eigene Worte)
- Tabelleneintrag 10 Byte: far Zeiger auf die Zeichen, Höhe, Vorschub, erstes Zeichen. `05DD` berechnet den Eintrag
  (Nr.·10) mit Segmentkorrektur und setzt `top = y − Höhe + 1`.
- Je Zeichen: `c & 7Fh`, 0 beendet; Zeichenadresse = (c − erstes Zeichen)·2·Höhe; Aufruf von Vektor 0C, dann x += Vorschub.
  Zum Schluss Vektor 04 (`30D1`).
- `31FD`: Zeichen 16 Pixel breit, 2 Byte je Zeile (big-endian, höchstwertiges Bit links). Je Ebene wird das Zeilenwort um
  `(4 − x&3 + Ebene) & 3` rotiert und die Bits 15, 11, 7, 3 auf vier Bytes verteilt; Höhe als Byte (0 = 256 Zeilen).

## Seiteneffekte in Reihenfolge
Map Mask, Read Map (Rücksetzen) und Video-RAM. Kein Zufall, keine Uhr, kein DS-Zugriff.

## C#-Umsetzung
`src/Traders.Vga/ModeXDriver.cs`: `DrawText`, `DrawGlyph`, Eigenschaften `Font`, `Color`; Tabelle als `FontDescriptor`.

## Prüfung
`DriverDifferentialTests.DrawText_05DD_matches_original_for_both_fonts` (40 Zeichenketten bis 15 Zeichen, beide Schriften
aus `FONTS.PP`, Farbe und Position zufällig); `Driver_state_setters_and_controller_reset_match_original` vergleicht
`CS:0586/0588` nach `05B6`/`05C3`. Zeichen über dem letzten Schriftzeichen und Segmentüberläufe sind nicht geprüft.
