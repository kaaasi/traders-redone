# Review R6 Spur 3 `stationen` – PR #151 (Erstreview)
Reviewer: Claude Opus 5.5 (geplanter Lauf, eigene Sitzung, nicht an #151 beteiligt) · Base `94f6390` · Head
`3ceda46312470bb52e1bd608ca67695632571548` · Checkout verifiziert (`git rev-parse HEAD` = Head, `git status --porcelain` leer),
Originaldaten geprüft (Hashes in `tools/pruefen.sh`).

## 1. Merge-Empfehlung

**Nein** für `3ceda46`, wegen eines P1. Alle drei Läufe von `tools/pruefen.sh` sind grün, die CI `linux` auf dem Head auch.
Die Kernroutinen stimmen mit `T.EXE` überein. Alle Gegenproben der Abnahme und R6N-1 werden rot. Der Gleichschritt vergleicht
DS, `random`-Zahl, beide Seiten und jedes Lesen von `06DE` gegen den RefEmu. Offen bleibt der Takt des Handlers: `StationsStep`
tickt je Seitenwechsel und nicht je Bild (R6S-1). Damit fehlt der Bildzähler aus der Abnahme. Spur 0 kann Aufruf 11 deshalb
nicht „Bild für Bild“ einbinden. Beheben kann das nur Spur 3, denn `Stations/**` gehört ihr.

## 2. Befunde

**R6S-1 (P1) – Der Handler für Aufruf 11 tickt einmal je Seitenwechsel statt einmal je Bild. Bildzähler und Host-Takt fehlen.**
[`StationsStep.cs:28–30`](https://github.com/kaaasi/traders-redone/blob/3ceda46312470bb52e1bd608ca67695632571548/src/Traders.Core/Stations/StationsStep.cs#L28-L30)
(`[APPROX]` „one tick per page flip“), dazu
[`StationPhase.cs:91`](https://github.com/kaaasi/traders-redone/blob/3ceda46312470bb52e1bd608ca67695632571548/src/Traders.Core/Stations/StationPhase.cs#L91).
`Delay(n)` ist dort nur ein Ereignis und kein Halt der Koroutine. Dasselbe gilt für Blenden, Tipptexte und `060D`/`0665`.
- **Beleg:** Standardpartie Woche 0. `Game_week(0)` meldet `frames 189`, das sind die Halte von `StationPhase.Run` und damit die
  Ticks von `StationsStep`. `Scene_game_week_0` zählt im Original `pictures 1268`, das sind die Einträge in `0D73:00D3`. Befehl:
  `TR_REQUIRE_GAMEDATA=1 dotnet test tests/Traders.Tests -c Release --filter "FullyQualifiedName~StationsLockstepTests.Game_week|FullyQualifiedName~Scene_game_week_0" --logger "console;verbosity=detailed"`.
  Die Phasenmaschine verarbeitet also 189 Takte, wo das Original 1268 Bilder hat. `Pause 100` (`0705`), `Pause 50` (`10EA`)
  und die Blenden fallen jeweils in einen einzigen Takt. `StationsSceneHarness.cs:250` prüft nur `pass == step.Flips` und nicht
  die Bildzahl. Die Bildgleichheit je Rücklauf entsteht nur in den Testschichten (`Capturing : IRetraceWait`, Z. 130). Die
  Szene spielt einen ganzen Takt sofort ab.
- **Warum P1:** Erstens verlangt die Abnahme Spur 3 den Gleichschritt mit Bildzähler. Zweitens gilt die Architekturregel
  `AGENTS.md` §4: Der Host meldet jedes Bild mit `PhaseMachine.Advance`, und der Kern verarbeitet jedes Bild genau einmal.
  Drittens fordert die Abnahme Spur 0 Teil 3 für Aufruf 11 einen Vergleich „Bild für Bild“, und das geht ohne Änderung an
  `Stations/**` nicht. Im Host liefe der Flug mit einem Tick je Host-Bild etwa 6,7-mal zu schnell, und Pausen und Blenden wären
  unsichtbar. Die Handler der Spuren 1 und 2 ticken je Bild (`CatastropheStep` „one frame further“, Zähler `Retraces`;
  `WeekIncidentsStep` mit `MenuWait.Retrace`).
- **Vorschlag:** `StationPhase` hält bei jedem Rücklauf, also bei `21FD:0185(n)`, beim Rücklauf im Seitenwechsel, bei
  Blenden, Tipptexten und `060D`/`0665`, so wie `Planting.Retraces`/`Fade`. `StationsStep` zählt `Retraces`. Ein Test prüft die
  Taktzahl gegen die Bildzahl des Originals, je Zustand in `Step_handler_call_11`. Die Szene bekommt je Takt nur die Ereignisse
  dieses Bildes.
  Gegenprobe: ein Rücklauf zu viel oder zu wenig in `Delay` → rot.

**R6S-2 (P3) – Die Standardpartie läuft im Gleichschritt nur mit Computerspielern.** `Game_week` (Woche 0/1/2) und
`Scene_game_week_0` laufen mit vier Computern. Menschen kommen nur in den Zufallszuständen vor (`Whole_phase_0009`,
`Scene_with_humans`, `Step_handler_call_11`). `1B03` liest die Wochennummer nicht (Codesuche `Week` in `Core/Stations` ohne
Regelzugriff; `0009` liest nur `08D0`, `2EEA…2EF6`). Die Zufallszustände decken deshalb auch „spätere Woche mit Mensch“ ab. Die
echte Partie mit einem Menschen bringt die Mehrrundenprobe von Spur 0 Teil 3. Dort nachziehen, kein eigener Lauf.

**Bestätigt, kein Befund:**
- **R6N-1 erneut ausgeführt:** `1B03:1178` (`11CB` `call 23CD:06DE`, `11D2` `cmp ax,1`/`jnz 11F6`, `11DB` `random(2)`) ist
  in [`StationPhase.Pixels.cs:53–67`](https://github.com/kaaasi/traders-redone/blob/3ceda46312470bb52e1bd608ca67695632571548/src/Traders.Core/Stations/StationPhase.Pixels.cs#L53-L67)
  genau so umgesetzt, mit Liste leeren vor jedem Punkt und Ausführen danach. Die Lesestellen `08ED` (`09B1`) und `0BE7` (`0BFE`,
  Explosion `x − 8`/`y − 6`, `idiv 3`) stimmen ebenfalls. Gelesen wird über `StationsPage.Read` → `RulePage.Get`. Der Punkt
  geht als Listeneintrag erst bei `RunList` auf die Seite, so wie `0A96:06A8`. `Star_field_1178` zählt 200 Lesungen je Zustand
  und Farbe 1 in 100…1500 von 1600. Damit ist der Pixel‑1‑Zweig belegt, den die Plan-Nachprüfung offen ließ. Mutationen M5
  (`random(2)` immer gezogen) und M6 (Stern nicht auf die Regelseite) → rot.
- **Gegenproben der Abnahme** (Tabelle unten): Station vertauscht, Kollision ±1 in `0BE7` und in `08ED`, Ziehung ausgelassen,
  Ertrag doppelt → alle rot.
- **Grenzfälle:**
  - Kein Geld: `Fill` setzt in 30 % der Zustände negatives oder kleines Bargeld, Bonus `cash < 1000`.
  - Rakete getroffen: `Whole_phase_0009` und `Scene_with_humans` verlangen `lost > 0`.
  - Spieler ohne Raketenfeld: 20 %.
  - Zeitablauf: `02C0` hat keine Zeitgrenze. Die Schleife endet nur über den Zähler `[bp-1]` (`1B03:0401–0407`). Der Flug endet,
    wenn der Fortschritt `> 59h` ist. Beides ist im Gleichschritt abgedeckt.
- **Dateigrenzen:** Aus `Pixels/**` ist nur `Pixels/StationsPage.cs` neu (`git diff --stat 94f6390..3ceda46 -- src/Traders.Core/Pixels`).
  `RulePage` wird nur über die öffentliche Schnittstelle genutzt. Gemeinsame Dateien: nur der erzeugte Block in `STATUS.md`,
  der eigene Abschnitt in `DATENLAYOUT.md` und der eigene Status in `arbeitspakete.md`.
- **Erwartet offen bestätigt:** Ton `21A7:0054` gibt es nur als `StationCall.Sound`; die Szene reicht ihn an den optionalen
  `Sound` weiter (`StationScene.cs:119`), offen bis R10. `StationsStep` ist nirgends eingebunden (Codesuche `StationsStep` in
  `src/` nur in der eigenen Datei), offen für Spur 0 Teil 3.
- **Probe-Merge:** `3ceda46` + `main` (`adf0606`, mit #146) + #153 (`e3df8ff`). Konflikte gibt es nur im erzeugten Block von
  `STATUS.md` (gelöst mit `abdeckung.py --write`) und in `DATENLAYOUT.md` (zwei Abschnitte nebeneinander). Code ist konfliktfrei.
  #153 ändert `RulePage.cs` nur additiv (`partial`, `DrawOnShownPage`, `Tiles`). Auf dem Ergebnis sind `check-architecture.py`
  (mit der neuen Leseregel aus `main`) und `abdeckung.py --check` grün. Build mit Warnungen als Fehler grün, Tests
  `Stations|Pixels|Disasters` mit Originaldaten 108/108 grün. Ein Rebase auf `main` ist nötig, aber nur für die beiden Doku-Konflikte.

## 3. Ausgeführte Prüfungen

`tools/pruefen.sh` (alle drei Läufe, Head `3ceda46`, dotnet 10.0.402): Hashes, Architektur, Abdeckung, Restore `--locked-mode`,
Build und Tests grün.

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| Mit Originaldaten (`TR_REQUIRE_GAMEDATA=1`) | 928 | 0 | 0 |
| Ohne Originaldaten (`TR_SKIP_GAMEDATA=1`) | 456 | 0 | 282 |
| Pflichtlauf ohne Daten (muss scheitern) | 456 | 472 | 0 |

Gegenproben in einer temporären Kopie des Heads mit
`TR_REQUIRE_GAMEDATA=1 dotnet test … --filter FullyQualifiedName~Traders.Tests.Stations` (39 Tests):

| Mutation | rot |
|---|---|
| M1 `04D4`: Station in `DS:450E` des anderen Spielers (`p ^ 1`) | 10 (u. a. `Take_station_04D4`, `Choice_loop_02C0`, `Whole_phase_0009`, Szene) |
| M2a `0BE7`: Treffer-Pixel bei `x + 1` | 3 (`Game_week`, `Whole_phase_0009`, `Step_handler_call_11`) |
| M2b `08ED`: Flug-Pixel bei `x + 1` | 7 (Gleichschritt und Szene) |
| M3 `1178`: Ziehung `random(2)` ausgelassen | 8 (u. a. `Star_field_1178`) |
| M4 `0EA9`: Bargeldbonus doppelt gebucht | 2 (`Bonuses_0EA9`, `Whole_phase_0009`) |
| M5 R6N-1: `random(2)` unabhängig vom Pixel | 8 |
| M6 Sternpunkt nicht auf die Regelseite | 8 |

`T.EXE`: `analyse/disasm/1B03.asm` gelesen, `1178–1240`, `0BE7–0C53`, Lesestelle `09B1` in `08ED` und die Ausstiege von `02C0`.
Die Differenztests gewinnen ihre Erwartung aus dem RefEmu (`StationsLockstep.RunOriginal`: Haken an `06DE`, `01BA`, `051F`;
der Port läuft auf einer Kopie des Original-DS). CI `linux` auf dem Head: success.

**Akzeptierte `[APPROX]`:**
- `0632`: uninitialisierter Startindex.
- Tafeltexte `0A69` nicht auf der Regelseite: Lesen im Feld nur „= 1“ verglichen, die Szene ist dort pixelgenau.
- `StationScene.LoadPicture` wie `FieldScene`.

**Nicht akzeptiert:** `StationsStep` „ein Takt je Seitenwechsel“ (R6S-1).

**Annahmen und nicht ausgeführt:**
- Kein `pruefen.sh --schnell` auf dem Probe-Merge (Merge-Prüfung §6 Nr. 2).
- Keine Messung im Host.
- Die Überschlagszahl 6,7 ist 1268/189. Die Pausen dazwischen unterscheiden sich je Zustand.
