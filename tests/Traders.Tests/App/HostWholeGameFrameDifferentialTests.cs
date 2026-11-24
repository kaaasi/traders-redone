using Traders.App;
using Traders.Core;
using Traders.Core.Ending;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.Sound;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.Phases;
using Traders.Tests.Startup;
using Xunit.Abstractions;

namespace Traders.Tests.App;

/// <summary>
/// Whole-game probe of lane partie (R9 parts 3 and 4): the host (robot switch on, F3 = A of R7) plays a game from the program
/// start through the main menu (four computers, or player 1 human and three computers) week by week to its end and on, frame by frame against the
/// RefEmu with the same clock and keys (as <see cref="HostExtraPhaseFrameDifferentialTests"/>: every host frame is one retrace of
/// the original; the original jumps over its call 18 where the host's stand-in skips it). At every retrace the week, the random
/// state DS:10D2, the data segment (outside <see cref="ProgramHostTests.Ignored"/>), the picture shown and the sound commands
/// issued so far (<see cref="OriginalSoundRecorder"/>: int 64h, effect machine, timer callback; the key clicks of the scenes
/// included) are equal. The game ends at the end check (call 19) by the target or by F10; the end screens run frame by frame
/// up to their key wait 27C6:00D2 (no key pressed before it; there the host polls its first time, data segment, picture and
/// sound compared once more), RETURN ends them, the week loop returns AL = 1. With <c>--original-ende</c> main goes to
/// its teardown 1777:018C like the original (data segment, picture and every sound command up to the program's exit equal);
/// with the default the host goes back to the main menu where the original tears down (host deviation R9 F3 = B; the sound
/// commands of the teardown are the original's, the main menu follows).
/// Expected values come only from the RefEmu.
/// </summary>
public class HostWholeGameFrameDifferentialTests
{
    private static string Dir => TestGameData.DosDir;

    private const ushort RetraceSegment = 0x0D73, RetraceEntry = 0x00D3, RetraceEnd = 0x00F7;

    /// <summary>The any-key wait 27C6:00D2 of the end screens: it polls the make counter DS:115D without a retrace wait.</summary>
    private const ushort WaitKeySegment = 0x27C6, WaitKeyEntry = 0x00D2;

    /// <summary>Key presses in the main menu by menu pass: player 1 human, play, confirm controls.</summary>
    private static readonly (int Passes, byte Key)[] HumanMenuPlan = [(3, MenuKeys.F1), (6, MenuKeys.Enter), (9, MenuKeys.Enter)];

    /// <summary>RETURN after the second pass: the default game with four computers (as in <see cref="ProgramHostTests"/>).</summary>
    private static readonly (int Passes, byte Key)[] ComputerMenuPlan = [(2, MenuKeys.Enter)];

    private const int MenuHold = 6;

    /// <summary>What the human presses in the week (control 4: arrows, Ins), one key every 35 frames, held 6 frames.</summary>
    private static readonly byte[] HumanCycle = [0x4D, 0x52, 0x4D, 0x52, 0x50, 0x52, 0x4B, 0x52, 0x50, 0x52, 0x48, 0x52];

    private const int KeyEvery = 35, KeyHold = 6;

    private const int MaxFrames = 200_000;

    private readonly ITestOutputHelper _out;

    public HostWholeGameFrameDifferentialTests(ITestOutputHelper output) => _out = output;

    private enum EndBy
    {
        /// <summary>The target DS:08F4 set to 0 on both sides after the end check of the week before the end week.</summary>
        Target,

        /// <summary>F10 held in the original from the harvest (call 16) of the end week on (key table DS:3D8B).</summary>
        F10,
    }

    private sealed record Outcome(ProgramHost Host, OriginalProgram Original, Dictionary<(int Week, int Call), int> Frames,
        int Clicks, int Commands);

    /// <summary>
    /// Probe 1777:000C, 1777:01F8, 20B3:0005, 21A7:0054, 218B:0094 – the whole game to the
    /// target with four computers: weeks 0 and 1 with all
    /// their ported calls, week 2 (with the contest) up to its end check, which ends the game (target 0 since the end check of
    /// week 1); end screens, the end music 218B:0094(2, 3), the key, the teardown 1777:018C and the program's exit with
    /// <c>--original-ende</c>: equal frame by frame, sound commands included.
    /// Counter-checks (local, review): the rest of call 19 left out, DS:4B9A not incremented at the end, a click with DS:10A7 = 0,
    /// the host holding at the end → red.
    /// </summary>
    [GameDataFact]
    public void Whole_game_to_the_target_in_week_2_runs_frame_by_frame_with_its_sounds_to_the_teardown()
    {
        var r = Play(EndBy.Target, endWeek: 2, returnToMenu: false, human: false);
        var host = r.Host;
        Assert.Equal(ProgramStatus.Ended, host.Machine.Status);
        Assert.Equal(0, host.Machine.ExitCode);
        Assert.True(r.Original.Machine.Exited, "original did not exit");
        Assert.Equal(3, r.Original.Week); // 20B3:0071 counts the week of the end once more
        Assert.Equal(3, host.Core.S16(DgroupLayout.Week));
        Assert.Equal(3, host.RobotSkip!.Skipped);
        AssertWeeks(r, lastWeek: 2);
        // the end music (piece 2: TRAD_END.CMF) after the effects of the weeks; the teardown's commands at the very end
        var commands = ((StubSoundHost)host.SoundHost).Executed;
        Assert.Contains(commands, c => c is { Kind: SoundCommandKind.Driver, Function: SoundCommand.Load });
        Assert.Equal(new SoundCommand(SoundCommandKind.EffectsStop), commands[^1]);
    }

    /// <summary>
    /// Probe 20B3:0005, 21A7:0054 – four computers, the game ended with F10 at the end
    /// check of week 0 (no player at the target), with the
    /// host's default: frame by frame through the end screens to the return of the week loop, the sound commands up to the
    /// teardown's equal; then the host's main menu instead of the original's teardown (R9 F3 = B).
    /// </summary>
    [GameDataFact]
    public void Game_ended_with_F10_runs_frame_by_frame_to_its_end_and_the_host_goes_back_to_the_main_menu()
    {
        var r = Play(EndBy.F10, endWeek: 0, returnToMenu: true, human: false);
        var host = r.Host;
        Assert.Equal(1, host.MenuReturn!.Returns);
        Assert.All(Enumerable.Range(0, 4), pl => Assert.True(host.Core.U32(DgroupLayout.CurrentWealth + 4 * pl) < host.Core.U32(DgroupLayout.Target)));
        Assert.Equal(1, r.Original.Week);
        AssertWeeks(r, lastWeek: 0);
        // the new main menu (the menu of the first game stays in Menu.Menu until the step starts anew)
        var before = host.Menu.Menu;
        long f = host.LastFrame + 1;
        for (long until = f + 2000; f < until && !(host.Machine.Status == ProgramStatus.Waiting && host.Menu.Menu != before); f++)
            host.Frame(f, []);
        Assert.NotNull(host.Menu.Menu);
        Assert.NotSame(before, host.Menu.Menu);
        Assert.Equal(ProgramStatus.Waiting, host.Machine.Status);
        Assert.Equal(ProgramLoop.MainMenu, host.Machine.NextStep);
    }

    /// <summary>
    /// Probe with player 1 human: as the first probe; the human's key clicks of the
    /// scenes' typewriter texts are among the sound commands compared at every retrace.
    /// </summary>
    [GameDataFact]
    public void Whole_game_with_a_human_to_the_target_in_week_2_runs_frame_by_frame_with_its_sounds_to_the_teardown()
    {
        var r = Play(EndBy.Target, endWeek: 2, returnToMenu: false, human: true);
        Assert.Equal(ProgramStatus.Ended, r.Host.Machine.Status);
        AssertWeeks(r, lastWeek: 2);
        Assert.True(r.Clicks > 0, "no key click of the scenes");
    }

    private void AssertWeeks(Outcome r, int lastWeek)
    {
        int Frames(int week, int call) => r.Frames.GetValueOrDefault((week, call));
        _out.WriteLine($"frames by week and call {string.Join(", ", r.Frames.OrderBy(k => k.Key))}; clicks {r.Clicks}; commands {r.Commands}");
        for (int w = 0; w <= lastWeek; w++)
        {
            Assert.True(Frames(w, WeekSteps.PlantingCall) > 100, $"week {w}: too few planting frames");
            Assert.True(Frames(w, WeekSteps.StationsCall) > 100, $"week {w}: too few frames of the stations phase");
            Assert.True(Frames(w, WeekSteps.HarvestCall) > 100, $"week {w}: too few frames of the harvest");
        }
        for (int w = 0; w < lastWeek; w++)
            Assert.True(Frames(w, WeekSteps.TradeCall) > 100, $"week {w}: too few frames of the trade");
        Assert.True(Frames(lastWeek, EndCheckStep.Call) > 1000, "too few frames of the end screens");
        Assert.Equal(0, Frames(lastWeek, WeekSteps.TradeCall)); // the week of the end has no trade
    }

    private Outcome Play(EndBy endBy, int endWeek, bool returnToMenu, bool human)
    {
        var start = ProgramStartDifferentialTests.ReferenceStart;
        var (clock, _, _) = ProgramStartDifferentialTests.OriginalInputs(start, ProgramStartDifferentialTests.NoExtraKeys);
        var p = OriginalProgram.Start(Dir, ProgramStartDifferentialTests.Options(start));
        var sound = new OriginalSoundRecorder(p);
        var plan = human ? HumanMenuPlan : ComputerMenuPlan;
        int emuPasses = 0;
        ushort menu = p.Segment(0x192D);
        foreach (ushort site in new ushort[] { 0x0345, 0x1AAD })
            p.Machine.AddCodeHook(menu, site, m =>
            {
                emuPasses++;
                foreach (var (n, key) in plan)
                    if (n == emuPasses) m.ScheduleKey(m.Seconds + 0.001, key, MenuHold / 70.0);
            });
        // bytes the loader and the C start-up set before main: host values (as in ProgramHostTests)
        new RefEmuProgramOracle(p).RunToCallSite(ProgramLoop.Step(1));
        var image = ProgramSteps.InitialState(File.ReadAllBytes(TestGameData.Dos("T.EXE"))).Bytes;
        var atMain = p.DataSegment();
        var compared = ComparedRanges(Enumerable.Range(0, DgroupLayout.Size).Where(i => image[i] != atMain[i]).ToHashSet());

        ushort loop = p.Segment(WeekLoop.Segment), call3 = WeekLoop.Step(WeekSteps.ReportCall).Site,
            call18 = WeekLoop.Step(WeekSteps.RobotCall).Site;
        ushort main = p.Segment(ProgramLoop.Segment);
        p.Machine.Run(40_000_000_000, m => m.Cpu.Cs == loop && m.Cpu.IP == call3);
        Assert.True(p.Machine.Cpu.Cs == loop && p.Machine.Cpu.IP == call3 && p.Week == 0, "original did not reach call 3 of week 0");

        var stub = new StubSoundHost();
        var host = new ProgramHost(Dir, new RecordedClock([clock]), new NoSaveFile(), skipRobotPhase: true, returnToMenu, stub);
        var holds = new List<(long From, long To, byte Key)>();
        int planned = 0;
        long f = 0;
        for (; f < 5000; f++)
        {
            int passes = host.Menu.Menu?.RandomCalls.Count(c => c.Argument == 1) ?? 0;
            for (; planned < plan.Length && host.Machine.NextStep == ProgramLoop.MainMenu && passes >= plan[planned].Passes; planned++)
                holds.Add((f, f + MenuHold, plan[planned].Key));
            host.Frame(f, holds.Where(h => f >= h.From && f < h.To).Select(h => h.Key).ToList());
            if (host.Machine.WeekMachine is { NextStep: WeekSteps.ReportCall, Status: PhaseStatus.Waiting }) break;
        }
        Assert.True(host.Machine.WeekMachine is { NextStep: WeekSteps.ReportCall, Status: PhaseStatus.Waiting, Week: 0 }, "host did not reach the week report");
        Assert.Equal(human ? 0 : 1, host.Core.U8(DgroupLayout.IsComputer));
        int checkedCommands = CompareSound(sound, stub, 0, "week 0, call 3");

        ushort retrace = p.Segment(RetraceSegment), waitKey = p.Segment(WaitKeySegment);
        var m = p.Machine;
        var frames = new Dictionary<(int Week, int Call), int>();
        byte? held = null;
        bool targetSet = false, f10Down = false, f10Up = false, ended = false, keyWait = false;
        int lastWeek = 0, lastStep = WeekSteps.ReportCall;
        for (int frame = 0; ; frame++)
        {
            Assert.True(frame < MaxFrames, $"the game did not end within {MaxFrames} frames");
            bool hostDone = host.Machine.Status == ProgramStatus.Ended || (host.MenuReturn?.Returns ?? 0) > 0;
            m.Run(1);
            m.Run(8_000_000_000, mm => (mm.Cpu.Cs == retrace && mm.Cpu.IP == RetraceEntry) || (mm.Cpu.Cs == loop && mm.Cpu.IP == call18)
                || (mm.Cpu.Cs == main && mm.Cpu.IP == ProgramLoop.TeardownSite) || (mm.Cpu.Cs == waitKey && mm.Cpu.IP == WaitKeyEntry));
            Assert.False(m.Exited, "original exited");
            if (m.Cpu.Cs == loop && m.Cpu.IP == call18)
            {
                m.Cpu.IP = (ushort)(call18 + 5); // over the far call 1C68:000B as the host's stand-in skips it
                frame--;
                continue;
            }
            string where = $"frame {frame} (host frame {f}, week {lastWeek}, call {lastStep})";
            if (m.Cpu.Cs == main && m.Cpu.IP == ProgramLoop.TeardownSite)
            {
                Assert.True(hostDone, $"{where}: the original goes to its teardown, the host does not");
                ended = true;
                break;
            }
            Assert.False(hostDone, $"{where}: the host's game has ended, the original waits a retrace");
            if (m.Cpu.Cs == waitKey && m.Cpu.IP == WaitKeyEntry)
            {
                // the end screens' key wait: the host polls in this frame (its first poll after the last retrace); data
                // segment, picture and sound as there, then a key (RETURN) ends the wait on both sides (the host counts the
                // make in its next frame, DS:115D), and the week loop returns AL = 1
                where = $"key wait of the end screens (frame {frame}, host frame {f})";
                Assert.False(keyWait, $"{where}: a second key wait");
                keyWait = true;
                Assert.True(host.Screens.Ending?.Current is EndingStep { Polls: 1 },
                    $"{where}: the host does not poll its first time ({(host.Screens.Ending?.Current as EndingStep)?.Polls} polls)");
                Assert.True(p.RandomState == host.Core.U32(DgroupLayout.RandomSeed), $"{where}: random state");
                CompareDataSegment(p, host, compared, where);
                HostPictures.Compare(p, host, where);
                checkedCommands = CompareSound(sound, stub, checkedCommands, where);
                // 27C6:00D2 clears DS:115D first: the make comes a little later (as in EndingFrameHarness)
                p.Press(MenuKeys.Enter, delay: 0.02, hold: 0.05);
                f++;
                host.Frame(f, [MenuKeys.Enter]);
                frame--;
                continue;
            }
            var week = host.Machine.WeekMachine;
            Assert.True(week is not null, $"{where}: the host left the week loop, the original waits a retrace");
            lastWeek = week.Week;
            lastStep = week.NextStep;
            where = $"frame {frame} (host frame {f}, week {lastWeek}, call {lastStep}, its frame {frames.GetValueOrDefault((lastWeek, lastStep))})";
            Assert.True(host.Halt is null, $"{where}: the host halted ({host.Halt})");
            Assert.True(lastWeek == p.Week, $"{where}: week of the original {p.Week}");
            Assert.True(p.RandomState == host.Core.U32(DgroupLayout.RandomSeed),
                $"{where}: random state original {p.RandomState:X8}, host {host.Core.U32(DgroupLayout.RandomSeed):X8}");
            CompareDataSegment(p, host, compared, where);
            if (host.Screens.Ending?.Current is EndingStep ending && lastStep == EndCheckStep.Call)
                where += $", end screens: retraces {ending.Retraces}, polls {ending.Polls}";
            if (lastStep is not (WeekSteps.ReportCall or WeekSteps.IncidentsCall)) HostPictures.Compare(p, host, where);
            else HostPictures.CompareShown(p, host, where);
            // the sound requests of the week report and the incidents go to the core after each of their frames (SoundAfterStep),
            // like their scenes possibly one frame later than in the original: compared at the next frame of another step
            if (lastStep is not (WeekSteps.ReportCall or WeekSteps.IncidentsCall))
                checkedCommands = CompareSound(sound, stub, checkedCommands, where);
            frames[(lastWeek, lastStep)] = frames.GetValueOrDefault((lastWeek, lastStep)) + 1;

            m.Run(4_000_000, mm => mm.Cpu.Cs == retrace && mm.Cpu.IP == RetraceEnd);
            Assert.True(m.Cpu.Cs == retrace && m.Cpu.IP == RetraceEnd, $"{where}: retrace wait did not end");
            // the end: the target after the end check of the week before (nothing reads it before the next week report), or F10
            // from the harvest of the end week until the end screens run
            if (endBy == EndBy.Target && !targetSet && lastWeek == endWeek - 1 && lastStep > EndCheckStep.Call)
            {
                m.Mem.Write16(p.Dgroup, DgroupLayout.Target, 0);
                m.Mem.Write16(p.Dgroup, DgroupLayout.Target + 2, 0);
                host.Core.Set(DgroupLayout.Target, 0u);
                targetSet = true;
            }
            if (endBy == EndBy.F10 && !f10Down && lastWeek == endWeek && lastStep == WeekSteps.HarvestCall)
            {
                Key(p, EndCheck.EndKey, where);
                f10Down = true;
            }
            if (f10Down && !f10Up && host.GameOver)
            {
                Key(p, (byte)(EndCheck.EndKey | 0x80), where);
                f10Up = true;
            }
            // the human: one key every KeyEvery frames, held KeyHold frames; none pressed while the end screens run up to their
            // key wait (as in EndingFrameHarness), the next one ends them
            bool endScreens = host.GameOver && host.Screens.Ending?.Current is EndingStep { Polls: 0 };
            if (frame % KeyEvery == 0 && !endScreens)
            {
                held = HumanCycle[frame / KeyEvery % HumanCycle.Length];
                Key(p, held.Value, where);
            }
            else if (held is { } k && frame % KeyEvery == KeyHold)
            {
                Key(p, (byte)(k | 0x80), where);
                held = null;
            }
            var keys = m.Mem.ReadBlock(p.Dgroup, DgroupLayout.KeyState, 0x80);
            f++;
            host.Frame(f, [.. Enumerable.Range(0, keys.Length).Where(sc => keys[sc] != 0).Select(sc => (byte)sc)]);
        }
        Assert.True(ended);
        Assert.True(keyWait, "the original did not wait for a key at the end screens");
        Assert.True(endBy != EndBy.Target || targetSet, "target not set");
        Assert.True(endBy != EndBy.F10 || f10Up, "F10 not pressed and released");
        Assert.True(host.GameOver || host.MenuReturn is { Returns: 1 });

        // the teardown 1777:018C: data segment and picture as there (the host has ended or gone back to the menu in its frame)
        string at = "teardown 1777:018C";
        if (!returnToMenu)
        {
            CompareDataSegment(p, host, compared, at);
            HostPictures.CompareShown(p, host, at);
        }
        // the sound part of the teardown (218B:0153, 21A7:0043) and the program's exit: every command equal
        m.Run(4_000_000_000, mm => mm.Exited);
        Assert.True(m.Exited, "original did not exit after its teardown");
        var original = sound.Commands().Select(c => OriginalSoundRecorder.Comparable(c, m.LoadSegment)).ToList();
        var port = stub.Executed.Select(c => OriginalSoundRecorder.Comparable(c, 0)).ToList();
        Assert.True(original.SequenceEqual(port), $"sound commands at the exit: original {original.Count}, host {port.Count}; " +
            $"original tail {string.Join(", ", original.TakeLast(6))} | host tail {string.Join(", ", port.TakeLast(6))}");
        return new Outcome(host, p, frames, host.Clicks.Count, original.Count);
    }

    /// <summary>The commands issued since <paramref name="done"/> are equal on both sides; returns the number compared.</summary>
    private static int CompareSound(OriginalSoundRecorder sound, StubSoundHost stub, int done, string where)
    {
        var original = sound.Commands();
        var port = stub.Executed;
        for (int i = done; i < Math.Max(original.Count, port.Count); i++)
        {
            if (i >= original.Count || i >= port.Count)
                Assert.Fail($"{where}: sound command {i}: original {string.Join(", ", original.Skip(i - 2).Take(8))} | host {string.Join(", ", port.Skip(i - 2).Take(8))}");
            var a = OriginalSoundRecorder.Comparable(original[i], LoadSegment(original[i], port[i]));
            var b = OriginalSoundRecorder.Comparable(port[i], 0);
            Assert.True(a == b, $"{where}: sound command {i}: original {a}, host {b}");
        }
        return original.Count;
    }

    /// <summary>The name's segment of function 00h is the original's DS (relocated) and the host's DGROUP value: compared relative.</summary>
    private static ushort LoadSegment(SoundCommand original, SoundCommand port) =>
        original is { Kind: SoundCommandKind.Driver, Function: SoundCommand.Load } ? (ushort)(original.Bx - port.Bx) : (ushort)0;

    /// <summary>Data segment ranges compared at every retrace: all but the loader's bytes and <see cref="ProgramHostTests.Ignored"/>.</summary>
    private static (int Offset, int Length)[] ComparedRanges(HashSet<int> loaded)
    {
        var skip = new bool[DgroupLayout.Size];
        foreach (int i in loaded) skip[i] = true;
        foreach (var (o, l) in ProgramHostTests.Ignored)
            for (int i = o; i < o + l && i < skip.Length; i++) skip[i] = true;
        var ranges = new List<(int, int)>();
        for (int i = 0; i < skip.Length;)
        {
            if (skip[i]) { i++; continue; }
            int a = i;
            while (i < skip.Length && !skip[i]) i++;
            ranges.Add((a, i - a));
        }
        return [.. ranges];
    }

    private static void CompareDataSegment(OriginalProgram p, ProgramHost host, (int Offset, int Length)[] compared, string where)
    {
        var original = p.Machine.Mem.ReadBlock(p.Dgroup, 0, DgroupLayout.Size);
        foreach (var (o, l) in compared)
        {
            var a = original.AsSpan(o, l);
            var b = host.Core.Bytes.AsSpan(o, l);
            int k = a.CommonPrefixLength(b);
            if (k >= l) continue;
            int from = Math.Max(0, o + k - 4), n = Math.Min(12, DgroupLayout.Size - from);
            Assert.Fail($"{where}: DS:{o + k:X4} original {a[k]:X2}, host {b[k]:X2} (from DS:{from:X4} original " +
                $"{Convert.ToHexString(original, from, n)}, host {Convert.ToHexString(host.Core.Bytes, from, n)})");
        }
    }

    /// <summary>
    /// A key event in the original at the end of its retrace wait (0D73:00F7), where the host takes the key table for its next
    /// frame: the keyboard interrupt runs before the game goes on, so both sides read the new state from the same frame on.
    /// </summary>
    private static void Key(OriginalProgram p, byte scancode, string where)
    {
        var m = p.Machine;
        ushort at = (ushort)(DgroupLayout.KeyState + (scancode & 0x7F));
        byte want = (byte)((scancode & 0x80) == 0 ? 1 : 0);
        m.ScheduleScancode(m.Seconds, scancode);
        m.Run(200_000, mm => (mm.Mem.Read8(p.Dgroup, at) != 0 ? 1 : 0) == want);
        Assert.True((m.Mem.Read8(p.Dgroup, at) != 0 ? 1 : 0) == want, $"{where}: key {scancode:X2} not taken by the keyboard handler");
    }

    private sealed class NoSaveFile : IMenuHost
    {
        public bool LoadGame(Dgroup state) => false;

        public bool SaveGame(Dgroup state) => false;

        public bool CalibrationFailed(int stick, int step) => false;
    }
}
