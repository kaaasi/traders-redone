# 0A96:000A / 0A96:001B / 0A96:06A8 / 0A96:0030 / 0A96:0089 / 0A96:00E2 / 0A96:013B / 0A96:01AA / 0A96:020E / 0A96:0267 / 0A96:02D6 / 0A96:0311 / 0A96:035F / 0A96:03AD / 0A96:03FB / 0A96:0433 / 0A96:0481 / 0A96:04D7 / 0A96:0525 / 0A96:057A / 0A96:05FB / 0A96:0686 / 0A96:0C68 / 0A96:0C3F / 0A96:07E4 / 0A96:0884 / 0A96:0A51 – Befehlsliste

- **Ladebild-Adresse:** Segment `0A96` (Dateioffset `0xDF60`); `06A8` bei `0xE608`.
- **Aufgabe:** Zeichenbefehle sammeln (je Typ eine Einreihroutine) und gesammelt über die Treiber-Sprungleiste `23CD` ausführen.
- **Aufgerufen von:** alle Szenen; Bildschleifen `21FD:0004/060D/0634/0665` führen die Liste aus (`060D` zweimal: vor und nach `051F`).
- **Parameter / Rückgabe:** Felder je Typ siehe `docs/analyse/GRAFIK.md` §2 (Tabelle); keine Rückgabe.

## Ablauf (eigene Worte)
- Eintrag 1Ch Byte am Far-Zeiger `DS:15BA/15BC`, Typ als Wort bei +1Ah; nach dem Einreihen Zähler `DS:15BE` + 1, Zeiger + 1Ch.
  Feldbelegung: +0/+2 Quelle bzw. Punkt, +4/+6 Ziel bzw. zweiter Punkt, +8/+0A Maske bzw. Fläche (Typ 6) bzw. Schatten (+8),
  +0C/+0E Breite/Höhe, +10 Farbe, +12/+14 Textzeiger bzw. Zahlentext.
- `001B`: Zähler 0, Zeiger `DS:15C0`. `000A(a, b)`: Bytes `DS:0142/0143`.
- `06A8`: Anzahl merken, Zeiger und Zähler auf den Anfang, je Eintrag `call far [01F4 + 4·Typ]` (17 Zeiger, alle in `0A96`),
  danach Zeiger + 1Ch, Zähler + 1. **Die Liste wird nicht geleert**: danach stehen Zähler und Zeiger wieder hinter dem letzten
  Eintrag; ein zweiter Aufruf zeichnet dieselben Einträge erneut (so bedient `21FD:060D` beide Seiten).
- Ausführende (nicht exportiert): `06E4/0720/075C/0848` Blits, `0798` maskiert, `07E4` Maske füllen, `0884` Kacheln (Zeilen und
  Spalten mit vorzeichenbehafteten Vergleichen, letzte Kachel auf den Rest gekürzt), `094F/0987/09BF` feste Breite 1/2/4·8,
  `09F7` Linie, `0A51` Rahmen (oben, rechts, unten, links), `0B20` Rechteck, `0B7A`/`0BB2` Text/Zahl über `0C68`, `0BE9` Muster,
  `0BFB` Pixel. Farben bei `DS:090E` ≠ 1 über `DS:0154 + 16·[09C8] + (Farbe ∧ 0Fh)`.
- `057A`/`05FB` (Zahl, `$`-Betrag): Text entsteht in der Liste selbst ab +12h (+13h nach `$`) über `2295:0001`, Ende nach
  „Stellen“ Zeichen. Mehr als 7 (bzw. 6) Stellen überschreiben das Typwort +1Ah – beim Ausführen springt das Original dann ins Leere.
- `0C68(x, y, far Text, Farbe, Schatten)`: Nullzeiger → nichts. Schatten ≥ 0: Rechteck (x, y − [0143] + 1, strlen·[0142], [0143],
  Bytes vorzeichenbehaftet) in der Schattenfarbe, dann Text über `23CD:05DD`. Negative Bytes ergeben Rechtecke mit fast 64 K Zeilen.
- `0C3F(i)`: `0B67:0299` mit dem unteren Wort von `DS:40DE[i]`; auf Tandy-Karten zuvor `21FD:051F`.

## Zustand
| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `15BA` | Far | – | nächster Eintrag | g/s |
| `15BE` | W | s | Anzahl | g/s |
| `15C0` | 1Ch·n | – | Einträge | g/s |
| `01F4` | Far[17] | – | Ausführende | g |
| `0142`/`0143` | B | s | Schattenbreite je Zeichen / Schattenhöhe | g/s |
| `0154`, `09C8` | B[]/W | s | EGA-Farbtabelle, Palettennummer | g |

## Seiteneffekte in Reihenfolge
Nur DS beim Einreihen; beim Ausführen VRAM, RAM-Atlas (Typ 1), Treiberzustand (Farbe, Muster). Kein Zufall, keine Uhr.

## C#-Umsetzung
`src/Traders.Vga/Layers/CommandList.cs` (`GraphicsLayers.Add…`, `ClearList`, `ExecuteList`, `DrawText`, `CopyGamePage`). Die
Einträge liegen wie im Original im DS-Abbild; Far-Zeiger außerhalb von DS über `IFarMemory`.

## Prüfung
`tests/Traders.Tests/Vga/CommandListTests.cs`: `List_with_all_types_matches_original` (Modus 1 und 0, sechs Runden mit 8–30
zufälligen Einträgen, in Runde 1 alle 17 Typen und `02D6`; DS nach jedem Einreihen, Bild nach zwei Ausführungen),
`Text_shadow_sizes_match_original`, `Frames_with_line_pattern_match_original`, `CopyGamePage_0A96_0C3F_matches_original`.
