# Datenlayout DGROUP (T.EXE)

Fassung: `T.EXE` SHA-256 `b73a2170…af0185`. DGROUP = Ladesegment + `0x28DC`, Größe bis zum
Stacksegment (SS = `0x2E66`) `0x58A0` Byte. Initialisierte Daten reichen bis etwa `DS:1570`,
danach BSS (beim Start null). C#-Gegenstück: `src/Traders.Core/State/` (`DgroupLayout`,
`GameState`, `GameStateMapper`, `SaveGame`, `Dgroup`).

**Belegstufen:** **G** = gesichert (aus Code + Texten/Menü eindeutig), **W** = wahrscheinlich
(Verwendung passt, Einzelheiten offen), **?** = Bedeutung offen (Struktur/Breite gesichert).

## 1. Werkzeuge und Quellen

| Quelle | Inhalt |
|---|---|
| `tools/datenlayout/dsrefs.py` | Sammelt alle DS-Zugriffe aus der Disassemblierung (`*.asm`, `tools/claude/fetch-reference.sh disasm` bzw. `tools/segmap.py`) (9296 Zugriffe): Offset, Breite, Lesen/Schreiben, Indexregister, Funktion. `python3 -I tools/datenlayout/dsrefs.py ~/traders-referenz/analyse/disasm refs.json refs.md` |
| `tools/datenlayout/savegame.py` | Liest die Feldreihenfolge aus der Speicherroutine `2336:000A` → `tests/Traders.Tests/State/SaveGameSites.txt` (Test vergleicht mit der C#-Beschreibung) |
| `2336:000A` / `2336:0539` | Spielstand speichern / laden – **vollständige Liste des persistenten Zustands** |
| `0E31:0003` (+ `0149`, `01CD`, `023D`) | Neue Partie: Startwerte aller Spieler- und Weltfelder |
| `192D:00C5…1B90` | Haupt-, Schwierigkeits-, Einstellungs-, Diskettenmenü (Bedeutung der Optionen) |
| `ENGLISCH.TXT` | Texte; Zeile *k* liegt als Far-Pointer bei `DS:37D0 + 4·k` (siehe §6) |

## 2. Spielstand `SAVEGAME.DAT`

- Speichern `2336:000A` (Aufruf aus Diskettenmenü `192D:11E1`), Laden `2336:0539` (`192D:1188`).
- Puffer: Far-Pointer `DS:45D6`, Schreibzeiger `DS:57F8`. Datei immer **0x900 Byte**; Laden
  verlangt genau 0x900 Byte, sonst „LOADING ERROR!“ und keine Änderung.
- Jeder Wert wird über `2336:051E` als **32-Bit-Dword** abgelegt: Byte mit `cbw` (vorzeichen-
  behaftet) oder `mov ah,0` (vorzeichenlos), Wort mit `cwd` oder `xor dx,dx`. Laden schneidet auf
  die Feldbreite ab. 522 Dwords = 0x828 Byte; der Rest des Puffers wird nicht beschrieben (im
  Original Inhalt des Speicherblocks, im Port Nullen).
- Die Schleifen benutzen **globale** Zählvariablen: danach stehen `DS:37AA` = 4, `DS:45D0` = 4,
  `DS:2EB0` = 9, `DS:2F00` = 6 (im Port nachgebildet).
- **Nicht** gespeichert: Zufallsseed `DS:10D2`, Landart `DS:08F1`, Ton `DS:10A7`, alle
  abgeleiteten Preise (§5). Nach dem Laden wird nur `192D:0058` (Spielfeldursprung) neu berechnet.
- Reihenfolge (Spalte „#“): Spielerblock je Spieler 0–3, darin die Einträge 1–25; danach 26–52.

| # | DS | Breite | Vz | Wiederholung | Name (C#) | Start (0E31) | Bedeutung | Beleg |
|---:|---|---|---|---|---|---|---|---|
| 1 | `40FE` | B | s | [p] | `HqX` | −1 | HQ-Spalte; −1 = noch kein HQ | G – `0609:06F5` setzt beim HQ-Bau Cursor-x |
| 2 | `4102` | B | s | [p] | `HqY` | – | HQ-Zeile | G |
| 3 | `4BAC` | W | s | [p] | `Rockets` | 5 | Raketen | G – Laden `08CF:09F1` (Preis `48A0`, max 10) |
| 4 | `4520` | W | s | [p] | `Oil` | 40 | Öl (40 = voll) | G – Ölkauf `08CF:04BA` bis 40 |
| 5 | `300E` | W | s | [p] | `GemCredit` | 50 | Edelstein-Handelskonto in Einheiten `BaseUnit/100`, max 2000 | W – `08CF:0813/084F/0886`, Vermögen `0825:08EB` |
| 6 | `37AF` | B | s | [p] | `Gems` | 2 | Edelsteine (max 10) | G – `08CF:178D` |
| 7 | `3CD2` | D | s | [p] | `Cash` | `InitialCash` | Bargeld | G – 16 Schreibstellen (Laden, Ereignisse, Rakete, Fat Mike) |
| 8 | `450E` | W | s | [p] | `Station` | −1 | gewählte Station 0–11, −1 = keine | G – Modul 1B03 |
| 9 | `4BC2` | B | u | [p] | `FatMikeFlag` | 0 | Merker aus Fat Mikes Mietrunde | W – `1F63:0711` |
| 10 | `40F6` | W | s | [p] | `Unknown40F6` | 0,2,1,0 | ? (Module 11A3/135D) | ? |
| 11 | `0840` | B | u | [p] | `IsComputer` | init. Daten 1,1,1,1 | 0 = Mensch, 1 = Computer | G – Hauptmenü `192D:0376`, Text HUMAN/COMPUTER |
| 12 | `083C` | B | s | [p] | `ControlDevice` | init. Daten 4,3,0,1 | Steuerung 0 COMPUTER, 1 JOYSTICK 1, 2 JOYSTICK 2, 3 MOUSE, 4 LEFT KEYS, 5 RIGHT KEYS | G – `192D:1ACE` (0..5), Texte 283–288 |
| 13 | `4BBE` | B | u | [p] | `Unknown4BBE` | – | ? (Modul 135D) | ? |
| 14 | `4BA8` | B | u | [p] | `FieldCount` | 0 | Anzahl eigener Felder | G – `0609:10D2`, Landvermögen `0825:086F` |
| 15 | `2EE7` | B | s | [p·4+k], k 0–3 | `FieldsByKind` | 0 | Felder je Art 0–3 | G – `0609:10D2` (Feldart ändern) |
| 16 | `3CF4` | W | s | [p·8+k·2], k 0–3 | `Stock` | 70 | Vorrat je Ware, max 999 | G – Wetter/Produktion 22A3, Vermögen `0825:088D` |
| 17 | `411C` | D | s | [p·0xA0+w·4], w 0–39 | `WealthHistory` | – | Gesamtvermögen je Woche (Ring über `Week mod 40`) | G – `0825:07C4` |
| 18 | `43CE` | B | u | [p·6+i], i 0–5 | `RobotParts` | 0 | Roboter-Kampf (Teile/Ausstattung) | W – Module 0EDE/1C68 |
| 19 | `376C` | W | u | [p·4] | `Robot376C` | 0 | Roboter-Kampf | W – 1C68:03C8 setzt 19–25 gemeinsam |
| 20 | `376E` | W | u | [p·4] | `Robot376E` | 0 | Roboter-Kampf | W |
| 21 | `2EF8` | W | u | [p·2] | `Robot2EF8` | 0 | Roboter-Kampf | W |
| 22 | `377C` | B | u | [p] | `Robot377C` | 1 | Roboter-Kampf | W |
| 23 | `40EC` | W | u | [p·2] | `Robot40EC` | 50 | Roboter-Kampf (Kampfwert, Modul 0535) | W |
| 24 | `33DC` | W | u | [p·2] | `Robot33DC` | 30 | Roboter-Kampf (Modul 0535) | W |
| 25 | `3016` | W | u | [p·2] | `Robot3016` | 0 | Roboter-Kampf (Modul 0535) | W |
| 26 | `4898` | W | s | [k·2], k 0–3 | `WarePrices` | 3 | Preis je Ware (Vermögen = Vorrat/10 · Preis) | G – `0825:088D`; gesetzt im Handel (0B9F) |
| 27 | `4442` | B | s | Feld [x·6+y] | `FieldOwner` | −1 | Besitzer | G – 22A3 Produktion |
| 28 | `2EB1` | B | s | Feld | `FieldKind` | −2 | −2 leer, 0–2 Produktionsart, 3 Rakete, 4 HQ | G/W – HQ = 4 (`0609:06DB`), 3 zählt als Rakete (`0825:096D`) |
| 29 | `45DA` | B | s | Feld | `FieldQuality` | – | Bodengüte (Index in Tabelle `DS:0902`) | W – 22A3 Produktion |
| 30 | `08FC` | W | s | – | `FieldColumns` | init. 8 | Spielfeldspalten 4–9 | G – Einstellungen „CURSORS: PLAYFIELD“ (`192D:0D10`, Kopie `5330`) |
| 31 | `08FE` | W | s | – | `FieldRows` | init. 6 | Spielfeldzeilen 3–6 | G (`192D:0D42`, Kopie `5332`) |
| 32 | `2F1A` | B | u | [i], i 0–11 | `StationVisited` | 0 | Station angeflogen/markiert | W – `22A3:010F` |
| 33 | `44F4` | W | s | [i·2] | `StationKind` | Zufall | Stationsart 0–2 (6×0, 4×1, 2×2) | G – Verteilung `0E31:023D` |
| 34 | `348A` | W | s | [i·2] | `StationValue` | Zufall | −1 (½), 10 (¼) oder 0–3; Verteilung belegt s. §3.1 | G (Erzeugung), Bedeutung W |
| 35 | `0900` | W | s | – | `PiratePresence` | init. 1 | 0 NONE … 3 HIGH | G – `192D:0742/0935` |
| 36 | `40E6` | B | s | – | `Weather` | Zufall | Wetter 0–5 (`random(2)`, +3 bei `random(4)==0`) | G – `21FD:021F`, Texte 245–262 |
| 37 | `2F05` | B | s | – | `FieldTotal` | Spalten·Zeilen | Feldanzahl | G – `0E31:01D5` |
| 38 | `4B9A` | W | s | – | `Week` | 0 | Wochen-/Rundenzähler | G – `++` in 1777/20B3, Index Vermögensverlauf |
| 39 | `411A` | B | u | – | `RobotBattle` | BSS 0 | ROBOT BATTLE an/aus | G |
| 40 | `45BA` | B | u | – | `OilConsumption` | BSS 0 | OIL CONSUMPTION an/aus | G |
| 41 | `08F2` | W | s | – | `InitialCash` | init. 500 | 500–2000 Schritt 500 | G – `192D:0C08` |
| 42 | `08F4` | D | s | – | `Target` | init. 20000 | Zielvermögen 10000–50000 | G – `192D:0C34` |
| 43 | `08F9` | B | s | – | `LottoReturns` | init. 1 | 0 LOW, 1 NORM, 2 HIGH | G |
| 44 | `08F8` | B | u | – | `TimeLimit` | init. 10 | 10/20/30 | G |
| 45 | `4580` | B | s | – | `Unknown4580` | −1 | ? (gesetzt in `11A3:0180`) | ? |
| 46 | `08EA` | W | s | – | `Level` | init. 2 | 0–3, −1 = benutzerdefiniert | G – `192D:05DD` |
| 47 | `2E8E` | W | s | – | `ComputerIq` | BSS 0 | 0 DIM, 1 GOOD, 2 SHREWD, 3 MEAN (global!) | G – `192D:0698`, Texte 136–139 |
| 48 | `2F30` | B | u | – | `Catastrophes` | BSS 0 | an/aus | G |
| 49 | `4BF2` | B | u | – | `GemTrading` | BSS 0 | an/aus | G |
| 50 | `08EC` | B | s | – | `RobotCycle` | init. 1 | 1–4 | G – `192D:0CE7` |
| 51 | `3CF0` | D | s | – | `BaseUnit` | `InitialCash/40` | Grundeinheit für Edelsteinwerte | G – `0E31:00DE` |
| 52 | `4436` | D | s | – | `GemPriceFactor` | `random(120)+40` | Edelsteinkurs; Wert eines Steins = `BaseUnit·GemPriceFactor/100` | G – `0E31:00EF`, `0825:08EB` |

Breite: B = Byte, W = Wort, D = Dword; Vz: s = vorzeichenbehaftet, u = vorzeichenlos (aus der
Speicherroutine). „init.“ = Wert aus den initialisierten Daten der EXE (Test
`Initial_state_matches_initialised_data_in_TEXE`), „BSS 0“ = beim Programmstart null.

### Schwierigkeitsstufen (Tabelle `DS:0B90…0BAF`, kopiert von `192D:0592`)

| Level | IQ | Öl | Roboter | Edelsteine | Katastrophen | Piraten |
|---:|---|---|---|---|---|---|
| 0 | DIM | aus | aus | aus | aus | NONE |
| 1 | GOOD | an | aus | aus | an | LOW |
| 2 | SHREWD | an | aus | an | an | NORM |
| 3 | SHREWD | an | an | an | an | HIGH |

Beim Programmstart wird die Voreinstellung des Levels (2) erst beim Betreten des Menüs
(`192D:0007` → `0592`) übernommen; BSS-Optionen sind bis dahin 0.

## 3. Neue Partie (`0E31:0003`)

Reihenfolge: Land erzeugen (`01CD`: `0361` → zufällig `0379` oder real `0465` je `LandType`;
alle Felder Besitzer −1, Art −2; `FieldTotal`), Stationen (`023D`, verbraucht Zufall),
Wetter (`21FD:021F`), je Spieler die Startwerte aus §2, `BaseUnit`, `GemPriceFactor`
(`random(120)`), Warenpreise 3, `Week` 0, `Unknown4580` −1, `Unknown40F6` = 0,2,1,0.
Die Zufallsreihenfolge ist für M4 zu portieren (Befund dann in `docs/routines/`).

### 3.1 Neue Partie belegt (Spur `partie`, R4)

Befund [0E31_0003_neue_partie.md](../routines/0E31_0003_neue_partie.md), Differenztests
`tests/Traders.Tests/Rules/NewGameDifferentialTests.cs` (ganzes Datensegment gegen RefEmu). Präzisierungen zu §2 und §3:

- `LandType` 0 → zufälliges Land `0379`, **jeder andere Wert** → reales Land `0465`; beide setzen die Bodengüte `45DA`
  (je `Spalten·Zeilen/4 − 1` Felder Güte 0, 1, 2, Rest 3), Besitzer und Art setzt `01CD`.
- `StationValue` (`348A`): −1 mit 2/3, 10 mit 2/9, 0…3 mit je 1/36 (`random(n)` liefert 0…n; die Angabe „½, ¼“ in Zeile 34
  gilt nicht).
- `Cash` = `InitialCash` vorzeichenbehaftet erweitert, `BaseUnit` = `InitialCash idiv 40`; `GemPriceFactor` Hochwort 0.
- Hinterlassene Schleifenvariablen: `37AA` = 4, `45D0` = 4, `2EB0` = Spalten, `2F00` = Zeilen. Nicht berührt: `HqY`,
  `Unknown4BBE`, `WealthHistory`.

## 4. Laufzeitzustand außerhalb des Spielstands (Auswahl)

| DS | Breite | Name | Bedeutung | Beleg |
|---|---|---|---|---|
| `10D2` | D u | `RandomSeed` | Zustand von `random()` (Startwert 1) – **nicht** im Spielstand | G (RNG-Befund) |
| `37AA` | B s | `ActivePlayer` | aktueller Spieler; zugleich globale Schleifenvariable | G |
| `45D0` | W | – | globale Schleifenvariable (Waren) | G |
| `2EB0` / `2F00` | B | – | globale Schleifenvariablen Feld-x / Feld-y | G |
| `08F1` | B | `LandType` | 0 RANDOM, 1 REAL | G – `192D:0CC7` |
| `10A7` | W | `Sound` | Ton an/aus | G – `192D:0D74` |
| `304E` / `305A` | B[4] | – | Cursorposition x / y je Spieler | G – `0609:069F` |
| `3D42` | B[4] | – | Rang (3 − Anzahl schlechterer Gegner), aus Vermögen `0825:06CB` | W |
| `302A` | B[4] | – | Rang (Variante bei Gleichstand) | W |
| `2F06` | D[4] | – | aktuelles Gesamtvermögen (`0825:0813`) | G |
| `43F4` | W | – | Ölpreis | G – gesetzt in `1777:02DC` |
| `48A0` | W | – | Raketenpreis | G – `1777:02DC` |
| `40CC` | W | – | Lottopreis | W – `1777:02DC`, Ladenanzeige |
| `3746` | W | – | Landpreis (Vermögen = Felder·Preis/3) | G – `1777:02DC` |
| `4B98` | W | – | Ölvorrat des Ladens | G – `08CF:05A3` |
| `2F26` | B | – | Obergrenze je Woche = min(30, F/2 + 3), F = Produktionsfelder aller Spieler; Bedeutung offen (BESTAND B3) | W – `0609:1178` (zweimal je Woche), RefEmu-Probe P3 |
| `48A2` | B | – | Obergrenze je Woche = min(30, F/2 + 6); Bedeutung offen (BESTAND B3) | W – `0609:1178`, Probe P3 |
| `48F4` | B[9·26] | – | Produktionsergebnis je Feld (4–60), Zwischenspeicher; im Roboterkampf dasselbe Raster als Kampffeld (`1E7E` u. a., BESTAND B5) | W – `22A3:0003`; S – `1E7E` |
| `451E` / `4528` | W | – | Spielfeldursprung x = (320 − Spalten·32)/2, y = ((146 − Zeilen·22) − 14)/2 + 58 | G – `192D:0058`, Differenztest |
| `117E` / `1180` | W | – | Tickzähler des Timer-Interrupts | G – `27C6:03C3` |
| `5330` / `5332` | W | – | Menü-Kopie von Spalten/Zeilen | G |
| `3D47` | B[128] | – | Tastenzustand je Scancode (F1 = `3D82`, Enter = `3D63`) | G |
| `37CC` | Far[304] | – | Textzeiger: Zeile *k* bei `37D0 + 4·k` | G – Ladecode `192D:18B6` |
| `0DE7` | char[] | – | 12 Stationsnamen (VULCAN … LAVE) | G (Test) |
| `09CE` | 10×19 B | – | Ressourcenliste `V-*.PP`/`E-*.PP` | G (Vorversuch) |

### 4.1 Eingabe und Zeitgeber (Spur `host`, R3)

Belege: Befunde `docs/routines/27C6_0043_eingabe_zeitgeber.md`, `0E9E_000E_steuerung.md`; RefEmu-Proben in
`tests/Traders.Tests/App/`. Präzisiert die Zeilen `117E`/`1180` und `3D47` oben (geschrieben von `27C6:037A`/`03E4` bzw. `0043`).

| DS | Breite | Name | Bedeutung | Beleg |
|---|---|---|---|---|
| `1158` | B | – | letzter Make-Code | G – `27C6:0043`, Probe |
| `115D` | B | – | Make-Zähler mod 256 | G – `27C6:0043`, Probe |
| `117E` / `1180` | W | – | Tickzähler 72,82 Hz (PIT-Teiler `4000h`); `1180 & 3` = 0 → alter `int 8` | G – `27C6:03E4`, Probe |
| `083C` | B[4] | – | Steuerungsart je Spieler (0/1 Joystick, 2 Maus, 3 Tasten QAZX, 4 Cursor, 5 Parallelport) | G – `0E9E:0035`, Probe |
| `0840` | B[4] | – | 1 = Computer | G – `0E9E:0035`, Probe |
| `37C3` | B[4] | – | Steuerbits (1 rechts, 2 links, 4 hoch, 8 runter, 10h Feuer) | G – `0E9E:0035`, Probe |
| `3786` / `302E` | DW[4] | – | Bewegungs- / Feuerverzögerung | G – `0E9E:0264`/`0312`, Probe |
| `44BE`, `43F0`, `439D`, `44BA`, `4894` | B[4] | – | Merker rechts, links, hoch, runter, Feuer | G – `0E9E:0264`/`0312`, Probe |
| `3308` | B[4] | – | Aktion −1/0 rechts/1 links/2 hoch/3 runter/10 Feuer | G – `0E9E:037A`, Probe |
| `2C10` | W | – | Spielerzähler in `0E9E:000E` | G – Probe |

### Grafikschichten (Spur `schichten`, R3)

Belege: Differenztests `tests/Traders.Tests/Vga/PageLayerTests.cs`, `PaletteLayerTests.cs` (ganzes DS gegen RefEmu).

| DS | Breite | Name | Bedeutung | Beleg |
|---|---|---|---|---|
| `090E` | W | `CardMode` | Anzeigemodus (0 EGA, 1 VGA, 2/63h Tandy); VGA = 1 | G – `0B67:0002`, `2295:006D` |
| `0238` | W | `ShownPage` | angezeigte Seite (0 = Seite 0, sonst Seite 1) | G – `0B67:0196/01B5` |
| `023A` | W | `DrawHidden` | 0 = auf der angezeigten Seite zeichnen, sonst auf der verdeckten | G – `0B67:01D4/0206` |
| `2BA0` / `2BA4` | Far | `Page0` / `Page1` | Adresse der Seiten (VGA `A000:0000` / `A400:0000`) | G – `0B67:007F` |
| `2BA8` | Far | – | Tandy-Puffer (nur Modus 63h) | G – `0B67:0018` |
| `2BAC` | Far | `PageBuffer` | Puffer 500h Byte (VGA), angelegt in `0B67:0002` | G |
| `082E` / `0830` | W | `DrawPageIndex` / `ShownPageIndex` | Seitenindex des Spiels: gezeichnet / angezeigt (`0830` = 1 genau dann, wenn `082E` = 0) | G – `21FD:051F` |
| `40DE` | D[2] | `PageNumbers` | Seitennummern (1/2) für `0B67:0238`, Index `082E`/`0830` | G – `21FD:051F` |
| `4516` | D | `CurrentDrawPage` | Seitennummer, auf die nach dem letzten Wechsel gezeichnet wird | G – `21FD:051F` |
| `3D95` / `3D91` | B | `NoFlipWait` / `SlowFlip` | ≠ 0: `051F` wartet nicht auf den Strahlrücklauf / wartet vier weitere | G – `21FD:051F` |
| `3DCB` | B[768] | `BasePalette` | Basispalette | G – `21FD:026C/02C9` |
| `5017` | B[768] | `LoadedPalette` | Palette des zuletzt geladenen Bildes | G – `21FD:026C` |
| `54E8` | B[768] | `WorkPalette` | Arbeitspalette (Ergebnis der Blende, an den DAC) | G – `21FD:03EB` |
| `10D8` | W | `BlendColor` | Blendfarbe (12 Bit RGB) der Einblendungen | G – `21FD:04DA/0329` |
| `33E4` | 4 B·2·8 | `ObjectRestoreRecords` | Rückschreibsätze der bewegten Objekte je Seite (Seitenoffset, x&3, gültig), beim Ausblenden genullt | G – `21AF:000D/04A2/04CD` |
| `330C` | 1Ah·8 | `ObjectTable` | Objektsätze (Belegung `GRAFIK.md` §4) | G – `21AF:000D`, `23CD:06E9` |
| `34A6` / `3068` | Far[21]·8 | `ObjectImages` / `ObjectMasks` | Bild- bzw. Maskenzeiger je Objekt und Bild | G – `21AF:00CA` |
| `3438` | Far[2]·8 | `ObjectSaveBuffers` | Sicherungspuffer je Objekt und Seite (im RAM-Atlas) | G – `21AF:022B` |
| `45D6` / `4BC6` | Far | `AtlasPointer` / `MaskPointer` | RAM-Atlas / Maskenatlas | G – `21AF:00CA` |
| `15BA` / `15BE` / `15C0` | Far / W / 1Ch·n | `ListPointer` / `ListCount` / `ListStart` | Befehlsliste `0A96` | G – `0A96:001B/06A8` |
| `0142` / `0143` | B s | `TextAdvance` / `ShadowHeight` | Schattenrechteck je Zeichen / Höhe | G – `0A96:000A/0C68` |
| `01F4` | Far[17] | `HandlerTable` | Ausführende der Befehlsliste | G – `0A96:06A8` |
| `1126` / `1127` | B / char[] | `NumberFill` | Füllzeichen / Format `%*lu` für Zahlen | G – `2295:0001/0062` |
| `3CA8` / `34A4` | W | `FontAdvance` / `FontLineHeight` | Zeichenbreite / Zeilenhöhe der Schrift | G – `0A69:0090/0288` |
| `0140` | char[2] | `TypewriterChar` | Ein-Zeichen-Text der Schreibmaschine | G – `0A69:01AD` |

## 5. Abgeleitete Werte

Preise (`43F4`, `48A0`, `40CC`, `3746`, `4B98`) werden in `1777:02DC` aus Bargeld der Spieler, Feldzahlen, Woche und
Lotto-Einstellung berechnet – ohne Uhr, Zufall oder Host-Dienst (RefEmu-Probe P2, BESTAND B1) – und nicht gespeichert. Vermögen (`0825:0813`) = Bargeld + Land
(`FieldCount·[3746]/3`) + Waren (Σ `Stock/10·WarePrice`) + Edelsteine (nur wenn an:
`Gems·BaseUnit·GemPriceFactor/100 + GemCredit·BaseUnit/100`) + Sonstiges
(`(Rockets + FieldsByKind[3])·[48A0]`, plus `Oil·[43F4]` wenn Ölverbrauch an).
Portierung gehört zu M11; Rechenreihenfolge und 32-Bit-Zwischenwerte dabei aus dem Code.

## 6. Offene Punkte

- Bedeutung `Unknown40F6`, `Unknown4BBE`, `Unknown4580`, Roboterfelder 18–25 (M12).
- Zuordnung Feldart 0–2 → Ware (Tabelle `DS:08E4`: Art 0→2, 1→0, 2→1) und Warennamen
  (Wasser/Blumen/Parfüm laut Wettertexten; vierte Ware offen).
- Bedeutung `StationValue` und `StationVisited` (M9).

## 7. Runde 3, Spur `regeln` (Wochenregeln)

Ergänzungen mit Beleg aus den portierten Routinen; Namen in `DgroupLayout.cs` (additiv). Die Einträge `43F4`, `48A0`,
`40CC`, `3746`, `4B98`, `2F26`, `48A2` in §4 gelten weiter; hier nur Neues und Präzisierungen.

| DS | Breite | Name (C#) | Bedeutung | Beleg |
|---|---|---|---|---|
| `34A2` | W s | `LottoPrize` | Lottowert der Woche: Durchschnittsbargeld / 20, 10, 5 je Lotto-Einstellung, mindestens 20; bei ungültiger Einstellung unverändert (vorläufig: Gewinn) | G – `1777:02DC`, gelesen `08CF:10C8`, `08CF:1668`; Differenztest |
| `40CC` | W s | `LottoPrice` | `34A2` / 32 (vorläufig: Lospreis) | G – `1777:02DC`; Differenztest |
| `4B98` | W s | `ShopOil` | (Bargeldsumme · Woche) mod (2 · (Spalten·Zeilen − `2F05`) + 1), 16-bittig; `08CF:05A3` zählt herunter | G – `1777:02DC`; Differenztest |
| `43F4` / `48A0` / `3746` | W s | `OilPrice` / `RocketPrice` / `LandPrice` | Formeln in [1777_02DC_wochenpreise.md](../routines/1777_02DC_wochenpreise.md) | G – Differenztest |
| `2F26` / `48A2` | B s | `LimitLow` / `LimitHigh` | 8-Bit-Rechnung mit vorzeichenbehaftetem Vergleich ([0609_1178_obergrenzen.md](../routines/0609_1178_obergrenzen.md)) | G – Differenztest; Bedeutung offen |
| `45D0` | W s | `LoopWare` | globale Schleifenvariable; nach `0609:1178` = 3, nach `22A3:026B` = 4 | G – Differenztest |
| `2F06` | D[4] s | `CurrentWealth` | Vermögen je Spieler, nur von `0825:06CB` geschrieben | G – Differenztest |
| `3D42` | B[4] u | `Rank` | 3 − Zahl der anderen mit Vermögen ≤ eigenem; Gleichstand senkt beide (präzisiert §4) | G – `0825:06CB`; Differenztest |
| `302A` | B[4] u | `RankUnique` | wie `3D42`, Gleichstand zugunsten der kleineren Spielernummer (eindeutig 0–3) | G – `0825:06CB`; Differenztest |
| `342C` | B[4][3] u | `ProductionShares` | Anteil der Art an den eigenen Produktionsfeldern in % | G – `22A3:0750`; Differenztest |
| `57F0` / `57F2` | W s | `ProductionIdle` / `ProductionActive` | Felder ohne Ertrag (unbebaut oder Vorrat < `2F26`) / mit Verbrauch | G (Zählung), W (Verwendung) – `22A3:0003` |
| `2EB0` / `2F00` | B s | `LoopX` / `LoopY` | nach `22A3:0003` = Spalten / Zeilen | G – Differenztest |
| `08E4` / `0902` / `112C` | B s | `KindWare` / `QualityTable` / `WeatherTable` | Ware je Art; Güte-Zuschlag [Art·4 + Güte]; Wetter-% [Art·6 + Wetter] | G – `22A3:0003`; Differenztest |
| `2BC2` / `2BBE` | W s | `AuctionStartPrice` / `AuctionBestBid` | Startpreis (Kopie des Warenpreises, `0B9F:0093`) / Höchstgebot der Versteigerung, 0 = keins (vorläufig) | G – `0B9F:078A`; Differenztest |

Präzisierung zu §5 (Vermögen): Edelsteine = Steine · (Grundeinheit · Kurs / 100) + Grundeinheit · Konto / 100; Land-, Waren-,
Raketen- und Ölprodukte werden auf 16 Bit abgeschnitten; Öl zählt nur bei `45BA` = 1
([0825_0813_vermoegen.md](../routines/0825_0813_vermoegen.md)).

## 8. Runde 4, Spur `menue` (Menüs `192D`)

Namen in `src/Traders.Core/Menus/MenuLayout.cs`; Befund [192D_0007_menues.md](../routines/192D_0007_menues.md).

| DS | Breite | Name (C#) | Bedeutung | Beleg |
|---|---|---|---|---|
| `3CCC` | B | `Choice` | 1 neue Partie, 2 geladene Partie fortsetzen | G – `192D:12A6`; Differenztest |
| `4581` | B | `AllComputers` | 1, wenn alle vier Spieler Computer sind | G – `192D:03E3`; Differenztest |
| `5330` / `5332` | W s | `EditColumns` / `EditRows` | Spalten/Zeilen im Einstellungsmenü, übernommen nur bei neuer Partie (`00A3`) | G – `192D:0092/00A3/0D10/0D42`; Differenztest |
| `5334` | W s | `PlaqueTop` | Oberkante der Menütafel (200 − h)/2, Diskette 1Bh | G – `192D:16AF/0FB7`; Differenztest |
| `08FA` | B | `GameInProgress` | Partie läuft bzw. geladen (Speichern erlaubt, Fortsetzungsfrage) | G – `192D:11A0/11C9/12A9` |
| `08DA` | W | `PictureResource` | Ressourcennummer des Menübilds (`186C:0002`) | G – `192D:000A` |
| `37CC…3C8F` | far[305] | `TextPointers` | Eintrag k = Zeile k − 1 von ENGLISCH.TXT, Eintrag 0 null | G – `192D:18B6`; Differenztest |

## 9. Runde 4, Spur `programm` (Programmablauf)

Belege: [1777_000C_main.md](../routines/1777_000C_main.md), [1777_04E4_initialisierung.md](../routines/1777_04E4_initialisierung.md);
Gleichschritt-Differenztest `ProgramMachineDifferentialTests`.

| DS-Offset | Typ | Name (C#) | Bedeutung | Beleg |
|---|---|---|---|---|
| `090E` | W s | `ProgramInitialisation.GraphicsCard` | Grafikkarte: 1 VGA, 0 EGA, 2 Tandy, 63h Tandy-Emulation, −1 erkennen | S `1777:04E7`, `0D73:0712`; P = 1; T |
| `0910` | W s | `ProgramInitialisation.SoundDevice` | Tonart: 0 PC-Lautsprecher, 1 AdLib, −1 erkennen | S `1777:04FF`, `0D73:04CA`; P = 1; T |
| `08FA` | B | `ProgramLoop.GameRunning` | vor jeder Wochenschleife 1 (Bedeutung offen; gelesen `192D:11B8`/`12A6`) | S `1777:017A`; T |
| `3CCC` | B | `ProgramLoop.NewGameRequest` | Menüergebnis „neue Partie“ (1), gesetzt in `192D:12A6` | S `1777:016E`; P; T |
| `10A3` | W | – | Musiktreiber vorhanden (`218B:0002`; `218B:013A` liefert 0, wenn 0) | S `218B:013D`; P = 1 |
| `10A5` / `10A9` | W / W | – | Musik läuft / aktuelles Stück (`218B:0094`) | S `218B:00B4`, `0131`; P |

**Teil 2 (Programmstart `0DFD:000D`, Code-Abfrage `21FD:0687`):** Belege [0DFD_000D_programmstart.md](../routines/0DFD_000D_programmstart.md),
[186C_ressourcenlader.md](../routines/186C_ressourcenlader.md), [21FD_0687_codeabfrage.md](../routines/21FD_0687_codeabfrage.md);
Differenztests `ProgramStartDifferentialTests`, `CodeQueryDifferentialTests`.

| DS-Offset | Typ | Name (C#) | Bedeutung | Beleg |
|---|---|---|---|---|
| `09C8` | W s | `ImageTable.CurrentImage` | zuletzt geladenes Bild (−1 vor dem ersten; zugleich EGA-Palettennummer für `0A96`) | S `186C:008B`; P (2, dann 6); T |
| `09CE` | 13h·10 | `ImageTable.Start` | Bildtabelle: +0/+4 Far-Zeiger VGA-/EGA-Name, +8 Diskette, +0Bh Dateilänge, +0Fh entpackte Länge (0 = noch nicht geöffnet) | S `186C:0997`; P; T |
| `08CE…08DA` | W[7] | `ImageTable.PreloadNumbers`, `CodeRequestNumber` | Bildnummern der sechs vorgeladenen Bilder (`0DFD:0305…033B`) und des Abfragebilds | G `0DFD:0305`, `21FD:06A9`; T |
| `10E0` | Far[4] | `CodeQuery.Texts` | Zeiger auf die vier verschlüsselten Abfragetexte (in DGROUP, `10FF`, `110C`, `110E`, …) | G/S `21FD:081D`; P |
| `10E8` | Far | `CodeQuery.CodeText` | dritter Zeiger: Text „CODE: X-nn“ (Zeichen +6, +8, +9) | S `21FD:0717…075C`; P; T |
| `10F0…10FA` | B[11] | – | Ziffernblock-Tabelle, adressiert als `[SI + 10A9h]` mit SI = 47h…51h (7 8 9 – 4 5 6 + 1 2 3) | G `21FD:07CD`; P |
| `4884` | Far | `MenuLayout.TextBuffer` (`TextFile`) | Textpuffer von `192D:18B6`; im Port Far-Adresse des Hosts (Teil 3) | S `0DFD:0074`; T |
| `10A9` | W | – | **aktuelles Musikstück** (FFFF aus der EXE, 0 nach `218B:0094(0, 0)` bei `0DFD:02FE`); kein Teil der Ziffernblock-Tabelle | S `218B:00EE`; P |

## 10. Runde 4, Spur `spielfeld` (Steuerung `0E9E`, Spielfeld `0609`)

Ergänzt §4.1; Namen der Konstanten in `src/Traders.Core/Input/ControlReader.cs` bzw. `JoystickReader.cs`.

| DS | Typ | Name | Bedeutung | Beleg |
|---|---|---|---|---|
| `37C3`, `3786`/`302E`, Merker, `3308`, `2C10` | wie §4.1 | `Bits`, `MoveDelay`/`FireDelay`, `Flag*`, `Action`, `PlayerIndex` | Verzögerungen als `long` vorzeichenbehaftet | G – Differenztest `0E9E:*` |
| `2BE6` | W | `MouseDriver` | AX von `int 33h`/0 (`0D73:00F8`); ≠ 0 → `0D73:07E0` fragt die Maus ab | G – Differenztest `0D73:07E0` |
| `1166…1170` / `1172…117C` | W[6] je Knüppel | `Stick1` / `Stick2` | Mitte x, Mitte y, Schritt x unter/über der Mitte, Schritt y unter/über der Mitte | G – Differenztest `27C6:0268` |
| `2F05` | B u | `FreeFields` | freie Felder (vom Feldsetzen `0609:10D2` gezählt; die neue Partie setzt Spalten·Zeilen) | G – Differenztest `0609:10D2` |
| `4BA8` / `2EE7` | B[4] / B[4][4] u | `OwnedFields` / `OwnedKinds` | Felder je Spieler / je Spieler und Art 0–3 | G – Differenztest `0609:10D2` |
| `37AA` | B s | `CurrentPlayer` | Spieler am Zug (Vergleich in `0609:1072`) | G – Differenztest `0609:1072` |
| `304E` / `305A` | B[4] s | `CursorX` / `CursorY` | Cursor je Spieler | G – Differenztest `0609:047B` |
| `43B2` | B[4] u | `Mode` | Modus im Spielfeld: 4 = Art wählen, 6 = fertig (übrige Werte vgl. `0609:064D`) | G (4, 6), W (übrige) – `0609:047B`, `0EDD` |
| `4BCA` / `3022` / `4BD2` | W[4] s | `Direction` / `MoveTimer` / `MoveFrame` | Zugrichtung 0–3 / Zugdauer 11h bzw. 0Ch / Anfangsbild | G (Werte), W (Bedeutung) – `0609:047B` |
| `2E94` | B[4] s | `Selection` | gewählte Art −1…4 | G – Differenztest `0609:0BEA` |
| `45DA` | B[9·6] u | `Terrain` | Gelände je Feld; 3 überspringt Art 3 | G (Prüfung), W (Bedeutung) – `0609:0BEA` |
| `1582` / `45BA` | B u | `NoTime` / `TimeLimit` | Züge kosten keine Zeit (1) / Zeitlimit (≠ 0) | G – Differenztest `0609:0EA1` |
| `4520` | W[4] s | `TimeLeft` | Zeiteinheiten übrig; `1B72` addiert den Schritt | G – Differenztest `0609:1B72` |
| `3CE6` | B u | `Finished` | von `0EDD` herabgezählt; die Spielfeldschleife `0609:03E0` läuft bis 0 | G (Zählung), W (Bedeutung) |
| `00E0` | B[4] u | `Blink` | Blinkzähler des Cursors (13h nach `random(200)` = 0) | G – Differenztest `0609:215E` |
| `330C` / `3324` | B / W, Sätze zu 1Ah | `Sprite` / `SpriteFrame` | Sprite sichtbar / Bild (Satz je Spieler) | G (Schreiben `21AF:045E`, `215E`), W (Bedeutung) |
| `3D42` | B[4] u | `RankTable` | Rang je Spieler (Abzeichen im Wappen, `0609:1854`) | G – Differenztest `0609:1854` |
| `00D8` | W[4] s | `RankOffsetY` | y-Versatz des Rangabzeichens im Wappen | G – Differenztest `0609:1879` |
| `43FA` | B[4] u | `Marker` | 1 = Merker neben dem Rangsymbol (`0609:1A61`) | G (Prüfung), W (Bedeutung) – Differenztest `0609:1A61` |

## 11. Runde 5, Spur `planung` (Wochenplanung `11A3`)

Namen in `src/Traders.Core/Ai/WeekPlanningLayout.cs` (alle vorläufig); Formeln und Leser im Befund
[11A3_0000_wochenplanung.md](../routines/11A3_0000_wochenplanung.md); Beleg jeweils G – `11A3:*`, Differenztest
`WeekPlanningDifferentialTests`. Präzisiert §2 Nr. 10 (`40F6`) und Nr. 45 (`4580`).

| DS | Breite | Name (C#) | Bedeutung |
|---|---|---|---|
| `4580` | B s | `TargetPlayer` | Zielspieler der Computer, −1 = keiner (`0180`, höchstens ein `random(5)`) |
| `40F6` | W[4] s | `MainKind` | Hauptart 0–2 je Spieler; Mensch −1 ohne Art mit ≥ 70 % (`0599`, `061A`, `067D`) |
| `2C74` / `2C7C` | W[4] s / W s | `Ware3Rank` / `Ware3Lead` | Rang nach Vorrat Ware 3 (Gleichstand senkt beide) / Vorsprung des Ersten (`007D`) |
| `2C4C` | W[4][3] s, Stride 8 | `QualityShare` | Güteanteil je Art in % (`043F`) |
| `2C44…2C4A` / `2C6C…2C72` | W[4] s | `PlanFields` / `PlanFieldsLimited` | Sollfelder je Art, [3] = Raketen; zweite Fassung nach Vorratsgrenzen (`078A`) |
| `303E…3044` / `3046` | W[4] s | `FieldSurplus` / `RocketShortfall` | eigene Felder − Soll / max(0, −`3044`) je Spieler (`078A`, `104B`) |
| `2F31`, `4654`, `472C` / `48EC` | B[4][54] / W[4] | `PlantList`, `PlantListX`, `PlantListY` / `PlantListLength` | Pflanzliste Plan, x, y / Länge (`0F2E`) |
| `4560` / `452A` | W[4][4] s, Stride 8 | `WareNeed` / `PriceLimit` | Warenbedarf / Preisgrenze je Ware, `452A[p][3]` = `4560[p][3]` (`104B`) |
| `48F4` | B[9][26] | `ProductionResult` | in Durchgang 0 Anbauplan (100 = frei, sonst Art bzw. 3 = Rakete), `0B3B` |
| `08E7` | B[3] s | `SecondWare` | zweite Ware je Art (init. 1, 2, 0), `0AA5`, `104B` |

## 12. Runde 5, Spur `anbau-ki` (Computerzug `135D`)

Belege: Differenztests `tests/Traders.Tests/Ai/Planting*`, Befund [../routines/135D_000E_computerzug.md](../routines/135D_000E_computerzug.md).
Konstanten in `src/Traders.Core/Ai/Planting.cs`. `135D` zieht keinen Zufall.

| DS | Typ | Name | Bedeutung | Beleg |
|---|---|---|---|---|
| `40CE` / `40D6` | W[4] s | `AiTargetX` / `AiTargetY` | von der KI gewähltes Zielfeld | G – `135D:00AC`/`02BD`/`0480` |
| `410A` / `4112` | W[4] s | `AiPickX` / `AiPickY` | bestes freies Feld aus `135D:05AF` | G – `135D:05AF` |
| `4654` / `472C` / `2F31` | B[4·54] | `AiCandX` / `AiCandY` / `AiCandKind` | Kandidatenliste je Spieler (Stride 54) | G – `135D:0935`, L – `02BD`/`0A59` |
| `48EC` | W[4] s | `AiCandCount` | Zahl der Kandidaten je Spieler | G – `135D:0935` |
| `3C90` | W[4] s | `AiCandIndex` | aktuell abgearbeiteter Kandidatenindex | G – `135D:02BD`/`0A59` |
| `4BBE` | B[4] u | `AiNoneFound` | 1 = `05AF` fand kein Feld | G – `135D:05AF` |
| `43F6` / `43F8` | W s | `AiRefX` / `AiRefY` | Zielbezug (Cursor-x/y aus `135D:000E`) | G – `135D:000E` |
| `40F6` | W[4] s | `AiWantKind` (vorher `Unknown40F6`) | Wunschart der KI (von `0AB3` bestimmt) | G – `135D:0AB3`, L – `06A1`/`0935` |
| `08E7` | B[?] s | `KindWare2` | Art → Ware-Index (Vorratsprüfung `02BD`) | L – `135D:02BD` |

## 13. Runde 5, Spur `bilanz` (Warteschleife `21FD:0004`, Wochenbilanz `0825`)

Namen der Konstanten in `src/Traders.Core/Week/WaitLoop.cs` (`WaitLoopLayout`).

| DS | Typ | Name | Bedeutung | Beleg |
|---|---|---|---|---|
| `08C6` | B[8] u | `PlayerBits` | Bitmasken 01h…80h; Eintrag p ist das Bit von Spieler p in der Maske von `21FD:0004` | G – Differenztest `21FD:0004` |
| `3D55` | B u | `AbortKey` | Tastentabelle `3D47` + 0Eh (Rücktaste); ≠ 0 beendet `21FD:0004` sofort (Abbruchweg A) | G – Differenztest `21FD:0004` |
| `3D60` | B u | – | Tastentabelle `3D47` + 19h (P); `21FD:0245` wartet nur, solange P gedrückt ist (Pause), kein Zeitgeber-Merker | G – Differenztest `21FD:0004` (Szenario Pause) |
| `54B0` | B s | `IndicatorCountdown` | Abwärtszähler der Anzeige in `1F63:113E`: > 0 zählt herab (bei 0 aus), sonst `random(200)` = 0 → an und 8 | G – Differenztest `21FD:0004` (Modus 4) |
| `3308` | B[4] s | `Action` (§10) | in `21FD:0004` für Computer überschrieben: 0Ah, wenn Zähler = 20·p, sonst FFh | G – Differenztest `21FD:0004` |

Gelesen von der Szene der Wochenbilanz (`ReportLayout` in `src/Traders.Core/Week/Report.cs`, nur Lesen):

| DS | Typ | Name | Bedeutung | Beleg |
|---|---|---|---|---|
| `00C8` | W[4] | `PlayerColors` | Farbe je Spieler für Tabellenzahlen und Verlauf (`0825:0178`, `0560`); vorbelegt | G – Bildvergleich `0825:000A` |
| `00D8` | W[4] | `EmblemBadgeOffsets` | y-Versatz des Rangabzeichens im Wappen je Spieler (`0609:1879`); vorbelegt | G – Bildvergleich `0825:000A` (Woche 1) |
| `37F4` | far ptr | `TitleText` | Text 0Ah der Tabelle `37CC`: Überschrift der Bilanz (`0825:0072`) | G – Bildvergleich `0825:000A` |
| `37F8…380F` | far ptr[6] | `RowTexts`, `TotalText` | Texte 0Bh…0Fh (Zeilen Bargeld bis Übriges) und 10h (Gesamt), `0825:0178` | G – Bildvergleich `0825:000A` |

## 14. Runde 5, Spur `anbau` (Anbauphase `0609:0004`)

Befund [0609_0004_anbau.md](../routines/0609_0004_anbau.md). G = belegt durch Differenztest, W = Bedeutung aus dem Ablauf gefolgert.

| DS-Offset | Typ | Name (C#) | Bedeutung | Beleg |
|---|---|---|---|---|
| `43B2` | B[4] u | `Planting.Mode` | Modus im Anbau: 2 HQ bauen, 3 Cursor, 4 Art wählen, 5 freies Feld (zweite Runde `0080`), 6 fertig | G – `0609:0004`, `064D` |
| `43FA` | B[4] u | `Planting.Purchase` | 1 = darf in dieser Runde ein freies Nachbarfeld nehmen (gesetzt, wenn `4BC2` = 0 und `2F05` > 0); präzisiert §10 `Marker` | G – `0609:0004`, `07D6`; W |
| `3CAF` / `3CB3` | B[4] s | `Planting.SoldX` / `SoldY` | zum Verkauf markiertes Feld (−1 = keins); `0FA9` gibt es frei und zahlt `3746` | G – `096A`, `07D6`, `0FA9` |
| `48A2` | B s | `Planting.PlantCost` | Vorrat der Ware (Art + 1) mod 3, den das Bepflanzen eines Feldes der Art 0–2 kostet (klärt BESTAND B3 teilweise) | G – `0609:096A` |
| `4BAC` | W[4] s | `Rockets` | eine Rakete je Raketenbasis (Art 3) | G – `096A` |
| `1582` | B u | `Planting.NoTime` | 0 in `0004`, 1 in der zweiten Runde `0080` (keine Wartezeit, Cursor sofort sichtbar, Züge ohne Zeit) | G – `00D6`, `032A` |
| `3CE6` | B u | `Planting.Players` | Spieler noch im Feld; `032A` zählt, `0EDD` zählt herunter, `03E0` läuft bis 0 | G – `032A`, `03E0` |
| `330C + 1Ah·p` +0/+2/+14h/+16h/+18h | B/W | `Planting.Objects`, `ObjectLayer`, `ObjectX`, `ObjectY`, `ObjectFrame` | Cursorobjekt je Spieler: aktiv, Ebene 0Ah, Position, Bild; **Besitzer Kern** (#103 P3-4), Zeiger +4…+13h und `33E4`… gehören der Objektschicht | G – `032A`, `1F2D`, `1FCC`, `1FE0` |
| `08CE` | W | `Planting.PictureNumber` | Bildnummer des Anbaubilds (`186C:0002`) | G – `00D6` |
| `2EB0` / `2F00` | B s | `LoopX` / `LoopY` | nach `1BE5` Spalten bzw. Zeilen (Schleifenende), in `069F` Cursor für `074D` | G – `1BE5`, `069F` |

## Runde 6, Spur `ereignisse` (Wochenereignisse `17CB`)

Befund [17CB_0005_wochenereignisse.md](../routines/17CB_0005_wochenereignisse.md); Namen in `IncidentLayout`
(`src/Traders.Core/Events/WeekIncidents.cs`). G = belegt durch Differenztest `17CB:0005`.

| DS | Typ | Name | Bedeutung | Beleg |
|---|---|---|---|---|
| `09AA` / `09B9` | B[5] s | `GainNumberLine` / `LossNumberLine` | je Betragsklasse: Zeile des Betrags im Gewinn- bzw. Verlusttext; vorbelegt | G |
| `09AF` / `09BE` | B[5] s | `GainText` / `LossText` | je Betragsklasse: erster Textindex (Tabelle `37CC`); vorbelegt | G |
| `09B4` / `09C3` | B[5] s | `GainLines` / `LossLines` | je Betragsklasse: Zeilenzahl des Textes; vorbelegt | G |
| `3940` | far ptr | `TakeoverText` | Text 5Dh der Tabelle `37CC`: Feldübernahme (7 Zeilen) | G |
| `395C + 10h·k` | far ptr | `BlightText` | Text 64h + 4·k: Ernteausfall der Art k (4 Zeilen) | G |
| `082E` | W | `PageCopyArgument` | Argument der Seitenkopie `0A96:0C3F` im Fenster `0986` | G |
| `0140` | B | `TypewriterChar` | Zeichen der Schreibmaschine `0A69:01AD` (vom Kern geschrieben, `0141` = 0) | G |
| `1126` | B | `IncidentText.NumberFill` | Füllzeichen für führende Leerzeichen/Nullen in `2295:0001` (gelesen) | G |

## Runde 6, Spur `stationen` (Stationen `1B03`)

Befund [1B03_0009_stationen.md](../routines/1B03_0009_stationen.md), im Code `StationsLayout`. G = belegt durch Differenztest,
W = Bedeutung aus dem Ablauf gefolgert.

| DS-Offset | Typ | Name (C#) | Bedeutung | Beleg |
|---|---|---|---|---|
| `0C84` / `0C9C` | W[12] / B[12] u | `StationX` / `StationY` | Lage der Stationen auf `V-RAKETE` | G – `1617`, `0153`, `0CCF` |
| `0CA8` / `0D08` | far[12] / far[3] | `StationName` / `KindName` | Texte Stationsname / Art | G – `0548` |
| `0CF8` / `0D00` | W[4] | `RocketColor` / `PlayerColor` | Farbe von Raketenpunkt und Bestätigung / Textfarbe | G – `02C0`, `0548` |
| `0D14` | B[] u | `PirateChance` | Startchance 1/n des Piratenschiffs je Ort | G – `0A62` |
| `0D17` | W[90] | `FlightCurve` | Flugkurve (ungerade Adresse), Index Fortschritt 0…59h | G – `0CCF` |
| `0DCD`, `0DD9`, `0DDC`…`0DE4` | B[] | `StationPicture`, `StarColor`, `Pirate*` | Bildspalten, Sternfarben, Schiffsteile | W – `1178`, G – `128D` |
| `0E3E` / `0E40` | Text | `StationMarkText` / `ConfirmMarkText` | Marke an der Station / Bestätigung | G – `0153`, `02C0` |
| `08D0` / `0CD8` | W / Palette | `Picture` / `Palette` | Bild `V-RAKETE`, Palette | G – `0009` (Teile) |
| `4888` | W[4] s | `Cursor` | Station unter dem Zeiger je Spieler | G – `0201`, `0413`, `0632` |
| `5428` | B[12] u | `Taken` | 0 frei, 1 gehalten, 2 bestätigt | G – `0201`, `02C0`, `0413`, `04D4` |
| `546A` | B[4] u | `Occupied` | Wert der eigenen Station nennt einen anderen Spieler | G – `0763`, `0B7A` |
| `546E` / `546F` | B s | `PiratePosition` / `Flash` | Piratenort (−1 keine) / Blitzschritte | G – `01C3`, `0A62`, `0C89` |
| `5338` / `5336` / `5337` | B | `PirateState` / `ExplosionVisible` / `LightFrame` | Zustand des Schiffs / Explosion sichtbar / Lichtbild 0…3 | G – `0A62`, `12D7`; W – `0BE7` |
| `53AA` / `53B1` | W / B | `LightCounter` / `ShipMoved` | Zähler der Lichter / Schiff an Platz 1 | G – `12D7`, `1345` |
| `53A6` / `53AD` | B[4] | `Launched` / `Lost` | Raketen eingeplant / an Piraten verloren | G – `07C4`, `0849`, `0E11`, `0DC0` |
| `53AC`, `53B2`, `53B3`, `53B4`, `53B5` | B | `RocketIndex`, `RocketTotal`, `PiratesReady`, `Flying`, `Attacked` | Flugzustand | G – `0740`, `0849`, `0B7A` |
| `53B6` / `533A` / `5434` | W[54] / W[54] / B[54] | `Progress` / `Target` / `Owner` | je Rakete Fortschritt (negativ = Startverzug), Ziel-x, Besitzer | G – `0849` |
| `5422`, `5424`, `5426` | B, W, W | `ExplosionFrame`, `RocketX`, `RocketY` | Explosionsbild, Lage der bewegten Rakete | G – `0CCF`, `0B7A` |
| `3376`, `330E + 1Ah·p` | W | `ObjectLayer` | Ebene 13 (Explosion, Objekt 4) / 15 (Zeiger) | G – `148B` |
| `3786` / `302E` / `37C3` | L[4] / L[4] / B[4] | `ControlReader.MoveDelay` / `FireDelay` / `Bits` | Verzug 3p, 43 − 10·IQ bzw. 20; Feuerverzug 10000; Computereingabe | G – `0201`, `0413`, `04D4`, `05E7` |

## 15. Runde 6, Spur `ernte` (Ernte `22A3:02D3`)

Befund [22A3_02D3_ernte.md](../routines/22A3_02D3_ernte.md). G = belegt durch Differenztest, W = Bedeutung aus dem Ablauf gefolgert.

| DS-Offset | Typ | Name (C#) | Bedeutung | Beleg |
|---|---|---|---|---|
| `57E8` | W[4] s | `Harvest.Counter` | Bilder der laufenden Sammlung eines Feldes; Balkenhöhe, laufender Vorrat + 2·Zähler; gebucht, sobald > Ertrag / 2 | G – `062E`, `0340`; W |
| `57F4` | B[4] u | `Harvest.Automatic` | 1 = Cursor geht ohne Taste weiter (Computer `0840` = 1 oder nach Feuer 0Ah) | G – `0340`, `04B1`, `04DA` |
| `43B2` | B[4] u | `FieldCursor.Mode` | in der Ernte: 7 warten, 8 sammeln, 6 fertig (ergänzt §14) | G – `0426`, `04F7`, `062E` |
| `3BA4` | Fernzeiger[6][3] | `Harvest.WeatherLines` | Wetterhinweis: Zeilen F5h + 3·Wetter der Texttabelle `37D0` (0-basiert) | G – `08C1`; W |
| `00C8` | W[4] | `Harvest.ColourOfPlayer` | Farbe des Erntebalkens je Spieler (initialisierte Daten) | G – `07C2` (nur Ausgabe) |
| `45D0` | W s | `LoopWare` | nach `0340` 4, in `062E` Art des gesammelten Feldes (Index in `3CF4`) | G – `0340`, `062E` |
| `0140` | B | – | Zeichenpuffer der Schreibmaschine `0A69:01AD` (Ausgabe) | G – `08C1` (ausgenommen) |

## Runde 7, Spur `handelski` (Handels-KI `141D`)

Befund [141D_0008_handelski.md](../routines/141D_0008_handelski.md); Namen in `src/Traders.Core/Ai/TradeAiLayout.cs`. p = `37AA`,
w = `45D0`. G = belegt durch Differenztest (`TradeAiDifferentialTests`), W = Bedeutung aus dem Ablauf gefolgert.

| DS-Offset | Typ | Name (C#) | Bedeutung | Beleg |
|---|---|---|---|---|
| `2D3E` | L[4] s | `Budget` | Bargeld / (150 − (10000 − Bargeld) / 83); unteres Wort = Grundgebot der Stufen | G – `0029`; W |
| `2D78` / `2D64` / `2D80` | W[4] s | `Need` / `Reserve` / `Surplus` | Bedarf ≥ 0 / Preisgrenze bzw. 30, 0 bei Überschuss / Überschuss ≥ 0 der Ware w | G – `021A` |
| `2D88` | L[4][4] s, Stride 16 | `WareBudget` | Bargeldanteil je Ware; `0CCA` zieht den Preis ab | G – `008C`, `05A3`, `0CCA` |
| `2C7E` | L[4][4] s, Stride 16 | `BidLevel` | Gebotsstufen Grund+Schritt, Grund, Grund−Schritt, Warenpreis | G – `04A5` |
| `2CFE` | W[4][4] s, Stride 8 | `BidUnits` | Einheiten je Stufe (Schritte von 10) | G – `05A3`, `0CCA` |
| `2D6C` / `2D50` | W[4] / L[4] | `CurrentLevel` / `CurrentBid` | erste Stufe mit Einheiten (4 = keine) / deren Gebot (0 = keins) | G – `05A3`, `0CCA` |
| `2CBE` / `2D1E` | L[16] / W[16] | `SortedBid` / `SortedUnits` | Stufen aller Käufer nach Rang; Verkäufer 0 | G – `0806`, `0E67` |
| `2D74` / `2D60` | W / L | `SortedIndex` / `AskFloor` | erster Listeneintrag mit Einheiten (16 = keiner) / dessen Gebot, 1 dahinter | G – `0806`, `0ED7`, `0BFD`, `0E67` |
| `2D76` / `2DC8` | W / L | `SellerSurplus` / `HighestBid` | Σ Überschuss der Verkäufer / größtes Gebot der Computer-Käufer | G – `0F01`, `0E09` |
| `2BBA` | W s | `LastSale` | Countdownstand beim letzten erzwungenen Schritt eines Verkäufers | G – `0BFD` |
| `43B2` | B[4] u | `Side` | im Handel: 0 Käufer, 1 Verkäufer (ergänzt §14/§15) | G – `09F3`, `0E09`, `0F01`; W – `0B9F:00FE` |
| `43A2` / `455A` / `3D1C` | W[4] / W / W s | `Position` / `MarketPrice` / `Spread` | Lage auf dem Handelsbild (−3 Käufer, 103 Verkäufer beim Seitenwechsel) / Startpreis / Spanne | G – `0A1D`, `0BFD`; W |
| `2BB8` / `2BC0` | B s / B | `Sellers` / `BuyerFlag` | Divisor in `0A1D` (0 bzw. −1 → Divisionsfehler) / 1 schaltet die Höchstgebotsregel ein | G – `0A1D`; W |
| `3780` / `2F16` / `2F18` | W s | `Countdown` / `CountdownA` / `CountdownB` | Handels-Countdown (`21FD:04E5`/`04F9`); `0397` handelt nur bei `2F18` = `2F16` | G – `0397`, `0BFD`; W |
| `4106` / `44F2` | W s | `Buyer` / `Seller` | Käufer (10 = Laden, dann kein `0CCA`) / Verkäufer eines Abschlusses | G – `0CCA`, `0E67`; W – `0C29:03B2` |

## Runde 7, Spur `zusatz` (Zusatzphase `1DEF:000C`)

Befund [1DEF_000C_zusatzphase.md](../routines/1DEF_000C_zusatzphase.md). G = belegt durch Differenztest, W = Bedeutung aus dem Ablauf gefolgert.

| DS-Offset | Typ | Name (C#) | Bedeutung | Beleg |
|---|---|---|---|---|
| `54A6` / `547C` / `54A2` | Fernzeiger | `ExtraPhase.GroundPointer` / `GoodsPointer` / `ClaimsPointer` | auf die Stapelfelder von `000C`: Bodenkachel W[42], Ware W[42] (−1 = keine), Ziel B[42] (FFh = frei); Zelle = 6·Zeile + Spalte | G – `000C` (Zeiger), alle Routinen (Felder) |
| `5480` | W s | `ExtraPhase.Collected` | aufgenommene Stücke; die Phase endet bei 25 | G – `000C`, `04A9`, `0651` |
| `5482` | W[4][4] s | `ExtraPhase.Gains` | Gewinn je Spieler und Ware (Schritt 8), am Ende auf `3CF4` addiert ohne Obergrenze | G – `03CD`, `0651`, `0751` |
| `0F22` / `0F2A` | W[4] | `ExtraPhase.TileX` / `TileY` | Atlaskoordinaten der vier Bodenkacheln (initialisierte Daten) | G – `0308` (nur Ausgabe) |
| `3B5C` | Fernzeiger[2] | `ExtraPhase.NoticeLines` | Hinweis: Textzeilen 227/228 der Tabelle `37D0` (0-basiert) | G – `0105`; W |
| `57F0` / `57F2` | W s | `ProductionIdle` / `ProductionActive` | Auslösung: 2·`57F2` < `57F0` (ergänzt §7) | G – `00DB` |
| `2F26` | B s | `LimitLow` | Einheiten je Stück 2·`2F26` (ergänzt §7) | G – `0651` |
| `304E` / `305A` | B[4] | `FieldCursor.CursorX` / `CursorY` | in der Zusatzphase Spalte 0…5 / Zeile 0…6 des Bretts | G – `03CD`, `04A9`, `0651`, `07AD` |

## Runde 7, Spur `handel` (Handel `0B9F:0002`, `0C29`)

Befund [0B9F_0002_handel.md](../routines/0B9F_0002_handel.md), Countdown [21FD_04E5_countdown.md](../routines/21FD_04E5_countdown.md).
G = belegt durch Differenztest, W = Bedeutung aus dem Ablauf gefolgert. Namen in `Traders.Core.Trade.TradeLayout`.

| DS-Offset | Typ | Name (C#) | Bedeutung | Beleg |
|---|---|---|---|---|
| `45D0` | W s | `Ware` | gehandelte Ware (1. Argument von `0C29:0001`) | G – `005C`, `0321`, `03B2` |
| `2BC5` | B | `Variant` | Bild: 0 `V-AHANDE`, 1 `V-BHANDE` (2. Argument) | G – `051A`, `1108`; W |
| `43B2` | B[4] u | `Role` | 0 Käufer, 1 Verkäufer, 6 ausgeschieden (ergänzt §14/§15) | G – `00FE`, `0321`, `0FB9` |
| `43A2` | W[4] s | `Position` | Position auf der Leiter −3 … 67h; Preis = Position / 2 + `455A` | G – `0321`, `03B2` |
| `43B6` | DW[4] s | `Need` | Bedarf der Ware (Anzeige) | G – `009E`; W |
| `455A` | W s | `BasePrice` | Grundpreis (Position 0) | G – `005C`, `02BE`, `04FC` |
| `2BC2` | W s | `StartPrice` | Grundpreis zu Beginn = `4898[Ware]` (ergänzt `AuctionStartPrice`) | G – `005C`, `02BE` |
| `2BBE` | W s | `BestPrice` | höchster gezahlter Preis, 0 = keiner (bestätigt `AuctionBestBid`) | G – `03B2` |
| `2BBC` | W s | `Sellers` | Zahl der Verkäufer; Bietphase endet bei 0 | G – `01E1`, `051A`, `0FB9` |
| `439C` / `450C` | B | `NoHumanBuyer` / `NoHumanSeller` | 1 = kein Mensch kauft / verkauft | G – `01E1`; W |
| `2BB8` / `2BC0` | B | `TopBuyers` / `NoHumanTopBuyer` | Käufer auf `3D1C` / 1 = keiner davon Mensch | G – `02BE` |
| `2BC4` / `023E` | B | `Moved` / `MovedBefore` | jemand bewegte sich in diesem / im vorigen Bild | G – `0321`, `0549` |
| `2BC6` | B[4] | `Leaving` | 1 = Verkäufer geht (Feuer oder Vorrat < 10) | G – `0321`, `0FB9` |
| `3D1C` / `43EE` | W s | `HighestBuyer` / `LowestSeller` | höchste Käufer- / tiefste Verkäuferposition | G – `0586`, `060A` |
| `3D18` | B s | `Settling` | Bilder ohne Handel nach Änderung der Extreme (3) | G – `0586`, `0323` |
| `3CAE` / `3D46` | B s | `DealTimer` / `DealDelay` | Bilder bis zum nächsten Handel / Abstand (Eh, sinkt bis 5) | G – `0323` |
| `2BCC` | B | `DealParties` | Spielerbits `08C6` der Parteien des letzten Handels | G – `0323`, `0FB9` |
| `44F2` / `4106` | W s | `DealSeller` / `DealBuyer` | Parteien des Handels, Käufer 10 = keiner | G – `060A`, `03B2` |
| `2BB4` / `2BB6` | W s | `SellerCursor` / `BuyerCursor` | Reihum-Zeiger der Suche | G – `060A` |
| `023C` | W s | `PushCounter` | Stöße über 63h bis zum Anheben des Grundpreises (Startwert 2) | G – `04FC` |
| `2BBA` | W s | `CountdownMark` | Countdown beim Start / beim letzten Handel | G – `024A`, `0695`; W |
| `2BD0` / `2BDA` | W[4] | `PanelX` / `PanelY` | Lage der Spielertafeln | G – `051A` |
| `2BCE` / `2BCA` / `2BD8` / `2BE2` | W | `ScaleX` / `CountdownX` / `LabelX` / `LabelY` | Lage von Leiter, Countdown, Beschriftung | G – `051A` |
| `2BE4` | W | `ShownBasePrice` | zuletzt gezeichneter Grundpreis | W – `0B1F` |
| `0634` / `0642` | W[4] / B | `ShownStock` / `BarColour` | gezeigter Vorrat / 10 in Bild 1; Balkenfarbe DAh auf Karte 1 | G – `134D` |
| `0258` / `025A` | B[2] | `BuyerPanelY` / `SellerPanelY` | y der Tafeln je Bild (initialisiert 32h, 0 / 0, 63h) | G – `051A` |
| `3780` / `2F16` / `2F18` | W | `Countdown.*` | Countdown, Takt, Takt-Rest | G – `21FD:04E5`, `04F9` |

**Szene (nur gelesen, Konstanten; Namen in `src/Traders.Presentation/Trade/TradeScene.cs`):** `0240` B[2][4] Zahlenfarbe je Bild
und Spieler · `0248` W[5] / `0252` W[3] Farben der Waren 2 / 3 (`04BD`) · `025C`/`025E`/`0260` B[2] Farbe Vorrat, Farbe und y des
Grundpreises · `0262`/`0276`/`028A` W[10] Atlas-x, -y, Höhe der Teile von `0BD9` · `029E` B[2][4] Randfarben `0C8A` ·
`02AA` W[4], `02B2`/`02B6`/`02BA`/`02BE` B[4] Warenzeichen `1050` · `02C2` (4 × 19h), `0326` (4 × 64h), `04B6` (4 × 0Ah), `04DE`
(4 × 22h) Haufenzeilen der Spieler 0–3 (`1163`) · `0566` B[4] Haufenfarbe · `056A`/`05CF` B[101] Endpunkte der Balken `146B` ·
`063C`/`0640` B[4] Balkenfarben `134D` · `02A6` Fernzeiger auf den Zahlenpuffer `0644` (Ausgabe).

## Runde 8, Spur `einkauf` (Einkauf `08CF:0004`)

Befund [08CF_0004_einkauf.md](../routines/08CF_0004_einkauf.md). G = belegt durch Differenztest, W = Bedeutung aus dem Ablauf
gefolgert. Namen in `Traders.Core.Purchase.PurchaseLayout`. Indizierte Ziele aus dem Disassembly (`[bx+…]`), nicht aus
`callgraph.py`.

| DS-Offset | Typ | Name (C#) | Bedeutung | Beleg |
|---|---|---|---|---|
| `43B2` | B[4] u | `Mode` | 0Ah Straße, 0Bh Raketen, 0Ch Öl, 0Dh Lotto, 0Eh Edelsteine (ergänzt §14) | G – `039C`, `04BA`, `0CD5`; W |
| `15AB` | B[4] s | `Shop` | Laden vor dem Spieler: 0 Lotto, 1 Öl, 2 Raketen, 3 Edelsteine | G – `029A`, `039C`, `0455` |
| `3308` | B[4] s | `Action` | wie §10; vom Computer in `03E5`/`04BA`/`069F`/`09F1`/`0CD5` gesetzt (FFh = kein Ziel) | G – `03E5`, `0455` |
| `4BCA` / `3022` / `4BD2` | W[4] s | `Direction` / `MoveTimer` / `MoveFrame` | Gehrichtung 0/1, Gehdauer 21h (`0609:1FE0`) | G – `029A` |
| `3786` | DW[4] s | `MoveDelay` | Eingabesperre: 3 nach Öl, 64h nach Edelstein, 12Ch nach Lottozahl (Mensch); Computer kauft Öl erst bei ≤ 0 | G – `04BA`, `0886`, `0E53` |
| `1586` | B[4] s | `Picks` | getippte Lottozahlen 0 … 4 | G – `0BE1`, `0E53`, `0F4D` |
| `158A` | B[4][4] | `Tickets` | getippte Zahlen (1 oder 2), Schritt 4 | G – `0E53`, `0F4D` |
| `159C` | B[4] s | `GemDeals` | Edelsteingeschäfte im Einkauf (Grenze des Computers) | G – `069F`, `0914` |
| `15A0` / `15B0` / `15B8` / `15B2` | W s | `LottoX` / `OilX` / `RocketX` / `GemX` | x der Läden (40h Abstand; `15B0` nur mit Ölverbrauch) | G – `0082` |
| `15A2` | B s | `DrawState` | Ziehung: FFh wartet, 0 … 3 Kugeln, F6h keine mehr | G – `0F4D`, `161F` |
| `15A3` / `15A7` | B[4] | `Matches` / `Balls` | Treffer je Spieler (4 = Gewinn) / gezogene Kugeln 1 oder 2; `15A3[4…7]` liest `15A7` | G – `0F4D`, `115C` |
| `15AF` | B s | `BlinkStep` | Blinkschritt der Gewinner 0 … 7 | G – `115C`, `0F4D` |
| `159A` | W s | `NextBall` | Countdownwert der nächsten Kugel (`3780` − 5, − 1, − 2) | G – `0F4D`, `161F` |
| `15B4` | W s | `ComputerDelay` | Bilder bis zur nächsten Computerentscheidung 5 … 0 | W – `01BD`; G – `069F`, `09F1`, `0CD5` |
| `15B6` | W s | `TextX` | x der Preiszeile (`1543`) | G – `1348`, `1543` |
| `00E8` / `00EA` | W s | `ShownTime` / `BlinkTimer` | zuletzt gezeigte Zeit bis zur Kugel (init. FFFFh) / Blinktakt (init. 1, dann 2) | G – `0F4D`, `115C` |
| `00E4` | B[4] | `FigureX` | x-Versatz der Figur am Lottoladen (init. 25h, 1Dh, 23h, 19h) | W – `00CD` |
| `08ED` | B[…] | `Preset` | vorgegebene Lottozahlen (init. FFh = keine); `08EE` ≠ FFh lässt jeden mit Losgeld spielen | G – `0CD5`, `0DEF`, `0F4D` |
| `3046` | W[4] s | `RocketShortfall` | fehlende Raketen (§11); Computer kauft nur darunter | G – `0AF0` |
| `4520` / `4B98` | W[4] s / W s | `Oil` / `ShopOil` | Öl (max. 28h) / Ölvorrat des Ladens | G – `04BA`, `05F1` |
| `4BAC` / `37AF` / `300E` | W[4] / B[4] / W[4] | `Rockets` / `Gems` / `GemCredit` | Raketen (max. 10), Edelsteine (max. 10), Edelsteinkonto (max. 2000) | G – `09F1`, `178D`, `0886` |
| `4BC2` | B[4] | `FatMikeFlag` | = 1: Computer kauft Raketen wie bei < 3 Raketen + Basen | G – `0AF0` |

## Runde 8, Spur `fatmike` (Fat Mike `1F63:000F`, Wettbewerb `1F63:0B03`)

Befunde [`1F63_000F_fatmike.md`](../routines/1F63_000F_fatmike.md), [`1F63_0B03_wettbewerb.md`](../routines/1F63_0B03_wettbewerb.md);
Namen in `src/Traders.Core/FatMike/FatMikePhase.cs` und `ContestPhase.cs`. Belege: Differenztests
`tests/Traders.Tests/FatMike/FatMikeDifferentialTests.cs`, `ContestDifferentialTests.cs`.

| Offset | Typ | Name | Bedeutung | Beleg |
|---|---|---|---|---|
| `08D4` | W | `PictureNumber` | Bildnummer `V-FATMIK` (initialisiert 3) | G – `000F` |
| `08ED` | B[4] u | `Lamps` | Masken 1/2 der Blinkfolge (`0305`) | G – `0305` |
| `4890` | B[4] u | `Bidders` | 1 = größter Vorrat 3 und `4BC2` = 0 (`0C82`); im Wettbewerb die Gewinner (Preis, Blinken, Modus 5) | G – `0C82`, `0B03` |
| `4BC2` | B[4] u | `FatMikeFlag` | 1 = hat zuletzt nicht gezahlt; präzisiert §2 Nr. 9 | G – `0711` |
| `54B1` | B u | `Contest` | 0 Fat Mike, 1 Wettbewerb `0B03` | G – `000F` |
| `54B2` / `54B4` / `54B8` | W s | `Unit` / `Base` / `Price` | Leitereinheit max/120 + 1 / Grundpreis ≤ 30 / laufender Preis | G – `01B2`, `0390` |
| `54BA` | W[4] s | `Paid` | 1 = hat gezahlt | G – `04D8`, `0711` |
| `54C2` / `54CC` / `54D2` | W[3] / W / W s | `Lots` / `Lot` / `Cheapest` | Preise der Runden / Runde / billigste Runde | G – `0390` |
| `54C8` | B u | `NobodyPaid` | 1 = in keiner Runde gezahlt | G – `000F` |
| `54CA` | W s | `Slide` | x der Objektreihe | G – `13FE`, `147B` |
| `54CE` | B[4] u | `Verdict` | 0 nichts, 1 Ernte vernichtet, 2 Felder verloren | G – `0711`, `08DA` |
| `1020` / `101C` / `103C` | B[4] / B[4] / Text | `Patience` / `Colour` / `PaidText` | Angebotsdauer je IQ (6, 5, 4, 2 Runden zu 31 Bildern) / Textfarbe 0Ch…0Fh / „OK“ | G – `04D8` |
| `102E` / `1035` | B[7] | `RowX` / `RowY` | Versatz der sieben Objekte | G – `143E` |
| `1024` / `1026` / `1028` / `102C` | B[2] / B[2] / W[2] / B[2] | `LampX` … | Atlaslagen der Lampen `0FDF` | G – `0FDF` |
| `3B64` / `3B74` / `3B94` / `3BA0` | Fernzeiger | `NoticeLines` … | Texte der Tabelle `37CC` (Hinweis, Feldhinweise, Ausschluss) | G – `000F`, `079A`, `0F46` |
| `15BE` | W s | `ListCount` | Listeneinträge `0A96`; `04D8` liest > 0 (im Port zählt der Kern) | G – `04D8` |
| `3B6C` / `3B70` | Fernzeiger | `HeadingText` / `FieldText` | Wettbewerb: Überschrift (`0A69:0243`) / Text statt Preis, wenn ein Feld gewonnen wird | G – `0B03` |
| `1014` | W[4] | `PlayerColours` | rgb12 der Spielerfarben 0Ch…0Fh, mit denen `11AD` nach dem Blinken zurücksetzt | G – `11AD` |
| `37E0` | Fernzeiger[2] | `SecondRoundLines` (`Planting`) | Zeilen des Hinweises der zweiten Feldrunde (Tabelle `37CC`, Einträge 5 und 6) | G – `0609:0204` |
| `43B2` / `43FA` / `1582` | B[4] / B[4] / B | – | `0609:0080`: Gewinner Modus 5 und Kaufmerker 1, übrige Modus 6; `1582` = 1 | G – `0609:0080` |

## Runde 9, Spur `effekte` (Tonsteuerung `218B`/`21A7`)

Ergänzt §9 (`0910`, `10A3`, `10A5`, `10A9`); Befund `docs/routines/218B_21A7_ton.md`, Port `src/Traders.Core/Sound/SoundControl.cs`.

| DS-Offset | Breite | Name (Port) | Bedeutung | Beleg |
|---|---|---|---|---|
| `1094` | Fernzeiger[3] (Offset, Segment) | `PieceNames` | Dateinamen der Stücke (ab `DS:10AB`), Fn 00h bekommt BX:CX = Segment:Offset | G – `218B:00DC`; P |
| `10A0` | B[3] s | `PieceDisk` | Diskette je Stück, nur an die Dateisuche `186C:0A6C` (im Port entfallen) | G – `218B:00BA` |
| `10A5` | W | `MusicPlaying` | bleibt gesetzt, wenn `0153` den Treiber stehend findet (Fn 12h → 0) | G – `218B:0167`; P |

## Runde 9, Spur `ende` (Ende und Abend `20B3:0053`–`0D77`)

Befund [`20B3_008E_ende.md`](../routines/20B3_008E_ende.md); Namen in `src/Traders.Core/Ending/EndScreens.cs` und
`src/Traders.Core/Pixels/EndingPage.cs`. Belege: `tests/Traders.Tests/Ending/EndScreensDifferentialTests.cs`,
`EveningRulePageTests.cs`, `WaitKeyDifferentialTests.cs`.

| Offset | Typ | Name | Bedeutung | Beleg |
|---|---|---|---|---|
| `08CE` / `08E0` / `08DC` | W | `FieldPictureNumber` / `EveningPictureNumber` / `RankPictureNumber` | Bildnummern Schlussbild (`V-ANBAU`), Abend (`V-ABEND`), Rangbild (`V-ENDE`) | G – `008E`, `0525`, `0863` |
| `1040` / `1060` | Palette | `RankPalette` / `EveningPalette` | Argumente von `21FD:026C` | G – `0525`, `0863` |
| `1080` | W s | `Walk` | Lage auf dem Gang des Siegers (initialisiert 0, nicht zurückgesetzt) | G – `09D4`, `0863` |
| `1082` / `1084` | W s / B s | `BlinkTimer` / `BlinkStep` | Takt (initialisiert 1) und Bild 0…3 des Blinkens | G – `0986` |
| `1085` / `1089` | W s | `Bow1` / `Bow2` | Bilder der Verbeugungen (initialisiert 15) | G – `0A9F`, `0B40` |
| `1087` | W s | `Drink` | Bilder des Trunks 0…100 | G – `0ADC` |
| `108B` | W[2] | `ObjectFrames` | y der Objektbilder je Argument von `0D0D` | G – `0D0D` |
| `108F` | Text | `RankText` | „RANK“ | G – `05B4` |
| `0D17` | W[91] s | `CosTable` | cos 0…90° skaliert | G – `0B7D` |
| `10DA` / `10DD` | B[3] | `Advances` / `Heights` | Vorschub und Höhe je Schrift (6, 14, 5 / 5, 12, 5) | G – `21FD:05DB` |
| `3CA8` / `34A4` | W | `Advance` / `Height` | Vorschub und Höhe der gewählten Schrift | G – `21FD:05DB` |
| `54D4` | W u | `RankState` | Zustand des Rangbilds 0…7 | G – `0863` |
| `54D6` | B s | `Winners` | Sieger (Rang 0), danach verbleibende Trunkrunden | G – `008E`, `0ADC` |
| `115D` | B u | `MakeCount` | Make-Zähler; `27C6:00D2` = 0 | G – `27C6:00D2` |
