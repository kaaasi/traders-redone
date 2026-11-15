# 111D:0009 / 111D:0112 / 111D:0264 / 111D:03B7 / 111D:0519 / 111D:0757 – Katastrophen (Wächter R5, Rest und Arten R6)

- **Ladebild-Adresse:** `111D:0009` (Dateioffset `0x147D9`, Segment `111D` bei `0x147D0`)
- **Aufgabe:** Aufruf 4 der Wochenschleife `1777:01F8` (`1777:021A`). Der **Wächter** (R5P-6) kehrt in den ersten Wochen und bei
  abgeschalteten Katastrophen sofort zurück. Der **Rest** ab `001E` (R6, Spur `katastrophen` Teil 1, Abschnitt „Rest ab 001E“):
  ob und welche Katastrophe kommt, Ränge, Titel, Sternvorführung `0112` (Befund `111D_0112_regelseite.md`), eine der Arten
  `0264`/`03B7` mit ihrer Wirkung auf die Felder, Meldefenster `0757`, Warteschleife `21FD:0004(3)`.
- **Aufgerufen von / Phase:** nur `1777:021A` (Aufruf 4, nach der Wochenbilanz).
- **Parameter / Rückgabe:** keine; AL wird von der Wochenschleife nach Aufruf 4 nicht ausgewertet.

## Ablauf des Wächters (eigene Worte)

`push bp`/`mov bp,sp`/`push si`; Wort `DS:4B9A` (Woche, mit Vorzeichen) mit 2 vergleichen: nicht größer (`jng`) → Sprung zum
Ende `010F`. Sonst Byte `DS:2F30` mit 0 vergleichen: gleich → Ende. Erst dann (`001E`) folgt der Rest mit `21FD:01BA(10)`.
Das Ende `010F` ist `pop si`, `pop bp`, `retf` – kein Schreibzugriff, kein Zufall.

## Zustand

| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `4B9A` | W | s | Woche (`DgroupLayout.Week`) | gelesen |
| `2F30` | B | u | Katastrophen an/aus (`DgroupLayout.Catastrophes`) | gelesen |

## Seiteneffekte in Reihenfolge

Im Wächterweg keine (kein Zufall, keine Uhr, keine Grafik, kein Ton, keine Eingabe).

## C#-Umsetzung

`src/Traders.Core/Phases/CatastropheGuardStep.cs`: `ReturnsAtOnce` = Wächter; der Schritt endet dort sofort. Sonst meldet er
sich über `IPartialWeekStepHandler.IsPending` als ausstehend: die Phasenmaschine hält an Aufruf 4 an (`PhaseStatus.Blocked`,
Ereignis `StepPending`), ohne ihn zu beginnen. Ein Test kann den Rest mit einem Orakel ausführen (Parameter `rest`).

## Prüfung

`tests/Traders.Tests/Phases/CatastropheGuardDifferentialTests.cs`: Original über `LoadOnly`/`Call(111D:0009)` mit gesetzter
Woche (−32768, −1, 0, 1, 2, 3, 4, 15, 30, 32767) und `2F30` (0, 1, 80h, FFh); der Rest ab `001E` ist durch den eigenen Epilog
ersetzt, sein Erreichen wird aufgezeichnet. Wächterweg: Port ändert nichts, ganzes Datensegment samt `10D2` gleich dem Original;
sonst ist der Port ausstehend, genau wenn das Original `001E` erreicht. In der Mehrrundenprobe
(`MultiWeekScreenDifferentialTests`) läuft Aufruf 4 im Port, solange der Wächter greift, sonst aus dem RefEmu.

## Rest ab 001E (R6, eigene Worte)

**Zufall:** `21FD:01BA random(n)` liefert **0 … n** (Rest durch n + 1, `OriginalRandom.Next`); alle Bereiche unten schließen n ein
(Berichtigung 15.11.2026, Fortsetzung 1: der erste Nachtrag las `random(n)` als 0 … n − 1).

**Auswahl (`001E–0046`):** `r = random(10)`; ist `r ≠ 0` und die Woche kein Vielfaches von 15 (`idiv`, Rest mit Vorzeichen), Ende
(nur diese eine Ziehung). Sonst `si = random(2)` ∈ {0, 1, 2}. Eine Katastrophe kommt also mit 1/11 und in den Wochen 15, 30, … immer.

**Ablauf danach:** Ränge `0825:06CB`; Bild `[08CE]` (`V-ANBAU`) laden, Ausblenden `21FD:037D(550h)`; Liste leeren, Kacheln
`0609:1ED9`, Rahmen `0609:1256(8Ch, 88h, A0h, 2Ch)`, Titel `0A69:00D3(8Ch, A0h, A0h, Text [385C], 5, 0Bh, −1, 1)`; `21FD:060D`
(beide Seiten); Palette `DS:00A8`, Einblenden `0329`; Bild `[08D2]` (`V-BERGE`) laden, Ausblenden `037D(550h)`; Sternvorführung
`111D:0112`; dann nach `si`: 0 → `0264`, 1 → `03B7`, 2 → `0519` (je 1/3); zuletzt `21FD:0004(3)`.

**`0264` (Art 0, neue Bodengüte):** Palette `DS:07A0`, Einblenden, `0185(100)`, Ton `21A7:0054(0, 48h)`; 80-mal `0245`, zweimal
(`051F`, `0185(1)`); Ton aus; Meldung `0757(DS:3854, 2)`; **`0E31:0361`** (Bodengüte neu wie bei einer neuen Partie, nach `DS:08F1`
zufällig oder „echt“); dann für alle Felder (Zeile `DS:2F00` außen, Spalte `DS:2EB0` innen, Zelle Spalte·6 + Zeile): Hauptquartier
(Art 4) → Güte 3; Raketenfeld (Art 3) mit Güte 3 → Güte `random(2)`; zuletzt `0185(100)`.

**`03B7` (Art 1, Feldarten wechseln):** Palette `DS:0780`, Einblenden, `0185(100)`; Farben 2 … 7 − i aus `DS:0780 + 2·Farbe` + i + 1
(`2295:006D`) für i = 0 … 5, je `0185(10)`; Ton `49h`, `0185(250)`, Ton aus; rückwärts i = 5 … 0 mit + i; Meldung `0757(DS:384C, 2)`;
`n = random(15) + 10` Ziehungen: Spalte `random(Spalten − 1)` nach `DS:2EB0`, Zeile `random(Zeilen − 1)` nach `DS:2F00` (also jede Spalte
und Zeile); ist das Feld kein Hauptquartier: `0609:10D2(x, y, Besitzer, random(2))` – Besitzer bleibt, Art
0 oder 1, Zähler wandern mit.

**`0519` (Art 2, Felder geräumt):** Palette `DS:0780`, Einblenden, `0185(100)`, `21FD:0590`, Farbe 12 = 770h;
4-mal: `0245`, Ton `(1, 4Ch)`, Spalten E5h … 0 an (`111D:07E2(x, 1)`), `0185(5)`, `0245`, Spalten E5h … 0 aus; `0185(100)`; Farbe 12 =
700h, Ton `(1, 4Bh)`, `0185(1)`, Ton `(2, 4Bh)`, Spalten 0 … E5h an, `0185(10)`, `0245`, Spalten 0 … E5h aus; Ton `(0, 4Ah)`, 50-mal
`0245` und zweimal (`051F`, `0185(1)`); Ton aus; Meldung `0757(DS:3834, 6)`; dann für alle Felder: Besitzer nach `DS:37AA`; ist er
≠ −1 und das Feld kein Hauptquartier: `r = random(19)`; ist `r ≤ 11 − 3·Rang[Besitzer]` (`DS:3D42`), `0609:10D2(x, y, Besitzer, −2)`.

**`0757(Zeilen, Anzahl)` (Meldefenster):** Bild `[08CE]` laden, Ausblenden `037D(4)`, Liste leeren, Kacheln, Rahmen
`0609:1256(28h, 4Bh, F0h, 32h)`, `060D`, Palette `DS:00A8`, Einblenden; getippte Zeilen
`0A69:0288(28h, (32h − 6·Anzahl)/2 + 51h, F0h, Zeilen, Anzahl, 5, 3)`. Wartebilder von `0288` (über `0243`/`01AD`): je Zeile mit Zeiger ≠ 0 je Zeichen
(solange `strlen > i`, vorzeichenlos) Liste leeren, Zeichen nach `DS:0140`, `0A96:0525`, `21FD:060D` (ein Seitenwechsel mit seinen
Wartebildern), Ton `21A7:0054(1, 16h)` außer bei Leerzeichen, `21FD:0245`, `21FD:0185(3)`; keine Schlusswartezeit (−1). Die Texte
stehen im Textpuffer (`DS:3834` … `385C` sind Einträge 26 … 36 der Texttabelle `DS:37CC`, `192D:18B6`).
**`21FD:0590`** (in `0519`) wartet zwei Bilder: `0B67:0238` zeigt die Seite aus `DS:0830` erneut (`0196`/`01B5`, ein Rücklauf), dann
`0B67:028F` (ein Rücklauf); keine Tastenabfrage wie in `051F`; `DS:023A` wird 0 (Ausgabeschicht).

**`07E2(x, an)` (nur Zeichnen):** `si = max(0, 45 − x/3)`, `h = 46 − x/5 − si`; Liste leeren; an: senkrechte Linie
`0A69:002C(x + 60, si, h, 12)`, sonst Atlas-Spalte `0A96:0030(x + 60, si, 1, h, x + 60, si)`; Liste ausführen. Gehört zur Szene (Teil 2).

**Sternvorführung `0112`** (Lesestellen und Regelseite: [111D_0112_regelseite.md](111D_0112_regelseite.md)): Liste leeren, Blit
`0A96:0030(0, 0, 140h, C8h, 0, 0)` (Bild `V-BERGE` aus dem Atlas), `06A8`, Liste leeren; 90-mal `x = random(13Fh)` (0 … 319), `y =
random(96h)` (0 … 150), beide gemerkt, `23CD:06DE(x, y)` auf der Zeichenseite; ist es 1, `0A96:03FB(x, y, random(2) + 8)` (Farbe 8 … 10);
`06A8`, `21FD:051F`; Liste leeren, Blit `0030(0, 0, 140h, C5h, 0, 3)`, Rechteck `04D7(0, 0, 140h, 3, 1)`; je gemerktem Stern
`06DE(x, y − 3)` und bei 1 `03FB(x, y, random(2) + 8)`; `06A8`. Kein Wartebild außer dem Seitenwechsel. Gelesen wird in der ersten
Schleife das unveränderte `V-BERGE` (Zeilen 0–150), in der zweiten die andere Seite **vor** dem Blit (Zeilen −3–147): dort stehen die
Kacheln `0609:1ED9` und der Rahmen `0609:1256(8Ch, 88h, A0h, 2Ch)` aus `0009` (der Titel liegt tiefer, im Test belegt).

**Gerufene, noch nicht portierte Routinen** (`tools/callgraph.py`-Liste der Leitstelle nachgeprüft): nur `111D:07E2` (Szene); alle übrigen Ziele sind portiert (`0825:06CB`, `0E31:0361`, `0609:10D2`,
`0609:1ED9`/`1256`, `0A69:00D3`/`0288`/`002C`, `0A96`, `21FD:0004`/`0185`/`0245`/`026C`/`0329`/`037D`/`051F`/`0590`/`060D`,
`2295:006D`, `186C:0002`, `21A7:0054` als Ereignis).

### Zustand (Rest)

| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `4B9A` | W | s | Woche (Vielfaches von 15) | gelesen |
| `08FC` / `08FE` | W | s | Spalten / Zeilen | gelesen |
| `2EB0` / `2F00` | B | s | Schleifen-Spalte / -Zeile (`LoopX`/`LoopY`), bleiben hinter dem Ende stehen | geschrieben |
| `2EB1` / `4442` / `45DA` | B[54] | s | Feldart / Besitzer / Bodengüte | gelesen, geschrieben (`0264`, über `0609:10D2`) |
| `3D42` | B[4] | u | Rang (`0519`) | gelesen; geschrieben von `0825:06CB` |
| `37AA` | B | s | Besitzer des zuletzt gelesenen Felds (`0519`) | geschrieben |
| `08F1` | B | u | Landart für `0E31:0361` | gelesen |
| `08CE` / `08D2` | W | s | Bildnummern Feld / Berge | gelesen |
| `385C`, `3854`, `384C`, `3834` | D | – | Fernzeiger auf Titel und Meldezeilen | gelesen (nur weitergereicht) |
| `07A0`, `0780` | W[] | u | Paletten der Arten (12-Bit-Farben) | gelesen |

### Zufall in Reihenfolge

`random(10)`; dann (wenn es kommt) `random(2)`; `0112`: 90 × (`random(13Fh)`, `random(96h)`, bei Pixel 1 `random(2)`), zweite
Schleife je Pixel 1 `random(2)` – die Zahl hängt vom Seiteninhalt ab; `0264`: Ziehungen von `0E31:0361`, dann je Raketenfeld mit Güte 3 `random(2)`; `03B7`:
`random(15)`, dann je Ziehung `random(Spalten − 1)`, `random(Zeilen − 1)`, außer bei Hauptquartier `random(2)`; `0519`: je Feld
mit Besitzer außer Hauptquartier `random(19)`.

### C#-Umsetzung (Rest)

`src/Traders.Core/Disasters/Catastrophes.cs`: `Choose` (Auswahl), `Run` (Rest ab `001E`, Bild für Bild), `LandRedrawn` (`0264`),
`FieldsChanged` (`03B7`), `FieldsCleared` (`0519`), `Message` (`0757`). Ausgaben als `DisasterEvent` (`DisasterEvents.cs`), Wartebilder
wie in der Anbauphase (`Planting.Fade`/`Flip`/`Retraces`). `CatastropheStep` ist der Rest-Handler hinter `CatastropheGuardStep`
(Vertrag `IWeekStepHandler`: `Begin` bis zur ersten Wartestelle, `Tick` je Bild, fertig im Bild der Rückkehr, sofort ohne Katastrophe;
Bildladen mit `ImageTable.RecordLoad`); eingebunden wird er von Spur 0 Teil 3. `StarShow` (`0112`) im Kern auf der Regelseite
(`RulePage`, `src/Traders.Core/Pixels/`): `Run` zeichnet vor `0112` Kacheln und Rahmen (`FieldBackdrop`, `0609:1ED9`/`1256`/`1F04` über
`RulePage.Tiles` = `0A96:0267`/`0884`) auf beide Seiten, folgt jedem Seitenwechsel und `21FD:0590` (`RulePage.DrawOnShownPage`);
Zeichenbefehle der Vorführung als `DisasterEvent` (`Blit`, `FillRect`, `SetPixel`, `ExecuteList`). Was nach `0112` gezeichnet wird,
liest `111D` nicht mehr und steht nicht auf der Regelseite. `CatastropheStep` bekommt Regelseite und Bildpixel (`Func<int, IndexedImage>`)
als Eingabe. Die getippten Zeilen (`TypeLines`, `0A69:0288`/`0243`/`01AD`) liefern ihre Wartebilder je Zeichen; die Textlängen kommen
aus dem Textpuffer als expliziter Eingabe (`DisasterTexts`, im Host `ProgramStart.TextBuffer`), jedes Zeichen als Ereignis
`TypeCharacter` für die Szene. `DisasterHost` ist der Host mit Pause `21FD:0245` und Warteschleife `21FD:0004(3)` (Spur bilanz).

### Prüfung (Rest)

`tests/Traders.Tests/Disasters/CatastropheFrameDifferentialTests.cs` (`CatastropheFrameHarness`): `111D:0009` als Aufruf 4 der
Phasenmaschine (`CatastropheGuardStep` mit `CatastropheStep`, Host `DisasterHost`) im Gleichschritt mit dem Original aus einer
laufenden Partie (Abzug beim Eintritt in `0009`, Wochen 3, 4, 5, vier Computer; Grafik, Ton und Tastatur echt): an jedem Rücklauf
des Originals wartet der Port sein Bild, Datensegment (außer den Bytes der Ausgabeschichten, dazu `DS:0140` und `DS:023A`),
`DS:10D2` und Zahl der Ziehungen gleich, bei der Rückkehr erneut. Fälle: keine, Art 0, 1, 2 mit vier Computern und je eine Art mit
einem Menschen (Steuerung 4, Ins bestätigt die Warteschleife); die Art liest der Test aus dem Original (Eintritt `0264`/`03B7`/`0519`).
Gegenproben lokal rot: Wartezeit je Zeichen 2 statt 3; `21FD:0590` ohne Wartebilder. Nicht geprüft: Pause (P) mitten in der Routine.

`tests/Traders.Tests/Disasters/StarShowDifferentialTests.cs` (Abzug am Hauptmenü, Zeichnen des Originals echt; Ein-/Ausblenden,
Paletten, Ton, getippte Zeilen, `07E2`, Wartezeiten, Pause und Warteschleife durch `retf` ersetzt; mitgeschrieben die Aufrufe aus
Segment `111D`): `Catastrophe_with_star_show_matches_the_original` (24 Fälle: Wochen 3, 4, 5, 15, 30, Felder 9 × 6, 6 × 4, 4 × 3,
beide Landarten; keine, Art 0, 1, 2) – Folge der Aufrufe, `DS:10D2`, Zahl der Ziehungen und Datensegment (außer den Bytes der
Ausgabeschichten, `ProgramStartDifferentialTests.OutputOwned`, und den Tickzählern `117E`/`1180`) gleich, beim Eintritt in `0112`
Regelseite = VRAM in den gelesenen Zeilen beider Seiten; `Star_show_matches_the_original` (`0112` allein, 16 Fälle, beide Seiten
danach vollständig gleich dem VRAM); `Field_backdrop_matches_the_command_list` (Kacheln und Rahmen). `CatastropheDifferentialTests.cs`
(LoadOnly, Ausgaben und `0112` durch `retf` ersetzt): `Third_kind_matches_the_original` (`0519` direkt, ganzes Datensegment);
`Every_branch_of_0009_is_covered`: keine Katastrophe, Art 0, 1, 2, Treffer über Woche 15/30 und über `random(10)` = 0. Gegenproben
der Vorführung lokal rot: zweite Schleife liest `y` statt `y − 3`; Sterne sofort statt bei `06A8` auf die Seite; Rahmen nicht auf
der Regelseite; Seitenwechsel in `060D` ausgelassen; aus dem ersten Nachtrag (gegen die neuen Tests wiederholt): Grenze 12 statt
11 in `0519`; Art 0 und 1 vertauscht; Raketenfeld ohne Güteprüfung; Spalte `random(Spalten)` statt `random(Spalten − 1)`.
