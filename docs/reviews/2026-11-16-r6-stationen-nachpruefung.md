# Nachprüfung R6 Spur 3 `stationen` – PR #151
Reviewer: Claude Opus 5.5 (geplanter Lauf, eigene Sitzung, nicht an #151 beteiligt) · Nachprüfung von
[`2026-11-15-r6-stationen.md`](2026-11-15-r6-stationen.md) · bisher geprüfter Head `3ceda46` · Base `eba85d5` (= `main`) · Head
`97446ecaf5e64faef0dffcf10c9bdddd1ce5aaa1` · Delta `3ceda46..97446ec` · Checkout verifiziert (`git rev-parse HEAD` = Head,
`git status --porcelain` leer). Die Originaldaten wurden geprüft (Hashes in `tools/pruefen.sh`), `T.EXE` über `fetch-reference.sh disasm`.

## 1. Merge-Empfehlung

**Ja** für `97446ec`. R6S-1 ist behoben. `StationPhase` hält jetzt bei jedem Rücklauf des Originals. `StationsStep` tickt
einmal je Bild (`Retraces`/`Polls`). Das `[APPROX]` „one tick per page flip“ ist entfernt. `Step_handler_call_11` vergleicht
die Taktzahl je Zustand und für Woche 0 mit den Rückläufen `0D73:00D3` des Originals, und eine Rückmutation um ±1 Rücklauf
wird rot. Der Rebase auf `eba85d5` ändert keinen Code. `tools/pruefen.sh --schnell` ist grün. Offen bleiben nur die erwarteten
Punkte für Spur 0.

## 2. Befunde

Es gibt keine neuen Befunde.

**R6S-1 (P1) – behoben.**
- **Gegenbeispiel erneut ausgeführt:** Die Standardpartie in Woche 0 läuft über `StationsStep` in der `PhaseMachine` und meldet
  `week 0: 1268` Bilder. Die Differenz von 1 ist belegt: `StationsSceneHarness.RunOriginal` nimmt nach `m.CallFar(…0009…)` noch
  ein `Take()` auf. Das ist das Schlussbild nach dem Rücksprung und kein Rücklauf.
  [`RunPort`](https://github.com/kaaasi/traders-redone/blob/97446ecaf5e64faef0dffcf10c9bdddd1ce5aaa1/tests/Traders.Tests/Stations/StationsSceneHarness.cs#L283-L284)
  bekommt deshalb `original.Captures.Count - 1`.
  [`RunStep`](https://github.com/kaaasi/traders-redone/blob/97446ecaf5e64faef0dffcf10c9bdddd1ce5aaa1/tests/Traders.Tests/Stations/StationsSceneHarness.cs#L262-L265)
  verlangt `frames == originalPictures` und `frames == Retraces + Polls`. Für Woche 0 sind das also 1267 Takte gegen 1267
  Rückläufe. Der Erwartungswert kommt nur aus dem RefEmu, nämlich aus dem Haken an `0D73:00D3`.
- **Rückmutation `21FD:0185` im Kern** (`StationPhase.cs:112`, `Planting.Retraces(retraces ± 1, 0)`, temporäre Kopie, Tests
  `Traders.Tests.Stations`):

  | Mutation | rot |
  |---|---|
  | `Delay` +1 Rücklauf | 1: `Step_handler_call_11` – „frames of the phase machine 1679, retrace waits of the original 1453“ |
  | `Delay` −1 Rücklauf | 3: `Step_handler_call_11`, `Scene_game_week_0`, `Scene_with_humans` |
  | Einblendung `0329` mit 4 statt 3 Rückläufen je Stufe | 1: `Step_handler_call_11` |

- **Taktzahl gegen `T.EXE`** (gelesen in `21FD.asm`, `0A69.asm`, `1B03.asm`):
  - `0185`: Die Schleife ruft `call 0D73:00D3` auf, solange der vorzeichenbehaftete 32-Bit-Zähler kleiner als `n` ist. Das
    entspricht `Planting.Retraces` mit dem Abbruch bei `high < 0`.
  - `037D`/`0329`: je acht Stufen mit `03EB` und danach `0185(4)` bzw. `0185(3)`. Das entspricht
    `Fade(_d, 4/3)` mit +1 auf VGA.
  - `060D`/`0665` rufen jeweils genau einmal `051F` auf. Der Port nutzt `Planting.Flip`.
  - `0A69:01AD`: Bei einem Nullzeiger (`or ax,[bp+0xc]`) passiert nichts. Sonst läuft je Zeichen `060D`, der Klick außer bei `20h`,
    `0245` und `0185(cwd wait)`, danach `0185(final)` bei `> 0`. Das entspricht `Typewriter`. `0A69:004D`/`011B` haben keine
    Wartestellen.
  - Zahl der Aufrufstellen in `1B03` gegen den Port: `0185` 6, `051F` 2, `060D` 5, `0665` 2, `0245` 2, `03EB`/`037D`/`0329`
    je 1, `0243`/`0288` je 1. Alle stimmen überein.
  - **Pause `0245`:** Das Original pollt dort nur aktiv `DS:3D60` und hat keinen Rücklauf. Der Port meldet je Abfrage ein
    `FieldWait.Poll` wie `Planting.Pause`. Das ist ein Host-Takt und kein Bild des Originals. `Polls` wird getrennt gezählt, und
    in den Tests wird P nie gedrückt, also ist `Polls` = 0.
- **Szene je Takt:** Ein Takt der Szene bringt nur die Ereignisse dieses Bildes. `StationScene.Present` wirft, wenn noch eine
  Warteroutine läuft (`StationScene.cs`, `Present`). Dazu passt `Assert.False(scene.Presenting)` am Ende. Die Mutation −1 wird
  genau dadurch auch in den Szenentests rot. `NextFrame` läuft in beiden Harness-Pfaden vor dem Takt des Kerns. Im Produktcode
  ist das nur als Vertrag dokumentiert (`StationsStep`, `StationScene.NextFrame`), weil der Handler noch nicht eingebunden ist
  (Spur 0).
- **Keine Kernzustände in der Szene:** Die Szene hält nur `_pending` als Darstellungszustand. Die Pausetaste liest sie aus der
  eigenen Kopie der Tastentabelle in `_gfx.Ds`, die `NextFrame` schreibt. `check-architecture.py` (mit der Leseregel) ist grün.

**Erwartet offen, bestätigt:**
- **R6S-2 (#170, P3):** unverändert. Die Standardpartie läuft im Gleichschritt nur mit Computern. Das wird in Spur 0 Teil 3
  nachgezogen.
- **`StationsStep` nicht eingebunden:** Die Codesuche `StationsStep` in `src/` findet nur die eigene Datei. Das gehört zu
  Spur 0.

**Hinweise ohne Befund:**
- Die Prüfung der Taktzahl steckt nur im Pfad `viaStep`. Die Szenentests ohne Step erkennen einen zusätzlichen leeren Takt des
  Kerns nicht (Mutation +1 bleibt dort grün). Abgedeckt ist das trotzdem, weil `Step_handler_call_11` rot wird.
- `Typewriter` mit `finalWait > 0` wird in `1B03` nie erreicht. Beide Aufrufstellen übergeben `−1` (`1B03:1107ff.`,
  `TypedLines`), und eine Mutation dieses Zweigs bleibt grün. Der Zweig ist trotzdem wie im Original portiert.

## 3. Ausgeführte Prüfungen

- `git range-diff 94f6390..3ceda46 eba85d5..97446ec~1`: Unterschiede gibt es nur in `STATUS.md` (erzeugter Block) und
  `docs/analyse/DATENLAYOUT.md`. Der Code ist identisch. Das Delta im Code ist allein `97446ec` (18 Dateien).
- `tools/pruefen.sh --schnell` auf `97446ec` (dotnet 10.0.402) ist grün: Hashes, Selbsttest, Architektur, Abdeckung, Restore
  `--locked-mode`, Build mit Warnungen als Fehler und Tests.

  | Lauf | bestanden | fehlgeschlagen | übersprungen |
  |---|---:|---:|---:|
  | Mit Originaldaten (`TR_REQUIRE_GAMEDATA=1`) | 1025 | 0 | 0 |

- `TR_REQUIRE_GAMEDATA=1 dotnet test tests/Traders.Tests -c Release --filter "FullyQualifiedName~Step_handler_call_11|FullyQualifiedName~Scene_game_week_0|FullyQualifiedName~StationsLockstepTests.Game_week" --logger "console;verbosity=detailed"`:
  5/5 grün. Ausgabe: `pictures 6564, week 0: 1268` und `week 0: pictures 1268`. `Game_week` meldet `frames 189/392/230`, das
  sind die Seitenwechsel aus dem DS-Gleichschritt.
- Mutationen (Tabelle oben) in einem temporären Worktree des Heads, jeweils mit 39 Tests `Traders.Tests.Stations`.
- Probe-Merge entfällt, denn die Base `eba85d5` ist der aktuelle `main` (`git ls-remote … HEAD`).

**Akzeptierte `[APPROX]`:** unverändert gegenüber dem Erstreview (`0632` Startindex, Tafeltexte `0A69` nicht auf der
Regelseite, `LoadPicture` wie `FieldScene`). Neue `[APPROX]` kommen nicht hinzu. Das zu R6S-1 ist entfernt.

**Annahmen und nicht ausgeführt:**
- Die Pause mit gedrückter P-Taste wird in Aufruf 11 nicht im Gleichschritt geprüft. Der Code ist gleich wie bei
  `Planting.Pause`.
- Keine Messung im Host, denn es gibt keine Einbindung.
- Nur der Lauf `--schnell` wurde ausgeführt, so wie es die Vorlage für eine Nachprüfung vorsieht.
