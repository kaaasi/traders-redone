# Vorlage: Hörvergleich Ton (M14)

**Kurzaufruf** (der Nutzer schreibt ihn in einen neuen Chat; der Chat führt ihn durch den Vergleich):

> Repo kaaasi/traders-redone. Führe mich durch den Hörvergleich nach `docs/vorlagen/hoervergleich.md`.
> Code-Stand: `<sha | main>` · Vergleichsquelle: `<DOSBox mit AdLib | Video: Link/Datei>` · binde traders-referenz mit push ein.

## Zweck

Die Tests belegen, dass der Port dieselben OPL2-Registerschreibungen wie das Original erzeugt (Effekte, `FXDRIVER`-Musik) und
dass die Synthese bitgleich zu ymfm rechnet (F1 = B). Ob das Ergebnis **klingt** wie das Original, prüft nur der Nutzer
(`PORTIERUNGSPLAN.md` M14, „Hörvergleich mit Video“). Der Chat portiert nichts und ändert keinen Code.

## Ablauf für den Chat

1. **Zugriff und Stand:** `git ls-remote` beider Repos; Code-Stand nennen (`git rev-parse --short origin/main`). Ohne Schreibzugriff
   das Ergebnis am Ende als Datei an den Nutzer geben.
2. **Vorbereitung beim Nutzer** (Schritt für Schritt, je Schritt auf Rückmeldung warten):
   - Vergleichsquelle: bevorzugt DOSBox mit AdLib (Sound Blaster/OPL2), `TRADERS.BAT` aus `original/dos/` des Referenz-Repos, Ton im
     Menü an; sonst ein Video mit Ton (Hinweis: Videos zeigen teils die Fassung von 1991).
   - Port auf dem Rechner des Nutzers mit Tongerät: `dotnet run --project src/Traders.App -c Release -- <pfad>/original/dos`
     (Standard: Roboterphase übersprungen, nach dem Ende zurück ins Hauptmenü). Ohne Tongerät läuft der Port stumm – dann melden.
   - Optional ohne Spiel: `OPL_WAV_OUT=<verzeichnis> TR_REQUIRE_GAMEDATA=1 dotnet test tests/Traders.Tests -c Release --filter OplTraceTests`
     → `opl-menu.wav` (Menümusik aus der RefEmu-Folge); `traders-tools opl-wav <trace.txt[.gz]> <out.wav> [rate]` rendert weitere Folgen.
3. **Hörstellen** nacheinander, je Stelle erst das Original, dann der Port (gleiche Lautstärke):

   | Nr. | Stelle | So erreichen | Quelle im Port |
   |---|---|---|---|
   | H1 | Vorspann `TRADINTR` | Programmstart | `FxMusicDriver`, Stück 0 |
   | H2 | Menümusik `TRADMENU` (Wiederholung) | Hauptmenü, ≥ 1 Durchlauf | Stück 1, Merker 2 |
   | H3 | Erster Effekt beendet die Menümusik | Neue Partie starten | `21A7:0054` → `218B:0153` |
   | H4 | Tastenklick beim Schreibmaschinentext | Wochenbericht, Feldhinweis, Stationen | Effekt 16h (`TypewriterClicks`, #262) |
   | H5 | Effekte der Woche | Anbau, Ernte, Wetter, Handel, Rakete, Piraten, Fat Mike, Einkauf | Effektmaschine `0000:06FE`–`15B1` |
   | H6 | Endmusik `TRAD_END` | F10 nach einer Woche bzw. Ziel erreicht | Stück 2, Merker 3 |
   | H7 | Ton aus im Menü | Ton im Menü abschalten, Woche spielen | `DS:10A7` = 0 → still |

4. **Je Stelle abfragen** (kurz, Mehrfachauswahl wo möglich): Tonhöhe, Tempo, Klangfarbe/Instrument, Ein- und Ausschwingen,
   Schlagzeug, Lautstärkeverhältnis, fehlende oder hängende Töne; Urteil **gleich / kleine Abweichung / deutlich anders / nicht
   prüfbar** und ein Satz in den Worten des Nutzers. Nicht suggerieren; Unterschiede der echten Soundkarte (Rauschen, Pegel) gelten
   nicht als Abweichung.
5. **Ablage:** `docs/reviews/<JJJJ-MM-TT>-hoervergleich-m14.md` auf Branch `review/hoervergleich-m14` (Basis `main`), PR nur mit
   dieser Datei, Agenten-Trailer; Inhalt (höchstens 80 Zeilen):
   1. Kopf: Datum, Code-Stand, Vergleichsquelle (DOSBox-Version und Einstellungen bzw. Video mit Zeitstempeln), Port-Start
      (Befehl, Betriebssystem, Tongerät).
   2. **Gesamturteil:** M14 erfüllt ja/nein.
   3. Tabelle H1–H7: Urteil, Beschreibung des Nutzers, Zeitpunkt/Bild, Vermutung des Chats getrennt gekennzeichnet („Vermutung“).
   4. Abweichungen als Liste `HV-<n>` mit Stelle, Beobachtung, vermuteter Ursache (Treiber, Effektmaschine, Synthese, Host-Takt,
      Mischung) – nur als Vermutung, kein Befund ohne Beleg (`zusammenarbeit.md` §5 Nr. 7).
6. **Übergabe an die Leitstelle:** Je Abweichung „deutlich anders“ ein Issue `HV-<n>: …` mit Labels `befund` und `P2`, „kleine
   Abweichung“ `P3`; Link auf die Datei. Am Review-PR keine Labels. Antwort an den Nutzer: Link zum PR, Gesamturteil in einem
   Satz. Die Leitstelle setzt danach M14 in `STATUS.md` (✅ oder Restpunkte) und plant Fixläufe.

## Hinweise

- Keine Original-Audiodateien, Aufnahmen oder WAVs ins Code-Repo (AGENTS.md §2.1); Aufnahmen gehören, wenn überhaupt, per PR der
  Leitstelle ins Referenz-Repo.
- Der Chat legt keine geplanten Läufe an (Nutzer 12.11.2026).
