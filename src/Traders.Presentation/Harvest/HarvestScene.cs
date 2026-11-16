using Traders.Core.Field;
using Traders.Core.Harvest;
using Traders.Core.State;
using Traders.Presentation.Field;
using Traders.Presentation.Menus;
using Traders.Vga.Layers;

namespace Traders.Presentation.Harvest;

/// <summary>
/// Drawing side of the harvest 22A3:02D3 (finding docs/routines/22A3_02D3_ernte.md): the requests it shares with the planting
/// phase (board, panels, objects, fades, pages, waits) are played by a <see cref="FieldScene"/> on the same layers; the
/// harvest's own requests (<see cref="HarvestEvent"/>: font, the typed weather notice, tiles, goods, numbers, the bar) append
/// entries to the command list 0A96 or set the font. No game state is changed; the core's data segment is only read (the
/// pointer table of the notice DS:3BA4).
/// </summary>
public sealed class HarvestScene
{
    private readonly GraphicsLayers _gfx;
    private readonly Dgroup _state;
    private (short X, short Y, short W, short Lines, short Colour) _typing;

    /// <param name="layers">Graphics layers (command list, pages, atlas).</param>
    /// <param name="state">The core's data segment (read only).</param>
    /// <param name="pictures">Picture resources for 186C:0002.</param>
    public HarvestScene(GraphicsLayers layers, Dgroup state, IPictureSource? pictures)
    {
        _gfx = layers;
        _state = state;
        Field = new FieldScene(layers, state) { Pictures = pictures };
    }

    /// <summary>The field scene that plays the shared requests and the frame-stepped output routines.</summary>
    public FieldScene Field { get; }

    public GraphicsLayers Layers => _gfx;

    /// <summary>True while a fade, flip or wait still has retrace waits for later frames.</summary>
    public bool Presenting => Field.Presenting;

    /// <summary>Starts a new frame with the keys held in it (see <see cref="FieldScene.NextFrame"/>).</summary>
    public void NextFrame(IReadOnlyCollection<byte> held) => Field.NextFrame(held);

    /// <summary>Runs a presenting routine in progress to its end.</summary>
    public void Finish() => Field.Finish();

    public void Apply(FieldEvent e) => Field.Apply(e);

    public void Apply(HarvestEvent e)
    {
        Field.Finish();
        var a = e.Args;
        switch (e.Kind)
        {
            case HarvestEventKind.SetFont: Font(a[0]); break;
            case HarvestEventKind.TypeLines: _typing = (a[0], a[1], a[2], a[3], a[6]); break;
            case HarvestEventKind.TypeCharacter: TypeCharacter((byte)a[0], a[1], a[2], a[3]); break;
            case HarvestEventKind.TileArea: Field.Interior(a[0], a[1], a[2], a[3]); break;
            case HarvestEventKind.DrawEmptyPanel: Field.SelectionPanel(); break;
            case HarvestEventKind.DrawGoods: Field.GoodIcon(a[0], a[1], a[2]); break;
            case HarvestEventKind.Number:
                _gfx.AddNumber(W(a[0]), W(a[1]), (uint)(W(a[2]) | W(a[3]) << 16), W(a[4]), W(a[5]), W(a[6]));
                break;
            case HarvestEventKind.Line: _gfx.AddLine(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4])); break;
            case HarvestEventKind.FillRect: _gfx.AddFilledRect(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4])); break;
            default: throw new InvalidOperationException($"harvest event {e.Kind}");
        }
    }

    private static ushort W(int v) => unchecked((ushort)v);

    private short Ds16(int at) => (short)(_gfx.Ds[at] | _gfx.Ds[at + 1] << 8);

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

    /// <summary>
    /// [ORIGINAL] 0A69:01AD, the drawing of one character of 0A69:0288/0243: line i at y + i·([34A4] + [34A4]/5) (signed),
    /// centred by 0243 at x + 1 + (w − length·[3CA8]) / 2 (unsigned shift), character k at + k·[3CA8]; DS:0140 = character,
    /// then text 0A96:0525(x, y, DS:0140, colour, −1). The list was emptied by the event before.
    /// </summary>
    private void TypeCharacter(byte c, short line, short index, short length)
    {
        short advance = Ds16(GraphicsLayers.FontAdvance), height = Ds16(GraphicsLayers.FontLineHeight);
        ushort y = W(_typing.Y + (short)(height + height / 5) * line);
        ushort x0 = W(_typing.X + 1 + (W(_typing.W - (short)(length * advance)) >> 1));
        _gfx.Ds[GraphicsLayers.TypewriterChar] = c;
        _gfx.AddText(W(x0 + index * advance), y, GraphicsLayers.TypewriterChar, _gfx.DataSegment, W(_typing.Colour), 0xFFFF);
    }
}
