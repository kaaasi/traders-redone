namespace Traders.Audio.Opl;

/// <summary>
/// One OPL2 register write with the tick it belongs to: the common interface of the sound spurs (R9, docs/arbeitspakete.md
/// "Ton im Kern nur als Befehl"). For music the tick is the number of the driver's timer call (FXDRIVER CS:1262, counted from 1);
/// writes outside a tick (driver functions 00h, 01h, 03h, 05h, 06h) carry the tick of the last call before them.
/// </summary>
public readonly record struct OplWrite(long Tick, byte Register, byte Value)
{
    public override string ToString() => $"{Tick} {Register:X2} {Value:X2}";
}
