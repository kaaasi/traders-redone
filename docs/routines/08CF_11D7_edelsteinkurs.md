# 08CF:11D7 – Edelsteinkurs der Woche

- **Ladebild-Adresse:** `08CF:11D7` (Dateioffset `0xD4C7`)
- **Aufgabe:** Grundeinheit `DS:3CF0` = Bargeldsumme / 160 und Drift des Kursfaktors `DS:4436` um Woche + 100.
- **Aufgerufen von / Phase:** Wochenschleife `1777:02BA` (Schritt 21), unbedingt (auch ohne Edelsteinhandel).
- **Parameter / Rückgabe:** keine.

## Zustand
| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `3CD2` | D[4] | s | Bargeld | r |
| `37AA` | B | s | Spielerschleife (global), danach 4 | w |
| `3CF0` | D | s | Grundeinheit | w |
| `4B9A` | W | s | Woche | r |
| `4436` | D | s | Kursfaktor | r/w |
| `10D2` | D | u | Zustand von `random()` | r/w |

## Ablauf
`W(k)` = (Woche + k) 16-bittig, mit `cwd` auf 32 Bit; Vergleiche mit F = `4436` vorzeichenbehaftet 32-bittig.
```
base = Σ cash / 160
up = (W(155) > F && random(1) != 0) || W(45) > F
up:   F = W(100) > F ? (uint16)(Woche + 100 + random(30)) : F + random(20);   F = min(F, W(160))
down: F = W(100) < F ? (uint16)(Woche + 100 - random(30)) : F - random(20);   F = max(F, W(40))
```
Der Neusatz auf Woche + 100 ± `random(30)` speichert die 16-Bit-Summe mit Hochwort 0 (nicht vorzeichenerweitert).

## Seiteneffekte in Reihenfolge
Ein bis zwei Ziehungen: `random(1)` nur, wenn F < W(155), danach genau eine von `random(30)` / `random(20)`. Ohne
16-Bit-Überlauf der Woche treten die Folgen (1, 30), (1, 20) und (30) auf; (20) allein nur mit übergelaufenem W(155).
Keine Uhr, keine Ausgabe.

## C#-Umsetzung
`src/Traders.Core/Rules/GemPrice.cs` (`Apply(Dgroup, OriginalRandom)`). Mehrrundenprobe: Eingang `3CD2`, `4B9A`, `4436`,
`10D2`; Ausgabe `37AA`, `3CF0`, `4436`, `10D2`.

## Prüfung
`tests/Traders.Tests/Rules/RandomRulesDifferentialTests.cs`: Faktoren an allen Schwellen (W(40), 45, 100, 155, 160 ± 1)
für fünf Wochen und je vier Zufallszustände, Extremwerte (Faktor `MinValue`/`MaxValue`, negative und überlaufende Wochen,
Überlauf der Bargeldsumme, 16-Bit-Neusatz), 60 Wochen in Folge; Ziehungen gegen die Aufrufe von `21FD:01BA`.
