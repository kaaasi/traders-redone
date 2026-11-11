using Traders.RefEmu;
using Traders.RefEmu.Hardware;
using static Traders.Tests.Phases.WeekLoopSequence;

namespace Traders.Tests.Phases;

/// <summary>
/// The week loop 1777:01F8 in the RefEmu against its finding docs/routines/1777_01F8_wochenschleife.md: order of the 30 far
/// calls, phase markers 0..6 and week counter DS:4B9A over two complete weeks, and exit A back to the main menu. No port is
/// involved; the expected values are the statically read table in <see cref="WeekLoopSequence"/>.
/// </summary>
public class WeekLoopTests
{
    private static string Dir => TestGameData.DosDir;

    /// <summary>Records the events of the week loop (call sites, markers, counter step) while the program runs.</summary>
    private sealed class Recorder
    {
        public List<WeekEvent> Events { get; } = [];

        public Recorder(OriginalProgram p)
        {
            var m = p.Machine;
            ushort loop = p.Segment(Segment);
            foreach (var c in Calls.Select(c => c.Site).Distinct())
                m.AddCodeHook(loop, c, mm =>
                {
                    // far call: 9A offset segment (segment relocated at load time)
                    byte op = mm.Mem.Read8(mm.Cpu.Cs, mm.Cpu.IP);
                    ushort off = mm.Mem.Read16(mm.Cpu.Cs, (ushort)(mm.Cpu.IP + 1));
                    ushort seg = mm.Mem.Read16(mm.Cpu.Cs, (ushort)(mm.Cpu.IP + 3));
                    Assert.Equal(0x9A, op);
                    Events.Add(new WeekEvent(WeekEventKind.Call, p.Week, mm.Cpu.IP, (ushort)(seg - mm.LoadSegment), off));
                });
            m.AddCodeHook(p.Segment(OriginalProgram.PhaseMarkerSegment), OriginalProgram.PhaseMarkerOffset,
                _ => Events.Add(new WeekEvent(WeekEventKind.Marker, p.Week, p.StackArgument(0))));
            m.AddCodeHook(loop, CounterIncrement, _ => Events.Add(new WeekEvent(WeekEventKind.Increment, p.Week, p.Week)));
            m.AddCodeHook(loop, BackJump, _ => Events.Add(new WeekEvent(WeekEventKind.BackJump, p.Week, p.Week)));
        }
    }

    /// <summary>
    /// Default game (four computer players, RETURN after 0.5 s) from the main menu to the start of week 2: weeks 0 and 1 run
    /// all 30 calls with markers 0..6 in the order of the finding, and the counter steps 0 → 1 → 2 exactly at 1777:02D3.
    /// Takes about 45 s of host time.
    /// </summary>
    [GameDataFact]
    public void Two_weeks_follow_the_call_order_markers_and_week_counter_of_the_finding()
    {
        var p = OriginalProgram.StartGame(Dir);
        var rec = new Recorder(p);
        bool reached = p.RunToWeekStart(2, 600);
        string? diff = Difference(Expected(0, 2), rec.Events);
        Assert.True(diff is null, diff);
        Assert.True(reached, $"week 2 not reached at {p.Machine.Seconds:F1} s");
        Assert.Equal(2, p.Week);
    }

    /// <summary>
    /// Exit A: with scancode 0Eh (Backspace) held while the weekly balance of week 0 is shown, the loop returns AL = 0 right
    /// after the balance - no disasters, no markers, counter unchanged - and main calls the main menu again.
    /// </summary>
    [GameDataFact]
    public void Backspace_after_the_weekly_balance_returns_to_the_main_menu()
    {
        var p = OriginalProgram.StartGame(Dir);
        var m = p.Machine;
        var rec = new Recorder(p);
        Assert.True(p.RunToWeekStart(0, 5), "week 0 did not start within 5 s after the menu");
        int? alAtReturn = null, weekAtReturn = null;
        bool menuAgain = false;
        m.AddCodeHook(p.Segment(Segment), ReturnInMain, mm => { alAtReturn ??= mm.Cpu.Ax & 0xFF; weekAtReturn ??= p.Week; });
        m.AddCodeHook(p.Segment(MainMenuSegment), MainMenuOffset, _ => menuAgain |= alAtReturn is not null);
        // Held over the whole balance screen; the loop reads the key state DS:3D55 once after the balance returns.
        p.Press(Scancodes.Backspace, delay: 0.5, hold: 30);
        m.RunFor(60, _ => menuAgain);

        Assert.True(menuAgain, $"main menu not entered again; events: {string.Join(", ", rec.Events)}");
        Assert.Equal(0, alAtReturn);
        Assert.Equal(0, weekAtReturn);
        var expected = Calls.Take(3).Select(c => new WeekEvent(WeekEventKind.Call, 0, c.Site, c.TargetSegment, c.TargetOffset)).ToList();
        string? diff = Difference(expected, rec.Events);
        Assert.True(diff is null, diff);
    }
}
