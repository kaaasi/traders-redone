# Review R3-Freigabe-Referenz – PR #64 `Prozess: Freigabe Referenz-Repo für Leitstellen`

Reviewer: Claude (Opus 5.5), frische Sitzung als geplanter Lauf (11.11.2026), ohne Mitarbeit an PR #64 (Autor-Sitzung
`session_01GhACvUEhzV58N8eBsGTSUm`, diese Sitzung eine andere).
Geprüft: Base `7a645ff` · Head `1b20f01a1caa8ff8d2792d12fd6e528324ff7fd0`. Checkout auf genau diesem SHA, `git status --porcelain`
leer. Originaldaten: `fetch-reference.sh original` (`7fcfb24`).

## 1. Merge-Empfehlung

**Ja für genau diesen Head.** Reine Doku (3 × `*.md`, keine Datei unter `docs/routines/**`, kein `STATUS.md`), CI entfällt
nach §6 Nr. 3. Die Vermerke zu Go, Zugriff und Labels widersprechen weder `zusammenarbeit.md` §5 Punkt 8 noch `anhang.md` F;
`CLAUDE.md` und `leitstelle-start.md` sind untereinander stimmig. Gegen `main` ohne Konflikt (Base = aktueller `main`,
`mergeable_state: clean`). Nur zwei P3 (Präzisierung, PR-Text), kein Merge-Hindernis.

## 2. Befunde

- **R3F-1 (P3) Label-Erlaubnis steht nicht in der Regelquelle, auf die sie verweist.**
  [`leitstelle-start.md` Z. 35–36](https://github.com/kaaasi/traders-redone/blob/1b20f01a1caa8ff8d2792d12fd6e528324ff7fd0/docs/vorlagen/leitstelle-start.md#L35-L36) und
  [`arbeitspakete.md` Z. 121–122](https://github.com/kaaasi/traders-redone/blob/1b20f01a1caa8ff8d2792d12fd6e528324ff7fd0/docs/arbeitspakete.md#L121-L122) erlauben der Leitstelle, Labels „immer selbst“ zu setzen
  und zu entfernen; [`zusammenarbeit.md` §5 Punkt 8](https://github.com/kaaasi/traders-redone/blob/1b20f01a1caa8ff8d2792d12fd6e528324ff7fd0/docs/prozess/zusammenarbeit.md#L78-L90) (verbindliche Tabelle, auf die
  Schritt 10 verweist) kennt das nur für `review-ja`/`review-nein` bei Reviews ohne Netz und für das Entfernen abgearbeiteter
  Labels. Kein Widerspruch (Erlaubnis, keine Pflichtverschiebung; §6 Nr. 5 verlangt ohnehin die Review-Datei), aber der Umfang
  ist offen – etwa ob die Leitstelle `review-ja` ohne Review-Datei setzen dürfte.
  Probe: `grep -n "Labels setzen\|darf immer" docs/prozess/zusammenarbeit.md docs/prozess/anhang.md` → keine Treffer.
  Vorschlag: in §5 Punkt 8 unter der Tabelle ein Satz „Die Leitstelle darf jedes Label setzen und entfernen (Nutzer 11.11.2026);
  `review-ja` nur nach ausgewerteter Review-Datei.“
- **R3F-2 (P3) PR-Beschreibung veraltet.** Sie nennt Base `aba13bf` (tatsächlich `7a645ff` nach Merge von `main` in
  `cbe1ed8`) und nur die Änderungen an `leitstelle-start.md` Schritt 0 und `CLAUDE.md`; Go/Zugriff in `arbeitspakete.md`
  und die Label-Erlaubnis in Schritt 10 (Commit `1b20f01`) fehlen. Probe: `git diff --stat 7a645ff 1b20f01` → 3 Dateien,
  darunter `docs/arbeitspakete.md`. Vorschlag: Beschreibung beim Merge nachziehen.

Hinweis ohne Befund: „erneut versuchen“ in [Z. 15](https://github.com/kaaasi/traders-redone/blob/1b20f01a1caa8ff8d2792d12fd6e528324ff7fd0/docs/vorlagen/leitstelle-start.md#L13-L15) nennt keine Obergrenze; mit
`anhang.md` F („sonst keinen Lauf anlegen und dem Nutzer melden“) bleibt der Ablauf trotzdem eindeutig (einmal wiederholen
genügt).

## 3. Ausgeführte Prüfungen

| Prüfung | Befehl | Ergebnis |
|---|---|---|
| Zugriff | `git ls-remote …/traders-redone HEAD`, `…/traders-referenz HEAD` | `7a645ff`, `7fcfb24` |
| Stand | `git rev-parse HEAD`, `git status --porcelain` | `1b20f01…`, leer |
| Nur Doku | `git diff --stat 7a645ff 1b20f01`; Pfadfilter `.github/workflows/build.yml` Z. 13–26 | `CLAUDE.md`, `docs/arbeitspakete.md`, `docs/vorlagen/leitstelle-start.md`; CI startet nicht |
| Abgleich Zugriff | `CLAUDE.md` Z. 17, 36–38 ↔ `leitstelle-start.md` Schritt 0 ↔ `anhang.md` F | stimmig (beide Repos `push`, Läufe nur nach erfolgreichem `ls-remote`) |
| Abgleich Go | `arbeitspakete.md` Z. 8, 120 ↔ `leitstelle-start.md` Schritt 2/3 | stimmig; Plan-Befunde R3P-1…5 in `504e27c` umgesetzt |
| Abgleich Labels | `leitstelle-start.md` Schritt 10 ↔ `zusammenarbeit.md` §5 Punkt 8 | kein Widerspruch, siehe R3F-1 |
| Mergebarkeit | `pull_request_read get #64` | `clean`, kein Draft |
| `tools/pruefen.sh` (alle drei Läufe) | `source ~/claude-tools-traders/env.sh; tools/pruefen.sh` | Ergebnis: grün |

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| Mit Originaldaten (TR_REQUIRE_GAMEDATA=1) | 451 | 0 | 0 |
| Ohne Originaldaten (TR_SKIP_GAMEDATA=1) | 363 | 0 | 70 |
| Pflichtlauf ohne Daten (muss scheitern) | 363 | 88 | 0 |

dotnet 10.0.402. Markerbericht: 33 `[ORIGINAL]`, 4 `[APPROX]` (unverändert, kein Code im Diff); keine neuen `[APPROX]` zu akzeptieren.

**Annahmen (nicht prüfbar):** Go und Zugriffsfreigabe des Nutzers vom 11.11.2026 sind nur durch die Texte der Leitstelle belegt;
das Review prüft Konsistenz, nicht die Erteilung selbst.
