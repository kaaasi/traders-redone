using Traders.Core;
using Traders.Core.Phases;
using Traders.Core.State;

namespace Traders.Tests.Phases;

/// <summary>
/// The phase machine without original data: step table against the statically read finding (<see cref="WeekLoopSequence"/>),
/// pending steps, frames processed exactly once, per-frame random draws, commands and both exits.
/// </summary>
public class PhaseMachineTests
{
    private sealed class Instant(byte al = 0) : IWeekStepHandler
    {
        public int Calls { get; private set; }
        public StepOutcome Begin(StepContext c) { Calls++; return StepOutcome.Finished(al); }
        public StepOutcome Tick(StepContext c) => throw new InvalidOperationException("instant step ticked");
    }

    /// <summary>Waits <paramref name="frames"/> frames and draws random(9) once per frame, like a wait loop of the original.</summary>
    private sealed class Waiting(int frames, bool draw = true) : IWeekStepHandler
    {
        public List<long> Ticked { get; } = [];
        public List<IPlayerCommand> Received { get; } = [];
        public StepOutcome Begin(StepContext c) => StepOutcome.Running;

        public StepOutcome Tick(StepContext c)
        {
            Ticked.Add(c.Frame);
            if (draw) c.Random.Next(9);
            while (c.TakeCommand() is { } cmd) Received.Add(cmd);
            return Ticked.Count >= frames ? StepOutcome.Finished() : StepOutcome.Running;
        }
    }

    private sealed record Plant(int Player, int Field) : IPlayerCommand;

    private static Dictionary<int, IWeekStepHandler> AllInstant(Dictionary<int, IWeekStepHandler>? overrides = null)
    {
        var d = WeekLoop.Steps.Where(s => s.Kind == WeekStepKind.Call).ToDictionary(s => s.Number, _ => (IWeekStepHandler)new Instant());
        foreach (var (k, v) in overrides ?? []) d[k] = v;
        return d;
    }

    private static PhaseMachine Machine(Dgroup state, Dictionary<int, IWeekStepHandler> handlers, uint seed = 1) =>
        new(state, new OriginalRandom(new RecordedClock([]), seed), handlers);

    [Fact]
    public void Step_table_equals_the_finding()
    {
        Assert.Equal(WeekLoopSequence.Calls.Length, WeekLoop.Steps.Count);
        for (int i = 0; i < WeekLoop.Steps.Count; i++)
        {
            var s = WeekLoop.Steps[i];
            var c = WeekLoopSequence.Calls[i];
            Assert.Equal(i + 1, s.Number);
            Assert.Equal((c.Site, c.TargetSegment, c.TargetOffset, c.Marker), (s.Site, s.TargetSegment, s.TargetOffset, s.Marker));
            Assert.Equal(c.Marker is not null, s.Kind == WeekStepKind.Marker);
        }
        Assert.Equal(WeekLoopSequence.Segment, WeekLoop.Segment);
        Assert.Equal(WeekLoopSequence.Entry, WeekLoop.Entry);
        Assert.Equal(WeekLoopSequence.CounterIncrement, WeekLoop.CounterIncrementSite);
        // BESTAND.md §1.4: steps 1..23 on the calls, markers without a step number; exits after the balance and the end check
        Assert.Equal(Enumerable.Range(1, 23), WeekLoop.Steps.Where(s => s.Kind == WeekStepKind.Call).Select(s => s.PlanStep!.Value));
        Assert.All(WeekLoop.Steps.Where(s => s.Kind == WeekStepKind.Marker), s => Assert.Null(s.PlanStep));
        Assert.Equal([3, 19], WeekLoop.Steps.Where(s => s.ExitAfter != WeekExitCheck.None).Select(s => s.Number));
        Assert.Equal((0x0825, 0x000A, WeekExitCheck.AbortToMenu), (WeekLoop.Step(3).TargetSegment, WeekLoop.Step(3).TargetOffset, WeekLoop.Step(3).ExitAfter));
        Assert.Equal((0x20B3, 0x0005, WeekExitCheck.GameOver), (WeekLoop.Step(19).TargetSegment, WeekLoop.Step(19).TargetOffset, WeekLoop.Step(19).ExitAfter));
        // arguments pushed by the loop: balance 1, computer players 0 and 1
        Assert.Equal([(3, 1), (6, 0), (21, 1)], WeekLoop.Steps.Where(s => s.Kind == WeekStepKind.Call && s.Argument is not null)
            .Select(s => (s.Number, s.Argument!.Value)));
    }

    [Fact]
    public void A_step_without_handler_blocks_the_machine_instead_of_being_skipped()
    {
        var state = new Dgroup();
        var handlers = AllInstant();
        handlers.Remove(4);
        var m = Machine(state, handlers);
        m.Advance(new PhaseTick(0));
        Assert.Equal(PhaseStatus.Blocked, m.Status);
        Assert.Equal(4, m.NextStep);
        var pending = Assert.Single(m.Events, e => e.Kind == PhaseEventKind.StepPending);
        Assert.Equal(4, pending.Step);
        Assert.DoesNotContain(m.Events, e => e.Kind == PhaseEventKind.StepStarted && e.Step > 3);
        m.DrainEvents();
        m.Advance(new PhaseTick(5));
        Assert.Equal(PhaseStatus.Blocked, m.Status);
        Assert.Equal(4, m.NextStep);
        Assert.All(m.Events, e => Assert.Equal(PhaseEventKind.Frame, e.Kind));
        Assert.Equal(5, m.Events.Count);
    }

    [Fact]
    public void Without_any_handler_the_first_step_is_pending()
    {
        var m = Machine(new Dgroup(), []);
        m.Advance(new PhaseTick(0));
        Assert.Equal(PhaseStatus.Blocked, m.Status);
        Assert.Equal(1, m.NextStep);
    }

    [Fact]
    public void A_week_of_instant_steps_runs_in_order_and_increments_the_counter()
    {
        var state = new Dgroup();
        var m = Machine(state, AllInstant());
        m.Advance(new PhaseTick(0));
        var started = m.Events.Where(e => e.Kind is PhaseEventKind.StepStarted or PhaseEventKind.MarkerPassed).Select(e => e.Step);
        Assert.Equal(Enumerable.Range(1, 30), started);
        Assert.Equal([0, 1, 2, 3, 4, 5, 6], m.Events.Where(e => e.Kind == PhaseEventKind.MarkerPassed).Select(e => e.Value));
        Assert.Equal(1, state.S16(DgroupLayout.Week));
        Assert.Equal(PhaseStatus.Ready, m.Status);
        Assert.Equal(1, m.NextStep);
        m.Advance(new PhaseTick(1));
        Assert.Equal(2, state.S16(DgroupLayout.Week));
    }

    [Fact]
    public void Week_counter_wraps_at_16_bit()
    {
        var state = new Dgroup();
        state.Set(DgroupLayout.Week, (short)0x7FFF);
        var m = Machine(state, AllInstant());
        m.Advance(new PhaseTick(0));
        Assert.Equal(short.MinValue, state.S16(DgroupLayout.Week));
    }

    [Fact]
    public void Frames_are_processed_exactly_once_and_carry_the_random_draws_of_the_running_step()
    {
        var state = new Dgroup();
        state.Set(DgroupLayout.RandomSeed, 0x12345678u);
        var wait = new Waiting(frames: 4);
        var m = Machine(state, AllInstant(new() { [8] = wait }));
        m.Advance(new PhaseTick(0));            // steps 1..7 instant, step 8 starts and waits
        Assert.Equal(PhaseStatus.Waiting, m.Status);
        Assert.Equal(8, m.NextStep);
        m.Advance(new PhaseTick(2));            // frames 1 and 2 (catch-up inside the core)
        m.Advance(new PhaseTick(2));            // same frame again: nothing
        m.Advance(new PhaseTick(1));            // older frame: nothing
        Assert.Equal([1L, 2L], wait.Ticked);
        m.Advance(new PhaseTick(4));
        Assert.Equal([1L, 2L, 3L, 4L], wait.Ticked);
        Assert.Equal(1, m.Week); // step 8 returned in frame 4, the rest of week 0 ran and step 8 of week 1 waits again
        Assert.Equal(PhaseStatus.Waiting, m.Status);
        var frames = m.Events.Where(e => e.Kind == PhaseEventKind.Frame).ToList();
        Assert.Equal([0L, 1L, 2L, 3L, 4L], frames.Select(e => e.Frame));
        Assert.Equal([0, 1, 1, 1, 1], frames.Select(e => e.Value));
        // the draws went through DS:10D2: four steps of the original generator from the seed in the data segment
        var r = new OriginalRandom(new RecordedClock([]), 0x12345678u);
        for (int i = 0; i < 4; i++) r.Next(9);
        Assert.Equal(r.Seed, state.U32(DgroupLayout.RandomSeed));
    }

    [Fact]
    public void Commands_reach_the_running_step_with_the_next_frame()
    {
        var wait = new Waiting(frames: 3, draw: false);
        var m = Machine(new Dgroup(), AllInstant(new() { [8] = wait }));
        m.Advance(new PhaseTick(0));
        var a = new Plant(0, 12);
        var b = new Plant(2, 7);
        m.Submit(a);
        m.Submit(b);
        Assert.Empty(wait.Received);
        m.Advance(new PhaseTick(1));
        Assert.Equal([a, b], wait.Received);
    }

    [Fact]
    public void Exit_A_after_the_balance_when_backspace_is_held()
    {
        var state = new Dgroup();
        var handlers = AllInstant();
        var m = Machine(state, handlers);
        m.Advance(new PhaseTick(0, [0x0E]));
        Assert.Equal(PhaseStatus.Finished, m.Status);
        Assert.Equal(WeekLoop.ResultMainMenu, m.Result);
        Assert.Equal(1, state.U8(WeekLoop.AbortKeyState));
        Assert.Equal([1, 2, 3], m.Events.Where(e => e.Kind == PhaseEventKind.StepStarted).Select(e => e.Step));
        Assert.Equal(0, ((Instant)handlers[4]).Calls);
        Assert.Equal(0, state.S16(DgroupLayout.Week));
        m.Advance(new PhaseTick(3));
        Assert.Equal(3, m.Events.Count(e => e.Kind == PhaseEventKind.StepStarted));
    }

    [Fact]
    public void Other_keys_do_not_exit_and_released_backspace_clears_the_key_state()
    {
        var state = new Dgroup();
        var wait = new Waiting(frames: 1, draw: false);
        var m = Machine(state, AllInstant(new() { [3] = wait }));
        m.Advance(new PhaseTick(0, [0x0E]));      // balance shown, Backspace held
        m.Advance(new PhaseTick(1, [0x1C]));      // released before the balance returns, Enter held instead
        Assert.NotEqual(PhaseStatus.Finished, m.Status);
        Assert.Equal(0, state.U8(WeekLoop.AbortKeyState));
        Assert.Equal(1, state.U8(DgroupLayout.KeyState + 0x1C));
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(0, false)]
    [InlineData(2, false)] // cmp al,1 / jnz: only exactly 1 ends the game
    public void Exit_B_only_when_the_end_check_returns_1(byte al, bool exits)
    {
        var state = new Dgroup();
        var m = Machine(state, AllInstant(new() { [19] = new Instant(al) }));
        m.Advance(new PhaseTick(0));
        var markers = m.Events.Where(e => e.Kind == PhaseEventKind.MarkerPassed).Select(e => e.Value).ToList();
        if (exits)
        {
            Assert.Equal(PhaseStatus.Finished, m.Status);
            Assert.Equal(WeekLoop.ResultGameOver, m.Result);
            Assert.Equal([0, 1, 2, 3], markers);
            Assert.Equal(19, m.Events.Where(e => e.Kind == PhaseEventKind.StepStarted).Max(e => e.Step));
            Assert.Equal(0, state.S16(DgroupLayout.Week));
        }
        else
        {
            Assert.Equal(PhaseStatus.Ready, m.Status);
            Assert.Equal([0, 1, 2, 3, 4, 5, 6], markers);
            Assert.Equal(1, state.S16(DgroupLayout.Week));
        }
    }

    [Fact]
    public void A_handler_that_replaces_the_data_segment_keeps_its_random_state()
    {
        var state = new Dgroup();
        state.Set(DgroupLayout.RandomSeed, 5u);
        var replace = new Replace(state);
        var m = Machine(state, AllInstant(new() { [1] = replace }));
        m.Advance(new PhaseTick(0));
        Assert.Equal(0xCAFEBABEu, state.U32(DgroupLayout.RandomSeed));
    }

    private sealed class Replace(Dgroup state) : IWeekStepHandler
    {
        public StepOutcome Begin(StepContext c)
        {
            state.Set(DgroupLayout.RandomSeed, 0xCAFEBABEu);
            return StepOutcome.Finished();
        }

        public StepOutcome Tick(StepContext c) => StepOutcome.Finished();
    }
}
