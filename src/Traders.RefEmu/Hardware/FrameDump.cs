using System.Security.Cryptography;
using Traders.Assets;

namespace Traders.RefEmu.Hardware;

/// <summary>Captured picture of the VGA output: indexed pixels plus the DAC palette at capture time.</summary>
public sealed class FrameDump
{
    public int Width { get; }
    public int Height { get; }
    public byte[] Pixels { get; }
    public byte[] Palette { get; }

    private FrameDump(int width, int height, byte[] pixels, byte[] palette)
    {
        Width = width;
        Height = height;
        Pixels = pixels;
        Palette = palette;
    }

    public static FrameDump Capture(VgaAdapter vga)
    {
        var (w, h, px) = vga.RenderFrame();
        return new FrameDump(w, h, px, (byte[])vga.Dac.Clone());
    }

    public bool IsEmpty => Width == 0 || Height == 0;

    /// <summary>Hash over size, pixels and palette (to detect changed frames).</summary>
    public string Hash()
    {
        using var sha = SHA256.Create();
        var head = BitConverter.GetBytes(Width).Concat(BitConverter.GetBytes(Height)).ToArray();
        sha.TransformBlock(head, 0, head.Length, null, 0);
        sha.TransformBlock(Pixels, 0, Pixels.Length, null, 0);
        sha.TransformFinalBlock(Palette, 0, Palette.Length);
        return Convert.ToHexString(sha.Hash!)[..16];
    }

    public void WritePng(string path)
    {
        if (IsEmpty) throw new InvalidOperationException("Display is not in a graphics mode.");
        using var f = File.Create(path);
        PngWriter.WriteIndexed(f, Width, Height, Pixels, Traders.Assets.Palette.FromVga6(Palette));
    }
}
