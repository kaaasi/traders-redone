#!/usr/bin/env bash
# Pushes a CI report folder to the orphan branch ci-results:
#   runs/<sha>/          report of this run (newest 40 runs are kept)
#   latest/<ref>/        newest report per branch
#   index.md             list of runs
set -euo pipefail
report="$(cd "$1" && pwd)"; sha="$2"; ref="$3"
safe_ref="${ref//\//_}"
work="$(mktemp -d)"
url="https://x-access-token:${GITHUB_TOKEN}@github.com/${GITHUB_REPOSITORY}.git"
if git ls-remote --exit-code --heads "$url" ci-results >/dev/null 2>&1; then
  git clone --quiet --depth 1 --branch ci-results "$url" "$work"
else
  git init --quiet "$work"
  git -C "$work" checkout --quiet --orphan ci-results
  git -C "$work" remote add origin "$url"
fi
cd "$work"
rm -rf "runs/$sha" "latest/$safe_ref"
mkdir -p runs latest
cp -r "$report" "runs/$sha"
cp -r "$report" "latest/$safe_ref"
# keep newest 40 runs
ls -1t runs | tail -n +41 | while read -r old; do rm -rf "runs/$old"; done
{
  echo "# CI-Ergebnisse"; echo
  echo "Neueste je Branch: \`latest/<branch>/SUMMARY.md\`"; echo
  for d in $(ls -1t runs); do echo "- [\`${d:0:7}\`](runs/$d/SUMMARY.md) – $(head -1 "runs/$d/SUMMARY.md" | sed 's/^# //')"; done
} > index.md
git add -A
git -c user.name="ci" -c user.email="ci@users.noreply.github.com" commit --quiet -m "CI-Ergebnis $sha ($ref)" || exit 0
for i in 1 2 3 4 5; do
  git push --quiet origin ci-results && exit 0
  sleep $((i * 3))
  git pull --quiet --rebase origin ci-results || true
done
exit 1
