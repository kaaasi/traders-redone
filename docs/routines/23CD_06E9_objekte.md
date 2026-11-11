# 23CD:06E9 / 23CD:071C / 23CD:3AAB / 23CD:3C42 – bewegte Objekte zeichnen und Hintergrund zurückschreiben (VGA)

- **Ladebild-Adresse:** Schleifen `06E9` (zeichnen) und `071C` (zurückschreiben), kartenunabhängig; je Objekt Vektor 24
  (`3AAB`) bzw. 28 (`3C42`). Die Vektorroutinen nutzen den Stapelrahmen der Schleife (Seitenargument und lokale Variablen
  über `bp`) und sind nur über `06E9`/`071C` aufrufbar.
- **Aufgabe:** bis zu acht 16 Pixel breite Objekte maskiert auf eine Seite zeichnen und vorher deren Hintergrund sichern;
  später den Hintergrund zurückschreiben.
- **Aufgerufen von:** `06E9` an 23, `071C` an 25 Stellen in elf Segmenten (`0535`, `0609`, `08CF`, `0EDE`, `1B03`, `1C68`, `1DEF`, `1F63`, `20B3`, `21FD`, `22A3`).
- **Parameter / Rückgabe:** `06E9(Seite)`, `071C(Seite)` mit Seite als Index in die Tabellen je Objekt; keine Rückgabe.

## Zustand
| DS-Offset | Breite | Bedeutung | gelesen/geschrieben |
|---|---|---|---|
| `330C + 1Ah·i`, i = 0…7 | 1Ah | Objektsatz (Belegung in `GRAFIK.md` §4) | r |
| `+00` | Byte | aktiv (≠ 0) | r (`06E9`) |
| `+02` | Wort | Höhe | r |
| `+04` / `+08` | far | Tabelle der Bild- bzw. Maskenzeiger je Bild-Nr. | r |
| `+0C` / `+10` | far | Tabelle der Sicherungspuffer bzw. Rückschreibsätze je Seite | r; Satz w |
| `+14` / `+16` / `+18` | Wort | x, y, Bild-Nr. | r |

Rückschreibsatz (4 Byte): Seitenoffset, `x & 3`, gültig.

## Ablauf (eigene Worte)
- `06E9`: Objekte 0 bis 7, nur aktive; danach `30D1`. `071C`: Objekte 7 bis 0 **ohne** Prüfung auf aktiv; danach `30D1`.
- `3AAB`: Rückschreibsatz der Seite = (y·80 + x/4, x & 3, 1). Hintergrund: vier Durchgänge, Leseebene ab `x & 3` (Read Map),
  Sicherungsebene ab 0; je Zeile 4 Byte, Puffer im Atlasformat (320 Byte je Zeile, Ebene bei ·80). Dann Bild: vier
  Durchgänge, Zielebene ab `x & 3` (Map Mask), Quellebene ab 0; Maskenwort der Zeile (big-endian, 40 Byte je Zeile) um die
  Durchgangsnummer nach links geschoben, Bits 15, 11, 7, 3 je Zielbyte. Ende Map Mask 0Fh.
- `3C42`: liest Offset und das **Wort** aus `x & 3` und gültig, setzt dieses Wort auf 0 und bricht nur ab, wenn es 0 war
  (ein Satz mit gültig = 0, aber `x & 3` ≠ 0 wird also zurückgeschrieben). Rückschreiben auf die **aktuelle** Zeichenseite
  (`CS:0002`), vier Durchgänge wie beim Sichern umgekehrt. Map Mask bleibt auf der letzten Ebene stehen.
- Höhe 0 bedeutet 65536 Zeilen (Zähler läuft bis 0).

## Seiteneffekte in Reihenfolge
Rückschreibsätze und Sicherungspuffer (über far Zeiger), Map Mask, Read Map, Video-RAM. Kein Zufall, keine Uhr.

## C#-Umsetzung
`src/Traders.Vga/ModeXObjects.cs`: `DrawObjects`, `RestoreObjects`, `DrawObject`, `RestoreObject`; Objektsatz
`MovingObject`, far Zeiger `FarBuffer` (16-Bit-Offsetrechnung), `RestoreRecord`. Die Objektsätze selbst gehören später in
den Kern (`21AF`, R3); hier sind sie Eingabe.

## Prüfung
`DriverLineAndObjectTests.Moving_objects_06E9_071C_match_original`: acht Objekte (Höhe 1–24, vier Bilder aus dem RAM-Atlas,
Masken aus dem Maskenatlas, Sicherungspuffer und Rückschreibsätze mit Zufallsinhalt, auch gültig = 0 mit `x & 3` ≠ 0);
30 Runden mit neuer Lage, Bild-Nr. und Aktivität, zufällig `071C` vor `06E9`, Seite und Seitenindex zufällig. Nach jedem
Aufruf VRAM, Register, alle Sicherungspuffer und Rückschreibsätze gleich. Gegenproben (lokal): Abbruch nur nach gültig,
Reihenfolge 0…7 beim Zurückschreiben, Leseebene ab 0, Maskenbit 3 → 2, Map Mask 0Fh nach `3C42` → jeweils rot.
