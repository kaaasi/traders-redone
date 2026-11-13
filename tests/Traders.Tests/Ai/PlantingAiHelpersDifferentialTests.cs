using Traders.Core;
using Traders.Core.Field;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.Input;

namespace Traders.Tests.Ai;

/// <summary>
/// Differential tests of the two field helpers the computer turn needs (findings docs/routines/0609_074D_pflanzbar.md and
/// 0609_100F_nachbar.md). The same data segment goes into the original (RefEmu, LoadOnly, direct Call) and the port; AL and
/// the whole data segment must match. These routines write nothing to the data segment.
/// </summary>
public class PlantingAiHelpersDifferentialTests
{
    private const ushort Seg = 0x0609;
    private const int Owner = 0x4442, Kind = 0x2EB1, Quality = 0x45DA, Cols = 0x08FC, Rows = 0x08FE, Cur = 0x37AA;
    private const int LoopX = 0x2EB0, LoopY = 0x2F00;

    private static (OriginalProgram P, byte[] Initial) Load()
    {
        var p = OriginalProgram.LoadOnly(TestGameData.DosDir);
        return (p, p.DataSegment());
    }

    private static void Arrange(OriginalProgram p, byte[] initial, Random r)
    {
        p.Machine.Mem.WriteBlock(Traders.RefEmu.Cpu.Memory.Linear(p.Dgroup, 0), initial);
        p.WriteWord(Cols, (ushort)r.Next(1, 10));
        p.WriteWord(Rows, (ushort)r.Next(1, 7));
        p.WriteByte(Cur, (byte)r.Next(4));
        for (int c = 0; c < 9 * 6; c++)
        {
            p.WriteByte((ushort)(Owner + c), (byte)(r.Next(4) == 0 ? 0xFF : r.Next(4)));
            p.WriteByte((ushort)(Kind + c), (byte)new[] { -2, 0, 1, 2, 3, 4 }[r.Next(6)]);
            p.WriteByte((ushort)(Quality + c), (byte)r.Next(0, 4));
        }
    }

    // [DIFFERENTIAL] 0609:074D
    [GameDataFact]
    public void CanPlant_matches_the_original()
    {
        var (p, initial) = Load();
        var r = new Random(0x074D);
        for (int n = 0; n < 800; n++)
        {
            Arrange(p, initial, r);
            p.WriteByte(LoopX, (byte)(r.Next(10) == 0 ? r.Next(-2, 11) : r.Next(0, 9)));
            p.WriteByte(LoopY, (byte)(r.Next(10) == 0 ? r.Next(-2, 8) : r.Next(0, 6)));
            var ds = new Dgroup(p.DataSegment());
            byte al = (byte)p.Call(Seg, 0x074D).Ax;
            Assert.True(al == (PlantingAiHelpers.CanPlant(ds) ? 1 : 0),
                $"074D case {n}: x={ds.S8(LoopX)} y={ds.S8(LoopY)} al={al}");
            ControlHarness.AssertSameData($"074D case {n}", p.DataSegment(), ds.Bytes);
        }
    }

    // [DIFFERENTIAL] 0609:100F
    [GameDataFact]
    public void HasOwnNeighbour_matches_the_original()
    {
        var (p, initial) = Load();
        var r = new Random(0x100F);
        for (int n = 0; n < 800; n++)
        {
            Arrange(p, initial, r);
            short x = (short)(r.Next(10) == 0 ? r.Next(-2, 11) : r.Next(0, 9));
            short y = (short)(r.Next(10) == 0 ? r.Next(-2, 8) : r.Next(0, 6));
            var ds = new Dgroup(p.DataSegment());
            byte al = (byte)p.Call(Seg, 0x100F, (ushort)x, (ushort)y).Ax;
            Assert.True(al == (PlantingAiHelpers.HasOwnNeighbour(ds, x, y) ? 1 : 0), $"100F({x},{y}) case {n}");
            ControlHarness.AssertSameData($"100F case {n}", p.DataSegment(), ds.Bytes);
        }
    }
}
