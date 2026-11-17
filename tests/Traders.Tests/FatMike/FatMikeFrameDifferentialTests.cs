using Traders.Core;
using Traders.Core.FatMike;
using Traders.Core.Field;
using Traders.Core.State;
using Xunit.Abstractions;
using static Traders.Tests.FatMike.FatMikeFrameHarness;

namespace Traders.Tests.FatMike;

/// <summary>
/// Fat Mike 1F63:000F (call 26) and the contest 1F63:0B03 (call 30) as steps of the phase machine in lockstep with the original
/// in a running game (<see cref="FatMikeFrameHarness"/>): every retrace of the original is a frame of the port (frame counter of
/// the handler); data segment, DS:10D2, the number of random() calls and the picture of the port's scene (VRAM planes, DAC,
/// CRTC start) are equal at each of them and at the return.
/// </summary>
public class FatMikeFrameDifferentialTests
{
    private readonly ITestOutputHelper _out;

    public FatMikeFrameDifferentialTests(ITestOutputHelper output) => _out = output;

    private const byte Ins = 0x52;

    private const int Stock3 = DgroupLayout.Stock + 6;

    private Result Check(Script script)
    {
        var r = Run(script);
        _out.WriteLine($"{script.Routine} week {script.Week}: retraces {r.Retraces}, random {r.RandomCalls}, pictures compared " +
            $"{r.PicturesCompared} ({r.DistinctPictures} distinct)");
        _out.WriteLine(string.Join("; ", r.Events.GroupBy(e => e[..e.IndexOf('(')]).Select(g => $"{g.Key} {g.Count()}")));
        Assert.Equal(r.Retraces + 1, r.PicturesCompared);
        Assert.True(r.DistinctPictures > 10, $"only {r.DistinctPictures} distinct pictures");
        return r;
    }

    /// <summary>Ins held 3 frames every <paramref name="every"/> frames from <paramref name="from"/> to <paramref name="to"/>.</summary>
    private static Press[] Fire(long from, long every, long to) =>
        [.. Enumerable.Range(0, (int)((to - from) / every)).Select(i => new Press(from + i * every, 3, Ins))];

    private static (int, byte[]) Word(int offset, short value) => (offset, BitConverter.GetBytes(value));

    private static int Count(Result r, string prefix) => r.Events.Count(e => e.StartsWith(prefix, StringComparison.Ordinal));

    // [DIFFERENTIAL] 1F63:000F – whole Fat Mike frame by frame in week 0, four computers (scene, scale, notice, offers)
    // [DIFFERENTIAL] 1F63:104B – the indicator pictures
    [GameDataFact]
    public void Fat_Mike_with_four_computers_runs_in_lockstep_in_week_0()
    {
        var r = Check(new Script(Routine.FatMike, 0, [], [], []));
        Assert.True(Count(r, nameof(FatMikeEventKind.Indicator)) > 0);
        Assert.True(Count(r, nameof(FatMikeEventKind.TypeCharacter)) > 0);
    }

    /// <summary>
    /// Feasibility of a punishment in the state <paramref name="ds"/> (1F63:08DA ends only when its count of fields is hit): the
    /// count of 1F63:0A8A for player <paramref name="p"/> with <paramref name="verdict"/> must not exceed his fields that qualify
    /// (own, not the HQ, for crops destroyed not empty), and the field needs two columns and rows (random(0) would reseed from
    /// the clock). Returns the count; an impossible state fails with a clear message instead of running endlessly.
    /// </summary>
    private static int FeasiblePunishment(byte[] ds, int p, int verdict, bool nobodyPaid)
    {
        var d = new Dgroup((byte[])ds.Clone());
        int fields = d.U8(DgroupLayout.FieldCount + p);
        int count = nobodyPaid ? (fields - 1) / 4
            : verdict == 1 ? Enumerable.Range(0, 4).Sum(k => d.S8(FieldGrid.OwnedKinds + p * 4 + k)) / 4
            : fields / 5;
        int cols = d.S16(FieldGrid.Columns), rows = d.S16(FieldGrid.Rows);
        int qualifying = 0;
        for (int x = 0; x < cols; x++)
            for (int y = 0; y < rows; y++)
            {
                int cell = x * 6 + y;
                if (d.S8(FieldGrid.Owner + cell) != p) continue;
                sbyte kind = d.S8(FieldGrid.Kind + cell);
                if (kind == 4 || (verdict == 1 && kind == -2)) continue;
                qualifying++;
            }
        Assert.True(cols > 1 && rows > 1, $"field {cols}×{rows}: random(0) in 1F63:08DA would reseed from the clock");
        Assert.True(count <= qualifying,
            $"player {p} verdict {verdict}: count {count} exceeds his {qualifying} qualifying fields (08DA would not end)");
        return count;
    }

    /// <summary>A seed for DS:10D2 whose first draw random(6) of 1F63:000F is 0 (the blinking 0305 runs).</summary>
    private static uint BlinkSeed(int start)
    {
        for (uint s = (uint)start; ; s++)
            if (new OriginalRandom(new RecordedClock([]), s).Next(6) == 0) return s;
    }

    // [DIFFERENTIAL] 1F63:000F – week 1, players 0 and 2 human with stock 300 paying with Ins, the blinking 0305 (seed chosen)
    // [DIFFERENTIAL] 1F63:0305 – frames and pictures of the blinking
    // [DIFFERENTIAL] 1F63:04D8 – offers with human payment, indicator 113E per frame
    [GameDataFact]
    public void Fat_Mike_with_paying_humans_and_blinking_runs_in_lockstep_in_week_1()
    {
        uint seed = BlinkSeed(0x2601);
        var r = Check(new Script(Routine.FatMike, 1, [0, 2],
            [Word(Stock3, 300), Word(Stock3 + 16, 300), (DgroupLayout.RandomSeed, BitConverter.GetBytes(seed))],
            Fire(300, 9, 3000)));
        Assert.True(Count(r, nameof(FatMikeEventKind.Indicator)) > 4, "no blinking or indicator");
        Assert.True(Count(r, "Sound(0, 37)") >= 2, "the humans did not pay");
    }

    // [DIFFERENTIAL] 1F63:079A – week 3, the player with the most fields human without stock 3: punished on the field
    // [DIFFERENTIAL] 1F63:08DA – emblem, panel, typed lines of the text table, fields hit with 25 retraces each
    // [DIFFERENTIAL] 0609:1BE5 – the board of the punishment (with its border draws)
    [GameDataTheory]
    [InlineData(0)]
    [InlineData(1)]
    public void Fat_Mike_punishing_a_human_runs_in_lockstep_in_week_3(int flag)
    {
        var ds = DataSegmentAt(Routine.FatMike, 3);
        int most = Enumerable.Range(0, 4).MaxBy(i => ds[DgroupLayout.FieldCount + i]);
        // the computers can always pay (prices below week 4 are at most 5·30): only the human is punished
        var set = Enumerable.Range(0, 4).Select(p => Word(Stock3 + 8 * p, (short)(p == most ? 0 : 500)))
            .Append((DgroupLayout.FatMikeFlag + most, [(byte)flag])).ToList();
        foreach (var (o, b) in set) Array.Copy(b, 0, ds, o, b.Length);
        int count = FeasiblePunishment(ds, most, flag == 1 ? 2 : 1, nobodyPaid: false);
        _out.WriteLine($"player {most}: {count} fields hit");
        var r = Check(new Script(Routine.FatMike, 3, [most], [.. set], Fire(200, 11, 6000)));
        Assert.True(Count(r, nameof(FieldEventKind.DrawFieldAndNeighbours)) == count, "fields hit");
        Assert.True(Count(r, nameof(FatMikeEventKind.Emblem)) == 1 && Count(r, nameof(FatMikeEventKind.TypeLines)) == 2);
    }

    // [DIFFERENTIAL] 1F63:079A – week 3, nobody paid (four humans without stock 3): the warning screen DS:3B74, the notice
    // DS:3B94 on the field and every player punished (fields lost)
    [GameDataFact]
    public void Fat_Mike_when_nobody_paid_runs_in_lockstep_in_week_3()
    {
        var ds = DataSegmentAt(Routine.FatMike, 3);
        var set = Enumerable.Range(0, 4).Select(p => Word(Stock3 + 8 * p, 0)).ToArray();
        foreach (var (o, b) in set) Array.Copy(b, 0, ds, o, b.Length);
        int total = Enumerable.Range(0, 4).Sum(p => Math.Max(0, FeasiblePunishment(ds, p, 2, nobodyPaid: true)));
        var r = Check(new Script(Routine.FatMike, 3, [0, 1, 2, 3], set, Fire(200, 11, 8000)));
        Assert.Equal(1, r.DataSegment[FatMikePhase.NobodyPaid]);
        Assert.Equal(total, Count(r, nameof(FieldEventKind.DrawFieldAndNeighbours)));
    }

    // [DIFFERENTIAL] 1F63:0B03 – whole contest frame by frame in week 2, four computers (cash prize, flashing, wait loop mode 4)
    // [DIFFERENTIAL] 1F63:11AD – the flashing colours
    [GameDataFact]
    public void Contest_with_four_computers_runs_in_lockstep_in_week_2()
    {
        var set = Enumerable.Range(0, 4).Select(p => Word(Stock3 + 8 * p, (short)(20 + 7 * p)))
            .Append((DgroupLayout.FatMikeFlag, [0, 0, 0, 0])).ToArray();
        var r = Check(new Script(Routine.Contest, 2, [], set, []));
        Assert.True(Count(r, nameof(FatMikeEventKind.DacColour)) == 8, "no flashing");
        Assert.True(Count(r, nameof(FatMikeEventKind.Number)) == 1, "no cash prize");
    }

    // [DIFFERENTIAL] 1F63:0B03 – week 2, the largest stock 200 of a computer: the field prize DS:3B70 and the second field round
    // [DIFFERENTIAL] 0609:0080 – the second field round frame by frame, its typed notice (FieldScene.NoticeSteps) in the picture
    // [DIFFERENTIAL] 0609:0204 – status with DS:1582 = 1
    [GameDataFact]
    public void Contest_with_the_field_prize_runs_in_lockstep_in_week_2()
    {
        var ds = DataSegmentAt(Routine.Contest, 2);
        Assert.True(ds[FieldGrid.FreeFields] > 0, "no free field left for the field prize");
        var set = Enumerable.Range(0, 4).Select(p => Word(Stock3 + 8 * p, (short)(p == 1 ? 200 : 30 + p)))
            .Append((DgroupLayout.FatMikeFlag, [0, 0, 0, 0])).ToArray();
        var r = Check(new Script(Routine.Contest, 2, [], set, []));
        Assert.True(Count(r, nameof(FatMikeEventKind.Text)) == 1, "no field prize");
        Assert.True(Count(r, nameof(FieldEventKind.DrawStatus)) > 0 && Count(r, nameof(FieldEventKind.LastFrame)) == 1, "no second round");
    }

    // [DIFFERENTIAL] 1F63:0B03 – week 2, two humans (0 and 1, the winner a human) confirming the wait loop with Ins
    [GameDataFact]
    public void Contest_with_humans_runs_in_lockstep_in_week_2()
    {
        var set = new[] { Word(Stock3, 60), Word(Stock3 + 8, 10), Word(Stock3 + 16, 5), Word(Stock3 + 24, 0),
            (DgroupLayout.FatMikeFlag, new byte[] { 0, 0, 0, 0 }) };
        var r = Check(new Script(Routine.Contest, 2, [0, 1], set, Fire(400, 13, 4000)));
        Assert.True(Count(r, nameof(FatMikeEventKind.Number)) == 1);
    }

    /// <summary>
    /// Counter-tests: an indicator picture, a typed character, the bright colour of the flashing or the shadow of a stock left
    /// out must fail on the picture; one frame processed twice by the phase machine must fail at all (data segment, frames or
    /// the machine itself).
    /// </summary>
    [GameDataTheory]
    [InlineData(nameof(Mutation.SkipIndicator), nameof(Routine.FatMike), 1)]
    [InlineData(nameof(Mutation.SkipTypedCharacter), nameof(Routine.FatMike), 0)]
    [InlineData(nameof(Mutation.StockWithoutShadow), nameof(Routine.FatMike), 0)]
    [InlineData(nameof(Mutation.SkipFlash), nameof(Routine.Contest), 2)]
    [InlineData(nameof(Mutation.DoubleFrame), nameof(Routine.FatMike), 0)]
    public void Mutations_are_detected(string mutation, string routine, int week)
    {
        // the original files must be there: a run that fails for want of them proves nothing (pflichtlauf without data)
        Assert.True(TestGameData.Available, $"original files missing ({TestGameData.DosDir})");
        (int, byte[])[] set = week switch
        {
            1 => [(DgroupLayout.RandomSeed, BitConverter.GetBytes(BlinkSeed(0x2601)))],
            2 => [Word(Stock3, 40), (DgroupLayout.FatMikeFlag, [0, 0, 0, 0])],
            _ => [],
        };
        var e = Record.Exception(() => Run(new Script(Enum.Parse<Routine>(routine), week, [], set, []), Enum.Parse<Mutation>(mutation)));
        Assert.NotNull(e);
        string first = e.Message.Split('\n')[0];
        _out.WriteLine(first);
        if (mutation != nameof(Mutation.DoubleFrame)) Assert.Matches("plane|DAC|display start", first);
    }
}
