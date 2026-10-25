using Traders.Assets;
using Traders.RefEmu;
using Traders.Tests.RefEmu;
using Traders.Vga;

namespace Traders.Tests.Vga;

/// <summary>
/// Runs the original 23CD VGA driver in the reference emulator next to <see cref="ModeXDriver"/>.
/// Both sides get identical VRAM, atlas, mask and font contents; every operation is applied to
/// both and the complete VRAM (and atlas) can be compared afterwards.
/// </summary>
internal sealed class DriverHarness
{
    // Emulator placement (all free memory above the loaded image, which ends at 3E6E:0000).
    public const ushort AtlasSeg = 0x5000, MaskSeg = 0x7000, FontTableSeg = 0x7200, GlyphSeg = 0x7300, ScratchSeg = 0x7400;
    private const ushort Driver = 0x23CD;

    public OriginalProgram Original { get; }
    public ModeXDriver Port { get; } = new();
    public Random Rng { get; }

    public DriverHarness(int seed, bool realFonts = false)
    {
        Rng = new Random(seed);
        Original = OriginalProgram.LoadOnly(Path.Combine(RefEmuPaths.RepoRoot, "original", "dos"));
        Original.Call(Driver, 0x04F6, 1); // select VGA driver: mode 13h -> Mode X, vectors to CS:0004

        // atlas, masks: random contents
        var atlas = new byte[PlanarAtlas.Length];
        Rng.NextBytes(atlas);
        Port.Atlas = new PlanarAtlas((byte[])atlas.Clone());
        Original.Machine.Mem.WriteBlock((uint)AtlasSeg << 4, atlas);
        var masks = new byte[8000];
        Rng.NextBytes(masks);
        Port.Masks = (byte[])masks.Clone();
        Original.Machine.Mem.WriteBlock((uint)MaskSeg << 4, masks);

        // fonts: FONTS.PP as the game loads it (large 16x12 from 0, small 16x5 from 1440)
        var fontData = PowerPacker.Decrunch(File.ReadAllBytes(RefEmuPaths.Dos("FONTS.PP")));
        Original.Machine.Mem.WriteBlock((uint)GlyphSeg << 4, fontData);
        var fonts = new[]
        {
            new FontDescriptor(fontData, 0, 12, 13, 32),
            new FontDescriptor(fontData, 1440, 5, 6, 32),
        };
        Port.Fonts = fonts;
        var table = new byte[fonts.Length * 10];
        for (int i = 0; i < fonts.Length; i++)
        {
            var f = fonts[i];
            Put(table, i * 10 + 0, (ushort)f.Offset);
            Put(table, i * 10 + 2, GlyphSeg);
            Put(table, i * 10 + 4, f.Height);
            Put(table, i * 10 + 6, f.Advance);
            Put(table, i * 10 + 8, f.FirstChar);
        }
        Original.Machine.Mem.WriteBlock((uint)FontTableSeg << 4, table);
        Original.Call(Driver, 0x058A, 0, FontTableSeg, 0, AtlasSeg, 0, 0, 0, MaskSeg);

        // VRAM: random contents on both sides
        for (int p = 0; p < 4; p++)
        {
            Rng.NextBytes(Port.Vram.Planes[p]);
            Port.Vram.Planes[p].CopyTo(Original.Machine.Vga.Planes[p], 0);
        }
        SetPage(0xA000);
        SetColor(0x0F);
    }

    private static void Put(byte[] b, int at, ushort v)
    {
        b[at] = (byte)v;
        b[at + 1] = (byte)(v >> 8);
    }

    public (ushort Ax, ushort Dx) CallOriginal(ushort offset, params ushort[] args) => Original.Call(Driver, offset, args);

    public void SetPage(ushort segment)
    {
        CallOriginal(0x0570, 0, segment);           // jmp far [cs:44] -> 311A
        Port.SetDrawPage(0, segment);
    }

    public void SetColor(ushort color)
    {
        CallOriginal(0x05C3, color);
        Port.Color = color;
    }

    public void SetFont(ushort font)
    {
        CallOriginal(0x05B6, font);
        Port.Font = font;
    }

    public void AssertVramEqual(string what)
    {
        for (int p = 0; p < 4; p++)
        {
            var a = Original.Machine.Vga.Planes[p];
            var b = Port.Vram.Planes[p];
            int i = FirstDifference(a, b);
            if (i >= 0)
                Assert.Fail($"{what}: plane {p} offset {i:X4} original {a[i]:X2} port {b[i]:X2}");
        }
    }

    public void AssertAtlasEqual(string what)
    {
        var a = Original.Machine.Mem.ReadBlock(AtlasSeg, 0, 0x10000)
            .Concat(Original.Machine.Mem.ReadBlock(AtlasSeg + 0x1000, 0, PlanarAtlas.Length - 0x10000)).ToArray();
        int i = FirstDifference(a, Port.Atlas.Data);
        if (i >= 0)
            Assert.Fail($"{what}: atlas byte {i:X5} original {a[i]:X2} port {Port.Atlas.Data[i]:X2}");
    }

    private static int FirstDifference(byte[] a, byte[] b)
    {
        int n = Math.Min(a.Length, b.Length);
        for (int i = 0; i < n; i++)
            if (a[i] != b[i]) return i;
        return a.Length == b.Length ? -1 : n;
    }

    public ushort Next(int minInclusive, int maxInclusive) => (ushort)Rng.Next(minInclusive, maxInclusive + 1);

    /// <summary>Second page at A400h (offset 4000h) or first page.</summary>
    public void RandomPage() => SetPage(Rng.Next(2) == 0 ? (ushort)0xA000 : (ushort)0xA400);
}
