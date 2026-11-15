# Review R6 – Regelseite des Kerns (PR #149, Erstreview)

Base `e04742b` · Head `bbd0b30` · Reviewer: Claude Opus 5.5 (geplanter Lauf, eigene Sitzung, nicht am PR beteiligt).
Checkout nachgewiesen: `git rev-parse HEAD` = `bbd0b3016f8a2adadc8b7457e2d8091b5316fa02`, `git status --porcelain` leer.
Originaldaten geprüft (`fetch-reference.sh original`, Referenz `7fcfb24`; Disassemblierung `… disasm`).

## 1. Merge-Empfehlung

**Ja** für genau `bbd0b30`. Die Regelseite gibt `23CD:3764` an allen Lesestellen von `111D:0112` exakt wieder. Die
Erwartungswerte stammen aus dem RefEmu: AX des Originals an den Rücksprüngen `018D`/`022D` und die VGA-Ebenen. Sie kommen nicht
aus dem Port. Der Befund „zweite Sternschleife liest vor `0112`“ stimmt mit der Disassemblierung überein. Eine Gegenprobe
bestätigt ihn: Wendet man die Liste sofort an, wird der Test rot. `Traders.Core` bleibt ohne Projektreferenz. Alle drei Läufe
sind grün, und der Pflichtlauf scheitert wie verlangt. Offen bleiben zwei P3. Keiner blockiert.

## 2. Befunde

**Gegenbeispiele R6P-1/R6N-1 erneut ausgeführt – erledigt.** `ndisasm` von `111D:0112–0263` (`analyse/disasm/111D.asm`):
- Schleife 1: `001B`, dann `0030(0,0,320,200,0,0)` und `06A8` (`013C`). Danach liest `06DE` an `0188`, gefolgt von `cmp ax,1`/`jnz`
  (`018F`/`0192`). Nur bei einem Treffer folgt `random(2)` (`0198`). Abgeschlossen wird mit `06A8` und `051F` (`01B4`/`01B9`).
- Schleife 2: `001B`, dann werden `0030(0,0,320,197,0,3)` und `04D7(0,0,320,3,1)` nur eingereiht. Gelesen wird an `0228` (`y + FFFDh`,
  `021F`), `random(2)` folgt erst nach einem Treffer (`022F`/`0232`/`0238`). Ausgeführt wird die Liste erst an `0259`.
- Die Zahl der Ziehungen hängt also von der Seite ab, und Schleife 2 sieht die Seite im Zustand vor `0112`. Das deckt sich mit dem Befund.
- `RulePage` stellt das nach: `Star_show_reads_the_rule_page_like_the_original` prüft 16 Läufe zu je 180 Lesungen gegen AX.
- Gegenprobe M7 (Testkopie: Blit und Rechteck schon beim Einreihen ausführen statt bei `06A8`): `Star_show…` wird rot. Die
  Reihenfolge ist damit aus dem Original belegt, nicht nur behauptet.

**Besonders geprüft – Spur 3 (`1B03:08ED`/`0BE7`/`1178`):** Alle drei lesen `06DE` und prüfen mit `cmp ax,1`
(`09B1`/`09B8`, `0BFE`/`0C05`, `11CB`/`11D2`). Für `1178` reicht die Schnittstelle so, wie sie ist:
- `1178` ruft `0030(0,0,320,200,0,0)` und `06A8`.
- Danach folgen 199-mal `random(13Fh)`, `random(C7h)`, `001B`, das Lesen und bei einem Treffer `03FB(x, y, [0DD9 + random(2)])`, jeweils mit `06A8`.
- Das sind `Blit`, `Get` und `SetPixel`.

`08ED` liest dagegen nach `051F` (`08F3`), `23CD:071C([082E])` (Objekte zurückschreiben, `090A`) und `06A8` der Liste aus
dem Vorbild. Dafür fehlen Zeichenschritte. Spur 3 kann sie nur über die öffentliche Schnittstelle ergänzen (`SetPixel`,
Indexer, `SelectDrawPage`). Siehe R6R-1. Seitenwechsel je Bild: `Flip`/`DrawPageOf` sind für beide Paritäten belegt. Gegenprobe
M2 (`Flip` ohne Wirkung) macht alle drei Tests rot.

**R6R-1 (P3) – `RulePage` ist `sealed` ohne `partial`. „Ergänzungen additiv in `Pixels/Stations*`“ geht nur über die öffentliche
Schnittstelle** ([RulePage.cs Z. 14](https://github.com/kaaasi/traders-redone/blob/bbd0b3016f8a2adadc8b7457e2d8091b5316fa02/src/Traders.Core/Pixels/RulePage.cs#L14); Plan `arbeitspakete.md` Z. 43–44 und
Befund [Z. 8](https://github.com/kaaasi/traders-redone/blob/bbd0b3016f8a2adadc8b7457e2d8091b5316fa02/docs/routines/111D_0112_regelseite.md#L8)).
- **Beleg:** Probe `src/Traders.Core/Pixels/StationsProbe.cs` mit `public sealed partial class RulePage { }` und
  `dotnet build src/Traders.Core -c Release` ergibt `RulePage.cs(14,21): error CS0260: Missing partial modifier on declaration of type 'RulePage'`.
- `_pages` ist privat. Die Regel R6P-3 („`partial` setzt die erste Spur“) nennt nur `OriginalProgram`/`GameState`, und `RulePage.cs`
  liegt außerhalb der Dateigrenze von Spur 3.
- Für Objekte (`071C`) und maskierte Bilder an Seitenrändern bleiben Spur 3 damit nur Erweiterungsmethoden mit Einzelpixeln.
  Das ist möglich, aber eine Änderung an `RulePage.cs` bräuchte `spur-wartet`.
- **Vorschlag:** Spur 1 (Besitzer) setzt `partial`, im Fixlauf oder in Teil 1. Kein Mergehindernis.

**R6R-2 (P3) – 16-Bit-Umlauf des Offsets im Leser ist ungetestet**
([RulePage.cs Z. 79](https://github.com/kaaasi/traders-redone/blob/bbd0b3016f8a2adadc8b7457e2d8091b5316fa02/src/Traders.Core/Pixels/RulePage.cs#L79), [Test Z. 185](https://github.com/kaaasi/traders-redone/blob/bbd0b3016f8a2adadc8b7457e2d8091b5316fa02/tests/Traders.Tests/Pixels/RulePageDifferentialTests.cs#L185)).
- **Beleg:** Gegenprobe M4 `ushort offset = (ushort)(…)` → `int offset = y * RowBytes + (x >> 2)`: alle drei Tests bleiben grün.
- Ursache: Keiner der Punkte führt über den Umlauf zurück in eine Seite, auch nicht `(0, short.MaxValue)` oder `(-32768, 0)`.
- Probe (Testkopie, Punkte `(0, 820)`, `(17, 1024)`, `(300, 1023)`, `(9, -820)` ergänzt):
  - unveränderter Port: grün, der Port stimmt also;
  - mit M4: rot, `page 0 (0, 820): original 6, rule page 0`.
- Die Lesestellen in R6 (`y` ≥ −3) erreichen den Umlauf nicht. „Für jedes Wortargument“ ist so aber nicht belegt.
- **Vorschlag:** die vier Punkte in `Pixel_reader_matches_for_every_word_argument` aufnehmen.

## 3. Ausgeführte Prüfungen

`tools/claude/fetch-reference.sh original`, `… disasm`, `tools/claude/setup-workspace.sh`, dann `tools/pruefen.sh` (alle drei Läufe):
Hashes, Selbsttest, Architekturgrenzen (`check-architecture.py`, `'Traders.Core': set()`, csproj ohne Referenz), Abdeckung,
Restore, Build ohne Warnungen: grün. Ergebnis: **grün**.

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| Mit Originaldaten (TR_REQUIRE_GAMEDATA=1) | 885 | 0 | 0 |
| Ohne Originaldaten (TR_SKIP_GAMEDATA=1) | 455 | 0 | 249 |
| Pflichtlauf ohne Daten (muss scheitern) | 455 | 430 | 0 |

Gegenproben (temporäre Kopie; `dotnet test --filter RulePageDifferentialTests`, 3 Tests; jeweils danach zurückgesetzt):

| ID | Mutation | Ergebnis |
|---|---|---|
| M1 | `Get` außerhalb 1 statt 0 | rot (2): `loop 2 read (148, -2) on page 0: original 0, rule page 1` |
| M2 | `Flip()` ohne Wirkung | rot (3) |
| M3 | Ebene `(x + 1) & 3` | rot (2) |
| M4 | kein 16-Bit-Umlauf | **grün** → R6R-2 |
| M5 | Blit: jede 4. Spalte aus der Nachbarspalte | rot (3) |
| M6 | `FillRect` ohne letzte Zeile | rot (2) |
| M7 | Test: Liste beim Einreihen statt bei `06A8` anwenden | rot (1, `Star_show…`) |
| M8 | `Get` nur in der Zeichenseite (keine Seite 1 von Seite 0 aus) | rot (1, `Pixel_reader…`) |

**Akzeptierte `[APPROX]`:** Die einzige neue Stelle ist `RulePage.Get` außerhalb beider Seiten = 0 (Z. 75–77). Der RefEmu liefert dort
0, das prüft der Test an negativen `y` auf beiden Seiten. Auf echter Hardware ist B3F10h… (von Seite 1) offener Bus, vermutlich
FFh. Alle fünf Leser in R6 vergleichen nur mit 1 (Disassemblierung oben), das Ergebnis der Regel ist also gleich.

**Annahmen (nicht geprüft):**
- `0EDE:1FC3` und `20B3:069A` (Leser außerhalb von R6) habe ich nicht angesehen.
- Den Inhalt der zweiten Seite vor `0112` im echten Wochenablauf (Kacheln `V-ANBAU`, Rahmen) übernehme ich aus dem Befund.
  Belegen muss ihn Spur 1 Teil 1.
