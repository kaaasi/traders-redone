# Erstreview R7 Spur 2 `handelski` – PR #184
Reviewer: Claude Opus 5.5 (geplanter Lauf, eigene Sitzung, nicht an #184 beteiligt) · Base `006509fd39910108111f78a44a27f049afd8c226`
(= `main`) · Head `5f1670ee50f119eda0b012cda53d767dd17c3523` · Checkout verifiziert (`git rev-parse HEAD` = Head,
`git status --porcelain` leer). Die Originaldaten wurden geprüft (Hashes in `tools/pruefen.sh`), `T.EXE` über `fetch-reference.sh disasm`
(`analyse/disasm/141D.asm`, `0B9F.asm`, `0C29.asm`).

## 1. Merge-Empfehlung

**Ja** für `5f1670e`. Es gibt keinen P0–P2-Befund. Alle drei Läufe von `tools/pruefen.sh` sind grün, und die Zahlen stimmen mit
der PR-Beschreibung überein. Die drei Gegenproben der Abnahme werden rot, ebenso die Regel #163 mit einer Verletzung. Den Vertrag
für Spur 1 habe ich je Einsprung gegen `141D.asm` und die Aufrufstellen in `0B9F`/`0C29` gelesen: Parameter und Rückgabe
stimmen. Die Erwartungswerte aller Differenztests kommen aus dem RefEmu. #163, #165 und #166 sind inhaltlich erledigt, und die
Dateigrenzen sind eingehalten. Offen bleiben drei P3-Punkte. Der wichtigste ist R7H-1: Der Rückgabewert von `ChooseSide` ist
richtig, wird aber von keinem Test geprüft.

## 2. Befunde

**R7H-1 (P3, Testlücke Vertrag) – Rückgabewert von `ChooseSide` (`0397`) ungeprüft.** Der Vertrag
([Befund Z. 28/44](https://github.com/kaaasi/traders-redone/blob/5f1670ee50f119eda0b012cda53d767dd17c3523/docs/routines/141D_0008_handelski.md#L28))
sagt, dass `ChooseSide` das Endwort von `[bp-2]` zurückgibt, und Spur 1 baut darauf. Die Abnahme verlangt „Ergebnis“ im Gleichschritt.
[`TradeAiHarness.Run`](https://github.com/kaaasi/traders-redone/blob/5f1670ee50f119eda0b012cda53d767dd17c3523/tests/Traders.Tests/Ai/TradeAiHarness.cs#L140)
verwirft den Wert jedoch. Der Gleichschritt nimmt das Stapelwort jedes Aufrufs neu aus dem Original
([`TradeAiGame` Z. 115–119](https://github.com/kaaasi/traders-redone/blob/5f1670ee50f119eda0b012cda53d767dd17c3523/tests/Traders.Tests/Ai/TradeAiGame.cs#L115-L119))
und führt den Rückgabewert nicht weiter.
- **Beleg (M4):** `return v;` → `return staleLocal;` in
  [`TradeAi.cs` Z. 63](https://github.com/kaaasi/traders-redone/blob/5f1670ee50f119eda0b012cda53d767dd17c3523/src/Traders.Core/Ai/TradeAi.cs#L63).
  `dotnet test --filter FullyQualifiedName~TradeAiDifferentialTests` (mit Daten) ergibt **4/4 grün**. Außerhalb der Tests
  ruft niemand `ChooseSide` auf (`grep -rn "ChooseSide(" src tests`: nur `TradeAiHarness.cs:140`).
- **Heute richtig:** In `141D.asm` schreiben alle Zuweisungen an `[bp-2]` ein ganzes Wort (`mov word [bp-0x2],…` bei `03B2`,
  `03CF`, `03E2`, `03F8`, `03FF`, `041F`, `042D`, `044E`). `0457` liest nur das untere Byte. Das Endwort ist also `v`.
- **Vorschlag:** Im Harness `[bp-2]` bei `141D:0460` lesen und mit dem Rückgabewert vergleichen, auch in den 2400 Variationen.

**R7H-2 (P3, Prozess) – PR schließt #163/#165/#166 nicht.** Die PR-Beschreibung nennt die Issues ohne `Fixes`. Laut
`issue_read` gilt für alle drei `closed_by_pull_requests.total_count = 0`. Nach `zusammenarbeit.md` §5 soll der behebende PR
sie schließen. **Vorschlag:** `Fixes #163, Fixes #165, Fixes #166` in die Beschreibung aufnehmen oder die Issues beim Merge von
Hand schließen.

**R7H-3 (P3, vorbestehend, nicht durch #184) – Architekturcheck folgt keinem `var`.** Probe mit `core_writers`/`scene_violations`
über `src/Traders.Core`:
- Gemeldet werden `ITextMemory t = _texts; t.Write(1,2,3);`, `IncidentText.FormatNumber(_d, _texts, …)`, `TextBufferMemory t = _t; t.Write(…)`,
  `_texts.Write(…)` und ein Parameter `ITextMemory mem`. Damit ist die Probe aus R6E-2 erfüllt.
- Nicht gemeldet wird `var t = _texts; t.Write(1,2,3);` → `[]`.
- Dieselbe Lücke besteht schon für die alten Schreiber: `private readonly A _a; void F() { var a = _a; a.Write(_state); }` → `[]`
  mit den Selbstprüfungsquellen `_core`.
- Ich lege dafür kein Issue an. Die Leitstelle entscheidet, ob es zu Spur 0 gehört.

### Gegenbeispiele (Abnahme der Spur)

Alle Mutationen liefen in einer temporären Kopie (`git worktree`, Head `5f1670e`), jeweils mit
`dotnet test … --filter FullyQualifiedName~TradeAiDifferentialTests` und `TR_REQUIRE_GAMEDATA=1`:

| # | Mutation | Ergebnis |
|---|---|---|
| M1 | `0397`: zweite Ziehung `if (random.Next(3) == 0) v = 2;` entfernt | **rot** 4/4, `ChooseSide week 0 player 0: DS:10D2 original F6 port E9` |
| M2 | `05A3`: `Level(p, 3) <= budget` → `<` | **rot** 1/4 (nur Variationen), `variation 276 of PlanBids week 3: DS:2D1C original 14 port 0A` |
| M3 | `0CCA`: `T.Buyer` → `T.Seller` (Spieler vertauscht) | **rot** 4/4, `Bought week 1 player 3: DS:2CFE original 00 port F6` |
| M4 | `ChooseSide` gibt `staleLocal` zurück | **grün** 4/4 → R7H-1 |
| #163a | `_STATE_WRITERS = {}` | **rot**: `python3 tools/check-architecture.py` meldet 4× „Selbstprüfung …: Befund erwartet, 0 gefunden“, rc 1 |
| #163b | `_texts.Write(segment, 0, 0);` in `IncidentScene.cs` nach Z. 215 | **rot**: „Szene ruft schreibende Kernroutine ITextMemory.Write über _texts“, rc 1; zurückgesetzt: rc 0 |
| #165 | `weeks = [15, 30, 15, 15, 30]` und `Assert.Contains("kind -1")` entfernt | **rot**: `Not found: "random(10) = 0"` (ohne die Entfernung schlägt zuerst „kind -1“ fehl) |
| #166 | `Catastrophes.Choose`: `random.Next(10) != 99` (Mutation aus R6K-3) | **rot statt Hängen**: 15/15 in `CatastropheFrameDifferentialTests`, `InvalidOperationException: no random state for kind 2 in week 3 within 100000 tries`, 2 min 20 s |

### Besonders geprüft

1. **Vertrag Spur 1:**
   - `0008`, `021A`, `0464`, `09F3` und `0E67` haben keinen Stapelparameter. Ihre Aufrufe `0B9F:000A`, `0C29:0075`, `00D0`,
     `0284` und `0445` legen davor nichts ab.
   - `0CCA` liest `[bp+6]` mit `cwd`, und `sub`/`sbb` gehen auf `2D88`. Der Aufrufer legt `push si` / `call` / `pop cx` ab
     (`0C29:04A9`). Das entspricht `Bought(d, short price)` mit `(long)… - price`.
   - `0397`: Das uninitialisierte Wort entsteht durch `dec sp; dec sp` bei `039A`. Der Harness setzt es bei `039C`, nach dem
     Frame. Das passt zu `staleLocal`. Zur Rückgabe siehe R7H-1.
   - Die Ziehungen stimmen in Reihenfolge und Bedingung: `0397` zweimal `random(3)` nur bei `[3780] > 4`. `0BFD` zieht
     `random(400)` nur bei `t ≥ 3` und `Surplus < 0x258` (`0C56`–`0C62`).
2. **Erwartungswerte aus dem RefEmu:**
   - `TradeAiHarness.AssertSame` vergleicht das Datensegment des Originals nach `_program.Call` mit dem des Ports.
   - `TradeAiGame` rechnet den Port beim Eintritt und vergleicht ihn beim Rücksprung an der Aufrufstelle mit `p.DataSegment()`
     des Originals. Das Feld `Expected` enthält dabei das Ergebnis des Ports; der Name ist irreführend, die Richtung stimmt.
   - Die 2400 Variationen erzeugt `Vary` mit `Inputs`. Der Port ist daran nicht beteiligt, und die Ergebnisse kommen nur aus dem Original.
   - M1–M3 zeigen, dass beide Wege rot werden können.
3. **#163/#165/#166 und Dateigrenzen:**
   - Die Tabellen oben belegen, dass alle drei Punkte erledigt sind.
   - Geändert wurden `src/Traders.Core/Ai/Trade*`, `tests/Traders.Tests/Ai/Trade*`, `docs/routines/141D_*`,
     `tools/check-architecture.py` und `tests/Traders.Tests/Disasters/**`.
   - Dazu kommen `STATUS.md` (nur der erzeugte Block, `abdeckung.py --check` grün), ein eigener Abschnitt in
     `DATENLAYOUT.md` und die Statuszeile in `arbeitspakete.md`.
   - `src/Traders.Core/Disasters/**` ist unverändert.

### Erwartet offen (bestätigt)

- Der Mensch läuft im Gleichschritt nur untätig mit (keine Tasten, Sitz nur während `0B9F` menschlich, `TradeAiGame` Z. 71–82).
  Menschen mit Eingaben kommen nur über die Variationen vor.
- Die Bedeutungen von `DS:43A2`, `455A`, `3D1C`, `2BB8` und `2BC0` sind in `DATENLAYOUT.md` als W gefolgert. Gesetzt werden
  diese Adressen in `0B9F`/`0C29`, also in Spur 1.

## 3. Ausgeführte Prüfungen

- Zugriff: `git ls-remote` auf beide Repos war erfolgreich.
- Einrichtung: `tools/claude/fetch-reference.sh original`, dann `disasm`, `tools/claude/setup-workspace.sh`,
  `source ~/claude-tools-traders/env.sh`.
- Volllauf: `tools/pruefen.sh` (alle drei Läufe) auf `5f1670e` ergibt **„Ergebnis: grün“**. Hashes, Selbsttest, Architektur,
  Abdeckung, Restore, Build mit `-warnaserror` und Marker waren grün.

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| Mit Originaldaten (`TR_REQUIRE_GAMEDATA=1`) | 1045 | 0 | 0 |
| Ohne Originaldaten (`TR_SKIP_GAMEDATA=1`) | 456 | 0 | 311 |
| Pflichtlauf ohne Daten (muss scheitern) | 456 | 589 | 0 |

- Markerbericht: `[ORIGINAL]` 538, `[APPROX]` 40, Core 21. Die PR bringt **keine neuen `[APPROX]`**, also gibt es nichts zu akzeptieren.
- Mutationen M1–M4, #163a/b, #165 und #166 siehe oben. Die Probe zu R7H-3 lief mit einem Python-Skript, das die Funktionen
  aus `tools/check-architecture.py` lädt.
- CI auf dem Head: `linux` success (Run 43870870412). `windows` und `downloads` wurden übersprungen.
- Annahmen, nicht geprüft: Die gemessene Verteilung der Stapelwörter (FFFF 19085× usw.) und ihre Herkunft habe ich nicht
  nachgemessen. Die Divisionsfehler bei `0029`/`0A1D` stehen nicht im Test. Laut PR-Beschreibung wirft der Port dort eine
  Ausnahme; ich habe das nicht ausgeführt.
