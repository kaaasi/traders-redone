# Arbeitspakete

Die Portierung läuft in **Runden**; je Runde arbeiten mehrere **Spuren** parallel, jede auf einem Branch `paket/<spur>` mit
einem PR, eigenen Dateien und klarer Abnahme. Regeln: [AGENTS.md](../AGENTS.md), Ablauf:
[prozess/zusammenarbeit.md](prozess/zusammenarbeit.md). Den Status pflegt jede Spur nur in ihrem eigenen Abschnitt; Plan-
abschnitte ändert die Leitstelle. Abgeschlossene Runden stehen in [archiv/](archiv/) (Paketphase AP1–AP5: [archiv/runde-0.md](archiv/runde-0.md), Runde 1: [archiv/runde-1.md](archiv/runde-1.md), Runde 2: [archiv/runde-2.md](archiv/runde-2.md), Runde 3: [archiv/runde-3.md](archiv/runde-3.md), Runde 4: [archiv/runde-4.md](archiv/runde-4.md), Runde 5: [archiv/runde-5.md](archiv/runde-5.md), Runde 6: [archiv/runde-6.md](archiv/runde-6.md)).

## Aktuelle Runde: – (Runde 6 abgeschlossen am 16.11.2026: [archiv/runde-6.md](archiv/runde-6.md))

Die nächste Leitstelle plant Runde 7 in einem neuen Chat (`vorlagen/leitstelle-start.md`, Auftrag „Runde 7 planen“).

## Vorschau: Runde 7 (Plan folgt, Go nötig)

Nach dem Fahrplan (Zeile R8, da R7 `1B03` in R6 vorgezogen wurde): Handel `0B9F:0002` (Aufruf 23) · Handels-KI `0C29`/`141D` ·
Zusatzphase `1DEF:000C` (Aufruf 24). Der Host hält seit R6 an Aufruf 18 (Roboterphase `1C68`, Fahrplan R11–R12); davor liegt
Aufruf 19 (Endprüfung `20B3`, R10). **Wunsch des Nutzers (16.11.2026):** Im Remake den Roboterkampf vorerst überspringen, bis er
portiert ist – bewusste, abschaltbare Abweichung vom Original, damit spätere Phasen vorher spielbar und testbar sind. Für den Plan
R7: Schalter im Host (nicht im Kern-Ablauf der Originaltreue), Aufruf 18 als gekennzeichneter Leerlauf; Differenztests und
Mehrrundenprobe laufen weiter mit Aufruf 18 aus dem Orakel (sonst weichen Zustand und Zufall ab). **Entscheidung des Nutzers
(16.11.2026):** Endprüfung `20B3:0005` (Aufruf 19) originalgetreu in R7 vorziehen, damit der Host danach bis zum Handel spielt. Pflichten aus R6:
Testlücke Host-Seitenzustand (#180), `HostGame.Update` (#158), Wochenbericht im Host (#157). Offen: P3 #49, #75, #76, #162,
#163, #165, #166.

## Fahrplan

Nach Runde 2, je Runde 2–4 Spuren; Vorschlag nach BESTAND §3.2 (Leitstelle 11.11.2026), jede Runde mit eigenem Plan und Go:

| Runde | Themen | Segmente |
|---|---|---|
| R3 | Phasenmaschine mit Takt und Zufallsereignissen (F3 = A) · Wochenregeln ohne Bildschirm · Grafikschichten · App-Host mit Eingabe · Ton-Analyse | `1777`, Teile von `0825`/`0609`/`22A3`/`08CF`/`0B9F`; `0A96`, `0A69`, `0B67`, `21AF`, `21FD`-Paletten; `0E9E`/`27C6` |
| R4 | Menüs · neue Partie · Start/Code-Abfrage · Spielfeldschicht | `192D`, `0E31`, `0DFD`, `21FD:0687`, Teile von `0609` |
| R5 | Anbau · Anbau-KI und Wochenplanung · Wochenbilanz | `0609`, `135D`, `11A3`, `0825` |
| R6 | Produktion/Ernte/Wetter · Katastrophen · Wochenereignisse | `22A3`, `111D`, `17CB` |
| R7 | Stationen/Rakete/Piraten – in R6 vorgezogen (F1 = B); die folgenden Runden rücken bei ihrer Planung je eine vor | (`1B03`) |
| R8 | Handel · Handels-KI · Zusatzphase | `0B9F`, `0C29`, `141D`, `1DEF` |
| R9 | Fat Mike und Wettbewerb · Einkauf | `1F63`, `08CF` |
| R10 | Ende/Abend · Ton · **Gesamtpartie ohne Roboterkampf** (Zwischenziel) | `20B3`, `218B`, `21A7` |
| R11–R12 | Roboterkauf, Aufstellung, Kampffeld, Kampf, je mit KI | `1C68`, `16D6`, `0535`, `12C8`, `1E7E`, `0EDE`, `150F` |
| R13 | Gesamtpartie v1.0 | – |

Danach Erweiterungen ([PORTIERUNGSPLAN.md](PORTIERUNGSPLAN.md)).
