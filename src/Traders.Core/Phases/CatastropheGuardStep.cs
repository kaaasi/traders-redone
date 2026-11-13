using Traders.Core.State;

namespace Traders.Core.Phases;

/// <summary>
/// A step handler that covers its call only in some states; in the others the call is still pending and the phase machine
/// stops there (<see cref="PhaseStatus.Blocked"/>) instead of running the handler.
/// </summary>
public interface IPartialWeekStepHandler : IWeekStepHandler
{
    /// <summary>True if the call cannot be carried out in <paramref name="state"/> (not ported for it yet).</summary>
    bool IsPending(Dgroup state);
}

/// <summary>
/// Call 4 of the week loop, 111D:0009 (catastrophes and week events; finding docs/routines/111D_0009_katastrophen.md): only
/// its guard 111D:000D–001B is ported. In weeks DS:4B9A ≤ 2 (signed) or with catastrophes switched off (DS:2F30 = 0) the
/// routine returns at once without touching state or drawing a random number; that is this step. Otherwise the call is
/// pending (R5P-6): the machine stops, or <paramref name="rest"/> (for example the RefEmu oracle of a test) carries it out.
/// </summary>
/// <param name="rest">Handler for the states the guard does not cover; null = pending.</param>
public sealed class CatastropheGuardStep(IWeekStepHandler? rest = null) : IPartialWeekStepHandler
{
    /// <summary>Call number of 111D:0009 in <see cref="WeekLoop"/>.</summary>
    public const int Call = 4;

    /// <summary>Last week (DS:4B9A) without catastrophes: 111D:000D compares with 2 and returns if not greater.</summary>
    public const short LastQuietWeek = 2;

    private IWeekStepHandler? _running;

    /// <summary>[ORIGINAL] 111D:000D – the guard: true if 111D:0009 returns at once (week ≤ 2 or DS:2F30 = 0).</summary>
    // [ORIGINAL] 111D:0009 – guard only (111D:000D–001B, return 111D:010F); the rest is pending (R5P-6)
    public static bool ReturnsAtOnce(Dgroup state) =>
        state.S16(DgroupLayout.Week) <= LastQuietWeek || state.U8(DgroupLayout.Catastrophes) == 0;

    public bool IsPending(Dgroup state) => rest is null && !ReturnsAtOnce(state);

    public StepOutcome Begin(StepContext context)
    {
        _running = null;
        if (ReturnsAtOnce(context.State)) return StepOutcome.Finished();
        _running = rest ?? throw new InvalidOperationException("111D:0009 beyond its guard is not ported");
        return _running.Begin(context);
    }

    public StepOutcome Tick(StepContext context) =>
        (_running ?? throw new InvalidOperationException("the guard of 111D:0009 finishes at once")).Tick(context);
}
