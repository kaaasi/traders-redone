#!/usr/bin/env bash
# Poll compact metadata with a bounded deadline; missing reports never mean success.
set -euo pipefail
sha="$(git rev-parse "${1:-HEAD}")"
limit="${CI_WAIT_SECONDS:-900}"
[[ "$limit" =~ ^[0-9]+$ ]] || exit 2
deadline=$((SECONDS + limit))
while (( SECONDS < deadline )); do
  if git fetch --quiet --depth 1 origin ci-results; then
    if git cat-file -e "FETCH_HEAD:commits/$sha/result.json" 2>/dev/null; then
      mkdir -p .ci-results
      git show "FETCH_HEAD:commits/$sha/result.json" > .ci-results/result.json
      git show "FETCH_HEAD:commits/$sha/SUMMARY.md" > .ci-results/SUMMARY.md
      cat .ci-results/SUMMARY.md
      python3 - "$sha" <<'PY'
import sys
from pathlib import Path
sys.path.insert(0, 'tools')
from repo_policy import check_report
errors = check_report(Path('.ci-results/result.json').read_text(), sys.argv[1])
for error in errors:
    print(error, file=sys.stderr)
sys.exit(bool(errors))
PY
      exit $?
    fi
  else
    echo 'CI-Bericht derzeit nicht abrufbar; erneuter Versuch bis Timeout.' >&2
  fi
  sleep 10
done
echo "Timeout: kein CI-Bericht für $sha. Actions prüfen; fehlend ist nicht grün." >&2
exit 1
