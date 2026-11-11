namespace Traders.Vga.Layers;

/// <summary>Page management 0B67 and page flip 21FD:051F.</summary>
public sealed partial class GraphicsLayers
{
    /// <summary>DS:0238 – page shown: 0 = page 0, otherwise page 1.</summary>
    public const int ShownPage = 0x0238;

    /// <summary>DS:023A – 0 = draw on the page shown, otherwise on the hidden page.</summary>
    public const int DrawHidden = 0x023A;

    /// <summary>DS:2BA0/2BA2 – far address of page 0; DS:2BA4/2BA6 – page 1 (VGA: A000:0000, A400:0000).</summary>
    public const int Page0 = 0x2BA0, Page1 = 0x2BA4;

    /// <summary>DS:2BA8/2BAA – Tandy buffer (mode 63h only); DS:2BAC/2BAE – buffer allocated by 0B67:0002.</summary>
    public const int TandyBuffer = 0x2BA8, PageBuffer = 0x2BAC;

    /// <summary>DS:082E – page index drawn to by the game (0/1); DS:0830 – page index shown (its complement).</summary>
    public const int DrawPageIndex = 0x082E, ShownPageIndex = 0x0830;

    /// <summary>DS:40DE – two page numbers (long, 1 or 2 for 0B67:0238), indexed by 082E/0830.</summary>
    public const int PageNumbers = 0x40DE;

    /// <summary>DS:4516/4518 – page number drawn to after the last flip (copy of the entry for 082E).</summary>
    public const int CurrentDrawPage = 0x4516;

    /// <summary>DS:3D95 – non-zero: 21FD:051F does not wait for the retrace.</summary>
    public const int NoFlipWait = 0x3D95;

    /// <summary>DS:3D91 – non-zero: 21FD:051F waits four extra retraces.</summary>
    public const int SlowFlip = 0x3D91;

    /// <summary>
    /// [ORIGINAL] 0B67:0002(): set up the pages for the card in DS:090E. VGA path (0B67:007F): driver part
    /// 23CD:04F6(1), pages A000:0000 and A400:0000, a 500h-byte buffer, then both pages cleared to colour 0 (first the
    /// page chosen by 01D4, then by 0206). Returns 0, or 1 if the buffer could not be allocated.
    /// The allocation (186C:02C6, C runtime heap) is the caller's: <paramref name="allocatedBuffer"/> is the far
    /// pointer it returned (0 = failure).
    /// </summary>
    public ushort InitializePages(uint allocatedBuffer)
    {
        ushort mode = U16(CardMode);
        if (mode != 1)
            throw new NotSupportedException($"Display mode {mode}: only the VGA path of 0B67:0002 is ported.");
        Driver.SelectCard(mode);
        Set16(Page0, 0x0000);
        Set16(Page0 + 2, 0xA000);
        Set16(Page1, 0x0000);
        Set16(Page1 + 2, 0xA400);
        Set16(PageBuffer, (ushort)allocatedBuffer);
        Set16(PageBuffer + 2, (ushort)(allocatedBuffer >> 16));
        if (allocatedBuffer == 0) return 1;
        DrawOnShownPage();
        Driver.Color = 0;
        Driver.FillRect(0, 0, 320, 200);
        DrawOnHiddenPage();
        Driver.FillRect(0, 0, 320, 200);
        return 0;
    }

    /// <summary>[ORIGINAL] 0B67:016B: show the other page, then keep drawing on the shown or hidden page as before.</summary>
    public void FlipShownPage()
    {
        if (U16(ShownPage) == 0) ShowPage1(); else ShowPage0();
        if (U16(DrawHidden) != 0) DrawOnHiddenPage(); else DrawOnShownPage();
    }

    /// <summary>[ORIGINAL] 0B67:0196: show page 0 (CRTC start), then wait for the retrace.</summary>
    public void ShowPage0()
    {
        Set16(ShownPage, 0);
        Driver.SetDisplayStart(U16(Page0), U16(Page0 + 2));
        WaitRetrace();
    }

    /// <summary>[ORIGINAL] 0B67:01B5: show page 1, then wait for the retrace.</summary>
    public void ShowPage1()
    {
        Set16(ShownPage, 1);
        Driver.SetDisplayStart(U16(Page1), U16(Page1 + 2));
        WaitRetrace();
    }

    /// <summary>[ORIGINAL] 0B67:01D4: draw on the page shown (DS:023A = 0).</summary>
    public void DrawOnShownPage()
    {
        Set16(DrawHidden, 0);
        int page = U16(ShownPage) == 0 ? Page0 : Page1;
        Driver.SetDrawPage(U16(page), U16(page + 2));
    }

    /// <summary>[ORIGINAL] 0B67:0206: draw on the hidden page (DS:023A = 1).</summary>
    public void DrawOnHiddenPage()
    {
        Set16(DrawHidden, 1);
        int page = U16(ShownPage) == 0 ? Page1 : Page0;
        Driver.SetDrawPage(U16(page), U16(page + 2));
    }

    /// <summary>
    /// [ORIGINAL] 0B67:0238(draw, show as longs): <paramref name="show"/> 1 → show page 0, 2 → show page 1, otherwise
    /// unchanged; then <paramref name="draw"/> 1 or 2 → draw on the shown page if it equals <paramref name="show"/>,
    /// else on the hidden page; other values leave the draw page.
    /// </summary>
    public void SelectPages(uint draw, uint show)
    {
        if (show == 1) ShowPage0();
        if (show == 2) ShowPage1();
        if (draw is 1 or 2)
        {
            if (draw == show) DrawOnShownPage(); else DrawOnHiddenPage();
        }
    }

    /// <summary>[ORIGINAL] 0B67:028F: wait for the retrace.</summary>
    public void WaitForRetrace() => WaitRetrace();

    /// <summary>
    /// [ORIGINAL] 0B67:0299(direction): copy one page to the other with the latch copy 23CD:06B9 (VGA path 0B67:0313:
    /// 3E80h bytes in one row = 16000 bytes · 4 planes); direction 1 copies page 0 to page 1, any other value back.
    /// </summary>
    public void CopyPage(ushort direction)
    {
        int src = direction == 1 ? Page0 : Page1, dst = direction == 1 ? Page1 : Page0;
        ushort mode = U16(CardMode);
        if (mode != 1)
        {
            if (mode is 0 or 2 or 0x63)
                throw new NotSupportedException($"Display mode {mode}: only the VGA path of 0B67:0299 is ported.");
            return;
        }
        Driver.LatchCopy(U16(src), U16(src + 2), U16(dst), U16(dst + 2), 0x3E80, 1);
    }

    /// <summary>
    /// [ORIGINAL] 21FD:051F: flip the game's pages: 082E ^= 1, 0830 = (082E == 0), 4516 = page number of 082E, then
    /// 0B67:0238(4516, page number of 0830); afterwards a retrace wait unless DS:3D95 is set, and four more if
    /// DS:3D91 is set.
    /// </summary>
    public void FlipPages()
    {
        ushort draw = (ushort)(U16(DrawPageIndex) ^ 1);
        Set16(DrawPageIndex, draw);
        ushort shown = (ushort)(draw == 0 ? 1 : 0);
        Set16(ShownPageIndex, shown);
        uint drawNumber = U32(PageNumbers + (ushort)(draw << 2));
        Set16(CurrentDrawPage, (ushort)drawNumber);
        Set16(CurrentDrawPage + 2, (ushort)(drawNumber >> 16));
        SelectPages(drawNumber, U32(PageNumbers + (ushort)(shown << 2)));
        if (Ds[NoFlipWait] == 0) WaitForRetrace();
        if (Ds[SlowFlip] != 0) WaitRetraces(4);
    }
}
