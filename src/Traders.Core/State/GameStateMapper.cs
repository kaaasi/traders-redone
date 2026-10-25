using L = Traders.Core.State.DgroupLayout;

namespace Traders.Core.State;

/// <summary>Maps <see cref="GameState"/> to and from its original DGROUP locations.</summary>
public static class GameStateMapper
{
    /// <summary>Writes every modelled field into <paramref name="ds"/>; other bytes stay untouched.</summary>
    public static void Store(GameState s, Dgroup ds)
    {
        var o = s.Settings;
        ds.Set(L.Level, o.Level);
        ds.Set(L.ComputerIq, o.ComputerIq);
        ds.Set(L.OilConsumption, o.OilConsumption);
        ds.Set(L.RobotBattle, o.RobotBattle);
        ds.Set(L.GemTrading, o.GemTrading);
        ds.Set(L.Catastrophes, o.Catastrophes);
        ds.Set(L.PiratePresence, o.PiratePresence);
        ds.Set(L.InitialCash, o.InitialCash);
        ds.Set(L.Target, o.Target);
        ds.Set(L.LottoReturns, o.LottoReturns);
        ds.Set(L.TimeLimit, o.TimeLimit);
        ds.Set(L.LandType, o.LandType);
        ds.Set(L.RobotCycle, o.RobotCycle);
        ds.Set(L.FieldColumns, o.FieldColumns);
        ds.Set(L.FieldRows, o.FieldRows);
        ds.Set(L.Sound, o.Sound);

        for (int p = 0; p < L.PlayerCount; p++)
        {
            var pl = s.Players[p];
            ds.Set(L.IsComputer + p, pl.IsComputer);
            ds.Set(L.ControlDevice + p, pl.ControlDevice);
            ds.Set(L.Cash + p * 4, pl.Cash);
            ds.Set(L.Rockets + p * 2, pl.Rockets);
            ds.Set(L.Oil + p * 2, pl.Oil);
            ds.Set(L.Gems + p, pl.Gems);
            ds.Set(L.GemCredit + p * 2, pl.GemCredit);
            ds.Set(L.HqX + p, pl.HqX);
            ds.Set(L.HqY + p, pl.HqY);
            ds.Set(L.Station + p * 2, pl.Station);
            ds.Set(L.FatMikeFlag + p, pl.FatMikeFlag);
            ds.Set(L.Unknown40F6 + p * 2, pl.Unknown40F6);
            ds.Set(L.Unknown4BBE + p, pl.Unknown4BBE);
            ds.Set(L.FieldCount + p, pl.FieldCount);
            for (int k = 0; k < 4; k++)
            {
                ds.Set(L.FieldsByKind + p * 4 + k, pl.FieldsByKind[k]);
                ds.Set(L.Stock + p * 8 + k * 2, pl.Stock[k]);
            }
            for (int w = 0; w < L.WealthHistoryLength; w++)
                ds.Set(L.WealthHistory + p * 0xA0 + w * 4, pl.WealthHistory[w]);
            for (int i = 0; i < L.RobotPartCount; i++)
                ds.Set(L.RobotParts + p * 6 + i, pl.RobotParts[i]);
            ds.Set(L.Robot376C + p * 4, pl.Robot376C);
            ds.Set(L.Robot376E + p * 4, pl.Robot376E);
            ds.Set(L.Robot2EF8 + p * 2, pl.Robot2EF8);
            ds.Set(L.Robot377C + p, pl.Robot377C);
            ds.Set(L.Robot40EC + p * 2, pl.Robot40EC);
            ds.Set(L.Robot33DC + p * 2, pl.Robot33DC);
            ds.Set(L.Robot3016 + p * 2, pl.Robot3016);
        }

        for (int i = 0; i < s.Fields.Length; i++)
        {
            ds.Set(L.FieldOwner + i, s.Fields[i].Owner);
            ds.Set(L.FieldKind + i, s.Fields[i].Kind);
            ds.Set(L.FieldQuality + i, s.Fields[i].Quality);
        }
        for (int i = 0; i < L.StationCount; i++)
        {
            ds.Set(L.StationVisited + i, s.Stations[i].Visited);
            ds.Set(L.StationKind + i * 2, s.Stations[i].Kind);
            ds.Set(L.StationValue + i * 2, s.Stations[i].Value);
        }
        for (int k = 0; k < L.WareCount; k++)
            ds.Set(L.WarePrice + k * 2, s.WarePrices[k]);

        ds.Set(L.Weather, s.Weather);
        ds.Set(L.FieldTotal, s.FieldTotal);
        ds.Set(L.Week, s.Week);
        ds.Set(L.Unknown4580, s.Unknown4580);
        ds.Set(L.BaseUnit, s.BaseUnit);
        ds.Set(L.GemPriceFactor, s.GemPriceFactor);
        ds.Set(L.ActivePlayer, s.ActivePlayer);
        ds.Set(L.RandomSeed, s.RandomSeed);
    }

    public static GameState Load(Dgroup ds)
    {
        var s = new GameState();
        var o = s.Settings;
        o.Level = ds.S16(L.Level);
        o.ComputerIq = ds.S16(L.ComputerIq);
        o.OilConsumption = ds.U8(L.OilConsumption);
        o.RobotBattle = ds.U8(L.RobotBattle);
        o.GemTrading = ds.U8(L.GemTrading);
        o.Catastrophes = ds.U8(L.Catastrophes);
        o.PiratePresence = ds.S16(L.PiratePresence);
        o.InitialCash = ds.S16(L.InitialCash);
        o.Target = ds.S32(L.Target);
        o.LottoReturns = ds.S8(L.LottoReturns);
        o.TimeLimit = ds.U8(L.TimeLimit);
        o.LandType = ds.U8(L.LandType);
        o.RobotCycle = ds.S8(L.RobotCycle);
        o.FieldColumns = ds.S16(L.FieldColumns);
        o.FieldRows = ds.S16(L.FieldRows);
        o.Sound = ds.S16(L.Sound);

        for (int p = 0; p < L.PlayerCount; p++)
        {
            var pl = s.Players[p];
            pl.IsComputer = ds.U8(L.IsComputer + p);
            pl.ControlDevice = ds.S8(L.ControlDevice + p);
            pl.Cash = ds.S32(L.Cash + p * 4);
            pl.Rockets = ds.S16(L.Rockets + p * 2);
            pl.Oil = ds.S16(L.Oil + p * 2);
            pl.Gems = ds.S8(L.Gems + p);
            pl.GemCredit = ds.S16(L.GemCredit + p * 2);
            pl.HqX = ds.S8(L.HqX + p);
            pl.HqY = ds.S8(L.HqY + p);
            pl.Station = ds.S16(L.Station + p * 2);
            pl.FatMikeFlag = ds.U8(L.FatMikeFlag + p);
            pl.Unknown40F6 = ds.S16(L.Unknown40F6 + p * 2);
            pl.Unknown4BBE = ds.U8(L.Unknown4BBE + p);
            pl.FieldCount = ds.U8(L.FieldCount + p);
            for (int k = 0; k < 4; k++)
            {
                pl.FieldsByKind[k] = ds.S8(L.FieldsByKind + p * 4 + k);
                pl.Stock[k] = ds.S16(L.Stock + p * 8 + k * 2);
            }
            for (int w = 0; w < L.WealthHistoryLength; w++)
                pl.WealthHistory[w] = ds.S32(L.WealthHistory + p * 0xA0 + w * 4);
            for (int i = 0; i < L.RobotPartCount; i++)
                pl.RobotParts[i] = ds.U8(L.RobotParts + p * 6 + i);
            pl.Robot376C = ds.U16(L.Robot376C + p * 4);
            pl.Robot376E = ds.U16(L.Robot376E + p * 4);
            pl.Robot2EF8 = ds.U16(L.Robot2EF8 + p * 2);
            pl.Robot377C = ds.U8(L.Robot377C + p);
            pl.Robot40EC = ds.U16(L.Robot40EC + p * 2);
            pl.Robot33DC = ds.U16(L.Robot33DC + p * 2);
            pl.Robot3016 = ds.U16(L.Robot3016 + p * 2);
        }

        for (int i = 0; i < s.Fields.Length; i++)
        {
            s.Fields[i].Owner = ds.S8(L.FieldOwner + i);
            s.Fields[i].Kind = ds.S8(L.FieldKind + i);
            s.Fields[i].Quality = ds.S8(L.FieldQuality + i);
        }
        for (int i = 0; i < L.StationCount; i++)
        {
            s.Stations[i].Visited = ds.U8(L.StationVisited + i);
            s.Stations[i].Kind = ds.S16(L.StationKind + i * 2);
            s.Stations[i].Value = ds.S16(L.StationValue + i * 2);
        }
        for (int k = 0; k < L.WareCount; k++)
            s.WarePrices[k] = ds.S16(L.WarePrice + k * 2);

        s.Weather = ds.S8(L.Weather);
        s.FieldTotal = ds.S8(L.FieldTotal);
        s.Week = ds.S16(L.Week);
        s.Unknown4580 = ds.S8(L.Unknown4580);
        s.BaseUnit = ds.S32(L.BaseUnit);
        s.GemPriceFactor = ds.S32(L.GemPriceFactor);
        s.ActivePlayer = ds.S8(L.ActivePlayer);
        s.RandomSeed = ds.U32(L.RandomSeed);
        return s;
    }
}
