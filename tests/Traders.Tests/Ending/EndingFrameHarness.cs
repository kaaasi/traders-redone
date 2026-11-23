using Traders.Assets;
using Traders.Core;
using Traders.Core.Ending;
using Traders.Core.Field;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.Pixels;
using Traders.Core.State;
using Traders.Presentation.Ending;
using Traders.Presentation.Menus;
using Traders.RefEmu;
using Traders.RefEmu.Hardware;
using Traders.RefEmu.Machine;
using Traders.Tests.Field;
using Traders.Tests.Harvest;
using Traders.Tests.Startup;
using Traders.Vga;
using Traders.Vga.Layers;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Tests.Ending;

/// <summary>
/// The end check 20B3:0005 where the game ends (call 19 with its rest 20B3:0053–0088, finding docs/routines/20B3_008E_ende.md)
/// frame by frame in a running game against the original. The original runs from a snapshot at the routine's entry (reached by
/// the game itself with four computers) with live graphics, its pixel reads 23CD:06DE on the real VRAM, the report 0825:000A(0)
/// and the key wait 27C6:00D2; a frame of the original is one retrace wait (entry into 0D73:00D3). The port runs
/// <see cref="EndingStep"/> in a <see cref="PhaseMachine"/> from call 19, one <see cref="PhaseTick"/> per frame with the key table
/// the original held at the end of the retrace before; its evening reads the core's rule page (<see cref="EndingPage"/>, R6P-1),
/// its report is the one of lane bilanz (<see cref="EndScreens.Report"/>). At the k-th retrace of the original the port must
/// wait its k-th frame (<see cref="EndingStep.Retraces"/>); there the data segment (outside <see cref="Owned"/>), DS:10D2 and
/// the number of random() calls are compared. When the original enters the key wait, the port must poll; after
/// <see cref="Script.Polls"/> port frames a key is pressed in the original and the port's keyboard handler counts the make
/// (DS:115D + 1, as the host's <c>KeyStateTable</c> does); both must return in that frame with AL = 1, compared once more.
/// The port's <see cref="EndingScene"/> plays every event (and the report's) through the graphics layers frame by frame: at
/// each retrace the scene must make its k-th retrace wait in that frame, and the picture (four VRAM planes, DAC, CRTC start) and
/// the command list 0A96 (count and used entries) are compared as well; at the key wait and the return once more (part 3).
/// </summary>
internal static class EndingFrameHarness
{
    private const ushort Seg20B3 = 0x20B3, Entry = 0x0005, Seg21FD = 0x21FD, SegRetrace = 0x0D73, Seg27C6 = 0x27C6,
        WaitKey = 0x00D2;

    /// <summary>
    /// A run: the game (delay of RETURN in the main menu, which picks the random state of the new game, probe P5), the week of
    /// its end check (snapshot at the entry), how it ends (F10 held, or the target DS:08F4 set to 0), bytes set in the data
    /// segment at the entry (both sides), port frames polled in the key wait before the key comes.
    /// </summary>
    public sealed record Script(double MenuDelay, int Week, bool F10, (int Offset, byte[] Bytes)[] Set, int Polls = 3);

    /// <summary>
    /// Data segment bytes the core leaves to others: the output layers (the planting's and the harvest's set: pages, palettes,
    /// command list, objects, typewriter, font; the interrupt handlers' bytes; the draw page switch DS:023A), the picture cache of the loader 186C:0002 with
    /// its heap pointers (host, docs/routines/186C_ressourcenlader.md) and the music control's words that 218B:0094(2, 3)
    /// writes (sound command of lanes effekte/musik, the core emits only the request).
    /// </summary>
    public static readonly (int Offset, int Length, string Owner)[] Owned =
    [
        .. HarvestFrameHarness.Owned,
        (0x023A, 2, "0B67:01D4/0206 draw on the page shown / hidden (Vga pages; the core's rule page keeps its own draw page)"),
        (0x4C00, 0x113, "186C:0002 picture cache with heap pointers (host)"),
        .. Traders.Tests.Phases.RefEmuProgramOracle.SoundOwned,
    ];

    private const long MaxFrames = 60_000;

    /// <summary>Pixel reads after which the port's host gives up (abort counter of the reveal, R9PL-6).</summary>
    public const int ReadLimit = 400_000;

    /// <summary>Result of a run (diagnostics and coverage of the scenarios).</summary>
    public sealed record Result(int Retraces, int Polls, long RandomCalls, int PixelReads, byte[] DataSegment, List<string> Events,
        int PicturesCompared, int DistinctPictures);

    /// <summary>Counter-tests: a mutation of the port's host or phase machine.</summary>
    public enum Mutation
    {
        None,

        /// <summary>The report is called with argument 1 (opening and wait loop) instead of 0.</summary>
        ReportArgument1,

        /// <summary>The evening's pixel reads answer colour 2 for 6 and 6 for 2.</summary>
        SwapPixelTargets,

        /// <summary>The step processes one frame twice.</summary>
        DoubleFrame,

        /// <summary>The week counter DS:4B9A is taken back when the step finishes (20B3:0071 left out).</summary>
        KeepWeek,

        /// <summary>Scene: the reveal's cell is drawn on the hidden page (0B67:0206 for 0B67:01D4).</summary>
        SceneRevealOnHiddenPage,

        /// <summary>Scene: the palette step of the wipe writes the DAC before its retrace (21FD:03EB without frame step).</summary>
        ScenePaletteStepAtOnce,

        /// <summary>Scene: the masked blit of the closing picture's scenery (20B3:015B) is left out.</summary>
        SceneWithoutMaskedBlit,
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

    /// <summary>Real-mode memory as a flat copy (texts outside DS).</summary>
    private sealed class LinearMemory(byte[] memory) : IFarMemory
    {
        public byte Read(ushort segment, ushort offset) => memory[((segment << 4) + offset) & 0xFFFFF];

        public void Write(ushort segment, ushort offset, byte value) => memory[((segment << 4) + offset) & 0xFFFFF] = value;
    }

    /// <summary>The port's output: the scene on layers set up from the original at the entry.</summary>
    private sealed class PortOutput
    {
        public PortOutput(OriginalProgram o, Dgroup core)
        {
            Driver = Traders.Tests.Field.FieldSceneHarness.DriverFrom(o);
            o.Machine.Vga.Dac.CopyTo(Dac.Registers, 0);
            var memory = o.Machine.Mem.ReadBlock(0, 0, 0x100000);
            var layers = new GraphicsLayers(Driver, o.DataSegment(), Dac, Retraces)
                { DataSegment = o.Dgroup, OtherMemory = new LinearMemory(memory) };
            Scene = new EndingScene(layers, core, layers.OtherMemory, new Pictures());
        }

        public EndingScene Scene { get; }
        public ModeXDriver Driver { get; }
        public VgaDac Dac { get; } = new();
        public FrameRetraces Retraces { get; } = new();
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

    /// <summary>The command list 0A96: count DS:15BE and its used entries (count · 1Ch bytes from DS:15C0), first difference named.</summary>
    private static void CompareList(OriginalProgram o, PortOutput port, string where)
    {
        var ods = o.DataSegment();
        var pds = port.Scene.Layers.Ds;
        int count = ods[GraphicsLayers.ListCount] | ods[GraphicsLayers.ListCount + 1] << 8;
        int portCount = pds[GraphicsLayers.ListCount] | pds[GraphicsLayers.ListCount + 1] << 8;
        Assert.True(count == portCount, $"{where}: command list with {count} entries in the original, {portCount} in the port");
        var a = ods.AsSpan(GraphicsLayers.ListStart, count * GraphicsLayers.EntrySize);
        var b = pds.AsSpan(GraphicsLayers.ListStart, count * GraphicsLayers.EntrySize);
        int k = a.CommonPrefixLength(b);
        if (k < a.Length)
            Assert.Fail($"{where}: command list entry {k / GraphicsLayers.EntrySize} byte {k % GraphicsLayers.EntrySize:X2} " +
                        $"original {a[k]:X2} port {b[k]:X2}");
    }

    /// <summary>
    /// Scene waits so far: one per retrace of the original, the last one in <paramref name="frame"/> (the port frame of the
    /// original's last retrace).
    /// </summary>
    private static void CompareWaits(PortOutput port, int waits, long frame, string where)
    {
        var f = port.Retraces.Frames;
        Assert.True(f.Count == waits && (waits == 0 || f[^1] == frame),
            $"{where}: the core has waited {waits} retraces, the scene made {f.Count} waits (last in frame {(f.Count > 0 ? f[^1] : -1)})");
    }

    private static void CompareOutput(OriginalProgram o, PortOutput port, string where)
    {
        ComparePicture(o, port, where);
        CompareList(o, port, where);
    }

    /// <summary>Games run on to the entries asked for; snapshots by game and week.</summary>
    private static class Snapshots
    {
        private static readonly object Gate = new();
        private static readonly Dictionary<(double, int), MachineSnapshot> Taken = [];
        private static readonly Dictionary<double, (OriginalProgram Game, int Last)> Games = [];

        public static MachineSnapshot At(double menuDelay, int week)
        {
            lock (Gate)
            {
                if (Taken.TryGetValue((menuDelay, week), out var s)) return s;
                if (!Games.TryGetValue(menuDelay, out var g))
                    g = (OriginalProgram.StartGame(TestGameData.DosDir, menuDelay: menuDelay), -1);
                var game = g.Game;
                for (int w = g.Last + 1; w <= week; w++)
                {
                    Assert.True(game.RunToWeekStart(w, 900), $"week {w} not reached");
                    Assert.True(game.RunToRoutine(Seg20B3, Entry, 900), $"end check of week {w} not reached");
                    Assert.Equal(w, game.Week);
                    Taken[(menuDelay, w)] = game.Snapshot();
                    game.Machine.RunFor(0.05);
                }
                Assert.True(Taken.ContainsKey((menuDelay, week)), $"end check of week {week} already passed");
                Games[menuDelay] = (game, Math.Max(g.Last, week));
                return Taken[(menuDelay, week)];
            }
        }
    }

    public static Result Run(Script script, Mutation mutation = Mutation.None)
    {
        var o = OriginalProgram.Resume(TestGameData.DosDir, Snapshots.At(script.MenuDelay, script.Week));
        var m = o.Machine;
        Assert.True(o.IsAt(Seg20B3, Entry), "snapshot not at 20B3:0005");
        ushort retIp = m.Mem.Read16(m.Cpu.Ss, m.Cpu.Sp), retCs = m.Mem.Read16(m.Cpu.Ss, (ushort)(m.Cpu.Sp + 2));
        if (script.F10) o.WriteByte((ushort)(L.KeyState + EndCheck.EndKey), 1);
        else o.WriteDword((ushort)L.Target, 0);
        foreach (var (at, bytes) in script.Set)
            for (int i = 0; i < bytes.Length; i++) o.WriteByte((ushort)(at + i), bytes[i]);

        var run = new OriginalRun(o, retCs, retIp);
        var core = new Dgroup(o.DataSegment()[..L.Size]);
        var random = new OriginalRandom(new RecordedClock([]), core.U32(L.RandomSeed));
        var output = new PortOutput(o, core);
        var host = new Host(mutation, output.Scene);
        var step = new EndingStep(host, ProgramStartDifferentialTests.Files, n => EndingInputs.Pictures.Value[n],
            EndingInputs.Fonts.Value);
        IWeekStepHandler handler = mutation switch
        {
            Mutation.DoubleFrame => new DoubleTick(step, 300),
            Mutation.KeepWeek => new WeekTakenBack(step),
            _ => step,
        };
        var machine = new PhaseMachine(core, random, new Dictionary<int, IWeekStepHandler> { [EndCheckStep.Call] = handler },
            nextStep: EndCheckStep.Call);

        long frame = 0;
        var held = run.Held();
        output.Scene.NextFrame(held);
        machine.Advance(new PhaseTick(frame, held));
        int retraces = 0, pictures = 0;
        var distinct = new HashSet<ulong>();
        while (true)
        {
            Assert.True(frame < MaxFrames, $"the end screens did not end within {MaxFrames} frames");
            Assert.True(machine.Status == PhaseStatus.Waiting,
                $"port finished ({machine.Status}) after {retraces} retraces, the original goes on");
            var stop = run.RunToRetraceWaitOrReturn();
            string where = $"retrace {retraces} (port frame {frame})";
            if (stop != Stop.Retrace)
            {
                Assert.True(stop == Stop.WaitKey, $"the original returned without its key wait after {retraces} retraces");
                break;
            }
            Assert.True(step.Retraces == retraces + 1 && step.Polls == 0,
                $"{where}: the original waits its retrace {retraces}, the port has waited {step.Retraces} retraces and {step.Polls} polls");
            run.Compare(core, random.Calls, where);
            CompareWaits(output, retraces + 1, frame, where);
            CompareOutput(o, output, where);
            pictures++;
            distinct.Add(Traders.Tests.Presentation.MenuSceneHarness.Hash(m.Vga.Planes, m.Vga.Dac, m.Vga.StartAddress));
            retraces++;
            run.RunToRetraceEnd();
            run.CheckControls();
            frame++;
            output.Retraces.CurrentFrame = frame;
            held = run.Held();
            output.Scene.NextFrame(held);
            machine.Advance(new PhaseTick(frame, held));
        }

        // 27C6:00D2: the original clears DS:115D and loops until a make is counted; the port polls once per frame.
        string atWait = $"key wait after {retraces} retraces (port frame {frame})";
        Assert.True(machine.Status == PhaseStatus.Waiting && step.Retraces == retraces && step.Polls == 1,
            $"{atWait}: port {machine.Status}, {step.Retraces} retraces and {step.Polls} polls");
        run.Compare(core, random.Calls, atWait);
        Assert.False(output.Scene.Presenting, $"{atWait}: the scene still presents");
        CompareWaits(output, retraces, frame - 1, atWait);
        CompareOutput(o, output, atWait);
        pictures++;
        for (int k = 1; k < script.Polls; k++)
        {
            frame++;
            output.Retraces.CurrentFrame = frame;
            held = run.Held();
            output.Scene.NextFrame(held);
            machine.Advance(new PhaseTick(frame, held));
            Assert.True(machine.Status == PhaseStatus.Waiting, $"{atWait}: the port ended its wait without a key");
        }
        Assert.Equal(script.Polls, step.Polls);
        o.Press(Scancodes.Enter, delay: 0.02, hold: 0.05);
        Assert.True(run.RunToReturn(), $"{atWait}: the original did not return after the key");
        run.CheckControls();
        core.Set(EndScreens.MakeCount, unchecked((byte)(core.U8(EndScreens.MakeCount) + 1))); // 27C6:0043, the make
        frame++;
        output.Retraces.CurrentFrame = frame;
        held = run.Held();
        output.Scene.NextFrame(held);
        machine.Advance(new PhaseTick(frame, held));

        string end = $"return of 20B3:0005 after {retraces} retraces";
        Assert.True(machine.Status == PhaseStatus.Finished && machine.Result == WeekLoop.ResultGameOver,
            $"{end}: phase machine {machine.Status} at {machine.NextStep}");
        Assert.Equal(1, m.Cpu.Ax & 0xFF);
        run.Compare(core, random.Calls, end);
        Assert.Equal(o.ReadByte(EndScreens.MakeCount), core.U8(EndScreens.MakeCount));
        Assert.Equal(retraces, step.Retraces);
        Assert.Equal(run.PixelReads, host.PixelReads);
        Assert.False(output.Scene.Presenting, $"{end}: the scene still presents");
        Assert.Equal(retraces, output.Retraces.Frames.Count);
        CompareOutput(o, output, end);
        pictures++;
        return new Result(retraces, step.Polls, random.Calls, host.PixelReads, core.Bytes, host.Events, pictures, distinct.Count);
    }

    /// <summary>
    /// The port's host for the test: records the events and plays them on the scene, reads the rule page (with an abort
    /// counter), runs the report of lane bilanz with argument 0 and plays its events after each of its frames.
    /// </summary>
    private sealed class Host(Mutation mutation, EndingScene scene) : IEndingHost
    {
        public List<string> Events { get; } = [];
        public int PixelReads { get; private set; }

        public void ComputerTurn(Dgroup d, short x, short y) =>
            throw new InvalidOperationException("The end screens have no computer turn.");

        public void Emit(FieldEvent e)
        {
            if (e.Kind is FieldEventKind.LoadImage or FieldEventKind.Sound) Events.Add(e.ToString());
            scene.Apply(e);
        }

        public void Emit(EndingEvent e)
        {
            if (e.Kind is EndingEventKind.Music or EndingEventKind.WaitKey) Events.Add(e.ToString());
            switch (mutation)
            {
                case Mutation.SceneRevealOnHiddenPage when e.Kind == EndingEventKind.DrawShownPage:
                    scene.Apply(e with { Kind = EndingEventKind.DrawHiddenPage });
                    return;
                case Mutation.ScenePaletteStepAtOnce when e.Kind == EndingEventKind.PaletteStep:
                    scene.Finish();
                    scene.Layers.Blend((ushort)e.Args[0], (ushort)e.Args[1]);
                    return;
                case Mutation.SceneWithoutMaskedBlit when e.Kind == EndingEventKind.MaskedBlit:
                    return;
            }
            scene.Apply(e);
        }

        public byte ReadPixel(RulePage page, short x, short y)
        {
            if (++PixelReads > ReadLimit) throw new InvalidOperationException($"the reveal read more than {ReadLimit} pixels");
            byte v = page.Get(x, y);
            return mutation == Mutation.SwapPixelTargets && v is 2 or 6 ? (byte)(8 - v) : v;
        }

        public IEnumerable<FieldWait> Report(Dgroup d, OriginalRandom random)
        {
            Events.Add("Report");
            if (mutation != Mutation.ReportArgument1)
                return EndScreens.Report(d, random, events =>
                {
                    foreach (var e in events) scene.Apply(e);
                });
            return ReportWith1(d, random);
        }

        private static IEnumerable<FieldWait> ReportWith1(Dgroup d, OriginalRandom random)
        {
            var report = new Traders.Core.Week.Report(d, random, Traders.Core.Input.KeyboardOnlyDevices.Instance, 1);
            foreach (var w in report.Run())
            {
                report.DrainEvents();
                yield return w == Traders.Core.Menus.MenuWait.Poll ? FieldWait.Poll : FieldWait.Retrace;
            }
        }
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

    /// <summary>Counter-test: the week counter is decremented again when the step finishes.</summary>
    private sealed class WeekTakenBack(IWeekStepHandler inner) : IWeekStepHandler
    {
        public StepOutcome Begin(StepContext context) => Done(context, inner.Begin(context));

        public StepOutcome Tick(StepContext context) => Done(context, inner.Tick(context));

        private static StepOutcome Done(StepContext context, StepOutcome outcome)
        {
            if (outcome.Done && outcome.Result == 1)
                context.State.Set(L.Week, Int16Math.S(context.State.S16(L.Week) - 1));
            return outcome;
        }
    }

    private enum Stop { Retrace, WaitKey, Return }

    /// <summary>The original side: run to retraces, the key wait and the return, compare, controls read.</summary>
    private sealed class OriginalRun
    {
        private readonly OriginalProgram _o;
        private readonly ushort _retCs, _retIp, _retraceCs, _waitCs;
        private readonly List<(int Index, byte[] Keys)> _controlReads = [];
        private byte[] _keysGiven = [];
        private int _retraceIndex = -1;

        public OriginalRun(OriginalProgram o, ushort retCs, ushort retIp)
        {
            _o = o;
            _retCs = retCs;
            _retIp = retIp;
            _retraceCs = o.Segment(SegRetrace);
            _waitCs = o.Segment(Seg27C6);
            o.Machine.AddCodeHook(o.Segment(Seg21FD), 0x01BA, _ => RandomCalls++);
            o.Machine.AddCodeHook(o.Segment(0x23CD), 0x06DE, _ => PixelReads++);
            o.Machine.AddCodeHook(o.Segment(0x0E9E), 0x037A, _ => _controlReads.Add((_retraceIndex, Keys())));
        }

        public long RandomCalls { get; private set; }

        public int PixelReads { get; private set; }

        private byte[] Keys() => _o.Machine.Mem.ReadBlock(_o.Dgroup, L.KeyState, KeyTable.Length);

        private bool At(ushort cs, ushort ip) => _o.Machine.Cpu.Cs == cs && _o.Machine.Cpu.IP == ip;

        /// <summary>Held keys for the next port frame: the original's key table now.</summary>
        public IReadOnlyCollection<byte> Held()
        {
            _keysGiven = Keys();
            return [.. Enumerable.Range(0, KeyTable.Length).Where(sc => _keysGiven[sc] != 0).Select(sc => (byte)sc)];
        }

        /// <summary>Runs the original to the next retrace entry, the entry of the key wait or the routine's return.</summary>
        public Stop RunToRetraceWaitOrReturn()
        {
            var m = _o.Machine;
            m.Run(1);
            m.Run(4_000_000_000, mm => (mm.Cpu.Cs == _retCs && mm.Cpu.IP == _retIp)
                                       || (mm.Cpu.Cs == _retraceCs && mm.Cpu.IP == 0x00D3)
                                       || (mm.Cpu.Cs == _waitCs && mm.Cpu.IP == WaitKey));
            Assert.False(m.Exited, "original exited");
            if (At(_retraceCs, 0x00D3))
            {
                _retraceIndex++;
                return Stop.Retrace;
            }
            if (At(_waitCs, WaitKey)) return Stop.WaitKey;
            Assert.True(At(_retCs, _retIp), $"original stopped at {m.Cpu.Cs - _o.Segment(0):X4}:{m.Cpu.IP:X4}");
            return Stop.Return;
        }

        /// <summary>Runs the original from the key wait to the routine's return (false: a retrace came first).</summary>
        public bool RunToReturn()
        {
            var m = _o.Machine;
            m.Run(1);
            m.Run(4_000_000_000, mm => (mm.Cpu.Cs == _retCs && mm.Cpu.IP == _retIp) || (mm.Cpu.Cs == _retraceCs && mm.Cpu.IP == 0x00D3));
            Assert.False(m.Exited, "original exited");
            return At(_retCs, _retIp);
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
            var expected = _o.DataSegment()[..L.Size];
            foreach (var (offset, length, _) in Owned) Array.Copy(core.Bytes, offset, expected, offset, length);
            if (!expected.AsSpan().SequenceEqual(core.Bytes))
            {
                var diffs = Enumerable.Range(0, expected.Length).Where(i => expected[i] != core.Bytes[i]).ToList();
                int first = diffs[0];
                Assert.Fail($"{where}: DS:{first:X4} original {expected[first]:X2}, port {core.Bytes[first]:X2}; " +
                            $"differing {string.Join(" ", diffs.Take(40).Select(i => $"{i:X4}:{expected[i]:X2}/{core.Bytes[i]:X2}"))}");
            }
            Assert.True(RandomCalls == portRandomCalls, $"{where}: original drew {RandomCalls} times, port {portRandomCalls}");
        }
    }
}
