# Review PR #94 – Aufgaben (geplante Läufe) legt allein die Leitstelle an

Base `a25a5c8` · Head `5bc436b2e2a97b34933617ecc77d69e226c68835` · Reviewer: Claude (geplanter Lauf, eigene Sitzung, nicht am PR beteiligt) · 12.11.2026

## 1. Merge-Empfehlung

**Ja** für genau diesen Head. Die Nutzervorgabe (12.11.2026) ist in `zusammenarbeit.md` §4/§5 Punkt 8, `anhang.md` F und
`spur-start.md` (Kurzaufruf, Fixlauf, „Teile und Fortsetzung“) umgesetzt; in `AGENTS.md`, `CLAUDE.md`, `docs/prozess/**`,
`docs/vorlagen/**` bleibt **keine** Stelle, die Spuren, Fixläufen oder Reviews `create_trigger`, Selbstfortsetzung oder das
Anlegen einer Nachprüfung erlaubt. Label-Tabelle und Schritte 5/6 sind stimmig. Ein P2 (Prüfregel der Leitstelle für
Nachprüfungs-Prompts) und zwei P3 – keiner blockiert; Behebung per kleinem Folge-PR mit Delta-Prüfung der Leitstelle genügt.

## 2. Befunde

**R4PA-1 (P2) – Prüfregel für „Nachprüfung anlegen – Leitstelle“ verweist auf den falschen Kurzaufruf.**
[`anhang.md` Z. 73–74](https://github.com/kaaasi/traders-redone/blob/5bc436b2e2a97b34933617ecc77d69e226c68835/docs/prozess/anhang.md#L73-L74): Der PR erweitert den Satz um „bzw. ‚Nachprüfung anlegen –
Leitstelle‘“, lässt aber „vorher gegen den Kurzaufruf ‚Geplanter Lauf mit Teilen‘ prüfen; abweichen dürfen nur Spur, Modell,
Zähler und PR-Nummer“ stehen. Ein Nachprüfungs-Prompt folgt dem Kurzaufruf aus `review-anfrage.md` (PR, Base, Head, Ablage,
Delta, Befunde) und weicht zwangsläufig in genau diesen Feldern ab – wörtlich angewandt müsste die Leitstelle jede korrekte
Nachprüfung ablehnen bzw. hat keine Prüfvorgabe dafür.
Beleg: `sed -n 73,74p docs/prozess/anhang.md` → Text wie zitiert; `sed -n 3,18p docs/vorlagen/review-anfrage.md` → Felder
`PR`, `Base`, `Head`, `Ablage`, „Nachprüfung des Reviews …, nur Delta `<alt>..<neu>`“, keine davon in der Ausnahmeliste.
Vorschlag: „… gegen den Kurzaufruf ‚Geplanter Lauf mit Teilen‘ bzw. (Nachprüfung) `review-anfrage.md` samt Zusatz
‚Nachprüfung‘ prüfen; abweichen dürfen nur die Platzhalterfelder.“

**R4PA-2 (P3) – `review-anfrage.md` nennt das Verbot nicht.**
[`review-anfrage.md` Z. 5–18](https://github.com/kaaasi/traders-redone/blob/5bc436b2e2a97b34933617ecc77d69e226c68835/docs/vorlagen/review-anfrage.md#L5-L18) ist im PR unverändert; Kurzaufruf und Zusatz „Geplanter
Lauf“ enthalten anders als `spur-start.md` Z. 12–14 kein „Keine Aufgaben anlegen (kein `create_trigger`)“. Kein Widerspruch
(es wird nichts erlaubt, `anhang.md` F Z. 71–72 verbietet allgemein), aber der Reviewer liest laut Ablauf Schritt 1 nur
`AGENTS.md` und `zusammenarbeit.md` §5–6 – §4 Z. 51–53 mit dem Verbot liegt außerhalb. Der Aufruf dieses Laufs hat den Satz
von Hand ergänzt. Beleg: `grep -n "create_trigger" docs/vorlagen/review-anfrage.md` → keine Treffer. Vorschlag: Satz in den
Zusatz „Geplanter Lauf“ aufnehmen und dort den Namen auf „Geplanter Lauf mit Teilen“ korrigieren (Z. 17 nennt noch
„Kurzaufruf ‚Geplanter Lauf‘“, den es in `spur-start.md` nicht gibt; älter als der PR).

**R4PA-3 (P3) – „Einzelner Folgeteil“ verliert die Abgrenzung zu Schritt 3/5.**
[`spur-start.md` Z. 25](https://github.com/kaaasi/traders-redone/blob/5bc436b2e2a97b34933617ecc77d69e226c68835/docs/vorlagen/spur-start.md#L25) und Z. 54–55: Vorher galt „nur Teil <n>“ ausdrücklich „ohne
Schritt 5“. Jetzt heißt es nur „Nur Teil <n>.“; Schritt 3 („nächsten Teil im selben Lauf beginnen, solange Zeit bleibt“) und
Schritt 5 (Fortsetzung anmelden) sind für diesen Fall nicht mehr ausgenommen. Folge harmlos (die Leitstelle entscheidet über
`spur-wartet`), aber „Nur Teil n“ und „nächsten Teil beginnen“ widersprechen sich. Beleg: `git diff a25a5c8 5bc436b --
docs/vorlagen/spur-start.md` → entfernt „(dann ohne Schritt 5)“ und „keine Fortsetzung anlegen“. Vorschlag: „Nur Teil <n>;
danach Schritt 5 bzw. 6 ohne weiteren Teil.“

**Zu „Besonders prüfen“:**
1. Suche `grep -rnE "create_trigger|list_triggers|update_trigger|Selbstfortsetz|anlegen|Fortsetzung" AGENTS.md CLAUDE.md
   docs/prozess docs/vorlagen tools/claude/README.md`: `create_trigger`/`list_triggers`/`update_trigger` nur noch in
   `anhang.md` F (Leitstelle: Zugriff Z. 63, Modell Z. 69, Verbot Z. 71–72, Laufende Z. 79) und als Verbot in
   `spur-start.md` Z. 12, 55. „Selbstfortsetzung“: 0 Treffer. `zusammenarbeit.md` Z. 51–53, 79–80 und `spur-start.md` Z. 28–31,
   68–70 übertragen das Anlegen an die Leitstelle. `CLAUDE.md` Z. 39 und `leitstelle-start.md` Schritte 3/4 sprechen nur die
   Leitstelle an. Treffer in `docs/reviews/2026-11-11-r2-zugriff*.md` sind historische Berichte, keine Regeln. → erfüllt.
2. Label-Tabelle [`zusammenarbeit.md` Z. 88](https://github.com/kaaasi/traders-redone/blob/5bc436b2e2a97b34933617ecc77d69e226c68835/docs/prozess/zusammenarbeit.md#L88) („jeder Lauf, nach dem ein weiterer nötig
   ist (Fortsetzung, Nachprüfung, Review) oder der blockiert ist“) deckt ab: Schritt 1 (fremder Bearbeiter), 4 (blockiert),
   5 (Fortsetzung), 6 (Kettenende → Review; n = 3 → Entscheidung Nutzer), Fixlauf P0/P1 (Nachprüfung). Fixlauf nur P2/P3 →
   `leitstelle-delta`, kein weiterer Lauf → kein Widerspruch. Ein Review mit `review-nein` setzt kein `spur-wartet`; die
   Leitstelle wartet dort auf `review-nein` (Punkt 8) – stimmig. → erfüllt.

## 3. Ausgeführte Prüfungen

| Prüfung | Ergebnis |
|---|---|
| `git ls-remote …/traders-redone HEAD`, `…/traders-referenz HEAD` | beide erreichbar (`a25a5c8`, `7fcfb24`) |
| `git rev-parse HEAD` / `git status --porcelain` | `5bc436b2e2a9…` / leer |
| `tools/claude/fetch-reference.sh original`, `setup-workspace.sh` | Originaldaten verknüpft, SDK 10.0.402 |
| `git diff a25a5c8 5bc436b` | 3 Dateien, +23/−21, nur Doku |
| Suche wie oben unter „Besonders prüfen“ 1 | siehe dort |
| `tools/pruefen.sh --schnell` (Originaldaten, `TR_REQUIRE_GAMEDATA=1`) | 635 bestanden, 0 fehlgeschlagen, 0 übersprungen – grün |

Akzeptierte `[APPROX]`: keine (Doku-PR). Annahmen: keine; Mutationen entfallen (keine Tests geändert).
