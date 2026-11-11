using Traders.Core.State;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Core.Rules;

/// <summary>
/// Weekly production of every field (docs/routines/22A3_0003_produktion.md): the result per field (4..60) goes to the
/// grid DS:48F4 (stride 26, the same buffer serves as robot battle field, BESTAND B5); a producing field consumes
/// DS:2F26 units of its ware. No random numbers.
/// </summary>
public static class Production
{
    public const int GridStride = 26;
    public const short Minimum = 4, Maximum = 60;

    /// <summary>Share of the production fields per player and kind in percent (byte, 0 without fields).</summary>
    // [ORIGINAL] 22A3:0750
    public static void Shares(Dgroup d)
    {
        for (sbyte p = 0; p < L.PlayerCount; p++)
        {
            d.Set(L.ActivePlayer, p);
            short total = 0;
            for (int k = 0; k <= 2; k++) total = Int16Math.S(total + d.S8(L.FieldsByKind + 4 * p + k));
            for (int k = 0; k <= 2; k++)
            {
                short share = total == 0
                    ? (short)0
                    : Int16Math.Div(Int16Math.S(d.S8(L.FieldsByKind + 4 * p + k) * 100), total);
                d.Set(L.ProductionShares + 3 * p + k, unchecked((byte)share));
            }
        }
        d.Set(L.ActivePlayer, (sbyte)L.PlayerCount);
    }

    // [ORIGINAL] 22A3:0003
    public static void Apply(Dgroup d)
    {
        Shares(d);

        // Yield factor per player and kind: share * 3 / 4 + 25, kept as a byte (a local array in the original).
        var factor = new byte[L.PlayerCount * 3];
        for (int p = 0; p < L.PlayerCount; p++)
            for (int k = 0; k <= 2; k++)
            {
                short v = Int16Math.Div(Int16Math.S(d.U8(L.ProductionShares + 3 * p + k) * 3), 4);
                factor[3 * p + k] = unchecked((byte)(v + 25));
            }
        d.Set(L.ActivePlayer, (sbyte)L.PlayerCount);

        d.Set(L.ProductionIdle, (short)0);
        d.Set(L.ProductionActive, (short)0);
        // Rows outside, columns inside, both on global byte variables (DS:2F00, DS:2EB0) compared as signed words.
        for (d.Set(L.LoopY, (sbyte)0); d.S8(L.LoopY) <= Int16Math.S(d.S16(L.FieldRows) - 1); d.Set(L.LoopY, (sbyte)(d.S8(L.LoopY) + 1)))
            for (d.Set(L.LoopX, (sbyte)0); d.S8(L.LoopX) <= Int16Math.S(d.S16(L.FieldColumns) - 1); d.Set(L.LoopX, (sbyte)(d.S8(L.LoopX) + 1)))
                Field(d, d.S8(L.LoopX), d.S8(L.LoopY), factor);
    }

    private static void Field(Dgroup d, int x, int y, byte[] factor)
    {
        int cell = L.ProductionResult + Int16Math.S(x * GridStride) + y;
        d.Set(cell, (byte)0);
        int field = Int16Math.S(x * L.FieldRowsMax) + y;
        sbyte owner = d.S8(L.FieldOwner + field);
        d.Set(L.ActivePlayer, owner);
        int kind = d.S8(L.FieldKind + field);
        int quality = d.S8(L.FieldQuality + field);

        if (owner == -1) return;
        if (kind == -2)
        {
            Increment(d, L.ProductionIdle);
            return;
        }
        if (kind == 4) return;
        // The original indexes its local factor array with owner and kind; other values read the stack and are not
        // reachable in the game (owners 0..3, kinds -2 and 0..4).
        if (owner is < 0 or >= L.PlayerCount || kind is < 0 or > 3)
            throw new InvalidOperationException($"Field ({x},{y}): owner {owner}, kind {kind} cannot occur in the original.");

        short amount = 0;
        if (kind == 3)
        {
            // Rocket field: marks the player's station as visited, yield from rocket fields and the station kind.
            short station = d.S16(L.Station + 2 * owner);
            d.Set(Int16Math.U(L.StationVisited + station), (byte)1);
            amount = Int16Math.S(d.S8(L.FieldsByKind + 4 * owner + 3) * 6);
            amount = Int16Math.S(amount + Int16Math.S(d.S16(Int16Math.U(L.StationKind + Int16Math.S(station << 1))) * 10));
        }
        else
        {
            int ware = d.S8(L.KindWare + kind);
            int stock = L.Stock + Int16Math.S(owner * 8 + Int16Math.S(ware << 1));
            short need = d.S8(L.LimitLow);
            if (d.S16(stock) < need)
            {
                Increment(d, L.ProductionIdle);
            }
            else
            {
                Increment(d, L.ProductionActive);
                d.Set(stock, Int16Math.S(d.S16(stock) - need));
                amount = Int16Math.S(d.S8(L.FieldsByKind + 4 * owner + kind) * 3);
                amount = Int16Math.S(amount + Int16Math.S(d.S8(L.QualityTable + Int16Math.S((kind << 2) + quality)) * 10));
                amount = Int16Math.Div(Int16Math.S(factor[3 * owner + kind] * amount), 100);
                sbyte weather = d.S8(L.WeatherTable + Int16Math.S(kind * 6 + d.S8(L.Weather)));
                amount = Int16Math.S(amount + Int16Math.Div(Int16Math.S(weather * amount), 100));
            }
        }
        if (amount < Minimum) amount = Minimum;
        if (amount > Maximum) amount = Maximum;
        d.Set(cell, unchecked((byte)amount));
    }

    private static void Increment(Dgroup d, int offset) => d.Set(offset, Int16Math.S(d.S16(offset) + 1));
}
