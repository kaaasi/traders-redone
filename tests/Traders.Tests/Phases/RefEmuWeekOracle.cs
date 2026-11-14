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
    private void Compare(Dgroup core, string where, bool skipRandomState, (int Offset, int Length)[]? owned = null)
    {
        var emu = _p.DataSegment();
        var diffs = new List<int>();
        for (int i = 0; i < DgroupLayout.Size; i++)
        {
            if (core.Bytes[i] == emu[i] || IsInterruptOwned(i)) continue;
            if (owned != null && owned.Any(r => i >= r.Offset && i < r.Offset + r.Length)) continue;
            if (skipRandomState && i >= DgroupLayout.RandomSeed && i < DgroupLayout.RandomSeed + 4) continue;
            diffs.Add(i);
        }
        if (diffs.Count == 0) return;
        int first = diffs[0];
        var ranges = new List<string>();
        for (int k = 0; k < diffs.Count;)
        {
            int a = diffs[k], b = a;
            while (k + 1 < diffs.Count && diffs[k + 1] <= b + 4) b = diffs[++k];
            k++;
            ranges.Add($"{a:X4}-{b:X4}");
        }
        Differences.Add($"{where}: DS:{first:X4} core {core.Bytes[first]:X2} RefEmu {emu[first]:X2} (differing {string.Join(" ", ranges.Take(20))})");
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

    /// <summary>
    /// Handler for a ported call step with a screen, frame by frame in lockstep with the RefEmu (<see cref="FramedStep"/>).
    /// <paramref name="owned"/> are the data segment bytes of the output layers (not compared; taken from the RefEmu at the return).
    /// </summary>
    public FramedStep FramedPortHandler(Dgroup core, IWeekStepHandler port, (int Offset, int Length)[] owned,
        Action<OriginalProgram>? atCallSite = null) => new(this, core, port, owned, atCallSite);

    /// <summary>
    /// A ported screen step in lockstep with the RefEmu: at the call site the data segment is compared and taken over; then every
    /// frame of the port is one retrace of the original (entry of the retrace wait 0D73:00D3). Where the port waits a frame the
    /// RefEmu must stand at its next retrace, where the port returns the RefEmu must return to the loop; at both the data
    /// segment outside <see cref="InterruptOwned"/> and the output bytes, and the port's generator against DS:10D2, are
    /// compared. Before the next frame the driver calls <see cref="EndRetrace"/> and gives the port the key table the
    /// RefEmu holds then (<see cref="Held"/>).
    /// </summary>
    public sealed class FramedStep(RefEmuWeekOracle o, Dgroup core, IWeekStepHandler port, (int Offset, int Length)[] owned,
        Action<OriginalProgram>? atCallSite) : IWeekStepHandler
    {
        private const ushort RetraceSegment = 0x0D73, RetraceEntry = 0x00D3, RetraceEnd = 0x00F7;
        private int _week;
        private long _draws;

        /// <summary>True while the RefEmu stands at a retrace entry and the port waits for the next frame.</summary>
        public bool AtRetrace { get; private set; }

        /// <summary>Retraces in lockstep so far (all runs of the step).</summary>
        public int Retraces { get; private set; }

        /// <summary>Frames of each run of the step, in order (week, retraces).</summary>
        public List<(int Week, int Retraces)> Runs { get; } = [];

        public StepOutcome Begin(StepContext context)
        {
            var step = context.Step;
            o.RunToCallSite(step);
            _week = o._p.Week;
            o.Compare(core, $"week {_week} at {step}");
            atCallSite?.Invoke(o._p);
            o.TakeOver(core);
            _draws = context.Random.Calls;
            Runs.Add((_week, 0));
            return Sync(context, port.Begin(context));
        }

        public StepOutcome Tick(StepContext context)
        {
            if (AtRetrace) throw new InvalidOperationException("EndRetrace not called before the frame");
            return Sync(context, port.Tick(context));
        }

        /// <summary>Runs the RefEmu through the retrace wait it stands at.</summary>
        public void EndRetrace()
        {
            var m = o._p.Machine;
            ushort seg = o._p.Segment(RetraceSegment);
            m.Run(4_000_000, mm => mm.Cpu.Cs == seg && mm.Cpu.IP == RetraceEnd);
            if (!(m.Cpu.Cs == seg && m.Cpu.IP == RetraceEnd)) throw new InvalidOperationException("retrace wait did not end");
            AtRetrace = false;
        }

        /// <summary>Scancodes held in the RefEmu's key table now.</summary>
        public IReadOnlyCollection<byte> Held()
        {
            var keys = o._p.Machine.Mem.ReadBlock(o._p.Dgroup, DgroupLayout.KeyState, 0x80);
            return [.. Enumerable.Range(0, keys.Length).Where(sc => keys[sc] != 0).Select(sc => (byte)sc)];
        }

        private StepOutcome Sync(StepContext context, StepOutcome outcome)
        {
            var step = context.Step;
            var m = o._p.Machine;
            ushort retrace = o._p.Segment(RetraceSegment), ret = (ushort)(step.Site + 5);
            m.Run(1);
            m.Run(4_000_000_000, mm => (mm.Cpu.Cs == retrace && mm.Cpu.IP == RetraceEntry) || (mm.Cpu.Cs == o._loopSegment && mm.Cpu.IP == ret));
            if (m.Exited) throw new InvalidOperationException("original exited");
            bool atRetrace = m.Cpu.Cs == retrace && m.Cpu.IP == RetraceEntry;
            string where = $"week {_week} {step} retrace {Runs[^1].Retraces}";
            if (outcome.Done == atRetrace)
                throw new InvalidOperationException(outcome.Done
                    ? $"{where}: the port returned, the original waits another retrace"
                    : $"{where}: the port waits a frame, the original returned");
            o.Compare(core, where + (outcome.Done ? " (return)" : ""), skipRandomState: true, owned);
            uint emuSeed = BitConverter.ToUInt32(o._p.DataSegment(), DgroupLayout.RandomSeed);
            if (context.Random.Seed != emuSeed) o.Differences.Add($"{where}: random state core {context.Random.Seed:X8} RefEmu {emuSeed:X8}");
            if (o.Differences.Count > 0) throw new InvalidOperationException(o.Differences[^1]);
            if (!outcome.Done)
            {
                AtRetrace = true;
                Retraces++;
                Runs[^1] = (Runs[^1].Week, Runs[^1].Retraces + 1);
                return outcome;
            }
            o.Passed.Add(ret);
            o.PortedSteps.Add((_week, step.Number, context.Random.Calls - _draws));
            var emu = o._p.DataSegment();
            foreach (var (offset, length) in owned) emu.AsSpan(offset, length).CopyTo(core.Bytes.AsSpan(offset));
            o.TakeOverInterruptOwned(core);
            return StepOutcome.Finished((byte)m.Cpu.Ax);
        }
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
