# Gesamtreview Runde 3 – Fortsetzung

Reviewer: Codex, unabhängige Sitzung. Base `7a645ffbd9bc08d85c5bb8088a7fe78efe8989c5`,
geprüfter Head `fd46c2ba100d0ee55fe9cb0f0b4741f0dc8f1107`; Fortsetzung von `2026-11-12-r3-gesamt.md`.
Vor dem Lauf: `git checkout` des Head, `git rev-parse HEAD` gleich Head, `git status --porcelain` leer.

## 1. Merge-Empfehlung

**Ja für den geprüften Head**, soweit der Stand der Runde betroffen ist: keine neuen Produktfehler nachgewiesen;
Originaldatenlauf und beide Gegenproben sind erfolgreich. Das ist keine Aussage über noch nicht portierte Szenen.

## 2. Befunde

Keine neuen nachgewiesenen P0–P3-Fehler.

**Testlücke:** Der Bildtakt und Zufallsverbrauch eines wartenden Handlers werden mit einem synthetischen
Handler geprüft ([PhaseMachineTests.cs:131](https://github.com/kaaasi/traders-redone/blob/fd46c2ba100d0ee55fe9cb0f0b4741f0dc8f1107/tests/Traders.Tests/Phases/PhaseMachineTests.cs#L131)).
Die Mehrrundenprobe führt Bildschirmphasen im RefEmu aus und übernimmt deren Zustand; sie belegt daher
noch keinen bildgenauen Vergleich eines portierten Bildschirmhandlers samt Zufallsfolge (F3 = A).
Die Vierwochenprobe verwendet vier Computer; Mensch ab R4 ist bereits in #81 offen.

**Erwartet offen bestätigt:** #39 (R3-Anteil), #49, #74–#76 und #81 waren bei der GitHub-Abfrage offen;
keine erneute Bewertung dieser bekannten Punkte.

## 3. Ausgeführte Prüfungen

| Prüfung | Ergebnis |
|---|---|
| `dotnet --version` | `10.0.402` |
| `tools/pruefen.sh --schnell`, Originaldaten aus lokalem Referenz-Checkout | Hashes, Architektur, Abdeckung, Restore und Release-Build grün; Testtabelle unten |
| R3T-B1: temporär `Production.cs` Faktor `+25` → `+26`; `TR_REQUIRE_GAMEDATA=1 dotnet test tests/Traders.Tests --filter FullyQualifiedName~Four_weeks_with_mixed_field_kinds_match_the_original` | rot: Woche 0, Schritt 13, `DS:48F9` Port `0A`, RefEmu `09` |
| R3T-B2: temporär Rückschreiben in `PhaseMachine.Invoke` entfernt; gleicher Befehl mit Filter `FullyQualifiedName~Four_weeks_with_ported_rule_steps_match_the_original` | rot: Woche 0, Schritt 13, RNG Port `8EF3671D`, RefEmu `DB5C7693` |
| `gh api repos/kaaasi/traders-redone/commits/fd46c2ba100d0ee55fe9cb0f0b4741f0dc8f1107/check-runs` | Linux erfolgreich, Windows und Downloads übersprungen |

| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| Mit Originaldaten (`TR_REQUIRE_GAMEDATA=1`) | 635 | 0 | 0 |

Die Mutationen erfolgten in einer temporären Kopie; die grünen Tests liefen ohne Mutation. Der erste
Schnellversuch brach beim Restore wegen eines nicht beschreibbaren .NET-Profils ab; der erfolgreiche Lauf
verwendete ein temporäres Profil und lief außerhalb der Sandbox. Während der Tests wurde auf den Review-Branch
gewechselt, daher meldete das Skript am Ende `c947c46`: `git diff fd46c2b..c947c46` enthält ausschließlich
die vorherige Review-Datei, keine Quell-, Test- oder Werkzeugänderungen. Build und Test begannen auf `fd46c2b`.

Risikoprüfung: `Invoke` lädt `DS:10D2`, schreibt einen veränderten Generator zurück; die Gegenprobe trifft
den Übergang. Die Regeltests vergleichen RefEmu-DS und RNG, einschließlich Schwund-Überlauf und Abschneiden;
Warenpreisproben überwachen Original-Zufallsaufrufe und Seed. Disassemblierung `22A3:0295–029F` zeigt
16-Bit-Schieben und `idiv`, `0B9F:06A0/078A` die dokumentierte Arithmetik. Maschinenabzugstests vergleichen
Fortsetzung an der nächsten Marke einschließlich Zustandsbestandteilen; Grafiktests vergleichen RefEmu-
`VgaAdapter` mit dem Port einschließlich DAC, VRAM, CRTC und Retrace-Zählung, soweit die jeweiligen Tests dies prüfen.

**Akzeptierte `[APPROX]`:** Tastentabelle 1/0 und ein Takt = ein 70-Hz-Bild. **Annahmen:** keine zusätzliche
Originaltreue aus synthetischen Bildhandlern oder nicht portierten Szenen abgeleitet.
