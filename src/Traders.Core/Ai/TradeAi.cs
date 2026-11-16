using Traders.Core.Input;
using Traders.Core.Rules;
using Traders.Core.State;
using L = Traders.Core.State.DgroupLayout;
using T = Traders.Core.Ai.TradeAiLayout;

namespace Traders.Core.Ai;

/// <summary>
/// Trade AI of the computer players, module 141D (docs/routines/141D_0008_handelski.md). The trade 0B9F/0C29 calls seven
/// entries; each works on the data segment like the original, including the global loop variables DS:37AA (player) and
/// DS:45D0 (ware), and sets the joystick action DS:3308[p] the trade then reads (2 = up, 3 = down, -1 = none). Addresses
/// are computed in 16 bits as in the original, so out-of-range indices (shop 10 as buyer or seller) read the same bytes.
/// Only <see cref="ChooseSide"/> (random(3) twice) and <see cref="Act"/> (random(400) once) draw.
/// </summary>
public static class TradeAi
{
    /// <summary>
    /// 141D:0008 – budgets of all four players for the current ware: 0029 and 008C for DS:37AA = 0..3. Leaves DS:37AA = 4 and
    /// DS:45D0 = 4. Called by 0B9F:0002 at the start of every ware.
    /// </summary>
    // [ORIGINAL] 141D:0008
    public static void Budgets(Dgroup d)
    {
        for (SetPlayer(d, 0); Player(d) < 4; SetPlayer(d, Player(d) + 1))
        {
            Budget(d);
            WareBudgets(d);
        }
    }

    /// <summary>
    /// 141D:0397 – side choice of computer player DS:37AA in 0B9F:00FE: DS:3308[p] = 2 (become seller), 3 (become buyer) or
    /// -1. In one branch (countdown &gt; 4, both random(3) not 0) the original leaves its local variable uninitialised and
    /// stores the stack word it finds; <paramref name="staleLocal"/> is that word (finding, section "Uninitialised local").
    /// Returns the final value of the local, which stays on the stack for the next call at the same depth.
    /// </summary>
    // [ORIGINAL] 141D:0397
    public static short ChooseSide(Dgroup d, OriginalRandom random, short staleLocal)
    {
        int p = Player(d);
        short v = staleLocal;
        if (d.S16(T.CountdownB) != d.S16(T.CountdownA) || Int16Math.Mod(d.S16(T.Countdown), 4) != 0)
        {
            v = -1;
        }
        else if (d.S16(T.Countdown) > 4)
        {
            if (random.Next(3) == 0) v = 3;
            if (random.Next(3) == 0) v = 2;
        }
        else
        {
            v = d.S16(W(T.Surplus, 2 * p)) >= 10 ? (short)2 : (short)3;
            if (d.S16(L.LoopWare) == 3)
            {
                if (d.U8(B(T.NoFieldFound, p)) == 1 && d.S8(L.FieldTotal) >= 8) v = 3;
                if (d.S8(L.ActivePlayer) == d.S8(T.TargetPlayer)) v = 3;
                if (d.U8(B(L.Rank, p)) <= 1 || d.S16(W(T.Ware3Rank, 2 * p)) == 0) v = 3;
            }
        }
        d.Set(B(ControlReader.Action, p), unchecked((byte)v));
        return v;
    }

    /// <summary>
    /// 141D:021A – need, reserve and surplus of player DS:37AA for ware DS:45D0. Computers take the planned need and price
    /// limit of 11A3:104B; humans are estimated (ware 3: average ware-3 stock, otherwise fields of the next kind times
    /// DS:2F26) with reserve 30. Called by 0C29:0086, 008C and 0464.
    /// </summary>
    // [ORIGINAL] 141D:021A
    public static void Demand(Dgroup d)
    {
        int p = Player(d);
        short w = d.S16(L.LoopWare);
        int row = Int16Math.S((p << 3) + (w << 1));
        if (d.U8(B(L.IsComputer, p)) == 1)
        {
            d.Set(W(T.Need, 2 * p), Int16Math.S(d.S16(W(T.WareNeed, row)) - d.S16(W(L.Stock, row))));
            d.Set(W(T.Reserve, 2 * p), d.S16(W(T.PriceLimit, row)));
        }
        else
        {
            short estimate;
            if (w == 3)
            {
                short sum = Int16Math.S(d.S16(L.Stock + 6) + d.S16(L.Stock + 14) + d.S16(L.Stock + 22) + d.S16(L.Stock + 30));
                estimate = Int16Math.Div(sum, 4);
            }
            else
            {
                int next = Int16Math.S(w + 1);
                if (next == 3) next = 0;
                estimate = Int16Math.S(d.S8(B(L.FieldsByKind, (p << 2) + next)) * d.S8(L.LimitLow));
            }
            d.Set(W(T.Need, 2 * p), Int16Math.S(estimate - d.S16(W(L.Stock, row))));
            d.Set(W(T.Reserve, 2 * p), (short)30);
        }
        d.Set(W(T.Surplus, 2 * p), Int16Math.S(-Int16Math.S(d.S16(W(T.Need, 2 * p)) + d.S16(W(T.Reserve, 2 * p)))));
        if (d.S16(W(T.Need, 2 * p)) < 0) d.Set(W(T.Need, 2 * p), (short)0);
        if (d.S16(W(T.Surplus, 2 * p)) < 0) d.Set(W(T.Surplus, 2 * p), (short)0);
        else d.Set(W(T.Reserve, 2 * p), (short)0);
    }

    /// <summary>
    /// 141D:0464 – bid plan of all buyers for ware DS:45D0 at the start of the trade (0C29:0046): Demand for every player,
    /// bid levels (04A5) and units per level (05A3) for every buyer, then the highest bid (0E09), the sorted bids (0806)
    /// and the sellers' surplus (0F01). Leaves DS:37AA = 4.
    /// </summary>
    // [ORIGINAL] 141D:0464
    public static void PlanBids(Dgroup d)
    {
        for (SetPlayer(d, 0); Player(d) < 4; SetPlayer(d, Player(d) + 1))
        {
            Demand(d);
            if (d.U8(B(T.Side, Player(d))) != 0) continue;
            BidLevels(d);
            BidUnits(d);
        }
        UpdateHighestBid(d);
        SortBids(d);
        UpdateSellerSurplus(d);
    }

    /// <summary>
    /// 141D:09F3 – action of computer player DS:37AA during the trade (0C29:01B7, once per frame): buyers 0A1D, sellers
    /// 0BFD (at most one random(400)); other sides do nothing.
    /// </summary>
    // [ORIGINAL] 141D:09F3
    public static void Act(Dgroup d, OriginalRandom random)
    {
        switch (d.U8(B(T.Side, Player(d))))
        {
            case 0: BuyerAction(d); break;
            case 1: SellerAction(d, random); break;
        }
    }

    /// <summary>
    /// 141D:0CCA – bookkeeping after buyer DS:4106 took ten units at <paramref name="price"/> (0C29:03B2): need and units of
    /// the current level drop by 10, the ware budget by the price; an exhausted level moves to the next one with units and
    /// then the highest bid is recomputed. Nothing for a human buyer.
    /// </summary>
    // [ORIGINAL] 141D:0CCA
    public static void Bought(Dgroup d, short price)
    {
        int b = d.S16(T.Buyer);
        if (d.U8(B(L.IsComputer, b)) == 0) return;
        short need = Int16Math.S(d.S16(W(T.Need, b << 1)) - 10);
        d.Set(W(T.Need, b << 1), need);
        if (need < 0) d.Set(W(T.Need, b << 1), (short)0);
        int si = d.S16(W(T.CurrentLevel, b << 1));
        int units = W(T.BidUnits, (b << 3) + (si << 1));
        d.Set(units, Int16Math.S(d.S16(units) - 10));
        int budget = W(T.WareBudget, (b << 4) + (d.S16(L.LoopWare) << 2));
        d.Set(budget, Int16Math.L((long)d.S32(budget) - price));
        if (d.S16(W(T.BidUnits, (b << 3) + (si << 1))) > 0) return;
        while (true)
        {
            si = Int16Math.S(si + 1);
            short level = Int16Math.S(d.S16(W(T.CurrentLevel, b << 1)) + 1);
            d.Set(W(T.CurrentLevel, b << 1), level);
            if (level > 3)
            {
                d.Set(W(T.CurrentBid, b << 2), 0);
                d.Set(W(T.WareBudget, (b << 4) + (d.S16(L.LoopWare) << 2)), 0);
                break;
            }
            if (d.S16(W(T.BidUnits, (b << 3) + (si << 1))) > 0)
            {
                d.Set(W(T.CurrentBid, b << 2), d.S32(W(T.BidLevel, (b << 4) + (si << 2))));
                break;
            }
        }
        UpdateHighestBid(d);
    }

    /// <summary>
    /// 141D:0E67 – bookkeeping after seller DS:44F2 sold ten units (0C29:03B2): surplus drops by 10, seller surplus is
    /// recomputed, the units of the current sorted bid drop by 10 and the index moves to the next entry with units (0ED7).
    /// </summary>
    // [ORIGINAL] 141D:0E67
    public static void Sold(Dgroup d)
    {
        int s = d.S16(T.Seller);
        short surplus = Int16Math.S(d.S16(W(T.Surplus, s << 1)) - 10);
        d.Set(W(T.Surplus, s << 1), surplus);
        if (surplus < 0) d.Set(W(T.Surplus, s << 1), (short)0);
        UpdateSellerSurplus(d);
        if (d.S16(T.SortedIndex) > 15) return;
        int units = W(T.SortedUnits, d.S16(T.SortedIndex) << 1);
        d.Set(units, Int16Math.S(d.S16(units) - 10));
        while (d.S16(W(T.SortedUnits, d.S16(T.SortedIndex) << 1)) <= 0 && d.S16(T.SortedIndex) <= 15)
            d.Set(T.SortedIndex, Int16Math.S(d.S16(T.SortedIndex) + 1));
        UpdateAskFloor(d);
    }

    // ------------------------------------------------------------------ internal routines

    /// <summary>141D:0029 – DS:2D3E[p] = cash / (150 - (10000 - cash) / 83), long divisions; divisor 0 faults.</summary>
    // [ORIGINAL] 141D:0029
    private static void Budget(Dgroup d)
    {
        int p = Player(d);
        int cash = d.S32(W(L.Cash, p << 2));
        short divisor = Int16Math.S(150 - LongMath.Div(Int16Math.L(10000L - cash), 83));
        d.Set(W(T.Budget, p << 2), LongMath.Div(cash, divisor));
    }

    /// <summary>
    /// 141D:008C – cash share per ware of player DS:37AA: weight[w] = budget * (need / 10) + (reserve / 30) * price[w]
    /// (Demand for every ware, DS:45D0 = 0..3), then DS:2D88[p][w] = cash * (100 * weight[w] / sum) / 100, or 0 when the
    /// weights add up to 0. Leaves DS:45D0 = 4.
    /// </summary>
    // [ORIGINAL] 141D:008C
    private static void WareBudgets(Dgroup d)
    {
        int p = Player(d);
        var weight = new int[4];
        int sum = 0;
        for (d.Set(L.LoopWare, (short)0); d.S16(L.LoopWare) <= 3; d.Set(L.LoopWare, Int16Math.S(d.S16(L.LoopWare) + 1)))
        {
            Demand(d);
            int w = d.S16(L.LoopWare);
            int value = LongMath.Mul(d.S32(W(T.Budget, p << 2)), Int16Math.Div(d.S16(W(T.Need, p << 1)), 10));
            short extra = Int16Math.S(Int16Math.Div(d.S16(W(T.Reserve, p << 1)), 30) * d.S16(W(L.WarePrice, w << 1)));
            weight[w] = Int16Math.L((long)value + extra);
            sum = Int16Math.L((long)sum + weight[w]);
        }
        for (d.Set(L.LoopWare, (short)0); d.S16(L.LoopWare) <= 3; d.Set(L.LoopWare, Int16Math.S(d.S16(L.LoopWare) + 1)))
        {
            int w = d.S16(L.LoopWare);
            int target = W(T.WareBudget, (p << 4) + (w << 2));
            if (sum == 0)
            {
                d.Set(target, 0);
                continue;
            }
            short percent = Int16Math.S(LongMath.Div(LongMath.Mul(100, weight[w]), sum));
            d.Set(target, LongMath.Div(LongMath.Mul(d.S32(W(L.Cash, p << 2)), percent), 100));
        }
    }

    /// <summary>
    /// 141D:04A5 – bid levels of buyer DS:37AA: base = low word of DS:2D3E[p], step = clamp(base / 10, 2, 20); levels
    /// base + step, base, base - step (16-bit) and the ware price; the first three at least the ware price.
    /// </summary>
    // [ORIGINAL] 141D:04A5
    private static void BidLevels(Dgroup d)
    {
        int p = Player(d);
        short basis = d.S16(W(T.Budget, p << 2));
        short step = Int16Math.Div(basis, 10);
        if (step < 2) step = 2;
        if (step > 20) step = 20;
        short price = d.S16(W(L.WarePrice, d.S16(L.LoopWare) << 1));
        d.Set(Level(p, 0), (int)Int16Math.S(basis + step));
        d.Set(Level(p, 1), (int)basis);
        d.Set(Level(p, 2), (int)Int16Math.S(basis - step));
        d.Set(Level(p, 3), (int)price);
        for (int i = 0; i <= 2; i++)
            if (d.S32(Level(p, i)) < d.S16(W(L.WarePrice, d.S16(L.LoopWare) << 1)))
                d.Set(Level(p, i), (int)d.S16(W(L.WarePrice, d.S16(L.LoopWare) << 1)));
    }

    /// <summary>
    /// 141D:05A3 – units per bid level of buyer DS:37AA from its ware budget: rounds over the first three levels add ten
    /// units to every level that is still cheaper than the remaining budget, until the need is covered or no level is
    /// affordable; the rest of the need goes to the reserve, which buys at the ware price while affordable. Sets the
    /// current level (first with units, 4 = none) and its bid; with no units the ware budget is cleared.
    /// </summary>
    // [ORIGINAL] 141D:05A3
    private static void BidUnits(Dgroup d)
    {
        int p = Player(d);
        int budget = d.S32(W(T.WareBudget, (p << 4) + (d.S16(L.LoopWare) << 2)));
        for (int i = 0; i <= 3; i++) d.Set(Units(p, i), (short)0);
        short need = d.S16(W(T.Need, p << 1));
        short reserve = d.S16(W(T.Reserve, p << 1));
        var full = new bool[3];
        while (!(full[0] && full[1] && full[2]))
        {
            if (need <= 0) break;
            for (int i = 0; i <= 2; i++)
            {
                if (d.S32(Level(p, i)) >= budget)
                {
                    full[i] = true;
                    continue;
                }
                d.Set(Units(p, i), Int16Math.S(d.S16(Units(p, i)) + 10));
                need = Int16Math.S(need - 10);
                budget = Int16Math.L((long)budget - d.S32(Level(p, i)));
            }
        }
        if (need > 0) reserve = Int16Math.S(reserve + need);
        while (d.S32(Level(p, 3)) <= budget)
        {
            if (reserve <= 0) break;
            d.Set(Units(p, 3), Int16Math.S(d.S16(Units(p, 3)) + 10));
            reserve = Int16Math.S(reserve - 10);
            budget = Int16Math.L((long)budget - d.S32(Level(p, 3)));
        }
        int first = 0;
        while (first <= 3 && d.S16(Units(p, first)) <= 0) first++;
        d.Set(W(T.CurrentLevel, p << 1), (short)first);
        if (first == 4)
        {
            d.Set(W(T.CurrentBid, p << 2), 0);
            d.Set(W(T.WareBudget, (p << 4) + (d.S16(L.LoopWare) << 2)), 0);
        }
        else
        {
            d.Set(W(T.CurrentBid, p << 2), d.S32(Level(p, first)));
        }
    }

    /// <summary>
    /// 141D:0806 – ranks the 16 bid levels of all players (count of levels lower than or tied with a later one) and writes
    /// them in rank order to DS:2CBE / DS:2D1E (buyers only, sellers' entries 0; a later level with the same rank
    /// overwrites); DS:2D74 = first entry with units, then 0ED7.
    /// </summary>
    // [ORIGINAL] 141D:0806
    private static void SortBids(Dgroup d)
    {
        var rank = new short[16];
        for (int i = 0; i <= 15; i++) d.Set(W(T.SortedBid, i << 2), 0);
        for (int i = 0; i <= 15; i++)
            for (int j = i + 1; j <= 15; j++)
            {
                int a = d.S32(Level(i / 4, i % 4)), b = d.S32(Level(j / 4, j % 4));
                if (a > b) rank[j]++;
                if (b >= a) rank[i]++;
            }
        for (int r = 0; r <= 15; r++)
            for (int j = 0; j <= 15; j++)
            {
                if (rank[j] != r) continue;
                if (d.U8(B(T.Side, j / 4)) == 0)
                {
                    d.Set(W(T.SortedBid, r << 2), d.S32(Level(j / 4, j % 4)));
                    d.Set(W(T.SortedUnits, r << 1), d.S16(Units(j / 4, j % 4)));
                }
                else
                {
                    d.Set(W(T.SortedBid, r << 2), 0);
                    d.Set(W(T.SortedUnits, r << 1), (short)0);
                }
            }
        short first = 0;
        while (first <= 15 && d.S16(W(T.SortedUnits, first << 1)) <= 0) first++;
        d.Set(T.SortedIndex, first);
        UpdateAskFloor(d);
    }

    /// <summary>
    /// 141D:0A1D – buyer move of computer DS:37AA: target = market price + spread / 2, two (or one) steps lower when the
    /// sellers' surplus per seller exceeds the need (or the highest bid rule applies), at least the current bid and the
    /// market price, at most the current bid + 60 (exactly the bid on level 3) and the ware budget; DS:3308[p] = 2 when
    /// the target is above the own position, 3 when below, otherwise -1. Divides by DS:2BB8 + 1 or DS:2BB8 (0 faults).
    /// </summary>
    // [ORIGINAL] 141D:0A1D
    private static void BuyerAction(Dgroup d)
    {
        int p = Player(d);
        short market = d.S16(T.MarketPrice);
        short di = Int16Math.S(Int16Math.Div(d.S16(T.Spread), 2) + market);
        short bid = d.S16(W(T.CurrentBid, p << 2));
        d.Set(B(ControlReader.Action, p), (byte)0xFF);
        short position = d.S16(W(T.Position, p << 1));
        int own = position >= 0
            ? Int16Math.S(market + Int16Math.Div(position, 2))
            : Int16Math.S(market + Int16Math.Div(Int16Math.S(position - 1), 2));
        short si;
        if (position < d.S16(T.Spread))
        {
            short perSeller = Int16Math.Div(d.S16(T.SellerSurplus), Int16Math.S(d.S8(T.Sellers) + 1));
            si = perSeller > d.S16(W(T.Need, p << 1)) ? Int16Math.S(di - 2) : di;
        }
        else
        {
            short perSeller = Int16Math.Div(d.S16(T.SellerSurplus), d.S8(T.Sellers));
            if (perSeller > Int16Math.S(d.S16(W(T.Need, p << 1)) + 50)) si = Int16Math.S(di - 2);
            else if (d.U8(T.BuyerFlag) == 1 && di > d.S32(T.HighestBid)) si = Int16Math.S(di - 1);
            else si = di;
        }
        if (si < bid) si = bid;
        if (si < market) si = market;
        int cap = d.S16(W(T.CurrentLevel, p << 1)) == 3 ? bid : Int16Math.S(bid + 60);
        int wareBudget = d.S32(W(T.WareBudget, (p << 4) + (d.S16(L.LoopWare) << 2)));
        if (wareBudget < cap) cap = wareBudget;
        if (si > cap) si = Int16Math.S(cap);
        if (si > own) d.Set(B(ControlReader.Action, p), (byte)2);
        else if (si < own) d.Set(B(ControlReader.Action, p), (byte)3);
    }

    /// <summary>
    /// 141D:0BFD – seller move of computer DS:37AA: no surplus below 10 or 12 countdown steps since the last forced sale →
    /// 10 (fire); from 3 steps on a seller with surplus 600 or more, or after random(400) = 0, moves to the next sorted bid
    /// (0ED7) and resets DS:2BBA (-1); otherwise 3 above the ask floor, 2 below, -1 on it.
    /// </summary>
    // [ORIGINAL] 141D:0BFD
    private static void SellerAction(Dgroup d, OriginalRandom random)
    {
        int p = Player(d);
        short di = Int16Math.S(Int16Math.Div(d.S16(W(T.Position, p << 1)), 2) + d.S16(T.MarketPrice));
        short since = Int16Math.S(d.S16(T.LastSale) - d.S16(T.Countdown));
        short surplus = d.S16(W(T.Surplus, p << 1));
        short v;
        if (surplus < 10 || since >= 12)
        {
            v = 10;
        }
        else if (since >= 3 && (surplus >= 600 || random.Next(400) == 0))
        {
            d.Set(T.SortedIndex, Int16Math.S(d.S16(T.SortedIndex) + 1));
            UpdateAskFloor(d);
            d.Set(T.LastSale, d.S16(T.Countdown));
            v = -1;
        }
        else if (di > d.S32(T.AskFloor)) v = 3;
        else if (di < d.S32(T.AskFloor)) v = 2;
        else v = -1;
        d.Set(B(ControlReader.Action, p), unchecked((byte)v));
    }

    /// <summary>141D:0E09 – DS:2DC8 = highest current bid of a computer buyer (0 if none).</summary>
    // [ORIGINAL] 141D:0E09
    private static void UpdateHighestBid(Dgroup d)
    {
        d.Set(T.HighestBid, 0);
        for (int i = 0; i <= 3; i++)
        {
            if (d.U8(T.Side + i) != 0 || d.U8(L.IsComputer + i) == 0) continue;
            if (d.S32(T.CurrentBid + 4 * i) > d.S32(T.HighestBid)) d.Set(T.HighestBid, d.S32(T.CurrentBid + 4 * i));
        }
    }

    /// <summary>141D:0ED7 – DS:2D60 = sorted bid at DS:2D74, or 1 when the index is past the last entry.</summary>
    // [ORIGINAL] 141D:0ED7
    private static void UpdateAskFloor(Dgroup d)
    {
        short i = d.S16(T.SortedIndex);
        d.Set(T.AskFloor, i > 15 ? 1 : d.S32(W(T.SortedBid, i << 2)));
    }

    /// <summary>141D:0F01 – DS:2D76 = sum of the surplus of all sellers (16-bit).</summary>
    // [ORIGINAL] 141D:0F01
    private static void UpdateSellerSurplus(Dgroup d)
    {
        d.Set(T.SellerSurplus, (short)0);
        for (int i = 0; i <= 3; i++)
            if (d.U8(T.Side + i) == 1)
                d.Set(T.SellerSurplus, Int16Math.S(d.S16(T.SellerSurplus) + d.S16(T.Surplus + 2 * i)));
    }

    // ------------------------------------------------------------------ helpers

    private static int Level(int p, int i) => W(T.BidLevel, (p << 4) + (i << 2));

    private static int Units(int p, int i) => W(T.BidUnits, (p << 3) + (i << 1));

    /// <summary>Address base + index in 16 bits, as the original's [bx+disp] (word/dword access).</summary>
    private static int W(int basis, int index) => Int16Math.U(basis + index);

    private static int B(int basis, int index) => Int16Math.U(basis + index);

    private static int Player(Dgroup d) => d.S8(L.ActivePlayer);

    private static void SetPlayer(Dgroup d, int p) => d.Set(L.ActivePlayer, unchecked((sbyte)p));
}
