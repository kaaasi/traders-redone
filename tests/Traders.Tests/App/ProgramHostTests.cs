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
        var host = PlayToHalt(skipRobotPhase: false);
        var week = host.Machine.WeekMachine!;
        Assert.Equal(0, week.Week);
        Assert.Equal(WeekSteps.RobotCall, week.NextStep);
        Assert.Contains("call 18 ", host.Halt);
        Assert.False(host.GameOver);
        Assert.Null(host.RobotSkip);
        Assert.Null(host.Deviation);
        AssertLikeOriginal(host, WeekSteps.RobotCall, week: 0);
    }

    /// <summary>
    /// [DIFFERENTIAL] 20B3:0005, 0B9F:0002, 1DEF:000C, 1F63:000F, 08CF:0004, 1F63:0B03 – the host run with the robot switch on
    /// (default, F3 = A of R7) without a halt over the week boundary to the end of the game (lane woche part 3): week 0 as above,
    /// then the host's stand-in skips call 18 in its first frame, the end check (call 19) goes on (AL = 0), calls 20 and 21,
    /// the trade (23), the extra phase (24), Fat Mike (26), the gem rate (27), the purchase (29) and the contest (30) run in
    /// the core with their scenes, the week counter DS:4B9A goes to 1 and week 1 runs through all its calls up to its end check.
    /// After the end check of week 0 the target DS:08F4 is set to 0 on both sides (the only other reader is the target line of
    /// the week report, drawn on both sides alike), so the end check of week 1 ends the game (exit B): the end screens run in
    /// the core with their scene (lane partie part 1) up to the any-key wait 27C6:00D2, a key (RETURN) ends it, the week loop
    /// returns AL = 1 and main goes to its teardown (1777:0184 → 018C): the program machine ends with exit code 0. Expected
    /// values from the original with its call 18 jumped over in both weeks (IP set to the return address at the call site, the
    /// call takes no arguments) and a key once it waits in 27C6:00D2: data segment and picture shown at the teardown
    /// 1777:018C are equal.
    /// Counter-check (local, lane woche): the host built with <c>round8: false</c> (halts at call 26) → red.
    /// </summary>
    [GameDataFact]
    public void Host_with_the_robot_switch_plays_over_the_week_boundary_through_the_end_screens_to_the_teardown_like_the_original()
    {
        var host = PlayToHalt(skipRobotPhase: true);
        var week = host.Machine.WeekMachine!;
        Assert.Equal(1, week.Week);
        Assert.Equal(WeekSteps.EndCall, week.NextStep);
        Assert.True(host.GameOver);
        Assert.Null(host.Halt);
        Assert.Equal(2, host.RobotSkip!.Skipped);
        Assert.NotNull(host.Deviation);
        Assert.False(host.Trade.Presenting);
        Assert.False(host.Extra.Presenting);
        Assert.False(host.FatMike.Presenting);
        Assert.False(host.Purchase.Presenting);
        var trade = Assert.IsType<Traders.Core.Trade.TradeStep>(host.Screens.Trade.Current);
        Assert.True(trade.Retraces > 100, $"trade ran {trade.Retraces} frames");
        Assert.IsType<Traders.Core.Extra.ExtraStep>(host.Screens.Extra.Current);
        Assert.IsType<Traders.Core.FatMike.FatMikeStep>(host.Screens.FatMike.Current);
        var purchase = Assert.IsType<Traders.Core.Purchase.PurchaseStep>(host.Screens.Purchase.Current);
        Assert.True(purchase.Retraces > 100, $"purchase ran {purchase.Retraces} frames");
        Assert.IsType<Traders.Core.FatMike.ContestStep>(host.Screens.Contest.Current);

        // the end screens up to the key wait, a key, the teardown
        var ending = Assert.IsType<Traders.Core.Ending.EndingStep>(host.Screens.Ending!.Current);
        long f = host.LastFrame + 1;
        for (long until = f + 60_000; f < until && ending.Polls == 0; f++) host.Frame(f, []);
        Assert.True(ending.Polls > 0, "the end screens did not reach the key wait");
        Assert.True(ending.Retraces > 1000, $"the end screens ran {ending.Retraces} frames");
        for (int i = 0; i < 6; i++) host.Frame(f++, [MenuKeys.Enter]);
        for (long until = f + 100; f < until && host.Machine.Status != ProgramStatus.Ended; f++) host.Frame(f, []);
        Assert.Equal(ProgramStatus.Ended, host.Machine.Status);
        Assert.Equal(0, host.Machine.ExitCode);
        Assert.Null(host.Machine.WeekMachine);
        Assert.False(host.Ending.Presenting);
        AssertLikeOriginal(host, Teardown, week: 1);
    }

    /// <summary><see cref="AssertLikeOriginal"/> up to the teardown 1777:018C of main after the end of the game.</summary>
    private const int Teardown = -1;

    /// <summary>
    /// R7E-2 (#201): in a frame in which a step ends and the next one starts, the scenes get the rest of the week report
    /// (call 3) and of the incidents (call 9) before the first event of the next step, as the original returns from its wait
    /// loop 21FD:0004 before the next call starts. Weeks 0 and 1 with the robot switch on run through all ported steps; they
    /// have the change from call 3 to call 8 in one frame (finding of review fix-stationen-host) and the end of call 9. Events
    /// of a step before the report in the same frame (the contest, call 30, of the week before) are played before it.
    /// Counter-check (local): the planting output without <c>PlayEarlier</c> → red (report after field events in that frame).
    /// </summary>
    [GameDataFact]
    public void Report_and_incidents_are_played_before_the_next_step_in_the_same_frame()
    {
        var log = new List<(long Frame, object Event)>();
        long current = -1;
        var host = PlayToHalt(skipRobotPhase: true, (h, f) =>
        {
            current = f;
            h.Played ??= e => log.Add((current, e));
        });
        Assert.True(host.GameOver);
        static bool Earlier(object e) => e is Traders.Core.Week.WeekEvent or Traders.Core.Events.IncidentEvent;
        int mixed = 0;
        foreach (var frame in log.GroupBy(x => x.Frame))
        {
            var events = frame.Select(x => x.Event).ToList();
            // events of a step before the report in the same frame (the contest, call 30, of the week before) come first
            int firstEarlier = events.FindIndex(Earlier);
            if (firstEarlier < 0) continue;
            int firstOther = events.FindIndex(firstEarlier, e => !Earlier(e));
            int lastEarlier = events.FindLastIndex(Earlier);
            if (firstOther < 0) continue;
            mixed++;
            Assert.True(lastEarlier < firstOther,
                $"frame {frame.Key}: {events[lastEarlier]} played after {events[firstOther]} of the next step");
        }
        Assert.True(mixed >= 1, "no frame with the end of call 3 or 9 and the start of the next step");
        Assert.Contains(log, x => x.Event is Traders.Core.Trade.TradeEvent);
        Assert.Contains(log, x => x.Event is Traders.Core.Field.FieldEvent);
    }

    /// <summary>
    /// Start, main menu with RETURN after the second pass, the week loop until the host halts; common checks. With the robot
    /// switch the target DS:08F4 is set to 0 after the end check of week 0 (<see cref="EndTarget"/>), so the game ends at the
    /// end check of week 1.
    /// </summary>
    private static ProgramHost PlayToHalt(bool skipRobotPhase, Action<ProgramHost, long>? beforeFrame = null)
    {
        var start = ProgramStartDifferentialTests.ReferenceStart;
        var (clock, _, _) = ProgramStartDifferentialTests.OriginalInputs(start, ProgramStartDifferentialTests.NoExtraKeys);
        var host = new ProgramHost(Dir, new RecordedClock([clock]), new NoSaveFile(), skipRobotPhase);
        bool targetSet = false;
        void Frame(long frame, IReadOnlyCollection<byte> held)
        {
            beforeFrame?.Invoke(host, frame);
            host.Frame(frame, held);
            // the end check of week 0 has run: nothing reads the target before the week report of week 1
            if (skipRobotPhase && !targetSet && host.Machine.WeekMachine is { Week: 0, NextStep: > WeekSteps.EndCall })
            {
                host.Core.Set(DgroupLayout.Target, EndTarget);
                targetSet = true;
            }
        }
        long f = 0;
        for (; f < 2000 && Passes(host) < 2; f++) Frame(f, []);
        for (long until = f + 6; f < until; f++) Frame(f, [MenuKeys.Enter]);
        for (long until = f + 300_000; f < until && host.Halt is null && !host.GameOver; f++) Frame(f, []);

        Assert.Equal(ProgramStatus.InWeek, host.Machine.Status);
        Assert.Equal(!skipRobotPhase, host.Machine.IsBlocked);
        Assert.Equal(skipRobotPhase, host.GameOver);
        Assert.Equal(skipRobotPhase, targetSet);
        Assert.Equal(1, host.Core.U8(ProgramLoop.GameRunning));
        Assert.Equal(new byte[] { 1, 1, 1, 1 }, Enumerable.Range(0, 4).Select(i => host.Core.U8(DgroupLayout.IsComputer + i)));
        Assert.NotSame(host.Core.Bytes, host.Layers.Ds);
        Assert.False(host.Field.Presenting);
        Assert.False(host.Report.Presenting);
        Assert.False(host.Stations.Presenting);
        Assert.False(host.Harvest.Presenting);
        Assert.False(host.Trade.Presenting);
        Assert.False(host.Extra.Presenting);
        Assert.False(host.FatMike.Presenting);
        Assert.False(host.Purchase.Presenting);
        return host;
    }

    /// <summary>Target DS:08F4 from week 1 on in the runs with the robot switch: every wealth reaches it.</summary>
    private const uint EndTarget = 0;

    /// <summary>
    /// The original with the same clock and the same key at the same menu pass, to the call site of <paramref name="call"/> in
    /// <paramref name="week"/> (every call 18 before it jumped over, and after the end check of week 0 the target set to
    /// <see cref="EndTarget"/> as in <see cref="PlayToHalt"/>), or with <see cref="Teardown"/> through the end of the game in
    /// that week (a key once the original waits in 27C6:00D2) to the teardown 1777:018C: data segment and picture shown equal
    /// the host's.
    /// </summary>
    private static void AssertLikeOriginal(ProgramHost host, int call, int week)
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
        ushort loop = p.Segment(WeekLoop.Segment), call18 = WeekLoop.Step(WeekSteps.RobotCall).Site,
            call20 = WeekLoop.Step(WeekSteps.EndCall + 1).Site,
            target = call == Teardown ? ProgramLoop.TeardownSite : WeekLoop.Step(call).Site;
        if (call == Teardown)
            p.Machine.AddCodeHook(p.Segment(0x27C6), 0x00D2, m => m.ScheduleKey(m.Seconds + 0.1, MenuKeys.Enter, 6 / 70.0));
        void RunTo(ushort site, string what)
        {
            p.Machine.Run(80_000_000_000, m => m.Cpu.Cs == loop && m.Cpu.IP == site);
            Assert.True(p.Machine.Cpu.Cs == loop && p.Machine.Cpu.IP == site, $"original did not reach {what} (week {p.Week})");
        }
        for (int w = 0; ; w++)
        {
            RunTo(call18, "call 18");
            Assert.Equal(w, p.Week);
            if (call == WeekSteps.RobotCall && w == week) break;
            p.Machine.Cpu.IP = (ushort)(call18 + 5); // over the far call 1C68:000B (no arguments, nothing to pop)
            if (w == week)
            {
                RunTo(target, $"call {call}");
                break;
            }
            RunTo(call20, "call 20");
            if (w == 0)
            {
                p.Machine.Mem.Write16(p.Dgroup, DgroupLayout.Target, (ushort)EndTarget);
                p.Machine.Mem.Write16(p.Dgroup, DgroupLayout.Target + 2, (ushort)(EndTarget >> 16));
            }
        }
        Assert.Equal(call == Teardown ? week + 1 : week, p.Week); // 20B3:0071 counts the week of the end once more

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
            .Concat(Traders.Tests.Trade.TradeFrameHarness.Owned.Select(r => (r.Offset, r.Length)))
            .Concat(Traders.Tests.Extra.ExtraFrameHarness.Owned.Select(r => (r.Offset, r.Length)))
            .Concat(Traders.Tests.FatMike.FatMikeFrameHarness.Owned.Select(r => (r.Offset, r.Length)))
            .Concat(Traders.Tests.Purchase.PurchaseFrameHarness.Owned.Select(r => (r.Offset, r.Length)))
            .Concat(Traders.Tests.Ending.EndingFrameHarness.Owned.Select(r => (r.Offset, r.Length)))
            // segment words of the far pointers of 1DEF:000C into its stack frame: the host gives the game's frame relative
            // to its DS value (ExtraStep.GameFrame), the original's SS is relocated; the offsets are compared
            .Append((Traders.Core.Extra.ExtraPhase.GoodsPointer + 2, 2))
            .Append((Traders.Core.Extra.ExtraPhase.ClaimsPointer + 2, 2))
            .Append((Traders.Core.Extra.ExtraPhase.GroundPointer + 2, 2))
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
