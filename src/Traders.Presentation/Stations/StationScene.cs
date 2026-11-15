using Traders.Core.State;
using Traders.Core.Stations;
using Traders.Presentation.Menus;
using Traders.Vga;
using Traders.Vga.Layers;

namespace Traders.Presentation.Stations;

/// <summary>
/// Drawing side of the stations phase 1B03:0009 (finding docs/routines/1B03_0009_stationen.md, section "Szene"): plays every
/// <see cref="StationEvent"/> of the core's <see cref="StationPhase"/> through the graphics layers – command list 0A96,
/// shadowed and typed texts 0A69, numbers in texts 2295, objects 21AF/23CD, pages 0B67, palettes, fades, flips and retrace
/// waits 21FD, the picture load 186C. The events come in the order and with the arguments of the original's calls; the scene
/// receives each one when the core reports it (<see cref="StationPhase.Output"/>), so what it reads from the core's data
/// segment (object records, key table) is the state at that call. The core's data segment is never written; the graphics
/// state lives in the layers' own data segment image, the number texts 2295:0001 in the text buffer behind
/// <see cref="GraphicsLayers.OtherMemory"/>. Routines that wait for retraces (21FD:0185, the flips 21FD:051F/060D/0665, the
/// fades, the palette step 21FD:03EB, the typed texts 0A69:0243/0288) and the pause 21FD:0245 run up to their next wait per
/// frame (<see cref="NextFrame"/>), in step with the core's frames (StationPhase halts at the same waits); each retrace wait
/// goes to the layers' <see cref="GraphicsLayers.Retrace"/>.
/// </summary>
public sealed class StationScene
{
    private const int ObjectCoreFields = 5;

    private readonly GraphicsLayers _gfx;
    private readonly Dgroup _state;
    private readonly IPictureSource _pictures;
    private IEnumerator<int>? _pending;

    /// <param name="layers">
    /// Graphics layers with their own data segment image (DataSegment = segment value of DS) and the text buffer DS:4884 as
    /// <see cref="GraphicsLayers.OtherMemory"/> (typed texts, names, number texts).
    /// </param>
    /// <param name="state">The core's data segment (read only).</param>
    /// <param name="pictures">Picture resources for 186C:0002.</param>
    public StationScene(GraphicsLayers layers, Dgroup state, IPictureSource pictures)
    {
        _gfx = layers;
        _state = state;
        _pictures = pictures;
    }

    public GraphicsLayers Layers => _gfx;

    /// <summary>Sounds 21A7:0054(channel, sound) of the phase, for the sound layer (optional).</summary>
    public Action<short, short>? Sound { get; set; }

    /// <summary>True while a routine still has waits for later frames.</summary>
    public bool Presenting => _pending is not null;

    /// <summary>
    /// Starts a new frame with the keys held in it (the key table the core gets for the same frame); a presenting routine runs
    /// on up to its next wait. Called before the core's tick of the frame.
    /// </summary>
    public void NextFrame(IReadOnlyCollection<byte> held)
    {
        Array.Clear(_gfx.Ds, DgroupLayout.KeyState, Core.Menus.KeyTable.Length);
        foreach (byte sc in held)
            if (sc < Core.Menus.KeyTable.Length) _gfx.Ds[DgroupLayout.KeyState + sc] = 1;
        Step();
    }

    /// <summary>Runs a presenting routine in progress to its end (after the phase returned).</summary>
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
        if (_pending is not null) throw new InvalidOperationException("A presenting routine is still running.");
        _pending = steps.GetEnumerator();
        Step();
    }

    private static ushort W(short v) => unchecked((ushort)v);

    /// <summary>A segment argument: the core's placeholder for DS becomes the layers' DS segment.</summary>
    private ushort Seg(short v) => v == StationEvent.DataSegment ? _gfx.DataSegment : W(v);

    private static uint Long(short low, short high) => (uint)(W(low) | W(high) << 16);

    /// <summary>Plays one output call of the phase.</summary>
    public void Apply(StationEvent e)
    {
        var a = e.Args;
        MirrorKeys();
        switch (e.Call)
        {
            // ---------------------------------------------------------- command list 0A96
            case StationCall.ClearList: _gfx.ClearList(); break;
            case StationCall.RunList: _gfx.ExecuteList(); break;
            case StationCall.Blit: _gfx.AddBlit(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4]), W(a[5])); break;
            case StationCall.Blit8: _gfx.AddBlit8(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4])); break;
            case StationCall.Plot: _gfx.AddPixel(W(a[0]), W(a[1]), W(a[2])); break;
            case StationCall.Text: _gfx.AddText(W(a[0]), W(a[1]), W(a[2]), Seg(a[3]), W(a[4]), W(a[5])); break;
            case StationCall.CopyPage: _gfx.CopyGamePage(W(a[0])); break;

            // ---------------------------------------------------------- texts 0A69, numbers 2295
            case StationCall.ShadowText:
                _gfx.AddShadowText(W(a[0]), W(a[1]), W(a[2]), Seg(a[3]), W(a[4]), W(a[5]), W(a[6]), W(a[7]));
                break;
            case StationCall.ShadowNumber:
                _gfx.AddShadowNumber(W(a[0]), W(a[1]), Long(a[2], a[3]), W(a[4]), W(a[5]), W(a[6]), W(a[7]), W(a[8]));
                break;
            case StationCall.TypedText:
                Present(CenteredTypewriterSteps(W(a[0]), W(a[1]), W(a[2]), W(a[3]), Seg(a[4]), W(a[5]), a[6], a[7]));
                break;
            case StationCall.TypedLines:
                Present(TypewriterLinesSteps(W(a[0]), W(a[1]), W(a[2]), W(a[3]), Seg(a[4]), a[5], W(a[6]), a[7]));
                break;
            case StationCall.NumberText: _gfx.FormatNumber(Long(a[0], a[1]), W(a[2]), W(a[3]), Seg(a[4])); break;

            // ---------------------------------------------------------- objects 21AF, 23CD
            case StationCall.ObjectImage: _gfx.SetObjectImage(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4]), W(a[5])); break;
            case StationCall.ObjectBuffers: _gfx.SetObjectSaveBuffers(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4])); break;
            case StationCall.ShowObject: _gfx.ShowObject(W(a[0]), W(a[1])); break;
            case StationCall.HideObject: _gfx.HideObject(W(a[0])); break;
            case StationCall.DrawObjects: DrawObjects(W(a[0])); break;
            case StationCall.RestoreObjects: RestoreObjects(W(a[0])); break;

            // ---------------------------------------------------------- pages 0B67, frames 21FD
            case StationCall.DrawOnShownPage: _gfx.DrawOnShownPage(); break;
            case StationCall.DrawOnHiddenPage: _gfx.DrawOnHiddenPage(); break;
            case StationCall.Flip: Present(_gfx.FlipPagesSteps()); break;
            case StationCall.ListOnBothPages: Present(ListOnBothPagesSteps()); break;
            case StationCall.LastFrame: Present(LastFrameSteps()); break;
            case StationCall.Delay: Present(RetraceSteps(a[0], a[1])); break;
            case StationCall.Pause: Present(PauseSteps()); break; // nothing drawn; the polls keep the frames in step

            // ---------------------------------------------------------- palettes 21FD, picture 186C, sound 21A7
            case StationCall.FadeOut: Present(FadeOutSteps(W(a[0]))); break;
            // [ORIGINAL] 21FD:026C – the argument is not used: loaded palette DS:5017 → base palette DS:3DCB
            case StationCall.BasePalette: _gfx.CopyLoadedToBase(); break;
            case StationCall.FadeIn: Present(_gfx.FadeInSteps()); break;
            case StationCall.PaletteStep: Present(BlendSteps(W(a[0]), W(a[1]))); break;
            case StationCall.LoadPicture: LoadPicture(a[0]); break;
            case StationCall.Sound: Sound?.Invoke(a[0], a[1]); break;
            default: throw new NotSupportedException($"Station call {e.Call} is not played.");
        }
    }

    /// <summary>DS:082E of the layers: index of the page being drawn (21FD:051F).</summary>
    private ushort DrawPage => (ushort)(_gfx.Ds[GraphicsLayers.DrawPageIndex] | _gfx.Ds[GraphicsLayers.DrawPageIndex + 1] << 8);

    private IEnumerable<int> Retrace()
    {
        _gfx.WaitRetrace();
        yield return 0;
    }

    /// <summary>[ORIGINAL] 21FD:0185(n) as steps: n retraces, n a long (low, high); none when the high word is negative.</summary>
    private IEnumerable<int> RetraceSteps(short low, short high)
    {
        if (high < 0) yield break;
        long n = ((long)high << 16) | W(low);
        for (long i = 0; i < n; i++)
            foreach (int s in Retrace()) yield return s;
    }

    /// <summary>[ORIGINAL] 21FD:060D as steps: 0A96:06A8, 21FD:051F, 0A96:06A8.</summary>
    private IEnumerable<int> ListOnBothPagesSteps()
    {
        _gfx.ExecuteList();
        foreach (int s in _gfx.FlipPagesSteps()) yield return s;
        _gfx.ExecuteList();
    }

    /// <summary>[ORIGINAL] 21FD:0665 as steps: 21FD:051F, 23CD:071C([082E]), 0A96:06A8, 23CD:06E9([082E]).</summary>
    private IEnumerable<int> LastFrameSteps()
    {
        foreach (int s in _gfx.FlipPagesSteps()) yield return s;
        RestoreObjects(DrawPage);
        _gfx.ExecuteList();
        DrawObjects(DrawPage);
    }

    /// <summary>
    /// [ORIGINAL] 21FD:0245 as steps: with P held (DS:3D60) one frame per busy poll while it is held, released, pressed again
    /// and released, as the core polls; nothing is drawn.
    /// </summary>
    private IEnumerable<int> PauseSteps()
    {
        if (_gfx.Ds[PauseKey] == 0) yield break;
        while (_gfx.Ds[PauseKey] != 0) yield return 0;
        while (_gfx.Ds[PauseKey] == 0) yield return 0;
        while (_gfx.Ds[PauseKey] != 0) yield return 0;
    }

    private const int PauseKey = DgroupLayout.KeyState + 0x19;

    private bool IsVga => (_gfx.Ds[GraphicsLayers.CardMode] | _gfx.Ds[GraphicsLayers.CardMode + 1] << 8) == 1;

    /// <summary>
    /// [ORIGINAL] 21FD:03EB(rgb12, steps) as steps: the working palette, on VGA the retrace wait of 2295:00C5 and then the DAC
    /// (the DAC values become visible after the retrace).
    /// </summary>
    private IEnumerable<int> BlendSteps(ushort rgb12, ushort steps)
    {
        var dac = _gfx.Dac.Registers;
        var before = (byte[])dac.Clone();
        _gfx.Blend(rgb12, steps);
        if (!IsVga) yield break;
        var after = (byte[])dac.Clone();
        before.CopyTo(dac, 0);
        yield return 0;
        after.CopyTo(dac, 0);
    }

    /// <summary>
    /// [ORIGINAL] 21FD:037D(rgb12) as steps: eight stages of 21FD:03EB and 21FD:0185(4); then the blend colour and 21AF:04CD.
    /// </summary>
    private IEnumerable<int> FadeOutSteps(ushort rgb12)
    {
        for (ushort n = 0; n <= 7; n++)
        {
            foreach (int s in BlendSteps(rgb12, n)) yield return s;
            foreach (int s in RetraceSteps(4, 0)) yield return s;
        }
        _gfx.SetBlendColor(rgb12);
        _gfx.InvalidateAllRestoreRecords();
    }

    /// <summary>
    /// [ORIGINAL] 0A69:0288(x, y, w, far lines, count, colour, wait) as steps: lines i = 0 … count − 1 (signed) of the far
    /// pointer table, each with 0243 at y + i·([34A4] + [34A4]/5) (signed division), final wait −1.
    /// </summary>
    private IEnumerable<int> TypewriterLinesSteps(ushort x, ushort y, ushort w, ushort linesOffset, ushort linesSegment,
        short count, ushort colour, short wait)
    {
        for (short i = 0; i < count; i++)
        {
            short height = (short)U16(GraphicsLayers.FontLineHeight);
            ushort step = (ushort)(height + height / 5);
            ushort at = (ushort)(linesOffset + (ushort)(i << 2));
            ushort lineOffset = ReadFar16(linesSegment, at), lineSegment = ReadFar16(linesSegment, (ushort)(at + 2));
            foreach (int s in CenteredTypewriterSteps(x, (ushort)(y + step * i), w, lineOffset, lineSegment, colour, wait, -1))
                yield return s;
        }
    }

    /// <summary>
    /// [ORIGINAL] 0A69:0243(x, y, w, far text, colour, wait, final wait) as steps: 01AD centred in w,
    /// x + 1 + (w − strlen · [3CA8]) / 2 (the difference shifted as an unsigned word); nothing for a null pointer.
    /// </summary>
    private IEnumerable<int> CenteredTypewriterSteps(ushort x, ushort y, ushort w, ushort offset, ushort segment, ushort colour,
        short wait, short finalWait)
    {
        if (offset == 0 && segment == 0) yield break;
        int length = Length(segment, offset);
        ushort left = (ushort)(x + 1 + ((ushort)(w - length * (short)U16(GraphicsLayers.FontAdvance)) >> 1));
        foreach (int s in TypewriterSteps(left, y, offset, segment, colour, wait, finalWait)) yield return s;
    }

    /// <summary>
    /// [ORIGINAL] 0A69:01AD as steps: per character DS:0140 = character, list emptied, the character as text at
    /// x + i·[3CA8] (no shadow), 21FD:060D; the key click (21A7:0054(1, 16h)) except for blanks, 21FD:0245, 21FD:0185(wait);
    /// finally 0185(final wait) if it is &gt; 0.
    /// </summary>
    private IEnumerable<int> TypewriterSteps(ushort x, ushort y, ushort offset, ushort segment, ushort colour, short wait,
        short finalWait)
    {
        for (ushort i = 0; Length(segment, offset) > i; i++)
        {
            _gfx.Ds[GraphicsLayers.TypewriterChar] = ReadFar(segment, (ushort)(offset + i));
            _gfx.ClearList();
            _gfx.AddText((ushort)(x + i * (short)U16(GraphicsLayers.FontAdvance)), y, GraphicsLayers.TypewriterChar,
                _gfx.DataSegment, colour, 0xFFFF);
            foreach (int s in ListOnBothPagesSteps()) yield return s;
            if (_gfx.Ds[GraphicsLayers.TypewriterChar] != (byte)' ') _gfx.Effects?.PlaySound(1, 0x16);
            foreach (int s in PauseSteps()) yield return s;
            foreach (int s in RetraceSteps(wait, (short)(wait >> 15))) yield return s;
        }
        if (finalWait > 0)
            foreach (int s in RetraceSteps(finalWait, (short)(finalWait >> 15))) yield return s;
    }

    private ushort U16(int at) => (ushort)(_gfx.Ds[at] | _gfx.Ds[at + 1] << 8);

    private byte ReadFar(ushort segment, ushort offset) =>
        segment == _gfx.DataSegment ? _gfx.Ds[offset]
        : (_gfx.OtherMemory ?? throw new InvalidOperationException("No far memory for the texts.")).Read(segment, offset);

    private ushort ReadFar16(ushort segment, ushort offset) =>
        (ushort)(ReadFar(segment, offset) | ReadFar(segment, (ushort)(offset + 1)) << 8);

    private int Length(ushort segment, ushort offset)
    {
        int n = 0;
        while (ReadFar(segment, (ushort)(offset + n)) != 0) n++;
        return n;
    }

    /// <summary>[ORIGINAL] 23CD:071C(page) on the layers' records with the core's fields mirrored.</summary>
    private void RestoreObjects(ushort page)
    {
        MirrorObjects();
        _gfx.RestoreObjects(page);
    }

    /// <summary>[ORIGINAL] 23CD:06E9(page) on the layers' records with the core's fields mirrored.</summary>
    private void DrawObjects(ushort page)
    {
        MirrorObjects();
        _gfx.DrawObjects(page);
    }

    /// <summary>
    /// The core's fields of the eight object records DS:330C (+0 active, +2 height, +14h/+16h position, +18h frame) copied
    /// into the layers' records; image and save pointers and the restore records stay the layers'.
    /// </summary>
    private void MirrorObjects()
    {
        ReadOnlySpan<int> fields = [0x00, 0x02, 0x14, 0x16, 0x18];
        for (int i = 0; i < GraphicsLayers.ObjectCount; i++)
        {
            int at = GraphicsLayers.ObjectTable + i * GraphicsLayers.ObjectSize;
            for (int k = 0; k < ObjectCoreFields; k++)
            {
                int f = at + fields[k];
                _gfx.Ds[f] = _state.U8(f);
                if (k > 0) _gfx.Ds[f + 1] = _state.U8(f + 1);
            }
        }
    }

    /// <summary>The key table DS:3D47 as the core holds it (21FD:051F reads DS:3D95/3D91).</summary>
    private void MirrorKeys()
    {
        for (int sc = 0; sc < Core.Menus.KeyTable.Length; sc++)
            _gfx.Ds[DgroupLayout.KeyState + sc] = _state.U8(DgroupLayout.KeyState + sc);
    }

    /// <summary>
    /// Picture load 186C:0002(n) on VGA as the menu and field scenes do it: DS:09C8 = n, the decrunched picture's first 300h
    /// bytes become the loaded palette DS:5017, the rest goes to the start of the atlas as plane rows, then 21AF:000D.
    /// </summary>
    // [APPROX] Effect only, from the resource source instead of 186C (lane programm), like FieldScene.LoadPicture; the loader's
    // cache (resource table DS:09CE, EMS/disk state) is not modelled.
    private void LoadPicture(int resource)
    {
        var data = _pictures.Decrunched(resource);
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
