"""Shared, fail-closed repository policy helpers (no network or mutations)."""
import json
import re


def matches(path, rule):
    if path.startswith('/') or '..' in path.split('/'):
        return False
    if rule.endswith('/'):
        return path.startswith(rule)
    if rule.endswith('_') or rule.endswith('-'):
        return path.startswith(rule) and '/' not in path[len(rule):]
    return path == rule


def check_paths(paths, package, config):
    if package not in config['packages']:
        return [f'Unbekanntes Paket: {package}']
    errors = []
    for path in paths:
        if path == 'ci/experiment.sh':
            errors.append('ci/experiment.sh vor Integration nach experiments/apN/ migrieren')
        elif any(matches(path, p) for p in config['shared']):
            errors.append(f'Gemeinsame Datei geändert: {path}')
        elif not any(matches(path, p) for p in config['packages'][package]):
            errors.append(f'Außerhalb von {package}: {path}')
    return errors


def check_report(text, sha):
    try:
        data = json.loads(text)
    except (ValueError, TypeError):
        return ['CI-Bericht ist kein gültiges JSON']
    if not isinstance(data, dict):
        return ['CI-Bericht muss ein Objekt sein']
    errors = []
    if data.get('schema_version') != 1 or data.get('sha') != sha:
        errors.append('CI-Schema oder Commit stimmt nicht überein')
    jobs = data.get('jobs')
    if not isinstance(jobs, dict):
        return errors + ['CI-Jobs fehlen']
    for job in ('linux', 'windows', 'policy'):
        if jobs.get(job) != 'success':
            errors.append(f'CI-Job {job}: {jobs.get(job, "fehlt")}')
    return errors


def is_ready(report):
    return bool(re.search(r'^- \*\*Bereit zur Übernahme:\*\*\s*ja\s*$', report, re.M | re.I))


def check_merge_result(code, stderr=''):
    if code == 0:
        return []
    return ['Merge-Konflikte' if code == 1 else f'Merge-Prüfung fehlgeschlagen ({code}): {stderr.strip()}']
