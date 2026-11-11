using Traders.Tests.RefEmu;
using Traders.Vga.Layers;

namespace Traders.Tests.Vga;

/// <summary>
/// Differential tests of the page management 0B67 and the page flip 21FD:051F: random page state in the data segment,
/// the original routine in the RefEmu and the port; afterwards data segment, DAC, retrace waits, CRTC start, the
/// driver's draw page and VRAM equal.
/// </summary>
public class PageLayerTests
{
    private const ushort Pages = 0x0B67, Palettes = 0x21FD;

    /// <summary>Page pointers as set by 0B67:0002 on VGA, random shown page and draw mode.</summary>
    private static void SetUpPages(LayerHarness h)
    {
        h.WriteDwordBoth(GraphicsLayers.Page0, 0xA000_0000);
        h.WriteDwordBoth(GraphicsLayers.Page1, 0xA400_0000);
        h.WriteWordBoth(GraphicsLayers.ShownPage, h.Rng.Next(3) switch { 0 => 0, 1 => 1, _ => h.Next(2, 0xFFFF) });
        h.WriteWordBoth(GraphicsLayers.DrawHidden, h.Rng.Next(3) switch { 0 => 0, 1 => 1, _ => h.Next(2, 0xFFFF) });
    }

    // [DIFFERENTIAL] 0B67:0196
    // [DIFFERENTIAL] 0B67:01B5
    // [DIFFERENTIAL] 0B67:01D4
    // [DIFFERENTIAL] 0B67:0206
    // [DIFFERENTIAL] 0B67:016B
    // [DIFFERENTIAL] 0B67:028F
    // [DIFFERENTIAL] 0D73:00D3
    [GameDataFact]
    public void Show_draw_and_flip_0B67_match_original()
    {
        var h = new LayerHarness(200);
        (ushort Offset, Action<GraphicsLayers> Port)[] routines =
        [
            (0x0196, l => l.ShowPage0()),
            (0x01B5, l => l.ShowPage1()),
            (0x01D4, l => l.DrawOnShownPage()),
            (0x0206, l => l.DrawOnHiddenPage()),
            (0x016B, l => l.FlipShownPage()),
            (0x028F, l => l.WaitForRetrace()),
        ];
        for (int i = 0; i < 120; i++)
        {
            if (i % 10 == 0) SetUpPages(h);
            var (offset, port) = routines[h.Rng.Next(routines.Length)];
            h.Call(Pages, offset);
            port(h.Port);
            h.AssertScreenEqual($"0B67:{offset:X4} #{i}");
        }
    }

    // [DIFFERENTIAL] 0B67:0238
    [GameDataFact]
    public void SelectPages_0B67_0238_matches_original()
    {
        var h = new LayerHarness(210);
        uint[] values = [0, 1, 2, 3, 0x0001_0001, 0x0002_0000, 0xFFFF_FFFF];
        for (int i = 0; i < 150; i++)
        {
            if (i % 7 == 0) SetUpPages(h);
            uint draw = values[h.Rng.Next(values.Length)], show = values[h.Rng.Next(values.Length)];
            h.Call(Pages, 0x0238, (ushort)draw, (ushort)(draw >> 16), (ushort)show, (ushort)(show >> 16));
            h.Port.SelectPages(draw, show);
            h.AssertScreenEqual($"0238({draw:X8},{show:X8}) #{i}");
        }
    }

    // [DIFFERENTIAL] 0B67:0299
    [GameDataFact]
    public void CopyPage_0B67_0299_matches_original()
    {
        var h = new LayerHarness(220);
        foreach (ushort direction in new ushort[] { 1, 0, 2, 1, 0xFFFF })
        {
            SetUpPages(h);
            for (int p = 0; p < 4; p++)
            {
                h.Rng.NextBytes(h.Driver.Port.Vram.Planes[p]);
                h.Driver.Port.Vram.Planes[p].CopyTo(h.Original.Machine.Vga.Planes[p], 0);
            }
            h.Call(Pages, 0x0299, direction);
            h.Port.CopyPage(direction);
            h.AssertScreenEqual($"0299({direction:X4})");
        }
        // Card modes outside the table (0, 1, 2, 63h) copy nothing.
        var other = new LayerHarness(221, 5);
        other.Call(Pages, 0x0299, 1);
        other.Port.CopyPage(1);
        other.AssertScreenEqual("0299(1) with DS:090E = 5");
    }

    // [DIFFERENTIAL] 0B67:0002
    [GameDataTheory]
    [InlineData(0x8000_0000u, (ushort)0)]
    [InlineData(0x1234_0010u, (ushort)0)]
    [InlineData(0u, (ushort)1)]
    public void InitializePages_0B67_0002_matches_original(uint buffer, ushort expected)
    {
        var h = new LayerHarness(230);
        SetUpPages(h);
        h.Randomize(GraphicsLayers.PageBuffer, 4);
        // The buffer comes from the C runtime heap (186C:02C6), which LoadOnly does not set up: the allocator is
        // replaced by "mov ax,lo / mov dx,hi / retf" in the emulator, and the port takes the same pointer as input.
        h.Original.Machine.Mem.WriteBlock(((uint)h.Original.Segment(0x186C) << 4) + 0x02C6,
            [0xB8, (byte)buffer, (byte)(buffer >> 8), 0xBA, (byte)(buffer >> 16), (byte)(buffer >> 24), 0xCB]);
        var (ax, _) = h.Call(Pages, 0x0002);
        ushort result = h.Port.InitializePages(buffer);
        Assert.Equal(expected, ax);
        Assert.Equal(ax, result);
        h.AssertScreenEqual($"0B67:0002 (buffer {buffer:X8})");
        if (expected == 0)
            for (int p = 0; p < 4; p++)
                Assert.All(new ArraySegment<byte>(h.Driver.Port.Vram.Planes[p], 0, 0x8000), b => Assert.Equal(0, b));
    }

    // [DIFFERENTIAL] 21FD:051F
    [GameDataFact]
    public void FlipPages_21FD_051F_matches_original()
    {
        var h = new LayerHarness(240);
        uint[] numbers = [1, 2, 0, 3, 0x0001_0001];
        for (int i = 0; i < 120; i++)
        {
            if (i % 6 == 0)
            {
                SetUpPages(h);
                h.WriteWordBoth(GraphicsLayers.DrawPageIndex, h.Rng.Next(4) == 0 ? h.Next(2, 0xFFFF) : h.Next(0, 1));
                h.WriteBoth(GraphicsLayers.NoFlipWait, [(byte)(h.Rng.Next(3) == 0 ? h.Next(1, 255) : 0)]);
                h.WriteBoth(GraphicsLayers.SlowFlip, [(byte)(h.Rng.Next(3) == 0 ? h.Next(1, 255) : 0)]);
                // usually the game's pairing (1, 2); sometimes other numbers
                bool usual = h.Rng.Next(2) == 0;
                h.WriteDwordBoth(GraphicsLayers.PageNumbers, usual ? 1 : numbers[h.Rng.Next(numbers.Length)]);
                h.WriteDwordBoth(GraphicsLayers.PageNumbers + 4, usual ? 2 : numbers[h.Rng.Next(numbers.Length)]);
            }
            h.Call(Palettes, 0x051F);
            h.Port.FlipPages();
            h.AssertScreenEqual($"21FD:051F #{i}");
        }
    }

    /// <summary>With the game's page numbers (1, 2) the flip alternates the shown page and always draws on the hidden one.</summary>
    [GameDataFact]
    public void FlipPages_alternates_shown_and_draw_page()
    {
        var h = new LayerHarness(250);
        SetUpPages(h);
        h.WriteWordBoth(GraphicsLayers.DrawPageIndex, 0);
        h.WriteDwordBoth(GraphicsLayers.PageNumbers, 1);
        h.WriteDwordBoth(GraphicsLayers.PageNumbers + 4, 2);
        h.WriteBoth(GraphicsLayers.NoFlipWait, [0]);
        h.WriteBoth(GraphicsLayers.SlowFlip, [0]);
        for (int i = 0; i < 4; i++)
        {
            h.Call(Palettes, 0x051F);
            h.Port.FlipPages();
            h.AssertScreenEqual($"flip #{i}");
            bool drawPage1 = i % 2 == 0;
            Assert.Equal(drawPage1 ? 0x0000 : 0x4000, h.Driver.Port.DisplayStart);
            Assert.Equal(drawPage1 ? (ushort)0xA400 : (ushort)0xA000, h.Driver.Port.PageSegment);
        }
        Assert.Equal(8, h.PortRetraces.Count);   // 0196/01B5 and the wait of 051F
    }
}
