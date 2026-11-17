using Traders.Core;
using Traders.Core.Field;
using Traders.Core.Rules;
using Traders.Core.State;
using Traders.Tests.Rules;
using L = Traders.Core.State.DgroupLayout;
using P = Traders.Core.Purchase.PurchaseLayout;
using R = Traders.Core.Purchase.PurchaseRules;

namespace Traders.Tests.Purchase;

/// <summary>
/// Differential tests of the rules of the purchase 08CF (finding docs/routines/08CF_0004_einkauf.md) on prepared data segments:
/// every routine called directly in the original (<see cref="PurchaseHarness"/>), the port on the same bytes. The states mix
/// plausible values with the edges: no cash, cash just below and at the prices, oil 27h/28h, shop oil 0/1, rockets 9/10, gems
/// 0/9/10, the gem account around 2000, factors far from and near week + 100, the draw in every state, 16-bit and 32-bit wraps.
/// </summary>
public class PurchaseRulesDifferentialTests
{
    private static readonly Lazy<PurchaseHarness> Shared = new(() => new PurchaseHarness());
    private static PurchaseHarness H => Shared.Value;

    private const int Cases = 400;

    private static readonly int[] Cashes =
        [0, 1, 4, 5, 9, 10, 29, 30, 49, 50, 99, 100, 101, 499, 500, 1000, 5000, 50_000, 1_000_000, -1, -100, int.MaxValue, int.MinValue];
    private static readonly short[] Prices = [0, 1, 2, 5, 10, 20, 31, 50, 99, 100, 500, 999, -1, short.MaxValue, short.MinValue];
    private static readonly short[] Credits = [0, 1, 39, 40, 100, 159, 160, 1839, 1840, 1841, 1900, 1960, 1999, 2000, 2001, -1, short.MinValue];
    private static readonly int[] Factors = [0, 1, 39, 40, 41, 99, 100, 101, 102, 110, 111, 120, 140, 159, 160, 161, 2000, -1, 0x10064, int.MaxValue];

    private static T0 Pick<T0>(Inputs r, T0[] values) => values[r.Range(0, values.Length - 1)];

    /// <summary>A purchase state: four players with modes, shops, actions, cash and goods; prices, draw state, layout.</summary>
    internal static Dgroup State(Inputs r)
    {
        var d = H.Initial();
        PurchaseHarness.UseTestTexts(d);
        d.Set(L.RandomSeed, r.Next());
        d.Set(P.Player, (sbyte)(r.Chance(97) ? r.Range(0, 3) : r.Range(-2, 5)));
        d.Set(L.OilConsumption, (byte)(r.Chance(95) ? r.Range(0, 1) : r.Range(2, 255)));
        d.Set(L.GemTrading, (byte)(r.Chance(95) ? r.Range(0, 1) : r.Range(2, 255)));
        for (int p = 0; p < 4; p++)
        {
            d.Set(L.IsComputer + p, (byte)(r.Chance(95) ? r.Range(0, 1) : r.Range(2, 255)));
            d.Set(P.Mode + p, (byte)(r.Chance(90) ? r.Range(P.Street, P.GemShop) : r.Range(0, 255)));
            d.Set(P.Shop + p, (sbyte)(r.Chance(90) ? r.Range(0, 3) : r.Range(-2, 5)));
            d.Set(P.Action + p, r.Chance(90) ? Pick(r, new sbyte[] { -1, -1, 0, 1, 2, 3, 3, 10, 10 }) : (sbyte)r.Range(-128, 127));
            bool moving = r.Chance(25);
            d.Set(P.MoveTimer + 2 * p, moving ? (short)r.Range(1, 0x21) : (short)0);
            d.Set(P.Direction + 2 * p, (short)r.Range(0, moving ? 1 : 3));
            d.Set(P.MoveFrame + 2 * p, (short)r.Range(-1, 3));
            d.Set(Planting.ObjectX + Planting.ObjectSize * p, (short)r.Range(0, 0x140));
            d.Set(Planting.ObjectY + Planting.ObjectSize * p, (short)r.Range(0x70, 0x90));
            d.Set(Planting.ObjectFrame + Planting.ObjectSize * p, (short)r.Range(0, 8));
            d.Set(P.MoveDelay + 4 * p, r.Chance(50) ? 0 : r.Chance(50) ? r.Range(-5, 400) : (int)r.Next());
            d.Set(L.Cash + 4 * p, r.Chance(60) ? Pick(r, Cashes) : r.Range(0, 200_000));
            d.Set(L.Oil + 2 * p, r.Chance(60) ? Pick(r, new short[] { 0, 1, 9, 10, 11, 0x27, 0x28, 0x29, -1 }) : (short)r.Range(0, 0x28));
            d.Set(L.Rockets + 2 * p, r.Chance(60) ? Pick(r, new short[] { 0, 1, 2, 3, 9, 10, 11, -1 }) : (short)r.Range(0, 10));
            d.Set(P.RocketShortfall + 2 * p, (short)(r.Chance(80) ? r.Range(0, 12) : r.Range(-3, 100)));
            d.Set(L.Gems + p, (sbyte)(r.Chance(80) ? r.Range(0, 10) : Pick(r, new sbyte[] { -1, 11, 127, -128 })));
            d.Set(L.GemCredit + 2 * p, r.Chance(60) ? Pick(r, Credits) : (short)r.Range(0, 2000));
            d.Set(P.GemDeals + p, (sbyte)(r.Chance(80) ? r.Range(0, 30) : r.Range(-128, 127)));
            d.Set(L.Rank + p, (byte)(r.Chance(90) ? r.Range(0, 3) : r.Range(4, 255)));
            d.Set(L.FatMikeFlag + p, (byte)(r.Chance(80) ? r.Range(0, 1) : r.Range(2, 255)));
            d.Set(L.FieldsByKind + 4 * p + 3, (sbyte)(r.Chance(90) ? r.Range(0, 5) : r.Range(-128, 127)));
            d.Set(P.Picks + p, (sbyte)(r.Chance(90) ? r.Range(0, 4) : r.Range(-3, 8)));
            d.Set(P.Matches + p, (byte)(r.Chance(70) ? r.Range(0, 4) : r.Range(0, 255)));
            for (int k = 0; k < 4; k++) d.Set(P.Tickets + 4 * p + k, (byte)(r.Chance(90) ? r.Range(0, 2) : r.Range(0, 255)));
        }
        for (int k = 0; k < 4; k++)
        {
            d.Set(P.Balls + k, (byte)(r.Chance(90) ? r.Range(1, 2) : r.Range(0, 255)));
            d.Set(P.Preset + k, (byte)(r.Chance(85) ? 0xFF : r.Range(0, 3)));
        }
        d.Set(P.DrawState, r.Chance(70) ? Pick(r, new sbyte[] { P.DrawWaiting, P.DrawWaiting, 0, 1, 2, 3, P.DrawClosed }) : (sbyte)r.Range(-128, 127));
        d.Set(P.BlinkStep, (sbyte)(r.Chance(90) ? r.Range(0, 7) : r.Range(-128, 127)));
        d.Set(P.BlinkTimer, (short)(r.Chance(70) ? r.Range(1, 2) : r.Range(-2, 5)));
        d.Set(P.ComputerDelay, (short)(r.Chance(50) ? 0 : r.Range(-1, 5)));
        d.Set(P.Remaining, (short)(r.Chance(80) ? r.Range(0, 30) : r.Word()));
        d.Set(P.NextBall, (short)(r.Chance(80) ? r.Range(-2, 30) : r.Word()));
        if (r.Chance(30)) d.Set(P.NextBall, d.S16(P.Remaining));
        d.Set(P.ShownTime, (short)(r.Chance(80) ? r.Range(-1, 30) : r.Word()));
        d.Set(L.OilPrice, r.Chance(70) ? (short)r.Range(1, 60) : Pick(r, Prices));
        d.Set(L.RocketPrice, r.Chance(70) ? (short)r.Range(50, 600) : Pick(r, Prices));
        d.Set(L.LottoPrice, r.Chance(70) ? (short)r.Range(1, 40) : Pick(r, Prices));
        d.Set(L.LottoPrize, r.Chance(70) ? (short)r.Range(20, 2000) : Pick(r, Prices));
        d.Set(L.ShopOil, (short)(r.Chance(70) ? r.Range(0, 3) : r.Chance(50) ? r.Range(0, 400) : r.Word()));
        d.Set(L.Week, (short)(r.Chance(90) ? r.Range(0, 60) : r.Word()));
        d.Set(L.GemPriceFactor, r.Chance(50) ? Pick(r, Factors) : Int16Math.S(d.S16(L.Week) + 100 + r.Range(-60, 60)));
        d.Set(L.BaseUnit, r.Chance(80) ? r.Range(0, 2000) : (int)r.Next());
        d.Set(P.LottoX, (short)(0x20 * r.Range(1, 3)));
        d.Set(P.OilX, (short)r.Range(0, 0x140));
        d.Set(P.RocketX, (short)r.Range(0, 0x140));
        d.Set(P.GemX, (short)r.Range(0, 0x140));
        d.Set(P.TextX, r.Word());
        d.Set(P.DrawPage, (short)r.Range(0, 1));
        return d;
    }

    private static void Each(uint seed, ushort off, Action<Dgroup, OriginalRandom, PurchaseHarness.Recorder> port,
        Func<Inputs, ushort[]>? args = null)
    {
        var r = new Inputs(seed);
        for (int i = 0; i < Cases; i++)
        {
            var d = State(r);
            var a = args?.Invoke(r) ?? [];
            H.AssertSame(d, 0x08CF, off, (p, rnd, rec) =>
            {
                port(p, rnd, rec);
                return null;
            }, $"08CF:{off:X4} case {i}", args: a);
        }
    }

    private static void EachResult(uint seed, ushort off, Func<Dgroup, int> port, int mask = 0xFF)
    {
        var r = new Inputs(seed);
        for (int i = 0; i < Cases; i++)
            H.AssertSame(State(r), 0x08CF, off, (p, _, _) => port(p), $"08CF:{off:X4} case {i}", mask);
    }

    private static int B(bool b) => b ? 1 : 0;

    // [DIFFERENTIAL] 08CF:0082
    [GameDataFact]
    public void Layout_of_the_shops() => Each(0x0082, 0x0082, (d, _, _) => R.Layout(d));

    // [DIFFERENTIAL] 08CF:029A
    [GameDataFact]
    public void Street_frame() => Each(0x029A, 0x029A, R.Street);

    // [DIFFERENTIAL] 08CF:039C
    [GameDataFact]
    public void Entering_a_shop() => Each(0x039C, 0x039C, (d, _, o) => R.Enter(d, o));

    // [DIFFERENTIAL] 08CF:03E5
    [GameDataFact]
    public void Computer_choice_in_the_street() => Each(0x03E5, 0x03E5, (d, _, _) => R.ComputerChoice(d));

    // [DIFFERENTIAL] 08CF:0455
    [GameDataFact]
    public void Heading_for_a_shop()
    {
        var r = new Inputs(0x0455);
        for (int i = 0; i < Cases; i++)
        {
            short target = r.Chance(90) ? (short)r.Range(0, 3) : r.Word();
            H.AssertSame(State(r), 0x08CF, 0x0455, (p, _, _) =>
            {
                R.Head(p, target);
                return null;
            }, $"case {i}", args: (ushort)target);
        }
    }

    // [DIFFERENTIAL] 08CF:0492, 08CF:066D, 08CF:09C4, 08CF:0BE1
    [GameDataFact]
    public void Shop_entries()
    {
        Each(0x0492, 0x0492, (d, _, o) => R.EnterOil(d, o));
        Each(0x066D, 0x066D, (d, _, o) => R.EnterGems(d, o));
        Each(0x09C4, 0x09C4, (d, _, o) => R.EnterRockets(d, o));
        Each(0x0BE1, 0x0BE1, (d, _, o) => R.EnterLotto(d, o));
    }

    // [DIFFERENTIAL] 08CF:04BA
    [GameDataFact]
    public void Oil_shop_frame() => Each(0x04BA, 0x04BA, (d, _, o) => R.OilShop(d, o));

    // [DIFFERENTIAL] 08CF:05F1
    [GameDataFact]
    public void Computer_wants_oil() => EachResult(0x05F1, 0x05F1, d => B(R.ComputerWantsOil(d)));

    // [DIFFERENTIAL] 08CF:069F
    [GameDataFact]
    public void Gem_shop_frame() => Each(0x069F, 0x069F, (d, _, o) => R.GemShop(d, o));

    // [DIFFERENTIAL] 08CF:0813, 08CF:084F
    [GameDataFact]
    public void Gem_account_checks()
    {
        EachResult(0x0813, 0x0813, d => B(R.CanSellGem(d)));
        EachResult(0x084F, 0x084F, d => B(R.CanBuyGem(d)));
    }

    // [DIFFERENTIAL] 08CF:0886, 08CF:178D
    [GameDataFact]
    public void Gem_deal_and_count()
    {
        foreach (sbyte dir in new sbyte[] { 1, -1 })
        {
            Each((uint)(0x0886 + dir), 0x0886, (d, _, o) => R.TradeGem(d, dir, o), _ => [(ushort)(byte)dir]);
            Each((uint)(0x178D + dir), 0x178D, (d, _, o) => R.ChangeGems(d, dir, o), _ => [(ushort)(byte)dir]);
        }
    }

    // [DIFFERENTIAL] 08CF:0914
    [GameDataFact]
    public void Computer_gem_choice() => EachResult(0x0914, 0x0914, d => R.ComputerGemChoice(d), 0xFFFF);

    // [DIFFERENTIAL] 08CF:09F1
    [GameDataFact]
    public void Rocket_shop_frame() => Each(0x09F1, 0x09F1, (d, _, o) => R.RocketShop(d, o));

    // [DIFFERENTIAL] 08CF:0AF0
    [GameDataFact]
    public void Computer_wants_a_rocket() => EachResult(0x0AF0, 0x0AF0, d => B(R.ComputerWantsRocket(d)));

    // [DIFFERENTIAL] 08CF:0CD5
    [GameDataFact]
    public void Lotto_frame() => Each(0x0CD5, 0x0CD5, R.Lotto);

    // [DIFFERENTIAL] 08CF:0DEF
    [GameDataFact]
    public void May_play_lotto()
    {
        EachResult(0x0DEF, 0x0DEF, d => B(R.CanPlayLotto(d)));
        var r = new Inputs(0x0DF0);
        for (int i = 0; i < Cases; i++)
        {
            var d = State(r);
            d.Set(P.Preset + 1, (byte)r.Range(0, 2));
            H.AssertSame(d, 0x08CF, 0x0DEF, (p, _, _) => B(R.CanPlayLotto(p)), $"preset case {i}", 0xFF);
        }
    }

    // [DIFFERENTIAL] 08CF:0E53
    [GameDataFact]
    public void Lotto_pick()
    {
        foreach (short n in new short[] { 1, 2, 0, 3 })
            Each((uint)(0x0E53 + n), 0x0E53, (d, _, o) => R.Pick(d, n, o), _ => [(ushort)n]);
    }

    // [DIFFERENTIAL] 08CF:0F4D, 08CF:161F
    [GameDataFact]
    public void Lotto_draw()
    {
        Each(0x0F4D, 0x0F4D, R.Draw);
        Each(0x161F, 0x161F, (d, _, o) => R.NewDraw(d, o));
    }

    // [DIFFERENTIAL] 08CF:115C
    [GameDataFact]
    public void Winner_blink() => Each(0x115C, 0x115C, (d, _, o) => R.Blink(d, o));

    // [DIFFERENTIAL] 08CF:1348
    [GameDataFact]
    public void Street_setup() => Each(0x1348, 0x1348, R.Setup);

    // [DIFFERENTIAL] 08CF:1543
    [GameDataFact]
    public void Price_line()
    {
        var r = new Inputs(0x1543);
        for (int i = 0; i < Cases; i++)
        {
            var d = State(r);
            int text = r.Range(16, 22);
            int value = r.Chance(70) ? r.Range(0, 9999) : (int)r.Next();
            short digits = (short)(r.Chance(90) ? r.Range(1, 5) : r.Range(-3, 30));
            ushort off = d.U16(P.TextTable + 4 * text), seg = d.U16(P.TextTable + 4 * text + 2);
            H.AssertSame(d, 0x08CF, 0x1543, (p, _, o) =>
            {
                R.PriceLine(p, o, text, value, digits);
                return null;
            }, $"case {i}", args: [off, seg, (ushort)value, (ushort)(value >> 16), (ushort)digits]);
        }
    }
}
