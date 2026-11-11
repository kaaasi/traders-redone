using Traders.Core.Rules;
using Traders.Core.State;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Tests.Rules;

/// <summary>
/// Differential tests of weather, gem price and ware prices (round 3, track "regeln", part 3). Random state before and
/// after and the number of draws (calls of 21FD:01BA in the original) are compared with the port.
/// </summary>
public class RandomRulesDifferentialTests
{
    private static readonly RulesHarness Harness = new();

    // ------------------------------------------------------------------ 21FD:021F weather

    // [DIFFERENTIAL] 21FD:021F
    [GameDataFact]
    public void Weather_matches_for_many_seeds_with_two_draws()
    {
        var r = new Inputs(0x021F);
        var seen = new HashSet<int>();
        for (int i = 0; i < 400; i++)
        {
            var d = Harness.Base(r);
            if (i < 3) d.Set(L.RandomSeed, new uint[] { 1, 0, uint.MaxValue }[i]);
            d.Set(L.Weather, (sbyte)r.Range(-128, 127));
            var (ported, draws) = Harness.AssertSameWithRandom(d, 0x21FD, 0x021F, WeatherRule.Apply, $"seed {i}");
            Assert.Equal(new short[] { 2, 4 }, draws);
            seen.Add(ported.S8(L.Weather));
        }
        Assert.Equal(new[] { 0, 1, 2, 3, 4, 5 }, seen.Order());
    }

    // ------------------------------------------------------------------ 08CF:11D7 gem price

    private static Dgroup GemInput(Inputs r, int factor, short week, int[]? cash = null)
    {
        var d = Harness.Base(r);
        d.Set(L.GemPriceFactor, factor);
        d.Set(L.Week, week);
        d.Set(L.BaseUnit, (int)r.Next());
        for (int p = 0; p < 4; p++) d.Set(L.Cash + 4 * p, cash?[p] ?? r.Range(-1000, 80_000));
        return d;
    }

    // [DIFFERENTIAL] 08CF:11D7
    [GameDataFact]
    public void Gem_price_every_branch()
    {
        var r = new Inputs(0x11D7);
        var drawPatterns = new HashSet<string>();
        // Factors around each threshold (week + 40, 45, 100, 155, 160) for several weeks and seeds.
        foreach (short week in new short[] { 0, 1, 9, 29, 100 })
            foreach (int delta in new[] { -200, -61, -60, -56, -55, -54, -1, 0, 1, 54, 55, 56, 59, 60, 61, 200 })
                for (int s = 0; s < 4; s++)
                {
                    var d = GemInput(r, week + 100 + delta, week);
                    var (_, draws) = Harness.AssertSameWithRandom(d, 0x08CF, 0x11D7, GemPrice.Apply, $"week {week} delta {delta} seed {s}");
                    drawPatterns.Add(string.Join(",", draws));
                }
        // Without 16-bit wrap three draw sequences occur: random(1) then 30 or 20, and 30 alone (the drift up by
        // random(20) without random(1) needs a wrapped week + 155, see the edge cases).
        Assert.Equal(new[] { "1,20", "1,30", "30" }, drawPatterns.Order());
    }

    // [DIFFERENTIAL] 08CF:11D7
    [GameDataFact]
    public void Gem_price_edge_cases()
    {
        var r = new Inputs(77);
        int max = int.MaxValue, min = int.MinValue;
        var cases = new (int Factor, short Week, int[]? Cash, string Label)[]
        {
            (40, 0, [500, 500, 500, 500], "start of game"),
            (160, 0, [0, 0, 0, 0], "upper bound, no cash"),
            (int.MinValue, 5, null, "lowest factor"),
            (int.MaxValue, 5, null, "highest factor"),
            (-5, 0, null, "negative factor"),
            (32_000, short.MaxValue, null, "week + 100 wraps to negative"),
            (100, short.MaxValue - 50, null, "week + 155 wraps, + 45 does not"),
            (100, short.MinValue, null, "lowest week"),
            (70_000, 10, [max, max, max, max], "cash sum overflow"),
            (65_600, 32_700, [min, -1, 0, 0], "16-bit reset with high word 0"),
        };
        foreach (var c in cases)
            for (int s = 0; s < 6; s++)
                Harness.AssertSameWithRandom(GemInput(r, c.Factor, c.Week, c.Cash), 0x08CF, 0x11D7, GemPrice.Apply,
                    $"{c.Label} seed {s}");
        // Week 32700: week + 155 and week + 100 wrap negative, week + 45 does not → up by random(20) alone.
        var (_, draws) = Harness.AssertSameWithRandom(GemInput(r, 32_720, 32_700), 0x08CF, 0x11D7, GemPrice.Apply, "only random(20)");
        Assert.Equal(new short[] { 20 }, draws);
    }

    // [DIFFERENTIAL] 08CF:11D7
    [GameDataFact]
    public void Gem_price_follows_over_many_weeks()
    {
        // The factor of one week is the input of the next (and the random state carries over), as in a game.
        var r = new Inputs(5);
        var d = GemInput(r, 40 + r.Range(0, 120), 0);
        for (short week = 0; week < 60; week++)
        {
            d.Set(L.Week, week);
            var (ported, _) = Harness.AssertSameWithRandom(d, 0x08CF, 0x11D7, GemPrice.Apply, $"week {week}");
            d = ported;
        }
    }

    // ------------------------------------------------------------------ 0B9F:06A0 / 0B9F:078A ware prices

    // [DIFFERENTIAL] 0B9F:06A0
    [GameDataFact]
    public void Ware_minimum_prices_without_random()
    {
        var r = new Inputs(0x06A0);
        int faults = 0;
        var cashSets = new int[][]
        {
            [500, 1200, 80_000, 37], [1_000_000, 1_000_000, 1_000_000, 1_000_000], [2_000_000, 3_000_000, 1_500_000, 1_000_001],
            [10_000, 20_000, 30_000, 40_000], [9_999, 0, 50, 7], [-7_000, 0, 0, 0], [-7_420, 0, 0, 0], [-7_500, 0, 0, 0], [int.MinValue, 0, 0, 0],
            [int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue],
        };
        foreach (var cash in cashSets)
            foreach (short week in new short[] { 0, 1, 7, 40, 595, 600, short.MaxValue, -1, short.MinValue })
            {
                var d = Harness.Base(r);
                for (int p = 0; p < 4; p++) d.Set(L.Cash + 4 * p, cash[p]);
                d.Set(L.Week, week);
                for (int w = 0; w < 4; w++) d.Set(L.WarePrice + 2 * w, r.Chance(70) ? (short)r.Range(0, 320) : r.Word());
                if (MinimumDivisorIsZero(d))
                {
                    var original = Harness.RunOriginal(d, 0x0B9F, 0x06A0);
                    Assert.Contains("Divide error", original.Fault?.Message);
                    Assert.Throws<DivideByZeroException>(() => WarePrices.RaiseToMinimum(new Dgroup((byte[])d.Bytes.Clone())));
                    faults++;
                    continue;
                }
                Harness.AssertSame(d, 0x0B9F, 0x06A0, WarePrices.RaiseToMinimum, $"cash {string.Join(",", cash)} week {week}");
            }
        Assert.Equal(9, faults);   // the cash set with lowest cash -7420, every week
    }

    // Input check only: (10000 - lowest cash) / 58 == 300 makes the divisor of the original zero.
    private static bool MinimumDivisorIsZero(Dgroup d)
    {
        int min = 1_000_000;
        for (int p = 0; p < 4; p++) min = Math.Min(min, d.S32(L.Cash + 4 * p));
        return (short)(300 - (int)(unchecked(10_000 - min) / 58L)) == 0;
    }

    // [DIFFERENTIAL] 0B9F:078A
    [GameDataFact]
    public void Ware_price_after_auction_without_random()
    {
        var r = new Inputs(0x078A);
        short[] values = [0, 1, 2, 3, 4, 5, 9, 10, 29, 30, 100, 299, 300, 301, 450, 1000, 4681, 4682, short.MaxValue, -1, -3, short.MinValue];
        foreach (short ware in new short[] { 0, 1, 2, 3, 4, -1 })
            foreach (short start in values)
                foreach (short bid in new short[] { 0, start, (short)(start + 1), (short)(start - 1), (short)(start + 7), 300, -100 })
                {
                    var d = Harness.Base(r);
                    d.Set(L.LoopWare, ware);
                    d.Set(L.AuctionStartPrice, start);
                    d.Set(L.AuctionBestBid, bid);
                    d.Set(L.Week, r.Chance(80) ? (short)r.Range(0, 40) : r.Word());
                    Harness.AssertSame(d, 0x0B9F, 0x078A, WarePrices.AfterAuction, $"ware {ware} start {start} bid {bid}");
                }
    }
}
