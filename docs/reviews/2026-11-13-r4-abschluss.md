# Review Abschluss Runde 4 (PR #114)

**Reviewer:** Claude (geplanter Lauf, neue Sitzung ohne Mitarbeit an #114) · **Base:** `69db0d4` · **Head:** `1f2b354`
(`1f2b3546fb80b7d1c0b42fa2e88d4ca5a14c4327`, `git rev-parse HEAD` bestätigt, `git status --porcelain` leer)

## 1. Merge-Empfehlung

**Ja** für genau `1f2b354`. Reiner Doku-PR (4 Dateien, `git diff --stat 69db0d4 1f2b354`); `tools/pruefen.sh --schnell` mit
Originaldaten grün (754/0/0). Der archivierte Plan ist wortgleich mit dem gemergten Stand, der Abschluss nennt alle neun
R4-Reviews aus `docs/reviews/`, die Issue-Listen stimmen mit GitHub, M4 ✅ ist durch Gesamtreview + Fix #111 (Review „Ja“)
gedeckt. Einziger Befund: zwei relative Links, die durch das Verschieben ins Archiv brechen (P3, nicht mergehindernd).

## 2. Befunde

**R4A-1 (P3) – Zwei relative Links im Archiv zeigen ins Leere.** Der Plan wurde aus `docs/arbeitspakete.md` (Verzeichnis
`docs/`) nach `docs/archiv/` verschoben; die Links `routines/…` lösen jetzt nach `docs/archiv/routines/…` auf:
[runde-4.md Z. 42](https://github.com/kaaasi/traders-redone/blob/1f2b3546fb80b7d1c0b42fa2e88d4ca5a14c4327/docs/archiv/runde-4.md#L42)
(`21FD_0687_codeabfrage.md`) und
[Z. 105](https://github.com/kaaasi/traders-redone/blob/1f2b3546fb80b7d1c0b42fa2e88d4ca5a14c4327/docs/archiv/runde-4.md#L105)
(`0E9E_000E_steuerung.md`). Beleg (Linkprobe relativ zum Dateiverzeichnis über `runde-4.md`, `arbeitspakete.md`, `STATUS.md`,
`CHANGELOG.md`):

```
docs/archiv/runde-4.md:42 BROKEN routines/21FD_0687_codeabfrage.md
docs/archiv/runde-4.md:105 BROKEN routines/0E9E_000E_steuerung.md
```

Die Ziele existieren unter `docs/routines/`; dieselbe Probe über `docs/archiv/runde-2.md` und `runde-3.md` meldet nichts. Fix:
`../routines/…` (Plantext sonst unverändert lassen; „wie gemergt“ bleibt inhaltlich gewahrt).

Sonst keine Befunde. Geprüft und in Ordnung:

- **Archiv, Plan:** `sed -n '8,158p'` der Base-`arbeitspakete.md` gegen `sed -n '1,151p' docs/archiv/runde-4.md` → einziger
  Unterschied die Überschrift (`## Aktuelle Runde: 4 …` → `# Runde 4 …`), wie in `runde-3.md`. Statusabschnitte der vier Spuren
  (Z. 60, 84, 100, 117) nennen PR, Merge und Review „Ja“ bzw. die Restlücken `0609:0C73`, `0609:03E0`, `135D:000E` (→ R5, #103).
- **Archiv, Abschluss:** PR-Nummern #90, #92–#96, #106, #111 und `main` `69db0d4` stimmen mit GitHub (alle gemergt; `8d688e2` =
  Head #96). Alle Dateien `docs/reviews/2026-11-1[23]-*` mit R4-Bezug (plan, prozess-aufgaben, partie, menue, spielfeld,
  programm, status, gesamt, host-bildnachholung) sind genannt, Urteile stimmen (plan/status/gesamt „Nein“, übrige „Ja“).
  Gesamtreview: einziger Befund P2 Bildnachholung (#110), behoben mit #111, Review `2026-11-13-host-bildnachholung.md` „Ja“.
- **Issues:** offen laut GitHub genau #49, #75, #76, #99, #101, #103, #105, #109, #113 (= Erwartung, = Archiv Z. 163);
  geschlossen seit 12.11. u. a. #39, #74, #81, #86, #89, #110 (= Archiv). #52, #77, #79, #80 gehören zu R3 (`runde-3.md`).
- **`STATUS.md`:** Kopfzeile auf 13.11./abgeschlossen mit gültigem Archivlink; M4 🟢 → ✅ mit #111, Muster wie M3 („Gesamtreview
  … erledigt“). Nach `zusammenarbeit.md` §5 Punkt 4 braucht ein P2-Fix keine Nachprüfung; Erstreview „Ja“ liegt vor.
  `abdeckung.py --check` grün (der generierte Block ist unverändert).
- **`CHANGELOG.md`:** Zeitraum 12.–13.11., Abschluss `69db0d4`, Gesamtreview „Nein“ → Fix #111, M4 abgeschlossen – stimmt.
  (Die Testzahl 751 bezieht sich auf die Spuren; der Head hat 754 durch #111 – kein Befund.)
- **`docs/arbeitspakete.md`:** keine aktuelle Runde, Archivlink ergänzt; Vorschau R5 nach Fahrplan (`0609`, `135D`, `11A3`,
  `0825`) mit den Pflichten aus R4: Leseregel (#101, #103, #105), `DS:330C` ein Besitzer (#103 P3-4), Sammelmarker (#109); übrige
  offene P3 (#49, #75, #76 P3-2, #99, #113) genannt – zusammen alle neun offenen Issues. Die R5-Hinweise aus den Reviews
  (`r4-programm.md` Z. 37 → #105; `r4-status.md` Z. 60 → #103) sind abgedeckt. Fahrplan unverändert.

## 3. Ausgeführte Prüfungen

```
git ls-remote https://github.com/kaaasi/traders-redone HEAD     # 69db0d4…  (lesbar)
git ls-remote https://github.com/kaaasi/traders-referenz HEAD   # 7fcfb24…  (lesbar)
git checkout 1f2b3546fb80b7d1c0b42fa2e88d4ca5a14c4327
tools/claude/fetch-reference.sh original && tools/claude/setup-workspace.sh
source ~/claude-tools-traders/env.sh && tools/pruefen.sh --schnell
python3 -I tools/abdeckung.py --check                            # „Abdeckung aktuell.“ rc 0
diff <(git show 69db0d4:docs/arbeitspakete.md | sed -n '8,158p') <(sed -n '1,151p' docs/archiv/runde-4.md)
GitHub: list_issues (OPEN; CLOSED seit 2026-11-12), list_pull_requests (state all), pull_request_read #114
```

`tools/pruefen.sh --schnell` (Originaldaten SHA-256 bestanden, Architekturgrenzen, Abdeckung, Restore `--locked-mode`, Release-Build):

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| Mit Originaldaten (TR_REQUIRE_GAMEDATA=1) | 754 | 0 | 0 |

dotnet 10.0.402 · Marker 248 `[ORIGINAL]` / 28 `[APPROX]`. **Akzeptierte `[APPROX]`:** keine neuen (Doku-PR). **CI:** auf dem Head
`linux` success (Lauf 43385320565, wegen `STATUS.md`, §6 Nr. 3), `windows`/`downloads` übersprungen. **Annahmen:** keine.
