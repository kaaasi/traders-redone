using Traders.Core.State;

namespace Traders.Core.Robots.Field;

/// <summary>
/// Cell helpers of the battle field, segment 1E7E (finding docs/routines/1E7E_0004_kampffeld.md): each one writes cells of
/// the grid <see cref="BattleFieldLayout.Grid"/> and appends the drawing commands of the original, in its order, to the
/// caller's list. No random numbers, no other state except <see cref="BattleFieldLayout.RobotFrame"/> (08F1).
/// Called by the placement 0535:041F (083F, 0870, 088D), the field 1E7E:0004 (08F1) and in R11 by the battle 0EDE.
/// All arithmetic is the original's 16-bit arithmetic; data segment addresses wrap at 64 KB.
/// </summary>
public static class FieldCells
{
    /// <summary>
    /// [ORIGINAL] 1E7E:0DB3(sx, sy, x, y, value) – one cell: 0A96:0311 blits the 8-pixel-wide tile from (sx, sy) to
    /// (8·x + 38h, max(8·y − 4, 0)), 8 lines high (4 in the half rows y = 0, where the source starts 4 lines lower, and
    /// y = 19h); then <c>grid[26·x + y] = value</c> (low byte of the fifth argument).
    /// </summary>
    public static void Cell(Dgroup d, ushort sx, ushort sy, ushort x, ushort y, byte value, IList<FieldCommand> commands)
    {
        ushort height = 8;
        ushort ty = unchecked((ushort)((y << 3) + 0xFFFC));
        if ((short)ty < 0) ty = 0;
        if (y == 0)
        {
            sy = unchecked((ushort)(sy + 4));
            height = 4;
        }
        else if (y == 0x19)
        {
            height = 4;
        }

        commands.Add(new FieldCommand(FieldCommandKind.Blit8, sx, sy, height, unchecked((ushort)((x << 3) + 0x38)), ty));
        d.Set(unchecked((ushort)(BattleFieldLayout.Grid + (ushort)(x * BattleFieldLayout.Stride) + y)), value);
    }

    /// <summary>
    /// [ORIGINAL] 1E7E:083F(x, y, kind) – cell of kind <paramref name="kind"/> (0535 passes 0…5): tile (8·kind + 50h, 0),
    /// grid value (kind + 7)·8 + 3 as a byte.
    /// </summary>
    public static void PlaceKind(Dgroup d, ushort x, ushort y, ushort kind, IList<FieldCommand> commands) =>
        Cell(d, unchecked((ushort)((kind << 3) + 0x50)), 0, x, y, unchecked((byte)((((byte)(kind + 7)) << 3) + 3)), commands);

    /// <summary>[ORIGINAL] 1E7E:0870(x, y) – tile (20h, 0), grid value 1Ah.</summary>
    public static void PlaceBlock(Dgroup d, ushort x, ushort y, IList<FieldCommand> commands) =>
        Cell(d, 0x20, 0, x, y, 0x1A, commands);

    /// <summary>
    /// [ORIGINAL] 1E7E:088D(x, y) – marker of the player <see cref="DgroupLayout.ActivePlayer"/> (signed byte p): tile
    /// (8·p + 20h, 30h), grid value 23h.
    /// </summary>
    public static void PlaceMarker(Dgroup d, ushort x, ushort y, IList<FieldCommand> commands) =>
        Cell(d, unchecked((ushort)(((short)d.S8(DgroupLayout.ActivePlayer) << 3) + 0x20)), 0x30, x, y, 0x23, commands);

    /// <summary>
    /// [ORIGINAL] 1E7E:08F1(player, frame) – the player's robot, 2×2 cells at (RobotX[p] + i, RobotY[p] + j), j outer,
    /// i inner: tile (78h·frame + 8·i, 8·j + 8), grid value <c>FrameCellValue[frame]</c>. Then, if the label index read at
    /// the start is not −1 and frame ≠ 1, the label text (0A96:0525) at (8·RobotX[p] + 3Eh, 8·RobotY[p] + 6) in
    /// <c>LabelColour[p]</c> without shadow; at last <c>RobotFrame[p] = frame</c>.
    /// </summary>
    public static void DrawRobot(Dgroup d, ushort player, byte frame, IList<FieldCommand> commands)
    {
        sbyte label = d.S8(At(BattleFieldLayout.RobotLabel, player));
        for (ushort j = 0; j <= 1; j++)
        {
            for (ushort i = 0; i <= 1; i++)
            {
                byte value = d.U8(At(BattleFieldLayout.FrameCellValue, frame));
                ushort y = (ushort)(d.U8(At(BattleFieldLayout.RobotY, player)) + j);
                ushort x = (ushort)(d.U8(At(BattleFieldLayout.RobotX, player)) + i);
                Cell(d, unchecked((ushort)(frame * 0x78 + (i << 3))), (ushort)((j << 3) + 8), x, y, value, commands);
            }
        }

        if (label != -1 && frame != 1)
        {
            ushort entry = unchecked((ushort)(label * 4));
            commands.Add(new FieldCommand(FieldCommandKind.Text,
                (ushort)((d.U8(At(BattleFieldLayout.RobotX, player)) << 3) + 0x3E),
                (ushort)((d.U8(At(BattleFieldLayout.RobotY, player)) << 3) + 6),
                d.U16(At(BattleFieldLayout.LabelText, entry)),
                d.U16(At(BattleFieldLayout.LabelText + 2, entry)),
                d.U8(At(BattleFieldLayout.LabelColour, player)),
                0xFFFF));
        }

        d.Set(At(BattleFieldLayout.RobotFrame, player), frame);
    }

    private static ushort At(int table, ushort index) => unchecked((ushort)(table + index));
}
