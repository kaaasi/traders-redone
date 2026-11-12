using System.Runtime.InteropServices;
using Traders.Assets;
using Traders.Core;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.State;
using Traders.Presentation.Menus;
using Traders.RefEmu;
using Traders.RefEmu.Cpu;
using Traders.RefEmu.Machine;
using Traders.Tests.Menus;
using Traders.Vga;
using Traders.Vga.Layers;

namespace Traders.Tests.Presentation;

/// <summary>
/// The main menu 192D:0007 with all graphics live: the original booted to the entry of 192D:0007 (snapshot), and the port's
/// <see cref="MenuRunner"/> with <see cref="MenuScene"/> on the graphics state of that snapshot. Keys change at the frame
/// boundaries: every retrace wait (end of 0D73:00D3) and every taken back edge of a key wait loop; save file and joystick are stubbed with the
/// same scripted results on both sides. At the entry of every flip (21FD:051F), fade-out (03B4) and fade-in (0353, 0329)
/// and at the end, VRAM (all four planes), DAC and CRTC start are captured; the port captures before it plays the
/// corresponding event. The sequences of captures must agree.
/// </summary>
internal static class MenuSceneHarness
{
    private const ushort Seg192D = 0x192D, Seg21FD = 0x21FD, Driver = 0x23CD;

    /// <summary>Entries of the presenting routines of 21FD (captured before each).</summary>
    private static readonly (ushort Entry, string Name)[] Presenting =
        [(0x051F, "flip"), (0x03B4, "fade-out"), (0x0353, "fade-in"), (0x0329, "slow fade-in")];

    internal static readonly Lazy<MachineSnapshot> AtMenu = new(() =>
    {
        var p = OriginalProgram.Start(TestGameData.DosDir);
        Assert.True(p.RunToRoutine(Seg192D, 0x0007, 40), "192D:0007 not reached after the start.");
        return p.Snapshot();
    });

    /// <summary>A capture: VRAM/DAC/start hash and the retrace waits so far (entries into 0D73:00D3 resp. the port's counter).</summary>
    public sealed record Capture(string Kind, ulong Hash, int DisplayStart, int Retraces);

    /// <summary>Keys per frame, scripted save/load/calibration results, bytes set in the data segment before the start.</summary>
    public sealed record Script(KeyScript Keys, bool[] Loads, bool[] Saves, bool[] Calibrations, (int Offset, byte Value)[]? Set = null);

    public sealed class Result
    {
        public required List<Capture> Captures { get; init; }
        public required byte[] DataSegment { get; init; }
        public required int Al { get; init; }
    }

    /// <summary>Frame state as hashed: four planes, DAC, CRTC start.</summary>
    internal static ulong Hash(byte[][] planes, byte[] dac, int start)
    {
        ulong h = 1469598103934665603UL;
        void Add(ReadOnlySpan<byte> bytes)
        {
            foreach (ulong v in MemoryMarshal.Cast<byte, ulong>(bytes)) h = (h ^ v) * 1099511628211UL;
            foreach (byte b in bytes[(bytes.Length & ~7)..]) h = (h ^ b) * 1099511628211UL;
        }
        foreach (var p in planes) Add(p);
        Add(dac);
        Add([(byte)start, (byte)(start >> 8)]);
        return h;
    }

    // ------------------------------------------------------------------ original

    /// <param name="keep">Capture index whose planes are kept for diagnostics (−1 = none).</param>
    public static Result RunOriginal(Script script, int keep, out byte[][]? kept)
    {
        var p = OriginalProgram.Resume(TestGameData.DosDir, AtMenu.Value);
        var m = p.Machine;
        ushort retIp = m.Mem.Read16(m.Cpu.Ss, m.Cpu.Sp), retCs = m.Mem.Read16(m.Cpu.Ss, (ushort)(m.Cpu.Sp + 2));
        long frame = 0;
        int retraces = 0;
        m.AddCodeHook(p.Segment(0x0D73), 0x00D3, _ => retraces++);
        foreach (var (at, v) in script.Set ?? []) p.WriteByte((ushort)at, v);
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
            captures.Add(new Capture(kind, Hash(vga.Planes, vga.Dac, vga.StartAddress), vga.StartAddress, retraces));
        }
        foreach (var (entry, name) in Presenting) m.AddCodeHook(p.Segment(Seg21FD), entry, _ => Take(name));
        // every retrace wait (end of 0D73:00D3) is a frame boundary
        m.AddCodeHook(p.Segment(0x0D73), 0x00F7, _ => NextFrame());
        foreach (var (off, onZero) in MenuHarness.WaitEdges192D)
            m.AddCodeHook(p.Segment(Seg192D), off, mm => { if (mm.Cpu.GetFlag(Cpu286.ZF) == onZero) NextFrame(); });
        foreach (var (off, onZero) in MenuHarness.WaitEdges21FD)
            m.AddCodeHook(p.Segment(Seg21FD), off, mm => { if (mm.Cpu.GetFlag(Cpu286.ZF) == onZero) NextFrame(); });
        m.AddCodeHook(p.Segment(Seg192D), 0x1309, _ => NextFrame());

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

        WriteKeys(script.Keys.At(0));
        m.RunFor(60, mm => mm.Cpu.Cs == retCs && mm.Cpu.IP == retIp);
        Assert.True(m.Cpu.Cs == retCs && m.Cpu.IP == retIp,
            $"192D:0007 did not return: frame {frame}, at {m.Cpu.Cs - p.Segment(0):X4}:{m.Cpu.IP:X4}, captures {captures.Count}");
        Take("end");
        Assert.True(loads.Count == 0 && saves.Count == 0 && calibrations.Count == 0, "original: scripted results left over");
        kept = keptPlanes;
        return new Result { Captures = captures, DataSegment = p.DataSegment(), Al = m.Cpu.Ax & 0xFF };
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
    /// The port on the snapshot's state: core data segment, graphics data segment image, VRAM, DAC and driver registers as
    /// the original left them at 192D:0007; fonts and masks from FONTS.PP and MASKS.PP, the text buffer from ENGLISCH.TXT
    /// (192D:18B6 of the port at the original's buffer address).
    /// </summary>
    internal static (Dgroup Core, MenuScene Scene, ModeXDriver Driver, VgaDac Dac) SetUpPort((int Offset, byte Value)[]? set,
        IRetraceWait retraces)
    {
        var snap = OriginalProgram.Resume(TestGameData.DosDir, AtMenu.Value);
        var ds = snap.DataSegment();
        foreach (var (at, v) in set ?? []) ds[at] = v;
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
        // FONTS.PP as the start (0DFD) sets it up: small 5/6 at 5A0h, large 12/14 at 0, digits 5/5 from "/" at 7F8h
        driver.Fonts =
        [
            new FontDescriptor(fontData, 0x5A0, 5, 6, 0x20), new FontDescriptor(fontData, 0, 12, 14, 0x20),
            new FontDescriptor(fontData, 0x7F8, 5, 5, 0x2F),
        ];
        AssertFontTable(snap, DriverWord(0x057A), DriverWord(0x057C), driver.Fonts);
        driver.Masks = PowerPacker.Decrunch(File.ReadAllBytes(TestGameData.Dos(ResourceTable.Masks)));

        var dac = new VgaDac();
        vga.Dac.CopyTo(dac.Registers, 0);
        var layers = new GraphicsLayers(driver, (byte[])ds.Clone(), dac, retraces) { DataSegment = snap.Dgroup };

        ushort bufOff = core.U16(MenuLayout.TextBuffer), bufSeg = core.U16(MenuLayout.TextBuffer + 2);
        var text = new byte[0x2000];
        File.ReadAllBytes(TestGameData.Dos(ResourceTable.EnglishText)).CopyTo(text, 0);
        MenuTexts.Build(new Dgroup((byte[])core.Bytes.Clone()), text);
        layers.OtherMemory = new FarBuffer(bufSeg, bufOff, text);

        return (core, new MenuScene(layers, core, layers.OtherMemory, new Pictures()), driver, dac);
    }

    /// <summary>Port run of a script, keys per frame.</summary>
    public static Result RunPort(Script script, int keep, out byte[][]? kept, Action<MenuScene>? mutate = null)
    {
        var retraces = new RetraceCounter();
        var (core, scene, driver, dac) = SetUpPort(script.Set, retraces);
        mutate?.Invoke(scene);
        var captures = new List<Capture>();
        byte[][]? keptPlanes = null;
        void Take(string kind)
        {
            int start = driver.DisplayStart;
            if (captures.Count == keep) keptPlanes = driver.Vram.Planes.Select(x => (byte[])x.Clone()).ToArray();
            captures.Add(new Capture(kind, Hash(driver.Vram.Planes, dac.Registers, start), start, retraces.Count));
        }
        scene.BeforePresent = e => Take(e.Kind switch
        {
            MenuEventKind.Flip => "flip",
            MenuEventKind.FadeOut => "fade-out",
            MenuEventKind.FadeIn => e.A == 0x0329 ? "slow fade-in" : "fade-in",
            _ => e.Kind.ToString(),
        });

        var host = new ScriptedMenuHost();
        foreach (bool b in script.Loads) host.LoadResults.Enqueue(b);
        foreach (bool b in script.Saves) host.SaveResults.Enqueue(b);
        foreach (bool b in script.Calibrations) host.CalibrationFailures.Enqueue(b);
        var random = new OriginalRandom(new RecordedClock([]), core.U32(DgroupLayout.RandomSeed));
        var runner = new MenuRunner(core, random, host);
        runner.Start(0, script.Keys.At(0));
        foreach (var e in runner.Menu.DrainEvents()) scene.Apply(e);
        for (long f = 1; !runner.Finished; f++)
        {
            if (f > 200_000) throw new InvalidOperationException("Port menu did not finish.");
            runner.Advance(new PhaseTick(f, script.Keys.At(f)));
            scene.NextFrame();
            foreach (var e in runner.Menu.DrainEvents()) scene.Apply(e);
        }
        scene.Finish();
        Take("end");
        Assert.True(host.LoadResults.Count == 0 && host.SaveResults.Count == 0 && host.CalibrationFailures.Count == 0,
            "port: scripted results left over");
        kept = keptPlanes;
        return new Result { Captures = captures, DataSegment = core.Bytes, Al = runner.Menu.Result ?? -1 };
    }

    /// <summary>The original's font table (driver CS:057A) must describe the fonts the port uses.</summary>
    private static void AssertFontTable(OriginalProgram p, ushort off, ushort seg, IReadOnlyList<FontDescriptor> fonts)
    {
        for (int i = 0; i < fonts.Count; i++)
        {
            ushort Word(int k) => p.Machine.Mem.Read16(seg, (ushort)(off + i * 10 + k));
            var f = fonts[i];
            Assert.True(Word(4) == f.Height && Word(6) == f.Advance && Word(8) == f.FirstChar,
                $"font {i}: original {string.Join(" ", Enumerable.Range(0, 30).Select(k => Word(2 * k).ToString("X4")))}");
        }
    }

    /// <summary>Compares two capture sequences; on the first difference names its index, kind and first differing byte.</summary>
    public static void AssertSame(Script script, string label, Action<MenuScene>? mutate = null)
    {
        var original = RunOriginal(script, -1, out _);
        var port = RunPort(script, -1, out _, mutate);
        int n = Math.Min(original.Captures.Count, port.Captures.Count);
        for (int i = 0; i < n; i++)
        {
            var (o, q) = (original.Captures[i], port.Captures[i]);
            if (o == q) continue;
            RunOriginal(script, i, out var op);
            RunPort(script, i, out var pp, mutate);
            string where = "planes equal (DAC or start differ)";
            for (int pl = 0; pl < 4 && op is not null && pp is not null; pl++)
            {
                int k;
                for (k = 0; k < op[pl].Length; k++)
                    if (op[pl][k] != pp[pl][k]) break;
                if (k < op[pl].Length)
                {
                    int px = (k % 80) * 4 + pl;
                    where = $"plane {pl} offset {k:X4} (page {k / 0x4000}, x {px}, y {k % 0x4000 / 80}) original {op[pl][k]:X2} port {pp[pl][k]:X2}";
                    break;
                }
            }
            Assert.Fail($"{label}: capture {i} ({o.Kind}) differs: original start {o.DisplayStart:X4} retraces {o.Retraces}, port {q.Kind} start {q.DisplayStart:X4} retraces {q.Retraces}; {where}");
        }
        Assert.True(original.Captures.Count == port.Captures.Count,
            $"{label}: captures original {original.Captures.Count} ({string.Join(",", original.Captures.Select(c => c.Kind).Distinct())}), port {port.Captures.Count}");
        Assert.Equal(original.Al, port.Al);
        Assert.Contains(original.Captures, c => c.Kind == "fade-in");
        Console.WriteLine($"{label}: {original.Captures.Count} captures, {original.Captures.Select(c => c.Hash).Distinct().Count()} distinct");
    }
}
