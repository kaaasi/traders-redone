# 0B9F:06A0 – Warenpreise vor und nach der Versteigerung (mit 0B9F:078A)

- **Ladebild-Adresse:** `0B9F:06A0` (Dateioffset `0xF690`), `0B9F:078A` (`0xF77A`)
- **Aufgabe:** `06A0` hebt die Preise `DS:4898` auf ihr Minimum (höchstens 300); `078A` setzt den Preis der Ware `DS:45D0`
  nach ihrer Versteigerung.
- **Aufgerufen von / Phase:** `06A0` zu Beginn des Handels `0B9F:0005`; `078A` am Ende einer Versteigerung `0C29:007F`.
- **Parameter / Rückgabe:** keine. Entgegen der Planannahme **ohne Zufall**.

## Zustand
| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `3CD2` | D[4] | s | Bargeld (`06A0`) | r |
| `37AA` | B | s | Spielerschleife (`06A0`), danach 4 | w |
| `45D0` | W | s | Warenschleife (`06A0`, danach 4) / Ware (`078A`, Eingang) | w / r |
| `4B9A` | W | s | Woche | r |
| `4898` | W[4] | s | Warenpreis | r/w |
| `2BC2` | W | s | Startpreis der Versteigerung (= Preis, `0B9F:0093`) | r |
| `2BBE` | W | s | Höchstgebot, 0 = keins (`0C29:04B6`, vorläufig) | r |

## Ablauf
```
06A0: min = kleinstes Bargeld, Start 1000000;  t = (int16)(300 - (10000 - min) / 58)   // 32-Bit-Sub, LDIV
      für w = 0..3: m = w == 3 ? Woche/2 + 3 : (min / t) unteres Wort; preis = max(preis, m); preis = min(preis, 300)
078A: w == 3: preis = Woche/2 + 3 (ohne Grenzen)
      sonst: kein Gebot → Start·7/10; Gebot = Start → Start·9/10; sonst Start + (Gebot - Start)/3; dann 3..300
```
Alle Produkte und Differenzen in `078A` 16-bittig. `t` = 0 (kleinstes Bargeld −7400 … −7457) endet in beiden im
Divisionsfehler.

## Seiteneffekte in Reihenfolge
Keine Ziehung, keine Uhr, keine Ausgabe.

## C#-Umsetzung
`src/Traders.Core/Rules/WarePrices.cs` (`RaiseToMinimum`, `AfterAuction`). Mehrrundenprobe: gehört zum Handel (R8); hier
nur die Routinen.

## Prüfung
`tests/Traders.Tests/Rules/RandomRulesDifferentialTests.cs`: zehn Bargeldmuster × neun Wochen mit beliebigen Startpreisen
(einschließlich Divisor 0), Versteigerung für sechs Waren (auch −1 und 4), 22 Startpreise × 7 Gebote; jeweils keine Ziehung.
