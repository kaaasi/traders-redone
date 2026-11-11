# 22A3:0003 – Produktion der Woche (mit 22A3:0750)

- **Ladebild-Adresse:** `22A3:0003` (Dateioffset `0x26033`); Anteile `22A3:0750`.
- **Aufgabe:** Ertrag jedes Feldes (4–60) ins Raster `DS:48F4` (Stride 26; dasselbe Raster ist das Kampffeld des
  Roboterkampfs, BESTAND B5); ein produzierendes Feld verbraucht `DS:2F26` Einheiten seiner Ware.
- **Aufgerufen von / Phase:** Wochenschleife `1777:0254` (Schritt 11); `0750` außerdem aus `11A3:061E`, `17CB:073D`.
- **Parameter / Rückgabe:** keine.

## Zustand
| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `2EE7` | B[4][4] | s | Felder je Spieler und Art | r |
| `342C` | B[4][3] | u | Anteil der Art an den Produktionsfeldern in % (`0750`) | w |
| `37AA` | B | s | Spieler; zuletzt Besitzer des letzten Feldes (−1 möglich) | w |
| `08FC` / `08FE` | W | s | Spalten / Zeilen | r |
| `2EB0` / `2F00` | B | s | Schleifen Spalte / Zeile (global), danach Spalten / Zeilen | w |
| `4442`, `2EB1`, `45DA` | B[9·6] | s | Besitzer, Art, Bodengüte | r |
| `48F4` | B[9·26] | u | Ertrag je Feld | w |
| `57F0` / `57F2` | W | s | Felder ohne / mit Produktion (vorläufig) | w |
| `08E4` | B[3] | s | Ware je Art (init. Daten) | r |
| `0902` | B[3][4] | s | Güte-Zuschlag je Art und Bodengüte (init. Daten) | r |
| `112C` | B[3][6] | s | Wetter-Änderung in % je Art und Wetter (init. Daten) | r |
| `40E6` | B | s | Wetter | r |
| `2F26` | B | s | Verbrauch je Feld (`0609:1178`) | r |
| `3CF4` | W[4][4] | s | Vorrat | r/w |
| `450E`, `44F4`, `2F1A` | W[4], W[12], B[12] | s | Station, Stationsart, Station besucht | r, r, w |

## Ablauf
```
0750: je Spieler n = Σ Arten 0–2; Anteil[k] = n == 0 ? 0 : (short)(count[k]*100) / n      (Byte)
Faktor[p][k] = (byte)(Anteil * 3 / 4 + 25)                                              (lokales Feld)
57F0 = 57F2 = 0
für y = 0 … Zeilen−1, x = 0 … Spalten−1:
  ertrag[x*26+y] = 0; p = Besitzer; 37AA = p
  p == −1 oder Art 4 (HQ): weiter
  Art −2: 57F0++, weiter
  Art 3: besucht[station[p]] = 1; e = Raketenfelder[p]*6 + Stationsart[station[p]]*10
  sonst: w = Ware[Art]; vorrat[p][w] < 2F26 → 57F0++, e = 0
         sonst 57F2++, vorrat[p][w] -= 2F26; e = count[p][Art]*3 + Güte[Art][q]*10
               e = (short)(Faktor * e) / 100;  e += (short)(Wetter[Art][wetter] * e) / 100
  ertrag = clamp(e, 4, 60)
```
Alle Produkte 16 Bit (cwd nach imul), Divisionen IDIV. Station −1 schreibt `DS:2F19` und liest `DS:44F2`. Besitzer
außerhalb 0–3 oder Arten außer −2, 0–4 indizieren das lokale Faktorfeld außerhalb und lesen den Stapel; im Spiel nicht
erreichbar, der Port wirft dann `InvalidOperationException` (kein Nachbau).

## Seiteneffekte in Reihenfolge
Kein Zufall, keine Uhr, keine Ausgabe (RefEmu-Protokoll leer, `DS:10D2` unverändert). Die Ernteanzeige ist `22A3:02D3`.

## C#-Umsetzung
`src/Traders.Core/Rules/Production.cs` (`Shares`, `Apply`). Mehrrundenprobe (Schritt 11): Eingang alle r-Felder oben;
Ausgabe `342C`, `37AA`, `2EB0`, `2F00`, `48F4`, `57F0`, `57F2`, `3CF4`, `2F1A`.

## Prüfung
`tests/Traders.Tests/Rules/WealthProductionDifferentialTests.cs`: Anteile für 200 Zustände (ohne Felder, Summe 0 bei
Zählern ≠ 0), Produktion für 200 zufällige Spielfelder (alle Arten, Besitzer −1…3, Station −1…11, Extremwerte bei Wetter,
Güte, Verbrauch, Vorrat), Vorrat genau am Verbrauch und darunter, Raster 9×6, 1×1, ohne Zeilen, ohne Spalten.
