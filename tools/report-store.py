"""Retain compact reports by explicit run IDs; Git history is intentionally not rewritten."""
import json
from pathlib import Path
import shutil
import sys

source, target = map(Path, sys.argv[1:])
data = json.loads((source / 'result.json').read_text())
run_id, attempt = int(data['run_id']), int(data['run_attempt'])
key = f'{run_id}-{attempt}'
dest = target / 'runs' / key
if dest.exists():
    shutil.rmtree(dest)
shutil.copytree(source, dest)
# Legacy report trees remain in history but no longer occupy the current tree.
for entry in (target / 'runs').iterdir():
    if not (entry / 'result.json').is_file():
        shutil.rmtree(entry)
reports = []
for entry in (target / 'runs').iterdir():
    item = json.loads((entry / 'result.json').read_text())
    reports.append((int(item['run_id']), int(item['run_attempt']), entry, item))
reports.sort(key=lambda item: item[:2], reverse=True)
for _, _, entry, _ in reports[40:]:
    shutil.rmtree(entry)
for name in ('commits', 'latest'):
    shutil.rmtree(target / name, ignore_errors=True)
for _, _, entry, item in reports[:40]:
    for name, identifier in [('commits', item['sha']), ('latest', item['ref'].replace('/', '_'))]:
        dest = target / name / identifier
        if not dest.exists():
            dest.mkdir(parents=True)
            # Index copies are deliberately just metadata + summary, not duplicate logs.
            for file in ('result.json', 'SUMMARY.md'):
                shutil.copy(entry / file, dest / file)
(target / 'index.md').write_text('# CI-Ergebnisse\n\n' + '\n'.join(
    f'- [{item[2].name}](runs/{item[2].name}/SUMMARY.md)' for item in reports[:40]) + '\n')
