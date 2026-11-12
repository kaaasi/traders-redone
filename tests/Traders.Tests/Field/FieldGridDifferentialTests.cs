using Traders.Core.Field;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.Input;
using F = Traders.Core.Field.FieldGrid;

namespace Traders.Tests.Field;

/// <summary>
/// Differential tests of the field layer routines 0609:10A7, 1072 and 10D2 (finding docs/routines/0609_10D2_feldart.md): the same
/// data segment and arguments go into the original (RefEmu, LoadOnly, direct calls) and the port; AL and the whole data
/// segment must be equal.
/// </summary>
public class FieldGridDifferentialTests
{
    private const ushort Seg = 0x0609;

    private static readonly short[] Coordinates = [-32768, -2, -1, 0, 0, 1, 2, 3, 4, 5, 5, 6, 8, 9, 10, 300];
    private static readonly short[] Owners = [-1, -1, 0, 1, 2, 3, 0, 1, 2, 3, -2, 4, 7];
    private static readonly short[] Kinds = [-2, -1, 0, 1, 2, 3, 4, 0, 1, 2, 3, 5, -3, 255];

    private static (OriginalProgram P, byte[] Initial) Load()
    {
        var p = OriginalProgram.LoadOnly(TestGameData.DosDir);
        return (p, p.DataSegment());
    }

    private static void Arrange(OriginalProgram p, byte[] initial, Random r)
    {
        p.Machine.Mem.WriteBlock(Traders.RefEmu.Cpu.Memory.Linear(p.Dgroup, 0), initial);
        p.WriteWord(F.Columns, (ushort)(r.Next(8) == 0 ? r.Next(-2, 12) : r.Next(4, 10)));
        p.WriteWord(F.Rows, (ushort)(r.Next(8) == 0 ? r.Next(-2, 9) : r.Next(3, 7)));
        p.WriteByte(F.CurrentPlayer, (byte)(r.Next(6) == 0 ? 0xFF : r.Next(4)));
        for (int c = 0; c < 9 * 6; c++)
        {
            p.WriteByte((ushort)(F.Owner + c), (byte)Owners[r.Next(Owners.Length - 3)]);       // -1..3
            p.WriteByte((ushort)(F.Kind + c), (byte)Kinds[r.Next(7)]);                        // -2..4
        }
        p.WriteByte(F.FreeFields, (byte)r.Next(256));
        for (int i = 0; i < 4; i++) p.WriteByte((ushort)(F.OwnedFields + i), (byte)r.Next(256));
        for (int i = 0; i < 16; i++) p.WriteByte((ushort)(F.OwnedKinds + i), (byte)r.Next(256));
    }

    // [DIFFERENTIAL] 0609:10A7
    // [DIFFERENTIAL] 0609:1072
    [GameDataFact]
    public void Bounds_and_ownership_tests_match_the_original()
    {
        var (p, initial) = Load();
        var r = new Random(0x10A7);
        for (int n = 0; n < 400; n++)
        {
            Arrange(p, initial, r);
            short x = Coordinates[r.Next(Coordinates.Length)], y = Coordinates[r.Next(Coordinates.Length)];
            var ds = new Dgroup(p.DataSegment());
            byte inside = (byte)p.Call(Seg, 0x10A7, (ushort)x, (ushort)y).Ax;
            Assert.True(inside == (F.Inside(ds, x, y) ? 1 : 0), $"10A7({x},{y}) cols {ds.S16(F.Columns)} rows {ds.S16(F.Rows)}");
            if (!F.Inside(ds, x, y) && r.Next(2) == 0) continue; // 1072 outside the field reads no cell; keep some such cases
            byte owned = (byte)p.Call(Seg, 0x1072, (ushort)x, (ushort)y).Ax;
            Assert.True(owned == (F.OwnedByCurrent(ds, x, y) ? 1 : 0), $"1072({x},{y})");
            ControlHarness.AssertSameData($"case {n}", p.DataSegment(), ds.Bytes);
        }
    }

    // [DIFFERENTIAL] 0609:10D2
    [GameDataFact]
    public void Set_field_matches_the_original_including_counters()
    {
        var (p, initial) = Load();
        var r = new Random(0x10D2);
        for (int n = 0; n < 600; n++)
        {
            Arrange(p, initial, r);
            short x = (short)r.Next(0, 9), y = (short)r.Next(0, 6);
            if (r.Next(10) == 0)
            {
                // Old owner and kind outside the usual range (counters next to the tables are hit).
                p.WriteByte((ushort)(F.Owner + x * 6 + y), (byte)Owners[r.Next(Owners.Length)]);
                p.WriteByte((ushort)(F.Kind + x * 6 + y), (byte)Kinds[r.Next(Kinds.Length)]);
            }
            short owner = Owners[r.Next(Owners.Length)], kind = Kinds[r.Next(Kinds.Length)];
            var ds = new Dgroup(p.DataSegment());
            F.Set(ds, x, y, owner, kind);
            p.Call(Seg, 0x10D2, (ushort)x, (ushort)y, (ushort)owner, (ushort)kind);
            ControlHarness.AssertSameData($"10D2({x},{y},{owner},{kind}) case {n}", p.DataSegment(), ds.Bytes);
        }
    }

    [GameDataFact]
    public void Setting_a_free_field_moves_it_to_the_player()
    {
        var (p, _) = Load();
        p.WriteByte(F.Owner + 2 * 6 + 1, 0xFF);
        p.WriteByte(F.Kind + 2 * 6 + 1, 0xFE);
        p.WriteByte(F.FreeFields, 20);
        var ds = new Dgroup(p.DataSegment());
        F.Set(ds, 2, 1, 3, 1);
        p.Call(Seg, 0x10D2, 2, 1, 3, 1);
        ControlHarness.AssertSameData("free to player 3", p.DataSegment(), ds.Bytes);
        Assert.Equal(19, p.ReadByte(F.FreeFields));
        Assert.Equal(1, p.ReadByte(F.OwnedFields + 3));
        Assert.Equal(1, p.ReadByte(F.OwnedKinds + 3 * 4 + 1));
        Assert.Equal(3, ds.S8(F.Owner + 13));
    }
}
