using Traders.Core.Phases;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.RefEmu.Cpu;

namespace Traders.Tests.Phases;

/// <summary>
/// Runs main 1777:000C in the RefEmu in lockstep with the core's <see cref="ProgramMachine"/>, like
/// <see cref="RefEmuWeekOracle"/> for the week loop: for every program step the RefEmu is run to its call site in main (data
/// segment compared with the core there) and on to the return address. The week loop itself is run by a
/// <see cref="RefEmuWeekOracle"/> on the same machine.
/// </summary>
internal sealed class RefEmuProgramOracle
{
    /// <summary>
    /// Bytes the initialisation 1777:04E4 writes that the core leaves to the host (finding 1777_04E4_initialisierung.md).
    /// </summary>
    public static readonly (int Offset, int Length, string Owner)[] HostOwnedByInitialisation =
    [
        (0x007D, 1, "0D73:0712 card detection"),
        (0x146E, 1, "0D73:0712 card detection"),
        (0x2BE8, 2, "0D73:04CA AdLib port"),
        (0x0089, 2, "186C:021E heap"),
        (0x1292, 2, "186C:021E heap"),
        (0x5318, 0x18, "186C:021E heap block"),
        (0x4C00, 0x113, "186C:0544 resource table (lane programm part 2)"),
        (0x588E, 4, "0D73:06F5 saved interrupt vector"),
        (0x0648, 1, "0D73:049B handler mask"),
        (0x065D, 1, "0D73:049B DOS filter"),
        (0x115A, 3, "0D73:049B"),
        (0x10A3, 1, "218B:0002 music driver present"),
        (0x023A, 1, "0B67:0002 draw page"),
        (0x2BA3, 0x0D, "0B67:0002 page pointers and buffer"),
    ];

    /// <summary>Bytes the sound calls of main write in the original; the core's <see cref="PendingSoundStep"/> leaves them.</summary>
    public static readonly (int Offset, int Length, string Owner)[] SoundOwned =
    [
        (0x10A5, 2, "218B:0094/0153 music playing"),
        (0x10A9, 2, "218B:0094 current piece"),
    ];

    private readonly OriginalProgram _p;
    private readonly ushort _main;
    private readonly HashSet<uint> _stops = [];

    public RefEmuProgramOracle(OriginalProgram p)
    {
        _p = p;
        _main = p.Segment(ProgramLoop.Segment);
        foreach (var s in ProgramLoop.Steps)
        {
            _stops.Add(Memory.Linear(_main, s.Site));
            _stops.Add(Memory.Linear(_main, s.ReturnSite));
        }
        _stops.Add(Memory.Linear(_main, ProgramLoop.TeardownSite));
    }

    /// <summary>Sites in main the RefEmu passed (call sites, return addresses, teardown), in order.</summary>
    public List<ushort> Passed { get; } = [];

    /// <summary>Differences between core and RefEmu data segment (empty = none).</summary>
    public List<string> Differences { get; } = [];

    public bool IsAt(ushort site) => _p.Machine.Cpu.Cs == _main && _p.Machine.Cpu.IP == site;

    /// <summary>Runs the RefEmu to the next stop in main; false if not reached within the limit.</summary>
    public bool RunToNextStop(long maxInstructions = 2_000_000_000)
    {
        var m = _p.Machine;
        m.Run(1);
        m.Run(maxInstructions, mm => _stops.Contains((((uint)mm.Cpu.Cs << 4) + mm.Cpu.IP) & 0xFFFFF));
        if (m.Exited || !_stops.Contains((((uint)m.Cpu.Cs << 4) + m.Cpu.IP) & 0xFFFFF)) return false;
        Passed.Add(m.Cpu.IP);
        return true;
    }

    /// <summary>Runs the RefEmu to the call site of <paramref name="step"/>, passing return addresses on the way.</summary>
    public void RunToCallSite(ProgramStep step)
    {
        while (!IsAt(step.Site))
        {
            if (!RunToNextStop()) throw new InvalidOperationException($"RefEmu did not reach program step {step}");
            ushort at = Passed[^1];
            if (ProgramLoop.Steps.FirstOrDefault(s => s.Site == at) is { } other && other != step)
                throw new InvalidOperationException($"RefEmu calls {other} instead of {step}");
            if (at == ProgramLoop.TeardownSite) throw new InvalidOperationException($"RefEmu in the teardown instead of {step}");
        }
    }

    private void RunToReturn(ProgramStep step)
    {
        if (!RunToNextStop() || !IsAt(step.ReturnSite))
            throw new InvalidOperationException($"RefEmu did not return from program step {step}");
    }

    /// <summary>Compares the data segment outside the interrupt-owned bytes and <paramref name="skip"/>; records the first difference.</summary>
    public void Compare(Dgroup core, string where, IEnumerable<(int Offset, int Length, string Owner)>? skip = null)
    {
        var emu = _p.DataSegment();
        var ranges = RefEmuWeekOracle.InterruptOwned.Select(r => (r.Offset, r.Length)).Concat((skip ?? []).Select(r => (r.Offset, r.Length))).ToList();
        for (int i = 0; i < DgroupLayout.Size; i++)
        {
            if (core.Bytes[i] == emu[i] || ranges.Any(r => i >= r.Offset && i < r.Offset + r.Length)) continue;
            Differences.Add($"{where}: DS:{i:X4} core {core.Bytes[i]:X2} RefEmu {emu[i]:X2}");
            return;
        }
    }

    private void TakeOver(Dgroup core) => _p.DataSegment().AsSpan(0, DgroupLayout.Size).CopyTo(core.Bytes);

    private void TakeOver(Dgroup core, IEnumerable<(int Offset, int Length, string Owner)> ranges)
    {
        var emu = _p.DataSegment();
        foreach (var (offset, length, _) in ranges.Concat(RefEmuWeekOracle.InterruptOwned.Select(r => (r.Offset, r.Length, ""))))
            emu.AsSpan(offset, length).CopyTo(core.Bytes.AsSpan(offset));
    }

    /// <summary>Handler for a program step done by the RefEmu: call site (compare), return, hand over the data segment and AL.</summary>
    public IProgramStepHandler Handler(Dgroup core, Action<OriginalProgram>? atCallSite = null) => new StepHandler(this, core, atCallSite);

    /// <summary>
    /// Handler for a ported program step: call site (compare), the core runs <paramref name="port"/> on the data segment of the
    /// RefEmu, the RefEmu runs to the return and the data segment is compared outside <paramref name="hostOwned"/>; those bytes
    /// are then taken over from the RefEmu (the host's part). Where main checks the result, it must equal AL of the RefEmu.
    /// </summary>
    public IProgramStepHandler PortHandler(Dgroup core, IProgramStepHandler port,
        IReadOnlyList<(int Offset, int Length, string Owner)> hostOwned) => new PortStepHandler(this, core, port, hostOwned);

    /// <summary>Handler that only runs the RefEmu to the call site and compares there; the step never finishes (end of a test).</summary>
    public IProgramStepHandler StopAtCallSite(Dgroup core) => new StopHandler(this, core);

    /// <summary>
    /// Wraps call 1 of the week loop: while the RefEmu is still in main (before the week loop), it is first run to the call site
    /// of the week loop 1777:017F and compared there (DS:08FA already set by the core), then <paramref name="inner"/> runs.
    /// </summary>
    public IWeekStepHandler EnteringWeekLoop(Dgroup core, IWeekStepHandler inner) => new WeekEntryHandler(this, core, inner);

    private sealed class WeekEntryHandler(RefEmuProgramOracle o, Dgroup core, IWeekStepHandler inner) : IWeekStepHandler
    {
        public StepOutcome Begin(StepContext context)
        {
            var m = o._p.Machine;
            if (m.Cpu.Cs == o._main && m.Cpu.IP < WeekLoop.Entry)
            {
                var step = ProgramLoop.Step(ProgramLoop.WeekLoopStep);
                o.RunToCallSite(step);
                o.Compare(core, $"at {step}");
            }
            return inner.Begin(context);
        }

        public StepOutcome Tick(StepContext context) => inner.Tick(context);
    }

    private sealed class StepHandler(RefEmuProgramOracle o, Dgroup core, Action<OriginalProgram>? atCallSite) : IProgramStepHandler
    {
        public StepOutcome Begin(ProgramStepContext context)
        {
            o.RunToCallSite(context.Step);
            o.Compare(core, $"at {context.Step}");
            atCallSite?.Invoke(o._p);
            o.RunToReturn(context.Step);
            o.TakeOver(core);
            return StepOutcome.Finished((byte)o._p.Machine.Cpu.Ax);
        }

        public StepOutcome Tick(ProgramStepContext context) => throw new InvalidOperationException("oracle steps finish at once");
    }

    private sealed class PortStepHandler(RefEmuProgramOracle o, Dgroup core, IProgramStepHandler port,
        IReadOnlyList<(int Offset, int Length, string Owner)> hostOwned) : IProgramStepHandler
    {
        public StepOutcome Begin(ProgramStepContext context)
        {
            var step = context.Step;
            o.RunToCallSite(step);
            o.Compare(core, $"at {step}");
            o.TakeOver(core);
            uint seed = context.Random.Seed;
            var outcome = port.Begin(context);
            if (!outcome.Done) throw new InvalidOperationException($"ported step {step} did not finish at once");
            // the machine stores a changed seed after the handler returns; the comparison below comes first
            if (context.Random.Seed != seed) core.Set(DgroupLayout.RandomSeed, context.Random.Seed);
            o.RunToReturn(step);
            int known = o.Differences.Count;
            o.Compare(core, $"after {step} (port)", hostOwned);
            byte al = (byte)o._p.Machine.Cpu.Ax;
            // main ignores the result of steps without a check (21A7:000D returns 1, the stub 0)
            if (step.After != ProgramResultCheck.None && outcome.Result != al) o.Differences.Add($"after {step} (port): result core {outcome.Result} RefEmu {al}");
            if (o.Differences.Count != known) throw new InvalidOperationException(o.Differences[^1]);
            o.TakeOver(core, hostOwned);
            return outcome;
        }

        public StepOutcome Tick(ProgramStepContext context) => throw new InvalidOperationException("ported steps finish at once");
    }

    private sealed class StopHandler(RefEmuProgramOracle o, Dgroup core) : IProgramStepHandler
    {
        public StepOutcome Begin(ProgramStepContext context)
        {
            o.RunToCallSite(context.Step);
            o.Compare(core, $"at {context.Step}");
            return StepOutcome.Running;
        }

        public StepOutcome Tick(ProgramStepContext context) => StepOutcome.Running;
    }
}
