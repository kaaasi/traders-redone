# Hinweise für Claude

Die Projektregeln stehen werkzeugneutral in [AGENTS.md](AGENTS.md) und gelten vollständig – vor Arbeitsbeginn lesen.
Hier stehen nur Hinweise zu Claudes Arbeitsumgebung. Bei Widerspruch gilt AGENTS.md.

## Umgebung einrichten (jede neue Sitzung)

```bash
git clone https://github.com/kaaasi/traders-redone      # bzw. vorhandenen Klon nutzen
cd traders-redone
tools/claude/fetch-reference.sh original                 # Originaldateien aus kaaasi/traders-referenz, Verknüpfung gamedata/
tools/claude/setup-workspace.sh                          # SDK + NuGet-Spiegel aus dem Release "claude-werkzeuge"
source ~/claude-tools-traders/env.sh
tools/pruefen.sh --schnell
```

- **Referenz-Repo:** `kaaasi/traders-referenz` per `add_repo` einbinden (access `read`; Leitstelle immer `push`), falls
  `git ls-remote https://github.com/kaaasi/traders-referenz HEAD` scheitert. Weitere Teile bei Bedarf:
  `tools/claude/fetch-reference.sh disasm|refemu|videos` (ein vorhandener Klon wird ergänzt). Rückfallweg ohne Zugriff: den
  Nutzer bitten, die DOS-Dateien anzuhängen, und sie nach `gamedata/dos/` legen.

Microsoft- und NuGet-Server sind aus der Arbeitsumgebung gesperrt, GitHub nicht. Fehlt das Release oder sind Pakete
veraltet: `gh api -X POST repos/kaaasi/traders-redone/actions/workflows/werkzeuge.yml/dispatches -f ref=main` (läuft auch
automatisch, wenn sich Projekt- oder Lockdateien auf `main` ändern). Details: [tools/claude/README.md](tools/claude/README.md).

## GitHub aus der Sitzung

- GraphQL ist gesperrt → `gh api` (REST) statt `gh pr …`/`gh release …`. PR anlegen:
  `gh api repos/kaaasi/traders-redone/pulls -f title=… -f head=<branch> -f base=main -F draft=true -f body=…`.
- Draft-Status: „Ready for review“ mit `gh api -X POST repos/kaaasi/traders-redone/pulls/<nr>/ccr/ready_for_review`, zurück mit
  `…/ccr/convert_to_draft`.
- Nur repository-bezogene Endpunkte sind erreichbar; Releases entstehen nur über Workflows.
- Mergen (nur als Leitstelle, nach der Merge-Prüfung in `docs/prozess/zusammenarbeit.md` §6) immer mit dem geprüften Head-SHA:
  `gh api -X PUT repos/kaaasi/traders-redone/pulls/<nr>/merge -f sha=<head> -f merge_method=merge`.
- Commit-Trailer: die Zeilen aus der Sitzungsvorgabe, sonst `Co-Authored-By: Claude <noreply@anthropic.com>`.
- **Leitstelle:** immer Lese- und Schreibzugriff auf beide Repos (`add_repo` für `traders-redone` und `traders-referenz`, jeweils
  access `push`), z. B. für neue Ablagen im Referenz-Repo (dort per PR, Branch `daten/…`). Der Nutzer hat das Einbinden von
  `kaaasi/traders-referenz` für jede Leitstelle, auch künftige, ausdrücklich erlaubt (11.11.2026, bestätigt 12.11.2026, `leitstelle-start.md` Schritt 0); lehnt die Werkzeugfreigabe trotzdem ab, hilft die Bitte an den Nutzer um die Anweisung im Chat (bewährter Weg in Schritt 0, 13.11.2026).
- **Geplante Läufe** (Anlegen nur nach Einbinden beider Repos in der anlegenden Sitzung; Modell, Warten, Laufende) und **Git im flachen Klon**: `docs/prozess/anhang.md` F, G.

## Rolle in diesem Projekt

Die **Leitstelle** ist eine Claude-Sitzung je Runde (`docs/vorlagen/leitstelle-start.md`): plant, verteilt Spuren als geplante
Läufe, integriert, gibt dem Nutzer die Review-Anfrage für ChatGPT als ausgefüllten Kurzaufruf und wertet `docs/reviews/` aus.
Sie portiert nicht selbst (Ausnahme: Repositorypflege im Nutzerauftrag, dann mit unabhängigem Review). Antworten knapp nach
`AGENTS.md` §7.
