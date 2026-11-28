# 12C8:000B, 12C8:003B, 12C8:005A, 12C8:00B5, 12C8:015C, 12C8:024E, 12C8:02BB, 12C8:02EA, 12C8:030B, 12C8:0539, 12C8:0621, 12C8:06D6, 12C8:08EA – Aufstellungs-KI der Roboterphase (Sammelbefund)

- **Ladebild-Adresse:** Segment `12C8` (Dateioffset `0x16280`, Größe `0x950`); `12C8:08EA` endet in den ersten 14 Byte von
  Segment `135D` (`135D:0000`–`000D`, Ziel des Sprungs `EB 0B` bei `12C8:094F`).
- **Aufgabe:** Computergegner in der Aufstellung vor dem Kampf: Felder des eigenen Quadranten bewerten, Mittel auf Ringe um die
  eigene Ecke verteilen, Zielzellen wählen und je Bild die Steuerbits `DS:37C3` setzen, die `0E9E:000E` wie eine Eingabe liest.
- **Aufgerufen von / Phase:** Aufruf 18 der Woche (`1C68:000B`): `12C8:000B` einmal nach `150F:012E`; `0535:00B4` (aus
  `0535:000A`) ruft `12C8:003B` je Computer (`0840[p] = 1`) mit `37AA = p`; `0535:012B` ruft `12C8:0539` in jedem Bild der
  Aufstellungsschleife vor `0E9E:000E`.
- **Parameter / Rückgabe:** Einsprünge von außen ohne Parameter; Hilfen siehe unten.
- **Aufrufgraph** (`tools/callgraph.py`, R10-Teil 1 nachgeprüft): 13 Routinen, 0x953 Byte, nur Aufrufe innerhalb von `12C8`, keine
  Aufrufstelle von `random`, kein Bild, kein Ton. Direktes Schreibziel nur `37AA` (`0539`); indizierte Schreibziele aus dem
  Disassembly: `44C6`, `44CE`, `374C`, `2E98`, `4650`, `45CC`, `454A`, `4552`, `37C3`.

## Routinen

| Adresse | Name im Port | Inhalt |
|---|---|---|
| `000B` | `ResetMarks` | `44C6[0..7]`: Zeilen 0, 1, 6, 7 = C3h, Zeilen 2–5 = 0 (Ränder des Grobrasters gelten als markiert). |
| `003B` | `Plan` | `4650[p] = 0`, dann `005A`, `030B`, `06D6`. |
| `005A` | `MarkQuadrant` | Ringe r = 0..3, Schritt s = 0..2r: Block `00B5(r, s)`; ist `4478[8x+y]` ≠ 0 → `015C(x, y)`. |
| `00B5` | `RingBlock` | (r, s, far x, far y): s ≤ r: (cx + dx·r, cy + dy·s), sonst (cx + dx·(2r − s), cy + dy·r); Bytes `07C0/07C4/07C8/07CC[p]` vorzeichenbehaftet, 16-Bit-`imul`. |
| `015C` | `MarkBetween` | Acht Nachbarn (`0878/0880`) im Raster und belegt: einer mit Abstand 4 **und** einer mit Abstand 2 → Block selbst und jeder solche Nachbar mit Abstand 2 markiert (`02BB(…, 1)`). |
| `024E` | `Distance` | x oder y außerhalb 0..7 → −1; sonst max(\|x − cx\|, \|y − cy\|), vorzeichenbehaftet. |
| `02BB` | `SetMark` | (x, y, b): Byte b = 1 → `44C6[y] |= 08C6[x]`, sonst `&= FFh − 08C6[x]`. |
| `02EA` | `IsMarked` | AL = 1, wenn `44C6[y] & 08C6[x]`, sonst 0. |
| `030B` | `PlanRings` | Zählt markierte Blöcke c[r] je Ring; L = min(`2EF8[p]`/20, `3016[p]`) vorzeichenlos; e = `40EC[p]`, q = min(e/16, L) vorzeichenbehaftet. Gang 1 (r = 0..3): r ≥ 1 und c[r] ≤ q → `44CE = 2`, `374C = 1`, q −= c[r], e −= 16·c[r]; sonst beide 0. Gang 2 (r = 3..0): r = 0 mit Schritt 4, genommene Ringe mit 8·c[r]: solange e ≥ Schritt, e −= Schritt, `44CE` + 1, Abbruch bei ≥ 6 (Byte vorzeichenbehaftet). Gang 3 (r = 1..3, s = `33DC[p]`): `44CE` = 0 und 9·c[r] ≤ s → `2E98 = 1`, s −= 9·c[r]; sonst 0. |
| `06D6` | `ChooseTarget` | Zielsuche im Quadranten, siehe unten. |
| `0539` | `Steer` | Je Bild für p = 0..3 (schreibt `37AA`, endet bei 4), nur Computer: `37C3[p] = 0`; Bewegungs- oder Feuerverzögerung (`3786`/`302E`, Doppelwort vorzeichenbehaftet) > 0 → weiter. Modus `43B2[p]` = 0Eh: Zeiger ≠ Ziel → `0621`; sonst `45CC[p]` > 0 → `37C3 = 10h`, `45CC` − 1; sonst `06D6`. Anderer Modus → `08EA`. |
| `0621` | `SteerToTarget` | Ziel x > Zeiger x → Bit 1, < → Bit 2; Ziel y > Zeiger y → Bit 8, < → Bit 4 (Wörter vorzeichenbehaftet). |
| `08EA` | `SteerByPass` | `2E90[p]` < `4650[p]` (vorzeichenlos) → Bit 1, > → Bit 2, gleich → `37C3 = 10h`. |

**`06D6` im Einzelnen:** x, y = Low-Bytes von `3052/305E[p]`; stehen sie auf dem Start (`0844/0848[p]`), beginnt die Suche bei
(`0854[p]` − 1, `085C[p]`). Schleife: x + 1; x > `0858[p]` → x = `0854[p]`, y + 1; y > `0860[p]` → `4650[p]` + 1, Ziel = Start,
`45CC = 1`, fertig (alles vorzeichenlose Bytes). Zelle `48F4[26x + y]` / 8 muss 0 oder 2 sein; Block ((x − 1)/3, (y − 1)/3)
markiert (`02EA`); r = `024E` des Blocks. `4650` = 2: `2E98[p·4 + r]` ≠ 0 → `45CC = 1`, fertig. Sonst `44CE[p·4 + r]` ≠ 0 nötig;
`4650` = 0: zusätzlich `374C` ≠ 0 und (x − 2) mod 3 = 0 und (y − 2) mod 3 = 0 (Blockmitte) → `45CC = 1`; andere Gänge →
`45CC = 44CE`. Ziel als vorzeichenerweiterte Bytes nach `454A/4552[p]`.
**Endlichkeit:** Innerhalb der Schleife ändern sich nur x und y; mit den Tabellen aus `T.EXE` (`0858`/`0860` ≤ 24) endet sie
nach höchstens einem Durchlauf des Quadranten. Nur bei `0858` oder `0860` = FFh liefe das Original endlos; der Port wirft dann
nach 256·257 + 1 Schritten (`TargetSearchLimit`, beweisbar Wiederholung), Tests rufen mit Befehlsbudget.

## Zustand
| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `07C0`/`07C4` | B[4] | s | Ecke des Spielers im Grobraster (0,7,7,0 / 0,0,7,7) | g |
| `07C8`/`07CC` | B[4] | s | Richtung von der Ecke ins Feld (±1) | g |
| `0840` | B[4] | u | `IsComputer` | g |
| `0844`/`0848` | B[4] | u | Startzelle (3,22,22,3 / 3,3,22,22) | g |
| `0854`–`0860` | B[4] je | u | Quadrant: x min/max (1/12 bzw. 13/24), y min/max | g |
| `0878`/`0880` | B[8] | s | Nachbarversatz, im Uhrzeigersinn ab (0, −1) | g |
| `08C6` | B[8] | u | Bitmasken 01h…80h | g |
| `2E90` | B[4] | u | Vergleichswert zu `4650` in `08EA` (von `0535`) | g |
| `2E98` | B[4][4] | u | Ring noch aus `33DC` bezahlbar | g/s |
| `2EF8`, `3016`, `40EC`, `33DC` | W[4] | u/u/s/s | Roboterfelder 21, 25, 23, 24 (Mittel, Obergrenzen) | g |
| `302E`, `3786` | DW[4] | s | Feuer- und Bewegungsverzögerung von `0E9E` | g |
| `3052`/`305E` | W[4] | s | Zeiger der Aufstellung (Feldzelle) | g |
| `374C` | B[4][4] | u | Ring in Gang 1 genommen | g/s |
| `37AA` | B | s | aktueller Spieler, `0539` als Schleifenzähler | g/s |
| `37C3` | B[4] | u | Steuerbits (1, 2, 4, 8 Richtungen, 10h Feuer) | s |
| `43B2` | B[4] | u | Modus; 0Eh = Roboter setzen | g |
| `4478` | B[8][8] | u | Grobraster 8x + y, ≠ 0 belegt (von `1E7E`/`150F`) | g |
| `44C6` | B[8] | u | Markierungen: Bit `08C6[x]` in Byte y | g/s |
| `44CE` | B[4][4] | s | geplante Einheiten je Ring (bis 6) | g/s |
| `454A`/`4552` | W[4] | s | Zielzelle | g/s |
| `45CC` | B[4] | u | verbleibende Feuerdrücke am Ziel | g/s |
| `4650` | B[4] | u | Gang der Zielsuche | g/s |
| `48F4` | B[26][26] | u | Kampffeld (Spur `feld`), Zelle 26x + y | g |

## Seiteneffekte in Reihenfolge
Keine: kein Zufall (`DS:10D2` unverändert), keine Uhr, keine Grafik, keine Eingabe, kein Ton. `0539` schreibt nur Steuerbits; die
Bewegung des Zeigers macht `0E9E:000E` (Teil 2).

## C#-Umsetzung
`src/Traders.Core/Ai/RobotLineupAi.cs` (eine Methode je Routine, `[ORIGINAL] 12C8:xxxx`), Adressen in
`src/Traders.Core/Ai/RobotLineupLayout.cs`. `43B2` und `48F4` dort bzw. privat, bis Spur `kauf` (R10PL-5) und Spur `feld` gemergt
sind; Roboterfelder über die Namen in `DgroupLayout`, `37C3`/`3786`/`302E` über `ControlReader`. Keine `[APPROX]`.

## Prüfung
- `tests/Traders.Tests/Ai/RobotLineupAiDifferentialTests.cs`: alle 13 Einsprünge einzeln auf erzeugten Zuständen (Spieler 0–4,
  Grobraster, Markierungen, Ringtabellen, Zeiger am Start, am Quadrantenende und außerhalb, Verzögerungen 0/−1/nur High-Wort,
  Budgetgrenzen von `030B` um L und e/16, Flag von `02BB` als Byte), dazu ein Ablauf `000B` → `003B` je Computer → 400 Bilder
  `0539` mit Zeigerbewegung wie `0E9E`. Ganzes DS bis `DgroupLayout.Size` und `DS:10D2` gleich, AX bei `024E`/`02EA`.
- `tests/Traders.Tests/Ai/RobotLineupAiGameStateDifferentialTests.cs` (Mehrrundenprobe über Orakelzustände): Standardpartie im
  RefEmu, Roboterkampf eingeschaltet, Zyklus 2/1 abwechselnd, Wochen 1–4 mit Aufstellung; Zustände am Einsprung von `000B`,
  `003B`, `005A`, `030B`, `06D6`, `0539`, `0621`, `08EA` aufgezeichnet und gegen den Port nachgespielt; zusätzlich je Zustand ein
  Computer als Mensch (`0840` = 0).
- Gegenprobe (lokal, nicht eingecheckt): Abbruch bei `> 6` statt `>= 6`, Bit 2 statt 1 in `0621`, `<=` in `08EA`, Abstand 3
  statt 4 in `015C`, Blockmitte `x − 1` → jeweils rot.
