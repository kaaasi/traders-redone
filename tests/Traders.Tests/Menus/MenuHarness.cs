using Traders.Core;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.RefEmu.Cpu;
using Traders.RefEmu.Machine;

namespace Traders.Tests.Menus;

/// <summary>
/// Runs a menu routine of 192D in the original (T.EXE loaded with <see cref="OriginalProgram.LoadOnly"/>) and the port on the
/// same data segment and the same keys per frame, and compares the data segment, random() and the calls to the layers.
///
/// Graphics, sound, resources, save file and joystick are the scene's resp. the host's business, so in the original their far
/// routines return at once (stubs: 0A96, 0A69, 21A7, 186C, 2336, 27C6:00E6 and the palette, fade, flip and font routines of
/// 21FD). Frames: the original's frame boundaries are the page flip 21FD:051F (two retraces: 0B67:0196/01B5 and 0B67:028F; one with keypad plus, four more with keypad minus held), the
/// fades (21FD:03B4 24, 0353 16, 0329 32 retraces) and every taken back edge of a key wait loop; at each boundary the key table
/// gets the keys of the next frame. The port yields at exactly these points (<see cref="MainMenu"/>), so frame numbers,
/// random() per frame and the order of the layer calls must agree.
/// </summary>
internal sealed class MenuHarness
{
    private const ushort Seg192D = 0x192D, Seg21FD = 0x21FD;

    /// <summary>Text buffer for ENGLISCH.TXT in free memory (segment, offset 0).</summary>
    public const ushort TextSegment = 0x7000;

    // Back edges of the key wait loops: (offset, branch taken when ZF is set). From 192D.asm: every jz/jnz back to the
    // "cmp byte [key],0" just before it, plus the dialog loop 12A6 (unconditional jmp at 1309).
    internal static readonly (ushort Off, bool TakenOnZero)[] WaitEdges192D =
    [
        (0x0270, false), (0x028A, false), (0x02A4, false), (0x02BE, false), (0x02D3, false), (0x02E8, false), (0x02FD, false),
        (0x0312, false), (0x0326, false), (0x033A, false),
        (0x04D4, false), (0x04E9, false), (0x04FE, false), (0x0513, false), (0x0528, false), (0x053D, false), (0x0552, false),
        (0x0566, false),
        (0x0AEC, false), (0x0B01, false), (0x0B16, false), (0x0B2B, false), (0x0B40, false), (0x0B55, false), (0x0B6F, false),
        (0x0B89, false), (0x0BA3, false), (0x0BBD, false), (0x0BD2, false), (0x0BE6, false),
        (0x112F, false), (0x1144, false), (0x1158, false),
        (0x1295, true), (0x129E, false),
        (0x1A40, false), (0x1A5A, false), (0x1A74, false), (0x1A8E, false), (0x1AA2, false),
    ];

    /// <summary>Pause 21FD:0245: wait for release, press, release.</summary>
    internal static readonly (ushort Off, bool TakenOnZero)[] WaitEdges21FD = [(0x0256, false), (0x025F, true), (0x0268, false)];

    /// <summary>21FD routines stubbed with their retrace waits as frames (0 = no frame).</summary>
    private static readonly Dictionary<ushort, int> Stubs21FD = new()
    {
        [0x026C] = 0, [0x0329] = 32, [0x0353] = 16, [0x03B4] = 24, [0x051F] = 2, [0x05DB] = 0,
    };

    private readonly OriginalProgram _program = OriginalProgram.LoadOnly(TestGameData.DosDir);
    private readonly byte[] _initial;
    private Func<long, IReadOnlyCollection<byte>> _keys = _ => [];
    private long _frame;

    public MenuHarness()
    {
        _initial = _program.DataSegment();
        var m = _program.Machine;
        var exe = m.Files.Load("T.EXE") ?? throw new FileNotFoundException("T.EXE");
        foreach (var (seg, off) in MzLoader.FarCallTargets(exe))
        {
            bool stub = seg is 0x0A96 or 0x0A69 or 0x21A7 or 0x186C or 0x2336 || (seg == 0x27C6 && off == 0x00E6)
                || (seg == Seg21FD && Stubs21FD.ContainsKey(off));
            if (stub) m.AddCodeHook(_program.Segment(seg), off, mm => Stub(mm, seg, off));
        }
        foreach (var (off, onZero) in WaitEdges192D)
            m.AddCodeHook(_program.Segment(Seg192D), off, mm => { if (mm.Cpu.GetFlag(Cpu286.ZF) == onZero) NextFrame(); });
        foreach (var (off, onZero) in WaitEdges21FD)
            m.AddCodeHook(_program.Segment(Seg21FD), off, mm => { if (mm.Cpu.GetFlag(Cpu286.ZF) == onZero) NextFrame(); });
        m.AddCodeHook(_program.Segment(Seg192D), 0x1309, _ => NextFrame());
        m.AddCodeHook(_program.Segment(Seg21FD), 0x01BA, _ => _draws.Add((_frame, (short)_program.StackArgument(0))));
    }

    private readonly List<(long Frame, short Arg)> _draws = [];
    private readonly List<(ushort Seg, ushort Off, ushort[] Args, ushort Caller)> _calls = [];

    /// <summary>Results the stubbed save/load and calibration routines return, in call order.</summary>
    public Queue<bool> LoadResults { get; } = new();

    public Queue<bool> SaveResults { get; } = new();

    public Queue<bool> CalibrationFailures { get; } = new();

    private void Stub(PcMachine m, ushort seg, ushort off)
    {
        var c = m.Cpu;
        var args = new ushort[8];
        for (int i = 0; i < args.Length; i++) args[i] = _program.StackArgument(i);
        ushort ip = c.Pop(), cs = c.Pop();
        ushort caller = (ushort)(cs - _program.Segment(0) == Seg192D ? ip : 0xFFFF);
        _calls.Add((seg, off, args, caller));
        int frames = 0;
        if (seg == Seg21FD && Stubs21FD.TryGetValue(off, out int n))
        {
            frames = n;

        }
        if (seg == 0x2336) c.Ax = (ushort)(Dequeue(off == 0x0539 ? LoadResults : SaveResults) ? 1 : 0);
        if (seg == 0x27C6) c.Ax = (ushort)(Dequeue(CalibrationFailures) ? 1 : 0);
        c.IP = ip;
        c.Cs = cs;
        if (seg == Seg21FD && off == 0x051F)
        {
            // show-page retrace, then 3D95 (keypad plus) decides on 028F, then 3D91 (keypad minus) on four more
            NextFrame();
            if (!Held(MenuKeys.KeypadPlus)) NextFrame();
            if (Held(MenuKeys.KeypadMinus))
                for (int i = 0; i < 4; i++) NextFrame();
            return;
        }
        for (int i = 0; i < frames; i++) NextFrame();
    }

    private static bool Dequeue(Queue<bool> q) => q.Count > 0 && q.Dequeue();

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

    public sealed record Outcome(byte[] DataSegment, long Frames, IReadOnlyList<(long Frame, short Arg)> Draws,
        IReadOnlyList<(ushort Seg, ushort Off, ushort[] Args, ushort Caller)> Calls, int Ax);

    /// <summary>Calls the original routine with <paramref name="input"/> as data segment and the keys per frame.</summary>
    public Outcome RunOriginal(Dgroup input, ushort offset, Func<long, IReadOnlyCollection<byte>> keys)
    {
        var m = _program.Machine;
        var full = (byte[])_initial.Clone();
        input.Bytes.AsSpan(0, DgroupLayout.Size).CopyTo(full);
        m.Mem.WriteBlock((uint)(_program.Dgroup * 16), full);
        m.Cpu.Ss = 0x9000;
        m.Cpu.Sp = 0xFFF0;
        _keys = keys;
        _frame = 0;
        _draws.Clear();
        _calls.Clear();
        WriteKeys(keys(0));
        var (ax, _) = _program.Call(Seg192D, offset);
        return new Outcome(_program.DataSegment(), _frame, _draws.ToList(), _calls.ToList(), ax);
    }

    /// <summary>Writes a text buffer to the original's memory at <see cref="TextSegment"/>:0000.</summary>
    public void WriteTextBuffer(byte[] buffer) => _program.Machine.Mem.WriteBlock((uint)(TextSegment * 16), buffer);

    public byte[] ReadTextBuffer(int length) => _program.Machine.Mem.ReadBlock(TextSegment, 0, length);

    /// <summary>
    /// Data segment with the text table built (ENGLISCH.TXT at <see cref="TextSegment"/>, 192D:18B6 run in the original with the
    /// loader stubbed), as the menus need it for their texts.
    /// </summary>
    public Dgroup WithTexts(Dgroup d)
    {
        d.Set(MenuLayout.TextBuffer, (ushort)0);
        d.Set(MenuLayout.TextBuffer + 2, TextSegment);
        WriteTextBuffer(File.ReadAllBytes(TestGameData.Dos("ENGLISCH.TXT")));
        var o = RunOriginal(d, 0x18B6, _ => []);
        return new Dgroup(o.DataSegment[..DgroupLayout.Size]);
    }

    /// <summary>Port run: the main menu coroutine with the same keys per frame.</summary>
    public sealed class PortRun
    {
        public required Dgroup State { get; init; }
        public required MenuRunner Runner { get; init; }
        public required List<(long Frame, int Count)> DrawsPerFrame { get; init; }
    }

    public static PortRun RunPort(Dgroup input, Func<long, IReadOnlyCollection<byte>> keys, IMenuHost host, long maxFrames = 100_000)
    {
        var d = new Dgroup((byte[])input.Bytes.Clone());
        var random = new OriginalRandom(new RecordedClock([]), d.U32(DgroupLayout.RandomSeed));
        var runner = new MenuRunner(d, random, host);
        var perFrame = new List<(long, int)>();
        long calls = 0;
        runner.Start(0, keys(0));
        perFrame.Add((0, (int)(random.Calls - calls)));
        calls = random.Calls;
        for (long f = 1; !runner.Finished; f++)
        {
            if (f > maxFrames) throw new InvalidOperationException("Port menu did not finish.");
            runner.Advance(new PhaseTick(f, keys(f)));
            perFrame.Add((f, (int)(random.Calls - calls)));
            calls = random.Calls;
        }
        return new PortRun { State = d, Runner = runner, DrawsPerFrame = perFrame };
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

/// <summary>Host for the port with scripted save/load and calibration results (same queues as the original's stubs).</summary>
internal sealed class ScriptedMenuHost : IMenuHost
{
    public Queue<bool> LoadResults { get; } = new();
    public Queue<bool> SaveResults { get; } = new();
    public Queue<bool> CalibrationFailures { get; } = new();

    public bool LoadGame(Dgroup state) => LoadResults.Count > 0 && LoadResults.Dequeue();

    public bool SaveGame(Dgroup state) => SaveResults.Count > 0 && SaveResults.Dequeue();

    public bool CalibrationFailed(int stick, int step) => CalibrationFailures.Count > 0 && CalibrationFailures.Dequeue();
}

/// <summary>Key script: presses as (first frame, number of frames held, scancode).</summary>
internal sealed class KeyScript
{
    private readonly List<(long From, long To, byte Key)> _holds = [];

    /// <summary>The holds as added: key held in the frames [From, To).</summary>
    public IReadOnlyList<(long From, long To, byte Key)> Holds => _holds;

    public KeyScript Hold(long from, long frames, byte key)
    {
        _holds.Add((from, from + frames, key));
        return this;
    }

    /// <summary>Presses the keys one after another, each held <paramref name="hold"/> frames, <paramref name="gap"/> frames apart.</summary>
    public static KeyScript Sequence(long start, long gap, long hold, params byte[] keys)
    {
        var s = new KeyScript();
        for (int i = 0; i < keys.Length; i++) s.Hold(start + i * gap, hold, keys[i]);
        return s;
    }

    public IReadOnlyCollection<byte> At(long frame) =>
        _holds.Where(h => frame >= h.From && frame < h.To).Select(h => h.Key).Distinct().ToArray();
}
