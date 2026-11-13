# Review R5 `anbau-ki` – PR #125 (Nachprüfung)

Head `a6368acd1e468fff89e2ad3452903491d5b37890`, Base `41605360af8dd3dc29ea33b19213523a1822992b`, bisher geprüfter Head
`e1effc0` ([Erstreview](2026-11-13-r5-anbau-ki.md)). Reviewer: Claude (Opus), frische Sitzung ohne Implementierungskontext.
Checkout verifiziert (`git rev-parse HEAD` = Head, `git status --porcelain` leer), Originaldaten aus `kaaasi/traders-referenz`
(`7fcfb24`, Hashes in `pruefen.sh` geprüft), Disassemblierung `analyse/disasm` aus demselben Stand.

## 1. Merge-Empfehlung

**Ja** für diesen Head. Der Rebase ist sauber: `git range-diff b80c07d..e1effc0 4160536..a6368ac` zeigt die vier
Commits als identisch (`=`), neu ist nur `a6368ac`. Der Fixlauf behebt R5A-1 bis R5A-4 vollständig. Ich habe den Vertrag für
Spur 0 Befehl für Befehl gegen `0609:069F` und `07D6` gelesen, er stimmt. Die RefEmu-Probe bestätigt die Aufrufstellen erneut,
M4, M1b und M1c werden rot. Im Code ändern sich nur Kommentare, in den Tests eine Zeile (`4BA8`).

**Hinweis für die Merge-Prüfung (kein Befund am Delta):** GitHub meldet `mergeable_state: dirty`. Nach dem Merge von
`paket/planung` (#123) ergibt `git merge-tree --write-tree origin/main a6368ac` (main `7071187`) Konflikte in `STATUS.md` und
`docs/analyse/DATENLAYOUT.md`. Beides sind gemeinsame Dateien (erzeugter Block bzw. eigene Abschnitte). Vor dem Merge braucht
es einen Rebase. Eine erneute Nachprüfung ist dafür nicht nötig, wenn `git range-diff` außer diesen beiden Dateien nichts
Neues zeigt und `pruefen.sh --schnell` grün bleibt.

## 2. Befunde

**R5A-1 (P1) – behoben.**
[Befund Z. 13–17](https://github.com/kaaasi/traders-redone/blob/a6368acd1e468fff89e2ad3452903491d5b37890/docs/routines/135D_000E_computerzug.md?plain=1#L13-L17),
[Vertrag Z. 194–203](https://github.com/kaaasi/traders-redone/blob/a6368acd1e468fff89e2ad3452903491d5b37890/docs/routines/135D_000E_computerzug.md?plain=1#L194-L203),
Kommentare in `Planting.cs` und PR-Beschreibung nennen jetzt `0609:0961` (in `07D6`) und `0609:0746` (Ende von `069F`).
- **Probe (erneut ausgeführt):** In einer temporären Kopie hängt ein Code-Hook an `135D:0935`/`0AB3`. Er liest die
  Rücksprungadresse (`StackArgument(-2/-1)`) und `0840[p]`. Die Standardpartie (`StartGame`) läuft bis Woche 2.
  `dotnet test … --filter ProbeCallSites` liefert für alle 8 Aufrufe von `0935` (Wochen 0–1, Spieler 0–3)
  `mode=3 caller=0609:0966 comp=1` und für alle 4 Aufrufe von `0AB3` (Woche 0) `mode=3 caller=0609:074B comp=1`. Die
  Rücksprungadressen liegen direkt hinter `0961` bzw. `0746`.
- **Vertrag gegen die Disassemblierung** (`0609.asm`):
  - Die Verteilung `064D` (nur bei `3308[p]` = 0Ah) springt über die Tabelle `0697`: Modus 2 → `069F`, 3 → `07D6`,
    4 → `096A`, 5 → `0AEF`. Das steht so auch in `0609_047B_cursor.md` Z. 10.
  - `07D6`, Weg zu `0961`: Ist das Feld eigen (`4442` = `37AA`), geht es nach `080B`, sonst nach `08E5`. Dort folgen
    `100F(x, y)` = 1, `4442[cell]` = 0xFF und `43FA[p]` = 1. Danach in dieser Reihenfolge `10D2(x, y, p, 2EB1[cell])`,
    `21A7:0054(0, 0Ah)`, `1CB1(x, y)`, `43FA[p]` = 0 und `1A97`. Bei `0840[p]` = 1 folgt `call 135D:0935`.
  - `069F`: Ist `074D` = 0, geht es nach `074B` (Ende). Sonst `10D2(x, y, p, 4)`, `40FE`/`4102` = x/y, `1C83(x, y)`,
    `21A7:0054(0, 7)` und `43B2[p]` = 3. Bei `0840[p]` = 1 folgt `call 135D:0AB3`.
  - Bedingungen und Reihenfolge im Vertrag stimmen damit. Der Modus beim Aufruf ist in beiden Fällen 3, für `0AB3`, weil
    `43B2` vorher umgesetzt wird. Das passt zur Probe.
- Die Bytesuche im Vertrag lässt sich nachvollziehen: `call 0x135d:0x935` steht in `0609.asm` nur bei `0961`,
  `call 0x135d:0xab3` nur bei `0746`.

**R5A-2 (P2) – behoben.**
[Z. 126–128](https://github.com/kaaasi/traders-redone/blob/a6368acd1e468fff89e2ad3452903491d5b37890/docs/routines/135D_000E_computerzug.md?plain=1#L126-L128):
Der Befund sagt jetzt „einer der Fälle … und Güte ≠ 3“. Das entspricht dem Port (`rocketPath`, `||`). Die Gegenprobe M4
(Bedingung als `!( … || … || … )`) macht `CommitCandidate_matches_the_original` rot.

**R5A-3 (P3) – behoben.**
[PlantingDifferentialTests.cs Z. 59](https://github.com/kaaasi/traders-redone/blob/a6368acd1e468fff89e2ad3452903491d5b37890/tests/Traders.Tests/Ai/PlantingDifferentialTests.cs#L59)
setzt `4BA8` jetzt zufällig auf 0–7. M1b (`<= 3` → `< 3`) und M1c (Bedingung gestrichen) machen jetzt
`ComputerTurn_matches_the_original_for_all_modes` im Zufallstest rot. Im Erstreview blieben beide dort grün.

**R5A-4 (P3) – behoben**, jeweils gegen `135D.asm` geprüft:
- `02BD`: `036D cmp al,0FFh` / `0371 jmp 0431` → `inc si`, der Eintrag wird übersprungen
  ([Z. 75–77](https://github.com/kaaasi/traders-redone/blob/a6368acd1e468fff89e2ad3452903491d5b37890/docs/routines/135D_000E_computerzug.md?plain=1#L75-L77)).
- `0AB3:0B24 cmp [bx+4442],0FFh` steht jetzt als „frei“ im Text.
- Die Zeile `48EC` in der Zustandstabelle stimmt: `02BD` liest bei `043A`, `0935` liest bei `0943` und zählt bei `0A50` hoch.
- `0609_074D` Z. 7 nennt `069F` als HQ-Bau aller Spieler.

**R5N-1 (P3, Hinweis, nicht blockierend) – „Art“ im Vertrag für `10D2` ist nicht benannt.**
[Z. 196](https://github.com/kaaasi/traders-redone/blob/a6368acd1e468fff89e2ad3452903491d5b37890/docs/routines/135D_000E_computerzug.md?plain=1#L196)
schreibt `0609:10D2(x, y, p, Art)`. Im Original ist das die aktuelle Feldart `2EB1[x·6 + y]` (`0918`–`091D`), nicht die
Wunschart. Für Spur 0 wäre ein Halbsatz hilfreich. Der Port von `135D` ist davon nicht betroffen.
Beleg: `0609.asm` Z. 0918–0925.

**Erwartet offen, bestätigt:** Die Einbindung fehlt noch. `grep -rn "PlantingAi\." src` findet außer in `Ai/Planting.cs`
keine Treffer. Die Mehrrundenprobe mit dem Port im Ablauf liefert erst Spur 0. Die PR-Beschreibung sagt das richtig.

## 3. Ausgeführte Prüfungen

- `git range-diff b80c07d..e1effc0 4160536..a6368ac`: Commits 1–4 sind `=`, Commit 5 (`a6368ac`) ist neu. Diff `e1effc0..a6368ac`:
  5 Dateien (Befund, `0609_074D`, `arbeitspakete.md`, Kommentare in `Planting.cs`, eine Testzeile).
- `tools/pruefen.sh --schnell` auf dem Head, Ergebnis **grün** (Hashes, Architektur, `abdeckung.py` „Abdeckung aktuell“,
  Restore `--locked-mode`, Build mit Warnungen als Fehler):

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| Mit Originaldaten (TR_REQUIRE_GAMEDATA=1) | 760 | 0 | 0 |

- CI `linux` auf dem Head grün (`windows`/`downloads` übersprungen).
- Gegenproben in einer temporären Kopie (`git worktree`), Filter `Ai.PlantingDifferentialTests`:

| ID | Mutation | Ergebnis |
|---|---|---|
| Probe R5A-1 | Hook `0935`/`0AB3`, Rücksprungadresse, Standardpartie Wochen 0–1 | `0609:0966` ×8, `0609:074B` ×4, Modus 3, nur Computer |
| M4 | `0935`: Raketenbedingung negiert | rot (`CommitCandidate…`) |
| M1b | `06A1`: `4BA8 <= 3` → `< 3` | rot (`ComputerTurn…`) |
| M1c | `06A1`: Bedingung `4BA8 ≤ 3` gestrichen | rot (`ComputerTurn…`) |
| – | unmutiert | grün (3/3) |

- Ich habe `0609:064D`, `069F` und `07D6` sowie `135D:02BD`, `0935` und `0AB3` (betroffene Stellen) gegen den neuen
  Befundtext gelesen.
- `git merge-tree --write-tree origin/main a6368ac`: Konflikte in `STATUS.md` und `DATENLAYOUT.md` (siehe Hinweis in Teil 1).

**Akzeptierte `[APPROX]`:** keine neuen. **Annahmen** wie im Erstreview (HQ im Raster, Spalten/Zeilen ≥ 1, Wunschart 0–2),
unverändert und nicht geprüft.
