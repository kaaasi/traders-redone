# Review: Vorlage Hörvergleich (PR #278)

Datum 2026-11-24 · Head `02f44dd` (`prozess/vorlage-hoervergleich`) · Base `main` `e1df2a6` · Reviewer: Claude Haiku 5.5, frische Sitzung.
Nur Codelesen: Checkout ohne Build und ohne Tests. Kein Testnachweis nach `zusammenarbeit.md` §6 Nr. 4.

## 1. Merge-Empfehlung

**ja.** Reine Doku (+57 Zeilen, nur `docs/vorlagen/hoervergleich.md`). Kein Widerspruch zu AGENTS.md §2.1, §6 und §7 gefunden. Befehle, Pfade, Stücke und Merker stimmen mit dem Code überein (siehe 3).

## 2. Befunde

- **P3** `docs/vorlagen/hoervergleich.md:6`: Der Kurzaufruf verlangt „binde traders-referenz mit push ein“. Der Ablauf braucht nur Lesezugriff (Vergleichsquelle `TRADERS.BAT`, Schritt 1). AGENTS.md §2.1 sagt, dass nur Leitstelle und Nutzer dort ablegen. Vorschlag: „mit access read“.

Keine Befunde P0–P2. Kein „Verdacht“ ohne Beleg.

## 3. Ausgeführte Prüfungen (nur Lesen, per Grep und Datei)

- `src/Traders.App/HostArguments.cs`: Standard `SkipRobotPhase` und `ReturnToMenu` = true. Startbefehl und Verhalten passen.
- `src/Traders.Tools/Program.cs`: `opl-wav <trace> <out> [rate]` passt.
- `tests/Traders.Tests/Audio/OplTraceTests.cs`: `OPL_WAV_OUT`, `opl-menu.wav`, `[GameDataFact]` mit `TR_REQUIRE_GAMEDATA` (Support). Passt.
- `docs/analyse/TON.md` §3/§4: Stück 0/1/2 mit Merker 0/2/3, `21A7:0054` → `218B:0153`, `10A7` Ton-Schalter, `0000:06FE`, Tabelle `CS:15B2`. Passt.
- `TypewriterClicks.cs`: Effekt `21A7:0054(1, 16h)`, #262 in `arbeitspakete.md`. Passt.
- `traders-referenz` `original/dos/`: `TRADERS.BAT`, `TRADINTR.CMF`, `TRADMENU.CMF`, `TRAD_END.CMF` vorhanden.
- `PORTIERUNGSPLAN.md` M14 „Hörvergleich mit Video“ und E8 „F1 = B“ (22.11.2026): passt.
- Hörstellen H1–H7: Stellen im Code gefunden. Erreichbarkeit im laufenden Spiel **nicht ausgeführt**.
- Nicht ausgeführt: Build, Tests, `tools/pruefen.sh` (entfällt laut Auftrag). Akzeptierte `[APPROX]`: keine.
