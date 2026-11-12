# Referenz-Emulator (`src/Traders.RefEmu`)

Führt die originale `T.EXE` deterministisch aus, um Differenztests (C#-Port gegen Original)
und Referenzspuren (Zustand, Grafik, Ton-Aufrufe) zu erzeugen.

## Aufbau

| Teil | Datei | Inhalt |
|---|---|---|
| CPU | `Cpu/Cpu286.cs` | 80286 im Realmodus (8086-Befehle + 186/286-Ergänzungen), Fehlerverhalten wie der echte 286 |
| Speicher | `Cpu/Memory.cs` | 1 MB + HMA, A20 (aus = Umlauf bei 1 MB), Geräte-Fenster (VGA bei A0000–AFFFF) |
| Maschine | `Machine/PcMachine*.cs` | Laufschleife, Zeitmodell, Interrupt-Zustellung, Host-Stubs, DOS (int 21h), BIOS (int 10h/16h/1Ah/11h/12h), `int 64h`-Aufzeichnung |
| Lader | `Machine/MzLoader.cs` | MZ-Kopf, Ladebild, Relokationen; PSP, Umgebung, Speicherblöcke in `LoadProgram` |
| Dateien | `Machine/DosFileSystem.cs` | Laufwerk C: = Originalordner, nur lesend; Schreibzugriffe nur im Speicher |
| Hardware | `Hardware/*.cs` | PIC 8259, PIT 8253, Tastatur-Controller (60h/61h/64h), VGA auf Registerebene, Bildabzug |
| Protokoll | `Machine/EventLog.cs` | Alle Host-Dienste mit Befehlszähler und emulierter Zeit |
| Schnittstelle | `OriginalProgram.cs` | Adressen wie in der Analyse (Ladebild-Segment, DS-Offset), `LoadOnly`, `BootToMainMenu`, `Call` |
| AdLib | `Hardware/AdLibStub.cs` | Stilles OPL2 bei 388h: Registerdatei + Timer-Status für die Kartenerkennung |
| Tastatur | `Hardware/Scancodes.cs`, `PcMachine.ScheduleKey` | Set-1-Scancodes; Tastendruck = Make-Code, Break-Code nach Haltezeit (Vorgabe 0,1 s) |
| TSR | `PcMachine.Dos.cs` (`RunResidentProgram`, `int 21h/31h`, `int 27h`) | residente Programme vor `T.EXE` (FXDRIVER wie in `TRADERS.BAT`); `T.EXE` lädt dann darüber |
| Code-Haken | `PcMachine.AddCodeHook` | Rückruf vor dem Befehl an `seg:off`; nur beobachten, nichts ändern |
| Spuren | `Machine/CallTracer.cs`, `Machine/OplTrace.cs` | far-Aufrufe in Ladebild-Segmente (z. B. Grafiktreiber `0A96`/`0A69`) mit Argumenten und Rückgabe; jeder OPL2-Registerzugriff mit Zeit |

## Prüfung der CPU

`tests/Traders.Tests/RefEmu/Cpu286SingleStepTests.cs` führt 13 040 Einzelschritt-Tests aus der
Sammlung *SingleStepTests/80286* (MIT, Daniel Balsom; aufgezeichnet an einem echten Harris
80C286) aus – 40 je Befehlsform, alle 326 Formen des Realmodus. Undefinierte Flags werden nach
den Masken der Sammlung ausgeblendet. Die Testdaten (`tests/Traders.Tests/RefEmu/Data/cputests-286.bin.gz`,
1 MB) erzeugt `src/Traders.RefEmu/Tools/make_cputests.py` aus den MOO-Dateien der Sammlung.

Bewusst nachgebildete 286-Eigenheiten (alle durch die Tests belegt):

- `PUSH SP` legt den alten Wert ab; Schiebezähler werden auf 5 Bit maskiert.
- Ungültige Opcodes und Registerformen (`LEA reg,reg`, `LES/LDS` mit Register, `POP CS` …) → `int 6`.
- Wortzugriff auf Offset `FFFF` → `int 13`; der Befehl wird mit der Adresse des ersten Präfixbytes
  neu gestartet. Befehle über 10 Byte → `int 13`.
- Teilende Befehle (`DIV`, `IDIV`, `AAM 0`) melden `int 0` mit der Adresse des fehlerhaften Befehls;
  `IDIV` mit Quotient −128 bzw. −32768 löst *keinen* Fehler aus.
- String-Befehle: Bei einem Fehler sind die Indexregister des fehlgeschlagenen Zugriffs bereits
  weitergezählt, unter `REP` ist `CX` schon verringert (`REP INSW` sogar um 2).
- `SHR`: Überlaufflag = höchstes Bit vor dem letzten Einzelschritt.
- `SALC` (D6) ist vorhanden; ESC-Befehle (D8–DF) ohne Koprozessor berechnen nur ihre Adresse.
- Im Realmodus lesen sich die Flag-Bits 12–15 als 0.

## Zeitmodell (deterministisch)

- Emulierte Zeit in PIT-Takten (1 193 182 Hz) = ausgeführte Befehle × 1 193 182 / `InstructionsPerSecond`
  (Vorgabe 3 000 000) plus übersprungene Leerlaufzeit (`HLT` bzw. wartende BIOS-Eingabe).
- Timer-IRQ0 nach dem programmierten PIT-Wert (das Spiel stellt 4× 18,2 Hz ein), Bildwechsel/
  Retrace-Signal (Port 3DAh) mit 70 Hz aus derselben Zeit.
- DOS-Datum/Uhrzeit = `StartTime` (Vorgabe 01.06.1991 12:00:00) + emulierte Zeit.
  Jede Abfrage steht im Protokoll (`Clock`).
- Tastatur: geplante Scancodes (`ScheduleKey`, `ScheduleScancode`), ein Byte je IRQ1, mindestens 10 ms Abstand. Das Spiel
  fragt eine Tastenzustandstabelle ab; deshalb folgt der Break-Code erst nach einer Haltezeit.

Gleiche Optionen + gleiche Eingaben ⇒ gleicher Lauf.

## Host-Stubs

Alle Interruptvektoren zeigen anfangs auf kleine Stubs im Segment `F000`
(`F1 00 nn` = Host-Dienst *nn*, danach `RETF 2` bzw. bei IRQs `IRET`). Programme können Vektoren
umbiegen und auf die alten Werte weiterleiten wie bei echter Hardware. `F000:0000` (`F1 01`) ist
die Rücksprungmarke für `CallFar`, mit der einzelne Originalroutinen aufgerufen werden.
Vektoren 60h–67h sind leer, außer 64h bei `RecordSoundDriver` (Vorgabe: an).

## Befunde über das Original

- **Version:** Das Hauptmenü dieser `T.EXE` zeigt „(C) 1992 by Linel Switzerland“ und
  „Code/Grafix/FX: André Wüthrich, R. T. Smith (PC)“. Video 1 zeigt dagegen „(C) 1991“ mit
  anderen Credits (u. a. Intro-/Menümusik Michael Tschögl) und vorher ein Linel-Intro. Das Video
  stammt also von einer anderen Fassung; Abweichungen zwischen Video und RefEmu sind möglich.
- Die Code-Abfrage beim Start nimmt jede Eingabe an und setzt den Zufallsgenerator aus der Uhr.
- AdLib: Das Spiel prüft die Karte selbst an Port 388h (Timer-Test); nur dann ruft es `int 64h` auf.

- `T.EXE` biegt `int 8` (Timer, PIT auf 0x4000 = 72,8 Hz, jeder 4. Takt geht an das BIOS weiter),
  `int 9` (eigene Tastaturbehandlung über Port 60h, Tastentabelle `DS:3D47`) und `int 21h`
  (fängt Get/Set-Vector für `int 8` ab) um (`27C6:0000`, `27C6:037A`, `27C6:0551`).
- Grafik: Modus 13h, danach Chain-4 aus, Doubleword aus, Byte-Modus an (Mode X, `23CD:304D`).
- Borland `time()` (`0000:3405`) rechnet die lokale DOS-Zeit mit `timezone` = 18000 s und
  Sommerzeit (Standard EST5EDT, kein `TZ` gesetzt) in Sekunden seit 1970 um. Dieser Wert ist die
  Uhr-Eingabe von `random(0)`.

## Referenzlauf bis ins Hauptmenü

`OriginalProgram.BootToMainMenu` (Optionen: Vorgaben von `MachineOptions`):

| Zeit (emuliert) | Ereignis |
|---|---|
| 0,00 s | Start, Speicher anpassen, `int 24h/9/8/21h` umgebogen, PIT 4× |
| 0,00 s | AdLib-Erkennung (Ports 388h/389h), `int 64h`-Initialisierung (24 × Funktion 10h) |
| 0,00 s | Modus 13h → Mode X |
| 0,04–0,12 s | `FONTS.PP`, `ENGLISCH.TXT`, `MASKS.PP` |
| 0,87 s | Ladebild „TRADERS – LOADING DATA. PLEASE WAIT.“ |
| 0,89–10,8 s | `V-ANBAU`, `V-RAKETE`, `V-AHANDE`, `V-BHANDE`, `V-FATMIK`, `V-BERGE`, `V-MENU` (je 4× öffnen: Größe, Endbytes, Daten) |
| 12,2 s | Code-Abfrage „INSERT COIN!“ ([21FD:0687](../routines/21FD_0687_codeabfrage.md)), `random(0)` = Uhr-Neusaat |
| 14,0–15,2 s | Eingabe: viermal Taste „1“ |
| 17,3 s | Hauptmenü (Bild-Hash `DBAB2787F9AA5222`) |

Bilder: Referenz-Repo `refemu/refemu-codeabfrage.png`, `refemu-hauptmenue.png` (`tools/claude/fetch-reference.sh refemu`).
Frames und Ereignisprotokoll eines Startlaufs schreibt `REFEMU_OUT=<ordner> dotnet test … --filter FullyQualifiedName~RefEmu.BootTests` lokal.

## Referenzlauf mit FXDRIVER

`OriginalProgram.Start(…, runFxDriver: true)` startet erst `FXDRIVER.EXE` (bleibt resident, setzt `int 64h`), dann `T.EXE`
über dem Treiber (Ladesegment `1776` statt `1000`); jeder `int 64h`-Aufruf wird als Ereignis `Sound` protokolliert. Gemessen
(Test `GameRunTests`, eigene Probe): Das Hauptmenü wird später betreten (15,40 s statt 15,33 s), `random()` hat dort einen anderen
Zustand, und weil das Menü beim Aufbau `random()` aufruft (BESTAND.md, P4), ist das Bild ein anderes:
Hash `A1237183394811CE` (`MainMenuFrameHashWithFxDriver`). Ohne FXDRIVER zeichnet der RefEmu `int 64h` nur auf
(`RecordSoundDriver`). Der Treiber selbst ist nicht kartiert (BESTAND.md §3.3).

## Spiellauf nach dem Hauptmenü (Wochenlauf als Orakel)

`OriginalProgram.StartGame` = Start, Hauptmenü, Eingabetaste nach 0,5 s (Vorgabe: vier Computerspieler). Danach läuft die
Partie ohne weitere Eingaben. Adressen und Folge nach BESTAND.md §1.4/P1: Wochenpreise `1777:02DC` zu Beginn jeder Woche
(Wochenzähler `DS:4B9A` schon erhöht), Phasenmarken `21FD:0977(n)` mit n = 0…6, Zufallszustand `DS:10D2`.

| Zeit (emuliert, ohne FXDRIVER) | Ereignis |
|---|---|
| 18,10 s | neue Partie `0E31:0003`, Zufallszustand `F9D5A4AC` |
| 18,12 s | Woche 0: Wochenpreise |
| 25,66 / 55,86 / 75,70 / 83,77 / 83,78 / 180,38 / 194,72 s | Marken 0–6 der Woche 0 |
| 220,85 s | Woche 1: Wochenpreise (mit FXDRIVER 214,05 s) |

Belegt durch `GameRunTests`: Markenfolge und Wochenwechsel, zwei Läufe bitgleich (Marken mit Zufallszustand,
Befehlszähler, Datensegment zu Beginn von Woche 1), Zeiten wie die unabhängige Probe P1. Laufzeit: eine Spielwoche rund
17 s Rechenzeit (2 Kerne, Release); der Test mit zwei Läufen braucht so etwa 35 s.

- `RunToWeekStart(w, max)` hält vor dem ersten Befehl der Wochenpreise von Woche *w*, `RunToMarker(w, n, max)` vor der
  Markenroutine mit Argument *n* – dort liegt der Zustand zwischen zwei Phasen im Datensegment (`DataSegment()`). Ein erneuter
  Aufruf läuft von der Haltestelle weiter.
- **Maschinenabzug** (R3 Spur `takt` Teil 1, BESTAND.md §3.1 Punkt 3): `OriginalProgram.Snapshot()` sichert an einer Haltestelle
  die ganze Maschine (`Machine/MachineSnapshot.cs`), `OriginalProgram.Resume(dir, abzug)` setzt in einer neuen Maschine mit
  denselben Optionen wieder auf. Teile (`SnapshotParts`): CPU (Register, Flags, Halt, Befehlszähler, Interrupt-Schatten),
  Speicher (1 MB + HMA, A20 – darin auch FXDRIVER und BIOS-Datenbereich), PIC, PIT, Tastatur-Controller, VGA (Ebenen, Register,
  Latches, DAC), AdLib, DOS (offene Dateien, Speicherblöcke, PSP, DTA, Suche), geschriebene Dateien, Uhr der Maschine
  (Leerlauf-Versatz, nächster Timer-IRQ, Eingabeskript, Ende). Nicht enthalten: Ereignisprotokoll, Code-Haken, Spuren
  (Beobachter; nach `Resume` neu anhängen – den FXDRIVER-Haken setzt `Resume` selbst, wenn `int 64h` außerhalb der Stubs zeigt)
  und der Host-Ordner (Eingabe). `WriteTo`/`ReadFrom` schreiben gzip; Abzüge enthalten aus `T.EXE` abgeleiteten Speicher und
  gehören nur in Zwischenordner, nie ins Repo (`AGENTS.md` §2.1).
  Belegt durch `SnapshotTests`: Abzug an Marke 0 von Woche 0, über Datei zurückgelesen, Lauf ab dem Abzug = durchgehender Lauf
  nach 300 000 Befehlen und an Marke 1 (Datensegment, `DS:10D2` = `FBBDAB8D` wie P1, Befehlszähler, Zeit, Bild-Hash, CS:IP und
  alle Teile des Abzugs bitgleich). Gegenprobe: ohne CPU, Speicher, PIT, VGA oder Uhr weicht der Lauf ab; PIC, Tastatur, AdLib,
  DOS und Dateien sind zwischen Marke 0 und 1 vom Neuzustand nicht unterscheidbar – für sie hält eine Feldinventur
  (`Snapshot_covers_every_field_of_the_machine`) den Abzug vollständig: ein neues Feld in einer Maschinenklasse macht sie rot.
  Mit FXDRIVER ebenso bitgleich an Marke 1; gemessen: alle 36 `int 64h`-Aufrufe bis Woche 0 liegen vor Marke 0, zwischen
  Marke 0 und 6 ruft das Spiel den Treiber nicht auf.
- Experiment: `bash experiments/refemu/spiellauf.sh <ordner> [sekunden]` schreibt Bilder, Ereignisprotokoll, Grafiktreiber-
  und OPL-Spur eines Laufs mit FXDRIVER (nur lokal, nicht committen).

## Verwendung

```csharp
var m = new PcMachine(new MachineOptions { HostDirectory = "gamedata/dos" });
m.LoadProgram("T.EXE");
m.RunFor(10.0);                          // 10 s emulierte Zeit
FrameDump.Capture(m.Vga).WritePng("frame.png");
// einzelne Routine: DS = DGROUP, freier Stapel, far-Aufruf
m.Cpu.Ds = (ushort)(m.LoadSegment + 0x28DC);
var (ax, dx) = m.CallFar((ushort)(m.LoadSegment + 0x21FD), 0x01BA, [6]);
```

```csharp
// Zustand nach Marke 3 der Woche 0 (Standardpartie, vier Computerspieler)
var p = OriginalProgram.StartGame("gamedata/dos");
if (p.RunToMarker(week: 0, marker: 3, maxSeconds: 120)) { var ds = p.DataSegment(); uint rnd = p.RandomState; }
// Abzug an der Marke, später ohne Neustart weiter
var abzug = p.Snapshot();
var q = OriginalProgram.Resume("gamedata/dos", abzug);
q.RunToMarker(week: 0, marker: 4, maxSeconds: 60);
```

Segmentangaben der Analyse (Ladebild-Segmente) + `LoadSegment` (Vorgabe `0x1000`) = Segment im Emulator.
