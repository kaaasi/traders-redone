"""Read package branch, exact CI report and mergeability; never merge or push."""
import argparse
import json
import re
import subprocess
import sys
from pathlib import Path
from repo_policy import check_paths, check_report, is_ready, check_merge_result


def git(*args, check=True):
    return subprocess.run(['git', *args], text=True, capture_output=True, check=check)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('branch')
    parser.add_argument('--integrate', action='store_true')
    args = parser.parse_args()
    if not re.fullmatch(r'dev/ap\d+-[a-z0-9-]+', args.branch):
        parser.error('Branch muss dev/apN-name entsprechen')
    package = args.branch.split('/')[1].split('-')[0]
    cfg = json.loads(Path('ci/arbeitspakete.json').read_text())
    if package not in cfg['packages']:
        parser.error('Unbekanntes Arbeitspaket')
    git('fetch', '--quiet', 'origin')
    ref = 'origin/' + args.branch
    head = git('rev-parse', ref).stdout.strip()
    base = git('merge-base', 'origin/main', ref).stdout.strip()
    paths = git('diff', '--name-only', base, ref).stdout.splitlines()
    errors = check_paths(paths, package, cfg)
    report = git('show', f'{ref}:docs/uebergabe/{package.upper()}.md', check=False)
    if report.returncode:
        errors.append('Übergabebericht fehlt')
    elif not is_ready(report.stdout):
        if args.integrate:
            errors.append('Übergabe ist nicht ausdrücklich bereit')
        else:
            print('Hinweis: Übergabe noch nicht bereit (Vorprüfung)')
    # Read metadata, never infer success from an empty Markdown table.
    ci = git('show', f'origin/ci-results:commits/{head}/result.json', check=False)
    errors += check_report(ci.stdout, head) if ci.returncode == 0 else ['CI-JSON für Branch-Kopf fehlt']
    merge = git('merge-tree', '--write-tree', 'origin/main', ref, check=False)
    errors += check_merge_result(merge.returncode, merge.stderr)
    # A cheap supplement to code review and architecture tests, not a proof of determinism.
    diff = git('diff', '-U0', base, ref, '--', 'src/Traders.Core/').stdout
    forbidden = r'DateTime\.(Now|UtcNow|Today)|Environment\.TickCount|new\s+Random\s*\(|Random\.Shared|Stopwatch|Guid\.NewGuid|Console\.|Thread\.Sleep|Task\.Delay'
    for line in diff.splitlines():
        if line.startswith('+') and not line.startswith('+++') and re.search(forbidden, line):
            errors.append('Core: potenziell nichtdeterministische Änderung: ' + line[1:120])
    print(f'{args.branch}: {head}; {len(paths)} Dateien')
    for error in errors:
        print('FEHLER: ' + error)
    return bool(errors)


if __name__ == '__main__':
    try:
        sys.exit(main())
    except subprocess.CalledProcessError as error:
        print('Git-Prüfung fehlgeschlagen: ' + error.stderr, file=sys.stderr)
        sys.exit(1)
