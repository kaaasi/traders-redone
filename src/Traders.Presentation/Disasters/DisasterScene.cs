using Traders.Core.Disasters;
using Traders.Core.Menus;
using Traders.Core.State;
using Traders.Presentation.Field;
using Traders.Presentation.Menus;
using Traders.Vga;
using Traders.Vga.Layers;
using E = Traders.Core.Disasters.DisasterEventKind;

namespace Traders.Presentation.Disasters;

/// <summary>
/// Drawing side of the catastrophes 111D:0009 (finding docs/routines/111D_0009_katastrophen.md, lane katastrophen part 2):
/// plays the core's <see cref="DisasterEvent"/>s through the graphics layers. Routines that wait for retraces (fades, flips,
/// 21FD:060D, 21FD:0590, 21FD:0185, a typed character) run up to their next retrace wait per frame
/// (<see cref="NextFrame"/>), in step with the core's frames. Background and frame are the field layer's
/// (<see cref="FieldScene.Background"/>, <see cref="FieldScene.Frame"/>). Texts behind far pointers are read through the
/// layers' far memory (<see cref="GraphicsLayers.OtherMemory"/>, the text buffer). No game state is changed; values the
/// original reads from the data segment (far text pointers, DS:00A8 …) are read from the core's data segment.
/// </summary>
public sealed class DisasterScene
{
    private readonly GraphicsLayers _gfx;
    private readonly Dgroup _state;
    private readonly FieldScene _field;
    private IEnumerator<int>? _pending;

    // arguments of the last 0A69:0288 (x, y, w, lines, count, colour, wait)
    private short[] _lines = [];

    /// <param name="layers">Graphics layers (command list, pages, palettes, atlas).</param>
    /// <param name="state">The core's data segment (read only).</param>
    public DisasterScene(GraphicsLayers layers, Dgroup state)
    {
        _gfx = layers;
        _state = state;
        _field = new FieldScene(layers, state);
    }

    /// <summary>Picture resources for 186C:0002 (event <see cref="E.LoadImage"/>).</summary>
    public IPictureSource? Pictures { get; init; }

    /// <summary>True while a routine still has retrace waits for later frames.</summary>
    public bool Presenting => _pending is not null;

    /// <summary>Starts a new frame with the keys held in it; a presenting routine runs on up to its next retrace wait.</summary>
    public void NextFrame(IReadOnlyCollection<byte> held)
    {
        Array.Clear(_gfx.Ds, DgroupLayout.KeyState, KeyTable.Length);
        foreach (byte sc in held)
            if (sc < KeyTable.Length) _gfx.Ds[DgroupLayout.KeyState + sc] = 1;
        Step();
    }

    /// <summary>Runs a presenting routine in progress to its end.</summary>
    public void Finish()
    {
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
        if (_pending is not null) throw new InvalidOperationException("a presenting routine is still running");
        _pending = steps.GetEnumerator();
        Step();
    }

    private static ushort W(int v) => unchecked((ushort)v);

    /// <summary>Plays one event of the core.</summary>
    public void Apply(DisasterEvent e)
    {
        var a = e.Args;
        switch (e.Kind)
        {
            case E.LoadImage: LoadPicture(a[0]); break;
            case E.FadeOut: Present(FadeOutSteps(W(a[0]))); break;
            case E.FadeIn: Present(_gfx.FadeInSteps()); break;
            // [ORIGINAL] 21FD:026C – takes no argument: base palette ← loaded palette (the pointer pushed by 111D is ignored)
            case E.BasePalette: _gfx.CopyLoadedToBase(); break;
            case E.SetColour: _gfx.SetColorRegister(W(a[0]), W(a[1])); break;
            case E.ClearList: _gfx.ClearList(); break;
            case E.DrawBackground: _field.Background(); break;
            case E.DrawFrame: _field.Frame(a[0], a[1], a[2], a[3]); break;
            case E.DrawTitle:
                _gfx.AddCenteredShadowText(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4]), W(a[5]), W(a[6]), W(a[7]), W(a[8]));
                break;
            case E.TypeLines: _lines = a; break;
            case E.TypeCharacter: Present(CharacterSteps(a[0], a[1])); break;
            case E.ListOnBothPages: Present(ListOnBothPagesSteps()); break;
            case E.FlipPages: Present(_gfx.FlipPagesSteps()); break;
            case E.DrawOnShownPage: Present(DrawOnShownPageSteps()); break;
            case E.Column: Column(a[0], a[1]); break;
            case E.Sound: break; // the sound driver's
            case E.Delay: Present(RetraceSteps(a[0], a[1])); break;
            case E.Blit: _gfx.AddBlit(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4]), W(a[5])); break;
            case E.FillRect: _gfx.AddFilledRect(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4])); break;
            case E.SetPixel: _gfx.AddPixel(W(a[0]), W(a[1]), W(a[2])); break;
            case E.ExecuteList: _gfx.ExecuteList(); break;
            case E.RestoreObjects: _gfx.RestoreObjects(DrawPage); break;
            case E.DrawObjects: _gfx.DrawObjects(DrawPage); break;
            default: throw new InvalidOperationException($"unknown event {e}");
        }
    }

    /// <summary>DS:082E of the layers: index of the page being drawn.</summary>
    private ushort DrawPage => (ushort)(_gfx.Ds[GraphicsLayers.DrawPageIndex] | _gfx.Ds[GraphicsLayers.DrawPageIndex + 1] << 8);

    private IEnumerable<int> Retrace()
    {
        _gfx.WaitRetrace();
        yield return 0;
    }

    /// <summary>[ORIGINAL] 21FD:0185(n): n retraces, n a long (low, high); none when n ≤ 0 (signed high word).</summary>
    private IEnumerable<int> RetraceSteps(short low, short high)
    {
        if (high < 0) yield break;
        long n = ((long)high << 16) | (ushort)low;
        for (long i = 0; i < n; i++)
            foreach (int s in Retrace()) yield return s;
    }

    /// <summary>
    /// [ORIGINAL] 21FD:037D(rgb12) as steps (as FieldScene's): eight stages, each 21FD:03EB (on VGA a retrace wait, then
    /// the DAC) and 21FD:0185(4); then the blend colour and 21AF:04CD. The DAC values become visible after the retrace.
    /// </summary>
    private IEnumerable<int> FadeOutSteps(ushort rgb12)
    {
        var dac = _gfx.Dac.Registers;
        bool vga = (_gfx.Ds[GraphicsLayers.CardMode] | _gfx.Ds[GraphicsLayers.CardMode + 1] << 8) == 1;
        for (ushort n = 0; n <= 7; n++)
        {
            var before = (byte[])dac.Clone();
            _gfx.Blend(rgb12, n);
            if (vga)
            {
                var after = (byte[])dac.Clone();
                before.CopyTo(dac, 0);
                yield return 0;
                after.CopyTo(dac, 0);
            }
            foreach (int s in RetraceSteps(4, 0)) yield return s;
        }
        _gfx.SetBlendColor(rgb12);
        _gfx.InvalidateAllRestoreRecords();
    }

    /// <summary>[ORIGINAL] 21FD:060D as steps: 0A96:06A8, 21FD:051F, 0A96:06A8.</summary>
    private IEnumerable<int> ListOnBothPagesSteps()
    {
        _gfx.ExecuteList();
        foreach (int s in _gfx.FlipPagesSteps()) yield return s;
        _gfx.ExecuteList();
    }

    /// <summary>
    /// [ORIGINAL] 21FD:0590 as steps: DS:082E = DS:0830, the draw pointer DS:4516 from the page numbers DS:40DE, then
    /// 0B67:0238(draw, page of 0830): the page shown again (retrace), draw on it (0B67:01D4); then 0B67:028F (retrace).
    /// </summary>
    private IEnumerable<int> DrawOnShownPageSteps()
    {
        ushort shown = (ushort)(_gfx.Ds[GraphicsLayers.ShownPageIndex] | _gfx.Ds[GraphicsLayers.ShownPageIndex + 1] << 8);
        Set16(GraphicsLayers.DrawPageIndex, shown);
        int at = GraphicsLayers.PageNumbers + (shown << 2);
        ushort low = U16(at), high = U16(at + 2);
        Set16(GraphicsLayers.CurrentDrawPage, low);
        Set16(GraphicsLayers.CurrentDrawPage + 2, high);
        uint number = (uint)(low | high << 16);
        _gfx.SelectPages(number, number); // show (with its retrace wait), then draw on the page shown
        yield return 0;
        foreach (int s in Retrace()) yield return s;
    }

    /// <summary>
    /// [ORIGINAL] 0A69:01AD for one character (event <see cref="E.TypeCharacter"/>): line <paramref name="line"/> of the last
    /// 0A69:0288 (0243: centred in w, y + line · ([34A4] + [34A4]/5)), list emptied, the character at x + index · [3CA8]
    /// (DS:0140, 0A96:0525, colour, no shadow), 21FD:060D; then 21FD:0185(wait). The sound and the pause check are not the
    /// scene's.
    /// </summary>
    private IEnumerable<int> CharacterSteps(short line, short index)
    {
        short x = _lines[0], y = _lines[1], w = _lines[2], lines = _lines[3], colour = _lines[5], wait = _lines[6];
        ushort at = W(lines + 4 * line);
        ushort offset = _state.U16(at), segment = _state.U16(W(at + 2));
        short height = (short)U16(LineHeight);
        ushort step = W(height + height / 5);
        ushort advance = U16(GraphicsLayers.FontAdvance);
        int length = 0;
        while (ReadFar(segment, W(offset + length)) != 0) length++;
        ushort left = W(x + 1 + (W(w - length * (short)advance) >> 1));
        byte c = ReadFar(segment, W(offset + index));
        _gfx.Ds[TypewriterChar] = c;
        _gfx.ClearList();
        _gfx.AddText(W(left + index * (short)advance), W(y + step * line), TypewriterChar, _gfx.DataSegment, W(colour), 0xFFFF);
        foreach (int s in ListOnBothPagesSteps()) yield return s;
        foreach (int s in RetraceSteps(wait, (short)(wait >> 15))) yield return s;
    }

    private const int TypewriterChar = 0x0140, LineHeight = 0x34A4;

    private byte ReadFar(ushort segment, ushort offset) =>
        segment == _gfx.DataSegment ? _gfx.Ds[offset]
        : (_gfx.OtherMemory ?? throw new InvalidOperationException("No far memory for the texts.")).Read(segment, offset);

    /// <summary>
    /// [ORIGINAL] 111D:07E2(x, on): top si = max(0, 45 − x/3), height 46 − x/5 − si (signed divisions); list emptied; on: a
    /// vertical line 0A69:002C(x + 60, si, h, 12), otherwise the atlas column 0A96:0030(x + 60, si, 1, h, x + 60, si); the list
    /// executed.
    /// </summary>
    private void Column(short x, short on)
    {
        short si = (short)(0x2D - x / 3);
        if (si < 0) si = 0;
        short h = (short)(0x2E - x / 5 - si);
        _gfx.ClearList();
        if (on != 0) _gfx.AddVerticalLine(W(x + 0x3C), W(si), W(h), 0x0C);
        else _gfx.AddBlit(W(x + 0x3C), W(si), 1, W(h), W(x + 0x3C), W(si));
        _gfx.ExecuteList();
    }

    private ushort U16(int offset) => (ushort)(_gfx.Ds[offset] | _gfx.Ds[offset + 1] << 8);

    private void Set16(int offset, ushort v)
    {
        _gfx.Ds[offset] = (byte)v;
        _gfx.Ds[offset + 1] = (byte)(v >> 8);
    }

    /// <summary>Picture load 186C:0002(n) on VGA as the field scene does it (effect only, from the resource source).</summary>
    // [APPROX] Effect only, from the resource source instead of 186C (lane programm), like FieldScene.LoadPicture.
    private void LoadPicture(int resource)
    {
        var source = Pictures ?? throw new InvalidOperationException("No picture source for 186C:0002.");
        var data = source.Decrunched(resource);
        _gfx.Ds[0x09C8] = (byte)resource;
        _gfx.Ds[0x09C9] = (byte)(resource >> 8);
        Array.Copy(data, 0, _gfx.Ds, GraphicsLayers.LoadedPalette, 0x300);
        var atlas = _gfx.Driver.Atlas.Data;
        int n = Math.Min(atlas.Length, data.Length - 0x300);
        Array.Copy(data, 0x300, atlas, 0, n);
        PlanarAtlas.ConvertToPlanarRows(atlas, PlanarAtlas.HeaderSize, PlanarAtlas.Width, PlanarAtlas.Height);
        _gfx.InitializeObjects();
    }
}
