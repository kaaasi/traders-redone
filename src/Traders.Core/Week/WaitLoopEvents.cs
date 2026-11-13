namespace Traders.Core.Week;

/// <summary>What an event of the week screens tells the scene (or the sound and object layers).</summary>
public enum WeekEventKind
{
    /// <summary>21FD:051F: page flip (its retraces are the waits that follow, see <see cref="Menus.MenuWait.Flip"/>).</summary>
    Flip,

    /// <summary>0A96:06A8: the command list is executed.</summary>
    ListExecute,

    /// <summary>0A96:001B: the command list is emptied.</summary>
    ListClear,

    /// <summary>23CD:071C(DS:082E): backgrounds of the moving objects written back on the hidden page.</summary>
    RestoreObjects,

    /// <summary>23CD:06E9(DS:082E): the moving objects are drawn on the hidden page.</summary>
    DrawObjects,

    /// <summary>21A7:0054(A, B): sound call (not ported yet, R10).</summary>
    Sound,

    /// <summary>1F63:104B(A, B): indicator picture(s) A (bit 0 left at x D0h, bit 1 right at x F4h) in state B (0, 1 or 2).</summary>
    Indicator,

    /// <summary>0609:1FCC: object A (player DS:37AA) shown with picture 0 (21AF:0412(A, 0)).</summary>
    ShowPlayerObject,

    /// <summary>
    /// 0825:0686(A): the confirmation mark of player A is drawn in the row of its rank DS:302A[A] (0A96:001B, 0A96:0267); the
    /// page flip of 21FD:060D follows as <see cref="ListExecute"/>, <see cref="Flip"/>, <see cref="ListExecute"/>.
    /// </summary>
    ReportMark,

    /// <summary>186C:0002(A): picture A is loaded (week report: DS:08CE).</summary>
    LoadPicture,

    /// <summary>
    /// A drawing routine of the week report runs: A = its offset in segment 0825 (0072, 0178, 0364, 047D, 0560);
    /// <see cref="WeekEvent.Values"/> carries what the core computed for it (0178: per player land, wares, gems, other, total).
    /// </summary>
    Draw,

    /// <summary>21FD:0590: the drawing page becomes the shown page (two retraces follow as waits).</summary>
    DrawOnShownPage,

    /// <summary>
    /// 21FD:03EB(A, B): palette blend toward colour A at stage B; in the opening wipe 0825:09C9 it follows the column pair
    /// si = B·16 + B0h (one retrace follows as wait).
    /// </summary>
    PaletteStep,

    /// <summary>21FD:04DA(A): blend colour DS:10D8 = A.</summary>
    BlendColor,

    /// <summary>21AF:04CD: the restore records of all objects are invalidated.</summary>
    InvalidateObjects,

    /// <summary>0A96:0C3F(DS:082E): the drawing page is copied to the other page.</summary>
    CopyPage,

    /// <summary>21FD:026C(DS:A): base palette from DS:A.</summary>
    LoadPalette,

    /// <summary>21FD:0329: slow fade-in (32 retraces follow as waits).</summary>
    FadeIn,
}

/// <summary>An event of the week screens with the frame in which it happened.</summary>
public sealed record WeekEvent(WeekEventKind Kind, long Frame, int A = 0, int B = 0, IReadOnlyList<int>? Values = null)
{
    public override string ToString() => $"frame {Frame} {Kind} {A:X} {B:X}";
}
