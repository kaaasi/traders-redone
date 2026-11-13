using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.State;
using Traders.Core.Week;
using Traders.Tests.Menus;

namespace Traders.Tests.Week;

// Routines covered by the scenarios of Wait_loop_21FD_0004_matches_original:
// [DIFFERENTIAL] 1F63:113E (through 21FD:0004, mode 4)
// [DIFFERENTIAL] 0825:0686 (through 21FD:0004, mode 2)
// [DIFFERENTIAL] 21FD:060D (through 21FD:0004, mode 2: list, page flip, list)
// [DIFFERENTIAL] 1F63:104B (through 21FD:0004, mode 4: calls with their arguments)

/// <summary>
/// [DIFFERENTIAL] 21FD:0004 – the common wait loop of the week screens against the original (stubbed layers, see
/// <see cref="WaitLoopHarness"/>): whole data segment (DS:3308, DS:37AA, DS:54B0, control state, DS:10D2), frame count,
/// random() per frame and the layer calls with their frames.
/// </summary>
public sealed class WaitLoopDifferentialTests
{
    private static WaitLoopHarness? _harness;
    private static readonly object Lock = new();

    private static WaitLoopHarness Harness()
    {
        lock (Lock) return _harness ??= new WaitLoopHarness();
    }

    private const byte Fire = 0x52; // cursor keys control (type 4): Ins fires
    private const byte Abort = 0x0E, Pause = MenuKeys.P;

    private sealed record Scenario(int Mode, bool[] Computers, uint Seed, sbyte Indicator, KeyScript Keys, byte[]? Ranks = null);

    private static readonly Dictionary<string, Scenario> Table = new()
    {
        // all computers: countdown 150, players confirm at 60, 40, 20 and 0 (player 0 once the countdown has stopped at 0)
        ["report all computers"] = new(2, [true, true, true, true], 0x12345678, 0, new KeyScript()),
        ["report mixed"] = new(2, [false, true, false, true], 0x0BADF00D, 0,
            new KeyScript().Hold(30, 3, Fire).Hold(150, 400, Fire), [2, 0, 3, 1]),
        ["report human first frame"] = new(2, [false, false, false, false], 1, 0, new KeyScript().Hold(0, 600, Fire)),
        ["report keypad plus and minus"] = new(2, [true, false, true, true], 7, 0,
            new KeyScript().Hold(10, 40, MenuKeys.KeypadPlus).Hold(60, 30, MenuKeys.KeypadMinus).Hold(100, 2, Fire)),
        ["planting mixed"] = new(1, [true, false, true, false], 99, 0, new KeyScript().Hold(5, 2, Fire).Hold(250, 2, Fire)),
        ["plain mode 3"] = new(3, [true, true, false, true], 3, 0, new KeyScript().Hold(77, 2, Fire)),
        ["pause"] = new(2, [true, true, true, true], 5, 0,
            new KeyScript().Hold(20, 4, Pause).Hold(40, 3, Pause).Hold(60, 2, Pause).Hold(80, 2, Pause)),
        ["abort"] = new(2, [false, true, true, true], 11, 0, new KeyScript().Hold(90, 1, Abort)),
        ["abort first frame"] = new(1, [true, true, true, true], 11, 0, new KeyScript().Hold(0, 3, Abort)),
        // mode 4: the indicator counts down from 3 (switched off at 0), then random(200) once per pass until it hits 0
        ["indicator countdown"] = new(4, [true, true, true, true], 0xCAFEBABE, 3, new KeyScript()),
        ["indicator negative"] = new(4, [true, false, true, true], 0x00000001, -5, new KeyScript().Hold(700, 2, Fire)),
        ["indicator long a"] = new(4, [false, true, true, true], 0x5EED0001, 0, new KeyScript().Hold(900, 2, Fire)),
        ["indicator long b"] = new(4, [false, true, true, true], 0x5EED0002, 0, new KeyScript().Hold(900, 2, Fire)),
        ["indicator long c"] = new(4, [false, true, true, true], 0x5EED0003, 0, new KeyScript().Hold(900, 2, Fire)),
    };

    public static TheoryData<string> Scenarios => new(Table.Keys);

    private static Dgroup Input(WaitLoopHarness h, Scenario s)
    {
        var d = h.Initial();
        int computers = 0;
        for (int p = 0; p < DgroupLayout.PlayerCount; p++)
        {
            d.Set(DgroupLayout.IsComputer + p, (byte)(s.Computers[p] ? 1 : 0));
            d.Set(ControlReader.ControlType + p, (byte)4);
            d.Set(DgroupLayout.RankUnique + p, s.Ranks?[p] ?? (byte)p);
            if (s.Computers[p]) computers++;
        }
        d.Set(MenuLayout.AllComputers, (byte)(computers == 4 ? 1 : 0));
        d.Set(WaitLoopLayout.IndicatorCountdown, s.Indicator);
        d.Set(DgroupLayout.RandomSeed, s.Seed);
        d.Set(DgroupLayout.ActivePlayer, (sbyte)2);
        return d;
    }

    [GameDataTheory]
    [MemberData(nameof(Scenarios))]
    public void Wait_loop_21FD_0004_matches_original(string name)
    {
        var s = Table[name];
        var h = Harness();
        var input = Input(h, s);
        WaitLoopHarness.Outcome o;
        lock (Lock) o = h.RunOriginal(input, s.Mode, s.Keys.At);
        var (port, loop) = WaitLoopHarness.RunPort(input, s.Mode, s.Keys.At);

        WaitLoopHarness.AssertSameDs(o.DataSegment, port.State, name);
        Assert.Equal(o.Frames, port.Frames);
        Assert.Equal(o.Random, loop.RandomCalls);
        Assert.Equal(o.Calls, port.Calls);
        bool aborted = s.Keys.Holds.Any(k => k.Key == Abort);
        Assert.Equal(aborted, loop.Aborted);
    }

    /// <summary>The scenarios reach the paths they are named for (checked on the original's run only).</summary>
    [GameDataFact]
    public void Scenarios_reach_their_paths_in_the_original()
    {
        var h = Harness();
        WaitLoopHarness.Outcome Run(string n)
        {
            lock (Lock) return h.RunOriginal(Input(h, Table[n]), Table[n].Mode, Table[n].Keys.At);
        }

        // all computers: four confirmations with their sounds, four marks
        var all = Run("report all computers");
        Assert.Equal(4, all.Calls.Count(c => c.Seg == 0x0825));
        Assert.Equal(4, all.Calls.Count(c => c.Seg == 0x21A7));
        // the long indicator runs together switch the indicator on at least once (random(200) = 0)
        int on = new[] { "indicator long a", "indicator long b", "indicator long c" }
            .Sum(n => Run(n).Calls.Count(c => c.Seg == 0x1F63 && c.B == 0));
        Assert.True(on > 0, "no indicator switched on");
        // the countdown scenario switches it off
        Assert.Contains(Run("indicator countdown").Calls, c => c.Seg == 0x1F63 && c.B == 1);
        // the planting run shows four player objects
        Assert.Equal(4, Run("planting mixed").Calls.Count(c => c.Seg == 0x0609));
    }
}
