"""Provider-neutral branch overview; trailers are provenance, never authorization."""
import subprocess
from repo_policy import check_report, is_ready


def git(*args):
    return subprocess.run(['git', *args], capture_output=True, text=True, check=True).stdout.strip()


git('fetch', '--quiet', 'origin')
print('| Branch | voraus | zurück | Übergabe | CI |')
print('|---|---:|---:|---|---|')
for ref in git('for-each-ref', '--format=%(refname:short)', 'refs/remotes/origin/dev/ap*').splitlines():
    ap = ref.split('/')[-1].split('-')[0].upper()
    sha = git('rev-parse', ref)
    report = subprocess.run(['git', 'show', f'{ref}:docs/uebergabe/{ap}.md'], capture_output=True, text=True)
    ci = subprocess.run(['git', 'show', f'origin/ci-results:commits/{sha}/result.json'], capture_output=True, text=True)
    state = 'grün' if ci.returncode == 0 and not check_report(ci.stdout, sha) else 'fehlt/rot'
    print(f'| {ref} | {git("rev-list", "--count", "origin/main.." + ref)} | {git("rev-list", "--count", ref + "..origin/main")} | {"ja" if is_ready(report.stdout) else "nein"} | {state} |')
print('\nLeitstellenberechtigung wird durch dokumentierte Rollenvergabe bestimmt, nicht durch Session-Trailer.')
