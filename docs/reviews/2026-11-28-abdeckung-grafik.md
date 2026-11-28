# Review: Abdeckungsüberblick T.EXE (PR #283)

Datum 2026-11-28 · Head `8929339` · Base `08c086a` · Reviewer: Claude Opus 5.5, frische Sitzung (geplanter Lauf), ohne
Implementierungskontext. Checkout `git rev-parse HEAD` = `89293397739d123cbbc47334c5e06923fdd5c3d4`, `git status --porcelain`
leer (auch nach allen Läufen). Originaldaten aus `kaaasi/traders-referenz` `0183d15` (Hashes geprüft von `fetch-reference.sh`).

## 1. Merge-Empfehlung

**nein** für genau diesen Head – wegen eines kleinen P2 (B1): drei der neun Grau-Belege in `tools/abdeckungsgrafik.py` treffen
keine Funktion und wirken nicht. Die README behauptet trotzdem, sie seien grau. Alles Übrige ist stimmig und reproduzierbar:
die Seite lässt sich aus den Werkzeugen bis auf Erzeugungsdatum und Probetext byte-gleich erzeugen, die Zahlen stimmen, und es gibt
keine Originaltexte oder -bytes in den Repo-Dateien. B1 ist mit wenigen Zeilen behebbar (Delta-Prüfung der Leitstelle genügt, nur P2/P3).

„Erwartet offen“ bestätigt: der Restbereich von `23CD` ist rot (56 rote, 25 grüne, 1 blaue Funktion, 0 graue). README Abweichung 7
(Einordnung ohne Review) ist mit diesem Review abgedeckt.

## 2. Befunde

**B1 (P2) – Grau-Belege `23CD:0797/1645/3F0A` ohne Wirkung, README Abweichung 8 falsch.**
[`tools/abdeckungsgrafik.py:53-55`](https://github.com/kaaasi/traders-redone/blob/8929339/tools/abdeckungsgrafik.py#L53-L55)
verlangt mit `hi=None` einen Funktionsstart genau an diesen Versätzen. `tools/funktionskarte.py` erkennt dort aber keinen Start:
die drei Ziele haben keinen Prolog und nur einen Near-Aufrufer, und die Regel in
[`funktionskarte.py:231-234`](https://github.com/kaaasi/traders-redone/blob/8929339/tools/funktionskarte.py#L231-L234) verlangt
einen Prolog oder mindestens zwei Aufrufer. Der Lauf meldet das nicht. In
[`README.md` Abweichung 8](https://github.com/kaaasi/traders-redone/blob/8929339/docs/analyse/abdeckung-grafik/README.md) steht
dagegen: „Vom Treiber `23CD` sind nur die drei Einstiegsfunktionen … grau“.
Beleg (Probe):
```bash
python3 -I - <<'E'
import sys; sys.path.insert(0,'tools'); from pathlib import Path
import abdeckungsgrafik as ag, funktionskarte as fk
an=fk.Analyzer(Path('gamedata/dos/T.EXE').read_bytes()); an.run()
for s,lo,hi,_ in ag.GREY:
    print(f'{s:04X}:{lo:04X}', sum(f.region.seg==s and ((hi is None and f.offset==lo) or (hi is not None and lo<=f.offset<=hi)) for f in an.functions))
E
```
Ergebnis: `23CD:0797 0`, `23CD:1645 0`, `23CD:3F0A 0` (alle anderen Einträge ≥ 1). Die Versätze liegen in den Funktionen `23CD:071C`
(grün), `23CD:136E` (rot) und `23CD:3DE5` (rot). Grau `23CD` = 0 Byte. Die Gesamtzahl grau (131 Funktionen) enthält sie also nicht.
Gegenprobe in `T.EXE` (`ndisasm -b16 -o 0x4F6 -e $((0x272D0+0x4F6)) gamedata/dos/T.EXE`): `23CD:04F6` ruft `call 0x797`,
`call 0x3f0a` und `call 0x1645` je genau einmal. Das passt zu `docs/analyse/GRAFIK.md:53`. Die Einsprünge selbst sind also echt,
nur die Karte zerlegt sie nicht.
Vorschlag: (a) für Einträge mit `hi=None` einen Funktionsstart erzwingen, z. B. über eine Liste zusätzlicher Starts in
`funktionskarte.py` mit Beleg `GRAFIK.md:53`. Oder (b) die README berichtigen („keine Funktion grau, Einsprünge nicht abgegrenzt“).
In beiden Fällen sollte `abdeckungsgrafik.py` mit Rückgabe ≠ 0 abbrechen, wenn ein `GREY`/`YELLOW`-Eintrag keine Funktion trifft.
Wirkung von (a) laut Funktionsgrenzen: etwa 98 Byte (17 + 37 + 44) von grün/rot nach grau.

**B2 (P3, Verdacht) – A032 als K ohne Beleg „ohne Spielwirkung“.**
[`restliste-klassen.csv` A032](https://github.com/kaaasi/traders-redone/blob/8929339/docs/analyse/restliste-klassen.csv):
Laut Marker
([`StepHandling.cs:75`](https://github.com/kaaasi/traders-redone/blob/8929339/src/Traders.Core/Phases/StepHandling.cs#L75)) und
`1777_01F8_wochenschleife.md:86` ist das Verhältnis der Warteschleifen zum Bildwechsel noch offen. Die Spalte „Verdacht“ sagt das
auch. Ob das ohne Wirkung auf den Zufall je Bild bleibt (F3 = A), ist nicht belegt. Die Klasse K behauptet es aber. Vorschlag:
Klasse V oder einen RefEmu-Beleg in die Spalte „Beleg“. Für die Grafik wäre das gelb → rot (Funktionen mit diesem Marker).
Kein ausführbarer Gegenbeleg, daher nur Verdacht.

Keine weiteren Befunde. Kein P0/P1.

## 3. Ausgeführte Prüfungen

**`tools/pruefen.sh` (alle drei Läufe, Head `8929339`, Ergebnis: grün, dotnet 10.0.402):**

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| Mit Originaldaten (TR_REQUIRE_GAMEDATA=1) | 1395 | 0 | 0 |
| Ohne Originaldaten (TR_SKIP_GAMEDATA=1) | 589 | 0 | 468 |
| Pflichtlauf ohne Daten (muss scheitern) | 589 | 806 | 0 |

**Prüfpunkt (3), keine Originaltexte oder -bytes (AGENTS.md 2.1):**
- Alle druckbaren Folgen ≥ 6 Zeichen aus `T.EXE` gegen die 8 geänderten Dateien abgeglichen: 0 Treffer (nur Leerzeichenfolgen in `tools/`).
- Alle Einträge der CSV-Spalte `texte` (`funktionskarte.py --csv` außerhalb des Repos, 9 Gruppen, auch einzeln an „ / “ getrennt)
  gegen `abdeckung.html`, `restliste.md`, `restliste-klassen.csv` und die README abgeglichen: 0 Treffer.
- Hinweise `h` in `abdeckung.html`: nur „gerufen von …“ (964), „Globale DS:…“ (51), „–“ (101). Die Spalte „Wortlaut“ der Restliste
  zitiert nur Kommentare des Ports.
- Werkzeuge: einziges Bytemuster ist der Prolog `55 8B EC` (allgemeiner x86-Code). Die Laufzeit wird nur über das Wort „Borland“ bzw.
  „Copyright“ an `DS:0004` erkannt; das ist geprüft, der Text steht nicht im Repo.

**Prüfpunkt (2), Grau/Gelb, reproduzierbar:**
- `python3 -I tools/abdeckungsgrafik.py gamedata/dos/T.EXE --ohne-texte --stand "main 08c086a" --html …`: Daten (`const D`) gleich
  der Repo-Seite bis auf `probe`. Die übrige Seite unterscheidet sich nur im Erzeugungsdatum. Summen: 554/19/41/131/371 Funktionen,
  91 230/1 643/7 442/15 181/51 864 Byte, Summe 1116 / 167 360 = README.
- Grau je Segment: `0000` 124 (dazu 19 grün und 2 blau vor grau, `GREY_EXCEPT` 06FE/0724/0750 grün bzw. blau), `2850` 4 (`00CC` gelb,
  K vor grau, wie README 11), `186C:0B63` 1 (`0A6C` blau wegen A038 E, Vorrang), `28DA`/`28DB` je 1, `23CD` 0 (B1).
  Zeilenverweise `BESTAND.md:40/81/83/84/357`, `PORTIERUNGSPLAN.md:19`, `GRAFIK.md:53` und `186C_ressourcenlader.md:19` passen inhaltlich.
- Gelb: 14 Handler `0A96` aus `CommandList.cs:277–303`. Alle 17 `case`-Versätze sind Funktionsstarts. `07E4`, `0884` und `0A51` sind
  `umgesetzt` und damit grün. Dazu 4 Funktionen in `2821` (PowerPacker) und `2850:00CC` (A012 K).
- Gegenprobe README: alle 653 Adressen aus `tools/abdeckung.py scan('[ORIGINAL]')` liegen in Funktionen mit Status `umgesetzt` (0 Abweichungen).
- `funktionskarte.py --vorlagenregel`: 602 Funktionen `umgesetzt` wie in README Abweichung 5.

**Prüfpunkt (1), Klassen (Stichprobe 20 von 54 gegen den Kommentar im Code):** A003, A007, A008, A010, A012, A014, A015, A016,
A024, A029, A030, A031, A032, A036, A038, A042, A045, A046, A047, A054. Alle passen außer A032 (B2). Grenzfall A012 (K): `DS:10A5`
bleibt im Kern 1, das Original löscht es. Das wirkt nur auf die nächste Treiberabfrage, also K vertretbar (#262 offen).
`python3 tools/markerliste.py` Rückgabe 0. `--write` lässt den Baum unverändert (Block aktuell). Zählung `grep -rc` = 41 Dateien, 54 Zeilen.

**Nicht ausgeführt:** Mutationen (der PR bringt keine Tests; die Python-Werkzeuge haben keine Tests). Gezielte Handprobe von je
10 Funktionen je Farbe: nur für Grau, Gelb und `23CD`.

**Akzeptierte `[APPROX]`:** keine neuen im PR. Die Einordnung A001–A054 ist bis auf A032 als Stichprobe akzeptiert.
**Annahmen:** Die übrigen 34 Klassen sind nicht einzeln gelesen.
