using Traders.Core.Field;
using Traders.Core.State;

namespace Traders.Core.FatMike;

/// <summary>
/// Drawing requests of segment 1F63 (Fat Mike 1F63:000F, contest 1F63:0B03) that the field layer's <see cref="FieldEventKind"/>
/// does not cover. The arguments are the original's in call order.
/// </summary>
public enum FatMikeEventKind
{
    /// <summary>0A96:0089(sx, sy, w, h, dx, dy) – command type 1, copy inside the RAM atlas.</summary>
    CopyAtlas,

    /// <summary>0A96:00E2(sx, sy, w, h, dx, dy) – command type 2, copy page → page.</summary>
    CopyPage,

    /// <summary>0A96:0267(sx, sy, tw, th, dx, dy, w, h) – command type 6, area tiled with a tile of the atlas.</summary>
    Tile,

    /// <summary>0A69:002C(x, y, h, colour) – vertical line.</summary>
    VerticalLine,

    /// <summary>0A69:004D(x, y, text offset, text segment, colour, shadow colour, ox, oy) – shadowed text at a far address.</summary>
    ShadowText,

    /// <summary>0A69:011B(x, y, value low, value high, digits, colour, shadow colour, ox, oy) – shadowed number.</summary>
    ShadowNumber,

    /// <summary>1F63:104B(mask, state) – the indicator picture(s): bit 0 left (x D0h), bit 1 right (x F4h), state 0, 1 or 2.</summary>
    Indicator,

    /// <summary>0609:1879(x, y, rank) – emblem of the player in the fourth argument (DS:37AA at the call).</summary>
    Emblem,

    /// <summary>21FD:05DB(font) – driver font.</summary>
    Font,

    /// <summary>
    /// 0A69:0288(x, y, w, lines offset, lines segment, count, colour, wait) – typed lines from the far pointers at DS:lines; the
    /// characters follow as <see cref="TypeCharacter"/>.
    /// </summary>
    TypeLines,

    /// <summary>0A69:01AD, one character (character, line, index, length) of the last <see cref="TypeLines"/>.</summary>
    TypeCharacter,

    /// <summary>21AF:0472 – all eight objects hidden (the core clears their visible bytes).</summary>
    HideObjects,

    /// <summary>
    /// 0A69:0243(x, y, w, text offset, text segment, colour, wait, final wait) – one typed line centred in w (contest heading);
    /// the characters follow as <see cref="TypeCharacter"/> with line 0.
    /// </summary>
    TypeLine,

    /// <summary>0A96:05FB(x, y, value low, value high, digits, colour, background) – number with a leading "$" (contest prize).</summary>
    Number,

    /// <summary>0A96:0525(x, y, text offset, text segment, colour, background) – text at a far address.</summary>
    Text,

    /// <summary>2295:006D(index, rgb12) – one DAC colour set at once (the flashing 1F63:11AD).</summary>
    DacColour,
}

/// <summary>A drawing request of segment 1F63 with the original's arguments in call order.</summary>
public sealed record FatMikeEvent(FatMikeEventKind Kind, params short[] Args)
{
    public override string ToString() => $"{Kind}({string.Join(", ", Args)})";
}

/// <summary>What Fat Mike 1F63:000F needs from outside the core: outputs, controls, pause, wait loop and texts.</summary>
public interface IFatMikeHost
{
    /// <summary>Requests shared with the field layer (picture, palette, list, pages, objects, field, sound).</summary>
    void Emit(FieldEvent e);

    /// <summary>The segment's own drawing requests.</summary>
    void Emit(FatMikeEvent e);

    /// <summary>0E9E:037A – reads the controls and writes the actions DS:3308 (ControlReader.ReadActions).</summary>
    void ReadControls(Dgroup d);

    /// <summary>21FD:0245 – pause key check, frame by frame (<see cref="Planting.Pause"/>).</summary>
    IEnumerable<FieldWait> PauseCheck(Dgroup d);

    /// <summary>21FD:0004(mode) – the common wait loop of lane bilanz, frame by frame (Fat Mike: mode 0 after the field part).</summary>
    IEnumerable<FieldWait> WaitLoop(Dgroup d, OriginalRandom random, short mode);

    /// <summary>Zero-terminated string at a far address, without the terminator (typed texts of the text table).</summary>
    byte[] FarString(ushort offset, ushort segment);
}

/// <summary>What the contest 1F63:0B03 needs besides Fat Mike's host: the second field round 0609:0080.</summary>
public interface IContestHost : IFatMikeHost
{
    /// <summary>
    /// 0609:0080, frame by frame: the core's <see cref="ContestPhase.SecondRoundFrames"/> on the field's host (computer turn,
    /// helpers, controls, pause and the notice's texts); a test host may record the call instead.
    /// </summary>
    IEnumerable<FieldWait> SecondRound(Dgroup d, OriginalRandom random);
}
