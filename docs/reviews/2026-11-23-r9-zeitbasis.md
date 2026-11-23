# Review R9 `zeitbasis` – PR #250 (Head `bd7190a`, Base `9c04b1e`)

Reviewer: Claude Haiku 5.5 (geplanter Lauf, eigene Sitzung, nicht am PR beteiligt). Nur Codelesen: Checkout `bd7190a`,
Diff `9c04b1e..bd7190a` = 1 Datei (`docs/arbeitspakete.md`, +6/−2). Kein Code, kein `tools/pruefen.sh` (entfällt).

## 1. Merge-Empfehlung

**Nein** für `bd7190a` – nur Codelesen. Grund: Der Phasenbezug ist im selben Absatz widersprüchlich (R9T-1). Die Korrektur ist
ein Satz; danach ist die Vorgabe konsistent und ein Ja naheliegend.

## 2. Befunde

**R9T-1 (P2) – Phase der Musikticks doppelt und widersprüchlich festgelegt.**
[`arbeitspakete.md` Z. 40](https://github.com/kaaasi/traders-redone/blob/bd7190a0c8590a06178c605038dde96234297dfd/docs/arbeitspakete.md#L40)
(„Phase relativ zu Fn 01h“) gegen [Z. 43](https://github.com/kaaasi/traders-redone/blob/bd7190a0c8590a06178c605038dde96234297dfd/docs/arbeitspakete.md#L43)
(„Phase aus dem Zeitgeberzähler von `T.EXE`“). Beleg: Die Messtabelle in `docs/reviews/2026-11-23-r9-musik.md` (main, R9M-1)
zeigt Fn-01h-relative Abstände von 0,907 bis 3,010 (in 4000h) – Fn 01h ist also kein Phasenanker. Der Test „Phase gegen
RefEmu-Probe“ (Z. 43) braucht genau einen Bezug. Vorschlag: Z. 40 auf „Musik – Tick = ⌊(Time − T₀)/4000h⌋ mit T₀ = Setzen des
PIT-Teilers“ kürzen und den Phasensatz aus Z. 43 als einzige Phasenregel stehen lassen.

**R9T-2 (P3) – Nullpunkt der beiden Ticks nicht gleich, Einheit schon.** Effekte zählen Aufrufe von `0000:0732` ab 1, Musik
zählt ab T₀. Beide sind 72,82 Hz (Z. 42), der Nullpunkt unterscheidet sich. Unkritisch, weil der Vergleich je Tonbefehl
relativ zum Befehlstick läuft (Z. 40/95). Vorschlag: einen Satz „Nullpunkte verschieden, Vergleich nur relativ“ ergänzen.

**R9T-3 (P3) – Herkunft des Zeitgeberzählers offen.** Z. 43 verlangt „Phase aus dem Zeitgeberzähler von `T.EXE`“, sagt aber
nicht, wie der Host den Zählerstand liest (Speicherstelle, Zeitpunkt, Zuordnung zu Spur 0). Vorschlag: Adresse oder
Verfahrensschritt nennen, sofern bekannt; sonst als offen markieren.

**Signaturen (R9M-2):** Die Regel „Kopffelder und Prüfsumme statt Bytes“ (Z. 49–50) entspricht `AGENTS.md` §2.1: keine
Originalbytes im Code, Adressen und Prüfsummen sind zulässig. Kein Befund.

**Einheit (R9M-1):** Z. 42–44 setzen für Effekte und Musik dieselbe Einheit (Zeitgebertick 72,82 Hz) und legen fest, dass der
Musiktick vom Aufrufer kommt und nur jeder vierte Tick gilt. Das ist mit R9PL-2 (Musik ⌊(Time − T₀)/4000h⌋ ≈ 72,82 Hz) und mit
dem Review-Befund (Abstand genau 4,000) vereinbar. Kein Widerspruch zu R9PL-3 (Z. 47ff.) oder zu den Spurgrenzen. Kein Befund.

**Nachzug (kein Befund, Spuraufgabe):** `OplWrite.cs`, `FXDRIVER.md` §6 und `TON.md` §2 sind im Head noch nicht angepasst; der
Review R9M-1 hatte das verlangt.

## 3. Ausgeführte Prüfungen

- `git ls-remote` auf Base und Head: Base `9c04b1e`, Head `bd7190a` wie in der Vorgabe.
- Diff gelesen (1 Datei, nur Plantext). Querverweise geprüft: R9PL-2, R9PL-3, `AGENTS.md` §2.1, Messtabelle R9M-1.
- Gegenbeispiele: keine vorgegeben. Test- und Originalprüfung: nicht zutreffend (reine Doku).

**Akzeptierte `[APPROX]`:** keine. **Annahmen (nicht geprüft):** 72,82 Hz für den Effektaufruf von `0000:0732` stammt aus R9PL-2
und dem Review, ist hier nicht neu gemessen.
