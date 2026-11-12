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
| 34 | `348A` | W | s | [i·2] | `StationValue` | Zufall | −1 (½), 10 (¼) oder 0–3 | G (Erzeugung), Bedeutung W |
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

## 8. Runde 4, Spur `programm` (Programmablauf)

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
| `10A9` | W | – | **aktuelles Musikstück** (FFFF aus der EXE, 0 nach `218B:0094(0, 0)` bei `0DFD:02FE`); kein Teil der Ziffernblock-Tabelle | S `218B:00EE`; P |
