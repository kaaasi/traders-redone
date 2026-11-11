#!/usr/bin/env bash
# Lane "bestand": RefEmu probes P1-P6 of docs/analyse/BESTAND.md (appendix A).
#
#   bash experiments/bestand/proben.sh <absolute output dir> [emulated seconds for P1, default 900]
#
# Needs the original files (gamedata/ from tools/claude/fetch-reference.sh original), dotnet (source the env file of
# tools/claude/setup-workspace.sh first) and the old branches dev/ap1-refemu and dev/ap2-vga on origin.
# Works in throw-away worktrees of the current HEAD; the repository itself is not changed.
#   P1-P5: HEAD + dev/ap1-refemu (940e4b0, a12822d) + BestandProben.cs  -> <out>/p1-*.log ... p5.log
#   P6:    HEAD + dev/ap2-vga (0e95549, dab5889, 5a9f178), filter Vga  -> <out>/p6.log
# The whole script takes about 8 minutes with 230 s for P1 (reaches week 1) and correspondingly longer with 900 s.
set -euo pipefail
out="$(realpath "$1")"
seconds="${2:-900}"
here="$(cd "$(dirname "$0")" && pwd)"
repo="$(cd "$here/../.." && pwd)"
data="$(realpath "$repo/gamedata")"
[ -f "$data/dos/T.EXE" ] || { echo "Originaldaten fehlen ($repo/gamedata/dos/T.EXE)" >&2; exit 2; }
mkdir -p "$out"
work="$(mktemp -d)"
cleanup() {
  git -C "$repo" worktree remove --force "$work/ap1" 2>/dev/null || true
  git -C "$repo" worktree remove --force "$work/ap2" 2>/dev/null || true
  rm -rf "$work"
}
trap cleanup EXIT

git -C "$repo" fetch -q --depth=200 origin \
  "+refs/heads/dev/ap1-refemu:refs/remotes/origin/dev/ap1-refemu" "+refs/heads/dev/ap2-vga:refs/remotes/origin/dev/ap2-vga"

prepare() {  # <name> <commits...>: worktree of HEAD with the given commits applied (not committed)
  local name="$1"; shift
  git -C "$repo" worktree add -q --detach "$work/$name" HEAD
  git -C "$work/$name" cherry-pick -n "$@" >/dev/null
  ln -sfn "$data" "$work/$name/gamedata"
  ln -sfn "$data" "$work/$name/original"   # the old branches still read original/dos
}

prepare ap1 940e4b0 a12822d
cp "$here/BestandProben.cs" "$work/ap1/tests/Traders.Tests/RefEmu/"
( cd "$work/ap1" && BESTAND_OUT="$out" BESTAND_SECONDS="$seconds" TR_REQUIRE_GAMEDATA=1 \
    dotnet test tests/Traders.Tests --filter "FullyQualifiedName~BestandProben" 2>&1 | tail -5 ) | tee "$out/p1-p5-testlauf.log"

prepare ap2 0e95549 dab5889 5a9f178
( cd "$work/ap2" && TR_REQUIRE_GAMEDATA=1 dotnet test tests/Traders.Tests --filter "FullyQualifiedName~Vga" 2>&1 | tail -3 ) | tee "$out/p6.log"
echo "Ergebnisse: $out"
