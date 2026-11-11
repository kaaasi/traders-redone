# 0825:0813 – Vermögen, Rang, Verlauf (mit 0825:086F, 0825:088D, 0825:08EB, 0825:096D, 0825:06CB, 0825:07C4)

- **Ladebild-Adresse:** `0825:0813` (Dateioffset `0xC663`); Teile `086F`, `088D`, `08EB`, `096D`; Rang `06CB`, Verlauf `07C4`.
- **Aufgabe:** Gesamtvermögen des Spielers in `DS:37AA` (Rückgabe DX:AX); Rangfolge aller Spieler; Eintrag der Woche im
  Vermögensverlauf.
- **Aufgerufen von / Phase:** `07C4` in der Wochenschleife (`1777:0200`, Schritt 2) und `20B3:0075`; `06CB` aus
  `0609`, `0825`, `08CF`, `111D`, `150F`, `16D6`, `17CB`, `20B3` (Anzeigen und Ende); `0813` zusätzlich aus `0825:02AA`
  und `1C68:01C3`.
- **Parameter / Rückgabe:** keine Argumente; Spieler über die globale Variable `DS:37AA`. `0813` und die Teile liefern
  einen 32-Bit-Wert in DX:AX.

## Zustand
| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `37AA` | B | s | Spieler (Eingang der Teile; `06CB`/`07C4`: Schleife, danach 4) | r / w |
| `3CD2` | D[4] | s | Bargeld | r |
| `4BA8`, `3746` | B[4] u, W s | | Felder, Landpreis | r |
| `3CF4`, `4898` | W[4][4], W[4] | s | Vorrat, Warenpreis | r |
| `45D0` | W | s | Warenschleife in `088D` (global), danach 4 | w |
| `4BF2`, `3CF0`, `4436` | B u, D s, D s | | Edelsteinhandel an, Grundeinheit, Kurs | r |
| `37AF`, `300E` | B[4] s, W[4] s | | Edelsteine, Edelsteinkonto | r |
| `4BAC`, `2EEA` | W[4] s, B s (Stride 4) | | Raketen, Raketenfelder (Art 3) | r |
| `45BA`, `4520`, `43F4`, `48A0` | B u, W[4] s, W s, W s | | Ölverbrauch, Öl, Öl-/Raketenpreis | r |
| `2F06` | D[4] | s | Vermögen je Spieler (`06CB`) | w |
| `3D42` / `302A` | B[4] | u | Rang / Rang mit Stichentscheid (`06CB`) | w |
| `4B9A` | W | s | Woche (`07C4`) | r |
| `411C` | D[4][40] | s | Verlauf (`07C4`), Stride 0xA0 | w |

## Ablauf
```
land   = (short)(fields * landPrice) / 3                         // 086F: imul, cwd verwirft DX, IDIV
wares  = Σw (short)((stock[w] / 10) * price[w])                   // 088D: je Produkt nur das untere Wort, cwd
gems   = trading ? gems * (base * factor / 100) + base * credit / 100 : 0     // 08EB: LXMUL/LDIV, 32 bit
other  = (short)((rockets + rocketFields) * rocketPrice) + (oilUse == 1 ? (short)(oil * oilPrice) : 0)   // 096D
wealth = land + cash + wares + gems + other                      // 0813, 32 bit
```
Rang `06CB`: zuerst je Spieler `3D42` = `302A` = 3 und `2F06` = Vermögen; dann für jedes Paar p < q:
`W[p] ≥ W[q]` → `3D42[p]--`, `302A[p]--`, sonst `302A[q]--`; zusätzlich `W[q] ≥ W[p]` → `3D42[q]--`. `3D42` = 3 − Zahl
der anderen mit Vermögen ≤ eigenem (Gleichstand senkt beide), `302A` ist eindeutig (Gleichstand zugunsten der kleineren
Nummer). Verlauf `07C4`: Index = Woche, solange > 39 um 40 verringert (negative Wochen bleiben negativ und schreiben vor
`411C`); `411C + p·0xA0 + 4·Index` = Vermögen.

Abweichend von `DATENLAYOUT.md` §5: Edelsteine = Steine · (Grundeinheit · Kurs / 100), nicht (Steine · Grundeinheit · Kurs)/100;
Land-, Waren-, Raketen- und Ölprodukte werden auf 16 Bit abgeschnitten; Öl zählt nur bei `45BA` genau 1.

## Seiteneffekte in Reihenfolge
Kein Zufall, keine Uhr, keine Ausgabe (RefEmu-Protokoll leer, `DS:10D2` unverändert).

## C#-Umsetzung
`src/Traders.Core/Rules/Wealth.cs`: `Wealth.Total/Land/Wares/Gems/Other`, `Ranking.Apply`, `WealthHistoryRecord.Apply`.
Mehrrundenprobe (Spur 0 Teil 3, Schritt 2 = `07C4`): Eingang alle Felder der Tabelle bis `48A0` und `4B9A`; Ausgabe
`411C`, `37AA`, `45D0`.

## Prüfung
`tests/Traders.Tests/Rules/WealthProductionDifferentialTests.cs`: alle fünf Vermögensroutinen einzeln mit Rückgabewert für
300 Zustände (davon 150 mit Extremwerten: 32-Bit-Bargeld, Preise und Vorräte über 16 Bit, negative Zähler, Schalter ≠ 0/1),
gezielter 16-Bit-Überlauf jedes Produkts; Rang mit sieben Gleichstandsmustern und 150 Zuständen; Verlauf für 14 Wochen
(0, 39, 40, 79, 1239, −1, −40 …) und 100 Zustände. Verglichen werden Rückgabe, ganzes Datensegment und Protokoll.
