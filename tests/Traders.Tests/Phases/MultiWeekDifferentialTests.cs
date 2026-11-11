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
    [Trait(Long.Trait, Long.Value)]
    public void Four_weeks_with_ported_rule_steps_match_the_original()
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
                ? oracle.PortHandler(core, WeekRuleSteps.Handler(s.Number))
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
}
