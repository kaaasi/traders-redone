# Ressourcenformate (DOS-Fassung)

Alle `*.PP` sind PowerPacker **PP20** (Effizienzbytes `09 0A 0C 0D` bzw. `09 0A 0B 0B`), entpackt
mit `Traders.Assets.PowerPacker` (Referenz: `tools/pp20.py`).

## V-*.PP – VGA-Bilder (82692 Byte entpackt)

| Offset | Länge | Inhalt |
|---:|---:|---|
| 0 | 768 | Palette, 256 × RGB, 6 Bit je Kanal (0–63) |
| 768 | 81920 | Atlas 320 × 256, 1 Byte Farbindex je Pixel, zeilenweise |
| 82688 | 4 | unbekannt (z. B. `00000000`, `63636366`, `FFFFFFFA`) |

Zeilen 0–199 entsprechen dem sichtbaren Bildschirm, Zeilen 200–255 enthalten Sprites und
Bausteine. Zuordnung zur Ressourcenliste bei `DS:09CE` siehe `ResourceTable`.

## E-*.PP – EGA-Bilder (40964 Byte entpackt)
Planarformat, noch nicht dekodiert (nur zur Gegenprüfung relevant).

## FONTS.PP – Schriften (2150 Byte entpackt)

| Offset | Schrift | Glyphen | Raster | Bytes/Glyphe |
|---:|---|---|---|---:|
| 0 | groß | ASCII 32–91 (60) | 16 × 12 | 24 |
| 1440 | klein | ASCII 32–102 (71) | 16 × 5 | 10 |

Jede Zeile = 2 Byte, höchstwertiges Bit = linkes Pixel. Leere Glyphen für nicht genutzte
Zeichen (z. B. `%`, `&`). In der kleinen Schrift enthalten die Codes 93–102 einen zweiten
Ziffernsatz 0–9. Zeichenbreiten/Abstände stehen nicht in der Datei → aus den Textroutinen
der EXE ermitteln.

## MASKS.PP – Masken (8000 Byte entpackt)
1-Bit-Atlas 320 × 200 (40 Byte je Zeile, MSB links). Transparenzmasken für den maskierten
Blit `0A96:013B`. Die Zuordnung Farbatlas-Rechteck ↔ Maskenrechteck ergibt sich aus dem Code.

## ENGLISCH.TXT – Texte
303 CRLF-Zeilen, angesprochen über Zeilenindex. Danach ohne Zeilenende der
Entwicklermarker `--- max 303 Lines ---`. Enthält mindestens ein Byte 0xA7.

## *.CMF – Musik im Format „A.H.“ (kein Creative CMF)

| Offset | Inhalt |
|---:|---|
| 0x00 | Kennung `A.H.` |
| 0x04 | Reihenfolge-Liste, 99 Wörter (Big-Endian), Ende = 99 (`0x63`) |
| 0xCA | Anzahl Patterns (= höchste Patternnummer + 1) |
| 0xCC | Instrumente und Patterns – Aufbau offen (M14) |

Abgespielt von `FXDRIVER.EXE` (Linel 1991, resident). Das Spiel steuert ihn über
**`int 64h`** aus Modul `218B` (und `21A7`); Funktionsnummer in AX. Beobachtete Funktionen:
`0x01`, `0x03`, `0x05`, `0x06`, `0x10` (mit Far-Pointer CX:DX), `0x12`, `0x18`, `0x19`,
`0x1A`, `0x1B`. Bedeutung wird in M14 aus dem Treiber ermittelt.
