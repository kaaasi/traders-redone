# R6-Gesamtreview – Fortsetzung Teil 2

## 1. Merge-Empfehlung

**Ja** für den geprüften Head `ebaddee5578eb98e1aa6d76beada781804926fb3`, Base `e04742bca7899a8a69b46fa5a8a2ba0b147c357d`, über beide Teile: keine neuen belegten Fehler; die verlangten Gegenbeispiele und der Schnelllauf sind bestanden. Teil 1: [Zwischenstand](2026-11-16-r6-gesamt.md). Dies ist ein risikoorientiertes Gesamtreview, keine Vollinventur.

## 2. Befunde

Keine neuen P0–P3-Fehler nachgewiesen. **Testlücke:** Der Host erreicht in Woche 0 Aufruf 18; die Übernahme des Seitenzustands bei Aufruf 4 mit tatsächlicher Katastrophe und bei Aufruf 9 mit Ereignis ist im Host nicht durchgehend bildweise gegen das Original belegt. Die Mehrwochenprobe übernimmt hier den Originalzustand; sie ersetzt diesen Host-Nachweis nicht. Siehe [WeekScreens.cs](https://github.com/kaaasi/traders-redone/blob/ebaddee5578eb98e1aa6d76beada781804926fb3/src/Traders.Core/Phases/WeekScreens.cs#L64) und [Mehrwochentest](https://github.com/kaaasi/traders-redone/blob/ebaddee5578eb98e1aa6d76beada781804926fb3/tests/Traders.Tests/Phases/MultiWeekRound6DifferentialTests.cs#L118).

Erwartet offen bestätigt: #49, #75, #76, #157, #158, #162, #163, #165, #166 sind offen und P3; Aufruf 18 ist Haltepunkt, Tonausgabe bleibt bis R10 unvollständig. #147: Gegenprobe bereits in Teil 1 dokumentiert.

## 3. Ausgeführte Prüfungen und akzeptierte [APPROX]

Sauberer Checkout: `git rev-parse HEAD` = angegebener Head, `git status --porcelain` leer. `origin/main` danach `e70f8b3`; `git diff --stat ebaddee origin/main` enthält nur Teil 1. SDK 10.0.402, Originaldaten `/workspace/traders-referenz/original`.

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| Mit Originaldaten (`TR_REQUIRE_GAMEDATA=1`) | 1041 | 0 | 0 |

`TRADERS_GAMEDATA=/workspace/traders-referenz/original NUGET_HTTP_CACHE_PATH=/tmp/r6-nuget-cache tools/pruefen.sh --schnell`: **grün**; Original-Hashes, Fetch-Selbsttest, Architektur, Abdeckung, Restore und Release-Build ohne gemeldeten Fehler. Marker: 521 `[ORIGINAL]`, 40 `[APPROX]`.

- R6S-1: `dotnet test tests/Traders.Tests -c Release --no-build --filter 'FullyQualifiedName~Step_handler_call_11' --logger 'console;verbosity=detailed'` mit Originaldaten: **1 bestanden**, Woche 0 **1268 Aufnahmen**, einschließlich einer Aufnahme nach Rückkehr; also **1267 Rückläufe/Takte**. Der Harness vergleicht die Takte mit den Original-Rückläufen und hält bei Aufruf 12.
- R6E-1: `dotnet test tests/Traders.Tests --no-build --filter 'FullyQualifiedName~WeekScreensIncidentsDifferentialTests'` mit Originaldaten: **1 bestanden**. In separater Kopie `WeekScreens.cs` bei `TextBufferMemory` den Puffer durch `() => (byte[]?)inputs.TextBuffer()?.Clone()` ersetzt und denselben Filter ausgeführt: **1 rot**, Textpuffer `+03B6` Original `20`, Port `5F`. Der Test erreicht Woche 2 und verlangt einen tatsächlichen Schreibzugriff des Originals.
- R6P-2: `dotnet test tests/Traders.Tests -c Release --filter 'FullyQualifiedName~MultiWeekRound6DifferentialTests' --logger 'console;verbosity=detailed'` mit Originaldaten: **1 bestanden**, Woche 4 `0112` und Art 0, im Port 1402 Rückläufe; Mensch und Computer beteiligt, DS und RNG im Gleichschritt geprüft. Aufrufe ab 18 liefert das Orakel.
- Risikoprüfung: `111D:0112` und `1B03:1178` koppeln Farbziehungen an Pixel; Listen werden zum jeweiligen Ausführungszeitpunkt angewandt. `RulePageDifferentialTests` vergleicht Lesewerte und Seiten mit RefEmu-VRAM; der vollständige Schnelllauf umfasst diese Tests. `WeekScreens` erzeugt die Handler pro Aufruf neu; `PhaseMachine.Advance` verarbeitet fortlaufende Bilder einmal, Host-Szenen laufen vor `Advance`. Der Mehrwochentest prüft 4/9/11/16 bildweise, mit obiger Host-Lücke.
- `22A3:062E` in `analyse/disasm/22A3.asm` gegen `Harvest.Collect`: Wortaddition, danach vorzeichenbehafteter Vergleich mit 999; Differenztest `HarvestDifferentialTests.Collection_step_matches_the_original` im Schnelllauf. `17CB`-Geldbuchungen und Vorzeichen-/Breitenbehandlung gegen Code und Differenztests geprüft; keine Abweichung nachgewiesen.

**Akzeptierte `[APPROX]`:** `MirrorPages` übernimmt `DS:082E/0830`, `40DE…40E5`, `4516` aus der Ausgabe bei 4/9/11; Ernte liest sie nicht. Die genannte Host-Testlücke bleibt. `RulePage.Get` liefert außerhalb beider Seiten 0: für RefEmu und die geprüften Leser akzeptiert, keine Aussage über beliebige Hardwarewerte. **Annahmen:** Ausgabe und Kern erhalten den Seitenzustand zum Aufrufzeitpunkt synchron; reale Hardware außerhalb der Seiten kann abweichen.
