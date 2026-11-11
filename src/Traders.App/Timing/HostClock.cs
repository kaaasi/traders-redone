namespace Traders.App.Timing;

/// <summary>
/// Turns host time into the two clocks of the original: timer ticks of the reprogrammed PIT (sound callback and tick
/// counters, 27C6:03E4) and display frames (vertical retrace, the pace of the game in 0D73:00D3). Time is counted in
/// PIT input clocks without rounding drift, so the number of ticks and frames depends only on the total time passed,
/// not on how it is split into host updates.
/// </summary>
public sealed class HostClock
{
    /// <summary>PIT input clock in Hz.</summary>
    public const long PitHz = 1_193_182;

    /// <summary>Divisor of PIT channel 0 set by 27C6:037A (out 40h: 00h, 40h) – about 72.82 Hz.</summary>
    // [ORIGINAL] 27C6:037A
    public const int TimerDivisor = 0x4000;

    /// <summary>Frame period in PIT clocks.</summary>
    // [APPROX] 70 Hz as in the RefEmu (Pit.ClockHz / 70); a real VGA in the 320×200 modes runs at
    // 25.175 MHz / (800 · 449) ≈ 70.086 Hz.
    public const long FramePeriod = PitHz / 70;

    private const long TicksPerSecond = TimeSpan.TicksPerSecond;

    private long _clocks;
    private long _remainder; // host ticks · PitHz not yet converted into whole clocks

    /// <summary>PIT clocks passed since start.</summary>
    public long Clocks => _clocks;

    /// <summary>Timer ticks passed since start.</summary>
    public long Ticks => _clocks / TimerDivisor;

    /// <summary>Frames passed since start.</summary>
    public long Frames => _clocks / FramePeriod;

    public static double TimerHz => (double)PitHz / TimerDivisor;

    /// <summary>Advances by <paramref name="elapsed"/> and returns how many ticks and frames were completed.</summary>
    public (int Ticks, int Frames) Advance(TimeSpan elapsed)
    {
        if (elapsed < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(elapsed));
        long ticks0 = Ticks, frames0 = Frames;
        long scaled = checked(elapsed.Ticks * PitHz + _remainder);
        _clocks += scaled / TicksPerSecond;
        _remainder = scaled % TicksPerSecond;
        return ((int)(Ticks - ticks0), (int)(Frames - frames0));
    }
}
