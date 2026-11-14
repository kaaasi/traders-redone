using Traders.Core;
using Traders.Core.Ai;
using Traders.Core.Field;
using Traders.Core.Input;
using Traders.Core.Phases;
using Traders.Core.Rules;
using Traders.Core.State;
using Traders.Core.Week;
using Traders.RefEmu;
using Traders.RefEmu.Hardware;
using Traders.Tests.Field;
using Traders.Tests.Startup;
using Xunit.Abstractions;

namespace Traders.Tests.Phases;

/// <summary>
/// Multi-round probe (AGENTS.md §5, docs/arbeitspakete.md R5 lane anbau part 3) with the ported screen steps of the week: the
/// default game runs in the RefEmu with player 0 made human (control 4: arrows, Ins) and players 1–3 computers; the core's phase
/// machine runs the handlers of <see cref="WeekSteps"/> – the rule steps, the week report (call 3), the guard of 111D:0009
/// (call 4), the week planning (calls 6 and 21) and the planting phase with the computer turn and the wait loop (call 8) – in
/// lockstep. Rule steps and calls 4, 6, 21 are compared at their return; calls 3 and 8 frame by frame: every frame of the port
/// is a retrace of the original, the data segment (outside the bytes of the output layers and the interrupt handlers) and
/// DS:10D2 are compared at every retrace and at the return. The not yet ported calls (9 ff., and 4 beyond its guard) are
/// carried out by the RefEmu. The human presses a key every half second in the RefEmu (at a fixed phase of the frame); the port
/// gets the key table the original holds at the end of each retrace. Expected values come only from the RefEmu.
/// </summary>
public class MultiWeekScreenDifferentialTests
{
    private const int Weeks = 4;

    private readonly ITestOutputHelper _out;

    public MultiWeekScreenDifferentialTests(ITestOutputHelper output) => _out = output;

    /// <summary>What the human presses (control 4), one key every 35 frames, held 6 frames.</summary>
    private static readonly byte[] HumanCycle = [0x4D, 0x52, 0x4D, 0x52, 0x50, 0x52, 0x4B, 0x52, 0x50, 0x52, 0x48, 0x52];

    private const int KeyEvery = 35, KeyHold = 6;

    /// <summary>Phase of a key event within a frame (away from the retrace at the frame's end and the control reads after it).</summary>
    private const double KeyPhase = 0.4;

    /// <summary>
    /// Data segment bytes of the output layers in the report and the planting phase: the core leaves them to the scenes and
    /// Traders.Vga (see <see cref="PlantingFrameHarness.Owned"/>).
    /// </summary>
    private static readonly (int Offset, int Length)[] OutputOwned =
    [
        .. PlantingFrameHarness.Owned.Select(r => (r.Offset, r.Length)),
        (0x023A, 2), // 0B67:01D4/0206 draw on the shown or the hidden page (Vga pages; report 21FD:0590)
    ];

    /// <summary>
    /// [DIFFERENTIAL] 1777:01F8 with 0825:000A, 111D:0009 (guard), 11A3:0000, 0609:0004, 135D:000E, 21FD:0004 – weeks 0..3 and
    /// call 1 of week 4 with one human and three computers: calls 3, 6, 8, 21 run in the port in every week, call 4 in the port
    /// while its guard holds; nothing differs at any call site, retrace or return; the human took part (key presses read in the
    /// planting phase) and the computers planted.
    /// </summary>
    [GameDataFact]
    public void Four_weeks_with_ported_screen_steps_and_a_human_match_the_original()
    {
        var p = OriginalProgram.StartGame(TestGameData.DosDir);
        Assert.True(p.RunToWeekStart(0, 5), "week 0 did not start within 5 s after the menu");
        p.WriteByte((ushort)(ControlReader.IsComputer + 0), 0);
        p.WriteByte((ushort)(ControlReader.ControlType + 0), 4);
        ScheduleHuman(p, 250_000);
        var core = new Dgroup(p.DataSegment().AsSpan(0, DgroupLayout.Size).ToArray());
        var oracle = new RefEmuWeekOracle(p);

        // Call 1 of week 0: the RefEmu stands at the entry of 1777:02DC (same data segment as at its call site).
        WeeklyPrices.Apply(core);
        Assert.True(oracle.RunToNextStop() && oracle.IsAtLoop((ushort)(WeekLoop.Step(1).Site + 5)), "return of call 1");
        oracle.Compare(core, "week 0 after call 1 (port)");
        p.DataSegment().AsSpan(0, DgroupLayout.Size).CopyTo(core.Bytes);
        oracle.ResetPassed();

        var human = new List<string>();
        var ported = new WeekSteps(_ => { }, ProgramStartDifferentialTests.Files);
        var report = oracle.FramedPortHandler(core, ported.Handlers[WeekSteps.ReportCall], OutputOwned);
        var planting = oracle.FramedPortHandler(core, new HumanProbe(ported.Handlers[WeekSteps.PlantingCall], human), OutputOwned);
        int guardWeeks = 0;
        var handlers = WeekLoop.Steps.Where(s => s.Kind == WeekStepKind.Call).ToDictionary(s => s.Number, s => s.Number switch
        {
            WeekSteps.ReportCall => report,
            WeekSteps.PlantingCall => planting,
            CatastropheGuardStep.Call => new GuardOrOracle(oracle.PortHandler(core, ported.Handlers[s.Number]), oracle.Handler(core),
                () => guardWeeks++),
            _ when ported.Handlers.ContainsKey(s.Number) => oracle.PortHandler(core, ported.Handlers[s.Number]),
            _ => oracle.Handler(core),
        });
        handlers[2] = new StopAtWeek(handlers[2], Weeks); // the probe ends at call 2 of week 4 (a frame can run on into week 4)
        var random = new OriginalRandom(new RecordedClock([]));
        var machine = new PhaseMachine(core, random, handlers, nextStep: 2);
        var events = new List<PhaseEvent>();
        RefEmuWeekOracle.FramedStep[] framed = [report, planting];
        for (int f = 0; f < 200_000 && !(machine.Week == Weeks && machine.NextStep == 2 && machine.Status == PhaseStatus.Waiting); f++)
        {
            IReadOnlyCollection<byte>? held = null;
            foreach (var step in framed.Where(s => s.AtRetrace))
            {
                step.EndRetrace();
                held = step.Held();
            }
            machine.Advance(new PhaseTick(machine.LastFrame + 1, held));
            var drained = machine.DrainEvents();
            events.AddRange(drained);
            if (Environment.GetEnvironmentVariable("TRADERS_PROGRESS") is { Length: > 0 } progress) // diagnostics of long runs
                foreach (var e in drained.Where(e => e.Kind == PhaseEventKind.StepStarted))
                    File.AppendAllText(progress, $"{e} emulated {p.Machine.Seconds:F1} s\n");
        }
        _out.WriteLine($"frames {machine.LastFrame + 1}; report {string.Join(", ", report.Runs)}; planting {string.Join(", ", planting.Runs)}; guard ported in {guardWeeks} weeks; catastrophes {core.U8(DgroupLayout.Catastrophes)}");
        _out.WriteLine(string.Join("; ", human.Take(40)));

        Assert.Empty(oracle.Differences);
        Assert.Equal(PhaseStatus.Waiting, machine.Status);
        Assert.Equal(2, machine.NextStep);
        Assert.Equal(Weeks, machine.Week);
        Assert.Equal(Weeks, p.Week);

        var expected = Enumerable.Range(2, 29).Concat(Enumerable.Range(0, Weeks - 1).SelectMany(_ => Enumerable.Range(1, 30))).Append(1).ToList();
        Assert.Equal(expected.Append(2), events.Where(e => e.Kind is PhaseEventKind.StepStarted or PhaseEventKind.MarkerPassed).Select(e => e.Step));
        Assert.Equal(expected.Append(2), oracle.Passed.Select(site => WeekLoop.Steps.FirstOrDefault(s => s.Site == site)?.Number).OfType<int>());
        Assert.Equal(Enumerable.Range(1, Weeks), events.Where(e => e.Kind == PhaseEventKind.WeekAdvanced).Select(e => e.Value));

        // calls 3, 6, 8, 21 (and every rule step) ran as port in every week, call 4 while its guard held (weeks 0..2 at least)
        int[] screens = [WeekSteps.ReportCall, WeekPlanningStep.BeforePlantingCall, WeekSteps.PlantingCall, WeekPlanningStep.BeforeTradeCall];
        for (int week = 0; week < Weeks; week++)
        {
            var steps = oracle.PortedSteps.Where(x => x.Week == week).Select(x => x.Step).ToList();
            Assert.All(screens.Concat(WeekRuleSteps.Rules.Keys.Where(n => week > 0 || n != 1)), n => Assert.Contains(n, steps));
            if (week <= CatastropheGuardStep.LastQuietWeek) Assert.Contains(CatastropheGuardStep.Call, steps);
        }
        Assert.Equal(Weeks, report.Runs.Count);
        Assert.Equal(Weeks, planting.Runs.Count);
        Assert.All(report.Runs.Concat(planting.Runs), r => Assert.True(r.Retraces > 100, $"week {r.Week}: only {r.Retraces} frames"));
        Assert.NotEmpty(human); // the human's actions reached the planting phase
    }

    /// <summary>Key presses of the human in the RefEmu: every <see cref="KeyEvery"/> frames from the next frame on.</summary>
    private static void ScheduleHuman(OriginalProgram p, int frames)
    {
        var m = p.Machine;
        long period = m.Vga.FramePeriod, frame0 = m.FrameNumber + 1;
        double Time(long frame) => (double)((frame0 + frame) * period + (long)(KeyPhase * period)) / Pit.ClockHz;
        for (int k = 0; k * KeyEvery < frames; k++)
        {
            byte key = HumanCycle[k % HumanCycle.Length];
            m.ScheduleScancode(Time(k * KeyEvery), key);
            m.ScheduleScancode(Time(k * KeyEvery + KeyHold), (byte)(key | 0x80));
        }
    }

    /// <summary>Call 4: the port while the guard 111D:000D holds, otherwise the RefEmu.</summary>
    private sealed class GuardOrOracle(IWeekStepHandler port, IWeekStepHandler oracle, Action ported) : IWeekStepHandler
    {
        public StepOutcome Begin(StepContext context)
        {
            if (!CatastropheGuardStep.ReturnsAtOnce(context.State)) return oracle.Begin(context);
            ported();
            return port.Begin(context);
        }

        public StepOutcome Tick(StepContext context) => throw new InvalidOperationException("call 4 finishes at once");
    }

    /// <summary>Records the human's actions read in the planting phase (DS:3308[0] after each frame, when not idle).</summary>
    private sealed class HumanProbe(IWeekStepHandler inner, List<string> log) : IWeekStepHandler
    {
        public StepOutcome Begin(StepContext context) => Note(context, inner.Begin(context));

        public StepOutcome Tick(StepContext context) => Note(context, inner.Tick(context));

        private StepOutcome Note(StepContext context, StepOutcome outcome)
        {
            var d = context.State;
            sbyte action = d.S8(ControlReader.Action);
            if (action != -1 && d.U8(ControlReader.IsComputer) == 0)
                log.Add($"w{d.S16(DgroupLayout.Week)} a{action} m{d.U8(Planting.Mode)} {d.S8(FieldCursor.CursorX)},{d.S8(FieldCursor.CursorY)}");
            return outcome;
        }
    }
}
