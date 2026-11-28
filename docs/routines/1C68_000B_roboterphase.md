# 1C68:000B / 1C68:006B / 1C68:00A5 / 1C68:00FC / 1C68:01A8 / 1C68:02ED / 1C68:03C8 / 1C68:0641 / 0EDE:0322 / 0EDE:1826 – Roboterphase: Steuerung und Roboterkauf

- **Ladebild-Adresse:** Segment `1C68` (Dateioffset `0x1FC80`, Größe `0x1870`); `0EDE:0322`/`1826` im Segment `0EDE`.
- **Aufgabe:** Aufruf 18 der Wochenschleife. `000B` prüft die Bedingung, ruft den Kauf `00A5`, fragt mit `006B`, ob jemand
  kämpft, und ruft dann Feld, Kampfvorbereitung, Aufstellung, Kampf und Nachlauf. `00A5` ist der ganze Kaufbildschirm mit
  Mensch- und Computerkauf.
- **Aufgerufen von / Phase:** `000B` aus der Wochenschleife (Aufruf 18); alle anderen nur aus `000B` abwärts. `0EDE:0322`/`1826`
  ruft in R11 auch der Kampf.
- **Parameter / Rückgabe:** `006B` und `0EDE:1826` liefern AL (0/1); `0EDE:0322` hat keine Rückgabe (AL = Spielernummer).
- **Hülle** (`tools/callgraph.py` auf `T.EXE`, transitive Hülle ab `00A5`/`006B` in `1C68` plus `0EDE:0322`/`1826`): 43
  Routinen ≈ 6 400 Byte. Davon gehören `0A28`/`0AB0`/`0B02` (Preise, Teileart) und die Kauf-KI `16D6` der Spur `kauf-ki`; nach
  außen gehen nur schon portierte Routinen (`0825:0813`, `0A96`, `0E9E:000E`, `186C:0002`, `21A7:0054`, `21AF`, `21FD`, `23CD`,
  Laufzeit `0000:4F4E`/`4F89`).
- **Indizierte Schreibziele** (R8PL-3, aus dem Disassembly, vom Aufrufgraphen nicht gezählt): `[bx+…]` auf `45D2`, `4BF4`/`4BF6`,
  `3CB7`, `44C2`, `2F01`, `3CBC`/`3CBE`, `443E`, `3320`/`3322`, `330E`, `3786`/`3788`, `302E`/`3030`, `43B2`, `48A4`…`48B2`,
  Felder 18–25 (`43CE`/`43CF`, `376C`, `376E`, `2EF8`, `377C`, `40EC`, `33DC`, `3016`). Direkt: `37AA`, `547A`, `0E72`, `0F0A`,
  `5470`–`5478`. Bedeutungen: `docs/analyse/DATENLAYOUT.md`, Abschnitt „Runde 10, Spur `kauf`“.

## Ablauf (eigene Worte)

- **`000B`** (Spur `roboter`): `411A` = 0 → Ende; sonst `4B9A` mod `08EC` mit Vorzeichen (`cwd`/`idiv`) ≠ 0 → Ende. Dann `00A5`,
  `006B`; AL = 0 → Ende. Sonst `21FD:037D(47h)`, `1E7E:0004`, `150F:012E`, `12C8:000B`, `0535:078A`, `0535:08ED`, `0535:000A`,
  `0EDE:0009`, `0535:06CE`.
- **`006B`:** Merker = 0; `37AA` = 0…3: `0EDE:0322`, bei `45D2[p]` = 1 Merker = 1. AL = Merker, `37AA` bleibt 4.
- **`0EDE:0322`:** `45D2[p]` = 0, wenn `1826` = 1; sonst 1, wenn das Wort `376C + 4p` ≠ 0 (nur das untere Wort), sonst 0.
- **`0EDE:1826`:** si = 0…5: Byte `43CE + 6p + si` ≥ 10 (vorzeichenlos, `jc`) → AL = 0; alle kleiner → AL = 1.
- **`00A5`** (Kauf): Bild `186C:0002([08DE])`, `177F` (Zeigerobjekte: Höhe `330E` = 0Bh, `21AF:022B`/`00CA`), `01A8`,
  `21FD:037D(553h)`, `0B75` (Laden zeichnen), Kauf-KI `16D6:000E`, `17FE` (`21AF:0412` je Spieler), Palette
  `21FD:026C(DS:0E4C)`, `21FD:0329`, `0A96:001B`, `00FC`, `21AF:0472`, `03C8`.
- **`00FC`** (Bildschleife, endet bei `547A` = 4): `21FD:051F`, `23CD:071C([082E])`, `0A96:06A8`, `0A96:001B`; `21FD:0245`, Kauf-KI
  `16D6:06F6` (Steuerbits der Computer), `0E9E:000E`; je Spieler nach `43B2`: 0Bh → `04A9` und `0641`, 0Ch → `0681`; dann `1824`
  (Laufgeräusch), `14B9` (Animationen), `0B51` (Tafel von Spieler `0E72`), `0A96:06A8`, `23CD:06E9([082E])`; `0E72` + 1, über 3 →
  0. Nach der Schleife `21FD:0665`.
- **`01A8`:** `547A` = 0; je Spieler (`37AA`): Geld = `0825:0813` / 10 (LDIV), mindestens 500, höchstens 2000 (32 Bit mit
  Vorzeichen); Computer (`0840` = 1, nicht ≠ 0) + (Geld / 100) · (`2E8E`·12, `imul` und `cwd` des unteren Worts) (LXMUL);
  `4BF4[p]`; `3CB7`, `44C2`, `2F01` = FFh; `3320` = 6p + 8Ch, `3322` = p + 6Eh; `443E` = 0; `3786` = 0 (Dword); `43B2` = 0Bh;
  `02ED`.
- **`02ED`:** `48A4 + 16p` = 0, `48A6`…`48B2` = Felder 19–25 (`377C` vorzeichenlos zum Wort).
- **`03C8`:** je Spieler (`37AA` = 0…3, bleibt 4) Felder 19–25 = `48A6`…`48B2` (`377C` nur das untere Byte); Posten 0 verfällt.
- **`04A9`** (Laufen): rechts (`44BE`) → `443E` = 2, sonst links (`43F0`) → −2; hoch (`439D`) → `0515`; danach beim Menschen
  `3786` = 1000.
- **`0515`** (Theke betreten): `443E` = 0, Ton (0, 32h); si = 0…4: x des Zeigers in `[08B2 + 4si, 08B4 + 4si]` → `43B2` = 0Ch,
  `3CB7` = `0E85[si]`, `44C2` = `0E8A[si]`, `3CBC` = `0A28(44C2)` (mit `cwd`); Theke 1 zeichnet Rahmen und Typ (`0D8C`, `0DEA`,
  `1422(0)`), Theke 0 (Ausgang) zählt `547A` hoch.
- **`0641`:** x = `3320` + `443E` (16 Bit, Schritt mit Vorzeichen); nur bei 0 ≤ x < 12Dh gespeichert.
- **`0681`** (an der Theke): runter (`44BA`) → verlassen (Theke 0: `547A` − 1; `44C2`, `3CB7` = FFh, `43B2` = 0Bh, Tafel `1061`,
  Ton 33h); links (`43F0`) → Posten − 1 (unter `0898[Theke]` auf `089D[Theke]`), rechts (`44BE`) → + 1 (über `089D` auf `0898`),
  je `3CBC` neu, Ton 34h/35h, `3786` = 3; sonst Feuer (`4894`) oder hoch (`439D`) → `0812`.
- **`0812`** (kaufen): Posten k < 8 und `48A4 + 16p + 2k` ≥ `08A2[k]` (vorzeichenlos) → nichts; Geld − Preis < 0 → nichts; sonst
  `4BF4` −= Preis, `0EF6` (Geld zeigen); k = 0 → nur Ton und Verzögerung (kein Vorrat); k = 1…7 → Vorrat + 1 und `0F41`; k = 8…11 → `2F01` = k − 8, `0FE9`, Preis neu; k ≥ 12 →
  `099D(k − 12)`, Preis neu; Ton (1, 36h); beim Menschen `302E` = 10000 (k ≥ 8) bzw. 7 und `3786` = 10000 bzw. 3.
- **`099D(k)`:** Paar si = k / 7, Wert = (k mod 7)·10 + 9 in `43CE + 6p + 2si` und `+1`; Bild `0E87` und `1372(si)`.
- **Zeichnen** (alle über `0A96`, Teil 3): `0B75` Laden und je Spieler Tafel (`0E67`, `0EF6`, `0D8C`, `0DEA`, `1372`), `0D8C`/`0DEA`
  Rahmen und Typbild, `0E67`/`0E87` Teile (mit `0B02`), `0EF6` Geld (`0A96:05FB`), `0F41` Vorrat (`0A96:057A`), `0FA3`/`0FE9` Texte
  (`0A96:0525`), `1061`/`10A3` Tafel löschen, `0B51` → `10A3` (kein Posten) oder `10DC` (Posten mit Bild, Preis `129F`), `1372`/`1422`
  Teilebalken, `14B9` mit `14D7`/`152E`/`15BB`/`1617`/`16F4` Ladenanimationen über `5470`–`5478`.
- **`1824`:** läuft ein Zeiger (`443E[0…3]` ≠ 0) und `0F0A` = 0 → Ton (2, 37h), `0F0A` = 1; steht alles und `0F0A` = 1 → Ton (2, 0) und
  `0F0A` = 0. Die Routine reicht über das Segmentende von `1C68` hinaus (endet bei `0x187B`).

## Zustand

| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `37AA` | B | s | Spielerschleife, bleibt nach `006B`, `01A8`, `03C8` auf 4 | L/S |
| `43CE + 6p` | B[6] | u | Teile (Paare, 10·Stufe + 9; < 10 nicht gekauft) | L `1826`; S `099D` |
| `376C + 4p` | W | u | Posten 1 (Feld 19); ≠ 0 nötig zum Kämpfen | L `0322`, `02ED`; S `03C8` |
| `376E`, `2EF8`, `377C`, `40EC`, `33DC`, `3016` | W/B | u | Posten 2–7 (Felder 20–25) | L `02ED`; S `03C8` |
| `45D2` | B[4] | u | kämpft | S `0322` |
| `4BF4` | D[4] | s | Kaufgeld | S `01A8`, `0812` |
| `0840`, `2E8E` | B[4], W | u, s | Computer, IQ | L `01A8` |
| `48A4 + 16p` | W[8] | u | Einkaufskopie der Posten 0–7 | S `02ED`, `0812`; L `03C8` |
| `3CB7`, `44C2`, `2F01`, `443E`, `43B2`, `3320`/`3322`, `3786` | s. DATENLAYOUT | | Kaufzustand je Spieler | S `01A8` u. a. |
| `547A` | W | s | Spieler fertig | S `01A8`, `0515`, `0681`; L `00FC` |

## Seiteneffekte in Reihenfolge

`006B`, `0322`, `1826`, `01A8`, `02ED`, `03C8`, `0641`: keine (kein Zufall, keine Uhr, keine Ausgabe). Im Kauf zieht nur die Kauf-KI
(`16D6`, eine Aufrufstelle von `random`); Ton nur über `21A7:0054` (`0515`, `0681`, `0812`, `1824`), Eingabe über `0E9E:000E`.

## C#-Umsetzung

`src/Traders.Core/Robots/Purchase/RobotPurchaseRules.cs` (`Setup` = `01A8` mit `SaveStock` = `02ED`, `Commit` = `03C8`,
`MovePointer` = `0641`), `RobotReadiness.cs` (`AnyoneFights` = `006B`, `UpdateFights` = `0EDE:0322`, `HasNoParts` = `0EDE:1826`).
Teil 2 (Kaufablauf `00A5`/`00FC` als Kern-Ereignis je Bild mit `04A9`, `0515`, `0681`, `0812`, `099D`, `1824`) folgt nach dem Merge
der Spur `kauf-ki`; Teil 3 (Szene, alle Zeichenroutinen als Befehlsliste) danach.

## Prüfung

`tests/Traders.Tests/Robots/Purchase/RobotPurchaseRulesDifferentialTests.cs` (`RulesHarness`: Original über `LoadOnly`/`Call` auf
vorbereitetem DS, ganzes DS einschließlich `DS:10D2` gleich, keine Ziehung, kein Hostereignis; AL getrennt verglichen):
viele Zustände je Routine, dazu Grenzfälle – Vermögen 4999/5000/20009/20010, negativ, `int.MinValue`/`MaxValue`; IQ 0…3, 0AABh
(Produkt 8004h mit Vorzeichen) und −1; `0840` = 0/1/2; Zeiger x 0/1/2, 12Ah–12Dh, −1, −3, 7FFFh mit Schritten ±2, −128, 127; Teile
9/10/FFh je Paar, `376C` = 0 bei gesetztem `376E`, 8000h. Gegenproben (lokal, je einzeln rot): `≥ 10` als `> 10`, Merker in `006B`
vergessen, Computer `≠ 0` statt `= 1`, IQ·12 ohne 16-Bit-Schnitt, Obergrenze 2000 fehlt, `≤ 12Dh`, `377C` als Wort zurück.
