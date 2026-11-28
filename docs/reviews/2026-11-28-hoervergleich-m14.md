# Hörvergleich Ton (M14) – 28.11.2026

Durchgeführt vom Nutzer mit einer Claude-Sitzung nach `docs/vorlagen/hoervergleich.md`. Der Chat hat nichts portiert und keinen
Code geändert. Gehört hat nur der Nutzer; die Zahlen unten stammen aus einer rechnerischen Auswertung beider Aufnahmen.

## 1. Kopf

- **Code-Stand:** `main` = `08c086a` (davor `813e83e`; dazwischen nur Prozess-Doku, kein Code unter `src/`/`tests/`).
- **Vergleichsquelle:** DOSBox 0.74-3 mit AdLib/OPL2 (`sbtype=sb16`, `oplmode=opl2`, `oplemu=default`, `oplrate=44100`,
  Mixer 44100, `blocksize=1024`, `prebuffer=25`), `TRADERS.BAT` mit den Dateien aus `original/dos/`, Ton im Menü an.
- **Aufnahme Original:** WAV-Mitschnitt von DOSBox (`Strg+Alt+F6`), 109,3 s, 44,1 kHz, Stereo. Abgelegt im Referenz-Repo,
  [PR #2](https://github.com/kaaasi/traders-referenz/pull/2), `audio/dosbox-0.74-3/2026-11-28-programmstart-opl2.wav`.
- **Port-Start:** `dotnet run --project src/Traders.App -c Release -- <Ordner mit den Originaldateien>`, Windows 11 26H2,
  Standard-Tongerät. Aufnahme des Nutzers: 107,3 s, 48 kHz, mono (nicht im Repo, `AGENTS.md` §2.1). Der Nutzer hat im Port nichts
  bedient und blieb im Hauptmenü.

## 2. Gesamturteil

**M14 erfüllt: nein.** Nur H1 wurde geprüft (deutlich anders). Auf Wunsch des Nutzers wurde die Tonausgabe danach zurückgestellt
(bis kurz vor V1.0); H2 bis H7 sind **nicht geprüft**.

## 3. Hörstellen

| Nr. | Urteil | Beschreibung des Nutzers | Zeitpunkt / Beleg |
|---|---|---|---|
| H1 | deutlich anders | Original: Musik beginnt sofort, zunächst ohne Drums, die später einsetzen und Tempo geben. Port: klingt, als spielten mehrere Stücke nacheinander, keins wie im Original; es fehlen vor allem die Drums. Gewählt: andere Melodie, Schlagzeug, fehlende/hängende Töne. | Original: Ton ab Enter + ca. 2 s. Port: Biep ca. 6 s nach Menübild, „Titelmusik“ ca. 11 s danach. |
| H2 | nicht geprüft | „Das ist ein Loop im Original“, Wiederholungszeit nicht bestimmt. | – |
| H3–H7 | nicht geprüft | – | – |

**Vermutung des Chats (rechnerische Auswertung, kein Hörurteil):**

- Original: Ton ab 3,9 s nach Aufnahmestart, danach lückenlos bis 109 s; Materialwechsel bei ca. 27–36 s, danach stabil.
- Port: kurzer Ton 6,5–7,0 s, Stille bis 13,5 s; Musik in vier Blöcken (13,5–39 s, 42–56 s, 63,8–77,5 s, 80,2–107 s) mit Pausen
  von etwa 3, 8 und 3 s. Die Tonhöhenprofile der Blöcke unterscheiden sich untereinander und vom Original.
- Beide haben ein Raster von etwa 0,22 s. Im Original klar (Korrelation 0,86) mit tiefem Schlag alle 0,44 s (0,67); im Port
  schwach (0,3–0,5), der 0,44-s-Schlag fehlt im Tiefbereich bei Block 1.
- Tiefbereich relativ zu 300–1000 Hz: unter 100 Hz Original +0,5 dB, Port −44 bis −69 dB; 100–300 Hz Original +4 dB, Port
  +0,3 bis −31 dB. Über 4 kHz in beiden ähnlich (−6 bis −9 dB).

## 4. Abweichungen

- **HV-1 (H1):** Stückfolge und Melodie. Statt Vorspann und anschließender Menümusik hört der Nutzer mehrere Stücke, keins wie das
  Original; zuerst ein kurzer Ton, dann ca. 6 s Stille. Vermutung: Stückwahl der Effektmaschine/des Treibers beim Start
  (`0DFD:000D`, `218B:0094`), nicht belegt.
- **HV-2 (H1):** Schlagzeug/Tiefbereich fehlt. Messwerte oben. Vermutung: Schlagzeugkanal des Treibers oder Synthese/Mischung,
  nicht belegt; ein Aufnahmeweg des Nutzers als Ursache ist nicht ausgeschlossen.
- **HV-3 (H1):** Pausen und Stille zwischen den Musikblöcken des Ports ohne Eingabe. Vermutung: Treiber oder Host-Takt, nicht belegt.

Hinweis: Pegelunterschiede und Übersteuerung der DOSBox-Aufnahme (Clipping bei 18,4 s) gelten nicht als Abweichung.
