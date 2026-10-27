"""Builds the ci-results report from the per-job result folders.

Usage: python3 tools/ci-report.py <results-dir> <report-dir> <sha> <ref> <job=result>...
<results-dir> contains one folder per job artifact (e.g. linux-results/, windows-results/),
each with steps.tsv, logs/, tests/*.trx and optional images/ and experiment/.
"""
import sys, os, shutil, glob, datetime, json
import xml.etree.ElementTree as ET

src, dst, sha, ref = sys.argv[1:5]
jobs = dict(a.split('=', 1) for a in sys.argv[5:])
os.makedirs(dst, exist_ok=True)
NS = '{http://microsoft.com/schemas/VisualStudio/TeamTest/2010}'

lines = [f'# CI-Ergebnis `{sha[:7]}` ({ref})', '',
         f'Zeit: {datetime.datetime.utcnow():%Y-%m-%d %H:%M} UTC', '',
         '| Job | Ergebnis |', '|---|---|']
for j, r in jobs.items():
    lines.append(f'| {j} | {"✅" if r == "success" else "❌"} {r} |')

for job_dir in sorted(glob.glob(os.path.join(src, '*'))):
    job = os.path.basename(job_dir)
    lines += ['', f'## {job}', '']
    steps = os.path.join(job_dir, 'steps.tsv')
    if os.path.exists(steps):
        lines += ['| Schritt | Exit | Sekunden |', '|---|---:|---:|']
        for row in open(steps, encoding='utf-8', errors='replace'):
            parts = row.rstrip('\n').split('\t')
            if len(parts) == 3:
                lines.append(f'| {parts[0]} | {"✅" if parts[1] == "0" else "❌ " + parts[1]} | {parts[2]} |')
    for trx in glob.glob(os.path.join(job_dir, 'tests', '*.trx')):
        root = ET.parse(trx).getroot()
        results = root.iter(NS + 'UnitTestResult')
        passed, failed, other = [], [], []
        for r in results:
            name, outcome = r.get('testName'), r.get('outcome')
            if outcome == 'Passed':
                passed.append(name)
            elif outcome == 'Failed':
                msg = r.find(f'{NS}Output/{NS}ErrorInfo/{NS}Message')
                st = r.find(f'{NS}Output/{NS}ErrorInfo/{NS}StackTrace')
                failed.append((name, (msg.text if msg is not None else '') or '', (st.text if st is not None else '') or ''))
            else:
                other.append((name, outcome))
        lines += ['', f'**Tests:** {len(passed)} bestanden, {len(failed)} fehlgeschlagen, {len(other)} sonstige', '']
        for name, msg, st in failed:
            lines += [f'### ❌ {name}', '```', msg.strip()[:3000], st.strip()[:1500], '```']
        if passed:
            lines += ['<details><summary>Bestandene Tests</summary>', '']
            lines += [f'- {n}' for n in sorted(passed)]
            lines += ['', '</details>']
    # copy logs, images, experiment output
    for sub in ('logs',):
        p = os.path.join(job_dir, sub)
        if os.path.isdir(p):
            target = os.path.join(dst, job, sub)
            os.makedirs(target, exist_ok=True)
            for log in glob.glob(os.path.join(p, '*.log')):
                with open(log, encoding='utf-8', errors='replace') as stream:
                    tail = stream.read()[-32000:]
                open(os.path.join(target, os.path.basename(log)), 'w', encoding='utf-8').write(tail)
    for trx in glob.glob(os.path.join(job_dir, 'tests', '*.trx')):
        os.makedirs(os.path.join(dst, job, 'tests'), exist_ok=True)
        shutil.copy(trx, os.path.join(dst, job, 'tests'))

open(os.path.join(dst, 'SUMMARY.md'), 'w', encoding='utf-8').write('\n'.join(lines) + '\n')
print('\n'.join(lines))


metadata = {'schema_version': 1, 'sha': sha, 'ref': ref, 'jobs': jobs,
            'run_id': os.environ['GITHUB_RUN_ID'], 'run_attempt': os.environ['GITHUB_RUN_ATTEMPT'],
            'created_at': datetime.datetime.now(datetime.timezone.utc).isoformat()}
with open(os.path.join(dst, 'result.json'), 'w', encoding='utf-8') as stream:
    json.dump(metadata, stream, indent=2)
