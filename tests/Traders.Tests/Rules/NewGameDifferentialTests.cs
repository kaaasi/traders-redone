using Traders.Core.Rules;
using Traders.Core.State;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Tests.Rules;

/// <summary>
/// Differential tests of the new game, segment 0E31 (round 4, track "partie"; docs/routines/0E31_0003_neue_partie.md).
/// Every case runs the original routine in the RefEmu and the port on the same data segment and compares the whole data
/// segment, DS:10D2 and the number of random draws. All fields the routines write are filled with garbage beforehand, so a
/// missing write shows up as a difference.
/// </summary>
public class NewGameDifferentialTests
{
    private static readonly RulesHarness Harness = new();

    /// <summary>Option fields per level as copied from the level table DS:0B90 (docs/analyse/DATENLAYOUT.md §2).</summary>
    private static readonly (short Iq, byte Oil, byte Robots, byte Gems, byte Catastrophes, short Pirates)[] Levels =
    [
        (0, 0, 0, 0, 0, 0),
        (1, 1, 0, 0, 1, 1),
        (2, 1, 0, 1, 1, 2),
        (2, 1, 1, 1, 1, 3),
    ];

    private static readonly (int Offset, int Length)[] Written =
    [
        (L.FieldQuality, 54), (L.FieldOwner, 54), (L.FieldKind, 54), (L.FieldTotal, 1),
        (L.StationVisited, 12), (L.StationValue, 24), (L.StationKind, 24), (L.Station, 8), (L.Weather, 1),
        (L.HqX, 4), (L.Rockets, 8), (L.FieldCount, 4), (L.Oil, 8), (L.Cash, 16), (L.FatMikeFlag, 4), (L.GemCredit, 8),
        (L.Gems, 4), (L.FieldsByKind, 16), (L.Stock, 32), (L.RobotParts, 24), (L.Robot376C, 16), (L.Robot2EF8, 8),
        (L.Robot377C, 4), (L.Robot40EC, 8), (L.Robot33DC, 8), (L.Robot3016, 8), (L.BaseUnit, 4), (L.GemPriceFactor, 4),
        (L.WarePrice, 8), (L.Week, 2), (L.Unknown4580, 1), (L.Unknown40F6, 8), (L.LoopX, 1), (L.LoopY, 1),
    ];

    private static Dgroup Input(Inputs r, int level, byte landType, int columns, int rows, short? cash = null,
        bool[]? computer = null)
    {
        var d = Harness.Base(r);
        foreach (var (offset, length) in Written)
            for (int i = 0; i < length; i++) d.Set(offset + i, (byte)r.Next());

        var o = Levels[level];
        d.Set(L.Level, (short)level);
        d.Set(L.ComputerIq, o.Iq);
        d.Set(L.OilConsumption, o.Oil);
        d.Set(L.RobotBattle, o.Robots);
        d.Set(L.GemTrading, o.Gems);
        d.Set(L.Catastrophes, o.Catastrophes);
        d.Set(L.PiratePresence, o.Pirates);
        d.Set(L.LandType, landType);
        d.Set(L.FieldColumns, (short)columns);
        d.Set(L.FieldRows, (short)rows);
        d.Set(L.InitialCash, cash ?? (short)(500 * r.Range(1, 4)));
        for (int p = 0; p < 4; p++)
        {
            bool isComputer = computer?[p] ?? r.Chance(50);
            d.Set(L.IsComputer + p, (byte)(isComputer ? 1 : 0));
            d.Set(L.ControlDevice + p, (sbyte)(isComputer ? 0 : r.Range(1, 5)));
        }
        return d;
    }

    private static string Label(int level, byte land, int columns, int rows, int seed) =>
        $"level {level} land {land} field {columns}x{rows} seed {seed}";

    // ------------------------------------------------------------------ 0E31:0003 whole start state

    // [DIFFERENTIAL] 0E31:0003
    [GameDataFact]
    public void Start_matches_for_every_level_land_type_and_player_mix()
    {
        var r = new Inputs(0x0003);
        for (int level = 0; level < 4; level++)
            foreach (byte land in new byte[] { 0, 1 })
                for (int s = 0; s < 6; s++)
                {
                    int columns = r.Range(4, 9), rows = r.Range(3, 6);
                    var d = Input(r, level, land, columns, rows);
                    if (s == 0) d.Set(L.RandomSeed, 1u);
                    var (ported, draws) = Harness.AssertSameWithRandom(d, 0x0E31, 0x0003, NewGame.Start,
                        Label(level, land, columns, rows, s));
                    // The last three draws are always weather (2, 4) and the gem price factor (120).
                    Assert.Equal(new short[] { 2, 4, 120 }, draws.TakeLast(3));
                    Assert.Equal(4, ported.S8(L.ActivePlayer));
                    Assert.Equal(4, ported.S16(L.LoopWare));
                }
    }

    // [DIFFERENTIAL] 0E31:0003
    [GameDataFact]
    public void Start_matches_for_every_field_size()
    {
        var r = new Inputs(0x01CD);
        for (int columns = 4; columns <= 9; columns++)
            for (int rows = 3; rows <= 6; rows++)
                foreach (byte land in new byte[] { 0, 1 })
                {
                    var d = Input(r, r.Range(0, 3), land, columns, rows);
                    var (ported, _) = Harness.AssertSameWithRandom(d, 0x0E31, 0x0003, NewGame.Start,
                        Label(-1, land, columns, rows, 0));
                    int n = columns * rows / 4 - 1;
                    var counts = new int[4];
                    for (int x = 0; x < columns; x++)
                        for (int y = 0; y < rows; y++) counts[ported.S8(L.FieldQuality + L.FieldIndex(x, y))]++;
                    Assert.Equal(new[] { n, n, n, columns * rows - 3 * n }, counts);
                }
    }

    // ------------------------------------------------------------------ parts

    // [DIFFERENTIAL] 0E31:023D
    [GameDataFact]
    public void Stations_match_for_many_seeds()
    {
        var r = new Inputs(0x023D);
        var values = new HashSet<short>();
        for (int s = 0; s < 300; s++)
        {
            var d = Input(r, 2, 0, 8, 6);
            if (s < 3) d.Set(L.RandomSeed, new uint[] { 1, 0, uint.MaxValue }[s]);
            var (ported, draws) = Harness.AssertSameWithRandom(d, 0x0E31, 0x023D, NewGame.CreateStations, $"seed {s}");
            Assert.All(draws, n => Assert.True(n is 2 or 3));
            var kinds = Enumerable.Range(0, 12).Select(i => ported.S16(L.StationKind + 2 * i)).ToList();
            Assert.Equal(new[] { 6, 4, 2 }, new[] { 0, 1, 2 }.Select(k => kinds.Count(x => x == k)));
            for (int i = 0; i < 12; i++) values.Add(ported.S16(L.StationValue + 2 * i));
            Assert.Equal(4, ported.S8(L.ActivePlayer));
        }
        Assert.Equal(new short[] { -1, 0, 1, 2, 3, 10 }, values.Order());
    }

    // [DIFFERENTIAL] 0E31:0379
    [GameDataFact]
    public void Random_land_matches()
    {
        var r = new Inputs(0x0379);
        for (int s = 0; s < 60; s++)
        {
            int columns = r.Range(4, 9), rows = r.Range(3, 6);
            var d = Input(r, 0, 0, columns, rows);
            var (_, draws) = Harness.AssertSameWithRandom(d, 0x0E31, 0x0379, NewGameLand.Random,
                Label(0, 0, columns, rows, s));
            Assert.All(draws.Where((_, i) => i % 2 == 0), n => Assert.Equal(columns - 1, n));
            Assert.All(draws.Where((_, i) => i % 2 == 1), n => Assert.Equal(rows - 1, n));
        }
    }

    // [DIFFERENTIAL] 0E31:0465
    [GameDataFact]
    public void Real_land_matches()
    {
        var r = new Inputs(0x0465);
        for (int s = 0; s < 60; s++)
        {
            int columns = r.Range(4, 9), rows = r.Range(3, 6);
            var d = Input(r, 0, 1, columns, rows);
            var (_, draws) = Harness.AssertSameWithRandom(d, 0x0E31, 0x0465, NewGameLand.Real,
                Label(0, 1, columns, rows, s));
            // After the seeds every attempt starts with random(1); radii stay within 1..max(cols, rows) - 1.
            Assert.All(draws, n => Assert.InRange(n, (short)0, (short)(Math.Max(columns, rows) - 1)));
        }
    }

    // [DIFFERENTIAL] 0E31:0361
    [GameDataFact]
    public void Land_dispatch_matches_for_any_land_type()
    {
        var r = new Inputs(0x0361);
        foreach (byte land in new byte[] { 0, 1, 2, 0x7F, 0x80, 0xFF })
            for (int s = 0; s < 4; s++)
            {
                int columns = r.Range(4, 9), rows = r.Range(3, 6);
                var d = Input(r, 1, land, columns, rows);
                Harness.AssertSameWithRandom(d, 0x0E31, 0x0361, NewGameLand.Create, Label(1, land, columns, rows, s));
            }
    }

    // [DIFFERENTIAL] 0E31:01CD
    [GameDataFact]
    public void Field_matches()
    {
        var r = new Inputs(0x01CD);
        foreach (byte land in new byte[] { 0, 1 })
            for (int s = 0; s < 10; s++)
            {
                int columns = r.Range(4, 9), rows = r.Range(3, 6);
                var d = Input(r, 3, land, columns, rows);
                var (ported, _) = Harness.AssertSameWithRandom(d, 0x0E31, 0x01CD, NewGame.CreateField,
                    Label(3, land, columns, rows, s));
                Assert.Equal(columns, ported.S8(L.LoopX));
                Assert.Equal(rows, ported.S8(L.LoopY));
            }
    }

    // [DIFFERENTIAL] 0E31:0149
    [GameDataFact]
    public void Robot_values_match_for_every_player()
    {
        var r = new Inputs(0x0149);
        for (sbyte p = 0; p < 4; p++)
        {
            var d = Input(r, 3, 0, 8, 6);
            d.Set(L.ActivePlayer, p);
            Harness.AssertSame(d, 0x0E31, 0x0149, NewGame.ResetRobots, $"player {p}");
        }
    }

    // ------------------------------------------------------------------ edge cases (part 3)

    // [DIFFERENTIAL] 0E31:0003
    [GameDataFact]
    public void Start_edge_cases()
    {
        var r = new Inputs(0x0E31);
        var cases = new (int Level, byte Land, int Columns, int Rows, short Cash, bool[] Computer, string Label)[]
        {
            (0, 0, 4, 3, 500, [true, true, true, true], "all computer, smallest field, lowest cash"),
            (3, 1, 9, 6, 2000, [true, true, true, true], "all computer, largest field, highest cash"),
            (3, 0, 9, 6, 2000, [false, false, false, false], "all human, random land"),
            (1, 1, 4, 6, 1000, [false, true, false, true], "narrow and high, real land"),
            (2, 1, 9, 3, 1500, [true, false, true, false], "wide and flat, real land"),
            (2, 1, 6, 6, 500, [false, true, true, true], "square, real land (radius limit from rows)"),
            (2, 0, 8, 6, 39, [false, true, true, true], "cash below 40: base unit 0"),
            (2, 0, 8, 6, -41, [false, true, true, true], "negative cash: sign-extended, base unit -1"),
            (2, 1, 8, 6, short.MinValue, [false, true, true, true], "lowest cash"),
            (2, 1, 8, 6, short.MaxValue, [false, true, true, true], "highest cash"),
        };
        foreach (var c in cases)
            for (int s = 0; s < 3; s++)
            {
                var d = Input(r, c.Level, c.Land, c.Columns, c.Rows, c.Cash, c.Computer);
                var (ported, _) = Harness.AssertSameWithRandom(d, 0x0E31, 0x0003, NewGame.Start, $"{c.Label}, seed {s}");
                Assert.Equal((int)c.Cash, ported.S32(L.Cash + 12));
                Assert.Equal(c.Cash / 40, ported.S32(L.BaseUnit));
            }
    }

    /// <summary>
    /// Counter-check: <see cref="NewGame.Start"/> gives the original land, while the same draws with the stations before the
    /// land give another one; so a swap of the two in <see cref="NewGame.Start"/> turns this test red (Abnahme "eine Ziehung
    /// vertauscht -> rot", #99 R4P-R3).
    /// </summary>
    [GameDataFact]
    public void Swapped_draw_order_is_detected()
    {
        var r = new Inputs(0x5A5A);
        var d = Input(r, 2, 1, 8, 6);
        var original = Harness.RunOriginal(d, 0x0E31, 0x0003);
        var started = new Dgroup((byte[])d.Bytes.Clone());
        NewGame.Start(started, new Traders.Core.OriginalRandom(new Traders.Core.RecordedClock([]), started.U32(L.RandomSeed)));
        Assert.Equal(original.DataSegment[L.FieldQuality..(L.FieldQuality + 54)], started.Bytes[L.FieldQuality..(L.FieldQuality + 54)]);
        var swapped = new Dgroup((byte[])d.Bytes.Clone());
        var random = new Traders.Core.OriginalRandom(new Traders.Core.RecordedClock([]), swapped.U32(L.RandomSeed));
        NewGame.CreateStations(swapped, random);
        NewGame.CreateField(swapped, random);
        Assert.NotEqual(original.DataSegment[L.FieldQuality..(L.FieldQuality + 54)], swapped.Bytes[L.FieldQuality..(L.FieldQuality + 54)]);
    }
}
