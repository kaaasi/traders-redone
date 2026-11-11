<!-- Regeln: AGENTS.md und docs/prozess/zusammenarbeit.md. Nicht Zutreffendes mit „–“ beantworten, nicht löschen. -->

**Bearbeiter:** <Agent> · **Spur:** <name> · **Befunde/Issues:** <IDs>

## Prüfstand
- Base: `<sha>`
- Head: `<sha>`

## Was und warum
<!-- Ziel, Originalroutinen (Segment:Offset), Änderungsumfang -->

## Originalbefund und Differenztest
<!-- Befund docs/routines/…; Differenztest gegen RefEmu: Name, Eingangszustände, Grenzfälle, geprüft (DS, RNG, Seiteneffekte) -->

## Fehler vorher / Regressionstest
<!-- Bei Fixes: Ablauf und beobachteter Wert vor dem Fix; Test rot vor dem Fix (wie gezeigt?), grün danach -->

## Tests zum Head (Ausgabe von `tools/pruefen.sh --schnell` einfügen; alle drei Läufe nur bei Integration/Test-Infrastruktur, AGENTS.md §5)
| Lauf | bestanden | fehlgeschlagen | übersprungen |
|---|---:|---:|---:|
| Mit Originaldaten (`TR_REQUIRE_GAMEDATA=1`) | | | |

Mehrrundenprobe (falls Phasenfolge, Geld, Vorräte oder KI berührt): <Test / –>

## Gemeinsame Dateien und Dateigrenzen
<!-- Berührte Dateien außerhalb der eigenen Spur mit Begründung, sonst „keine“ -->

## Restlücken, neue [APPROX]
- …

## Auswirkungen auf GitHub-Kontingente
<!-- neue/geänderte Workflows, Artefakte, Laufzeit – sonst „keine“ -->
