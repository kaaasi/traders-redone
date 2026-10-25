using Traders.Assets;

namespace Traders.Tests;

public class FontMaskMusicTests
{
    private static FontSet Fonts => FontSet.Load(OriginalFiles.Dos(ResourceTable.Fonts));

    [Fact]
    public void Font_layout_covers_file_exactly()
    {
        Assert.Equal(FontSet.ExpectedLength,
            FontSet.LargeCount * FontSet.LargeHeight * 2 + FontSet.SmallCount * FontSet.SmallHeight * 2);
        var f = Fonts;
        Assert.Equal(91, f.Large.LastChar);
        Assert.Equal(102, f.Small.LastChar);
    }

    [Fact]
    public void Space_is_empty_in_both_fonts()
    {
        var f = Fonts;
        foreach (var font in new[] { f.Large, f.Small })
            for (int y = 0; y < font.GlyphHeight; y++)
                Assert.Equal(0, font.Row(' ', y));
    }

    [Fact]
    public void Large_exclamation_mark_matches_original_bits()
    {
        // Rows of '!' as found in FONTS.PP: 18 3C 3C 3C 3C 3C 18 00 18 3C 3C 18 (left byte).
        byte[] expected = [0x18, 0x3C, 0x3C, 0x3C, 0x3C, 0x3C, 0x18, 0x00, 0x18, 0x3C, 0x3C, 0x18];
        var f = Fonts;
        for (int y = 0; y < 12; y++)
            Assert.Equal(expected[y] << 8, f.Large.Row('!', y));
    }

    [Fact]
    public void Small_hash_sign_matches_original_bits()
    {
        // '#' in the small font: .#.#. / ##### / .#.#. / ##### / .#.#.
        byte[] expected = [0x50, 0xF8, 0x50, 0xF8, 0x50];
        var f = Fonts;
        for (int y = 0; y < 5; y++)
            Assert.Equal(expected[y] << 8, f.Small.Row('#', y));
    }

    [Fact]
    public void Masks_are_a_full_screen_bitplane_with_content()
    {
        var m = MaskImage.Load(OriginalFiles.Dos(ResourceTable.Masks));
        int set = 0;
        for (int y = 0; y < MaskImage.Height; y++)
            for (int x = 0; x < MaskImage.Width; x++)
                if (m[x, y]) set++;
        Assert.InRange(set, 1000, 40000);
    }

    [Theory]
    [InlineData("TRADINTR.CMF", 30, 24)]
    [InlineData("TRADMENU.CMF", 11, 9)]
    [InlineData("TRAD_END.CMF", 28, 12)]
    public void Music_header_orders_and_pattern_count(string file, int orders, int patterns)
    {
        var m = MusicModule.Load(OriginalFiles.Dos(file));
        Assert.Equal(orders, m.Orders.Count);
        Assert.Equal(patterns, m.PatternCount);
        Assert.Equal(patterns - 1, m.Orders.Max());
    }

    [Fact]
    public void Asset_library_loads_everything()
    {
        var lib = AssetLibrary.Load(OriginalFiles.DosDir);
        Assert.Equal(10, lib.Pictures.Count);
        Assert.Equal(3, lib.Music.Count);
    }
}
