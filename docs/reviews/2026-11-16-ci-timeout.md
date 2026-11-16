# Review: PR #172 – CI-Zeitlimit Job `linux` (#167)

Reviewer: Claude (geplanter Lauf, nicht an #172 beteiligt) · Base `0311ff5` · Head `fe49ac9149d4df917678049dde2375d67b06032f`

## 1. Merge-Empfehlung

**Ja** für Head `fe49ac9`. Der Diff ist genau eine Zeile (`timeout-minutes: 15` → `30` im Job `linux`), die Begründung ist
durch die Actions-Läufe belegt, und der CI-Lauf auf diesem Head wäre unter dem alten Limit selbst abgebrochen
(`tools/pruefen.sh` 15:03 min, Job 15:20 min). Kontingent nach `anhang.md` B unkritisch.

## 2. Befunde

Keine P0–P2.

- **P3 (Hinweis) Streuung statt Trend:** Laufzeiten desselben Stands schwanken stark. Push-Lauf `main` `59cae18`
  (43787571445, 01:39:59–01:47:38) 7,7 min, fast gleichzeitig der PR-Lauf zu #172 (43787649075, Merge-Commit `main`+1 Zeile)
  15,3 min. 30 min ergeben gegenüber dem bisher längsten beobachteten Lauf etwa Faktor 2 Reserve – ausreichend, aber bei
  weiter wachsenden Bildfolgetests im Blick behalten (#167 nach Merge schließen; bei erneuten Läufen > 20 min neues Issue,
  ggf. Ursache der Streuung im `pruefen.sh`-Log suchen statt Limit weiter anheben).
- **P3 (Hinweis):** Job `windows` (`timeout-minutes: 20`, nur ein `dotnet test`-Lauf) und `downloads` (15) bleiben unverändert;
  laut Auftrag gewollt, beide nur per `workflow_dispatch`. Kein Beleg für ein Problem dort.

## 3. Ausgeführte Prüfungen

**Diff** (`git diff 0311ff5 fe49ac9`): nur `.github/workflows/build.yml`, 1+/1−, Zeile 44 im Job `linux`. Übrige
`timeout-minutes` (`build.yml` 97/117, `aufraeumen.yml`, `werkzeuge.yml`) unverändert. Merge-Base = Base `0311ff5` = aktueller
`main`.

**Laufzeiten** (`gh api repos/kaaasi/traders-redone/actions/runs/<id>/attempts/<n>/jobs`, Schritt `tools/pruefen.sh`):

| Lauf | PR | Versuch | Ergebnis | Schritt `pruefen.sh` |
|---|---|---|---|---|
| 43767584534 | #153 (`e3df8ff`) | 1 | cancelled | 22:08:48–22:23:39 (14:51, Job 15:15 = Limit) |
| 43767584534 | #153 | 2 | success | Job 22:25:54–22:34:58 (9:04) |
| 43780479270 | #151 (`97446ec`) | 1 | cancelled | 00:27:50–00:42:47 (14:57, Job 15:17 = Limit) |
| 43780479270 | #151 | 2 | success | 01:25:32–01:34:51 (9:19) |
| 43787649075 | #172 (Head) | 1 | **success** | 01:41:01–01:56:04 (15:03, Job 15:20) |

Die PR-Beschreibung („nach 15 min abgebrochen, Wiederholungen nach 9–10 min grün“) stimmt mit den Läufen überein.
Push-Läufe auf `main` 12.–16.11.: 4–10 min.

**Kontingent (`anhang.md` B, 2.000 min/Monat):** Summe der Wall-Clock-Zeiten aller nicht übersprungenen Läufe seit 01.11.
≈ 348 min (56 Läufe, `gh api "…/actions/runs?created=>=2026-11-01&per_page=100"`). Das höhere Limit kostet nur bei hängenden
Läufen bis zu 15 min mehr je Lauf; abgebrochene Läufe mussten bisher ohnehin wiederholt werden (≈ 15 min verschenkt je
Abbruch), die Änderung spart also eher Minuten. Kein Einfluss auf Windows-Faktor, Draft-Regel oder Pfadfilter.

**CI auf Head:** Build 43787649075 `success`, Originaldaten eingebunden (Schritt „Original files …“ success), volle Prüfung:

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---|---|---|
| Mit Originaldaten (TR_REQUIRE_GAMEDATA=1) | 1025 | 0 | 0 |
| Ohne Originaldaten (TR_SKIP_GAMEDATA=1) | 456 | 0 | 294 |
| Pflichtlauf ohne Daten (muss scheitern) | 456 | 569 | 0 |

Markerbericht: [ORIGINAL] 501, [APPROX] 39 (unverändert, kein Code geändert).

**Lokale `tools/pruefen.sh`-Läufe:** nicht ausgeführt – nur CI-Konfiguration geändert, lokale Läufe unverändert; kein Code,
kein Test und kein Skript betroffen. Die drei Läufe sind durch den CI-Lauf auf genau diesem Head (mit Originaldaten) belegt.
Testnachweis damit aus CI, nicht aus eigenem Checkout-Build.

**Akzeptierte `[APPROX]`:** keine neuen. **Annahmen:** Laufzeitstreuung liegt am Runner, nicht am Code (nicht untersucht).
