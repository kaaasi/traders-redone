# Review PR #87 – Abschluss Runde 3 (Archiv, Vorschau R4, STATUS)

Reviewer: Claude (geplanter Lauf, frische Sitzung ohne Mitarbeit an PR #87). Base `6564b7d12deb4e65294279d97d7a4ced3e518b8a`,
Head `8f8e9491e094bfba419e17b86335734fb5873388`. Checkout verifiziert: `git rev-parse HEAD` = Head, `git status --porcelain`
leer. Originaldaten: `tools/claude/fetch-reference.sh original` (Referenz `7fcfb24`, Hashes grün in `tools/pruefen.sh`).

## 1. Merge-Empfehlung

**Ja für Head `8f8e949`.** Reiner Doku-PR (`CHANGELOG.md`, `STATUS.md`, `docs/arbeitspakete.md`, `docs/archiv/runde-3.md`); der
erzeugte Block in `STATUS.md` ist unverändert, `abdeckung.py --check` grün (lokal und CI `linux` auf dem Head: success).
Das Archiv gibt den Plan der Runde wörtlich wieder (einzige Auslassung: die überholte „Stand“-Zeile), der Abschluss deckt sich
mit PRs #63–#86, den Reviews und Gesamtreview Teil 1/2. M3 ✅ entspricht dem Abnahmekriterium im `PORTIERUNGSPLAN.md`
(„RNG-Folgen vs. RefEmu“) und dem Rundenziel; B1/B2 aus `2026-11-12-r3-status.md` sind umgesetzt. Die Vorschau R4 folgt dem
Fahrplan und nennt #81, #86. Zwei P3, keine blockierend. `main` = Base, `git merge-tree` ohne Konflikt.

## 2. Befunde

**B1 (P3) – `STATUS.md` nennt #60 noch als offen.**
[`STATUS.md:90`](https://github.com/kaaasi/traders-redone/blob/8f8e9491e094bfba419e17b86335734fb5873388/STATUS.md#L90):
„✅ VGA-Teil (… #59; offen #60)“. Beleg: `gh api repos/kaaasi/traders-redone/issues/60 --jq .state,.state_reason` →
`closed completed`; `CHANGELOG.md:11` im selben Head: „#60 geklärt (nicht erreichbar)“. Die Zeile stammt aus der Zeit vor
R3 (nicht im Diff), wird mit dem Abschluss aber als aktueller Stand veröffentlicht. Vorschlag: „; #60 geklärt (R3)“ bzw.
„offen #60“ streichen – im nächsten Leitstellen-PR.

**B2 (P3, Hinweis) – Abschluss nennt nicht alle Reviews der Runde.**
[`docs/archiv/runde-3.md:131–137`](https://github.com/kaaasi/traders-redone/blob/8f8e9491e094bfba419e17b86335734fb5873388/docs/archiv/runde-3.md#L131)
führt Erstreviews der Spuren und das Gesamtreview, aber nicht das Plan-Review `2026-11-11-r3-plan.md` (#65, „Nein“ wegen
R3P-1/R3P-2, P2 Plantexte, vor dem Go behoben), `2026-11-11-r3-freigabe-referenz.md` (#70) und `2026-11-12-r3-status.md` (#84,
B1/B2). `archiv/runde-2.md:125` listet alle Reviews der Runde. Beleg: `ls docs/reviews | grep r3-` (9 Dateien) gegen die
Abschlusszeilen. Inhaltlich nichts falsch; Vorschlag: Zeile „Reviews der Runde: `reviews/2026-11-1{1,2}-r3-*.md`“ ergänzen.

Keine P0–P2.

**Geprüft ohne Befund:**

- **Archiv = Plan:** Plan an Base (`docs/arbeitspakete.md` ab „Aktuelle Runde: 3“ bis vor „Fahrplan“) gegen
  `docs/archiv/runde-3.md:2–128` per `diff`: einzige Abweichung die entfernte Zeile „Stand 12.11.2026 …“ (durch „Abschluss“
  ersetzt; hinterlässt eine doppelte Leerzeile in Z. 2–3, kosmetisch). Spur-„Status“-Zeilen bleiben im Stand des Plans, wie
  in `runde-2.md`.
- **Abschluss gegen GitHub:** #63, #64, #66–#69, #82 gemergt mit den genannten Branches; #79, #80, #77 geschlossen; offen
  #39, #49, #74, #75, #76, #81, #86 – genau die Liste in Z. 137, `arbeitspakete.md:17` und `CHANGELOG.md`. Takt-Review
  B1 = P2, behoben über #79 (Issue geschlossen). Teil 1 `r3-gesamt.md` „Nein … überwiegend Codelesen“, Teil 2 „Ja“, keine
  neuen Befunde, Testlücke = #86 – wie in Archiv und CHANGELOG. „635 Tests grün“ = eigener Lauf.
- **Segmentstatus (B1 r3-status):** `1777` und `27C6` jetzt 🟡 mit unverändertem Text, konsistent mit den übrigen teilportierten
  Segmenten (`0609`, `0825`, `08CF`, `0B9F`, `21FD`, `22A3`). **B2 r3-status:** P3-Liste „#74–#76“, Codex statt ChatGPT.
- **M3 ✅:** Plan-Ziel „M3 abgeschlossen (Phasenmaschine, Wochenregeln mit Mehrrundenprobe)“ und Gesamtreview „Ja“ liegen
  vor; die fehlende Mensch-Probe ist in #81 ausdrücklich nach R4 verschoben und in der Vorschau als Pflicht genannt.
- **Vorschau R4:** Themen und Segmente = Fahrplanzeile R4 (`192D`, `0E31`, `0DFD`, `21FD:0687`, Teile `0609`); `0E9E` und
  `Traders.App`-Anbindung entsprechen „Port von `0E9E` in den Kern offen für R4“ bzw. „Anbindung folgt in R4“ (Plan Spur 3);
  #86 (Bild- und Zufallsfolge eines Handlers), #81 (Mensch, Rohoffset `0x10D2`) inhaltlich korrekt wiedergegeben.
- Keine Originaldaten oder Abgeleitetes im Diff; nur Leitstellendateien geändert (`arbeitspakete.md` „Zuständigkeit“).

## 3. Ausgeführte Prüfungen

| Prüfung | Ergebnis |
|---|---|
| `git ls-remote` beider Repos | erreichbar (`6564b7d`, `7fcfb24`) |
| `git diff --stat 6564b7d 8f8e949` | 4 Dateien, nur Doku (+153/−130) |
| `tools/pruefen.sh --schnell` auf Head | grün: Hashes, Selbsttest, Architektur, Abdeckung, Restore, Build, Tests |
| `python3 -I tools/abdeckung.py --check` | „Abdeckung aktuell.“ rc 0 |
| Gegenprobe: Zeile `23CD` im erzeugten Block `… 29 \| 28`, `--check` | rc 1 „STATUS.md ist veraltet“ (zurückgesetzt, Baum sauber) |
| CI Check-Runs Head `8f8e949` | `linux` success (`windows`, `downloads` skipped) |
| `diff` Plan an Base gegen Archiv Z. 2–128 | nur „Stand“-Zeile entfernt |
| `git merge-tree --write-tree origin/main pr87` | ohne Konflikt (`main` = Base) |
| GitHub PRs #63–#87, Issues #39, #49, #60, #74–#77, #79–#81, #86 | wie oben |

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| Mit Originaldaten (TR_REQUIRE_GAMEDATA=1) | 635 | 0 | 0 |

Markerbericht: 127 `[ORIGINAL]`, 10 `[APPROX]` (unverändert, PR ändert keinen Code). Nur `--schnell` (Doku-PR, Vorgabe).

**Erwartet offen bestätigt:** #39 (R3-Anteil), #49, #74, #75, #76, #81, #86 offen.

**Akzeptierte `[APPROX]`:** keine neuen. **Annahmen:** Inhalte der Spur-PRs nicht erneut dynamisch geprüft; Abgleich gegen
deren Reviews, GitHub-Status und den erzeugten Block.
