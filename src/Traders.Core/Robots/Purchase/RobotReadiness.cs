using Traders.Core.State;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Core.Robots.Purchase;

/// <summary>
/// "Does anyone fight?" after the robot purchase (docs/routines/1C68_000B_roboterphase.md): 1C68:006B with the per-player
/// readiness 0EDE:0322 and its part check 0EDE:1826. Pure functions on the data segment, no random numbers; R11 calls
/// 0322 and 1826 again from the battle.
/// </summary>
public static class RobotReadiness
{
    /// <summary>Part values below this mean "not bought" (1C68:099D stores 10 * level + 9).</summary>
    public const int BoughtPartMinimum = 10;

    private static int Player(Dgroup d) => d.S8(L.ActivePlayer);

    /// <summary>
    /// 0EDE:1826 – true (AL = 1) if all six part bytes DS:43CE + 6p of player DS:37AA are below 10 (unsigned compare),
    /// false (AL = 0) at the first part of 10 or more.
    /// </summary>
    // [ORIGINAL] 0EDE:1826
    public static bool HasNoParts(Dgroup d)
    {
        int row = Int16Math.S(Player(d) * L.RobotPartCount);
        for (int i = 0; i <= 5; i++)
            if (d.U8(L.RobotParts + Int16Math.S(row + i)) >= BoughtPartMinimum) return false;
        return true;
    }

    /// <summary>
    /// 0EDE:0322 – DS:45D2[p] for player DS:37AA: 0 without bought parts (1826), otherwise 1 if the word DS:376C + 4p is
    /// not 0. No result (AL holds the player index afterwards).
    /// </summary>
    // [ORIGINAL] 0EDE:0322
    public static void UpdateFights(Dgroup d)
    {
        int p = Player(d);
        bool fights = !HasNoParts(d) && d.U16(L.Robot376C + Int16Math.S(p * 4)) != 0;
        d.Set(L.RobotFights + p, (byte)(fights ? 1 : 0));
    }

    /// <summary>
    /// 1C68:006B – updates DS:45D2 of all four players (0322, loop variable DS:37AA ends at 4) and returns true (AL = 1)
    /// if at least one of them fights.
    /// </summary>
    // [ORIGINAL] 1C68:006B
    public static bool AnyoneFights(Dgroup d)
    {
        bool any = false;
        for (d.Set(L.ActivePlayer, (sbyte)0); Player(d) < 4; d.Set(L.ActivePlayer, (sbyte)(Player(d) + 1)))
        {
            UpdateFights(d);
            if (d.U8(L.RobotFights + Player(d)) == 1) any = true;
        }
        return any;
    }
}
