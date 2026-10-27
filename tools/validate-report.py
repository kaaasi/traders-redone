"""Validate artifact boundaries and replace claimed job outcomes with GitHub's API data."""
import json
import os
from pathlib import Path
import subprocess
import sys

root = Path(sys.argv[1])
files = list(root.rglob('*'))
if any(p.is_symlink() for p in files) or sum(p.stat().st_size for p in files if p.is_file()) > 5_000_000:
    raise SystemExit('Report contains links or exceeds 5 MB')
for p in files:
    if p.is_file() and p.suffix not in ('.md', '.json', '.log', '.trx'):
        raise SystemExit('Unexpected report file: ' + str(p))
path = root / 'result.json'
data = json.loads(path.read_text())
for key, env in [('sha', 'REPORT_SHA'), ('run_id', 'REPORT_RUN_ID'), ('run_attempt', 'REPORT_RUN_ATTEMPT')]:
    if str(data.get(key)) != os.environ[env]:
        raise SystemExit('Report provenance mismatch: ' + key)
repo = os.environ['GITHUB_REPOSITORY']
run = os.environ['REPORT_RUN_ID']
attempt = os.environ['REPORT_RUN_ATTEMPT']
# Job results originate from GitHub, never from untrusted artifact metadata.
raw = subprocess.check_output(['gh', 'api', f'repos/{repo}/actions/runs/{run}/attempts/{attempt}/jobs?per_page=100'], text=True)
jobs = json.loads(raw)['jobs']
data['jobs'] = {name: next((j['conclusion'] for j in jobs if j['name'] == name), 'missing')
                for name in ('linux', 'windows', 'policy')}
data['ref'] = os.environ['REPORT_REF']
path.write_text(json.dumps(data, indent=2) + '\n')
