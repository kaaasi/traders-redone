using Traders.Core.Rules;
using Traders.Core.State;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Core.Robots.Purchase;

/// <summary>
/// Data steps of the robot purchase 1C68:00A5 without screen and input (docs/routines/1C68_000B_roboterphase.md): budget
/// and start state of every player (01A8 with 02ED), the take-over of the shopping copy into the robot fields (03C8) and
/// the pointer step (0641). All of them work on the data segment exactly like the original, including the player loop
/// variable DS:37AA that they leave behind; none of them draws random numbers.
/// </summary>
public static class RobotPurchaseRules
{
    /// <summary>Lowest and highest budget before the computer bonus (1C68:01DD, 01F6).</summary>
    public const int BudgetMin = 500, BudgetMax = 2000;

    /// <summary>Right edge (exclusive) of the pointer's x position (1C68:0665).</summary>
    public const int PointerLimit = 0x12D;

    private static int Player(Dgroup d) => d.S8(L.ActivePlayer);

    /// <summary>
    /// 1C68:01A8 – for every player p = 0..3 (DS:37AA): budget = wealth (0825:0813) / 10, at least 500 and at most 2000
    /// (32-bit compare with sign); a computer (DS:0840 = 1) adds (budget / 100) * (IQ * 12, cut to 16 bit). Then shop,
    /// item and type to FFh, pointer object to x = 6p + 8Ch and y = p + 6Eh, step and move delay to 0, mode 0Bh
    /// (walking) and the shopping copy of the robot fields (02ED). DS:547A (players done) starts at 0.
    /// </summary>
    // [ORIGINAL] 1C68:01A8
    public static void Setup(Dgroup d)
    {
        d.Set(L.RobotPlayersDone, (short)0);
        for (d.Set(L.ActivePlayer, (sbyte)0); Player(d) < 4; d.Set(L.ActivePlayer, (sbyte)(Player(d) + 1)))
        {
            int p = Player(d);
            int budget = LongMath.Div(Wealth.Total(d), 10);
            if (budget < BudgetMin) budget = BudgetMin;
            if (budget > BudgetMax) budget = BudgetMax;
            if (d.U8(L.IsComputer + p) == 1)
            {
                int factor = Int16Math.S(d.S16(L.ComputerIq) * 12); // imul, then cwd of the low word
                budget = unchecked(budget + LongMath.Mul(LongMath.Div(budget, 100), factor));
            }
            d.Set(L.RobotBudget + 4 * p, budget);
            d.Set(L.RobotShop + p, (byte)0xFF);
            d.Set(L.RobotItem + p, (byte)0xFF);
            d.Set(L.RobotType + p, (byte)0xFF);
            d.Set(L.ObjectX + Int16Math.S(p * L.ObjectStride), Int16Math.S(p * 6 + 0x8C));
            d.Set(L.ObjectY + Int16Math.S(p * L.ObjectStride), Int16Math.S(p + 0x6E));
            d.Set(L.RobotStep + p, (byte)0);
            d.Set(L.MoveDelay + 4 * p, 0);
            d.Set(L.RobotPhaseMode + p, (byte)0x0B);
            SaveStock(d);
        }
    }

    /// <summary>
    /// The robot field behind purchase item k = 1..7 (item 0 has none): offset, stride per player and byte width. 02ED
    /// copies them into the shopping copy DS:48A4 + 16p + 2k, 03C8 back.
    /// </summary>
    public static readonly (int Offset, int Stride, bool IsByte)[] StockFields =
    [
        (L.Robot376C, 4, false), (L.Robot376E, 4, false), (L.Robot2EF8, 2, false), (L.Robot377C, 1, true),
        (L.Robot40EC, 2, false), (L.Robot33DC, 2, false), (L.Robot3016, 2, false),
    ];

    private static int Stock(int p, int item) => L.RobotStock + Int16Math.S(p << 4) + 2 * item;

    /// <summary>
    /// 1C68:02ED – shopping copy of player DS:37AA: item 0 = 0, items 1..7 = robot fields 19..25 (the byte field 377Ch is
    /// widened without sign).
    /// </summary>
    // [ORIGINAL] 1C68:02ED
    public static void SaveStock(Dgroup d)
    {
        int p = Player(d);
        d.Set(Stock(p, 0), (short)0);
        for (int k = 1; k <= 7; k++)
        {
            var (offset, stride, isByte) = StockFields[k - 1];
            int at = offset + Int16Math.S(p * stride);
            d.Set(Stock(p, k), isByte ? (ushort)d.U8(at) : d.U16(at));
        }
    }

    /// <summary>
    /// 1C68:03C8 – for every player p = 0..3 (DS:37AA) the shopping items 1..7 go back into the robot fields 19..25 (the
    /// byte field takes the low byte). Item 0 is not stored.
    /// </summary>
    // [ORIGINAL] 1C68:03C8
    public static void Commit(Dgroup d)
    {
        for (d.Set(L.ActivePlayer, (sbyte)0); Player(d) < 4; d.Set(L.ActivePlayer, (sbyte)(Player(d) + 1)))
        {
            int p = Player(d);
            for (int k = 1; k <= 7; k++)
            {
                var (offset, stride, isByte) = StockFields[k - 1];
                int at = offset + Int16Math.S(p * stride);
                if (isByte) d.Set(at, d.U8(Stock(p, k)));
                else d.Set(at, d.U16(Stock(p, k)));
            }
        }
    }

    /// <summary>
    /// 1C68:0641 – pointer of player DS:37AA: x + step (16 bit, step with sign); stored only if 0 ≤ x &lt; 12Dh.
    /// </summary>
    // [ORIGINAL] 1C68:0641
    public static void MovePointer(Dgroup d)
    {
        int at = L.ObjectX + Int16Math.S(Player(d) * L.ObjectStride);
        short x = Int16Math.S(d.S16(at) + d.S8(L.RobotStep + Player(d)));
        if (x >= 0 && x < PointerLimit) d.Set(at, x);
    }
}
