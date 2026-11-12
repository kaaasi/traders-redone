using Traders.Core.State;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Core.Rules;

/// <summary>
/// Total wealth of the player in DS:37AA and its parts (docs/routines/0825_0813_vermoegen.md): cash + land + wares +
/// gems + rockets/oil. Several parts form a 16-bit product and sign-extend only its low word (cwd after imul), so
/// large values wrap exactly as in the original.
/// </summary>
public static class Wealth
{
    private static int Player(Dgroup d) => d.S8(L.ActivePlayer);

    // [ORIGINAL] 0825:0813
    public static int Total(Dgroup d)
    {
        int p = Player(d);
        int sum = unchecked(Land(d) + d.S32(L.Cash + 4 * p));
        sum = unchecked(sum + Wares(d));
        sum = unchecked(sum + Gems(d));
        return unchecked(sum + Other(d));
    }

    /// <summary>Land: fields owned * land price (low word) / 3.</summary>
    // [ORIGINAL] 0825:086F
    public static int Land(Dgroup d)
    {
        int p = Player(d);
        short product = Int16Math.S(d.U8(L.FieldCount + p) * d.S16(L.LandPrice));
        return Int16Math.Div(product, 3);
    }

    /// <summary>Wares: sum of (stock / 10 * price) per ware, each product cut to 16 bit; loop variable DS:45D0 ends at 4.</summary>
    // [ORIGINAL] 0825:088D
    public static int Wares(Dgroup d)
    {
        int p = Player(d), sum = 0;
        for (short w = 0; w <= 3; w++)
        {
            d.Set(L.LoopWare, w);
            short tenth = Int16Math.Div(d.S16(L.Stock + 8 * p + 2 * w), 10);
            sum = unchecked(sum + Int16Math.S(tenth * d.S16(L.WarePrice + 2 * w)));
        }
        d.Set(L.LoopWare, (short)4);
        return sum;
    }

    /// <summary>Gems (only with gem trading on): gems * (base * factor / 100) + credit * base / 100, 32 bit.</summary>
    // [ORIGINAL] 0825:08EB
    public static int Gems(Dgroup d)
    {
        if (d.U8(L.GemTrading) == 0) return 0;
        int p = Player(d), unit = d.S32(L.BaseUnit);
        int stone = LongMath.Div(LongMath.Mul(unit, d.S32(L.GemPriceFactor)), 100);
        int stones = LongMath.Mul(d.S8(L.Gems + p), stone);
        int credit = LongMath.Div(LongMath.Mul(unit, d.S16(L.GemCredit + 2 * p)), 100);
        return unchecked(stones + credit);
    }

    /// <summary>
    /// Rockets (bought and rocket fields, kind 3) * rocket price, plus oil * oil price when oil consumption is exactly 1;
    /// both products cut to 16 bit.
    /// </summary>
    // [ORIGINAL] 0825:096D
    public static int Other(Dgroup d)
    {
        int p = Player(d);
        short rockets = Int16Math.S(d.S16(L.Rockets + 2 * p) + d.S8(L.FieldsByKind + 4 * p + 3));
        int sum = Int16Math.S(rockets * d.S16(L.RocketPrice));
        if (d.U8(L.OilConsumption) == 1)
            sum = unchecked(sum + Int16Math.S(d.S16(L.Oil + 2 * p) * d.S16(L.OilPrice)));
        return sum;
    }
}

/// <summary>Ranking of the players by wealth (docs/routines/0825_0813_vermoegen.md, section 0825:06CB).</summary>
public static class Ranking
{
    // [ORIGINAL] 0825:06CB
    public static void Apply(Dgroup d)
    {
        // First loop: both ranks start at 3, wealth of every player into DS:2F06 (loop variable DS:37AA).
        for (sbyte p = 0; p < L.PlayerCount; p++)
        {
            d.Set(L.ActivePlayer, p);
            d.Set(L.Rank + p, (byte)3);
            d.Set(L.RankUnique + p, (byte)3);
            d.Set(L.CurrentWealth + 4 * p, Wealth.Total(d));
        }
        // Pairs p < q: Rank drops for everyone not below the other (ties lower both); RankUnique drops for exactly one,
        // the lower player number on a tie.
        for (sbyte p = 0; p < L.PlayerCount; p++)
        {
            d.Set(L.ActivePlayer, p);
            for (int q = p + 1; q <= 3; q++)
            {
                int wp = d.S32(L.CurrentWealth + 4 * p), wq = d.S32(L.CurrentWealth + 4 * q);
                if (wp >= wq)
                {
                    Decrement(d, L.Rank + p);
                    Decrement(d, L.RankUnique + p);
                }
                else
                {
                    Decrement(d, L.RankUnique + q);
                }
                if (wq >= wp) Decrement(d, L.Rank + q);
            }
        }
        d.Set(L.ActivePlayer, (sbyte)L.PlayerCount);
    }

    private static void Decrement(Dgroup d, int offset) => d.Set(offset, unchecked((byte)(d.U8(offset) - 1)));
}

/// <summary>Weekly entry of every player's wealth in the 40-week history (docs/routines/0825_0813_vermoegen.md).</summary>
public static class WealthHistoryRecord
{
    // [ORIGINAL] 0825:07C4
    public static void Apply(Dgroup d)
    {
        // Slot = week reduced by 40 while above 39 (a negative week stays negative, 16-bit offsets).
        short slot = d.S16(L.Week);
        while (slot > 39) slot = Int16Math.S(slot - 40);
        for (sbyte p = 0; p < L.PlayerCount; p++)
        {
            d.Set(L.ActivePlayer, p);
            int wealth = Wealth.Total(d);
            ushort offset = Int16Math.U(L.WealthHistory + Int16Math.S(p * 0xA0) + Int16Math.S(slot << 2));
            // [APPROX] offsets behind the data segment image (from week -4168, #74 R3R-2) throw instead of writing
            // memory beyond DGROUP as the original does; unreachable in a game because the week counts up from 0.
            d.Set(offset, wealth);
        }
        d.Set(L.ActivePlayer, (sbyte)L.PlayerCount);
    }
}
