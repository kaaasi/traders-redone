# Traders.RefEmu (Arbeitspaket AP1)

Deterministischer Referenz-Emulator: 80286-Realmodus-Interpreter, PC-Hardware (PIC, PIT, Tastatur,
VGA, stilles AdLib) und DOS/BIOS-Dienste auf Host-Seite. Führt die originale `T.EXE` bis ins
Hauptmenü aus und ruft einzelne Originalroutinen für Differenztests auf.

Einstieg für Differenztests: `OriginalProgram` (`LoadOnly`, `BootToMainMenu`, `Call`, DS-Zugriffe).
Ausführliche Beschreibung: [docs/analyse/REFEMU.md](../../docs/analyse/REFEMU.md).
