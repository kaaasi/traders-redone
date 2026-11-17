# 1F63:0B03 / 1F63:11AD / 0609:0080 – Wettbewerb (Aufruf 30) und zweite Feldrunde

- **Ladebild-Adresse:** Segment `1F63` (Dateioffset `0x22C30`); `0B03` bei `0x23733`, `11AD` bei `0x23DDD`. `0609:0080` im Segment
  `0609` (Dateioffset `0x9690`) bei `0x9710`.
- **Aufgabe:** Aufruf 30 der Wochenschleife `1777:01F8` (`1777:02CE`). Jede dritte Woche (Woche mod 3 = 2) zeigt Fat Mikes Szene
  die Vorräte der Ware 3; wer zuletzt gezahlt hat (`DS:4BC2` = 0) und den größten Vorrat hat, gewinnt Woche·50 Bargeld – oder,
  wenn der größte Vorrat mindestens 70 ist und es freie Felder gibt, ein freies Feld in einer zweiten Feldrunde `0609:0080`.
- **Aufgerufen von / Phase:** `0B03` nur von `1777:02CE`; `11AD` nur von `0B03` (`1F63:0B98`); `0609:0080` nur von `0B03`
  (`1F63:0C79`). Gemeinsam mit Fat Mike (Befund [`1F63_000F_fatmike.md`](1F63_000F_fatmike.md)) und dort portiert: `01B2`, `0C82`,
  `0D24`, `0E5C`, `0EC7`, `0F46` (`tools/callgraph.py`: im Baum von `0B03` sonst nur portierte Routinen der Segmente `0609`,
  `0A69`, `0A96`, `186C`, `21A7`, `21AF`, `21FD`, `2295`, `23CD`).
- **Parameter / Rückgabe:** keine; `01B2` liefert den größten Vorrat (di).

## Ablauf (eigene Worte)

- **`0B03`:** si = `4B9A`·50 (imul, 16 Bit); Woche mod 3 (idiv, mit Vorzeichen) ≠ 2 → Ende ohne jede Ausgabe. Sonst Bild
  `186C:0002([08D4])`, `21FD:037D(50h)`, `0D24`, `21FD:026C(DS:0FFC)`, `21FD:0329`, `54B1` = 1, di = `01B2` (Spieler mit `4BC2` = 1
  nehmen nicht teil, `0C82` setzt `4890`), Überschrift `0A69:0243(B1h, 0Ah, 8Fh, [3B6C], Farbe 8, Warten 2, −1)`, `21FD:05DB(1)`,
  `0185(50)`, `11AD`. Je Spieler mit `4890` = 1: `0A96:001B`; bei di < 46h oder `2F05` = 0 Zahl `0A96:05FB(29h, 21h·p + 22h, si, 0,
  5, 0, −1)` und Bargeld `3CD2[p]` += si (`add`/`adc 0`, also vorzeichenlos), sonst Text `0A96:0525(29h, 21h·p + 21h, [3B70], 0,
  −1)`; `21FD:060D`, Ton (0, 28h), `0185(50)`. Dann `05DB(0)`, Warteschleife `21FD:0004(4)` (Modus 4: Anzeige `1F63:113E` mit
  `random(200)`), bei di ≥ 46h und `2F05` > 0 (Byte mit Vorzeichen) `0609:0080`.
- **`11AD`:** si = 0 … 7: je Spieler mit `4890` = 1 bei geradem si `2295:006D(0Ch + p, FFFh)` und Ton (0, 29h), bei ungeradem
  `2295:006D(0Ch + p, W[1014 + 2p])` und Ton (0, 2Ah); danach `0185(15)`.
- **`0609:0080`:** `21AF:000D`; je Spieler `4890` = 1 → `43B2[p]` = 5 (freies Feld wählen) und `43FA[p]` = 1, sonst `43B2[p]` = 6
  (fertig); `1582` = 1, `0609:00D6` (Aufbau und Feldschleife `03E0` bis `21FD:0665`). Anders als `0609:0004` keine Abrechnung
  `0FA9`. Die `1582`-Zweige in `00D6` (keine Warteschleife), `032A` (Läufer sofort sichtbar), `0EA1` (Züge kosten keine Zeit) und
  `0204` sind im Nachtrag von [`0609_0004_anbau.md`](0609_0004_anbau.md) beschrieben; Modus 5 handelt `0609:0AEF`.
- **Zufall:** `0B03` und `11AD` ziehen selbst nicht. Gezogen wird nur in der Warteschleife (Modus 4 je Durchlauf `113E`, bei
  abgelaufenem Zähler `DS:54B0` `random(200)`) und in der zweiten Feldrunde (Randziehungen `0609:13C5` in `1BE5`, Computerzug
  `135D:000E`). Damit ist die Spalte „Zufall“ in BESTAND §1.4 belegt.

## Zustand

| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `4B9A` | W | s | Woche (Preis Woche·50, Wettbewerb bei Woche mod 3 = 2) | L `0B03` |
| `3CFA + 8p` | W | s | Vorrat Ware 3 | L `01B2`/`0C82` |
| `4BC2` | B[4] | u | 1 = nimmt nicht teil (hat zuletzt nicht gezahlt) | L `0C82`, `01B2` |
| `4890` | B[4] | u | 1 = Gewinner | W `0C82`; L `0B03`, `11AD`, `0609:0080` |
| `54B1` | B | u | 1 = Wettbewerb | W `0B03` |
| `3CD2 + 4p` | DW | s | Bargeld (+ Woche·50, vorzeichenlos addiert) | W `0B03` |
| `2F05` | B | s | freie Felder | L `0B03` |
| `3B6C` / `3B70` | Fernzeiger | – | Überschrift / Text statt Preis (Feld) | L `0B03` |
| `1014` | W[4] | – | rgb12 der Spielerfarben 0Ch…0Fh | L `11AD` |
| `43B2` / `43FA` / `1582` | B[4] / B[4] / B | u | Modus 5 bzw. 6 / Kaufmerker / zweite Runde | W `0609:0080` |
| `37AA` | B | s | Schleifenvariable Spieler | W |

## Seiteneffekte in Reihenfolge

Siehe Ablauf. Jedes Warten (Blenden, Seitenwechsel, `0185`, Schreibmaschine, Warteschleife, Feldrunde) ist ein Bild. Woche
mod 3 ≠ 2: kein Bild, kein Zustand.

## C#-Umsetzung

`src/Traders.Core/FatMike/ContestPhase.cs` (`Run` = `0B03`, `FlashFrames` = `11AD`, `SecondRoundFrames`/`PrepareSecondRound` =
`0609:0080`, `TypeLine` = Bilder und Ereignisse von `0A69:0243`/`01AD`), Ereignisse `TypeLine`, `Number`, `Text`, `DacColour` in
`FatMikeEvents.cs`, Host `IContestHost` (zweite Runde; `FatMikeHost` mit `ContestFieldHost` als Feld-Host), Handler
`ContestStep` (Aufruf 30; eingebunden von Spur `woche`). Gerufen statt nachgebaut: `FatMikePhase.SceneFrames`/`ScaleFrames`,
`Planting.SetupFrames`/`PassFrames`/`Flip`, Warteschleife `Week.WaitLoop` (Modus 4).

## Prüfung

`tests/Traders.Tests/FatMike/ContestDifferentialTests.cs`: `11AD` (Gewinnermasken auch ≠ 0/1, beliebige Farben), `0B03` aus
LoadOnly-Zuständen (Wochen mit/ohne Wettbewerb, negative und große Wochen mit Preis ≥ 8000h, Vorräte um 70 und an den
16-Bit-Grenzen, Gleichstand, freie Felder 0/positiv/negativ, Texte null oder gesetzt; `0609:0080` ersetzt und als Aufruf
verglichen). Aus echten Partien (`FatMikeDifferentialTests.Fat_Mike_in_real_games_matches_the_original`, Woche 2): `0B03` ganz
mit der zweiten Feldrunde (Computer und Feldhelfer im Port aus Spur `anbau-ki`, im Original `135D`), Menschen per Skript,
Vorräte und `4BC2` verändert; Feldschleife nach höchstens 400 Durchgängen beidseitig beendet. Verglichen: Datensegment, `DS:10D2`,
Zahl der `random()`-Aufrufe, Aufruffolge mit Argumenten. Bildgenau (Bildzähler, Szene): Teil 3.
