# Arbeitsregeln für dieses Repo

- Sprache der Doku und Commits: Deutsch. Code-Bezeichner und Code-Kommentare: Englisch.
- Original ist die Wahrheit: `original/dos/T.EXE` (SHA-256 b73a2170…af0185). Jede portierte
  Routine bekommt einen Befund in `docs/routines/` (Vorlage `_vorlage.md`) und einen
  Differenztest gegen das Original.
- `Traders.Core` bleibt deterministisch und plattformfrei: kein Rendering, keine Uhr,
  keine Zufallsquelle außer dem portierten Original-RNG.
- Rendern/Animation darf keinen Zufall verbrauchen und nichts buchen.
- 16-Bit-Semantik über `Int16Math`, nicht über „saubere“ Neurechnung.
- **`STATUS.md` bei jedem Push aktuell halten**: Meilensteine, Ressourcen, Module und
  einzelne Routinen mit Status (✅ / 🟢 / 🟡 / 🔍 / ⬜).
- Zwischenstände pro Meilenstein in `CHANGELOG.md` mit Commit festhalten. Tags (`v0.x`)
  kann die Claude-Sitzung nicht pushen; der Nutzer setzt sie bei Bedarf.
