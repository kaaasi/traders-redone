using Traders.App;
using Traders.Core;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.RefEmu.Cpu;
using Traders.Tests.Startup;
using Xunit.Abstractions;

namespace Traders.Tests.App;

/// <summary>
/// Issue #180: the host's page state at call 4 with a catastrophe and at call 9 with a week incident, frame by frame against
/// the original. A game of the default kind (four computers) is played in the RefEmu to the start of week 3 with the
/// catastrophes switched on (DS:2F30 = 1); its save file (2336:000A layout, <see cref="SaveGame"/>) with the week set to
/// <see cref="LoadedWeek"/> (15: 111D:0009 strikes in every multiple of 15) is then put into the data segment of a fresh host and a fresh original at the same main menu pass,
/// as the disk menu's load 192D:1177 does (DS:08FA = 1 as well; the disk menu's screens are left out on both sides). RETURN
/// and A (continue, 192D:12A6) go on with the loaded game. From the first retrace of the week report of that week up to the
/// first frame of the stations phase (call 11) every host frame is one retrace of the original: random state DS:10D2 and the
/// picture – VRAM, DAC and CRTC start, in the report and the incidents the shown page and the DAC – are compared at every
/// retrace, and at the first frame of call 11 the page state of the output layers (DS:082E/0830, 40DE…40E5, 4516) with the
/// shown page. The stations phase after the incident is left out: from its 41st frame the host shows a figure at the top
/// left that the original does not draw (the hidden page already differs by 88 bytes at its first frame) – finding of this
/// test, reported in PR #185 for the scenes. Expected values come only from the RefEmu.
/// </summary>
public class HostLoadedWeekFrameDifferentialTests
{
    private static string Dir => TestGameData.DosDir;

    private const ushort RetraceSegment = 0x0D73, RetraceEntry = 0x00D3, RetraceEnd = 0x00F7;

    /// <summary>Week the game is played to in the RefEmu (catastrophes from week 3 on, 111D:000D).</summary>
    private const int PlayedWeek = 3;

    /// <summary>Week of the loaded game: a multiple of 15, so 111D:0009 brings a catastrophe whatever it draws.</summary>
    private const int LoadedWeek = 15;

    /// <summary>Main menu pass after which the save file is put in, and the pass at which RETURN is pressed.</summary>
    private const int LoadPass = 2, EnterPass = 3;

    /// <summary>Frames RETURN and A are held (A covers the fade-in of the dialog 192D:12A6 and its key poll).</summary>
    private const int EnterHold = 6, ContinueHold = 120;

    private const int MaxWeekFrames = 40_000;

    /// <summary>
    /// Start time of both runs (the clock of random(0) at the code request), chosen so that the loaded week also has a week
    /// incident: of the start times 12:00, 12:07 … 12:42 only 12:21 and 12:35 give one (host runs, checked by this test).
    /// </summary>
    private static readonly DateTime Start = new(1991, 6, 1, 12, 35, 0);

    private readonly ITestOutputHelper _out;

    public HostLoadedWeekFrameDifferentialTests(ITestOutputHelper output) => _out = output;

    /// <summary>
    /// [DIFFERENTIAL] 111D:0009, 17CB:0005 – catastrophe and week incident of a loaded week 15 as the host shows them, frame by
    /// frame against the RefEmu (page state of the output layers given to the core at the start of calls 4, 9 and 11,
    /// <see cref="WeekScreens.MirrorPages"/>).
    /// </summary>
    [GameDataFact]
    public void Host_shows_a_loaded_week_with_catastrophe_and_incident_frame_by_frame_like_the_original()
    {
        var save = WeekSave();
        var (clock, _, _) = ProgramStartDifferentialTests.OriginalInputs(Start, ProgramStartDifferentialTests.NoExtraKeys);

        var p = OriginalProgram.Start(Dir, ProgramStartDifferentialTests.Options(Start));
        int emuPasses = 0;
        ushort menu = p.Segment(0x192D);
        foreach (ushort site in new ushort[] { 0x0345, 0x1AAD })
            p.Machine.AddCodeHook(menu, site, m =>
            {
                emuPasses++;
                if (emuPasses == LoadPass) Load(p, save);
                if (emuPasses == EnterPass)
                {
                    m.ScheduleKey(m.Seconds + 0.001, MenuKeys.Enter, EnterHold / 70.0);
                    m.ScheduleKey(m.Seconds + 0.001, MenuKeys.A, ContinueHold / 70.0);
                }
            });
        ushort loop = p.Segment(WeekLoop.Segment), call3 = WeekLoop.Step(WeekSteps.ReportCall).Site;
        p.Machine.Run(40_000_000_000, m => m.Cpu.Cs == loop && m.Cpu.IP == call3);
        Assert.True(p.Machine.Cpu.Cs == loop && p.Machine.Cpu.IP == call3, "original did not reach call 3");
        Assert.Equal(LoadedWeek, p.Week);

        var host = new ProgramHost(Dir, new RecordedClock([clock]), new NoSaveFile(), skipRobotPhase: false);
        long f = 0, enterFrom = -1;
        bool loaded = false;
        for (; f < 8000; f++)
        {
            var held = enterFrom >= 0 && f >= enterFrom
                ? (f < enterFrom + EnterHold ? [MenuKeys.Enter, MenuKeys.A] : f < enterFrom + ContinueHold ? [MenuKeys.A] : new List<byte>())
                : [];
            host.Frame(f, held);
            int passes = host.Menu.Menu?.RandomCalls.Count(c => c.Argument == 1) ?? 0;
            if (passes == LoadPass && !loaded)
            {
                Load(host.Core, save);
                loaded = true;
            }
            if (passes == EnterPass && enterFrom < 0) enterFrom = f + 1;
            if (host.Machine.WeekMachine is { NextStep: WeekSteps.ReportCall, Status: PhaseStatus.Waiting }) break;
        }
        Assert.True(host.Machine.WeekMachine is { NextStep: WeekSteps.ReportCall, Status: PhaseStatus.Waiting, Week: LoadedWeek },
            "host did not reach the week report of the loaded week");

        ushort retrace = p.Segment(RetraceSegment);
        var m = p.Machine;
        var frames = new Dictionary<int, int>();
        for (int frame = 0; ; frame++)
        {
            Assert.True(frame < MaxWeekFrames, $"the week did not reach call 11 within {MaxWeekFrames} frames");
            m.Run(1);
            m.Run(4_000_000_000, mm => mm.Cpu.Cs == retrace && mm.Cpu.IP == RetraceEntry);
            Assert.False(m.Exited, "original exited");
            Assert.True(host.Halt is null, $"week frame {frame}: the host halted, the original waits a retrace");
            int step = host.Machine.WeekMachine!.NextStep;
            string where = $"week frame {frame} (host frame {f}, call {step})";
            Assert.True(p.RandomState == host.Core.U32(DgroupLayout.RandomSeed),
                $"{where}: random state original {p.RandomState:X8}, host {host.Core.U32(DgroupLayout.RandomSeed):X8}");
            if (step == WeekSteps.StationsCall && frames.GetValueOrDefault(step) == 0)
            {
                // first frame of the stations phase: the page state the core took over and the screen
                var emu = p.DataSegment();
                foreach (var (offset, length) in WeekScreens.PageState)
                    for (int i = offset; i < offset + length; i++)
                    {
                        Assert.True(emu[i] == host.Layers.Ds[i], $"{where}: page state DS:{i:X4} original {emu[i]:X2} layers {host.Layers.Ds[i]:X2}");
                        Assert.True(emu[i] == host.Core.U8(i), $"{where}: page state DS:{i:X4} original {emu[i]:X2} core {host.Core.U8(i):X2}");
                    }
                HostPictures.CompareShown(p, host, where);
            }
            if (step == WeekSteps.HarvestCall && frames.GetValueOrDefault(step) >= 50) break;
            if (step is WeekSteps.ReportCall or WeekSteps.IncidentsCall) HostPictures.CompareShown(p, host, where);
            else HostPictures.Compare(p, host, where);
            frames[step] = frames.GetValueOrDefault(step) + 1;

            m.Run(4_000_000, mm => mm.Cpu.Cs == retrace && mm.Cpu.IP == RetraceEnd);
            Assert.True(m.Cpu.Cs == retrace && m.Cpu.IP == RetraceEnd, $"{where}: retrace wait did not end");
            var keys = m.Mem.ReadBlock(p.Dgroup, DgroupLayout.KeyState, 0x80);
            f++;
            host.Frame(f, [.. Enumerable.Range(0, keys.Length).Where(sc => keys[sc] != 0).Select(sc => (byte)sc)]);
        }
        _out.WriteLine($"frames by call {string.Join(", ", frames.OrderBy(k => k.Key))}");

        Assert.Equal(LoadedWeek, p.Week);
        Assert.True(frames.GetValueOrDefault(CatastropheGuardStep.Call) > 100, "no catastrophe in the loaded week");
        Assert.True(frames.GetValueOrDefault(WeekSteps.IncidentsCall) > 100, "no week incident in the loaded week");
    }

    /// <summary>
    /// The save file of the default game at the start of <see cref="PlayedWeek"/>, catastrophes on (from the RefEmu), with
    /// the week counter set to <see cref="LoadedWeek"/>.
    /// </summary>
    private static byte[] WeekSave()
    {
        var p = OriginalProgram.StartGame(Dir);
        Assert.True(p.RunToWeekStart(0, 5), "week 0 did not start");
        p.WriteByte(DgroupLayout.Catastrophes, 1);
        Assert.True(p.RunToWeekStart(PlayedWeek, 3600), $"week {PlayedWeek} did not start");
        var d = new Dgroup(p.DataSegment().AsSpan(0, DgroupLayout.Size).ToArray());
        d.Set(DgroupLayout.Week, (short)LoadedWeek);
        return SaveGame.Write(d);
    }

    /// <summary>What the load 192D:1177 does to the data segment: the saved fields (2336:0539) and DS:08FA = 1.</summary>
    private static void Load(Dgroup d, byte[] save)
    {
        Assert.True(SaveGame.Read(save, d));
        d.Set(MenuLayout.GameInProgress, (byte)1);
    }

    private static void Load(OriginalProgram p, byte[] save)
    {
        var d = new Dgroup(p.DataSegment().AsSpan(0, DgroupLayout.Size).ToArray());
        Load(d, save);
        p.Machine.Mem.WriteBlock(Memory.Linear(p.Dgroup, 0), d.Bytes.AsSpan(0, DgroupLayout.Size));
    }

    private sealed class NoSaveFile : IMenuHost
    {
        public bool LoadGame(Dgroup state) => false;

        public bool SaveGame(Dgroup state) => false;

        public bool CalibrationFailed(int stick, int step) => false;
    }
}
