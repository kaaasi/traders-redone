using Traders.Core;
using Traders.Core.Field;
using Traders.Core.State;
using Traders.Vga.Layers;

namespace Traders.Presentation.Field;

/// <summary>
/// Drawing side of the field layer 0609 (findings docs/routines/0609_1256_rahmen.md and 0609_0C73_anbauszene.md):
/// background, frame, the player's emblem with rank, the planting phase's field, panels and status lines, and the drawing
/// requests of the core's cursor step and planting phase (<see cref="FieldEvent"/>). Every routine only
/// appends entries to the command list 0A96 of the graphics layers; it changes no game state. Values the original reads
/// from the data segment (player at turn DS:37AA, rank DS:3D42, emblem offsets DS:00D8, flag DS:43FA) are read from the
/// core's data segment.
/// </summary>
public sealed class FieldScene
{
    private const int CurrentPlayer = 0x37AA; // byte, signed
    private const int RankTable = 0x3D42;         // byte[4], unsigned
    private const int RankOffsetY = 0x00D8;   // word[4], y offset of the rank badge in the emblem
    private const int Marker = 0x43FA;        // byte[4], 1 = marker next to the rank icon

    private readonly GraphicsLayers _gfx;
    private readonly Dgroup _state;

    /// <param name="layers">Graphics layers (command list, pages, atlas).</param>
    /// <param name="state">The core's data segment (read only).</param>
    public FieldScene(GraphicsLayers layers, Dgroup state)
    {
        _gfx = layers;
        _state = state;
    }

    public GraphicsLayers Layers => _gfx;

    private static ushort W(int v) => unchecked((ushort)v);

    /// <summary>Player at turn, sign-extended as the original's cbw.</summary>
    private int Player => _state.S8(CurrentPlayer);

    /// <summary>[ORIGINAL] 0609:1ED9: the whole screen 140h × C8h tiled with the atlas tile (120h, 58h), 20h × 16h.</summary>
    public void Background() => _gfx.AddTiles(0x120, 0x58, 0x20, 0x16, 0, 0, 0x140, 0xC8);

    /// <summary>
    /// [ORIGINAL] 0609:1256(x, y, w, h): frame with four corners 10h × 16h (atlas 80h/90h, 0/42h), top and bottom edge
    /// 0Bh high tiled from (120h, 9Ah)/(120h, A5h), left and right edge 10h wide tiled from (80h, 16h)/(90h, 16h), then
    /// the interior 0609:1F04(x + 10h, y + 0Bh, w − 20h, h − 16h). All arithmetic 16-bit.
    /// </summary>
    public void Frame(short x, short y, short w, short h)
    {
        _gfx.AddBlit(0x80, 0, 0x10, 0x16, W(x), W(y));
        _gfx.AddBlit(0x90, 0, 0x10, 0x16, W(x + w - 0x10), W(y));
        _gfx.AddBlit(0x80, 0x42, 0x10, 0x16, W(x), W(y + h - 0x16));
        _gfx.AddBlit(0x90, 0x42, 0x10, 0x16, W(x + w - 0x10), W(y + h - 0x16));
        _gfx.AddTiles(0x120, 0x9A, 0x20, 0x0B, W(x + 0x10), W(y), W(w - 0x20), 0x0B);
        _gfx.AddTiles(0x120, 0xA5, 0x20, 0x0B, W(x + 0x10), W(y + h - 0x0B), W(w - 0x20), 0x0B);
        _gfx.AddTiles(0x80, 0x16, 0x10, 0x2C, W(x), W(y + 0x16), 0x10, W(h - 0x2C));
        _gfx.AddTiles(0x90, 0x16, 0x10, 0x2C, W(x + w - 0x10), W(y + 0x16), 0x10, W(h - 0x2C));
        Interior((short)(x + 0x10), (short)(y + 0x0B), (short)(w - 0x20), (short)(h - 0x16));
    }

    /// <summary>[ORIGINAL] 0609:1F04(x, y, w, h): the area tiled with the atlas tile (F0h, B0h), 20h × 16h.</summary>
    public void Interior(short x, short y, short w, short h) => _gfx.AddTiles(0xF0, 0xB0, 0x20, 0x16, W(x), W(y), W(w), W(h));

    /// <summary>
    /// [ORIGINAL] 0609:1879(x, y, rank): emblem of the player at turn p = DS:37AA, 30h × 2Ch from atlas (C0h, 2Ch·p); for
    /// rank &gt; 0 (signed) the badge 10h × 0Bh from atlas ((rank − 1)·10h, 0Bh·p + C6h) at (x + 10h, y + DS:00D8[p]).
    /// </summary>
    public void Emblem(short x, short y, short rank)
    {
        int p = Player;
        _gfx.AddBlit(0xC0, W(p * 0x2C), 0x30, 0x2C, W(x), W(y));
        if (rank <= 0) return;
        ushort dy = W(y + _state.U16(W(RankOffsetY + 2 * p)));
        _gfx.AddBlit(W((rank - 1) << 4), W(p * 0x0B + 0xC6), 0x10, 0x0B, W(x + 0x10), dy);
    }

    /// <summary>
    /// [ORIGINAL] 0609:1854(x, y): <see cref="Emblem"/> with the rank DS:3D42[p] (unsigned byte), then 0609:1A61: if
    /// DS:43FA[p] = 1 the 8-pixel marker from atlas (F8h, C6h), 8 rows, at ((p &lt;&lt; 6) + 20h, 2Ah).
    /// </summary>
    public void Rank(short x, short y)
    {
        int p = Player;
        Emblem(x, y, _state.U8(W(RankTable + p)));
        PurchaseMarker();
    }


    // ------------------------------------------------------------------ planting phase 0609:0004 (finding 0609_0004_anbau.md)

    private const int OriginX = 0x451E, OriginY = 0x4528; // word, top left corner of the field on screen
    private const int Terrain = 0x45DA;                   // sbyte[x*6+y], 0..3
    private const int TilePieceX = 0x00D0, TilePieceY = 0x00D4; // byte[4], quarter tiles by neighbours
    private const int OwnerColour = 0x00C8;               // word[owner], colour of the owner's dotted border
    private const int SaleX = 0x3CAF, SaleY = 0x3CB3;     // sbyte[owner], field marked for sale
    private const int Selection = 0x2E94;                 // sbyte[4], selected kind
    private const int CursorX = 0x304E, CursorY = 0x305A; // sbyte[4]
    private const int Rockets = 0x4BAC;                   // word[4]
    private const int Stock = 0x3CF4;                     // word[4][4]
    private const int OwnedKinds = 0x2EE7;                // byte[4][4]
    private const int Cost = 0x48A2;                      // sbyte, units per planting
    private const int Yield = 0x2F26;                     // sbyte, units per field (divisor of the bars)
    private const int NeedOf = 0x08E7, MadeOf = 0x08E4;   // sbyte[], goods of a kind
    private const int LandPrice = 0x3746;                 // word
    private const int TimeLimit = 0x45BA, TimeLeft = 0x4520; // byte; word[4]
    private const int NoTime = 0x1582;                    // byte, 1 = second round
    private const int TextNone = 0x37F0, TextWeather = 0x37E8, TextLimit = 0x37EC; // far pointers

    private short S8(int offset) => _state.S8(W(offset));

    private short S16(int offset) => _state.S16(W(offset));

    private ushort U16(int offset) => _state.U16(W(offset));

    /// <summary>x of the player's panel: (DS:37AA &lt;&lt; 6) + d.</summary>
    private ushort PanelX(int d) => W((Player << 6) + d);

    /// <summary>16-bit idiv of the original; a quotient that does not fit (or a zero divisor) traps in the original.</summary>
    private static short Idiv(short a, short b)
    {
        if (b == 0 || (a == short.MinValue && b == -1))
            throw new DivideByZeroException($"idiv {a}/{b}: the original traps (INT 0).");
        return (short)(a / b);
    }

    /// <summary>
    /// [ORIGINAL] 0609:13C5(x, y, w, h): border around the field – corners 10h × 4/0Ah from atlas (120h/130h, 6Eh/72h), for
    /// i = 0 … w/20h − 1 (idiv, signed loop) the top piece (120h, 7Ch + 0Eh·random(1)) 20h × 4 and the bottom piece (120h,
    /// 80h + 0Eh·random(1)) 20h × 0Ah, then the left and right edge h high tiled from (120h/130h, 0), tile 10h × 58h.
    /// The random(1) results (0 or 1, top and bottom per column) are drawn by the core (Planting.Border) and passed in
    /// <paramref name="draws"/>.
    /// </summary>
    public void Border(short x, short y, short w, short h, ReadOnlySpan<short> draws)
    {
        _gfx.AddBlit(0x120, 0x6E, 0x10, 4, W(x - 0x10), W(y - 4));
        _gfx.AddBlit(0x130, 0x6E, 0x10, 4, W(x + w), W(y - 4));
        _gfx.AddBlit(0x120, 0x72, 0x10, 0x0A, W(x - 0x10), W(y + h));
        _gfx.AddBlit(0x130, 0x72, 0x10, 0x0A, W(x + w), W(y + h));
        short last = (short)(Idiv(w, 0x20) - 1);
        for (short i = 0; i <= last; i++)
        {
            ushort dx = W(x + (i << 5));
            _gfx.AddBlit(0x120, W(draws[2 * i] * 0x0E + 0x7C), 0x20, 4, dx, W(y - 4));
            _gfx.AddBlit(0x120, W(draws[2 * i + 1] * 0x0E + 0x80), 0x20, 0x0A, dx, W(y + h));
        }
        _gfx.AddTiles(0x120, 0, 0x10, 0x58, W(x - 0x10), W(y), 0x10, W(h));
        _gfx.AddTiles(0x130, 0, 0x10, 0x58, W(x + w), W(y), 0x10, W(h));
    }

    private static int Cell(int x, int y) => W(x * 6 + y);

    /// <summary>
    /// [ORIGINAL] 0609:1540(x, y): the field's tile at (DS:451E + 20h·x, DS:4528 + 16h·y), nothing outside the field
    /// (0609:10A7). Terrain 3: empty (kind −2) (A0h, 42h), HQ (kind 4) (20h, 42h + 16h·owner), else (20h, 16h·kind);
    /// terrain 2: empty (0, 58h + 16h·((x·y) mod 3)), else (0, 16h·kind); other terrain: four quarters 10h × 0Bh chosen by
    /// the neighbours of the same terrain (tables DS:00D0/00D4, left/right +1, up/down +2), base 0 for terrain 0 and 20h
    /// otherwise, then a planted kind masked from (base + 40h, 16h·kind) with the mask at (base + 90h, 16h·kind).
    /// </summary>
    public void Tile(short x, short y)
    {
        ushort dx = W((x << 5) + S16(OriginX)), dy = W(y * 0x16 + S16(OriginY));
        if (!FieldGrid.Inside(_state, x, y)) return;
        int c = Cell(x, y);
        short kind = S8(FieldGrid.Kind + c), owner = S8(FieldGrid.Owner + c), terrain = S8(Terrain + c);
        switch (terrain)
        {
            case 3:
                if (kind == -2) _gfx.AddBlit(0xA0, 0x42, 0x20, 0x16, dx, dy);
                else if (kind == 4) _gfx.AddBlit(0x20, W(owner * 0x16 + 0x42), 0x20, 0x16, dx, dy);
                else _gfx.AddBlit(0x20, W(kind * 0x16), 0x20, 0x16, dx, dy);
                return;
            case 2:
                if (kind == -2) _gfx.AddBlit(0, W((short)(x * y) % 3 * 0x16 + 0x58), 0x20, 0x16, dx, dy);
                else _gfx.AddBlit(0, W(kind * 0x16), 0x20, 0x16, dx, dy);
                return;
        }
        int b = terrain == 0 ? 0 : 0x20;
        int up = 0, left = 0, right = 0, down = 0;
        if (y > 0 && S8(Terrain + c - 1) == terrain) up = 2;
        if (x > 0 && S8(Terrain + c - 6) == terrain) left = 1;
        if (Int16Math.S(S16(FieldGrid.Columns) - 1) > x && S8(Terrain + c + 6) == terrain) right = 1;
        if (Int16Math.S(S16(FieldGrid.Rows) - 1) > y && S8(Terrain + c + 1) == terrain) down = 2;
        void Quarter(int i, int ox, int oy) =>
            _gfx.AddBlit(W(_state.U8(TilePieceX + i) + b + ox), W(_state.U8(TilePieceY + i) + oy), 0x10, 0x0B, W(dx + ox), W(dy + oy));
        Quarter(up + left, 0, 0);
        Quarter(up + right, 0x10, 0);
        Quarter(down + left, 0, 0x0B);
        Quarter(down + right, 0x10, 0x0B);
        if (kind != -2)
            _gfx.AddMaskedBlit(W(b + 0x40), W(kind * 0x16), W(b + 0x90), W(kind * 0x16), 0x20, 0x16, dx, dy);
    }

    /// <summary>0609:1E6B(x, y, owner) – 1 if the field is outside or not the owner's (edge of the owner's land).</summary>
    // [ORIGINAL] 0609:1E6B
    private bool Edge(short x, short y, short owner) =>
        !FieldGrid.Inside(_state, x, y) || S8(FieldGrid.Owner + Cell(x, y)) != owner;

    /// <summary>
    /// [ORIGINAL] 0609:1CF4(x, y): for an owned field the dotted border (line pattern 5555h) in the owner's colour DS:00C8
    /// on every side towards another owner or the edge (left, right, top, bottom; 0609:1E6B), pattern FFFFh again, and a
    /// solid cross over the field marked for sale (DS:3CAF/3CB3 of the owner).
    /// </summary>
    public void OwnerBorder(short x, short y)
    {
        int c = Cell(x, y);
        short owner = S8(FieldGrid.Owner + c);
        if (owner == -1) return;
        ushort l = W((x << 5) + S16(OriginX)), t = W(y * 0x16 + S16(OriginY));
        ushort r = W(l + 0x1F), b = W(t + 0x15);
        ushort colour = U16(OwnerColour + (owner << 1));
        _gfx.AddLinePattern(0x5555);
        if (Edge((short)(x - 1), y, owner)) _gfx.AddLine(l, t, l, b, colour);
        if (Edge((short)(x + 1), y, owner)) _gfx.AddLine(r, t, r, b, colour);
        if (Edge(x, (short)(y - 1), owner)) _gfx.AddLine(l, t, r, t, colour);
        if (Edge(x, (short)(y + 1), owner)) _gfx.AddLine(l, b, r, b, colour);
        _gfx.AddLinePattern(0xFFFF);
        if (S8(SaleX + owner) == x && S8(SaleY + owner) == y)
        {
            _gfx.AddLine(l, t, r, b, colour);
            _gfx.AddLine(l, b, r, t, colour);
        }
    }

    /// <summary>[ORIGINAL] 0609:1C83(x, y): inside the field 0609:1540 and 0609:1CF4, else nothing.</summary>
    public void Field(short x, short y)
    {
        if (!FieldGrid.Inside(_state, x, y)) return;
        Tile(x, y);
        OwnerBorder(x, y);
    }

    /// <summary>[ORIGINAL] 0609:1CB1(x, y): 0609:1C83 for the field, left, right, above and below.</summary>
    public void FieldAndNeighbours(short x, short y)
    {
        Field(x, y);
        Field((short)(x - 1), y);
        Field((short)(x + 1), y);
        Field(x, (short)(y - 1));
        Field(x, (short)(y + 1));
    }

    /// <summary>
    /// [ORIGINAL] 0609:1A03: picture of the terrain under the cursor of the player at turn, 30h × 2Ch from (F0h,
    /// 2Ch·terrain) at (p·40h + 20h, 6), then the purchase marker 0609:1A61.
    /// </summary>
    public void Panel()
    {
        int p = Player;
        short terrain = S8(Terrain + Cell(S8(CursorX + p), S8(CursorY + p)));
        _gfx.AddBlit(0xF0, W(terrain * 0x2C), 0x30, 0x2C, PanelX(0x20), 6);
        PurchaseMarker();
    }

    /// <summary>[ORIGINAL] 0609:1A61: if DS:43FA[p] = 1 the marker 8 wide from (F8h, C6h), 8 rows, at (p·40h + 20h, 2Ah).</summary>
    public void PurchaseMarker()
    {
        if (_state.U8(W(Marker + Player)) == 1) _gfx.AddBlit8(0xF8, 0xC6, 8, PanelX(0x20), 0x2A);
    }

    /// <summary>[ORIGINAL] 0609:1A97: the purchase marker covered with (C0h, 24h), 8 × 8, at (p·40h + 20h, 2Ah).</summary>
    public void PurchaseUsed() => _gfx.AddBlit8(0xC0, 0x24, 8, PanelX(0x20), 0x2A);

    /// <summary>
    /// [ORIGINAL] 0609:1AC0: time bar of the player at turn – case 10h × 2Ch from (110h, D0h) at (p·40h + 10h, 6), the
    /// empty bar 10h × 20h at y 7 tiled from (110h, F1h), tile 10h × 8, and the filled part t = DS:4520[p] (40 without
    /// time limit DS:45BA ≠ 1) high from y 2Fh − t tiled from (110h, 8p + D1h).
    /// </summary>
    public void TimeBar()
    {
        int p = Player;
        short t = _state.U8(TimeLimit) == 1 ? S16(TimeLeft + 2 * p) : (short)0x28;
        _gfx.AddBlit(0x110, 0xD0, 0x10, 0x2C, PanelX(0x10), 6);
        _gfx.AddTiles(0x110, 0xF1, 0x10, 8, PanelX(0x10), 7, 0x10, 0x20);
        _gfx.AddTiles(0x110, W((p << 3) + 0xD1), 0x10, 8, PanelX(0x10), W(0x2F - t), 0x10, W(t));
    }

    /// <summary>[ORIGINAL] 0609:18E5: empty selection panel 30h × 2Ch from (C0h, B0h) at (p·40h + 20h, 6).</summary>
    public void SelectionPanel() => _gfx.AddBlit(0xC0, 0xB0, 0x30, 0x2C, PanelX(0x20), 6);

    /// <summary>[ORIGINAL] 0609:1912(good, x, y): goods icon 10h × 7 from (10h·good + C0h, DCh).</summary>
    public void GoodIcon(short good, short x, short y) => _gfx.AddBlit(W((good << 4) + 0xC0), 0xDC, 0x10, 7, W(x), W(y));

    /// <summary>[ORIGINAL] 0609:193C: icon 10h × 0Eh of the selected kind from (10h·DS:2E94[p] + 40h, EEh) at (p·40h + 31h, 0Ah).</summary>
    public void SelectionIcon() => _gfx.AddBlit(W((S8(Selection + Player) << 4) + 0x40), 0xEE, 0x10, 0x0E, PanelX(0x31), 0x0A);

    /// <summary>
    /// [ORIGINAL] 0609:1978: one rocket 7 × 9 from (F0h, C6h) per rocket DS:4BAC[p] (signed loop, i ≤ n − 1): the first five
    /// at (p·40h + 8i + 26h, 18h), the rest at (p·40h + 8(i − 5) + 26h, 22h).
    /// </summary>
    public void RocketIcons()
    {
        short last = Int16Math.S(S16(Rockets + 2 * Player) - 1);
        for (int i = 0; i <= last; i++)
        {
            if (i > 4) _gfx.AddBlit(0xF0, 0xC6, 7, 9, W((Player << 6) + ((i - 5) << 3) + 0x26), 0x22);
            else _gfx.AddBlit(0xF0, 0xC6, 7, 9, W((Player << 6) + (i << 3) + 0x26), 0x18);
        }
    }

    /// <summary>
    /// [ORIGINAL] 0609:0DF9(stock, need, extra, y): bar at (p·40h + 2Eh, y), 5 high, stock/DS:2F26 + 1 wide (at most 1Fh),
    /// colour 8 if need &gt; stock, 0Eh if need + extra &gt; stock, else 0Dh; a mark (line colour 2 from y − 1 to y + 6) at
    /// need/DS:2F26 if that is ≤ 1Eh. Divisions are idiv; DS:2F26 = 0 traps in the original.
    /// </summary>
    public void StockBar(short stock, short need, short extra, short y)
    {
        short unit = S8(Yield);
        short w = (short)(Idiv(stock, unit) + 1);
        if (w > 0x1F) w = 0x1F;
        short mark = Idiv(need, unit);
        ushort colour = need > stock ? (ushort)8 : Int16Math.S(need + extra) > stock ? (ushort)0x0E : (ushort)0x0D;
        _gfx.AddFilledRect(PanelX(0x2E), W(y), W(w), 5, colour);
        if (mark > 0x1E) return;
        ushort lx = W((Player << 6) + mark + 0x2E);
        _gfx.AddLine(lx, W(y - 1), lx, W(y + 6), 2);
    }

    /// <summary>
    /// [ORIGINAL] 0609:0D02: for the selected kind s = DS:2E94[p] the goods a = DS:08E7[s] and b = DS:08E4[s] (icons
    /// 0609:1912 at y 1Bh and 23h), then the bars 0609:0DF9(stock a, fields of kind b · DS:2F26, DS:48A2, 1Ch) and
    /// (stock b, fields of kind s · DS:2F26, DS:2F26, 24h).
    /// </summary>
    public void GoodsBars()
    {
        int p = Player;
        short s = S8(Selection + p);
        short a = S8(NeedOf + s), b = S8(MadeOf + s);
        GoodIcon(a, Int16Math.S((p << 6) + 0x20), 0x1B);
        GoodIcon(b, Int16Math.S((p << 6) + 0x20), 0x23);
        short unit = S8(Yield);
        StockBar(S16(Stock + (p << 3) + (a << 1)), Int16Math.S(S8(OwnedKinds + (p << 2) + b) * unit), S8(Cost), 0x1C);
        StockBar(S16(Stock + (p << 3) + (b << 1)), Int16Math.S(S8(OwnedKinds + (p << 2) + s) * unit), unit, 0x24);
    }

    /// <summary>
    /// [ORIGINAL] 0609:0C73: selection display of the player at turn – panel 0609:18E5, icon 0609:193C, then by the kind
    /// DS:2E94[p]: −1 the text at the far pointer DS:37F0 (colour 8) at (p·40h + 2Dh, 23h); 3 the rockets 0609:1978; 4 the
    /// land price DS:3746 (cwd) as "$" amount, 4 digits, colour 0Dh at (p·40h + 2Ah, 23h); otherwise 0609:0D02.
    /// </summary>
    public void SelectionDisplay()
    {
        SelectionPanel();
        SelectionIcon();
        switch (S8(Selection + Player))
        {
            case -1:
                _gfx.AddText(PanelX(0x2D), 0x23, U16(TextNone), U16(TextNone + 2), 8, 0xFFFF);
                break;
            case 3:
                RocketIcons();
                break;
            case 4:
                _gfx.AddAmount(PanelX(0x2A), 0x23, unchecked((uint)(int)S16(LandPrice)), 4, 0x0D, 0xFFFF);
                break;
            default:
                GoodsBars();
                break;
        }
    }

    /// <summary>
    /// [ORIGINAL] 0609:0204 (drawing part, before its 21FD:060D): the list is emptied, then the weather line (text at the far
    /// pointer DS:37E8, colour 5, shadow 0Bh at offset (−1, 1)) at (112h, 11h) with DS:48A2 (2 digits, colour 2, shadow
    /// 0Ah) at (118h, 18h), and the limit line (DS:37EC) at (112h, 1Fh) with DS:2F26 at (118h, 26h). In the second round
    /// (DS:1582 = 1) the area (10h, 6, 120h, 2Ch) is cleared with 0609:1F04 instead.
    /// [APPROX] Second round only (0609:0080, lane R9): the typed notice before it (21FD:05DB, 0A69:0288 from DS:37E0,
    /// 21FD:0185(50)) is not drawn.
    /// </summary>
    public void Status()
    {
        _gfx.ClearList();
        if (_state.U8(NoTime) == 1)
        {
            Interior(0x10, 6, 0x120, 0x2C);
            return;
        }
        _gfx.AddShadowText(0x112, 0x11, U16(TextWeather), U16(TextWeather + 2), 5, 0x0B, 0xFFFF, 1);
        _gfx.AddShadowNumber(0x118, 0x18, unchecked((uint)(int)S8(Cost)), 2, 2, 0x0A, 0xFFFF, 1);
        _gfx.AddShadowText(0x112, 0x1F, U16(TextLimit), U16(TextLimit + 2), 5, 0x0B, 0xFFFF, 1);
        _gfx.AddShadowNumber(0x118, 0x26, unchecked((uint)(int)S8(Yield)), 2, 2, 0x0A, 0xFFFF, 1);
    }

    /// <summary>
    /// Plays a drawing request of the core (cursor step, planting phase): the drawing routines of segment 0609 above with
    /// the event's arguments, and <see cref="FieldEventKind.DrawCommand"/> as the 0A96:0030 call of 0609:1B72. Requests of
    /// the output layers (pages, palettes, objects, list execution, waits) and sound are not the scene's.
    /// </summary>
    public void Apply(FieldEvent e)
    {
        var a = e.Args;
        switch (e.Kind)
        {
            case FieldEventKind.DrawCommand: _gfx.AddBlit(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4]), W(a[5])); break;
            case FieldEventKind.DrawRank: Rank(a[0], a[1]); break;
            case FieldEventKind.DrawSelection: SelectionDisplay(); break;
            case FieldEventKind.DrawBackground: Background(); break;
            case FieldEventKind.DrawFrame: Frame(a[0], a[1], a[2], a[3]); break;
            case FieldEventKind.DrawBorder: Border(a[0], a[1], a[2], a[3], a.AsSpan(4)); break;
            case FieldEventKind.DrawField: Field(a[0], a[1]); break;
            case FieldEventKind.DrawFieldAndNeighbours: FieldAndNeighbours(a[0], a[1]); break;
            case FieldEventKind.DrawPanel: Panel(); break;
            case FieldEventKind.DrawPurchaseUsed: PurchaseUsed(); break;
            case FieldEventKind.DrawPlayer: TimeBar(); break;
            case FieldEventKind.DrawStatus: Status(); break;
        }
    }
}
