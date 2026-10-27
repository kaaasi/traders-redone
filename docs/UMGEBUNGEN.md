# Entwicklungsumgebungen

Voraussetzungen: Git mit merge-tree --write-tree, Python 3.10+, Bash und .NET gemäß global.json.
Analyse mit tools/segmap.py benötigt ndisasm (NASM). Viewer-Diagnosen unter Linux benötigen Xvfb
und die MonoGame-Systembibliotheken. gh wird für CI-Abfragen benötigt, nicht für den Build.
Keine Secrets in Logs ausgeben. Authentifizierung der jeweiligen Umgebung verwenden.

## SDK und Pakete

global.json legt .NET 10.0.100 fest und erlaubt mit latestFeature neuere .NET-10-Feature-Bands.
Dies ist bewusst für verfügbare Cloud-SDKs gewählt; CI protokolliert die SDK-Version. Bei exakt
zu reproduzierenden Befunden zusätzlich dotnet --info und Paketversionen festhalten.
NuGet-Lockdateien werden mit dotnet restore --use-lock-file erzeugt. Erst vollständig erzeugte
und geprüfte Lockdateien committen; danach --locked-mode in CI aktivieren. Keine Lockdateien
von Hand erfinden. Die Umstellung ist in docs/UMSETZUNGSSTAND.md nachzuverfolgen.

## CI-Fallback

Wenn lokal kein SDK verfügbar ist, Branch pushen und vorhandene CI prüfen. tools/ci-wait.sh
wartet begrenzt auf den kompakten Bericht. Vollständige Logs, TRX und Bilder bleiben Actions-
Artefakte (kurze Aufbewahrung). Für eingeschränkte Agenten enthält ci-results SUMMARY.md,
result.json und gekürzte Textlogs. Fehlende Reports sind kein Testnachweis.
Manuelle Diagnosen über diagnostics.yml erzeugen Viewer-Paket/Bilder und führen optional ein
explizit benanntes experiments/apN/*.sh aus. Kein ungeprüfter PR-Code mit Schreibrechten.

## Referenzbilder bei reinen Codeaufgaben auslassen

```bash
git sparse-checkout init --cone
git sparse-checkout set src tests tools ci docs original .github experiments
```

Wurzeldateien bleiben im Cone-Modus verfügbar. Für Bildanalyse reference wieder hinzufügen.
Originaldateien für Tests behalten. Siehe reference/README.md; keine LFS-Migration erforderlich.
