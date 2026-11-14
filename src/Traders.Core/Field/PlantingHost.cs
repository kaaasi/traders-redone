using Traders.Core.Ai;
using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.State;
using Traders.Core.Week;

namespace Traders.Core.Field;

/// <summary>
/// The planting phase's host with the ported routines of the other lanes (docs/arbeitspakete.md R5, lane anbau part 3): the
/// computer turn 135D:000E and its entries 0935/0AB3 (<see cref="PlantingAi"/>, lane anbau-ki), the field helpers
/// 0609:074D/100F (<see cref="PlantingAiHelpers"/>), the control reader 0E9E:037A, the pause key 21FD:0245 and the wait loop
/// 21FD:0004 in mode 1 (<see cref="Week.WaitLoop"/>, lane bilanz). Drawing and sound go to <paramref name="output"/> as
/// <see cref="FieldEvent"/>s; the wait loop's events are translated into the field's events in their order, and its
/// confirmation 0609:1FCC is the core's <see cref="Planting.ShowCursor(Dgroup, IFieldOutput, int)"/>.
/// </summary>
/// <param name="output">Receiver of the drawing and sound requests (the scene).</param>
/// <param name="devices">Control devices besides the key table (joystick, mouse); keyboard only if null.</param>
public sealed class PlantingHost(Action<FieldEvent> output, IControlDevices? devices = null) : IPlantingHost
{
    private readonly IControlDevices _devices = devices ?? KeyboardOnlyDevices.Instance;

    public void Emit(FieldEvent e) => output(e);

    // 135D:000E – called from 0609:047B (PlantingAi.ComputerTurn)
    public void ComputerTurn(Dgroup d, short x, short y) => PlantingAi.ComputerTurn(d, x, y);

    // 0609:074D – PlantingAiHelpers.CanPlant
    public bool CanPlaceHq(Dgroup d) => PlantingAiHelpers.CanPlant(d);

    // 0609:100F – PlantingAiHelpers.HasOwnNeighbour
    public bool HasOwnNeighbour(Dgroup d, short x, short y) => PlantingAiHelpers.HasOwnNeighbour(d, x, y);

    // 135D:0AB3 – called from 0609:0746 at the end of 069F (PlantingAi.ChooseWantKind)
    public void ComputerPlacedHq(Dgroup d) => PlantingAi.ChooseWantKind(d);

    // 135D:0935 – called from 0609:0961 in 07D6 (PlantingAi.CommitCandidate)
    public void ComputerBoughtField(Dgroup d) => PlantingAi.CommitCandidate(d);

    // 0E9E:037A – ControlReader.ReadActions
    public void ReadControls(Dgroup d) => ControlReader.ReadActions(d, _devices);

    // 21FD:0245 – Planting.Pause
    public IEnumerable<FieldWait> PauseCheck(Dgroup d) => Planting.Pause(d);

    /// <summary>
    /// 21FD:0004(mode) of lane bilanz frame by frame: after every step of the loop its events are played as field events (before
    /// the frame ends, so the scene draws them in the same frame), then the wait is passed on – a page flip or retrace as
    /// <see cref="FieldWait.Retrace"/>, a poll of the pause key as <see cref="FieldWait.Poll"/>.
    /// </summary>
    // 21FD:0004 – Week.WaitLoop (lane bilanz), called from 0609:00D6 with mode 1
    public IEnumerable<FieldWait> WaitLoop(Dgroup d, OriginalRandom random, short mode)
    {
        var loop = new WaitLoop(d, random, _devices, mode);
        foreach (var w in loop.Run())
        {
            Play(d, loop.DrainEvents());
            yield return w == MenuWait.Poll ? FieldWait.Poll : FieldWait.Retrace;
        }
        Play(d, loop.DrainEvents());
    }

    /// <summary>The wait loop's events (modes 1 and 2 without the report's mark) as the field's output requests.</summary>
    private void Play(Dgroup d, List<WeekEvent> events)
    {
        foreach (var e in events)
        {
            switch (e.Kind)
            {
                case WeekEventKind.Flip: Emit(new FieldEvent(FieldEventKind.FlipPages)); break;
                case WeekEventKind.ListExecute: Emit(new FieldEvent(FieldEventKind.RunList)); break;
                case WeekEventKind.ListClear: Emit(new FieldEvent(FieldEventKind.ClearList)); break;
                case WeekEventKind.RestoreObjects: Emit(new FieldEvent(FieldEventKind.RestoreObjects)); break;
                case WeekEventKind.DrawObjects: Emit(new FieldEvent(FieldEventKind.DrawObjects)); break;
                case WeekEventKind.Sound: Emit(new FieldEvent(FieldEventKind.Sound, (short)e.A, (short)e.B)); break;
                case WeekEventKind.ShowPlayerObject: Planting.ShowCursor(d, this, e.A); break; // 0609:1FCC
                default: throw new InvalidOperationException($"wait loop event {e.Kind} is not part of the planting phase");
            }
        }
    }
}
