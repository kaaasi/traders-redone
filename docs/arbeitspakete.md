# Arbeitspakete

Die Portierung läuft in **Runden**; je Runde arbeiten mehrere **Spuren** parallel, jede auf einem Branch `paket/<spur>` mit
einem PR, eigenen Dateien und klarer Abnahme. Regeln: [AGENTS.md](../AGENTS.md), Ablauf:
[prozess/zusammenarbeit.md](prozess/zusammenarbeit.md). Den Status pflegt jede Spur nur in ihrem eigenen Abschnitt; Plan-
abschnitte ändert die Leitstelle. Abgeschlossene Runden stehen in [archiv/](archiv/) (Paketphase AP1–AP5: [archiv/runde-0.md](archiv/runde-0.md), Runde 1: [archiv/runde-1.md](archiv/runde-1.md), Runde 2: [archiv/runde-2.md](archiv/runde-2.md), Runde 3: [archiv/runde-3.md](archiv/runde-3.md), Runde 4: [archiv/runde-4.md](archiv/runde-4.md), Runde 5: [archiv/runde-5.md](archiv/runde-5.md), Runde 6: [archiv/runde-6.md](archiv/runde-6.md), Runde 7: [archiv/runde-7.md](archiv/runde-7.md)).

## Aktuelle Runde: – (Runde 7 abgeschlossen am 17.11.2026: [archiv/runde-7.md](archiv/runde-7.md))

Die nächste Leitstelle plant Runde 8 in einem neuen Chat (`vorlagen/leitstelle-start.md`, Auftrag „Runde 8 planen“).

## Vorschau: Runde 8 (Plan folgt, Go nötig)

Nach dem Fahrplan (Zeile R8): Fat Mike `1F63:000F` (Aufruf 26) und Wettbewerb/Auswertung `1F63:0B03` (30) · Einkauf `08CF:0004`
(29; Edelsteinkurs `08CF:11D7` liegt seit R3 vor, Countdown `21FD:04E5`/`04F9` seit R7 in `Rules/Countdown`). Danach spielt der
Host die ganze Woche (Roboterphase übersprungen) und beginnt die nächste. Pflichten aus R7: P3 #193, #205, #208 (Sammellauf,
Sonnet). Offen: P3 #49, #75, #76. Modell je Spur nach `prozess/zusammenarbeit.md` §5.

## Fahrplan

Nach Runde 2, je Runde 2–4 Spuren; Vorschlag nach BESTAND §3.2 (Leitstelle 11.11.2026), jede Runde mit eigenem Plan und Go:

| Runde | Themen | Segmente |
|---|---|---|
| R3 | Phasenmaschine mit Takt und Zufallsereignissen (F3 = A) · Wochenregeln ohne Bildschirm · Grafikschichten · App-Host mit Eingabe · Ton-Analyse | `1777`, Teile von `0825`/`0609`/`22A3`/`08CF`/`0B9F`; `0A96`, `0A69`, `0B67`, `21AF`, `21FD`-Paletten; `0E9E`/`27C6` |
| R4 | Menüs · neue Partie · Start/Code-Abfrage · Spielfeldschicht | `192D`, `0E31`, `0DFD`, `21FD:0687`, Teile von `0609` |
| R5 | Anbau · Anbau-KI und Wochenplanung · Wochenbilanz | `0609`, `135D`, `11A3`, `0825` |
| R6 | Produktion/Ernte/Wetter · Katastrophen · Wochenereignisse | `22A3`, `111D`, `17CB` |
| R7 | Stationen/Rakete/Piraten – in R6 vorgezogen (F1 = B); dafür jetzt Endprüfung (Aufruf 19) · Handel · Handels-KI · Zusatzphase; Roboterphase im Host übersprungen (Abweichung, Nutzer 16.11.2026) | `20B3:0005`, `0B9F`, `0C29`, `141D`, `1DEF` |
| R8 | Fat Mike und Wettbewerb · Einkauf | `1F63`, `08CF` |
| R9 | Ende/Abend (Rest `20B3`) · Ton · **Gesamtpartie ohne Roboterkampf** (Zwischenziel) | `20B3`, `218B`, `21A7` |
| R10–R11 | Roboterkauf, Aufstellung, Kampffeld, Kampf, je mit KI | `1C68`, `16D6`, `0535`, `12C8`, `1E7E`, `0EDE`, `150F` |
| R12 | Gesamtpartie v1.0 | – |

Danach Erweiterungen ([PORTIERUNGSPLAN.md](PORTIERUNGSPLAN.md)).
