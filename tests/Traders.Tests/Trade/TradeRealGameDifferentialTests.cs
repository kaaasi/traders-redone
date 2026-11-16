using Traders.Core.State;
using Traders.RefEmu;
using L = Traders.Core.State.DgroupLayout;
using R = Traders.Core.Trade.TradeRules;

namespace Traders.Tests.Trade;

/// <summary>
/// The rules of the trade (finding docs/routines/0B9F_0002_handel.md) on states of real trades: the default game (four
/// computers) runs in the RefEmu through the trades of weeks 0–3; at the entries of the rule routines the data segment and the
/// arguments are captured (a sample per routine and week). Each state is then called again in the original
/// (<see cref="TradeHarness"/>, computer 141D replaced) and in the port, once as captured and once with players 0 and 2 made
/// human. Expected values come only from the original.
/// </summary>
public class TradeRealGameDifferentialTests
{
    private sealed record Captured(int Week, ushort Seg, ushort Off, byte[] DataSegment, ushort[] Args)
    {
        public override string ToString() => $"week {Week} {Seg:X4}:{Off:X4}";
    }

    /// <summary>Rule routines and their argument count.</summary>
    private static readonly (ushort Seg, ushort Off, int Args)[] Routines =
    [
        (0x0B9F, 0x005C, 0), (0x0B9F, 0x009E, 0), (0x0B9F, 0x00FE, 0), (0x0B9F, 0x01E1, 0), (0x0B9F, 0x024A, 0),
        (0x0B9F, 0x02BE, 0), (0x0B9F, 0x0321, 0), (0x0B9F, 0x04FC, 0), (0x0B9F, 0x0549, 0), (0x0B9F, 0x0586, 0),
        (0x0B9F, 0x060A, 0), (0x0B9F, 0x0695, 0), (0x0C29, 0x0323, 0), (0x0C29, 0x03B2, 0), (0x0C29, 0x0FB9, 0),
        (0x0C29, 0x051A, 0), (0x0C29, 0x1108, 1),
    ];

    /// <summary>At most this many states per routine and week, every <see cref="Stride"/>-th call.</summary>
    private const int PerWeek = 12, Stride = 7;

    private static readonly Lazy<List<Captured>> States = new(Capture);

    private static readonly List<byte[]> EntryStates = [];

    /// <summary>The data segment at the entry of 0B9F:0002 in weeks 0–3 of the same game (for the flow tests).</summary>
    internal static IReadOnlyList<byte[]> Entries
    {
        get
        {
            _ = States.Value;
            return EntryStates;
        }
    }

    private static List<Captured> Capture()
    {
        var p = OriginalProgram.StartGame(TestGameData.DosDir);
        var list = new List<Captured>();
        var seen = new Dictionary<(int, ushort, ushort), int>();
        int week = -1;
        foreach (var (seg, off, args) in Routines)
            p.Machine.AddCodeHook(p.Segment(seg), off, _ =>
            {
                if (week < 0) return;
                var key = (week, seg, off);
                int n = seen.GetValueOrDefault(key);
                seen[key] = n + 1;
                if (n % Stride != 0 || n / Stride >= PerWeek) return;
                list.Add(new Captured(week, seg, off, p.DataSegment()[..L.Size],
                    Enumerable.Range(0, args).Select(i => p.StackArgument(i)).ToArray()));
            });
        for (int w = 0; w < 4; w++)
        {
            Assert.True(p.RunToRoutine(0x0B9F, 0x0002, 1800), $"0B9F:0002 not reached (week {p.Week})");
            Assert.Equal(w, p.Week);
            EntryStates.Add(p.DataSegment()[..L.Size]);
            week = w;
            Assert.True(p.RunToRoutine(0x1DEF, 0x000C, 1800), $"end of the trade of week {w} not reached");
            week = -1;
        }
        return list;
    }

    private static readonly Lazy<TradeHarness> Shared = new(() => new TradeHarness());

    private static int? Run(ushort seg, ushort off, Dgroup d, TradeHarness.Recorder rec, ushort[] args)
    {
        switch (seg, off)
        {
            case (0x0B9F, 0x005C): R.ResetWare(d); break;
            case (0x0B9F, 0x009E): R.ComputeNeed(d); break;
            case (0x0B9F, 0x00FE): R.ChooseRoles(d, rec.Random, rec, rec); break;
            case (0x0B9F, 0x01E1): R.CountRoles(d); break;
            case (0x0B9F, 0x024A): return R.BiddingLength(d);
            case (0x0B9F, 0x02BE): R.CheckTopBuyers(d); break;
            case (0x0B9F, 0x0321): R.Step(d); break;
            case (0x0B9F, 0x04FC): R.PushScale(d); break;
            case (0x0B9F, 0x0549): R.MovementSound(d, rec); break;
            case (0x0B9F, 0x0586): R.UpdateExtremes(d); break;
            case (0x0B9F, 0x060A): R.FindParties(d); break;
            case (0x0B9F, 0x0695): R.MarkCountdown(d); break;
            case (0x0C29, 0x0323): R.Match(d, rec.Random, rec, rec); break;
            case (0x0C29, 0x03B2): R.Settle(d, rec.Random, rec, rec); break;
            case (0x0C29, 0x0FB9): R.Leave(d, rec); break;
            case (0x0C29, 0x051A): R.Layout(d); break;
            case (0x0C29, 0x1108): R.DrawStockChange(d, rec, (short)args[0]); break;
            default: throw new InvalidOperationException($"{seg:X4}:{off:X4}");
        }
        return null;
    }

    // [DIFFERENTIAL] 0B9F:0321, 0C29:0323, 0C29:03B2, 0B9F:00FE
    [GameDataFact]
    public void Rules_of_the_trade_on_states_of_weeks_0_to_3()
    {
        var states = States.Value;
        foreach (var (seg, off, _) in Routines)
            Assert.True(states.Any(s => s.Seg == seg && s.Off == off) || (seg, off) == (0x0B9F, 0x04FC),
                $"{seg:X4}:{off:X4} not reached in weeks 0-3");
        Assert.Equal([0, 1, 2, 3], states.Select(s => s.Week).Distinct().Order());
        foreach (var s in states)
            foreach (bool humans in new[] { false, true })
            {
                var d = new Dgroup((byte[])s.DataSegment.Clone());
                if (humans)
                {
                    d.Set(L.IsComputer + 0, (byte)0);
                    d.Set(L.IsComputer + 2, (byte)0);
                }
                Shared.Value.AssertSame(d, s.Seg, s.Off, (p, rec) => Run(s.Seg, s.Off, p, rec, s.Args), $"{s} humans {humans}",
                    mask: (s.Seg, s.Off) == (0x0B9F, 0x024A) ? 0xFFFF : 0, args: s.Args);
            }
        // the bookings of real deals: seller and buyer, with and without buyer
        Assert.Contains(states, s => (s.Seg, s.Off) == (0x0C29, 0x03B2));
    }
}
