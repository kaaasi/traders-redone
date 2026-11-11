using Traders.Core;
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

    private readonly List<short> _randomArguments = [];

    public RulesHarness()
    {
        _initial = _program.DataSegment();
        // Every entry of random(n) at 21FD:01BA with its argument n (far and near calls both leave CS:IP on the stack).
        _program.Machine.AddCodeHook(_program.Segment(0x21FD), 0x01BA, _ => _randomArguments.Add((short)_program.StackArgument(0)));
    }

    /// <summary>Fresh copy of the data segment as loaded from T.EXE (initialised data, BSS zero).</summary>
    public Dgroup Initial() => new(_initial[..DgroupLayout.Size]);

    /// <summary>
    /// Initial data segment with arbitrary loop variables and random state: the routines must overwrite the former and
    /// must not consume random().
    /// </summary>
    public Dgroup Base(Inputs r)
    {
        var d = Initial();
        d.Set(DgroupLayout.ActivePlayer, (sbyte)r.Range(-128, 127));
        d.Set(DgroupLayout.LoopWare, r.Word());
        d.Set(DgroupLayout.RandomSeed, r.Next());
        return d;
    }

    /// <summary>Result of the original routine for the given input state.</summary>
    public sealed record Outcome(byte[] DataSegment, uint RandomState, int Events, Exception? Fault, int Result,
        IReadOnlyList<short> RandomArguments);

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
        _randomArguments.Clear();
        Exception? fault = null;
        int result = 0;
        try
        {
            var (ax, dx) = _program.Call(segment, offset);
            result = ax | (dx << 16);
        }
        catch (EmulationException e)
        {
            fault = e;
        }
        return new Outcome(_program.DataSegment(), _program.RandomState, m.Log.Events.Count - before, fault, result,
            _randomArguments.ToArray());
    }

    /// <summary>
    /// Runs original and port on the same input and asserts that the data segment (including the random state DS:10D2) is
    /// byte-identical afterwards, that the original logged no host service (no clock, file, video or sound access) and
    /// that the bytes behind the DGROUP image stay untouched. Returns the port's result.
    /// </summary>
    public Dgroup AssertSame(Dgroup input, ushort segment, ushort offset, Action<Dgroup> port, string label) =>
        AssertSame(input, segment, offset, d => { port(d); return null; }, label);

    /// <summary>As above for a routine with a result in DX:AX, compared with the port's return value.</summary>
    public Dgroup AssertSame(Dgroup input, ushort segment, ushort offset, Func<Dgroup, int> port, string label) =>
        AssertSame(input, segment, offset, d => (int?)port(d), label);

    /// <summary>
    /// As above for a routine that draws random numbers: the port gets an <see cref="OriginalRandom"/> started from DS:10D2
    /// and writes its state back; the number of draws must equal the calls of 21FD:01BA in the original. Returns the
    /// port's result and the arguments of the original draws.
    /// </summary>
    public (Dgroup Ported, IReadOnlyList<short> Draws) AssertSameWithRandom(Dgroup input, ushort segment, ushort offset,
        Action<Dgroup, OriginalRandom> port, string label)
    {
        long calls = 0;
        IReadOnlyList<short> draws = [];
        var ported = Compare(input, segment, offset, d =>
        {
            var random = new OriginalRandom(new RecordedClock([]), d.U32(DgroupLayout.RandomSeed));
            port(d, random);
            d.Set(DgroupLayout.RandomSeed, random.Seed);
            calls = random.Calls;
            return null;
        }, label, o => draws = o.RandomArguments);
        Assert.True(draws.Count == calls, $"{label}: original drew {draws.Count} times, port {calls}");
        return (ported, draws);
    }

    private Dgroup AssertSame(Dgroup input, ushort segment, ushort offset, Func<Dgroup, int?> port, string label)
    {
        var ported = Compare(input, segment, offset, port, label, o =>
        {
            Assert.True(o.RandomArguments.Count == 0, $"{label}: original drew {o.RandomArguments.Count} random numbers");
            Assert.Equal(input.U32(DgroupLayout.RandomSeed), o.RandomState);
        });
        return ported;
    }

    private Dgroup Compare(Dgroup input, ushort segment, ushort offset, Func<Dgroup, int?> port, string label,
        Action<Outcome> check)
    {
        var original = RunOriginal(input, segment, offset);
        Assert.True(original.Fault is null, $"{label}: original faulted: {original.Fault?.Message}");
        var ported = new Dgroup((byte[])input.Bytes.Clone());
        int? result = port(ported);
        if (result is int r) Assert.True(r == original.Result, $"{label}: result original {original.Result}, port {r}");

        Assert.Equal(0, original.Events);
        Assert.Equal(_initial[DgroupLayout.Size..], original.DataSegment[DgroupLayout.Size..]);
        var diff = Enumerable.Range(0, DgroupLayout.Size)
            .Where(i => original.DataSegment[i] != ported.Bytes[i])
            .Take(8)
            .Select(i => $"DS:{i:X4} original {original.DataSegment[i]:X2} port {ported.Bytes[i]:X2}")
            .ToList();
        Assert.True(diff.Count == 0, $"{label}: " + string.Join(", ", diff));
        check(original);
        return ported;
    }

    /// <summary>Original values of a few DS words after the call (for documented probe values).</summary>
    public static short Word(byte[] ds, int offset) => (short)(ds[offset] | (ds[offset + 1] << 8));
}

/// <summary>Test-local generator for input states (not the game's random()).</summary>
internal sealed class Inputs(uint seed)
{
    private uint _s = seed;
    public uint Next() => _s = _s * 1664525 + 1013904223;
    public int Range(int lo, int hi) => lo + (int)(Next() % (uint)(hi - lo + 1));
    public short Word() => (short)(Next() >> 16);
    public bool Chance(int percent) => Next() % 100 < (uint)percent;
}
