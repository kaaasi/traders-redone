# 23CD:04F6 / 23CD:3042 – Kartenwahl und VGA-Einrichtung (Mode X)

- **Ladebild-Adresse:** `04F6` (Kartenwahl), `3042` (VGA-Teil, near aus `04F6`), Vektorkopie `053C` (19 far Zeiger aus
  `23CD:2FF6` nach `CS:0004`).
- **Aufgabe:** Treiberteil zur Anzeigeart wählen und die Karte einrichten.
- **Aufgerufen von:** `0B67` (eine Stelle).
- **Parameter / Rückgabe:** `04F6(Modus)`: 0 → EGA (`0797`), 63h → `3F0A`, 2 → `1645` (Tandy-Varianten), 1 → VGA (`3042`),
  andere Werte → nichts. Keine definierte Rückgabe.

## Ablauf (eigene Worte)
`3042`: Vektoren kopieren, BIOS-Modus 13h (INT 10h), Sequencer 4 Bit 3 löschen (Chain-4 aus), CRTC 14h Bit 6 löschen
(kein Doppelwortmodus), CRTC 17h Bit 6 setzen (Bytemodus), dann `30D1`. Ergebnis: 4 Ebenen, 80 Byte je Zeile und Ebene.

## Seiteneffekte in Reihenfolge
`CS:0004…004F` (Vektoren), BIOS-Moduswechsel (Register, Bildspeicher, Standardpalette), Sequencer/CRTC wie oben. Kein
Zufall, keine Uhr, kein DS-Zugriff.

## C#-Umsetzung
`src/Traders.Vga/ModeXSetup.cs`: `SelectCard` (0, 2, 63h → `NotSupportedException`, Entscheidung „nur VGA“, `BESTAND.md`
§3.3), `InitializeVga`. Die Vektorkopie entfällt (Methoden). `[APPROX]`: Wirkung des BIOS-Moduswechsels wie im RefEmu-Stub
(alle Ebenen gelöscht, Map Mask 0Fh, Startadresse 0). Ebenfalls `[APPROX]`: das Original beginnt mit Zeichenseite `CS:0002`
= 0, der Port mit A000h.

## Prüfung
`tests/Traders.Tests/Vga/DriverEdgeCaseTests.cs` `Card_selection_04F6_sets_up_mode_x_like_the_original`: Modus 7 ändert
nichts; Modus 1 nach zufälligem VRAM, Map Mask 5, Read Map 2, Anzeigestart 4123h: VRAM, Register und Startadresse gleich,
im RefEmu Chain-4 aus, CRTC 14h/17h wie oben; Modus 0 wirft im Port. Gegenprobe (lokal): ohne Löschen → rot.
