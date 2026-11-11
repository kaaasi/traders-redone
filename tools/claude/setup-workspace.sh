#!/usr/bin/env bash
# Sets up Claude's work environment: downloads the .NET SDK and NuGet packages from the release "claude-werkzeuge",
# registers the offline package source and writes the environment variables. The original files are in the repo (original/).
# Usage: tools/claude/setup-workspace.sh      then: source ~/claude-tools-traders/env.sh
set -euo pipefail

REPO="${REPO:-kaaasi/traders-redone}"
TAG="claude-werkzeuge"
BASE="${CLAUDE_TOOLS:-$HOME/claude-tools-traders}"   # own folder, separate from other projects' tool folders
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

# Use only offline sources (nuget.org cannot be reached from the work environment).
# Own config file, used by pruefen.sh via TRADERS_NUGET_CONFIG (--configfile); it contains only this project's source.
cat > "$BASE/NuGet.Config" <<XML
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="traders-offline" value="$BASE/nuget-packages" />
  </packageSources>
</configuration>
XML
# Additionally merge the source into the user config so that a plain "dotnet restore" works. Sources other projects
# registered there (e.g. the Eishockey-Manager's offline mirror) are kept; only nuget.org is dropped.
mkdir -p "$HOME/.nuget/NuGet"
python3 - "$HOME/.nuget/NuGet/NuGet.Config" "$BASE/nuget-packages" <<'PY'
import os, sys
import xml.etree.ElementTree as ET
path, source = sys.argv[1], sys.argv[2]
root = ET.parse(path).getroot() if os.path.exists(path) else ET.Element("configuration")
sources = root.find("packageSources")
if sources is None:
    sources = ET.SubElement(root, "packageSources")
kept = [e for e in sources.findall("add")
        if e.get("key") not in ("traders-offline", "nuget.org") and "nuget.org" not in (e.get("value") or "")]
for child in list(sources):
    sources.remove(child)
ET.SubElement(sources, "clear")
for e in kept:
    sources.append(e)
ET.SubElement(sources, "add", key="traders-offline", value=source)
ET.indent(root)
ET.ElementTree(root).write(path, encoding="utf-8", xml_declaration=True)
PY

cat > "$BASE/env.sh" <<ENV
export DOTNET_ROOT="$BASE/dotnet"
export PATH="$BASE/dotnet:\$PATH"
export DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_NOLOGO=1 DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
export TRADERS_NUGET_CONFIG="$BASE/NuGet.Config"
ENV

echo "Fertig. Vor dem Bauen: source $BASE/env.sh"
