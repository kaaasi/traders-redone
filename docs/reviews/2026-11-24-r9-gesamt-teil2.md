# Gesamtreview Runde 9 – Teil 2

Reviewer: Codex, unabhängige Sitzung · 24.11.2026 · Fortsetzung von [Teil 1 (PR #273)](https://github.com/kaaasi/traders-redone/pull/273/files).
Base `64f3481` · geprüfter Code `c8e35de23aa6ccdd648ad5a2731a44d8cbbfb76d` · geprüfter Checkout `10ca151d224678400c8feae846043acac885b7eb` (`origin/main`), vor dem Lauf sauber.
`git diff --stat c8e35de origin/main`: nur `CHANGELOG.md`, `STATUS.md`, `docs/PORTIERUNGSPLAN.md`, `docs/arbeitspakete.md` und `docs/reviews/2026-11-24-r9-status-2.md` geändert; kein Code- oder Testdelta. `STATUS.md` ist eine Eingabe der Abdeckungsprüfung und wurde mitgeprüft.

## 1. Merge-Empfehlung

**Ja für Runde 9 auf dem geprüften Code-Stand `c8e35de`**: vollständiger Schnelllauf auf dem dokumentationsaktualisierten `main` grün, die drei Gesamtpartieproben einschließlich F10 bestanden; keine neuen belegten Fehler. Die unten genannten Testlücken und erwarteten Abweichungen bleiben offen. Dies ist keine Abnahme der gesamten Originaltreue oder des Hörvergleichs.

## 2. Befunde und Testlücken

**Nachgewiesene neue Fehler:** keine in der risikoorientierten Stichprobe. Die Gegenproben R9M-1, R9K-1/M7 und #264 wurden laut Teil 1 bereits rot ausgeführt; hier nicht wiederholt.

**Testlücke – unbeeinflusstes Zielende:** Die [Gesamtpartie](https://github.com/kaaasi/traders-redone/blob/c8e35de23aa6ccdd648ad5a2731a44d8cbbfb76d/tests/Traders.Tests/App/HostWholeGameFrameDifferentialTests.cs#L207) überspringt Aufruf 18 auf beiden Seiten und setzt [das Ziel vor Woche 2 auf null](https://github.com/kaaasi/traders-redone/blob/c8e35de23aa6ccdd648ad5a2731a44d8cbbfb76d/tests/Traders.Tests/App/HostWholeGameFrameDifferentialTests.cs#L268). Es fehlt ein begrenzter, reproduzierbarer Host/RefEmu-Partietest mit regulär gewähltem, während der Partie unverändertem Ziel: tatsächliches Erreichen durch erwirtschaftetes Vermögen, Endprüfung, Phasenwechsel und Endbilder samt RNG, Zustand, Bild und Ton vergleichen; Eingaben und Seed dokumentieren. Die Standardabweichung Roboter überspringen dabei ausdrücklich ausweisen; eine gesonderte Partie mit echter Roboterphase bleibt für M12 nötig. Kein Fehlernachweis.

**Testlücke – „fortsetzen“ nach Spielende:** [F3-B-Rückkehr](https://github.com/kaaasi/traders-redone/blob/c8e35de23aa6ccdd648ad5a2731a44d8cbbfb76d/src/Traders.App/Deviations/MenuAfterGameOver.cs#L16) erhält den Zustand und `DS:08FA`; das Menü bietet A/O/V zum Fortsetzen, die Programmmachine überspringt bei Auswahl 2 die Neupartie und startet die Wochenschleife. Der [Hosttest](https://github.com/kaaasi/traders-redone/blob/c8e35de23aa6ccdd648ad5a2731a44d8cbbfb76d/tests/Traders.Tests/App/ProgramHostTests.cs#L189) prüft jedoch nach Rückkehr nur **N** für Neupartie; die separate Ladeprobe prüft Fortsetzen eines geladenen Spielstands, nicht diesen Übergang. Es fehlt eine Hostprobe Ende → Menü → A/O/V → nächste Woche mit Prüfung von Wochenzähler, Vermögen, Vorräten, RNG und Tonzustand. Für die bewusste Rückkehr nach Spielende gibt es keinen entsprechenden Originalablauf. Kein Fehlernachweis.

**Erwartet offen / akzeptiert:** #262 (Tastenklick-Buchung `[APPROX]`), `WeekScreens.MirrorFont`, `MenuAfterGameOver` (F3 = B, `--original-ende`, „fortsetzen“ bleibt), FXDRIVER-Kartentest ohne Statuslesen, #75 B1, #249 und Hörvergleich M14 durch den Nutzer; keine erneute Einstufung als neuer Fehler. F1 = B: C#-Port von ymfm, BSD-3-Clause samt Lizenz in `THIRD_PARTY.md`, kein NuGet-Paket. F3 = B ist abschaltbar; Originalpfad und Rückkehr sind getestet.

## 3. Ausgeführte Prüfungen und Grenzen

- `tools/pruefen.sh --schnell`, Originaldaten vorhanden und Hashprüfung bestanden; Architektur, Abdeckung, Restore mit Lockdateien und Release-Build mit Warnungen als Fehler bestanden. Vollständig bis Exit 0 ausgeführt, .NET SDK 10.0.402:

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| Mit Originaldaten (TR_REQUIRE_GAMEDATA=1) | 1395 | 0 | 0 |

- [Gesamtpartietests](https://github.com/kaaasi/traders-redone/blob/c8e35de23aa6ccdd648ad5a2731a44d8cbbfb76d/tests/Traders.Tests/App/HostWholeGameFrameDifferentialTests.cs#L80): Ziel Woche 2 mit vier Computern und mit Mensch sowie F10 Woche 0 grün. F10: kein Spieler am Ziel, Drücken und Loslassen, Bild/Zustand/RNG und Tonbefehle bis zum Ende, danach Hauptmenü erreicht. Die Host-Rückkehr ist keine Originalausführung; im F10-Zweig wird am Teardown kein DS-/Bildvergleich behauptet. Endbild-F10-Gleichschritt der zweiten Partie ebenfalls grün.
- Phasenwechsel: Endbilder, Tastensperre, Wochenzähler und Teardown werden in den obigen Proben erfasst; die Zielpartien umfassen Wochen 0–2, separate Vierwochenproben sind grün. Fortsetzen und natürliches Zielende siehe Testlücken.
- Tonmerker nach #270: [Ausschlussliste](https://github.com/kaaasi/traders-redone/blob/c8e35de23aa6ccdd648ad5a2731a44d8cbbfb76d/tests/Traders.Tests/App/ProgramHostTests.cs#L411) filtert `DS:10A5`/`10A9` aus beiden betreffenden Listen; Gesamtpartien vergleichen die verbleibenden DS-Bereiche und Tonbefehle im Gleichschritt. Der Aufbau schließt zusätzlich zur Ausschlussliste die zur Laufzeit als Loader-Differenzen ermittelten Bytes aus. Kein Hörtest; #262 bleibt eine bekannte Grenze.
- Annahme: Das reine Dokumentationsdelta verändert das ausführbare Verhalten nicht. Kein Anspruch auf Vollinventur; frühere Einzelbefunde und Gegenproben stehen in Teil 1.
