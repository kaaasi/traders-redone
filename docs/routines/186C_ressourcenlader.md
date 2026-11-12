# 186C:0002 / 186C:01A9 / 186C:021E / 186C:02C6 / 186C:0997 / 186C:0AB1 / 186C:0B0A – Ressourcenlader

- **Ladebild-Adresse:** Segment `186C` (Dateioffset `0x1BCC0`); `0002` bei `0x1BCC2`.
- **Aufgabe:** eigener Heap, Dateien laden, Bilder (PP20) mit Zwischenspeicher entpacken und in den Atlas bringen.
- **Aufgerufen von / Phase:** Initialisierung `1777:04E4` (`021E`, `0544`), Programmstart `0DFD` (`02C6`, `0AB1`, `01A9`, `0002`),
  Code-Abfrage (`0002`), Szenen (`0002` je Bildwechsel).
- **Parameter / Rückgabe:** `0002(n)` Bild n; `02C6(long Größe)` → far Zeiger; `0AB1(far Ziel, far Name)` → long Länge
  (0 bei Fehler); `0B0A` wie `0AB1`, Datei schreibend geöffnet; `01A9(far Puffer, long Länge)` entpackt an Ort und Stelle.

## Ablauf (eigene Worte)
- `021E`: größter freier DOS-Block − 1000 Byte (Tandy weitere 64 KB) als eigener Heap; Grenzen `DS:5318…532F`.
- `02C6`: Zuteilung aus diesem Heap (Zeiger normalisiert, Grenzen `5320/5324/5328/532C`).
- `0002(n)`: Eintrag `DS:09CE + 13h·n`; Name VGA (+0) bzw. EGA (+4) nach `DS:090E`. Ist die entpackte Länge (+0Fh) 0, liest
  `0997` die Datei an: +0Bh = Dateilänge, +0Fh = drei Byte vier Byte vor dem Dateiende (big endian; PP20-Nachspann).
  Dann `DS:09C8` = n; Zwischenspeicher `056D/05AF` (Tabelle `DS:4C04`, 25 Einträge mit Heap-Zeigern) – Treffer: Kopie
  `0958`, sonst `0AB1` + `01A9` in den Atlas `DS:45D6`; VGA: Palette 300h Byte nach `DS:5017`, Rest an den Atlasanfang
  (`27C6:04CE`), `23CD:3079` (Atlas), Tandy `23CD:166A`; zuletzt `21AF:000D`.
- `0AB1`/`0B0A`: `open` (8001h bzw. 4001h), `0D73:059A` lesen bzw. schreiben, `close`; Rückgabe die Länge.
- `0A6C`/`0B63`: Datei suchen, fehlt sie: Diskette anfordern (Nummer +8) – im Port entfällt das (F2 = A).

## Zustand
| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `09C8` | W | s | zuletzt geladenes Bild (−1 am Anfang) | s |
| `09CE` | 13h·10 | – | Bildtabelle (Namen, Diskette, Längen) | g/s |
| `4C00…4D12` | – | – | Bildcache (Nummer, Länge, Heap-Zeiger) | g/s |
| `5318…532F` | – | – | Heap-Grenzen | g/s |
| `45D6`, `5017` | Far, 300h | – | Atlas, geladene Palette | g / s |

## Seiteneffekte in Reihenfolge
Dateien, Heap, Atlas, Palette; kein Zufall, keine Uhr.

## C#-Umsetzung
Kern: nur die Bildtabelle (`src/Traders.Core/Startup/ImageTable.cs`, `RecordLoad`); die Längen liefert der Host aus den
Dateien (`src/Traders.Assets/Loading/ImageFiles.cs`). `[APPROX]`: Heap, Cache `4C00` (Heap-Zeiger), Datei-I/O, Entpacken
(`Traders.Assets.PowerPacker`) und Atlas/Palette (`Traders.Vga`) sind Host bzw. Ausgabe; ihre Bytes sind im Differenztest
ausdrücklich ausgenommen ([0DFD_000D_programmstart.md](0DFD_000D_programmstart.md)).

## Prüfung
`ProgramStartDifferentialTests` (`[DIFFERENTIAL] 0DFD:000D`): `09C8…0A8B` nach dem Programmstart gleich dem RefEmu (sieben
Bilder: sechs vorgeladene, Abfragebild); Gegenprobe falsche Länge → rot `DS:09DD`. `StartupTests`: zweites Laden lässt die
Längen stehen; PP20-Nachspann.
