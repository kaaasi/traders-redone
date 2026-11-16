using Traders.Core.Field;
using Traders.Core.State;

namespace Traders.Core.Extra;

/// <summary>Drawing requests of the extra phase 1DEF that the field layer's <see cref="FieldEventKind"/> does not cover.</summary>
public enum ExtraEventKind
{
    /// <summary>
    /// 0A69:0288(x, y, w, lines offset, lines segment, count, colour, wait) – typed lines from the far pointers at
    /// DS:lines; the characters follow as <see cref="TypeCharacter"/>.
    /// </summary>
    TypeLines,

    /// <summary>0A69:01AD, one character (character, line, index, length) of the last <see cref="TypeLines"/>.</summary>
    TypeCharacter,

    /// <summary>0609:1912(good, x, y) – goods icon 10h × 7.</summary>
    DrawGoods,

    /// <summary>0A96:013B(sx, sy, mx, my, w, h, dx, dy) – masked blit from the atlas (mask at mx, my).</summary>
    MaskedBlit,

    /// <summary>0A96:057A(x, y, value low, value high, digits, colour, background) – a number as text.</summary>
    Number,
}

/// <summary>A drawing request of the extra phase with the original's arguments in call order.</summary>
public sealed record ExtraEvent(ExtraEventKind Kind, params short[] Args)
{
    public override string ToString() => $"{Kind}({string.Join(", ", Args)})";
}

/// <summary>What the extra phase 1DEF:000C needs from outside the core: outputs, controls, pause, wait loop and texts.</summary>
public interface IExtraHost
{
    /// <summary>Requests shared with the field layer (picture, palette, list, pages, objects, board, sound).</summary>
    void Emit(FieldEvent e);

    /// <summary>The phase's own drawing requests.</summary>
    void Emit(ExtraEvent e);

    /// <summary>0E9E:037A – reads the controls and writes the actions DS:3308 (ControlReader.ReadActions).</summary>
    void ReadControls(Dgroup d);

    /// <summary>21FD:0245 – pause key check, frame by frame (<see cref="Planting.Pause"/>).</summary>
    IEnumerable<FieldWait> PauseCheck(Dgroup d);

    /// <summary>21FD:0004(mode) – the common wait loop of lane bilanz, frame by frame (called with mode 0).</summary>
    IEnumerable<FieldWait> WaitLoop(Dgroup d, OriginalRandom random, short mode);

    /// <summary>Zero-terminated string at a far address, without the terminator (texts of the notice, DS:3B5C).</summary>
    byte[] FarString(ushort offset, ushort segment);
}

/// <summary>
/// The three local arrays of 1DEF:000C (its stack frame; the original reaches them through the far pointers it stores in
/// DS:54A6, DS:547C and DS:54A2): per cell of the 6 × 7 board (cell = row·6 + column) the ground tile, the goods lying there
/// and the player who claimed the cell as target.
/// </summary>
public sealed class ExtraBoard
{
    /// <summary>Cells of the board (indices 0 … 29h).</summary>
    public const int Cells = 42;

    /// <summary>word[42] at [bp−54h] (pointer DS:54A6): ground tile 0…3 (1DEF:0173).</summary>
    public short[] Ground { get; } = new short[Cells];

    /// <summary>word[42] at [bp−0A8h] (pointer DS:547C): goods kind 0…3 lying on the cell, −1 = none (1DEF:01AF).</summary>
    public short[] Goods { get; } = new short[Cells];

    /// <summary>byte[42] at [bp−0D2h] (pointer DS:54A2): player who aims at the cell, FFh = nobody (1DEF:020E, 07AD).</summary>
    public byte[] Claims { get; } = new byte[Cells];
}

/// <summary>
/// The stack frame of 1DEF:000C as the original has it: <paramref name="Bp"/> after its <c>mov bp, sp</c> and the stack segment.
/// The core keeps the arrays in <see cref="ExtraBoard"/>; only the far pointers to them, which 000C writes into the data segment,
/// depend on this frame.
/// </summary>
public readonly record struct ExtraFrame(ushort Bp, ushort Ss)
{
    /// <summary>Far pointer DS:54A6 → ground array ([bp−54h]).</summary>
    public ushort GroundOffset => unchecked((ushort)(Bp - 0x54));

    /// <summary>Far pointer DS:547C → goods array ([bp−0A8h]).</summary>
    public ushort GoodsOffset => unchecked((ushort)(Bp - 0xA8));

    /// <summary>Far pointer DS:54A2 → claims array ([bp−0D2h]).</summary>
    public ushort ClaimsOffset => unchecked((ushort)(Bp - 0xD2));
}
