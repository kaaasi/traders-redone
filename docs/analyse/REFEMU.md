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

- `T.EXE` biegt `int 8` (Timer, PIT auf 0x4000 = 72,8 Hz, jeder 4. Takt geht an das BIOS weiter),
  `int 9` (eigene Tastaturbehandlung über Port 60h, Tastentabelle `DS:3D47`) und `int 21h`
  (fängt Get/Set-Vector für `int 8` ab) um (`27C6:0000`, `27C6:037A`, `27C6:0551`).
- Grafik: Modus 13h, danach Chain-4 aus, Doubleword aus, Byte-Modus an (Mode X, `23CD:304D`).
- Borland `time()` (`0000:3405`) rechnet die lokale DOS-Zeit mit `timezone` = 18000 s und
  Sommerzeit (Standard EST5EDT, kein `TZ` gesetzt) in Sekunden seit 1970 um. Dieser Wert ist die
  Uhr-Eingabe von `random(0)`.

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
