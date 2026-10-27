#!/usr/bin/env bash
# Publish compact reports without force-push; retry from the newest tree after races.
set -euo pipefail
report="$(cd "$1" && pwd)"
sha="$2"
ref="$3"
[[ "$sha" =~ ^[0-9a-f]{40}$ ]] || exit 1
work="$(mktemp -d)"
trap 'rm -rf "$work"' EXIT
gh auth setup-git
url="https://github.com/${GITHUB_REPOSITORY}.git"
for attempt in 1 2 3 4 5; do
  rm -rf "$work/repo"
  if git ls-remote --exit-code --heads "$url" ci-results >/dev/null 2>&1; then
    git clone --quiet --depth 1 --branch ci-results "$url" "$work/repo"
  else
    git init --quiet "$work/repo"
    git -C "$work/repo" checkout --quiet --orphan ci-results
    git -C "$work/repo" remote add origin "$url"
  fi
  python3 tools/report-store.py "$report" "$work/repo"
  git -C "$work/repo" add -A
  if git -C "$work/repo" diff --cached --quiet; then exit 0; fi
  git -C "$work/repo" -c user.name=ci -c user.email=ci@users.noreply.github.com commit --quiet -m "CI-Ergebnis $sha ($ref)"
  if git -C "$work/repo" push --quiet origin ci-results; then exit 0; fi
  sleep "$attempt"
done
echo 'Report publication failed after concurrent-update retries' >&2
exit 1
