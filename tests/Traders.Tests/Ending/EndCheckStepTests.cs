using Traders.Core;
using Traders.Core.Ending;
using Traders.Core.Phases;
using Traders.Core.State;

namespace Traders.Tests.Ending;

/// <summary>
/// The end check as a step of the phase machine (without original data): in a zeroed data segment the wealth is the cash
/// DS:3CD2, so the decision is set by cash, target and key alone. Where the game ends the call is pending (the machine stops
/// at call 19 without changing a byte) or goes to the rest from the unchanged state (R7P-2).
/// </summary>
public class EndCheckStepTests
{
    private static Dgroup State(int[] cash, uint target, bool key = false)
    {
        var d = new Dgroup();
        for (int i = 0; i < 4; i++) d.Set(DgroupLayout.Cash + 4 * i, cash[i]);
        d.Set(DgroupLayout.Target, target);
        d.Set(DgroupLayout.KeyState + EndCheck.EndKey, key ? (byte)1 : (byte)0);
        d.Set(DgroupLayout.Week, (short)7);
        return d;
    }

    private static PhaseMachine Machine(Dgroup d, IWeekStepHandler step) =>
        new(d, new OriginalRandom(new RecordedClock([])), new Dictionary<int, IWeekStepHandler> { [EndCheckStep.Call] = step },
            nextStep: EndCheckStep.Call);

    [Theory]
    [InlineData(new[] { 9_999, 0, 0, 0 }, 10_000u, false, false, 4)]
    [InlineData(new[] { 9_999, 0, 0, 0 }, 10_000u, true, true, 4)]
    [InlineData(new[] { 9_999, 10_000, 0, 0 }, 10_000u, false, true, 1)]
    [InlineData(new[] { 0, 0, 0, -1 }, 10_000u, false, true, 3)]       // negative wealth counts as huge (unsigned)
    [InlineData(new[] { 0xFFFF, 0, 0, 0 }, 0x0001_0000u, false, false, 4)] // high word first
    [InlineData(new[] { 0x1_0000, 0, 0, 0 }, 0x0001_FFFFu, false, false, 4)]
    public void Decision_and_DS_37AA(int[] cash, uint target, bool key, bool ends, int activePlayer)
    {
        var d = State(cash, target, key);
        Assert.Equal(ends, EndCheck.Ends(d));
        Assert.Equal(ends, EndCheck.Decide(d));
        Assert.Equal(activePlayer, d.S8(DgroupLayout.ActivePlayer));
        for (int i = 0; i < 4; i++) Assert.Equal(cash[i], d.S32(DgroupLayout.CurrentWealth + 4 * i));
    }

    /// <summary>The game goes on: the step returns AL = 0 at once and the machine moves on to call 20, without a random draw.</summary>
    [Fact]
    public void Where_the_game_goes_on_the_step_returns_0_at_once()
    {
        var d = State([5, 6, 7, 8], 10_000);
        var step = new EndCheckStep();
        var machine = Machine(d, step);
        machine.Advance(new PhaseTick(0));
        // call 20 has no handler here: the machine waits there
        Assert.Equal(PhaseStatus.Blocked, machine.Status);
        Assert.Equal(EndCheckStep.Call + 1, machine.NextStep);
        Assert.Contains(machine.Events, e => e is { Kind: PhaseEventKind.StepFinished, Step: EndCheckStep.Call, Value: 0 });
        Assert.All(machine.Events.Where(e => e.Kind == PhaseEventKind.Frame), e => Assert.Equal(0, e.Value));
        Assert.Equal(4, d.S8(DgroupLayout.ActivePlayer));
        Assert.Equal(8, d.S32(DgroupLayout.CurrentWealth + 12));
        Assert.False(step.GameOver);
    }

    /// <summary>The game ends without a rest: the machine stops at call 19 ("game over") and the data segment is unchanged.</summary>
    [Fact]
    public void Where_the_game_ends_the_machine_stops_at_call_19_without_a_change()
    {
        var d = State([5, 10_000, 7, 8], 10_000);
        var before = (byte[])d.Bytes.Clone();
        var machine = Machine(d, new EndCheckStep());
        machine.Advance(new PhaseTick(3));
        Assert.Equal(PhaseStatus.Blocked, machine.Status);
        Assert.Equal(EndCheckStep.Call, machine.NextStep);
        Assert.Contains(machine.Events, e => e is { Kind: PhaseEventKind.StepPending, Step: EndCheckStep.Call });
        Assert.DoesNotContain(machine.Events, e => e.Kind == PhaseEventKind.StepStarted);
        Assert.Equal(before, d.Bytes);
    }

    /// <summary>With a rest the whole call goes to it, from the state before the decision; its AL = 1 ends the loop (exit B).</summary>
    [Fact]
    public void Where_the_game_ends_the_rest_runs_from_the_unchanged_state()
    {
        var d = State([5, 6, 7, 8], 10_000, key: true);
        var before = (byte[])d.Bytes.Clone();
        var rest = new Rest();
        var step = new EndCheckStep(rest);
        Assert.False(step.IsPending(d));
        var machine = Machine(d, step);
        machine.Advance(new PhaseTick(0));
        Assert.Equal(before, rest.Seen);
        Assert.True(step.GameOver);
        Assert.Equal(PhaseStatus.Finished, machine.Status);
        Assert.Equal(WeekLoop.ResultGameOver, machine.Result);
    }

    private sealed class Rest : IWeekStepHandler
    {
        public byte[]? Seen { get; private set; }

        public StepOutcome Begin(StepContext context)
        {
            Seen = (byte[])context.State.Bytes.Clone();
            return StepOutcome.Finished(1);
        }

        public StepOutcome Tick(StepContext context) => throw new InvalidOperationException();
    }
}
