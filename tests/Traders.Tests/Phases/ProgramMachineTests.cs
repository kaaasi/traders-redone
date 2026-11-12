using Traders.Core;
using Traders.Core.Phases;
using Traders.Core.State;

namespace Traders.Tests.Phases;

/// <summary>
/// The program machine without original data: step table against the statically read finding, the result checks of main,
/// the guard DS:3CCC, DS:08FA = 1, the loop back after the week loop, pending steps, frames and commands.
/// </summary>
public class ProgramMachineTests
{
    /// <summary>
    /// main 1777:0119..018A as read in docs/routines/1777_000C_main.md (table "Ablauf"): site, far target, C arguments.
    /// Written down from the finding, not from <see cref="ProgramLoop"/>.
    /// </summary>
    private static readonly (ushort Site, ushort Seg, ushort Off, int[] Args)[] Finding =
    [
        (0x0119, 0x1777, 0x04E4, []),
        (0x0126, 0x0DFD, 0x000D, []),
        (0x012B, 0x218B, 0x013A, []),
        (0x0136, 0x21A7, 0x000D, []),
        (0x013B, 0x218B, 0x013A, []),
        (0x014D, 0x21A7, 0x0054, [0, 1]),
        (0x015C, 0x218B, 0x0094, [1, 2]),
        (0x0163, 0x192D, 0x0007, []),
        (0x0175, 0x0E31, 0x0003, []),
        (0x017F, 0x1777, 0x01F8, []),
    ];

    private sealed class Instant(Func<Dgroup, byte>? result = null) : IProgramStepHandler
    {
        public int Calls { get; private set; }
        public StepOutcome Begin(ProgramStepContext c) { Calls++; return StepOutcome.Finished(result?.Invoke(c.State) ?? 0); }
        public StepOutcome Tick(ProgramStepContext c) => throw new InvalidOperationException("instant step ticked");
    }

    private sealed class Waiting(int frames) : IProgramStepHandler
    {
        public List<long> Ticked { get; } = [];
        public List<IPlayerCommand> Received { get; } = [];
        public StepOutcome Begin(ProgramStepContext c) => StepOutcome.Running;

        public StepOutcome Tick(ProgramStepContext c)
        {
            Ticked.Add(c.Frame);
            c.Random.Next(9);
            while (c.TakeCommand() is { } cmd) Received.Add(cmd);
            return Ticked.Count >= frames ? StepOutcome.Finished(1) : StepOutcome.Running;
        }
    }

    private sealed class WeekInstant(Func<Dgroup, byte>? result = null, Action<Dgroup>? effect = null) : IWeekStepHandler
    {
        public StepOutcome Begin(StepContext c) { effect?.Invoke(c.State); return StepOutcome.Finished(result?.Invoke(c.State) ?? 0); }
        public StepOutcome Tick(StepContext c) => throw new InvalidOperationException();
    }

    private sealed record Key(int Player) : IPlayerCommand;

    /// <summary>Menu: AL = 1 and "new game" requested; the second entry ends the program (AL = 0).</summary>
    private static Dictionary<int, IProgramStepHandler> Program(Dictionary<int, IProgramStepHandler>? overrides = null)
    {
        int menus = 0;
        var d = ProgramSteps.Defaults();
        d[2] = new Instant();
        d[8] = new Instant(s =>
        {
            s.Set(ProgramLoop.NewGameRequest, (byte)1);
            return (byte)(++menus == 1 ? 1 : 0);
        });
        d[9] = new Instant();
        foreach (var (k, v) in overrides ?? []) d[k] = v;
        return d;
    }

    /// <summary>Week loop with exit A after call 3 (abort key set by call 3).</summary>
    private static Dictionary<int, IWeekStepHandler> WeekExitA() =>
        WeekLoop.Steps.Where(s => s.Kind == WeekStepKind.Call).ToDictionary(s => s.Number,
            s => (IWeekStepHandler)new WeekInstant(effect: s.Number == 3 ? d => d.Set(WeekLoop.AbortKeyState, (byte)1) : null));

    /// <summary>Week loop with exit B (end check returns 1).</summary>
    private static Dictionary<int, IWeekStepHandler> WeekExitB() =>
        WeekLoop.Steps.Where(s => s.Kind == WeekStepKind.Call).ToDictionary(s => s.Number,
            s => (IWeekStepHandler)new WeekInstant(result: s.Number == 19 ? _ => 1 : null));

    private static Dgroup NewState()
    {
        var d = new Dgroup();
        d.Set(ProgramInitialisation.GraphicsCard, (short)-1);
        d.Set(ProgramInitialisation.SoundDevice, (short)-1);
        return d;
    }

    private static ProgramMachine Machine(Dgroup state, Dictionary<int, IProgramStepHandler> handlers,
        Dictionary<int, IWeekStepHandler> week) => new(state, new OriginalRandom(new RecordedClock([]), 1), handlers, week);

    private static List<ProgramEvent> RunAll(ProgramMachine m, int frames = 20)
    {
        var events = new List<ProgramEvent>();
        for (int f = 0; f < frames && m.Status != ProgramStatus.Ended; f++)
        {
            m.Advance(new PhaseTick(m.LastFrame + 1));
            events.AddRange(m.DrainEvents());
        }
        return events;
    }

    private static List<int> Started(IEnumerable<ProgramEvent> events) =>
        events.Where(e => e.Kind == ProgramEventKind.StepStarted).Select(e => e.Step).ToList();

    [Fact]
    public void Step_table_equals_the_finding()
    {
        Assert.Equal(Finding.Length, ProgramLoop.Steps.Count);
        for (int i = 0; i < Finding.Length; i++)
        {
            var s = ProgramLoop.Steps[i];
            Assert.Equal(i + 1, s.Number);
            Assert.Equal((Finding[i].Site, Finding[i].Seg, Finding[i].Off), (s.Site, s.TargetSegment, s.TargetOffset));
            Assert.Equal(Finding[i].Args, s.Arguments);
        }
        Assert.Equal(ProgramStepKind.WeekLoop, ProgramLoop.Step(ProgramLoop.WeekLoopStep).Kind);
        Assert.Equal((WeekLoop.Segment, WeekLoop.Entry), (ProgramLoop.Step(10).TargetSegment, ProgramLoop.Step(10).TargetOffset));
        Assert.Equal(0x013B, ProgramLoop.Step(ProgramLoop.LoopStart).Site);
    }

    /// <summary>
    /// New game, exit A back to the loop start (music query, effect, music, menu), menu AL = 0 ends the program with exit code 0.
    /// Initialisation and program start run once only.
    /// </summary>
    [Fact]
    public void Exit_A_goes_back_to_the_loop_start_and_menu_zero_ends()
    {
        var state = NewState();
        var m = Machine(state, Program(), WeekExitA());
        var events = RunAll(m);

        Assert.Equal(ProgramStatus.Ended, m.Status);
        Assert.Equal([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 5, 6, 7, 8], Started(events));
        var end = Assert.Single(events, e => e.Kind == ProgramEventKind.Ended);
        Assert.Equal((ProgramLoop.MainMenu, 0), (end.Step, end.Value));
        Assert.Equal(0, m.ExitCode);
        Assert.Equal(1, state.U8(ProgramLoop.GameRunning));
        Assert.Null(m.WeekMachine);
    }

    /// <summary>Exit B (game over, AL = 1) ends the program after the week loop; no second menu.</summary>
    [Fact]
    public void Game_over_ends_the_program()
    {
        var m = Machine(NewState(), Program(), WeekExitB());
        var events = RunAll(m);

        Assert.Equal(ProgramStatus.Ended, m.Status);
        Assert.Equal([1, 2, 3, 4, 5, 6, 7, 8, 9, 10], Started(events));
        var end = Assert.Single(events, e => e.Kind == ProgramEventKind.Ended);
        Assert.Equal((ProgramLoop.WeekLoopStep, 0), (end.Step, end.Value));
        Assert.Contains(m.DrainWeekEvents(), e => e.Kind == PhaseEventKind.Exited && e.Value == WeekLoop.ResultGameOver);
    }

    /// <summary>Music query AL ≠ 0 skips the sound source (1777:0134) and, at the loop start, effect and music (1777:0144).</summary>
    [Fact]
    public void Music_query_not_zero_skips_the_sound_calls()
    {
        var m = Machine(NewState(), Program(new() { [3] = new Instant(_ => 1), [5] = new Instant(_ => 1) }), WeekExitA());
        var events = RunAll(m);

        Assert.Equal([1, 2, 3, 5, 8, 9, 10, 5, 8], Started(events));
    }

    /// <summary>Initialisation result ≠ 0 (DI) ends the program at once with that exit code (message DS:0946/095A).</summary>
    [Fact]
    public void Initialisation_error_ends_with_its_code()
    {
        var m = Machine(NewState(), Program(new() { [1] = new Instant(_ => 2) }), WeekExitA());
        var events = RunAll(m);

        Assert.Equal([1], Started(events));
        Assert.Equal(2, m.ExitCode);
        Assert.Equal(2, Assert.Single(events, e => e.Kind == ProgramEventKind.Ended).Value);
    }

    /// <summary>
    /// Without the menu result "new game" (DS:3CCC ≠ 1, for example after loading a game) the new game is skipped, but DS:08FA is
    /// still set to 1 before the week loop. Counter-check: with DS:3CCC = 1 the new game runs.
    /// </summary>
    [Theory]
    [InlineData(0, false)]
    [InlineData(2, false)]
    [InlineData(1, true)]
    public void New_game_only_on_request_and_DS_08FA_always(byte request, bool newGame)
    {
        int menus = 0;
        var newGameStep = new Instant();
        var state = NewState();
        var m = Machine(state, Program(new()
        {
            [8] = new Instant(s => { s.Set(ProgramLoop.NewGameRequest, request); return (byte)(++menus == 1 ? 1 : 0); }),
            [9] = newGameStep,
        }), WeekExitA());
        var events = RunAll(m);

        Assert.Equal(newGame ? 1 : 0, newGameStep.Calls);
        Assert.Equal(newGame ? 0 : 1, events.Count(e => e.Kind == ProgramEventKind.StepSkipped && e.Step == ProgramLoop.NewGame));
        Assert.Equal(1, state.U8(ProgramLoop.GameRunning));
        int set = events.FindIndex(e => e.Kind == ProgramEventKind.GameRunningSet);
        int week = events.FindIndex(e => e.Kind == ProgramEventKind.StepStarted && e.Step == ProgramLoop.WeekLoopStep);
        Assert.True(set >= 0 && set < week, "DS:08FA is set right before the week loop");
    }

    /// <summary>Program start, menu and new game are pending by default: the machine stops at the start and stays there.</summary>
    [Fact]
    public void Missing_steps_are_pending()
    {
        var m = Machine(NewState(), ProgramSteps.Defaults(), WeekExitA());
        m.Advance(new PhaseTick(5));
        var events = m.DrainEvents();

        Assert.Equal(ProgramStatus.Blocked, m.Status);
        Assert.True(m.IsBlocked);
        Assert.Equal(2, m.NextStep);
        Assert.Single(events, e => e.Kind == ProgramEventKind.StepPending && e.Step == 2);
        Assert.Equal(6, events.Count(e => e.Kind == ProgramEventKind.Frame));
    }

    /// <summary>A pending week step blocks the week loop; the program machine reports it through <see cref="ProgramMachine.IsBlocked"/>.</summary>
    [Fact]
    public void Pending_week_step_blocks_inside_the_week_loop()
    {
        var week = WeekExitB();
        week.Remove(4);
        var m = Machine(NewState(), Program(), week);
        m.Advance(new PhaseTick(3));

        Assert.Equal(ProgramStatus.InWeek, m.Status);
        Assert.True(m.IsBlocked);
        Assert.Equal(PhaseStatus.Blocked, m.WeekMachine!.Status);
        Assert.Equal(4, m.WeekMachine.NextStep);
    }

    /// <summary>
    /// A step with a screen gets every frame once, in order; frames already processed change nothing; its random draws count
    /// for the frame and are stored in DS:10D2; commands reach it.
    /// </summary>
    [Fact]
    public void Waiting_step_gets_each_frame_once()
    {
        var menu = new Waiting(3);
        var state = NewState();
        var m = Machine(state, Program(new() { [8] = menu }), WeekExitA());
        m.Advance(new PhaseTick(0)); // steps 1..7 and the start of the menu
        m.Submit(new Key(0));
        m.Advance(new PhaseTick(1));
        m.Advance(new PhaseTick(1));
        m.Advance(new PhaseTick(0));
        var events = m.DrainEvents();

        Assert.Equal([1L], menu.Ticked);
        Assert.Equal([new Key(0)], menu.Received);
        Assert.Equal(ProgramStatus.Waiting, m.Status);
        Assert.Equal([0, 1], events.Where(e => e.Kind == ProgramEventKind.Frame).Select(e => e.Value));
        var reference = new OriginalRandom(new RecordedClock([]), 0); // the machine seeds from DS:10D2 (0 here)
        reference.Next(9);
        Assert.Equal(reference.Seed, state.U32(DgroupLayout.RandomSeed));
    }

    /// <summary>Commands submitted while the week loop runs go to the week step.</summary>
    [Fact]
    public void Commands_in_the_week_go_to_the_week_step()
    {
        var received = new List<IPlayerCommand>();
        var week = WeekExitA();
        week[3] = new WeekWaiting(received);
        var m = Machine(NewState(), Program(), week);
        m.Advance(new PhaseTick(0));
        Assert.Equal(ProgramStatus.InWeek, m.Status);
        m.Submit(new Key(2));
        m.Advance(new PhaseTick(1));

        Assert.Equal([new Key(2)], received);
    }

    private sealed class WeekWaiting(List<IPlayerCommand> received) : IWeekStepHandler
    {
        public StepOutcome Begin(StepContext c) => StepOutcome.Running;

        public StepOutcome Tick(StepContext c)
        {
            while (c.TakeCommand() is { } cmd) received.Add(cmd);
            return StepOutcome.Running;
        }
    }

    /// <summary>Initialisation: card -1 → VGA (1), sound -1 → the host's device; values from the command line are kept.</summary>
    [Theory]
    [InlineData(-1, -1, 0, 1, 0)]
    [InlineData(-1, -1, 1, 1, 1)]
    [InlineData(0, 0, 1, 0, 0)]
    [InlineData(2, -1, 1, 2, 1)]
    public void Initialisation_sets_card_and_sound(short card, short sound, short host, short expectedCard, short expectedSound)
    {
        var state = new Dgroup();
        state.Set(ProgramInitialisation.GraphicsCard, card);
        state.Set(ProgramInitialisation.SoundDevice, sound);
        var before = (byte[])state.Bytes.Clone();
        var m = Machine(state, new() { [1] = new ProgramInitialisation(host) }, WeekExitA());
        m.Advance(new PhaseTick(0));

        Assert.Equal(expectedCard, state.S16(ProgramInitialisation.GraphicsCard));
        Assert.Equal(expectedSound, state.S16(ProgramInitialisation.SoundDevice));
        Assert.Equal(0, m.ExitCode);
        for (int i = 0; i < before.Length; i++)
            if (i < ProgramInitialisation.GraphicsCard || i >= ProgramInitialisation.SoundDevice + 2)
                Assert.True(before[i] == state.Bytes[i], $"DS:{i:X4} changed");
    }
}
