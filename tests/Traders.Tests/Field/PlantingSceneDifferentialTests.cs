using Traders.Core;
using Traders.Core.Field;
using Traders.Core.State;
using Traders.Presentation.Field;
using Traders.Vga.Layers;
using H = Traders.Tests.Field.FieldSceneHarness;

namespace Traders.Tests.Field;

/// <summary>
/// Pixel and command-list tests of the planting phase's drawing routines (finding docs/routines/0609_0C73_anbauszene.md)
/// against the RefEmu on the week state of <see cref="H"/>: same data segment and arguments, the original's routine and
/// the scene each fill the command list 0A96, the lists are compared entry by entry, executed, and all four planes
/// compared (#103 P3-3). The scene must leave the core's data segment unchanged.
/// </summary>
public class PlantingSceneDifferentialTests
{
    private const ushort Seg = 0x0609;

    private static ushort W(int v) => unchecked((ushort)v);

    private static IEnumerable<(int, byte)> Word(int at, int v) => [(at, (byte)v), (at + 1, (byte)(v >> 8))];

    private static (int, byte)[] Player(int p) => [(FieldGrid.CurrentPlayer, (byte)p)];

    /// <summary>A random field: size, origin, terrain, owners with kinds, sale marks of the four players.</summary>
    private sealed record Board(int Columns, int Rows, (int, byte)[] Set);

    /// <summary>
    /// Kinds of an owned field: empty (bought), planted 0–2, rocket base 3, HQ 4. Kind −1 is unreachable (0609:096A plants
    /// only kinds 0–3, the other writers keep the kind or write 4/−2); it would read atlas and masks before their start.
    /// </summary>
    private static readonly sbyte[] OwnedKinds = [-2, 0, 1, 2, 3, 4, 4];

    private static Board RandomBoard(Random r)
    {
        int cols = r.Next(1, 10), rows = r.Next(1, 7);
        var set = new List<(int, byte)>();
        set.AddRange(Word(FieldGrid.Columns, cols));
        set.AddRange(Word(FieldGrid.Rows, rows));
        set.AddRange(Word(0x451E, (0x140 - (cols << 5)) / 2 + r.Next(-8, 9)));
        set.AddRange(Word(0x4528, 0x40 + (6 - rows) * 0x0B + r.Next(-6, 7)));
        bool uniform = r.Next(4) == 0;
        byte t0 = (byte)r.Next(4);
        for (int c = 0; c < 9 * 6; c++)
        {
            byte terrain = uniform ? t0 : (byte)r.Next(4);
            sbyte owner = r.Next(3) == 0 ? (sbyte)-1 : (sbyte)r.Next(4);
            sbyte kind = owner == -1 ? (sbyte)-2 : OwnedKinds[r.Next(OwnedKinds.Length)];
            set.Add((0x45DA + c, terrain));
            set.Add((FieldGrid.Owner + c, (byte)owner));
            set.Add((FieldGrid.Kind + c, (byte)kind));
        }
        for (int p = 0; p < 4; p++)
        {
            bool marked = r.Next(2) == 0;
            set.Add((0x3CAF + p, marked ? (byte)r.Next(cols) : (byte)0xFF));
            set.Add((0x3CB3 + p, marked ? (byte)r.Next(rows) : (byte)0xFF));
        }
        return new Board(cols, rows, [.. set]);
    }

    // [DIFFERENTIAL] 0609:1540
    // [DIFFERENTIAL] 0609:1CF4
    // [DIFFERENTIAL] 0609:1E6B
    // [DIFFERENTIAL] 0609:1C83
    // [DIFFERENTIAL] 0609:1CB1
    [GameDataFact]
    public void Fields_tiles_and_owner_borders_match_the_original()
    {
        var r = new Random(0x1540);
        bool drawn = false;
        for (int n = 0; n < 48; n++)
        {
            var b = RandomBoard(r);
            // a few fields per case (the list holds the drawing of one 0609:03E0 pass), the edges and outside included
            var cells = Enumerable.Range(0, 4).Select(_ => ((short)r.Next(-1, b.Columns + 1), (short)r.Next(-1, b.Rows + 1))).ToArray();
            var c = new H.Case($"1C83 board {n} ({b.Columns}×{b.Rows}) {string.Join(" ", cells)}", b.Set,
                o => { foreach (var (x, y) in cells) o.Call(Seg, 0x1C83, W(x), W(y)); },
                (_, s) => { foreach (var (x, y) in cells) s.Field(x, y); });
            if (!drawn && cells.Any(q => q.Item1 >= 0 && q.Item1 < b.Columns && q.Item2 >= 0 && q.Item2 < b.Rows))
                drawn = H.Draws(c);
            H.AssertReadOnly(c);

            var (cx, cy) = ((short)r.Next(-1, b.Columns + 1), (short)r.Next(-1, b.Rows + 1));
            H.AssertReadOnly(new H.Case($"1CB1({cx},{cy}) board {n}", b.Set,
                o => o.Call(Seg, 0x1CB1, W(cx), W(cy)), (_, s) => s.FieldAndNeighbours(cx, cy)));
        }
        Assert.True(drawn, "1C83 draws nothing");
    }

    // [DIFFERENTIAL] 0609:1540
    [GameDataFact]
    public void Tile_draws_every_kind_on_every_terrain_like_the_original()
    {
        // one field 3 × 3 with the middle under test: all terrains × kinds × same-terrain neighbour patterns
        var r = new Random(0x15C9);
        foreach (byte terrain in new byte[] { 0, 1, 2, 3 })
            foreach (sbyte kind in new sbyte[] { -2, 0, 1, 2, 3, 4 })
            {
                var set = new List<(int, byte)>();
                set.AddRange(Word(FieldGrid.Columns, 3));
                set.AddRange(Word(FieldGrid.Rows, 3));
                set.AddRange(Word(0x451E, 0x70));
                set.AddRange(Word(0x4528, 0x50));
                int mask = r.Next(16);
                for (int x = 0; x < 3; x++)
                    for (int y = 0; y < 3; y++)
                    {
                        int c = x * 6 + y, nb = (x, y) switch { (0, 1) => 1, (2, 1) => 2, (1, 0) => 4, (1, 2) => 8, _ => 0 };
                        set.Add((0x45DA + c, (nb & mask) != 0 || (x, y) == (1, 1) ? terrain : (byte)((terrain + 1) & 3)));
                        set.Add((FieldGrid.Owner + c, (byte)(kind == -2 ? 0xFF : r.Next(4))));
                        set.Add((FieldGrid.Kind + c, (byte)kind));
                    }
                H.AssertReadOnly(new H.Case($"1540(1,1) terrain {terrain} kind {kind} neighbours {mask}", [.. set],
                    o => o.Call(Seg, 0x1540, 1, 1), (_, s) => s.Tile(1, 1)));
            }
    }

    // [DIFFERENTIAL] 0609:13C5
    [GameDataFact]
    public void Border_matches_the_original()
    {
        var args = new List<(short X, short Y, short W, short H)> { (0x20, 0x40, 0x120, 0x84), (0x60, 0x50, 0x20, 0x16), (0x70, 0x50, 0, 0x16),
            (0x70, 0x50, 0x1F, 0x16), (0x50, 0x48, 0x41, 0x2C), (0x30, 0x40, -0x20, 0x16) };
        var r = new Random(0x13C5);
        for (int i = 0; i < 12; i++)
        {
            int cols = r.Next(1, 10), rows = r.Next(1, 7);
            args.Add(((short)((0x140 - (cols << 5)) / 2), (short)(0x40 + r.Next(0, 0x20)), (short)(cols << 5), (short)(rows * 0x16)));
        }
        // the core draws the random(1) results (Planting.Border), the scene draws the pieces they pick
        foreach (var (x, y, w, h) in args)
        {
            OriginalRandom? random = null;
            var c = new H.Case($"13C5({x},{y},{w},{h})", [], o => o.Call(Seg, 0x13C5, W(x), W(y), W(w), W(h)), (d, s) =>
            {
                random = new OriginalRandom(new RecordedClock([]), d.U32(DgroupLayout.RandomSeed));
                Planting.Border(x, y, w, h, random, new BoardOutput(s));
            });
            var (ods, _) = H.AssertSame(c);
            Assert.Equal(new Dgroup(ods[..DgroupLayout.Size]).U32(DgroupLayout.RandomSeed), random!.Seed);
        }
    }

    /// <summary>Selection state of player p: kind, stocks, field counts, cost, yield, rockets, land price.</summary>
    private static (int, byte)[] SelectionState(Random r, int p, sbyte kind)
    {
        var set = new List<(int, byte)>(Player(p)) { (0x2E94 + p, (byte)kind) };
        short yield = r.Next(6) == 0 ? (short)-r.Next(1, 20) : (short)r.Next(1, 40);
        set.Add((0x2F26, (byte)yield));
        set.Add((0x48A2, (byte)(r.Next(5) == 0 ? -r.Next(1, 30) : r.Next(0, 60))));
        for (int g = 0; g < 4; g++)
        {
            int stock = r.Next(4) switch { 0 => 0, 1 => r.Next(0, 40), 2 => r.Next(0, 2000), _ => r.Next(-2000, 0) };
            set.AddRange(Word(0x3CF4 + p * 8 + 2 * g, stock));
            set.Add((0x2EE7 + p * 4 + g, (byte)r.Next(0, 12)));
        }
        set.AddRange(Word(0x4BAC + 2 * p, r.Next(5) == 0 ? r.Next(-3, 1) : r.Next(0, 11)));
        set.AddRange(Word(0x3746, r.Next(3) == 0 ? r.Next(-40000, 0) : r.Next(0, 10000)));
        return [.. set];
    }

    // [DIFFERENTIAL] 0609:0C73
    // [DIFFERENTIAL] 0609:0D02
    // [DIFFERENTIAL] 0609:0DF9
    // [DIFFERENTIAL] 0609:18E5
    // [DIFFERENTIAL] 0609:1912
    // [DIFFERENTIAL] 0609:193C
    // [DIFFERENTIAL] 0609:1978
    [GameDataFact]
    public void Selection_display_matches_the_original_for_every_kind()
    {
        var r = new Random(0x0C73);
        bool drawn = false;
        for (int n = 0; n < 96; n++)
        {
            int p = n % 4;
            sbyte kind = (sbyte)((n / 4) % 6 - 1);
            var c = new H.Case($"0C73 p {p} kind {kind} case {n}", SelectionState(r, p, kind),
                o => o.Call(Seg, 0x0C73), (_, s) => s.SelectionDisplay());
            if (!drawn) drawn = H.Draws(c);
            H.AssertReadOnly(c);
        }
        Assert.True(drawn, "0C73 draws nothing");
    }

    // [DIFFERENTIAL] 0609:0DF9
    [GameDataFact]
    public void Stock_bar_matches_the_original_at_its_limits()
    {
        var r = new Random(0x0DF9);
        foreach (short unit in new short[] { 1, 2, 7, 20, -1, -5, 127, -128 })
            foreach (short stock in new short[] { 0, 1, 30, 31, 32, 0x7FFF, -1, -0x7FFF })
            {
                short need = (short)r.Next(-40, 80), extra = (short)r.Next(-10, 40);
                if (r.Next(3) == 0) need = stock;
                int p = r.Next(4);
                H.AssertReadOnly(new H.Case($"0DF9({stock},{need},{extra}) unit {unit} p {p}",
                    [.. Player(p), (0x2F26, (byte)unit)],
                    o => o.Call(Seg, 0x0DF9, W(stock), W(need), W(extra), 0x1C), (_, s) => s.StockBar(stock, need, extra, 0x1C)));
            }
    }

    // [DIFFERENTIAL] 0609:1A03
    // [DIFFERENTIAL] 0609:1A61
    // [DIFFERENTIAL] 0609:1A97
    // [DIFFERENTIAL] 0609:1AC0
    [GameDataFact]
    public void Panel_purchase_marker_and_time_bar_match_the_original()
    {
        var r = new Random(0x1A03);
        for (int n = 0; n < 32; n++)
        {
            int p = n % 4;
            var b = RandomBoard(r);
            sbyte cx = (sbyte)r.Next(b.Columns), cy = (sbyte)r.Next(b.Rows);
            var set = b.Set.Concat(Player(p)).Concat([(0x304E + p, (byte)cx), (0x305A + p, (byte)cy), (0x43FA + p, (byte)r.Next(3))]).ToArray();
            H.AssertReadOnly(new H.Case($"1A03 p {p} cursor ({cx},{cy}) case {n}", set, o => o.Call(Seg, 0x1A03), (_, s) => s.Panel()));
            H.AssertReadOnly(new H.Case($"1A97 p {p}", set, o => o.Call(Seg, 0x1A97), (_, s) => s.PurchaseUsed()));
            byte limit = (byte)r.Next(3);
            short left = (short)r.Next(0, 0x29);
            var timed = set.Concat([(0x45BA, limit)]).Concat(Word(0x4520 + 2 * p, left)).ToArray();
            H.AssertReadOnly(new H.Case($"1AC0 p {p} limit {limit} left {left}", timed, o => o.Call(Seg, 0x1AC0), (_, s) => s.TimeBar()));
        }
    }

    // [DIFFERENTIAL] 0609:0204
    [GameDataFact]
    public void Status_lines_match_the_original()
    {
        // 21FD:060D at the end of 0204 is replaced by retf in the original: the harness executes the list once
        void Status(Traders.RefEmu.OriginalProgram o)
        {
            o.Machine.Mem.Write8(o.Segment(0x21FD), 0x060D, 0xCB);
            o.Call(Seg, 0x0204);
        }
        var r = new Random(0x0204);
        foreach (sbyte cost in new sbyte[] { 0, 5, 12, 99, -1, -128, 127 })
        {
            sbyte yield = (sbyte)r.Next(-128, 128);
            H.AssertReadOnly(new H.Case($"0204 cost {cost} yield {yield}", [(0x1582, 0), (0x48A2, (byte)cost), (0x2F26, (byte)yield)],
                Status, (_, s) => s.Status()));
        }
        Assert.True(H.Draws(new H.Case("0204 draws", [(0x1582, 0)], Status, (_, _) => { })));
        // second round: the typed notice runs its own 060D calls (replaced as well), then the list holds only 0609:1F04; its
        // unused entry bytes still hold the notice's entries, which the scene does not draw ([APPROX]), so only the pixels count
        H.AssertReadOnly(new H.Case("0204 second round", [(0x1582, 1)], Status, (_, s) => s.Status()), compareList: false);
    }

    /// <summary>Plays the core's list and page requests on the layers (ClearList, RunList, ShowPage) and the drawings on the scene.</summary>
    private sealed class BoardOutput(FieldScene scene) : IFieldOutput
    {
        public void ComputerTurn(Dgroup d, short x, short y) => throw new InvalidOperationException("no computer turn expected");

        public void Emit(FieldEvent e)
        {
            var gfx = scene.Layers;
            switch (e.Kind)
            {
                case FieldEventKind.ClearList: gfx.ClearList(); break;
                case FieldEventKind.RunList: gfx.ExecuteList(); break;
                case FieldEventKind.ShowPage: gfx.CopyGamePage(gfx.Ds[0x082E]); break;
                default: scene.Apply(e); break;
            }
        }
    }

    /// <summary>
    /// 0609:1BE5 end to end: the original's board (background, frame, border with its random draws, every field in its own
    /// list, page copy) against <see cref="Planting.Board"/> with the scene playing its events.
    /// </summary>
    // [DIFFERENTIAL] 0609:1BE5
    [GameDataFact]
    public void Board_draws_like_the_original()
    {
        var r = new Random(0x1BE5);
        for (int n = 0; n < 6; n++)
        {
            var b = RandomBoard(r);
            OriginalRandom? random = null;
            var c = new H.Case($"1BE5 board {n} ({b.Columns}×{b.Rows})", b.Set, o => o.Call(Seg, 0x1BE5), (d, s) =>
            {
                random = new OriginalRandom(new RecordedClock([]), d.U32(DgroupLayout.RandomSeed));
                Planting.Board(d, random, new BoardOutput(s));
            });
            var (ods, core) = H.AssertSame(c);
            var od = new Dgroup(ods[..DgroupLayout.Size]);
            Assert.Equal(od.U32(DgroupLayout.RandomSeed), random!.Seed);
            Assert.Equal(ods[DgroupLayout.LoopX], core[DgroupLayout.LoopX]);
            Assert.Equal(ods[DgroupLayout.LoopY], core[DgroupLayout.LoopY]);
        }
    }
}
