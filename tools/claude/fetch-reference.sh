#!/usr/bin/env bash
# Loads reference data from the private repo kaaasi/traders-referenz (sparse, only the requested folders) and links the
# original files into the code repo as gamedata/ (ignored by git). Nothing from there is ever committed here (AGENTS.md 2.1).
#
#   tools/claude/fetch-reference.sh [original] [disasm] [refemu] [videos]     default: original
#
# Target: $TRADERS_REFERENZ_DIR (default ~/traders-referenz); branch: $TRADERS_REFERENZ_REF (default main).
# An existing clone is extended by the requested folders and moved to the current state of the remote branch. The clone is
# shallow (--depth 1), so old and new states share no history: the branch is reset to the fetched commit instead of merged.
# Local changes and local commits are never overwritten (exit code 3); commit them on a branch daten/... in the reference repo first.
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
  if [ -n "$(git -C "$DEST" status --porcelain --untracked-files=no)" ]; then
    echo "Lokale Änderungen in $DEST – nicht aktualisiert (erst sichern oder verwerfen):" >&2
    git -C "$DEST" status --short --untracked-files=no >&2
    exit 3
  fi
  if [ -z "$(git -C "$DEST" branch -r --points-at HEAD)" ]; then
    echo "Lokale Commits in $DEST ($(git -C "$DEST" rev-parse --short HEAD) liegt auf keinem Remote-Branch) – nicht aktualisiert" >&2
    exit 3
  fi
  git -C "$DEST" fetch -q --depth 1 origin "$REF"
  git -C "$DEST" checkout -q -B "$REF" FETCH_HEAD
  git -C "$DEST" sparse-checkout add "${dirs[@]}"
fi

if [[ " ${dirs[*]} " == *" original "* ]]; then
  ( cd "$DEST/original/dos" && sha256sum --quiet -c "$ROOT/tools/originale/SHA256SUMS.dos" ) \
    || { echo "Prüfsummen DOS stimmen nicht mit tools/originale/SHA256SUMS.dos überein" >&2; exit 3; }
  ln -sfn "$DEST/original" "$ROOT/gamedata"
  echo "Originaldateien: $ROOT/gamedata -> $DEST/original"
fi
echo "Referenzdaten: $DEST ($(git -C "$DEST" rev-parse --short HEAD), Teile: ${dirs[*]})"
