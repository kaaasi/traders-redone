"""Leitstelle: checks a work-package branch before integration into main.

Usage: python3 -I tools/pruefe-branch.py dev/apN-...   (run in the repo root)

Checks: commits ahead of main, files outside the package area or in shared files,
determinism rules for Traders.Core, routine documentation for new Core code, handover report,
CI result for the branch head (from branch ci-results) and merge conflicts with main.
"""
import json, re, subprocess, sys

def git(*args, check=True):
    r = subprocess.run(['git', *args], capture_output=True, text=True)
    if check and r.returncode != 0:
        raise SystemExit(f'git {" ".join(args)} failed: {r.stderr.strip()}')
    return r.stdout

branch = sys.argv[1]
ref = f'origin/{branch}'
git('fetch', '-q', 'origin')
m = re.match(r'dev/(ap\d+)', branch)
if not m:
    raise SystemExit('Branch must be dev/apN-...')
ap = m.group(1)
cfg = json.load(open('ci/arbeitspakete.json'))
allowed = cfg['packages'].get(ap, [])
shared = cfg['shared']

problems, warnings, infos = [], [], []
base = git('merge-base', 'origin/main', ref).strip()
commits = git('log', '--format=%h %s', f'origin/main..{ref}').strip().splitlines()
own_commits = git('log', '--no-merges', '--format=%h', f'origin/main..{ref}').strip().splitlines()
files = git('diff', '--name-only', base, ref).strip().splitlines()
infos.append(f'{len(commits)} Commits vor main ({len(own_commits)} ohne Merges), {len(files)} geänderte Dateien')

for f in files:
    if f == 'ci/experiment.sh':
        warnings.append('ci/experiment.sh geändert – wird bei der Übernahme nicht nach main übernommen')
    elif any(f.startswith(s) for s in shared):
        problems.append(f'Gemeinsame Datei geändert: {f}')
    elif not any(f.startswith(a) for a in allowed):
        problems.append(f'Außerhalb des Dateibereichs von {ap.upper()}: {f}')

# determinism rules for Traders.Core (added lines only)
forbidden = [r'DateTime\.(Now|UtcNow|Today)', r'Environment\.TickCount', r'new\s+Random\s*\(', r'Random\.Shared',
             r'Stopwatch', r'Guid\.NewGuid', r'Console\.', r'Thread\.Sleep', r'Task\.Delay']
diff_core = git('diff', '-U0', base, ref, '--', 'src/Traders.Core/')
for line in diff_core.splitlines():
    if line.startswith('+') and not line.startswith('+++'):
        for pat in forbidden:
            if re.search(pat, line):
                problems.append(f'Traders.Core nicht deterministisch ({pat}): {line[1:].strip()[:100]}')

new_core = [f for f in files if f.startswith('src/Traders.Core/') and f.endswith('.cs')]
new_routines = [f for f in files if f.startswith('docs/routines/')]
if new_core and not new_routines:
    warnings.append('Kern-Code geändert, aber kein Routinenbefund in docs/routines/ – prüfen, ob Originalroutinen portiert wurden')
new_tests = [f for f in files if f.startswith('tests/')]
if any(f.startswith('src/') and f.endswith('.cs') for f in files) and not new_tests:
    warnings.append('Quellcode geändert, aber keine Tests geändert')

# handover report
report_path = f'docs/uebergabe/{ap.upper()}.md'
report = subprocess.run(['git', 'show', f'{ref}:{report_path}'], capture_output=True, text=True)
if report.returncode != 0:
    problems.append(f'Übergabebericht {report_path} fehlt')
else:
    ready = re.search(r'Bereit zur Übernahme:\*\*\s*(\w+)', report.stdout)
    infos.append(f'Übergabebericht: bereit = {ready.group(1) if ready else "?"}')

# CI result for branch head
head = git('rev-parse', ref).strip()
git('fetch', '-q', 'origin', 'ci-results', check=False)
summary = subprocess.run(['git', 'show', f'origin/ci-results:runs/{head}/SUMMARY.md'], capture_output=True, text=True)
if summary.returncode != 0:
    problems.append(f'Kein CI-Bericht für Branch-Stand {head[:7]} (noch laufend oder nicht gepusht?)')
else:
    jobs = re.findall(r'^\| (\w+) \| (✅|❌) (\w+) \|', summary.stdout.split('\n## ')[0], re.M)
    failed = [j for j, mark, _ in jobs if mark != '✅']
    tests = re.findall(r'\*\*Tests:\*\* (\d+) bestanden, (\d+) fehlgeschlagen', summary.stdout)
    infos.append('CI: ' + ', '.join(f'{j} {r}' for j, _, r in jobs) + (f'; Tests {tests}' if tests else ''))
    if failed:
        problems.append(f'CI rot für {head[:7]}: {", ".join(failed)}')

# merge conflicts with main
mt = subprocess.run(['git', 'merge-tree', '--write-tree', '--name-only', 'origin/main', ref], capture_output=True, text=True)
if mt.returncode == 1:
    conflicted = [l for l in mt.stdout.splitlines()[1:] if l and not l.startswith('Auto-merging') and not l.startswith('CONFLICT')]
    problems.append('Merge-Konflikte mit main: ' + ', '.join(conflicted[:10]))

print(f'# Prüfung {branch} ({head[:7]})\n')
for i in infos: print(f'- {i}')
print('\n## Probleme' if problems else '\n## Probleme: keine')
for p in problems: print(f'- ❌ {p}')
print('\n## Hinweise' if warnings else '\n## Hinweise: keine')
for w in warnings: print(f'- ⚠️ {w}')
print('\n## Commits')
for c in commits: print(f'- {c}')
print('\n## Dateien')
for f in files: print(f'- {f}')
sys.exit(1 if problems else 0)
