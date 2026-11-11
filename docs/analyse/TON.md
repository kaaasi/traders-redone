# Ton: FXDRIVER, Musik `218B`, Effekte `21A7`

Stand 11.11.2026, Spur `host` (R3, Teil 3). Nur Analyse, nichts portiert; die Tonausgabe des Hosts folgt mit R10. Belege:
Disassemblierung (`tools/segmap.py`; `FXDRIVER.EXE` ist LZEXE-gepackt und wurde im RefEmu entpackt – resident laufen lassen,
Segment des `int 64h`-Vektors abziehen), RefEmu-Proben `tests/Traders.Tests/App/OriginalSoundProbeTests.cs` (**P**). Ohne
**P** gelesen aus dem Code (**C**), Vermutungen als solche markiert (**V**). Schalter in DGROUP: `0910` Tonart (1 = AdLib, beim
Start über den Timer-Test an Port 388h erkannt, sonst 0 = PC-Lautsprecher), `10A7` Ton an/aus (Menü), `10A3` Treiber bereit,
`10A5` Musik läuft, `10A9` Nummer des geladenen Stücks.

## 1. Drei Wege

| Weg | Was | Wo | Port |
|---|---|---|---|
| Musik | drei CMF-Stücke `TRADINTR`, `TRADMENU`, `TRAD_END` (Namen ab `DS:10AB`, Zeiger `DS:1094+4i`) | residenter Treiber `FXDRIVER.EXE` über `int 64h`; Aufrufer `218B` | ja, R10 |
| Effekte mit AdLib (`0910` = 1) | Effektnummer → eigene FM-Klangfolgen | Effektmaschine im Laufzeitsegment `0000` (Daten im Codesegment), Tick-Rückruf `0000:0732` | ja, R10 |
| Effekte ohne AdLib (`0910` = 0) | PC-Lautsprecher | `2850` (Tick-Rückruf `2850:0061`, Auftrag `2850:00CC`) | **nein** (Nutzer 11.11.2026, BESTAND §5) |

Musik und Effekte schließen sich aus: ein Effektauftrag beendet laufende Musik (§3).

## 2. Treiber `FXDRIVER.EXE` und `int 64h`

V1.06 (Ausgabe beim Start), Turbo-C-Programm, bleibt mit 756h Absätzen resident und setzt nur `int 64h` (**P**). Eingang:
AX = Funktion, BX/CX/DX = Parameter; Rückgabe in AX/BX/CX/DX (AX = Funktionsnummer). Sprungleiste mit 1Dh Einträgen (0…1Ch);
größere Nummern geben die Register unverändert zurück (**P**). Die Spalte „Spiel“ nennt die Aufrufer in `T.EXE`.

| Fn | Parameter → Ergebnis | Spiel | Beleg |
|---|---|---|---|
| 00h | BX:CX = Zeiger auf Dateinamen; Treiber öffnet die CMF-Datei selbst (Kopf 198 Bytes, Instrumente je 69 Bytes, dann Ereignisse in die Puffer aus Fn 10h) | `218B:0094` | **P** |
| 01h | Wiedergabe starten | `218B:0094` | **C**, **P** (Folge) |
| 03h | Wiedergabe beenden | `218B:0153` | **C** |
| 05h | Grundzustand: Fahrtwerte (`[17Ah]`, `[19Eh]`) = 0, dann interne Routine `0073h` | `218B:0002`, `01B1` | **C**; **V** Bedeutung |
| 06h | Karte suchen: BX = 0 → Ports 388h, 288h, 318h, 218h nacheinander, sonst BX = Port; ohne Karte Fehlerwert `DS:0042` = 2 | `218B:0002`, `01B1` | **C**, **P** (BX = 0) |
| 10h | Puffer BX (0…23) = DX:CX (Offset:Segment) | `218B:0002`, 24 × je 0C04h Bytes aus einem Block von 73 824 Bytes | **P** |
| 11h | → BX = 1, CX = 0 (Version) | – | **P** |
| 12h | → BX = Zustand (`[50h]`; 0 = steht) | `218B:013A` (BL ≠ 0 = läuft) | **P** (0 ohne Stück) |
| 15h | → BX = 0C04h (Größe eines Puffers) | – | **P** |
| 18h | Wiederholen an (BX = 1) / aus (0) | `218B:0094` (Merker Bit 1) | **C**; **V** Bedeutung |
| 19h | Lautstärkefahrt abwärts (Stand 1, Richtung 0) | `218B:016F` (kein Aufrufer gefunden) | **C**; **V** |
| 1Ah | Lautstärkefahrt aufwärts (Stand 7Fh, Richtung 1) | `218B:0094` | **C**; **V** |
| 1Bh | Schrittweite der Fahrt = BX (3 langsam, 7Fh sofort) | `218B:0094`, `016F`, `01A4` | **C**; **V** |
| 02h, 04h, 07h–0Fh, 13h, 14h, 16h, 17h, 1Ch | Kanal-, Instrument- und Registerzugriffe, Freigabe | – (nicht genutzt) | nicht untersucht |

**Takt des Treibers:** Bei der Installation setzt er nur `int 64h` (und Laufzeitvektoren 0/4/5/6, **P**), `int 8` erst auf einen Aufruf aus `T.EXE` hin, also nach dessen Handlern. Der DOS-Filter `27C6:058B` leitet das
Setzen in das Weitergabeziel `27C6:[CS:0412]` um (Befund `docs/routines/27C6_0043_eingabe_zeitgeber.md`): nach dem Hochlauf zeigt
es in den Treiber (`1292:1262`), Vektor 8 bleibt `27C6:03E4`, PIT-Teiler `4000h` (**P**, Lauf mit `runFxDriver`). Die Musik läuft
damit auf jedem vierten Tick: 72,82 Hz / 4 ≈ 18,2 Hz.

## 3. `218B` – Musik

- `0002` Start (aus `1777`): nur bei `0910` = 1 und gesetztem `int 64h`. Fn 06h (BX = 0), Fn 05h, `farmalloc(73 824)`, 24 × Fn 10h,
  `10A3` = 1. Rückgabe 1 bei Erfolg (**P**).
- `0094(i, Merker)`: nur bei `10A7` und `10A3`. Erst `0153`; ist `i` ≠ `10A9`: `186C:0A6C(Name i, DS:10A0+i)` (öffnet und schließt die
  Datei, **P**), Fn 00h mit dem Namen, `10A9` = i. Dann Fn 18h (Bit 1), Fn 1Bh (Bit 0 → 3, sonst 7Fh), Fn 1Ah, Fn 01h; `10A5` = 1.
  Aufrufe: Vorspann `0DFD:02FE` (beim Hochlauf Stück 0 mit Merker 0, **P**), Menü `1777:015C` (1, Merker 2), Ende `20B3:0060` (2, Merker 3).
- `013A` Zustand (Fn 12h, ohne Treiber 0), `0153` Stopp (Fn 03h bei laufender Musik, `10A5` = 0), `01A4(x)` Fn 1Bh.
- `016F` (ausblenden), `0195` (warten bis Ende), `01B1` (Fn 06h/05h): kein Fernaufruf im Programm gefunden.

## 4. `21A7` – Effekte

- `000D` Tick-Rückruf wählen (`27C6:041B`): `0910` = 1 → `0000:06FE` (Effektmaschine zurücksetzen: Tabelle `CS:15B2` → `CS:0415`,
  Kopf/Ende `CS:0417/0419` = 0) und Rückruf `0000:0732`; sonst `2850:0061` (**P**). Aufruf beim Start (`1777`) und aus `0054`.
- `0043` (`1777`): bei `0910` = 1 `0000:0724` (Effekte anhalten, **V**).
- `0054(a, b)`, 144 Aufrufstellen, `b` = Effektnummer (beobachtet 1…52h), **`a` wird nicht gelesen**: läuft Musik (`10A5`), erst
  `218B:0153` und `000D` – der Rückruf zeigt danach auf die Effekte. Ist der Ton an (`10A7`): `0910` = 1 → `0000:0750(b)`,
  sonst `2850:00CC(b)` (**P**).
- **P3 #39 R2:** Der Ruf nach `2850:00CC` steht bei `21A7:0085`; da `21A7` nur 80h Bytes lang ist, liegt die Stelle in der
  Segmentkarte schon in `21AF` (`21AF:0005`). `2850` wird also auf zwei Wegen erreicht: als Tick-Rückruf (`000D`) und direkt
  (`0054`). BESTAND §1.2 nennt nur den ersten; Nachtrag dort durch die Leitstelle (nicht in den Dateigrenzen dieser Spur).
- **Auftragsring `0000:0750`:** schreibt `b` als **Wort** nach `CS:041B + Kopf`, Kopf = (Kopf + 1) & 0Fh, ohne Prüfung auf vollen
  Ring. Der Kopf zählt Bytes, die Maschine liest nur das niedrige Byte (`0000:0770`). Folge: das hohe Byte (bei allen Aufrufern 0)
  überschreibt den nächsten Eintrag, Eintrag 15 schreibt in `CS:042B` hinter dem Ring; der 17. offene Auftrag überschreibt den
  ersten (**P**). Für den Port: Nummern als Bytes, Ring mit 16 Plätzen, Überlauf wie im Original.
- **Tick `0000:0732`** (72,82 Hz): `0770` (Auftrag holen, Klangfolge aus der Tabelle starten), `0866`, `121C` (Stimmen
  fortschreiben, OPL-Register) – Inhalt nicht untersucht.

## 5. Für den Port (R10)

- Zwei Takte aus `Traders.App/Timing`: Effekte je Tick (72,82 Hz), Musik je vierten Tick (18,2 Hz). Der Kern löst nur Ereignisse
  aus (Effekt `b`, Stück `i` mit Merkern); Ausgabe und FM-Synthese gehören in `Traders.Audio`.
- Vor der Portierung zu klären: Ereignisformat in den 24 Puffern (Fn 00h), Bedeutung der Lautstärkefahrt (19h–1Bh), die
  Effektmaschine `0000:0750…131F` samt Tabelle `CS:15B2`. Referenz für den Vergleich: OPL-Spur des RefEmu (`AdLibStub`, Lauf mit
  `runFxDriver`).
- Offen: Tonart mit Treiber, aber ohne AdLib-Karte (Fn 06h findet nichts) – `10A3` wird trotzdem 1 (**C**: `0002` prüft das
  Ergebnis von Fn 06h nicht).
