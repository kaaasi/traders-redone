using Traders.Core.Extra;
using Traders.Core.Field;
using Traders.Core.State;
using Traders.Presentation.Field;
using Traders.Presentation.Menus;
using Traders.Vga.Layers;

namespace Traders.Presentation.Extra;

/// <summary>
/// Drawing side of the extra phase 1DEF:000C (finding docs/routines/1DEF_000C_zusatzphase.md) on the picture V-ANBAU: the
/// requests it shares with the planting phase (picture, background, frames, border, tiles of the board, objects, fades,
/// pages, waits) are played by a <see cref="FieldScene"/> on the same layers; the phase's own requests (<see cref="ExtraEvent"/>:
/// the typed notice, goods icons, the masked goods on the board, the gain numbers) append entries to the command list 0A96.
/// No game state is changed; the core's data segment is only read.
/// </summary>
public sealed class ExtraScene
{
    private readonly GraphicsLayers _gfx;
    private (short X, short Y, short W, short Lines, short Colour) _typing;

    /// <param name="layers">Graphics layers (command list, pages, atlas).</param>
    /// <param name="state">The core's data segment (read only).</param>
    /// <param name="pictures">Picture resources for 186C:0002.</param>
    public ExtraScene(GraphicsLayers layers, Dgroup state, IPictureSource? pictures)
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

    public void Apply(FieldEvent e) => Field.Apply(e);

    public void Apply(ExtraEvent e)
    {
        Field.Finish();
        var a = e.Args;
        switch (e.Kind)
        {
            case ExtraEventKind.TypeLines: _typing = (a[0], a[1], a[2], a[3], a[6]); break;
            case ExtraEventKind.TypeCharacter: TypeCharacter((byte)a[0], a[1], a[2], a[3]); break;
            // [ORIGINAL] 0609:1912 – goods icon (FieldScene.GoodIcon)
            case ExtraEventKind.DrawGoods: Field.GoodIcon(a[0], a[1], a[2]); break;
            // [ORIGINAL] 0A96:013B – type 3, masked blit
            case ExtraEventKind.MaskedBlit:
                _gfx.AddMaskedBlit(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4]), W(a[5]), W(a[6]), W(a[7]));
                break;
            // [ORIGINAL] 0A96:057A – type 13, number as text
            case ExtraEventKind.Number:
                _gfx.AddNumber(W(a[0]), W(a[1]), (uint)(W(a[2]) | W(a[3]) << 16), W(a[4]), W(a[5]), W(a[6]));
                break;
            default: throw new InvalidOperationException($"extra phase event {e.Kind}");
        }
    }

    private static ushort W(int v) => unchecked((ushort)v);

    private short Ds16(int at) => (short)(_gfx.Ds[at] | _gfx.Ds[at + 1] << 8);

    /// <summary>
    /// [ORIGINAL] 0A69:01AD, the drawing of one character of 0A69:0288/0243 (as in the harvest's scene): line i at
    /// y + i·([34A4] + [34A4]/5) (signed), centred by 0243 at x + 1 + (w − length·[3CA8]) / 2 (unsigned shift), character k at
    /// + k·[3CA8]; DS:0140 = character, then text 0A96:0525(x, y, DS:0140, colour, −1).
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
