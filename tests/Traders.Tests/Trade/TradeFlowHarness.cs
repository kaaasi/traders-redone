using Traders.Core.State;
using Traders.Core.Trade;
using Traders.RefEmu;
using Traders.RefEmu.Cpu;
using T = Traders.Core.Trade.TradeLayout;

namespace Traders.Tests.Trade;

/// <summary>
/// Runs the whole trade 0B9F:0002 (or a part of it) in the original (T.EXE loaded with <see cref="OriginalProgram.LoadOnly"/>,
/// called with a prepared data segment) and in the port (<see cref="Trade"/>) on the same bytes. Sound, the frame and page
/// routines of 21FD/0A96, the controls, the pause check, the trading computer 141D and the trade's pure drawing routines are
/// replaced by a far return and recorded with their arguments; the drawing routines that also write the data segment
/// (0C29:0635, 109F, 1108, 0E01, 0B1F) run with their drawing primitives replaced and are recorded once at their entry, nothing
/// inside them. 2295:0062 runs and is recorded. Compared: the whole data segment, the random state (141D replaced: no draws)
/// and the recorded calls. Expected values come only from the original run.
/// </summary>
internal sealed class TradeFlowHarness
{
    private readonly OriginalProgram _program = OriginalProgram.LoadOnly(TestGameData.DosDir);
    private readonly byte[] _initial;
    private readonly List<string> _calls = [];
    private int _randomCalls, _depth, _reads;
    private Func<int, Func<int, byte>, sbyte[]>? _script;

    /// <summary>Replaced by retf and recorded outside the traced routines: (segment, offset, argument words, name).</summary>
    private static readonly (ushort Seg, ushort Off, int Args, string Name)[] Stubs =
    [
        (0x21A7, 0x0054, 2, nameof(TradeEventKind.Sound)),
        (0x0A96, 0x04D7, 5, nameof(TradeEventKind.FillRect)),
        (0x0C29, 0x0F68, 0, nameof(TradeEventKind.ClearMarker)),
        (0x0C29, 0x09BB, 0, nameof(TradeEventKind.DrawPlayerNumbers)),
        (0x0C29, 0x0B60, 0, nameof(TradeEventKind.DrawNeed)),
        (0x0C29, 0x0CC1, 0, nameof(TradeEventKind.DrawCountdown)),
        (0x0C29, 0x0D6A, 0, nameof(TradeEventKind.CountdownStep)),
        (0x0C29, 0x04BD, 0, nameof(TradeEventKind.TradePalette)),
        (0x0B9F, 0x080C, 0, nameof(TradeEventKind.Transition)),
        (0x186C, 0x0002, 1, nameof(TradeEventKind.LoadImage)),
        (0x21FD, 0x05DB, 1, nameof(TradeEventKind.SetFont)),
        (0x0A96, 0x001B, 0, nameof(TradeEventKind.ClearList)),
        (0x0A96, 0x06A8, 0, nameof(TradeEventKind.RunList)),
        (0x0A96, 0x0C3F, 0, nameof(TradeEventKind.ShowPage)),
        (0x21FD, 0x051F, 0, nameof(TradeEventKind.FlipPages)),
        (0x21FD, 0x0665, 0, nameof(TradeEventKind.LastFrame)),
        (0x21FD, 0x0185, 2, nameof(TradeEventKind.Delay)),
        (0x21FD, 0x0329, 0, nameof(TradeEventKind.FadeIn)),
        (0x21FD, 0x0353, 0, nameof(TradeEventKind.FadeInFast)),
        (0x21FD, 0x03B4, 1, nameof(TradeEventKind.FadeOutFast)),
        (0x0E9E, 0x037A, 0, "ReadControls"),
        (0x21FD, 0x0245, 0, "PauseCheck"),
        (0x141D, 0x0008, 0, "Ai.Prepare"),
        (0x141D, 0x021A, 0, "Ai.EnterTrade"),
        (0x141D, 0x0397, 0, "Ai.ChooseRole"),
        (0x141D, 0x0464, 0, "Ai.StartBidding"),
        (0x141D, 0x09F3, 0, "Ai.Bid"),
        (0x141D, 0x0E67, 0, "Ai.AfterSale"),
        (0x141D, 0x0CCA, 1, "Ai.AfterPurchase"),
        // drawing primitives inside the traced routines (recorded only if reached outside them)
        (0x0A96, 0x0030, 6, "DrawCommand"),
        (0x0A96, 0x020E, 0, "Primitive 0A96:020E"),
        (0x0A96, 0x0267, 0, "Primitive 0A96:0267"),
        (0x0A96, 0x057A, 7, "Number"),
        (0x0A69, 0x000B, 4, "Primitive 0A69:000B"),
        (0x0A69, 0x002C, 0, "Primitive 0A69:002C"),
        (0x23CD, 0x05C3, 1, "Primitive 23CD:05C3"),
        (0x23CD, 0x05DD, 0, "Primitive 23CD:05DD"),
        (0x23CD, 0x06D4, 0, "Primitive 23CD:06D4"),
        (0x2295, 0x0001, 0, "FormatNumber"),
        (0x0C29, 0x1163, 2, "Primitive 0C29:1163"),
        (0x0C29, 0x146B, 4, "Primitive 0C29:146B"),
    ];

    /// <summary>Drawing routines that run (they write the data segment): entry, offset of their retf, name.</summary>
    private static readonly (ushort Off, ushort Retf, string Name)[] Traced =
    [
        (0x0635, 0x065B, nameof(TradeEventKind.DrawBoard)),
        (0x109F, 0x1107, nameof(TradeEventKind.DrawStock)),
        (0x1108, 0x1162, nameof(TradeEventKind.DrawStockChange)),
        (0x0E01, 0x0F67, nameof(TradeEventKind.DrawMarkers)),
        (0x0B1F, 0x0B5F, nameof(TradeEventKind.DrawBasePrice)),
    ];

    public TradeFlowHarness()
    {
        _initial = _program.DataSegment();
        var m = _program.Machine;
        foreach (var (seg, off, args, name) in Stubs)
        {
            m.AddCodeHook(_program.Segment(seg), off, _ =>
            {
                if (_depth > 0) return;
                _calls.Add(name switch
                {
                    nameof(TradeEventKind.ClearMarker) => TradeHarness.Format(name, Player(), Role(), Position()),
                    nameof(TradeEventKind.DrawPlayerNumbers) => TradeHarness.Format(name, Player(), Role(),
                        (short)_program.ReadWord(Cash()), (short)_program.ReadWord((ushort)(Cash() + 2)), Stock()),
                    nameof(TradeEventKind.DrawNeed) => TradeHarness.Format(name, Player()),
                    _ => TradeHarness.Format(name, Enumerable.Range(0, args).Select(i => (short)_program.StackArgument(i)).ToArray()),
                });
                if (name == "ReadControls" && _script is { } script)
                {
                    var a = script(_reads++, at => _program.ReadByte((ushort)at));
                    for (int i = 0; i < 4; i++) _program.WriteByte((ushort)(T.Action + i), unchecked((byte)a[i]));
                }
            });
            m.Mem.Write8(_program.Segment(seg), off, 0xCB); // retf: the caller removes the arguments
        }
        m.AddCodeHook(_program.Segment(0x2295), 0x0062, _ =>
        {
            if (_depth == 0) _calls.Add(TradeHarness.Format(nameof(TradeEventKind.NumberFill), (short)(_program.StackArgument(0) & 0xFF)));
        });
        foreach (var (off, retf, name) in Traced)
        {
            m.AddCodeHook(_program.Segment(0x0C29), off, _ =>
            {
                if (_depth++ > 0) return;
                _calls.Add(name switch
                {
                    nameof(TradeEventKind.DrawStock) => TradeHarness.Format(name, Player()),
                    nameof(TradeEventKind.DrawBasePrice) => TradeHarness.Format(name, (short)_program.ReadWord(T.BasePrice)),
                    nameof(TradeEventKind.DrawStockChange) => TradeHarness.Format(name, (short)_program.StackArgument(0), Player(),
                        Role(), Stock(), (short)_program.ReadWord((ushort)(T.ShownStock + 2 * Player()))),
                    _ => TradeHarness.Format(name),
                });
            });
            m.AddCodeHook(_program.Segment(0x0C29), retf, _ => _depth--);
        }
        m.AddCodeHook(_program.Segment(0x21FD), 0x01BA, _ => _randomCalls++);
    }

    private short Player() => (sbyte)_program.ReadByte(T.Player);

    private short Role() => _program.ReadByte((ushort)(T.Role + Player()));

    private short Position() => (short)_program.ReadWord((ushort)(T.Position + 2 * Player()));

    private ushort Cash() => (ushort)(DgroupLayout.Cash + 4 * Player());

    private short Stock() =>
        (short)_program.ReadWord((ushort)(DgroupLayout.Stock + 8 * Player() + 2 * (short)_program.ReadWord(T.Ware)));

    public sealed record Outcome(byte[] DataSegment, uint RandomState, IReadOnlyList<string> Calls, Exception? Fault, int Events);

    /// <summary>
    /// Calls the routine in the original on <paramref name="input"/>; <paramref name="script"/> writes the actions from the
    /// control read's number and the data segment (byte reader).
    /// </summary>
    public Outcome RunOriginal(Dgroup input, ushort segment, ushort offset, ushort[] args, Func<int, Func<int, byte>, sbyte[]>? script)
    {
        var m = _program.Machine;
        var full = (byte[])_initial.Clone();
        input.Bytes.AsSpan(0, DgroupLayout.Size).CopyTo(full);
        m.Mem.WriteBlock((uint)(_program.Dgroup * 16), full);
        m.Cpu.Ss = 0x9000;
        m.Cpu.Sp = 0xFFF0;
        _calls.Clear();
        _randomCalls = 0;
        _depth = 0;
        _reads = 0;
        _script = script;
        int before = m.Log.Events.Count;
        Exception? fault = null;
        try
        {
            _program.Call(segment, offset, args);
        }
        catch (EmulationException e)
        {
            fault = e;
        }
        Assert.Equal(0, _depth);
        return new Outcome(_program.DataSegment(), _program.RandomState, _calls.ToList(), fault, m.Log.Events.Count - before);
    }

    /// <summary>
    /// Runs original and port on the same input and asserts the same data segment, random state, recorded calls; returns the
    /// port's data segment and the number of frames the port waited.
    /// </summary>
    public (Dgroup Port, int Frames, IReadOnlyList<string> Calls) AssertSame(Dgroup input, ushort segment, ushort offset,
        Func<Dgroup, TradeHarness.Recorder, IEnumerable<Traders.Core.Field.FieldWait>> port, string label,
        Func<int, Func<int, byte>, sbyte[]>? script, params ushort[] args)
    {
        var original = RunOriginal(input, segment, offset, args, script);
        Assert.True(original.Fault is null, $"{label}: original faulted: {original.Fault?.Message}");
        Assert.True(original.Events == 0, $"{label}: original used {original.Events} host services");
        Assert.True(_randomCalls == 0, $"{label}: original drew random numbers");
        var ported = new Dgroup((byte[])input.Bytes.Clone());
        var recorder = new TradeHarness.Recorder(null, script);
        int frames = port(ported, recorder).Count();
        Assert.Equal(_initial[DgroupLayout.Size..], original.DataSegment[DgroupLayout.Size..]);
        var diff = Enumerable.Range(0, DgroupLayout.Size)
            .Where(i => original.DataSegment[i] != ported.Bytes[i])
            .Take(8)
            .Select(i => $"DS:{i:X4} original {original.DataSegment[i]:X2} port {ported.Bytes[i]:X2}")
            .ToList();
        Assert.True(diff.Count == 0, $"{label}: " + string.Join(", ", diff));
        Assert.Equal(input.U32(DgroupLayout.RandomSeed), original.RandomState);
        if (!original.Calls.SequenceEqual(recorder.Calls))
        {
            int k = original.Calls.Zip(recorder.Calls).TakeWhile(x => x.First == x.Second).Count();
            Assert.Fail($"{label}: calls differ at {k} of {original.Calls.Count}/{recorder.Calls.Count}\n" +
                        $" original: {string.Join(" ", original.Calls.Skip(Math.Max(0, k - 6)).Take(14))}\n" +
                        $" port:     {string.Join(" ", recorder.Calls.Skip(Math.Max(0, k - 6)).Take(14))}");
        }
        return (ported, frames, original.Calls);
    }
}
