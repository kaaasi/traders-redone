namespace Traders.Audio.Opl;

/// <summary>
/// One OPL2 register write at a tick: the interface between the sound lanes (effect machine, music player) and the
/// synthesis, comparable with the RefEmu's <c>OplTrace</c>. The unit of <see cref="Tick"/> is chosen by the producer and
/// passed to the consumer as a clock rate (see <see cref="OplClock"/>).
/// </summary>
public readonly record struct OplWrite(long Tick, byte Register, byte Value);

/// <summary>
/// Rate of a tick unit as an exact fraction <c>Numerator / Denominator</c> ticks per second, so tick times can be mapped
/// to sample positions without rounding drift.
/// </summary>
public readonly record struct OplClock(long Numerator, long Denominator)
{
    /// <summary>PIT input clock (1 193 182 Hz) – the unit of <c>OplTrace.Write.Time</c>.</summary>
    public static OplClock Pit => new(1_193_182, 1);

    /// <summary>Timer tick of the game (PIT divisor 4000h, about 72.82 Hz) – the effect machine's tick.</summary>
    public static OplClock TimerTick => new(1_193_182, 0x4000);

    /// <summary>Ticks per second as a floating-point value (for display only).</summary>
    public double Hz => (double)Numerator / Denominator;

    /// <summary>Index of the first output sample at or after <paramref name="tick"/> at <paramref name="sampleRate"/>.</summary>
    public long SampleAt(long tick, int sampleRate)
    {
        if (Numerator <= 0 || Denominator <= 0) throw new InvalidOperationException("clock rate must be positive");
        // ceil(tick · sampleRate · Denominator / Numerator), in 128-bit to stay exact for long traces
        Int128 n = (Int128)tick * sampleRate * Denominator;
        Int128 q = n / Numerator;
        if (q * Numerator < n) q++;
        return (long)q;
    }
}
