using Traders.Core.State;

namespace Traders.Core.Rules;

/// <summary>
/// Countdown 21FD:04E5 / 21FD:04F9 (finding docs/routines/21FD_04E5_countdown.md): a value DS:3780 that drops by one every
/// <c>period</c> calls of the tick. Used by the trade 0C29 (R7) and the gem trade 08CF (R8); neutral, no phase of its own.
/// </summary>
public static class Countdown
{
    /// <summary>word, remaining value (the trade ends at 0).</summary>
    public const int Remaining = 0x3780;

    /// <summary>word, ticks per step.</summary>
    public const int Period = 0x2F16;

    /// <summary>word, ticks left in the current step.</summary>
    public const int Ticks = 0x2F18;

    /// <summary>21FD:04E5(value, period) – starts the countdown.</summary>
    // [ORIGINAL] 21FD:04E5
    public static void Start(Dgroup d, short value, short period)
    {
        d.Set(Remaining, value);
        d.Set(Period, period);
        d.Set(Ticks, period);
    }

    /// <summary>
    /// 21FD:04F9 after its pause check 21FD:0245 (the caller runs that first, it can take frames): one tick; at the end of a
    /// step the value drops by one, the step restarts and the result is true (the original's AL = 1).
    /// </summary>
    // [ORIGINAL] 21FD:04F9
    public static bool Tick(Dgroup d)
    {
        short ticks = Int16Math.S(d.S16(Ticks) - 1);
        d.Set(Ticks, ticks);
        if (ticks != 0) return false;
        d.Set(Remaining, Int16Math.S(d.S16(Remaining) - 1));
        d.Set(Ticks, d.S16(Period));
        return true;
    }
}
