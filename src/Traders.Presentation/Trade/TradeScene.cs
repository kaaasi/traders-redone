using Traders.Core;
using Traders.Core.Field;
using Traders.Core.Rules;
using Traders.Core.State;
using Traders.Core.Trade;
using Traders.Presentation.Field;
using Traders.Presentation.Menus;
using Traders.Vga.Layers;
using Clock = Traders.Core.Rules.Countdown;
using L = Traders.Core.State.DgroupLayout;
using T = Traders.Core.Trade.TradeLayout;

namespace Traders.Presentation.Trade;

/// <summary>
/// Drawing side of the trade 0B9F:0002 (finding docs/routines/0B9F_0002_handel.md, section "Szene"): plays the requests of
/// the core (<see cref="TradeEvent"/>) in the order they are emitted. The trade's own drawing routines of segment 0C29 (board
/// of picture V-AHANDE or V-BHANDE, panels with cash and stock, need bars, markers on the price scale, countdown) append to
/// the command list 0A96 or draw with the driver 23CD as the original does; pictures, pages, fades and waits are played frame
/// by frame, in step with the core's <see cref="FieldWait.Retrace"/> frames (the shared ones by a <see cref="FieldScene"/> on
/// the same layers). No game state is changed: the core's data segment is only read, at the moment the request is applied,
/// so every request has to be applied when the core emits it.
/// </summary>
public sealed class TradeScene
{
    // Constant tables of the data segment (bytes unless noted).
    private const int PlayerColour = 0x0240;      // [picture·4 + player], colour of the player's numbers
    private const int Ware2Colours = 0x0248;      // word[5], base palette colours 1..5 for ware 2
    private const int Ware3Colours = 0x0252;      // word[3], base palette colours 1..3 for ware 3
    private const int StockColour = 0x025C;       // [picture]
    private const int PriceColour = 0x025E;       // [picture]
    private const int PriceY = 0x0260;            // [picture]
    private const int PartX = 0x0262, PartY = 0x0276, PartH = 0x028A; // word[10], parts of picture A (0C29:0BD9)
    private const int ScaleEdge = 0x029E;         // [picture·4 + column], colours of the scale's edge
    private const int WareX = 0x02AA;             // word[4], the ware's emblem in the atlas
    private const int WareY = 0x02B2, WareH = 0x02B6, WareDx = 0x02BA, WareDy = 0x02BE; // [ware]; Dx, Dy signed
    private const int Heap0 = 0x02C2, Heap1 = 0x0326, Heap2 = 0x04B6, Heap3 = 0x04DE; // rows of the stock heaps per player
    private const int HeapColour = 0x0566;        // [player]
    private const int BarDx = 0x056A, BarDy = 0x05CF; // [stock / 10], end point of a bar of picture B
    private const int NeedColour = 0x063C, ShownColour = 0x0640; // [player], bars of picture B
    private const int NumberBuffer = 0x02A6;      // far pointer to the number buffer (DS:0644)

    private readonly GraphicsLayers _gfx;
    private readonly Dgroup _state;
    private IEnumerator<int>? _pending;

    /// <param name="layers">Graphics layers (command list, pages, atlas).</param>
    /// <param name="state">The core's data segment (read only).</param>
    /// <param name="pictures">Picture resources for 186C:0002.</param>
    public TradeScene(GraphicsLayers layers, Dgroup state, IPictureSource? pictures)
    {
        _gfx = layers;
        _state = state;
        Field = new FieldScene(layers, state) { Pictures = pictures };
    }

    /// <summary>The field scene that plays the shared output requests (picture, pages, fade in, waits).</summary>
    public FieldScene Field { get; }

    public GraphicsLayers Layers => _gfx;

    /// <summary>True while a fade, flip, transition or wait still has retrace waits for later frames.</summary>
    public bool Presenting => _pending is not null || Field.Presenting;

    /// <summary>
    /// Starts a new frame with the keys held in it: a presenting routine in progress runs on up to its next retrace wait.
    /// Call once per frame before the core processes the frame.
    /// </summary>
    public void NextFrame(IReadOnlyCollection<byte> held)
    {
        Field.NextFrame(held);
        Step();
    }

    /// <summary>Runs a presenting routine in progress to its end.</summary>
    public void Finish()
    {
        Field.Finish();
        while (_pending is not null) Step();
    }

    private void Step()
    {
        if (_pending is not null && !_pending.MoveNext())
        {
            _pending.Dispose();
            _pending = null;
        }
    }

    private void Present(IEnumerable<int> steps)
    {
        _pending = steps.GetEnumerator();
        Step();
    }

    private void Shared(FieldEventKind kind, params short[] args) => Field.Apply(new FieldEvent(kind, args));

    /// <summary>Plays a request of the trade. Sound is not the scene's.</summary>
    public void Apply(TradeEvent e)
    {
        Finish();
        var a = e.Args;
        switch (e.Kind)
        {
            case TradeEventKind.Sound: break; // the sound driver's
            case TradeEventKind.LoadImage: Shared(FieldEventKind.LoadImage, a[0]); break;
            case TradeEventKind.SetFont: Font(a[0]); break;
            case TradeEventKind.NumberFill: _gfx.SetNumberFill(W(a[0])); break;
            case TradeEventKind.ClearList: _gfx.ClearList(); break;
            case TradeEventKind.RunList: _gfx.ExecuteList(); break;
            case TradeEventKind.ShowPage: Shared(FieldEventKind.ShowPage); break;
            case TradeEventKind.FlipPages: Shared(FieldEventKind.FlipPages); break;
            case TradeEventKind.LastFrame: Shared(FieldEventKind.LastFrame); break;
            case TradeEventKind.Delay: Shared(FieldEventKind.Delay, a[0], a[1]); break;
            case TradeEventKind.FadeIn: Shared(FieldEventKind.FadeIn); break;
            case TradeEventKind.FadeInFast: Present(_gfx.FadeInFastSteps()); break;      // 21FD:0353
            case TradeEventKind.FadeOutFast: Present(_gfx.FadeOutFastSteps(W(a[0]))); break; // 21FD:03B4
            case TradeEventKind.Transition: Present(TransitionSteps()); break;
            case TradeEventKind.TradePalette: Palette(); break;
            case TradeEventKind.FillRect: _gfx.AddFilledRect(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4])); break;
            case TradeEventKind.DrawBoard: Board(); break;
            case TradeEventKind.DrawBasePrice: BasePrice(a[0]); break;
            case TradeEventKind.DrawStock: Stock(a[0]); break;
            case TradeEventKind.DrawNeed: NeedBar(a[0]); break;
            case TradeEventKind.DrawPlayerNumbers: PlayerNumbers(a[0], a[1], (a[2] & 0xFFFF) | a[3] << 16, a[4]); break;
            case TradeEventKind.DrawStockChange: StockChange(a[0], a[1], a[2], a[3], a[4]); break;
            case TradeEventKind.DrawMarkers: Markers(); break;
            case TradeEventKind.ClearMarker: ClearMarker(a[0], a[1], a[2]); break;
            case TradeEventKind.DrawCountdown: Countdown(); break;
            case TradeEventKind.CountdownStep: CountdownStep(); break;
            default: throw new InvalidOperationException($"trade event {e.Kind}");
        }
    }

    private static ushort W(int v) => unchecked((ushort)v);

    private byte B(int at) => _state.U8(W(at));

    private short S8(int at) => _state.S8(W(at));

    private short S16(int at) => _state.S16(W(at));

    private ushort U16(int at) => _state.U16(W(at));

    private int Picture => S8(T.Variant);

    private int Ware => S16(T.Ware);

    private short PanelX(int p) => S16(T.PanelX + 2 * p);

    private short PanelY(int p) => S16(T.PanelY + 2 * p);

    private short ScaleX => S16(T.ScaleX);

    private short StockOf(int p) => S16(L.Stock + (p << 3) + (Ware << 1));

    private int NeedOf(int p) => _state.S32(W(T.Need + 4 * p));

    private bool Vga => (_gfx.Ds[GraphicsLayers.CardMode] | _gfx.Ds[GraphicsLayers.CardMode + 1] << 8) == 1;

    private ushort Gfx16(int at) => (ushort)(_gfx.Ds[at] | _gfx.Ds[at + 1] << 8);

    private void SetGfx16(int at, ushort v)
    {
        _gfx.Ds[at] = (byte)v;
        _gfx.Ds[at + 1] = (byte)(v >> 8);
    }

    /// <summary>
    /// [ORIGINAL] 21FD:05DB(font): driver font (23CD:05B6), DS:3CA8 = advance B[10DA + font], DS:34A4 = line height
    /// B[10DD + font], text shadow 0A96:000A(advance, height) (as HarvestScene.Font).
    /// </summary>
    private void Font(int font)
    {
        _gfx.Driver.Font = (ushort)font;
        ushort advance = _gfx.Ds[0x10DA + font], height = _gfx.Ds[0x10DD + font];
        SetGfx16(GraphicsLayers.FontAdvance, advance);
        SetGfx16(GraphicsLayers.FontLineHeight, height);
        _gfx.SetTextShadow(advance, height);
    }

    /// <summary>
    /// [ORIGINAL] 0C29:04BD: base palette = loaded palette (21FD:026C), then for ware 2 the colours 1..5 from the words
    /// DS:0248 and for ware 3 the colours 1..3 from DS:0252 (21FD:02C9).
    /// </summary>
    private void Palette()
    {
        _gfx.CopyLoadedToBase();
        if (Ware == 2)
            for (int i = 0; i <= 4; i++) _gfx.SetBaseColor(W(i + 1), U16(Ware2Colours + 2 * i));
        if (Ware == 3)
            for (int i = 0; i <= 2; i++) _gfx.SetBaseColor(W(i + 1), U16(Ware3Colours + 2 * i));
    }

    /// <summary>
    /// [ORIGINAL] 0B9F:080C: draw on the page shown (21FD:0590); 100 times: list emptied, a black line across row i and row
    /// C7h − i (0A69:000B), from row 31h on every seventh row the next stage of the blend towards colour 3 (21FD:03EB), on
    /// card 1 one retrace (21FD:0185), list executed; then blend colour 3 (21FD:04DA) and a flip (21FD:051F). The DAC values
    /// of 03EB become visible after its retrace, so the registers are held back until the next frame (as FieldScene.Output).
    /// </summary>
    private IEnumerable<int> TransitionSteps()
    {
        foreach (int s in DrawOnShownPageSteps()) yield return s;
        var dac = _gfx.Dac.Registers;
        bool vga = Vga;
        ushort stage = 0;
        for (short i = 0; i < 0x64; i++)
        {
            _gfx.ClearList();
            _gfx.AddHorizontalLine(0, W(i), 0x140, 0);
            _gfx.AddHorizontalLine(0, W(0xC7 - i), 0x140, 0);
            if (i >= 0x31 && i % 7 == 0)
            {
                stage++;
                var before = (byte[])dac.Clone();
                _gfx.Blend(3, stage);
                if (vga)
                {
                    var after = (byte[])dac.Clone();
                    before.CopyTo(dac, 0);
                    yield return 0;
                    after.CopyTo(dac, 0);
                }
            }
            if (vga)
            {
                _gfx.WaitRetrace();
                yield return 0;
            }
            _gfx.ExecuteList();
        }
        _gfx.SetBlendColor(3);
        foreach (int s in _gfx.FlipPagesSteps()) yield return s;
    }

    /// <summary>
    /// [ORIGINAL] 21FD:0590 as steps: DS:082E = DS:0830, the draw pointer DS:4516 from the page numbers DS:40DE, then
    /// 0B67:0238(draw, page of 0830): the page shown again (retrace), draw on it; then 0B67:028F (retrace) (as DisasterScene).
    /// </summary>
    private IEnumerable<int> DrawOnShownPageSteps()
    {
        ushort shown = Gfx16(GraphicsLayers.ShownPageIndex);
        SetGfx16(GraphicsLayers.DrawPageIndex, shown);
        int at = GraphicsLayers.PageNumbers + (shown << 2);
        ushort low = Gfx16(at), high = Gfx16(at + 2);
        SetGfx16(GraphicsLayers.CurrentDrawPage, low);
        SetGfx16(GraphicsLayers.CurrentDrawPage + 2, high);
        uint number = (uint)(low | high << 16);
        _gfx.SelectPages(number, number);
        yield return 0;
        _gfx.WaitRetrace();
        yield return 0;
    }

    /// <summary>
    /// [ORIGINAL] 0C29:0635: list emptied, the board of the picture (0C29:065C picture A, 0739 picture B), the base price
    /// 0C29:0B1F, list executed.
    /// </summary>
    private void Board()
    {
        _gfx.ClearList();
        if (Picture == 0) BoardA(); else BoardB();
        BasePrice(S16(T.BasePrice));
        _gfx.ExecuteList();
    }

    /// <summary>
    /// [ORIGINAL] 0C29:065C – picture A: per player the panel part of the player at (DS:2BD0, DS:2BDA) and the buyer's top
    /// (part 8 at y 0) or the seller's bottom (part 6 at y 96h); the scale (part 4) at DS:2BCE, the label (part 9) with the
    /// ware's emblem 0C29:1050 at (DS:2BD8, DS:2BE2), the countdown's frame (part 5) at (DS:2BCA, 96h), and the edges
    /// 0C29:0C8A left of the scale (if it is not at 0) and right of it.
    /// </summary>
    private void BoardA()
    {
        for (int p = 0; p < 4; p++)
        {
            short x = PanelX(p);
            Part(p, x, PanelY(p));
            if (B(T.Role + p) == 0) Part(8, x, 0);
            else Part(6, x, 0x96);
        }
        Part(4, ScaleX, 0);
        Part(9, S16(T.LabelX), S16(T.LabelY));
        Emblem(S16(T.LabelX), S16(T.LabelY));
        Part(5, S16(T.CountdownX), 0x96);
        ScaleEdges();
    }

    /// <summary>[ORIGINAL] 0C29:065C / 0739, their end: the scale's edges 0C29:0C8A at DS:2BCE − 4 (if DS:2BCE &gt; 0) and + 40h.</summary>
    private void ScaleEdges()
    {
        if (ScaleX > 0) Edge(Int16Math.S(ScaleX - 4));
        Edge(Int16Math.S(ScaleX + 0x40));
    }

    /// <summary>
    /// [ORIGINAL] 0C29:0BD9(part, x, y): part 4 is the scale – black column 40h × C8h, its head (0, 96h) 40h × 8 at y 5, the
    /// row (40h, C8h) 40h × 1 tiled over 40h × 5 at y 0, its foot (0, A0h) 40h × 28h at y A0h; every other part is 40h wide
    /// from the atlas at (W[0262 + 2·part], W[0276 + 2·part]), W[028A + 2·part] high.
    /// </summary>
    private void Part(int part, short x, short y)
    {
        if (part == 4)
        {
            _gfx.AddFilledRect(W(x), 0, 0x40, 0xC8, 0);
            _gfx.AddBlit(0, 0x96, 0x40, 8, W(x), 5);
            _gfx.AddTiles(0x40, 0xC8, 0x40, 1, W(x), 0, 0x40, 5);
            _gfx.AddBlit(0, 0xA0, 0x40, 0x28, W(x), 0xA0);
            return;
        }
        _gfx.AddBlit(U16(PartX + 2 * part), U16(PartY + 2 * part), 0x40, U16(PartH + 2 * part), W(x), W(y));
    }

    /// <summary>[ORIGINAL] 0C29:0C8A(x): four columns C8h high at x … x + 3 in the colours B[029E + 4·picture + i] (0A69:002C).</summary>
    private void Edge(short x)
    {
        for (int i = 0; i <= 3; i++)
            _gfx.AddVerticalLine(W(x + i), 0, 0xC8, B(ScaleEdge + (Picture << 2) + i));
    }

    /// <summary>
    /// [ORIGINAL] 0C29:1050(x, y): the ware's emblem OR-blitted (0A96:020E) from (W[02AA + 2·ware], B[02B2 + ware]), 30h wide,
    /// B[02B6 + ware] high, to (x + B[02BA + ware], y + B[02BE + ware]) (offsets signed).
    /// </summary>
    private void Emblem(short x, short y)
    {
        int w = Ware;
        _gfx.AddOrBlit(U16(WareX + 2 * w), B(WareY + w), 0x30, B(WareH + w), W(x + S8(WareDx + w)), W(y + S8(WareDy + w)));
    }

    /// <summary>
    /// [ORIGINAL] 0C29:0739 – picture B: per player at x = DS:2BD0 a buyer's column (top (0, 2Ch) 40h × 57h, the player's
    /// emblem OR-blitted at (x + 9, 13h), a black field (x, 57h) 40h × 2Ch with the player's picture (30h·p, 0) 30h × 2Ch at
    /// x + 8, the foot (0 or 40h, 83h) 40h × 45h at y 83h – the first for x = 0) or a seller's column (bottom (80h, 63h)
    /// 40h × 65h at y 63h, emblem at (x + 9, 7Ch), black field (x, 37h) with the picture at x + 8, the head (80h for
    /// x = 100h, otherwise 40h; 2Ch) 40h × 37h at y 0). Then the scale: black column at DS:2BCE, its head (F0h, 0) 40h × 0Dh,
    /// its foot (F0h, 0Dh) 40h × 27h at y A1h, the ware's emblem 0C29:1050 at (DS:2BD8, DS:2BE2) and the edges.
    /// </summary>
    private void BoardB()
    {
        for (int p = 0; p < 4; p++)
        {
            short x = PanelX(p);
            ushort emblemX = W(p % 2 * 0x30 + 0xC0), emblemY = W(p / 2 * 0x2E + 0x3B);
            if (B(T.Role + p) == 0)
            {
                _gfx.AddBlit(0, 0x2C, 0x40, 0x57, W(x), 0);
                _gfx.AddOrBlit(emblemX, emblemY, 0x30, 0x2E, W(x + 9), 0x13);
                _gfx.AddFilledRect(W(x), 0x57, 0x40, 0x2C, 0);
                _gfx.AddBlit(W(p * 0x30), 0, 0x30, 0x2C, W(x + 8), 0x57);
                if (x == 0) _gfx.AddBlit(0, 0x83, 0x40, 0x45, 0, 0x83);
                else _gfx.AddBlit(0x40, 0x83, 0x40, 0x45, W(x), 0x83);
            }
            else
            {
                _gfx.AddBlit(0x80, 0x63, 0x40, 0x65, W(x), 0x63);
                _gfx.AddOrBlit(emblemX, emblemY, 0x30, 0x2E, W(x + 9), 0x7C);
                _gfx.AddFilledRect(W(x), 0x37, 0x40, 0x2C, 0);
                _gfx.AddBlit(W(p * 0x30), 0, 0x30, 0x2C, W(x + 8), 0x37);
                if (x == 0x100) _gfx.AddBlit(0x80, 0x2C, 0x40, 0x37, 0x100, 0);
                else _gfx.AddBlit(0x40, 0x2C, 0x40, 0x37, W(x), 0);
            }
        }
        _gfx.AddFilledRect(W(ScaleX), 0, 0x40, 0xC8, 0);
        _gfx.AddBlit(0xF0, 0, 0x40, 0x0D, W(ScaleX), 0);
        _gfx.AddBlit(0xF0, 0x0D, 0x40, 0x27, W(ScaleX), 0xA1);
        Emblem(S16(T.LabelX), S16(T.LabelY));
        ScaleEdges();
    }

    /// <summary>
    /// [ORIGINAL] 0C29:0B1F: the base price (cwd), three digits, colour B[025E + picture], shadow colour 0, at
    /// (DS:2BCE + 27h, B[0260 + picture]) (0A96:057A).
    /// </summary>
    private void BasePrice(short price) =>
        _gfx.AddNumber(W(ScaleX + 0x27), B(PriceY + Picture), unchecked((uint)(int)price), 3, B(PriceColour + Picture), 0);

    /// <summary>
    /// [ORIGINAL] 0C29:09BB: cash (five digits, colour B[0240 + 4·picture + player]) and stock (cwd, three digits, colour
    /// B[025C + picture]) of the player in his panel at (x, y) = (DS:2BD0, DS:2BDA): picture A at (x + 0Dh, y + 90h) and
    /// (x + 29h, y + 90h); picture B at (x + 17h, y + 4Ch) and (x + 1Ah, y + 44h) for a buyer, (x + 17h, y + 15h) and
    /// (x + 1Ah, y + 4Ah) otherwise. Shadow colour 0.
    /// </summary>
    private void PlayerNumbers(int p, short role, int cash, short stock)
    {
        short x = PanelX(p), y = PanelY(p);
        ushort colour = B(PlayerColour + (Picture << 2) + p), stockColour = B(StockColour + Picture);
        uint cashValue = unchecked((uint)cash), stockValue = unchecked((uint)(int)stock);
        if (Picture == 0)
        {
            _gfx.AddNumber(W(x + 0x0D), W(y + 0x90), cashValue, 5, colour, 0);
            _gfx.AddNumber(W(x + 0x29), W(y + 0x90), stockValue, 3, stockColour, 0);
            return;
        }
        _gfx.AddNumber(W(x + 0x17), W(y + (role == 0 ? 0x4C : 0x15)), cashValue, 5, colour, 0);
        _gfx.AddNumber(W(x + 0x1A), W(y + (role == 0 ? 0x44 : 0x4A)), stockValue, 3, stockColour, 0);
    }

    /// <summary>
    /// [ORIGINAL] 0C29:0B60: in picture A, for a need DS:43B6[p] &gt; 0 (long), a black line 8 wide (0A69:000B) at
    /// (DS:2BD0 + 18h, DS:2BDA + 89h − need / 10) (long division 0000:4F89, low word).
    /// </summary>
    private void NeedBar(int p)
    {
        int need = NeedOf(p);
        if (need <= 0 || Picture == 1) return;
        _gfx.AddHorizontalLine(W(PanelX(p) + 0x18), W(PanelY(p) + 0x89 - (short)(need / 10)), 8, 0);
    }

    /// <summary>
    /// [ORIGINAL] 0C29:109F: picture A – for i = 0 … stock / 10 − 1: list emptied, heap row 0C29:1163(i, 1), list executed;
    /// then the need bar 0C29:0B60 the same way. Picture B – list emptied, bars 0C29:134D, list executed.
    /// </summary>
    private void Stock(int p)
    {
        if (Picture == 0)
        {
            short rows = Int16Math.Div(StockOf(p), 10);
            for (short i = 0; rows > i; i++)
            {
                _gfx.ClearList();
                HeapRow(p, i, true);
                _gfx.ExecuteList();
            }
            _gfx.ClearList();
            NeedBar(p);
            _gfx.ExecuteList();
            return;
        }
        _gfx.ClearList();
        Bars(p, B(T.Role + p), StockOf(p), S16(T.ShownStock + 2 * p));
        _gfx.ExecuteList();
    }

    /// <summary>
    /// [ORIGINAL] 0C29:1108(direction): picture A – with n = stock / 10 − 1 the row n added (direction 1, bought) or the row
    /// n + 1 removed (sold), then the need bar 0C29:0B60; picture B – the bars 0C29:134D. The stock is the one after the deal.
    /// </summary>
    private void StockChange(short direction, int p, short role, short stock, short shown)
    {
        if (Picture != 0)
        {
            Bars(p, role, stock, shown);
            return;
        }
        short n = Int16Math.S(Int16Math.Div(stock, 10) - 1);
        if (direction == 1) HeapRow(p, n, true);
        else HeapRow(p, Int16Math.S(n + 1), false);
        NeedBar(p);
    }

    /// <summary>
    /// [ORIGINAL] 0C29:1163(row, add): the row of the player's heap by the tables of his shape (jump table 0C29:1276):
    /// player 0 index row mod 25 (DS:02C2…), player 1 index 63h − row (DS:0326…), player 2 index row for row ≤ 9, 63h − row
    /// for row ≥ 5Ah, otherwise 9 (DS:04B6…), player 3 index row for row ≤ 21h, otherwise 20h − (row − 1) mod 21h
    /// (DS:04DE…); each table set gives x1, w1, x2, w2 for 0C29:127E.
    /// </summary>
    private void HeapRow(int p, short row, bool add)
    {
        int table, size, index;
        switch (p)
        {
            case 0: (table, size, index) = (Heap0, 0x19, Int16Math.Mod(row, 0x19)); break;
            case 1: (table, size, index) = (Heap1, 0x64, Int16Math.S(0x63 - row)); break;
            case 2: (table, size, index) = (Heap2, 0x0A, row <= 9 ? row : row >= 0x5A ? Int16Math.S(0x63 - row) : 9); break;
            case 3:
                (table, size, index) = (Heap3, 0x22, row <= 0x21 ? row : Int16Math.S(0x20 - Int16Math.Mod(Int16Math.S(row - 1), 0x21)));
                break;
            default: return;
        }
        HeapLine(p, row, B(table + index), B(table + size + index), B(table + 2 * size + index), B(table + 3 * size + index), add);
    }

    /// <summary>
    /// [ORIGINAL] 0C29:127E(row, x1, w1, x2, w2, add) at y = DS:2BDA + 88h − row: adding draws two lines (0A69:000B) at
    /// DS:2BD0 + x1 (w1 wide, colour B[0566 + p]) and DS:2BD0 + x2 (w2 wide, colour B[0240 + p]); removing restores the row
    /// from the atlas: (40h·p + x1, 88h − row), w1 × 1, to (DS:2BD0 + x1, y).
    /// </summary>
    private void HeapLine(int p, short row, byte x1, byte w1, byte x2, byte w2, bool add)
    {
        ushort y = W(PanelY(p) + 0x88 - row);
        if (add)
        {
            _gfx.AddHorizontalLine(W(PanelX(p) + x1), y, w1, B(HeapColour + p));
            _gfx.AddHorizontalLine(W(PanelX(p) + x2), y, w2, B(PlayerColour + p));
            return;
        }
        _gfx.AddBlit(W((p << 6) + x1), W(0x88 - row), w1, 1, W(PanelX(p) + x1), y);
    }

    /// <summary>
    /// [ORIGINAL] 0C29:134D – picture B, at (DS:2BD0, 17h for a buyer, 80h otherwise): the bar of the stock shown before
    /// (DS:0634[p], colour B[0640 + p]; on card 1 the routine first sets DS:0642 = DAh, player 2's colour), for wares 0–2 with
    /// a need &gt; 0 the bar of need / 10 (colour B[063C + p]), then the bar of stock / 10 in colour 3. The writes of DS:0642
    /// and DS:0634 are the core's (TradeRules.StockBars), made after the request.
    /// </summary>
    private void Bars(int p, short role, short stock, short shown)
    {
        short x = PanelX(p), y = role == 0 ? (short)0x17 : (short)0x80;
        ushort old = p == 2 && Vga ? (ushort)0xDA : B(ShownColour + p);
        Bar(x, y, shown, old);
        int need = NeedOf(p);
        if (Ware != 3 && need > 0) Bar(x, y, (short)(need / 10), B(NeedColour + p));
        Bar(x, y, Int16Math.Div(stock, 10), 3);
    }

    /// <summary>
    /// [ORIGINAL] 0C29:146B(x, y, k, colour): line (0A96:0433) from (x + 20h, y + 13h) to (x + B[056A + k] + 4,
    /// y + B[05CF + k]).
    /// </summary>
    private void Bar(short x, short y, short k, ushort colour) =>
        _gfx.AddLine(W(x + 0x20), W(y + 0x13), W(x + B(BarDx + k) + 4), W(y + B(BarDy + k)), colour);

    /// <summary>
    /// [ORIGINAL] 0C29:0E01 – drawn with the driver at once, not through the list: per player the marker 10h × 28h
    /// (23CD:06D4, two units) at (DS:2BCE + 10h·p, 74h − position + 2 for a buyer): buyer from (10h·p, C8h), seller from
    /// (40h + 10h·p, C8h), each 80h further right while the player is a party of the deal shown (DS:08C6[p] &amp; DS:2BCC);
    /// any other role from (100h, C8h). For a buyer or seller on the scale (0 ≤ position &lt; 64h) the price position / 2 +
    /// DS:455A (2295:0001, three characters into the buffer at the far pointer DS:02A6) in the player's colour
    /// B[0240 + 4·picture + p] (23CD:05C3) at (DS:2BCE + 10h·p, 74h − position + 23h) (23CD:05DD).
    /// </summary>
    private void Markers()
    {
        for (int p = 0; p < 4; p++)
        {
            short position = S16(T.Position + 2 * p);
            byte role = B(T.Role + p);
            bool party = (B(T.PlayerBits + p) & B(T.DealParties)) != 0;
            int sx, adjust = 0;
            if (role == 0)
            {
                sx = (party ? 0x80 : 0) + 0x10 * p;
                adjust = 2;
            }
            else if (role == 1) sx = (party ? 0xC0 : 0x40) + 0x10 * p;
            else sx = 0x100;
            _gfx.Driver.BlitAtlasToPageFixed(W(sx), 0xC8, 2, 0x28, W(ScaleX + 0x10 * p), W(0x74 - position + adjust));
            if (role == 6 || position < 0 || position >= 0x64) continue;
            ushort offset = Gfx16(NumberBuffer), segment = Gfx16(NumberBuffer + 2);
            if (segment != _gfx.DataSegment) throw new NotSupportedException($"Number buffer {segment:X4}:{offset:X4} outside the data segment.");
            short price = Int16Math.S(Int16Math.Div(position, 2) + S16(T.BasePrice));
            _gfx.FormatNumber(unchecked((uint)(int)price), 3, offset, segment);
            _gfx.Driver.Color = B(PlayerColour + (Picture << 2) + p);
            int length = Array.IndexOf(_gfx.Ds, (byte)0, offset) - offset;
            _gfx.Driver.DrawText(W(ScaleX + (p << 4)), W(0x74 - position + 0x23), _gfx.Ds.AsSpan(offset, length));
        }
    }

    /// <summary>
    /// [ORIGINAL] 0C29:0F68: the player's marker field blacked out (0A96:04D7): 10h × 28h at (DS:2BCE + 10h·p,
    /// 74h − position + 2 for a buyer).
    /// </summary>
    private void ClearMarker(int p, short role, short position) =>
        _gfx.AddFilledRect(W(ScaleX + (p << 4)), W(0x74 - position + (role == 0 ? 2 : 0)), 0x10, 0x28, 0);

    /// <summary>
    /// [ORIGINAL] 0C29:0CC1: picture A – the countdown DS:3780 as two columns of at most 1Eh rows from the atlas (130h,
    /// 50h − n), 10h wide, to (DS:2BCA + 0Ah, C1h − n) and, for the part above 1Eh, to (DS:2BCA + 27h, C1h − n); picture B –
    /// a bar (DS:2BCA + 4, 4), min(DS:3780, 39h) wide, 3 high, colour 0Dh.
    /// </summary>
    private void Countdown()
    {
        short left = S16(Clock.Remaining);
        short x = S16(T.CountdownX);
        if (Picture != 0)
        {
            short w = left > 0x39 ? (short)0x39 : left;
            _gfx.AddFilledRect(W(x + 4), 4, W(w), 3, 0x0D);
            return;
        }
        short n = left > 0x1E ? (short)0x1E : left;
        _gfx.AddBlit(0x130, W(0x50 - n), 0x10, W(n), W(x + 0x0A), W(0xC1 - n));
        if (left <= 0x1E) return;
        n = Int16Math.S(left - 0x1E);
        if (n > 0x1E) n = 0x1E;
        _gfx.AddBlit(0x130, W(0x50 - n), 0x10, W(n), W(x + 0x27), W(0xC1 - n));
    }

    /// <summary>
    /// [ORIGINAL] 0C29:0D6A – the step n = DS:3780 + 1 just counted off (nothing for DS:3780 &lt; 0): picture A (n ≤ 3Ch) –
    /// the row of the column (the second column for n &gt; 1Eh, with n − 1Eh) restored from the atlas (4Ah, C1h − n), 10h × 1,
    /// at (DS:2BCA + 0Ah or 27h, C1h − n); picture B (n ≤ 39h) – a black column 3 high at (DS:2BCA + n + 3, 4) (0A69:002C).
    /// </summary>
    private void CountdownStep()
    {
        short left = S16(Clock.Remaining);
        if (left < 0) return;
        short n = Int16Math.S(left + 1), x = S16(T.CountdownX);
        if (Picture != 0)
        {
            if (n > 0x39) return;
            _gfx.AddVerticalLine(W(x + n + 3), 4, 3, 0);
            return;
        }
        if (n > 0x3C) return;
        short row = n > 0x1E ? Int16Math.S(n - 0x1E) : n;
        int column = n > 0x1E ? 0x27 : 0x0A;
        _gfx.AddBlit(0x4A, W(0xC1 - row), 0x10, 1, W(x + column), W(0xC1 - row));
    }
}
