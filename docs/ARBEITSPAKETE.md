# Arbeitspakete für parallele Chats

Jeder Paket-Chat bearbeitet **genau ein** Paket auf seinem eigenen Branch und schreibt nur in
seinen Dateibereich (maschinenlesbar: `ci/arbeitspakete.json`). Diese Datei pflegt **nur die
Leitstelle**; Paket-Chats ändern sie nicht. Regeln: [AGENTS.md](../AGENTS.md), Abschnitt
„Rollen“.

## Jetzt parallel möglich

| AP | Paket | Meilenstein | Branch | Dateibereich | Hängt ab von | Status | Chat / seit |
|---|---|---|---|---|---|---|---|
| AP1 | **Referenz-Emulator**: 8086-Interpreter, MZ-Lader mit Relokationen, DOS/BIOS-Stubs (Datei, Speicher, Zeit, Tastatur, Modus 13h, Timer, `int 64h` aufzeichnen), Framebuffer-Dump; erster Differenztest `random()` | M2 | `dev/ap1-refemu` | `src/Traders.RefEmu/`, `tests/Traders.Tests/RefEmu/` | – | Stand 1 übernommen (25.10.), in Arbeit | Chat „AP1 Referenz-Emulator“ / 25.10.2026 |
| AP2 | **Grafik-Kern**: Grafiktreiber `0A96` + Hilfen `0A69`, Textausgabe (Zeichenbreiten), maskierte Blits (`0A96:013B`), Paletten-/Fade-Effekte → persistenter Framebuffer mit Original-Operationen | M2/M3 | `dev/ap2-vga` | `src/Traders.Vga/`, `tests/Traders.Tests/Vga/`, `docs/routines/0A96_*`, `0A69_*` | – | in Arbeit | Chat „AP2 Grafik-Kern“ / 25.10.2026 |
| AP3 | **Datenlayout & Spielzustand**: DGROUP-Variablen benennen (Spieler, Felder, Vorräte, Geld, Phasen-/Rundenzähler, IQ), Strukturen und Breiten; C#-Zustandsmodell (serialisierbar) | M3 | `dev/ap3-state` | `docs/analyse/DATENLAYOUT.md`, `src/Traders.Core/State/`, `tests/Traders.Tests/State/` | – | Stand 2 übernommen (27.10., PR #2); neue Aufgabe nur nach Vergabe | Chat „AP3 Datenlayout & Spielzustand“ / 25.10.2026 |
| AP4 | **Ton**: `FXDRIVER.EXE` disassemblieren, `int 64h`-Funktionen, Format „A.H.“ vollständig, OPL2-Emulation, Musikplayer | M14 | `dev/ap4-audio` | `src/Traders.Audio/`, `tests/Traders.Tests/Audio/`, `docs/analyse/FXDRIVER.md` | – | offen | |
| AP5 | **Programmablauf & Phasen**: `main()` und Hauptschleife finden, Aufrufgraph, Reihenfolge und Bedingungen aller Phasen, Zuordnung der „?“-Module, Video-Zeitstempel je Phase | M3 (Vorarbeit M4–M13) | `dev/ap5-phasen` | `docs/analyse/PHASEN.md`, `docs/analyse/callgraph/`, `tools/callgraph.py` | – | offen | |

## Später (nach AP1 + AP3 + AP5)

Je Phase ein Paket: Menü/Setup (M4), Anbau (M5), Produktion/Wetter (M6), Fat Mike (M7),
Auktion (M8), Rakete/Piraten (M9), Ereignisse/Katastrophen (M10), Bilanz/Einkauf (M11),
Kampf (M12), Ende (M13). Diese werden hier eingetragen, sobald die Grundlagen stehen.

## Ablauf

| Schritt | Wer | Was |
|---|---|---|
| Vergabe | Nutzer + Leitstelle | Nutzer startet einen Chat mit „übernimm APn“; Leitstelle trägt ihn hier ein. |
| Arbeit | Paket-Chat | Commits auf `dev/apN-…`, CI über `tools/ci-wait.sh` (nach Publisher-Aktivierung auf main), `main` per Merge hereinholen. |
| Übergabe | Paket-Chat | `docs/uebergabe/APn.md` aktualisieren, „Bereit zur Übernahme: ja“, CI grün. |
| Prüfung | Leitstelle | `tools/pruefe-branch.py dev/apN-…`: Dateibereich, Regeln des Kerns, CI, Konflikte. |
| Übernahme | Leitstelle | Merge nach `main`, CI grün, `STATUS.md`/`CHANGELOG.md`/diese Datei aktualisieren. |
| Rückmeldung | Leitstelle → Nutzer | Was übernommen wurde, was der Paket-Chat nachbessern soll. |

Experimente liegen unter `experiments/apN/` und laufen nur manuell. Bestehende `ci/experiment.sh` vor Integration migrieren.


## Verifizierter Branch-Stand vom 27.10.2026 (keine Übernahmefreigabe)

| AP | Kopf | vor main | Beobachtung |
|---|---|---:|---|
| AP1 | `2f03e46` | 2 | Tracing/Spielstart-Erweiterungen; Übergabe nennt noch älteren Stand. |
| AP2 | `3fc6033` | 7 | Mode-X-Code und Differenztests vorhanden; Übergabe „noch kein Code“ veraltet. |
| AP3 | `f45f778` | 3 | Savegame-Differenztests vorhanden; separater Integrationsreview erforderlich. |
| AP5 | `92edea9` | 1 | Callgraph und Experiment vorhanden; bisheriger Status „offen“ beschreibt nicht den Branch. |

Alle vier Branches basieren bei diesem Abruf auf main `717a226`. Aktive Bearbeiter müssen ihren
Status bestätigen; aus einem Commit wird keine aktive Chat-Zuständigkeit abgeleitet. Historische
Tabellen oben beschreiben die ursprüngliche Vergabe. Vor Neuvergabe Branch und Übergabe lesen.
Leitstelle für die Repositorymodernisierung: ChatGPT im ausdrücklichen Nutzerauftrag;
Arbeitsbranch `dev/repo-collaboration`. Keine parallele Paketarbeit dadurch neu vergeben.

## Integration vom 27.10.2026

AP3 Stand 2 (`f45f778`) ist mit PR #2 in `main` (`d13c5b3`) übernommen. Der kombinierte
Stand bestand 396 C#-Tests auf Linux; Linux-/Windows-CI und neun Repositorytests sind grün.
Die Branch-Inventur oben bleibt eine datierte Momentaufnahme vor der Integration.
AP1/AP2 bleiben ohne Übernahmefreigabe; AP5 benötigt eine aktuelle Übergabe. Diese Chats
werden durch die Repositorypflege weder neu gestartet noch als beendet markiert.
Neue Bearbeiter zuerst auf aktuellen main-Stand bringen und nach der gemeinsamen Vorlage übergeben.
