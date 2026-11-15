using Traders.Core.Startup;
using Traders.Core.State;
using Traders.Core.Stations;

namespace Traders.Core.Pixels;

/// <summary>
/// Font of the graphics driver as explicit input of the core (lane stationen): the glyph bytes of FONTS.PP as the driver
/// table holds them (16-pixel-wide glyphs, two bytes per row, high bit = leftmost pixel), the glyph height, the advance and
/// the first character. The host and the tests take it from Traders.Assets (the font chosen last by 21FD:05DB).
/// </summary>
public sealed record TextFont(byte[] Data, int Offset, int Height, int Advance, int FirstChar);

/// <summary>A read of the pixel reader 23CD:06DE through the rule page: word arguments and the colour returned.</summary>
public readonly record struct PixelRead(short X, short Y, byte Value);

/// <summary>
/// Rule page of the stations phase 1B03:0009 (docs/routines/1B03_0009_stationen.md, section "Regelseite"): plays the
/// output calls of <see cref="StationPhase"/> onto a <see cref="RulePage"/> as far as they change pixels the phase reads
/// back (the star field 1B03:1178, the rocket trail in 1B03:08ED and the hit test 1B03:0BE7 compare a pixel with 1).
/// The command list 0A96 is kept like the original's: entries are queued and drawn when the list is executed
/// (0A96:06A8, also inside 21FD:060D and 21FD:0665), 0A96:001B empties it. Queued and drawn: blits from the RAM atlas
/// (0A96:0030, 0311), pixels (03FB) and texts from DS without shadow (0525). Page flips 21FD:051F (also in 060D/0665), the
/// page copy 0A96:0C3F and the draw-page switches 0B67:01D4/0206 move the draw page.
/// Objects (21AF, 23CD:06E9/071C) are drawn and restored like the driver does it, because a restore record cleared by
/// 21AF:0412 leaves the object's picture on a page (the explosion of a hit rocket stays where a new one starts, and a later
/// read sees it). Not drawn: the panel texts 0A69 (shadow texts, numbers, typed lines) and palettes.
/// </summary>
// [APPROX] the panel texts 0A69:004D/011B/0243/0288 are not drawn: they lie in the message panel 70h…CFh × AFh…C7h, whose
// pixels in V-RAKETE are never 1, in colours other than 1 (player colours DS:0D00, shadow 0); a read there is ≠ 1 with or
// without them, but its exact value can differ from the VRAM
public sealed class StationsPage
{
    /// <summary>Object records DS:330C (1Ah bytes): +0 active, +2 height, +14h x, +16h y, +18h frame.</summary>
    private const int Objects = 0x330C, ObjectSize = 0x1A, ObjectCount = 8, ObjectWidth = 16;

    private readonly IndexedImage _atlas;
    private readonly byte[] _masks;
    private readonly TextFont _font;
    private readonly TextFile _texts;
    private readonly Dgroup _d;
    private readonly Dictionary<(int Object, int Frame), (int X, int Y, int Mx, int My)> _images = [];
    private readonly ObjectSlot?[,] _restore = new ObjectSlot?[ObjectCount, 2];
    private readonly List<Action> _list = [];
    private readonly List<PixelRead> _reads = [];
    private int? _hidden;

    /// <param name="page">Rule page; its draw page must be the original's when the phase starts.</param>
    /// <param name="atlas">RAM atlas: the picture DS:08D0 (V-RAKETE) the phase loads with 186C:0002.</param>
    /// <param name="masks">Mask atlas: MASKS.PP decrunched (40 bytes per row, high bit = leftmost pixel).</param>
    /// <param name="font">Current font of the driver.</param>
    /// <param name="texts">Text buffer at its far address (DS:4884) as the program start leaves it, for the typed texts.</param>
    /// <param name="state">
    /// Data segment of the phase (object records DS:330C, texts DS:0E3E/0E40, page numbers DS:40DE). At the start no object
    /// has a valid restore record and objects 5…7 are inactive (as the original's data segment shows at 1B03:0009).
    /// </param>
    public StationsPage(RulePage page, IndexedImage atlas, byte[] masks, TextFont font, TextFile texts, Dgroup state)
    {
        _texts = texts;
        Page = page;
        _atlas = atlas;
        _masks = masks;
        _font = font;
        _d = state;
        DrawIndex = state.S16(RulePage.DrawPageIndex);
        ShownIndex = state.S16(ShownPageIndex);
    }

    private const int ShownPageIndex = 0x0830;

    /// <summary>
    /// DS:082E as the original's 21FD:051F keeps it: the index of the page being drawn, flipped with every page flip of the
    /// phase (also inside 21FD:060D, 0665 and the typed texts). The core's data segment leaves it to the output layers; the
    /// phase passes this value where the original pushes [082E] (23CD:06E9/071C, 0A96:0C3F).
    /// </summary>
    public short DrawIndex { get; private set; }

    /// <summary>DS:0830 as 21FD:051F keeps it: 1 when the draw index is 0, else 0 (the start value from the data segment).</summary>
    public short ShownIndex { get; private set; }

    // [ORIGINAL] 21FD:051F – 082E ^= 1, 0830 = (082E == 0), draw page = page of 40DE[082E]
    private void Flip()
    {
        Page.Flip();
        DrawIndex ^= 1;
        ShownIndex = (short)(DrawIndex == 0 ? 1 : 0);
    }

    public RulePage Page { get; }

    /// <summary>Every read of 23CD:06DE so far, in order.</summary>
    public IReadOnlyList<PixelRead> Reads => _reads;

    /// <summary>23CD:06DE(x, y) – the pixel of the draw page as the original reads it (<see cref="RulePage.Get(int, int)"/>).</summary>
    // [ORIGINAL] 23CD:06DE – through the rule page (R6P-1)
    public byte Read(short x, short y)
    {
        byte v = Page.Get(x, y);
        _reads.Add(new PixelRead(x, y, v));
        return v;
    }

    /// <summary>Plays one output call of the phase onto the rule page.</summary>
    public void Apply(StationEvent e)
    {
        var a = e.Args;
        switch (e.Call)
        {
            case StationCall.ClearList:
                _list.Clear();
                break;
            case StationCall.Blit:
                _list.Add(() => Page.Blit(_atlas, a[0], a[1], a[2], a[3], a[4], a[5]));
                break;
            case StationCall.Blit8:
                // [ORIGINAL] 0A96:0311 – type 7, executed through 0A96:094F as 23CD:3A9A: the blit 0030 with width 8
                _list.Add(() => Page.Blit(_atlas, a[0], a[1], 8, a[2], a[3], a[4]));
                break;
            case StationCall.Plot:
                _list.Add(() => Page.SetPixel(a[0], a[1], (byte)a[2]));
                break;
            case StationCall.Text:
                if (a[5] >= 0)
                    throw new NotSupportedException("Text with shadow: 1B03 draws its texts from DS without shadow.");
                _list.Add(() => DrawText(a[0], a[1], (ushort)a[2], (byte)a[4]));
                break;
            case StationCall.RunList:
                RunList();
                break;
            case StationCall.ListOnBothPages:
                // [ORIGINAL] 21FD:060D – 0A96:06A8, 21FD:051F, 0A96:06A8
                RunList();
                Flip();
                RunList();
                break;
            case StationCall.Flip:
                Flip();
                break;
            case StationCall.LastFrame:
                // [ORIGINAL] 21FD:0665 – 21FD:051F, 23CD:071C, 0A96:06A8, 23CD:06E9
                Flip();
                RestoreObjects();
                RunList();
                DrawObjects();
                break;
            case StationCall.TypedText:
                TypedText((ushort)a[3], (ushort)a[4]);
                break;
            case StationCall.TypedLines:
                // [ORIGINAL] 0A69:0288 – 0243 for the count far pointers of the table, count a signed word
                for (int i = 0; i < a[5]; i++)
                {
                    int entry = Int16Math.U(a[3] + 4 * i);
                    TypedText(_d.U16(entry), _d.U16(Int16Math.U(entry + 2)));
                }
                break;
            case StationCall.ObjectImage:
                _images[(a[0], a[1])] = (a[2], a[3], a[4], a[5]);
                break;
            case StationCall.ShowObject:
                // [ORIGINAL] 21AF:0412 – restore records of both pages cleared
                _restore[a[0], 0] = _restore[a[0], 1] = null;
                break;
            case StationCall.DrawObjects:
                DrawObjects();
                break;
            case StationCall.RestoreObjects:
                RestoreObjects();
                break;
            case StationCall.CopyPage:
                CopyPage(a[0]);
                break;
            case StationCall.DrawOnShownPage:
                _hidden ??= Page.DrawPage;
                Page.SelectDrawPage(1 - _hidden.Value);
                break;
            case StationCall.DrawOnHiddenPage:
                if (_hidden is int h) Page.SelectDrawPage(h);
                _hidden = null;
                break;
        }
    }

    // [ORIGINAL] 0A96:06A8 – the entries in order; the list stays (0A96:001B empties it)
    private void RunList()
    {
        foreach (var step in _list) step();
    }

    /// <summary>
    /// 0A69:0243 → 01AD – typed text: per character the list emptied, the character (0A96:0525 of DS:0140) queued and
    /// 21FD:060D (list, flip, list). The characters lie in the message panel and are not drawn (see the class remarks); the
    /// flips move the draw page once per character, the list stays empty of drawn entries.
    /// </summary>
    // [ORIGINAL] 0A69:0243, 0A69:01AD – strlen(text) passes with 21FD:060D; nothing for a null pointer
    private void TypedText(ushort offset, ushort segment)
    {
        if (offset == 0 && segment == 0) return;
        int length = TextLength(offset, segment);
        for (int i = 0; i < length; i++)
        {
            _list.Clear();
            Flip();
        }
    }

    /// <summary>strlen of a text in the text buffer DS:4884 (far pointer <paramref name="segment"/>:<paramref name="offset"/>).</summary>
    // strlen of the C runtime (0000:42E5), as called by 0A69:01AD
    public int TextLength(ushort offset, ushort segment)
    {
        int at = (segment << 4) + offset - ((_texts.BufferSegment << 4) + _texts.BufferOffset);
        var span = _texts.Contents.Span;
        if (at < 0 || at >= span.Length)
            throw new NotSupportedException($"Typed text at {segment:X4}:{offset:X4} lies outside the text buffer.");
        int length = span[at..].IndexOf((byte)0);
        if (length < 0) throw new NotSupportedException($"Typed text at {segment:X4}:{offset:X4} has no end in the text buffer.");
        return length;
    }

    /// <summary>Saved background of an object on one page: where it was drawn and the 16 × height pixels under it.</summary>
    private sealed record ObjectSlot(int X, int Y, byte[] Pixels);

    /// <summary>
    /// 23CD:06E9 – every active object 0…7: the 16 × height pixels at (x, y) saved for this page (restore record valid),
    /// then the frame's image from the RAM atlas where its mask bit is set. The restore records are kept per physical page
    /// (the original keeps them per page index DS:082E, which belongs to one physical page for the whole game).
    /// </summary>
    // [ORIGINAL] 23CD:06E9, 23CD:3AAB – background 4 bytes per plane and row from plane x & 3, image through the mask word
    private void DrawObjects()
    {
        for (int i = 0; i < ObjectCount; i++)
        {
            int at = Objects + i * ObjectSize;
            if (_d.U8(at) == 0) continue;
            int h = _d.U16(at + 2), x = _d.S16(at + 0x14), y = _d.S16(at + 0x16), frame = _d.U16(at + 0x18);
            if (!_images.TryGetValue((i, frame), out var image))
                throw new NotSupportedException($"Object {i} frame {frame}: no image set by 21AF:00CA in this phase.");
            var saved = new byte[ObjectWidth * h];
            for (int r = 0; r < h; r++)
                for (int c = 0; c < ObjectWidth; c++)
                    saved[r * ObjectWidth + c] = Page.Get(x + c, y + r);
            _restore[i, Page.DrawPage] = new ObjectSlot(x, y, saved);
            // 21AF:00CA: image at atlas y·320 + x/4 (planar rows), mask word at 2·(my·20 + mx/16), 40 bytes per row
            int ix = image.X & ~3, mask = 2 * (image.My * 20 + image.Mx / 16);
            for (int r = 0; r < h; r++)
            {
                int bits = _masks[mask + 40 * r] << 8 | _masks[mask + 40 * r + 1];
                for (int c = 0; c < ObjectWidth; c++)
                    if ((bits & (0x8000 >> c)) != 0) Plot(x + c, y + r, _atlas[ix + c, image.Y + r]);
            }
        }
    }

    /// <summary>23CD:071C – objects 7…0, active or not: a valid restore record of this page writes the saved pixels back and is cleared.</summary>
    // [ORIGINAL] 23CD:071C, 23CD:3C42
    private void RestoreObjects()
    {
        for (int i = ObjectCount - 1; i >= 0; i--)
        {
            if (_restore[i, Page.DrawPage] is not { } slot) continue;
            _restore[i, Page.DrawPage] = null;
            int h = slot.Pixels.Length / ObjectWidth;
            for (int r = 0; r < h; r++)
                for (int c = 0; c < ObjectWidth; c++)
                    Plot(slot.X + c, slot.Y + r, slot.Pixels[r * ObjectWidth + c]);
        }
    }

    /// <summary>
    /// 0A96:0C3F(i) on VGA: 0B67:0299 with the page number DS:40DE[i] – number 1 copies page 0 to page 1, any other page 1
    /// to page 0.
    /// </summary>
    // [ORIGINAL] 0A96:0C3F – 0B67:0299(40DE[i]); VGA path 0B67:0313: latch copy of the whole page
    private void CopyPage(short index)
    {
        uint number = _d.U32(Int16Math.U(RulePage.PageNumbers + 4 * index));
        int from = number == 1 ? 0 : 1;
        var pixels = Page.PagePixels(from);
        var image = new IndexedImage(RulePage.Width, RulePage.Height, pixels);
        int keep = Page.DrawPage;
        Page.SelectDrawPage(1 - from);
        Page.Blit(image, 0, 0, RulePage.Width, RulePage.Height, 0, 0);
        Page.SelectDrawPage(keep);
    }

    /// <summary>
    /// A glyph pixel as the driver addresses it: offset (y·80 + x/4) mod 10000h in the segment of the draw page, plane x mod 4.
    /// Offsets inside page 0 or page 1 of the VGA window set the pixel there (x ≥ 320 runs on into the next row); others
    /// (negative y, the unused VRAM behind a page, memory behind the window) hold nothing the rule page keeps, like
    /// <see cref="RulePage.Get(ushort, ushort)"/>.
    /// </summary>
    // [ORIGINAL] 23CD:31FD – di = 23CD:3162(x, y), one byte per plane and pixel group, di + 77 per row
    private void Plot(int x, int y, byte colour)
    {
        ushort offset = (ushort)(y * RulePage.RowBytes + (x >> 2));
        int linear = Page.DrawPage * RulePage.PageDistance + offset;
        int keep = Page.DrawPage;
        for (int page = 0; page < 2; page++)
        {
            int relative = linear - page * RulePage.PageDistance;
            if (relative < 0 || relative >= RulePage.PageBytes) continue;
            Page.SelectDrawPage(page);
            Page.SetPixel(relative % RulePage.RowBytes * 4 + (x & 3), relative / RulePage.RowBytes, colour);
            Page.SelectDrawPage(keep);
            return;
        }
    }

    /// <summary>
    /// 0A96:0525 executed (0A96:0C68 → 23CD:05DD): the zero-terminated text at DS:offset with the current font, y the bottom
    /// row of the glyphs; per character (masked to 7 bits) a 16-pixel glyph, x advancing by the font's advance. Pixels run on
    /// in the page memory like the original's (x ≥ 320 continues in the next row).
    /// </summary>
    // [ORIGINAL] 23CD:05DD, 23CD:31FD – glyph rows as words, bit 15 − i = pixel x + i
    private void DrawText(short x, short y, ushort offset, byte colour)
    {
        int top = y - _font.Height + 1;
        int bx = x;
        for (int at = offset; ; at = (ushort)(at + 1))
        {
            int c = _d.U8(at) & 0x7F;
            if (c == 0) break;
            int glyph = _font.Offset + (c - _font.FirstChar) * 2 * _font.Height;
            for (int r = 0; r < _font.Height; r++)
            {
                int bits = _font.Data[glyph + 2 * r] << 8 | _font.Data[glyph + 2 * r + 1];
                for (int i = 0; i < 16; i++)
                {
                    if ((bits & (0x8000 >> i)) == 0) continue;
                    Plot(bx + i, top + r, colour);
                }
            }
            bx += _font.Advance;
        }
    }
}
