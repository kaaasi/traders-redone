namespace Traders.Vga.Layers;

/// <summary>Palettes, fades and colour registers (21FD:026C…04DA, 2295:006D/00C5, 21AF:04A2/04CD).</summary>
public sealed partial class GraphicsLayers
{
    /// <summary>DS:3DCB – base palette (768 bytes, three components per colour).</summary>
    public const int BasePalette = 0x3DCB;

    /// <summary>DS:5017 – palette as loaded from the last image (768 bytes, filled by 186C:0002).</summary>
    public const int LoadedPalette = 0x5017;

    /// <summary>DS:54E8 – working palette: result of the last blend, sent to the DAC.</summary>
    public const int WorkPalette = 0x54E8;

    /// <summary>DS:10D8 – blend colour (12-bit RGB, one nibble per component) used by the fade-ins.</summary>
    public const int BlendColor = 0x10D8;

    /// <summary>DS:33E4 – restore records of the moving objects, two per object (page 0, page 1; 4 bytes each: page offset,
    /// x&amp;3, valid), cleared by 21AF:04A2/04CD.</summary>
    public const int ObjectRestoreRecords = 0x33E4;

    /// <summary>
    /// [ORIGINAL] 2295:006D(index, rgb12): one DAC register from a 12-bit colour, each nibble shifted left by three
    /// (bytes, so the result is 0…120 and the DAC keeps the low six bits). Only on VGA; no retrace wait.
    /// </summary>
    public void SetColorRegister(ushort index, ushort rgb12)
    {
        var (r, g, b) = Components(rgb12);
        if (IsVga) Dac.Write((byte)index, r, g, b);   // 27C6:042E with cx = 1, bx = index (port 3C8h gets bl)
    }

    /// <summary>
    /// [ORIGINAL] 2295:00C5(far palette): wait for the retrace (also on other cards), then on VGA all 256 DAC
    /// registers from the 768 bytes of <paramref name="palette"/>.
    /// </summary>
    public void SetAllColorRegisters(ReadOnlySpan<byte> palette)
    {
        WaitRetrace();
        if (!IsVga) return;
        for (int i = 0; i < 256; i++) Dac.Write((byte)i, palette[i * 3], palette[i * 3 + 1], palette[i * 3 + 2]);
    }

    /// <summary>[ORIGINAL] 21FD:026C: base palette ← loaded palette (768 bytes).</summary>
    public void CopyLoadedToBase() => Array.Copy(Ds, LoadedPalette, Ds, BasePalette, 768);

    /// <summary>
    /// [ORIGINAL] 21FD:02C9(index, rgb12): one colour of the base palette from a 12-bit colour (nibbles · 8 as
    /// bytes). The address 3DCBh + index·3 is formed in 16 bits (imul, no range check).
    /// </summary>
    public void SetBaseColor(ushort index, ushort rgb12)
    {
        var (r, g, b) = Components(rgb12);
        int at = (ushort)(index * 3);
        Ds[(BasePalette + at) & 0xFFFF] = r;
        Ds[(BasePalette + 1 + at) & 0xFFFF] = g;
        Ds[(BasePalette + 2 + at) & 0xFFFF] = b;
    }

    /// <summary>
    /// [ORIGINAL] 21FD:03EB(rgb12, steps): working palette = base palette moved towards the colour by
    /// <paramref name="steps"/>·8 per component, never past it (all comparisons signed on bytes, the step is
    /// formed as a byte); on VGA the working palette then goes to the DAC via 2295:00C5.
    /// </summary>
    public void Blend(ushort rgb12, ushort steps)
    {
        var (tr, tg, tb) = Components(rgb12);
        Span<byte> target = [tr, tg, tb];
        byte step = (byte)(steps << 3);
        for (int color = 0; color <= 0xFF; color++)
        {
            for (int k = 0; k <= 2; k++)
            {
                byte c = Ds[BasePalette + color * 3 + k];
                sbyte t = (sbyte)target[k];
                if ((sbyte)c > t)
                {
                    c = (byte)(c - step);
                    if ((sbyte)c < t) c = (byte)t;
                }
                else
                {
                    c = (byte)(c + step);
                    if ((sbyte)c > t) c = (byte)t;
                }
                Ds[WorkPalette + color * 3 + k] = c;
            }
        }
        if (IsVga) SetAllColorRegisters(Ds.AsSpan(WorkPalette, 768));
    }

    /// <summary>[ORIGINAL] 21FD:0329: fade in from the blend colour, steps 7…0, three retraces after each.</summary>
    public void FadeIn() => FadeIn(3);

    /// <summary>[ORIGINAL] 21FD:0353: like 0329 with one retrace per step.</summary>
    public void FadeInFast() => FadeIn(1);

    private void FadeIn(int wait)
    {
        for (short n = 7; n >= 0; n--)
        {
            Blend(U16(BlendColor), (ushort)n);
            WaitRetraces(wait);
        }
    }

    /// <summary>
    /// [ORIGINAL] 21FD:037D(rgb12): fade out to the colour, steps 0…7, four retraces after each; then the colour
    /// becomes the blend colour (04DA) and all restore records of the moving objects are cleared (21AF:04CD).
    /// </summary>
    public void FadeOut(ushort rgb12) => FadeOut(rgb12, 4);

    /// <summary>[ORIGINAL] 21FD:03B4(rgb12): like 037D with two retraces per step.</summary>
    public void FadeOutFast(ushort rgb12) => FadeOut(rgb12, 2);

    private void FadeOut(ushort rgb12, int wait)
    {
        for (ushort n = 0; n <= 7; n++)
        {
            Blend(rgb12, n);
            WaitRetraces(wait);
        }
        SetBlendColor(rgb12);
        InvalidateAllRestoreRecords();
    }

    /// <summary>[ORIGINAL] 21FD:04DA(rgb12): DS:10D8 = blend colour.</summary>
    public void SetBlendColor(ushort rgb12) => Set16(BlendColor, rgb12);

    /// <summary>[ORIGINAL] 21AF:04A2(i): both restore records of object <paramref name="i"/> at DS:33E4 + 8·i = 0.</summary>
    public void InvalidateRestoreRecords(ushort i)
    {
        int at = (ushort)(i << 3);
        for (int k = 0; k < 8; k++) Ds[(ObjectRestoreRecords + at + k) & 0xFFFF] = 0;
    }

    /// <summary>[ORIGINAL] 21AF:04CD: 04A2 for entries 0…7.</summary>
    public void InvalidateAllRestoreRecords()
    {
        for (ushort i = 0; i < 8; i++) InvalidateRestoreRecords(i);
    }

    /// <summary>
    /// 12-bit colour → three bytes as 21FD:02C9/03EB and 2295:006D form them: R = (rgb &gt;&gt; 8) as byte · 8,
    /// G = ((rgb &amp; F0h) / 16) · 8, B = (rgb &amp; 0Fh) · 8. Bits above the twelfth reach R through the byte shift.
    /// </summary>
    private static (byte R, byte G, byte B) Components(ushort rgb12) =>
        ((byte)((rgb12 >> 8) << 3), (byte)(((rgb12 & 0xF0) >> 4) << 3), (byte)((rgb12 & 0x0F) << 3));
}
