# 0DFD:000D / 0DFD:0074 / 0DFD:0132 / 0DFD:0158 / 0DFD:0239 – Programmstart

- **Ladebild-Adresse:** `0DFD:000D` (Dateioffset `0x115DD`); Segment `0DFD` ab `0x115D0`.
- **Aufgabe:** einmal nach der Initialisierung: Puffer anlegen, Seiten und Objekte einrichten, Schriften, Masken und Texte
  laden, Ladebild mit Musik zeigen, sechs Bilder vorladen, dann die Code-Abfrage `21FD:0687`.
- **Aufgerufen von / Phase:** `main` `1777:0126` (Programmschritt 2, [1777_000C_main.md](1777_000C_main.md)).
- **Parameter / Rückgabe:** keine.

## Ablauf (eigene Worte)
1. `0074`: vier Puffer über `186C:02C6` (VGA: Atlas 14314h → `DS:45D6`, dann 1F50h → `4BC6`, 11A4h → `4884`, 876h → `3CAA`);
   Seitennummern `DS:40DE…40E4` = 1, 0, 2, 0; `0B67:0238` mit den Seiten zu `DS:082E`/`0830`.
2. `0132`: Seitenwechsel `21FD:051F`; Zeigerkopien `DS:44DE` ← `4BC6`, `DS:45BC` ← `45D6`.
3. `21AF:000D`: Objektsätze `DS:330C` ([21AF_000D_objekte.md](21AF_000D_objekte.md)).
4. `0158`: Datei `DS:0681` über `186C:0AB1`/`01A9` nach `3CAA`; drei Schriftsätze `DS:2BF0 + 10·i` (Zeiger in den Puffer +
   `2·[0660 + 2i]`, Breiten/Höhen aus `DS:0666/066C/0672`), `23CD:058A`, `23CD:05B6(0)`.
5. `21FD:05DB(0)`: Schrift 0, Vorschub `DS:3CA8` = `[10DA]`, Zeilenhöhe `DS:34A4` = `[10DD]`, `0A96:000A` (`DS:0142/0143`).
6. `192D:18B6`: Textzeigertabelle `DS:37CC…3C8F` (Spur `menue`).
7. Datei `DS:0678` über `186C:0AB1`/`01A9` nach `4BC6` (Masken).
8. `0239`: Objekte aus (`21FD:03B4(0)`), drei Grundfarben (`21FD:02C9`), Ladebildtexte (`0A69:0090`, `21FD:060D`), Einblenden,
   Musik `218B:0094(0, 0)` (schreibt `DS:10A5`, `DS:10A9`), dann `186C:0002` für die Bildnummern `DS:08CE, 08D0, 08D6, 08D8,
   08D4, 08D2` (in dieser Reihenfolge).
9. `21FD:0687`: Code-Abfrage ([21FD_0687_codeabfrage.md](21FD_0687_codeabfrage.md)).

## Zustand
Schreibzugriffe aus dem Probelauf (Datensegment je Unteraufruf verglichen, Schreiber per Haltepunkt bestimmt), Zuordnung im Port:

| DS-Offset | Breite | Bedeutung | Schreiber | Port |
|---|---|---|---|---|
| `09C8`, `09CE…0A8B` | W, 13h·10 | Bildtabelle, zuletzt geladenes Bild | `186C:0002/0997` | Kern `ImageTable` |
| `10D2` | 32 Bit | Zufalls-Seed (Neusaat, Ziehungen) | `21FD:01BA` | Kern |
| `10FF…1124` | Texte | entschlüsselte Abfragetexte, Code | `21FD:081D`, `0687` | Kern `CodeQuery` |
| `5334` | W | Tafeloberkante | `192D:16AF` | Kern `MenuTextPlaque` (über `MenuBackdrop` aus Spur `menue`) |
| `37CC…3C8F` | Zeiger | Texttabelle | `192D:18B6` | Kern `MenuTexts.Build` im Programmstart |
| `4884` | Far | Textpuffer (Heap, `0074`) | `0DFD:0074` | Kern, Adresse als Host-Eingabe (`TextFile`) |
| `0142`, `34A4`, `3CA8`, `2BF0…2C0D` | – | Schrift, Farben, Schriftsätze (mit Pufferzeigern) | `0158`, `05DB` | Ausgabe (`Traders.Vga`) |
| `0238`, `082E…0831`, `40DE…40E5`, `4516` | – | Seiten | `0B67`, `051F`, `0074` | Ausgabe (`Traders.Vga`) |
| `330C…33DB` | – | Objektsätze (Zeiger) | `21AF:000D` | Ausgabe (`Traders.Vga`) |
| `3DCB`, `5017`, `54E8` (je 300h), `10D8` | – | Paletten, Mischfarbe | `21FD:026C…04DA`, `186C:0002` | Ausgabe (`Traders.Vga`) |
| `15BA…1FB3` | – | Zeichenliste | `0A96` | Ausgabe (`Traders.Vga`) |
| `3CAA`, `44DE`, `45BC`, `45D6`, `4BC6`, `5318…532F`, `4C00…4D12` | – | Heap-Zeiger, Heap, Bildcache | `186C` | Host |
| `128A`, `1300…1307`, `5894…589B` | – | C-Laufzeit: Heap, Dateitabelle, `tzname` | `0000:2B14`, `32CC`, `4299` | Host |
| `10A5`, `10A9` | W | Musik läuft, aktuelles Stück | `218B:0094` | Ton (R10) |
| `1158`, `115D` | B | letzter Make-Code, Zähler | int 9 `27C6:006F/0073` | Eingabe (F1 = B) |

## Seiteneffekte in Reihenfolge
Dateien (Schriften, Masken, Texte, Bilder, Musik), Grafik (Ladebild, Paletten), Ton; **Zufall** nur in der Code-Abfrage:
`random(0)`, `random(3FFh)` und die Ziehungen der Texttafel `192D:16AF` (Mauer, Nieten); Uhr nur über `random(0)`.

## C#-Umsetzung
`src/Traders.Core/Startup/ProgramStart.cs` (Programmschritt 2), `ImageTable.cs`, `CodeQuery.cs`; Dateilängen über
`src/Traders.Assets/Loading/ImageFiles.cs` (Host-Eingabe). Texttabelle `192D:18B6` (`MenuTexts.Build`, Spur `menue`) auf dem
Textpuffer, dessen Far-Adresse `DS:4884` der Host vorgibt (`TextFile`; `[APPROX]` Heap des Hosts); Texttafel der Abfrage
`MenuTextPlaque` (`192D:16AF` über `MenuBackdrop`). `[APPROX]`: Anzeige-, Datei-, Heap- und Tonteile oben laufen nicht im Kern
(Tabelle „Port“); die Anzeigebytes führt `Traders.Vga` in eigenem Zustand (Entscheidung Leitstelle 12.11.2026), der Host
(`src/Traders.App/ProgramHost.cs`) richtet davon Seiten (`0B67:0002`), Schriften und Masken ein. Ladebild und Abfragebild
zeigt der Port nicht (F1 = B).

## Prüfung
`tests/Traders.Tests/Startup/ProgramStartDifferentialTests.cs` (`[DIFFERENTIAL] 0DFD:000D`): Gleichschritt Schritt 1 → 2 → Aufrufstelle 3
über `RefEmuProgramOracle.PortHandler`, ganzes Datensegment nach der Rückkehr verglichen außer `OutputOwned` (Tabelle oben);
vier Eingaben (1111, Ziffernblock, falsche Ziffern, Buchstaben und Minus zuerst), zwei Uhrwerte (beide Spaltenformen).
Gegenproben: ohne Neusaat rot (`DS:10D2`); lokal geprüft: ohne Entschlüsselung rot `DS:10FF`, falsche Länge rot `DS:09DD`,
falsches `09C8` rot. Seit Teil 3 auch Texttabelle, `DS:4884`, `DS:5334` und der Textpuffer (gegen den Heap des Originals) im
Vergleich; `ProgramHumanDifferentialTests` führt den Programmstart im ganzen Programmlauf (s. [1777_000C_main.md](1777_000C_main.md)).
Nicht verglichen: die Anzeigebytes von `Traders.Vga` (Abbildung auf die DS-Adressen erst mit einem Port von `0074`/`0132`/`0158`/`21AF:000D`).
