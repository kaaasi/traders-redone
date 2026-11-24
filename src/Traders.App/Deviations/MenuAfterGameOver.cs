using Traders.Core;
using Traders.Core.Phases;
using Traders.Core.State;

namespace Traders.App.Deviations;

/// <summary>
/// The host's return to the main menu after the end of a game. In the original the week loop returns AL = 1 once the end check
/// (call 19) has ended the game and main goes to its teardown (1777:0184 → 018C): the program ends. With this switch on (default)
/// the host lets main go on at its loop start 1777:013B instead, as after exit A (AL = 0): music query, sound effect, menu
/// music and the main menu run again on the data segment as the end of the game left it. The core itself stays faithful: its
/// program machine ends at the teardown, and the host continues main with a new machine at the loop start (switch
/// <c>--original-ende</c> turns this off).
/// </summary>
// [APPROX] bewusste Abweichung, Nutzer 23.11.2026 (R9 F3 = B): back to the main menu instead of the end of the program. The
// machine ends in the frame the week loop returns; the loop start runs in the next frame (the original has no such step).
// DS:08FA stays 1 as after exit A, so the menu asks "new game or continue" before a game (192D:12A6); "continue" runs the
// week loop of the ended game again. The core's machine has run the sound part of the teardown 1777:018C (music off 218B:0153,
// effects off 21A7:0043) when it ended; the loop start then starts the menu music again (218B:013A = 0).
public sealed class MenuAfterGameOver(
    Dgroup state,
    OriginalRandom random,
    IReadOnlyDictionary<int, IProgramStepHandler> handlers,
    IReadOnlyDictionary<int, IWeekStepHandler> weekHandlers,
    ProgramSound? sound = null)
{
    /// <summary>Returns to the main menu so far.</summary>
    public int Returns { get; private set; }

    /// <summary>True if <paramref name="events"/> hold the end of main after the week loop's exit B (game over).</summary>
    public static bool EndedByGameOver(IEnumerable<ProgramEvent> events) =>
        events.Any(e => e.Kind == ProgramEventKind.Ended && e.Step == ProgramLoop.WeekLoopStep);

    /// <summary>
    /// The machine that goes on where <paramref name="machine"/> ended after the end of a game (<paramref name="events"/> are
    /// its events of the last frame): main at the loop start 1777:013B from the next frame on. Null if the machine did not end
    /// that way (still running, menu result 0, error of the initialisation).
    /// </summary>
    public ProgramMachine? Continue(ProgramMachine machine, IReadOnlyList<ProgramEvent> events)
    {
        if (machine.Status != ProgramStatus.Ended || !EndedByGameOver(events)) return null;
        Returns++;
        return new ProgramMachine(state, random, handlers, weekHandlers, ProgramLoop.LoopStart, machine.LastFrame, sound);
    }
}
