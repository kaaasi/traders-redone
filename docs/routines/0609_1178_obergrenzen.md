# 0609:1178 – Obergrenzen aus den Produktionsfeldern

- **Ladebild-Adresse:** `0609:1178` (Dateioffset `0xA808`)
- **Aufgabe:** zwei Obergrenzen der Woche aus der Zahl der Produktionsfelder F aller Spieler (Arten 0–2; Art 3 zählt nicht):
  `DS:48A2` = min(30, F/2 + 6), `DS:2F26` = min(30, F/2 + 3). Bedeutung der beiden Werte offen (BESTAND B3; gelesen in
  `0609`, `0B9F`, `11A3`).
- **Aufgerufen von / Phase:** Wochenschleife `1777:01F8` zweimal je Woche (Schritte 5 und 16, `BESTAND.md` §1.4).
- **Parameter / Rückgabe:** keine.

## Zustand
| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `2EE7` | B[4][4] | s | Felder je Spieler und Art (Schritt 4), gelesen Arten 0–2 | r |
| `37AA` | B | s | Spielerschleife (global), danach 4 | w |
| `45D0` | W | s | Artenschleife (global), danach 3 | w |
| `48A2` | B | s | F/2 + 6, höchstens 30 | w |
| `2F26` | B | s | F/2 + 3, höchstens 30 | w |

## Ablauf
F wird 16-bittig summiert (Bytes mit `cbw`), `F/2` per IDIV, dann **8-bittig** `+6` bzw. `+3` und vorzeichenbehaftet mit
30 verglichen (`jng`). Ab F/2 + 6 > 127 (F ≥ 244) bzw. F/2 + 3 > 127 (F ≥ 250) wird der Wert negativ und nicht gekappt; im
Spiel (höchstens 54 Felder) unerreichbar.

## Seiteneffekte in Reihenfolge
Kein Zufall, keine Uhr, keine Ausgabe (Probe P3, RefEmu-Protokoll leer).

## C#-Umsetzung
`src/Traders.Core/Rules/ProductionLimits.cs` (`Apply(Dgroup)`). Eingang für die Mehrrundenprobe: `2EE7`; Ausgabe: `37AA`,
`45D0`, `48A2`, `2F26`.

## Prüfung
`tests/Traders.Tests/Rules/WeeklyRulesDifferentialTests.cs`: Probe P3 (k = 0, 1, 2, 5, 9 → 3/6, 9/12, 15/18, 30/30,
30/30), 23 Summen um den Byte-Überlauf und negative Zählwerte, 300 zufällige Zustände; ganzes Datensegment verglichen.
