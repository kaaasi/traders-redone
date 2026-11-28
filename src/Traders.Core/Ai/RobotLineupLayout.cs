namespace Traders.Core.Ai;

/// <summary>
/// Data segment addresses of the lineup AI, segment 12C8 (R10 lane aufstellung, part 1; finding
/// docs/routines/12C8_000B_aufstellung_ki.md, docs/analyse/DATENLAYOUT.md section "R10 aufstellung"). The battle field is
/// the 26 × 26 grid DS:48F4 of lane feld; the coarse grid is 8 × 8 blocks of 3 × 3 field cells (block = (cell - 1) / 3).
/// Names beyond the access pattern are tentative. Robot fields 18..25 use the names in DgroupLayout; addresses that lane
/// kauf names (R10PL-5) are private here until that lane is merged.
/// </summary>
public static class RobotLineupLayout
{
    // ---- initialised tables (T.EXE) ----

    /// <summary>sbyte[4], block x of the player's home corner in the coarse grid (0, 7, 7, 0).</summary>
    public const int CornerX = 0x07C0;

    /// <summary>sbyte[4], block y of the player's home corner (0, 0, 7, 7).</summary>
    public const int CornerY = 0x07C4;

    /// <summary>sbyte[4], x direction from the corner into the field (+1 / -1).</summary>
    public const int DirectionX = 0x07C8;

    /// <summary>sbyte[4], y direction from the corner into the field (+1 / -1).</summary>
    public const int DirectionY = 0x07CC;

    /// <summary>byte[4], field x of the player's start position (3, 22, 22, 3).</summary>
    public const int StartX = 0x0844;

    /// <summary>byte[4], field y of the player's start position (3, 3, 22, 22).</summary>
    public const int StartY = 0x0848;

    /// <summary>byte[4], lowest field x of the player's quadrant (1 or 13).</summary>
    public const int QuadrantMinX = 0x0854;

    /// <summary>byte[4], highest field x of the player's quadrant (12 or 24).</summary>
    public const int QuadrantMaxX = 0x0858;

    /// <summary>byte[4], lowest field y of the player's quadrant (1 or 13).</summary>
    public const int QuadrantMinY = 0x085C;

    /// <summary>byte[4], highest field y of the player's quadrant (12 or 24).</summary>
    public const int QuadrantMaxY = 0x0860;

    /// <summary>sbyte[8], x offset of the eight neighbours, clockwise from (0, -1): 0, 1, 1, 1, 0, -1, -1, -1.</summary>
    public const int NeighbourX = 0x0878;

    /// <summary>sbyte[8], y offset of the eight neighbours: -1, -1, 0, 1, 1, 1, 0, -1.</summary>
    public const int NeighbourY = 0x0880;

    /// <summary>byte[8], bit masks 01h..80h (also TradeLayout.PlayerBits).</summary>
    public const int Bits = 0x08C6;

    // ---- BSS ----

    /// <summary>word[4], field x of the lineup pointer (set to StartX by 0535:00B4).</summary>
    public const int PointerX = 0x3052;

    /// <summary>word[4], field y of the lineup pointer (set to StartY by 0535:00B4).</summary>
    public const int PointerY = 0x305E;

    /// <summary>byte[8][8], coarse grid, index bx = 8·x + y; nonzero = block in use (written by 1E7E/150F).</summary>
    public const int Blocks = 0x4478;

    /// <summary>byte[8], marked blocks: bit Bits[x] of byte y; 12C8:000B sets rows 0, 1, 6, 7 to C3h.</summary>
    public const int Marks = 0x44C6;

    /// <summary>byte[4][4] stride 4, planned units per ring 0..3 (12C8:030B, at most 6 after the rest is spread).</summary>
    public const int RingUnits = 0x44CE;

    /// <summary>byte[4][4] stride 4, 1 = ring taken in the first pass of 12C8:030B.</summary>
    public const int RingTaken = 0x374C;

    /// <summary>byte[4][4] stride 4, 1 = empty ring still affordable from Robot33DC (12C8:030B).</summary>
    public const int RingSpare = 0x2E98;

    /// <summary>byte[4], pass of the target search 12C8:06D6 (0, 1, 2, …; +1 per full sweep of the quadrant).</summary>
    public const int Pass = 0x4650;

    /// <summary>byte[4], fire presses left at the target (12C8:0539 sends 10h and counts down).</summary>
    public const int Presses = 0x45CC;

    /// <summary>word[4], field x of the target chosen by 12C8:06D6.</summary>
    public const int TargetX = 0x454A;

    /// <summary>word[4], field y of the target chosen by 12C8:06D6.</summary>
    public const int TargetY = 0x4552;

    /// <summary>byte[4], value compared with Pass in 12C8:08EA (outside mode 0Eh; written by 0535).</summary>
    public const int Wanted = 0x2E90;

    /// <summary>byte[4], phase mode of the player; 0Eh = placing robots (0535:00B4). Named by lane kauf (R10PL-5).</summary>
    public const int Mode = 0x43B2;

    /// <summary>Mode value while the player places robots.</summary>
    public const byte ModePlacing = 0x0E;

    /// <summary>Field cells per block side in the coarse grid.</summary>
    public const int BlockSide = 3;
}
