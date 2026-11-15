# Review R6 Spur 2 `ereignisse` – PR #146 (Erstreview)
Reviewer: Claude Opus 5.5 (geplanter Lauf, eigene Sitzung, nicht an #146 beteiligt) · Base `c9d7b99` · Head
`0d3da4a13868cfbed53aad39dceba0055db9e366` · Checkout verifiziert (`git rev-parse HEAD` = Head, `git status --porcelain` leer),
Originaldaten geprüft (Hashes in `tools/pruefen.sh`).

## 1. Merge-Empfehlung

**Ja** für `0d3da4a`. Alle drei Läufe von `tools/pruefen.sh` grün, CI `linux` grün auf dem Head. Kern `17CB:0005`
mit `001E`/`02BD`/`039D`/`047E`/`04D7`/`0736`/`0986`/`09E0` gegen die Disassemblierung gelesen: Reihenfolge, Vorzeichen
und Breiten stimmen. Die Differenztests holen die Erwartungswerte aus dem RefEmu. Die Gegenproben der Abnahme sind rot. Die
Leseregel #139 und die Rückrichtung #76 P3-1 wirken. Es bleiben drei P3, keiner davon blockiert. Gegen aktuellen `main` (`94f6390`)
gibt es keinen Konflikt (`git merge-tree`); auf dem Merge-Ergebnis sind `abdeckung.py --check` und `check-architecture.py` grün.
`pruefen.sh --schnell` auf dem Merge-Ergebnis habe ich nicht ausgeführt, das bleibt Merge-Prüfung §6 Nr. 2.

## 2. Befunde

**R6E-1 (P3, Testlücke) – Grenzwerte der Ereignisse sind nicht getestet.** Der Port ist an diesen Stellen richtig
(`17CB:0489` `jnc` → `≥ 200` Klasse 1, `17CB:04C7` `≥ 2000` Klasse 4, `17CB:0206` Kappung erst `> 3EDh`, `17CB:057B` `jnc` →
mindestens 7 Felder). Mutationen genau an diesen Grenzen fallen aber in keinem Test auf:
[`WeekIncidents.cs:211`](https://github.com/kaaasi/traders-redone/blob/0d3da4a13868cfbed53aad39dceba0055db9e366/src/Traders.Core/Events/WeekIncidents.cs#L211)
`amount < 200` → `<= 200` und `< 2000` → `<= 2000`, die Kappung in
[Z. 167](https://github.com/kaaasi/traders-redone/blob/0d3da4a13868cfbed53aad39dceba0055db9e366/src/Traders.Core/Events/WeekIncidents.cs#L167)
`> 0x3ED` → `>= 0x3ED` und die Feldgrenze in
[Z. 235](https://github.com/kaaasi/traders-redone/blob/0d3da4a13868cfbed53aad39dceba0055db9e366/src/Traders.Core/Events/WeekIncidents.cs#L235)
`< 7` → `< 8`. Beleg: `dotnet test … --filter FullyQualifiedName~Traders.Tests.Events` mit `TR_REQUIRE_GAMEDATA=1` → je 24/24
grün. Vorschlag: je ein Szenario mit Betrag genau 200/2000 (Lücke 267 bzw. 2667 bei ¾), Verlust ⅓ genau 3EDh und
Übernahme mit genau 7 Feldern.

**R6E-2 (P3, Testlücke Architekturcheck) – Schreiben in den Textspeicher erkennt die Leseregel nicht.** Mit diesem PR hat der
Kern einen zweiten beschreibbaren Zustand außerhalb des Datensegments (`ITextMemory.Write`,
[`IncidentTexts.cs:15`](https://github.com/kaaasi/traders-redone/blob/0d3da4a13868cfbed53aad39dceba0055db9e366/src/Traders.Core/Events/IncidentTexts.cs#L15),
`IncidentText.FormatNumber` Z. 57). `_DIRECT_WRITE` erkennt nur `.Set(` und `Bytes[…]`. Probe: `core_writers` über
`src/Traders.Core/**/*.cs`, dann `scene_violations` für `ITextMemory t = _texts; t.Write(1, 2, 3);`,
`IncidentText.FormatNumber(_d, _texts, 5, 4, 0, 0);` und `TextBufferMemory t = _t; t.Write(1,2,3);` → jeweils `[]`, und
`TextBufferMemory.Write`/`FormatNumber` stehen nicht in den Schreibern. Heute schreibt keine Szene dort: `IncidentScene` ruft nur
`IncidentText.Length` und `_texts`-Lesen, `grep` ohne Treffer für `Write`. Die Szene bekommt aber die beschreibbare
Schnittstelle (`IncidentScene(…, ITextMemory texts)`). Vorschlag: lesende Schnittstelle für die Szene oder `ITextMemory.Write`
und `FormatNumber` als Schreiber in `check-architecture.py`, mit Gegenprobe.

**R6E-3 (P3) – Die Mehrrundenprobe hat nur Computerspieler.** `IncidentsMultiWeekTests` (Standardspiel, vier Computer) erfüllt
`AGENTS.md` §5 „Mensch und Computer“ nicht vollständig. Menschen sind in den Einzelszenarien abgedeckt (`takeover humans
keypad`, `loss human not immune`, Szene mit Keypad/Feuer). Die volle Probe mit einem Menschen bringt Spur 0 Teil 3 (Aufruf 9
im Port). Dort nachziehen, kein eigener Lauf.

**Bestätigt, kein Befund:**
- Kern/Szene-Trennung: `0A69:0288`/`0243`/`01AD` und `2295:0001` laufen im Kern richtig. Sie schreiben `DS:0140` und
  Textpuffer und verbrauchen Bilder (`060D`, `0245`, `0185(5)`). Die Szene zeichnet nur `0A96:0525` an der zentrierten
  Position und liest Kern und Text.
- 16-Bit-Geldgrenzen: Vermögen und Buchung laufen in 32 Bit. Die Mutationen `Wealth` auf 16 Bit (16 rot) und Buchung auf
  16 Bit (9 rot) werden gefangen. Die Klasse in `047E` vergleicht vorzeichenbehaftet in 32 Bit wie `17CB:0481`.
- Hinweis für Spur 0 bestätigt: `ProgramHost.TextMemory`
  ([`ProgramHost.cs:173`](https://github.com/kaaasi/traders-redone/blob/0d3da4a13868cfbed53aad39dceba0055db9e366/src/Traders.App/ProgramHost.cs#L173))
  ist ein `IFarMemory` und wirft bei `Write`. `WeekIncidentsStep` braucht `new TextBufferMemory(core, () => Start.TextBuffer)`,
  so wie `IncidentsMultiWeekTests` es verwendet.
- Erwartet offen bestätigt: Ton `21A7:0054` gibt es nur als `IncidentEventKind.Sound`. Die Szene ignoriert ihn
  (`IncidentScene.cs:104`), offen bis R10.

## 3. Ausgeführte Prüfungen

`tools/pruefen.sh` (alle drei Läufe, Head `0d3da4a`, dotnet 10.0.402): Hashes, Selbsttest fetch-reference, Architektur,
Abdeckung, Restore `--locked-mode`, Build Release mit Warnungen als Fehler, alle grün.

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| Mit Originaldaten (TR_REQUIRE_GAMEDATA=1) | 916 | 0 | 0 |
| Ohne Originaldaten (TR_SKIP_GAMEDATA=1) | 455 | 0 | 253 |
| Pflichtlauf ohne Daten (muss scheitern) | 455 | 461 | 0 |

Markerbericht: 395 `[ORIGINAL]`, 35 `[APPROX]`. Dieser PR bringt keine neuen `[APPROX]`.

**Gegenbeispiele:**
- #139: Probe aus `2026-11-14-r5-gesamt.md` → `{'A.Write': False, 'B.Go': True} ['Szene ruft schreibende Kernroutine B.Go']`
  (vorher `[]`).
- #76 P3-1 in temporärer Kopie: `ProjectReference` auf `Traders.App` in RefEmu, Viewer, Tools, Vga, Audio und Assets ergibt
  jeweils rc 1 „Rückrichtung auf Traders.App“. Viewer → Presentation ergibt „Szenen nur vom Host“, Core → RefEmu ergibt beide
  Meldungen. Unverändert: rc 0.

**Mutationen Architekturcheck:** Rückrichtung aus, Presentation-Regel aus, „Projekt fehlt“ aus und Feldscan aus scheitern
jeweils an der Selbstprüfung (rc 1). Nimmt man nur die Parameterauswertung (`_locals(parameters + ')')`) heraus, bleibt alles
grün (rc 0). Der Klassenscan der Felder erfasst die Signaturen schon mit, also ist die Parameterauswertung redundant, aber
nicht falsch.

**Mutationen Kern** (Filter `Traders.Tests.Events`, 24 Tests, Release, mit Daten):

| Mutation | rot |
|---|---:|
| A Gruppen vertauscht (`Money` vor `Takeover`) | 23 |
| B Vorzeichen der Buchung | 15 |
| C `random(9)` ausgelassen | 4 |
| D Vermögen 16 Bit | 16 |
| E Bargeld 16 Bit | 9 |
| I halbes Bargeld nicht halbiert | 2 |
| J `random(0)` bei Rang 3 ausgelassen | 3 |
| K MEAN `>= 3` → `> 3` | 1 |
| L Betrag nicht in den Text geschrieben | 15 |
| M Feldrang ohne Gleichstand | 2 |
| F/G/H/N Grenzen 200, 2000, 3EDh, 7 Felder | 0 (R6E-1) |

**Mutation Szene:** Wappen-x + 1 → 6 von 7 `EventsScene`-Tests rot. Der grüne Test ist „kein Ereignis“, dort wird kein Wappen
gezeichnet.

**T.EXE:** `tools/segmap.py`, Segment `17CB` vollständig gelesen (`0005`–`0A0D`), außerdem `2295:0001`/`0062`.
Übereinstimmend mit Befund und Port:
- Reihenfolge `04D7` → `001E` → `0736`.
- Bei Gewinn wird zuerst LXMUL/LDIV gerechnet, dann `random(9)`.
- Bei Verlust kommt `random(Rang·5+8)` vor der Lücke; `random(100)` nur für Rang ≠ 0; halbes Bargeld vor der
  MEAN-Prüfung.
- Übernahme: Text vor den Wappen.
- Ernteausfall: Wappen vor dem Text, `random(3 − Rang)`.
- `2295:0001` arbeitet mit Füllung `di − 1` und kopiert genau `di` Zeichen.

**Differenztests nachvollzogen:**
- `IncidentsHarness.RunOriginal` ruft `17CB:0005` im RefEmu auf. Gestubbt sind nur die Ausgabeschichten; die
  Schreibmaschine und `2295:0001` laufen echt.
- Der Test vergleicht ganzes DS, Aufrufe mit Bild und Argumenten, `random()` je Bild und die Bildzahl. Die
  `random(0)`-Uhrwerte kommen aus dem Original.
- Die Zweigabdeckung wird nur auf den Originalläufen geprüft.
- `EventsSceneHarness` vergleicht gegen den live laufenden RefEmu (VRAM, DAC, CRTC).

**Annahmen, nicht geprüft:** Windows-Lauf, Pixelvergleich außerhalb der 7 Szenenszenarien.
