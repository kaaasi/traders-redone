# Arbeitsregeln für dieses Repo

- Sprache der Doku und Commits: Deutsch. Code-Bezeichner und Code-Kommentare: Englisch.
- Original ist die Wahrheit: `original/dos/T.EXE` (SHA-256 b73a2170…af0185). Jede portierte
  Routine bekommt einen Befund in `docs/routines/` (Vorlage `_vorlage.md`) und einen
  Differenztest gegen das Original.
- `Traders.Core` bleibt deterministisch und plattformfrei: kein Rendering, keine Uhr,
  keine Zufallsquelle außer dem portierten Original-RNG.
- Rendern/Animation darf keinen Zufall verbrauchen und nichts buchen.
- 16-Bit-Semantik über `Int16Math`, nicht über „saubere“ Neurechnung.
- **`STATUS.md` bei jedem Push aktuell halten**: Meilensteine, Ressourcen, Module und
  einzelne Routinen mit Status (✅ / 🟢 / 🟡 / 🔍 / ⬜).
- Zwischenstände pro Meilenstein in `CHANGELOG.md` mit Commit festhalten. Tags (`v0.x`)
  kann die Claude-Sitzung nicht pushen; der Nutzer setzt sie bei Bedarf.

## Paralleles Arbeiten in mehreren Chats

Mehrere Chats arbeiten gleichzeitig an diesem Repo. Vor jeder Arbeit
[`docs/ARBEITSPAKETE.md`](docs/ARBEITSPAKETE.md) lesen: ein Paket übernehmen (eigene Zeile
eintragen und sofort pushen), nur auf dem eigenen Branch `dev/apN-…` und im eigenen
Dateibereich arbeiten, gemeinsame Dateien nur minimal ändern, regelmäßig auf `origin/main`
rebasen. Nach `main` nur mit grüner CI und per Rebase (kein Merge-Commit, kein Force-Push).
Referenzbilder aus den Videos: `reference/` (die Videos selbst sind nicht im Repo).

## Bauen und Testen (ohne lokales .NET)

Die Claude-Sitzung hat kein .NET-SDK; gebaut und getestet wird über GitHub Actions.
Logs/Artefakte sind über den Proxy nicht lesbar, deshalb schreibt CI alles in den Branch
**`ci-results`**:

1. Commit pushen (`main` oder `dev/<thema>`).
2. `tools/ci-wait.sh` ausführen (Hintergrund): wartet auf alle Checks, holt den Bericht nach
   `.ci-results/` und gibt `SUMMARY.md` aus (Schritte, Testergebnisse, Fehlermeldungen).
3. Details: `.ci-results/<job>/logs/*.log`, Bilder unter `.ci-results/<job>/images/`
   (Asset-Export, Viewer-Screenshots), Experiment-Ausgaben unter `.../experiment/`.
4. Analysen mit .NET: `ci/experiment.sh` anlegen (siehe `ci/README.md`), bevorzugt auf `dev/...`.

Jobs: `linux` (Build, Tests, Export, Viewer-Screenshots unter xvfb, Experiment),
`windows` (Build, Tests, startfertiger Viewer als Artefakt für den Nutzer), `report`.
