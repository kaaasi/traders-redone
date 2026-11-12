using Traders.Core.Field;
using Traders.Core.State;
using Traders.Vga.Layers;

namespace Traders.Presentation.Field;

/// <summary>
/// Drawing side of the field layer 0609 (finding docs/routines/0609_1256_rahmen.md): background, frame and the player's
/// emblem with rank, and the drawing requests of the core's cursor step (<see cref="FieldEvent"/>). Every routine only
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
        if (_state.U8(W(Marker + p)) == 1) _gfx.AddBlit8(0xF8, 0xC6, 8, W((p << 6) + 0x20), 0x2A);
    }

    /// <summary>
    /// Plays a drawing request of the cursor step. <see cref="FieldEventKind.DrawCommand"/> is the 0A96:0030 call of
    /// 0609:1B72 with its six arguments; <see cref="FieldEventKind.DrawRank"/> is 0609:1854. Sound is not the scene's.
    /// </summary>
    public void Apply(FieldEvent e)
    {
        switch (e.Kind)
        {
            case FieldEventKind.DrawCommand:
                var a = e.Args;
                _gfx.AddBlit(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4]), W(a[5]));
                break;
            case FieldEventKind.DrawRank:
                Rank(e.Args[0], e.Args[1]);
                break;
            case FieldEventKind.DrawSelection:
                // [APPROX] 0609:0C73 (selection display, calls 18E5/193C/1978/0D02 and 0A96:0525/05FB) is not part of this
                // lane's part 3; the request is dropped until it is ported.
                break;
            case FieldEventKind.Sound:
                break;
        }
    }
}
