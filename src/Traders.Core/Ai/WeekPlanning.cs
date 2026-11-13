using Traders.Core.Rules;
using Traders.Core.State;
using L = Traders.Core.State.DgroupLayout;
using W = Traders.Core.Ai.WeekPlanningLayout;

namespace Traders.Core.Ai;

/// <summary>
/// Week planning of the computer players 11A3:0000 without a screen (docs/routines/11A3_0000_wochenplanung.md): main kind,
/// target player, planned fields per kind; pass 0 (call 6, before planting) fills the planting plan DS:48F4 and the plant
/// list, pass 1 (call 21, before trading) the ware needs and price limits. Works directly on the data segment, including
/// the global loop variables DS:37AA, DS:45D0, DS:2EB0 and DS:2F00 the original leaves behind. At most one random(5).
/// </summary>
public static class WeekPlanning
{
    private const int StockStride = 8, PlayerRowStride = 8, KindsStride = 4, GridStride = 26;

    /// <summary>Runs 11A3:0000 with argument <paramref name="pass"/> (0 before planting, otherwise before trading).</summary>
    // [ORIGINAL] 11A3:0000
    public static void Apply(Dgroup d, OriginalRandom random, byte pass)
    {
        Ware3Ranks(d);
        ChooseTarget(d, random);
        for (SetPlayer(d, 0); Player(d) < L.PlayerCount; SetPlayer(d, Player(d) + 1))
        {
            if (!IsComputer(d, Player(d))) continue;
            QualityShares(d);
            MainKindByQuality(d);
        }
        HumanMainKinds(d);
        FillMissingKinds(d);
        for (SetPlayer(d, 0); Player(d) < L.PlayerCount; SetPlayer(d, Player(d) + 1))
        {
            if (!IsComputer(d, Player(d))) continue;
            PlanFields(d);
            if (pass == 0)
            {
                PlanGrid(d);
                PlantList(d);
            }
            else
            {
                WareNeeds(d);
            }
        }
    }

    /// <summary>Rank of every player by the stock of ware 3 and the lead of the first (DS:2C74, DS:2C7C).</summary>
    // [ORIGINAL] 11A3:007D
    private static void Ware3Ranks(Dgroup d)
    {
        for (SetPlayer(d, 0); Player(d) < L.PlayerCount; SetPlayer(d, Player(d) + 1))
            d.Set(W.Ware3Rank + 2 * Player(d), (short)3);
        for (SetPlayer(d, 0); Player(d) < L.PlayerCount; SetPlayer(d, Player(d) + 1))
        {
            int p = Player(d);
            for (int q = p + 1; q <= 3; q++)
            {
                if (Ware3Stock(d, p) >= Ware3Stock(d, q)) Decrement(d, W.Ware3Rank + 2 * p);
                if (Ware3Stock(d, q) >= Ware3Stock(d, p)) Decrement(d, W.Ware3Rank + 2 * q);
            }
        }
        int first = 0, leaders = -1, second = -1;
        for (SetPlayer(d, 0); Player(d) < L.PlayerCount; SetPlayer(d, Player(d) + 1))
        {
            int p = Player(d);
            if (d.S16(W.Ware3Rank + 2 * p) == 0)
            {
                first++;
                leaders = p;
            }
            if (d.S16(W.Ware3Rank + 2 * p) == 1) second = p;
        }
        if (first > 1)
        {
            d.Set(W.Ware3Lead, (short)0);
            return;
        }
        // CX and DX keep their values from the caller when no player has rank 0 or 1; with four players that cannot happen.
        if (leaders < 0 || second < 0) throw new InvalidOperationException("11A3:007D: no rank 0 or rank 1 player (register values at entry).");
        d.Set(W.Ware3Lead, Int16Math.S(Ware3Stock(d, leaders) - Ware3Stock(d, second)));
    }

    /// <summary>Chooses the target player DS:4580 of the computers; draws random(5) at most once.</summary>
    // [ORIGINAL] 11A3:0180
    private static void ChooseTarget(Dgroup d, OriginalRandom random)
    {
        // Local frame from BP-0Ch: byte flags[4], then word counts[4]; flags[4] reads the low byte of counts[0].
        var frame = new byte[12];
        sbyte chosen = -1;
        sbyte computers = 0;
        for (SetPlayer(d, 0); Player(d) < L.PlayerCount; SetPlayer(d, Player(d) + 1))
        {
            int p = Player(d);
            if (IsComputer(d, p)) computers++;
            short station = d.S16(L.Station + 2 * p);
            bool flag = station != -1 && d.U8(L.StationVisited + station) != 0 && d.S16(L.StationKind + 2 * station) != 0;
            frame[p] = flag ? (byte)1 : (byte)0;
            SetCount(frame, p, 0);
            ForEachField(d, (x, y) =>
            {
                int cell = FieldCell(x, y);
                if (d.U8(L.FieldOwner + cell) != d.U8(L.ActivePlayer) || d.U8(L.FieldKind + cell) == 4
                    || d.U8(L.FieldQuality + cell) == 3) return;
                int owner = d.S8(L.FieldOwner + cell);
                SetCount(frame, owner, Int16Math.S(Count(frame, owner) + 1));
            });
        }

        for (SetPlayer(d, 0); Player(d) < L.PlayerCount; SetPlayer(d, Player(d) + 1))
        {
            int p = Player(d);
            if (!IsComputer(d, p) || d.U8(0x4BBE + p) != 1 || d.S8(L.FieldTotal) <= 8 || frame[p] == 0) continue;
            if (chosen == -1 || Count(frame, p) > Count(frame, chosen)) chosen = (sbyte)p;
        }

        if (chosen == -1 && d.S8(L.FieldTotal) >= 16)
        {
            chosen = d.S8(W.TargetPlayer);
            bool contest = true;
            for (SetPlayer(d, 0); Player(d) < L.PlayerCount; SetPlayer(d, Player(d) + 1))
                if (IsComputer(d, Player(d)) && d.S16(W.Ware3Rank + 2 * Player(d)) == 0) contest = false;
            if (computers <= 1) contest = false;

            if (d.S8(W.TargetPlayer) == -1)
            {
                if (random.Next(5) == 0 || contest)
                {
                    for (SetPlayer(d, 0); Player(d) < L.PlayerCount; SetPlayer(d, Player(d) + 1))
                    {
                        int p = Player(d);
                        if (!IsComputer(d, p) || d.U8(L.Rank + p) == 0 || frame[p] == 0) continue;
                        if (chosen == -1 || Count(frame, p) > Count(frame, chosen)) chosen = (sbyte)p;
                    }
                }
            }
            else
            {
                short r = random.Next(5);
                if (r != 0 || contest)
                {
                    // indexed with the loop variable, which is 4 here: flags[4] = low byte of counts[0]
                    if (Frame(frame, Player(d)) == 0) chosen = -1;
                }
                else
                {
                    chosen = -1;
                }
            }
        }
        d.Set(W.TargetPlayer, chosen);
    }

    /// <summary>Share of good land per kind in percent for the current player (DS:2C4C).</summary>
    // [ORIGINAL] 11A3:043F
    private static void QualityShares(Dgroup d)
    {
        int p = Player(d);
        var sums = new short[3];
        short total = 0;
        int weight = -1; // CL: only set for table values 0..2, otherwise kept from the previous field
        for (d.Set(L.LoopWare, (short)0); d.S16(L.LoopWare) <= 2; d.Set(L.LoopWare, Int16Math.S(d.S16(L.LoopWare) + 1)))
        {
            int k = d.S16(L.LoopWare);
            sums[k] = 0;
            ForEachField(d, (x, y) =>
            {
                int cell = FieldCell(x, y);
                if (d.U8(L.FieldOwner + cell) != d.U8(L.ActivePlayer) || d.U8(L.FieldKind + cell) == 4) return;
                switch (d.S8(L.QualityTable + Int16Math.S(k * 4) + d.S8(L.FieldQuality + cell)))
                {
                    case 0: weight = 0; break;
                    case 1: weight = 2; break;
                    case 2: weight = 5; break;
                }
                if (weight < 0) throw new InvalidOperationException("11A3:043F: quality table value outside 0..2 before any other (CL at entry).");
                sums[k] = Int16Math.S(sums[k] + weight);
                total = Int16Math.S(total + weight);
            });
        }
        for (d.Set(L.LoopWare, (short)0); d.S16(L.LoopWare) <= 2; d.Set(L.LoopWare, Int16Math.S(d.S16(L.LoopWare) + 1)))
        {
            int k = d.S16(L.LoopWare);
            short share = total == 0 ? (short)0 : Int16Math.Div(Int16Math.S(sums[k] * 100), total);
            d.Set(W.QualityShare + p * PlayerRowStride + 2 * k, share);
        }
    }

    /// <summary>Main kind DS:40F6 of the current player = kind with the largest share of good land.</summary>
    // [ORIGINAL] 11A3:0599
    private static void MainKindByQuality(Dgroup d)
    {
        int p = Player(d);
        int row = W.QualityShare + p * PlayerRowStride;
        sbyte best = d.S8(row + Int16Math.S(d.S16(W.MainKind + 2 * p) << 1));
        for (d.Set(L.LoopWare, (short)0); d.S16(L.LoopWare) <= 2; d.Set(L.LoopWare, Int16Math.S(d.S16(L.LoopWare) + 1)))
        {
            int k = d.S16(L.LoopWare);
            if (d.S16(row + 2 * k) <= best) continue;
            d.Set(W.MainKind + 2 * p, (short)k);
            best = d.S8(row + 2 * k);
        }
    }

    /// <summary>Main kind of the humans: first kind with a production share of at least 70 percent, else -1.</summary>
    // [ORIGINAL] 11A3:061A
    private static void HumanMainKinds(Dgroup d)
    {
        Production.Shares(d);
        for (SetPlayer(d, 0); Player(d) < L.PlayerCount; SetPlayer(d, Player(d) + 1))
        {
            int p = Player(d);
            if (d.U8(L.IsComputer + p) != 0) continue;
            d.Set(W.MainKind + 2 * p, (short)-1);
            for (int k = 0; k <= 2; k++)
            {
                if (d.U8(L.ProductionShares + p * 3 + k) < 70) continue;
                d.Set(W.MainKind + 2 * p, (short)k);
                break;
            }
        }
    }

    /// <summary>A kind nobody has as main kind is taken over by the first computer with enough good land for it.</summary>
    // [ORIGINAL] 11A3:067D
    private static void FillMissingKinds(Dgroup d)
    {
        var players = new short[3]; // local word[4]; element 3 is never initialised
        for (d.Set(L.LoopWare, (short)0); d.S16(L.LoopWare) <= 2; d.Set(L.LoopWare, Int16Math.S(d.S16(L.LoopWare) + 1)))
            players[d.S16(L.LoopWare)] = 0;
        for (SetPlayer(d, 0); Player(d) < L.PlayerCount; SetPlayer(d, Player(d) + 1))
            for (d.Set(L.LoopWare, (short)0); d.S16(L.LoopWare) <= 2; d.Set(L.LoopWare, Int16Math.S(d.S16(L.LoopWare) + 1)))
                if (d.S16(W.MainKind + 2 * Player(d)) == d.S16(L.LoopWare))
                    players[d.S16(L.LoopWare)]++;
        for (d.Set(L.LoopWare, (short)0); d.S16(L.LoopWare) <= 2; d.Set(L.LoopWare, Int16Math.S(d.S16(L.LoopWare) + 1)))
        {
            int k = d.S16(L.LoopWare);
            if (players[k] != 0) continue;
            for (SetPlayer(d, 0); Player(d) < L.PlayerCount; SetPlayer(d, Player(d) + 1))
            {
                int p = Player(d);
                if (!IsComputer(d, p)) continue;
                short main = d.S16(W.MainKind + 2 * p);
                if (d.S16(W.QualityShare + p * PlayerRowStride + 2 * k) < 0x21) continue;
                if (main is < 0 or > 2) throw new InvalidOperationException($"11A3:067D: main kind {main} of computer {p} indexes outside the local array.");
                if (players[main] < 2) continue;
                players[main]--;
                d.Set(W.MainKind + 2 * p, (short)k);
            }
        }
    }

    /// <summary>Planned fields per kind for the current computer (DS:2C44, DS:2C6C, DS:303E).</summary>
    // [ORIGINAL] 11A3:078A
    private static void PlanFields(Dgroup d)
    {
        int p = Player(d);
        byte fields = 0;
        short good = 0;
        sbyte main = unchecked((sbyte)d.U8(W.MainKind + 2 * p));
        sbyte second = d.S8(L.KindWare + main);
        for (d.Set(L.LoopWare, (short)0); d.S16(L.LoopWare) <= 3; d.Set(L.LoopWare, Int16Math.S(d.S16(L.LoopWare) + 1)))
        {
            d.Set(W.PlanFieldsLimited + 2 * d.S16(L.LoopWare), (short)0);
            d.Set(W.PlanFields + 2 * d.S16(L.LoopWare), (short)0);
        }
        ForEachField(d, (x, y) =>
        {
            int cell = FieldCell(x, y);
            if (d.U8(L.FieldOwner + cell) != d.U8(L.ActivePlayer) || d.U8(L.FieldKind + cell) == 4) return;
            fields++;
            if (d.U8(L.FieldQuality + cell) != 3) good++;
        });

        bool alone = true;
        for (byte q = 0; q <= 3; q++)
            if (d.U8(L.IsComputer + q) == 1 && d.S16(W.MainKind + 2 * q) == second) alone = false;

        short rockets;
        if (d.U8(L.ActivePlayer) == d.U8(W.TargetPlayer))
        {
            short lead = d.S16(W.Ware3Lead);
            if (d.S16(W.Ware3Rank + 2 * p) != 0 || lead < 100) rockets = good;
            else if (lead < 200) rockets = Int16Math.S(Int16Math.Div(fields, 2) + 1);
            else if (lead < 300) rockets = Int16Math.S(Int16Math.Div(fields, 3) + 1);
            else rockets = Int16Math.S(Int16Math.Div(fields, 4) + 1);
        }
        else
        {
            rockets = Int16Math.Div(fields, d.U8(L.FatMikeFlag + p) == 1 ? (short)3 : (short)4);
            if (Ware3Stock(d, p) < 200) rockets++;
            if (Ware3Stock(d, p) < 80 && rockets < 2) rockets = 2;
        }
        if (rockets > good) rockets = good;

        short secondFields = alone ? Int16Math.Div(Int16Math.S(fields - rockets), 5) : (short)0;
        short mainFields = Int16Math.S(fields - Int16Math.S(rockets + secondFields));
        d.Set(W.PlanFields + 2 * main, mainFields);
        d.Set(W.PlanFields + 2 * second, secondFields);
        d.Set(W.PlanFields + 6, rockets);

        short launchable = Int16Math.S(d.S8(L.FieldsByKind + p * KindsStride + 3) + d.S16(L.Rockets + 2 * p));
        if (launchable < good) good = launchable;
        if (rockets > good) rockets = good;
        short limit = StockLimit(d, second, 0);
        secondFields = alone ? Int16Math.Div(Int16Math.S(fields - rockets), 5) : (short)0;
        if (secondFields > limit) secondFields = limit;
        mainFields = Int16Math.S(fields - Int16Math.S(rockets + secondFields));
        limit = StockLimit(d, main, Int16Math.S(d.S8(L.LimitLow) * secondFields));
        if (mainFields > limit) mainFields = limit;
        d.Set(W.PlanFieldsLimited + 2 * main, mainFields);
        d.Set(W.PlanFieldsLimited + 2 * second, secondFields);
        d.Set(W.PlanFieldsLimited + 6, rockets);

        for (d.Set(L.LoopWare, (short)0); d.S16(L.LoopWare) <= 3; d.Set(L.LoopWare, Int16Math.S(d.S16(L.LoopWare) + 1)))
        {
            int k = d.S16(L.LoopWare);
            d.Set(W.FieldSurplus + 2 * k,
                Int16Math.S(d.S8(L.FieldsByKind + p * KindsStride + k) - d.S16(W.PlanFields + 2 * k)));
        }
    }

    /// <summary>Fields of <paramref name="kind"/> the stock allows: consumed ware / DS:2F26, second ware / DS:48A2 + own fields.</summary>
    // [ORIGINAL] 11A3:0AA5
    private static short StockLimit(Dgroup d, short kind, short reserved)
    {
        int p = Player(d);
        short consumed = d.S8(L.KindWare + kind);
        short secondWare = d.S8(W.SecondWare + kind);
        short limit = Int16Math.Div(d.S16(Stock(p, consumed)), d.S8(L.LimitLow));
        short bySecond = Int16Math.Div(Int16Math.S(d.S16(Stock(p, secondWare)) - reserved), d.S8(L.LimitHigh));
        short own = d.S8(L.FieldsByKind + p * KindsStride + kind);
        if (Int16Math.S(bySecond + own) < limit) limit = Int16Math.S(bySecond + own);
        return limit;
    }

    /// <summary>Planting plan of the current computer in the grid DS:48F4 (100 = no plan).</summary>
    // [ORIGINAL] 11A3:0B3B
    private static void PlanGrid(Dgroup d)
    {
        int p = Player(d);
        byte main = d.U8(W.MainKind + 2 * p);
        byte second = d.U8(L.KindWare + main);
        ForEachField(d, (x, y) =>
        {
            if (d.U8(L.FieldOwner + FieldCell(x, y)) == d.U8(L.ActivePlayer))
                d.Set(L.ProductionResult + PlanCell(x, y), (byte)W.PlanFree);
        });

        for (byte stage = 0; stage <= 2; stage++)
        {
            byte target = stage switch { 0 => second, 1 => main, _ => 3 };
            byte left = d.U8(W.PlanFieldsLimited + 2 * target);
            for (byte level = 0; level <= 11; level++)
            {
                if (left == 0) break;
                for (d.Set(L.LoopY, (sbyte)0); d.S8(L.LoopY) <= Int16Math.S(d.S16(L.FieldRows) - 1); d.Set(L.LoopY, (sbyte)(d.S8(L.LoopY) + 1)))
                    for (d.Set(L.LoopX, (sbyte)0); d.S8(L.LoopX) <= Int16Math.S(d.S16(L.FieldColumns) - 1); d.Set(L.LoopX, (sbyte)(d.S8(L.LoopX) + 1)))
                    {
                        if (left == 0) break;
                        int x = d.S8(L.LoopX), y = d.S8(L.LoopY);
                        int cell = FieldCell(x, y);
                        if (d.U8(L.FieldOwner + cell) != d.U8(L.ActivePlayer) || d.U8(L.FieldKind + cell) == 4) continue;
                        if (d.U8(L.ProductionResult + PlanCell(x, y)) != W.PlanFree) continue;
                        bool fits = stage switch
                        {
                            0 => MatchesLevel(d, level, target),
                            1 => MatchesLevelPlusOne(d, level, target),
                            _ => MatchesRocketLevel(d, level),
                        };
                        if (!fits) continue;
                        d.Set(L.ProductionResult + PlanCell(x, y), target);
                        left--;
                    }
            }
        }
    }

    /// <summary>Quality class (1..4) times kind class (1 same, 2 empty, 3 other) equals <paramref name="level"/>.</summary>
    // [ORIGINAL] 11A3:0D35
    private static bool MatchesLevel(Dgroup d, short level, short kind)
    {
        int cell = CurrentCell(d);
        short bonus = d.S8(L.QualityTable + Int16Math.S(kind << 2) + d.S8(L.FieldQuality + cell));
        bool poor = d.U8(L.FieldQuality + cell) == 3;
        short quality = bonus == 2 && !poor ? (short)1 : poor ? (short)2 : bonus == 1 ? (short)3 : (short)4;
        return Int16Math.S(quality * KindClass(d, cell, kind)) == level;
    }

    /// <summary>As <see cref="MatchesLevel"/> with poor land first and compared with <paramref name="level"/> + 1.</summary>
    // [ORIGINAL] 11A3:0E01
    private static bool MatchesLevelPlusOne(Dgroup d, short level, short kind)
    {
        int cell = CurrentCell(d);
        short bonus = d.S8(L.QualityTable + Int16Math.S(kind << 2) + d.S8(L.FieldQuality + cell));
        short quality = d.U8(L.FieldQuality + cell) == 3 ? (short)1 : bonus == 2 ? (short)2 : bonus == 1 ? (short)3 : (short)4;
        return Int16Math.S(quality * KindClass(d, cell, kind)) == Int16Math.S(level + 1);
    }

    /// <summary>Rocket fields: level 0 existing rocket field, 1 empty field, 2 any field; never on quality 3.</summary>
    // [ORIGINAL] 11A3:0EB8
    private static bool MatchesRocketLevel(Dgroup d, short level)
    {
        int cell = CurrentCell(d);
        if (d.U8(L.FieldQuality + cell) == 3) return false;
        return level switch
        {
            0 => d.U8(L.FieldKind + cell) == 3,
            1 => d.U8(L.FieldKind + cell) == 0xFE,
            2 => true,
            _ => false,
        };
    }

    /// <summary>Plant list of the current computer: own fields whose plan differs from what grows there.</summary>
    // [ORIGINAL] 11A3:0F2E
    private static void PlantList(Dgroup d)
    {
        int p = Player(d);
        short n = 0;
        ForEachField(d, (x, y) =>
        {
            int cell = FieldCell(x, y);
            if (d.U8(L.FieldOwner + cell) != d.U8(L.ActivePlayer) || d.U8(L.FieldKind + cell) == 4) return;
            byte plan = d.U8(L.ProductionResult + PlanCell(x, y));
            if (plan == W.PlanFree || d.U8(L.FieldKind + cell) == plan) return;
            int entry = Int16Math.S(p * W.PlantListStride) + n;
            d.Set(W.PlantList + entry, plan);
            d.Set(W.PlantListX + entry, d.U8(L.LoopX));
            d.Set(W.PlantListY + entry, d.U8(L.LoopY));
            n++;
        });
        d.Set(W.PlantListLength + 2 * p, n);
    }

    /// <summary>Ware needs DS:4560 and price limits DS:452A of the current computer, rocket shortfall DS:3046.</summary>
    // [ORIGINAL] 11A3:104B
    private static void WareNeeds(Dgroup d)
    {
        int p = Player(d);
        int need = W.WareNeed + p * PlayerRowStride;
        for (d.Set(L.LoopWare, (short)0); d.S16(L.LoopWare) <= 3; d.Set(L.LoopWare, Int16Math.S(d.S16(L.LoopWare) + 1)))
            d.Set(need + 2 * d.S16(L.LoopWare), (short)0);
        for (d.Set(L.LoopWare, (short)0); d.S16(L.LoopWare) <= 2; d.Set(L.LoopWare, Int16Math.S(d.S16(L.LoopWare) + 1)))
        {
            int k = d.S16(L.LoopWare);
            int consumed = need + 2 * d.S8(L.KindWare + k);
            d.Set(consumed, Int16Math.S(d.S16(consumed) + Int16Math.S(d.S16(W.PlanFields + 2 * k) * d.S8(L.LimitLow))));
            short surplus = d.S16(W.FieldSurplus + 2 * k);
            if (surplus >= 0) continue;
            int secondWare = need + 2 * d.S8(W.SecondWare + k);
            d.Set(secondWare, Int16Math.S(d.S16(secondWare) + Int16Math.S(Int16Math.S(-surplus) * d.S8(L.LimitHigh))));
        }
        short allWare3 = Int16Math.S(Ware3Stock(d, 0) + Ware3Stock(d, 1) + Ware3Stock(d, 2) + Ware3Stock(d, 3));
        d.Set(need + 6, Int16Math.Div(allWare3, 8));
        short rocketSurplus = d.S16(W.FieldSurplus + 6);
        d.Set(W.RocketShortfall + 2 * p, rocketSurplus >= 0 ? (short)0 : Int16Math.S(-rocketSurplus));

        int limits = W.PriceLimit + p * PlayerRowStride;
        for (d.Set(L.LoopWare, (short)0); d.S16(L.LoopWare) <= 2; d.Set(L.LoopWare, Int16Math.S(d.S16(L.LoopWare) + 1)))
        {
            int k = d.S16(L.LoopWare);
            short weeks = Int16Math.S(60 - Int16Math.S(Int16Math.Div(d.S8(L.FieldsByKind + p * KindsStride + k), 3) * 10));
            if (weeks < 10) weeks = 10;
            int divisor = Int16Math.S(d.S16(L.WarePrice + 2 * k) * weeks); // IMUL then CWD: the 16-bit product, sign-extended
            short limit = Int16Math.S(LongMath.Mul(LongMath.Div(d.S32(L.Cash + 4 * p), divisor), 10));
            if (limit < 30) limit = 30;
            d.Set(limits + 2 * k, limit);
        }
        d.Set(limits + 6, d.S16(need + 6));
    }

    private static short KindClass(Dgroup d, int cell, short kind) =>
        d.S8(L.FieldKind + cell) == kind ? (short)1 : d.U8(L.FieldKind + cell) == 0xFE ? (short)2 : (short)3;

    private static int Player(Dgroup d) => d.S8(L.ActivePlayer);

    private static void SetPlayer(Dgroup d, int p) => d.Set(L.ActivePlayer, unchecked((sbyte)p));

    private static bool IsComputer(Dgroup d, int p) => d.U8(L.IsComputer + p) == 1;

    private static int Stock(int p, int ware) => L.Stock + Int16Math.S(p * StockStride + Int16Math.S(ware << 1));

    private static short Ware3Stock(Dgroup d, int p) => d.S16(Stock(p, 3));

    private static void Decrement(Dgroup d, int offset) => d.Set(offset, Int16Math.S(d.S16(offset) - 1));

    private static int FieldCell(int x, int y) => Int16Math.S(x * L.FieldRowsMax) + y;

    private static int PlanCell(int x, int y) => Int16Math.S(x * GridStride) + y;

    private static int CurrentCell(Dgroup d) => FieldCell(d.S8(L.LoopX), d.S8(L.LoopY));

    private static short Count(byte[] frame, int p) => (short)(Frame(frame, 4 + 2 * p) | (Frame(frame, 5 + 2 * p) << 8));

    private static void SetCount(byte[] frame, int p, short v)
    {
        frame[4 + 2 * p] = unchecked((byte)v);
        frame[5 + 2 * p] = unchecked((byte)(v >> 8));
    }

    private static byte Frame(byte[] frame, int index) =>
        index is >= 0 and < 12
            ? frame[index]
            : throw new InvalidOperationException($"11A3:0180: local frame index {index} outside the initialised locals.");

    /// <summary>Rows outside, columns inside, on the global byte variables DS:2F00 / DS:2EB0 compared as signed words.</summary>
    private static void ForEachField(Dgroup d, Action<int, int> body)
    {
        for (d.Set(L.LoopY, (sbyte)0); d.S8(L.LoopY) <= Int16Math.S(d.S16(L.FieldRows) - 1); d.Set(L.LoopY, (sbyte)(d.S8(L.LoopY) + 1)))
            for (d.Set(L.LoopX, (sbyte)0); d.S8(L.LoopX) <= Int16Math.S(d.S16(L.FieldColumns) - 1); d.Set(L.LoopX, (sbyte)(d.S8(L.LoopX) + 1)))
                body(d.S8(L.LoopX), d.S8(L.LoopY));
    }
}
