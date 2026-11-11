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
- Tastatur: geplante Scancodes (`ScheduleKey`), ein Byte je IRQ1, mindestens 10 ms Abstand.

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

Bilder: `tests/Traders.Tests/RefEmu/Data/refemu-codeabfrage.png`, `refemu-hauptmenue.png`.
Frames und Ereignisprotokoll eines Startlaufs schreibt `REFEMU_OUT=<ordner> dotnet test … --filter FullyQualifiedName~RefEmu.BootTests` lokal.

## Verwendung

```csharp
var m = new PcMachine(new MachineOptions { HostDirectory = "original/dos" });
m.LoadProgram("T.EXE");
m.RunFor(10.0);                          // 10 s emulierte Zeit
FrameDump.Capture(m.Vga).WritePng("frame.png");
// einzelne Routine: DS = DGROUP, freier Stapel, far-Aufruf
m.Cpu.Ds = (ushort)(m.LoadSegment + 0x28DC);
var (ax, dx) = m.CallFar((ushort)(m.LoadSegment + 0x21FD), 0x01BA, [6]);
```

Segmentangaben der Analyse (Ladebild-Segmente) + `LoadSegment` (Vorgabe `0x1000`) = Segment im Emulator.
