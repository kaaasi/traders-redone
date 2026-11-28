#!/usr/bin/env bash
# Uniform check before every push - the same for all agents (AGENTS.md, section "Tests").
#
#   tools/pruefen.sh            hashes, architecture, coverage, restore (lock files), build without warnings and three test runs
#                               (integration PRs, first reviews, changes to the test infrastructure)
#   tools/pruefen.sh --schnell  the same with one test run: with original data if present, otherwise without (normal PR)
#   tools/pruefen.sh --hintergrund [--schnell]
#                               starts the check detached and returns at once; log TR_PRUEF_STAND/lauf.log (default
#                               ${TMPDIR:-/tmp}/traders-pruefen/lauf.log). The log ends with the table and then, as its very
#                               last line, "ENDE-MARKER rc=<exit code>" - only that line means the run has finished.
#   tools/pruefen.sh --stand    reports a background run: "läuft"/"fertig" (rc), last step, test results so far per run, and
#                               after the end marker the table; exit code 0 = finished green, 2 = still running, 1 = finished red,
#                               3 = no run known
#
# Test runs:
#   1. with original data   TR_REQUIRE_GAMEDATA=1 - fails if gamedata/ is missing (only when present)
#   2. without original data TR_SKIP_GAMEDATA=1 (data hidden) - like CI without the reference repo: original-data tests
#      skipped visibly, all others must pass without any original file
#   3. mandatory run without data (TR_REQUIRE_GAMEDATA=1, TRADERS_GAMEDATA pointing nowhere) - MUST fail; otherwise
#      original-data tests silently run empty. Every test that passes here must also pass in run 2: a test that passes
#      without data but is skipped in run 2 carries [GameDataFact]/[GameDataTheory] needlessly and would no longer run in CI
# Hang limit per test run: TR_HANG_TIMEOUT (default 20m, dotnet test --blame-hang-timeout); a hang counts as an error.
# The original files come from the private repo kaaasi/traders-referenz (tools/claude/fetch-reference.sh original).
# Prints the table for PR and handoff at the end. Exit code 0 only if every step is green and run 3 fails.
set -uo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"
SLN=TradersRedone.slnx
STAND="${TR_PRUEF_STAND:-${TMPDIR:-/tmp}/traders-pruefen}"
QUICK=0; BG=0; QUERY=0
for arg in "$@"; do
  case "$arg" in
    --schnell) QUICK=1 ;;
    --hintergrund) BG=1 ;;
    --stand) QUERY=1 ;;
    *) echo "Unbekannte Option: $arg (--schnell, --hintergrund, --stand)"; exit 64 ;;
  esac
done

if [ "$QUERY" -eq 1 ]; then
  [ -f "$STAND/lauf.log" ] || { echo "Kein Hintergrundlauf bekannt ($STAND/lauf.log fehlt)"; exit 3; }
  ende=$(grep -E '^ENDE-MARKER rc=[0-9]+$' "$STAND/lauf.log" | tail -1)
  pid=$(cat "$STAND/lauf.pid" 2>/dev/null || true)
  if [ -z "$ende" ] && { [ -z "$pid" ] || ! kill -0 "$pid" 2>/dev/null; }; then
    echo "Lauf abgebrochen ohne Ende-Marker (Prozess ${pid:-?} läuft nicht mehr); letzte Zeilen:"; tail -5 "$STAND/lauf.log"; exit 1
  fi
  [ -z "$ende" ] && echo "läuft (Prozess $pid)" || echo "fertig (${ende#ENDE-MARKER })"
  echo "Schritt: $(grep -E '^== ' "$STAND/lauf.log" | tail -1)"
  protokolle=$(grep -m1 -E '^Protokolle: ' "$STAND/lauf.log" | sed 's/^Protokolle: //')
  if [ -n "$protokolle" ] && ls "$protokolle"/test-*.txt >/dev/null 2>&1; then
    for t in "$protokolle"/test-*.txt; do
      echo "  $(basename "$t" .txt): $(grep -c -E '^\s*Passed ' "$t") bestanden, $(grep -c -E '^\s*Failed ' "$t") fehlgeschlagen bisher"
    done
  fi
  if [ -n "$ende" ]; then
    sed -n '/^### Tests zum Head/,/^Ergebnis:/p' "$STAND/lauf.log"
    [ "$ende" = "ENDE-MARKER rc=0" ] && exit 0 || exit 1
  fi
  exit 2
fi

if [ "$BG" -eq 1 ]; then
  mkdir -p "$STAND"
  if [ -f "$STAND/lauf.pid" ] && kill -0 "$(cat "$STAND/lauf.pid")" 2>/dev/null; then
    echo "Es läuft schon eine Prüfung (Prozess $(cat "$STAND/lauf.pid")); Abfrage: tools/pruefen.sh --stand"; exit 65
  fi
  args=(); [ "$QUICK" -eq 1 ] && args+=(--schnell)
  : >"$STAND/lauf.log"
  # the end marker is written by the wrapper after the child has returned, i.e. after the table
  TR_LOG="$STAND/lauf.log" setsid nohup bash -c 'rc=0; "$0" "$@" >>"$TR_LOG" 2>&1 || rc=$?; echo "ENDE-MARKER rc=$rc" >>"$TR_LOG"' "$0" ${args[@]+"${args[@]}"} \
    >/dev/null 2>&1 </dev/null &
  echo $! >"$STAND/lauf.pid"
  echo "Prüfung im Hintergrund gestartet (Prozess $!). Abfrage: tools/pruefen.sh --stand; Log: $STAND/lauf.log"
  exit 0
fi
GAMEDATA="${TRADERS_GAMEDATA:-$ROOT/gamedata}"
DATA=0; [ -f "$GAMEDATA/dos/T.EXE" ] && DATA=1
LOG="$(mktemp -d)"
echo "Protokolle: $LOG"
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
  # --blame-hang-timeout: a hanging test aborts the run as an error naming the test (TR_HANG_TIMEOUT, default 20m);
  # console logger "normal": the log shows during the run which tests have finished (the table still comes from the TRX).
  env "$@" dotnet test "$SLN" -c Release --no-build --logger trx --logger "console;verbosity=normal" \
    --blame-hang-timeout "${TR_HANG_TIMEOUT:-20m}" --blame-hang-dump-type none \
    --results-directory "$LOG/trx-$id" >"$log" 2>&1 || rc=$?
  # Table source is the TRX (Counters of every test project), not the console output.
  local trx; trx=$(python3 - "$LOG/trx-$id" <<'PY'
import glob, sys, xml.etree.ElementTree as ET
p = f = s = n = 0
for fn in glob.glob(sys.argv[1] + "/*.trx"):
    for e in ET.parse(fn).iter():
        if e.tag.endswith("Counters"):
            p += int(e.get("passed", 0)); f += int(e.get("failed", 0)) + int(e.get("error", 0)) + int(e.get("timeout", 0)) + int(e.get("aborted", 0))
            s += int(e.get("total", 0)) - int(e.get("executed", 0)); n += 1   # skipped = total - executed
print(p, f, s, n)
PY
)
  local np nf ns nt; read -r np nf ns nt <<<"$trx"
  # An aborted run (hang limit, test host crash, "Test Run Aborted", missing TRX) is an error in EVERY run, the mandatory
  # run included, and shows in the table row: the hanging test never reaches the TRX counters, so the row would look green.
  local abgebrochen=0
  if grep -q -E "^\s*Test Run Aborted" "$log" || [ "$nt" -ne "$PROJEKTE" ]; then abgebrochen=1; fi
  if [ "$abgebrochen" -eq 1 ]; then
    # all tests running at the abort (the block ends at the first empty line)
    sed -n '/The test running when/,/^\s*$/p' "$log" | head -40
    grep -E "hang timeout|inactivity time|Test Run Aborted" "$log" | head -5
    [ "$nt" -eq "$PROJEKTE" ] || echo "  Ergebnisse für $nt von $PROJEKTE Testprojekten"
    fehler "$name abgebrochen (Hang-Limit, Testhost-Absturz oder fehlende Ergebnisse)"
    [ "$nf" -ge 1 ] || nf=1   # never an all-green row for an aborted run
  elif [ "$soll" -eq 1 ]; then
    [ "$rc" -ne 0 ] || fehler "$name lief grün – Originaldaten-Tests laufen ohne Daten still leer"
  else
    [ "$rc" -eq 0 ] || { grep -E -A1 "^\s*Failed [A-Za-z]" "$log" | head -30; fehler "$name"; }
  fi
  local marke=""; [ "$abgebrochen" -eq 1 ] && marke=" – **abgebrochen**"
  ZEILEN_TAB+=("| $name$marke | $np | $nf | $ns |")
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
