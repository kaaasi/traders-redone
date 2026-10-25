# Portierungsstatus

Stand: 25.10.2026 · letzter geprüfter Stand: `main` (CI grün)

**Legende:** ✅ portiert und gegen Original geprüft · 🟢 portiert, Tests grün, Originalvergleich
(RefEmu) noch offen · 🟡 in Arbeit · 🔍 analysiert, noch nicht portiert · ⬜ offen

## 1. Meilensteine

| # | Meilenstein | Status | Bemerkung |
|---|---|---|---|
| M0 | Fundament (Repo, CI, Originale, Plan) | ✅ | Hash-Prüfung der Originale in CI |
| M1 | Ressourcen | 🟢 | PP20, VGA-Atlas, Palette, Texte, Schriften, Masken, Musik-Kopf, Asset-Viewer; Bildschirm-Zuordnungen folgen mit den Phasen |
| M2 | Analyse-Infrastruktur / RefEmu | 🟡 | Segmentkarte + Disassemblierung fertig; 8086-Emulator offen |
| M3 | Kern-Fundament | 🟡 | RNG und 16-Bit-Arithmetik fertig; Spielzustand/DS-Layout, Phasenmodell offen |
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
| `V-*.PP` (10) | VGA-Bilder: Palette + 320×256-Atlas | `VgaImage` | ja (4 Endbytes unbekannt) | 🟢 |
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
| `0535` | 0xD40 | ? | ⬜ |
| `0609` | 0x21C0 | Anbau / Spielfeld / HUD | ⬜ |
| `0825` | 0xAA0 | Runden-/Bilanzdarstellung, Vermögen (`0825:06CB`) | ⬜ |
| `08CF` | 0x19A0 | Einkaufsphase | ⬜ |
| `0A69` | 0x2D0 | Grafik-Hilfsfunktionen (Wrapper um 0A96) | 🔍 |
| `0A96` | 0xD10 | Grafiktreiber, maskierte Blits (`0A96:013B`) | ⬜ |
| `0B67`–`0EDE` | | ? (u. a. 0E31 nutzt viel Zufall) | ⬜ |
| `111D` | 0x860 | Katastrophen + Sternvorführung | ⬜ |
| `11A3`–`16D6` | | ? | ⬜ |
| `1777` | 0x540 | ? (nutzt Uhr-Funktion) | ⬜ |
| `17CB` | 0xA10 | Ereignisfenster, Geld-Events | ⬜ |
| `186C` | 0xC10 | Ressourcenliste / Laden (`186C:0002`) | ⬜ |
| `192D` | 0x1D60 | ? (viel Zufall) | ⬜ |
| `1B03` | 0x1650 | Stationswahl, Raketenflug, Piraten | ⬜ |
| `1C68` | 0x1870 | ? | ⬜ |
| `1DEF` | 0x8F0 | Bonusrennen | ⬜ |
| `1E7E` | 0xE50 | ? (viel Zufall) | ⬜ |
| `1F63` | 0x1500 | Fat Mike | ⬜ |
| `20B3` | 0xD80 | Schlussauswertung, Abend-/Endbild | ⬜ |
| `218B` | 0x1C0 | Ton: Treiberaufrufe `int 64h` | 🔍 |
| `21A7`–`21AF` | | ? (21A7: Ton-Effekte) | ⬜ |
| `21FD` | 0x980 | Hilfsroutinen: Zufall, Warten | 🟡 |
| `2295` | 0xE0 | Zahl → Text mit Füllzeichen | 🔍 |
| `22A3` | 0x930 | Wettertexte / -vorführung | ⬜ |
| `2336`–`2850` | | ? (u. a. Ton/Treiber) | ⬜ |

## 4. Werkzeuge

| Werkzeug | Zweck | Status |
|---|---|---|
| `src/Traders.Viewer` | Asset-Viewer (MonoGame): Atlanten, Masken, Schriften; Maus zeigt Koordinate/Palettenindex; `--screenshot` für CI | 🟢 läuft unter virtuellem Bildschirm (13 Screenshots), Windows-Build als Download |
| `src/Traders.Tools` | Export aller Ressourcen als PNG/BIN (CI-Artefakt `asset-export`) | 🟢 |
| `tools/segmap.py` | Segmentkarte + Disassemblierung | 🟢 |
| CI (`.github/workflows/ci.yml`) | Linux + Windows bauen/testen, Bericht in Branch `ci-results`, Windows-Viewer als Download | ✅ |
| `tools/ci-wait.sh` | Wartet auf CI und holt den Bericht (für Claude-Sitzungen) | ✅ |
| `ci/experiment.sh`-Hook | Beliebige .NET-Analysen in CI, Ausgabe im Bericht | ✅ (auf `dev/ci-probe` geprüft) |

## 5. Einzelne Routinen

| Adresse | Funktion | C# | Befund | Status |
|---|---|---|---|---|
| `21FD:01BA` | `random(n)` inkl. Uhr-Neusaat | `Core/OriginalRandom.cs` | [Befund](docs/routines/21FD_01BA_random.md) | 🟢 |
| `0000:4F4E` / `5058` / `4FA0` | Long-Multiplikation, unsigned Shift, unsigned Modulo | in `OriginalRandom` | im RNG-Befund | 🟢 |
| `0000:3405` | `time()` | Eingabe `IClockInput` | im RNG-Befund | 🟢 |

## 6. Bekannte offene Punkte

- Differenztests gegen das ausgeführte Original fehlen noch für alles (RefEmu, M2).
- Kampf und Ton waren schon im Vorversuch unverstanden.
