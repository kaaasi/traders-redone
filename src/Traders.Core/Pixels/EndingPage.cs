namespace Traders.Core.Pixels;

/// <summary>
/// One of the original's bitmap fonts as explicit input of the rule page (R6P-1): glyphs of 16 pixels width, one 16-bit row
/// each (most significant bit = leftmost pixel), stored from <see cref="FirstChar"/> on; <see cref="Advance"/> pixels per
/// character. The host and the tests build it from FONTS.PP (Traders.Assets); the core never reads files.
/// </summary>
public sealed class RuleFont
{
    private readonly ushort[] _rows;

    public RuleFont(int height, int advance, int firstChar, ReadOnlySpan<ushort> rows)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);
        if (rows.Length % height != 0) throw new ArgumentException("Rows must be whole glyphs.", nameof(rows));
        Height = height;
        Advance = advance;
        FirstChar = firstChar;
        _rows = rows.ToArray();
    }

    public int Height { get; }

    public int Advance { get; }

    public int FirstChar { get; }

    /// <summary>Row <paramref name="y"/> of the glyph of character <paramref name="c"/> (0 outside the font).</summary>
    public ushort Row(int c, int y)
    {
        int index = (c - FirstChar) * Height + y;
        return c >= FirstChar && (uint)y < (uint)Height && index < _rows.Length ? _rows[index] : (ushort)0;
    }
}

/// <summary>
/// The rule page of the end screens (R6P-1, finding docs/routines/20B3_008E_ende.md, „Abend“): the evening 20B3:0525 reads
/// the draw page with 23CD:06DE in the cells (x, y + 6), x ≤ 35, y ≤ 12 (20B3:06DC). The core follows the drawing that reaches
/// those pixels: the command list 0A96 (blit of the picture, filled rectangles, pixels, texts and numbers in the font
/// <see cref="Font"/>), applied when the original executes it (0A96:06A8), and the page selection (flips, 21FD:0590,
/// 0B67:01D4/0206). Drawing outside the page is not modelled; the end screens draw only inside it.
/// </summary>
public sealed class EndingPage(RulePage page, IReadOnlyList<RuleFont>? fonts = null)
{
    private readonly List<Action<RulePage>> _list = [];
    private int _hidden;

    public RulePage Page { get; } = page;

    /// <summary>The driver's fonts by number (21FD:05DB); null = texts not drawn.</summary>
    public IReadOnlyList<RuleFont>? Fonts { get; } = fonts;

    /// <summary>Font of the texts and numbers of the list; null = texts not drawn.</summary>
    public RuleFont? Font { get; set; }

    /// <summary>DS:10DA – advance per font number (byte[3]); DS:10DD – height per font number; DS:3CA8/34A4 – those of the font set.</summary>
    public const int Advances = 0x10DA, Heights = 0x10DD, Advance = 0x3CA8, Height = 0x34A4;

    /// <summary>
    /// The font the driver draws with, as the data segment states it: 21FD:05DB(f) sets the driver's font f and copies its
    /// advance DS:10DA[f] to DS:3CA8 and its height DS:10DD[f] to DS:34A4; the font is the one whose pair matches. Without
    /// fonts or without a match (two fonts with the same pair) no text is drawn.
    /// </summary>
    // [ORIGINAL] 21FD:05DB – 23CD:05B6(f), DS:3CA8 = DS:10DA[f], DS:34A4 = DS:10DD[f]
    public void SelectFont(State.Dgroup d)
    {
        Font = null;
        if (Fonts is null) return;
        int found = -1;
        for (int f = 0; f < Fonts.Count; f++)
        {
            if (d.U8(Advances + f) != d.U16(Advance) || d.U8(Heights + f) != d.U16(Height)) continue;
            if (found >= 0) return;
            found = f;
        }
        if (found >= 0) Font = Fonts[found];
    }

    /// <summary>The picture last loaded (186C:0002), source of the blits; null = blits not drawn.</summary>
    public IndexedImage? Atlas { get; set; }

    // [ORIGINAL] 0A96:001B – the list emptied
    public void Clear() => _list.Clear();

    // [ORIGINAL] 0A96:06A8 – the list executed on the draw page (it stays in the list)
    public void Run()
    {
        foreach (var step in _list) step(Page);
    }

    /// <summary>21FD:060D: the list on the draw page, flip, the list again.</summary>
    // [ORIGINAL] 21FD:060D – 0A96:06A8, 21FD:051F, 0A96:06A8
    public void RunOnBothPages()
    {
        Run();
        Page.Flip();
        Run();
    }

    // [ORIGINAL] 0B67:01D4 – the draw page is the page shown
    public void DrawOnShown()
    {
        _hidden = Page.DrawPage;
        Page.SelectDrawPage(_hidden ^ 1);
    }

    // [ORIGINAL] 0B67:0206 – the draw page is the hidden page again
    public void DrawOnHidden() => Page.SelectDrawPage(_hidden);

    public void Blit(short sx, short sy, short w, short h, short dx, short dy)
    {
        if (Atlas is { } atlas) _list.Add(p => p.Blit(atlas, sx, sy, w, h, dx, dy));
    }

    public void FillRect(short x, short y, short w, short h, short colour) =>
        _list.Add(p => p.FillRect(x, y, w, h, (byte)colour));

    public void SetPixel(short x, short y, short colour) => _list.Add(p => p.SetPixel(x, y, (byte)colour));

    /// <summary>
    /// A text in the list (0A96:0525 type 12, without shadow: background &lt; 0): the characters masked to 7 bits, each glyph
    /// with its bottom row at y, x advancing by the font's advance (23CD:05DD).
    /// </summary>
    // [ORIGINAL] 23CD:05DD – glyphs of 16 pixels, top row y − height + 1, set bits in the colour
    public void Text(short x, short y, byte[] text, short colour)
    {
        if (Font is not { } font) return;
        _list.Add(p =>
        {
            int left = x, top = y - font.Height + 1;
            foreach (byte raw in text)
            {
                int c = raw & 0x7F;
                if (c == 0) break;
                for (int row = 0; row < font.Height; row++)
                {
                    ushort bits = font.Row(c, row);
                    for (int i = 0; i < 16; i++)
                        if ((bits & (0x8000 >> i)) != 0) Set(p, left + i, top + row, colour);
                }
                left += font.Advance;
            }
        });
    }

    private static void Set(RulePage p, int x, int y, short colour)
    {
        if ((uint)x < RulePage.Width && (uint)y < RulePage.Height) p.SetPixel(x, y, (byte)colour);
    }
}
