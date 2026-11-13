using Traders.Assets;
using Traders.Core.Field;
using Traders.Core.State;
using Traders.Presentation.Field;
using Traders.RefEmu;
using Traders.RefEmu.Machine;
using Traders.Vga;
using Traders.Vga.Layers;

namespace Traders.Tests.Field;

/// <summary>
/// The field layer's drawing routines on a week state: the original started as a new game (four computer players) and run
/// to the entry of the week screen 0825:0072, right after 0825:000A has loaded its picture (186C:0002). Both sides start
/// from that snapshot; the original calls 0A96:001B (empty the list), the routines under test with their arguments and
/// 0A96:06A8 (execute the list), the port does the same through <see cref="FieldScene"/> and the graphics layers on the
/// snapshot's VRAM, atlas, masks and driver registers. The command list (count DS:15BE and its entries from DS:15C0) is
/// compared before it is executed (#103 P3-3), then all four planes.
/// </summary>
internal static class FieldSceneHarness
{
    private const ushort Seg0A96 = 0x0A96, Driver = 0x23CD;

    internal static readonly Lazy<MachineSnapshot> AtWeek = new(() =>
    {
        var p = OriginalProgram.StartGame(TestGameData.DosDir);
        Assert.True(p.RunToRoutine(0x0825, 0x0072, 120), "0825:0072 not reached after the new game.");
        return p.Snapshot();
    });

    /// <summary>One side of a case: DS bytes to set first, then the drawing calls.</summary>
    public sealed record Case(string Label, (int Offset, byte Value)[] Set, Action<OriginalProgram> Original,
        Action<Dgroup, FieldScene> Port);

    /// <summary>The command list before execution: count DS:15BE, then the entries from DS:15C0.</summary>
    private static byte[] List(byte[] ds)
    {
        int n = ds[GraphicsLayers.ListCount] | ds[GraphicsLayers.ListCount + 1] << 8;
        return ds.AsSpan(GraphicsLayers.ListCount, 2 + n * GraphicsLayers.EntrySize).ToArray();
    }

    /// <summary>The original: set, empty the list, draw, execute; returns the four planes, the data segment and the list.</summary>
    public static (byte[][] Planes, byte[] Ds, byte[] List) RunOriginalWithList(Case c)
    {
        var p = OriginalProgram.Resume(TestGameData.DosDir, AtWeek.Value);
        foreach (var (at, v) in c.Set) p.WriteByte((ushort)at, v);
        p.Call(Seg0A96, 0x001B);
        c.Original(p);
        var list = List(p.DataSegment());
        p.Call(Seg0A96, 0x06A8);
        return (p.Machine.Vga.Planes.Select(x => (byte[])x.Clone()).ToArray(), p.DataSegment(), list);
    }

    /// <summary>The original: set, empty the list, draw, execute; returns the four planes and the data segment.</summary>
    public static (byte[][] Planes, byte[] Ds) RunOriginal(Case c)
    {
        var (planes, ds, _) = RunOriginalWithList(c);
        return (planes, ds);
    }

    /// <summary>The port on the snapshot's state; returns the four planes and the core's data segment.</summary>
    public static (byte[][] Planes, byte[] Core) RunPort(Case c)
    {
        var (planes, core, _) = RunPortWithList(c);
        return (planes, core);
    }

    /// <summary>The port on the snapshot's state; returns the four planes, the core's data segment and the list.</summary>
    public static (byte[][] Planes, byte[] Core, byte[] List) RunPortWithList(Case c)
    {
        var snap = OriginalProgram.Resume(TestGameData.DosDir, AtWeek.Value);
        var ds = snap.DataSegment();
        foreach (var (at, v) in c.Set) ds[at] = v;
        var core = new Dgroup(ds[..DgroupLayout.Size]);
        var mem = snap.Machine.Mem;
        var driver = DriverFrom(snap);

        var dac = new VgaDac();
        snap.Machine.Vga.Dac.CopyTo(dac.Registers, 0);
        var layers = new GraphicsLayers(driver, (byte[])ds.Clone(), dac, new RetraceCounter())
            { DataSegment = snap.Dgroup, OtherMemory = new MachineMemory(mem) };
        var scene = new FieldScene(layers, core);
        layers.ClearList();
        c.Port(core, scene);
        var list = List(layers.Ds);
        layers.ExecuteList();
        return (driver.Vram.Planes.Select(x => (byte[])x.Clone()).ToArray(), core.Bytes, list);
    }

    /// <summary>
    /// A driver with the original's state: VRAM, display start, draw page, colour, line pattern, map mask and read map, the
    /// atlas (far pointer CS:0582, set by 23CD:058A) and masks (CS:057E) from its memory, and the fonts from FONTS.PP as the
    /// start (0DFD) sets them up (checked against the original's table at driver CS:057A).
    /// </summary>
    internal static ModeXDriver DriverFrom(OriginalProgram snap)
    {
        var driver = new ModeXDriver();
        SyncDriver(snap, driver);
        driver.Font = DriverWord(snap, 0x0586);
        var fontData = PowerPacker.Decrunch(File.ReadAllBytes(TestGameData.Dos(ResourceTable.Fonts)));
        driver.Fonts =
        [
            new FontDescriptor(fontData, 0x5A0, 5, 6, 0x20), new FontDescriptor(fontData, 0, 12, 14, 0x20),
            new FontDescriptor(fontData, 0x7F8, 5, 5, 0x2F),
        ];
        var mem = snap.Machine.Mem;
        for (int i = 0; i < driver.Fonts.Count; i++)
        {
            ushort FontWord(int k) => mem.Read16(DriverWord(snap, 0x057C), (ushort)(DriverWord(snap, 0x057A) + i * 10 + k));
            var f = driver.Fonts[i];
            Assert.True(FontWord(4) == f.Height && FontWord(6) == f.Advance && FontWord(8) == f.FirstChar, $"font {i} differs from the original's table");
        }
        return driver;
    }

    private static ushort DriverWord(OriginalProgram p, ushort off) => p.Machine.Mem.Read16(p.Segment(Driver), off);

    /// <summary>The driver's state taken over from the original (VRAM, registers, atlas, masks); the fonts stay.</summary>
    internal static void SyncDriver(OriginalProgram snap, ModeXDriver driver)
    {
        var mem = snap.Machine.Mem;
        var vga = snap.Machine.Vga;
        for (int i = 0; i < 4; i++) vga.Planes[i].CopyTo(driver.Vram.Planes[i], 0);
        driver.SetDisplayStart((ushort)vga.StartAddress, 0xA000);
        driver.SetDrawPage(DriverWord(snap, 0x0000), DriverWord(snap, 0x0002));
        driver.Color = DriverWord(snap, 0x0588);
        driver.LinePattern = DriverWord(snap, 0x04F4);
        driver.Vram.MapMask = (byte)(vga.Seq[2] & 0x0F);
        driver.Vram.ReadMap = (byte)(vga.Gc[4] & 0x03);
        driver.Atlas = new PlanarAtlas(mem.ReadBlock(DriverWord(snap, 0x0584), DriverWord(snap, 0x0582), PlanarAtlas.Length));
        driver.Masks = mem.ReadBlock(DriverWord(snap, 0x0580), DriverWord(snap, 0x057E), driver.Masks.Length);
    }

    /// <summary>
    /// Runs both sides; the command lists and then the planes must agree byte for byte (first difference named).
    /// </summary>
    public static (byte[] OriginalDs, byte[] PortCore) AssertSame(Case c, bool compareList = true)
    {
        var (op, ods, ol) = RunOriginalWithList(c);
        var (pp, core, pl0) = RunPortWithList(c);
        if (compareList) AssertSameList(c.Label, ol, pl0);
        for (int pl = 0; pl < 4; pl++)
        {
            int k = 0;
            while (k < op[pl].Length && op[pl][k] == pp[pl][k]) k++;
            if (k < op[pl].Length)
                Assert.Fail($"{c.Label}: plane {pl} offset {k:X4} (page {k / 0x4000}, x {(k % 80) * 4 + pl}, y {k % 0x4000 / 80}) " +
                            $"original {op[pl][k]:X2} port {pp[pl][k]:X2}");
        }
        return (ods, core);
    }

    private static void AssertSameList(string label, byte[] ol, byte[] pl)
    {
        if (ol.Length != pl.Length)
            Assert.Fail($"{label}: command list with {(ol.Length - 2) / GraphicsLayers.EntrySize} entries in the original, " +
                        $"{(pl.Length - 2) / GraphicsLayers.EntrySize} in the port");
        int e = 0;
        while (e < ol.Length && ol[e] == pl[e]) e++;
        if (e < ol.Length)
            Assert.Fail($"{label}: command list entry {(e - 2) / GraphicsLayers.EntrySize} byte {(e - 2) % GraphicsLayers.EntrySize:X2} " +
                        $"original {ol[e]:X2} port {pl[e]:X2}");
    }

    /// <summary>True if the case changes VRAM against the snapshot (a test that draws nothing proves nothing).</summary>
    public static bool Draws(Case c)
    {
        var (op, _) = RunOriginal(c);
        var before = OriginalProgram.Resume(TestGameData.DosDir, AtWeek.Value).Machine.Vga.Planes;
        return Enumerable.Range(0, 4).Any(pl => !op[pl].AsSpan().SequenceEqual(before[pl]));
    }

    /// <summary>
    /// Core bytes of the object records (all eight; DS:37AA is often 4 after a player loop, #103 P3-2/P3-4) get patterns ≠ 0
    /// first, so a scene that writes them (for example by hiding the cursor) changes the compared data segment.
    /// </summary>
    private static readonly (int, byte)[] OwnedPatterns =
    [
        .. Enumerable.Range(0, 8).SelectMany(p => new[] { 0, 2, 0x14, 0x16, 0x18 }
            .Select(o => (Planting.Objects + Planting.ObjectSize * p + o, (byte)(0xA5 + p + o)))),
    ];

    /// <summary><see cref="AssertSame"/> with the object patterns set; the scene must leave the core's data segment as it was.</summary>
    public static void AssertReadOnly(Case c, bool compareList = true)
    {
        c = c with { Set = [.. OwnedPatterns, .. c.Set] };
        var (_, core) = AssertSame(c, compareList);
        var before = OriginalProgram.Resume(TestGameData.DosDir, AtWeek.Value).DataSegment()[..DgroupLayout.Size];
        foreach (var (at, v) in c.Set) before[at] = v;
        Assert.True(before.AsSpan().SequenceEqual(core), $"{c.Label}: the scene changed the core's data segment");
    }

    /// <summary>Texts outside the data segment (far pointers into the loaded text file) read from the snapshot's memory.</summary>
    internal sealed class MachineMemory(Traders.RefEmu.Cpu.Memory mem) : IFarMemory
    {
        public byte Read(ushort segment, ushort offset) => mem.Read8(segment, offset);

        public void Write(ushort segment, ushort offset, byte value) => mem.Write8(segment, offset, value);
    }

    /// <summary>Records the cursor step's events and plays them through the scene at once.</summary>
    public sealed class SceneOutput(FieldScene scene) : IFieldOutput
    {
        public List<FieldEvent> Events { get; } = [];

        public void ComputerTurn(Dgroup d, short x, short y) => throw new InvalidOperationException("no computer turn expected");

        public void Emit(FieldEvent e)
        {
            Events.Add(e);
            scene.Apply(e);
        }
    }
}
