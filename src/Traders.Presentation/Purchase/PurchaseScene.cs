using Traders.Core;
using Traders.Core.Field;
using Traders.Core.Purchase;
using Traders.Core.State;
using Traders.Presentation.Field;
using Traders.Presentation.Menus;
using Traders.Vga.Layers;
using K = Traders.Core.Purchase.PurchaseEventKind;
using P = Traders.Core.Purchase.PurchaseLayout;

namespace Traders.Presentation.Purchase;

/// <summary>
/// Drawing side of the purchase 08CF:0004 (finding docs/routines/08CF_0004_einkauf.md, section "Szene"): plays the requests of
/// the core (<see cref="PurchaseEvent"/>) in the order they are emitted. The purchase's own drawing routines of segment 08CF
/// (shops, decoration and drawn balls, lotto ticket, gems, gem account, clock, cash, oil stock of the shop) and the command list
/// calls the core emits with their arguments append to the command list 0A96 as the original does; the shared routines of
/// segment 0609 (emblem, panel, rockets, oil bar, background, frame) and every <see cref="FieldEvent"/> (pictures, pages,
/// fades, objects, waits) are played by a <see cref="FieldScene"/> on the same layers, frame by frame in step with the core's
/// <see cref="FieldWait.Retrace"/> frames. No game state is changed: the core's data segment is only read, at the moment the
/// request is applied, so every request has to be applied when the core emits it.
/// </summary>
public sealed class PurchaseScene
{
    // Constant tables of the data segment (bytes).
    private const int ShopAtlasX = 0x00EC, ShopAtlasY = 0x00F0; // [shop], picture of a shop in the atlas
    private const int ClockDx = 0x00F4, ClockDy = 0x0113;       // sbyte[0..1Eh], end point of the clock hand
    private const int CashColour = 0x0132;                      // sbyte[player]
    private const int CreditText = 0x013C;                      // near string, label of the gem account

    private readonly GraphicsLayers _gfx;
    private readonly Dgroup _state;

    /// <param name="layers">Graphics layers (command list, pages, atlas).</param>
    /// <param name="state">The core's data segment (read only).</param>
    /// <param name="pictures">Picture resources for 186C:0002.</param>
    public PurchaseScene(GraphicsLayers layers, Dgroup state, IPictureSource? pictures)
    {
        _gfx = layers;
        _state = state;
        Field = new FieldScene(layers, state) { Pictures = pictures };
    }

    /// <summary>The field scene that plays the shared requests (0609 drawing, pictures, pages, fades, objects, waits).</summary>
    public FieldScene Field { get; }

    public GraphicsLayers Layers => _gfx;

    /// <summary>True while a fade, flip or wait still has retrace waits for later frames.</summary>
    public bool Presenting => Field.Presenting;

    /// <summary>
    /// Starts a new frame with the keys held in it: a presenting routine in progress runs on up to its next retrace wait.
    /// Call once per frame before the core processes the frame.
    /// </summary>
    public void NextFrame(IReadOnlyCollection<byte> held) => Field.NextFrame(held);

    /// <summary>Runs a presenting routine in progress to its end.</summary>
    public void Finish() => Field.Finish();

    /// <summary>Plays a request of the field routines or the common frame routines the purchase calls.</summary>
    public void Apply(FieldEvent e) => Field.Apply(e);

    /// <summary>Plays a request of the purchase. Sound is not the scene's.</summary>
    public void Apply(PurchaseEvent e)
    {
        Field.Finish();
        var a = e.Args;
        switch (e.Kind)
        {
            case K.Sound: break; // the sound driver's
            case K.Emblem: Field.Emblem(a[0], a[1], a[2]); break;
            case K.EmptyPanel: Field.SelectionPanel(); break;
            case K.DrawRockets: Field.RocketIcons(); break;
            case K.DrawOilBar: Field.TimeBar(); break;
            case K.DrawBackground: Field.Background(); break;
            case K.DrawFrame: Field.Frame(a[0], a[1], a[2], a[3]); break;
            case K.ClearList: _gfx.ClearList(); break;
            case K.RunList: _gfx.ExecuteList(); break;
            case K.ShowPage: Field.Apply(new FieldEvent(FieldEventKind.ShowPage)); break;
            case K.Blit: _gfx.AddBlit(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4]), W(a[5])); break;
            case K.Tiles: _gfx.AddTiles(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4]), W(a[5]), W(a[6]), W(a[7])); break;
            case K.Line: _gfx.AddLine(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4])); break;
            case K.FillRect: _gfx.AddFilledRect(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4])); break;
            case K.Text: _gfx.AddText(W(a[0]), W(a[1]), W(a[2]), Segment(a[3]), W(a[4]), W(a[5])); break;
            case K.Number: _gfx.AddNumber(W(a[0]), W(a[1]), Long(a[2], a[3]), W(a[4]), W(a[5]), W(a[6])); break;
            case K.Money: _gfx.AddAmount(W(a[0]), W(a[1]), Long(a[2], a[3]), W(a[4]), W(a[5]), W(a[6])); break;
            case K.PriceText: _gfx.AddShadowText(W(a[0]), W(a[1]), W(a[2]), Segment(a[3]), W(a[4]), W(a[5]), W(a[6]), W(a[7])); break;
            case K.PriceNumber:
                _gfx.AddShadowAmount(W(a[0]), W(a[1]), Long(a[2], a[3]), W(a[4]), W(a[5]), W(a[6]), W(a[7]), W(a[8]));
                break;
            case K.DrawCash: Cash(a[0], Long(a[1], a[2])); break;
            case K.DrawShopOil: ShopOil(a[0]); break;
            case K.DrawGemCredit: GemCredit(a[0], a[1]); break;
            case K.DrawGems: Gems(a[0], a[1]); break;
            case K.DrawTicket: Ticket(a[0], a[1], a[2]); break;
            case K.ClearTicket: ClearTicket(a[0]); break;
            case K.DrawShop: Shop(a[0], a[1]); break;
            case K.DrawDecoration: Decoration(a[0], a[1], a[2]); break;
            case K.ClearBalls: ClearBalls(); break;
            case K.DrawClock: Clock(); break;
            default: throw new InvalidOperationException($"purchase event {e.Kind}");
        }
    }

    private static ushort W(int v) => unchecked((ushort)v);

    private static uint Long(short low, short high) => (uint)(W(low) | W(high) << 16);

    /// <summary>The segment of a text request: <see cref="PurchaseRules.DataSegment"/> stands for DS.</summary>
    private ushort Segment(short segment) => segment == PurchaseRules.DataSegment ? _gfx.DataSegment : W(segment);

    private byte B(int at) => _state.U8(W(at));

    private short S8(int at) => _state.S8(W(at));

    private static ushort PanelX(int p, int add) => W((p << 6) + add);

    /// <summary>
    /// [ORIGINAL] 08CF:15C6(shop, x): the shop's picture 40h × 2Ch from the atlas (B[00EC + shop], B[00F0 + shop]) at (x, 50h).
    /// </summary>
    private void Shop(short shop, short x) =>
        _gfx.AddBlit(B(ShopAtlasX + shop), B(ShopAtlasY + shop), 0x40, 0x2C, W(x), 0x50);

    /// <summary>[ORIGINAL] 08CF:15F5(x, y, ball): a ball 20h × 16h from the atlas (0, 16h·ball + 58h) at (x, y).</summary>
    private void Decoration(short x, short y, short ball) =>
        _gfx.AddBlit(0, W(ball * 0x16 + 0x58), 0x20, 0x16, W(x), W(y));

    /// <summary>[ORIGINAL] 08CF:1688: the row of drawn balls (20h, A8h) 80h × 16h tiled with the atlas tile (A0h, 0), 20h × 16h.</summary>
    private void ClearBalls() => _gfx.AddTiles(0xA0, 0, 0x20, 0x16, 0x20, 0xA8, 0x80, 0x16);

    /// <summary>
    /// [ORIGINAL] 08CF:16B4(slot, number): the number's field 0Ah × 0Ah from the atlas (0Ah·number + F0h, CFh) at
    /// (p·40h + 0Bh·slot + 1Ah, 35h), p = DS:37AA (number 0 = empty field).
    /// </summary>
    private void Ticket(short slot, short number, short p) =>
        _gfx.AddBlit(W(number * 0x0A + 0xF0), 0xCF, 0x0A, 0x0A, W((p << 6) + slot * 0x0B + 0x1A), 0x35);

    /// <summary>
    /// [ORIGINAL] 08CF:16F5: the ticket of the player DS:37AA, 2Ch × 0Ah at (p·40h + 1Ah, 35h), tiled with the atlas tile
    /// (F0h, B0h), 20h × 16h.
    /// </summary>
    private void ClearTicket(short p) => _gfx.AddTiles(0xF0, 0xB0, 0x20, 0x16, PanelX(p, 0x1A), 0x35, 0x2C, 0x0A);

    /// <summary>
    /// [ORIGINAL] 08CF:172A: for i = 0 … DS:37AF[p] − 1 (signed) a gem 7 × 7 from the atlas (109h, C6h) at
    /// (p·40h + 8·(i mod 5) + 26h, 8·(i / 5) + 14h).
    /// </summary>
    private void Gems(short p, short gems)
    {
        for (short i = 0; gems > i; i++)
            _gfx.AddBlit(0x109, 0xC6, 7, 7, W((p << 6) + Int16Math.Mod(i, 5) * 8 + 0x26), W(Int16Math.Div(i, 5) * 8 + 0x14));
    }

    /// <summary>
    /// [ORIGINAL] 08CF:1859: the label DS:013C (colour 0Dh, shadow 5) at (p·40h + 26h, 2Ah) and the gem account
    /// DS:300E[p] (cwd, four digits, colour 0, shadow 5) at (p·40h + 34h, 2Ah).
    /// </summary>
    private void GemCredit(short p, short credit)
    {
        _gfx.AddText(PanelX(p, 0x26), 0x2A, CreditText, _gfx.DataSegment, 0x0D, 5);
        _gfx.AddNumber(PanelX(p, 0x34), 0x2A, unchecked((uint)(int)credit), 4, 0, 5);
    }

    /// <summary>
    /// [ORIGINAL] 08CF:18B5, 08CF:1935 (1935 only calls 18B5): the clock face 20h × 20h from the atlas (110h, B0h) at (110h, 6), then with
    /// n = DS:3780 (at most 1Eh) the hand as two lines – (11Fh, 16h) to (10Fh + B[00F4 + n], 7 + B[0113 + n]) in colour 0Fh and
    /// (120h, 15h) to (110h + B[00F4 + n], 6 + B[0113 + n]) in colour 5 (table bytes signed).
    /// </summary>
    private void Clock()
    {
        short n = _state.S16(P.Remaining);
        if (n > 0x1E) n = 0x1E;
        _gfx.AddBlit(0x110, 0xB0, 0x20, 0x20, 0x110, 6);
        short dx = S8(ClockDx + n), dy = S8(ClockDy + n);
        _gfx.AddLine(0x11F, 0x16, W(dx + 0x10F), W(dy + 7), 0x0F);
        _gfx.AddLine(0x120, 0x15, W(dx + 0x110), W(dy + 6), 5);
    }

    /// <summary>
    /// [ORIGINAL] 08CF:193E: the cash DS:3CD2[p] (long, five digits, "$", colour B[0132 + p] signed, shadow 5) at
    /// (p·40h + 26h, 11h).
    /// </summary>
    private void Cash(short p, uint cash) => _gfx.AddAmount(PanelX(p, 0x26), 0x11, cash, 5, W(S8(CashColour + p)), 5);

    /// <summary>
    /// [ORIGINAL] 08CF:1981: the oil stock of the shop DS:4B98 (cwd, three digits, colour 5, shadow 0Ah) at (DS:15B0 + 2Bh,
    /// 77h).
    /// </summary>
    private void ShopOil(short oil) =>
        _gfx.AddNumber(W(_state.S16(P.OilX) + 0x2B), 0x77, unchecked((uint)(int)oil), 3, 5, 0x0A);
}
