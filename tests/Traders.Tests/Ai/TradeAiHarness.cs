using Traders.Core;
using Traders.Core.Ai;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.RefEmu.Cpu;

namespace Traders.Tests.Ai;

/// <summary>The seven entries of 141D that 0B9F/0C29 call (docs/routines/141D_0008_handelski.md).</summary>
internal enum TradeAiEntry : ushort
{
    Budgets = 0x0008,
    Demand = 0x021A,
    ChooseSide = 0x0397,
    PlanBids = 0x0464,
    Act = 0x09F3,
    Bought = 0x0CCA,
    Sold = 0x0E67,
}

/// <summary>
/// Calls one entry of 141D with a prepared data segment in the original (T.EXE loaded with <see cref="OriginalProgram.LoadOnly"/>)
/// and runs <see cref="TradeAi"/> on the same bytes; compares the whole data segment (including the random state DS:10D2) and
/// the number and arguments of the random() draws. For 141D:0397 the uninitialised local [bp-2] is set to the given stale word
/// after the frame is built (141D:039C). Records which of <see cref="Branches"/> the original passed. Expected values come only
/// from the original run.
/// </summary>
internal sealed class TradeAiHarness
{
    public const ushort Seg = 0x141D;

    /// <summary>141D:039C: first instruction after `dec sp; dec sp` of 0397, the local [bp-2] is in place.</summary>
    public const ushort ChooseSideFrameReady = 0x039C;

    /// <summary>Instructions in 141D that mark one path each (offset, meaning).</summary>
    public static readonly (ushort Offset, string Path)[] Branches =
    [
        (0x0156, "008C weights sum 0"), (0x0179, "008C weights sum not 0"),
        (0x022E, "021A computer"), (0x029E, "021A human ware 3"), (0x02BC, "021A human other ware"), (0x02C7, "021A next kind wraps"),
        (0x035F, "021A need clamped"), (0x0374, "021A surplus clamped"), (0x038C, "021A reserve cleared"),
        (0x03B2, "0397 not on a step"), (0x03C1, "0397 draws"), (0x03CF, "0397 first draw 0"), (0x03E2, "0397 second draw 0"),
        (0x03E7, "0397 after draws"), (0x03F8, "0397 surplus >= 10"), (0x03FF, "0397 surplus < 10"), (0x041F, "0397 rule 4BBE"),
        (0x042D, "0397 target player"), (0x044E, "0397 rank rule"),
        (0x04CD, "04A5 step min"), (0x04D5, "04A5 step max"), (0x0572, "04A5 level raised to price"),
        (0x065C, "05A3 level full"), (0x066C, "05A3 units added"), (0x06F0, "05A3 need to reserve"), (0x0700, "05A3 reserve buys"),
        (0x079A, "05A3 no units"), (0x07D2, "05A3 current level"),
        (0x0897, "0806 rank later"), (0x08F5, "0806 rank earlier"), (0x093F, "0806 buyer entry"), (0x099D, "0806 seller entry"),
        (0x09E4, "0806 index"),
        (0x0A61, "0A1D position >= 0"), (0x0A83, "0A1D position < 0"), (0x0AB6, "0A1D below spread"), (0x0AD7, "0A1D first -2"),
        (0x0AE0, "0A1D at or above spread"), (0x0B19, "0A1D highest bid rule"), (0x0B20, "0A1D target kept"),
        (0x0B24, "0A1D second -2"), (0x0B30, "0A1D raised to bid"), (0x0B39, "0A1D raised to market"), (0x0B4C, "0A1D level 3"),
        (0x0B8D, "0A1D capped by budget"), (0x0BBE, "0A1D target capped"), (0x0BD0, "0A1D up"), (0x0BEC, "0A1D down"),
        (0x0C36, "0BFD surplus < 10"), (0x0C42, "0BFD 12 steps"), (0x0C5E, "0BFD draw"), (0x0C6C, "0BFD forced step"),
        (0x0C93, "0BFD down"), (0x0CAB, "0BFD up"), (0x0CB2, "0BFD on the floor"),
        (0x0CD9, "0CCA human"), (0x0CFD, "0CCA need clamped"), (0x0D6C, "0CCA units left"), (0x0D8A, "0CCA levels exhausted"),
        (0x0DD4, "0CCA next level"),
        (0x0E49, "0E09 new highest bid"),
        (0x0E85, "0E67 surplus clamped"), (0x0E9D, "0E67 index past end"), (0x0EB8, "0E67 entry skipped"),
        (0x0EE1, "0ED7 past end"),
    ];

    private readonly OriginalProgram _program = OriginalProgram.LoadOnly(TestGameData.DosDir);
    private readonly byte[] _initial;
    private readonly List<short> _draws = [];
    private short _stale;

    public TradeAiHarness()
    {
        _initial = _program.DataSegment();
        var m = _program.Machine;
        m.AddCodeHook(_program.Segment(0x21FD), 0x01BA, _ => _draws.Add((short)_program.StackArgument(0)));
        ushort seg = _program.Segment(Seg);
        m.AddCodeHook(seg, ChooseSideFrameReady, _ => m.Mem.Write16(m.Cpu.Ss, (ushort)(m.Cpu.Bp - 2), unchecked((ushort)_stale)));
        foreach (var (offset, _) in Branches) m.AddCodeHook(seg, offset, _ => Hits.Add(offset));
    }

    /// <summary>Offsets of <see cref="Branches"/> the original passed so far (over all runs).</summary>
    public HashSet<ushort> Hits { get; } = [];

    /// <summary>Runs per entry so far.</summary>
    public Dictionary<TradeAiEntry, int> Runs { get; } = [];

    /// <summary>Runs in which the original drew at least once / never, per entry.</summary>
    public Dictionary<TradeAiEntry, (int With, int Without)> Draws { get; } = [];

    /// <summary>
    /// Runs original and port on <paramref name="input"/>; <paramref name="argument"/> is the price of 0CCA, <paramref name="stale"/>
    /// the stack word under 0397's local. Asserts identical results and returns the port's state.
    /// </summary>
    public Dgroup AssertSame(TradeAiEntry entry, Dgroup input, string label, short argument = 0, short stale = 0)
    {
        var m = _program.Machine;
        var full = (byte[])_initial.Clone();
        input.Bytes.AsSpan(0, DgroupLayout.Size).CopyTo(full);
        m.Mem.WriteBlock(Memory.Linear(_program.Dgroup, 0), full);
        m.Cpu.Ss = 0x9000;
        m.Cpu.Sp = 0xFFF0;
        int events = m.Log.Events.Count;
        _draws.Clear();
        _stale = stale;
        try
        {
            if (entry == TradeAiEntry.Bought) _program.Call(Seg, (ushort)entry, unchecked((ushort)argument));
            else _program.Call(Seg, (ushort)entry);
        }
        catch (EmulationException e)
        {
            Assert.Fail($"{label}: original faulted: {e.Message}");
        }
        var original = _program.DataSegment();

        var ported = Run(entry, input, argument, stale, out long calls);

        Assert.True(m.Log.Events.Count == events, $"{label}: original used a host service");
        Assert.Equal(_initial[DgroupLayout.Size..], original[DgroupLayout.Size..]);
        var diff = Enumerable.Range(0, DgroupLayout.Size)
            .Where(i => original[i] != ported.Bytes[i])
            .Take(8)
            .Select(i => $"DS:{i:X4} original {original[i]:X2} port {ported.Bytes[i]:X2}")
            .ToList();
        Assert.True(diff.Count == 0, $"{label} ({entry}): " + string.Join(", ", diff));
        Assert.True(_draws.Count == calls, $"{label}: original drew {_draws.Count} times, port {calls}");
        short expected = entry == TradeAiEntry.ChooseSide ? (short)3 : (short)400;
        Assert.All(_draws, n => Assert.Equal(expected, n));
        Runs[entry] = Runs.GetValueOrDefault(entry) + 1;
        var (with, without) = Draws.GetValueOrDefault(entry);
        Draws[entry] = _draws.Count > 0 ? (with + 1, without) : (with, without + 1);
        return ported;
    }

    /// <summary>Runs the port of <paramref name="entry"/> on a copy of <paramref name="input"/>; DS:10D2 carries the random state.</summary>
    public static Dgroup Run(TradeAiEntry entry, Dgroup input, short argument, short stale, out long draws)
    {
        var d = new Dgroup((byte[])input.Bytes.Clone());
        var random = new OriginalRandom(new RecordedClock([]), d.U32(DgroupLayout.RandomSeed));
        switch (entry)
        {
            case TradeAiEntry.Budgets: TradeAi.Budgets(d); break;
            case TradeAiEntry.Demand: TradeAi.Demand(d); break;
            case TradeAiEntry.ChooseSide: TradeAi.ChooseSide(d, random, stale); break;
            case TradeAiEntry.PlanBids: TradeAi.PlanBids(d); break;
            case TradeAiEntry.Act: TradeAi.Act(d, random); break;
            case TradeAiEntry.Bought: TradeAi.Bought(d, argument); break;
            case TradeAiEntry.Sold: TradeAi.Sold(d); break;
            default: throw new ArgumentOutOfRangeException(nameof(entry));
        }
        d.Set(DgroupLayout.RandomSeed, random.Seed);
        draws = random.Calls;
        return d;
    }
}
