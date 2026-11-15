namespace Traders.Core.Pixels;

/// <summary>
/// Indexed picture handed to the core as explicit input (R6P-1): the pixels of a V-*.PP atlas (320 × 256, original index
/// values unchanged) as the host and the tests decode them with Traders.Assets. The core never reads files itself; a rule
/// that needs picture pixels takes a <c>Func&lt;int, IndexedImage&gt;</c> by picture number (entry n of the picture table
/// DS:09CE, the argument of 186C:0002), like <c>Func&lt;int, ImageFileLengths&gt;</c>.
/// </summary>
public sealed class IndexedImage
{
    private readonly byte[] _pixels;

    public IndexedImage(int width, int height, ReadOnlySpan<byte> pixels)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);
        if (pixels.Length != width * height)
            throw new ArgumentException($"{width} × {height} picture needs {width * height} pixels, got {pixels.Length}.",
                nameof(pixels));
        Width = width;
        Height = height;
        _pixels = pixels.ToArray();
    }

    public int Width { get; }

    public int Height { get; }

    /// <summary>Colour index at (x, y); row-major, row 0 at the top.</summary>
    public byte this[int x, int y]
    {
        get
        {
            if ((uint)x >= (uint)Width || (uint)y >= (uint)Height)
                throw new ArgumentOutOfRangeException(nameof(x), $"({x}, {y}) lies outside the {Width} × {Height} picture.");
            return _pixels[y * Width + x];
        }
    }
}
