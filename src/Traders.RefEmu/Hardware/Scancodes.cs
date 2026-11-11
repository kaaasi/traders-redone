namespace Traders.RefEmu.Hardware;

/// <summary>Set-1 make codes of the keys used in input scripts (break code = make | 80h).</summary>
public static class Scancodes
{
    public const byte Escape = 0x01, Enter = 0x1C, Space = 0x39, Backspace = 0x0E, Tab = 0x0F;
    public const byte LeftShift = 0x2A, RightShift = 0x36, Ctrl = 0x1D, Alt = 0x38;
    public const byte Up = 0x48, Down = 0x50, Left = 0x4B, Right = 0x4D;

    /// <summary>F1..F10 → 3Bh..44h.</summary>
    public static byte F(int n) => n is >= 1 and <= 10 ? (byte)(0x3A + n) : throw new ArgumentOutOfRangeException(nameof(n));

    /// <summary>Main keyboard digits: 1..9 → 02h..0Ah, 0 → 0Bh.</summary>
    public static byte Digit(int d) => d is >= 1 and <= 9 ? (byte)(0x01 + d) : d == 0 ? (byte)0x0B : throw new ArgumentOutOfRangeException(nameof(d));

    private const string Letters = "QWERTYUIOP\0\0\0\0ASDFGHJKL\0\0\0\0\0ZXCVBNM";

    /// <summary>Letter keys A..Z (US layout).</summary>
    public static byte Letter(char c)
    {
        int i = Letters.IndexOf(char.ToUpperInvariant(c));
        return i >= 0 && char.IsLetter(c) ? (byte)(0x10 + i) : throw new ArgumentOutOfRangeException(nameof(c));
    }
}
