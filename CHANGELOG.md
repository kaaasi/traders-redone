# Zwischenstände

## Runde 8 – Fat Mike, Wettbewerb, Einkauf, ganze Woche im Host (17.–22.11.2026) · letzte Spur `4aa1566`, Gesamtreview offen

- Prozess: Plan (PR #214, Plan-Review „Nein“ R8PL-1–5 eingearbeitet, Delta-Prüfung Leitstelle).
- Spur 3 `sammel` (PR #216): Alias-Regel im Architekturcheck (#193), Handel Wochen 1/3 Bild für Bild (#205), Host-Bildtest
  mit Roboter-Schalter und Zusatzphase (#208).
- Spur 1 `fatmike` (PR #217): Fat Mike `1F63:000F` und Wettbewerb `1F63:0B03` samt zweiter Feldrunde im Kern und als Szene.
- Spur 2 `einkauf` (PR #218): Einkauf `08CF:0004` (41 Routinen) Bild für Bild, Szene pixelgenau; Grenzfall Bargeld = Preis (R8E-1).
- `zeitlimit` (PR #223): Zeitlimit 20 min je Test in `tools/pruefen.sh` und CI, Abbruch gilt in jedem Lauf als Fehler; Regel
  Machbarkeitsprüfung/Abbruchzähler für Differenztests (Anlass: Endlosschleife im Testaufbau von #217); #228.
- Spur 0 `woche` (PR #230): Aufrufe 26, 29, 30 eingebunden, Mehrrundenprobe Wochen 0–4, Host spielt Woche für Woche; Rechenbudget
  im RefEmu (#222).
- Prozess: Nachtrag (PR #224, Aufräumen vor v1.0 im Fahrplan). 1204 Tests grün mit Originaldaten.

## Runde 7 – Endprüfung, Handel, Handels-KI, Zusatzphase (16.–17.11.2026) · letzte Spur `b335163`, Abschluss nach Gesamtreview „Ja“

- Spur 2 `handelski` (PR #184): Handels-KI `141D` (17 Routinen) mit Vertrag für den Handel, Gleichschritt Wochen 0–3; #163,
  #165, #166.
- Spur 0 `ende` Teil 1 (PR #188): Endprüfung `20B3:0005` bis zur Entscheidung (32 Bit vorzeichenlos, R7P-1/-2), Host-Schalter
  `RobotPhaseSkip` für Aufruf 18 (abschaltbar `--original-roboter`, Nutzer 16.11.2026); #157, #158, #180.
- Spur 3 `zusatz` (PR #187): Zusatzphase `1DEF:000C` im Kern und als Szene; #162.
- Fix #189 (PR #199): Reihenfolge Wochenereignis → Stationsphase im Host.
- Spur 1 `handel` (PR #186): Handel `0B9F:0002`/`0C29` Bild für Bild mit KI-Anbindung (Stapelwort von `0397`), Szene pixelgenau; #192.
- Spur 0 `ende` Teil 3 (PR #206): Aufrufe 23/24 eingebunden, Mehrrundenprobe Wochen 0–4, Host bis Fat Mike; #194, #201.
- Prozess: Plan (PR #182, Plan-Review „Ja“ mit R7P-1–4), Status (PR #209), Modellwahl Haiku–Fable. 1141 Tests grün mit Originaldaten.
- Gesamtreview (Codex) „Ja“: keine neuen Fehler oder Testlücken. M8 abgeschlossen.

## Runde 6 – Katastrophen, Wochenereignisse, Stationen, Ernte (15.–16.11.2026) · letzte Spur `ebaddee`, Abschluss nach Gesamtreview

- Regelseite des Kerns (PR #149, Spur 1 Teil 1a): `src/Traders.Core/Pixels/` für regelrelevante Pixel ohne Projektreferenz
  (R6P-1, `AGENTS.md` §4); #155/#156.
- Fix #147 (PR #150): Host setzt die Heap-Zeiger von Atlas und Masken; Host-Test Bild für Bild durch den Anbau; #113.
- Spur 2 `ereignisse` (PR #146): Wochenereignisse `17CB:0005` im Kern und als Szene; Leseregel über Methodenparameter (#139),
  Rückrichtung für alle Projekte (#76 P3-1).
- Spur 1 `katastrophen` (PR #153): Katastrophen `111D:0009` mit Sternvorführung `0112` auf der Regelseite, alle Arten im
  Gleichschritt, Szene pixelgenau; #140, #131.
- Spur 3 `stationen` (PR #151, F1 = B): Stationen, Raketenflug, Piraten `1B03:0009` (44 Routinen), Takt je Bild nach R6S-1.
- Spur 0 `ernte` (PR #152): Ernte `22A3:02D3` mit Szene; Einbindung der Aufrufe 4, 9, 11, 16; Mehrrundenprobe Wochen 0–4 mit
  Mensch und Katastrophe; Host spielt bis zur Ernte, Halt an Aufruf 18; #135.
- Prozess: Plan (PR #143, #148), CI-Zeitlimit 30 min (PR #172, #167). 1041 Tests grün mit Originaldaten.
- Gesamtreview (Codex, zwei Teile) „Ja“: keine Fehler, eine Testlücke (Host-Seitenzustand, #180). M6, M9, M10 abgeschlossen.

## Runde 5 – Wochenbilanz, Wochenplanung, Anbau mit Anbau-KI (13.–14.11.2026) · letzte Spur `96b01bf`, Abschluss `9c7c564`

- Spur 2 `planung` (PR #123): Wochenplanung der Computerspieler `11A3` (15 Routinen, beide Durchgänge) mit Differenztests über vier
  Wochen; Sammelmarker in `abdeckung.py` (#109), Erbe nur für Far-Einsprünge des Kopfsegments (R5PL-1); #99 R1/R3.
- Spur 1 `anbau-ki` (PR #125): Computerzug `135D` (14 Routinen) und Helfer `0609:074D`/`100F`, Vertrag für die Anbauphase.
- Spur 3 `bilanz` (PR #124): Leseregel der Szenen (#101, #103, #105), Warteschleife `21FD:0004` mit `1F63:113E`, Wochenbilanz
  `0825:000A`/`02D7` im Kern und als Szene pixelgenau; #101 R4M-1.
- Spur 0 `anbau` (PR #122): Anbauphase `0609:0004`/`03E0` im Kern und als Szene Bild für Bild gegen den RefEmu; Einbindung der
  Aufrufe 3/6/8/21 und Wächter 4; Mehrrundenprobe mit Mensch über vier Wochen; Host spielt Bilanz und Anbau (F1 = A).
- Fix R4W-1 (PR #127, #126): Host richtet Seitennummern und ersten Seitenwechsel ein; Menüänderungen sofort sichtbar.
- Prozess: Plan (PR #117), Zugriffsweg Referenz-Repo (PR #118). 882 Tests grün mit Originaldaten.
- Gesamtreview (Codex) „Ja“: keine Fehler, eine Testlücke der Leseregel (R5G-1, #139). M5 abgeschlossen.

## Runde 4 – Programmablauf, Menüs, neue Partie, Spielfeldschicht (12.–13.11.2026) · letzte Spur `9efc2d2`, Abschluss nach Fix #111 `69db0d4`

- Spur 2 `partie` (PR #92): neue Partie `0E31` (sieben Routinen, Ziehungsfolge) mit Differenztests; #74.
- Spur 1 `menue` (PR #93): Menülogik `192D` im Kern (sieben Zufallsquellen), neues Projekt `Traders.Presentation` mit
  Menüszene pixelgenau, Bild- und Zufallsfolge je Rücklauf im Gleichschritt mit dem RefEmu (#86); Architekturregeln mit Rückrichtung.
- Spur 3 `spielfeld` (PR #95): Befehlsquelle `0E9E` mit sechs Steuerungsarten (Gerätemodell im RefEmu), Feldart `0609:10D2`,
  Cursor `0609:047B`, Spielfeld-Szene pixelgenau.
- Spur 0 `programm` (PR #96): Programmphasen `1777:000C`, Start `0DFD:000D`, Lader `186C`, Code-Abfrage `21FD:0687` (F1 = B),
  Host mit Menüszene, Mehrrundenprobe mit Mensch über vier Wochen (#81).
- Prozess: Plan (PR #90), Aufgaben legt allein die Leitstelle an (PR #94). 751 Tests grün mit Originaldaten.
- Gesamtreview (Codex) „Nein“ wegen P2 R4G-1 (Bildnachholung im Host, #110); Fix #111 mit Regressionstest. M4 abgeschlossen.

## Runde 3 – Phasenmaschine, Wochenregeln, Grafikschichten, Host (11.–12.11.2026) · letzte Spur `fd46c2b`

- Spur 0 `takt` (PR #66): Maschinenabzug im RefEmu (Wiederaufsetzen bitgleich), Phasenmaschine im Kern mit Tick-Takt und Zufall je
  Bild als Kern-Ereignis (F3 = A, `AGENTS.md` §4), acht Regelschritte angebunden, zwei Mehrrundenproben über vier Wochen gegen den
  RefEmu, Abbruchweg B; Fixlauf B1/B2/B5 (#79, #80).
- Spur 1 `regeln` (PR #67): Wochenpreise, Obergrenzen, Schwund, Vermögen/Rang/Verlauf, Produktion, Wetter, Edelsteinkurs,
  Warenpreise – 15 Originalroutinen mit Befund und Differenztest.
- Spur 2 `schichten` (PR #68): Seiten `0B67`, Paletten/Blenden `21FD`, Farbregister `2295`, Befehlsliste `0A96` (17 Typen), Hilfen
  `0A69`, bewegte Objekte `21AF`; #60 geklärt (nicht erreichbar).
- Spur 3 `host` (PR #69): Befunde Eingabe/Zeitgeber `27C6` und Steuerung `0E9E`, `src/Traders.App` (MonoGame, ohne Szenen),
  Ton-Analyse `docs/analyse/TON.md`.
- Prozess: Plan (PR #63), Freigaben der Leitstelle (Referenz-Repo, Labels, Merges; PR #64). 635 Tests grün mit Originaldaten.
- Gesamtreview (Codex, zwei Teile) „Ja“: keine neuen Fehler; Testlücke Bildschirmhandler mit Zufallsfolge → #86 (R4). M3 abgeschlossen.

## Runde 2 – Altbestand übernehmen (11.11.2026) · letzte Spur `b55ce3a`, Abschluss nach Fixlauf #59 `4022a66`

- Spur 0 Bestandsaufnahme (PR #33, Fable): Karte aller 45 Segmente von `T.EXE`, Spiegel gegen Port und Altbranches, RefEmu-Proben P1–P6
  (`docs/analyse/BESTAND.md`); Nachzug in `DATENLAYOUT.md`, `STATUS.md`, Plan (PR #40); Entscheidungen: nur VGA, Zwischenziel
  Gesamtpartie ohne Roboterkampf, F3 = A (Zufall je Bild als Kern-Ereignis).
- Spur 1 RefEmu (PR #46): Tastendruck mit Haltezeit, FXDRIVER als TSR, Code-Haken, Aufrufspur, OPL-Spur; Wochenlauf als Orakel
  (`StartGame`, `RunToWeekStart`, `RunToMarker`) mit festen Zufallszuständen aus P1/P5.
- Spur 2 VGA-Treiber `23CD` (PR #43): Mode-X-Speicher, RAM-Atlas, 29 Treiberadressen mit Sammelbefunden und Differenztests.
- Spur 3 Phasen (PR #44): `tools/callgraph.py` (Ausgabe nur lokal), Befunde `1777:000C`/`01F8`, Test der Markenfolge über zwei Wochen.
- Prozess: Zugriff der Läufe auf das Code-Repo (PR #32), `proben.sh` nach Übernahme (PR #45).
- Gesamtreview (Codex) „Nein“ wegen P2 #57; Fixlauf #59 behebt #57, #58, #50 (16-Bit-Umlauf, Speicher außerhalb der Fenster). 451 Tests
  grün mit Originaldaten.

## Runde 1 – Umstellung auf den Ablauf des Eishockey-Managers (11.11.2026) · `main` `1aead2b`

- 1. Werkzeuge (PR #4): SDK-Release `claude-werkzeuge`, `tools/pruefen.sh`, schlanke `build.yml` statt `ci.yml`.
- 2. Prozessdoku (PR #5): Rollen, Runden, Kurzaufrufe, Review-Kette, Merge-Prüfung; Labels.
- 3. Abdeckung (PR #6): `tools/abdeckung.py` erzeugt die Routinen-/Segmenttabellen in `STATUS.md`; Marker `[ORIGINAL]`,
  `[DIFFERENTIAL]`; Kommentare im Quelltext Englisch.
- 4. Referenz-Repo (PR #19, traders-referenz#1): Originale, Disassemblierung, RefEmu-Bilder und Videobilder nur noch im privaten
  Repo `kaaasi/traders-referenz` (`tools/claude/fetch-reference.sh`); Tests mit Originaldaten `[GameDataFact]`; drei Prüfläufe;
  CI mit Originaldaten über Secret `TRADERS_REFERENZ_TOKEN`; Git-Historie bereinigt.
- Workflow „Branches aufräumen“ (PR #18).
- Kein neuer Portierungsmeilenstein; 396 Tests grün mit Originaldaten (`tools/pruefen.sh`, CI).

## Repositorymodernisierung (27.10.2026) · Integration `76df32b`

- Anbieterunabhängige AGENTS.md, Übergaben und Worktree-/PR-Ablauf.
- Integrationsprüfung mit JSON-Nachweisen und Fehlerfalltests.
- Lesende Build-CI, separater vertrauenswürdiger Publisher, manuelle Diagnosen.
- IDIV-Grenzfall korrigiert und gegen RefEmu abgesichert; gemeinsame Testpfade.
- Kein neuer Portierungsmeilenstein; Paketstände bleiben separat zu integrieren.


Jeder Meilenstein-Stand wird hier mit Commit festgehalten (keine Tags).

## v0.3 – Integration AP1 + AP3 (25.10.2026) · Commit `2c02a38`

- AP1: Referenz-Emulator (286, DOS/BIOS/VGA), T.EXE läuft bis ins Hauptmenü, `random()` gegen Original geprüft ✅
- AP3: Datenlayout (52 Spielstandfelder), C#-Spielzustand, Spielstand lesen/schreiben
- Leitstelle: VGA-Bildkopf korrigiert (Pixel ab Byte 772), Emulator-Startlauf in jedem CI-Lauf
- 388 Tests grün (Linux + Windows)

## v0.2 – Ressourcen M1 (25.10.2026) · Commit `502f9a1`

- Schriften (groß 16×12, klein 16×5), Masken-Atlas, Musik-Kopf „A.H.“
- Ton-Treiberschnittstelle `int 64h` dokumentiert
- Asset-Viewer (MonoGame) und erweiterter PNG-Export
- Formatdoku `docs/RESSOURCENFORMATE.md`

## v0.1 – Fundament (25.10.2026) · Commit `a6cc3fe`

- Portierungsplan M0–M15, Erweiterungen E1–E7
- Originaldateien (DOS, Amiga) mit SHA-256-Prüfung in CI
- Ressourcen: PowerPacker PP20, VGA-Atlas + Palette, Texttabelle, PNG-Export
- Analyse: Segmentkarte (45 Module, ~890 Funktionen), Disassemblierung je Modul
- Kern: Original-Zufallsgenerator `21FD:01BA`, 16-Bit-Arithmetik-Helfer


## AP3 Stand 2 – Savegame-Differenztests

- Stand `f45f778` mit bewahrter Branch-Historie integriert.
- Original-Speichern/Laden, Speicher-Nebenwirkungen und Kurzdatei-Fehlerfall gegen RefEmu geprüft.
