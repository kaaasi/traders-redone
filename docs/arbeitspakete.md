# Arbeitspakete

Die Portierung läuft in **Runden**; je Runde arbeiten mehrere **Spuren** parallel, jede auf einem Branch `paket/<spur>` mit
einem PR, eigenen Dateien und klarer Abnahme. Regeln: [AGENTS.md](../AGENTS.md), Ablauf:
[prozess/zusammenarbeit.md](prozess/zusammenarbeit.md). Den Status pflegt jede Spur nur in ihrem eigenen Abschnitt; Plan-
abschnitte ändert die Leitstelle. Abgeschlossene Runden stehen in [archiv/](archiv/) (Paketphase AP1–AP5: [archiv/runde-0.md](archiv/runde-0.md)).

## Aktuelle Runde: – (Runde 1 abgeschlossen am 11.11.2026: [archiv/runde-1.md](archiv/runde-1.md))

Die nächste Leitstelle plant Runde 2 in einem neuen Chat (`vorlagen/leitstelle-start.md`, Auftrag „Runde 2 planen“).

## Vorschau: Runde 2 – Altbestand übernehmen (Plan folgt, Go nötig)

| Spur | Quelle | Inhalt |
|---|---|---|
| `refemu` | `dev/ap1-refemu` (`2f03e46`) | Tastatur mit Haltezeit, FXDRIVER als TSR, Code-Haken, Aufrufspur Grafiktreiber, OPL-Spur; auf neuen `main`, Übergabe nachholen |
| `grafik` | `dev/ap2-vga` (`3fc6033`) | Mode-X-VRAM, RAM-Atlas, Treiberroutinen `23CD`, Grafikhilfen `0A96`/`0A69`, Differenztests |
| `phasen` | `dev/ap5-phasen` (`92edea9`) | Aufrufgraph, `main()`, Hauptschleife, `docs/analyse/PHASEN.md` (Phasenfolge, Bedingungen, Video-Zeitstempel) |

Fahrplan danach (je Runde 2–4 Spuren): R3 Phasenmaschine + App-Host + Ton-Analyse · R4 Menü & Setup · R5 Anbau · R6 Produktion,
Fat Mike · R7 Handel · R8 Rakete & Piraten · R9 Ereignisse, Bilanz/Einkauf · R10 Roboterkampf · R11 Ende, Ton · R12 Gesamtpartie
(v1.0) · danach Erweiterungen ([PORTIERUNGSPLAN.md](PORTIERUNGSPLAN.md)).
