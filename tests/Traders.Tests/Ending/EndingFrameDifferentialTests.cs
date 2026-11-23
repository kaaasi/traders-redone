using Traders.Core.Ending;
using Xunit.Abstractions;
using static Traders.Tests.Ending.EndingFrameHarness;

namespace Traders.Tests.Ending;

/// <summary>
/// The end check 20B3:0005 where the game ends (call 19 with the end screens) as a step of the phase machine in lockstep with
/// the original in two running games (<see cref="EndingFrameHarness"/>): every retrace of the original is a frame of the port
/// (frame counter of the step), the key wait a poll per frame; data segment, DS:10D2 and the number of random() calls are equal
/// at each of them and at the return; the scene (<see cref="Traders.Presentation.Ending.EndingScene"/>) makes its retrace waits in
/// the same frames, its picture (VRAM, DAC, CRTC start) and command list are equal there too. Counter-tests: the report with
/// argument 1, the reveal's colours 2 and 6 swapped, a frame processed twice, DS:4B9A not incremented, and of the scene the
/// reveal on the hidden page, the wipe's palette step without its frame, the scenery's masked blit left out – each must fail.
/// </summary>
public class EndingFrameDifferentialTests
{
    private readonly ITestOutputHelper _out;

    public EndingFrameDifferentialTests(ITestOutputHelper output) => _out = output;

    /// <summary>The first game: RETURN in the main menu after 0.5 s (four computers).</summary>
    private const double GameA = 0.5;

    /// <summary>The second game: RETURN after 1.3 s (another random state at the new game).</summary>
    private const double GameB = 1.3;

    private Result Check(Script script)
    {
        var r = Run(script);
        _out.WriteLine($"game {script.MenuDelay} week {script.Week}{(script.F10 ? " F10" : "")}: retraces {r.Retraces}, " +
            $"polls {r.Polls}, random {r.RandomCalls}, pixel reads {r.PixelReads}, pictures {r.PicturesCompared} " +
            $"({r.DistinctPictures} distinct)");
        _out.WriteLine(string.Join("; ", r.Events.GroupBy(e => e.Split('(')[0]).Select(g => $"{g.Key} {g.Count()}")));
        Assert.True(r.Retraces > 2_000, $"only {r.Retraces} retraces");
        Assert.True(r.PixelReads >= 5 * EndScreens.RevealCells);
        Assert.Equal(["Music(2, 3)"], r.Events.Where(e => e.StartsWith("Music", StringComparison.Ordinal)));
        Assert.Equal("WaitKey()", r.Events[^1]);
        Assert.Equal(r.Retraces + 2, r.PicturesCompared);
        Assert.True(r.DistinctPictures > 1_000, $"only {r.DistinctPictures} distinct pictures");
        return r;
    }

    // [DIFFERENTIAL] 20B3:0005 – rest 20B3:0053–0088 frame by frame in a running game (first game, week 0, target reached)
    [GameDataFact]
    public void End_screens_run_in_lockstep_in_the_first_game_at_the_target() =>
        Check(new Script(GameA, 0, F10: false, []));

    // [DIFFERENTIAL] 20B3:0005 – rest 20B3:0053–0088 frame by frame in a running game (second game, week 2, F10 held)
    [GameDataFact]
    public void End_screens_run_in_lockstep_in_the_second_game_with_F10() =>
        Check(new Script(GameB, 2, F10: true, [], Polls: 1));

    /// <summary>
    /// Counter-tests of the acceptance (review of lane ende): the report 0825:000A with argument 1, the colours 2 and 6 of the
    /// reveal swapped, one frame processed twice, DS:4B9A not incremented, and three of the scene – the lockstep must fail.
    /// </summary>
    [GameDataTheory]
    [InlineData(nameof(Mutation.ReportArgument1))]
    [InlineData(nameof(Mutation.SwapPixelTargets))]
    [InlineData(nameof(Mutation.DoubleFrame))]
    [InlineData(nameof(Mutation.KeepWeek))]
    [InlineData(nameof(Mutation.SceneRevealOnHiddenPage))]
    [InlineData(nameof(Mutation.ScenePaletteStepAtOnce))]
    [InlineData(nameof(Mutation.SceneWithoutMaskedBlit))]
    public void A_mutated_port_leaves_the_lockstep(string name)
    {
        var mutation = Enum.Parse<Mutation>(name);
        // the original files must be there: a run that fails for want of them proves nothing (pflichtlauf without data)
        Assert.True(Traders.Tests.TestGameData.Available, $"original files missing ({Traders.Tests.TestGameData.DosDir})");
        var e = Record.Exception(() => Run(new Script(GameA, 0, F10: false, []), mutation));
        Assert.NotNull(e);
        _out.WriteLine($"{mutation}: {e.Message}");
    }
}
