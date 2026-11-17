using Traders.Assets;
using Traders.Core;
using Traders.Core.FatMike;
using Traders.Core.Field;
using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.State;
using Traders.Presentation.FatMike;
using Traders.Presentation.Menus;
using Traders.RefEmu;
using Traders.RefEmu.Machine;
using Traders.Tests.Harvest;
using Traders.Tests.Phases;
using Traders.Tests.Startup;
using Traders.Vga;
using Traders.Vga.Layers;

namespace Traders.Tests.FatMike;

/// <summary>
/// Fat Mike 1F63:000F (call 26) and the contest 1F63:0B03 (call 30) frame by frame in a running game against the original.
/// The original runs from a snapshot at the routine's entry (reached by the game itself with four computers) with live graphics
/// and sound; keys reach it as real keyboard interrupts at emulated times. A frame of the original is one retrace wait (entry
/// into 0D73:00D3). The port runs <see cref="FatMikeStep"/> or <see cref="ContestStep"/> in a <see cref="PhaseMachine"/> from
/// its call, one <see cref="PhaseTick"/> per frame, with the key table the original held at the end of the retrace before, on
/// the production host <see cref="FatMikeHost"/> (wait loop 21FD:0004 of lane bilanz; for the contest the second field round
/// on <see cref="ContestFieldHost"/> with the computer turn of lane anbau-ki). The port's <see cref="FatMikeScene"/> plays
/// every event through the graphics layers frame by frame. At the k-th retrace of the original the port must wait its k-th
/// frame and the scene make its k-th retrace wait; there the data segment (outside <see cref="Owned"/>), DS:10D2, the number of
/// random() calls, the picture (four VRAM planes, DAC, CRTC start) and the command list 0A96 (count and used entries) are
/// compared; at the return once more.
/// </summary>
internal static class FatMikeFrameHarness
{
    private const ushort Seg1F63 = 0x1F63, Seg21FD = 0x21FD, SegRetrace = 0x0D73;

    /// <summary>Phase of a key event within a frame (0 = frame start; the retrace starts at the frame start).</summary>
    private const double KeyPhase = 0.4;

    /// <summary>The routine under test: Fat Mike (call 26) or the contest (call 30).</summary>
    public enum Routine { FatMike, Contest }

    /// <summary>A key held from frame <c>From</c> for <c>Frames</c> frames (70 Hz, counted from the routine's start).</summary>
    public sealed record Press(long From, long Frames, byte Key);

    /// <summary>
    /// A run: routine and week of the game (snapshot at its entry), players made human (control 4: arrows, Ins), bytes set in
    /// the data segment at the entry (both sides), key presses.
    /// </summary>
    public sealed record Script(Routine Routine, int Week, int[] Humans, (int Offset, byte[] Bytes)[] Set, Press[] Presses);

    /// <summary>Data segment bytes the output layers own (the harvest's set: planting output, typewriter, font).</summary>
    public static readonly (int Offset, int Length, string Owner)[] Owned = HarvestFrameHarness.Owned;

    private const long MaxFrames = 30_000;

    /// <summary>One default game (four computers), run on to the entries asked for; snapshots by routine and week.</summary>
    private static class Snapshots
    {
        private static readonly object Gate = new();
        private static readonly Dictionary<(Routine, int), MachineSnapshot> Taken = [];
        private static OriginalProgram? _game;
        private static (Routine, int) _last = (Routine.Contest, -1);

        public static MachineSnapshot At(Routine routine, int week)
        {
            lock (Gate)
            {
                if (Taken.TryGetValue((routine, week), out var s)) return s;
                _game ??= OriginalProgram.StartGame(TestGameData.DosDir);
                while (true)
                {
                    // the entries alternate: 1F63:000F (call 26), then 1F63:0B03 (call 30) of the same week
                    var next = _last.Item1 == Routine.Contest ? (Routine.FatMike, _last.Item2 + 1) : (Routine.Contest, _last.Item2);
                    Assert.True(next.Item2 <= week, $"{routine} of week {week} already passed");
                    Assert.True(_game.RunToRoutine(Seg1F63, next.Item1 == Routine.FatMike ? (ushort)0x000F : (ushort)0x0B03, 900),
                        $"{next.Item1} not reached (week {_game.Week})");
                    Assert.Equal(next.Item2, _game.Week);
                    Taken[next] = _game.Snapshot();
                    _last = next;
                    _game.Machine.RunFor(0.05);
                    if (next == (routine, week)) return Taken[next];
                }
            }
        }
    }

    /// <summary>The original's data segment at the entry of <paramref name="routine"/> in <paramref name="week"/> (scenario set-up).</summary>
    public static byte[] DataSegmentAt(Routine routine, int week) =>
        OriginalProgram.Resume(TestGameData.DosDir, Snapshots.At(routine, week)).DataSegment()[..DgroupLayout.Size];

    /// <summary>Result of a run (diagnostics and coverage of the scenarios).</summary>
    public sealed record Result(int Retraces, long RandomCalls, byte[] DataSegment, List<string> Events, int PicturesCompared,
        int DistinctPictures);

    /// <summary>Counter-tests: a mutation of the port's output or phase machine.</summary>
    public enum Mutation { None, SkipIndicator, SkipTypedCharacter, SkipFlash, StockWithoutShadow, DoubleFrame }

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
        public required FatMikeScene Scene { get; init; }
        public required ModeXDriver Driver { get; init; }
        public required VgaDac Dac { get; init; }
        public required FrameRetraces Retraces { get; init; }
    }

    private static PortOutput SetUpOutput(OriginalProgram o, Dgroup core, byte[] memory)
    {
        var retraces = new FrameRetraces();
        var driver = Traders.Tests.Field.FieldSceneHarness.DriverFrom(o);
        var dac = new VgaDac();
        o.Machine.Vga.Dac.CopyTo(dac.Registers, 0);
        var layers = new GraphicsLayers(driver, o.DataSegment(), dac, retraces)
            { DataSegment = o.Dgroup, OtherMemory = new LinearMemory(memory) };
        var scene = new FatMikeScene(layers, core, new Pictures());
        return new PortOutput { Scene = scene, Driver = driver, Dac = dac, Retraces = retraces };
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

    /// <summary>
    /// The command list 0A96: count DS:15BE and its used entries (count · 1Ch bytes from DS:15C0) of the original's data segment
    /// against the port's layers (first difference named).
    /// </summary>
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

    private static ulong PictureHash(OriginalProgram o) =>
        Traders.Tests.Presentation.MenuSceneHarness.Hash(o.Machine.Vga.Planes, o.Machine.Vga.Dac, o.Machine.Vga.StartAddress);

    public static Result Run(Script script, Mutation mutation = Mutation.None)
    {
        bool contest = script.Routine == Routine.Contest;
        ushort entry = contest ? (ushort)0x0B03 : (ushort)0x000F;
        var o = OriginalProgram.Resume(TestGameData.DosDir, Snapshots.At(script.Routine, script.Week));
        var m = o.Machine;
        Assert.True(o.IsAt(Seg1F63, entry), $"snapshot not at 1F63:{entry:X4}");
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
        var output = SetUpOutput(o, core, memory);
        var events = new List<string>();
        byte[] FarString(ushort offset, ushort segment)
        {
            var bytes = new List<byte>();
            for (int i = (segment << 4) + offset; memory[i & 0xFFFFF] != 0; i++) bytes.Add(memory[i & 0xFFFFF]);
            return [.. bytes];
        }
        void Field(FieldEvent e)
        {
            if (e.Kind is FieldEventKind.Sound or FieldEventKind.DrawStatus or FieldEventKind.DrawFieldAndNeighbours
                or FieldEventKind.LastFrame)
                events.Add(e.ToString());
            output.Scene.Apply(e);
        }
        var secondRound = new ContestFieldHost(Field, FarString, null, core, ProgramStartDifferentialTests.Files);
        var host = new FatMikeHost(Field, e =>
        {
            if (e.Kind != FatMikeEventKind.CopyAtlas) events.Add(e.ToString());
            switch (mutation)
            {
                case Mutation.SkipIndicator when e.Kind == FatMikeEventKind.Indicator:
                case Mutation.SkipTypedCharacter when e.Kind == FatMikeEventKind.TypeCharacter && e.Args[2] == 2:
                case Mutation.SkipFlash when e.Kind == FatMikeEventKind.DacColour && e.Args[1] == 0xFFF:
                    return;
                case Mutation.StockWithoutShadow when e.Kind == FatMikeEventKind.ShadowNumber && e.Args[4] == 3:
                    output.Scene.Apply(e with { Args = [.. e.Args[..7], 0, 0] });
                    return;
            }
            output.Scene.Apply(e);
        }, FarString, null, secondRound);
        IWeekStepHandler step = contest
            ? new ContestStep(host, ProgramStartDifferentialTests.Files)
            : new FatMikeStep(host, ProgramStartDifferentialTests.Files);
        int call = contest ? 30 : 26;
        IWeekStepHandler handler = mutation == Mutation.DoubleFrame ? new DoubleTick(step, 200) : step;
        var machine = new PhaseMachine(core, random, new Dictionary<int, IWeekStepHandler> { [call] = handler }, nextStep: call);

        long frame = 0;
        var held = run.Held();
        output.Scene.NextFrame(held);
        machine.Advance(new PhaseTick(frame, held));
        int retraces = 0, sceneWaits = 0, pictures = 0;
        var distinct = new HashSet<ulong>();
        while (machine.Status == PhaseStatus.Waiting)
        {
            Assert.True(frame < MaxFrames, $"{script.Routine} did not end within {MaxFrames} frames; {string.Join("; ", events.TakeLast(20))}");
            Assert.True(run.RunToRetraceOrReturn(), $"port waits frame {frame}, the original returned (retrace {retraces})");
            string where = $"retrace {retraces} (port frame {frame})";
            run.Compare(core, random.Calls, where, 0);
            sceneWaits++;
            Assert.True(output.Retraces.Frames.Count == sceneWaits && output.Retraces.Frames[^1] == frame,
                $"{where}: the core waits its retrace {sceneWaits - 1}, the scene made {output.Retraces.Frames.Count} waits (last in frame {(output.Retraces.Frames.Count > 0 ? output.Retraces.Frames[^1] : -1)})");
            ComparePicture(o, output, where);
            CompareList(o, output, where);
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
        Assert.False(run.RunToRetraceOrReturn(), $"the port returned after {retraces} retraces, the original waits another");
        run.CheckControls();
        // after call 30 the machine increments the week counter DS:4B9A (1777:02D3), which the original does after the return
        int next = contest ? 1 : 27;
        Assert.True(machine.Status == PhaseStatus.Blocked && machine.NextStep == next, $"phase machine {machine.Status} at {machine.NextStep}");
        run.Compare(core, random.Calls, $"return of 1F63:{entry:X4} after {retraces} retraces", contest ? 1 : 0);
        Assert.False(output.Scene.Presenting, "the scene still presents after the return");
        Assert.Equal(sceneWaits, output.Retraces.Frames.Count);
        ComparePicture(o, output, $"return of 1F63:{entry:X4} after {retraces} retraces");
        CompareList(o, output, $"return of 1F63:{entry:X4} after {retraces} retraces");
        pictures++;
        int stepRetraces = step is ContestStep cs ? cs.Retraces : ((FatMikeStep)step).Retraces;
        int stepPolls = step is ContestStep cp ? cp.Polls : ((FatMikeStep)step).Polls;
        Assert.Equal(retraces, stepRetraces);
        Assert.Equal(0, stepPolls);
        return new Result(retraces, random.Calls, core.Bytes, events, pictures, distinct.Count);
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

        /// <summary>Runs the original to the next retrace entry (true) or the routine's return (false).</summary>
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

        /// <param name="weekAhead">Added to the original's week counter DS:4B9A (the port's machine is past call 30).</param>
        public void Compare(Dgroup core, long portRandomCalls, string where, int weekAhead)
        {
            var expected = _o.DataSegment()[..DgroupLayout.Size];
            foreach (var (offset, length, _) in Owned) Array.Copy(core.Bytes, offset, expected, offset, length);
            if (weekAhead != 0)
            {
                var week = new Dgroup(expected);
                week.Set(DgroupLayout.Week, Int16Math.S(week.S16(DgroupLayout.Week) + weekAhead));
            }
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
