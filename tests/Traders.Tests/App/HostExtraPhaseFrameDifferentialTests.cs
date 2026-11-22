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
/// #208 (R7EE-1) and lane woche part 3 (R8): the host with the robot switch on (<c>skipRobotPhase: true</c>, the default) frame
/// by frame through week 0 and over the week boundary into the planting phase of week 1, against the RefEmu with the same
/// clock and menu keys. As in <see cref="HostPlantingFrameDifferentialTests"/> every host frame is one retrace of the original;
/// the original jumps over its call 18 (IP set past the far call) where the host's stand-in skips it, and both sides run on
/// through calls 19–21, the trade (23), the extra phase (24), Fat Mike (26), the purchase (29) and the contest (30), then the
/// week counter DS:4B9A goes to 1 and week 1 runs from its balance and week report (the week axis of 0825:047D now with two
/// weeks) up to the end of its planting phase (call 8). Without help week 0 has no extra phase (random(20) = 0 is needed), so
/// after the harvest of week 0 DS:57F0 is set to 2 * DS:57F2 + 1 on both sides: the decision of 1DEF:00DB (the only reader)
/// then falls, the pictures of the extra scene are compared pixel by pixel. random(20) is still drawn on both sides.
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

    private const int MaxWeekFrames = 60_000;

    private readonly ITestOutputHelper _out;

    public HostExtraPhaseFrameDifferentialTests(ITestOutputHelper output) => _out = output;

    /// <summary>The data word the extra phase compares (1DEF:00DB reads DS:57F0 and shifts DS:57F2).</summary>
    private const int ExtraLimit = 0x57F0, ExtraBase = 0x57F2;

    [GameDataFact]
    public void Host_with_the_robot_switch_shows_week_0_and_the_week_boundary_into_week_1_frame_by_frame()
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
        int skipped = 0;
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
        var frames = new Dictionary<(int Week, int Call), int>();
        for (int frame = 0; ; frame++)
        {
            Assert.True(frame < MaxWeekFrames, $"week 1 did not reach the end of its planting within {MaxWeekFrames} frames");
            m.Run(1);
            m.Run(4_000_000_000, mm => (mm.Cpu.Cs == retrace && mm.Cpu.IP == RetraceEntry) || (mm.Cpu.Cs == loop && mm.Cpu.IP == call18));
            Assert.False(m.Exited, "original exited");
            if (m.Cpu.Cs == loop && m.Cpu.IP == call18)
            {
                // the original jumps over its robot phase (far call 1C68:000B, no arguments) as the host's stand-in does in its
                // next frame; in week 0 both sides then draw random(20) in 1DEF:00DB against DS:57F0, forced to fall
                Assert.True(skipped == 0, "call 18 reached in week 1 before the end of its planting");
                skipped++;
                Assert.Equal(0, p.Week);
                m.Cpu.IP = (ushort)(call18 + 5);
                ushort limit = (ushort)(2 * m.Mem.Read16(p.Dgroup, ExtraBase) + 1);
                m.Mem.Write16(p.Dgroup, ExtraLimit, limit);
                host.Core.Set(ExtraLimit, limit);
                frame--;
                continue;
            }
            int step = host.Machine.WeekMachine!.NextStep, week = host.Machine.WeekMachine.Week;
            string where = $"week frame {frame} (host frame {f}, week {week}, call {step}, planting frame {planting})";
            Assert.True(host.Halt is null, $"{where}: the host halted ({host.Halt}), the original waits a retrace");
            Assert.True(week == p.Week, $"{where}: week of the original {p.Week}");
            Assert.True(p.RandomState == host.Core.U32(DgroupLayout.RandomSeed),
                $"{where}: random state original {p.RandomState:X8}, host {host.Core.U32(DgroupLayout.RandomSeed):X8}");
            // the report's and the incidents' scenes are compared at their presenting routines (WeekReportSceneHarness,
            // EventsSceneHarness), not at every retrace: their drawing between two retraces may fall into a different frame than
            // in the original (hidden page only)
            // there the shown page (from the display start) and the DAC are compared at every retrace (#157)
            if (step is not (WeekSteps.ReportCall or WeekSteps.IncidentsCall)) HostPictures.Compare(p, host, where);
            else HostPictures.CompareShown(p, host, where);
            // the frames of week 1 are compared up to the first one after its planting phase
            if (week == 1 && step > WeekSteps.PlantingCall) break;
            retraces++;
            frames[(week, step)] = frames.GetValueOrDefault((week, step)) + 1;
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
        _out.WriteLine($"retraces {retraces}, planting {planting}, with objects {plantingWithObjects}; frames by week and call {string.Join(", ", frames.OrderBy(k => k.Key))}");

        Assert.Equal(1, p.Week);
        Assert.Null(host.Halt);
        Assert.False(host.GameOver);
        Assert.Equal(1, skipped);
        Assert.Equal(1, host.RobotSkip!.Skipped);
        int Frames(int week, int call) => frames.GetValueOrDefault((week, call));
        Assert.True(Frames(0, WeekSteps.TradeCall) > 100, "too few frames of the trade");
        Assert.True(Frames(0, WeekSteps.ExtraCall) > 100, "too few frames of the extra phase");
        Assert.True(Frames(0, WeekSteps.StationsCall) > 100, "too few frames of the stations phase");
        Assert.True(Frames(0, WeekSteps.HarvestCall) > 100, "too few frames of the harvest");
        Assert.True(Frames(0, WeekSteps.FatMikeCall) > 0, "no frame of Fat Mike");
        Assert.True(Frames(0, WeekSteps.PurchaseCall) > 100, "too few frames of the purchase");
        // the contest of week 0 ends without a retrace wait (frame by frame in a contest week: MultiWeekRound7DifferentialTests)
        Assert.IsType<Traders.Core.FatMike.ContestStep>(host.Screens.Contest.Current);
        Assert.True(Frames(1, WeekSteps.ReportCall) > 0, "no frame of the week report of week 1");
        Assert.True(Frames(1, WeekSteps.PlantingCall) > 100, "too few planting frames in week 1");
        Assert.True(planting > 200, $"only {planting} planting frames");
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
