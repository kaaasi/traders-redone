# Umsetzung der Repositoryanalyse

Arbeitsbranch: dev/repo-collaboration; Basis main 717a226. Alle Änderungen sind im PR prüfbar.

## Implementiert

- Gemeinsame Regeln, CLAUDE-Einstieg, CONTRIBUTING, Umgebungen und Architekturvertrag.
- Isolation paralleler Chats, Rollenübergabe, aktualisierte Übergabevorlage und Branch-Inventur.
- Exakte Dateibereiche, paketbezogene Routinenordner, versionierte Experimente.
- Fail-closed Integrationsprüfung, Architekturprüfung, Tests für Fehlerfälle und Reports.
- Read-only Build-Jobs; Publisher verwendet main-Code und authentische GitHub-Jobergebnisse.
- Kompakte Reports pro Run/Attempt, begrenztes Warten und konfliktfeste Veröffentlichungsversuche.
- Manuelle Diagnosen/Viewer-Builds, Aufbewahrungsfristen, Concurrency und fixierte Action-SHAs.
- Durch echten Actions-Restore erzeugte NuGet-Lockdateien und Locked Restore.
- Editorconfig/LF, Ignore-Ergänzungen, Issue-/PR-Vorlagen, Referenzindex und Testdatenlizenz.
- Int16Math-IDIV-Fehlerpfad und gemeinsame Repositorypfade der Tests.

## Aktivierung und bewusst nicht erfundene Ergebnisse

- publish-report.yml wird erst nach Integration dieses PRs auf main aktiv. Vorher Actions direkt
  prüfen; ein fehlender neuer JSON-Bericht ist kein grüner Integrationsnachweis.
- Paketstände AP1/AP2/AP3/AP5 bleiben separat. Insbesondere AP1/AP2 melden nicht übernahmebereit.
  Ihre alten Übergaben werden nicht stellvertretend als aktuell/fertig ausgegeben. Ein verifizierter
  Snapshot steht in ARBEITSPAKETE.md. Die neuen Regeln sind nach Integration in Paketbranches zu mergen.
- Upstream-Revision der bereits vorhandenen CPU-Testdaten ist nicht überliefert. Lizenz liegt bei;
  beim nächsten bewussten Regenerieren Revision/Hashes nach Vorlage festhalten.
- Bestehende ci-results-Historie wird nicht umgeschrieben. Kleine zukünftige Reports begrenzen
  Wachstum; vorherige große Git-Objekte verschwinden nicht durch Löschen im aktuellen Baum.
- Kontoweites Actions-Ausgabenlimit und ein optionales GitHub Project lassen sich nicht über die
  verfügbaren Repository-Werkzeuge konfigurieren. GitHub Free bleibt Voraussetzung.
- Kein kosmetischer CPU-Großumbau und keine neue Netzwerkfunktion: beides ist bedarfsabhängig und
  würde laufende AP1-Arbeit beziehungsweise die Reihenfolge Originalportierung vor Erweiterungen stören.
- Meilensteintags erst für ausdrücklich freigegebene Meilensteine setzen; keine Version neu erfunden.

## Prüfungen

Python-Policy-/Reporttests und Architekturprüfung lokal; C#-Build und Tests über Actions.
Konkrete Run-Links und Ergebnisse werden im PR dokumentiert. Kein lokales .NET-SDK verfügbar.

Erster vollständiger Lauf: https://github.com/kaaasi/traders-redone/actions/runs/40494382881
Linux/Windows erfolgreich; Linux 391 Tests bestanden, keine fehlgeschlagen.
