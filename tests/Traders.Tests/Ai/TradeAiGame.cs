using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.Phases;

namespace Traders.Tests.Ai;

/// <summary>
/// The default game (four computer players) in the RefEmu through weeks 0..3 with hooks on the seven entries of 141D. At every
/// entry the data segment, the argument (0CCA) and, for 0397, the stale stack word under its local are taken; a sample of
/// these states is kept as input for single calls (<see cref="Samples"/>). For every call from 0B9F/0C29 the port runs on the
/// entry state and is compared with the original at the return address of that call (whole data segment including DS:10D2):
/// the lockstep at the call sites (<see cref="Differences"/>, <see cref="Compared"/>). Expected values come only from the RefEmu.
/// </summary>
internal sealed class TradeAiGame
{
    public const int Weeks = 4;

    /// <summary>Return addresses of the calls from 0B9F/0C29 (segment, offset) per entry.</summary>
    public static readonly Dictionary<TradeAiEntry, (ushort Seg, ushort Off)> CallSites = new()
    {
        [TradeAiEntry.Budgets] = (0x0B9F, 0x000F),
        [TradeAiEntry.ChooseSide] = (0x0B9F, 0x0120),
        [TradeAiEntry.PlanBids] = (0x0C29, 0x007A),
        [TradeAiEntry.Demand] = (0x0C29, 0x00D5),
        [TradeAiEntry.Act] = (0x0C29, 0x0289),
        [TradeAiEntry.Sold] = (0x0C29, 0x044A),
        [TradeAiEntry.Bought] = (0x0C29, 0x04AF),
    };

    /// <summary>Every n-th call per (entry, week) is kept as a sample; 1 = all.</summary>
    private static readonly Dictionary<TradeAiEntry, int> SampleEvery = new()
    {
        [TradeAiEntry.ChooseSide] = 40,
        [TradeAiEntry.Act] = 12,
    };

    public sealed record Call(TradeAiEntry Entry, int Week, int Player, byte[] Data, short Argument, short Stale, bool External)
    {
        public Dgroup Copy() => new((byte[])Data.Clone());

        public override string ToString() => $"{Entry} week {Week} player {Player}{(External ? "" : " (inner)")}";
    }

    private sealed record Pending(TradeAiEntry Entry, ushort Cs, ushort Ip, ushort Sp, byte[]? Expected, string Label);

    public static readonly Lazy<TradeAiGame> Default = new(() => new TradeAiGame(0));

    /// <summary>The default game with player 0 human during every trade (no key presses: the human stays idle).</summary>
    public static readonly Lazy<TradeAiGame> WithHuman = new(() => new TradeAiGame(1));

    public List<Call> Samples { get; } = [];

    /// <summary>All calls per (entry, week), inner calls of 021A included.</summary>
    public Dictionary<(TradeAiEntry, int), int> Counts { get; } = [];

    /// <summary>Calls from 0B9F/0C29 compared at their return, per (entry, week).</summary>
    public Dictionary<(TradeAiEntry, int), int> Compared { get; } = [];

    public List<string> Differences { get; } = [];

    /// <summary>Stale words found under 0397's local in the game (value, count).</summary>
    public Dictionary<short, int> StaleWords { get; } = [];

    /// <summary>Bytes the timer and keyboard handlers may change while a call runs (not compared in the lockstep).</summary>
    private static bool InterruptOwned(int offset) =>
        RefEmuWeekOracle.InterruptOwned.Any(r => offset >= r.Offset && offset < r.Offset + r.Length);

    private TradeAiGame(int humans)
    {
        var p = OriginalProgram.StartGame(TestGameData.DosDir);
        if (humans > 0)
        {
            // An idle human would stop the planting; the seats are human only while the trade 0B9F:0002 runs (its countdown
            // ends it without input) and computers again from its retf on.
            ushort trade = p.Segment(0x0B9F);
            void Seats(byte v)
            {
                for (int i = 0; i < humans; i++) p.WriteByte((ushort)(DgroupLayout.IsComputer + i), v);
            }
            p.Machine.AddCodeHook(trade, 0x0002, _ => Seats(0));
            p.Machine.AddCodeHook(trade, 0x005B, _ => Seats(1));
        }
        var m = p.Machine;
        ushort seg = p.Segment(TradeAiHarness.Seg);
        var outer = new HashSet<ushort> { p.Segment(0x0B9F), p.Segment(0x0C29) };
        var pending = new List<Pending>();

        void Enter(TradeAiEntry entry, ushort entrySp, short stale)
        {
            int week = p.Week, player = (sbyte)p.ReadByte(0x37AA);
            ushort ip = m.Mem.Read16(m.Cpu.Ss, entrySp), cs = m.Mem.Read16(m.Cpu.Ss, (ushort)(entrySp + 2));
            short argument = entry == TradeAiEntry.Bought ? (short)m.Mem.Read16(m.Cpu.Ss, (ushort)(entrySp + 4)) : (short)0;
            bool external = outer.Contains(cs);
            var data = p.DataSegment().AsSpan(0, DgroupLayout.Size).ToArray();
            var key = (entry, week);
            int n = Counts[key] = Counts.GetValueOrDefault(key) + 1;
            var call = new Call(entry, week, player, data, argument, stale, external);
            if ((n - 1) % SampleEvery.GetValueOrDefault(entry, 1) == 0) Samples.Add(call);
            if (!external) return;
            byte[]? expected = null;
            try
            {
                expected = TradeAiHarness.Run(entry, call.Copy(), argument, stale, out _).Bytes;
            }
            catch (Exception e)
            {
                Differences.Add($"{call}: port threw {e.GetType().Name}: {e.Message}");
            }
            pending.Add(new Pending(entry, cs, ip, (ushort)(entrySp + 4), expected, $"{call} #{n}"));
        }

        foreach (var entry in Enum.GetValues<TradeAiEntry>().Where(e => e != TradeAiEntry.ChooseSide))
            m.AddCodeHook(seg, (ushort)entry, _ => Enter(entry, m.Cpu.Sp, 0));
        // 0397: the stale word is final only once both `dec sp` have run (an interrupt before them may still write it).
        m.AddCodeHook(seg, TradeAiHarness.ChooseSideFrameReady, _ =>
        {
            short stale = (short)m.Mem.Read16(m.Cpu.Ss, (ushort)(m.Cpu.Bp - 2));
            StaleWords[stale] = StaleWords.GetValueOrDefault(stale) + 1;
            Enter(TradeAiEntry.ChooseSide, (ushort)(m.Cpu.Bp + 2), stale);
        });

        foreach (var (entry, (s, o)) in CallSites)
            m.AddCodeHook(p.Segment(s), o, _ =>
            {
                int i = pending.FindLastIndex(x => x.Cs == m.Cpu.Cs && x.Ip == m.Cpu.IP && x.Sp == m.Cpu.Sp);
                if (i < 0) return; // 0B9F:0120 and 0C29:0289 are also passed for human players without a call
                var c = pending[i];
                pending.RemoveAt(i);
                if (c.Entry != entry) Differences.Add($"{c.Label}: returned at the call site of {entry}");
                if (c.Expected is null) return;
                var original = p.DataSegment();
                var diff = Enumerable.Range(0, DgroupLayout.Size).Where(x => original[x] != c.Expected[x] && !InterruptOwned(x)).Take(6)
                    .Select(x => $"DS:{x:X4} original {original[x]:X2} port {c.Expected[x]:X2}").ToList();
                if (diff.Count > 0) Differences.Add($"{c.Label}: " + string.Join(", ", diff));
                var key = (c.Entry, p.Week);
                Compared[key] = Compared.GetValueOrDefault(key) + 1;
            });

        for (int w = 1; w <= Weeks; w++)
            Assert.True(p.RunToWeekStart(w, 1800), $"week {w} not reached (at week {p.Week}, {m.Seconds:F0} s emulated)");
        if (pending.Count > 0) Differences.Add($"{pending.Count} calls without return, first {pending[0].Label}");
    }
}
