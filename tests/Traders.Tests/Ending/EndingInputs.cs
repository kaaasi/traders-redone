using Traders.Assets;
using Traders.Core.Pixels;

namespace Traders.Tests.Ending;

/// <summary>The explicit inputs of the evening's rule page (<see cref="EndingPage"/>) from the original files.</summary>
internal static class EndingInputs
{
    /// <summary>Pixels of picture n (186C:0002(n)), from the V-*.PP files.</summary>
    public static readonly Lazy<IndexedImage[]> Pictures = new(() =>
        [.. ResourceTable.Pictures.OrderBy(e => (int)e.Id).Select(e =>
        {
            var image = VgaImage.Load(TestGameData.Dos(e.Vga));
            return new IndexedImage(VgaImage.Width, VgaImage.Height, image.Pixels);
        })]);

    /// <summary>The driver's three fonts (as in the scene harnesses): small (5 rows, advance 6), large (12, 14), digits (5, 5).</summary>
    public static readonly Lazy<RuleFont[]> Fonts = new(() =>
    {
        var data = PowerPacker.Decrunch(File.ReadAllBytes(TestGameData.Dos(ResourceTable.Fonts)));
        RuleFont Font(int offset, int height, int advance, int first)
        {
            int glyphs = (data.Length - offset) / (2 * height);
            var rows = new ushort[glyphs * height];
            for (int i = 0; i < rows.Length; i++) rows[i] = (ushort)(data[offset + 2 * i] << 8 | data[offset + 2 * i + 1]);
            return new RuleFont(height, advance, first, rows);
        }
        return [Font(0x5A0, 5, 6, 0x20), Font(0, 12, 14, 0x20), Font(0x7F8, 5, 5, 0x2F)];
    });
}
