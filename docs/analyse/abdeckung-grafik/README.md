# Abdeckungsüberblick `T.EXE` (Stand `main` `08c086a`, 26.11.2026)

Erzeugt nach [docs/vorlagen/abdeckung-grafik.md](../../vorlagen/abdeckung-grafik.md); Farbregeln unverändert aus
`kaaasi/eishockey-manager-redone`. Seite: [abdeckung.html](abdeckung.html) (Fassung `--ohne-texte`: ohne Textauszüge aus dem
Original, AGENTS.md 2.1). Werkzeuge: `tools/funktionskarte.py`, `tools/markerliste.py`, `tools/abdeckungsgrafik.py`;
Restliste mit Klassen: [../restliste.md](../restliste.md), [../restliste-klassen.csv](../restliste-klassen.csv).

Neu erzeugen:

```bash
tools/claude/fetch-reference.sh original
python3 tools/markerliste.py            # Rückgabe 0: alle [APPROX]-Zeilen eingeordnet
python3 tools/abdeckungsgrafik.py gamedata/dos/T.EXE --ohne-texte --stand "main $(git rev-parse --short HEAD)" \
  --html docs/analyse/abdeckung-grafik/abdeckung.html
```

## Ergebnis

1116 Funktionen, 167 360 Byte Code (`0000`–`28DB`).

| Farbe | Funktionen | Bytes | Anteil Bytes |
|---|---:|---:|---:|
| grün | 554 | 91 230 | 54,5 % |
| gelb | 19 | 1 643 | 1,0 % |
| blau | 41 | 7 442 | 4,4 % |
| grau | 131 | 15 181 | 9,1 % |
| rot | 371 | 51 864 | 31,0 % |

Rot nach Gruppe: Roboterkampf 22 416 Byte (`0EDE`, `1C68`, `1E7E`, `0535`), Computerspieler 12 388 (`150F`, `16D6`, `12C8`:
KI des Roboterkampfs), Grafik 11 650 (fast nur `23CD`, Treiber in Assembler ohne Zuordnung der Teile), System/Hilfen 4 888
(`186C`, `0D73`, `27C6`), Phasen/Regeln 439, Ton 83. Größte rote Blöcke je Segment: `23CD` 11 602 Byte, `0EDE` 9 200,
`150F` 7 280.

**Probe:** Summe der Bytes aller Farben = Summe `groesse` der CSV = 167 360 (0x28DC0); 1116 Funktionen in Karte und CSV.
Stichprobe je Farbe 10 Funktionen (gelb 10 von 19) von Hand gegen Fundstelle und Marker: alle stimmig. Gegenprobe: alle 653
Adressen der `[ORIGINAL]`-Listen (Parser von `tools/abdeckung.py`) liegen in Funktionen mit Status „umgesetzt“.

## Abweichungen von der Vorlage

1. **Zielprogramm** heißt `T.EXE` (`TRADERS.BAT` startet es); eine Datei `traders.exe` gibt es nicht. `FXDRIVER.EXE` (Marker
   `[ORIGINAL-FX]`) ist nicht Gegenstand.
2. **Ablage:** Vorlage „nichts ins Repo“; hier auf Wunsch des Nutzers zusätzlich im Repo, und zwar nur die Seite ohne
   Textauszüge und diese Beschreibung; die CSV (mit Textauszügen) bleibt außerhalb. Artifact mit Textauszügen.
3. **Pfade:** Werkzeuge flach in `tools/` (Repo-Konvention) statt `tools/reverse-engineering/`; Restliste
   `docs/analyse/restliste.md` / `restliste-klassen.csv` statt `restliste-v1*`. Zuordnung und Grafik liegen als Skript
   `tools/abdeckungsgrafik.py` im Repo (Probencode gehört ins Repo, Nutzer 11.11.2026) statt nur im Arbeitsordner.
4. **`funktionskarte.py`, Layout:** MZ-Ladebild mit Segmenten aus der Relokationstabelle (wie `tools/segmap.py`), keine
   Borland-Overlays und Stubs, Adressen `SSSS:OOOO` statt Dateioffsets; die Heuristiken für Dateioffset/DGROUP-Werte, für
   Ghidra-Segmentnamen und für Stub-Aufrufe (`call_refs`) entfallen. Zusätzliche Funktionsstarts: Far-Zeiger in der DGROUP.
   Kein `--write` (es gibt kein `re-abdeckung.md`; die Tabellen in `STATUS.md` erzeugt `tools/abdeckung.py`); die
   Markdown-Ausgabe enthält keine Originaltexte. Hinweise: Bildnummern an den Lader `186C:0002` statt `kb\`-Lader, keine
   XOR-Texte.
5. **`[ORIGINAL]`-Zuordnung:** „umgesetzt“ zählen nur die Adressen der eigenen Liste direkt hinter dem Marker (Bedeutung des
   Markers in AGENTS.md 2.8 und `tools/abdeckung.py`); weitere Adressen nach einem `[ORIGINAL]` (gerufene Routinen) gelten als
   „dokumentiert“. Die Regel der Vorlage (jede Adresse nach dem Marker) gibt es als `--vorlagenregel`: 602 statt 587 Funktionen
   „umgesetzt“, +946 Byte.
6. **Bereiche:** nur `SSSS:OOOO..OOOO`, `…` oder `–` ohne Leerzeichen und Ende im selben Segment, weil „ – “ in den Kommentaren
   Adresse und Text trennt.
7. **`markerliste.py`:** ohne Erstbelegung aus `inventur.md` und ohne Funktionszeilen (F-IDs); IDs ab A001. Die 54 Zeilen sind
   erstmals eingeordnet (31 E, 22 K, 1 D, kein V oder X) – Einordnung durch diese Sitzung, noch ohne unabhängiges Review.
8. **Grau-Belege:** Laufzeit `0000` auf Segmentebene (BESTAND.md §1.2 „nicht zu portieren“, Kennung: Borland-Copyright der
   Laufzeit am Anfang der DGROUP) statt je Funktion; ausgenommen die Tonroutinen `0000:06FE/0724/0750` (Zuordnung offen). Vom
   Treiber `23CD` sind nur die drei Einstiegsfunktionen von EGA (`0797`), Tandy (`1645`) und Tandy-Emulation (`3F0A`) grau,
   weil die Ausdehnung dieser Teile nicht belegt ist; der Rest von `23CD` bleibt rot (wahrscheinlich größtenteils EGA/Tandy).
9. **Gelb ohne Adresse:** statt der Lückenliste der Vorlage zwei Nachweise: PP20-Entpacker `2821` (`src/Traders.Assets/PowerPacker.cs`,
   BESTAND.md „vorhanden, unbelegt“) und die Befehlsausführung `0A96:06E4…0BFB` (Versätze als `case` in
   `src/Traders.Vga/Layers/CommandList.cs`, ohne Marker).
10. **Grafik:** Karte als Speicherkarte (512 Byte je Zeile, Adressreihenfolge, Fläche nach Größe; lange Funktionen laufen über
    mehrere Zeilen) statt Rechteckflächen; Grau zusätzlich schraffiert, weil es ohne Farbsättigung ist; nur heller Modus.
11. **Ungenauigkeiten der Abgrenzung** (wie in der Vorlage, „eine Fundstelle zählt die ganze Funktion“): Assemblerteile ohne
    Prolog werden zu groß abgegrenzt (z. B. `0000:0812` 2211 Byte, `23CD:2D33` 838 Byte); 36 Füllstücke am Segmentanfang
    (284 Byte) zählen als eigene Funktionen, meist rot. `2850:00CC` ist gelb (genähert, Klasse K), obwohl der PC-Lautsprecher
    entfällt – die Farbregeln gehen vor.
