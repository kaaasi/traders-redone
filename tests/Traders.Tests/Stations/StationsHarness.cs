using Traders.Core;
using Traders.Core.Field;
using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.State;
using Traders.Core.Stations;
using Traders.RefEmu;
using Traders.RefEmu.Cpu;
using Traders.Tests.Field;

namespace Traders.Tests.Stations;

/// <summary>
/// Runs routines of segment 1B03 in the original (T.EXE loaded with <see cref="OriginalProgram.LoadOnly"/>, direct far calls)
/// and in the port on the same data segment. Every call into the output layers is replaced by a far return and recorded with
/// its word arguments (the data segment as "DS"); the port reports the same calls as <see cref="StationEvent"/>s. 21AF:0412
/// and 21AF:045E run in the original and are only recorded (their pointer and restore bytes are the output's, see
/// <see cref="PlantingDifferentialTests.OutputOwned"/>). 0E31:023D, 0609:10D2, 0E9E:000E, the runtime's LDIV and random()
/// run in the original and are called by the port. Compared: the whole data segment (with DS:10D2), the number of random()
/// calls, the result in AX and the sequence of recorded calls. At every page flip 21FD:051F the key table gets the keys of the
/// next pass, in the original inside the stub, in the port at the first frame after a flip of 1B03 (<see cref="StationPhase.Flips"/>).
/// </summary>
internal sealed class StationsHarness
{
    public const ushort Seg = 0x1B03;

    /// <summary>Output calls replaced by retf: address, number of word arguments, port event.</summary>
    public static readonly (ushort Seg, ushort Off, int Args, StationCall Call)[] Stubs =
    [
        (0x0A96, 0x001B, 0, StationCall.ClearList),
        (0x0A96, 0x06A8, 0, StationCall.RunList),
        (0x0A96, 0x0030, 6, StationCall.Blit),
        (0x0A96, 0x0311, 5, StationCall.Blit8),
        (0x0A96, 0x03FB, 3, StationCall.Plot),
        (0x0A96, 0x0525, 6, StationCall.Text),
        (0x0A96, 0x0C3F, 1, StationCall.CopyPage),
        (0x0A69, 0x004D, 8, StationCall.ShadowText),
        (0x0A69, 0x011B, 9, StationCall.ShadowNumber),
        (0x0A69, 0x0243, 8, StationCall.TypedText),
        (0x0A69, 0x0288, 8, StationCall.TypedLines),
        (0x21A7, 0x0054, 2, StationCall.Sound),
        (0x23CD, 0x06E9, 1, StationCall.DrawObjects),
        (0x23CD, 0x071C, 1, StationCall.RestoreObjects),
        (0x186C, 0x0002, 1, StationCall.LoadPicture),
        (0x21AF, 0x00CA, 6, StationCall.ObjectImage),
        (0x21AF, 0x022B, 5, StationCall.ObjectBuffers),
        (0x21FD, 0x0185, 2, StationCall.Delay),
        (0x21FD, 0x060D, 0, StationCall.ListOnBothPages),
        (0x21FD, 0x051F, 0, StationCall.Flip),
        (0x21FD, 0x0245, 0, StationCall.Pause),
        (0x21FD, 0x0665, 0, StationCall.LastFrame),
        (0x21FD, 0x037D, 1, StationCall.FadeOut),
        (0x21FD, 0x026C, 2, StationCall.BasePalette),
        (0x21FD, 0x0329, 0, StationCall.FadeIn),
        (0x21FD, 0x03EB, 2, StationCall.PaletteStep),
        (0x0B67, 0x01D4, 0, StationCall.DrawOnShownPage),
        (0x0B67, 0x0206, 0, StationCall.DrawOnHiddenPage),
        (0x2295, 0x0001, 5, StationCall.NumberText),
    ];

    /// <summary>Bytes the C runtime's tzset writes on the first time() of random(0) (not game state).</summary>
    private static readonly (int Offset, int Length, string Owner) TimeZoneNames = (0x5894, 8, "C runtime tzset: time zone names");

    /// <summary>Calls that run in the original and are only recorded.</summary>
    public static readonly (ushort Seg, ushort Off, int Args, StationCall Call)[] Traced =
    [
        (0x21AF, 0x0412, 2, StationCall.ShowObject),
        (0x21AF, 0x045E, 1, StationCall.HideObject),
    ];

    private readonly OriginalProgram _program = OriginalProgram.LoadOnly(TestGameData.DosDir);
    private readonly byte[] _initial;
    private readonly List<string> _calls = [];
    private readonly List<short> _random = [];
    private readonly List<int> _clock = [];
    private Func<int, IReadOnlyCollection<byte>> _keys = _ => [];
    private int _pass;

    public StationsHarness()
    {
        _initial = _program.DataSegment();
        var m = _program.Machine;
        foreach (var (seg, off, args, call) in Stubs)
        {
            ushort s = _program.Segment(seg);
            m.AddCodeHook(s, off, _ =>
            {
                Record(call, args);
                if (call == StationCall.Flip) WriteKeys(_keys(++_pass));
            });
            m.Mem.Write8(s, off, 0xCB); // retf: the callers remove their arguments
        }
        foreach (var (seg, off, args, call) in Traced)
            m.AddCodeHook(_program.Segment(seg), off, _ => Record(call, args));
        m.AddCodeHook(_program.Segment(0x21FD), 0x01BA, _ => _random.Add((short)_program.StackArgument(0)));
        // random(0) seeds from time() (0000:3405); its result DX:AX feeds the port's clock.
        m.AddCodeHook(_program.Segment(0x21FD), 0x01CE, mm => _clock.Add(mm.Cpu.Ax | (mm.Cpu.Dx << 16)));
    }

    private void Record(StationCall call, int args)
    {
        var words = Enumerable.Range(0, args).Select(i => _program.StackArgument(i))
            .Select(w => w == _program.Dgroup ? StationEvent.DataSegment : (short)w).ToArray();
        _calls.Add(new StationEvent(call, words).ToString());
    }

    private void WriteKeys(IReadOnlyCollection<byte> held)
    {
        for (int sc = 0; sc < KeyTable.Length; sc++) _program.WriteByte((ushort)(DgroupLayout.KeyState + sc), 0);
        foreach (byte sc in held) _program.WriteByte((ushort)(DgroupLayout.KeyState + sc), 1);
    }

    /// <summary>Segment of the data segment in the emulator (for far pointer arguments).</summary>
    public ushort Dgroup => _program.Dgroup;

    /// <summary>Fresh copy of the data segment as loaded from T.EXE.</summary>
    public Dgroup Initial() => new(_initial[..DgroupLayout.Size]);

    public sealed record Outcome(byte[] DataSegment, IReadOnlyList<short> Random, IReadOnlyList<string> Calls, int Ax,
        Exception? Fault, int Passes);

    /// <summary>Calls 1B03:<paramref name="off"/>(<paramref name="args"/>) in the original with <paramref name="input"/>.</summary>
    public Outcome RunOriginal(Dgroup input, ushort off, ushort[] args, Func<int, IReadOnlyCollection<byte>>? keys = null)
    {
        var m = _program.Machine;
        var full = (byte[])_initial.Clone();
        input.Bytes.AsSpan(0, DgroupLayout.Size).CopyTo(full);
        m.Mem.WriteBlock((uint)(_program.Dgroup * 16), full);
        m.Cpu.Ss = 0x9000;
        m.Cpu.Sp = 0xFFF0;
        _calls.Clear();
        _random.Clear();
        _clock.Clear();
        _keys = keys ?? (_ => []);
        _pass = 0;
        WriteKeys(_keys(0));
        Exception? fault = null;
        int ax = 0;
        try
        {
            ax = _program.Call(Seg, off, args).Ax;
        }
        catch (EmulationException e)
        {
            fault = e;
        }
        return new Outcome(_program.DataSegment(), _random.ToList(), _calls.ToList(), ax, fault, _pass);
    }

    public sealed record PortOutcome(Dgroup State, long RandomCalls, IReadOnlyList<string> Calls, int Result, int Passes);

    /// <summary>
    /// Runs <paramref name="port"/> on a copy of <paramref name="input"/>; it may return a coroutine (driven to its end, keys
    /// written at the first frame after every flip of 1B03) and a result (compared with AX).
    /// </summary>
    public PortOutcome RunPort(Dgroup input, Func<StationPhase, (IEnumerable<FieldWait>? Run, Func<int>? Result)> port,
        Func<int, IReadOnlyCollection<byte>>? keys = null)
    {
        keys ??= _ => [];
        var d = new Dgroup((byte[])input.Bytes.Clone());
        var random = new OriginalRandom(new RecordedClock(_clock.ToList()), d.U32(DgroupLayout.RandomSeed));
        // The output is stubbed on both sides, so the frames of the typed texts are not compared here (no text buffer).
        var phase = new StationPhase(d, random, KeyboardOnlyDevices.Instance) { TextLength = (_, _) => 0 };
        KeyTable.Write(d, keys(0));
        var (run, result) = port(phase);
        int passes = 0;
        if (run is not null)
            foreach (var _ in run)
                if (phase.Flips > passes)
                {
                    KeyTable.Write(d, keys(++passes));
                    if (passes > 100_000) throw new InvalidOperationException("Port coroutine did not finish.");
                }
        int r = result?.Invoke() ?? 0;
        d.Set(DgroupLayout.RandomSeed, random.Seed);
        // Far pointers read from the data segment hold its segment value; both sides show it as "DS".
        var calls = phase.Events
            .Select(e => new StationEvent(e.Call, e.Args.Select(a => (ushort)a == _program.Dgroup ? StationEvent.DataSegment : a).ToArray()))
            .Select(e => e.ToString()).ToList();
        return new PortOutcome(d, random.Calls, calls, r, passes);
    }

    /// <summary>
    /// Original and port on the same input: data segment byte-identical (except the output's object bytes), same number of
    /// random() calls, same recorded calls, same AX (when the port gives a result). Returns the port's state.
    /// </summary>
    public Dgroup AssertSame(Dgroup input, ushort off, ushort[] args,
        Func<StationPhase, (IEnumerable<FieldWait>? Run, Func<int>? Result)> port, string label,
        Func<int, IReadOnlyCollection<byte>>? keys = null)
    {
        var original = RunOriginal(input, off, args, keys);
        Assert.True(original.Fault is null, $"{label}: original faulted: {original.Fault?.Message}");
        var ported = RunPort(input, port, keys);
        Assert.Equal(_initial[DgroupLayout.Size..], original.DataSegment[DgroupLayout.Size..]);
        var owned = new bool[DgroupLayout.Size];
        foreach (var (o, len, _) in PlantingDifferentialTests.OutputOwned.Append(TimeZoneNames))
            for (int i = o; i < o + len; i++) owned[i] = true;
        var diff = Enumerable.Range(0, DgroupLayout.Size)
            .Where(i => !owned[i] && original.DataSegment[i] != ported.State.Bytes[i])
            .Take(8)
            .Select(i => $"DS:{i:X4} original {original.DataSegment[i]:X2} port {ported.State.Bytes[i]:X2}")
            .ToList();
        Assert.True(diff.Count == 0, $"{label}: " + string.Join(", ", diff));
        Assert.True(original.Random.Count == ported.RandomCalls,
            $"{label}: original drew {original.Random.Count} times ({string.Join(", ", original.Random)}), port {ported.RandomCalls}");
        Assert.Equal(original.Passes, ported.Passes);
        for (int i = 0; i < Math.Max(original.Calls.Count, ported.Calls.Count); i++)
        {
            string o = i < original.Calls.Count ? original.Calls[i] : "(none)";
            string p = i < ported.Calls.Count ? ported.Calls[i] : "(none)";
            Assert.True(o == p, $"{label}: call {i}: original {o}, port {p}");
        }
        return ported.State;
    }

    /// <summary>As <see cref="AssertSame"/> for a routine with a result in AX (compared as a word).</summary>
    public Dgroup AssertSameResult(Dgroup input, ushort off, ushort[] args, Func<StationPhase, short> port, string label)
    {
        var original = RunOriginal(input, off, args);
        short result = 0;
        var state = AssertSame(input, off, args, ph => (null, () => result = port(ph)), label);
        Assert.True((short)original.Ax == result, $"{label}: AX original {(short)original.Ax}, port {result}");
        return state;
    }
}
