# Vorlage: Start einer Spur

**Kurzaufruf** (das ist alles, was der Nutzer in den neuen Chat schreibt):

> Repo kaaasi/traders-redone. Arbeite nach `docs/vorlagen/spur-start.md`. Spur: `<name>`. Agent: `<Claude | ChatGPT | …>`.
> Besonderheiten: `<nur falls nötig, sonst weglassen>`

**Geplanter Lauf mit Teilen** (Startaufruf der Leitstelle; eine Fortsetzung legt ebenfalls die Leitstelle an und hängt
`Fortsetzung <n> von 3, Übergabe in PR #<nr>.` an):

> Repo kaaasi/traders-redone. Arbeite nach `docs/vorlagen/spur-start.md`. Spur: `<name>`. Agent: Claude.
> Unbeaufsichtigt, keine Rückfragen. Teile nach Abschnitt „Teile und Fortsetzung“. Keine Aufgaben anlegen (kein `create_trigger`):
> Fortsetzungen, Nachprüfungen und Reviews legt allein die Leitstelle an (Nutzer 12.11.2026); am Ende Label `spur-wartet` und
> Übergabe mit fertigem Prompt. Modell der Spur: `<modell>`.
> Zugriff zuerst prüfen: `git ls-remote https://github.com/kaaasi/traders-redone HEAD` und
> `git ls-remote https://github.com/kaaasi/traders-referenz HEAD` (prüft nur Lesen). Scheitert eines, `add_repo` (ggf. erst per `ToolSearch` mit
> `select:mcp__claude-code-remote__add_repo` laden; meist nicht vorhanden) für `kaaasi/traders-redone` (access push) bzw.
> `kaaasi/traders-referenz` (access read) – beides ausdrücklich autorisiert – und erneut prüfen; bleibt eines unerreichbar,
> sofort blockiert melden (genaue Fehlermeldungen), keine Code-Arbeit; gibt es einen PR der Spur, dort Kommentar und Label
> `spur-wartet`.

**Anlegen** (Leitstelle oder Nutzer): dem Lauf beide Repos mitgeben, `kaaasi/traders-redone` mit Schreibzugriff und
`kaaasi/traders-referenz` mindestens lesend; der `add_repo`-Weg im Startaufruf ist nur der Rückfall.

**Einzelner Folgeteil:** wie oben mit dem Zusatz „Nur Teil <n>; danach Schritt 5 bzw. 6 ohne weiteren Teil.“

**Fixlauf** (geplanter Lauf): Startaufruf wie oben mit Auftrag „Befunde `<IDs>` aus `<review-datei>` zu PR #`<nr>` beheben, Head
vorher `<sha>`“ – ohne Befund (nur Rebase): „PR #`<nr>` auf `main` rebasen, Head vorher `<sha>`“. Am Ende
(`zusammenarbeit.md` §5 Punkt 8): bei behobenem P0/P1 Label `spur-wartet` und in der Übergabe „Nachprüfung anlegen –
Leitstelle“ mit fertigem Kurzaufruf aus `review-anfrage.md` (Delta alter..neuer Head, nur diese Befunde; nicht selbst anlegen),
sonst Label `leitstelle-delta`. Mit dem Push des neuen
Heads `review-nein` entfernen. Widerspricht der Lauf einem Befund mit eigenem Beleg: `beleg-strittig`, beide Belege in der Übergabe.

## Ablauf für den Agenten

1. Lies `AGENTS.md`, `docs/prozess/zusammenarbeit.md` §2–4 und in `docs/arbeitspakete.md` **nur** die aktuelle Runde und deine
   Spur. Archiv, alte Reviews und `docs/prozess/anhang.md` nur bei konkretem Bedarf.
2. Einrichten nach `AGENTS.md` §3 (Claude: zusätzlich `CLAUDE.md`), Originaldaten nach `gamedata/`
   (`tools/claude/fetch-reference.sh original`). Läufe der Leitstelle haben dort auch Schreibrecht, Spuren schreiben dort
   trotzdem nie; fehlen Daten oder Bilder, im PR melden – ablegen nur Leitstelle oder Nutzer.
3. Offener PR der Spur mit Bearbeiter? Nur nach Übergabekommentar weiterarbeiten (Übernahme: `zusammenarbeit.md` §2), sonst den
   Nutzer fragen (geplante Läufe: „Teile und Fortsetzung“ Schritt 1). Neu: Branch `paket/<spur>` von aktuellem `main`, sofort
   Draft-PR mit `Bearbeiter: <Agent>` und Base-SHA.
4. Harte Regeln: Befund in `docs/routines/` → Differenztest gegen RefEmu (rot bzw. fehlend) → Port → grün; Dateigrenzen;
   `[ORIGINAL]`/`[APPROX]`; Mehrrundenprobe, wenn Phasenfolge, Geld, Vorräte oder KI berührt sind.
5. Vor jedem Push: `git fetch origin && git rebase origin/main`, `tools/pruefen.sh --schnell` (`AGENTS.md` §5).
6. Fertig: Beschreibung nach `.github/pull_request_template.md`, Draft aufheben, Status im eigenen Abschnitt von
   `docs/arbeitspakete.md`.
7. **Immer zum Schluss** (fertig wie unterbrochen): Übergabekommentar nach `docs/handoffs/VORLAGE.md` (`Bearbeiter: frei`).
8. Antwort an den Nutzer **knapp** (`AGENTS.md` §7): PR-Link, Ergebnis in einem Satz, offene Fragen.

## Teile und Fortsetzung (geplante Läufe)

Gilt, wenn die Spur in `docs/arbeitspakete.md` eine geordnete Liste von Teilen hat. **Läufe legen nie selbst Aufgaben an**
(`create_trigger`); jede Fortsetzung legt die Leitstelle an (Nutzer 12.11.2026).

1. **Stand ermitteln:** offenen PR (`paket/<spur>`) und letzten Übergabekommentar lesen; dort steht „Fortsetzungen verbraucht:
   n von 3“ (nennt der Startaufruf einen anderen Zähler, gilt der höhere). Bearbeiter ≠ `frei` ohne Übergabekommentar **nach**
   dessen letztem Bearbeiter- oder Übernahme-Eintrag: nichts am Branch ändern, nur Label `spur-wartet` und Hinweiskommentar,
   keine Fortsetzung, Lauf beenden.
2. **Ein Branch, ein PR:** Teil 1 legt `paket/<spur>` von aktuellem `main` an und öffnet den Draft-PR mit der Teilliste als
   Aufgabenliste. Jeder Teil sind Commits auf diesem Branch; eine Fortsetzung übernimmt ihn nach der Übergabe – nacheinander, nie
   gleichzeitig. Vor dem Push auf `origin/main` rebasen, dann `git push --force-with-lease`.
3. **Teil abschließen:** grünes `tools/pruefen.sh --schnell`, gepusht, Teil im PR abgehakt, Übergabe mit „Fortsetzungen verbraucht: n von 3“.
   Der PR bleibt Draft, bis alle Teile fertig sind; dann den nächsten Teil im selben Lauf beginnen, solange Zeit bleibt.
4. **Teil blockiert** (kein grünes `pruefen.sh --schnell`, Beleg fehlt, fremde Datei nötig, Frage an den Nutzer): Draft mit Übergabe, Label
   `spur-wartet`, in der Übergabe „blockiert – Entscheidung Nutzer“ und Grund. Keine Fortsetzung.
5. **Fortsetzung anmelden** (nur wenn Teile offen sind, nichts blockiert und n < 3): Label `spur-wartet`, in der Übergabe
   „Fortsetzung anlegen – Leitstelle“ mit fertigem Prompt (Startaufruf oben mit `Fortsetzung <n+1> von 3, Übergabe in PR #<nr>.`)
   und „Modell der Fortsetzung: <modell>“. Selbst keine Aufgabe anlegen.
6. **Kettenende:** Alle Teile fertig → Beschreibung vervollständigen, „Ready for review“
   (`gh api -X POST repos/kaaasi/traders-redone/pulls/<nr>/ccr/ready_for_review`; scheitert es, Fehlermeldung in der Übergabe),
   `spur-wartet`, Übergabe „Spur fertig – Review“. n = 3 und Teile offen: Draft lassen, `spur-wartet`, „Fortsetzungen erschöpft –
   Entscheidung Nutzer“. Erstreview und Merge legt die Leitstelle an.
7. Nie eine fremde Spur fortsetzen oder starten.
