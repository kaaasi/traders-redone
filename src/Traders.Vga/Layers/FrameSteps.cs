namespace Traders.Vga.Layers;

/// <summary>
/// Frame-stepped forms of the presenting routines: the same operations in the same order as <see cref="FadeOutFast"/>,
/// <see cref="FadeIn()"/>, <see cref="FadeInFast"/> and <see cref="FlipPages"/>, but every retrace wait (0D73:00D3, counted
/// through <see cref="Retrace"/>) is followed by a yield. A scene runs such a sequence up to the next yield per frame, so
/// the work after each retrace lands in the frame after it, as in the original.
/// </summary>
public sealed partial class GraphicsLayers
{
    /// <summary>[ORIGINAL] 0D73:00D3 as a step: one retrace wait, then the frame ends.</summary>
    private IEnumerable<int> RetraceStep()
    {
        WaitRetrace();
        yield return 0;
    }

    private IEnumerable<int> RetraceSteps(int count)
    {
        for (int i = 0; i < count; i++)
            foreach (int s in RetraceStep()) yield return s;
    }

    /// <summary>[ORIGINAL] 21FD:03EB(rgb12, steps) as steps: working palette, then on VGA 2295:00C5 (retrace wait, DAC).</summary>
    private IEnumerable<int> BlendSteps(ushort rgb12, ushort steps)
    {
        BlendWorkPalette(rgb12, steps);
        if (!IsVga) yield break;
        foreach (int s in RetraceStep()) yield return s;
        var palette = Ds.AsSpan(WorkPalette, 768);
        for (int i = 0; i < 256; i++) Dac.Write((byte)i, palette[i * 3], palette[i * 3 + 1], palette[i * 3 + 2]);
    }

    private IEnumerable<int> FadeInSteps(int wait)
    {
        for (short n = 7; n >= 0; n--)
        {
            foreach (int s in BlendSteps(U16(BlendColor), (ushort)n)) yield return s;
            foreach (int s in RetraceSteps(wait)) yield return s;
        }
    }

    /// <summary>[ORIGINAL] 21FD:0329 as steps (see <see cref="FadeIn()"/>).</summary>
    public IEnumerable<int> FadeInSteps() => FadeInSteps(3);

    /// <summary>[ORIGINAL] 21FD:0353 as steps (see <see cref="FadeInFast"/>).</summary>
    public IEnumerable<int> FadeInFastSteps() => FadeInSteps(1);

    /// <summary>[ORIGINAL] 21FD:03B4(rgb12) as steps (see <see cref="FadeOutFast"/>).</summary>
    public IEnumerable<int> FadeOutFastSteps(ushort rgb12)
    {
        for (ushort n = 0; n <= 7; n++)
        {
            foreach (int s in BlendSteps(rgb12, n)) yield return s;
            foreach (int s in RetraceSteps(2)) yield return s;
        }
        SetBlendColor(rgb12);
        InvalidateAllRestoreRecords();
    }

    /// <summary>
    /// [ORIGINAL] 21FD:051F as steps (see <see cref="FlipPages"/>): page indices, 0B67:0238 with the show-page retrace of
    /// 0B67:0196/01B5, then 0B67:028F unless DS:3D95 is set and four more retraces if DS:3D91 is set.
    /// </summary>
    public IEnumerable<int> FlipPagesSteps()
    {
        ushort draw = (ushort)(U16(DrawPageIndex) ^ 1);
        Set16(DrawPageIndex, draw);
        ushort shown = (ushort)(draw == 0 ? 1 : 0);
        Set16(ShownPageIndex, shown);
        uint drawNumber = U32(PageNumbers + (ushort)(draw << 2));
        Set16(CurrentDrawPage, (ushort)drawNumber);
        Set16(CurrentDrawPage + 2, (ushort)(drawNumber >> 16));
        uint showNumber = U32(PageNumbers + (ushort)(shown << 2));
        // 0B67:0238(draw, show): show first (with its retrace), then the draw page
        if (showNumber is 1 or 2)
        {
            int page = showNumber == 1 ? Page0 : Page1;
            Set16(ShownPage, (ushort)(showNumber - 1));
            Driver.SetDisplayStart(U16(page), U16(page + 2));
            foreach (int s in RetraceStep()) yield return s;
        }
        if (drawNumber is 1 or 2)
        {
            if (drawNumber == showNumber) DrawOnShownPage(); else DrawOnHiddenPage();
        }
        if (Ds[NoFlipWait] == 0)
            foreach (int s in RetraceStep()) yield return s;
        if (Ds[SlowFlip] != 0)
            foreach (int s in RetraceSteps(4)) yield return s;
    }
}
