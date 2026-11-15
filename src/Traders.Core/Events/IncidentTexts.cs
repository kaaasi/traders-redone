using System.Globalization;
using Traders.Core.Menus;
using Traders.Core.State;

namespace Traders.Core.Events;

/// <summary>
/// Memory behind the far text pointers of the table DS:37CC (the text buffer DS:4884 of the program start). The week incidents
/// read their lines there (typewriter 0A69:0288) and write the amount into a line (2295:0001), as the original does.
/// </summary>
public interface ITextMemory
{
    byte Read(ushort segment, ushort offset);

    void Write(ushort segment, ushort offset, byte value);
}

/// <summary>
/// The text buffer of the program start (<c>ProgramStart.TextBuffer</c>, filled by 192D:18B6) behind its far address DS:4884
/// (taken from <paramref name="state"/> at every access).
/// </summary>
public sealed class TextBufferMemory(Dgroup state, Func<byte[]?> buffer) : ITextMemory
{
    private (byte[] Buffer, int Index) At(ushort segment, ushort offset)
    {
        var b = buffer() ?? throw new InvalidOperationException("Text buffer used before the program start.");
        ushort baseOffset = state.U16(MenuLayout.TextBuffer), baseSegment = state.U16(MenuLayout.TextBuffer + 2);
        int i = (segment - baseSegment) * 16 + offset - baseOffset;
        if (i < 0 || i >= b.Length) throw new InvalidOperationException($"Far address {segment:X4}:{offset:X4} outside the text buffer.");
        return (b, i);
    }

    public byte Read(ushort segment, ushort offset)
    {
        var (b, i) = At(segment, offset);
        return b[i];
    }

    public void Write(ushort segment, ushort offset, byte value)
    {
        var (b, i) = At(segment, offset);
        b[i] = value;
    }
}

/// <summary>The text routines the week incidents call, as far as they change memory or take frames.</summary>
public static class IncidentText
{
    /// <summary>DS:1126 – fill character for leading blanks and zeros of numbers (2295:0062).</summary>
    public const int NumberFill = 0x1126;

    /// <summary>
    /// [ORIGINAL] 2295:0001(number, width, far target): sprintf("%*lu", width, number) into a 22-byte stack buffer, leading
    /// blanks and zeros (at most width − 1) replaced by DS:1126, then exactly width characters copied to the target (a longer
    /// number loses its last digits; no terminator is written).
    /// </summary>
    public static void FormatNumber(Dgroup d, ITextMemory memory, uint number, short width, ushort offset, ushort segment)
    {
        if (width > 21 || width < 0)
            throw new ArgumentOutOfRangeException(nameof(width), "The incidents use width 4; the original's buffer holds 22 bytes.");
        string s = number.ToString(CultureInfo.InvariantCulture).PadLeft(width);
        var buffer = new byte[Math.Max(s.Length + 1, 22)];
        for (int i = 0; i < s.Length; i++) buffer[i] = (byte)s[i];
        for (int i = 0; width - 1 > i; i++)
        {
            if (buffer[i] is (byte)' ' or (byte)'0') buffer[i] = d.U8(NumberFill);
            else break;
        }
        for (int i = 0; i < width; i++) memory.Write(segment, Int16Math.U(offset + i), buffer[i]);
    }

    /// <summary>strlen (0000:42E5) of the zero-terminated text at the far address (16-bit offset arithmetic).</summary>
    public static ushort Length(ITextMemory memory, ushort offset, ushort segment)
    {
        ushort n = 0;
        while (memory.Read(segment, Int16Math.U(offset + n)) != 0) n++;
        return n;
    }
}
