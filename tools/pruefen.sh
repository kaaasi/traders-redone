#!/usr/bin/env bash
# Einheitliche Prüfung vor jedem Push – für alle Agenten gleich (AGENTS.md, Abschnitt „Tests“).
#
#   tools/pruefen.sh            Hashes, Architektur, Abdeckung, Restore (Lockdateien), Build ohne Warnungen, alle Tests, Markerbericht
#   tools/pruefen.sh --schnell  derselbe Lauf (die Originaldaten liegen im Repo, es gibt nur einen Testlauf);
#                               die Option existiert, damit Kurzaufrufe mit dem Eishockey-Ablauf übereinstimmen
#
# Gibt am Ende die Tabelle für PR und Übergabe aus. Exit-Code 0 nur, wenn jeder Schritt grün ist.
set -uo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"
SLN=TradersRedone.slnx
LOG="$(mktemp -d)"
STATUS=0
schritt() { echo "== $1"; }
fehler() { echo "FEHLER: $1"; STATUS=1; }

schritt "Originaldateien (SHA-256)"
( cd original/dos && sha256sum --quiet -c ../SHA256SUMS.dos ) || fehler "Hash DOS"
( cd original/amiga && sha256sum --quiet -c ../SHA256SUMS.amiga ) || fehler "Hash Amiga"

schritt "Architekturgrenzen"
python3 tools/check-architecture.py || fehler "Architekturprüfung"

schritt "Abdeckung (STATUS.md, Marker, Befunde, Differenztests)"
python3 tools/abdeckung.py --check || fehler "Abdeckung – tools/abdeckung.py --write"

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

schritt "Tests"
dotnet test "$SLN" -c Release --no-build >"$LOG/test.txt" 2>&1 || fehler "Tests"
# Jedes Testprojekt muss eine Ergebniszeile liefern; fehlt eine, ist ein Projekt abgestürzt oder nicht geladen.
PROJEKTE=$(ls tests/*/*.csproj 2>/dev/null | wc -l)
ZEILEN=$(grep -E "^(Passed|Failed)! " "$LOG/test.txt" || true)
[ "$(printf '%s\n' "$ZEILEN" | grep -c .)" -eq "$PROJEKTE" ] || { tail -20 "$LOG/test.txt"; fehler "Ergebnisse für weniger als $PROJEKTE Testprojekte"; }
grep -E "^\s*Failed [A-Za-z]" "$LOG/test.txt" | head -20
summe() { printf '%s\n' "$ZEILEN" | grep -oE "$1: +[0-9]+" | grep -oE "[0-9]+" | awk '{s+=$1} END {print s+0}'; }

echo
python3 tools/markerbericht.py
echo
echo "### Tests zum Head \`$(git rev-parse --short HEAD)\`$( [ -n "$(git status --porcelain)" ] && echo ' (mit lokalen Änderungen)')"
echo
echo "| Lauf | bestanden | fehlgeschlagen | übersprungen |"
echo "|---|---:|---:|---:|"
echo "| \`tools/pruefen.sh\` (dotnet $(dotnet --version)) | $(summe Passed) | $(summe Failed) | $(summe Skipped) |"
echo
[ "$STATUS" -eq 0 ] && echo "Ergebnis: grün" || echo "Ergebnis: ROT – Protokolle in $LOG"
exit "$STATUS"
