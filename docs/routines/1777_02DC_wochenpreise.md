# 1777:02DC – Wochenpreise

- **Ladebild-Adresse:** `1777:02DC` (Dateioffset `0x1B34C`)
- **Aufgabe:** Ladenvorrat Öl, Öl-, Raketen-, Lotto- und Landpreis der Woche aus Bargeld, Feldzahlen, Woche und
  Lotto-Einstellung.
- **Aufgerufen von / Phase:** Wochenschleife `1777:01F8` als erster Schritt (Schritt 1 in `BESTAND.md` §1.4), vor
  `0825:07C4`.
- **Parameter / Rückgabe:** keine.

## Zustand
| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `3CD2` | D[4] | s | Bargeld je Spieler | r |
| `08FC` / `08FE` | W | s | Spalten / Zeilen | r |
| `2F05` | B | s | Feldanzahl | r |
| `4B9A` | W | s | Woche | r |
| `08F9` | B | s | Lotto-Einstellung 0 LOW, 1 NORM, 2 HIGH | r |
| `37AA` | B | s | Spielerschleife (global), danach 4 | w |
| `4B98` | W | s | Ölvorrat des Ladens | w, dann r |
| `43F4` | W | s | Ölpreis | w |
| `48A0` | W | s | Raketenpreis | w |
| `34A2` | W | s | Lottogewinn (vorläufig), nur bei Einstellung 0–2 neu | r/w |
| `40CC` | W | s | Lottopreis (vorläufig) = `34A2`/32 | w |
| `3746` | W | s | Landpreis | w |

## Ablauf
```
min = 1000000; sum = 0;                                   // 32 bit; min: vorzeichenbehafteter 32-Bit-Vergleich
for (p = 0; p < 4; p++) { if (cash[p] < min) min = cash[p]; sum += cash[p]; }   // Zähler DS:37AA
avg = sum / 4;                                            // LDIV 0000:4F89
t = (cols*rows - total) * 2 + 1;                          // 16 bit (imul: unteres Wort), cwd → immer ungerade
shopOil = (sum * week) % t;                               // LXMUL 0000:4F4E, LMOD 0000:4F98; unteres Wort
oil = min / (shopOil/4*10 + 200);                         // 16-bit IDIV/IMUL/ADD, Divisor cwd, LDIV; unteres Wort
if (avg/1000 > oil) oil = avg/1000;                       // 32-Bit-Vergleich mit cwd(oil), dann unteres Wort
if (oil < 1) oil = 1;
rocket = avg/70 + 1;  if (sum % 5 == 0) rocket <<= 1;     // 16 bit
switch (lotto) { 0: prize = avg/20; 1: avg/10; 2: avg/5; sonst unverändert }
if (prize < 20) prize = 20;  lottoPrice = prize / 32;     // 16-bit IDIV
land = avg/10; clamp 100..550                             // 16-Bit-Vergleiche nach dem Abschneiden
```
Laufzeitdivision (`0000:4FA3`): Beträge, Vorzeichen danach – rundet gegen null, Rest mit Vorzeichen des Dividenden,
`MinValue / -1` = `MinValue` ohne Fehler; Divisor 0 endet in `div bx` (`0000:5027`) → Divisionsfehler.

## Seiteneffekte in Reihenfolge
Kein Zufall (`DS:10D2` unverändert), keine Uhr, keine Grafik, Eingabe oder Ton (RefEmu-Protokoll leer; Probe P2,
BESTAND B1). Einziger Fehlerfall: Öl-Divisor 0 bei Ladenvorrat −80…−83 (nur mit negativem Bargeld erreichbar); dann ist
`4B98` schon geschrieben, die übrigen Preise nicht.

## C#-Umsetzung
`src/Traders.Core/Rules/WeeklyPrices.cs` (`Apply(Dgroup)`), Laufzeitarithmetik `src/Traders.Core/Rules/LongMath.cs`.
Eingangsfelder für die Mehrrundenprobe (Spur 0 Teil 3): `3CD2`, `08FC`, `08FE`, `2F05`, `4B9A`, `08F9`, `34A2`;
Ausgabefelder: `37AA`, `4B98`, `43F4`, `48A0`, `34A2`, `40CC`, `3746`.

## Prüfung
`tests/Traders.Tests/Rules/WeeklyRulesDifferentialTests.cs`: Probe P2 (Öl 20, Raketen 292, Lotto 63, Land 550), jede
Lotto-Einstellung einschließlich ungültiger, 18 Grenzfälle je Einstellung 0–2 (32-Bit-Überlauf der Summe, negatives
Bargeld, Preise über 16 Bit, Divisor 1/−1/−32767, `MinValue % −1`, negative Woche), 400 zufällige Zustände, Division durch
null in beiden. Verglichen wird das ganze Datensegment einschließlich `DS:10D2` und das leere Ereignisprotokoll.
