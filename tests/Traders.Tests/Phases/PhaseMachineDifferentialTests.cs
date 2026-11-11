using Traders.Core;
using Traders.Core.Phases;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.RefEmu.Hardware;

namespace Traders.Tests.Phases;

/// <summary>
/// [DIFFERENTIAL] 1777:01F8 – the core's phase machine against the original week loop in the RefEmu, in lockstep
/// (<see cref="RefEmuWeekOracle"/>): every call step is carried out by the RefEmu and its data segment and AL handed to the
/// core; the loop itself – order of the 30 calls, markers, exit checks, week counter DS:4B9A – runs in the core. Checked at
/// every call site: the RefEmu arrives at the call the core starts, with the same data segment (outside the bytes owned by
/// interrupt handlers). Expected values come only from the RefEmu.
/// </summary>
public class PhaseMachineDifferentialTests
{
    private static string Dir => TestGameData.DosDir;

    private sealed record Lockstep(PhaseMachine Machine, Dgroup Core, RefEmuWeekOracle Oracle, OriginalProgram Program, List<PhaseEvent> Events);

    private static Lockstep Start(Action<OriginalProgram>? beforeWeek = null)
    {
        var p = OriginalProgram.StartGame(Dir);
        Assert.True(p.RunToWeekStart(0, 5), "week 0 did not start within 5 s after the menu");
        beforeWeek?.Invoke(p);
        // The RefEmu is now at 1777:02DC (entry of call 1); the tests run it to the return of call 1 and start the core at call 2.
        var core = new Dgroup(p.DataSegment().AsSpan(0, DgroupLayout.Size).ToArray());
        var oracle = new RefEmuWeekOracle(p);
        var handlers = WeekLoop.Steps.Where(s => s.Kind == WeekStepKind.Call).ToDictionary(s => s.Number, _ => oracle.Handler(core));
        var machine = new PhaseMachine(core, new OriginalRandom(new RecordedClock([])), handlers, nextStep: 2);
        return new Lockstep(machine, core, oracle, p, []);
    }

    /// <summary>Advances the core frame by frame until <paramref name="until"/> holds (all oracle steps finish at once).</summary>
    private static void Run(Lockstep l, Func<Lockstep, bool> until, int maxFrames = 10)
    {
        for (int f = 0; f < maxFrames && !until(l); f++)
        {
            l.Machine.Advance(new PhaseTick(l.Machine.LastFrame + 1));
            l.Events.AddRange(l.Machine.DrainEvents());
        }
    }

    /// <summary>Loop call numbers the RefEmu entered (call sites only, in order).</summary>
    private static List<int> EmuCalls(RefEmuWeekOracle o) =>
        o.Passed.Select(site => WeekLoop.Steps.FirstOrDefault(s => s.Site == site)?.Number).OfType<int>().ToList();

    /// <summary>Call numbers the core started or passed (markers), in order.</summary>
    private static List<int> CoreCalls(IEnumerable<PhaseEvent> events) =>
        events.Where(e => e.Kind is PhaseEventKind.StepStarted or PhaseEventKind.MarkerPassed).Select(e => e.Step).ToList();

    /// <summary>
    /// Week 0 of the default game (four computer players) from call 2 to call 1 of week 1: both take the same 30 calls with the
    /// markers at the same places, the core increments the counter to 1 exactly where the original does (the data segment at
    /// call 1 of week 1 is equal), and no frame consumes random numbers in the core (all draws are inside the RefEmu steps).
    /// </summary>
    [GameDataFact]
    public void Week_0_in_lockstep_with_the_original()
    {
        var l = Start();
        // the RefEmu stopped at the entry of call 1 (1777:02DC); the core starts with call 2 after the return of call 1
        Assert.True(l.Oracle.RunToNextStop() && l.Oracle.IsAtLoop((ushort)(WeekLoop.Step(1).Site + 5)), "return of call 1");
        l.Program.DataSegment().AsSpan(0, DgroupLayout.Size).CopyTo(l.Core.Bytes);
        l.Oracle.ResetPassed();

        Run(l, x => x.Machine.Week == 1 && x.Machine.NextStep == 2);

        Assert.Empty(l.Oracle.Differences);
        Assert.Equal(PhaseStatus.Ready, l.Machine.Status);
        var expected = Enumerable.Range(2, 29).Append(1).ToList(); // calls 2..30 of week 0, call 1 of week 1
        Assert.Equal(expected, CoreCalls(l.Events));
        // the RefEmu stops at the return of call 1, which is the site of call 2 of week 1
        Assert.Equal(expected.Append(2), EmuCalls(l.Oracle));
        Assert.Equal([0, 1, 2, 3, 4, 5, 6], l.Events.Where(e => e.Kind == PhaseEventKind.MarkerPassed).Select(e => e.Value));
        var advanced = Assert.Single(l.Events, e => e.Kind == PhaseEventKind.WeekAdvanced);
        Assert.Equal(1, advanced.Value);
        Assert.Equal(1, l.Program.Week);
        Assert.All(l.Events.Where(e => e.Kind == PhaseEventKind.Frame), e => Assert.Equal(0, e.Value));
    }

    /// <summary>
    /// Exit A: Backspace (scancode 0Eh) held during the weekly balance of week 0. The core sees DS:3D55 in the data segment of
    /// the RefEmu after call 3 and exits with AL = 0; the original returns into main (1777:0184) without call 4.
    /// </summary>
    [GameDataFact]
    public void Exit_A_in_lockstep_with_the_original()
    {
        var l = Start(p => p.Press(Scancodes.Backspace, delay: 0.5, hold: 30));
        Assert.True(l.Oracle.RunToNextStop() && l.Oracle.IsAtLoop((ushort)(WeekLoop.Step(1).Site + 5)), "return of call 1");
        l.Program.DataSegment().AsSpan(0, DgroupLayout.Size).CopyTo(l.Core.Bytes);
        l.Oracle.ResetPassed();

        Run(l, x => x.Machine.Status == PhaseStatus.Finished);

        Assert.Empty(l.Oracle.Differences);
        Assert.Equal(PhaseStatus.Finished, l.Machine.Status);
        Assert.Equal(WeekLoop.ResultMainMenu, l.Machine.Result);
        Assert.NotEqual(0, l.Core.U8(WeekLoop.AbortKeyState));
        Assert.Equal([2, 3], CoreCalls(l.Events));
        // the original: after the return of call 3 straight back into main, AL = 0
        Assert.True(l.Oracle.RunToNextStop(), "RefEmu did not leave the loop");
        Assert.Equal(WeekLoopSequence.ReturnInMain, l.Oracle.Passed[^1]);
        Assert.Equal(0, l.Program.Machine.Cpu.Ax & 0xFF);
        Assert.Equal([2, 3], EmuCalls(l.Oracle));
        Assert.Equal(0, l.Machine.Week);
    }
}
