# Traders.Audio (Arbeitspaket AP4)
Musikformat „A.H.“, Nachbildung von FXDRIVER (int 64h) und OPL2-Emulation.

## `Opl/` – OPL2-Synthese (R9, Spur `klang`)

C#-Übertragung der YM3812-Teile von [ymfm](https://github.com/aaronsgiles/ymfm) (BSD 3-Clause, `THIRD_PARTY.md`; F1 = B).
Kein Original des Spiels, daher ohne Originalmarker.

| Typ | Aufgabe |
|---|---|
| `Opl2` | Chip: `WriteRegister`/`WritePort`, `Generate` mit 3 579 545 / 72 ≈ 49 716 Hz, 16 Bit mono, DAC-Rundung (YM3014). Nicht nachgebildet: Zeitgeber, Statusregister, CSM. |
| `OplStream` | `Opl2` auf Ausgaberate (44,1/48 kHz), lineare Interpolation mit exaktem ganzzahligem Schritt (ohne Drift). |
| `OplWrite`, `OplClock` | Schnittstelle der Tonspuren: `(Tick, Register, Wert)`; Tickrate als exakter Bruch (`Pit`, `TimerTick`). |
| `OplRenderer` | Folge von `OplWrite` → PCM; Lesen des Textformats von `OplTrace.WriteText`; WAV schreiben. |

Deterministisch: gleiche Schreibfolge an gleichen Samplepositionen → bitgleiche Ausgabe. Tests:
`tests/Traders.Tests/Audio/Opl*` (Erwartungen aus dem YM3812-Datenblatt; `OplTraceTests` mit echter Menü-Folge aus dem
RefEmu, WAV mit `OPL_WAV_OUT=<verzeichnis>` – nie ins Repo).

**Host** (`src/Traders.App/Audio/`): `HostAudio` (MonoGame `DynamicSoundEffectInstance`, 44,1 kHz, drei Puffer à 1/60 s
vorgehalten, ohne Tongerät stumm weiter) mit `OplScheduler` (Schreibungen mit Host-Tick → Samplestelle, Neuverankerung bei
Rückstand). Einbindung in `HostGame` durch Spur 0: je Update `Update()`, Schreibungen mit `Write(HostClock.Ticks, reg, wert)`.

**Hörprobe:** `traders-tools opl-wav <opl-trace.txt[.gz]> <out.wav> [rate]` rendert eine `OplTrace`-Textdatei
(z. B. `opl.txt.gz` aus `GameRunTests` mit `REFEMU_GAME_OUT`, oder `opl-menu.txt` aus `OplTraceTests` mit `OPL_WAV_OUT`).
