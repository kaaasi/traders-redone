using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.Rules;
using Traders.Core.State;

namespace Traders.Core.Week;

/// <summary>DS offsets of the week report 0825:000A (docs/routines/0825_000A_wochenbilanz.md).</summary>
public static class ReportLayout
{
    /// <summary>word – picture number of the report screen (first word of the preload table DS:08CE).</summary>
    public const int Picture = 0x08CE;

    /// <summary>word, signed – scale of the wealth graph, set by 0825:02D7.</summary>
    public const int GraphScale = 0x1584;

    /// <summary>DS offset of the palette the report loads with 21FD:026C.</summary>
    public const int Palette = 0x00A8;

    /// <summary>Divisor of the richest player's wealth in 0825:02D7.</summary>
    public const int ScaleDivisor = 0x78;

    /// <summary>word[4] – colour of each player's numbers and graph line (initialised data, read by 0825:0178 and 0560).</summary>
    public const int PlayerColors = 0x00C8;

    /// <summary>word[4] – y offset of the rank badge in each player's emblem (initialised data, read by 0609:1879).</summary>
    public const int EmblemBadgeOffsets = 0x00D8;

    /// <summary>Far text pointer of the report's title (text 0Ah of the table DS:37CC, drawn by 0825:0072).</summary>
    public const int TitleText = MenuLayout.TextPointers + 4 * 0x0A;

    /// <summary>Far text pointers of the five row labels (texts 0Bh…0Fh) and of the total row (text 10h), 0825:0178.</summary>
    public const int RowTexts = MenuLayout.TextPointers + 4 * 0x0B, TotalText = MenuLayout.TextPointers + 4 * 0x10;
}

/// <summary>
/// The week report 0825:000A(arg) as a coroutine of the core: ranking, the screen (drawing reported as <see cref="WeekEvent"/>s,
/// with the values the core computed), the scale of the wealth graph DS:1584 and – with arg = 1 – the opening wipe and the wait
/// loop 21FD:0004 in mode 2. Wherever the original waits (page flips, palette steps, fade-in, pause) the coroutine yields a
/// <see cref="MenuWait"/> and continues in the next frame. Called with arg = 1 from the week loop (1777:0208, call 3) and with
/// arg = 0 from 20B3:007D (end of the game, no wait).
/// </summary>
// [ORIGINAL] 0825:000A
public sealed class Report
{
    private readonly Dgroup _d;
    private readonly int _arg;
    private readonly List<WeekEvent> _events = [];

    public Report(Dgroup state, OriginalRandom random, IControlDevices devices, int arg)
    {
        _d = state;
        _arg = arg;
        WaitLoop = new WaitLoop(state, random, devices, 2);
    }

    /// <summary>The wait loop of the report (runs only with arg = 1); its events are merged into <see cref="Events"/>.</summary>
    public WaitLoop WaitLoop { get; }

    private long _frame;

    /// <summary>Frame stamped on new events (set by the driver).</summary>
    public long Frame
    {
        get => _frame;
        set
        {
            _frame = value;
            WaitLoop.Frame = value;
        }
    }

    /// <summary>True when the wait loop ended through the abort key (the week loop then returns to the menu, 1777:020E).</summary>
    public bool Aborted => WaitLoop.Aborted;

    public IReadOnlyList<WeekEvent> Events
    {
        get
        {
            Merge();
            return _events;
        }
    }

    public List<WeekEvent> DrainEvents()
    {
        Merge();
        var list = new List<WeekEvent>(_events);
        _events.Clear();
        return list;
    }

    private void Merge() => _events.AddRange(WaitLoop.DrainEvents());

    // Retrace waits (docs/routines/21FD_026C_paletten.md, as in MainMenu): slow fade-in 0329 eight stages of one retrace and
    // 0185(3); a palette step 03EB one retrace (2295:00C5); 21FD:0590 the show-page retrace of 0B67:0238 and 0B67:028F.
    private const int SlowFadeInRetraces = 8 * 4, PaletteStepRetraces = 1, DrawOnShownPageRetraces = 2;

    /// <summary>[ORIGINAL] 0825:000A – the whole report up to its return.</summary>
    public IEnumerable<MenuWait> Run()
    {
        Ranking.Apply(_d); // 0825:06CB
        Emit(WeekEventKind.LoadPicture, _d.U16(ReportLayout.Picture));
        if (_arg == 1)
            foreach (var w in Opening()) yield return w;
        DrawLoop(0x0072);
        Columns();
        Scale(_d);
        Emit(WeekEventKind.Draw, 0x0364);
        Emit(WeekEventKind.Draw, 0x047D);
        Emit(WeekEventKind.Draw, 0x0560);
        // 0825:0560 runs its player loop only from week index 0 on (di = 0 > DS:4B9A ends it at once).
        if (_d.S16(DgroupLayout.Week) >= 0) _d.Set(DgroupLayout.ActivePlayer, (sbyte)DgroupLayout.PlayerCount);
        Emit(WeekEventKind.CopyPage);
        Emit(WeekEventKind.LoadPalette, ReportLayout.Palette);
        Emit(WeekEventKind.FadeIn);
        for (int i = 0; i < SlowFadeInRetraces; i++) yield return MenuWait.Retrace;
        if (_arg != 1) yield break;
        foreach (var w in WaitLoop.Run()) // 21FD:0004(2)
        {
            Merge();
            yield return w;
        }
        Merge();
    }

    /// <summary>
    /// [ORIGINAL] 0825:09C9 – opening wipe: 21FD:0590, list emptied, 320 column pairs drawn (drawing only); from column C0h on
    /// every 16th pair a palette step 21FD:03EB(0, 1…8); then blend colour 0, page flip, object restore records invalidated.
    /// </summary>
    private IEnumerable<MenuWait> Opening()
    {
        Emit(WeekEventKind.DrawOnShownPage);
        for (int i = 0; i < DrawOnShownPageRetraces; i++) yield return MenuWait.Retrace;
        // The list calls of the wipe itself (0A96:001B, 0433, 06A8) are the scene's drawing of 09C9.
        int stage = 0;
        for (short si = 0; si <= 0x13F; si++)
        {
            if (si < 0xC0 || si % 16 != 0) continue;
            Emit(WeekEventKind.PaletteStep, 0, ++stage);
            for (int i = 0; i < PaletteStepRetraces; i++) yield return MenuWait.Retrace;
        }
        Emit(WeekEventKind.BlendColor, 0);
        foreach (var w in Flip()) yield return w;
        Emit(WeekEventKind.InvalidateObjects);
    }

    /// <summary>
    /// [ORIGINAL] 0825:0072 – drawing routine whose loop over the players runs in DS:37AA (left at 4).
    /// </summary>
    private void DrawLoop(int routine)
    {
        Emit(WeekEventKind.Draw, routine);
        _d.Set(DgroupLayout.ActivePlayer, (sbyte)DgroupLayout.PlayerCount);
    }

    /// <summary>
    /// [ORIGINAL] 0825:0178 – the columns of the table: per player (DS:37AA) land, wares, gems, other and total wealth
    /// (0825:086F, 088D, 08EB, 096D, 0813; 088D and 0813 leave DS:45D0 = 4). The values go to the scene with the event.
    /// </summary>
    private void Columns()
    {
        var values = new List<int>(5 * DgroupLayout.PlayerCount);
        for (_d.Set(DgroupLayout.ActivePlayer, (sbyte)0); _d.S8(DgroupLayout.ActivePlayer) < 4;
             _d.Set(DgroupLayout.ActivePlayer, (sbyte)(_d.S8(DgroupLayout.ActivePlayer) + 1)))
        {
            values.Add(Wealth.Land(_d));
            values.Add(Wealth.Wares(_d));
            values.Add(Wealth.Gems(_d));
            values.Add(Wealth.Other(_d));
            values.Add(Wealth.Total(_d));
        }
        _events.Add(new WeekEvent(WeekEventKind.Draw, Frame, 0x0178, Values: values));
    }

    /// <summary>
    /// [ORIGINAL] 0825:02D7 – scale of the wealth graph: wealth DS:2F06 of the (last) player with rank DS:3D42 = 0, divided by
    /// 120 (LDIV), plus 1, as a word; above 10 rounded up to the next multiple of 10 above (n/10·10 + 10), above 100 likewise to
    /// 100. Loop variable DS:37AA ends at 4.
    /// </summary>
    public static void Scale(Dgroup d)
    {
        int richest = 0;
        // [APPROX] With no player at rank 0 the original divides an uninitialised local; 0825:06CB always leaves one at rank 0.
        for (d.Set(DgroupLayout.ActivePlayer, (sbyte)0); d.S8(DgroupLayout.ActivePlayer) < 4;
             d.Set(DgroupLayout.ActivePlayer, (sbyte)(d.S8(DgroupLayout.ActivePlayer) + 1)))
        {
            int p = d.S8(DgroupLayout.ActivePlayer);
            if (d.U8(DgroupLayout.Rank + p) == 0) richest = d.S32(DgroupLayout.CurrentWealth + 4 * p);
        }
        short scale = Int16Math.S(LongMath.Div(richest, ReportLayout.ScaleDivisor) + 1);
        if (scale > 10) scale = Int16Math.S(Int16Math.S(Int16Math.Div(scale, 10) * 10) + 10);
        if (scale > 100) scale = Int16Math.S(Int16Math.S(Int16Math.Div(scale, 100) * 100) + 100);
        d.Set(ReportLayout.GraphScale, scale);
    }

    /// <summary>[ORIGINAL] 21FD:051F – page flip as in <see cref="MainMenu"/> (keypad plus/minus, one frame per retrace).</summary>
    private IEnumerable<MenuWait> Flip()
    {
        Emit(WeekEventKind.Flip);
        yield return MenuWait.Flip;
        if (!Key(MenuKeys.KeypadPlus)) yield return MenuWait.Retrace;
        if (!Key(MenuKeys.KeypadMinus)) yield break;
        for (int i = 0; i < 4; i++) yield return MenuWait.Retrace;
    }

    private bool Key(byte scancode) => _d.U8(DgroupLayout.KeyState + scancode) != 0;

    private void Emit(WeekEventKind kind, int a = 0, int b = 0)
    {
        Merge();
        _events.Add(new WeekEvent(kind, Frame, a, b));
    }
}

/// <summary>
/// The week report as a step of the week loop (<see cref="IWeekStepHandler"/>, call 3 = 0825:000A(1)): <see cref="Begin"/> runs
/// it to its first wait, every <see cref="Tick"/> one frame further. The machine writes the key table and keeps DS:10D2 in
/// sync; the abort key (DS:3D55) is evaluated by the week loop after the step (1777:020E).
/// </summary>
public sealed class ReportStep(IControlDevices? devices = null, int arg = 1) : IWeekStepHandler
{
    private IEnumerator<MenuWait>? _run;

    public Report? Report { get; private set; }

    public StepOutcome Begin(StepContext context)
    {
        Report = new Report(context.State, context.Random, devices ?? KeyboardOnlyDevices.Instance, arg);
        _run = Report.Run().GetEnumerator();
        return Tick(context);
    }

    public StepOutcome Tick(StepContext context)
    {
        if (_run is null || Report is null) throw new InvalidOperationException("Not started.");
        Report.Frame = context.Frame;
        return _run.MoveNext() ? StepOutcome.Running : StepOutcome.Finished();
    }
}
