using Traders.Core.State;

namespace Traders.Core.Menus;

/// <summary>
/// Text table of the program (192D:18B6): after the text file has been loaded to the far buffer DS:4884 (186C:0B0A, file name
/// DS:0C76), every line gets a far pointer in DS:37CC + 4·k and the buffer is turned into zero-terminated strings.
/// </summary>
public static class MenuTexts
{
    /// <summary>Bytes scanned at most (1194h).</summary>
    public const int ScanLimit = 0x1194;

    /// <summary>
    /// [ORIGINAL] 192D:18B6 after the load: entry 0 = null, entry 1 = start of the buffer; each LF becomes 0 and starts the next
    /// entry, CR becomes 0, TAB becomes a space. The scan stops after <see cref="ScanLimit"/> bytes or when entry 12Fh would be
    /// passed (entry 130h is still written). Pointers are built from DS:4884 with 16-bit offset arithmetic.
    /// </summary>
    /// <param name="state">Data segment; DS:4884 holds the buffer address, the pointer table is written.</param>
    /// <param name="buffer">Contents of the far buffer (the loaded file, then whatever follows); changed in place.</param>
    public static void Build(Dgroup state, Span<byte> buffer)
    {
        ushort baseOffset = state.U16(MenuLayout.TextBuffer), baseSegment = state.U16(MenuLayout.TextBuffer + 2);
        state.Set(MenuLayout.TextPointers, (ushort)0);
        state.Set(MenuLayout.TextPointers + 2, (ushort)0);
        state.Set(MenuLayout.TextPointers + 4, baseOffset);
        state.Set(MenuLayout.TextPointers + 6, baseSegment);
        int entry = 1;
        for (int i = 0; i < ScanLimit && entry < MenuLayout.TextPointerCount; i++)
        {
            if (i >= buffer.Length) throw new ArgumentException("The scan ran past the given buffer.", nameof(buffer));
            if (buffer[i] == 0x0A)
            {
                buffer[i] = 0;
                entry++;
                int at = MenuLayout.TextPointers + 4 * entry;
                state.Set(at, Int16Math.U(baseOffset + i + 1));
                state.Set(at + 2, baseSegment);
            }
            if (buffer[i] == 0x0D) buffer[i] = 0;
            if (buffer[i] == 0x09) buffer[i] = 0x20;
        }
    }

    /// <summary>Offset of entry <paramref name="entry"/> relative to the buffer start (for comparisons; null entry → null).</summary>
    public static int? RelativeOffset(Dgroup state, int entry)
    {
        int at = MenuLayout.TextPointers + 4 * entry;
        if (state.U16(at) == 0 && state.U16(at + 2) == 0) return null;
        return Int16Math.U(state.U16(at) - state.U16(MenuLayout.TextBuffer));
    }
}
