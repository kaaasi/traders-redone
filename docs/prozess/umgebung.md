# Eingeschränkte Arbeitsumgebungen: früh erkennen, gezielt ausweichen

Ergänzung zu [AGENTS.md](../../AGENTS.md) §3 und §5. Lesen, sobald Build, Tests, Fetch oder GitHub-Schreibaktionen in der eigenen
Umgebung scheitern – nicht vorsorglich. Übernommen aus den Erfahrungen im Eishockey-Manager.

- **SDK vor Build und Review nutzbar machen:** Benötigt wird das .NET-SDK gemäß `global.json`. Zuerst `dotnet --version` im
  Repo prüfen. Fehlt es, auf dem vorgesehenen Weg einrichten (Claude: `tools/claude/setup-workspace.sh`, siehe `CLAUDE.md`) – nicht
  zusätzlich auf anderem Weg oder mehrfach herunterladen. Fehlendes SDK und gesperrter VSTest-Testhost sind unterschiedliche Fehler.
- **Kein lokales SDK möglich:** Branch pushen, PR auf „Ready for review“ stellen und die Check-Runs des Head-SHA abwarten
  (`gh api repos/kaaasi/traders-redone/commits/<sha>/check-runs`); die Zusammenfassung des Laufs enthält die Tabelle von
  `tools/pruefen.sh`. Im PR „lokal blockiert, Nachweis CI-Lauf <link>“ vermerken.
- **Netzwerkversuche begrenzen:** z. B. `timeout 45s git fetch origin`; bei `curl` `--connect-timeout 5 --max-time 30`. Nach
  einem Proxy-/Berechtigungsfehler Ursache klären oder auf einen GitHub-Connector wechseln; denselben blockierten Versuch nicht
  wiederholt starten. Proxy und Sandbox nicht umgehen.
- **GitHub-Schreibaktionen eindeutig beauftragen:** Im Kurzaufruf Remote-Branch, Commit und PR-Erstellung ausdrücklich nennen. Eine
  Ablehnung der Freigabeprüfung ist kein Nachweis fehlender Rechte; dieselbe Aktion nicht über ein anderes Werkzeug umgehen. Ohne
  zulässigen Schreibweg die fertige Datei bereitstellen.
- **VSTest benötigt lokale TCP-Sockets.** Prüfung: `python3 -c 'import socket; s=socket.socket(); s.bind(("127.0.0.1", 0))'`.
  Scheitert VSTest mit `SocketException (13): Permission denied`, sind Teststarts abgebrochen, nicht Tests fehlgeschlagen oder
  bestanden; als blockiert dokumentieren.
- **Build ohne brauchbare Meldung:** `DOTNET_PROCESSOR_COUNT=1 dotnet build TradersRedone.slnx -c Release --no-restore -m:1
  -p:UseSharedCompilation=false -warnaserror`.
