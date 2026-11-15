using Traders.Assets;
using Traders.Core;
using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.Phases;
using Traders.Core.Pixels;
using Traders.Core.Startup;
using Traders.Core.State;
using Traders.Core.Stations;
using Traders.Presentation.Menus;
using Traders.Presentation.Stations;
using Traders.RefEmu;
using Traders.RefEmu.Machine;
using Traders.Tests.Presentation;
using Traders.Vga;
using Traders.Vga.Layers;

namespace Traders.Tests.Stations;

/// <summary>
/// The stations phase 1B03:0009 picture by picture against the original (lane stationen part 3). The original runs the phase
/// in the RefEmu with all its output (command list, VGA pages, objects, palettes, fades and its retrace waits); only the
/// sounds 21A7:0054 are far returns. A picture is one retrace wait (entry into 0D73:00D3): there the four VRAM planes, the
/// DAC, the CRTC start and the command list DS:15BE/15C0 (count and entries) are captured. The port runs
/// <see cref="StationPhase.Run"/> on the original's start state with its rule page, every output call played at once by
/// <see cref="StationScene"/> through graphics layers set up from the same state; the layers' retrace waits capture the same
/// values. Keys change at every page flip called by 1B03 on both sides (as in <see cref="StationsLockstep"/>). Expected
/// values come from the original only.
/// </summary>
internal static class StationsSceneHarness
{
    private const ushort Seg21FD = 0x21FD, SegRetrace = 0x0D73, Driver = 0x23CD;

    public sealed record Capture(ulong Picture, ulong List, int DisplayStart, int Retrace);

    public sealed class Result
    {
        public required List<Capture> Captures { get; init; }
        public required byte[] DataSegment { get; init; }
        public byte[][]? Kept { get; init; }
        public byte[]? KeptDac { get; init; }
        public HashSet<StationCall> Calls { get; init; } = [];

        /// <summary>Port: the last 30 output calls before the kept picture.</summary>
        public List<string>? KeptEvents { get; init; }
    }

    private static ulong ListHash(byte[] ds)
    {
        int count = ds[GraphicsLayers.ListCount] | ds[GraphicsLayers.ListCount + 1] << 8;
        int length = 2 + Math.Clamp(count, 0, 100) * GraphicsLayers.EntrySize;
        ulong h = 14695981039346656037;
        foreach (byte b in ds.AsSpan(GraphicsLayers.ListCount, length)) h = (h ^ b) * 1099511628211;
        return h;
    }

    /// <summary>
    /// The original from <paramref name="from"/> (or the menu snapshot with font <paramref name="font"/>) after
    /// <paramref name="setUp"/>: returns the start snapshot and the run of 1B03:0009.
    /// </summary>
    public static (MachineSnapshot Start, List<int> Clock, Result Run) RunOriginal(Action<OriginalProgram> setUp, int font,
        Func<int, IReadOnlyCollection<byte>> keys, MachineSnapshot? from, int keep)
    {
        var p = OriginalProgram.Resume(TestGameData.DosDir, from ?? MenuSceneHarness.AtMenu.Value);
        var m = p.Machine;
        m.Mem.Write8(p.Segment(0x21A7), 0x0054, 0xCB);
        if (from is null) p.Call(Seg21FD, 0x05DB, (ushort)font);
        setUp(p);
        var start = p.Snapshot();

        var clock = new List<int>();
        var captures = new List<Capture>();
        byte[][]? kept = null;
        byte[]? keptDac = null;
        bool armed = false;
        int pass = 0;
        ushort seg1B03 = p.Segment(StationsLockstep.Seg);
        void Take()
        {
            var vga = m.Vga;
            if (captures.Count == keep)
            {
                kept = vga.Planes.Select(x => (byte[])x.Clone()).ToArray();
                keptDac = (byte[])vga.Dac.Clone();
            }
            captures.Add(new(MenuSceneHarness.Hash(vga.Planes, vga.Dac, vga.StartAddress), ListHash(p.DataSegment()),
                vga.StartAddress, captures.Count));
        }
        m.AddCodeHook(p.Segment(Seg21FD), 0x01CE, mm => { if (armed) clock.Add(mm.Cpu.Ax | (mm.Cpu.Dx << 16)); });
        m.AddCodeHook(p.Segment(SegRetrace), 0x00D3, _ => { if (armed) Take(); });
        m.AddCodeHook(p.Segment(Seg21FD), 0x051F, mm =>
        {
            if (armed && mm.Mem.Read16(mm.Cpu.Ss, (ushort)(mm.Cpu.Sp + 2)) == seg1B03) WriteKeys(p, keys(++pass));
        });
        WriteKeys(p, keys(0));
        armed = true;
        m.CallFar(seg1B03, 0x0009, [], 4_000_000_000);
        armed = false;
        Take();
        return (start, clock, new Result { Captures = captures, DataSegment = p.DataSegment()[..DgroupLayout.Size], Kept = kept, KeptDac = keptDac });
    }

    private static void WriteKeys(OriginalProgram p, IReadOnlyCollection<byte> held)
    {
        for (int sc = 0; sc < KeyTable.Length; sc++) p.WriteByte((ushort)(DgroupLayout.KeyState + sc), 0);
        foreach (byte sc in held) p.WriteByte((ushort)(DgroupLayout.KeyState + sc), 1);
    }

    private sealed class FarBuffer(ushort segment, ushort offset, byte[] data) : IFarMemory
    {
        private int Index(ushort seg, ushort off)
        {
            int i = (seg - segment) * 16 + off - offset;
            if (i < 0 || i >= data.Length) throw new InvalidOperationException($"Far access {seg:X4}:{off:X4} outside the text buffer.");
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

    /// <summary>Retrace waits of the layers: the capture at the entry of each.</summary>
    private sealed class Capturing(Action take) : IRetraceWait
    {
        public void WaitForRetrace() => take();
    }

    /// <summary>
    /// The port from the original's start state: core data segment, rule page from both VRAM pages, graphics layers with the
    /// data segment image, VRAM, DAC and driver registers of the start; fonts and masks from FONTS.PP and MASKS.PP, the text
    /// buffer from the original's memory. random(0) gets the original's clock values.
    /// </summary>
    public static Result RunPort(MachineSnapshot start, List<int> clock, int font, Func<int, IReadOnlyCollection<byte>> keys,
        int keep, bool viaStep = false)
    {
        var snap = OriginalProgram.Resume(TestGameData.DosDir, start);
        var ds = snap.DataSegment();
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
        driver.Masks = StationsLockstep.Masks.Value;
        var dac = new VgaDac();
        vga.Dac.CopyTo(dac.Registers, 0);

        var captures = new List<Capture>();
        byte[][]? kept = null;
        byte[]? keptDac = null;
        GraphicsLayers? layers = null;
        Func<StationPhase?> currentPhase = () => null;
        List<string>? keptEvents = null;
        void Take()
        {
            if (captures.Count == keep)
            {
                kept = driver.Vram.Planes.Select(x => (byte[])x.Clone()).ToArray();
                keptDac = (byte[])dac.Registers.Clone();
            }
            if (captures.Count == keep) keptEvents = currentPhase()?.Events.TakeLast(30).Select(x => x.ToString()).ToList();
            captures.Add(new(MenuSceneHarness.Hash(driver.Vram.Planes, dac.Registers, driver.DisplayStart), ListHash(layers!.Ds),
                driver.DisplayStart, captures.Count));
        }
        layers = new GraphicsLayers(driver, (byte[])ds.Clone(), dac, new Capturing(Take)) { DataSegment = snap.Dgroup };
        ushort textOff = core.U16(MenuLayout.TextBuffer), textSeg = core.U16(MenuLayout.TextBuffer + 2);
        var textBytes = snap.Machine.Mem.ReadBlock(textSeg, textOff, 0x8000);
        layers.OtherMemory = new FarBuffer(textSeg, textOff, (byte[])textBytes.Clone());
        var scene = new StationScene(layers, core, new Pictures());
        var random = new OriginalRandom(new RecordedClock(clock.ToList()), core.U32(DgroupLayout.RandomSeed));
        var texts = new TextFile(textBytes, textOff, textSeg);
        if (viaStep) return RunStep(core, random, scene, texts, font, keys, captures, () => (kept, keptDac, keptEvents), Take,
            source => currentPhase = source);

        var page = new RulePage(RulePage.DrawPageOf(core));
        int draw = page.DrawPage;
        for (int i = 0; i < 2; i++)
        {
            page.SelectDrawPage(i);
            page.Blit(StationsLockstep.VramPage(snap, i), 0, 0, RulePage.Width, RulePage.Height, 0, 0);
        }
        page.SelectDrawPage(draw);
        var phase = new StationPhase(core, random, KeyboardOnlyDevices.Instance)
        {
            Pixels = new StationsPage(page, StationsLockstep.Rakete.Value, StationsLockstep.Masks.Value, StationsLockstep.Font(font),
                texts, core),
            Output = scene.Apply,
        };
        currentPhase = () => phase;
        int pass = 0;
        KeyTable.Write(core, keys(0));
        foreach (var w in phase.Run())
        {
            if (w != StationWait.Flip) continue;
            KeyTable.Write(core, keys(++pass));
            if (pass > 100_000) throw new InvalidOperationException("Port coroutine did not finish.");
        }
        core.Set(DgroupLayout.RandomSeed, random.Seed);
        Take();
        return new Result
        {
            Captures = captures, DataSegment = core.Bytes[..DgroupLayout.Size], Kept = kept, KeptDac = keptDac,
            Calls = phase.Events.Select(e => e.Call).ToHashSet(), KeptEvents = keptEvents,
        };
    }

    /// <summary>
    /// The port through the handler of call 11 (<see cref="StationsStep"/>) in a <see cref="PhaseMachine"/> started at call 11:
    /// one frame per tick with the keys of the next pass, the scene as the step's output, the rule page empty at the start.
    /// The machine must stop at call 12 (no handler).
    /// </summary>
    private static Result RunStep(Dgroup core, OriginalRandom random, StationScene scene, TextFile texts, int font,
        Func<int, IReadOnlyCollection<byte>> keys, List<Capture> captures, Func<(byte[][]?, byte[]?, List<string>?)> kept,
        Action take, Action<Func<StationPhase?>> phaseSource)
    {
        var step = new StationsStep(new StationsInputs(StationsLockstep.Rakete.Value, StationsLockstep.Masks.Value,
            StationsLockstep.Font(font), texts), KeyboardOnlyDevices.Instance, scene.Apply, _ => new ImageFileLengths(100, 200));
        var machine = new PhaseMachine(core, random, new Dictionary<int, IWeekStepHandler> { [11] = step }, nextStep: 11);
        phaseSource(() => step.Phase);
        int pass = 0;
        machine.Advance(new PhaseTick(0, keys(0)));
        while (machine.Status == PhaseStatus.Waiting)
        {
            machine.Advance(new PhaseTick(pass + 1, keys(pass + 1)));
            pass++;
            if (pass > 100_000) throw new InvalidOperationException("Port step did not finish.");
        }
        Assert.Equal(PhaseStatus.Blocked, machine.Status);
        Assert.Equal(12, machine.NextStep);
        Assert.Equal(pass, step.Flips);
        take();
        var (planes, dac, events) = kept();
        return new Result
        {
            Captures = captures, DataSegment = core.Bytes[..DgroupLayout.Size], Kept = planes, KeptDac = dac,
            Calls = step.Phase!.Events.Select(e => e.Call).ToHashSet(), KeptEvents = events,
        };
    }

    /// <summary>
    /// Original and port picture by picture; on the first difference both are run again keeping that picture, and the first
    /// differing pixel (or the DAC entry) is named. Returns the number of pictures.
    /// </summary>
    public static (int Pictures, HashSet<StationCall> Calls, byte[] End) AssertSame(Action<OriginalProgram> setUp, int font,
        Func<int, IReadOnlyCollection<byte>> keys, string label, MachineSnapshot? from = null, bool viaStep = false)
    {
        var (start, clock, original) = RunOriginal(setUp, font, keys, from, -1);
        var port = RunPort(start, clock, font, keys, -1, viaStep);
        int n = Math.Min(original.Captures.Count, port.Captures.Count);
        for (int i = 0; i < n; i++)
        {
            var (o, q) = (original.Captures[i], port.Captures[i]);
            if (o == q) continue;
            string where = o.List != q.List ? "command list differs; " : "";
            if (o.Picture != q.Picture)
            {
                var ok = RunOriginal(_ => { }, font, keys, start, i).Run;
                var pk = RunPort(start, clock, font, keys, i, viaStep);
                where += Describe(ok.Kept!, pk.Kept!, ok.KeptDac!, pk.KeptDac!) + " – port calls before: " + string.Join(" | ", pk.KeptEvents ?? []);
            }
            Assert.Fail($"{label}: picture {i} of {original.Captures.Count} differs: start original {o.DisplayStart:X4}, port " +
                        $"{q.DisplayStart:X4}; {where}");
        }
        Assert.True(original.Captures.Count == port.Captures.Count,
            $"{label}: pictures original {original.Captures.Count}, port {port.Captures.Count}");
        var diff = StationsLockstep.Diff(original.DataSegment, port.DataSegment);
        Assert.True(diff.Count == 0, $"{label}: end: " + string.Join(", ", diff));
        return (original.Captures.Count, port.Calls, original.DataSegment);
    }

    private static string Describe(byte[][] op, byte[][] pp, byte[] od, byte[] pd)
    {
        int count = 0;
        string first = "";
        var more = new List<string>();
        for (int pl = 0; pl < 4; pl++)
            for (int k = 0; k < op[pl].Length; k++)
            {
                if (op[pl][k] == pp[pl][k]) continue;
                if (count < 12) more.Add($"p{pl}:{k:X4}({k % 0x4000 % 80 * 4 + pl},{k % 0x4000 / 80}) {op[pl][k]:X2}/{pp[pl][k]:X2}");
                if (count++ == 0)
                    first = $"plane {pl} offset {k:X4} (page {k / 0x4000}, x {k % 0x4000 % 80 * 4 + pl}, y {k % 0x4000 / 80}) " +
                            $"original {op[pl][k]:X2} port {pp[pl][k]:X2}";
            }
        if (count > 0) return $"{count} VRAM bytes differ, first {first}; " + string.Join(" ", more);
        for (int k = 0; k < od.Length; k++)
            if (od[k] != pd[k]) return $"DAC byte {k:X3} original {od[k]:X2} port {pd[k]:X2}";
        return "planes and DAC equal (start differs)";
    }
}
