#!/usr/bin/env bash
# Runs a CI step; on failure, the tail of its output is emitted as a GitHub annotation
# (annotations are readable via the check-runs API even when job logs are not).
name="$1"; shift
log="$(mktemp)"
"$@" 2>&1 | tee "$log"
rc=${PIPESTATUS[0]}
if [ "$rc" -ne 0 ]; then
  tail -n 60 "$log" | sed 's/%/%25/g' | awk 'BEGIN{ORS="%0A"}{print}' > "$log.tail"
  echo "::error title=${name} failed (exit ${rc})::$(cat "$log.tail")"
fi
exit "$rc"
