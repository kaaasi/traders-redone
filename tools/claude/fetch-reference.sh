#!/usr/bin/env bash
# Loads reference data from the private repo kaaasi/traders-referenz (sparse, only the requested folders) and links the
# original files into the code repo as gamedata/ (ignored by git). Nothing from there is ever committed here (AGENTS.md 2.1).
#
#   tools/claude/fetch-reference.sh [original] [disasm] [refemu] [videos]     default: original
#
# Target: $TRADERS_REFERENZ_DIR (default ~/traders-referenz); branch: $TRADERS_REFERENZ_REF (default main).
# An existing clone is extended; if it is behind the remote branch, it is updated (fast-forward only).
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
DEST="${TRADERS_REFERENZ_DIR:-$HOME/traders-referenz}"
REF="${TRADERS_REFERENZ_REF:-main}"
URL="${TRADERS_REFERENZ_URL:-https://github.com/kaaasi/traders-referenz}"
[ $# -gt 0 ] || set -- original

dirs=()
for part in "$@"; do
  case "$part" in
    original) dirs+=(original) ;;
    disasm)   dirs+=(analyse/disasm) ;;
    refemu)   dirs+=(refemu) ;;
    videos)   dirs+=(videos) ;;
    *) echo "Unbekannter Teil: $part (erlaubt: original disasm refemu videos)" >&2; exit 2 ;;
  esac
done

if [ ! -d "$DEST/.git" ]; then
  git clone --depth 1 --filter=blob:none --sparse --branch "$REF" "$URL" "$DEST"
  git -C "$DEST" sparse-checkout set "${dirs[@]}"
else
  git -C "$DEST" sparse-checkout add "${dirs[@]}"
  git -C "$DEST" fetch --depth 1 origin "$REF"
  git -C "$DEST" merge --ff-only FETCH_HEAD >/dev/null
fi

if [[ " ${dirs[*]} " == *" original "* ]]; then
  ( cd "$DEST/original/dos" && sha256sum --quiet -c "$ROOT/tools/originale/SHA256SUMS.dos" ) \
    || { echo "Prüfsummen DOS stimmen nicht mit tools/originale/SHA256SUMS.dos überein" >&2; exit 3; }
  ln -sfn "$DEST/original" "$ROOT/gamedata"
  echo "Originaldateien: $ROOT/gamedata -> $DEST/original"
fi
echo "Referenzdaten: $DEST ($(git -C "$DEST" rev-parse --short HEAD), Teile: ${dirs[*]})"
