# Befund: Musiktreiber `FXDRIVER.EXE` V1.06

Stand 22.11.2026, Spur `musik` (R9, Teil 1). Belege: entpacktes Abbild (`tools/fxdriver.py`, Disassemblierung mit `ndisasm`,
Ausgabe nur lokal unter `out/fxdriver/`), RefEmu-Proben `tests/Traders.Tests/Audio/MusicFxDriverImageTests.cs` und
`tests/Traders.Tests/App/OriginalSoundProbeTests.cs` (**P**). Ohne **P** aus dem Code gelesen (**C**), Vermutungen **V**.
Ergänzt `analyse/TON.md` §2 (Schnittstelle aus Sicht von `T.EXE`); dieser Befund beschreibt das Innere des Treibers als
Vorlage für den Player in `src/Traders.Audio/Music/` (Teil 2).

## 1. Abbild, Offsets, Marker

- `FXDRIVER.EXE` ist mit **LZEXE 0.91** gepackt (Kennung am Dateianfang entfernt, Entpacker an CS:000E des gepackten
  Programms). Entpacken: `tools/fxdriver.py` (Werkzeug) bzw. `Traders.Assets.FxDriverImage` (Laufzeit). Ergebnis: Ladeabbild
  0x6490 Bytes, 277 Relokationen, Einsprung 0000:0000, Stapel 064A:0080. Gegenprobe gegen den RefEmu: Nach
  `RunResidentProgram` stimmt der Speicher ab dem Ladesegment im Codeteil Byte für Byte mit Abbild + Relokation überein (**P**).
- **Offsets** `OOOO` (Marker `[ORIGINAL-FX] OOOO`, Testkennung `[DIFFERENTIAL-FX] OOOO`) sind Byte-Offsets im Ladeabbild:
  0 = erstes Byte nach dem MZ-Kopf des entpackten Programms = Ladesegment (PSP + 10h). `tools/abdeckung.py` prüft, dass der
  Offset im Abbild liegt, hier als `` `OOOO` `` vorkommt und einen eigenen Differenztest hat.
- **Segmente** (relativ zum Ladesegment): Hauptprogramm (Borland C++ 1991, Start bei 0000) mit eigenem DGROUP `05E1`
  (Texte „FX-Driver V1.06 …“, Installationsmeldungen); **residenter Treiber**: Code `0292` (Abbild ab `2920`), Daten `0481`
  (Abbild ab `4810`, 1600h Bytes). Der Treiber lädt DS in jeder Routine mit `mov …,0481h` (Operand am Einsprung: `4287`).
  Umrechnung: Treibercode CS:x → Abbild `2920` + x, Treiberdaten DS:x → Abbild `4810` + x.
- `int 64h` zeigt auf CS:195D = `427D` (**P**). Sprungleiste CS:1CD6 = `45F6`, 1Dh Einträge.

## 2. `int 64h`-Funktionen

Eingang am Einsprung: AX, BX, CX, DX nach DS:007E/0080/0082/0084; Rückgabe aus denselben Zellen (AX bleibt die Funktion).
Die Routinen entsprechen in Aufbau und Namen der Beispiel-Schnittstelle von Ad Lib Inc. (SoundWarmInit, SetMode,
SetVoiceTimbre, NoteOn …, **V** Herkunft; das Verhalten unten ist aus dem Code gelesen).

| Fn | Handler (Abbild) | Routine (Abbild) | Wirkung | Spiel |
|---|---|---|---|---|
| 00h | `42DB` | `3657` | Stück laden: BX:CX = Dateiname (ohne Punkt wird `.CMF` angehängt), Format §3; bei `[42]` = 0 danach SetMode(`[44]`) | ja |
| 01h | `42EC` | `4150` | Start: `[50]` = 1, Kanalzustand `[01A6]` (11 × 4 Bytes) = 0, Grundwerte aller 18 Operatoren (`2DE1`), `int 8` auf CS:1262 (über DOS 25h), NoteOff 0…10, `[6A]` = `[6C]` = 0 | ja |
| 02h | `42F3` | `41EE` | Ausgabeart `[42]` (BX > 1: BX − 1; 0/1: SetMode) – nur ohne AdLib | nein |
| 03h | `42FF` | `4236` | Stopp: Fahrt `[17A]` = `[19E]` = 0, NoteOff 0…10, Lautsprecher aus (`1CA4`), `int 8` zurück, `[50]` = 0 | ja |
| 04h | `430E` | `2DE1` | Grundwerte aller Operatoren schreiben | nein |
| 05h | `4315` | `2993` | Fahrt = 0, Grundzustand: Register 01…F5 = 0, Reg 04 = 06, Zustände, SetMode(0), SetGParam(0,0,0), SetPitchRange(1), SetWaveSel(1) | ja |
| 06h | `4324` | `296B` | Karte suchen (BX = 0: 388h, 288h, 318h, 218h; je Timer-Test `3403` und `2993`), dann SetMode(1), `[50]` = 0; ohne Karte `[42]` = 2 | ja |
| 07h…0Fh | `43A5`… | `2A3F`, `2AA1`, `2AF5`, `2B1D`, `2B45`, `2BFA`, `2C79`, `2CBB`, `2D75` | SetMode, SetWaveSel, SetPitchRange, SetGParam, SetVoiceTimbre (BX, Instrument CX aus der Tabelle), SetVoiceVolume, SetVoicePitch, NoteOn, NoteOff | nein |
| 10h | `4439` | – | Puffer BX = DX:CX (Offset:Segment) nach DS:1430 + 4·BX | ja |
| 11h, 15h | `4453`, `44A4` | – | Version (1, 0), Puffergröße 0C04h | nein |
| 12h | `4462` | – | BX = `[50]` (1 = läuft, 0 = steht) | ja |
| 13h | `446B` | – | Abmelden: Lautsprecher-Ausgabe zurücksetzen, `int 64h`-Vektor (`[142E]`) auf 0 (**V**) | nein |
| 14h, 1Ch | `4490`, `45BF` | `3A14`, `35DB` | Klangdateien laden | nein |
| 16h | `44AD` | – | Zelle der aktuellen Zeile für Kanal BX lesen | nein |
| 17h | `4579` | `2B45` | wie 0Bh | nein |
| 18h | `4593` | – | Wiederholen `[7C]` = BX | ja |
| 19h | `459B` | – | Ausblenden: `[19E]` = 1, Richtung `[17A]` = 0 | nein (`218B:016F` ohne Aufrufer) |
| 1Ah | `45A9` | – | Einblenden: `[19E]` = 7Fh, Richtung `[17A]` = 1 | ja |
| 1Bh | `45B7` | – | Schrittweite der Fahrt `[1A0]` = BX | ja |

Abfolge im Spiel (**P**, TON §3): beim Hochlauf 06h, 05h, 24 × 10h; je Stück 12h, 00h, 18h, 1Bh, 1Ah, 01h; Stopp 03h.
OPL-Schreibungen außerhalb des Takts entstehen in 06h, 05h, 00h (SetMode) und 01h (Grundwerte, NoteOff); 03h schreibt
NoteOff.

## 3. Stückformat („A.H.“, Endung `.CMF`, kein Creative CMF)

Gelesen von `3657` mit `read()` in dieser Reihenfolge (**C**; alle drei Stücke werden damit genau bis zum Dateiende
gelesen, Werkzeugprobe):

1. 5 Bytes Kennung, verglichen mit „A.H.“ samt Nullbyte; sonst Abbruch.
2. 198 Bytes Reihenfolge nach DS:008A: 99 **Little-Endian**-Wörter ab Dateioffset 5. Einträge ≥ 40h beenden das Stück
   (in den Dateien 63h). `Traders.Assets.MusicModule` liest dieselben Werte als Big-Endian ab Offset 4 – gleiches Ergebnis nur,
   solange alle Werte < 100h sind; der Player folgt dem Treiber.
3. Je ein Wort: Zahl der Muster → `[40]`, Tempo-Exponent → `[48]`, Schlagzeugmodus → `[44]`, Zahl der Instrumente (lokal).
4. Instrumente zu je 69 Bytes, die ersten 64 nach DS:01EA + 45h·i (vorher mit Nullen und dem Namen aus DS:160B belegt),
   weitere in einen Wegwerfpuffer. Aufbau: 13 Wörter Operator 1, 13 Wörter Operator 2 (je KSL, MULT, FB, AR, SL, EG, DR, RR,
   TL, AM, VIB, KSR, FM; genutzt wird das niedrige Byte), Wellenform 1, Wellenform 2 (Wörter), 13 Bytes Name.
5. Muster: zuerst jedes der `[40]` Muster leeren (64 Zeilen × Kanäle 0…10: Zelle 00 00 7F 20; Länge `+0C02` = 40h, `+0C00` = 1),
   dann je Muster eine Folge von Einträgen: Zeile (1 Byte); FFh beendet das Muster, sonst Kanal (1 Byte) und 4 Bytes Zelle
   (bei Zeile ≥ 40h verworfen). Nach FFh: Länge = erste Zeile, deren Zelle in **Kanal 0** Byte 0 = 1 hat (sonst 40h).
   Puffer: 24 × 0C04h Bytes (Fn 10h), Zeile 30h Bytes (12 Zellen, genutzt 11), Zelle 4 Bytes. Mehr als 24 Muster prüft der
   Treiber nicht (**C**).

**Zelle** (Byte 0…3): `n` Note bzw. Befehl, `i` Instrument (1…64, 0 = keins), `v` Lautstärke (0…7Fh), `b` Tonhöhenbeugung
(oberes Byte, 20h = Mitte). `n` ≥ 0Ch ist eine Note (NoteOn rechnet `n` − 12, Oktave = (`n` − 12) / 12 − 1, Rest → F-Zahl);
`n` = 4 schaltet ab, `n` = 1 markiert nur beim Laden das Musterende, andere Werte < 0Ch wirken nur ohne AdLib.

## 4. Takt `3B82` (CS:1262)

Aufgerufen über die Kette des Zeitgebers von `T.EXE` (jeder vierte Tick, TON §2); ruft zuerst den alten Vektor (`[16A]`).

1. `[50]` > 1: herunterzählen, fertig (wird im Spiel nicht genutzt: `[50]` ist 0 oder 1).
2. Muster `p` = Reihenfolge[`[6C]`]. Ist `p` ≥ 40h: ohne Wiederholen → **Ende** (Schritt 6); mit Wiederholen `[6C]` = 0, `p` neu.
3. Für Kanal `k` = 0…10 (`3BED`, CS:12CD) die Zelle (`p`, Zeile `[6A]`, `k`); Zustand je Kanal `[01A6 + 4k]`: Note, Instrument,
   Lautstärke, Beugung. Nur bei `n` > 0Bh:
   - Instrument: in Zeile 0 immer, sonst nur bei `i` ≠ gespeichertem Wert: bei `i` ≠ 0 SetVoiceTimbre(`k`, Instrument `i`);
     gespeichert wird `i` (auch 0).
   - Lautstärke: in Zeile 0 immer, sonst wenn `v` − `[19E]` (vorzeichenbehaftet) ≠ gespeichertem Byte: SetVoiceVolume(`k`,
     max(`v` − `[19E]`, 0)); gespeichert wird das Byte `v` − `[19E]`.
   - Beugung: in Zeile 0 immer, sonst wenn `b` ≠ gespeichert: SetVoicePitch(`k`, `b`·100h + FFh) – also 20FFh statt 2000h in
     der Mitte, die F-Zahl wird dann neu gerechnet (**C**).
   - nur im ersten Untertakt (`[4A]` = 0): NoteOff(`k`), NoteOn(`k`, `n`), Note merken.

   Unabhängig davon: `n` = 4 → Note = 0, NoteOff(`k`). Die Prüfungen laufen in **jedem** Untertakt erneut; in Zeile 0 werden
   Instrument, Lautstärke und Beugung also je Untertakt neu geschrieben, während einer Fahrt die Lautstärke je Takt.
4. **Tempo:** `[4A]` + 1; erreicht es 2^(`[48]` − 1), dann `[4A]` = 0 und nächste Zeile. Zeilen ≥ Musterlänge → Zeile 0, `[6C]` + 1
   (bei 40h → 0). Die Stücke haben `[48]` = 2 (TRADINTR, TRAD_END) bzw. 3 (TRADMENU): 2 bzw. 4 Takte je Zeile. `[4A]` wird
   beim Start (Fn 01h) **nicht** zurückgesetzt (**C**) – nach einem Stopp mitten in einer Zeile beginnt das nächste Stück im
   alten Untertakt.
5. **Fahrt** (`40B4`, nach den Kanälen): bei `[19E]` ≠ 0: Richtung 1 (ein) `[19E]` −= min(`[1A0]`, `[19E]`); Richtung 0 (aus)
   `[19E]` += `[1A0]`, bei ≥ 80h **Ende**. `[19E]` wird von der Lautstärke jeder Note abgezogen; Fn 1Ah mit Schritt 7Fh
   spielt also den ersten Takt stumm und ist danach voll laut, Schritt 3 blendet über 43 Takte ein.
6. **Ende** (`405C`): Lautsprecher aus, NoteOff 0…10, `int 8` über DOS 25h zurück, `[50]` = 0, `[6C]` = 41h.

## 5. AdLib-Schicht (Register, Tabellen)

- Schreiben `2928` (CS:0008): Register an `[1D6]`, 6 Lesezugriffe, Wert an `[1D6]` + 1, 35 Lesezugriffe.
- Schlagzeugmodus (`[1530]` = 1, 11 Stimmen: 0…5 melodisch, 6 Basstrommel, 7 Snare, 8 Tom, 9 Becken, 10 Hi-Hat), sonst
  9 Stimmen. SetMode(1) setzt zuvor Beugung und Tonhöhe der Stimmen 7/8 (`[1E6]`/`[1E8]` = 2000h, `[177]` = 1Fh,
  `[178]` = 18h) und schreibt die F-Zahlen.
- **SetVoiceTimbre** `2B45`: Parameter der 1–2 Operatoren der Stimme (Tabellen DS:0000 melodisch = `4810`, DS:0012 Schlagzeug =
  `4822`, je 2 Bytes, FFh = kein zweiter) in den Operatorspeicher DS:1332 (18 × 14 Bytes), dann alle Register des Operators in
  der Folge BD, 08, 40, C0, 60, 80, 20, E0 (`3031`).
- **Register** je Operator (Versatz DS:15A6 = `5DB6`, Träger-Kennung DS:15B8 = `5DC8`, Stimme des Operators DS:15CA = `5DDA`
  bzw. DS:15DC = `5DEC`): 20h = AM·80h + VIB·40h + EG·20h + KSR·10h + MULT; 40h = KSL·40h + Dämpfung; 60h = AR·10h + DR;
  80h = SL·10h + RR; C0h (Stimme) = FB·2 + (FM = 0); E0h = Wellenform & 3 nur bei `[16E]` ≠ 0; 08h = `[150]`·40h;
  BDh = AM-Tiefe·80h + VIB-Tiefe·40h + Schlagzeug·20h + Schlagbits `[132E]`.
- **Lautstärke** `307D`: t = 63 − (TL & 63); skaliert mit Stimmlautstärke `[15A + Stimme]` als (vol·t + 40h) >> 7, wenn der
  Operator Träger ist, FM = 0 hat oder im Schlagzeugmodus zu Stimme > 6 gehört; Register = 63 − t + KSL·40h.
- **Frequenz** `34AD` (CS:0B8D): Oktave = Note / 12 − 1, F-Zahl aus DS:0028 = `4838` (12 Wörter); Beugung ≠ 2000h verschiebt
  linear zur Nachbarnote im Abstand `[1330]` Halbtöne (`mul`, 13 Bits nach rechts) und normalisiert die Oktave über 3FFh bzw.
  unter DS:0028; schreibt A0h + Stimme und B0h + Stimme = Schlüssel `[151]` | Oktave·4 | F-Zahl >> 8 (Byte, eine Oktave −1
  wird nicht abgefangen) und merkt B0h in `[194]`.
- **NoteOn** `2CBB`: Note − 12 (≥ 0); melodisch: Note merken, Schlüssel 20h, Frequenz. Schlagzeug: Stimme 6 setzt `[176]` und
  schreibt Stimme 6; Stimmen 7/8 schreiben bei geänderter Tom-Note Stimme 8; **alle** Schlagzeugstimmen setzen danach `[177]` =
  Note, schreiben Stimme 7 und das Schlagbit (`[132E]` |= Maske DS:1531 = `5D41`), dann BDh.
- **NoteOff** `2D75`: melodisch Schlüssel 0 und B0h mit gelöschtem Bit 5 aus `[194]`; Schlagzeug Schlagbit löschen, BDh.
- Grundwerte der Operatoren (`2DE1`): melodisch DS:1536/1544 (= `5D46`/`5D54`, Modulator/Träger), Schlagzeug DS:1552 … 1598
  (= `5D62` … `5DA8`).

## 6. Port (Teil 2)

`src/Traders.Audio/Music/FxMusicDriver*.cs` überträgt den Treiber als Zustandsmaschine auf einer Kopie seines Datensegments
(aus `FxDriverImage`, Offsets wie im Original); Tabellen werden dort gelesen, nie in den Code kopiert. Ausgabe: Folge der
OPL-Schreibungen (`Written`, mit Tick `WrittenAt`/`OplWrite`); der Host ruft `Tick(timerTick)` je Aufruf von CS:1262, solange
`TimerActive`. **Einheit des Ticks** (R9M-1, Leitstelle 23.11.2026): wie bei den Effekten ein Zeitgeberticks (72,82 Hz, Teiler
`4000h`); der Aufrufer ruft jeden vierten Tick (Abstand genau 4) und gibt die Nummer mit. Die Phase φ (0…3, Tick des ersten Aufrufs nach
Fn 01h, ⌊(Zeit − Zeit von Fn 01h)/4000h⌋) hängt vom Zähler im Zeitgeber von `T.EXE` ab, nicht vom Treiber; sie liefert allein der
Host. Schreibungen der Funktionen außerhalb eines Takts tragen `CurrentTick`, den der Host davor setzt. RefEmu-Probe (Review): φ = 3 beim
Vorspann, 2 bzw. 1 im Menü (`+1,01 s` bzw. `+1,07 s`), 0 am Ende (`+1,03 s`).

| Original (Abbild) | Port | Anmerkung |
|---|---|---|
| `4630` Start des Treibers (CS:1D10) | `Install` | Fahrt aus, Schritt 1, Karte an 388h angenommen, SetMode(1); `[APPROX]` Statusprüfung |
| `427D` Verteiler, `4324` Fn 06h, `296B`/`3403` Karte, `2993` Grundzustand | `Call`, `FindCard`, `Detect`, `WarmInit` | Registerfolge des Timer-Tests ohne Statuslesen |
| `3657` Laden, `4150` Start, `4236` Stopp | `Load`, `Start`, `Stop` | Lautsprecher (`1CA4`) entfällt |
| `3B82` Takt, `3BED` Kanal, `405C` Ende, `40B4` Fahrt | `Tick`, `PlayChannel`, `End` | Ablauf §4 |
| `2928` Schreiben, `2A3F`, `2AA1`, `2AF5`, `2B1D` | `Out`, `SetMode`, `SetWaveSelect`, `SetPitchRange`, `SetGlobalParameters` | |
| `2B45`, `2BFA`, `2C79`, `2CBB`, `2D75` | `SetVoiceTimbre`, `SetVoiceVolume`, `SetVoicePitch`, `NoteOn`, `NoteOff` | |
| `2DE1`, `2F55`, `2EFA`, `3031` | `DefaultOperators`, `SetSlotFromBytes`, `StoreSlot` | Folge BD, 08, 40, C0, 60, 80, 20, E0 |
| `307D`, `3151`, `3176`, `31D8`, `322B`, `327E`, `3326`, `337E` | `WriteLevel`, `WriteNoteSelect`, `WriteFeedback`, `WriteAttackDecay`, `WriteSustainRelease`, `WriteCharacter`, `WriteRhythm`, `WriteWave` | Register 40h, 08h, C0h, 60h, 80h, 20h, BDh, E0h |
| `33C8`, `34AD` | `SetFrequency`, `Frequency` | A0h/B0h, Schatten `[194]` |

**Differenztests** (`tests/Traders.Tests/Audio/Music*`, **P**): (a) Treiber im RefEmu und Port mit denselben int-64h-Aufrufen
und Taktaufrufen (IRQ 0 maskiert, Takt = Aufruf von CS:1262): OPL-Schreibungen je Aufruf und Takt, Rückgaberegister,
Datensegment (bis auf gesicherten `int 8`-Vektor `016A`–`016D` und DOS-Registerblock `017E`–`0191`) und Musterpuffer gleich – alle
drei Stücke je ≥ 60 s mit den Merkern des Spiels, Vorspann bis zum Ende, Menü mit Wiederholen über den Umlauf, Stopp mitten in
einer Zeile und Wechsel, Ausblenden bis zum Ende. (b) Spiel mit residentem Treiber (`runFxDriver`): alle int-64h-Aufrufe und
Taktaufrufe aus `T.EXE` werden in den Port gespielt, die Schreibungen aus dem Treibercode je Ereignis verglichen – Vorspann
beim Hochlauf über ≥ 60 s (das Spiel startet im Hauptmenü kein anderes Stück), Menü- und Endmusik über `218B:0094(1, 2)` bzw.
`(2, 3)` je ≥ 60 s; dabei 1108…1113 Taktaufrufe in 61 s (jeder vierte Zeitgebertick, TON §2 bestätigt). Der Port bekommt je Aufruf
⌊(`Machine.Now` − T₀)/4000h⌋ (T₀ = Zeit von Fn 01h); der Test prüft Abstand 4 und die Phase.

Offen: Weg ohne AdLib (`[42]` ≠ 0, Lautsprecher) entfällt; nicht genutzte Funktionen (02h, 04h, 07h–0Fh, 13h, 14h, 16h, 17h,
1Ch) sind nicht portiert und geben nur die Register zurück.
