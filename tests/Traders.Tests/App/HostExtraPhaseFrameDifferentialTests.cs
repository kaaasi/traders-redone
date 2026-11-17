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
/// #208 (R7EE-1): the host with the robot switch on (<c>skipRobotPhase: true</c>, the default) frame by frame through week 0
/// into the extra phase, against the RefEmu with the same clock and menu keys. As in <see cref="HostPlantingFrameDifferentialTests"/>
/// every host frame is one retrace of the original; the original jumps over its call 18 (IP set past the far call) where the
/// host's stand-in skips it, and both sides run on through calls 19–21, the trade (23) and the extra phase (24) to the call
/// site of Fat Mike (26, 1777:02B5). Without help week 0 has no extra phase (random(20) = 0 is needed), so after the harvest
/// DS:57F0 is set to 2 * DS:57F2 + 1 on both sides: the decision of 1DEF:00DB (the only reader) then falls, the pictures of
/// the extra scene are compared pixel by pixel. random(20) is still drawn on both sides.
/// Expected values come only from the RefEmu.
/// </summary>
public class HostExtraPhaseFrameDifferentialTests
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

    public HostExtraPhaseFrameDifferentialTests(ITestOutputHelper output) => _out = output;

    /// <summary>The data word the extra phase compares (1DEF:00DB reads DS:57F0 and shifts DS:57F2).</summary>
    private const int ExtraLimit = 0x57F0, ExtraBase = 0x57F2;

    [GameDataFact]
    public void Host_with_the_robot_switch_shows_week_0_through_the_forced_extra_phase_frame_by_frame()
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
            call18 = WeekLoop.Step(WeekSteps.RobotCall).Site, call26 = WeekLoop.Step(WeekSteps.FatMikeCall).Site;
        bool skipped = false;
        p.Machine.Run(40_000_000_000, m => m.Cpu.Cs == loop && m.Cpu.IP == call3);
        Assert.True(p.Machine.Cpu.Cs == loop && p.Machine.Cpu.IP == call3 && p.Week == 0, "original did not reach call 3 of week 0");
        Assert.Equal(0, p.ReadByte(DgroupLayout.IsComputer));
        ScheduleHuman(p);

        // the host up to the frame in which the week report waits its first retrace
        var host = new ProgramHost(Dir, new RecordedClock([clock]), new NoSaveFile(), skipRobotPhase: true);
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
            Assert.True(frame < MaxWeekFrames, $"week 0 did not reach call 26 within {MaxWeekFrames} frames");
            m.Run(1);
            m.Run(4_000_000_000, mm => (mm.Cpu.Cs == retrace && mm.Cpu.IP == RetraceEntry) || (mm.Cpu.Cs == loop && (mm.Cpu.IP == call18 || mm.Cpu.IP == call26)));
            Assert.False(m.Exited, "original exited");
            if (m.Cpu.Cs == loop && m.Cpu.IP == call18)
            {
                // the original jumps over its robot phase (far call 1C68:000B, no arguments) as the host's stand-in does in its
                // next frame; both sides then draw random(20) in 1DEF:00DB against DS:57F0, forced to fall
                Assert.False(skipped, "call 18 reached twice");
                skipped = true;
                Assert.Equal(0, p.Week);
                m.Cpu.IP = (ushort)(call18 + 5);
                ushort limit = (ushort)(2 * m.Mem.Read16(p.Dgroup, ExtraBase) + 1);
                m.Mem.Write16(p.Dgroup, ExtraLimit, limit);
                host.Core.Set(ExtraLimit, limit);
                frame--;
                continue;
            }
            bool atRetrace = m.Cpu.Cs == retrace && m.Cpu.IP == RetraceEntry;
            int step = host.Machine.WeekMachine!.NextStep;
            string where = $"week frame {frame} (host frame {f}, call {step}, planting frame {planting})";
            Assert.True(atRetrace == (host.Halt is null),
                atRetrace ? $"{where}: the host halted, the original waits a retrace" : $"{where}: the host waits a frame, the original reached call 26");
            Assert.True(p.RandomState == host.Core.U32(DgroupLayout.RandomSeed),
                $"{where}: random state original {p.RandomState:X8}, host {host.Core.U32(DgroupLayout.RandomSeed):X8}");
            // the report's and the incidents' scenes are compared at their presenting routines (WeekReportSceneHarness,
            // EventsSceneHarness), not at every retrace: their drawing between two retraces may fall into a different frame than
            // in the original (hidden page only)
            // there the shown page (from the display start) and the DAC are compared at every retrace (#157)
            if (step is not (WeekSteps.ReportCall or WeekSteps.IncidentsCall)) HostPictures.Compare(p, host, where);
            else HostPictures.CompareShown(p, host, where);
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
        Assert.Contains("call 26 ", host.Halt);
        Assert.True(skipped, "the robot phase was not skipped");
        Assert.Equal(1, host.RobotSkip!.Skipped);
        Assert.True(frames.GetValueOrDefault(WeekSteps.TradeCall) > 100, "too few frames of the trade");
        Assert.True(frames.GetValueOrDefault(WeekSteps.ExtraCall) > 100, "too few frames of the extra phase");
        Assert.True(planting > 100, $"only {planting} planting frames");
        Assert.True(frames.GetValueOrDefault(WeekSteps.StationsCall) > 100, "too few frames of the stations phase");
        Assert.True(frames.GetValueOrDefault(WeekSteps.HarvestCall) > 100, "too few frames of the harvest");
        Assert.True(plantingWithObjects > 100, $"only {plantingWithObjects} planting frames with moving objects");
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

    private sealed class NoSaveFile : IMenuHost
    {
        public bool LoadGame(Dgroup state) => false;

        public bool SaveGame(Dgroup state) => false;

        public bool CalibrationFailed(int stick, int step) => false;
    }
}
