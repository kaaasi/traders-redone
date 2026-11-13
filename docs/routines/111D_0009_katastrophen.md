# 111D:0009 – Katastrophen und Wochenereignisse (nur Wächter `111D:000D–001B`)

- **Ladebild-Adresse:** `111D:0009` (Dateioffset `0x147D9`, Segment `111D` bei `0x147D0`)
- **Aufgabe:** Aufruf 4 der Wochenschleife `1777:01F8` (`1777:021A`). Portiert ist **nur der Wächter** (R5P-6): in den ersten
  Wochen und bei abgeschalteten Katastrophen kehrt die Routine sofort zurück. Der Rest (Zufall `random(10)`, `random(2)`, Bild
  `186C:0002([08CE])`, Ränge `0825:06CB`, Einblendung, Änderungen an Bodengüte und Feldart) ist ausstehend.
- **Aufgerufen von / Phase:** nur `1777:021A` (Aufruf 4, nach der Wochenbilanz).
- **Parameter / Rückgabe:** keine; AL wird von der Wochenschleife nach Aufruf 4 nicht ausgewertet.

## Ablauf des Wächters (eigene Worte)

`push bp`/`mov bp,sp`/`push si`; Wort `DS:4B9A` (Woche, mit Vorzeichen) mit 2 vergleichen: nicht größer (`jng`) → Sprung zum
Ende `010F`. Sonst Byte `DS:2F30` mit 0 vergleichen: gleich → Ende. Erst dann (`001E`) folgt der Rest mit `21FD:01BA(10)`.
Das Ende `010F` ist `pop si`, `pop bp`, `retf` – kein Schreibzugriff, kein Zufall.

## Zustand

| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `4B9A` | W | s | Woche (`DgroupLayout.Week`) | gelesen |
| `2F30` | B | u | Katastrophen an/aus (`DgroupLayout.Catastrophes`) | gelesen |

## Seiteneffekte in Reihenfolge

Im Wächterweg keine (kein Zufall, keine Uhr, keine Grafik, kein Ton, keine Eingabe).

## C#-Umsetzung

`src/Traders.Core/Phases/CatastropheGuardStep.cs`: `ReturnsAtOnce` = Wächter; der Schritt endet dort sofort. Sonst meldet er
sich über `IPartialWeekStepHandler.IsPending` als ausstehend: die Phasenmaschine hält an Aufruf 4 an (`PhaseStatus.Blocked`,
Ereignis `StepPending`), ohne ihn zu beginnen. Ein Test kann den Rest mit einem Orakel ausführen (Parameter `rest`).

## Prüfung

`tests/Traders.Tests/Phases/CatastropheGuardDifferentialTests.cs`: Original über `LoadOnly`/`Call(111D:0009)` mit gesetzter
Woche (−32768, −1, 0, 1, 2, 3, 4, 15, 30, 32767) und `2F30` (0, 1, 80h, FFh); der Rest ab `001E` ist durch den eigenen Epilog
ersetzt, sein Erreichen wird aufgezeichnet. Wächterweg: Port ändert nichts, ganzes Datensegment samt `10D2` gleich dem Original;
sonst ist der Port ausstehend, genau wenn das Original `001E` erreicht. In der Mehrrundenprobe
(`MultiWeekScreenDifferentialTests`) läuft Aufruf 4 im Port, solange der Wächter greift, sonst aus dem RefEmu.
