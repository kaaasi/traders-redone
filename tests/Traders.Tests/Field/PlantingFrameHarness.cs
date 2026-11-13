using Traders.Assets;
using Traders.Core;
using Traders.Core.Field;
using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.State;
using Traders.Presentation.Field;
using Traders.Presentation.Menus;
using Traders.RefEmu;
using Traders.RefEmu.Machine;
using Traders.Tests.Input;
using Traders.Tests.Phases;
using Traders.Tests.Startup;
using Traders.Vga;
using Traders.Vga.Layers;
using P = Traders.Core.Field.Planting;

namespace Traders.Tests.Field;

/// <summary>
/// The planting phase 0609:0004 frame by frame in a running game against the original. The original runs from a snapshot at
/// the entry of 0609:0004 (call 8 of the week loop, reached by the game itself) with live graphics and sound; keys reach it as
/// real keyboard interrupts at emulated times. A frame of the original is one retrace wait (entry into 0D73:00D3). The port
/// runs <see cref="PlantingStep"/> in a <see cref="PhaseMachine"/> from call 8, one <see cref="PhaseTick"/> per frame, with
/// the key table the original held at the end of the retrace before. At the k-th retrace of the original the port must wait
/// its k-th frame; there the data segment (outside <see cref="Owned"/>), DS:10D2 and the number of random() calls are compared.
/// At its return the whole data segment is compared again.
///
/// The picture is compared too (Teil 2, #103 P3-3): the port's <see cref="FieldScene"/> plays every event of the core through
/// the graphics layers frame by frame (<see cref="FieldScene.NextFrame"/> before each frame); it must make its k-th retrace
/// wait in the port frame of the k-th retrace, and there the four VRAM planes, the DAC and the CRTC start equal the
/// original's at the entry of its retrace wait; at the return once more.
///
/// Not yet ported and answered from the RefEmu (gleichschritt oracle, docs/arbeitspakete.md §0): the helpers and computer hooks
/// of lane anbau-ki (0609:074D, 0609:100F, 135D:000E, 135D:0AB3, 135D:0935) run in a second RefEmu on the port's data
/// segment; the wait loop 21FD:0004 of lane bilanz is taken from the original run itself: the port waits as many frames as the
/// original does inside 21FD:0004 and takes over its data segment at each of those retraces and at its return. Its picture is
/// not compared; at its return the port's output (VRAM, DAC, driver registers, atlas, masks and the layers' data segment
/// image) is taken over from the original as well.
/// </summary>
internal static class PlantingFrameHarness
{
    private const ushort Seg0609 = 0x0609, Seg21FD = 0x21FD, SegRetrace = 0x0D73, Seg135D = 0x135D;

    /// <summary>Phase of a key event within a frame (0 = frame start; the retrace starts at the frame start).</summary>
    private const double KeyPhase = 0.4;

    /// <summary>A key held from frame <c>From</c> for <c>Frames</c> frames (70 Hz, counted from the phase's start).</summary>
    public sealed record Press(long From, long Frames, byte Key);

    /// <summary>
    /// A run: the snapshot (week of the game, reached by four computer players), players made human (control 4: arrows,
    /// Ins), bytes set in the data segment at the entry (both sides), key presses.
    /// </summary>
    public sealed record Script(int Week, int[] Humans, (int Offset, byte[] Bytes)[] Set, Press[] Presses,
        Action<OriginalProgram>? Arrange = null);

    /// <summary>
    /// Data segment bytes the original writes in the planting phase that the core leaves to the output layers and the host
    /// (they are compared nowhere; the scene of part 2 owns them), plus the bytes of the interrupt handlers.
    /// </summary>
    public static readonly (int Offset, int Length, string Owner)[] Owned =
    [
        .. RefEmuWeekOracle.InterruptOwned.Select(r => (r.Offset, r.Length, "interrupt handlers 27C6")),
        .. PlantingDifferentialTests.OutputOwned,
        (0x0238, 2, "0B67:0196/01B5 page being shown (Vga pages)"),
        (0x082E, 4, "21FD:051F pages shown/drawn (Vga pages)"),
        (0x4516, 4, "21FD:051F far pointer of the drawn page (Vga pages)"),
        (0x10D8, 2, "21FD:04DA blend colour of the fades (Vga palettes)"),
        (0x15BA, 0x9FA, "0A96 drawing list (Vga command list)"),
        (0x3DCB, 0x300, "21FD:026C/02C9 base palette (Vga palettes)"),
        (0x5017, 0x300, "186C:0002 palette of the loaded picture (Vga palettes)"),
        (0x34A6, 0x2A0, "21AF:00CA image pointers of the object frames (Vga objects)"),
        (0x3068, 0x2A0, "21AF:00CA mask pointers of the object frames (Vga objects)"),
        (0x3438, 0x40, "21AF:022B save buffers of the objects per page (Vga objects)"),
        (0x54E8, 0x300, "21FD:03EB working palette (Vga palettes)"),
    ];

    private const long MaxFrames = 20_000;

    private static readonly Dictionary<int, Lazy<MachineSnapshot>> AtPlanting = new()
    {
        [0] = new(() => Snapshot(0)),
        [1] = new(() => Snapshot(1)),
    };

    /// <summary>Default game (four computers) run to the entry of 0609:0004 in week <paramref name="week"/>.</summary>
    private static MachineSnapshot Snapshot(int week)
    {
        var p = OriginalProgram.StartGame(TestGameData.DosDir);
        for (int w = 0; ; w++)
        {
            Assert.True(p.RunToRoutine(Seg0609, 0x0004, 600), $"0609:0004 not reached (week {p.Week})");
            if (p.Week == week) return p.Snapshot();
            Assert.True(p.Week < week, $"week {week} skipped");
        }
    }

    /// <summary>Result of a run (diagnostics and coverage of the scenarios).</summary>
    public sealed record Result(int Retraces, int OracleRetraces, int Passes, long RandomCalls, byte[] DataSegment, List<string> Events,
        int PicturesCompared, int DistinctPictures);

    /// <summary>Counter-tests of the picture comparison: a mutation of the port's output.</summary>
    public enum Mutation { None, SkipFlip, SkipObjects, FadeOutNoRetrace, WrongPicture }

    /// <summary>Retrace wait of the port's layers: notes the port frame of every wait.</summary>
    private sealed class FrameRetraces : IRetraceWait
    {
        public long CurrentFrame { get; set; }

        public List<long> Frames { get; } = [];

        public void WaitForRetrace() => Frames.Add(CurrentFrame);
    }

    /// <summary>Picture resources of the planting phase (186C:0002) from the original files.</summary>
    private sealed class Pictures : IPictureSource
    {
        public byte[] Decrunched(int resource) =>
            PowerPacker.Decrunch(File.ReadAllBytes(TestGameData.Dos(ResourceTable.Pictures[resource].Vga)));
    }

    /// <summary>The port's output: scene, layers, driver, DAC and its retrace waits.</summary>
    private sealed class PortOutput
    {
        public required FieldScene Scene { get; init; }
        public required GraphicsLayers Layers { get; init; }
        public required ModeXDriver Driver { get; init; }
        public required VgaDac Dac { get; init; }
        public required FrameRetraces Retraces { get; init; }
        public Mutation Mutation { get; init; }

        /// <summary>The output state taken over from the original (set-up, and the return of the wait loop oracle).</summary>
        public void TakeOver(OriginalProgram o)
        {
            FieldSceneHarness.SyncDriver(o, Driver);
            o.Machine.Vga.Dac.CopyTo(Dac.Registers, 0);
            var ds = o.DataSegment();
            Array.Copy(ds, Layers.Ds, Math.Min(ds.Length, Layers.Ds.Length));
        }

        public void Apply(FieldEvent e)
        {
            switch (Mutation)
            {
                case Mutation.SkipFlip when e.Kind == FieldEventKind.FlipPages:
                    Scene.Apply(new FieldEvent(FieldEventKind.Delay, 2, 0)); // the flip's two retraces without the flip
                    return;
                case Mutation.SkipObjects when e.Kind == FieldEventKind.DrawObjects:
                    return;
                case Mutation.WrongPicture when e.Kind == FieldEventKind.LoadImage:
                    Scene.Apply(new FieldEvent(FieldEventKind.LoadImage, (short)(e.Args[0] + 1)));
                    return;
                case Mutation.FadeOutNoRetrace when e.Kind == FieldEventKind.FadeOut:
                    Layers.FadeOut((ushort)e.Args[0]);
                    for (int i = 0; i < 8; i++) Scene.Apply(new FieldEvent(FieldEventKind.Delay, 5, 0));
                    return;
            }
            Scene.Apply(e);
        }
    }

    private static PortOutput SetUpOutput(OriginalProgram o, Dgroup core, Mutation mutation)
    {
        var retraces = new FrameRetraces();
        var driver = FieldSceneHarness.DriverFrom(o);
        var dac = new VgaDac();
        o.Machine.Vga.Dac.CopyTo(dac.Registers, 0);
        // texts behind far pointers (DS:37E8 …) from a copy of the original's memory
        var memory = o.Machine.Mem.ReadBlock(0, 0, 0x100000);
        var layers = new GraphicsLayers(driver, o.DataSegment(), dac, retraces)
            { DataSegment = o.Dgroup, OtherMemory = new LinearMemory(memory) };
        var scene = new FieldScene(layers, core) { Pictures = new Pictures() };
        return new PortOutput { Scene = scene, Layers = layers, Driver = driver, Dac = dac, Retraces = retraces, Mutation = mutation };
    }

    /// <summary>Real-mode memory as a flat copy (texts outside DS).</summary>
    private sealed class LinearMemory(byte[] memory) : IFarMemory
    {
        public byte Read(ushort segment, ushort offset) => memory[((segment << 4) + offset) & 0xFFFFF];

        public void Write(ushort segment, ushort offset, byte value) => memory[((segment << 4) + offset) & 0xFFFFF] = value;
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

    public static Result Run(Script script, Mutation mutation = Mutation.None)
    {
        var o = OriginalProgram.Resume(TestGameData.DosDir, AtPlanting[script.Week].Value);
        var m = o.Machine;
        Assert.True(o.IsAt(Seg0609, 0x0004), "snapshot not at 0609:0004");
        ushort retIp = m.Mem.Read16(m.Cpu.Ss, m.Cpu.Sp), retCs = m.Mem.Read16(m.Cpu.Ss, (ushort)(m.Cpu.Sp + 2));
        foreach (int h in script.Humans)
        {
            o.WriteByte((ushort)(ControlReader.IsComputer + h), 0);
            o.WriteByte((ushort)(ControlReader.ControlType + h), 4);
        }
        foreach (var (at, bytes) in script.Set)
            for (int i = 0; i < bytes.Length; i++) o.WriteByte((ushort)(at + i), bytes[i]);
        script.Arrange?.Invoke(o);

        long period = m.Vga.FramePeriod;
        long frame0 = m.FrameNumber + 1;
        double Time(long frame) => (double)((frame0 + frame) * period + (long)(KeyPhase * period)) / Traders.RefEmu.Hardware.Pit.ClockHz;
        foreach (var press in script.Presses)
        {
            m.ScheduleScancode(Time(press.From), press.Key);
            m.ScheduleScancode(Time(press.From + press.Frames), (byte)(press.Key | 0x80));
        }

        var run = new OriginalRun(o, retCs, retIp);
        var core = new Dgroup(o.DataSegment()[..DgroupLayout.Size]);
        var random = new OriginalRandom(new RecordedClock([]), core.U32(DgroupLayout.RandomSeed));
        var output = SetUpOutput(o, core, mutation);
        var host = new OracleHost(run, output);
        var step = new PlantingStep(host, ProgramStartDifferentialTests.Files);
        var machine = new PhaseMachine(core, random, new Dictionary<int, IWeekStepHandler> { [8] = step }, nextStep: 8);

        long frame = 0;
        var held = run.Held();
        output.Scene.NextFrame(held);
        machine.Advance(new PhaseTick(frame, held));
        int retraces = 0, sceneWaits = 0, pictures = 0;
        var distinct = new HashSet<ulong>();
        while (machine.Status == PhaseStatus.Waiting)
        {
            Assert.True(frame < MaxFrames, $"planting did not end within {MaxFrames} frames: modes {core.U8(P.Mode)} {core.U8(P.Mode + 1)} {core.U8(P.Mode + 2)} {core.U8(P.Mode + 3)}, time {core.S16(FieldCursor.TimeLeft)}, cursor {core.S8(FieldCursor.CursorX)},{core.S8(FieldCursor.CursorY)}, players {core.U8(P.Players)}, keys {string.Join(",", run.Held())}; {string.Join("; ", host.Events.TakeLast(30))}");
            if (!run.Positioned)
                Assert.True(run.RunToRetraceOrReturn(), $"port waits frame {frame}, the original returned (retrace {retraces})");
            run.Positioned = false;
            string where = $"retrace {retraces} (port frame {frame})";
            run.Compare(core, random.Calls + host.OracleDraws, where);
            if (host.InWaitLoop)
                Assert.True(output.Retraces.Frames.Count == sceneWaits, $"{where}: the scene waited during the wait loop oracle");
            else
            {
                sceneWaits++;
                Assert.True(output.Retraces.Frames.Count == sceneWaits && output.Retraces.Frames[^1] == frame,
                    $"{where}: the core waits its retrace {sceneWaits - 1}, the scene made {output.Retraces.Frames.Count} waits (last in frame {(output.Retraces.Frames.Count > 0 ? output.Retraces.Frames[^1] : -1)})");
                ComparePicture(o, output, where);
                pictures++;
                distinct.Add(PictureHash(o));
            }
            retraces++;
            run.RunToRetraceEnd();
            run.CheckControls();
            frame++;
            output.Retraces.CurrentFrame = frame;
            held = run.Held();
            output.Scene.NextFrame(held);
            machine.Advance(new PhaseTick(frame, held));
        }
        Assert.Equal(0, step.Polls);
        Assert.False(run.RunToRetraceOrReturn(), $"the port returned after {retraces} retraces, the original waits another");
        run.CheckControls();
        Assert.True(machine.Status == PhaseStatus.Blocked && machine.NextStep == 9, $"phase machine {machine.Status} at {machine.NextStep}");
        run.Compare(core, random.Calls + host.OracleDraws, $"return of 0609:0004 after {retraces} retraces");
        Assert.False(output.Scene.Presenting, "the scene still presents after the return");
        Assert.Equal(sceneWaits, output.Retraces.Frames.Count);
        ComparePicture(o, output, $"return of 0609:0004 after {retraces} retraces");
        pictures++;
        Assert.Equal(retraces, step.Retraces);
        return new Result(retraces, host.OracleRetraces, host.Passes, random.Calls + host.OracleDraws, core.Bytes, host.Events,
            pictures, distinct.Count);
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
            o.Machine.AddCodeHook(o.Segment(Seg21FD), 0x01BA, _ => RandomCalls++);
            o.Machine.AddCodeHook(o.Segment(0x0E9E), 0x037A, _ => _controlReads.Add((_retraceIndex, Keys())));
        }

        public OriginalProgram Program => _o;

        public long RandomCalls { get; private set; }

        /// <summary>True when the original already stands at the entry of the next retrace wait (set by the wait loop oracle).</summary>
        public bool Positioned { get; set; }

        private byte[] Keys() => _o.Machine.Mem.ReadBlock(_o.Dgroup, DgroupLayout.KeyState, KeyTable.Length);

        private bool At(ushort cs, ushort ip) => _o.Machine.Cpu.Cs == cs && _o.Machine.Cpu.IP == ip;

        /// <summary>Held keys for the next port frame: the original's key table now.</summary>
        public IReadOnlyCollection<byte> Held()
        {
            _keysGiven = Keys();
            return [.. Enumerable.Range(0, KeyTable.Length).Where(sc => _keysGiven[sc] != 0).Select(sc => (byte)sc)];
        }

        /// <summary>Runs the original to the next retrace entry (true) or the return of 0609:0004 (false).</summary>
        public bool RunToRetraceOrReturn() => RunTo((cs, ip) => cs == _retraceCs && ip == 0x00D3, true);

        /// <summary>
        /// Runs the original to the next retrace entry (true) or to <paramref name="cs"/>:<paramref name="ip"/> (false); the
        /// return of 0609:0004 must not come first.
        /// </summary>
        public bool RunToRetraceOr(ushort cs, ushort ip)
        {
            bool retrace = RunTo((c, i) => (c == _retraceCs && i == 0x00D3) || (c == cs && i == ip), false);
            if (retrace) return true;
            Assert.True(At(cs, ip), "original not at the expected stop");
            return false;
        }

        /// <summary>Runs the original to <paramref name="cs"/>:<paramref name="ip"/> without passing a retrace or the return.</summary>
        public void RunToWithoutRetrace(ushort cs, ushort ip, string what) =>
            Assert.False(RunTo((c, i) => c == cs && i == ip, false) || !At(cs, ip),
                $"original waits for a retrace (called from {Caller()}) before {what}");

        /// <summary>Far return address on the stack (where the routine entered just now was called from).</summary>
        private string Caller()
        {
            var m = _o.Machine;
            ushort ip = m.Mem.Read16(m.Cpu.Ss, m.Cpu.Sp), cs = m.Mem.Read16(m.Cpu.Ss, (ushort)(m.Cpu.Sp + 2));
            return $"{cs - _o.Segment(0):X4}:{ip:X4}";
        }

        private bool RunTo(Func<ushort, ushort, bool> stop, bool returnAllowed)
        {
            var m = _o.Machine;
            m.Run(1);
            m.Run(4_000_000_000, mm => stop(mm.Cpu.Cs, mm.Cpu.IP) || (mm.Cpu.Cs == _retCs && mm.Cpu.IP == _retIp)
                || (mm.Cpu.Cs == _retraceCs && mm.Cpu.IP == 0x00D3));
            Assert.False(m.Exited, "original exited");
            if (At(_retraceCs, 0x00D3))
            {
                _retraceIndex++;
                return true;
            }
            if (At(_retCs, _retIp))
            {
                Assert.True(returnAllowed, "0609:0004 returned unexpectedly");
                return false;
            }
            Assert.True(stop(m.Cpu.Cs, m.Cpu.IP), $"original stopped at {m.Cpu.Cs - _o.Segment(0):X4}:{m.Cpu.IP:X4}");
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
            var emu = _o.DataSegment();
            var expected = emu[..DgroupLayout.Size];
            foreach (var (offset, length, _) in Owned) Array.Copy(core.Bytes, offset, expected, offset, length);
            if (!expected.AsSpan().SequenceEqual(core.Bytes))
            {
                var diffs = Enumerable.Range(0, expected.Length).Where(i => expected[i] != core.Bytes[i]).ToList();
                var ranges = new List<string>();
                for (int k = 0; k < diffs.Count;)
                {
                    int a = diffs[k], b = a;
                    while (k + 1 < diffs.Count && diffs[k + 1] <= b + 4) b = diffs[++k];
                    k++;
                    ranges.Add($"{a:X4}-{b:X4}");
                }
                int first = diffs[0];
                Assert.Fail($"{where}: DS:{first:X4} original {expected[first]:X2}, port {core.Bytes[first]:X2}; differing {string.Join(" ", ranges.Take(40))}");
            }
            Assert.True(RandomCalls == portRandomCalls, $"{where}: original drew {RandomCalls} times, port {portRandomCalls}");
        }

        /// <summary>The original's data segment taken over by the port (the oracle steps).</summary>
        public void TakeOver(Dgroup core) => _o.DataSegment().AsSpan(0, DgroupLayout.Size).CopyTo(core.Bytes);
    }

    /// <summary>The port's host: core controls and pause; oracles for lanes anbau-ki and bilanz.</summary>
    private sealed class OracleHost(OriginalRun run, PortOutput output) : IPlantingHost
    {
        /// <summary>True while the port waits a frame of the wait loop oracle.</summary>
        public bool InWaitLoop { get; private set; }

        private readonly OriginalProgram _helper = OriginalProgram.LoadOnly(TestGameData.DosDir);

        public List<string> Events { get; } = [];

        public int OracleRetraces { get; private set; }

        public long OracleDraws { get; private set; }

        public int Passes { get; private set; }

        public void Emit(FieldEvent e)
        {
            output.Apply(e);
            if (e.Kind == FieldEventKind.FlipPages) Passes++;
            if (e.Kind is FieldEventKind.Sound or FieldEventKind.DrawField or FieldEventKind.DrawFieldAndNeighbours
                or FieldEventKind.DrawRank or FieldEventKind.DrawSelection or FieldEventKind.DrawPurchaseUsed)
                Events.Add(e.ToString());
        }

        public void ReadControls(Dgroup d)
        {
            ControlReader.ReadActions(d, KeyboardOnlyDevices.Instance);
            if (d.S8(ControlReader.Action) != -1)
                Events.Add($"Human(a {d.S8(ControlReader.Action)} mode {d.U8(P.Mode)} at {d.S8(FieldCursor.CursorX)},{d.S8(FieldCursor.CursorY)} t {d.S16(FieldCursor.TimeLeft)} pass {Passes})");
        }

        public IEnumerable<FieldWait> PauseCheck(Dgroup d) => P.Pause(d);

        public void ComputerTurn(Dgroup d, short x, short y)
        {
            Events.Add($"ComputerTurn({x}, {y})");
            Call(d, Seg135D, 0x000E, true, (ushort)x, (ushort)y);
        }

        public bool CanPlaceHq(Dgroup d) => (byte)Call(d, Seg0609, 0x074D, false) != 0;

        public bool HasOwnNeighbour(Dgroup d, short x, short y) => (byte)Call(d, Seg0609, 0x100F, false, (ushort)x, (ushort)y) == 1;

        public void ComputerPlacedHq(Dgroup d)
        {
            Events.Add("ComputerPlacedHq()");
            Call(d, Seg135D, 0x0AB3, true);
        }

        public void ComputerBoughtField(Dgroup d)
        {
            Events.Add("ComputerBoughtField()");
            Call(d, Seg135D, 0x0935, true);
        }

        /// <summary>The original routine in a second RefEmu on the port's data segment; DS is taken back if it may write.</summary>
        private ushort Call(Dgroup d, ushort seg, ushort off, bool writes, params ushort[] args)
        {
            var mem = _helper.Machine.Mem;
            mem.WriteBlock(Traders.RefEmu.Cpu.Memory.Linear(_helper.Dgroup, 0), d.Bytes.AsSpan(0, DgroupLayout.Size).ToArray());
            ushort ax = _helper.Call(seg, off, args).Ax;
            var after = _helper.DataSegment();
            if (writes) after.AsSpan(0, DgroupLayout.Size).CopyTo(d.Bytes);
            else ControlHarness.AssertSameData($"helper {seg:X4}:{off:X4} changed DS", after[..DgroupLayout.Size], d.Bytes);
            return ax;
        }

        /// <summary>
        /// 21FD:0004 from the original run: it is run to the entry (no retrace before), compared, then retrace by retrace to its
        /// return; at every retrace and at the return the port takes over its data segment.
        /// </summary>
        public IEnumerable<FieldWait> WaitLoop(Dgroup d, OriginalRandom random, short mode)
        {
            Events.Add($"WaitLoop({mode})");
            Assert.False(output.Scene.Presenting, "the scene still presents at the entry of 21FD:0004");
            var o = run.Program;
            run.RunToWithoutRetrace(o.Segment(Seg21FD), 0x0004, "21FD:0004");
            Assert.Equal(mode, (short)o.StackArgument(0));
            d.Set(DgroupLayout.RandomSeed, random.Seed);
            run.Compare(d, random.Calls + OracleDraws, "entry of 21FD:0004");
            var m = o.Machine;
            ushort retIp = m.Mem.Read16(m.Cpu.Ss, m.Cpu.Sp), retCs = m.Mem.Read16(m.Cpu.Ss, (ushort)(m.Cpu.Sp + 2));
            long drawsBefore = run.RandomCalls;
            while (run.RunToRetraceOr(retCs, retIp))
            {
                Take(d, random, drawsBefore);
                drawsBefore = run.RandomCalls;
                run.Positioned = true;
                OracleRetraces++;
                InWaitLoop = true;
                yield return FieldWait.Retrace;
                InWaitLoop = false;
            }
            Take(d, random, drawsBefore);
            output.TakeOver(o);
        }

        private void Take(Dgroup d, OriginalRandom random, long drawsBefore)
        {
            run.TakeOver(d);
            random.Seed = d.U32(DgroupLayout.RandomSeed);
            OracleDraws += run.RandomCalls - drawsBefore;
        }
    }
}
