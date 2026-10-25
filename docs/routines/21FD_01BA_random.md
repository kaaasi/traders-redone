# 21FD:01BA – random(n)

- **Ladebild-Adresse:** `21FD:01BA` (Dateioffset `0x2578A`)
- **Aufgabe:** Zufallszahl `0..n` (inklusive); `n == 0` setzt den Seed aus der Uhr.
- **Aufgerufen von:** ~145 Far-Aufrufstellen in 16 Modulen (u. a. 0E31, 111D, 17CB, 192D, 1B03,
  1E7E, 1F63) sowie near innerhalb 21FD (z. B. `21FD:021F`).
- **Parameter / Rückgabe:** `int n` → `int` in AX.

## Zustand
| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `10D2` | 32 Bit | unsigned | Seed, Startwert 1 (Datensegment) | r/w |

## Ablauf
```
if (n == 0) { seed = time(NULL); return 0; }      // 0000:3405 = Borland time()
seed = seed * 0xBB40E62D + 1;                      // 0000:4F4E = LXMUL (CX:BX = BB40:E62D)
return (seed >> 8) % (unsigned long)(n + 1);       // 0000:5058 = unsigned long >>, 0000:4FA0 Modus 3 = LUMOD
```
`n + 1` wird 16-bittig gebildet und vorzeichenlos auf 32 Bit erweitert (`xor dx,dx`).

## Seiteneffekte
- Uhr: nur bei `n == 0` (`time()` → DOS `int 21h` AH=2Ah/2Ch). Für Wiederholbarkeit muss
  dieser Wert als Eingabe aufgezeichnet werden.
- Borland `time()` (`0000:3405`) rechnet die lokale DOS-Zeit mit der Vorgabe-Zeitzone EST5EDT um
  (`timezone` = 18000 s, Sommerzeit an, kein `TZ` gesetzt): Seed = Sekunden seit 1970 der lokalen
  Zeit + 18000 − 3600 (im Sommer). Im RefEmu-Lauf 01.06.1991 12:00:00 → `675792000`.
- `random(-1)`: `n + 1 = 0` → Division durch 0 in LUMOD (`int 0`). Der Port wirft
  `DivideByZeroException`; beides ist als Fehlerfall geprüft.
- Erster Aufruf mit `n == 0` im Spiel: Code-Abfrage beim Start ([21FD:0687](21FD_0687_codeabfrage.md)).

## Plausibilitätsbefund
Erster Schritt ab Startseed 1 ergibt `3141592622` (π) – die Konstante ist offensichtlich bewusst gewählt.

## C#-Umsetzung
`src/Traders.Core/OriginalRandom.cs`

## Prüfung
- `tests/Traders.Tests/OriginalRandomTests.cs` (Formel-Referenz, Neusaat, fehlende Uhr-Eingabe).
- **Differenztest gegen das ausgeführte Original:** `tests/Traders.Tests/RefEmu/RandomDifferentialTests.cs`
  – 3000 Aufrufe mit Grenzwerten und zufälligen `n` (Rückgabe und Seed nach jedem Aufruf), fünf
  Start-Seeds, `n = 0` mit Borland-`time()`, `n = -1` (Division durch 0). Alle gleich.
