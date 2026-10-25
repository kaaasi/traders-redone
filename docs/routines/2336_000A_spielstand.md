# 2336:000A / 2336:0539 – Spielstand speichern / laden

- **Ladebild-Adresse:** `2336:000A` (speichern), `2336:0539` (laden), Hilfen `2336:051E`
  (Dword in Puffer schreiben), `2336:0947` (Dword aus Puffer lesen); Segment `2336` beginnt bei
  Dateioffset `0x26960`.
- **Aufgabe:** gesamten persistenten Spielzustand nach `SAVEGAME.DAT` schreiben bzw. daraus lesen.
- **Aufgerufen von / Phase:** Diskettenmenü `192D:0FB7` → `192D:11E1` (S) bzw. `192D:1188` (L).
- **Parameter / Rückgabe:** keine / `al` = 1 bei Erfolg (genau 0x900 Byte geschrieben/gelesen).

## Zustand
Alle 52 Speicherstellen mit Breite, Vorzeichen und Wiederholung: Tabelle in
[DATENLAYOUT.md §2](../analyse/DATENLAYOUT.md#2-spielstand-savegamedat).
Zusätzlich: Pufferzeiger `DS:45D6` (far, gelesen), Schreib-/Lesezeiger `DS:57F8` (far, r/w),
Ressourcen-Nummer `DS:08DA` (Bildschirm neu laden danach).

## Ablauf
```
ptr = buffer;                                   // DS:57F8 = DS:45D6
for (player = 0; player < 4; player++)          // global DS:37AA
    put(HqX[p]) … put(Robot3016[p]);            // 25 Stellen, darin Schleifen über DS:45D0 und si
for (DS:45D0 = 0; DS:45D0 <= 3; ) put(WarePrice[i]);
for (DS:2F00 = 0; <= 5; ) for (DS:2EB0 = 0; <= 8; ) put(owner, kind, quality of field[x*6+y]);
put(columns); put(rows); for (si = 0; si <= 11; si++) put(station flag, kind, value);
put(pirates) … put(GemPriceFactor);             // 18 Einzelwerte
fd = open("SAVEGAME.DAT", O_WRONLY|O_CREAT|O_TRUNC|O_BINARY, S_IWRITE); write(fd, buffer, 0x900);
```
`put(v)` legt `v` als 32-Bit-Wert ab (Byte per `cbw` bzw. `mov ah,0`, Wort per `cwd` bzw.
`xor dx,dx` erweitert). Laden liest die Datei über `186C:0AB1`, bricht bei Länge ≠ 0x900 ab und
schreibt die Werte in derselben Reihenfolge abgeschnitten zurück; danach `192D:0058`
(Spielfeldursprung `DS:451E/4528` aus Spalten/Zeilen).

## Seiteneffekte in Reihenfolge
- Kein Zufall, keine Uhr. Datei-E/A über Borland `open` (`0000:3FED`) / Leseroutine `186C:0AB1`.
- Globale Schleifenvariablen bleiben stehen: `DS:37AA` = 4, `DS:45D0` = 4, `DS:2EB0` = 9, `DS:2F00` = 6.
- Grafik: Bildschirmressource `DS:08DA` neu laden (`186C:0002`).
- Nicht gespeichert: Zufallsseed `DS:10D2`, `LandType`, `Sound`, abgeleitete Preise.

## C#-Umsetzung
`src/Traders.Core/State/SaveGame.cs` (Dword-Format, Reihenfolge, Abschneiden, Schleifenvariablen),
Zustand ↔ DS-Abbild `GameStateMapper.cs`.

## Prüfung
- `tests/Traders.Tests/State/StateTests.cs`: Reihenfolge gegen die per
  `tools/datenlayout/savegame.py` unabhängig aus der Disassemblierung gelesene Liste,
  522 Dwords, Erweiterung/Abschneiden, Rundreise, Längenprüfung.
- **Offen:** Differenztest gegen das ausgeführte Original (RefEmu, AP1): DS-Abbild vorbereiten,
  `2336:000A` ausführen, Puffer bei `DS:45D6` mit `SaveGame.Write` vergleichen.
