# Review: Fixlauf `fix/stationen-host` (PR #199, R7E-1 / #189)

- **Geprüfter Stand:** Base `b083f9f3ccf969c1b49ffdc52767f24360fe7891`, Head `93b136a7ee31a27c52e28f3eabc171970fd56073`
  (`git rev-parse HEAD` = Head, `git status --porcelain` leer), Originaldaten aus `kaaasi/traders-referenz` (`7fcfb24`, Hashes geprüft).
- **Reviewer:** Claude (Opus 5.5), Erstreview als geplanter Lauf, eigene Sitzung ohne Implementierungskontext.

## 1. Merge-Empfehlung

**Ja, für genau Head `93b136a`.** Die Ursache von #189 ist im Host belegt und behoben: Gegenbeispiel R7E-1 ist rot an der
erwarteten Stelle, mit dem Fix grün. Die Reihenfolge entspricht dem Original (alle Ereignisse von Aufruf 9 vor dem ersten
Aufruf von `1B03:0009`). Alle drei Läufe von `tools/pruefen.sh` grün, CI `linux` auf dem Head grün. Die Befunde sind P3
und blockieren nicht. Gegen den aktuellen `main` (mit #187) gibt `git merge-tree` keinen Konflikt, und `main` ändert seit der
Base nichts unter `src/Traders.App`, `src/Traders.Core/Phases` und `tests/Traders.Tests/App`. Die Integration prüft die Leitstelle.

## 2. Befunde

Kein P0–P2.

- **P3 R7E-2 – derselbe Reihenfolgefehler beim Wochenbericht (Aufruf 3), ohne sichtbare Wirkung.** Die Ereignisse des Berichts
  spielt der Host weiterhin erst nach `Advance` ab
  ([ProgramHost.cs:202–203](https://github.com/kaaasi/traders-redone/blob/93b136a7ee31a27c52e28f3eabc171970fd56073/src/Traders.App/ProgramHost.cs#L202-L203)).
  Endet Aufruf 3 im selben Bild, in dem Aufruf 4 oder 8 beginnt, laufen dessen erste Ausgaben daher vor dem letzten
  Durchlauf der Warteschleife des Berichts. Im Original ist es umgekehrt: `0825:000A(1)` kehrt zurück, bevor `111D:0009` bzw.
  `0609:0004` starten. **Beleg (Probe):** In einer temporären Kopie habe ich `ProgramHost.Frame` so instrumentiert, dass jedes
  Bild protokolliert wird, in dem Bericht-, Menü- oder Ereignis-Ereignisse nach `Advance` gespielt werden, während derselbe
  `Advance` Ausgaben anderer Schritte lieferte. Lauf:
  `TR_REQUIRE_GAMEDATA=1 dotnet test tests/Traders.Tests -c Release --filter FullyQualifiedName~Traders.Tests.App`, 116/116 grün.
  Ergebnis:
  - `ProgramHostTests.PlayWeek0` Bild 403 und `HostPlantingFrameDifferentialTests` Bild 327, Aufruf 3→8: Feld-Ereignisse
    `InitObjects`, `LoadImage(0)`, `ResetObjectRestore` (insgesamt 47) werden **vor** den Bericht-Ereignissen
    `ListExecute`, `ListExecute`, `DrawObjects` gespielt.
  - `HostLoadedWeekFrameDifferentialTests` Bild 461, Aufruf 3→4: `LoadImage(0)`, `FadeOut(1360)` werden **vor** denselben
    drei Bericht-Ereignissen gespielt.

  Die Bild-für-Bild-Tests bleiben grün, vermutlich weil der Bericht keine aktiven Objekte hat. Dann zeichnet `DrawObjects`
  (`23CD:06E9`) nichts und sichert nichts. Eine sichtbare Abweichung ist nicht belegt, deshalb nur P3. Vorschlag: die
  Bericht-Ereignisse wie die von Aufruf 9 vor den Ausgaben und der Seitenübernahme des Folgeschritts abspielen, oder Bericht
  und Ereignisse an einer gemeinsamen Stelle abspielen. Für das Menü fand die Probe keinen Fall mit falscher Reihenfolge. Es
  gab nur Menü→Bericht (Bilder 51, 109, 115), und dort spielt der Host das Menü vor dem Bericht, also wie das Original.
- **P3 R7E-3 – die Gegenprobe in der PR-Beschreibung stimmt nicht, die Ausgaben-Hüllen sind ungetestet.** Laut PR macht schon
  das Entfernen der `IncidentsPlayed(…)` um die Ausgaben den Test rot. Das stimmt nicht. Mutation nur dieser Hüllen
  ([Z. 82–83](https://github.com/kaaasi/traders-redone/blob/93b136a7ee31a27c52e28f3eabc171970fd56073/src/Traders.App/ProgramHost.cs#L82-L83)):
  `HostLoadedWeekFrameDifferentialTests` bleibt **grün**. Der Grund: `WeekScreens` ruft `Pages(c)` → `OutputPages`
  beim Anlegen jedes Schritts auf, und das spielt die restlichen Ereignisse schon vorher ab (Ernte ohne `Pages`, folgt aber nie
  im selben Bild auf Aufruf 9). Die Hüllen sind daher nur eine Absicherung. Wirksam und durch den Test abgedeckt ist die Hülle
  an `OutputPages`. Nur sie zu entfernen macht den Test rot (s. Tabelle). Kein Codefehler. Bitte die Beschreibung bzw. den
  Kommentar richtigstellen oder die Hüllen streichen.
- **P3 Hinweis (kein Befund):** Dass der Schrift-Reset `21FD:05DB(0)` am Ende von `17CB:0005` vor `inputs.Font()` des
  Stationsschritts ankommt, hängt nur an der Auswertungsreihenfolge der Argumente in
  [WeekScreens.cs:74](https://github.com/kaaasi/traders-redone/blob/93b136a7ee31a27c52e28f3eabc171970fd56073/src/Traders.Core/Phases/WeekScreens.cs#L74)
  (`Pages(c)` steht vor `Font()`). Das ist korrekt, aber fragil. Ein Kommentar dort würde helfen.

**Reihenfolge im Original (geprüft in `analyse/disasm`):** Jede der drei Gruppen in `17CB` endet mit `21FD:0004(0)` und
`21FD:05DB(0)` (z. B. `17CB:0966`/`096F` vor `retf` bei `0985`). Danach folgen `1777:023B` → `0240`: `21FD:0977(1)` (leer, `push
bp/pop bp/retf`) und `1777:024A` `call 1B03:0009`. Dort laufen `1B03:0079`, `186C:0002` und `1B03:148B`. Der letzte Durchlauf
der Warteschleife endet nach `21FD_0004_warteschleife.md` mit `23CD:06E9` (Objekte zeichnen und sichern). Er liegt also
vollständig vor `148B`. Der Fix stellt genau das her.

**Dateigrenzen:** Geändert sind nur `src/Traders.App/ProgramHost.cs` und `tests/Traders.Tests/App/HostLoadedWeekFrameDifferentialTests.cs`.
Beide liegen in den Grenzen der Spur `ende` (`docs/arbeitspakete.md`: `src/Traders.App/**`, `tests/Traders.Tests/App/**`). Die
Ursache ist im Host belegt (Probe und Gegenprobe oben), daher ist die Änderung dort begründet. Szenen, Kern und gemeinsame
Dateien bleiben unberührt. Spur `ende` Teil 3 muss auf diesen Stand rebasen.

## 3. Ausgeführte Prüfungen

| Prüfung | Befehl / Ablauf | Ergebnis |
|---|---|---|
| Gegenbeispiel R7E-1 | Kopie des Heads, `IncidentsPlayed(…)` an `OutputPages` **und** den Ausgaben zurückgebaut; `TR_REQUIRE_GAMEDATA=1 dotnet test … -c Release --filter FullyQualifiedName~HostLoadedWeekFrameDifferentialTests` | **rot:** `week frame 4703 (host frame 4812, call 11): plane 0 offset 1CDB (page 0, x 108, y 92) original 06 host 03` |
| Gegenprobe nur `OutputPages` | nur `OutputPages: () => Layers!.Ds` zurück | **rot:** `week frame 5628 (host frame 5737, call 11): plane 0 offset 5BA7 (page 1, x 156, y 88) original 07 host 01` |
| Gegenprobe nur Ausgaben | nur die vier Hüllen der Ausgaben zurück | **grün** (R7E-3) |
| Reihenfolge-Probe Bericht/Menü | instrumentierte Kopie, Filter `Traders.Tests.App` | 116/116 grün, Treffer s. R7E-2 |
| Original-Reihenfolge | `analyse/disasm/17CB.asm`, `1777.asm` (`0217`–`0222`), `1B03.asm` (`000C`–`0039`), `21FD.asm` (`0977`) | wie oben |
| Merge gegen `main` | `git merge-tree --write-tree origin/main 93b136a` (`main` = `62dbc3f`) | konfliktfrei |
| CI auf dem Head | Check-Runs von `93b136a` | `linux` success (`windows`, `downloads` übersprungen) |
| Volle Prüfung | `tools/pruefen.sh` (alle drei Läufe), Head `93b136a`, dotnet 10.0.402 | grün |

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| Mit Originaldaten (TR_REQUIRE_GAMEDATA=1) | 1080 | 0 | 0 |
| Ohne Originaldaten (TR_SKIP_GAMEDATA=1) | 469 | 0 | 316 |
| Pflichtlauf ohne Daten (muss scheitern) | 469 | 611 | 0 |

Hashes, Architektur und Abdeckung grün. Marker: `[ORIGINAL]` 539, `[APPROX]` 41. Der Diff bringt keine neuen `[APPROX]` und
keine neuen Marker.

**Der Test bezieht seine Erwartungswerte nur aus dem RefEmu:** Zufallszustand, beide Seiten, DAC und Anzeigestart werden je
Retrace verglichen, zusätzlich der Seitenzustand am ersten Bild von Aufruf 11. Neu ist, dass der Test über die ganze
Stationsphase und 50 Bilder der Ernte läuft. Die Prüfung `> 100` Stationsbilder stellt sicher, dass die Stationsphase
überhaupt erreicht wird.

**Akzeptierte `[APPROX]`:** keine neuen.
**Annahmen (nicht belegt):** Dass R7E-2 ohne sichtbare Wirkung bleibt, gilt nur für die geprüften Läufe (Woche 0, geladene
Woche 15). Dass der Bericht nie aktive Objekte hat, habe ich nicht im Original geprüft.
