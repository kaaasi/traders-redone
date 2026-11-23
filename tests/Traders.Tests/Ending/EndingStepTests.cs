using Traders.Core;
using Traders.Core.Ending;
using Traders.Core.Phases;
using Traders.Core.Pixels;
using Traders.Core.Startup;
using Traders.Core.State;

namespace Traders.Tests.Ending;

/// <summary>
/// The whole end check 20B3:0005 as a step (<see cref="EndingStep"/>, without original data): in a zeroed data segment the
/// wealth is the cash. Where the game goes on the step returns AL = 0 at once without a change beyond the decision; where it
/// ends it runs the end screens frame by frame and finishes with AL = 1, week counter + 1, the key wait passed. The pixel reads
/// come from a script (the zeroed rule page holds no picture), the report is recorded.
/// </summary>
public class EndingStepTests
{
    private static Dgroup State(int[] cash, uint target)
    {
        var d = new Dgroup();
        for (int i = 0; i < 4; i++) d.Set(DgroupLayout.Cash + 4 * i, cash[i]);
        d.Set(DgroupLayout.Target, target);
        d.Set(DgroupLayout.Week, (short)3);
        d.Set(0x090E, (short)1);
        d.Set(RulePage.PageNumbers, 1u);
        d.Set(RulePage.PageNumbers + 4, 2u);
        d.Set(EndScreens.Bow1, (short)15);
        d.Set(EndScreens.Bow2, (short)15);
        return d;
    }

    private static (EndingStep Step, EndScreensRecorder Host) Step(Dgroup d)
    {
        int n = 0;
        var host = new EndScreensRecorder(_ => (byte)(n++ % 3 == 0 ? 2 : 9)) { State = d };
        return (new EndingStep(host, _ => default, _ => new IndexedImage(320, 256, new byte[320 * 256]), []), host);
    }

    private static PhaseMachine Machine(Dgroup d, IWeekStepHandler step, OriginalRandom random) =>
        new(d, random, new Dictionary<int, IWeekStepHandler> { [EndCheckStep.Call] = step }, nextStep: EndCheckStep.Call);

    [Fact]
    public void Where_the_game_goes_on_the_step_returns_0_at_once()
    {
        var d = State([5, 6, 7, 8], 10_000);
        var (step, host) = Step(d);
        var machine = Machine(d, step, new OriginalRandom(new RecordedClock([])));
        machine.Advance(new PhaseTick(0));
        Assert.Contains(machine.Events, e => e is { Kind: PhaseEventKind.StepFinished, Step: EndCheckStep.Call, Value: 0 });
        Assert.Empty(host.Calls);
        Assert.Equal(3, d.S16(DgroupLayout.Week));
    }

    [Fact]
    public void Where_the_game_ends_the_step_runs_the_end_screens_and_returns_1()
    {
        var d = State([5, 10_000, 7, 8], 10_000);
        var (step, host) = Step(d);
        var random = new OriginalRandom(new RecordedClock([]));
        var machine = Machine(d, step, random);
        long frames = 0;
        while (!machine.Events.Any(e => e is { Kind: PhaseEventKind.StepFinished, Step: EndCheckStep.Call }) && frames < 1_000_000)
            machine.Advance(new PhaseTick(frames++));
        Assert.Contains(machine.Events, e => e is { Kind: PhaseEventKind.StepFinished, Step: EndCheckStep.Call, Value: 1 });
        Assert.InRange(frames, 2_000, 999_999);
        Assert.Equal(4, d.S16(DgroupLayout.Week));
        Assert.Equal(1, d.U8(EndScreens.MakeCount));
        Assert.Equal(7, d.S16(EndScreens.RankState));
        Assert.Equal(["Music(2, 3)"], host.Calls.Where(c => c.StartsWith("Music")));
        Assert.Equal("WaitKey()", host.Calls[^1]);
        Assert.Contains("Report(0)", host.Calls);
        // 2340 cells accepted in five reveals, every third read rejected
        Assert.Equal(5 * EndScreens.RevealCells, host.Calls.Count(c => c.StartsWith("Pixel(") && c.EndsWith(", 2)")));
        Assert.True(random.Calls > 2 * 5 * EndScreens.RevealCells);
    }
}
