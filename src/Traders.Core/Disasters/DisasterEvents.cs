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
    /// <summary>0A69:0288(x, y, w, lines offset, count, colour, wait) – typed lines from the far pointers at DS:offset.</summary>
    TypeLines,
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

    /// <summary>
    /// 111D:0112 – the star show (two loops of 90 stars, random positions, pixel reads of the draw page). Its rule part needs
    /// the rule page (src/Traders.Core/Pixels, PR of part 1a); until that is merged the host carries it out (tests: the
    /// original in the RefEmu).
    /// </summary>
    IEnumerable<FieldWait> StarShow(Dgroup d, OriginalRandom random);
}
