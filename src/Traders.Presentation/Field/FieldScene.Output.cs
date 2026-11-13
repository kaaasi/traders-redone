using Traders.Core.Field;
using Traders.Core.Menus;
using Traders.Core.State;
using Traders.Presentation.Menus;
using Traders.Vga;
using Traders.Vga.Layers;

namespace Traders.Presentation.Field;

/// <summary>
/// The output side of the planting phase 0609:0004 (finding docs/routines/0609_0C73_anbauszene.md, section "Ausgabe"): the
/// requests of the core to the output layers – picture load, objects, palettes and fades, pages, list execution and the
/// retrace waits of 21FD:0185 – played through the graphics layers frame by frame. A routine that waits for retraces
/// (fade, flip, 21FD:060D, 21FD:0665, 21FD:0185) runs up to its next retrace wait per frame (<see cref="NextFrame"/>), in
/// step with the core's <see cref="FieldWait.Retrace"/> frames, so the work after each retrace lands in the frame after it.
/// The core's fields of the object records (DS:330C: +0, +2, +14h, +16h, +18h; #103 P3-4) and the key table are mirrored
/// into the layers' data segment image before they are read; the core's data segment is never written.
/// </summary>
public sealed partial class FieldScene
{
    private const int ObjectCoreFields = 5;

    private IEnumerator<int>? _pending;

    /// <summary>Picture resources for 186C:0002 (event <see cref="FieldEventKind.LoadImage"/>).</summary>
    public IPictureSource? Pictures { get; init; }

    /// <summary>True while a fade, flip or wait still has retrace waits for later frames.</summary>
    public bool Presenting => _pending is not null;

    /// <summary>
    /// Starts a new frame with the keys held in it (the key table the core gets for the frame): a presenting routine in
    /// progress runs on up to its next retrace wait. Call once per frame before the core processes the frame.
    /// </summary>
    public void NextFrame(IReadOnlyCollection<byte> held)
    {
        MirrorKeys(held);
        Step();
    }

    /// <summary>Runs a presenting routine in progress to its end (for a host that does not present frame by frame).</summary>
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

    /// <summary>Starts a frame-stepped routine and runs it up to its first retrace wait (or to its end).</summary>
    private void Present(IEnumerable<int> steps)
    {
        _pending = steps.GetEnumerator();
        Step();
    }

    /// <summary>Plays a request of the output layers; false for the drawing requests and sound.</summary>
    private bool ApplyOutput(FieldEvent e)
    {
        var a = e.Args;
        switch (e.Kind)
        {
            case FieldEventKind.LoadImage: LoadPicture(a[0]); break;
            case FieldEventKind.InitObjects: _gfx.InitializeObjects(); break;
            case FieldEventKind.ResetObjectRestore: _gfx.InvalidateAllRestoreRecords(); break;
            case FieldEventKind.ObjectBuffers: _gfx.SetObjectSaveBuffers(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4])); break;
            case FieldEventKind.ObjectImage: _gfx.SetObjectImage(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4]), W(a[5])); break;
            case FieldEventKind.ShowObject: _gfx.ShowObject(W(a[0]), W(a[1])); break;
            case FieldEventKind.FadeOut: Present(FadeOutSteps(W(a[0]))); break;
            case FieldEventKind.BasePalette: _gfx.CopyLoadedToBase(); break;
            case FieldEventKind.PaletteColour: _gfx.SetBaseColor(W(a[0]), W(a[1])); break;
            case FieldEventKind.FadeIn: Present(_gfx.FadeInSteps()); break;
            case FieldEventKind.ClearList: _gfx.ClearList(); break;
            case FieldEventKind.RunList: _gfx.ExecuteList(); break;
            case FieldEventKind.ShowPage: _gfx.CopyGamePage(DrawPage); break;
            case FieldEventKind.ListOnBothPages: Present(ListOnBothPagesSteps()); break;
            case FieldEventKind.FlipPages: Present(_gfx.FlipPagesSteps()); break;
            case FieldEventKind.RestoreObjects: RestoreObjects(); break;
            case FieldEventKind.DrawObjects: DrawObjects(); break;
            case FieldEventKind.Delay: Present(RetraceSteps(a[0], a[1])); break;
            case FieldEventKind.LastFrame: Present(LastFrameSteps()); break;
            case FieldEventKind.DrawStatus: Present(StatusSteps()); break;
            case FieldEventKind.Sound: break; // the sound driver's
            default: return false;
        }
        return true;
    }

    /// <summary>DS:082E of the layers: index of the page being drawn (21FD:051F).</summary>
    private ushort DrawPage => (ushort)(_gfx.Ds[GraphicsLayers.DrawPageIndex] | _gfx.Ds[GraphicsLayers.DrawPageIndex + 1] << 8);

    /// <summary>[ORIGINAL] 0D73:00D3 as a step: one retrace wait, then the frame ends.</summary>
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
    /// [ORIGINAL] 21FD:037D(rgb12) as steps: eight stages 0…7, each 21FD:03EB (working palette, on VGA 2295:00C5: retrace
    /// wait, then the DAC) and 21FD:0185(4); then the blend colour 21FD:04DA and 21AF:04CD. <see cref="GraphicsLayers.Blend"/>
    /// does 03EB at once (its retrace wait is counted there); the DAC values it writes become visible only after that retrace,
    /// so the registers are held back until the next frame.
    /// </summary>
    private IEnumerable<int> FadeOutSteps(ushort rgb12)
    {
        var dac = _gfx.Dac.Registers;
        bool vga = (_gfx.Ds[GraphicsLayers.CardMode] | _gfx.Ds[GraphicsLayers.CardMode + 1] << 8) == 1; // DS:090E
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

    /// <summary>
    /// [ORIGINAL] 0609:0204 as steps (the whole call, event <see cref="FieldEventKind.DrawStatus"/>): the status lines
    /// (<see cref="Status"/>), then 21FD:060D. In the second round (DS:1582 = 1) 21FD:0185(50) comes first and the lines are
    /// the empty interior.
    /// </summary>
    // [APPROX] Second round (DS:1582 = 1, 0609:0080 from 1F63, lane R9): the typed notice 0A69:0288 with its fonts 21FD:05DB and
    // its own flips is not drawn, as in the core's frames (Planting.StatusFrames).
    private IEnumerable<int> StatusSteps()
    {
        if (_state.U8(NoTime) == 1)
            foreach (int s in RetraceSteps(50, 0)) yield return s;
        Status();
        foreach (int s in ListOnBothPagesSteps()) yield return s;
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
        RestoreObjects();
        _gfx.ExecuteList();
        DrawObjects();
    }

    /// <summary>[ORIGINAL] 23CD:071C([082E]) on the layers' records with the core's fields mirrored.</summary>
    private void RestoreObjects()
    {
        MirrorObjects();
        _gfx.RestoreObjects(DrawPage);
    }

    /// <summary>[ORIGINAL] 23CD:06E9([082E]) on the layers' records with the core's fields mirrored.</summary>
    private void DrawObjects()
    {
        MirrorObjects();
        _gfx.DrawObjects(DrawPage);
    }

    /// <summary>
    /// The core's fields of the eight object records (#103 P3-4: +0 active, +2 layer, +14h/+16h position, +18h frame) copied
    /// into the layers' records; pointers and restore records stay the layers'.
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

    /// <summary>The key table DS:3D47 as the core holds it in the frame (21FD:051F reads DS:3D95/3D91 after its first retrace).</summary>
    private void MirrorKeys(IReadOnlyCollection<byte> held)
    {
        Array.Clear(_gfx.Ds, DgroupLayout.KeyState, KeyTable.Length);
        foreach (byte sc in held)
            if (sc < KeyTable.Length) _gfx.Ds[DgroupLayout.KeyState + sc] = 1;
    }

    /// <summary>
    /// Picture load 186C:0002(n) on VGA as the menu scene does it (MenuScene.LoadPicture, loader of lane programm): DS:09C8 = n,
    /// the decrunched picture's first 300h bytes become the loaded palette DS:5017, the rest goes to the start of the atlas
    /// as plane rows, then 21AF:000D.
    /// </summary>
    // [APPROX] Effect only, from the resource source instead of 186C (lane programm), like MenuScene.LoadPicture; the loader's
    // cache (resource table DS:09CE, EMS/disk state) is not modelled.
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
