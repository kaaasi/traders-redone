namespace Traders.Core.State;

/// <summary>
/// Persistent game state of the original, field by field as in DGROUP (see <see cref="DgroupLayout"/>).
/// Types follow the original widths and signedness exactly (the save routine 2336:000A tells
/// signed from unsigned); semantic helpers are kept out so that every DS value round-trips.
/// The class is a plain data container (System.Text.Json serialisable).
/// </summary>
public sealed class GameState
{
    public GameSettings Settings { get; set; } = new();

    public PlayerState[] Players { get; set; } =
        Enumerable.Range(0, DgroupLayout.PlayerCount).Select(_ => new PlayerState()).ToArray();

    /// <summary>Field grid, index <see cref="DgroupLayout.FieldIndex"/> (x*6+y), always 9×6 cells.</summary>
    public FieldCell[] Fields { get; set; } =
        Enumerable.Range(0, DgroupLayout.FieldColumnsMax * DgroupLayout.FieldRowsMax).Select(_ => new FieldCell()).ToArray();

    public StationState[] Stations { get; set; } =
        Enumerable.Range(0, DgroupLayout.StationCount).Select(_ => new StationState()).ToArray();

    /// <summary>DS:4898 price per ware unit (wealth = stock/10 * price).</summary>
    public short[] WarePrices { get; set; } = new short[DgroupLayout.WareCount];

    /// <summary>DS:40E6 weather 0..5 (SUNNY, WINDY, RAINY, HEAT-WAVE, MONSOON, BLIZZARD).</summary>
    public sbyte Weather { get; set; }

    /// <summary>DS:2F05 number of fields (columns × rows), set when the land is generated.</summary>
    public sbyte FieldTotal { get; set; }

    /// <summary>DS:4B9A week counter; selects the wealth history slot (mod 40).</summary>
    public short Week { get; set; }

    /// <summary>DS:4580, init -1; meaning not yet confirmed.</summary>
    public sbyte Unknown4580 { get; set; }

    /// <summary>DS:3CF0 initial cash / 40; unit for gem values.</summary>
    public int BaseUnit { get; set; }

    /// <summary>DS:4436 gem price factor, random(120)+40 at the start of a game.</summary>
    public int GemPriceFactor { get; set; }

    /// <summary>DS:37AA current player (also the original's player loop variable). Not saved.</summary>
    public sbyte ActivePlayer { get; set; }

    /// <summary>DS:10D2 state of random(). Not part of SAVEGAME.DAT.</summary>
    public uint RandomSeed { get; set; } = 1;

    public FieldCell Field(int x, int y) => Fields[DgroupLayout.FieldIndex(x, y)];

    /// <summary>
    /// State as present in DGROUP right after T.EXE is loaded: initialised data from the EXE,
    /// everything in BSS zero. A new game is then set up by 0E31:0003 (not yet ported).
    /// </summary>
    public static GameState OriginalInitial()
    {
        var s = new GameState();
        sbyte[] devices = [4, 3, 0, 1];
        for (int p = 0; p < DgroupLayout.PlayerCount; p++)
        {
            s.Players[p].IsComputer = 1;
            s.Players[p].ControlDevice = devices[p];
        }
        return s;
    }
}

/// <summary>Options from the main, difficulty and preferences menus.</summary>
public sealed class GameSettings
{
    /// <summary>DS:08EA 0..3, -1 = user defined. A level preset sets IQ, oil, robots, gems, catastrophes, pirates.</summary>
    public short Level { get; set; } = 2;
    /// <summary>DS:2E8E 0 DIM, 1 GOOD, 2 SHREWD, 3 MEAN (one value for all computer players).</summary>
    public short ComputerIq { get; set; }
    /// <summary>DS:45BA</summary>
    public byte OilConsumption { get; set; }
    /// <summary>DS:411A</summary>
    public byte RobotBattle { get; set; }
    /// <summary>DS:4BF2</summary>
    public byte GemTrading { get; set; }
    /// <summary>DS:2F30</summary>
    public byte Catastrophes { get; set; }
    /// <summary>DS:0900 0 NONE, 1 LOW, 2 NORM, 3 HIGH.</summary>
    public short PiratePresence { get; set; } = 1;
    /// <summary>DS:08F2</summary>
    public short InitialCash { get; set; } = 500;
    /// <summary>DS:08F4</summary>
    public int Target { get; set; } = 20000;
    /// <summary>DS:08F9 0 LOW, 1 NORM, 2 HIGH.</summary>
    public sbyte LottoReturns { get; set; } = 1;
    /// <summary>DS:08F8 10/20/30.</summary>
    public byte TimeLimit { get; set; } = 10;
    /// <summary>DS:08F1 0 RANDOM, 1 REAL. Not saved.</summary>
    public byte LandType { get; set; }
    /// <summary>DS:08EC 1..4.</summary>
    public sbyte RobotCycle { get; set; } = 1;
    /// <summary>DS:08FC 4..9.</summary>
    public short FieldColumns { get; set; } = 8;
    /// <summary>DS:08FE 3..6.</summary>
    public short FieldRows { get; set; } = 6;
    /// <summary>DS:10A7 0/1. Not saved.</summary>
    public short Sound { get; set; } = 1;

    /// <summary>Default values as initialised data in T.EXE (BSS options start at 0).</summary>
    public static GameSettings OriginalDefaults() => new();
}

public sealed class PlayerState
{
    /// <summary>DS:0840 0 human, 1 computer.</summary>
    public byte IsComputer { get; set; } = 1;
    /// <summary>DS:083C 0 computer, 1 joystick 1, 2 joystick 2, 3 mouse, 4 left keys, 5 right keys.</summary>
    public sbyte ControlDevice { get; set; }
    /// <summary>DS:3CD2</summary>
    public int Cash { get; set; }
    /// <summary>DS:4BAC</summary>
    public short Rockets { get; set; }
    /// <summary>DS:4520</summary>
    public short Oil { get; set; }
    /// <summary>DS:37AF</summary>
    public sbyte Gems { get; set; }
    /// <summary>DS:300E gem trade account (tentative name).</summary>
    public short GemCredit { get; set; }
    /// <summary>DS:40FE, -1 = no HQ.</summary>
    public sbyte HqX { get; set; }
    /// <summary>DS:4102</summary>
    public sbyte HqY { get; set; }
    /// <summary>DS:450E chosen station 0..11, -1 = none.</summary>
    public short Station { get; set; }
    /// <summary>DS:4BC2 (tentative).</summary>
    public byte FatMikeFlag { get; set; }
    /// <summary>DS:40F6 (meaning open).</summary>
    public short Unknown40F6 { get; set; }
    /// <summary>DS:4BBE (meaning open).</summary>
    public byte Unknown4BBE { get; set; }
    /// <summary>DS:4BA8 number of fields owned.</summary>
    public byte FieldCount { get; set; }
    /// <summary>DS:2EE7 fields per kind 0..3.</summary>
    public sbyte[] FieldsByKind { get; set; } = new sbyte[4];
    /// <summary>DS:3CF4 stock per ware.</summary>
    public short[] Stock { get; set; } = new short[DgroupLayout.WareCount];
    /// <summary>DS:411C total wealth per week (ring of 40).</summary>
    public int[] WealthHistory { get; set; } = new int[DgroupLayout.WealthHistoryLength];
    /// <summary>DS:43CE (robot battle, tentative).</summary>
    public byte[] RobotParts { get; set; } = new byte[DgroupLayout.RobotPartCount];
    public ushort Robot376C { get; set; }
    public ushort Robot376E { get; set; }
    public ushort Robot2EF8 { get; set; }
    public byte Robot377C { get; set; }
    public ushort Robot40EC { get; set; }
    public ushort Robot33DC { get; set; }
    public ushort Robot3016 { get; set; }
}

public sealed class FieldCell
{
    /// <summary>DS:4442 owning player, -1 = nobody.</summary>
    public sbyte Owner { get; set; }
    /// <summary>DS:2EB1 -2 empty, 0..2 production, 3 rocket, 4 HQ.</summary>
    public sbyte Kind { get; set; }
    /// <summary>DS:45DA land quality.</summary>
    public sbyte Quality { get; set; }
}

public sealed class StationState
{
    /// <summary>DS:2F1A</summary>
    public byte Visited { get; set; }
    /// <summary>DS:44F4 0..2.</summary>
    public short Kind { get; set; }
    /// <summary>DS:348A -1, 10 or 0..3.</summary>
    public short Value { get; set; }
}
