# Bestandsaufnahme `T.EXE` gegen den bisherigen Stand

Spur ⓪ `bestand` (Runde 2), Stand 11.11.2026. Reine Analyse, kein Port. Geprüft gegen `T.EXE` mit SHA-256 `b73a2170…`
(`tools/originale/SHA256SUMS.dos`), `main` `9995b6e`, `dev/ap1-refemu` `a12822d`, `dev/ap2-vga` `5a9f178`, `dev/ap5-phasen` `8ed0d34`.
Adressen = Ladebild-Segment:Offset, `DS` = DGROUP `28DC`. Textnummern = Zeile von `ENGLISCH.TXT` (0-basiert).

**Vorgehen:** eigene Auswertung von MZ-Kopf, Relokationen und Disassemblierung (`tools/segmap.py`,
`fetch-reference.sh disasm`) – Far-Aufrufe, Aufrufer, Zufalls-, Grafik-, Port- und Interruptstellen, DS-Zugriffe je Funktion –,
erst danach Abgleich mit `docs/analyse/*`, `docs/routines/*`, `STATUS.md` und den Altbranches. Aussagen über Verhalten sind
mit einer RefEmu-Probe belegt (Anhang A); der Rest ist statisch gelesen und so gekennzeichnet.

**Belegstufen:** **P** = RefEmu-Probe (Anhang A) · **S** = statisch aus `T.EXE` gelesen, nachzählbar · **V** = Vermutung aus
Aufrufprofil und Datenzugriffen.

## 1. Karte von `T.EXE`

### 1.1 Datei und Ladebild (S)

| Merkmal | Wert |
|---|---|
| Datei / Kopf / Ladebild | 204000 Byte / 0x3600 / 0x2E6E0; keine Daten hinter dem Ladebild, `minalloc` = 0 |
| Einstieg | `0000:0000` (Startcode), ruft `1777:000C` (`main`) aus `0000:00E3` |
| Code | `0000`–`28DB`, 0x28DC0 Byte, 45 Segmente (identisch mit `docs/analyse/segments.md`, neu erzeugt und verglichen) |
| DGROUP | `28DC`, 0x58A0 Byte bis zum Stapel (`2E66`, 0x80 Byte); letzte von null verschiedene Datenstelle `DS:1564`, der Rest steht als Nullen **in der Datei** |
| Relokationen | 3394: 3307 im Code, 87 in den Daten (67 Zeiger in die DGROUP, 17 auf `0A96`, 3 auf `0000`) |
| Funktionen | 891 Far-Einsprünge (`segmap.py`); mit Near- und `push cs`-Aufrufen 919 aufgerufene Funktionen, mit allen Prologen rund 1100 (Zählung `tools/callgraph.py` aus `dev/ap5-phasen`) |
| Zufall `21FD:01BA` | 149 Aufrufstellen in 17 Segmenten: 145 Far-Aufrufe plus 4 Aufrufe innerhalb von `21FD` (in `segments.md` nicht mitgezählt) |

Segmentgrenzen sind Paragraphen: Die erste Funktion eines Segments beginnt bis zu 15 Byte nach der Segmentbasis, die letzte des
vorigen kann bis dorthin reichen; Größen sind deshalb auf ±15 Byte genau.

### 1.2 Segmente

Spalten: Far-Einsprünge nach `segmap.py`; Fkt. = aufgerufene Funktionen; Zufall = Aufrufstellen von `21FD:01BA`; Grafik =
Aufrufstellen nach `0A96`/`0A69`/`23CD`. Rolle und Beleg sind Stufe S (Aufrufer, Aufrufziele, Texte, geschriebene Felder), soweit
nicht anders vermerkt. „Stand `main`“ nach §2.4 (Codesuche).

| Segment | Größe | Far-Einspr. | Fkt. | Zufall | Grafik | Gruppe | Rolle | Beleg | Stand `main` |
|---|---:|---:|---:|---:|---:|---|---|---|---|
| `0000` | 0x5350 | 76 | 93 | 0 | 0 | Laufzeit | Turbo-C-Laufzeit: Startcode (ruft `1777:000C`), Long-Arithmetik, Zeichenketten, Dateien, Speicher, `time()`; dazu Tonroutinen `06FE/0724/0750` (Effektquelle bei `DS:0910` = 1, Zuordnung offen). Einstieg: – | 76 Einsprünge, keine Aufrufe in Spielsegmente außer `0000:00E3`→main | – (Laufzeit, nicht zu portieren) |
| `0535` | 0xD40 | 24 | 24 | 0 | 24 | Roboterkampf | Roboterkampf: Aufstellung vor dem Kampf (Bildschirm, Spielerzug über `0E9E`, KI über `12C8`), Anzeige der Kampfwerte. Einstieg: `000A`, `078A`, `08ED`, `06CE` | Aufrufer nur `1C68`/`0EDE`; schreibt Kampfwerte `33DC/3016/40EC` | fehlt |
| `0609` | 0x21C0 | 56 | 57 | 3 | 73 | Phasen/Regeln | Anbauphase und gemeinsame Spielfeldschicht: Feldraster zeichnen, Cursor, HQ/Feld kaufen, Feldart setzen (`10D2`), Kopfzeile; wird von fast allen Phasen mitbenutzt. Einstieg: `0004` (Phase), `1178` (Obergrenzen aus Feldzahl), `10D2`, `1256`, `1879`, `1ED9` | 94 Aufrufe aus 12 Segmenten; Texte 6–8; schreibt Besitz/Art/Cursor | fehlt |
| `0825` | 0xAA0 | 16 | 16 | 0 | 43 | Phasen/Regeln | Wochenbilanz (Darstellung) und Vermögens-/Rangberechnung. Einstieg: `000A`, `06CB`, `07C4`, `0813` | Texte 9, 10, 15; schreibt `2F06`, `3D42`, `411C` | fehlt |
| `08CF` | 0x19A0 | 42 | 42 | 9 | 35 | Phasen/Regeln | Einkauf (Laden): Öl, Raketen, Lotto, Edelsteine; Edelsteinkurs der Woche (`11D7`, 5 Zufallsaufrufe). Einstieg: `0004`, `11D7` | nur aus Wochenschleife; Texte 16–22; schreibt Bargeld, Kurs, Ladenvorrat | fehlt |
| `0A69` | 0x2D0 | 9 | 10 | 0 | 11 | Grafik | Zeichenhilfen über der Befehlsliste: Linien-, Rahmen- und Textvarianten, Meldungsfenster mit Warten. Einstieg: `000B`…`0288` | ruft nur `0A96` und Laufzeit; 10 Funktionen | fehlt |
| `0A96` | 0xD10 | 24 | 24 | 0 | 28 | Grafik | Grafik-Befehlsliste: 18 Einreihfunktionen und eine Sammelfunktion für 17 Befehlstypen (Sätze zu 0x1C Byte ab `DS:15C0`), Ausführung `06A8` über Far-Zeiger-Tabelle `DS:01F4`. Einstieg: `001B`, `0030`…`0686`, `06A8`, `0C3F` | 585 Aufrufe aus 21 Segmenten; 17 Relokationsziele in den Daten | fehlt |
| `0B67` | 0x380 | 8 | 8 | 0 | 14 | Grafik | Bildschirmseiten: Einrichten (Puffer anfordern, Treiber starten), Seitenwechsel `0238`, Warten `028F`, Seitenabgleich `0299`. Einstieg: `0002`, `0238`, `028F`, `0299` | Aufrufer `1777:04E4`, `21FD:051F/0590`, `0A96:0C3F`; 14 Treiberaufrufe | fehlt |
| `0B9F` | 0x8A0 | 16 | 16 | 0 | 4 | Phasen/Regeln | Handel: Ablaufsteuerung und Regeln (Angebote, Preise `4898`, Gebotsprüfung), ohne eigenes Zeichnen. Einstieg: `0002`, `06A0`, `078A` | `0002` aus Wochenschleife; 12 Rückrufe aus `0C29` | fehlt |
| `0C29` | 0x14A0 | 29 | 29 | 0 | 67 | Phasen/Regeln | Handel: Bildschirm und Bedienung (Auktion/Verkauf, zwei Bilder), bucht Bargeld und Vorräte. Einstieg: `0001` | 59 Grafikaufrufe; lädt Handelsbilder; Aufrufer nur `0B9F` | fehlt |
| `0D73` | 0x8A0 | 15 | 19 | 0 | 1 | System/Hilfen | Systemschicht: Interrupts ein-/aushängen (über `27C6`), Grafikkarten- und Tonerkennung, Speicher, Dateien lesen/schreiben (`059A`, `064E`), Warten `00D3`. Einstieg: `00D3`, `00F8`, `0712`, `04CA`, `059A`, `064E` | Aufrufer `1777:04E4`, `186C`, `2336`; 32 Laufzeitaufrufe | fehlt |
| `0DFD` | 0x340 | 5 | 5 | 0 | 7 | Phasen/Regeln | Programmstart nach der Initialisierung: Schrift und Masken laden, Ladebild, sechs Bilder vorladen, Texte laden, Code-Abfrage anstoßen. Einstieg: `000D` | `000D` einmal aus main; 14 Laderaufrufe | fehlt |
| `0E31` | 0x6D0 | 7 | 7 | 14 | 0 | Phasen/Regeln | Neue Partie: Land (zufällig oder fest), Stationen, Startwerte je Spieler. Einstieg: `0003`, `023D`, `0361` | 14 Zufallsaufrufe; schreibt den gesamten Startzustand | fehlt (nur Befund/Analyse) |
| `0E9E` | 0x400 | 5 | 5 | 0 | 0 | System/Hilfen | Eingabe je Spieler nach Steuerungsart (`DS:083C`) → Richtungs-/Knopfzustand. Einstieg: `000E`, `037A` | 12 Aufrufer (alle Phasen mit Spielerzug); liest `0840`, `083C` | fehlt |
| `0EDE` | 0x23F0 | 56 | 56 | 6 | 22 | Roboterkampf | Roboterkampf: Ablauf, Bewegung, Treffer, Beute. Einstieg: `0009`, `0322` | 56 Funktionen; Texte 23–24; Aufrufer `1C68`, `0535` | fehlt |
| `111D` | 0x860 | 7 | 7 | 12 | 21 | Phasen/Regeln | Katastrophen und Wochenereignis-Einblendung; verändert Bodengüte, Feldart. Einstieg: `0009` | Text 35; 12 Zufallsaufrufe; liest Schalter `2F30` | fehlt |
| `11A3` | 0x1250 | 15 | 15 | 2 | 0 | Computerspieler | Computerspieler: Wochenplanung in zwei Durchgängen (vor Anbau, vor Handel), reine Logik. Einstieg: `0000` | `0000(0/1)` aus Wochenschleife; keine Grafikaufrufe | fehlt |
| `12C8` | 0x950 | 12 | 13 | 0 | 0 | Computerspieler | Computerspieler: Roboter-Aufstellung, reine Logik. Einstieg: `000B`, `003B`, `0539` | Aufrufer `0535`, `1C68`; keine Aufrufe nach außen | fehlt |
| `135D` | 0xC00 | 14 | 14 | 0 | 0 | Computerspieler | Computerspieler: Anbauphase (Feldwahl, Cursorziel), reine Logik. Einstieg: `000E`, `0935`, `0AB3` | Aufrufer nur `0609` | fehlt |
| `141D` | 0xF20 | 17 | 17 | 3 | 0 | Computerspieler | Computerspieler: Handel (Gebote, Mengen), reine Logik. Einstieg: `0008`, `0464`, `09F3` | Aufrufer `0B9F`, `0C29`; 3 Zufallsaufrufe | fehlt |
| `150F` | 0x1C70 | 41 | 42 | 7 | 0 | Computerspieler | Computerspieler: Roboterkampf, reine Logik. Einstieg: `004C`, `012E`, `06E4` | Aufrufer `0EDE`, `16D6`, `1C68`; 7 Zufallsaufrufe | fehlt |
| `16D6` | 0xA10 | 12 | 12 | 1 | 0 | Computerspieler | Computerspieler: Roboterkauf, reine Logik. Einstieg: `000E`, `06F6` | Aufrufer nur `1C68` | fehlt |
| `1777` | 0x540 | 4 | 4 | 0 | 0 | Phasen/Regeln | Hauptprogramm: `main` (Kommandozeile, Init, Menü↔Partie), Wochenschleife, Wochenpreise, Initialisierung. Einstieg: `000C`, `01F8`, `02DC`, `04E4` | `000C` wird nur vom Startcode gerufen; 4 Funktionen | fehlt |
| `17CB` | 0xA10 | 9 | 9 | 12 | 10 | Phasen/Regeln | Wochenereignisse: Geld- und Feldereignisse mit Fenster. Einstieg: `0005` | 12 Zufallsaufrufe; bucht Bargeld; ruft `0609:10D2` | fehlt |
| `186C` | 0xC10 | 19 | 19 | 0 | 7 | System/Hilfen | Ressourcenlader: Bild *i* laden/zwischenspeichern (`0002`, Tabelle `DS:09CE`), entpacken (`01A9`), Speicherblöcke (`02C6`), Dateien lesen (`0AB1`, `0B0A`), Diskettenwechsel-Meldung. Einstieg: `0002`, `01A9`, `02C6`, `0AB1`, `0B0A` | 29 Aufrufer von `0002`; ruft Entpacker `2821` und Atlas-Umwandlung `23CD` | fehlt |
| `192D` | 0x1D60 | 74 | 74 | 17 | 99 | Phasen/Regeln | Menüs: Haupt-, Stufen-, Einstellungs-, Disketten-, Steuerungsmenü; Texttabelle aufbauen (`18B6`). Einstieg: `0007`, `0058`, `0592`, `18B6` | 74 Funktionen; schreibt alle Optionsfelder | fehlt (nur Befund/Analyse) |
| `1B03` | 0x1650 | 44 | 44 | 20 | 51 | Phasen/Regeln | Stationswahl, Raketenflug, Piraten, Stationsertrag. Einstieg: `0009` | 20 Zufallsaufrufe; schreibt Station, Öl, Edelsteinkonto, Bargeld | fehlt |
| `1C68` | 0x1870 | 40 | 41 | 0 | 56 | Roboterkampf | Roboterphase: Steuerung (Schalter, Zyklus, Woche) und Roboterkauf-Bildschirm. Einstieg: `000B` | `000B` aus Wochenschleife; Text 226; lädt Kampfbild | fehlt |
| `1DEF` | 0x8F0 | 14 | 14 | 6 | 19 | Phasen/Regeln | Zufällig ausgelöste Zusatzphase auf dem Spielfeld nach dem Handel (ändert Vorräte); genaue Regel offen. Einstieg: `000C` | 6 Zufallsaufrufe; erste Funktion entscheidet per Zufall | fehlt |
| `1E7E` | 0xE50 | 19 | 19 | 18 | 16 | Roboterkampf | Roboterkampf: Kampffeld erzeugen und Kacheln setzen (Raster `DS:48F4`). Einstieg: `0004`, `0DB3` | 18 Zufallsaufrufe; Aufrufer `1C68`, `0EDE`, `0535` | fehlt |
| `1F63` | 0x1500 | 27 | 27 | 13 | 57 | Phasen/Regeln | Fat Mike (Mietrunde) und Wettbewerb nach dem Einkauf (`0B03`). Einstieg: `000F`, `0B03` | Texte 231, 232, 244; 13 Zufallsaufrufe; schreibt Merker `4BC2` | fehlt |
| `20B3` | 0xD80 | 27 | 27 | 2 | 61 | Phasen/Regeln | Endprüfung (ein Vermögen ≥ Ziel oder Taste mit Scancode 44h), danach Abend- und Endbildschirme. Einstieg: `0005` | `0005` aus Wochenschleife, Rückgabe 1 = Partie zu Ende | fehlt |
| `218B` | 0x1C0 | 5 | 5 | 0 | 0 | Ton | Musik/Ton über residenten Treiber (`int 64h`): Initialisierung, Stück laden und starten, stoppen. Einstieg: `0002`, `0094`, `013A`, `0153` | 12 `int 64h`-Stellen; Namen der drei Musikdateien ab `DS:10AB` | fehlt |
| `21A7` | 0x80 | 3 | 3 | 0 | 0 | Ton | Toneffekt auslösen (`0054(a,b)`, 144 Aufrufstellen), Effektquelle wählen (Treiber, PC-Lautsprecher). Einstieg: `000D`, `0043`, `0054` | verweist auf `2850` und Laufzeit-Tonroutinen | fehlt |
| `21AF` | 0x4E0 | 7 | 8 | 0 | 0 | Grafik | Bewegte Objekte: 8 Sätze zu 0x1A Byte ab `DS:330C` einrichten, Bahn/Schritt berechnen, ein-/ausschalten. Einstieg: `000D`, `00CA`, `022B`, `0412`, `045E` | Aufrufer: alle Phasen mit Figuren; Long-Arithmetik | fehlt |
| `21FD` | 0x980 | 28 | 28 | 4 | 24 | System/Hilfen | Gemeinsame Hilfen: Zufall `01BA`, Wetter würfeln `021F`, Warten `0185`, Paletten/Blenden `026C…04DA`, Seitenwechsel `051F`, Eingabe-Warteschleife `0004`, Code-Abfrage `0687`, leere Marke `0977`. Einstieg: siehe Text | 491 Aufrufe aus 27 Segmenten | teilweise: `01BA` ✅ belegt; `0687` nur Befund; Rest fehlt |
| `2295` | 0xE0 | 4 | 4 | 0 | 0 | System/Hilfen | Zahl→Zeichenkette (`0001`), Einzelfarbe setzen (`006D`), Palette an die Karte (`00C5`). Einstieg: `0001`, `006D`, `00C5` | 4 Funktionen | fehlt |
| `22A3` | 0x930 | 15 | 15 | 0 | 13 | Phasen/Regeln | Produktion: Ertrag je Feld aus Art, Güte, Wetter, Station (`0003`), Wochenschwund der Vorräte auf 8/10 (`026B`), Erntedarstellung (`02D3`). Einstieg: `0003`, `026B`, `02D3` | schreibt Raster `48F4`, Vorräte; kein Zufall | fehlt |
| `2336` | 0x970 | 4 | 4 | 0 | 0 | System/Hilfen | Spielstand speichern/laden. Einstieg: `000A`, `0539` | 4 Funktionen; 52 Feldgruppen | portiert und belegt (`000A`, `0539`) |
| `23CD` | 0x3F90 | 25 | 26 | 0 | 0 | Grafik | Grafiktreiber in Assembler: Sprungleiste `0570…06E3`, vier Kartenteile (EGA, Tandy, VGA/Mode X, Tandy-Emulation), Objekt-Zeichnen `06E9/071C`, Atlas-Umwandlung `166A/3079`, Kartenerkennung `1715`. Einstieg: `04F6`, `0570…071C`, `166A`, `1715`, `3079` | 85 Selbstverweise (Vektortabellen); 179 Port-Ausgaben | fehlt |
| `27C6` | 0x5B0 | 15 | 15 | 0 | 0 | System/Hilfen | Interrupt-Handler (Timer, Tastatur, DOS-Vektorabfang), Joystick-Abfrage, Farbregister schreiben (`042E`), Tick-Zähler. Einstieg: `0000`…`0576` | 46 Port-Lesestellen; schreibt `117E` | fehlt |
| `2821` | 0x2F0 | 1 | 1 | 0 | 0 | System/Hilfen | PP20-Entpacker in Assembler. Einstieg: `0008` | einziger Aufrufer `186C:01A9` | vorhanden, unbelegt (`PowerPacker` aus Formatwissen, kein Differenztest gegen `2821:0008`) |
| `2850` | 0x8A0 | 1 | 1 | 0 | 0 | Ton | PC-Lautsprecher-Effekte in Assembler (eigene Daten im Codesegment, Warteschlange mit 16 Plätzen). Einstieg: `00CC` | über Segmentverweis aus `21A7:000D` eingehängt und direkt aus `21A7:0085` gerufen (R3 Spur 3, #39 R2); Ports 42h/43h/61h | fehlt |
| `28DA` | 0x10 | 0 | 0 | 0 | 0 | Laufzeit | 16 Byte, kein Code-Einsprung (Segmentmarke der Laufzeit). Einstieg: – | 1 Relokationsverweis aus `0000` | – (Laufzeit, nicht zu portieren) |
| `28DB` | 0x10 | 0 | 0 | 0 | 0 | Laufzeit | 16 Byte, kein Code-Einsprung (Segmentmarke der Laufzeit). Einstieg: – | 1 Relokationsverweis aus `0000` | – (Laufzeit, nicht zu portieren) |

### 1.3 Gruppen

| Gruppe | Segmente | Größe | Anteil | Far-Einsprünge |
|---|---|---:|---:|---:|
| Phasen/Regeln | `0609` `0825` `08CF` `0B9F` `0C29` `0DFD` `0E31` `111D` `1777` `17CB` `192D` `1B03` `1DEF` `1F63` `20B3` `22A3` | 0xED50 | 36 % | 392 |
| Roboterkampf | `0535` `0EDE` `1C68` `1E7E` | 0x57F0 | 13 % | 139 |
| Computerspieler | `11A3` `12C8` `135D` `141D` `150F` `16D6` | 0x5D40 | 14 % | 111 |
| Grafik | `0A69` `0A96` `0B67` `21AF` `23CD` | 0x57D0 | 13 % | 73 |
| System/Hilfen | `0D73` `0E9E` `186C` `21FD` `2295` `2336` `27C6` `2821` | 0x3520 | 8 % | 91 |
| Ton | `218B` `21A7` `2850` | 0xAE0 | 2 % | 9 |
| Laufzeit | `0000` `28DA` `28DB` | 0x5370 | 13 % | 76 |
| **Summe** | 45 | **0x28DC0** | 100 % | **891** |

Folgerungen (S): Die Computerspieler liegen in sechs Segmenten **ohne Grafik-, Ton- und Eingabeaufrufe** – auch nicht
mittelbar über die von ihnen gerufenen Funktionen in `0609`, `0825`, `1C68`, `22A3` (reine Logik auf dem Zustand). Der Roboterkampf umfasst mit seinen drei KI-Segmenten 0x87C0 Byte, gut ein Fünftel des Codes, und ist in den
Stufen 0–2 abgeschaltet (Tabelle `DS:0B90`). Vom Grafiktreiber `23CD` gehört nur der Teil ab `3042` zur VGA; EGA (`0797`), Tandy
(`1645`) und Tandy-Emulation (`3F0A`) machen nach dem Abstand dieser vier Einstiege rund zwei Drittel des Segments aus (V).

### 1.4 Programmablauf (S, Reihenfolge durch Probe P1 bestätigt)

`main` (`1777:000C`): Kommandozeilenschalter für Grafikkarte (vier Arten → `DS:090E` = 1 VGA, 0 EGA, 2 Tandy, 63h
Tandy-Emulation; Vorgabe −1 = erkennen), PC-Lautsprecher (`DS:0910` = 0) und einen Schalter `DS:0912`; dann `1777:04E4`
(Initialisierung), `0DFD:000D` (Start, Code-Abfrage), danach Schleife Hauptmenü `192D:0007` → bei `DS:3CCC` = 1 neue Partie
`0E31:0003` → Wochenschleife `1777:01F8` (Rückgabe 1 = Partie beendet).

Wochenschleife `1777:01F8` – vollständige Aufruffolge einer Woche:

| # | Aufruf | Inhalt | Zufall | Bildschirm |
|---:|---|---|---|---|
| 1 | `1777:02DC` | Wochenpreise (Öl, Raketen, Lotto, Land, Ladenvorrat) | nein | nein |
| 2 | `0825:07C4` | Vermögen in den Verlauf schreiben | nein | nein |
| 3 | `0825:000A(1)` | Wochenbilanz; danach Abbruch ins Menü, wenn Taste mit Scancode 0Eh gedrückt (`DS:3D55`) | nein | ja |
| 4 | `111D:0009` | Katastrophen | ja | ja |
| 5 | `0609:1178` | zwei Obergrenzen aus der Zahl der Produktionsfelder (`DS:2F26`, `DS:48A2`) | nein | nein |
| 6 | `11A3:0000(0)` | Computerspieler: Planung vor dem Anbau | ja | nein |
| – | `21FD:0977(0)` | leere Marke | | |
| 7 | `0609:0004` | Anbau | ja | ja |
| 8 | `17CB:0005` | Wochenereignisse | ja | ja |
| – | `21FD:0977(1)` | leere Marke | | |
| 9 | `1B03:0009` | Stationen, Rakete, Piraten | ja | ja |
| 10 | `21FD:021F` | Wetter würfeln | ja | nein |
| 11 | `22A3:0003` | Produktion | nein | nein |
| 12 | `22A3:026B` | Vorräte aller Spieler auf 8/10 | nein | nein |
| – | `21FD:0977(2)` | leere Marke | | |
| 13 | `22A3:02D3` | Ernte zeigen und einsammeln | ja (über `0609:13C5`, P4) | ja |
| – | `21FD:0977(3)` | leere Marke | | |
| 14 | `1C68:000B` | Roboterphase (nur wenn eingeschaltet und im Zyklus) | ja | ja |
| 15 | `20B3:0005` | Endprüfung; bei Ende Abend-/Endbild, Rückgabe 1 | ja (Endbild) | ja |
| 16 | `0609:1178` | wie 5 | nein | nein |
| 17 | `11A3:0000(1)` | Computerspieler: Planung vor dem Handel | ja | nein |
| – | `21FD:0977(4)` | leere Marke | | |
| 18 | `0B9F:0002` | Handel | ja | ja |
| 19 | `1DEF:000C` | zufällig ausgelöste Zusatzphase | ja | ja |
| – | `21FD:0977(5)` | leere Marke | | |
| 20 | `1F63:000F` | Fat Mike | ja | ja |
| 21 | `08CF:11D7` | Edelsteinkurs der Woche | ja | nein |
| – | `21FD:0977(6)` | leere Marke | | |
| 22 | `08CF:0004` | Einkauf | ja | ja |
| 23 | `1F63:0B03` | Wettbewerb/Auswertung nach dem Einkauf | offen | ja |
| 24 | `DS:4B9A`++ | Wochenzähler | | |

Die sieben leeren Marken `21FD:0977(n)` sind feste, seiteneffektfreie Haltepunkte im Original – als Vergleichspunkte für
Mehrrundenproben geeignet (P1). Schritte 1, 2, 5, 10, 11, 12 und 21 sind Regeln ohne Bildschirm und Eingabe; die
Spalte „Zufall“ ist statisch gelesen (Aufrufstellen im Modul), für Woche 0 des Referenzlaufs durch P4 gedeckt.

**Zufall hängt an der Darstellung (P4, P5):** Das Original zieht Zufallszahlen auch unabhängig von Spielentscheidungen.
Belegt: Im Hauptmenü ändert sich der Zufallszustand mit der Verweildauer (P5), und die gemeinsame Warteschleife `21FD:0004`
ruft im Fat-Mike-Modus je Durchlauf `1F63:113E` mit einem Zufallsaufruf (S; 125 Aufrufe in Woche 0, P4). Weitere
Darstellungsstellen mit vielen Aufrufen in P4 – Raketenphase `1B03:1178`/`0A62`, Zusatzphase `1DEF:0173`/`01AF` – sind
nicht daraufhin gelesen, ob sie je Bild oder einmalig ziehen. Bei gleichen Eingaben zur gleichen emulierten Zeit ist der
Lauf bitgleich wiederholbar (P1, P5).

### 1.5 Datenbereiche der DGROUP

Initialisierte Daten `DS:0000–1564`, danach beim Start null (S). Zugriffe mit fester Adresse: rund 1040 verschiedene Adressen
ab `DS:0100`, davon 900 aus Spielcode; `DATENLAYOUT.md` benennt 79 davon (S, Zählung über Adressliterale; indizierte Felder
zählen einmal). Bereiche nach Zugriffsprofil – Stufe S für die Zeilen mit Adresse aus Relokation oder direkt gelesenem Code,
sonst V:

| Bereich | Inhalt | Zugriff aus | Stufe |
|---|---|---|---|
| `0000–00FF`, `0300–04FF`, `1200–15B9` | Laufzeitdaten (Startcode, Dateitabellen, Abschlussroutinen `1286…1290`) | `0000` | S |
| `0142`, `0143`, `0154…` | Textabstand, Schattenversatz, Farbtabellen für die Nicht-VGA-Karten | `0A96` | S |
| `01F4–0237` | 17 Far-Zeiger: Ausführung der Grafikbefehle Typ 0–16 (`0A96:06E4…0BFB`) | `0A96:06A8` | S |
| `0200–02FF`, `0500–05FF` | Tabellen des Handelsbildschirms | `0C29`, `0B9F` | V |
| `0600–07FF` | Dateinamen Schrift/Masken (`0678`, `0681`), Tabellen des Roboterkampfs | `0DFD`, `0EDE`, `12C8`, `150F` | V |
| `082E`, `0830` | Nummer der sichtbaren/verdeckten Bildschirmseite | `21FD:051F` | S |
| `083C–0843` | Steuerungsart und Mensch/Computer je Spieler | `192D`, `0E9E`, alle KI-Segmente | S |
| `08C6–08E3` | Bitmasken 01h…80h (`08C6`), Bildnummern der zehn Bilder (`08CE…08E0`) | alle Phasen | S |
| `08E4–0900`, `0902` | Optionen der Partie (wie `DATENLAYOUT.md` §2), Gütetabelle | `192D`, `0E31`, `22A3` | S |
| `090E`, `0910`, `0912` | Grafikkarte, Tonart, Kommandozeilenschalter | `1777`, `186C`, `21A7`, `0A96` | S |
| `09C8`, `09CE–0A8B` | aktuelles Bild; Ressourcentabelle 10 × 19 Byte (zwei Namenszeiger, Byte +8 Diskette, Byte +9 Kennzahl 1–3, Far-Zeiger +0Fh auf den geladenen Puffer) | `186C` | S |
| `0B72–0BC6`, `0CAA–0D12`, `0EAF–0EBB`, `0F74–0F80`, `1096–109E`, `10E2–10EE`, `14E6` | Zeigertabellen in die DGROUP (Menü, Stationen, Roboterkauf, Kampffeld, Ende) | `192D`, `1B03`, `1C68`, `1E7E`, `20B3` | S (Relokation), Inhalt V |
| `0B90–0BAF` | Stufentabelle (6 Optionen × 4 Stufen) | `192D:0592` | S |
| `0DE7…` | zwölf Stationsnamen | `1B03` | S |
| `10A5`, `10A7`, `10AB…` | Musikmerker (V), Ton an, Namen der Musikdateien | `218B`, `21A7` | S |
| `10D2`, `10D8` | Zufallszustand, Blendfarbe | `21FD` | S |
| `117E`, `1180`, `113E…` | Tickzähler, Name des Spielstands | `27C6`, `2336` | S |
| `15BA–15BF`, `15C0…` | Zeiger und Zähler der Grafik-Befehlsliste, Liste selbst | `0A96` | S |
| `2B00–2DFF` | Arbeitsdaten Handel und Handels-KI | `0C29`, `0B9F`, `141D` | V |
| `2E8E–3067` | Spielzustand Teil 1 (Felder, Stationen, Spielerwerte, Cursor) | Phasen, KI | S (soweit in `DATENLAYOUT.md`) |
| `3068–34A5` | Bewegte Objekte: 8 Sätze zu 0x1A Byte ab `330C`, davor und dahinter Bild-/Maskenzeiger und Bahndaten | `21AF`, `23CD` | S |
| `3746–37B0` | Landpreis, Roboterwerte, aktiver Spieler `37AA`, Edelsteine | Phasen | S |
| `37CC–3C8F` | 304 Far-Zeiger auf die Textzeilen (`37D0` = Zeile 0) | `192D:18B6`, alle Phasen | S |
| `3CCC`, `3CD2–3D46` | Menüergebnis, Bargeld, Grundeinheit, Vorräte, Rang | Phasen | S |
| `3D47–3DC6` | Tastenzustand je Scancode | `27C6`, Phasen | S |
| `3DCB–40CA`, `5017–5316`, `54E8–57E7` | Basis-, geladene und Arbeitspalette (je 768 Byte) | `21FD`, `186C` | S |
| `40CC–45FF`, `4898–48A2`, `4B98–4BF2` | Spielzustand Teil 2 (Preise, HQ, Verlauf `411C`, Roboterteile, Felder `4442`/`45DA`, Stationen) | Phasen, KI | S (soweit in `DATENLAYOUT.md`) |
| `48F4–49DD` | Raster 9 × 26 Byte, **mehrfach genutzt**: Produktionsergebnis (`22A3`, `11A3`) und Kampffeld (`1E7E`, `0EDE`, `0535`, `12C8`, `150F`) | s. links | S |
| `4C00–4CFF`, `5330–53FF` | Zustand des Ressourcenladers, Menükopien, Raketenphase | `186C`, `192D`, `1B03` | V |
| `5400–54E7` | Arbeitsdaten Fat Mike, Roboterkauf, Rakete, Kampffeld, Zusatzphase | `1F63`, `1C68`, `1B03`, `1E7E`, `1DEF` | V |
| `57F8` | Schreibzeiger Spielstand | `2336` | S |

### 1.6 Ressourcen und Dateien (S)

| Datei | Zugriff im Original | Bemerkung |
|---|---|---|
| `V-*.PP` / `E-*.PP` (je 10) | `186C:0002(i)` über `DS:09CE`; Namenswahl nach `DS:090E`; entpacken `186C:01A9` → `2821:0008`; Umwandlung in das Atlasformat der Karte `23CD:3079` (VGA) bzw. `23CD:166A` (andere Karten, V) | sechs Bilder werden beim Start vorgeladen (`0DFD`), `MENU`, `ENDE`, `KAMPF`, `ABEND` bei Bedarf |
| `FONTS.PP`, `MASKS.PP` | `0DFD:0074`, Übergabe an den Treiber `23CD:058A` | Zeichenabstand nicht in der Datei, sondern `DS:0142` |
| `ENGLISCH.TXT` | `192D:18B6` (liest über `186C:0B0A`, baut `DS:37D0…`) | die Phasenmodule greifen direkt auf die Zeiger zu, teils mit festem, teils mit berechnetem Index |
| `TRAD*.CMF` (3) | `218B:0094` lädt, `int 64h` spielt | Treiber ist das getrennte Programm `FXDRIVER.EXE` (hier nicht kartiert) |
| `SAVEGAME.DAT` | `2336:000A` / `2336:0539` | wie `docs/routines/2336_000A_spielstand.md` |
| Toneffekte | `21A7:0054(a,b)`, 144 Aufrufstellen | drei Wege je nach `DS:0910`: Treiber, `0000:0750`, PC-Lautsprecher `2850` |

Welche Phase welches Bild lädt: `ANBAU` – `0609`, `0825`, `08CF`, `111D`, `1DEF`, `1F63`, `20B3`, `22A3` · `RAKETE` – `1B03` ·
`BERGE` – `111D` · `FATMIK` – `1F63` · `AHANDE`/`BHANDE` – `0C29` · `MENU` – `192D`, `21FD:0687`, `2336` · `ENDE`/`ABEND` – `20B3` ·
`KAMPF` – `1C68`.

## 2. Spiegel gegen den Bestand

### 2.1 Stand je Bereich

| Bereich (Original) | `main` | Altbranch | Urteil |
|---|---|---|---|
| Zufall `21FD:01BA` | `OriginalRandom`, Befund, Differenztest | – | **portiert und belegt** |
| Spielstand `2336:000A/0539` | `SaveGame`, Befund, Differenztest | – | **portiert und belegt** |
| 16-Bit-Division (Laufzeit) | `Int16Math`, Befund | – | portiert und belegt (Hilfsbaustein) |
| Zustand/DGROUP | `Dgroup`, `GameState`, Mapper; Test gegen die initialisierten Daten | – | vorhanden; benannt sind 79 von rund 900 Adressen des Spielcodes |
| PP20-Entpacker `2821:0008` | `PowerPacker` (aus Formatwissen, Tests gegen Prüfsummen) | – | **vorhanden, unbelegt** – kein Befund, kein Differenztest gegen das Original |
| Bildformat `186C:0002`, `23CD:3079` | `VgaImage` (Kopf bei 768 berücksichtigt) | `ap2`: Atlas-Umwandlung `3079` mit Test | vorhanden, unbelegt (`main`); `ap2` s. u. |
| Schrift, Masken, Texte, Ressourcentabelle | `BitmapFont`, `MaskImage`, `TextTable`, `ResourceTable` | `ap2`: Textausgabe `23CD:05DD` | vorhanden, unbelegt (Formate, keine Originalroutine) |
| Musikdateien | `MusicModule` (nur Kopf) | – | teilweise, unbelegt |
| RefEmu | CPU, DOS/BIOS/VGA, Start bis Hauptmenü | `ap1`: Start mit residentem Tontreiber, Tastendruck mit Haltezeit, Code-Haken, Aufrufspur | `main` belegt bis Hauptmenü; `ap1` läuft mindestens vier Wochen (P1), bisher nur durch einen 15-Sekunden-Test gedeckt |
| VGA-Treiber `23CD` ab `3042` | – | `ap2`: 15 der 19 Vektorroutinen, Textausgabe, Atlas-Umwandlung; 13 Differenztests | **vorhanden, unbelegt** – Tests grün (P6), aber kein Befund, keine Marker, `[Fact]` statt `[GameDataFact]`, fester Pfad `original/dos` |
| Treiber: Linie (`378B`, `38F2`), Objekt zeichnen/zurückschreiben (`3AAB`, `3C42`, `06E9`, `071C`) | – | – | **fehlt** |
| Befehlsliste `0A96`, Hilfen `0A69`, Seiten `0B67`, Objekte `21AF`, Paletten/Blenden `21FD:026C…04DA`, Farbregister `2295:006D/00C5` | – | `ap2`: nur Analyse `GRAFIK.md` | **fehlt** (Analyse stimmt in den Stichproben, §2.3) |
| Phasenfolge | – | `ap5`: `tools/callgraph.py`, erzeugte Ausgabe, Experiment | Analyse stimmt mit §1.4 überein; Phasenmaschine **fehlt** |
| Hauptprogramm, Menüs, neue Partie, alle Phasen, Wochenregeln | – (nur Analyse in `DATENLAYOUT.md`) | – | **fehlt** |
| Computerspieler (6 Segmente), Roboterkampf (4 Segmente) | – | – | **fehlt**, auch keine Analyse |
| Eingabe `0E9E`/`27C6`, Systemschicht `0D73`, Lader `186C` | – | – | **fehlt** (im Port durch Host zu ersetzen) |
| Ton `218B`, `21A7`, `2850`, `FXDRIVER.EXE` | – (Aufrufe werden im RefEmu protokolliert) | `ap1`: OPL-Spur | **fehlt**; `FXDRIVER.EXE` ist nirgends kartiert |

### 2.2 Altbranches: Übernahmeprobe (lokal, Wegwerf-Worktrees auf `main` `9995b6e`)

| Branch | `git cherry-pick` | Ergebnis nach Übernahme | Für die Spur zu tun |
|---|---|---|---|
| `dev/ap1-refemu` | `940e4b0`, `a12822d` ohne Konflikt | baut; P1/P4/P5 liefen darauf | `GameRunTests`: `[Fact]` und Pfad `original/dos` → `[GameDataFact]`/`TestGameData`; `docs/uebergabe/AP1.md` und `ci/experiment.sh` nicht übernehmen; `OriginalProgram.cs` ist gemeinsame Datei |
| `dev/ap2-vga` | `f4eb83e` scheitert (4 Dateien, auf `main` schon anders gelöst) und ist mit `ff31159` ein Commit/Revert-Paar → beide auslassen; `0e95549`, `dab5889`, `5a9f178` ohne Konflikt | baut; Filter `Vga`: 20 bestanden (P6) | wie oben `[GameDataFact]`/Pfad; Kommentar verweist auf `docs/routines/23CD_*.md`, die es nicht gibt; `docs/uebergabe/AP2.md` nicht übernehmen; `GRAFIK.md` enthält nur Adressen und eigene Worte |
| `dev/ap5-phasen` | `8ed0d34` ohne Konflikt | `tools/callgraph.py` erzeugt die vier Ausgabedateien bytegleich neu | `docs/analyse/callgraph/*` enthält Originaltexte → nicht übernehmen (F1); `ci/experiment.sh` → `experiments/phasen/` |

### 2.3 Widersprüche und Korrekturen

| ID | Stelle | Bisherige Aussage | Befund | Beleg |
|---|---|---|---|---|
| B1 | `DATENLAYOUT.md` §2 (Kopf), §5; `STATUS.md` §3 Zeile `1777` | `1777:02DC` „benutzt die Uhr“ | Die Wochenpreise hängen nur von Bargeld der Spieler, Feldzahlen, Woche und Lotto-Einstellung ab: keine Uhr, kein Zufall, kein Host-Dienst | P2 |
| B2 | `STATUS.md` §3 | `1777` = „Wochenpreise“ | `1777` ist das Hauptprogramm: `main` `000C`, Wochenschleife `01F8`, Preise `02DC`, Initialisierung `04E4` | S: einziger Aufrufer von `000C` ist `0000:00E3`; Test `MachineTests` nennt denselben Einsprung |
| B3 | `DATENLAYOUT.md` §4 | `DS:2F26` „Betriebsmittel je Produktionsfeld (Standard 30)“ | `0609:1178` setzt je Woche zweimal `DS:2F26` = min(30, F/2 + 3) und `DS:48A2` = min(30, F/2 + 6), F = Produktionsfelder aller Spieler; 30 ist die Obergrenze, nicht der Standard | P3 |
| B4 | `STATUS.md` §3 | `22A3` = „Wettertexte / -vorführung“ | `22A3` rechnet die Produktion (`0003`), kürzt die Vorräte auf 8/10 (`026B`, ganzzahlig) und zeigt die Ernte (`02D3`) | P3 (Schwund), S (Schreibzugriffe) |
| B5 | `DATENLAYOUT.md` §4 | `DS:48F4` „Produktionsergebnis je Feld“ | derselbe Puffer ist im Roboterkampf das Kampffeld (fünf weitere Segmente schreiben/lesen ihn) | S |
| B6 | `STATUS.md` §3 | „?“ bzw. „wahrscheinlich“ bei `0535`, `0B67`, `0B9F`, `0C29`–`0E9E`, `0EDE`, `11A3`–`16D6`, `1C68`, `1E7E`, `21A7`–`21AF`, `2821`–`2850` | Rollen nach §1.2 | S |
| B7 | `segments.md`, `STATUS.md` §5 | Aufrufzahlen „×n“ = Far-Aufrufe | Aufrufe innerhalb eines Segments fehlen (z. B. `21FD:01BA` 145 statt 149); als Aufwandsmaß je Segment zu niedrig | S |
| B8 | `AGENTS.md` §4 „Szenen … verbrauchen keinen Zufall“ | – | Das Original zieht in Bildschleifen Zufall (§1.4); der Port braucht dafür Kern-Ereignisse je Bild, sonst weicht die Zufallsfolge ab | P4, P5 |

B1–B5 sind Korrekturen an `docs/analyse/DATENLAYOUT.md` bzw. `STATUS.md` und nach den Dateigrenzen dieser Spur nur hier
gemeldet. Kein Widerspruch zum **Code** der Altbranches gefunden; geprüft wurden dort nur die unten genannten Stichproben.

Bestätigte Stichproben (S, soweit nicht anders vermerkt): Segmentliste und Größen (`segmap.py` neu erzeugt, identisch) ·
Startwerte der initialisierten Daten aus `DATENLAYOUT.md` §2 (Spielfeld 8 × 6, Startgeld, Ziel, Stufe, Piraten, Lotto,
Zeitlimit, Steuerung, Zufallszustand 1, Stufentabelle, zwölf Stationsnamen, Zuordnung Feldart→Ware) · Ressourcentabelle
10 × 19 Byte · `GRAFIK.md`: Sprungtabelle `DS:01F4` mit 17 Zielen in genau der genannten Reihenfolge, Zuordnung
Einreihfunktion→Typ, Listenaufbau (`15BA/15BE/15C0`, Satz 0x1C, Typ bei +1Ah), Kartenwahl `23CD:04F6` (0/1/2/63h), 19 Vektoren ·
Phasenfolge und leere Marke aus `dev/ap5-phasen` (30 Knoten = §1.4) · Aufrufgraph `ap5` gegen eigene Zählung: 918 gemeinsame
Funktionen, Zufallsstellen 149 = 149.

### 2.4 Suchbelege für „fehlt“

Befehl je Segment `SSSS` und Stand `<ref>`: `git grep -IhoE '\bSSSS:[0-9A-F]{4}\b' <ref> -- src tests docs/routines`.
Ergebnis: In `src/` kommen auf `main` nur Adressen aus `21FD` (Zufall) und `2336` (Spielstand) als Port vor, dazu je ein
Kommentar zu `0A96`, `0E31` und `186C`; `dev/ap2-vga` fügt 27 Adressen aus `23CD` hinzu, `dev/ap1-refemu` und
`dev/ap5-phasen` keine. Für die übrigen 36 Spielsegmente ist die Suche in `src/` in allen vier Ständen leer; in
`docs/routines/` gibt es Befunde nur zu `21FD:01BA`, `21FD:0687`, `2336:000A/0539` und zur 16-Bit-Division. Gegenprobe für
die fehlenden Treiberteile: `git grep -nIE '378B|38F2|3AAB|3C42|DrawLine' origin/dev/ap2-vga -- src tests` ohne Treffer.

## 3. Empfehlung

### 3.1 Zuschnitt der Spuren ①–③

**① `refemu`** – Übernahme ist klein (2 Commits, konfliktfrei). Der eigentliche Wert für die folgenden Runden fehlt noch:

1. Übernehmen und auf die Regeln bringen (§2.2).
2. **Wochenlauf als Orakel:** Lauf mit vier Computerspielern bis zu einer Marke `21FD:0977(n)` in Woche *w*, Rückgabe des
   Datensegments. P1 zeigt, dass das trägt (4 Wochen, alle Marken, bitgleich wiederholbar). Dazu ein Test, der Reihenfolge
   und Wochenzähler prüft – er ersetzt den 15-Sekunden-Test als Nachweis.
3. **Laufzeit:** Eine Woche kostet rund 200 s emulierte Zeit, hier knapp 2 Minuten Rechenzeit; vier Wochen sind für
   `pruefen.sh --schnell` zu lang. Nötig ist ein Maschinenabzug zum Wiederaufsetzen an einer Marke oder ein nachweislich
   gleichwertiges Überspringen der Wartezeiten. Wegen B8 muss der Nachweis die Zufallsfolge einschließen.
4. Zustand setzen statt spielen: Aufruf einzelner Phasenroutinen aus einem vorbereiteten Datensegment (wie P2/P3) als
   dokumentierter Weg für Differenztests der Wochenregeln.

Eingaben eines menschlichen Spielers über mehrere Phasen sind damit noch nicht abgedeckt; das gehört zur jeweiligen
Phasenrunde, nicht zu ①.

**② `grafik`** – der Altbestand deckt nur den Treiberkern ab; darüber liegen vier ungeportete Schichten, die jeder Bildschirm
braucht. Vorschlag: ② auf den Treiber begrenzen und die Schichten als eigene Spur in R3 planen.

1. Übernehmen (drei Commits, §2.2), Tests auf `[GameDataFact]`, Marker; Befunde als wenige Sammelbefunde (ein Befund darf
   mehrere Adressen im Titel tragen, wie beim Spielstand): Rechteck/Pixel, Blits, Masken, Text, Seiten, Atlas-Umwandlung.
2. Fehlende Treiberteile: Linie (2 Vektoren), Objekt zeichnen/zurückschreiben (2 Vektoren) mit `06E9`/`071C`.
3. Nicht in ②: `0A96`, `0A69`, `0B67`, `21AF`, Paletten/Blenden, Farbregister.

Hinweis für die Abdeckung: Die Treiberroutinen liegen hinter der Vektortabelle und sind keine Far-Einsprünge; die Spalte
„Far-Einsprünge“ (25 für `23CD`) taugt dort nicht als Nenner. EGA-, Tandy-Teil und `E-*.PP` sollten ausdrücklich als „nicht
zu portieren“ geführt werden (entschieden, §3.3).

**③ `phasen`** – die Analyse des Altbestands ist richtig und bytegleich reproduzierbar; zu übernehmen ist nur das Werkzeug.

1. `tools/callgraph.py` und das Experiment nach `experiments/phasen/`; Ausgabe bleibt lokal (F1).
2. Befund zur Wochenschleife `1777:01F8` in `docs/routines/` (Folge aus §1.4 mit den beiden Abbruchwegen: Scancode 0Eh
   nach der Bilanz, Rückgabe aus `20B3:0005`) und zu `main`.
3. Statt eines Ports: Test, der die Markenfolge im RefEmu gegen den Befund prüft (braucht ① Teil 2 → ③ nach ①).
4. Die Korrekturen B1–B7 an `STATUS.md`/`DATENLAYOUT.md` kann die Leitstelle mit ③ oder im Abschluss-PR nachziehen.

### 3.2 Runden R3 ff.

Leitlinien aus der Karte:

- **Grafikschichten vor allen Bildschirmen.** `0A96` hat 585 Aufrufstellen aus 21 Segmenten; ohne Befehlsliste, Seiten,
  Objekte und Blenden lässt sich kein Bild pixelgenau vergleichen. → R3.
- **Spielfeldschicht aus dem Anbau herauslösen.** `0609` wird von zwölf Segmenten benutzt (Raster, Cursor, Feldart setzen,
  Kopfzeile). Diese Teile zuerst, die Anbauphase selbst danach.
- **Regeln ohne Bildschirm zuerst.** Preise `1777:02DC`, Vermögen/Rang/Verlauf `0825:0813/06CB/07C4`, Obergrenzen
  `0609:1178`, Wetter `21FD:021F`, Produktion `22A3:0003`, Schwund `22A3:026B`, Edelsteinkurs `08CF:11D7`, Warenpreise
  `0B9F:06A0/078A`, neue Partie `0E31:0003`, Endbedingung in `20B3:0005`: rund ein Dutzend Routinen, jede einzeln mit
  gesetztem Zustand gegen den RefEmu prüfbar. Sie geben der Phasenmaschine früh echten Inhalt und der Mehrrundenprobe ihre
  Vergleichswerte.
- **Computerspieler mit ihrer Phase, als eigene Spur.** Reine Logik, deshalb gut differenzierbar: `135D` zum Anbau, `141D`
  zum Handel, `11A3` zur Phasenmaschine bzw. Produktion (liest die Felder beider Planungsdurchgänge), `12C8`/`150F`/`16D6`
  zum Roboterkampf.
- **Zufall je Bild als Kern-Ereignis** (B8) gehört in den Entwurf der Phasenmaschine, bevor die erste Szene entsteht.
- **Roboterkampf zuletzt und geteilt.** Gut ein Fünftel des Codes, in den Stufen 0–2 aus. Eine Runde reicht dafür nicht.

Vorschlag (Reihenfolge der Themen wie im Fahrplan, Zuschnitt angepasst):

| Runde | Spuren | Segmente |
|---|---|---|
| R3 | Phasenmaschine mit Takt und Zufallsereignissen · Wochenregeln ohne Bildschirm · Grafikschichten · App-Host mit Eingabe · Ton-Analyse (`FXDRIVER.EXE`, `218B`, `21A7`) | `1777`, Teile von `0825`/`0609`/`22A3`/`08CF`/`0B9F`; `0A96`, `0A69`, `0B67`, `21AF`, `21FD`-Paletten; `0E9E`/`27C6` als Vorlage |
| R4 | Menüs · neue Partie · Start/Code-Abfrage · Spielfeldschicht | `192D`, `0E31`, `0DFD`, `21FD:0687`, gemeinsame Teile von `0609` |
| R5 | Anbau · Anbau-KI und Wochenplanung · Wochenbilanz (wird schon in Woche 0 gezeigt) | `0609`, `135D`, `11A3`, `0825` |
| R6 | Produktion/Ernte/Wetter · Katastrophen · Wochenereignisse | `22A3`, `111D`, `17CB` |
| R7 | Stationen/Rakete/Piraten | `1B03` |
| R8 | Handel · Handels-KI · Zusatzphase | `0B9F`, `0C29`, `141D`, `1DEF` |
| R9 | Fat Mike und Wettbewerb · Einkauf | `1F63`, `08CF` |
| R10 | Ende/Abend · Ton · Gesamtpartie ohne Roboter (Stufen 0–2) | `20B3`, `218B`, `21A7` |
| R11–R12 | Roboterkauf mit KI · Aufstellung mit KI · Kampffeld · Kampf mit KI | `1C68`, `16D6`, `0535`, `12C8`, `1E7E`, `0EDE`, `150F` |
| R13 | Gesamtpartie v1.0 | – |

Gegenüber dem Fahrplan: Rakete rückt vor den Handel (Reihenfolge der Woche, §1.4), Bilanz nach vorn, Roboterkampf in zwei
Runden ans Ende, eine spielbare Partie ohne Roboter als Zwischenziel.

### 3.3 Offene Punkte

- **Entschieden (Nutzer 11.11.2026):** Es wird nur VGA portiert – EGA, Tandy, `E-*.PP` und PC-Lautsprecher `2850` entfallen.
  Zwischenziel ist die „Gesamtpartie ohne Roboterkampf“ vor dem Roboterkampf (R10 in §3.2). Der Probencode liegt im Repo
  (Anhang A). Eintrag in Plan und `PORTIERUNGSPLAN.md`: Leitstelle.
- Regel der Zusatzphase `1DEF`, Bedeutung von `DS:48A2`/`DS:2F26`, Kennzahl +9 der Ressourcentabelle, Tonroutinen
  `0000:06FE…0750`, Schalter `DS:0912`: nicht geklärt.
- Nicht untersucht: `FXDRIVER.EXE`, Amiga-Fassung, Videobilder; die Rollen in §1.2 sind bis auf die belegten Stellen nicht
  Routine für Routine gelesen.

## Anhang A: RefEmu-Proben

Alle Proben sind xUnit-Tests gegen `OriginalProgram`: `experiments/bestand/BestandProben.cs`, gestartet mit
`bash experiments/bestand/proben.sh <Ausgabeordner> [Sekunden für P1]` (Nutzer 11.11.2026: Probencode ins Repo). Das Skript
arbeitet in Wegwerf-Worktrees und wendet dort die Commits der Altbranches an: P1, P4, P5 brauchen `Start`, `Press` und
`AddCodeHook` aus `dev/ap1-refemu`, P2 und P3 liefen auch mit `main` allein. Ein zweiter vollständiger Lauf des Skripts
(P1 mit 230 s) ergab dieselben Werte. Optionen: Vorgaben von `MachineOptions`, soweit nicht genannt.

| ID | Aufbau | Eingangszustand | Ergebnis |
|---|---|---|---|
| P1 | `Start`, 17,5 s laufen, Eingabetaste (Scancode 1Ch) nach 0,5 s, dann 900 s; Haken auf `1777:02DC`, `21FD:0977` (Argument vom Stapel), `0E31:0003`, Phaseneinstiege; je Treffer Zeit, `DS:4B9A`, `DS:10D2` | Vorgabe: vier Computerspieler | Wochenbeginn bei 18,12 / 220,85 / 381,65 / 577,39 / 742,47 s; in jeder Woche Marken 0–6 in der Folge von §1.4; kein Abbruch; zweiter Lauf (300 s) mit denselben Zeiten und Zufallszuständen. Mit residentem `FXDRIVER.EXE`: Woche 1 ab 214,05 s |
| P2 | `LoadOnly`, `Call(1777:02DC)`; zweimal mit Startzeit 01.06.1991 12:00:00 und 03.02.1993 04:05:06 | Bargeld der Spieler 500, 1200, 80000, 37; `DS:4B9A` = 7; `DS:2F05` = 48 | beide Male 0 Protokolleinträge, `DS:10D2` unverändert 1, ganzes Datensegment bytegleich; Öl 20, Raketen 292, Lotto 63, Land 550 |
| P3 | `LoadOnly`, `Call(0609:1178)`; danach `Call(22A3:026B)` | je Spieler *k* Felder in den Arten 0–2 (*k* = 0, 1, 2, 5, 9), Art 3 = 7; Vorräte Spieler 0: 70, 999, 5, 13 | `DS:2F26` = 3, 9, 15, 30, 30; `DS:48A2` = 6, 12, 18, 30, 30 (Art 3 zählt nicht); Vorräte danach 56, 799, 4, 10 |
| P4 | wie P1, 215 s; Haken auf `21FD:01BA`, Rücksprungadresse und Argument vom Stapel, gezählt je Abschnitt zwischen den Marken | wie P1 | Woche 0: u. a. `1F63:113E` 125 ×, `1B03:1178` 507 ×, `1B03:0A62` 90 ×, `1DEF:0173`/`01AF` 108 ×, im Menü `192D:0234` 21 × und `192D:14B4` 80 × |
| P5 | `Start`, 17,5 s, Eingabetaste nach 0,5 / 0,5 / 1,5 / 3,0 s; `DS:10D2` beim Eintritt in `192D:0007` und `0E31:0003` | – | Menü jeweils `2689D01E`; neue Partie `F9D5A4AC`, `F9D5A4AC`, `211A7193`, `17ED9B74` |
| P6 | `dev/ap2-vga` (`0e95549`, `dab5889`, `5a9f178`) per Cherry-Pick auf `main`, Verknüpfung `original` → Originaldaten, `dotnet test --filter FullyQualifiedName~Vga` | – | 20 bestanden, 0 fehlgeschlagen |

Statische Zählungen (§1.1–1.3, §1.5) liefert `python3 -I experiments/bestand/statik.py gamedata/dos/T.EXE <disasm> <aus>`; die
Segmentliste dieses Dokuments wurde maschinell gegen `docs/analyse/segments.md` verglichen (45 Segmente, 891 Far-Einsprünge,
Größen gleich).
