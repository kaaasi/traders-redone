# Review: Abschluss Runde 6 (PR #177)

**Reviewer:** Claude (geplanter Lauf, eigene Sitzung, nicht am PR beteiligt) · **PR:** #177 · **Base:** `main` `e711ddf`
· **Head:** `574a12addaa2a97e945bb267883d3bb18d5ac17c` (`prozess/r6-status`) · Umfang nach Auftrag: nur Doku und `STATUS.md`.

## 1. Merge-Empfehlung

**Nein** für genau `574a12a`, wegen R6Ab-1 (P2, wenige Zeilen). `STATUS.md` setzt M6, M9, M10 auf ✅, die Segmenttabelle
(§2) führt dieselben Segmente aber weiter als offen bzw. ⬜, und die Stand-Zeile nennt noch Runde 5. Sonst stimmt alles mit
GitHub überein: `CHANGELOG.md`, `docs/archiv/runde-6.md` (PRs, Reviews, geschlossene und offene Issues), `docs/arbeitspakete.md`
(Aktuelle Runde: –, Vorschau R7) und die Aufrufnummern der Vorschau. `abdeckung.py --check` und `tools/pruefen.sh --schnell`
mit Originaldaten sind grün (1041/0/0), CI `linux` auf dem Head grün. Nach dem Fix von R6Ab-1 genügt eine Delta-Prüfung durch
die Leitstelle.

## 2. Befunde

**R6Ab-1 (P2) – Segmenttabelle und Stand-Zeile in `STATUS.md` nicht auf R6 nachgezogen**

- [STATUS.md#L3](https://github.com/kaaasi/traders-redone/blob/574a12addaa2a97e945bb267883d3bb18d5ac17c/STATUS.md#L3):
  „Stand: 14.11.2026 · Runde 5 … abgeschlossen …; nächste: Runde 6“.
- [L66](https://github.com/kaaasi/traders-redone/blob/574a12addaa2a97e945bb267883d3bb18d5ac17c/STATUS.md#L66) `111D`:
  „🟡 … Katastrophen offen (R6)“; [L74](https://github.com/kaaasi/traders-redone/blob/574a12addaa2a97e945bb267883d3bb18d5ac17c/STATUS.md#L74)
  `17CB`: „⬜“; [L77](https://github.com/kaaasi/traders-redone/blob/574a12addaa2a97e945bb267883d3bb18d5ac17c/STATUS.md#L77)
  `1B03`: „⬜“; [L88](https://github.com/kaaasi/traders-redone/blob/574a12addaa2a97e945bb267883d3bb18d5ac17c/STATUS.md#L88)
  `22A3`: „🟡 … Ernte offen“.
- Widerspruch im selben Dokument: der erzeugte Block „Abdeckung je Segment“ zählt `111D` 7/7/7/7 (L548), `17CB` 9/9/9/9 (L556),
  `1B03` 44/44/44/44 (L559), `22A3` 15/15/15/15 (L570) Far-Einsprünge mit Befund/portiert/Differenztest, und M6/M9/M10 stehen auf ✅.
  `1777` (L73) nennt die Einbindung der Aufrufe 4, 9, 11, 16 (R6 Spur 0) nicht.
- Beleg: `grep -nE '^\| \`(111D|17CB|1B03|22A3)\`' STATUS.md` auf dem Head → genau die acht genannten Zeilen; `git diff
  origin/main...574a12a -- STATUS.md` ändert nur L18, L21, L22 (Meilensteine). Vorbild für die Pflege: die R5-Zeilen (`21FD` „R5
  Spur 3“, `111D` Wächter „R5 Spur 0“). §2 außerhalb des erzeugten Blocks gehört laut Archiv („Zuständigkeit“) der Leitstelle.
- Vorschlag: Stand-Zeile auf 16.11.2026 / Runde 6 abgeschlossen ([archiv/runde-6.md](docs/archiv/runde-6.md)), nächste: Runde 7;
  `111D`, `17CB`, `1B03`, `22A3` auf ✅ mit Kurzangabe (R6 Spur 1/2/3/0, Regelseite `Pixels/`), `1777` um „Aufrufe 4, 9, 11, 16
  eingebunden (R6)“ ergänzen.

**R6Ab-2 (P3) – PR-Beschreibung nennt veralteten Head**

- PR #177 nennt „**Head:** `3617fa5`“; der Head ist `574a12a` (zwei weitere Commits `cd92862`, `574a12a` mit den
  Nutzerentscheidungen zur Vorschau R7). Die Beschreibung spricht noch von einer „Frage zur Reihenfolge Aufruf 18/19“, die
  inzwischen entschieden ist. Beleg: `gh api repos/kaaasi/traders-redone/pulls/177 --jq .head.sha` → `574a12a…`; `git log
  --oneline origin/main..574a12a` → 5 Commits, oberster `574a12a`. Vorschlag: Beschreibung beim Fix von R6Ab-1 nachziehen.

**Ohne Befund geprüft**

- **Archiv, gemergte PRs:** #143, #146, #148, #149, #150, #151, #152, #153, #172 alle `merged=true`; #152 hat Merge-Commit
  `ebaddee` (= „`main` am Ende der Spuren“). Integrationsfolge Regelseite → 2 → 1 → 3 → 0 passt zu den Merge-Zeitpunkten.
  Review-PRs #144, #145, #154, #159, #160, #161, #168, #171, #173, #174, #178, #179 gemergt.
- **Archiv, Reviews:** alle zwölf Dateien `docs/reviews/2026-11-1[56]-*` vorhanden; die genannten Urteile stimmen (Plan Nein →
  Nachprüfung Ja; Regelseite, Fix, Ereignisse, Katastrophen, CI Ja; Stationen Nein → Nachprüfung Ja; Ernte Nein; Gesamt Teil 1
  „noch keine Freigabe“ = Zwischenstand, Teil 2 Ja für `ebaddee`).
- **Archiv, Issues:** Geschlossen-Liste (16 Issues) = genau die seit 15.11.2026 geschlossenen Nicht-PR-Issues (#99/#103 am
  14.11. gehören zu R5). Offen = genau die offenen Issues des Repos außer PR #177: #49, #75, #76, #157, #158, #162, #163, #165,
  #166, #180, alle `befund,P3` – wie erwartet bestätigt.
- **Links:** alle relativen Links in `docs/archiv/runde-6.md`, `docs/arbeitspakete.md`, `STATUS.md`, `CHANGELOG.md` lösen auf; alle
  in Backticks genannten Review-Dateien existieren.
- **`CHANGELOG.md`:** PR-Nummern und Issue-Bezüge wie im Archiv; „1041 Tests grün mit Originaldaten“ = eigener Lauf.
- **`docs/arbeitspakete.md`:** „Aktuelle Runde: –“ mit Archivlink; Offen-Liste plus Pflichten (#157, #158, #180) = erwartete
  Menge. **Vorschau R7, Aufrufnummern** gegen `docs/routines/1777_01F8_wochenschleife.md` (L34–L41): 18 `1C68:000B`
  Roboterphase, 19 `20B3:0005` Endprüfung, 23 `0B9F:0002` Handel, 24 `1DEF:000C` Zusatzphase – stimmen. „Host danach bis zum
  Handel“ ist schlüssig: 20 (`0609:1178`) und 21 sind portiert (`src/Traders.Core/Phases/WeekRuleSteps.cs` L19/L23), 22 ist eine
  Marke. Hinweis für den Plan R7 (kein Befund): Aufruf 19 hat den Abbruchweg B (`0281`, `AL` = 1 → Rückkehr), der mit `20B3`
  im Ablauf abzubilden ist.
- **`STATUS.md` M6/M9/M10:** Inhalte decken sich mit Archiv und CHANGELOG (Wetter/Produktion/Schwund R3, Ernte R6; `1B03` 44
  Routinen; `111D:0009`, `17CB:0005`, Halt an Aufruf 18).

## 3. Ausgeführte Prüfungen

Checkout: `git rev-parse HEAD` = `574a12addaa2a97e945bb267883d3bb18d5ac17c`, `git status --porcelain` leer. Zugriff: `git
ls-remote` auf `traders-redone` (`e711ddf`) und `traders-referenz` (`7fcfb24`) erfolgreich. Originaldaten über
`tools/claude/fetch-reference.sh original`, SDK über `tools/claude/setup-workspace.sh`.

```
python3 tools/abdeckung.py --check   → Abdeckung aktuell. (rc 0)
tools/pruefen.sh --schnell           → Ergebnis: grün
gh api repos/kaaasi/traders-redone/pulls/<nr>, /issues/<nr>, /issues?state=open, /issues?state=closed&since=2026-11-14
gh api repos/kaaasi/traders-redone/commits/574a12a…/check-runs   → linux success (windows, downloads skipped)
```

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| Mit Originaldaten (TR_REQUIRE_GAMEDATA=1) | 1041 | 0 | 0 |

Hashes, Architekturgrenzen, Abdeckung, Restore (Lockdateien), Build ohne Warnungen grün. Marker gesamt `[ORIGINAL]` 521,
`[APPROX]` 40 (unverändert gegenüber `main`, der PR ändert keinen Code).

**Akzeptierte `[APPROX]`:** keine neuen (reiner Doku-PR). **Annahmen:** Die Pflege von §2 außerhalb des erzeugten Blocks
gehört zum Rundenabschluss (Vorbild R5-Zeilen); Gegenbeispiele waren keine verlangt.
