"""Leitstelle: overview of all work-package branches and an audit of main.

Usage: python3 -I tools/lagebericht.py [--seit <commit>]

- Per dev/ap* branch: commits ahead of main, behind main, last commit, handover flag, CI result.
- Audit: first-parent commits on main since <commit> (default: the commit that introduced the
  Leitstelle rules) that were not made by the Leitstelle session.
"""
import json, re, subprocess, sys

def git(*a, check=True):
    r = subprocess.run(['git', *a], capture_output=True, text=True)
    if check and r.returncode != 0:
        raise SystemExit(r.stderr)
    return r.stdout.strip()

cfg = json.load(open('ci/arbeitspakete.json'))
session = cfg['leitstelle_session']
git('fetch', '-q', '--prune', 'origin')
since = sys.argv[sys.argv.index('--seit') + 1] if '--seit' in sys.argv else \
    git('log', '--format=%H', '-1', '--grep=Leitstelle-Regeln', 'origin/main')

print('# Lagebericht\n')
print('| Branch | vor main | hinter main | letzter Commit | Übergabe bereit | CI (letzter Stand) |')
print('|---|---:|---:|---|---|---|')
for b in git('branch', '-r', '--list', 'origin/dev/ap*').split():
    name = b.removeprefix('origin/')
    ahead = git('rev-list', '--count', f'origin/main..{b}')
    behind = git('rev-list', '--count', f'{b}..origin/main')
    last = git('log', '-1', '--format=%cd · %s', '--date=format:%d.%m. %H:%M', b)[:70]
    ap = re.match(r'dev/(ap\d+)', name)
    ready = '–'
    if ap:
        r = subprocess.run(['git', 'show', f'{b}:docs/uebergabe/{ap.group(1).upper()}.md'], capture_output=True, text=True)
        if r.returncode == 0:
            m = re.search(r'Bereit zur Übernahme:\*\*\s*(\w+)', r.stdout)
            ready = m.group(1) if m else '?'
    head = git('rev-parse', b)
    s = subprocess.run(['git', 'show', f'origin/ci-results:runs/{head}/SUMMARY.md'], capture_output=True, text=True)
    if s.returncode != 0:
        ci = 'kein Bericht'
    else:
        jobs = re.findall(r'^\| (\w+) \| (✅|❌)', s.stdout.split('\n## ')[0], re.M)
        ci = ' '.join(f'{j}{m}' for j, m in jobs)
    print(f'| `{name}` | {ahead} | {behind} | {last} | {ready} | {ci} |')

print('\n## Wächter: Commits auf main seit Einführung der Leitstelle\n')
bad = []
if since:
    for line in git('log', '--first-parent', '--format=%h%x09%s%x09%(trailers:key=Claude-Session,valueonly,separator=)',
                    f'{since}..origin/main').splitlines():
        h, subj, trailer = (line.split('\t') + ['', ''])[:3]
        if session not in trailer:
            bad.append(f'- ❌ `{h}` {subj} (nicht von der Leitstelle)')
print('\n'.join(bad) if bad else '- ✅ alle Commits auf main stammen von der Leitstelle')
