# CI

Build/Test: .github/workflows/ci.yml (nur Leserechte).
Veröffentlichung: publish-report.yml führt ausschließlich main-Code aus und prüft
Jobstatus über GitHub, bevor kompakte Artefakte nach ci-results übernommen werden.
Der Publisher wird erst wirksam, wenn sein Workflow auf main liegt.
Diagnosen/Experimente: diagnostics.yml, siehe experiments/README.md.
Ergebnisformat: result.json schema_version=1, sha, ref, run_id, run_attempt, jobs.
Erforderliche Jobs: linux, windows, policy; jeder muss success sein.
Historische Markdown-Berichte gelten nicht als maschinenlesbarer Integrationsnachweis.

40 Berichte im aktuellen Baum; frühere Git-Objekte bleiben in der Historie. Keine
automatische Historienumschreibung. Speicherwachstum beobachten.
