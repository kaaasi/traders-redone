using Traders.Core.Field;
using Traders.Core.State;

namespace Traders.Core.Harvest;

/// <summary>
/// Drawing requests of the harvest 22A3:02D3 that the field layer does not have (finding docs/routines/22A3_02D3_ernte.md).
/// The harvest also emits <see cref="FieldEvent"/>s for the calls it shares with the planting phase (lists, pages, objects,
/// fades, sound, board). None of these requests changes game state.
/// </summary>
public enum HarvestEventKind
{
    /// <summary>21FD:05DB(font) – selects the font (its advance and line height DS:3CA8/34A4 are the output's).</summary>
    SetFont,

    /// <summary>
    /// 0A69:0288(x, y, w, lines offset, lines segment, count, colour, wait) – start of the typed weather notice; the characters
    /// follow as <see cref="TypeCharacter"/>.
    /// </summary>
    TypeLines,

    /// <summary>
    /// 0A69:01AD, one character (char, line, index): the list is emptied and the character drawn as text 0A96:0525 (the
    /// character buffer DS:0140 and the position are the output's); 21FD:060D follows as its own event.
    /// </summary>
    TypeCharacter,

    /// <summary>0609:1F04(x, y, w, h) – area tiled with the field tile.</summary>
    TileArea,

    /// <summary>0609:18E5 – empty selection panel of the player at turn.</summary>
    DrawEmptyPanel,

    /// <summary>0609:1912(good, x, y) – goods icon.</summary>
    DrawGoods,

    /// <summary>0A96:057A(x, y, value low, value high, digits, colour, background) – number as text (command type 13).</summary>
    Number,

    /// <summary>0A96:0433(x1, y1, x2, y2, colour) – line (command type 9).</summary>
    Line,

    /// <summary>0A96:04D7(x, y, w, h, colour) – filled rectangle (command type 11).</summary>
    FillRect,
}

/// <summary>A drawing request of the harvest with the original's arguments in call order.</summary>
public sealed record HarvestEvent(HarvestEventKind Kind, params short[] Args)
{
    public override string ToString() => $"{Kind}({string.Join(", ", Args)})";
}

/// <summary>
/// What the harvest needs from outside the core: the outputs (scene, sound), the controls 0E9E:037A, the pause check 21FD:0245
/// and the texts of the weather notice (zero-terminated strings in the far text buffer, an explicit input like the text file
/// of the program start).
/// </summary>
public interface IHarvestHost
{
    void Emit(FieldEvent e);

    void Emit(HarvestEvent e);

    /// <summary>0E9E:037A – reads the controls and writes the actions DS:3308 (ControlReader.ReadActions).</summary>
    void ReadControls(Dgroup d);

    /// <summary>21FD:0245 – pause key check, frame by frame (<see cref="Planting.Pause"/> is the original's).</summary>
    IEnumerable<FieldWait> PauseCheck(Dgroup d);

    /// <summary>
    /// The bytes of the zero-terminated string at the far address (without the terminator); never called for a null pointer.
    /// </summary>
    byte[] FarString(ushort offset, ushort segment);
}
