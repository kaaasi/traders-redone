using Traders.App;
using Traders.App.Rendering;
using Traders.App.Timing;
using Traders.Core;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.RefEmu.Machine;
using Traders.Tests.Startup;

namespace Traders.Tests.App;

/// <summary>
/// The host's program run (<see cref="ProgramHost"/>): clock value of time() against the original, the main menu picture of
/// the host against the RefEmu's screen, and the run from the start through the menu into the week loop.
/// </summary>
public class ProgramHostTests
{
    private static string Dir => TestGameData.DosDir;

    /// <summary>Return address of Borland time() in random(0) 21FD:01BA (DX:AX = clock value).</summary>
    private const ushort TimeReturn = 0x01CE;

    public static TheoryData<string> Starts => new() { "1991-06-01 12:00:00", "1992-01-15 08:30:01", "1991-10-27 00:30:00", "1991-04-07 01:59:50" };

    /// <summary>
    /// [DIFFERENTIAL] 21FD:01BA – random(0) takes time() of the C runtime: the host's conversion of the local clock (EST5EDT,
    /// daylight saving of the Borland runtime) gives the value the original computed from the DOS clock, in summer, in winter
    /// and around the two switches of 1991.
    /// </summary>
    [GameDataTheory]
    [MemberData(nameof(Starts))]
    public void Clock_value_of_the_host_is_the_one_time_returned_in_the_original(string start)
    {
        var t0 = DateTime.Parse(start, System.Globalization.CultureInfo.InvariantCulture);
        var p = OriginalProgram.Start(Dir, new MachineOptions { HostDirectory = Dir, RecordSoundDriver = true, StartTime = t0 });
        int? clock = null;
        DateTime at = default;
        ushort seg = p.Segment(0x21FD);
        p.Machine.AddCodeHook(seg, TimeReturn, m =>
        {
            if (clock is not null) return;
            clock = (int)(m.Cpu.Ax | (uint)m.Cpu.Dx << 16);
            at = m.CurrentTime;
        });
        p.Machine.Run(2_000_000_000, m => clock is not null);
        Assert.NotNull(clock);
        // the DOS clock was read just before the return; allow the second to turn over in between
        int host = BorlandClock.FromLocal(at);
        Assert.True(clock == host || clock == BorlandClock.FromLocal(at.AddSeconds(-1)), $"original {clock}, host {host} at {at:O}");
    }

    [Fact]
    public void Daylight_saving_follows_the_Borland_rule()
    {
        Assert.False(BorlandClock.IsDaylightSaving(new DateTime(1991, 4, 7, 1, 59, 59)));
        Assert.True(BorlandClock.IsDaylightSaving(new DateTime(1991, 4, 7, 2, 0, 0)));
        Assert.True(BorlandClock.IsDaylightSaving(new DateTime(1991, 10, 27, 0, 59, 59)));
        Assert.False(BorlandClock.IsDaylightSaving(new DateTime(1991, 10, 27, 1, 0, 0)));
        Assert.True(BorlandClock.IsDaylightSaving(new DateTime(1986, 4, 27, 3, 0, 0))); // before 1987: last Sunday in April
        Assert.False(BorlandClock.IsDaylightSaving(new DateTime(1986, 4, 6, 3, 0, 0)));
        Assert.Equal(0 + 5 * 3600, BorlandClock.FromLocal(new DateTime(1970, 1, 1)));
    }

    /// <summary>
    /// The main menu as the host shows it, against the RefEmu: same clock value (seed after the code request), and after the
    /// third pass of the main menu loop (192D:0345) seed and displayed picture – pixels through the DAC – are the original's
    /// (both pages hold the whole menu, so the picture does not depend on the page shown).
    /// </summary>
    // Counter-check (local): a wrong font height in the host's font table → red.
    // [DIFFERENTIAL] 0B67:0002 – pages of the host (VGA path) under the menu picture
    [GameDataFact]
    public void Main_menu_picture_of_the_host_is_the_original_screen()
    {
        var start = ProgramStartDifferentialTests.ReferenceStart;
        var (clock, _, _) = ProgramStartDifferentialTests.OriginalInputs(start, ProgramStartDifferentialTests.NoExtraKeys);
        var p = OriginalProgram.Start(Dir, ProgramStartDifferentialTests.Options(start));
        int passes = 0;
        p.Machine.AddCodeHook(p.Segment(0x192D), 0x0345, _ => passes++);
        // the hook runs at the call of the 4th random(1): three draws done, the 4th pass flipped
        p.Machine.Run(4_000_000_000, _ => passes == 4);
        Assert.Equal(4, passes);
        var original = p.Frame();

        var host = new ProgramHost(Dir, new RecordedClock([clock]), new NoSaveFile());
        for (long f = 0; f < 2000 && Passes(host) < 3; f++) host.Frame(f, []);
        Assert.Equal(3, Passes(host));
        var shown = new uint[FramePresenter.Width * FramePresenter.Height];
        FramePresenter.Present(host.Driver.Vram, host.Driver.DisplayStart, host.Layers.Dac.Registers, shown);

        Assert.Equal(p.RandomState, host.Core.U32(DgroupLayout.RandomSeed));
        var expected = new uint[shown.Length];
        FramePresenter.Present(OriginalScreen(original), 0, original.Palette, expected);
        Assert.True(shown.Distinct().Count() > 8, "the host shows no menu picture");
        int diff = Enumerable.Range(0, shown.Length).Count(i => shown[i] != expected[i]);
        Assert.True(diff == 0, $"{diff} pixels differ, first at {Array.FindIndex(Enumerable.Range(0, shown.Length).ToArray(), i => shown[i] != expected[i])}");
    }

    /// <summary>
    /// [DIFFERENTIAL] 1777:01F8 – the whole host run without a window against the original (F1 = B of R6), robot switch off
    /// (<c>--original-roboter</c>): start, main menu (RETURN after the second pass, as in the RefEmu by the same pass), new game
    /// with four computers, then week 0 in the core with its ported screen steps – week report, guard of call 4, week planning,
    /// planting phase, week incidents, stations, harvest, each with its scene – up to the first pending call 18 (robot phase
    /// 1C68), where the host halts with a notice. There the data segment (outside the bytes of the output layers and the
    /// interrupt handlers) and the picture shown equal the original's at the call site of call 18. The scenes never write the
    /// core's data segment.
    /// </summary>
    [GameDataFact]
    public void Host_plays_week_0_up_to_the_robot_phase_like_the_original()
    {
        var host = PlayWeek0(skipRobotPhase: false);
        var week = host.Machine.WeekMachine!;
        Assert.Equal(WeekSteps.RobotCall, week.NextStep);
        Assert.Contains("call 18 ", host.Halt);
        Assert.Null(host.RobotSkip);
        Assert.Null(host.Deviation);
        AssertLikeOriginal(host, WeekSteps.RobotCall, skipRobotCall: false);
    }

    /// <summary>
    /// [DIFFERENTIAL] 20B3:0005 – the host run with the robot switch on (default, F3 = A of R7): week 0 as above, then the
    /// host's stand-in skips call 18 in its first frame, the end check (call 19) runs in the core and goes on (AL = 0), calls
    /// 20 and 21 follow, and the host halts at the first pending call, the trade 0B9F:0002 (call 23). Expected values from the
    /// original with its call 18 jumped over (IP set to the return address at the call site, the call takes no arguments):
    /// data segment and picture shown at the call site of call 23 are equal.
    /// </summary>
    [GameDataFact]
    public void Host_with_the_robot_switch_plays_week_0_up_to_the_trade_like_the_original_without_its_robot_phase()
    {
        var host = PlayWeek0(skipRobotPhase: true);
        var week = host.Machine.WeekMachine!;
        Assert.Equal(WeekSteps.TradeCall, week.NextStep);
        Assert.Contains("call 23 ", host.Halt);
        Assert.Equal(1, host.RobotSkip!.Skipped);
        Assert.NotNull(host.Deviation);
        Assert.False(host.Week.End.GameOver);
        AssertLikeOriginal(host, WeekSteps.TradeCall, skipRobotCall: true);
    }

    /// <summary>Start, main menu with RETURN after the second pass, week 0 until the host halts; common checks.</summary>
    private static ProgramHost PlayWeek0(bool skipRobotPhase)
    {
        var start = ProgramStartDifferentialTests.ReferenceStart;
        var (clock, _, _) = ProgramStartDifferentialTests.OriginalInputs(start, ProgramStartDifferentialTests.NoExtraKeys);
        var host = new ProgramHost(Dir, new RecordedClock([clock]), new NoSaveFile(), skipRobotPhase);
        long f = 0;
        for (; f < 2000 && Passes(host) < 2; f++) host.Frame(f, []);
        for (long until = f + 6; f < until; f++) host.Frame(f, [MenuKeys.Enter]);
        for (long until = f + 60_000; f < until && host.Halt is null; f++) host.Frame(f, []);

        Assert.Equal(ProgramStatus.InWeek, host.Machine.Status);
        Assert.True(host.Machine.IsBlocked);
        Assert.Equal(0, host.Machine.WeekMachine!.Week);
        Assert.Equal(1, host.Core.U8(ProgramLoop.GameRunning));
        Assert.Equal(new byte[] { 1, 1, 1, 1 }, Enumerable.Range(0, 4).Select(i => host.Core.U8(DgroupLayout.IsComputer + i)));
        Assert.NotSame(host.Core.Bytes, host.Layers.Ds);
        Assert.False(host.Field.Presenting);
        Assert.False(host.Report.Presenting);
        Assert.False(host.Stations.Presenting);
        Assert.False(host.Harvest.Presenting);
        return host;
    }

    /// <summary>
    /// The original with the same clock and the same key at the same menu pass, to the call site of <paramref name="call"/> in
    /// week 0 (with <paramref name="skipRobotCall"/> its call 18 jumped over): data segment and picture shown equal the host's.
    /// </summary>
    private static void AssertLikeOriginal(ProgramHost host, int call, bool skipRobotCall)
    {
        var start = ProgramStartDifferentialTests.ReferenceStart;
        var p = OriginalProgram.Start(Dir, ProgramStartDifferentialTests.Options(start));
        int passes = 0;
        ushort menu = p.Segment(0x192D);
        foreach (ushort site in new ushort[] { 0x0345, 0x1AAD })
            p.Machine.AddCodeHook(menu, site, m =>
            {
                if (++passes == 2) m.ScheduleKey(m.Seconds + 0.001, MenuKeys.Enter, 6 / 70.0);
            });
        // bytes the loader and the C start-up set before main (relocated far pointers, environment): the host's initial state
        // is the unrelocated image (Dgroup.FromExe), so these are host values, not game state
        new Traders.Tests.Phases.RefEmuProgramOracle(p).RunToCallSite(ProgramLoop.Step(1));
        var image = ProgramSteps.InitialState(File.ReadAllBytes(TestGameData.Dos("T.EXE"))).Bytes;
        var atMain = p.DataSegment();
        var loaded = Enumerable.Range(0, DgroupLayout.Size).Where(i => image[i] != atMain[i]).ToHashSet();
        ushort loop = p.Segment(WeekLoop.Segment), call18 = WeekLoop.Step(WeekSteps.RobotCall).Site, target = WeekLoop.Step(call).Site;
        p.Machine.Run(80_000_000_000, m => m.Cpu.Cs == loop && m.Cpu.IP == call18);
        Assert.True(p.Machine.Cpu.Cs == loop && p.Machine.Cpu.IP == call18, "original did not reach call 18");
        if (skipRobotCall)
        {
            p.Machine.Cpu.IP = (ushort)(call18 + 5); // over the far call 1C68:000B (no arguments, nothing to pop)
            p.Machine.Run(80_000_000_000, m => m.Cpu.Cs == loop && m.Cpu.IP == target);
            Assert.True(p.Machine.Cpu.Cs == loop && p.Machine.Cpu.IP == target, $"original did not reach call {call}");
        }
        Assert.Equal(0, p.Week);

        var original = p.DataSegment();
        // bytes of the host (heap, C runtime), the output layers and the music, as in the lockstep tests of program start,
        // menu and planting; everything else is the core's
        List<(int Offset, int Length)> ignored = Traders.Tests.Phases.RefEmuWeekOracle.InterruptOwned.Select(r => (r.Offset, r.Length))
            .Concat(Traders.Tests.Phases.RefEmuProgramOracle.HostOwnedByInitialisation.Select(r => (r.Offset, r.Length)))
            .Concat(Traders.Tests.Phases.RefEmuProgramOracle.SoundOwned.Select(r => (r.Offset, r.Length)))
            .Concat(ProgramStartDifferentialTests.OutputOwned.Select(r => (r.Offset, r.Length)))
            .Concat(Traders.Tests.Phases.ProgramHumanDifferentialTests.MenuOutputOwned.Select(r => (r.Offset, r.Length)))
            .Concat(Traders.Tests.Field.PlantingFrameHarness.Owned.Select(r => (r.Offset, r.Length)))
            .Concat(Traders.Tests.Disasters.CatastropheFrameHarness.Owned.Select(r => (r.Offset, r.Length)))
            .Concat(Traders.Tests.Stations.StationsLockstep.OutputOwned.Select(r => (r.Offset, r.Length)))
            .Concat(Traders.Tests.Harvest.HarvestFrameHarness.Owned.Select(r => (r.Offset, r.Length)))
            .Append((0x023A, 2)) // 0B67:01D4/0206 draw on the shown or the hidden page (Vga pages)
            .Append((0x37CC, 0x3C90 - 0x37CC)) // text table 192D:18B6: far pointers into the host's text buffer (6000:0000)
            .Append((0x4884, 4)) // far pointer of the text buffer (host heap)
            .ToList();
        var differing = Enumerable.Range(0, DgroupLayout.Size)
            .Where(i => original[i] != host.Core.Bytes[i] && !loaded.Contains(i) && !ignored.Any(r => i >= r.Offset && i < r.Offset + r.Length)).ToList();
        var ranges = new List<string>();
        for (int k = 0; k < differing.Count;)
        {
            int a = differing[k], b = a;
            while (k + 1 < differing.Count && differing[k + 1] <= b + 4) b = differing[++k];
            k++;
            ranges.Add($"{a:X4}-{b:X4}");
        }
        Assert.True(differing.Count == 0, $"DS differs at {string.Join(" ", ranges.Take(30))}; first {differing.FirstOrDefault():X4}");

        var shown = new uint[FramePresenter.Width * FramePresenter.Height];
        FramePresenter.Present(host.Driver.Vram, host.Driver.DisplayStart, host.Layers.Dac.Registers, shown);
        var frame = p.Frame();
        var expected = new uint[shown.Length];
        FramePresenter.Present(OriginalScreen(frame), 0, frame.Palette, expected);
        Assert.True(shown.Distinct().Count() > 8, "the host shows no field");
        int diff = Enumerable.Range(0, shown.Length).Count(i => shown[i] != expected[i]);
        Assert.True(diff == 0, $"{diff} pixels differ, first at {Enumerable.Range(0, shown.Length).FirstOrDefault(i => shown[i] != expected[i], -1)}");
    }

    private static int Passes(ProgramHost host) => host.Menu.Menu?.RandomCalls.Count(c => c.Argument == 1) ?? 0;

    /// <summary>The RefEmu's rendered picture as a mode-X page at offset 0 (pixel i in plane i % 4).</summary>
    private static Traders.Vga.ModeXMemory OriginalScreen(Traders.RefEmu.Hardware.FrameDump frame)
    {
        var vram = new Traders.Vga.ModeXMemory();
        for (int i = 0; i < frame.Width * frame.Height; i++)
        {
            int y = i / frame.Width, x = i % frame.Width;
            vram.Planes[x & 3][y * (frame.Width / 4) + (x >> 2)] = frame.Pixels[i];
        }
        return vram;
    }

    private sealed class NoSaveFile : IMenuHost
    {
        public bool LoadGame(Dgroup state) => false;

        public bool SaveGame(Dgroup state) => false;

        public bool CalibrationFailed(int stick, int step) => false;
    }
}
