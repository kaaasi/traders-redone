using Traders.Core;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.State;

namespace Traders.Tests.Menus;

/// <summary>Frame handling of the menu runner and the phase-machine adapter (no original data needed).</summary>
public sealed class MenuRunnerTests
{
    private static Dgroup State()
    {
        var d = new Dgroup();
        for (int p = 0; p < 4; p++) d.Set(DgroupLayout.IsComputer + p, (byte)1);
        d.Set(DgroupLayout.Level, (short)1);
        d.Set(DgroupLayout.FieldColumns, (short)8);
        d.Set(DgroupLayout.FieldRows, (short)6);
        d.Set(DgroupLayout.RandomSeed, 0x1234u);
        return d;
    }

    private static IReadOnlyCollection<byte> Keys(long f) => f is >= 100 and < 103 ? [MenuKeys.Enter] : [];

    [Fact]
    public void Repeated_frame_numbers_change_nothing()
    {
        var d = State();
        var runner = new MenuRunner(d, new OriginalRandom(new RecordedClock([])), new ScriptedMenuHost());
        runner.Start();
        for (long f = 1; f <= 50; f++)
        {
            runner.Advance(new PhaseTick(f, Keys(f)));
            var before = (byte[])d.Bytes.Clone();
            int events = runner.Menu.Events.Count;
            runner.Advance(new PhaseTick(f, Keys(f)));
            runner.Advance(new PhaseTick(f - 1, Keys(f)));
            Assert.Equal(before, d.Bytes);
            Assert.Equal(events, runner.Menu.Events.Count);
        }
    }

    [Fact]
    public void Phase_machine_step_and_runner_give_the_same_state()
    {
        var a = State();
        var runner = new MenuRunner(a, new OriginalRandom(new RecordedClock([])), new ScriptedMenuHost());
        runner.Start(0, Keys(0));
        for (long f = 1; !runner.Finished; f++) runner.Advance(new PhaseTick(f, Keys(f)));

        // the same menu as a handler of a phase machine (here in the slot of call 1, which is enough to drive it): Begin in
        // frame 0, one Tick per frame; the machine writes the key table and keeps DS:10D2 in sync
        var b = State();
        var step = new MainMenuStep(new ScriptedMenuHost());
        var machine = new PhaseMachine(b, new OriginalRandom(new RecordedClock([])),
            new Dictionary<int, IWeekStepHandler> { [1] = step });
        long frame = 0;
        for (; machine.Status != PhaseStatus.Blocked; frame++) machine.Advance(new PhaseTick(frame, Keys(frame)));
        var finished = machine.Events.Single(e => e.Kind == PhaseEventKind.StepFinished);

        Assert.Equal(runner.Menu.Result, (byte)finished.Value);
        Assert.Equal(runner.LastFrame, finished.Frame);
        b.Set(DgroupLayout.KeyState + MenuKeys.Enter, a.U8(DgroupLayout.KeyState + MenuKeys.Enter));
        Assert.Equal(a.Bytes, b.Bytes);
        Assert.Equal(1, finished.Value);
        Assert.Equal(1, b.U8(MenuLayout.Choice));
    }
}
