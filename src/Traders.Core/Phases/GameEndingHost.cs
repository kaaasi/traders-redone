using Traders.Core.Ending;
using Traders.Core.Field;
using Traders.Core.State;
using Traders.Core.Week;

namespace Traders.Core.Phases;

/// <summary>
/// The end screens' host in the game (call 19 where the game ends, <see cref="WeekScreens.Ending"/>): field and segment
/// requests go to the output, the pixel reads to the core's rule page (the default of <see cref="IEndingHost"/>), and
/// 0825:000A(0) is the report of lane bilanz (<see cref="EndScreens.Report"/>), its requests passed on after every frame.
/// </summary>
/// <param name="field">Receiver of the field-layer requests.</param>
/// <param name="ending">Receiver of the requests of segment 20B3.</param>
/// <param name="report">Receiver of the requests of the report 0825:000A(0).</param>
public sealed class GameEndingHost(Action<FieldEvent> field, Action<EndingEvent> ending, Action<WeekEvent> report) : IEndingHost
{
    public void ComputerTurn(Dgroup d, short x, short y) =>
        throw new InvalidOperationException("The end screens have no computer turn.");

    public void Emit(FieldEvent e) => field(e);

    public void Emit(EndingEvent e) => ending(e);

    public IEnumerable<FieldWait> Report(Dgroup d, OriginalRandom random) =>
        EndScreens.Report(d, random, events =>
        {
            foreach (var e in events) report(e);
        });
}
