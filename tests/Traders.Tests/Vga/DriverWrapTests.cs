using Traders.Vga;

namespace Traders.Tests.Vga;

/// <summary>
/// Driver calls whose 16-bit offsets leave the buffers they start in, against the original in the reference emulator:
/// narrow widths (w &lt; 4) whose zero-byte planes run 65536 bytes per row because of LOOP, reads behind the RAM atlas,
/// and pages whose offsets run past the end of the A000h window (AFFFFh). Findings B1/B2 of the overall review R2
/// (docs/reviews/2026-11-11-r2-gesamt.md); expectations come from the emulator only.
/// </summary>
public class DriverWrapTests
{
    // [DIFFERENTIAL] 23CD:06AA
    // [DIFFERENTIAL] 23CD:3386
    [GameDataFact]
    public void B1_narrow_or_blit_at_atlas_row_200_matches_original()
    {
        var h = new DriverHarness(71);
        h.SetPage(0xA000);
        h.CallOriginal(0x06AA, 0, 200, 1, 1, 0, 0);
        h.Port.OrAtlasToPage(0, 200, 1, 1, 0, 0);
        h.AssertVramEqual("B1");
    }

    // [DIFFERENTIAL] 23CD:06AA
    // [DIFFERENTIAL] 23CD:3386
    [GameDataFact]
    public void Narrow_or_blits_with_every_plane_alignment_match_original()
    {
        var h = new DriverHarness(73);
        for (ushort w = 1; w <= 3; w++)
            for (ushort da = 0; da < 4; da++)
            {
                h.RandomPage();
                // sy <= 204: the 64 KB run behind the atlas stays in emulator memory that holds nothing else
                ushort sx = h.Next(0, 319), sy = h.Next(150, 204), hh = h.Next(1, 2);
                ushort dx = (ushort)(h.Next(0, 79) * 4 + da), dy = h.Next(0, 199);
                h.CallOriginal(0x06AA, sx, sy, w, hh, dx, dy);
                h.Port.OrAtlasToPage(sx, sy, w, hh, dx, dy);
                h.AssertVramEqual($"Or w={w} dx={dx} sy={sy} h={hh} page {h.Port.PageSegment:X4}");
                h.AssertOutsideWindowEqual($"Or w={w} dx={dx}");
            }
    }

    // [DIFFERENTIAL] 23CD:35DF
    // [DIFFERENTIAL] 23CD:3CE8
    [GameDataFact]
    public void Narrow_masked_blits_and_fills_match_original()
    {
        var h = new DriverHarness(74);
        h.SyncMaskSegment();   // the 64 KB mask run reads past MASKS.PP into the rest of the mask segment
        for (ushort w = 1; w <= 3; w++)
            for (ushort da = 0; da < 4; da += 3)
            {
                h.RandomPage();
                ushort sx = h.Next(0, 319), sy = h.Next(0, 204), hh = h.Next(1, 2);
                ushort dx = (ushort)(h.Next(0, 79) * 4 + da), dy = h.Next(0, 199);
                // even mask offset (mx/8 even): with an odd one the 64 KB run reads a word at offset FFFFh, which
                // faults on a 286 (the original does not return in the RefEmu)
                ushort mx = (ushort)(h.Next(0, 19) * 16 + h.Next(0, 7)), my = h.Next(0, 199);
                h.CallOriginal(0x06A5, sx, sy, w, hh, dx, dy, mx, my);
                h.Port.MaskedBlit(sx, sy, w, hh, dx, dy, mx, my);
                h.AssertVramEqual($"MaskedBlit w={w} dx={dx} page {h.Port.PageSegment:X4}");
                ushort c = h.Next(0, 255);
                h.CallOriginal(0x06E3, mx, my, w, hh, dx, dy, c);
                h.Port.MaskFill(mx, my, w, hh, dx, dy, c);
                h.AssertVramEqual($"MaskFill w={w} dx={dx} page {h.Port.PageSegment:X4}");
                h.AssertOutsideWindowEqual($"masked w={w} dx={dx}");
            }
    }

    // [DIFFERENTIAL] 23CD:069B
    // [DIFFERENTIAL] 23CD:31A6
    [GameDataFact]
    public void B2_zero_rows_on_page_A400_write_past_the_window_like_the_original()
    {
        var h = new DriverHarness(72);
        h.SetPage(0xA400);
        h.SetColor(123);
        h.CallOriginal(0x069B, 0, 7, 4, 0);
        h.Port.FillRect(0, 7, 4, 0);
        h.AssertVramEqual("B2");
        h.AssertOutsideWindowEqual("B2");
    }

    // [DIFFERENTIAL] 23CD:069B
    // [DIFFERENTIAL] 23CD:31A6
    // [DIFFERENTIAL] 23CD:328E
    // [DIFFERENTIAL] 23CD:351E
    [GameDataFact]
    public void Zero_rows_on_page_A400_and_reads_past_the_window_match_original()
    {
        var h = new DriverHarness(75);
        h.SetPage(0xA400);
        foreach (var (x, w) in new (ushort, ushort)[] { (13, 3), (301, 7) })
        {
            h.SetColor(h.Next(0, 255));
            h.CallOriginal(0x069B, x, 7, w, 0);
            h.Port.FillRect(x, 7, w, 0);
            h.AssertVramEqual($"FillRect({x},7,{w},0) on A400");
            h.AssertOutsideWindowEqual($"FillRect({x},7,{w},0) on A400");
        }

        // latch copy from behind the window (B000:0000 = A400 page offset C000h) into the first page, and back out
        h.CallOriginal(0x06B9, 0x0010, 0xB000, 0x0100, 0xA000, 40, 30);
        h.Port.LatchCopy(0x0010, 0xB000, 0x0100, 0xA000, 40, 30);
        h.AssertVramEqual("LatchCopy B000 -> A000");
        h.CallOriginal(0x06B9, 0x0200, 0xA000, 0x0400, 0xB100, 40, 30);
        h.Port.LatchCopy(0x0200, 0xA000, 0x0400, 0xB100, 40, 30);
        h.AssertVramEqual("LatchCopy A000 -> B100");
        h.AssertOutsideWindowEqual("LatchCopy A000 -> B100");

        // page copy on A400 from rows past the window (y = 200 + 7 → offset 4000h + 40B0h + wrap) back into it
        h.CallOriginal(0x06B4, 0, 760, 37, 20, 3, 10);
        h.Port.CopyPageToPage(0, 760, 37, 20, 3, 10);
        h.AssertVramEqual("PageCopy from past the window");
    }
}
