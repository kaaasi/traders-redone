# Übergabebericht AP3 – Datenlayout & Spielzustand

- **Branch:** `dev/ap3-state`
- **Stand (Commit):** wird bei jedem Push aktualisiert
- **CI für diesen Commit:** noch nicht gelaufen
- **Bereit zur Übernahme:** nein

## Erledigt seit letzter Übernahme
- `tools/datenlayout/dsrefs.py`: sammelt alle DS-Zugriffe aus `docs/analyse/disasm/*.asm`
  (Offset, Breite, Lesen/Schreiben, Index, Funktion) – 9296 Zugriffe.
- Spielstand-Routinen gefunden: `2336:000A` (Speichern), `2336:0539` (Laden), Puffer 0x900 Byte,
  jedes Feld als 32-Bit-Wert. Liefert die vollständige Liste des persistenten Spielzustands.
- Menüvariablen (Schwierigkeit, Einstellungen, Spieler Mensch/Computer, Steuerung) und
  Neue-Partie-Initialisierung `0E31:0003` ausgewertet.

## Offen / nächste Schritte
- DATENLAYOUT.md schreiben, C#-Zustandsmodell + Tests.

## Vorschläge für STATUS.md
| Bereich (Meilenstein/Modul/Routine) | neuer Status | Bemerkung |
|---|---|---|
| M3 Spielzustand/DS-Layout | 🟡 | in Arbeit |

## Neue Erkenntnisse, die andere Pakete betreffen
- Haupt- und Schwierigkeitsmenü rufen in jeder Warteschleifen-Iteration `random(1)` auf
  (`192D:0345`, `192D:0571`): Leerlauf im Menü verbraucht Zufall (AP5/M4).

## Anfragen an die Leitstelle
- keine
