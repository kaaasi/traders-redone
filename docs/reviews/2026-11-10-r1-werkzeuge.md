# Review R1① – PR #4 `prozess/werkzeuge`

Reviewer: Claude (Opus 5.5), frische Sitzung (geplanter Lauf 10.11.2026), ohne Mitarbeit an PR #4–#6.
Geprüft: Base `bbeacf4` · Head `be19f27d5b9a6be655cfaeaa13fc24f4e793a478` (= `gh api …/pulls/4 --jq .head.sha`, unverändert); `git rev-parse HEAD` = Head,
`git status --porcelain` leer.

## 1. Merge-Empfehlung

**Ja** für genau diesen Head. Einrichtung nach PR in frischer Sitzung gelungen, `tools/pruefen.sh` grün (396/0/0) und wird
bei allen vier Gegenproben rot; CI `linux` grün auf dem Head; SHA-512-Prüfung im Release-Lauf belegt. Die Prüfschritte des
entfernten `ci.yml` (Hashes, `--locked-mode`, Architektur) stecken jetzt in `pruefen.sh`, Build zusätzlich `-warnaserror`.
Nur P3-Befunde.

## 2. Befunde

- **R4-1 (P3) Release legt Git-Tag an – widerspricht „Keine Tags“.**
  [werkzeuge.yml Z. 65–70](https://github.com/kaaasi/traders-redone/blob/be19f27d5b9a6be655cfaeaa13fc24f4e793a478/.github/workflows/werkzeuge.yml#L65-L70) erzeugt mit `gh release create` den Tag
  `claude-werkzeuge`; [build.yml Z. 5](https://github.com/kaaasi/traders-redone/blob/be19f27d5b9a6be655cfaeaa13fc24f4e793a478/.github/workflows/build.yml#L5) und `AGENTS.md` §6 (PR #5) sagen „Keine Tags“.
  Beleg: `git ls-remote --tags origin` → `1e9469bbe82d… refs/tags/claude-werkzeuge` (Bootstrap-Commit, nur auf
  `prozess/werkzeuge`; der Tag hält ihn auch nach Löschen des Branches). Vorschlag: Ausnahme in AGENTS §6 nennen
  („außer dem technischen Release-Tag `claude-werkzeuge`“).
- **R4-2 (P3) CI baut mit anderem SDK als die Claude-Sitzung.** `setup-dotnet` mit `global-json-file` und `latestPatch`
  ([build.yml Z. 37](https://github.com/kaaasi/traders-redone/blob/be19f27d5b9a6be655cfaeaa13fc24f4e793a478/.github/workflows/build.yml#L37)) zieht 10.0.404, das Release liefert fest 10.0.402. Beleg: Log Job
  130154766930 „`tools/pruefen.sh` (dotnet 10.0.404) | 396 | 0 | 0“; lokal „dotnet 10.0.402“. Die PR-Aussage „fest 10.0.402“
  gilt nur für die Sitzung. Vorschlag: `dotnet-version: 10.0.402` in build.yml oder Abweichung dokumentieren.
- **R4-3 (P3) `setup-workspace.sh` überschreibt die globale NuGet-Konfiguration.**
  [Z. 9/47](https://github.com/kaaasi/traders-redone/blob/be19f27d5b9a6be655cfaeaa13fc24f4e793a478/tools/claude/setup-workspace.sh#L47) – Kommentar „kollidiert nicht mit anderen Projekten“, aber
  `~/.nuget/NuGet/NuGet.Config` enthält danach nur `traders-offline` (Beleg: `cat ~/.nuget/NuGet/NuGet.Config` nach dem
  Lauf). Das Eishockey-Skript tut dasselbe; in einer Sitzung mit beiden Repos verliert das zuerst eingerichtete einen schlichten
  `dotnet restore`. `pruefen.sh` selbst ist sicher (`--configfile`). Vorschlag: Kommentar korrigieren oder beide Quellen eintragen.

Ohne Befund geprüft: `werkzeuge.yml` Rechte (`contents: write` nur dort, `persist-credentials: false`), Trigger nur `main` +
Hand (Bootstrap-Trigger in `be19f27` entfernt); `build.yml` `contents: read`, Drafts übersprungen bis `ready_for_review`,
Doku-Ausschluss, Windows/Downloads nur `workflow_dispatch`, `cancel-in-progress` nur bei PRs. Verloren aus `ci.yml` nur
Bequemlichkeit (TRX-Artefakte, `bash -n` der entfernten Skripte, Python-Tests der entfernten Werkzeuge) – akzeptabel.
Wechselwirkung des Doku-Ausschlusses mit `tools/abdeckung.py` (PR #6): dort R6-2.

## 3. Ausgeführte Prüfungen

| Schritt (frische Sitzung, Worktree auf Head) | Ergebnis |
|---|---|
| `tools/claude/setup-workspace.sh` | SDK + Pakete aus Release geladen, 29 s |
| `source ~/claude-tools-traders/env.sh` | `dotnet --version` 10.0.402 |
| `tools/pruefen.sh` | grün, 33 s |

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| `tools/pruefen.sh` (dotnet 10.0.402) auf `be19f27` | 396 | 0 | 0 |
| CI `build.yml`/linux, Run 42909114591 (dotnet 10.0.404) | 396 | 0 | 0 |

Gegenproben in temporärer Kopie (danach verworfen), jeweils `tools/pruefen.sh`:

| Mutation | Ausgabe | Exit |
|---|---|---:|
| `Int16MathTests.cs` Z. 11 Erwartung +1 | `FEHLER: Tests`, 394/2/0 | 1 |
| `int unused = 1;` in `Traders.Core` | `error CS0219` → `FEHLER: Build` | 1 |
| ein Byte an `original/dos/T.EXE` angehängt | `FEHLER: Hash DOS` (+ Tests rot) | 1 |
| zweites Testprojekt `tests/Probe.Tests/` außerhalb der Solution | `FEHLER: Ergebnisse für weniger als 2 Testprojekte` | 1 |

Weitere Belege: Release-Lauf 42908671355 „dotnet-sdk-linux-x64.tar.gz: OK“ (`sha512sum -c`), Release-Assets SDK 240 MB,
Pakete 68 MB; `original/SHA256SUMS.*` decken alle 29 bzw. 3 Dateien ab.

Akzeptierte `[APPROX]`: keine neuen. Annahmen: SHA-512 aus Microsoft-Metadaten nur über das Lauf-Log belegt (Metadaten aus der
Sitzung nicht erreichbar); Windows-Job nicht ausgeführt (nur per Hand, D4).
