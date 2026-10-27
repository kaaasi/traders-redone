import json
import os
from pathlib import Path
import subprocess
import sys
import tempfile
import unittest

ROOT = Path(__file__).resolve().parents[2]


class ReportTests(unittest.TestCase):
    def test_compact_report_records_identity_and_excludes_images(self):
        with tempfile.TemporaryDirectory() as tmp:
            base = Path(tmp)
            logs = base / 'results/linux-results/logs'
            logs.mkdir(parents=True)
            (logs / 'build.log').write_text('x' * 40000)
            images = logs.parent / 'images'
            images.mkdir()
            (images / 'image.png').write_bytes(b'png')
            env = dict(os.environ, GITHUB_RUN_ID='10', GITHUB_RUN_ATTEMPT='2')
            subprocess.run([sys.executable, str(ROOT / 'tools/ci-report.py'), str(base / 'results'),
                            str(base / 'report'), 'a' * 40, 'dev/ap1-test',
                            'linux=success', 'windows=failure', 'policy=success'], env=env, check=True, capture_output=True)
            data = json.loads((base / 'report/result.json').read_text())
            self.assertEqual('2', data['run_attempt'])
            self.assertEqual('failure', data['jobs']['windows'])
            self.assertFalse((base / 'report/linux-results/images').exists())
            self.assertEqual(32000, len((base / 'report/linux-results/logs/build.log').read_text()))

    def test_out_of_order_publishing_does_not_replace_newer_run(self):
        with tempfile.TemporaryDirectory() as tmp:
            base = Path(tmp)
            source = base / 'source'
            source.mkdir()
            sha = 'a' * 40
            for run, attempt in [(20, 1), (19, 1), (20, 2)]:
                (source / 'result.json').write_text(json.dumps({'sha': sha, 'ref': 'dev/ap1-x',
                    'run_id': str(run), 'run_attempt': str(attempt)}))
                (source / 'SUMMARY.md').write_text(f'{run}-{attempt}')
                subprocess.run([sys.executable, str(ROOT / 'tools/report-store.py'), str(source), str(base / 'store')], check=True)
            data = json.loads((base / f'store/commits/{sha}/result.json').read_text())
            self.assertEqual(('20', '2'), (data['run_id'], data['run_attempt']))
            self.assertEqual(3, len(list((base / 'store/runs').iterdir())))

    def test_cancelled_run_cannot_displace_completed_commit_checks(self):
        with tempfile.TemporaryDirectory() as tmp:
            base = Path(tmp)
            source = base / 'source'
            source.mkdir()
            sha = 'a' * 40
            for run, status in [(10, 'success'), (11, 'cancelled'), (12, 'failure')]:
                (source / 'result.json').write_text(json.dumps({'sha': sha, 'ref': 'main',
                    'run_id': str(run), 'run_attempt': '1',
                    'jobs': dict.fromkeys(('linux', 'windows', 'policy'), status)}))
                (source / 'SUMMARY.md').write_text(status)
                subprocess.run([sys.executable, str(ROOT / 'tools/report-store.py'), str(source), str(base / 'store')], check=True)
                selected = json.loads((base / f'store/commits/{sha}/result.json').read_text())
                self.assertEqual('failure' if run == 12 else 'success', selected['jobs']['linux'])
                latest = json.loads((base / 'store/latest/main/result.json').read_text())
                self.assertEqual(status, latest['jobs']['linux'])
