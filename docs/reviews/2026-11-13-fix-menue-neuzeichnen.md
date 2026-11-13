# Review: Fix R4W-1 – Menü neu zeichnen im Host (PR #127)

Erstreview, Claude Opus 5.5 (geplanter Lauf, eigene Sitzung ohne Implementierungskontext) · Base `b80c07d` · Head
`23429a4a04a9ad1efaa11922f4ef2fbcb942ac7d` · geprüft im eigenen Checkout (`git rev-parse HEAD` = Head, `git status --porcelain` leer),
Originaldaten aus `kaaasi/traders-referenz` (`fetch-reference.sh original`, Hashes geprüft).

## 1. Merge-Empfehlung

**Ja, für genau Head `23429a4`.** `SetUpGamePages` gibt `0DFD:0074` (Seitenteil) und `0DFD:0132` (erster Flip) befehlsgenau
wieder; der neue Bildfolgetest vergleicht Pixel, CRTC-Start, `DS:082E/0830` und Seed je Retrace gegen den RefEmu und wird
ohne den Fix rot – auch dann, wenn man nur die Pixel vergleicht. Keine P0–P2. Merge gegen aktuellen `main` (`4160536`)
konfliktfrei (`git merge-tree`); da `main` seit der Base neu ist, gilt Merge-Prüfung §6 Nr. 2.

## 2. Befunde

- **P3 R4W-1-R1 – Routinenbefund nicht nachgezogen.** `docs/routines/0DFD_000D_programmstart.md` (Abschnitte
  „C#-Umsetzung“ und „Prüfung“, Satz „Nicht verglichen: die Anzeigebytes … erst mit einem Port von `0074`/`0132`…“) nennt
  weder `ProgramHost.SetUpGamePages` noch `HostMenuPictureDifferentialTests`, obwohl `STATUS.md` beide Adressen jetzt als ✅
  führt. Beleg: `grep -n "HostMenuPicture\|SetUpGamePages" docs/routines/0DFD_000D_programmstart.md` → keine Treffer. Der
  PR weist selbst darauf hin (außerhalb seiner Dateigrenzen) → Issue `P3`, beim nächsten Lauf mitnehmen.
- **P3 R4W-1-R2 – Hinweis, kein Fehler: Tastentakt nur auf zwei Retraces genau.** Die Tastenabbildung ist stimmig (beide
  Seiten setzen denselben Scancode aus `MenuKeys` – Original über `DgroupLayout.KeyState + sc`, Host über `held` –, gedrückt
  am Ende desselben ankerrelativen Retrace, Loslassen nach 3 Retraces bzw. an der Freigabeschleife). Gegenproben (nur
  Testseite des Hosts verschoben): Druck **1 Retrace später** → weiterhin grün; **1 früher** sowie **2 oder 3 später** → rot
  (`retrace 2: … start 0000/4000`). Ungerade Druckzeitpunkte (1/13, 81/101/131) auf beiden Seiten → grün. Der Test löst den
  Tastenzeitpunkt also nur auf ein Fenster von zwei Retraces auf (Menü fragt einmal je Durchlauf = Flip mit zwei Retraces
  ab); für R4W-1 genügt das. Der Satz „pressed at the same retrace“ ist richtig für die Eingabe, die Prüfung selbst
  unterscheidet aber r und r+1 nicht. Optional im Kopfkommentar erwähnen.

Gegenbeispiel **R4W-1** erneut ausgeführt: bestätigt behoben (Abschnitt 3).

## 3. Ausgeführte Prüfungen

**`tools/pruefen.sh` (alle drei Läufe) zum Head `23429a4`:** Ergebnis grün; Hashes, Architektur, Abdeckung, Restore
`--locked-mode`, Build mit Warnungen als Fehler bestanden.

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| Mit Originaldaten (TR_REQUIRE_GAMEDATA=1) | 757 | 0 | 0 |
| Ohne Originaldaten (TR_SKIP_GAMEDATA=1) | 439 | 0 | 209 |
| Pflichtlauf ohne Daten (muss scheitern) | 439 | 318 | 0 |

Markerbericht: Summe `[ORIGINAL]` 250, `[APPROX]` 29 (Traders.App 6/9). CI `build.yml` auf dem Head: `linux` success
(Run 43429517532), `windows`/`downloads` übersprungen.

**Gegenproben** (temporärer Worktree, je `TR_REQUIRE_GAMEDATA=1 dotnet test tests/Traders.Tests -c Release --filter
FullyQualifiedName~HostMenuPictureDifferentialTests`):

| Mutation | Ergebnis |
|---|---|
| R4W-1: Aufruf `SetUpGamePages(Layers)` im `ProgramHost`-Konstruktor entfernt | **rot** 3/3: `retrace 0: 0 pixels differ; start 0000/0000, DS:082E 1/0, DS:0830 0/1` |
| dasselbe, Assertion nur auf Pixel (`diff == 0`) | **rot** 3/3: `main-f1-f3: retrace 2: 125 pixels differ; start 4000/0000`; `level`/`settings`: `retrace 6: 64000 pixels differ` |
| nur die beiden `Set32(PageNumbers…)` entfernt (Flip bleibt) | **rot** 3/3: `retrace 1: … seed CD18219E/8CF1DCC7` (Retrace-Folge verschoben) |
| Host-Tastendruck +1 Retrace | grün (siehe R4W-1-R2) |
| Host-Tastendruck −1, +2, +3 Retraces | **rot** 3/3: `retrace 2: … start 0000/4000` |
| Druckzeitpunkte ungerade (beide Seiten) | grün |

**Fundstellen in `T.EXE`** (`python3 -I tools/segmap.py gamedata/dos/T.EXE /tmp/segmap`):
- `0DFD:000D` ruft unmittelbar nacheinander `0DFD:0074` (`0013`) und `0DFD:0132` (`0018`) – die Reihenfolge im Host
  (`InitializePages` → `SetUpGamePages`) passt; `0B67:0002` liegt davor.
- `0074` ab `00EA`: `[40DE]=1, [40E0]=0, [40E2]=2, [40E4]=0` (zwei Longs 1/2), dann `0B67:0238(page[082E], page[0830], −1)`
  mit Index `·4` aus `40DE`. `0B67:0238` liest nur `bp+6…bp+0C` (das −1 ist unbenutzt): zeigen bei show 1/2, Zeichenseite
  bei draw 1/2 (gleich → gezeigte, sonst verborgene Seite) – identisch zu `GraphicsLayers.SelectPages(draw, show)`; der
  Host übergibt `Page(Index(082E))`, `Page(Index(0830))` in derselben Reihenfolge.
- `0132`: `call 21FD:051F`, dann nur Zeigerkopien `44DE ← 4BC6`, `45BC ← 45D6` → `layers.FlipPages()` plus `[APPROX]`.
- Erwartungswerte des Tests stammen aus dem RefEmu-Lauf (`OriginalProgram.Start`, Abzug an `0D73:00D3`), nicht aus dem Port.

**Testlücke `MenuFrameDifferentialTests`:** Begründung nachvollzogen – `MenuSceneHarness.SetUpPort` baut die Ebenen aus dem
DS-Abbild des Originals am Menüeintritt (`MenuSceneHarness.cs:190`, `ds.Clone()` aus dem Snapshot), dort sind
`40DE…40E5` schon gesetzt; `ProgramHost` wird nur von `ProgramHostTests`, `ProgramHostCatchUpTests` und dem neuen Test
verwendet. Der neue Kopfkommentar trifft zu.

**Akzeptierte `[APPROX]`:**
- `SetUpGamePages`: Heap-Puffer von `0074` und Zeigerkopien von `0132` (`DS:44DE`, `45BC`) beim Host – reine Heapadressen,
  vom Original unabhängig, ohne Einfluss auf das Bild (Test grün).
- Retraces des ersten Flips werden in `Retraces` gezählt, belegen aber keine Anzeigebilder (Ladebild nicht gezeigt, F1 = B)
  – vertretbar, da der Test ab dem Anker vergleicht und der Uhrwert aus dem Original kommt.
- Erweiterter `[APPROX]`-Kopf in `ProgramHost` (Anzeigeteil von `0DFD:000D` reduziert) – unverändert in der Sache.

**Annahmen / nicht ausgeführt:** Windows-Start per Hand nicht geprüft; Merge-Ergebnis mit aktuellem `main` nicht gebaut
(nur konfliktfrei geprüft).
