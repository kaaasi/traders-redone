# Review r3-schichten (PR #68)

- **Geprüft:** Head `df7d406d1bc7db544cdb2bd00a03dad1c4020258` (Base `7a645ffbd9bc08d85c5bb8088a7fe78efe8989c5` = aktueller `main`),
  Checkout verifiziert (`git rev-parse HEAD` = Head, `git status --porcelain` leer), Originaldaten aus `kaaasi/traders-referenz`
  `7fcfb24` (Hashes geprüft), Disassemblierung `analyse/disasm` aus demselben Stand.
- **Reviewer:** Claude Opus 5.5, geplanter Lauf in eigener Sitzung ohne Implementierungskontext.

## 1. Merge-Empfehlung

**Ja** für genau diesen Head. Alle drei Läufe von `tools/pruefen.sh` sind grün, die CI (`linux`) ist auf dem Head grün, und die
Base ist der aktuelle `main`. Jeder neue Differenztest holt seine Erwartungswerte aus dem RefEmu: ganzes DS (mit `DS:10D2`),
`Vga.Dac`, `Vga.Planes`, CRTC-Start, Zeichenseite des Treibers und die Rücklaufwarten am Einsprung `0D73:00D3`. Der Port
liefert nur die zweite Seite des Vergleichs. Von 32 Rückmutationen werden 28 rot. Von den vier überlebenden sind zwei gleichwertig zum
Original, die anderen beiden belegen B1. P3 #60 ist mit dem Bytebeleg unten bestätigt. Es bleiben nur P3.

Zu den drei Schwerpunkten:

1. **Befehlsliste, Hilfen, Paletten, Blenden, `0185`, `051F`, `2295:*`:** Im Test `List_with_all_types` werden alle 17
   Ausführenden (Tabelle `DS:01F4`) schon in Runde 0 angesprungen, bei beiden Seeds (300, 301) und in Modus 1 wie 0. Belegt mit
   Code-Hooks an allen 17 Tabelleneinträgen. VRAM und DAC werden gegen den `VgaAdapter` verglichen, nirgends gegen Port-Werte.
2. **`0B67:0002` mit ersetztem Allokator:** Der Stub verfälscht den Vergleich nicht. `farmalloc` (`186C:02C6`) wird mit
   `pop cx; pop cx` aufgeräumt, `retf` passt also. Die Nullprüfung `or ax,[2BAE]` prüft beide Wörter, genau wie
   `allocatedBuffer == 0`. Was der Stub entfernt (Heapzustand), lässt der Port bewusst beim Aufrufer. Ungeprüft bleibt nur die
   Größe 500h, die der Stub nicht liest; der Port hat dafür keinen Parameter. Der Test ist aber für die beiden Füllungen blind
   (B1).
3. **P3 #60 / kein Spielzustand, kein Zufall:** Die Aussage in GRAFIK.md §8 trifft in der Sache zu. Die Zählung im Text ist
   falsch (B2). `Traders.Vga` referenziert `Traders.Core` nicht, kann also kein `OriginalRandom` nutzen. In den
   Originalsegmenten `0A96`, `0A69`, `0B67`, `21AF`, `2295` gibt es keinen Aufruf von `random` (`21FD:01BA`). Weil das ganze DS
   samt `DS:10D2` verglichen wird, würde jede Zustandsänderung im Original auffallen: die Probe MR (`Ds[0x10D2]++` in
   `ExecuteList`) macht `List_with_all_types` rot (2/2).

## 2. Befunde

**B1 (P3): `InitializePages_0B67_0002_matches_original` erkennt die beiden Seitenfüllungen nicht.**
[PageLayerTests.cs#L99](https://github.com/kaaasi/traders-redone/blob/df7d406d1bc7db544cdb2bd00a03dad1c4020258/tests/Traders.Tests/Vga/PageLayerTests.cs#L99):
`0002` ruft zuerst `23CD:04F6(1)` auf, also Modus 13h. Dieser Aufruf löscht im RefEmu wie im Port alle vier Ebenen
([ModeXSetup.cs#L31](https://github.com/kaaasi/traders-redone/blob/df7d406d1bc7db544cdb2bd00a03dad1c4020258/src/Traders.Vga/ModeXSetup.cs#L31)).
Die anschließenden Füllungen in Farbe 0 (`23CD:069B(0,0,320,200)` auf der gezeigten und der verdeckten Seite) hinterlassen deshalb
keine sichtbare Spur. Auch die Prüfung auf Nullen in
[#L115](https://github.com/kaaasi/traders-redone/blob/df7d406d1bc7db544cdb2bd00a03dad1c4020258/tests/Traders.Tests/Vga/PageLayerTests.cs#L115)
ist ohne die Füllungen wahr.
Beleg: M4b (zweite `FillRect` in `InitializePages` entfernt) und M4c (beide entfernt) bleiben grün (3/3).
Laut Disassemblierung (`0B67:010D…0149`) setzt der Port die Füllungen richtig um. Im Port ändern sie nichts, weil
`InitializeVga` ebenfalls löscht. Auf echter Hardware löscht das BIOS in Chain-4, dort können sie sichtbar sein (`[APPROX]` in
`ModeXSetup.cs`). Deshalb P3.
Vorschlag: im RefEmu einen Hook am Rücksprung von `04F6` (`0B67:003B`) setzen und VRAM auf beiden Seiten gleich zufällig füllen.
Das geht, wenn der Port die Füllungen über eine Prüfnaht zugänglich macht. Alternativ die Aufrufe von `23CD:069B` mit Farbe und
Seite protokollieren und gegen den Port vergleichen.

**B2 (P3): GRAFIK.md §8 zählt zehn Einreihaufrufe, im Binär stehen neun.**
[GRAFIK.md#L145](https://github.com/kaaasi/traders-redone/blob/df7d406d1bc7db544cdb2bd00a03dad1c4020258/docs/analyse/GRAFIK.md#L145):
„`1DEF` zweimal“ stimmt nicht. Die Bytesuche über das ganze `T.EXE` findet `9A 3B 01 96 0A` sechsmal (`0609:1846`, `0EDE:1FB9`,
`192D:14F1`, `1DEF:03BF`, `1F63:0004` (Funktion beginnt in `1E7E`), `20B3:022E`) und `9A AA 01 96 0A` dreimal (`0535:08E1`,
`1C68:0E31`, `1C68:0E5B`).
Folgende Referenzen gibt es nicht:
- keine weiteren Far-Zeiger auf `013B`/`01AA`/`06A5`/`06E3`;
- keine Near-Aufrufe in `0A96` auf `013B`, `01AA`, `0798` oder `07E4`;
- keine Verweise auf `35DF`/`3CE8` in `23CD` außer der Vektorbelegung. Der Treffer bei `23CD:0F07` liegt mitten in einem Befehl
  und zählt nicht.

`0798`/`07E4` stehen nur in der Tabelle `DS:0200/0204`. Die übergebenen Breiten habe ich an allen neun Stellen gelesen: 20h, 10h,
7, 10h, 8, 3Bh (`013B`) und 38h, 0Fh, 0Fh (`01AA`), alle Konstanten ≥ 4. Das Ergebnis „statisch nicht erreichbar“ bleibt
gültig. Die Liste in §8 sollte auf neun Stellen korrigiert werden.
Beleg: Probe P60 in Teil 3.

**B3 (P3, nur Testpflege):**
- Laut Kommentar von `Frames_with_line_pattern_match_original`
  ([CommandListTests.cs#L332](https://github.com/kaaasi/traders-redone/blob/df7d406d1bc7db544cdb2bd00a03dad1c4020258/tests/Traders.Tests/Vga/CommandListTests.cs#L332))
  ist „die Richtung jeder der vier Linien sichtbar“. Für die Waagerechten stimmt das nicht: der Treiber zeichnet sie immer ab dem
  kleineren x (`ModeXLines.cs#L65`, Original `378B/38F2`). Beleg: M18 (untere Linie umgedreht) bleibt grün und ist gleichwertig;
  M18b (linke Senkrechte umgedreht) wird rot.
- `AddRandom` wird mit `h.Rng.Next(20)` aufgerufen
  ([#L211](https://github.com/kaaasi/traders-redone/blob/df7d406d1bc7db544cdb2bd00a03dad1c4020258/tests/Traders.Tests/Vga/CommandListTests.cs#L211)).
  Für Art 19 gibt es keinen Zweig, also läuft rund jeder zwanzigste Zug leer. Das ist harmlos.
- M19 (`index << 1` in `CopyGamePage`) bleibt mit den Seitennummern (1, 2) gleichwertig, weil `0299` jede Richtung ≠ 1 gleich
  behandelt. M19b (`index ^ 1`) wird rot.

Keine P0/P1/P2. Issues für B1–B2 (Label `befund`, `P3`) legt die Leitstelle an, weil dieser Lauf keine Issues anlegen durfte.

## 3. Ausgeführte Prüfungen

`tools/claude/fetch-reference.sh original` und `disasm`, `tools/claude/setup-workspace.sh`, dann `tools/pruefen.sh` (alle drei
Läufe, Ergebnis grün, Architektur und Abdeckung geprüft):

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| Mit Originaldaten (TR_REQUIRE_GAMEDATA=1) | 479 | 0 | 0 |
| Ohne Originaldaten (TR_SKIP_GAMEDATA=1) | 363 | 0 | 95 |
| Pflichtlauf ohne Daten (muss scheitern) | 363 | 116 | 0 |

Markerbericht: `Traders.Vga` 102 `[ORIGINAL]`, 4 `[APPROX]` (unverändert gegenüber `main`). CI auf dem Head: `linux` success
(Lauf 43064655256).

**Rückmutationen** (je eine Ersetzung im Port, gefilterter Lauf mit Daten, danach zurückgesetzt):
- **rot (28):** M1 `ShowPage1` → 2; M2 `SelectPages` nur 1; M3 `CopyPage` Richtung ≠ 0; M4 `PageBuffer`-Hochwort fehlt;
  M5 `051F` 3 statt 4 Warten; M6 `006D` ohne VGA-Prüfung; M7 `00C5` wartet nur bei VGA; M8 `026C` 767 Byte; M9 `02C9` Adresse
  & 7FFFh; M10 `03EB` vorzeichenlos; M11 Einblenden ohne Stufe 0; M12 Ausblenden ohne `04CD`; M13 `04A2` nur 4 Byte; M14 Kacheln
  ohne Kürzung; M15 `07E4` ohne Farbabbildung (Modus 0 rot); M16 `2295:0001` Füllgrenze; M17 Schattenrechteck y − h; M18b; M19b;
  M20 Zentrierung vorzeichenbehaftet; M21 Klick auch bei Leerzeichen; M21b Zeilenabstand h/4; M22 Rückschreibsatz 4·i; M23
  `000D` aktiv = 1; M24 Tandy x/4; M25 `5101` ohne Übertrag; M26 Maskenzeiger mx/8; MR `DS:10D2` (Zufallszustand).
- **grün (4):** M4b, M4c (B1), M18 und M19 (gleichwertig, B3).

**Typabdeckung:** temporäre Hooks an den 17 Zeigern aus `DS:01F4` in `List_with_all_types`. Ergebnis: Typen 0…16 bei Seed 300
und 301 jeweils schon in Runde 0.

**Probe P60** (`python3 -I`, `T.EXE` mit Kopf 3600h): Suche nach `3B01960A`, `AA01960A`, `A506CD23`, `E306CD23`, `9807960A`,
`E407960A` über die ganze Datei. Dazu Near-`E8`/`E9`-Ziele in `0A96` (`0xDF60`, `0xD10`) und `23CD` (`0x272D0`, `0x3F90`) sowie die
Wörter `35DF`/`3CE8` in `23CD`. Ergebnis wie in B2. Die Breiten habe ich mit `ndisasm` an jeder Stelle gelesen, für
`1F63:0004` ab linear `1F5F4`.

**Fundstellen gelesen:** `0B67:0002…015A` (Modustabelle `CS:015B`, VGA `007F`, `farmalloc(500h)`, Nullprüfung, Füllungen),
`0A96:0798/07E4` (Weitergabe an `23CD:06A5/06E3`, Vektoren `CS:2C/48`).

**Akzeptierte `[APPROX]`:** keine neuen in diesem PR.
**Annahmen (nicht ausgeführt):** Windows-Lauf. Pfade außerhalb von VGA (EGA/Tandy) prüfen die Tests nur als Zeigerarithmetik
bzw. Farbabbildung; ausgeführt wird nur der VGA-Treiber.
