# Review PR #111 – Host holt übersprungene Bilder bildweise nach (R4G-1)

Erstreview (Claude Opus, frische Sitzung ohne Implementierungskontext). Base `5f102a5`, Head
[`defac95`](https://github.com/kaaasi/traders-redone/commit/defac95b32d78c85bfada5eef3867aa772aadc31); Checkout mit
`git rev-parse HEAD` = Head, `git status --porcelain` leer. Originaldaten geprüft (Referenz-Repo `7fcfb24`, Hashes grün).

## 1. Merge-Empfehlung

**Ja, für genau Head `defac95`.** R4G-1 ist behoben: die Probe aus `2026-11-12-r4-gesamt.md` ist auf dem Head grün und auf der
alten Nachholung rot, die drei neuen Testfälle ebenso. `ProgramHost.Frame` führt je nachgeholtem Bild Kern (`Advance` mit genau
`LastFrame + 1`), Ereignisleerung, `Scene.NextFrame` und `Scene.Apply` in dieser Reihenfolge aus – identisch zum bildweisen
Host. Eine eigene Probe über den Übergang Menü → Partie → Wochenschleife (Sprünge zu 17 und 18 Bildern, RETURN innerhalb eines
Sprungs) ist ebenfalls bildgleich. Alle drei Läufe von `tools/pruefen.sh` grün, CI `linux` auf dem Head grün. Es bleiben nur
zwei P3.

## 2. Befunde

**P3-1 – Obergrenze im Test und Kommentar zu knapp beziffert.**
[`ProgramHostCatchUpTests.cs` Z. 22](https://github.com/kaaasi/traders-redone/blob/defac95b32d78c85bfada5eef3867aa772aadc31/tests/Traders.Tests/App/ProgramHostCatchUpTests.cs#L22)
nennt 17 Bilder „the upper bound of one update“. Mit `MaxStep` = 0,25 s und `HostClock.FramePeriod` = 17045 PIT-Takte sind es
0,25 · 1193182 / 17045 ≈ 17,5, durch den Rest also abwechselnd 17 und **18**. Beleg (Probe, temporäre Kopie):
`new HostClock()` 1000-mal `Advance(HostGame.MaxStep)` → größter Bildzuwachs `Assert.Equal(18, max)` grün. Die Funktion ist
nicht betroffen (Sprung 18 und Sprung 0 → 600 sind in der Probe bildgleich), nur Testdaten/Kommentar: Fall `17` → `18` bzw.
„17–18“.

**P3-2 – Kommentar „after a short stall“ untertreibt den Normalfall; Schleife in `HostGame.Update` ist redundant.**
[`HostGame.cs` Z. 91–97](https://github.com/kaaasi/traders-redone/blob/defac95b32d78c85bfada5eef3867aa772aadc31/src/Traders.App/HostGame.cs#L91):
Mehrere fällige Bilder je Update entstehen nicht nur nach einem Stillstand, sondern regelmäßig bei 60-Hz-Anzeige (Vsync) gegen
70 Hz Spieltakt (etwa jedes sechste Update zwei Bilder) – der Fix wirkt also im Normalbetrieb. Die Schleife
`for (f = LastFrame + 1; f < _clock.Frames …) _program.Frame(f, _held)` ist gleichwertig zu einem Aufruf
`_program.Frame(_clock.Frames - 1, _held)`, da `ProgramHost.Frame` selbst bildweise nachholt; sie ist unschädlich, aber
doppelte Logik. `HostGame.Update` ist ohne Fenster nicht testbar; die Gleichwertigkeit ist nur durch Codelesen belegt
(Verdacht-Niveau, kein Fehler).

**Geprüfte Punkte ohne Befund:**

- **Tasten je Bild:** Alle nachgeholten Bilder bekommen die jetzt gehaltenen Tasten; `ProgramMachine.Advance` schreibt die
  Tastentabelle nun je Bild (vorher einmal je Sprung) – genau wie beim bildweisen Host, daher gleich. Tastenverlauf innerhalb
  des Stillstands ist dem Host unbekannt; unverändertes Verhalten gegenüber Base.
- **Uhr je Bild:** `Timer.Tick()` wird vor den Bildern gesammelt gezählt; `TimerTicks` wird außerhalb `HostGame` nirgends gelesen
  (`grep -rn "\.Timer\b\|TimerTicks" src`). `time()` (`BorlandClock`) liefert Sekunden – innerhalb eines Updates ohne Wirkung.
- **Übergang Menü → Partie und Ende:** eigene Probe (unten) bis `InWeek` + blockiert bildgleich; die Schleifen brechen bei
  `ProgramStatus.Ended` ab, `Advance` erhöht `LastFrame` sonst immer → keine Endlosschleife.
- **`MaxStep` 0,25 s:** begrenzt den Zuwachs der Host-Uhr selbst, nicht nur die Verarbeitung. Bei längerem Stillstand gehen die
  darüber hinausgehenden Bilder als Zeit verloren (das Spiel läuft entsprechend später weiter), es entsteht keine Lücke zwischen
  `_clock.Frames` und `LastFrame`. Höchstens 18 Bilder je Update; Kosten der Nachholung vernachlässigbar (Sprung 0 → 600 in
  Millisekunden). `[APPROX]`-Begründung (verlorene Interrupts) ist plausibel; das Original holt nach einem Stillstand gar
  nichts nach (Takt = Retrace-Warten), was der Kappung entspricht.
- **Architektur:** `Traders.App.csproj` referenziert Assets, Core, Presentation, Vga; keines davon referenziert RefEmu
  (`grep -l RefEmu src/*/*.csproj` → nur RefEmu selbst); `tools/check-architecture.py` grün.

## 3. Ausgeführte Prüfungen

| Prüfung | Ergebnis |
|---|---|
| `tools/pruefen.sh` (alle drei Läufe) auf `defac95` | grün |
| Mit Originaldaten (`TR_REQUIRE_GAMEDATA=1`) | 754 bestanden / 0 / 0 |
| Ohne Originaldaten (`TR_SKIP_GAMEDATA=1`) | 439 / 0 / 208 übersprungen |
| Pflichtlauf ohne Daten (muss scheitern) | 439 / 315 fehlgeschlagen (erwartet) |
| Markerbericht | 248 `[ORIGINAL]`, 28 `[APPROX]` (App 4/8) |
| CI `linux` auf Head (Run 43319002467) | success |

**Gegenbeispiel R4G-1** (Probe `ReviewFrameProbe` wörtlich aus `2026-11-12-r4-gesamt.md`, temporär nach
`tests/Traders.Tests/App/`, `TR_REQUIRE_GAMEDATA=1 dotnet test … -c Release --filter "FullyQualifiedName~ReviewFrameProbe|FullyQualifiedName~ProgramHostCatchUp"`):

| Stand | Probe R4G-1 | `ProgramHostCatchUpTests` (3 Fälle) |
|---|---|---|
| Head `defac95` | grün | 3 grün |
| `ProgramHost.cs` + `HostGame.cs` aus Base `5f102a5` | **rot** (Pixel weichen ab) | **3 rot** (Z. 46, Szenen-Datensegment) |

**Eigene Probe `ReviewCatchUpProbe2`** (temporär, nicht committet): zwei Hosts mit `RecordedClock([12345678])`, einer bildweise,
einer in Sprüngen von n Bildern; nach zwei Menüdurchläufen RETURN für einen ganzen Sprung; Vergleich nach jedem Sprung von
`LastFrame`, `Status`, `NextStep`, Kern-DS, Szenen-DS, DAC, `DisplayStart`, Pixel bis `InWeek` + blockiert. n = 17 und 18 grün;
Sprung 0 → 600 grün; `Max_frames_per_update` = 18 grün. Gegen Base: n = 17/18 rot.

**Mutationen** (temporäre Kopie, `ProgramHost.Frame`; Filter wie oben plus `ProgramHostTests`, 14 Tests):

| Mutation | neue Tests | Probe 2 |
|---|---|---|
| M1 `Scene.NextFrame()` nur einmal nach der Schleife | 3 rot | rot |
| M2 `Menu.DrainEvents`/`Apply` erst nach der Schleife | 3 rot | rot |
| M3 `PhaseTick(frame, …)` statt `LastFrame + 1` (Kern springt) | 3 rot | rot |
| M4 `Machine.DrainWeekEvents()` entfernt | grün | grün |

M4 bleibt grün, weil das Leeren der Wochenereignisse keinen beobachtbaren Zustand ändert (nur Pufferwachstum); unverändert
gegenüber Base, kein Befund.

**Akzeptierte `[APPROX]`:** `HostGame.MaxStep` (0,25 s, unverändert, Begründung oben nachvollzogen). Keine neuen Marker im Diff.

**Annahmen / nicht ausgeführt:** Kein RefEmu-Pixelvergleich für Zwischenstände nach einem Sprung (der Test vergleicht den Host
gegen sich selbst bildweise; die bildweise Treue belegen die bestehenden Menü-Differenztests). `HostGame.Update` nicht
ausgeführt (MonoGame-Fenster), nur Codelesen. Keine `T.EXE`-Fundstelle betroffen (reiner Host-Takt).
