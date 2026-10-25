# Traders Redone

Originalgetreue C#-Portierung von *Traders – The Intergalactic Trading Game* (Linel, 1991,
DOS-Fassung) mit anschließenden Erweiterungen (IQ je Gegner, Cheats, Übersetzungen,
moderne Grafik, Netzwerkspiel, Dedicated Server).

- **Aktueller Stand: [STATUS.md](STATUS.md)** · Zwischenstände: [CHANGELOG.md](CHANGELOG.md)
- Plan und Meilensteine: [docs/PORTIERUNGSPLAN.md](docs/PORTIERUNGSPLAN.md)
- Erkenntnisse aus dem Vorversuch: [docs/ERKENNTNISSE_VORVERSUCH.md](docs/ERKENNTNISSE_VORVERSUCH.md)
- Routinenbefunde: [docs/routines/](docs/routines/)

## Bauen

Voraussetzung: .NET 10 SDK.

```
dotnet build TradersRedone.slnx
dotnet test TradersRedone.slnx
dotnet run --project src/Traders.Tools -- export original/dos out/assets
```

`original/` enthält die Originaldateien (privates Repo). `original/SHA256SUMS.*` sichert
die analysierte Fassung ab.
