using Traders.Core.Field;
using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.State;
using Traders.Core.Week;

namespace Traders.Core.FatMike;

/// <summary>
/// Fat Mike's host in the game: the control reader 0E9E:037A, the pause key 21FD:0245 (<see cref="Planting.Pause"/>),
/// the wait loop 21FD:0004 of lane bilanz (<see cref="Week.WaitLoop"/>) and the texts of the notice from the far text buffer
/// (<paramref name="farString"/>, the host's copy of the buffer loaded at the program start). Drawing and sound go to
/// <paramref name="field"/> and <paramref name="fatMike"/>; the wait loop's events are translated into field events in their order.
/// </summary>
/// <param name="field">Receiver of the requests shared with the field layer (the scene).</param>
/// <param name="fatMike">Receiver of the segment's own drawing requests (the scene).</param>
/// <param name="farString">Zero-terminated string at a far address, without the terminator.</param>
/// <param name="devices">Control devices besides the key table (joystick, mouse); keyboard only if null.</param>
/// <param name="secondRound">
/// The field's host for the contest's second field round 0609:0080 (<see cref="ContestFieldHost"/>); only the contest needs it.
/// </param>
public sealed class FatMikeHost(Action<FieldEvent> field, Action<FatMikeEvent> fatMike, Func<ushort, ushort, byte[]> farString,
    IControlDevices? devices = null, IPlantingHost? secondRound = null) : IContestHost
{
    private readonly IControlDevices _devices = devices ?? KeyboardOnlyDevices.Instance;

    // 0609:0080 – ContestPhase.SecondRoundFrames on the field's host
    public IEnumerable<FieldWait> SecondRound(Dgroup d, OriginalRandom random) =>
        ContestPhase.SecondRoundFrames(d, random,
            secondRound ?? throw new InvalidOperationException("The second field round needs the field's host."));

    public void Emit(FieldEvent e) => field(e);

    public void Emit(FatMikeEvent e) => fatMike(e);

    // 0E9E:037A – ControlReader.ReadActions
    public void ReadControls(Dgroup d) => ControlReader.ReadActions(d, _devices);

    // 21FD:0245 – Planting.Pause
    public IEnumerable<FieldWait> PauseCheck(Dgroup d) => Planting.Pause(d);

    public byte[] FarString(ushort offset, ushort segment) => farString(offset, segment);

    /// <summary>
    /// 21FD:0004(mode) of lane bilanz frame by frame: after every step of the loop its events are played as field events (before
    /// the frame ends, so the scene draws them in the same frame), then the wait is passed on – a page flip or retrace as
    /// <see cref="FieldWait.Retrace"/>, a poll of the pause key as <see cref="FieldWait.Poll"/>.
    /// </summary>
    // 21FD:0004 – Week.WaitLoop (lane bilanz), called from 1F63:08D1 with mode 0 and from 1F63:0C67 with mode 4
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

    /// <summary>The wait loop's events in modes 0 and 4 (no confirmation drawing; the indicator 1F63:104B in mode 4).</summary>
    private void Play(List<WeekEvent> events)
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
                case WeekEventKind.Indicator: Emit(new FatMikeEvent(FatMikeEventKind.Indicator, (short)e.A, (short)e.B)); break;
                default: throw new InvalidOperationException($"wait loop event {e.Kind} is not part of Fat Mike");
            }
        }
    }
}
