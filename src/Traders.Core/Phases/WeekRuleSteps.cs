using Traders.Core.Rules;
using Traders.Core.State;

namespace Traders.Core.Phases;

/// <summary>
/// The screenless rule steps of the week loop (docs/analyse/BESTAND.md §1.4) bound to the ported weekly rules of
/// <c>Traders.Core.Rules</c>. Each handler runs its rule on the data segment in <see cref="IWeekStepHandler.Begin"/> and
/// returns at once; random rules draw from <see cref="StepContext.Random"/>, which the phase machine seeds from DS:10D2
/// before the call and stores back afterwards (the rule itself never reads DS:10D2). All other call steps stay pending.
/// </summary>
public static class WeekRuleSteps
{
    /// <summary>Rule of each bound call step, by call number 1..30 of <see cref="WeekLoop"/>.</summary>
    public static IReadOnlyDictionary<int, Action<Dgroup, OriginalRandom>> Rules { get; } = new Dictionary<int, Action<Dgroup, OriginalRandom>>
    {
        [1] = (d, _) => WeeklyPrices.Apply(d),          // plan step 1, 1777:02DC
        [2] = (d, _) => WealthHistoryRecord.Apply(d),   // plan step 2, 0825:07C4
        [5] = (d, _) => ProductionLimits.Apply(d),      // plan step 5, 0609:1178
        [12] = WeatherRule.Apply,                       // plan step 10, 21FD:021F
        [13] = (d, _) => Production.Apply(d),           // plan step 11, 22A3:0003
        [14] = (d, _) => StockDecay.Apply(d),           // plan step 12, 22A3:026B
        [20] = (d, _) => ProductionLimits.Apply(d),     // plan step 16, 0609:1178
        [27] = GemPrice.Apply,                          // plan step 21, 08CF:11D7
    };

    /// <summary>Handler that runs the rule of call step <paramref name="number"/> (must be one of <see cref="Rules"/>).</summary>
    public static IWeekStepHandler Handler(int number) =>
        Rules.TryGetValue(number, out var rule)
            ? new RuleStep(rule)
            : throw new ArgumentOutOfRangeException(nameof(number), number, "no ported rule for this call step");

    /// <summary>Handlers for all bound rule steps, for <see cref="PhaseMachine"/>; combine with handlers of the screen steps.</summary>
    public static Dictionary<int, IWeekStepHandler> Handlers() => Rules.Keys.ToDictionary(n => n, Handler);

    private sealed class RuleStep(Action<Dgroup, OriginalRandom> rule) : IWeekStepHandler
    {
        public StepOutcome Begin(StepContext context)
        {
            rule(context.State, context.Random);
            return StepOutcome.Finished();
        }

        public StepOutcome Tick(StepContext context) => throw new InvalidOperationException("rule steps finish at once");
    }
}
