using Traders.Tests.RefEmu;
using Traders.Vga.Layers;

namespace Traders.Tests.Vga;

/// <summary>
/// Differential tests of the palette layer (21FD:026C…04DA), the colour registers (2295:006D/00C5) and the clearing
/// of the object pointer pairs (21AF:04A2/04CD): random data segment contents, the original routine in the RefEmu and
/// the port; afterwards data segment (including the random state DS:10D2), DAC and retrace waits equal.
/// </summary>
public class PaletteLayerTests
{
    private const ushort Palettes = 0x21FD, Colors = 0x2295, Objects = 0x21AF;

    /// <summary>12-bit colours plus values with bits above the twelfth (they reach red through the byte shift).</summary>
    private static ushort RandomColor(LayerHarness h) => h.Rng.Next(4) == 0 ? h.Next(0, 0xFFFF) : h.Next(0, 0x0FFF);

    // [DIFFERENTIAL] 2295:006D
    [GameDataFact]
    public void SetColorRegister_2295_006D_matches_original()
    {
        foreach (ushort mode in new ushort[] { 1, 0, 2 })
        {
            var h = new LayerHarness(100 + mode, mode);
            for (int i = 0; i < 60; i++)
            {
                ushort index = i switch { 0 => 0, 1 => 255, 2 => 256, 3 => 0xFFFF, _ => h.Next(0, i % 5 == 0 ? 0xFFFF : 255) };
                ushort rgb = i switch { 0 => 0x0FFF, 1 => 0xFFFF, 2 => 0x8000, _ => RandomColor(h) };
                h.Call(Colors, 0x006D, index, rgb);
                h.Port.SetColorRegister(index, rgb);
                h.AssertStateEqual($"mode {mode} 006D({index:X4},{rgb:X4}) #{i}");
            }
        }
    }

    // [DIFFERENTIAL] 2295:00C5
    [GameDataFact]
    public void SetAllColorRegisters_2295_00C5_matches_original()
    {
        foreach (ushort mode in new ushort[] { 1, 0 })
        {
            var h = new LayerHarness(110 + mode, mode);
            foreach (ushort at in new ushort[] { 0x5017, 0x3DCB, 0x8000, 0xC000 })
            {
                h.Randomize(at, 768);
                h.Call(Colors, 0x00C5, at, h.Original.Dgroup);
                h.Port.SetAllColorRegisters(h.Port.Ds.AsSpan(at, 768));
                h.AssertStateEqual($"mode {mode} 00C5(DS:{at:X4})");
            }
        }
    }

    // [DIFFERENTIAL] 21FD:026C
    [GameDataFact]
    public void CopyLoadedToBase_21FD_026C_matches_original()
    {
        var h = new LayerHarness(120);
        for (int i = 0; i < 3; i++)
        {
            h.Randomize(GraphicsLayers.LoadedPalette, 768);
            h.Randomize(GraphicsLayers.BasePalette, 768);
            h.Call(Palettes, 0x026C);
            h.Port.CopyLoadedToBase();
            h.AssertStateEqual($"026C #{i}");
        }
    }

    // [DIFFERENTIAL] 21FD:02C9
    [GameDataFact]
    public void SetBaseColor_21FD_02C9_matches_original()
    {
        var h = new LayerHarness(130);
        // 5555h·3 = FFFFh and 5556h·3 = 2 (mod 10000h): the 16-bit address wraps into the palette itself.
        ushort[] fixedIndex = [0, 255, 256, 0x5555, 0x5556, 0xAAAB];
        for (int i = 0; i < 80; i++)
        {
            ushort index = i < fixedIndex.Length ? fixedIndex[i] : h.Next(0, 300);
            ushort rgb = i < fixedIndex.Length ? (ushort)0xFFFF : RandomColor(h);
            h.Call(Palettes, 0x02C9, index, rgb);
            h.Port.SetBaseColor(index, rgb);
            h.AssertStateEqual($"02C9({index:X4},{rgb:X4}) #{i}");
        }
    }

    // [DIFFERENTIAL] 21FD:03EB
    [GameDataFact]
    public void Blend_21FD_03EB_matches_original()
    {
        foreach (ushort mode in new ushort[] { 1, 0 })
        {
            var h = new LayerHarness(140 + mode, mode);
            for (int i = 0; i < 24; i++)
            {
                // full byte range in the base palette: components ≥ 80h compare as negative
                h.Randomize(GraphicsLayers.BasePalette, 768);
                ushort rgb = i switch { 0 => 0, 1 => 0x0FFF, 2 => 0xF888, _ => RandomColor(h) };
                ushort steps = i switch { 0 => 0, 1 => 7, 2 => 0x20, 3 => 0xFFFF, 4 => 16, _ => h.Next(0, 31) };
                h.Call(Palettes, 0x03EB, rgb, steps);
                h.Port.Blend(rgb, steps);
                h.AssertStateEqual($"mode {mode} 03EB({rgb:X4},{steps:X4}) #{i}");
            }
        }
    }

    // [DIFFERENTIAL] 21FD:0329
    // [DIFFERENTIAL] 21FD:0353
    // [DIFFERENTIAL] 21FD:04DA
    // [DIFFERENTIAL] 21FD:0185
    [GameDataFact]
    public void FadeIn_21FD_0329_and_0353_match_original()
    {
        var h = new LayerHarness(150);
        for (int i = 0; i < 4; i++)
        {
            ushort blend = RandomColor(h);
            h.Call(Palettes, 0x04DA, blend);
            h.Port.SetBlendColor(blend);
            h.Randomize(GraphicsLayers.BasePalette, 768);
            if (i % 2 == 0)
            {
                h.Call(Palettes, 0x0329);
                h.Port.FadeIn();
            }
            else
            {
                h.Call(Palettes, 0x0353);
                h.Port.FadeInFast();
            }
            h.AssertStateEqual($"fade in #{i} from {blend:X4}");
        }
        Assert.Equal(2 * (8 * 3 + 8) + 2 * (8 * 1 + 8), h.PortRetraces.Count);   // per step: blend (00C5) + wait
    }

    // [DIFFERENTIAL] 21FD:037D
    // [DIFFERENTIAL] 21FD:03B4
    // [DIFFERENTIAL] 21AF:04CD
    [GameDataFact]
    public void FadeOut_21FD_037D_and_03B4_match_original()
    {
        var h = new LayerHarness(160);
        for (int i = 0; i < 4; i++)
        {
            ushort color = RandomColor(h);
            h.Randomize(GraphicsLayers.BasePalette, 768);
            h.Randomize(GraphicsLayers.ObjectPointerPairs, 0x40);
            h.Randomize(GraphicsLayers.BlendColor, 2);
            if (i % 2 == 0)
            {
                h.Call(Palettes, 0x037D, color);
                h.Port.FadeOut(color);
            }
            else
            {
                h.Call(Palettes, 0x03B4, color);
                h.Port.FadeOutFast(color);
            }
            h.AssertStateEqual($"fade out #{i} to {color:X4}");
        }
    }

    // [DIFFERENTIAL] 21AF:04A2
    [GameDataFact]
    public void ClearObjectPointerPair_21AF_04A2_matches_original()
    {
        var h = new LayerHarness(170);
        for (int i = 0; i < 20; i++)
        {
            h.Randomize(GraphicsLayers.ObjectPointerPairs - 8, 0x50);
            ushort index = i < 8 ? (ushort)i : h.Next(0, 9);
            h.Call(Objects, 0x04A2, index);
            h.Port.ClearObjectPointerPair(index);
            h.AssertStateEqual($"04A2({index}) #{i}");
        }
    }

    /// <summary>
    /// Base all 0, target white (78h): the blend moves by 8·n and stops at 78h; from n = 16 the step is the byte 80h,
    /// which compares as negative, so the component stays at 80h instead of being clamped.
    /// </summary>
    [GameDataFact]
    public void Blend_stops_at_the_target_but_byte_step_80h_passes_unclamped()
    {
        var h = new LayerHarness(180);
        h.WriteBoth(GraphicsLayers.BasePalette, new byte[768]);
        foreach (ushort steps in new ushort[] { 0, 1, 7, 8, 15, 16 })
        {
            h.Call(Palettes, 0x03EB, 0x0FFF, steps);
            h.Port.Blend(0x0FFF, steps);
            h.AssertStateEqual($"03EB(0FFF,{steps})");
            Assert.Equal(steps == 16 ? (byte)0x80 : (byte)Math.Min(steps * 8, 120), h.Port.Ds[GraphicsLayers.WorkPalette]);
        }
    }
}
