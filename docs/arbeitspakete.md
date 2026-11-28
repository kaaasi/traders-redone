# Arbeitspakete

Die Portierung läuft in **Runden**; je Runde arbeiten mehrere **Spuren** parallel, jede auf einem Branch `paket/<spur>` mit
einem PR, eigenen Dateien und klarer Abnahme. Regeln: [AGENTS.md](../AGENTS.md), Ablauf:
[prozess/zusammenarbeit.md](prozess/zusammenarbeit.md). Den Status pflegt jede Spur nur in ihrem eigenen Abschnitt; Plan-
abschnitte ändert die Leitstelle. Abgeschlossene Runden stehen in [archiv/](archiv/) (Paketphase AP1–AP5: [archiv/runde-0.md](archiv/runde-0.md), Runde 1: [archiv/runde-1.md](archiv/runde-1.md), Runde 2: [archiv/runde-2.md](archiv/runde-2.md), Runde 3: [archiv/runde-3.md](archiv/runde-3.md), Runde 4: [archiv/runde-4.md](archiv/runde-4.md), Runde 5: [archiv/runde-5.md](archiv/runde-5.md), Runde 6: [archiv/runde-6.md](archiv/runde-6.md), Runde 7: [archiv/runde-7.md](archiv/runde-7.md), Runde 8: [archiv/runde-8.md](archiv/runde-8.md), Runde 9: [archiv/runde-9.md](archiv/runde-9.md)).

## Aktuelle Runde: 10 – Roboterphase bis vor den Kampf (Plan Leitstelle 28.11.2026, Go Nutzer 28.11.2026)

**Ziel:** Erste Hälfte von M12. Aufruf 18 der Woche (`1C68:000B`) läuft im Kern originalgetreu **bis vor den Kampf**:
Bedingung (Schalter `DS:411A`, Zyklus `DS:4B9A` mod `DS:08EC`), Roboterkauf `1C68:00A5` mit Kauf-KI `16D6`, Entscheidung
`1C68:006B` („kämpft jemand?“), Ausblenden `21FD:037D(47h)`, Kampffeld `1E7E:0004`, Kampfvorbereitung der KI `150F:012E`,
Aufstellung `12C8:000B`, `0535:078A`, `0535:08ED`, `0535:000A` mit Aufstellungs-KI `12C8`. Kampf `0EDE:0009` und Nachlauf
`0535:06CE` folgen in R11 (Schnitt unten). Dazu die Testlücken #275, #276, die P3 #75, #249 und zwei Werkzeugpunkte aus den
Erfahrungen R9. Ergebnis: Kauf, Kampffeld und Aufstellung je Bild gleich dem RefEmu, Aufruf 18 vollständig, wenn kein Kampf folgt.

**Leitstelle R10:** Claude (Sitzung vom 28.11.2026). **Modell** (`prozess/zusammenarbeit.md` §5): Spuren 0–4 Claude Opus
(Disassembly, Spielkern, Zufall, KI, Differenztests), Spur 5 Claude Sonnet (Testergänzungen, Werkzeug, Doku); Erstreviews und
Plan-Review Opus. **Base:** `main` `08c086a`. Namen der Läufe: `Traders R10 <spur>`.
**Starts zeitversetzt** (Nutzungslimit): 2, 3 und 5 sofort nach dem Go, 1 eine Stunde später, 4 zwei Stunden später, 0 nach Merge
von 1–4. Spur 1 Teil 2 erst nach Merge der ganzen Spur 2, Spur 4 Teil 2 erst nach Merge von Spur 3 Teil 1 (Vorab-PRs, unten).

**Schnitt R10/R11** (Leitstelle 28.11.2026, `tools/callgraph.py` auf `T.EXE`, transitive Hülle ab `1C68:000B` innerhalb der sieben
Robotersegmente; alle Aufrufe nach außen sind schon portiert; Größen in Byte): gesamt 207 Routinen ≈ 34 700. **R10:** 115
Routinen ≈ 18 900 (Kauf 55 ≈ 9 000 mit 1 Aufrufstelle von `random`, Feld und Vorbereitung ≈ 3 900 mit 17, Aufstellung 41 ≈ 6 000 ohne).
**R11:** 92 weitere ≈ 15 800 (`0EDE` fast ganz, `150F` ohne `012E`-Zweig und `1B56`, `1E7E:0BCA`/`0C87`/`08B5`/`08D3`/`09FD`/`0E21`,
`0535:06CE`; 14 Aufrufstellen von `random`) plus Einbindung in Host und Gesamtpartie. `150F:04A8` hat keinen Aufrufer.

**Gemeinsame Vorgaben** (wie R9):

- Branch `paket/<spur>` von aktuellem `main`, ein Draft-PR, Teile als Aufgabenliste (`spur-start.md` „Teile und Fortsetzung“).
  **Vorab-PR:** Spur 2 Teil 1 und Spur 3 Teil 1 auf `paket/<spur>`, Review und Merge vor den übrigen Teilen; die Fortsetzung arbeitet
  auf `paket/<spur>-2` (Erfahrung R7, #185).
- Je portierter Originalroutine Befund in `docs/routines/` (Sammelbefund erlaubt; Sammelmarker nur in Listenform
  `[ORIGINAL] XXXX:YYYY, XXXX:YYYY`), `[DIFFERENTIAL]`-Test gegen den RefEmu mit Grenzfällen und Zufallszustand `DS:10D2` danach;
  Erwartungswerte nie aus dem Port. Teil 1 jeder Code-Spur prüft ihre Routinenliste mit `tools/callgraph.py` nach und ergänzt
  **indizierte Schreibziele** aus dem Disassembly (`[bx+…]`, `[si+…]`; der Aufrufgraph zählt nur direkte, R8PL-3). Was einer anderen
  Spur gehört oder schon portiert ist, wird gerufen, nicht nachgebaut. Ausnahmelisten im Befund (Felder, die der Port bewusst
  anders hält) bekommen einen eigenen Test im Gleichschritt (`routines/_vorlage.md`, Erfahrung R9). Endet das Original nur unter
  einer Bedingung (Eingabe, Verwerfungsschleife), Machbarkeitsprüfung oder Abbruchzähler im Testaufbau, Zeitlimit 20 min je Test.
- **Kern und Szene getrennt** (`AGENTS.md` §4): Ablauf, Buchungen, Zufall im Kern als Kern-Ereignis je Bild; Szenen in
  `src/Traders.Presentation/Robots/` lesen nur. Handler **takten je Bild des Originals** (R6S-1), Abnahme „Bildzähler“. Eingaben
  des Menschen (`1C68:00FC`, `0535:012B` rufen `0E9E:000E`) als Steuerbefehl an den Kern wie in R5–R8; Ton (`21A7:0054` u. a.) nur als
  Tonbefehl über `SoundControl` (R9).
- Regeln auf `Dgroup` mit Offsets aus `DgroupLayout.cs` (nur additiv); `GameState`/`GameStateMapper.cs` nur additiv (`partial`).
  Neue DGROUP-Adressen mit Beleg in `docs/analyse/DATENLAYOUT.md` (je Spur eigener Unterabschnitt „R10 <spur>“); dort auch die
  Bedeutung der Roboterfelder 18–25 (`43CE`, `376C`, `376E`, `2EF8`, `377C`, `40EC`, `33DC`, `3016`), soweit die Spur sie belegt.
- **Mehrrundenprobe** (`AGENTS.md` §5, KI und Geld berührt): Spuren 1–4 über Orakelzustände am Einsprung ihrer Routinen aus echten
  Partien (mindestens vier Roboterwochen, Mensch und Computer, Zyklus 1 und 2); Spur 0 durchgehend über vier Wochen.
- Nur VGA. `STATUS.md`: nur der erzeugte Block per `tools/abdeckung.py --write`; nach jedem Merge Abgleichlauf der nächsten Spur.
  Keine neuen NuGet-Pakete.

**Schnittstellen zwischen den Spuren** (vollständig festgelegt, Erfahrung R9):

| Was | Einheit, Nullpunkt, Phase | zuständig |
|---|---|---|
| Teilschritte von Aufruf 18 | je Original-Routine ein `IWeekStepHandler` (`Begin`/`Tick` wie `PurchaseStep`), beginnt im Bild des Far-Calls aus `1C68:000B` und endet in dem Bild, in dem das Original deren `retf` erreicht; Bildzähler = Bilder seit Beginn von Aufruf 18 | jede Spur für ihre Einsprünge; Spur 0 setzt zusammen |
| `1C68:006B` „kämpft jemand?“ | Ergebnis `bool` (AL ≠ 0) im Ergebnis des Kauf-Handlers; schreibt `DS:37AA` | Spur 1 |
| Kauf-KI `16D6:000E` und `16D6:06F6` | reine Funktionen auf `Dgroup` + `OriginalRandom` (eine Aufrufstelle von `random` laut Graph), ohne Bild; `000E` gerufen aus dem Kauf-Handler an `1C68:00CB`, `06F6` aus `1C68:00FC` an `1C68:0127` (schreibt `[bx+37C3]` über `074B`/`08D6`, Steuerbits der Computer im Kaufbildschirm, R10PL-1) | Spur 2 |
| Hilfsroutinen `1C68:0A28`/`0AB0`/`0B02`, `150F:1B56` | `public static` auf `Dgroup`, ohne Bild, in `src/Traders.Core/Robots/Shared/` (Vorab-PR Spur 2); `0A28`/`0AB0` von `16D6` und `1C68:0515`/`0681`/`0812` (nicht in R11), `0B02` von `16D6:0823`, `1C68:0E87` und in R11 `0EDE:16D2`, `1B56` von `16D6:0067` und in R11 `0EDE:051C`/`16D2` gerufen (R10PL-1, R10PL-4) | Spur 2 |
| Kampffeld `DS:48F4` | 9 × 26 Byte (Zeile × Spalte wie `22A3`, `DATENLAYOUT.md` §4), Arbeitsdaten `54xx` des Feldes; `1E7E:0004` füllt es, danach nur lesend außer durch Routinen der Spur 3 | Spur 3 |
| Feld-Hilfen `1E7E:083F`/`0870`/`088D`/`08F1`/`0DB3` | `public static` auf `Dgroup` in `src/Traders.Core/Robots/Field/` (Vorab-PR Spur 3); **Zeichenanteile als Befehlsliste** (`0DB3` ruft `0A96:0311` Typ 7 vor dem Schreiben des Rasters, `08F1` zusätzlich `0A96:0525` Typ 12; R10PL-2), Befehle gehen an den Aufrufer zurück; gerufen von `0535:041F` (`083F`/`0870`/`088D`) und in R11 `0EDE:11DA`/`136C`/`189C` (nicht `0870`, R10PL-4) | Spur 3 |
| Kampfvorbereitung `150F:012E`, `150F:0406` | Teilschritt-Handler für den Aufruf aus `1C68:000B` **und** daneben `public static` auf `Dgroup` (rein, kein Zufall); R11 ruft `012E` erneut aus `0EDE:0362`, `0406` aus `150F:100B` (R10PL-4) | Spur 3 |
| Aufstellungshilfen `0535:095C`/`0AE1`/`0B3F`/`0B8D`, `0EDE:1EA8`/`1F3F`, `1C68:0E87`, `0EDE:0322`/`1826` | öffentlich aufrufbar (R11 nutzt sie im Kampf); Zeichenanteile als Befehlsliste der Szene | 4 bzw. 1 |
| Roboterfelder 18–25 (`43CE`, `376C`, `376E`, `2EF8`, `377C`, `40EC`, `33DC`, `3016`) und gemeinsame indizierte Ziele (`302E`/`3030`, `3786`/`3788`, `3320`/`3322`, `330E`, `43B2`; `37C3` mit Spur 2) | Namen und Einträge in `DgroupLayout.cs`/`DATENLAYOUT.md` legt **Spur 1** an (`1C68:03C8`, `099D`); Spur 2 und 4 nutzen sie und ergänzen nur eigene Adressen (R10PL-5) | Spur 1 |
| Zufall | nur `OriginalRandom` auf `DS:10D2`; Tests prüfen Zahl und Reihenfolge der Ziehungen je Routine | alle |

**Zuständigkeit gemeinsamer Dateien:** `WeekSteps.cs`/`WeekScreens.cs` (Bindung von Aufruf 18) und `src/Traders.App/**` nur Spur 0;
`DgroupLayout.cs`, `GameState*.cs`, `OriginalProgram.cs`, `DATENLAYOUT.md` additiv durch alle mit Begründung im PR.
**Integrationsfolge:** 2 Teil 1, 3 Teil 1 (vorab) → 5 → 2, 3 → 1 → 4 → 0.

## 0 `roboter` – Aufruf 18 bis vor den Kampf im Kern, Host

- **Teile:** (1) **Handler `RobotPhaseStep`** (`src/Traders.Core/Robots/Phase/`): `1C68:000B` – Bedingung (`411A` = 0 oder
  `4B9A` mod `08EC` ≠ 0 → sofort zurück; `idiv` mit Vorzeichen über `Int16Math`), Kauf (Spur 1), bei `006B` = 0 Ende, sonst
  `21FD:037D(47h)`, Feld (Spur 3), `150F:012E` (Spur 3), `12C8:000B` und Aufstellung (Spur 4) nacheinander je Bild; **Kampf
  `0EDE:0009` und `0535:06CE` als ausstehender Rest** (`IPartialWeekStepHandler`-Rest wie Aufruf 19 in R7), in Tests aus dem Orakel.
  Bindung an Aufruf 18 in `WeekSteps`/`WeekScreens` (additiv, ohne Rest hält die Maschine vor dem Kampf). Grenzfall der Bedingung
  synthetisch über `OriginalProgram.Call(0x1C68, 0x000B)` mit Abbruch am Einsprung `1C68:00A5`: `4B9A` = `8001h`, `08EC` = 3
  (mit Vorzeichen Rest −1 → Rückkehr; vorzeichenlos 0 → Kauf), weil Woche ≥ 0 und Zyklus 1–4 `div` und `idiv` nicht unterscheiden (R10PL-3). (2) **Probe:** Gleichschritt
  Bild für Bild (Datensegment, `DS:10D2`, Bildzähler) über vier Wochen mit Schalter an, Zyklus 1 und 2, Mensch und drei Computer:
  Wochen ohne Roboterphase, Kauf ohne Kampf (Aufruf 18 vollständig), Kauf mit Kampf bis zum Einsprung `0EDE:0009`, Kampf aus dem
  Orakel; Fall Schalter aus. `MultiWeekRound7DifferentialTests` erweitern statt verdoppeln. (3) **Host nach F1 = A:** Überspringen bleibt Standard; `--original-roboter` spielt bis vor den Kampf und hält dort.
- **Dateigrenzen:** `src/Traders.Core/Robots/Phase/**`, `tests/Traders.Tests/Robots/Phase/**`, `src/Traders.Core/Phases/**` (nur
  Bindung von Aufruf 18), `tests/Traders.Tests/Phases/**`, `src/Traders.App/**`, `tests/Traders.Tests/App/**` außer den Dateien der
  Spur 5 (`HostWholeGame*`, `ProgramHostTests.cs`; nach deren Merge erweiterbar), `src/Traders.RefEmu/OriginalProgram.cs` (additiv).
- **Abnahme:** alle drei Läufe grün; Gleichschritt bis vor den Kampf und vollständig ohne Kampf; Gegenprobe im Review (Zyklus mit
  `div` statt `idiv` im synthetischen Grenzfall, `006B` ignoriert, Ausblenden fehlt, Aufstellung vor Feld, ein Bild doppelt → rot); Host nach F1;
  `check-architecture.py` grün.
- **Abhängigkeiten:** Start nach Merge von 1–4.
- **Status:** –

## 1 `kauf` – Roboterkauf `1C68` (Steuerung, Bildschirm, Mensch)

- **Teile:** (1) **Befund** `1C68_000B_roboterphase.md` (alle Routinen von `1C68` in der R10-Hülle außer `0A28`/`0AB0`, dazu
  `0EDE:0322`/`1826`): Ablauf `00A5` (lädt `V-KAMPF`, `177F`, `01A8`/`02ED`, `21FD:037D(553h)`, `0B75` mit `0E67`/`0E87`/`0EF6`/
  `0D8C`/`0DEA`/`1372` (`0B02` von Spur 2), Kauf-KI, `17FE`, Palette `21FD:026C(E4Ch)`, `00FC` Mensch mit `0681`/`1061`/`0812`/`0F41`/`0FE9`/`099D`,
  `04A9`/`0515`/`1422`, `0641`, `1824`, `14B9`/`14D7`/`152E`/`15BB`/`1617`/`16F4`, `0B51`/`10A3`/`10DC`/`0FA3`/`129F`, `21AF:0472`, `03C8` setzt Felder 19–25), `006B`, geschriebene Adressen (direkt: `0E72`, `0F0A`, `37AA`, `5470`–`5478`, `547A`; indizierte nach R10PL-5, Felder 18–25
  benennt diese Spur), Kern ohne
  Kaufablauf (Steuerung, `006B`, `03C8`), Differenztests über `LoadOnly`/`Call`. (2) **Kaufablauf** (nach Merge der ganzen Spur 2):
  Kauf-Handler Bild für Bild mit Menscheneingaben (kaufen, verkaufen, nichts, Geld knapp, Grenzen) und Kauf-KI an `00CB` und `0127`; Ergebnis
  `006B`. (3) **Szene** `src/Traders.Presentation/Robots/Purchase*`: Kaufbildschirm pixelgenau gegen den RefEmu je Bild, Befehlsliste.
- **Dateigrenzen:** `src/Traders.Core/Robots/Purchase/**`, `tests/Traders.Tests/Robots/Purchase/**`,
  `src/Traders.Presentation/Robots/Purchase*`, `tests/Traders.Tests/Presentation/Robots/Purchase*`, `docs/routines/1C68_000B_*`.
- **Abnahme:** alle drei Läufe grün; Kauf im Gleichschritt mit Mensch und Computern, Szene pixelgenau; Gegenprobe im Review
  (Felder 19–25 nicht gesetzt, Preis mit Vorzeichen falsch, Mensch-Eingabe doppelt, `006B` liefert 1 ohne Roboter → rot).
- **Abhängigkeiten:** Teil 2 nach Merge der ganzen Spur 2 (R10PL-1). Größe ≈ 6 100 Byte.
- **Status:** Teil 1 fertig (PR #294, Claude): Befund, `RobotPurchaseRules`/`RobotReadiness` (`006B`, `01A8`, `02ED`, `03C8`, `0641`, `0EDE:0322`/`1826`) mit Differenztests, Namen in `DgroupLayout.cs`/`DATENLAYOUT.md`. Teil 2 wartet auf Merge der Spur 2; Teil 3 baut auf Teil 2 auf.

## 2 `kauf-ki` – Kauf-KI `16D6` und gemeinsame Hilfen

- **Teile:** (1) **Vorab-PR:** Befund und Port `1C68:0A28`/`0AB0`/`0B02`, `150F:1B56` (`Robots/Shared/`, Schnittstelle oben), Differenztests
  mit Grenzfällen. (2) **Kauf-KI** `16D6:000E` mit `0067`, `01ED`/`02F7`, `037C`/`0499`/`057C`, `05DB`/`0823`, `06F6`/`074B`/`08D6`
  (Befund `16D6_000E_roboterkauf_ki.md`; Zufall 1×, Reihenfolge belegen), Differenztests je Computerspieler über Orakelzustände
  (vier Roboterwochen, verschiedene Level/IQ, Geld knapp, alle Teile gekauft). (3) Mehrrundenprobe der KI allein (Zustand nach
  `16D6:000E` je Woche gegen den RefEmu).
- **Dateigrenzen:** `src/Traders.Core/Robots/Shared/**`, `src/Traders.Core/Ai/RobotPurchase*`, `tests/Traders.Tests/Robots/Shared/**`,
  `tests/Traders.Tests/Ai/RobotPurchase*`, `docs/routines/16D6_*`, `docs/routines/1C68_0A28_*`, `docs/routines/1C68_0B02_*`, `docs/routines/150F_1B56_*`.
- **Abnahme:** alle drei Läufe grün; Gegenprobe im Review (Ziehung ausgelassen, Vergleich vorzeichenlos statt mit Vorzeichen,
  Spielerschleife ab 1 statt 0 → rot).
- **Abhängigkeiten:** keine. Größe ≈ 3 000 Byte.
- **Status:** –

## 3 `feld` – Kampffeld `1E7E:0004` und Kampfvorbereitung der KI `150F:012E`

- **Teile:** (1) **Vorab-PR:** Befund `1E7E_0004_kampffeld.md` (ganzes Segment kurz, R10-Teil ausführlich) und Port der Feld-Hilfen
  `083F`/`0870`/`088D`/`08F1`/`0DB3` (Schnittstelle oben) mit Differenztests. (2) **Kampffeld** `1E7E:0004` mit `07E8`, `07AA`,
  `0132`, `049C`/`06A7`, `0812`, `09C4`: 17 Aufrufstellen von `random`, Ziehungen in Reihenfolge, Raster `DS:48F4` und Arbeitsdaten; Verwerfungs- oder
  Wiederholschleifen mit Abbruchzähler; Differenztests über viele Startwerte (`new Random(seed + n)`), Grenzen des Rasters. (3)
  **`150F:012E`** mit `0438`, `02C2`/`0331`/`0406`, `065A`, `04D1`/`05BB` (Befund `150F_012E_kampfvorbereitung.md`), Differenztests je
  Computerspieler. (4) **Szene** `src/Traders.Presentation/Robots/Field*`: `1E7E:0004` zeichnet (Hülle ruft `0A96:001B`, `0267`, `0311`, `0525`,
  `06A8`, `0C3F`; R10PL-2) – Kampffeld pixelgenau gegen den RefEmu je Bild, Befehlsliste.
- **Dateigrenzen:** `src/Traders.Core/Robots/Field/**`, `src/Traders.Core/Ai/RobotBattle*`, `tests/Traders.Tests/Robots/Field/**`,
  `tests/Traders.Tests/Ai/RobotBattle*`, `src/Traders.Presentation/Robots/Field*`, `tests/Traders.Tests/Presentation/Robots/Field*`,
  `docs/routines/1E7E_*`, `docs/routines/150F_012E_*`.
- **Abnahme:** alle drei Läufe grün; Raster, `DS:10D2` und Ziehungszahl gleich, Befehlsliste der Feld-Hilfen gleich, Szene pixelgenau je Bild; Gegenprobe im Review (zwei Ziehungen vertauscht,
  Raster um eine Spalte verschoben, Abbruchbedingung `<` statt `<=` → rot).
- **Abhängigkeiten:** keine. Größe ≈ 3 900 Byte.
- **Status:** –

## 4 `aufstellung` – Aufstellung `0535` mit Aufstellungs-KI `12C8`

- **Teile:** (1) **Aufstellungs-KI** `12C8` ganz (`000B`, `003B`/`005A`/`00B5`/`015C`/`024E`/`02BB`, `030B`/`02EA`, `06D6`,
  `0539`/`0621`/`08EA`; reine Logik, Befund `12C8_000B_aufstellung_ki.md`), Differenztests über Orakelzustände. (2) **Aufstellung im
  Kern** (nach Merge Spur 3 Teil 1): `0535:078A`/`07CA`, `08ED` mit `0EDE:1F3F`/`1EA8`, `000A` mit `0BFD`, `00B4`/`031E`/`095C`/`0995`/
  `0A07`, `0C88`/`0CB3`, `0B8D`, `012B` (Mensch über `0E9E`) mit `0AE1`/`0B3F`/`027A`/`02C1`/`0360`/`041F`/`05E8`/`0D04`, `063A`
  (Befund `0535_000A_aufstellung.md`); Handler je Einsprung Bild für Bild, Effekte `21A7:0054` (0/1/2, 38h–3Ah) als Tonbefehl.
  (3) **Szene** `src/Traders.Presentation/Robots/Lineup*`: Kampfwertanzeige `078A`/`08ED` und Aufstellung pixelgenau je Bild.
- **Dateigrenzen:** `src/Traders.Core/Robots/Lineup/**`, `src/Traders.Core/Ai/RobotLineup*`, `tests/Traders.Tests/Robots/Lineup/**`,
  `tests/Traders.Tests/Ai/RobotLineup*`, `src/Traders.Presentation/Robots/Lineup*`, `tests/Traders.Tests/Presentation/Robots/Lineup*`,
  `docs/routines/0535_*`, `docs/routines/12C8_*`, `docs/routines/0EDE_1EA8_*`.
- **Abnahme:** alle drei Läufe grün; Aufstellung im Gleichschritt mit Mensch und Computern, Szene pixelgenau; Gegenprobe im Review
  (Kampfwert `40EC` ohne Obergrenze, KI wählt Platz eins daneben, Effekt nicht gestoppt, Bild doppelt → rot).
- **Abhängigkeiten:** Teil 2 nach Merge von Spur 3 Teil 1. Größe ≈ 5 800 Byte.
- **Status:** –

## 5 `sammel` – Testlücken, P3, Werkzeuge

- **Teile:** (1) **#275:** Host/RefEmu-Partie mit regulär gewähltem, unverändertem Ziel (niedrigstes Zielvermögen, vier Computer,
  Startwert und Eingaben dokumentiert, Roboterphase übersprungen und ausgewiesen) bis zum Ende Bild für Bild; Laufzeit begrenzen.
  (2) **#276:** Hostprobe Ende → Hauptmenü → fortsetzen → nächste Woche (Wochenzähler, Vermögen, Vorräte, `DS:10D2`, Tonzustand),
  Erwartung aus dem Kern-Zustand vor dem Ende. (3) **#249** Kommentar, **#75** B1–B3 (Seitenfüllungen im Test, GRAFIK.md §8,
  Mutationen M18/M19). (4) **Werkzeug:** Hintergrundmodus `tools/pruefen.sh --hintergrund` (Log-Datei, Fortschrittszeile je Lauf,
  Ende-Marker mit Tabelle, Abfrage `--stand`), Doku in `spur-start.md` Schritt 5 und `review-anfrage.md`; Prüfung der
  Ausnahmelisten-Tests in `tools/abdeckung.py --check` (Befund mit Ausnahmeliste ohne zugehörigen Test → Warnung).
- **Dateigrenzen:** `tests/Traders.Tests/App/HostWholeGame*`, `tests/Traders.Tests/App/ProgramHostTests.cs`,
  `tests/Traders.Tests/FatMike/FatMikeDifferentialTests.cs`, `tests/Traders.Tests/Vga/**`, `docs/analyse/GRAFIK.md`, `tools/pruefen.sh`,
  `tools/abdeckung.py`, `docs/vorlagen/spur-start.md` (nur Schritt 5), `docs/vorlagen/review-anfrage.md` (nur Laufzeit),
  `docs/routines/_vorlage.md`.
- **Abnahme:** alle drei Läufe grün (Test-Infrastruktur); Gegenprobe im Review (Ziel nachträglich gesetzt, Fortsetzen ohne Woche +1,
  Hintergrundlauf meldet Ende vor Testende → rot).
- **Abhängigkeiten:** keine; vor Spur 0 mergen (Spur 0 erweitert `App/**`).
- **Status:** –

## Entscheidungen des Nutzers (Runde 10, 28.11.2026)

- **G1 = Go.**
- **F1 = A:** Überspringen bleibt im Host Standard; mit `--original-roboter` spielt der Host Kauf, Feld und Aufstellung original
  und hält vor dem Kampf (Spur 0 Teil 3).
- **F2 = nein:** keine Fortsetzungen über das Limit von 3.
- **F3 = ja:** Erstreview für #283 (Abdeckungsgrafik) als Lauf (Opus), danach Merge; #284 (Hörvergleich M14) mergen.

Nicht in R10: Ton-Befunde #285–#287 (HV-1–3, Tonausgabe bis kurz vor v1.0 zurückgestellt, Nutzer 28.11.2026) → R12; #262
(Tastenklicks in den Kern) → Aufräumen R12 (Leitstelle 28.11.2026).

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
| R10 | Roboterphase bis vor den Kampf: Steuerung, Kauf mit KI, Kampffeld, Kampfvorbereitung der KI, Aufstellung mit KI (Plan 28.11.2026) | `1C68`, `16D6`, `1E7E:0004`, `150F:012E`, `12C8`, `0535` (ohne `06CE`) |
| R11 | Kampf mit KI, Nachlauf `0535:06CE`, Einbindung Aufruf 18 im Host (Ende der Überspringung), Gesamtpartie mit Roboterkampf; M12 | `0EDE`, `150F` (Rest), `1E7E` (Rest), `0535:06CE` |
| R12 | Gesamtpartie v1.0; vorher Ton-Befunde #285–#287 (Tonausgabe zurückgestellt, Nutzer 28.11.2026); als letzter Schritt vor v1.0 **Aufräumen** (Nutzer 17.11.2026): Quelltext nach Thema zusammenführen, Leichen entfernen, #262 · Testdateien nach Thema bündeln · veraltete Analysen und Reviews verdichten oder archivieren, Verweise prüfen · ungenutzte Skripte entfernen | – |

Danach Erweiterungen ([PORTIERUNGSPLAN.md](PORTIERUNGSPLAN.md)).
