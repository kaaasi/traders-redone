# Traders Redone

Originalgetreue C#-Portierung von *Traders – The Intergalactic Trading Game* (Linel, 1991,
DOS-Fassung) mit anschließenden Erweiterungen (IQ je Gegner, Cheats, Übersetzungen,
moderne Grafik, Netzwerkspiel, Dedicated Server).

- **Aktueller Stand: [STATUS.md](STATUS.md)** · Zwischenstände: [CHANGELOG.md](CHANGELOG.md)
- Runden und Spuren: [docs/arbeitspakete.md](docs/arbeitspakete.md)
- Referenzbilder aus den Videos: [reference/](reference/)
- Plan und Meilensteine: [docs/PORTIERUNGSPLAN.md](docs/PORTIERUNGSPLAN.md)
- Erkenntnisse aus dem Vorversuch: [docs/ERKENNTNISSE_VORVERSUCH.md](docs/ERKENNTNISSE_VORVERSUCH.md)
- Ressourcenformate: [docs/RESSOURCENFORMATE.md](docs/RESSOURCENFORMATE.md)
- Routinenbefunde: [docs/routines/](docs/routines/)

## Bauen

Voraussetzung: .NET-SDK nach `global.json` (10.0.402). Claude-Sitzungen: [CLAUDE.md](CLAUDE.md).

```
tools/pruefen.sh                                   # Prüfung wie in der CI (Build, Tests, Hashes, Marker)
dotnet run --project src/Traders.Tools -- export original/dos out/assets
dotnet run --project src/Traders.Viewer            # Asset-Viewer
```

Asset-Viewer: Pfeiltasten links/rechts = Seite, +/- = Zoom, L = Linie bei Zeile 200,
Esc = Ende. Koordinate und Palettenindex unter der Maus stehen im Fenstertitel.

## CI

`build.yml` führt `tools/pruefen.sh` auf Ubuntu für PRs (nicht für Drafts) und `main` aus; reine Doku-Änderungen bauen nicht.
Windows-Build und Viewer-Downloads (Windows, Linux, macOS) nur per Hand („Run workflow“). `werkzeuge.yml` hält SDK und
NuGet-Spiegel für Claude im Release `claude-werkzeuge` aktuell.

`original/` enthält die Originaldateien (privates Repo). `original/SHA256SUMS.*` sichert die analysierte Fassung ab.

## Mitarbeit

Start: [AGENTS.md](AGENTS.md) (Regeln), [docs/prozess/zusammenarbeit.md](docs/prozess/zusammenarbeit.md) (Rollen, Review, Merge),
Kurzaufrufe in [docs/vorlagen/](docs/vorlagen/), [Architektur](docs/ARCHITEKTUR.md).
