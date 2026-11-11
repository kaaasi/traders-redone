# Grafiksystem von T.EXE

Stand: 11.11.2026 (Analyse aus `dev/ap2-vga` vom 25.10.2026, übernommen von Spur ② `grafik`) · Quelle: Disassemblierung
`0A96`, `0A69`, `23CD` (VGA-Teil), `21FD`, `186C`. Befunde zu den portierten Treiberroutinen: `docs/routines/23CD_*.md`.
Adressen = Ladebild-Segment:Offset, DS = DGROUP `0x28DC`.

## 1. Schichten

```
Spiel-Code ──► 0A69 (Hilfen: Rahmen, zentrierter Text, Schatten-Text, Tipp-Text)
          └──► 0A96 (Befehlsliste: Einträge sammeln, mit 0A96:06A8 ausführen)
                   └──► 23CD Sprungleiste 05C3…06E3 ──► Treibervektoren CS:0004…004C
                                                        (VGA: Mode X, 23CD:30D1…3DE4)
21FD:026C…04DA  Palette: Basispalette, Blende, DAC über 2295:00C5
23CD:06E9/071C   Sprite-System (8 Objekte, Hintergrund sichern/zurückschreiben)
```

## 2. Befehlsliste `0A96`

- Zeiger auf nächsten Eintrag: far `DS:15BA/15BC`, Zähler `DS:15BE`, Liste ab `DS:15C0`.
- Eintrag 0x1C Byte; Typ als Wort bei `+0x1A`. `0A96:001B` = Liste leeren,
  `0A96:06A8` = alle Einträge über Sprungtabelle `DS:01F4` (17 Far-Pointer) ausführen und
  Liste leeren.
- `0A96:000A(a,b)`: Bytes `DS:0142` (Zeichenabstand), `DS:0143` (Schattenversatz) für Text.

| Typ | Einreihen | Felder | Ausführen | Wirkung |
|---:|---|---|---|---|
| 0 | `0030` | sx,sy,w,h,dx,dy | `06E4` → `23CD:06A0` (`32D8`) | Blit RAM-Atlas → Seite |
| 1 | `0089` | wie 0 | `0720` → `06AF` (`3449`) | Kopie RAM-Atlas → RAM-Atlas |
| 2 | `00E2` | wie 0 | `075C` → `06B4` (`351E`) | Kopie Seite → Seite (byteweise je Ebene) |
| 3 | `013B` | sx,sy,mx,my,w,h,dx,dy | `0798` → `06A5` (`35DF`) | maskierter Blit (Maske aus `MASKS`) |
| 4 | `01AA` | mx,my,w,h,dx,dy,Farbe | `07E4` → `06E3` (`3CE8`) | Maske in Farbe füllen |
| 5 | `020E` | wie 0 | `0848` → `06AA` (`3386`) | OR-Blit (liest Ziel über Read-Map) |
| 6 | `0267` | sx,sy,tw,th,dx,dy,w,h | `0884` | Kachelung aus Typ-0-Blits |
| 7/8/16 | `0311`/`035F`/`03AD` | sx,sy,h,dx,dy | `094F`/`0987`/`09BF` → `06D4` (`3A9A`) | Blit fester Breite 8/16/32 |
| 9 | `0433` | x1,y1,x2,y2,Farbe | `09F7` | Linie |
| 10 | `0481` | x,y,w,h,Farbe | `0A51` | Rechteckrahmen (4 Linien) |
| 11 | `04D7` | x,y,w,h,Farbe | `0B20` → `069B` (`31A6`) | gefülltes Rechteck |
| 12 | `0525` | x,y,far str,Farbe,Schatten | `0B7A` → `0C68` | Text |
| 13 | `057A`/`05FB` | x,y,Zahl(long),Stellen,Farbe,Schatten | `0BB2` → `0C68` | Zahl (über `2295:0001`), `05FB` mit „$“ |
| 14 | `0686` | Muster | `0BE9` → `05D0` | Linienmuster setzen (`CS:04F4`, `FFFF` = durchgezogen) |
| 15 | `03FB` | x,y,Farbe | `0BFB` → `06D9` (`373B`) | Pixel |

`0A96:02D6`: fünf Typ-7-Blits nebeneinander (Breite 40). Farben laufen bei EGA
(`DS:090E` ≠ 1) über die Tabelle `DS:0154 + 16·[DS:09C8]`; bei VGA (`090E` = 1) unverändert.

Text `0A96:0C68(x,y,str,Farbe,Schatten)`: Schatten ≥ 0 → zuerst Rechteck
(`x`, `y−[0143]+1`, `strlen·[0142]`, `[0143]`) in Schattenfarbe, dann Text in Farbe über
`23CD:05DD`.

## 3. Treiber `23CD`

- `23CD:04F6(Modus)`: 0 → `0797` (EGA 0Dh), 1 → `3042` (**VGA**), 2 → `1645`, 0x63 → `3F0A`.
  Jeder Teil kopiert 19 Far-Vektoren nach `CS:0004` (`23CD:053C`).
- VGA-Init `3042`: Modus 13h, dann Chain-4 aus (SEQ 4), CRTC 14/17 → **Mode X**
  (planar, 80 Byte je Zeile und Ebene, Seiten über Segment).
- Zustand im Codesegment: `CS:0000/0002` Zielseite (Offset/Segment, `311A` setzt),
  `CS:0586` Schrift-Nr., `CS:0588` Farbe (`05C3`), `CS:04F4` Linienmuster,
  `CS:057A` Schrifttabelle, `CS:0582` RAM-Atlas, `CS:057E` Maskenatlas (`058A` setzt).
- RAM-Atlas: Zeile = 320 Byte **ebenengetrennt** (Ebene k bei `k·80`), Basis +4
  (Kopf übersprungen). Umwandlung beim Laden: `23CD:3079(buf+4, 320, 256)`.
  Adresse (x,y) = Basis + 4 + y·320 + x/4 (+ Ebene·80).
- Masken: 1 Bit/Pixel, 40 Byte je Zeile, MSB links; Adresse = Basis + y·40 + x/8
  (untere 3 Bit von x werden ignoriert!).

| Vektor | VGA | Aufgabe |
|---|---|---|
| 04 | `30D1` | GC-Register zurücksetzen |
| 08 | `30E9` | Anzeigestart setzen (Seitenumschaltung) |
| 0C | `31FD` | Zeichen 16 px aus Schrift zeichnen |
| 10 | `31A6` | Rechteck füllen |
| 14 | `32D8` | Blit RAM → Seite |
| 18 | `328E` | Latch-Kopie VRAM → VRAM (4-Pixel-Einheiten) |
| 1C / 4C | `378B` / `38F2` | Linie durchgezogen / mit Muster |
| 20 | `3A9A` | Blit Breite = Arg·8 |
| 24 / 28 | `3AAB` / `3C42` | Sprite zeichnen / Hintergrund zurück |
| 2C | `35DF` | maskierter Blit |
| 30 / 34 | `373B` / `3764` | Pixel setzen / lesen |
| 38 | `3386` | OR-Blit |
| 3C | `3449` | RAM → RAM |
| 40 | `351E` | Seite → Seite |
| 44 | `311A` | Zielseite setzen |
| 48 | `3CE8` | Maske einfärben |

Die Routinen haben viele Ausrichtungs-Eigenheiten (Ebenen-Rotation je nach `x&3` von
Quelle und Ziel, Breitenaufteilung `w/4` + Rest). Port daher **wörtlich** gegen ein
emuliertes Mode-X-VRAM (4 × 64 KB, Map-Mask, Read-Map, Latches).

## 4. Sprite-System

8 Objekte à 0x1A Byte ab `DS:330C` (Initialisierung `21AF:000D`):
`+0` aktiv, `+2` Höhe, `+4` far → Bildzeiger je Frame, `+8` far → Maskenzeiger je Frame,
`+0C` far → Sicherungspuffer je Seite, `+10` far → Restore-Datensatz je Seite
(`di`, `x&3`, gültig), `+14/+16` x/y, `+18` Frame. Breite fest 16 px.
`23CD:06E9(Seite)`: aktive Objekte 0…7 sichern+zeichnen; `23CD:071C(Seite)`: alle Objekte
7…0 zurückschreiben (nur gültige Datensätze).

## 5. Palette und Blende (`21FD`)

- Geladene Palette: `DS:5017` (768 B, aus `186C:0002`), Basispalette `DS:3DCB`,
  Arbeitspalette `DS:54E8`, Blendfarbe `DS:10D8` (12-Bit-RGB).
- `026C`: Basis ← geladen. `02C9(i, rgb12)`: Basis[i] = (R·8, G·8, B·8) als Byte.
- `03EB(rgb12, n)`: je Kanal `c` der Basis, Ziel `t`: `c>t` → `c−8n`, nicht unter `t`;
  sonst `c+8n`, nicht über `t` — **Vergleiche vorzeichenbehaftet (Byte)**. Ergebnis nach
  `54E8`, bei VGA `2295:00C5` (Strahlrücklauf abwarten `0D73:00D3`, dann DAC-Block).
- Einblenden `0329`/`0353`: n = 7…0, je Stufe 3 bzw. 1 Tick warten (`21FD:0185`).
  Ausblenden `037D`/`03B4`(Farbe): n = 0…7, Wartezeit 4 bzw. 2, dann Blendfarbe setzen und
  `21AF:04CD`.

## 6. Ressourcenformat

`V-*.PP` = Palette (768) + Kopf (LE-Wörter Höhe 256, Breite 320) + Atlas ab Byte 772. `Traders.Assets.VgaImage`
berücksichtigt den Kopf auf `main`; `PlanarAtlas` übernimmt den Kopf als die ersten 4 Byte des Treiberpuffers.

## 7. Portierungsstand (Spur ②)

- Portiert mit Differenztest: alle 19 Vektoren; Text `05DD`, Setzer `05B6`/`05C3`/`05D0`, Linienauswahl `06BE`,
  Objektschleifen `06E9`/`071C`, Atlas-Umwandlung `3079`, Kartenwahl `04F6` (nur Modus 1) mit VGA-Init `3042`.
- Grenzfälle (Ränder ohne Abschneiden, kleine Breiten, `h` = 0, extreme Linien, Seitenwechsel) in `DriverEdgeCaseTests`.
- Nicht in ②: EGA (`0797`), Tandy (`1645`, `3F0A`), Umwandlung `166A` (nur VGA); `0A96`, `0A69`, `0B67`, `21AF`,
  Paletten/Blenden und Farbregister → R3.

## 8. Grafikschichten (Spur `schichten`, R3)

- **Teil 1** (portiert mit Differenztest, `src/Traders.Vga/Layers/`): Seitenverwaltung `0B67` (`0002` nur VGA, `016B`, `0196`,
  `01B5`, `01D4`, `0206`, `0238`, `028F`, `0299`), Paletten und Blenden `21FD:026C…04DA` mit Warten `0185`, Seitenwechsel
  `21FD:051F`, Farbregister `2295:006D/00C5`, Rückschreibsätze `21AF:04A2/04CD`. Befunde: `docs/routines/0B67_0002_seiten.md`,
  `21FD_026C_paletten.md`, `21FD_051F_seitenwechsel.md`, `2295_006D_farbregister.md`, `21AF_000D_objekte.md`; DS-Adressen in
  `DATENLAYOUT.md` (Unterabschnitt „Grafikschichten“).
- Die Schichten arbeiten auf dem DS-Abbild an den Originaloffsets; DAC als `VgaDac` (6 Bit je Kanal). Jedes Warten auf den
  Strahlrücklauf (`0D73:00D3`) ist genau ein Aufruf von `IRetraceWait` – keine eigene Uhr; die Tests zählen die Warten gegen den RefEmu.
- Korrektur zu §5: In `03EB` sind auch Ergebnisse jenseits des Ziels möglich (Schritt als Byte, Vergleich vorzeichenbehaftet:
  Stufe 16 ergibt Schritt 80h). Ausblenden ruft zusätzlich `21AF:04CD` (Rückschreibsätze `DS:33E4` nullen), zeichnet aber keine Objekte.
- **Teil 2** (portiert mit Differenztest): Befehlsliste `0A96` (alle 17 Typen, Einreihen, `06A8`, Text `0C68`, `0C3F`), Zahl
  `2295:0001/0062`, Hilfen `0A69` einschließlich Schreibmaschinentext `01AD/0243/0288` (Tastenklick und Zeitgeber-Warten als
  Ereignisse `ITypewriterEffects`). Befunde `docs/routines/0A96_06A8_befehlsliste.md`, `0A69_000B_hilfen.md`, `2295_0001_zahl.md`.
- Korrektur zu §2: `06A8` leert die Liste **nicht**; Zähler und Zeiger stehen danach wieder hinter dem letzten Eintrag, ein zweiter
  Aufruf zeichnet dieselben Einträge erneut (`21FD:060D`). Leeren nur `001B`. Zahlen mit mehr als 7 (`$`: 6) Stellen überschreiben
  das Typwort ihres Eintrags.
- **Teil 3** (portiert mit Differenztest): Objektverwaltung `21AF` (`000D`, `00CA`, `022B`, `0412`, `045E`, `0472`) und die Brücke
  von den DS-Sätzen zu `23CD:06E9/071C` (`GraphicsLayers.DrawObjects/RestoreObjects`). Befund `docs/routines/21AF_000D_objekte.md`.
  Korrektur zu §4: +10 zeigt direkt auf die Rückschreibsätze (4 Byte je Seite, ab `DS:33E4 + 8·i`), nicht auf Zeiger; Bildzeiger
  (VGA) werden über `0000:5101` normalisiert; Sicherungspuffer liegen im RAM-Atlas.
- **P3 #60** (maskierter Blit `35DF` bzw. Maske füllen `3CE8` mit w < 4 und ungeradem Maskenoffset): statisch nicht erreichbar.
  Einzige Aufrufer sind `0A96:0798/07E4` (Typ 3/4); die zehn Einreihaufrufe von `0A96:013B/01AA` (`0535`, `0609`, `0EDE`, `192D`,
  `1C68` zweimal, `1DEF` zweimal, `20B3`, `1F63`-Grenze) übergeben alle feste Breiten 7…59 (7, 8, 0Fh, 10h, 20h, 38h, 3Bh).
  Aufrufe über Funktionszeiger auf `013B/01AA` gibt es nicht (Suche nach den Adressen in der Disassemblierung).

