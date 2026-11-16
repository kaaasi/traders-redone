using Traders.Assets;
using Traders.Core;
using Traders.Core.Ai;
using Traders.Core.Disasters;
using Traders.Core.Events;
using Traders.Core.Field;
using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.Pixels;
using Traders.Core.Rules;
using Traders.Core.State;
using Traders.Core.Stations;
using Traders.RefEmu;
using Traders.RefEmu.Hardware;
using Traders.Tests.Disasters;
using Traders.Tests.Field;
using Traders.Tests.Harvest;
using Traders.Tests.Startup;
using Traders.Tests.Stations;
using Xunit.Abstractions;

namespace Traders.Tests.Phases;

/// <summary>
/// Multi-round probe of round 6 (AGENTS.md §5, docs/arbeitspakete.md R6 lane ernte part 3): the default game runs in the RefEmu
/// with player 0 made human (control 4: arrows, Ins), players 1–3 computers and the catastrophes switched on (DS:2F30 = 1); the
/// core's phase machine runs the handlers of <see cref="WeekScreens"/> bound into <see cref="WeekSteps"/> in lockstep: besides
/// the rule steps and the week planning (calls 6, 21, compared at their return) the screen steps 3 (week report), 4 (guard and
/// catastrophes 111D:0009), 8 (planting), 9 (week incidents 17CB:0005), 11 (stations 1B03:0009) and 16 (harvest 22A3:02D3) frame
/// by frame: every frame of the port is one retrace of the original (entry of 0D73:00D3); at every retrace and at the return the
/// data segment (outside the bytes of the output layers of that step and of the interrupt handlers) and DS:10D2 are compared.
/// The end check (call 19, 20B3:0005 up to its decision, R7 lane ende) runs as port, compared at its return with AL; call
/// 18 and the trade steps 23–30 are carried out by the RefEmu. The human presses a key every half second in the
/// RefEmu (R5 key pattern); the port gets the key table the original holds at the end of each retrace, a time() value the
/// original reads (random(0)) and, at the call site of call 11, the driver's current font. Expected values come only from the
/// RefEmu.
/// </summary>
public class MultiWeekRound6DifferentialTests
{
    /// <summary>Weeks 0..4 run in full (call 4 beyond its guard from week 3 on); the probe ends at call 2 of week 5.</summary>
    private const int Weeks = 5;

    private const ushort Seg111D = 0x111D, Driver = 0x23CD, DriverFont = 0x0586;

    private readonly ITestOutputHelper _out;

    public MultiWeekRound6DifferentialTests(ITestOutputHelper output) => _out = output;

    /// <summary>What the human presses (control 4), one key every 35 frames, held 6 frames (the R5 pattern).</summary>
    private static readonly byte[] HumanCycle = [0x4D, 0x52, 0x4D, 0x52, 0x50, 0x52, 0x4B, 0x52, 0x50, 0x52, 0x48, 0x52];

    private const int KeyEvery = 35, KeyHold = 6;

    /// <summary>Phase of a key event within a frame (away from the retrace at the frame's end and the control reads after it).</summary>
    private const double KeyPhase = 0.4;

    private static (int Offset, int Length)[] Ranges(IEnumerable<(int Offset, int Length, string Owner)> owned) =>
        [.. owned.Select(r => (r.Offset, r.Length))];

    /// <summary>Output bytes of the week report and the planting phase (as in <see cref="MultiWeekScreenDifferentialTests"/>).</summary>
    private static readonly (int Offset, int Length)[] PlantingOwned =
        [.. Ranges(PlantingFrameHarness.Owned), (0x023A, 2)];

    /// <summary>Output bytes of each round-6 step: those its own frame harness leaves to the scene.</summary>
    private static readonly (int Offset, int Length)[] CatastropheOwned = Ranges(CatastropheFrameHarness.Owned);

    private static readonly (int Offset, int Length)[] IncidentsOwned =
    [
        .. PlantingOwned,
        (0x0140, 1),  // 0A69:01AD character being typed (Vga text)
        (0x0142, 2),  // 21FD:05DB text advance and shadow height (Vga)
        (0x34A4, 2),  // 21FD:05DB line height (Vga)
        (0x3CA8, 2),  // 21FD:05DB text advance (Vga)
    ];

    private static readonly (int Offset, int Length)[] StationsOwned = Ranges(StationsLockstep.OutputOwned);

    private static readonly (int Offset, int Length)[] HarvestOwned = Ranges(HarvestFrameHarness.Owned);

    private static readonly Lazy<IndexedImage[]> Pictures = new(() =>
        [.. ResourceTable.Pictures.OrderBy(e => (int)e.Id).Select(e =>
        {
            var image = VgaImage.Load(TestGameData.Dos(e.Vga));
            return new IndexedImage(VgaImage.Width, VgaImage.Height, image.Pixels);
        })]);

    /// <summary>
    /// [DIFFERENTIAL] 1777:01F8 with 0825:000A, 111D:0009, 11A3:0000, 0609:0004, 17CB:0005, 1B03:0009, 22A3:02D3, 20B3:0005 – weeks 0..4 and
    /// call 1 of week 5 with one human and three computers, catastrophes on: calls 3, 4, 6, 8, 9, 11, 16, 21 run in the port in
    /// every week (3, 4, 8, 9, 11, 16 frame by frame); nothing differs at any call site, retrace or return; at least one
    /// catastrophe (111D:0112 with one of 0264, 03B7, 0519) runs in the port; the human took part; the text buffer (amounts of
    /// 2295:0001) equals the original's after every ported step of round 6 (taken over from the RefEmu after its steps).
    /// </summary>
    [GameDataFact]
    public void Five_weeks_with_the_steps_of_round_6_and_a_human_match_the_original()
    {
        var p = OriginalProgram.StartGame(TestGameData.DosDir);
        Assert.True(p.RunToWeekStart(0, 5), "week 0 did not start within 5 s after the menu");
        p.WriteByte((ushort)(ControlReader.IsComputer + 0), 0);
        p.WriteByte((ushort)(ControlReader.ControlType + 0), 4);
        p.WriteByte(DgroupLayout.Catastrophes, 1);
        ScheduleHuman(p, 600_000);
        var m = p.Machine;
        var core = new Dgroup(p.DataSegment().AsSpan(0, DgroupLayout.Size).ToArray());
        var oracle = new RefEmuWeekOracle(p);

        // time() values the original reads (random(0)); the oracle runs each frame of the original before the port's
        var clock = new Queue<int>();
        m.AddCodeHook(p.Segment(0x21FD), 0x01CE, mm => clock.Enqueue(mm.Cpu.Ax | (mm.Cpu.Dx << 16)));
        // catastrophes in the original: entries of the star show 111D:0112 and of the three kinds
        var struck = new List<(int Week, string What)>();
        ushort own = p.Segment(Seg111D);
        m.AddCodeHook(own, 0x0112, _ => struck.Add((p.Week, "0112")));
        foreach (var (entry, kind) in new[] { (0x0264, "kind 0"), (0x03B7, "kind 1"), (0x0519, "kind 2") })
            m.AddCodeHook(own, (ushort)entry, _ => struck.Add((p.Week, kind)));

        // the port's text buffer: a copy of the original's after the program start; 2295:0001 writes amounts into it
        ushort textOff = core.U16(MenuLayout.TextBuffer), textSeg = core.U16(MenuLayout.TextBuffer + 2);
        var texts = m.Mem.ReadBlock(textSeg, textOff, 0x8000);
        int font = -1;
        var masks = StationsLockstep.Masks.Value;
        var screens = new WeekScreens(
            new WeekScreenInputs(ProgramStartDifferentialTests.Files, n => Pictures.Value[n], masks,
                () => StationsLockstep.Font(font), () => texts),
            new WeekScreenOutputs(_ => { }, e => NumberIntoText(core, e, texts), _ => { }, _ => { }));

        // Call 1 of week 0: the RefEmu stands at the entry of 1777:02DC (same data segment as at its call site).
        WeeklyPrices.Apply(core);
        Assert.True(oracle.RunToNextStop() && oracle.IsAtLoop((ushort)(WeekLoop.Step(1).Site + 5)), "return of call 1");
        oracle.Compare(core, "week 0 after call 1 (port)");
        p.DataSegment().AsSpan(0, DgroupLayout.Size).CopyTo(core.Bytes);
        oracle.ResetPassed();

        var human = new List<string>();
        var ported = screens.Week(_ => { }, ProgramStartDifferentialTests.Files);
        void ClearClock(OriginalProgram _) => clock.Clear();
        var report = oracle.FramedPortHandler(core, ported.Handlers[WeekSteps.ReportCall], PlantingOwned, ClearClock);
        var catastrophes = oracle.FramedPortHandler(core, ported.Handlers[CatastropheGuardStep.Call], CatastropheOwned, ClearClock);
        var planting = oracle.FramedPortHandler(core, new HumanProbe(ported.Handlers[WeekSteps.PlantingCall], human), PlantingOwned, ClearClock);
        var incidents = oracle.FramedPortHandler(core, ported.Handlers[WeekSteps.IncidentsCall], IncidentsOwned, ClearClock);
        var humanStations = new List<string>();
        var humanHarvest = new List<string>();
        var stations = oracle.FramedPortHandler(core, new HumanProbe(ported.Handlers[WeekSteps.StationsCall], humanStations), StationsOwned, o =>
        {
            clock.Clear();
            font = o.Machine.Mem.Read16(o.Segment(Driver), DriverFont); // the driver's current font: input of the stations phase
        });
        var harvest = oracle.FramedPortHandler(core, new HumanProbe(ported.Handlers[WeekSteps.HarvestCall], humanHarvest), HarvestOwned, ClearClock);
        RefEmuWeekOracle.FramedStep[] framed = [report, catastrophes, planting, incidents, stations, harvest];
        // the text buffer outside DS: compared after every ported step, taken over after every step of the RefEmu (the trade
        // phases also write amounts into it)
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
            WeekSteps.ReportCall => report,
            CatastropheGuardStep.Call => new AfterStep(catastrophes, CompareTexts),
            WeekSteps.PlantingCall => planting,
            WeekSteps.IncidentsCall => new AfterStep(incidents, CompareTexts),
            WeekSteps.StationsCall => new AfterStep(stations, CompareTexts),
            WeekSteps.HarvestCall => new AfterStep(harvest, CompareTexts),
            WeekSteps.EndCall => oracle.PortHandler(core, ported.Handlers[s.Number], compareResult: true),
            _ when ported.Handlers.ContainsKey(s.Number) => oracle.PortHandler(core, ported.Handlers[s.Number]),
            _ => new AfterStep(oracle.Handler(core), TakeOverTexts),
        });
        handlers[2] = new StopAtWeek(handlers[2], Weeks); // the probe ends at call 2 of week 5 (a frame can run on into week 5)
        var random = new OriginalRandom(new QueueClock(clock));
        var machine = new PhaseMachine(core, random, handlers, nextStep: 2);
        var events = new List<PhaseEvent>();
        var catastrophesInPort = new List<(int Week, int Retraces)>();
        for (int f = 0; f < 400_000 && !(machine.Week == Weeks && machine.NextStep == 2 && machine.Status == PhaseStatus.Waiting); f++)
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
            foreach (var e in drained.Where(e => e is { Kind: PhaseEventKind.StepFinished, Step: CatastropheGuardStep.Call }))
                if (screens.Catastrophes.Current is CatastropheStep c && !CatastropheGuardStep.ReturnsAtOnce(core))
                    catastrophesInPort.Add((e.Week, c.Retraces));
            if (Environment.GetEnvironmentVariable("TRADERS_PROGRESS") is { Length: > 0 } progress) // diagnostics of long runs
                foreach (var e in drained.Where(e => e.Kind == PhaseEventKind.StepStarted))
                    File.AppendAllText(progress, $"{e} emulated {p.Machine.Seconds:F1} s\n");
        }
        _out.WriteLine($"frames {machine.LastFrame + 1}; struck {string.Join(", ", struck)}; catastrophes in port {string.Join(", ", catastrophesInPort)}");
        foreach (var (name, step) in new[] { ("report", report), ("call 4", catastrophes), ("planting", planting), ("incidents", incidents), ("stations", stations), ("harvest", harvest) })
            _out.WriteLine($"{name}: {string.Join(", ", step.Runs)}");
        _out.WriteLine($"human actions: planting {human.Count}, stations {humanStations.Count}, harvest {humanHarvest.Count}");
        _out.WriteLine(string.Join("; ", human.Take(40)));

        Assert.Empty(oracle.Differences);
        Assert.Equal(PhaseStatus.Waiting, machine.Status);
        Assert.Equal(2, machine.NextStep);
        Assert.Equal(Weeks, machine.Week);
        Assert.Equal(Weeks, p.Week);

        var expected = Enumerable.Range(2, 29).Concat(Enumerable.Range(0, Weeks - 1).SelectMany(_ => Enumerable.Range(1, 30))).Append(1).ToList();
        Assert.Equal(expected.Append(2), events.Where(e => e.Kind is PhaseEventKind.StepStarted or PhaseEventKind.MarkerPassed).Select(e => e.Step));
        Assert.Equal(expected.Append(2), oracle.Passed.Select(site => WeekLoop.Steps.FirstOrDefault(s => s.Site == site)?.Number).OfType<int>());

        // calls 3, 4, 6, 8, 9, 11, 16, 19, 21 (and every rule step) ran as port in every week
        int[] screenCalls =
        [
            WeekSteps.ReportCall, CatastropheGuardStep.Call, WeekPlanningStep.BeforePlantingCall, WeekSteps.PlantingCall,
            WeekSteps.IncidentsCall, WeekSteps.StationsCall, WeekSteps.HarvestCall, WeekSteps.EndCall, WeekPlanningStep.BeforeTradeCall,
        ];
        for (int week = 0; week < Weeks; week++)
        {
            var steps = oracle.PortedSteps.Where(x => x.Week == week).Select(x => x.Step).ToList();
            Assert.All(screenCalls.Concat(WeekRuleSteps.Rules.Keys.Where(n => week > 0 || n != 1)), n => Assert.Contains(n, steps));
        }
        foreach (var step in new[] { report, planting, stations, harvest })
        {
            Assert.Equal(Weeks, step.Runs.Count);
            Assert.All(step.Runs, r => Assert.True(r.Retraces > 100, $"week {r.Week}: only {r.Retraces} frames"));
        }
        // the incidents return at once in a week without an incident; at least one week shows one
        Assert.Equal(Weeks, incidents.Runs.Count);
        Assert.Contains(incidents.Runs, r => r.Retraces > 100);
        Assert.Equal(Weeks, catastrophes.Runs.Count);

        // R6P-2: at least one catastrophe (star show 0112 and a kind) ran in the port frame by frame and was compared
        var weeksStruck = struck.Where(s => s.What != "0112" && struck.Contains((s.Week, "0112"))).Select(s => s.Week).ToHashSet();
        Assert.NotEmpty(weeksStruck);
        Assert.Contains(catastrophesInPort, c => weeksStruck.Contains(c.Week) && c.Retraces > 0);
        Assert.All(weeksStruck, w => Assert.Contains(catastrophes.Runs, r => r.Week == w && r.Retraces > 0));

        // the human's actions reached the planting phase, the stations phase and the harvest (R6E-3 #164, R6S-2 #170)
        Assert.NotEmpty(human);
        Assert.NotEmpty(humanStations);
        Assert.NotEmpty(humanHarvest);
        Assert.Equal(4 * Weeks, textChecks.Count); // text buffer equal after calls 4, 9, 11, 16 of every week
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

    /// <summary>
    /// What the stations scene does with a number for a text (2295:0001 is an output call of the stations phase, the layers
    /// write it into the text memory, StationScene): written into the port's text buffer when the core reports it.
    /// </summary>
    private static void NumberIntoText(Dgroup core, StationEvent e, byte[] texts)
    {
        if (e.Call != StationCall.NumberText || e.Args[4] == StationEvent.DataSegment) return;
        var a = e.Args;
        IncidentText.FormatNumber(core, new TextBufferMemory(core, () => texts), (uint)((ushort)a[0] | (ushort)a[1] << 16), a[2],
            (ushort)a[3], (ushort)a[4]);
    }

    /// <summary>Runs <paramref name="after"/> when the inner step returns.</summary>
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

    /// <summary>The time() values the original read in the frame the port processes (the original runs it first).</summary>
    private sealed class QueueClock(Queue<int> values) : IClockInput
    {
        public int TimeSeconds() =>
            values.Count > 0 ? values.Dequeue() : throw new InvalidOperationException("the original read no time() value in this frame");
    }

    /// <summary>Records the human's actions read in a phase (DS:3308[0] after each frame, when not idle).</summary>
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
