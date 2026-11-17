# 08CF:0004 / 08CF:0082 / 08CF:00CD / 08CF:01BD / 08CF:029A / 08CF:039C / 08CF:03E5 / 08CF:0455 / 08CF:0492 / 08CF:04BA / 08CF:05F1 / 08CF:066D / 08CF:069F / 08CF:0813 / 08CF:084F / 08CF:0886 / 08CF:0914 / 08CF:09C4 / 08CF:09F1 / 08CF:0AF0 / 08CF:0BE1 / 08CF:0CD5 / 08CF:0DEF / 08CF:0E53 / 08CF:0F4D / 08CF:115C / 08CF:1348 / 08CF:1543 / 08CF:15C6 / 08CF:15F5 / 08CF:161F / 08CF:1688 / 08CF:16B4 / 08CF:16F5 / 08CF:172A / 08CF:178D / 08CF:1859 / 08CF:18B5 / 08CF:1935 / 08CF:193E / 08CF:1981 – Einkauf

- **Ladebild-Adresse:** Segment `08CF` (Dateioffset `0xC2F0`, Größe `0x19A0`), `08CF:0004` bei `0xC2F4`; `1981` endet in
  `0A69:0000`–`000A`. Sprungtabellen: `08CF:0290` (5 Einträge, Modus − 0Ah), `08CF:03DD` (4, Laden), `08CF:07FD` (11, Aktion).
- **Aufgabe:** Einkauf der Woche (Aufruf 29 der Wochenschleife `1777:01F8`) auf `V-ANBAU`: jeder Spieler geht eine Straße mit
  bis zu vier Läden entlang – Lotto, Öl (nur mit OIL CONSUMPTION `45BA`), Raketen, Edelsteine (nur mit GEM TRADING `4BF2`) –,
  betritt einen mit „hoch“ und kauft dort, bis er mit „runter“ geht. Nebenher läuft die Lottoziehung. Ein Countdown begrenzt
  die Phase.
- **Aufgerufen von / Phase:** `08CF:0004` nur von `1777:01F8` (Aufruf 29); alle übrigen Routinen nur aus `08CF` (Aufrufgraph
  `tools/callgraph.py` auf `T.EXE`, Prüfung Teil 1: genau die 41 Routinen der Planliste, `11D7` gehört nicht dazu).
- **Parameter / Rückgabe:** `0455(Ziel)`, `0886(Richtung)`, `0E53(Zahl)`, `178D(Richtung)` (Byte), `1543(Text fern,
  Wert lang, Stellen)`, `15C6(Laden, x)`, `15F5(x, y, Kugel)`, `16B4(Platz, Zahl)`; `05F1`, `0813`, `084F`, `0AF0`, `0DEF`
  liefern AL (0/1), `0914` AX (0 … 3).

## Ablauf (eigene Worte)

- **`0004`:** Bild `[08CE]` laden (`186C:0002`), `0609:1F2D` (Objekte), `0082`, Countdown `21FD:04E5([08F8], 28h)` (Zeitlimit
  10/20/30, 40 Takte je Schritt; `Rules/Countdown`), `21FD:037D(0)`, `1348`, Ränge `0825:06CB`, `00CD`, `21FD:026C(DS:00A8)`,
  `0609:11E7`, `21FD:0329`, `0A96:001B`, `01BD`; danach je Spieler `0609:21BA`, zuletzt `21AF:000D`.
- **`0082`:** x des Lottoladens `15A0` = 60h − 20h · (`45BA` + `4BF2`), dann in Schritten von 40h `15B0` (nur bei `45BA` = 1),
  `15B8`, `15B2` (16 Bit).
- **`00CD`:** je Spieler `0A96:001B`; Figur x `3320[1Ah·p]` = `15A0` + `00E4[p]`, y `3322[1Ah·p]` = 3p + 79h; Wappen
  `0609:1879(40h·p + 20h, 6, Rang)`, Ölbalken `0609:1AC0`; `43B2` = 0Ah, `15AB` = 0, `4BD2` = `3022` = 0, `3786` = 0 (lang),
  `1586` = `159C` = 0; `0609:1FCC`, `21FD:060D`.
- **`01BD`** (je Bild, solange `3780` ≠ 0): `21FD:051F`, `23CD:071C([082E])`, `0A96:06A8`, `0A96:001B`, `21FD:0185(2)`,
  `0E9E:037A`; `15B4` − 1, unter 0 → 5; je Spieler nach `43B2` − 0Ah: 0 `029A`, 1 `09F1`, 2 `04BA`, 3 `0CD5`, 4 `069F`
  (andere Modi nichts); `115C` bei `15A2` = 3; `0F4D`; `21FD:04F9` = 1 → `1935` (Uhr); `0A96:06A8`, `23CD:06E9([082E])`. Danach
  `21FD:0665`.
- **`029A`** (Straße): läuft ein Gang (`3022` ≠ 0) → `0609:1FE0`. Sonst Computer → `03E5`; Aktion 0: nächster Laden rechts
  (Öl übersprungen ohne `45BA`, Edelsteine ohne `4BF2`), 1: links (Öl übersprungen), jeweils nur innerhalb 0 … 3 (8-Bit-Zähler
  mit Vorzeichen), dann `15AB`, Richtung `4BCA` = 0/1, `3022` = 21h; 2: `039C`.
- **`039C`:** nach `15AB`: 0 `0BE1` nur bei `15A2` = FFh, 1 `0492`, 2 `09C4`, 3 `066D`.
- **`03E5`** (Computer in der Straße): `0DEF` = 1 und `1586` < 4 → `0455(0)`; sonst `05F1` → `0455(1)`; `0AF0` → `0455(2)`;
  `0914` ≠ 0 → `0455(3)`; sonst Aktion FFh.
- **`0455(Ziel)`:** Aktion = 0 (Laden < Ziel), 1 (>), 2 (=).
- **`0492` / `09C4` / `066D` / `0BE1`:** Modus 0Ch / 0Bh / 0Eh / 0Dh, `0609:18E5`, `193E`, Ton (0, 50h); `09C4` zeigt die
  Raketen (`0609:1978`), `066D` Edelsteine (`172A`) und Konto (`1859`) vor dem Ton. `0BE1` nach dem Ton: `1586` = 0, `16F5`;
  Bargeld ≥ Lospreis `40CC` → Text 18 und vier leere Zahlen (`158A` = 0, `16B4(i, 0)`), sonst Texte 16 und 17.
- **`04BA`** (Öl): Computer → ohne Ölwunsch (`05F1`) Aktion 3, sonst bei `3786` ≤ 0 (32 Bit) Aktion 0. Aktion 0 oder Ah:
  Bargeld ≥ Ölpreis `43F4`, Öl < 28h, `4B98` ≠ 0 → `0609:1B72(1)` (Öl + 1, Balken), `4B98` − 1, `1981`, Bargeld − Preis, Ton
  (0, 52h), `193E`, `3786` = 3 (auch beim Computer). Aktion 3: Wappen mit Rang, Modus 0Ah, Ton (0, 51h).
- **`05F1`:** 1, wenn `45BA` ≠ 0, Öl < 28h, `4B98` ≠ 0 und Bargeld / t ≥ Ölpreis (LDIV, 32 Bit) mit t = 100 bei Öl < 10, sonst
  10 · Öl + 200.
- **`069F`** (Edelsteine): Computer bei `15B4` = 0 → `0914`: 1 → Aktion 1, 2 → 0, 3 → Ah, sonst 3. Aktion 0: `084F` →
  `0886(1)`, `159C` + 1; 1: `0813` → `0886(−1)`, `159C` + 1; Ah: mit Edelstein `178D(−1)`, Bargeld + `4436` · `3CF0` / 100
  (LXMUL, LDIV), `193E`, Ton (0, 52h); 3: verlassen wie bei `04BA`.
- **`0813`:** 1, wenn Konto `300E` (cwd) + `4436` ≤ 2000 (32 Bit mit Vorzeichen) und ein Edelstein da ist.
- **`084F`:** 1, wenn weniger als 10 Edelsteine und Konto ≥ `4436` (32 Bit).
- **`0886(r)`:** `178D(r)`, Konto − (Unterwort von r · `4436`), `1859`, Ton (0, 2Bh) bei r = 1, sonst (0, 2Ch); Mensch: `3786` =
  64h.
- **`0914`:** s = `4436` − (Woche + 100) (16 Bit). Ohne `4BF2` → 0. s > 0: mehr als 5 Edelsteine und 75 − 5 · Edelsteine ≤ s
  → 3; sonst `0813` und `159C` < (s − 10) / 4 → 1; sonst 0. s < 0: `084F` und `159C` < −(s + 10) / 4 → 2; sonst 0 (idiv).
- **`09F1`** (Raketen): Computer bei `15B4` = 0 → Aktion Ah, wenn `0AF0`, sonst 3. Ah: Bargeld ≥ `48A0` und Raketen ≠ 10 →
  Raketen + 1, `0609:1978`, Bargeld − Preis, `193E`, Ton (0, 52h). 3: verlassen.
- **`0AF0`:** 0, wenn Raketen ≥ `3046[p]`. Grenze: bei `4BC2` = 1 oder Raketen + Raketenbasen (`2EE7[4p + 3]`) < 3 Bargeld / 10
  (Rang ≥ 2) bzw. / 30, sonst / 50; 1, wenn Preis (cwd) ≤ Grenze **vorzeichenlos** 32 Bit.
- **`0CD5`** (Lotto): k = `1586` > 3 → Aktion 3. Computer bei `15B4` = 0: Zahl = `08ED[k]` (≠ FFh) oder `random(1)`; Aktion 1
  bei Zahl 1, sonst 0; `0DEF` = 0 → 3. `15A2` ≠ FFh → Aktion 3 (für alle). Aktion 0 → `0E53(2)`, 1 → `0E53(1)`, 3 → `16F5`,
  Wappen, Modus 0Ah, Ton (0, 51h).
- **`0DEF`:** 0 bei Bargeld < `40CC`; 1 bei `08EE` ≠ FFh; sonst 1, wenn Bargeld / 5 ≥ `40CC`.
- **`0E53(z)`:** nur bei Bargeld ≥ `40CC`: `158A[4p + k]` = z, `16B4(k, z)`, Ton (0, 2Dh) bei z = 1 sonst (0, 2Eh), Mensch
  `3786` = 12Ch, `1586` + 1; über 3: Bargeld − `40CC`, Wappen, Modus 0Ah (ohne Ton).
- **`0F4D`** (Ziehung, je Bild): bei `15A2` = FFh Zeit `3780` − `159A` in `00E8`, bei Änderung gezeigt (`0A96:057A`). Weiter nur
  bei `3780` ≤ `159A` und `15A2` ≠ F6h: `15A2` + 1; über 3 → `161F`, je Spieler `16F5`, `1586` = 0. Sonst Kugel k = `08ED[k]`
  (danach FFh) oder `random(1)` + 1 nach `15A7[k]`, gezeichnet, Ton (1, 2Fh) bzw. (1, 30h), `159A` = `3780` − 1. Nach der vierten
  je Spieler mit vier Zahlen Treffer `15A3`; vier Treffer → Bargeld + `34A2`, `193E` (im Laden) bzw. Wappen ohne Rang (Straße);
  `0825:06CB`, `15AF` = 0, `159A` = `3780` − 2.
- **`115C`** (bei `15A2` = 3): `00EA` − 1; bei 0 blinken die Zahlen der Gewinner (Schritt 0 … 3 leer, 4 … 7 Kugel `15A3[Schritt]`
  = `15A7[Schritt − 4]`), `15AF` + 1 bis 8 → 0, `00EA` = 2.
- **`161F`:** `1688`, `159A` = `3780` − 5; < 5 → Text DS:0136, `15A2` = F6h; sonst Gewinn `34A2` (`0A96:05FB`), `15A2` = FFh.
- **`1348`:** `0A96:001B`, `0609:1ED9`, `0609:1256(0, 0, 140h, 4Ch)`, `0609:13C5(20h, 50h, 100h, 6Eh)` (16 × `random(1)`),
  Regale `0A96:0267`; 2 × 8 Plätze: `random(1)` = 0 → Kugel `random(2)` (`15F5`); Läden `15C6`, Ölvorrat `1981`, Kurs `4436`,
  Uhr `18B5`, `161F`; Preiszeile ab `15B6` = 58h (− 1Dh mit Öl, − 2Ch mit Edelsteinen): Texte 22, 21, 20, 19 mit `40CC`, `43F4`,
  `48A0`, `4436` · `3CF0` / 100 (`1543`); `0A96:06A8`, `0A96:0C3F([082E])`.
- **`1543`:** `0A69:004D` bei `15B6`, `15B6` + 6 · strlen (`0000:42E5`), Zahl `0A69:0164`, `15B6` + 6 · (Stellen + 1) + 0Bh.
- **`178D(r)`:** Edelsteine + r (Byte); r = 1 zeichnet einen Edelstein an Platz n − 1, sonst wird Platz n gelöscht (`0A96:04D7`;
  fünf je Reihe, 8 Pixel Abstand).
- **Nur Zeichnen** (Teil 3): `15C6`, `15F5`, `1688`, `16B4`, `16F5`, `172A`, `1859`, `18B5` (Zeiger mit `3780` ≤ 30 aus den
  Tabellen DS:00F4/0113), `1935` (= `18B5`), `193E` (Bargeld, Farbe DS:0132[p]), `1981` (Ölvorrat).

### Zufall in Reihenfolge

`1348`: 16 × `random(1)` in `0609:13C5`, dann je Platz `random(1)` und bei 0 `random(2)`. Je Bild: `0CD5` (je Computer am
Lotto bei `15B4` = 0 ohne Vorgabe) `random(1)` in der Spielerschleife, danach `0F4D` `random(1)` je Kugel ohne Vorgabe. Sonst
kein Zufall (auch nicht in den Computerentscheidungen).

### Kern und Zeichenteil

| Routine | Kern (`PurchaseRules`) | Zeichenteil |
|---|---|---|
| `0082`, `03E5`, `0455`, `05F1`, `0813`, `084F`, `0914`, `0AF0`, `0DEF` | ganz | – |
| `029A`, `039C`, `0492`, `04BA`, `066D`, `069F`, `0886`, `09C4`, `09F1`, `0BE1`, `0CD5`, `0E53`, `0F4D`, `115C`, `161F`, `178D` | Regeln, Buchungen, Zufall | Ton und Zeichnen als Ereignis |
| `1348`, `1543` | Zufall, `15B6`, `159A`, `15A2` | Ereignisse |
| `0004`, `00CD`, `01BD` | ganz, Bild für Bild (`Purchase`) | Ereignisse (Teil 3 zeichnet) |
| `15C6`, `15F5`, `1688`, `16B4`, `16F5`, `172A`, `1859`, `18B5`, `1935`, `193E`, `1981` | – | Teil 3 (`PurchaseScene`) |

Gerufen, nicht nachgebaut: `0609:1FE0` (`Planting.Move`), `0609:1B72` (`FieldCursor.TimeBar`), `0609:13C5` (`Planting.Border`),
`0825:06CB` (`Ranking.Apply`), Countdown `21FD:04E5`/`04F9` (`Rules/Countdown`, Teil 2). Gelesen aus Aufruf 27: Kursfaktor
`4436` und Grundeinheit `3CF0` (`08CF:11D7`, unverändert).

## Szene (Teil 3)

`PurchaseScene` spielt die `PurchaseEvent`s und `FieldEvent`s in der Reihenfolge des Kerns; sie liest den Kern nur, im
Augenblick der Anforderung. Gemeinsames (Bild `186C:0002`, Seiten, Blenden, Objekte, `0185`, `060D`, `0665`, `0C3F`) und die
Zeichenroutinen aus `0609` (`1879`, `18E5`, `1978`, `1AC0`, `1ED9`, `1256`, `13C5`, `1B72`) spielt eine `FieldScene` auf
denselben Schichten; die Aufrufe `0A96:0030`/`0267`/`0433`/`04D7`/`0525`/`057A`/`05FB` und `0A69:004D`/`0164`, die der Kern
mit ihren Argumenten meldet, gehen unverändert in die Befehlsliste (Textsegment 0 = DS). Alle eigenen Routinen hängen nur an
die Liste an:

| Routine | Zeichenteil |
|---|---|
| `15C6(Laden, x)` | Atlas (`B[00EC + Laden]`, `B[00F0 + Laden]`) 40h × 2Ch nach (x, 50h) |
| `15F5(x, y, Kugel)` | Atlas (0, 16h · Kugel + 58h) 20h × 16h nach (x, y) |
| `1688` | (20h, A8h) 80h × 16h mit der Kachel (A0h, 0) 20h × 16h |
| `16B4(Platz, Zahl)` | Atlas (0Ah · Zahl + F0h, CFh) 0Ah × 0Ah nach (40h · p + 0Bh · Platz + 1Ah, 35h) |
| `16F5` | (40h · p + 1Ah, 35h) 2Ch × 0Ah mit der Kachel (F0h, B0h) 20h × 16h |
| `172A` | je Edelstein i < `37AF[p]` Atlas (109h, C6h) 7 × 7 nach (40h · p + 8 · (i mod 5) + 26h, 8 · (i / 5) + 14h) |
| `1859` | Text DS:013C (Farbe 0Dh, Schatten 5) bei (40h · p + 26h, 2Ah), Konto `300E[p]` 4 Stellen (Farbe 0, Schatten 5) bei 40h · p + 34h |
| `18B5`, `1935` | Zifferblatt (110h, B0h) 20h × 20h nach (110h, 6); n = min(`3780`, 1Eh), Zeiger (11Fh, 16h) – (10Fh + `B[00F4 + n]`, 7 + `B[0113 + n]`) Farbe 0Fh und (120h, 15h) – (110h + …, 6 + …) Farbe 5 (Tabellen mit Vorzeichen) |
| `193E` | Bargeld `3CD2[p]` mit „$“, 5 Stellen, Farbe `B[0132 + p]` (mit Vorzeichen), Schatten 5, bei (40h · p + 26h, 11h) |
| `1981` | Ölvorrat `4B98` 3 Stellen, Farbe 5, Schatten 0Ah, bei (`15B0` + 2Bh, 77h) |

## Zustand

Siehe [DATENLAYOUT.md](../analyse/DATENLAYOUT.md), Abschnitt der Spur `einkauf`. Geschrieben: direkt `00E8`/`00EA`,
`159A`–`15B8`, `37AA`, `4B98`; indiziert (Disassembly) `08ED`, `1586`, `158A`, `159C`, `15A3`–`15AB`, `300E`, `3022`, `3320`/`3322`
(`00CD`), `3308`, `3786`/`3788`, `37AF`, `3CD2`/`3CD4`, `43B2`, `4520` (über `1B72`), `4BAC`, `4BCA`, `4BD2`.

## Seiteneffekte in Reihenfolge

Zufall siehe oben; Eingabe `0E9E:037A` und Countdown mit Pausenprüfung `21FD:0245` (in `04F9`) nur in `01BD`; Ton `21A7:0054`
als Ereignis; keine Uhr, keine Dateien.

## C#-Umsetzung

`Traders.Core/Purchase/PurchaseRules.cs` (Teil 1), `PurchaseLayout.cs`, `PurchaseEvents.cs` (`IPurchaseOutput` : `IFieldOutput`,
Texte über `ITextMemory`). Teil 2: `Purchase.cs` – `0004`, `00CD`, `01BD` als Folge von `FieldWait` (je Rückgabe ein Bild):
Blende `037D` (8 × (1 + 4)), `0329` (8 × (1 + 3)), Seitenwechsel `051F`, `060D` je Spieler in `00CD`, `0185(2)` je Durchlauf,
Pausenprüfung vor dem Countdown-Takt, `0665` am Ende; `0A96:0C3F` am Ende von `1348` wechselt nur auf Karte 2/63h die Seite.
Gemeinsame Rahmenroutinen als `FieldEvent` (wie Anbau/Ernte), die eigenen als `PurchaseEvent`. `IPurchaseHost` (Steuerung
`0E9E:037A`, Pause `21FD:0245`), `PurchaseHost` (Spiel), `PurchaseStep` (Handler für Aufruf 29, Einbindung durch Spur `woche`).
Szene (Teil 3): `Traders.Presentation/Purchase/PurchaseScene.cs`.

## Prüfung

`tests/Traders.Tests/Purchase/PurchaseRulesDifferentialTests.cs`: jede Kernroutine direkt im Original (`PurchaseHarness`,
Zeichnen und Ton als `retf` mit Aufzeichnung, `1FE0`/`1B72`/`13C5`/`06CB` laufen) und im Port auf 400 erzeugten Zuständen
(kein/knappes Geld, Öl 27h/28h, Ladenöl 0/1, Raketen 9/10, Edelsteine 0/9/10, Konto um 2000, Kurs um Woche + 100, Ziehung in
jedem Zustand, Vorgaben `08ED`, 16-/32-Bit-Grenzen): Datensegment, Zufallszustand und Zahl der Ziehungen, Aufrufe, Ergebnis.
`PurchaseRealGameDifferentialTests.cs`: Zustände der Vorgabepartie (vier Computer) in den Wochen 0–3 an den Einsprüngen, je
wie erfasst, mit Mensch für Spieler 0 und 2 und mit beiden Schaltern aus. In den Wochen 0–3 nicht erreicht: `084F`, `09C4`,
`09F1` (kein Computer betritt den Raketenladen oder kauft vom Konto). Gegenprobe: ohne Geldprüfung beim Öl, ohne
Raketenobergrenze, mit falscher Kugel oder falscher Dekorationsziehung → rot.

`tests/Traders.Tests/Purchase/PurchaseFrameDifferentialTests.cs` (`PurchaseFrameHarness`, Teil 2): Gleichschritt ab dem Einsprung
`08CF:0004` der laufenden Vorgabepartie – Original mit Grafik und Ton, Tasten als Tastaturinterrupts; Port `PurchaseStep` mit
`PurchaseHost` in der `PhaseMachine`. Je Bild (Rücksprung-Eintritt `0D73:00D3`) Datensegment (ohne die Bytes der Ausgabe wie beim
Anbau), `DS:10D2`, Zahl der Ziehungen; am Ende erneut, Bildzahl gleich, Phasenmaschine bei Aufruf 30. Fälle: Woche 0 vier
Computer; Woche 2 Spieler 0 und 2 als Mensch (Lotto, Öl- und Raketenkauf); Woche 0 Spieler 0 und 2 im Edelsteinladen (Konto 500:
aufs Konto, zurückkaufen, gegen Bargeld). Je Lauf `DS:08F8` · 28h Durchläufe, Uhr je Countdown-Schritt, vier Kugeln.

Szene (Teil 3) im selben Gleichschritt: `PurchaseScene` auf eigenen Grafikschichten (Treiber, DAC und Datensegment aus dem
Einsprung); je Bild und am Ende VRAM aller vier Ebenen, DAC und Bildstart gleich, die Zahl der Rücksprung-Warten der Szene gleich
der des Kerns, und jede Befehlsliste, die der Ablauf ausführt (`0A96:06A8` aus Segment `08CF`), vor dem Ausführen gleich der des
Originals. Alle drei Fälle: 1683 Bilder verglichen (303–411 verschiedene), 801 Listen. Gegenprobe (im Test festgehalten): ein
Bild doppelt verarbeitet, ein Countdown-Takt verloren, eine Uhr, ein Bargeld bzw. eine Lottozahl nicht gezeichnet → rot (die
Listenprüfung schlägt jeweils vor der Datensegmentprüfung an).
