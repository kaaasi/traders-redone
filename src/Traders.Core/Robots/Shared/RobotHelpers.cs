using Traders.Core.State;
using static Traders.Core.Robots.Shared.RobotSharedLayout;

namespace Traders.Core.Robots.Shared;

/// <summary>
/// Shared helpers of the robot phase (R10 lane kauf-ki, part 1; findings docs/routines/1C68_0A28_roboterhilfen.md and
/// docs/routines/150F_1B56_kampfstufe.md). Called by the purchase AI 16D6, the purchase screen 1C68 and, from round 11, the
/// battle 0EDE. All four work on the current player DS:37AA, draw nothing and use no random; only 150F:1B56 writes.
/// Index arithmetic is 16-bit as in the original (bx + displacement wraps at 64 KiB).
/// </summary>
public static class RobotHelpers
{
    /// <summary>Byte <c>RobotParts[p*6 + slot*2 + half]</c> of the current player p, divided by 10 (unsigned byte, signed idiv).</summary>
    private static short PartTenth(Dgroup d, short slot, int half)
    {
        int bx = Int16Math.U(Int16Math.S(d.S8(DgroupLayout.ActivePlayer) * 6) + Int16Math.S(slot << 1));
        return Int16Math.S(d.U8(Int16Math.U(DgroupLayout.RobotParts + half + bx)) / 10);
    }

    /// <summary>
    /// 1C68:0AB0(slot) – value of the part pair <paramref name="slot"/> of the current player:
    /// <c>(RobotParts[p*6+2*slot]/10 + RobotParts[p*6+2*slot+1]/10) * 75</c> (low word of the 16-bit imul).
    /// </summary>
    // [ORIGINAL] 1C68:0AB0
    public static short PartPairValue(Dgroup d, short slot) =>
        Int16Math.S(Int16Math.S(PartTenth(d, slot, 0) + PartTenth(d, slot, 1)) * 75);

    /// <summary>
    /// 1C68:0B02(slot) – mean of the part pair <paramref name="slot"/> of the current player:
    /// <c>(RobotParts[p*6+2*slot]/10 + RobotParts[p*6+2*slot+1]/10) / 2</c>, truncated (signed idiv).
    /// </summary>
    // [ORIGINAL] 1C68:0B02
    public static short PartPairAverage(Dgroup d, short slot) =>
        Int16Math.Div(Int16Math.S(PartTenth(d, slot, 0) + PartTenth(d, slot, 1)), 2);

    /// <summary>
    /// 1C68:0A28(item) – price of purchase item <paramref name="item"/> for the current player, signed comparisons:
    /// item &lt; 8: <c>ItemPrice[item]</c>; item &lt; 12: <c>ItemPrice[item]</c> minus <c>UpgradePrice[OwnedUpgrade[p]]</c> unless
    /// that byte is 0xFF; otherwise with t = (sbyte)(item - 12): <c>(sbyte)((t % 7) &lt;&lt; 1) * 75 - PartPairValue(t / 7)</c>.
    /// </summary>
    // [ORIGINAL] 1C68:0A28
    public static short ItemPrice(Dgroup d, short item)
    {
        if (item < 8)
            return d.S16(Int16Math.U(RobotSharedLayout.ItemPrice + Int16Math.U(item << 1)));
        if (item < 12)
        {
            short price = d.S16(Int16Math.U(RobotSharedLayout.ItemPrice + Int16Math.U(item << 1)));
            byte owned = d.U8(Int16Math.U(OwnedUpgrade + d.S8(DgroupLayout.ActivePlayer)));
            if (owned != 0xFF)
                price = Int16Math.S(price - d.S16(Int16Math.U(UpgradePrice + Int16Math.U(unchecked((sbyte)owned) << 1))));
            return price;
        }
        sbyte t = unchecked((sbyte)(item + 0xF4));        // add al,0F4h: only the low byte of the item counts
        sbyte level = unchecked((sbyte)((t % 7) << 1));    // remainder in DL, shl dl,1
        short quotient = (short)(t / 7);                    // AX after the idiv, passed as word
        return Int16Math.S(Int16Math.S(level * 75) - PartPairValue(d, quotient));
    }

    /// <summary>
    /// 150F:1B56 – sets <c>BattleLevel[p]</c> of the current player p: 0 while the word <c>Robot376C[p*4]</c> is at most 5
    /// (unsigned), else the low byte of <c>(RobotParts[p*6+2]/10 * 4 + RobotParts[p*6+3]/10 + RobotParts[p*6+4]/10 +
    /// RobotParts[p*6+5]/10) / 7</c>. Callers ignore AX.
    /// </summary>
    // [ORIGINAL] 150F:1B56
    public static void UpdateBattleLevel(Dgroup d)
    {
        short p = d.S8(DgroupLayout.ActivePlayer);
        int level = Int16Math.U(BattleLevel + p);
        if (d.U16(Int16Math.U(DgroupLayout.Robot376C + Int16Math.U(p << 2))) <= 5)
        {
            d.Set(level, (byte)0);
            return;
        }
        int bx = Int16Math.U(p * 6);
        short Tenth(int k) => Int16Math.S(d.U8(Int16Math.U(DgroupLayout.RobotParts + k + bx)) / 10);
        short sum = Int16Math.S((Tenth(2) << 2) + Tenth(3) + Tenth(4) + Tenth(5));
        d.Set(level, unchecked((byte)Int16Math.Div(sum, 7)));
    }
}
