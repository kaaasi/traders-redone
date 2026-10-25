# 21FD:0687 – Code-Abfrage „INSERT COIN!“

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
for i in 0..3: digit[i] = 21FD:08D3(row + i, col)   // erwartete Ziffern aus einer Formel (LXMUL)
Anzeige: Buchstabe 'A' + row/4, Zahl col (ein- oder zweistellig) in den Text bei far [DS:10E8]
for i in 0..3:
    warte auf Taste: [DS:1158] (letzter Make-Code aus der eigenen int-9-Routine) != 0,
    dann bis sie losgelassen ist (Tastentabelle DS:3D47)
    Scancode 2..10 → Ziffer 1..9, Ziffernblock 47h..51h über Tabelle DS:10A9
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
| `10A9` | Tabelle | – | Ziffernblock-Scancode → Ziffer | r |

## Seiteneffekte in Reihenfolge
1. **Zufall:** `random(0)` – Seed = Borland `time()` (siehe REFEMU-Befund: lokale Zeit + 18000 − 3600 s),
   danach `random(0x3FF)`. **Hier wird der Zufallsgenerator der ganzen Partie aus der Uhr gesetzt.**
2. Grafik: Bild, Text, vier Kästchen.
3. Eingabe: vier Tastendrücke.

## C#-Umsetzung
Noch nicht portiert (gehört zu M4 Menü & Setup). Für den Port ist wichtig: Uhrwert als Eingabe
aufzeichnen, `random` genau zweimal (0 und 0x3FF) aufrufen.

## Prüfung
Referenzlauf `tests/Traders.Tests/RefEmu/BootTests.cs`: vier Tastendrücke „1“ ab 14 s → Hauptmenü bei
17,3 s (Bilder `tests/Traders.Tests/RefEmu/Data/refemu-codeabfrage.png`, `refemu-hauptmenue.png`).
Im Video 1 erscheint die Abfrage mit „CODE: A-21“ (anderer Uhrwert).
