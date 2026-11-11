# 23CD:311A / 23CD:30E9 / 23CD:30D1 / 23CD:05D0 – Zeichenseite, Anzeigestart, Rücksetzen, Linienmuster (VGA)

- **Ladebild-Adresse:** `311A` (Vektor 44, Sprungleiste `23CD:0570`), `30E9` (Vektor 08, `0575`), `30D1` (Vektor 04, `056B`),
  `05D0` (Linienmuster nach `CS:04F4`, kartenunabhängig).
- **Aufgabe:** Zielseite für alle Zeichenroutinen wählen; sichtbare Seite umschalten; Graphics Controller in den
  Grundzustand; Muster für Linien setzen (FFFFh = durchgezogen, wertet `06BE` aus).
- **Aufgerufen von:** `0570`/`0575` aus `0B67` (Seitenverwaltung, 4 bzw. 2 Stellen); `30D1` innerhalb des Treibers nach
  Lesezugriffen (direkt oder über Vektor 04); `05D0` aus `0A96` (Typ 14).
- **Parameter / Rückgabe:** `311A(Offset, Segment)`; `30E9(Offset, Segment)`; `30D1()`; `05D0(Muster)`.

## Ablauf (eigene Worte)
- `311A` speichert Offset und Segment in `CS:0000/0002`. Alle Zeichenroutinen adressieren nur über das Segment; der
  Offset wird nicht verwendet.
- `30E9`: Startadresse = (Segment − A000h)·16 + Offset, Ausgabe auf CRTC 0Ch/0Dh am Port aus `0040:0063`, unter `cli`;
  danach CRTC 08h (Preset Row Scan) = 0.
- `30D1`: Graphics-Controller-Register 0–4 = 0 (u. a. Read Map 0), 7 und 8 = FFh; Register 5 (Modus) bleibt unverändert.

## Seiteneffekte in Reihenfolge
VGA-Register; `CS:0000/0002/04F4`. Kein Zufall, keine Uhr, kein DS-Zugriff. Auf Strahlrücklauf wird hier nicht gewartet.

## C#-Umsetzung
`src/Traders.Vga/ModeXDriver.cs`: `SetDrawPage`, `SetDisplayStart`, `ResetGraphicsController` (modelliert nur Read Map;
die übrigen Register bleiben im Modell fest auf dem Grundzustand), Eigenschaft `LinePattern`.

## Prüfung
`DriverDifferentialTests.SetDisplayStart_30E9_matches_original` (vier Seiten, CRTC-Startadresse gleich),
`Driver_state_setters_and_controller_reset_match_original` (40 zufällige Werte: `CS:0000/0002/04F4`, Read Map nach `30D1`),
`Drawing_goes_to_the_selected_page` (Seiten A000h, A400h, A800h, AC00h, A123h); `tests/Traders.Tests/Vga/DriverEdgeCaseTests.cs` `Page_flip_shows_the_page_drawn_to`: auf A400h/A000h zeichnen, Anzeigestart umschalten, das vom RefEmu dargestellte Bild (320 × 200) gleich `PageToLinear(DisplayStart)`. Offsets jenseits des A000-Fensters (Segment·16 + Offset > AFFFFh, Umlauf bei 1 MB): im Original gewöhnlicher Speicher statt Bildspeicher; der Port führt ihn getrennt (`ModeXMemory`, `[APPROX]`: anfangs null wie im RefEmu ab B0000h), Lesen dort lädt keine Latches. Geprüft in `tests/Traders.Tests/Vga/DriverWrapTests.cs` (`B2_…`, `Zero_rows_on_page_A400_…`: `31A6` mit h = 0 auf A400h, `328E` von/nach B000h/B100h, `351E` aus Zeilen hinter dem Fenster), Befund B2 des Gesamtreviews R2 (#58).
