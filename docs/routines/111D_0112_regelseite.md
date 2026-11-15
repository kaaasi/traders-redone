# 111D:0112, 23CD:06DE – Lesestellen der Sternvorführung und Regelseite des Kerns (R6P-1, Teil 1a)

- **Ladebild-Adresse:** `111D:0112` (Dateioffset `0x148E2`, Segment `111D` bei `0x147D0`); Pixel-Leser `23CD:06DE` → `23CD:3764`
- **Aufgabe:** Dieser Befund beschreibt nur, **was** `0112` vom Bildschirm liest und wodurch das Gelesene entsteht – die Grundlage der
  Regelseite `src/Traders.Core/Pixels/RulePage.cs`. Ablauf, Zufall und Bildfolge der Vorführung portiert Spur 1 Teil 1
  (Nachtrag in `111D_0009_katastrophen.md`).
- **Aufgerufen von / Phase:** `111D:00D9` (Katastrophen, Aufruf 4 der Wochenschleife). Weitere Leser von `23CD:06DE`:
  `1B03:08ED`, `0BE7`, `1178` (Spur 3, Ergänzungen in `Pixels/Stations*`).
- **Parameter / Rückgabe:** `23CD:06DE(x, y)` (Wörter, `x` als erstes Argument) liefert in AL den Farbindex des Pixels der
  **Zeichenseite**; `0112` vergleicht AX mit 1.

## Ablauf von 0112, soweit er Pixel liest oder schreibt (eigene Worte)

Die Zeichenbefehle der Grafikschicht `0A96` werden nur in die Befehlsliste gestellt und erst von `0A96:06A8` ausgeführt; `06A8`
leert die Liste nicht (`0A96:001B` leert sie).

1. `001B`; Blit Atlas → Seite `0A96:0030(0, 0, 320, 200, 0, 0)`; `06A8` – die Zeichenseite (Seite A) zeigt Zeilen 0–199 des
   RAM-Atlas, hier `V-BERGE` (geladen von `111D:00C9` mit `186C:0002([08D2])`).
2. `001B`; 90-mal: `x = random(13Fh)`, `y = random(96h)`, beide in einer Stapeltabelle gemerkt; **Lesen** `06DE(x, y)` auf Seite A;
   ist es 1: Pixel `0A96:03FB(x, y, random(2) + 8)` in die Liste. Gelesen wird also das unveränderte `V-BERGE` – die Sterne
   sind noch nicht gezeichnet.
3. `06A8` (Sterne auf Seite A), Seitenwechsel `21FD:051F` (Seite A wird gezeigt, gezeichnet wird auf Seite B).
4. `001B`; in die Liste: Blit `0030(0, 0, 320, 197, 0, 3)` (Bild um drei Zeilen nach unten) und Rechteck `0A96:04D7(0, 0, 320, 3, 1)`;
   dann 90-mal je gemerktem Stern: **Lesen** `06DE(x, y − 3)` auf Seite B; ist es 1: `03FB(x, y, random(2) + 8)`.
5. `06A8`: Seite B zeigt das verschobene Bild, oben drei Zeilen Farbe 1, und die neuen Sterne.

**Befund (belegt, Test unten):** In Schritt 4 sind Verschiebung und Rechteck beim Lesen **noch nicht ausgeführt**. Die zweite
Schleife liest deshalb Seite B in dem Zustand **vor** `0112`, nicht das verschobene Bild. Für `y < 3` ist `y − 3` negativ: der
Leser adressiert dann Offset `FF10h…FFFFh` des Seitensegments – auf Seite 0 (`A000`) ungenutzter VRAM, auf Seite 1 (`A400`)
Speicher hinter dem VGA-Fenster (`B3F10h…`). Die Zahl der Ziehungen `random(2)` in Schritt 4 hängt also vom Inhalt der vorher
gezeichneten Seite ab.

**Was vorher auf Seite B steht (beobachtet, nicht als Test eingecheckt):** Ein lokaler Lauf der Standardpartie bis Woche 3
(`2F30` = 1, `random(10)` in `0028` auf 0 gesetzt) zeigte: alle 90 Lesestellen der ersten Schleife gleich `V-BERGE`, die der
zweiten nicht (`V-BERGE` 5, `V-ANBAU` 12 von 90 gleich). Nach der Disassemblierung zeichnet `0009` vor `0112` mit `21FD:060D` auf
**beide** Seiten: Kacheln `0609:1ED9` (Atlas `V-ANBAU`), Rahmen `0609:1256(8Ch, 88h, A0h, 2Ch)` und den Meldetext
`0A69:00D3(8Ch, A0h, …)`. Der Text liegt ab Zeile A0h (160) und damit unter den Lesezeilen (`y − 3` ≤ 146); Kacheln und Rahmen
nicht. Diese Schritte muss Spur 1 Teil 1 vor `0112` auf die Regelseite zeichnen und im Differenztest der ganzen Routine belegen.

## Zustand

| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `082E` | W | u | Seitenindex, auf den gezeichnet wird (`RulePage.DrawPageIndex`) | gelesen (`DrawPageOf`) |
| `40DE` | D[2] | u | Seitennummern 1/2 je Index (`RulePage.PageNumbers`) | gelesen (`DrawPageOf`) |
| `15BE` | W | s | Zahl der Einträge der Befehlsliste | nur im Test geprüft |

## Seiteneffekte in Reihenfolge

Der Leser `23CD:3764` selbst: Read-Map-Register = `x & 3`, ein Lesezugriff, `23CD:30D1` (Grafik-Controller zurück); kein Zufall,
kein Zustand im Datensegment. Zufall, Bildfolge und Seitenwechsel gehören zu `0112` (Spur 1 Teil 1).

## C#-Umsetzung

`src/Traders.Core/Pixels/RulePage.cs` – zwei physische Seiten (0 = `A000:0000`, 1 = `A400:0000`) zu 320 × 200, Zeichenseite
`DrawPage`:

- `Get(x, y)` wie `23CD:3764` für jedes Wortpaar: Offset `(y·80 + x/4) mod 10000h` im Segment der Zeichenseite, Ebene `x & 3`;
  liegt die Adresse in Seite 0 oder 1 des Fensters, das Pixel dort (von Seite 0 aus adressiert `y` = 205…404 Seite 1), sonst 0
  (`[APPROX]`: RefEmu-Wert; die Leser vergleichen nur mit 1).
- `Blit` (`0A96:0030` → `23CD:32D8`), `FillRect` (`04D7` → `31A6`), `SetPixel` (`03FB` → `373B`) – angewandt dort, wo das Original
  `06A8` ausführt; nur Rechtecke innerhalb von Atlas und Seite (`h` = 0 und Adressumlauf nicht nachgebildet).
- `Flip()` (`21FD:051F`, solange `40DE` die Nummern 1 und 2 hält) und `DrawPageOf(Dgroup)` (Seitennummer `40DE[082E]` über
  `0B67:0238`).
- `src/Traders.Core/Pixels/IndexedImage.cs` – Bildpixel als explizite Eingabe (Host und Tests aus `Traders.Assets`,
  `Func<int, IndexedImage>` je Bildnummer wie `Func<int, ImageFileLengths>`); keine Projektreferenz.

## Prüfung

`tests/Traders.Tests/Pixels/RulePageDifferentialTests.cs` (Original aus dem Abzug am Hauptmenü, Grafik eingerichtet):

- `Star_show_reads_the_rule_page_like_the_original` – `[DIFFERENTIAL] 23CD:06DE` an den Lesestellen von `0112`: `V-BERGE` im RAM-Atlas, zweite Seite mit
  `V-ANBAU` bzw. `V-BERGE` vorbelegt, beide Paritäten der Zeichenseite, vier Zufallszustände `DS:10D2`. Die Zeichenschritte des
  Originals werden zur Laufzeit auf die Regelseite übertragen (Listeneinträge bei `06A8`, Wechsel bei `051F`); an jeder der 180
  Lesestellen gleich dem Rückgabewert des Originals, danach beide Seiten vollständig gleich dem VRAM und `DrawPageOf` gleich der
  Zeichenseite. Abgedeckt (Ausgabe): Treffer auf 1 in beiden Schleifen, negative `y` auf beiden Seiten (je 22).
- `Pixel_reader_matches_for_every_word_argument` – `[DIFFERENTIAL] 23CD:3764`: `06DE` direkt gerufen, beide Seiten, Ecken, negative
  `x`/`y`, `y` ≥ 200 (Lücke hinter Seite 0, Seite 1 von Seite 0 aus), `x` ≥ 320.
- `Drawing_steps_match_the_command_list` – `[DIFFERENTIAL] 0A96:0030, 0A96:04D7, 0A96:03FB`: unausgerichtete Blits (verschiedene
  `x mod 4`, Breiten ≠ 4n, Atlaszeilen ≥ 200), Rechtecke, Pixel an den Rändern; danach beide Seiten gleich dem VRAM.
- Gegenproben (lokal ausgeführt, rot): ohne `Flip` beim Wechsel; `Get` außerhalb der Seiten 1 statt 0; Blit mit vertauschten
  Nachbarspalten.
