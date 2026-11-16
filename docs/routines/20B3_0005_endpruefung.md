# 20B3:0005 – Endprüfung (bis zur Entscheidung, R7 Spur `ende`)

- **Ladebild-Adresse:** `20B3:0005` (Dateioffset `0x24135`, Segment `20B3` bei `0x24130`)
- **Aufgabe:** Aufruf 19 der Wochenschleife `1777:01F8` (`1777:027C`): prüft nach der Roboterphase, ob die Partie zu Ende ist –
  ein Spieler hat das Zielvermögen erreicht oder die Taste mit Scancode 44h (F10) ist gedrückt. Bei Ende Endbilder, Abschluss der
  Woche und AL = 1; sonst AL = 0.
- **Aufgerufen von / Phase:** nur `1777:027C` (Aufruf 19, nach der Roboterphase `1C68:000B`); die Schleife prüft danach AL
  (Abbruchweg B, AL = 1 → Rückkehr mit AL = 1).
- **Parameter / Rückgabe:** keine; AL = 0 (weiter) oder 1 (Partie zu Ende).

## Ablauf (eigene Worte)

1. `push bp`/`mov bp,sp`, dann `0825:06CB` (Rangfolge: Vermögen aller Spieler nach `DS:2F06`, Ränge `DS:3D42`/`302A`,
   `DS:37AA` = 4 danach; Befund `0825_0813_vermoegen.md`).
2. `DS:37AA` = 0; Schleife, solange `DS:37AA` < 4 (vorzeichenbehaftet, `jl`): Doppelwort `DS:2F06 + 4·37AA` (DX = Low, AX =
   High) gegen das Ziel `DS:08F4` (Low)/`08F6` (High): erst High-Wort `cmp ax,[08F6]`, kleiner (`jc`) → nächster Spieler,
   ungleich (`jnz`) → Abbruch; bei gleichem High-Wort Low-Wort `cmp dx,[08F4]`, kleiner (`jc`) → nächster Spieler, sonst Abbruch.
   Also **32 Bit vorzeichenlos**, Vermögen ≥ Ziel bricht ab; `DS:37AA` bleibt beim ersten solchen Spieler stehen.
3. `20B3:0041`: `DS:37AA` = 4 **und** Tastenbyte `DS:3D8B` (= Tastentabelle `DS:3D47` + 44h) = 0 → AL = 0, Rückkehr (`004F`).
4. Sonst ab `20B3:0053` der **Rest (nicht portiert, F1 = B):** `20B3:008E` (Endbild), `218B:0094(2, 3)` (Ton),
   `20B3:0525`, `20B3:0863`, `inc word [DS:4B9A]` (`0071`), `0825:07C4`, `0825:000A(0)`, `27C6:00D2` (Warten auf Taste),
   AL = 1.

Ein negatives Vermögen (Bit 31 gesetzt) ist vorzeichenlos riesig und beendet die Partie.

**Offene Routinen** (`tools/callgraph.py` auf `T.EXE`, 16.11.2026): `20B3:0005` ruft `0825:06CB` (portiert, `Ranking`),
`20B3:008E`, `218B:0094`, `20B3:0525`, `20B3:0863`, `0825:07C4` (portiert), `0825:000A` (portiert, Bilanz), `27C6:00D2`. Bis zur
Entscheidung wird nur `06CB` gebraucht; alles ab `0053` gehört zu Ende/Abend (Fahrplan R9).

## Zustand

| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `2F06` | D[4] | u (Vergleich) | Vermögen je Spieler (`CurrentWealth`), frisch aus `06CB` | geschrieben (06CB), gelesen |
| `08F4` | D | u | Zielvermögen (`Target`) | gelesen |
| `37AA` | B | s | Schleifenvariable / erster Spieler ≥ Ziel, sonst 4 (`ActivePlayer`) | geschrieben |
| `3D8B` | B | u | Taste 44h gedrückt (Tastentabelle `KeyState` + 44h) | gelesen |
| `3D42`, `302A`, `45D0` | B[4], B[4], W | – | Ränge und Warenschleife aus `06CB`/`0813` | geschrieben (06CB) |

## Seiteneffekte in Reihenfolge

Bis zur Entscheidung: kein Zufall, keine Uhr, keine Grafik, kein Ton; Eingabe nur als Lesen der Tastentabelle (`DS:3D8B`).
Der Rest (Endbilder, Ton, Wochenzähler, Bilanz, Tastenwarten) ist ausstehend.

**Vorbedingung R7P-2:** Die Entscheidung hängt an `DS:2F06`, das erst `06CB` in `0005` schreibt. `IsPending` rechnet deshalb
auf einer Kopie des Datensegments (`EndCheck.Ends`); bei Ende bleibt der Zustand unverändert, und ein Rest (z. B. das Orakel)
ruft `0005` vollständig. **`06CB` ist bei gleichem Zustand idempotent:** es schreibt nur `2F06`, die Ränge, `37AA` und `45D0`
und liest davon nur `37AA` (Spielerindex, den es selbst je Spieler setzt) und `45D0` (in `0813` vor dem Lesen gesetzt); belegt
durch `The_ranking_is_idempotent_in_the_original` (zweimal im RefEmu, Datensegment gleich).

## C#-Umsetzung

`src/Traders.Core/Ending/EndCheck.cs`: `EndCheck.Decide` (0005–0051 auf dem Datensegment, Rückgabe „Ende“), `EndCheck.Ends`
(auf einer Kopie). `EndCheckStep` (Aufruf 19, `IPartialWeekStepHandler`): ohne Ende `Decide` auf dem Zustand und AL = 0 im selben
Bild; bei Ende ausstehend (Maschine hält an 19, `ProgramHost.Halt` meldet „game over“) oder Übergabe des ganzen Aufrufs an `rest`
(Test: RefEmu-Orakel), `GameOver` zeigt die Entscheidung. `WeekSteps` bindet den Schritt immer (`WeekSteps.End`).

## Prüfung

- `tests/Traders.Tests/Ending/EndCheckDifferentialTests.cs`: `LoadOnly`/`Call(20B3:0005)`; Bargeld je Spieler so gesetzt, dass
  das Original-`06CB` das gewünschte Vermögen ergibt (Restvermögen vorher aus dem Original gelesen); Rest ab `0053` durch den
  Epilog ersetzt, Erreichen aufgezeichnet. 18 Fälle: Ziel 10000/30000/50000, `0001_0000` (Ziel − 1 = `0000_FFFF`), `0001_FFFF`
  (Ziel + 1 = `0002_0000`), gleiche High-Wörter, negatives Vermögen, Gleichstand, Ziel 0 und `FFFF_FFFF`, Taste 44h mit und ohne
  Ende. Verglichen: Entscheidung, ganzes Datensegment, `DS:10D2` unverändert, AL = 0 und Lauf in der Phasenmaschine.
- `EndCheckStepTests.cs` (ohne Daten): Entscheidung und `37AA`, Halt an 19 ohne Änderung, Rest vom unveränderten Zustand.
- `PhaseMachineDifferentialTests`: Woche 0 mit Port für Aufruf 19 (AL verglichen), Abbruchweg B mit Port-Entscheidung „Ende“ und
  Rest aus dem RefEmu. Mehrrundenproben `MultiWeekScreenDifferentialTests` (Wochen 0–3) und `MultiWeekRound6DifferentialTests`
  (Wochen 0–4): Aufruf 19 im Port, AL und Datensegment am Rücksprung verglichen.
