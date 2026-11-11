# Review R1③ – PR #6 `prozess/abdeckung`

Reviewer: Claude (Opus 5.5), frische Sitzung (geplanter Lauf 10.11.2026), ohne Mitarbeit an PR #4–#6.
Geprüft: Base `be19f27` (Head PR #4) · Head `bdd691744fa397132a9fa493924295a8c59a4a60` (= aktueller Head laut API); `git status --porcelain` leer.

## 1. Merge-Empfehlung

**Nein** für diesen Head. Das Werkzeug arbeitet richtig für den heutigen Bestand (Segmente identisch mit
`docs/analyse/segments.md`, `--write` idempotent, die drei geforderten Gegenproben werden rot, `pruefen.sh` grün). Zwei
P2 machen das Prüftor aber unzuverlässig: ein portierter Befund gilt als differenzgetestet, sobald seine Adresse in irgendeinem
Kommentar einer `*Differential*`-Datei steht (R6-1), und Doku-PRs können `--check` rot machen, ohne dass CI läuft (R6-2).
Beides vor dem Merge beheben oder vom Nutzer zurückstellen lassen.

## 2. Befunde

- **R6-1 (P2) Differenztest-Zuordnung über jede Adresse im Dateitext.**
  [abdeckung.py Z. 24/76](https://github.com/kaaasi/traders-redone/blob/bdd691744fa397132a9fa493924295a8c59a4a60/tools/abdeckung.py#L76) zählt jede `SSSS:OOOO` in `tests/**/*Differential*.cs`.
  `SaveGameDifferentialTests.cs` nennt in Kommentaren `27C6:03C3` (int-8-Handler) und `192D:0058`, prüft sie aber nicht.
  Beleg (temporäre Kopie): `docs/routines/27C6_03C3_probe.md` mit Titel `# 27C6:03C3 – …` und
  `// [ORIGINAL] 27C6:03C3` in `src/Traders.Core/OriginalRandom.cs`, `abdeckung.py --write`, dann `--check` →
  `Abdeckung aktuell.`, Exit 0, Tabelle: `| 27C6:03C3 | … | SaveGameDifferentialTests.cs | ✅ |`. Regel AGENTS §2.7 damit
  scheinbar erfüllt. Vorschlag: nur ausdrückliche Kennzeichnung zählen (z. B. `[DIFFERENZ] SSSS:OOOO` oder Konstanten
  `Seg/Off` im Testkopf), Kommentaradressen ignorieren.
- **R6-2 (P2) Doku-Änderungen beeinflussen `--check`, CI läuft dafür nicht.** Eingaben von `abdeckung.py` sind
  `docs/routines/*.md` und `STATUS.md`; [build.yml Z. 9/12](https://github.com/kaaasi/traders-redone/blob/bdd691744fa397132a9fa493924295a8c59a4a60/.github/workflows/build.yml#L9-L12) ignoriert
  `docs/**`/`**/*.md`, `zusammenarbeit.md` §6 Nr. 3 (PR #5) erlaubt „entfällt, nur Doku“. Beleg (temporäre Kopie):
  `printf '# 0609:0010 – Probe\n' > docs/routines/0609_0010_probe.md`, `python3 tools/abdeckung.py --check` →
  `FEHLER: STATUS.md ist veraltet …`, Exit 1. Ein reiner Befund-PR ohne `--write` geht so ungeprüft durch und macht `main`
  für alle folgenden PRs rot. Vorschlag: `docs/routines/**` und `STATUS.md` per `!`-Muster aus dem Ausschluss nehmen
  (`paths-ignore` → `paths` mit Negation) und §6 Nr. 3 entsprechend einschränken.
- **R6-3 (P3) Marker in anderer Schreibweise werden still ignoriert; Abnahme Int16Math offen.**
  [Z. 25](https://github.com/kaaasi/traders-redone/blob/bdd691744fa397132a9fa493924295a8c59a4a60/tools/abdeckung.py#L25) erkennt nur `[ORIGINAL] HHHH:HHHH` in Großbuchstaben. Beleg: `// [ORIGINAL] 0609:00aa`
  und `// [ORIGINAL] ohne Adresse` in `OriginalRandom.cs` → `--check` Exit 0, keiner erscheint in der Tabelle; AGENTS §2.8
  verlangt aber die Fundstelle. Vorschlag: jedes `[ORIGINAL]` ohne gültige Adresse als Fehler melden. Die Abnahme in
  `docs/arbeitspakete.md` („`Int16Math` mit Markern versehen“) ist nicht umgesetzt (`grep -c ORIGINAL
  src/Traders.Core/Int16Math.cs` → 0); entweder Marker (`[APPROX]`/Verweis auf `Int16Math_IDIV.md`) oder Abnahme anpassen.

Ohne Befund: `Befund ohne Port` (🔍 `21FD:0687`) ist bewusst kein Fehler – die Abnahme „Befund ohne Test“ ist als „Port ohne
Differenztest“ umgesetzt, wie im Auftrag. Merge mit PR #5 konfliktfrei (`STATUS.md` auto-merge).

## 3. Ausgeführte Prüfungen

| Prüfung | Ergebnis |
|---|---|
| `python3 tools/abdeckung.py --check` | `Abdeckung aktuell.`, Exit 0 |
| `--write` zweimal, `git status --porcelain` | leer (kein Diff) |
| Segmenttabelle gegen `docs/analyse/segments.md` (Python-Vergleich Segment, Größe, Far-Einsprünge) | 45 = 45 Segmente, alle gleich, 891 Far-Einsprünge, Summe 0x28DC0 |
| Gegenprobe Marker `[ORIGINAL] 1234:5678` ohne Befund | `FEHLER: Marker … ohne Befund`, Exit 1 |
| Gegenprobe `RandomDifferentialTests.cs` entfernt | `FEHLER: 21FD:01BA ist portiert … ohne Differenztest` + `STATUS.md ist veraltet`, Exit 1 |
| Gegenprobe Statuszelle `21FD:0687` von Hand auf ✅ | `FEHLER: STATUS.md ist veraltet`, Exit 1 |
| Gegenproben R6-1/R6-2/R6-3 | siehe Befunde |

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| `tools/pruefen.sh` (dotnet 10.0.402) auf `bdd6917`, Abdeckung grün | 396 | 0 | 0 |
| `tools/pruefen.sh` auf `main`+#4+#5+#6 (lokaler Merge `7c76599`) | 396 | 0 | 0 |
| CI `build.yml`/linux auf Head, Run 42910213248 | grün | | |

Markerbericht: Traders.Core 3 `[ORIGINAL]`, 0 `[APPROX]`. Akzeptierte `[APPROX]`: keine. Annahme: die Segmentermittlung
gleicht `tools/segmap.py` nur im Ergebnis (Code nachgelesen, nicht gemeinsam genutzt).
