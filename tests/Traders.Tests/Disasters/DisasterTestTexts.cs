using Traders.Core.Disasters;
using Traders.Core.Menus;
using Traders.RefEmu;

namespace Traders.Tests.Disasters;

/// <summary>The text buffer of the original (far pointer DS:4884) as the catastrophes' text input.</summary>
internal static class DisasterTestTexts
{
    public static DisasterTexts From(OriginalProgram p)
    {
        ushort offset = p.ReadWord(MenuLayout.TextBuffer), segment = p.ReadWord(MenuLayout.TextBuffer + 2);
        if (offset == 0 && segment == 0) return DisasterTexts.None;
        var bytes = new byte[MenuTexts.ScanLimit + 0x100];
        int linear = segment * 16 + offset;
        for (int i = 0; i < bytes.Length; i++)
            bytes[i] = p.Machine.Mem.Read8((ushort)((linear + i) >> 4), (ushort)((linear + i) & 0xF));
        return new DisasterTexts(bytes);
    }
}
