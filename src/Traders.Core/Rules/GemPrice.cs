using Traders.Core.State;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Core.Rules;

/// <summary>
/// Weekly gem price (docs/routines/08CF_11D7_edelsteinkurs.md): base unit = cash sum / 160, then the factor DS:4436
/// drifts up or down around week + 100 and stays within week + 40 .. week + 160. The week offsets are 16-bit sums,
/// sign-extended for the 32-bit compares; a reset to week + 100 ± random(30) stores the 16-bit sum with high word 0.
/// </summary>
public static class GemPrice
{
    // [ORIGINAL] 08CF:11D7
    public static void Apply(Dgroup d, OriginalRandom random)
    {
        int sum = 0;
        for (sbyte p = 0; p < L.PlayerCount; p++)
        {
            d.Set(L.ActivePlayer, p);
            sum = unchecked(sum + d.S32(L.Cash + 4 * p));
        }
        d.Set(L.ActivePlayer, (sbyte)L.PlayerCount);
        d.Set(L.BaseUnit, LongMath.Div(sum, 160));

        short week = d.S16(L.Week);
        int Factor() => d.S32(L.GemPriceFactor);
        int Week(int add) => Int16Math.S(week + add);

        // Up if the factor is below week + 155 and random(1) says so, or if it is below week + 45.
        bool up = (Week(155) > Factor() && random.Next(1) != 0) || Week(45) > Factor();
        if (up)
        {
            if (Week(100) > Factor())
                d.Set(L.GemPriceFactor, (int)Int16Math.U(week + 100 + random.Next(30)));
            else
                d.Set(L.GemPriceFactor, unchecked(Factor() + random.Next(20)));
            if (Week(160) < Factor()) d.Set(L.GemPriceFactor, Week(160));
        }
        else
        {
            if (Week(100) < Factor())
                d.Set(L.GemPriceFactor, (int)Int16Math.U(week + 100 - random.Next(30)));
            else
                d.Set(L.GemPriceFactor, unchecked(Factor() - random.Next(20)));
            if (Week(40) > Factor()) d.Set(L.GemPriceFactor, Week(40));
        }
    }
}
