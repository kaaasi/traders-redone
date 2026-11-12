using Traders.Core;
using Traders.Core.Phases;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.RefEmu.Hardware;

namespace Traders.Tests.Phases;

/// <summary>
/// [DIFFERENTIAL] 1777:000C – the core's program machine against main in the RefEmu, in lockstep
/// (<see cref="RefEmuProgramOracle"/>, week loop via <see cref="RefEmuWeekOracle"/>): the program flow – order of the calls,
/// result checks, guard DS:3CCC, DS:08FA = 1, loop back after exit A – runs in the core; program start, main menu and new game
/// are carried out by the RefEmu; the initialisation (game part) and the sound stubs are the core's and are compared after
/// their return outside the bytes the host owns. Expected values come only from the RefEmu.
/// </summary>
public class ProgramMachineDifferentialTests
{
    private static string Dir => TestGameData.DosDir;

    /// <summary>Delay of RETURN after the main menu is entered (the menu is drawn by then; as the 18 s of the reference run).</summary>
    private const double MenuDelay = 3.0;

    private sealed record Lockstep(ProgramMachine Machine, Dgroup Core, RefEmuProgramOracle Oracle, RefEmuWeekOracle WeekOracle,
        OriginalProgram Program, List<ProgramEvent> Events);

    /// <param name="programOverrides">Handlers that replace the default of a program step (built with the oracles).</param>
    /// <param name="weekAtCallSite">Actions at call sites of the week loop (RefEmu), by call number.</param>
    private static Lockstep Start(Func<Lockstep, Dictionary<int, IProgramStepHandler>>? programOverrides = null,
        IReadOnlyDictionary<int, Action<OriginalProgram>>? weekAtCallSite = null,
        IReadOnlyDictionary<int, Action<OriginalProgram>>? programAtCallSite = null, int? weekStopAt = null)
    {
        var p = OriginalProgram.Start(Dir);
        var oracle = new RefEmuProgramOracle(p);
        // the C runtime startup is the host's: the core starts with the data segment at the first call of main
        oracle.RunToCallSite(ProgramLoop.Step(1));
        var core = new Dgroup(p.DataSegment().AsSpan(0, DgroupLayout.Size).ToArray());
        var weekOracle = new RefEmuWeekOracle(p);
        var weekHandlers = WeekLoop.Steps.Where(s => s.Kind == WeekStepKind.Call)
            .ToDictionary(s => s.Number, s => weekOracle.Handler(core, weekAtCallSite?.GetValueOrDefault(s.Number)));
        // oracle steps finish at once, so one frame would run whole weeks; a test that ends in the week stops at weekStopAt
        if (weekStopAt is int stop) weekHandlers[stop] = new WeekStop();
        weekHandlers[1] = oracle.EnteringWeekLoop(core, weekHandlers[1]);
        var handlers = new Dictionary<int, IProgramStepHandler>();
        var defaults = ProgramSteps.Defaults(ProgramInitialisation.AdLib);
        handlers[1] = oracle.PortHandler(core, defaults[1], RefEmuProgramOracle.HostOwnedByInitialisation);
        foreach (int n in ProgramSteps.SoundSteps) handlers[n] = oracle.PortHandler(core, defaults[n], RefEmuProgramOracle.SoundOwned);
        foreach (int n in new[] { 2, 8, 9 }) handlers[n] = oracle.Handler(core, programAtCallSite?.GetValueOrDefault(n));
        var machine = new ProgramMachine(core, new OriginalRandom(new RecordedClock([])), handlers, weekHandlers);
        var l = new Lockstep(machine, core, oracle, weekOracle, p, []);
        foreach (var (n, h) in programOverrides?.Invoke(l) ?? []) handlers[n] = h;
        return l;
    }

    private static void Run(Lockstep l, Func<Lockstep, bool> until, int maxFrames = 20)
    {
        for (int f = 0; f < maxFrames && !until(l); f++)
        {
            l.Machine.Advance(new PhaseTick(l.Machine.LastFrame + 1));
            l.Events.AddRange(l.Machine.DrainEvents());
        }
    }

    private static readonly Dictionary<int, Action<OriginalProgram>> EnterInMenu = new()
    {
        [ProgramLoop.MainMenu] = p => p.Press(Scancodes.Enter, delay: MenuDelay),
    };

    /// <summary>Program step numbers the core started, in order.</summary>
    private static List<int> CoreSteps(IEnumerable<ProgramEvent> events) =>
        events.Where(e => e.Kind == ProgramEventKind.StepStarted).Select(e => e.Step).ToList();

    /// <summary>Program steps whose call site the RefEmu reached, in order.</summary>
    private static List<int> EmuSteps(RefEmuProgramOracle o) =>
        o.Passed.Select(site => ProgramLoop.Steps.FirstOrDefault(s => s.Site == site)?.Number).OfType<int>().ToList();

    /// <summary>
    /// From the first call of main to the entry of the weekly prices 1777:02DC of week 0 (call 1 of the week loop): both take
    /// the same calls (initialisation, start, music query, sound source, music query, effect, music, menu, new game, week loop),
    /// the data segment is equal at every call site (DS:10D2 included), DS:08FA is 1 at the week loop, and the core's
    /// initialisation and sound stubs leave nothing different outside the host-owned bytes.
    /// </summary>
    // [DIFFERENTIAL] 1777:04E4 – initialisation (game part) compared after its return outside the host-owned bytes
    [GameDataFact]
    public void Program_start_to_week_0_in_lockstep_with_the_original()
    {
        var l = Start(programAtCallSite: EnterInMenu, weekStopAt: 2);

        Run(l, x => x.Machine.WeekMachine is { NextStep: 2 });

        Assert.Empty(l.Oracle.Differences);
        Assert.Empty(l.WeekOracle.Differences);
        Assert.Equal(ProgramStatus.InWeek, l.Machine.Status);
        int[] expected = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        Assert.Equal(expected, CoreSteps(l.Events));
        Assert.Equal(expected, EmuSteps(l.Oracle));
        Assert.Contains(l.Events, e => e.Kind == ProgramEventKind.GameRunningSet);
        Assert.Equal(1, l.Core.U8(ProgramLoop.GameRunning));
        Assert.Equal(1, l.Core.U8(ProgramLoop.NewGameRequest));
        // the week oracle compared at call 1 (1777:01FB, entry of 1777:02DC) and the RefEmu returned from it
        Assert.Equal(WeekLoop.Step(1).Site + 5, l.WeekOracle.Passed[^1]);
        Assert.Equal(0, l.Program.Week);
        Assert.Equal(l.Program.RandomState, l.Core.U32(DgroupLayout.RandomSeed));
        Assert.All(l.Events.Where(e => e.Kind == ProgramEventKind.Frame), e => Assert.Equal(0, e.Value));
    }

    /// <summary>
    /// Exit A of week 0 (Backspace held during the weekly balance): the core goes back to the loop start 1777:013B – music query,
    /// effect, music – and reaches the main menu again with the same data segment as the original; no second initialisation
    /// and no second program start.
    /// </summary>
    [GameDataFact]
    public void Exit_A_returns_to_the_main_menu_in_lockstep_with_the_original()
    {
        int menus = 0;
        var l = Start(
            programOverrides: x => new() { [ProgramLoop.MainMenu] = new SecondEntryStops(x.Oracle.Handler(x.Core, EnterInMenu[ProgramLoop.MainMenu]), x.Oracle.StopAtCallSite(x.Core), () => ++menus) },
            programAtCallSite: null,
            weekAtCallSite: new Dictionary<int, Action<OriginalProgram>> { [2] = p => p.Press(Scancodes.Backspace, delay: 0.5, hold: 30) });

        Run(l, x => menus == 2, maxFrames: 40);

        Assert.Empty(l.Oracle.Differences);
        Assert.Empty(l.WeekOracle.Differences);
        Assert.Equal(2, menus);
        int[] expected = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 5, 6, 7, 8];
        Assert.Equal(expected, CoreSteps(l.Events));
        Assert.Equal(expected, EmuSteps(l.Oracle));
        var week = Assert.Single(l.Events, e => e.Kind == ProgramEventKind.StepFinished && e.Step == ProgramLoop.WeekLoopStep);
        Assert.Equal(WeekLoop.ResultMainMenu, week.Value);
        Assert.Equal(ProgramStatus.Waiting, l.Machine.Status);
        Assert.Equal(ProgramLoop.MainMenu, l.Machine.NextStep);
    }

    /// <summary>Week step that never finishes and leaves the RefEmu alone (the test ends there).</summary>
    private sealed class WeekStop : IWeekStepHandler
    {
        public StepOutcome Begin(StepContext context) => StepOutcome.Running;

        public StepOutcome Tick(StepContext context) => StepOutcome.Running;
    }

    /// <summary>First entry: <paramref name="first"/>; every later entry: <paramref name="later"/> (stops the test there).</summary>
    private sealed class SecondEntryStops(IProgramStepHandler first, IProgramStepHandler later, Func<int> count) : IProgramStepHandler
    {
        private IProgramStepHandler? _current;

        public StepOutcome Begin(ProgramStepContext context)
        {
            _current = count() == 1 ? first : later;
            return _current.Begin(context);
        }

        public StepOutcome Tick(ProgramStepContext context) => _current!.Tick(context);
    }
}
