using Traders.Core.Events;
using Traders.Core.Menus;
using Traders.Core.State;
using Traders.Core.Week;
using Traders.Presentation.Field;
using Traders.Vga.Layers;

namespace Traders.Presentation.Events;

/// <summary>
/// The scene of the week incidents 17CB:0005 (call 9): plays the <see cref="IncidentEvent"/>s of the core's
/// <see cref="WeekIncidents"/> through the graphics layers (finding docs/routines/17CB_0005_wochenereignisse.md). The window
/// 17CB:0986 (fade-out, field background and frame of 0609, page copy, font, palette, fade-in), the emblems 0609:1879, the
/// typewriter characters of 0A69:0288 at their centred positions, the page flips with their retraces, the pauses 21FD:0185 and
/// the drawing of the wait loop 21FD:0004(0). The core decides and books, writes the amount into the text (2295:0001) and
/// DS:0140; the scene reads the core's data segment and the text memory and never changes them. The graphics state (pages,
/// palettes, fonts, list, DS:0140 of the list entry) lives in the layers' own data segment image.
/// </summary>
public sealed class IncidentScene
{
    private readonly GraphicsLayers _gfx;
    private readonly Dgroup _state;
    private readonly ITextMemory _texts;
    private readonly FieldScene _field;
    private IEnumerator<int>? _pending;

    /// <summary>Lines of the text being typed (from its <see cref="IncidentEventKind.Text"/> event).</summary>
    private int _lines;

    /// <param name="layers">Graphics layers with their own data segment image.</param>
    /// <param name="state">The core's data segment (read only).</param>
    /// <param name="texts">The text memory behind DS:37CC (read only).</param>
    public IncidentScene(GraphicsLayers layers, Dgroup state, ITextMemory texts)
    {
        _gfx = layers;
        _state = state;
        _texts = texts;
        _field = new FieldScene(layers, state);
    }

    public GraphicsLayers Layers => _gfx;

    /// <summary>
    /// Called right before a routine that hands the picture to the display or the DAC runs (21FD:037D, 0329, 051F), at the
    /// point where the original enters it.
    /// </summary>
    public Action<IncidentEvent>? BeforePresent { get; set; }

    /// <summary>True while a fade, flip or pause still has retrace waits for later frames.</summary>
    public bool Presenting => _pending is not null;

    /// <summary>
    /// Starts a new frame: a fade, flip or pause in progress runs on up to its next retrace wait (one retrace per frame, in step
    /// with the core's waits). Call once per frame before the frame's events.
    /// </summary>
    public void NextFrame()
    {
        if (_pending is null) return;
        MirrorKeys();
        if (!_pending.MoveNext())
        {
            _pending.Dispose();
            _pending = null;
        }
    }

    /// <summary>Runs a fade, flip or pause in progress to its end.</summary>
    public void Finish()
    {
        while (_pending is not null) NextFrame();
    }

    private void Present(IEnumerable<int> steps)
    {
        _pending = steps.GetEnumerator();
        NextFrame();
    }

    /// <summary>Plays one event (a presenting routine still in progress is finished first).</summary>
    public void Apply(IncidentEvent e)
    {
        Finish();
        switch (e.Kind)
        {
            case IncidentEventKind.FadeOut:
                BeforePresent?.Invoke(e);
                Present(FadeOutSteps(W(e.A)));
                break;
            case IncidentEventKind.ListClear: _gfx.ClearList(); break;
            case IncidentEventKind.FieldBackdrop: _field.Background(); break;
            case IncidentEventKind.Frame: _field.Frame((short)e.A, (short)e.B, (short)e.C, (short)e.D); break;
            case IncidentEventKind.ListExecute: _gfx.ExecuteList(); break;
            case IncidentEventKind.CopyPage: _gfx.CopyGamePage(DrawPage); break; // 0A96:0C3F([082E]) of the layers
            case IncidentEventKind.Font: Font(e.A); break;
            case IncidentEventKind.LoadPalette: _gfx.CopyLoadedToBase(); break; // 21FD:026C (its far argument is not used)
            case IncidentEventKind.FadeIn:
                BeforePresent?.Invoke(e);
                Present(_gfx.FadeInSteps());
                break;
            case IncidentEventKind.Pause: Present(RetraceSteps(e.A)); break;
            case IncidentEventKind.Emblem: Emblem(e.D, (short)e.A, (short)e.B, (short)e.C); break;
            case IncidentEventKind.Flip:
                BeforePresent?.Invoke(e);
                Flip();
                break;
            case IncidentEventKind.Text: _lines = e.B; break;
            case IncidentEventKind.Character: Character(e.A, e.B, e.C, e.D); break;
            case IncidentEventKind.Number: break;  // written into the text by the core
            case IncidentEventKind.Sound: break;   // the sound driver's
            case IncidentEventKind.WaitLoop: ApplyWaitLoop(e); break;
            default: throw new ArgumentOutOfRangeException(nameof(e), $"{e.Kind} is no event of the week incidents.");
        }
    }

    /// <summary>The drawing of the wait loop 21FD:0004(0): list, page flip, objects 23CD:071C/06E9 on the drawing page.</summary>
    private void ApplyWaitLoop(IncidentEvent e)
    {
        var inner = e.Inner ?? throw new ArgumentException("Wait loop event without its inner event.", nameof(e));
        switch (inner.Kind)
        {
            case WeekEventKind.Flip:
                BeforePresent?.Invoke(e);
                Flip();
                break;
            case WeekEventKind.ListExecute: _gfx.ExecuteList(); break;
            case WeekEventKind.ListClear: _gfx.ClearList(); break;
            case WeekEventKind.RestoreObjects: _gfx.RestoreObjects(DrawPage); break;
            case WeekEventKind.DrawObjects: _gfx.DrawObjects(DrawPage); break;
            case WeekEventKind.Sound: break;
            default: throw new ArgumentOutOfRangeException(nameof(e), $"{inner.Kind} is no event of the wait loop in mode 0.");
        }
    }

    private ushort DrawPage => (ushort)(_gfx.Ds[GraphicsLayers.DrawPageIndex] | _gfx.Ds[GraphicsLayers.DrawPageIndex + 1] << 8);

    private static ushort W(int v) => unchecked((ushort)v);

    private short LayerWord(int at) => (short)(_gfx.Ds[at] | _gfx.Ds[at + 1] << 8);

    /// <summary>[ORIGINAL] 21FD:051F as steps; the key table bytes the flip reads (DS:3D91, DS:3D95) as the core holds them.</summary>
    private void Flip()
    {
        MirrorKeys();
        Present(_gfx.FlipPagesSteps());
    }

    private void MirrorKeys()
    {
        for (int sc = 0; sc < KeyTable.Length; sc++) _gfx.Ds[DgroupLayout.KeyState + sc] = _state.U8(DgroupLayout.KeyState + sc);
    }

    /// <summary>[ORIGINAL] 21FD:0185(n) as steps: n retraces (the incidents only pass positive constants).</summary>
    private IEnumerable<int> RetraceSteps(int n)
    {
        for (int i = 0; i < n; i++)
        {
            _gfx.WaitRetrace();
            yield return 0;
        }
    }

    /// <summary>
    /// [ORIGINAL] 21FD:037D(rgb12) as steps: eight stages 0…7, each 21FD:03EB (working palette, on VGA 2295:00C5: retrace wait,
    /// then the DAC) and 21FD:0185(4); then the blend colour 21FD:04DA and 21AF:04CD (as FieldScene.Output.cs). The DAC values
    /// of a stage become visible after its retrace, so they are held back until the next frame.
    /// </summary>
    private IEnumerable<int> FadeOutSteps(ushort rgb12)
    {
        var dac = _gfx.Dac.Registers;
        bool vga = LayerWord(GraphicsLayers.CardMode) == 1; // DS:090E
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
            foreach (int s in RetraceSteps(4)) yield return s;
        }
        _gfx.SetBlendColor(rgb12);
        _gfx.InvalidateAllRestoreRecords();
    }

    /// <summary>
    /// [ORIGINAL] 0609:1879(x, y, badge) for player <paramref name="p"/> (DS:37AA at the call, carried by the event): emblem 30h ×
    /// 2Ch from atlas (C0h, 2Ch·p); for badge &gt; 0 the badge 10h × 0Bh from atlas ((badge − 1)·10h, 0Bh·p + C6h) at
    /// (x + 10h, y + DS:00D8[p]).
    /// </summary>
    private void Emblem(int p, short x, short y, short badge)
    {
        _gfx.AddBlit(0xC0, W(p * 0x2C), 0x30, 0x2C, W(x), W(y));
        if (badge <= 0) return;
        ushort dy = W(y + _state.U16(W(ReportLayout.EmblemBadgeOffsets + 2 * p)));
        _gfx.AddBlit(W((badge - 1) << 4), W(p * 0x0B + 0xC6), 0x10, 0x0B, W(x + 0x10), dy);
    }

    /// <summary>
    /// [ORIGINAL] 0A69:01AD, 0A69:0243, 0A69:0288 (drawing part, called by 17CB:09E0 with x 0, y 86h − 7·lines, width 140h) –
    /// character <paramref name="index"/> of line <paramref name="line"/> (text <paramref name="text"/>): DS:0140 = the
    /// character, then 0A96:0525 at x = 1 + (140h − strlen·[3CA8]) / 2 (unsigned) + index·[3CA8] and
    /// y = 86h − 7·lines + line·([34A4] + [34A4] / 5) (signed), colour, no shadow. The list was emptied before, 21FD:060D follows.
    /// </summary>
    private void Character(int text, int index, int colour, int line)
    {
        int at = W(MenuLayout.TextPointers + (text << 2));
        ushort offset = _state.U16(at), segment = _state.U16(W(at + 2));
        ushort length = IncidentText.Length(_texts, offset, segment);
        short advance = LayerWord(GraphicsLayers.FontAdvance), height = LayerWord(GraphicsLayers.FontLineHeight);
        ushort x0 = W(1 + (W(0x140 - length * advance) >> 1));
        ushort y = W(0x86 - 7 * _lines + W(height + height / 5) * line);
        _gfx.Ds[GraphicsLayers.TypewriterChar] = _texts.Read(segment, W(offset + index));
        _gfx.AddText(W(x0 + index * advance), y, GraphicsLayers.TypewriterChar, _gfx.DataSegment, W(colour), 0xFFFF);
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
}
