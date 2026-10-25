# Experimente in CI

Liegt hier eine Datei `experiment.sh`, führt der Linux-Job sie nach dem Build aus:

```
bash ci/experiment.sh <ausgabe-ordner>
```

Alles, was das Skript in `<ausgabe-ordner>` schreibt, landet im Branch `ci-results` unter
`runs/<commit>/linux-results/experiment/`. Gedacht für Analysen, die eine .NET-Umgebung
brauchen (z. B. Referenzläufe des Originals im Emulator). Experimente laufen idealerweise
auf einem Branch `dev/...`, damit `main` sauber bleibt.
