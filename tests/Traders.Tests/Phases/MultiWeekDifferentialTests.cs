using Traders.Core;
using Traders.Core.Phases;
using Traders.Core.Rules;
using Traders.Core.State;
using Traders.RefEmu;

namespace Traders.Tests.Phases;

/// <summary>
/// Multi-round probe (AGENTS.md §5) of the week loop with the ported rule steps (<see cref="WeekRuleSteps"/>): the default
/// game with four computer players runs in the RefEmu, the core's phase machine runs in lockstep. At every bound rule step the
/// core takes the RefEmu data segment at the call site, runs its own port of the routine and is compared with the RefEmu at the
/// return (whole data segment including DS:10D2); the screen steps are carried out by the RefEmu. Also compared: order of the
/// 30 calls and the markers, and the week counter DS:4B9A. Expected values come only from the RefEmu.
/// </summary>
public class MultiWeekDifferentialTests
{
    private const int Weeks = 4;

    private static string Dir => TestGameData.DosDir;

    /// <summary>
    /// [DIFFERENTIAL] 1777:01F8 with 1777:02DC, 0825:07C4, 0609:1178, 21FD:021F, 22A3:0003, 22A3:026B, 08CF:11D7 – weeks 0..3
    /// and call 1 of week 4: every bound rule step is the port, each compared at its return; nothing differs, every rule step
    /// ran in every week, and the random rules drew in the core exactly as the original.
    /// </summary>
    [GameDataFact]
    public void Four_weeks_with_ported_rule_steps_match_the_original() => RunWeeks();

    /// <summary>
    /// [DIFFERENTIAL] 1777:01F8 with 22A3:0003 – the same probe, but at the call site of 22A3:0003 in every week the players
    /// own production fields of mixed kinds by a set state in the RefEmu (<see cref="MixFieldKinds"/>), which the core takes
    /// over with the data segment. The default game gives each player one kind only (share 100, factor 100), where the
    /// truncation in share * 3 / 4 and the constant 25 of the yield factor do not show. A set state before week 0 does not
    /// last: call 8 (screen step, computer players) rebuilds the grid in week 0 and regroups the kinds in later weeks.
    /// Checked: in every week a field produces while some share DS:342C lies between 0 and 100 and is not a multiple of 4
    /// (the factor path is reached); the comparison with the RefEmu at the return of 22A3:0003 covers the grid DS:48F4.
    /// Counter-checks (local): factor rounded up ((share * 3 + 3) / 4) → red; factor + 26 instead of + 25 → red.
    /// </summary>
    [GameDataFact]
    public void Four_weeks_with_mixed_field_kinds_match_the_original()
    {
        // per production call: does a producing field use a share between 0 and 100 that is not a multiple of 4?
        var mixed = new List<bool>();
        RunWeeks(atProduction: MixFieldKinds, afterProduction: d => mixed.Add(d.S16(DgroupLayout.ProductionActive) > 0
            && Enumerable.Range(0, DgroupLayout.PlayerCount * 3).Select(i => d.U8(DgroupLayout.ProductionShares + i))
                .Any(share => share is > 0 and < 100 && share % 4 != 0)));
        Assert.Equal(Enumerable.Repeat(true, Weeks), mixed);
    }

    /// <summary>
    /// Mixed production fields by a set state: first every player p gets free fields (owner -1) until he owns at least 3 + p
    /// production fields, then the production fields (kinds 0..2) of every player get the kinds 0, 1, 2, 0, … in grid order
    /// (columns outside, rows inside), and the counts DS:2EE7 for kinds 0..2 are set to match. Land quality, rocket fields and
    /// headquarters stay as they are. The default game has only a few fields per player in the first weeks.
    /// </summary>
    private static void MixFieldKinds(OriginalProgram p)
    {
        var d = new Dgroup(p.DataSegment().AsSpan(0, DgroupLayout.Size).ToArray());
        var fields = new List<int>();
        for (int x = 0; x < d.S16(DgroupLayout.FieldColumns); x++)
            for (int y = 0; y < d.S16(DgroupLayout.FieldRows); y++) fields.Add(DgroupLayout.FieldIndex(x, y));
        bool Production(int f) => d.S8(DgroupLayout.FieldOwner + f) is >= 0 and < DgroupLayout.PlayerCount
            && d.S8(DgroupLayout.FieldKind + f) is >= 0 and <= 2;

        var owned = new int[DgroupLayout.PlayerCount];
        foreach (int f in fields.Where(Production)) owned[d.S8(DgroupLayout.FieldOwner + f)]++;
        foreach (int f in fields.Where(f => d.S8(DgroupLayout.FieldOwner + f) == -1))
        {
            int player = Enumerable.Range(0, DgroupLayout.PlayerCount).FirstOrDefault(q => owned[q] < 3 + q, -1);
            if (player < 0) break;
            d.Set(DgroupLayout.FieldOwner + f, (sbyte)player);
            d.Set(DgroupLayout.FieldKind + f, (sbyte)0);
            owned[player]++;
        }

        var counts = new int[DgroupLayout.PlayerCount * 3];
        foreach (int f in fields.Where(Production))
        {
            int owner = d.S8(DgroupLayout.FieldOwner + f);
            int n = counts[3 * owner] + counts[3 * owner + 1] + counts[3 * owner + 2];
            d.Set(DgroupLayout.FieldKind + f, (sbyte)(n % 3));
            counts[3 * owner + n % 3]++;
        }
        for (int player = 0; player < DgroupLayout.PlayerCount; player++)
            for (int kind = 0; kind < 3; kind++)
                d.Set(DgroupLayout.FieldsByKind + 4 * player + kind, (sbyte)counts[3 * player + kind]);
        foreach (int f in fields)
        {
            p.WriteByte((ushort)(DgroupLayout.FieldOwner + f), d.U8(DgroupLayout.FieldOwner + f));
            p.WriteByte((ushort)(DgroupLayout.FieldKind + f), d.U8(DgroupLayout.FieldKind + f));
        }
        for (int i = 0; i < 4 * DgroupLayout.PlayerCount; i++)
            p.WriteByte((ushort)(DgroupLayout.FieldsByKind + i), d.U8(DgroupLayout.FieldsByKind + i));
    }

    private static void RunWeeks(Action<OriginalProgram>? atProduction = null, Action<Dgroup>? afterProduction = null)
    {
        var p = OriginalProgram.StartGame(Dir);
        Assert.True(p.RunToWeekStart(0, 5), "week 0 did not start within 5 s after the menu");
        var core = new Dgroup(p.DataSegment().AsSpan(0, DgroupLayout.Size).ToArray());
        var oracle = new RefEmuWeekOracle(p);

        // Call 1 of week 0: the RefEmu stands at the entry of 1777:02DC (same data segment as at its call site).
        WeeklyPrices.Apply(core);
        Assert.True(oracle.RunToNextStop() && oracle.IsAtLoop((ushort)(WeekLoop.Step(1).Site + 5)), "return of call 1");
        oracle.Compare(core, "week 0 after call 1 (port)");
        p.DataSegment().AsSpan(0, DgroupLayout.Size).CopyTo(core.Bytes);
        oracle.ResetPassed();

        var handlers = WeekLoop.Steps.Where(s => s.Kind == WeekStepKind.Call).ToDictionary(
            s => s.Number,
            s => WeekRuleSteps.Rules.ContainsKey(s.Number)
                ? s.Number == 13
                    ? oracle.PortHandler(core, After.Wrap(WeekRuleSteps.Handler(s.Number), afterProduction), atProduction)
                    : oracle.PortHandler(core, WeekRuleSteps.Handler(s.Number))
                : oracle.Handler(core));
        var random = new OriginalRandom(new RecordedClock([]));
        var machine = new PhaseMachine(core, random, handlers, nextStep: 2);
        var events = new List<PhaseEvent>();
        for (int f = 0; f < 40 * Weeks && !(machine.Week == Weeks && machine.NextStep == 2); f++)
        {
            machine.Advance(new PhaseTick(machine.LastFrame + 1));
            events.AddRange(machine.DrainEvents());
        }

        Assert.Empty(oracle.Differences);
        Assert.Equal(PhaseStatus.Ready, machine.Status);
        Assert.Equal(Weeks, machine.Week);
        Assert.Equal(Weeks, p.Week);

        // order of calls and markers: weeks 0..3 complete, then call 1 of week 4; the RefEmu took the same calls
        var expected = Enumerable.Range(2, 29).Concat(Enumerable.Range(0, Weeks - 1).SelectMany(_ => Enumerable.Range(1, 30))).Append(1).ToList();
        Assert.Equal(expected, events.Where(e => e.Kind is PhaseEventKind.StepStarted or PhaseEventKind.MarkerPassed).Select(e => e.Step));
        Assert.Equal(expected.Append(2), oracle.Passed.Select(site => WeekLoop.Steps.FirstOrDefault(s => s.Site == site)?.Number).OfType<int>());
        Assert.Equal(Enumerable.Range(0, Weeks).SelectMany(_ => Enumerable.Range(0, 7)),
            events.Where(e => e.Kind == PhaseEventKind.MarkerPassed).Select(e => e.Value));
        Assert.Equal(Enumerable.Range(1, Weeks), events.Where(e => e.Kind == PhaseEventKind.WeekAdvanced).Select(e => e.Value));

        // every bound rule step ran as port in every week (call 1 of week 0 above, of weeks 1..4 here)
        var ruleSteps = WeekRuleSteps.Rules.Keys.Order().ToList();
        for (int week = 0; week < Weeks; week++)
        {
            var expectedSteps = week == 0 ? ruleSteps.Where(n => n != 1) : ruleSteps;
            Assert.Equal(expectedSteps, oracle.PortedSteps.Where(x => x.Week == week).Select(x => x.Step));
        }
        Assert.Equal([1], oracle.PortedSteps.Where(x => x.Week == Weeks).Select(x => x.Step));
        // weather draws random(2) and maybe random(4); the gem price draws at least once in these weeks
        Assert.All(oracle.PortedSteps.Where(x => x.Step == 12), x => Assert.InRange(x.Draws, 1, 2));
        Assert.All(oracle.PortedSteps.Where(x => x.Step is not (12 or 27)), x => Assert.Equal(0, x.Draws));
        Assert.Contains(oracle.PortedSteps, x => x.Step == 27 && x.Draws > 0);
    }

    /// <summary>Runs <paramref name="inspect"/> on the data segment after the step of <paramref name="inner"/> (test probe only).</summary>
    private sealed class After(IWeekStepHandler inner, Action<Dgroup> inspect) : IWeekStepHandler
    {
        public static IWeekStepHandler Wrap(IWeekStepHandler inner, Action<Dgroup>? inspect) =>
            inspect == null ? inner : new After(inner, inspect);

        public StepOutcome Begin(StepContext context)
        {
            var outcome = inner.Begin(context);
            inspect(context.State);
            return outcome;
        }

        public StepOutcome Tick(StepContext context) => inner.Tick(context);
    }
}
