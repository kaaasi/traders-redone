# Review R4 Spur spielfeld – PR #95 (Erstreview)

- **PR:** #95 · **Base:** `5920e64` · **Head:** `df39405e5da19995e5fd555fc59707da1b850660`
- **Reviewer:** Claude Opus (geplanter Lauf, frische Sitzung ohne Implementierungskontext)
- **Stand:** Checkout `git rev-parse HEAD` = Head, `git status --porcelain` leer; Originaldaten aus `kaaasi/traders-referenz`
  (`7fcfb24`, Hashes geprüft). Disassemblierung lokal mit `tools/segmap.py`.

## 1. Merge-Empfehlung

**Ja** für genau `df39405`. Alle drei Läufe von `tools/pruefen.sh` grün, CI (linux) auf dem Head grün. Die vier
Gegenbeispiele (a)–(d) werden rot; von 35 Mutationen insgesamt 30 rot, davon sind c3/c6/m6/m8 pixel- bzw. verhaltensgleich,
d3 (Szene ruft schreibende Kern-Routine) ist P3-1/P3-2 (siehe 3).
Das RefEmu-Gerätemodell ist rein additiv und standardmäßig aus; geänderte Bestandstests: keine. Die Erwartungen kommen in allen
neuen Differenztests aus dem RefEmu (`_p.Call` bzw. `RunOriginal`), nicht aus dem Port. Stichproben der Cursorlogik gegen
`T.EXE` (`047B` mit Sprungtabelle `0645`, `0BEA`, `1B72`, `215E`) stimmen mit Befund und Port überein. Offen bleiben nur P3.

## 2. Befunde

Keine P0–P2.

- **P3-1 Prüflücke Architektur: Szene kann Kern-Routinen mit Schreibwirkung aufrufen.** `tools/check-architecture.py`
  ([Z. 31–37](https://github.com/kaaasi/traders-redone/blob/df39405e5da19995e5fd555fc59707da1b850660/tools/check-architecture.py#L31-L37))
  sucht nur `.Set(`/`.Bytes`/Phasenmaschine. Seit diesem PR gibt es öffentliche, schreibende Kern-Routinen auf `Dgroup`
  (`FieldCursor.HideCursor/Step/…`, `FieldGrid.Set`, `ControlReader.*`). Beleg: Zeile
  `Traders.Core.Field.FieldCursor.HideCursor(_state);` in `FieldScene.Emblem` eingefügt →
  `python3 tools/check-architecture.py` „Architekturgrenzen geprüft“, rc = 0. Die Szene selbst liest heute nur
  (Codelesen: ausschließlich `U8/U16/S8` auf `_state`). Vorschlag: Aufrufe von `FieldCursor.`/`FieldGrid.Set`/`ControlReader.`
  aus `src/Traders.Presentation` mitprüfen.
- **P3-2 Lese-Prüfung der Szenentests ist für `DS:330C` blind.** Dieselbe Mutation (Szene ruft `HideCursor`) lässt
  `FieldSceneDifferentialTests` grün (5/5), obwohl `AssertReadOnly`
  ([Z. 22](https://github.com/kaaasi/traders-redone/blob/df39405e5da19995e5fd555fc59707da1b850660/tests/Traders.Tests/Field/FieldSceneDifferentialTests.cs#L22))
  das Kern-DS vergleicht – vermutlich ist das Sichtbar-Byte im Wochenzustand schon 0 (nicht separat geprüft). Vorschlag: vor dem
  Szenenaufruf die gelesenen und angrenzenden Kern-Bytes mit Mustern ≠ 0 belegen.
- **P3-3 Szenentests vergleichen nur die Ebenen, nicht die Befehlsliste.** `AssertSame`
  ([Z. 78](https://github.com/kaaasi/traders-redone/blob/df39405e5da19995e5fd555fc59707da1b850660/tests/Traders.Tests/Field/FieldSceneHarness.cs#L78))
  prüft VRAM; ein zusätzlicher (deckungsgleicher) Kantenbefehl in `Frame` bleibt grün (Mutation c3). Pixelgleich, also kein
  Bildfehler; Listenlänge/-inhalt (DS der Grafikschicht gegen Original-DS `0A96`-Liste) wäre ein schärferes Kriterium.
- **P3-4 Zwei Besitzer der Objektsätze `DS:330C`.** `FieldCursor.HideCursor`
  ([Z. 181](https://github.com/kaaasi/traders-redone/blob/df39405e5da19995e5fd555fc59707da1b850660/src/Traders.Core/Field/FieldCursor.cs#L181))
  portiert `21AF:045E` ein zweites Mal (neben `Objects.HideObject` im DS-Abbild der Grafikschicht), `Idle` schreibt `3324`
  im Kern-DS. Ohne Spiegelung erreicht weder Ausblenden noch Blinken den Bildschirm; der PR nennt das als Restlücke
  („Spiegeln gehört zur Szene der Spielfeldschleife“). Für `0609:03E0` festhalten, welche Bytes (+0, +18h je Satz) die Szene
  in die Objektschicht überträgt, ohne die von `21AF:000D` gesetzten Zeiger zu überschreiben.
- **P3-5 Restlücke `135D:000E` nicht in `docs/arbeitspakete.md`.** Dort (Z. 126) stehen `0C73` und `03E0` als offen, der
  Computerzug nur im PR-Text (Merge-Prüfung §6 Nr. 6). Außerdem Merge- statt Rebase-Abgleich mit `main` (AGENTS §6), im PR
  begründet; Leitstelle entscheidet.

**Erwartet offen – bestätigt:** `0609:0C73` DrawSelection (`[APPROX]`, Ereignis wird in `FieldScene.Apply` verworfen),
`135D:000E` (nur Rückruf `IFieldOutput.ComputerTurn`), `0609:03E0` (keine Fundstelle in `src/`; `grep -rn "0C73\|03E0\|135D" src`).

## 3. Ausgeführte Prüfungen

`tools/claude/fetch-reference.sh original`, `tools/claude/setup-workspace.sh`, `tools/pruefen.sh` (ohne Option) auf `df39405`:

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| Mit Originaldaten (TR_REQUIRE_GAMEDATA=1) | 709 | 0 | 0 |
| Ohne Originaldaten (TR_SKIP_GAMEDATA=1) | 416 | 0 | 194 |
| Pflichtlauf ohne Daten (muss scheitern) | 416 | 293 | 0 |

Hashes, Architektur, Abdeckung, Restore `--locked-mode`, Build grün; Marker 232 `[ORIGINAL]` / 15 `[APPROX]`. CI-Check-Runs des
Heads: `linux` success (übrige skipped).

**Mutationen** (je eine in der Arbeitskopie, `dotnet test --filter <Klasse>`, mit Daten, danach `git checkout`):

| ID | Mutation | Ergebnis |
|---|---|---|
| a1–a7 | `0035`: Cursortasten re/li, Buchstaben hoch/runter, Joystick y, Maus y, LPT re/li, Joystick-Gleichstand `>`→`>=`, Stick 1/2 | alle rot (3–5 Tests) |
| b1–b6 | `10D2`: ohne `4BA8+`, ohne `4BA8−`, ohne `2EE7+`, `2EE7−` Grenze 3→2, ohne `2F05+`; `10A7` Rand `>=`→`>` | alle rot |
| c1, c2, c4, c5, c7 | `1ED9` Ziel x+1, `1256` Ecke y+1, `1879` Quelle y+1, Abzeichen x+1, `1A61` y+1 | alle rot |
| c3 / c6 | `1256` zusätzliche deckungsgleiche Kante / Reihenfolge zweier nicht überlappender Ecken | grün (pixelgleich, P3-3) |
| d1, d2 | Szene `_state.Set(0x37AA, …)` / `_state.Bytes[…] = 0` | `check-architecture.py` rot |
| d3 | Szene ruft `FieldCursor.HideCursor(_state)` | Prüfung und Tests grün (P3-1, P3-2) |
| m1–m5 | `215E` Blinkstart 13h→12h, `047B` runter Bild 1→0Bh, runter HQ-Feld (x,y) statt (x,y+1), `0BEA` Tonargumente vertauscht, `0F0D` ohne oberen Nachbarn | alle rot |
| m6 | `047B` Spieler nach Computerzug nicht neu lesen | grün – gleichwertig, der Stub ändert `37AA` nicht |
| m7, m9–m11 | `0268` Knopf 2 aus Bit von Knopf 1, `037A` Vorrang hoch vor Feuer, `0264` Computer-Bedingung weg, `03EB` 100 000→99 999 | alle rot |
| m8 | `07E0` Bedingung `ax ≠ 0` weg | grün – äquivalent (ax = 0 ⇒ keine Richtung) |
| m12 | RefEmu-Achsmodell `n < axes`→`n <=` | rot (Harness-Modell von `01D5` weicht ab) |

**T.EXE gelesen:** `0609:047B` (Sprungtabelle `0645`: 0→`0529` rechts, 1→`04E5` links, 2→`0574`, 3→`05CA`; Feldindex aus
altem x/y, `[bx+si+2EB2]` für runter), `0BEA`, `1B72` (`idiv` vorzeichenbehaftet, `cmp byte` auf Schritt), `215E`
(`random(200)` über `21FD:01BA`). Übereinstimmend mit `docs/routines/0609_047B_cursor.md` und dem Port.

**RefEmu-Gerätemodell:** `PcMachine.Devices.cs` greift nur bei gesetztem `AxisReads`/`ParallelStatus`/`Mouse`; sonst fällt
`In8/Out8` in die alte Weiche, `int 33h` liefert weiter AX = 0. `git diff --diff-filter=M` zeigt unter `tests/` keine Datei.

**Akzeptierte `[APPROX]`:** Joystick-Schritt 0 (Original hängt), `DrawSelection` verworfen. **Annahmen (nicht geprüft):**
Ursache von P3-2 (Wert im Wochenzustand); Verhalten echter Gameports jenseits des Zählmodells (Host-Sache, `01D5` nicht portiert).
