# 0B9F:0002 / 0B9F:005C / 0B9F:009E / 0B9F:00FE / 0B9F:01E1 / 0B9F:024A / 0B9F:02BE / 0B9F:0321 / 0B9F:04FC / 0B9F:0549 / 0B9F:0586 / 0B9F:060A / 0B9F:0695 / 0B9F:080C / 0C29:0001 / 0C29:0046 / 0C29:0086 / 0C29:015C / 0C29:01B7 / 0C29:0323 / 0C29:03B2 / 0C29:04BD / 0C29:051A / 0C29:0635 / 0C29:065C / 0C29:0739 / 0C29:09BB / 0C29:0B1F / 0C29:0B60 / 0C29:0BD9 / 0C29:0C8A / 0C29:0CC1 / 0C29:0D6A / 0C29:0E01 / 0C29:0F68 / 0C29:0FB9 / 0C29:1050 / 0C29:109F / 0C29:1108 / 0C29:1163 / 0C29:127E / 0C29:134D / 0C29:146B – Handel

- **Ladebild-Adresse:** Segment `0B9F` (Dateioffset `0xEFF0`, Größe `0x8A0`), Segment `0C29` (Dateioffset `0xF890`, Größe
  `0x14A0`); `0B9F:0002` bei `0xEFF2`. Sprungtabellen: `0B9F:04E4` (12 Einträge, Verkäuferaktion + 1), `0C29:1276` (4 Einträge,
  Spieler in `1163`).
- **Aufgabe:** Handel der Woche (Aufruf 23 der Wochenschleife `1777:01F8`): für jede der vier Waren eine Preisleiter. Jeder Spieler
  wählt Käufer oder Verkäufer (Verkäufer nur mit mindestens zehn Einheiten), dann steigen die Käufer, die Verkäufer sinken; treffen
  der höchste Käufer und der tiefste Verkäufer aufeinander, wechseln zehn Einheiten zum Preis „Position / 2 + Grundpreis“. Ein
  Countdown begrenzt beide Abschnitte.
- **Aufgerufen von / Phase:** `0B9F:0002` nur von `1777:01F8` (Aufruf 23); `0B9F:06A0` (Warenpreise, portiert) vorher, `078A`
  (portiert) am Ende jeder Ware. Alle übrigen Routinen nur aus `0B9F`/`0C29` (Aufrufgraph `tools/callgraph.py`, Prüfung Teil 1).
- **Parameter / Rückgabe:** `0C29:0001(Ware, Bild)`; `024A` gibt die Länge der Bietphase zurück (AX); `1108(Richtung)`;
  übrige ohne.

## Ablauf (eigene Worte)

- **`0B9F:0002`:** `06A0`, `141D:0008`, `2295:0062(2Fh)`, dann `0C29:0001` viermal mit (Ware, Bild) = **(1, 0), (0, 1), (2, 0),
  (3, 1)** – Ware 1 und 2 auf `V-AHANDE`, Ware 0 und 3 auf `V-BHANDE` –, zuletzt `2295:0062(20h)`. Das erste Argument wird `45D0`
  (Ware, Wort), das zweite `2BC5` (Bild, Byte); `0001` lädt das Bild (`186C:0002` mit `[08D6]` bzw. `[08D8]`), Schrift 2,
  `0046`, Schrift 0.
- **`0046`:** `3D46` = Eh; `005C`, `009E`, `0086` (Rollenwahl), `0A96:001B`, `015C` (Rollenwahl-Schleife), `01E1`; gibt es
  Verkäufer (`2BBC` ≠ 0): `141D:0464`, `01B7` (Bietphase), `078A`.
- **`005C`:** `2BC6[0..3]` = 0 (Schleife `37AA`, bleibt 4), `3CAE` = 0, `3D18` = 0, `455A` = `2BC2` = `4898[Ware]`, `2BBE` = 0.
- **`009E`:** Bedarf `43B6[p]` (Doppelwort): Ware 3 → 100, sonst `2EE7[p·4 + k]` · `2F26` (Bytes mit Vorzeichen, 16-Bit-Produkt,
  Unterwort mit `cwd` erweitert) mit k = Ware + 1, bei 3 → 0. Nur für die Anzeige (`0B60`, `134D`).
- **`0086`:** alle Rollen 1; `051A`, `080C` (Übergang), `0635`; je Spieler `109F`, `0A96:001B`, `09BB`, `0B60`, `141D:021A`,
  Rolle 0 und Position −3, `0A96:06A8`. Dann `0E01`, `0A96:001B`, `3D1C` = 0, `43EE` = Ah,
  `21FD:04E5(19h − 5·IQ, 14h)`, `0CC1`, `0A96:06A8`, `0A96:0C3F([082E])`, Ton (2, 4Dh), `04BD`, `21FD:0329`.
- **`015C`** (Rollenwahl, je Bild solange `3780` ≠ 0): `21FD:051F`, `0A96:06A8`, `0A96:001B`, `00FE`, `21FD:04F9` (= 1 →
  `0D6A`), `0A96:06A8`, `0E01`. Danach `21FD:0665`, Ton (2, 0), `21FD:0185(19h)`.
- **`00FE`** (ein Bild der Rollenwahl): `0E9E:037A`; je Spieler: Computer (`0840` = 1) → `141D:0397`; Aktion `3308` 3 (ab) und
  Rolle ≠ 0 → Ton (0, 19h), `0F68` (alte Marke löschen), Rolle 0, Position −3; Aktion 2 (auf), Rolle ≠ 1 und Vorrat ≥ 10 → Ton
  (1, 18h), `0F68`, Rolle 1, Position 67h.
- **`01E1`:** `2BBC` = Zahl der Verkäufer; `439C` = 0, wenn ein Mensch kauft, `450C` = 0, wenn ein Mensch verkauft (sonst 1).
- **`01B7`** (Bietphase): `051A`, `21FD:03B4(0)`, `0635`, je Spieler `109F`, `0A96:001B`, `09BB`, `0B60`, `0A96:06A8`; `0E01`,
  `0A96:001B`, `21FD:04E5(024A(), 1Eh)`, `0CC1`, `0A96:06A8`, `0A96:0C3F([082E])`, `21FD:0353`, `4106` = `44F2` = −1, `3D1C` = 0,
  `43EE` = B4h, Ton (2, 4Dh), `0A96:001B`. Dann je Bild solange (`3780` ≠ 0 und `2BBC` ≠ 0) oder `3CAE` ≠ 0: `21FD:051F`,
  `0A96:06A8`, `0E9E:037A`, `02BE`, `2BC4` = 0, je Spieler (Computer → `141D:09F3`) `0321`; `0E01`, `0549`, `0A96:001B`, `0FB9`,
  `0B1F` wenn `455A` ≠ `2BE4`, `21FD:04F9` (= 1 → `0D6A`), `0323`, `0A96:06A8`. Danach `21FD:0665`, Ton (2, 0), Ton (0, 0),
  `21FD:0185(19h)`, `2BCC` = 0.
- **`024A`:** Summe über die Verkäufer von Vorrat / (5·IQ `2E8E` + 15) (16 Bit, `idiv`; Teiler 0 bei IQ −3 → Divisionsfehler),
  begrenzt auf 15 … 120; `2BBA` = Summe − 5; Rückgabe Summe.
- **`02BE`:** `2BB8` = Käufer auf der höchsten Käuferposition `3D1C`, `2BC0` = 0, wenn ein Mensch darunter ist; ohne solchen
  Käufer `455A` = `2BC2`.
- **`0321`** (Schritt des Spielers `37AA`): Position si. **Käufer:** Preis(si) = si / 2 + `455A` (16 Bit, `cwd`) größer als das
  Bargeld (32 Bit mit Vorzeichen) oder Vorrat ≥ 3DEh → Aktion = 3. Aktion −1: gerade si wie 2, ungerade nichts; 2: nicht wenn
  si = `43EE`, Preis = Bargeld (32 Bit) oder Preis = 3E7h (16 Bit); sonst Position + 1, > 63h → `04FC`; `2BC4` = 1. 3: si > −3 →
  Position − 1, `2BC4` = 1. **Verkäufer:** Vorrat < 10 → Aktion = Ah. Über die Tabelle `04E4` mit Aktion + 1 (vorzeichenlos ≤ 11):
  −1: ungerade si wie 3; 2: si < 67h → + 1; 3: si > 0 und si ≠ `3D1C` → − 1 (beide mit `2BC4` = 1); Ah: `2BC6` = 1 (geht). Andere
  Rollen nichts. Immer zuletzt `0586`.
- **`04FC`:** jeder Käufer Position − 1, nicht unter −3; `023C` − 1, bei 0 `455A` + 1 und `023C` = 2.
- **`0549`:** Ton (0, 1Ah) beim Übergang still → bewegt, Ton (0, 0) beim Übergang bewegt → still (`023E` = voriges `2BC4`).
- **`0586`:** `3D1C` = größte Käuferposition (ab 0), `43EE` = kleinste Position eines Verkäufers mit `2BC6` = 0 (ab 66h); hat sich
  eines geändert, `3D18` = 3.
- **`0323`** (je Bild): `3CAE` > 0 → −1; bei 2 `2BCC` = 0; bei `3D46` − 2 `37AA` = Verkäufer und `09BB`. Sonst `3D18` > 0 → −1 und
  `3D46` = Eh. Sonst bei `3D1C` = `43EE`: `060A`, `2BCC` = Bit des Verkäufers (`08C6`) plus Bit des Käufers (falls `4106` ≠ 10),
  `03B2`, `3D46` > 5 → −1, `3CAE` = `3D46`.
- **`060A`:** reihum ab `2BB4` + 1 (über 3 → 0) der nächste Verkäufer auf `43EE` → `44F2` (sonst unverändert), ab `2BB6` + 1 der
  nächste Käufer auf `3D1C` → `4106` (sonst 10).
- **`03B2`:** Preis = `3D1C` / 2 + `455A`; Ton (1, 1Bh + Käufer) bzw. (1, 1Fh) ohne Käufer; `0695`; Verkäufer: Bargeld + Preis
  (32 Bit), Vorrat − 10, `37AA` = Verkäufer, `1108(−1)`, `141D:0E67`. Mit Käufer: Bargeld − Preis, Vorrat + 10, `37AA` = Käufer,
  `09BB`, `1108(1)`, `141D:0CCA(Preis)`; Preis > `2BBE` → `2BBE` = Preis. Ohne Käufer (Verkäufer allein auf 0) bekommt der
  Verkäufer das Geld trotzdem.
- **`0695`:** `2BBA` = `3780`.
- **`0FB9`:** je Spieler mit `2BC6` = 1, der nicht Partei des gezeigten Handels ist (`08C6[p]` & `2BCC` = 0): Ton (1, 20h), Rolle 6,
  Position 67h, `2BBC` − 1, `0A96:04D7(2BCE + 16p, Dh, 10h, 8Dh, 0)`, `2BC6` = 0.
- **`051A`:** Käufer (Rolle 0) ab x = 0 in Schritten 40h (`2BD0`), y aus `0258[Bild]`; `2BCE` = x der Leiter; Verkäufer ab
  `2BCE` + 40h, y aus `025A[Bild]`, `2BBC` neu gezählt. Bild 0: Beschriftung `2BD8`/`2BE2` = (40h, 0) bei ≤ 2 Verkäufern, (0, 0)
  bei 3, sonst (C0h, 96h); Countdown `2BCA` = C0h bei 3 Verkäufern, sonst `2BCE` + 40h. Bild 1: (100h, 0), `2BCA` = `2BCE`.
- **`134D`** (Bild 1, Vorratsbalken): auf Grafikkarte 1 (`090E`) `0642` = DAh; Balken mit `146B`; zuletzt `0634[p]` = Vorrat / 10.

### Kern und Zeichenteil

| Routine | Kern | Zeichenteil |
|---|---|---|
| `0002`, `0001`, `0046`, `0086`, `015C`, `01B7` | Ablauf, Schleifen, Countdown, Aufrufe der KI | Bild, Schrift, Listen, Seiten, Paletten, Ton |
| `005C`, `009E`, `01E1`, `024A`, `02BE`, `04FC`, `0586`, `060A`, `0695`, `051A` | ganz | – |
| `00FE`, `0321`, `0323`, `03B2`, `0549`, `0FB9` | Regeln, Buchung | Ton, `0F68`, `09BB`, `1108`, `0A96:04D7` als Ereignisse |
| `134D` | Schreiben von `0642`, `0634` | Balken |
| `080C`, `04BD`, `0635`, `065C`, `0739`, `09BB`, `0B1F` (schreibt `2BE4`), `0B60`, `0BD9`, `0C8A`, `0CC1`, `0D6A`, `0E01`, `0F68`, `1050`, `109F`, `1108`, `1163`, `127E`, `146B` | `0B1F`: `2BE4` = `455A` | ganz (Teil 3) |

Rückrufe aus `0C29` nach `0B9F`: `0046` → `005C`, `009E`, `01E1`, `078A`; `0086` → `080C`; `015C` → `00FE`; `01B7` → `024A`,
`02BE`, `0321`, `0549`; `0323` → `060A`; `03B2` → `0695`. Countdown `21FD:04E5`/`04F9`:
[21FD_04E5_countdown.md](21FD_04E5_countdown.md).

KI `141D` (Spur `handelski`, Vertrag `ITradeAi`): `0008` (einmal), `021A` (je Spieler in `0086`), `0397` (je Computer und Bild in
`00FE`), `0464` (vor der Bietphase), `09F3` (je Computer und Bild in `01B7`), `0E67` (nach dem Verkauf, `37AA` = Verkäufer),
`0CCA(Preis)` (nach dem Kauf, `37AA` = Käufer). Keine liefert einen Wert, der verwendet wird.

## Zustand

Siehe [DATENLAYOUT.md](../analyse/DATENLAYOUT.md), Abschnitt der Spur `handel`.

## Seiteneffekte in Reihenfolge

Kein Zufall im Handel selbst (nur in der KI `141D`). Ton `21A7:0054` als Ereignis; Eingabe `0E9E:037A`; Pausenprüfung
`21FD:0245` in `21FD:04F9`.

## C#-Umsetzung

`Traders.Core/Trade/TradeRules.cs` (Regeln, Teil 1), `TradeLayout.cs`, `TradeEvents.cs` (`ITradeHost`, `ITradeAi`);
`Traders.Core/Rules/Countdown.cs`. Ablauf Bild für Bild (Teil 2) und Szene (Teil 3) folgen.

## Prüfung

`tests/Traders.Tests/Trade/TradeRulesDifferentialTests.cs`: jede Regelroutine direkt im RefEmu gerufen
(`TradeHarness`: Ton, Zeichenroutinen, Treiber, Steuerung und `141D` durch `retf` ersetzt und mit Argumenten bzw. gelesenen
DS-Werten aufgezeichnet), Vergleich ganzes Datensegment, Zufallszustand, Aufruffolge, Rückgabe; Grenzfälle Positionen −3/0/63h/67h,
Preis 999, Bargeld unter/gleich Preis, Vorrat 9/10/990, ohne Käufer/Verkäufer, Cursor außerhalb, Teiler 0 in `024A`.
