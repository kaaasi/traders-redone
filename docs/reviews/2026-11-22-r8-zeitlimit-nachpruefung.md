# Nachprüfung R8-zeitlimit – PR #223 (Head `1cb7ccc`, Base `740c6b1`)

Reviewer: Claude Opus 5.5 (geplanter Lauf, eigene Sitzung, nicht am PR beteiligt). Nachprüfung von
`docs/reviews/2026-11-17-r8-zeitlimit.md` (PR #229, geprüfter Head `1168a93`), Delta `1168a93..1cb7ccc`: Merge von `main`
(`11e3c79`, ohne Konfliktauflösung, `git show --remerge-diff` leer) und Fixlauf `1cb7ccc` (3 Dateien). Checkout
`1cb7cccc825f7becdc1266e8e943eb3a01388110` (`git rev-parse HEAD` = Head, `git status --porcelain` leer), Originaldaten aus
`kaaasi/traders-referenz` (`7fcfb24`), dotnet 10.0.402. `main` = `60e65f4` ist im Head enthalten.

## 1. Merge-Empfehlung

**Ja** für `1cb7ccc`. R8Z-1 und R8Z-2 sind behoben: beide Gegenbeispiele laufen jetzt rot, die Tabellenzeile trägt
„**abgebrochen**“ und fehlgeschlagen ≥ 1; die Gegenprobe (Abbrucherkennung entfernt) bringt die grün aussehende Zeile zurück.
R8Z-3/R8Z-4/R8Z-5 sind umgesetzt, R8Z-4 jetzt mit zwei echten gleichzeitig laufenden Tests belegt. `--schnell` mit
Originaldaten grün. Offen bleibt nur P3 (veraltete PR-Beschreibung, R8ZN-1).

## 2. Befunde

**R8Z-1 (P1) – behoben.** [`tools/pruefen.sh` Z. 93–104](https://github.com/kaaasi/traders-redone/blob/1cb7cccc825f7becdc1266e8e943eb3a01388110/tools/pruefen.sh#L93-L104): Abbruch (`Test Run Aborted` oder
TRX-Zahl ≠ Testprojekte) wird vor der `soll`-Unterscheidung geprüft und ist in jedem Lauf `fehler`. Beleg (temporäre Kopie,
Probetest `HangProbe.Artificial_hang_probe`, hängt bei `HANG_PROBE=1` immer, bei `pflicht` nur mit `TR_REQUIRE_GAMEDATA=1` und
`TRADERS_GAMEDATA=…keine-originaldaten`): `TRADERS_GAMEDATA=/nonexistent HANG_PROBE=pflicht TR_HANG_TIMEOUT=1m bash tools/pruefen.sh`
→ `EXIT 1`, „FEHLER: Pflichtlauf ohne Daten (muss scheitern) abgebrochen …“, Testname ausgegeben, Zeile
`| Pflichtlauf ohne Daten (muss scheitern) – **abgebrochen** | 475 | 725 | 0 |`, „Ergebnis: ROT“ (vorher `EXIT 0`, grün).

**R8Z-2 (P2) – behoben.** [Z. 103, 109–110](https://github.com/kaaasi/traders-redone/blob/1cb7cccc825f7becdc1266e8e943eb3a01388110/tools/pruefen.sh#L103-L110). Beleg:
`TRADERS_GAMEDATA=/nonexistent HANG_PROBE=1 TR_HANG_TIMEOUT=1m bash tools/pruefen.sh --schnell` → `EXIT 1`,
`| Ohne Originaldaten (TR_SKIP_GAMEDATA=1) – **abgebrochen** | 475 | 1 | 413 |`. Gegenprobe: Z. 96 durch `:` ersetzt (keine
Abbrucherkennung) → wieder `| Ohne Originaldaten (TR_SKIP_GAMEDATA=1) | 475 | 0 | 413 |` bei „Ergebnis: ROT“ – der Fix ist die
Ursache.

**R8Z-3 (P2) – behoben.** [`build.yml` Z. 97, 110](https://github.com/kaaasi/traders-redone/blob/1cb7cccc825f7becdc1266e8e943eb3a01388110/.github/workflows/build.yml#L97-L110): Windows `${TR_HANG_TIMEOUT:-20m}`,
Job-Limit 30 min. Beleg: `grep -n "timeout-minutes\|20m" .github/workflows/build.yml` → Z. 44 `60`, Z. 97 `30`, Z. 110 `20m`.
Windows-Job nicht ausgeführt (nur per Hand, Check-Run „skipped“).

**R8Z-4 (P3) – behoben.** [Z. 99](https://github.com/kaaasi/traders-redone/blob/1cb7cccc825f7becdc1266e8e943eb3a01388110/tools/pruefen.sh#L99): Block bis zur Leerzeile. Beleg (Kopie, mit Daten):
`TRADERS_GAMEDATA=…/gamedata HANG_PROBE=1 TR_HANG_TIMEOUT=1m bash tools/pruefen.sh --schnell` → Ausgabe nennt
`HangProbe.Artificial_hang_probe` **und** `CatastropheFrameDifferentialTests.Catastrophe_scene_matches_the_original_frame_by_frame`,
Zeile `… – **abgebrochen** | 899 | 1 | 0 |`, `EXIT 1`.

**R8Z-5 (P3) – behoben.** [`build.yml` Z. 44](https://github.com/kaaasi/traders-redone/blob/1cb7cccc825f7becdc1266e8e943eb3a01388110/.github/workflows/build.yml#L44): Linux-Job 60 min. CI auf `1cb7ccc`: `linux`
17 min 13 s (23:53:38–00:10:51 UTC) inkl. Einrichtung. Hinweis (kein Befund, aus der R8Z-4-Probe): das Inaktivitätslimit
greift erst, wenn **kein** paralleler Test mehr fertig wird – der Probetest hing ab Laufbeginn, der Abbruch kam erst nach
≈ 15 min Testlauf (`real 16m56s` inkl. Build), als nur noch er und der lange Katastrophentest liefen. Ein Hänger verlängert
einen Lauf also auf höchstens Laufdauer + 20 min; der erste Hänger (≈ 17 + 20 min) wird damit sicher vor dem Job-Limit
gemeldet, die Ausgabe geht über `tee` sofort ins Job-Log. Nur ein Hänger in einem Test ohne Daten, der in allen drei Läufen
hängt, könnte das 60-min-Limit reißen – dann ist der Testname aus dem ersten Lauf schon im Log. Kein Handlungsbedarf.

**Punkt 5 / #228 – behoben.** [`PlantingSceneDifferentialTests.cs` Z. 246–249](https://github.com/kaaasi/traders-redone/blob/1cb7cccc825f7becdc1266e8e943eb3a01388110/tests/Traders.Tests/Field/PlantingSceneDifferentialTests.cs#L246-L249):
`[APPROX]` entfernt, Verweis auf `FieldScene.NoticeSteps` (seit #217) stimmt mit
`src/Traders.Presentation/Field/FieldScene.Output.cs` (`StatusSteps` ruft `NoticeSteps` bei `Planting.NoTime` = 1) überein;
der Test vergleicht `s.Status()` ohne Liste (`compareList: false`), wie beschrieben. `tools/abdeckung.py --check`: „Abdeckung
aktuell.“ Commit enthält `Fixes #228` (schließt beim Merge).

**R8ZN-1 (P3) – PR-Beschreibung veraltet.** PR #223: „Prüfstand“ nennt noch Head `1168a93`/Base `420c831`, „Vorgabe 15m wegen
Job-Limit 20m“, „Punkt 5 … unverändert“ und die alte Tabelle (1144/366/669). Der aktuelle Stand steht nur im
Übergabekommentar vom 18.11.2026 (`gh api repos/kaaasi/traders-redone/issues/223/comments`). Vorschlag: Beschreibung vor dem
Merge auf `1cb7ccc` nachziehen (§6 Nr. 6) oder im Merge-Kommentar auf die Übergabe verweisen. Ebenfalls nur Text: die
Übergabe nennt Base `740c6b1`, hereingemergt ist `60e65f4` (Nachfolger, enthält `740c6b1`).

**Erwartet offen – bestätigt:** #49, #75, #76, #220, #221 offen (P3, `gh api …/issues/<nr>` → `open`).

## 3. Ausgeführte Prüfungen

- `git ls-remote` beider Repos (lesbar); `fetch-reference.sh original`, `setup-workspace.sh`; `git diff 11e3c79 1cb7ccc`,
  `git show --remerge-diff 11e3c79`.
- `bash tools/pruefen.sh --schnell` auf dem Head mit Originaldaten → `EXIT 0`, „Ergebnis: grün“, 21 min 31 s:

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| Mit Originaldaten (TR_REQUIRE_GAMEDATA=1) | 1200 | 0 | 0 |

- Gegenbeispiele R8Z-1, R8Z-2, R8Z-4 und Gegenprobe R8Z-2 wie oben, nur in temporärem Worktree (`git worktree add`, Probetest
  und Mutation nicht eingecheckt, Worktree entfernt). Lauf ohne Daten in der R8Z-1-Probe:
  `| Ohne Originaldaten (TR_SKIP_GAMEDATA=1) | 476 | 0 | 413 |` (475 + Probetest), Markerprüfung ohne Befund.
- CI auf `1cb7ccc`: `linux` success, `windows`/`downloads` skipped (`gh api …/commits/1cb7ccc…/check-runs`).

Akzeptierte `[APPROX]`: keine neuen (ein `[APPROX]`-Kommentar entfällt, #228). Annahmen: Windows-Job (R8Z-3) und
Job-Limit-Verhalten in CI (R8Z-5) nur aus Konfiguration und lokaler Probe, nicht in CI ausgeführt.
