#!/usr/bin/env bash
# Richtet Claudes Arbeitsumgebung ein: .NET-SDK und NuGet-Pakete aus dem Release "claude-werkzeuge" laden,
# Offline-Paketquelle eintragen, Umgebungsvariablen schreiben. Die Originaldateien liegen im Repo (original/).
# Aufruf: tools/claude/setup-workspace.sh      danach: source ~/claude-tools-traders/env.sh
set -euo pipefail

REPO="${REPO:-kaaasi/traders-redone}"
TAG="claude-werkzeuge"
BASE="${CLAUDE_TOOLS:-$HOME/claude-tools-traders}"   # eigener Ordner: kollidiert nicht mit anderen Projekten
mkdir -p "$BASE"

download() {
  local name="$1" id
  id=$(gh api "repos/$REPO/releases/tags/$TAG" -q ".assets[] | select(.name==\"$name\") | .id")
  [ -n "$id" ] || { echo "Datei $name fehlt im Release $TAG – Workflow 'Werkzeuge für Claude' starten:" >&2
                    echo "  gh api -X POST repos/$REPO/actions/workflows/werkzeuge.yml/dispatches -f ref=main" >&2; exit 1; }
  curl -fsSL -H "Authorization: Bearer $(gh auth token)" -H "Accept: application/octet-stream" \
    -o "$BASE/$name" "https://api.github.com/repos/$REPO/releases/assets/$id"
}

WANT=$(python3 -c 'import json,sys; print(json.load(open(sys.argv[1]))["sdk"]["version"])' "$(dirname "$0")/../../global.json")
if [ ! -x "$BASE/dotnet/dotnet" ] || [ "$("$BASE/dotnet/dotnet" --version 2>/dev/null)" != "$WANT" ]; then
  echo "Lade .NET-SDK $WANT …"
  rm -rf "$BASE/dotnet"
  download dotnet-sdk-linux-x64.tar.gz
  mkdir -p "$BASE/dotnet" && tar -xzf "$BASE/dotnet-sdk-linux-x64.tar.gz" -C "$BASE/dotnet"
  rm -f "$BASE/dotnet-sdk-linux-x64.tar.gz"
fi

echo "Lade NuGet-Pakete …"
download nuget-packages.tar.gz
rm -rf "$BASE/nuget-packages" && tar -xzf "$BASE/nuget-packages.tar.gz" -C "$BASE" && rm -f "$BASE/nuget-packages.tar.gz"

# Nur die Offline-Quelle verwenden (nuget.org ist aus der Arbeitsumgebung nicht erreichbar).
# Eigene Konfigurationsdatei, über NUGET_CONFIG_FILE in pruefen.sh bzw. --configfile nutzbar; zusätzlich als
# Benutzerkonfiguration, damit ein schlichtes "dotnet restore" funktioniert.
cat > "$BASE/NuGet.Config" <<XML
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="traders-offline" value="$BASE/nuget-packages" />
  </packageSources>
</configuration>
XML
mkdir -p "$HOME/.nuget/NuGet"
cp "$BASE/NuGet.Config" "$HOME/.nuget/NuGet/NuGet.Config"

cat > "$BASE/env.sh" <<ENV
export DOTNET_ROOT="$BASE/dotnet"
export PATH="$BASE/dotnet:\$PATH"
export DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_NOLOGO=1 DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
export TRADERS_NUGET_CONFIG="$BASE/NuGet.Config"
ENV

echo "Fertig. Vor dem Bauen: source $BASE/env.sh"
