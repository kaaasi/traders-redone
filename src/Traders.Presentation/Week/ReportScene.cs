using Traders.Core.Menus;
using Traders.Core.State;
using Traders.Core.Week;
using Traders.Presentation.Field;
using Traders.Presentation.Menus;
using Traders.Vga.Layers;

namespace Traders.Presentation.Week;

/// <summary>
/// The week report scene: plays the <see cref="WeekEvent"/>s of the core's <see cref="Report"/> (0825:000A with the wait loop
/// 21FD:0004 in mode 2) through the graphics layers (finding docs/routines/0825_000A_wochenbilanz.md). It ports the drawing
/// routines 0825:09C9, 0072, 0178, 0364, 047D, 0560 and 0686; the table values come with the event of 0178, everything else
/// (ranks, cash, scale, target, week, wealth history, player colours, texts) is read from the core's data segment, which the
/// scene never changes. The loop variable DS:37AA of the original's player loops is a local of the scene; the core leaves
/// it at the value the original leaves. The graphics state lives in the layers' own data segment image.
/// </summary>
public sealed class ReportScene
{
    private readonly GraphicsLayers _gfx;
    private readonly Dgroup _state;
    private readonly MenuScene _loader;
    private readonly FieldScene _field;

    /// <param name="layers">Graphics layers with their own data segment image (DataSegment = segment value of DS).</param>
    /// <param name="state">The core's data segment (read only).</param>
    /// <param name="far">Memory behind far text pointers outside DS (the text buffer DS:4884).</param>
    /// <param name="pictures">Picture resources for 186C:0002.</param>
    public ReportScene(GraphicsLayers layers, Dgroup state, IFarMemory? far, IPictureSource pictures)
    {
        _gfx = layers;
        _state = state;
        _loader = new MenuScene(layers, state, far, pictures);
        _field = new FieldScene(layers, state);
    }

    public GraphicsLayers Layers => _gfx;

    /// <summary>
    /// Called right before a routine that hands the picture to the display or the DAC (21FD:0590, 03EB, 051F, 0329) runs, at
    /// the point where the original enters it.
    /// </summary>
    public Action<WeekEvent>? BeforePresent { get; set; }

    /// <summary>True while a fade or flip still has retrace waits for later frames.</summary>
    public bool Presenting => _pending is not null;

    private IEnumerator<int>? _pending;

    /// <summary>
    /// Starts a new frame: a fade or flip in progress runs on up to its next retrace wait (one retrace per frame, like the
    /// core's <see cref="MenuWait"/> frames). Call once per frame before the frame's events.
    /// </summary>
    public void NextFrame()
    {
        if (_pending is null) return;
        MirrorKeys();
        if (!_pending.MoveNext()) _pending = null;
    }

    /// <summary>Runs a fade or flip in progress to its end.</summary>
    public void Finish()
    {
        while (_pending is not null) NextFrame();
    }

    private void Present(IEnumerable<int> steps)
    {
        _pending = steps.GetEnumerator();
        NextFrame();
    }

    private void Announce(WeekEvent e) => BeforePresent?.Invoke(e);

    /// <summary>Plays one event (a presenting routine still in progress is finished first).</summary>
    public void Apply(WeekEvent e)
    {
        Finish();
        switch (e.Kind)
        {
            case WeekEventKind.LoadPicture:
                _loader.Apply(new MenuEvent(MenuEventKind.LoadPicture, e.Frame, e.A)); // 186C:0002 as in the menu scene
                break;
            case WeekEventKind.DrawOnShownPage:
                Announce(e);
                DrawOnShownPage();
                _gfx.ClearList(); // 0825:09C9 empties the list after 21FD:0590
                _wipe = 0;
                break;
            case WeekEventKind.PaletteStep:
                WipeStep(e);
                break;
            case WeekEventKind.BlendColor:
                if (_wipe is { } next) WipeColumns(next, WipeEnd, null);
                _wipe = null;
                _gfx.SetBlendColor((ushort)e.A);
                break;
            case WeekEventKind.Flip:
                Announce(e);
                MirrorKeys();
                Present(_gfx.FlipPagesSteps());
                break;
            case WeekEventKind.InvalidateObjects:
                _gfx.InvalidateAllRestoreRecords();
                break;
            case WeekEventKind.Draw:
                Draw(e);
                break;
            case WeekEventKind.CopyPage:
                _gfx.CopyGamePage(DrawPage);
                break;
            case WeekEventKind.LoadPalette:
                _gfx.CopyLoadedToBase(); // 21FD:026C (its far argument is not used)
                break;
            case WeekEventKind.FadeIn:
                Announce(e);
                Present(_gfx.FadeInSteps());
                break;
            case WeekEventKind.ListExecute:
                _gfx.ExecuteList();
                break;
            case WeekEventKind.ListClear:
                _gfx.ClearList();
                break;
            case WeekEventKind.RestoreObjects:
                _gfx.RestoreObjects(DrawPage);
                break;
            case WeekEventKind.DrawObjects:
                _gfx.DrawObjects(DrawPage);
                break;
            case WeekEventKind.ReportMark:
                Mark(e.B);
                break;
            case WeekEventKind.Sound:
                break; // sound is not the scene's
            default:
                throw new ArgumentOutOfRangeException(nameof(e), $"{e.Kind} is no event of the week report.");
        }
    }

    private ushort DrawPage => (ushort)(_gfx.Ds[GraphicsLayers.DrawPageIndex] | _gfx.Ds[GraphicsLayers.DrawPageIndex + 1] << 8);

    private static ushort W(int v) => unchecked((ushort)v);

    /// <summary>The key table bytes the flip reads (DS:3D91 keypad minus, DS:3D95 keypad plus) as the core holds them.</summary>
    private void MirrorKeys()
    {
        for (int sc = 0; sc < KeyTable.Length; sc++) _gfx.Ds[DgroupLayout.KeyState + sc] = _state.U8(DgroupLayout.KeyState + sc);
    }

    // ------------------------------------------------------------------ pages and the opening wipe

    /// <summary>
    /// [ORIGINAL] 21FD:0590: draw on the page shown – DS:082E = DS:0830, DS:4516 = its page number, 0B67:0238(that page,
    /// page of DS:0830) with its show-page retrace, then the retrace of 0B67:028F.
    /// </summary>
    private void DrawOnShownPage()
    {
        var ds = _gfx.Ds;
        ds[GraphicsLayers.DrawPageIndex] = ds[GraphicsLayers.ShownPageIndex];
        ds[GraphicsLayers.DrawPageIndex + 1] = ds[GraphicsLayers.ShownPageIndex + 1];
        int at = GraphicsLayers.PageNumbers + (DrawPage << 2);
        for (int i = 0; i < 4; i++) ds[GraphicsLayers.CurrentDrawPage + i] = ds[at + i];
        uint page = (uint)(ds[at] | ds[at + 1] << 8 | ds[at + 2] << 16 | ds[at + 3] << 24);
        _gfx.SelectPages(page, page);
        _gfx.WaitForRetrace();
    }

    private const int WipeEnd = 0x13F;

    /// <summary>Next column pair of the opening wipe, or null outside the wipe.</summary>
    private int? _wipe;

    /// <summary>
    /// 21FD:03EB(A, B) inside 0825:09C9: the wipe draws on to the column pair si = B·16 + B0h, the palette step runs after its
    /// two lines are added and before they are executed.
    /// </summary>
    private void WipeStep(WeekEvent e)
    {
        if (_wipe is not { } next) throw new InvalidOperationException("Palette step outside the opening wipe.");
        int si = e.B * 16 + 0xB0;
        WipeColumns(next, si, () =>
        {
            Announce(e);
            _gfx.Blend((ushort)e.A, (ushort)e.B); // [ORIGINAL] 21FD:03EB: working palette, retrace, DAC
        });
        _wipe = si + 1;
    }

    /// <summary>
    /// [ORIGINAL] 0825:09C9 (column loop) – for si = <paramref name="from"/> … <paramref name="to"/>: the line from (si, 0) to
    /// (si − C7h, C7h) clipped to the left edge and its mirror image from the bottom right, both in colour 0; on VGA
    /// (DS:090E = 1) the list is executed and emptied after every pair, otherwise after every third (si mod 3 = 0).
    /// <paramref name="beforeLast"/> runs between adding and executing the last pair.
    /// </summary>
    private void WipeColumns(int from, int to, Action? beforeLast)
    {
        for (int si = from; si <= to; si++)
        {
            int x2, y2;
            if (si <= 0xC7) (x2, y2) = (0, si);
            else (x2, y2) = (si - 0xC7, 0xC7);
            _gfx.AddLine(W(si), 0, W(x2), W(y2), 0);
            _gfx.AddLine(W(0x13F - si), 0xC7, W(0x13F - x2), W(0xC7 - y2), 0);
            if (si == to) beforeLast?.Invoke();
            bool vga = (_gfx.Ds[GraphicsLayers.CardMode] | _gfx.Ds[GraphicsLayers.CardMode + 1] << 8) == 1;
            if (vga || si % 3 == 0)
            {
                _gfx.ExecuteList();
                _gfx.ClearList();
            }
        }
    }

    // ------------------------------------------------------------------ drawing routines

    private void Draw(WeekEvent e)
    {
        switch (e.A)
        {
            case 0x0072: Frame(); break;
            case 0x0178: Table(e.Values ?? throw new ArgumentException("0825:0178 without values.", nameof(e))); break;
            case 0x0364: Scale(); break;
            case 0x047D: Weeks(); break;
            case 0x0560: Graph(); break;
            default: throw new ArgumentOutOfRangeException(nameof(e), $"No drawing routine 0825:{e.A:X4}.");
        }
    }

    private (ushort Off, ushort Seg) Pointer(int dsOffset) => (_state.U16(dsOffset), _state.U16(dsOffset + 2));

    private int Rank(int p) => _state.U8(DgroupLayout.Rank + p);

    private int RowOfRank(int p) => _state.U8(DgroupLayout.RankUnique + p) * 0x2C + 0x0E;

    private ushort Color(int p) => _state.U16(ReportLayout.PlayerColors + 2 * p);

    private short GraphScale => _state.S16(ReportLayout.GraphScale);

    /// <summary>
    /// [ORIGINAL] 0825:0072 – background (0609:1ED9) and frame (0609:1256) of the whole screen, the graph area (37h, 16h)
    /// C4h × 79h and the table area (37h, 95h) C4h × 2Ah in colour 5, the table outlined in colour 9, per player p
    /// (DS:37AA) the emblem 0609:1879(100h, row of rank DS:302A[p], DS:3D42[p]); then in font 1 the title (text 0Ah)
    /// centred in 140h at y 10h, colour 5, shadow 0Ah at (−5, +3); back to font 0.
    /// </summary>
    private void Frame()
    {
        _gfx.ClearList();
        _field.Background();
        _field.Frame(0, 0, 0x140, 0xC8);
        _gfx.AddFilledRect(0x37, 0x16, 0xC4, 0x79, 5);
        _gfx.AddFilledRect(0x37, 0x95, 0xC4, 0x2A, 5);
        _gfx.AddFrame(0x37, 0x95, 0xC4, 0x2A, 9);
        for (int p = 0; p < DgroupLayout.PlayerCount; p++) Emblem(p, 0x100, (short)RowOfRank(p), (short)Rank(p));
        _gfx.ExecuteList();
        Font(1);
        _gfx.ClearList();
        var title = Pointer(ReportLayout.TitleText);
        _gfx.AddCenteredShadowText(0, 0x10, 0x140, title.Off, title.Seg, 5, 0x0A, W(-5), 3);
        _gfx.ExecuteList();
        Font(0);
    }

    /// <summary>
    /// 0609:1879(x, y, rank) for player <paramref name="p"/>: as <see cref="FieldScene.Emblem"/>, which takes the player from
    /// the core's DS:37AA; here the player is the scene's own loop variable (the core leaves DS:37AA at 4 after 0825:0072,
    /// and a scene never writes it). Emblem 30h × 2Ch from atlas (C0h, 2Ch·p); for rank &gt; 0 the badge 10h × 0Bh from
    /// atlas ((rank − 1)·10h, 0Bh·p + C6h) at (x + 10h, y + DS:00D8[p]).
    /// </summary>
    // [ORIGINAL] 0609:1879
    private void Emblem(int p, short x, short y, short rank)
    {
        _gfx.AddBlit(0xC0, W(p * 0x2C), 0x30, 0x2C, W(x), W(y));
        if (rank <= 0) return;
        ushort dy = W(y + _state.U16(ReportLayout.EmblemBadgeOffsets + 2 * p));
        _gfx.AddBlit(W((rank - 1) << 4), W(p * 0x0B + 0xC6), 0x10, 0x0B, W(x + 0x10), dy);
    }

    /// <summary>
    /// [ORIGINAL] 0825:0178 – the table: the five row labels (texts 0Bh…0Fh) at (39h, 9Bh + 6·i) and the total label (text
    /// 10h) at (39h, BCh) in colour 2; per player p (DS:37AA) in column x = 5Dh + 28h·p and the player's colour the amounts
    /// (5 digits, "$") cash DS:3CD2[p] at y 9Bh, land A1h, wares A7h, gems ADh, other B3h and total BCh – the last five as
    /// the core computed them (0825:086F, 088D, 08EB, 096D, 0813).
    /// </summary>
    private void Table(IReadOnlyList<int> values)
    {
        _gfx.ClearList();
        for (int i = 0; i <= 4; i++)
        {
            var label = Pointer(ReportLayout.RowTexts + 4 * i);
            _gfx.AddText(0x39, W(i * 6 + 0x9B), label.Off, label.Seg, 2, 0xFFFF);
        }
        var total = Pointer(ReportLayout.TotalText);
        _gfx.AddText(0x39, 0xBC, total.Off, total.Seg, 2, 0xFFFF);
        ReadOnlySpan<int> rows = [0xA1, 0xA7, 0xAD, 0xB3, 0xBC];
        for (int p = 0; p < DgroupLayout.PlayerCount; p++)
        {
            ushort color = Color(p), x = W(p * 0x28 + 0x5D);
            _gfx.AddAmount(x, 0x9B, _state.U32(DgroupLayout.Cash + 4 * p), 5, color, 0xFFFF);
            for (int k = 0; k < rows.Length; k++)
                _gfx.AddAmount(x, W(rows[k]), unchecked((uint)values[5 * p + k]), 5, color, 0xFFFF);
        }
        _gfx.ExecuteList();
    }

    /// <summary>
    /// [ORIGINAL] 0825:0364 – amount axis: for i = 0 … 12 the amount DS:1584·i·10 (5 digits, "$", colour 5, shadow 0Bh at
    /// (−1, +1)) at (10h, 90h − 10·i) and a horizontal line in colour 9 at y 8Eh − 10·i from x 37h, C4h wide; if
    /// DS:1584·120 ≥ the target DS:08F4 (unsigned) the target line in colour 2 at y 8Eh − target / DS:1584 (unsigned).
    /// </summary>
    private void Scale()
    {
        _gfx.ClearList();
        int scale = GraphScale;
        for (int i = 0; i < 13; i++)
        {
            uint amount = unchecked((uint)(scale * i * 10)); // LXMUL 0000:4F4E twice
            _gfx.AddShadowAmount(0x10, W(0x90 - i * 10), amount, 5, 5, 0x0B, 0xFFFF, 1);
            _gfx.AddHorizontalLine(0x37, W(0x8E - i * 10), 0xC4, 9);
        }
        uint target = _state.U32(DgroupLayout.Target);
        if (unchecked((uint)(scale * ReportLayout.ScaleDivisor)) >= target)
            _gfx.AddHorizontalLine(0x37, W(0x8E - (int)UnsignedDiv(target, unchecked((uint)scale))), 0xC4, 2);
        _gfx.ExecuteList();
    }

    /// <summary>
    /// [ORIGINAL] 0825:047D – week axis: first shown week si = DS:4B9A − 27h (at least 0); for i = 0 … 39 with (si + i) mod
    /// 10 = 0 a vertical line in colour 9 at x 37h + 5·i from y 20h, 6Fh high, and for 2 ≤ i ≤ 25h the week number si + i
    /// (2 digits, colour 0Ah, no shadow) at (32h + 5·i, 1Ch).
    /// </summary>
    private void Weeks()
    {
        _gfx.ClearList();
        int si = Math.Max(0, (int)(short)(_state.S16(DgroupLayout.Week) - 0x27));
        for (int i = 0; i < 0x28; i++)
        {
            if ((si + i) % 10 != 0) continue; // LMOD 0000:4F98
            _gfx.AddVerticalLine(W(i * 5 + 0x37), 0x20, 0x6F, 9);
            if (i is >= 2 and <= 0x25) _gfx.AddNumber(W(i * 5 + 0x32), 0x1C, unchecked((uint)(si + i)), 2, 0x0A, 0xFFFF);
        }
        _gfx.ExecuteList();
    }

    /// <summary>
    /// [ORIGINAL] 0825:0560 – wealth graph: the ring buffer DS:411C (40 weeks per player, stride A0h) is read from the oldest
    /// shown week on (index DS:4B9A + 1 wrapped into 0…27h once more than 40 weeks have passed, else 0); for di = 0 … 39 while
    /// di ≤ DS:4B9A, per player p (DS:37AA) the height v = wealth / DS:1584 (LDIV, at most 78h) and from di = 1 on a line
    /// in the player's colour from the previous point to (37h + 5·di, 8Eh − v); each week's lines are executed together.
    /// Then the graph area is outlined in colour 9.
    /// </summary>
    private void Graph()
    {
        short week = _state.S16(DgroupLayout.Week);
        int si = week > 0x27 ? week + 1 : 0;
        while (si > 0x27) si -= 0x28;
        Span<int> previous = stackalloc int[DgroupLayout.PlayerCount];
        for (int di = 0; di < 0x28 && di <= week; di++)
        {
            _gfx.ClearList();
            for (int p = 0; p < DgroupLayout.PlayerCount; p++)
            {
                int wealth = _state.S32(DgroupLayout.WealthHistory + p * 0xA0 + si * 4);
                int v = (short)SignedDiv(wealth, GraphScale); // low word, compared signed
                if (v > 0x78) v = 0x78;
                if (di > 0) _gfx.AddLine(W((di - 1) * 5 + 0x37), W(previous[p]), W(di * 5 + 0x37), W(0x8E - v), Color(p));
                previous[p] = (short)(0x8E - v);
            }
            _gfx.ExecuteList();
            if (++si > 0x27) si = 0;
        }
        _gfx.ClearList();
        _gfx.AddFrame(0x37, 0x16, 0xC4, 0x79, 9);
        _gfx.ExecuteList();
    }

    /// <summary>
    /// [ORIGINAL] 0825:0686 (drawing part) – the confirmation mark of a player: list emptied, the emblem area 30h × 2Ch at
    /// (100h, row of the player's rank <paramref name="rank"/>) tiled with the interior tile (F0h, B0h) 20h × 16h. The core
    /// reports the following 21FD:060D as list, flip, list.
    /// </summary>
    private void Mark(int rank)
    {
        _gfx.ClearList();
        _gfx.AddTiles(0xF0, 0xB0, 0x20, 0x16, 0x100, W(rank * 0x2C + 0x0E), 0x30, 0x2C);
    }

    /// <summary>
    /// [ORIGINAL] 21FD:05DB(font): driver font (23CD:05B6), DS:3CA8 = advance B[10DA + font], DS:34A4 = line height
    /// B[10DD + font], text shadow 0A96:000A(advance, height).
    /// </summary>
    private void Font(int font)
    {
        _gfx.Driver.Font = (ushort)font;
        ushort advance = _gfx.Ds[0x10DA + font], height = _gfx.Ds[0x10DD + font];
        Set16(GraphicsLayers.FontAdvance, advance);
        Set16(GraphicsLayers.FontLineHeight, height);
        _gfx.SetTextShadow(advance, height);
    }

    private void Set16(int at, ushort v)
    {
        _gfx.Ds[at] = (byte)v;
        _gfx.Ds[at + 1] = (byte)(v >> 8);
    }

    // ------------------------------------------------------------------ runtime long division (0000:4FA3)

    /// <summary>
    /// LDIV 0000:4F89: magnitudes divided, sign fixed afterwards (truncates toward zero, MinValue / −1 wraps); a zero divisor
    /// ends in the 16-bit DIV of 0000:5027 and the divide error (the original program stops).
    /// </summary>
    private static int SignedDiv(int a, int b)
    {
        if (b == 0) throw new DivideByZeroException("0825:0560: scale DS:1584 = 0 (divide error in the original).");
        return unchecked((int)((long)a / b));
    }

    /// <summary>LUDIV 0000:4F90 (unsigned); a zero divisor raises the divide error as in <see cref="SignedDiv"/>.</summary>
    private static uint UnsignedDiv(uint a, uint b)
    {
        if (b == 0) throw new DivideByZeroException("0825:0364: scale DS:1584 = 0 (divide error in the original).");
        return a / b;
    }
}
