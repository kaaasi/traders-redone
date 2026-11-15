using Traders.Core.Events;
using Traders.Core.Field;
using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.State;
using Traders.Tests.Menus;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Tests.Events;

// Routines covered by the scenarios of Week_incidents_17CB_0005_matches_original:
// [DIFFERENTIAL] 17CB:001E (through 17CB:0005: money incident, gain and loss branches)
// [DIFFERENTIAL] 17CB:02BD (through 17CB:0005: gain booking, calls)
// [DIFFERENTIAL] 17CB:039D (through 17CB:0005: loss booking, calls)
// [DIFFERENTIAL] 17CB:047E (through 17CB:0005: amount classes 0…4 in the Number/Text calls)
// [DIFFERENTIAL] 17CB:04D7 (through 17CB:0005: field takeover)
// [DIFFERENTIAL] 17CB:0736 (through 17CB:0005: harvest loss)
// [DIFFERENTIAL] 17CB:0986 (through 17CB:0005: window calls and frames)
// [DIFFERENTIAL] 17CB:09E0 (through 17CB:0005: text calls)

/// <summary>
/// [DIFFERENTIAL] 17CB:0005 – week incidents against the original (stubbed layers, see <see cref="IncidentsHarness"/>): whole
/// data segment (cash, fields, ranks, shares, loop variables, DS:10D2), frame count, random() per frame and the layer calls
/// with frames and arguments.
/// </summary>
public sealed class IncidentsDifferentialTests
{
    private static IncidentsHarness? _harness;
    private static readonly object Lock = new();

    internal static IncidentsHarness Harness()
    {
        lock (Lock) return _harness ??= new IncidentsHarness();
    }

    private const byte Fire = 0x52, Abort = 0x0E;

    /// <summary>
    /// A state: cash and fields per player (fields: first one the HQ, then kinds cycling through <see cref="Kinds"/>), computers,
    /// DS:2E8E, week, random seed DS:10D2 and the keys.
    /// </summary>
    internal sealed record Scenario(int[] Cash, int[] Fields, bool[] Computers, uint Seed, KeyScript Keys, short Iq = 1,
        short Week = 2, int[][]? Kinds = null, int Columns = 9, int Rows = 6, short LandPrice = 0);

    internal static Dgroup Input(IncidentsHarness h, Scenario s)
    {
        var d = h.Initial();
        d.Set(L.FieldColumns, (short)s.Columns);
        d.Set(L.FieldRows, (short)s.Rows);
        for (int i = 0; i < 9 * 6; i++)
        {
            d.Set(FieldGrid.Owner + i, unchecked((byte)-1));
            d.Set(FieldGrid.Kind + i, unchecked((byte)-2));
        }
        d.Set(FieldGrid.FreeFields, (byte)(s.Columns * s.Rows));
        int computers = 0;
        int next = 0; // cells in column-major order (x·6 + y), only inside the grid
        var cells = new List<(int X, int Y)>();
        for (int y = 0; y < s.Rows; y++)
            for (int x = 0; x < s.Columns; x++)
                cells.Add((x, y));
        cells = cells.OrderBy(c => (c.X * 7 + c.Y * 3) % cells.Count).ThenBy(c => c.X).ToList(); // spread the players
        for (int p = 0; p < L.PlayerCount; p++)
        {
            d.Set(L.IsComputer + p, (byte)(s.Computers[p] ? 1 : 0));
            d.Set(ControlReader.ControlType + p, (byte)4);
            if (s.Computers[p]) computers++;
            d.Set(L.Cash + 4 * p, s.Cash[p]);
            for (int w = 0; w < 4; w++) d.Set(L.Stock + 8 * p + 2 * w, (short)0);
            d.Set(L.Gems + p, (sbyte)0);
            d.Set(L.GemCredit + 2 * p, (short)0);
            d.Set(L.Rockets + 2 * p, (short)0);
            d.Set(L.Oil + 2 * p, (short)0);
            int[] kinds = s.Kinds?[p] ?? [0, 1, 2];
            for (int f = 0; f < s.Fields[p]; f++)
            {
                var (x, y) = cells[next++];
                FieldGrid.Set(d, (short)x, (short)y, (short)p, (short)(f == 0 ? 4 : kinds[(f - 1) % kinds.Length]));
            }
        }
        d.Set(L.LandPrice, s.LandPrice);
        d.Set(L.RocketPrice, (short)0);
        d.Set(L.GemTrading, (byte)0);
        d.Set(L.OilConsumption, (byte)0);
        d.Set(L.ComputerIq, s.Iq);
        d.Set(L.Week, s.Week);
        d.Set(0x090E, (short)1); // VGA (DS:090E, fades with the retrace of 2295:00C5)
        d.Set(MenuLayout.AllComputers, (byte)(computers == 4 ? 1 : 0));
        d.Set(L.RandomSeed, s.Seed);
        d.Set(L.LoopWare, (short)-1);
        return d;
    }

    /// <summary>Fire pressed for two frames every <paramref name="every"/> frames from <paramref name="from"/> on (up to 4000).</summary>
    private static KeyScript Taps(long from, long every, byte key)
    {
        var k = new KeyScript();
        for (long f = from; f < 4_000; f += every) k.Hold(f, 2, key);
        return k;
    }

    private static readonly bool[] AllComputers = [true, true, true, true];

    internal static IncidentsHarness.Outcome RunOriginal(Scenario s)
    {
        var h = Harness();
        lock (Lock) return h.RunOriginal(Input(h, s), s.Keys.At);
    }

    private static readonly bool[] Mixed = [false, true, false, true];
    private static readonly int[] Spread = [500, 3_000, 20_000, 90_000], SpreadFields = [4, 5, 6, 8];
    private static readonly int[] Close = [30_000, 32_000, 34_000, 36_000];
    private static readonly int[] Leaders = [100_000, 100_000, 100_000, 1_000];
    private static readonly int[][] BlightKinds = [[0], [0, 1], [1, 2], [0]];

    /// <summary>Scenarios with the branch they reach in the original (seeds found by searching the original's runs).</summary>
    private static readonly Dictionary<string, Scenario> Table = new()
    {
        ["nothing happens"] = new([90_000, 50_000, 20_000, 5_000], [9, 5, 5, 5], AllComputers, 15, new KeyScript(), Kinds: BlightKinds, Week: 0),
        ["takeover"] = new(Leaders, [10, 10, 10, 3], AllComputers, 1, new KeyScript(), Week: 1),
        ["takeover and gain"] = new(Leaders, [10, 10, 10, 3], AllComputers, 4, new KeyScript(), Week: 3),
        ["takeover and loss rank 0 of nothing"] = new(Leaders, [10, 10, 10, 3], AllComputers, 12, new KeyScript()),
        ["takeover needs random(8)"] = new(Leaders, [8, 8, 8, 6], AllComputers, 1, new KeyScript()),
        ["takeover humans keypad"] = new(Leaders, [10, 10, 10, 3], Mixed, 1,
            new KeyScript().Hold(150, 40, MenuKeys.KeypadPlus).Hold(200, 30, MenuKeys.KeypadMinus).Hold(300, 2, Fire).Hold(420, 2, Fire)
                .Hold(600, 3, Fire).Hold(700, 3, Fire).Hold(900, 2, Fire)),
        ["loss capped 3ED"] = new(Spread, SpreadFields, AllComputers, 1, new KeyScript()),
        ["loss capped half cash"] = new(Spread, SpreadFields, AllComputers, 25, new KeyScript(), Week: 0),
        ["loss immune MEAN computer"] = new(Spread, SpreadFields, AllComputers, 1, new KeyScript(), Iq: 3),
        ["loss human not immune"] = new(Spread, SpreadFields, [true, true, true, false], 25, Taps(380, 40, Fire), Iq: 3),
        ["loss needs random(rank*5+8)"] = new(Close, SpreadFields, AllComputers, 1, new KeyScript()),
        ["loss rank 0 needs random"] = new(Close, SpreadFields, AllComputers, 27, new KeyScript(), Week: 1),
        ["gain needs random(9)"] = new(Close, SpreadFields, AllComputers, 95, new KeyScript()),
        ["gain class 0"] = new([1_000, 1_100, 1_200, 1_300], SpreadFields, AllComputers, 95, new KeyScript()),
        ["gain class 1"] = new([1_000, 1_500, 2_000, 2_500], SpreadFields, AllComputers, 95, new KeyScript()),
        ["gain class 3 pause and abort"] = new([1_000, 3_000, 5_000, 7_000], SpreadFields, AllComputers, 14,
            new KeyScript().Hold(100, 3, PauseKey).Hold(130, 3, PauseKey).Hold(400, 2, Abort)),
        ["gain beyond 16 bit"] = new([0, 1_000, 2_000_000, 3_000_000], SpreadFields, AllComputers, 4, new KeyScript(), Week: 2),
        ["gain negative wealth"] = new([-50_000, -40_000, -30_000, -20_000], SpreadFields, AllComputers, 4, new KeyScript(), Week: 3),
        ["loss without cash"] = new([100, 200, 300, 0], SpreadFields, AllComputers, 25, new KeyScript(), LandPrice: 3_000),
        ["blight with random(0)"] = new([90_000, 50_000, 20_000, 5_000], [9, 5, 5, 5], AllComputers, 6, new KeyScript(), Kinds: BlightKinds),
        ["blight and gain"] = new([90_000, 50_000, 20_000, 5_000], [9, 5, 5, 5], AllComputers, 91, new KeyScript(), Kinds: BlightKinds),
        ["blight rank 1 small grid"] = new([5_000, 90_000, 50_000, 20_000], [4, 5, 5, 4], AllComputers, 272, new KeyScript(),
            Kinds: [[2, 0], [1], [0], [1, 2]], Columns: 5, Rows: 4),
    };

    private const byte PauseKey = 0x19;

    public static TheoryData<string> Scenarios => new(Table.Keys);

    [GameDataTheory]
    [MemberData(nameof(Scenarios))]
    public void Week_incidents_17CB_0005_matches_original(string name)
    {
        var s = Table[name];
        var h = Harness();
        var input = Input(h, s);
        IncidentsHarness.Outcome o;
        lock (Lock) o = h.RunOriginal(input, s.Keys.At);
        var port = IncidentsHarness.RunPort(input, s.Keys.At, o.Clock);

        IncidentsHarness.AssertSameDs(o.DataSegment, port.State, name);
        Assert.Equal(o.Calls, port.Calls);
        Assert.Equal(o.Random, port.Incidents.RandomCalls);
        Assert.Empty(port.Incidents.WaitLoopRandomCalls);
        Assert.Equal(o.Frames, port.Frames);
    }

    /// <summary>
    /// Every branch point of 17CB is reached by at least one scenario in the original, the five amount classes appear in the
    /// Number calls, random(0) and the abort key occur (checked on the original's runs only).
    /// </summary>
    [GameDataFact]
    public void Scenarios_reach_every_branch_in_the_original()
    {
        var visited = new HashSet<string>();
        var classes = new HashSet<int>();
        bool clock = false, abort = false;
        foreach (var (name, s) in Table)
        {
            var o = RunOriginal(s);
            visited.UnionWith(o.Visited);
            foreach (var c in o.Calls.Where(c => c.Seg == 0x2295)) classes.Add(WeekIncidents.AmountClass(c.A));
            clock |= o.Clock.Count > 0;
            abort |= s.Keys.Holds.Any(k => k.Key == Abort) && o.Calls.Count(c => c.Seg == 0x23CD) > 0;
        }
        Assert.Equal(IncidentsHarness.Branches.Values.Order(), visited.Intersect(IncidentsHarness.Branches.Values).Order());
        Assert.Equal([0, 1, 2, 3, 4], classes.Order());
        Assert.True(clock, "no random(0)");
        Assert.True(abort, "no abort");
    }
}
