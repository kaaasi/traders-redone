# Traders – Erkenntnisse für einen vollständigen Neustart

## 1. Zuerst klären: Was mit „Originalsourcecode“ gemeint ist

Die bisherige Arbeit stützt sich auf die DOS-Datei **T.EXE**, ihre Ressourcen,
Disassemblierung und die Ausführung ausgewählter Originalroutinen in einer
Referenzumgebung. Ein vollständiger ursprünglicher Hochsprachen-Quellbaum ist durch
die Übergabe **nicht belegt**. Die C#-Dateien sind die rekonstruierte Portierung,
nicht der originale Entwicklerquellcode.

## 2. Die wichtigsten Erkenntnisse auf einen Blick

1. **Der richtige Raketenbildschirm ist V-RAKETE.PP.** V-BERGE.PP ist die
   Katastrophenszene. Dieser frühere Zuordnungsfehler betraf Grafik **und** Flugkollision.
2. Die VGA-Ressourcen sind nicht bloß fertige Hintergrundbilder. Außerhalb des
   sichtbaren Bildbereichs liegen relevante Sprites und Grafikbausteine.
3. Originalgrafik kann an der Simulation beteiligt sein: Bildpixel werden bei
   Flug und Sternvorführung ausgewertet. Darstellung und Regeln lassen sich erst
   nach Verständnis dieser Abhängigkeiten sauber trennen.
4. Die Reihenfolge und Anzahl originaler Zufallsaufrufe muss exakt erhalten bleiben.
   Auch scheinbar rein kosmetische Abläufe können den späteren Spielzustand beeinflussen.
5. Originale zeitabhängige Zufalls-Neusaat muss als Eingabe aufgezeichnet werden.
   Gleicher anfänglicher Seed allein garantiert keine gleiche Partie.
6. 16-Bit-Zwischenergebnisse, Vorzeichen, Überläufe und Ganzzahldivision sind Teil
   des beobachteten Verhaltens. Eine mathematisch „saubere“ Neurechnung kann falsch sein.
7. Die langen Verkäuferbalken entstehen aus einer **erhaltenen Bildspur**. Ein
   korrekt ausgeschnittenes Einzelbild der Figur reicht nicht.
8. KI umfasst strategische Vorbereitung und phasenspezifische Entscheidungen.
   Nur eine sichtbare Bewegungsroutine zu portieren reicht nicht.
9. Phasen benötigen gültige Eingangszustände. Ein gesetzter Phasenname allein ist
   weder ein spielbarer Testfall noch eine zuverlässige Integration.
10. Einzelroutinen, Bildfolgen und ganze Partien benötigen unterschiedliche Prüfungen.
    Erfolgreiche C#-Partien allein belegen keine vollständige Originalgleichheit.

## 3. Originaldateien und Analysebasis zuerst sichern

### 3.1 Die konkrete EXE-Version identifizieren

Die bisherigen Adress- und Testreferenzen beziehen sich auf diese Original-EXE-Prüfsumme
laut gespeichertem Regelprofil:

```text
SHA-256 T.EXE:
b73a2170922b2c797b3478e5b010bf890b3b9360452bce28968752b989af0185
```

Beim Neustart die tatsächlich vorliegende Datei hashen. Bei Abweichung alte Adressen,
Tabellen und Testfälle erst auf die andere Fassung abbilden. Dateinamen allein sind
kein Versionsnachweis.

### 3.2 Adressen und Daten systematisch dokumentieren

Bei der Untersuchung des DOS-Programms Dateiposition, geladenes EXE-Abbild,
Segment:Offset und Datenadressen unterscheiden. Relokationen berücksichtigen.
Sprungtabellen und andere Daten nicht versehentlich als ausführbaren Code deuten.

Für jede portierte Routine empfiehlt sich ein kurzer Befund mit:

- Originaladresse und fachlicher Aufgabe;
- gelesenen und geschriebenen Zustandsfeldern;
- Datenbreiten, Vorzeichen und Tabellenindizes;
- Voraussetzungen und aufrufender Phase;
- Zufalls-, Uhr-, Grafik- und Eingabeaufrufen;
- mindestens einem nachvollziehbaren Originalvergleich.

Das ist eine Empfehlung für den Neustart, keine Behauptung eines bereits vollständig
vorhandenen Originalsymbolverzeichnisses.

## 4. Ressourcen: Der wichtigste bereits geklärte Zuordnungsfehler

### 4.1 Bestätigte originale Ressourcenliste

Bisher dokumentierter Einstieg: **186C:0002**, Ressourcenliste bei **DS:09CE**,
Einträge von **19 Bytes**. Adressen gelten für die untersuchte Originalfassung.

| Index | Datei | Bedeutung im bisherigen Befund |
|---:|---|---|
| 0 | V-ANBAU.PP | Spielfeld und zahlreiche Wirtschafts-/HUD-Bausteine |
| 1 | V-RAKETE.PP | Tatsächliche Stations- und Raketenflugphase, Bereich 1B03 |
| 2 | V-BERGE.PP | Katastrophenszene, Bereich 111D |
| 3 | V-FATMIK.PP | Fat Mike und Schneckenanzeigen |
| 4 | V-AHANDE.PP | Auktionsgrafik |
| 5 | V-BHANDE.PP | Weitere Auktionsgrafik |
| 6 | V-MENU.PP | Menüs |
| 7 | V-ENDE.PP | Endsequenz |
| 8 | V-KAMPF.PP | Kampfressource; Kampf noch nicht vollständig portiert |
| 9 | V-ABEND.PP | Abend-/Schlussdarstellung |

Die genaue Auswahl der beiden Auktionsressourcen je Rohstoff aus den vorhandenen
Referenzen übernehmen bzw. erneut prüfen; aus ihrem Dateinamen allein nicht ableiten.

### 4.2 Warum der alte Raketenansatz falsch war

Der frühere Port verwendete V-BERGE für den Flug. Die Spritebereiche unterhalb der
sichtbaren 200 Zeilen waren dort leer. V-RAKETE enthält die zum tatsächlichen Flug
gehörenden Bausteine und den Bildschirm mit der großen roten Rakete am Baum.

Das war kein bloßer Austausch einer Illustration: Das verwendete Pixelbild ging
auch in die Flugauswertung ein. Mit V-RAKETE wurden deshalb die Originalvergleiche
neu erzeugt: **160 Flugfälle und 80 Zwischenstände**.

**Für den Neustart:** V-RAKETE direkt als kanonische Flugressource verwenden.
V-BERGE nicht als angeblich korrekten späteren Flugabschnitt beibehalten.
Die zwischenzeitliche Idee, V-RAKETE nur als zusätzliches Zwischenbild einzufügen,
behebt den eigentlichen Fehler nicht.

### 4.3 Vollständige Bilddaten erhalten

Die bisherige VGA-Auswertung arbeitet mit 320 × 256 Pixel großen Ressourcen bei
320 × 200 sichtbaren Pixeln. Die unteren Zeilen enthalten unter anderem Sprites.
Originale `.PP`-Ressourcen werden entpackt; die bestehende PowerPacker-Auswertung ist
ein sinnvoller wiederverwendbarer Ausgangspunkt. `FONTS.PP`, `MASKS.PP` und die
Originaltexte gehören zur Darstellung, nicht nur die großen VGA-Bilder.

Bildindizes, Palette, Masken und vollständige Atlasabmessungen erhalten. Für das
Spiel relevante Indizes nicht durch einen beliebigen RGB-Export ersetzen.
Ein hübsches PNG allein ist kein Ersatz für die ursprünglichen Pixel-/Maskendaten.

## 5. Grafik und Spielzustand: bekannte Fallen

### 5.1 Atlas, Zeichenfläche und Kollisionsfläche unterscheiden

Bei den Flügen werden originale einpixelige Spuren in einer erhaltenen Fläche
geführt. Nicht nachträglich größere Projektile oder erfundene Stationszahlen über
Originalgrafiken malen. Originale Stationsnamen und P-Kennzeichen aus den passenden
Zeichenroutinen ableiten.

Unveränderliche geladene Grafik und veränderlicher Bildpuffer sind unterschiedliche
Dinge. Ein Blit aus dem Originalatlas ist nicht automatisch eine Kopie aus dem
zuletzt angezeigten Frame. Diese Unterscheidung beeinflusste auch die Sternvorführung.

**Architekturfolgerung:** Eine eventuell regelrelevante Pixel-/Kollisionsauswertung
muss reproduzierbar im Kern bzw. einem kanonischen Inhaltsadapter verfügbar sein.
Später austauschbare moderne Grafiken dürfen keine anderen Flugergebnisse verursachen.

### 5.2 Verkäuferbalken sind zeitliche Bildspuren

Im Original bleibt beim Abwärtsbewegen eines Verkäufers die oberste Spritezeile
an bisherigen Positionen stehen. Daraus entstehen die langen Balken bzw. Rohre über
den Figuren. Ein Renderer, der jedes Bild vollständig neu aufbaut, verliert diesen
Effekt, wenn er ihn nicht bewusst rekonstruiert.

Die frühere Prüfung nur der kurzen Spriteoberkante übersah genau diesen Fehler.
Nötig sind Vergleiche mehrerer aufeinanderfolgender Frames und die Rekonstruktion
nach Laden eines Spielstands **ohne** vorherige Browserbilder. Käufer und bereits
ausgestiegene Spieler getrennt prüfen.

### 5.3 Masken, Schrift und Zahlendarstellung nicht vereinheitlichen, bevor sie verstanden sind

Falsche Spritebreiten und eine unpassende Schrift führten zu falschen Figuren und
überlappenden Zahlen. Originale Quellrechtecke, Koordinaten, Ziffernbreiten und
Maskenaufrufe übernehmen statt die Menüschrift überall einzusetzen.

Für die neu analysierten Szenen ist die native Argumentreihenfolge von
**A96:013B** in das interne Maskenformat umgesetzt worden. Nicht daraus schließen,
dass jede bisherige Referenzdatei bereits dieselbe Reihenfolge verwendet.
Eine Änderung des gemeinsamen Adapters erfordert Prüfung der betroffenen Referenzen.

### 5.4 Piratenanimation besteht aus mehreren Zuständen

Dokumentierter Befund: **1B03:131B**, Grafik-Opcode 7, zeichnet einen acht Pixel
breiten Streifen mit 16 Pixel Höhe für die Kanonenanimation. Vier Kanonenbilder und
aktive/pausierte Gesichter wurden in 24 Kombinationen gegen Originalaufrufe geprüft.

Das Gesicht folgt der Pause-/Angriffsstellung, nicht schlicht `Frame % 2`.
Vollständige Laser- und Abgangseffekte waren am Ende von v0.17 weiterhin offen.
Die vorhandene Teilreferenz nicht als vollständigen Piratenablauf ausweisen.

### 5.5 Fat Mikes Zeigestab benötigt Kontinuität

Explizite Nutzeranforderung und in v0.17 behobenes Verhalten: Beim zweiten und
dritten Angebot vom vorherigen Betrag zum neuen Ziel wandern. Nicht jedes Mal
vom rechten Bildrand beginnen.

Position entweder explizit speichern oder eindeutig aus Angebotsindex, vorherigem
Ziel und gespeichertem Animationstakt ableiten. Dabei Ersterscheinen, Angebotswechsel
und Abgang auseinanderhalten. Ein Bilderzeuger darf die Position nicht bei jedem
Rendern neu initialisieren.

### 5.6 Schlusssequenzen dürfen keine Referenz-Beispielwerte einfrieren

Die originale große Rang-/Vermögensanzeige wird als Mosaik aufgebaut. Eine statische
Aufzeichnung von Zeichenbefehlen kann Werte einer Beispielpartie enthalten.
Im Spiel müssen die großen Ziffern aus dem tatsächlichen Ergebnis entstehen.
Nur die kleine Zahl zu ersetzen, während das große Mosaik unverändert bleibt,
wäre weiterhin falsch.

## 6. Zufall, Zeit und Zahlenarithmetik sind Teil der Originaltreue

### 6.1 Zufallsaufrufe vollständig erhalten

Der gesamte Aufrufpfad zählt: Bereichsgrenzen, Reihenfolge, bedingte Aufrufe und
Sonderbehandlung von Nullgrenzen. Einen vermeintlich unwichtigen Zufallsaufruf zu
entfernen, kann alle späteren Ereignisse und KI-Entscheidungen verändern.

In der bisherigen Entwicklung wurden tatsächlich fehlende Originalzufallsaufrufe
gefunden. Auch die Katastrophen-Sternvorführung verbraucht Zufall abhängig von
Sichtbarkeitsprüfungen gegen **V-BERGE**. Sie darf nicht ersatzlos entfallen, nur
weil man die eigentliche Katastrophenrechnung schon implementiert hat.

**Saubere Trennung:** Originale spielrelevante Aufrufe einmal an der vorgesehenen
Stelle durchführen. Die Anzeige später aus gespeicherten Informationen bzw. einer
Kopie des damaligen Seeds rekonstruieren. Rendern und Laden dürfen nicht erneut
Zufall aus dem laufenden Spielgenerator verbrauchen.

### 6.2 Uhr-Eingaben aufzeichnen

Die Originalroutinen können die Zufallsfolge aus Uhrzeit neu initialisieren.
Eine Wiederholung braucht deshalb nicht nur einen Anfangsseed, sondern auch die
betreffenden zukünftigen Uhr-Eingaben. Diese sollten kontrolliert eingespeist und
für Wiederholungen protokolliert werden.

Browserbildrate, Zahl der PNG-Abfragen oder die Geschwindigkeit des Testrechners
dürfen keine zusätzliche Spielentscheidung auslösen. Feste Simulationstakte und
separate Anzeige sind dafür die sinnvolle Grundlage.

### 6.3 Datenbreiten und Rechenreihenfolge bewahren

Bei der Übertragung in C# bzw. eine andere moderne Sprache besonders prüfen:

- vorzeichenbehaftete und vorzeichenlose Vergleiche;
- 16-Bit-Zwischenergebnisse gegenüber 32-Bit-Geldwerten;
- Überlauf vor einer nachfolgenden Division;
- Abschneiden bei Ganzzahldivision und Behandlung negativer Werte;
- Bereichsgrenzen von Zufallsfunktionen und Tabellenindizes;
- Reihenfolge von Mutation, Rangberechnung und Folgeschritt.

Eine algebraisch gleichwertig wirkende Umformung kann das beobachtete
Maschinenverhalten verändern. Originalvergleiche müssen auch Grenzfälle umfassen.

Historischer Sprachfehler im Plan: „80-%-Schwund“ ist keine verlässliche neue
Regeldefinition. Für die übliche betreffende Vorratsrechnung sind **80 % Restbestand
bzw. 20 % Verlust** gemeint. Maßgeblich bleiben Originalroutine und geprüfte Werte.

## 7. KI, Regeln und Phasen vollständig zusammendenken

### 7.1 Strategie ist eine eigene Grundlage

Die bisherige Analyse trennte strategische Vorbereitung und taktische Anbau-KI.
Die Strategievorbereitung besitzt zwei später benötigte Berechnungszweige.
Eine einzelne Bewegungs- oder Bewertungsroutine ohne ihre vorbereiteten Tabellen
ist noch keine vollständig portierte Original-KI.

Ebenso besitzen Stationswahl, Handel, Bonusrennen, Einkauf und weitere Abschnitte
jeweils eigene Entscheidungsabläufe. Vor einer Portierung Eingangsdaten und
Aufrufreihenfolge des Originals klären, nicht alle Phasen mit einer allgemeinen
neu erfundenen KI füllen.

### 7.2 Vier IQ-Stufen: Originalwerte und Erweiterung auseinanderhalten

| Originalwert | Bezeichnung |
|---:|---|
| 0 | DIM |
| 1 | GOOD |
| 2 | SHREWD |
| 3 | MEAN |

Die vier Stufen sind original. **Die unabhängige Auswahl je KI-Spieler ist der
gewünschte erweiterte Bedien-/Konfigurationsumfang.** Bei der bisherigen Analyse
wurde ein global ausgewerteter Computer-IQ durch den Wert des handelnden Spielers
ergänzt. Allgemeine Regeln wie Öl, Piraten und Katastrophen nicht damit vermischen.

Für Originalvergleiche alle Computer auf denselben globalen Wert setzen; die
Erweiterung anschließend mit gemischten IQs prüfen. Die ursprünglichen allgemeinen
Schwierigkeitsvorgaben verwendeten laut Plan DIM, GOOD, SHREWD und SHREWD; das macht
MEAN nicht zu einer unerwünschten oder erfundenen Stufe.

### 7.3 HQ, Eingaben und Phasengrenzen

- Ein HQ gehört auf eine freie grüne Ebene. Direkt links oder rechts darf kein
  weiteres HQ stehen. Enter auf ungeeignetem Gelände muss nicht zum Bau führen.
- Ein früherer menschlicher Enter-Fehler bestand trotz funktionierender KI.
  Deshalb menschliche Eingaben genauso prüfen wie automatische Entscheidungen.
- Gehaltene Taste, einmalige Aktion, KI-Verzögerung und Darstellungstakt trennen.
- Das originale Time Limit gehört laut bisherigem Befund nicht zur ersten Anbauphase.
  Nicht aus einem vorhandenen Countdown schließen, dass er überall gelten muss.
- Nach der letzten Auktion ist der zusätzliche Port-Bildschirm
  „Rohstoffhandel abgeschlossen“ kein zu bewahrender Originalspielabschnitt.

### 7.4 Einmal rechnen, sichtbar vorführen

Bei Produktion müssen Ausgangsvorräte, Betriebsmittelverbrauch, Schwund,
Feldsammlung und Endbestand nachvollziehbar zusammenpassen. Originale Feldbesuche,
Figurenbewegung und laufende Anzeigen sind ein Teil des gewünschten sichtbaren Ablaufs.

Die Anzeige darf die bereits durchgeführte Rechnung nicht erneut buchen.
Dasselbe gilt für Ereignisse, Handelsabschluss und Siegprüfung. Wiederholtes Rendern,
Pause oder Laden dürfen keine zweite Auszahlung und keinen zweiten Phasenabschluss erzeugen.

### 7.5 Bedingte Phasen von Anfang an abbilden

Eine Wirtschaftsrunde ist keine immer gleich lange lineare Liste: Stationen/Flüge,
Bonusrennen, Schneckenprämien, Katastrophen, Sieg und Kampf hängen von Bedingungen ab.
Ein vollständiges Phasenmodell einschließlich Ein-/Austrittsbedingungen aufbauen.

Roboterkampf und Originalton waren in v0.17 noch offen. V-KAMPF.PP allein oder eine
Arena-Vorschau beweisen keine vorhandene Kampfmechanik, Kampf-KI oder Audioportierung.

## 8. Praktische Einstiegspunkte aus der bisherigen Analyse

Diese Adressen sind Arbeitsreferenzen für die oben identifizierte EXE-Fassung,
keine offiziellen Quelldatei-/Symbolnamen. Die Aufgabenbeschreibungen begrenzen,
was damit bisher tatsächlich untersucht wurde.

| Originalbereich / Adresse | Hilfreicher Anknüpfungspunkt |
|---|---|
| 186C:0002 / DS:09CE | Ressourcenliste und Zuordnung der Bilddateien |
| Bereich 0609 | Anbau-/Spielfeld- und Anzeigeaufrufe; Details in den alten Referenzen |
| 0825:0072 | Runden-/Bilanzdarstellung |
| 0825:06CB | Vermögensberechnung als Anknüpfungspunkt der bisherigen Rekonstruktion |
| Bereich 111D; 111D:0112 | Katastrophen und zugehörige Bild-/Sternvorführung |
| 17CB:0986 | Originales Ereignisfenster |
| 17CB:02BD / 039D | Anzeigen für Geldzuwendung bzw. Abgabe |
| Bereich 1B03 | Stationswahl, Raketenflug, Ergebnisse und zugehörige KI |
| 1B03:131B | Kanonen-Streifenanimation der Piraten |
| 1DEF:0227 | Bonusrennen-Bildaufbau / Anzeigen |
| Bereich 1F63 | Fat Mike und zugehörige Anzeigen |
| Bereich 08CF | Einkaufsphase und deren Bildaufbau |
| 22A3:08C1 | Originale Wettertexte/-vorführung |
| 20B3:015B / 0525 / 0863 | Teile von Schlussauswertung, Abendbild und Endbildfolge |
| A96:013B | Maskierte Grafikoperation; Argumentreihenfolge beachten |

Nicht jede hier aufgeführte Routine ist bereits in allen Varianten vollständig
verstanden. Vor allem Kampf, Ton und seltene Grafik-/Ereignisvarianten weiter analysieren.

## 9. Angefragte Sonderfunktionen und Erweiterungen

### 9.1 Ausdrücklich vom Nutzer angefragt

| Funktion | Konkret gewünschter Umfang | Bisheriger Stand / Einordnung |
|---|---|---|
| Netzwerkspiel mit Spieler als Host | Ein Spieler spielt selbst mit und hostet die Partie. | Als spätere Erweiterung angefragt; nicht fertig implementiert. |
| Dedicated Server | Alternativ betreibt ein eigenständiger Server die Partie. | Als spätere Erweiterung angefragt; nicht fertig implementiert. |
| Schwierigkeit pro Computergegner | Jeden KI-Spieler unabhängig konfigurieren. Vier Originalstufen DIM, GOOD, SHREWD, MEAN beibehalten. | Im bisherigen Port vorhanden; bei Neustart erneut vorsehen. Die anfängliche Bitte um drei Stufen wurde ausdrücklich durch vier Originalstufen ersetzt. |
| Modernere Grafiken | Später modernere Darstellung ermöglichen. | Spätere Erweiterung; ursprüngliche VGA-Portierung hat zunächst Vorrang. |
| Übersetzungen | Spiel in weitere Sprachen übersetzen können. | Spätere Erweiterung; keine konkrete abschließende Sprachliste festgelegt. |
| Cheatmenü | Mehr Geld, mehr Schnecken, mehr Rohstoffe, schnellere Bewegung, Unverwundbarkeit und gegebenenfalls weitere Cheats. | Spätere Erweiterung; konkrete Wirkungsgrenzen noch nicht abschließend festgelegt. |
| Direkte Phasensprünge per Buttons | Einzelne Abschnitte ohne Durchspielen der vorherigen Phasen direkt testen. | Gültige spielbare Teststände statt bloßer Phasenetiketten. |

Diese angefragten Cheats sind einzeln zu berücksichtigen:

1. Geld hinzufügen.
2. Schnecken hinzufügen.
3. Einzelne Rohstoffe/Vorräte erhöhen.
4. Schnellere Bewegung.
5. Unverwundbarkeit.
6. Weitere sinnvolle Cheats erst nach konkreter Festlegung.

**Noch keine festgelegte Bedeutung:** „Unverwundbarkeit“ könnte sich auf Kampf,
Katastrophen oder andere Verluste beziehen. Diese Wirkungen nicht eigenmächtig als
einen umfassenden Schutz gegen alles festschreiben. Ebenso ist schnellere Bewegung
nicht automatisch eine schnellere globale Partie oder nur eine schnellere Animation.

### 9.2 Im Plan ausgearbeitete Architekturvorschläge

Die folgenden Punkte konkretisieren die Wünsche sinnvoll. Sie sind keine zusätzlichen,
einzeln vom Nutzer verlangten fertigen Spielfunktionen:

- Ein verbindlicher Spielkern für lokale Partie, mitspielenden Host und Dedicated Server.
- Gemeinsame geprüfte Spielerbefehle; Netzwerkclients dürfen nicht durch Wechsel der
  lokalen aktiven Figur beliebige andere Spieler steuern.
- Server entscheidet über KI, Zufall und Cheatberechtigungen.
- Spätere Lobby, Platzvergabe, Wiederbeitritt sowie Verhalten bei Verbindungsabbruch.
- Versionierte Zustandsabbilder und passende Regel-/Inhaltsversionen zwischen Teilnehmern.
- Moderne Grafik über semantische Asset-IDs anbinden und Original-VGA auswählbar erhalten.
- Sprachen über Text-IDs mit Parametern statt über fest eingebaute deutsche Meldungen;
  Unicode, längere Texte und spätere geeignete Schrift-/Layoutsysteme vorbereiten.
- Cheats zentral validieren und als explizite, gespeicherte Modifikatoren kennzeichnen.
- Testnavigation mit reproduzierbarem Neustart und Rückkehr zur vorherigen Partie.

Die bisherige Empfehlung war: erst Originalportierung, danach lokales Cheatmenü,
zusätzliche Sprache/Grafikpaket, Host-Netzwerk und Dedicated Server. Diese spätere
Reihenfolge ist eine Empfehlung und kann vom Nutzer geändert werden.