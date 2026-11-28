# 150F:1B56 – Kampfstufe des Spielers am Zug setzen

- **Ladebild-Adresse:** Segment `150F` (Dateioffset `0x186F0`); `1B56` bei `0x1A246`.
- **Aufgabe:** setzt das Byte `BattleLevel[p]` (`DS:40E7 + p`, Name vermutet) des Spielers am Zug `p = DS:37AA`: 0, solange das
  Wort `Robot376C[p·4]` höchstens 5 ist (Vergleich **ohne** Vorzeichen, `ja`), sonst ein gewichtetes Mittel der Teilebytes 2–5.
- **Aufgerufen von / Phase:** laut `tools/callgraph.py` `16D6:0067` (Kauf-KI, liest danach `40E7[p]`, R10) sowie `0EDE:051C` und
  `0EDE:16D2` (Kampf, R11). Kein Aufrufer liest `AX` (Aufrufstellen `16D6:0151`, `0EDE:053B`, `0EDE:1801` geprüft).
- **Parameter / Rückgabe:** keine; `AX` danach unbestimmt (für Aufrufer ohne Bedeutung).

## Ablauf (eigene Worte)

- `p` = `cbw(DS:37AA)`. Ist `U16[376C + 4p] ≤ 5`: `40E7[p] = 0`, Ende.
- Sonst mit `T(k) = RobotParts[p·6 + k] / 10` (Byte ohne Vorzeichen, `idiv`): `s = T(2)·4 + T(3) + T(4) + T(5)` (16 Bit),
  `40E7[p]` = unteres Byte von `s / 7` (`idiv`). Höchstwert 25 (alle Teile 255).
- Indizes 16 Bit (`p·4` per `shl`, `p·6` aus `imul`).

## Zustand

| DS-Offset | Breite | Vorzeichen | Bedeutung (vermutet) | gelesen/geschrieben |
|---|---|---|---|---|
| `37AA` | B | s | Spieler am Zug `p` | L |
| `376C` | W [p·4] | u | `Robot376C` (Feld 19), Schwelle > 5 | L |
| `43D0`–`43D3` | B [p·6] | u | `RobotParts` Bytes 2–5 | L |
| `40E7` | B[4] | – | `BattleLevel` (indiziertes Schreibziel, im Aufrufgraph nicht gezählt) | S |

## Seiteneffekte in Reihenfolge

Nur der Byteschreibzugriff `40E7[p]`; kein Zufall, keine Grafik, keine Eingabe, kein Ton.

## C#-Umsetzung

`src/Traders.Core/Robots/Shared/RobotHelpers.cs`: `UpdateBattleLevel(Dgroup)`, `public static`, ohne Bild.

## Prüfung

`tests/Traders.Tests/Robots/Shared/RobotHelpersDifferentialTests.cs`: ganzes DS und `DS:10D2` gleich; Schwelle 0, 5, 6, `0x7FFF`,
`0x8000`, `0xFFFF` und zufällig, Spieler 0–4 und negativ, `40E7` vorher ≠ 0 (zeigt das Nullsetzen), alle Teile 255 (Stufe 25,
Erwartung von Hand gerechnet). Gegenprobe (lokal ausgeführt): Schwelle mit Vorzeichen → rot; Gewicht 2 statt 4 → rot.
