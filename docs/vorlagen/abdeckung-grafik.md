# Vorlage: Grafischer Überblick der Abdeckung von `T.EXE`

Übernommen aus `kaaasi/eishockey-manager-redone` (`docs/vorlagen/abdeckung-grafik.md`, dort für `em.exe`) und an dieses Repo
angepasst; die Farbregeln sind unverändert. Abweichungen vom Vorbild: [docs/analyse/abdeckung-grafik/README.md](../analyse/abdeckung-grafik/README.md)
Abschnitt „Abweichungen“.

Erzeugt eine Grafik, wie weit die Funktionen von `T.EXE` im Port umgesetzt sind, in fünf Farben. Nur lesen: nichts committen,
außer der Nutzer verlangt eine Ablage (dann nur die Fassung `--ohne-texte`, AGENTS.md 2.1).

**Kurzaufruf** (der Nutzer schreibt ihn in einen neuen Chat):

> Repo kaaasi/traders-redone. Erstelle den Abdeckungsüberblick nach `docs/vorlagen/abdeckung-grafik.md`.
> Stand: `<main | Branch/SHA>`.

**Modell:** Claude Sonnet 5.5 reicht: Die Zuordnung läuft über Skripte und feste Regeln, Unklares wird rot. Claude Opus 5.5 nur,
wenn die unbekannten Funktionen inhaltlich eingeordnet werden sollen (Zusatz im Kurzaufruf: „mit Einordnung der roten
Funktionen“).

## Farben (je Funktion von `T.EXE`, gewichtet nach Bytes)

| Farbe | Bedeutung | Regel |
|---|---|---|
| **grün** | belegt umgesetzt | Status `umgesetzt` der Funktionskarte, und keine Fundstelle der Funktion gehört zu einem `[APPROX]` der Klasse E |
| **gelb** | umgesetzt ohne Beleg | Status `genähert` mit `[APPROX]` der Klasse K oder D; ebenso `dokumentiert` oder `unbekannt`, wenn der Port die Funktion nachweislich ohne Adresse nachbaut (z. B. aus Formatwissen oder über einen Versatz im Code), mit Nachweis (Datei im Port) |
| **blau** | anders umgesetzt | eine Fundstelle der Funktion gehört zu einem `[APPROX]` der Klasse E (Nutzerentscheidung, korrigierter Fehler des Originals, Plattformersatz, Erweiterung) – blau hat Vorrang vor grün und gelb |
| **grau** | weggefallen | nur mit Beleg: Doku oder Code nennt die Funktion als entfallen bzw. Plattformschicht ohne Gegenstück (EGA, Tandy, PC-Lautsprecher, Diskettenwechsel, Turbo-C-Laufzeit mit erkennbarem Kennzeichen) |
| **rot** | noch unbekannt | alles Übrige, besonders `unbekannt` und `dokumentiert` ohne Port-Nachweis und ohne Entfall-Beleg; im Zweifel rot |

„Gehört zu einem `[APPROX]`“ heißt wie in `tools/funktionskarte.py` Schritt 3: der nächste Marker vor der Fundstelle im selben
Kommentar (sonst danach). Die Klasse steht in der Restliste (`tools/markerliste.py`, Spalte Klasse; Gründe in
`docs/analyse/restliste-klassen.csv`).

## Ablauf

1. Lies `AGENTS.md` §2 und §4 sowie `docs/analyse/BESTAND.md` §1. Originaldaten: `tools/claude/fetch-reference.sh original`
   (Claude: Referenz-Repo per `add_repo`, access read). `ndisasm` (Paket `nasm`).
2. Stand auschecken; `git rev-parse --short HEAD` notieren. `python3 tools/markerliste.py` muss 0 liefern (alle
   `[APPROX]`-Zeilen eingeordnet); sonst die neuen Zeilen in `docs/analyse/restliste-klassen.csv` einordnen.
3. Daten außerhalb des Repos erzeugen (Arbeitsordner, z. B. `/tmp/abdeckung/`):
   - `python3 tools/funktionskarte.py gamedata/dos/T.EXE --csv /tmp/abdeckung/funktionen.csv`
     (Spalten u. a. `adresse`, `datei`, `segment`, `groesse`, `status`, `texte`, `approx`, `fundstellen`),
   - `python3 tools/markerliste.py --abweichungen` bzw. die Tabelle in `docs/analyse/restliste.md`.
4. Zuordnung und Grafik: `python3 tools/abdeckungsgrafik.py gamedata/dos/T.EXE --html /tmp/abdeckung/abdeckung.html
   --json /tmp/abdeckung/farben.json --stand <SHA>` (Farben nach der Tabelle oben; Grau und Gelb ohne Adresse nur über die
   Listen `GREY`/`YELLOW` im Skript, je mit Beleg). Neue Entfall- oder Nachbau-Belege dort eintragen, sonst rot.
5. Probe: Summe der Bytes aller Farben = Summe `groesse` der CSV; Anzahl Funktionen gleich; Stichprobe 10 Funktionen je Farbe
   von Hand gegen Fundstelle und Marker prüfen (im Ergebnis nennen, `--probe "<p>…</p>"` schreibt sie in die Seite).
6. Seite prüfen: eine selbständige HTML-Seite (keine externen Skripte oder Schriften) mit Stand, Legende, Gesamtbalken nach Bytes
   und Anzahl, Karte je Segment in Adressreihenfolge (Fläche nach Größe, Überfahren zeigt Dateioffset, Größe, Farbe, Klasse,
   Fundstellen, Hinweis; Filter nach Farbe), Tabellen der 20 größten roten und gelben Funktionen und der grauen mit Beleg.
   Aus dem Original nur Adressen, Größen und kurze Textauszüge; keine Bilder, keine Bytes; im Repo ohne Textauszüge.
7. Ausgabe: Claude als Artifact (zuerst die Artefakttypen auflisten; passt der Typ Dashboard, diesen nehmen, sonst die HTML-Seite
   veröffentlichen). Andere Agenten: HTML-Datei an den Nutzer.
8. Antwort an den Nutzer **knapp** (`AGENTS.md` §7): Link bzw. Datei, Anteil je Farbe in Bytes (Prozent), die drei größten roten
   Blöcke, Ergebnis der Probe.
