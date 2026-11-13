# 11A3:0000 – Wochenplanung der Computerspieler (Sammelbefund mit 11A3:007D, 11A3:0180, 11A3:043F, 11A3:0599, 11A3:061A, 11A3:067D, 11A3:078A, 11A3:0AA5, 11A3:0B3B, 11A3:0D35, 11A3:0E01, 11A3:0EB8, 11A3:0F2E, 11A3:104B)

- **Ladebild-Adresse:** `11A3:0000` (Dateioffset `0x15030`, Segment `0x1250` Byte); die letzte Routine `104B` läuft über die
  Segmentgrenze bis `12C8:000A` (`retf`).
- **Aufgabe:** plant für jeden Computerspieler die Woche ohne Bildschirm: Hauptart der Produktion (`40F6`), Zielspieler
  (`4580`), Sollzahl der Felder je Art; Durchgang 0 legt daraus den Anbauplan im Raster `48F4` und die Pflanzliste an (gelesen
  von der Anbau-KI `135D`), Durchgang 1 den Warenbedarf und die Preisgrenzen für Handel und Einkauf (`141D`, `12C8`, `08CF`).
- **Aufgerufen von / Phase:** nur Wochenschleife `1777:01F8`: Aufruf 6 bei `1777:0227` mit Argument 0 (vor dem Anbau), Aufruf 21
  bei `1777:0291` mit Argument 1 (vor dem Handel); Befund [1777_01F8_wochenschleife.md](1777_01F8_wochenschleife.md). Das
  Argument kommt als `mov al,n / push ax` (AH beliebig), gelesen wird nur das Byte `[bp+6]`.
- **Parameter / Rückgabe:** Byte Durchgang (0 / ≠ 0); keine Rückgabe (AL beliebig, die Schleife wertet es nicht aus).

## Hülle (`tools/callgraph.py`, transitiv)

15 Routinen in `11A3` – `0000`, `007D`, `0180`, `043F`, `0599`, `061A`, `067D`, `078A`, `0AA5`, `0B3B`, `0D35`, `0E01`,
`0EB8`, `0F2E`, `104B` – und außerhalb nur bereits Portiertes: `21FD:01BA` random (`OriginalRandom`), `22A3:0750` Anteile
(`Production.Shares`), Laufzeit `0000:4F89` LDIV und `0000:4F4E` LXMUL (`LongMath`). Nichts gehört einer anderen Spur.

## Ablauf `0000` (p = `37AA`, global)
```
007D; 0180
für p = 0..3: Computer → 043F; 0599
061A; 067D
für p = 0..3: Computer → 078A; Durchgang 0 → 0B3B; 0F2E   sonst → 104B
```
`37AA` ist danach 4; `45D0`, `2EB0`, `2F00` bleiben als Schleifenreste der letzten Unterroutine stehen. Nur `0180` zieht Zufall.

## Unterroutinen (alle 16-Bit-Arithmetik `cwd`/`idiv`; F = Felder von p mit Art ≠ 4, Raster y außen, x innen über `2F00`/`2EB0`)

| Routine | Liest | Schreibt | Inhalt |
|---|---|---|---|
| `007D` | `3CFA` (Vorrat Ware 3, Stride 8) | `2C74[4]`, `2C7C`, `37AA` | `2C74[p]` = 3 − Zahl der anderen mit Vorrat ≤ eigenem (Gleichstand senkt beide); mehr als ein Spieler mit 0 → `2C7C` = 0, sonst Vorrat(Rang 0) − Vorrat(letzter mit Rang 1) |
| `0180` | `0840`, `450E`, `2F1A`, `44F4`, `4442`/`2EB1`/`45DA`, `4BBE`, `2F05`, `2C74`, `3D42`, `4580` | `4580`, `37AA`, `2EB0`/`2F00`; **Zufall** | lokal: Merker[p] = Station ≠ −1 ∧ besucht ∧ Stationsart ≠ 0; Zahl[p] = F mit Güte ≠ 3. Kandidat = Computer mit `4BBE` = 1, `2F05` > 8, Merker, größte Zahl (erster gewinnt). Keiner und `2F05` ≥ 16: Merkerflag = (≥ 2 Computer) ∧ (kein Computer mit `2C74` = 0); ist `4580` = −1: `random(5)`, bei 0 oder Merkerflag Auswahl wie oben, aber mit `3D42` > 0 statt `4BBE`/`2F05`; sonst `random(5)`, bei ≠ 0 oder Merkerflag bleibt `4580`, wenn das **Byte Merker[4]** (= Low-Byte von Zahl[0], Lesen hinter dem lokalen Feld) ≠ 0, sonst −1 |
| `043F` | Raster, `0902` | `2C4C[p][0..2]` (W, Stride 8), `45D0` | Güteanteil je Art in %: Gewicht je Feld 0/2/5 nach `0902[k·4+Güte]` = 0/1/2; Summe 0 → 0. Andere Tabellenwerte ließen CL vom Vorfeld stehen (Tabelle hat nur 0–2) |
| `0599` | `2C4C`, `40F6` | `40F6[p]`, `45D0` | Hauptart = Art mit größtem Güteanteil, Start mit dem **Byte** `2C4C[p][40F6[p]]` (vorzeichenerweitert), echter `>` |
| `061A` | `0840`, `342C` | `40F6` (Menschen), `37AA`, über `22A3:0750` `342C` | Mensch: −1, dann erste Art mit Anteil ≥ 70 % |
| `067D` | `40F6`, `0840`, `2C4C` | `40F6`, `45D0`, `37AA` | Art ohne Spieler: erster Computer (p aufsteigend) mit Güteanteil ≥ 33 % für sie, dessen Hauptart ≥ 2 Spieler hat, wechselt |
| `078A` | Raster, `40F6`, `08E4`, `0840`, `4580`, `2C74`, `2C7C`, `4BC2`, `3CFA`, `2EE7`, `4BAC`, `2F26` | `2C44[4]`, `2C6C[4]`, `2C4A`, `2C72`, `303E[4]`, `45D0` | Sollfelder: a = Hauptart (Byte, vorzeichenerweitert), b = `08E4[a]`; n = F, g = F mit Güte ≠ 3; Raketensoll s (Zielspieler nach `2C74`/`2C7C`: g, n/2+1, n/3+1, n/4+1; sonst n/3 bzw. n/4 nach `4BC2`, +1 bei Ware-3-Vorrat < 200, mindestens 2 bei < 80) ≤ g; Zweitart (n − s)/5 nur ohne anderen Computer mit Hauptart b; Rest Hauptart → `2C44`. Dann mit g ≤ Raketenfelder + Raketen und Vorratsgrenzen aus `0AA5` → `2C6C`. `303E[k]` = Felder[p][k] − `2C44[k]` |
| `0AA5(k, abzug)` | `08E4`, `08E7`, `3CF4`, `2F26`, `48A2`, `2EE7` | – | min(Vorrat[`08E4[k]`] / `2F26`, (Vorrat[`08E7[k]`] − abzug) / `48A2` + Felder[k]) |
| `0B3B` | Raster, `40F6`, `08E4`, `2C6C` | `48F4` (Stride 26), `2EB0`/`2F00` | eigene Felder (auch HQ) → 100; dann je Stufe 0–11 und Ziel (Zweitart, Hauptart, 3 = Rakete) Felder mit Wert 100 nach `0D35`/`0E01`/`0EB8` belegen, bis das **Low-Byte** von `2C6C[Ziel]` aufgebraucht ist. Hauptart hier als Byte **ohne** Vorzeichen (`mov ah,0`) |
| `0D35(stufe, art)` | Raster, `0902` | – | (Güteklasse 1–4) · (1 gleiche Art, 2 leer, 3 sonst) = stufe |
| `0E01(stufe, art)` | wie `0D35` | – | andere Güteklassen, Vergleich mit stufe + 1 |
| `0EB8(stufe)` | Raster | – | Güte 3 → 0; stufe 0: Art 3, 1: leer, 2: jedes Feld |
| `0F2E` | Raster, `48F4` | `2F31`/`4654`/`472C` (B[4][54]), `48EC[p]`, `2EB0`/`2F00` | Pflanzliste: eigene Felder ≠ HQ mit Plan ≠ 100 und Plan ≠ Art: Plan, x, y |
| `104B` | `2C44`, `303E`, `08E4`/`08E7`, `2F26`, `48A2`, `3CFA…3D12`, `3044`, `2EE7`, `4898`, `3CD2` | `4560[p][0..3]`, `3046[p]`, `452A[p][0..3]`, `45D0` | Warenbedarf je Ware; Bedarf Ware 3 = Σ Ware-3-Vorräte / 8; `3046[p]` = max(0, −`3044`); Preisgrenze k = max(30, Low-Wort(10 · Bargeld / (16 Bit) (Preis[k] · max(10, 60 − Felder[k]/3·10)))); `452A[p][3]` = `4560[p][3]` |

## Zustand (neu belegt; Namen in `src/Traders.Core/Ai/WeekPlanningLayout.cs`, alle vorläufig außer den Formeln)
| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `2C74` | W[4] | s | Rang nach Vorrat Ware 3 (0 = höchster) | w `007D`, r `0180`, `078A`, `141D` |
| `2C7C` | W | s | Vorsprung des Ersten in Ware 3 | w `007D`, r `078A` |
| `4580` | B | s | Zielspieler der Computer, −1 = keiner | r/w `0180`; r `078A`, `135D`, `141D` |
| `2C4C` | W[4][3] | s | Güteanteil je Art in % (Stride 8) | w `043F` |
| `40F6` | W[4] | s | Hauptart je Spieler (0–2, Mensch −1 möglich) | r/w |
| `2C44…2C4A` | W[4] | s | Sollfelder je Art, [3] = Raketen | w `078A`, r `104B` |
| `2C6C…2C72` | W[4] | s | Sollfelder nach Vorratsgrenzen, [3] = Raketen | w `078A`, r `0B3B` (Low-Byte) |
| `303E…3044` | W[4] | s | eigene Felder − Soll je Art | w `078A`, r `104B` |
| `48F4` | B[9][26] | u | Anbauplan (100 = frei), Raster der Produktion | w `0B3B`, r `0F2E`, `135D` |
| `2F31`, `4654`, `472C` | B[4][54] | u | Pflanzliste Plan / x / y | w `0F2E`, r `135D` |
| `48EC` | W[4] | s | Länge der Pflanzliste | w `0F2E`, r `135D` |
| `4560` | W[4][4] | s | Warenbedarf (Stride 8) | w `104B`, r `141D` |
| `452A` | W[4][4] | s | Preisgrenze je Ware (Stride 8), [3] = Bedarf Ware 3 | w `104B`, r `141D`, `12C8` |
| `3046` | W[4] | s | Raketenfehlbetrag | w `104B`, r `08CF` |

## Abhängigkeiten (Stufe, Woche, Zufall)
- **Stufe:** `11A3` liest weder `08EA` noch `2E8E` (Suche in `11A3.asm` ohne Treffer); die Stufe wirkt nur über den Spielzustand.
- **Woche:** kein Zugriff auf `4B9A`; Woche 0 unterscheidet sich nur durch den Zustand (`2F05` ≥ 16 und Station).
- **Zufall:** höchstens ein `random(5)` je Aufruf, in `0180` nach der Kandidatensuche; nur wenn kein Kandidat und `2F05` ≥ 16.
- Keine Uhr, keine Ausgabe, kein Ton, keine Eingabe.

## Randfälle
- Divisionen durch `2F26`/`48A2` (≥ 3 im Spiel) und durch `Preis · di` (Preise ≥ 3) – Null würde im Original fehlschlagen,
  der Port wirft dann ebenso (`DivideByZeroException`).
- Hauptart −1 bei einem Computer kommt im Spiel nicht vor (`0E31` setzt 0,2,1,0, nur Menschen bekommen −1); der Port indiziert
  dann wie das Original (`078A` vorzeichenbehaftet, `0B3B` ohne Vorzeichen).

## C#-Umsetzung
`src/Traders.Core/Ai/WeekPlanning.cs` (je Routine ein `[ORIGINAL]`-Marker), Schritt-Handler `WeekPlanningStep` für die Aufrufe
6 und 21 (Einbindung in die Phasenmaschine: Spur `anbau` Teil 3).

## Prüfung
`tests/Traders.Tests/Ai/WeekPlanningDifferentialTests.cs`: `Call(11A3:0000)` mit Argument 0 und 1 auf Zuständen an den Aufrufen
6 und 21 der Wochen 0–3 einer echten Partie, dazu Mensch/Computer gemischt, alle Stufen (`2E8E`), Ziel `4580` gesetzt/frei;
verglichen werden ganzes Datensegment und `DS:10D2`.
