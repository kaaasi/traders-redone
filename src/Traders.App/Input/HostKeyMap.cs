using Microsoft.Xna.Framework.Input;

namespace Traders.App.Input;

/// <summary>
/// Maps host keys to the set-1 scancode bytes a PC/AT keyboard sends on make and break: extended keys (separate cursor
/// block, right Ctrl/Alt, keypad Enter and slash) are preceded by E0h, all others are single bytes.
/// </summary>
// [APPROX] The host reports keys by label (layout dependent), the original reads key positions. The table follows the
// US layout, so on a German keyboard the key labelled Z sends 2Ch (US Z) instead of 15h. Pause/Print Screen (multi-byte
// sequences) and the Windows keys are not mapped; the original does not read them.
public static class HostKeyMap
{
    public const byte ExtendedPrefix = 0xE0;

    private static readonly Dictionary<Keys, (byte Code, bool Extended)> Map = Build();

    private static Dictionary<Keys, (byte, bool)> Build()
    {
        var m = new Dictionary<Keys, (byte, bool)>
        {
            [Keys.Escape] = (0x01, false), [Keys.Back] = (0x0E, false), [Keys.Tab] = (0x0F, false),
            [Keys.Enter] = (0x1C, false), [Keys.LeftControl] = (0x1D, false), [Keys.LeftShift] = (0x2A, false),
            [Keys.RightShift] = (0x36, false), [Keys.LeftAlt] = (0x38, false), [Keys.Space] = (0x39, false),
            [Keys.CapsLock] = (0x3A, false), [Keys.NumLock] = (0x45, false), [Keys.Scroll] = (0x46, false),
            [Keys.F11] = (0x57, false), [Keys.F12] = (0x58, false),

            [Keys.OemMinus] = (0x0C, false), [Keys.OemPlus] = (0x0D, false), [Keys.OemOpenBrackets] = (0x1A, false),
            [Keys.OemCloseBrackets] = (0x1B, false), [Keys.OemSemicolon] = (0x27, false), [Keys.OemQuotes] = (0x28, false),
            [Keys.OemTilde] = (0x29, false), [Keys.OemPipe] = (0x2B, false), [Keys.OemComma] = (0x33, false),
            [Keys.OemPeriod] = (0x34, false), [Keys.OemQuestion] = (0x35, false), [Keys.OemBackslash] = (0x56, false),

            [Keys.NumPad7] = (0x47, false), [Keys.NumPad8] = (0x48, false), [Keys.NumPad9] = (0x49, false),
            [Keys.Subtract] = (0x4A, false), [Keys.NumPad4] = (0x4B, false), [Keys.NumPad5] = (0x4C, false),
            [Keys.NumPad6] = (0x4D, false), [Keys.Add] = (0x4E, false), [Keys.NumPad1] = (0x4F, false),
            [Keys.NumPad2] = (0x50, false), [Keys.NumPad3] = (0x51, false), [Keys.NumPad0] = (0x52, false),
            [Keys.Decimal] = (0x53, false), [Keys.Multiply] = (0x37, false),

            [Keys.RightControl] = (0x1D, true), [Keys.RightAlt] = (0x38, true), [Keys.Divide] = (0x35, true),
            [Keys.Home] = (0x47, true), [Keys.Up] = (0x48, true), [Keys.PageUp] = (0x49, true),
            [Keys.Left] = (0x4B, true), [Keys.Right] = (0x4D, true), [Keys.End] = (0x4F, true),
            [Keys.Down] = (0x50, true), [Keys.PageDown] = (0x51, true), [Keys.Insert] = (0x52, true),
            [Keys.Delete] = (0x53, true),
        };
        const string row1 = "1234567890", rowQ = "QWERTYUIOP", rowA = "ASDFGHJKL", rowZ = "ZXCVBNM";
        for (int i = 0; i < row1.Length; i++) m[Keys.D0 + (row1[i] - '0')] = ((byte)(0x02 + i), false);
        for (int i = 0; i < rowQ.Length; i++) m[Keys.A + (rowQ[i] - 'A')] = ((byte)(0x10 + i), false);
        for (int i = 0; i < rowA.Length; i++) m[Keys.A + (rowA[i] - 'A')] = ((byte)(0x1E + i), false);
        for (int i = 0; i < rowZ.Length; i++) m[Keys.A + (rowZ[i] - 'A')] = ((byte)(0x2C + i), false);
        for (int i = 0; i < 10; i++) m[Keys.F1 + i] = ((byte)(0x3B + i), false);
        return m;
    }

    /// <summary>All mapped host keys.</summary>
    public static IEnumerable<Keys> MappedKeys => Map.Keys;

    /// <summary>Scancode (make code, bit 7 clear) and whether the key is extended (E0h prefix).</summary>
    public static bool TryGetScancode(Keys key, out byte makeCode, out bool extended)
    {
        if (Map.TryGetValue(key, out var e))
        {
            (makeCode, extended) = e;
            return true;
        }
        makeCode = 0;
        extended = false;
        return false;
    }

    /// <summary>Bytes the keyboard sends when <paramref name="key"/> is pressed or released; empty for unmapped keys.</summary>
    public static byte[] Bytes(Keys key, bool make)
    {
        if (!TryGetScancode(key, out byte code, out bool extended)) return [];
        byte b = make ? code : (byte)(code | 0x80);
        return extended ? [ExtendedPrefix, b] : [b];
    }
}
