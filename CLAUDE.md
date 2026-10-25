# Arbeitsregeln für dieses Repo

- Sprache der Doku und Commits: Deutsch. Code-Bezeichner und Code-Kommentare: Englisch.
- Original ist die Wahrheit: `original/dos/T.EXE` (SHA-256 b73a2170…af0185). Jede portierte
  Routine bekommt einen Befund in `docs/routines/` (Vorlage `_vorlage.md`) und einen
  Differenztest gegen das Original.
- `Traders.Core` bleibt deterministisch und plattformfrei: kein Rendering, keine Uhr,
  keine Zufallsquelle außer dem portierten Original-RNG.
- Rendern/Animation darf keinen Zufall verbrauchen und nichts buchen.
- 16-Bit-Semantik über `Int16Math`, nicht über „saubere“ Neurechnung.
- `STATUS.md` (Status je Meilenstein/Ressource/Modul/Routine: ✅ / 🟢 / 🟡 / 🔍 / ⬜) und
  `CHANGELOG.md` (Zwischenstände mit Commit) pflegt **nur die Leitstelle** bei jeder
  Übernahme nach `main`. Tags kann keine Claude-Sitzung pushen; der Nutzer setzt sie bei Bedarf.

## Rollen: Leitstelle und Paket-Chats

Mehrere Chats arbeiten gleichzeitig an diesem Repo.

**Leitstelle** (ein zentraler Chat, portiert selbst nicht): vergibt Arbeitspakete, prüft
Paket-Branches und übernimmt sie als **einziger** nach `main` (Merge-Commit
„Integration APn: …“). Pflegt `STATUS.md`, `CHANGELOG.md`, `docs/ARBEITSPAKETE.md`,
CI und gemeinsame Projektdateien. Prüfwerkzeug: `tools/pruefe-branch.py`.

**Paket-Chats** (je Arbeitspaket einer):
1. Vor Beginn [`docs/ARBEITSPAKETE.md`](docs/ARBEITSPAKETE.md) lesen; nur das zugewiesene Paket bearbeiten.
2. **Nie nach `main` pushen.** Nur auf den eigenen Branch `dev/apN-…`.
3. Nur im eigenen Dateibereich schreiben (siehe `ci/arbeitspakete.json`). Gemeinsame Dateien
   (`STATUS.md`, `CHANGELOG.md`, `docs/ARBEITSPAKETE.md`, `TradersRedone.slnx`,
   `Directory.Build.props`, `global.json`, `.github/`, `tools/ci-*`, `original/`,
   `docs/analyse/disasm/`) nicht ändern. Braucht ein Paket eine Änderung dort, im
   Übergabebericht unter „Anfragen an die Leitstelle“ beschreiben.
4. Neuen Stand von `main` per `git fetch origin && git merge origin/main` holen
   (kein Rebase bereits gepushter Commits, kein Force-Push – beides ist gesperrt).
5. Übergabebericht `docs/uebergabe/APn.md` (Vorlage `docs/uebergabe/_vorlage.md`) aktuell
   halten: Stand, erledigte/offene Punkte, Status-Vorschläge für `STATUS.md`, Anfragen.
   Ist ein Zwischenstand übernahmereif, dort „Bereit zur Übernahme: ja“ setzen und CI grün haben.

Referenzbilder aus den Videos: `reference/`. Dichtere Bildfolgen aus den Videos kann nur die
Leitstelle erzeugen → als Anfrage im Übergabebericht.

## Bauen und Testen (ohne lokales .NET)

Die Claude-Sitzung hat kein .NET-SDK; gebaut und getestet wird über GitHub Actions.
Logs/Artefakte sind über den Proxy nicht lesbar, deshalb schreibt CI alles in den Branch
**`ci-results`**:

1. Commit pushen (Paket-Chats: eigener `dev/apN-…`-Branch).
2. `tools/ci-wait.sh` ausführen (Hintergrund): wartet auf alle Checks, holt den Bericht nach
   `.ci-results/` und gibt `SUMMARY.md` aus (Schritte, Testergebnisse, Fehlermeldungen).
3. Details: `.ci-results/<job>/logs/*.log`, Bilder unter `.ci-results/<job>/images/`
   (Asset-Export, Viewer-Screenshots), Experiment-Ausgaben unter `.../experiment/`.
4. Analysen mit .NET: `ci/experiment.sh` anlegen (siehe `ci/README.md`), bevorzugt auf `dev/...`.

Jobs: `linux` (Build, Tests, Export, Viewer-Screenshots unter xvfb, Experiment),
`windows` (Build, Tests, startfertiger Viewer als Artefakt für den Nutzer), `report`.
