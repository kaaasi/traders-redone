# Traders Redone

Originalgetreue C#-Portierung von *Traders – The Intergalactic Trading Game* (Linel, 1991,
DOS-Fassung) mit anschließenden Erweiterungen (IQ je Gegner, Cheats, Übersetzungen,
moderne Grafik, Netzwerkspiel, Dedicated Server).

- **Aktueller Stand: [STATUS.md](STATUS.md)** · Zwischenstände: [CHANGELOG.md](CHANGELOG.md)
- Runden und Spuren: [docs/arbeitspakete.md](docs/arbeitspakete.md)
- Originaldateien und Referenzbilder: privates Repo `kaaasi/traders-referenz` (`tools/claude/fetch-reference.sh`)
- Plan und Meilensteine: [docs/PORTIERUNGSPLAN.md](docs/PORTIERUNGSPLAN.md)
- Erkenntnisse aus dem Vorversuch: [docs/ERKENNTNISSE_VORVERSUCH.md](docs/ERKENNTNISSE_VORVERSUCH.md)
- Ressourcenformate: [docs/RESSOURCENFORMATE.md](docs/RESSOURCENFORMATE.md)
- Routinenbefunde: [docs/routines/](docs/routines/)

## Bauen

Voraussetzung: .NET-SDK nach `global.json` (10.0.402). Claude-Sitzungen: [CLAUDE.md](CLAUDE.md).

```
tools/pruefen.sh --schnell                         # Prüfung wie in der CI (Build, Tests, Hashes, Marker)
tools/claude/fetch-reference.sh original           # Originaldateien nach gamedata/ (Leserecht auf traders-referenz)
dotnet run --project src/Traders.Tools -- export gamedata/dos out/assets
dotnet run --project src/Traders.Viewer            # Asset-Viewer
```

Asset-Viewer: Pfeiltasten links/rechts = Seite, +/- = Zoom, L = Linie bei Zeile 200,
Esc = Ende. Koordinate und Palettenindex unter der Maus stehen im Fenstertitel.

## CI

`build.yml` führt `tools/pruefen.sh` auf Ubuntu für PRs (nicht für Drafts) und `main` aus; reine Doku-Änderungen bauen nicht.
Windows-Build und Viewer-Downloads (Windows, Linux, macOS) nur per Hand („Run workflow“). `werkzeuge.yml` hält SDK und
NuGet-Spiegel für Claude im Release `claude-werkzeuge` aktuell.

Die Originaldateien liegen nur im privaten Repo `kaaasi/traders-referenz`; die CI holt sie über das Secret
`TRADERS_REFERENZ_TOKEN`. `tools/originale/SHA256SUMS.*` sichert die analysierte Fassung ab.

## Mitarbeit

Start: [AGENTS.md](AGENTS.md) (Regeln), [docs/prozess/zusammenarbeit.md](docs/prozess/zusammenarbeit.md) (Rollen, Review, Merge),
Kurzaufrufe in [docs/vorlagen/](docs/vorlagen/), [Architektur](docs/ARCHITEKTUR.md).
