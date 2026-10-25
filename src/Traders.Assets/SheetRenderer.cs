namespace Traders.Assets;

/// <summary>Builds indexed preview sheets (0 = background, 1 = set pixel, 2 = grid) for 1-bit assets.</summary>
public static class SheetRenderer
{
    public readonly record struct Sheet(int Width, int Height, byte[] Pixels);

    public static Sheet Font(BitmapFont font, int columns = 16, int cellWidth = 18)
    {
        int cellHeight = font.GlyphHeight + 2;
        int rows = (font.GlyphCount + columns - 1) / columns;
        int w = columns * cellWidth, h = rows * cellHeight;
        var px = new byte[w * h];
        for (int g = 0; g < font.GlyphCount; g++)
        {
            int ox = (g % columns) * cellWidth, oy = (g / columns) * cellHeight;
            for (int y = 0; y < cellHeight; y++)
                for (int x = 0; x < cellWidth; x++)
                {
                    bool inGlyph = x < BitmapFont.GlyphWidth && y < font.GlyphHeight;
                    px[(oy + y) * w + ox + x] = inGlyph
                        ? (byte)(font.IsSet(BitmapFont.FirstChar + g, x, y) ? 1 : 0)
                        : (byte)2;
                }
        }
        return new Sheet(w, h, px);
    }

    public static Sheet Mask(MaskImage mask)
    {
        var px = new byte[MaskImage.Width * MaskImage.Height];
        for (int y = 0; y < MaskImage.Height; y++)
            for (int x = 0; x < MaskImage.Width; x++)
                px[y * MaskImage.Width + x] = (byte)(mask[x, y] ? 1 : 0);
        return new Sheet(MaskImage.Width, MaskImage.Height, px);
    }

    /// <summary>Glyph index of a sheet position, or -1 (for viewers).</summary>
    public static int GlyphAt(BitmapFont font, int x, int y, int columns = 16, int cellWidth = 18)
    {
        int cellHeight = font.GlyphHeight + 2;
        int col = x / cellWidth, row = y / cellHeight;
        if (col >= columns) return -1;
        int g = row * columns + col;
        return g < font.GlyphCount ? g : -1;
    }
}
