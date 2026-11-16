# Arbeitspakete

Die Portierung läuft in **Runden**; je Runde arbeiten mehrere **Spuren** parallel, jede auf einem Branch `paket/<spur>` mit
einem PR, eigenen Dateien und klarer Abnahme. Regeln: [AGENTS.md](../AGENTS.md), Ablauf:
[prozess/zusammenarbeit.md](prozess/zusammenarbeit.md). Den Status pflegt jede Spur nur in ihrem eigenen Abschnitt; Plan-
abschnitte ändert die Leitstelle. Abgeschlossene Runden stehen in [archiv/](archiv/) (Paketphase AP1–AP5: [archiv/runde-0.md](archiv/runde-0.md), Runde 1: [archiv/runde-1.md](archiv/runde-1.md), Runde 2: [archiv/runde-2.md](archiv/runde-2.md), Runde 3: [archiv/runde-3.md](archiv/runde-3.md), Runde 4: [archiv/runde-4.md](archiv/runde-4.md), Runde 5: [archiv/runde-5.md](archiv/runde-5.md), Runde 6: [archiv/runde-6.md](archiv/runde-6.md)).

## Aktuelle Runde: 7 – Endprüfung, Handel, Handels-KI, Zusatzphase (Plan Leitstelle 16.11.2026, F1 = B, F3 = A, Go Nutzer 16.11.2026)

**Ziel:** Nach der Ernte läuft die Woche im Port weiter bis Fat Mike: Endprüfung `20B3:0005` (Aufruf 19, Entscheidung des
Nutzers 16.11.2026), Handel `0B9F:0002` mit Bildschirm und Bedienung `0C29` (Aufruf 23), Handels-KI `141D` und Zusatzphase
`1DEF:000C` (Aufruf 24). Aufruf 20 (`0609:1178`) und 21 (`11A3:0000(1)`) liegen seit R3/R5 vor. Die Roboterphase `1C68` (Aufruf
18, Fahrplan R10–R11) überspringt der **Host** auf Wunsch des Nutzers (16.11.2026) als bewusste, abschaltbare Abweichung; der Kern
bleibt originalgetreu und hält ohne Schalter an Aufruf 18. Danach spielt der Host Woche 0 von der Bilanz bis zum Handel und zur
Zusatzphase und hält an Fat Mike `1F63:000F` (Aufruf 26). Dazu die Pflichten aus R6: #157, #158, #180 (Host), P3 #162, #163,
#165, #166. Ergebnis: M8 (Auktion / Handel) ✅.

**Leitstelle R7:** Claude (Sitzung vom 16.11.2026). **Modell:** alle Spuren und Erstreviews Claude Opus (Spielkern, Zufall, KI,
Disassembly, Differenztests; `zusammenarbeit.md` §5 „Modellwahl“). **Base:** `main` `1eb6b31`. Namen der Läufe: `Traders R7 <spur>`.
**Starts zeitversetzt** (Nutzungslimit): 2 und 0 sofort, 1 und 3 eine Stunde später.

**Gemeinsame Vorgaben für alle vier Spuren** (wie R6):

- Branch `paket/<spur>` von aktuellem `main`, ein Draft-PR, Teile als Aufgabenliste (`spur-start.md` „Teile und Fortsetzung“).
- Je portierter Originalroutine Befund in `docs/routines/` (Sammelbefund erlaubt; Sammelmarker nur in Listenform
  `[ORIGINAL] XXXX:YYYY, XXXX:YYYY`), `[DIFFERENTIAL]`-Test gegen den RefEmu mit Grenzfällen und Zufallszustand `DS:10D2` danach;
  Erwartungswerte nie aus dem Port. Teil 1 jeder Spur prüft die Liste der offenen Routinen unten mit `tools/callgraph.py` nach
  und schreibt sie in den Befund; was einer anderen Spur gehört, wird gerufen, nicht nachgebaut.
- **Kern und Szene getrennt** (`AGENTS.md` §4): Ablauf, Buchungen, Zufall im Kern als Kern-Ereignis je Bild; Szenen in
  `src/Traders.Presentation/` lesen nur und spielen Zeichenereignisse ab (Muster aus R5/R6). Eingaben über die Befehlsquelle
  `0E9E`, Spielerhandlungen als Befehle; alle sechs Steuerungsarten im Kern (R4 F3 = A), Host nur Tastatur. Handler **takten je
  Bild des Originals** (R6S-1); die Abnahme „Bildzähler“ gilt für jeden Handler. Warteschleife `21FD:0004` wird gerufen.
- **Ton** `21A7:0054`/`000D`, `218B:013A`/`0153`: nur als Ereignis bzw. `[APPROX]` bis zur Ton-Runde, wie R6.
- Schritt-Handler liefern die Spuren 1 und 3 als Klassen in ihren Dateien; **eingebunden in die Phasenmaschine werden sie nur von
  Spur 0** (`Phases/**` gehört 0). Bis dahin laufen ihre Differenztests über `LoadOnly`/`Call` bzw. das Wochenorakel.
- **Mehrrundenprobe** (`AGENTS.md` §5; Handel bucht Geld und Vorräte, KI): Spuren 1–3 je Differenztests über Orakelzustände
  ihrer Aufrufstelle in den Wochen 0–3 (Mensch und Computer); Spur 0 Teil 3 die durchgehende Probe.
- Nur VGA; Regeln auf `Dgroup` mit Offsets aus `DgroupLayout.cs` (nur additiv); `GameState`/`GameStateMapper.cs` nur additiv
  (`partial`). Neue DGROUP-Adressen mit Beleg in `docs/analyse/DATENLAYOUT.md` (je Spur eigener Unterabschnitt).
- Regelrelevante Pixel: Laut Aufrufgraph liest keine Routine der R7-Spuren `23CD:06DE` (nur die Endbilder `20B3`, nicht in R7,
  F1); fällt doch eine Pixelentscheidung auf, gilt R6P-1 (Regelseite `src/Traders.Core/Pixels/`, Ergänzung additiv in eigener
  Datei `Pixels/<Spur>*`) und Meldung im PR.
- `STATUS.md`: nur der erzeugte Block per `tools/abdeckung.py --write`; nach jedem Merge Abgleichlauf der nächsten Spur (Merge
  von `main`, `--write`, Label `leitstelle-delta`).

**Noch offene Routinen laut Aufrufgraph** (Leitstelle, `tools/callgraph.py` auf `T.EXE`, transitiv ab dem Einsprung bis zu
Routinen mit `[ORIGINAL]`-Marker): `0B9F`: `0002`, `005C`, `009E`, `00FE`, `01E1`, `024A`, `02BE`, `0321`, `04FC`, `0549`, `0586`,
`060A`, `0695`, `080C` (`06A0`/`078A` Warenpreise portiert) · `0C29`: `0001`, `0046`, `0086`, `015C`, `01B7`, `0323`, `03B2`, `04BD`,
`051A`, `0635`, `065C`, `0739`, `09BB`, `0B1F`, `0B60`, `0BD9`, `0C8A`, `0CC1`, `0D6A`, `0E01`, `0F68`, `0FB9`, `1050`, `109F`,
`1108`, `1163`, `127E`, `134D`, `146B` (Bilder `V-AHANDE`/`V-BHANDE`) · `21FD:04E5`, `21FD:04F9` (gerufen aus `0C29`, später auch
aus `08CF`) · `141D`: `0008`, `0029`, `008C`, `021A`, `0397`, `0464`, `04A5`, `05A3`, `0806`, `09F3`, `0A1D`, `0BFD`, `0CCA`, `0E09`,
`0E67`, `0ED7`, `0F01` (Zufall `0397` 3/3, `0BFD` 400; Einsprünge von außen `0008`, `0397` aus `0B9F`, `021A`, `0464`, `09F3`,
`0CCA`, `0E67` aus `0C29`) · `1DEF`: `000C`, `00DB`, `0105`, `0173`, `01AF`, `020E`, `0227`, `0308`, `03CD`, `04A9`, `0651`,
`06F8`, `0751`, `07AD` (Zufall `00DB` 20, `0173` 6, `01AF` 41/3, `03CD` 5/6) · `20B3:0005` bis zur Entscheidung (`0825:06CB`,
Vergleich Vermögen `DS:2F06` mit Ziel `DS:08F4`, Taste 44h). Schon portiert und gerufen u. a.: `0B9F:06A0`/`078A`, `0E9E:037A`,
`2295:0001`/`0062`, `0A69:000B`/`002C`/`0288`, `0A96`-Schichten, `0B67`, `21FD:0004`/`0185`/`0245`/`051F`/`05DB`/`026C`,
`0609`-Feldroutinen (`11E7`, `13C5`, `1912`, `1ED9`, `1F2D`, `1FCC`, `1FE0`, `215E`, `21BA`), `0825:06CB`/`07C4`/`000A`.

## 0 `ende` – Endprüfung `20B3:0005`, Roboter-Schalter im Host, Einbindung, Host-Pflichten

- **Teile:** (1) **Endprüfung und Host:** Befund `20B3_0005_endpruefung.md`: Rangfolge `0825:06CB`, Vergleich jedes Vermögens mit
  dem Ziel, Taste mit Scancode 44h, Rückgabe `AL` 0/1, `DS:37AA`. Handler für Aufruf 19 im Kern (`src/Traders.Core/Ending/**`):
  ohne Ende Rückgabe 0 im Gleichschritt; bei Ende bleibt der Rest (Endbilder `008E`/`0525`/`0863`, `0825:07C4`, `0825:000A(0)`,
  `27C6:00D2`, Wochenzähler `20B3:0071`) nach F1 = B ausstehend (Muster `CatastropheGuardStep(rest)`; Maschine hält mit Hinweis
  „Partie zu Ende“). Differenztests über `Call(20B3:0005)` und Abbruchweg B
  (`Exit_B_in_lockstep_with_the_original`, jetzt mit Port bis zur Entscheidung). **Roboter-Schalter** (F3 = A: Vorgabe an, `--original-roboter` hält an 18): Host-seitiger
  Leerlauf für Aufruf 18 in `src/Traders.App/Deviations/` (ändert kein Byte von `Dgroup`, zieht keinen Zufall, zeichnet nicht,
  endet im ersten Bild), gekennzeichnet `[APPROX] bewusste Abweichung, Nutzer 16.11.2026`; `WeekSteps` nimmt dafür einen optionalen
  Handler für Aufruf 18 (Vorgabe `null` = ausstehend). Host-Pflichten #157 (Wochenbericht im Host), #158 (Test `HostGame.Update`),
  #180 (Host-Seitenzustand bei Aufruf 4 mit Katastrophe und 9 mit Ereignis Bild für Bild gegen das Original).
  (2) entfällt (F1 = B). (3) **Einbindung** (nach Merge von 1 und 3): Handler für Aufruf 23 (Spur 1) und 24 (Spur 3) in `WeekSteps`;
  **Mehrrundenprobe** über ≥ 4 Wochen mit einem Menschen und drei Computerspielern, in der die Aufrufe 3, 4, 6, 8, 9, 11, 16, 19,
  21, 23, 24 im Port laufen (Aufruf 18 und 26–30 aus dem Orakel; Vergleich Datensegment und `DS:10D2` je Aufrufstelle, Bild für
  Bild für 23 und 24) und nachweislich (Assert) mindestens einmal die Zusatzphase eintritt (Woche bzw. Tastenmuster im Test
  festlegen); Host spielt mit Schalter die Woche bis Aufruf 24 und hält mit Hinweis an Aufruf 26, ohne Schalter weiter an 18.
- **Dateigrenzen:** `src/Traders.Core/Ending/**`, `tests/Traders.Tests/Ending/**`, `docs/routines/20B3_*`,
  `src/Traders.Core/Phases/**`, `tests/Traders.Tests/Phases/**`, `src/Traders.App/**`, `tests/Traders.Tests/App/**`,
  `src/Traders.RefEmu/OriginalProgram.cs` (additiv).
- **Abnahme:** alle drei Läufe grün; Endprüfung im Gleichschritt (beide Rückgaben, Taste 44h, Vermögen = Ziel und Ziel − 1, 16-Bit-
  Grenzen); Kern ohne Schalter hält an 18 (Test), Leerlauf ändert nachweislich nichts (Datensegment und `DS:10D2` vor/nach gleich);
  Gegenprobe im Review (Vergleich `>` statt `>=`, Leerlauf zieht eine Zufallszahl, Handler 23 doppelt getaktet → rot);
  `check-architecture.py` grün.
- **Abhängigkeiten:** Teil 1 sofort; Teil 3 nach Merge von 1 und 3 (sonst `spur-wartet`, „wartet auf 1/3“).
- **Status:** –

## 1 `handel` – Handel `0B9F:0002` mit Bildschirm und Bedienung `0C29`

- **Teile:** (1) **Befund und Kern I** (`src/Traders.Core/Trade/**`): Befund `0B9F_0002_handel.md` mit allen offenen Routinen
  von `0B9F` und `0C29` (Liste oben), Kern- und Zeichenteil je Routine belegen und trennen: Ablauf (vier Aufrufe `0C29:0001` mit
  Argumenten (1,0), (0,1), (2,0), (3,1) – Bedeutung belegen), Angebote, Gebotsprüfung, Preise `DS:4898`, Buchungen auf Bargeld und
  Vorräte, Rückrufe aus `0C29` nach `0B9F`, `21FD:04E5`/`04F9`; KI-Entscheidungen nur über die Klassen der Spur 2 (Vertrag unten).
  Differenztests über `LoadOnly`/`Call` mit Zuständen aus echten Partien (Wochen 0–3, Mensch und Computer, kein Geld, Vorräte an
  der Obergrenze, 16-Bit-Grenzen). (2) **Kern II:** Bedienung Bild für Bild als Kern-Ereignisse (Mensch als Befehle über `0E9E`,
  Computer über Spur 2), Handler für Aufruf 23 (ohne Einbindung); Gleichschritt mit dem RefEmu (Datensegment, `DS:10D2`,
  Bildzähler) für Woche 0 und eine spätere Woche. (3) **Szene** `src/Traders.Presentation/Trade/`: Zeichenteil aller Routinen
  (`V-AHANDE`/`V-BHANDE`, Texte `23CD:05DD`, Zahlen `2295`), pixelgenau gegen den RefEmu je Bild über einen Handel mit Mensch,
  auch Befehlsliste.
- **Dateigrenzen:** `src/Traders.Core/Trade/**`, `tests/Traders.Tests/Trade/**`, `src/Traders.Presentation/Trade/**`,
  `tests/Traders.Tests/Presentation/Trade*`, `docs/routines/0B9F_*` außer `0B9F_06A0*` (dort nur Nachtrag „Aufruf aus
  `0B9F:0002`“), `docs/routines/0C29_*`, `docs/routines/21FD_04E5*`.
- **Abnahme:** alle drei Läufe grün; Handel im Gleichschritt mit dem RefEmu, Szene pixelgenau; Gegenprobe im Review (Käufer und
  Verkäufer vertauscht, Buchung mit Vorzeichenfehler, Gebot an der Grenze falsch geprüft, ein Bild doppelt verarbeitet → rot).
- **Abhängigkeiten:** Spur 2 für alle Stellen, an denen `141D` gerufen wird: bis zu deren Merge Befund, pixelfreie und KI-freie
  Teile und Szene; ist sonst nichts mehr zu tun, `spur-wartet` mit „wartet auf handelski“. Größte Spur der Runde (≈ 0x1D00 Byte);
  bis zu zwei Fortsetzungen über das Limit von 3 (F2 = ja).
- **Status:** –

## 2 `handelski` – Handels-KI `141D` und P3-Sammlung

- **Teile:** (1) **Befund und Kern** (`src/Traders.Core/Ai/Trade*`, neue Dateien): Befund `141D_0008_handelski.md` mit allen 17
  Routinen, Zufall in Reihenfolge (`0397`, `0BFD`), geschriebene Adressen (`DS:2BBA`, `2D60`/`2D62`, `2D74`/`2D76`, `2DC8`/`2DCA`,
  `37AA`, `45D0`). **Vertrag für Spur 1:** je Einsprung von außen (`0008`, `0397`, `021A`, `0464`, `09F3`, `0CCA`, `0E67`) eine
  öffentliche Methode auf `Dgroup` mit `OriginalRandom` des Aufrufers, Parameter und Rückgabe wie im Original (im Befund
  festhalten); die Signaturen committet Teil 1 als Erstes. Differenztests über `LoadOnly`/`Call` je Einsprung mit Zuständen aus
  echten Partien (Wochen 0–3, alle Computerspieler, Grenzfälle Geld/Vorräte/Preise), Zufallszustand danach. (2) **Mehrrundenprobe
  der KI:** an jeder Aufrufstelle aus `0B9F`/`0C29` in den Wochen 0–3 der Vorgabepartie (Orakel) Ergebnis und `DS:10D2` gleich.
  (3) **P3-Sammlung:** #163 (Leseregel Textspeicher in `tools/check-architecture.py`, mit Gegenprobe), #165 und #166 (Tests der
  Katastrophen).
- **Dateigrenzen:** `src/Traders.Core/Ai/Trade*`, `tests/Traders.Tests/Ai/Trade*`, `docs/routines/141D_*`; für #163
  `tools/check-architecture.py` mit Tests; für #165/#166 `tests/Traders.Tests/Disasters/**` (und `src/Traders.Core/Disasters/**`
  nur, wenn #166 einen Fehler belegt).
- **Abnahme:** alle drei Läufe grün; jeder Einsprung im Gleichschritt (Ergebnis, Datensegment, `DS:10D2`); Gegenprobe im Review
  (Ziehung ausgelassen, Vergleich `<` statt `<=`, Spieler vertauscht → rot; neue Architekturregel mit Verletzung → rot).
- **Abhängigkeiten:** keine. Früh mergen (Integrationsfolge), Spur 1 hängt daran.
- **Status:** –

## 3 `zusatz` – Zusatzphase `1DEF:000C`

- **Teile:** (1) **Befund und Kern** (`src/Traders.Core/Extra/**`): Befund `1DEF_000C_zusatzphase.md` mit allen 14 Routinen:
  Auslöseregel (`00DB`, `random(20)`), Wirkung auf Felder und Vorräte (`0609`-Routinen gerufen), Zufall in Reihenfolge, geschriebene
  Adressen `DS:37AA`, `547C`–`5480`, `54A2`–`54A8`, Eingabe über `0E9E:037A`, Warteschleife `21FD:0004(0)`, jedes Bild als
  Kern-Ereignis; Handler für Aufruf 24 (ohne Einbindung). Differenztests über `LoadOnly`/`Call(1DEF:000C)` mit Zuständen, in
  denen die Phase eintritt und nicht eintritt (Zufallszustand gezielt setzen), Wochen 0–3, Mensch und Computer.
  (2) **Szene** `src/Traders.Presentation/Extra/`: Bildteil auf `V-ANBAU`, pixelgenau gegen den RefEmu je Bild, auch Befehlsliste.
  (3) P3 #162 (Grenzwerte der Wochenereignisse im Test).
- **Dateigrenzen:** `src/Traders.Core/Extra/**`, `tests/Traders.Tests/Extra/**`, `src/Traders.Presentation/Extra/**`,
  `tests/Traders.Tests/Presentation/Extra*`, `docs/routines/1DEF_*`; für #162 `tests/Traders.Tests/Events/**`.
- **Abnahme:** alle drei Läufe grün; Phase im Gleichschritt mit dem RefEmu (Datensegment, `DS:10D2`, Bildzähler), Szene pixelgenau;
  Gegenprobe im Review (Auslösung invertiert, eine Ziehung ausgelassen, Vorrat ohne Obergrenze → rot).
- **Abhängigkeiten:** keine.
- **Status:** –

## Zuständigkeit gemeinsamer Dateien in R7

`src/Traders.Core/Phases/**`, `src/Traders.App/**`, `OriginalProgram.cs` (additiv): 0; braucht eine andere Spur eine Hilfe in
`OriginalProgram.cs`, legt sie sie additiv in eine eigene `partial`-Datei. `tools/check-architecture.py`: 2 (nur #163).
`src/Traders.Core/`: `Ending/` 0, `Trade/` 1, `Ai/Trade*` 2, `Extra/` 3; übrige `Ai/`-, `Field/`-, `Week/`-, `Rules/`-,
`Disasters/`-, `Events/`-, `Stations/`-, `Harvest/`-Klassen werden gerufen, nicht geändert (Ausnahme: Tests für #162/#165/#166 wie
oben); braucht eine Spur dort eine Änderung, meldet sie `spur-wartet` mit Begründung. `src/Traders.Presentation/`: `Trade/` 1, `Extra/` 3. `DgroupLayout.cs`, `GameState.cs` (`partial`), `GameStateMapper.cs`: nur additiv (alle), bei
Konflikt rebased die später gemergte Spur. `docs/analyse/DATENLAYOUT.md`: je Spur eigener Unterabschnitt. `STATUS.md`: nur der
erzeugte Block (alle). Übriges `STATUS.md`, `CHANGELOG.md`, dieser Plan, `docs/prozess/**`, `docs/vorlagen/**`: Leitstelle.
`TradersRedone.slnx`, `Directory.Build.props`, `global.json`: niemand.

## Integrationsfolge R7

2 → 3 → 1 → 0 (Spur 0 Teil 1 darf vorher als eigener Merge kommen, wenn er fertig ist und 0 dafür einen eigenen PR
`paket/ende-host` öffnet; Teil 3 erst nach Merge von 1 und 3). Gesamtreview am Rundenende (ChatGPT/Codex), danach Abschluss.

## Entscheidungen des Nutzers (R7)

- **G1 = Go** (Nutzer 16.11.2026): Runde 7 mit diesem Plan (vier Spuren, alle Opus).
- **F1 = B** (Nutzer 16.11.2026): Endprüfung `20B3:0005` nur bis zur Entscheidung; bei Partieende hält die Maschine mit Hinweis
  „Partie zu Ende“, Abend- und Endbilder kommen mit Ende/Abend und Ton (Fahrplan R9). Teil 2 der Spur 0 entfällt.
- **F2 = ja** (Nutzer 16.11.2026): Spur 1 `handel` darf bis zu zwei Fortsetzungen über das Limit von 3 hinaus bekommen.
- **F3 = A** (Nutzer 16.11.2026): Roboter-Schalter im Host standardmäßig an (Aufruf 18 übersprungen, Hinweis in der
  Statuszeile); Startoption `--original-roboter` hält wie bisher an Aufruf 18.

Offen und nicht in R7: P3 #49, #75, #76 (P3-2/-3).

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
| R9 | Ende/Abend (Rest `20B3`) · Ton · **Gesamtpartie ohne Roboterkampf** (Zwischenziel) | `20B3`, `218B`, `21A7` |
| R10–R11 | Roboterkauf, Aufstellung, Kampffeld, Kampf, je mit KI | `1C68`, `16D6`, `0535`, `12C8`, `1E7E`, `0EDE`, `150F` |
| R12 | Gesamtpartie v1.0 | – |

Danach Erweiterungen ([PORTIERUNGSPLAN.md](PORTIERUNGSPLAN.md)).
