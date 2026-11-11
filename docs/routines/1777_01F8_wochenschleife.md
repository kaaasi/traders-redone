# 1777:01F8 – Wochenschleife (`play_game`)

- **Ladebild-Adresse:** `1777:01F8` (Dateioffset `0x1AF68`, Kopf `0x3600`), Länge `0xE4` bis `retf` bei `1777:02DB`
- **Aufgabe:** spielt die Partie Woche für Woche: 30 Far-Aufrufe je Woche in fester Folge, dazwischen sieben leere Marken
  `21FD:0977(n)`, n = 0…6; am Ende der Woche `DS:4B9A`++ und Sprung an den Anfang. Keine eigene Regel, kein eigener Zufall,
  kein eigenes Zeichnen.
- **Aufgerufen von / Phase:** nur `main` `1777:000C` bei `1777:017F` (Befund [1777_000C_main.md](1777_000C_main.md)), nach
  `DS:08FA` = 1. Einzige Schleife über Wochen im Programm.
- **Parameter / Rückgabe:** keine Parameter, kein lokaler Rahmen außer `bp`. Rückgabe in `AL`: 0 = Abbruch ins Hauptmenü,
  1 = Partie zu Ende (Endprüfung). Eine dritte Rückkehr gibt es nicht; die Schleife hat keinen Wochenhöchstwert.

## Aufruffolge einer Woche (S: lineares Lesen ab `01FB`; P: Probelauf unten)

| # | Stelle | Aufruf | Rolle (nach `docs/analyse/BESTAND.md` §1.2/§1.4) |
|---:|---|---|---|
| 1 | `01FB` | `1777:02DC` | Wochenpreise; **Schleifenanfang** (Ziel des Rücksprungs bei `02D7`) |
| 2 | `0200` | `0825:07C4` | Vermögen in den Verlauf schreiben |
| 3 | `0208` | `0825:000A(1)` | Wochenbilanz (Bildschirm) |
| – | `020E` | Abbruchweg A | `DS:3D55` ≠ 0 → `AL` = 0, Rückkehr (`0215`→`02DA`) |
| 4 | `021A` | `111D:0009` | Katastrophen |
| 5 | `021F` | `0609:1178` | Obergrenzen aus der Zahl der Produktionsfelder |
| 6 | `0227` | `11A3:0000(0)` | Computerspieler, Durchgang 0 (vor dem Anbau) |
| 7 | `0230` | `21FD:0977(0)` | **Marke 0** |
| 8 | `0236` | `0609:0004` | Anbau |
| 9 | `023B` | `17CB:0005` | Wochenereignisse |
| 10 | `0244` | `21FD:0977(1)` | **Marke 1** |
| 11 | `024A` | `1B03:0009` | Stationen, Rakete, Piraten |
| 12 | `024F` | `21FD:021F` | Wetter würfeln |
| 13 | `0254` | `22A3:0003` | Produktion |
| 14 | `0259` | `22A3:026B` | Vorräte auf 8/10 |
| 15 | `0262` | `21FD:0977(2)` | **Marke 2** |
| 16 | `0268` | `22A3:02D3` | Ernte zeigen und einsammeln |
| 17 | `0271` | `21FD:0977(3)` | **Marke 3** |
| 18 | `0277` | `1C68:000B` | Roboterphase |
| 19 | `027C` | `20B3:0005` | Endprüfung |
| – | `0281` | Abbruchweg B | Rückgabe `AL` = 1 aus `20B3:0005` → `AL` = 1, Rückkehr (`0285`→`02DA`) |
| 20 | `0289` | `0609:1178` | wie 5 |
| 21 | `0291` | `11A3:0000(1)` | Computerspieler, Durchgang 1 (vor dem Handel) |
| 22 | `029B` | `21FD:0977(4)` | **Marke 4** |
| 23 | `02A1` | `0B9F:0002` | Handel |
| 24 | `02A6` | `1DEF:000C` | Zusatzphase |
| 25 | `02AF` | `21FD:0977(5)` | **Marke 5** |
| 26 | `02B5` | `1F63:000F` | Fat Mike |
| 27 | `02BA` | `08CF:11D7` | Edelsteinkurs der Woche |
| 28 | `02C3` | `21FD:0977(6)` | **Marke 6** |
| 29 | `02C9` | `08CF:0004` | Einkauf |
| 30 | `02CE` | `1F63:0B03` | Wettbewerb/Auswertung nach dem Einkauf |
| – | `02D3` | `inc word [DS:4B9A]` | Wochenzähler +1, danach `jmp 01FB` |

Argumente werden als Byte (`mov al,n` / `push ax`, Aufrufe 3, 6, 21) bzw. Wort (`mov ax,n` / `push ax`, Marken) übergeben und
mit `pop cx` abgeräumt. Dieselbe Folge erzeugt `tools/callgraph.py` als `phases.dot` (lokale Ausgabe).

## Zustand
| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `4B9A` | Wort | s | Wochenzähler (neue Partie setzt 0, `DATENLAYOUT.md` Nr. 38) | geschrieben (`inc` bei `02D3`) |
| `3D55` | Byte | – | Tastenzustand für Scancode 0Eh (Tabelle ab `DS:3D47`, ein Byte je Scancode) | gelesen (`020E`) |

Weitere Schreiber von `DS:4B9A` laut Aufrufgraph: `0E31:0003` (neue Partie), `20B3:0005`, `2336:0539` (Spielstand laden).

## Seiteneffekte in Reihenfolge
Die Schleife selbst hat keine: kein Zufall, keine Uhr, keine Grafik, keine Eingabe außer dem Lesen von `DS:3D55` (gesetzt vom
Tastaturhandler), kein Ton. Alle Wirkungen liegen in den 30 Aufrufen. Die Marke `21FD:0977` ist eine leere Funktion (P1 in
`BESTAND.md`), also ein seiteneffektfreier Haltepunkt mit dem Phasenindex als einzigem Stapelargument.

**Abbruchwege:**

- **A – Abbruch ins Menü:** einzige Abfrage nach der Wochenbilanz (Aufruf 3). Ist die Taste mit Scancode 0Eh zu diesem
  Zeitpunkt als gedrückt vermerkt, endet die Schleife vor den Katastrophen mit `AL` = 0; `main` kehrt ins Hauptmenü zurück. Der
  Wochenzähler bleibt unverändert, die Woche ist bis einschließlich Bilanz gelaufen.
- **B – Partieende:** `20B3:0005` liefert `AL` = 1 (Endprüfung; Bedingungen in `BESTAND.md` §1.2) → Rückgabe 1 vor dem
  zweiten Planungsdurchgang; Marken 4–6 und der Wochenzähler-Schritt dieser Woche fehlen. `main` beendet daraufhin das
  Programm (siehe Befund `main`).

## C#-Umsetzung
Kein Port in Runde 2. Ziel ist die Phasenmaschine des Kerns in R3 (`AGENTS.md` §4); die Marken 0–6 sind dort die
Vergleichspunkte für Mehrrundenproben.

## Prüfung
- **Statisch (S):** Disassemblierung `1777:01F8`–`02DB` (`tools/segmap.py` bzw. `tools/callgraph.py`), alle Ziele sind
  verlagerte Far-Aufrufe.
- **Probelauf (P):** `bash experiments/phasen/phasentrace.sh <aus> 400` auf `main` `657805b`, Lauf A (Vorgaben, vier
  Computerspieler, nur Eingabetaste): Woche 0 und Woche 1 jeweils `1777:02DC` → Marken 0, 1, 2, 3, 4, 5, 6 in dieser Folge,
  danach `1777:02DC` mit `DS:4B9A` um 1 erhöht; Woche 2 bis Marke 4 im Zeitfenster. Emulierte Zeiten der Marken in Woche 0:
  25,3 / 56,2 / 69,0 / 77,4 / 77,4 / 147,3 / 161,6 s (Lauf bitgleich wiederholbar, `BESTAND.md` P1).
- **RefEmu-Test (T):** `tests/Traders.Tests/Phases/WeekLoopTests.cs` (Spur `phasen` Teil 3) prüft gegen die Tabelle oben
  (`WeekLoopSequence.cs`, statisch gelesen, nicht aus dem Lauf): Vorgabepartie bis zum Beginn von Woche 2 – je Woche die 30
  Aufrufstellen mit ihren Zielen, Marken 0–6 an ihren Stellen, `DS:4B9A` 0 → 1 → 2 genau bei `02D3`. Abbruchweg A:
  Rücktaste (0Eh) während der Bilanz von Woche 0 gehalten → nur Aufrufe 1–3, Rückkehr nach `main` (`1777:0184`) mit
  `AL` = 0 und `DS:4B9A` = 0, danach erneut Hauptmenü `192D:0007`. Gegenproben: Marke in der Tabelle verändert bzw. Taste
  weggelassen → beide Tests rot; vertauschte, fehlende und zusätzliche Marken, vertauschte Aufrufe und falscher Zähler ohne
  Originaldaten in `WeekLoopSequenceTests.cs`.
- **Nicht ausgeführt:** Abbruchweg B (Partieende) im RefEmu.
