# Review R8-zeitlimit – PR #223 (Head `1168a93`, Base `420c831`)

Reviewer: Claude Opus 5.5 (geplanter Lauf, eigene Sitzung, nicht am PR beteiligt). Erstreview, Änderung an der
Test-Infrastruktur. Checkout `1168a935196bad44a7a5296d9d32590f8caf70e1` (`git rev-parse HEAD` = Head, Arbeitskopie sauber),
Originaldaten aus `kaaasi/traders-referenz` (`7fcfb24`), dotnet 10.0.402. Base-Vergleich in eigenem Worktree auf `420c831`.

## 1. Merge-Empfehlung

**Nein** für `1168a93`. Die Tabelle aus den TRX-Zählern liefert auf dem Head exakt dieselben Zahlen wie `main` (alle drei
Läufe, Pflichtlauf scheitert), und das Hang-Limit greift wie beschrieben. Aber: ein Abbruch durch das neue Limit gilt im
**Pflichtlauf als grün** (R8Z-1, P1), und im Lauf ohne Daten zeigt die Tabellenzeile eines abgebrochenen Laufs dieselben
Zahlen wie ein grüner Lauf (R8Z-2). Beides widerspricht „Abbruch darf nie als grün gelten“. Dazu weicht der Windows-Job von
der einheitlichen Vorgabe 20 min ab (R8Z-3). Fixlauf mit `tools/pruefen.sh` (alle drei Läufe), danach Nachprüfung.

## 2. Befunde

**R8Z-1 (P1) – Hänger im Pflichtlauf ergibt „Ergebnis: grün“.**
[`tools/pruefen.sh` Z. 93–94](https://github.com/kaaasi/traders-redone/blob/1168a935196bad44a7a5296d9d32590f8caf70e1/tools/pruefen.sh#L93-L94):
für `soll=1` zählt nur `rc≠0`; ein Abbruch (Hang-Limit, Testhost-Absturz) ist ebenfalls `rc≠0`. Die Markerprüfung danach sieht
nur die bis zum Abbruch gelaufenen Tests – ein still leer laufender Originaldaten-Test nach dem Abbruchpunkt fällt nicht auf.
Ohne die Änderung hing derselbe Lauf endlos (nie grün); das Limit macht daraus einen grünen Durchlauf.
Beleg (temporäre Kopie, Test `HangProbe.Artificial_hang_probe` hängt nur bei `TR_REQUIRE_GAMEDATA=1` +
`TRADERS_GAMEDATA=…/keine-originaldaten`):
`TRADERS_GAMEDATA=/nonexistent HANG_PROBE=pflicht TR_HANG_TIMEOUT=1m bash tools/pruefen.sh` → `EXIT 0`, „Ergebnis: grün“,
Pflichtlauf-Zeile `475 | 669 | 0` (Probe fehlt, 1145 erwartet); `test-pflicht.txt`: „inactivity time of 60 seconds has
elapsed“, „Test Run Aborted.“, „The test running when the crash occurred: Traders.Tests.HangProbe.Artificial_hang_probe“.
Vorschlag: in `lauf()` für **jeden** Lauf `grep -q "^Test Run Aborted" "$log"` (bzw. fehlende/abgebrochene TRX) → `fehler
"$name abgebrochen"`; im Pflichtlauf zusätzlich `nt -eq PROJEKTE` prüfen.

**R8Z-2 (P2) – Tabellenzeile eines abgebrochenen Laufs ist von grün nicht zu unterscheiden.**
[Z. 81–92, 99](https://github.com/kaaasi/traders-redone/blob/1168a935196bad44a7a5296d9d32590f8caf70e1/tools/pruefen.sh#L81-L99):
Der hängende Test erscheint in den TRX-`Counters` nicht (weder `failed` noch `aborted`/`timeout`), `nt` = 1 = `PROJEKTE`.
Beleg: `TRADERS_GAMEDATA=/nonexistent HANG_PROBE=1 TR_HANG_TIMEOUT=1m bash tools/pruefen.sh --schnell` → `EXIT 1` nach 72 s,
„FEHLER: Ohne Originaldaten“, Ausgabe nennt `Traders.Tests.HangProbe.Artificial_hang_probe` – aber Zeile
`| Ohne Originaldaten (TR_SKIP_GAMEDATA=1) | 475 | 0 | 366 |`, identisch mit dem grünen Lauf. Nur „Ergebnis: ROT“ zeigt den
Fehler; wer die Tabelle in PR/Merge-Kommentar kopiert (§6 Nr. 4), überträgt einen scheinbar grünen Lauf. Auf `main` fehlte bei
Abbruch die „Passed!“-Zeile und der Projektzähler schlug an. Vorschlag: bei Abbruch Zeile mit „abgebrochen“ kennzeichnen bzw.
`fehlgeschlagen` ≥ 1 (z. B. `Counters`/`ResultSummary outcome` plus `rc`/Abbruchzeile auswerten).

**R8Z-3 (P2) – Vorgabe nicht einheitlich 20 min.**
[`.github/workflows/build.yml` Z. 110](https://github.com/kaaasi/traders-redone/blob/1168a935196bad44a7a5296d9d32590f8caf70e1/.github/workflows/build.yml#L110):
`${TR_HANG_TIMEOUT:-15m}` statt 20m (Plan-Review-Hinweis); Grund ist `timeout-minutes: 20` des Jobs (Z. 97). Beleg: `grep -n
"15m\|timeout-minutes" .github/workflows/build.yml`. Vorschlag: `20m` und Job-Limit z. B. 30 min, damit das Hang-Limit vor dem
Job-Limit greift und den Test nennt.

**R8Z-4 (P3) – Ausgabe nennt bei mehreren laufenden Tests nur den ersten.**
[Z. 96](https://github.com/kaaasi/traders-redone/blob/1168a935196bad44a7a5296d9d32590f8caf70e1/tools/pruefen.sh#L96):
`grep -A1` nach „The test running when“. Beleg: `HANG_PROBE=1 TR_HANG_TIMEOUT=1m bash tools/pruefen.sh --schnell` (mit Daten)
→ Log listet zwei laufende Tests (`TradeAiDifferentialTests.Every_call_…`, `MultiWeekDifferentialTests.Four_weeks_with_mixed_…`),
die Ausgabe nur den ersten. Vorschlag: Block bis zur Leerzeile ausgeben (z. B. `sed -n '/The test running when/,/^$/p'`).

**R8Z-5 (P3, Verdacht) – Linux-Job-Limit 30 min.** Voller Lauf ≈ 25 min (hier gemessen); ein Hänger mit 20-min-Limit führt
in CI meist zum Job-Abbruch, bevor `pruefen.sh` den Testnamen ausgibt (Konsolenausgabe geht in `$LOG`). Rot bleibt es; nur
die Diagnose fehlt. Nicht in CI ausgeführt.

Hinweis (kein Befund): Das Limit ist ein Inaktivitätslimit; mit `TR_HANG_TIMEOUT=1m` und Originaldaten bricht der Lauf auch
ohne Hänger ab (echte Tests > 1 min, z. B. `Entries_match_the_original_on_states_of_weeks_0_to_3` 1 m 34 s). 20 min ist
ausreichend; für Proben mit Daten 1m nur mit `--filter`.

**Erwartet offen – bestätigt:** Punkt (5) Kommentar in `PlantingSceneDifferentialTests.cs` unverändert; #217 ist inzwischen
auf `main` gemergt (17.11.2026 21:25 UTC, `740c6b1`), Punkt 5 ist damit jetzt umsetzbar (Fixlauf nach Merge von `main`).
Windows-Job nicht gelaufen (nur per Hand; Check-Run „skipped“). P3 #49, #75, #76, #220, #221 offen.

**Regeltext (3):** `docs/routines/_vorlage.md` und `zusammenarbeit.md` §5 Nr. 1 knapp, eindeutig und gleichlautend; kein Befund.

## 3. Ausgeführte Prüfungen

- `git ls-remote` beider Repos (lesbar); `fetch-reference.sh original`, `setup-workspace.sh`; `git diff 420c831 1168a93`.
- `bash tools/pruefen.sh` (alle drei Läufe) auf Head und auf Base `420c831`, beide `EXIT 0`, Tabellen identisch:

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| Mit Originaldaten (TR_REQUIRE_GAMEDATA=1) | 1144 | 0 | 0 |
| Ohne Originaldaten (TR_SKIP_GAMEDATA=1) | 475 | 0 | 366 |
| Pflichtlauf ohne Daten (muss scheitern) | 475 | 669 | 0 |

  TRX-`Counters` Head: mit `total=1144 executed=1144`; ohne `total=841 executed=475` (366 × `NotExecuted`); Pflicht
  `total=1144 passed=475 failed=669` – übersprungen = total − executed stimmt.
- Gegenbeispiel mit Änderung: Probe (s. R8Z-2) → Abbruch nach ≈ 60 s Inaktivität, Lauf 72 s inkl. Build, Testname in der
  Ausgabe, `EXIT 1`.
- Gegenbeispiel ohne Änderung (Base-Worktree, Probe kopiert, Aufruf wie `lauf()` auf `main`):
  `HANG_PROBE=1 timeout 240 dotnet test TradersRedone.slnx -c Release --no-build --filter FullyQualifiedName~HangProbe` →
  `rc 124` nach 240 s, kein Abbruch durch `dotnet test`.
- Pflichtlauf-Probe (R8Z-1) und Mehrfachnennung (R8Z-4) wie oben. Probedateien nur in temporären Worktrees, entfernt.
- CI auf dem Head: `linux` success, `windows`/`downloads` skipped (`gh api …/commits/1168a93/check-runs`).
- `main` ist seit Base weiter (`1ff6468`, u. a. #217), `git merge-tree` ohne Konflikt.

Akzeptierte `[APPROX]`: keine (kein Produktivcode). Annahmen: R8Z-5 nicht in CI belegt.
