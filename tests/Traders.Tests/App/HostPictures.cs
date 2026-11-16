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
                            $"original {a[k]:X2} host {b[k]:X2}");
        }
        var dac = host.Layers.Dac.Registers;
        int d = vga.Dac.AsSpan().CommonPrefixLength(dac);
        if (d < dac.Length)
            Assert.Fail($"{where}: DAC colour {d / 3} component {d % 3} original {vga.Dac[d]:X2} host {dac[d]:X2}");
    }
}
