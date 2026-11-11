# Traders Redone – Portierungsplan

Ziel: *Traders – The Intergalactic Trading Game* (Linel, 1991) originalgetreu von der
DOS-Fassung nach C# portieren und danach um neue Funktionen erweitern.

Stand: 25.10.2026 · Basis: `T.EXE` SHA-256 `b73a2170…af0185` (identisch mit der Fassung
des Vorversuchs, alle dort genannten Adressen gelten).

---

## 1. Ausgangslage (geprüft)

| Befund | Bedeutung für den Port |
|---|---|
| `T.EXE`: Turbo C++ 1990 (Borland), unkomprimiert, 3394 Relokationen, DGROUP = `0x28DC` | Gut dekompilierbar: C-Aufrufkonventionen, Borland-Laufzeit erkennbar (u. a. `rand()`). |
| Ressourcenliste bei `DS:09CE`, 10 Einträge à 19 Byte, je Far-Pointer auf `V-*.PP` und `E-*.PP` | Befund aus dem Vorversuch bestätigt. `V-` = VGA, `E-` = EGA. |
| `*.PP` sind PowerPacker PP20 | Entpacker bereits geschrieben (`tools/pp20.py`). |
| `V-*.PP` entpackt: 768 Byte Palette (6-Bit-RGB) + 320×256 Pixel (8 Bit) + 4 Byte | Atlas inklusive Sprites unterhalb der sichtbaren 200 Zeilen. |
| `E-*.PP`: 40964 Byte (EGA-Planar) | Für den Port zweitrangig, nur zur Gegenprüfung. |
| `FONTS.PP` (2150 B), `MASKS.PP` (8000 B), `ENGLISCH.TXT` | Schrift, Masken, alle Spieltexte (englisch). |
| `*.CMF` mit Kennung `A.H.` (kein Creative-CMF) + `FXDRIVER.EXE` (Linel, 1991) | Eigenes Musikformat + AdLib-Treiber → OPL2-Emulation nötig. |
| Amiga-ADFs (3 Fassungen): u. a. `AnbauBild.bm`, `SchlachtBild.bm`, … | Zweite Quelle für Logik (68k-Code) und Samples; nur zur Gegenprüfung. |
| Zwei Videos (46 min @720p, 94 min @360p) | Komplette Partien inkl. Kampf, Auktion, Rakete, Endrang. Visuelle Referenz. |

## 2. Leitprinzipien (aus dem Vorversuch gelernt)

1. **Original ist die Wahrheit.** Jede Routine wird aus `T.EXE` abgeleitet und gegen das
   Original *ausgeführt* verglichen – nicht gegen Erinnerung oder Video allein.
2. **Deterministischer Kern.** Zufall (Borland-LCG), Uhr-Neusaat, 16-Bit-Arithmetik,
   Aufrufreihenfolge exakt erhalten. Uhr- und Tastatureingaben werden als Eingabestrom
   aufgezeichnet.
3. **Einmal rechnen, dann vorführen.** Der Kern bucht; Animationen lesen nur. Rendern,
   Pause, Laden verbrauchen nie Zufall und buchen nie doppelt.
4. **Regelrelevante Pixel gehören in den Kern.** Kollision gegen `V-RAKETE`, Sichtbarkeit
   gegen `V-BERGE` nutzen den kanonischen Original-Atlas – unabhängig vom späteren
   Grafikpaket.
5. **Persistenter Bildpuffer.** Der VGA-Renderer arbeitet wie das Original auf einem
   bleibenden Framebuffer (Verkäuferbalken/Spuren entstehen so von selbst).
6. **Erweiterungen erst auf geprüftem Kern.** Jede Erweiterung ist abschaltbar; der
   „Originalmodus“ bleibt bitgenau.

## 3. Architektur

```
TradersRedone.sln
├─ src/Traders.Assets      PP20, VGA-Atlas+Palette, Fonts, Masken, Texte, Musikdaten
├─ src/Traders.Core        Spielzustand, RNG, Int16-Helfer, Phasenmaschine, Regeln, KI
│                          (kein Rendering, keine Plattform-Abhängigkeit, serialisierbar)
├─ src/Traders.Presentation Phasen-Szenen: Animation/Darstellung aus Kernzustand
├─ src/Traders.Vga         Software-Framebuffer 320×200 indiziert, Original-Blit-/Masken-Ops
├─ src/Traders.Audio       A.H.-Musikplayer + OPL2-Emulator
├─ src/Traders.App         Host (MonoGame DesktopGL): Fenster, Skalierung, Eingabe, Ton
├─ src/Traders.RefEmu      8086-Interpreter + DOS/BIOS/VGA/Timer-Stubs: führt T.EXE-Routinen
│                          und ganze Partien aus → Referenzspuren
├─ tests/Traders.Tests     xUnit: Einheiten-, Differenz- (C# vs. RefEmu) und Bildtests
├─ tools/                  Analyse-Skripte (Python), Disassemblierung, Exporte
├─ tools/originale/        SHA256SUMS der Originale (Dateien selbst im privaten Repo kaaasi/traders-referenz)
└─ docs/                   Plan, Routinenbefunde (docs/routines/*.md), Datenlayout
```

Technik: **.NET 10 (LTS)**, **MonoGame DesktopGL** (Windows/Linux/macOS), xUnit.
Build und Tests laufen bei jedem Push in **GitHub Actions**.

### Fester Takt
Original-Timerfrequenz wird nachgebildet (Simulations-Ticks), Darstellung entkoppelt.
Bildrate oder Rechnergeschwindigkeit beeinflussen keine Spielentscheidung.

## 4. Vorgehen je Routine

Für jede portierte Originalroutine (Vorlage in `docs/routines/_vorlage.md`):

1. Adresse (Segment:Offset im Ladebild), Aufgabe, Aufrufer, Phase.
2. Gelesene/geschriebene Zustandsfelder (DS-Offsets, Breiten, Vorzeichen).
3. Zufall-, Uhr-, Grafik-, Eingabe-Aufrufe in Reihenfolge.
4. C#-Umsetzung im Kern.
5. **Differenztest:** gleicher Eingangszustand → RefEmu führt Original aus, C# rechnet →
   DS-Speicher, RNG-Zustand und Grafikaufrufe müssen identisch sein. Inkl. Grenzfälle.

## 5. Meilensteine

Jeder Meilenstein endet mit einem lauffähigen Zwischenstand auf `main`, festgehalten in `STATUS.md` und `CHANGELOG.md` (keine Tags, Nutzer 10.11.2026). Ablauf in Runden: `docs/arbeitspakete.md`.

| # | Meilenstein | Inhalt | Prüfung |
|---|---|---|---|
| **M0** | Fundament | Repo, Solution, CI, Originale + Hashes, Plan | CI grün, Hash-Test |
| **M1** | Ressourcen | PP20 in C#, VGA-Atlas+Palette, Fonts, Masken, Texte, Asset-Viewer | Byte-Vergleich mit Python-Referenz; Bilder vs. Video |
| **M2** | Analyse-Infrastruktur | Segment-/Funktionskarte, Borland-Laufzeit markiert, Datenlayout DS, RefEmu startet T.EXE bis zum Menü (Framebuffer-Dump) | Menübild aus RefEmu = Video |
| **M3** | Kern-Fundament | Spielzustand (DS-Layout), Borland-`rand`, Uhr-Eingaben, Int16-Arithmetik, Phasenmodell mit Ein-/Austrittsbedingungen | RNG-Folgen vs. RefEmu |
| **M4** | Menü & Setup | Hauptmenü, Spieler 1–4, Mensch/Computer, IQ, Optionen | Spielstart-Zustand identisch |
| **M5** | Anbau | Spielfeld, Bauen/Pflanzen, HQ-Regeln, Anbau-KI inkl. Strategievorbereitung | Differenztests, KI-Züge |
| **M6** | Produktion & Wetter | Wettertexte, Produktion, Schwund, Feldbesuche-Animation | Bilanzwerte |
| **M7** | Fat Mike | Schneckenmiete, Schneckenrennen/-prämien, Zeigestab-Kontinuität | Ablauf vs. Video |
| **M8** | Auktion/Handel | A-/B-Handel, Käufer/Verkäufer, Balkenspuren, Handels-KI | Mehr-Frame-Bildvergleich |
| **M9** | Rakete & Stationen | `V-RAKETE`, Flug mit Pixelkollision, Piraten (Kanone, Laser, Abgang), Stations-KI | 160 Flugfälle |
| **M10** | Ereignisse & Katastrophen | Ereignisfenster, Geld-Events, Katastrophen + Sternvorführung (`V-BERGE`, Zufall!) | RNG-Verbrauch |
| **M11** | Wochenbilanz & Einkauf | Bilanzgraph, Vermögen, Einkaufsphase | Werte vs. RefEmu |
| **M12** | Roboterkampf | `V-KAMPF`, Robo-Store, Kampfmechanik + KI (im Vorversuch offen) | Differenztests, Video 2 |
| **M13** | Ende | Siegprüfung, Abendbild, Rang-Mosaik aus echten Werten | Endbild vs. Video |
| **M14** | Ton | A.H.-Format, OPL2-Emulation, FX-Treiber-Effekte | Hörvergleich mit Video |
| **M15** | Gesamtpartie | Komplette Partien mit aufgezeichneten Eingaben C# vs. RefEmu | Zustand pro Runde identisch → **v1.0 Originalport** |

## 6. Erweiterungen (nach v1.0, Reihenfolge änderbar)

| # | Erweiterung | Kernpunkte |
|---|---|---|
| E1 | Testsprünge | Gültige spielbare Phasen-Startzustände, Rückkehr zur Partie (praktisch ab M5 als Entwicklerwerkzeug) |
| E2 | IQ je Computergegner | DIM/GOOD/SHREWD/MEAN je Spieler; Originalmodus = globaler Wert |
| E3 | Cheatmenü | Geld, Schnecken, Rohstoffe, schnellere Bewegung, Unverwundbarkeit – Wirkungsumfang je Cheat vorher festlegen; als Modifikator gespeichert |
| E4 | Übersetzungen | Text-IDs mit Parametern, Deutsch als erste Zusatzsprache, Unicode-Schrift |
| E5 | Moderne Grafik | Semantische Asset-IDs, austauschbares Grafikpaket, Original-VGA wählbar |
| E6 | Netzwerk mit Host | Gemeinsamer Kern, geprüfte Spielerbefehle, Server entscheidet Zufall/KI/Cheats |
| E7 | Dedicated Server | Headless-Host, Lobby, Wiederbeitritt, versionierte Zustände |

Netzwerkfähigkeit wird ab M3 im Kern vorbereitet (Befehle statt direkter Zustandsänderung,
serialisierbarer Zustand), damit E6/E7 keinen Umbau erfordern.

## 7. Risiken

| Risiko | Gegenmaßnahme |
|---|---|
| Fehlende Originalzufallsaufrufe | Differenztests prüfen RNG-Zustand nach jeder Routine |
| Grafik ist regelrelevant | Kanonischer Atlas im Kern, Tests mit vertauschtem Grafikpaket |
| Kampf/Ton schlecht verstanden | Eigene Meilensteine, Amiga-Fassung als Zweitquelle |
| RefEmu-Aufwand | Nur benötigte DOS/BIOS/VGA-Funktionen; Grafiktreiber-Aufrufe abfangen statt VGA-Hardware voll zu emulieren |
