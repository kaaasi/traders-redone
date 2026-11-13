# 0609:0C73 / 0609:0D02 / 0609:0DF9 / 0609:1540 / 0609:18E5 / 0609:1912 / 0609:193C / 0609:1978 / 0609:1A03 / 0609:1A97 / 0609:1AC0 / 0609:1C83 / 0609:1CB1 / 0609:1CF4 / 0609:1E6B – Zeichenteil der Anbauphase (mit dem Zeichenteil von `13C5` und `0204`)

- **Ladebild-Adresse:** Segment `0609` (Dateioffset `0x9690`); `0C73` bei `0xA303`, `1540` bei `0xABD0`, `1CF4` bei `0xB384`.
- **Aufgabe:** Zeichenroutinen der Anbauphase `0609:0004` (Befund [0609_0004_anbau.md](0609_0004_anbau.md)): Feldkacheln mit
  Besitzerrand und Verkaufskreuz, Rand des Spielfelds, Tafel des Spielers am Zug (Gelände unter dem Cursor, Zeitbalken,
  Kaufmarke), Auswahlanzeige (gewählte Art mit Waren und Vorratsbalken, Raketen oder Landpreis) und Statuszeilen. Alle hängen
  nur Einträge an die Befehlsliste `0A96` an; keine schreibt ins Datensegment (Suche nach Speicherzugriffen, R5P-4), keine
  wartet und keine zieht Zufall (die Ziehungen in `13C5` gehören dem Kern).
- **Aufgerufen von / Phase:** Anbau (Aufruf 8): `1BE5` (Aufbau: `1ED9`, `1256`, `13C5`, je Feld `1C83`), `1EA3` (`1AC0`, `1A03`),
  `0204`, `03E0`-Durchgänge über `064D` (`1C83`, `1CB1`, `0C73`, `1A03`, `1A97`) und `047B` (`0C73`). `0C73` auch aus der
  Cursorlogik (Ereignis `DrawSelection`, [0609_047B_cursor.md](0609_047B_cursor.md)).
- **Parameter / Rückgabe:** `1540(x, y)`, `1C83(x, y)`, `1CB1(x, y)`, `1CF4(x, y)` Feldkoordinaten; `1E6B(x, y, Besitzer)` → AL;
  `1912(Ware, x, y)`; `0DF9(Vorrat, Bedarf, Zusatz, y)`; `13C5(x, y, w, h)`; die übrigen ohne. p = `37AA` (cbw) in allen.

## Ablauf (eigene Worte)

Atlaskoordinaten (sx, sy), Größe w × h, Ziel (x, y); Rechnungen 16-bittig, Feldindex x·6 + y (`imul`). Feldecke links oben
(`451E` + 20h·x, `4528` + 16h·y), Feld 20h × 16h. Px = (p << 6).

- **`1540`:** außerhalb (`10A7`) nichts. Gelände t = `45DA`, Art k = `2EB1`, Besitzer b = `4442`. t = 3: k = −2 → (A0h, 42h); k = 4 →
  (20h, 42h + 16h·b); sonst (20h, 16h·k). t = 2: k = −2 → (0, 58h + 16h·((x·y) mod 3), `idiv`); sonst (0, 16h·k). Sonst Basis 0
  (t = 0) bzw. 20h; Nachbarn gleichen Geländes: oben +2 (y > 0), links +1 (x > 0), rechts +1 (x < Spalten − 1), unten +2
  (y < Zeilen − 1); vier Viertel 10h × 0Bh aus (`00D0[i]` + Basis (+10h rechts), `00D4[i]` (+0Bh unten)) mit i = oben+links,
  oben+rechts, unten+links, unten+rechts. Danach k ≠ −2: maskiert (Basis + 40h, 16h·k), Maske (Basis + 90h, 16h·k), 20h × 16h.
- **`1CF4`:** b = −1 → nichts. Farbe `00C8[b]` (Wort); Linienmuster 5555h; Linien an jeder Seite, deren Nachbar außerhalb liegt
  oder einem anderen gehört (`1E6B`), Reihenfolge links, rechts, oben, unten; Muster FFFFh; steht die Verkaufsmarke des
  Besitzers (`3CAF`/`3CB3`) auf dem Feld, zwei Diagonalen.
- **`1C83`:** innen `1540` und `1CF4`. **`1CB1`:** `1C83` für Feld, links, rechts, oben, unten.
- **`13C5` (Zeichenteil):** Ecken (120h/130h, 6Eh) 10h × 4 an (x − 10h | x + w, y − 4), (120h/130h, 72h) 10h × 0Ah an
  (… , y + h); je Spalte i = 0 … w/20h − 1 (`idiv`, Vergleich mit Vorzeichen) oben (120h, 7Ch + 0Eh·r) 20h × 4 an (x + 20h·i,
  y − 4), unten (120h, 80h + 0Eh·r′) 20h × 0Ah an (x + 20h·i, y + h); r, r′ = `random(1)` ∈ {0, 1} (`21FD:01BA` liefert 0 … n);
  zuletzt die Seiten als Kacheln (120h/130h, 0) 10h × 58h über (x − 10h | x + w, y) 10h × h.
- **`1A03`:** Gelände unter dem Cursor (`304E`/`305A`): (F0h, 2Ch·t) 30h × 2Ch an (Px + 20h, 6), dann `1A61`.
- **`1A97`:** (C0h, 24h) 8 × 8 an (Px + 20h, 2Ah) (`0A96:0311`). **`18E5`:** (C0h, B0h) 30h × 2Ch an (Px + 20h, 6).
- **`1AC0`:** t = `4520[p]`, wenn `45BA` = 1, sonst 28h. (110h, D0h) 10h × 2Ch an (Px + 10h, 6); Kacheln (110h, F1h) 10h × 8 über
  (Px + 10h, 7) 10h × 20h; Kacheln (110h, 8p + D1h) über (Px + 10h, 2Fh − t) 10h × t.
- **`1912`:** (10h·Ware + C0h, DCh) 10h × 7. **`193C`:** (10h·`2E94[p]` + 40h, EEh) 10h × 0Eh an (Px + 31h, 0Ah).
- **`1978`:** für i = 0 … `4BAC[p]` − 1 (mit Vorzeichen) Rakete (F0h, C6h) 7 × 9 an (Px + 8i + 26h, 18h), ab i = 5 an
  (Px + 8(i − 5) + 26h, 22h).
- **`0DF9`:** s = Vorrat / `2F26` + 1 (höchstens 1Fh), m = Bedarf / `2F26` (`idiv`); Farbe 8 (Bedarf > Vorrat), 0Eh (Bedarf +
  Zusatz > Vorrat), sonst 0Dh; gefülltes Rechteck (Px + 2Eh, y) s × 5; m ≤ 1Eh → Linie Farbe 2 (Px + m + 2Eh, y − 1 … y + 6).
- **`0D02`:** s = `2E94[p]`, a = `08E7[s]`, b = `08E4[s]`; `1912(a, Px + 20h, 1Bh)`, `1912(b, Px + 20h, 23h)`;
  `0DF9(3CF4[p][a], 2EE7[p][b]·2F26, 48A2, 1Ch)`, `0DF9(3CF4[p][b], 2EE7[p][s]·2F26, 2F26, 24h)` (Indizes wie im Original
  vertauscht: Bedarf des ersten Balkens aus Art b, des zweiten aus der gewählten Art s).
- **`0C73`:** `18E5`, `193C`; Art −1: Text an `[37F0]` (Fernzeiger) Farbe 8 an (Px + 2Dh, 23h); 3: `1978`; 4: Landpreis `3746`
  (cwd) mit „$“, 4 Stellen, Farbe 0Dh an (Px + 2Ah, 23h); sonst `0D02`.
- **`0204` (Zeichenteil):** `001B`; `1582` ≠ 1: Schattentext `0A69:004D` (112h, 11h, `[37E8]`, 5, 0Bh, −1, 1), Zahl `0A69:011B`
  (118h, 18h, `48A2`, 2, 2, 0Ah, −1, 1), Text (112h, 1Fh, `[37EC]`), Zahl (118h, 26h, `2F26`); `1582` = 1: vorher die getippte
  Notiz (`21FD:05DB`, `0A69:0288` aus `37E0`, `0185(50)`), dann `1F04(10h, 6, 120h, 2Ch)`. Danach `21FD:060D` (Ausgabe).
- **`1BE5`:** Ablauf und Kernteil im Befund `0004`; im Bild: Liste mit `1ED9`, `1256(0, 0, 140h, 36h)`, `13C5`; je Feld eine
  eigene Liste mit `1C83`; `0A96:0C3F([082E])`.

## Zustand

Nur gelesen: `37AA`, `451E`/`4528`, `08FC`/`08FE`, `45DA`, `2EB1`, `4442`, `00C8`, `00D0`/`00D4`, `3CAF`/`3CB3`, `2E94`, `304E`/`305A`,
`43FA`, `45BA`, `4520`, `4BAC`, `3CF4`, `2EE7`, `48A2`, `2F26`, `08E4`/`08E7`, `3746`, `1582`, Fernzeiger `37E8`, `37EC`, `37F0`.
Geschrieben wird nur die Befehlsliste (`15BA`/`15BE`/`15C0`…, Ausgabe).

## Seiteneffekte in Reihenfolge

Grafik: Listeneinträge in der beschriebenen Reihenfolge. Zufall nur in `13C5` (Kern, `Planting.Border`). Keine Uhr, keine
Eingabe, kein Ton, kein Warten (außer der getippten Notiz in `0204` bei `1582` = 1).

**Korrektur zu Teil 1:** `random(1)` liefert 0 oder 1 (`21FD:01BA`: `(Zustand >> 8) mod (n + 1)`), nicht immer 0. Die Ziehungen
in `13C5` wählen das Randstück; der Kern gibt die Ergebnisse deshalb mit dem Ereignis `DrawBorder(x, y, w, h, r0, r′0, r1, …)`
an die Szene.

## C#-Umsetzung

`src/Traders.Presentation/Field/FieldScene.cs`: `Tile` = `1540`, `OwnerBorder` = `1CF4` (mit `Edge` = `1E6B`), `Field` = `1C83`,
`FieldAndNeighbours` = `1CB1`, `Border` = Zeichenteil `13C5` (Ziehungen als Argument), `Panel` = `1A03`, `PurchaseMarker` =
`1A61`, `PurchaseUsed` = `1A97`, `TimeBar` = `1AC0`, `SelectionPanel` = `18E5`, `GoodIcon` = `1912`, `SelectionIcon` = `193C`,
`RocketIcons` = `1978`, `StockBar` = `0DF9`, `GoodsBars` = `0D02`, `SelectionDisplay` = `0C73`, `Status` = Zeichenteil `0204`
(ohne `060D`). `Apply` spielt die Zeichenereignisse des Kerns (`FieldEventKind.Draw*`) ab; Seiten, Paletten, Objekte und
Listenausführung gehören der Ausgabe (Teil 3). Kernteil `13C5`: `Planting.Border` (Ziehungen im Ereignis).

- `[APPROX]` `0204` bei `1582` = 1 (zweite Runde `0080`, Spur R9): die getippte Notiz wird nicht gezeichnet (wie ihre Bilder im Kern).
- Division durch `2F26` = 0 oder −32768/−1 in `0DF9` löst im Original INT 0 aus; der Port wirft.
- Art −1 eines Feldes ist unerreichbar (`096A` pflanzt nur 0–3, die übrigen Schreiber behalten die Art oder schreiben 4/−2);
  sie läse Atlas und Masken vor deren Anfang, wo `Traders.Vga` Nullen annimmt (`[APPROX]` dort). Nicht getestet.

## Prüfung

`tests/Traders.Tests/Field/PlantingSceneDifferentialTests.cs` mit `FieldSceneHarness` (Wochenzustand, Original ruft die
Routine, Port die Szene): **Befehlsliste** (Zahl `15BE`, Einträge ab `15C0`) Eintrag für Eintrag vor der Ausführung (#103 P3-3),
dann alle vier Ebenen; Datensegment des Kerns unverändert (Objektsätze mit Mustern ≠ 0). Fälle: 48 Zufallsfelder (1–9 × 1–6,
Gelände, Besitzer, Arten, Verkaufsmarken) mit je vier `1C83` (auch Rand und außerhalb) und einem `1CB1`; `1540` für jedes
Gelände × jede Art × Nachbarmuster; `13C5` mit 18 Größen (auch w = 0, 1Fh, negativ) über `Planting.Border` mit den Ziehungen
des Kerns (Zustand `10D2` danach gleich); `0C73` 96 Fälle (alle Arten −1…4, Vorräte auch negativ, `2F26` auch negativ,
Raketen auch ≤ 0, Landpreis negativ); `0DF9` an den Grenzen (Vorrat 0, 1Eh–20h, ±7FFFh; `2F26` ±1, 127, −128); `1A03`/`1A97`/`1AC0`
je 32; `0204` mit `48A2`/`2F26` an den Grenzen und `1582` = 1 (dort nur Pixel, die ungenutzten Eintragsbytes stammen von der
nicht gezeichneten Notiz); `1BE5` ganz (Original gegen `Planting.Board` mit Szene, Seitenkopie, Zustand `10D2` und
Schleifenvariablen gleich). Gegenproben (je rot, lokal): rechter Nachbar mit `>=`, Balkenfarben 0Dh/0Eh vertauscht, zweite
Raketenreihe ab i = 4, eine Diagonale des Verkaufskreuzes fehlt, Randziehung oben ignoriert, Gelände 2 mit x + y, Schatten
x + 1 statt − 1, Zeitgrenze `≠ 0` statt `= 1`.
