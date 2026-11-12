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
/// snapshot's VRAM, atlas, masks and driver registers. All four planes are compared.
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

    /// <summary>The original: set, empty the list, draw, execute; returns the four planes and the data segment.</summary>
    public static (byte[][] Planes, byte[] Ds) RunOriginal(Case c)
    {
        var p = OriginalProgram.Resume(TestGameData.DosDir, AtWeek.Value);
        foreach (var (at, v) in c.Set) p.WriteByte((ushort)at, v);
        p.Call(Seg0A96, 0x001B);
        c.Original(p);
        p.Call(Seg0A96, 0x06A8);
        return (p.Machine.Vga.Planes.Select(x => (byte[])x.Clone()).ToArray(), p.DataSegment());
    }

    /// <summary>The port on the snapshot's state; returns the four planes and the core's data segment.</summary>
    public static (byte[][] Planes, byte[] Core) RunPort(Case c)
    {
        var snap = OriginalProgram.Resume(TestGameData.DosDir, AtWeek.Value);
        var ds = snap.DataSegment();
        foreach (var (at, v) in c.Set) ds[at] = v;
        var core = new Dgroup(ds[..DgroupLayout.Size]);
        var mem = snap.Machine.Mem;
        ushort DriverWord(ushort off) => mem.Read16(snap.Segment(Driver), off);

        var driver = new ModeXDriver();
        var vga = snap.Machine.Vga;
        for (int i = 0; i < 4; i++) vga.Planes[i].CopyTo(driver.Vram.Planes[i], 0);
        driver.SetDisplayStart((ushort)vga.StartAddress, 0xA000);
        driver.SetDrawPage(DriverWord(0x0000), DriverWord(0x0002));
        driver.Color = DriverWord(0x0588);
        driver.LinePattern = DriverWord(0x04F4);
        driver.Vram.MapMask = (byte)(vga.Seq[2] & 0x0F);
        driver.Vram.ReadMap = (byte)(vga.Gc[4] & 0x03);
        // atlas (far pointer CS:0582, set by 23CD:058A) and masks (CS:057E) as the original holds them
        driver.Atlas = new PlanarAtlas(mem.ReadBlock(DriverWord(0x0584), DriverWord(0x0582), PlanarAtlas.Length));
        driver.Masks = mem.ReadBlock(DriverWord(0x0580), DriverWord(0x057E), driver.Masks.Length);

        var dac = new VgaDac();
        vga.Dac.CopyTo(dac.Registers, 0);
        var layers = new GraphicsLayers(driver, (byte[])ds.Clone(), dac, new RetraceCounter()) { DataSegment = snap.Dgroup };
        var scene = new FieldScene(layers, core);
        layers.ClearList();
        c.Port(core, scene);
        layers.ExecuteList();
        return (driver.Vram.Planes.Select(x => (byte[])x.Clone()).ToArray(), core.Bytes);
    }

    /// <summary>Runs both sides; the planes must agree byte for byte (first difference named) and the draw must change VRAM.</summary>
    public static (byte[] OriginalDs, byte[] PortCore) AssertSame(Case c)
    {
        var (op, ods) = RunOriginal(c);
        var (pp, core) = RunPort(c);
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

    /// <summary>True if the case changes VRAM against the snapshot (a test that draws nothing proves nothing).</summary>
    public static bool Draws(Case c)
    {
        var (op, _) = RunOriginal(c);
        var before = OriginalProgram.Resume(TestGameData.DosDir, AtWeek.Value).Machine.Vga.Planes;
        return Enumerable.Range(0, 4).Any(pl => !op[pl].AsSpan().SequenceEqual(before[pl]));
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
