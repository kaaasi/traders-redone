using System.Runtime.CompilerServices;
using Traders.Core;
using Traders.Core.FatMike;
using Traders.Core.Field;
using Traders.Core.State;
using Traders.Core.Week;
using Traders.RefEmu;
using Traders.RefEmu.Cpu;
using C = Traders.Core.Field.FieldCursor;
using F = Traders.Core.Field.FieldGrid;
using L = Traders.Core.State.DgroupLayout;
using M = Traders.Core.FatMike.FatMikePhase;
using P = Traders.Core.Field.Planting;

namespace Traders.Tests.FatMike;

/// <summary>
/// Differential tests of Fat Mike 1F63:000F and its routines (finding docs/routines/1F63_000F_fatmike.md) against the original
/// in the RefEmu (<see cref="FatMikeHarness"/>): the same state on both sides, then the routine – compared are the whole data
/// segment (outside <see cref="FatMikeHarness.OutputOwned"/>), the random state, the number of random() calls and the sequence
/// of recorded calls (drawing, sound, controls, pause, wait loop with their arguments).
/// </summary>
public class FatMikeDifferentialTests
{
    private const ushort Seg = FatMikeHarness.Seg;
    private const int Stock3 = L.Stock + 6;

    private static ushort W(int v) => unchecked((ushort)v);

    /// <summary>A random state of the routines' inputs; with <paramref name="keepGame"/> the game's own values mostly stay.</summary>
    internal static void Arrange(FatMikeHarness h, Random r, bool keepGame = false)
    {
        var p = h.Program;
        if (!keepGame) p.Machine.Mem.WriteBlock(Traders.RefEmu.Cpu.Memory.Linear(p.Dgroup, 0), h.Initial);
        void B(int o, int v) => p.WriteByte((ushort)o, unchecked((byte)v));
        void Wd(int o, int v) => p.WriteWord((ushort)o, unchecked((ushort)v));
        B(L.ActivePlayer, r.Next(4));
        for (int i = 0; i < 4; i++)
        {
            if (!keepGame || r.Next(3) == 0) B(L.IsComputer + i, r.Next(3) == 0 ? 1 : r.Next(8) == 0 ? r.Next(256) : 0);
            if (!keepGame || r.Next(2) == 0)
                Wd(Stock3 + 8 * i, r.Next(8) == 0 ? r.Next(32700, 32768) : r.Next(8) == 0 ? r.Next(-40, 65536) : r.Next(0, 1000));
            B(L.FatMikeFlag + i, r.Next(3) == 0 ? 1 : r.Next(10) == 0 ? r.Next(256) : 0);
            Wd(M.Paid + 2 * i, r.Next(3) == 0 ? 1 : r.Next(10) == 0 ? r.Next(65536) : 0);
            B(M.Verdict + i, r.Next(3));
            B(C.Action + i, r.Next(3) == 0 ? 0x0A : r.Next(4) == 0 ? -1 : r.Next(8) == 0 ? r.Next(256) : r.Next(4));
            B(M.Bidders + i, r.Next(256));
            if (!keepGame)
            {
                B(F.OwnedFields + i, r.Next(10) == 0 ? r.Next(256) : r.Next(0, 20));
                for (int k = 0; k < 4; k++) B(F.OwnedKinds + 4 * i + k, r.Next(10) == 0 ? r.Next(256) : r.Next(0, 6));
            }
        }
        if (!keepGame || r.Next(2) == 0) Wd(L.Week, r.Next(8) == 0 ? r.Next(-3, 65536) : r.Next(0, 12));
        if (!keepGame) B(F.FreeFields, r.Next(6) == 0 ? r.Next(256) : r.Next(0, 8));
        if (!keepGame) Wd(L.ComputerIq, r.Next(4));
        B(M.Contest, r.Next(4) == 0 ? 1 : 0);
        Wd(M.Unit, r.Next(6) == 0 ? r.Next(1, 400) : r.Next(1, 10));
        Wd(M.Base, r.Next(0, 31));
        Wd(M.Price, r.Next(8) == 0 ? r.Next(-50, 65536) : r.Next(10, 200));
        Wd(M.Lot, r.Next(8) == 0 ? r.Next(-2, 5) : r.Next(0, 3));
        Wd(M.Cheapest, r.Next(0, 3));
        for (int i = 0; i < 3; i++) Wd(M.Lots + 2 * i, r.Next(10, 300));
        B(M.NobodyPaid, r.Next(4) == 0 ? 1 : 0);
        Wd(M.Slide, r.Next(8) == 0 ? r.Next(-300, 600) : r.Next(0x94, 0x101));
        B(P.Players, r.Next(0, 5));
        B(WaitLoopLayout.IndicatorCountdown, r.Next(4) == 0 ? r.Next(-3, 9) : 0);
        Wd(L.LoopWare, r.Next(65536));
        if (!keepGame) Wd(P.CardMode, r.Next(6) == 0 ? new[] { 2, 0x63, 0 }[r.Next(3)] : 1);
        p.WriteDword(OriginalProgram.RandomStateOffset, (uint)r.Next() * 2654435761u);
        if (!keepGame || r.Next(2) == 0) h.WriteTexts(r);
    }

    /// <summary>
    /// A consistent field (columns 4…9, rows 3…6): owners −1…3 with at most one HQ (kind 4) per player, kinds −2 or 0…3, and the
    /// counters DS:2F05, DS:4BA8, DS:2EE7 as 0609:10D2 keeps them; the verdicts fit the counts and DS:54C8 (no endless draw in 08DA).
    /// </summary>
    internal static void ArrangeField(FatMikeHarness h, Random r)
    {
        var p = h.Program;
        int cols = r.Next(4, 10), rows = r.Next(3, 7);
        p.WriteWord(F.Columns, (ushort)cols);
        p.WriteWord(F.Rows, (ushort)rows);
        var fields = new int[4];
        var kinds = new int[4, 4];
        int free = 0;
        var hq = new bool[4];
        for (int x = 0; x < 9; x++)
            for (int y = 0; y < 6; y++)
            {
                int cell = x * 6 + y;
                int owner = -1, kind = -2;
                if (x < cols && y < rows)
                {
                    owner = r.Next(3) == 0 ? -1 : r.Next(4);
                    if (owner >= 0 && !hq[owner] && r.Next(4) == 0) { kind = 4; hq[owner] = true; }
                    else kind = r.Next(3) == 0 ? -2 : r.Next(4);
                    if (owner == -1) { free++; kind = r.Next(4) == 0 ? r.Next(4) : -2; }
                    else
                    {
                        fields[owner]++;
                        if (kind is >= 0 and <= 3) kinds[owner, kind]++;
                    }
                }
                p.WriteByte((ushort)(F.Owner + cell), unchecked((byte)owner));
                p.WriteByte((ushort)(F.Kind + cell), unchecked((byte)kind));
            }
        p.WriteByte(F.FreeFields, (byte)free);
        for (int i = 0; i < 4; i++)
        {
            p.WriteByte((ushort)(F.OwnedFields + i), (byte)fields[i]);
            for (int k = 0; k < 4; k++) p.WriteByte((ushort)(F.OwnedKinds + 4 * i + k), (byte)kinds[i, k]);
            // nobody paid: 0711 gives every player verdict 2 (a 0 or 1 there would make 08DA draw for crops it may not find)
            if (p.ReadByte(M.NobodyPaid) == 1) p.WriteByte((ushort)(M.Verdict + i), 2);
        }
    }

    /// <summary>Runs the port on a copy of the RefEmu's state, then the original; compares everything.</summary>
    internal static (ushort Ax, ushort Dx) AssertSame(FatMikeHarness h, string label, ushort off,
        Action<Dgroup, OriginalRandom, FatMikeRecorder> port, params ushort[] args)
    {
        var d = new Dgroup(h.Program.DataSegment());
        var random = new OriginalRandom(new RecordedClock([]), d.U32(L.RandomSeed));
        var rec = new FatMikeRecorder(h);
        port(d, random, rec);
        d.Set(L.RandomSeed, random.Seed);
        h.Calls.Clear();
        h.RandomArgs.Clear();
        h.ControlReads = 0;
        h.Passes = 0;
        var result = h.Program.Call(Seg, off, args);
        FatMikeHarness.Compare(label, h.Program.DataSegment(), d);
        Assert.True(h.Calls.SequenceEqual(rec.Calls),
            $"{label}: calls original [{string.Join("; ", h.Calls)}], port [{string.Join("; ", rec.Calls)}]");
        Assert.True(h.RandomArgs.Count == random.Calls, $"{label}: original drew {h.RandomArgs.Count} times, port {random.Calls}");
        return result;
    }

    internal static void Drain(IEnumerable<FieldWait> frames)
    {
        foreach (var _ in frames) { }
    }

    /// <summary>Control actions per read: humans fire now and then (more often with <paramref name="eager"/>), else move or nothing.</summary>
    internal static Func<int, sbyte[]> Script(int seed, bool eager)
    {
        var r = new Random(seed);
        var cache = new List<sbyte[]>();
        return n =>
        {
            while (cache.Count <= n)
                cache.Add([.. Enumerable.Range(0, 4).Select(_ => (sbyte)(r.Next(eager ? 25 : 120) == 0 ? 0x0A : r.Next(3) == 0 ? -1 : r.Next(4)))]);
            return cache[n];
        };
    }

    // [DIFFERENTIAL] 1F63:0390
    [GameDataFact]
    public void Prices_match_the_original()
    {
        var h = new FatMikeHarness();
        var r = new Random(0x0390);
        var kinds = new HashSet<short>();
        for (int n = 0; n < 300; n++)
        {
            Arrange(h, r);
            AssertSame(h, $"0390 case {n}", 0x0390, (d, random, _) => M.Prices(d, random));
            for (int i = 0; i < 3; i++) kinds.Add((short)h.Program.ReadWord((ushort)(M.Lots + 2 * i)));
        }
        Assert.Contains((short)999, kinds);
        Assert.Contains((short)10, kinds);
    }

    // [DIFFERENTIAL] 1F63:0C82
    [GameDataFact]
    public void Largest_stock_matches_the_original()
    {
        var h = new FatMikeHarness();
        var r = new Random(0x0C82);
        for (int n = 0; n < 200; n++)
        {
            Arrange(h, r);
            if (r.Next(3) == 0)
                for (int i = 0; i < 4; i++) h.Program.WriteWord((ushort)(Stock3 + 8 * i), (ushort)(r.Next(0, 3) * 100));
            short port = 0;
            var (ax, _) = AssertSame(h, $"0C82 case {n}", 0x0C82, (d, _, _) => port = M.MaxStock(d));
            Assert.Equal(port, (short)ax);
        }
    }

    // [DIFFERENTIAL] 1F63:01B2
    // [DIFFERENTIAL] 1F63:0E5C
    // [DIFFERENTIAL] 1F63:0EC7
    // [DIFFERENTIAL] 1F63:0F46
    [GameDataFact]
    public void Scale_matches_the_original()
    {
        var h = new FatMikeHarness();
        var r = new Random(0x01B2);
        for (int n = 0; n < 60; n++)
        {
            Arrange(h, r);
            for (int i = 0; i < 4; i++)
                if (r.Next(2) == 0) h.Program.WriteWord((ushort)(Stock3 + 8 * i), (ushort)r.Next(0, 1000));
            var max = new StrongBox<short>();
            var (ax, _) = AssertSame(h, $"01B2 case {n}", 0x01B2, (d, _, rec) => Drain(M.ScaleFrames(d, rec, max)));
            Assert.Equal(max.Value, (short)ax);
        }
    }

    // [DIFFERENTIAL] 1F63:064A
    [GameDataFact]
    public void Computer_choice_matches_the_original()
    {
        var h = new FatMikeHarness();
        var r = new Random(0x064A);
        int fire = 0, none = 0;
        for (int n = 0; n < 400; n++)
        {
            Arrange(h, r);
            AssertSame(h, $"064A case {n}", 0x064A, (d, random, _) => M.Decide(d, random));
            if (h.Program.ReadByte((ushort)(C.Action + h.Program.ReadByte(L.ActivePlayer))) == 0x0A) fire++;
            else none++;
        }
        Assert.True(fire > 50 && none > 50, $"fire {fire}, none {none}");
    }

    // [DIFFERENTIAL] 1F63:0711
    // [DIFFERENTIAL] 1F63:0A8A
    [GameDataFact]
    public void Verdicts_match_the_original()
    {
        var h = new FatMikeHarness();
        var r = new Random(0x0711);
        var seen = new HashSet<byte>();
        for (int n = 0; n < 300; n++)
        {
            Arrange(h, r);
            AssertSame(h, $"0711 case {n}", 0x0711, (d, _, _) => M.Verdicts(d));
            for (int i = 0; i < 4; i++) seen.Add(h.Program.ReadByte((ushort)(M.Verdict + i)));
            Arrange(h, r);
            short port = 0;
            var (ax, _) = AssertSame(h, $"0A8A case {n}", 0x0A8A, (d, _, _) => port = M.Count(d));
            Assert.Equal(port, (short)ax);
        }
        Assert.Equal(new byte[] { 0, 1, 2 }, seen.Order().ToArray());
    }

    // [DIFFERENTIAL] 1F63:04D8
    // [DIFFERENTIAL] 1F63:0E8F
    // [DIFFERENTIAL] 1F63:1185
    [GameDataFact]
    public void Offer_matches_the_original()
    {
        var h = new FatMikeHarness();
        var r = new Random(0x04D8);
        int paid = 0;
        for (int n = 0; n < 60; n++)
        {
            Arrange(h, r);
            h.Script = Script(r.Next(), r.Next(2) == 0);
            int before = h.Program.ReadByte(P.Players);
            AssertSame(h, $"04D8 case {n}", 0x04D8, (d, random, rec) =>
                Drain(M.OfferFrames(d, random, rec, new WaitLoop(d, random, Traders.Core.Input.KeyboardOnlyDevices.Instance, 4))));
            if (h.Program.ReadByte(P.Players) < before) paid++;
            h.Script = null;
        }
        Assert.True(paid > 10, $"paid in {paid} cases");
    }

    // [DIFFERENTIAL] 1F63:0305
    // [DIFFERENTIAL] 1F63:0D24
    // [DIFFERENTIAL] 1F63:0F84
    // [DIFFERENTIAL] 1F63:0FDF
    // [DIFFERENTIAL] 1F63:1247
    // [DIFFERENTIAL] 1F63:13FE
    // [DIFFERENTIAL] 1F63:143E
    // [DIFFERENTIAL] 1F63:147B
    [GameDataFact]
    public void Scene_parts_match_the_original()
    {
        var h = new FatMikeHarness();
        var r = new Random(0x0D24);
        for (int n = 0; n < 40; n++)
        {
            Arrange(h, r);
            AssertSame(h, $"0305 case {n}", 0x0305, (d, random, rec) => Drain(M.BlinkFrames(d, random, rec)));
            AssertSame(h, $"0D24 case {n}", 0x0D24, (d, _, rec) => Drain(M.SceneFrames(d, rec)));
            short k = (short)r.Next(-1, 4);
            AssertSame(h, $"0F84 case {n}", 0x0F84, (d, _, rec) => Drain(M.IndicatorFrames(d, rec, k)), W(k));
            AssertSame(h, $"1247 case {n}", 0x1247, (d, _, rec) => M.Objects(d, rec));
            AssertSame(h, $"13FE case {n}", 0x13FE, (d, _, rec) => Drain(M.ShowRowFrames(d, rec)));
            short price = (short)(r.Next(6) == 0 ? r.Next(-2000, 32768) : r.Next(0, 1000));
            AssertSame(h, $"147B case {n}", 0x147B, (d, _, rec) => Drain(M.SlideFrames(d, rec, price)), W(price));
        }
    }

    // [DIFFERENTIAL] 1F63:079A
    // [DIFFERENTIAL] 1F63:08DA
    [GameDataFact]
    public void Punishment_matches_the_original()
    {
        var h = new FatMikeHarness();
        var r = new Random(0x08DA);
        int lost = 0, destroyed = 0;
        for (int n = 0; n < 60; n++)
        {
            Arrange(h, r);
            ArrangeField(h, r);
            AssertSame(h, $"08DA case {n}", 0x08DA, (d, random, rec) => Drain(M.PunishPlayerFrames(d, random, rec)));
            Arrange(h, r);
            ArrangeField(h, r);
            AssertSame(h, $"079A case {n}", 0x079A, (d, random, rec) => Drain(M.PunishFrames(d, random, rec)));
            lost += h.Calls.Count(c => c == "Sound(0, 39)");
            destroyed += h.Calls.Count(c => c == "Sound(0, 38)");
        }
        Assert.True(lost > 5 && destroyed > 5, $"lost {lost}, destroyed {destroyed}");
    }

    /// <summary>
    /// A field with exactly one cell the punishment of player <paramref name="p"/> (crops destroyed, one field to hit) may hit
    /// (<paramref name="qualifying"/>: kind 0 of that player), or none at all (endless in the original). All other cells belong to
    /// other players or nobody, with consistent counters.
    /// </summary>
    private static void ArrangeSearch(FatMikeHarness h, int p, int cols, int rows, (int X, int Y)? qualifying)
    {
        var o = h.Program;
        o.WriteWord(F.Columns, (ushort)cols);
        o.WriteWord(F.Rows, (ushort)rows);
        o.WriteByte(L.ActivePlayer, (byte)p);
        o.WriteByte(M.NobodyPaid, 0);
        for (int i = 0; i < 4; i++)
        {
            o.WriteByte((ushort)(M.Verdict + i), 0);
            o.WriteByte((ushort)(F.OwnedFields + i), 0);
            for (int k = 0; k < 4; k++) o.WriteByte((ushort)(F.OwnedKinds + 4 * i + k), 0);
        }
        o.WriteByte((ushort)(M.Verdict + p), 1);
        int free = 0, other = (p + 1) % 4;
        for (int x = 0; x < 9; x++)
            for (int y = 0; y < 6; y++)
            {
                bool inside = x < cols && y < rows;
                bool mine = qualifying == (x, y);
                int owner = !inside ? -1 : mine ? p : (x + y) % 2 == 0 ? other : -1;
                int kind = mine ? 0 : -2;
                if (inside && owner == -1) free++;
                if (inside && owner == other) o.WriteByte((ushort)(F.OwnedFields + other), (byte)(o.ReadByte((ushort)(F.OwnedFields + other)) + 1));
                o.WriteByte((ushort)(F.Owner + x * 6 + y), unchecked((byte)owner));
                o.WriteByte((ushort)(F.Kind + x * 6 + y), unchecked((byte)kind));
            }
        o.WriteByte(F.FreeFields, (byte)free);
        if (qualifying != null)
        {
            o.WriteByte((ushort)(F.OwnedFields + p), 1);
            o.WriteByte((ushort)(F.OwnedKinds + 4 * p), 4); // count of 0A8A: (4 + 0 + 0 + 0) / 4 = 1 field to hit
        }
        else
            o.WriteByte((ushort)(F.OwnedKinds + 4 * p), 4); // count 1 although the player owns no field: 08DA never ends
    }

    /// <summary>First seed of DS:10D2 from <paramref name="from"/> on for which the 08DA search for the single cell needs at least <paramref name="minPairs"/> draws of (column, row).</summary>
    private static uint? SeedForLongSearch(int from, int cols, int rows, (int X, int Y) target, int minPairs)
    {
        for (uint seed = (uint)from; seed < (uint)from + 3000; seed++)
        {
            var rnd = new OriginalRandom(new RecordedClock([]), seed);
            int pairs = 0;
            while (true)
            {
                pairs++;
                int x = rnd.Next((short)(cols - 1)), y = rnd.Next((short)(rows - 1));
                if ((x, y) == target) break;
                if (pairs >= 20_000) break;
            }
            if (pairs >= minPairs && pairs < 20_000) return seed;
        }
        return null;
    }

    // [DIFFERENTIAL] 1F63:08DA – a search that needs hundreds of draws (one hittable field) ends in step with the original
    [GameDataFact]
    public void Punishment_with_a_long_search_matches_the_original()
    {
        var h = new FatMikeHarness();
        var r = new Random(0x236);
        long maxDraws = 0;
        for (int n = 0; n < 12; n++)
        {
            Arrange(h, r);
            int cols = r.Next(4, 10), rows = r.Next(3, 7), p = r.Next(4);
            var target = (X: r.Next(cols), Y: r.Next(rows));
            ArrangeSearch(h, p, cols, rows, target);
            if (SeedForLongSearch(r.Next(1, 1 << 20), cols, rows, target, 150) is not { } seed)
            {
                n--; // no seed found: power-of-two range divisors bound the search length from above, so no seed in the window may reach minPairs (#249)
                continue;
            }
            h.Program.WriteDword(OriginalProgram.RandomStateOffset, seed);
            AssertSame(h, $"08DA long search {n}", 0x08DA, (d, random, rec) => Drain(M.PunishPlayerFrames(d, random, rec)));
            maxDraws = Math.Max(maxDraws, h.RandomArgs.Count);
        }
        Assert.True(maxDraws >= 300, $"the longest search drew only {maxDraws} times");
    }

    /// <summary>
    /// 1F63:08DA when no field qualifies although the count is positive: the original draws on without end (finding
    /// 1F63_000F_fatmike.md, #236). The call is stopped by the instruction budget (#222); until then every draw is
    /// random(columns − 1) then random(rows − 1), the seed is that of the replayed draws, and field, counters and the rest of the data
    /// segment are unchanged. The port's loop (FatMikePhase.PunishPlayerFrames) has no yield or budget on this path and cannot be run
    /// bounded without a production hook; its draw order is the one the original shows here.
    /// </summary>
    // [DIFFERENTIAL] 1F63:08DA – endless draw without a hittable field, bounded by the instruction budget
    [GameDataFact]
    public void Punishment_without_a_hittable_field_draws_endlessly_like_the_original()
    {
        var h = new FatMikeHarness();
        var r = new Random(0x2360);
        for (int n = 0; n < 6; n++)
        {
            Arrange(h, r);
            int cols = r.Next(4, 10), rows = r.Next(3, 7), p = r.Next(4);
            ArrangeSearch(h, p, cols, rows, null);
            uint seed = (uint)r.Next(1, int.MaxValue);
            h.Program.WriteDword(OriginalProgram.RandomStateOffset, seed);
            var before = h.Program.DataSegment();
            h.Calls.Clear();
            h.RandomArgs.Clear();
            var e = Assert.Throws<EmulationException>(() => h.Program.CallWithBudget(300_000, Seg, 0x08DA));
            Assert.StartsWith(OriginalProgram.BudgetMessage, e.Message);
            Assert.True(h.RandomArgs.Count > 100, $"case {n}: only {h.RandomArgs.Count} draws within the budget");
            // the budget may stop the routine inside a draw: the seed then fits the draws counted (hook at the entry)
            var rnd = new OriginalRandom(new RecordedClock([]), seed);
            for (int i = 0; i < h.RandomArgs.Count; i++)
            {
                short expected = (short)(i % 2 == 0 ? cols - 1 : rows - 1);
                Assert.True(h.RandomArgs[i] == expected, $"case {n}: draw {i} was random({h.RandomArgs[i]}), expected random({expected})");
                rnd.Next(expected);
            }
            var after = h.Program.DataSegment();
            Assert.Equal(rnd.Seed, h.Program.ReadDword(OriginalProgram.RandomStateOffset));
            // the field and its counters are untouched
            Assert.True(before.AsSpan(F.Owner, 54).SequenceEqual(after.AsSpan(F.Owner, 54)), "owners changed");
            Assert.True(before.AsSpan(F.Kind, 54).SequenceEqual(after.AsSpan(F.Kind, 54)), "kinds changed");
            Assert.True(before.AsSpan(F.OwnedFields, 4).SequenceEqual(after.AsSpan(F.OwnedFields, 4)), "counters changed");
        }
    }

    // [DIFFERENTIAL] 1F63:000F
    [GameDataFact]
    public void Fat_Mike_matches_the_original()
    {
        var h = new FatMikeHarness();
        var r = new Random(0x000F);
        for (int n = 0; n < 12; n++)
        {
            Arrange(h, r);
            ArrangeField(h, r);
            for (int i = 0; i < 4; i++) h.Program.WriteWord((ushort)(M.Paid + 2 * i), 0);
            h.Script = Script(r.Next(), r.Next(2) == 0);
            AssertSame(h, $"000F case {n}", 0x000F, (d, random, rec) => Drain(M.Run(d, random, rec)));
            h.Script = null;
        }
    }

    /// <summary>
    /// Fat Mike in real games: the original (four computers) is run to the entry of 1F63:000F in weeks 0 to 3; there the state is
    /// taken as it is and, in further runs, players are made human (with scripted fire), the flag DS:4BC2 and the stocks varied.
    /// Then the same game runs on to the contest 1F63:0B03 of the week (<see cref="Contest"/>).
    /// </summary>
    // [DIFFERENTIAL] 1F63:000F
    [GameDataFact]
    public void Fat_Mike_in_real_games_matches_the_original()
    {
        var game = OriginalProgram.StartGame(TestGameData.DosDir);
        var r = new Random(0x26);
        int paid = 0, punished = 0, typed = 0, prizes = 0, rounds = 0, passes = 0;
        for (int week = 0; week < 4; week++)
        {
            Assert.True(game.RunToRoutine(Seg, 0x000F, 900), $"1F63:000F not reached (week {game.Week})");
            Assert.Equal(week, game.Week);
            var snapshot = game.Snapshot();
            for (int run = 0; run < 5; run++)
            {
                var h = new FatMikeHarness(OriginalProgram.Resume(TestGameData.DosDir, snapshot));
                if (run > 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (r.Next(2) == 0) h.Program.WriteByte((ushort)(L.IsComputer + i), 0);
                        if (r.Next(3) == 0) h.Program.WriteByte((ushort)(L.FatMikeFlag + i), 1);
                        if (r.Next(3) == 0) h.Program.WriteWord((ushort)(Stock3 + 8 * i), (ushort)r.Next(0, 400));
                    }
                    if (run == 4) h.Program.WriteDword(OriginalProgram.RandomStateOffset, (uint)r.Next() * 2654435761u);
                }
                h.Script = Script(r.Next(), r.Next(2) == 0);
                if (run == 3)
                {
                    // the player with the most fields is a human who never pays: he is punished (crops or fields)
                    int most = Enumerable.Range(0, 4).MaxBy(i => h.Program.ReadByte((ushort)(F.OwnedFields + i)));
                    h.Program.WriteByte((ushort)(L.IsComputer + most), 0);
                    h.Program.WriteByte((ushort)(L.FatMikeFlag + most), (byte)(week % 2));
                    h.Script = _ => [-1, -1, -1, -1];
                }
                AssertSame(h, $"week {week} run {run}", 0x000F, (d, random, rec) => Drain(M.Run(d, random, rec)));
                paid += h.Calls.Count(c => c == "Sound(0, 37)");
                punished += h.Calls.Count(c => c is "Sound(0, 38)" or "Sound(0, 39)");
                typed += h.Calls.Count(c => c.StartsWith(nameof(FatMikeEventKind.TypeCharacter), StringComparison.Ordinal));
            }
            game.Machine.RunFor(0.05);
            Contest(game, week, r, ref prizes, ref rounds, ref passes);
            game.Machine.RunFor(0.05);
        }
        Assert.True(paid > 4 && punished > 2 && typed > 0, $"paid {paid}, punished {punished}, typed {typed}");
        Assert.True(prizes > 0 && rounds > 1 && passes > rounds, $"contest prizes {prizes}, second rounds {rounds}, field passes {passes}");
    }

    /// <summary>
    /// The contest 1F63:0B03 of the same week (call 30), whole with its second field round 0609:0080 (computers and helpers of
    /// lane anbau-ki in the port, the original's in the RefEmu): the state as it is, then with humans (scripted fire), stocks
    /// around the field prize limit 70 and the flag DS:4BC2 varied. Only week 2 of weeks 0–3 has a contest.
    /// </summary>
    // [DIFFERENTIAL] 1F63:0B03
    // [DIFFERENTIAL] 0609:0080
    private static void Contest(OriginalProgram game, int week, Random r, ref int prizes, ref int rounds, ref int passes)
    {
        Assert.True(game.RunToRoutine(Seg, 0x0B03, 900), $"1F63:0B03 not reached (week {game.Week})");
        Assert.Equal(week, game.Week);
        var snapshot = game.Snapshot();
        for (int run = 0; run < (week % 3 == 2 ? 6 : 1); run++)
        {
            var h = new FatMikeHarness(OriginalProgram.Resume(TestGameData.DosDir, snapshot), stubSecondRound: false);
            if (run > 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (r.Next(2) == 0) h.Program.WriteByte((ushort)(L.IsComputer + i), 0);
                    if (r.Next(3) == 0) h.Program.WriteByte((ushort)(L.FatMikeFlag + i), (byte)r.Next(2));
                    // odd runs: some stocks of 70 and more (field prize); even runs: all below 70 (cash prize)
                    if (run % 2 == 0) h.Program.WriteWord((ushort)(Stock3 + 8 * i), (ushort)r.Next(0, 70));
                    else if (r.Next(2) == 0) h.Program.WriteWord((ushort)(Stock3 + 8 * i), (ushort)r.Next(70, 400));
                }
                if (run == 5) h.Program.WriteDword(OriginalProgram.RandomStateOffset, (uint)r.Next() * 2654435761u);
            }
            h.Script = Script(r.Next(), true);
            AssertSame(h, $"contest week {week} run {run}", 0x0B03, (d, random, rec) => Drain(ContestPhase.Run(d, random, rec)));
            prizes += h.Calls.Count(c => c.StartsWith(nameof(FatMikeEventKind.Number), StringComparison.Ordinal));
            rounds += h.Calls.Count(c => c == nameof(FieldEventKind.DrawStatus) + "()");
            passes += h.Passes;
        }
    }
}
