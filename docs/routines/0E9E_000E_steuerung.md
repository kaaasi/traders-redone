# 0E9E:000E / 0E9E:0035 / 0E9E:0264 / 0E9E:0312 / 0E9E:037A – Steuerungsarten, Richtungs- und Feuerbits

- **Ladebild-Adresse:** Segment `0E9E` (Dateioffset `0x11FE0`, Länge `0x400`; die Schleife von `037A` endet erst bei `0EDE:0002`).
- **Aufgabe:** je Spieler die gewählte Steuerung abfragen und in Bits, Einzelmerker und einen Aktionscode umsetzen. Menschen
  und Computer teilen dieselbe Schnittstelle: die Bits `DS:37C3+i` (für Computer schreiben sie u. a. `12C8`, `150F`).
- **Aufgerufen von / Phase:** `037A` aus allen Spielphasen mit Menüs (`0609`, `08CF`, `0B9F`, `0C29`, `1DEF`, `1F63`, `21FD`,
  `22A3`); `000E` direkt aus den Aktionsszenen (`0535`, `0EDE`, `1B03`, `1C68`), die die Einzelmerker lesen.
- **Parameter / Rückgabe:** keine; Ergebnis im DS.

## Ablauf (eigene Worte)

- **`000E`:** `0035` für alle Spieler, dann für `DS:2C10` = 0…3 je `0264` und `0312` (der Zähler bleibt danach auf 4).
- **`0035`:** für jeden Spieler mit `DS:0840+i` ≠ 1 (kein Computer) Bits aus der Steuerungsart `DS:083C+i` (Sprungleiste `CS:0258`):

  | Art | Gerät | rechts 1 | links 2 | hoch 4 | runter 8 | Feuer 10h |
  |---|---|---|---|---|---|---|
  | 0 / 1 | Joystick 1 / 2 über `27C6:0268` | x > 1 | x < −1 | y < 0 | y > 1 | Knopf 1 |
  | 2 | Maus über `0D73:07E0` (`int 33h`, Bewegungszähler) | Bits 0–4 des Ergebnisses | | | | oder Tastenbyte 7Eh |
  | 3 | Tasten | X (2Dh) | Z (2Ch) | Q (10h) | A (1Eh) | Leertaste (39h) |
  | 4 | Tasten | → (4Dh) | ← (4Bh) | ↑ (48h) | ↓ (50h) | Einfg/Ziffernblock 0 (52h) |
  | 5 | Parallelport-Adapter: `~(1 << i)` an LPT1 (`0040:0008`), Status von LPT1+1 | Bit 4 = 0 | Bit 5 = 0 | Bit 6 = 0 | Bit 7 = 1 | Bit 3 = 0 |

  Bei Art 3 und 4 schließen sich rechts/links und hoch/runter aus (rechts bzw. hoch zuerst). Joystick: nur die betragsgrößere
  Achse zählt, bei Gleichstand die y-Achse. Ohne Maustreiber (`DS:2BE6` = 0) liefert Art 2 nur das Feuer aus Tastenbyte 7Eh.
  Grundeinstellung im
  Ladebild: Spieler 0–3 = 4, 3, 0, 1; das Menü zählt sie mit `192D:1ACE` reihum 0…5.
- **`0264` (Spieler `DS:2C10`):** d = Bits & 0Fh. d = 0 und Mensch → Bewegungsverzögerung `DS:3786+4i` (32 Bit) = 0. Sonst, wenn
  die Verzögerung > 0 (vorzeichenbehaftet): um 1 senken, d = 0. Merker rechts `44BE+i`, links `43F0+i`, hoch `439D+i`, runter `44BA+i`
  = Bits von d. Computer zählen die Verzögerung also auch ohne Bits herunter; gesetzt wird sie in den Phasen (`0535`, `0609`,
  `08CF`, `0EDE`: Werte 3…10 000).
- **`0312`:** f = Feuerbit. f = 0 → Feuerverzögerung `DS:302E+4i` = 0; sonst bei Verzögerung > 0 senken und f = 0. Merker `4894+i` = f.
- **`037A`:** `000E`, dann je Spieler Aktion `DS:3308+i` = −1 (Computer bleiben dabei). Vorrang Feuer (10) > hoch (2) > runter (3)
  > rechts (0) > links (1). Bei Feuer zusätzlich Feuerverzögerung = 100 000: gehaltenes Feuer wird erst nach dem Loslassen
  wieder gemeldet (Ablauf erst nach 100 000 Aufrufen).

## Zustand

| DS-Offset | Breite | Bedeutung | gelesen/geschrieben |
|---|---|---|---|
| `083C` | B[4] | Steuerungsart 0…5 je Spieler | L; G Menü `192D:1ACE` |
| `0840` | B[4] | 1 = Computer | L |
| `37C3` | B[4] | Bits (rechts 1, links 2, hoch 4, runter 8, Feuer 10h) | G `0035` (nur Menschen), L `0264`, `0312` |
| `3786` / `302E` | DW[4] | Bewegungs- bzw. Feuerverzögerung (vorzeichenbehaftet) | L/G |
| `44BE`, `43F0`, `439D`, `44BA`, `4894` | B[4] | Merker rechts, links, hoch, runter, Feuer | G `0264`, `0312` |
| `3308` | B[4] | Aktion: −1 keine, 0 rechts, 1 links, 2 hoch, 3 runter, 10 Feuer | G `037A` |
| `2C10` | W | Spielerzähler der Schleife in `000E` (bleibt 4) | G |
| `2BE6` | W | Maustreiber vorhanden | L (`0D73:07E0`) |

## Seiteneffekte in Reihenfolge

Eingabe: Tastentabelle `3D47` (Befund [27C6_0043_eingabe_zeitgeber.md](27C6_0043_eingabe_zeitgeber.md)), Port 201h, `int 33h`,
LPT-Ports. Kein Zufall, keine Uhr, keine Grafik.

## C#-Umsetzung

Noch keine. Die Abfrage gehört als Befehlsquelle in den Kern (Phasenmaschine, R4); der Host liefert nur den Tastenzustand.

## Prüfung

RefEmu-Proben `tests/Traders.Tests/App/OriginalControlProbeTests.cs` auf dem Ladebild (`LoadOnly`, direkte Aufrufe): Arten 3 und
4 mit Einzel- und Doppeltasten je Spieler, fremde Tasten ohne Wirkung, Computer unberührt; Art 2 ohne Treiber, Tastenbyte 7Eh;
Art 5 ohne Adapter (nur „runter“); Vorrang der Aktion; Feuerverzögerung 100 000 und Loslassen; Bewegungsverzögerung zählt,
wird bei Menschen ohne Richtung gelöscht, negativ nicht gezählt; Computer-Bits werden übernommen und Verzögerungen gezählt.
Nicht geprüft: Joystick (RefEmu ohne Gerät an 201h, ohne Kalibrierung hängt `0268`), Maus mit Treiber (RefEmu ohne `int 33h`).
