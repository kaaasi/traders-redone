using Traders.Core.Menus;
using Traders.Core.State;

namespace Traders.Core.Disasters;

/// <summary>
/// The text buffer of the program (DS:4884, built by 192D:18B6, <see cref="MenuTexts"/>) as an explicit input of the
/// catastrophes: the typed lines 0A69:0288 wait per character, so the core needs the lengths of the zero-terminated texts
/// behind the far pointers DS:3834 … 385C (entries of the text table DS:37CC). The host passes the buffer of its program start
/// (<see cref="Startup.ProgramStart.TextBuffer"/>), tests a copy of the original's memory at DS:4884.
/// </summary>
public sealed class DisasterTexts(ReadOnlyMemory<byte> buffer)
{
    /// <summary>No texts: every far pointer must be null (tests of single routines without a program start).</summary>
    public static DisasterTexts None { get; } = new(ReadOnlyMemory<byte>.Empty);

    /// <summary>
    /// 0:42E5 (strlen) on the far text at <paramref name="segment"/>:<paramref name="offset"/>, relative to the buffer start
    /// DS:4884 (16-byte paragraphs). A pointer outside the buffer is an error (the original reads whatever memory holds).
    /// </summary>
    public int Length(Dgroup d, ushort offset, ushort segment)
    {
        int start = (segment - d.U16(MenuLayout.TextBuffer + 2)) * 16 + offset - d.U16(MenuLayout.TextBuffer);
        var span = buffer.Span;
        if (start < 0 || start >= span.Length)
            throw new InvalidOperationException($"text {segment:X4}:{offset:X4} lies outside the text buffer");
        int end = span[start..].IndexOf((byte)0);
        if (end < 0) throw new InvalidOperationException($"text {segment:X4}:{offset:X4} is not terminated in the buffer");
        return end;
    }

    /// <summary>The character <paramref name="index"/> of the far text (for the scene's events).</summary>
    public byte At(Dgroup d, ushort offset, ushort segment, int index)
    {
        int start = (segment - d.U16(MenuLayout.TextBuffer + 2)) * 16 + offset - d.U16(MenuLayout.TextBuffer);
        return buffer.Span[start + index];
    }
}
