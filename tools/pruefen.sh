#!/usr/bin/env bash
# Uniform check before every push - the same for all agents (AGENTS.md, section "Tests").
#
#   tools/pruefen.sh            hashes, architecture, coverage, restore (lock files), build without warnings and three test runs
#                               (integration PRs, first reviews, changes to the test infrastructure)
#   tools/pruefen.sh --schnell  the same with one test run: with original data if present, otherwise without (normal PR)
#
# Test runs:
#   1. with original data   TR_REQUIRE_GAMEDATA=1 - fails if gamedata/ is missing (only when present)
#   2. without original data TR_SKIP_GAMEDATA=1 (data hidden) - like CI without the reference repo: original-data tests
#      skipped visibly, all others must pass without any original file
#   3. mandatory run without data (TR_REQUIRE_GAMEDATA=1, TRADERS_GAMEDATA pointing nowhere) - MUST fail; otherwise
#      original-data tests silently run empty
# The original files come from the private repo kaaasi/traders-referenz (tools/claude/fetch-reference.sh original).
# Prints the table for PR and handoff at the end. Exit code 0 only if every step is green and run 3 fails.
set -uo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"
SLN=TradersRedone.slnx
QUICK=0; [ "${1:-}" = "--schnell" ] && QUICK=1
GAMEDATA="${TRADERS_GAMEDATA:-$ROOT/gamedata}"
DATA=0; [ -f "$GAMEDATA/dos/T.EXE" ] && DATA=1
LOG="$(mktemp -d)"
STATUS=0
schritt() { echo "== $1"; }
fehler() { echo "FEHLER: $1"; STATUS=1; }

schritt "Originaldateien (SHA-256 gegen tools/originale/)"
if [ "$DATA" -eq 1 ]; then
  ( cd "$GAMEDATA/dos" && sha256sum --quiet -c "$ROOT/tools/originale/SHA256SUMS.dos" ) || fehler "Hash DOS"
  if [ -d "$GAMEDATA/amiga" ]; then
    ( cd "$GAMEDATA/amiga" && sha256sum --quiet -c "$ROOT/tools/originale/SHA256SUMS.amiga" ) || fehler "Hash Amiga"
  fi
else
  echo "Originaldaten fehlen ($GAMEDATA) – nicht geprüft (tools/claude/fetch-reference.sh original)"
fi

schritt "Architekturgrenzen"
python3 tools/check-architecture.py || fehler "Architekturprüfung"

schritt "Abdeckung (STATUS.md, Marker, Befunde, Differenztests)"
if [ "$DATA" -eq 1 ]; then
  python3 tools/abdeckung.py --check || fehler "Abdeckung – tools/abdeckung.py --write"
else
  echo "Abdeckung nicht geprüft (braucht gamedata/dos/T.EXE)"
fi

if [ -d tests/tools ] && ls tests/tools/test_*.py >/dev/null 2>&1; then
  schritt "Python-Tests der Werkzeuge"
  python3 -m unittest discover -s tests/tools -q 2>"$LOG/py.txt" || { tail -20 "$LOG/py.txt"; fehler "Python-Tests"; }
fi

command -v dotnet >/dev/null || { fehler "dotnet fehlt (Claude: tools/claude/setup-workspace.sh, dann source ~/claude-tools-traders/env.sh)"; exit 1; }
CONFIG=(); [ -n "${TRADERS_NUGET_CONFIG:-}" ] && CONFIG=(--configfile "$TRADERS_NUGET_CONFIG")

schritt "Restore (Lockdateien, --locked-mode)"
dotnet restore "$SLN" --locked-mode "${CONFIG[@]}" >"$LOG/restore.txt" 2>&1 || { tail -20 "$LOG/restore.txt"; fehler "Restore"; exit 1; }

schritt "Build (Release, Warnungen als Fehler)"
dotnet build "$SLN" -c Release --no-restore -warnaserror >"$LOG/build.txt" 2>&1 || { grep -E "error|warn" "$LOG/build.txt" | sort -u | head -30; fehler "Build"; exit 1; }

# Every test project must report a result line; a missing one means a project crashed or was not loaded.
PROJEKTE=$(ls tests/*/*.csproj 2>/dev/null | wc -l)
ZEILEN_TAB=()
lauf() {   # lauf <name> <must_fail 0|1> <env...>
  local name="$1" soll="$2"; shift 2
  local log="$LOG/test-$(echo "$name" | tr -c 'a-zA-Z0-9' '_').txt" rc=0
  schritt "Tests: $name"
  env "$@" dotnet test "$SLN" -c Release --no-build >"$log" 2>&1 || rc=$?
  local zeilen; zeilen=$(grep -E "^(Passed|Failed)! " "$log" || true)
  summe() { printf '%s\n' "$zeilen" | grep -oE "$1: +[0-9]+" | grep -oE "[0-9]+" | awk '{s+=$1} END {print s+0}'; }
  if [ "$soll" -eq 1 ]; then
    [ "$rc" -ne 0 ] || fehler "$name lief grün – Originaldaten-Tests laufen ohne Daten still leer"
  else
    [ "$rc" -eq 0 ] || { grep -E "^\s*Failed [A-Za-z]" "$log" | head -20; fehler "$name"; }
    [ "$(printf '%s\n' "$zeilen" | grep -c .)" -eq "$PROJEKTE" ] || { tail -20 "$log"; fehler "$name: Ergebnisse für weniger als $PROJEKTE Testprojekte"; }
  fi
  ZEILEN_TAB+=("| $name | $(summe Passed) | $(summe Failed) | $(summe Skipped) |")
}
if [ "$DATA" -eq 1 ]; then lauf "Mit Originaldaten (TR_REQUIRE_GAMEDATA=1)" 0 TR_REQUIRE_GAMEDATA=1; fi
if [ "$QUICK" -eq 0 ] || [ "$DATA" -eq 0 ]; then lauf "Ohne Originaldaten (TR_SKIP_GAMEDATA=1)" 0 TR_SKIP_GAMEDATA=1 TRADERS_GAMEDATA="$LOG/keine-originaldaten"; fi
if [ "$QUICK" -eq 0 ]; then
  lauf "Pflichtlauf ohne Daten (muss scheitern)" 1 TR_REQUIRE_GAMEDATA=1 TRADERS_GAMEDATA="$LOG/keine-originaldaten"
fi

echo
python3 tools/markerbericht.py
echo
echo "### Tests zum Head \`$(git rev-parse --short HEAD)\`$( [ -n "$(git status --porcelain)" ] && echo ' (mit lokalen Änderungen)')"
echo
echo "| Lauf | bestanden | fehlgeschlagen | übersprungen |"
echo "|---|---:|---:|---:|"
printf '%s\n' "${ZEILEN_TAB[@]}"
echo
echo "dotnet $(dotnet --version)$( [ "$DATA" -eq 1 ] || echo ' · **Originaldaten nicht geprüft**')"
echo
[ "$STATUS" -eq 0 ] && echo "Ergebnis: grün" || echo "Ergebnis: ROT – Protokolle in $LOG"
exit "$STATUS"
