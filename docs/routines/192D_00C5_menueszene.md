# 192D:00C5 / 192D:03AB / 192D:042F / 192D:0653 / 192D:076C / 192D:07AC / 192D:07FC / 192D:084A / 192D:0899 / 192D:08E7 / 192D:0935 / 192D:09B0 / 192D:0A26 / 192D:0DC0 / 192D:0E11 / 192D:0E64 / 192D:0E8E / 192D:0EAE / 192D:0ED9 / 192D:0EF8 / 192D:0F16 / 192D:0F6E / 192D:0FB7 / 192D:120C / 192D:16AF / 192D:17F9 / 192D:1855 / 192D:19A4 / 192D:1B06 / 192D:1B90 / 21FD:05DB – Zeichenseite der Menüs (Menüszene, Spur `menue`, R4)

- **Ladebild-Adresse:** Segment `192D` (Dateioffset `0x1C8D0`); `21FD:05DB` bei Dateioffset `0x2C99B`.
- **Aufgabe:** alles, was die Menüs aus [192D_0007_menues.md](192D_0007_menues.md) zeichnen: Bildaufbau je Menü, Teilzeichnungen
  nach Tasten, Texte und Zahlen, Schriftwahl. Logik und Zufall bleiben im Kern, die Szene liest nur.
- **Aufgerufen von / Phase:** aus den Menüroutinen von `192D` (Hauptmenü `0007`), vor der ersten Woche.
- **Parameter / Rückgabe:** je Routine unten / keine.

## Ablauf (eigene Worte)
- **Gemeinsame Bausteine:** `17F9(x, y, Text, n)`: Tafelfarbe 2 über n·6 Pixel bei (x + 45h, `5334` + y − 4), dann Text bei
  (x + 46h, `5334` + y), Farbe 1, Schatten 4 bei (−1, +1) über `0A69:004D`. `1855(x, y, Zahl, n)` ebenso mit n-stelliger Zahl
  (`0A69:011B`). `09B0(sx, sy, w, h, dx, dy, an)`: an → Ausschnitt des Bilds nach (dx + 45h, `5334` + dy + 61h), aus → Tafelfarbe.
  `21FD:05DB(f)`: Treiberschrift f (`23CD:05B6`), `3CA8` = B[`10DA`+f], `34A4` = B[`10DD`+f], `0A96:000A(3CA8, 34A4)`.
- **`16AF` nach dem Ausblenden:** Liste leeren, Mauer und Tafel (Zeichenaufrufe aus dem Kern), ausführen; Schrift 1, Titel
  (Eintrag 0) zentriert, Farbe 1, Schatten 4 bei (−4, +4), ausführen; Schrift 0, ab `5334` + Abstand + 14h die Zeilen 1… bis `@`
  (Zeichen `'` ohne Bit 7 = 3 Pixel Lücke, sonst Text bei x = 46h, 7 Pixel Abstand), nach dem Zusatzabstand die Zeile hinter `@`
  zentriert; ausführen.
- **Bildaufbau** (je nach `16AF` Liste leeren, Inhalt, ausführen, `0A96:0C3F([082E])`): `00C5` Titelstreifen (Bild 47h/28h,
  ABh × 1Ch), vier Zeilen aus `DS:0B70` (je zwei Texte, Farben 0 und Eh), vier Spielerzeilen `03AB`, Fuß `3C80`, dann `21FD:026C`;
  `042F` Tastenhilfe `0653` (nur Stufe −1), Zeilen `076C…0935`; `0A26` Kopf `3C78`, `0DC0…0F16`, `0F6E`; `0FB7` Mauer, zwei Tafeln,
  Titel `3A74` in Schrift 1, Zeilen `3A78`, `3A7C`, Fuß `3A84`; `1263`/`12A6` nur `16AF` und `0C3F`; `19A4` vier Zeilen `1B06`,
  `026C`; `1B90` je Joystick: Name (Text 125h/126h, x = 5Ah − strlen·3), `3C74`, Aufforderung `3C68`, `026C`; vor Schritt 1 und 2
  jeweils Liste leeren, Aufforderung `3C6C` bzw. `3C70`, ausführen, `0C3F` (`1CAB`, `1D00`).
- **Teilzeichnungen:** `03AB(p)` Text `3BEC`+4·B[`0840`+p] bei (78h, 78h + 7p); `0653` sechs Zeilen `0BB0` bei (46h, 4Ch + 7i)
  ohne `5334`; `05DD` bei Stufe 0 Tafelfarbe (45h, 47h, 18h × 2Ah); `076C` Stufe −1 Text `DS:0C74`, sonst Zahl Stufe + 1;
  `07AC` IQ-Text `39F0` und Bild (IQ·15h, 6Dh); `07FC/084A/0899/08E7` Text `398C`+4·Wert und `09B0`; `0935` Piratentext `3A00`
  und Bild (Wert·1Ah + 1Dh, 51h) bzw. Tafelfarbe; `0DC0`/`0E11` Betrag mit `$` (`0A69:0164`, 5 Stellen) über Tafelfarbe;
  `0E64` (`8F9` vorzeichenbehaftet), `0EAE`, `0F6E` (`10A7` als Wort) Texte; `0E8E`, `0ED9`, `0EF8`, `0F16` Zahlen; `1B06(p)`
  Computer `3C3C`, Gerät 0…4 `3C40`+4·Gerät, Gerät 5 `3C7C`; `120C(Text)` Meldung der Diskette bei 2·`5334` + 6Ah.
- **Schleifen:** Am Schleifenanfang Liste leeren; je Durchlauf `21FD:051F`, ausführen, leeren, Tasten (Teilzeichnungen nur
  eingereiht), am Ende ausführen. So erscheint eine Teilzeichnung auf der Zeichenseite sofort und nach dem nächsten Seitenwechsel
  noch einmal auf der anderen Seite.

## Zustand
| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|
| `5334` | W | s | Tafeloberkante | g |
| Optionen (`08E4…0902`, `10A7`, `2E8E`, `2F30`, `411A`, `45BA`, `4BF2`, `0840`, `083C`, `5330/5332`) | – | wie DATENLAYOUT | Anzeige | g |
| `37CC…3C8F`, `0B70`, `0BB0`, `0C74` | far/Text | – | Texte | g |
| `3CA8`, `34A4`, `0142/0143` | W/B | – | Schriftmaße (Grafikseite) | s |
| `082E`, Befehlsliste, Paletten | – | – | Grafikzustand (Schichten R3) | g/s |

## Seiteneffekte in Reihenfolge
Nur Grafik: Befehlsliste `0A96`/`0A69`, Seiten `0B67`/`21FD:051F`, Blenden `21FD:03B4/0353/0329`, `026C`. Kein Zufall, keine
Änderung am Spielzustand.

## C#-Umsetzung
`src/Traders.Presentation/Menus/MenuScene.cs`: spielt die `MenuEvent`s des Kerns (`LoadPicture`, `FadeOut`, `Screen`, `Draw`,
`DiskMessage`, `FadeIn`, `Flip`, `ListExecute`, `ListClear`) über `GraphicsLayers` ab; liest Optionen, Textzeiger und `5334` aus
dem Kern-DS, schreibt nur in das DS-Abbild der Grafikschichten. Die Tastentabelle wird vor jedem Seitenwechsel und in jedem
Bild eines laufenden Seitenwechsels in dieses Abbild gespiegelt (`3D91`/`3D95` steuern das Warten in `051F`).
**Bildweise:** Blenden und Seitenwechsel laufen über die schrittweisen Formen der Schichten (`GraphicsLayers.FadeOutFastSteps`,
`FadeInSteps`, `FadeInFastSteps`, `FlipPagesSteps` in `src/Traders.Vga/Layers/FrameSteps.cs`, je Rücklauf ein Schritt); der Host
ruft je Bild `NextFrame()` vor den Ereignissen des Bilds, die Szene führt die laufende Routine bis zum nächsten Rücklauf weiter.
So liegt jeder Rücklauf im Bild, in dem der Kern ihn wartet, und das sichtbare Bild (DAC, Seite) entspricht je Bild dem Original.

## [APPROX]
- `186C:0002` (Spur `programm`): die Szene lädt das Menübild aus den Assets mit der Wirkung des VGA-Pfads (`09C8`, Palette nach
  `5017`, Atlas ab Byte 300h, `23CD:3079`, `21AF:000D`); Zwischenspeicher und Laderzustand `09CE…`, `4C00…` fehlen.

## Prüfung
`tests/Traders.Tests/Presentation/MenuSceneDifferentialTests.cs` (`[DIFFERENTIAL]` je Routine, Harness `MenuSceneHarness`):
Original bis zum Eintritt `192D:0007` gestartet (Schnappschuss), Grafik ungestubbt; Port mit `MenuRunner` und `MenuScene` auf dem
Grafikzustand des Schnappschusses. Vor jedem `21FD:051F`, `03B4`, `0353`, `0329` und am Ende: alle vier VRAM-Ebenen, DAC,
CRTC-Start und Zahl der Rücklaufwartungen gleich. Szenarien: Hauptmenü, Spieler, alle Stufen und Einzeloptionen, alle
Einstellungen, Diskette (Laden/Speichern mit Erfolg und Fehlschlag), Info, Steuerung mit zwei Kalibrierläufen, Fortsetzen,
Keypad-Minus. Gegenproben lokal (je eine Änderung → rot): Schattenfarbe in `17F9`, x von `0F6E`, Lücke für `'` in `16AF`,
y in `120C`, Aufforderungen in `1B90` vertauscht, Tastenspiegel vor dem Seitenwechsel entfernt (Rücklaufzahl). Bildgrenzen des
Originals dort: Ende jedes `0D73:00D3` und jede genommene Rückkante einer Tastenwarteschleife. Bild für Bild mit echten
Tasteninterrupts: `MenuFrameDifferentialTests` (siehe [192D_0007_menues.md](192D_0007_menues.md) „Prüfung“). Schrittformen der
Schichten: `tests/Traders.Tests/Vga/FrameStepsTests.cs` (je Rücklauf DS, DAC, CRTC-Start gleich dem Original bei `21FD:03B4`,
`0353`, `0329`, `051F`; ausgeschöpft gleich den Routinen am Stück).
