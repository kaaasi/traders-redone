# 1C68:0A28, 1C68:0AB0, 1C68:0B02 – Preis eines Kaufpostens, Wert und Mittel eines Teilepaars (Sammelbefund)

- **Ladebild-Adresse:** Segment `1C68` (Dateioffset `0x1FC80`); `0A28` bei `0x206A8`, `0AB0` bei `0x20730`, `0B02` bei `0x20782`.
- **Aufgabe:** drei reine Rechenhilfen der Roboterphase für den Spieler am Zug `p = DS:37AA` (Byte mit Vorzeichen):
  `0A28(item)` liefert den Preis eines Kaufpostens, `0AB0(slot)` den Wert eines Teilepaars, `0B02(slot)` dessen Mittel.
  Bedeutung der Posten und Teile ist **vermutet** (Kaufbildschirm `1C68:00A5` und Kauf-KI `16D6`, Spuren 1 und 2 Teil 2).
- **Aufgerufen von / Phase:** Aufruf 18 der Woche (Roboterkauf). Laut `tools/callgraph.py`: `0A28` von `16D6:0067`, `16D6:02F7`,
  `16D6:057C`, `1C68:0515`, `1C68:0681`, `1C68:0812`; `0AB0` nur von `0A28` (`1C68:0A9C`); `0B02` von `16D6:0823`,
  `1C68:0E87` und `0EDE:16D2` (R11). Die Schnittstellentabelle des Plans nennt für `0AB0` auch `16D6` – im Aufrufgraph nicht belegt.
- **Parameter / Rückgabe:** je ein Wort bei `bp+6`; Ergebnis in `AX` (Wort mit Vorzeichen). `0AB0` lässt in `DX` das hohe Wort der
  Multiplikation, `0B02` den Rest der Division; kein Aufrufer liest `DX` (Aufrufstellen geprüft: Ergebnis wird aus `AX` gespeichert).
  Kein Zustand geschrieben, kein Zufall.

## Ablauf (eigene Worte)

- **Teilezehntel:** `T(slot, h) = RobotParts[p·6 + slot·2 + h] / 10` mit `h` 0 oder 1; das Byte wird ohne Vorzeichen erweitert,
  geteilt wird mit `idiv` (Ergebnis 0–25). Der Index ist 16 Bit (`p·6` aus `imul`, `slot` per `shl` verdoppelt, Summe in `bx`).
- **`0AB0(slot)`** = `(T(slot,0) + T(slot,1)) · 75`, unteres Wort von `imul` (höchstens 3 750).
- **`0B02(slot)`** = `(T(slot,0) + T(slot,1)) / 2`, abgeschnitten (`idiv` 2).
- **`0A28(item)`**, Vergleiche mit Vorzeichen auf dem ganzen Wort:
  - `item < 8`: `ItemPrice[item]` (Wort bei `0E8F + 2·item`, Index per `shl` 16 Bit, auch negativ).
  - `item < 12`: `ItemPrice[item]`; ist `o = OwnedUpgrade[p]` (Byte bei `2F01 + p`) ≠ `0xFF`, wird `UpgradePrice[o]` abgezogen
    (Wort bei `0E9F + 2·o`, `o` mit Vorzeichen erweitert; `0E9F` = `0E8F + 16`, also der Preis des Postens `8 + o`).
  - sonst nur das untere Byte zählt: `t = (sbyte)(item − 12)` (`add al,0F4h`); `r = t mod 7`, `q = t / 7` (`idiv` mit `cbw`/`cwd`,
    Rest mit Vorzeichen des Dividenden); Ergebnis `(sbyte)(r·2) · 75 − 0AB0(q)` (`q` als ganzes Wort übergeben).

## Zustand

| DS-Offset | Breite | Vorzeichen | Bedeutung (vermutet) | gelesen/geschrieben |
|---|---|---|---|---|
| `37AA` | B | s | Spieler am Zug `p` | L |
| `43CE` | B[4·6] | u | `RobotParts`, je Spieler drei Paare | L (`0AB0`, `0B02`) |
| `0E8F` | W[12] | s | `ItemPrice`, Grundpreis je Kaufposten 0–11 (initialisierte Daten, Ladebild: 90, 190, 380, 660 für 8–11) | L (`0A28`) |
| `0E9F` | W[4] | s | `UpgradePrice` = `ItemPrice[8..11]` | L (`0A28`) |
| `2F01` | B[4] | s | `OwnedUpgrade`, besessener Posten der Gruppe 8–11 (0–3), `0xFF` = keiner | L (`0A28`) |

## Seiteneffekte in Reihenfolge

Keine (kein Zufall, keine Grafik, keine Eingabe, kein Ton; Stapel ausgeglichen, `si`/`di` gesichert).

## C#-Umsetzung

`src/Traders.Core/Robots/Shared/RobotHelpers.cs`: `ItemPrice(Dgroup, short)` = `0A28`, `PartPairValue(Dgroup, short)` = `0AB0`,
`PartPairAverage(Dgroup, short)` = `0B02`; Namen der Adressen in `RobotSharedLayout.cs`. `public static`, ohne Bild (Schnittstelle
R10 „Hilfsroutinen“).

## Prüfung

`tests/Traders.Tests/Robots/Shared/RobotHelpersDifferentialTests.cs` (RefEmu `LoadOnly`, direkter Far-Call): `AX`, ganzes DS und
Zufallszustand `DS:10D2` gleich. Eingänge: Spieler 0–4 und negativ, Teilebytes 0–255 (Vielfache von 10 und knapp darunter),
Preistabelle original und zufällig (auch negativ), `OwnedUpgrade` `0xFF`, 0–3 und andere negative Bytes, Posten an den Kanten
7/8, 11/12, 18/19, 32/33, negativ und mit überlaufendem unteren Byte (139/140, 267/268, `±32767`), Paare −2 bis 5.
Gegenprobe (lokal ausgeführt): `OwnedUpgrade` ohne Vorzeichen erweitert → rot; Quotient `q` vorzeichenlos → rot.
