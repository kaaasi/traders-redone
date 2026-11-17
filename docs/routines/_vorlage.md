# <Adresse> – <Kurzname>

- **Ladebild-Adresse:** `SSSS:OOOO` (Dateioffset `0x…`)
- **Aufgabe:**
- **Aufgerufen von / Phase:**
- **Parameter / Rückgabe:**

## Zustand
| DS-Offset | Breite | Vorzeichen | Bedeutung | gelesen/geschrieben |
|---|---|---|---|---|

## Seiteneffekte in Reihenfolge
Zufall, Uhr, Grafik, Eingabe, Ton.

## C#-Umsetzung
`Traders.Core/...`

## Prüfung
Differenztest(s), Grenzfälle, Videovergleich.

Endet das Original nur unter einer Bedingung oder „zieht endlos“ (17.11.2026, Quelle: Leitstelle R8, Erfahrung #217): Der
Differenztest braucht eine Machbarkeitsprüfung im Testaufbau (verwirft den Fall mit klarer Meldung oder scheitert) oder einen
Abbruchzähler im Harness; Empfehlung: eigener Zufallsstartwert je Testfall (`new Random(seed + n)`), keine fortlaufende Folge.
