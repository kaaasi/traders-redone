using Traders.Core.Field;
using Traders.Core.State;
using Xunit.Abstractions;
using P = Traders.Core.Field.Planting;
using static Traders.Tests.Field.PlantingFrameHarness;

namespace Traders.Tests.Field;

/// <summary>
/// The planting phase 0609:0004 as call 8 of the phase machine in lockstep with the original in a running game
/// (<see cref="PlantingFrameHarness"/>): every retrace of the original is a frame of the port, data segment, DS:10D2 and
/// the number of random() calls are equal at each of them and at the return.
/// </summary>
public class PlantingFrameDifferentialTests
{
    private readonly ITestOutputHelper _out;

    public PlantingFrameDifferentialTests(ITestOutputHelper output) => _out = output;

    private Result Check(Script script)
    {
        var r = Run(script);
        _out.WriteLine($"retraces {r.Retraces} (wait loop {r.OracleRetraces}), passes {r.Passes}, random {r.RandomCalls}");
        _out.WriteLine(string.Join("; ", r.Events.GroupBy(e => e[..e.IndexOf('(')]).Select(g => $"{g.Key} {g.Count()}")));
        _out.WriteLine(string.Join("; ", r.Events.Where(e => !e.StartsWith("ComputerTurn") && !e.StartsWith("Sound(2,")).Skip(54).Take(200)));
        return r;
    }

    // [DIFFERENTIAL] 0609:0004 – whole phase frame by frame in week 0, four computers
    // [DIFFERENTIAL] 0609:0204 – status lines and their flip (frames only)
    // [DIFFERENTIAL] 21FD:060D – execute the list, flip the pages, execute again (the panels' flip)
    [GameDataFact]
    public void Planting_with_four_computers_runs_in_lockstep_in_week_0()
    {
        var r = Check(new Script(0, [], [], []));
        Assert.True(r.Passes > 0);
    }

    /// <summary>
    /// A human with control 4 (cursor keys, Ins), keys held 6 frames: Ins in the wait loop, then from frame
    /// <paramref name="from"/> every 110 frames (a move takes about 100) build the HQ, step right, buy, choose, plant, back
    /// to the HQ and end (in a later week the cursor starts on the HQ: no build); then cycling through moves and Ins up to frame <paramref name="to"/>.
    /// </summary>
    private static Press[] Human(long from, long to, bool buildHq = true)
    {
        byte[] plan = buildHq ? [Ins, Right, Ins, Ins, Down, Ins, Left, Ins] : [Right, Ins, Ins, Down, Ins, Left, Ins];
        byte[] cycle = [Right, Ins, Down, Ins, Ins, Up, Ins, Left, Ins, Down, Down, Ins];
        var list = new List<Press> { new(400, 6, Ins) };
        long f = from;
        foreach (byte k in plan)
        {
            list.Add(new Press(f, 6, k));
            f += 110;
        }
        for (int i = 0; f < to; f += 110, i++) list.Add(new Press(f, 6, cycle[i % cycle.Length]));
        return [.. list];
    }

    private const byte Ins = 0x52, Right = 0x4D, Left = 0x4B, Up = 0x48, Down = 0x50;

    private static (int, byte[]) Bytes(int offset, int count, byte value) => (offset, Enumerable.Repeat(value, count).ToArray());

    private static (int, byte[]) Word(int offset, short value) => (offset, BitConverter.GetBytes(value));

    /// <summary>Time limit on (DS:45BA) and <paramref name="units"/> time units for player 0, so a scripted human ends for sure.</summary>
    private static (int, byte[])[] Limited(short units) => [Bytes(FieldCursor.TimeLimit, 1, 1), Word(FieldCursor.TimeLeft, units)];

    /// <summary>Terrain 3 on the start field (0, 0) of player 0, so the human may build the HQ there (0609:074D).</summary>
    private static readonly (int, byte[]) HqAtStart = Bytes(FieldCursor.Terrain, 1, 3);

    // [DIFFERENTIAL] 0609:0004 – week 0, player 0 human (HQ, purchase, planting, end on the HQ), players 1–3 computers
    [GameDataFact]
    public void Planting_with_a_human_runs_in_lockstep_in_week_0()
    {
        var r = Check(new Script(0, [0], [.. Limited(12), HqAtStart], Human(650, 6000)));
        Assert.Contains(r.Events, e => e.StartsWith("ComputerTurn"));
        Assert.Equal(0, r.DataSegment[DgroupLayout.HqX]); // the human built the HQ on (0, 0)
        Assert.Equal(0, r.DataSegment[FieldGrid.Owner + 6]); // bought (1, 0)
        Assert.Equal(0, r.DataSegment[FieldGrid.Kind + 6]); // and planted kind 0 there (one step down from none)
        Assert.Contains("DrawRank(32, 6)", r.Events); // ended on the HQ
        Assert.True(BitConverter.ToInt16(r.DataSegment, FieldCursor.TimeLeft) > 0);
    }

    // [DIFFERENTIAL] 0609:0004 – week 0, the human's time runs out on the way back (one time unit)
    [GameDataFact]
    public void Planting_with_a_human_runs_in_lockstep_until_the_time_runs_out()
    {
        var r = Check(new Script(0, [0], [.. Limited(1), HqAtStart], Human(650, 6000)));
        Assert.Equal(0, BitConverter.ToInt16(r.DataSegment, FieldCursor.TimeLeft));
        Assert.Equal(6, r.DataSegment[P.Mode]);
    }

    // [DIFFERENTIAL] 0609:0004 – week 0 with a human, no cash, no stock, no rockets (plantings fail)
    [GameDataFact]
    public void Planting_without_money_and_stock_runs_in_lockstep()
    {
        Check(new Script(0, [0],
            [Bytes(DgroupLayout.Cash, 16, 0), Bytes(DgroupLayout.Stock, 32, 0), Bytes(DgroupLayout.Rockets, 8, 0), .. Limited(24), HqAtStart],
            Human(650, 8000)));
    }

    // [DIFFERENTIAL] 0609:0004 – week 1 (HQs built, fields owned), player 0 human, players 1–3 computers
    [GameDataFact]
    public void Planting_with_a_human_runs_in_lockstep_in_week_1()
    {
        var r = Check(new Script(1, [0], Limited(24), Human(650, 8000, buildHq: false)));
        Assert.Contains(r.Events, e => e.StartsWith("ComputerTurn"));
    }

    // [DIFFERENTIAL] 0609:0004 – week 1 with a human, every free field taken by player 3 (nothing left to buy)
    [GameDataFact]
    public void Planting_with_every_field_taken_runs_in_lockstep()
    {
        var r = Check(new Script(1, [0], Limited(24), Human(650, 8000, buildHq: false), o =>
        {
            for (int c = 0; c < 54; c++)
                if (o.ReadByte((ushort)(FieldGrid.Owner + c)) == 0xFF) o.WriteByte((ushort)(FieldGrid.Owner + c), 3);
            o.WriteByte(FieldGrid.FreeFields, 0);
        }));
        Assert.DoesNotContain(r.Events, e => e.StartsWith("ComputerBoughtField"));
    }
}
