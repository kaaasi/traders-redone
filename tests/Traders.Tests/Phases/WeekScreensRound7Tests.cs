using Traders.Core;
using Traders.Core.Extra;
using Traders.Core.Phases;
using Traders.Core.Pixels;
using Traders.Core.Trade;

namespace Traders.Tests.Phases;

/// <summary>
/// Binding of round 7 (lane ende part 3): <see cref="WeekScreens.Week"/> binds the trade (call 23, <see cref="TradeStep"/>)
/// and the extra phase (call 24, <see cref="ExtraStep"/>), made anew at every call; the core alone (<see cref="WeekSteps"/>
/// without handlers) keeps them pending, and call 18 stays pending without the host's switch.
/// </summary>
public class WeekScreensRound7Tests
{
    private static WeekScreens Screens() => new(
        new WeekScreenInputs(_ => default, _ => new IndexedImage(1, 1, new byte[1]), [], () => default!, () => null),
        new WeekScreenOutputs(_ => { }, _ => { }, _ => { }, _ => { }));

    [Fact]
    public void The_week_binds_calls_23_and_24_and_leaves_18_and_26_pending()
    {
        var screens = Screens();
        var week = screens.Week(_ => { }, _ => default);
        Assert.Same(screens.Trade, week.Handlers[WeekSteps.TradeCall]);
        Assert.Same(screens.Extra, week.Handlers[WeekSteps.ExtraCall]);
        Assert.False(week.Handlers.ContainsKey(WeekSteps.RobotCall));
        Assert.False(week.Handlers.ContainsKey(WeekSteps.FatMikeCall));
        Assert.Equal([WeekSteps.TradeCall, WeekSteps.ExtraCall], WeekSteps.Round7Calls);

        var core = new WeekSteps(_ => { }, _ => default);
        Assert.False(core.Handlers.ContainsKey(WeekSteps.TradeCall));
        Assert.False(core.Handlers.ContainsKey(WeekSteps.ExtraCall));
    }
}
