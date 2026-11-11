# Review R1-Abschluss – PR #27 `prozess/abschluss-r1`

Reviewer: Claude (Opus 5.5), frische Sitzung (11.11.2026), ohne Mitarbeit an PR #27 oder #28.
Geprüft: Base `c4ea5d83e54990b16a87adcafce73e1212f32920` · Head `1b2721323e4b6f1ae64d53b1897810bfb7f33060`
(= aktueller PR-Head). Worktree auf genau diesem SHA, `git status --porcelain` leer.
Originaldaten: `tools/claude/fetch-reference.sh original` (traders-referenz `7fcfb24`), Hashes geprüft.

## 1. Merge-Empfehlung

**Ja für genau diesen Head.** Das Archiv ist vollständig, `abdeckung.py --check` und alle drei Läufe von `tools/pruefen.sh`
sind grün, die CI auf dem Head ist grün, #26 ist behoben. Es bleibt nur ein kosmetischer P3 (R1A-1), der nicht blockiert.

## 2. Befunde

Kein P0–P2.

- **R1A-1 (P3) Überschrift nennt noch Referenzbilder.** [tests/Traders.Tests/RefEmu/Data/README.md:1](https://github.com/kaaasi/traders-redone/blob/1b2721323e4b6f1ae64d53b1897810bfb7f33060/tests/Traders.Tests/RefEmu/Data/README.md#L1)
  „CPU-Testdaten und Referenzbilder“, obwohl dort nur noch CPU-Testdaten liegen. Beleg:
  `git ls-tree -r --name-only 1b27213 tests/Traders.Tests/RefEmu/Data` → `LICENSE.SingleStepTests`, `README.md`,
  `cputests-286.bin.gz`. Z. 14–15 erklärt den Umzug jetzt korrekt; nur die Überschrift ist übrig geblieben. Vorschlag:
  „CPU-Testdaten“ – kann mit dem nächsten Lauf mitgehen, kein eigener Lauf.

## Besonders geprüft

- **Archiv vollständig.** Abschnitt „Aktuelle Runde: Runde 1 …“ aus `docs/arbeitspakete.md@c4ea5d8` gegen
  `docs/archiv/runde-1.md@1b27213` gediffet: Unterschiede sind nur die Überschriftenebenen (`##`→`#`, `###`→`##`) und die
  letzte Zeile „Offen: Gesamtreview …“, ersetzt durch den Abschnitt „Abschluss (11.11.2026)“ (Gesamtreview „Ja“, P3 #26,
  `c4ea5d8` = Merge PR #17 bestätigt per `git log`, Liste der vier Spur-Reviews – alle vier Dateien existieren in
  `docs/reviews/`, Hinweis auf die Support-Anfrage). Kein Inhalt verloren. Relativer Link `../reviews/2026-11-11-r1-gesamt.md`
  passt.
- **`docs/arbeitspakete.md`:** keine aktuelle Runde, Link `archiv/runde-1.md` auflösbar, Vorschau Runde 2 unverändert.
- **`STATUS.md`:** nur die Stand-Zeile; der erzeugte Block zwischen `abdeckung:anfang/ende` ist unberührt.
- **#26 behoben:** Z. 13 („Die PNGs …“) ersetzt durch einen Verweis auf `refemu/` im Referenz-Repo
  (`fetch-reference.sh refemu`). Kein Originalinhalt, nur Pfadangabe.
- **AGENTS §2.1:** Diff enthält nur `*.md` ohne Originaltexte, -bilder oder Disassemblierung.

## 3. Ausgeführte Prüfungen

```
git -C wt27 rev-parse HEAD                      # 1b2721323e4b6f1ae64d53b1897810bfb7f33060, status leer
git diff --stat c4ea5d8 1b27213                 # 4 Dateien, nur *.md
diff <(Abschnitt aus arbeitspakete.md@c4ea5d8) docs/archiv/runde-1.md@1b27213
python3 -I tools/abdeckung.py --check           # „Abdeckung aktuell.“, rc=0
source ~/claude-tools-traders/env.sh && tools/pruefen.sh   # alle drei Läufe, rc=0, „Ergebnis: grün“
gh api …/commits/1b27213…/check-runs            # linux: success; windows, downloads: skipped (nur per Hand)
```

Tabelle aus `tools/pruefen.sh` (Head `1b27213`, dotnet 10.0.402):

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| Mit Originaldaten (TR_REQUIRE_GAMEDATA=1) | 396 | 0 | 0 |
| Ohne Originaldaten (TR_SKIP_GAMEDATA=1) | 350 | 0 | 29 |
| Pflichtlauf ohne Daten (muss scheitern) | 350 | 46 | 0 |

Markerbericht: `[ORIGINAL]` 3 (Traders.Core), `[APPROX]` 0. Zahlen identisch mit dem Gesamtreview R1 (erwartbar, kein Code geändert).

**Akzeptierte `[APPROX]`:** keine.
**Annahmen:** keine; Mutationen entfallen (keine neuen Tests).
