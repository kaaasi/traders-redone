# Übergabebericht AP3 – Datenlayout & Spielzustand

- **Branch:** `dev/ap3-state`
- **Stand (Commit):** siehe Branch-Spitze (wird mit jedem Push aktualisiert)
- **CI für diesen Commit:** siehe unten
- **Bereit zur Übernahme:** nein

## Erledigt seit letzter Übernahme
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
- Werkzeuge: `tools/datenlayout/dsrefs.py` (alle DS-Zugriffe), `tools/datenlayout/savegame.py`.

## Offen / nächste Schritte
- Bedeutung offener Felder (`40F6`, `4BBE`, `4580`, Roboterfelder, Stationswert) – ergibt sich
  mit M9/M12.
- Differenztest Speichern/Laden im RefEmu, sobald AP1 Routinen ausführen kann
  (`2336:000A` mit präpariertem DS-Abbild → Puffer bei `DS:45D6` vergleichen).
- Neue-Partie-Routine `0E31:0003` portieren (Zufallsreihenfolge) – gehört zu M4.

## Vorschläge für STATUS.md
| Bereich (Meilenstein/Modul/Routine) | neuer Status | Bemerkung |
|---|---|---|
| M3 Kern-Fundament | 🟡 | Spielzustand/DS-Layout fertig (AP3); Phasenmodell offen (AP5) |
| Routine `2336:000A` Spielstand speichern | 🟢 | `Core/State/SaveGame.cs`, RefEmu-Vergleich offen |
| Routine `2336:0539` Spielstand laden | 🟢 | `Core/State/SaveGame.cs`, RefEmu-Vergleich offen |
| Modul `2336` | 🔍 | Spielstand (000A/0539/051E/0947) |
| Modul `0E31` | 🔍 | Neue Partie, Landerzeugung, Stationen (nicht `?` mehr) |
| Modul `1777` | 🔍 | Wochenpreise (Öl, Raketen, Lotto, Land), nutzt Uhr |
| Modul `0B9F` | 🔍 | Handel (setzt Warenpreise `4898`) – wahrscheinlich |
| Modul `1C68` / `0535` / `0EDE` | 🔍 | Roboterkampf (Robo-Store/Kampf/Beute) – wahrscheinlich |
| Modul `192D` | 🔍 | Menüs (Haupt, Schwierigkeit, Einstellungen, Disk, Steuerung) |

## Neue Erkenntnisse, die andere Pakete betreffen
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
