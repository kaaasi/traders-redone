using Traders.Vga;

namespace Traders.App.Rendering;

/// <summary>Content for the host window while there are no scenes yet (R3).</summary>
public static class DemoPicture
{
    /// <summary>Writes linear 320-wide pixels (at least 200 rows) into page 0 of the planar memory.</summary>
    public static void Write(ModeXMemory vram, ReadOnlySpan<byte> linear)
    {
        for (int y = 0; y < FramePresenter.Height; y++)
            for (int x = 0; x < FramePresenter.Width; x++)
                vram.Planes[x & 3][y * ModeXMemory.BytesPerRow + (x >> 2)] = linear[y * FramePresenter.Width + x];
    }

    /// <summary>All 256 palette indices as 16 × 16 blocks with a simple colour ramp as DAC; returns the DAC.</summary>
    public static byte[] ColourBars(ModeXMemory vram)
    {
        var linear = new byte[FramePresenter.Width * FramePresenter.Height];
        for (int y = 0; y < FramePresenter.Height; y++)
            for (int x = 0; x < FramePresenter.Width; x++)
                linear[y * FramePresenter.Width + x] = (byte)(y * 16 / FramePresenter.Height * 16 + x * 16 / FramePresenter.Width);
        Write(vram, linear);
        var dac = new byte[768];
        for (int i = 0; i < 256; i++)
        {
            dac[3 * i] = (byte)(i >> 2);
            dac[3 * i + 1] = (byte)((i & 0x0F) * 4);
            dac[3 * i + 2] = (byte)((i >> 4) * 4);
        }
        return dac;
    }
}
