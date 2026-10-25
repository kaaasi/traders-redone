namespace Traders.Assets;

/// <summary>
/// Decrunched V-*.PP resource: 256-colour VGA palette (6 bits per channel) followed by
/// a 320x256 indexed atlas. Rows 200..255 lie outside the visible screen and hold
/// sprites and graphic building blocks. The original index values are kept unchanged
/// because game rules may evaluate pixels (e.g. rocket flight collision).
/// </summary>
public sealed class VgaImage
{
    public const int Width = 320;
    public const int Height = 256;
    public const int VisibleHeight = 200;
    public const int PaletteSize = 768;
    public const int ExpectedLength = PaletteSize + Width * Height + 4;

    public Palette Palette { get; }
    public byte[] Pixels { get; }
    /// <summary>The four trailing bytes after the pixel data (meaning not yet known).</summary>
    public byte[] Trailer { get; }

    private VgaImage(Palette palette, byte[] pixels, byte[] trailer)
    {
        Palette = palette;
        Pixels = pixels;
        Trailer = trailer;
    }

    public byte this[int x, int y] => Pixels[y * Width + x];

    public static VgaImage FromDecrunched(ReadOnlySpan<byte> data)
    {
        if (data.Length != ExpectedLength)
            throw new InvalidDataException($"VGA resource has {data.Length} bytes, expected {ExpectedLength}.");
        var palette = Palette.FromVga6(data[..PaletteSize]);
        var pixels = data.Slice(PaletteSize, Width * Height).ToArray();
        var trailer = data[(PaletteSize + Width * Height)..].ToArray();
        return new VgaImage(palette, pixels, trailer);
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

    /// <summary>Converts a 6-bit component to 8 bits the way the classic VGA DAC is usually shown (v*255/63).</summary>
    public static byte To8Bit(byte v6) => (byte)((v6 * 255 + 31) / 63);

    public (byte R, byte G, byte B) Rgb(int index) =>
        (To8Bit(Vga6[index * 3]), To8Bit(Vga6[index * 3 + 1]), To8Bit(Vga6[index * 3 + 2]));
}
