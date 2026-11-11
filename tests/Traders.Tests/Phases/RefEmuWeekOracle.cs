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

    /// <summary>
    /// Handler for one call step: RefEmu to the call site (compare), on to the return, hand over DS and AL.
    /// <paramref name="atCallSite"/> runs at the call site after the comparison (for example to schedule key presses).
    /// </summary>
    public IWeekStepHandler Handler(Dgroup core, Action<OriginalProgram>? atCallSite = null) => new StepHandler(this, core, atCallSite);

    /// <summary>
    /// Handler for a ported call step: RefEmu to the call site (compare), the RefEmu data segment is taken over by the core,
    /// <paramref name="port"/> runs on it, the RefEmu runs on to the return, and the core result is compared with the RefEmu
    /// there (whole data segment outside <see cref="InterruptOwned"/>, which includes the random state DS:10D2). Afterwards
    /// the core continues from the RefEmu data segment. A difference after the port ends the run at once (exception with
    /// the first differing byte).
    /// </summary>
    public IWeekStepHandler PortHandler(Dgroup core, IWeekStepHandler port) => new PortStepHandler(this, core, port);

    /// <summary>Ported steps compared at their return, in order (week, call number, random draws of the port).</summary>
    public List<(int Week, int Step, long Draws)> PortedSteps { get; } = [];

    /// <summary>Runs the RefEmu to the call site of <paramref name="step"/>, passing markers and return addresses on the way.</summary>
    private void RunToCallSite(WeekStep step)
    {
        // a return address can be the next call site; marker calls on the way are passed, the core passes its markers itself
        while (!IsAtLoop(step.Site))
        {
            if (!RunToNextStop()) throw new InvalidOperationException($"RefEmu did not reach step {step}");
            ushort at = Passed[^1];
            bool markerSite = WeekLoop.Steps.Any(s => s.Kind == WeekStepKind.Marker && (s.Site == at || s.Site + 5 == at));
            bool returnSite = WeekLoop.Steps.Any(s => s.Site + 5 == at);
            if (!markerSite && !returnSite && at != step.Site)
                throw new InvalidOperationException($"RefEmu at 1777:{at:X4} instead of step {step}");
            if (WeekLoop.Steps.FirstOrDefault(s => s.Site == at) is { Kind: WeekStepKind.Call } other && other != step)
                throw new InvalidOperationException($"RefEmu calls {other} instead of step {step}");
        }
    }

    private void RunToReturn(WeekStep step)
    {
        if (!RunToNextStop() || !IsAtLoop((ushort)(step.Site + 5)))
            throw new InvalidOperationException($"RefEmu did not return from step {step}");
    }

    private void TakeOver(Dgroup core) => _p.DataSegment().AsSpan(0, DgroupLayout.Size).CopyTo(core.Bytes);

    private sealed class StepHandler(RefEmuWeekOracle o, Dgroup core, Action<OriginalProgram>? atCallSite) : IWeekStepHandler
    {
        public StepOutcome Begin(StepContext context)
        {
            var step = context.Step;
            o.RunToCallSite(step);
            o.Compare(core, $"week {o._p.Week} at {step}");
            atCallSite?.Invoke(o._p);
            o.RunToReturn(step);
            o.TakeOver(core);
            return StepOutcome.Finished((byte)o._p.Machine.Cpu.Ax);
        }

        public StepOutcome Tick(StepContext context) => throw new InvalidOperationException("oracle steps finish at once");
    }

    private sealed class PortStepHandler(RefEmuWeekOracle o, Dgroup core, IWeekStepHandler port) : IWeekStepHandler
    {
        public StepOutcome Begin(StepContext context)
        {
            var step = context.Step;
            o.RunToCallSite(step);
            int week = o._p.Week; // after the counter step of the previous week
            o.Compare(core, $"week {week} at {step}");
            o.TakeOver(core);
            // the machine seeded the generator from DS:10D2 before this call; the RefEmu state just taken over replaces it
            context.Random.Seed = core.U32(DgroupLayout.RandomSeed);
            long draws = context.Random.Calls;
            var outcome = port.Begin(context);
            if (!outcome.Done) throw new InvalidOperationException($"ported step {step} did not finish at once");
            // the machine stores the seed back after this handler; the comparison needs it now
            core.Set(DgroupLayout.RandomSeed, context.Random.Seed);
            o.RunToReturn(step);
            o.PortedSteps.Add((week, step.Number, context.Random.Calls - draws));
            int known = o.Differences.Count;
            o.Compare(core, $"week {week} after {step} (port)");
            // a wrong port fails at once instead of after the remaining weeks
            if (o.Differences.Count != known) throw new InvalidOperationException(o.Differences[^1]);
            o.TakeOver(core);
            context.Random.Seed = core.U32(DgroupLayout.RandomSeed);
            return StepOutcome.Finished((byte)o._p.Machine.Cpu.Ax);
        }

        public StepOutcome Tick(StepContext context) => throw new InvalidOperationException("ported rule steps finish at once");
    }
}
