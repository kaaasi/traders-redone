# Arbeitsregeln für alle Agenten

Diese Regeln gelten für Claude, ChatGPT und andere Werkzeuge. Vor jeder Aufgabe auch
CONTRIBUTING.md, docs/ARBEITSPAKETE.md und den zugehörigen Übergabebericht lesen.
Explizite Anweisungen des Nutzers haben Vorrang.

- Dokumentation und Commits Deutsch; Code-Bezeichner und Code-Kommentare Englisch.
- Original ist die Wahrheit: original/dos/T.EXE; vollständiger Hash in original/SHA256SUMS.dos.
  Jede portierte Routine benötigt einen Befund in docs/routines/ und einen Differenztest.
- Traders.Core bleibt deterministisch und plattformfrei. Uhrwerte sind explizite Eingaben;
  Zufall ausschließlich über OriginalRandom. Rendering bucht nichts und verbraucht keinen RNG.
- Originale, Referenzdaten und 16-Bit-Semantik nicht beiläufig verändern.
- Ein aktiver Bearbeiter pro Aufgabenbranch, ein Worktree/Clone pro gleichzeitigem Agenten.
  Niemals im gemeinsamen Checkout parallel arbeiten. Keine Hintergrundagenten ohne Auftrag.
- Paket-Chats schreiben nur in ihren zugewiesenen Bereich (ci/arbeitspakete.json).
  Gemeinsame Dateien und bestehende fremde Routinenbefunde benötigen Koordination.
- Nur die Leitstelle integriert nach main. Die Rolle ist übertragbar und an keinen Anbieter
  oder Session-Trailer gebunden. Nutzerauftrag zur Repositorypflege autorisiert die nötigen
  gemeinsamen Änderungen auf einem separaten Branch.
- Kein Force-Push, kein Rebase veröffentlichter Paket-Commits. Dies sind Arbeitsregeln,
  keine auf GitHub Free technisch erzwungenen Schutzmaßnahmen.
- main per Fetch und Merge einbeziehen. Übernahme über PR mit Merge-Commit; kein automatisches
  Zusammenführen nur aufgrund grüner CI. Kombinierten Stand prüfen.
- Nach jedem abgeschlossenen Teilschritt docs/uebergabe/APn.md aktualisieren. Vor Wechsel des
  Bearbeiters Änderungen committen/pushen und den bisherigen Bearbeiter ausdrücklich pausieren.
- STATUS.md beschreibt integrierten Stand, Übergaben beschreiben Branch-Arbeit. Erkenntnisse,
  Vermutungen, nicht ausgeführte Tests und tatsächlich bestandene Tests deutlich unterscheiden.
- Keine Zugangsdaten, privaten Chat-Inhalte oder Tokens in Commits/Übergaben aufnehmen.

Werkzeuge und Umgebungsgrenzen stehen in docs/UMGEBUNGEN.md. Fehlendes SDK ist eine Eigenschaft
der aktuellen Umgebung, nicht eines Anbieters. Siehe docs/ARCHITEKTUR.md für Schnittstellen.
