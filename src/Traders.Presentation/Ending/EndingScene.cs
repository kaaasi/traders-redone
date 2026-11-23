using Traders.Core.Ending;
using Traders.Core.Field;
using Traders.Core.State;
using Traders.Core.Week;
using Traders.Presentation.Field;
using Traders.Presentation.Menus;
using Traders.Presentation.Week;
using Traders.Vga.Layers;

namespace Traders.Presentation.Ending;

/// <summary>
/// Drawing side of the end screens 20B3:0053–0088 (finding docs/routines/20B3_008E_ende.md): the closing picture V-ANBAU, the
/// evening V-ABEND and the ranking V-ENDE, then the report 0825:000A(0). The requests shared with the field layer (pictures,
/// objects, palettes, fades, pages, list, waits, background, frame, border; <see cref="FieldEvent"/>) are played by a
/// <see cref="FieldScene"/>, the report's (<see cref="WeekEvent"/>) by a <see cref="ReportScene"/>, both on the same layers; the
/// segment's own requests (<see cref="EndingEvent"/>: tiles, masked blit, filled rectangle, texts and numbers, pixels and lines,
/// the emblem, the draw page switches of the reveal, the palette step and the draw page of the wipe) append entries to the
/// command list 0A96 or set the layers' pages and palette. The routines that wait for retraces (21FD:0590, 21FD:03EB on VGA)
/// run frame by frame in step with the core's frames (<see cref="NextFrame"/>). No game state is changed; the core's data
/// segment is only read. Sound, the pixel reads and the key wait are not the scene's.
/// </summary>
public sealed class EndingScene
{
    private readonly GraphicsLayers _gfx;
    private IEnumerator<int>? _pending;

    /// <param name="layers">Graphics layers (command list, pages, atlas) with their own data segment image.</param>
    /// <param name="state">The core's data segment (read only).</param>
    /// <param name="far">Memory behind far text pointers outside DS (texts of the report).</param>
    /// <param name="pictures">Picture resources for 186C:0002.</param>
    public EndingScene(GraphicsLayers layers, Dgroup state, IFarMemory? far, IPictureSource pictures)
    {
        _gfx = layers;
        Field = new FieldScene(layers, state) { Pictures = pictures };
        Report = new ReportScene(layers, state, far, pictures);
    }

    /// <summary>The field scene that plays the shared requests and the frame-stepped output routines.</summary>
    public FieldScene Field { get; }

    /// <summary>The scene of the report 0825:000A(0) at the end.</summary>
    public ReportScene Report { get; }

    public GraphicsLayers Layers => _gfx;

    /// <summary>True while a fade, flip, palette step or wait still has retrace waits for later frames.</summary>
    public bool Presenting => _pending is not null || Field.Presenting || Report.Presenting;

    /// <summary>
    /// Starts a new frame with the keys held in it: a presenting routine in progress runs on up to its next retrace wait. Call
    /// once per frame before the core processes the frame.
    /// </summary>
    public void NextFrame(IReadOnlyCollection<byte> held)
    {
        Field.NextFrame(held);
        Step();
        Report.NextFrame();
    }

    /// <summary>Runs a presenting routine in progress to its end.</summary>
    public void Finish()
    {
        Field.Finish();
        while (_pending is not null) Step();
        Report.Finish();
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

    /// <summary>A request shared with the field layer.</summary>
    public void Apply(FieldEvent e)
    {
        Finish();
        Field.Apply(e);
    }

    /// <summary>A request of the report 0825:000A(0).</summary>
    public void Apply(WeekEvent e)
    {
        Finish();
        Report.Apply(e);
    }

    /// <summary>A request of segment 20B3; a presenting routine still in progress is finished first.</summary>
    public void Apply(EndingEvent e)
    {
        Finish();
        var a = e.Args;
        switch (e.Kind)
        {
            case EndingEventKind.Tile:
                _gfx.AddTiles(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4]), W(a[5]), W(a[6]), W(a[7]));
                break;
            case EndingEventKind.MaskedBlit:
                _gfx.AddMaskedBlit(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4]), W(a[5]), W(a[6]), W(a[7]));
                break;
            case EndingEventKind.FillRect: _gfx.AddFilledRect(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4])); break;
            case EndingEventKind.Text: _gfx.AddText(W(a[0]), W(a[1]), W(a[2]), Segment(a[3]), W(a[4]), W(a[5])); break;
            case EndingEventKind.Number: _gfx.AddNumber(W(a[0]), W(a[1]), Long(a[2], a[3]), W(a[4]), W(a[5]), W(a[6])); break;
            case EndingEventKind.Money: _gfx.AddAmount(W(a[0]), W(a[1]), Long(a[2], a[3]), W(a[4]), W(a[5]), W(a[6])); break;
            case EndingEventKind.Pixel: _gfx.AddPixel(W(a[0]), W(a[1]), W(a[2])); break;
            case EndingEventKind.Line: _gfx.AddLine(W(a[0]), W(a[1]), W(a[2]), W(a[3]), W(a[4])); break;
            case EndingEventKind.VerticalLine: _gfx.AddVerticalLine(W(a[0]), W(a[1]), W(a[2]), W(a[3])); break;
            // the core's DS:37AA at the call is the fourth argument; the field scene reads it from the core's data segment
            case EndingEventKind.Emblem: Field.Emblem(a[0], a[1], a[2]); break;
            case EndingEventKind.PaletteStep: Present(PaletteStepFrames(W(a[0]), W(a[1]))); break;
            case EndingEventKind.BlendColor: _gfx.SetBlendColor(W(a[0])); break;
            case EndingEventKind.DrawOnShownPage: Present(DrawOnShownPageFrames()); break;
            case EndingEventKind.DrawShownPage: _gfx.DrawOnShownPage(); break;
            case EndingEventKind.DrawHiddenPage: _gfx.DrawOnHiddenPage(); break;
            // [ORIGINAL] 21AF:045E – the visible byte; the field scene mirrors the core's object fields again before it restores
            case EndingEventKind.HideObject: _gfx.HideObject(W(a[0])); break;
            case EndingEventKind.Music: // the sound control's
            case EndingEventKind.ReadPixel: // the core's rule page (R6P-1)
            case EndingEventKind.WaitKey: // the core's
                break;
            default: throw new InvalidOperationException($"End screen event {e.Kind}");
        }
    }

    private static ushort W(int v) => unchecked((ushort)v);

    private static uint Long(short low, short high) => (uint)(W(low) | W(high) << 16);

    /// <summary>The core's symbolic data segment (<see cref="DgroupLayout.Segment"/>) is the layers' data segment.</summary>
    private ushort Segment(short segment) => W(segment) == DgroupLayout.Segment ? _gfx.DataSegment : W(segment);

    private bool Vga => (_gfx.Ds[GraphicsLayers.CardMode] | _gfx.Ds[GraphicsLayers.CardMode + 1] << 8) == 1; // DS:090E

    /// <summary>
    /// [ORIGINAL] 21FD:03EB(rgb12, stage) as frame steps: the working palette, on VGA 2295:00C5 (retrace wait, then the DAC).
    /// <see cref="GraphicsLayers.Blend"/> does it at once (its retrace wait is counted there); the DAC values become visible
    /// only after that retrace, so they are held back until the next frame.
    /// </summary>
    private IEnumerable<int> PaletteStepFrames(ushort rgb12, ushort stage)
    {
        var dac = _gfx.Dac.Registers;
        var before = (byte[])dac.Clone();
        _gfx.Blend(rgb12, stage);
        if (!Vga) yield break;
        var after = (byte[])dac.Clone();
        before.CopyTo(dac, 0);
        yield return 0;
        after.CopyTo(dac, 0);
    }

    /// <summary>
    /// [ORIGINAL] 21FD:0590 as frame steps: draw on the page shown – DS:082E = DS:0830, DS:4516 = its page number,
    /// 0B67:0238(that page, that page) with its show-page retrace (pages 1 and 2), then the retrace of 0B67:028F.
    /// </summary>
    private IEnumerable<int> DrawOnShownPageFrames()
    {
        var ds = _gfx.Ds;
        ds[GraphicsLayers.DrawPageIndex] = ds[GraphicsLayers.ShownPageIndex];
        ds[GraphicsLayers.DrawPageIndex + 1] = ds[GraphicsLayers.ShownPageIndex + 1];
        int index = ds[GraphicsLayers.DrawPageIndex] | ds[GraphicsLayers.DrawPageIndex + 1] << 8;
        int at = GraphicsLayers.PageNumbers + (index << 2);
        for (int i = 0; i < 4; i++) ds[GraphicsLayers.CurrentDrawPage + i] = ds[at + i];
        uint page = (uint)(ds[at] | ds[at + 1] << 8 | ds[at + 2] << 16 | ds[at + 3] << 24);
        _gfx.SelectPages(page, page);
        if (page is 1 or 2) yield return 0;
        _gfx.WaitForRetrace();
        yield return 0;
    }
}
