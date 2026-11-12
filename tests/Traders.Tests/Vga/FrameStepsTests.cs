using Traders.Vga.Layers;

namespace Traders.Tests.Vga;

/// <summary>
/// The frame-stepped presenting routines of <see cref="GraphicsLayers"/> (FrameSteps.cs) against the original, retrace by
/// retrace: at every entry into 0D73:00D3 the original's data segment, DAC and CRTC start are captured; the port must have
/// the same state at each yield of the step sequence (one yield per retrace wait), and the same state at the end.
/// </summary>
public class FrameStepsTests
{
    private const ushort Palettes = 0x21FD;

    private sealed record State(ulong Ds, ulong Dac, int Start, int Retraces);

    private static ulong Hash(ReadOnlySpan<byte> bytes)
    {
        ulong h = 1469598103934665603UL;
        foreach (byte b in bytes) h = (h ^ b) * 1099511628211UL;
        return h;
    }

    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<LayerHarness, List<State>> Recording = new();

    private static List<State> CaptureOriginal(LayerHarness h, ushort offset, params ushort[] args)
    {
        var o = h.Original;
        if (!Recording.TryGetValue(h, out var states))
        {
            states = [];
            Recording.Add(h, states);
            o.Machine.AddCodeHook(o.Segment(0x0D73), 0x00D3, m =>
                states.Add(new State(Hash(o.DataSegment()), Hash(m.Vga.Dac), m.Vga.StartAddress, states.Count)));
        }
        states.Clear();
        h.Call(Palettes, offset, args);
        return [.. states];
    }

    private static List<State> CapturePort(LayerHarness h, IEnumerable<int> steps)
    {
        var states = new List<State>();
        int before = h.PortRetraces.Count;
        foreach (int _ in steps)
        {
            Assert.Equal(before + states.Count + 1, h.PortRetraces.Count); // exactly one retrace wait per step
            states.Add(new State(Hash(h.Port.Ds), Hash(h.Port.Dac.Registers), h.Driver.Port.DisplayStart, states.Count));
        }
        return states;
    }

    private static void AssertSameSteps(LayerHarness h, string what, ushort offset, ushort[] args, IEnumerable<int> steps)
    {
        var original = CaptureOriginal(h, offset, args);
        var port = CapturePort(h, steps);
        for (int i = 0; i < Math.Min(original.Count, port.Count); i++)
            Assert.True(original[i] == port[i], $"{what}: retrace {i} original {original[i]} port {port[i]}");
        Assert.True(original.Count == port.Count, $"{what}: retraces original {original.Count} port {port.Count}");
        h.AssertScreenEqual(what);
    }

    // [DIFFERENTIAL] 21FD:0329
    // [DIFFERENTIAL] 21FD:0353
    // [DIFFERENTIAL] 21FD:03B4
    // [DIFFERENTIAL] 21FD:051F
    [GameDataFact]
    public void Frame_steps_match_original_retrace_by_retrace()
    {
        var h = new LayerHarness(310);
        h.WriteDwordBoth(GraphicsLayers.Page0, 0xA000_0000);
        h.WriteDwordBoth(GraphicsLayers.Page1, 0xA400_0000);
        h.WriteDwordBoth(GraphicsLayers.PageNumbers, 1);
        h.WriteDwordBoth(GraphicsLayers.PageNumbers + 4, 2);
        for (int i = 0; i < 12; i++)
        {
            ushort color = h.Next(0, 0x0FFF);
            h.Randomize(GraphicsLayers.BasePalette, 768);
            h.Randomize(GraphicsLayers.ObjectRestoreRecords, 0x40);
            AssertSameSteps(h, $"fade-out #{i}", 0x03B4, [color], h.Port.FadeOutFastSteps(color));
            h.Randomize(GraphicsLayers.BasePalette, 768);
            if (i % 2 == 0) AssertSameSteps(h, $"fade-in #{i}", 0x0353, [], h.Port.FadeInFastSteps());
            else AssertSameSteps(h, $"slow fade-in #{i}", 0x0329, [], h.Port.FadeInSteps());
            h.WriteWordBoth(GraphicsLayers.DrawPageIndex, h.Next(0, 1));
            h.WriteBoth(GraphicsLayers.NoFlipWait, [(byte)(i % 3 == 1 ? 1 : 0)]);
            h.WriteBoth(GraphicsLayers.SlowFlip, [(byte)(i % 4 == 2 ? 1 : 0)]);
            AssertSameSteps(h, $"flip #{i}", 0x051F, [], h.Port.FlipPagesSteps());
        }
    }

    /// <summary>A step sequence drained at once leaves the same state as the synchronous routine.</summary>
    [GameDataFact]
    public void Drained_steps_equal_synchronous_routines()
    {
        var a = new LayerHarness(320);
        var b = new LayerHarness(320);
        foreach (var h in new[] { a, b })
        {
            h.WriteDwordBoth(GraphicsLayers.Page0, 0xA000_0000);
            h.WriteDwordBoth(GraphicsLayers.Page1, 0xA400_0000);
            h.WriteDwordBoth(GraphicsLayers.PageNumbers, 1);
            h.WriteDwordBoth(GraphicsLayers.PageNumbers + 4, 2);
            h.WriteBoth(GraphicsLayers.SlowFlip, [1]);
        }
        a.Port.FadeOutFast(0x0420);
        foreach (int _ in b.Port.FadeOutFastSteps(0x0420)) { }
        a.Port.FadeIn();
        foreach (int _ in b.Port.FadeInSteps()) { }
        a.Port.FadeInFast();
        foreach (int _ in b.Port.FadeInFastSteps()) { }
        a.Port.FlipPages();
        foreach (int _ in b.Port.FlipPagesSteps()) { }
        Assert.Equal(a.Port.Ds, b.Port.Ds);
        Assert.Equal(a.Port.Dac.Registers, b.Port.Dac.Registers);
        Assert.Equal(a.Driver.Port.DisplayStart, b.Driver.Port.DisplayStart);
        Assert.Equal(a.PortRetraces.Count, b.PortRetraces.Count);
    }
}
