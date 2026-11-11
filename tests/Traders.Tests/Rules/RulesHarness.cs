using Traders.Core.State;
using Traders.RefEmu;
using Traders.RefEmu.Cpu;

namespace Traders.Tests.Rules;

/// <summary>
/// Runs a weekly rule once in the original (T.EXE loaded with <see cref="OriginalProgram.LoadOnly"/>, routine called with
/// a prepared data segment) and once in the port on the same bytes, and compares the whole data segment, the random
/// state and the event log. Expected values come only from the original run.
/// </summary>
internal sealed class RulesHarness
{
    private readonly OriginalProgram _program = OriginalProgram.LoadOnly(TestGameData.DosDir);
    private readonly byte[] _initial;

    public RulesHarness()
    {
        _initial = _program.DataSegment();
    }

    /// <summary>Fresh copy of the data segment as loaded from T.EXE (initialised data, BSS zero).</summary>
    public Dgroup Initial() => new(_initial[..DgroupLayout.Size]);

    /// <summary>Result of the original routine for the given input state.</summary>
    public sealed record Outcome(byte[] DataSegment, uint RandomState, int Events, Exception? Fault);

    /// <summary>Writes <paramref name="input"/> into the emulator, calls the routine and returns the data segment.</summary>
    public Outcome RunOriginal(Dgroup input, ushort segment, ushort offset)
    {
        var m = _program.Machine;
        var full = (byte[])_initial.Clone();
        input.Bytes.AsSpan(0, DgroupLayout.Size).CopyTo(full);
        m.Mem.WriteBlock((uint)(_program.Dgroup * 16), full);
        m.Cpu.Ss = 0x9000;
        m.Cpu.Sp = 0xFFF0;
        int before = m.Log.Events.Count;
        Exception? fault = null;
        try
        {
            _program.Call(segment, offset);
        }
        catch (EmulationException e)
        {
            fault = e;
        }
        return new Outcome(_program.DataSegment(), _program.RandomState, m.Log.Events.Count - before, fault);
    }

    /// <summary>
    /// Runs original and port on the same input and asserts that the data segment (including the random state DS:10D2) is
    /// byte-identical afterwards, that the original logged no host service (no clock, file, video or sound access) and
    /// that the bytes behind the DGROUP image stay untouched. Returns the port's result.
    /// </summary>
    public Dgroup AssertSame(Dgroup input, ushort segment, ushort offset, Action<Dgroup> port, string label)
    {
        var original = RunOriginal(input, segment, offset);
        Assert.True(original.Fault is null, $"{label}: original faulted: {original.Fault?.Message}");
        var ported = new Dgroup((byte[])input.Bytes.Clone());
        port(ported);

        Assert.Equal(0, original.Events);
        Assert.Equal(_initial[DgroupLayout.Size..], original.DataSegment[DgroupLayout.Size..]);
        var diff = Enumerable.Range(0, DgroupLayout.Size)
            .Where(i => original.DataSegment[i] != ported.Bytes[i])
            .Take(8)
            .Select(i => $"DS:{i:X4} original {original.DataSegment[i]:X2} port {ported.Bytes[i]:X2}")
            .ToList();
        Assert.True(diff.Count == 0, $"{label}: " + string.Join(", ", diff));
        Assert.Equal(input.U32(DgroupLayout.RandomSeed), original.RandomState);
        return ported;
    }

    /// <summary>Original values of a few DS words after the call (for documented probe values).</summary>
    public static short Word(byte[] ds, int offset) => (short)(ds[offset] | (ds[offset + 1] << 8));
}
