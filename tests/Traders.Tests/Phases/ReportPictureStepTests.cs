using Traders.Core;
using Traders.Core.Phases;
using Traders.Core.Startup;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.Startup;

namespace Traders.Tests.Phases;

/// <summary>
/// Quick test of the picture load of call 3 (#135, R5A-2): the handler <see cref="WeekSteps"/> binds to call 3 records the
/// picture load 186C:0002 of the week report 0825:000A in the picture table (entries DS:09CE, number DS:09C8); before, only
/// the multi-round probe covered it. The default game runs in the RefEmu to the call site of call 3 in week 0 and on to its
/// return; the port runs the bound handler alone in a phase machine from call 3 on the data segment of the call site.
/// </summary>
public class ReportPictureStepTests
{
    private static readonly (int Offset, int Length) Table =
        (ImageTable.CurrentImage, ImageTable.Start + ImageTable.EntrySize * ImageTable.Count - ImageTable.CurrentImage);

    /// <summary>
    /// [DIFFERENTIAL] 0825:000A – the picture table (DS:09C8…0A8D) after call 3 of week 0 equals the original's; the bare report
    /// step without the picture load leaves it as at the call site, which the original does not (counter-check).
    /// </summary>
    [GameDataFact]
    public void Call_3_records_the_report_picture_in_the_picture_table_like_the_original()
    {
        var p = OriginalProgram.StartGame(TestGameData.DosDir);
        Assert.True(p.RunToWeekStart(0, 5), "week 0 did not start within 5 s after the menu");
        var m = p.Machine;
        ushort loop = p.Segment(WeekLoop.Segment), site = WeekLoop.Step(WeekSteps.ReportCall).Site;
        m.Run(4_000_000_000, mm => mm.Cpu.Cs == loop && mm.Cpu.IP == site);
        Assert.True(m.Cpu.Cs == loop && m.Cpu.IP == site, "call site of call 3 not reached");
        var atCall = p.DataSegment().AsSpan(0, DgroupLayout.Size).ToArray();
        m.Run(1);
        m.Run(4_000_000_000, mm => mm.Cpu.Cs == loop && mm.Cpu.IP == site + 5);
        Assert.True(m.Cpu.Cs == loop && m.Cpu.IP == site + 5, "return of call 3 not reached");
        var original = p.DataSegment().AsSpan(Table.Offset, Table.Length).ToArray();
        Assert.False(original.AsSpan().SequenceEqual(atCall.AsSpan(Table.Offset, Table.Length)), "the original changed no picture table byte");

        var steps = new WeekSteps(_ => { }, ProgramStartDifferentialTests.Files);
        Assert.Equal(original, Run(atCall, steps.Handlers[WeekSteps.ReportCall]));
        Assert.Equal(atCall.AsSpan(Table.Offset, Table.Length).ToArray(), Run(atCall, steps.Report));
    }

    /// <summary>The handler alone from call 3 until the machine stops at call 4 (no handler); the picture table afterwards.</summary>
    private static byte[] Run(byte[] atCall, IWeekStepHandler handler)
    {
        var core = new Dgroup((byte[])atCall.Clone());
        var machine = new PhaseMachine(core, new OriginalRandom(new RecordedClock([])),
            new Dictionary<int, IWeekStepHandler> { [WeekSteps.ReportCall] = handler }, nextStep: WeekSteps.ReportCall);
        for (long f = 0; f < 5000 && machine.Status != PhaseStatus.Blocked; f++) machine.Advance(new PhaseTick(f, []));
        Assert.Equal(PhaseStatus.Blocked, machine.Status);
        Assert.Equal(CatastropheGuardStep.Call, machine.NextStep);
        return core.Bytes.AsSpan(Table.Offset, Table.Length).ToArray();
    }
}
