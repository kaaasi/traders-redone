# Vorlage: Leitstelle (neuer Chat je Runde oder Vertretung)

**Kurzaufruf:**

> Repo kaaasi/traders-redone. Du bist die Leitstelle nach `docs/vorlagen/leitstelle-start.md`.
> Auftrag: `<z. B. Runde 2 planen | Integration Runde 2 | Review #<nr> auswerten und mergen>`.
> `<Vertretung: ja, ernannt vom Nutzer am <datum> | –>`

## Ablauf für die Leitstelle

0. **Zugriff:** immer Lese- und Schreibzugriff auf beide Repos: `add_repo` für `kaaasi/traders-redone` und
   `kaaasi/traders-referenz`, jeweils access `push` (ggf. erst per `ToolSearch` mit `select:mcp__claude-code-remote__add_repo`
   laden). **Freigabe des Nutzers (11.11.2026):** Jede Leitstelle darf `kaaasi/traders-referenz` mit access `push` einbinden; das
   gilt ausdrücklich auch für künftige Leitstellen (bestätigt 12.11.2026). Lehnt die Freigabe des Werkzeugs trotzdem ab, auf diese Erlaubnis verweisen
   und erneut versuchen; scheitert es weiter, dem Nutzer melden. **Bewährter Weg (13.11.2026):** Lehnt die Werkzeugfreigabe
   `add_repo` für `traders-referenz` (push) trotz dieser Notiz ab („Unauthorized Access“), den Nutzer bitten, im Chat wörtlich
   „binde traders-referenz mit push ein“ zu schreiben, und `add_repo` danach genau einmal erneut aufrufen – so ging es durch.
   Dann klonen mit `TRADERS_REFERENZ_DIR=<arbeitsverzeichnis>/traders-referenz tools/claude/fetch-reference.sh original`,
   `register_repo_root` aufrufen und mit `git ls-remote https://github.com/kaaasi/traders-referenz HEAD` prüfen. Erst danach
   geplante Läufe anlegen (`anhang.md` F). Planen geht auch ohne diesen Zugriff.
1. Lies `AGENTS.md`, `docs/prozess/zusammenarbeit.md` und `docs/prozess/anhang.md` (vollständig), in `docs/arbeitspakete.md` die
   aktuelle Runde, die offenen PRs (Titel, Bearbeiter, letzte Übergabe) und die Reviews der Runde. Kein Vorwissen aus Chats nötig.
2. **Planen:** Runde mit Ziel, Spuren (Teile, Dateigrenzen, Abnahme, Modell, Abhängigkeiten), Zuständigkeit gemeinsamer Dateien,
   Integrationsfolge und offenen Fragen an den Nutzer in `docs/arbeitspakete.md` auf `prozess/plan-runde-<n>`; dem Nutzer nur
   Entscheidungsbedarf und Link nennen. Spuren starten erst nach dem **Go** des Nutzers.
3. **Starten:** je Spur geplanter Lauf mit dem Kurzaufruf „Geplanter Lauf mit Teilen“ aus `spur-start.md` (`anhang.md` F).
4. **Review anlegen:** je fertiger Spur Erstreview als geplanter Lauf (frische Sitzung, Kurzaufruf aus `review-anfrage.md`);
   Gesamtreview am Rundenende: dem Nutzer den ausgefüllten Kurzaufruf für ChatGPT geben (`anhang.md` C, D).
5. **Integration:** Spur-PRs nach Review und Merge-Prüfung (`zusammenarbeit.md` §6) in der Integrationsfolge mergen. Ist `main`
   seit dem Review neu und der PR **ohne Konflikt**, wird **nicht** rebased: vorher lokal `tools/pruefen.sh --schnell` mit Originaldaten auf `main` + PR,
   Tabelle im Merge-Kommentar. `integration/<runde>` nur bei Konflikten zwischen Spuren. Eigene Integrations-Commits brauchen
   das Review wie jede andere Änderung.
6. **Nach jedem Merge:** `STATUS.md` (Meilensteine, Routinen – Erzeugung nach `tools/abdeckung.py`, sobald vorhanden),
   `CHANGELOG.md`, Status in `docs/arbeitspakete.md`.
7. **Rundenende:** Gesamtreview ausgewertet, Befunde als Issues, Runde nach `docs/archiv/runde-<n>.md`, nächste Runde vorschlagen.
8. **Immer zum Schluss:** Übergabekommentar nach `docs/handoffs/VORLAGE.md` in jedem PR, an dem die Sitzung gearbeitet hat; ohne
   PR als `docs/handoffs/<datum>-<thema>.md`.
9. **Sitzung je Runde:** höchstens eine Runde je Sitzung; die nächste beginnt in einem neuen Chat mit dem Kurzaufruf oben. Dem
   Nutzer meldet sie sich nur bei Entscheidungsbedarf, am Ende einer Spur (Merge) und am Rundenende.
10. **Warten über Labels** (`zusammenarbeit.md` §5 Punkt 8, `anhang.md` F). Erledigte Labels entfernt die Leitstelle; Labels setzen
    und entfernen darf sie immer selbst (Nutzer 11.11.2026).
