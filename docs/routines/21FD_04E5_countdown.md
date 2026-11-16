# 21FD:04E5 / 21FD:04F9 – Countdown

- **Ladebild-Adresse:** Segment `21FD`; `04E5` (Größe 14h), `04F9` (Größe 26h).
- **Aufgabe:** ein Zähler, der alle *Takt* Aufrufe um eins sinkt; begrenzt im Handel (`0C29`) Rollenwahl und Bietphase, in R8 auch
  den Edelsteinhandel (`08CF`).
- **Aufgerufen von / Phase:** `04E5` aus `0C29:0086`, `0C29:01B7`, `08CF:0004`; `04F9` aus `0C29:015C`, `0C29:01B7`, `08CF:01BD`.
- **Parameter / Rückgabe:** `04E5(Wert, Takt)`; `04F9()` gibt in AL 1 zurück, wenn der Wert gesunken ist, sonst 0 (AH ist der
  Rest des Zwischenwerts, wird nicht verwendet).

## Zustand
| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `3780` | Wort | mit | verbleibender Wert | `04E5` s, `04F9` l/s |
| `2F16` | Wort | – | Takt (Aufrufe je Schritt) | `04E5` s, `04F9` l |
| `2F18` | Wort | – | Aufrufe bis zum nächsten Schritt | `04E5` s, `04F9` l/s |

## Seiteneffekte in Reihenfolge
`04F9` ruft zuerst die Pausenprüfung `21FD:0245`, dann `2F18` − 1; bei 0: `3780` − 1, `2F18` = `2F16`, AL = 1. Kein Zufall.

## C#-Umsetzung
`Traders.Core/Rules/Countdown.cs` (`Start`, `Tick`; die Pausenprüfung ruft der Aufrufer vorher).

## Prüfung
`tests/Traders.Tests/Trade/TradeRulesDifferentialTests.cs` (`Countdown_start`, `Countdown_tick`): Takt 0/−1/1, Zähler 0/1/−32768
(Unterlauf), Pausenprüfung als aufgezeichneter Aufruf.
