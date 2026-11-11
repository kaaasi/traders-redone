# Portierungsstatus

Stand: 11.11.2026 · Runde 2 (Altbestand) abgeschlossen ([archiv/runde-2.md](docs/archiv/runde-2.md)); nächste: Runde 3

**Legende:** ✅ portiert und gegen Original geprüft · 🟢 portiert, Tests grün, Originalvergleich
(RefEmu) noch offen · 🟡 in Arbeit · 🔍 analysiert, noch nicht portiert · ⬜ offen

## 1. Meilensteine

| # | Meilenstein | Status | Bemerkung |
|---|---|---|---|
| M0 | Fundament (Repo, CI, Originale, Plan) | ✅ | Hash-Prüfung der Originale in CI |
| M1 | Ressourcen | 🟢 | PP20, VGA-Atlas (Kopf korrigiert), Palette, Texte, Schriften, Masken, Musik-Kopf, Asset-Viewer |
| M2 | Analyse-Infrastruktur / RefEmu | 🟢 | 286-Emulator (SingleStepTests grün), DOS/BIOS/VGA, Bildabzug; Partie mit FXDRIVER über mehrere Wochen, Halt an Phasenmarken (`RunToWeekStart`/`RunToMarker`, R2 Spur 1); Bestandsaufnahme aller 45 Segmente (R2 Spur 0) |
| M3 | Kern-Fundament | 🟡 | RNG ✅, 16-Bit-Arithmetik, Spielzustand/DS-Layout + Spielstand fertig; Wochenschleife `1777:01F8` belegt (R2 Spur 3), VGA-Treiber `23CD` portiert (R2 Spur 2); Phasenmaschine offen (R3) |
| M4 | Menü & Setup | ⬜ | |
| M5 | Anbau | ⬜ | |
| M6 | Produktion & Wetter | ⬜ | |
| M7 | Fat Mike / Schnecken | ⬜ | |
| M8 | Auktion / Handel | ⬜ | |
| M9 | Rakete & Stationen / Piraten | ⬜ | |
| M10 | Ereignisse & Katastrophen | ⬜ | |
| M11 | Wochenbilanz & Einkauf | ⬜ | |
| M12 | Roboterkampf | ⬜ | |
| M13 | Ende / Rang | ⬜ | |
| M14 | Ton | ⬜ | |
| M15 | Gesamtpartie = v1.0 | ⬜ | |
| E1–E7 | Erweiterungen | ⬜ | nach v1.0, siehe Plan |

## 2. Ressourcen

| Datei | Inhalt | Laden | Format verstanden | Status |
|---|---|---|---|---|
| `V-*.PP` (10) | VGA-Bilder: Palette + Größenkopf + 320×256-Atlas | `VgaImage` | ja (Kopf bei 768, Pixel ab 772 – Befund AP2) | 🟢 |
| `E-*.PP` (10) | EGA-Bilder | entpackt | nein | – entfällt (nur VGA, Nutzer 11.11.2026) |
| `FONTS.PP` | 2 Schriften (16×12, 16×5) | `FontSet` | ja (Breiten aus Code offen) | 🟢 |
| `MASKS.PP` | 1-Bit-Masken 320×200 | `MaskImage` | ja (Zuordnung aus Code offen) | 🟢 |
| `ENGLISCH.TXT` | 303 Spieltexte | `TextTable` | ja | 🟢 |
| `*.CMF` (3) | Musik, eigenes Format „A.H.“ | `MusicModule` (Kopf) | teilweise: Reihenfolge, Patternanzahl | 🟡 Rest in M14 |
| `FXDRIVER.EXE` | Ton-Treiber (Linel), `int 64h` | – | Schnittstelle teilweise | 🔍 |

## 3. Originalmodule (Codesegmente in T.EXE)

Rollen nach der Bestandsaufnahme [docs/analyse/BESTAND.md](docs/analyse/BESTAND.md) §1.2 (Stufe S: Aufrufer, Aufrufziele,
Texte, geschriebene Felder; nicht Routine für Routine gelesen). Einsprünge: [docs/analyse/segments.md](docs/analyse/segments.md) –
die Zahlen „×n“ dort zählen nur Far-Aufrufe, Aufrufe innerhalb eines Segments fehlen (BESTAND B7). Nur VGA wird portiert
(Nutzer 11.11.2026): EGA-, Tandy-Teile, `E-*.PP` und PC-Lautsprecher entfallen.

| Segment | Größe | Rolle | Status |
|---|---:|---|---|
| `0000` | 0x5350 | Turbo-C-Laufzeit (Startcode ruft `1777:000C`, Long-Arithmetik, Dateien, `time()`, Tonroutinen `06FE/0724/0750`) | 🔍 nur benötigte Teile werden nachgebildet |
| `0535` | 0xD40 | Roboterkampf: Aufstellung, Kampfwerte | 🔍 |
| `0609` | 0x21C0 | Anbauphase und gemeinsame Spielfeldschicht (Raster, Cursor, Feldart `10D2`, Kopfzeile); Obergrenzen `1178` | ⬜ |
| `0825` | 0xAA0 | Wochenbilanz (Darstellung), Vermögen/Rang (`0813`, `06CB`, `07C4`) | ⬜ |
| `08CF` | 0x19A0 | Einkauf (Öl, Raketen, Lotto, Edelsteine), Edelsteinkurs `11D7` | ⬜ |
| `0A69` | 0x2D0 | Grafik: Linien-, Rahmen-, Texthilfen über der Befehlsliste | 🔍 |
| `0A96` | 0xD10 | Grafik: Befehlsliste (17 Befehlstypen, Ausführung `06A8`) | 🔍 |
| `0B67` | 0x380 | Grafik: Bildschirmseiten, Seitenwechsel, Warten | ⬜ |
| `0B9F` | 0x8A0 | Handel: Ablauf und Regeln (Angebote, Warenpreise `DS:4898`) | 🔍 |
| `0C29` | 0x14A0 | Handel: Bildschirm und Bedienung, bucht Bargeld/Vorräte | ⬜ |
| `0D73` | 0x8A0 | Systemschicht: Interrupts, Karten-/Tonerkennung, Speicher, Dateien | ⬜ |
| `0DFD` | 0x340 | Programmstart: Schrift, Masken, Bilder, Texte laden; Code-Abfrage | ⬜ |
| `0E31` | 0x6D0 | Neue Partie: Land, Stationen, Startwerte | 🔍 |
| `0E9E` | 0x400 | Eingabe je Spieler nach Steuerungsart | ⬜ |
| `0EDE` | 0x23F0 | Roboterkampf: Ablauf, Bewegung, Treffer, Beute | 🔍 |
| `111D` | 0x860 | Katastrophen und Wochenereignis-Einblendung | ⬜ |
| `11A3` | 0x1250 | Computerspieler: Wochenplanung (zwei Durchgänge) | ⬜ |
| `12C8` | 0x950 | Computerspieler: Roboter-Aufstellung | ⬜ |
| `135D` | 0xC00 | Computerspieler: Anbau | ⬜ |
| `141D` | 0xF20 | Computerspieler: Handel | ⬜ |
| `150F` | 0x1C70 | Computerspieler: Roboterkampf | ⬜ |
| `16D6` | 0xA10 | Computerspieler: Roboterkauf | ⬜ |
| `1777` | 0x540 | Hauptprogramm: `main` `000C`, Wochenschleife `01F8`, Wochenpreise `02DC` (ohne Uhr, BESTAND B1), Initialisierung `04E4` | 🔍 `000C`/`01F8` mit Befund und RefEmu-Test (R2 Spur 3) |
| `17CB` | 0xA10 | Wochenereignisse (Geld, Felder) mit Fenster | ⬜ |
| `186C` | 0xC10 | Ressourcenlader: Bild *i* laden (`0002`), entpacken, Dateien | 🔍 |
| `192D` | 0x1D60 | Menüs (Haupt, Stufe, Einstellungen, Diskette, Steuerung), Texttabelle | 🔍 |
| `1B03` | 0x1650 | Stationswahl, Raketenflug, Piraten, Stationsertrag | ⬜ |
| `1C68` | 0x1870 | Roboterphase: Steuerung und Roboterkauf-Bildschirm | 🔍 |
| `1DEF` | 0x8F0 | Zufällige Zusatzphase nach dem Handel (Regel offen) | ⬜ |
| `1E7E` | 0xE50 | Roboterkampf: Kampffeld erzeugen (Raster `DS:48F4`) | ⬜ |
| `1F63` | 0x1500 | Fat Mike und Wettbewerb nach dem Einkauf | ⬜ |
| `20B3` | 0xD80 | Endprüfung, Abend- und Endbildschirme | ⬜ |
| `218B` | 0x1C0 | Musik über residenten Treiber (`int 64h`) | 🔍 |
| `21A7` | 0x80 | Toneffekt auslösen, Effektquelle wählen | ⬜ |
| `21AF` | 0x4E0 | Grafik: bewegte Objekte (8 Sätze ab `DS:330C`) | ⬜ |
| `21FD` | 0x980 | Hilfen: Zufall ✅, Wetter `021F`, Warten, Paletten/Blenden, Seitenwechsel, Code-Abfrage `0687`, leere Marke `0977` | 🟡 |
| `2295` | 0xE0 | Zahl → Text, Einzelfarbe, Palette an die Karte | 🔍 |
| `22A3` | 0x930 | Produktion (`0003`), Wochenschwund der Vorräte (`026B`), Erntedarstellung (`02D3`) (BESTAND B4) | ⬜ |
| `2336` | 0x970 | Spielstand speichern/laden (`000A`, `0539`) | 🟢 |
| `23CD` | 0x3F90 | Grafiktreiber in Assembler; portiert wird nur der VGA-Teil (Mode X), Objekte `06E9/071C`, Atlas `166A/3079` | ✅ VGA-Teil (29 Adressen mit Differenztest, Randfälle laut Gesamtreview R2 behoben, #59; offen #60) |
| `27C6` | 0x5B0 | Interrupt-Handler (Timer, Tastatur, DOS), Joystick, Farbregister | 🔍 |
| `2821` | 0x2F0 | PP20-Entpacker (`PowerPacker` aus Formatwissen, ohne Differenztest) | 🟢 |
| `2850` | 0x8A0 | PC-Lautsprecher-Effekte | – entfällt (nur VGA/AdLib) |
| `28DA`, `28DB` | 0x10 | Segmentmarken der Laufzeit, kein Code | – |

## 4. Werkzeuge

| Werkzeug | Zweck | Status |
|---|---|---|
| `src/Traders.Viewer` | Asset-Viewer (MonoGame): Atlanten, Masken, Schriften; Maus zeigt Koordinate/Palettenindex; `--screenshot` für CI | 🟢 läuft unter virtuellem Bildschirm (13 Screenshots), Windows-Build als Download |
| `src/Traders.Tools` | Export aller Ressourcen als PNG/BIN | 🟢 |
| `tools/segmap.py` | Segmentkarte + Disassemblierung | 🟢 |
| `src/Traders.RefEmu` | Referenz-Emulator: führt T.EXE aus, `OriginalProgram` für Differenztests | 🟢 |
| `tools/datenlayout/` | DS-Zugriffe, Spielstandfelder aus der Disassemblierung | 🟢 |
| CI (`.github/workflows/build.yml`) | Ubuntu: `tools/pruefen.sh` je PR/Push auf main; Windows und Viewer-Downloads nur per Hand | ✅ |
| `tools/pruefen.sh`, `tools/claude/` | Einheitliche Prüfung; SDK + NuGet-Spiegel für Claude (Release `claude-werkzeuge`) | ✅ |
| `kaaasi/traders-referenz` | Originale, Disassemblierung, RefEmu-Bilder, Videobilder (alle 4 s bzw. 5 s); `tools/claude/fetch-reference.sh` | ✅ |
| `experiments/apN/` | Analysen, lokal ausgeführt | 🟢 |

## 5. Einzelne Routinen

Die Tabellen dieses Abschnitts erzeugt `tools/abdeckung.py --write` aus `T.EXE`, `docs/routines/`, den Markern
`[ORIGINAL] SSSS:OOOO` im Code und den Differenztests (`tests/**/*Differential*.cs`); `tools/pruefen.sh` prüft, dass sie aktuell
sind. Mitgeprüfte Laufzeithilfen (`0000:4F4E`/`5058`/`4FA0` Long-Arithmetik, `0000:3405` `time()`) stehen im Befund von
`21FD:01BA`, der 16-Bit-IDIV-Grenzfall in [Int16Math_IDIV.md](docs/routines/Int16Math_IDIV.md).

<!-- abdeckung:anfang -->
<!-- Erzeugt von tools/abdeckung.py --write; nicht von Hand ändern. -->

### Routinen mit Befund

| Adresse | Befund | C# (`[ORIGINAL]`) | Differenztest | Status |
|---|---|---|---|---|
| `0B67:0002` | [0B67_0002_seiten.md](docs/routines/0B67_0002_seiten.md) | `src/Traders.Vga/Layers/Pages.cs` | `PageLayerTests.cs` | ✅ |
| `0B67:016B` | [0B67_0002_seiten.md](docs/routines/0B67_0002_seiten.md) | `src/Traders.Vga/Layers/Pages.cs` | `PageLayerTests.cs` | ✅ |
| `0B67:0196` | [0B67_0002_seiten.md](docs/routines/0B67_0002_seiten.md) | `src/Traders.Vga/Layers/Pages.cs` | `PageLayerTests.cs` | ✅ |
| `0B67:01B5` | [0B67_0002_seiten.md](docs/routines/0B67_0002_seiten.md) | `src/Traders.Vga/Layers/Pages.cs` | `PageLayerTests.cs` | ✅ |
| `0B67:01D4` | [0B67_0002_seiten.md](docs/routines/0B67_0002_seiten.md) | `src/Traders.Vga/Layers/Pages.cs` | `PageLayerTests.cs` | ✅ |
| `0B67:0206` | [0B67_0002_seiten.md](docs/routines/0B67_0002_seiten.md) | `src/Traders.Vga/Layers/Pages.cs` | `PageLayerTests.cs` | ✅ |
| `0B67:0238` | [0B67_0002_seiten.md](docs/routines/0B67_0002_seiten.md) | `src/Traders.Vga/Layers/Pages.cs` | `PageLayerTests.cs` | ✅ |
| `0B67:028F` | [0B67_0002_seiten.md](docs/routines/0B67_0002_seiten.md) | `src/Traders.Vga/Layers/Pages.cs` | `PageLayerTests.cs` | ✅ |
| `0B67:0299` | [0B67_0002_seiten.md](docs/routines/0B67_0002_seiten.md) | `src/Traders.Vga/Layers/Pages.cs` | `PageLayerTests.cs` | ✅ |
| `0D73:00D3` | [0B67_0002_seiten.md](docs/routines/0B67_0002_seiten.md) | `src/Traders.Vga/Layers/GraphicsLayers.cs` | `PageLayerTests.cs` | ✅ |
| `1777:000C` | [1777_000C_main.md](docs/routines/1777_000C_main.md) | – | – | 🔍 |
| `1777:01F8` | [1777_01F8_wochenschleife.md](docs/routines/1777_01F8_wochenschleife.md) | – | – | 🔍 |
| `21AF:04A2` | [21AF_04A2_zeigerpaare.md](docs/routines/21AF_04A2_zeigerpaare.md) | `src/Traders.Vga/Layers/Palettes.cs` | `PaletteLayerTests.cs` | ✅ |
| `21AF:04CD` | [21AF_04A2_zeigerpaare.md](docs/routines/21AF_04A2_zeigerpaare.md) | `src/Traders.Vga/Layers/Palettes.cs` | `PaletteLayerTests.cs` | ✅ |
| `21FD:0185` | [21FD_026C_paletten.md](docs/routines/21FD_026C_paletten.md) | `src/Traders.Vga/Layers/GraphicsLayers.cs` | `PaletteLayerTests.cs` | ✅ |
| `21FD:01BA` | [21FD_01BA_random.md](docs/routines/21FD_01BA_random.md) | `src/Traders.Core/OriginalRandom.cs` | `RandomDifferentialTests.cs` | ✅ |
| `21FD:026C` | [21FD_026C_paletten.md](docs/routines/21FD_026C_paletten.md) | `src/Traders.Vga/Layers/Palettes.cs` | `PaletteLayerTests.cs` | ✅ |
| `21FD:02C9` | [21FD_026C_paletten.md](docs/routines/21FD_026C_paletten.md) | `src/Traders.Vga/Layers/Palettes.cs` | `PaletteLayerTests.cs` | ✅ |
| `21FD:0329` | [21FD_026C_paletten.md](docs/routines/21FD_026C_paletten.md) | `src/Traders.Vga/Layers/Palettes.cs` | `PaletteLayerTests.cs` | ✅ |
| `21FD:0353` | [21FD_026C_paletten.md](docs/routines/21FD_026C_paletten.md) | `src/Traders.Vga/Layers/Palettes.cs` | `PaletteLayerTests.cs` | ✅ |
| `21FD:037D` | [21FD_026C_paletten.md](docs/routines/21FD_026C_paletten.md) | `src/Traders.Vga/Layers/Palettes.cs` | `PaletteLayerTests.cs` | ✅ |
| `21FD:03B4` | [21FD_026C_paletten.md](docs/routines/21FD_026C_paletten.md) | `src/Traders.Vga/Layers/Palettes.cs` | `PaletteLayerTests.cs` | ✅ |
| `21FD:03EB` | [21FD_026C_paletten.md](docs/routines/21FD_026C_paletten.md) | `src/Traders.Vga/Layers/Palettes.cs` | `PaletteLayerTests.cs` | ✅ |
| `21FD:04DA` | [21FD_026C_paletten.md](docs/routines/21FD_026C_paletten.md) | `src/Traders.Vga/Layers/Palettes.cs` | `PaletteLayerTests.cs` | ✅ |
| `21FD:051F` | [21FD_051F_seitenwechsel.md](docs/routines/21FD_051F_seitenwechsel.md) | `src/Traders.Vga/Layers/Pages.cs` | `PageLayerTests.cs` | ✅ |
| `21FD:0687` | [21FD_0687_codeabfrage.md](docs/routines/21FD_0687_codeabfrage.md) | – | – | 🔍 |
| `2295:006D` | [2295_006D_farbregister.md](docs/routines/2295_006D_farbregister.md) | `src/Traders.Vga/Layers/Palettes.cs` | `PaletteLayerTests.cs` | ✅ |
| `2295:00C5` | [2295_006D_farbregister.md](docs/routines/2295_006D_farbregister.md) | `src/Traders.Vga/Layers/Palettes.cs` | `PaletteLayerTests.cs` | ✅ |
| `2336:000A` | [2336_000A_spielstand.md](docs/routines/2336_000A_spielstand.md) | `src/Traders.Core/State/SaveGame.cs` | `SaveGameDifferentialTests.cs` | ✅ |
| `2336:0539` | [2336_000A_spielstand.md](docs/routines/2336_000A_spielstand.md) | `src/Traders.Core/State/SaveGame.cs` | `SaveGameDifferentialTests.cs` | ✅ |
| `23CD:04F6` | [23CD_04F6_kartenwahl.md](docs/routines/23CD_04F6_kartenwahl.md) | `src/Traders.Vga/ModeXSetup.cs` | `DriverEdgeCaseTests.cs` | ✅ |
| `23CD:05B6` | [23CD_05DD_text.md](docs/routines/23CD_05DD_text.md) | `src/Traders.Vga/ModeXDriver.cs` | `DriverDifferentialTests.cs` | ✅ |
| `23CD:05C3` | [23CD_05DD_text.md](docs/routines/23CD_05DD_text.md) | `src/Traders.Vga/ModeXDriver.cs` | `DriverDifferentialTests.cs` | ✅ |
| `23CD:05D0` | [23CD_311A_seiten.md](docs/routines/23CD_311A_seiten.md) | `src/Traders.Vga/ModeXDriver.cs` | `DriverDifferentialTests.cs` | ✅ |
| `23CD:05DD` | [23CD_05DD_text.md](docs/routines/23CD_05DD_text.md) | `src/Traders.Vga/ModeXDriver.cs` | `DriverDifferentialTests.cs` | ✅ |
| `23CD:06BE` | [23CD_378B_linie.md](docs/routines/23CD_378B_linie.md) | `src/Traders.Vga/ModeXLines.cs` | `DriverLineAndObjectTests.cs` | ✅ |
| `23CD:06E9` | [23CD_06E9_objekte.md](docs/routines/23CD_06E9_objekte.md) | `src/Traders.Vga/ModeXObjects.cs` | `DriverLineAndObjectTests.cs` | ✅ |
| `23CD:071C` | [23CD_06E9_objekte.md](docs/routines/23CD_06E9_objekte.md) | `src/Traders.Vga/ModeXObjects.cs` | `DriverLineAndObjectTests.cs` | ✅ |
| `23CD:3042` | [23CD_04F6_kartenwahl.md](docs/routines/23CD_04F6_kartenwahl.md) | `src/Traders.Vga/ModeXSetup.cs` | `DriverEdgeCaseTests.cs` | ✅ |
| `23CD:3079` | [23CD_3079_atlas.md](docs/routines/23CD_3079_atlas.md) | `src/Traders.Vga/ModeXMemory.cs` | `DriverDifferentialTests.cs` | ✅ |
| `23CD:30D1` | [23CD_311A_seiten.md](docs/routines/23CD_311A_seiten.md) | `src/Traders.Vga/ModeXDriver.cs` | `DriverDifferentialTests.cs` | ✅ |
| `23CD:30E9` | [23CD_311A_seiten.md](docs/routines/23CD_311A_seiten.md) | `src/Traders.Vga/ModeXDriver.cs` | `DriverDifferentialTests.cs`, `DriverEdgeCaseTests.cs` | ✅ |
| `23CD:311A` | [23CD_311A_seiten.md](docs/routines/23CD_311A_seiten.md) | `src/Traders.Vga/ModeXDriver.cs` | `DriverDifferentialTests.cs`, `DriverEdgeCaseTests.cs` | ✅ |
| `23CD:31A6` | [23CD_31A6_rechteck_pixel.md](docs/routines/23CD_31A6_rechteck_pixel.md) | `src/Traders.Vga/ModeXDriver.cs` | `DriverDifferentialTests.cs`, `DriverEdgeCaseTests.cs`, `DriverWrapTests.cs` | ✅ |
| `23CD:31FD` | [23CD_05DD_text.md](docs/routines/23CD_05DD_text.md) | `src/Traders.Vga/ModeXDriver.cs` | `DriverDifferentialTests.cs` | ✅ |
| `23CD:328E` | [23CD_32D8_blits.md](docs/routines/23CD_32D8_blits.md) | `src/Traders.Vga/ModeXDriver.cs` | `DriverDifferentialTests.cs`, `DriverWrapTests.cs` | ✅ |
| `23CD:32D8` | [23CD_32D8_blits.md](docs/routines/23CD_32D8_blits.md) | `src/Traders.Vga/ModeXDriver.cs` | `DriverDifferentialTests.cs`, `DriverEdgeCaseTests.cs` | ✅ |
| `23CD:3386` | [23CD_32D8_blits.md](docs/routines/23CD_32D8_blits.md) | `src/Traders.Vga/ModeXDriver.cs` | `DriverDifferentialTests.cs`, `DriverWrapTests.cs` | ✅ |
| `23CD:3449` | [23CD_32D8_blits.md](docs/routines/23CD_32D8_blits.md) | `src/Traders.Vga/ModeXDriver.cs` | `DriverDifferentialTests.cs`, `DriverEdgeCaseTests.cs` | ✅ |
| `23CD:351E` | [23CD_32D8_blits.md](docs/routines/23CD_32D8_blits.md) | `src/Traders.Vga/ModeXDriver.cs` | `DriverDifferentialTests.cs`, `DriverEdgeCaseTests.cs`, `DriverWrapTests.cs` | ✅ |
| `23CD:35DF` | [23CD_35DF_masken.md](docs/routines/23CD_35DF_masken.md) | `src/Traders.Vga/ModeXDriver.cs` | `DriverDifferentialTests.cs`, `DriverEdgeCaseTests.cs`, `DriverWrapTests.cs` | ✅ |
| `23CD:373B` | [23CD_31A6_rechteck_pixel.md](docs/routines/23CD_31A6_rechteck_pixel.md) | `src/Traders.Vga/ModeXDriver.cs` | `DriverDifferentialTests.cs` | ✅ |
| `23CD:3764` | [23CD_31A6_rechteck_pixel.md](docs/routines/23CD_31A6_rechteck_pixel.md) | `src/Traders.Vga/ModeXDriver.cs` | `DriverDifferentialTests.cs` | ✅ |
| `23CD:378B` | [23CD_378B_linie.md](docs/routines/23CD_378B_linie.md) | `src/Traders.Vga/ModeXLines.cs` | `DriverEdgeCaseTests.cs`, `DriverLineAndObjectTests.cs` | ✅ |
| `23CD:38F2` | [23CD_378B_linie.md](docs/routines/23CD_378B_linie.md) | `src/Traders.Vga/ModeXLines.cs` | `DriverEdgeCaseTests.cs`, `DriverLineAndObjectTests.cs` | ✅ |
| `23CD:3A9A` | [23CD_32D8_blits.md](docs/routines/23CD_32D8_blits.md) | `src/Traders.Vga/ModeXDriver.cs` | `DriverDifferentialTests.cs` | ✅ |
| `23CD:3AAB` | [23CD_06E9_objekte.md](docs/routines/23CD_06E9_objekte.md) | `src/Traders.Vga/ModeXObjects.cs` | `DriverLineAndObjectTests.cs` | ✅ |
| `23CD:3C42` | [23CD_06E9_objekte.md](docs/routines/23CD_06E9_objekte.md) | `src/Traders.Vga/ModeXObjects.cs` | `DriverLineAndObjectTests.cs` | ✅ |
| `23CD:3CE8` | [23CD_35DF_masken.md](docs/routines/23CD_35DF_masken.md) | `src/Traders.Vga/ModeXDriver.cs` | `DriverDifferentialTests.cs`, `DriverWrapTests.cs` | ✅ |

Status: ✅ portiert mit Differenztest (`[DIFFERENTIAL]`) · 🟢 portiert, Differenztest fehlt · 🔍 nur Befund.

### Abdeckung je Segment

| Segment | Größe | Far-Einsprünge | mit Befund | portiert | mit Differenztest |
|---|---:|---:|---:|---:|---:|
| `0000` | 0x5350 | 76 | 0 | 0 | 0 |
| `0535` | 0xD40 | 24 | 0 | 0 | 0 |
| `0609` | 0x21C0 | 56 | 0 | 0 | 0 |
| `0825` | 0xAA0 | 16 | 0 | 0 | 0 |
| `08CF` | 0x19A0 | 42 | 0 | 0 | 0 |
| `0A69` | 0x2D0 | 9 | 0 | 0 | 0 |
| `0A96` | 0xD10 | 24 | 0 | 0 | 0 |
| `0B67` | 0x380 | 8 | 9 | 9 | 9 |
| `0B9F` | 0x8A0 | 16 | 0 | 0 | 0 |
| `0C29` | 0x14A0 | 29 | 0 | 0 | 0 |
| `0D73` | 0x8A0 | 15 | 1 | 1 | 1 |
| `0DFD` | 0x340 | 5 | 0 | 0 | 0 |
| `0E31` | 0x6D0 | 7 | 0 | 0 | 0 |
| `0E9E` | 0x400 | 5 | 0 | 0 | 0 |
| `0EDE` | 0x23F0 | 56 | 0 | 0 | 0 |
| `111D` | 0x860 | 7 | 0 | 0 | 0 |
| `11A3` | 0x1250 | 15 | 0 | 0 | 0 |
| `12C8` | 0x950 | 12 | 0 | 0 | 0 |
| `135D` | 0xC00 | 14 | 0 | 0 | 0 |
| `141D` | 0xF20 | 17 | 0 | 0 | 0 |
| `150F` | 0x1C70 | 41 | 0 | 0 | 0 |
| `16D6` | 0xA10 | 12 | 0 | 0 | 0 |
| `1777` | 0x540 | 4 | 2 | 0 | 0 |
| `17CB` | 0xA10 | 9 | 0 | 0 | 0 |
| `186C` | 0xC10 | 19 | 0 | 0 | 0 |
| `192D` | 0x1D60 | 74 | 0 | 0 | 0 |
| `1B03` | 0x1650 | 44 | 0 | 0 | 0 |
| `1C68` | 0x1870 | 40 | 0 | 0 | 0 |
| `1DEF` | 0x8F0 | 14 | 0 | 0 | 0 |
| `1E7E` | 0xE50 | 19 | 0 | 0 | 0 |
| `1F63` | 0x1500 | 27 | 0 | 0 | 0 |
| `20B3` | 0xD80 | 27 | 0 | 0 | 0 |
| `218B` | 0x1C0 | 5 | 0 | 0 | 0 |
| `21A7` | 0x80 | 3 | 0 | 0 | 0 |
| `21AF` | 0x4E0 | 7 | 2 | 2 | 2 |
| `21FD` | 0x980 | 28 | 12 | 11 | 11 |
| `2295` | 0xE0 | 4 | 2 | 2 | 2 |
| `22A3` | 0x930 | 15 | 0 | 0 | 0 |
| `2336` | 0x970 | 4 | 2 | 2 | 2 |
| `23CD` | 0x3F90 | 25 | 29 | 29 | 29 |
| `27C6` | 0x5B0 | 15 | 0 | 0 | 0 |
| `2821` | 0x2F0 | 1 | 0 | 0 | 0 |
| `2850` | 0x8A0 | 1 | 0 | 0 | 0 |
| `28DA` | 0x10 | 0 | 0 | 0 | 0 |
| `28DB` | 0x10 | 0 | 0 | 0 | 0 |
| **Summe** | **0x28DC0** | **891** | **59** | **56** | **56** |

<!-- abdeckung:ende -->

## 6. Runden und Spuren

Vergabe und Stand: [docs/arbeitspakete.md](docs/arbeitspakete.md) (ab 10.11.2026 Runden nach dem Ablauf des Eishockey-Managers).
Nicht integriert und für Runde 2 vorgesehen: `dev/ap1-refemu`, `dev/ap2-vga`, `dev/ap5-phasen`. AP4 Ton offen. Übernahme nach
`main` nur durch die Leitstelle.

## 7. Bekannte offene Punkte

- Differenztests gegen das ausgeführte Original: `random()` sowie Speichern/Laden (AP3 Stand 2); weitere Routinen folgen mit den Paketen.
- Die Videos zeigen eine andere Fassung (© 1991) als unsere T.EXE (© 1992). Bei Abweichungen gilt der RefEmu.
- IQ ist im Original ein globaler Wert (`DS:2E8E`); IQ je Gegner (E2) ist eine echte Erweiterung.
- Kampf und Ton waren schon im Vorversuch unverstanden.
