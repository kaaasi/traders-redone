# Architektur und Verträge

- Traders.Core: deterministische Originalsemantik, Zustand, RNG und Savegame-Codec; keine
  Projektabhängigkeiten. Keine UI, Host-Uhr, Dateizugriffe oder eigene Zufallsquelle.
- Traders.Assets: Ressourcenformate und Export; keine Abhängigkeit auf Viewer oder RefEmu.
- Traders.Vga / Traders.Audio: Ausgabeoperationen auf Assets; verändern keinen Spielzustand/RNG.
- Traders.Viewer / Traders.Tools: Host-Ein-/Ausgabe und Darstellung.
- Traders.RefEmu: unabhängige Ausführung des Originals, Testorakel; nie Laufzeitabhängigkeit des Core.
- Traders.Tests: verbindet Portierung und Original für Differenztests.

OriginalProgram.LoadOnly/BootToMainMenu, Call, DataSegment und Frame bilden den Vertrag für
Differenztests. Neue Hooks additiv und mit der Spur `refemu` (Leitstelle) koordinieren; Aufrufkonvention, Segmentbasis,
Zeit-/Eingabemodell sowie zulässige Seiteneffekte dokumentieren. Tests vergleichen Rückgaben,
relevante Speicherbereiche und Seiteneffekte. Erwartungswerte nicht aus der Portierung selbst
gewinnen. Der Emulator ist ebenfalls Software: unabhängige CPU-Testdaten und Originalbefunde behalten.

DGROUP ist das Byteabbild, GameState das modellierte Zustandsobjekt, SAVEGAME.DAT das historische
Dateiformat. Letzteres enthält nicht alle Zustandsfelder (z.B. RNG-Seed). Ein künftiger Replay- oder
Netzwerksnapshot braucht zusätzlich RNG, Zeit-/Eingabefolge und Phasenzustand, versioniert und mit
Wiederanlaufstest. Dies ist ein Zukunftsvertrag, keine bereits implementierte Netzwerkfunktion.

CPU-Interpreter bei fachlichen Änderungen nach Instruktionsgruppen aufteilen, wenn dies Reviews
verbessert; kein rein kosmetischer Großumbau parallel zu laufender RefEmu-Arbeit. Gemeinsame Schnittstellenänderungen
als eigene Vorarbeit integrieren, danach abhängige Pakete darauf aktualisieren.
