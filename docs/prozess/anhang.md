# Anhang zur Prozessdoku – Sonderfälle

Nur bei Bedarf lesen. Ergänzt [zusammenarbeit.md](zusammenarbeit.md) und die Vorlagen in `docs/vorlagen/`; bei Widerspruch gilt
`AGENTS.md`. Die Leitstelle liest den Anhang vollständig.

## A. Vertretung der Leitstelle

1. Der Nutzer ernennt ausdrücklich einen anderen Agenten (Eintrag in `zusammenarbeit.md` §1 per PR, sobald möglich).
2. Die neue Leitstelle liest `AGENTS.md`, `zusammenarbeit.md`, diesen Anhang, `docs/arbeitspakete.md` (aktuelle Runde), die offenen
   PRs mit Übergabekommentaren und die letzten Reviews – mehr ist nicht nötig.
3. Gleiche Regeln: Merge-Prüfung, Review durch eine andere Sitzung. **Hat sie selbst umgesetzt, gibt sie nicht selbst frei.**
4. Kann der Agent nicht mergen, bereitet er die Merge-Prüfung als PR-Kommentar vor; der Nutzer merged (§6 Nr. 7 beachten).

Es gibt **keine automatische Integration**, nur weil ein Kontingent erschöpft ist.

## B. GitHub Free: was gilt, was nicht

| Funktion | Lage im privaten Repo | Folge |
|---|---|---|
| Branch-Schutz, Rulesets, Pflicht-Checks, Pflicht-Reviews | nicht verfügbar | Prozessregeln (§6), keine technische Sperre |
| Merge per REST mit `sha` | verfügbar | einzige serverseitige Bindung an den geprüften Head |
| Draft-PRs | verfügbar | Sperre für Branches; CI baut Drafts nicht |
| Issues, Labels | verfügbar | Befunde als Issues (§5) |
| GitHub Actions | 2.000 Minuten/Monat (Windows zählt doppelt), 500 MB Artefakte | Windows und Downloads nur per Hand; Artefakte 7 Tage; bezahlte Actions-Nutzung im Konto deaktiviert lassen (Ausgabenlimit 0) |

## C. Gesamtreview am Rundenende (risikoorientiert)

Ziel: Entspricht der Port dem Original hinreichend, und enthalten die Änderungen der Runde konkrete Fehler? Prüflauf:
`tools/pruefen.sh --schnell` auf dem Head; ohne Originaldaten im Bericht „Originaldaten nicht geprüft“. Die Leitstelle setzt im Kurzaufruf hinter „Besonders prüfen:“ diesen Absatz ein und ergänzt die
riskantesten Änderungen der Runde:

> Risikoorientiert, keine Vollinventur: Prüfe bevorzugt riskante Änderungen und Übergänge – Zufallsverbrauch und -reihenfolge,
> 16-Bit-Arithmetik, Phasenwechsel, Spielstand, Geld und Vorräte, KI-Entscheidungen, regelrelevante Pixel – und die Entscheidungen der
> Runde. Nutze den RefEmu und `T.EXE` gezielt dort, wo sie eine relevante Frage klären. Führe die genannten Gegenbeispiele erneut aus.
> Mutationen dienen der Untersuchung konkreter Risiken; keine Mutation jedes Tests. Melde Testlücken als solche (Kennzeichen
> „Testlücke“), getrennt von nachgewiesenen Fehlern. Bekannte `[APPROX]` und erwartet offene Issues nur bestätigen. Höchstens 120
> Zeilen; reicht eine Sitzung nicht, Bericht mit Stand ablegen, Fortsetzung als neue Datei `<datei>-teil<n>.md` auf neuem Branch.

Die umfassende Prüfung aller Routinen, Tests und Entscheidungen erfolgt einmal als **Abschlussprüfung** vor v1.0 (M15).

## D. Aufrufe für ChatGPT/Codex

Codex klont das Repo beim Start mit dem dort gewählten Branch und legt den PR selbst an. Reviews laufen **mit Netzzugang**; nur dann
setzt Codex Labels, sonst wartet die Leitstelle auf den Review-PR `review/*` und setzt das Label beim Auswerten.

1. **Head nur auf dem gewählten Branch:** Der Head-SHA ist der Stand des Branches, den der Nutzer in Codex wählt; liegt er auf
   einem anderen Branch, nennt die Leitstelle diesen Branch.
2. **Ablage immer neu:** neuer Branch `review/<thema>`, neue Datei, neuer PR – auch bei einer Fortsetzung (`…-teil<n>.md`).
3. **Keine Netzbefehle im Aufruf** (kein `git fetch`, `ls-remote`, `add_repo`). SDK und Originaldaten besorgt die Codex-Umgebung
   (Setup-Skript bzw. GitHub-Connector für `kaaasi/traders-referenz`, nach `gamedata/`); fehlen sie, gehört das als offener
   Prüfbereich in den Bericht („Originaldaten nicht geprüft“).
4. Der Zugriffsabsatz aus `spur-start.md` gilt nur für Claude-Läufe.

## E. Review ohne vollständigen Klon

Fehlen Commit oder Dateien im Checkout und scheitert der Fetch, einen verfügbaren GitHub-Connector nutzen und den Checkout darüber
rekonstruieren. Den vollständigen Git-Tree gegen den Zielcommit verifizieren (Tree-SHA, z. B. `git add -A && git write-tree`).
Ist das nicht erbracht, gilt der Stand als „unverifiziert“: Läufe daran sind Diagnose, kein Test des exakten Heads.

## F. Geplante Läufe – Leitstelle

- **Zugriff:** Läufe erhalten nur die Repos, die die **anlegende Sitzung** eingebunden hat (`add_repo` steht im Lauf nicht
  verlässlich bereit); vor dem ersten `create_trigger` `leitstelle-start.md` Schritt 0 erledigt (beide Repos `push`) und
  `git ls-remote https://github.com/kaaasi/traders-referenz HEAD` erfolgreich – sonst keinen Lauf anlegen und dem Nutzer melden.
  Jeder Startaufruf enthält den Zugriffsabsatz aus `spur-start.md`. Legt der Nutzer den Lauf selbst an, gibt er beide Repos
  mit (Code-Repo mit Schreibzugriff, Referenz-Repo mindestens lesend). Fehlt ein Repo, versucht der Lauf den `add_repo`-Rückfall
  aus dem Startaufruf; fehlt das Werkzeug oder lehnt die Freigabe ab, meldet er sich blockiert (Spur 0 in R2, 11.11.2026).
  `git ls-remote` belegt nur Leserecht; Schreibrecht zeigt erst der erste Push.
- **Modell:** nach dem Anlegen mit `update_trigger` setzen, wenn es von Opus abweicht (nur für Mechanisches nach
  `zusammenarbeit.md` §5 „Modellwahl“ oder auf Wunsch des Nutzers).
- **`spur-wartet` durchsehen:** „Fortsetzung anlegen – Leitstelle“ heißt, die Aufgabe mit dem Prompt aus der Übergabe anlegen –
  vorher gegen den Kurzaufruf „Geplanter Lauf mit Teilen“ prüfen; abweichen dürfen nur Spur, Modell, Zähler und PR-Nummer.
  Draft-Status, den ein Lauf nicht aufheben konnte, hebt die Leitstelle selbst auf.
- **Warten über Labels:** je Label eine Abfrage (`gh api 'repos/kaaasi/traders-redone/issues?labels=<label>&state=open'`; mehrere
  Labels in einer Abfrage sind UND-verknüpft), bei Reviews durch ChatGPT/Codex zusätzlich offene PRs mit Head `review/*`;
  Intervall 60 s, Schleife im Hintergrund, Obergrenze rund zwei Stunden.
- **Ende eines Laufs:** Läufe melden ihr Ende nicht. Vor jeder Statusauskunft `list_triggers` (`last_run.status`) prüfen, nicht nur
  den PR; ein Draft-PR heißt nicht „läuft noch“.

## G. Claude: Git im flachen Klon

- Fetch-Regeln für einzelne Branches brechen `git fetch` ab, sobald der Branch auf GitHub gelöscht ist („couldn't find remote ref“)
  – dann `git config --unset-all remote.origin.fetch 'refs/heads/<b>:'`.
- Meldet der Stop-Hinweis „unpushed commits / no remote branch“, obwohl gepusht ist, fehlt nur das Tracking:
  `git config branch.<b>.remote origin`, `branch.<b>.merge refs/heads/<b>` und Fetch-Regel
  `+refs/heads/<b>:refs/remotes/origin/<b>`, dann `git fetch`.
- Vor einem Push aus einem alten flachen Klon `git fetch origin main` (sonst lädt der Push den ganzen Baum hoch, HTTP 413).
