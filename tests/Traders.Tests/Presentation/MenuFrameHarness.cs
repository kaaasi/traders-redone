using Traders.Core;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.Menus;
using Traders.Vga.Layers;

namespace Traders.Tests.Presentation;

/// <summary>
/// The main menu 192D:0007 frame by frame against the original (#86). The original runs from the snapshot at its entry
/// with live graphics and no frame-boundary hooks: keys reach it as real keyboard interrupts (<c>ScheduleKey</c>) at
/// emulated times with hold durations, like a human at the keyboard; a frame of the original is one retrace wait
/// (entry into 0D73:00D3). At every retrace the original's VRAM (four planes), DAC, CRTC start, DS:10D2 and the number of
/// random() calls are captured.
///
/// The port runs <see cref="MenuRunner"/> and <see cref="MenuScene"/> one frame per <see cref="PhaseTick"/>. Its frames are
/// either retrace frames (the core waits on <see cref="MenuWait.Flip"/>/<see cref="MenuWait.Retrace"/>) or poll frames
/// (<see cref="MenuWait.Poll"/>: the original's busy key loops, which wait for no retrace). The k-th retrace frame of the
/// port is compared with the k-th retrace of the original; the scene must have made its k-th retrace wait in exactly that
/// frame. Keys of the port per frame are what the original's key table held at the same point: after a retrace frame the
/// table at the end of that retrace, in poll frames the next state the keyboard interrupt left while the original polled.
/// A key change while the original neither waited for a retrace nor polled would make that mapping ambiguous; the harness
/// rejects such a script.
/// </summary>
internal static class MenuFrameHarness
{
    private const ushort Seg192D = 0x192D, Seg21FD = 0x21FD, SegRetrace = 0x0D73, SegKeyboard = 0x27C6;

    /// <summary>Phase of a key event within a frame (0 = frame start): about halfway between two retrace starts.</summary>
    private const double KeyPhase = 0.4;

    /// <summary>A key held from frame <c>From</c> for <c>Frames</c> frames (70 Hz, counted from the menu's start).</summary>
    public sealed record Press(long From, long Frames, byte Key);

    /// <summary>Script: presses, scripted save/load/calibration results, bytes set in the data segment before the start.</summary>
    public sealed record Script(Press[] Presses, bool[] Loads, bool[] Saves, bool[] Calibrations, (int Offset, byte Value)[]? Set = null);

    /// <summary>State at a retrace: VRAM/DAC/start hash, random state, random() calls so far.</summary>
    public sealed record Frame(ulong Hash, int DisplayStart, uint Seed, int RandomCalls);

    private enum ChangeKind { InRetraceWait, InPresenting, InPoll, Elsewhere }

    private sealed class OriginalRun
    {
        public List<Frame> Retraces { get; } = [];

        /// <summary>Real (70 Hz) frame number of every retrace, relative to the start.</summary>
        public List<long> RealFrames { get; } = [];

        /// <summary>Key table at the end of every retrace wait.</summary>
        public List<byte[]> KeysAfter { get; } = [];

        /// <summary>Key table after every keyboard interrupt in a poll loop, by the retrace before it (−1: before the first).</summary>
        public Dictionary<int, List<byte[]>> PollStates { get; } = [];

        public List<string> Ambiguous { get; } = [];

        /// <summary>Every key change with its place (diagnostics).</summary>
        public List<string> Changes { get; } = [];

        public byte[] DataSegment { get; set; } = [];

        public int Al { get; set; }
    }

    /// <summary>Menu outputs compared at the end (option fields, choice, players, controls, field origin).</summary>
    private static readonly (int Offset, int Length, string Name)[] Outputs =
    [
        (0x083C, 8, "controls/players"), (0x08E4, 0x1F, "options 08E4…0902"), (0x10A7, 1, "sound"), (0x2E8E, 2, "IQ"),
        (0x2F30, 1, "2F30"), (0x411A, 1, "411A"), (0x45BA, 1, "45BA"), (0x4BF2, 1, "4BF2"), (0x3CCC, 1, "choice 3CCC"),
        (0x4581, 1, "all computers"), (0x5330, 4, "columns/rows"), (0x451E, 2, "origin x"), (0x4528, 2, "origin y"),
    ];

    /// <summary>The busy key loops of 192D (compare and back edge of every wait loop, the dialog loop of 12A6) and 21FD:0245.</summary>
    private static Func<ushort, ushort, bool> PollLoops(OriginalProgram p)
    {
        ushort s192D = p.Segment(Seg192D), s21FD = p.Segment(Seg21FD);
        var ranges = MenuHarness.WaitEdges192D.Select(e => (From: e.Off - 5, To: e.Off + 2)).Append((From: 0x12DF, To: 0x130B)).ToArray();
        return (cs, ip) => (cs == s192D && ranges.Any(r => ip >= r.From && ip < r.To)) || (cs == s21FD && ip >= 0x0251 && ip < 0x026A);
    }

    /// <summary>
    /// Code of the presenting routines between their retrace waits (21FD:0185…05DA without the pause 0245, palettes 2295,
    /// pages 0B67): none of it reads the key table before its next retrace wait (21FD:051F reads DS:3D91/3D95 only after
    /// the first), so a key change there counts like one during the next retrace wait.
    /// </summary>
    private static Func<ushort, ushort, bool> Presenting(OriginalProgram p)
    {
        ushort s21FD = p.Segment(Seg21FD), s2295 = p.Segment(0x2295), s0B67 = p.Segment(0x0B67);
        return (cs, ip) => (cs == s21FD && ip >= 0x0185 && ip < 0x05DB && !(ip >= 0x0245 && ip < 0x026C)) || cs == s2295 || cs == s0B67;
    }

    private static OriginalRun RunOriginal(Script script)
    {
        var p = OriginalProgram.Resume(TestGameData.DosDir, MenuSceneHarness.AtMenu.Value);
        var m = p.Machine;
        ushort retIp = m.Mem.Read16(m.Cpu.Ss, m.Cpu.Sp), retCs = m.Mem.Read16(m.Cpu.Ss, (ushort)(m.Cpu.Sp + 2));
        foreach (var (at, v) in script.Set ?? []) p.WriteByte((ushort)at, v);
        var isPoll = PollLoops(p);
        var isPresenting = Presenting(p);

        // key events at a fixed phase of the 70 Hz frame, frames counted from the next frame start
        long period = m.Vga.FramePeriod;
        long frame0 = m.FrameNumber + 1;
        double Time(long frame) => (double)((frame0 + frame) * period + (long)(KeyPhase * period)) / Traders.RefEmu.Hardware.Pit.ClockHz;
        foreach (var press in script.Presses)
        {
            m.ScheduleScancode(Time(press.From), press.Key);
            m.ScheduleScancode(Time(press.From + press.Frames), (byte)(press.Key | 0x80));
        }

        var run = new OriginalRun();
        bool inWait = false;
        int randomCalls = 0;
        ChangeKind change = ChangeKind.Elsewhere;
        string where = "";
        byte[] Keys() => m.Mem.ReadBlock(p.Dgroup, DgroupLayout.KeyState, KeyTable.Length);

        m.AddCodeHook(p.Segment(Seg21FD), 0x01BA, _ => randomCalls++);
        m.AddCodeHook(p.Segment(SegRetrace), 0x00D3, mm =>
        {
            inWait = true;
            var vga = mm.Vga;
            run.Retraces.Add(new Frame(MenuSceneHarness.Hash(vga.Planes, vga.Dac, vga.StartAddress), vga.StartAddress,
                p.ReadDword(DgroupLayout.RandomSeed), randomCalls));
            run.RealFrames.Add(mm.FrameNumber - frame0);
        });
        m.AddCodeHook(p.Segment(SegRetrace), 0x00F7, _ =>
        {
            inWait = false;
            run.KeysAfter.Add(Keys());
        });
        // keyboard interrupt 27C6:0043: where it interrupted; at its iret (008E) the key table is updated
        m.AddCodeHook(p.Segment(SegKeyboard), 0x0043, mm =>
        {
            ushort ip = mm.Mem.Read16(mm.Cpu.Ss, mm.Cpu.Sp), cs = mm.Mem.Read16(mm.Cpu.Ss, (ushort)(mm.Cpu.Sp + 2));
            change = inWait ? ChangeKind.InRetraceWait : isPoll(cs, ip) ? ChangeKind.InPoll
                : isPresenting(cs, ip) ? ChangeKind.InPresenting : ChangeKind.Elsewhere;
            where = $"{cs - p.Segment(0):X4}:{ip:X4}";
        });
        m.AddCodeHook(p.Segment(SegKeyboard), 0x008E, mm =>
        {
            int gap = run.KeysAfter.Count - 1;
            run.Changes.Add($"{change} {where} gap {gap} real {mm.FrameNumber - frame0} sc {p.ReadByte(0x1158):X2}");
            switch (change)
            {
                case ChangeKind.InPoll:
                    if (!run.PollStates.TryGetValue(gap, out var list)) run.PollStates[gap] = list = [];
                    list.Add(Keys());
                    break;
                case ChangeKind.Elsewhere:
                    run.Ambiguous.Add($"key change at {where} after retrace {gap} (real frame {mm.FrameNumber - frame0})");
                    break;
            }
        });

        var loads = new Queue<bool>(script.Loads);
        var saves = new Queue<bool>(script.Saves);
        var calibrations = new Queue<bool>(script.Calibrations);
        void Stub(ushort seg, ushort off, Queue<bool> results) => m.AddCodeHook(p.Segment(seg), off, mm =>
        {
            var c = mm.Cpu;
            ushort ip = c.Pop(), cs = c.Pop();
            c.Ax = (ushort)(results.Count > 0 && results.Dequeue() ? 1 : 0);
            c.IP = ip;
            c.Cs = cs;
        });
        Stub(0x2336, 0x0539, loads);
        Stub(0x2336, 0x000A, saves);
        Stub(0x27C6, 0x00E6, calibrations);

        long last = script.Presses.Max(x => x.From + x.Frames);
        m.RunFor(last / 70.0 + 30, mm => mm.Cpu.Cs == retCs && mm.Cpu.IP == retIp);
        Assert.True(m.Cpu.Cs == retCs && m.Cpu.IP == retIp,
            $"192D:0007 did not return: at {m.Cpu.Cs - p.Segment(0):X4}:{m.Cpu.IP:X4}, retraces {run.Retraces.Count}; key changes: {string.Join("; ", run.Changes)}");
        Assert.True(loads.Count == 0 && saves.Count == 0 && calibrations.Count == 0, "original: scripted results left over");
        run.DataSegment = p.DataSegment();
        run.Al = m.Cpu.Ax & 0xFF;
        return run;
    }

    /// <summary>Retrace wait of the port's layers: counts and notes the port frame of every wait.</summary>
    private sealed class FrameRetraces : IRetraceWait
    {
        public long CurrentFrame { get; set; }

        public List<long> Frames { get; } = [];

        public void WaitForRetrace() => Frames.Add(CurrentFrame);
    }

    private static IReadOnlyCollection<byte> Held(byte[] table)
    {
        var held = new List<byte>();
        for (int sc = 0; sc < table.Length; sc++)
            if (table[sc] != 0) held.Add((byte)sc);
        return held;
    }

    /// <summary>
    /// Runs both sides and compares them retrace by retrace. <paramref name="doubleFrame"/>: counter-test – the port
    /// processes the frame with this number twice (two core frames for one host frame).
    /// </summary>
    public static void AssertSame(Script script, string label, long doubleFrame = -1)
    {
        var original = RunOriginal(script);
        Assert.True(original.Ambiguous.Count == 0, $"{label}: script not unambiguous: {string.Join("; ", original.Ambiguous)}");

        var retraces = new FrameRetraces();
        var (core, scene, driver, dac) = MenuSceneHarness.SetUpPort(script.Set, retraces);
        var host = new ScriptedMenuHost();
        foreach (bool b in script.Loads) host.LoadResults.Enqueue(b);
        foreach (bool b in script.Saves) host.SaveResults.Enqueue(b);
        foreach (bool b in script.Calibrations) host.CalibrationFailures.Enqueue(b);
        var random = new OriginalRandom(new RecordedClock([]), core.U32(DgroupLayout.RandomSeed));
        var runner = new MenuRunner(core, random, host);

        int k = 0, gap = -1, poll = 0, polls = 0;
        var empty = new byte[KeyTable.Length];
        void Check(long f)
        {
            if (runner.Finished || runner.Waits[^1] == MenuWait.Poll)
            {
                Assert.True(retraces.Frames.Count == k, $"{label}: port frame {f} (poll) made a retrace wait");
                return;
            }
            Assert.True(k < original.Retraces.Count, $"{label}: port retrace {k} in frame {f}, the original has only {original.Retraces.Count}");
            Assert.True(retraces.Frames.Count == k + 1 && retraces.Frames[k] == f,
                $"{label}: core waits for retrace {k} in frame {f}, the scene made {retraces.Frames.Count} waits (last in frame {(retraces.Frames.Count > 0 ? retraces.Frames[^1] : -1)})");
            var port = new Frame(MenuSceneHarness.Hash(driver.Vram.Planes, dac.Registers, driver.DisplayStart), driver.DisplayStart,
                core.U32(DgroupLayout.RandomSeed), runner.Menu.RandomCalls.Count);
            var o = original.Retraces[k];
            Assert.True(o == port,
                $"{label}: retrace {k} (port frame {f}, original real frame {original.RealFrames[k]}) differs: original {o}, port {port}");
            k++;
        }

        IReadOnlyCollection<byte> KeysFor()
        {
            if (runner.Waits[^1] != MenuWait.Poll)
            {
                gap++;
                poll = 0;
                return Held(gap < original.KeysAfter.Count ? original.KeysAfter[gap] : empty);
            }
            polls++;
            var states = original.PollStates.GetValueOrDefault(gap);
            if (states is { Count: > 0 }) return Held(states[Math.Min(poll++, states.Count - 1)]);
            return Held(gap >= 0 ? original.KeysAfter[gap] : empty);
        }

        retraces.CurrentFrame = 0;
        runner.Start(0, []);
        foreach (var e in runner.Menu.DrainEvents()) scene.Apply(e);
        Check(0);
        for (long f = 1; !runner.Finished; f++)
        {
            if (f > 400_000) throw new InvalidOperationException($"{label}: port menu did not finish.");
            retraces.CurrentFrame = f;
            var keys = KeysFor();
            runner.Advance(new PhaseTick(f, keys));
            if (f == doubleFrame && !runner.Finished) runner.Advance(new PhaseTick(++f, keys));
            scene.NextFrame();
            foreach (var e in runner.Menu.DrainEvents()) scene.Apply(e);
            Check(f);
        }
        scene.Finish();
        Assert.True(k == original.Retraces.Count, $"{label}: retraces original {original.Retraces.Count}, port {k}");
        Assert.True(retraces.Frames.Count == k, $"{label}: scene waits {retraces.Frames.Count} after the end, retraces {k}");
        Assert.Equal(original.Al, runner.Menu.Result ?? -1);
        foreach (var (at, n, name) in Outputs)
            for (int i = 0; i < n; i++)
                Assert.True(original.DataSegment[at + i] == core.U8(at + i),
                    $"{label}: {name} DS:{at + i:X4} original {original.DataSegment[at + i]:X2} port {core.U8(at + i):X2}");
        Assert.True(host.LoadResults.Count == 0 && host.SaveResults.Count == 0 && host.CalibrationFailures.Count == 0,
            $"{label}: port: scripted results left over");
        long realFrames = original.RealFrames.Count > 0 ? original.RealFrames[^1] : 0;
        Console.WriteLine($"{label}: {k} retraces equal, {original.Retraces.Select(r => r.Seed).Distinct().Count()} random states, " +
            $"{runner.Menu.RandomCalls.Count} random calls, port frames {runner.LastFrame} ({polls} poll frames), original real frames {realFrames}");
    }
}
