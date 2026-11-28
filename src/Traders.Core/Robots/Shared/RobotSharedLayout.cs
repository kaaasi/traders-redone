namespace Traders.Core.Robots.Shared;

/// <summary>
/// DGROUP offsets used only by the shared robot helpers (docs/routines/1C68_0A28_roboterhilfen.md,
/// docs/routines/150F_1B56_kampfstufe.md). Names are tentative. The robot fields 18-25 (<see cref="State.DgroupLayout.RobotParts"/>,
/// <see cref="State.DgroupLayout.Robot376C"/>) and the current player stay in <see cref="State.DgroupLayout"/>.
/// </summary>
public static class RobotSharedLayout
{
    // Read by 1C68:0A28
    public const int ItemPrice = 0x0E8F;       // word[12]   base price per purchase item 0..11 (initialised data)
    public const int UpgradePrice = 0x0E9F;    // word[4]    = ItemPrice + 8 * 2: price of items 8..11, indexed by the owned one
    public const int OwnedUpgrade = 0x2F01;    // sbyte[4]   per player: owned item of group 8..11 (0..3), 0xFF = none

    // Written by 150F:1B56
    public const int BattleLevel = 0x40E7;     // byte[4]    per player: 0 while Robot376C <= 5, else weighted part average
}
