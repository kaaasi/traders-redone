using Traders.Core.State;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Core.Rules;

/// <summary>
/// Two weekly limits from the number of production fields of all players (docs/routines/0609_1178_obergrenzen.md):
/// DS:48A2 = min(30, F/2 + 6), DS:2F26 = min(30, F/2 + 3); F counts kinds 0..2, the rocket kind 3 does not count.
/// </summary>
public static class ProductionLimits
{
    public const sbyte Cap = 30;

    // [ORIGINAL] 0609:1178
    public static void Apply(Dgroup d)
    {
        short fields = 0;
        // Both loops run on the global variables DS:37AA (player) and DS:45D0 (kind); they end at 4 and 3.
        for (sbyte p = 0; p < L.PlayerCount; p++)
        {
            d.Set(L.ActivePlayer, p);
            for (short k = 0; k <= 2; k++)
            {
                d.Set(L.LoopWare, k);
                fields = Int16Math.S(fields + d.S8(L.FieldsByKind + 4 * p + k));
            }
            d.Set(L.LoopWare, (short)3);
        }
        d.Set(L.ActivePlayer, (sbyte)L.PlayerCount);

        // Byte arithmetic with a signed compare: a sum of 125 or more wraps below zero and is not capped.
        sbyte half = unchecked((sbyte)Int16Math.Div(fields, 2));
        d.Set(L.LimitHigh, Capped(half, 6));
        d.Set(L.LimitLow, Capped(half, 3));
    }

    private static sbyte Capped(sbyte half, int add)
    {
        sbyte v = unchecked((sbyte)(half + add));
        return v > Cap ? Cap : v;
    }
}
