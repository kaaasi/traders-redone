using Traders.Core;
using Traders.Core.Events;
using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.State;
using Traders.Core.Week;
using Traders.RefEmu;
using Traders.RefEmu.Cpu;
using Traders.RefEmu.Machine;

namespace Traders.Tests.Events;

/// <summary>
/// Runs the week incidents 17CB:0005 in the original (T.EXE loaded with <see cref="OriginalProgram.LoadOnly"/>) and the port on
/// the same data segment and the same keys per frame, and compares the data segment, random() and the calls to the layers.
///
/// Graphics, sound, palettes and waits are the scene's business, so in the original their far routines return at once (stubs:
/// 0A96, 0A69, 21A7, 23CD, 186C, 21AF, 2295, 0609:1FCC/1ED9/1256/1879 and in 21FD the page flip 051F, the fades 037D/0329, the
/// palette routines 026C/03EB/04DA/0590, the font 05DB and the wait 0185) and are logged with their arguments; the rules
/// (0825:06CB, 22A3:0750, 0609:10D2, the wait loop 21FD:0004 with 0E9E:037A) run. Frames as in
/// <see cref="Week.WaitLoopHarness"/>: page flip two retraces (keypad plus/minus), 0185(n) n retraces, the fades eight stages of
/// 0185(4) resp. 0185(3) plus the retrace of 2295:00C5 in VGA mode (DS:090E = 1), every taken back edge of the pause loop one.
/// The text pointers DS:37CC are replaced by marks (text i → 7000:i·100h) so the logged far pointers name the text index.
/// </summary>
internal sealed class IncidentsHarness
{
    private const ushort Seg21FD = 0x21FD, Seg17CB = 0x17CB;

    /// <summary>Segment of the marked text pointers; text i points to <see cref="TextSegment"/>:i·100h.</summary>
    public const ushort TextSegment = 0x7000;

    /// <summary>Number of text pointers replaced by marks (indices 0…6Fh, up to DS:398B).</summary>
    public const int MarkedTexts = 0x70;

    private static readonly (ushort Off, bool TakenOnZero)[] PauseEdges = [(0x0256, false), (0x025F, true), (0x0268, false)];

    private static readonly HashSet<ushort> Stubs21FD = [0x051F, 0x0590, 0x03EB, 0x04DA, 0x026C, 0x0329, 0x037D, 0x05DB, 0x0185];

    /// <summary>Branch points of 17CB, logged when reached (coverage of the scenarios in the original).</summary>
    public static readonly Dictionary<ushort, string> Branches = new()
    {
        [0x00F1] = "gain",
        [0x0294] = "loss",
        [0x020D] = "loss capped 3ED",
        [0x024E] = "loss capped half cash",
        [0x0286] = "loss not immune",
        [0x027F] = "loss computer check",
        [0x01E6] = "loss rank above 0",
        [0x01C0] = "loss rank 0",
        [0x00D5] = "gain needs random(9)",
        [0x016B] = "loss needs random(rank*5+8)",
        [0x05E8] = "takeover needs random(8)",
        [0x05F9] = "takeover",
        [0x07A0] = "blight",
        [0x090E] = "blight field emptied",
        [0x075F] = "blight rank 1",
        [0x0777] = "blight rank 0",
    };

    private readonly OriginalProgram _program = OriginalProgram.LoadOnly(TestGameData.DosDir);
    private readonly byte[] _initial;
    private Func<long, IReadOnlyCollection<byte>> _keys = _ => [];
    private long _frame;
    private readonly List<Call> _calls = [];
    private readonly List<(long Frame, short Arg)> _random = [];
    private readonly List<int> _clock = [];
    private readonly HashSet<string> _visited = [];

    /// <summary>A logged call of a layer routine: frame, address and the arguments that matter for the comparison.</summary>
    public readonly record struct Call(long Frame, ushort Seg, ushort Off, int A = 0, int B = 0, int C = 0, int D = 0)
    {
        public override string ToString() => $"frame {Frame} {Seg:X4}:{Off:X4}({A:X}, {B:X}, {C:X}, {D:X})";
    }

    public IncidentsHarness()
    {
        // The game called time() long before (code query 21FD:0687), so the runtime's time zone names (tzset, DS:5894…) are
        // set in every real state; one call here gives the inputs the same bytes.
        _program.Call(0x0000, 0x3405, 0, 0);
        _initial = _program.DataSegment();
        var m = _program.Machine;
        var exe = m.Files.Load("T.EXE") ?? throw new FileNotFoundException("T.EXE");
        foreach (var (seg, off) in MzLoader.FarCallTargets(exe))
        {
            bool stub = seg is 0x0A96 or 0x0A69 or 0x21A7 or 0x23CD or 0x186C or 0x21AF or 0x2295
                || (seg == 0x0609 && off is 0x1FCC or 0x1ED9 or 0x1256 or 0x1879)
                || (seg == Seg21FD && Stubs21FD.Contains(off));
            if (stub) m.AddCodeHook(_program.Segment(seg), off, mm => Stub(mm, seg, off));
        }
        foreach (var (off, onZero) in PauseEdges)
            m.AddCodeHook(_program.Segment(Seg21FD), off, mm => { if (mm.Cpu.GetFlag(Cpu286.ZF) == onZero) NextFrame(); });
        m.AddCodeHook(_program.Segment(Seg21FD), 0x01BA, _ => _random.Add((_frame, (short)_program.StackArgument(0))));
        // random(0): the clock value time() returned (AX:DX after 0000:3405), fed to the port as recorded clock
        m.AddCodeHook(_program.Segment(Seg21FD), 0x01CE, mm => _clock.Add(mm.Cpu.Ax | (mm.Cpu.Dx << 16)));
        foreach (var (off, name) in Branches)
            m.AddCodeHook(_program.Segment(Seg17CB), off, _ => _visited.Add(name));
    }

    private void Stub(PcMachine m, ushort seg, ushort off)
    {
        var c = m.Cpu;
        ushort ip = c.Pop(), cs = c.Pop();
        c.IP = ip;
        c.Cs = cs;
        Log(seg, off);
        if (seg != Seg21FD) return;
        switch (off)
        {
            case 0x051F:
                NextFrame(); // show-page retrace, then DS:3D95 decides on 028F, then DS:3D91 on four more
                if (!Held(MenuKeys.KeypadPlus)) NextFrame();
                if (Held(MenuKeys.KeypadMinus))
                    for (int i = 0; i < 4; i++) NextFrame();
                break;
            case 0x037D or 0x0329:
                int stage = (_program.ReadWord(0x090E) == 1 ? 1 : 0) + (off == 0x037D ? 4 : 3);
                for (int i = 0; i < 8 * stage; i++) NextFrame();
                break;
            case 0x0185:
                long n = (ushort)ArgAt(0) | ((long)(short)ArgAt(1) << 16);
                for (long i = 0; i < n; i++) NextFrame();
                break;
        }
    }

    // After the return address is popped the arguments start at SS:SP.
    private int ArgAt(int i) => _program.Machine.Mem.Read16(_program.Machine.Cpu.Ss, (ushort)(_program.Machine.Cpu.Sp + 2 * i));

    private void Log(ushort seg, ushort off)
    {
        switch ((seg, off))
        {
            case (0x21A7, 0x0054):
                _calls.Add(new(_frame, seg, off, ArgAt(0), ArgAt(1)));
                break;
            case (0x0609, 0x1256):
                _calls.Add(new(_frame, seg, off, ArgAt(0), ArgAt(1), ArgAt(2), ArgAt(3)));
                break;
            case (0x0609, 0x1879):
                _calls.Add(new(_frame, seg, off, ArgAt(0), ArgAt(1), ArgAt(2), (sbyte)_program.ReadByte(DgroupLayout.ActivePlayer)));
                break;
            case (0x2295, 0x0001):
                // number (long), width, far target = text pointer + 2
                _calls.Add(new(_frame, seg, off, ArgAt(0) | (ArgAt(1) << 16), ArgAt(2), TextIndex(ArgAt(3) - 2, ArgAt(4))));
                break;
            case (0x0A69, 0x0288):
                // 17CB:09E0: (0, 86h − 7·lines, 140h, far pointer to the table entry, lines, colour, 5)
                Assert.Equal(0x86 - 7 * (short)ArgAt(5), (short)ArgAt(1));
                Assert.Equal((0, 0x140, 5), (ArgAt(0), ArgAt(2), ArgAt(7)));
                Assert.Equal(_program.Dgroup, ArgAt(4));
                _calls.Add(new(_frame, seg, off, (ArgAt(3) - MenuLayout.TextPointers) / 4, ArgAt(5), ArgAt(6)));
                break;
            case (0x0A96, 0x0C3F) or (Seg21FD, 0x037D) or (Seg21FD, 0x05DB) or (Seg21FD, 0x026C):
                _calls.Add(new(_frame, seg, off, ArgAt(0)));
                break;
            case (Seg21FD, 0x0185):
                _calls.Add(new(_frame, seg, off, ArgAt(0) | (ArgAt(1) << 16)));
                break;
            default:
                _calls.Add(new(_frame, seg, off));
                break;
        }
    }

    private static int TextIndex(int offset, int segment)
    {
        Assert.Equal(TextSegment, segment);
        Assert.Equal(0, offset & 0xFF);
        return offset >> 8;
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

    /// <summary>Fresh copy of the data segment as loaded from T.EXE, with the text pointers replaced by marks.</summary>
    public Dgroup Initial()
    {
        var d = new Dgroup(_initial[..DgroupLayout.Size]);
        for (int i = 0; i < MarkedTexts; i++)
        {
            d.Set(MenuLayout.TextPointers + 4 * i, (short)(i << 8));
            d.Set(MenuLayout.TextPointers + 4 * i + 2, unchecked((short)TextSegment));
        }
        return d;
    }

    public sealed record Outcome(byte[] DataSegment, long Frames, IReadOnlyList<(long Frame, short Arg)> Random,
        IReadOnlyList<Call> Calls, IReadOnlyList<int> Clock, IReadOnlySet<string> Visited);

    /// <summary>Calls 17CB:0005 in the original with <paramref name="input"/> and the keys per frame.</summary>
    public Outcome RunOriginal(Dgroup input, Func<long, IReadOnlyCollection<byte>> keys)
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
        _clock.Clear();
        _visited.Clear();
        WriteKeys(keys(0));
        _program.Call(Seg17CB, 0x0005);
        return new Outcome(_program.DataSegment(), _frame, _random.ToList(), _calls.ToList(), _clock.ToList(), _visited.ToHashSet());
    }

    public sealed record PortOutcome(Dgroup State, long Frames, IReadOnlyList<Call> Calls, WeekIncidents Incidents);

    /// <summary>
    /// Port run: frame 0 runs the coroutine to its first wait, every later frame resumes it once with the keys of that frame
    /// written to the key table first; random() is synchronised with DS:10D2 around every resume.
    /// </summary>
    public static PortOutcome RunPort(Dgroup input, Func<long, IReadOnlyCollection<byte>> keys, IReadOnlyList<int> clock)
    {
        var d = new Dgroup((byte[])input.Bytes.Clone());
        var random = new OriginalRandom(new RecordedClock(clock), d.U32(DgroupLayout.RandomSeed));
        var incidents = new WeekIncidents(d, random, KeyboardOnlyDevices.Instance);
        using var run = incidents.Run().GetEnumerator();
        long frame = 0;
        KeyTable.Write(d, keys(0));
        while (true)
        {
            incidents.Frame = frame;
            random.Seed = d.U32(DgroupLayout.RandomSeed);
            bool more = run.MoveNext();
            d.Set(DgroupLayout.RandomSeed, random.Seed);
            if (!more) break;
            if (++frame > 50_000) throw new InvalidOperationException("Port coroutine did not finish.");
            KeyTable.Write(d, keys(frame));
        }
        return new PortOutcome(d, frame, Expand(incidents.Events), incidents);
    }

    /// <summary>The original calls each port event stands for.</summary>
    private static List<Call> Expand(IEnumerable<IncidentEvent> events)
    {
        var calls = new List<Call>();
        foreach (var e in events)
        {
            Call c = e.Kind switch
            {
                IncidentEventKind.FadeOut => new(e.Frame, Seg21FD, 0x037D, e.A),
                IncidentEventKind.ListClear => new(e.Frame, 0x0A96, 0x001B),
                IncidentEventKind.FieldBackdrop => new(e.Frame, 0x0609, 0x1ED9),
                IncidentEventKind.Frame => new(e.Frame, 0x0609, 0x1256, e.A, e.B, e.C, e.D),
                IncidentEventKind.ListExecute => new(e.Frame, 0x0A96, 0x06A8),
                IncidentEventKind.CopyPage => new(e.Frame, 0x0A96, 0x0C3F, e.A),
                IncidentEventKind.Font => new(e.Frame, Seg21FD, 0x05DB, e.A),
                IncidentEventKind.LoadPalette => new(e.Frame, Seg21FD, 0x026C, e.A),
                IncidentEventKind.FadeIn => new(e.Frame, Seg21FD, 0x0329),
                IncidentEventKind.Sound => new(e.Frame, 0x21A7, 0x0054, e.A, e.B),
                IncidentEventKind.Pause => new(e.Frame, Seg21FD, 0x0185, e.A),
                IncidentEventKind.Emblem => new(e.Frame, 0x0609, 0x1879, e.A, e.B, e.C, e.D),
                IncidentEventKind.Flip => new(e.Frame, Seg21FD, 0x051F),
                IncidentEventKind.Number => new(e.Frame, 0x2295, 0x0001, e.A, e.B, e.C),
                IncidentEventKind.Text => new(e.Frame, 0x0A69, 0x0288, e.A, e.B, e.C),
                IncidentEventKind.WaitLoop => Inner(e.Inner!),
                _ => throw new InvalidOperationException(e.Kind.ToString()),
            };
            calls.Add(c);
        }
        return calls;
    }

    private static Call Inner(WeekEvent e) => e.Kind switch
    {
        WeekEventKind.Flip => new(e.Frame, Seg21FD, 0x051F),
        WeekEventKind.ListExecute => new(e.Frame, 0x0A96, 0x06A8),
        WeekEventKind.ListClear => new(e.Frame, 0x0A96, 0x001B),
        WeekEventKind.RestoreObjects => new(e.Frame, 0x23CD, 0x071C),
        WeekEventKind.DrawObjects => new(e.Frame, 0x23CD, 0x06E9),
        WeekEventKind.Sound => new(e.Frame, 0x21A7, 0x0054, e.A, e.B),
        _ => throw new InvalidOperationException(e.Kind.ToString()),
    };

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
