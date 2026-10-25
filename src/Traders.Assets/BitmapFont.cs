namespace Traders.Assets;

/// <summary>
/// One of the two 1-bit fonts in FONTS.PP. Each glyph row is 16 bits (2 bytes, MSB = leftmost
/// pixel); glyphs are stored consecutively starting at ASCII 32. Glyph widths and spacing used
/// by the original text routines are not part of the file and are added once the drawing code
/// has been analysed.
/// </summary>
public sealed class BitmapFont
{
    public const int GlyphWidth = 16;
    public const int FirstChar = 32;

    private readonly byte[] _data;

    public int GlyphHeight { get; }
    public int GlyphCount { get; }
    public int LastChar => FirstChar + GlyphCount - 1;

    internal BitmapFont(byte[] data, int glyphHeight, int glyphCount)
    {
        _data = data;
        GlyphHeight = glyphHeight;
        GlyphCount = glyphCount;
    }

    public int BytesPerGlyph => GlyphHeight * 2;

    public bool HasGlyph(int code) => code >= FirstChar && code <= LastChar;

    /// <summary>Returns whether pixel (x,y) of the glyph for <paramref name="code"/> is set.</summary>
    public bool IsSet(int code, int x, int y)
    {
        if (!HasGlyph(code) || (uint)x >= GlyphWidth || (uint)y >= GlyphHeight) return false;
        int offset = (code - FirstChar) * BytesPerGlyph + y * 2 + (x >> 3);
        return (_data[offset] & (0x80 >> (x & 7))) != 0;
    }

    /// <summary>Raw 16-bit row (big-endian bit order) as stored in the file.</summary>
    public ushort Row(int code, int y)
    {
        int offset = (code - FirstChar) * BytesPerGlyph + y * 2;
        return (ushort)(_data[offset] << 8 | _data[offset + 1]);
    }
}

/// <summary>
/// FONTS.PP decrunched (2150 bytes): large font 60 glyphs × 16×12 (ASCII 32..91, 1440 bytes),
/// followed by small font 71 glyphs × 16×5 (ASCII 32..102, 710 bytes). In the small font the
/// codes 93..102 hold a second set of digits 0..9.
/// </summary>
public sealed class FontSet
{
    public const int ExpectedLength = 2150;
    public const int LargeHeight = 12, LargeCount = 60;
    public const int SmallHeight = 5, SmallCount = 71;

    public BitmapFont Large { get; }
    public BitmapFont Small { get; }

    private FontSet(BitmapFont large, BitmapFont small)
    {
        Large = large;
        Small = small;
    }

    public static FontSet FromDecrunched(ReadOnlySpan<byte> data)
    {
        if (data.Length != ExpectedLength)
            throw new InvalidDataException($"FONTS has {data.Length} bytes, expected {ExpectedLength}.");
        int largeBytes = LargeCount * LargeHeight * 2;
        var large = new BitmapFont(data[..largeBytes].ToArray(), LargeHeight, LargeCount);
        var small = new BitmapFont(data[largeBytes..].ToArray(), SmallHeight, SmallCount);
        return new FontSet(large, small);
    }

    public static FontSet Load(string path) => FromDecrunched(PowerPacker.Decrunch(File.ReadAllBytes(path)));
}
