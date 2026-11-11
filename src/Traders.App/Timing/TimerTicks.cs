namespace Traders.App.Timing;

/// <summary>
/// Bookkeeping of the original timer interrupt per tick: callback first, then both 16-bit tick counters; every tick on
/// which the second counter becomes a multiple of four is passed on to the previous handler (BIOS clock, 18.2 Hz).
/// </summary>
public sealed class TimerTicks
{
    /// <summary>DGROUP offsets of the counters.</summary>
    public const ushort CounterAOffset = 0x117E, CounterBOffset = 0x1180;

    /// <summary>DS:117E (not read anywhere else in the original).</summary>
    public ushort CounterA { get; set; }

    /// <summary>DS:1180, decides when the previous handler is called.</summary>
    public ushort CounterB { get; set; }

    /// <summary>Ticks passed on to the previous handler.</summary>
    public long Chained { get; private set; }

    /// <summary>Called first on every tick (far pointer CS:0416 in the original, set by 27C6:041B).</summary>
    public Action? Callback { get; set; }

    /// <summary>One timer tick. Returns true if it is passed on to the previous handler.</summary>
    // [ORIGINAL] 27C6:03E4
    public bool Tick()
    {
        Callback?.Invoke();
        CounterA++;
        CounterB++;
        if ((CounterB & 3) != 0) return false;
        Chained++;
        return true;
    }

    /// <summary>Restart as 27C6:037A does when the timer is switched on.</summary>
    public void Reset()
    {
        CounterA = 0;
        CounterB = 0;
    }
}
