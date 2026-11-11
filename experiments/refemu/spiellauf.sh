#!/usr/bin/env bash
# Spur refemu: game run beyond the main menu with FXDRIVER resident; writes frames, events, graphics-driver and OPL traces.
# Usage: bash experiments/refemu/spiellauf.sh <output folder> [emulated seconds after the menu, default 120]
# Needs a Release build (dotnet build TradersRedone.slnx -c Release) and the original files in gamedata/.
# The output contains frames of the original: keep it local, never commit it (AGENTS.md §2.1).
set -uo pipefail
[ $# -ge 1 ] || { echo "Aufruf: $0 <ausgabeordner> [sekunden]" >&2; exit 2; }
out="$(realpath -m "$1")"
mkdir -p "$out/spiel"
cd "$(dirname "$0")/../.."
REFEMU_GAME_OUT="$out/spiel" REFEMU_GAME_SECONDS="${2:-120}" TR_REQUIRE_GAMEDATA=1 \
  dotnet test TradersRedone.slnx -c Release --no-build \
  --filter "FullyQualifiedName~RefEmu.GameRunTests.Game_starts_from_main_menu_and_runs" \
  --logger "console;verbosity=normal" > "$out/spiel-test.log" 2>&1
rc=$?
echo "exit $rc" >> "$out/spiel-test.log"
echo "Ergebnis: $out (exit $rc)"
exit $rc
