# Traders Redone

Originalgetreue C#-Portierung von *Traders – The Intergalactic Trading Game* (Linel, 1991,
DOS-Fassung) mit anschließenden Erweiterungen (IQ je Gegner, Cheats, Übersetzungen,
moderne Grafik, Netzwerkspiel, Dedicated Server).

- **Aktueller Stand: [STATUS.md](STATUS.md)** · Zwischenstände: [CHANGELOG.md](CHANGELOG.md)
- Parallele Arbeitspakete: [docs/ARBEITSPAKETE.md](docs/ARBEITSPAKETE.md)
- Referenzbilder aus den Videos: [reference/](reference/)
- Plan und Meilensteine: [docs/PORTIERUNGSPLAN.md](docs/PORTIERUNGSPLAN.md)
- Erkenntnisse aus dem Vorversuch: [docs/ERKENNTNISSE_VORVERSUCH.md](docs/ERKENNTNISSE_VORVERSUCH.md)
- Ressourcenformate: [docs/RESSOURCENFORMATE.md](docs/RESSOURCENFORMATE.md)
- Routinenbefunde: [docs/routines/](docs/routines/)

## Bauen

Voraussetzung: .NET 10 SDK.

```
dotnet build TradersRedone.slnx
dotnet test TradersRedone.slnx
dotnet run --project src/Traders.Tools -- export original/dos out/assets
dotnet run --project src/Traders.Viewer          # Asset-Viewer
```

Asset-Viewer: Pfeiltasten links/rechts = Seite, +/- = Zoom, L = Linie bei Zeile 200,
Esc = Ende. Koordinate und Palettenindex unter der Maus stehen im Fenstertitel.

## CI

Pushes auf `main` und `dev/...` sowie PRs bauen und testen unter Linux und Windows.
Kompakte Ergebnisse stehen nach Aktivierung des Publisher-Workflows auf `main` im Branch
`ci-results` (`commits/<sha>/result.json`, `latest/main/SUMMARY.md`). Vollständige Logs bleiben
7 Tage als Actions-Artefakte verfügbar. Der manuelle Workflow `Diagnostics` erzeugt Bilder,
Experimente und das Windows-Paket `traders-viewer-win-x64`.

`original/` enthält die Originaldateien (privates Repo). `original/SHA256SUMS.*` sichert
die analysierte Fassung ab.


## Mitarbeit

Start: [AGENTS.md](AGENTS.md), [CONTRIBUTING.md](CONTRIBUTING.md),
[Architektur](docs/ARCHITEKTUR.md), [Umgebungen](docs/UMGEBUNGEN.md).
