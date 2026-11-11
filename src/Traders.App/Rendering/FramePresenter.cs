using Traders.Assets;
using Traders.Vga;

namespace Traders.App.Rendering;

/// <summary>
/// Converts the displayed mode-X page into 32-bit pixels for the host window: 320 × 200 palette indices from the
/// four planes starting at the display start address, then the 6-bit DAC values scaled to 8 bits.
/// </summary>
public static class FramePresenter
{
    public const int Width = 320, Height = 200;

    /// <summary>
    /// Fills <paramref name="rgba"/> (Width·Height entries, packed as R | G &lt;&lt; 8 | B &lt;&lt; 16 | A &lt;&lt; 24 like
    /// MonoGame's Color) from the page at <paramref name="displayStart"/> and the DAC (768 bytes, 6 bits each).
    /// </summary>
    public static void Present(ModeXMemory vram, int displayStart, ReadOnlySpan<byte> dac, Span<uint> rgba)
    {
        if (dac.Length < 768) throw new ArgumentException("DAC needs 768 bytes.", nameof(dac));
        if (rgba.Length < Width * Height) throw new ArgumentException("Target too small.", nameof(rgba));
        Span<uint> lut = stackalloc uint[256];
        for (int i = 0; i < 256; i++)
        {
            uint r = Palette.To8Bit((byte)(dac[3 * i] & 0x3F));
            uint g = Palette.To8Bit((byte)(dac[3 * i + 1] & 0x3F));
            uint b = Palette.To8Bit((byte)(dac[3 * i + 2] & 0x3F));
            lut[i] = r | g << 8 | b << 16 | 0xFF000000u;
        }
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                rgba[y * Width + x] = lut[vram.Pixel(displayStart, x, y)];
    }
}
