using Traders.Assets;
using Traders.Core;
using Traders.Core.Events;
using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.State;
using Traders.Presentation.Events;
using Traders.RefEmu;
using Traders.RefEmu.Cpu;
using Traders.RefEmu.Machine;
using Traders.Tests.Menus;
using Traders.Vga;
using Traders.Vga.Layers;

namespace Traders.Tests.Presentation;

/// <summary>
/// The week incidents 17CB:0005 with all graphics live: the original started as a new game (four computer players) and run to
/// the entry of the incidents of a week (snapshot), optionally with data segment values set (both sides), and the port's
/// <see cref="WeekIncidents"/> with <see cref="IncidentScene"/> on the graphics state of that snapshot. Keys change at the frame
/// boundaries (every retrace wait and every taken back edge of the pause loop 21FD:0245), as in
/// <see cref="WeekReportSceneHarness"/>. At the entry of every routine that hands the picture to the display or the DAC – 21FD:037D
/// (fade-out), 0329 (slow fade-in), 051F (page flip) – and at the end, VRAM (all four planes), DAC, CRTC start and the retraces
/// so far are captured together with the frame; the port captures where its scene enters the same routine.
/// </summary>
internal static class EventsSceneHarness
{
    private const ushort Seg21FD = 0x21FD, Driver = 0x23CD;

    private static readonly (ushort Entry, string Name)[] Presenting = [(0x037D, "fade-out"), (0x0329, "slow fade-in"), (0x051F, "flip")];

    private static readonly Lazy<OriginalProgram> Game = new(() => OriginalProgram.StartGame(TestGameData.DosDir));

    private static readonly Dictionary<int, MachineSnapshot> Snapshots = [];

    /// <summary>The machine at the entry of the incidents of <paramref name="week"/>.</summary>
    internal static MachineSnapshot AtIncidents(int week)
    {
        lock (Snapshots)
        {
            if (Snapshots.TryGetValue(week, out var s)) return s;
            var p = Game.Value;
            while (!Snapshots.ContainsKey(week))
            {
                Assert.True(p.RunToRoutine(0x17CB, 0x0005, 1800), $"incidents of week {week} not reached (at week {p.Week})");
                Snapshots[p.Week] = p.Snapshot();
            }
            return Snapshots[week];
        }
    }

    /// <summary>Week of the snapshot, keys per frame and a change of the snapshot's data segment (both sides).</summary>
    public sealed record Script(int Week, KeyScript Keys, Action<Dgroup>? Change = null);

    /// <summary>A capture with the frame it was taken in.</summary>
    public sealed record Capture(long Frame, MenuSceneHarness.Capture Picture);

    public sealed class Result
    {
        public required List<Capture> Captures { get; init; }
        public required byte[] DataSegment { get; init; }
        public required byte[] Texts { get; init; }
        public List<IncidentEvent> Events { get; init; } = [];
    }

    /// <summary>The snapshot's data segment with the script's change applied.</summary>
    private static byte[] InputDs(OriginalProgram snap, Script script)
    {
        var ds = snap.DataSegment();
        if (script.Change is { } change)
        {
            var d = new Dgroup(ds[..DgroupLayout.Size]);
            change(d);
            d.Bytes.CopyTo(ds, 0);
        }
        return ds;
    }

    private static (ushort Off, ushort Seg) TextBuffer(byte[] ds) =>
        ((ushort)(ds[MenuLayout.TextBuffer] | ds[MenuLayout.TextBuffer + 1] << 8),
            (ushort)(ds[MenuLayout.TextBuffer + 2] | ds[MenuLayout.TextBuffer + 3] << 8));

    private static byte[] ReadTexts(OriginalProgram p, byte[] ds)
    {
        var (off, seg) = TextBuffer(ds);
        var b = new byte[MenuTexts.ScanLimit + 1];
        for (int i = 0; i < b.Length; i++) b[i] = p.Machine.Mem.Read8(seg, (ushort)(off + i));
        return b;
    }

    // ------------------------------------------------------------------ original

    public static Result RunOriginal(Script script, int keep, out byte[][]? kept)
    {
        var p = OriginalProgram.Resume(TestGameData.DosDir, AtIncidents(script.Week));
        var m = p.Machine;
        ushort retIp = m.Mem.Read16(m.Cpu.Ss, m.Cpu.Sp), retCs = m.Mem.Read16(m.Cpu.Ss, (ushort)(m.Cpu.Sp + 2));
        var ds = InputDs(p, script);
        for (int i = 0; i < DgroupLayout.Size; i++) p.WriteByte((ushort)i, ds[i]);
        long frame = 0;
        int retraces = 0;
        m.AddCodeHook(p.Segment(0x0D73), 0x00D3, _ => retraces++);
        var captures = new List<Capture>();
        byte[][]? keptPlanes = null;
        void WriteKeys(IReadOnlyCollection<byte> held)
        {
            for (int sc = 0; sc < KeyTable.Length; sc++) p.WriteByte((ushort)(DgroupLayout.KeyState + sc), 0);
            foreach (byte sc in held) p.WriteByte((ushort)(DgroupLayout.KeyState + sc), 1);
        }
        void NextFrame()
        {
            frame++;
            WriteKeys(script.Keys.At(frame));
        }
        void Take(string kind)
        {
            var vga = m.Vga;
            if (captures.Count == keep) keptPlanes = vga.Planes.Select(x => (byte[])x.Clone()).ToArray();
            captures.Add(new(frame, new(kind, MenuSceneHarness.Hash(vga.Planes, vga.Dac, vga.StartAddress), vga.StartAddress, retraces)));
        }
        foreach (var (entry, name) in Presenting) m.AddCodeHook(p.Segment(Seg21FD), entry, _ => Take(name));
        m.AddCodeHook(p.Segment(0x0D73), 0x00F7, _ => NextFrame());
        foreach (var (off, onZero) in MenuHarness.WaitEdges21FD)
            m.AddCodeHook(p.Segment(Seg21FD), off, mm => { if (mm.Cpu.GetFlag(Cpu286.ZF) == onZero) NextFrame(); });

        WriteKeys(script.Keys.At(0));
        m.RunFor(300, mm => mm.Cpu.Cs == retCs && mm.Cpu.IP == retIp);
        Assert.True(m.Cpu.Cs == retCs && m.Cpu.IP == retIp,
            $"17CB:0005 did not return: frame {frame}, at {m.Cpu.Cs - p.Segment(0):X4}:{m.Cpu.IP:X4}, captures {captures.Count}");
        Take("end");
        kept = keptPlanes;
        var outDs = p.DataSegment();
        return new Result { Captures = captures, DataSegment = outDs, Texts = ReadTexts(p, outDs) };
    }

    // ------------------------------------------------------------------ port

    /// <summary>
    /// The port on the snapshot's state: core data segment, graphics data segment image, VRAM, DAC and driver registers as the
    /// original left them at 17CB:0005; fonts and masks from FONTS.PP and MASKS.PP, the text buffer as the original holds it.
    /// </summary>
    public static Result RunPort(Script script, int keep, out byte[][]? kept)
    {
        var retraces = new RetraceCounter();
        var snap = OriginalProgram.Resume(TestGameData.DosDir, AtIncidents(script.Week));
        var ds = InputDs(snap, script);
        var core = new Dgroup(ds[..DgroupLayout.Size]);
        var textBuffer = ReadTexts(snap, ds);
        var texts = new TextBufferMemory(core, () => textBuffer);

        var driver = new ModeXDriver();
        var vga = snap.Machine.Vga;
        Field.FieldSceneHarness.SyncDriver(snap, driver); // VRAM, registers, atlas (the field picture) and masks
        driver.Font = snap.Machine.Mem.Read16(snap.Segment(Driver), 0x0586);
        var fontData = PowerPacker.Decrunch(File.ReadAllBytes(TestGameData.Dos(ResourceTable.Fonts)));
        driver.Fonts =
        [
            new FontDescriptor(fontData, 0x5A0, 5, 6, 0x20), new FontDescriptor(fontData, 0, 12, 14, 0x20),
            new FontDescriptor(fontData, 0x7F8, 5, 5, 0x2F),
        ];
        var dac = new VgaDac();
        vga.Dac.CopyTo(dac.Registers, 0);
        var layers = new GraphicsLayers(driver, (byte[])ds.Clone(), dac, retraces) { DataSegment = snap.Dgroup };
        var scene = new IncidentScene(layers, core, texts);

        var captures = new List<Capture>();
        byte[][]? keptPlanes = null;
        long frame = 0;
        void Take(string kind)
        {
            int start = driver.DisplayStart;
            if (captures.Count == keep) keptPlanes = driver.Vram.Planes.Select(x => (byte[])x.Clone()).ToArray();
            captures.Add(new(frame, new(kind, MenuSceneHarness.Hash(driver.Vram.Planes, dac.Registers, start), start, retraces.Count)));
        }
        scene.BeforePresent = e => Take(e.Kind switch
        {
            IncidentEventKind.FadeOut => "fade-out",
            IncidentEventKind.FadeIn => "slow fade-in",
            _ => "flip",
        });

        var clock = new RecordedClock(OriginalClock(script));
        var random = new OriginalRandom(clock, core.U32(DgroupLayout.RandomSeed));
        var incidents = new WeekIncidents(core, random, KeyboardOnlyDevices.Instance, texts);
        var events = new List<IncidentEvent>();
        using var run = incidents.Run().GetEnumerator();
        for (frame = 0; ; frame++)
        {
            if (frame > 20_000) throw new InvalidOperationException("Port incidents did not finish.");
            KeyTable.Write(core, script.Keys.At(frame));
            scene.NextFrame();
            incidents.Frame = frame;
            random.Seed = core.U32(DgroupLayout.RandomSeed);
            bool more = run.MoveNext();
            core.Set(DgroupLayout.RandomSeed, random.Seed);
            foreach (var e in incidents.DrainEvents())
            {
                events.Add(e);
                scene.Apply(e);
            }
            if (!more) break;
        }
        scene.Finish();
        Take("end");
        kept = keptPlanes;
        return new Result { Captures = captures, DataSegment = core.Bytes, Texts = textBuffer, Events = events };
    }

    private static readonly Dictionary<Script, List<int>> Clocks = [];

    /// <summary>The values time() returned in the original's run of the script (for random(0)).</summary>
    private static List<int> OriginalClock(Script script)
    {
        lock (Clocks)
        {
            if (Clocks.TryGetValue(script, out var c)) return c;
            var p = OriginalProgram.Resume(TestGameData.DosDir, AtIncidents(script.Week));
            var m = p.Machine;
            ushort retIp = m.Mem.Read16(m.Cpu.Ss, m.Cpu.Sp), retCs = m.Mem.Read16(m.Cpu.Ss, (ushort)(m.Cpu.Sp + 2));
            var ds = InputDs(p, script);
            for (int i = 0; i < DgroupLayout.Size; i++) p.WriteByte((ushort)i, ds[i]);
            var clock = new List<int>();
            m.AddCodeHook(p.Segment(Seg21FD), 0x01CE, mm => clock.Add(mm.Cpu.Ax | (mm.Cpu.Dx << 16)));
            long frame = 0;
            void Keys()
            {
                for (int sc = 0; sc < KeyTable.Length; sc++) p.WriteByte((ushort)(DgroupLayout.KeyState + sc), 0);
                foreach (byte sc in script.Keys.At(frame)) p.WriteByte((ushort)(DgroupLayout.KeyState + sc), 1);
            }
            m.AddCodeHook(p.Segment(0x0D73), 0x00F7, _ => { frame++; Keys(); });
            foreach (var (off, onZero) in MenuHarness.WaitEdges21FD)
                m.AddCodeHook(p.Segment(Seg21FD), off, mm => { if (mm.Cpu.GetFlag(Cpu286.ZF) == onZero) { frame++; Keys(); } });
            Keys();
            m.RunFor(300, mm => mm.Cpu.Cs == retCs && mm.Cpu.IP == retIp);
            return Clocks[script] = clock;
        }
    }

    /// <summary>
    /// Compares the capture sequences (picture, start, retraces and frame) and the incidents' own state; on the first
    /// difference names its index, kind and first differing byte. Returns the port's events.
    /// </summary>
    public static List<IncidentEvent> AssertSame(Script script, string label)
    {
        var original = RunOriginal(script, -1, out _);
        var port = RunPort(script, -1, out _);
        int n = Math.Min(original.Captures.Count, port.Captures.Count);
        for (int i = 0; i < n; i++)
        {
            var (o, q) = (original.Captures[i], port.Captures[i]);
            if (o == q) continue;
            RunOriginal(script, i, out var op);
            RunPort(script, i, out var pp);
            string where = "planes equal (DAC, start, retraces or frame differ)";
            for (int pl = 0; pl < 4 && op is not null && pp is not null; pl++)
            {
                int k = 0;
                while (k < op[pl].Length && op[pl][k] == pp[pl][k]) k++;
                if (k < op[pl].Length)
                {
                    where = $"plane {pl} offset {k:X4} (page {k / 0x4000}, x {(k % 80) * 4 + pl}, y {k % 0x4000 / 80}) " +
                            $"original {op[pl][k]:X2} port {pp[pl][k]:X2}";
                    break;
                }
            }
            Assert.Fail($"{label}: capture {i} ({o.Picture.Kind}) differs: original frame {o.Frame} start {o.Picture.DisplayStart:X4} " +
                        $"retraces {o.Picture.Retraces}, port {q.Picture.Kind} frame {q.Frame} start {q.Picture.DisplayStart:X4} " +
                        $"retraces {q.Picture.Retraces}; {where}");
        }
        Assert.True(original.Captures.Count == port.Captures.Count,
            $"{label}: captures original {original.Captures.Count}, port {port.Captures.Count}");
        Assert.True(original.Texts.AsSpan().SequenceEqual(port.Texts), $"{label}: text buffer differs");
        foreach (var (at, length) in new[] { (DgroupLayout.Cash, 16), (DgroupLayout.RandomSeed, 4), (IncidentLayout.TypewriterChar, 1) })
            for (int i = 0; i < length; i++)
                Assert.True(original.DataSegment[at + i] == port.DataSegment[at + i], $"{label}: DS:{at + i:X4} differs");
        Console.WriteLine($"{label}: {original.Captures.Count} captures, " +
                          $"{original.Captures.Select(c => c.Picture.Hash).Distinct().Count()} distinct");
        return port.Events;
    }
}
