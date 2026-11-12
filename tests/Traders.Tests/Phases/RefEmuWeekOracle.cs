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
        (0x1158, 1),                    // last make code (keyboard handler 27C6:0043; a human presses keys during the weeks)
        (0x115D, 1),                    // make counter (keyboard handler 27C6:0043)
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
    public void Compare(Dgroup core, string where) => Compare(core, where, skipRandomState: false);

    /// <summary>
    /// As <see cref="Compare(Dgroup, string)"/>; with <paramref name="skipRandomState"/> the random state DS:10D2 is left out
    /// (inside a handler, before the machine has stored the seed back).
    /// </summary>
    private void Compare(Dgroup core, string where, bool skipRandomState)
    {
        var emu = _p.DataSegment();
        for (int i = 0; i < DgroupLayout.Size; i++)
        {
            if (core.Bytes[i] == emu[i] || IsInterruptOwned(i)) continue;
            if (skipRandomState && i >= DgroupLayout.RandomSeed && i < DgroupLayout.RandomSeed + 4) continue;
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
    /// there (whole data segment outside <see cref="InterruptOwned"/> and DS:10D2, plus the generator state of the port
    /// against DS:10D2 of the RefEmu). The core keeps its own result; only the interrupt-owned bytes are taken from the
    /// RefEmu. DS:10D2 is not written here: the machine stores the seed back after the handler, and the comparison at the
    /// next call site (which includes DS:10D2) checks that write-back against the RefEmu. A difference after the port ends
    /// the run at once (exception with the first differing byte).
    /// </summary>
    /// <remarks><paramref name="atCallSite"/> runs at the call site after the comparison and before the take-over (for example
    /// to set a state in the RefEmu that both sides then start the step from).</remarks>
    public IWeekStepHandler PortHandler(Dgroup core, IWeekStepHandler port, Action<OriginalProgram>? atCallSite = null) =>
        new PortStepHandler(this, core, port, atCallSite);

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
            throw new InvalidOperationException($"RefEmu did not return from step {step} (week {_p.Week}, {_p.Machine.Seconds:F1} s emulated)");
    }

    private void TakeOver(Dgroup core) => _p.DataSegment().AsSpan(0, DgroupLayout.Size).CopyTo(core.Bytes);

    private void TakeOverInterruptOwned(Dgroup core)
    {
        var emu = _p.DataSegment();
        foreach (var (offset, length) in InterruptOwned) emu.AsSpan(offset, length).CopyTo(core.Bytes.AsSpan(offset));
    }

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

    private sealed class PortStepHandler(RefEmuWeekOracle o, Dgroup core, IWeekStepHandler port, Action<OriginalProgram>? atCallSite)
        : IWeekStepHandler
    {
        public StepOutcome Begin(StepContext context)
        {
            var step = context.Step;
            o.RunToCallSite(step);
            int week = o._p.Week; // after the counter step of the previous week
            o.Compare(core, $"week {week} at {step}");
            atCallSite?.Invoke(o._p);
            // equal after the comparison (else the run fails; atCallSite leaves DS:10D2 alone); the generator keeps the seed the
            // machine loaded from DS:10D2
            o.TakeOver(core);
            long draws = context.Random.Calls;
            var outcome = port.Begin(context);
            if (!outcome.Done) throw new InvalidOperationException($"ported step {step} did not finish at once");
            o.RunToReturn(step);
            o.PortedSteps.Add((week, step.Number, context.Random.Calls - draws));
            int known = o.Differences.Count;
            o.Compare(core, $"week {week} after {step} (port)", skipRandomState: true);
            uint emuSeed = BitConverter.ToUInt32(o._p.DataSegment(), DgroupLayout.RandomSeed);
            if (context.Random.Seed != emuSeed)
                o.Differences.Add($"week {week} after {step} (port): random state core {context.Random.Seed:X8} RefEmu {emuSeed:X8}");
            // a wrong port fails at once instead of after the remaining weeks
            if (o.Differences.Count != known) throw new InvalidOperationException(o.Differences[^1]);
            o.TakeOverInterruptOwned(core);
            return StepOutcome.Finished((byte)o._p.Machine.Cpu.Ax);
        }

        public StepOutcome Tick(StepContext context) => throw new InvalidOperationException("ported rule steps finish at once");
    }
}
