using Traders.Core.FatMike;
using Traders.Core.Purchase;
using Traders.Core.Phases;
using Traders.Core.Pixels;

namespace Traders.Tests.Phases;

/// <summary>
/// Binding of round 8 (lane woche part 1): <see cref="WeekScreens.Week"/> binds Fat Mike (call 26, <see cref="FatMikeStep"/>),
/// the purchase (call 29, <see cref="PurchaseStep"/>) and the contest (call 30, <see cref="ContestStep"/> with its second field
/// round), made anew at every call; the core alone (<see cref="WeekSteps"/> without handlers) and a week made with
/// <c>round8: false</c> keep them pending.
/// </summary>
public class WeekScreensRound8Tests
{
    private static WeekScreens Screens() => new(
        new WeekScreenInputs(_ => default, _ => new IndexedImage(1, 1, new byte[1]), [], () => default!, () => null),
        new WeekScreenOutputs(_ => { }, _ => { }, _ => { }, _ => { }));

    [Fact]
    public void The_week_binds_calls_26_29_and_30_and_leaves_18_pending()
    {
        var screens = Screens();
        var week = screens.Week(_ => { }, _ => default);
        Assert.Same(screens.FatMike, week.Handlers[WeekSteps.FatMikeCall]);
        Assert.Same(screens.Purchase, week.Handlers[WeekSteps.PurchaseCall]);
        Assert.Same(screens.Contest, week.Handlers[WeekSteps.ContestCall]);
        Assert.False(week.Handlers.ContainsKey(WeekSteps.RobotCall));
        Assert.Equal([WeekSteps.FatMikeCall, WeekSteps.PurchaseCall, WeekSteps.ContestCall], WeekSteps.Round8Calls);
        Assert.Equal([26, 29, 30], WeekSteps.Round8Calls.Select(n => WeekLoop.Step(n).Number));
        Assert.Equal((0x1F63, 0x000F), (WeekLoop.Step(WeekSteps.FatMikeCall).TargetSegment, WeekLoop.Step(WeekSteps.FatMikeCall).TargetOffset));
        Assert.Equal((0x08CF, 0x0004), (WeekLoop.Step(WeekSteps.PurchaseCall).TargetSegment, WeekLoop.Step(WeekSteps.PurchaseCall).TargetOffset));
        Assert.Equal((0x1F63, 0x0B03), (WeekLoop.Step(WeekSteps.ContestCall).TargetSegment, WeekLoop.Step(WeekSteps.ContestCall).TargetOffset));

        // with a stand-in for call 18 every call step of the week has a handler
        var all = screens.Week(_ => { }, _ => default, robots: new RobotPhaseSkipStub());
        Assert.All(WeekLoop.Steps.Where(s => s.Kind == WeekStepKind.Call), s => Assert.True(all.Handlers.ContainsKey(s.Number), $"call {s.Number}"));

        var before = screens.Week(_ => { }, _ => default, round8: false);
        Assert.All(WeekSteps.Round8Calls, n => Assert.False(before.Handlers.ContainsKey(n)));
        var core = new WeekSteps(_ => { }, _ => default);
        Assert.All(WeekSteps.Round8Calls, n => Assert.False(core.Handlers.ContainsKey(n)));
    }

    /// <summary>A handler standing in for call 18 (finishes at once).</summary>
    private sealed class RobotPhaseSkipStub : IWeekStepHandler
    {
        public StepOutcome Begin(StepContext context) => StepOutcome.Finished();

        public StepOutcome Tick(StepContext context) => throw new InvalidOperationException("finishes at once");
    }
}
