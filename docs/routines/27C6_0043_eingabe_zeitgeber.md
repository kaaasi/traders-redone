# 27C6:0043 / 27C6:03E4 / 27C6:037A / 27C6:03C3 / 27C6:041B / 27C6:0000 / 27C6:0027 / 27C6:00AC / 27C6:00D2 / 27C6:0551 / 27C6:0576 – Tastatur, Zeitgeber, Vektoren

- **Ladebild-Adresse:** Segment `27C6` (Dateioffset `0x2B260`, Länge `0x5B0`, Assembler, kein C-Rahmen). Unterbrechungsroutinen
  ohne eigenen Einsprung: Tastatur `27C6:0043` (IRQ 1, `int 9`), Zeitgeber `27C6:03E4` (IRQ 0, `int 8`), DOS-Filter `27C6:058B`
  (`int 21h`).
- **Aufgabe:** eigene Tastatur- und Zeitgeberunterbrechung für das ganze Spiel: Tastenzustand je Scancode, letzter Make-Code,
  Make-Zähler; Zeitgeber auf etwa 72,8 Hz mit Rückruf je Tick und Weitergabe an den alten Handler bei jedem vierten Tick.
- **Aufgerufen von / Phase:** Ein- und Ausschalten nur über `0D73:040E(Maske)` (Bit 0 Tastatur, Bit 1 Zeitgeber; Zustand in
  `DS:0648`, Filter-Merker `DS:065D`), beim Start aus `1777:0527` über `0D73:049B` mit Maske 3. Rückruf setzen: `21A7:000D`
  (Tonweg, `DS:0910`; Teil 3 der Spur). Auf Taste warten: `27C6:00D2` aus `186C:0BED`, `20B3:0083`.
- **Parameter / Rückgabe:** `041B(Offset, Segment)` setzt den Rückruf; alle anderen ohne Parameter und Rückgabe.

## Ablauf (eigene Worte)

- **`0043` Tastatur:** Byte von Port 60h lesen, Port 61h Bit 7 kurz setzen und zurücknehmen (Quittung). Bit 7 = 0 (Make):
  `DS:1158` = Code, Byte `DS:115D` += 1 (läuft über), `DS:3D47 + Code` = 1. Bit 7 = 1 (Break): `DS:3D47 + (Code & 7Fh)` = 0.
  Dann spezifisches EOI für IRQ 1 (`out 20h, 61h`). Kein Ringpuffer, kein BIOS-Aufruf, keine Umschalt- oder Sperrtasten-Logik.
  Der Präfix `E0h` (erweiterte Tasten) hat Bit 7 gesetzt und löscht damit `DS:3DA7` (Taste 60h); der folgende Code zählt wie
  bei der Grundtaste (Cursor hoch = `48h` wie Ziffernblock 8). Ein `cmp al, 7` nach dem Make-Zweig bleibt ohne Wirkung; der Block
  `27C6:008F` (Vektoren zurück, Textmodus, `int 21h/4Ch`) ist von dort nicht erreichbar (kein Sprung im Programm gefunden).
- **`037A` Zeitgeber an:** alten `int 8` (über DOS 35h) nach `CS:040E` und `CS:0412` merken, `int 8` auf `03E4` setzen, PIT
  Kanal 0 Modus 3 mit Teiler `4000h` (1 193 182 / 16 384 ≈ 72,82 Hz), `DS:117E` = `DS:1180` = 0.
- **`03E4` Zeitgeber-Tick:** zuerst Rückruf `call far [CS:0416]` (Vorgabe `27C6:041A` = `retf`), dann `DS:117E` += 1 und
  `DS:1180` += 1. Ist `DS:1180 & 3` ≠ 0: EOI, `iret`; sonst Sprung in den gemerkten Handler `[CS:0412]` (der das EOI gibt) – der
  alte Takt bleibt so bei 18,2 Hz. `117E` liest im Programm niemand außer dieser Routine (Codesuche über alle Segmente).
- **`03C3` Zeitgeber aus:** PIT-Teiler `FFFFh` (nicht `0` = 65 536; Abweichung ≈ 0,0015 % vom BIOS-Takt), alten Vektor zurück.
- **`0551` / `0576` DOS-Filter:** setzt bzw. entfernt `058B` als `int 21h`. `058B` fängt „Vektor 8 lesen“ (liefert `[CS:040E]`)
  und „Vektor 8 setzen“ (schreibt nur `[CS:0412]`, das Ziel der Weitergabe) ab; alles andere geht an das alte `int 21h`. Ein
  später geladener Treiber hängt sich damit hinter den Spielhandler statt ihn zu ersetzen.
- **`0000` / `0027`:** `int 9` auf `0043` setzen (alten Vektor in `DS:1159`) bzw. zurück. **`00AC`:** Tastentabelle `3D47…3DC6`
  löschen und den BIOS-Tastaturpuffer leeren (Kopf = Ende in `0040:001A/1C`). **`00D2`:** `DS:115D` = 0, warten bis ein Make kommt.
- **Joystick** (für `0E9E`, Befund [0E9E_000E_steuerung.md](0E9E_000E_steuerung.md)): `00E6(n, Schritt)` kalibriert über Port
  201h (Mitte, dann zwei Ecken; Schrittweiten `DS:1166…117C`, je mindestens 1), `0268(n, Ziel)` liest Achsen als vorzeichen-
  behaftete Stufen nach `Ziel[0..1]` und Knöpfe in AX (Bit 0 = Knopf 1, Bit 1 = Knopf 2; Joystick 1 Bits 10h/20h, Joystick 2
  40h/80h von Port 201h). Ohne Kalibrierung (Schrittweite 0) endet die Zählschleife in `0268` nicht.
- Übrige Einsprünge ohne Bezug zur Eingabe: `042E` (DAC-Register laden, aus `0D73`/`2295`), `048B`/`04CE` (Bytekopie über
  Segmentgrenzen rückwärts/vorwärts, `186C`), `0511` (AdLib-Register über Port `DS:2BE8`, `0D73`).

## Zustand

| DS-Offset | Breite | Bedeutung | gelesen/geschrieben |
|---|---|---|---|
| `3D47` | B[128] | Tastenzustand je Scancode (1 gedrückt, 0 los) | G `0043`, `00AC`; L u. a. `0E9E`, `21FD:07A5` |
| `1158` | B | letzter Make-Code | G `0043`; L/G `21FD:07A5` (Code-Abfrage) |
| `115D` | B | Make-Zähler (mod 256) | G `0043`, `00D2` |
| `1159` | Far | alter `int 9` | G `0000`, L `0027` |
| `117E` / `1180` | W | Tickzähler (72,82 Hz); `1180 & 3` steuert die Weitergabe | G `037A`, `03E4` |
| `115E…1165`, `1166…117C` | W | Joystick-Kalibrierung (Ecken, Mitte, Schrittweiten) | G `00E6`, L `0268` |
| `0648`, `065D` | W | eingeschaltete Handler (Maske), DOS-Filter aktiv | G `0D73:040E` |

## Seiteneffekte in Reihenfolge

Eingabe (Port 60h/61h, 201h), Zeitgeber (PIT, Vektoren 8/9/21h), PIC-EOI. Kein Zufall, keine Grafik. Der Rückruf je Tick ist der
einzige Weg vom Zeitgeber in den Ton (`21A7:000D`, Teil 3).

**Spieltakt:** Weder `117E` noch `1180` dienen dem Spiel als Uhr; gewartet wird auf den Strahlrücklauf (`0D73:00D3`: Port 3DAh
Bit 3 erst 0, dann 1 – ein Bild bei 70 Hz) und auf Make-Codes. Der Host braucht daher zwei Takte: Bildtakt (Rücklauf) und
PIT-Tick 72,82 Hz für den Tonrückruf.

## C#-Umsetzung

`src/Traders.App` (Host, R3): `Input/KeyStateTable.cs` (`0043` als `Receive`, Tabellenteil von `00AC` als `Clear`, Abbild in
ein DGROUP-Bytefeld mit `CopyTo`), `Timing/TimerTicks.cs` (`03E4`: Rückruf, beide Zähler, Weitergabe), `Timing/HostClock.cs`
(Teiler aus `037A`; Host-Zeit → Ticks und Bilder ohne Rundungsdrift; Bildtakt 70 Hz wie der RefEmu, `[APPROX]`).
Host-Ersatz ohne Original: `Input/HostKeyMap.cs` (Host-Taste → Set-1-Bytes mit `E0h`, US-Belegung nach Beschriftung statt
Position, `[APPROX]`), `Input/KeyboardEdges.cs` (Reihenfolge gleichzeitiger Änderungen, keine Tastenwiederholung, `[APPROX]`),
`HostGame.MaxStep` (Stillstand > 0,25 s wird nicht nachgeholt, `[APPROX]`). Nicht portiert: Vektor- und PIT-Verwaltung, DOS-Filter,
`00D2` (Warteschleife; im Host Sache der Phasenmaschine), Joystick.

## Prüfung

RefEmu-Proben `tests/Traders.Tests/App/OriginalInputProbeTests.cs` (nach dem Hochlauf ins Hauptmenü): Vektoren 8/9 zeigen auf
`03E4`/`0043`, PIT-Teiler `4000h`; Tickzähler +728 ± 1 in 10 emulierten Sekunden, BIOS-Uhr `0040:006C` ein Viertel davon;
Make/Break von Umschalt links (Tabelle, letzter Code, Zähler); `E0 48`/`E0 C8` (Tabelle 60h gelöscht, Cursor wie `48h`);
Überlauf des Make-Zählers ohne Nachbarbyte. Nicht geprüft: Joystick (RefEmu ohne Port 201h-Gerät), Rückruf je Tick (Teil 3).

Differenztests: `tests/Traders.Tests/App/HostInputDifferentialTests.cs` – 300 zufällige Bytes (Umschalt-, Sperr- und unbelegte
Tasten, `E0h`, Codes `00h`/`7Fh`/`FFh`) nach dem Hochlauf, nach jedem Byte Tabelle, letzter Make-Code und Zähler gleich; `00AC`
auf zufälliger Tabelle (Nachbarwörter unberührt); Host-Tasten → Tabelle → `0E9E:0035` im Original. `HostTimerTests.cs` – `037A`
auf dem Ladebild (Teiler, Zähler 0, Vektor 8); `03E4` mit Startwerten 0/0, FFF0h/FFFEh (Überlauf), 7/1, 8000h/3 über 1,5–5 s:
Zähler und Anzahl der Weitergaben (BIOS-Uhr) gleich, Tickzahl = Zeit · 72,82 Hz ± 1. Host-Uhr unabhängig von der Bildrate
(30–1000 Hz, unregelmäßige Schritte).
