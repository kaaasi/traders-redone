using Traders.Core;
using Traders.Core.Events;
using Traders.Core.Field;
using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.Purchase;
using Traders.Core.Rules;
using Traders.Core.State;
using Traders.Assets;
using Traders.Presentation.Menus;
using Traders.Presentation.Purchase;
using Traders.RefEmu;
using Traders.RefEmu.Machine;
using Traders.Tests.Field;
using Traders.Tests.Startup;
using Traders.Vga;
using Traders.Vga.Layers;

namespace Traders.Tests.Purchase;

/// <summary>
/// The purchase 08CF:0004 frame by frame in a running game against the original. The original runs from a snapshot at the
/// entry of 08CF:0004 (call 29 of the week loop, reached by the game itself with four computers) with live graphics and sound;
/// keys reach it as real keyboard interrupts at emulated times. A frame of the original is one retrace wait (entry into
/// 0D73:00D3). The port runs <see cref="PurchaseStep"/> with the production host <see cref="PurchaseHost"/> in a
/// <see cref="PhaseMachine"/> from call 29, one <see cref="PhaseTick"/> per frame, with the key table the original held at the
/// end of the retrace before. At the k-th retrace of the original the port must wait its k-th frame; there the data segment
/// (outside <see cref="Owned"/>), DS:10D2, the number of random() calls and the picture of the port's <see cref="PurchaseScene"/>
/// (VRAM planes, DAC, CRTC start) are compared; at the return once more. Every command list the purchase's flow executes
/// (0A96:06A8 called from segment 08CF) is compared with the original's before it is executed.
/// </summary>
internal static class PurchaseFrameHarness
{
    private const ushort SegRetrace = 0x0D73;

    /// <summary>Phase of a key event within a frame (0 = frame start; the retrace starts at the frame start).</summary>
    private const double KeyPhase = 0.4;

    /// <summary>A key held from frame <c>From</c> for <c>Frames</c> frames (70 Hz, counted from the purchase's start).</summary>
    public sealed record Press(long From, long Frames, byte Key);

    /// <summary>
    /// A run: the snapshot (week of the game), players made human (control 4: arrows, Ins), bytes set in the data segment at
    /// the entry (both sides), key presses.
    /// </summary>
    public sealed record Script(int Week, int[] Humans, Press[] Presses, (int Offset, byte[] Bytes)[]? Set = null);

    /// <summary>
    /// Data segment bytes the original writes in the purchase that the core leaves to the output layers (those of the planting
    /// phase's output, which the purchase's drawing shares) and the bytes of the interrupt handlers.
    /// </summary>
    public static readonly (int Offset, int Length, string Owner)[] Owned =
    [
        .. PlantingFrameHarness.Owned,
    ];

    private const long MaxFrames = 20_000;

    private static readonly Dictionary<int, Lazy<MachineSnapshot>> AtPurchase = new()
    {
        [0] = new(() => Snapshot(0)),
        [2] = new(() => Snapshot(2)),
    };

    /// <summary>Default game (four computers) run to the entry of 08CF:0004 in week <paramref name="week"/>.</summary>
    private static MachineSnapshot Snapshot(int week)
    {
        var p = OriginalProgram.StartGame(TestGameData.DosDir);
        while (true)
        {
            Assert.True(p.RunToRoutine(0x08CF, 0x0004, 1800), $"08CF:0004 not reached (week {p.Week})");
            if (p.Week == week) return p.Snapshot();
            Assert.True(p.Week < week, $"week {week} skipped");
        }
    }

    /// <param name="Retraces">Frames of the purchase.</param>
    /// <param name="Passes">Passes of the shop loop 08CF:01BD.</param>
    /// <param name="Sounds">Sound requests (channel, sound) of the port with their count.</param>
    /// <param name="Kinds">Requests of the port by kind (purchase and field requests).</param>
    /// <param name="Log">Sound requests of channel 0 with the frame, the pass and the player DS:37AA (diagnostics).</param>
    /// <param name="PicturesCompared">Frames whose picture was compared.</param>
    /// <param name="DistinctPictures">Different pictures of the original among them.</param>
    /// <param name="ListsCompared">Command lists of the purchase's flow compared, <c>NonEmptyLists</c> of them with entries.</param>
    public sealed record Result(int Retraces, int Passes, long RandomCalls, byte[] Entry, byte[] DataSegment,
        IReadOnlyDictionary<(short Channel, short Sound), int> Sounds, IReadOnlyDictionary<string, int> Kinds,
        IReadOnlyList<string> Log, int PicturesCompared, int DistinctPictures, int ListsCompared, int NonEmptyLists);

    /// <summary>
    /// Counter-tests, mutations of the port: <c>DoubleFrame</c>: frame 300 processed twice (two frames in one tick);
    /// <c>CountdownHeld</c>: the countdown tick of the 200th pass does not count (one tick more before the step). Scene:
    /// <c>SkipClock</c>: the third clock request is not drawn; <c>SkipCash</c>: the first cash request is not drawn;
    /// <c>SkipTicket</c>: the first lotto number is not drawn.
    /// </summary>
    public enum Mutation { None, DoubleFrame, CountdownHeld, SkipClock, SkipCash, SkipTicket }

    /// <summary>Real-mode memory as a flat copy (the texts behind the far pointers DS:37D0 + 4k).</summary>
    private sealed class LinearTexts(byte[] memory) : ITextMemory
    {
        public byte Read(ushort segment, ushort offset) => memory[((segment << 4) + offset) & 0xFFFFF];

        public void Write(ushort segment, ushort offset, byte value) => throw new InvalidOperationException("texts are read only");
    }

    /// <summary>Retrace wait of the port's layers: notes the port frame of every wait.</summary>
    private sealed class FrameRetraces : IRetraceWait
    {
        public long CurrentFrame { get; set; }

        public List<long> Frames { get; } = [];

        public void WaitForRetrace() => Frames.Add(CurrentFrame);
    }

    /// <summary>Picture resources (186C:0002) from the original files.</summary>
    private sealed class Pictures : IPictureSource
    {
        public byte[] Decrunched(int resource) =>
            PowerPacker.Decrunch(File.ReadAllBytes(TestGameData.Dos(ResourceTable.Pictures[resource].Vga)));
    }

    /// <summary>Real-mode memory as a flat copy (texts outside DS) for the layers.</summary>
    private sealed class LinearMemory(byte[] memory) : IFarMemory
    {
        public byte Read(ushort segment, ushort offset) => memory[((segment << 4) + offset) & 0xFFFFF];

        public void Write(ushort segment, ushort offset, byte value) => memory[((segment << 4) + offset) & 0xFFFFF] = value;
    }

    private sealed record PortOutput(PurchaseScene Scene, ModeXDriver Driver, VgaDac Dac, FrameRetraces Retraces);

    private static PortOutput SetUpOutput(OriginalProgram o, Dgroup core)
    {
        var retraces = new FrameRetraces();
        var driver = FieldSceneHarness.DriverFrom(o);
        var dac = new VgaDac();
        o.Machine.Vga.Dac.CopyTo(dac.Registers, 0);
        var layers = new GraphicsLayers(driver, o.DataSegment(), dac, retraces)
            { DataSegment = o.Dgroup, OtherMemory = new LinearMemory(o.Machine.Mem.ReadBlock(0, 0, 0x100000)) };
        return new PortOutput(new PurchaseScene(layers, core, new Pictures()), driver, dac, retraces);
    }

    /// <summary>Picture state: VRAM planes, DAC, CRTC start (first difference named).</summary>
    private static void ComparePicture(OriginalProgram o, PortOutput port, string where)
    {
        var vga = o.Machine.Vga;
        Assert.True(vga.StartAddress == port.Driver.DisplayStart,
            $"{where}: display start original {vga.StartAddress:X4}, port {port.Driver.DisplayStart:X4}");
        for (int pl = 0; pl < 4; pl++)
        {
            var a = vga.Planes[pl].AsSpan();
            var b = port.Driver.Vram.Planes[pl].AsSpan();
            int k = a.CommonPrefixLength(b);
            if (k < a.Length)
                Assert.Fail($"{where}: plane {pl} offset {k:X4} (page {k / 0x4000}, x {(k % 0x4000) % 80 * 4 + pl}, y {k % 0x4000 / 80}) " +
                            $"original {a[k]:X2} port {b[k]:X2}");
        }
        int d = vga.Dac.AsSpan().CommonPrefixLength(port.Dac.Registers);
        if (d < port.Dac.Registers.Length)
            Assert.Fail($"{where}: DAC colour {d / 3} component {d % 3} original {vga.Dac[d]:X2} port {port.Dac.Registers[d]:X2}");
    }

    private static ulong PictureHash(OriginalProgram o) =>
        Traders.Tests.Presentation.MenuSceneHarness.Hash(o.Machine.Vga.Planes, o.Machine.Vga.Dac, o.Machine.Vga.StartAddress);

    /// <summary>The command list before execution: count DS:15BE, then the entries from DS:15C0.</summary>
    private static byte[] ListBytes(byte[] ds)
    {
        int n = ds[GraphicsLayers.ListCount] | ds[GraphicsLayers.ListCount + 1] << 8;
        return ds.AsSpan(GraphicsLayers.ListCount, 2 + n * GraphicsLayers.EntrySize).ToArray();
    }

    public static Result Run(Script script, Mutation mutation = Mutation.None)
    {
        var o = OriginalProgram.Resume(TestGameData.DosDir, AtPurchase[script.Week].Value);
        var m = o.Machine;
        Assert.True(o.IsAt(0x08CF, 0x0004), "snapshot not at 08CF:0004");
        ushort retIp = m.Mem.Read16(m.Cpu.Ss, m.Cpu.Sp), retCs = m.Mem.Read16(m.Cpu.Ss, (ushort)(m.Cpu.Sp + 2));
        foreach (int h in script.Humans)
        {
            o.WriteByte((ushort)(ControlReader.IsComputer + h), 0);
            o.WriteByte((ushort)(ControlReader.ControlType + h), 4);
        }
        foreach (var (at, bytes) in script.Set ?? [])
            for (int i = 0; i < bytes.Length; i++) o.WriteByte((ushort)(at + i), bytes[i]);

        long period = m.Vga.FramePeriod;
        long frame0 = m.FrameNumber + 1;
        double Time(long frame) => (double)((frame0 + frame) * period + (long)(KeyPhase * period)) / Traders.RefEmu.Hardware.Pit.ClockHz;
        foreach (var press in script.Presses)
        {
            m.ScheduleScancode(Time(press.From), press.Key);
            m.ScheduleScancode(Time(press.From + press.Frames), (byte)(press.Key | 0x80));
        }

        var run = new OriginalRun(o, retCs, retIp);
        var entry = o.DataSegment()[..DgroupLayout.Size];
        var core = new Dgroup((byte[])entry.Clone());
        var random = new OriginalRandom(new RecordedClock([]), core.U32(DgroupLayout.RandomSeed));
        var sounds = new Dictionary<(short, short), int>();
        var kinds = new Dictionary<string, int>();
        int passes = 0;
        long frame = 0;
        var log = new List<string>();
        void Count(string kind) => kinds[kind] = kinds.GetValueOrDefault(kind) + 1;
        var texts = new LinearTexts(m.Mem.ReadBlock(0, 0, 0x100000));
        var output = SetUpOutput(o, core);
        int lists = 0, nonEmpty = 0;
        string where = "entry";
        void List()
        {
            output.Scene.Finish();
            var list = ListBytes(output.Scene.Layers.Ds);
            run.CompareList(list, lists, where);
            lists++;
            if (list.Length > 2) nonEmpty++;
        }
        var inner = new PurchaseHost(e =>
        {
            Count(e.Kind.ToString());
            int n = kinds[e.Kind.ToString()];
            if (e.Kind == PurchaseEventKind.RunList) List();
            if (mutation == Mutation.SkipClock && e.Kind == PurchaseEventKind.DrawClock && n == 3) return;
            if (mutation == Mutation.SkipCash && e.Kind == PurchaseEventKind.DrawCash && n == 1) return;
            if (mutation == Mutation.SkipTicket && e.Kind == PurchaseEventKind.DrawTicket && n == 1) return;
            output.Scene.Apply(e);
            if (e.Kind != PurchaseEventKind.Sound) return;
            sounds[(e.Args[0], e.Args[1])] = sounds.GetValueOrDefault((e.Args[0], e.Args[1])) + 1;
            if (e.Args[0] == 0) log.Add($"{frame}/{passes}:p{core.S8(DgroupLayout.ActivePlayer)}:{e.Args[1]:X2}");
        }, e =>
        {
            Count(e.Kind.ToString());
            if (e.Kind == FieldEventKind.RunList) List();
            output.Scene.Apply(e);
            if (e.Kind == FieldEventKind.FlipPages) passes++;
            if (e.Kind == FieldEventKind.Sound) sounds[(e.Args[0], e.Args[1])] = sounds.GetValueOrDefault((e.Args[0], e.Args[1])) + 1;
        }, texts);
        IPurchaseHost host = mutation == Mutation.CountdownHeld ? new HeldCountdown(inner, () => passes, 200) : inner;
        var step = new PurchaseStep(host, ProgramStartDifferentialTests.Files);
        IWeekStepHandler handler = mutation == Mutation.DoubleFrame ? new DoubleTick(step, 300) : step;
        var machine = new PhaseMachine(core, random, new Dictionary<int, IWeekStepHandler> { [29] = handler }, nextStep: 29);

        var held = run.Held();
        output.Scene.NextFrame(held);
        machine.Advance(new PhaseTick(frame, held));
        int retraces = 0, pictures = 0;
        var distinct = new HashSet<ulong>();
        while (machine.Status == PhaseStatus.Waiting)
        {
            Assert.True(frame < MaxFrames, $"purchase did not end within {MaxFrames} frames (countdown {core.S16(Countdown.Remaining)})");
            Assert.True(run.RunToRetraceOrReturn(), $"port waits frame {frame}, the original returned (retrace {retraces})");
            where = $"retrace {retraces} (port frame {frame}, pass {passes})";
            run.Compare(core, random.Calls, where);
            Assert.True(output.Retraces.Frames.Count == retraces + 1 && output.Retraces.Frames[^1] == frame,
                $"{where}: the core waits its retrace {retraces}, the scene made {output.Retraces.Frames.Count} waits " +
                $"(last in frame {(output.Retraces.Frames.Count > 0 ? output.Retraces.Frames[^1] : -1)})");
            ComparePicture(o, output, where);
            pictures++;
            distinct.Add(PictureHash(o));
            retraces++;
            run.RunToRetraceEnd();
            run.CheckControls();
            frame++;
            output.Retraces.CurrentFrame = frame;
            held = run.Held();
            output.Scene.NextFrame(held);
            where = $"after retrace {retraces - 1} (port frame {frame}, pass {passes})";
            machine.Advance(new PhaseTick(frame, held));
        }
        Assert.Equal(0, step.Polls);
        Assert.False(run.RunToRetraceOrReturn(), $"the port returned after {retraces} retraces, the original waits another");
        run.CheckControls();
        Assert.True(machine.Status == PhaseStatus.Blocked && machine.NextStep == 30, $"phase machine {machine.Status} at {machine.NextStep}");
        run.Compare(core, random.Calls, $"return of 08CF:0004 after {retraces} retraces");
        Assert.Equal(retraces, step.Retraces);
        Assert.False(output.Scene.Presenting, "the scene still presents after the return");
        Assert.Equal(retraces, output.Retraces.Frames.Count);
        ComparePicture(o, output, $"return of 08CF:0004 after {retraces} retraces");
        pictures++;
        Assert.True(run.FlowLists == lists, $"the original executed {run.FlowLists} lists in the flow, the port {lists}");
        return new Result(retraces, passes, random.Calls, entry, core.Bytes, sounds, kinds, log, pictures, distinct.Count, lists,
            nonEmpty);
    }

    /// <summary>Counter-test: the step processes frame <paramref name="at"/> twice (two of its frames in one tick).</summary>
    private sealed class DoubleTick(IWeekStepHandler inner, int at) : IWeekStepHandler
    {
        private int _ticks;

        public StepOutcome Begin(StepContext context) => inner.Begin(context);

        public StepOutcome Tick(StepContext context)
        {
            var outcome = inner.Tick(context);
            if (++_ticks == at && !outcome.Done) outcome = inner.Tick(context);
            return outcome;
        }
    }

    /// <summary>
    /// Counter-test: in pass <paramref name="at"/> the pause check (right before the countdown tick) adds one tick to the step,
    /// so that tick does not count.
    /// </summary>
    private sealed class HeldCountdown(IPurchaseHost inner, Func<int> passes, int at) : IPurchaseHost
    {
        public ITextMemory Texts => inner.Texts;
        public void Emit(PurchaseEvent e) => inner.Emit(e);
        public void Emit(FieldEvent e) => inner.Emit(e);
        public void ComputerTurn(Dgroup d, short x, short y) => inner.ComputerTurn(d, x, y);
        public void ReadControls(Dgroup d) => inner.ReadControls(d);

        public IEnumerable<FieldWait> PauseCheck(Dgroup d)
        {
            if (passes() == at) d.Set(Countdown.Ticks, Int16Math.S(d.S16(Countdown.Ticks) + 1));
            return inner.PauseCheck(d);
        }
    }

    /// <summary>The original side: run to retraces and the return, compare, controls read.</summary>
    private sealed class OriginalRun
    {
        private readonly OriginalProgram _o;
        private readonly ushort _retCs, _retIp, _retraceCs;
        private readonly List<(int Index, byte[] Keys)> _controlReads = [];
        private byte[] _keysGiven = [];
        private int _retraceIndex = -1;

        public OriginalRun(OriginalProgram o, ushort retCs, ushort retIp)
        {
            _o = o;
            _retCs = retCs;
            _retIp = retIp;
            _retraceCs = o.Segment(SegRetrace);
            o.Machine.AddCodeHook(o.Segment(0x21FD), 0x01BA, _ => RandomCalls++);
            o.Machine.AddCodeHook(o.Segment(0x0E9E), 0x037A, _ => _controlReads.Add((_retraceIndex, Keys())));
            // 0A96:06A8 called from the purchase's flow (segment 08CF): its list
            ushort flow = o.Segment(0x08CF);
            o.Machine.AddCodeHook(o.Segment(0x0A96), 0x06A8, m =>
            {
                ushort cs = m.Mem.Read16(m.Cpu.Ss, (ushort)(m.Cpu.Sp + 2));
                if (cs == flow) _lists.Add(ListBytes(_o.DataSegment()));
            });
        }

        private readonly List<byte[]> _lists = [];

        /// <summary>Lists the original executed from the purchase's flow so far.</summary>
        public int FlowLists => _lists.Count;

        /// <summary>
        /// The port's <paramref name="index"/>-th list of the flow against the original's. The port runs its frame before the
        /// original does, so the original is advanced to that list first only if it has not got there (it then stands inside
        /// the frame; the next retrace stop is unaffected).
        /// </summary>
        public void CompareList(byte[] list, int index, string where)
        {
            if (_lists.Count <= index)
                _o.Machine.Run(4_000_000_000, mm => _lists.Count > index || (mm.Cpu.Cs == _retCs && mm.Cpu.IP == _retIp)
                    || (mm.Cpu.Cs == _retraceCs && mm.Cpu.IP == 0x00D3));
            Assert.True(_lists.Count > index, $"{where}: the port executes list {index} of the flow, the original has none there");
            var expected = _lists[index];
            int k = expected.AsSpan().CommonPrefixLength(list);
            if (k == expected.Length && k == list.Length) return;
            int entries = (expected.Length - 2) / GraphicsLayers.EntrySize, portEntries = (list.Length - 2) / GraphicsLayers.EntrySize;
            Assert.Fail($"{where}: list {index} of the flow: original {entries} entries, port {portEntries}; first difference in entry " +
                        $"{Math.Max(0, k - 2) / GraphicsLayers.EntrySize} byte {Math.Max(0, k - 2) % GraphicsLayers.EntrySize:X2}" +
                        (k < expected.Length && k < list.Length ? $": original {expected[k]:X2}, port {list[k]:X2}" : ""));
        }

        public long RandomCalls { get; private set; }

        private byte[] Keys() => _o.Machine.Mem.ReadBlock(_o.Dgroup, DgroupLayout.KeyState, KeyTable.Length);

        private bool At(ushort cs, ushort ip) => _o.Machine.Cpu.Cs == cs && _o.Machine.Cpu.IP == ip;

        /// <summary>Held keys for the next port frame: the original's key table now.</summary>
        public IReadOnlyCollection<byte> Held()
        {
            _keysGiven = Keys();
            return [.. Enumerable.Range(0, KeyTable.Length).Where(sc => _keysGiven[sc] != 0).Select(sc => (byte)sc)];
        }

        /// <summary>Runs the original to the next retrace entry (true) or the return of 08CF:0004 (false).</summary>
        public bool RunToRetraceOrReturn()
        {
            var m = _o.Machine;
            m.Run(1);
            m.Run(4_000_000_000, mm => (mm.Cpu.Cs == _retCs && mm.Cpu.IP == _retIp) || (mm.Cpu.Cs == _retraceCs && mm.Cpu.IP == 0x00D3));
            Assert.False(m.Exited, "original exited");
            if (At(_retraceCs, 0x00D3))
            {
                _retraceIndex++;
                return true;
            }
            Assert.True(At(_retCs, _retIp), $"original stopped at {m.Cpu.Cs - _o.Segment(0):X4}:{m.Cpu.IP:X4}");
            return false;
        }

        /// <summary>Runs the original through the retrace wait it stands at (to 0D73:00F7).</summary>
        public void RunToRetraceEnd()
        {
            var m = _o.Machine;
            m.Run(4_000_000, mm => mm.Cpu.Cs == _retraceCs && mm.Cpu.IP == 0x00F7);
            Assert.True(At(_retraceCs, 0x00F7), "retrace wait did not end");
        }

        /// <summary>Every control read of the original since the last check saw the keys the port got for that frame.</summary>
        public void CheckControls()
        {
            foreach (var (index, keys) in _controlReads)
                Assert.True(keys.AsSpan().SequenceEqual(_keysGiven),
                    $"keys changed between the end of retrace {index} and the control read of the original (script ambiguous)");
            _controlReads.Clear();
        }

        public void Compare(Dgroup core, long portRandomCalls, string where)
        {
            var expected = _o.DataSegment()[..DgroupLayout.Size];
            foreach (var (offset, length, _) in Owned) Array.Copy(core.Bytes, offset, expected, offset, length);
            if (!expected.AsSpan().SequenceEqual(core.Bytes))
            {
                var diffs = Enumerable.Range(0, expected.Length).Where(i => expected[i] != core.Bytes[i]).ToList();
                int first = diffs[0];
                Assert.Fail($"{where}: DS:{first:X4} original {expected[first]:X2}, port {core.Bytes[first]:X2}; " +
                            $"differing {string.Join(" ", diffs.Take(40).Select(i => i.ToString("X4")))}");
            }
            Assert.True(RandomCalls == portRandomCalls, $"{where}: original drew {RandomCalls} times, port {portRandomCalls}");
        }
    }
}
