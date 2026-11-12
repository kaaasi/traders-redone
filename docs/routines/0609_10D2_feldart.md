# 0609:10D2 / 0609:10A7 / 0609:1072 – Feldart setzen, Feld im Raster, Feld des Spielers am Zug

- **Ladebild-Adresse:** Segment `0609` (Dateioffset `0x9690`); `10D2` bei `0xA762`, `10A7` bei `0xA737`, `1072` bei `0xA702`.
- **Aufgabe:** gemeinsame Spielfeldschicht: Besitzer und Art eines Feldes setzen und die Zähler nachführen; prüfen, ob eine
  Koordinate im Raster liegt und ob das Feld dem Spieler am Zug gehört.
- **Aufgerufen von / Phase:** `10D2` aus `0609` (5 Stellen: Anbau, HQ, Kauf), `111D`, `17CB`, `1B03`, `1F63`; `10A7` aus `0609`
  und `135D` (Computer im Anbau); `1072` nur aus `0609`.
- **Parameter / Rückgabe:** `10D2(x, y, Besitzer, Art)` (Wörter), keine Rückgabe. `10A7(x, y)`, `1072(x, y)`: AL = 1/0
  (AH unbestimmt). Index immer x·6 + y (16-Bit-`imul`, keine Bereichsprüfung).

## Ablauf (eigene Worte)

- **`10A7`:** 1, wenn 0 ≤ x ≤ Spalten − 1 und 0 ≤ y ≤ Zeilen − 1 (alles vorzeichenbehaftete Wörter), sonst 0.
- **`1072`:** `10A7`; nur wenn 1, Besitzerbyte des Feldes mit `DS:37AA` vergleichen (Byte, auch −1 = −1 ergibt 1).
- **`10D2`:** alten Besitzer s und alte Art k als Bytes mit Vorzeichen lesen. s = −1 → freie Felder `DS:2F05` − 1; sonst
  `DS:4BA8+s` − 1 und, wenn 0 ≤ k ≤ 3, `DS:2EE7+4s+k` − 1. Danach dasselbe mit + 1 für den neuen Besitzer und die neue Art
  (Wörter, Vergleich vorzeichenbehaftet). Zuletzt Besitzer und Art als Bytes ins Feld. Art 4 (HQ) und −2 (leer) zählen nur in
  der Feldzahl. Andere Besitzerwerte (−2, 4 …) treffen ohne Prüfung die Bytes neben den Tabellen (16-Bit-Adressüberlauf);
  so übernommen. `10D2` ruft nichts.

## Zustand

| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `4442` | B[9·6] | s | Besitzer je Feld, −1 = niemand | L/G `10D2`, L `1072` |
| `2EB1` | B[9·6] | s | Art je Feld (−2 leer, 0–2 Anbau, 3 Rakete, 4 HQ) | L/G `10D2` |
| `2F05` | B | u | freie Felder | L/G `10D2` |
| `4BA8` | B[4] | u | Felder je Spieler | L/G `10D2` |
| `2EE7` | B[4][4] | u | Felder je Spieler und Art 0–3 | L/G `10D2` |
| `08FC` / `08FE` | W | s | Spalten / Zeilen | L `10A7` |
| `37AA` | B | s | Spieler am Zug | L `1072` |

## Seiteneffekte in Reihenfolge

Keine: kein Zufall, keine Uhr, keine Grafik, keine Eingabe.

## C#-Umsetzung

`src/Traders.Core/Field/FieldGrid.cs`: `Set` = `10D2`, `Inside` = `10A7`, `OwnedByCurrent` = `1072`; Adressen mit 16-Bit-Überlauf
wie im Original.

## Prüfung

`tests/Traders.Tests/Field/FieldGridDifferentialTests.cs` (RefEmu `LoadOnly`, direkte Aufrufe, ganzes DS und AL): `10A7`/`1072`
400 Fälle mit Koordinaten −32 768…300 und Spalten/Zeilen auch ≤ 0; `10D2` 600 Fälle mit Besitzer −2…7, Art −3…255, alten Werten
auch außerhalb der Tabellen. Gegenproben: 4 Mutationen, 3 rot, eine im gültigen Raster gleichwertig (Indexüberlauf).
