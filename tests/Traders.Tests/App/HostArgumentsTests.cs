using Traders.App;
using Traders.Core;
using Traders.Core.Phases;

namespace Traders.Tests.App;

/// <summary>
/// The host's command line (<see cref="HostArguments"/>, P3 #194 of review r7-ende-host): the robot switch is on unless
/// <c>--original-roboter</c> is given (decision F3 = A of R7), the menu switch unless <c>--original-ende</c> is given (decision
/// F3 = B of R9), and the program run made from the arguments follows them.
/// </summary>
public class HostArgumentsTests
{
    /// <summary>Counter-check: the option ignored (switch always on) or the default off → red.</summary>
    [Theory]
    [InlineData(new string[0], true)]
    [InlineData(new[] { "--original-roboter" }, false)]
    [InlineData(new[] { "dos", "--scale", "2", "--original-roboter", "--aspect" }, false)]
    [InlineData(new[] { "--scale", "2", "dos" }, true)]
    public void The_robot_switch_is_on_unless_original_robots_is_given(string[] args, bool skip)
    {
        var a = HostArguments.Parse(args);
        Assert.Equal(skip, a.SkipRobotPhase);
    }

    /// <summary>Counter-check: the option ignored (switch always on) or the default off → red.</summary>
    [Theory]
    [InlineData(new string[0], true, true)]
    [InlineData(new[] { "--original-ende" }, false, true)]
    [InlineData(new[] { "dos", "--original-ende", "--original-roboter" }, false, false)]
    [InlineData(new[] { "--original-roboter", "dos" }, true, false)]
    public void The_menu_switch_is_on_unless_original_end_is_given(string[] args, bool menu, bool skip)
    {
        var a = HostArguments.Parse(args);
        Assert.Equal(menu, a.ReturnToMenu);
        Assert.Equal(skip, a.SkipRobotPhase);
    }

    [Fact]
    public void The_other_options_and_the_directory_are_read()
    {
        var a = HostArguments.Parse(["--scale", "12", "--aspect", "--frames", "70", "--original-roboter", "gamedata/dos"]);
        Assert.Equal("gamedata/dos", a.Directory);
        Assert.Equal(8, a.Options.Scale);
        Assert.True(a.Options.AspectCorrection);
        Assert.Equal(70, a.Options.MaxFrames);
        Assert.False(a.SkipRobotPhase);
        var none = HostArguments.Parse([]);
        Assert.Null(none.Directory);
        Assert.Equal(new HostOptions(), none.Options);
    }

    /// <summary>
    /// The program run made from the arguments: with <c>--original-roboter</c> no stand-in for call 18 is bound (the week halts
    /// there, ProgramHostTests.Host_plays_week_0_up_to_the_robot_phase_like_the_original), with <c>--original-ende</c> no
    /// return to the menu, with both no deviation is noted; without them call 18 is the host's
    /// <see cref="Traders.App.Deviations.RobotPhaseSkip"/> and the host has its <see cref="Traders.App.Deviations.MenuAfterGameOver"/>.
    /// </summary>
    [GameDataFact]
    public void The_program_run_follows_the_switches()
    {
        var off = HostArguments.Parse([HostArguments.OriginalRobots, HostArguments.OriginalEnd])
            .CreateProgram(TestGameData.DosDir, new RecordedClock([0]));
        Assert.Null(off.RobotSkip);
        Assert.Null(off.MenuReturn);
        Assert.Null(off.Deviation);
        Assert.False(off.Week.Handlers.ContainsKey(WeekSteps.RobotCall));

        var robots = HostArguments.Parse([HostArguments.OriginalRobots]).CreateProgram(TestGameData.DosDir, new RecordedClock([0]));
        Assert.Null(robots.RobotSkip);
        Assert.NotNull(robots.MenuReturn);
        Assert.DoesNotContain("robot", robots.Deviation);

        var end = HostArguments.Parse([HostArguments.OriginalEnd]).CreateProgram(TestGameData.DosDir, new RecordedClock([0]));
        Assert.Null(end.MenuReturn);
        Assert.DoesNotContain("menu", end.Deviation);

        var on = HostArguments.Parse([]).CreateProgram(TestGameData.DosDir, new RecordedClock([0]));
        Assert.Contains("robot", on.Deviation);
        Assert.Contains("menu", on.Deviation);
        Assert.Same(on.RobotSkip, on.Week.Handlers[WeekSteps.RobotCall]);
        Assert.NotNull(on.MenuReturn);
    }
}
