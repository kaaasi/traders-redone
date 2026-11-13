# Portierungsstatus

Stand: 13.11.2026 · Runde 4 (Programmablauf, Menüs, neue Partie, Spielfeldschicht) abgeschlossen ([archiv/runde-4.md](docs/archiv/runde-4.md)); nächste: Runde 5

**Legende:** ✅ portiert und gegen Original geprüft · 🟢 portiert, Tests grün, Originalvergleich
(RefEmu) noch offen · 🟡 in Arbeit · 🔍 analysiert, noch nicht portiert · ⬜ offen

## 1. Meilensteine

| # | Meilenstein | Status | Bemerkung |
|---|---|---|---|
| M0 | Fundament (Repo, CI, Originale, Plan) | ✅ | Hash-Prüfung der Originale in CI |
| M1 | Ressourcen | 🟢 | PP20, VGA-Atlas (Kopf korrigiert), Palette, Texte, Schriften, Masken, Musik-Kopf, Asset-Viewer |
| M2 | Analyse-Infrastruktur / RefEmu | 🟢 | 286-Emulator (SingleStepTests grün), DOS/BIOS/VGA, Bildabzug; Partie mit FXDRIVER über mehrere Wochen, Halt an Phasenmarken (`RunToWeekStart`/`RunToMarker`, R2 Spur 1); Bestandsaufnahme aller 45 Segmente (R2 Spur 0); Maschinenabzug zum Wiederaufsetzen an Marken (R3 Spur 0) |
| M3 | Kern-Fundament | ✅ | RNG ✅, 16-Bit-Arithmetik, Spielzustand/DS-Layout + Spielstand; VGA-Treiber `23CD` (R2) und Grafikschichten `0A96`/`0A69`/`0B67`/`21AF`/Paletten (R3); Phasenmaschine mit Takt und Zufall je Bild, Wochenregeln ohne Bildschirm mit Differenztests und Mehrrundenprobe über vier Wochen (R3); Host `Traders.App` ohne Szenen (R3). Gesamtreview R3 „Ja“ |
| M4 | Menü & Setup | ✅ | Programmablauf `1777:000C` mit Start `0DFD`, Lader `186C`, Code-Abfrage `21FD:0687` (ohne Anzeige, F1 = B); Menüs `192D` im Kern, erste Szene pixel- und zufallsgenau je Bild gegen den RefEmu; neue Partie `0E31`; Host zeigt Menüszene und holt Bilder bildweise nach (#111); Mehrrundenprobe mit Mensch über vier Wochen (R4). Gesamtreview R4 nach Fix R4G-1 erledigt |
| M5 | Anbau | 🟡 | Vorarbeit R4: Befehlsquelle `0E9E` (sechs Steuerungsarten), Feldart `0609:10D2`, Cursor `0609:047B`, Spielfeld-Szene `1256/1879/1ED9` |
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
| `0609` | 0x21C0 | Anbauphase und gemeinsame Spielfeldschicht (Raster, Cursor, Feldart `10D2`, Kopfzeile); Obergrenzen `1178` | 🟡 `1178` (R3); Feldart `10D2`, Cursor `047B`, Spielfeld-Szene `1256/1879/1ED9/1F04/1854/1A61` (R4 Spur 3); offen `0C73`, `03E0`, Phase `0004` |
| `0825` | 0xAA0 | Wochenbilanz (Darstellung), Vermögen/Rang (`0813`, `06CB`, `07C4`) | 🟡 Vermögen/Rang/Verlauf `0813`/`06CB`/`07C4` portiert (R3 Spur 1) |
| `08CF` | 0x19A0 | Einkauf (Öl, Raketen, Lotto, Edelsteine), Edelsteinkurs `11D7` | 🟡 Edelsteinkurs `11D7` portiert (R3 Spur 1) |
| `0A69` | 0x2D0 | Grafik: Linien-, Rahmen-, Texthilfen über der Befehlsliste | 🟢 Hilfen portiert (R3 Spur 2) |
| `0A96` | 0xD10 | Grafik: Befehlsliste (17 Befehlstypen, Ausführung `06A8`) | 🟢 Befehlsliste, alle 17 Typen (R3 Spur 2) |
| `0B67` | 0x380 | Grafik: Bildschirmseiten, Seitenwechsel, Warten | 🟢 Seiten und Warten (R3 Spur 2) |
| `0B9F` | 0x8A0 | Handel: Ablauf und Regeln (Angebote, Warenpreise `DS:4898`) | 🟡 Warenpreise `06A0`/`078A` portiert (R3 Spur 1) |
| `0C29` | 0x14A0 | Handel: Bildschirm und Bedienung, bucht Bargeld/Vorräte | ⬜ |
| `0D73` | 0x8A0 | Systemschicht: Interrupts, Karten-/Tonerkennung, Speicher, Dateien | ⬜ |
| `0DFD` | 0x340 | Programmstart: Schrift, Masken, Bilder, Texte laden; Code-Abfrage | 🟡 `000D`/`0239` mit Differenztest (R4 Spur 0); offen `0074`/`0132`/`0158` (Anzeige) |
| `0E31` | 0x6D0 | Neue Partie: Land, Stationen, Startwerte | ✅ alle sieben Routinen mit Differenztest (R4 Spur 2) |
| `0E9E` | 0x400 | Eingabe je Spieler nach Steuerungsart | ✅ Befehlsquelle im Kern, alle sechs Steuerungsarten (R4 Spur 3) |
| `0EDE` | 0x23F0 | Roboterkampf: Ablauf, Bewegung, Treffer, Beute | 🔍 |
| `111D` | 0x860 | Katastrophen und Wochenereignis-Einblendung | ⬜ |
| `11A3` | 0x1250 | Computerspieler: Wochenplanung (zwei Durchgänge) | ⬜ |
| `12C8` | 0x950 | Computerspieler: Roboter-Aufstellung | ⬜ |
| `135D` | 0xC00 | Computerspieler: Anbau | ⬜ |
| `141D` | 0xF20 | Computerspieler: Handel | ⬜ |
| `150F` | 0x1C70 | Computerspieler: Roboterkampf | ⬜ |
| `16D6` | 0xA10 | Computerspieler: Roboterkauf | ⬜ |
| `1777` | 0x540 | Hauptprogramm: `main` `000C`, Wochenschleife `01F8`, Wochenpreise `02DC` (ohne Uhr, BESTAND B1), Initialisierung `04E4` | 🟡 `000C` als Programmphasen mit Gleichschritt, `04E4` (R4 Spur 0); `01F8` als Phasenmaschine mit Mehrrundenprobe, `02DC` portiert (R3) |
| `17CB` | 0xA10 | Wochenereignisse (Geld, Felder) mit Fenster | ⬜ |
| `186C` | 0xC10 | Ressourcenlader: Bild *i* laden (`0002`), entpacken, Dateien | 🟡 Bildtabelle `0002`/`0997` im Kern (R4 Spur 0) |
| `192D` | 0x1D60 | Menüs (Haupt, Stufe, Einstellungen, Diskette, Steuerung), Texttabelle | ✅ Menülogik im Kern, Menüszene in `Traders.Presentation` (R4 Spur 1) |
| `1B03` | 0x1650 | Stationswahl, Raketenflug, Piraten, Stationsertrag | ⬜ |
| `1C68` | 0x1870 | Roboterphase: Steuerung und Roboterkauf-Bildschirm | 🔍 |
| `1DEF` | 0x8F0 | Zufällige Zusatzphase nach dem Handel (Regel offen) | ⬜ |
| `1E7E` | 0xE50 | Roboterkampf: Kampffeld erzeugen (Raster `DS:48F4`) | ⬜ |
| `1F63` | 0x1500 | Fat Mike und Wettbewerb nach dem Einkauf | ⬜ |
| `20B3` | 0xD80 | Endprüfung, Abend- und Endbildschirme | ⬜ |
| `218B` | 0x1C0 | Musik über residenten Treiber (`int 64h`) | 🔍 Analyse `docs/analyse/TON.md` (R3 Spur 3) |
| `21A7` | 0x80 | Toneffekt auslösen, Effektquelle wählen | 🔍 Analyse `docs/analyse/TON.md` (R3 Spur 3) |
| `21AF` | 0x4E0 | Grafik: bewegte Objekte (8 Sätze ab `DS:330C`) | 🟢 bewegte Objekte (R3 Spur 2) |
| `21FD` | 0x980 | Hilfen: Zufall ✅, Wetter `021F`, Warten, Paletten/Blenden, Seitenwechsel, Code-Abfrage `0687`, leere Marke `0977` | 🟡 Zufall ✅, Wetter `021F`, Paletten/Blenden `026C…04DA`, Seitenwechsel `051F` (R3); Code-Abfrage `0687` (R4 Spur 0) |
| `2295` | 0xE0 | Zahl → Text, Einzelfarbe, Palette an die Karte | 🟢 portiert (R3 Spur 2) |
| `22A3` | 0x930 | Produktion (`0003`), Wochenschwund der Vorräte (`026B`), Erntedarstellung (`02D3`) (BESTAND B4) | 🟡 Produktion `0003`, Schwund `026B` portiert (R3 Spur 1); Ernte offen |
| `2336` | 0x970 | Spielstand speichern/laden (`000A`, `0539`) | 🟢 |
| `23CD` | 0x3F90 | Grafiktreiber in Assembler; portiert wird nur der VGA-Teil (Mode X), Objekte `06E9/071C`, Atlas `166A/3079` | ✅ VGA-Teil (29 Adressen mit Differenztest, Randfälle laut Gesamtreview R2 behoben, #59; #60 statisch nicht erreichbar, GRAFIK.md §8) |
| `27C6` | 0x5B0 | Interrupt-Handler (Timer, Tastatur, DOS), Joystick, Farbregister | 🟡 Eingabe/Zeitgeber mit Differenztests, Host-Ersatz in `Traders.App` (R3 Spur 3) |
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
| `0000:5101` | [21AF_000D_objekte.md](docs/routines/21AF_000D_objekte.md) | `src/Traders.Vga/Layers/Objects.cs` | `ObjectLayerTests.cs` | ✅ |
| `0609:047B` | [0609_047B_cursor.md](docs/routines/0609_047B_cursor.md) | `src/Traders.Core/Field/FieldCursor.cs` | `FieldCursorDifferentialTests.cs` | ✅ |
| `0609:0B8E` | [0609_047B_cursor.md](docs/routines/0609_047B_cursor.md) | `src/Traders.Core/Field/FieldCursor.cs` | `FieldCursorDifferentialTests.cs` | ✅ |
| `0609:0BEA` | [0609_047B_cursor.md](docs/routines/0609_047B_cursor.md) | `src/Traders.Core/Field/FieldCursor.cs` | `FieldCursorDifferentialTests.cs` | ✅ |
| `0609:0EA1` | [0609_047B_cursor.md](docs/routines/0609_047B_cursor.md) | `src/Traders.Core/Field/FieldCursor.cs` | `FieldCursorDifferentialTests.cs` | ✅ |
| `0609:0EDD` | [0609_047B_cursor.md](docs/routines/0609_047B_cursor.md) | `src/Traders.Core/Field/FieldCursor.cs` | `FieldCursorDifferentialTests.cs` | ✅ |
| `0609:0F0D` | [0609_047B_cursor.md](docs/routines/0609_047B_cursor.md) | `src/Traders.Core/Field/FieldCursor.cs` | `FieldCursorDifferentialTests.cs` | ✅ |
| `0609:0F70` | [0609_047B_cursor.md](docs/routines/0609_047B_cursor.md) | `src/Traders.Core/Field/FieldCursor.cs` | `FieldCursorDifferentialTests.cs` | ✅ |
| `0609:1072` | [0609_10D2_feldart.md](docs/routines/0609_10D2_feldart.md) | `src/Traders.Core/Field/FieldGrid.cs` | `FieldGridDifferentialTests.cs` | ✅ |
| `0609:10A7` | [0609_10D2_feldart.md](docs/routines/0609_10D2_feldart.md) | `src/Traders.Core/Field/FieldGrid.cs` | `FieldGridDifferentialTests.cs` | ✅ |
| `0609:10D2` | [0609_10D2_feldart.md](docs/routines/0609_10D2_feldart.md) | `src/Traders.Core/Field/FieldGrid.cs` | `FieldGridDifferentialTests.cs` | ✅ |
| `0609:1178` | [0609_1178_obergrenzen.md](docs/routines/0609_1178_obergrenzen.md) | `src/Traders.Core/Rules/ProductionLimits.cs` | `WeeklyRulesDifferentialTests.cs` | ✅ |
| `0609:1256` | [0609_1256_rahmen.md](docs/routines/0609_1256_rahmen.md) | `src/Traders.Presentation/Field/FieldScene.cs` | `FieldSceneDifferentialTests.cs` | ✅ |
| `0609:1854` | [0609_1256_rahmen.md](docs/routines/0609_1256_rahmen.md) | `src/Traders.Presentation/Field/FieldScene.cs` | `FieldSceneDifferentialTests.cs` | ✅ |
| `0609:1879` | [0609_1256_rahmen.md](docs/routines/0609_1256_rahmen.md) | `src/Traders.Presentation/Field/FieldScene.cs` | `FieldSceneDifferentialTests.cs` | ✅ |
| `0609:1A61` | [0609_1256_rahmen.md](docs/routines/0609_1256_rahmen.md) | – | `FieldSceneDifferentialTests.cs` | 🔍 |
| `0609:1B72` | [0609_047B_cursor.md](docs/routines/0609_047B_cursor.md) | `src/Traders.Core/Field/FieldCursor.cs` | `FieldCursorDifferentialTests.cs` | ✅ |
| `0609:1ED9` | [0609_1256_rahmen.md](docs/routines/0609_1256_rahmen.md) | `src/Traders.Presentation/Field/FieldScene.cs` | `FieldSceneDifferentialTests.cs` | ✅ |
| `0609:1F04` | [0609_1256_rahmen.md](docs/routines/0609_1256_rahmen.md) | `src/Traders.Presentation/Field/FieldScene.cs` | `FieldSceneDifferentialTests.cs` | ✅ |
| `0609:215E` | [0609_047B_cursor.md](docs/routines/0609_047B_cursor.md) | `src/Traders.Core/Field/FieldCursor.cs` | `FieldCursorDifferentialTests.cs` | ✅ |
| `0609:21BA` | [0609_047B_cursor.md](docs/routines/0609_047B_cursor.md) | `src/Traders.Core/Field/FieldCursor.cs` | `FieldCursorDifferentialTests.cs` | ✅ |
| `0825:06CB` | [0825_0813_vermoegen.md](docs/routines/0825_0813_vermoegen.md) | `src/Traders.Core/Rules/Wealth.cs` | `WealthProductionDifferentialTests.cs` | ✅ |
| `0825:07C4` | [0825_0813_vermoegen.md](docs/routines/0825_0813_vermoegen.md) | `src/Traders.Core/Rules/Wealth.cs` | `WealthProductionDifferentialTests.cs` | ✅ |
| `0825:0813` | [0825_0813_vermoegen.md](docs/routines/0825_0813_vermoegen.md) | `src/Traders.Core/Rules/Wealth.cs` | `WealthProductionDifferentialTests.cs` | ✅ |
| `0825:086F` | [0825_0813_vermoegen.md](docs/routines/0825_0813_vermoegen.md) | `src/Traders.Core/Rules/Wealth.cs` | `WealthProductionDifferentialTests.cs` | ✅ |
| `0825:088D` | [0825_0813_vermoegen.md](docs/routines/0825_0813_vermoegen.md) | `src/Traders.Core/Rules/Wealth.cs` | `WealthProductionDifferentialTests.cs` | ✅ |
| `0825:08EB` | [0825_0813_vermoegen.md](docs/routines/0825_0813_vermoegen.md) | `src/Traders.Core/Rules/Wealth.cs` | `WealthProductionDifferentialTests.cs` | ✅ |
| `0825:096D` | [0825_0813_vermoegen.md](docs/routines/0825_0813_vermoegen.md) | `src/Traders.Core/Rules/Wealth.cs` | `WealthProductionDifferentialTests.cs` | ✅ |
| `08CF:11D7` | [08CF_11D7_edelsteinkurs.md](docs/routines/08CF_11D7_edelsteinkurs.md) | `src/Traders.Core/Rules/GemPrice.cs` | `RandomRulesDifferentialTests.cs` | ✅ |
| `0A69:000B` | [0A69_000B_hilfen.md](docs/routines/0A69_000B_hilfen.md) | `src/Traders.Vga/Layers/DrawHelpers.cs` | `CommandListTests.cs` | ✅ |
| `0A69:002C` | [0A69_000B_hilfen.md](docs/routines/0A69_000B_hilfen.md) | `src/Traders.Vga/Layers/DrawHelpers.cs` | `CommandListTests.cs` | ✅ |
| `0A69:004D` | [0A69_000B_hilfen.md](docs/routines/0A69_000B_hilfen.md) | `src/Traders.Vga/Layers/DrawHelpers.cs` | `CommandListTests.cs` | ✅ |
| `0A69:0090` | [0A69_000B_hilfen.md](docs/routines/0A69_000B_hilfen.md) | `src/Traders.Vga/Layers/DrawHelpers.cs` | `CommandListTests.cs` | ✅ |
| `0A69:00D3` | [0A69_000B_hilfen.md](docs/routines/0A69_000B_hilfen.md) | `src/Traders.Vga/Layers/DrawHelpers.cs` | `CommandListTests.cs` | ✅ |
| `0A69:011B` | [0A69_000B_hilfen.md](docs/routines/0A69_000B_hilfen.md) | `src/Traders.Vga/Layers/DrawHelpers.cs` | `CommandListTests.cs` | ✅ |
| `0A69:0164` | [0A69_000B_hilfen.md](docs/routines/0A69_000B_hilfen.md) | `src/Traders.Vga/Layers/DrawHelpers.cs` | `CommandListTests.cs` | ✅ |
| `0A69:01AD` | [0A69_000B_hilfen.md](docs/routines/0A69_000B_hilfen.md) | `src/Traders.Vga/Layers/DrawHelpers.cs` | `CommandListTests.cs` | ✅ |
| `0A69:0243` | [0A69_000B_hilfen.md](docs/routines/0A69_000B_hilfen.md) | `src/Traders.Vga/Layers/DrawHelpers.cs` | `CommandListTests.cs` | ✅ |
| `0A69:0288` | [0A69_000B_hilfen.md](docs/routines/0A69_000B_hilfen.md) | `src/Traders.Vga/Layers/DrawHelpers.cs` | `CommandListTests.cs` | ✅ |
| `0A96:000A` | [0A96_06A8_befehlsliste.md](docs/routines/0A96_06A8_befehlsliste.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `0A96:001B` | [0A96_06A8_befehlsliste.md](docs/routines/0A96_06A8_befehlsliste.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `0A96:0030` | [0A96_06A8_befehlsliste.md](docs/routines/0A96_06A8_befehlsliste.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `0A96:0089` | [0A96_06A8_befehlsliste.md](docs/routines/0A96_06A8_befehlsliste.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `0A96:00E2` | [0A96_06A8_befehlsliste.md](docs/routines/0A96_06A8_befehlsliste.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `0A96:013B` | [0A96_06A8_befehlsliste.md](docs/routines/0A96_06A8_befehlsliste.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `0A96:01AA` | [0A96_06A8_befehlsliste.md](docs/routines/0A96_06A8_befehlsliste.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `0A96:020E` | [0A96_06A8_befehlsliste.md](docs/routines/0A96_06A8_befehlsliste.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `0A96:0267` | [0A96_06A8_befehlsliste.md](docs/routines/0A96_06A8_befehlsliste.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `0A96:02D6` | [0A96_06A8_befehlsliste.md](docs/routines/0A96_06A8_befehlsliste.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `0A96:0311` | [0A96_06A8_befehlsliste.md](docs/routines/0A96_06A8_befehlsliste.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `0A96:035F` | [0A96_06A8_befehlsliste.md](docs/routines/0A96_06A8_befehlsliste.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `0A96:03AD` | [0A96_06A8_befehlsliste.md](docs/routines/0A96_06A8_befehlsliste.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `0A96:03FB` | [0A96_06A8_befehlsliste.md](docs/routines/0A96_06A8_befehlsliste.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `0A96:0433` | [0A96_06A8_befehlsliste.md](docs/routines/0A96_06A8_befehlsliste.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `0A96:0481` | [0A96_06A8_befehlsliste.md](docs/routines/0A96_06A8_befehlsliste.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `0A96:04D7` | [0A96_06A8_befehlsliste.md](docs/routines/0A96_06A8_befehlsliste.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `0A96:0525` | [0A96_06A8_befehlsliste.md](docs/routines/0A96_06A8_befehlsliste.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `0A96:057A` | [0A96_06A8_befehlsliste.md](docs/routines/0A96_06A8_befehlsliste.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `0A96:05FB` | [0A96_06A8_befehlsliste.md](docs/routines/0A96_06A8_befehlsliste.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `0A96:0686` | [0A96_06A8_befehlsliste.md](docs/routines/0A96_06A8_befehlsliste.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `0A96:06A8` | [0A96_06A8_befehlsliste.md](docs/routines/0A96_06A8_befehlsliste.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `0A96:07E4` | [0A96_06A8_befehlsliste.md](docs/routines/0A96_06A8_befehlsliste.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `0A96:0884` | [0A96_06A8_befehlsliste.md](docs/routines/0A96_06A8_befehlsliste.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `0A96:0A51` | [0A96_06A8_befehlsliste.md](docs/routines/0A96_06A8_befehlsliste.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `0A96:0C3F` | [0A96_06A8_befehlsliste.md](docs/routines/0A96_06A8_befehlsliste.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `0A96:0C68` | [0A96_06A8_befehlsliste.md](docs/routines/0A96_06A8_befehlsliste.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `0B67:0002` | [0B67_0002_seiten.md](docs/routines/0B67_0002_seiten.md) | `src/Traders.Vga/Layers/Pages.cs` | `ProgramHostTests.cs`, `PageLayerTests.cs` | ✅ |
| `0B67:016B` | [0B67_0002_seiten.md](docs/routines/0B67_0002_seiten.md) | `src/Traders.Vga/Layers/Pages.cs` | `PageLayerTests.cs` | ✅ |
| `0B67:0196` | [0B67_0002_seiten.md](docs/routines/0B67_0002_seiten.md) | `src/Traders.Vga/Layers/Pages.cs` | `PageLayerTests.cs` | ✅ |
| `0B67:01B5` | [0B67_0002_seiten.md](docs/routines/0B67_0002_seiten.md) | `src/Traders.Vga/Layers/Pages.cs` | `PageLayerTests.cs` | ✅ |
| `0B67:01D4` | [0B67_0002_seiten.md](docs/routines/0B67_0002_seiten.md) | `src/Traders.Vga/Layers/Pages.cs` | `PageLayerTests.cs` | ✅ |
| `0B67:0206` | [0B67_0002_seiten.md](docs/routines/0B67_0002_seiten.md) | `src/Traders.Vga/Layers/Pages.cs` | `PageLayerTests.cs` | ✅ |
| `0B67:0238` | [0B67_0002_seiten.md](docs/routines/0B67_0002_seiten.md) | `src/Traders.Vga/Layers/Pages.cs` | `PageLayerTests.cs` | ✅ |
| `0B67:028F` | [0B67_0002_seiten.md](docs/routines/0B67_0002_seiten.md) | `src/Traders.Vga/Layers/Pages.cs` | `PageLayerTests.cs` | ✅ |
| `0B67:0299` | [0B67_0002_seiten.md](docs/routines/0B67_0002_seiten.md) | `src/Traders.Vga/Layers/Pages.cs` | `PageLayerTests.cs` | ✅ |
| `0B9F:06A0` | [0B9F_06A0_warenpreise.md](docs/routines/0B9F_06A0_warenpreise.md) | `src/Traders.Core/Rules/WarePrices.cs` | `RandomRulesDifferentialTests.cs` | ✅ |
| `0B9F:078A` | [0B9F_06A0_warenpreise.md](docs/routines/0B9F_06A0_warenpreise.md) | `src/Traders.Core/Rules/WarePrices.cs` | `RandomRulesDifferentialTests.cs` | ✅ |
| `0D73:00D3` | [0B67_0002_seiten.md](docs/routines/0B67_0002_seiten.md) | `src/Traders.Vga/Layers/FrameSteps.cs`, `src/Traders.Vga/Layers/GraphicsLayers.cs` | `PageLayerTests.cs` | ✅ |
| `0D73:07E0` | [0E9E_000E_steuerung.md](docs/routines/0E9E_000E_steuerung.md) | `src/Traders.Core/Input/MouseReader.cs` | `ControlReaderDifferentialTests.cs` | ✅ |
| `0DFD:000D` | [0DFD_000D_programmstart.md](docs/routines/0DFD_000D_programmstart.md) | `src/Traders.Core/Startup/ProgramStart.cs` | `ProgramStartDifferentialTests.cs` | ✅ |
| `0DFD:0074` | [0DFD_000D_programmstart.md](docs/routines/0DFD_000D_programmstart.md) | – | – | 🔍 |
| `0DFD:0132` | [0DFD_000D_programmstart.md](docs/routines/0DFD_000D_programmstart.md) | – | – | 🔍 |
| `0DFD:0158` | [0DFD_000D_programmstart.md](docs/routines/0DFD_000D_programmstart.md) | – | – | 🔍 |
| `0DFD:0239` | [0DFD_000D_programmstart.md](docs/routines/0DFD_000D_programmstart.md) | `src/Traders.Core/Startup/ProgramStart.cs` | `ProgramStartDifferentialTests.cs` | ✅ |
| `0E31:0003` | [0E31_0003_neue_partie.md](docs/routines/0E31_0003_neue_partie.md) | `src/Traders.Core/Rules/NewGame.cs` | `ProgramHumanDifferentialTests.cs`, `NewGameDifferentialTests.cs` | ✅ |
| `0E31:0149` | [0E31_0003_neue_partie.md](docs/routines/0E31_0003_neue_partie.md) | `src/Traders.Core/Rules/NewGame.cs` | `NewGameDifferentialTests.cs` | ✅ |
| `0E31:01CD` | [0E31_0003_neue_partie.md](docs/routines/0E31_0003_neue_partie.md) | `src/Traders.Core/Rules/NewGame.cs` | `NewGameDifferentialTests.cs` | ✅ |
| `0E31:023D` | [0E31_0003_neue_partie.md](docs/routines/0E31_0003_neue_partie.md) | `src/Traders.Core/Rules/NewGame.cs` | `NewGameDifferentialTests.cs` | ✅ |
| `0E31:0361` | [0E31_0003_neue_partie.md](docs/routines/0E31_0003_neue_partie.md) | `src/Traders.Core/Rules/NewGameLand.cs` | `NewGameDifferentialTests.cs` | ✅ |
| `0E31:0379` | [0E31_0003_neue_partie.md](docs/routines/0E31_0003_neue_partie.md) | `src/Traders.Core/Rules/NewGameLand.cs` | `NewGameDifferentialTests.cs` | ✅ |
| `0E31:0465` | [0E31_0003_neue_partie.md](docs/routines/0E31_0003_neue_partie.md) | `src/Traders.Core/Rules/NewGameLand.cs` | `NewGameDifferentialTests.cs` | ✅ |
| `0E9E:000E` | [0E9E_000E_steuerung.md](docs/routines/0E9E_000E_steuerung.md) | `src/Traders.Core/Input/ControlReader.cs` | `ControlReaderDifferentialTests.cs` | ✅ |
| `0E9E:0035` | [0E9E_000E_steuerung.md](docs/routines/0E9E_000E_steuerung.md) | `src/Traders.Core/Input/ControlReader.cs` | `ControlReaderDifferentialTests.cs` | ✅ |
| `0E9E:0264` | [0E9E_000E_steuerung.md](docs/routines/0E9E_000E_steuerung.md) | `src/Traders.Core/Input/ControlReader.cs` | `ControlReaderDifferentialTests.cs` | ✅ |
| `0E9E:0312` | [0E9E_000E_steuerung.md](docs/routines/0E9E_000E_steuerung.md) | `src/Traders.Core/Input/ControlReader.cs` | `ControlReaderDifferentialTests.cs` | ✅ |
| `0E9E:037A` | [0E9E_000E_steuerung.md](docs/routines/0E9E_000E_steuerung.md) | `src/Traders.Core/Input/ControlReader.cs` | `ControlReaderDifferentialTests.cs` | ✅ |
| `11A3:0000` | [11A3_0000_wochenplanung.md](docs/routines/11A3_0000_wochenplanung.md) | `src/Traders.Core/Ai/WeekPlanning.cs` | `WeekPlanningDifferentialTests.cs` | ✅ |
| `11A3:007D` | [11A3_0000_wochenplanung.md](docs/routines/11A3_0000_wochenplanung.md) | `src/Traders.Core/Ai/WeekPlanning.cs` | `WeekPlanningDifferentialTests.cs` | ✅ |
| `11A3:0180` | [11A3_0000_wochenplanung.md](docs/routines/11A3_0000_wochenplanung.md) | `src/Traders.Core/Ai/WeekPlanning.cs` | `WeekPlanningDifferentialTests.cs` | ✅ |
| `11A3:043F` | [11A3_0000_wochenplanung.md](docs/routines/11A3_0000_wochenplanung.md) | `src/Traders.Core/Ai/WeekPlanning.cs` | `WeekPlanningDifferentialTests.cs` | ✅ |
| `11A3:0599` | [11A3_0000_wochenplanung.md](docs/routines/11A3_0000_wochenplanung.md) | `src/Traders.Core/Ai/WeekPlanning.cs` | `WeekPlanningDifferentialTests.cs` | ✅ |
| `11A3:061A` | [11A3_0000_wochenplanung.md](docs/routines/11A3_0000_wochenplanung.md) | `src/Traders.Core/Ai/WeekPlanning.cs` | `WeekPlanningDifferentialTests.cs` | ✅ |
| `11A3:067D` | [11A3_0000_wochenplanung.md](docs/routines/11A3_0000_wochenplanung.md) | `src/Traders.Core/Ai/WeekPlanning.cs` | `WeekPlanningDifferentialTests.cs` | ✅ |
| `11A3:078A` | [11A3_0000_wochenplanung.md](docs/routines/11A3_0000_wochenplanung.md) | `src/Traders.Core/Ai/WeekPlanning.cs` | `WeekPlanningDifferentialTests.cs` | ✅ |
| `11A3:0AA5` | [11A3_0000_wochenplanung.md](docs/routines/11A3_0000_wochenplanung.md) | `src/Traders.Core/Ai/WeekPlanning.cs` | `WeekPlanningDifferentialTests.cs` | ✅ |
| `11A3:0B3B` | [11A3_0000_wochenplanung.md](docs/routines/11A3_0000_wochenplanung.md) | `src/Traders.Core/Ai/WeekPlanning.cs` | `WeekPlanningDifferentialTests.cs` | ✅ |
| `11A3:0D35` | [11A3_0000_wochenplanung.md](docs/routines/11A3_0000_wochenplanung.md) | `src/Traders.Core/Ai/WeekPlanning.cs` | `WeekPlanningDifferentialTests.cs` | ✅ |
| `11A3:0E01` | [11A3_0000_wochenplanung.md](docs/routines/11A3_0000_wochenplanung.md) | `src/Traders.Core/Ai/WeekPlanning.cs` | `WeekPlanningDifferentialTests.cs` | ✅ |
| `11A3:0EB8` | [11A3_0000_wochenplanung.md](docs/routines/11A3_0000_wochenplanung.md) | `src/Traders.Core/Ai/WeekPlanning.cs` | `WeekPlanningDifferentialTests.cs` | ✅ |
| `11A3:0F2E` | [11A3_0000_wochenplanung.md](docs/routines/11A3_0000_wochenplanung.md) | `src/Traders.Core/Ai/WeekPlanning.cs` | `WeekPlanningDifferentialTests.cs` | ✅ |
| `11A3:104B` | [11A3_0000_wochenplanung.md](docs/routines/11A3_0000_wochenplanung.md) | `src/Traders.Core/Ai/WeekPlanning.cs` | `WeekPlanningDifferentialTests.cs` | ✅ |
| `1777:000C` | [1777_000C_main.md](docs/routines/1777_000C_main.md) | `src/Traders.Core/Phases/ProgramLoop.cs`, `src/Traders.Core/Phases/ProgramMachine.cs`, `src/Traders.Core/Phases/ProgramSteps.cs` | `ProgramHumanDifferentialTests.cs`, `ProgramMachineDifferentialTests.cs` | ✅ |
| `1777:01F8` | [1777_01F8_wochenschleife.md](docs/routines/1777_01F8_wochenschleife.md) | `src/Traders.Core/Phases/PhaseMachine.cs`, `src/Traders.Core/Phases/WeekLoop.cs` | `MultiWeekDifferentialTests.cs`, `PhaseMachineDifferentialTests.cs` | ✅ |
| `1777:02DC` | [1777_02DC_wochenpreise.md](docs/routines/1777_02DC_wochenpreise.md) | `src/Traders.Core/Rules/WeeklyPrices.cs` | `WeeklyRulesDifferentialTests.cs` | ✅ |
| `1777:04E4` | [1777_04E4_initialisierung.md](docs/routines/1777_04E4_initialisierung.md) | `src/Traders.Core/Phases/ProgramSteps.cs` | `ProgramMachineDifferentialTests.cs` | ✅ |
| `186C:0002` | [186C_ressourcenlader.md](docs/routines/186C_ressourcenlader.md) | `src/Traders.Core/Startup/ImageTable.cs` | `ProgramStartDifferentialTests.cs` | ✅ |
| `186C:01A9` | [186C_ressourcenlader.md](docs/routines/186C_ressourcenlader.md) | – | – | 🔍 |
| `186C:021E` | [186C_ressourcenlader.md](docs/routines/186C_ressourcenlader.md) | – | – | 🔍 |
| `186C:02C6` | [186C_ressourcenlader.md](docs/routines/186C_ressourcenlader.md) | – | – | 🔍 |
| `186C:0997` | [186C_ressourcenlader.md](docs/routines/186C_ressourcenlader.md) | `src/Traders.Core/Startup/ImageTable.cs` | `ProgramStartDifferentialTests.cs` | ✅ |
| `186C:0AB1` | [186C_ressourcenlader.md](docs/routines/186C_ressourcenlader.md) | – | – | 🔍 |
| `186C:0B0A` | [186C_ressourcenlader.md](docs/routines/186C_ressourcenlader.md) | – | – | 🔍 |
| `192D:0007` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | `MainMenuDifferentialTests.cs`, `ProgramHumanDifferentialTests.cs` | ✅ |
| `192D:0058` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | `MainMenuDifferentialTests.cs` | ✅ |
| `192D:00C5` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Core/Menus/MainMenu.cs`, `src/Traders.Presentation/Menus/MenuScene.cs` | `MainMenuDifferentialTests.cs`, `MenuSceneDifferentialTests.cs` | ✅ |
| `192D:0234` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | `MainMenuDifferentialTests.cs`, `MenuFrameDifferentialTests.cs` | ✅ |
| `192D:0376` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | `MainMenuDifferentialTests.cs` | ✅ |
| `192D:03AB` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Presentation/Menus/MenuScene.cs` | `MenuSceneDifferentialTests.cs` | ✅ |
| `192D:03E3` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | `MainMenuDifferentialTests.cs` | ✅ |
| `192D:040E` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | `MainMenuDifferentialTests.cs` | ✅ |
| `192D:042F` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Presentation/Menus/MenuScene.cs` | `MenuSceneDifferentialTests.cs` | ✅ |
| `192D:0495` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | `MainMenuDifferentialTests.cs`, `MenuFrameDifferentialTests.cs` | ✅ |
| `192D:0592` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | `MainMenuDifferentialTests.cs` | ✅ |
| `192D:05DD` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | `MainMenuDifferentialTests.cs` | ✅ |
| `192D:0653` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Presentation/Menus/MenuScene.cs` | `MenuSceneDifferentialTests.cs` | ✅ |
| `192D:0698` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | `MainMenuDifferentialTests.cs` | ✅ |
| `192D:06C2` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | – | 🟢 über `192D:0698` |
| `192D:06E2` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | – | 🟢 über `192D:0698` |
| `192D:0702` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | – | 🟢 über `192D:0698` |
| `192D:0722` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | – | 🟢 über `192D:0698` |
| `192D:0742` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | – | 🟢 über `192D:0698` |
| `192D:076C` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Presentation/Menus/MenuScene.cs` | `MenuSceneDifferentialTests.cs` | ✅ |
| `192D:07AC` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Presentation/Menus/MenuScene.cs` | `MenuSceneDifferentialTests.cs` | ✅ |
| `192D:07FC` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Presentation/Menus/MenuScene.cs` | `MenuSceneDifferentialTests.cs` | ✅ |
| `192D:084A` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Presentation/Menus/MenuScene.cs` | `MenuSceneDifferentialTests.cs` | ✅ |
| `192D:0899` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Presentation/Menus/MenuScene.cs` | `MenuSceneDifferentialTests.cs` | ✅ |
| `192D:08E7` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Presentation/Menus/MenuScene.cs` | `MenuSceneDifferentialTests.cs` | ✅ |
| `192D:0935` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Presentation/Menus/MenuScene.cs` | `MenuSceneDifferentialTests.cs` | ✅ |
| `192D:09B0` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Presentation/Menus/MenuScene.cs` | `MenuSceneDifferentialTests.cs` | ✅ |
| `192D:0A05` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | `MainMenuDifferentialTests.cs` | ✅ |
| `192D:0A26` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Presentation/Menus/MenuScene.cs` | `MenuSceneDifferentialTests.cs` | ✅ |
| `192D:0AB4` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | `MainMenuDifferentialTests.cs` | ✅ |
| `192D:0C08` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | `MainMenuDifferentialTests.cs` | ✅ |
| `192D:0C34` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | – | 🟢 über `192D:0C08` |
| `192D:0C75` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | – | 🟢 über `192D:0C08` |
| `192D:0C9E` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | – | 🟢 über `192D:0C08` |
| `192D:0CC7` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | – | 🟢 über `192D:0C08` |
| `192D:0CE7` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | – | 🟢 über `192D:0C08` |
| `192D:0D10` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | `MainMenuDifferentialTests.cs` | ✅ |
| `192D:0D42` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | – | 🟢 über `192D:0D10` |
| `192D:0DC0` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Presentation/Menus/MenuScene.cs` | `MenuSceneDifferentialTests.cs` | ✅ |
| `192D:0E11` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Presentation/Menus/MenuScene.cs` | `MenuSceneDifferentialTests.cs` | ✅ |
| `192D:0E64` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Presentation/Menus/MenuScene.cs` | `MenuSceneDifferentialTests.cs` | ✅ |
| `192D:0E8E` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Presentation/Menus/MenuScene.cs` | `MenuSceneDifferentialTests.cs` | ✅ |
| `192D:0EAE` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Presentation/Menus/MenuScene.cs` | `MenuSceneDifferentialTests.cs` | ✅ |
| `192D:0ED9` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Presentation/Menus/MenuScene.cs` | `MenuSceneDifferentialTests.cs` | ✅ |
| `192D:0EF8` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Presentation/Menus/MenuScene.cs` | `MenuSceneDifferentialTests.cs` | ✅ |
| `192D:0F16` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Presentation/Menus/MenuScene.cs` | `MenuSceneDifferentialTests.cs` | ✅ |
| `192D:0F6E` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Presentation/Menus/MenuScene.cs` | `MenuSceneDifferentialTests.cs` | ✅ |
| `192D:0F96` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | `MainMenuDifferentialTests.cs` | ✅ |
| `192D:0FB7` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Presentation/Menus/MenuScene.cs` | `MenuSceneDifferentialTests.cs` | ✅ |
| `192D:10F8` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | `MainMenuDifferentialTests.cs` | ✅ |
| `192D:120C` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Presentation/Menus/MenuScene.cs` | `MenuSceneDifferentialTests.cs` | ✅ |
| `192D:1263` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | `MainMenuDifferentialTests.cs` | ✅ |
| `192D:12A6` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | `MainMenuDifferentialTests.cs` | ✅ |
| `192D:131B` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MenuBackdrop.cs` | `MainMenuDifferentialTests.cs` | ✅ |
| `192D:138F` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MenuBackdrop.cs` | `MainMenuDifferentialTests.cs` | ✅ |
| `192D:14B4` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MenuBackdrop.cs` | `MainMenuDifferentialTests.cs` | ✅ |
| `192D:14FB` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MenuBackdrop.cs` | `MainMenuDifferentialTests.cs` | ✅ |
| `192D:16AF` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Core/Menus/MainMenu.cs`, `src/Traders.Core/Startup/MenuTextPlaque.cs`, `src/Traders.Presentation/Menus/MenuScene.cs` | `MainMenuDifferentialTests.cs`, `MenuSceneDifferentialTests.cs`, `ProgramStartDifferentialTests.cs` | ✅ |
| `192D:17F9` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Presentation/Menus/MenuScene.cs` | `MenuSceneDifferentialTests.cs` | ✅ |
| `192D:1855` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Presentation/Menus/MenuScene.cs` | `MenuSceneDifferentialTests.cs` | ✅ |
| `192D:18B6` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MenuTexts.cs` | `MainMenuDifferentialTests.cs` | ✅ |
| `192D:197F` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | `MainMenuDifferentialTests.cs` | ✅ |
| `192D:19A4` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Presentation/Menus/MenuScene.cs` | `MenuSceneDifferentialTests.cs` | ✅ |
| `192D:1A04` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | `MainMenuDifferentialTests.cs`, `MenuFrameDifferentialTests.cs` | ✅ |
| `192D:1B06` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Presentation/Menus/MenuScene.cs` | `MenuSceneDifferentialTests.cs` | ✅ |
| `192D:1B90` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Core/Menus/MainMenu.cs`, `src/Traders.Presentation/Menus/MenuScene.cs` | `MainMenuDifferentialTests.cs`, `MenuSceneDifferentialTests.cs` | ✅ |
| `21AF:000D` | [21AF_000D_objekte.md](docs/routines/21AF_000D_objekte.md) | `src/Traders.Vga/Layers/Objects.cs` | `ObjectLayerTests.cs` | ✅ |
| `21AF:00CA` | [21AF_000D_objekte.md](docs/routines/21AF_000D_objekte.md) | `src/Traders.Vga/Layers/Objects.cs` | `ObjectLayerTests.cs` | ✅ |
| `21AF:022B` | [21AF_000D_objekte.md](docs/routines/21AF_000D_objekte.md) | `src/Traders.Vga/Layers/Objects.cs` | `ObjectLayerTests.cs` | ✅ |
| `21AF:0412` | [21AF_000D_objekte.md](docs/routines/21AF_000D_objekte.md) | `src/Traders.Vga/Layers/Objects.cs` | `ObjectLayerTests.cs` | ✅ |
| `21AF:045E` | [21AF_000D_objekte.md](docs/routines/21AF_000D_objekte.md) | `src/Traders.Core/Field/FieldCursor.cs`, `src/Traders.Vga/Layers/Objects.cs` | `FieldCursorDifferentialTests.cs`, `ObjectLayerTests.cs` | ✅ |
| `21AF:0472` | [21AF_000D_objekte.md](docs/routines/21AF_000D_objekte.md) | `src/Traders.Vga/Layers/Objects.cs` | `ObjectLayerTests.cs` | ✅ |
| `21AF:04A2` | [21AF_000D_objekte.md](docs/routines/21AF_000D_objekte.md) | `src/Traders.Vga/Layers/Palettes.cs` | `PaletteLayerTests.cs` | ✅ |
| `21AF:04CD` | [21AF_000D_objekte.md](docs/routines/21AF_000D_objekte.md) | `src/Traders.Vga/Layers/Palettes.cs` | `PaletteLayerTests.cs` | ✅ |
| `21FD:0185` | [21FD_026C_paletten.md](docs/routines/21FD_026C_paletten.md) | `src/Traders.Vga/Layers/GraphicsLayers.cs` | `PaletteLayerTests.cs` | ✅ |
| `21FD:01BA` | [21FD_01BA_random.md](docs/routines/21FD_01BA_random.md) | `src/Traders.Core/OriginalRandom.cs` | `ProgramHostTests.cs`, `RandomDifferentialTests.cs` | ✅ |
| `21FD:021F` | [21FD_021F_wetter.md](docs/routines/21FD_021F_wetter.md) | `src/Traders.Core/Rules/WeatherRule.cs` | `RandomRulesDifferentialTests.cs` | ✅ |
| `21FD:0245` | [192D_0007_menues.md](docs/routines/192D_0007_menues.md) | `src/Traders.Core/Menus/MainMenu.cs` | `MainMenuDifferentialTests.cs`, `MenuFrameDifferentialTests.cs` | ✅ |
| `21FD:026C` | [21FD_026C_paletten.md](docs/routines/21FD_026C_paletten.md) | `src/Traders.Vga/Layers/Palettes.cs` | `PaletteLayerTests.cs` | ✅ |
| `21FD:02C9` | [21FD_026C_paletten.md](docs/routines/21FD_026C_paletten.md) | `src/Traders.Vga/Layers/Palettes.cs` | `PaletteLayerTests.cs` | ✅ |
| `21FD:0329` | [21FD_026C_paletten.md](docs/routines/21FD_026C_paletten.md) | `src/Traders.Vga/Layers/FrameSteps.cs`, `src/Traders.Vga/Layers/Palettes.cs` | `FrameStepsTests.cs`, `PaletteLayerTests.cs` | ✅ |
| `21FD:0353` | [21FD_026C_paletten.md](docs/routines/21FD_026C_paletten.md) | `src/Traders.Vga/Layers/FrameSteps.cs`, `src/Traders.Vga/Layers/Palettes.cs` | `FrameStepsTests.cs`, `PaletteLayerTests.cs` | ✅ |
| `21FD:037D` | [21FD_026C_paletten.md](docs/routines/21FD_026C_paletten.md) | `src/Traders.Vga/Layers/Palettes.cs` | `PaletteLayerTests.cs` | ✅ |
| `21FD:03B4` | [21FD_026C_paletten.md](docs/routines/21FD_026C_paletten.md) | `src/Traders.Vga/Layers/FrameSteps.cs`, `src/Traders.Vga/Layers/Palettes.cs` | `FrameStepsTests.cs`, `PaletteLayerTests.cs` | ✅ |
| `21FD:03EB` | [21FD_026C_paletten.md](docs/routines/21FD_026C_paletten.md) | `src/Traders.Vga/Layers/FrameSteps.cs`, `src/Traders.Vga/Layers/Palettes.cs` | `PaletteLayerTests.cs` | ✅ |
| `21FD:04DA` | [21FD_026C_paletten.md](docs/routines/21FD_026C_paletten.md) | `src/Traders.Vga/Layers/Palettes.cs` | `PaletteLayerTests.cs` | ✅ |
| `21FD:051F` | [21FD_051F_seitenwechsel.md](docs/routines/21FD_051F_seitenwechsel.md) | `src/Traders.Core/Menus/MainMenu.cs`, `src/Traders.Vga/Layers/FrameSteps.cs`, `src/Traders.Vga/Layers/Pages.cs` | `MenuFrameDifferentialTests.cs`, `FrameStepsTests.cs`, `PageLayerTests.cs` | ✅ |
| `21FD:05DB` | [192D_00C5_menueszene.md](docs/routines/192D_00C5_menueszene.md) | `src/Traders.Presentation/Menus/MenuScene.cs` | `MenuSceneDifferentialTests.cs` | ✅ |
| `21FD:0687` | [21FD_0687_codeabfrage.md](docs/routines/21FD_0687_codeabfrage.md) | `src/Traders.Core/Startup/CodeQuery.cs` | `ProgramStartDifferentialTests.cs` | ✅ |
| `21FD:081D` | [21FD_0687_codeabfrage.md](docs/routines/21FD_0687_codeabfrage.md) | `src/Traders.Core/Startup/CodeQuery.cs` | `CodeQueryDifferentialTests.cs` | ✅ |
| `21FD:08D3` | [21FD_0687_codeabfrage.md](docs/routines/21FD_0687_codeabfrage.md) | `src/Traders.Core/Startup/CodeQuery.cs` | `CodeQueryDifferentialTests.cs` | ✅ |
| `2295:0001` | [2295_0001_zahl.md](docs/routines/2295_0001_zahl.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `2295:0062` | [2295_0001_zahl.md](docs/routines/2295_0001_zahl.md) | `src/Traders.Vga/Layers/CommandList.cs` | `CommandListTests.cs` | ✅ |
| `2295:006D` | [2295_006D_farbregister.md](docs/routines/2295_006D_farbregister.md) | `src/Traders.Vga/Layers/Palettes.cs` | `PaletteLayerTests.cs` | ✅ |
| `2295:00C5` | [2295_006D_farbregister.md](docs/routines/2295_006D_farbregister.md) | `src/Traders.Vga/Layers/Palettes.cs` | `PaletteLayerTests.cs` | ✅ |
| `22A3:0003` | [22A3_0003_produktion.md](docs/routines/22A3_0003_produktion.md) | `src/Traders.Core/Rules/Production.cs` | `WealthProductionDifferentialTests.cs` | ✅ |
| `22A3:026B` | [22A3_026B_schwund.md](docs/routines/22A3_026B_schwund.md) | `src/Traders.Core/Rules/StockDecay.cs` | `WeeklyRulesDifferentialTests.cs` | ✅ |
| `22A3:0750` | [22A3_0003_produktion.md](docs/routines/22A3_0003_produktion.md) | `src/Traders.Core/Rules/Production.cs` | `WealthProductionDifferentialTests.cs` | ✅ |
| `2336:000A` | [2336_000A_spielstand.md](docs/routines/2336_000A_spielstand.md) | `src/Traders.Core/State/SaveGame.cs` | `SaveGameDifferentialTests.cs` | ✅ |
| `2336:0539` | [2336_000A_spielstand.md](docs/routines/2336_000A_spielstand.md) | `src/Traders.Core/State/SaveGame.cs` | `SaveGameDifferentialTests.cs` | ✅ |
| `23CD:04F6` | [23CD_04F6_kartenwahl.md](docs/routines/23CD_04F6_kartenwahl.md) | `src/Traders.Vga/ModeXSetup.cs` | `DriverEdgeCaseTests.cs` | ✅ |
| `23CD:05B6` | [23CD_05DD_text.md](docs/routines/23CD_05DD_text.md) | `src/Traders.Vga/ModeXDriver.cs` | `DriverDifferentialTests.cs` | ✅ |
| `23CD:05C3` | [23CD_05DD_text.md](docs/routines/23CD_05DD_text.md) | `src/Traders.Vga/ModeXDriver.cs` | `DriverDifferentialTests.cs` | ✅ |
| `23CD:05D0` | [23CD_311A_seiten.md](docs/routines/23CD_311A_seiten.md) | `src/Traders.Vga/ModeXDriver.cs` | `DriverDifferentialTests.cs` | ✅ |
| `23CD:05DD` | [23CD_05DD_text.md](docs/routines/23CD_05DD_text.md) | `src/Traders.Vga/ModeXDriver.cs` | `DriverDifferentialTests.cs` | ✅ |
| `23CD:06BE` | [23CD_378B_linie.md](docs/routines/23CD_378B_linie.md) | `src/Traders.Vga/ModeXLines.cs` | `DriverLineAndObjectTests.cs` | ✅ |
| `23CD:06E9` | [23CD_06E9_objekte.md](docs/routines/23CD_06E9_objekte.md) | `src/Traders.Vga/Layers/Objects.cs`, `src/Traders.Vga/ModeXObjects.cs` | `DriverLineAndObjectTests.cs`, `ObjectLayerTests.cs` | ✅ |
| `23CD:071C` | [23CD_06E9_objekte.md](docs/routines/23CD_06E9_objekte.md) | `src/Traders.Vga/Layers/Objects.cs`, `src/Traders.Vga/ModeXObjects.cs` | `DriverLineAndObjectTests.cs`, `ObjectLayerTests.cs` | ✅ |
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
| `27C6:0000` | [27C6_0043_eingabe_zeitgeber.md](docs/routines/27C6_0043_eingabe_zeitgeber.md) | – | – | 🔍 |
| `27C6:0027` | [27C6_0043_eingabe_zeitgeber.md](docs/routines/27C6_0043_eingabe_zeitgeber.md) | – | – | 🔍 |
| `27C6:0043` | [27C6_0043_eingabe_zeitgeber.md](docs/routines/27C6_0043_eingabe_zeitgeber.md) | `src/Traders.App/Input/KeyStateTable.cs` | `HostInputDifferentialTests.cs` | ✅ |
| `27C6:00AC` | [27C6_0043_eingabe_zeitgeber.md](docs/routines/27C6_0043_eingabe_zeitgeber.md) | `src/Traders.App/Input/KeyStateTable.cs` | `HostInputDifferentialTests.cs` | ✅ |
| `27C6:00D2` | [27C6_0043_eingabe_zeitgeber.md](docs/routines/27C6_0043_eingabe_zeitgeber.md) | – | – | 🔍 |
| `27C6:0268` | [0E9E_000E_steuerung.md](docs/routines/0E9E_000E_steuerung.md) | `src/Traders.Core/Input/JoystickReader.cs` | `ControlReaderDifferentialTests.cs` | ✅ |
| `27C6:037A` | [27C6_0043_eingabe_zeitgeber.md](docs/routines/27C6_0043_eingabe_zeitgeber.md) | `src/Traders.App/Timing/HostClock.cs` | `HostTimerTests.cs` | ✅ |
| `27C6:03C3` | [27C6_0043_eingabe_zeitgeber.md](docs/routines/27C6_0043_eingabe_zeitgeber.md) | – | – | 🔍 |
| `27C6:03E4` | [27C6_0043_eingabe_zeitgeber.md](docs/routines/27C6_0043_eingabe_zeitgeber.md) | `src/Traders.App/Timing/TimerTicks.cs` | `HostTimerTests.cs` | ✅ |
| `27C6:041B` | [27C6_0043_eingabe_zeitgeber.md](docs/routines/27C6_0043_eingabe_zeitgeber.md) | – | – | 🔍 |
| `27C6:0551` | [27C6_0043_eingabe_zeitgeber.md](docs/routines/27C6_0043_eingabe_zeitgeber.md) | – | – | 🔍 |
| `27C6:0576` | [27C6_0043_eingabe_zeitgeber.md](docs/routines/27C6_0043_eingabe_zeitgeber.md) | – | – | 🔍 |

Status: ✅ portiert mit eigenem Differenztest (`[DIFFERENTIAL]`) · 🟢 portiert, Differenztest fehlt · 🔍 nur Befund · „über `X`“: Befund vom Listenkopf `X` geerbt (nur Far-Einsprünge desselben Segments; Differenztest wird nie geerbt, nicht in der Segmenttabelle gezählt).

### Abdeckung je Segment

| Segment | Größe | Far-Einsprünge | mit Befund | portiert | mit Differenztest |
|---|---:|---:|---:|---:|---:|
| `0000` | 0x5350 | 76 | 1 | 1 | 1 |
| `0535` | 0xD40 | 24 | 0 | 0 | 0 |
| `0609` | 0x21C0 | 56 | 20 | 19 | 19 |
| `0825` | 0xAA0 | 16 | 7 | 7 | 7 |
| `08CF` | 0x19A0 | 42 | 1 | 1 | 1 |
| `0A69` | 0x2D0 | 9 | 10 | 10 | 10 |
| `0A96` | 0xD10 | 24 | 27 | 27 | 27 |
| `0B67` | 0x380 | 8 | 9 | 9 | 9 |
| `0B9F` | 0x8A0 | 16 | 2 | 2 | 2 |
| `0C29` | 0x14A0 | 29 | 0 | 0 | 0 |
| `0D73` | 0x8A0 | 15 | 2 | 2 | 2 |
| `0DFD` | 0x340 | 5 | 5 | 2 | 2 |
| `0E31` | 0x6D0 | 7 | 7 | 7 | 7 |
| `0E9E` | 0x400 | 5 | 5 | 5 | 5 |
| `0EDE` | 0x23F0 | 56 | 0 | 0 | 0 |
| `111D` | 0x860 | 7 | 0 | 0 | 0 |
| `11A3` | 0x1250 | 15 | 15 | 15 | 15 |
| `12C8` | 0x950 | 12 | 0 | 0 | 0 |
| `135D` | 0xC00 | 14 | 0 | 0 | 0 |
| `141D` | 0xF20 | 17 | 0 | 0 | 0 |
| `150F` | 0x1C70 | 41 | 0 | 0 | 0 |
| `16D6` | 0xA10 | 12 | 0 | 0 | 0 |
| `1777` | 0x540 | 4 | 4 | 4 | 4 |
| `17CB` | 0xA10 | 9 | 0 | 0 | 0 |
| `186C` | 0xC10 | 19 | 7 | 2 | 2 |
| `192D` | 0x1D60 | 74 | 55 | 55 | 55 |
| `1B03` | 0x1650 | 44 | 0 | 0 | 0 |
| `1C68` | 0x1870 | 40 | 0 | 0 | 0 |
| `1DEF` | 0x8F0 | 14 | 0 | 0 | 0 |
| `1E7E` | 0xE50 | 19 | 0 | 0 | 0 |
| `1F63` | 0x1500 | 27 | 0 | 0 | 0 |
| `20B3` | 0xD80 | 27 | 0 | 0 | 0 |
| `218B` | 0x1C0 | 5 | 0 | 0 | 0 |
| `21A7` | 0x80 | 3 | 0 | 0 | 0 |
| `21AF` | 0x4E0 | 7 | 8 | 8 | 8 |
| `21FD` | 0x980 | 28 | 17 | 17 | 17 |
| `2295` | 0xE0 | 4 | 4 | 4 | 4 |
| `22A3` | 0x930 | 15 | 3 | 3 | 3 |
| `2336` | 0x970 | 4 | 2 | 2 | 2 |
| `23CD` | 0x3F90 | 25 | 29 | 29 | 29 |
| `27C6` | 0x5B0 | 15 | 12 | 5 | 5 |
| `2821` | 0x2F0 | 1 | 0 | 0 | 0 |
| `2850` | 0x8A0 | 1 | 0 | 0 | 0 |
| `28DA` | 0x10 | 0 | 0 | 0 | 0 |
| `28DB` | 0x10 | 0 | 0 | 0 | 0 |
| **Summe** | **0x28DC0** | **891** | **252** | **236** | **236** |

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
