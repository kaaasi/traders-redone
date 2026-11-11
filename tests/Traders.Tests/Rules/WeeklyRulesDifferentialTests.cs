using Traders.Core.Rules;
using Traders.Core.State;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Tests.Rules;

/// <summary>
/// Differential tests of the weekly rules without random numbers (round 3, track "regeln", part 1): the original routine
/// runs in the RefEmu on a prepared data segment, the port on the same bytes; the whole data segment must match.
/// </summary>
public class WeeklyRulesDifferentialTests
{
    private static readonly RulesHarness Harness = new();

    /// <summary>Test-local generator for input states (not the game's random()).</summary>
    private sealed class Inputs(uint seed)
    {
        private uint _s = seed;
        public uint Next() => _s = _s * 1664525 + 1013904223;
        public int Range(int lo, int hi) => lo + (int)(Next() % (uint)(hi - lo + 1));
        public short Word() => (short)(Next() >> 16);
        public bool Chance(int percent) => Next() % 100 < (uint)percent;
    }

    private static Dgroup Base(Inputs r)
    {
        var d = Harness.Initial();
        // Loop variables and random state hold arbitrary values before the call; the routines must not consume random().
        d.Set(L.ActivePlayer, (sbyte)r.Range(-128, 127));
        d.Set(L.LoopWare, r.Word());
        d.Set(L.RandomSeed, r.Next());
        return d;
    }

    // ------------------------------------------------------------------ 1777:02DC weekly prices

    private static Dgroup PricesInput(int[] cash, short week, short columns, short rows, sbyte total, sbyte lotto,
        short lottoPrizeBefore = 0, Inputs? r = null)
    {
        var d = Base(r ?? new Inputs(7));
        for (int p = 0; p < 4; p++) d.Set(L.Cash + 4 * p, cash[p]);
        d.Set(L.Week, week);
        d.Set(L.FieldColumns, columns);
        d.Set(L.FieldRows, rows);
        d.Set(L.FieldTotal, total);
        d.Set(L.LottoReturns, lotto);
        d.Set(L.LottoPrize, lottoPrizeBefore);
        return d;
    }

    private static Dgroup Prices(Dgroup input, string label) =>
        Harness.AssertSame(input, 0x1777, 0x02DC, WeeklyPrices.Apply, label);

    // [DIFFERENTIAL] 1777:02DC
    [GameDataFact]
    public void Weekly_prices_probe_P2()
    {
        var d = Prices(PricesInput([500, 1200, 80000, 37], 7, 8, 6, 48, 1), "P2");
        Assert.Equal(20, d.S16(L.OilPrice));
        Assert.Equal(292, d.S16(L.RocketPrice));
        Assert.Equal(63, d.S16(L.LottoPrice));
        Assert.Equal(550, d.S16(L.LandPrice));
        Assert.Equal(4, d.S8(L.ActivePlayer));
    }

    // [DIFFERENTIAL] 1777:02DC
    [GameDataTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(-1)]
    [InlineData(127)]
    public void Weekly_prices_for_every_lotto_setting(int lotto)
    {
        // Settings outside 0..2 keep the previous lotto prize (here 777 and 5, the latter raised to 20).
        Prices(PricesInput([2000, 2000, 2000, 2000], 3, 8, 6, 30, (sbyte)lotto, 777), $"lotto {lotto}");
        Prices(PricesInput([2000, 2000, 2000, 2000], 3, 8, 6, 30, (sbyte)lotto, 5), $"lotto {lotto}, prize 5");
        Prices(PricesInput([9_999_999, 640, 0, -5], 11, 9, 6, 1, (sbyte)lotto, -300), $"lotto {lotto}, mixed cash");
    }

    // [DIFFERENTIAL] 1777:02DC
    [GameDataFact]
    public void Weekly_prices_edge_cases()
    {
        int max = int.MaxValue, min = int.MinValue;
        var cases = new (int[] Cash, short Week, short Cols, short Rows, sbyte Total, string Label)[]
        {
            ([1_000_000, 1_000_000, 1_000_000, 1_000_000], 0, 8, 6, 48, "minimum start value reached exactly"),
            ([1_000_001, 2_000_000, 5_000_000, 1_000_001], 1, 8, 6, 48, "all above the minimum start value"),
            ([999_999, 1_000_000, 1_000_001, 0], 2, 8, 6, 48, "minimum just below start value"),
            ([5, 5, 5, 5], 4, 8, 6, 0, "sum multiple of 5 doubles rockets"),
            ([0, 0, 0, 0], 0, 4, 3, 12, "no cash at all"),
            ([-1000, -20000, -5, -77], 9, 8, 6, 20, "negative cash"),
            ([max, max, max, max], 39, 9, 6, 54, "32-bit overflow of the cash sum"),
            ([min, min, min, min], 1, 9, 6, 54, "sum wraps to zero"),
            ([min, -1, 0, 0], 1, 9, 6, 54, "lowest possible cash"),
            ([400_000_000, 400_000_000, 400_000_000, 400_000_000], 20, 9, 6, 54, "prices beyond 16 bit"),
            ([13_000, 22_000, 31_000, 1], short.MaxValue, 9, 6, -3, "largest week, negative field total"),
            ([13_000, 22_000, 31_000, 1], short.MinValue, 9, 6, 127, "negative week"),
            ([13_000, 22_000, 31_000, 1], 7, 300, 300, 0, "16-bit wrap of columns*rows"),
            ([123_457, 9_876, 31_000, 44], 7, 0, 0, 0, "divisor 1 from empty grid"),
            ([123_457, 9_876, 31_000, 44], 7, 1, 0, 1, "divisor -1"),
            ([min, 0, 0, 0], -1, 1, 0, 1, "MinValue modulo -1"),
            ([1, 2, 3, 4], 5, 16384, 1, 0, "divisor wraps to -32767"),
            ([-80_000, 0, 0, 0], 1, 30_000, 1, 0, "negative shop oil stock"),
        };
        foreach (var c in cases)
            foreach (sbyte lotto in new sbyte[] { 0, 1, 2 })
                Prices(PricesInput(c.Cash, c.Week, c.Cols, c.Rows, c.Total, lotto), $"{c.Label}, lotto {lotto}");
    }

    // [DIFFERENTIAL] 1777:02DC
    [GameDataFact]
    public void Weekly_prices_random_states()
    {
        var r = new Inputs(0x02DC);
        for (int i = 0; i < 400; i++)
        {
            int[] cash = new int[4];
            for (int p = 0; p < 4; p++)
                cash[p] = r.Chance(70) ? r.Range(-2000, 120_000) : (int)r.Next();
            short week = r.Chance(80) ? (short)r.Range(0, 60) : r.Word();
            short cols = r.Chance(80) ? (short)r.Range(4, 9) : r.Word();
            short rows = r.Chance(80) ? (short)r.Range(3, 6) : r.Word();
            sbyte total = r.Chance(80) ? (sbyte)r.Range(0, 54) : (sbyte)r.Range(-128, 127);
            sbyte lotto = r.Chance(90) ? (sbyte)r.Range(0, 2) : (sbyte)r.Range(-128, 127);
            var input = PricesInput(cash, week, cols, rows, total, lotto, r.Word(), r);
            if (OilDivisorIsZero(input)) continue;
            Prices(input, $"random state {i}");
        }
    }

    // [DIFFERENTIAL] 1777:02DC
    [GameDataFact]
    public void Weekly_prices_divide_by_zero_in_both()
    {
        // 100 columns * 1 row, no fields used: divisor 201; sum -80 in week 1 gives shop oil -80 and oil divisor
        // (-80 / 4) * 10 + 200 = 0. The original ends in the divide error, the port throws.
        var input = PricesInput([-80, 0, 0, 0], 1, 100, 1, 0, 1);
        Assert.True(OilDivisorIsZero(input));
        var original = Harness.RunOriginal(input, 0x1777, 0x02DC);
        Assert.NotNull(original.Fault);
        Assert.Contains("Divide error", original.Fault!.Message);
        Assert.Equal(-80, RulesHarness.Word(original.DataSegment, L.ShopOil));
        var ported = new Dgroup((byte[])input.Bytes.Clone());
        Assert.Throws<DivideByZeroException>(() => WeeklyPrices.Apply(ported));
        Assert.Equal(-80, ported.S16(L.ShopOil));
    }

    // Input check only (no expected value): would the oil divisor of the original be zero?
    private static bool OilDivisorIsZero(Dgroup d)
    {
        int sum = 0;
        for (int p = 0; p < 4; p++) sum = unchecked(sum + d.S32(L.Cash + 4 * p));
        short divisor = (short)(((short)(d.S16(L.FieldColumns) * d.S16(L.FieldRows)) - d.S8(L.FieldTotal)) * 2 + 1);
        short stock = (short)((long)unchecked(sum * d.S16(L.Week)) % divisor);
        return (short)(stock / 4 * 10 + 200) == 0;
    }

    // ------------------------------------------------------------------ 0609:1178 limits

    private static Dgroup Limits(Dgroup input, string label) =>
        Harness.AssertSame(input, 0x0609, 0x1178, ProductionLimits.Apply, label);

    private static Dgroup LimitsInput(Func<int, int, sbyte> fields, Inputs r)
    {
        var d = Base(r);
        d.Set(L.LimitLow, (sbyte)r.Range(-128, 127));
        d.Set(L.LimitHigh, (sbyte)r.Range(-128, 127));
        for (int p = 0; p < 4; p++)
            for (int k = 0; k < 4; k++)
                d.Set(L.FieldsByKind + 4 * p + k, fields(p, k));
        return d;
    }

    // [DIFFERENTIAL] 0609:1178
    [GameDataFact]
    public void Limits_probe_P3()
    {
        // Probe P3: k fields in kinds 0..2 for every player, kind 3 = 7 (does not count).
        int[] ks = [0, 1, 2, 5, 9];
        sbyte[] low = [3, 9, 15, 30, 30], high = [6, 12, 18, 30, 30];
        for (int i = 0; i < ks.Length; i++)
        {
            int k = ks[i];
            var d = Limits(LimitsInput((_, kind) => (sbyte)(kind == 3 ? 7 : k), new Inputs((uint)i)), $"P3 k={k}");
            Assert.Equal(low[i], d.S8(L.LimitLow));
            Assert.Equal(high[i], d.S8(L.LimitHigh));
            Assert.Equal(3, d.S16(L.LoopWare));
        }
    }

    // [DIFFERENTIAL] 0609:1178
    [GameDataFact]
    public void Limits_edge_cases_and_random_states()
    {
        // Sums around the byte wrap (half + 6 > 127), negative counts and the extremes of the signed bytes.
        int[] totals = [0, 1, 47, 48, 49, 53, 54, 241, 242, 247, 248, 249, 250, 254, 255, 256, 300, 500, 508, -1, -2, -13, -512];
        foreach (int total in totals)
        {
            var d = LimitsInput((p, kind) => kind == 3 ? (sbyte)-128 : Spread(total, p, kind), new Inputs((uint)total));
            Limits(d, $"sum {total}");
        }
        var r = new Inputs(0x1178);
        for (int i = 0; i < 300; i++)
            Limits(LimitsInput((_, _) => r.Chance(80) ? (sbyte)r.Range(0, 20) : (sbyte)r.Range(-128, 127), r), $"random state {i}");
    }

    // Distributes a total over the 12 counted bytes (kinds 0..2 of four players), each within sbyte range.
    private static sbyte Spread(int total, int p, int kind)
    {
        int index = p * 3 + kind, share = total / 12, rest = total % 12;
        return (sbyte)(share + (Math.Abs(rest) > index ? Math.Sign(rest) : 0));
    }

    // ------------------------------------------------------------------ 22A3:026B stock decay

    private static Dgroup Decay(Dgroup input, string label) =>
        Harness.AssertSame(input, 0x22A3, 0x026B, StockDecay.Apply, label);

    private static Dgroup DecayInput(Func<int, int, short> stock, Inputs r)
    {
        var d = Base(r);
        for (int p = 0; p < 4; p++)
            for (int w = 0; w < 4; w++)
                d.Set(L.Stock + 8 * p + 2 * w, stock(p, w));
        return d;
    }

    // [DIFFERENTIAL] 22A3:026B
    [GameDataFact]
    public void Decay_probe_P3()
    {
        short[] before = [70, 999, 5, 13], after = [56, 799, 4, 10];
        var d = Decay(DecayInput((p, w) => p == 0 ? before[w] : (short)0, new Inputs(3)), "P3");
        for (int w = 0; w < 4; w++) Assert.Equal(after[w], d.S16(L.Stock + 2 * w));
        Assert.Equal(4, d.S16(L.LoopWare));
        Assert.Equal(4, d.S8(L.ActivePlayer));
    }

    // [DIFFERENTIAL] 22A3:026B
    [GameDataFact]
    public void Decay_wraps_from_4096_P3_39_R1()
    {
        // P3 #39 R1: stock * 8 is 16 bit, so 4095 still decays to 3276 while 4096 turns into -3276.
        short[] stocks = [0, 1, 9, 10, 11, 999, 1000, 4095, 4096, 4097, 8191, 8192, 12287, 12288, 16383, 16384,
            32767, -1, -9, -10, -4096, -4097, short.MinValue, -32767];
        for (int i = 0; i < stocks.Length; i += 16)
        {
            int start = i;
            var d = Decay(DecayInput((p, w) => stocks[Math.Min(start + p * 4 + w, stocks.Length - 1)], new Inputs((uint)i)),
                $"stocks from {i}");
            if (start == 0)
            {
                Assert.Equal(3276, d.S16(L.Stock + 8 * 1 + 2 * 3));   // 4095
                Assert.Equal(-3276, d.S16(L.Stock + 8 * 2 + 2 * 0));  // 4096
            }
        }
        var r = new Inputs(0x026B);
        for (int i = 0; i < 200; i++)
            Decay(DecayInput((_, _) => r.Chance(70) ? (short)r.Range(0, 999) : r.Word(), r), $"random state {i}");
    }
}
