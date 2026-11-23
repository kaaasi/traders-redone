using Traders.App;
using Traders.Core;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.Sound;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.RefEmu.Machine;
using Traders.Tests.Phases;
using Traders.Tests.Startup;

namespace Traders.Tests.App;

/// <summary>
/// The sound commands of the program run (lane partie part 2) against the original: the host with the RefEmu's recording driver
/// (<see cref="StubSoundHost"/>: int 64h there, function 12h answers "nothing playing") and the RefEmu's default run give the
/// same commands in the same order from the program start through the main menu (RETURN after its second pass) to the first
/// call of the week loop: 218B:0002, the intro 0DFD:02FE, the sound calls of main and the menu's sound requests. The data
/// segment with the sound flags DS:10A3, 10A5, 10A9 is compared there too.
/// </summary>
public class HostSoundDifferentialTests
{
    private static string Dir => TestGameData.DosDir;

    /// <summary>Records int 64h (from the RefEmu's log) and the calls of the effect machine and of 27C6:041B, in order.</summary>
    private sealed class OriginalSound
    {
        private readonly OriginalProgram _p;
        private readonly List<(int LogIndex, SoundCommand Command)> _hooked = [];

        public OriginalSound(OriginalProgram p)
        {
            _p = p;
            var m = p.Machine;
            void Hook(ushort seg, ushort off, Func<PcMachine, SoundCommand> record) =>
                m.AddCodeHook(p.Segment(seg), off, mm => _hooked.Add((mm.Log.Events.Count, record(mm))));
            Hook(SoundControl.EffectsSegment, 0x06FE, _ => new SoundCommand(SoundCommandKind.EffectsReset));
            Hook(SoundControl.EffectsSegment, 0x0724, _ => new SoundCommand(SoundCommandKind.EffectsStop));
            Hook(SoundControl.EffectsSegment, 0x0750, mm => new SoundCommand(SoundCommandKind.Effect, Bx: Arg(mm, 0)));
            Hook(SoundControl.SpeakerSegment, 0x00CC, mm => new SoundCommand(SoundCommandKind.SpeakerEffect, Bx: Arg(mm, 0)));
            Hook(0x27C6, 0x041B, mm => new SoundCommand(SoundCommandKind.TickCallback,
                Bx: (ushort)(Arg(mm, 1) - mm.LoadSegment), Cx: Arg(mm, 0)));
        }

        private static ushort Arg(PcMachine m, int index) => m.Mem.Read16(m.Cpu.Ss, (ushort)(m.Cpu.Sp + 4 + 2 * index));

        /// <summary>The commands so far, int 64h with the registers the game sets on purpose.</summary>
        public List<SoundCommand> Commands()
        {
            var log = _p.Machine.Log.Events;
            var list = new List<SoundCommand>();
            int h = 0;
            for (int i = 0; i <= log.Count; i++)
            {
                while (h < _hooked.Count && _hooked[h].LogIndex == i) list.Add(_hooked[h++].Command);
                if (i == log.Count || log[i].Kind != EventKind.Sound) continue;
                var f = log[i].Text.Split(' ').Skip(2).Select(t => Convert.ToUInt16(t[3..], 16)).ToArray(); // AX= BX= CX= DX=
                list.Add(SoundCommand.Driver(f[0], f[1], f[2], f[3]));
            }
            return list;
        }
    }

    /// <summary>Host values out of the comparison: buffer addresses of function 10h, the name's segment of function 00h.</summary>
    private static SoundCommand Comparable(SoundCommand c, ushort nameSegment) => c switch
    {
        { Kind: SoundCommandKind.Driver, Function: SoundCommand.Buffer } => c with { Cx = 0, Dx = 0 },
        { Kind: SoundCommandKind.Driver, Function: SoundCommand.Load } => c with { Bx = (ushort)(c.Bx - nameSegment) },
        _ => c,
    };

    /// <summary>
    /// [DIFFERENTIAL] 218B:0002, 218B:0094, 218B:013A, 21A7:000D, 21A7:0054 – program start, sound calls of main and the menu's
    /// requests up to the first call of the week loop (1777:01FB): same commands, same data segment (output and host bytes
    /// excepted as in the lockstep tests; the sound flags DS:10A3, 10A5, 10A9 compared).
    /// Counter-check (local): the intro of 0DFD:02FE left out (<see cref="ProgramStartWithMusic"/> not bound) → red.
    /// </summary>
    [GameDataFact]
    public void Sound_commands_of_start_and_menu_are_the_originals()
    {
        var start = ProgramStartDifferentialTests.ReferenceStart;
        var (clock, _, _) = ProgramStartDifferentialTests.OriginalInputs(start, ProgramStartDifferentialTests.NoExtraKeys);

        var p = OriginalProgram.Start(Dir, ProgramStartDifferentialTests.Options(start));
        var recorder = new OriginalSound(p);
        int passes = 0;
        ushort menu = p.Segment(0x192D);
        foreach (ushort site in new ushort[] { 0x0345, 0x1AAD })
            p.Machine.AddCodeHook(menu, site, m =>
            {
                if (++passes == 2) m.ScheduleKey(m.Seconds + 0.001, MenuKeys.Enter, 6 / 70.0);
            });
        ushort loop = p.Segment(WeekLoop.Segment), call1 = WeekLoop.Step(1).Site;
        p.Machine.Run(20_000_000_000, m => m.Cpu.Cs == loop && m.Cpu.IP == call1);
        Assert.True(p.Machine.Cpu.Cs == loop && p.Machine.Cpu.IP == call1, "original did not reach the week loop");
        var original = recorder.Commands().Select(c => Comparable(c, p.Machine.LoadSegment)).ToList();

        var sound = new StubSoundHost();
        var host = new ProgramHost(Dir, new RecordedClock([clock]), new NoSaveFile(), sound: sound);
        long f = 0;
        for (; f < 2000 && Passes(host) < 2; f++) host.Frame(f, []);
        for (long until = f + 6; f < until; f++) host.Frame(f, [MenuKeys.Enter]);
        for (long until = f + 600; f < until && host.Machine.Status != ProgramStatus.InWeek; f++) host.Frame(f, []);
        Assert.Equal(ProgramStatus.InWeek, host.Machine.Status);
        var port = sound.Executed.Select(c => Comparable(c, 0)).ToList();

        Assert.Contains(original, c => c.Kind == SoundCommandKind.Effect);
        Assert.Equal(2, original.Count(c => c is { Kind: SoundCommandKind.Driver, Function: SoundCommand.Load })); // intro, menu
        Assert.True(original.SequenceEqual(port),
            $"original {original.Count}: {string.Join(", ", original.Skip(26))}\nport {port.Count}: {string.Join(", ", port.Skip(26))}");
        var ds = p.DataSegment();
        foreach (int at in new[] { SoundControl.DriverReady, SoundControl.MusicPlaying, SoundControl.CurrentPiece, SoundControl.SoundOn })
            Assert.True(ds[at] == host.Core.Bytes[at] && ds[at + 1] == host.Core.Bytes[at + 1], $"DS:{at:X4} differs");
    }

    /// <summary>
    /// [DIFFERENTIAL] 21A7:0054 – the sound requests of week 0 (report, planting, incidents, stations, harvest …) up to the
    /// robot phase (call 18, robot switch off), in order, with the menu's before them.
    /// </summary>
    [GameDataFact]
    public void Sound_commands_of_week_0_are_the_originals()
    {
        var start = ProgramStartDifferentialTests.ReferenceStart;
        var (clock, _, _) = ProgramStartDifferentialTests.OriginalInputs(start, ProgramStartDifferentialTests.NoExtraKeys);

        var p = OriginalProgram.Start(Dir, ProgramStartDifferentialTests.Options(start));
        var recorder = new OriginalSound(p);
        int passes = 0;
        ushort menu = p.Segment(0x192D);
        foreach (ushort site in new ushort[] { 0x0345, 0x1AAD })
            p.Machine.AddCodeHook(menu, site, m =>
            {
                if (++passes == 2) m.ScheduleKey(m.Seconds + 0.001, MenuKeys.Enter, 6 / 70.0);
            });
        ushort loop = p.Segment(WeekLoop.Segment), call18 = WeekLoop.Step(WeekSteps.RobotCall).Site;
        p.Machine.Run(80_000_000_000, m => m.Cpu.Cs == loop && m.Cpu.IP == call18);
        Assert.True(p.Machine.Cpu.Cs == loop && p.Machine.Cpu.IP == call18, "original did not reach call 18");
        var original = recorder.Commands().Select(c => Comparable(c, p.Machine.LoadSegment)).ToList();

        var sound = new StubSoundHost();
        var host = new ProgramHost(Dir, new RecordedClock([clock]), new NoSaveFile(), skipRobotPhase: false, sound: sound);
        long f = 0;
        for (; f < 2000 && Passes(host) < 2; f++) host.Frame(f, []);
        for (long until = f + 6; f < until; f++) host.Frame(f, [MenuKeys.Enter]);
        for (long until = f + 300_000; f < until && host.Halt is null; f++) host.Frame(f, []);
        Assert.Contains("call 18 ", host.Halt);
        var port = sound.Executed.Select(c => Comparable(c, 0)).ToList();

        int first = Enumerable.Range(0, Math.Min(original.Count, port.Count)).FirstOrDefault(i => original[i] != port[i], -1);
        Assert.True(first < 0 && original.Count == port.Count,
            $"first difference at {first} of {original.Count}/{port.Count}: original {string.Join(", ", original.Skip(Math.Max(0, first - 3)).Take(12))}\n" +
            $"port {string.Join(", ", port.Skip(Math.Max(0, first - 3)).Take(12))}");
    }

    private static int Passes(ProgramHost host) => host.Menu.Menu?.RandomCalls.Count(c => c.Argument == 1) ?? 0;

    private sealed class NoSaveFile : IMenuHost
    {
        public bool LoadGame(Dgroup state) => false;

        public bool SaveGame(Dgroup state) => false;

        public bool CalibrationFailed(int stick, int step) => false;
    }
}
