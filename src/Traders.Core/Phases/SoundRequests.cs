using Traders.Core.Disasters;
using Traders.Core.Ending;
using Traders.Core.Events;
using Traders.Core.Field;
using Traders.Core.Purchase;
using Traders.Core.Stations;
using Traders.Core.Trade;
using Traders.Core.Week;

namespace Traders.Core.Phases;

/// <summary>
/// The sound requests of the week's steps on the program's sound control (lane partie part 2): every 21A7:0054(a, b) a step
/// reports as an output event (sound kind of its event type) runs <see cref="ProgramSound.Effect"/> at the place of the
/// request, before the event goes on to the scene; the end screens' 218B:0094(a, b) (20B3:0060) runs
/// <see cref="ProgramSound.Music"/>. Steps whose events the core collects (week report, incidents) are handled after each of
/// their frames by <see cref="SoundAfterStep"/>. Without a sound control the receivers are returned unchanged.
/// </summary>
// [ORIGINAL] 21A7:0054, 218B:0094 – called by the week's steps (144 call sites of 21A7:0054; 20B3:0060 for the end tune)
public static class SoundRequests
{
    public static Action<FieldEvent> Field(Action<FieldEvent> output, ProgramSound? sound) => sound is null ? output : e =>
    {
        if (e.Kind == FieldEventKind.Sound) sound.Effect(e.Args[0], e.Args[1]);
        output(e);
    };

    public static Action<DisasterEvent> Disasters(Action<DisasterEvent> output, ProgramSound? sound) => sound is null ? output : e =>
    {
        if (e.Kind == DisasterEventKind.Sound) sound.Effect(e.Args[0], e.Args[1]);
        output(e);
    };

    public static Action<StationEvent> Stations(Action<StationEvent> output, ProgramSound? sound) => sound is null ? output : e =>
    {
        if (e.Call == StationCall.Sound) sound.Effect(e.Args[0], e.Args[1]);
        output(e);
    };

    public static Action<TradeEvent> Trade(Action<TradeEvent> output, ProgramSound? sound) => sound is null ? output : e =>
    {
        if (e.Kind == TradeEventKind.Sound) sound.Effect(e.Args[0], e.Args[1]);
        output(e);
    };

    public static Action<PurchaseEvent> Purchase(Action<PurchaseEvent> output, ProgramSound? sound) => sound is null ? output : e =>
    {
        if (e.Kind == PurchaseEventKind.Sound) sound.Effect(e.Args[0], e.Args[1]);
        output(e);
    };

    public static Action<EndingEvent> Ending(Action<EndingEvent> output, ProgramSound? sound) => sound is null ? output : e =>
    {
        if (e.Kind == EndingEventKind.Music) sound.Music(e.Args[0], e.Args[1]);
        output(e);
    };

    public static Action<WeekEvent> Week(Action<WeekEvent> output, ProgramSound? sound) => sound is null ? output : e =>
    {
        if (e.Kind == WeekEventKind.Sound) sound.Effect(e.A, e.B);
        output(e);
    };

    /// <summary>The sound requests among the week report's events (call 3).</summary>
    public static IEnumerable<(int A, int B)> Of(IEnumerable<WeekEvent> events) =>
        events.Where(e => e.Kind == WeekEventKind.Sound).Select(e => (e.A, e.B));

    /// <summary>The sound requests among the incidents' events (call 9).</summary>
    public static IEnumerable<(int A, int B)> Of(IEnumerable<IncidentEvent> events) =>
        events.Where(e => e.Kind == IncidentEventKind.Sound).Select(e => (e.A, e.B));
}

/// <summary>
/// A week step whose events the core collects for the host (week report, incidents): after each <see cref="Begin"/> and
/// <see cref="Tick"/> the sound requests it added in that frame run on the sound control, in their order. The host drains the
/// collected events between frames; requests already run are not run again.
/// </summary>
/// <param name="inner">The step.</param>
/// <param name="requests">The sound requests among the events the step holds now (oldest first), or null.</param>
/// <param name="sound">The program's sound control.</param>
// [APPROX] The requests of a frame run when the step returns from that frame, after the rest of the frame's work of the step
// instead of at their exact place in it; nothing the step reads in that frame depends on them (21A7:0054 writes only
// DS:10A5 and the sound state).
public sealed class SoundAfterStep(IWeekStepHandler inner, Func<IReadOnlyList<(int A, int B)>?> requests, ProgramSound sound)
    : IWeekStepHandler
{
    private int _done;

    public StepOutcome Begin(StepContext context) => After(() => inner.Begin(context));

    public StepOutcome Tick(StepContext context) => After(() => inner.Tick(context));

    private StepOutcome After(Func<StepOutcome> call)
    {
        _done = Math.Min(_done, requests()?.Count ?? 0); // drained by the host since
        var outcome = call();
        var now = requests() ?? [];
        for (int i = _done; i < now.Count; i++) sound.Effect(now[i].A, now[i].B);
        _done = now.Count;
        return outcome;
    }
}
