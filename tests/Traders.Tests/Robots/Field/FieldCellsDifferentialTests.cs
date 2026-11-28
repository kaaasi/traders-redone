using Traders.Core.Robots.Field;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.Vga;
using Traders.Vga.Layers;
using L = Traders.Core.Robots.Field.BattleFieldLayout;

namespace Traders.Tests.Robots.Field;

/// <summary>
/// Differential tests of the battle field cell helpers 1E7E:0DB3, 083F, 0870, 088D and 08F1 (finding
/// docs/routines/1E7E_0004_kampffeld.md): the same data segment and arguments go into the original (RefEmu, LoadOnly, direct
/// calls after 0A96:001B) and the port. The port's commands are put into the command list of <see cref="GraphicsLayers"/>
/// on a copy of the data segment; afterwards the whole 64 KB data segment – grid, list entries, list pointer and count,
/// robot frames and the random state DS:10D2 – must be equal.
/// </summary>
public class FieldCellsDifferentialTests
{
    private const ushort Seg = 0x1E7E;

    private static readonly short[] Coordinates = [-3, -1, 0, 0, 1, 2, 5, 12, 23, 24, 24, 25, 25, 26, 27];
    private static readonly short[] Edges = [0, 0x19, 0, 0x19, -1, -4, 0x0FFF, 0x1000, 0x2000];

    private sealed class Case
    {
        public required OriginalProgram P { get; init; }
        public required byte[] Initial { get; init; }

        public byte[] Port { get; private set; } = [];
        public Dgroup D { get; private set; } = null!;
        public GraphicsLayers Layers { get; private set; } = null!;
        public List<FieldCommand> Commands { get; } = [];

        public void Begin(Random r)
        {
            P.Machine.Mem.WriteBlock(Traders.RefEmu.Cpu.Memory.Linear(P.Dgroup, 0), Initial);
            for (int i = 0; i < L.Size * L.Size; i++) P.WriteByte((ushort)(L.Grid + i), (byte)r.Next(256));
            for (int p = 0; p < 4; p++)
            {
                P.WriteByte((ushort)(L.RobotX + p), (byte)(r.Next(8) == 0 ? r.Next(256) : r.Next(0, 25)));
                P.WriteByte((ushort)(L.RobotY + p), (byte)(r.Next(8) == 0 ? r.Next(256) : r.Next(0, 25)));
                P.WriteByte((ushort)(L.RobotLabel + p), (byte)(r.Next(3) == 0 ? 0xFF : r.Next(4)));
                P.WriteByte((ushort)(L.LabelColour + p), (byte)r.Next(256));
                P.WriteByte((ushort)(L.RobotFrame + p), (byte)r.Next(256));
            }
            P.WriteByte(DgroupLayout.ActivePlayer, (byte)(r.Next(6) == 0 ? r.Next(256) : r.Next(4)));
            P.WriteDword(OriginalProgram.RandomStateOffset, (uint)r.Next());
            Port = new byte[0x10000];
            P.DataSegment().CopyTo(Port, 0);
            D = new Dgroup(Port);
            Layers = new GraphicsLayers(new ModeXDriver(), Port, new VgaDac(), new RetraceCounter()) { DataSegment = P.Dgroup };
            Commands.Clear();
        }

        /// <summary>Puts the port's commands into its list and compares the whole data segment with the original.</summary>
        public void AssertSame(string label)
        {
            foreach (var c in Commands)
            {
                if (c.Kind == FieldCommandKind.Blit8) Layers.AddBlit8(c.A, c.B, c.C, c.D, c.E);
                else Layers.AddText(c.A, c.B, c.C, c.D, c.E, c.F);
            }
            byte[] original = P.DataSegment();
            for (int i = 0; i < 0x10000; i++)
            {
                if (original[i] != Port[i])
                    Assert.Fail($"{label}: DS:{i:X4} original {original[i]:X2}, port {Port[i]:X2}; commands {string.Join(", ", Commands)}");
            }
        }
    }

    private static Case Load()
    {
        var p = OriginalProgram.LoadOnly(TestGameData.DosDir);
        p.Call(0x0A96, 0x001B);
        return new Case { P = p, Initial = p.DataSegment() };
    }

    // [DIFFERENTIAL] 1E7E:0DB3
    [GameDataFact]
    public void Cell_matches_the_original_including_half_rows_and_signs()
    {
        var c = Load();
        for (int n = 0; n < 400; n++)
        {
            var r = new Random(0x0DB3 + n);
            c.Begin(r);
            short x = Coordinates[r.Next(Coordinates.Length)];
            short y = r.Next(4) == 0 ? Edges[r.Next(Edges.Length)] : Coordinates[r.Next(Coordinates.Length)];
            if (y >= 0x1000) x = (short)(-(y / L.Stride)); // keep the grid write near the field
            ushort sx = (ushort)r.Next(0x10000), sy = (ushort)(r.Next(4) == 0 ? r.Next(0xFFF8, 0x10000) : r.Next(0, 200));
            ushort value = (ushort)r.Next(0x10000);
            FieldCells.Cell(c.D, sx, sy, (ushort)x, (ushort)y, (byte)value, c.Commands);
            c.P.Call(Seg, 0x0DB3, sx, sy, (ushort)x, (ushort)y, value);
            c.AssertSame($"0DB3({sx:X4},{sy:X4},{x},{y},{value:X4}) case {n}");
            Assert.Single(c.Commands);
        }
    }

    // [DIFFERENTIAL] 1E7E:083F
    // [DIFFERENTIAL] 1E7E:0870
    // [DIFFERENTIAL] 1E7E:088D
    [GameDataFact]
    public void Placement_cells_match_the_original()
    {
        var c = Load();
        ushort[] kinds = [0, 1, 2, 3, 4, 5, 0, 1, 2, 3, 4, 5, 6, 0x18, 0x19, 0xFF, 0x100, 0x1FFF, 0xFFFF];
        for (int n = 0; n < 600; n++)
        {
            var r = new Random(0x083F + n);
            c.Begin(r);
            ushort x = (ushort)Coordinates[r.Next(Coordinates.Length)], y = (ushort)Coordinates[r.Next(Coordinates.Length)];
            string what;
            switch (n % 3)
            {
                case 0:
                    ushort kind = kinds[r.Next(kinds.Length)];
                    what = $"083F({(short)x},{(short)y},{kind:X4})";
                    FieldCells.PlaceKind(c.D, x, y, kind, c.Commands);
                    c.P.Call(Seg, 0x083F, x, y, kind);
                    break;
                case 1:
                    what = $"0870({(short)x},{(short)y})";
                    FieldCells.PlaceBlock(c.D, x, y, c.Commands);
                    c.P.Call(Seg, 0x0870, x, y);
                    break;
                default:
                    what = $"088D({(short)x},{(short)y}) player {c.D.S8(DgroupLayout.ActivePlayer)}";
                    FieldCells.PlaceMarker(c.D, x, y, c.Commands);
                    c.P.Call(Seg, 0x088D, x, y);
                    break;
            }
            c.AssertSame($"{what} case {n}");
        }
    }

    // [DIFFERENTIAL] 1E7E:08F1
    [GameDataFact]
    public void Robot_matches_the_original_with_and_without_label()
    {
        var c = Load();
        ushort[] players = [0, 1, 2, 3, 0, 1, 2, 3, 4, 0xFFFF];
        byte[] frames = [0, 1, 2, 0, 1, 2, 3, 0x80, 0xFF];
        int labels = 0;
        for (int n = 0; n < 400; n++)
        {
            var r = new Random(0x08F1 + n);
            c.Begin(r);
            ushort player = players[r.Next(players.Length)];
            byte frame = frames[r.Next(frames.Length)];
            if (r.Next(10) == 0) c.P.WriteByte((ushort)(L.RobotLabel + player), (byte)(sbyte)r.Next(-3, 6)); // odd indices
            c.D.Set((ushort)(L.RobotLabel + player), c.P.ReadByte((ushort)(L.RobotLabel + player)));
            FieldCells.DrawRobot(c.D, player, frame, c.Commands);
            c.P.Call(Seg, 0x08F1, player, frame);
            c.AssertSame($"08F1({(short)player},{frame}) case {n}");
            Assert.InRange(c.Commands.Count, 4, 5);
            if (c.Commands.Count == 5) labels++;
        }
        Assert.InRange(labels, 50, 350); // both branches are covered
    }

    [GameDataFact]
    public void Robot_of_player_two_writes_its_four_cells_and_label_in_order()
    {
        var c = Load();
        c.Begin(new Random(7));
        c.P.WriteByte(L.RobotX + 2, 10);
        c.P.WriteByte(L.RobotY + 2, 0);
        c.P.WriteByte(L.RobotLabel + 2, 2);
        c.P.DataSegment().CopyTo(c.Port, 0);
        FieldCells.DrawRobot(c.D, 2, 0, c.Commands);
        c.P.Call(Seg, 0x08F1, 2, 0);
        c.AssertSame("08F1(2,0)");
        byte value = c.D.U8(L.FrameCellValue);
        Assert.Equal(value, c.D.U8(L.Grid + 26 * 10 + 0));
        Assert.Equal(value, c.D.U8(L.Grid + 26 * 11 + 0));
        Assert.Equal(value, c.D.U8(L.Grid + 26 * 10 + 1));
        Assert.Equal(value, c.D.U8(L.Grid + 26 * 11 + 1));
        Assert.Equal(new FieldCommand(FieldCommandKind.Blit8, 0, 12, 4, 0x88, 0), c.Commands[0]); // half row y = 0
        Assert.Equal(new FieldCommand(FieldCommandKind.Blit8, 8, 16, 8, 0x90, 4), c.Commands[3]);
        Assert.Equal(FieldCommandKind.Text, c.Commands[4].Kind);
        Assert.Equal((ushort)(10 * 8 + 0x3E), c.Commands[4].A);
        Assert.Equal((ushort)0xFFFF, c.Commands[4].F);
        Assert.Equal(0, c.D.U8(L.RobotFrame + 2));
    }
}
