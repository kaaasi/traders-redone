using Traders.Core.Field;
using Traders.Core.State;

namespace Traders.Core.Disasters;

/// <summary>
/// Output calls of the catastrophes 111D:0009 (finding docs/routines/111D_0009_katastrophen.md) in the original's order. None
/// of them changes game state; the scene (lane katastrophen part 2) draws them, the sound layer plays <see cref="Sound"/>.
/// Arguments are the original's words; far pointers into the data segment are given by their offset only.
/// </summary>
public enum DisasterEventKind
{
    /// <summary>186C:0002(n) – load picture n (the step handler records it with ImageTable.RecordLoad).</summary>
    LoadImage,
    /// <summary>21FD:037D(rgb12) – fade out to a colour.</summary>
    FadeOut,
    /// <summary>21FD:0329 – fade in.</summary>
    FadeIn,
    /// <summary>21FD:026C(offset) – base palette from the palette at DS:offset.</summary>
    BasePalette,
    /// <summary>2295:006D(index, rgb12) – one colour register.</summary>
    SetColour,
    /// <summary>0A96:001B – empty the command list.</summary>
    ClearList,
    /// <summary>0609:1ED9 – background tiles.</summary>
    DrawBackground,
    /// <summary>0609:1256(x, y, w, h) – frame.</summary>
    DrawFrame,
    /// <summary>0A69:00D3(x, y, w, text offset, text segment, colour, shadow, ox, oy) – centred title; the far text pointer is the one in DS:385C.</summary>
    DrawTitle,
    /// <summary>
    /// 0A69:0288(x, y, w, lines offset, count, colour, wait) – typed lines from the far pointers at DS:offset; every character
    /// follows as <see cref="TypeCharacter"/>.
    /// </summary>
    TypeLines,
    /// <summary>
    /// 0A69:01AD, one character (line, index, character) of the last <see cref="TypeLines"/>: list emptied, the character at
    /// its place (DS:0140, 0A96:0525), 21FD:060D (list, page flip, list), sound 21A7:0054(1, 16h) unless a space; the waits
    /// (flip, pause check, 21FD:0185(wait)) are the core's.
    /// </summary>
    TypeCharacter,
    /// <summary>21FD:060D – execute the list, flip the pages, execute the list again.</summary>
    ListOnBothPages,
    /// <summary>21FD:051F – flip the pages.</summary>
    FlipPages,
    /// <summary>21FD:0590 – draw on the page shown.</summary>
    DrawOnShownPage,
    /// <summary>111D:07E2(x, on) – one column of the third catastrophe drawn (on = 1) or restored from the atlas (on = 0).</summary>
    Column,
    /// <summary>21A7:0054(a, b) – sound request.</summary>
    Sound,
    /// <summary>21FD:0185(low, high) – wait for that many retraces (the core yields them).</summary>
    Delay,
    /// <summary>0A96:0030(sx, sy, w, h, dx, dy) – atlas rectangle to the draw page (command list, star show 111D:0112).</summary>
    Blit,
    /// <summary>0A96:04D7(x, y, w, h, colour) – filled rectangle (command list, star show).</summary>
    FillRect,
    /// <summary>0A96:03FB(x, y, colour) – one pixel, a star (command list, star show).</summary>
    SetPixel,
    /// <summary>0A96:06A8 – execute the command list on the draw page (star show, wait loop).</summary>
    ExecuteList,
    /// <summary>23CD:071C(DS:082E) – backgrounds of the moving objects written back (wait loop 21FD:0004).</summary>
    RestoreObjects,
    /// <summary>23CD:06E9(DS:082E) – the moving objects drawn (wait loop 21FD:0004).</summary>
    DrawObjects,
}

public sealed record DisasterEvent(DisasterEventKind Kind, params short[] Args)
{
    public override string ToString() => $"{Kind}({string.Join(", ", Args)})";
}

/// <summary>Receiver of the catastrophes' outputs and the routines of other lanes they call.</summary>
public interface IDisasterHost
{
    void Emit(DisasterEvent e);

    /// <summary>21FD:0245 – pause check (<see cref="Planting.Pause"/> is the original's).</summary>
    IEnumerable<FieldWait> PauseCheck(Dgroup d);

    /// <summary>21FD:0004(mode) – wait loop of lane bilanz (R5).</summary>
    IEnumerable<FieldWait> WaitLoop(Dgroup d, OriginalRandom random, short mode);
}
