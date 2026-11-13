# 21FD:0004 / 21FD:060D / 1F63:113E / 1F63:104B / 0825:0686 – Warteschleife der Wochenbildschirme (Spur `bilanz`, R5)

- **Ladebild-Adresse:** Segment `21FD` (Dateioffset `0x255D0`), `0004` bei `0x255D4`; `1F63:113E`, `0825:0686` in ihren Segmenten.
- **Aufgabe:** gemeinsame Bestätigungsschleife: jeder der vier Spieler bestätigt einmal (Mensch mit Feuer, Computer von
  selbst), dann kehrt sie zurück. Modus (Argument) wählt die Zusatzwirkung je Bestätigung bzw. je Durchlauf.
- **Aufgerufen von / Phase:** zehn Aufrufstellen; `0609:014E` (Anbau, Modus 1), `0825:006A` (Wochenbilanz, Modus 2),
  `111D:0109` (Modus 3), `1F63:0C67` (Modus 4), weitere in `17CB`, `1DEF`, `1F63:08D1` mit berechnetem Modus.
- **Parameter / Rückgabe:** `[bp+6]` Modus; kein Rückgabewert.

## Ablauf (eigene Worte)
- Maske `[bp-1]` = 0Fh, Befehlsliste leeren (`0A96:001B`), Zähler `si` = 150, wenn alle vier Computer sind (`DS:4581` = 1),
  sonst 100.
- Je Durchlauf, solange die Maske ≠ 0: Seitenwechsel `21FD:051F`, Objekthintergründe zurück (`23CD:071C`), Liste ausführen und
  leeren, Pause `21FD:0245` (wartet nur bei gedrücktem P, `DS:3D60` = Tastentabelle + 19h), `si` −1 solange > 0; Rücktaste
  gedrückt (`DS:3D55`) → sofort zurück; Modus 4 → `1F63:113E`; Steuerung `0E9E:037A` (setzt `DS:3308` für Menschen).
- Schleife über `DS:37AA` (bei `0067`): Computer (`DS:0840` = 1) bekommen Aktion 0Ah, wenn `p·20 = si`, sonst FFh. Damit bestätigen
  die Computer 3, 2, 1 bei `si` = 60, 40, 20 und Spieler 0 bei `si` = 0 (der Zähler bleibt dort stehen).
- Schleife über `DS:37AA` (bei `00A5`): Spieler mit Bit `DS:08C6[p]` in der Maske und Aktion 0Ah: Bit löschen
  (`mask &= FFh − bit`), Ton über die Sprungtabelle `21FD:017D` (`21A7:0054` mit (0, 3), (1, 4), (2, 5), (0, 6)), Modus 1 →
  `0609:1FCC` (Objekt `DS:37AA` mit Bild 0 zeigen, `21AF:0412`), Modus 2 → `0825:0686(p)`.
- Ende des Durchlaufs: Liste ausführen, Objekte zeichnen (`23CD:06E9`). Danach steht `DS:37AA` = 4.
- `0825:0686(p)`: Liste leeren, Marke in der Zeile des Rangs `DS:302A[p]` (y = Rang·44 + 14, `0A96:0267`), dann `21FD:060D`
  (Liste ausführen, Seitenwechsel, Liste ausführen) – also ein Seitenwechsel mehr je Bestätigung in Modus 2.
- `1F63:113E`: `DS:54B0` > 0 (vorzeichenbehaftet) → herabzählen, bei 0 `1F63:104B(3, 1)`; sonst `random(200)` (`21FD:01BA`), bei 0
  `1F63:104B(3, 0)` und `DS:54B0` = 8. Einziger Zufall der Schleife.
- `1F63:104B(bits, Zustand)`: je Bit 0/1 ein Bild 20h·28h an x D0h bzw. F4h über `0A96:0030`, Bildquelle nach Zustand 1, 2 oder
  sonst; nur Grafik.
- `DS:2701` (im Plan als geschrieben genannt) ist ein Lesefehler der Disassemblierung: die Bytes ab `21FD:017D` sind die
  Sprungtabelle (`00F5`, `0105`, `0116`, `0127`), kein Code. `21FD:0665`/`0590` werden von `21FD:0004` nicht gerufen (Teil 2b).

## Zustand
| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `4581` | B | u | alle Spieler Computer | g |
| `3D55`, `3D60`, `3D91`, `3D95` | B | u | Tastentabelle: Rücktaste, P, Ziffernblock −/+ | g |
| `0840` | B[4] | u | Computer | g |
| `08C6` | B[8] | u | Bitmasken | g |
| `3308` | B[4] | s | Aktion je Spieler | g/s |
| `37AA` | B | s | Schleifenvariable, danach 4 | s |
| `54B0` | B | s | Zähler der Anzeige (Modus 4) | g/s |
| `302A` | B[4] | u | Rang (Modus 2, nur Zeichnen) | g |
| `10D2` | DW | – | Zufall (Modus 4) | g/s |
| Steuerung `0E9E` | wie `0E9E_000E_steuerung.md` | | | g/s |

## Seiteneffekte in Reihenfolge
Je Durchlauf: Seitenwechsel (Rahmen), Liste, Pause, Zufall (nur Modus 4, höchstens einmal), Eingabe, Ton je Bestätigung,
Objekt bzw. Marke mit Seitenwechsel je Bestätigung, Liste, Objekte. Keine Uhr.

Transitiv gerufen und noch nicht portiert: nur `21A7:0054` (Ton, R10) – als Ereignis `Sound` gemeldet (`[APPROX]` wie im Menü).
Übrige Rufe sind portiert: `0E9E:037A` (`ControlReader`), `21FD:051F`/`0245` (wie `MainMenu`), Liste `0A96`, Objekte `23CD`/`21AF`
(`Traders.Vga`); sie werden als Ereignisse an die Szene gemeldet.

## C#-Umsetzung
`src/Traders.Core/Week/WaitLoop.cs` (`WaitLoop.Run()` als Koroutine wie `MainMenu`, wartet mit `MenuWait` an Seitenwechsel und
Pause), Ereignisse `WaitLoopEvents.cs` (`WeekEvent`). Zustand und Zufall im Kern; Zeichnen, Ton und Objekte meldet der Kern als
Ereignisse, die Szene führt sie aus.

## Prüfung
`tests/Traders.Tests/Week/WaitLoopDifferentialTests.cs` (`[DIFFERENTIAL] 21FD:0004`, Harness `WaitLoopHarness`): Original mit
gestubbten Schichten (`0A96`, `0A69`, `21A7`, `23CD`, `0609:1FCC`, `21FD:051F`), Rahmen wie im Menü-Harness; verglichen werden das
ganze Datensegment (inkl. `DS:10D2`), die Rahmenzahl, `random()` je Rahmen und die Schichtaufrufe mit Rahmen und Argumenten.
Szenarien: alle Computer (Zähler 150, Spieler 0 bei 0), gemischt mit Rangfolge, Menschen im ersten Rahmen, Ziffernblock +/−,
Modus 1 und 3, Pause, Abbruch (auch im ersten Rahmen), Modus 4 mit herabzählender, negativer und ruhender Anzeige über lange
Läufe. Gegenprobe: Ziehung in `1F63:113E` ausgelassen → 5 Szenarien rot; Computerzeitpunkt um eins verschoben → 12 rot.
