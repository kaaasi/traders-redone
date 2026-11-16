using Traders.Assets;
using Traders.Core;
using Traders.Core.Field;
using Traders.Core.Harvest;
using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.State;
using Traders.Presentation.Harvest;
using Traders.Presentation.Menus;
using Traders.RefEmu;
using Traders.RefEmu.Machine;
using Traders.Tests.Field;
using Traders.Tests.Phases;
using Traders.Tests.Startup;
using Traders.Vga;
using Traders.Vga.Layers;

namespace Traders.Tests.Harvest;

/// <summary>
/// The harvest 22A3:02D3 frame by frame in a running game against the original. The original runs from a snapshot at the
/// entry of 22A3:02D3 (call 16 of the week loop, reached by the game itself with four computers) with live graphics and
/// sound; keys reach it as real keyboard interrupts at emulated times. A frame of the original is one retrace wait (entry
/// into 0D73:00D3). The port runs <see cref="HarvestStep"/> in a <see cref="PhaseMachine"/> from call 16, one
/// <see cref="PhaseTick"/> per frame, with the key table the original held at the end of the retrace before. At the k-th
/// retrace of the original the port must wait its k-th frame; there the data segment (outside <see cref="Owned"/>), DS:10D2,
/// the number of random() calls and the picture of the port's <see cref="HarvestScene"/> (VRAM planes, DAC, CRTC start) are
/// compared; at the return once more.
/// </summary>
internal static class HarvestFrameHarness
{
    private const ushort Seg22A3 = 0x22A3, Seg21FD = 0x21FD, SegRetrace = 0x0D73;

    /// <summary>Phase of a key event within a frame (0 = frame start; the retrace starts at the frame start).</summary>
    private const double KeyPhase = 0.4;

    /// <summary>A key held from frame <c>From</c> for <c>Frames</c> frames (70 Hz, counted from the harvest's start).</summary>
    public sealed record Press(long From, long Frames, byte Key);

    /// <summary>
    /// A run: the snapshot (week of the game), players made human (control 4: arrows, Ins), bytes set in the data segment at
    /// the entry (both sides), key presses.
    /// </summary>
    public sealed record Script(int Week, int[] Humans, (int Offset, byte[] Bytes)[] Set, Press[] Presses);

    /// <summary>
    /// Data segment bytes the original writes in the harvest that the core leaves to the output layers (the scene owns them),
    /// plus the bytes of the interrupt handlers: those of the planting phase's output and the typewriter's character and font.
    /// </summary>
    public static readonly (int Offset, int Length, string Owner)[] Owned =
    [
        .. PlantingFrameHarness.Owned,
        (0x0140, 1, "0A69:01AD character being typed (Vga text)"),
        (0x0142, 2, "23CD:05B6 via 21FD:05DB: text advance and shadow height (Vga)"),
        (0x34A4, 2, "21FD:05DB line height (Vga)"),
        (0x3CA8, 2, "21FD:05DB text advance (Vga)"),
    ];

    private const long MaxFrames = 20_000;

    private static readonly Dictionary<int, Lazy<MachineSnapshot>> AtHarvest = new()
    {
        [0] = new(() => Snapshot(0)),
        [2] = new(() => Snapshot(2)),
    };

    /// <summary>Default game (four computers) run to the entry of 22A3:02D3 in week <paramref name="week"/>.</summary>
    private static MachineSnapshot Snapshot(int week)
    {
        var p = OriginalProgram.StartGame(TestGameData.DosDir);
        while (true)
        {
            Assert.True(p.RunToRoutine(Seg22A3, 0x02D3, 900), $"22A3:02D3 not reached (week {p.Week})");
            if (p.Week == week) return p.Snapshot();
            Assert.True(p.Week < week, $"week {week} skipped");
        }
    }

    /// <summary>Result of a run (diagnostics and coverage of the scenarios).</summary>
    public sealed record Result(int Retraces, int Passes, long RandomCalls, byte[] DataSegment, List<string> Events,
        int PicturesCompared, int DistinctPictures);

    /// <summary>Counter-tests: a mutation of the port's core or output.</summary>
    public enum Mutation { None, SkipLine, SkipTypedCharacter, DoubleFrame }

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

    private sealed class PortOutput
    {
        public required HarvestScene Scene { get; init; }
        public required ModeXDriver Driver { get; init; }
        public required VgaDac Dac { get; init; }
        public required FrameRetraces Retraces { get; init; }
        public Mutation Mutation { get; init; }
    }

    private static PortOutput SetUpOutput(OriginalProgram o, Dgroup core, Mutation mutation, byte[] memory)
    {
        var retraces = new FrameRetraces();
        var driver = FieldSceneHarness.DriverFrom(o);
        var dac = new VgaDac();
        o.Machine.Vga.Dac.CopyTo(dac.Registers, 0);
        var layers = new GraphicsLayers(driver, o.DataSegment(), dac, retraces)
            { DataSegment = o.Dgroup, OtherMemory = new LinearMemory(memory) };
        var scene = new HarvestScene(layers, core, new Pictures());
        return new PortOutput { Scene = scene, Driver = driver, Dac = dac, Retraces = retraces, Mutation = mutation };
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
        var o = OriginalProgram.Resume(TestGameData.DosDir, AtHarvest[script.Week].Value);
        var m = o.Machine;
        Assert.True(o.IsAt(Seg22A3, 0x02D3), "snapshot not at 22A3:02D3");
        ushort retIp = m.Mem.Read16(m.Cpu.Ss, m.Cpu.Sp), retCs = m.Mem.Read16(m.Cpu.Ss, (ushort)(m.Cpu.Sp + 2));
        foreach (int h in script.Humans)
        {
            o.WriteByte((ushort)(ControlReader.IsComputer + h), 0);
            o.WriteByte((ushort)(ControlReader.ControlType + h), 4);
        }
        foreach (var (at, bytes) in script.Set)
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
        var core = new Dgroup(o.DataSegment()[..DgroupLayout.Size]);
        var random = new OriginalRandom(new RecordedClock([]), core.U32(DgroupLayout.RandomSeed));
        var memory = o.Machine.Mem.ReadBlock(0, 0, 0x100000); // the text buffer of the program start (far heap)
        var output = SetUpOutput(o, core, mutation, memory);
        var events = new List<string>();
        int passes = 0;
        var host = new HarvestHost(e =>
        {
            if (e.Kind == FieldEventKind.FlipPages) passes++;
            if (e.Kind == FieldEventKind.Sound) events.Add(e.ToString());
            output.Scene.Apply(e);
        }, e =>
        {
            events.Add(e.ToString());
            if (mutation == Mutation.SkipLine && e.Kind == HarvestEventKind.Line) return;
            if (mutation == Mutation.SkipTypedCharacter && e.Kind == HarvestEventKind.TypeCharacter && e.Args[2] == 1) return;
            output.Scene.Apply(e);
        }, (offset, segment) =>
        {
            var bytes = new List<byte>();
            for (int i = (segment << 4) + offset; memory[i & 0xFFFFF] != 0; i++) bytes.Add(memory[i & 0xFFFFF]);
            return [.. bytes];
        });
        var step = new HarvestStep(host, ProgramStartDifferentialTests.Files);
        IWeekStepHandler handler = mutation == Mutation.DoubleFrame ? new DoubleTick(step, 300) : step;
        var machine = new PhaseMachine(core, random, new Dictionary<int, IWeekStepHandler> { [16] = handler }, nextStep: 16);

        long frame = 0;
        var held = run.Held();
        output.Scene.NextFrame(held);
        machine.Advance(new PhaseTick(frame, held));
        int retraces = 0, sceneWaits = 0, pictures = 0;
        var distinct = new HashSet<ulong>();
        while (machine.Status == PhaseStatus.Waiting)
        {
            Assert.True(frame < MaxFrames, $"harvest did not end within {MaxFrames} frames: modes {core.U8(FieldCursor.Mode)} " +
                $"{core.U8(FieldCursor.Mode + 1)} {core.U8(FieldCursor.Mode + 2)} {core.U8(FieldCursor.Mode + 3)}, players {core.U8(Planting.Players)}");
            Assert.True(run.RunToRetraceOrReturn(), $"port waits frame {frame}, the original returned (retrace {retraces})");
            string where = $"retrace {retraces} (port frame {frame})";
            run.Compare(core, random.Calls, where);
            sceneWaits++;
            Assert.True(output.Retraces.Frames.Count == sceneWaits && output.Retraces.Frames[^1] == frame,
                $"{where}: the core waits its retrace {sceneWaits - 1}, the scene made {output.Retraces.Frames.Count} waits (last in frame {(output.Retraces.Frames.Count > 0 ? output.Retraces.Frames[^1] : -1)})");
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
            machine.Advance(new PhaseTick(frame, held));
        }
        Assert.Equal(0, step.Polls);
        Assert.False(run.RunToRetraceOrReturn(), $"the port returned after {retraces} retraces, the original waits another");
        run.CheckControls();
        Assert.True(machine.Status == PhaseStatus.Blocked && machine.NextStep == 18, $"phase machine (17 = marker 3, 18 = robots 1C68) {machine.Status} at {machine.NextStep}");
        run.Compare(core, random.Calls, $"return of 22A3:02D3 after {retraces} retraces");
        Assert.False(output.Scene.Presenting, "the scene still presents after the return");
        Assert.Equal(sceneWaits, output.Retraces.Frames.Count);
        ComparePicture(o, output, $"return of 22A3:02D3 after {retraces} retraces");
        pictures++;
        Assert.Equal(retraces, step.Retraces);
        return new Result(retraces, passes, random.Calls, core.Bytes, events, pictures, distinct.Count);
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
            o.Machine.AddCodeHook(o.Segment(Seg21FD), 0x01BA, _ => RandomCalls++);
            o.Machine.AddCodeHook(o.Segment(0x0E9E), 0x037A, _ => _controlReads.Add((_retraceIndex, Keys())));
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

        /// <summary>Runs the original to the next retrace entry (true) or the return of 22A3:02D3 (false).</summary>
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
