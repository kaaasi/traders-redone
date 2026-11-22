# Arbeitspakete

Die Portierung läuft in **Runden**; je Runde arbeiten mehrere **Spuren** parallel, jede auf einem Branch `paket/<spur>` mit
einem PR, eigenen Dateien und klarer Abnahme. Regeln: [AGENTS.md](../AGENTS.md), Ablauf:
[prozess/zusammenarbeit.md](prozess/zusammenarbeit.md). Den Status pflegt jede Spur nur in ihrem eigenen Abschnitt; Plan-
abschnitte ändert die Leitstelle. Abgeschlossene Runden stehen in [archiv/](archiv/) (Paketphase AP1–AP5: [archiv/runde-0.md](archiv/runde-0.md), Runde 1: [archiv/runde-1.md](archiv/runde-1.md), Runde 2: [archiv/runde-2.md](archiv/runde-2.md), Runde 3: [archiv/runde-3.md](archiv/runde-3.md), Runde 4: [archiv/runde-4.md](archiv/runde-4.md), Runde 5: [archiv/runde-5.md](archiv/runde-5.md), Runde 6: [archiv/runde-6.md](archiv/runde-6.md), Runde 7: [archiv/runde-7.md](archiv/runde-7.md), Runde 8: [archiv/runde-8.md](archiv/runde-8.md)).

## Aktuelle Runde: 9 – Ende und Abend, Ton, Gesamtpartie ohne Roboterkampf (Plan Leitstelle 22.11.2026, Go Nutzer 22.11.2026)

**Ziel:** Zwischenziel **„Gesamtpartie ohne Roboterkampf“** (Nutzer 11.11.2026): Der Host spielt mit Roboter-Schalter
(R7 F3 = A) vom Hauptmenü über eine neue Partie Woche für Woche bis zum Ende (Zielvermögen oder F10), zeigt die Endbilder
(Rest von `20B3:0005`, R7 F1 = B) und kehrt ins Hauptmenü zurück – **mit Ton** (Musik und Effekte über AdLib/OPL2). Dazu die
offenen P3 #49, #75, #76, #220, #221, #236. Ergebnis: M13 (Ende/Rang) und M14 (Ton) abgeschlossen, Meilenstein
„Gesamtpartie ohne Roboterkampf“ in `STATUS.md`.

**Leitstelle R9:** Claude (Sitzung vom 22.11.2026). **Modell** (`prozess/zusammenarbeit.md` §5): Spuren 0–4 Claude Opus
(Disassembly, Spielkern, Zufall, RefEmu, Differenztests, Klangsynthese), Spur 5 Claude Sonnet (Testergänzungen, Doku, kleine
Werkzeugfixes); alle Erstreviews und das Plan-Review Opus. **Base:** `main` `64f3481`. Namen der Läufe: `Traders R9 <spur>`.
**Starts zeitversetzt** (Nutzungslimit): 5 und 1 sofort nach dem Go, 2 eine Stunde später, 3 zwei Stunden später, 4 nach Merge
von 2 oder 3 (braucht nur `OplTrace`, darf auch früher starten, falls Kontingent frei), 0 nach Merge von 1–4.

**Gemeinsame Vorgaben** (wie R8):

- Branch `paket/<spur>` von aktuellem `main`, ein Draft-PR, Teile als Aufgabenliste (`spur-start.md` „Teile und Fortsetzung“);
  Teilstände, die vorab gemergt werden sollen, auf eigenem Branch, Fortsetzung auf neuem Branch (Erfahrung R7, #185).
- Je portierter Originalroutine Befund in `docs/routines/` (Sammelbefund erlaubt; Sammelmarker nur in Listenform
  `[ORIGINAL] XXXX:YYYY, XXXX:YYYY`), `[DIFFERENTIAL]`-Test gegen den RefEmu mit Grenzfällen und Zufallszustand `DS:10D2` danach;
  Erwartungswerte nie aus dem Port. Teil 1 jeder Code-Spur prüft die Liste der offenen Routinen unten mit `tools/callgraph.py`
  nach (indizierte Schreibziele aus dem Disassembly ergänzen, R8PL-3); was einer anderen Spur gehört oder schon portiert ist, wird
  gerufen, nicht nachgebaut. Endet das Original nur unter einer Bedingung, Machbarkeitsprüfung oder Abbruchzähler im Testaufbau
  (`routines/_vorlage.md`, Zeitlimit 20 min je Test).
- **Kern und Szene getrennt** (`AGENTS.md` §4): Ablauf, Buchungen, Zufall im Kern als Kern-Ereignis je Bild; Szenen in
  `src/Traders.Presentation/` lesen nur (Muster R5–R8). Handler **takten je Bild des Originals** (R6S-1), Abnahme „Bildzähler“.
- **Ton im Kern nur als Befehl:** Der Kern bucht die DGROUP-Merker der Tonroutinen (`DS:10A3`, `10A5`, `10A7`, `10A9`, `0910`)
  und gibt **Tonbefehle** aus (Effekt `b`, Stück `i` mit Merker, Stopp, Rückruf wählen); keine Ausgabe, keine Uhr. Ausgabe und
  Synthese liegen in `src/Traders.Audio/`, Takt aus `Traders.App/Timing` (Effekte je Tick 72,82 Hz, Musik je vierten Tick,
  `analyse/TON.md` §2). **Schnittstelle zwischen den Tonspuren:** Folge von OPL2-Registerschreibungen mit Tick
  (`(Tick, Register, Wert)`), vergleichbar mit `Traders.RefEmu/Machine/OplTrace.cs`. **Zeitbasis** (R9PL-2): Effekte – Tick =
  Aufrufnummer von `0000:0732` (per `Call` einzeln treibbar, endet mit `retf`); Musik – Tick = ⌊(Time − T₀)/4000h⌋ mit T₀ = Setzen
  des PIT-Teilers, Phase relativ zu Fn 01h; Schreibungen in Fn 00h/05h/06h außerhalb eines Ticks gesondert. Vergleiche je
  Tonbefehl relativ zum Befehlstick, nie absolute PIT-Zeit über Bilder (der Port taktet Bilder). PC-Lautsprecher `2850` entfällt (Nutzer
  11.11.2026); der Port nimmt `DS:0910` = 1 (AdLib) an.
- **Daten und Marker der Tonspuren** (R9PL-3, Leitstelle 22.11.2026): Tabellen aus dem Original (`CS:15B2` und weitere der
  Effektmaschine, Noten-, Operator- und Tempotabellen von `FXDRIVER`) nie als Tabelle im Code, sondern zur Laufzeit gelesen –
  aus `T.EXE` bzw. aus `FXDRIVER.EXE` über einen eigenen LZEXE-Entpacker in `src/Traders.Assets/` (Spur 3, Muster
  `Dgroup.FromExe`). Ausgaben von `tools/fxdriver*` nur in ein git-ignoriertes Verzeichnis. Marker für Fremdprogramme:
  `[ORIGINAL-FX] OOOO` (Offset im entpackten Abbild von `FXDRIVER.EXE` V1.06); `tools/abdeckung.py` erweitert Spur 3 additiv
  (zählt und prüft sie getrennt, Regel 2.8 gilt sinngemäß). OPL2-Synthese (Spur 4) ist kein Original und bekommt keine Marker.
- **Mehrrundenprobe** (`AGENTS.md` §5): Spur 1 über Orakelzustände an Aufruf 19 aus echten Partien (Ende in Woche 0–3, Mensch
  und Computer); Spur 0 die durchgehende Probe bis zum Ende.
- Nur VGA; Regeln auf `Dgroup` mit Offsets aus `DgroupLayout.cs` (nur additiv); `GameState`/`GameStateMapper.cs` nur additiv
  (`partial`). Neue DGROUP-Adressen mit Beleg in `docs/analyse/DATENLAYOUT.md` (je Spur eigener Unterabschnitt).
- Regelrelevante Pixel (R9PL-6): `20B3:069A` (`06BD`–`06ED`) zieht `random(35)`, `random(12)` und wiederholt, bis das Pixel
  `23CD:06DE(x, y+6)` = 2 bzw. 6 ist (nach `DS:090E`) – Zahl der Ziehungen hängt an Pixeln. Spur 1 nach R6P-1:
  `src/Traders.Core/Pixels/Ending*` (additiv), Abbruchzähler im Testaufbau.
- `STATUS.md`: nur der erzeugte Block per `tools/abdeckung.py --write`; nach jedem Merge Abgleichlauf der nächsten Spur.
- Externe Bibliotheken: keine neuen NuGet-Pakete ohne Entscheidung des Nutzers (Lockdateien, Spiegel im Release
  `claude-werkzeuge`); Fremdcode nur nach F1 mit Lizenzhinweis.

**Noch offene Routinen laut Aufrufgraph** (Leitstelle 22.11.2026, `tools/callgraph.py` auf `T.EXE`, transitiv ab dem Einsprung
bis zu Routinen mit `[ORIGINAL]`-Marker; Größen in Byte):

- **Ende `20B3:0005` ab `0053`** (≈ 3300, 26 Routinen): Endbild `008E` (`V-ANBAU`, schreibt `DS:37AA`, `54D6`) mit `015B`,
  `0267`, `0304`, `034F`, `03BF`, `03E5`, `0432`, `047B`, `04DE` und `21FD:0620`/`0634`; Abend `0525` (`V-ABEND`) mit `05B4`,
  `069A` (Zufall 35, 12), `07BA` (schreibt `3320`/`3322`); Rang `0863` (`V-ENDE`, schreibt `54D4`) mit `0986`, `09D4`, `0A9F`,
  `0ADC`, `0B40`, `0B7D`, `0BF0`, `0C05`, `0D0D`, `0D65`, `0D77` (schreiben `1080`–`1089`, `330E`, `3324`); indiziert (Plan-Review
  R9PL-4) in `0267`/`0304`/`034F`/`03BF`: `[bx+4BD2]`, `[bx+3320]`/`[bx+3322]`, `[bx+3022]`, `[bx+4BCA]`; Warten auf Taste
  `27C6:00D2` (schreibt `115D`); `inc word [DS:4B9A]` bei `0071`. Gerufen (portiert): `0825:06CB`, `07C4`, `0813`,
  `0825:000A(0)` (Bilanz – Parameter 0 im Befund prüfen), `0609:1879`/`1FE0`/`13C5` …, `0A96`-, `0B67`-, `21AF`-, `21FD`-, `23CD`-Routinen.
- **Ton in `T.EXE`:** `218B:0002`, `0094`, `013A`, `0153`, `01A4` (Musiksteuerung, ≈ 380), `21A7:000D`, `0043`, `0054`
  (Effekte, ≈ 130), `27C6:041B` (Rückruf setzen, Zustand `CS:0416`/`0418` im Segment `27C6`, nicht DGROUP, R9PL-8);
  Effektmaschine im Laufzeitsegment `0000:06FE`–`15B1` (≈ 0xE20 Code, R9PL-1): Tick `0000:0732` mit `0770` (ruft `13BE`),
  `0866` (ruft `1352`), `121C`, `11C2` (ruft `150E`), **eigener Zufallsgenerator `0000:150E`** (Zustand `CS:03F8`, nicht
  `DS:10D2`; Reset `0000:0812` schreibt `03F0`, `03F4`, `03F8`), Sprungtabelle `0000:151E` (Ziele in `0866`), Tabelle `CS:15B2`,
  Ring `CS:041B`–`042C`. `2850` entfällt.
- **Ton außerhalb `T.EXE`:** `FXDRIVER.EXE` V1.06 (LZEXE-gepackt), genutzte `int 64h`-Funktionen 00h, 01h, 03h, 05h, 06h, 10h,
  12h, 18h, 1Ah, 1Bh (19h ohne Aufrufer); CMF-Stücke `TRADINTR`, `TRADMENU`, `TRAD_END`. Der Musikweg `218B:0094` ruft
  `186C:0A6C` (→ `0000:2C39`, `285B`, Dateisuche) und `186C:0B63` (Diskettendialog, ruft `27C6:00D2`, `21A7:0054`): **entfällt**
  laut `186C_ressourcenlader.md` (F2 = A), Spur 2 meldet sie nicht als offen (R9PL-5). Disassemblierung liegt nicht im
  Referenz-Repo; Entpacken im RefEmu nach `analyse/TON.md` (Abzug bleibt lokal, nie ins Repo).

## 0 `partie` – Gesamtpartie im Host, Einbindung Ende und Ton

- **Teile:** (1) **Einbindung Ende:** `EndCheckStep` (Aufruf 19) ruft bei „Ende“ den Rest aus Spur 1 statt anzuhalten; Abbruchweg
  B zurück nach `1777:000C`, Hauptmenü erneut. (2) **Einbindung Ton:** Tonbefehle aus Kern und Programmablauf (`ProgramSteps`
  Schritte 3–7, Ereignisse „sound call“ der Module, Tastenklick `21A7:0054(1, 16h)` aus den Szenen – `DrawHelpers.cs`,
  `FieldScene.Output.cs`, `StationScene.cs` –, bei laufender Musik mit Stopp und `DS:10A5` = 0 im Kern, R9PL-9) an Spur 2/3 anschließen; Host gibt über Spur 4 aus, Ton an/aus aus dem Menü
  (`DS:10A7`). (3) **Gesamtpartieprobe:** vom Hochlauf über Menü und neue Partie (ein Mensch, drei Computer, niedriges
  Zielvermögen) bis zum Ende in Woche ≥ 2 und zurück ins Hauptmenü; Aufruf 18 aus dem Orakel; Datensegment und `DS:10D2` je
  Aufrufstelle, Bild für Bild für Aufruf 19, Tonbefehlsfolge je Bild gleich; OPL-Registerfolge gegen `OplTrace` (Lauf mit
  `runFxDriver`) je Tonbefehl relativ zum Befehlstick (Zeitbasis oben) mindestens für Vorspann, Menümusik, eine Woche Effekte und
  die Endmusik. Zweiter Fall: Ende über F10. (4) **Host:** spielt die Gesamtpartie ohne Halt,
  Bild für Bild gegen das Original über das Ende bis ins Hauptmenü; Hinweis „Partie zu Ende“ entfällt.
  `MultiWeekRound7DifferentialTests` erweitern statt verdoppeln.
- **Dateigrenzen:** `src/Traders.Core/Phases/**`, `tests/Traders.Tests/Phases/**`, `src/Traders.App/**` außer
  `src/Traders.App/Audio/**`, `tests/Traders.Tests/App/**`, `src/Traders.RefEmu/OriginalProgram.cs` (additiv).
- **Abnahme:** alle drei Läufe grün; Gesamtpartie im Gleichschritt bis ins Hauptmenü, Tonbefehle je Bild und OPL-Folge je
  Befehl relativ zum Befehlstick gleich; Host-Bildtest über das
  Ende; Gegenprobe im Review (Rest von 19 ausgelassen, Wochenzähler bei Ende nicht erhöht, Ton bei `10A7` = 0 trotzdem
  ausgegeben, Musik vor Effekt nicht gestoppt, Host hält am Ende → rot); `check-architecture.py` grün.
- **Abhängigkeiten:** Start nach Merge von 1–4.

## 1 `ende` – Ende und Abend `20B3:0005` ab `0053`

- **Teile:** (1) **Befund und Kern** (`src/Traders.Core/Ending/**`): Befund `20B3_0005_endpruefung.md` erweitern bzw.
  `20B3_008E_ende.md` mit allen Routinen (Liste oben): Ablauf, Zufall in Reihenfolge (`069A`), geschriebene Adressen, Rangfolge,
  `4B9A`++, `0825:07C4`, Bilanz mit Parameter 0, Warten auf Taste `27C6:00D2` (wartet auf ein Make im Tastatur-IRQ `DS:115D`,
  keine Steuerung über `0E9E`: Wartebefehl der Phasenmaschine, `115D` = 0 buchen, R9PL-7), Verwerfungsschleife `069A` mit Pixeln
  (R9PL-6); Tonaufrufe (`218B:0094(2, 3)`
  u. a.) nur als Tonbefehl. Rest als Handler für Aufruf 19 (ohne Einbindung, `IPartialWeekStepHandler`-Rest). Differenztests über
  `LoadOnly`/`Call(20B3:0005)` mit Zuständen aus echten Partien (Ende in Woche 0–3, Sieger Mensch bzw. Computer, Gleichstand,
  F10 ohne Ziel, negatives Vermögen, 16-Bit-Grenzen). (2) **Bild für Bild:** Kern-Ereignisse je Bild, Gleichschritt mit dem RefEmu
  (Datensegment, `DS:10D2`, Bildzähler) für zwei Partien. (3) **Szene** `src/Traders.Presentation/Ending/`: `V-ANBAU`, `V-ABEND`,
  `V-ENDE` pixelgenau gegen den RefEmu je Bild, auch Befehlsliste.
- **Dateigrenzen:** `src/Traders.Core/Ending/**` (`EndCheck.Decide`/`Ends` unverändert, Rest additiv), `tests/Traders.Tests/Ending/**`,
  `src/Traders.Presentation/Ending/**`, `tests/Traders.Tests/Presentation/Ending*`, `docs/routines/20B3_*`, Nachtrag in
  `docs/routines/27C6_0043_eingabe_zeitgeber.md` (nur `00D2`) und `0825_000A_wochenbilanz.md` (nur Parameter 0);
  `src/Traders.Core/Input/**` nur additiv für `27C6:00D2`; `src/Traders.Core/Pixels/Ending*` (neu, R6P-1).
- **Abnahme:** alle drei Läufe grün; Rest von 19 im Gleichschritt, Szene pixelgenau; Gegenprobe im Review (Ziehung 35/12
  vertauscht, Pixelziel 2/6 vertauscht, `4B9A` nicht erhöht, Bilanz mit Parameter 1, Rang bei Gleichstand falsch, ein Bild doppelt
  verarbeitet → rot).
- **Abhängigkeiten:** keine. Größe ≈ 0xCE4 Byte.

## 2 `effekte` – Tonsteuerung `218B`/`21A7` und Effektmaschine

- **Teile:** (1) **Kern** (`src/Traders.Core/Sound/**`): `218B:0002`/`0094`/`013A`/`0153`/`01A4`, `21A7:000D`/`0043`/`0054`,
  `27C6:041B` auf `Dgroup` (Merker `0910`, `10A3`, `10A5`, `10A7`, `10A9`, `10A0+i`), Rückrufzustand `27C6:0416`/`0418` explizit
  geführt (R9PL-8), mit Tonbefehlen als Ausgabe; `int 64h` und
  Treiberzustand als explizite Antwort (`013A` liest den Wiedergabezustand). Differenztests über `LoadOnly`/`Call` (Ton an/aus,
  Musik läuft/steht, gleiches/anderes Stück, ohne Treiber). (2) **Effektmaschine** (`src/Traders.Audio/Effects/**`): Ring mit 16
  Plätzen und Überlauf wie im Original (`0000:0750`, #76 R3H-2), Tick `0000:0732` mit `0770`, `0866`, `121C`, `1352`, `13BE`,
  `11C2`, eigener Zufall `0000:150E` (Zustand `CS:03F8`), Sprungtabelle `151E`, Tabelle `CS:15B2` (zur Laufzeit aus `T.EXE`);
  Ausgabe OPL-Registerfolge je Tick. Differenztest gegen `OplTrace` für alle beobachteten Effekte 1…52h einzeln, eine Folge
  schneller Aufträge (Überlauf) und Effekt während laufender Musik. Befund `0000_0750_effekte.md` und `218B_21A7_ton.md`.
- **Dateigrenzen:** `src/Traders.Core/Sound/**`, `src/Traders.Audio/Effects/**`, `src/Traders.Audio/Opl/OplWrite*.cs`
  (Schnittstelle, gemeinsam mit 3 und 4, zuerst gemergte Spur legt sie an), `tests/Traders.Tests/Sound/**`,
  `tests/Traders.Tests/Audio/Effects*`, `docs/routines/218B_*`, `21A7_*`, `0000_*`, Nachtrag `analyse/TON.md` §4.
- **Abnahme:** alle drei Läufe grün; Merker und Befehle wie im Original; OPL-Folge je Effekt gleich (Tick und Wert), `CS:03F8`
  und `CS:0415`–`042C` nach jedem Tick gleich, auch über Effektgrenzen; Gegenprobe im Review (Ring ohne Überlauf, `a` statt `b`
  gelesen, Musik nicht gestoppt, Ton aus ignoriert, Zufallszustand `03F8` zwischen Effekten zurückgesetzt → rot).
- **Abhängigkeiten:** keine. Größe ≈ 0xF60 Byte; bis zu zwei Fortsetzungen über das Limit (F2 = ja).

## 3 `musik` – CMF-Wiedergabe nach `FXDRIVER.EXE`

- **Teile:** (1) **Befund:** Treiber im RefEmu entpacken (Werkzeug in `tools/` oder Test-Hilfe; Abzug und Disassemblierung bleiben
  lokal), Befund `docs/analyse/FXDRIVER.md` in eigenen Worten: genutzte `int 64h`-Funktionen, Ereignisformat der CMF-Puffer,
  Tempo, Wiederholen (18h), Lautstärkefahrt (19h–1Bh). Marker `[ORIGINAL-FX] OOOO` (Vorgaben oben); Teil 1 erweitert
  `tools/abdeckung.py` additiv (eigene Zählung, Gegenprobe ungültiger Offset → Fehler) und baut den LZEXE-Entpacker in
  `src/Traders.Assets/` (Treibertabellen zur Laufzeit, nie im Code).
  (2) **Player** (`src/Traders.Audio/Music/**`): CMF-Kopf, Instrumente, Ereignisse, Tick je vierten Zeitgebertick, Steuerung
  über die Tonbefehle aus Spur 2. Differenztest gegen `OplTrace` (Lauf mit `runFxDriver`) für alle drei Stücke über ≥ 60 s,
  Stopp, Wechsel, Wiederholen, Einblenden.
- **Dateigrenzen:** `src/Traders.Audio/Music/**`, `src/Traders.Audio/Opl/OplWrite*.cs` (wie 2), `tests/Traders.Tests/Audio/Music*`,
  `src/Traders.Assets/FxDriver*` (neu) mit Tests, `tools/abdeckung.py` (nur Marker `[ORIGINAL-FX]`, additiv),
  `docs/analyse/FXDRIVER.md`, Nachtrag `analyse/TON.md` §2/§5; neues Werkzeug unter `tools/fxdriver*`, Ausgabe nur git-ignoriert.
- **Abnahme:** alle drei Läufe grün; OPL-Folge je Stück gleich (Tick und Wert); Gegenprobe im Review (Tempo ±1, Instrument
  vertauscht, Wiederholen aus, Fahrt ohne Schrittweite → rot).
- **Abhängigkeiten:** keine (Tonbefehle aus 2 über eine schmale eigene Schnittstelle; bei Konflikt rebased die später gemergte
  Spur). Größe unbekannt (Treiber 756h Absätze resident inkl. Puffer); bis zu zwei Fortsetzungen über das Limit (F2).
- **Stand** (Spur, 22.11.2026): PR #244 (Draft). Teil 1 fertig – `FxDriverImage` (LZEXE-Entpacker, Gegenprobe gegen den
  RefEmu), `tools/fxdriver.py`, `[ORIGINAL-FX]` in `tools/abdeckung.py`, Befund `analyse/FXDRIVER.md` (Stückformat „A.H.“,
  Takt, Tempo, Wiederholen, Fahrt, AdLib-Schicht). Teil 2 (Player) offen.

## 4 `klang` – OPL2-Synthese und Tonausgabe im Host

- **Teile:** (1) **Synthese** (`src/Traders.Audio/Opl/**`): YM3812 (OPL2) aus Registerschreibungen zu PCM (49 716 Hz intern,
  Ausgabe 44,1/48 kHz), nach F1. Tests: Frequenz-, Hüllkurven- und Rückkopplungsfälle, Registerfolgen aus `OplTrace` ohne
  Absturz, bitgleiche Ausgabe bei gleicher Eingabe. (2) **Ausgabe** (`src/Traders.App/Audio/**`): MonoGame
  `DynamicSoundEffectInstance`, Takt aus `Traders.App/Timing`, Puffer ohne Lücken; ohne Tongerät stumm weiter. (3) **Hörprobe:**
  WAV-Export einer `OplTrace`-Folge (Werkzeug in `src/Traders.Tools`) für den Hörvergleich mit dem Video durch den Nutzer (M14).
- **Dateigrenzen:** `src/Traders.Audio/Opl/**`, `src/Traders.App/Audio/**` (neu), `tests/Traders.Tests/Audio/Opl*`,
  `src/Traders.Tools/` (nur neuer Befehl), `src/Traders.Audio/README.md`; Lizenzhinweis nach F1 (`THIRD_PARTY.md` neu).
- **Abnahme:** alle drei Läufe grün; Synthese deterministisch und mit Tests nach F1; Host läuft mit und ohne Tongerät; WAV der
  Menümusik liegt dem Nutzer vor (nicht im Repo). Gegenprobe im Review (Hüllkurve ohne Release, Frequenzzahl um ein Bit
  verschoben → rot).
- **Abhängigkeiten:** keine (Eingabe `OplTrace`); Einbindung in den Host durch 0.

## 5 `sammel` – P3-Sammlung (Sonnet)

- **Teile:** (1) #236: Endlosfall `1F63:08DA` begrenzt gegen das Original vergleichen (Abbruchzähler im Harness). (2) #221:
  Alias-Regel in `check-architecture.py` je Methode statt dateiweit, mit Test und Gegenprobe. (3) #220: Mehrrundenprobe in
  `22A3_02D3_ernte.md` auf den Stand R8. (4) #75: R3S-B1–B3. (5) #76: R3H-1, R3H-3 (R3H-2 Ring → Spur 2). (6) #49: `proben.sh`
  N1–N3.
- **Dateigrenzen:** `tests/Traders.Tests/FatMike/**` (nur Test zu #236), `tools/check-architecture.py` mit Tests, `experiments/bestand/proben.sh`,
  `docs/routines/22A3_02D3_ernte.md`, `docs/analyse/GRAFIK.md` §8, Testdateien aus #75/#76 (im PR nennen). Kein Produktivcode;
  belegt ein Test einen Fehler, `spur-wartet` mit Befund.
- **Abnahme:** alle drei Läufe grün; #49, #75, #220, #221, #236 geschlossen, #76 bis auf R3H-2 (`Fixes #…` bzw. Kommentar).
- **Abhängigkeiten:** keine. Früh mergen.
- **Status (Claude Sonnet, 22.11.2026, PR #242):** alle Teile erledigt, `pruefen.sh --schnell` grün (1206 Tests). #236: lange Suche
  beidseitig im Gleichschritt, Endlosfall original-seitig über das Befehlsbudget begrenzt (Port-Schleife ohne Prüfnaht nicht
  begrenzbar, kein Produktivcode). #221: Alias je Block, Gegenprobe rot. #220: Absatz auf Stand R8. #75: B2 (neun Aufrufe) und B3
  (getauschte Seitenzahlen, `index << 2` wird sichtbar) erledigt; **B1 offen**: die Seitenfüllungen von `0B67:0002` sind im Port
  nicht beobachtbar (`InitializeVga` löscht den ganzen VRAM zuvor), eine Prüfnaht in `src/Traders.Vga/` wäre nötig. #76: R3H-1 (Rückrichtung,
  seit R4 in `check-architecture.py`) und R3H-3 (Wortlaut `27C6:00AC`) standen schon auf `main`. #49: N1, N2 erledigt, N3 nur dokumentiert.

## Zuständigkeit gemeinsamer Dateien in R9

`src/Traders.Core/Phases/**`, `src/Traders.App/**` außer `Audio/`, `OriginalProgram.cs` (additiv): 0; braucht eine andere Spur
eine Hilfe in `OriginalProgram.cs`, legt sie sie additiv in eine eigene `partial`-Datei. `src/Traders.Core/`: `Ending/` 1,
`Sound/` 2; `Input/` nur additiv 1; übrige Ordner werden gerufen, nicht geändert (Bedarf: `spur-wartet` mit Begründung).
`src/Traders.Audio/`: `Effects/` 2, `Music/` 3, `Opl/` 4 außer `OplWrite*.cs` (2/3/4, zuerst gemergte Spur legt an, danach nur
additiv). `src/Traders.App/Audio/` 4. `src/Traders.Presentation/Ending/` 1; private Zeichenhilfen anderer Szenen nur
Sichtbarkeit (R8PL-5c). `tools/check-architecture.py`, `experiments/bestand/proben.sh`: 5; `tools/abdeckung.py`: 3 (nur `[ORIGINAL-FX]`). `DgroupLayout.cs`, `GameState.cs` (`partial`),
`GameStateMapper.cs`: nur additiv (alle), bei Konflikt rebased die später gemergte Spur. `docs/analyse/DATENLAYOUT.md`: je Spur
eigener Unterabschnitt; `docs/analyse/TON.md`: 2 (§4), 3 (§2, §5). `STATUS.md`: nur der erzeugte Block (alle). Übriges
`STATUS.md`, `CHANGELOG.md`, dieser Plan, `docs/prozess/**`, `docs/vorlagen/**`: Leitstelle. `TradersRedone.slnx`,
`Directory.Build.props`, `global.json`, Lockdateien: niemand (neue Pakete nur nach Entscheidung des Nutzers).

## Integrationsfolge R9

5 → 1 / 2 / 3 / 4 (in der Reihenfolge der Freigabe) → 0. Gesamtreview am Rundenende (ChatGPT/Codex), danach Abschluss.

## Entscheidungen des Nutzers (R9)

- **G1 = Go** (Nutzer 22.11.2026): Runde 9 mit diesem Plan, sobald das Plan-Review vorliegt (Befunde arbeitet die Leitstelle ein).
- **F2 = ja** (Nutzer 22.11.2026): Spuren 2 `effekte` und 3 `musik` dürfen je bis zu zwei Fortsetzungen über das Limit von 3
  hinaus bekommen.
- **F1 = B** (Nutzer 22.11.2026): OPL2-Synthese in Spur 4 als C#-Übertragung eines freien Emulators mit freizügiger Lizenz
  (z. B. `ymfm`, BSD-3-Clause; Lizenzhinweis und Lizenztext in `THIRD_PARTY.md`, kein NuGet-Paket). Nach v1.0 durch eine eigene
  Umsetzung ersetzen (Erweiterung E8, `PORTIERUNGSPLAN.md` §6).
- **Plan-Review** `reviews/2026-11-22-r9-plan.md` „Nein“ (Opus): P2 R9PL-1–3, P3 R9PL-4–9 eingearbeitet (Leitstelle
  22.11.2026; R9PL-3 entschieden: Marker `[ORIGINAL-FX]`, Tabellen zur Laufzeit, Werkzeugausgabe git-ignoriert); nur Plantext,
  daher Delta-Prüfung der Leitstelle statt Nachprüfung (vom Review so freigestellt).

## Fahrplan

Nach Runde 2, je Runde 2–4 Spuren; Vorschlag nach BESTAND §3.2 (Leitstelle 11.11.2026), jede Runde mit eigenem Plan und Go:

| Runde | Themen | Segmente |
|---|---|---|
| R3 | Phasenmaschine mit Takt und Zufallsereignissen (F3 = A) · Wochenregeln ohne Bildschirm · Grafikschichten · App-Host mit Eingabe · Ton-Analyse | `1777`, Teile von `0825`/`0609`/`22A3`/`08CF`/`0B9F`; `0A96`, `0A69`, `0B67`, `21AF`, `21FD`-Paletten; `0E9E`/`27C6` |
| R4 | Menüs · neue Partie · Start/Code-Abfrage · Spielfeldschicht | `192D`, `0E31`, `0DFD`, `21FD:0687`, Teile von `0609` |
| R5 | Anbau · Anbau-KI und Wochenplanung · Wochenbilanz | `0609`, `135D`, `11A3`, `0825` |
| R6 | Produktion/Ernte/Wetter · Katastrophen · Wochenereignisse | `22A3`, `111D`, `17CB` |
| R7 | Stationen/Rakete/Piraten – in R6 vorgezogen (F1 = B); dafür jetzt Endprüfung (Aufruf 19) · Handel · Handels-KI · Zusatzphase; Roboterphase im Host übersprungen (Abweichung, Nutzer 16.11.2026) | `20B3:0005`, `0B9F`, `0C29`, `141D`, `1DEF` |
| R8 | Fat Mike und Wettbewerb · Einkauf | `1F63`, `08CF` |
| R9 | Ende/Abend (Rest `20B3`) · Ton (Steuerung, Effekte, CMF-Musik, OPL2-Synthese, Ausgabe im Host) · **Gesamtpartie ohne Roboterkampf** (Zwischenziel) | `20B3`, `218B`, `21A7`, `0000:06FE`–`131F`, `FXDRIVER` |
| R10–R11 | Roboterkauf, Aufstellung, Kampffeld, Kampf, je mit KI | `1C68`, `16D6`, `0535`, `12C8`, `1E7E`, `0EDE`, `150F` |
| R12 | Gesamtpartie v1.0; als letzter Schritt vor v1.0 **Aufräumen** (Nutzer 17.11.2026): Quelltext nach Thema zusammenführen, Leichen entfernen · Testdateien nach Thema bündeln · veraltete Analysen und Reviews verdichten oder archivieren, Verweise prüfen · ungenutzte Skripte entfernen | – |

Danach Erweiterungen ([PORTIERUNGSPLAN.md](PORTIERUNGSPLAN.md)).
