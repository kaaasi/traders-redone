# 0825:000A / 0825:02D7 / 0825:0072 / 0825:0178 / 0825:0560 / 0825:09C9 – Wochenbilanz (Spur `bilanz`, R5)

- **Ladebild-Adresse:** Segment `0825`, `000A`; Zeichenroutinen `0072`, `0178`, `0364`, `047D`, `0560`, `09C9`; Maßstab `02D7`.
- **Aufgabe:** Bilanzbildschirm der Woche: Rangfolge, Tabelle (Land, Waren, Edelsteine, Übriges, Gesamt je Spieler),
  Vermögensverlauf mit Maßstab `DS:1584`; mit Argument 1 Öffnungswischer und Bestätigung aller Spieler.
- **Aufgerufen von / Phase:** `1777:0208` (Wochenschleife, Aufruf 3, Argument 1; danach prüft `1777:020E` die Rücktaste
  `DS:3D55` → zurück ins Menü), `20B3:007D` (Spielende, Argument 0, ohne Warten).
- **Parameter / Rückgabe:** `[bp+6]` (Byte-Vergleich mit 1); kein Rückgabewert.

## Ablauf (eigene Worte)
1. `0825:06CB` Rangfolge (portiert, `Ranking.Apply`).
2. `186C:0002(DS:08CE)` Bild laden.
3. Argument 1: `09C9` Öffnungswischer – `21FD:0590` (Zeichenseite = Anzeigeseite, zwei Rückläufe), Liste leeren, 320 Spaltenpaare
   (je zwei Linien `0A96:0433`; Liste ausführen bei VGA jedes Paar, sonst jedes dritte); ab Spalte C0h jede 16. eine
   Blendstufe `21FD:03EB(0, 1…8)` (je ein Rücklauf), danach `21FD:04DA(0)`, Seitenwechsel `21FD:051F`, `21AF:04CD`.
   Nur Grafik und Rahmen, kein Zustand.
4. `0072` Hintergrund, Rahmen, Wappen je Spieler (`0609:1ED9`, `1256`, `1879`), Überschrift (`0A69:00D3`); Schleife über
   `DS:37AA` (danach 4), sonst nur Grafik.
5. `0178` Tabelle: je Spieler (`DS:37AA`) Name, Land `086F`, Waren `088D`, Edelsteine `08EB`, Übriges `096D`, Gesamt `0813`
   (`088D`/`0813` hinterlassen `DS:45D0` = 4), Zahlen über `0A96:05FB`; danach `DS:37AA` = 4.
6. `02D7` Maßstab: Vermögen `DS:2F06` des (letzten) Spielers mit Rang `DS:3D42` = 0, LDIV durch 120, + 1, als Wort; > 10 →
   (n/10)·10 + 10; danach > 100 → (n/100)·100 + 100 (16-Bit-`idiv`). Schleifenvariable `DS:37AA` = 4. Ohne Spieler mit Rang 0
   wäre der Wert eine uninitialisierte lokale Variable; `06CB` lässt immer einen Spieler auf Rang 0.
7. `0364`, `047D` Beschriftung und Achsen (nur Lesen, Langarithmetik der Laufzeit), `0560` Verlauf je Spieler (`DS:37AA`,
   danach 4; lokales Feld auf dem Stapel).
8. `0A96:0C3F(DS:082E)`, `21FD:026C(DS:00A8)`, langsames Einblenden `21FD:0329` (32 Rückläufe).
9. Argument 1: Warteschleife `21FD:0004(2)` (`21FD_0004_warteschleife.md`): Bestätigung mit Marke in der Rangzeile.

## Zustand
| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `3D42`, `302A`, `2F06` | B[4], B[4], DW[4] | u, u, s | Rang, eindeutiger Rang, Vermögen (`06CB`) | s |
| `1584` | W | s | Maßstab des Verlaufs | s |
| `37AA` | B | s | Schleifenvariable, danach 4 | s |
| `45D0` | W | s | Schleifenvariable der Waren, danach 4 | s |
| `08CE` | W | – | Bildnummer | g |
| Vermögensquellen | wie `0825_0813_vermoegen.md` | | | g |
| `090E` | W | – | Anzeigemodus (nur Zeichnen) | g |

## Seiteneffekte in Reihenfolge
Bild laden, Wischer mit Blendstufen und Seitenwechsel (Rahmen), Zeichnen, Palette, Einblenden (Rahmen), Warteschleife mit
Eingabe, Ton und Seitenwechseln. Zufall nur in der Warteschleife (Modus 2 zieht nicht). Keine Uhr.

Transitiv gerufen und noch nicht portiert: `21A7:0054` (Ton, R10, Ereignis). Die Zeichenroutinen `0072`, `0178`, `0364`,
`047D`, `0560`, `09C9` und `0609:1ED9/1256/1879` zeichnet die Szene (Teil 3); `0A69`, `0A96`, `21FD:026C/0329/03EB/04DA/0590`,
`21AF:04CD` sind in `Traders.Vga`.

## C#-Umsetzung
`src/Traders.Core/Week/Report.cs`: `Report.Run()` als Koroutine wie `MainMenu` (Rangfolge, Spalten mit Werten im Ereignis
`Draw 0178`, `Scale`, Ereignisse für jede Zeichenroutine und Schicht, Wartepunkte je Rücklauf, danach `WaitLoop`), `ReportStep`
als Schritt der Wochenschleife (Aufruf 3; Einbindung in die Phasenmaschine durch Spur 0). Abbruchweg A bleibt in der
Wochenschleife (`Report.Aborted`).

## Prüfung
- `tests/Traders.Tests/Week/ReportDifferentialTests.cs` (`[DIFFERENTIAL] 0825:000A`, Harness `WaitLoopHarness` mit gestubbten
  Schichten wie im Menü): ganzes Datensegment, Rahmenzahl, `random()` je Rahmen, Schichtaufrufe mit Rahmen und Argumenten.
  Szenarien: Wochenschleife mit vier Computern, gemischt, Ziffernblock +/− im Wischer, Abbruch, Spielende (Argument 0),
  Maßstab klein, Zehner, Hunderter, genau an der Grenze, sehr groß, alle Vermögen negativ; Ast-Prüfung am Original.
- `ReportMultiWeekTests` (Mehrrundenprobe): Standardspiel mit vier Computern im RefEmu mit allen Schichten, Wochen 0–3; der
  Kern übernimmt das DS am Eingang von `0825:000A`, läuft die Bilanz samt Warteschleife und wird an der Rückkehr `1777:020D` mit
  dem RefEmu verglichen (Rang, Vermögen, Maßstab, Schleifenvariablen, Aktionen, `DS:54B0`, `DS:10D2`). Grafik-, Paletten- und
  Seitenzustand nicht (Szene). Laufzeit etwa 7 min.
- Gegenprobe: Maßstab vom Spieler mit Rang 3 statt 0 → 11 von 12 rot; Rangfolge ausgelassen → 11 von 12 rot.
