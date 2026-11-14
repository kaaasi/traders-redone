using Traders.Core;
using Traders.Core.Phases;
using Traders.Core.State;
using Traders.RefEmu;

namespace Traders.Tests.Phases;

/// <summary>
/// The guard of 111D:0009 (call 4 of the week loop, finding docs/routines/111D_0009_katastrophen.md) against the original: the
/// routine runs in the RefEmu (<see cref="OriginalProgram.LoadOnly"/>) with the week DS:4B9A and the switch DS:2F30 set; the
/// rest after the guard (111D:001E) is replaced by the routine's own epilogue (pop si, pop bp, retf) and its entry is recorded.
/// Where the guard returns at once, the port's step finishes without a change: whole data segment and DS:10D2 equal; where the
/// original goes on to 001E, the port's step is pending.
/// </summary>
public class CatastropheGuardDifferentialTests
{
    private const ushort Segment = 0x111D, Entry = 0x0009, AfterGuard = 0x001E;

    public static TheoryData<short, byte> Cases()
    {
        var data = new TheoryData<short, byte>();
        foreach (short week in new short[] { short.MinValue, -1, 0, 1, 2, 3, 4, 15, 30, short.MaxValue })
            foreach (byte on in new byte[] { 0, 1, 0x80, 0xFF })
                data.Add(week, on);
        return data;
    }

    // [DIFFERENTIAL] 111D:0009 – guard 111D:000D–001B only (week ≤ 2 signed or DS:2F30 = 0 → return)
    [GameDataTheory]
    [MemberData(nameof(Cases))]
    public void Guard_returns_exactly_where_the_original_returns(short week, byte catastrophes)
    {
        var p = OriginalProgram.LoadOnly(TestGameData.DosDir);
        ushort seg = p.Segment(Segment);
        p.Machine.Mem.Write8(seg, AfterGuard, 0x5E);              // pop si
        p.Machine.Mem.Write8(seg, (ushort)(AfterGuard + 1), 0x5D);          // pop bp
        p.Machine.Mem.Write8(seg, (ushort)(AfterGuard + 2), 0xCB);          // retf
        bool passed = false;
        p.Machine.AddCodeHook(seg, AfterGuard, _ => passed = true);
        p.WriteWord(DgroupLayout.Week, (ushort)week);
        p.WriteByte(DgroupLayout.Catastrophes, catastrophes);
        p.WriteDword(DgroupLayout.RandomSeed, 0x1234_5678);

        var core = new Dgroup(p.DataSegment().AsSpan(0, DgroupLayout.Size).ToArray());
        var before = (byte[])core.Bytes.Clone();
        var guard = new CatastropheGuardStep();
        bool pending = guard.IsPending(core);
        p.Call(Segment, Entry);

        Assert.Equal(passed, pending);
        Assert.Equal(!passed, CatastropheGuardStep.ReturnsAtOnce(core));
        if (passed) return;
        var machine = new PhaseMachine(core, new OriginalRandom(new RecordedClock([])),
            new Dictionary<int, IWeekStepHandler> { [CatastropheGuardStep.Call] = guard }, nextStep: CatastropheGuardStep.Call);
        machine.Advance(new PhaseTick(0));
        Assert.Equal(CatastropheGuardStep.Call + 1, machine.NextStep);
        Assert.Equal(before, core.Bytes);
        Assert.Equal(p.DataSegment().AsSpan(0, DgroupLayout.Size).ToArray(), core.Bytes);
    }

    /// <summary>Without a handler for the rest the machine stops at call 4 beyond the guard (pending, R5P-6).</summary>
    [Fact]
    public void Machine_stops_at_call_4_beyond_the_guard()
    {
        var d = new Dgroup(new byte[DgroupLayout.Size]);
        d.Set(DgroupLayout.Week, (short)3);
        d.Set(DgroupLayout.Catastrophes, (byte)1);
        var machine = new PhaseMachine(d, new OriginalRandom(new RecordedClock([])),
            new Dictionary<int, IWeekStepHandler> { [CatastropheGuardStep.Call] = new CatastropheGuardStep() }, nextStep: CatastropheGuardStep.Call);
        machine.Advance(new PhaseTick(0));
        Assert.Equal(PhaseStatus.Blocked, machine.Status);
        Assert.Equal(CatastropheGuardStep.Call, machine.NextStep);
        Assert.Contains(machine.Events, e => e.Kind == PhaseEventKind.StepPending && e.Step == CatastropheGuardStep.Call);
        Assert.DoesNotContain(machine.Events, e => e.Kind == PhaseEventKind.StepStarted);
    }
}
