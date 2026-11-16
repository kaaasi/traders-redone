using Traders.Core;
using Traders.Core.Rules;
using Traders.Core.State;
using Traders.Core.Trade;
using Traders.Tests.Rules;
using L = Traders.Core.State.DgroupLayout;
using R = Traders.Core.Trade.TradeRules;
using T = Traders.Core.Trade.TradeLayout;

namespace Traders.Tests.Trade;

/// <summary>
/// Differential tests of the rules of the trade 0B9F / 0C29 (finding docs/routines/0B9F_0002_handel.md) on prepared data
/// segments: every routine called directly in the original (<see cref="TradeHarness"/>), the port on the same bytes. The states
/// mix values of real trades with the edges: positions −3/0/63h/67h, prices 999, cash below and at the price, stocks 9/10/990,
/// no seller or buyer, cursors out of range, 16-bit wrap of the base price.
/// </summary>
public class TradeRulesDifferentialTests
{
    private static readonly Lazy<TradeHarness> Shared = new(() => new TradeHarness());
    private static TradeHarness H => Shared.Value;

    private const int Cases = 400;

    private static readonly short[] Positions = [-3, -2, -1, 0, 1, 2, 3, 0x31, 0x32, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67];
    private static readonly short[] Stocks = [0, 1, 9, 10, 11, 19, 20, 70, 989, 990, 991, 998, 999];
    private static readonly int[] Cashes = [0, 1, 2, 3, 49, 50, 51, 100, 999, 1000, 50_000, 1_000_000, -1, -500, int.MaxValue, int.MinValue];
    private static readonly short[] Prices = [0, 1, 3, 10, 49, 50, 100, 299, 300, 948, 949, 950, 998, 999, -1, short.MaxValue, short.MinValue];

    private static T0 Pick<T0>(Inputs r, T0[] values) => values[r.Range(0, values.Length - 1)];

    /// <summary>A trade state: four players with roles, positions, cash, stocks, actions; extremes, deal state, layout.</summary>
    internal static Dgroup State(Inputs r)
    {
        var d = H.Initial();
        d.Set(L.RandomSeed, r.Next());
        d.Set(T.Player, (sbyte)r.Range(-128, 127));
        d.Set(T.Ware, r.Chance(95) ? (short)r.Range(0, 3) : (short)r.Range(-1, 4));
        d.Set(T.Variant, (byte)(r.Chance(50) ? 0 : 1));
        for (int i = 0; i < 8; i++) d.Set(T.PlayerBits + i, (byte)(1 << i));
        for (int p = 0; p < 4; p++)
        {
            d.Set(T.Role + p, r.Chance(90) ? Pick(r, new byte[] { T.Buyer, T.Seller, T.Seller, T.Buyer, T.Withdrawn }) : (byte)r.Range(0, 255));
            d.Set(T.Position + 2 * p, r.Chance(70) ? Pick(r, Positions) : (short)r.Range(-3, 0x67));
            d.Set(L.Cash + 4 * p, r.Chance(60) ? Pick(r, Cashes) : r.Range(0, 200_000));
            for (int w = 0; w < 4; w++) d.Set(L.Stock + 8 * p + 2 * w, r.Chance(60) ? Pick(r, Stocks) : (short)r.Range(0, 999));
            d.Set(T.Action + p, r.Chance(90) ? Pick(r, new sbyte[] { -1, -1, 0, 1, 2, 2, 3, 3, 10 }) : (sbyte)r.Range(-128, 127));
            d.Set(T.Leaving + p, (byte)(r.Chance(80) ? 0 : r.Chance(80) ? 1 : r.Range(2, 255)));
            d.Set(L.IsComputer + p, (byte)(r.Chance(90) ? r.Range(0, 1) : r.Range(2, 255)));
            d.Set(T.Need + 4 * p, (int)r.Word());
            d.Set(T.ShownStock + 2 * p, (short)r.Range(0, 99));
            for (int k = 0; k < 4; k++) d.Set(L.FieldsByKind + 4 * p + k, (sbyte)(r.Chance(90) ? r.Range(0, 20) : r.Range(-128, 127)));
        }
        for (int w = 0; w < 4; w++) d.Set(L.WarePrice + 2 * w, r.Chance(80) ? (short)r.Range(3, 300) : Pick(r, Prices));
        d.Set(T.BasePrice, r.Chance(60) ? (short)r.Range(3, 300) : Pick(r, Prices));
        d.Set(T.StartPrice, r.Chance(60) ? (short)r.Range(3, 300) : Pick(r, Prices));
        d.Set(T.BestPrice, r.Chance(50) ? (short)0 : r.Word());
        d.Set(T.HighestBuyer, r.Chance(80) ? (short)r.Range(0, 0x63) : r.Word());
        d.Set(T.LowestSeller, r.Chance(80) ? (short)r.Range(0, 0x67) : r.Word());
        if (r.Chance(30)) d.Set(T.LowestSeller, d.S16(T.HighestBuyer));
        d.Set(T.Settling, (byte)(r.Chance(60) ? 0 : r.Chance(80) ? r.Range(1, 3) : r.Range(0, 255)));
        d.Set(T.DealTimer, (byte)(r.Chance(50) ? 0 : r.Chance(80) ? r.Range(1, 14) : r.Range(0, 255)));
        d.Set(T.DealDelay, (byte)(r.Chance(80) ? r.Range(5, 14) : r.Range(0, 255)));
        d.Set(T.DealParties, (byte)r.Range(0, 255));
        d.Set(T.DealSeller, r.Chance(90) ? (short)r.Range(0, 3) : (short)-1);
        d.Set(T.DealBuyer, r.Chance(80) ? (short)r.Range(0, 3) : r.Chance(50) ? T.NoBuyer : (short)-1);
        d.Set(T.SellerCursor, r.Chance(90) ? (short)r.Range(0, 3) : (short)r.Range(-2, 6));
        d.Set(T.BuyerCursor, r.Chance(90) ? (short)r.Range(0, 3) : (short)r.Range(-2, 6));
        d.Set(T.PushCounter, r.Chance(90) ? (short)r.Range(1, 2) : r.Word());
        d.Set(T.Moved, (byte)(r.Chance(90) ? r.Range(0, 1) : r.Range(0, 255)));
        d.Set(T.MovedBefore, (byte)(r.Chance(90) ? r.Range(0, 1) : r.Range(0, 255)));
        d.Set(T.Sellers, (short)r.Range(-1, 4));
        d.Set(T.ScaleX, (short)(0x40 * r.Range(0, 4)));
        d.Set(L.ComputerIq, r.Chance(95) ? (short)r.Range(0, 3) : (short)r.Range(-2, 100));
        d.Set(L.LimitLow, (sbyte)(r.Chance(90) ? r.Range(3, 30) : r.Range(-128, 127)));
        d.Set(Countdown.Remaining, r.Chance(80) ? (short)r.Range(0, 120) : r.Word());
        d.Set(Traders.Core.Field.Planting.CardMode, (short)r.Range(0, 2));
        return d;
    }

    private static void Each(uint seed, Action<Dgroup, int> body)
    {
        var r = new Inputs(seed);
        for (int i = 0; i < Cases; i++) body(State(r), i);
    }

    private static int? Void(Action action)
    {
        action();
        return null;
    }

    // [DIFFERENTIAL] 0B9F:005C
    [GameDataFact]
    public void Reset_of_a_ware() =>
        Each(0x005C, (d, i) => H.AssertSame(d, 0x0B9F, 0x005C, (p, _) => Void(() => R.ResetWare(p)), $"case {i}"));

    // [DIFFERENTIAL] 0B9F:009E
    [GameDataFact]
    public void Need_per_player() =>
        Each(0x009E, (d, i) => H.AssertSame(d, 0x0B9F, 0x009E, (p, _) => Void(() => R.ComputeNeed(p)), $"case {i}"));

    // [DIFFERENTIAL] 0B9F:00FE
    [GameDataFact]
    public void Role_choice_frame()
    {
        var r = new Inputs(0x00FE);
        for (int i = 0; i < Cases; i++)
        {
            var d = State(r);
            var actions = Enumerable.Range(0, 4).Select(_ => Pick(r, new sbyte[] { -1, 0, 1, 2, 3, 10, 2, 3 })).ToArray();
            H.AssertSame(d, 0x0B9F, 0x00FE, (p, rec) => Void(() => R.ChooseRoles(p, rec.Random, rec, rec)), $"case {i}", _ => actions);
        }
    }

    // [DIFFERENTIAL] 0B9F:01E1
    [GameDataFact]
    public void Counting_the_roles() =>
        Each(0x01E1, (d, i) => H.AssertSame(d, 0x0B9F, 0x01E1, (p, _) => Void(() => R.CountRoles(p)), $"case {i}"));

    // [DIFFERENTIAL] 0B9F:024A
    [GameDataFact]
    public void Length_of_the_bidding()
    {
        int faults = 0;
        Each(0x024A, (d, i) =>
        {
            if (i % 50 == 0) d.Set(L.ComputerIq, (short)-3); // divisor 0: divide error in both (with a seller)
            var original = H.RunOriginal(d, 0x0B9F, 0x024A, []);
            if (original.Fault is not null)
            {
                Assert.Contains("Divide error", original.Fault.Message);
                Assert.Throws<DivideByZeroException>(() => R.BiddingLength(new Dgroup((byte[])d.Bytes.Clone())));
                faults++;
                return;
            }
            H.AssertSame(d, 0x0B9F, 0x024A, (p, _) => R.BiddingLength(p), $"case {i}", mask: 0xFFFF);
        });
        Assert.InRange(faults, 1, Cases / 50);
    }

    // [DIFFERENTIAL] 0B9F:02BE
    [GameDataFact]
    public void Buyers_at_the_top() =>
        Each(0x02BE, (d, i) => H.AssertSame(d, 0x0B9F, 0x02BE, (p, _) => Void(() => R.CheckTopBuyers(p)), $"case {i}"));

    // [DIFFERENTIAL] 0B9F:0321
    [GameDataFact]
    public void Step_of_a_player() =>
        Each(0x0321, (d, i) =>
        {
            d.Set(T.Player, (sbyte)(i % 4));
            H.AssertSame(d, 0x0B9F, 0x0321, (p, _) => Void(() => R.Step(p)), $"case {i}");
        });

    // [DIFFERENTIAL] 0B9F:04FC
    [GameDataFact]
    public void Push_of_the_scale() =>
        Each(0x04FC, (d, i) => H.AssertSame(d, 0x0B9F, 0x04FC, (p, _) => Void(() => R.PushScale(p)), $"case {i}"));

    // [DIFFERENTIAL] 0B9F:0549
    [GameDataFact]
    public void Movement_sound() =>
        Each(0x0549, (d, i) => H.AssertSame(d, 0x0B9F, 0x0549, (p, rec) => Void(() => R.MovementSound(p, rec)), $"case {i}"));

    // [DIFFERENTIAL] 0B9F:0586
    [GameDataFact]
    public void Extremes_of_the_scale() =>
        Each(0x0586, (d, i) => H.AssertSame(d, 0x0B9F, 0x0586, (p, _) => Void(() => R.UpdateExtremes(p)), $"case {i}"));

    // [DIFFERENTIAL] 0B9F:060A
    [GameDataFact]
    public void Parties_of_a_deal() =>
        Each(0x060A, (d, i) => H.AssertSame(d, 0x0B9F, 0x060A, (p, _) => Void(() => R.FindParties(p)), $"case {i}"));

    // [DIFFERENTIAL] 0B9F:0695
    [GameDataFact]
    public void Countdown_mark() =>
        Each(0x0695, (d, i) => H.AssertSame(d, 0x0B9F, 0x0695, (p, _) => Void(() => R.MarkCountdown(p)), $"case {i}"));

    // [DIFFERENTIAL] 0C29:0323
    [GameDataFact]
    public void Matching_frame() =>
        Each(0x0323, (d, i) => H.AssertSame(d, 0x0C29, 0x0323, (p, rec) => Void(() => R.Match(p, rec.Random, rec, rec)), $"case {i}"));

    // [DIFFERENTIAL] 0C29:03B2
    [GameDataFact]
    public void Booking_of_a_deal() =>
        Each(0x03B2, (d, i) =>
        {
            if (i % 3 == 0) d.Set(T.DealBuyer, T.NoBuyer);
            H.AssertSame(d, 0x0C29, 0x03B2, (p, rec) => Void(() => R.Settle(p, rec.Random, rec, rec)), $"case {i}");
        });

    // [DIFFERENTIAL] 0C29:0FB9
    [GameDataFact]
    public void Leaving_sellers() =>
        Each(0x0FB9, (d, i) => H.AssertSame(d, 0x0C29, 0x0FB9, (p, rec) => Void(() => R.Leave(p, rec)), $"case {i}"));

    // [DIFFERENTIAL] 0C29:051A
    [GameDataFact]
    public void Layout_of_the_panels() =>
        Each(0x051A, (d, i) => H.AssertSame(d, 0x0C29, 0x051A, (p, _) => Void(() => R.Layout(p)), $"case {i}"));

    // [DIFFERENTIAL] 0C29:134D
    [GameDataFact]
    public void Stock_display_after_a_deal() =>
        Each(0x1108, (d, i) =>
        {
            d.Set(T.Player, (sbyte)(i % 4));
            short direction = (short)(i % 2 == 0 ? -1 : 1);
            H.AssertSame(d, 0x0C29, 0x1108, (p, rec) => Void(() => R.DrawStockChange(p, rec, direction)), $"case {i}",
                args: [(ushort)direction]);
        });

    // [DIFFERENTIAL] 21FD:04E5
    [GameDataFact]
    public void Countdown_start() =>
        Each(0x04E5, (d, i) =>
        {
            var r = new Inputs((uint)i);
            short value = (short)r.Range(-5, 130), period = Pick(r, new short[] { 1, 20, 30, 0, -1 });
            H.AssertSame(d, 0x21FD, 0x04E5, (p, _) => Void(() => Countdown.Start(p, value, period)), $"case {i}",
                args: [(ushort)value, (ushort)period]);
        });

    // [DIFFERENTIAL] 21FD:04F9
    [GameDataFact]
    public void Countdown_tick() =>
        Each(0x04F9, (d, i) =>
        {
            var r = new Inputs((uint)i);
            d.Set(Countdown.Period, Pick(r, new short[] { 1, 20, 30, 0, -1 }));
            d.Set(Countdown.Ticks, Pick(r, new short[] { 1, 2, 20, 0, -1, short.MinValue }));
            H.AssertSame(d, 0x21FD, 0x04F9, (p, rec) =>
            {
                rec.Calls.Add(TradeHarness.Format("PauseCheck"));
                return Countdown.Tick(p) ? 1 : 0;
            }, $"case {i}", mask: 0xFF);
        });
}
