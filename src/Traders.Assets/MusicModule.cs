namespace Traders.Assets;

/// <summary>
/// Header of the "A.H." music modules (*.CMF, not Creative CMF). Played by FXDRIVER.EXE, which
/// the game drives through int 64h (module 218B). Known so far (all words big-endian):
/// <list type="bullet">
/// <item>0x00: magic "A.H."</item>
/// <item>0x04: order list, 99 words, terminated by 99 (0x63)</item>
/// <item>0xCA: number of patterns (= highest pattern number in the order list + 1)</item>
/// <item>0xCC..: instruments and patterns – decoded in milestone M14 from the driver code</item>
/// </list>
/// </summary>
public sealed class MusicModule
{
    public const int OrderListOffset = 4;
    public const int OrderListWords = 99;
    public const int PatternCountOffset = OrderListOffset + OrderListWords * 2; // 0xCA
    public const int OrderEnd = 99;

    public IReadOnlyList<int> Orders { get; }
    public int PatternCount { get; }
    public byte[] Raw { get; }

    private MusicModule(List<int> orders, int patternCount, byte[] raw)
    {
        Orders = orders;
        PatternCount = patternCount;
        Raw = raw;
    }

    public static MusicModule Parse(ReadOnlySpan<byte> data)
    {
        if (data.Length < PatternCountOffset + 2 || data[0] != 'A' || data[1] != '.' || data[2] != 'H' || data[3] != '.')
            throw new InvalidDataException("Not an A.H. music module.");
        var orders = new List<int>();
        for (int i = 0; i < OrderListWords; i++)
        {
            int v = data[OrderListOffset + i * 2] << 8 | data[OrderListOffset + i * 2 + 1];
            if (v == OrderEnd) break;
            orders.Add(v);
        }
        int patterns = data[PatternCountOffset] << 8 | data[PatternCountOffset + 1];
        return new MusicModule(orders, patterns, data.ToArray());
    }

    public static MusicModule Load(string path) => Parse(File.ReadAllBytes(path));
}
