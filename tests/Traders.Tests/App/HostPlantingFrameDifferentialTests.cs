using Traders.App;
using Traders.Core;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.RefEmu.Hardware;
using Traders.Tests.Startup;
using Xunit.Abstractions;

namespace Traders.Tests.App;

/// <summary>
/// The host's picture frame by frame through week 0 (#147, R6 lane ernte part 3): <see cref="ProgramHost"/> as the window
/// runs it – the program start, the main menu with player 1 made human, new game, then the week report (call 3), the planting
/// phase (call 8), the week incidents (call 9), the stations phase (call 11) and the harvest (call 16) – against the RefEmu
/// with the same clock and the same menu keys. From the first retrace of the week report up to the call site of call 18 (robot
/// phase, the first pending call) every host frame is one retrace of the original (entry of 0D73:00D3); there DS:10D2 of the
/// host equals the original's, and from the first frame of the planting phase on also the four VRAM planes, the DAC and the
/// CRTC start (the report's and the incidents' pictures are compared at their presenting routines by WeekReportSceneHarness and
/// EventsSceneHarness, not per retrace). The human presses a key every 35 frames in the RefEmu; the host gets the key table the
/// original holds at the end of each retrace. Expected values come only from the RefEmu.
/// </summary>
public class HostPlantingFrameDifferentialTests
{
    private static string Dir => TestGameData.DosDir;

    private const ushort RetraceSegment = 0x0D73, RetraceEntry = 0x00D3, RetraceEnd = 0x00F7;

    /// <summary>Key presses in the main menu by menu pass (as in ProgramHumanDifferentialTests): player 1 human, play, confirm controls.</summary>
    private static readonly (int Passes, byte Key)[] MenuPlan = [(3, MenuKeys.F1), (6, MenuKeys.Enter), (9, MenuKeys.Enter)];

    private const int MenuHold = 6;

    /// <summary>What the human presses in the week (control 4: arrows, Ins), one key every 35 frames, held 6 frames.</summary>
    private static readonly byte[] HumanCycle = [0x4D, 0x52, 0x4D, 0x52, 0x50, 0x52, 0x4B, 0x52, 0x50, 0x52, 0x48, 0x52];

    private const int KeyEvery = 35, KeyHold = 6;

    /// <summary>Phase of a key event within a frame (away from the retrace at the frame's start and the control reads after it).</summary>
    private const double KeyPhase = 0.4;

    private const int MaxWeekFrames = 30_000;

    private readonly ITestOutputHelper _out;

    public HostPlantingFrameDifferentialTests(ITestOutputHelper output) => _out = output;

    /// <summary>
    /// [DIFFERENTIAL] 0609:0004, 1B03:0009, 22A3:02D3 – the planting phase, the stations phase and the harvest of week 0 as the
    /// host shows them (with the week report and the incidents), frame by frame against the RefEmu: VRAM, DAC, CRTC start and
    /// random state at every retrace and at the call site of call 18. The moving objects 21AF (cursors and player figures) are
    /// on screen in the planting frames.
    /// </summary>
    // Counter-check (#147): without the host's heap pointers of the atlas and the masks (DS:45D6/4BC6 left at 0000:0000) the
    // object masks are read from the picture atlas – red at the first planting frame that draws an object.
    [GameDataFact]
    public void Host_shows_week_0_up_to_the_harvest_frame_by_frame_like_the_original()
    {
        var start = ProgramStartDifferentialTests.ReferenceStart;
        var (clock, _, _) = ProgramStartDifferentialTests.OriginalInputs(start, ProgramStartDifferentialTests.NoExtraKeys);
        var p = OriginalProgram.Start(Dir, ProgramStartDifferentialTests.Options(start));
        int emuPasses = 0;
        ushort menu = p.Segment(0x192D);
        foreach (ushort site in new ushort[] { 0x0345, 0x1AAD })
            p.Machine.AddCodeHook(menu, site, m =>
            {
                emuPasses++;
                foreach (var (n, key) in MenuPlan)
                    if (n == emuPasses) m.ScheduleKey(m.Seconds + 0.001, key, MenuHold / 70.0);
            });
        ushort loop = p.Segment(WeekLoop.Segment), call3 = WeekLoop.Step(WeekSteps.ReportCall).Site,
            call18 = WeekLoop.Step(WeekSteps.RobotCall).Site;
        p.Machine.Run(40_000_000_000, m => m.Cpu.Cs == loop && m.Cpu.IP == call3);
        Assert.True(p.Machine.Cpu.Cs == loop && p.Machine.Cpu.IP == call3 && p.Week == 0, "original did not reach call 3 of week 0");
        Assert.Equal(0, p.ReadByte(DgroupLayout.IsComputer));
        ScheduleHuman(p);

        // the host up to the frame in which the week report waits its first retrace
        var host = new ProgramHost(Dir, new RecordedClock([clock]), new NoSaveFile(), skipRobotPhase: false);
        var holds = new List<(long From, long To, byte Key)>();
        int planned = 0;
        long f = 0;
        for (; f < 5000; f++)
        {
            int passes = host.Menu.Menu?.RandomCalls.Count(c => c.Argument == 1) ?? 0;
            for (; planned < MenuPlan.Length && host.Machine.NextStep == ProgramLoop.MainMenu && passes >= MenuPlan[planned].Passes; planned++)
                holds.Add((f, f + MenuHold, MenuPlan[planned].Key));
            host.Frame(f, holds.Where(h => f >= h.From && f < h.To).Select(h => h.Key).ToList());
            if (host.Machine.WeekMachine is { NextStep: WeekSteps.ReportCall, Status: PhaseStatus.Waiting }) break;
        }
        Assert.True(host.Machine.WeekMachine is { NextStep: WeekSteps.ReportCall, Status: PhaseStatus.Waiting, Week: 0 }, "host did not reach the week report");
        Assert.Equal(0, host.Core.U8(DgroupLayout.IsComputer));
        Assert.Equal(new byte[] { 1, 1, 1 }, Enumerable.Range(1, 3).Select(i => host.Core.U8(DgroupLayout.IsComputer + i)));

        ushort retrace = p.Segment(RetraceSegment);
        var m = p.Machine;
        int retraces = 0, planting = 0, plantingWithObjects = 0;
        var frames = new Dictionary<int, int>();
        for (int frame = 0; ; frame++)
        {
            Assert.True(frame < MaxWeekFrames, $"week 0 did not reach call 18 within {MaxWeekFrames} frames");
            m.Run(1);
            m.Run(4_000_000_000, mm => (mm.Cpu.Cs == retrace && mm.Cpu.IP == RetraceEntry) || (mm.Cpu.Cs == loop && mm.Cpu.IP == call18));
            Assert.False(m.Exited, "original exited");
            bool atRetrace = m.Cpu.Cs == retrace && m.Cpu.IP == RetraceEntry;
            int step = host.Machine.WeekMachine!.NextStep;
            string where = $"week frame {frame} (host frame {f}, call {step}, planting frame {planting})";
            Assert.True(atRetrace == (host.Halt is null),
                atRetrace ? $"{where}: the host halted, the original waits a retrace" : $"{where}: the host waits a frame, the original reached call 18");
            Assert.True(p.RandomState == host.Core.U32(DgroupLayout.RandomSeed),
                $"{where}: random state original {p.RandomState:X8}, host {host.Core.U32(DgroupLayout.RandomSeed):X8}");
            // the report's and the incidents' scenes are compared at their presenting routines (WeekReportSceneHarness,
            // EventsSceneHarness), not at every retrace: their drawing between two retraces may fall into a different frame than
            // in the original (hidden page only)
            // there the shown page (from the display start) and the DAC are compared at every retrace (#157)
            if (step is not (WeekSteps.ReportCall or WeekSteps.IncidentsCall)) ComparePicture(p, host, where);
            else CompareShown(p, host, where);
            if (!atRetrace) break;
            retraces++;
            frames[step] = frames.GetValueOrDefault(step) + 1;
            if (step == WeekSteps.PlantingCall)
            {
                planting++;
                if (Enumerable.Range(0, 8).Any(i => m.Mem.Read8(p.Dgroup, (ushort)(0x330C + i * 0x1A)) != 0)) plantingWithObjects++;
            }

            m.Run(4_000_000, mm => mm.Cpu.Cs == retrace && mm.Cpu.IP == RetraceEnd);
            Assert.True(m.Cpu.Cs == retrace && m.Cpu.IP == RetraceEnd, $"{where}: retrace wait did not end");
            var keys = m.Mem.ReadBlock(p.Dgroup, DgroupLayout.KeyState, 0x80);
            f++;
            host.Frame(f, [.. Enumerable.Range(0, keys.Length).Where(sc => keys[sc] != 0).Select(sc => (byte)sc)]);
        }
        _out.WriteLine($"retraces {retraces}, planting {planting}, with objects {plantingWithObjects}; frames by call {string.Join(", ", frames.OrderBy(k => k.Key))}");

        Assert.Equal(0, p.Week);
        Assert.Contains("call 18 ", host.Halt);
        Assert.True(planting > 100, $"only {planting} planting frames");
        Assert.True(frames.GetValueOrDefault(WeekSteps.StationsCall) > 100, "too few frames of the stations phase");
        Assert.True(frames.GetValueOrDefault(WeekSteps.HarvestCall) > 100, "too few frames of the harvest");
        Assert.True(plantingWithObjects > 100, $"only {plantingWithObjects} planting frames with moving objects");
    }

    /// <summary>
    /// Quick check of #147 without the RefEmu: the host's heap pointers of the RAM atlas (DS:45D6) and the masks (DS:4BC6) in
    /// the layers' data segment address separate ranges, apart from the page buffer, the text buffer and DGROUP – the object
    /// layer 21AF tells image, mask and save buffer pointers apart by these ranges.
    /// </summary>
    [GameDataFact]
    public void Host_heap_buffers_of_atlas_and_masks_are_separate()
    {
        var host = new ProgramHost(Dir, new RecordedClock([0]), new NoSaveFile());
        var ds = host.Layers.Ds;
        int Linear(int at) => (BitConverter.ToUInt16(ds, at + 2) << 4) + BitConverter.ToUInt16(ds, at);
        int Of(uint far) => (int)(far >> 16 << 4) + (ushort)far;
        (int Start, int End, string Name)[] ranges =
        [
            (Linear(Traders.Vga.Layers.GraphicsLayers.AtlasPointer), Linear(Traders.Vga.Layers.GraphicsLayers.AtlasPointer) + host.Driver.Atlas.Data.Length, "atlas"),
            (Linear(Traders.Vga.Layers.GraphicsLayers.MaskPointer), Linear(Traders.Vga.Layers.GraphicsLayers.MaskPointer) + host.Driver.Masks.Length, "masks"),
            (Of(ProgramHost.PageBuffer), Of(ProgramHost.PageBuffer) + 0x10000, "page buffer"),
            ((ProgramHost.TextBufferSegment << 4) + ProgramHost.TextBufferOffset, (ProgramHost.TextBufferSegment << 4) + 0x10000, "text buffer"),
            (ProgramHost.DataSegmentValue << 4, (ProgramHost.DataSegmentValue << 4) + 0x10000, "DGROUP"),
        ];
        Assert.Equal(Of(ProgramHost.AtlasBuffer), ranges[0].Start);
        Assert.Equal(Of(ProgramHost.MaskBuffer), ranges[1].Start);
        foreach (var a in ranges)
            foreach (var b in ranges.Where(b => b.Name != a.Name))
                Assert.True(a.End <= b.Start || b.End <= a.Start, $"{a.Name} {a.Start:X5}-{a.End:X5} overlaps {b.Name} {b.Start:X5}-{b.End:X5}");
        Assert.True(ranges.All(r => r.End <= 0x100000), "a buffer lies above 1 MB");
    }

    /// <summary>Key presses of the human in the RefEmu: every <see cref="KeyEvery"/> frames from the next frame on.</summary>
    private static void ScheduleHuman(OriginalProgram p)
    {
        var m = p.Machine;
        long period = m.Vga.FramePeriod, frame0 = m.FrameNumber + 1;
        double Time(long frame) => (double)((frame0 + frame) * period + (long)(KeyPhase * period)) / Pit.ClockHz;
        for (int k = 0; k * KeyEvery < MaxWeekFrames; k++)
        {
            byte key = HumanCycle[k % HumanCycle.Length];
            m.ScheduleScancode(Time(k * KeyEvery), key);
            m.ScheduleScancode(Time(k * KeyEvery + KeyHold), (byte)(key | 0x80));
        }
    }

    /// <summary>VRAM planes, DAC and CRTC start of the host against the original's (first difference named).</summary>
    /// <summary>
    /// The picture on the screen: display start, the shown page (16000 bytes per plane from the display start) and the DAC;
    /// the hidden page is left out.
    /// </summary>
    private static void CompareShown(OriginalProgram o, ProgramHost host, string where)
    {
        var vga = o.Machine.Vga;
        Assert.True(vga.StartAddress == host.Driver.DisplayStart,
            $"{where}: display start original {vga.StartAddress:X4}, host {host.Driver.DisplayStart:X4}");
        int start = vga.StartAddress;
        for (int pl = 0; pl < 4; pl++)
        {
            var a = vga.Planes[pl].AsSpan(start, ShownBytes);
            var b = host.Driver.Vram.Planes[pl].AsSpan(start, ShownBytes);
            int k = a.CommonPrefixLength(b);
            if (k < ShownBytes)
                Assert.Fail($"{where}: shown page, plane {pl} offset {k:X4} (x {k % 80 * 4 + pl}, y {k / 80}) original {a[k]:X2} host {b[k]:X2}");
        }
        var dac = host.Layers.Dac.Registers;
        int d = vga.Dac.AsSpan().CommonPrefixLength(dac);
        if (d < dac.Length)
            Assert.Fail($"{where}: DAC colour {d / 3} component {d % 3} original {vga.Dac[d]:X2} host {dac[d]:X2}");
    }

    /// <summary>Bytes of a page per plane (320 × 200 / 4).</summary>
    private const int ShownBytes = 16000;

    private static void ComparePicture(OriginalProgram o, ProgramHost host, string where)
    {
        var vga = o.Machine.Vga;
        Assert.True(vga.StartAddress == host.Driver.DisplayStart,
            $"{where}: display start original {vga.StartAddress:X4}, host {host.Driver.DisplayStart:X4}");
        for (int pl = 0; pl < 4; pl++)
        {
            var a = vga.Planes[pl].AsSpan();
            var b = host.Driver.Vram.Planes[pl].AsSpan();
            int k = a.CommonPrefixLength(b);
            if (k < Math.Min(a.Length, b.Length))
                Assert.Fail($"{where}: plane {pl} offset {k:X4} (page {k / 0x4000}, x {k % 0x4000 % 80 * 4 + pl}, y {k % 0x4000 / 80}) " +
                            $"original {a[k]:X2} host {b[k]:X2}");
        }
        var dac = host.Layers.Dac.Registers;
        int d = vga.Dac.AsSpan().CommonPrefixLength(dac);
        if (d < dac.Length)
            Assert.Fail($"{where}: DAC colour {d / 3} component {d % 3} original {vga.Dac[d]:X2} host {dac[d]:X2}");
    }

    private sealed class NoSaveFile : IMenuHost
    {
        public bool LoadGame(Dgroup state) => false;

        public bool SaveGame(Dgroup state) => false;

        public bool CalibrationFailed(int stick, int step) => false;
    }
}
