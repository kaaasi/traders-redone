using Traders.Assets;
using Traders.Core;
using Traders.Core.Disasters;
using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.Pixels;
using Traders.Core.State;
using Traders.Presentation.Disasters;
using Traders.Presentation.Menus;
using Traders.RefEmu;
using Traders.RefEmu.Machine;
using Traders.Tests.Field;
using Traders.Tests.Input;
using Traders.Tests.Startup;
using Traders.Vga;
using Traders.Vga.Layers;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Tests.Disasters;

/// <summary>
/// The catastrophes 111D:0009 frame by frame in a running game against the original (lane katastrophen part 1). The original
/// runs from a snapshot at the entry of 111D:0009 (call 4 of the week loop, reached by the game itself with four computers)
/// with live graphics and sound; keys reach it as real keyboard interrupts at emulated times. A frame of the original is one
/// retrace wait (entry into 0D73:00D3). The port runs <see cref="CatastropheGuardStep"/> with <see cref="CatastropheStep"/>
/// and the production host <see cref="DisasterHost"/> (pause check, wait loop 21FD:0004 of lane bilanz) in a
/// <see cref="PhaseMachine"/> from call 4, one <see cref="PhaseTick"/> per frame, with the key table the original held at the
/// end of the retrace before. At the k-th retrace of the original the port must wait its k-th frame; there the data segment
/// (outside <see cref="Owned"/>), DS:10D2 and the number of random() calls are compared, at the return the whole data segment
/// again. The kind that struck is taken from the original (entries of 111D:0264, 03B7, 0519). Expected values come only from
/// the RefEmu; the port's random generator only chooses input states.
/// </summary>
internal static class CatastropheFrameHarness
{
    private const ushort Seg111D = 0x111D, Seg21FD = 0x21FD, SegRetrace = 0x0D73;

    /// <summary>Phase of a key event within a frame (0 = frame start; the retrace starts at the frame start).</summary>
    private const double KeyPhase = 0.4;

    /// <summary>Scancodes of control 4: Ins (fire), P (pause).</summary>
    public const byte Ins = 0x52, P = 0x19;

    /// <summary>A key held from frame <c>From</c> for <c>Frames</c> frames (counted from the entry of 111D:0009).</summary>
    public sealed record Press(long From, long Frames, byte Key);

    /// <summary>
    /// A run: week of the snapshot, players made human (control 4), the random state DS:10D2 at the entry (null: as the game
    /// left it), key presses.
    /// </summary>
    public sealed record Script(int Week, int[] Humans, uint? Random, Press[] Presses)
    {
        public override string ToString() =>
            $"week {Week}, humans [{string.Join(",", Humans)}], random {(Random is { } r ? r.ToString("X8") : "game")}, {Presses.Length} presses";
    }

    /// <summary>
    /// Data segment bytes the original writes in 111D:0009 that the core leaves to the output layers: those of the planting
    /// harness (pages, palettes, list, objects, interrupt handlers), the character of the typewriter 0A69:01AD and the draw-page
    /// switch DS:023A (21FD:0590 in 111D:0519; the core's rule page keeps its own draw page).
    /// </summary>
    public static readonly (int Offset, int Length, string Owner)[] Owned =
    [
        .. PlantingFrameHarness.Owned,
        (0x0140, 1, "0A69:01AD character being typed (Vga draw helpers)"),
        (0x023A, 2, "0B67:01D4/0206 draw on the page shown / hidden, set by 21FD:0590 and every flip (Vga pages)"),
    ];

    private const long MaxFrames = 20_000;

    private static readonly Dictionary<int, Lazy<MachineSnapshot>> AtCatastrophes = new()
    {
        [3] = new(() => Snapshot(3)),
        [4] = new(() => Snapshot(4)),
        [5] = new(() => Snapshot(5)),
    };

    /// <summary>Default game (four computers) run to the entry of 111D:0009 in week <paramref name="week"/>.</summary>
    private static MachineSnapshot Snapshot(int week)
    {
        var p = OriginalProgram.StartGame(TestGameData.DosDir);
        while (true)
        {
            Assert.True(p.RunToRoutine(Seg111D, 0x0009, 600), $"111D:0009 not reached (week {p.Week})");
            if (p.Week == week) return p.Snapshot();
            Assert.True(p.Week < week, $"week {week} skipped");
        }
    }

    /// <summary>The kind as the original took it: −1 none, 0 (0264), 1 (03B7), 2 (0519).</summary>
    public sealed record Result(int Kind, int Retraces, int Polls, long RandomCalls, int Typed, int WaitLoopFrames, byte[] DataSegment,
        int PicturesCompared);

    /// <summary>
    /// The picture (part 2): the port's <see cref="DisasterScene"/> plays every event of the core through the graphics layers
    /// frame by frame (<see cref="DisasterScene.NextFrame"/> before each frame); it must make its k-th retrace wait in the port
    /// frame of the k-th retrace, and there the four VRAM planes, the DAC and the CRTC start equal the original's at the entry
    /// of its retrace wait, and so does the command list 0A96 (DS:15BA) of the layers; at the return once more.
    /// </summary>
    private sealed class PortOutput
    {
        public required DisasterScene Scene { get; init; }
        public required ModeXDriver Driver { get; init; }
        public required VgaDac Dac { get; init; }
        public required FrameRetraces Retraces { get; init; }
        public required GraphicsLayers Layers { get; init; }
    }

    private sealed class FrameRetraces : IRetraceWait
    {
        public long CurrentFrame { get; set; }

        public List<long> Frames { get; } = [];

        public void WaitForRetrace() => Frames.Add(CurrentFrame);
    }

    private sealed class PictureFiles : IPictureSource
    {
        public byte[] Decrunched(int resource) =>
            PowerPacker.Decrunch(File.ReadAllBytes(TestGameData.Dos(ResourceTable.Pictures[resource].Vga)));
    }

    private sealed class LinearMemory(byte[] memory) : IFarMemory
    {
        public byte Read(ushort segment, ushort offset) => memory[((segment << 4) + offset) & 0xFFFFF];

        public void Write(ushort segment, ushort offset, byte value) => memory[((segment << 4) + offset) & 0xFFFFF] = value;
    }

    private static PortOutput SetUpOutput(OriginalProgram o, Dgroup core)
    {
        var retraces = new FrameRetraces();
        var driver = FieldSceneHarness.DriverFrom(o);
        var dac = new VgaDac();
        o.Machine.Vga.Dac.CopyTo(dac.Registers, 0);
        var memory = o.Machine.Mem.ReadBlock(0, 0, 0x100000);
        var layers = new GraphicsLayers(driver, o.DataSegment(), dac, retraces)
            { DataSegment = o.Dgroup, OtherMemory = new LinearMemory(memory) };
        var scene = new DisasterScene(layers, core) { Pictures = new PictureFiles() };
        return new PortOutput { Scene = scene, Driver = driver, Dac = dac, Retraces = retraces, Layers = layers };
    }

    private const int CommandList = 0x15BA, CommandListLength = 0x9FA;

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
        // the command list 0A96 (DS:15BA, 9FAh bytes) of the layers equals the original's
        var list = o.DataSegment().AsSpan(CommandList, CommandListLength);
        int l = list.CommonPrefixLength(port.Layers.Ds.AsSpan(CommandList, CommandListLength));
        if (l < CommandListLength)
            Assert.Fail($"{where}: command list DS:{CommandList + l:X4} original {list[l]:X2} port {port.Layers.Ds[CommandList + l]:X2}");
        int d = vga.Dac.AsSpan().CommonPrefixLength(port.Dac.Registers);
        if (d < port.Dac.Registers.Length)
            Assert.Fail($"{where}: DAC colour {d / 3} component {d % 3} original {vga.Dac[d]:X2} port {port.Dac.Registers[d]:X2}");
    }

    private static readonly Lazy<IndexedImage[]> Pictures = new(() =>
        [.. ResourceTable.Pictures.OrderBy(e => (int)e.Id).Select(e => Picture(e.Vga))]);

    private static IndexedImage Picture(string file)
    {
        var image = VgaImage.Load(TestGameData.Dos(file));
        return new IndexedImage(VgaImage.Width, VgaImage.Height, image.Pixels);
    }

    /// <summary>
    /// A random state from which 111D:001E chooses <paramref name="kind"/> (−1: none) in the snapshot's week, found with the
    /// port's generator (input choice only; the test asserts the kind the original took).
    /// </summary>
    public static uint RandomFor(int week, int kind)
    {
        var d = new Dgroup(new byte[L.Size]);
        d.Set(L.Week, (short)week);
        for (uint s = 1; ; s++)
        {
            uint seed = s * 0x9E3779B9u;
            var r = new OriginalRandom(new RecordedClock([]), seed);
            short? k = Catastrophes.Choose(d, r);
            if ((k ?? -1) == kind) return seed;
        }
    }

    public static Result Run(Script script, bool picture = false)
    {
        var o = OriginalProgram.Resume(TestGameData.DosDir, AtCatastrophes[script.Week].Value);
        var m = o.Machine;
        Assert.True(o.IsAt(Seg111D, 0x0009), "snapshot not at 111D:0009");
        ushort retIp = m.Mem.Read16(m.Cpu.Ss, m.Cpu.Sp), retCs = m.Mem.Read16(m.Cpu.Ss, (ushort)(m.Cpu.Sp + 2));
        o.WriteByte(L.Catastrophes, 1);
        foreach (int h in script.Humans)
        {
            o.WriteByte((ushort)(ControlReader.IsComputer + h), 0);
            o.WriteByte((ushort)(ControlReader.ControlType + h), 4);
        }
        if (script.Random is { } seed) o.WriteDword(L.RandomSeed, seed);

        int kind = -1;
        ushort own = o.Segment(Seg111D);
        m.AddCodeHook(own, 0x0264, _ => kind = 0);
        m.AddCodeHook(own, 0x03B7, _ => kind = 1);
        m.AddCodeHook(own, 0x0519, _ => kind = 2);

        long period = m.Vga.FramePeriod;
        long frame0 = m.FrameNumber + 1;
        double Time(long frame) => (double)((frame0 + frame) * period + (long)(KeyPhase * period)) / Traders.RefEmu.Hardware.Pit.ClockHz;
        foreach (var press in script.Presses)
        {
            m.ScheduleScancode(Time(press.From), press.Key);
            m.ScheduleScancode(Time(press.From + press.Frames), (byte)(press.Key | 0x80));
        }

        var run = new OriginalRun(o, retCs, retIp);
        var core = new Dgroup(o.DataSegment()[..L.Size]);
        var random = new OriginalRandom(new RecordedClock([]), core.U32(L.RandomSeed));
        var page = new RulePage(RulePage.DrawPageOf(core));
        var output = picture ? SetUpOutput(o, core) : null;
        var host = new CountingHost(output is null ? null : output.Scene.Apply);
        var step = new CatastropheStep(host, ProgramStartDifferentialTests.Files, page, n => Pictures.Value[n],
            DisasterTestTexts.From(o));
        var machine = new PhaseMachine(core, random,
            new Dictionary<int, IWeekStepHandler> { [CatastropheGuardStep.Call] = new CatastropheGuardStep(step) },
            nextStep: CatastropheGuardStep.Call);

        long frame = 0;
        int pictures = 0;
        var held = run.Held();
        output?.Scene.NextFrame(held);
        machine.Advance(new PhaseTick(frame, held));
        int retraces = 0;
        while (machine.Status == PhaseStatus.Waiting)
        {
            Assert.True(frame < MaxFrames, $"111D:0009 did not end within {MaxFrames} frames");
            Assert.True(run.RunToRetraceOrReturn(), $"port waits frame {frame}, the original returned (retrace {retraces}); last events {string.Join("; ", host.Last)}");
            string where = $"retrace {retraces} (port frame {frame}); last events {string.Join("; ", host.Last)}";
            run.Compare(core, random.Calls, where);
            if (output is not null)
            {
                Assert.True(output.Retraces.Frames.Count == retraces + 1 && output.Retraces.Frames[^1] == frame,
                    $"{where}: the core waits its retrace {retraces}, the scene made {output.Retraces.Frames.Count} waits (last in frame {(output.Retraces.Frames.Count > 0 ? output.Retraces.Frames[^1] : -1)})");
                ComparePicture(o, output, where);
                pictures++;
            }
            retraces++;
            run.RunToRetraceEnd();
            run.CheckControls();
            frame++;
            held = run.Held();
            if (output is not null)
            {
                output.Retraces.CurrentFrame = frame;
                output.Scene.NextFrame(held);
            }
            machine.Advance(new PhaseTick(frame, held));
        }
        Assert.False(run.RunToRetraceOrReturn(), $"the port returned after {retraces} retraces, the original waits another");
        run.CheckControls();
        Assert.True(machine.Status == PhaseStatus.Blocked && machine.NextStep == CatastropheGuardStep.Call + 1,
            $"phase machine {machine.Status} at {machine.NextStep}");
        run.Compare(core, random.Calls, $"return of 111D:0009 after {retraces} retraces");
        Assert.Equal(retraces, step.Retraces);
        if (output is not null)
        {
            Assert.False(output.Scene.Presenting, "the scene still presents after the return");
            Assert.Equal(retraces, output.Retraces.Frames.Count);
            ComparePicture(o, output, $"return of 111D:0009 after {retraces} retraces");
            pictures++;
        }
        return new Result(kind, retraces, step.Polls, random.Calls, host.Typed, host.WaitLoopFrames, core.Bytes, pictures);
    }

    /// <summary>The production host; counts typed characters and the wait loop's frames, keeps the last events.</summary>
    private sealed class CountingHost : IDisasterHost
    {
        private readonly DisasterHost _host;

        public CountingHost(Action<DisasterEvent>? scene) => _host = new DisasterHost(e =>
        {
            scene?.Invoke(e);
            if (e.Kind == DisasterEventKind.TypeCharacter) Typed++;
            Last.Enqueue(e.ToString());
            if (Last.Count > 8) Last.Dequeue();
        });

        public int Typed { get; private set; }

        public int WaitLoopFrames { get; private set; }

        public Queue<string> Last { get; } = new();

        public void Emit(DisasterEvent e) => _host.Emit(e);

        public IEnumerable<Traders.Core.Field.FieldWait> PauseCheck(Dgroup d) => _host.PauseCheck(d);

        public IEnumerable<Traders.Core.Field.FieldWait> WaitLoop(Dgroup d, OriginalRandom random, short mode)
        {
            foreach (var w in _host.WaitLoop(d, random, mode))
            {
                WaitLoopFrames++;
                yield return w;
            }
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

        private byte[] Keys() => _o.Machine.Mem.ReadBlock(_o.Dgroup, L.KeyState, KeyTable.Length);

        private bool At(ushort cs, ushort ip) => _o.Machine.Cpu.Cs == cs && _o.Machine.Cpu.IP == ip;

        /// <summary>Held keys for the next port frame: the original's key table now.</summary>
        public IReadOnlyCollection<byte> Held()
        {
            _keysGiven = Keys();
            return [.. Enumerable.Range(0, KeyTable.Length).Where(sc => _keysGiven[sc] != 0).Select(sc => (byte)sc)];
        }

        /// <summary>Runs the original to the next retrace entry (true) or the return of 111D:0009 (false).</summary>
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
            var expected = _o.DataSegment()[..L.Size];
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
    }
}
