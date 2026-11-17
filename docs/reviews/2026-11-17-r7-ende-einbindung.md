# Review: Spur `ende` Teil 3 – Einbindung Aufrufe 23/24, Mehrrundenprobe (PR #206)

Erstreview (Claude Opus, eigene Sitzung ohne Implementierungskontext). Base `3d79195e1423d02be9ee871dc95b75dcd799ee73`, Head
`5568e934e7444fdc6a889b1d178d8372422546a0`; Checkout verifiziert (`git rev-parse HEAD` = Head, `git status --porcelain` leer),
Originaldaten aus `kaaasi/traders-referenz` (`fetch-reference.sh original`, Hashes grün).

## 1. Merge-Empfehlung

**Ja, für genau diesen Head.** Alle drei Läufe von `tools/pruefen.sh` grün; CI `linux` auf dem Head grün. Die Mehrrundenprobe
(Wochen 0–4, 1 Mensch, 3 Computer) führt 3, 4, 6, 8, 9, 11, 16, 19, 21, 23, 24 im Port und 18, 26–30 aus dem RefEmu, vergleicht
Datensegment und `DS:10D2` an jeder Aufrufstelle, jedem Retrace (23/24) und jedem Rücksprung, Erwartungswerte nur aus dem RefEmu.
Alle verlangten Gegenproben werden rot. Eine zusätzliche Host-Probe Bild für Bild durch Handel **und** erzwungene Zusatzphase
(VRAM, DAC, CRTC-Start, `DS:10D2` je Retrace bis Aufruf 26) ist grün. Offen bleiben nur P3.

## 2. Befunde

Keine P0–P2.

- **R7EE-1 (P3) – Host Bild für Bild für 23/24 nicht eingecheckt.** Der Host-Test
  [`ProgramHostTests.cs` Z. 144](https://github.com/kaaasi/traders-redone/blob/5568e934e7444fdc6a889b1d178d8372422546a0/tests/Traders.Tests/App/ProgramHostTests.cs#L144)
  vergleicht nur am Aufrufort von 26; in Woche 0 des Hosts fällt die Zusatzphase nicht (Probe: `[24, 0]` Bilder), `ExtraScene`
  im Host ist damit von keinem eingecheckten Test gegen das Original gezeichnet. Beleg, dass es stimmt (Probe, nicht eingecheckt):
  Kopie von `HostPlantingFrameDifferentialTests` mit `skipRobotPhase: true`, im Original Aufruf 18 übersprungen (IP + 5), Ende an
  `1777:02B5`; mit `DS:57F0 = 2·DS:57F2 + 1` nach der Ernte auf beiden Seiten →
  `retraces 9383 … [16, 510], [23, 4436], [24, 1777]`, **grün** (ohne Erzwingen `[23, 4436]`, grün). Vorschlag: diese Probe als
  Test aufnehmen (≈ 21 s). Im PR als Restlücke genannt.
- **R7EE-2 (P3) – #194/#201 ohne `Fixes`.** Die PR-Beschreibung nennt #194 und #201, schließt sie aber nicht
  (`closed_by_pull_requests` leer); die Leitstelle muss beide beim Merge von Hand schließen. Ebenso veraltet (im PR genannt):
  `docs/routines/22A3_02D3_ernte.md` Z. 149 verweist auf die entfernte `MultiWeekRound6DifferentialTests.cs`.

Zu den Prüfpunkten:

1. **Erzwungene Zusatzphase.** Der Eingriff ([Z. 179](https://github.com/kaaasi/traders-redone/blob/5568e934e7444fdc6a889b1d178d8372422546a0/tests/Traders.Tests/Phases/MultiWeekRound7DifferentialTests.cs#L179))
   läuft im `atCallSite` von `FramedStep.Begin`: **nach** dem Vergleich an der Aufrufstelle (Port-Zustand bis dahin geprüft) und
   **vor** `TakeOver` (Kern übernimmt das RefEmu-Datensegment) → beide Seiten starten gleich. `DS:57F0` liest in `T.EXE` nur
   `1DEF:00DB` (`cmp ax,[0x57f0]` bei `1DEF:00F5`); geschrieben nur von `22A3` (`0061` Null, `00EF`/`0170` inc) – der Eingriff
   wirkt also nur auf diese eine Entscheidung und ist in Woche 3 durch die Ernte überschrieben. `random(20)` wird weiter in beiden
   gezogen und verglichen. Ergebnis der Probe: `extra: (0, 0), (1, 0), (2, 1862), (3, 0), (4, 0)`, `fell in weeks 2`. Für
   „nachweislich mindestens einmal“ reicht das: Assert auf ≥ 100 Bilder im Gleichschritt, Auslösung über den echten Zweig
   `2·57F2 < 57F0`. Nicht abgedeckt ist nur der Zweig `random(20) = 0` (in `ExtraDifferentialTests` belegt). Wortlaut der
   Abnahme („Woche bzw. Tastenmuster festlegen“) ist damit sinngemäß erfüllt; Erzwingen ist in PR und Status vermerkt.
2. **Szenen im Host.** `TradeScene`/`ExtraScene` bekommen Ereignisse während `Advance` (`Output<T>`), `NextFrame` vor `Advance`
   wie die übrigen Szenen; Szenen schreiben den Kern nicht (`check-architecture.py` grün, Datensegment am Aufrufort 26 gleich).
   Pixelgenau je Bild: siehe R7EE-1 (Probe grün, auch die Zusatzphase).
3. **Reihenfolge #201.** `Output<T>` ([`ProgramHost.cs` Z. 266](https://github.com/kaaasi/traders-redone/blob/5568e934e7444fdc6a889b1d178d8372422546a0/src/Traders.App/ProgramHost.cs#L266))
   spielt Bericht/Ereignisse vor dem ersten Ereignis des Folgeschritts; für 9 → 11 wirkt zusätzlich `OutputPages`
   ([Z. 86](https://github.com/kaaasi/traders-redone/blob/5568e934e7444fdc6a889b1d178d8372422546a0/src/Traders.App/ProgramHost.cs#L86)).
   Rückbau der Hülle (`PlayEarlier` in `Output<T>` aus) → Reihenfolgetest **rot**; die Pixeltests gegen das Original
   (`HostPlantingFrame…`, `HostLoadedWeekFrame…`, `ProgramHostTests.Host_*`) bleiben dabei grün – passt zu #201 („ohne sichtbare
   Wirkung“, Bericht nur Verdeckseite). Die Reihenfolge ist damit strukturell gesichert, nicht durch einen Originalvergleich;
   ohne sichtbare Wirkung genügt das.

## 3. Ausgeführte Prüfungen

`tools/pruefen.sh` (alle drei Läufe) auf dem Head:

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| Mit Originaldaten (TR_REQUIRE_GAMEDATA=1) | 1141 | 0 | 0 |
| Ohne Originaldaten (TR_SKIP_GAMEDATA=1) | 475 | 0 | 363 |
| Pflichtlauf ohne Daten (muss scheitern) | 475 | 666 | 0 |

dotnet 10.0.402; Hashes, Architektur, Abdeckung, Marker-Abgleich grün; `[ORIGINAL]` 620, `[APPROX]` 44. Mehrrundenprobe 1 min 49 s,
Ausgabe: Handel `(0, 4438) … (4, 5374)` Bilder, Mensch im Handel 2673 Aktionen, Katastrophe Woche 4 im Port (1402 Bilder).

Gegenproben (temporäre Kopie per `git worktree`, Schalter `PROBE_MUT`, nicht eingecheckt):

| Mutation | Test | Ergebnis |
|---|---|---|
| Handler 23 doppelt getaktet (`Tick` zweimal je Bild) | `MultiWeekRound7…` | rot: `week 0 … trade retrace 56: DS:2BE4 core 03 RefEmu 00` |
| Aufruf 24 ausgelassen (Handler endet in `Begin`) | `MultiWeekRound7…` | rot: `week 0 … extra phase retrace 0: random state core 41304ECA RefEmu 1CC75583` |
| Handler 23/24 vertauscht | `MultiWeekRound7…` | rot: `frame of 1DEF:000C not read` (Testgerüst bricht ab, bevor ein Vergleich läuft) |
| #194: `--original-roboter` lässt Schalter an | `HostArgumentsTests` | rot (4 von 6) |
| #201: `PlayEarlier` in `Output<T>` entfernt | `Report_and_incidents_are_played_before_the_next_step_in_the_same_frame` | rot: `frame 403: … DrawObjects 0 0 played after InitObjects()` |
| – (Host ohne Schalter hält an 18, mit an 26) | `ProgramHostTests.Host_plays_week_0_up_to_the_robot_phase…`, `…_up_to_Fat_Mike…` | grün |
| Host Bild für Bild bis 26, Zusatzphase erzwungen | Probe zu R7EE-1 | grün, `[24, 1777]` |

`T.EXE` (`tools/segmap.py`): `1777:02A1 call 0B9F:0002`, `02A6 call 1DEF:000C`, `02AB–02B4` Marke `21FD:0977(5)`,
`02B5 call 1F63:000F` – deckt `WeekLoop`-Einträge 23/24/26. `1DEF:000C` beginnt mit `push bp; mov bp,sp` → `BP = SP − 6` an der
Aufrufstelle wie im Test. `1DEF:00DB`: `random(20)`, dann `shl [57F2]`, `cmp [57F0]`, `jnl` (vorzeichenbehaftet) wie `ExtraPhase`.

**Erwartet offen, bestätigt:** Abend-/Endbilder bei Partieende weiter ausstehend (F1 = B, `EndCheckStep` ohne Rest);
`[APPROX] bewusste Abweichung` an `RobotPhaseSkip` unverändert; P3 #205 (R7H-6) offen – die Mehrrundenprobe vergleicht jetzt den
Handel mit `TradeComputer` in fünf Wochen Bild für Bild im Kern, das eingecheckte Bild für die Wochen 1/3 fehlt weiter.

**Annahmen (nicht ausgeführt):** Windows-Lauf; der Zweig `random(20) = 0` nur über die bestehenden Tests der Spur `zusatz`.
