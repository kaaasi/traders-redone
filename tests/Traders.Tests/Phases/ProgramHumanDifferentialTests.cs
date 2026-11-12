using Traders.Core;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.Startup;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.Menus;
using Traders.Tests.Startup;

namespace Traders.Tests.Phases;

/// <summary>
/// Multi-round probe with a human player (AGENTS.md §5, #81 B3): the whole program from the first call of main through four
/// weeks in lockstep with the RefEmu, with every call step of main ported – initialisation, program start with code request
/// and text table, sound stubs, main menu 192D:0007 (player 1 made human, controls confirmed), new game 0E31:0003 – and the
/// week loop with the ported rule steps; the screen steps of the week are carried out by the RefEmu (the human presses keys
/// there). At every call site the data segment is compared, after every ported step outside the bytes of the output layers
/// and the host. Expected values come only from the RefEmu.
/// </summary>
public class ProgramHumanDifferentialTests
{
    private const int Weeks = 4;

    private static string Dir => TestGameData.DosDir;

    /// <summary>
    /// Bytes the main menu 192D:0007 writes in the original that the core leaves to the output layers (Traders.Vga keeps them
    /// in its own state; the menu harness of lane menue stubs these layers). Found by a run without exceptions: all other
    /// bytes of the data segment are equal after the menu (key bytes of int 9 are interrupt-owned).
    /// </summary>
    public static readonly (int Offset, int Length, string Owner)[] MenuOutputOwned =
    [
        (0x15BA, 0x9FA, "0A96 drawing list (Vga command list)"),
        (0x3DCB, 0x300, "21FD:026C/02C9 base palette (Vga palettes)"),
        (0x54E8, 0x300, "21FD:03EB working palette (Vga palettes)"),
    ];

    /// <summary>Key presses in the main menu by menu pass: after the n-th random(1) of the menu loops (192D:0345, 192D:1AAD).</summary>
    private static readonly (int Passes, byte Key)[] MenuPlan =
    [
        (3, MenuKeys.F1),     // player 1 becomes human (default control 4: arrows, Ins)
        (6, MenuKeys.Enter),  // play → controls menu (192D:197F)
        (9, MenuKeys.Enter),  // controls confirmed
    ];

    /// <summary>Frames a menu key is held (the RefEmu holds it as long at 70 Hz).</summary>
    private const int HoldFrames = 6;

    /// <summary>What the human presses during the weeks in the RefEmu (control 4), one key every half second, from the menu's return.</summary>
    private static readonly byte[] HumanCycle = [0x4D, 0x52, 0x4D, 0x52, 0x50, 0x52, 0x4B, 0x52, 0x50, 0x52, 0x48, 0x52];

    private sealed class NoSaveFile : IMenuHost
    {
        public bool LoadGame(Dgroup state) => false;

        public bool SaveGame(Dgroup state) => false;

        public bool CalibrationFailed(int stick, int step) => false;
    }

    /// <summary>
    /// [DIFFERENTIAL] 1777:000C with 0DFD:000D, 192D:0007, 0E31:0003 and 1777:01F8 – program start to call 1 of week 4 with one
    /// human and three computer players: the core takes the same calls of main and the same 30 calls per week as the original,
    /// nothing differs at any call site or after any ported step, the menu made player 1 human, and DS:10D2 is the original's.
    /// </summary>
    // [DIFFERENTIAL] 192D:0007 – main menu as program step 8 (whole data segment outside MenuOutputOwned at its return)
    // [DIFFERENTIAL] 0E31:0003 – new game as program step 9 after the ported menu
    [GameDataFact]
    public void Program_with_a_human_player_runs_four_weeks_in_lockstep_with_the_original()
    {
        var r = Run(MenuPlan);

        Assert.Empty(r.Oracle.Differences);
        Assert.Empty(r.WeekOracle.Differences);
        Assert.Equal(0, r.Core.U8(DgroupLayout.IsComputer + 0));
        Assert.Equal(new byte[] { 1, 1, 1 }, Enumerable.Range(1, 3).Select(i => r.Core.U8(DgroupLayout.IsComputer + i)));
        Assert.Equal(Weeks, r.Program.Week);
        Assert.Equal(r.Program.RandomState, r.Core.U32(DgroupLayout.RandomSeed));
        int[] main = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        Assert.Equal(main, r.Events.Where(e => e.Kind == ProgramEventKind.StepStarted).Select(e => e.Step));
        Assert.Equal(main, r.Oracle.Passed.Select(site => ProgramLoop.Steps.FirstOrDefault(s => s.Site == site)?.Number).OfType<int>());
        var expected = Enumerable.Range(1, 30).Repeat(Weeks).Append(1).Append(2).ToList(); // call 2 of week 4 waits (end)
        Assert.Equal(expected, r.WeekEvents.Where(e => e.Kind is PhaseEventKind.StepStarted or PhaseEventKind.MarkerPassed).Select(e => e.Step));
        Assert.Equal(Enumerable.Range(1, Weeks), r.WeekEvents.Where(e => e.Kind == PhaseEventKind.WeekAdvanced).Select(e => e.Value));
        // every ported rule step ran in every week; the menu passed its two loops in the planned number of passes
        foreach (int week in Enumerable.Range(0, Weeks))
            Assert.Equal(WeekRuleSteps.Rules.Keys.Order(), r.WeekOracle.PortedSteps.Where(x => x.Week == week).Select(x => x.Step));
        Assert.Equal(10, r.Menu.Menu!.RandomCalls.Count(c => c.Argument == 1));
        Assert.Equal(10, r.EmuMenuPasses);
    }

    /// <summary>
    /// Counter-check: the same run with the controls confirmed one pass later in the core only (the original keeps the plan)
    /// draws random(1) once more in the core – the comparison after the main menu fails at the seed.
    /// </summary>
    [GameDataFact]
    public void Menu_with_one_more_pass_in_the_core_differs_from_the_original()
    {
        var e = Assert.Throws<InvalidOperationException>(() => Run(MenuPlan, [(3, MenuKeys.F1), (6, MenuKeys.Enter), (10, MenuKeys.Enter)]));
        Assert.Contains("DS:10D", e.Message);
    }

    private sealed record Result(ProgramMachine Machine, Dgroup Core, OriginalProgram Program, RefEmuProgramOracle Oracle,
        RefEmuWeekOracle WeekOracle, MainMenuProgramStep Menu, List<ProgramEvent> Events, List<PhaseEvent> WeekEvents, int EmuMenuPasses);

    private static Result Run((int Passes, byte Key)[] emuPlan, (int Passes, byte Key)[]? corePlan = null)
    {
        corePlan ??= emuPlan;
        var start = ProgramStartDifferentialTests.ReferenceStart;
        var (clock, _, buffer) = ProgramStartDifferentialTests.OriginalInputs(start, ProgramStartDifferentialTests.NoExtraKeys);
        var p = OriginalProgram.Start(Dir, ProgramStartDifferentialTests.Options(start));
        var emuPasses = HookMenu(p, emuPlan);

        var oracle = new RefEmuProgramOracle(p);
        oracle.RunToCallSite(ProgramLoop.Step(1));
        var core = new Dgroup(p.DataSegment().AsSpan(0, DgroupLayout.Size).ToArray());
        var weekOracle = new RefEmuWeekOracle(p);
        var weekHandlers = WeekLoop.Steps.Where(s => s.Kind == WeekStepKind.Call).ToDictionary(s => s.Number,
            s => WeekRuleSteps.Rules.ContainsKey(s.Number) ? weekOracle.PortHandler(core, WeekRuleSteps.Handler(s.Number)) : weekOracle.Handler(core));
        weekHandlers[1] = oracle.EnteringWeekLoop(core, weekHandlers[1]);
        weekHandlers[2] = new StopAtWeek(weekHandlers[2], Weeks); // oracle steps finish at once: a frame can run whole weeks

        var programStart = new ProgramStart(ProgramStartDifferentialTests.Files, new ProgramStartDifferentialTests.CheckedPlaque(),
            ProgramStartDifferentialTests.Texts(buffer));
        var menu = new MainMenuProgramStep(new NoSaveFile(), ProgramStartDifferentialTests.Files);
        var all = ProgramSteps.All(ProgramInitialisation.AdLib, programStart, menu);
        var handlers = new Dictionary<int, IProgramStepHandler>
        {
            [1] = oracle.PortHandler(core, all[1], RefEmuProgramOracle.HostOwnedByInitialisation),
            [2] = oracle.PortHandler(core, all[2], ProgramStartDifferentialTests.OutputOwned),
            [ProgramLoop.MainMenu] = oracle.FramedPortHandler(core, all[ProgramLoop.MainMenu], MenuOutputOwned),
            [ProgramLoop.NewGame] = oracle.PortHandler(core, all[ProgramLoop.NewGame], []),
        };
        foreach (int n in ProgramSteps.SoundSteps) handlers[n] = oracle.PortHandler(core, all[n], RefEmuProgramOracle.SoundOwned);
        var machine = new ProgramMachine(core, new OriginalRandom(new RecordedClock([clock])), handlers, weekHandlers);

        var events = new List<ProgramEvent>();
        var weekEvents = new List<PhaseEvent>();
        var holds = new List<(long From, long To, byte Key)>();
        int planned = 0;
        for (long f = 0; f < 4000 && !(machine.WeekMachine is { } w && w.Week == Weeks && w.Status == PhaseStatus.Waiting); f++)
        {
            int passes = machine.NextStep == ProgramLoop.MainMenu ? menu.Menu?.RandomCalls.Count(c => c.Argument == 1) ?? 0 : 0;
            for (; planned < corePlan.Length && machine.NextStep == ProgramLoop.MainMenu && passes >= corePlan[planned].Passes; planned++)
                holds.Add((f, f + HoldFrames, corePlan[planned].Key));
            var held = machine.Status == ProgramStatus.InWeek ? null : holds.Where(h => f >= h.From && f < h.To).Select(h => h.Key).ToList();
            machine.Advance(new PhaseTick(f, held));
            events.AddRange(machine.DrainEvents());
            weekEvents.AddRange(machine.DrainWeekEvents());
            menu.DrainEvents();
        }
        return new Result(machine, core, p, oracle, weekOracle, menu, events, weekEvents, emuPasses());

        static Func<int> HookMenu(OriginalProgram prog, (int Passes, byte Key)[] plan)
        {
            int count = 0;
            ushort seg = prog.Segment(0x192D);
            foreach (ushort site in new ushort[] { 0x0345, 0x1AAD })
                prog.Machine.AddCodeHook(seg, site, m =>
                {
                    count++;
                    foreach (var (n, key) in plan)
                        if (n == count) m.ScheduleKey(m.Seconds + 0.001, key, HoldFrames / 70.0);
                });
            // the human's keys during the weeks, from the return of the main menu on
            bool scheduled = false;
            prog.Machine.AddCodeHook(prog.Segment(ProgramLoop.Segment), ProgramLoop.Step(ProgramLoop.MainMenu).ReturnSite, m =>
            {
                if (scheduled) return;
                scheduled = true;
                for (int k = 0; k < 4000; k++) m.ScheduleKey(m.Seconds + 1 + 0.5 * k, HumanCycle[k % HumanCycle.Length], 0.1);
            });
            return () => count;
        }
    }
}

/// <summary>Call step that never finishes once the week counter DS:4B9A reached <paramref name="week"/> (end of the test).</summary>
internal sealed class StopAtWeek(IWeekStepHandler inner, int week) : IWeekStepHandler
{
    public StepOutcome Begin(StepContext context) =>
        context.State.S16(DgroupLayout.Week) == week ? StepOutcome.Running : inner.Begin(context);

    public StepOutcome Tick(StepContext context) => StepOutcome.Running;
}

internal static class SequenceExtensions
{
    public static IEnumerable<T> Repeat<T>(this IEnumerable<T> source, int times) => Enumerable.Range(0, times).SelectMany(_ => source);
}
