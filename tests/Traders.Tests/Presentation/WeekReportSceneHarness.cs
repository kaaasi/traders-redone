using Traders.Assets;
using Traders.Core;
using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.State;
using Traders.Core.Week;
using Traders.Presentation.Menus;
using Traders.Presentation.Week;
using Traders.RefEmu;
using Traders.RefEmu.Cpu;
using Traders.RefEmu.Machine;
using Traders.Tests.Menus;
using Traders.Vga;
using Traders.Vga.Layers;

namespace Traders.Tests.Presentation;

/// <summary>
/// The week report 0825:000A(1) with all graphics live: the original started as a new game (four computer players) and run to
/// the entry of the report of a week (snapshot), and the port's <see cref="Report"/> with <see cref="ReportScene"/> on the
/// graphics state of that snapshot. Keys change at the frame boundaries (every retrace wait, end of 0D73:00D3, and every taken
/// back edge of the pause loop 21FD:0245), as in <see cref="MenuSceneHarness"/>. At the entry of every routine that hands the
/// picture to the display or the DAC – 21FD:0590 (draw on the shown page), 03EB (palette step of the wipe), 051F (page flip)
/// and 0329 (slow fade-in) – and at the end, VRAM (all four planes), DAC, CRTC start and the retraces so far are captured;
/// the port captures where its scene enters the same routine. The sequences must agree.
/// </summary>
internal static class WeekReportSceneHarness
{
    private const ushort Seg21FD = 0x21FD, Driver = 0x23CD;

    private static readonly (ushort Entry, string Name)[] Presenting =
        [(0x0590, "shown page"), (0x03EB, "palette step"), (0x051F, "flip"), (0x0329, "slow fade-in")];

    private static readonly Lazy<OriginalProgram> Game = new(() => OriginalProgram.StartGame(TestGameData.DosDir));

    private static readonly Dictionary<int, MachineSnapshot> Snapshots = [];

    /// <summary>The machine at the entry of the report of <paramref name="week"/>.</summary>
    internal static MachineSnapshot AtReport(int week)
    {
        lock (Snapshots)
        {
            if (Snapshots.TryGetValue(week, out var s)) return s;
            var p = Game.Value;
            // every report passed on the way is kept, so the weeks can be requested in any order
            while (!Snapshots.ContainsKey(week))
            {
                Assert.True(p.RunToRoutine(0x0825, 0x000A, 1800), $"report of week {week} not reached (at week {p.Week})");
                Assert.Equal(1, p.StackArgument(0));
                Snapshots[p.Week] = p.Snapshot();
            }
            return Snapshots[week];
        }
    }

    /// <summary>Week of the snapshot, keys per frame, data segment bytes set before the start (both sides).</summary>
    public sealed record Script(int Week, KeyScript Keys, (int Offset, byte Value)[]? Set = null);

    public sealed class Result
    {
        public required List<MenuSceneHarness.Capture> Captures { get; init; }
        public required byte[] DataSegment { get; init; }
    }

    // ------------------------------------------------------------------ original

    public static Result RunOriginal(Script script, int keep, out byte[][]? kept)
    {
        var p = OriginalProgram.Resume(TestGameData.DosDir, AtReport(script.Week));
        var m = p.Machine;
        ushort retIp = m.Mem.Read16(m.Cpu.Ss, m.Cpu.Sp), retCs = m.Mem.Read16(m.Cpu.Ss, (ushort)(m.Cpu.Sp + 2));
        long frame = 0;
        int retraces = 0;
        m.AddCodeHook(p.Segment(0x0D73), 0x00D3, _ => retraces++);
        foreach (var (at, v) in script.Set ?? []) p.WriteByte((ushort)at, v);
        var captures = new List<MenuSceneHarness.Capture>();
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
            captures.Add(new(kind, MenuSceneHarness.Hash(vga.Planes, vga.Dac, vga.StartAddress), vga.StartAddress, retraces));
        }
        // 03EB also runs inside the fade-in 0329; only the wipe's own palette steps (called from 0825:09C9) are captured
        foreach (var (entry, name) in Presenting)
            m.AddCodeHook(p.Segment(Seg21FD), entry, mm =>
            {
                if (entry != 0x03EB || mm.Mem.Read16(mm.Cpu.Ss, (ushort)(mm.Cpu.Sp + 2)) == p.Segment(0x0825)) Take(name);
            });
        m.AddCodeHook(p.Segment(0x0D73), 0x00F7, _ => NextFrame());
        foreach (var (off, onZero) in MenuHarness.WaitEdges21FD)
            m.AddCodeHook(p.Segment(Seg21FD), off, mm => { if (mm.Cpu.GetFlag(Cpu286.ZF) == onZero) NextFrame(); });

        WriteKeys(script.Keys.At(0));
        m.RunFor(120, mm => mm.Cpu.Cs == retCs && mm.Cpu.IP == retIp);
        Assert.True(m.Cpu.Cs == retCs && m.Cpu.IP == retIp,
            $"0825:000A did not return: frame {frame}, at {m.Cpu.Cs - p.Segment(0):X4}:{m.Cpu.IP:X4}, captures {captures.Count}");
        Take("end");
        kept = keptPlanes;
        return new Result { Captures = captures, DataSegment = p.DataSegment() };
    }

    // ------------------------------------------------------------------ port

    private sealed class FarBuffer(ushort segment, ushort offset, byte[] data) : IFarMemory
    {
        private int Index(ushort seg, ushort off)
        {
            int i = (seg - segment) * 16 + off - offset;
            if (i < 0 || i >= data.Length) throw new InvalidOperationException($"Far read {seg:X4}:{off:X4} outside the text buffer.");
            return i;
        }

        public byte Read(ushort seg, ushort off) => data[Index(seg, off)];

        public void Write(ushort seg, ushort off, byte value) => data[Index(seg, off)] = value;
    }

    private sealed class Pictures : IPictureSource
    {
        public byte[] Decrunched(int resource) =>
            PowerPacker.Decrunch(File.ReadAllBytes(TestGameData.Dos(ResourceTable.Pictures[resource].Vga)));
    }

    /// <summary>
    /// The port on the snapshot's state: core data segment, graphics data segment image, VRAM, DAC and driver registers as the
    /// original left them at 0825:000A; fonts and masks from FONTS.PP and MASKS.PP, the text buffer from ENGLISCH.TXT (as in
    /// <see cref="MenuSceneHarness.SetUpPort"/>).
    /// </summary>
    private static (Dgroup Core, ReportScene Scene, ModeXDriver Driver, VgaDac Dac) SetUpPort(Script script, IRetraceWait retraces)
    {
        var snap = OriginalProgram.Resume(TestGameData.DosDir, AtReport(script.Week));
        var ds = snap.DataSegment();
        foreach (var (at, v) in script.Set ?? []) ds[at] = v;
        var core = new Dgroup(ds[..DgroupLayout.Size]);

        var driver = new ModeXDriver();
        var vga = snap.Machine.Vga;
        for (int i = 0; i < 4; i++) vga.Planes[i].CopyTo(driver.Vram.Planes[i], 0);
        ushort DriverWord(ushort off) => snap.Machine.Mem.Read16(snap.Segment(Driver), off);
        driver.SetDisplayStart((ushort)vga.StartAddress, 0xA000);
        driver.SetDrawPage(DriverWord(0x0000), DriverWord(0x0002));
        driver.Font = DriverWord(0x0586);
        driver.Color = DriverWord(0x0588);
        driver.LinePattern = DriverWord(0x04F4);
        driver.Vram.MapMask = (byte)(vga.Seq[2] & 0x0F);
        driver.Vram.ReadMap = (byte)(vga.Gc[4] & 0x03);
        var fontData = PowerPacker.Decrunch(File.ReadAllBytes(TestGameData.Dos(ResourceTable.Fonts)));
        driver.Fonts =
        [
            new FontDescriptor(fontData, 0x5A0, 5, 6, 0x20), new FontDescriptor(fontData, 0, 12, 14, 0x20),
            new FontDescriptor(fontData, 0x7F8, 5, 5, 0x2F),
        ];
        driver.Masks = PowerPacker.Decrunch(File.ReadAllBytes(TestGameData.Dos(ResourceTable.Masks)));

        var dac = new VgaDac();
        vga.Dac.CopyTo(dac.Registers, 0);
        var layers = new GraphicsLayers(driver, (byte[])ds.Clone(), dac, retraces) { DataSegment = snap.Dgroup };

        ushort bufOff = core.U16(MenuLayout.TextBuffer), bufSeg = core.U16(MenuLayout.TextBuffer + 2);
        var text = new byte[0x2000];
        File.ReadAllBytes(TestGameData.Dos(ResourceTable.EnglishText)).CopyTo(text, 0);
        MenuTexts.Build(new Dgroup((byte[])core.Bytes.Clone()), text);
        layers.OtherMemory = new FarBuffer(bufSeg, bufOff, text);
        return (core, new ReportScene(layers, core, layers.OtherMemory, new Pictures()), driver, dac);
    }

    /// <summary>Port run: the core's report frame by frame, its events played by the scene after every frame.</summary>
    public static Result RunPort(Script script, int keep, out byte[][]? kept)
    {
        var retraces = new RetraceCounter();
        var (core, scene, driver, dac) = SetUpPort(script, retraces);
        var captures = new List<MenuSceneHarness.Capture>();
        byte[][]? keptPlanes = null;
        void Take(string kind)
        {
            int start = driver.DisplayStart;
            if (captures.Count == keep) keptPlanes = driver.Vram.Planes.Select(x => (byte[])x.Clone()).ToArray();
            captures.Add(new(kind, MenuSceneHarness.Hash(driver.Vram.Planes, dac.Registers, start), start, retraces.Count));
        }
        scene.BeforePresent = e => Take(e.Kind switch
        {
            WeekEventKind.DrawOnShownPage => "shown page",
            WeekEventKind.PaletteStep => "palette step",
            WeekEventKind.Flip => "flip",
            WeekEventKind.FadeIn => "slow fade-in",
            _ => e.Kind.ToString(),
        });

        var random = new OriginalRandom(new RecordedClock([]), core.U32(DgroupLayout.RandomSeed));
        var report = new Report(core, random, KeyboardOnlyDevices.Instance, 1);
        using var run = report.Run().GetEnumerator();
        for (long f = 0; ; f++)
        {
            if (f > 20_000) throw new InvalidOperationException("Port report did not finish.");
            KeyTable.Write(core, script.Keys.At(f));
            scene.NextFrame();
            report.Frame = f;
            random.Seed = core.U32(DgroupLayout.RandomSeed);
            bool more = run.MoveNext();
            core.Set(DgroupLayout.RandomSeed, random.Seed);
            foreach (var e in report.DrainEvents()) scene.Apply(e);
            if (!more) break;
        }
        scene.Finish();
        Take("end");
        kept = keptPlanes;
        return new Result { Captures = captures, DataSegment = core.Bytes };
    }

    /// <summary>Compares the capture sequences; on the first difference names its index, kind and first differing byte.</summary>
    public static void AssertSame(Script script, string label)
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
            string where = "planes equal (DAC or start differ)";
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
            Assert.Fail($"{label}: capture {i} ({o.Kind}) differs: original start {o.DisplayStart:X4} retraces {o.Retraces}, " +
                        $"port {q.Kind} start {q.DisplayStart:X4} retraces {q.Retraces}; {where}");
        }
        Assert.True(original.Captures.Count == port.Captures.Count,
            $"{label}: captures original {original.Captures.Count} ({string.Join(",", original.Captures.Select(c => c.Kind))}), " +
            $"port {port.Captures.Count} ({string.Join(",", port.Captures.Select(c => c.Kind))})");
        Assert.Contains(original.Captures, c => c.Kind == "slow fade-in");
        Console.WriteLine($"{label}: {original.Captures.Count} captures, {original.Captures.Select(c => c.Hash).Distinct().Count()} distinct");
    }
}
