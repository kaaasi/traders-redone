# 1B03:0009 – Stationen, Raketenflug, Piraten, Ertrag (Sammelbefund, Spur `stationen`, R6) – 1B03:0079, 1B03:00D4, 1B03:0101, 1B03:0153, 1B03:01C3, 1B03:0201, 1B03:02C0, 1B03:0413, 1B03:04D4, 1B03:0548, 1B03:05E7, 1B03:0632, 1B03:0705, 1B03:0740, 1B03:0763, 1B03:07C4, 1B03:0849, 1B03:08ED, 1B03:0A62, 1B03:0B7A, 1B03:0BE7, 1B03:0C89, 1B03:0CAC, 1B03:0CCF, 1B03:0D75, 1B03:0DC0, 1B03:0E11, 1B03:0EA9, 1B03:10EA, 1B03:1178, 1B03:1257, 1B03:128D, 1B03:12D7, 1B03:131B, 1B03:1345, 1B03:139D, 1B03:13C9, 1B03:13EE, 1B03:141D, 1B03:1454, 1B03:148B, 1B03:15D7, 1B03:1617

- **Ladebild-Adresse:** Segment `1B03` (Dateioffset `0x1E630`, 0x1650 Byte); `1617` läuft über das Segmentende (Rest bei `1C68:0000`:
  `add dx,−2; mov [bx+3322h],dx`).
- **Aufgabe:** Wochenschritt Stationen: Stationswerte würfeln, auf `V-RAKETE` je Spieler eine Station wählen, Raketen von den
  Raketenfeldern starten, Piraten greifen an, verlorene Raketen kosten Raketenfelder, danach Zufallsboni.
- **Aufgerufen von / Phase:** Wochenschleife `1777:01F8`, Aufruf 11. Kein Parameter, kein Rückgabewert.
- **Zufall:** `random(n)` liefert 0…n (`21FD_01BA_random.md`); `random(0)` setzt den Zustand aus `time()` und liefert 0.

## Ablauf (eigene Worte)

**`0009`:** `0079`; hat kein Spieler ein Raketenfeld (`DS:2EEA + 4p`, Art 3 von `DS:2EE7`), Ende. Sonst Bild `DS:08D0` laden,
`148B`, Ausblenden `21FD:037D(2)`, Sternfeld `1178`, Palette `21FD:026C(DS:0CD8)`, Einblenden `0329`, Wahl `00D4`, Flug `0705`,
Blitz aus `0CAC`, Felder `0E11`, Boni `0EA9`.

- `0079`: viermal `si = random(11)` (Station 11 wird so nie gezogen, 0…11 möglich – `random` schließt n ein): Wert
  `DS:348A[si]` = −1 → `random(2) ≠ 0 ? 10 : random(3)`, sonst → −1.
- `148B`: Objekt 4 (Explosion) Ebene `DS:3376` = 13, `21AF:022B(4, 0, 78h, 0, 85h)`, fünf Bilder `21AF:00CA`; je Spieler Ebene 15,
  Puffer, zwei Bilder, `21AF:0412(p, 0)`. `DS:37AA` danach 4.
- `1178` (**Pixel**): Liste, Fläche 320 × 200 aus dem Atlas, ausführen; 200-mal `x = random(319)`, `y = random(199)`,
  liest `23CD:06DE(x, y)`; nur bei Pixel 1 zusätzlich `random(2)` (Sternfarbe `DS:0DD9[…]`) und Punkt `0A96:03FB`. Die Zahl der
  Ziehungen hängt also am Pixel der Zeichenseite (Regelseite R6P-1). Danach die zwölf Stationsbilder (`DS:0DCD`, `0C84`, `0C9C`),
  ausführen, `0A96:0C3F`.
- `00D4` (**Stationswahl**): `0101`, `0153`, `01C3`, `0201`, Liste leeren, `02C0`, `15D7`, `13EE`.
- `0101`: `random(6) = 0` → drei getippte Zeilen ab Text 192 (`0A69:0288`), **neue Stationen `0E31:023D`** (portiert,
  `NewGame.CreateStations`), Pause 50, `13EE`.
- `0153`: Marke (`DS:0E3E`) neben jeder Station mit Wert ≠ −1: Farbe 5 bei Wert 10, sonst `DS:0D00[Wert]`.
- `01C3`: Piratenstärke `DS:0900` ≥ 1 und `random(2) = 0` → Piratenort `DS:546E = random(Stärke − 1)`, Schiff `1257`; sonst −1.
  Bei Stärke 1 ist das `random(0)`: der Zufallszustand wird aus der Uhr neu gesetzt (Uhr = Eingabe des Kerns).
- `0201`: `DS:5428[0…11]` = 0; je Spieler Bewegungsverzug `DS:3786` = 3p (long), Zeiger `DS:4888[p]` = gehaltene Station
  `DS:450E[p]` (Station belegt = 1, Bild 1) bzw. 2p (Bild 0), Objekt `1617`, Zeile `0548`.
- `02C0`: lokale Merker je Spieler, Zähler 4; je Durchgang Seitenwechsel, `23CD:071C`, Liste, `12D7(10)`, Pause `21FD:0245`,
  `05E7`, **Steuerung `0E9E:000E`** (portiert); je wählender Spieler links `0413(−1)`, rechts `0413(1)`, Feuer: ohne Station
  `04D4`, mit Station bestätigt (`DS:5428` = 2, Marke `DS:0E40` in `DS:0CF8[p]`, Merker aus, Zähler − 1). Ende `21FD:0665`.
- `0413(d)`: gehaltene Station wird frei (−1, Bild 0); Zeiger + d mit Umlauf 11 → 0, 0 → 11; `1617`; Ton 10h; `0548`;
  Verzug Computer `43 − 10·IQ`, Mensch 20.
- `04D4`: Station unter dem Zeiger frei → halten (1), Bild 1, Ton 11h, `0548`; Mensch: Feuerverzug `DS:302E` = 10000.
- `0548`: `141D`, Stationsname (`DS:0CA8[Zeiger]`) bei x 89h; besucht (`DS:2F1A`) → Artname (`DS:0D08[Art]`) bei x 71h.
- `05E7`/`0632`: Computer feuert, wenn der Zeiger auf der besten Station steht, sonst rechts (`DS:37C3` = 10h/01h).
  Bewertung ab dem Zeiger, zwölf Stationen mit Umlauf: bestätigte überspringen, gehaltene fremde überspringen (außer Wert = p
  oder eigene); unbesucht 3, Art 0 → 1, Art k → 2k + 3; Wert −1 + 10, Wert p + 9, Wert eines anderen Computers negiert; erste
  mit höchstem Wert > −1000. Der Startwert des Index ist ein uninitialisiertes Stapelwort (wird im Spiel stets gesetzt: höchstens
  vier Stationen gehalten).
- `1617`: Objekt p bei `DS:0C84[Zeiger]`, `DS:0C9C[Zeiger] − 2`. `15D7`: Objekte 0…3 aus, beide Seiten neu.
- `0705` (**Raketenflug**): `0740`, `0763`, `07C4`, `0849`, Liste, `08ED`, Pause 100, `13EE`, `0DC0`.
- `0740`: `DS:53B4`, `53B2`, `5338`, `53B3`, `546F` = 0, `53B5` = −1.
- `0763`: Wert der eigenen Station (`DS:348A[2·DS:450E[p]]`, bei −1 also `DS:3488`) ≠ −1 und ≠ p → Meldung Text 195 (4 Zeilen),
  `DS:546A[p]` = 1, sonst 0.
- `07C4`: je Spieler gestartet `DS:53A6` und verloren `DS:53AD` = 0, Raketen summiert nach `DS:53B2` (Byte), Text 225, `0D75`.
- `0849`: Startspieler `random(3)` (Spieler 3 nie zuerst), reihum je Rakete k: Fortschritt `DS:53B6[k]` = −5k, Ziel
  `DS:533A[k]` = `random(299) + 10`, Besitzer `DS:5434[k]`; `DS:53B4` zählt fliegende.
- `08ED` (**Pixel**): solange `DS:53B4` > 0 je Bild: Seitenwechsel, Pause 3, Objekte, Liste, Pause; je Rakete: über 59h
  fertig; negativ: hochzählen; sonst (bei 0 Startton 12h) `0CCF`, `0B7A`, angegriffene → `0BE7`, sonst bei Pixel 1 an
  (`DS:5424`, `DS:5426`) Punkt in `DS:0CF8[p]`; Fortschritt + 1, über 59h → `DS:53B4` − 1. Dann `12D7(3)`, `0A62`, `0C89`, Liste,
  Objekte. Am Ende Ton 0, `21FD:0665`.
- `0CCF(z, t)`: Start x = Station x + 8, y = Station y + 7; x = Start + `DS:0D17[t]`·10 / (10000 / (z − Start)) (z = Start → z − 1),
  mindestens 0; y = C7h − `DS:0D17[59h − t]`·10 / (10000 / (C7h − Start y)); 16-Bit-Produkte, `idiv`.
- `0A62`: Zustand `DS:5338` = 0: mit Piraten und ohne Angriff Start mit Chance 1/`DS:0D14[Ort]`. Sonst + 1, Sprungtabelle
  `1B03:0B5A` (Index Zustand − 2): 2 Lichter 5 und Schiff an Platz 1; 8 Platz 0; 11 `random(10) = 0` → Wrack (`128D(1)`,
  `131B(4)`, `13C9`), Blitz 7, Ton (2, 13h), keine Piraten mehr, Zustand 0, sonst Kanone (`139D(1)`, Ton (1, 14h)); 14 Kanone
  zurück; 17 bereit `DS:53B3` = 1, Zustand 0.
- `0B7A`: Angriff auf Rakete `DS:53AC`, wenn (`546A[p]` = 1 oder ohne Piraten `random(5000) = 0`) und kein Angriff läuft und das
  Schiff wartet, oder mit Piraten nach „bereit“; nur bis y BEh: `DS:5422` = 0, `DS:53B5` = k, Ton (2, 15h), bereit = 0.
- `0BE7` (**Pixel**): erstes Bild: Blitz 7; Pixel an der Rakete = 1 → sichtbar `DS:5336` = 1, Objekt 4 an
  (x − 8, y − 6); bis Bild 14 bei sichtbar Objektbild `DS:5422 / 3`, sonst Objekt 4 aus, Rakete verloren (`53B6[k]` = 5Ah,
  `53B4` − 1, `53AD[p]` + 1, `0D75`), kein Angriff mehr. `DS:5422` + 1.
- `0C89`: `DS:546F` > 0 → − 1, `21FD:03EB(777h, Wert)`. `0CAC`: zehnmal `0C89`, Pause 4.
- `0DC0`: mehr als drei verloren und alle Raketenfelder verloren → Meldung Text 199 (7 Zeilen).
- `0E11` (**Ertrag**): Felder i = 0 … Spalten·Zeilen − 1, x = i mod Spalten, y = i / Spalten (`DS:2EB0`, `DS:2F00`), Besitzer nach
  `DS:37AA`; Raketenfeld (Art 3) eines Spielers mit verlorenen Raketen → `0609:10D2(x, y, Besitzer, −2)`, Zähler − 1.
- `0EA9` (**Boni**), je Spieler mit Raketenfeldern, Rang r (`DS:3D42`): `random(22 − 4r) = 5` und Edelsteinhandel → Edelsteinkonto
  + `random(2·DS:4436) + 2` (höchstens 2000, Texte 210…213, Zahl in 212); `random(22 − 4r) = 0` und Ölverbrauch → Öl +
  `random(20) + 2` (höchstens 40, Texte 206…209); `random(49 − 6r) = 0` und r > 0 → Geld + (unter 1000: `random(30) + 100`, sonst
  Geld / (10 − r), LDIV `0000:4F89`), Texte 214…224 (Zahl ab dem dritten Zeichen von Text 223). Zahlen über `2295:0001` in den Text.
- `10EA(Tabelle, n)`: getippte Zeilen (`0A69:0243`) in der Spielerfarbe, je vier eine Box (Pause 20, `13EE`), mindestens eine
  Zeile; danach Pause 50, `13EE`.
- Zeichenroutinen ohne Zustand: `0153`, `0548`, `0D75`, `131B`, `1257`, `128D`, `139D`, `13C9`, `13EE`, `141D`, `1454`; mit Zustand
  `1345(n)` (`DS:53B1` = n, `DS:53AA` = 1000) und `12D7(n)` (Zähler `DS:53AA`, Lichtbild `DS:5337` 0…3; nicht bei −1 Piraten oder
  `53B1` = 1).

## Zustand

Neue Adressen: `docs/analyse/DATENLAYOUT.md` Abschnitt „Spur stationen“, im Code `StationsLayout`. Gelesen außerdem: `DS:0840`,
`083C`, `0900`, `2E8E`, `08FC`/`08FE`, `2EE7` (Art 3), `2F1A`, `348A`, `44F4`, `450E`, `4442`, `2EB1`, `3D42`, `4BF2`, `45BA`,
`4436`, `300E`, `4520`, `3CD2`; geschrieben `348A`, `450E`, `2F1A`/`348A`/`44F4` (über `0E31:023D`), `3786`, `302E`, `37C3`,
Objektsätze `DS:330C` (+2, +14h, +16h, +18h, +0), Felder über `0609:10D2`, `300E`, `4520`, `3CD2`, `37AA`, `2EB0`, `2F00`.

## Zufall in Reihenfolge

`0079` 4 × (11, ggf. 2, ggf. 3) · `1178` 200 × (319, 199, bei Pixel 1: 2) · `0101` 6 (ggf. `0E31:023D`) · `01C3` ggf. 2,
Stärke − 1 · `05E7`/`0632` keiner · `0849` 3, je Rakete 299 · `08ED` je Bild: `0B7A` je fliegende Rakete ggf. 5000, `0A62` ggf.
`DS:0D14[Ort]`, Zustand 11: 10 · `0EA9` je Spieler mit Raketen 22 − 4r, ggf. 2·Faktor, 22 − 4r, ggf. 20, 49 − 6r, ggf. 30.

## Kern und Zeichenteil

Kern (`src/Traders.Core/Stations/`): alle Zustands- und Zufallsteile; jede Ausgabe als `StationEvent` mit den
Originalargumenten (Liste, Text, Objekte, Seiten, Palette, Pausen, Ton). `1178`, `08ED`, `0BE7` lesen die Zeichenseite über die
Regelseite (`Pixels/RulePage.cs`, PR #149; Ergänzung `Pixels/StationsPage.cs`). Teil 3: Szene (Abschnitt „Szene“).

**Transitiv gerufen:** portiert `0E31:023D`, `0E9E:000E`, `0609:10D2`, `21FD:01BA`, LDIV `0000:4F89`, `21AF:0412`/`045E` (Kernteil);
Ausgabe (Ereignis, in `Traders.Vga` vorhanden) `0A96:001B/0030/0311/03FB/0525/06A8/0C3F`, `0A69:004D/011B/0243/0288`,
`23CD:06E9/071C`, `21AF:00CA/022B`, `0B67:01D4/0206`, `21FD:0185/026C/0329/037D/03EB/051F/060D/0665`, `2295:0001`, `186C:0002`;
Pause `21FD:0245` (Ereignis); Ton `21A7:0054` (Ereignis bis R10); Pixel-Leser `23CD:06DE` → Regelseite. Keiner gehört
einer anderen Spur von R6.

## Regelseite (was die Lesestellen sehen)

Alle drei Leser vergleichen das Pixel (AX, `23CD:3764` löscht AH) mit 1. Was vorher auf die Zeichenseite kommt (belegt durch den
Gleichschritt unten, Seiten je Bild gleich dem VRAM außerhalb der Meldetafel):

- **Befehlsliste `0A96`:** Einträge werden erst bei `06A8` gezeichnet, auch in `21FD:060D` (Liste, Wechsel, Liste) und `0665`
  (Wechsel, Objekte zurück, Liste, Objekte); `001B` leert. Gezeichnet werden Blits aus dem RAM-Atlas (`0030`, `0311` = Breite 8),
  Pixel (`03FB`: Sterne, Raketenspur) und Texte aus DS ohne Schatten (`0525`: Marke `P` neben Stationen, `OK` links oben) mit der
  aktuellen Schrift; Glyphenpixel laufen im Seitenspeicher weiter (x ≥ 320 → nächste Zeile, Station 9).
- **`1178`:** die Lesestelle sieht das Bild und die schon gesetzten Sterne (jeder Stern wird sofort ausgeführt).
- **`08ED`/`0BE7`:** gelesen wird nach `071C` und `06A8` der Liste des Vorbilds, Punkte desselben Bildes sind noch nicht
  gezeichnet. Objekte sind beim Lesen im Normalfall zurückgesetzt; **aber** `21AF:0412` löscht die Rücksetzsätze beider Seiten:
  wird die Explosion (Objekt 4) für eine neue Rakete gezeigt, bleibt ihr altes Bild auf der Seite, auf der es zuletzt gezeichnet
  war, und ein späteres Lesen dort trifft es (Rakete dann „unsichtbar“ getroffen und sofort verloren). Die Regelseite zeichnet und
  setzt Objekte deshalb wie `23CD:06E9/071C` (16 Pixel breit, Maske aus `MASKS.PP`, Sätze je physischer Seite).
- **Seitenwechsel:** `0A69:0243`/`0288` (getippte Texte) rufen je Zeichen `21FD:060D` – die Zahl der Wechsel ist die Textlänge
  (Textpuffer `DS:4884` als Eingabe). Zeichenseite danach wie `21FD:051F`; `0B67:01D4/0206` in `15D7` nur für das Zurücksetzen.
- **Nicht gezeichnet (`[APPROX]`):** Texte der Meldetafel `0A69:004D/011B/0243/0288` (Tafel 70h…CFh × AFh…C7h ohne Farbe 1 in
  `V-RAKETE`, Textfarben ≠ 1): ein Lesen dort bleibt ≠ 1, der genaue Wert kann abweichen. Gilt für die kleine Schrift (Schrift 0,
  im Spiel an Aufruf 11 gewählt); mit der großen reichen Tafeltexte über die Tafel hinaus.
- **Annahmen beim Start:** kein Objekt hat einen gültigen Rücksetzsatz, Objekte 5…7 inaktiv (im Test am Original geprüft).
  `21AF:022B` legt die Sicherungspuffer in den RAM-Atlas (x 0…15, Zeilen 0…93h); in der Phase wird von dort nichts mehr geblittet,
  der Atlas der Regelseite bleibt das Bild.

## Szene (Teil 3)

Die Zeichenteile aller 44 Routinen sind Ausgabeaufrufe; die Szene `src/Traders.Presentation/Stations/StationScene.cs` spielt jedes
`StationEvent` über `Traders.Vga.Layers` ab (Liste `0A96`, Schatten- und Tipptexte `0A69`, Zahl im Text `2295:0001` in den
Textpuffer, Objekte `21AF`/`23CD`, Seiten `0B67`, `21FD:0185/026C/0329/037D/03EB/051F/060D/0665`, Bildlader `186C:0002` wie
`FieldScene`). Sie bekommt jedes Ereignis im Augenblick des Aufrufs (`StationPhase.Output`) und liest dann die Objektfelder
(`+0/+2/+14h/+16h/+18h`) und die Tastentabelle aus dem DS des Kerns; das DS des Kerns schreibt sie nie.

- **Seitenindex `DS:082E/0830`:** `1B03` übergibt `[082E]` an `23CD:06E9/071C` und `0A96:0C3F`, `15D7` auch `[0830]`. Beide
  gehören der Ausgabe (`21FD:051F`); der Kern führt sie deshalb in `StationsPage` (`DrawIndex`/`ShownIndex`, je Wechsel wie
  `051F`, auch in `060D`, `0665` und den Tipptexten) und übergibt diese Werte. Vorher stand dort der unveränderte DS-Wert (falsche
  Seite beim Zurücksetzen, im Bildvergleich rot; die Regelseite war nicht betroffen, sie nutzt ihre eigene Zeichenseite).
- **Bildtakt (R6S-1):** Der Kern hält bei jedem Rücklauf des Originals (`0D73:00D3`): `21FD:0185(n)` n-mal, Seitenwechsel
  `051F` (auch in `060D`, `0665` und je getipptem Zeichen `0A69:01AD`) einmal plus einmal ohne `Num+` (`DS:3D95`) plus viermal
  mit `Num−` (`DS:3D91`), Blenden `037D`/`0329` 8 · (4 bzw. 3 + 1 auf VGA), Palettenschritt `03EB` einmal auf VGA (`2295:00C5`);
  getippte Texte je Zeichen Wechsel, Pause `0245`, `0185(wait)`, danach `0185(Endwartezeit)` wenn > 0 (Länge per `strlen` aus dem
  Textpuffer). Die Pause `0245` hält je Abfrage einmal (`FieldWait.Poll`), solange P gedrückt, losgelassen, erneut gedrückt ist.
  Die Szene spielt jede Warteroutine schrittweise (`StationScene.NextFrame` vor dem Takt des Kerns, wie `DisasterScene`) und
  bekommt je Takt nur die Ereignisse dieses Bildes; sie hält bei denselben Rückläufen. Ton `21A7:0054` geht an
  `StationScene.Sound`, der Tastenklick der Tipptexte an `GraphicsLayers.Effects`.
- **Handler Aufruf 11:** `StationsStep` (`src/Traders.Core/Stations/StationsStep.cs`, nicht eingebunden): `Begin` bis zum ersten
  Halt, je `Tick` ein Bild (Zähler `Retraces`, `Polls`); fertig im Bild, in dem `0009` zurückkehrt. Bildladen in der Bildtabelle
  (`ImageTable.RecordLoad`) wie `PlantingStep`; Regelseite leer beim Start (`1178` überdeckt die Zeichenseite ganz und kopiert sie
  vor dem ersten Lesen). `DS:082E/0830/40DE` müssen beim Start die der Ausgabe sein (Einbindung Spur `ernte`).

## C#-Umsetzung

`StationPhase` (`StationPhase*.cs`): `Run` (`0009`) und `Flight` (`0705`) als Koroutinen (Halt je Bild des Originals, siehe
„Bildtakt“; `Flips` zählt die Seitenwechsel von `1B03`, die Eingabe des nächsten Durchgangs gilt ab dem Bild danach), darin `Begin`/`StarField`/`ShowScreen`, `Selection`/`Choose`,
`FlightBegin`/`FlightLoop`/`FlightEnd`, `End`; je Routine eine Methode mit Marke. `StationPhase.Pixels` (`StationsPage`) spielt
jedes Ereignis auf die Regelseite; Eingaben: Regelseite (Zeichenseite wie `DS:40DE[DS:082E]`), Bild `V-RAKETE`, `MASKS.PP`,
Schrift (`TextFont`), Textpuffer (`TextFile`).

## Prüfung

`tests/Traders.Tests/Stations/StationsDifferentialTests.cs` mit `StationsHarness` (`LoadOnly`, Ausgaberoutinen als `retf`
protokolliert, `21AF:0412/045E` laufen): zufällige Zustände je Routine; verglichen ganzes DS (ohne Objektzeiger/Restaurierung
und tzset-Namen), Zahl der `random`-Aufrufe, AX, Aufruffolge mit Argumenten, Tastendurchgänge. `random(0)`: die Uhr des Ports
bekommt den `time()`-Wert des Originals (Haken bei `21FD:01CE`).

`tests/Traders.Tests/Stations/StationsLockstepTests.cs` mit `StationsLockstep` (Original mit echter Ausgabe aus dem Abzug am
Hauptmenü bzw. aus der Standardpartie, nur Ton `21A7:0054` und Wartezeit `21FD:0185` als `retf`): je Bild (`051F` aus `1B03`) DS
ohne Ausgabebereiche (Befehlsliste, Seiten, Objekttabellen `21AF`, Palettenpuffer, Zeitgeber), Zahl der `random`-Aufrufe,
Zeichenseite und beide Seiten außerhalb der Tafel; jedes Lesen von `06DE` (in der Tafel nur „= 1“); Bildzahl und Endzustand.
`Star_field_1178` (8 Zustände, Seiten danach vollständig gleich), `Whole_phase_0009` (16 Zufallszustände mit Menschen und
Computern, 6184 Bilder), `Game_week` (Standardpartie Woche 0, 1, 2). Gegenproben beim Entwickeln: ohne Objekte auf der
Regelseite rot (Zustand 15, Explosionsrest), ohne Wechsel der getippten Texte rot (Seiten vertauscht), große Schrift rot.

`tests/Traders.Tests/Stations/StationsSceneTests.cs` mit `StationsSceneHarness` (Teil 3; Original mit ganzer Ausgabe einschließlich
der Rückläufe, nur Ton als `retf`): je Bild (Eintritt `0D73:00D3`) die vier VRAM-Ebenen, DAC, CRTC-Start und die Befehlsliste
(`DS:15BE` Zahl und Einträge) gleich dem Port (Kern mit Regelseite, `StationScene` auf Grafikschichten aus demselben Zustand, Takte
über die Rückläufe der Schichten); Bildzahl und Endzustand. `Scene_with_humans` (12 Zufallszustände, Menschen und Computer, jeder
zweite mit Piraten; alle 31 Aufrufarten gespielt, 22 703 Bilder), `Scene_game_week_0` (Standardpartie, 1268 Bilder),
`Step_handler_call_11` (4 Zustände und Standardpartie Woche 0 über `StationsStep` in einer `PhaseMachine` ab Aufruf 11, Halt an
Aufruf 12; je Zustand Zahl der Takte = Rückläufe des Originals, Woche 0: 1267 Takte, 1268 Bilder mit dem Schlussbild; vor R6S-1
rot mit 291 Takten gegen 1453 Rückläufe). Gegenproben beim Entwickeln: eine Pause zu kurz, Schatten- und Textfarbe vertauscht,
Objekte ohne Spiegelung der Kernfelder, `0185` im Kern ein Rücklauf mehr bzw. weniger – jeweils rot.
