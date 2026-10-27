# Zusammenarbeit

## Aufgabe und Isolation

Leitstelle/Nutzer vergibt eine Aufgabe mit Ziel, Dateibereich, Schnittstellen und Abnahmekriterien.
Ein Issue kann die Aufgabe tragen; ein PR dokumentiert die konkrete Integration. Die Leitstelle
führt die Zuweisung in docs/ARBEITSPAKETE.md, der Paket-Chat seinen Übergabebericht. Ein Project
ist optional und darf keine zweite manuell gepflegte Statusquelle werden.

Je gleichzeitigem Bearbeiter ein eigener Worktree oder Cloud-Clone und ein eigener Branch:

```bash
git fetch origin
git worktree add ../traders-ap2 dev/ap2-vga
# Neue Teilaufgabe statt zweitem Bearbeiter auf demselben Branch:
git worktree add -b dev/ap2-blit ../traders-ap2-blit origin/main
```

Unteraufgaben benötigen eigene Übergaben und koordinierte Dateibereiche. Das Prüfwerkzeug
akzeptiert Paket-Branches; die Paketkennung bestimmt die Grenzen. Sich überschneidende Aufgaben
werden nacheinander bearbeitet. Die Leitstelle hält die Vergabe aktuell, inklusive pausierter Chats.

## Entwickeln und prüfen

```bash
dotnet build TradersRedone.slnx -c Release
dotnet test TradersRedone.slnx -c Release --no-build
python3 -m unittest discover -s tests/tools -v
python3 tools/check-architecture.py
```

Kleine fachlich zusammenhängende Commits erstellen. Vor Integration main hineinmergen,
Tests auf dem kombinierten Stand ausführen und einen PR öffnen. Der PR enthält Originalbefunde,
Tests mit Commit/Run-Link und Risiken. CI für einen früheren Commit genügt nicht.

```bash
python3 tools/pruefe-branch.py dev/ap2-vga --integrate
```

Ohne --integrate dient der Befehl als Vorprüfung; fehlende Übernahmereife bleibt dann ein Hinweis.
Nur die Leitstelle übernimmt mit Merge-Commit und pflegt STATUS.md/CHANGELOG.md. Bei weiterlaufenden
Paket-Branches bleibt die Historie erhalten. Abgeschlossene Branches erst nach Prüfung unintegrierter
Arbeit löschen. Keine veröffentlichten Historien umschreiben.

## Anbieterwechsel und Leitstellenwechsel

Übergabevorlage aus docs/uebergabe/_vorlage.md verwenden. Vor Übergabe committen und pushen;
Arbeitsbaum auf ungesicherte Änderungen prüfen. Alten Bearbeiter pausieren, neuen Bearbeiter und
Zeitpunkt dokumentieren. Der Nachfolger verifiziert Branch und Code-Commit selbst. Sitzungslinks
sind optionale Hinweise, keine Berechtigung. Leitstellenwechsel im Übergabeprotokoll dokumentieren;
zu einem Zeitpunkt gibt es genau eine aktive Leitstelle.

## Experimente und Releases

Experimente liegen unter experiments/apN/ und dürfen nach Review versioniert werden. Keine
branchweite ci/experiment.sh mehr neu anlegen; bestehende Dateien vor Integration migrieren.
Experimente laufen nur manuell über den Diagnoseworkflow und dessen expliziten Pfad.

GitHub Free im privaten Repo: PRs, Issues und Actions nutzen; verpflichtende Reviews, Rulesets
und Branchschutz sind keine Voraussetzung. Unsere Regeln sind organisatorisch. Im Konto bezahlte
Actions-Nutzung deaktiviert lassen und das verfügbare Minuten-/Speicherbudget kontrollieren.

Meilensteine nach erfolgreicher Integration durch den Nutzer oder beauftragte Leitstelle markieren:

```bash
git tag -a v0.X GEPRUEFTER_COMMIT -m 'Meilenstein v0.X'
git push origin v0.X
```

Existierende Tags nicht versetzen. Tags sind unveränderliche Meilensteine, Branches Arbeitsstände.
