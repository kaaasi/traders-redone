using Traders.Core.Week;

namespace Traders.Core.Events;

/// <summary>
/// What an event of the week incidents 17CB:0005 tells the scene (or the sound layer). Each kind stands for exactly one call
/// of the original (docs/routines/17CB_0005_wochenereignisse.md); the core decides, the scene draws.
/// </summary>
public enum IncidentEventKind
{
    /// <summary>21FD:037D(A): fade-out toward colour A (its retraces follow as waits).</summary>
    FadeOut,

    /// <summary>0A96:001B: the command list is emptied.</summary>
    ListClear,

    /// <summary>0609:1ED9: the field picture is drawn (backdrop of the incident window).</summary>
    FieldBackdrop,

    /// <summary>0609:1256(A, B, C, D): frame at x A, y B, width C, height D.</summary>
    Frame,

    /// <summary>0A96:06A8: the command list is executed.</summary>
    ListExecute,

    /// <summary>0A96:0C3F(A): the drawing page is copied to the other page (A = DS:082E).</summary>
    CopyPage,

    /// <summary>21FD:05DB(A): driver font A (font metrics DS:3CA8/34A4 are the output's).</summary>
    Font,

    /// <summary>21FD:026C(DS:A): base palette from DS:A.</summary>
    LoadPalette,

    /// <summary>21FD:0329: slow fade-in (its retraces follow as waits).</summary>
    FadeIn,

    /// <summary>21A7:0054(A, B): sound call (not ported yet, R10).</summary>
    Sound,

    /// <summary>21FD:0185(A): A retraces (they follow as waits).</summary>
    Pause,

    /// <summary>0609:1879(A, B, C): emblem of player D (DS:37AA at the call) at x A, y B with badge picture C.</summary>
    Emblem,

    /// <summary>21FD:051F: page flip (its retraces follow as waits).</summary>
    Flip,

    /// <summary>
    /// 2295:0001(A, B, text C + 2): amount A as text of width B into text C (index of the table DS:37CC) from its third
    /// character on.
    /// </summary>
    Number,

    /// <summary>17CB:09E0 → 0A69:0288: B lines of text from text index A on (table DS:37CC), colour C, centred at y 86h − 7·B.</summary>
    Text,

    /// <summary>
    /// 0A69:01AD inside 0A69:0288: character B (0-based) of text A in colour C, line D of the text (the character itself is in
    /// DS:0140); the scene adds it to the list at its centred position (0A96:0525).
    /// </summary>
    Character,

    /// <summary>An event of the wait loop 21FD:0004(0) (<see cref="IncidentEvent.Inner"/>).</summary>
    WaitLoop,
}

/// <summary>An event of the week incidents with the frame in which it happened.</summary>
public sealed record IncidentEvent(IncidentEventKind Kind, long Frame, int A = 0, int B = 0, int C = 0, int D = 0,
    WeekEvent? Inner = null)
{
    public override string ToString() =>
        Inner is null ? $"frame {Frame} {Kind} {A:X} {B:X} {C:X} {D:X}" : $"frame {Frame} {Kind} {Inner}";
}
