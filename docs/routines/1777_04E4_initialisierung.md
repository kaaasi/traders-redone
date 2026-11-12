# 1777:04E4 – Initialisierung

- **Ladebild-Adresse:** `1777:04E4` (Dateioffset `0x1B254`), bis `retf` hinter `053F` (Segmentende `0x540`, `xor ax,ax`)
- **Aufgabe:** Grafikkarte und Tonart ermitteln, Speicher, Ressourcentabelle, Interrupts, Musiktreiber und Bildseiten einrichten.
- **Aufgerufen von / Phase:** `main` `1777:0119`, einmal je Programmlauf vor dem Programmstart `0DFD:000D`
  ([1777_000C_main.md](1777_000C_main.md)).
- **Parameter / Rückgabe:** keine; `AX` = 0 in Ordnung, 2 keine Grafikkarte erkannt, 1 Seitenpuffer nicht anlegbar
  (`0B67:0002`). `main` hält das Ergebnis in `DI`; ≠ 0 → sofort Abbau, Meldung nach Code.

## Ablauf (S: lineares Lesen; P: Probelauf mit Halt an jedem Aufruf)

| Stelle | Aufruf | geschriebene DS-Bytes im RefEmu-Lauf (P) | im Port |
|---|---|---|---|
| `04E7` | `DS:090E` = −1 → `0D73:0712` Grafikkarte erkennen | `090E` = 1 (VGA), `007D`, `146E` | `090E` = 1 fest (`[APPROX]`, nur VGA) |
| `04F3` | `DS:090E` noch −1 → Rückgabe 2 | – | übernommen (unerreichbar) |
| `04FF` | `DS:0910` = −1 → `0D73:04CA` AdLib an Port 388h prüfen, Ergebnis 0/1 nach `0910` | `0910` = 1, `2BE8` = 0388h | `0910` = Eingabe des Hosts (`[APPROX]`) |
| `050E` | `186C:021E` Heap: `farcoreleft − 1000`, Block anlegen | `0089`, `1292`, `5318–532F` | Host |
| `0513` | `186C:0544` Ressourcentabelle leeren | `4C04 + 0Bh·i` = FFh (i = 0..24) | Teil 2 der Spur (`186C`) |
| `0518`–`0522` | `0D73:00F8`, `03F9`, `06F5` | `588E–5891` (aus `06F5`, gesicherter Vektor) | Host |
| `0527` | `0D73:049B` Handler einschalten (Maske 3) | `0648`, `065D`, `115A–115C` | Host (`27C6`, R3) |
| `052C` | `218B:0002` Musiktreiber | `10A3` = 1, Heap | Ton (R10) |
| `0531` | `0B67:0002` Bildseiten; ≠ 0 → Rückgabe 1 | `023A` = 1, `2BA3–2BAF`, Heap | `Traders.Vga` |

## Zustand
| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `090E` | Wort | s | Grafikkarte (1 VGA, 0 EGA, 2 Tandy, 63h; −1 erkennen) | r/w |
| `0910` | Wort | s | Tonart (0 PC-Lautsprecher, 1 AdLib; −1 erkennen) | r/w |
| übrige | – | – | siehe Tabelle oben, Spalte „im Port“ | w |

## Seiteneffekte in Reihenfolge
Kein Zufall, keine Uhr außer `farcoreleft`; Ports der AdLib (`27C6:0511`), Interruptvektoren, Speicherblöcke.

## C#-Umsetzung
`Traders.Core/Phases/ProgramSteps.cs` – `ProgramInitialisation`: nur `090E` (fest 1) und `0910` (Eingabe des Hosts, Vorgabe 1
wie im RefEmu), jeweils nur bei −1 (Kommandozeile überschreibt). Alles Übrige ist Host bzw. andere Schicht (`[APPROX]`).

## Prüfung
- **Differenztest (T):** `ProgramMachineDifferentialTests.Program_start_to_week_0_in_lockstep_with_the_original` – Port auf dem
  Datensegment des RefEmu an `1777:0119`, Vergleich nach der Rückkehr (`011E`) über das ganze Datensegment außer den Bytes der
  Tabelle (`RefEmuProgramOracle.HostOwnedByInitialisation`) und Ergebnis 0. Gegenprobe (lokal): ohne `0910`-Zuweisung →
  „DS:0910 core FF RefEmu 01“.
- **Nicht ausgeführt:** Fehlerwege (Rückgabe 1/2), Tonart 0 (ohne AdLib im RefEmu).
