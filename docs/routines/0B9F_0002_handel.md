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
`0CCA(Preis)` (nach dem Kauf, `37AA` = Käufer). Keine liefert einen Wert, der verwendet wird. Im Spiel: `TradeComputer`
(Adapter auf `Traders.Core.Ai.TradeAi`).

### Stapelwort unter der lokalen Variable von `141D:0397` (Fortsetzung 2)

`0397` liest in einem Zweig seine nicht gesetzte lokale Variable (Befund `141D_0008_handelski.md`); sie liegt bei SP−8 von
`00FE`. Im RefEmu über die Wochen 0–3 der Vorgabepartie an `141D:039C` gemessen, wer das Wort zuletzt schrieb (19 200 Aufrufe):
`0E9E:037A` (dessen `[bp-2]` endet als Aktionswort von Spieler 3: FFFFh bei Computer, sonst 0, 1, 2, 3, Ah oder FFFFh), das
Endwort des vorigen `0397`, und `0C29:0F68` (`push si` nach einem Rollenwechsel; die Rücksprungadresse von `21A7:0054` davor
wird immer überschrieben). SI ändert zwischen `main` und `00FE` niemand (`1777:01F8`, `0B9F:0002`, `0C29:0001`…`015C` benutzen
SI nicht): es ist der Argumentzähler von `main` `1777:000C` (`mov si,1`, `inc si` je Argument), ohne Argumente 1 – gemessen
SI = 1 in allen vier Wochen. Der Port führt das Wort in `TradeRules.ChooseRoles` in dieser Reihenfolge mit
(`MainArgumentCount` = 1). **Nicht nachgebildet (`[APPROX]`):** ein Interrupt zwischen `037A` und `039C` kann das Wort mit
Flags, CS oder dem gesicherten AX überschreiben (8 der 19 200 Aufrufe, Werte 0001, 0002, 0046, 241D, FF02; keiner im Zweig, der
das Wort verwendet) – das hängt vom Zeitpunkt des Interrupts ab. Der Gleichschritt-Test verfolgt das Wort im Original und bricht
mit „not comparable“ ab, falls `0397` ein solches Wort verwendet.

## Zustand

Siehe [DATENLAYOUT.md](../analyse/DATENLAYOUT.md), Abschnitt der Spur `handel`.

## Seiteneffekte in Reihenfolge

Kein Zufall im Handel selbst (nur in der KI `141D`). Ton `21A7:0054` als Ereignis; Eingabe `0E9E:037A`; Pausenprüfung
`21FD:0245` in `21FD:04F9`.

## C#-Umsetzung

`Traders.Core/Trade/TradeRules.cs` (Regeln, Teil 1), `TradeLayout.cs`, `TradeEvents.cs` (`ITradeHost`, `ITradeAi`);
`Traders.Core/Rules/Countdown.cs`. Ablauf Bild für Bild (Teil 2): `Trade.cs` (`Run` = `0002`, `Ware` = `0001`, `WareFrames` =
`0046`, `Opening` = `0086`, `RoleChoice` = `015C`, `Bidding` = `01B7`, `Transition` = `080C`) als `IEnumerable<FieldWait>`,
`TradeStep.cs` (Handler für Aufruf 23, nicht eingebunden; Bildladen über `ImageTable.RecordLoad`), `TradeHost.cs` (Steuerung
`0E9E:037A`, Pause `21FD:0245`), `TradeComputer.cs` (KI `141D`). Szene: `Traders.Presentation/Trade/TradeScene.cs`.

## Szene (Teil 3)

`TradeScene` spielt die `TradeEvent`s in der Reihenfolge des Kerns; sie liest den Kern nur, und zwar im Augenblick der
Anforderung (jede Anforderung sofort anwenden). Gemeinsames (Bild `186C:0002`, Seiten, `0329`, `0185`, `0665`, `051F`,
`0C3F`) über eine `FieldScene` auf denselben Schichten; eigene Bildfolgen: `0353`, `03B4`, Übergang `080C`.

| Routine | Zeichenteil |
|---|---|
| `080C` | `21FD:0590`; 100-mal Liste leeren, schwarze Zeile i und C7h − i (`0A69:000B`), ab Zeile 31h jede siebte eine Stufe `03EB(3, n)`, auf Karte 1 `0185(1)`, Liste ausführen; `04DA(3)`, `051F` |
| `04BD` | `21FD:026C`; Ware 2: Farben 1–5 aus `W[0248]`, Ware 3: Farben 1–3 aus `W[0252]` (`02C9`) |
| `0635` | Liste leeren, `065C` (Bild 0) oder `0739` (Bild 1), `0B1F`, Liste ausführen |
| `065C` | je Spieler Teil p an (`2BD0`, `2BDA`), Käufer Teil 8 (y 0), sonst Teil 6 (y 96h); Leiter Teil 4 an `2BCE`, Beschriftung Teil 9 und `1050` an (`2BD8`, `2BE2`), Teil 5 an (`2BCA`, 96h); Ränder `0C8A` bei `2BCE` − 4 (nur `2BCE` > 0) und + 40h |
| `0BD9(Teil, x, y)` | Teil 4: schwarz 40h × C8h, Atlas (0, 96h) 40h × 8 nach y 5, Kachel (40h, C8h) 40h × 1 über 40h × 5, (0, A0h) 40h × 28h nach y A0h; sonst Atlas (`W[0262]`, `W[0276]`), 40h breit, `W[028A]` hoch (je + 2·Teil) |
| `0C8A(x)` | vier Spalten C8h hoch in den Farben `B[029E + 4·Bild + i]` (`0A69:002C`) |
| `1050(x, y)` | Warenzeichen mit ODER (`0A96:020E`) aus (`W[02AA + 2w]`, `B[02B2 + w]`), 30h × `B[02B6 + w]`, nach (x + `B[02BA + w]`, y + `B[02BE + w]`), Versatz mit Vorzeichen |
| `0739` | je Spieler Käufer- oder Verkäuferspalte aus `V-BHANDE` (Kopf/Fuß, Spielerzeichen mit ODER, schwarzes Feld mit Spielerbild), Leiter schwarz mit Kopf (F0h, 0) und Fuß (F0h, 0Dh) nach y A1h, `1050`, Ränder |
| `09BB` | Bargeld (5 Stellen, Farbe `B[0240 + 4·Bild + p]`) und Vorrat (3 Stellen, `B[025C + Bild]`) über `0A96:057A`; Bild 0 bei (x + 0Dh / 29h, y + 90h), Bild 1 Käufer (x + 17h, y + 4Ch) / (x + 1Ah, y + 44h), sonst y + 15h / 4Ah |
| `0B1F` | Grundpreis 3 Stellen, Farbe `B[025E + Bild]`, bei (`2BCE` + 27h, `B[0260 + Bild]`) |
| `0B60` | Bild 0, Bedarf > 0: schwarze Linie 8 breit bei (x + 18h, y + 89h − Bedarf / 10) |
| `0CC1`, `0D6A` | Countdown: Bild 0 zwei Säulen zu höchstens 1Eh Zeilen aus dem Atlas (130h, 50h − n), ein Schritt stellt eine Zeile aus (4Ah, C1h − n) wieder her; Bild 1 Balken (`2BCA` + 4, 4) Farbe 0Dh, ein Schritt eine schwarze Spalte |
| `0E01` | **direkt mit dem Treiber**, nicht über die Liste: Marke 10h × 28h (`23CD:06D4`) aus (10h·p [+ 40h Verkäufer] [+ 80h Partei des gezeigten Handels], C8h) bzw. (100h, C8h) nach (`2BCE` + 10h·p, 74h − Position [+ 2 Käufer]); auf der Leiter (0 ≤ Position < 64h, Rolle ≠ 6) der Preis (`2295:0001`, 3 Stellen nach `[02A6]`) in der Spielerfarbe (`23CD:05C3`, `05DD`) bei y 74h − Position + 23h |
| `0F68` | schwarzes Feld 10h × 28h an der Marke |
| `109F` | Bild 0: je Vorratszehner Liste leeren, `1163(i, 1)`, ausführen; dann `0B60` ebenso. Bild 1: `134D` |
| `1108(Richtung)` | Bild 0: Zeile Vorrat / 10 − 1 dazu (Kauf) bzw. die darüber weg (Verkauf), `0B60`; Bild 1: `134D` |
| `1163`, `127E` | Haufenzeile nach der Form des Spielers (Tabellen `02C2`, `0326`, `04B6`, `04DE`; Sprungtabelle `1276`): zwei Linien (Farben `B[0566 + p]`, `B[0240 + p]`) bzw. beim Wegnehmen die Zeile aus dem Atlas (40h·p + x1, 88h − Zeile) |
| `134D`, `146B` | Bild 1: Linien von (x + 20h, y + 13h) nach (x + `B[056A + k]` + 4, y + `B[05CF + k]`) für den zuletzt gezeigten Vorrat (`0634`, Farbe `B[0640 + p]`, nach dem Schreiben von `0642`), Bedarf / 10 (`B[063C + p]`, nur Waren 0–2) und Vorrat / 10 (Farbe 3) |

**Bilder (Teil 2):** `21FD:051F` und `0665` wie `Planting.Flip`; `21FD:0185(n)` n Bilder; Blenden `0329`/`0353`/`03B4` je acht
Stufen mit 3/1/2 Bildern plus einem Bild für `03EB` auf Karte 1 (`DS:090E`); `0A96:0C3F` blättert nur auf Karte 2/63h;
`080C`: `21FD:0590` (zwei Bilder), auf Karte 1 je Zeile (100) ein Bild `0185(1)` und ab Zeile 49 jede siebte eine Palettenstufe
`03EB` (ein Bild, also acht), zuletzt `051F`; `21FD:04F9` mit Pausenprüfung `0245` vor dem Takt.

**Kernanteile der Zeichenroutinen:** `0635` (über `065C`/`0739` Schleife `37AA` → 4; über `0B1F` `2BE4` = `455A`), `0B1F`
(`2BE4`), `0E01` (Schleife `37AA` → 4), `109F` in Bild 1 → `134D` (`0634`/`0642`), `2295:0062` (`DS:1126`, gelesen von
`2295:0001` im Kern). Der Zahlenpuffer von `0E01` (`2295:0001` nach `[02A6]`) und die Seitennummer `082E` von `0C3F` gehören der
Ausgabe.

## Prüfung

`tests/Traders.Tests/Trade/TradeRulesDifferentialTests.cs`: jede Regelroutine direkt im RefEmu gerufen
(`TradeHarness`: Ton, Zeichenroutinen, Treiber, Steuerung und `141D` durch `retf` ersetzt und mit Argumenten bzw. gelesenen
DS-Werten aufgezeichnet), Vergleich ganzes Datensegment, Zufallszustand, Aufruffolge, Rückgabe; Grenzfälle Positionen −3/0/63h/67h,
Preis 999, Bargeld unter/gleich Preis, Vorrat 9/10/990, ohne Käufer/Verkäufer, Cursor außerhalb, Teiler 0 in `024A`.

`TradeFlowDifferentialTests` (`TradeFlowHarness`): ganzer Handel `0B9F:0002` auf den Zuständen am Einsprung der Wochen 0–3
einer echten Partie, je mit 0, 2 und 4 Menschen, Aktionen aller vier Spieler per Skript; Zeichen-, Bild- und Seitenroutinen,
Ton, Steuerung, Pause und `141D` durch `retf` ersetzt und aufgezeichnet, die Zeichenroutinen mit DS-Schreibzugriffen (`0635`,
`109F`, `1108`, `0E01`, `0B1F`) laufen mit ersetzten Grundroutinen; Vergleich ganzes Datensegment, Zufallszustand,
Aufruffolge. Dazu eine Ware mit beiden Bildern auf Karte 1/2/63h. `TradeFrameDifferentialTests` (`TradeFrameHarness`):
Gleichschritt Bild für Bild in der laufenden Partie (Woche 0 Computer, Woche 2 mit zwei Menschen und Tasten), `141D` beiderseits
ersetzt; Gegenprobe mit doppelt verarbeitetem Bild.

Seit Fortsetzung 2 läuft der Gleichschritt mit der echten KI `141D` gegen `TradeComputer` (Woche 0 vier Computer; Woche 2
Spieler 0 und 3 Mensch mit Tasten; dazu Woche 2 mit ersetzter KI) und vergleicht je Bild auch das Bild der `TradeScene`
(vier Ebenen, DAC, Bildstart) sowie die Befehlslisten des Ablaufs (`0A96:06A8` aus `0C29:0046`…`01B7`, Zähler `15BE` und
Einträge ab `15C0`). Belegt im Test: `0397` verwendet das Stapelwort aus allen drei Quellen (`0397`, `0F68` = 1, `037A`).
Gegenproben: Stapelwort immer −1, SI = 0, Marken / Countdown-Schritt / Zahlen eines Spielers nicht gezeichnet → rot.
`TradeAiHarness`/`TradeAiGame` prüfen den Rückgabewert von `ChooseSide` gegen `[bp-2]` bei `141D:0460` (R7H-1, #192).
