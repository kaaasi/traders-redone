using Traders.Core;
using Traders.Core.State;
using Traders.Core.Trade;
using Traders.RefEmu;
using Traders.RefEmu.Cpu;
using Traders.Tests.Rules;
using T = Traders.Core.Trade.TradeLayout;

namespace Traders.Tests.Trade;

/// <summary>
/// Runs a routine of the trade 0B9F / 0C29 once in the original (T.EXE loaded with <see cref="OriginalProgram.LoadOnly"/>,
/// routine called with a prepared data segment) and once in the port on the same bytes. Sound, the trade's own drawing
/// routines, the graphics driver, the controls and the trading computer 141D are replaced by a far return in the original and
/// recorded with their arguments (the drawing routines with the data segment values they read); the port records its events
/// and its <see cref="ITradeAi"/> calls in the same form. Compared: the whole data segment, the random state, the recorded
/// calls and the result. Expected values come only from the original run.
/// </summary>
internal sealed class TradeHarness
{
    private readonly OriginalProgram _program = OriginalProgram.LoadOnly(TestGameData.DosDir);
    private readonly byte[] _initial;
    private readonly List<string> _calls = [];
    private readonly List<short> _randomArguments = [];
    private Func<int, sbyte[]>? _script;
    private int _reads;

    /// <summary>Calls replaced by retf: (segment, offset, argument words, name); the trade's routines record DS values.</summary>
    private static readonly (ushort Seg, ushort Off, int Args, string Name)[] Stubs =
    [
        (0x21A7, 0x0054, 2, nameof(TradeEventKind.Sound)),
        (0x0A96, 0x04D7, 5, nameof(TradeEventKind.FillRect)),
        (0x0C29, 0x0F68, 0, nameof(TradeEventKind.ClearMarker)),
        (0x0C29, 0x09BB, 0, nameof(TradeEventKind.DrawPlayerNumbers)),
        (0x0C29, 0x1163, 2, "Bars"),
        (0x0C29, 0x0B60, 0, "NeedBar"),
        (0x0C29, 0x146B, 4, "Bar"),
        (0x0E9E, 0x037A, 0, "ReadControls"),
        (0x21FD, 0x0245, 0, "PauseCheck"),
        (0x141D, 0x0008, 0, "Ai.Prepare"),
        (0x141D, 0x021A, 0, "Ai.EnterTrade"),
        (0x141D, 0x0397, 0, "Ai.ChooseRole"),
        (0x141D, 0x0464, 0, "Ai.StartBidding"),
        (0x141D, 0x09F3, 0, "Ai.Bid"),
        (0x141D, 0x0E67, 0, "Ai.AfterSale"),
        (0x141D, 0x0CCA, 1, "Ai.AfterPurchase"),
    ];

    public TradeHarness()
    {
        _initial = _program.DataSegment();
        var m = _program.Machine;
        foreach (var (seg, off, args, name) in Stubs)
        {
            m.AddCodeHook(_program.Segment(seg), off, _ =>
            {
                if (name is "Bars" or "NeedBar" or "Bar") return; // drawing inside 1108, covered by DrawStockChange
                _calls.Add(name switch
                {
                    nameof(TradeEventKind.ClearMarker) => Format(name, Player(), Role(), Position()),
                    nameof(TradeEventKind.DrawPlayerNumbers) => Format(name, Player(), Role(), (short)_program.ReadWord(Cash()),
                        (short)_program.ReadWord((ushort)(Cash() + 2)), Stock()),
                    _ => Format(name, Enumerable.Range(0, args).Select(i => (short)_program.StackArgument(i)).ToArray()),
                });
                if (name == "ReadControls" && _script is { } script)
                {
                    var a = script(_reads++);
                    for (int i = 0; i < 4; i++) _program.WriteByte((ushort)(T.Action + i), unchecked((byte)a[i]));
                }
            });
            m.Mem.Write8(_program.Segment(seg), off, 0xCB); // retf: the caller removes the arguments
        }
        // 0C29:1108 runs (its 134D writes DS) and is recorded at entry.
        m.AddCodeHook(_program.Segment(0x0C29), 0x1108, _ => _calls.Add(Format(nameof(TradeEventKind.DrawStockChange),
            (short)_program.StackArgument(0), Player(), Role(), Stock(), (short)_program.ReadWord((ushort)(T.ShownStock + 2 * Player())))));
        m.AddCodeHook(_program.Segment(0x21FD), 0x01BA, _ => _randomArguments.Add((short)_program.StackArgument(0)));
    }

    private short Player() => (sbyte)_program.ReadByte(T.Player);

    private short Role() => _program.ReadByte((ushort)(T.Role + Player()));

    private short Position() => (short)_program.ReadWord((ushort)(T.Position + 2 * Player()));

    private ushort Cash() => (ushort)(DgroupLayout.Cash + 4 * Player());

    private short Stock() =>
        (short)_program.ReadWord((ushort)(DgroupLayout.Stock + 8 * Player() + 2 * (short)_program.ReadWord(T.Ware)));

    internal static string Format(string name, params short[] args) => $"{name}({string.Join(", ", args)})";

    /// <summary>Fresh copy of the data segment as loaded from T.EXE.</summary>
    public Dgroup Initial() => new(_initial[..DgroupLayout.Size]);

    public sealed record Outcome(byte[] DataSegment, uint RandomState, IReadOnlyList<string> Calls, int Result, Exception? Fault,
        int Events, int ControlReads);

    /// <summary>Calls the routine in the original on <paramref name="input"/>; <paramref name="script"/> writes the actions.</summary>
    public Outcome RunOriginal(Dgroup input, ushort segment, ushort offset, ushort[] args, Func<int, sbyte[]>? script = null)
    {
        var m = _program.Machine;
        var full = (byte[])_initial.Clone();
        input.Bytes.AsSpan(0, DgroupLayout.Size).CopyTo(full);
        m.Mem.WriteBlock((uint)(_program.Dgroup * 16), full);
        m.Cpu.Ss = 0x9000;
        m.Cpu.Sp = 0xFFF0;
        _calls.Clear();
        _randomArguments.Clear();
        _script = script;
        _reads = 0;
        int before = m.Log.Events.Count;
        Exception? fault = null;
        int result = 0;
        try
        {
            var (ax, dx) = _program.Call(segment, offset, args);
            result = ax | (dx << 16);
        }
        catch (EmulationException e)
        {
            fault = e;
        }
        return new Outcome(_program.DataSegment(), _program.RandomState, _calls.ToList(), result, fault, m.Log.Events.Count - before,
            _reads);
    }

    /// <summary>
    /// Runs original and port on the same input and asserts the same data segment, random state (no draws), recorded calls and
    /// (if <paramref name="port"/> returns one) result under <paramref name="mask"/>. Returns the port's data segment.
    /// </summary>
    public Dgroup AssertSame(Dgroup input, ushort segment, ushort offset, Func<Dgroup, Recorder, int?> port, string label,
        Func<int, sbyte[]>? script = null, int mask = -1, params ushort[] args)
    {
        var original = RunOriginal(input, segment, offset, args, script);
        Assert.True(original.Fault is null, $"{label}: original faulted: {original.Fault?.Message}");
        Assert.True(original.Events == 0, $"{label}: original used {original.Events} host services");
        Assert.True(_randomArguments.Count == 0, $"{label}: original drew random numbers");
        var ported = new Dgroup((byte[])input.Bytes.Clone());
        var recorder = new Recorder(script);
        int? result = port(ported, recorder);
        if (result is int r)
            Assert.True((r & mask) == (original.Result & mask), $"{label}: result original {original.Result & mask}, port {r & mask}");
        Assert.Equal(_initial[DgroupLayout.Size..], original.DataSegment[DgroupLayout.Size..]);
        var diff = Enumerable.Range(0, DgroupLayout.Size)
            .Where(i => original.DataSegment[i] != ported.Bytes[i])
            .Take(8)
            .Select(i => $"DS:{i:X4} original {original.DataSegment[i]:X2} port {ported.Bytes[i]:X2}")
            .ToList();
        Assert.True(diff.Count == 0, $"{label}: " + string.Join(", ", diff));
        Assert.Equal(input.U32(DgroupLayout.RandomSeed), original.RandomState);
        Assert.True(original.Calls.SequenceEqual(recorder.Calls),
            $"{label}: calls differ\n original: {string.Join(" ", original.Calls)}\n port:     {string.Join(" ", recorder.Calls)}");
        return ported;
    }

    /// <summary>The port's host and trading computer: records like the stubs of the original.</summary>
    internal sealed class Recorder(Func<int, sbyte[]>? script, Func<int, Func<int, byte>, sbyte[]>? stateScript = null)
        : ITradeHost, ITradeAi
    {
        private int _reads;

        public List<string> Calls { get; } = [];

        public OriginalRandom Random { get; } = new(new RecordedClock([]), 1);

        public void Emit(TradeEvent e) => Calls.Add(Format(e.Kind.ToString(), e.Args));

        public void ReadControls(Dgroup d)
        {
            Calls.Add(Format("ReadControls"));
            if (script is null && stateScript is null) return;
            var a = stateScript is not null ? stateScript(_reads++, at => d.U8(at)) : script!(_reads++);
            for (int i = 0; i < 4; i++) d.Set(T.Action + i, a[i]);
        }

        public IEnumerable<Traders.Core.Field.FieldWait> PauseCheck(Dgroup d)
        {
            Calls.Add(Format("PauseCheck"));
            return [];
        }

        public void Prepare(Dgroup d, OriginalRandom random) => Calls.Add(Format("Ai.Prepare"));
        public void EnterTrade(Dgroup d, OriginalRandom random) => Calls.Add(Format("Ai.EnterTrade"));
        public void ChooseRole(Dgroup d, OriginalRandom random) => Calls.Add(Format("Ai.ChooseRole"));
        public void StartBidding(Dgroup d, OriginalRandom random) => Calls.Add(Format("Ai.StartBidding"));
        public void Bid(Dgroup d, OriginalRandom random) => Calls.Add(Format("Ai.Bid"));
        public void AfterSale(Dgroup d, OriginalRandom random) => Calls.Add(Format("Ai.AfterSale"));
        public void AfterPurchase(Dgroup d, OriginalRandom random, short price) => Calls.Add(Format("Ai.AfterPurchase", price));
    }
}
