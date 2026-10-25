namespace Traders.Vga;

/// <summary>
/// Port of the VGA part of the assembler graphics driver in segment 23CD of T.EXE
/// (installed by 23CD:3042 for display mode 1). Each method mirrors one driver routine
/// instruction by instruction, including 16-bit wrap-around and the counting quirks of the
/// original loops (see docs/analyse/GRAFIK.md and docs/routines/23CD_*.md).
///
/// Addressing: the draw page is a VRAM segment (CS:0002); a pixel (x, y) lives in plane x&amp;3 at
/// page offset y·80 + x/4. The RAM atlas and the mask atlas are addressed from offset 0 of
/// their buffers (the original far pointers' offsets do not change any result as long as no
/// 64 KB wrap occurs inside one row run).
/// </summary>
public sealed class ModeXDriver
{
    private static readonly byte[] StartSkip = [0x00, 0x01, 0x03, 0x07];   // 23CD:315A
    private static readonly byte[] ExtraByte = [0x00, 0x11, 0x33, 0x77];   // 23CD:315E

    public ModeXMemory Vram { get; } = new();
    public PlanarAtlas Atlas { get; set; } = new();
    /// <summary>Mask atlas (MASKS.PP, 40 bytes per row, MSB = left pixel); CS:057E.</summary>
    public byte[] Masks { get; set; } = new byte[8000];
    /// <summary>Font table; CS:057A.</summary>
    public IReadOnlyList<FontDescriptor> Fonts { get; set; } = [];

    /// <summary>Draw page segment (CS:0002, set by 23CD:311A); A000h = first page.</summary>
    public ushort PageSegment { get; set; } = 0xA000;
    /// <summary>Draw page offset (CS:0000); stored by 23CD:311A but never used for addressing.</summary>
    public ushort PageOffset { get; set; }
    /// <summary>Current colour (CS:0588, 23CD:05C3); drawing uses the low byte.</summary>
    public ushort Color { get; set; }
    /// <summary>Current font number (CS:0586, 23CD:05B6).</summary>
    public ushort Font { get; set; }
    /// <summary>Line pattern (CS:04F4, 23CD:05D0); FFFFh = solid.</summary>
    public ushort LinePattern { get; set; } = 0xFFFF;
    /// <summary>CRTC start address (registers 0C/0D) as last set by 23CD:30E9.</summary>
    public ushort DisplayStart { get; private set; }

    private int PageBase => (PageSegment - 0xA000) << 4;
    private int Page(ushort di) => PageBase + di;

    // ------------------------------------------------------------------ helpers

    /// <summary>23CD:3162: offset of (x, y) in the draw page.</summary>
    private static ushort PageAddr(ushort x, ushort y) => (ushort)(y * 80 + (x >> 2));

    /// <summary>23CD:3181: atlas address of (x, y) as paragraph + offset (header skipped).</summary>
    private static (int Para, ushort Off) AtlasAddr(ushort x, ushort y) =>
        ((ushort)(y * 20), (ushort)((x >> 2) + PlanarAtlas.HeaderSize));

    private byte AtlasRead(int para, ushort off) => Atlas.Data[para * 16 + off];
    private void AtlasWrite(int para, ushort off, byte v) => Atlas.Data[para * 16 + off] = v;

    /// <summary>23CD:054B: mask atlas offset of (x, y); the low three bits of x are dropped.</summary>
    private static ushort MaskAddr(ushort x, ushort y) => (ushort)(y * 40 + (x >> 3));

    private ushort MaskWord(ushort off) => (ushort)(Masks[off] << 8 | Masks[(ushort)(off + 1)]);

    /// <summary>23CD:312D: per-plane start skip (bh) and extra-byte (bl) bits, whole bytes (cx).</summary>
    private static (byte Bh, byte Bl, ushort Cx) Align(ushort x, ushort width)
    {
        int xa = x & 3;
        byte bh = StartSkip[xa];
        byte al = ExtraByte[width & 3];
        al = (byte)((al << xa) | (al >> (8 - xa)));
        return (bh, (byte)(al & 0x0F), (ushort)(width >> 2));
    }

    private static byte Ror8(byte v, out bool carry)
    {
        carry = (v & 1) != 0;
        return (byte)((v >> 1) | (v << 7));
    }

    /// <summary>23CD:30D1: reset graphics controller (read map 0, write mode 0 defaults).</summary>
    public void ResetGraphicsController() => Vram.ReadMap = 0;

    // ------------------------------------------------------------------ page / state

    /// <summary>23CD:311A: select the page drawn to.</summary>
    public void SetDrawPage(ushort offset, ushort segment)
    {
        PageSegment = segment;
        PageOffset = offset;
    }

    /// <summary>23CD:30E9: CRTC start address = (segment − A000h)·16 + offset.</summary>
    public void SetDisplayStart(ushort offset, ushort segment) =>
        DisplayStart = (ushort)(((segment - 0xA000) << 4) + offset);

    // ------------------------------------------------------------------ primitives

    /// <summary>23CD:31A6 (vector 10): filled rectangle in the current colour.</summary>
    public void FillRect(ushort x, ushort y, ushort w, ushort h)
    {
        ushort di = PageAddr(x, y);
        var (bh, bl, cx) = Align(x, w);
        byte color = (byte)Color;
        for (byte ah = 1; ah != 0x10; ah <<= 1)
        {
            ushort d = di, c = cx;
            Vram.MapMask = ah;
            if ((bh & 1) != 0) d++;
            bh >>= 1;
            if ((bl & 1) != 0) c++;
            bl >>= 1;
            ushort rows = h;
            do
            {
                for (ushort i = 0; i < c; i++) Vram.Write(Page((ushort)(d + i)), color);
                d = (ushort)(d + 80);
            } while (--rows != 0);
        }
        Vram.MapMask = 0x0F;
    }

    /// <summary>23CD:32D8 (vector 14): copy an atlas rectangle to the draw page.</summary>
    public void BlitAtlasToPage(ushort sx, ushort sy, ushort w, ushort h, ushort dx, ushort dy)
    {
        var (para, si) = AtlasAddr(sx, sy);
        ushort di = PageAddr(dx, dy);
        var (bh, bl, cx) = Align(dx, w);
        byte k = 0;
        for (byte ah = 1; ah != 0x10; ah <<= 1, k++)
        {
            ushort c = cx, d = di, s = si;
            bl = Ror8(bl, out bool cf);
            if (cf) c++;
            bh = Ror8(bh, out cf);
            if (cf) d++;
            byte dl = (byte)(((k + 4 - (dx & 3)) & 3) + (sx & 3));
            if ((dl & 4) != 0) s++;
            dl &= 3;
            s = (ushort)(s + dl * 16);
            s = (ushort)(s + dl * 64);
            Vram.MapMask = ah;
            ushort rows = h;
            do
            {
                for (ushort i = 0; i < c; i++) Vram.Write(Page((ushort)(d + i)), AtlasRead(para, (ushort)(s + i)));
                s = (ushort)(s + 320);
                d = (ushort)(d + 80);
            } while (--rows != 0);
        }
        Vram.MapMask = 0x0F;
    }

    /// <summary>23CD:3A9A (vector 20): <see cref="BlitAtlasToPage"/> with width = units·8.</summary>
    public void BlitAtlasToPageFixed(ushort sx, ushort sy, ushort units, ushort h, ushort dx, ushort dy) =>
        BlitAtlasToPage(sx, sy, (ushort)(units << 3), h, dx, dy);

    /// <summary>23CD:3386 (vector 38): OR an atlas rectangle onto the draw page.</summary>
    /// <remarks>The inner loop uses LOOP: a plane with zero bytes runs 65536 times (as the original).</remarks>
    public void OrAtlasToPage(ushort sx, ushort sy, ushort w, ushort h, ushort dx, ushort dy)
    {
        var (para, si) = AtlasAddr(sx, sy);
        ushort di = PageAddr(dx, dy);
        var (bh, bl, cx) = Align(dx, w);
        byte k = 0;
        for (byte ah = 1; ah != 0x10; ah <<= 1, k++)
        {
            ushort c = cx, d = di, s = si;
            bl = Ror8(bl, out bool cf);
            if (cf) c++;
            bh = Ror8(bh, out cf);
            if (cf) d++;
            byte dl = (byte)(((k + 4 - (dx & 3)) & 3) + (sx & 3));
            if ((dl & 4) != 0) s++;
            dl &= 3;
            s = (ushort)(s + dl * 16);
            s = (ushort)(s + dl * 64);
            Vram.MapMask = ah;
            Vram.ReadMap = k;
            ushort rows = h;
            do
            {
                ushort n = c;
                do
                {
                    byte v = (byte)(Vram.Read(Page(d)) | AtlasRead(para, s));
                    Vram.Write(Page(d), v);
                    s++;
                    d++;
                } while (--n != 0);
                s = (ushort)(s - c + 320);
                d = (ushort)(d - c + 80);
            } while (--rows != 0);
        }
        Vram.MapMask = 0x0F;
    }

    /// <summary>23CD:3449 (vector 3C): copy a rectangle inside the RAM atlas.</summary>
    public void CopyAtlasToAtlas(ushort sx, ushort sy, ushort w, ushort h, ushort dx, ushort dy)
    {
        var (dPara, dOff) = AtlasAddr(dx, dy);
        byte dp = (byte)(dx & 3);
        var (sPara, sOff) = AtlasAddr(sx, sy);
        byte sp = (byte)(sx & 3);
        ushort whole = (ushort)(w >> 2);
        byte rest = (byte)(w & 3);
        for (int iter = 0; iter < 4; iter++)
        {
            ushort s = (ushort)(sOff + sp * 80), d = (ushort)(dOff + dp * 80);
            ushort n = whole;
            if (rest != 0) { n++; rest--; }
            ushort rows = h;
            do
            {
                for (ushort i = 0; i < n; i++) AtlasWrite(dPara, (ushort)(d + i), AtlasRead(sPara, (ushort)(s + i)));
                s = (ushort)(s + 320);
                d = (ushort)(d + 320);
            } while (--rows != 0);
            dp = (byte)((dp + 1) & 3);
            if (dp == 0) dOff++;
            sp = (byte)((sp + 1) & 3);
            if (sp == 0) sOff++;
        }
    }

    /// <summary>23CD:351E (vector 40): copy a rectangle inside the draw page, plane by plane.</summary>
    public void CopyPageToPage(ushort sx, ushort sy, ushort w, ushort h, ushort dx, ushort dy)
    {
        ushort dOff = PageAddr(dx, dy);
        byte dp = (byte)(dx & 3);
        ushort sOff = PageAddr(sx, sy);
        byte sp = (byte)(sx & 3);
        ushort whole = (ushort)(w >> 2);
        byte rest = (byte)(w & 3);
        for (int iter = 0; iter < 4; iter++)
        {
            Vram.ReadMap = sp;
            Vram.MapMask = (byte)(1 << dp);
            ushort s = sOff, d = dOff;
            ushort n = whole;
            if (rest != 0) { n++; rest--; }
            ushort rows = h;
            do
            {
                for (ushort i = 0; i < n; i++) Vram.Write(Page((ushort)(d + i)), Vram.Read(Page((ushort)(s + i))));
                s = (ushort)(s + 80);
                d = (ushort)(d + 80);
            } while (--rows != 0);
            dp = (byte)((dp + 1) & 3);
            if (dp == 0) dOff++;
            sp = (byte)((sp + 1) & 3);
            if (sp == 0) sOff++;
        }
        Vram.MapMask = 0x0F;
    }

    /// <summary>
    /// 23CD:35DF (vector 2C): atlas rectangle to the draw page where the mask atlas has a set bit
    /// (mask origin (mx, my), x rounded down to a multiple of 8).
    /// </summary>
    /// <remarks>A plane with zero bytes (width &lt; 4) runs 65536 bytes per row, as in the original.</remarks>
    public void MaskedBlit(ushort sx, ushort sy, ushort w, ushort h, ushort dx, ushort dy, ushort mx, ushort my)
    {
        ushort dOff = PageAddr(dx, dy);
        byte dp = (byte)(dx & 3);
        var (para, sOff) = AtlasAddr(sx, sy);
        byte sp = (byte)(sx & 3);
        ushort mOff = MaskAddr(mx, my);
        ushort whole = (ushort)(w >> 2);
        byte rest = (byte)(w & 3);
        for (int iter = 0; iter < 4; iter++)
        {
            ushort bx = mOff, d = dOff;
            ushort s = (ushort)(sOff + sp * 80);
            Vram.MapMask = (byte)(1 << dp);
            ushort n = whole;
            if (rest != 0) { n++; rest--; }
            int cl = iter;
            ushort rows = h;
            do
            {
                ushort cnt = n;
                bool done = false;
                while (!done)
                {
                    ushort ax = (ushort)(MaskWord(bx) << cl);
                    bx += 2;
                    foreach (ushort bit in MaskBits)
                    {
                        if ((ax & bit) != 0) Vram.Write(Page(d), AtlasRead(para, s));
                        s++;
                        d++;
                        if (--cnt == 0) { done = true; break; }
                    }
                }
                s = (ushort)(s - n);
                d = (ushort)(d - n);
                bx = (ushort)(bx - (((ushort)(n + 3) >> 2) << 1));
                s = (ushort)(s + 320);
                d = (ushort)(d + 80);
                bx = (ushort)(bx + 40);
            } while (--rows != 0);
            dp = (byte)((dp + 1) & 3);
            if (dp == 0) dOff++;
            sp = (byte)((sp + 1) & 3);
            if (sp == 0) sOff++;
        }
        Vram.MapMask = 0x0F;
    }

    private static readonly ushort[] MaskBits = [0x8000, 0x0800, 0x0080, 0x0008];

    /// <summary>23CD:3CE8 (vector 48): fill the mask shape at (mx, my) with a colour on the draw page.</summary>
    /// <remarks>A plane with zero bytes (width &lt; 4) runs 65536 bytes per row, as in the original.</remarks>
    public void MaskFill(ushort mx, ushort my, ushort w, ushort h, ushort dx, ushort dy, ushort color)
    {
        ushort dOff = PageAddr(dx, dy);
        byte dp = (byte)(dx & 3);
        ushort mOff = MaskAddr(mx, my);
        ushort whole = (ushort)(w >> 2);
        byte rest = (byte)(w & 3);
        byte bh = (byte)color;
        for (int iter = 0; iter < 4; iter++)
        {
            ushort si = mOff, d = dOff;
            Vram.MapMask = (byte)(1 << dp);
            ushort n = whole;
            if (rest != 0) { n++; rest--; }
            int cl = iter;
            ushort rows = h;
            do
            {
                ushort cnt = n;
                bool done = false;
                while (!done)
                {
                    ushort ax = (ushort)(MaskWord(si) << cl);
                    si += 2;
                    foreach (ushort bit in MaskBits)
                    {
                        if ((ax & bit) != 0) Vram.Write(Page(d), bh);
                        d++;
                        if (--cnt == 0) { done = true; break; }
                    }
                }
                d = (ushort)(d - n);
                si = (ushort)(si - (((ushort)(n + 3) >> 2) << 1));
                d = (ushort)(d + 80);
                si = (ushort)(si + 40);
            } while (--rows != 0);
            dp = (byte)((dp + 1) & 3);
            if (dp == 0) dOff++;
        }
        Vram.MapMask = 0x0F;
    }

    /// <summary>23CD:373B (vector 30): set one pixel. Leaves the map mask at FFh.</summary>
    public void SetPixel(ushort x, ushort y, ushort color)
    {
        ushort di = PageAddr(x, y);
        Vram.MapMask = (byte)(1 << (x & 3));
        Vram.Write(Page(di), (byte)color);
        Vram.MapMask = 0xFF;
    }

    /// <summary>23CD:3764 (vector 34): read one pixel of the draw page.</summary>
    public byte GetPixel(ushort x, ushort y)
    {
        ushort di = PageAddr(x, y);
        Vram.ReadMap = (byte)(x & 3);
        byte v = Vram.Read(Page(di));
        ResetGraphicsController();
        return v;
    }

    /// <summary>
    /// 23CD:328E (vector 18): latch copy of w bytes × h rows between two VRAM addresses
    /// (all planes enabled in the map mask at once, i.e. 4 pixels per byte).
    /// </summary>
    public void LatchCopy(ushort srcOff, ushort srcSeg, ushort dstOff, ushort dstSeg, ushort w, ushort h)
    {
        int srcBase = (srcSeg - 0xA000) << 4, dstBase = (dstSeg - 0xA000) << 4;
        ushort si = srcOff, di = dstOff, skip = (ushort)(80 - w);
        ushort rows = h;
        do
        {
            for (ushort i = 0; i < w; i++)
            {
                Vram.Read(srcBase + si++);
                Vram.WriteLatches(dstBase + di++);
            }
            si = (ushort)(si + skip);
            di = (ushort)(di + skip);
        } while (--rows != 0);
        ResetGraphicsController();
    }

    /// <summary>
    /// 23CD:31FD (vector 0C): draw one 16-pixel-wide glyph with its top row at (x, yTop) in the
    /// current colour. <paramref name="height"/> is used as a byte (0 = 256 rows).
    /// </summary>
    public void DrawGlyph(ushort x, ushort yTop, byte[] glyphs, int glyphOffset, byte height)
    {
        ushort di = PageAddr(x, yTop);
        byte bh = StartSkip[x & 3];
        int cl = (4 - (x & 3)) & 3;
        byte color = (byte)Color;
        for (byte ah = 1; ah != 0x10; ah <<= 1)
        {
            ushort d = di;
            int si = glyphOffset;
            Vram.MapMask = ah;
            bh = Ror8(bh, out bool cf);
            if (cf) d++;
            byte rows = height;
            do
            {
                ushort dx = (ushort)(glyphs[si] << 8 | glyphs[si + 1]);
                dx = (ushort)((dx << cl) | (dx >> (16 - cl)));
                if ((dx & 0x8000) != 0) Vram.Write(Page(d), color);
                d++;
                if ((dx & 0x0800) != 0) Vram.Write(Page(d), color);
                d++;
                if ((dx & 0x0080) != 0) Vram.Write(Page(d), color);
                d++;
                if ((dx & 0x0008) != 0) Vram.Write(Page(d), color);
                d = (ushort)(d + 77);
                si += 2;
            } while (--rows != 0);
            cl = (cl + 1) & 3;
        }
        Vram.MapMask = 0x0F;
    }

    /// <summary>
    /// 23CD:05DD: draw a zero-terminated string with the current font; y is the bottom row of the
    /// glyphs. Characters are masked to 7 bits; x advances by the font's advance per character.
    /// </summary>
    public void DrawText(ushort x, ushort y, ReadOnlySpan<byte> text)
    {
        var font = Fonts[Font];
        ushort bx = x;
        ushort top = (ushort)(y - font.Height + 1);
        foreach (byte raw in text)
        {
            int c = raw & 0x7F;
            if (c == 0) break;
            ushort index = (ushort)((ushort)(c - font.FirstChar) * 2);
            uint offset = (uint)index * font.Height;
            DrawGlyph(bx, top, font.Data, font.Offset + (int)offset, (byte)font.Height);
            bx = (ushort)(bx + font.Advance);
        }
        ResetGraphicsController();
    }
}
