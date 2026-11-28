namespace Traders.Core.Robots.Field;

/// <summary>Drawing routines of the command list 0A96 that the battle field helpers of segment 1E7E enqueue.</summary>
public enum FieldCommandKind
{
    /// <summary>
    /// 0A96:0311 – type 7, blit 8 pixels wide: <see cref="FieldCommand.A"/> = source x, B = source y, C = height,
    /// D = target x, E = target y (argument order of the original).
    /// </summary>
    Blit8,

    /// <summary>
    /// 0A96:0525 – type 12, text: <see cref="FieldCommand.A"/> = x, B = y, C = text offset, D = text segment (far pointer as
    /// stored in the data segment), E = colour, F = shadow colour (0FFFFh = none).
    /// </summary>
    Text,
}

/// <summary>
/// One drawing request of a battle field helper, with the original's arguments in call order. The core does not draw: the
/// caller hands the commands in this order to the command list of the output layers (finding
/// docs/routines/1E7E_0004_kampffeld.md).
/// </summary>
public readonly record struct FieldCommand(FieldCommandKind Kind, ushort A, ushort B, ushort C, ushort D, ushort E, ushort F = 0)
{
    public override string ToString() => Kind == FieldCommandKind.Text
        ? $"{Kind}({A:X4}, {B:X4}, {C:X4}, {D:X4}, {E:X4}, {F:X4})"
        : $"{Kind}({A:X4}, {B:X4}, {C:X4}, {D:X4}, {E:X4})";
}
