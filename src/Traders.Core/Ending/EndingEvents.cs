using Traders.Core.Field;
using Traders.Core.State;

namespace Traders.Core.Ending;

/// <summary>
/// Requests of the end screens 20B3:0053–0088 (finding docs/routines/20B3_008E_ende.md) that the field layer's
/// <see cref="FieldEventKind"/> does not cover. The arguments are the original's in call order; a far pointer into DGROUP
/// carries <see cref="DgroupLayout.Segment"/> as its segment.
/// </summary>
public enum EndingEventKind
{
    /// <summary>218B:0094(a, b) – music request of the sound control (20B3:0060: (2, 3), the end tune); a sound command for lane effekte/musik.</summary>
    Music,

    /// <summary>0A96:0267(sx, sy, tw, th, dx, dy, w, h) – command type 6, area tiled with a tile of the atlas.</summary>
    Tile,

    /// <summary>0A96:013B(sx, sy, mx, my, w, h, dx, dy) – command type 3, masked blit.</summary>
    MaskedBlit,

    /// <summary>0A96:04D7(x, y, w, h, colour) – filled rectangle.</summary>
    FillRect,

    /// <summary>0A96:0525(x, y, text offset, text segment, colour, background) – text at a far address.</summary>
    Text,

    /// <summary>0A96:057A(x, y, value low, value high, digits, colour, background) – number.</summary>
    Number,

    /// <summary>0A96:05FB(x, y, value low, value high, digits, colour, background) – number with a leading "$".</summary>
    Money,

    /// <summary>0A96:03FB(x, y, colour) – one pixel.</summary>
    Pixel,

    /// <summary>0A96:0433(x1, y1, x2, y2, colour) – line.</summary>
    Line,

    /// <summary>0A69:002C(x, y, h, colour) – vertical line.</summary>
    VerticalLine,

    /// <summary>0609:1879(x, y, rank) – emblem of the player in the fourth argument (DS:37AA at the call).</summary>
    Emblem,

    /// <summary>21FD:03EB(colour, stage) – one step of the palette blend (one retrace on card 1).</summary>
    PaletteStep,

    /// <summary>21FD:04DA(rgb12) – blend colour DS:10D8 (the output's).</summary>
    BlendColor,

    /// <summary>21FD:0590 – draw on the page shown (two retraces).</summary>
    DrawOnShownPage,

    /// <summary>0B67:01D4 – draw page = the page shown.</summary>
    DrawShownPage,

    /// <summary>0B67:0206 – draw page = the hidden page.</summary>
    DrawHiddenPage,

    /// <summary>23CD:06DE(x, y) – pixel of the draw page read (the value follows as third argument).</summary>
    ReadPixel,

    /// <summary>21AF:045E(object) – object hidden (the core clears its visible byte).</summary>
    HideObject,

    /// <summary>27C6:00D2 – wait for a key: DS:115D = 0, then until the keyboard handler counts a make.</summary>
    WaitKey,
}

/// <summary>A request of the end screens with the original's arguments in call order.</summary>
public sealed record EndingEvent(EndingEventKind Kind, params short[] Args)
{
    public override string ToString() => $"{Kind}({string.Join(", ", Args)})";
}

/// <summary>What the end screens need from outside the core: outputs, the pixel reads and the report 0825:000A(0).</summary>
public interface IEndingHost : IFieldOutput
{
    /// <summary>The segment's own requests.</summary>
    void Emit(EndingEvent e);

    /// <summary>
    /// 23CD:06DE(x, y) – colour of the pixel of the draw page: in the game the core's rule page (R6P-1, the default); a test
    /// host may answer from a script instead.
    /// </summary>
    byte ReadPixel(Pixels.RulePage page, short x, short y) => page.Get(x, y);

    /// <summary>
    /// 0825:000A(0) – the week report without its opening and wait loop (lane bilanz, <see cref="Week.Report"/>), frame by
    /// frame; a test host may record the call instead.
    /// </summary>
    IEnumerable<FieldWait> Report(Dgroup d, OriginalRandom random);
}
