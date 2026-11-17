using Traders.Core.FatMike;
using Traders.Core.Field;
using Traders.Core.State;
using Traders.Presentation.Field;
using Traders.Presentation.Menus;
using Traders.Vga.Layers;

namespace Traders.Presentation.FatMike;

/// <summary>
/// Drawing side of Fat Mike 1F63:000F and the contest 1F63:0B03 (findings docs/routines/1F63_000F_fatmike.md and
/// 1F63_0B03_wettbewerb.md): the requests shared with the field layer (pictures, objects, palettes, fades, pages, list, waits,
/// the board of the punishment and the second field round 0609:0080) are played by a <see cref="FieldScene"/> on the same
/// layers; the segment's own requests (<see cref="FatMikeEvent"/>: copies, tiles, lines, shadowed texts and numbers, the
/// indicator 1F63:104B, the emblem, font, the typewriter, objects off, the prize, the flashing colours) append entries to the
/// command list 0A96, set the font or one DAC colour. No game state is changed; the core's data segment is only read.
/// </summary>
public sealed class FatMikeScene
{
    private readonly GraphicsLayers _gfx;
    private (short X, short Y, short W, short Colour) _typing;

    /// <param name="layers">Graphics layers (command list, pages, atlas).</param>
    /// <param name="state">The core's data segment (read only).</param>
    /// <param name="pictures">Picture resources for 186C:0002.</param>
    public FatMikeScene(GraphicsLayers layers, Dgroup state, IPictureSource? pictures)
    {
        _gfx = layers;
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

    /// <summary>A request shared with the field layer (also those of the second field round's host).</summary>
    public void Apply(FieldEvent e) => Field.Apply(e);

    /// <summary>A request of segment 1F63; a presenting routine still in progress is finished first.</summary>
    public void Apply(FatMikeEvent e)
    {
        Field.Finish();
        var a = e.Args;
        switch (e.Kind)
        {
            case FatMikeEventKind.CopyAtlas: _gfx.AddAtlasCopy(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4]), W(a[5])); break;
            case FatMikeEventKind.CopyPage: _gfx.AddPageCopy(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4]), W(a[5])); break;
            case FatMikeEventKind.Tile:
                _gfx.AddTiles(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4]), W(a[5]), W(a[6]), W(a[7]));
                break;
            case FatMikeEventKind.VerticalLine: _gfx.AddVerticalLine(W(a[0]), W(a[1]), W(a[2]), W(a[3])); break;
            case FatMikeEventKind.ShadowText:
                _gfx.AddShadowText(W(a[0]), W(a[1]), W(a[2]), Segment(a[3]), W(a[4]), W(a[5]), W(a[6]), W(a[7]));
                break;
            case FatMikeEventKind.ShadowNumber:
                _gfx.AddShadowNumber(W(a[0]), W(a[1]), Long(a[2], a[3]), W(a[4]), W(a[5]), W(a[6]), W(a[7]), W(a[8]));
                break;
            case FatMikeEventKind.Indicator: Indicator(a[0], a[1]); break;
            case FatMikeEventKind.Emblem: Field.Emblem(a[0], a[1], a[2]); break;
            case FatMikeEventKind.Font: Font(a[0]); break;
            case FatMikeEventKind.TypeLines: _typing = (a[0], a[1], a[2], a[6]); break;
            case FatMikeEventKind.TypeLine: _typing = (a[0], a[1], a[2], a[5]); break;
            case FatMikeEventKind.TypeCharacter: TypeCharacter((byte)a[0], a[1], a[2], a[3]); break;
            case FatMikeEventKind.HideObjects: _gfx.HideAllObjects(); break;
            case FatMikeEventKind.Number: _gfx.AddAmount(W(a[0]), W(a[1]), Long(a[2], a[3]), W(a[4]), W(a[5]), W(a[6])); break;
            case FatMikeEventKind.Text: _gfx.AddText(W(a[0]), W(a[1]), W(a[2]), Segment(a[3]), W(a[4]), W(a[5])); break;
            case FatMikeEventKind.DacColour: _gfx.SetColorRegister(W(a[0]), W(a[1])); break;
            default: throw new InvalidOperationException($"Fat Mike event {e.Kind}");
        }
    }

    private static ushort W(int v) => unchecked((ushort)v);

    private static uint Long(short low, short high) => (uint)(W(low) | W(high) << 16);

    /// <summary>The core's symbolic data segment (<see cref="DgroupLayout.Segment"/>) is the layers' data segment.</summary>
    private ushort Segment(short segment) => W(segment) == DgroupLayout.Segment ? _gfx.DataSegment : W(segment);

    private short Ds16(int at) => (short)(_gfx.Ds[at] | _gfx.Ds[at + 1] << 8);

    /// <summary>
    /// [ORIGINAL] 1F63:104B(mask, state): for bit 0 the left picture at x D0h, for bit 1 the right one at x F4h (0A96:0030);
    /// state (low byte) 1: atlas (x, 6Ah) 20h × 28h at y 32h, state 2: atlas (38h or 58h, 43h) 20h × 28h at y 32h, otherwise
    /// atlas (38h or 58h, 6Bh) 20h × 1Fh at y 3Bh.
    /// </summary>
    public void Indicator(short mask, short state)
    {
        if ((mask & 1) != 0) IndicatorPicture((byte)state, 0xD0, 0x38);
        if ((mask & 2) != 0) IndicatorPicture((byte)state, 0xF4, 0x58);
    }

    private void IndicatorPicture(byte state, ushort x, ushort sx)
    {
        switch (state)
        {
            case 1: _gfx.AddBlit(x, 0x6A, 0x20, 0x28, x, 0x32); break;
            case 2: _gfx.AddBlit(sx, 0x43, 0x20, 0x28, x, 0x32); break;
            default: _gfx.AddBlit(sx, 0x6B, 0x20, 0x1F, x, 0x3B); break;
        }
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
