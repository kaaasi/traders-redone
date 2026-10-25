#!/usr/bin/env bash
# Runs one CI step and records it for the ci-results branch:
#   $CI_OUT/logs/<name>.log   full output
#   $CI_OUT/steps.tsv         name, exit code, seconds
# On failure the tail of the output is also emitted as a GitHub annotation
# (readable via the check-runs API).
name="$1"; shift
out="${CI_OUT:-ci-out}"
mkdir -p "$out/logs"
log="$out/logs/$name.log"
start=$(date +%s)
"$@" > >(tee "$log") 2>&1
rc=$?
wait
end=$(date +%s)
printf '%s\t%s\t%s\n' "$name" "$rc" "$((end - start))" >> "$out/steps.tsv"
if [ "$rc" -ne 0 ]; then
  tail -n 60 "$log" | sed 's/%/%25/g' | awk 'BEGIN{ORS="%0A"}{print}' > "$log.tail"
  echo "::error title=${name} failed (exit ${rc})::$(cat "$log.tail")"
  rm -f "$log.tail"
fi
exit "$rc"
