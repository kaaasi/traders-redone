# Übergabebericht AP3 – Datenlayout & Spielzustand

- **Branch:** `dev/ap3-state`
- **Stand (Commit):** siehe Branch-Spitze (wird mit jedem Push aktualisiert)
- **CI für diesen Commit:** grün für `fb64063` (393 Tests, Linux + Windows)
- **Bereit zur Übernahme:** ja (Stand 2: Differenztest Speichern/Laden)

## Erledigt seit letzter Übernahme
- **Differenztest Speichern/Laden gegen das Original** (`tests/Traders.Tests/State/SaveGameDifferentialTests.cs`):
  `2336:000A` und `2336:0539` laufen im RefEmu; Datei und Datensegment stimmen mit dem Port
  überein (auch Fehlerfall zu kurze Datei, Spielfeldursprung `192D:0058`). Damit sind beide
  Routinen ✅.

## Bereits übernommen (Stand 1)
- **Datenlayout** `docs/analyse/DATENLAYOUT.md`: alle 52 Felder des Spielstands (= persistenter
  Spielzustand) mit Offset, Breite, Vorzeichen, Wiederholung, Startwert, Bedeutung und Beleg;
  Schwierigkeitsstufen-Tabelle; wichtige Laufzeitvariablen (RNG-Seed, aktiver Spieler, globale
  Schleifenvariablen, Tastatur-/Texttabelle, Preise, Rang, Spielfeldursprung).
- **Spielstand-Format** aus `2336:000A`/`2336:0539`: 522 Dwords in 0x900 Byte, Vorzeichen-/
  Nullerweiterung je Feld, Abschneiden beim Laden, Nebenwirkung auf globale Schleifenvariablen.
- **C#** `src/Traders.Core/State/`: `DgroupLayout` (Offsets), `GameState` (+Player/Field/Station/
  Settings, JSON-serialisierbar, Originaltypen), `Dgroup` (DS-Abbild, `FromExe`),
  `GameStateMapper` (Zustand ↔ DS-Abbild, für RefEmu-Vergleiche), `SaveGame` (SAVEGAME.DAT
  bytegenau lesen/schreiben).
- **Tests** `tests/Traders.Tests/State/`: Feldreihenfolge gegen unabhängig aus der Disassemblierung
  extrahierte Liste (`tools/datenlayout/savegame.py`), Abdeckung aller Spielstand-Bytes,
  Rundreisen DS/Spielstand/JSON, Vorzeichenerweiterung, Startzustand = initialisierte Daten
  aus T.EXE, Level-Tabelle und Stationsnamen aus T.EXE.
- Routinenbefund `docs/routines/2336_000A_spielstand.md`.
- Werkzeuge: `tools/datenlayout/dsrefs.py` (alle DS-Zugriffe), `tools/datenlayout/savegame.py`.

## Offen / nächste Schritte
- Bedeutung offener Felder (`40F6`, `4BBE`, `4580`, Roboterfelder, Stationswert) – ergibt sich
  mit M9/M12.
- Neue-Partie-Routine `0E31:0003` portieren (Zufallsreihenfolge) – gehört zu M4.

## Vorschläge für STATUS.md
| Bereich (Meilenstein/Modul/Routine) | neuer Status | Bemerkung |
|---|---|---|
| M3 Kern-Fundament | 🟡 | Spielzustand/DS-Layout fertig (AP3); Phasenmodell offen (AP5) |
| Routine `2336:000A` Spielstand speichern | ✅ | Differenztest im RefEmu grün |
| Routine `2336:0539` Spielstand laden | ✅ | Differenztest im RefEmu grün (inkl. `192D:0058`) |
| Modul `2336` | 🔍 | Spielstand (000A/0539/051E/0947) |
| Modul `0E31` | 🔍 | Neue Partie, Landerzeugung, Stationen (nicht `?` mehr) |
| Modul `1777` | 🔍 | Wochenpreise (Öl, Raketen, Lotto, Land), nutzt Uhr |
| Modul `0B9F` | 🔍 | Handel (setzt Warenpreise `4898`) – wahrscheinlich |
| Modul `1C68` / `0535` / `0EDE` | 🔍 | Roboterkampf (Robo-Store/Kampf/Beute) – wahrscheinlich |
| Modul `192D` | 🔍 | Menüs (Haupt, Schwierigkeit, Einstellungen, Disk, Steuerung) |

## Neue Erkenntnisse, die andere Pakete betreffen
- `DS:117E`/`DS:1180` sind Tickzähler des int-8-Handlers (`27C6:03C3`); sie ändern sich bei
  jedem DS-Vergleich über emulierte Zeit und gehören in die Ausnahmeliste von Differenztests.
- Haupt- und Schwierigkeitsmenü rufen in jeder Warteschleifen-Iteration `random(1)` auf
  (`192D:0345`, `192D:0571`): Leerlauf im Menü verbraucht Zufall (AP5/M4).
- Der Zufallsseed `DS:10D2` wird **nicht** gespeichert: eine geladene Partie läuft mit dem
  aktuellen Seed weiter (RefEmu-Vergleiche über Laden müssen den Seed mitsetzen).
- Texte werden über eine Far-Pointer-Tabelle angesprochen: Zeile *k* = `DS:37D0 + 4k`. Damit
  lassen sich Funktionen per Text identifizieren (AP5: Phasenzuordnung).
- Tastenzustand je Scancode ab `DS:3D47` (F1 = `3D82`, Enter = `3D63`) – für Eingabe-Stubs (AP1).
- Der Computer-IQ ist im Original **ein** globaler Wert (`DS:2E8E`); IQ je Gegner (E2) braucht
  eine Erweiterung.

## Anfragen an die Leitstelle
- keine
