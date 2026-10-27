# Portierungsstatus

Stand: 27.10.2026 · Repositorymodernisierung und AP3 Stand 2

**Legende:** ✅ portiert und gegen Original geprüft · 🟢 portiert, Tests grün, Originalvergleich
(RefEmu) noch offen · 🟡 in Arbeit · 🔍 analysiert, noch nicht portiert · ⬜ offen

## 1. Meilensteine

| # | Meilenstein | Status | Bemerkung |
|---|---|---|---|
| M0 | Fundament (Repo, CI, Originale, Plan) | ✅ | Hash-Prüfung der Originale in CI |
| M1 | Ressourcen | 🟢 | PP20, VGA-Atlas (Kopf korrigiert), Palette, Texte, Schriften, Masken, Musik-Kopf, Asset-Viewer |
| M2 | Analyse-Infrastruktur / RefEmu | 🟢 | 286-Emulator (SingleStepTests grün), DOS/BIOS/VGA, T.EXE bis Hauptmenü, Bildabzug, Differenztests (AP1) |
| M3 | Kern-Fundament | 🟡 | RNG ✅, 16-Bit-Arithmetik, Spielzustand/DS-Layout + Spielstand (AP3) fertig; Phasenmodell offen (AP5) |
| M4 | Menü & Setup | ⬜ | |
| M5 | Anbau | ⬜ | |
| M6 | Produktion & Wetter | ⬜ | |
| M7 | Fat Mike / Schnecken | ⬜ | |
| M8 | Auktion / Handel | ⬜ | |
| M9 | Rakete & Stationen / Piraten | ⬜ | |
| M10 | Ereignisse & Katastrophen | ⬜ | |
| M11 | Wochenbilanz & Einkauf | ⬜ | |
| M12 | Roboterkampf | ⬜ | |
| M13 | Ende / Rang | ⬜ | |
| M14 | Ton | ⬜ | |
| M15 | Gesamtpartie = v1.0 | ⬜ | |
| E1–E7 | Erweiterungen | ⬜ | nach v1.0, siehe Plan |

## 2. Ressourcen

| Datei | Inhalt | Laden | Format verstanden | Status |
|---|---|---|---|---|
| `V-*.PP` (10) | VGA-Bilder: Palette + Größenkopf + 320×256-Atlas | `VgaImage` | ja (Kopf bei 768, Pixel ab 772 – Befund AP2) | 🟢 |
| `E-*.PP` (10) | EGA-Bilder | entpackt | nein (nur Gegenprüfung) | ⬜ |
| `FONTS.PP` | 2 Schriften (16×12, 16×5) | `FontSet` | ja (Breiten aus Code offen) | 🟢 |
| `MASKS.PP` | 1-Bit-Masken 320×200 | `MaskImage` | ja (Zuordnung aus Code offen) | 🟢 |
| `ENGLISCH.TXT` | 303 Spieltexte | `TextTable` | ja | 🟢 |
| `*.CMF` (3) | Musik, eigenes Format „A.H.“ | `MusicModule` (Kopf) | teilweise: Reihenfolge, Patternanzahl | 🟡 Rest in M14 |
| `FXDRIVER.EXE` | Ton-Treiber (Linel), `int 64h` | – | Schnittstelle teilweise | 🔍 |

## 3. Originalmodule (Codesegmente in T.EXE)

Aufgaben stammen aus dem Vorversuch bzw. eigener Analyse; „?“ = noch nicht zugeordnet.
Vollständige Liste der Einsprünge: [docs/analyse/segments.md](docs/analyse/segments.md).

| Segment | Größe | Aufgabe | Status |
|---|---:|---|---|
| `0000` | 0x5350 | Turbo-C-Laufzeit (Startup, Long-Arithmetik, Datei, Zeit) | 🔍 nur benötigte Teile werden nachgebildet |
| `0535` | 0xD40 | Roboterkampf (wahrscheinlich, AP3) | 🔍 |
| `0609` | 0x21C0 | Anbau / Spielfeld / HUD | ⬜ |
| `0825` | 0xAA0 | Runden-/Bilanzdarstellung, Vermögen (`0825:06CB`) | ⬜ |
| `08CF` | 0x19A0 | Einkaufsphase | ⬜ |
| `0A69` | 0x2D0 | Grafik: Text-/Rahmenhilfen (AP2) | 🔍 |
| `0A96` | 0xD10 | Grafik: Befehlsliste, 17 Befehlstypen (AP2) | 🔍 |
| `0B67` | 0x380 | ? | ⬜ |
| `0B9F` | 0x8A0 | Handel, setzt Warenpreise `DS:4898` (wahrscheinlich, AP3) | 🔍 |
| `0C29`–`0E9E` (ohne 0E31) | | ? | ⬜ |
| `0E31` | 0x6D0 | Neue Partie, Landerzeugung, Stationen (AP3) | 🔍 |
| `0EDE` | 0x23F0 | Roboterkampf / Beute (wahrscheinlich, AP3) | 🔍 |
| `111D` | 0x860 | Katastrophen + Sternvorführung | ⬜ |
| `11A3`–`16D6` | | ? | ⬜ |
| `1777` | 0x540 | Wochenpreise (Öl, Raketen, Lotto, Land), nutzt Uhr (AP3) | 🔍 |
| `17CB` | 0xA10 | Ereignisfenster, Geld-Events | ⬜ |
| `186C` | 0xC10 | Bild laden `186C:0002(i)`, setzt `DS:09C8` | 🔍 |
| `192D` | 0x1D60 | Menüs: Haupt, Schwierigkeit, Einstellungen, Disk, Steuerung (AP3) | 🔍 |
| `1B03` | 0x1650 | Stationswahl, Raketenflug, Piraten | ⬜ |
| `1C68` | 0x1870 | Robo-Store / Roboterkampf (wahrscheinlich, AP3) | 🔍 |
| `1DEF` | 0x8F0 | Bonusrennen | ⬜ |
| `1E7E` | 0xE50 | ? (viel Zufall) | ⬜ |
| `1F63` | 0x1500 | Fat Mike | ⬜ |
| `20B3` | 0xD80 | Schlussauswertung, Abend-/Endbild | ⬜ |
| `218B` | 0x1C0 | Ton: Treiberaufrufe `int 64h` (Startfolge im RefEmu protokolliert) | 🔍 |
| `21A7`–`21AF` | | ? (21A7: Ton-Effekte) | ⬜ |
| `21FD` | 0x980 | Hilfsroutinen: Zufall ✅, Code-Abfrage `0687`, Warten, Palette/Blende `026C…04DA` | 🟡 |
| `2295` | 0xE0 | Zahl → Text mit Füllzeichen | 🔍 |
| `22A3` | 0x930 | Wettertexte / -vorführung | ⬜ |
| `2336` | 0x970 | Spielstand speichern/laden (`000A`, `0539`, `051E`, `0947`) (AP3) | 🟢 |
| `23CD` | 0x3F90 | Grafiktreiber (Assembler, VGA = Mode X, Sprites `06E9/071C`) (AP2) | 🔍 |
| `27C6` | 0x5B0 | Interrupt-Handler: int 9 Tastatur, int 8 Timer, int 21h (AP1) | 🔍 |
| `2821`–`2850` | | ? | ⬜ |

## 4. Werkzeuge

| Werkzeug | Zweck | Status |
|---|---|---|
| `src/Traders.Viewer` | Asset-Viewer (MonoGame): Atlanten, Masken, Schriften; Maus zeigt Koordinate/Palettenindex; `--screenshot` für CI | 🟢 läuft unter virtuellem Bildschirm (13 Screenshots), Windows-Build als Download |
| `src/Traders.Tools` | Export aller Ressourcen als PNG/BIN (manueller Diagnoseworkflow) | 🟢 |
| `tools/segmap.py` | Segmentkarte + Disassemblierung | 🟢 |
| `src/Traders.RefEmu` | Referenz-Emulator: führt T.EXE aus, `OriginalProgram` für Differenztests | 🟢 |
| `tools/datenlayout/` | DS-Zugriffe, Spielstandfelder aus der Disassemblierung | 🟢 |
| CI (`.github/workflows/ci.yml`) | Linux + Windows bauen/testen, Bericht in Branch `ci-results`, Windows-Viewer als Download | ✅ |
| `tools/ci-wait.sh` | Wartet begrenzt auf den JSON-Nachweis und holt die Zusammenfassung (alle Agenten) | ✅ |
| `tools/pruefe-branch.py` | Leitstelle: Prüfung eines Paket-Branches vor der Übernahme | ✅ |
| `tools/lagebericht.py` | Leitstelle: anbieterunabhängige Übersicht aller Paket-Branches und CI-Nachweise | ✅ |
| `reference/` | Referenzbilder aus beiden Videos (alle 4 s bzw. 5 s) für alle Chats | ✅ |
| `experiments/apN/` | Explizite Analysen über manuellen Diagnoseworkflow | 🟢 manuell auslösbar |

## 5. Einzelne Routinen

| Adresse | Funktion | C# | Befund | Status |
|---|---|---|---|---|
| `21FD:01BA` | `random(n)` inkl. Uhr-Neusaat | `Core/OriginalRandom.cs` | [Befund](docs/routines/21FD_01BA_random.md) | ✅ Differenztest gegen Original (AP1) |
| `0000:4F4E` / `5058` / `4FA0` | Long-Multiplikation, unsigned Shift, unsigned Modulo | in `OriginalRandom` | im RNG-Befund | 🟢 |
| `0000:3405` | `time()` (Borland: lokale Zeit + 18000 s, −3600 s Sommerzeit) | Eingabe `IClockInput` | im RNG-Befund | ✅ |
| `21FD:0687` | Code-Abfrage vor dem Hauptmenü (setzt RNG aus der Uhr) | – | [Befund](docs/routines/21FD_0687_codeabfrage.md) | 🔍 |
| `2336:000A` / `0539` | Spielstand speichern / laden | `Core/State/SaveGame.cs` | [Befund](docs/routines/2336_000A_spielstand.md) | ✅ Differenztests gegen RefEmu |

## 6. Parallele Arbeitspakete

Vergabe und Stand: [docs/ARBEITSPAKETE.md](docs/ARBEITSPAKETE.md). AP1: erster Stand übernommen; AP3: Stand 2 einschließlich Savegame-Differenztests übernommen (PR #2). AP2 in Arbeit. AP4 Ton offen; AP5 hat einen Analysebranch, Übergabe ausstehend. Übernahme nach
`main` nur durch die Leitstelle.

## 7. Bekannte offene Punkte

- Differenztests gegen das ausgeführte Original: `random()` sowie Speichern/Laden (AP3 Stand 2); weitere Routinen folgen mit den Paketen.
- Die Videos zeigen eine andere Fassung (© 1991) als unsere T.EXE (© 1992). Bei Abweichungen gilt der RefEmu.
- IQ ist im Original ein globaler Wert (`DS:2E8E`); IQ je Gegner (E2) ist eine echte Erweiterung.
- Kampf und Ton waren schon im Vorversuch unverstanden.


## Repositorymodernisierung

Arbeitsstand und noch notwendige Aktivierung: [docs/UMSETZUNGSSTAND.md](docs/UMSETZUNGSSTAND.md).
