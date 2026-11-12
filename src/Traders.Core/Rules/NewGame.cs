using Traders.Core.State;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Core.Rules;

/// <summary>
/// Start of a new game, segment 0E31 (docs/routines/0E31_0003_neue_partie.md). All routines work on the data segment and
/// draw from the caller's <see cref="OriginalRandom"/>; the global loop variables DS:37AA, 45D0, 2EB0 and 2F00 are left
/// behind exactly as in the original.
/// </summary>
public static class NewGame
{
    /// <summary>Whole start state: land, field, stations, weather, per-player values, gem price, ware prices, week.</summary>
    // [ORIGINAL] 0E31:0003
    public static void Start(Dgroup d, OriginalRandom random)
    {
        CreateField(d, random);
        CreateStations(d, random);
        WeatherRule.Apply(d, random); // 0E31:0010

        for (d.Set(L.ActivePlayer, (sbyte)0); d.S8(L.ActivePlayer) < 4; d.Set(L.ActivePlayer, (sbyte)(d.S8(L.ActivePlayer) + 1)))
        {
            int p = d.S8(L.ActivePlayer);
            d.Set(L.HqX + p, (sbyte)-1);
            d.Set(L.Rockets + 2 * p, (short)5);
            d.Set(L.FieldCount + p, (byte)0);
            d.Set(L.Oil + 2 * p, (short)40);
            d.Set(L.Cash + 4 * p, (int)d.S16(L.InitialCash)); // cwd: sign-extended
            d.Set(L.FatMikeFlag + p, (byte)0);
            ResetRobots(d);
            d.Set(L.GemCredit + 2 * p, (short)50);
            d.Set(L.Gems + p, (sbyte)2);
            for (d.Set(L.LoopWare, (short)0); d.S16(L.LoopWare) <= 3; d.Set(L.LoopWare, (short)(d.S16(L.LoopWare) + 1)))
            {
                int k = d.S16(L.LoopWare);
                d.Set(L.FieldsByKind + 4 * p + k, (sbyte)0);
                d.Set(L.Stock + 8 * p + 2 * k, (short)70);
            }
        }

        // 0E31:00DE: signed 16-bit division, quotient sign-extended
        d.Set(L.BaseUnit, (int)(short)(d.S16(L.InitialCash) / 40));
        // 0E31:00EF: 16-bit sum, high word cleared
        d.Set(L.GemPriceFactor, (short)(random.Next(120) + 40));
        d.Set(L.GemPriceFactor + 2, (short)0);
        for (d.Set(L.LoopWare, (short)0); d.S16(L.LoopWare) <= 3; d.Set(L.LoopWare, (short)(d.S16(L.LoopWare) + 1)))
            d.Set(L.WarePrice + 2 * d.S16(L.LoopWare), (short)3);

        d.Set(L.Week, (short)0);
        d.Set(L.Unknown4580, (sbyte)-1);
        d.Set(L.Unknown40F6 + 0, (short)0);
        d.Set(L.Unknown40F6 + 2, (short)2);
        d.Set(L.Unknown40F6 + 4, (short)1);
        d.Set(L.Unknown40F6 + 6, (short)0);
    }

    /// <summary>Robot values of the player in DS:37AA.</summary>
    // [ORIGINAL] 0E31:0149
    public static void ResetRobots(Dgroup d)
    {
        int p = d.S8(L.ActivePlayer);
        for (int i = 0; i <= 5; i++) d.Set(L.RobotParts + p * L.RobotPartCount + i, (byte)0);
        d.Set(L.Robot376C + 4 * p, (short)0);
        d.Set(L.Robot376E + 4 * p, (short)0);
        d.Set(L.Robot2EF8 + 2 * p, (short)0);
        d.Set(L.Robot377C + p, (byte)1);
        d.Set(L.Robot40EC + 2 * p, (short)50);
        d.Set(L.Robot33DC + 2 * p, (short)30);
        d.Set(L.Robot3016 + 2 * p, (short)0);
    }

    /// <summary>Land quality, field count and an empty field (owner -1, kind -2).</summary>
    // [ORIGINAL] 0E31:01CD
    public static void CreateField(Dgroup d, OriginalRandom random)
    {
        NewGameLand.Create(d, random);
        d.Set(L.FieldTotal, unchecked((byte)(d.S16(L.FieldColumns) * d.S16(L.FieldRows))));
        NewGameLand.ForEachField(d, (x, y) =>
        {
            d.Set(L.FieldOwner + L.FieldIndex(x, y), (sbyte)-1);
            d.Set(L.FieldKind + L.FieldIndex(x, y), (sbyte)-2);
        });
    }

    /// <summary>Twelve stations: visited flag, value and kind (6 x kind 0, 4 x kind 1, 2 x kind 2); no station chosen.</summary>
    // [ORIGINAL] 0E31:023D
    public static void CreateStations(Dgroup d, OriginalRandom random)
    {
        Span<int> left = [6, 4, 2];
        for (int i = 0; i <= 11; i++)
        {
            d.Set(L.StationVisited + i, (byte)0);
            short value = random.Next(2) != 0 ? (short)-1 : random.Next(2) != 0 ? (short)10 : random.Next(3);
            d.Set(L.StationValue + 2 * i, value);

            int kind = -1;
            while (kind == -1)
            {
                int r = unchecked((byte)random.Next(2)); // 0..2, stored as byte
                if (left[r] == 0) continue;
                left[r]--;
                kind = r;
            }
            d.Set(L.StationKind + 2 * i, (short)kind);
        }

        for (d.Set(L.ActivePlayer, (sbyte)0); d.S8(L.ActivePlayer) < 4; d.Set(L.ActivePlayer, (sbyte)(d.S8(L.ActivePlayer) + 1)))
            d.Set(L.Station + 2 * d.S8(L.ActivePlayer), (short)-1);
    }
}
