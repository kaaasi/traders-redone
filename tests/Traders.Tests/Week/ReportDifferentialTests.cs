using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.State;
using Traders.Core.Week;
using Traders.Tests.Menus;

namespace Traders.Tests.Week;

// Routines covered by the scenarios of Week_report_0825_000A_matches_original:
// [DIFFERENTIAL] 0825:02D7 (through 0825:000A)
// [DIFFERENTIAL] 0825:0178 (through 0825:000A: DS effects of the columns, DS:37AA, DS:45D0)
// [DIFFERENTIAL] 0825:0072 (through 0825:000A: DS:37AA)
// [DIFFERENTIAL] 0825:0560 (through 0825:000A: DS:37AA)
// [DIFFERENTIAL] 0825:09C9 (through 0825:000A: calls and frames)

/// <summary>
/// [DIFFERENTIAL] 0825:000A – week report against the original (stubbed layers, see <see cref="WaitLoopHarness"/>): whole
/// data segment (ranking, DS:1584, loop variables, DS:10D2), frame count, random() per frame and the layer calls with frames.
/// </summary>
public sealed class ReportDifferentialTests
{
    private static WaitLoopHarness? _harness;
    private static readonly object Lock = new();

    private static WaitLoopHarness Harness()
    {
        lock (Lock) return _harness ??= new WaitLoopHarness();
    }

    private const byte Fire = 0x52, Abort = 0x0E;

    /// <summary>Arg, players that are computers, seed of the state generator, cash of player 1 (null = random), keys.</summary>
    private sealed record Scenario(int Arg, bool[] Computers, int StateSeed, int? Cash1, KeyScript Keys, bool Gems = true,
        bool Oil = true);

    private static readonly Dictionary<string, Scenario> Table = new()
    {
        ["week loop all computers"] = new(1, [true, true, true, true], 1, null, new KeyScript()),
        ["week loop mixed"] = new(1, [false, true, false, true], 2, null, new KeyScript().Hold(80, 2, Fire).Hold(200, 2, Fire)),
        ["week loop keypad plus during opening"] = new(1, [true, true, true, true], 3, null,
            new KeyScript().Hold(5, 30, MenuKeys.KeypadPlus).Hold(12, 4, MenuKeys.KeypadMinus)),
        ["week loop abort"] = new(1, [false, true, true, true], 4, null, new KeyScript().Hold(70, 2, Abort)),
        ["end of game no wait"] = new(0, [false, false, true, true], 5, null, new KeyScript(), Gems: false, Oil: false),
        // scale 02D7: richest wealth below 120·10 (no rounding), up to 120·100 (tens), above (hundreds), negative; "exactly 10"
        // and "exactly 100" end on the bounds of the two jng (raw 10 stays 10, raw 92 is rounded to 100 and stays), R5B-1
        ["scale small"] = new(0, [true, true, true, true], 6, 300, new KeyScript(), Gems: false, Oil: false),
        ["scale tens"] = new(0, [true, true, true, true], 7, 5_000, new KeyScript(), Gems: false, Oil: false),
        ["scale hundreds"] = new(0, [true, true, true, true], 8, 2_000_000, new KeyScript()),
        ["scale exactly 10"] = new(0, [true, true, true, true], 9, 1_100, new KeyScript(), Gems: false, Oil: false),
        ["scale exactly 100"] = new(0, [true, true, true, true], 12, 11_000, new KeyScript(), Gems: false, Oil: false),
        ["scale huge"] = new(0, [true, true, true, true], 10, 0x7FFF0000, new KeyScript()),
        ["all negative"] = new(0, [true, true, true, true], 11, -40_000, new KeyScript()),
    };

    public static TheoryData<string> Scenarios => new(Table.Keys);

    /// <summary>
    /// A data segment with every value the report reads set from a seeded generator (small or zero for the fixed-cash
    /// scenarios, so player 1's cash decides the richest wealth).
    /// </summary>
    private static Dgroup Input(WaitLoopHarness h, Scenario s)
    {
        var r = new Random(s.StateSeed);
        var d = h.Initial();
        bool fixedCash = s.Cash1 is not null;
        int computers = 0;
        for (int p = 0; p < DgroupLayout.PlayerCount; p++)
        {
            d.Set(DgroupLayout.IsComputer + p, (byte)(s.Computers[p] ? 1 : 0));
            d.Set(ControlReader.ControlType + p, (byte)4);
            if (s.Computers[p]) computers++;
            int cash = fixedCash ? (p == 1 ? s.Cash1!.Value : (s.Cash1!.Value < 0 ? s.Cash1.Value - 1000 * (p + 1) : 10 * p))
                : r.Next(-5_000, 200_000);
            d.Set(DgroupLayout.Cash + 4 * p, cash);
            d.Set(DgroupLayout.FieldCount + p, (byte)(fixedCash ? 0 : r.Next(0, 30)));
            for (int w = 0; w < 4; w++)
            {
                d.Set(DgroupLayout.Stock + 8 * p + 2 * w, (short)(fixedCash ? 0 : r.Next(0, 1000)));
                d.Set(DgroupLayout.FieldsByKind + 4 * p + w, (sbyte)(fixedCash ? 0 : r.Next(0, 8)));
            }
            d.Set(DgroupLayout.Gems + p, (sbyte)r.Next(0, 11));
            d.Set(DgroupLayout.GemCredit + 2 * p, (short)r.Next(0, 2001));
            d.Set(DgroupLayout.Rockets + 2 * p, (short)(fixedCash ? 0 : r.Next(0, 11)));
            d.Set(DgroupLayout.Oil + 2 * p, (short)r.Next(0, 41));
        }
        for (int w = 0; w < 4; w++) d.Set(DgroupLayout.WarePrice + 2 * w, (short)r.Next(1, 40));
        d.Set(DgroupLayout.LandPrice, (short)r.Next(100, 551));
        d.Set(DgroupLayout.RocketPrice, (short)r.Next(50, 500));
        d.Set(DgroupLayout.OilPrice, (short)r.Next(1, 30));
        d.Set(DgroupLayout.BaseUnit, r.Next(12, 50));
        d.Set(DgroupLayout.GemPriceFactor, r.Next(40, 161));
        d.Set(DgroupLayout.GemTrading, (byte)(s.Gems ? 1 : 0));
        d.Set(DgroupLayout.OilConsumption, (byte)(s.Oil ? 1 : 0));
        d.Set(MenuLayout.AllComputers, (byte)(computers == 4 ? 1 : 0));
        d.Set(DgroupLayout.RandomSeed, (uint)r.Next());
        d.Set(DgroupLayout.LoopWare, (short)-1);
        return d;
    }

    [GameDataTheory]
    [MemberData(nameof(Scenarios))]
    public void Week_report_0825_000A_matches_original(string name)
    {
        var s = Table[name];
        var h = Harness();
        var input = Input(h, s);
        WaitLoopHarness.Outcome o;
        lock (Lock) o = h.RunReportOriginal(input, s.Arg, s.Keys.At);
        var (port, report) = WaitLoopHarness.RunReportPort(input, s.Arg, s.Keys.At);

        WaitLoopHarness.AssertSameDs(o.DataSegment, port.State, name);
        Assert.Equal(o.Frames, port.Frames);
        Assert.Equal(o.Random, report.WaitLoop.RandomCalls);
        Assert.Equal(o.Calls, port.Calls);
        Assert.Equal(s.Keys.Holds.Any(k => k.Key == Abort), report.Aborted);
    }

    /// <summary>
    /// The scale scenarios reach the three branches of 0825:02D7 and both bounds of its jng (DS:1584 = 10 and 100 exactly;
    /// checked on the original's run only).
    /// </summary>
    [GameDataFact]
    public void Scale_scenarios_reach_their_branches_in_the_original()
    {
        var h = Harness();
        short Scale(string n)
        {
            WaitLoopHarness.Outcome o;
            lock (Lock) o = h.RunReportOriginal(Input(h, Table[n]), Table[n].Arg, Table[n].Keys.At);
            return (short)(o.DataSegment[ReportLayout.GraphScale] | (o.DataSegment[ReportLayout.GraphScale + 1] << 8));
        }

        Assert.InRange(Scale("scale small"), (short)1, (short)10);
        short tens = Scale("scale tens");
        Assert.True(tens is > 10 and <= 110 && tens % 10 == 0, $"tens {tens}");
        short hundreds = Scale("scale hundreds");
        Assert.True(hundreds > 100 && hundreds % 100 == 0, $"hundreds {hundreds}");
        Assert.Equal((short)10, Scale("scale exactly 10"));
        Assert.Equal((short)100, Scale("scale exactly 100"));
    }
}
