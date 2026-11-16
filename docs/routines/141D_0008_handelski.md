# 141D:0008 – Handels-KI der Computerspieler (Sammelbefund mit 141D:0029, 141D:008C, 141D:021A, 141D:0397, 141D:0464, 141D:04A5, 141D:05A3, 141D:0806, 141D:09F3, 141D:0A1D, 141D:0BFD, 141D:0CCA, 141D:0E09, 141D:0E67, 141D:0ED7, 141D:0F01)

- **Ladebild-Adresse:** `141D:0008` (Dateioffset `0x177D8`, Segment `141D` ab `0x177D0`, Größe `0xF20`); die letzte Routine
  `0F01` läuft über die Segmentgrenze bis `150F:000A` (`retf`).
- **Aufgabe:** Gebote und Bewegungen der Computerspieler im Handel `0B9F`/`0C29` je Ware: Budget je Ware, Bedarf und
  Überschuss, Seitenwahl Käufer/Verkäufer, Gebotsstufen und Stückzahlen der Käufer, nach Rang sortierte Gebotsliste für die
  Verkäufer, dann je Bild die Joystick-Aktion `DS:3308[p]` (2 hoch, 3 runter, 10 Feuer, −1 nichts), die der Handel wie eine
  Eingabe auswertet; Buchführung nach jedem Abschluss.
- **Aufgerufen von / Phase:** Handel (Aufruf 23 der Wochenschleife): `0B9F:0002` → `0008` (einmal je Handel), `0B9F:00FE` →
  `0397` (je Bild für jeden Computer), `0C29:0046` → `0464` (Beginn jeder Ware), `0C29:0086` → `021A` (je Spieler),
  `0C29:01B7` → `09F3` (je Bild für jeden Computer), `0C29:03B2` → `0E67` und `0CCA` (je Abschluss über zehn Einheiten).
- **Parameter / Rückgabe:** nur `0CCA` hat ein Argument (Wortpreis `[bp+6]`); keine Routine gibt etwas zurück (AX beliebig).
  Spieler ist immer das globale `DS:37AA`, Ware `DS:45D0`, Käufer `DS:4106` (10 = Laden), Verkäufer `DS:44F2`.

## Hülle (`tools/callgraph.py`, transitiv)

17 Routinen in `141D` – `0008`, `0029`, `008C`, `021A`, `0397`, `0464`, `04A5`, `05A3`, `0806`, `09F3`, `0A1D`, `0BFD`,
`0CCA`, `0E09`, `0E67`, `0ED7`, `0F01` – und außerhalb nur bereits Portiertes: `21FD:01BA` random (`OriginalRandom`),
Laufzeit `0000:4F89` LDIV und `0000:4F4E` LXMUL (`LongMath`). Keine Grafik, kein Ton, keine Eingabe, kein Text. Nichts
gehört einer anderen Spur. Einsprünge von außen genau die sieben oben; die übrigen zehn nur aus `141D`.

## Vertrag für Spur 1 (`Traders.Core.Ai.TradeAi`, öffentlich, arbeitet auf `Dgroup` wie das Original)

| Einsprung | Methode | Zufall | Anmerkung |
|---|---|---|---|
| `0008` | `Budgets(Dgroup)` | – | hinterlässt `37AA` = 4, `45D0` = 4 |
| `021A` | `Demand(Dgroup)` | – | Spieler `37AA`, Ware `45D0` |
| `0397` | `short ChooseSide(Dgroup, OriginalRandom, short staleLocal)` | 2× `random(3)` (nur ein Zweig) | gibt das Endwort der lokalen Variable zurück |
| `0464` | `PlanBids(Dgroup)` | – | hinterlässt `37AA` = 4 |
| `09F3` | `Act(Dgroup, OriginalRandom)` | höchstens 1× `random(400)` | Spieler `37AA` |
| `0CCA` | `Bought(Dgroup, short price)` | – | Käufer `4106` |
| `0E67` | `Sold(Dgroup)` | – | Verkäufer `44F2` |

Nur die beiden ziehenden Einsprünge bekommen den `OriginalRandom` des Aufrufers; der Zustand `DS:10D2` liegt dort. Adressen
werden wie im Original 16-bittig gebildet (`[bx+disp]`), auch für Laden 10 als Käufer oder Verkäufer.

### Uninitialisierte lokale Variable in `0397`

`0397` legt `[bp-2]` an (`dec sp; dec sp`), setzt sie aber im Zweig „Countdown > 4, beide `random(3)` ≠ 0“ nicht und schreibt
das untere Byte des vorgefundenen Stapelworts nach `3308[p]`. Das Wort liegt bei `SP−8` relativ zum Stapel von `0B9F:00FE`;
dort hinterlassen Spuren: das Endwort des vorigen `0397`-Aufrufs (meist), die lokale Variable von `0E9E:037A`, das von
`0C29:0F68` gesicherte SI und die Rücksprungadresse von `21A7:0054` (`0154`/`01AF`). In den Wochen 0–3 der Vorgabepartie
gemessen (bei `141D:039C`): FFFF 19085×, 0001 65×, 0003 34×, 0002 13×, Einzelwerte 241D, 0046, FF02. Das Wort ist deshalb
Parameter; `ChooseSide` gibt das Endwort zurück, damit der Aufrufer den Stapelplatz nachführen kann. Wirkung nur bei unterem
Byte 2 oder 3.

## Ablauf (p = `37AA`, w = `45D0`, alle Zahlen 16 Bit vorzeichenbehaftet außer „long“)

```
0008: für p = 0..3: 0029; 008C
0029: Budget[p](long) = Bargeld[p] / (int)(150 − (10000 − Bargeld[p]) / 83)        // LDIV, Divisor 0 → Divisionsfehler
008C: für w = 0..3: 021A; G[w](long) = Budget[p] · (Bedarf[p]/10) + (int)((Reserve[p]/30) · Preis[w])
      S = Σ G; für w = 0..3: Warenbudget[p][w] = S == 0 ? 0 : Bargeld[p] · (int)(100·G[w]/S) / 100
021A: Computer: Bedarf[p] = Warenbedarf[p][w] − Vorrat[p][w]; Reserve[p] = Preisgrenze[p][w]          (11A3:104B)
      Mensch:   Schätzung = w == 3 ? (Σ Vorrat[q][3]) / 4 : Felder[p][(w+1) mod 3] · DS:2F26; Bedarf = Schätzung − Vorrat; Reserve = 30
      Überschuss[p] = −(Bedarf + Reserve); Bedarf < 0 → 0; Überschuss < 0 → 0, sonst Reserve = 0
0397: 2F18 ≠ 2F16 oder Countdown mod 4 ≠ 0 → −1
      sonst Countdown > 4 → random(3)==0 → 3; random(3)==0 → 2 (sonst bleibt das Stapelwort)
      sonst Überschuss ≥ 10 → 2 sonst 3; bei w == 3 zusätzlich 3, wenn (4BBE[p] == 1 und 2F05 ≥ 8) oder p == 4580
            oder Rang[p] ≤ 1 (unsigned) oder 2C74[p] == 0;  3308[p] = Byte
0464: für p = 0..3: 021A; Käufer (43B2[p] == 0) → 04A5; 05A3.   Dann 0E09; 0806; 0F01
04A5: b = unteres Wort Budget[p], s = clamp(b/10, 2, 20); Stufe[p] = (b+s, b, b−s, Preis[w]) als long; Stufen 0–2 ≥ Preis[w]
05A3: Rest = Warenbudget[p][w]; Einheiten[p][0..3] = 0; n = Bedarf, r = Reserve
      solange nicht alle Stufen 0–2 „voll“ und n > 0: für i = 0..2: Stufe[i] ≥ Rest → voll, sonst +10 Einheiten, n −= 10, Rest −= Stufe[i]
      n > 0 → r += n;  solange Stufe[3] ≤ Rest und r > 0: Einheiten[3] += 10, r −= 10, Rest −= Stufe[3]
      Stufe = erste mit Einheiten > 0 (4 = keine); Gebot[p] = Stufe[Stufe] bzw. 0 und Warenbudget[p][w] = 0
0806: Rang[j] der 16 Stufen (alle Spieler): für i < j: S[i] > S[j] → Rang[j]++; S[j] ≥ S[i] → Rang[i]++
      für r = 0..15, j = 0..15 mit Rang[j] == r: Käufer → Liste[r] = (Stufe, Einheiten), Verkäufer → (0, 0) (spätere überschreiben)
      2D74 = erster Eintrag mit Einheiten > 0 (16 = keiner); 0ED7
09F3: 43B2[p] == 0 → 0A1D; == 1 → 0BFD
0A1D: Ziel = 455A + 3D1C/2; eigene Lage = 455A + (43A2 ≥ 0 ? 43A2/2 : (43A2−1)/2); 3308 = −1
      43A2 < 3D1C: 2D76/(2BB8+1) > Bedarf → Ziel − 2;  sonst: 2D76/2BB8 > Bedarf+50 → Ziel − 2,
        sonst 2BC0 == 1 und Ziel > Höchstgebot 2DC8 → Ziel − 1
      Ziel ≥ Gebot, ≥ 455A; Deckel = Stufe == 3 ? Gebot : Gebot + 60, ≤ Warenbudget[p][w]; Ziel ≤ Deckel
      Ziel > Lage → 2, Ziel < Lage → 3
0BFD: Lage = 455A + 43A2/2; t = 2BBA − Countdown
      Überschuss < 10 oder t ≥ 12 → 10;  t ≥ 3 und (Überschuss ≥ 600 oder random(400) == 0) → 2D74++, 0ED7, 2BBA = Countdown, −1
      sonst Lage > 2D60 → 3, Lage < 2D60 → 2, sonst −1
0CCA: b = 4106; Mensch → Ende. Bedarf[b] −= 10 (≥ 0); Einheiten[b][Stufe] −= 10; Warenbudget[b][w] −= Preis
      Einheiten > 0 → Ende ohne 0E09; sonst nächste Stufe mit Einheiten (Stufe > 3 → Gebot = 0, Warenbudget = 0); 0E09
0E09: 2DC8 = größtes Gebot der Computer-Käufer (0 wenn keiner)
0E67: s = 44F2; Überschuss[s] −= 10 (≥ 0); 0F01; 2D74 > 15 → Ende; Liste[2D74].Einheiten −= 10; 2D74 weiter, solange
      Einheiten ≤ 0 und 2D74 ≤ 15 (liest bei 16 das Wort 2D3E); 0ED7
0ED7: 2D60 = 2D74 > 15 ? 1 : Liste[2D74].Gebot
0F01: 2D76 = Σ Überschuss der Verkäufer (43B2 == 1)
```

Vergleiche von long-Werten sind vorzeichenbehaftet 32-bittig (oberes Wort signed, unteres unsigned). `imul` mit folgendem
`cwd` (008C, 021A) nimmt nur das untere Produktwort.

## Zustand

Neue Adressen mit Beleg: [DATENLAYOUT.md](../analyse/DATENLAYOUT.md), Abschnitt „Spur handelski (R7)“; Namen in
`src/Traders.Core/Ai/TradeAiLayout.cs`. Geschrieben: `37AA`, `45D0` (Schleifen), `3308[p]`, `2D3E`, `2D78`, `2D64`, `2D80`,
`2D88`, `2C7E`, `2CFE`, `2D6C`, `2D50`, `2CBE`, `2D1E`, `2D74`, `2D60`/`2D62`, `2D76`, `2DC8`/`2DCA`, `2BBA`, `10D2`. Die
Adressen `2D60`/`2D62`, `2D74`/`2D76`, `2DC8`/`2DCA` des Plans sind die Langwörter 2D60, 2DC8 und die Wörter 2D74, 2D76.

## Seiteneffekte in Reihenfolge

Nur Zufall: `0397` zweimal `random(3)` (erst „→ 3“, dann „→ 2“), nur bei 2F18 = 2F16, Countdown mod 4 = 0 und Countdown > 4;
`0BFD` einmal `random(400)`, nur bei t ≥ 3 und Überschuss < 600. Kein Bild, kein Ton, keine Eingabe, keine Uhr.

## C#-Umsetzung

`src/Traders.Core/Ai/TradeAi.cs` (alle 17 Routinen, `[ORIGINAL]` je Routine), `TradeAiLayout.cs`.

## Prüfung

`tests/Traders.Tests/Ai/TradeAiDifferentialTests.cs` mit `TradeAiHarness` (Aufruf über `LoadOnly`/`Call`, Stapelwort von
`0397` bei `141D:039C` gesetzt) und `TradeAiGame` (Vorgabepartie im RefEmu, Wochen 0–3):

- Einzelaufrufe auf den Eintrittszuständen der Partie (alle sieben Einsprünge in jeder Woche, `0CCA` ab Woche 1; `021A`
  auch als innerer Aufruf), Datensegment und `DS:10D2` gleich.
- 2400 Variationen aller gelesenen Größen (Sitze, Seiten, Bargeld, Vorräte, Bedarf, Grenzen, Preise, Lage, Countdown,
  Abschlüsse, Gebotstabellen, Zufallszustand, Stapelwort); alle 65 markierten Pfade durchlaufen, `0397` und `09F3` ziehen in
  manchen Läufen und in anderen nicht.
- Gleichschritt an jeder Aufrufstelle aus `0B9F`/`0C29` (Teil 2, Mehrrundenprobe): jeder Aufruf der Wochen 0–3 auf seinem
  Eintrittszustand portiert und bei seiner Rückkehr mit dem Original verglichen (ganzes Datensegment außer den
  Interrupt-Bytes, `DS:10D2`), in der Vorgabepartie und mit Spieler 0 als Mensch während der Handel.
- Gegenprobe lokal (16.11.2026): zweite Ziehung in `0397` ausgelassen, `<` statt `<=` in `05A3`, Käufer und Verkäufer in
  `0CCA` vertauscht → jeweils rot.
