#!/usr/bin/env bash
# For Claude sessions: waits until all check runs of a commit are complete, then fetches the
# ci-results branch and prints the SUMMARY. Results are left in .ci-results/ (git-ignored).
# Usage: tools/ci-wait.sh [commit]   (default: HEAD)
set -uo pipefail
sha="$(git rev-parse "${1:-HEAD}")"
repo="kaaasi/traders-redone"
while true; do
  st="$(gh api "repos/$repo/commits/$sha/check-runs" --jq '[.check_runs[] | .status] | (length|tostring) + " " + (map(select(. != "completed")) | length | tostring)' 2>/dev/null || echo "0 1")"
  total="${st% *}"; open="${st#* }"
  if [ "$total" != "0" ] && [ "$open" = "0" ]; then break; fi
  sleep 10
done
gh api "repos/$repo/commits/$sha/check-runs" --jq '.check_runs[] | "\(.name): \(.conclusion)"'
for i in $(seq 1 30); do
  git fetch --quiet --depth 1 origin ci-results 2>/dev/null || true
  if git cat-file -e "FETCH_HEAD:runs/$sha/SUMMARY.md" 2>/dev/null; then
    rm -rf .ci-results && mkdir -p .ci-results
    git archive FETCH_HEAD "runs/$sha" | tar -x -C .ci-results --strip-components=2
    cat .ci-results/SUMMARY.md
    exit 0
  fi
  sleep 10
done
echo "Kein Bericht für $sha im Branch ci-results gefunden." >&2
exit 1
