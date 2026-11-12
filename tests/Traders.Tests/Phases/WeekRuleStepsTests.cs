using Traders.Core;
using Traders.Core.Phases;
using Traders.Core.Rules;
using Traders.Core.State;

namespace Traders.Tests.Phases;

/// <summary>
/// Binding of the screenless rule steps without original data: which call steps are bound (against the plan list in
/// docs/arbeitspakete.md R3 lane 0 part 3 and the step table), that they finish at once inside the phase machine, run the
/// same rule as a direct call, and take their random state from DS:10D2 and store it back.
/// </summary>
public class WeekRuleStepsTests
{
    /// <summary>Plan steps of BESTAND.md §1.4 and their targets, as listed in the plan (not read from WeekLoop).</summary>
    private static readonly (int PlanStep, ushort Segment, ushort Offset)[] Planned =
    [
        (1, 0x1777, 0x02DC), (2, 0x0825, 0x07C4), (5, 0x0609, 0x1178), (10, 0x21FD, 0x021F), (11, 0x22A3, 0x0003),
        (12, 0x22A3, 0x026B), (16, 0x0609, 0x1178), (21, 0x08CF, 0x11D7),
    ];

    [Fact]
    public void Bound_steps_are_the_planned_screenless_rules()
    {
        var bound = WeekRuleSteps.Rules.Keys.Order().Select(WeekLoop.Step).ToList();
        Assert.Equal(Planned, bound.Select(s => (s.PlanStep!.Value, s.TargetSegment, s.TargetOffset)));
        Assert.All(bound, s => Assert.Equal((WeekStepKind.Call, false, WeekExitCheck.None), (s.Kind, s.Screen, s.ExitAfter)));
        Assert.Throws<ArgumentOutOfRangeException>(() => WeekRuleSteps.Handler(3));
    }

    /// <summary>
    /// Input state for the rules without original data: an empty data segment (no fields on the grid) with random cash, stocks,
    /// prices, week and seed in DS:10D2.
    /// </summary>
    private static Dgroup Arbitrary(int seed)
    {
        var rnd = new Random(seed);
        var d = new Dgroup(new byte[DgroupLayout.Size]);
        for (int p = 0; p < 4; p++)
        {
            d.Set(DgroupLayout.Cash + 4 * p, rnd.Next(0, 100_000));
            d.Set(DgroupLayout.FieldCount + p, (byte)rnd.Next(0, 30));
            d.Set(DgroupLayout.WarePrice + 2 * p, (short)rnd.Next(3, 300));
            for (int w = 0; w < 4; w++) d.Set(DgroupLayout.Stock + 8 * p + 2 * w, (short)rnd.Next(0, 1000));
        }
        d.Set(DgroupLayout.Week, (short)rnd.Next(0, 200));
        d.Set(DgroupLayout.RandomSeed, (uint)rnd.NextInt64(0, 1L << 32));
        return d;
    }

    /// <summary>
    /// Each bound step started by the phase machine (alone, at its call number) changes the data segment exactly as the
    /// rule called directly with a generator seeded from DS:10D2; the machine writes the new seed back to DS:10D2.
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(12)]
    [InlineData(13)]
    [InlineData(14)]
    [InlineData(20)]
    [InlineData(27)]
    public void Machine_runs_the_rule_with_the_seed_from_DS(int number)
    {
        for (int i = 0; i < 20; i++)
        {
            var viaMachine = Arbitrary(1000 * number + i);
            var direct = new Dgroup((byte[])viaMachine.Bytes.Clone());
            var expectedRandom = new OriginalRandom(new RecordedClock([]), direct.U32(DgroupLayout.RandomSeed));
            WeekRuleSteps.Rules[number](direct, expectedRandom);
            direct.Set(DgroupLayout.RandomSeed, expectedRandom.Seed);

            // the machine's generator starts from a different seed: only DS:10D2 may count
            var m = new PhaseMachine(viaMachine, new OriginalRandom(new RecordedClock([]), 0xDEADBEEF),
                new Dictionary<int, IWeekStepHandler> { [number] = WeekRuleSteps.Handler(number) }, nextStep: number);
            m.Advance(new PhaseTick(0));

            Assert.Equal(direct.Bytes, viaMachine.Bytes);
            var finished = Assert.Single(m.Events, e => e.Kind == PhaseEventKind.StepFinished);
            Assert.Equal(number, finished.Step);
            Assert.Equal(PhaseStatus.Blocked, m.Status); // the next call step has no handler
        }
    }

    /// <summary>The random rules draw (weather always), the others never; their draws count for the frame (F3 = A).</summary>
    [Fact]
    public void Only_weather_and_gem_price_draw()
    {
        foreach (int number in WeekRuleSteps.Rules.Keys)
        {
            var d = Arbitrary(number);
            var random = new OriginalRandom(new RecordedClock([]), 1);
            var m = new PhaseMachine(d, random, new Dictionary<int, IWeekStepHandler> { [number] = WeekRuleSteps.Handler(number) },
                nextStep: number);
            m.Advance(new PhaseTick(0));
            int draws = Assert.Single(m.Events, e => e.Kind == PhaseEventKind.Frame).Value;
            if (number == 12) Assert.InRange(draws, 1, 2);
            else if (number != 27) Assert.Equal(0, draws);
        }
    }

    /// <summary>Rule handlers bound together with screen handlers: a week of rule steps and instant screens passes in order.</summary>
    [Fact]
    public void Rule_steps_with_instant_screens_run_a_whole_week()
    {
        var handlers = WeekRuleSteps.Handlers();
        var screens = new List<int>();
        foreach (var s in WeekLoop.Steps.Where(s => s.Kind == WeekStepKind.Call && !handlers.ContainsKey(s.Number)))
            handlers[s.Number] = new Recording(screens);
        var d = Arbitrary(7);
        d.Set(WeekLoop.AbortKeyState, (byte)0);
        d.Set(DgroupLayout.Week, (short)5);
        var m = new PhaseMachine(d, new OriginalRandom(new RecordedClock([])), handlers);
        m.Advance(new PhaseTick(0));
        Assert.Equal(6, m.Week);
        Assert.Equal(WeekLoop.Steps.Where(s => s.Kind == WeekStepKind.Call && !WeekRuleSteps.Rules.ContainsKey(s.Number)).Select(s => s.Number), screens);
        Assert.Equal(WeekRuleSteps.Rules.Keys.Order(),
            m.Events.Where(e => e.Kind == PhaseEventKind.StepFinished && WeekRuleSteps.Rules.ContainsKey(e.Step)).Select(e => e.Step));
    }

    private sealed class Recording(List<int> calls) : IWeekStepHandler
    {
        public StepOutcome Begin(StepContext context)
        {
            calls.Add(context.Step.Number);
            return StepOutcome.Finished();
        }

        public StepOutcome Tick(StepContext context) => throw new InvalidOperationException();
    }
}
