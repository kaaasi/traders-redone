using System.Text;
using Traders.Core;
using Traders.Core.Events;
using Traders.Core.Field;
using Traders.Core.Purchase;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.RefEmu.Cpu;
using P = Traders.Core.Purchase.PurchaseLayout;

namespace Traders.Tests.Purchase;

/// <summary>
/// Runs a routine of the purchase 08CF once in the original (T.EXE loaded with <see cref="OriginalProgram.LoadOnly"/>, routine
/// called with a prepared data segment) and once in the port on the same bytes. Sound, the drawing routines of 08CF, 0609 and the
/// graphics driver are replaced by a far return in the original and recorded with their arguments (the drawing routines of 08CF
/// with the data segment values they read); the port records its events in the same form. 0609:1FE0, 0609:1B72, 0609:13C5 and
/// 0825:06CB run (they change the data segment or draw random numbers); the drawing inside 13C5 is not recorded. Compared: the
/// whole data segment, the random state and the number of draws, the recorded calls and the result. Expected values come only
/// from the original run.
/// </summary>
internal sealed class PurchaseHarness
{
    /// <summary>Segment of the test texts behind the far pointers DS:37D0 + 4k (k = 16 … 22).</summary>
    private const ushort TextSegment = 0x8000;

    /// <summary>Test texts of different lengths (not the game's texts); text k at TextSegment:(k − 16) · 20h.</summary>
    private static readonly string[] TestTexts = ["AB", "CDE", "F", "GHIJKL", "MNOPQRSTU", "VW", "XYZ0123"];

    private readonly OriginalProgram _program = OriginalProgram.LoadOnly(TestGameData.DosDir);
    private readonly byte[] _initial;
    private readonly List<string> _calls = [];
    private readonly List<short> _randomArguments = [];
    private bool _inBorder;

    /// <summary>Calls replaced by retf: (segment, offset, argument words, name).</summary>
    private static readonly (ushort Seg, ushort Off, int Args, string Name)[] Stubs =
    [
        (0x21A7, 0x0054, 2, nameof(PurchaseEventKind.Sound)),
        (0x0609, 0x1879, 3, nameof(PurchaseEventKind.Emblem)),
        (0x0609, 0x18E5, 0, nameof(PurchaseEventKind.EmptyPanel)),
        (0x0609, 0x1978, 0, nameof(PurchaseEventKind.DrawRockets)),
        (0x0609, 0x1AC0, 0, nameof(PurchaseEventKind.DrawOilBar)),
        (0x0609, 0x1ED9, 0, nameof(PurchaseEventKind.DrawBackground)),
        (0x0609, 0x1256, 4, nameof(PurchaseEventKind.DrawFrame)),
        (0x0609, 0x1A03, 0, nameof(FieldEventKind.DrawPanel)),
        (0x0A96, 0x001B, 0, nameof(PurchaseEventKind.ClearList)),
        (0x0A96, 0x06A8, 0, nameof(PurchaseEventKind.RunList)),
        (0x0A96, 0x0C3F, 1, nameof(PurchaseEventKind.ShowPage)),
        (0x0A96, 0x0030, 6, nameof(PurchaseEventKind.Blit)),
        (0x0A96, 0x0267, 8, nameof(PurchaseEventKind.Tiles)),
        (0x0A96, 0x0433, 5, nameof(PurchaseEventKind.Line)),
        (0x0A96, 0x04D7, 5, nameof(PurchaseEventKind.FillRect)),
        (0x0A96, 0x0525, 6, nameof(PurchaseEventKind.Text)),
        (0x0A96, 0x057A, 7, nameof(PurchaseEventKind.Number)),
        (0x0A96, 0x05FB, 7, nameof(PurchaseEventKind.Money)),
        (0x0A69, 0x004D, 8, nameof(PurchaseEventKind.PriceText)),
        (0x0A69, 0x0164, 9, nameof(PurchaseEventKind.PriceNumber)),
        (0x08CF, 0x193E, 0, nameof(PurchaseEventKind.DrawCash)),
        (0x08CF, 0x1981, 0, nameof(PurchaseEventKind.DrawShopOil)),
        (0x08CF, 0x1859, 0, nameof(PurchaseEventKind.DrawGemCredit)),
        (0x08CF, 0x172A, 0, nameof(PurchaseEventKind.DrawGems)),
        (0x08CF, 0x16B4, 2, nameof(PurchaseEventKind.DrawTicket)),
        (0x08CF, 0x16F5, 0, nameof(PurchaseEventKind.ClearTicket)),
        (0x08CF, 0x15C6, 2, nameof(PurchaseEventKind.DrawShop)),
        (0x08CF, 0x15F5, 3, nameof(PurchaseEventKind.DrawDecoration)),
        (0x08CF, 0x1688, 0, nameof(PurchaseEventKind.ClearBalls)),
        (0x08CF, 0x18B5, 0, nameof(PurchaseEventKind.DrawClock)),
    ];

    public PurchaseHarness()
    {
        _initial = _program.DataSegment();
        var m = _program.Machine;
        foreach (var (seg, off, args, name) in Stubs)
        {
            m.AddCodeHook(_program.Segment(seg), off, _ =>
            {
                if (_inBorder) return;
                short[] a = Enumerable.Range(0, args).Select(i => (short)_program.StackArgument(i)).ToArray();
                if (name == nameof(PurchaseEventKind.Text) && a[3] == (short)_program.Dgroup) a[3] = PurchaseRules.DataSegment;
                _calls.Add(name switch
                {
                    nameof(PurchaseEventKind.EmptyPanel) or nameof(PurchaseEventKind.DrawOilBar) or nameof(PurchaseEventKind.ClearTicket)
                        => Format(name, Player()),
                    nameof(PurchaseEventKind.DrawRockets) => Format(name, Player(), Word(DgroupLayout.Rockets + 2 * Player())),
                    nameof(PurchaseEventKind.DrawCash) => Format(name, Player(), Word(DgroupLayout.Cash + 4 * Player()),
                        Word(DgroupLayout.Cash + 4 * Player() + 2)),
                    nameof(PurchaseEventKind.DrawShopOil) => Format(name, Word(DgroupLayout.ShopOil)),
                    nameof(PurchaseEventKind.DrawGemCredit) => Format(name, Player(), Word(DgroupLayout.GemCredit + 2 * Player())),
                    nameof(PurchaseEventKind.DrawGems) => Format(name, Player(), (sbyte)_program.ReadByte(
                        (ushort)(DgroupLayout.Gems + Player()))),
                    nameof(PurchaseEventKind.DrawTicket) => Format(name, a[0], a[1], Player()),
                    nameof(PurchaseEventKind.DrawClock) => Format(name, Word(P.Remaining)),
                    _ => Format(name, a),
                });
            });
            m.Mem.Write8(_program.Segment(seg), off, 0xCB); // retf: the caller removes the arguments
        }
        // 0609:13C5 runs (its random draws); only its call is recorded, the drawing inside up to the return to 08CF:1382 not.
        m.AddCodeHook(_program.Segment(0x0609), 0x13C5, _ =>
        {
            _calls.Add(Format("Border", Enumerable.Range(0, 4).Select(i => (short)_program.StackArgument(i)).ToArray()));
            _inBorder = true;
        });
        m.AddCodeHook(_program.Segment(0x08CF), 0x1382, _ => _inBorder = false);
        m.AddCodeHook(_program.Segment(0x21FD), 0x01BA, _ => _randomArguments.Add((short)_program.StackArgument(0)));
        for (int k = 0; k < TestTexts.Length; k++)
        {
            var bytes = Encoding.ASCII.GetBytes(TestTexts[k] + "\0");
            for (int i = 0; i < bytes.Length; i++) m.Mem.Write8(TextSegment, (ushort)(0x20 * k + i), bytes[i]);
        }
    }

    private short Player() => (sbyte)_program.ReadByte(P.Player);

    private short Word(int offset) => (short)_program.ReadWord((ushort)offset);

    internal static string Format(string name, params short[] args) => $"{name}({string.Join(", ", args)})";

    /// <summary>Fresh copy of the data segment as loaded from T.EXE.</summary>
    public Dgroup Initial() => new(_initial[..DgroupLayout.Size]);

    /// <summary>Points the texts 16 … 22 at the test texts (both runs read them there).</summary>
    public static void UseTestTexts(Dgroup d)
    {
        for (int k = 0; k < TestTexts.Length; k++)
        {
            d.Set(P.TextTable + 4 * (16 + k), (ushort)(0x20 * k));
            d.Set(P.TextTable + 4 * (16 + k) + 2, TextSegment);
        }
    }

    public sealed record Outcome(byte[] DataSegment, uint RandomState, IReadOnlyList<string> Calls, int Result, Exception? Fault,
        int Events, int Draws);

    /// <summary>Calls the routine in the original on <paramref name="input"/>.</summary>
    public Outcome RunOriginal(Dgroup input, ushort segment, ushort offset, ushort[] args)
    {
        var m = _program.Machine;
        var full = (byte[])_initial.Clone();
        input.Bytes.AsSpan(0, DgroupLayout.Size).CopyTo(full);
        m.Mem.WriteBlock((uint)(_program.Dgroup * 16), full);
        m.Cpu.Ss = 0x9000;
        m.Cpu.Sp = 0xFFF0;
        _calls.Clear();
        _randomArguments.Clear();
        _inBorder = false;
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
            _randomArguments.Count);
    }

    /// <summary>
    /// Runs original and port on the same input and asserts the same data segment, random state and number of draws, recorded
    /// calls and (if <paramref name="port"/> returns one) result under <paramref name="mask"/>. Returns the port's data segment.
    /// </summary>
    public Dgroup AssertSame(Dgroup input, ushort segment, ushort offset, Func<Dgroup, OriginalRandom, Recorder, int?> port,
        string label, int mask = -1, params ushort[] args)
    {
        var original = RunOriginal(input, segment, offset, args);
        Assert.True(original.Fault is null, $"{label}: original faulted: {original.Fault?.Message}");
        Assert.True(original.Events == 0, $"{label}: original used {original.Events} host services");
        var ported = new Dgroup((byte[])input.Bytes.Clone());
        var random = new OriginalRandom(new RecordedClock([]), input.U32(DgroupLayout.RandomSeed));
        var recorder = new Recorder();
        int? result = port(ported, random, recorder);
        ported.Set(DgroupLayout.RandomSeed, random.Seed);
        if (result is int r)
            Assert.True((r & mask) == (original.Result & mask), $"{label}: result original {original.Result & mask}, port {r & mask}");
        Assert.Equal(_initial[DgroupLayout.Size..], original.DataSegment[DgroupLayout.Size..]);
        var diff = Enumerable.Range(0, DgroupLayout.Size)
            .Where(i => original.DataSegment[i] != ported.Bytes[i])
            .Take(8)
            .Select(i => $"DS:{i:X4} original {original.DataSegment[i]:X2} port {ported.Bytes[i]:X2}")
            .ToList();
        Assert.True(diff.Count == 0, $"{label}: " + string.Join(", ", diff));
        Assert.Equal(original.RandomState, random.Seed);
        Assert.Equal(original.Draws, (int)random.Calls);
        Assert.True(original.Calls.SequenceEqual(recorder.Calls),
            $"{label}: calls differ\n original: {string.Join(" ", original.Calls)}\n port:     {string.Join(" ", recorder.Calls)}");
        return ported;
    }

    /// <summary>The port's output: records like the stubs of the original.</summary>
    internal sealed class Recorder : IPurchaseOutput
    {
        public List<string> Calls { get; } = [];

        public ITextMemory Texts { get; } = new TestTextMemory();

        public void Emit(PurchaseEvent e) => Calls.Add(Format(e.Kind.ToString(), e.Args));

        public void Emit(FieldEvent e) => Calls.Add(e.Kind switch
        {
            FieldEventKind.Sound => Format(nameof(PurchaseEventKind.Sound), e.Args),
            FieldEventKind.DrawCommand => Format(nameof(PurchaseEventKind.Blit), e.Args),
            FieldEventKind.DrawPanel => Format(nameof(FieldEventKind.DrawPanel)),
            FieldEventKind.DrawBorder => Format("Border", e.Args[..4]),
            _ => throw new InvalidOperationException($"unexpected field event {e}"),
        });

        public void ComputerTurn(Dgroup d, short x, short y) => throw new InvalidOperationException("no computer turn in 08CF");
    }

    /// <summary>The test texts at TextSegment (read only; anything else reads as 0).</summary>
    private sealed class TestTextMemory : ITextMemory
    {
        public byte Read(ushort segment, ushort offset)
        {
            if (segment != TextSegment || offset / 0x20 >= TestTexts.Length) return 0;
            string t = TestTexts[offset / 0x20];
            return offset % 0x20 < t.Length ? (byte)t[offset % 0x20] : (byte)0;
        }

        public void Write(ushort segment, ushort offset, byte value) => throw new InvalidOperationException("texts are read only");
    }
}
