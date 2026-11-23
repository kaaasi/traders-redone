using Traders.App;
using Traders.RefEmu;

namespace Traders.Tests.App;

/// <summary>The host's picture against the original's at a retrace (frame-by-frame host tests).</summary>
internal static class HostPictures
{
    /// <summary>
    /// The picture on the screen: display start, the shown page (16000 bytes per plane from the display start) and the DAC;
    /// the hidden page is left out.
    /// </summary>
    internal static void CompareShown(OriginalProgram o, ProgramHost host, string where)
    {
        var vga = o.Machine.Vga;
        Assert.True(vga.StartAddress == host.Driver.DisplayStart,
            $"{where}: display start original {vga.StartAddress:X4}, host {host.Driver.DisplayStart:X4}");
        int start = vga.StartAddress;
        for (int pl = 0; pl < 4; pl++)
        {
            var a = vga.Planes[pl].AsSpan(start, ShownBytes);
            var b = host.Driver.Vram.Planes[pl].AsSpan(start, ShownBytes);
            int k = a.CommonPrefixLength(b);
            if (k < ShownBytes)
                Assert.Fail($"{where}: shown page, plane {pl} offset {k:X4} (x {k % 80 * 4 + pl}, y {k / 80}) original {a[k]:X2} host {b[k]:X2}");
        }
        var dac = host.Layers.Dac.Registers;
        int d = vga.Dac.AsSpan().CommonPrefixLength(dac);
        if (d < dac.Length)
            Assert.Fail($"{where}: DAC colour {d / 3} component {d % 3} original {vga.Dac[d]:X2} host {dac[d]:X2}");
    }

    /// <summary>
    /// Diagnostics of a failed comparison: per page the differing pixels and their bounding box, for a small difference its
    /// top-left corner (up to 21 rows of 61 pixels, low nibble per pixel) of the original and of the host side by side.
    /// </summary>
    private static string Differences(OriginalProgram o, ProgramHost host)
    {
        var vga = o.Machine.Vga;
        var sb = new System.Text.StringBuilder();
        for (int page = 0; page < 4; page++)
        {
            int x0 = 999, y0 = 999, x1 = -1, y1 = -1, n = 0;
            for (int y = 0; y < 200; y++)
                for (int x = 0; x < 320; x++)
                {
                    int off = page * 0x4000 + y * 80 + x / 4;
                    if (off >= vga.Planes[0].Length) continue;
                    if (vga.Planes[x % 4][off] == host.Driver.Vram.Planes[x % 4][off]) continue;
                    n++;
                    (x0, y0, x1, y1) = (Math.Min(x0, x), Math.Min(y0, y), Math.Max(x1, x), Math.Max(y1, y));
                }
            sb.Append($"page {page}: {n} px in ({x0},{y0})-({x1},{y1}); ");
            if (n > 0 && n < 5000)
            {
                for (int y = y0; y <= Math.Min(y1, y0 + 20); y++)
                {
                    sb.Append('\n');
                    for (int x = x0; x <= Math.Min(x1, x0 + 60); x++) sb.Append(Nibble(vga.Planes, page, x, y));
                    sb.Append(" | ");
                    for (int x = x0; x <= Math.Min(x1, x0 + 60); x++) sb.Append(Nibble(host.Driver.Vram.Planes, page, x, y));
                }
                sb.Append('\n');
            }
        }
        return sb.ToString();
    }

    private static string Nibble(byte[][] planes, int page, int x, int y) =>
        (planes[x % 4][page * 0x4000 + y * 80 + x / 4] & 0xF).ToString("X");

    /// <summary>Bytes of a page per plane (320 × 200 / 4).</summary>
    private const int ShownBytes = 16000;

    /// <summary>VRAM planes, DAC and CRTC start of the host against the original's (first difference named).</summary>
    internal static void Compare(OriginalProgram o, ProgramHost host, string where)
    {
        var vga = o.Machine.Vga;
        Assert.True(vga.StartAddress == host.Driver.DisplayStart,
            $"{where}: display start original {vga.StartAddress:X4}, host {host.Driver.DisplayStart:X4}");
        for (int pl = 0; pl < 4; pl++)
        {
            var a = vga.Planes[pl].AsSpan();
            var b = host.Driver.Vram.Planes[pl].AsSpan();
            int k = a.CommonPrefixLength(b);
            if (k < Math.Min(a.Length, b.Length))
                Assert.Fail($"{where}: plane {pl} offset {k:X4} (page {k / 0x4000}, x {k % 0x4000 % 80 * 4 + pl}, y {k % 0x4000 / 80}) " +
                            $"original {a[k]:X2} host {b[k]:X2}; {Differences(o, host)}");
        }
        var dac = host.Layers.Dac.Registers;
        int d = vga.Dac.AsSpan().CommonPrefixLength(dac);
        if (d < dac.Length)
            Assert.Fail($"{where}: DAC colour {d / 3} component {d % 3} original {vga.Dac[d]:X2} host {dac[d]:X2}");
    }
}
