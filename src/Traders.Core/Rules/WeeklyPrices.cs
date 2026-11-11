using Traders.Core.State;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Core.Rules;

/// <summary>
/// Weekly prices at the start of every week (docs/routines/1777_02DC_wochenpreise.md): shop oil stock, oil, rocket,
/// lotto and land price from the players' cash, the field counts, the week counter and the lotto setting. No clock,
/// no random numbers.
/// </summary>
public static class WeeklyPrices
{
    /// <summary>Initial value of the running minimum of the players' cash.</summary>
    public const int CashMinimumStart = 1_000_000;

    // [ORIGINAL] 1777:02DC
    public static void Apply(Dgroup d)
    {
        int minCash = CashMinimumStart, sum = 0;
        // The player loop runs on the global variable DS:37AA, which keeps the value 4 afterwards.
        for (sbyte p = 0; p < L.PlayerCount; p++)
        {
            d.Set(L.ActivePlayer, p);
            int cash = d.S32(L.Cash + 4 * p);
            if (cash < minCash) minCash = cash;
            sum = unchecked(sum + cash);
        }
        d.Set(L.ActivePlayer, (sbyte)L.PlayerCount);

        int average = LongMath.Div(sum, 4);

        // Unused fields * 2 + 1 (16-bit, always odd), week counter sign-extended.
        short grid = Int16Math.S(d.S16(L.FieldColumns) * d.S16(L.FieldRows));
        short divisor = Int16Math.S((grid - d.S8(L.FieldTotal)) * 2 + 1);
        int shopOil = LongMath.Mod(LongMath.Mul(sum, d.S16(L.Week)), divisor);
        d.Set(L.ShopOil, Int16Math.S(shopOil));

        // Oil: lowest cash / (stock / 4 * 10 + 200), raised to average / 1000 (32-bit compare), at least 1.
        short oilDivisor = Int16Math.S(Int16Math.Div(d.S16(L.ShopOil), 4) * 10 + 200);
        d.Set(L.OilPrice, Int16Math.S(LongMath.Div(minCash, oilDivisor)));
        int byAverage = LongMath.Div(average, 1000);
        if (byAverage > d.S16(L.OilPrice)) d.Set(L.OilPrice, Int16Math.S(byAverage));
        if (d.S16(L.OilPrice) < 1) d.Set(L.OilPrice, (short)1);

        // Rockets: average / 70 + 1, doubled when the cash sum is a multiple of 5.
        d.Set(L.RocketPrice, Int16Math.S(LongMath.Div(average, 70) + 1));
        if (LongMath.Mod(sum, 5) == 0) d.Set(L.RocketPrice, Int16Math.S(d.S16(L.RocketPrice) << 1));

        // Lotto: average / 20, 10 or 5 for LOW, NORM, HIGH; any other setting keeps the old value.
        int lottoDivisor = d.S8(L.LottoReturns) switch { 0 => 20, 1 => 10, 2 => 5, _ => 0 };
        if (lottoDivisor != 0) d.Set(L.LottoPrize, Int16Math.S(LongMath.Div(average, lottoDivisor)));
        if (d.S16(L.LottoPrize) < 20) d.Set(L.LottoPrize, (short)20);
        d.Set(L.LottoPrice, Int16Math.Div(d.S16(L.LottoPrize), 32));

        // Land: average / 10 clamped to 100..550 (16-bit compares).
        d.Set(L.LandPrice, Int16Math.S(LongMath.Div(average, 10)));
        if (d.S16(L.LandPrice) < 100) d.Set(L.LandPrice, (short)100);
        if (d.S16(L.LandPrice) > 550) d.Set(L.LandPrice, (short)550);
    }
}
