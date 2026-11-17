using Traders.Core;
using Traders.Core.Events;
using Traders.Core.FatMike;
using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.Pixels;
using Traders.Core.Purchase;
using Traders.Core.Rules;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.RefEmu.Hardware;
using Traders.Tests.FatMike;
using Traders.Tests.Purchase;
using Traders.Tests.Startup;
using Xunit.Abstractions;

namespace Traders.Tests.Phases;

/// <summary>
/// Binding of round 8 (lane woche part 1): <see cref="WeekScreens.Week"/> binds Fat Mike (call 26, <see cref="FatMikeStep"/>),
/// the purchase (call 29, <see cref="PurchaseStep"/>) and the contest (call 30, <see cref="ContestStep"/> with its second field
/// round), made anew at every call; the core alone (<see cref="WeekSteps"/> without handlers) and a week made with
/// <c>round8: false</c> keep them pending.
/// </summary>
public class WeekScreensRound8Tests
{
    private static WeekScreens Screens() => new(
        new WeekScreenInputs(_ => default, _ => new IndexedImage(1, 1, new byte[1]), [], () => default!, () => null),
        new WeekScreenOutputs(_ => { }, _ => { }, _ => { }, _ => { }));

    [Fact]
    public void The_week_binds_calls_26_29_and_30_and_leaves_18_pending()
    {
        var screens = Screens();
        var week = screens.Week(_ => { }, _ => default);
        Assert.Same(screens.FatMike, week.Handlers[WeekSteps.FatMikeCall]);
        Assert.Same(screens.Purchase, week.Handlers[WeekSteps.PurchaseCall]);
        Assert.Same(screens.Contest, week.Handlers[WeekSteps.ContestCall]);
        Assert.False(week.Handlers.ContainsKey(WeekSteps.RobotCall));
        Assert.Equal([WeekSteps.FatMikeCall, WeekSteps.PurchaseCall, WeekSteps.ContestCall], WeekSteps.Round8Calls);
        Assert.Equal([26, 29, 30], WeekSteps.Round8Calls.Select(n => WeekLoop.Step(n).Number));
        Assert.Equal((0x1F63, 0x000F), (WeekLoop.Step(WeekSteps.FatMikeCall).TargetSegment, WeekLoop.Step(WeekSteps.FatMikeCall).TargetOffset));
        Assert.Equal((0x08CF, 0x0004), (WeekLoop.Step(WeekSteps.PurchaseCall).TargetSegment, WeekLoop.Step(WeekSteps.PurchaseCall).TargetOffset));
        Assert.Equal((0x1F63, 0x0B03), (WeekLoop.Step(WeekSteps.ContestCall).TargetSegment, WeekLoop.Step(WeekSteps.ContestCall).TargetOffset));

        // with a stand-in for call 18 every call step of the week has a handler
        var all = screens.Week(_ => { }, _ => default, robots: new RobotPhaseSkipStub());
        Assert.All(WeekLoop.Steps.Where(s => s.Kind == WeekStepKind.Call), s => Assert.True(all.Handlers.ContainsKey(s.Number), $"call {s.Number}"));

        var before = screens.Week(_ => { }, _ => default, round8: false);
        Assert.All(WeekSteps.Round8Calls, n => Assert.False(before.Handlers.ContainsKey(n)));
        var core = new WeekSteps(_ => { }, _ => default);
        Assert.All(WeekSteps.Round8Calls, n => Assert.False(core.Handlers.ContainsKey(n)));
    }

    /// <summary>A handler standing in for call 18 (finishes at once).</summary>
    private sealed class RobotPhaseSkipStub : IWeekStepHandler
    {
        public StepOutcome Begin(StepContext context) => StepOutcome.Finished();

        public StepOutcome Tick(StepContext context) => throw new InvalidOperationException("finishes at once");
    }
}

/// <summary>
/// The handlers of round 8 as <see cref="WeekScreens"/> binds them, in a running game against the original (lane woche part 1):
/// the default game runs in the RefEmu with player 0 made human (control 4: arrows, Ins) and players 1–3 computers; every call
/// step except 26, 29 and 30 is carried out by the RefEmu (<see cref="RefEmuWeekOracle.Handler"/>, its data segment taken
/// over), the three steps run in the core's <see cref="PhaseMachine"/> frame by frame (<see cref="RefEmuWeekOracle.FramedPortHandler"/>):
/// every frame of the port is one retrace of the original; at every retrace and at the return the data segment (outside the
/// output layers' bytes of lanes fatmike and einkauf and the interrupt handlers) and DS:10D2 are compared. The human presses a
/// key every half second (R5 key pattern); the port gets the key table the original holds at the end of each retrace and the
/// time() values the original reads. The text buffer the handlers read is the original's after the program start, taken over
/// after every step of the RefEmu and compared after every ported step. Expected values come only from the RefEmu.
/// </summary>
public class WeekScreensRound8DifferentialTests
{
    /// <summary>Weeks 0..3 (the contest falls in week 2); the run ends at call 2 of week 4.</summary>
    private const int Weeks = 4;

    /// <summary>Frames at most (a stuck run ends red instead of running on).</summary>
    private const int MaxFrames = 200_000;

    private readonly ITestOutputHelper _out;

    public WeekScreensRound8DifferentialTests(ITestOutputHelper output) => _out = output;

    /// <summary>What the human presses (control 4), one key every 35 frames, held 6 frames (the R5 pattern).</summary>
    private static readonly byte[] HumanCycle = [0x4D, 0x52, 0x4D, 0x52, 0x50, 0x52, 0x4B, 0x52, 0x50, 0x52, 0x48, 0x52];

    private const int KeyEvery = 35, KeyHold = 6;

    private const double KeyPhase = 0.4;

    private static readonly (int Offset, int Length)[] FatMikeOwned = [.. FatMikeFrameHarness.Owned.Select(r => (r.Offset, r.Length))];

    private static readonly (int Offset, int Length)[] PurchaseOwned = [.. PurchaseFrameHarness.Owned.Select(r => (r.Offset, r.Length))];

    /// <summary>
    /// [DIFFERENTIAL] 1F63:000F, 08CF:0004, 1F63:0B03 – weeks 0..3 with one human and three computers: calls 26, 29 and 30 run
    /// in every week as the handlers <see cref="WeekScreens.FatMike"/>, <see cref="WeekScreens.Purchase"/> and
    /// <see cref="WeekScreens.Contest"/> (production hosts), 26 and 29 frame by frame, 30 with its scene in week 2; nothing
    /// differs at any call site, retrace or return; the loop passes all 30 calls of every week in order and the week counter
    /// goes 0 → 4 in the core's machine.
    /// </summary>
    /// <remarks>Counter-check (local): Fat Mike ticked twice in one frame → red at its first retrace.</remarks>
    [GameDataFact]
    public void Calls_26_29_and_30_bound_by_the_week_screens_match_the_original_for_four_weeks()
    {
        var p = OriginalProgram.StartGame(TestGameData.DosDir);
        Assert.True(p.RunToWeekStart(0, 5), "week 0 did not start within 5 s after the menu");
        p.WriteByte((ushort)(ControlReader.IsComputer + 0), 0);
        p.WriteByte((ushort)(ControlReader.ControlType + 0), 4);
        ScheduleHuman(p, 400_000);
        var m = p.Machine;
        var core = new Dgroup(p.DataSegment().AsSpan(0, DgroupLayout.Size).ToArray());
        var oracle = new RefEmuWeekOracle(p);

        var clock = new Queue<int>();
        m.AddCodeHook(p.Segment(0x21FD), 0x01CE, mm => clock.Enqueue(mm.Cpu.Ax | (mm.Cpu.Dx << 16)));

        ushort textOff = core.U16(MenuLayout.TextBuffer), textSeg = core.U16(MenuLayout.TextBuffer + 2);
        var texts = m.Mem.ReadBlock(textSeg, textOff, 0x8000);
        var screens = new WeekScreens(
            new WeekScreenInputs(ProgramStartDifferentialTests.Files, _ => throw new InvalidOperationException("no rule page in R8"),
                [], () => throw new InvalidOperationException("no font in R8"), () => texts),
            new WeekScreenOutputs(_ => { }, _ => { }, _ => { }, _ => { }));

        WeeklyPrices.Apply(core);
        Assert.True(oracle.RunToNextStop() && oracle.IsAtLoop((ushort)(WeekLoop.Step(1).Site + 5)), "return of call 1");
        oracle.Compare(core, "week 0 after call 1 (port)");
        p.DataSegment().AsSpan(0, DgroupLayout.Size).CopyTo(core.Bytes);
        oracle.ResetPassed();

        var ported = screens.Week(_ => { }, ProgramStartDifferentialTests.Files);
        void ClearClock(OriginalProgram _) => clock.Clear();
        var fatMike = oracle.FramedPortHandler(core, ported.Handlers[WeekSteps.FatMikeCall], FatMikeOwned, ClearClock);
        var purchase = oracle.FramedPortHandler(core, ported.Handlers[WeekSteps.PurchaseCall], PurchaseOwned, ClearClock);
        var contest = oracle.FramedPortHandler(core, ported.Handlers[WeekSteps.ContestCall], FatMikeOwned, ClearClock);
        RefEmuWeekOracle.FramedStep[] framed = [fatMike, purchase, contest];

        var textChecks = new List<string>();
        byte[] OriginalTexts() => m.Mem.ReadBlock(textSeg, textOff, MenuTexts.ScanLimit + 1);
        void CompareTexts(WeekStep step)
        {
            var o = OriginalTexts();
            int k = o.AsSpan().CommonPrefixLength(texts.AsSpan(0, o.Length));
            if (k < o.Length) throw new InvalidOperationException($"week {p.Week} after {step}: text buffer +{k:X4} original {o[k]:X2} port {texts[k]:X2}");
            textChecks.Add($"{p.Week}/{step.Number}");
        }
        void TakeOverTexts(WeekStep _) => OriginalTexts().CopyTo(texts, 0);
        var handlers = WeekLoop.Steps.Where(s => s.Kind == WeekStepKind.Call).ToDictionary(s => s.Number, s => s.Number switch
        {
            WeekSteps.FatMikeCall => new AfterStep(fatMike, CompareTexts),
            WeekSteps.PurchaseCall => new AfterStep(purchase, CompareTexts),
            WeekSteps.ContestCall => new AfterStep(contest, CompareTexts),
            _ => (IWeekStepHandler)new AfterStep(oracle.Handler(core), TakeOverTexts),
        });
        handlers[2] = new StopAtWeek(handlers[2], Weeks);
        var random = new OriginalRandom(new QueueClock(clock));
        var machine = new PhaseMachine(core, random, handlers, nextStep: 2);
        var events = new List<PhaseEvent>();
        for (int f = 0; f < MaxFrames && !(machine.Week == Weeks && machine.NextStep == 2 && machine.Status == PhaseStatus.Waiting); f++)
        {
            IReadOnlyCollection<byte>? held = null;
            foreach (var step in framed.Where(s => s.AtRetrace))
            {
                step.EndRetrace();
                held = step.Held();
            }
            machine.Advance(new PhaseTick(machine.LastFrame + 1, held));
            events.AddRange(machine.DrainEvents());
        }
        foreach (var (name, step) in new[] { ("Fat Mike", fatMike), ("purchase", purchase), ("contest", contest) })
            _out.WriteLine($"{name}: {string.Join(", ", step.Runs)}");
        _out.WriteLine($"frames {machine.LastFrame + 1}, emulated {m.Seconds:F0} s");

        Assert.Empty(oracle.Differences);
        Assert.Equal(PhaseStatus.Waiting, machine.Status);
        Assert.Equal(2, machine.NextStep);
        Assert.Equal(Weeks, machine.Week);
        Assert.Equal(Weeks, p.Week);
        var expected = Enumerable.Range(2, 29).Concat(Enumerable.Range(0, Weeks - 1).SelectMany(_ => Enumerable.Range(1, 30))).Append(1).ToList();
        Assert.Equal(expected.Append(2), events.Where(e => e.Kind is PhaseEventKind.StepStarted or PhaseEventKind.MarkerPassed).Select(e => e.Step));
        Assert.Equal(expected.Append(2), oracle.Passed.Select(site => WeekLoop.Steps.FirstOrDefault(s => s.Site == site)?.Number).OfType<int>());

        // Fat Mike and the purchase run frame by frame in every week; the contest returns at once outside week mod 3 = 2
        Assert.Equal(Weeks, fatMike.Runs.Count);
        Assert.All(fatMike.Runs, r => Assert.True(r.Retraces > 100, $"Fat Mike, week {r.Week}: only {r.Retraces} frames"));
        Assert.Equal(Weeks, purchase.Runs.Count);
        Assert.All(purchase.Runs, r => Assert.True(r.Retraces > 100, $"purchase, week {r.Week}: only {r.Retraces} frames"));
        Assert.Equal(Weeks, contest.Runs.Count);
        Assert.All(contest.Runs, r => Assert.True((r.Week % 3 == 2) == (r.Retraces > 0), $"contest, week {r.Week}: {r.Retraces} frames"));
        Assert.Equal(3 * Weeks, textChecks.Count);
    }

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

    private sealed class AfterStep(IWeekStepHandler inner, Action<WeekStep> after) : IWeekStepHandler
    {
        public StepOutcome Begin(StepContext context) => Done(context, inner.Begin(context));

        public StepOutcome Tick(StepContext context) => Done(context, inner.Tick(context));

        private StepOutcome Done(StepContext context, StepOutcome outcome)
        {
            if (outcome.Done) after(context.Step);
            return outcome;
        }
    }

    private sealed class QueueClock(Queue<int> values) : IClockInput
    {
        public int TimeSeconds() =>
            values.Count > 0 ? values.Dequeue() : throw new InvalidOperationException("the original read no time() value in this frame");
    }
}
