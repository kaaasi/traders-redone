# Übergabebericht AP1 – Referenz-Emulator

- **Branch:** `dev/ap1-refemu`
- **Stand (Commit):** `50e65fc` (Code und Doku; dieser Bericht kommt als eigener Commit darüber)
- **CI für diesen Commit:** grün für den Code-Stand `5ac3819` (Linux + Windows, 376 Tests);
  Ergebnis für den Berichts-Commit siehe `ci-results` → `runs/<sha>/SUMMARY.md`
- **Bereit zur Übernahme:** ja

## Erledigt seit letzter Übernahme
- **80286-Interpreter** (Realmodus, 8086 + 186/286-Befehle) `src/Traders.RefEmu/Cpu/`:
  besteht alle 326 Befehlsformen der Sammlung *SingleStepTests/80286* (13 040 Fälle, aufgezeichnet
  an einem echten 80C286, MIT-Lizenz). Testdaten 1 MB in `tests/Traders.Tests/RefEmu/Data/`,
  Erzeugung mit `src/Traders.RefEmu/Tools/make_cputests.py`.
- **PC-Maschine** `src/Traders.RefEmu/Machine/`, `Hardware/`: PIC, PIT, Tastatur-Controller,
  VGA auf Registerebene (Latches, Schreibmodi, Chain-4/Mode X, Seitenumschaltung, DAC, Retrace),
  stilles AdLib (Erkennung gelingt), Host-Stubs für alle Vektoren (Spiel kann Vektoren umbiegen
  und weiterleiten), DOS-Dienste (Dateien nur lesend + Schreib-Overlay im Speicher, Speicherblöcke,
  Datum/Zeit, Vektoren, Suche), BIOS (int 10h/16h/1Ah/11h/12h), `int 64h` wird aufgezeichnet.
- **MZ-Lader** mit 3394 Relokationen, PSP, Umgebung; Ladesegment `0x1000` (Ladebild-Segment + 0x1000).
- **Deterministisches Zeitmodell:** Zeit aus Befehlszähler (3 Mio. Befehle/s), DOS-Uhr ab
  01.06.1991 12:00, geplante Tastendrücke. Zwei Läufe sind bitgleich (Test).
- **Bildabzug** `FrameDump` (PNG, Hash). **Ereignisprotokoll** aller Host-Dienste.
- **T.EXE läuft bis ins Hauptmenü** (17,3 s emuliert, ca. 2 s Rechenzeit); Referenzbilder
  `tests/Traders.Tests/RefEmu/Data/refemu-codeabfrage.png`, `refemu-hauptmenue.png`.
- **Erster Differenztest:** `random()` (21FD:01BA) Original im Emulator gegen `OriginalRandom`:
  3000 Aufrufe + Grenzfälle + 5 Start-Seeds + Uhr-Neusaat über Borland `time()` + `n = -1` – alle gleich.
- **Schnittstelle für andere Pakete:** `OriginalProgram.LoadOnly(dir)` / `BootToMainMenu(dir)`,
  `Call(imageSeg, off, args…)`, `Read/WriteWord/Dword(dsOffset)`, `DataSegment()`, `Frame()`.
- Doku: `docs/analyse/REFEMU.md`, Befunde `docs/routines/21FD_0687_codeabfrage.md` (neu),
  `docs/routines/21FD_01BA_random.md` (ergänzt).

## Offen / nächste Schritte
- Eingabeskript über das Hauptmenü hinaus (Spielerwahl, Spielstart) und Zustandsabzüge je Phase
  für Differenztests (Vorschlag: `OriginalProgram.BootToGameStart`, sobald AP5 die Phasen kennt).
- Aufzeichnung der Grafiktreiber-Aufrufe (0A96/0A69) als Spur für AP2 (Hook an Far-Call-Adressen).
- OPL-Registerschreibzugriffe als Spur für AP4 ausgeben (Haken `AdLibStub.RegisterWritten` ist da).
- Optional: Leistung (derzeit ≈ 25 Mio. Befehle/s), Split-Screen/Line-Compare der VGA (vom Spiel
  bisher nicht genutzt).

## Vorschläge für STATUS.md
| Bereich (Meilenstein/Modul/Routine) | neuer Status | Bemerkung |
|---|---|---|
| M2 Analyse-Infrastruktur / RefEmu | 🟢 | 286-Emulator (Einzelschritt-Tests grün), DOS/BIOS/VGA, T.EXE bis Hauptmenü, Bildabzug |
| `21FD:01BA` random(n) | ✅ | Differenztest gegen das ausgeführte Original grün |
| `0000:3405` time() | ✅ | Borland EST5EDT-Umrechnung belegt (Seed = lokale Zeit + 18000 − 3600 s im Sommer) |
| `21FD:0687` Code-Abfrage | 🔍 | Befund angelegt; nimmt jede Eingabe an, setzt RNG aus der Uhr |
| `218B` Ton: Treiberaufrufe int 64h | 🔍 | Aufrufe beim Start im RefEmu-Protokoll (Fkt. 6, 5, 24× 10h, 12h, 0, 18h, 1Bh, 1Ah, 1) |
| `27C6` (neu zuordnen) | 🔍 | Interrupt-Handler des Spiels: int 9 (Tastatur), int 8 (Timer 4×), int 21h |
| Werkzeuge: CI-Experiment RefEmu-Bootlauf | ✅ | nur auf `dev/ap1-refemu` |

## Neue Erkenntnisse, die andere Pakete betreffen
- **Alle:** Differenztests über `Traders.RefEmu.OriginalProgram` (Beispiel:
  `tests/Traders.Tests/RefEmu/RandomDifferentialTests.cs`). Ladebild-Segment + `0x1000` = Emulator-Segment.
- **AP3/AP5/M4:** Der Zufallsgenerator wird bei der Code-Abfrage vor dem Hauptmenü per `random(0)`
  aus der Uhr gesetzt (danach `random(0x3FF)`). Borland `time()` = lokale Zeit + 18000 s (− 3600 s
  bei Sommerzeit) → für Wiederholung den `time()`-Wert als Eingabe aufzeichnen.
- **AP5:** Interrupt-Handler `27C6:0043` (int 9, letzter Make-Code `DS:1158`, Tastentabelle
  `DS:3D47`), `27C6:03E4` (int 8: Zähler `DS:117E`/`DS:1180`, Rückruf über far `[27C6:0416]`, PIT 0x4000),
  `27C6:058B` (int 21h, fängt Get/Set-Vector 8 ab). Startreihenfolge und Zeiten in `docs/analyse/REFEMU.md`.
- **AP2:** Das Spiel arbeitet in Mode X (Chain-4 aus, Byte-Modus) mit Seitenumschaltung über die
  CRTC-Startadresse; Paletten-Ein-/Ausblendungen sind im Bootlauf sichtbar (Frames im CI-Experiment).
  Die Referenzbilder aus dem RefEmu sind pixelgenaue Zielbilder für den Grafik-Kern.
- **AP4:** Das Spiel erkennt AdLib selbst (Timer-Test an 388h) und ruft nur dann `int 64h`.
  Beim Start: Fkt. 6, 5, dann 24× Fkt. 10h (BX = 0…23, CX:DX = far-Zeiger), später Fkt. 12h
  (Status in BL), 0 (BX:CX = Daten), 18h, 1Bh, 1Ah, 1. Volles Protokoll im CI-Experiment (`events.log`, Art `Sound`).
- **Alle (Video):** Die `T.EXE` ist „(C) 1992“ mit anderen Credits als Video 1 („(C) 1991“, mit
  Linel-Intro). Das Video zeigt also eine andere Fassung – bei Abweichungen gilt das RefEmu.

## Anfragen an die Leitstelle
- `ci/experiment.sh` bleibt auf dem Branch (Bootlauf mit Bildern); bitte nicht nach `main` übernehmen
  (wie vorgesehen). Falls der Bootlauf dauerhaft in CI laufen soll: eigener Schritt im Workflow
  `REFEMU_OUT=<abs. Pfad> dotnet test --filter FullyQualifiedName~RefEmu.BootTests` (Ausgabepfad muss absolut sein).
- Bitte die Status-Vorschläge oben in `STATUS.md` übernehmen und das Modul `27C6` in der Modultabelle ergänzen.
