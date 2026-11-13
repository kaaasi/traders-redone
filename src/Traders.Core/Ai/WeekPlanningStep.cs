using Traders.Core.Phases;

namespace Traders.Core.Ai;

/// <summary>
/// Handler of the week loop calls 6 (1777:0227, pass 0 before planting) and 21 (1777:0291, pass 1 before trading): runs
/// <see cref="WeekPlanning.Apply"/> on the data segment and returns at once, without a screen. Bound into the phase machine
/// by the track <c>anbau</c> (docs/arbeitspakete.md R5, part 3).
/// </summary>
public sealed class WeekPlanningStep(byte pass) : IWeekStepHandler
{
    /// <summary>Call number of the week loop for pass 0.</summary>
    public const int BeforePlantingCall = 6;

    /// <summary>Call number of the week loop for pass 1.</summary>
    public const int BeforeTradeCall = 21;

    public byte Pass { get; } = pass;

    /// <summary>Handler for call <paramref name="number"/> (6 or 21).</summary>
    public static WeekPlanningStep ForCall(int number) => number switch
    {
        BeforePlantingCall => new WeekPlanningStep(0),
        BeforeTradeCall => new WeekPlanningStep(1),
        _ => throw new ArgumentOutOfRangeException(nameof(number), number, "11A3:0000 is called only as call 6 and 21"),
    };

    public StepOutcome Begin(StepContext context)
    {
        WeekPlanning.Apply(context.State, context.Random, Pass);
        return StepOutcome.Finished();
    }

    public StepOutcome Tick(StepContext context) => throw new InvalidOperationException("the week planning finishes at once");
}
