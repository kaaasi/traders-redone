namespace Traders.Assets;

/// <summary>
/// MASKS.PP decrunched: a 320×200 1-bit atlas (40 bytes per row, MSB = leftmost pixel) holding
/// the transparency masks used by the masked blit (0A96:013B). Kept as raw bits; how source
/// rectangles in the colour atlases map to rectangles here is determined from the blit code.
/// </summary>
public sealed class MaskImage
{
    public const int Width = 320;
    public const int Height = 200;
    public const int Stride = Width / 8;
    public const int ExpectedLength = Stride * Height;

    public byte[] Bits { get; }

    private MaskImage(byte[] bits) => Bits = bits;

    public bool this[int x, int y] => (Bits[y * Stride + (x >> 3)] & (0x80 >> (x & 7))) != 0;

    public static MaskImage FromDecrunched(ReadOnlySpan<byte> data)
    {
        if (data.Length != ExpectedLength)
            throw new InvalidDataException($"MASKS has {data.Length} bytes, expected {ExpectedLength}.");
        return new MaskImage(data.ToArray());
    }

    public static MaskImage Load(string path) => FromDecrunched(PowerPacker.Decrunch(File.ReadAllBytes(path)));
}
