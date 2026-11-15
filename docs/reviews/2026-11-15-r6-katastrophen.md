# Review r6-katastrophen – PR #153 (Erstreview)

Head `fef2ec5dace3b5be46a3238d4998bef15428510f`, Base `94f63902557f4714e12d9a0ec8988bb6849c5d6d`. Reviewer: Claude Opus 5.5
(geplanter Lauf, eigene Sitzung; nicht an #153 beteiligt – die Umsetzung lief in `session_01GhACvVLS4j5d6KgJpmGCD8`).
Checkout nachgewiesen: `git rev-parse HEAD` = Head, `git status --porcelain` leer; Originaldaten aus `kaaasi/traders-referenz`
(`7fcfb24`, Hashes von `tools/pruefen.sh` geprüft).

## 1. Merge-Empfehlung

**Ja** für genau `fef2ec5`. Alle drei Läufe von `tools/pruefen.sh` grün, CI `linux` auf dem Head grün. Die Abnahme der Spur 1
ist erfüllt: jede Art und „keine“ laufen im Gleichschritt mit dem RefEmu (Datensegment, `DS:10D2`, Ziehungen je Rücklauf), die
Szene ist je Rücklauf pixelgenau (VRAM, DAC, CRTC, Befehlsliste); die drei geforderten Gegenproben (Art vertauscht, Ziehung der
Vorführung ausgelassen, falsches Feld) werden rot. Disassemblierung von `0009`, `03B7`, `0519` und `21FD:01BA` stimmt mit Befund
und Port überein. Verträglich mit #151 (Probe-Merge: nur `STATUS.md` im erzeugten Block in Konflikt, 991/991 Tests grün). Nur
P3-Befunde.

## 2. Befunde

**R6K-1 (P3) – #155/#156 schließen nicht automatisch.** Die PR-Beschreibung nennt #155/#156 nur als „mitgenommen“, ohne
`Fixes #155`/`Fixes #156`; GitHub meldet für beide `closed_by_pull_requests.total_count = 0`. Inhaltlich erledigt (siehe
unten). Vorschlag: Leitstelle schließt beide beim Merge per Hand oder ergänzt die Zeile.

**R6K-2 (P3) – `Every_branch_of_0009_is_covered` prüft den Zweig `random(10) = 0` nicht selbst**
([CatastropheDifferentialTests.cs Z. 151–176](https://github.com/kaaasi/traders-redone/blob/fef2ec5dace3b5be46a3238d4998bef15428510f/tests/Traders.Tests/Disasters/CatastropheDifferentialTests.cs#L151-L176)).
Der Befund sagt „Treffer über Woche 15/30 und über `random(10)` = 0“; die Variable `tenth` wird nur ausgegeben, assertiert
werden nur die Arten und „Woche Vielfaches von 15“. Tatsächlich abgedeckt ist der Zweig durch
`Catastrophe_with_four_computers_runs_in_lockstep` (Wochen 3–5, kein Vielfaches von 15, Art aus dem Original; Beleg: M1 dort
rot in Woche 3/4/5). Vorschlag: Assert ergänzen oder den Befundsatz anpassen.

**R6K-3 (P3, Verdacht) – `RandomFor` sucht unbegrenzt**
([CatastropheFrameHarness.cs Z. 183–194](https://github.com/kaaasi/traders-redone/blob/fef2ec5dace3b5be46a3238d4998bef15428510f/tests/Traders.Tests/Disasters/CatastropheFrameHarness.cs#L183-L194)).
Der Startzustand wird mit dem Port-`Choose` gesucht (`for (uint s = 1; ; s++)`). Das ist zulässig (der Erwartungswert kommt aus
dem Original, `Assert.Equal(kind, r.Kind)`), aber ein Fehler in `Choose`, der eine Art unerreichbar macht, lässt den Test hängen
statt rot werden. Nicht ausgeführt (Mutation `random(10) != 99` nach Start abgebrochen). Vorschlag: Schleife begrenzen.

### Gegenbeispiele und erwartet Offenes

- **R6P-2 erneut ausgeführt – bestätigt.** `111D:001E–003C`: `random(10)`, `or ax,ax / jz 003C`; sonst `idiv` durch 15,
  Rest ≠ 0 → Ende; `003C`: `si = random(2)`, `00E0–0103`: 0 → `0264`, 1 → `03B7`, 2 → `0519`. `21FD:01BA` teilt durch
  `[bp+6] + 1` (`0202–0216`, LUMOD) → 0 … n, wie `OriginalRandom.Next` (`divisor = n + 1`). `0519` ist erreichbar: RefEmu-Lauf
  `Catastrophe_with_four_computers_runs_in_lockstep(week: 3, kind: 2)` und `(4, 2)` grün, die Art liest der Test am Eintritt
  `111D:0519` des Originals. Die Tests der Spur erzwingen jede Art (Assert auf die Art), können also nicht leer bestehen.
- **#155 erledigt:** `public sealed partial class RulePage` (`RulePage.cs` Z. 14).
- **#156 erledigt:** neue Fälle `(0, 820)`, `(17, 1024)`, `(300, 1023)`, `(9, −820)` gegen `23CD:06DE`; Mutation M11 (Offset
  ohne 16-Bit-Umlauf) → rot genau an „page 0 (0, 820): original 6, rule page 0“.
- **Ton `21A7:0054`** nur als Ereignis `Sound`; `DisasterScene` ignoriert ihn (`case E.Sound: break;`) – offen bis R10, bestätigt.
- **Einbindung** in Phasenmaschine/Host: außerhalb von `Disasters/` keine Verwendung von `CatastropheStep`/`DisasterScene`
  (`grep -rn` in `src/`) – offen für Spur 0 Teil 3, bestätigt.

### Besonders geprüft

- **Kern/Szene:** `DisasterScene` schreibt nur in die DS-Kopie der Ausgabeschichten (`_gfx.Ds`, Tastentabelle), nie in den
  Kern-`Dgroup`, zieht keinen Zufall (`grep` ohne Treffer für `Random`/`.Set(`); Architekturprüfung grün. Der Kern liefert Ereignisse
  und Wartebilder, die Szene wartet ihre Rückläufe in den Bildern des Kerns.
- **`Pixels/**` vs. #151:** #153 ändert nur `RulePage.cs` (`partial`, `Tiles`, `DrawOnShownPage`) und neu `FieldBackdrop.cs`;
  #151 fügt `StationsPage.cs` hinzu und nutzt nur `Flip`, `Blit`, `SetPixel`, `Get`, Konstanten. Probe-Merge `fef2ec5` + `dede8b0`
  ohne Codekonflikt, `TR_REQUIRE_GAMEDATA=1 dotnet test`: 991 bestanden, 0 fehlgeschlagen.
- Fundstellen gelesen: `111D:0009–010F`, `0264` (Feldschleife `02F5–03A6`, HQ → 3, Rakete mit Güte 3 → `random(2)`), `03B7`
  (Farben `di` 0…5 mit `+ di + 1`, rückwärts `+ di`, `random(15) + 10`, `random(Spalten − 1)`/`random(Zeilen − 1)`, HQ übersprungen,
  `random(2)` vor dem Lesen des Besitzers), `0519` ab `06C2` (`37AA`, `random(19)`, `11 − 3·Rang` mit `jng`, `−2`): wie im Port.

## 3. Ausgeführte Prüfungen

`tools/pruefen.sh` (alle drei Läufe) auf `fef2ec5`, Ergebnis grün; Hash, Architektur, Abdeckung, Restore `--locked-mode`, Build
ohne Warnungen:

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| Mit Originaldaten (TR_REQUIRE_GAMEDATA=1) | 955 | 0 | 0 |
| Ohne Originaldaten (TR_SKIP_GAMEDATA=1) | 456 | 0 | 259 |
| Pflichtlauf ohne Daten (muss scheitern) | 456 | 499 | 0 |

Probe-Merge mit #151 (`dede8b0`): 991 / 0 / 0 mit Originaldaten.

**Mutationen** (Kopie per `git worktree`, je `dotnet test --filter …` mit Originaldaten, danach zurückgesetzt):

| ID | Mutation | Ergebnis |
|---|---|---|
| M1 | Art 0 ↔ 1 vertauscht (`Run`) | rot: 8/24 `Catastrophe_with_star_show…`, 4 Gleichschritt-Fälle (Woche 3/4/5, Art 0/1) |
| M2 | Vorführung: letzte der 90 Ziehungen ausgelassen | rot: 11 Fälle `StarShowDifferentialTests` |
| M3 | `03B7`: Feld (Zeile, Spalte) statt (Spalte, Zeile) | rot: 3 Fälle `Catastrophe_with_star_show…` |
| M4 | Woche mod 16 statt 15 | rot: 8 Fälle |
| M5 | `07E2`: Höhe `0x2F − x/5 − si` (Szene) | rot: Bildtest Art 2 (Computer und Mensch) |
| M6 | Meldefenster `y + 0x52` statt `0x51` (nur Szene betroffen) | rot: alle 4 Bildtests |
| M8 | `Tiles`: Spaltengrenze `w − 1` | grün – äquivalente Mutation (keine Breite ist Vielfaches + 1), verworfen |
| M8b | `Tiles`: letzte Spalte 1 Pixel schmaler | rot: 11 Fälle `StarShowDifferentialTests` |
| M10 | `0264`: Hauptquartier Güte 2 statt 3 | rot: 5 Fälle |
| M11 | `RulePage.Get` ohne 16-Bit-Umlauf | rot: `Pixel_reader_matches_for_every_word_argument` (neuer Fall) |

Differenztests gewinnen ihre Erwartung aus dem RefEmu: `CatastropheFrameHarness` vergleicht gegen den Original-Lauf aus einem
Abzug am Eintritt `111D:0009` (Partie mit vier Computern), Art am Eintritt der Original-Unterroutine gelesen; der Port-Zufall
wählt nur den Startwert von `DS:10D2`. `StarShowDifferentialTests`/`CatastropheDifferentialTests` rufen `111D:0009`/`0519` im
RefEmu (`Call`) und vergleichen Datensegment, `DS:10D2`, Ziehungen und Aufruffolge.

**Akzeptierte `[APPROX]`:** keine neuen; `DisasterScene.LoadPicture` wie `FieldScene` (bestehend). **Annahmen / nicht geprüft:**
Pause (P) mitten in der Routine (auch vom PR als ungeprüft genannt); Regelseite nach `0112` nicht weitergeführt (dokumentiert,
liest in `111D` niemand); Windows-Lauf nicht ausgeführt.
