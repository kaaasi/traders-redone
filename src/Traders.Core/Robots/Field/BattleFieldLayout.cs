namespace Traders.Core.Robots.Field;

/// <summary>
/// Data segment addresses of the battle field, segment 1E7E (finding docs/routines/1E7E_0004_kampffeld.md,
/// docs/analyse/DATENLAYOUT.md section "R10 feld"). Meanings beyond the access pattern are tentative.
/// </summary>
public static class BattleFieldLayout
{
    /// <summary>
    /// byte[26][26], the battle field: cell (x, y) at <c>Grid + 26·x + y</c>, x and y 0…25. The same buffer holds the
    /// production result (9·26 bytes) outside the robot phase; the field ends right before DS:4B98.
    /// </summary>
    public const int Grid = 0x48F4;

    /// <summary>Number of cells along each side of the field.</summary>
    public const int Size = 26;

    /// <summary>Stride of the first index (x) in the grid.</summary>
    public const int Stride = 26;

    /// <summary>byte[4], field x of the player's robot (read by 1E7E:08F1).</summary>
    public const int RobotX = 0x084C;

    /// <summary>byte[4], field y of the player's robot (read by 1E7E:08F1).</summary>
    public const int RobotY = 0x0850;

    /// <summary>sbyte[4], label of the player's robot: index into <see cref="LabelText"/>, -1 = no label (1E7E:08F1).</summary>
    public const int RobotLabel = 0x2F01;

    /// <summary>far pointer[], label texts (offset word, segment word), indexed by <see cref="RobotLabel"/>.</summary>
    public const int LabelText = 0x0F72;

    /// <summary>byte[4], colour of the player's robot label.</summary>
    public const int LabelColour = 0x0F82;

    /// <summary>byte[], grid value of a robot cell for each robot frame (1E7E:08F1).</summary>
    public const int FrameCellValue = 0x0F86;

    /// <summary>byte[4], robot frame last drawn for the player (written by 1E7E:08F1).</summary>
    public const int RobotFrame = 0x4BB6;
}
