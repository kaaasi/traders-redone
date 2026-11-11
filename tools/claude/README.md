# Arbeitsumgebung für Claude

Claude arbeitet in einer abgeschotteten Linux-Umgebung, in der die Downloadserver von Microsoft und nuget.org gesperrt
sind, GitHub aber erreichbar ist. Damit Claude trotzdem selbst bauen und testen kann:

1. Der Workflow **„Werkzeuge für Claude“** (`.github/workflows/werkzeuge.yml`) legt das offizielle .NET-SDK (Version aus
   `global.json`, SHA-512 geprüft) und alle NuGet-Pakete der Solution im Release `claude-werkzeuge` ab. Er läuft
   automatisch, sobald sich Projekt- oder Lockdateien auf `main` ändern, und lässt sich per Hand starten:
   `gh api -X POST repos/kaaasi/traders-redone/actions/workflows/werkzeuge.yml/dispatches -f ref=main`.
2. `tools/claude/setup-workspace.sh` lädt beides nach `~/claude-tools-traders/` und richtet die Offline-Paketquelle ein.
3. Danach: `source ~/claude-tools-traders/env.sh` und `tools/pruefen.sh --schnell`.

Die Originaldateien liegen im privaten Repo `kaaasi/traders-referenz`; `tools/claude/fetch-reference.sh original` lädt sie
nach `~/traders-referenz/` und verknüpft sie als `gamedata/`. Das Release enthält keine
Originaldateien und kann jederzeit gelöscht werden – es entsteht beim nächsten Workflow-Lauf neu.
