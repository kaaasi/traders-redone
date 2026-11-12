# Zusammenarbeit mehrerer Agenten

Gilt für alle Agenten und ergänzt [AGENTS.md](../../AGENTS.md). Jeder geeignete Agent kann ein Paket, ein Review oder – nach
Freigabe des Nutzers – die Leitstelle übernehmen, **nur anhand dessen, was in GitHub steht**; kein Schritt setzt einen Chatverlauf
voraus. Sonderfälle: [anhang.md](anhang.md). Vorbild ist der Ablauf in `kaaasi/eishockey-manager-redone` (Stand 10.11.2026).

## 1. Rollen

| Rolle | Aufgaben | Darf nicht |
|---|---|---|
| **Nutzer** (Projektinhaber) | Ziele, Go für Rundenpläne, Entscheidungen, Ernennung der Leitstelle, letzte Entscheidung | – |
| **Leitstelle** (vom Nutzer ernannt) | Runden planen, Spuren schneiden und als geplante Läufe starten, Merge-Prüfung, Review-Anfragen, `STATUS.md`/`CHANGELOG.md`/Plan pflegen | selbst portieren; eigene Änderungen (auch Integrations-Commits) ohne unabhängiges Review mergen |
| **Implementierer** | eine Spur auf **einem** Branch `paket/<spur>` mit **einem** PR | eigenen PR freigeben oder mergen; fremde Branches ohne Übergabe ändern |
| **Reviewer** (anderer Agent oder frische Sitzung ohne Implementierungskontext) | unabhängiges Review mit festen SHAs, Datei in `docs/reviews/` | Code oder den geprüften PR ändern (außer Label `review-ja`/`review-nein`) |

**Aktuelle Leitstelle:** Claude, je Runde eine frische Sitzung (Nutzer 10.11.2026); sie merged geprüfte PRs selbst (Nutzer 11.11.2026); Stand nur in GitHub (`docs/arbeitspakete.md`,
PRs, Labels). **Spuren und Erstreviews:** Claude (geplante Läufe). **Gesamtreview am Rundenende:** ChatGPT. Einen Wechsel
entscheidet der Nutzer; er wird hier per PR eingetragen (Datum, neue Rolle, Grund).

## 2. Spur übernehmen oder beginnen

Übernahmefähig ist eine Spur, wenn in `docs/arbeitspakete.md` oder im Draft-PR stehen: Ziel, Teile, Abnahme mit Differenz- bzw.
Regressionstest; Branch, Base-SHA, Dateigrenzen, Abhängigkeiten; Modell; Stand (Draft-PR, letzter Head-SHA, Bearbeiter). Ablauf:
`docs/vorlagen/spur-start.md`. **Bearbeiter ohne Übergabekommentar → nicht anfangen**, den Nutzer fragen (geplante Läufe:
`spur-start.md` „Teile und Fortsetzung“ Schritt 1). Wer übernimmt, vergleicht `git log -1` mit dem Head-SHA der letzten Übergabe
und arbeitet erst bei gleichem Stand und grünem `tools/pruefen.sh` weiter. Neu: Branch von aktuellem `main`, sofort **Draft-PR**
(Titel `Spur <name>: …`, `Bearbeiter: <Agent>`, Base-SHA) – er ist die Sperre für den Branch. Fertig: Beschreibung nach Vorlage,
„Ready for review“.

## 3. Übergabe zwischen Agenten

**Jede Arbeitssitzung endet mit einem Übergabekommentar** im PR nach [docs/handoffs/VORLAGE.md](../handoffs/VORLAGE.md) – nach
erledigter Arbeit wie bei Abbruch (Kontingent, Sitzungsende, Umverteilung); bei absehbarem Abbruch **vorher**. Alles ist gepusht,
der Kommentar nennt **Head-SHA**, erledigt/offen/nächsten Schritt, Testtabelle, Risiken, neue `[APPROX]` und `Bearbeiter: frei`
(oder den Nachfolger). **Höchstens 15 Zeilen plus Testtabelle**; was in PR oder Review steht, wird verlinkt.

Ohne PR (z. B. reine Analyse): `docs/handoffs/<datum>-<thema>.md` auf eigenem Branch. Abbruch ohne Kommentar: Es gilt der letzte
gepushte Stand; der Nachfolger schreibt zuerst einen Übernahmekommentar („übernommen ab SHA …, Tests …“). Sitzungslinks
(z. B. im Commit-Trailer) sind nur Hinweise, keine Berechtigung und keine Übergabe.

## 4. Parallele Arbeit ohne Überschreiben

- **Ein Branch – ein Bearbeiter.** Je gleichzeitigem Agenten ein eigener Klon bzw. Worktree. Fortsetzungen einer Spur arbeiten
  **nacheinander** auf `paket/<spur>` mit einem PR (`spur-start.md` „Teile und Fortsetzung“).
- Kein `git push --force` auf fremde Branches; auf eigenen nur `--force-with-lease` nach Rebase.
- Dateigrenzen einhalten; Berührungen gemeinsamer Dateien im PR begründen. Überschneidende Spuren laufen nacheinander.
- `docs/arbeitspakete.md` nur im eigenen Statusabschnitt; Planabschnitte ändert die Leitstelle.
- Abgeschlossene Branches erst löschen, wenn geprüft ist, dass keine unintegrierte Arbeit darauf liegt (`git log main..<branch>`).
- Status nur in GitHub (`docs/arbeitspakete.md`, PRs, Labels, `STATUS.md`); ein GitHub-Project ist optional und darf keine
  zweite, von Hand gepflegte Statusquelle werden.
- Geplante Läufe und Hintergrundagenten nur durch die Leitstelle oder als im Startaufruf autorisierte Fortsetzung, Nachprüfung bzw.
  Review – sonst nicht ohne Auftrag des Nutzers.

## 5. Unabhängiges Review

- Die Leitstelle legt das Review als geplanten Lauf an bzw. gibt dem Nutzer den ausgefüllten **Kurzaufruf** aus
  `docs/vorlagen/review-anfrage.md` (PR, Base- und Head-SHA, Gegenbeispiele, erwartet offen, Ablage).
- Reviewer ist nie die umsetzende Sitzung; hat derselbe Agent umgesetzt, prüft eine **neue** Sitzung ohne diesen Verlauf.
- Ergebnis: `docs/reviews/JJJJ-MM-TT-<thema>.md` auf `review/<thema>`, PR nur mit dieser Datei; ohne Schreibzugriff Datei an den
  Nutzer, die Leitstelle legt ab.
- Ändert sich der Head nach dem Review, ist eine **Nachprüfung** des Deltas nötig (`…-nachpruefung.md`) – außer in den Fällen von
  Punkt 4 unten.
- **Sparsame Kette:**
  1. **Erstreview** (Spur- oder Integrations-PR): vollständig (Claude Opus) – alle drei Läufe von `tools/pruefen.sh`, Mutationen der neuen Tests,
     Differenztests gegen RefEmu nachvollzogen, Fehlerfälle, Fundstellen in `T.EXE`.
  2. **Fixlauf:** behebt die Befunde, `tools/pruefen.sh --schnell`.
  3. **Nachprüfung** nur, wenn der Fixlauf **P0/P1** behebt: nur Delta, Gegenbeispiele und Rückmutationen dieser Befunde.
  4. **Delta-Prüfung der Leitstelle** statt Nachprüfung, wenn das Delta **nur P2/P3** behebt oder nur Rebase ohne Konflikt ist
     (`git range-diff`): Delta (`compare`/`--stat`), CI auf dem Head, Testtabelle des Fixlaufs; Vermerk im Merge-Kommentar
     („Delta geprüft durch Leitstelle, nur P2/P3“ bzw. „… nur Rebase“).
  5. **P3** bekommt keinen eigenen Lauf: Issue mit Label `P3`, gesammelt an den letzten Lauf der Runde. Ein ohnehin laufender
     Fixlauf darf P3 mitnehmen.
  6. **Gesamtreview** am Rundenende (ChatGPT): risikoorientiert ([anhang.md](anhang.md) C).
  7. **Befund nur mit Beleg:** Wer eine Lesart von `T.EXE`, einen Originalwert oder „fehlt im Port“ behauptet, nennt einen
     **ausführbaren Beleg** (roter Test oder RefEmu-Probe: Befehl, Eingangszustand, Ergebnis; bei „fehlt“ zusätzlich die erfolglose
     Codesuche). Ohne Beleg höchstens P3 „Verdacht“. Widerspricht ein Fixlauf mit eigenem Beleg: Label `beleg-strittig`, keine
     Nachprüfung für diesen Punkt; die Leitstelle führt beide Belege aus und entscheidet.
  8. **Verkettung und Labels:** Der Fixlauf legt bei behobenem P0/P1 selbst die Nachprüfung als geplanten Lauf an (Kurzaufruf aus
     `review-anfrage.md`); lehnt die Freigabe `create_trigger` ab: `spur-wartet`, Prompt im Übergabekommentar. Die Leitstelle
     wartet auf Labels bzw. Review-PRs, nicht auf Kommentare. **Wer ein Label abarbeitet, entfernt es;** die Leitstelle darf Labels jederzeit setzen und entfernen (Nutzer 11.11.2026):

     | Label | gesetzt von, wann | entfernt von, wann |
     |---|---|---|
     | `review-ja` / `review-nein` | jedes Review am geprüften PR (das andere entfernen) | durch das nächste Review; `review-nein` der Fixlauf mit dem Push des neuen Heads |
     | `leitstelle-delta` | Fixlauf, der nur P2/P3 behebt oder nur rebased | Leitstelle nach der Delta-Prüfung |
     | `beleg-strittig` | Fixlauf, der einem belegten Befund mit eigenem Beleg widerspricht | Leitstelle nach ihrer Entscheidung |
     | `spur-wartet` | Lauf ohne Fortsetzung (blockiert, Kettenende, `create_trigger` abgelehnt) | Leitstelle nach Anlegen des Laufs bzw. Entscheidung oder Vorlage beim Nutzer |

     Reviews ohne Netz (ChatGPT/Codex) setzen kein Label; dann wartet die Leitstelle auf den Review-PR (`review/*`) und setzt das
     Label beim Auswerten.
  9. **Länge:** Review-Bericht höchstens 120 Zeilen (Urteil, Befunde mit Beleg, Prüftabelle); PR-Beschreibungen ebenso knapp.

  **Modellwahl:** Claude Opus überall, wo es Nacharbeit vermeidet – Disassembly, Spielkern, RefEmu, Differenztests, Erstreviews
  und Nachprüfungen von Code. Claude Sonnet nur für Mechanisches: reine Doku, Kommentarumstellung, einfache Testergänzungen. Im
  Zweifel Opus. Die Leitstelle setzt das Modell ([anhang.md](anhang.md) F).
- **Befunde als Issues:** je Befund ein Issue `<ID>: <Kurzbeschreibung>`, Labels `befund` und `P0`–`P3`, Link auf die
  Review-Datei; der behebende PR schließt es (`Fixes #<nr>`). Ohne Zugriff auf Issues Befunde im PR nennen; die Leitstelle legt an.

## 6. Merge-Prüfung (vor jedem Merge, von Leitstelle oder Nutzer)

GitHub Free erzwingt nichts davon ([anhang.md](anhang.md) B); die Liste ist verbindlich und wird im Merge-Kommentar abgehakt:

1. [ ] **Head-SHA** = vom Review freigegebener SHA (sonst Nachprüfung; Ausnahmen mit Delta-Prüfung nach §5 Punkt 4).
2. [ ] **Base aktuell:** ohne Konflikt gegen aktuellen `main`; ist `main` seit dem Review neu: `tools/pruefen.sh --schnell` mit Originaldaten auf dem
   Merge-Ergebnis (`main` + PR) lokal wiederholen, Tabelle im Merge-Kommentar.
3. [ ] **CI grün auf genau diesem Head** (Check-Runs des Head-SHA). Nur `docs/**`/`*.md` geändert: CI startet nicht
   (Pfadfilter in `build.yml`) → „entfällt, nur Doku“ nach Bestätigung per `compare`/`--stat`. Gilt nicht für
   `docs/routines/**` und `STATUS.md`: sie sind Eingaben von `tools/abdeckung.py --check`, dafür läuft die CI immer.
4. [ ] **Originaldaten-Nachweis:** Tabelle von `tools/pruefen.sh` mit Originaldaten zum Head (PR, Fixlauf, Review oder CI mit
   `TRADERS_REFERENZ_TOKEN`); bei Integrations-PRs alle drei Läufe einmal. Ein Review „nur Codelesen“, an unverifiziertem Stand
   oder „Originaldaten nicht geprüft“ zählt nicht.
5. [ ] **Review-Freigabe** als Datei in `docs/reviews/`; offene P0/P1/P2 behoben oder vom Nutzer ausdrücklich zurückgestellt.
6. [ ] Kein Draft; Pflichtangaben vollständig; Restlücken in `docs/arbeitspakete.md` übertragen.
7. [ ] Merge **mit SHA-Bindung** (`PUT /repos/kaaasi/traders-redone/pulls/<nr>/merge` mit `sha=<freigegebener Head>`,
   `merge_method=merge`). In der Weboberfläche: Head-SHA unmittelbar vor dem Klick vergleichen.
8. [ ] Danach: CI auf `main` prüfen; Review-PR mergen; Status in `docs/arbeitspakete.md`, `STATUS.md`, `CHANGELOG.md` nachziehen.

## 7. Vertretung der Leitstelle → [anhang.md](anhang.md) A

## 8. GitHub Free → [anhang.md](anhang.md) B

Alle Agenten schreiben über dasselbe GitHub-Konto; wer was gemacht hat, zeigen nur Commit-Trailer, `Bearbeiter:` und
Übergabekommentare – deshalb Pflicht.
