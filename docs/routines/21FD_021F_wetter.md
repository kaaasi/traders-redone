# 21FD:021F – Wetter der Woche

- **Ladebild-Adresse:** `21FD:021F` (Dateioffset `0x257EF`)
- **Aufgabe:** Wetter `DS:40E6` = `random(2)`; bei `random(4) == 0` zusätzlich +3 (Byte). Werte 0–5 (Texte 245–262).
- **Aufgerufen von / Phase:** Wochenschleife `1777:024F` (Schritt 10, `BESTAND.md` §1.4) und neue Partie `0E31:0010`.
- **Parameter / Rückgabe:** keine.

## Zustand
| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `40E6` | B | s | Wetter 0–5 | w, dann r/w |
| `10D2` | D | u | Zustand von `random()` | r/w |

## Seiteneffekte in Reihenfolge
Zwei Ziehungen, immer in dieser Folge: `random(2)` (near über `push cs; call 01BA`), `random(4)`. Keine Uhr (kein
`random(0)`), keine Ausgabe.

## C#-Umsetzung
`src/Traders.Core/Rules/WeatherRule.cs` (`Apply(Dgroup, OriginalRandom)`); der Aufrufer hält den Zufallszustand (für den
Vergleich mit dem RefEmu `DS:10D2` vorher laden und nachher zurückschreiben). Mehrrundenprobe: Eingang `10D2`; Ausgabe
`40E6`, `10D2`.

## Prüfung
`tests/Traders.Tests/Rules/RandomRulesDifferentialTests.cs`: 400 Startzustände (darunter 0, 1, `FFFFFFFF`), Ziehungen
gegen die Aufrufe von `21FD:01BA` im Original (Argumente 2, 4), alle sechs Wetterwerte erreicht.
