using Traders.App;
using Traders.Core;
using Traders.Core.Phases;

namespace Traders.Tests.App;

/// <summary>
/// The host's command line (<see cref="HostArguments"/>, P3 #194 of review r7-ende-host): the robot switch is on unless
/// <c>--original-roboter</c> is given (decision F3 = A of R7), and the program run made from the arguments follows it.
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
    /// there, ProgramHostTests.Host_plays_week_0_up_to_the_robot_phase_like_the_original) and no deviation is noted; without it
    /// call 18 is the host's <see cref="Traders.App.Deviations.RobotPhaseSkip"/>.
    /// </summary>
    [GameDataFact]
    public void The_program_run_follows_the_switch()
    {
        var off = HostArguments.Parse([HostArguments.OriginalRobots]).CreateProgram(TestGameData.DosDir, new RecordedClock([0]));
        Assert.Null(off.RobotSkip);
        Assert.Null(off.Deviation);
        Assert.False(off.Week.Handlers.ContainsKey(WeekSteps.RobotCall));

        var on = HostArguments.Parse([]).CreateProgram(TestGameData.DosDir, new RecordedClock([0]));
        Assert.NotNull(on.Deviation);
        Assert.Same(on.RobotSkip, on.Week.Handlers[WeekSteps.RobotCall]);
    }
}
