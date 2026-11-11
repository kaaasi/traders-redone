using Traders.Core.Phases;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.RefEmu.Cpu;

namespace Traders.Tests.Phases;

/// <summary>
/// Runs the original week loop in the RefEmu in lockstep with the core's <see cref="PhaseMachine"/>: for every call step the
/// RefEmu is run to that call site (checking the data segment against the core there) and on to the return address; then
/// the RefEmu data segment and AL are handed to the core as the result of the step. The loop logic between the calls –
/// order, markers, exit checks, week counter – is the core's own and is compared at every call site.
/// </summary>
internal sealed class RefEmuWeekOracle
{
    /// <summary>Data segment bytes written asynchronously by the original's interrupt handlers (timer, keyboard).</summary>
    public static readonly (int Offset, int Length)[] InterruptOwned =
    [
        (0x117E, 4),                    // tick counters DS:117E/1180 (timer handler 27C6:0000)
        (DgroupLayout.KeyState, 0x80),  // key table DS:3D47 (keyboard handler 27C6:037A)
    ];

    private readonly OriginalProgram _p;
    private readonly ushort _loopSegment;
    private readonly HashSet<uint> _stops = [];

    public RefEmuWeekOracle(OriginalProgram p)
    {
        _p = p;
        _loopSegment = p.Segment(WeekLoop.Segment);
        foreach (var s in WeekLoop.Steps)
        {
            _stops.Add(Memory.Linear(_loopSegment, s.Site));
            _stops.Add(Memory.Linear(_loopSegment, (ushort)(s.Site + 5)));
        }
        _stops.Add(Memory.Linear(_loopSegment, WeekLoopSequence.ReturnInMain));
    }

    /// <summary>Loop sites the RefEmu passed (call sites, return addresses, return into main), in order.</summary>
    public List<ushort> Passed { get; } = [];

    /// <summary>Differences between core and RefEmu data segment found at call sites (empty = none).</summary>
    public List<string> Differences { get; } = [];

    /// <summary>Runs the RefEmu to the next stop in the loop (or main); false if not reached within the limit.</summary>
    public bool RunToNextStop(long maxInstructions = 2_000_000_000)
    {
        var m = _p.Machine;
        m.Run(1);
        m.Run(maxInstructions, mm => _stops.Contains((((uint)mm.Cpu.Cs << 4) + mm.Cpu.IP) & 0xFFFFF));
        if (m.Exited || !_stops.Contains((((uint)m.Cpu.Cs << 4) + m.Cpu.IP) & 0xFFFFF)) return false;
        Passed.Add(m.Cpu.Cs == _loopSegment ? m.Cpu.IP : (ushort)0xFFFF);
        return true;
    }

    /// <summary>Starts <see cref="Passed"/> again with the current position.</summary>
    public void ResetPassed()
    {
        Passed.Clear();
        Passed.Add(_p.Machine.Cpu.IP);
    }

    public bool IsAtLoop(ushort site) => _p.Machine.Cpu.Cs == _loopSegment && _p.Machine.Cpu.IP == site;

    /// <summary>Compares the core data segment with the RefEmu (outside <see cref="InterruptOwned"/>); records the first byte that differs.</summary>
    public void Compare(Dgroup core, string where)
    {
        var emu = _p.DataSegment();
        for (int i = 0; i < DgroupLayout.Size; i++)
        {
            if (core.Bytes[i] == emu[i] || IsInterruptOwned(i)) continue;
            Differences.Add($"{where}: DS:{i:X4} core {core.Bytes[i]:X2} RefEmu {emu[i]:X2}");
            return;
        }
    }

    private static bool IsInterruptOwned(int offset) =>
        InterruptOwned.Any(r => offset >= r.Offset && offset < r.Offset + r.Length);

    /// <summary>Handler for one call step: RefEmu to the call site (compare), on to the return, hand over DS and AL.</summary>
    public IWeekStepHandler Handler(Dgroup core) => new StepHandler(this, core);

    private sealed class StepHandler(RefEmuWeekOracle o, Dgroup core) : IWeekStepHandler
    {
        public StepOutcome Begin(StepContext context)
        {
            var step = context.Step;
            // to the call site of this step (a return address can be the next call site); marker calls on the way are passed,
            // the core passes its markers itself
            while (!o.IsAtLoop(step.Site))
            {
                if (!o.RunToNextStop()) throw new InvalidOperationException($"RefEmu did not reach step {step}");
                ushort at = o.Passed[^1];
                bool markerSite = WeekLoop.Steps.Any(s => s.Kind == WeekStepKind.Marker && (s.Site == at || s.Site + 5 == at));
                bool returnSite = WeekLoop.Steps.Any(s => s.Site + 5 == at);
                if (!markerSite && !returnSite && at != step.Site)
                    throw new InvalidOperationException($"RefEmu at 1777:{at:X4} instead of step {step}");
                if (WeekLoop.Steps.FirstOrDefault(s => s.Site == at) is { Kind: WeekStepKind.Call } other && other != step)
                    throw new InvalidOperationException($"RefEmu calls {other} instead of step {step}");
            }
            o.Compare(core, $"week {o._p.Week} at {step}");
            if (!o.RunToNextStop() || !o.IsAtLoop((ushort)(step.Site + 5)))
                throw new InvalidOperationException($"RefEmu did not return from step {step}");
            o._p.DataSegment().AsSpan(0, DgroupLayout.Size).CopyTo(core.Bytes);
            return StepOutcome.Finished((byte)o._p.Machine.Cpu.Ax);
        }

        public StepOutcome Tick(StepContext context) => throw new InvalidOperationException("oracle steps finish at once");
    }
}
