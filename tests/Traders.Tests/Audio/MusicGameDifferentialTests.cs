using Traders.Assets;
using Traders.Audio.Music;
using Traders.RefEmu;
using Traders.RefEmu.Machine;

namespace Traders.Tests.Audio;

/// <summary>
/// <see cref="FxMusicDriver"/> against the game run with the resident FXDRIVER (TRADERS.BAT, <c>runFxDriver</c>): every
/// int 64h call of T.EXE (hook at the driver's entry CS:195D) and every call of the driver's timer handler from the
/// game's timer chain (hook at CS:1262) is replayed into the port in the same order; the OPL writes the driver code makes
/// between two events must equal the port's writes for that event (comparison per driver command and tick, R9PL-2; the
/// writes of the effect machine in segment 0000 are not part of the music and not counted).
/// </summary>
public class MusicGameDifferentialTests
{
    private abstract record Event(int FirstWrite);
    private sealed record DriverCall(int FirstWrite, ushort Ax, ushort Bx, ushort Cx, ushort Dx, string? Name) : Event(FirstWrite);
    private sealed record TimerTick(int FirstWrite) : Event(FirstWrite);

    private sealed class Recorder
    {
        public readonly List<Event> Events = [];
        public readonly List<(byte, byte)> Writes = [];
        public readonly OplTrace Trace;
        public int Ticks;

        public Recorder(OriginalProgram p)
        {
            var m = p.Machine;
            ushort entry = m.Mem.Read16(0, 0x64 * 4), code = m.Mem.Read16(0, 0x64 * 4 + 2);
            Trace = new OplTrace(m);
            var previous = m.AdLib.RegisterWritten;
            m.AdLib.RegisterWritten = (reg, value) =>
            {
                previous?.Invoke(reg, value);
                if (m.Cpu.Cs == code) Writes.Add(((byte)reg, value));
            };
            m.AddCodeHook(code, entry, mm =>
            {
                var c = mm.Cpu;
                string? name = c.Ax == 0 ? mm.Mem.ReadAsciiz(c.Bx, c.Cx) : null;
                Events.Add(new DriverCall(Writes.Count, c.Ax, c.Bx, c.Cx, c.Dx, name));
            });
            m.AddCodeHook(code, 0x1262, _ =>
            {
                Events.Add(new TimerTick(Writes.Count));
                Ticks++;
            });
        }
    }

    /// <summary>Replays the recorded events into a fresh port and compares the writes of every event.</summary>
    private static void Replay(Recorder r, int from = 0)
    {
        var port = new FxMusicDriver(FxDriverImage.Load(TestGameData.Dos("FXDRIVER.EXE")),
            name => File.Exists(TestGameData.Dos(name)) ? File.ReadAllBytes(TestGameData.Dos(name)) : null);
        var written = new List<(byte, byte)>();
        port.Written += (reg, v) => written.Add((reg, v));
        port.Install();
        int ticks = 0;
        for (int i = 0; i < r.Events.Count; i++)
        {
            var e = r.Events[i];
            int end = i + 1 < r.Events.Count ? r.Events[i + 1].FirstWrite : r.Writes.Count;
            written.Clear();
            string what;
            if (e is DriverCall c)
            {
                port.Call(c.Ax, c.Bx, c.Cx, c.Dx, c.Name);
                what = $"int 64h {c.Ax:X2}h";
            }
            else
            {
                Assert.True(port.TimerActive, $"Takt {ticks}: Original ruft den Takt, Port hat keinen");
                port.Tick();
                what = $"Takt {ticks++}";
            }
            var original = r.Writes.Skip(e.FirstWrite).Take(end - e.FirstWrite).ToList();
            if (!original.SequenceEqual(written))
                Assert.Fail($"Ereignis {i} ({what}): Original {MusicDriverHarness.Hex(original.Take(30))} | Port {MusicDriverHarness.Hex(written.Take(30))}");
        }
    }

    // [DIFFERENTIAL-FX] 3B82, 3657, 4150
    [GameDataFact]
    public void Boot_and_a_minute_of_intro_music_write_like_the_driver_in_the_game()
    {
        // The intro (0DFD:02FE, piece 0, flag 0) keeps playing in the main menu; the game starts no other piece there.
        var p = OriginalProgram.Start(TestGameData.DosDir, runFxDriver: true);
        var r = new Recorder(p);
        p.Machine.RunFor(OriginalProgram.MainMenuTime + 62);
        Assert.Equal(["TRADINTR.CMF"], r.Events.OfType<DriverCall>().Where(c => c.Ax == 0).Select(c => c.Name));
        int start = r.Events.FindLastIndex(e => e is DriverCall { Ax: 0x01 });
        int ticks = r.Events.Skip(start).Count(e => e is TimerTick);
        Assert.True(ticks >= 1093, $"nur {ticks} Takte Vorspannmusik");
        Replay(r);
    }

    // [DIFFERENTIAL-FX] 3BED, 40B4, 4236
    [GameDataTheory]
    [InlineData(1, 2, "TRADMENU.CMF")] // menu: 1777:015C
    [InlineData(2, 3, "TRAD_END.CMF")] // end: 20B3:0060
    public void Piece_started_by_the_game_routine_writes_like_the_driver_for_a_minute(int piece, int flag, string file)
    {
        var p = OriginalProgram.Start(TestGameData.DosDir, runFxDriver: true);
        var r = new Recorder(p);
        p.Machine.RunFor(OriginalProgram.MainMenuTime + 1);
        // 218B:0094(piece, flag) as its callers do (stop of the intro, load, repeat, fade, start); the menu loop
        // continues afterwards from the saved registers.
        var cpu = p.Machine.Cpu;
        var saved = (cpu.Cs, cpu.IP, cpu.Flags, cpu.Ax, cpu.Bx, cpu.Cx, cpu.Dx, cpu.Si, cpu.Di, cpu.Bp, cpu.Sp, cpu.Ds, cpu.Es, cpu.Ss);
        p.Call(0x218B, 0x0094, (ushort)piece, (ushort)flag);
        (cpu.Cs, cpu.IP, cpu.Flags, cpu.Ax, cpu.Bx, cpu.Cx, cpu.Dx, cpu.Si, cpu.Di, cpu.Bp, cpu.Sp, cpu.Ds, cpu.Es, cpu.Ss) = saved;
        int start = r.Ticks;
        p.Machine.RunFor(61);
        Assert.Equal(file, r.Events.OfType<DriverCall>().Last(c => c.Ax == 0).Name);
        // the driver's timer handler runs on every fourth tick of the game's timer (PIT divisor 4000h): 18,2 Hz
        Assert.InRange(r.Ticks - start, 1108, 1113);
        Replay(r);
    }
}
