# Review PR #118 – Bewährter Weg für Schreibzugriff auf traders-referenz

Erstreview (Claude Opus, geplanter Lauf, frische Sitzung ohne Mitarbeit am PR). Base `542d0d4`, Head
[`5095d0f`](https://github.com/kaaasi/traders-redone/commit/5095d0f599b7d32334cbd4a648308969d25e5f1f); Checkout mit
`git rev-parse HEAD` = Head, `git status --porcelain` leer. Originaldaten geprüft (Referenz-Repo `7fcfb24`, Hashes grün).
Nur Doku (`CLAUDE.md`, `docs/vorlagen/leitstelle-start.md`), daher laut Auftrag nur `tools/pruefen.sh --schnell`.

## 1. Merge-Empfehlung

**Ja, für genau Head `5095d0f`.** Zu (1): Der neue Absatz in Schritt 0 umgeht keine Freigabe. Er lässt dasselbe Werkzeug
(`add_repo`) genau einmal erneut aufrufen, nachdem der Nutzer selbst die Anweisung im Chat gegeben hat – die Erlaubnis kommt
damit ausschließlich vom Nutzer, kein anderes Werkzeug, kein Proxy-/Sandbox-Umweg (`AGENTS.md` Z. 53–54, `umgebung.md`
„dieselbe Aktion nicht über ein anderes Werkzeug umgehen“). Widerspruchsfrei zu `CLAUDE.md` (Leitstelle beide Repos `push`,
neue Zeile verweist korrekt auf Schritt 0), zu `anhang.md` F (Läufe erst nach Einbinden beider Repos und erfolgreichem
`ls-remote`; der Absatz wiederholt genau das: „Erst danach geplante Läufe anlegen“) und zu `spur-start.md` (der Weg gilt nur
für die interaktive Leitstelle; Läufe behalten `read` und den Blockiert-Rückfall, `anhang.md` F Z. 66–67). Die Regel „kein
Schritt setzt einen Chatverlauf voraus“ (`zusammenarbeit.md` Z. 4) bleibt gewahrt, weil die Anweisung live eingeholt und nicht
aus einem früheren Chat übernommen wird. Zu (2): Der Klonbefehl passt zu `fetch-reference.sh` (Variable `TRADERS_REFERENZ_DIR`,
Z. 14) und funktioniert im vorgesehenen Fall (Zielordner existiert noch nicht), siehe §3. Es bleiben drei P3.

## 2. Befunde

**P3-1 – Klonbefehl scheitert, wenn `<arbeitsverzeichnis>/traders-referenz` schon ein voller Klon ist.**
[`leitstelle-start.md` Z. 18](https://github.com/kaaasi/traders-redone/blob/5095d0f599b7d32334cbd4a648308969d25e5f1f/docs/vorlagen/leitstelle-start.md#L18):
Sitzungen, denen das Referenz-Repo schon beim Start mitgegeben wurde, haben genau dort einen vollen, nicht sparsamen Klon (in
diesem Lauf `/home/claude/traders-referenz`, `git sparse-checkout list` → „this worktree is not sparse“). Dann nimmt
[`fetch-reference.sh` Z. 41](https://github.com/kaaasi/traders-redone/blob/5095d0f599b7d32334cbd4a648308969d25e5f1f/tools/claude/fetch-reference.sh#L41)
den Zweig „vorhandener Klon“ und bricht bei `sparse-checkout add` ab. Beleg (Kopie dieses Klons):
`cp -a /home/claude/traders-referenz $S/w/ && TRADERS_REFERENZ_DIR=$S/w/traders-referenz tools/claude/fetch-reference.sh original`
→ `fatal: no sparse-checkout to add to`, Exit 128, kein `gamedata/`-Link. Im beschriebenen Fall (Einbinden erst während der
Sitzung) tritt das vermutlich nicht auf, der Fehler ist laut. Vorschlag: „existiert der Ordner schon als Klon, `gamedata` direkt
verlinken bzw. ohne `TRADERS_REFERENZ_DIR` klonen“ – oder im Skript `sparse-checkout add` nur bei sparsamem Klon ausführen.

**P3-2 – `register_repo_root` ohne Ladehinweis.** Z. 19 nennt das Werkzeug wie selbstverständlich; in diesem Lauf liefert
`ToolSearch` „register_repo_root add_repo“ → „No matching deferred tools found“, und keine andere Datei im Repo erwähnt es
(`grep -rn register_repo_root` → nur diese Zeile). Wie bei `add_repo` „ggf. erst per `ToolSearch` laden; fehlt es, überspringen“
ergänzen.

**P3-3 – Zwei Wiederholungsregeln hintereinander.** Z. 14–15 sagt „auf diese Erlaubnis verweisen und erneut versuchen; scheitert
es weiter, dem Nutzer melden“, Z. 15–17 dann „Nutzer bitten … und `add_repo` danach genau einmal erneut aufrufen“. Die Reihenfolge
(erst Verweis-Wiederholung, dann Bitte an den Nutzer) und ob „genau einmal“ auch für den ersten Wiederholversuch gilt, ist nur
erschließbar. Vorschlag: „scheitert es weiter, den Nutzer um die Anweisung … bitten (bewährter Weg) und danach genau einmal …“.
Nur Wortlaut, kein Regelkonflikt.

## 3. Ausgeführte Prüfungen

- Zugriff: `git ls-remote https://github.com/kaaasi/traders-redone HEAD` → `542d0d4…`, `…/traders-referenz HEAD` → `7fcfb24…`.
- Checkout: `git rev-parse HEAD` = `5095d0f599b7d32334cbd4a648308969d25e5f1f`, `git status --porcelain` leer; Diff
  `542d0d4..5095d0f`: 2 Dateien, +7/−2.
- Klonbefehl, neuer Ordner: `TRADERS_REFERENZ_DIR=$S/w2/traders-referenz tools/claude/fetch-reference.sh original` → Exit 0,
  Prüfsummen DOS grün, `gamedata -> $S/w2/traders-referenz/original`, „Referenzdaten: … (7fcfb24, Teile: original)“.
- Klonbefehl, vorhandener voller Klon: siehe P3-1 (Exit 128).
- Abgleich gelesen: `CLAUDE.md`, `AGENTS.md`, `zusammenarbeit.md` Z. 1–10, `anhang.md` F, `umgebung.md`, `spur-start.md` Z. 13–23.
- `tools/claude/setup-workspace.sh` (dotnet 10.0.402), `tools/pruefen.sh --schnell`:

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| Mit Originaldaten (TR_REQUIRE_GAMEDATA=1) | 754 | 0 | 0 |

  Markerbericht: `[ORIGINAL]` 248, `[APPROX]` 28 (unverändert, Doku-PR). Ergebnis: grün.

**Akzeptierte `[APPROX]`:** keine neuen. **Annahmen:** Dass `add_repo` während der Sitzung keinen Klon im Arbeitsverzeichnis
anlegt, ist aus dem Absatz (eigener Klon, dann `register_repo_root`) erschlossen, nicht geprüft (Werkzeug hier nicht vorhanden).
