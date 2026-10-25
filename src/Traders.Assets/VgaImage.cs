namespace Traders.Assets;

/// <summary>
/// Decrunched V-*.PP resource: 256-colour VGA palette (6 bits per channel), a 4-byte size
/// header (height, width as little-endian words: 256, 320) and a 320x256 indexed atlas
/// starting at byte 772 (layout confirmed by the loader 186C:0002). Rows 200..255 lie outside the visible screen and hold
/// sprites and graphic building blocks. The original index values are kept unchanged
/// because game rules may evaluate pixels (e.g. rocket flight collision).
/// </summary>
public sealed class VgaImage
{
    public const int Width = 320;
    public const int Height = 256;
    public const int VisibleHeight = 200;
    public const int PaletteSize = 768;
    public const int HeaderSize = 4;
    public const int PixelOffset = PaletteSize + HeaderSize;
    public const int ExpectedLength = PixelOffset + Width * Height;

    public Palette Palette { get; }
    public byte[] Pixels { get; }

    private VgaImage(Palette palette, byte[] pixels)
    {
        Palette = palette;
        Pixels = pixels;
    }

    public byte this[int x, int y] => Pixels[y * Width + x];

    public static VgaImage FromDecrunched(ReadOnlySpan<byte> data)
    {
        if (data.Length != ExpectedLength)
            throw new InvalidDataException($"VGA resource has {data.Length} bytes, expected {ExpectedLength}.");
        var palette = Palette.FromVga6(data[..PaletteSize]);
        int height = data[PaletteSize] | data[PaletteSize + 1] << 8;
        int width = data[PaletteSize + 2] | data[PaletteSize + 3] << 8;
        if (width != Width || height != Height)
            throw new InvalidDataException($"VGA resource header says {width}x{height}, expected {Width}x{Height}.");
        var pixels = data.Slice(PixelOffset, Width * Height).ToArray();
        return new VgaImage(palette, pixels);
    }

    public static VgaImage Load(string path) =>
        FromDecrunched(PowerPacker.Decrunch(File.ReadAllBytes(path)));
}

/// <summary>VGA DAC palette with 6-bit components as stored by the original.</summary>
public sealed class Palette
{
    public byte[] Vga6 { get; }

    private Palette(byte[] vga6) => Vga6 = vga6;

    public static Palette FromVga6(ReadOnlySpan<byte> data)
    {
        if (data.Length != 768) throw new ArgumentException("Palette must be 768 bytes.", nameof(data));
        foreach (var b in data)
            if (b > 63) throw new InvalidDataException("Palette component exceeds 6 bits.");
        return new Palette(data.ToArray());
    }

    /// <summary>Preview palette (0 = black, 1 = white, 2 = dark grey, rest black) for 1-bit data such as masks and fonts.</summary>
    public static Palette Monochrome()
    {
        var p = new byte[768];
        p[3] = p[4] = p[5] = 63;
        p[6] = p[7] = p[8] = 12; // 2 = dark grey (grid in preview sheets)
        return new Palette(p);
    }

    /// <summary>Converts a 6-bit component to 8 bits the way the classic VGA DAC is usually shown (v*255/63).</summary>
    public static byte To8Bit(byte v6) => (byte)((v6 * 255 + 31) / 63);

    public (byte R, byte G, byte B) Rgb(int index) =>
        (To8Bit(Vga6[index * 3]), To8Bit(Vga6[index * 3 + 1]), To8Bit(Vga6[index * 3 + 2]));
}
