# Nachprüfung Review R6-Plan – PR #143 (Delta `5bbe3f7..ea1f33a`)

**Reviewer:** Claude Opus 5.5 (geplanter Lauf, unabhängige Sitzung, nicht am PR beteiligt) · **Base:** `15fd3bd` ·
**Geprüfter Head:** `ea1f33a26cd8444105b6444d31b809b70ed1e655` (Checkout nachgewiesen: `git rev-parse HEAD` = Head,
`git status --porcelain` leer) · Originaldaten geprüft (`traders-referenz` `7fcfb24`) · Vorreview:
[`2026-11-15-r6-plan.md`](2026-11-15-r6-plan.md) (Head `5bbe3f7`). Geprüft nur das Delta (3 Commits, `AGENTS.md`,
`docs/arbeitspakete.md`) und die genannten Befunde.

## 1. Merge-Empfehlung

**Ja** für genau `ea1f33a`. R6P-1 (P2) ist behoben: `23CD:06DE` steht nicht mehr als offen in der Liste. Der Weg zu den
Pixeln ist festgelegt: eine Regelseite im Kern, gespeist über eine explizite Eingabe, ohne Projektreferenz, mit Besitzer und
Differenztest gegen den VRAM. Auch `AGENTS.md` §4 passt dazu (`check-architecture.py` Z. 9 `'Traders.Core': set()`).
R6P-2 bis R6P-4 sind umgesetzt. Spur 3 `stationen` ist sauber geschnitten. Die Liste der 44 Routinen deckt sich genau mit dem
Aufrufgraph. Neu sind nur zwei P3. Sie betreffen die Abhängigkeit der Spur 3 von der Regelseite und einen Rest des alten
Atlas-Wortlauts. Beide blockieren nicht.

## 2. Befunde

**R6P-1 (P2) – behoben.** Ich habe das Gegenbeispiel erneut ausgeführt. `23CD:06DE` ist `size 5`, hat `calls []` und lautet
im Ladebild `2EFF2E3400` = `jmp far [cs:0x34]` → `3764`. `STATUS.md` Z. 434 führt `23CD:3764` als ✅. Laut Aufrufgraph rufen
`06DE` diese Routinen: `111D:0112` (an `0188`, `0228`), `1B03:08ED` (`09B1`), `1B03:0BE7` (`0BFE`), `1B03:1178` (`11CB`), dazu
außerhalb von R6 `0EDE:1FC3` und `20B3:069A`. Für R6 nennt der Plan also genau die richtigen Lesestellen
([Z. 36–45](https://github.com/kaaasi/traders-redone/blob/ea1f33a26cd8444105b6444d31b809b70ed1e655/docs/arbeitspakete.md#L36-L45)).
Besitzer ist Spur 1 (`Pixels/**`), Spur 3 ergänzt additiv `Pixels/Stations*`. Die Dateigrenzen sind disjunkt
(Z. 103, 143, 158). `06DE`/`3764` werden ausdrücklich nicht neu portiert.

**R6P-2 (P3) – umgesetzt** ([Z. 85–86](https://github.com/kaaasi/traders-redone/blob/ea1f33a26cd8444105b6444d31b809b70ed1e655/docs/arbeitspakete.md#L85-L86)):
Die Abnahme verlangt einen Assert für mindestens einen Lauf von `0112` mit einer Katastrophenart und eine im Test festgelegte
Woche bzw. ein festgelegtes Tastenmuster. **R6P-3 (P3) – umgesetzt** (Z. 157–158): Die erste Spur, die es braucht, setzt
`partial`, und zwar als einzige Änderung an der Datei. **R6P-4 (P3) – umgesetzt** (Z. 68–69, 80): Der Nachtrag steht jetzt in
`0609_0004_anbau.md`, und das ist die richtige Datei (sie existiert). Auch die Dateigrenze wurde auf `0609_0004*` umgestellt.

**R6N-1 (P3) – Spur 3 Teil 1 braucht die Regelseite doch; „Merge des Branches“ beißt sich mit der Rebase-Regel**
([Z. 148–149](https://github.com/kaaasi/traders-redone/blob/ea1f33a26cd8444105b6444d31b809b70ed1e655/docs/arbeitspakete.md#L148-L149)).
- Teil 1 umfasst den Aufbau mit den Ziehungen von `1178` 319/199/2 (Z. 134–135). `1B03:0009` lädt zuerst `V-RAKETE`
  (`186C:0002`) und ruft danach `1178` direkt, noch vor `00D4`/`0705`. In `1178` wird nach dem Lesen per `06DE`
  (`11CB`) `cmp ax,1` / `jnz 11F6` geprüft. Nur bei Pixel = 1 folgt `random(2)` (`11DB`). Wie viele Zufallszahlen gezogen
  werden, hängt also schon in Teil 1 von der Seite ab. Der Satz „Teil 1 braucht sie nicht“ stimmt deshalb nicht.
- **Beleg:** `ndisasm -b16` auf das Ladebild ab `0x1B03·16` (Segmentausschnitt `/tmp/1b03.bin`), Bereich `1178–1200`, und die
  Aufrufliste von `1B03:0009` in `callgraph.json`: `0079, 186C:0002(img:V-RAKETE), 148B, 21FD:037D, 1178, …`.
- Der Plan schlägt „per Merge des Branches“ vor. Das verträgt sich nicht mit der Pflicht, vor jedem Push
  `git rebase origin/main` auszuführen (`spur-start.md` Z. 46), und mit dem `--force-with-lease` der Spur 1. Der PR der Spur 3
  würde Dateien der Spur 1 (`Pixels/**`) mitführen und bei jedem Rebase der Spur 1 Konflikte erzeugen.
- **Vorschlag:** „Teil 1 braucht sie nicht“ ersetzen durch „Teil 1 braucht sie für `1178`“. Die Regelseite (Spur 1 Teil 1)
  kommt früh als eigener kleiner PR auf `main` (Integrationsfolge 1a vor 3). Bis dahin meldet Spur 3 `spur-wartet` oder
  arbeitet an den pixelfreien Routinen.

**R6N-2 (P3) – Alter Wortlaut „kanonischer Atlas im Kern“ an zwei Stellen**
- [`arbeitspakete.md` Z. 138](https://github.com/kaaasi/traders-redone/blob/ea1f33a26cd8444105b6444d31b809b70ed1e655/docs/arbeitspakete.md#L138)
  (Spur 3 Teil 2: „Kollision gegen `V-RAKETE` (kanonischer Atlas im Kern)“) und `docs/PORTIERUNGSPLAN.md` Z. 34–35 widersprechen
  dem neuen `AGENTS.md` §4 und der gemeinsamen Vorgabe. Spur 3 könnte so einen Atlas im Kern suchen oder anlegen.
  **Beleg:** `grep -rn kanonisch AGENTS.md docs/*.md` → genau diese zwei Treffer, dazu der Rückblick in
  `ERKENNTNISSE_VORVERSUCH.md`. **Vorschlag:** in beiden Fällen „Regelseite des Kerns (`Pixels/`)“ schreiben.
- *Hinweis, kein Befund:* `06DE` liest die jeweils aktuelle Zeichenseite. `111D:0112` liest vor und nach dem Seitenwechsel
  `21FD:051F` (`01B9`), `1B03:08ED` je Bild nach `051F` und `23CD:071C`. Das Modell „eine indizierte 320 × 200-Seite“ muss
  daher vielleicht je Seite geführt werden. Der vorgeschriebene Differenztest (Regelseite gleich dem VRAM der Zeichenseite an
  jeder Lesestelle) deckt eine Abweichung zuverlässig auf.

**Besonders geprüft, ohne Befund:**
- **Zuschnitt Spur 3:** Die Liste `1B03` in Z. 56–59 ist mengengleich mit dem Graphen (44 Routinen, siehe §3). Die Zufallsargumente
  `1178` 319/199/2 stimmen mit `callgraph.json` überein.
- **Kern/Szene:** Kern I, Kern II und Szene sind getrennt, „Kern- und Zeichenteil je Routine belegen und trennen“ ist verlangt.
- **Dateigrenzen:** `Stations/**`, `Pixels/Stations*`, `1B03_*` überschneiden sich mit keiner anderen Spur.
- **Startreihenfolge:** 1/2 sofort, 3/0 eine Stunde später. Das passt, weil Spur 1 die Regelseite als Erstes committet.
  Spur 0 Teil 3 wartet auf 1/2/3.
- **Aufruf 11 in Spur 0 Teil 3 (Z. 74–78):** Laut Graph ist die Wochenschleife `1777:01F8` so belegt: 11 = `1B03:0009`,
  15/17 = `21FD:0977(2/3)` (Prüfpunkt, leer), 18 = `1C68:000B`. Dass der Host an 18 hält und das Orakel ab 18/19 übernimmt, ist
  also widerspruchsfrei. Die Nachholgrenze 17–18 (#113) steht weiter im Plan.
- **`AGENTS.md` §4:** Die Regel passt zur Architekturprüfung und zum heutigen Muster `Func<int, ImageFileLengths>`. Einen Typ
  `IndexedImage` gibt es noch nicht. Mit „o. ä.“ darf Spur 1 ihn im Kern anlegen.
- **R7-Zeile:** „vorgezogen“ (Z. 189) ist konsistent.

**Erwartet offen, bestätigt:** P3 #49, #75, #76 (P3-2/-3) stehen im Plan als „nicht in R6“ (Z. 177). Der Ton `21A7:0054`/`000D`
mit `218B:013A`/`0153`, `27C6:041B`, `2850:00CC` bleibt bis R10 Ereignis bzw. `[APPROX]` (Z. 35). Im Graphen sind diese sechs auch
in `1B03:0009` die einzigen offenen Routinen außer `1B03` und `06DE`.

## 3. Ausgeführte Prüfungen

- `git ls-remote` auf beide Repos: ok (`f69f447`, `7fcfb24`). `tools/claude/fetch-reference.sh original`,
  `setup-workspace.sh`. `git diff --stat 5bbe3f7 ea1f33a`: nur `AGENTS.md` (+2/−1) und `docs/arbeitspakete.md`.
- `python3 -I tools/callgraph.py gamedata/dos/T.EXE /tmp/cg` → 1100 Funktionen, 3685 Aufrufe. Danach ein Skript, das
  transitiv vom Einsprung aus läuft und an den Adressen mit ✅ in `STATUS.md` abbricht (323 Zeilen; Segment `0000` ausgelassen):

| Einsprung | offen laut Graph (ohne Ton) | Plan |
|---|---|---|
| `1B03:0009` | 44 × `1B03` (`0009` … `1617`), `23CD:06DE` | mengengleich (`diff` leer); `06DE` als `3764` ✅ vermerkt |
| `111D:0009` | `0112 0264 03B7 0519 0757 07E2`, `23CD:06DE` | gleich, `06DE` vermerkt |
| `17CB:0005` | `001E 02BD 039D 047E 04D7 0736 0986 09E0` | gleich |
| `22A3:02D3` | `0340 0426 04B1 04DA 04F7 062E 07C2 0806 082F 0874 08C1` | gleich |

- Mit `ndisasm -b16` ausgewertet: `1B03:08ED–0A62`, `0BE7–0C89`, `1178–1257` und `111D:0112–0264` (Aufrufe, Vergleiche).
- `tools/pruefen.sh --schnell` (Head `ea1f33a`): Hash, Architektur und Abdeckung grün, Ergebnis grün.

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| Mit Originaldaten (TR_REQUIRE_GAMEDATA=1) | 882 | 0 | 0 |

dotnet 10.0.402. Geändert wurde nur Doku, daher keine Mutationen. **Akzeptierte `[APPROX]`:** keine neuen. **Annahme:** Ob der
Pixelwert 1 in `1178` auf der `V-RAKETE`-Seite tatsächlich vorkommt, habe ich nicht im RefEmu gemessen. R6N-1 stützt sich auf
den Kontrollfluss.
