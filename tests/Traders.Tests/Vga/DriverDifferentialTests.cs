using System.Text;
using Traders.Assets;
using Traders.Tests.RefEmu;
using Traders.Vga;

namespace Traders.Tests.Vga;

/// <summary>
/// Differential tests of the VGA driver routines in 23CD (Mode X): random call sequences are
/// executed by the original code in the reference emulator and by <see cref="ModeXDriver"/>;
/// after every call the full VRAM (and, where relevant, the RAM atlas) must be identical.
/// </summary>
public class DriverDifferentialTests
{
    private const int Calls = 40;

    // [DIFFERENTIAL] 23CD:31A6
    [GameDataFact]
    public void FillRect_31A6_matches_original()
    {
        var h = new DriverHarness(1);
        for (int i = 0; i < Calls; i++)
        {
            h.RandomPage();
            h.SetColor(h.Next(0, 255));
            ushort x = h.Next(0, 319), y = h.Next(0, 199), w = h.Next(1, 90), hh = h.Next(1, 50);
            h.CallOriginal(0x069B, x, y, w, hh);
            h.Port.FillRect(x, y, w, hh);
            h.AssertVramEqual($"FillRect({x},{y},{w},{hh}) #{i}");
        }
    }

    // [DIFFERENTIAL] 23CD:32D8
    [GameDataFact]
    public void BlitAtlasToPage_32D8_matches_original()
    {
        var h = new DriverHarness(2);
        for (int i = 0; i < Calls; i++)
        {
            h.RandomPage();
            ushort w = h.Next(1, 96), hh = h.Next(1, 60);
            ushort sx = h.Next(0, 320 - w), sy = h.Next(0, 256 - hh);
            ushort dx = h.Next(0, 319), dy = h.Next(0, 199);
            h.CallOriginal(0x06A0, sx, sy, w, hh, dx, dy);
            h.Port.BlitAtlasToPage(sx, sy, w, hh, dx, dy);
            h.AssertVramEqual($"Blit({sx},{sy},{w},{hh},{dx},{dy}) #{i}");
        }
    }

    // [DIFFERENTIAL] 23CD:3A9A
    [GameDataFact]
    public void BlitAtlasToPageFixed_3A9A_matches_original()
    {
        var h = new DriverHarness(3);
        for (int i = 0; i < Calls; i++)
        {
            ushort units = (ushort)(1 << h.Rng.Next(3)), hh = h.Next(1, 40);
            ushort sx = h.Next(0, 320 - units * 8), sy = h.Next(0, 256 - hh);
            ushort dx = h.Next(0, 300), dy = h.Next(0, 199);
            h.CallOriginal(0x06D4, sx, sy, units, hh, dx, dy);
            h.Port.BlitAtlasToPageFixed(sx, sy, units, hh, dx, dy);
            h.AssertVramEqual($"BlitFixed({sx},{sy},{units},{hh},{dx},{dy}) #{i}");
        }
    }

    // [DIFFERENTIAL] 23CD:3386
    [GameDataFact]
    public void OrAtlasToPage_3386_matches_original()
    {
        var h = new DriverHarness(4);
        for (int i = 0; i < Calls; i++)
        {
            h.RandomPage();
            ushort w = h.Next(4, 96), hh = h.Next(1, 50); // w < 4 runs 64 K bytes per row (LOOP with cx = 0)
            ushort sx = h.Next(0, 320 - w), sy = h.Next(0, 256 - hh);
            ushort dx = h.Next(0, 319), dy = h.Next(0, 199);
            h.CallOriginal(0x06AA, sx, sy, w, hh, dx, dy);
            h.Port.OrAtlasToPage(sx, sy, w, hh, dx, dy);
            h.AssertVramEqual($"Or({sx},{sy},{w},{hh},{dx},{dy}) #{i}");
        }
    }

    // [DIFFERENTIAL] 23CD:3449
    [GameDataFact]
    public void CopyAtlasToAtlas_3449_matches_original()
    {
        var h = new DriverHarness(5);
        for (int i = 0; i < Calls; i++)
        {
            ushort w = h.Next(1, 96), hh = h.Next(1, 60);
            ushort sx = h.Next(0, 320 - w), sy = h.Next(0, 256 - hh);
            ushort dx = h.Next(0, 320 - w), dy = h.Next(0, 256 - hh);
            h.CallOriginal(0x06AF, sx, sy, w, hh, dx, dy);
            h.Port.CopyAtlasToAtlas(sx, sy, w, hh, dx, dy);
            h.AssertAtlasEqual($"AtlasCopy({sx},{sy},{w},{hh},{dx},{dy}) #{i}");
        }
        h.AssertVramEqual("AtlasCopy leaves VRAM untouched");
    }

    // [DIFFERENTIAL] 23CD:351E
    [GameDataFact]
    public void CopyPageToPage_351E_matches_original()
    {
        var h = new DriverHarness(6);
        for (int i = 0; i < Calls; i++)
        {
            h.RandomPage();
            ushort w = h.Next(1, 96), hh = h.Next(1, 60);
            ushort sx = h.Next(0, 319), sy = h.Next(0, 199), dx = h.Next(0, 319), dy = h.Next(0, 199);
            h.CallOriginal(0x06B4, sx, sy, w, hh, dx, dy);
            h.Port.CopyPageToPage(sx, sy, w, hh, dx, dy);
            h.AssertVramEqual($"PageCopy({sx},{sy},{w},{hh},{dx},{dy}) #{i}");
        }
    }

    // [DIFFERENTIAL] 23CD:35DF
    [GameDataFact]
    public void MaskedBlit_35DF_matches_original()
    {
        var h = new DriverHarness(7);
        for (int i = 0; i < Calls; i++)
        {
            h.RandomPage();
            ushort w = h.Next(4, 96), hh = h.Next(1, 60); // w < 4: 64 K run per row in the original
            ushort sx = h.Next(0, 320 - w), sy = h.Next(0, 256 - hh);
            ushort mx = h.Next(0, 304 - w), my = h.Next(0, 200 - hh);
            ushort dx = h.Next(0, 319), dy = h.Next(0, 199);
            h.CallOriginal(0x06A5, sx, sy, w, hh, dx, dy, mx, my);
            h.Port.MaskedBlit(sx, sy, w, hh, dx, dy, mx, my);
            h.AssertVramEqual($"Masked({sx},{sy},{w},{hh},{dx},{dy},{mx},{my}) #{i}");
        }
    }

    // [DIFFERENTIAL] 23CD:3CE8
    [GameDataFact]
    public void MaskFill_3CE8_matches_original()
    {
        var h = new DriverHarness(8);
        for (int i = 0; i < Calls; i++)
        {
            h.RandomPage();
            ushort w = h.Next(4, 96), hh = h.Next(1, 60);
            ushort mx = h.Next(0, 304 - w), my = h.Next(0, 200 - hh);
            ushort dx = h.Next(0, 319), dy = h.Next(0, 199), c = h.Next(0, 255);
            h.CallOriginal(0x06E3, mx, my, w, hh, dx, dy, c);
            h.Port.MaskFill(mx, my, w, hh, dx, dy, c);
            h.AssertVramEqual($"MaskFill({mx},{my},{w},{hh},{dx},{dy},{c}) #{i}");
        }
    }

    // [DIFFERENTIAL] 23CD:373B
    // [DIFFERENTIAL] 23CD:3764
    [GameDataFact]
    public void SetPixel_373B_and_GetPixel_3764_match_original()
    {
        var h = new DriverHarness(9);
        for (int i = 0; i < 200; i++)
        {
            h.RandomPage();
            ushort x = h.Next(0, 319), y = h.Next(0, 199);
            if (h.Rng.Next(2) == 0)
            {
                ushort c = h.Next(0, 255);
                h.CallOriginal(0x06D9, x, y, c);
                h.Port.SetPixel(x, y, c);
            }
            else
            {
                var (ax, _) = h.CallOriginal(0x06DE, x, y);
                Assert.Equal((ushort)h.Port.GetPixel(x, y), ax);
            }
        }
        h.AssertVramEqual("SetPixel/GetPixel");
    }

    // [DIFFERENTIAL] 23CD:328E
    [GameDataFact]
    public void LatchCopy_328E_matches_original()
    {
        var h = new DriverHarness(10);
        for (int i = 0; i < Calls; i++)
        {
            ushort w = h.Next(1, 60), hh = h.Next(1, 40);
            ushort so = h.Next(0, 0x7FFF), d = h.Next(0, 0x7FFF);
            ushort ss = h.Rng.Next(2) == 0 ? (ushort)0xA000 : (ushort)0xA400, ds = h.Rng.Next(2) == 0 ? (ushort)0xA000 : (ushort)0xA400; // stay inside A0000-AFFFF
            h.CallOriginal(0x06B9, so, ss, d, ds, w, hh);
            h.Port.LatchCopy(so, ss, d, ds, w, hh);
            h.AssertVramEqual($"LatchCopy({so:X4}:{ss:X4} -> {d:X4}:{ds:X4}, {w}x{hh}) #{i}");
        }
    }

    // [DIFFERENTIAL] 23CD:05DD
    // [DIFFERENTIAL] 23CD:31FD
    [GameDataFact]
    public void DrawText_05DD_matches_original_for_both_fonts()
    {
        var h = new DriverHarness(11);
        for (int i = 0; i < Calls; i++)
        {
            h.RandomPage();
            ushort font = (ushort)h.Rng.Next(2);
            h.SetFont(font);
            h.SetColor(h.Next(0, 255));
            int last = font == 0 ? 91 : 102;
            var text = new byte[h.Rng.Next(1, 16)];
            for (int k = 0; k < text.Length; k++) text[k] = (byte)h.Rng.Next(32, last + 1);
            ushort x = h.Next(0, 200), y = h.Next(12, 199);
            var str = text.Append((byte)0).ToArray();
            h.Original.Machine.Mem.WriteBlock((uint)DriverHarness.ScratchSeg << 4, str);
            h.CallOriginal(0x05DD, x, y, 0, DriverHarness.ScratchSeg);
            h.Port.DrawText(x, y, str);
            h.AssertVramEqual($"Text({x},{y},font {font},\"{Encoding.ASCII.GetString(text)}\") #{i}");
        }
    }

    // [DIFFERENTIAL] 23CD:311A
    // [DIFFERENTIAL] 23CD:05B6
    // [DIFFERENTIAL] 23CD:05C3
    // [DIFFERENTIAL] 23CD:05D0
    // [DIFFERENTIAL] 23CD:30D1
    [GameDataFact]
    public void Driver_state_setters_and_controller_reset_match_original()
    {
        var h = new DriverHarness(14);
        for (int i = 0; i < Calls; i++)
        {
            ushort off = h.Next(0, 0xFFFF), seg = h.Next(0, 0xFFFF);
            h.CallOriginal(0x0570, off, seg);   // jmp far [cs:44] -> 311A
            h.Port.SetDrawPage(off, seg);
            Assert.Equal(h.DriverWord(0x0000), h.Port.PageOffset);
            Assert.Equal(h.DriverWord(0x0002), h.Port.PageSegment);

            ushort font = h.Next(0, 0xFFFF), color = h.Next(0, 0xFFFF), pattern = h.Next(0, 0xFFFF);
            h.CallOriginal(0x05B6, font);
            h.Port.Font = font;
            h.CallOriginal(0x05C3, color);
            h.Port.Color = color;
            h.CallOriginal(0x05D0, pattern);
            h.Port.LinePattern = pattern;
            Assert.Equal(h.DriverWord(0x0586), h.Port.Font);
            Assert.Equal(h.DriverWord(0x0588), h.Port.Color);
            Assert.Equal(h.DriverWord(0x04F4), h.Port.LinePattern);

            byte readMap = (byte)h.Next(0, 3);
            h.Original.Machine.Vga.Gc[4] = readMap;
            h.Port.Vram.ReadMap = readMap;
            h.CallOriginal(0x056B);             // jmp far [cs:04] -> 30D1
            h.Port.ResetGraphicsController();
            h.AssertRegistersEqual($"30D1 #{i}");
        }
    }

    // [DIFFERENTIAL] 23CD:311A
    [GameDataFact]
    public void Drawing_goes_to_the_selected_page()
    {
        var h = new DriverHarness(15);
        foreach (ushort seg in new ushort[] { 0xA000, 0xA400, 0xA800, 0xAC00, 0xA123 })
        {
            h.SetPage(seg);
            h.SetColor(h.Next(0, 255));
            ushort x = h.Next(0, 300), y = h.Next(0, 180);
            h.CallOriginal(0x069B, x, y, 17, 9);
            h.Port.FillRect(x, y, 17, 9);
            h.AssertVramEqual($"FillRect on page {seg:X4}");
        }
    }

    // [DIFFERENTIAL] 23CD:30E9
    [GameDataFact]
    public void SetDisplayStart_30E9_matches_original()
    {
        var h = new DriverHarness(12);
        foreach (var (off, seg) in new (ushort, ushort)[] { (0, 0xA000), (0, 0xA400), (0x0123, 0xA800), (0x50, 0xA000) })
        {
            h.CallOriginal(0x0575, off, seg);
            h.Port.SetDisplayStart(off, seg);
            Assert.Equal(h.Original.Machine.Vga.StartAddress, h.Port.DisplayStart);
        }
    }

    // [DIFFERENTIAL] 23CD:3079
    [GameDataFact]
    public void Planar_row_conversion_3079_matches_original()
    {
        var h = new DriverHarness(13);
        var buffer = new byte[PlanarAtlas.Length];
        h.Rng.NextBytes(buffer);
        var mem = h.Original.Machine.Mem;
        mem.WriteBlock((uint)DriverHarness.AtlasSeg << 4, buffer);
        // scratch row buffer used by the original: far pointer at DS:2BAC
        h.Original.WriteWord(0x2BAC, 0);
        h.Original.WriteWord(0x2BAE, DriverHarness.ScratchSeg);
        h.CallOriginal(0x3079, 4, DriverHarness.AtlasSeg, 320, 256);
        PlanarAtlas.ConvertToPlanarRows(buffer, 4, 320, 256);
        h.Port.Atlas = new PlanarAtlas(buffer);
        h.AssertAtlasEqual("3079 conversion");
    }

    [GameDataFact]
    public void Full_screen_blit_of_a_converted_picture_shows_the_picture()
    {
        var img = VgaImage.Load(RefEmuPaths.Dos("V-MENU.PP"));
        var d = new ModeXDriver { Atlas = PlanarAtlas.FromLinear(img.Pixels) };
        d.BlitAtlasToPage(0, 0, 320, 200, 0, 0);
        var screen = d.Vram.PageToLinear(0);
        Assert.Equal(img.Pixels.AsSpan(0, 320 * 200).ToArray(), screen);
    }
}
