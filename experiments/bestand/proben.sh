#!/usr/bin/env bash
# Lane "bestand": RefEmu probes P1-P6 of docs/analyse/BESTAND.md (appendix A).
#
#   bash experiments/bestand/proben.sh <absolute output dir> [emulated seconds for P1, default 900]
#
# Needs the original files (gamedata/ from tools/claude/fetch-reference.sh original), dotnet (source the env file of
# tools/claude/setup-workspace.sh first) and, only as long as HEAD does not contain the probed file of an old branch yet
# (CallTracer.cs for dev/ap1-refemu, ModeXDriver.cs for dev/ap2-vga), that branch on origin; it is fetched only then (#49 N1).
# Works in throw-away worktrees of the current HEAD; the repository itself is not changed.
#   P1-P5: HEAD (+ dev/ap1-refemu 940e4b0, a12822d, if not in HEAD yet) + BestandProben.cs -> <out>/p1-*.log ... p5.log
#   P6:    HEAD (+ dev/ap2-vga 0e95549, dab5889, 5a9f178, if not in HEAD yet), filter Vga  -> <out>/p6.log
# A partial takeover (the probed file in HEAD, but not every change of the old branch) is not detected: nothing is applied and
# the build or the probe fails loudly, never silently (#49 N3, suspicion from the review, by reading only).
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

prepare() {  # <name> <branch> <file> <commits...>: worktree of HEAD; the old branch is fetched and its commits applied (not
  # committed) only if HEAD lacks <file>, i.e. the branch has not been taken over yet (after the merge of lane ① or ② it is part
  # of HEAD, and the branch may be deleted on origin).
  local name="$1" branch="$2" probe="$3"; shift 3
  git -C "$repo" worktree add -q --detach "$work/$name" HEAD
  if [ -e "$work/$name/$probe" ]; then
    echo "$name: $probe already in HEAD, $branch neither fetched nor applied"
  else
    git -C "$repo" fetch -q --depth=200 origin "+refs/heads/$branch:refs/remotes/origin/$branch"
    git -C "$work/$name" cherry-pick -n "$@" >/dev/null
  fi
  ln -sfn "$data" "$work/$name/gamedata"
  ln -sfn "$data" "$work/$name/original"   # the old branches still read original/dos
}

prepare ap1 dev/ap1-refemu src/Traders.RefEmu/Machine/CallTracer.cs 940e4b0 a12822d
cp "$here/BestandProben.cs" "$work/ap1/tests/Traders.Tests/RefEmu/"
( cd "$work/ap1" && BESTAND_OUT="$out" BESTAND_SECONDS="$seconds" TR_REQUIRE_GAMEDATA=1 \
    dotnet test tests/Traders.Tests --filter "FullyQualifiedName~BestandProben" 2>&1 | tail -5 ) | tee "$out/p1-p5-testlauf.log"

prepare ap2 dev/ap2-vga src/Traders.Vga/ModeXDriver.cs 0e95549 dab5889 5a9f178
( cd "$work/ap2" && TR_REQUIRE_GAMEDATA=1 dotnet test tests/Traders.Tests --filter "FullyQualifiedName~Vga" 2>&1 | tail -3 ) | tee "$out/p6.log"
echo "Ergebnisse: $out"
