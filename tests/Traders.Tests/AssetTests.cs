using System.Security.Cryptography;
using Traders.Assets;

namespace Traders.Tests;

public class AssetTests
{
    [Fact]
    public void Executable_is_the_analysed_version()
    {
        var hash = Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(OriginalFiles.Dos(ResourceTable.Executable))));
        Assert.Equal(ResourceTable.ExecutableSha256, hash);
    }

    // Reference hashes produced by tools/pp20.py (independent Python implementation).
    [Theory]
    [InlineData("V-ABEND.PP", 82692, "20dd898d7f71b5d054d266ae8ed494368e1d880b02e8fab23ec1c1e54006f560")]
    [InlineData("V-AHANDE.PP", 82692, "f50fed22651d614341e87b9e2cd9ff708a2ec3a23b7fd85d48537093b4dc9fce")]
    [InlineData("V-ANBAU.PP", 82692, "3a35e18828cbb6e50e541edad1d84b958cfd7aa8e4ee7dc0345c5b949274b4e0")]
    [InlineData("V-BERGE.PP", 82692, "ff317a990992138a641ab4200abf1210ac94524f4bab81bd3e9a6c79c5312932")]
    [InlineData("V-BHANDE.PP", 82692, "91f5827c9c9756b5b9693e22e39bc35a502f0948417a4bca7d0150521a48483f")]
    [InlineData("V-ENDE.PP", 82692, "82b509239c2c62c2b05dbbff2671fc34e55dce7d9b93192fc1ad89769fb18c5c")]
    [InlineData("V-FATMIK.PP", 82692, "e5a4914b940bd4a046a2cd99dc05117349b987d15f1fe218cbb8b178f36c72e4")]
    [InlineData("V-KAMPF.PP", 82692, "2b3eb3aef10c0c976e3e5da169c9406edbcc11fafb3f0ef9945219b3b01be0e8")]
    [InlineData("V-MENU.PP", 82692, "19a072f12d1c1506f8a031f637c05378cd052d034d17ac2aedbb72ce2c6bccfa")]
    [InlineData("V-RAKETE.PP", 82692, "15b753dde823a8ec8ed949c7cb2dd587b5220870b1bd459b0149fbb05bd183ca")]
    [InlineData("E-ANBAU.PP", 40964, "bff151d3b1ef33da573c371975f5570ac3aec36c809eaf8eb1812c16a8b88610")]
    [InlineData("E-RAKETE.PP", 40964, "c6b6759c9f0f52963cff7b6341fc7cd5edc398d9020fffbcc9cffc3a676f4c7b")]
    [InlineData("FONTS.PP", 2150, "08b98cf0d0669dd5788171d1edee4e8f48abc24208da9de10e2065296ec7b3b4")]
    [InlineData("MASKS.PP", 8000, "d1f9d2d657efd842f6286c8a600aca308499315c4f196d11f1287cbe47eef79f")]
    public void PowerPacker_decrunches_like_reference(string file, int length, string sha256)
    {
        var data = PowerPacker.Decrunch(File.ReadAllBytes(OriginalFiles.Dos(file)));
        Assert.Equal(length, data.Length);
        Assert.Equal(sha256, Convert.ToHexStringLower(SHA256.HashData(data)));
    }

    [Fact]
    public void All_vga_pictures_load_with_full_atlas()
    {
        foreach (var (_, vga, _) in ResourceTable.Pictures)
        {
            var img = VgaImage.Load(OriginalFiles.Dos(vga));
            Assert.Equal(VgaImage.Width * VgaImage.Height, img.Pixels.Length);
            Assert.Equal(4, img.Trailer.Length);
        }
    }

    [Fact]
    public void Rocket_atlas_has_sprites_below_visible_area()
    {
        // Lesson from the previous attempt: V-RAKETE (not V-BERGE) carries the flight sprites
        // in rows 200..255. Make sure that region is not empty.
        var img = VgaImage.Load(OriginalFiles.Dos("V-RAKETE.PP"));
        var distinct = new HashSet<byte>();
        for (int y = VgaImage.VisibleHeight; y < VgaImage.Height; y++)
            for (int x = 0; x < VgaImage.Width; x++)
                distinct.Add(img[x, y]);
        Assert.True(distinct.Count > 8, $"only {distinct.Count} colours below row 200");
    }

    [Fact]
    public void English_text_table_starts_with_known_lines()
    {
        var t = TextTable.Load(OriginalFiles.Dos(ResourceTable.EnglishText));
        Assert.Equal(303, t.Count);
        Assert.Equal("SILOS", t[0]);
        Assert.Equal("WALLS", t[1]);
        Assert.Equal("MINES", t[2]);
        Assert.Equal("WEEKLY BALANCE", t[9]);
    }

    [Fact]
    public void Png_writer_produces_valid_signature()
    {
        var img = VgaImage.Load(OriginalFiles.Dos("V-MENU.PP"));
        using var ms = new MemoryStream();
        PngWriter.WriteIndexed(ms, VgaImage.Width, VgaImage.Height, img.Pixels, img.Palette);
        var bytes = ms.ToArray();
        Assert.Equal(new byte[] { 0x89, 0x50, 0x4E, 0x47 }, bytes[..4]);
    }
}
