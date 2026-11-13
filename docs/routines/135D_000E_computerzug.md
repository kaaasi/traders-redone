# 135D:000E / 135D:0240 / 135D:0263 / 135D:0284 / 135D:00AC / 135D:04F5 / 135D:0A59 / 135D:02BD / 135D:0480 / 135D:05AF / 135D:06A1 / 135D:0851 / 135D:0935 / 135D:0AB3 – Computerzug im Anbau

Sammelbefund der Spur **anbau-ki** (R5). Portiert werden `135D:000E`, die Einsprünge `135D:0935` und `135D:0AB3` und die
internen Routinen `00AC`, `0240`, `0263`, `0284`, `02BD`, `0480`, `04F5`, `05AF`, `06A1`, `0851`, `0A59` sowie die beiden
`0609`-Helfer `074D` ([0609_074D_pflanzbar.md](0609_074D_pflanzbar.md)) und `100F` ([0609_100F_nachbar.md](0609_100F_nachbar.md)).
`135D` zieht **keinen Zufall** (R5P-5).

- **Ladebild-Adresse:** Segment `135D` (Dateioffset `0x16BD0`, Größe `0xC00`). `000E` bei `0x16BDE`.
- **Aufgabe:** Der Computerspieler wählt pro Cursorschritt im Anbau seine nächste Aktion (`DS:3308[p]`): Cursor bewegen,
  die zu bauende Art umschalten oder die Taste (0Ah) „ausführen". Dazu bewertet und merkt er sich ein Zielfeld und eine
  Pflanzart. Alles arbeitet auf dem Datensegment.
- **Aufgerufen von / Phase:** `000E` aus `0609:047B` (Cursorschritt, Befund [0609_047B_cursor.md](0609_047B_cursor.md)) **vor**
  der Auswertung der Aktion, je Computerspieler einmal pro Bild. `0935` einzig aus `0609:0961` im Modus-3-Handler `0609:07D6`
  (Feld gekauft), `0AB3` einzig aus `0609:0746` am Ende des HQ-Baus `0609:069F` (Modus 2); beide nur für Computerspieler
  (`0840[p]` = 1). Genaue Bedingungen und Reihenfolge im Vertrag unten. Im Port dockt Spur 0 über den vorhandenen Rückruf
  `IFieldOutput.ComputerTurn(Dgroup, x, y)` (`Field/FieldCursor.cs`) an `000E` an; `0935`/`0AB3` ruft Spur 0 an diesen beiden
  Stellen.

## Transitiv gerufene Routinen (Teil 1, Stand main `b80c07d`)

| Gerufen | Von | Status | Besitzer |
|---|---|---|---|
| `0609:10A7` (`FieldGrid.Inside`) | `0851`, `100F→1072` | **portiert** | gemeinsam (`FieldGrid`) |
| `0609:1072` (`FieldGrid.OwnedByCurrent`) | `100F` | **portiert** | gemeinsam (`FieldGrid`) |
| `0609:074D` | `00AC` | **diese Spur** (R5P-2) | anbau-ki |
| `0609:100F` | `05AF`, `08B7` | **diese Spur** (R5P-2) | anbau-ki |

`135D` ruft sonst nur `135D`-interne Routinen. Keine offene fremde Routine. `135D:08B7` (ein zweiter Schleifenkörper, der über
`100F` den Merker `4BBE` setzt) ist **toter Code**: kein `call` im gesamten Abbild zeigt darauf (geprüft per Maschinenbytes
`9AB7085D13` und Nahaufrufe). Er wird dokumentiert, aber nicht portiert; sein einziger Aufruf `0609:100F` wird ohnehin portiert.

## Ablauf (eigene Worte)

Spieler am Zug `p = DS:37AA`. Alle `[o+p]`/`[o+2p]`/`[o+4p]` sind Byte-/Wort-/Dword-Felder zu je 4 Spielern.

### `000E(x, y)` – Verteiler

- Ist die Bewegungsverzögerung `DS:3786[p]` (Doppelwort; verglichen als Hochwort `3788` vorzeichenbehaftet, Niederwort `3786`
  vorzeichenlos) **> 0**, tut `000E` nichts (Rückkehr ohne Schreiben von `3308`). Weiter nur, wenn Hochwort < 0 **oder**
  (Hochwort = 0 **und** Niederwort = 0).
- Sonst: `x → DS:43F6`, `y → DS:43F8` (Zielbezug für `0263`/`04F5`). Ergebnisaktion vorbelegt mit −1 (0xFF).
- Nach Modus `DS:43B2[p]`: 2 → `00AC` dann `04F5`; 3 → `02BD` dann `04F5`; 4 → `0A59`; 5 → `0480` dann `04F5`. Jede liefert die
  Aktion in `AL`. Andere Modi (Index `Modus − 2` vorzeichenlos > 3) lassen die Aktion bei −1.
- Zuletzt: Aktion (Low-Byte) → `DS:3308[p]`.

Aktionscodes wie in `0E9E`/`047B`: 0 rechts, 1 links, 2 hoch, 3 runter, 0Ah = Taste (Feld/Art ausführen), −1 nichts.

### `00AC` – Feldwahl Modus 2 (bestes freies Feld)

1. Vier Byte-Zähler (einer je Quadrant) auf 0. Für jeden Spieler `q` ≠ `p` mit HQ (`DS:40FE[q]` ≠ 0xFF): Quadrant
   `0240(40FE[q], 4102[q])` bestimmen und den Zähler dieses Quadranten erhöhen (Zahl fremder HQ je Quadrant).
2. Über alle Felder (x = `DS:2EB0` 0…Spalten−1 innen, y = `DS:2F00` 0…Zeilen−1 außen): nur Felder mit `074D` = 1
   (bepflanzbar). Güte (Byte, kein Zufall):
   - Eckabstand `e` = (x < Spalten/2 ? x : Spalten−1−x) + (y < Zeilen/2 ? y : Zeilen−1−y) (signierte 16-Bit-Division durch 2).
   - `score = e − 2·(fremde HQ im Quadrant 0240(x, y))` (Byte-Arithmetik).
   - Es gewinnt der **höchste** `score`; bei Gleichstand der **kleinere** Abstand `0263` (Manhattan zum Ziel `43F6/43F8`).
     Startwerte: bester Score −100 (0x9C), bester Abstand 100 (0x64).
   - Das gewählte Feld: x → `DS:40CE[2p]`, y → `DS:40D6[2p]`.

### `0240(x, y)` – Quadrant

`(2x div Spalten) + 2·(2y div Zeilen)` (alles vorzeichenbehaftete 16-Bit-`idiv`; Zähler und das x-Ergebnis zusätzlich ·2 bzw.
der y-Term insgesamt ·2). Ergebnis 0…3 im Raster.

### `0263` / `0284` – Manhattan-Abstand

`0263()` = `0284(DS:2EB0, DS:2F00, DS:43F6, DS:43F8)` = |`2EB0` − `43F6`| + |`2F00` − `43F8`| (Betrag über Vergleich/Subtraktion).

### `02BD` – Feldwahl Modus 3 (Erweiterung / Kandidatenliste)

- Setzt `DS:3C90[2p]` = 0 (gewählter Listenindex).
- Ist `DS:43FA[p]` = 1: `05AF` (baut Auswahl, siehe unten). Hat `05AF` etwas gefunden (`DS:4BBE[p]` = 0): Kandidat
  `x = DS:410A[2p]`, `y = DS:4112[2p]`; Abstand `0263`. Reicht bei Zeitlimit (`DS:45BA` ≠ 0) die Restzeit `DS:4520[2p]` ≥ Abstand,
  so x → `40CE[2p]`, y → `40D6[2p]`, fertig.
- Sonst Schleife `si` = 0…(Kandidatenzahl−1) über die Liste `DS:4654[p·54 + si]` (x), `DS:472C[p·54 + si]` (y),
  `DS:2F31[p·54 + si]` (Art): x/y in `2EB0`/`2F00`; verbrauchte Einträge (x = 0xFF, `036F`) werden übersprungen, die Schleife
  läuft weiter. Abstand `0263`. Nur weiter, wenn Abstand ≤ bisher bester (Start 100).
  - Art 3 (Rakete): nur wenn `DS:4BAC[p]` (Raketen) ≠ 0.
  - Art ≠ 3: nur wenn Vorrat `DS:3CF4[p·8 + 2·Tab8E7[Art]]` ≥ Obergrenze `DS:48A2` (Ware je Art über Tabelle `DS:08E7[Art]`).
  - Bei Zeitlimit = 1 zusätzlich Restzeit `4520[2p]` ≥ Abstand.
  - Passt der Kandidat: bester Abstand = Abstand; x → `40CE[2p]`, y → `40D6[2p]`, `si` → `3C90[2p]`. (Wählt so den **nächsten**
    passenden Kandidaten.)
- Fand die Schleife nichts (bester Abstand noch 100): Rückfall auf das eigene HQ: `40FE[p]` → `40CE[2p]`, `4102[p]` → `40D6[2p]`.

### `0480` – Feldwahl Modus 5 (Startfeld)

Sind keine freien Felder mehr (`DS:2F05` = 0): Ziel = eigenes HQ (`40FE[p]`/`4102[p]`). Sonst `05AF`, danach Ziel =
`410A[2p]`/`4112[2p]`.

### `05AF` – bestes freies (angrenzendes) Feld

Über alle Felder (x = `2EB0`, y = `2F00`): nur freie (`4442[cell]` = 0xFF). In Modus ≠ 5 zusätzlich `100F` = 1
(an eigenes Land angrenzend); in Modus 5 jedes freie Feld. Bewertung `06A1` (Byte). Es gewinnt der **höchste** Score, bei
Gleichstand der **kleinere** `0263`-Abstand (Startscore −100, Startabstand 100). Gewählt: x → `410A[2p]`, y → `4112[2p]`.
`DS:4BBE[p]` = 1, wenn nichts gefunden (Score blieb −100), sonst 0.

### `06A1` – Feldbewertung (Rückgabe, kein Zustand)

- Grundwert: hat der Spieler ≤ 3 Felder (`DS:4BA8[p]`) und ist die Güte `45DA[cell]` = 3, dann 0; sonst
  `3 · QualityTable[DS:40F6[2p] (Wunschart) · 4 + Güte]` (Tabelle `DS:0902`, `sbyte`).
- Dazu die Nachbarsumme `0851` der vier orthogonalen Nachbarn (voll) plus der vier Diagonalnachbarn (jeweils ganzzahlig ÷ 2).
- Rückgabe = Grundwert + Nachbarsumme. (Ein zusätzlich berechneter Eckabstand ist im Original totes lokales Zwischenergebnis
  und fließt nicht in die Rückgabe ein.)

### `0851(x, y)` – Nachbarbeitrag (Rückgabe, kein Zustand)

`s = 4442[x·6 + y]` (Besitzer). `0609:10A7(x, y)` = 0 (außerhalb) → 0. `s` = −1 (frei) → 0. `s` = `DS:37AA` (eigenes) → 0.
In Modus 5 → −1 (0xFFFF). Gehört das Feld einem Menschen (`DS:0840[s]` = 0) → `DS:2E8E` (ComputerIq) + 3; einem anderen
Computer → 0.

### `04F5` – Aktion aus Ziel und Cursor

Vergleicht Ziel (`40CE[2p]`/`40D6[2p]`) mit dem Cursor (`43F6`/`43F8`): Ziel-y > Cursor-y und Feld (x, y+1) kein HQ → 3 (runter);
Ziel-y < Cursor-y und Feld (x, y) kein HQ → 2 (hoch); Ziel-x > Cursor-x → 0 (rechts); Ziel-x < Cursor-x → 1 (links); sonst,
wenn Ziel-y = Cursor-y → 0Ah (am Ziel, ausführen); andernfalls (y gleich erschöpft, x blockiert) 0 falls Spalten/2 > Cursor-x,
sonst 1. HQ-Prüfung über Feldart `2EB1[43F6·6 + 43F8]` (hoch) bzw. `2EB2[…]` = Feld (x, y+1) (runter).

### `0A59` – Aktion Modus 4 (Art umschalten)

Kandidatenart `cl = 2F31[p·54 + 3C90[2p]]`. Ist die aktuelle Auswahl `DS:2E94[p]` < `cl` → 3 (runter = Auswahl eins hoch).
Sonst Kandidat als verbraucht markieren (`4654[p·54 + 3C90[2p]]` = 0xFF) und 0Ah (ausführen).

### `0935` – Kandidat eintragen (aus `0609:0961` in `07D6`, Modus 3 nach dem Feldkauf)

Index `si = DS:48EC[2p]` (Kandidatenzahl). Wunschart `cl = DS:40F6[2p]` (Low). Feld `2EB0 = 410A[2p]`, `2F00 = 4112[2p]`.
- Soll statt der Wunschart eine Rakete (Art 3) gebaut werden? Ja, wenn **einer** der Fälle gilt (`0979`–`099E`, jeder springt
  nach `09A0`): `37AA` = `DS:4580`, `DS:4BC2[p]` = 1 oder `DS:2EEA[p·4]` < 2 – und die Güte des Feldes nicht 3 ist. Dann
  `cl` = 3. (Der Zwischenwert `0x4BAC` bei `09A0` ist eine Konstante ≠ 0 und wählt immer den Raketen-Prüfzweig.)
- Sonst (kein Fall oder Güte 3) Güteprüfung der Wunschart: ist `QualityTable[cl·4 + Güte]` = 0 → nichts eintragen. Sonst
  `cl` = Wunschart.
- Ist die aktuelle Feldart `2EB1[cell]` bereits `cl` → nichts eintragen.
- Sonst: `cl` → `2F31[p·54 + si]`, `2EB0` → `4654[p·54 + si]`, `2F00` → `472C[p·54 + si]`, `DS:48EC[2p]` += 1.

### `0AB3` – Wunschart bestimmen (aus `0609:0746` am Ende von `069F`, HQ-Bau Modus 2)

Vier Byte-Summen (je Art 0…2 + eine ungenutzte) auf 0. Über das 3×3-Feld um das HQ (x = `40FE[p]`−1…+1, y = `4102[p]`−1…+1),
nur Felder im Raster (`10A7`) **und frei** (`4442` = 0xFF, `0B24`): für Art j = 0…2 die Summe
`+= QualityTable[j·4 + Güte des Feldes]`. Danach die Art mit der
höchsten Summe wählen (Start: die bisherige Wunschart `DS:40F6[2p]`); Gewinner → `DS:40F6[2p]`.

## Zustand (geschrieben/gelesen)

| DS-Offset | Breite | Bedeutung | Routinen |
|---|---|---|---|
| `37AA` | B s | Spieler am Zug | L alle |
| `3786` | DW[4] | Bewegungsverzögerung (Wächter in `000E`) | L `000E` |
| `3308` | B[4] s | Aktion des Spielers | **G** `000E` |
| `43F6` / `43F8` | W s | Zielbezug (x/y aus `000E`) | G `000E`, L `0263`/`04F5` |
| `43B2` | B[4] u | Modus (2–5) | L `000E`/`05AF`/`0851` |
| `40CE` / `40D6` | W[4] s | gewähltes Feld x/y | **G** `00AC`/`02BD`/`0480`/`04F5`(L) |
| `410A` / `4112` | W[4] s | `05AF`-Kandidat x/y | **G** `05AF`, L `02BD`/`0480`/`0935` |
| `4BBE` | B[4] u | 1 = `05AF` fand nichts | **G** `05AF` |
| `3C90` | W[4] s | gewählter Kandidatenindex | **G** `02BD` (0 bzw. `si`) |
| `4654` / `472C` / `2F31` | B[4·54] | Kandidatenliste x/y/Art (Stride 54) | L `02BD`/`0A59`, **G** `0935`, **G**(0xFF) `0A59` |
| `48EC` | W[4] s | Kandidatenzahl | L `02BD` (Schleifenende), L/**G** `0935` (+1) |
| `40F6` | W[4] s | Wunschart | L `06A1`/`0935`, **G** `0AB3` |
| `40FE` / `4102` | B[4] s | HQ-Spalte/-Zeile (0xFF = kein HQ) | L `00AC`/`02BD`/`0480`/`0AB3` |
| `2EB0` / `2F00` | B s | globale Schleife x/y (hinterlassen = Spalten/Zeilen) | L/G alle Schleifen |
| `45D0` | W s | globale Schleife (in `0AB3`) | L/G `0AB3` |
| `2E94` | B[4] s | aktuelle Auswahl (−1…4) | L `0A59` |
| `2EB1` | B[9·6] s | Art je Feld | L `04F5`/`0935`/`074D` |
| `45DA` | B[9·6] s | Bodengüte je Feld | L `00AC`(074D)/`06A1`/`0935`/`0AB3` |
| `4442` | B[9·6] s | Besitzer je Feld | L `05AF`/`0851`/`074D`/`100F` |
| `4BA8` | B[4] u | Felder je Spieler | L `06A1` |
| `4BAC` | W[4] s | Raketen je Spieler | L `02BD` |
| `3CF4` | W[4·4] | Vorrat je Spieler/Ware (Stride 8) | L `02BD` |
| `48A2` | B s | Obergrenze (`0609:1178`) | L `02BD` |
| `4520` | W[4] s | Restzeit | L `02BD` |
| `45BA` | B u | Zeitlimit an | L `02BD` |
| `2F05` | B u | freie Felder | L `0480` |
| `0840` | B[4] u | Computer (1) je Spieler | L `0851` |
| `2E8E` | W s | ComputerIq (global) | L `0851` |
| `4580` | B s | Merker (gesetzt in `11A3:0180`) | L `0935` |
| `4BC2` | B[4] u | Fat-Mike-Merker | L `0935` |
| `2EEA` | B[4·4] | je Spieler/Ware (Stride 4) | L `0935` |
| `08E7` | B s | Art → Ware (Index) | L `02BD` |
| `08E4`/`0902` | B s | KindWare / QualityTable | L `06A1`/`0935`/`0AB3` |

## Seiteneffekte in Reihenfolge

- **Kein Zufall** (R5P-5), keine Uhr, keine Eingabe, keine Grafik, kein Ton. Nur Lesen/Schreiben im Datensegment.

## Neue DGROUP-Adressen

Siehe [../analyse/DATENLAYOUT.md](../analyse/DATENLAYOUT.md), Unterabschnitt „Spur anbau-ki (R5)": `40CE`/`40D6`
(`AiTargetX`/`AiTargetY`), `410A`/`4112` (`AiPickX`/`AiPickY`), `4654`/`472C`/`2F31` (`AiCandX`/`AiCandY`/`AiCandKind`,
Stride 54), `48EC` (`AiCandCount`), `3C90` (`AiCandIndex`), `4BBE` (`AiNoneFound`), `43F6`/`43F8` (`AiRefX`/`AiRefY`),
`40F6` (`Unknown40F6` → `AiWantKind`, Präzisierung), `08E7` (`KindWare2`).

## Vertrag für Spur 0 (Einbindung)

- `000E(x, y)` → Rückruf `IFieldOutput.ComputerTurn(Dgroup d, short x, short y)` (`Field/FieldCursor.cs`), aufgerufen von
  `0609:047B` vor der Aktionsauswertung. Port: `PlantingAi.ComputerTurn(d, x, y)`.
- `0935` → `PlantingAi.CommitCandidate(d)` an Stelle `0609:0961` im Modus-3-Handler `0609:07D6` (Feld kaufen). Zweig: Feld
  (`304E[p]`, `305A[p]`) nicht eigen, `0609:100F` = 1 (eigener Nachbar), Feld frei (`4442` = 0xFF), `43FA[p]` = 1. Dort in
  dieser Reihenfolge `0609:10D2(x, y, p, Art)`, Ton `21A7:0054(0, 0Ah)`, `0609:1CB1(x, y)`, `43FA[p]` = 0, `0609:1A97`, dann
  **nur bei `0840[p]` = 1** `0935`. Nicht aus `096A` (Modus 4); dort gibt es keinen Aufruf.
- `0AB3` → `PlantingAi.ChooseWantKind(d)` an Stelle `0609:0746`, am Ende des HQ-Baus `0609:069F` (Modus 2). Nur wenn
  `0609:074D` = 1 (HQ gebaut): nach `10D2(x, y, p, 4)`, `40FE[p]`/`4102[p]` = x/y, `0609:1C83(x, y)`, Ton
  `21A7:0054(0, 7)` und `43B2[p]` = 3, **nur bei `0840[p]` = 1**.
- Beleg der Aufrufstellen: Review R5A-1 (RefEmu-Probe Standardpartie, Rücksprungadressen `0609:0966` bzw. `0609:074B`, Modus
  beim Aufruf jeweils 3) und Bytesuche im gesamten `T.EXE` (`9A 35 09 5D 13` nur bei Dateioffset `0x9FF1` = `0609:0961`,
  `9A B3 0A 5D 13` nur bei `0x9DD6` = `0609:0746`; kein Nahaufruf in `135D`).
- Alle drei schreiben nur ins Datensegment, ziehen keinen Zufall, geben nichts zurück (bzw. `000E` setzt `3308[p]`).

## C#-Umsetzung

`src/Traders.Core/Ai/Planting.cs` (`PlantingAi`: `ComputerTurn` = `000E`, `CommitCandidate` = `0935`, `ChooseWantKind` = `0AB3`,
plus interne `Choose*`/`Score*`/`Action*`), `src/Traders.Core/Field/PlantingAiHelpers.cs` (`074D`, `100F`). Reine Logik auf
`Dgroup`, 16-Bit-Arithmetik über `Int16Math`, Byte-Zwischenwerte als `sbyte`/`byte` wie im Original.

## Prüfung

- `tests/Traders.Tests/Ai/PlantingDifferentialTests.cs` (RefEmu `LoadOnly`, direkte `Call(135D:*)`, ganzes DS inkl. `3308` und
  `10D2`): `000E` 4000 Zufallszustände mit Modus 0–7 (alle Zweige 2–5 und „sonst“, Bewegungsverzögerung 0/positiv, Cursor auch
  außerhalb), `0935` 3000 (Raketen-/Pflanzzweig, Merker `4580`/`4BC2`/`2EEA`, Güte 0–3), `0AB3` 2000 (HQ am Rand). Felder voll,
  leer und gemischt, Felder je Spieler `4BA8` 0–7 (beide Zweige von `06A1`), Stufen `2E8E` 0–3, Raketen 0/≠0, Vorrat gegen
  `48A2`, Zeitlimit 0/1 mit Restzeit auch negativ.
- `PlantingAiHelpersDifferentialTests.cs`: `0609:074D`/`100F` je 800 Fälle (AL und DS).
- `PlantingGameStateDifferentialTests.cs` (Teil 3): Standardpartie mit vier Computerspielern im RefEmu, Wochen 0–3; an jedem
  Einsprung `000E` (je Woche/Spieler/Modus die ersten 4), `0935`, `0AB3` (alle) DS und Argumente aufgezeichnet und danach
  Original und Port auf diesem Zustand verglichen. Erreicht: Modus 2 (Woche 0), 3 und 4 (jede Woche), 5 (Woche 2, Spieler 3),
  jeder Spieler in jeder Woche; `0935` jede Woche, `0AB3` Woche 0. Laufzeit ≈ 7 min (Emulation von vier Wochen).
- Eigene Gegenproben (lokal, nicht im Repo): `04F5` rechts/links vertauscht, Grundwert in `06A1` ·2 statt ·3, Gleichstand in
  `00AC` mit größerem statt kleinerem Abstand, Zeitprüfung in `02BD` mit 2 statt 1 → jeweils rot. Weitere Gegenproben im Review.
- Annahme (nicht nachgebildet): HQ-Koordinaten außerhalb des Rasters (Quadrant ∉ 0–3 in `00AC` würde lokale Variablen des
  Originals überschreiben) und Spalten/Zeilen = 0 (Division durch 0 im Original) kommen im Spiel nicht vor.
