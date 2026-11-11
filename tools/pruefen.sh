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
#      original-data tests silently run empty. Every test that passes here must also pass in run 2: a test that passes
#      without data but is skipped in run 2 carries [GameDataFact]/[GameDataTheory] needlessly and would no longer run in CI
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

schritt "Selbsttest tools/claude/fetch-reference.sh (lokales Bare-Repo)"
tools/claude/fetch-reference-selbsttest.sh >"$LOG/fetch-selbsttest.txt" 2>&1 || { cat "$LOG/fetch-selbsttest.txt"; fehler "Selbsttest fetch-reference.sh"; }

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
lauf() {   # lauf <id> <name> <must_fail 0|1> <env...>   (TRX results in $LOG/trx-<id>/)
  local id="$1" name="$2" soll="$3"; shift 3
  local log="$LOG/test-$id.txt" rc=0
  schritt "Tests: $name"
  env "$@" dotnet test "$SLN" -c Release --no-build --logger trx --results-directory "$LOG/trx-$id" >"$log" 2>&1 || rc=$?
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
if [ "$DATA" -eq 1 ]; then lauf mit "Mit Originaldaten (TR_REQUIRE_GAMEDATA=1)" 0 TR_REQUIRE_GAMEDATA=1; fi
if [ "$QUICK" -eq 0 ] || [ "$DATA" -eq 0 ]; then lauf ohne "Ohne Originaldaten (TR_SKIP_GAMEDATA=1)" 0 TR_SKIP_GAMEDATA=1 TRADERS_GAMEDATA="$LOG/keine-originaldaten"; fi
if [ "$QUICK" -eq 0 ]; then
  lauf pflicht "Pflichtlauf ohne Daten (muss scheitern)" 1 TR_REQUIRE_GAMEDATA=1 TRADERS_GAMEDATA="$LOG/keine-originaldaten"
  schritt "Marker: kein Originaldaten-Test kommt ohne Daten aus (Pflichtlauf gegen Lauf ohne Daten)"
  python3 - "$LOG/trx-ohne" "$LOG/trx-pflicht" <<'PY' || fehler "Unnötig markierte Originaldaten-Tests – [GameDataFact]/[GameDataTheory] durch [Fact]/[Theory] ersetzen"
import glob, sys, xml.etree.ElementTree as ET

def passed(folder):
    # Test method -> passed at least once (theory rows "Method(args)" count for their method).
    files = glob.glob(folder + "/*.trx")
    if not files:
        sys.exit(f"keine TRX-Ergebnisse in {folder}")
    result = {}
    for f in files:
        for e in ET.parse(f).iter():
            if e.tag.endswith("UnitTestResult"):
                method = e.get("testName").split("(")[0]
                result[method] = result.get(method, False) or e.get("outcome") == "Passed"
    return result

without, mandatory = passed(sys.argv[1]), passed(sys.argv[2])
needless = sorted(m for m, ok in mandatory.items() if ok and not without.get(m, False))
for m in needless:
    print(f"  bestanden ohne Daten, im Lauf ohne Daten aber übersprungen: {m}")
sys.exit(1 if needless else 0)
PY
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
