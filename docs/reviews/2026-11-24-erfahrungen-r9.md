# Review PR #281 – Prozess: Erfahrungen aus Runde 9

Reviewer: Claude Sonnet (geplanter Lauf, frische Sitzung, nicht am PR beteiligt) · Base `813e83e` · Head `01ec2d5`

## 1. Merge-Empfehlung

**Nein für `01ec2d5`** (nur Doku-Lesen; kein Build, reiner Doku-PR). Zwei Gründe, beide klein:

- P2-1: eine verbliebene Rebase-Anweisung widerspricht dem neuen §6 Nr. 2 (Befund unten).
- CI `linux` auf dem Head war bei Abschluss dieses Reviews noch `in_progress` (Run 45404194247); `zusammenarbeit.md` §6 Nr. 3
  verlangt „grün auf genau diesem Head“. `windows` ist `skipped` (Draft-/Pfadlogik, erwartungsgemäß).

Nach Behebung von P2-1 per Delta-Prüfung der Leitstelle (nur Text) und grüner CI ist der PR freigabefähig.

## 2. Befunde

**P2-1 – §5 Nr. 4 kennt weiter „Rebase“** ([zusammenarbeit.md Z. 77–79](https://github.com/kaaasi/traders-redone/blob/01ec2d5/docs/prozess/zusammenarbeit.md#L77-L79)).
„… oder nur Rebase ohne Konflikt ist (`git range-diff`)“ und Vermerk „… nur Rebase“. §6 Nr. 2 verweist dagegen auf das
Delta „nur Merge von main, §5 Nr. 4“ – diesen Fall nennt §5 Nr. 4 nicht. Folge: Die Delta-Prüfung für `update-branch`/Merge-Commit
hat keine Grundlage, und die Regel „Head-SHA = freigegebener SHA“ (§6 Nr. 1) greift nach `update-branch` formal ins Leere.
Beleg: `grep -rn "rebase\|force-with-lease" AGENTS.md CLAUDE.md docs/prozess docs/vorlagen` → Treffer
`zusammenarbeit.md:77`, `:79` (Rest: `leitstelle-start.md:37`, `spur-start.md:46`, beide verneinend, korrekt). Kein `force-with-lease` mehr.
Fix: „nur Rebase“ → „nur Merge von `main` ohne Konflikt“; `git range-diff` durch `git diff <alt>..<neu> -m --first-parent`
o. Ä. ersetzen (range-diff passt nicht zu Merge-Commits); Vermerk „… nur Merge von main“.

**P3-1 – „Merge-Lauf Haiku“ undefiniert** ([leitstelle-start.md Z. 37](https://github.com/kaaasi/traders-redone/blob/01ec2d5/docs/vorlagen/leitstelle-start.md#L37),
[zusammenarbeit.md Z. 116](https://github.com/kaaasi/traders-redone/blob/01ec2d5/docs/prozess/zusammenarbeit.md#L116)).
`spur-start.md` kennt den Fall als Fixlauf „`main` per Merge-Commit in PR #<nr> holen“, nicht unter diesem Namen; §5 Modelltabelle
nennt kein Haiku für ihn. Kurz verweisen („Fixlauf ohne Befund, Haiku“). Verdacht, kein roter Beleg.

**P3-2 – Wortlaut Force-Push uneinheitlich.** `zusammenarbeit.md` §4: „Kein `git push --force`“ (allgemein); `AGENTS.md` §6: „in
geplanten Läufen gesperrt“. Gleiche Formulierung wählen (AGENTS.md hat Vorrang).

## 3. Ausgeführte Prüfungen

Alle Punkte der PR-Beschreibung gegen den Diff abgeglichen:

| Punkt | Umsetzung | Ergebnis |
|---|---|---|
| 1 Merge-Prüfung Nr. 2 | `zusammenarbeit.md` §6, `leitstelle-start.md` Schritt 5 | konsistent; SHA-Bindung (Nr. 1), Originaldaten-Nachweis, CI-Pflicht (Nr. 3) bleiben; Lücke siehe P2-1 |
| 2 Merge statt Rebase | `AGENTS.md` §6, `zusammenarbeit.md` §4, `spur-start.md` (Z. 28, 46, 64), Label `leitstelle-delta` | umgesetzt; Rest siehe P2-1 |
| 3 Fixlauf außerhalb Plans | `zusammenarbeit.md` §4 | Freigabe beim Nutzer („Kurzsatz“), Anlegen bei der Leitstelle; verträglich mit „Aufgaben legt allein die Leitstelle an“ (12.11.2026) |
| 4 Schnittstellen | `leitstelle-start.md` Schritt 2 | vorhanden |
| 5 Laufzeit/Hintergrund | `leitstelle-start.md` Schritt 4, `spur-start.md` Schritt 5, `arbeitspakete.md` Vorschau R10 | vorhanden; Werkzeug nur als R10-Vorschau (erwartet offen, bestätigt) |
| 6 Doku-PRs bündeln | `leitstelle-start.md` Schritt 6 | vorhanden |
| 7 Ausnahmelisten | `routines/_vorlage.md` | vorhanden; Test erst R10 (erwartet offen, bestätigt) |

- `git diff --check 813e83e 01ec2d5` → leer; `git rev-parse HEAD` = `01ec2d5…`, Arbeitsbaum sauber; Diff nur 6 Doku-Dateien (+30/−15).
- `tools/pruefen.sh` nicht ausgeführt (reiner Doku-PR); Originaldaten nicht geprüft. Hinweis: `build.yml` lädt Originaldaten nur mit Secret, §6 Nr. 2 sollte „Originaldaten: ja/nein“ aus der CI-Zusammenfassung verlangen.
- CI: `linux` laufend, `windows` skipped (Stand Review). Kein `[APPROX]` berührt.
