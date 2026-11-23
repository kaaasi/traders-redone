using Traders.App.Deviations;
using Traders.Core;
using Traders.Core.Phases;
using Traders.Core.State;

namespace Traders.Tests.App;

/// <summary>
/// The host's menu switch (decision F3 = B of R9, <see cref="MenuAfterGameOver"/>) on the program machine with stand-in steps:
/// the core ends main after the week loop's exit B (game over) as the original does (1777:0184 → teardown 018C); with the
/// switch the host goes on at the loop start 1777:013B in the next frame, as after exit A. Menu result 0 and errors of the
/// initialisation still end the program.
/// </summary>
public class MenuAfterGameOverTests
{
    private sealed class Instant(Func<Dgroup, byte>? result = null) : IProgramStepHandler
    {
        public StepOutcome Begin(ProgramStepContext c) => StepOutcome.Finished(result?.Invoke(c.State) ?? 0);

        public StepOutcome Tick(ProgramStepContext c) => throw new InvalidOperationException("instant step ticked");
    }

    private sealed class WeekInstant(byte endCheck) : IWeekStepHandler
    {
        public StepOutcome Begin(StepContext c) => StepOutcome.Finished(c.Step.Number == WeekSteps.EndCall ? endCheck : (byte)0);

        public StepOutcome Tick(StepContext c) => throw new InvalidOperationException();
    }

    /// <summary>Menu: "new game" and AL = 1 for the first <paramref name="games"/> entries, then AL = 0 (quit).</summary>
    private static Dictionary<int, IProgramStepHandler> Program(int games, Func<int, byte>? initialisation = null)
    {
        int menus = 0;
        var d = ProgramSteps.Defaults();
        if (initialisation is not null) d[1] = new Instant(_ => initialisation(0));
        d[2] = new Instant();
        d[ProgramLoop.MainMenu] = new Instant(s =>
        {
            s.Set(ProgramLoop.NewGameRequest, (byte)1);
            return (byte)(++menus <= games ? 1 : 0);
        });
        d[ProgramLoop.NewGame] = new Instant();
        return d;
    }

    /// <summary>Week loop that ends the game at the end check of its first week (exit B).</summary>
    private static Dictionary<int, IWeekStepHandler> GameOver() =>
        WeekLoop.Steps.Where(s => s.Kind == WeekStepKind.Call).ToDictionary(s => s.Number, s => (IWeekStepHandler)new WeekInstant(1));

    private static (List<ProgramEvent> Events, ProgramMachine Last, MenuAfterGameOver? Switch) Run(bool returnToMenu, int games,
        Func<int, byte>? initialisation = null)
    {
        var state = new Dgroup();
        state.Set(ProgramInitialisation.GraphicsCard, (short)-1);
        state.Set(ProgramInitialisation.SoundDevice, (short)-1);
        var random = new OriginalRandom(new RecordedClock([]), 1);
        var steps = Program(games, initialisation);
        var week = GameOver();
        var machine = new ProgramMachine(state, random, steps, week);
        var menu = returnToMenu ? new MenuAfterGameOver(state, random, steps, week) : null;
        var events = new List<ProgramEvent>();
        for (int f = 0; f < 40 && machine.Status != ProgramStatus.Ended; f++)
        {
            machine.Advance(new PhaseTick(machine.LastFrame + 1));
            var frame = machine.DrainEvents();
            events.AddRange(frame);
            if (menu?.Continue(machine, frame) is { } next) machine = next;
        }
        return (events, machine, menu);
    }

    private static List<int> Started(IEnumerable<ProgramEvent> events) =>
        events.Where(e => e.Kind == ProgramEventKind.StepStarted).Select(e => e.Step).ToList();

    /// <summary>Without the switch: the program ends after the game over (step 10), no second menu – the original's way.</summary>
    [Fact]
    public void Without_the_switch_the_program_ends_after_the_game()
    {
        var (events, last, _) = Run(returnToMenu: false, games: 2);
        Assert.Equal(ProgramStatus.Ended, last.Status);
        Assert.Equal([1, 2, 3, 4, 5, 6, 7, 8, 9, 10], Started(events));
        var end = Assert.Single(events, e => e.Kind == ProgramEventKind.Ended);
        Assert.Equal(ProgramLoop.WeekLoopStep, end.Step);
        Assert.Equal(0, last.ExitCode);
    }

    /// <summary>
    /// With the switch: after each game over main goes on at the loop start (music query, effect, music, main menu) in the next
    /// frame; a second game ends the same way, and the menu result 0 ends the program at last (exit code 0). Initialisation
    /// and program start run once only.
    /// Counter-check: <see cref="MenuAfterGameOver.Continue"/> always null or resuming at step 8 → red.
    /// </summary>
    [Fact]
    public void With_the_switch_the_loop_start_follows_every_game_over()
    {
        var (events, last, menu) = Run(returnToMenu: true, games: 2);
        Assert.Equal([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 5, 6, 7, 8, 9, 10, 5, 6, 7, 8], Started(events));
        Assert.Equal(2, menu!.Returns);
        var ends = events.Where(e => e.Kind == ProgramEventKind.Ended).ToList();
        Assert.Equal([ProgramLoop.WeekLoopStep, ProgramLoop.WeekLoopStep, ProgramLoop.MainMenu], ends.Select(e => e.Step));
        Assert.Equal(ProgramStatus.Ended, last.Status);
        Assert.Equal(0, last.ExitCode);
        // the loop start runs in the frame after the end of the game
        var firstEnd = ends[0];
        var resumed = events.First(e => e.Kind == ProgramEventKind.StepStarted && e.Frame > firstEnd.Frame);
        Assert.Equal((ProgramLoop.LoopStart, firstEnd.Frame + 1), (resumed.Step, resumed.Frame));
    }

    /// <summary>With the switch the menu result 0 and an error of the initialisation still end the program.</summary>
    [Fact]
    public void With_the_switch_other_ends_stay_ends()
    {
        var (quit, last, menu) = Run(returnToMenu: true, games: 0);
        Assert.Equal([1, 2, 3, 4, 5, 6, 7, 8], Started(quit));
        Assert.Equal(ProgramStatus.Ended, last.Status);
        Assert.Equal(0, menu!.Returns);

        var (error, failed, menu2) = Run(returnToMenu: true, games: 1, initialisation: _ => ProgramInitialisation.NoGraphicsCard);
        Assert.Equal([1], Started(error));
        Assert.Equal(ProgramInitialisation.NoGraphicsCard, failed.ExitCode);
        Assert.Equal(0, menu2!.Returns);
    }
}
