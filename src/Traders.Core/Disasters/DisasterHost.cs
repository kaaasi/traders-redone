using Traders.Core.Field;
using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.State;
using Traders.Core.Week;

namespace Traders.Core.Disasters;

/// <summary>
/// The catastrophes' host with the ported routines of the other lanes: the pause check 21FD:0245 (<see cref="Planting.Pause"/>)
/// and the wait loop 21FD:0004 in mode 3 (<see cref="Week.WaitLoop"/>, lane bilanz). Drawing and sound go to
/// <paramref name="output"/> as <see cref="DisasterEvent"/>s; the wait loop's events are translated in their order.
/// </summary>
/// <param name="output">Receiver of the drawing and sound requests (the scene).</param>
/// <param name="devices">Control devices besides the key table (joystick, mouse); keyboard only if null.</param>
public sealed class DisasterHost(Action<DisasterEvent> output, IControlDevices? devices = null) : IDisasterHost
{
    private readonly IControlDevices _devices = devices ?? KeyboardOnlyDevices.Instance;

    public void Emit(DisasterEvent e) => output(e);

    // 21FD:0245 – Planting.Pause
    public IEnumerable<FieldWait> PauseCheck(Dgroup d) => Planting.Pause(d);

    /// <summary>21FD:0004(mode) frame by frame; its events are played before each wait is passed on.</summary>
    // 21FD:0004 – Week.WaitLoop (lane bilanz), called from 111D:010A with mode 3
    public IEnumerable<FieldWait> WaitLoop(Dgroup d, OriginalRandom random, short mode)
    {
        var loop = new WaitLoop(d, random, _devices, mode);
        foreach (var w in loop.Run())
        {
            Play(loop.DrainEvents());
            yield return w == MenuWait.Poll ? FieldWait.Poll : FieldWait.Retrace;
        }
        Play(loop.DrainEvents());
    }

    private void Play(List<WeekEvent> events)
    {
        foreach (var e in events)
        {
            output(e.Kind switch
            {
                WeekEventKind.Flip => new DisasterEvent(DisasterEventKind.FlipPages),
                WeekEventKind.ListExecute => new DisasterEvent(DisasterEventKind.ExecuteList),
                WeekEventKind.ListClear => new DisasterEvent(DisasterEventKind.ClearList),
                WeekEventKind.RestoreObjects => new DisasterEvent(DisasterEventKind.RestoreObjects),
                WeekEventKind.DrawObjects => new DisasterEvent(DisasterEventKind.DrawObjects),
                WeekEventKind.Sound => new DisasterEvent(DisasterEventKind.Sound, (short)e.A, (short)e.B),
                _ => throw new InvalidOperationException($"wait loop event {e.Kind} is not part of the catastrophes"),
            });
        }
    }
}
