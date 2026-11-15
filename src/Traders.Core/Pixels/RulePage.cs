using Traders.Core.State;

namespace Traders.Core.Pixels;

/// <summary>
/// Rule page of the core (R6P-1, finding docs/routines/111D_0112_regelseite.md): the two VGA pages of the original
/// (page 0 at A000:0000, page 1 at A400:0000, 320 × 200 each) as far as game rules read them back. Where the original reads
/// a pixel of its draw page to decide something (23CD:06DE, a jump to the pixel reader 23CD:3764; called from 111D:0112
/// and 1B03:08ED/0BE7/1178), the core reads <see cref="Get"/> here instead of the screen. The content comes from explicit
/// inputs only: picture pixels (<see cref="IndexedImage"/>) and exactly the drawing steps that change read pixels before a
/// read, applied when the original executes them (for the command list 0A96 that is 0A96:06A8, not the enqueue).
/// Scenes and the host never write here; the page carries no palette and no display state.
/// </summary>
public sealed partial class RulePage
{
    public const int Width = 320, Height = 200;

    /// <summary>Bytes per row and plane in mode X (23CD:3162: offset y·80 + x/4, plane x mod 4).</summary>
    public const int RowBytes = 80;

    /// <summary>Bytes of one page per plane (16000) and distance of page 1 from page 0 in the VGA window (A400h − A000h)·16.</summary>
    public const int PageBytes = Height * RowBytes, PageDistance = 0x4000;

    /// <summary>DS:082E – page index the game draws to; DS:40DE – two page numbers (long, 1 = page 0, 2 = page 1).</summary>
    public const int DrawPageIndex = 0x082E, PageNumbers = 0x40DE;

    private readonly byte[][] _pages = [new byte[Width * Height], new byte[Width * Height]];

    /// <summary>Physical page drawn to and read from: 0 = A000:0000, 1 = A400:0000.</summary>
    public int DrawPage { get; private set; }

    /// <summary>Both pages cleared to colour 0, drawing on <paramref name="drawPage"/>.</summary>
    public RulePage(int drawPage = 0) => SelectDrawPage(drawPage);

    public void SelectDrawPage(int page)
    {
        if (page is not (0 or 1)) throw new ArgumentOutOfRangeException(nameof(page), "Page 0 or 1.");
        DrawPage = page;
    }

    /// <summary>
    /// Physical draw page after 21FD:051F as the data segment states it: page number DS:40DE[DS:082E] through 0B67:0238
    /// (1 → page 0, 2 → page 1).
    /// </summary>
    // [ORIGINAL] 21FD:051F – 4516 = 40DE[082E]; 0B67:0238(4516, 40DE[0830]) – draw number 1 → page 0, 2 → page 1
    public static int DrawPageOf(Dgroup d)
    {
        uint number = d.U32(PageNumbers + 4 * d.U16(DrawPageIndex));
        return number switch
        {
            1 => 0,
            2 => 1,
            _ => throw new InvalidOperationException(
                $"Page number {number}: 0B67:0238 keeps the previous draw page, which the data segment does not tell."),
        };
    }

    /// <summary>
    /// The page flip 21FD:051F for the rule page: the game draws on the other page afterwards. Valid while DS:40DE holds the
    /// two different numbers 1 and 2 (set up once by the program start); otherwise use <see cref="DrawPageOf"/>.
    /// </summary>
    // [ORIGINAL] 21FD:051F – 082E ^= 1, draw page = page of 40DE[082E]
    public void Flip() => DrawPage ^= 1;

    /// <summary>
    /// 21FD:0590 for the rule page: the game draws on the page shown, i.e. on the other page (DS:0830 is the complement of
    /// DS:082E after every flip). Valid under the same condition as <see cref="Flip"/>.
    /// </summary>
    // [ORIGINAL] 21FD:0590 – 082E = 0830, draw page = page of 40DE[082E]
    public void DrawOnShownPage() => DrawPage ^= 1;

    /// <summary>Pixel (x, y) of the physical page <paramref name="page"/> (0 ≤ x &lt; 320, 0 ≤ y &lt; 200).</summary>
    public byte this[int page, int x, int y] => _pages[CheckPage(page)][Index(x, y)];

    /// <summary>
    /// Pixel of the draw page as the original reads it, for every word argument: offset (y·80 + x/4) mod 10000h in the
    /// segment of the draw page, plane x mod 4. Inside page 0 or page 1 of the VGA window this is the pixel there (from page 0
    /// a y of 205 to 404 addresses page 1). Elsewhere the original reads VRAM no drawing step writes
    /// (A000:3E80–3FFF, A000:7E80–FFFF) or, from page 1, memory behind the VGA window: <see cref="Get"/> returns 0 there.
    /// </summary>
    // [ORIGINAL] 23CD:06DE – jmp 23CD:3764; 23CD:3764 – di = 23CD:3162(x, y) = y·80 + x/4, read map x & 3, al = [draw page:di]
    // [APPROX] outside both pages 0 (RefEmu: VRAM cleared at the mode set, memory behind the window empty); the readers
    // (111D:0112, 1B03) only compare with 1, real hardware may return other values there
    public byte Get(ushort x, ushort y)
    {
        ushort offset = (ushort)(y * RowBytes + (x >> 2));
        int linear = DrawPage * PageDistance + offset;
        int plane = x & 3;
        for (int page = 0; page < 2; page++)
        {
            int relative = linear - page * PageDistance;
            if (relative >= 0 && relative < PageBytes)
                return _pages[page][relative / RowBytes * Width + relative % RowBytes * 4 + plane];
        }
        return 0;
    }

    /// <summary>Signed form of <see cref="Get(ushort, ushort)"/> (the original passes words, e.g. y − 3 for y &lt; 3).</summary>
    public byte Get(int x, int y) => Get(unchecked((ushort)x), unchecked((ushort)y));

    /// <summary>
    /// Atlas rectangle to the draw page, pixel for pixel: page (dx + i, dy + j) = atlas (sx + i, sy + j). Only rectangles
    /// inside the atlas and inside the page (w, h ≥ 1); the original's do/while rows (h = 0 → 10000h rows) and its
    /// address wrap beyond the page are not modelled.
    /// </summary>
    // [ORIGINAL] 0A96:0030 – type 0, executed by 0A96:06A8 through 0A96:06E4 → 23CD:32D8 (vector 14)
    public void Blit(IndexedImage atlas, int sx, int sy, int w, int h, int dx, int dy)
    {
        ArgumentNullException.ThrowIfNull(atlas);
        CheckRect(dx, dy, w, h);
        if (sx < 0 || sy < 0 || sx + w > atlas.Width || sy + h > atlas.Height)
            throw new ArgumentOutOfRangeException(nameof(sx), $"Source ({sx}, {sy}) {w} × {h} lies outside the atlas.");
        var page = _pages[DrawPage];
        for (int j = 0; j < h; j++)
            for (int i = 0; i < w; i++)
                page[Index(dx + i, dy + j)] = atlas[sx + i, sy + j];
    }

    /// <summary>
    /// Area w × h at (dx, dy) tiled with the atlas tile (sx, sy) tw × th: rows 0, th, 2·th … while below h, columns 0, tw …
    /// while below w (signed), the last row and column cut to what is left; each cell is a <see cref="Blit"/>. Only areas
    /// inside the page (w, h ≤ 0 draw nothing).
    /// </summary>
    // [ORIGINAL] 0A96:0267 – type 6, executed by 0A96:06A8 through 0A96:0884 (cells as type 0 → 23CD:32D8)
    public void Tiles(IndexedImage atlas, int sx, int sy, int tw, int th, int dx, int dy, int w, int h)
    {
        if (tw < 1 || th < 1) throw new ArgumentOutOfRangeException(nameof(tw), "Tile of at least 1 × 1.");
        for (int si = 0; si < h; si += th)
            for (int di = 0; di < w; di += tw)
                Blit(atlas, sx, sy, Math.Min(tw, w - di), Math.Min(th, h - si), dx + di, dy + si);
    }

    /// <summary>Filled rectangle on the draw page in colour <paramref name="color"/> (VGA: colour unchanged).</summary>
    // [ORIGINAL] 0A96:04D7 – type 11, executed by 0A96:06A8 through 0A96:0B20 → 23CD:31A6 (vector 10)
    public void FillRect(int x, int y, int w, int h, byte color)
    {
        CheckRect(x, y, w, h);
        var page = _pages[DrawPage];
        for (int j = 0; j < h; j++) page.AsSpan(Index(x, y + j), w).Fill(color);
    }

    /// <summary>One pixel on the draw page (VGA: colour unchanged).</summary>
    // [ORIGINAL] 0A96:03FB – type 15, executed by 0A96:06A8 through 0A96:0BFB → 23CD:373B (vector 30)
    public void SetPixel(int x, int y, byte color) => _pages[DrawPage][Index(x, y)] = color;

    /// <summary>Copy of a whole physical page, row-major (for comparisons).</summary>
    public byte[] PagePixels(int page) => (byte[])_pages[CheckPage(page)].Clone();

    private static int CheckPage(int page) =>
        page is 0 or 1 ? page : throw new ArgumentOutOfRangeException(nameof(page), "Page 0 or 1.");

    private static int Index(int x, int y)
    {
        if ((uint)x >= Width || (uint)y >= Height)
            throw new ArgumentOutOfRangeException(nameof(x), $"({x}, {y}) lies outside the page.");
        return y * Width + x;
    }

    private static void CheckRect(int x, int y, int w, int h)
    {
        if (w < 1 || h < 1 || x < 0 || y < 0 || x + w > Width || y + h > Height)
            throw new ArgumentOutOfRangeException(nameof(w), $"Rectangle ({x}, {y}) {w} × {h} lies outside the page.");
    }
}
