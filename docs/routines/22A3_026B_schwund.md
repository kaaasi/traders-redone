# 22A3:026B – Schwund der Vorräte

- **Ladebild-Adresse:** `22A3:026B` (Dateioffset `0x2629B`)
- **Aufgabe:** alle Vorräte aller Spieler auf 8/10: `stock = (stock << 3) / 10`.
- **Aufgerufen von / Phase:** Wochenschleife `1777:01F8` (Schritt 12, `BESTAND.md` §1.4).
- **Parameter / Rückgabe:** keine.

## Zustand
| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `3CF4` | W[4][4] | s | Vorrat je Spieler (Schritt 8) und Ware (Schritt 2) | r/w |
| `37AA` | B | s | Spielerschleife (global), danach 4 | w |
| `45D0` | W | s | Warenschleife (global), danach 4 | w |

## Ablauf
`stock << 3` mit drei 16-Bit-`shl`, dann `cwd`/`idiv 10` (rundet gegen null). Ab 4096 läuft das Achtfache über (P3 #39 R1):
4095 → 3276, 4096 → −3276, 8191 → 0 (−8 / 10), 8192 → 0. Im Spiel ist der Vorrat auf 999 begrenzt.

## Seiteneffekte in Reihenfolge
Kein Zufall, keine Uhr, keine Ausgabe (Probe P3, RefEmu-Protokoll leer).

## C#-Umsetzung
`src/Traders.Core/Rules/StockDecay.cs` (`Apply(Dgroup)`). Eingang für die Mehrrundenprobe: `3CF4`; Ausgabe: `3CF4`, `37AA`,
`45D0`.

## Prüfung
`tests/Traders.Tests/Rules/WeeklyRulesDifferentialTests.cs`: Probe P3 (70, 999, 5, 13 → 56, 799, 4, 10), 24 Grenzwerte
um 4096, 8192, 16384 und negative Werte, 200 zufällige Zustände; ganzes Datensegment verglichen.
