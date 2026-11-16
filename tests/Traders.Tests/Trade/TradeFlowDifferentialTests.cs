using Traders.Core;
using Traders.Core.Field;
using Traders.Core.State;
using Traders.Core.Trade;
using L = Traders.Core.State.DgroupLayout;
using Flow = Traders.Core.Trade.Trade;
using T = Traders.Core.Trade.TradeLayout;

namespace Traders.Tests.Trade;

/// <summary>
/// The flow of the trade 0B9F:0002 (finding docs/routines/0B9F_0002_handel.md) against the original with
/// <see cref="TradeFlowHarness"/>: the whole trade of weeks 0–3 of a real game (states at the entry of 0B9F:0002, captured by
/// <see cref="TradeRealGameDifferentialTests"/>), computer 141D replaced, the actions of all four players scripted. Expected
/// values come only from the original.
/// </summary>
public class TradeFlowDifferentialTests
{
    private static readonly Lazy<TradeFlowHarness> Shared = new(() => new TradeFlowHarness());

    /// <summary>
    /// Actions per control read from the data segment: in the role choice (countdown period 14h) about half of the players
    /// want to sell (up) and the others to buy (down); in the bidding (period 1Eh) buyers climb and sellers descend, so that deals
    /// happen. Every tenth action is idle or the opposite, now and then fire; <paramref name="salt"/> varies the run. Without
    /// <paramref name="sellers"/> everybody stays a buyer (no bidding).
    /// </summary>
    private static Func<int, Func<int, byte>, sbyte[]> Script(int salt, bool sellers = true) => (n, read) =>
        [.. Enumerable.Range(0, 4).Select(p =>
        {
            if (!sellers) return (sbyte)3;
            if ((n + 31 * p + salt) % 211 == 0) return (sbyte)10;
            int h = (int)((uint)(n * 2654435761u + (uint)(p * 40503 + salt * 977)) >> 16) % 10;
            bool roleChoice = read(Traders.Core.Rules.Countdown.Period) == 0x14;
            int ware = read(T.Ware);
            sbyte wanted = roleChoice
                ? ((p + ware + salt) % 2 == 0 ? (sbyte)2 : (sbyte)3)
                : read(T.Role + p) switch { T.Seller => (sbyte)3, T.Buyer => (sbyte)2, _ => (sbyte)-1 };
            return h switch { 0 => (sbyte)-1, 1 => (sbyte)(wanted == 2 ? 3 : 2), _ => wanted };
        })];

    // [DIFFERENTIAL] 0B9F:0002, 0C29:0001, 0C29:0046, 0C29:0086, 0C29:015C, 0C29:01B7, 0B9F:080C
    // [DIFFERENTIAL] 0C29:0635, 0C29:0B1F, 0C29:0E01, 0C29:109F
    [GameDataFact]
    public void Whole_trade_of_weeks_0_to_3()
    {
        var entries = TradeRealGameDifferentialTests.Entries;
        Assert.Equal(4, entries.Count);
        int deals = 0, bidding = 0, noSeller = 0;
        for (int week = 0; week < entries.Count; week++)
            foreach (int humans in new[] { 0, 0b0101, 0b1111 })
            {
                var d = new Dgroup((byte[])entries[week].Clone());
                for (int p = 0; p < 4; p++)
                    if ((humans >> p & 1) != 0) d.Set(L.IsComputer + p, (byte)0);
                string label = $"week {week} humans {humans:X}";
                var (_, frames, calls) = Shared.Value.AssertSame(d, 0x0B9F, 0x0002,
                    (port, rec) => Flow.Run(port, rec.Random, rec, rec), label, Script(week * 3 + humans));
                Assert.True(frames > 0, label);
                deals += calls.Count(c => c.StartsWith("Ai.AfterSale", StringComparison.Ordinal));
                bidding += calls.Count(c => c == "FadeInFast()");
                noSeller += 4 - calls.Count(c => c == "FadeInFast()");
            }
        foreach (int week in new[] { 0, 3 })
        {
            var d = new Dgroup((byte[])entries[week].Clone());
            var (_, _, calls) = Shared.Value.AssertSame(d, 0x0B9F, 0x0002, (port, rec) => Flow.Run(port, rec.Random, rec, rec),
                $"week {week} only buyers", Script(0, sellers: false));
            noSeller += 4 - calls.Count(c => c == "FadeInFast()");
        }
        // the scripts reach deals, the bidding and wares without a seller
        Assert.True(deals > 0 && bidding > 0 && noSeller > 0, $"deals {deals}, biddings {bidding}, without seller {noSeller}");
    }

    // [DIFFERENTIAL] 0C29:0001, 0C29:0046
    [GameDataFact]
    public void One_ware_with_both_pictures_and_cards()
    {
        var entry = TradeRealGameDifferentialTests.Entries[1];
        foreach (short picture in new short[] { 0, 1 })
            foreach (short card in new short[] { 1, 2, 0x63 })
            {
                var d = new Dgroup((byte[])entry.Clone());
                d.Set(Planting.CardMode, card);
                d.Set(L.IsComputer + 1, (byte)0);
                string label = $"picture {picture} card {card}";
                Shared.Value.AssertSame(d, 0x0C29, 0x0001, (port, rec) => Flow.Ware(port, rec.Random, rec, rec, 2, picture), label,
                    Script(picture + card), 2, (ushort)picture);
            }
    }

    /// <summary>Frames of the port's flow (the original's frames are compared in the lock-step test).</summary>
    [GameDataFact]
    public void Frames_of_the_opening_follow_the_card()
    {
        var entry = TradeRealGameDifferentialTests.Entries[0];
        int Frames(short card)
        {
            var d = new Dgroup((byte[])entry.Clone());
            d.Set(Planting.CardMode, card);
            var rec = new TradeHarness.Recorder(null);
            return Flow.Transition(d, rec).Count();
        }
        // 21FD:0590 (2), card 1: 100 lines with one retrace and 8 palette steps, the flip (2 without keypad plus)
        Assert.Equal(2 + 100 + 8 + 2, Frames(1));
        Assert.Equal(2 + 2, Frames(2));
    }
}
