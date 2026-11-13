# 0E31:0003 / 0E31:0149 / 0E31:01CD / 0E31:023D / 0E31:0361 / 0E31:0379 / 0E31:0465 – neue Partie

- **Ladebild-Adresse:** Segment `0E31` (Dateioffset `0x11910`, Kopf `0x3600`), Länge `0x6D0`. Die ersten drei Byte
  (`0000`–`0002`) sind der Schluss von `0DFD`; der Schluss von `0465` (`06CE`–`06DD`) liegt hinter `0x6D0` und erscheint in
  der Segmentkarte als `0E9E:0000`–`000D` (R4P-5). Gelesen mit `ndisasm -b16 -o 0x6CE` ab Dateioffset `0x11FDE`.
- **Aufgabe:** Startzustand einer Partie: Land (Bodengüte), Spielfeld leeren, Stationen, Wetter, Startwerte je Spieler, Grundeinheit,
  Edelsteinkurs, Warenpreise, Woche.
- **Aufgerufen von / Phase:** `0003` nur aus `main` `1777:0175` (bei `DS:3CCC` = 1, [1777_000C_main.md](1777_000C_main.md)).
  `023D` zusätzlich aus `1B03:0139`, `0361` zusätzlich aus `111D:02E8` (beide als einzeln aufrufbare Kernroutinen portiert).
  `0149`, `01CD`, `0379`, `0465` nur innerhalb von `0E31`.
- **Parameter / Rückgabe:** keine (alle `retf` ohne Stapelparameter; `AX` ohne Bedeutung).

## Ablauf (S: lineares Lesen; T: Differenztest)

### `0003` neue Partie
| Stelle | Schritt |
|---|---|
| `0006` | `01CD` Spielfeld (darin `0361` Land) |
| `000B` | `023D` Stationen |
| `0010` | `21FD:021F` Wetter ([21FD_021F_wetter.md](21FD_021F_wetter.md)) |
| `0015`–`00DB` | Schleife `p` = `DS:37AA` (Byte, global) 0…3: `HqX` −1, `Rockets` 5, `FieldCount` 0, `Oil` 40, `Cash` = `InitialCash` mit `cwd` (vorzeichenbehaftet auf 32 Bit), `FatMikeFlag` 0, `0149`, `GemCredit` 50, `Gems` 2; innere Schleife `k` = `DS:45D0` (Wort, global) 0…3: `FieldsByKind[p·4+k]` 0, `Stock[p·8+k·2]` 70 |
| `00DE` | `BaseUnit` (Dword) = `InitialCash idiv 40` (vorzeichenbehaftet, Rest verworfen), mit `cwd` erweitert |
| `00EF` | `GemPriceFactor` = `random(120) + 40` (16 Bit), Hochwort 0 |
| `0105` | Schleife `DS:45D0` 0…3: `WarePrice[k]` 3 |
| `0124` | `Week` 0, `DS:4580` −1, `DS:40F6…40FC` = 0, 2, 1, 0 |

Danach stehen `DS:37AA` = 4 und `DS:45D0` = 4.

### `0149` Roboterwerte des Spielers `DS:37AA`
`RobotParts[p·6+i]` = 0 für `i` = 0…5 (Index über `imul`, Zähler in `SI`), `376C[p·4]` 0, `376E[p·4]` 0, `2EF8[p·2]` 0,
`377C[p]` 1, `40EC[p·2]` 50, `33DC[p·2]` 30, `3016[p·2]` 0. Kein Zufall.

### `01CD` Spielfeld
`0361` (Land), dann `FieldTotal` = Low-Byte von `FieldColumns · FieldRows` (`imul`); Schleifen `y` = `DS:2F00`, `x` = `DS:2EB0`
(Bytes, global, Vergleich nach `cbw` mit `Zeilen − 1` bzw. `Spalten − 1`): `FieldOwner[x·6+y]` −1, `FieldKind[x·6+y]` −2.
Danach `DS:2EB0` = Spalten, `DS:2F00` = Zeilen.

### `0361` Land
`LandType` (`DS:08F1`) = 0 → `0379` (RANDOM), sonst `0465` (REAL).

### `0379` zufälliges Land
1. `n` = Low-Byte von `(Spalten · Zeilen) idiv 4` − 1 (`cwd` auf das Low-Wort des Produkts); Zähler `c[0..2]` = `n` (Bytes am Stapel).
2. Alle Felder `FieldQuality` = 3 (Schleifen wie `01CD`).
3. Für `q` = 0…2: solange `c[q] > 0` (vorzeichenbehaftet): ziehen `x = random(Spalten − 1)` → `DS:2EB0`,
   `y = random(Zeilen − 1)` → `DS:2F00` (je Low-Byte), bis `FieldQuality[x·6+y]` = 3; dann `FieldQuality` = `q`, `c[q]−−`.
   Je `q` also genau `n` Felder; der Rest bleibt Güte 3. Zwei Aufrufstellen von `random` (`03EA`, `03F8`).

### `0465` reales Land (Gebiete wachsen um drei Keime)
1. `m` = `max(Spalten, Zeilen) − 1` (Byte; bei Gleichstand Zeilen), `n` wie in `0379`, `c[0..2]` = `n`, `SI` = `c[0]+c[1]+c[2]`.
2. Alle Felder Güte 3.
3. **Keime** für `q` = 0…2 (unbedingt): ziehen `x`, `y` wie in `0379` bis Güte 3; Keim `(sx[q], sy[q])` = `(x, y)`, Güte `q`,
   `c[q]−−`, `SI−−`.
4. `r` = 1. Solange `SI > 0` (geprüft vor jeder Runde, kopfgesteuert: `058F jmp 06D2`): sechs Durchgänge, je `q` = 0…2 mit `c[q] > 0`:
   - `random(1)` = 0 → `dx = r`, `dy = random(r)`; sonst `dx = random(r)`, `dy = r`;
   - `random(1)` = 0 → `dx = −dx`; `random(1)` = 0 → `dy = −dy`;
   - `x = sx[q] + dx` → `DS:2EB0`, `y = sy[q] + dy` → `DS:2F00` (Byte-Addition);
   - liegt `(x, y)` im Feld (`0 ≤ x < Spalten`, `0 ≤ y < Zeilen`, vorzeichenbehaftet) und hat Güte 3: Güte `q`, `c[q]−−`, `SI−−`.

   Nach den sechs Durchgängen `r = r + 1`, bei `r > m` (vorzeichenlos) wieder 1.
   Ziehungen je Versuch: 4 (`random(1)`, `random(r)`, `random(1)`, `random(1)`). Sieben Aufrufstellen (`0502`, `0510`, `05B9`,
   `05CE`, `05DE`, `05F1`, `0607`).

`DS:2EB0`/`2F00` bleiben auf dem letzten Versuch stehen (auch außerhalb des Feldes); nach `01CD` sind sie überschrieben, beim
Aufruf von `0361` aus `111D:02E8` nicht.

### `023D` Stationen
Zähler je Art als Dwords am Stapel: 6, 4, 2. Für `i` = 0…11 (Dword-Zähler):
- `StationVisited[i]` = 0;
- Wert: `random(2)` ≠ 0 → −1; sonst `random(2)` ≠ 0 → 10; sonst `random(3)` (0…3). Da `random(n)` 0…n liefert, ist −1 mit
  2/3, 10 mit 2/9 und 0…3 mit je 1/36 belegt (die Angabe „½, ¼“ in `DATENLAYOUT.md` §2 Zeile 34 stimmt nicht; s. §3.1 dort);
- Art: `r = random(2)` (0…2) so lange, bis `Zähler[r]` ≠ 0; dann `Zähler[r]−−`, `StationKind[i]` = `r`.

Danach `Station[p]` = −1 für `p` = `DS:37AA` 0…3 (danach 4). Ziehungen: 1–3 für den Wert, ≥ 1 für die Art je Station; vier
Aufrufstellen (`027A`, `028D`, `02A0`, `02BF`).

**Statische Aufrufstellen von `21FD:01BA`:** 1 + 4 + 2 + 7 = 14 (R4P-5); die tatsächliche Zahl und Reihenfolge ergibt sich aus den
Wiederholschleifen oben und dem Zufallszustand. Reihenfolge der Ziehungen in `0003`: Land (`0379`/`0465`), Stationen, Wetter
(`random(2)`, `random(4)`), Edelsteinkurs `random(120)`.

## Zustand
| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `08F1` | B | – | `LandType` 0 RANDOM / sonst REAL | r |
| `08F2` | W | s | `InitialCash` | r |
| `08FC` / `08FE` | W | s | Spalten / Zeilen | r |
| `10D2` | D | u | Zufallszustand | r/w |
| `37AA`, `45D0`, `2EB0`, `2F00` | B, W, B, B | s | globale Schleifenvariablen | w |
| `45DA`, `4442`, `2EB1` | B[9·6] | s | Bodengüte, Besitzer, Art je Feld | w |
| `2F05` | B | s | Feldanzahl | w |
| `2F1A`, `348A`, `44F4` | B[12], W[12], W[12] | – / s / s | Stationen: besucht, Wert, Art | w |
| `40E6` | B | s | Wetter | w |
| Spielerfelder (`40FE`, `4BAC`, `4BA8`, `4520`, `3CD2`, `4BC2`, `300E`, `37AF`, `2EE7`, `3CF4`, `450E`, `43CE`, `376C`, `376E`, `2EF8`, `377C`, `40EC`, `33DC`, `3016`) | s. `DATENLAYOUT.md` §2 | | Startwerte | w |
| `3CF0`, `4436`, `4898`, `4B9A`, `4580`, `40F6` | D, D, W[4], W, B, W[4] | s | Grundeinheit, Edelsteinkurs, Warenpreise, Woche, … | w |

Nicht berührt: `HqY`, `Unknown4BBE`, `WealthHistory`, Optionen.

## Seiteneffekte in Reihenfolge
Nur Zufall (kein `random(0)`, also keine Uhr); keine Grafik, keine Eingabe, kein Ton, kein Dateizugriff.

## C#-Umsetzung
`src/Traders.Core/Rules/NewGame.cs` (`Start`, `ResetRobots`, `CreateField`, `CreateStations`) und `NewGameLand.cs` (`Create`,
`Random`, `Real`), je `(Dgroup, OriginalRandom)`; der Aufrufer hält den Zufallszustand. VGA-unabhängig, keine Zweige ausgelassen.

## Prüfung
`tests/Traders.Tests/Rules/NewGameDifferentialTests.cs`: gesamtes Datensegment, `DS:10D2` und Zahl der Ziehungen gegen den
RefEmu (`LoadOnly`/`Call`), alle vier Stufen, beide Landarten, gemischte Mensch/Computer-Belegung, viele Zufallszustände, alle
Feldgrößen 4–9 × 3–6, Teilroutinen `023D`, `0361`, `0379`, `0465`, `01CD` einzeln.
