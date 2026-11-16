using Traders.Core.Extra;
using Traders.Core.State;
using Xunit.Abstractions;
using static Traders.Tests.Extra.ExtraFrameHarness;

namespace Traders.Tests.Extra;

/// <summary>
/// The extra phase 1DEF:000C as call 24 of the phase machine in lockstep with the original in a running game
/// (<see cref="ExtraFrameHarness"/>): every retrace of the original is a frame of the port; data segment, DS:10D2, the number
/// of random() calls, the command list and the scene's picture (VRAM planes, DAC, CRTC start) are equal at each of them and at the
/// return.
/// Week 0 of the default game enters the phase by itself (2·DS:57F2 &lt; DS:57F0); in week 2 it is forced or prevented.
/// </summary>
public class ExtraFrameDifferentialTests
{
    private readonly ITestOutputHelper _out;

    public ExtraFrameDifferentialTests(ITestOutputHelper output) => _out = output;

    private const byte Ins = 0x52, Right = 0x4D, Left = 0x4B, Up = 0x48, Down = 0x50;

    private Result Check(Script script, bool entered = true)
    {
        var r = Run(script);
        _out.WriteLine($"retraces {r.Retraces}, passes {r.Passes}, random {r.RandomCalls}, pictures compared " +
            $"{r.PicturesCompared} ({r.DistinctPictures} distinct)");
        _out.WriteLine(string.Join("; ", r.Events.GroupBy(e => e[..e.IndexOf('(')]).Select(g => $"{g.Key} {g.Count()}")));
        Assert.Equal(r.Retraces + 1, r.PicturesCompared);
        if (!entered)
        {
            Assert.Equal(0, r.Retraces);
            return r;
        }
        Assert.Contains(r.Events, e => e.StartsWith(nameof(ExtraEventKind.TypeCharacter), StringComparison.Ordinal));
        Assert.Contains(r.Events, e => e.StartsWith(nameof(ExtraEventKind.Number), StringComparison.Ordinal));
        Assert.Equal(ExtraPhase.Pieces, BitConverter.ToInt16(r.DataSegment, ExtraPhase.Collected));
        return r;
    }

    /// <summary>A human walking: a direction held 2 frames every <paramref name="every"/> frames, then Ins held for the wait loop.</summary>
    private static Press[] Human(long from, long every, long until)
    {
        byte[] keys = [Right, Down, Right, Up, Left, Down, Down, Right];
        var list = new List<Press>();
        int i = 0;
        for (long f = from; f < until; f += every, i++) list.Add(new Press(f, 2, keys[i % keys.Length]));
        for (long f = until; f < until + 3000; f += 40) list.Add(new Press(f, 3, Ins));
        return [.. list];
    }

    private static (int, byte[]) Words(int offset, params short[] values) =>
        (offset, [.. values.SelectMany(BitConverter.GetBytes)]);

    // [DIFFERENTIAL] 1DEF:000C – whole phase frame by frame in week 0, four computers (entered by production)
    // [DIFFERENTIAL] 1DEF:0105 – notice typed frame by frame (picture at each character)
    // [DIFFERENTIAL] 1DEF:04A9 – the walk frame by frame with the picture of every frame
    [GameDataFact]
    public void Phase_with_four_computers_runs_in_lockstep_in_week_0()
    {
        var r = Check(new Script(0, [], [], []));
        Assert.True(r.Passes > 25);
    }

    // [DIFFERENTIAL] 1DEF:000C – week 0, player 1 human walking with the arrows, then Ins in the wait loop
    [GameDataFact]
    public void Phase_with_a_human_runs_in_lockstep_in_week_0()
    {
        Check(new Script(0, [1], [], Human(700, 25, 2200)));
    }

    // [DIFFERENTIAL] 1DEF:000C – week 2, forced (2·57F2 < 57F0), players 0 and 3 human; large 2F26
    [GameDataFact]
    public void Phase_with_humans_runs_in_lockstep_in_week_2()
    {
        Check(new Script(2, [0, 3], [Words(DgroupLayout.ProductionIdle, 40, 0), ((int)DgroupLayout.LimitLow, [30])],
            [.. Human(650, 20, 2000), .. Human(661, 33, 2000)]));
    }

    // [DIFFERENTIAL] 1DEF:000C – week 2 without the phase: no production idle, random(20) ≠ 0 – no frame at all
    [GameDataFact]
    public void Phase_not_entered_returns_at_once_in_week_2()
    {
        uint seed = ExtraDifferentialTests.SeedFor(new Random(2), zero: false);
        Check(new Script(2, [], [Words(DgroupLayout.ProductionIdle, 0, 0), (DgroupLayout.RandomSeed, BitConverter.GetBytes(seed))], []),
            entered: false);
    }

    /// <summary>
    /// Counter-tests (four computers, week 0): a goods icon or a typed character left out must fail on the command list or the picture; one frame
    /// processed twice by the phase machine must fail at all (data segment, frames or the machine itself).
    /// </summary>
    [GameDataTheory]
    [InlineData(nameof(Mutation.SkipGoods), "command list|plane|DAC|display start")]
    [InlineData(nameof(Mutation.SkipTypedCharacter), "command list|plane|DAC|display start")]
    [InlineData(nameof(Mutation.DoubleFrame), ".")]
    public void Mutations_are_detected(string mutation, string expected)
    {
        // the original files must be there: a run that fails for want of them proves nothing (pflichtlauf without data)
        Assert.True(TestGameData.Available, $"original files missing ({TestGameData.DosDir})");
        var e = Record.Exception(() => Run(new Script(0, [], [], []), Enum.Parse<Mutation>(mutation)));
        Assert.NotNull(e);
        string first = e.Message.Split('\n')[0];
        _out.WriteLine(first);
        Assert.Matches(expected, first);
    }
}
