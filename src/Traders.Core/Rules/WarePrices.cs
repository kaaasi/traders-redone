using Traders.Core.State;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Core.Rules;

/// <summary>
/// Ware prices around the auction (docs/routines/0B9F_06A0_warenpreise.md). No random numbers despite the plan's
/// assumption; both routines are pure arithmetic.
/// </summary>
public static class WarePrices
{
    public const short Ceiling = 300, Floor = 3;

    /// <summary>Raises the price of every ware to its minimum (lowest cash based, ware 3: week / 2 + 3), at most 300.</summary>
    // [ORIGINAL] 0B9F:06A0
    public static void RaiseToMinimum(Dgroup d)
    {
        int minCash = WeeklyPrices.CashMinimumStart;
        for (sbyte p = 0; p < L.PlayerCount; p++)
        {
            d.Set(L.ActivePlayer, p);
            int cash = d.S32(L.Cash + 4 * p);
            if (cash < minCash) minCash = cash;
        }
        d.Set(L.ActivePlayer, (sbyte)L.PlayerCount);

        // Divisor 300 - (10000 - lowest cash) / 58, 16 bit; zero ends in the divide error like the original.
        short divisor = Int16Math.S(Ceiling - LongMath.Div(unchecked(10_000 - minCash), 58));
        for (short w = 0; w <= 3; w++)
        {
            d.Set(L.LoopWare, w);
            short minimum = w == 3
                ? Int16Math.S(Int16Math.Div(d.S16(L.Week), 2) + 3)
                : Int16Math.S(LongMath.Div(minCash, divisor));
            int at = L.WarePrice + 2 * w;
            if (d.S16(at) < minimum) d.Set(at, minimum);
            if (d.S16(at) > Ceiling) d.Set(at, Ceiling);
        }
        d.Set(L.LoopWare, (short)4);
    }

    /// <summary>
    /// New price of the ware in DS:45D0 after its auction: no bid → 7/10 of the start price, bid = start → 9/10,
    /// otherwise start + (bid - start) / 3, clamped to 3..300; ware 3 gets week / 2 + 3 without clamping.
    /// </summary>
    // [ORIGINAL] 0B9F:078A
    public static void AfterAuction(Dgroup d)
    {
        short ware = d.S16(L.LoopWare), price;
        if (ware == 3)
        {
            price = Int16Math.S(Int16Math.Div(d.S16(L.Week), 2) + 3);
        }
        else
        {
            short bid = d.S16(L.AuctionBestBid), start = d.S16(L.AuctionStartPrice);
            if (bid == 0) price = Int16Math.Div(Int16Math.S(start * 7), 10);
            else if (bid == start) price = Int16Math.Div(Int16Math.S(start * 9), 10);
            else price = Int16Math.S(start + Int16Math.Div(Int16Math.S(bid - start), 3));
            if (price < Floor) price = Floor;
            if (price > Ceiling) price = Ceiling;
        }
        d.Set(Int16Math.U(L.WarePrice + Int16Math.S(ware << 1)), price);
    }
}
