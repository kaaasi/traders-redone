import json
import sys
import unittest
from pathlib import Path
sys.path.insert(0, str(Path(__file__).resolve().parents[2] / 'tools'))
from repo_policy import matches, check_report, check_paths, is_ready, check_merge_result


class IntegrationPolicyTests(unittest.TestCase):
    def test_exact_file_and_directory_boundaries(self):
        self.assertTrue(matches('a/file.cs', 'a/'))
        self.assertFalse(matches('ab/file.cs', 'a/'))
        self.assertFalse(matches('STATUS.md.backup', 'STATUS.md'))
        self.assertFalse(matches('a/../b', 'a/'))
        self.assertTrue(matches('docs/0A96_test.md', 'docs/0A96_'))

    def test_missing_malformed_or_wrong_commit_never_passes(self):
        for value in ('', '[]', '{}', '{broken', json.dumps({'schema_version': 1, 'sha': 'other', 'jobs': {}})):
            self.assertTrue(check_report(value, 'head'))

    def test_every_required_job_must_succeed(self):
        data = {'schema_version': 1, 'sha': 'head', 'jobs': dict.fromkeys(('linux', 'windows', 'policy'), 'success')}
        self.assertEqual([], check_report(json.dumps(data), 'head'))
        for status in ('failure', 'cancelled', 'skipped', None):
            data['jobs']['linux'] = status
            self.assertTrue(check_report(json.dumps(data), 'head'))

    def test_readiness_requires_explicit_yes(self):
        self.assertTrue(is_ready('- **Bereit zur Übernahme:** ja\n'))
        for report in ('', '- **Bereit zur Übernahme:** nein', '- **Bereit zur Übernahme:** ja, später'):
            self.assertFalse(is_ready(report))

    def test_unknown_package_and_shared_paths_fail(self):
        cfg = {'packages': {'ap1': ['src/ap1/']}, 'shared': ['src/ap1/shared.cs']}
        self.assertTrue(check_paths([], 'ap2', cfg))
        self.assertTrue(check_paths(['src/ap1/shared.cs'], 'ap1', cfg))
        self.assertTrue(check_paths(['ci/experiment.sh'], 'ap1', cfg))
        self.assertEqual([], check_paths(['src/ap1/ok.cs'], 'ap1', cfg))

    def test_merge_errors_are_not_success(self):
        self.assertEqual([], check_merge_result(0))
        for code in (1, 2, 128, -9):
            self.assertTrue(check_merge_result(code, 'failure'))
