using Traders.Core.State;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Core.Rules;

/// <summary>
/// Weekly decay of all stocks to 8/10 (docs/routines/22A3_026B_schwund.md). The product stock * 8 is formed with
/// 16-bit shifts, so stocks from 4096 on wrap (P3 #39 R1); the division truncates toward zero.
/// </summary>
public static class StockDecay
{
    // [ORIGINAL] 22A3:026B
    public static void Apply(Dgroup d)
    {
        // Loops on the global variables DS:37AA (player) and DS:45D0 (ware); they end at 4 and 4.
        for (sbyte p = 0; p < L.PlayerCount; p++)
        {
            d.Set(L.ActivePlayer, p);
            for (short w = 0; w <= 3; w++)
            {
                d.Set(L.LoopWare, w);
                int at = L.Stock + 8 * p + 2 * w;
                short eightfold = Int16Math.S(d.S16(at) << 3);
                d.Set(at, Int16Math.Div(eightfold, 10));
            }
            d.Set(L.LoopWare, (short)4);
        }
        d.Set(L.ActivePlayer, (sbyte)L.PlayerCount);
    }
}
