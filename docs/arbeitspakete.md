# Arbeitspakete

Die Portierung läuft in **Runden**; je Runde arbeiten mehrere **Spuren** parallel, jede auf einem Branch `paket/<spur>` mit
einem PR, eigenen Dateien und klarer Abnahme. Regeln: [AGENTS.md](../AGENTS.md), Ablauf:
[prozess/zusammenarbeit.md](prozess/zusammenarbeit.md). Den Status pflegt jede Spur nur in ihrem eigenen Abschnitt; Plan-
abschnitte ändert die Leitstelle. Abgeschlossene Runden stehen in [archiv/](archiv/) (Paketphase AP1–AP5: [archiv/runde-0.md](archiv/runde-0.md), Runde 1: [archiv/runde-1.md](archiv/runde-1.md), Runde 2: [archiv/runde-2.md](archiv/runde-2.md), Runde 3: [archiv/runde-3.md](archiv/runde-3.md), Runde 4: [archiv/runde-4.md](archiv/runde-4.md), Runde 5: [archiv/runde-5.md](archiv/runde-5.md), Runde 6: [archiv/runde-6.md](archiv/runde-6.md), Runde 7: [archiv/runde-7.md](archiv/runde-7.md)).

## Aktuelle Runde: 8 – Fat Mike, Wettbewerb, Einkauf, ganze Woche im Host (Plan Leitstelle 17.11.2026, Go Nutzer 17.11.2026)

**Ziel:** Die Woche läuft im Port vollständig durch: Fat Mike `1F63:000F` (Aufruf 26), Einkauf `08CF:0004` (29) und
Wettbewerb/Auswertung `1F63:0B03` (30); Aufruf 27 (Edelsteinkurs `08CF:11D7`) liegt seit R3 vor, der Wochenzähler `DS:4B9A`++
nach Aufruf 30 seit R3 in der Phasenmaschine. Danach spielt der Host mit Roboter-Schalter (R7 F3 = A) Woche für Woche ohne Halt,
bis die Endprüfung (Aufruf 19) „Partie zu Ende“ meldet (R7 F1 = B); ohne Schalter hält er weiter an Aufruf 18. Dazu die P3 aus
R7: #193, #205, #208. Ergebnis: Meilenstein „Woche komplett ohne Roboterkampf“ (Vorstufe des Zwischenziels R9).

**Leitstelle R8:** Claude (Sitzung vom 17.11.2026). **Modell** (`prozess/zusammenarbeit.md` §5): Spuren 0–2 und ihre Erstreviews
Claude Opus (Disassembly, Spielkern, Zufall, KI, Differenztests), Spur 3 Claude Sonnet (Testergänzungen, Doku, kleine
Architekturregel), ihr Erstreview Opus (`zusammenarbeit.md` §5 Nr. 1, R8PL-4); Plan-Review Opus. **Base:** `main` `4be1b06`. Namen der Läufe: `Traders R8 <spur>`.
**Starts zeitversetzt** (Nutzungslimit): 1 und 3 sofort nach dem Go, 2 eine Stunde später, 0 nach Merge von 1, 2 und 3.

**Gemeinsame Vorgaben** (wie R7):

- Branch `paket/<spur>` von aktuellem `main`, ein Draft-PR, Teile als Aufgabenliste (`spur-start.md` „Teile und Fortsetzung“);
  Teilstände, die vorab gemergt werden sollen, auf eigenem Branch, Fortsetzung auf neuem Branch (Erfahrung R7, #185).
- Je portierter Originalroutine Befund in `docs/routines/` (Sammelbefund erlaubt; Sammelmarker nur in Listenform
  `[ORIGINAL] XXXX:YYYY, XXXX:YYYY`), `[DIFFERENTIAL]`-Test gegen den RefEmu mit Grenzfällen und Zufallszustand `DS:10D2` danach;
  Erwartungswerte nie aus dem Port. Teil 1 jeder Code-Spur prüft die Liste der offenen Routinen unten mit `tools/callgraph.py`
  nach und schreibt sie in den Befund; was einer anderen Spur gehört oder schon portiert ist, wird gerufen, nicht nachgebaut.
  `callgraph.py` erfasst nur direkte Schreibziele `[imm16]`; **indizierte Ziele** (`[bx|si|di+0xNNNN]`, z. B. `4BC2[bx]` in
  `1F63:0711`) ergänzt Teil 1 aus dem Disassembly (`ndisasm` je Routine) in Befund und `DATENLAYOUT.md` (R8PL-3).
- **Kern und Szene getrennt** (`AGENTS.md` §4): Ablauf, Buchungen, Zufall im Kern als Kern-Ereignis je Bild; Szenen in
  `src/Traders.Presentation/` lesen nur und spielen Zeichenereignisse ab (Muster R5–R7). Eingaben über die Befehlsquelle `0E9E`,
  Spielerhandlungen als Befehle; alle sechs Steuerungsarten im Kern (R4 F3 = A), Host nur Tastatur. Handler **takten je Bild des
  Originals** (R6S-1), Abnahme „Bildzähler“ für jeden Handler. Warteschleife `21FD:0004` wird gerufen – im Fat-Mike-Modus samt
  Anzeige `1F63:113E`/`104B` (seit R5 in `Week/WaitLoop*`, `random(200)` je Durchlauf, `DS:54B0`; Befund
  `21FD_0004_warteschleife.md`). `113E` ruft auch `1F63:04D8` direkt; dafür darf Spur 1 `Indicator` in
  `src/Traders.Core/Week/WaitLoop.cs` additiv öffentlich machen (nur Sichtbarkeit, R8PL-2), nicht nachbauen.
- **Ton** `21A7:0054`/`000D` samt Helfern `218B:013A`/`0153`, `27C6:041B`, `2850:00CC`: nur als Ereignis bzw. `[APPROX]` bis
  zur Ton-Runde (R9, R8PL-5b).
- Schritt-Handler liefern die Spuren 1 und 2 als Klassen in ihren Dateien; **eingebunden in `WeekSteps`/`WeekScreens` werden sie
  nur von Spur 0** (`Phases/**` gehört 0). Bis dahin laufen ihre Differenztests über `LoadOnly`/`Call` bzw. das Wochenorakel.
- **Mehrrundenprobe** (`AGENTS.md` §5; Fat Mike und Einkauf buchen Geld `DS:3CD2` und Vorräte, Computerspieler): Spuren 1 und 2
  je Differenztests über Orakelzustände ihrer Aufrufstelle in den Wochen 0–3 (Mensch und Computer); Spur 0 die durchgehende Probe.
- Nur VGA; Regeln auf `Dgroup` mit Offsets aus `DgroupLayout.cs` (nur additiv); `GameState`/`GameStateMapper.cs` nur additiv
  (`partial`). Neue DGROUP-Adressen mit Beleg in `docs/analyse/DATENLAYOUT.md` (je Spur eigener Unterabschnitt).
- Regelrelevante Pixel: Laut Aufrufgraph ruft keine Routine der R8-Spuren `23CD:06DE`; fällt doch eine Pixelentscheidung auf,
  gilt R6P-1 (`src/Traders.Core/Pixels/<Spur>*`, additiv) und Meldung im PR.
- `STATUS.md`: nur der erzeugte Block per `tools/abdeckung.py --write`; nach jedem Merge Abgleichlauf der nächsten Spur (Merge
  von `main`, `--write`, Label `leitstelle-delta`).

**Noch offene Routinen laut Aufrufgraph** (Leitstelle 17.11.2026, `tools/callgraph.py` auf `T.EXE`, transitiv ab dem Einsprung bis
zu Routinen mit `[ORIGINAL]`-Marker; Größen in Byte; bestätigt im Plan-Review `reviews/2026-11-17-r8-plan.md`):

- **Fat Mike `1F63:000F`** (≈ 4500): `000F` (`V-FATMIK`, Text 229, `random(6)`), `01B2`, `0305` (Zufall 3, 1), `0390` (Zufall 6,
  15, 20, 5, 5, 5), `04D8` (ruft `113E` → `random(200)`, schreibt `3CFA[…]`), `064A` (Zufall 5), `0711` (schreibt Merker `DS:4BC2`), `079A` (`V-ANBAU`, zwei Texte), `08DA` (zwei
  Ziehungen mit berechneter Grenze), `0A8A`, `0C82`, `0D24`, `0E5C`, `0E8F`, `0EC7`, `0F46`, `0F84`, `0FDF`, `1185`, `1247`,
  `13FE`, `143E`, `147B`.
- **Wettbewerb `1F63:0B03`** (≈ 1700, davon gemeinsam mit Fat Mike `01B2`, `0C82`, `0D24`, `0E5C`, `0EC7`, `0F46`): `0B03`
  (`V-FATMIK`, Texte 231, 232), `11AD`, `0609:0080` (aus `0B03`, gehört zu Spur 1). Spalte „Zufall“ in BESTAND §1.4 war offen:
  statisch keine eigene Ziehung im Baum, nur über `21FD:0004(4)` @`0C67` (Modus 4 = `113E`) – im Befund belegen.
  `0609:0080` setzt `DS:43B2[p]`/`43FA[p]`, `DS:1582` = 1 und ruft die Feldrunde `0609:00D6`; deren Zweig für `1582` = 1
  (Schreibmaschinentext `0A69:0288` mit je Zeichen `21FD:060D` + `0185(2)`) ist im Port `[APPROX]`
  (`Field/Planting.cs`, `Presentation/Field/FieldScene*.cs`) und gehört Spur 1 (R8PL-1).
- Geschriebene Adressen beider Bäume: direkt `DS:1582`, `2EB0`, `2F00`, `330E`–`33AA` (Schritt 1Ah), `37AA`, `3CE6`, `45D0`,
  `54B1`–`54D2`; indiziert (Plan-Review, heuristisch) `08ED`, `3308`, `3320`/`3322`, `3CD2`/`3CD4`, `3CFA`, `43B2`/`43FA`,
  `4890`, `4BC2`, `54BA`, `54C2`, `54CE`. Texte 229, 231–233, 241, 244.
- **Einkauf `08CF:0004`** (≈ 6200, `V-ANBAU`): `0004`, `0082`, `00CD`, `01BD`, `029A`, `039C`, `03E5`, `0455`, `0492`, `04BA`,
  `05F1`, `066D`, `069F`, `0813`, `084F`, `0886`, `0914`, `09C4`, `09F1`, `0AF0`, `0BE1` (Texte 16–18), `0CD5` (Zufall 1), `0DEF`,
  `0E53`, `0F4D` (Zufall 1), `115C`, `1348` (Zufall 1, 2; Texte 19–22; schreibt `DS:15B6`), `1543`, `15C6`, `15F5`, `161F`, `1688`,
  `16B4`, `16F5`, `172A`, `178D`, `1859`, `18B5`, `1935`, `193E`, `1981`. Geschriebene Adressen: direkt `DS:00E8`/`00EA`, `159A`–`15B8`,
  `37AA`, `4B98`; indiziert (Plan-Review, heuristisch) `08ED`, `1586`, `158A`, `159C`, `15A3`–`15AB`, `300E`, `3022`, `3308`,
  `3320`/`3322`, `3786`/`3788`, `37AF`, `3CD2`/`3CD4`, `43B2`, `4BAC`, `4BCA`, `4BD2`.
- Schon portiert und gerufen u. a.: `0609:10D2`/`11E7`/`1256`/`13C5`/`1879`/`18E5`/`1978`/`1AC0`/`1B72`/`1BE5`/`1CB1`/`1ED9`/
  `1F2D`/`1FCC`/`1FE0`/`21BA`/`00D6`, `0825:06CB`, `0A69`-, `0A96`-, `21AF`-Routinen, `0E9E:037A`, `186C:0002`, `2295:006D`,
  `21FD:0004`/`0185`/`01BA`/`0245`/`026C`/`0329`/`037D`/`04E5`/`04F9` (`Rules/Countdown`)/`051F`/`05DB`/`060D`/`0665`,
  `23CD:06E9`/`071C`, `1F63:104B`/`113E`.

## 0 `woche` – Einbindung der Aufrufe 26, 29, 30, ganze Woche im Host

- **Teile:** (1) **Einbindung** (nach Merge von 1 und 2): Handler für Aufruf 26 und 30 (Spur 1) und 29 (Spur 2) in `WeekSteps`/
  `WeekScreens`; damit laufen alle 30 Aufrufe im Port außer 18 (Roboterphase, mit Schalter Leerlauf `RobotPhaseSkip`).
  (2) **Mehrrundenprobe** über ≥ 5 Wochen (Wochenwechsel 0→1→…→4 im Port, `DS:4B9A` und Vermögensverlauf `0825:07C4`
  je Woche) mit einem Menschen und drei Computerspielern; Aufruf 18 aus dem Orakel; Vergleich Datensegment und `DS:10D2` je
  Aufrufstelle, Bild für Bild für 26, 29, 30; mindestens einmal (Assert) Fat Mikes Merker `DS:4BC2` gesetzt und ein Kauf im
  Einkauf (Woche bzw. Tastenmuster im Test festlegen). (3) **Host:** spielt mit Schalter ohne Halt Woche für Woche (Bild für Bild
  gegen das Original bis in Woche 1, Aufruf 8); Hinweis „Partie zu Ende“ bei Ausgang B; Wochenbericht nach der Wochengrenze mit
  richtiger Wochenachse (`ReportScene`, `DS:4B9A` ≥ 1). `MultiWeekRound7DifferentialTests` ersetzen oder erweitern, nicht
  verdoppeln. Dazu #222: optionales Rechenbudget (Höchstzahl Befehle) in `OriginalProgram.Call`/`RunToRoutine`, Abbruch mit
  Routine und CS:IP (additiv).
- **Dateigrenzen:** `src/Traders.Core/Phases/**`, `tests/Traders.Tests/Phases/**`, `src/Traders.App/**`,
  `tests/Traders.Tests/App/**`, `src/Traders.RefEmu/OriginalProgram.cs` (additiv).
- **Abnahme:** alle drei Läufe grün; Probe über ≥ 5 Wochen im Gleichschritt; Host-Bildtest über die Wochengrenze; Gegenprobe
  im Review (Aufruf 30 ausgelassen, Wochenzähler doppelt erhöht, Handler 26 doppelt getaktet, Host hält an 26, Host ignoriert Ausgang B „Partie zu Ende“ → rot, R8PL-5d);
  `check-architecture.py` grün.
- **Abhängigkeiten:** Start nach Merge von 1, 2 und 3 (#208 liegt in `tests/Traders.Tests/App/**`).

## 1 `fatmike` – Fat Mike `1F63:000F` und Wettbewerb `1F63:0B03`

- **Teile:** (1) **Befund und Kern Fat Mike** (`src/Traders.Core/FatMike/**`): Befund `1F63_000F_fatmike.md` mit allen Routinen
  (Liste oben): Ablauf, Zufall in Reihenfolge (`000F`, `0305`, `0390`, `064A`, `08DA` und je Durchlauf `1F63:113E` in der
  Warteschleife), Buchungen auf Bargeld/Vorräte, Merker `DS:4BC2`, Arbeitsdaten `DS:54B1`–`54D2`, Computerspieler; Kern- und
  Zeichenteil je Routine trennen; Handler für Aufruf 26 (ohne Einbindung). Differenztests über `LoadOnly`/`Call(1F63:000F)` mit
  Zuständen aus echten Partien (Wochen 0–3, Mensch und Computer, kein Geld, 16-Bit-Grenzen). (2) **Wettbewerb** `1F63:0B03` mit
  `11AD`, `0609:0080` (Befund `1F63_0B03_wettbewerb.md`), Handler für Aufruf 30; gemeinsame Routinen nur einmal. Dazu die
  zweite Feldrunde (`DS:1582` = 1): `[APPROX]`-Zweige in `Field/Planting.cs` (`StatusFrames`) und
  `Presentation/Field/FieldScene*.cs` originalgetreu ersetzen (nur diese Zweige, additiv), Nachtrag in `0609_0004_anbau.md` (R8PL-1). Differenztests
  wie (1). (3) **Szene** `src/Traders.Presentation/FatMike/`: `V-FATMIK`/`V-ANBAU`, Texte 229, 231–233, 241, 244, pixelgenau
  gegen den RefEmu je Bild für 26 und 30, auch Befehlsliste.
- **Dateigrenzen:** `src/Traders.Core/FatMike/**`, `tests/Traders.Tests/FatMike/**`, `src/Traders.Presentation/FatMike/**`,
  `tests/Traders.Tests/Presentation/FatMike*`, `docs/routines/1F63_*`, `docs/routines/0609_0080*`; für die zweite Feldrunde
  `src/Traders.Core/Field/Planting.cs` und `src/Traders.Presentation/Field/FieldScene*.cs` (nur die `1582`-Zweige),
  Nachtrag in `docs/routines/0609_0004_anbau.md`; `src/Traders.Core/Week/WaitLoop.cs` nur Sichtbarkeit von `Indicator`, Nachtrag
  „Aufruf aus `1F63:04D8`“ in `docs/routines/21FD_0004_warteschleife.md`; übriges `Week/WaitLoop*` wird gerufen.
- **Abnahme:** alle drei Läufe grün; 26 und 30 im Gleichschritt mit dem RefEmu (Datensegment, `DS:10D2`, Bildzähler), Szene
  pixelgenau; Gegenprobe im Review (eine Ziehung ausgelassen oder vertauscht, Buchung mit Vorzeichenfehler, Merker `4BC2` nie
  gesetzt, ein Bild doppelt verarbeitet, zweite Feldrunde ohne `1582` = 1 bzw. ohne Tippbilder → rot).
- **Abhängigkeiten:** keine. Größe ≈ 0x1400 Byte; bis zu zwei Fortsetzungen über das Limit von 3 (F1 = ja).

## 2 `einkauf` – Einkauf `08CF:0004`

- **Teile:** (1) **Befund und Kern I** (`src/Traders.Core/Purchase/**`): Befund `08CF_0004_einkauf.md` mit allen 41 Routinen
  (Liste oben): Ablauf, Angebote und Preise (Edelsteinkurs aus Aufruf 27 wird gelesen, `08CF:11D7` nicht geändert), Buchungen,
  Countdown `21FD:04E5`/`04F9` über `Rules/Countdown`, Zufall in Reihenfolge (`0CD5`, `0F4D`, `1348`), geschriebene Adressen
  (`DS:00E8`/`00EA`, `159A`–`15B8`, `4B98`). Differenztests über `LoadOnly`/`Call` mit Zuständen aus echten Partien (Wochen 0–3,
  Mensch und Computer, kein Geld, Vorräte an der Obergrenze, 16-Bit-Grenzen). (2) **Kern II:** Bedienung Bild für Bild als
  Kern-Ereignisse (Mensch als Befehle über `0E9E`, Computerspieler), Handler für Aufruf 29 (ohne Einbindung); Gleichschritt mit dem
  RefEmu (Datensegment, `DS:10D2`, Bildzähler) für Woche 0 und eine spätere Woche. (3) **Szene**
  `src/Traders.Presentation/Purchase/`: Zeichenteil auf `V-ANBAU`, Texte 16–22, pixelgenau gegen den RefEmu je Bild, auch
  Befehlsliste.
- **Dateigrenzen:** `src/Traders.Core/Purchase/**`, `tests/Traders.Tests/Purchase/**`, `src/Traders.Presentation/Purchase/**`,
  `tests/Traders.Tests/Presentation/Purchase*`, `docs/routines/08CF_*` außer `08CF_11D7*` (dort nur Nachtrag „gelesen von
  `08CF:0004`“); `Rules/Countdown*` wird gerufen, nicht geändert.
- **Abnahme:** alle drei Läufe grün; Einkauf im Gleichschritt mit dem RefEmu, Szene pixelgenau; Gegenprobe im Review (Kauf ohne
  Geldprüfung, Vorrat ohne Obergrenze, Countdown nicht abgezogen, ein Bild doppelt verarbeitet → rot).
- **Abhängigkeiten:** keine. Größte Spur der Runde (≈ 0x1830 Byte); bis zu zwei Fortsetzungen über das Limit von 3 (F1 = ja).

## 3 `sammel` – P3-Sammlung aus R7 (Sonnet)

- **Teile:** (1) #193: `tools/check-architecture.py` erkennt Schreiber über `var`-Alias (`var t = _texts; t.Write(…)`), mit
  Test und Gegenprobe. (2) #205: Handel Wochen 1 und 3 mit echter KI und Bild als Test einchecken (Muster Woche 0/2 aus #186).
  (3) #208: Host-Bildtest „mit Roboter-Schalter, erzwungene Zusatzphase, Ende `1777:02B5`“ aufnehmen; veralteten Verweis in
  `docs/routines/22A3_02D3_ernte.md` (Z. 149, `MultiWeekRound6DifferentialTests.cs`) berichtigen.
- **Dateigrenzen:** `tools/check-architecture.py` mit Tests; `tests/Traders.Tests/Trade/**`; `tests/Traders.Tests/App/**`
  (nur neue Testdatei); `docs/routines/22A3_02D3_ernte.md` (nur der Verweis). Kein Produktivcode; belegt ein Test einen Fehler,
  `spur-wartet` mit Befund.
- **Abnahme:** alle drei Läufe grün (Änderung an der Test-Infrastruktur); neue Regel meldet die Alias-Verletzung (Gegenprobe rot);
  #193, #205, #208 geschlossen (`Fixes #…`).
- **Abhängigkeiten:** keine. Früh mergen (Spur 0 baut in `tests/Traders.Tests/App/**` darauf).
- **Status:** **gemergt 17.11.2026** (PR #216, Claude Sonnet; Review `reviews/2026-11-17-r8-sammel.md` „Ja“); #193, #205, #208
  erledigt; P3 R8S-1/-2 → #220, #221.
- **Status (Claude Sonnet, 17.11.2026):** alle drei Teile fertig, PR #216 (Ready for review), alle drei Läufe von `pruefen.sh` grün; #193, #205, #208 per `Fixes`. Neue Tests: `Week_1_computers_in_lock_step`, `Week_3_with_two_humans_in_lock_step` (`Trade/TradeFrameDifferentialTests.cs`), `HostExtraPhaseFrameDifferentialTests` (`App/`). Kein Produktivcode.

## Nachtrag `zeitlimit` – Zeitlimit und Fortschritt in den Testläufen (Leitstelle 17.11.2026)

- **Anlass:** Endlosschleife im Differenztest `FatMikeDifferentialTests.Punishment_matches_the_original` (#217): ein
  Testaufbau erzeugte einen Zustand, in dem `1F63:08DA` im Original wie im Port endlos zieht; `pruefen.sh` lief > 3 h.
- **Teile:** Zeitlimit je Test (`--blame-hang-timeout`, `TR_HANG_TIMEOUT`, Vorgabe 20 min) in `tools/pruefen.sh` und
  `build.yml`; Fortschritt in der Logdatei; Regel „Machbarkeitsprüfung oder Abbruchzähler“ in `routines/_vorlage.md` und
  `prozess/zusammenarbeit.md` §5 Nr. 1; Empfehlung eigener Zufallsstartwert je Testfall. Modell Sonnet, Erstreview Opus.
- **Status:** PR #223 (Claude, läuft). Rechenbudget im RefEmu (`OriginalProgram.Call`/`RunToRoutine`) → Spur 0 Teil 1 (#222).

## Zuständigkeit gemeinsamer Dateien in R8

`src/Traders.Core/Phases/**`, `src/Traders.App/**`, `OriginalProgram.cs` (additiv): 0; braucht eine andere Spur eine Hilfe in
`OriginalProgram.cs`, legt sie sie additiv in eine eigene `partial`-Datei. `src/Traders.Core/`: `FatMike/` 1, `Purchase/` 2;
übrige Ordner (`Week/`, `Rules/`, `Field/`, `Trade/`, `Ai/` …) werden gerufen, nicht geändert; braucht eine Spur dort eine
Änderung, meldet sie `spur-wartet` mit Begründung. `src/Traders.Presentation/`: `FatMike/` 1, `Purchase/` 2, in `Field/` die `1582`-Zweige 1; übrige Szenen-Ordner werden gerufen;
braucht eine Szene eine private Zeichenhilfe von dort (z. B. Emblem `0609:1879` in `FieldScene.cs`), darf sie sie additiv
öffentlich oder statisch machen (nur Sichtbarkeit, im PR begründet; bei Konflikt rebased die später gemergte Spur, R8PL-5c).
`tools/check-architecture.py`: 3. `DgroupLayout.cs`, `GameState.cs` (`partial`), `GameStateMapper.cs`: nur additiv (alle), bei
Konflikt rebased die später gemergte Spur. `docs/analyse/DATENLAYOUT.md`: je Spur eigener Unterabschnitt. `STATUS.md`: nur der
erzeugte Block (alle). Übriges `STATUS.md`, `CHANGELOG.md`, dieser Plan, `docs/prozess/**`, `docs/vorlagen/**`: Leitstelle.
`TradersRedone.slnx`, `Directory.Build.props`, `global.json`: niemand.

## Integrationsfolge R8

3 → `zeitlimit` / 1 / 2 (in der Reihenfolge der Freigabe) → 0. Gesamtreview am Rundenende (ChatGPT/Codex), danach Abschluss.

## Entscheidungen des Nutzers (R8)

- **G1 = Go** (Nutzer 17.11.2026): Runde 8 mit diesem Plan, sobald das Plan-Review vorliegt (Befunde arbeitet die Leitstelle ein).
- **F1 = ja** (Nutzer 17.11.2026): Spuren 1 `fatmike` und 2 `einkauf` dürfen je bis zu zwei Fortsetzungen über das Limit von 3
  hinaus bekommen.
- **Plan-Review** `reviews/2026-11-17-r8-plan.md` „Nein“ (Opus): P1 R8PL-1, P2 R8PL-2–4, P3 R8PL-5 eingearbeitet (Leitstelle
  17.11.2026); nur Plantext, daher Delta-Prüfung der Leitstelle statt Nachprüfung (vom Review so freigestellt).

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
| R12 | Gesamtpartie v1.0; als letzter Schritt vor v1.0 **Aufräumen** (Nutzer 17.11.2026): Quelltext nach Thema zusammenführen, Leichen entfernen · Testdateien nach Thema bündeln · veraltete Analysen und Reviews verdichten oder archivieren, Verweise prüfen · ungenutzte Skripte entfernen | – |

Danach Erweiterungen ([PORTIERUNGSPLAN.md](PORTIERUNGSPLAN.md)).
