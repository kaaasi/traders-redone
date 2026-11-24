using Traders.App.Sound;
using Traders.Core.Phases;
using Traders.Core.Sound;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.RefEmu.Hardware;
using Traders.RefEmu.Machine;
using Xunit.Abstractions;

namespace Traders.Tests.App;

/// <summary>
/// The host's sound output (<see cref="HostSound"/>: effect machine and music driver behind the core's commands, both driven
/// by the timer ticks) against the game run with the resident FXDRIVER (TRADERS.BAT, <c>runFxDriver</c>, lane partie part 3):
/// every sound command of T.EXE (int 64h at the driver's entry, 0000:06FE, 0724, 0750, the timer callback 27C6:041B) and every
/// timer interrupt (27C6:03E4, with the effect tick 0000:0732 and the driver's timer CS:1262 it makes) is replayed into the host
/// in the same order, the timer interrupt as <see cref="HostSound.Tick"/> with its number since the start. The OPL writes the
/// original makes between two such events (OplTrace of the AdLib) must equal the host's writes for that event, each stamped
/// with the tick of the event (the command's tick is the last timer tick before it). Covered: the intro (TRADINTR.CMF), the
/// menu music (218B:0094(1, 2) as 1777:015C calls it), the effects of week 0 (four computers) and the end music
/// (218B:0094(2, 3) of 20B3:0060, TRAD_END.CMF) for some seconds. The command order itself is the core's: compared with the
/// original in <see cref="HostSoundDifferentialTests"/> and <see cref="HostWholeGameFrameDifferentialTests"/>.
/// </summary>
public class HostSoundOplDifferentialTests(ITestOutputHelper output)
{
    private abstract record Event(int FirstWrite);
    private sealed record Command(int FirstWrite, SoundCommand Sound, string? Name) : Event(FirstWrite);
    private sealed record TimerTick(int FirstWrite, long Number) : Event(FirstWrite)
    {
        public bool Effects { get; set; }
        public bool Chained { get; set; }
    }

    private const ushort InputSegment = 0x27C6, TimerHandler = 0x03E4, DriverTimer = 0x1262;

    /// <summary>Seconds the end music is recorded after it is loaded.</summary>
    private const double EndMusicSeconds = 12;

    // [DIFFERENTIAL] 218B:0094, 21A7:0054, 0000:0732, 27C6:03E4
    [GameDataFact]
    public void Host_sound_writes_the_originals_opl_registers_for_intro_menu_music_a_week_of_effects_and_the_end_music()
    {
        var p = OriginalProgram.Start(TestGameData.DosDir, runFxDriver: true);
        var m = p.Machine;
        var events = new List<Event>();
        var writes = new List<(byte Register, byte Value)>();
        ushort entry = m.Mem.Read16(0, 0x64 * 4), driver = m.Mem.Read16(0, 0x64 * 4 + 2);
        ushort fx = p.Segment(SoundControl.EffectsSegment);
        // the writes of the effect machine (segment 0000) and of the driver; the AdLib detection of the initialisation is not
        // sound output
        var previous = m.AdLib.RegisterWritten;
        m.AdLib.RegisterWritten = (reg, value) =>
        {
            previous?.Invoke(reg, value);
            if (m.Cpu.Cs == fx || m.Cpu.Cs == driver) writes.Add(((byte)reg, value));
        };
        long ticks = 0;
        TimerTick? current = null;
        void Add(SoundCommand c, string? name = null) => events.Add(new Command(writes.Count, c, name));
        static ushort Arg(PcMachine mm, int index) => mm.Mem.Read16(mm.Cpu.Ss, (ushort)(mm.Cpu.Sp + 4 + 2 * index));
        m.AddCodeHook(driver, entry, mm =>
        {
            var c = mm.Cpu;
            Add(SoundCommand.Driver(c.Ax, c.Bx, c.Cx, c.Dx), c.Ax == SoundCommand.Load ? mm.Mem.ReadAsciiz(c.Bx, c.Cx) : null);
        });
        m.AddCodeHook(driver, DriverTimer, _ => current!.Chained = true);
        m.AddCodeHook(fx, 0x06FE, _ => Add(new SoundCommand(SoundCommandKind.EffectsReset)));
        m.AddCodeHook(fx, 0x0724, _ => Add(new SoundCommand(SoundCommandKind.EffectsStop)));
        m.AddCodeHook(fx, 0x0750, mm => Add(new SoundCommand(SoundCommandKind.Effect, Bx: Arg(mm, 0))));
        m.AddCodeHook(fx, SoundControl.EffectsTick, _ => current!.Effects = true);
        ushort input = p.Segment(InputSegment);
        m.AddCodeHook(input, 0x041B, mm => Add(new SoundCommand(SoundCommandKind.TickCallback,
            Bx: (ushort)(Arg(mm, 1) - mm.LoadSegment), Cx: Arg(mm, 0))));
        m.AddCodeHook(input, TimerHandler, _ =>
        {
            current = new TimerTick(writes.Count, ticks++);
            events.Add(current);
        });

        // the intro into the main menu, the menu music for some seconds (218B:0094(1, 2), the main loop's call 1777:015C, put in
        // from the saved registers as MusicGameDifferentialTests does), then RETURN: a new game with four computers
        m.RunFor(OriginalProgram.MainMenuTime + 1.01);
        var cpu = m.Cpu;
        var saved = (cpu.Cs, cpu.IP, cpu.Flags, cpu.Ax, cpu.Bx, cpu.Cx, cpu.Dx, cpu.Si, cpu.Di, cpu.Bp, cpu.Sp, cpu.Ds, cpu.Es, cpu.Ss);
        p.Call(0x218B, 0x0094, 1, 2);
        (cpu.Cs, cpu.IP, cpu.Flags, cpu.Ax, cpu.Bx, cpu.Cx, cpu.Dx, cpu.Si, cpu.Di, cpu.Bp, cpu.Sp, cpu.Ds, cpu.Es, cpu.Ss) = saved;
        m.RunFor(8);
        p.Press(Scancodes.Enter);
        Assert.True(p.RunToWeekStart(0, 5), "week 0 did not start");
        // the target 0: the end check of week 0 ends the game and 20B3:0060 starts the end music; the robot phase jumped over
        p.WriteWord(DgroupLayout.Target, 0);
        p.WriteWord(DgroupLayout.Target + 2, 0);
        ushort loop = p.Segment(WeekLoop.Segment), call18 = WeekLoop.Step(WeekSteps.RobotCall).Site;
        m.AddCodeHook(loop, call18, mm => mm.Cpu.IP = (ushort)(call18 + 5));
        double? endMusic = null;
        m.Run(400_000_000_000, mm =>
        {
            if (endMusic is null && events.Count > 0 && events[^1] is Command { Name: "TRAD_END.CMF" }) endMusic = mm.Seconds;
            return endMusic is { } t && mm.Seconds > t + EndMusicSeconds;
        });
        Assert.True(endMusic is not null, "the end music did not start");
        Assert.Equal(0, p.Week); // still in the end screens of week 0 (their key wait), 20B3:0071 not reached

        var commands = events.OfType<Command>().ToList();
        Assert.Equal(["TRADINTR.CMF", "TRADMENU.CMF", "TRAD_END.CMF"], commands.Where(c => c.Name is not null).Select(c => c.Name!));
        int effects = commands.Count(c => c.Sound.Kind == SoundCommandKind.Effect);
        Assert.True(effects >= 10, $"only {effects} effects");

        // the host replays the events in order
        string? name = null;
        var host = new HostSound(File.ReadAllBytes(TestGameData.Dos("T.EXE")), File.ReadAllBytes(TestGameData.Dos("FXDRIVER.EXE")),
            n => File.Exists(TestGameData.Dos(n)) ? File.ReadAllBytes(TestGameData.Dos(n)) : null, (_, _) => name);
        var written = new List<OplWriteStamp>();
        host.Written += w => written.Add(new OplWriteStamp(w.Tick, w.Register, w.Value));
        long last = 0;
        int compared = 0;
        for (int i = 0; i < events.Count; i++)
        {
            var e = events[i];
            int end = i + 1 < events.Count ? events[i + 1].FirstWrite : writes.Count;
            written.Clear();
            string what;
            if (e is Command c)
            {
                name = c.Name;
                host.Execute([c.Sound]);
                what = c.Sound.ToString();
            }
            else
            {
                var t = (TimerTick)e;
                Assert.True(t.Effects == host.EffectsTicking, $"tick {t.Number}: effect tick original {t.Effects}, host {host.EffectsTicking}");
                Assert.True(!t.Chained || host.Music!.TimerActive, $"tick {t.Number}: the original's driver ticks, the host's timer is off");
                host.Tick(t.Number, t.Chained);
                last = t.Number;
                what = $"tick {t.Number}";
            }
            Assert.All(written, w => Assert.Equal(last, w.Tick));
            var original = writes.Skip(e.FirstWrite).Take(end - e.FirstWrite).ToList();
            var port = written.Select(w => (w.Register, w.Value)).ToList();
            if (!original.SequenceEqual(port))
                Assert.Fail($"event {i} ({what}): original {Hex(original)} | host {Hex(port)}");
            compared += original.Count;
        }
        output.WriteLine($"events {events.Count}, ticks {ticks}, commands {commands.Count}, effects {effects}, OPL writes {compared}");
        Assert.Equal(writes.Count - events[0].FirstWrite, compared);
        Assert.True(compared > 10_000, $"only {compared} OPL writes");
    }

    private readonly record struct OplWriteStamp(long Tick, byte Register, byte Value);

    private static string Hex(IEnumerable<(byte Register, byte Value)> w) =>
        string.Join(" ", w.Take(24).Select(x => $"{x.Register:X2}={x.Value:X2}"));
}
