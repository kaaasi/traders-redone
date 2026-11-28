using Traders.Core.Input;
using Traders.Core.State;
using static Traders.Core.Ai.RobotLineupLayout;

namespace Traders.Core.Ai;

/// <summary>
/// Lineup AI of the robot phase, the whole segment 12C8 (R10 lane aufstellung, part 1; finding
/// docs/routines/12C8_000B_aufstellung_ki.md). Pure logic on the data segment: no random, no picture, no sound, no input.
/// 12C8:000B resets the block marks once per robot week (from 1C68:000B), 12C8:003B plans one computer player at the start
/// of the lineup (from 0535:00B4), 12C8:0539 writes the control bits DS:37C3 of all computers in every frame of the lineup
/// loop (from 0535:012B, before 0E9E:000E reads them). The current player is DS:37AA (sbyte); per-player index arithmetic is
/// 16-bit as in the original (bx + displacement wraps at 64 KiB).
/// </summary>
public static class RobotLineupAi
{
    private static short P(Dgroup d) => d.S8(DgroupLayout.ActivePlayer);

    /// <summary>Address <c>table + index</c> with 16-bit wrap.</summary>
    private static int At(int table, int index) => Int16Math.U(table + index);

    /// <summary>Byte of a per-player byte table (index p).</summary>
    private static int Byte4(int table, short p) => At(table, p);

    /// <summary>Word of a per-player word table (index p·2).</summary>
    private static int Word4(int table, short p) => At(table, Int16Math.S(p << 1));

    /// <summary>Byte of a per-player [4][4] ring table (index p·4 + ring).</summary>
    private static int Ring(int table, short p, short ring) => At(table, Int16Math.S((p << 2) + ring));

    /// <summary>
    /// 12C8:000B – marks rows 0, 1, 6 and 7 of the block marks <see cref="RobotLineupLayout.Marks"/> as C3h (blocks 0, 1, 6, 7)
    /// and clears rows 2..5: the edges of the coarse grid count as marked.
    /// </summary>
    // [ORIGINAL] 12C8:000B
    public static void ResetMarks(Dgroup d)
    {
        for (sbyte row = 0; row <= 7; row++)
            d.Set(At(Marks, row), row <= 1 || row >= 6 ? (byte)0xC3 : (byte)0);
    }

    /// <summary>
    /// 12C8:003B – plans the current computer player at the start of the lineup: <c>Pass[p] = 0</c>, then
    /// <see cref="MarkQuadrant"/> (005A), <see cref="PlanRings"/> (030B) and <see cref="ChooseTarget"/> (06D6).
    /// </summary>
    // [ORIGINAL] 12C8:003B
    public static void Plan(Dgroup d)
    {
        d.Set(Byte4(Pass, P(d)), (byte)0);
        MarkQuadrant(d);
        PlanRings(d);
        ChooseTarget(d);
    }

    /// <summary>
    /// 12C8:005A – walks the blocks of rings 0..3 around the player's corner (<see cref="RingBlock"/> with step 0..2·ring) and
    /// calls <see cref="MarkBetween"/> for every block that is in use in <see cref="RobotLineupLayout.Blocks"/>.
    /// </summary>
    // [ORIGINAL] 12C8:005A
    public static void MarkQuadrant(Dgroup d)
    {
        for (short ring = 0; ring <= 3; ring++)
            for (short step = 0; Int16Math.S(ring << 1) >= step; step++)
            {
                var (x, y) = RingBlock(d, ring, step);
                if (d.U8(At(Blocks, Int16Math.S((x << 3) + y))) != 0)
                    MarkBetween(d, x, y);
            }
    }

    /// <summary>
    /// 12C8:00B5(ring, step) – block of ring <paramref name="ring"/> (Chebyshev distance from the corner) at position
    /// <paramref name="step"/> 0..2·ring along the L from the x edge to the y edge: step ≤ ring gives
    /// (corner x + dx·ring, corner y + dy·step), otherwise (corner x + dx·(2·ring − step), corner y + dy·ring); low words of
    /// the 16-bit imul, direction and corner bytes signed.
    /// </summary>
    // [ORIGINAL] 12C8:00B5
    public static (short X, short Y) RingBlock(Dgroup d, short ring, short step)
    {
        short p = P(d);
        short dx = d.S8(Byte4(DirectionX, p)), dy = d.S8(Byte4(DirectionY, p));
        short cx = d.S8(Byte4(CornerX, p)), cy = d.S8(Byte4(CornerY, p));
        if (step <= ring)
            return (Int16Math.S(Int16Math.S(dx * ring) + cx), Int16Math.S(Int16Math.S(dy * step) + cy));
        short back = Int16Math.S((ring << 1) - step);
        return (Int16Math.S(Int16Math.S(back * dx) + cx), Int16Math.S(Int16Math.S(dy * ring) + cy));
    }

    /// <summary>
    /// 12C8:015C(x, y) – looks at the eight neighbours of block (x, y) that lie in the grid and are in use: if one of them has
    /// distance 4 and one distance 2 from the corner (<see cref="Distance"/>), the block itself and every such neighbour at
    /// distance 2 are marked (<see cref="SetMark"/>).
    /// </summary>
    // [ORIGINAL] 12C8:015C
    public static void MarkBetween(Dgroup d, short x, short y)
    {
        bool far = false, near = false;
        for (short k = 0; k <= 7; k++)
        {
            var (nx, ny, r) = Neighbour(d, x, y, k);
            if (r == -1 || !InUse(d, nx, ny)) continue;
            if (r == 4) far = true;
            if (r == 2) near = true;
        }
        if (!far || !near) return;
        SetMark(d, x, y, 1);
        for (short k = 0; k <= 7; k++)
        {
            var (nx, ny, r) = Neighbour(d, x, y, k);
            if (r == -1 || !InUse(d, nx, ny)) continue;
            if (r == 2) SetMark(d, nx, ny, 1);
        }
    }

    private static (short X, short Y, short Distance) Neighbour(Dgroup d, short x, short y, short k)
    {
        short nx = Int16Math.S(x + d.S8(At(NeighbourX, k)));
        short ny = Int16Math.S(y + d.S8(At(NeighbourY, k)));
        return (nx, ny, Distance(d, nx, ny));
    }

    private static bool InUse(Dgroup d, short x, short y) => d.U8(At(Blocks, Int16Math.S((x << 3) + y))) != 0;

    /// <summary>
    /// 12C8:024E(x, y) – Chebyshev distance of block (x, y) from the current player's corner, max(|x − cx|, |y − cy|) with
    /// signed comparisons; −1 when x or y is outside 0..7.
    /// </summary>
    // [ORIGINAL] 12C8:024E
    public static short Distance(Dgroup d, short x, short y)
    {
        if (x < 0 || x > 7 || y < 0 || y > 7) return -1;
        short p = P(d);
        short ax = Abs(Int16Math.S(x - d.S8(Byte4(CornerX, p))));
        short ay = Abs(Int16Math.S(y - d.S8(Byte4(CornerY, p))));
        return ax > ay ? ax : ay;
    }

    private static short Abs(short v) => v < 0 ? Int16Math.S(-v) : v;

    /// <summary>
    /// 12C8:02BB(x, y, on) – <paramref name="on"/> = 1 (low byte): sets bit <c>Bits[x]</c> in <c>Marks[y]</c>; otherwise clears
    /// it (<c>and</c> with FFh − mask).
    /// </summary>
    // [ORIGINAL] 12C8:02BB
    public static void SetMark(Dgroup d, short x, short y, byte on)
    {
        int row = At(Marks, y);
        byte mask = d.U8(At(Bits, x));
        d.Set(row, on == 1 ? (byte)(d.U8(row) | mask) : (byte)(d.U8(row) & (byte)(0xFF - mask)));
    }

    /// <summary>12C8:02EA(x, y) – true when bit <c>Bits[x]</c> is set in <c>Marks[y]</c> (AL = 1, else 0).</summary>
    // [ORIGINAL] 12C8:02EA
    public static bool IsMarked(Dgroup d, short x, short y) => (d.U8(At(Marks, y)) & d.U8(At(Bits, x))) != 0;

    /// <summary>
    /// 12C8:030B – spreads the robot's means over the rings 1..3. Count c[r] = marked blocks of ring r (0..3). Limit
    /// L = min(Robot2EF8[p] / 20, Robot3016[p]) (unsigned), budget e = Robot40EC[p] (signed), q = min(e / 16, L) (signed).
    /// Pass 1, rings 0..3: ring r ≥ 1 with c[r] ≤ q is taken (<c>RingUnits = 2</c>, <c>RingTaken = 1</c>, q −= c[r],
    /// e −= 16·c[r]); others get 0 and 0. Pass 2, rings 3..0: ring 0 with step 4, taken rings with step 8·c[r]: while e ≥ step,
    /// e −= step and <c>RingUnits</c> + 1, stop at 6 (signed byte). Pass 3, rings 1..3 with s = Robot33DC[p]: an empty ring with
    /// 9·c[r] ≤ s gets <c>RingSpare = 1</c> (s −= 9·c[r]), all others 0.
    /// </summary>
    // [ORIGINAL] 12C8:030B
    public static void PlanRings(Dgroup d)
    {
        Span<short> count = stackalloc short[4];
        for (short ring = 0; ring <= 3; ring++)
        {
            count[ring] = 0;
            for (short step = 0; Int16Math.S(ring << 1) >= step; step++)
            {
                var (x, y) = RingBlock(d, ring, step);
                if (IsMarked(d, x, y)) count[ring] = Int16Math.S(count[ring] + 1);
            }
        }

        short p = P(d);
        ushort limit = (ushort)(d.U16(Word4(DgroupLayout.Robot2EF8, p)) / 20);
        ushort cap = d.U16(Word4(DgroupLayout.Robot3016, p));
        if (cap < limit) limit = cap;
        short means = d.S16(Word4(DgroupLayout.Robot40EC, p));
        short quota = Int16Math.Div(means, 16);
        if (quota > (short)limit) quota = (short)limit;

        for (short ring = 0; ring <= 3; ring++)
        {
            if (ring > 0 && count[ring] <= quota)
            {
                d.Set(Ring(RingUnits, p, ring), (byte)2);
                d.Set(Ring(RingTaken, p, ring), (byte)1);
                quota = Int16Math.S(quota - count[ring]);
                means = Int16Math.S(means - Int16Math.S(count[ring] << 4));
            }
            else
            {
                d.Set(Ring(RingUnits, p, ring), (byte)0);
                d.Set(Ring(RingTaken, p, ring), (byte)0);
            }
        }

        for (short ring = 3; ring >= 0; ring--)
        {
            short step;
            if (ring == 0) step = 4;
            else if (d.U8(Ring(RingTaken, p, ring)) == 0) continue;
            else step = Int16Math.S(count[ring] << 3);
            int units = Ring(RingUnits, p, ring);
            while (means >= step)
            {
                means = Int16Math.S(means - step);
                byte v = (byte)(d.U8(units) + 1);
                d.Set(units, v);
                if ((sbyte)v >= 6) break;
            }
        }

        short spare = d.S16(Word4(DgroupLayout.Robot33DC, p));
        for (short ring = 1; ring <= 3; ring++)
        {
            int at = Ring(RingSpare, p, ring);
            short cost = Int16Math.S(count[ring] * 9);
            if (d.U8(Ring(RingUnits, p, ring)) != 0 || cost > spare)
            {
                d.Set(at, (byte)0);
                continue;
            }
            d.Set(at, (byte)1);
            spare = Int16Math.S(spare - cost);
        }
    }

    /// <summary>
    /// Iterations of the search in <see cref="ChooseTarget"/> after which its state provably repeats: the walk over (x, y)
    /// changes nothing but x and y, so more than 256·257 steps without a result mean the original never returns
    /// (only possible when QuadrantMaxX or QuadrantMaxY is FFh, never with the tables of T.EXE).
    /// </summary>
    public const int TargetSearchLimit = 256 * 257 + 1;

    /// <summary>
    /// 12C8:06D6 – chooses the next target cell of the current player in its quadrant. Starts at the low bytes of
    /// <c>PointerX/Y[p]</c> (from the start position: one before the quadrant's first cell) and walks x then y (unsigned bytes)
    /// over the quadrant. After the last cell: <c>Pass[p]</c> + 1, target = start position, <c>Presses = 1</c>. A cell
    /// qualifies when its field value / 8 is 0 or 2, its block ((x − 1) / 3, (y − 1) / 3) is marked and, by pass:
    /// pass 2 – <c>RingSpare</c> of the block's ring set (<c>Presses = 1</c>); otherwise <c>RingUnits</c> of the ring nonzero and
    /// pass 0 – ring taken and x − 2, y − 2 multiples of 3 (block centre, <c>Presses = 1</c>), other passes –
    /// <c>Presses = RingUnits</c>. The target goes to <c>TargetX/Y[p]</c> as sign-extended bytes.
    /// </summary>
    /// <exception cref="InvalidOperationException">The search would never end (see <see cref="TargetSearchLimit"/>).</exception>
    // [ORIGINAL] 12C8:06D6
    public static void ChooseTarget(Dgroup d)
    {
        short p = P(d);
        byte x = d.U8(Word4(PointerX, p)), y = d.U8(Word4(PointerY, p));
        if (d.U8(Byte4(StartX, p)) == x && d.U8(Byte4(StartY, p)) == y)
        {
            x = (byte)(d.U8(Byte4(QuadrantMinX, p)) - 1);
            y = d.U8(Byte4(QuadrantMinY, p));
        }

        int presses = Byte4(Presses, p);
        for (int guard = 0; ; guard++)
        {
            if (guard >= TargetSearchLimit)
                throw new InvalidOperationException("12C8:06D6 never ends: the quadrant walk repeats without a target");
            x = (byte)(x + 1);
            if (x > d.U8(Byte4(QuadrantMaxX, p)))
            {
                x = d.U8(Byte4(QuadrantMinX, p));
                y = (byte)(y + 1);
                if (y > d.U8(Byte4(QuadrantMaxY, p)))
                {
                    int pass = Byte4(Pass, p);
                    d.Set(pass, (byte)(d.U8(pass) + 1));
                    x = d.U8(Byte4(StartX, p));
                    y = d.U8(Byte4(StartY, p));
                    d.Set(presses, (byte)1);
                    break;
                }
            }

            short sx = (sbyte)x, sy = (sbyte)y;
            byte cell = d.U8(At(BattleGrid, Int16Math.S(Int16Math.S(sx * 26) + sy)));
            byte kind = (byte)(cell / 8);
            if (kind != 0 && kind != 2) continue;

            short bx = (sbyte)(byte)Int16Math.Div(Int16Math.S(sx - 1), BlockSide);
            short by = (sbyte)(byte)Int16Math.Div(Int16Math.S(sy - 1), BlockSide);
            if (!IsMarked(d, bx, by)) continue;
            short ring = Distance(d, bx, by);

            byte passNow = d.U8(Byte4(Pass, p));
            if (passNow == 2)
            {
                if (d.U8(Ring(RingSpare, p, ring)) == 0) continue;
                d.Set(presses, (byte)1);
                break;
            }
            if (d.U8(Ring(RingUnits, p, ring)) == 0) continue;
            if (passNow == 0)
            {
                if (d.U8(Ring(RingTaken, p, ring)) == 0) continue;
                if (Int16Math.Mod(Int16Math.S(sx - 2), BlockSide) != 0 || Int16Math.Mod(Int16Math.S(sy - 2), BlockSide) != 0) continue;
                d.Set(presses, (byte)1);
                break;
            }
            d.Set(presses, d.U8(Ring(RingUnits, p, ring)));
            break;
        }

        d.Set(Word4(TargetX, p), (short)(sbyte)x);
        d.Set(Word4(TargetY, p), (short)(sbyte)y);
    }

    /// <summary>Battle field byte[26][26] of lane feld (BattleFieldLayout.Grid), cell (x, y) at 26·x + y.</summary>
    private const int BattleGrid = 0x48F4;

    /// <summary>
    /// 12C8:0539 – control bits of the computer players for one frame of the lineup. For p = 0..3 (written to DS:37AA, ends
    /// at 4), computers only (<c>IsComputer[p] = 1</c>): bits = 0; nothing more while the move or fire delay of 0E9E is above 0
    /// (signed dword). In mode 0Eh: pointer off target → <see cref="SteerToTarget"/>; on target with presses left → bits 10h,
    /// presses − 1; else <see cref="ChooseTarget"/>. Other modes: <see cref="SteerByPass"/>.
    /// </summary>
    // [ORIGINAL] 12C8:0539
    public static void Steer(Dgroup d)
    {
        for (d.Set(DgroupLayout.ActivePlayer, (sbyte)0); d.S8(DgroupLayout.ActivePlayer) < 4;
             d.Set(DgroupLayout.ActivePlayer, unchecked((sbyte)(d.S8(DgroupLayout.ActivePlayer) + 1))))
        {
            short p = P(d);
            if (d.U8(Byte4(DgroupLayout.IsComputer, p)) != 1) continue;
            short px = d.S16(Word4(PointerX, p)), py = d.S16(Word4(PointerY, p));
            int bits = Byte4(ControlReader.Bits, p);
            d.Set(bits, (byte)0);
            if (d.S32(At(ControlReader.MoveDelay, Int16Math.S(p << 2))) > 0) continue;
            if (d.S32(At(ControlReader.FireDelay, Int16Math.S(p << 2))) > 0) continue;
            if (d.U8(Byte4(Mode, p)) != ModePlacing)
            {
                SteerByPass(d);
                continue;
            }
            if (d.S16(Word4(TargetX, p)) != px || d.S16(Word4(TargetY, p)) != py)
            {
                SteerToTarget(d);
                continue;
            }
            int presses = Byte4(Presses, p);
            if (d.U8(presses) > 0)
            {
                d.Set(bits, (byte)0x10);
                d.Set(presses, (byte)(d.U8(presses) - 1));
            }
            else
                ChooseTarget(d);
        }
    }

    /// <summary>
    /// 12C8:0621 – direction bits towards the target (signed words): target x &gt; pointer x → bit 1, &lt; → bit 2;
    /// target y &gt; pointer y → bit 8, &lt; → bit 4 (or-ed into DS:37C3[p]).
    /// </summary>
    // [ORIGINAL] 12C8:0621
    public static void SteerToTarget(Dgroup d)
    {
        short p = P(d);
        short tx = d.S16(Word4(TargetX, p)), ty = d.S16(Word4(TargetY, p));
        short px = d.S16(Word4(PointerX, p)), py = d.S16(Word4(PointerY, p));
        int bits = Byte4(ControlReader.Bits, p);
        if (tx > px) d.Set(bits, (byte)(d.U8(bits) | 1));
        else if (tx < px) d.Set(bits, (byte)(d.U8(bits) | 2));
        if (ty > py) d.Set(bits, (byte)(d.U8(bits) | 8));
        else if (ty < py) d.Set(bits, (byte)(d.U8(bits) | 4));
    }

    /// <summary>
    /// 12C8:08EA – outside mode 0Eh: <c>Wanted[p]</c> &lt; <c>Pass[p]</c> (unsigned) → or bit 1, &gt; → or bit 2, equal →
    /// bits = 10h.
    /// </summary>
    // [ORIGINAL] 12C8:08EA
    public static void SteerByPass(Dgroup d)
    {
        short p = P(d);
        byte wanted = d.U8(Byte4(Wanted, p)), pass = d.U8(Byte4(Pass, p));
        int bits = Byte4(ControlReader.Bits, p);
        if (wanted < pass) d.Set(bits, (byte)(d.U8(bits) | 1));
        else if (wanted > pass) d.Set(bits, (byte)(d.U8(bits) | 2));
        else d.Set(bits, (byte)0x10);
    }
}
