using Traders.RefEmu;
using Traders.Vga.Layers;

namespace Traders.Tests.Vga;

/// <summary>
/// The graphics layers (0B67, 21FD palettes and flip, 2295, 21AF) in the reference emulator next to
/// <see cref="GraphicsLayers"/>: both sides start from the same driver state (<see cref="DriverHarness"/>), the same
/// 64 KB data segment and the same DAC. After each call the whole data segment, the DAC, VRAM, CRTC start, the
/// driver's draw page and the number of retrace waits (entries into 0D73:00D3) must be equal.
/// </summary>
internal sealed class LayerHarness
{
    private const ushort RetraceSegment = 0x0D73, RetraceOffset = 0x00D3;

    public DriverHarness Driver { get; }
    public OriginalProgram Original => Driver.Original;
    public GraphicsLayers Port { get; }
    public RetraceCounter PortRetraces { get; } = new();
    public int OriginalRetraces { get; private set; }
    public Random Rng => Driver.Rng;

    public LayerHarness(int seed, ushort cardMode = 1)
    {
        Driver = new DriverHarness(seed);
        Original.Machine.AddCodeHook(Original.Segment(RetraceSegment), RetraceOffset, _ => OriginalRetraces++);
        Original.WriteWord(GraphicsLayers.CardMode, cardMode);
        var dac = new VgaDac();
        Original.Machine.Vga.Dac.CopyTo(dac.Registers, 0);
        Port = new GraphicsLayers(Driver.Port, Original.DataSegment(), dac, PortRetraces);
    }

    public ushort Next(int minInclusive, int maxInclusive) => (ushort)Rng.Next(minInclusive, maxInclusive + 1);

    public (ushort Ax, ushort Dx) Call(ushort segment, ushort offset, params ushort[] args) => Original.Call(segment, offset, args);

    /// <summary>Random bytes at the same DS offsets on both sides.</summary>
    public void Randomize(int dsOffset, int length)
    {
        var bytes = new byte[length];
        Rng.NextBytes(bytes);
        WriteBoth(dsOffset, bytes);
    }

    public void WriteBoth(int dsOffset, ReadOnlySpan<byte> bytes)
    {
        for (int i = 0; i < bytes.Length; i++)
        {
            Original.WriteByte((ushort)(dsOffset + i), bytes[i]);
            Port.Ds[(dsOffset + i) & 0xFFFF] = bytes[i];
        }
    }

    public void WriteWordBoth(int dsOffset, ushort value) => WriteBoth(dsOffset, [(byte)value, (byte)(value >> 8)]);

    public void WriteDwordBoth(int dsOffset, uint value) =>
        WriteBoth(dsOffset, [(byte)value, (byte)(value >> 8), (byte)(value >> 16), (byte)(value >> 24)]);

    /// <summary>Data segment, DAC and retrace count equal.</summary>
    public void AssertStateEqual(string what)
    {
        var ds = Original.DataSegment();
        for (int i = 0; i < ds.Length; i++)
            if (ds[i] != Port.Ds[i])
                Assert.Fail($"{what}: DS:{i:X4} original {ds[i]:X2} port {Port.Ds[i]:X2}");
        var dac = Original.Machine.Vga.Dac;
        for (int i = 0; i < dac.Length; i++)
            if (dac[i] != Port.Dac.Registers[i])
                Assert.Fail($"{what}: DAC {i / 3}.{i % 3} original {dac[i]:X2} port {Port.Dac.Registers[i]:X2}");
        Assert.True(OriginalRetraces == PortRetraces.Count,
            $"{what}: retrace waits original {OriginalRetraces} port {PortRetraces.Count}");
    }

    /// <summary>Additionally CRTC start, draw page (driver CS:0000/0002) and VRAM equal.</summary>
    public void AssertScreenEqual(string what)
    {
        AssertStateEqual(what);
        Assert.True(Original.Machine.Vga.StartAddress == Driver.Port.DisplayStart,
            $"{what}: CRTC start original {Original.Machine.Vga.StartAddress:X4} port {Driver.Port.DisplayStart:X4}");
        Assert.True(Driver.DriverWord(0x0000) == Driver.Port.PageOffset && Driver.DriverWord(0x0002) == Driver.Port.PageSegment,
            $"{what}: draw page original {Driver.DriverWord(0x0002):X4}:{Driver.DriverWord(0x0000):X4} port {Driver.Port.PageSegment:X4}:{Driver.Port.PageOffset:X4}");
        Driver.AssertVramEqual(what);
    }
}
