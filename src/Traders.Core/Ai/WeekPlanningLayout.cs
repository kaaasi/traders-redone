namespace Traders.Core.Ai;

/// <summary>
/// DGROUP offsets written or read only by the week planning 11A3 (docs/routines/11A3_0000_wochenplanung.md). Names are
/// tentative; the formulas are documented in the finding. Shared offsets stay in <see cref="State.DgroupLayout"/>.
/// </summary>
public static class WeekPlanningLayout
{
    public const int Ware3Rank = 0x2C74;       // word[4]     3 - number of others with ware-3 stock <= own (007D)
    public const int Ware3Lead = 0x2C7C;       // word        stock of rank 0 minus stock of the last rank 1, 0 on a shared lead
    public const int TargetPlayer = 0x4580;    // sbyte       player the computers aim at, -1 = none (0180)
    public const int QualityShare = 0x2C4C;    // word[4][3]  stride 8: percent of good land per kind (043F)
    public const int MainKind = 0x40F6;        // word[4]     main production kind 0..2, -1 for a human without one
    public const int PlanFields = 0x2C44;      // word[4]     planned fields per kind, [3] = rocket fields (078A)
    public const int PlanFieldsLimited = 0x2C6C; // word[4]   planned fields after the stock limits, [3] = rockets (078A)
    public const int FieldSurplus = 0x303E;    // word[4]     own fields minus planned fields per kind (078A)
    public const int RocketShortfall = 0x3046; // word[4]     max(0, -FieldSurplus[3]) per player (104B)
    public const int PlantList = 0x2F31;       // byte[4][54] stride 54: planned kind per entry (0F2E)
    public const int PlantListX = 0x4654;      // byte[4][54] stride 54: column of the entry
    public const int PlantListY = 0x472C;      // byte[4][54] stride 54: row of the entry
    public const int PlantListLength = 0x48EC; // word[4]
    public const int WareNeed = 0x4560;        // word[4][4]  stride 8: wares needed per ware (104B)
    public const int PriceLimit = 0x452A;      // word[4][4]  stride 8: price limit per ware, [3] = need of ware 3 (104B)
    public const int SecondWare = 0x08E7;      // sbyte[3]    second ware per kind (initialised data 1, 2, 0)
    public const int PlanFree = 100;           // value of a planned cell without a plan in the grid DS:48F4
    public const int PlantListStride = 0x36;
}
