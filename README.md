# Traders Redone

Originalgetreue C#-Portierung von *Traders – The Intergalactic Trading Game* (Linel, 1991,
DOS-Fassung) mit anschließenden Erweiterungen (IQ je Gegner, Cheats, Übersetzungen,
moderne Grafik, Netzwerkspiel, Dedicated Server).

- **Aktueller Stand: [STATUS.md](STATUS.md)** · Zwischenstände: [CHANGELOG.md](CHANGELOG.md)
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

`original/` enthält die Originaldateien (privates Repo). `original/SHA256SUMS.*` sichert
die analysierte Fassung ab.
