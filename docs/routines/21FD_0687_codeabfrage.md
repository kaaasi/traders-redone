# 21FD:0687 / 21FD:081D / 21FD:08D3 – Code-Abfrage „INSERT COIN!“

- **Ladebild-Adresse:** `21FD:0687` (Dateioffset `0x25C57`)
- **Aufgabe:** Kopierschutz-Abfrage beim Start: zeigt „INSERT COIN! CODE: X-nn“ und liest vier
  Ziffern (Tasten 1–9 bzw. Ziffernblock) ein. Läuft vor dem Hauptmenü.
- **Aufgerufen von / Phase:** Programmstart (nach dem Laden aller Bilder, ca. 12,2 s emulierte Zeit
  im Referenzlauf).
- **Parameter / Rückgabe:** keine.

## Ablauf
```
copy 4 bytes DS:10FB -> local[0..3]                (0000:5189 = struct copy)
random(0)                                           // Neusaat aus der Uhr (time())
186C:0002(word [08DA])                              // Bild laden/zeigen
21FD:081D                                           // 4 Zeichenketten bei DS:10E0 entschlüsseln (+0FDh je Byte)
x = random(0x3FF) & 0x3FC
row = x % 32, col = x / 32
for i in 0..3: digit[i] = 21FD:08D3(row + i, col)   // erwartete Ziffern: v = (col << 5) + row + i; v = v·v; v = v·v;
                                                    // v = v·7AB7h (32 Bit); digit = v / 100h % 9 + 1 (unsigned)
Anzeige: Buchstabe 'A' + row/4, Zahl col (ein- oder zweistellig) in den Text bei far [DS:10E8]
         (Zeichen +6, +8, +9; einstellig: +9 = Leerzeichen)
192D:16AF(50h, 1Eh, far DS:10E0, -3)                // Texttafel (Spur menue): zieht dabei viele random()-Werte
                                                    // (Mauer 131B/138F, Nieten 14B4, Tafel 14FB), setzt DS:5334
0A96:0C3F([DS:082E]); 21FD:026C; 21FD:02C9(9, 60h); 21FD:0329   // Seite kopieren, Palette, einblenden
for i in 0..3:
    warte auf Taste: [DS:1158] (letzter Make-Code aus der eigenen int-9-Routine) != 0,
    dann bis sie losgelassen ist (Tastentabelle DS:3D47)
    Scancode 2..10 → Ziffer 1..9, Ziffernblock 47h..51h über [SI + 10A9h] = DS:10F0..10FA
    21FD:0863(ziffer-1, i)                         // Kästchen zeichnen
    cmp ziffer, digit[i] ; jmp short weiter         // Ergebnis des Vergleichs wird nicht ausgewertet
```
Der Fehlerzweig (`21FD:07FC`: Ton 21A7:0054, Meldung 21FD:08AA, Endlosschleife) ist durch das
`jmp short` nach dem Vergleich nicht erreichbar: **jede Eingabe wird angenommen.**

## Zustand
| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `10D2` | 32 Bit | unsigned | Zufalls-Seed (über `random`) | r/w |
| `10E0` | 4×far | – | Zeiger auf 4 verschlüsselte Texte | r/w (Entschlüsselung) |
| `10E8` | far | – | Zeiger auf den Text „CODE: X-nn“ | w (Zeichen 6, 8, 9) |
| `10FB` | 4 Byte | – | Vorlage für lokale Ziffern | r |
| `1158` | 8 Bit | – | letzter Make-Scancode (int 9 des Spiels) | r/w |
| `3D47` | Tabelle | – | Tastenzustand je Scancode | r |
| `10F0…10FA` | 11 Byte | – | Ziffernblock-Scancode 47h…51h → Ziffer (7 8 9 – 4 5 6 + 1 2 3), adressiert als `[SI + 10A9h]` | r |

## Seiteneffekte in Reihenfolge
1. **Zufall:** `random(0)` – Seed = Borland `time()` (siehe REFEMU-Befund: lokale Zeit + 18000 − 3600 s),
   danach `random(0x3FF)` und die Ziehungen der Texttafel `192D:16AF` (im Referenzlauf zahlreiche, alle bei 12,44–12,46 s,
   keine je Bild während des Wartens). **Hier wird der Zufallsgenerator der ganzen Partie aus der Uhr gesetzt.**
2. Grafik: Bild, Text, vier Kästchen.
3. Eingabe: vier Tastendrücke.

**`DS:10A9` (geklärt, R4 Spur `programm` Teil 2):** Die frühere Angabe „Tabelle `DS:10A9`“ meinte nur die Basis der
Adressierung `[SI + 10A9h]`; gelesen werden ausschließlich `DS:10F0…10FA` (SI = 47h…51h). Das Wort `DS:10A9` selbst ist das
aktuelle Musikstück von `218B:0094` (FFFF aus der EXE, 0 nach `218B:0094(0, 0)` bei `0DFD:02FE`). Kein Widerspruch;
Beleg `CodeQueryDifferentialTests.Keypad_table_lies_behind_DS_10A9_and_does_not_touch_the_current_piece_word` (RefEmu-Probe:
Ziffernblock 7 8 9 4 → gelesene Adressen `10F0, 10F1, 10F2, 10F4`, Ziffern 7 8 9 4; `10A9` vor/nach `0DFD:02FE` FFFF/0).

## C#-Umsetzung
`src/Traders.Core/Startup/CodeQuery.cs` (Entscheidung F1 = B): `random(0)` aus dem Uhrwert des Hosts, Abfragebild in der
Bildtabelle (`ImageTable.RecordLoad`), `DecodeTexts` (081D), `random(3FFh)`, Code in den Text, Ziffern (`Digit` = 08D3), dann
die Texttafel über `ITextPlaque`, im Programm `MenuTextPlaque` (`DS:5334`, Mauer `192D:131B` und Tafel `14FB` aus
`MenuBackdrop` der Spur `menue`). `[APPROX]`: kein Bild, keine Tafelzeichnung (nur deren Ziehungen und Zeichenaufrufe), keine
Palette, keine Kästchen `0863`, kein Warten auf Tasten (`DS:1158`, `3D47`) – das Original nimmt jede Eingabe an.

## Prüfung
`[DIFFERENTIAL] 0DFD:000D` (`ProgramStartDifferentialTests`, ganzer Programmstart mit beliebiger Eingabe), `[DIFFERENTIAL]
21FD:08D3` (alle Zeilen/Spalten und Grenzwerte gegen direkten Aufruf), `[DIFFERENTIAL] 21FD:081D` (Datensegment nach zwei
Aufrufen). Referenzlauf `tests/Traders.Tests/RefEmu/BootTests.cs`: vier Tastendrücke „1“ ab 14 s → Hauptmenü bei
17,3 s (Bilder im Referenz-Repo: `refemu/refemu-codeabfrage.png`, `refemu-hauptmenue.png`).
Im Video 1 erscheint die Abfrage mit „CODE: A-21“ (anderer Uhrwert).
