#!/usr/bin/env bash
# Self-test of tools/claude/fetch-reference.sh against a local bare repo (no network, no original files needed):
#   1. fresh sparse clone of commit 1,
#   2. remote moves on (commit 2): an existing shallow clone is updated and extended (review R19-1),
#   3. local changes and local commits in the clone are never overwritten (exit code 3, clone unchanged).
# Called by tools/pruefen.sh; exit code 0 only if all cases pass.
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
SCRIPT="$ROOT/tools/claude/fetch-reference.sh"
TMP="$(mktemp -d)"
trap 'rm -rf "$TMP"' EXIT
g() { git -c push.negotiate=false -c user.name=selbsttest -c user.email=selbsttest@invalid -c init.defaultBranch=main "$@"; }
FAIL=0
pruefe() { if eval "$2"; then echo "ok   $1"; else echo "FEHLER $1"; FAIL=1; fi; }

g init -q --bare "$TMP/bare.git"
g -C "$TMP/bare.git" config uploadpack.allowFilter true
g init -q "$TMP/work"
mkdir -p "$TMP/work/analyse/disasm" "$TMP/work/videos"
echo eins >"$TMP/work/analyse/disasm/a.txt"
echo video >"$TMP/work/videos/v.txt"
g -C "$TMP/work" add -A && g -C "$TMP/work" commit -q -m eins && g -C "$TMP/work" push -q "$TMP/bare.git" HEAD:main

export TRADERS_REFERENZ_URL="file://$TMP/bare.git" TRADERS_REFERENZ_DIR="$TMP/clone" TRADERS_REFERENZ_REF=main
run() { local rc=0; "$SCRIPT" "$@" >"$TMP/out.txt" 2>&1 || rc=$?; echo "$rc"; }

rc=$(run disasm)
pruefe "Neuklon (rc=$rc)" '[ "$rc" -eq 0 ] && [ -f "$TMP/clone/analyse/disasm/a.txt" ] && [ ! -e "$TMP/clone/videos/v.txt" ]'

echo zwei >"$TMP/work/analyse/disasm/a.txt"
g -C "$TMP/work" commit -q -am zwei && g -C "$TMP/work" push -q "$TMP/bare.git" HEAD:main
NEU=$(g -C "$TMP/work" rev-parse HEAD)
rc=$(run videos)
pruefe "Vorhandener Klon aktualisiert und ergänzt (rc=$rc)" \
  '[ "$rc" -eq 0 ] && [ "$(git -C "$TMP/clone" rev-parse HEAD)" = "$NEU" ] && [ "$(cat "$TMP/clone/analyse/disasm/a.txt")" = zwei ] && [ -f "$TMP/clone/videos/v.txt" ]'

echo drei >"$TMP/work/analyse/disasm/a.txt"
g -C "$TMP/work" commit -q -am drei && g -C "$TMP/work" push -q "$TMP/bare.git" HEAD:main
echo lokal >"$TMP/clone/analyse/disasm/a.txt"
rc=$(run disasm)
pruefe "Lokale Änderung bleibt, Rückgabe 3 (rc=$rc)" \
  '[ "$rc" -eq 3 ] && [ "$(cat "$TMP/clone/analyse/disasm/a.txt")" = lokal ] && [ "$(git -C "$TMP/clone" rev-parse HEAD)" = "$NEU" ]'

git -C "$TMP/clone" checkout -q -- analyse/disasm/a.txt
echo commit >"$TMP/clone/analyse/disasm/b.txt"
g -C "$TMP/clone" add analyse/disasm/b.txt && g -C "$TMP/clone" commit -q -m lokal
LOKAL=$(git -C "$TMP/clone" rev-parse HEAD)
rc=$(run disasm)
pruefe "Lokaler Commit bleibt, Rückgabe 3 (rc=$rc)" '[ "$rc" -eq 3 ] && [ "$(git -C "$TMP/clone" rev-parse HEAD)" = "$LOKAL" ]'

g -C "$TMP/clone" reset -q --hard "$NEU"
rc=$(run disasm)
pruefe "Nach Verwerfen wieder aktualisiert (rc=$rc)" '[ "$rc" -eq 0 ] && [ "$(cat "$TMP/clone/analyse/disasm/a.txt")" = drei ]'

[ "$FAIL" -eq 0 ] || { echo "--- letzte Ausgabe von fetch-reference.sh:"; cat "$TMP/out.txt"; }
exit "$FAIL"
