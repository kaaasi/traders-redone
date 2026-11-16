using Traders.App.Deviations;
using Traders.Core;
using Traders.Core.Ending;
using Traders.Core.Phases;
using Traders.Core.State;

namespace Traders.Tests.App;

/// <summary>
/// The host's robot switch (decision F3 = A of R7): the stand-in for call 18 changes no byte of the data segment, draws no
/// random number and returns in the frame it starts; without it the core's week steps keep call 18 pending.
/// </summary>
public class RobotPhaseSkipTests
{
    /// <summary>Data segment with every byte set (seeded pattern), random state DS:10D2 included.</summary>
    private static Dgroup Filled()
    {
        var d = new Dgroup();
        var rng = new Random(18);
        rng.NextBytes(d.Bytes);
        return d;
    }

    /// <summary>Counter-check: a stand-in that draws one random number or writes a byte → red.</summary>
    [Fact]
    public void The_stand_in_changes_nothing_and_ends_in_its_first_frame()
    {
        var d = Filled();
        var before = (byte[])d.Bytes.Clone();
        var skip = new RobotPhaseSkip();
        var random = new OriginalRandom(new RecordedClock([]));
        var machine = new PhaseMachine(d, random, new Dictionary<int, IWeekStepHandler> { [WeekSteps.RobotCall] = skip },
            nextStep: WeekSteps.RobotCall);
        machine.Advance(new PhaseTick(0));

        Assert.Equal(1, skip.Skipped);
        Assert.Equal(before, d.Bytes);
        Assert.Equal(0, random.Calls);
        var started = Assert.Single(machine.Events, e => e.Kind == PhaseEventKind.StepStarted);
        var finished = Assert.Single(machine.Events, e => e.Kind == PhaseEventKind.StepFinished);
        Assert.Equal((WeekSteps.RobotCall, 0L), (started.Step, started.Frame));
        Assert.Equal((WeekSteps.RobotCall, 0L), (finished.Step, finished.Frame));
        Assert.Equal(WeekSteps.EndCall, machine.NextStep); // no handler for 19 here: pending
        Assert.Equal(0, Assert.Single(machine.Events, e => e.Kind == PhaseEventKind.Frame).Value);
    }

    /// <summary>The core's week steps: call 18 pending without a handler, the end check always bound, call 23 pending.</summary>
    [Fact]
    public void Without_the_switch_the_core_keeps_call_18_pending()
    {
        var core = new WeekSteps(_ => { }, _ => default);
        Assert.False(core.Handlers.ContainsKey(WeekSteps.RobotCall));
        Assert.IsType<EndCheckStep>(core.Handlers[WeekSteps.EndCall]);
        Assert.False(core.Handlers.ContainsKey(WeekSteps.TradeCall));

        var d = new Dgroup();
        var machine = new PhaseMachine(d, new OriginalRandom(new RecordedClock([])), core.Handlers, nextStep: WeekSteps.RobotCall);
        machine.Advance(new PhaseTick(0));
        Assert.Equal(PhaseStatus.Blocked, machine.Status);
        Assert.Equal(WeekSteps.RobotCall, machine.NextStep);

        var skip = new WeekSteps(_ => { }, _ => default, robots: new RobotPhaseSkip());
        Assert.IsType<RobotPhaseSkip>(skip.Handlers[WeekSteps.RobotCall]);
    }

    /// <summary>
    /// With the switch the week runs on from call 18 through the end check (no end: target above every wealth) and calls 20
    /// and 21 to the trade, where it waits; without a winner nothing ends.
    /// </summary>
    [Fact]
    public void With_the_switch_the_week_runs_on_to_the_trade()
    {
        var steps = new WeekSteps(_ => { }, _ => default, robots: new RobotPhaseSkip());
        var d = new Dgroup();
        d.Set(DgroupLayout.Target, 10_000);
        var machine = new PhaseMachine(d, new OriginalRandom(new RecordedClock([])), steps.Handlers, nextStep: WeekSteps.RobotCall);
        machine.Advance(new PhaseTick(0));
        Assert.Equal(PhaseStatus.Blocked, machine.Status);
        Assert.Equal(WeekSteps.TradeCall, machine.NextStep);
        Assert.False(steps.End.GameOver);
        Assert.Equal([18, 19, 20, 21, 22], machine.Events.Where(e => e.Kind is PhaseEventKind.StepStarted or PhaseEventKind.MarkerPassed).Select(e => e.Step));
    }
}
