# Arbeitspakete für parallele Chats

Jeder Chat übernimmt **genau ein** Paket, arbeitet auf seinem eigenen Branch und schreibt nur
in seinen Dateibereich. Gemeinsame Dateien (siehe unten) nur minimal ändern.

**Übernehmen:** Zeile unten auf „in Arbeit“ setzen, Chat-Name und Datum eintragen, sofort
nach `main` pushen (so sehen andere Chats, dass das Paket vergeben ist). Ist ein Paket schon
„in Arbeit“, ein anderes nehmen.

## Jetzt parallel möglich

| AP | Paket | Meilenstein | Branch | Dateibereich | Hängt ab von | Status | Chat / seit |
|---|---|---|---|---|---|---|---|
| AP1 | **Referenz-Emulator**: 8086-Interpreter, MZ-Lader mit Relokationen, DOS/BIOS-Stubs (Datei, Speicher, Zeit, Tastatur, Modus 13h, Timer, `int 64h` aufzeichnen), Framebuffer-Dump; erster Differenztest `random()` | M2 | `dev/ap1-refemu` | `src/Traders.RefEmu/`, `tests/Traders.Tests/RefEmu/` | – | in Arbeit | Chat „AP1 Referenz-Emulator“ / 25.10.2026 |
| AP2 | **Grafik-Kern**: Grafiktreiber `0A96` + Hilfen `0A69`, Textausgabe (Zeichenbreiten), maskierte Blits (`0A96:013B`), Paletten-/Fade-Effekte → persistenter Framebuffer mit Original-Operationen | M2/M3 | `dev/ap2-vga` | `src/Traders.Vga/`, `tests/Traders.Tests/Vga/`, `docs/routines/0A96_*`, `0A69_*` | – | in Arbeit | Chat „AP2 Grafik-Kern“ / 25.10.2026 |
| AP3 | **Datenlayout & Spielzustand**: DGROUP-Variablen benennen (Spieler, Felder, Vorräte, Geld, Phasen-/Rundenzähler, IQ), Strukturen und Breiten; C#-Zustandsmodell (serialisierbar) | M3 | `dev/ap3-state` | `docs/analyse/DATENLAYOUT.md`, `src/Traders.Core/State/`, `tests/Traders.Tests/State/` | – | offen | |
| AP4 | **Ton**: `FXDRIVER.EXE` disassemblieren, `int 64h`-Funktionen, Format „A.H.“ vollständig, OPL2-Emulation, Musikplayer | M14 | `dev/ap4-audio` | `src/Traders.Audio/`, `tests/Traders.Tests/Audio/`, `docs/analyse/FXDRIVER.md` | – | offen | |
| AP5 | **Programmablauf & Phasen**: `main()` und Hauptschleife finden, Aufrufgraph, Reihenfolge und Bedingungen aller Phasen, Zuordnung der „?“-Module, Video-Zeitstempel je Phase | M3 (Vorarbeit M4–M13) | `dev/ap5-phasen` | `docs/analyse/PHASEN.md`, `docs/analyse/callgraph/`, `tools/callgraph.py` | – | offen | |

## Später (nach AP1 + AP3 + AP5)

Je Phase ein Paket: Menü/Setup (M4), Anbau (M5), Produktion/Wetter (M6), Fat Mike (M7),
Auktion (M8), Rakete/Piraten (M9), Ereignisse/Katastrophen (M10), Bilanz/Einkauf (M11),
Kampf (M12), Ende (M13). Diese werden hier eingetragen, sobald die Grundlagen stehen.

## Gemeinsame Dateien – Regeln

| Datei | Regel |
|---|---|
| `STATUS.md` | Nur die Zeilen des eigenen Pakets/Moduls ändern. |
| `CHANGELOG.md` | Nur beim Abschluss eines Pakets einen Eintrag oben anfügen. |
| `docs/ARBEITSPAKETE.md` | Nur die eigene Zeile ändern. |
| `TradersRedone.slnx`, `Directory.Build.props`, `global.json`, CI | Möglichst nicht ändern; falls nötig, kleine eigene Commits. |
| `docs/analyse/disasm/` | Nicht von Hand ändern (erzeugt von `tools/segmap.py`). |

## Ablauf je Chat

1. `git fetch origin && git checkout -b dev/apN-… origin/main` (bzw. bestehenden Branch fortsetzen).
2. Arbeiten, committen, Branch pushen → CI läuft, `tools/ci-wait.sh` holt den Bericht.
3. Regelmäßig `git fetch origin && git rebase origin/main`, damit Änderungen anderer Chats
   früh ankommen.
4. Zwischenstand fertig und CI grün → nach `main` übernehmen:
   `git fetch origin && git rebase origin/main && git push origin HEAD:main`
   (scheitert der Push, weil ein anderer Chat schneller war: erneut rebasen und pushen).
5. `STATUS.md` und die eigene Zeile hier aktualisieren.
