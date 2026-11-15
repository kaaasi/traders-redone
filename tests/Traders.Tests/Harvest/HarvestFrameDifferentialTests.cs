using Traders.Core.Field;
using Traders.Core.Harvest;
using Traders.Core.State;
using Xunit.Abstractions;
using static Traders.Tests.Harvest.HarvestFrameHarness;

namespace Traders.Tests.Harvest;

/// <summary>
/// The harvest 22A3:02D3 as call 16 of the phase machine in lockstep with the original in a running game
/// (<see cref="HarvestFrameHarness"/>): every retrace of the original is a frame of the port; data segment, DS:10D2, the
/// number of random() calls and the scene's picture (VRAM planes, DAC, CRTC start) are equal at each of them and at the return.
/// </summary>
public class HarvestFrameDifferentialTests
{
    private readonly ITestOutputHelper _out;

    public HarvestFrameDifferentialTests(ITestOutputHelper output) => _out = output;

    private const byte Ins = 0x52, Right = 0x4D, Down = 0x50;

    private Result Check(Script script)
    {
        var r = Run(script);
        _out.WriteLine($"retraces {r.Retraces}, passes {r.Passes}, random {r.RandomCalls}, pictures compared " +
            $"{r.PicturesCompared} ({r.DistinctPictures} distinct)");
        _out.WriteLine(string.Join("; ", r.Events.GroupBy(e => e[..e.IndexOf('(')]).Select(g => $"{g.Key} {g.Count()}")));
        Assert.Equal(r.Retraces + 1, r.PicturesCompared);
        Assert.Contains(r.Events, e => e.StartsWith(nameof(HarvestEventKind.TypeCharacter), StringComparison.Ordinal));
        Assert.Contains(r.Events, e => e.StartsWith(nameof(HarvestEventKind.Line), StringComparison.Ordinal));
        for (int p = 0; p < 4; p++) Assert.Equal(Traders.Core.Harvest.Harvest.Done, r.DataSegment[FieldCursor.Mode + p]);
        return r;
    }

    /// <summary>Right held 3 frames every <paramref name="every"/> frames from <paramref name="from"/>, Ins at <paramref name="fire"/>.</summary>
    private static Press[] Human(long from, long every, long fire, byte other = Right)
    {
        var list = new List<Press>();
        int i = 0;
        for (long f = from; f < fire; f += every, i++) list.Add(new Press(f, 3, i % 5 == 4 ? Down : other));
        list.Add(new Press(fire, 3, Ins));
        return [.. list];
    }

    private static (int, byte[]) Words(int offset, int count, short value) =>
        (offset, [.. Enumerable.Range(0, count).SelectMany(_ => BitConverter.GetBytes(value))]);

    // [DIFFERENTIAL] 22A3:02D3 – whole harvest frame by frame in week 0, four computers
    // [DIFFERENTIAL] 22A3:08C1 – weather notice typed frame by frame (picture at each character)
    // [DIFFERENTIAL] 0A69:0288 – frames of the typewriter
    [GameDataFact]
    public void Harvest_with_four_computers_runs_in_lockstep_in_week_0()
    {
        var r = Check(new Script(0, [], [], []));
        Assert.True(r.Passes > 0);
    }

    // [DIFFERENTIAL] 22A3:02D3 – week 0, player 0 human stepping with "right", then fire (automatic); stocks near the limit
    [GameDataFact]
    public void Harvest_with_a_human_runs_in_lockstep_in_week_0()
    {
        var r = Check(new Script(0, [0], [Words(DgroupLayout.Stock, 4, 990)], Human(500, 40, 2500)));
        for (int k = 0; k < 4; k++) Assert.True(BitConverter.ToInt16(r.DataSegment, DgroupLayout.Stock + 2 * k) <= Traders.Core.Harvest.Harvest.StockLimit);
    }

    // [DIFFERENTIAL] 22A3:02D3 – week 2, players 0 and 2 human (0 steps, 2 fires at once)
    [GameDataFact]
    public void Harvest_with_humans_runs_in_lockstep_in_week_2()
    {
        Check(new Script(2, [0, 2], [], [.. Human(450, 30, 1800), new Press(700, 3, Ins)]));
    }

    /// <summary>
    /// Counter-tests (four computers, week 0): a bar line or a typed character left out must fail on the picture; one frame
    /// processed twice by the phase machine must fail at all (data segment, frames or the machine itself).
    /// </summary>
    [GameDataTheory]
    [InlineData(nameof(Mutation.SkipLine), "plane|DAC|display start")]
    [InlineData(nameof(Mutation.SkipTypedCharacter), "plane|DAC|display start")]
    [InlineData(nameof(Mutation.DoubleFrame), ".")]
    public void Mutations_are_detected(string mutation, string expected)
    {
        var e = Record.Exception(() => Run(new Script(0, [], [], []), Enum.Parse<Mutation>(mutation)));
        Assert.NotNull(e);
        string first = e.Message.Split('\n')[0];
        _out.WriteLine(first);
        Assert.Matches(expected, first);
    }
}
