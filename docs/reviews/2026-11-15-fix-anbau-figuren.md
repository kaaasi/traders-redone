# Review Fixlauf `fix/anbau-figuren` (PR #150, Erstreview) mit Plan-PR #148

- **PR:** #150 · **Base:** `e04742bca7899a8a69b46fa5a8a2ba0b147c357d` · **Head:** `fa98ba9de77d416796639e085e055267cf703d53`
- **Plan:** #148, Head `845043b` (nur `docs/arbeitspakete.md`, Abschnitt Fixlauf)
- **Reviewer:** Claude Opus (geplanter Lauf, eigene Sitzung; nicht an #150/#148 beteiligt)
- **Stand:** `git rev-parse HEAD` = Head, `git status --porcelain` leer; Originaldaten aus `kaaasi/traders-referenz` (`7fcfb24`)

## 1. Merge-Empfehlung

**Ja** für genau `fa98ba9`.

- #147 ist reproduziert und behoben: Ohne die beiden `Set32`-Zeilen wird der neue Host-Test genau im Anbaubild 136 rot
  (Meldung wie im PR), mit ihnen grün; der Erwartungswert kommt aus dem RefEmu (VRAM, DAC, CRTC-Start, `DS:10D2` je Rücklauf).
  `GraphicsLayers.Resolve` (`Objects.cs:222–232`) prüft erst den Atlas-, dann den Maskenbereich – bei `0000:0000` für beide
  landete jeder Maskenzeiger im Atlas; die neuen Host-Adressen (`8000:0000`, Atlas bis `94004`; `9800:0000`) trennen die
  Bereiche. `[APPROX]` an `SetUpGamePages` ist passend erweitert.
- #113: Grenze 17–18 belegt (neuer Test, Mutation rot), Jumps-Zeile für 18 ergänzt; die Nachholschleife in `HostGame.Update`
  ist durch einen `Frame`-Aufruf ersetzt, der gleich nachholt (s. u.).
- Kein Konflikt mit aktuellem `main` (`c9d7b99`, `git merge-tree` sauber; `main` hat seit Base #149/#154 aufgenommen →
  Merge-Prüfung §6 Nr. 2: `--schnell` auf dem Merge-Ergebnis wiederholen).
- Zwei P3 (Testlücken), keine P0–P2.

**Zu den Prüfpunkten:**

- **Lockstep Host ↔ Rücklauf ab dem Wochenbericht:** korrekt. Der Host steht vor dem Vergleich auf „Aufruf 3 wartet“, das
  Original auf seinem ersten Rücklauf in Aufruf 3; danach je Rücklauf genau ein `host.Frame(f+1)` mit der Tastentabelle am
  Rücklaufende. Der Test prüft je Bild `atRetrace == (Halt is null)` und den Zufallszustand, ab Aufruf 4 das Bild; 1445
  Rückläufe, 1233 Anbaubilder (1095 mit Objekten) bis Aufrufstelle 9 – eine Verschiebung um ein Bild würde im Anbau sofort rot.
- **`HostGame.Update`:** Alte Schleife `for f = LastFrame+1; f < Frames && !Ended; f++) Frame(f)` und neu
  `Frame(Frames − 1)` mit `while (LastFrame < frame && !Ended)` verarbeiten dieselben Bilder in derselben Reihenfolge mit
  denselben Tasten – gleichwertig. Ungetestet, siehe R-AF-2.
- **Bildvergleich im Bericht ausgelassen:** **ja, Testlücke** – die Begründung „nur verdeckte Seite“ stimmt nicht ganz:
  auch die **gezeigte** Seite und der DAC weichen ab (R-AF-1). Vorbestehend (gleich ohne den Fix), außerhalb des Auftrags #147.
- **Plan #148:** Fix passt zu Auftrag und Dateigrenzen. Geändert nur `src/Traders.App/**`, `tests/Traders.Tests/App/**` und
  eine von `tools/abdeckung.py --write` erzeugte Zeile in `STATUS.md` (gemeinsame Datei, im PR begründet – sonst wäre
  `pruefen.sh` rot); `Field/**` unberührt. Abnahme erfüllt: roter Test zuerst (`38f3580`, nur Testdatei), danach grün; alle
  drei Läufe grün; Gegenprobe rot (unten). Die Zuordnung von #113 aus Spur 0 an den Fixlauf ist umgesetzt. Statusabschnitt
  im Plan fehlt noch, weil #148 nicht auf `main` ist – Leitstelle beim Merge von #148.

## 2. Befunde

**R-AF-1 (P3, #157) – Wochenbericht im Host: Ausblenden einen Schritt früher, gezeigte Seite in Rückläufen 2–9 abweichend.**
[`HostPlantingFrameDifferentialTests.cs:110`](https://github.com/kaaasi/traders-redone/blob/fa98ba9de77d416796639e085e055267cf703d53/tests/Traders.Tests/App/HostPlantingFrameDifferentialTests.cs#L110)
lässt den Bildvergleich für Aufruf 3 ganz aus.
Beleg (Probe in temporärer Kopie): in Zeile 110 für Aufruf 3 zusätzlich CRTC-Start, die **gezeigte** Seite (16000 Byte je
Ebene ab `StartAddress`) und den DAC verglichen, Abweichungen nur protokolliert;
`TR_REQUIRE_GAMEDATA=1 dotnet test tests/Traders.Tests -c Release --filter "FullyQualifiedName~HostPlantingFrameDifferentialTests.Host_shows" --logger "console;verbosity=detailed"`:

```
week frame 2 (host frame 117, call 3): shown plane 0 off 0030 y 0 orig 0E host 00   (95 Byte verschieden)
week frame 2 (host frame 117, call 3): DAC 3 orig 3C host 34, orig max 3F host max 37, diffs 605
…
week frame 9 (host frame 124, call 3): DAC 3 orig 04 host 00, orig max 07 host max 00, diffs 152
PROBE report frames 212, shown-page diffs 8
```

Der Host blendet in den Rückläufen 2–9 jeweils einen Schritt (8 Stufen) früher aus; Rückläufe 2–6 zeigen bei y 0 ca. 95–99
Byte (Ebene 0) anders. CRTC-Start stets gleich, ab Rücklauf 10 alles gleich. Gleiches Ergebnis mit entfernten `Set32`-Zeilen
und alter `HostGame.cs` (Base-Verhalten) → nicht durch #150. Kurz und während des Ausblendens sichtbar; für #150 P3, eigene
Priorität entscheidet die Leitstelle. Abnahme eines Fixes: Bericht-Rückläufe im Host-Test mindestens an gezeigter Seite und
DAC vergleichen.

**R-AF-2 (P3, #158) – `HostGame.Update` ungetestet.**
[`HostGame.cs:99`](https://github.com/kaaasi/traders-redone/blob/fa98ba9de77d416796639e085e055267cf703d53/src/Traders.App/HostGame.cs#L99):
Mutation `_clock.Frames - 1` → `_clock.Frames` (ein Bild zu viel je Update) →
`dotnet test … --filter "FullyQualifiedName~App"`: 116 bestanden, 0 rot. Die Umstellung selbst ist gleichwertig (s. o.); die
Lücke betrifft die Abbildung Uhr → Zielbild. Vorschlag in #158.

Nebenbei (kein Befund): Der neue Host-Test läuft hier ca. 5 s, nicht ca. 40 s wie im PR genannt.

## 3. Ausgeführte Prüfungen

`tools/pruefen.sh` (alle drei Läufe) auf `fa98ba9`, sauberer Arbeitsbaum:

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| Mit Originaldaten (TR_REQUIRE_GAMEDATA=1) | 886 | 0 | 0 |
| Ohne Originaldaten (TR_SKIP_GAMEDATA=1) | 456 | 0 | 248 |
| Pflichtlauf ohne Daten (muss scheitern) | 456 | 430 | 0 |

Ergebnis: grün (Hashes, Architektur, Abdeckung, Restore `--locked-mode`, Build mit Warnungen als Fehler).

Gegenbeispiele und Mutationen (temporärer Worktree, jeweils `TR_REQUIRE_GAMEDATA=1 dotnet test … -c Release --filter …`):

| Mutation | Filter | Ergebnis |
|---|---|---|
| #147: beide `Set32(AtlasPointer/MaskPointer, …)` in `ProgramHost.cs` entfernt | `~HostPlantingFrameDifferentialTests` | **rot** 2/2: `week frame 348 (host frame 463, call 8, planting frame 136): plane 0 offset 55EC … original 05 host 07`; Schnelltest `Assert.Equal` |
| `MaskBuffer` = `9000:0000` (überlappt den Atlas) | dto. | **rot** 2/2: Anbaubild 136; `atlas 80000-94004 overlaps masks 90000-91F40` |
| #113: `MaxStep` 0,25 s → 0,26 s | `~ProgramHostCatchUpTests` | **rot**: `One_update_of_the_longest_step_covers_17_or_18_frames` erwartet `[17, 18]`, ist `[18, 19]` |
| `HostGame.cs:99` `Frames - 1` → `Frames` | `~App` | grün 116/116 → R-AF-2 |
| Probe Bericht (s. R-AF-1), Head und Base-Verhalten | `…Host_shows` | grün, protokolliert 8 abweichende Rückläufe |

Codelesen: `Objects.cs` `Resolve`/`00CA`-Zeigerbildung, `ProgramHost.Frame`, `SetUpGamePages`, Datenlayout `DS:45D6`/`4BC6`
(`DATENLAYOUT.md`, `21AF_000D_objekte.md`); der Spielstandpuffer `DS:45D6` im Kern-DGROUP ist vom Grafik-DS des Hosts getrennt
und nicht berührt. Keine portierte Originalroutine im Diff → keine `T.EXE`-Fundstelle zu prüfen.

**Akzeptierte `[APPROX]`:** `ProgramHost.SetUpGamePages` – Atlas- und Maskenadresse sind Host-Wahl; für `21AF` zählen nur
getrennte Bereiche (belegt durch den Differenztest und den Schnelltest).

**Annahmen (nicht belegt):** R-AF-1 könnte am Zeitpunkt des ersten Ausblendschritts in der Berichtsszene liegen; nicht
untersucht. CI auf dem Head nicht geprüft (Sache der Merge-Prüfung).
