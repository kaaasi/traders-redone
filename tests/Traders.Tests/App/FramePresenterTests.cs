using Traders.App;
using Traders.App.Rendering;
using Traders.Assets;
using Traders.RefEmu;
using Traders.Vga;

namespace Traders.Tests.App;

/// <summary>The host shows exactly the picture the RefEmu displays (indices from the display start, DAC colours).</summary>
public class FramePresenterTests
{
    [GameDataFact]
    public void Main_menu_frame_matches_the_refemu_display_pixel_for_pixel()
    {
        var p = OriginalProgram.BootToMainMenu(TestGameData.DosDir);
        var vga = p.Machine.Vga;
        var frame = p.Frame();
        Assert.Equal((320, 200), (frame.Width, frame.Height));

        var vram = new ModeXMemory();
        for (int plane = 0; plane < 4; plane++) vga.Planes[plane].CopyTo(vram.Planes[plane], 0);
        var rgba = new uint[320 * 200];
        FramePresenter.Present(vram, vga.StartAddress, vga.Dac, rgba);

        for (int i = 0; i < rgba.Length; i++)
        {
            int c = frame.Pixels[i];
            uint expected = Palette.To8Bit(frame.Palette[3 * c]) | (uint)Palette.To8Bit(frame.Palette[3 * c + 1]) << 8
                | (uint)Palette.To8Bit(frame.Palette[3 * c + 2]) << 16 | 0xFF000000u;
            Assert.True(expected == rgba[i], $"pixel {i % 320},{i / 320}");
        }
        Assert.True(frame.Pixels.Distinct().Count() > 8); // a real picture, not an empty page
    }

    [Fact]
    public void Display_start_selects_the_page()
    {
        var vram = new ModeXMemory();
        vram.Planes[1][0x4000 + 80 * 3 + 2] = 5; // page at 4000h, x = 4·2 + 1 = 9, y = 3
        var dac = new byte[768];
        dac[15] = 63;
        var rgba = new uint[320 * 200];
        FramePresenter.Present(vram, 0x4000, dac, rgba);
        Assert.Equal(0xFF0000FFu, rgba[3 * 320 + 9]);
        Assert.Equal(0xFF000000u, rgba[3 * 320 + 8]);
        FramePresenter.Present(vram, 0, dac, rgba);
        Assert.Equal(0xFF000000u, rgba[3 * 320 + 9]);
    }

    [Theory]
    [InlineData(960, 600, false, 0, 0, 960, 600)]
    [InlineData(1000, 700, false, 20, 50, 960, 600)]
    [InlineData(300, 150, false, -10, -25, 320, 200)]
    [InlineData(960, 720, true, 0, 0, 960, 720)]
    [InlineData(1280, 720, true, 160, 0, 960, 720)]
    public void Viewport_scales_by_integers_or_to_four_by_three(int ww, int wh, bool aspect, int x, int y, int w, int h)
    {
        var r = HostGame.Viewport(ww, wh, aspect);
        Assert.Equal((x, y, w, h), (r.X, r.Y, r.Width, r.Height));
    }
}
