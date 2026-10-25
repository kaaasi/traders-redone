#!/usr/bin/env bash
# AP1 experiment: boots T.EXE in the reference emulator and stores frames + event log.
set -uo pipefail
out="$(realpath "$1")"
mkdir -p "$out/boot"
REFEMU_OUT="$out/boot" REFEMU_SECONDS="${REFEMU_SECONDS:-40}" \
  dotnet test TradersRedone.slnx -c Release --no-build --filter "FullyQualifiedName~RefEmu.BootTests" \
  --logger "console;verbosity=normal" > "$out/boot-test.log" 2>&1
echo "exit $?" >> "$out/boot-test.log"
ls "$out/boot" | head -50 > "$out/boot-files.txt"
