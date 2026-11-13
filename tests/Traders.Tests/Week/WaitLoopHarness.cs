using Traders.Core;
using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.State;
using Traders.Core.Week;
using Traders.RefEmu;
using Traders.RefEmu.Cpu;
using Traders.RefEmu.Machine;

namespace Traders.Tests.Week;

/// <summary>
/// Runs the wait loop 21FD:0004 in the original (T.EXE loaded with <see cref="OriginalProgram.LoadOnly"/>) and the port on the
/// same data segment and the same keys per frame, and compares the data segment, random() and the calls to the layers.
///
/// Graphics, sound and objects are the scene's business, so in the original their far routines return at once (stubs: 0A96,
/// 0A69, 21A7, 23CD, 0609:1FCC and the page flip 21FD:051F); 1F63:104B and 0825:0686 run (their drawing calls are stubbed) and
/// are logged on entry. Frames as in <see cref="Menus.MenuHarness"/>: the page flip is two retraces (one with keypad plus, four
/// more with keypad minus), every taken back edge of the pause loop 21FD:0245 one; at each boundary the key table gets the
/// keys of the next frame. The port yields at exactly these points (<see cref="WaitLoop"/>).
/// </summary>
internal sealed class WaitLoopHarness
{
    private const ushort Seg21FD = 0x21FD;

    private static readonly (ushort Off, bool TakenOnZero)[] PauseEdges = [(0x0256, false), (0x025F, true), (0x0268, false)];

    private readonly OriginalProgram _program = OriginalProgram.LoadOnly(TestGameData.DosDir);
    private readonly byte[] _initial;
    private Func<long, IReadOnlyCollection<byte>> _keys = _ => [];
    private long _frame;
    private readonly List<Call> _calls = [];
    private readonly List<(long Frame, short Arg)> _random = [];

    /// <summary>A logged call of a layer routine: frame, address and the arguments that matter for the comparison.</summary>
    public readonly record struct Call(long Frame, ushort Seg, ushort Off, int A = 0, int B = 0)
    {
        public override string ToString() => $"frame {Frame} {Seg:X4}:{Off:X4}({A:X}, {B:X})";
    }

    public WaitLoopHarness()
    {
        _initial = _program.DataSegment();
        var m = _program.Machine;
        var exe = m.Files.Load("T.EXE") ?? throw new FileNotFoundException("T.EXE");
        foreach (var (seg, off) in MzLoader.FarCallTargets(exe))
        {
            bool stub = seg is 0x0A96 or 0x0A69 or 0x21A7 or 0x23CD || (seg == 0x0609 && off == 0x1FCC)
                || (seg == Seg21FD && off == 0x051F);
            if (stub) m.AddCodeHook(_program.Segment(seg), off, mm => Stub(mm, seg, off));
        }
        foreach (var (off, onZero) in PauseEdges)
            m.AddCodeHook(_program.Segment(Seg21FD), off, mm => { if (mm.Cpu.GetFlag(Cpu286.ZF) == onZero) NextFrame(); });
        m.AddCodeHook(_program.Segment(0x1F63), 0x104B,
            _ => _calls.Add(new(_frame, 0x1F63, 0x104B, _program.StackArgument(0), _program.StackArgument(1))));
        m.AddCodeHook(_program.Segment(0x0825), 0x0686, _ => _calls.Add(new(_frame, 0x0825, 0x0686, _program.StackArgument(0))));
        m.AddCodeHook(_program.Segment(Seg21FD), 0x01BA, _ => _random.Add((_frame, (short)_program.StackArgument(0))));
    }

    private void Stub(PcMachine m, ushort seg, ushort off)
    {
        var c = m.Cpu;
        ushort a0 = _program.StackArgument(0), a1 = _program.StackArgument(1);
        ushort ip = c.Pop(), cs = c.Pop();
        c.IP = ip;
        c.Cs = cs;
        // Only the calls the port reports; the drawing calls inside 1F63:104B and 0825:0686 (0A96:0030/0267) are covered by
        // the logged entries of those routines.
        switch ((seg, off))
        {
            case (0x0A96, 0x06A8) or (0x0A96, 0x001B) or (0x23CD, 0x06E9) or (0x23CD, 0x071C):
                _calls.Add(new(_frame, seg, off));
                break;
            case (0x21A7, 0x0054):
                _calls.Add(new(_frame, seg, off, a0, a1));
                break;
            case (0x0609, 0x1FCC):
                _calls.Add(new(_frame, seg, off, _program.ReadByte(DgroupLayout.ActivePlayer)));
                break;
            case (Seg21FD, 0x051F):
                _calls.Add(new(_frame, seg, off));
                NextFrame(); // show-page retrace, then DS:3D95 decides on 028F, then DS:3D91 on four more
                if (!Held(MenuKeys.KeypadPlus)) NextFrame();
                if (Held(MenuKeys.KeypadMinus))
                    for (int i = 0; i < 4; i++) NextFrame();
                break;
        }
    }

    private bool Held(byte sc) => _program.ReadByte((ushort)(DgroupLayout.KeyState + sc)) != 0;

    private void NextFrame()
    {
        _frame++;
        WriteKeys(_keys(_frame));
    }

    private void WriteKeys(IReadOnlyCollection<byte> held)
    {
        for (int sc = 0; sc < KeyTable.Length; sc++) _program.WriteByte((ushort)(DgroupLayout.KeyState + sc), 0);
        foreach (byte sc in held) _program.WriteByte((ushort)(DgroupLayout.KeyState + sc), 1);
    }

    /// <summary>Fresh copy of the data segment as loaded from T.EXE.</summary>
    public Dgroup Initial() => new(_initial[..DgroupLayout.Size]);

    public sealed record Outcome(byte[] DataSegment, long Frames, IReadOnlyList<(long Frame, short Arg)> Random,
        IReadOnlyList<Call> Calls);

    /// <summary>Calls 21FD:0004(<paramref name="mode"/>) in the original with <paramref name="input"/> and the keys per frame.</summary>
    public Outcome RunOriginal(Dgroup input, int mode, Func<long, IReadOnlyCollection<byte>> keys)
    {
        var m = _program.Machine;
        var full = (byte[])_initial.Clone();
        input.Bytes.AsSpan(0, DgroupLayout.Size).CopyTo(full);
        m.Mem.WriteBlock((uint)(_program.Dgroup * 16), full);
        m.Cpu.Ss = 0x9000;
        m.Cpu.Sp = 0xFFF0;
        _keys = keys;
        _frame = 0;
        _calls.Clear();
        _random.Clear();
        WriteKeys(keys(0));
        _program.Call(Seg21FD, 0x0004, (ushort)mode);
        return new Outcome(_program.DataSegment(), _frame, _random.ToList(), _calls.ToList());
    }

    public sealed record PortOutcome(Dgroup State, long Frames, WaitLoop Loop, IReadOnlyList<Call> Calls);

    /// <summary>
    /// Port run: frame 0 runs the coroutine to its first wait, every later frame resumes it once with the keys of that frame
    /// written to the key table first; random() is synchronised with DS:10D2 around every resume.
    /// </summary>
    public static PortOutcome RunPort(Dgroup input, int mode, Func<long, IReadOnlyCollection<byte>> keys, long maxFrames = 20_000)
    {
        var d = new Dgroup((byte[])input.Bytes.Clone());
        var random = new OriginalRandom(new RecordedClock([]), d.U32(DgroupLayout.RandomSeed));
        var loop = new WaitLoop(d, random, KeyboardOnlyDevices.Instance, mode);
        using var run = loop.Run().GetEnumerator();
        long frame = 0;
        KeyTable.Write(d, keys(0));
        while (true)
        {
            loop.Frame = frame;
            random.Seed = d.U32(DgroupLayout.RandomSeed);
            bool more = run.MoveNext();
            d.Set(DgroupLayout.RandomSeed, random.Seed);
            if (!more) break;
            if (++frame > maxFrames) throw new InvalidOperationException("Port wait loop did not finish.");
            KeyTable.Write(d, keys(frame));
        }
        return new PortOutcome(d, frame, loop, Expand(loop.Events));
    }

    /// <summary>The original calls each port event stands for.</summary>
    private static List<Call> Expand(IEnumerable<WeekEvent> events)
    {
        var calls = new List<Call>();
        foreach (var e in events)
        {
            switch (e.Kind)
            {
                case WeekEventKind.Flip: calls.Add(new(e.Frame, Seg21FD, 0x051F)); break;
                case WeekEventKind.ListExecute: calls.Add(new(e.Frame, 0x0A96, 0x06A8)); break;
                case WeekEventKind.ListClear: calls.Add(new(e.Frame, 0x0A96, 0x001B)); break;
                case WeekEventKind.RestoreObjects: calls.Add(new(e.Frame, 0x23CD, 0x071C)); break;
                case WeekEventKind.DrawObjects: calls.Add(new(e.Frame, 0x23CD, 0x06E9)); break;
                case WeekEventKind.Sound: calls.Add(new(e.Frame, 0x21A7, 0x0054, e.A, e.B)); break;
                case WeekEventKind.Indicator: calls.Add(new(e.Frame, 0x1F63, 0x104B, e.A, e.B)); break;
                case WeekEventKind.ShowPlayerObject: calls.Add(new(e.Frame, 0x0609, 0x1FCC, e.A)); break;
                case WeekEventKind.ReportMark:
                    calls.Add(new(e.Frame, 0x0825, 0x0686, e.A));
                    calls.Add(new(e.Frame, 0x0A96, 0x001B));
                    break;
                default: throw new InvalidOperationException(e.Kind.ToString());
            }
        }
        return calls;
    }

    /// <summary>Asserts byte equality of the data segment (first differences in the message).</summary>
    public static void AssertSameDs(byte[] original, Dgroup port, string label)
    {
        var diff = Enumerable.Range(0, DgroupLayout.Size)
            .Where(i => original[i] != port.Bytes[i])
            .Take(8)
            .Select(i => $"DS:{i:X4} original {original[i]:X2} port {port.Bytes[i]:X2}")
            .ToList();
        Assert.True(diff.Count == 0, $"{label}: " + string.Join(", ", diff));
    }
}
