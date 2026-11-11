namespace Traders.Core.State;

/// <summary>
/// Offsets of the game state in the original data segment (DGROUP = load segment + 0x28DC)
/// of T.EXE (SHA-256 b73a2170...af0185). Derivation and evidence per field:
/// docs/analyse/DATENLAYOUT.md. Names marked "(tentative)" there are not yet confirmed.
/// Per-player arrays are indexed by the player number 0..3 with the stride given in the
/// comment; the field grid is indexed by x*6+y (x = column 0..8, y = row 0..5).
/// </summary>
public static class DgroupLayout
{
    /// <summary>Paragraph of DGROUP relative to the load image.</summary>
    public const int Segment = 0x28DC;

    /// <summary>Bytes from DS:0000 up to the stack segment (SS = 0x2E66), initialised data + BSS.</summary>
    public const int Size = 0x58A0;

    public const int PlayerCount = 4;
    public const int WareCount = 4;
    public const int FieldColumnsMax = 9;
    public const int FieldRowsMax = 6;
    public const int StationCount = 12;
    public const int WealthHistoryLength = 40;
    public const int RobotPartCount = 6;

    // ---- settings (initialised data, defaults in T.EXE) -------------------------------
    public const int ControlDevice = 0x083C;   // byte[4]  signed  0..5 (computer/joy1/joy2/mouse/left keys/right keys)
    public const int IsComputer = 0x0840;      // byte[4]  0 = human, 1 = computer
    public const int Level = 0x08EA;           // word     0..3, -1 = user defined
    public const int RobotCycle = 0x08EC;      // byte     1..4
    public const int LandType = 0x08F1;        // byte     0 RANDOM, 1 REAL (not saved)
    public const int InitialCash = 0x08F2;     // word     500..2000 step 500
    public const int Target = 0x08F4;          // dword    10000..50000 step 10000
    public const int TimeLimit = 0x08F8;       // byte     10..30 step 10
    public const int LottoReturns = 0x08F9;    // byte     0 LOW, 1 NORM, 2 HIGH
    public const int FieldColumns = 0x08FC;    // word     4..9
    public const int FieldRows = 0x08FE;       // word     3..6
    public const int PiratePresence = 0x0900;  // word     0 NONE .. 3 HIGH
    public const int Sound = 0x10A7;           // word     0 off, 1 on (not saved)
    public const int RandomSeed = 0x10D2;      // dword    random() state (not saved!)

    // ---- settings in BSS -----------------------------------------------------------
    public const int ComputerIq = 0x2E8E;      // word     0 DIM, 1 GOOD, 2 SHREWD, 3 MEAN
    public const int Catastrophes = 0x2F30;    // byte     bool
    public const int RobotBattle = 0x411A;     // byte     bool
    public const int OilConsumption = 0x45BA;  // byte     bool
    public const int GemTrading = 0x4BF2;      // byte     bool

    // ---- global game state ------------------------------------------------------------
    public const int FieldOwner = 0x4442;      // sbyte[9*6]  -1 = nobody
    public const int FieldKind = 0x2EB1;       // sbyte[9*6]  -2 = empty, 0..2 production, 3 rocket, 4 HQ
    public const int FieldQuality = 0x45DA;    // sbyte[9*6]  land quality (index into table DS:0902)
    public const int FieldTotal = 0x2F05;      // sbyte       columns*rows
    public const int StationVisited = 0x2F1A;  // byte[12]
    public const int StationKind = 0x44F4;     // word[12]    0..2 (6/4/2 stations per kind)
    public const int StationValue = 0x348A;    // word[12]    -1, 10 or 0..3
    public const int WarePrice = 0x4898;       // word[4]     price per ware unit (init 3)
    public const int Weather = 0x40E6;         // sbyte       0..5 (SUNNY..BLIZZARD)
    public const int Week = 0x4B9A;            // word        round counter (wealth history index)
    public const int Unknown4580 = 0x4580;     // sbyte       init -1 (tentative: player marked by event 11A3)
    public const int BaseUnit = 0x3CF0;        // dword       initial cash / 40
    public const int GemPriceFactor = 0x4436;  // dword       40..160 (random(120)+40 at start)
    public const int ActivePlayer = 0x37AA;    // sbyte       current player / player loop variable (not saved)

    // ---- per player --------------------------------------------------------------------
    public const int HqX = 0x40FE;             // sbyte[4]    -1 = no HQ yet
    public const int HqY = 0x4102;             // sbyte[4]
    public const int Rockets = 0x4BAC;         // word[4]     init 5, max 10 in the shop
    public const int Oil = 0x4520;             // word[4]     init 40 (= full)
    public const int GemCredit = 0x300E;       // word[4]     init 50, gem trade account (unit BaseUnit/100), max 2000
    public const int Gems = 0x37AF;            // sbyte[4]    init 2, max 10
    public const int Cash = 0x3CD2;            // dword[4]    init InitialCash
    public const int Station = 0x450E;         // word[4]     chosen station, -1 = none
    public const int FatMikeFlag = 0x4BC2;     // byte[4]     set/cleared by Fat Mike's rent (tentative)
    public const int Unknown40F6 = 0x40F6;     // word[4]     init 0,2,1,0 (module 11A3, tentative)
    public const int Unknown4BBE = 0x4BBE;     // byte[4]     module 135D (tentative)
    public const int FieldCount = 0x4BA8;      // byte[4]     fields owned
    public const int FieldsByKind = 0x2EE7;    // sbyte[4][4] stride 4: fields per kind 0..3
    public const int Stock = 0x3CF4;           // word[4][4]  stride 8: stock per ware, init 70, max 999
    public const int WealthHistory = 0x411C;   // dword[4][40] stride 0xA0: total wealth per week
    public const int RobotParts = 0x43CE;      // byte[4][6]  stride 6 (robot battle, tentative)
    public const int Robot376C = 0x376C;       // uword[4]    stride 4 (robot battle, tentative)
    public const int Robot376E = 0x376E;       // uword[4]    stride 4 (robot battle, tentative)
    public const int Robot2EF8 = 0x2EF8;       // uword[4]    (robot battle, tentative)
    public const int Robot377C = 0x377C;       // byte[4]     init 1 (robot battle, tentative)
    public const int Robot40EC = 0x40EC;       // uword[4]    init 50 (robot battle, tentative)
    public const int Robot33DC = 0x33DC;       // uword[4]    init 30 (robot battle, tentative)
    public const int Robot3016 = 0x3016;       // uword[4]    init 0 (robot battle, tentative)

    // ---- derived weekly values and loop variables (round 3, track "regeln"; docs/analyse/DATENLAYOUT.md §7) ----
    public const int LoopWare = 0x45D0;        // word        global loop variable (ware / kind), left behind by loops
    public const int ShopOil = 0x4B98;         // word        oil stock of the shop, set by 1777:02DC
    public const int OilPrice = 0x43F4;        // word        set by 1777:02DC, at least 1
    public const int RocketPrice = 0x48A0;     // word        set by 1777:02DC
    public const int LottoPrize = 0x34A2;      // word        set by 1777:02DC, at least 20 (tentative: prize/pool)
    public const int LottoPrice = 0x40CC;      // word        LottoPrize / 32 (tentative: ticket price)
    public const int LandPrice = 0x3746;       // word        set by 1777:02DC, 100..550
    public const int LimitLow = 0x2F26;        // sbyte       min(30, F/2 + 3) from 0609:1178 (meaning open)
    public const int LimitHigh = 0x48A2;       // sbyte       min(30, F/2 + 6) from 0609:1178 (meaning open)
    public const int CurrentWealth = 0x2F06;   // dword[4]    total wealth per player, set by 0825:06CB
    public const int Rank = 0x3D42;            // byte[4]     3 - number of others with wealth <= own (ties count)
    public const int RankUnique = 0x302A;      // byte[4]     rank with ties broken toward the lower player number
    public const int ProductionShares = 0x342C; // byte[4][3] stride 3: percent of the production fields per kind
    public const int ProductionResult = 0x48F4; // byte[9][26] stride 26: production per field (4..60), see 22A3:0003
    public const int ProductionIdle = 0x57F0;  // word        owned fields that produced nothing this week (tentative)
    public const int ProductionActive = 0x57F2; // word       fields that consumed stock and produced (tentative)
    public const int LoopX = 0x2EB0;           // sbyte       global loop variable field column
    public const int LoopY = 0x2F00;           // sbyte       global loop variable field row
    public const int KindWare = 0x08E4;        // sbyte[3]    ware consumed by field kind 0..2 (initialised data)
    public const int QualityTable = 0x0902;    // sbyte[3][4] stride 4: yield bonus per kind and land quality
    public const int WeatherTable = 0x112C;    // sbyte[3][6] stride 6: percent change per kind and weather

    /// <summary>Index into the field grid arrays (x = column, y = row).</summary>
    public static int FieldIndex(int x, int y) => x * FieldRowsMax + y;
}
