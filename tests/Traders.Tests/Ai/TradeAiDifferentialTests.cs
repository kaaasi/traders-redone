using Traders.Core.State;
using Traders.Tests.Rules;
using L = Traders.Core.State.DgroupLayout;
using T = Traders.Core.Ai.TradeAiLayout;

namespace Traders.Tests.Ai;

/// <summary>
/// Differential tests of the trade AI 141D (docs/routines/141D_0008_handelski.md). Inputs are data segments taken at the seven
/// entries in the default game, weeks 0..3 (<see cref="TradeAiGame"/>), and random variations of them within values the game
/// can hold, with human and computer seats, buyers and sellers, shop deals and the stale word of 0397. Original and port run
/// on the same bytes; the whole data segment, DS:10D2 and the draws must match. Expected values come only from the original.
/// </summary>
public class TradeAiDifferentialTests
{
    /// <summary>
    /// [DIFFERENTIAL] 141D:0008 / 021A / 0397 / 0464 / 09F3 / 0CCA / 0E67 – single calls on the entry states of the default
    /// game, weeks 0..3 (all computer players, every ware): every entry in every week it is called, 0CCA from week 1 on.
    /// </summary>
    [GameDataFact]
    public void Entries_match_the_original_on_states_of_weeks_0_to_3()
    {
        var game = TradeAiGame.Default.Value;
        var h = new TradeAiHarness();
        foreach (var c in game.Samples)
            h.AssertSame(c.Entry, c.Copy(), c.ToString(), c.Argument, c.Stale);
        foreach (var entry in Enum.GetValues<TradeAiEntry>())
            for (int week = entry == TradeAiEntry.Bought ? 1 : 0; week < TradeAiGame.Weeks; week++)
                Assert.Contains(game.Samples, c => c.Entry == entry && c.Week == week);
        Assert.Contains(game.Samples, c => c.Entry == TradeAiEntry.Demand && !c.External);
    }

    /// <summary>
    /// [DIFFERENTIAL] 141D:0008 / 021A / 0397 / 0464 / 09F3 / 0CCA / 0E67 – the game states with random variations of
    /// everything 141D reads (seats, sides, cash, stocks, needs, limits, prices, positions, countdown, deals, bid tables,
    /// random state, stale word), with the final word of 0397's local against the port's return value (R7H-1); every listed path (<see cref="TradeAiHarness.Branches"/>) is passed, 0397 and 09F3 draw in
    /// some runs and not in others. Counter-checks (local): a draw of 0397 left out, `&lt;` instead of `&lt;=` in 05A3, buyer
    /// and seller swapped in 0CCA → red.
    /// </summary>
    // Every inner routine is checked through the entries (collective tag, every address counts):
    // [DIFFERENTIAL] 141D:0029 / 008C / 04A5 / 05A3 / 0806 / 0A1D / 0BFD / 0E09 / 0ED7 / 0F01
    [GameDataFact]
    public void Entries_match_the_original_on_varied_states()
    {
        var samples = TradeAiGame.Default.Value.Samples;
        var h = new TradeAiHarness();
        var r = new Inputs(0x141D);
        var entries = Enum.GetValues<TradeAiEntry>();
        for (int i = 0; i < 2400; i++)
        {
            var entry = entries[i % entries.Length];
            var source = samples.Where(c => c.Entry == entry).ToList();
            var c = source.Count > 0 ? source[r.Range(0, source.Count - 1)] : samples[r.Range(0, samples.Count - 1)];
            var d = c.Copy();
            Vary(d, r, entry);
            short argument = (short)r.Range(-20, 400);
            short stale = r.Chance(50) ? (short)r.Range(-2, 12) : r.Word();
            h.AssertSame(entry, d, $"variation {i} of {c}", argument, stale);
        }
        var missing = TradeAiHarness.Branches.Where(b => !h.Hits.Contains(b.Offset)).Select(b => $"{b.Offset:X4} {b.Path}").ToList();
        Assert.True(missing.Count == 0, "paths not exercised: " + string.Join(", ", missing));
        foreach (var entry in new[] { TradeAiEntry.ChooseSide, TradeAiEntry.Act })
        {
            var (with, without) = h.Draws[entry];
            Assert.True(with > 0 && without > 0, $"{entry}: runs with draws {with}, without {without}");
        }
    }

    /// <summary>
    /// [DIFFERENTIAL] 141D:0008 / 021A / 0397 / 0464 / 09F3 / 0CCA / 0E67 – lockstep at the call sites (part 2, multi-round
    /// probe): in weeks 0..3 of the default game (four computers) and of the same game with player 0 human during the trades,
    /// every call from 0B9F/0C29 is ported on its entry state and equals the original at its return (whole data segment except
    /// the interrupt-owned bytes, DS:10D2); the number of compared calls equals the number of calls. The human game passes the
    /// human branch of 021A; the stale word of 0397 is not always -1 (finding, "Uninitialised local").
    /// </summary>
    [GameDataTheory]
    [InlineData(0)]
    [InlineData(1)]
    public void Every_call_of_weeks_0_to_3_matches_the_original_at_its_call_site(int humans)
    {
        var game = humans == 0 ? TradeAiGame.Default.Value : TradeAiGame.WithHuman.Value;
        Assert.True(game.Differences.Count == 0, string.Join("\n", game.Differences.Take(10)));
        foreach (var ((entry, week), n) in game.Counts)
        {
            int inner = entry == TradeAiEntry.Demand ? game.Samples.Count(c => c.Entry == entry && c.Week == week && !c.External) : 0;
            Assert.True(game.Compared.GetValueOrDefault((entry, week)) == n - inner, $"{entry} week {week}: {n} calls, {inner} inner");
        }
        for (int week = 0; week < TradeAiGame.Weeks; week++)
            foreach (var entry in Enum.GetValues<TradeAiEntry>().Where(e => e != TradeAiEntry.Bought || week > 0))
                Assert.True(game.Compared.GetValueOrDefault((entry, week)) > 0, $"{entry} not compared in week {week}");
        Assert.Contains(game.StaleWords.Keys, w => w != -1);
        // R7H-1 (#192): every 0397 call's final local word equals the port's return value.
        Assert.Equal(Enumerable.Range(0, TradeAiGame.Weeks).Sum(w => game.Counts[(TradeAiEntry.ChooseSide, w)]), game.ReturnsCompared);
        if (humans > 0)
            Assert.Contains(game.Samples, c => c.Entry == TradeAiEntry.Demand && c.External && c.Data[L.IsComputer + c.Player] == 0);
    }

    /// <summary>Random variation of what 141D reads, within values the game can hold; divisors of the original stay non-zero.</summary>
    private static void Vary(Dgroup d, Inputs r, TradeAiEntry entry)
    {
        for (int p = 0; p < L.PlayerCount; p++)
        {
            if (r.Chance(30)) d.Set(L.IsComputer + p, (byte)(r.Chance(70) ? 1 : 0));
            if (r.Chance(40)) d.Set(T.Side + p, (byte)(r.Chance(10) ? 2 : r.Range(0, 1)));
            if (r.Chance(40)) d.Set(T.Position + 2 * p, (short)(r.Chance(30) ? r.Range(-3, 3) : r.Range(-60, 110)));
            if (r.Chance(40))
            {
                int cash = r.Chance(20) ? r.Range(-2000, 100) : r.Chance(20) ? r.Range(0, 2_000_000) : r.Range(0, 20000);
                d.Set(L.Cash + 4 * p, cash);
            }
            for (int w = 0; w < L.WareCount; w++)
            {
                if (r.Chance(30)) d.Set(L.Stock + 8 * p + 2 * w, (short)r.Range(0, 999));
                if (r.Chance(30)) d.Set(T.WareNeed + 8 * p + 2 * w, (short)r.Range(-50, 999));
                if (r.Chance(30)) d.Set(T.PriceLimit + 8 * p + 2 * w, (short)r.Range(0, 400));
                if (r.Chance(20)) d.Set(L.FieldsByKind + 4 * p + w, (sbyte)r.Range(0, 20));
                if (r.Chance(25)) d.Set(T.BidLevel + 16 * p + 4 * w, r.Chance(10) ? r.Range(-5, 5) : r.Range(1, 400));
                if (r.Chance(25)) d.Set(T.BidUnits + 8 * p + 2 * w, (short)(r.Chance(50) ? 10 * r.Range(-1, 6) : r.Range(-20, 60)));
                if (r.Chance(25)) d.Set(T.WareBudget + 16 * p + 4 * w, r.Range(-100, 30000));
            }
            if (r.Chance(30)) d.Set(T.Need + 2 * p, (short)r.Range(-10, 400));
            if (r.Chance(30)) d.Set(T.Reserve + 2 * p, (short)r.Range(-10, 300));
            if (r.Chance(30)) d.Set(T.Surplus + 2 * p, (short)(r.Chance(20) ? r.Range(550, 900) : r.Range(-10, 120)));
            if (r.Chance(30)) d.Set(T.CurrentLevel + 2 * p, (short)r.Range(0, 3));
            if (r.Chance(30)) d.Set(T.CurrentBid + 4 * p, r.Range(0, 400));
            if (r.Chance(20)) d.Set(T.Budget + 4 * p, r.Chance(20) ? r.Range(-30, 30) : r.Range(0, 600));
            if (r.Chance(30)) d.Set(T.NoFieldFound + p, (byte)r.Range(0, 1));
            if (r.Chance(30)) d.Set(L.Rank + p, (byte)r.Range(0, 3));
            if (r.Chance(30)) d.Set(T.Ware3Rank + 2 * p, (short)r.Range(0, 3));
        }
        for (int i = 0; i < 16; i++)
        {
            if (r.Chance(15)) d.Set(T.SortedBid + 4 * i, r.Range(0, 400));
            if (r.Chance(15)) d.Set(T.SortedUnits + 2 * i, (short)(10 * r.Range(-1, 3)));
        }
        for (int w = 0; w < L.WareCount; w++)
            if (r.Chance(30)) d.Set(L.WarePrice + 2 * w, (short)(r.Chance(70) ? r.Range(1, 120) : r.Range(1, 999)));
        if (r.Chance(60)) d.Set(L.ActivePlayer, (sbyte)r.Range(0, 3));
        if (r.Chance(50)) d.Set(L.LoopWare, (short)r.Range(0, 3));
        if (r.Chance(30)) d.Set(L.LimitLow, (sbyte)r.Range(0, 30));
        if (r.Chance(30)) d.Set(L.FieldTotal, (sbyte)r.Range(0, 54));
        if (r.Chance(30)) d.Set(T.TargetPlayer, (sbyte)r.Range(-1, 3));
        if (r.Chance(50))
        {
            short countdown = (short)(r.Chance(60) ? 4 * r.Range(0, 10) : r.Range(0, 40));
            d.Set(T.Countdown, countdown);
            short a = (short)r.Range(0, 5);
            d.Set(T.CountdownA, a);
            d.Set(T.CountdownB, r.Chance(70) ? a : (short)r.Range(0, 5));
            d.Set(T.LastSale, (short)(countdown + r.Range(-2, 16)));
        }
        if (r.Chance(40)) d.Set(T.Spread, (short)r.Range(-20, 120));
        if (r.Chance(40)) d.Set(T.MarketPrice, (short)r.Range(1, 300));
        if (r.Chance(40) || d.S8(T.Sellers) <= 0) d.Set(T.Sellers, (sbyte)r.Range(1, 3)); // 0A1D divides by it and by it + 1
        if (r.Chance(40)) d.Set(T.BuyerFlag, (byte)r.Range(0, 1));
        if (r.Chance(40)) d.Set(T.HighestBid, r.Range(0, 400));
        if (r.Chance(40)) d.Set(T.AskFloor, r.Range(0, 400));
        if (r.Chance(40)) d.Set(T.SellerSurplus, (short)r.Range(0, 900));
        if (r.Chance(40)) d.Set(T.SortedIndex, (short)r.Range(0, 17));
        if (r.Chance(50)) d.Set(T.Buyer, (short)(r.Chance(15) ? 10 : r.Range(0, 3)));
        if (r.Chance(50)) d.Set(T.Seller, (short)r.Range(0, 3));
        if (r.Chance(70)) d.Set(L.RandomSeed, r.Next());
        // The buyer of a 0CCA call and the player of 0397/09F3 are computers in the game; humans still take their paths.
        if (entry is TradeAiEntry.ChooseSide or TradeAiEntry.Act && r.Chance(80)) d.Set(L.IsComputer + d.S8(L.ActivePlayer), (byte)1);
        int player = d.S8(L.ActivePlayer);
        if (entry == TradeAiEntry.ChooseSide && r.Chance(25)) d.Set(T.TargetPlayer, (sbyte)player);
        if (entry == TradeAiEntry.Act && r.Chance(20)) // seller exactly on the ask floor (0BFD)
            d.Set(T.AskFloor, (int)(short)(d.S16(T.Position + 2 * player) / 2 + d.S16(T.MarketPrice)));
    }
}
