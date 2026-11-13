# 0609:074D – Feld bepflanzbar? (Helfer des Computerzugs)

- **Ladebild-Adresse:** Segment `0609` (Dateioffset `0x9690`); `074D` bei `0x9DDD`.
- **Aufgabe:** prüft, ob am Feld der **globalen** Schleifenkoordinaten `DS:2EB0` (x) / `DS:2F00` (y) ein HQ gebaut werden darf
  (gültige HQ-Platzierung): nur auf Sondergelände (Güte = 3), nur solange frei, und nicht unmittelbar links/rechts neben einem
  HQ. Kein Zufall, keine Grafik, keine Eingabe, kein Zustand geschrieben (reine Prüfung).
- **Aufgerufen von / Phase:** `135D:00AC` (Feldwahl Modus 2) und `0609:06BC` (menschlicher HQ-Bau). Diese Spur portiert
  die Routine (R5P-2), weil `135D` sie braucht; `0609:06BC` ruft sie.
- **Parameter / Rückgabe:** keine Parameter (liest `DS:2EB0`/`DS:2F00`); `AL` = 1 (bepflanzbar) oder 0, `AH` unbestimmt.
  Feldindex `x·6 + y` (16-Bit-`imul`, keine Bereichsprüfung).

## Ablauf (eigene Worte)

- `cell = 2EB0·6 + 2F00`.
- Ist die Bodengüte `45DA[cell]` ≠ 3 → `AL` = 0 (nur Güte-3-Gelände kommt in Frage).
- Ist das Feld nicht frei (`4442[cell]` ≠ 0xFF) → `AL` = 0.
- Wenn x = `2EB0` > 0: ist das linke Nachbarfeld eine Art 4 (HQ), also `2EB1[cell − 6]` = 4 → `AL` = 0.
  (`2EAB` = `2EB1` − 6 ist die Art des Feldes (x−1, y).)
- Wenn x < Spalten − 1 (`2EB0` < `08FC` − 1, vorzeichenbehaftet): ist das rechte Nachbarfeld `2EB1[cell + 6]` = 4
  (HQ) → `AL` = 0. (`2EB7` = `2EB1` + 6, Feld (x+1, y).)
- Sonst `AL` = 1.
- Die Prüfungen laufen in dieser Reihenfolge; das erste Nein beendet. Ein HQ sperrt so die unmittelbar links und rechts
  angrenzenden Felder (nicht oben/unten).

## Zustand

| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen |
|---|---|---|---|---|
| `2EB0` / `2F00` | B | s | globale Schleife Feld-x / Feld-y | L |
| `45DA` | B[9·6] | s | Bodengüte je Feld (3 = Sondergelände) | L |
| `4442` | B[9·6] | s | Besitzer je Feld (0xFF = frei) | L |
| `2EB1` | B[9·6] | s | Art je Feld (4 = HQ) | L (Nachbarn ±6) |
| `08FC` | W | s | Spalten | L |

## Seiteneffekte

Keine.

## C#-Umsetzung

`src/Traders.Core/Field/PlantingAiHelpers.cs`: `PlantingAiHelpers.CanPlant(Dgroup d)` = `074D` (liest die globalen
`LoopX`/`LoopY`). Indizes 16-bittig wie im Original.

## Prüfung

`tests/Traders.Tests/Ai/PlantingAiHelpersDifferentialTests.cs` (RefEmu `LoadOnly`, direkter `Call(0609:074D)`): ganzes DS und
`AL` gleich; Fälle mit Güte 0–4, Besitzer frei/belegt, HQ links/rechts, x am linken/rechten Rand, Spalten/Zeilen auch ≤ 0,
Koordinaten außerhalb. Gegenprobe: Nachbarprüfung links/rechts vertauscht, Güte-3-Zweig ausgelassen → rot.
