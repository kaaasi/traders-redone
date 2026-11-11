using Traders.Vga;

namespace Traders.Tests.Vga;

/// <summary>
/// Edge cases of the 23CD VGA driver against the original in the reference emulator: rectangles across the right and
/// bottom border (the driver does not clip), all small widths and plane alignments, a zero row count, lines with
/// extreme coordinates, page flipping with the displayed picture, and card selection.
/// </summary>
public class DriverEdgeCaseTests
{
    // [DIFFERENTIAL] 23CD:31A6
    // [DIFFERENTIAL] 23CD:32D8
    // [DIFFERENTIAL] 23CD:351E
    // [DIFFERENTIAL] 23CD:35DF
    [GameDataFact]
    public void Rectangles_across_the_border_wrap_like_the_original()
    {
        var h = new DriverHarness(31);
        for (int i = 0; i < 40; i++)
        {
            h.RandomPage();
            h.SetColor(h.Next(0, 255));
            ushort x = h.Next(290, 319), y = h.Next(180, 199), w = h.Next(4, 90), hh = h.Next(1, 50);
            h.CallOriginal(0x069B, x, y, w, hh);
            h.Port.FillRect(x, y, w, hh);
            ushort sx = h.Next(0, 230), sy = h.Next(0, 200);
            h.CallOriginal(0x06A0, sx, sy, w, hh, x, y);
            h.Port.BlitAtlasToPage(sx, sy, w, hh, x, y);
            ushort dx = (ushort)(319 - x), dy = h.Next(150, 199);
            h.CallOriginal(0x06B4, x, y, w, hh, dx, dy);
            h.Port.CopyPageToPage(x, y, w, hh, dx, dy);
            ushort mx = h.Next(0, 200), my = h.Next(0, 150);
            h.CallOriginal(0x06A5, sx, sy, w, hh, x, y, mx, my);
            h.Port.MaskedBlit(sx, sy, w, hh, x, y, mx, my);
            h.AssertVramEqual($"border x={x} y={y} w={w} h={hh} #{i}");
        }
    }

    // [DIFFERENTIAL] 23CD:31A6
    // [DIFFERENTIAL] 23CD:32D8
    // [DIFFERENTIAL] 23CD:3449
    // [DIFFERENTIAL] 23CD:351E
    [GameDataFact]
    public void Small_widths_with_every_plane_alignment_match_original()
    {
        var h = new DriverHarness(32);
        for (ushort w = 1; w <= 9; w++)
            for (ushort sa = 0; sa < 4; sa++)
                for (ushort da = 0; da < 4; da++)
                {
                    h.RandomPage();
                    h.SetColor(h.Next(0, 255));
                    ushort sx = (ushort)(h.Next(0, 70) * 4 + sa), sy = h.Next(0, 200);
                    ushort dx = (ushort)(h.Next(0, 70) * 4 + da), dy = h.Next(0, 190), hh = h.Next(1, 6);
                    string what = $"w={w} sx&3={sa} dx&3={da}";
                    h.CallOriginal(0x069B, dx, dy, w, hh);
                    h.Port.FillRect(dx, dy, w, hh);
                    h.AssertVramEqual("FillRect " + what);
                    h.CallOriginal(0x06A0, sx, sy, w, hh, dx, dy);
                    h.Port.BlitAtlasToPage(sx, sy, w, hh, dx, dy);
                    h.AssertVramEqual("Blit " + what);
                    h.CallOriginal(0x06B4, sx, (ushort)(sy % 190), w, hh, dx, dy);
                    h.Port.CopyPageToPage(sx, (ushort)(sy % 190), w, hh, dx, dy);
                    h.AssertVramEqual("PageCopy " + what);
                    h.CallOriginal(0x06AF, sx, sy, w, hh, dx, (ushort)(dy + 50));
                    h.Port.CopyAtlasToAtlas(sx, sy, w, hh, dx, (ushort)(dy + 50));
                    h.AssertAtlasEqual("AtlasCopy " + what);
                }
    }

    // [DIFFERENTIAL] 23CD:31A6
    [GameDataFact]
    public void Zero_rows_mean_65536_rows_like_the_original()
    {
        var h = new DriverHarness(33);
        h.SetPage(0xA000);   // the 16-bit offset wraps inside the A000 window only for the first page
        foreach (var (x, w) in new (ushort, ushort)[] { (0, 4), (13, 3), (301, 7) })
        {
            h.SetColor(h.Next(0, 255));
            h.CallOriginal(0x069B, x, 7, w, 0);
            h.Port.FillRect(x, 7, w, 0);
            h.AssertVramEqual($"FillRect({x},7,{w},0)");
        }
    }

    // [DIFFERENTIAL] 23CD:378B
    // [DIFFERENTIAL] 23CD:38F2
    [GameDataFact]
    public void Lines_with_extreme_coordinates_match_original()
    {
        var h = new DriverHarness(34);
        h.SetPage(0xA000);
        var lines = new (ushort, ushort, ushort, ushort)[]
        {
            (0, 0, 319, 199), (319, 199, 0, 0), (0, 199, 319, 0), (319, 0, 0, 199), (5, 5, 5, 5),
            (0, 0, 0, 199), (0, 199, 0, 0), (319, 7, 0, 7), (0, 0, 1, 199), (100, 50, 400, 51),
            (10, 20, 0x8100, 30), (0x8100, 30, 10, 20), (30, 10, 31, 0x80F0), (200, 0xFFF0, 210, 5),
        };
        int i = 0;
        foreach (var (x1, y1, x2, y2) in lines)
        {
            h.SetColor(h.Next(0, 255));
            h.SetLinePattern(i % 2 == 0 ? (ushort)0xFFFF : h.Next(0, 0xFFFF));
            h.CallOriginal(0x06BE, x1, y1, x2, y2);
            h.Port.DrawLine(x1, y1, x2, y2);
            h.AssertVramEqual($"Line({x1},{y1})-({x2},{y2}) pattern {h.Port.LinePattern:X4}");
            i++;
        }
    }

    // [DIFFERENTIAL] 23CD:30E9
    // [DIFFERENTIAL] 23CD:311A
    [GameDataFact]
    public void Page_flip_shows_the_page_drawn_to()
    {
        var h = new DriverHarness(35);
        foreach (ushort seg in new ushort[] { 0xA400, 0xA000, 0xA400 })
        {
            h.SetPage(seg);
            for (int k = 0; k < 6; k++)
            {
                ushort w = h.Next(8, 120), hh = h.Next(8, 80);
                ushort sx = h.Next(0, 320 - w), sy = h.Next(0, 256 - hh), dx = h.Next(0, 319), dy = h.Next(0, 199);
                h.CallOriginal(0x06A0, sx, sy, w, hh, dx, dy);
                h.Port.BlitAtlasToPage(sx, sy, w, hh, dx, dy);
            }
            h.CallOriginal(0x0575, 0, seg);
            h.Port.SetDisplayStart(0, seg);
            h.AssertVramEqual($"page {seg:X4}");
            var (width, height, shown) = h.Original.Machine.Vga.RenderFrame();
            Assert.Equal((320, 200), (width, height));
            Assert.Equal(shown, h.Port.Vram.PageToLinear(h.Port.DisplayStart));
        }
    }

    // [DIFFERENTIAL] 23CD:04F6
    // [DIFFERENTIAL] 23CD:3042
    [GameDataFact]
    public void Card_selection_04F6_sets_up_mode_x_like_the_original()
    {
        var h = new DriverHarness(36);
        h.CallOriginal(0x0575, 0x0123, 0xA400);
        h.Port.SetDisplayStart(0x0123, 0xA400);
        h.Original.Machine.Vga.Seq[2] = 0x05;
        h.Port.Vram.MapMask = 0x05;
        h.Original.Machine.Vga.Gc[4] = 2;
        h.Port.Vram.ReadMap = 2;

        h.CallOriginal(0x04F6, 7);   // unknown mode: nothing happens
        h.Port.SelectCard(7);
        h.AssertVramEqual("mode 7");

        h.CallOriginal(0x04F6, 1);
        h.Port.SelectCard(1);
        h.AssertVramEqual("mode 1");
        Assert.Equal(h.Original.Machine.Vga.StartAddress, h.Port.DisplayStart);
        var vga = h.Original.Machine.Vga;
        Assert.Equal(0, vga.Seq[4] & 0x08);      // chain-4 off
        Assert.Equal(0, vga.Crtc[0x14] & 0x40);  // no doubleword mode
        Assert.Equal(0x40, vga.Crtc[0x17] & 0x40); // byte mode

        Assert.Throws<NotSupportedException>(() => h.Port.SelectCard(0));
    }

    // [DIFFERENTIAL] 23CD:04F6
    // [DIFFERENTIAL] 23CD:311A
    [GameDataFact]
    public void Initial_draw_page_is_the_one_in_the_original_image()
    {
        var original = Traders.RefEmu.OriginalProgram.LoadOnly(TestGameData.DosDir);
        var port = new ModeXDriver();
        ushort cs = original.Segment(0x23CD);
        Assert.Equal(original.Machine.Mem.Read16(cs, 0x0002), port.PageSegment);
        Assert.Equal(original.Machine.Mem.Read16(cs, 0x0000), port.PageOffset);
        original.Call(0x23CD, 0x04F6, 1);
        port.SelectCard(1);
        Assert.Equal(original.Machine.Mem.Read16(cs, 0x0002), port.PageSegment);
        Assert.Equal(original.Machine.Mem.Read16(cs, 0x0000), port.PageOffset);
    }
}
