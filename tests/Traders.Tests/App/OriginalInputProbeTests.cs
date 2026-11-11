using Traders.RefEmu;
using Traders.RefEmu.Hardware;

namespace Traders.Tests.App;

/// <summary>
/// RefEmu probes for the input and timer layer of the original (segment 27C6, findings
/// docs/routines/27C6_0043_eingabe_zeitgeber.md): which handlers are installed, how fast the tick counters run and
/// how scancodes reach the key-state table. Expected values come from the disassembly reading in the finding, not
/// from a port; the port of the host side follows in Traders.App (part 2 of the track).
/// </summary>
public class OriginalInputProbeTests(OriginalInputProbeTests.MenuFixture menu) : IClassFixture<OriginalInputProbeTests.MenuFixture>
{
    /// <summary>One boot to the main menu shared by the probes (each probe only looks at changes it causes itself).</summary>
    public sealed class MenuFixture
    {
        private OriginalProgram? _program;
        public OriginalProgram Program => _program ??= OriginalProgram.BootToMainMenu(TestGameData.DosDir);
    }

    private const ushort InputSegment = 0x27C6;
    private const ushort KeyboardHandler = 0x0043, TimerHandler = 0x03E4;
    private const ushort KeyTable = 0x3D47, LastMake = 0x1158, MakeCount = 0x115D;
    private const ushort TickA = 0x117E, TickB = 0x1180;

    /// <summary>PIT channel 0 divisor written by 27C6:037A (out 40h: 00h, 40h).</summary>
    private const int TimerDivisor = 0x4000;

    private OriginalProgram Menu() => menu.Program;

    private static (ushort Seg, ushort Off) Vector(OriginalProgram p, int n) =>
        (p.Machine.Mem.Read16(0, (ushort)(n * 4 + 2)), p.Machine.Mem.Read16(0, (ushort)(n * 4)));

    [GameDataFact]
    public void Main_menu_runs_with_own_keyboard_and_timer_handlers_and_a_faster_timer()
    {
        var p = Menu();
        Assert.Equal((p.Segment(InputSegment), KeyboardHandler), Vector(p, 9));
        Assert.Equal((p.Segment(InputSegment), TimerHandler), Vector(p, 8));
        Assert.Equal(TimerDivisor, p.Machine.Pit.Channel0Period);
    }

    [GameDataFact]
    public void Tick_counters_run_at_the_programmed_rate_and_the_bios_clock_gets_every_fourth_tick()
    {
        var p = Menu();
        ushort a0 = p.ReadWord(TickA), b0 = p.ReadWord(TickB), bios0 = p.Machine.Mem.Read16(0x40, 0x6C);
        const double seconds = 10;
        p.Machine.RunFor(seconds);
        int da = (ushort)(p.ReadWord(TickA) - a0), db = (ushort)(p.ReadWord(TickB) - b0);
        int dbios = (ushort)(p.Machine.Mem.Read16(0x40, 0x6C) - bios0);

        double expected = seconds * Pit.ClockHz / TimerDivisor; // 728.3 ticks
        Assert.InRange(da, (int)expected - 1, (int)expected + 1);
        Assert.Equal(da, db);
        // The handler chains to the previous int 8 only when the incremented 1180h is a multiple of four.
        Assert.InRange(dbios, da / 4 - 1, da / 4 + 1);
    }

    [GameDataFact]
    public void Make_sets_and_break_clears_the_key_state_byte_and_only_makes_are_counted()
    {
        var p = Menu();
        var m = p.Machine;
        byte count0 = p.ReadByte(MakeCount);

        m.ScheduleScancode(m.Seconds + 0.02, Scancodes.LeftShift);
        m.RunFor(0.1);
        Assert.Equal(1, p.ReadByte((ushort)(KeyTable + Scancodes.LeftShift)));
        Assert.Equal(Scancodes.LeftShift, p.ReadByte(LastMake));
        Assert.Equal((byte)(count0 + 1), p.ReadByte(MakeCount));

        m.ScheduleScancode(m.Seconds + 0.02, (byte)(Scancodes.LeftShift | 0x80));
        m.RunFor(0.1);
        Assert.Equal(0, p.ReadByte((ushort)(KeyTable + Scancodes.LeftShift)));
        Assert.Equal(Scancodes.LeftShift, p.ReadByte(LastMake)); // a break does not touch the last make code
        Assert.Equal((byte)(count0 + 1), p.ReadByte(MakeCount));
    }

    [GameDataFact]
    public void Extended_prefix_is_handled_as_break_of_key_60h_and_the_cursor_key_uses_its_plain_code()
    {
        var p = Menu();
        var m = p.Machine;
        p.WriteByte(KeyTable + 0x60, 1);
        byte count0 = p.ReadByte(MakeCount);

        // Cursor up on an enhanced keyboard: E0 48 (make), E0 C8 (break).
        m.ScheduleScancode(m.Seconds + 0.02, 0xE0);
        m.ScheduleScancode(m.Seconds + 0.04, Scancodes.Up);
        m.RunFor(0.1);
        Assert.Equal(0, p.ReadByte(KeyTable + 0x60));
        Assert.Equal(1, p.ReadByte((ushort)(KeyTable + Scancodes.Up)));
        Assert.Equal(Scancodes.Up, p.ReadByte(LastMake));
        Assert.Equal((byte)(count0 + 1), p.ReadByte(MakeCount));

        m.ScheduleScancode(m.Seconds + 0.02, 0xE0);
        m.ScheduleScancode(m.Seconds + 0.04, (byte)(Scancodes.Up | 0x80));
        m.RunFor(0.1);
        Assert.Equal(0, p.ReadByte((ushort)(KeyTable + Scancodes.Up)));
        Assert.Equal((byte)(count0 + 1), p.ReadByte(MakeCount));
    }

    [GameDataFact]
    public void Make_count_is_a_byte_and_wraps()
    {
        var p = Menu();
        var m = p.Machine;
        p.WriteByte(MakeCount, 0xFF);
        byte next0 = p.ReadByte(MakeCount + 1);
        m.ScheduleScancode(m.Seconds + 0.02, Scancodes.LeftShift);
        m.ScheduleScancode(m.Seconds + 0.04, (byte)(Scancodes.LeftShift | 0x80));
        m.RunFor(0.1);
        Assert.Equal(0, p.ReadByte(MakeCount));
        Assert.Equal(next0, p.ReadByte(MakeCount + 1)); // neighbouring byte untouched
    }
}
