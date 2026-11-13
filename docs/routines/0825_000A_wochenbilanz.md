# 0825:000A / 0825:02D7 / 0825:0072 / 0825:0178 / 0825:0364 / 0825:047D / 0825:0560 / 0825:0686 / 0825:09C9 – Wochenbilanz (Spur `bilanz`, R5)

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

Transitiv gerufen und noch nicht portiert: `21A7:0054` (Ton, R10, Ereignis). Die Zeichenroutinen zeichnet die Szene
(Teil 3, Abschnitt „Bild“); `0A69`, `0A96`, `21FD:026C/0329/03EB/04DA/051F`, `21AF:04CD`, `23CD:06E9/071C` sind in
`Traders.Vga`, `0609:1ED9/1256` in `FieldScene` (gerufen), `186C:0002` über die Menüszene (gerufen).

## Bild (Teil 3, nur Zeichnen)
- `09C9` Wischer: `21FD:0590` (Zeichenseite = Anzeigeseite: `082E` = `0830`, `4516` = Seitennummer, `0B67:0238`, `0B67:028F`),
  Liste leeren; für si = 0…13Fh die Linie (si, 0)–(si − C7h, C7h) am linken Rand abgeschnitten (bis si = C7h nach (0, si)) und ihr
  Spiegelbild von unten rechts, Farbe 0; bei VGA (`DS:090E` = 1) nach jedem Paar Liste ausführen und leeren, sonst bei si mod 3 = 0.
  Die Blendstufe von si = C0h, D0h … 130h läuft nach dem Hinzufügen und vor dem Ausführen des Paars.
- `0072`: Liste leeren, `0609:1ED9`, `0609:1256(0, 0, 140h, C8h)`, gefüllte Flächen (37h, 16h) C4h × 79h und (37h, 95h) C4h × 2Ah in
  Farbe 5, Umriss der Tabelle in Farbe 9, je Spieler p `0609:1879(100h, B[302A + p]·2Ch + 0Eh, B[3D42 + p])` mit `DS:37AA` = p;
  ausführen; Schrift 1, Überschrift (Text 0Ah, Zeiger `DS:37F4`) zentriert in 140h bei y 10h, Farbe 5, Schatten 0Ah bei (−5, +3);
  Schrift 0.
- `0178`: Zeilentexte 0Bh…0Fh bei (39h, 9Bh + 6·i), Text 10h bei (39h, BCh), Farbe 2; je Spieler in Spalte 5Dh + 28h·p und Farbe
  `DS:00C8[p]` Beträge (5 Stellen, „$“): Bargeld `DS:3CD2[p]` (y 9Bh), Land A1h, Waren A7h, Edelsteine ADh, Übriges B3h, Gesamt BCh.
- `0364`: für i = 0…12 Betrag `DS:1584`·i·10 (LXMUL, 5 Stellen, „$“, Farbe 5, Schatten 0Bh bei (−1, +1)) bei (10h, 90h − 10·i)
  und waagerechte Linie Farbe 9 bei y 8Eh − 10·i ab x 37h, C4h breit; wenn `DS:1584`·120 ≥ Ziel `DS:08F4` (vorzeichenlos), die
  Ziellinie Farbe 2 bei y 8Eh − Ziel / `DS:1584` (LUDIV `0000:4F90`).
- `047D`: si = `DS:4B9A` − 27h, mindestens 0; für i = 0…39 mit (si + i) mod 10 = 0 (LMOD) senkrechte Linie Farbe 9 bei x 37h + 5·i ab
  y 20h, 6Fh hoch; für 2 ≤ i ≤ 25h die Wochenzahl si + i (2 Stellen, Farbe 0Ah, ohne Schatten) bei (32h + 5·i, 1Ch).
- `0560`: Ringpuffer `DS:411C` (40 Wochen je Spieler, Abstand A0h); erster gezeigter Index `DS:4B9A` + 1 (nur ab Woche 28h, sonst
  0), auf 0…27h zurückgefaltet; für di = 0…39, solange di ≤ `DS:4B9A`: Liste leeren, je Spieler (`DS:37AA`) Höhe v = Vermögen /
  `DS:1584` (LDIV, unteres Wort, höchstens 78h, kein unteres Limit), ab di = 1 Linie in der Spielerfarbe vom vorigen Punkt
  (lokales Feld auf dem Stapel) nach (37h + 5·di, 8Eh − v); ausführen. Danach Umriss (37h, 16h) C4h × 79h in Farbe 9. Bei
  `DS:4B9A` < 0 läuft die Spielerschleife nicht, `DS:37AA` bleibt (im Kern nachgebildet).
- `0686(p)`: Liste leeren, Fläche 30h × 2Ch bei (100h, B[302A + p]·2Ch + 0Eh) mit der Innenkachel (F0h, B0h) 20h × 16h belegen
  (`0A96:0267`) – die Marke löscht das Wappen –, dann `21FD:060D`.
- `DS:1584` = 0 (reichster Spieler mit Vermögen −239…−120) endet in `0364`/`0560` im Divisionsfehler der Laufzeit (`0000:5027`); die
  Szene wirft dann ebenfalls (`DivideByZeroException`).

## C#-Umsetzung
`src/Traders.Core/Week/Report.cs`: `Report.Run()` als Koroutine wie `MainMenu` (Rangfolge, Spalten mit Werten im Ereignis
`Draw 0178`, `Scale`, Ereignisse für jede Zeichenroutine und Schicht, Wartepunkte je Rücklauf, danach `WaitLoop`), `ReportStep`
als Schritt der Wochenschleife (Aufruf 3; Einbindung in die Phasenmaschine durch Spur 0). Abbruchweg A bleibt in der
Wochenschleife (`Report.Aborted`). Szene: `src/Traders.Presentation/Week/ReportScene.cs` spielt die `WeekEvent`s der Bilanz samt
Warteschleife (Modus 2) ab; Werte der Tabelle aus dem Ereignis `Draw 0178`, alles Übrige liest sie aus dem Kern-DS. Das Wappen
`0609:1879` zeichnet sie mit eigener Schleifenvariable nach (`FieldScene.Emblem` liest den Spieler aus `DS:37AA` des Kerns, den
eine Szene nicht setzen darf).

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
- Bild: `tests/Traders.Tests/Presentation/WeekReportSceneDifferentialTests.cs` (`[DIFFERENTIAL] 0825:000A` und Zeichenroutinen,
  Harness `WeekReportSceneHarness`): Original ab dem Eingang von `0825:000A` einer laufenden Partie mit allen Schichten, Port mit
  `Report` und `ReportScene` auf dessen Grafikzustand; VRAM aller Seiten, DAC, Bildstart und Rücklaufzahl an jedem Eingang von
  `21FD:0590`, `03EB` (nur aus `09C9`), `051F`, `0329` und am Ende. Szenarien: Woche 0, Woche 1, Woche 1 mit zwei Menschen und
  Ziffernblock +/−, langer Verlauf (Woche 45, Ringpuffer gefaltet, Werte negativ und über 78h), Ziellinie gezeichnet bzw. nicht.
  Gegenprobe: Marke eine Zeile tiefer, Ziellinie Farbe 9, Blendstufe nach dem Ausführen, Bargeldzeile y + 1 (Woche 0), Rangabzeichen
  verschoben, Verlauf 1 Pixel höher (Woche 1), Begrenzung 77h (langer Verlauf) → jeweils rot. Laufzeit etwa 3 min (Woche 1 wird erst
  erreicht).
