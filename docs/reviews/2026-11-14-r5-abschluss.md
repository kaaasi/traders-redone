# Nachprüfung Review R5 Status, Abschluss Runde 5 – PR #136

- **Geprüft:** Head `7969ff17b399bd79d380bf58ab1b59fa3a40b034`, Base `48aad3ea37d6df783f52f988d3723dc94a33a4aa`. Nachprüfung
  von `docs/reviews/2026-11-14-r5-status.md` (bisher geprüfter Head `86e6444`). Checkout des SHA, `git rev-parse HEAD` = Head,
  `git status --porcelain` leer (vor und nach dem Prüflauf). Originaldaten aus `kaaasi/traders-referenz` (`7fcfb24`). Reviewer:
  Claude Opus (geplanter Lauf, frische Sitzung, ohne Mitarbeit an diesem PR).

## 1. Merge-Empfehlung

**Ja** für genau `7969ff1`. `git range-diff 9c7c564..86e6444 48aad3e..7969ff1` zeigt: `86e6444` = `8a84dc7`, der Rebase war also
inhaltsgleich. Neu ist nur `7969ff1`. R5St-1 und R5St-3 sind behoben. R5St-2 (#140) und R5G-1 (#139) sind wie erwartet als offene
Issues geführt. Das Archiv `runde-5.md` ist der Plan plus der Abschnitt „Abschluss“. Jede Aussage im Abschluss stimmt mit GitHub
und `docs/reviews/` überein. M5 ✅ ist durch das Gesamtreview „Ja“ gedeckt. Offen bleibt nur ein P3 (R5Ab-1): In der Vorschau R6
steht die falsche Aufrufnummer.

## 2. Befunde

**R5St-1 (P2) – behoben.** Gegenbeispiel erneut ausgeführt: die Handzeilen in §3 gegen den erzeugten Block.

| Segment | Zeile (Head) | Block | Ergebnis |
|---|---|---|---|
| `0609` | [Z. 53](https://github.com/kaaasi/traders-redone/blob/7969ff17b399bd79d380bf58ab1b59fa3a40b034/STATUS.md#L53) ✅, `0004`/`03E0`/`0C73` genannt | Z. 462 56/56/56/56 | stimmt |
| `0825` | [Z. 54](https://github.com/kaaasi/traders-redone/blob/7969ff17b399bd79d380bf58ab1b59fa3a40b034/STATUS.md#L54) ✅, `000A`/`02D7` (R5 Spur 3) | Z. 463 16/16/16/16 | stimmt |
| `111D` | Z. 66 🟡, Wächter `000D–001B` | Z. 475 7/1/1/1 | stimmt |
| `11A3` | [Z. 67](https://github.com/kaaasi/traders-redone/blob/7969ff17b399bd79d380bf58ab1b59fa3a40b034/STATUS.md#L67) ✅ | Z. 476 15/15/15/15 | stimmt |
| `135D` | [Z. 69](https://github.com/kaaasi/traders-redone/blob/7969ff17b399bd79d380bf58ab1b59fa3a40b034/STATUS.md#L69) ✅ | Z. 478 14/14/14/14 | stimmt |
| `1F63` | Z. 81 🟡, `113E`/`104B` | Z. 490 27/2/2/2 | stimmt |
| `21FD` | Z. 86 🟡, Warteschleife `0004` ergänzt | Z. 495 28/21/21/21 | stimmt |

Statt 🟢 steht jetzt direkt ✅. Das ist richtig, denn das Gesamtreview ist inzwischen „Ja“. Zum Hinweis „`13C5` Ernte-Teil mit R6“
in der Zeile `0609`: `0609:13C5` selbst ist im Block ✅ (Z. 155). Der Hinweis betrifft nur die Aufrufer außerhalb des Anbaus. Das ist
kein Widerspruch.

**R5St-3 (P3) – behoben.** Gegenbeispiel: Der Statusabschnitt der Spur 0 endet in
[`docs/archiv/runde-5.md:64`](https://github.com/kaaasi/traders-redone/blob/7969ff17b399bd79d380bf58ab1b59fa3a40b034/docs/archiv/runde-5.md#L64)
mit „; offen P3 #113, #135“. Beide Issues sind laut GitHub offen.

**R5St-2 (P3) – erwartet offen, bestätigt.** Das Issue #140 ist offen („Sammelmarker ohne Listenform zählen nicht (192D:1177)“, P3).
Die Vorschau R6 führt es als Pflicht.
**R5G-1 (P3) – erwartet offen, bestätigt.** Das Issue #139 ist offen („Leseregel erkennt schreibende Kernaufrufe über
Methodenparameter nicht“, P3). Es ist im Archiv, im CHANGELOG und in der Vorschau genannt.

**R5Ab-1 (P3) – Vorschau R6 nennt für die Ernte die Aufrufnummer 13 statt 16.**
In [`docs/arbeitspakete.md:15`](https://github.com/kaaasi/traders-redone/blob/7969ff17b399bd79d380bf58ab1b59fa3a40b034/docs/arbeitspakete.md#L15)
steht „Ernte zeigen `22A3:02D3` mit `0609:13C5` (Aufruf 13)“. Nach
[`docs/routines/1777_01F8_wochenschleife.md:32`](https://github.com/kaaasi/traders-redone/blob/7969ff17b399bd79d380bf58ab1b59fa3a40b034/docs/routines/1777_01F8_wochenschleife.md#L32)
ist `22A3:02D3` aber Aufruf 16. Aufruf 13 ist die schon portierte Produktion `22A3:0003` (Z. 29). Beleg:
`grep -n '| 13 \|| 16 ' docs/routines/1777_01F8_wochenschleife.md` → `| 13 | 0254 | 22A3:0003 | Produktion |`,
`| 16 | 0268 | 22A3:02D3 | Ernte zeigen und einsammeln |`. Die Folgerung im selben Satz bleibt richtig: Mit 4 und 9 hielte der Host
an `1B03` (Aufruf 11), denn 10 ist eine Marke. Die Planung R6 könnte die Nummer aber falsch übernehmen. Vorschlag: „(Aufruf 16)“.
Behebbar beim Planen von R6, das blockiert diesen PR nicht.

**Geprüft ohne Befund:**
- **Archiv gegen Plan:** Der Bereich „Aktuelle Runde … bis vor ‚Fahrplan‘“ aus `arbeitspakete.md` in `8a84dc7` wurde mit `diff`
  gegen `docs/archiv/runde-5.md` verglichen. Abweichungen gibt es nur bei der Überschrift (Z. 1), bei der Statuszeile Spur 0
  (Z. 64, „; offen P3 #113, #135“) und beim angehängten Abschnitt „Abschluss“ (Z. 158–172). Linkpfade mit `../` mussten nicht
  angepasst werden, weil der Plan keine Markdown-Links enthält. Der einzige Link im Archiv (`../reviews/2026-11-14-r5-gesamt.md`)
  löst auf. Gegenüber der Base `48aad3e` kommen nur die Statuszeilen aus `8a84dc7` hinzu, und die wurden schon im Erstreview
  geprüft.
- **Abschluss gegen GitHub:** Die Merge-Folge 2 → 1 → 3 → 0 (#123, #125, #124, #122) passt zur Erstparent-Folge auf `main`
  (`be80730`, `97ed663`, `96b01bf`) und zum Erstreview. `9c7c564` = Merge #134 ist der Stand am Ende der Spuren und zugleich der
  Head des Gesamtreviews. Die Urteile wurden in den Review-Dateien nachgelesen: `r5-plan` Nein, `zugriff-methode` Ja,
  `r5-planung` Nein, `r5-anbau-ki` Nein → `…-nachpruefung` Ja, `r5-bilanz` Ja, `r5-anbau` Ja, `fix-menue-neuzeichnen` Ja,
  `r5-status` Nein, `r5-gesamt` Ja. „Gesamtreview Codex“ ist durch den Trailer `Co-Authored-By: Codex` des Commits belegt. Die
  geschlossenen Issues #99, #101, #103, #105, #109, #116 und #126 sind laut GitHub geschlossen. Die offenen Issues entsprechen
  genau der Liste in GitHub: #49, #75, #76, #113, #121, #131, #135, #139, #140 (9 Stück). Die Entscheidung „bis zu zwei
  zusätzliche Fortsetzungen, verbraucht eine“ ist durch den Kommentar an #122 vom 13.11. 16:40Z belegt, die Fortsetzung 4 von 5
  am 13.11. 22:04Z.
- **M5 ✅:** Die Zeile entspricht der aus `8a84dc7`, nur steht dort jetzt „Gesamtreview R5 ‚Ja‘“. `r5-gesamt.md` empfiehlt „Ja“
  für `9c7c564` und hat nur das P3 R5G-1. Der Kopf von `STATUS.md` verweist auf das Archiv.
- **CHANGELOG:** „Abschluss `9c7c564`“ folgt der Form aus R4 (dort der Stand, den das Gesamtreview geprüft hat). Die neue Zeile zum
  Gesamtreview stimmt mit Archiv und Review überein.
- **Vorschau R6:** Die Themen entsprechen der Fahrplanzeile R6 (`22A3`, `111D`, `17CB`). Die Aufrufnummern 4 (`111D:0009`),
  9 (`17CB:0005`) und 11 (`1B03`) stimmen mit der Wochenschleife überein. Wetter `21FD:021F` und Produktion/Schwund `22A3:0003/026B`
  liegen vor (Block ✅ bzw. §3). Die Pflichten #135, #113, #139, #140 und die übrigen offenen Issues stimmen mit GitHub überein.
  Einzige Abweichung ist R5Ab-1.

## 3. Ausgeführte Prüfungen

```
git fetch origin pull/136/head 86e6444…; git checkout 7969ff1; git rev-parse HEAD; git status --porcelain   # Head, sauber
git range-diff 9c7c564..86e6444 48aad3e..7969ff1        # 1 = 1 (86e6444 = 8a84dc7), 2 neu (7969ff1)
git diff 8a84dc7 7969ff1                                # CHANGELOG.md, STATUS.md, arbeitspakete.md, archiv/runde-5.md
diff <(sed -n '/^## Aktuelle Runde/,/^## Fahrplan/p' <(git show 8a84dc7:docs/arbeitspakete.md)) docs/archiv/runde-5.md
grep -n '^| `\(0609\|0825\|111D\|11A3\|135D\|1F63\|21FD\)`' STATUS.md
tools/claude/fetch-reference.sh original && tools/claude/setup-workspace.sh
python3 tools/abdeckung.py --check                     # Abdeckung aktuell.
source ~/claude-tools-traders/env.sh && tools/pruefen.sh --schnell   # Ergebnis: grün
```

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| Mit Originaldaten (TR_REQUIRE_GAMEDATA=1) | 882 | 0 | 0 |

Markerbericht: 367 `[ORIGINAL]`, 34 `[APPROX]`, unverändert. Der PR ist reine Doku, er bringt keine neuen `[APPROX]`. Auf GitHub
gelesen: offene Issues und seit dem 12.11. geschlossene Issues (MCP) sowie die Kommentare an #122 (`gh api`, REST).

**Annahmen / nicht ausgeführt:** Die Läufe ohne Originaldaten wurden nicht ausgeführt (für eine Nachprüfung reiner Doku genügt
`--schnell`). Die Inhalte der Spur-Reviews und des Gesamtreviews wurden nicht erneut geprüft. Geprüft wurden nur ihre Urteile und
Kennungen gegen Archiv, `STATUS.md` und `CHANGELOG.md`.
