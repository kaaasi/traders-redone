using Traders.Core.Rules;
using Traders.Core.State;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Tests.Rules;

/// <summary>
/// Differential tests of wealth, ranking, wealth history and production (round 3, track "regeln", part 2): original in
/// the RefEmu and port on the same data segment; results in DX:AX and the whole data segment must match.
/// </summary>
public class WealthProductionDifferentialTests
{
    private static readonly RulesHarness Harness = new();

    // ------------------------------------------------------------------ inputs

    /// <summary>A plausible game state with occasional extreme values in every field the wealth routines read.</summary>
    private static Dgroup WealthInput(Inputs r, bool extreme)
    {
        var d = Harness.Base(r);
        d.Set(L.ActivePlayer, (sbyte)r.Range(0, 3));
        d.Set(L.LandPrice, extreme && r.Chance(30) ? r.Word() : (short)r.Range(100, 550));
        d.Set(L.RocketPrice, extreme && r.Chance(30) ? r.Word() : (short)r.Range(1, 900));
        d.Set(L.OilPrice, extreme && r.Chance(30) ? r.Word() : (short)r.Range(1, 200));
        d.Set(L.GemTrading, (byte)(r.Chance(50) ? 0 : r.Chance(80) ? 1 : r.Range(2, 255)));
        d.Set(L.OilConsumption, (byte)(r.Chance(40) ? 0 : r.Chance(80) ? 1 : r.Range(2, 255)));
        d.Set(L.BaseUnit, extreme && r.Chance(30) ? (int)r.Next() : r.Range(12, 50));
        d.Set(L.GemPriceFactor, extreme && r.Chance(30) ? (int)r.Next() : r.Range(40, 160));
        for (int w = 0; w < 4; w++) d.Set(L.WarePrice + 2 * w, extreme && r.Chance(30) ? r.Word() : (short)r.Range(1, 60));
        for (int p = 0; p < 4; p++)
        {
            d.Set(L.Cash + 4 * p, extreme && r.Chance(30) ? (int)r.Next() : r.Range(-500, 90_000));
            d.Set(L.FieldCount + p, (byte)(extreme && r.Chance(30) ? r.Range(0, 255) : r.Range(0, 20)));
            d.Set(L.Rockets + 2 * p, extreme && r.Chance(30) ? r.Word() : (short)r.Range(0, 10));
            d.Set(L.Oil + 2 * p, extreme && r.Chance(30) ? r.Word() : (short)r.Range(0, 40));
            d.Set(L.GemCredit + 2 * p, extreme && r.Chance(30) ? r.Word() : (short)r.Range(0, 2000));
            d.Set(L.Gems + p, (sbyte)(extreme && r.Chance(30) ? r.Range(-128, 127) : r.Range(0, 10)));
            for (int k = 0; k < 4; k++)
                d.Set(L.FieldsByKind + 4 * p + k, (sbyte)(extreme && r.Chance(30) ? r.Range(-128, 127) : r.Range(0, 8)));
            for (int w = 0; w < 4; w++)
                d.Set(L.Stock + 8 * p + 2 * w, extreme && r.Chance(30) ? r.Word() : (short)r.Range(0, 999));
        }
        return d;
    }

    // ------------------------------------------------------------------ 0825:0813 and its parts

    private static readonly (ushort Offset, Func<Dgroup, int> Port)[] Parts =
    [
        (0x0813, Wealth.Total), (0x086F, Wealth.Land), (0x088D, Wealth.Wares), (0x08EB, Wealth.Gems), (0x096D, Wealth.Other),
    ];

    // [DIFFERENTIAL] 0825:0813
    // [DIFFERENTIAL] 0825:086F
    // [DIFFERENTIAL] 0825:088D
    // [DIFFERENTIAL] 0825:08EB
    // [DIFFERENTIAL] 0825:096D
    [GameDataFact]
    public void Wealth_and_parts_match_for_many_states()
    {
        var r = new Inputs(0x0813);
        for (int i = 0; i < 300; i++)
        {
            var input = WealthInput(r, extreme: i >= 150);
            foreach (var (offset, port) in Parts)
                Harness.AssertSame(input, 0x0825, offset, port, $"0825:{offset:X4} state {i}");
        }
    }

    // [DIFFERENTIAL] 0825:0813
    // [DIFFERENTIAL] 0825:086F
    // [DIFFERENTIAL] 0825:088D
    // [DIFFERENTIAL] 0825:096D
    [GameDataFact]
    public void Wealth_products_wrap_to_16_bit()
    {
        var r = new Inputs(1);
        var d = WealthInput(r, extreme: false);
        d.Set(L.ActivePlayer, (sbyte)2);
        d.Set(L.FieldCount + 2, (byte)200);              // 200 * 550 = 110000 → low word −20536
        d.Set(L.LandPrice, (short)550);
        d.Set(L.Stock + 16, (short)999);                // 99 * 400 = 39600 → −25936
        d.Set(L.WarePrice, (short)400);
        d.Set(L.Rockets + 4, (short)100);               // 100 * 900 = 90000 → 24464
        d.Set(L.RocketPrice, (short)900);
        d.Set(L.OilConsumption, (byte)1);
        d.Set(L.Oil + 4, (short)-40);
        d.Set(L.OilPrice, (short)1000);
        d.Set(L.Cash + 8, int.MaxValue);                // and a 32-bit wrap of the sum
        foreach (var (offset, port) in Parts) Harness.AssertSame(d, 0x0825, offset, port, $"0825:{offset:X4} wrap");
    }

    // ------------------------------------------------------------------ 0825:06CB ranking

    // [DIFFERENTIAL] 0825:06CB
    [GameDataFact]
    public void Ranking_matches_including_ties()
    {
        var r = new Inputs(0x06CB);
        int[][] cashSets =
        [
            [100, 200, 300, 400], [400, 300, 200, 100], [500, 500, 500, 500], [100, 100, 900, 900],
            [-1, 0, -1, 0], [int.MinValue, int.MaxValue, 0, -1], [7, 3, 7, 3],
        ];
        foreach (var cash in cashSets)
        {
            // Equal side values for everyone, so the cash alone orders the players.
            var d = WealthInput(new Inputs(5), extreme: false);
            for (int p = 0; p < 4; p++)
            {
                d.Set(L.Cash + 4 * p, cash[p]);
                d.Set(L.FieldCount + p, (byte)0);
                d.Set(L.Rockets + 2 * p, (short)0);
                d.Set(L.FieldsByKind + 4 * p + 3, (sbyte)0);
                d.Set(L.Oil + 2 * p, (short)0);
                d.Set(L.Gems + p, (sbyte)0);
                d.Set(L.GemCredit + 2 * p, (short)0);
                for (int w = 0; w < 4; w++) d.Set(L.Stock + 8 * p + 2 * w, (short)0);
                // Ranks start with leftovers that the routine must overwrite.
                d.Set(L.Rank + p, (byte)r.Range(0, 255));
                d.Set(L.RankUnique + p, (byte)r.Range(0, 255));
            }
            Harness.AssertSame(d, 0x0825, 0x06CB, Ranking.Apply, $"ranking {string.Join(",", cash)}");
        }
        for (int i = 0; i < 150; i++)
            Harness.AssertSame(WealthInput(r, extreme: i >= 75), 0x0825, 0x06CB, Ranking.Apply, $"ranking state {i}");
    }

    // ------------------------------------------------------------------ 0825:07C4 wealth history

    // [DIFFERENTIAL] 0825:07C4
    [GameDataFact]
    public void History_slot_for_weeks_and_states()
    {
        var r = new Inputs(0x07C4);
        // Week 40 and 79 wrap to slots 0 and 39; negative weeks stay negative and write in front of the history
        // (−1 → 0x4118, −40 → 0x407C, still inside the data segment image).
        short[] weeks = [0, 1, 39, 40, 41, 79, 80, 119, 400, 1239, -1, -2, -39, -40];
        foreach (short week in weeks)
        {
            var d = WealthInput(r, extreme: false);
            d.Set(L.Week, week);
            Harness.AssertSame(d, 0x0825, 0x07C4, WealthHistoryRecord.Apply, $"week {week}");
        }
        for (int i = 0; i < 100; i++)
        {
            var d = WealthInput(r, extreme: i >= 50);
            d.Set(L.Week, (short)r.Range(0, 200));
            Harness.AssertSame(d, 0x0825, 0x07C4, WealthHistoryRecord.Apply, $"history state {i}");
        }
    }

    // [DIFFERENTIAL] 0825:07C4
    [GameDataFact]
    public void History_for_strongly_negative_weeks_P3_74()
    {
        // #74 R3R-2: the slot offset is 16-bit. Down to week -4167 every player's entry lies in the image (player 0 at
        // DS:0000); from week -4168 player 0 wraps behind the image (DS:FFFC). The original writes there without a fault,
        // the port's data segment ends at 0x58A0 and throws instead (unreachable in a game, the week counts up from 0).
        // The offset repeats every 16384 weeks (e.g. week -16384 lands in the image again).
        var r = new Inputs(0x4167);
        foreach (short week in new short[] { -41, -1000, -4100, -4166, -4167 })
        {
            var d = WealthInput(r, extreme: false);
            d.Set(L.Week, week);
            Harness.AssertSame(d, 0x0825, 0x07C4, WealthHistoryRecord.Apply, $"week {week}");
        }
        foreach (short week in new short[] { -4168, -5000, -8000 })
        {
            var d = WealthInput(r, extreme: false);
            d.Set(L.Week, week);
            Assert.Null(Harness.RunOriginal(d, 0x0825, 0x07C4).Fault);
            Assert.Throws<ArgumentOutOfRangeException>(() => WealthHistoryRecord.Apply(new Dgroup((byte[])d.Bytes.Clone())));
        }
    }

    // ------------------------------------------------------------------ 22A3:0003 production

    private static Dgroup ProductionInput(Inputs r, bool extreme)
    {
        var d = Harness.Base(r);
        short cols = (short)r.Range(4, 9), rows = (short)r.Range(3, 6);
        d.Set(L.FieldColumns, cols);
        d.Set(L.FieldRows, rows);
        d.Set(L.Weather, (sbyte)(extreme && r.Chance(30) ? r.Range(-3, 9) : r.Range(0, 5)));
        d.Set(L.LimitLow, (sbyte)(extreme && r.Chance(30) ? r.Range(-128, 127) : r.Range(3, 30)));
        d.Set(L.ProductionIdle, r.Word());
        d.Set(L.ProductionActive, r.Word());
        d.Set(L.LoopX, (sbyte)r.Range(-128, 127));
        d.Set(L.LoopY, (sbyte)r.Range(-128, 127));
        for (int i = 0; i < 9 * Production.GridStride; i++) d.Set(L.ProductionResult + i, (byte)r.Range(0, 255));
        for (int i = 0; i < 12; i++)
        {
            d.Set(L.StationKind + 2 * i, (short)r.Range(0, 2));
            d.Set(L.StationVisited + i, (byte)r.Range(0, 1));
        }
        for (int p = 0; p < 4; p++)
        {
            d.Set(L.Station + 2 * p, (short)r.Range(-1, 11));
            for (int k = 0; k < 4; k++)
                d.Set(L.FieldsByKind + 4 * p + k, (sbyte)(extreme && r.Chance(30) ? r.Range(-128, 127) : r.Range(0, 12)));
            for (int w = 0; w < 4; w++)
                d.Set(L.Stock + 8 * p + 2 * w, extreme && r.Chance(30) ? r.Word() : (short)r.Range(0, 999));
        }
        int[] kinds = [-2, 0, 1, 2, 3, 4];
        for (int x = 0; x < 9; x++)
            for (int y = 0; y < 6; y++)
            {
                int f = L.FieldIndex(x, y);
                d.Set(L.FieldOwner + f, (sbyte)(r.Chance(30) ? -1 : r.Range(0, 3)));
                d.Set(L.FieldKind + f, (sbyte)kinds[r.Range(0, kinds.Length - 1)]);
                d.Set(L.FieldQuality + f, (sbyte)(extreme && r.Chance(30) ? r.Range(-4, 7) : r.Range(0, 3)));
            }
        return d;
    }

    // [DIFFERENTIAL] 22A3:0750
    [GameDataFact]
    public void Production_shares_match()
    {
        var r = new Inputs(0x0750);
        for (int i = 0; i < 200; i++)
        {
            var d = ProductionInput(r, extreme: i >= 100);
            if (i % 10 == 0)
                for (int k = 0; k < 3; k++) d.Set(L.FieldsByKind + 4 * (i / 10 % 4) + k, (sbyte)0);   // no fields: 0 %
            if (i % 10 == 1)
            {
                d.Set(L.FieldsByKind + 0, (sbyte)5);    // total 0 with nonzero counts
                d.Set(L.FieldsByKind + 1, (sbyte)-5);
                d.Set(L.FieldsByKind + 2, (sbyte)0);
            }
            Harness.AssertSame(d, 0x22A3, 0x0750, Production.Shares, $"shares state {i}");
        }
    }

    // [DIFFERENTIAL] 22A3:0003
    [GameDataFact]
    public void Production_matches_for_many_fields()
    {
        var r = new Inputs(0x0003);
        for (int i = 0; i < 200; i++)
            Harness.AssertSame(ProductionInput(r, extreme: i >= 100), 0x22A3, 0x0003, Production.Apply, $"production state {i}");
    }

    // [DIFFERENTIAL] 22A3:0003
    [GameDataFact]
    public void Production_edge_cases()
    {
        var r = new Inputs(42);
        // Every field owned by player 1 with kind 0 and stock exactly at the limit, one below, and a station of −1.
        foreach (short stock in new short[] { 9, 10, short.MaxValue, short.MinValue })
        {
            var d = ProductionInput(r, extreme: false);
            d.Set(L.LimitLow, (sbyte)10);
            for (int f = 0; f < 54; f++)
            {
                d.Set(L.FieldOwner + f, (sbyte)1);
                d.Set(L.FieldKind + f, (sbyte)(f % 7 == 0 ? 3 : 0));
            }
            d.Set(L.Station + 2, (short)-1);
            int ware = d.S8(L.KindWare);
            d.Set(L.Stock + 8 + 2 * ware, stock);
            Harness.AssertSame(d, 0x22A3, 0x0003, Production.Apply, $"stock {stock}");
        }
        // Grid of 9 x 6 and of 1 x 1; no rows at all leaves the column variable untouched.
        foreach (var (cols, rows) in new (short, short)[] { (9, 6), (1, 1), (5, 0), (0, 4) })
        {
            var d = ProductionInput(r, extreme: true);
            d.Set(L.FieldColumns, cols);
            d.Set(L.FieldRows, rows);
            Harness.AssertSame(d, 0x22A3, 0x0003, Production.Apply, $"grid {cols}x{rows}");
        }
    }
}
