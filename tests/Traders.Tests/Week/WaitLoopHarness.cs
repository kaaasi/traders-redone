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
            bool stub = seg is 0x0A96 or 0x0A69 or 0x21A7 or 0x23CD or 0x186C or 0x21AF
                || (seg == 0x0609 && off is 0x1FCC or 0x1ED9 or 0x1256 or 0x1879)
                || (seg == Seg21FD && Frames21FD.ContainsKey(off));
            if (stub) m.AddCodeHook(_program.Segment(seg), off, mm => Stub(mm, seg, off));
        }
        foreach (var (off, onZero) in PauseEdges)
            m.AddCodeHook(_program.Segment(Seg21FD), off, mm => { if (mm.Cpu.GetFlag(Cpu286.ZF) == onZero) NextFrame(); });
        m.AddCodeHook(_program.Segment(0x1F63), 0x104B,
            _ => _calls.Add(new(_frame, 0x1F63, 0x104B, _program.StackArgument(0), _program.StackArgument(1))));
        m.AddCodeHook(_program.Segment(0x0825), 0x0686, _ => _calls.Add(new(_frame, 0x0825, 0x0686, _program.StackArgument(0))));
        m.AddCodeHook(_program.Segment(Seg21FD), 0x01BA, _ => _random.Add((_frame, (short)_program.StackArgument(0))));
        m.AddCodeHook(_program.Segment(Seg21FD), 0x0004, _ => _inWaitLoop = true);
        foreach (ushort off in ReportDraws)
            m.AddCodeHook(_program.Segment(0x0825), off, _ => _calls.Add(new(_frame, 0x0825, off)));
    }

    /// <summary>Drawing routines of the week report, logged on entry (they run; their layer calls are stubbed).</summary>
    private static readonly ushort[] ReportDraws = [0x0072, 0x0178, 0x0364, 0x047D, 0x0560];

    /// <summary>
    /// 21FD routines stubbed with their retrace waits as frames (docs/routines/21FD_026C_paletten.md, as in the menu harness);
    /// the page flip 051F has its own rule below.
    /// </summary>
    private static readonly Dictionary<ushort, int> Frames21FD = new()
    {
        [0x051F] = 0, [0x0590] = 2, [0x03EB] = 1, [0x04DA] = 0, [0x026C] = 0, [0x0329] = 32, [0x05DB] = 0,
    };

    private bool _inWaitLoop;

    private void Stub(PcMachine m, ushort seg, ushort off)
    {
        var c = m.Cpu;
        ushort a0 = _program.StackArgument(0), a1 = _program.StackArgument(1);
        ushort ip = c.Pop(), cs = c.Pop();
        c.IP = ip;
        c.Cs = cs;
        // Only the calls the port reports; the drawing calls inside 1F63:104B and 0825:0686 (0A96:0030/0267) are covered by
        // the logged entries of those routines. List and object calls only inside the wait loop: the drawing routines of the
        // report call them many times, which is the scene's business.
        switch ((seg, off))
        {
            case (0x0A96, 0x06A8) or (0x0A96, 0x001B) or (0x23CD, 0x06E9) or (0x23CD, 0x071C):
                if (_inWaitLoop) _calls.Add(new(_frame, seg, off));
                break;
            case (0x21A7, 0x0054) or (Seg21FD, 0x03EB):
                _calls.Add(new(_frame, seg, off, a0, a1));
                break;
            case (0x186C, 0x0002) or (Seg21FD, 0x04DA) or (Seg21FD, 0x026C):
                _calls.Add(new(_frame, seg, off, a0));
                break;
            case (Seg21FD, 0x0590) or (Seg21FD, 0x0329) or (0x21AF, 0x04CD) or (0x0A96, 0x0C3F):
                _calls.Add(new(_frame, seg, off));
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
        if (seg == Seg21FD && Frames21FD.TryGetValue(off, out int n))
            for (int i = 0; i < n; i++) NextFrame();
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
    public Outcome RunOriginal(Dgroup input, int mode, Func<long, IReadOnlyCollection<byte>> keys) =>
        RunOriginal(input, Seg21FD, 0x0004, mode, keys);

    /// <summary>Calls the week report 0825:000A(<paramref name="arg"/>) in the original.</summary>
    public Outcome RunReportOriginal(Dgroup input, int arg, Func<long, IReadOnlyCollection<byte>> keys) =>
        RunOriginal(input, 0x0825, 0x000A, arg, keys);

    private Outcome RunOriginal(Dgroup input, ushort seg, ushort off, int arg, Func<long, IReadOnlyCollection<byte>> keys)
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
        _inWaitLoop = false;
        WriteKeys(keys(0));
        _program.Call(seg, off, (ushort)arg);
        return new Outcome(_program.DataSegment(), _frame, _random.ToList(), _calls.ToList());
    }

    public sealed record PortOutcome(Dgroup State, long Frames, IReadOnlyList<Call> Calls);

    /// <summary>Port run of the wait loop (see <see cref="Drive"/>).</summary>
    public static (PortOutcome Outcome, WaitLoop Loop) RunPort(Dgroup input, int mode, Func<long, IReadOnlyCollection<byte>> keys)
    {
        var d = new Dgroup((byte[])input.Bytes.Clone());
        var random = new OriginalRandom(new RecordedClock([]), d.U32(DgroupLayout.RandomSeed));
        var loop = new WaitLoop(d, random, KeyboardOnlyDevices.Instance, mode);
        long frames = Drive(d, random, loop.Run(), f => loop.Frame = f, keys);
        return (new PortOutcome(d, frames, Expand(loop.Events)), loop);
    }

    /// <summary>Port run of the week report (see <see cref="Drive"/>).</summary>
    public static (PortOutcome Outcome, Report Report) RunReportPort(Dgroup input, int arg, Func<long, IReadOnlyCollection<byte>> keys)
    {
        var d = new Dgroup((byte[])input.Bytes.Clone());
        var random = new OriginalRandom(new RecordedClock([]), d.U32(DgroupLayout.RandomSeed));
        var report = new Report(d, random, KeyboardOnlyDevices.Instance, arg);
        long frames = Drive(d, random, report.Run(), f => report.Frame = f, keys);
        return (new PortOutcome(d, frames, Expand(report.Events)), report);
    }

    /// <summary>
    /// Frame 0 runs the coroutine to its first wait, every later frame resumes it once with the keys of that frame written to
    /// the key table first; random() is synchronised with DS:10D2 around every resume. Returns the number of frames.
    /// </summary>
    private static long Drive(Dgroup d, OriginalRandom random, IEnumerable<MenuWait> coroutine, Action<long> setFrame,
        Func<long, IReadOnlyCollection<byte>> keys, long maxFrames = 20_000)
    {
        using var run = coroutine.GetEnumerator();
        long frame = 0;
        KeyTable.Write(d, keys(0));
        while (true)
        {
            setFrame(frame);
            random.Seed = d.U32(DgroupLayout.RandomSeed);
            bool more = run.MoveNext();
            d.Set(DgroupLayout.RandomSeed, random.Seed);
            if (!more) return frame;
            if (++frame > maxFrames) throw new InvalidOperationException("Port coroutine did not finish.");
            KeyTable.Write(d, keys(frame));
        }
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
                case WeekEventKind.LoadPicture: calls.Add(new(e.Frame, 0x186C, 0x0002, e.A)); break;
                case WeekEventKind.Draw: calls.Add(new(e.Frame, 0x0825, (ushort)e.A)); break;
                case WeekEventKind.DrawOnShownPage: calls.Add(new(e.Frame, Seg21FD, 0x0590)); break;
                case WeekEventKind.PaletteStep: calls.Add(new(e.Frame, Seg21FD, 0x03EB, e.A, e.B)); break;
                case WeekEventKind.BlendColor: calls.Add(new(e.Frame, Seg21FD, 0x04DA, e.A)); break;
                case WeekEventKind.InvalidateObjects: calls.Add(new(e.Frame, 0x21AF, 0x04CD)); break;
                case WeekEventKind.CopyPage: calls.Add(new(e.Frame, 0x0A96, 0x0C3F)); break;
                case WeekEventKind.LoadPalette: calls.Add(new(e.Frame, Seg21FD, 0x026C, e.A)); break;
                case WeekEventKind.FadeIn: calls.Add(new(e.Frame, Seg21FD, 0x0329)); break;
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
