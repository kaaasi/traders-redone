# Review: PR #270 – Probe mit Mensch freigeben, Tonmerker `DS:10A5`/`10A9` im Host-Gleichschritt vergleichen (fix-partie-mensch)

Reviewer: Claude Opus (geplanter Lauf, eigene Sitzung ohne Implementierungskontext) · Datum: 24.11.2026
PR: #270 · Base `83a7529` · Head `13c06fd` (geprüft: `git rev-parse HEAD` = `13c06fd2…`, `git status --porcelain` leer)
Originaldaten: geprüft (`tools/claude/fetch-reference.sh original`, Hashes grün).

## 1. Merge-Empfehlung

**Ja** für genau Head `13c06fd`. `compare --stat`: nur `ProgramHostTests.cs` und `HostWholeGameFrameDifferentialTests.cs`
(8+/9−), kein Produktivcode, keine Doku. Beide Filter nehmen genau `SoundOwned` heraus. Beide Gegenbeispiele werden rot.
Die Probe mit Mensch läuft ohne Skip grün gegen den RefEmu. Alle drei Läufe von `tools/pruefen.sh` sind grün. CI `linux`: success.
Damit ist #268 (R9P-1) erledigt. Keine P0–P2.

## 2. Befunde

**P3 (Hinweis) – `NotSoundFlag` vergleicht exakt `(Offset, Length)`.**
[`ProgramHostTests.cs:436`](https://github.com/kaaasi/traders-redone/blob/13c06fd2315bca9f7f70b450c8e4e8acd2ee8b66/tests/Traders.Tests/App/ProgramHostTests.cs#L436):
Eine künftige Liste mit einem Bereich, der `10A5`/`10A9` überdeckt (z. B. `(0x10A4, 8)`), nähme die Merker unbemerkt wieder
heraus. Heute gibt es keinen solchen Eintrag: Mutation A/B unten werden rot. Kein Handlungsbedarf.

**Zu den besonderen Prüfpunkten:**
- Filter: `OutputOwned` ([`ProgramStartDifferentialTests.cs:35–36`](https://github.com/kaaasi/traders-redone/blob/13c06fd2315bca9f7f70b450c8e4e8acd2ee8b66/tests/Traders.Tests/Startup/ProgramStartDifferentialTests.cs#L35))
  und `EndingFrameHarness.Owned` (über `.. SoundOwned`, Z. 62) führen `(0x10A5, 2)`/`(0x10A9, 2)` genau so wie
  `SoundOwned`. Andere Einträge haben keine gleiche Lage. Also fallen nur diese zwei heraus. `10A3` bleibt über
  `HostOwnedByInitialisation` ausgenommen; das behauptet niemand anders.
- `docs/routines/1777_000C_main.md` Z. 52 („die Host-Gleichschritttests vergleichen `10A5`/`10A9` seitdem mit“) stimmt jetzt:
  `Ignored` nutzen genau `ProgramHostTests` (Z. 384) und `HostWholeGameFrameDifferentialTests` (Z. 352); beide werden bei
  Mutation A/B rot.
- Erwartungswerte: `Play` startet `OriginalProgram.Start` (RefEmu) und vergleicht DS, Bild, Anzeigestart und Tonbefehle je Retrace
  gegen das Original, nicht gegen den Port.

## 3. Ausgeführte Prüfungen

Gegenproben in eigener Worktree-Kopie, Filter
`FullyQualifiedName~HostWholeGameFrameDifferentialTests|FullyQualifiedName~Traders.Tests.App.ProgramHostTests`, `TR_REQUIRE_GAMEDATA=1`:

| Probe | Änderung | Ergebnis |
|---|---|---|
| A | `SoundControl.cs:93` `MusicPlaying` 1 → 7 | 5/13 rot: 3 Gesamtpartieproben `DS:10A5 original 01, host 07`; 2 `ProgramHostTests` `DS differs at 10A5-10A5` |
| B | `SoundControl.cs:87` `CurrentPiece` = `piece \| 0x4000` | 5/13 rot: `DS:10AA original 00, host 40` bzw. `DS differs at 10AA-10AA` |
| A0 | Mutation A, Tests von Base `83a7529` | 12 grün, 1 übersprungen: Base war blind, der PR schließt die Lücke |
| C | #265 zurück: `StationScene` `ReadTable16` → `ReadFar16` | nur Probe mit Mensch rot: `frame 21286 (… week 2, call 11, its frame 72): display start original 0000, host 4000` |

`tools/pruefen.sh` (alle drei Läufe, Head `13c06fd`), Ergebnis grün:

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| Mit Originaldaten (`TR_REQUIRE_GAMEDATA=1`) | 1395 | 0 | 0 |
| Ohne Originaldaten (`TR_SKIP_GAMEDATA=1`) | 589 | 0 | 468 |
| Pflichtlauf ohne Daten (muss scheitern) | 589 | 806 | 0 |

Probe mit Mensch im Volllauf: `Passed … Whole_game_with_a_human_… [58 s]`. SDK 10.0.402.

**Akzeptierte `[APPROX]`:** keine neuen. **Annahmen:** keine.
