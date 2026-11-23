namespace Traders.Audio.Opl;

/// <summary>
/// One OPL2 register write with the tick it belongs to: the common interface of the sound spurs (R9, docs/arbeitspakete.md
/// "Ton im Kern nur als Befehl"). The tick is a timer tick of the game (72,82 Hz) for effects and music alike. Effects: the caller
/// numbers the calls of 0000:0732. Music: the host passes the tick to <c>FxMusicDriver.Tick</c>, which it calls on every fourth
/// timer tick (phase set by the host); writes outside a tick (driver functions 00h, 01h, 03h, 05h, 06h) carry the tick the
/// host set last.
/// </summary>
public readonly record struct OplWrite(long Tick, byte Register, byte Value)
{
    public override string ToString() => $"{Tick} {Register:X2} {Value:X2}";
}
