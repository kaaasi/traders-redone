using Traders.RefEmu;
using Traders.RefEmu.Cpu;

namespace Traders.Tests.RefEmu;

/// <summary>
/// The instruction budget of <see cref="OriginalProgram.Call"/>/<see cref="OriginalProgram.CallWithBudget"/> and
/// <see cref="OriginalProgram.RunToRoutine(ushort, ushort, double, long)"/> (#222, lane woche part 1): a run that does not get
/// there within the budget stops with an error naming the routine and the CS:IP where it stopped (image addresses); within the
/// budget the run is the same as without one.
/// </summary>
public class OriginalProgramBudgetTests
{
    private const ushort Seg21FD = 0x21FD, Wait = 0x0185;

    [GameDataFact]
    public void A_call_beyond_its_budget_stops_with_routine_and_cs_ip()
    {
        var p = OriginalProgram.LoadOnly(TestGameData.DosDir);
        var e = Assert.Throws<EmulationException>(() => p.CallWithBudget(2_000, Seg21FD, Wait, 50));
        Assert.StartsWith(OriginalProgram.BudgetMessage, e.Message);
        Assert.Contains("of 2000 for the call of 21FD:0185", e.Message);
        Assert.Matches(@"stopped at CS:IP [0-9A-F]{4}:[0-9A-F]{4} \(image\)", e.Message);
        Assert.Throws<ArgumentOutOfRangeException>(() => p.CallWithBudget(0, Seg21FD, Wait, 1));
    }

    [GameDataFact]
    public void A_call_within_its_budget_returns_as_without_one()
    {
        var a = OriginalProgram.LoadOnly(TestGameData.DosDir);
        var b = OriginalProgram.LoadOnly(TestGameData.DosDir);
        a.Call(Seg21FD, Wait, 3);
        b.CallWithBudget(OriginalProgram.DefaultCallBudget, Seg21FD, Wait, 3);
        Assert.Equal(a.Machine.Cpu.InstructionCount, b.Machine.Cpu.InstructionCount);
        Assert.Equal(a.DataSegment(), b.DataSegment());
    }

    [GameDataFact]
    public void A_run_to_a_routine_beyond_its_budget_stops_with_routine_and_cs_ip()
    {
        var p = OriginalProgram.Start(TestGameData.DosDir);
        // Fat Mike 1F63:000F is far behind the start: the budget runs out first
        var e = Assert.Throws<EmulationException>(() => p.RunToRoutine(0x1F63, 0x000F, 600, 100_000));
        Assert.StartsWith(OriginalProgram.BudgetMessage, e.Message);
        Assert.Contains("for the run to 1F63:000F", e.Message);

        // the main menu 192D:0007 within a generous budget: reached as without one
        var q = OriginalProgram.Start(TestGameData.DosDir);
        var r = OriginalProgram.Start(TestGameData.DosDir);
        Assert.True(q.RunToRoutine(0x192D, 0x0007, 30, 10_000_000_000));
        Assert.True(r.RunToRoutine(0x192D, 0x0007, 30));
        Assert.Equal(r.Machine.Cpu.InstructionCount, q.Machine.Cpu.InstructionCount);
    }
}
