# Regeln für alle Agenten

Gilt für **jeden** Agenten (Claude, ChatGPT, andere) und für Menschen; setzt keinen Chatverlauf und kein Werkzeug voraus.
Werkzeugspezifisches steht getrennt (Claude: [CLAUDE.md](CLAUDE.md)) und darf nicht widersprechen. Zusammenarbeit, Rollen,
Übergabe, Review, Merge-Prüfung: [docs/prozess/zusammenarbeit.md](docs/prozess/zusammenarbeit.md); Sonderfälle:
[docs/prozess/anhang.md](docs/prozess/anhang.md). Aktuelle Pakete: [docs/arbeitspakete.md](docs/arbeitspakete.md) („Aktuelle Runde“).
Ausdrückliche Anweisungen des Nutzers haben Vorrang.

## 1. Projekt

Portierung von **Traders – The Intergalactic Trading Game** (Linel, DOS 1991/92) nach C#/.NET 10: zuerst originalgetreu
(Regeln, Zufall, Abläufe, Pixel), danach abschaltbare Erweiterungen ([docs/PORTIERUNGSPLAN.md](docs/PORTIERUNGSPLAN.md)).
**Sprache:** Deutsch in Doku und Commit-Nachrichten; im Quelltext sind Programmcode und Kommentare stets Englisch (Nutzer
11.11.2026). Ausgabetexte der Werkzeuge dürfen Deutsch sein.

## 2. Harte Regeln

1. **Originaldaten nur an ihrem Platz:** Originaldateien liegen in `original/` (Prüfsummen `original/SHA256SUMS.*`),
   Videobilder in `reference/`, Disassemblierung in `docs/analyse/disasm/`, RefEmu-Bildabzüge in `tests/**/Data/`. Diese
   Dateien nie verändern und nirgends sonst hin kopieren (auch nicht in PR-Texte); neue Ablagen dort nur durch Leitstelle
   oder Nutzer per PR. Wahrheit ist `original/dos/T.EXE` – bei Abweichungen zum Video gilt der RefEmu.
2. **Nie direkt auf `main` schreiben.** Jede Änderung über Branch und Pull Request.
3. **Ein Branch – ein Agent.** Wer an einem Branch arbeitet, öffnet sofort einen Draft-PR und trägt sich als Bearbeiter ein.
   Fremde Branches nur nach dokumentierter Übergabe weiterführen.
4. **Keine Freigabe der eigenen Arbeit;** dafür gibt es ein unabhängiges Review (anderer Agent oder frische Sitzung ohne
   Implementierungskontext).
5. **Nur die eigenen Dateien ändern** (Dateigrenzen der Spur in `docs/arbeitspakete.md`). Gemeinsame Dateien
   (`TradersRedone.slnx`, `Directory.Build.props`, `global.json`, `README.md`, `STATUS.md`, `src/Traders.Core/State/GameState.cs`,
   `src/Traders.RefEmu/OriginalProgram.cs`) höchstens minimal und mit Begründung im PR.
6. **Erst reproduzieren, dann reparieren:** Befund nachstellen → roter Regressionstest → beheben.
7. **Jede portierte Originalroutine** hat einen Befund in `docs/routines/` (Vorlage `_vorlage.md`) und einen
   **Differenztest gegen den RefEmu** (gleicher Eingangszustand → DS-Speicher, RNG-Zustand und Seiteneffekte gleich), im Test
   gekennzeichnet mit `[DIFFERENTIAL] <Segment:Offset>` (nur diese Kennzeichnung zählt für `tools/abdeckung.py`).
   Erwartungswerte nie aus der Portierung selbst gewinnen.
8. **Marker:** Aus dem Original Belegtes bekommt im Code `[ORIGINAL] <Segment:Offset>` (Großbuchstaben, z. B. `21FD:01BA`;
   ohne gültige Adresse meldet `tools/abdeckung.py` einen Fehler), nicht Belegtes `[APPROX]` mit kurzer Begründung. Keine
   ungeprüfte Originaltreue behaupten.

## 3. Einrichtung

| Was | Woher |
|---|---|
| .NET-SDK | Version aus `global.json` (10.0.402, `latestPatch`); ohne Microsoft-Server: Release `claude-werkzeuge` (`tools/claude/setup-workspace.sh`, braucht `gh`). |
| NuGet-Pakete | `dotnet restore TradersRedone.slnx --locked-mode` (Lockdateien eingecheckt); ohne nuget.org: Offline-Spiegel im selben Release. Bewusste Paketänderung: `dotnet restore --force-evaluate`, Lock-Diff prüfen, mitcommitten. |
| Originaldateien, Videobilder | im Repo (`original/`, `reference/`); für reine Codeaufgaben `reference/` per Sparse-Checkout auslassen ([reference/README.md](reference/README.md)). |
| Analyse | `ndisasm` (NASM) für `tools/segmap.py`; Python 3.10+. |

Prüfen: `tools/pruefen.sh`. Scheitern Build, Tests oder GitHub-Schreibaktionen an der Umgebung (kein `dotnet`, kein Netz,
VSTest-Socket gesperrt, Freigabe abgelehnt): [docs/prozess/umgebung.md](docs/prozess/umgebung.md). Blockierte Läufe sind keine
Testergebnisse und werden als blockiert dokumentiert; Proxy, Sandbox und Freigaben nicht umgehen.

## 4. Aufbau

`App (MonoGame) → Presentation (Szenen) → Vga/Audio (Ausgabe) → Core (Zustand, Regeln, KI) → Assets (Originalformate)`;
`RefEmu` führt das Original aus und ist nur Testorakel. Verträge: [docs/ARCHITEKTUR.md](docs/ARCHITEKTUR.md).

- `src/Traders.Assets` – PP20, VGA-Atlas, Palette, Fonts, Masken, Texte, Musikdaten ([docs/RESSOURCENFORMATE.md](docs/RESSOURCENFORMATE.md)).
- `src/Traders.Core` – Spielzustand (DGROUP-Layout), `OriginalRandom`, `Int16Math`, Phasen, Regeln, KI. Neue Daten eines Pakets
  in eine eigene Datei (`partial`).
- `src/Traders.Vga`, `src/Traders.Audio` – persistenter Framebuffer mit Original-Operationen, Ton.
- `src/Traders.RefEmu` – 286-Interpreter, DOS/BIOS/VGA-Stubs, `OriginalProgram` für Differenztests.
- `src/Traders.Viewer`, `src/Traders.Tools` – Asset-Viewer, Export; `tests/Traders.Tests` – xUnit.

**Verbindliche Architekturregeln** (geprüft von `tools/check-architecture.py`, sonst im Review):

- **`Traders.Core` ist deterministisch und plattformfrei:** keine Host-Uhr, keine Dateien, keine Konsole, kein eigener Zufall.
  Uhrwerte und Eingaben sind explizite Eingaben; Zufall nur über `OriginalRandom`; 16-Bit-Arithmetik über `Int16Math`.
- **Zeit und Phasenwechsel nur über die Phasenmaschine des Kerns** (ab Runde 3); keine eigenen Nachhol-Zähler in Szenen.
- **Spielerhandlungen sind Befehle an den Kern,** keine direkten Zustandsänderungen (Vorbereitung Netzwerk E6/E7).
- **Szenen und Rendering lesen nur:** sie buchen nichts, verbrauchen keinen Zufall und ändern keinen Zustand durch Öffnen,
  Pause oder Laden. Was das Original beim Bildaufbau ändert, ist ein Ereignis im Kern, das die Szene genau einmal auslöst.
- **Regelrelevante Pixel** (Kollision gegen `V-RAKETE`, Sichtbarkeit gegen `V-BERGE`) nutzen den kanonischen Original-Atlas im Kern.

**Originaltreue:** (1) **Vorlage:** `T.EXE` (Disassemblierung `tools/segmap.py`, Segmentkarte `docs/analyse/segments.md`,
Datenlayout `docs/analyse/DATENLAYOUT.md`), RefEmu-Lauf, Videobilder `reference/`. (2) **Belegen:** Differenztest gegen RefEmu.
(3) **Bild:** Szene rendern und pixelgenau mit dem RefEmu-Framebuffer vergleichen; Video nur, wo der RefEmu nicht hinkommt.
(4) **Markieren** nach Regel 2.8.

## 5. Tests

- **Einheitliche Prüfung `tools/pruefen.sh`** (Hashes, Architektur, Restore `--locked-mode`, Build mit Warnungen als Fehler, alle
  Tests, Markerbericht; gibt die Tabelle für PR und Übergabe aus). Die Originaldaten liegen im Repo, daher gibt es **einen**
  Lauf; `--schnell` ist gleichbedeutend. Während der Arbeit nur betroffene Tests (`dotnet test … --filter …`), die volle Prüfung
  vor dem Push.
- Die CI (`build.yml`) führt dieselbe Prüfung auf Ubuntu aus; Windows nur per Hand. Grüne CI auf genau dem Head ist Pflicht für
  den Merge; Testanzahl ist keine Abnahme.
- **Differenztests** über `OriginalProgram` (`LoadOnly`, `BootToMainMenu`, `Call`, `DataSegment`, `Frame`); Grenzfälle und
  RNG-Zustand nach der Routine immer mitprüfen.
- **Mehrrundenprobe:** Wer Phasenfolge, Geld, Vorräte, Wochenbilanz oder KI berührt, liefert einen Test über mindestens vier
  Spielrunden (Mensch und Computer), der den Zustand je Runde gegen den RefEmu vergleicht, sobald dieser die Phase erreicht.
- In PR, Übergabe und Review **Belegtes, Vermutetes, nicht Ausgeführtes und tatsächlich Bestandenes** deutlich trennen.

## 6. Git und Pull Requests

- **Branches:** `paket/<spur>`, `fix/<thema>`, `prozess/<thema>`, `integration/<runde>`, `review/<thema>` (nur die
  Review-Datei). Vor jedem Push `git fetch origin && git rebase origin/main` (eigener Branch, `--force-with-lease`) und
  `tools/pruefen.sh` grün.
- **Commits** auf Deutsch, aussagekräftig, mit Agenten-Trailer (z. B. `Co-Authored-By: ChatGPT <noreply@openai.com>`) – alle
  Agenten schreiben über dasselbe GitHub-Konto.
- **Kleine PRs gegen `main`**, Beschreibung nach `.github/pull_request_template.md`; per Weboberfläche oder REST
  (`POST /repos/kaaasi/traders-redone/pulls`), GraphQL ist nicht überall erreichbar.
- **Status** einer Spur nur im eigenen Abschnitt von `docs/arbeitspakete.md`; `STATUS.md` und `CHANGELOG.md` pflegt die Leitstelle.
- **Übergabe immer:** Jede Sitzung endet – fertig wie bei Abbruch – mit einem Übergabekommentar im PR (`docs/handoffs/VORLAGE.md`).
- **Gemergt wird nur von Leitstelle oder Nutzer** nach der Merge-Prüfung (`zusammenarbeit.md` §6), mit Merge-Commit.
- **Keine Tags** (Nutzer); Meilensteine stehen in `STATUS.md`. Einzige Ausnahme: der technische Release-Tag `claude-werkzeuge`
  (`werkzeuge.yml`).
- **Keine Zugangsdaten, Tokens oder privaten Chat-Inhalte** in Commits, PRs, Übergaben oder Reviews.

## 7. Sparsam arbeiten

Jede Nachricht und jede gelesene Datei kostet Tokens – bei allen Agenten:

- **Antworten an den Nutzer knapp:** Ergebnis, Entscheidungsbedarf, nächster Schritt, mit Links. Keine Nacherzählung des
  Arbeitswegs, keine Wiederholung dessen, was in PR, Review oder Repo steht; ausführlich nur auf Wunsch.
- **Vorlagen nicht ausschreiben:** Spurstart, Review-Anfrage und Leitstelle laufen über die **Kurzaufrufe** in `docs/vorlagen/`.
- **Gezielt lesen:** in `docs/arbeitspakete.md` nur die aktuelle Runde und die eigene Spur; `docs/archiv/`, ältere Reviews,
  `docs/analyse/disasm/` und `reference/` nur bei Bedarf. Diffs zuerst mit `--stat`, Logs gefiltert (`tail`, `grep`).
- **Lange Inhalte ins Repo** (Bericht, Plan, Review), im Chat nur der Verweis.

## 8. Wo was steht

| Thema | Datei |
|---|---|
| Stand der Portierung (Meilensteine, Segmente, Routinen) | `STATUS.md` |
| Plan, Meilensteine, Erweiterungen | `docs/PORTIERUNGSPLAN.md` |
| Zusammenarbeit, Rollen, Übergabe, Review, Merge-Prüfung | `docs/prozess/zusammenarbeit.md` |
| Sonderfälle: Vertretung, GitHub Free, Gesamtreview, ChatGPT/Codex, geplante Läufe | `docs/prozess/anhang.md` |
| Eingeschränkte Arbeitsumgebungen | `docs/prozess/umgebung.md` |
| Arbeitspakete, Dateigrenzen, Abnahme, Status | `docs/arbeitspakete.md` |
| Kurzaufrufe: Spurstart, Review-Anfrage, Leitstelle | `docs/vorlagen/` |
| Reviews / abgeschlossene Runden | `docs/reviews/` / `docs/archiv/` |
| Routinenbefunde, Analyse, Formate | `docs/routines/`, `docs/analyse/`, `docs/RESSOURCENFORMATE.md` |
| Erkenntnisse des Vorversuchs | `docs/ERKENNTNISSE_VORVERSUCH.md` |
