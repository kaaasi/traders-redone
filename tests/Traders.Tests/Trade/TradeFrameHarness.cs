using Traders.Assets;
using Traders.Core;
using Traders.Core.Field;
using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.State;
using Traders.Core.Trade;
using Traders.Presentation.Menus;
using Traders.Presentation.Trade;
using Traders.RefEmu;
using Traders.RefEmu.Machine;
using Traders.Tests.Field;
using Traders.Tests.Harvest;
using Traders.Tests.Startup;
using Traders.Vga;
using Traders.Vga.Layers;

namespace Traders.Tests.Trade;

/// <summary>
/// The trade 0B9F:0002 frame by frame in a running game against the original. The original runs from a snapshot at the entry
/// of 0B9F:0002 (call 23 of the week loop, reached by the game itself with four computers) with live graphics and sound and
/// the trading computer 141D as it is; the port runs <see cref="TradeComputer"/> (with <see cref="Script.Ai"/> false 141D is
/// a far return on both sides and the computers keep the actions they last had); keys reach the original as real keyboard
/// interrupts at emulated times. A frame of the original is
/// one retrace wait (entry into 0D73:00D3). The port runs <see cref="TradeStep"/> in a <see cref="PhaseMachine"/> from call 23,
/// one <see cref="PhaseTick"/> per frame, with the key table the original held at the end of the retrace before. At the k-th
/// retrace of the original the port must wait its k-th frame; there the data segment (outside <see cref="Owned"/>), DS:10D2
/// the number of random() calls and the picture of the port's <see cref="TradeScene"/> (VRAM planes, DAC, CRTC start) are
/// compared; at the return once more. The command lists the trade's flow executes (0A96:06A8 called from 0C29:0046…01B7:
/// count DS:15BE and the entries from DS:15C0) are compared with the lists of the port's RunList requests, in order.
/// <para>
/// The stack word under the uninitialised local of 141D:0397 (<see cref="TradeRules.ChooseRoles"/>) is followed in the
/// original: which routine wrote it last (0E9E:037A, 141D:0397, 0C29:0F68) and whether 0397 stored it as the action
/// (<see cref="Result.StackWordUses"/>). A word written by an interrupt in between is timing, which the port does not
/// reproduce; if 0397 uses such a word the run is not comparable and fails with that message.
/// </para>
/// </summary>
internal static class TradeFrameHarness
{
    private const ushort SegRetrace = 0x0D73;

    private const double KeyPhase = 0.4;

    /// <summary>A key held from frame <c>From</c> for <c>Frames</c> frames (70 Hz, counted from the trade's start).</summary>
    public sealed record Press(long From, long Frames, byte Key);

    /// <summary>
    /// A run: the snapshot (week), players made human (control 4: arrows, Ins), key presses; <c>Ai</c> false replaces the
    /// trading computer 141D by a far return on both sides.
    /// </summary>
    public sealed record Script(int Week, int[] Humans, Press[] Presses, bool Ai = true);

    /// <summary>
    /// Data segment bytes the original writes in the trade that the core leaves to the output layers: those of the planting
    /// and harvest phases' outputs, and the number buffer of 0C29:0E01 (2295:0001 into the far pointer DS:02A6).
    /// </summary>
    public static readonly (int Offset, int Length, string Owner)[] Owned =
    [
        .. HarvestFrameHarness.Owned,
        (0x0644, 0x10, "0C29:0E01 number buffer of 2295:0001 (scene)"),
        (0x023A, 2, "0B67:01D7/0209 page flip state (Vga pages)"),
    ];

    private const long MaxFrames = 60_000;

    private static readonly Dictionary<int, Lazy<MachineSnapshot>> AtTrade = new()
    {
        [0] = new(() => Snapshot(0)),
        [1] = new(() => Snapshot(1)),
        [2] = new(() => Snapshot(2)),
        [3] = new(() => Snapshot(3)),
    };

    private static MachineSnapshot Snapshot(int week)
    {
        var p = OriginalProgram.StartGame(TestGameData.DosDir);
        while (true)
        {
            Assert.True(p.RunToRoutine(0x0B9F, 0x0002, 1800), $"0B9F:0002 not reached (week {p.Week})");
            if (p.Week == week) return p.Snapshot();
            Assert.True(p.Week < week, $"week {week} skipped");
        }
    }

    private static readonly ushort[] AiEntries = [0x0008, 0x021A, 0x0397, 0x0464, 0x09F3, 0x0E67, 0x0CCA];

    /// <param name="PicturesCompared">Frames whose picture was compared.</param>
    /// <param name="DistinctPictures">Different pictures of the original among them.</param>
    /// <param name="ListsCompared">Command lists of the flow compared (non-empty ones in brackets in the name).</param>
    /// <param name="Kinds">Requests of the port by kind.</param>
    /// <param name="StackWordUses">
    /// Calls of 141D:0397 that stored the stack word they found as the action, by the routine that had written it ("037A",
    /// "0397", "0F68") and the word.
    /// </param>
    /// <param name="StackWordNoise">Calls of 141D:0397 that found a word written by an interrupt (not used).</param>
    public sealed record Result(int Retraces, long RandomCalls, byte[] DataSegment, int Deals, int Biddings,
        IReadOnlyDictionary<(string Source, short Word), int> StackWordUses, int StackWordNoise,
        int PicturesCompared, int DistinctPictures, int ListsCompared, int NonEmptyLists,
        IReadOnlyDictionary<TradeEventKind, int> Kinds);

    /// <summary>
    /// Counter-tests: a mutation of the port. <c>DoubleFrame</c>: one frame processed twice; <c>StackWordLost</c>: 141D:0397
    /// always finds −1 under its local; <c>SiZero</c>: 0C29:0F68 leaves 0 instead of main's SI. Scene: <c>SkipMarkers</c>:
    /// the markers are not drawn from their 40th request on (a single one is redrawn unchanged); <c>SkipCountdownStep</c>: the third countdown step is not drawn;
    /// <c>SkipNumbers</c>: the tenth request for a player's numbers is not drawn.
    /// </summary>
    public enum Mutation { None, DoubleFrame, StackWordLost, SiZero, SkipMarkers, SkipCountdownStep, SkipNumbers }

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

    /// <summary>Real-mode memory as a flat copy (texts outside DS).</summary>
    private sealed class LinearMemory(byte[] memory) : IFarMemory
    {
        public byte Read(ushort segment, ushort offset) => memory[((segment << 4) + offset) & 0xFFFFF];

        public void Write(ushort segment, ushort offset, byte value) => memory[((segment << 4) + offset) & 0xFFFFF] = value;
    }

    private sealed record PortOutput(TradeScene Scene, ModeXDriver Driver, VgaDac Dac, FrameRetraces Retraces);

    private static PortOutput SetUpOutput(OriginalProgram o, Dgroup core)
    {
        var retraces = new FrameRetraces();
        var driver = FieldSceneHarness.DriverFrom(o);
        var dac = new VgaDac();
        o.Machine.Vga.Dac.CopyTo(dac.Registers, 0);
        var layers = new GraphicsLayers(driver, o.DataSegment(), dac, retraces)
            { DataSegment = o.Dgroup, OtherMemory = new LinearMemory(o.Machine.Mem.ReadBlock(0, 0, 0x100000)) };
        return new PortOutput(new TradeScene(layers, core, new Pictures()), driver, dac, retraces);
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

    /// <summary>The trading computer replaced: no call does anything (as the retf in the original).</summary>
    private sealed class NoAi : ITradeAi
    {
        public void Prepare(Dgroup d, OriginalRandom random) { }
        public void EnterTrade(Dgroup d, OriginalRandom random) { }
        public short ChooseRole(Dgroup d, OriginalRandom random, short stackWord) => stackWord;
        public void StartBidding(Dgroup d, OriginalRandom random) { }
        public void Bid(Dgroup d, OriginalRandom random) { }
        public void AfterSale(Dgroup d, OriginalRandom random) { }
        public void AfterPurchase(Dgroup d, OriginalRandom random, short price) { }
    }

    /// <summary>Counter-test: the trading computer with a wrong stack word for 141D:0397.</summary>
    private sealed class MutatedAi(Mutation mutation) : ITradeAi
    {
        private static readonly TradeComputer Ai = TradeComputer.Instance;

        public void Prepare(Dgroup d, OriginalRandom random) => Ai.Prepare(d, random);
        public void EnterTrade(Dgroup d, OriginalRandom random) => Ai.EnterTrade(d, random);

        public short ChooseRole(Dgroup d, OriginalRandom random, short stackWord) => mutation switch
        {
            Mutation.StackWordLost => Ai.ChooseRole(d, random, -1),
            // main's SI (1) can only come from 0C29:0F68: the controls leave −1, 0, 1 (left), 2, 3 or Ah, 0397 −1, 2 or 3 –
            // with four computers (week 0) a 1 is 0F68's.
            Mutation.SiZero => Ai.ChooseRole(d, random, stackWord == TradeRules.MainArgumentCount ? (short)0 : stackWord),
            _ => Ai.ChooseRole(d, random, stackWord),
        };

        public void StartBidding(Dgroup d, OriginalRandom random) => Ai.StartBidding(d, random);
        public void Bid(Dgroup d, OriginalRandom random) => Ai.Bid(d, random);
        public void AfterSale(Dgroup d, OriginalRandom random) => Ai.AfterSale(d, random);
        public void AfterPurchase(Dgroup d, OriginalRandom random, short price) => Ai.AfterPurchase(d, random, price);
    }

    public static Result Run(Script script, Mutation mutation = Mutation.None)
    {
        var o = OriginalProgram.Resume(TestGameData.DosDir, AtTrade[script.Week].Value);
        var m = o.Machine;
        Assert.True(o.IsAt(0x0B9F, 0x0002), "snapshot not at 0B9F:0002");
        if (!script.Ai)
            foreach (ushort off in AiEntries) m.Mem.Write8(o.Segment(0x141D), off, 0xCB);
        ushort retIp = m.Mem.Read16(m.Cpu.Ss, m.Cpu.Sp), retCs = m.Mem.Read16(m.Cpu.Ss, (ushort)(m.Cpu.Sp + 2));
        foreach (int h in script.Humans)
        {
            o.WriteByte((ushort)(ControlReader.IsComputer + h), 0);
            o.WriteByte((ushort)(ControlReader.ControlType + h), 4);
        }
        long period = m.Vga.FramePeriod;
        long frame0 = m.FrameNumber + 1;
        double Time(long frame) => (double)((frame0 + frame) * period + (long)(KeyPhase * period)) / Traders.RefEmu.Hardware.Pit.ClockHz;
        foreach (var press in script.Presses)
        {
            m.ScheduleScancode(Time(press.From), press.Key);
            m.ScheduleScancode(Time(press.From + press.Frames), (byte)(press.Key | 0x80));
        }

        OriginalRun run = new(o, retCs, retIp);
        var core = new Dgroup(o.DataSegment()[..DgroupLayout.Size]);
        var random = new OriginalRandom(new RecordedClock([]), core.U32(DgroupLayout.RandomSeed));
        int deals = 0, biddings = 0, lists = 0, nonEmpty = 0;
        var output = SetUpOutput(o, core);
        var kinds = new Dictionary<TradeEventKind, int>();
        string where = "entry";
        var host = new TradeHost(e =>
        {
            if (e.Kind == TradeEventKind.DrawStockChange && e.Args[0] == -1) deals++;
            if (e.Kind == TradeEventKind.FadeInFast) biddings++;
            int n = kinds[e.Kind] = kinds.GetValueOrDefault(e.Kind) + 1;
            if (e.Kind == TradeEventKind.RunList)
            {
                output.Scene.Finish();
                var list = ListBytes(output.Scene.Layers.Ds);
                run.CompareList(list, lists, where);
                lists++;
                if (list.Length > 2) nonEmpty++;
            }
            if (mutation == Mutation.SkipMarkers && e.Kind == TradeEventKind.DrawMarkers && n >= 40) return;
            if (mutation == Mutation.SkipCountdownStep && e.Kind == TradeEventKind.CountdownStep && n == 3) return;
            if (mutation == Mutation.SkipNumbers && e.Kind == TradeEventKind.DrawPlayerNumbers && n == 10) return;
            output.Scene.Apply(e);
        });
        ITradeAi ai = !script.Ai ? new NoAi()
            : mutation is Mutation.StackWordLost or Mutation.SiZero ? new MutatedAi(mutation) : TradeComputer.Instance;
        var step = new TradeStep(host, ai, ProgramStartDifferentialTests.Files);
        IWeekStepHandler handler = mutation == Mutation.DoubleFrame ? new DoubleTick(step, 500) : step;
        var machine = new PhaseMachine(core, random, new Dictionary<int, IWeekStepHandler> { [23] = handler }, nextStep: 23);

        long frame = 0;
        var held = run.Held();
        output.Scene.NextFrame(held);
        machine.Advance(new PhaseTick(frame, held));
        int retraces = 0, pictures = 0;
        var distinct = new HashSet<ulong>();
        while (machine.Status == PhaseStatus.Waiting)
        {
            Assert.True(frame < MaxFrames, $"trade did not end within {MaxFrames} frames");
            Assert.True(run.RunToRetraceOrReturn(), $"port waits frame {frame}, the original returned (retrace {retraces})");
            where = $"retrace {retraces} (port frame {frame})";
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
            where = $"after retrace {retraces - 1} (port frame {frame})";
            machine.Advance(new PhaseTick(frame, held));
        }
        Assert.Equal(0, step.Polls);
        Assert.False(run.RunToRetraceOrReturn(), $"the port returned after {retraces} retraces, the original waits another");
        run.CheckControls();
        Assert.True(machine.Status == PhaseStatus.Blocked && machine.NextStep == 24,
            $"phase machine {machine.Status} at {machine.NextStep}");
        run.Compare(core, random.Calls, $"return of 0B9F:0002 after {retraces} retraces");
        Assert.Equal(retraces, step.Retraces);
        Assert.False(output.Scene.Presenting, "the scene still presents after the return");
        Assert.Equal(retraces, output.Retraces.Frames.Count);
        ComparePicture(o, output, $"return of 0B9F:0002 after {retraces} retraces");
        pictures++;
        Assert.True(run.FlowLists == lists, $"the original executed {run.FlowLists} lists in the flow, the port {lists}");
        Assert.True(run.StackWordNoiseUsed is null, $"not comparable: {run.StackWordNoiseUsed}");
        return new Result(retraces, random.Calls, core.Bytes, deals, biddings, run.StackWordUses, run.StackWordNoise,
            pictures, distinct.Count, lists, nonEmpty, kinds);
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
            FollowStackWord();
            // 0A96:06A8 called from the trade's flow 0C29:0046 … 01B7 (return offsets below 0C29:0323): its list
            ushort flow = o.Segment(0x0C29);
            o.Machine.AddCodeHook(o.Segment(0x0A96), 0x06A8, m =>
            {
                ushort ip = m.Mem.Read16(m.Cpu.Ss, m.Cpu.Sp), cs = m.Mem.Read16(m.Cpu.Ss, (ushort)(m.Cpu.Sp + 2));
                if (cs == flow && ip < 0x0323) _lists.Add(ListBytes(_o.DataSegment()));
            });
        }

        private readonly List<byte[]> _lists = [];

        /// <summary>Lists the original executed from the trade's flow so far.</summary>
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

        /// <summary>Uses of the stack word by 141D:0397, by the routine that wrote it last and the word.</summary>
        public Dictionary<(string Source, short Word), int> StackWordUses { get; } = [];

        /// <summary>Calls of 141D:0397 that found a word none of the three routines had left (an interrupt's).</summary>
        public int StackWordNoise { get; private set; }

        /// <summary>Set if 141D:0397 stored an interrupt's word as the action.</summary>
        public string? StackWordNoiseUsed { get; private set; }

        /// <summary>
        /// The word at SP−8 of 0B9F:00FE: read back after each routine that leaves it there (return of 0E9E:037A at
        /// 0B9F:0106, end of 141D:0397 at 141D:0460, return of 0C29:0F68 at 0B9F:015B/01B6) and at 141D:039C, where 0397's
        /// frame is built. 0397 uses it when it drew (141D:03C1) and its local still holds it at the end.
        /// </summary>
        private void FollowStackWord()
        {
            var m = _o.Machine;
            ushort trade = _o.Segment(0x0B9F), ai = _o.Segment(0x141D);
            if (m.Mem.Read8(ai, 0x0397) == 0xCB) return;
            ushort slot = 0, left = 0, found = 0;
            string source = "";
            bool drew = false, assigned = false, noise = false;
            ushort Slot() => m.Mem.Read16(m.Cpu.Ss, slot);
            static ushort Expect(ushort? word, string where) =>
                word ?? throw new InvalidOperationException($"the stack word was not read at {where}");
            ushort flow = _o.Segment(0x0C29), control = _o.Segment(0x0E9E);
            ushort? left037A = null, left0F68 = null;
            m.AddCodeHook(trade, 0x0101, _ => (slot, left037A, left0F68) = ((ushort)(m.Cpu.Sp - 8), null, null));
            // read while the word is still inside the callee's frame (at or above SP), so that an interrupt after the
            // return counts as noise: 0E9E:0405 (mov sp,bp; [bp-2] = SP), 0C29:0FB6 (pop si; the pushed SI = SP)
            m.AddCodeHook(control, 0x0405, _ => { if ((ushort)(m.Cpu.Bp - 2) == slot) left037A = Slot(); });
            m.AddCodeHook(flow, 0x0FB6, _ => { if (m.Cpu.Sp == slot) left0F68 = Slot(); });
            m.AddCodeHook(trade, 0x0106, _ => (left, source) = (Expect(left037A, "0E9E:0405"), "037A"));
            m.AddCodeHook(trade, 0x015B, _ => (left, source) = (Expect(left0F68, "0C29:0FB6"), "0F68"));
            m.AddCodeHook(trade, 0x01B6, _ => (left, source) = (Expect(left0F68, "0C29:0FB6"), "0F68"));
            m.AddCodeHook(ai, 0x039C, _ =>
            {
                Assert.Equal(slot, (ushort)(m.Cpu.Bp - 2));
                found = Slot();
                drew = assigned = false;
                noise = found != left;
                if (noise) StackWordNoise++;
            });
            // 141D:03C1: the branch with the draws; 141D:03CF / 03E2: its assignments after a draw of 0
            m.AddCodeHook(ai, 0x03C1, _ => drew = true);
            m.AddCodeHook(ai, 0x03CF, _ => assigned = true);
            m.AddCodeHook(ai, 0x03E2, _ => assigned = true);
            m.AddCodeHook(ai, 0x0460, _ =>
            {
                if (drew && !assigned)
                {
                    if (noise)
                        StackWordNoiseUsed ??= $"141D:0397 used the word {found:X4} an interrupt wrote (retrace {_retraceIndex})";
                    else
                        StackWordUses[(source, (short)found)] = StackWordUses.GetValueOrDefault((source, (short)found)) + 1;
                }
                (left, source) = (Slot(), "0397");
            });
        }

        public long RandomCalls { get; private set; }

        private byte[] Keys() => _o.Machine.Mem.ReadBlock(_o.Dgroup, DgroupLayout.KeyState, KeyTable.Length);

        private bool At(ushort cs, ushort ip) => _o.Machine.Cpu.Cs == cs && _o.Machine.Cpu.IP == ip;

        public IReadOnlyCollection<byte> Held()
        {
            _keysGiven = Keys();
            return [.. Enumerable.Range(0, KeyTable.Length).Where(sc => _keysGiven[sc] != 0).Select(sc => (byte)sc)];
        }

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

        public void RunToRetraceEnd()
        {
            var m = _o.Machine;
            m.Run(4_000_000, mm => mm.Cpu.Cs == _retraceCs && mm.Cpu.IP == 0x00F7);
            Assert.True(At(_retraceCs, 0x00F7), "retrace wait did not end");
        }

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
