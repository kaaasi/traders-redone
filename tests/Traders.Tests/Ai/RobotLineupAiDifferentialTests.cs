using Traders.Core;
using Traders.Core.Ai;
using Traders.Core.Input;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.Input;
using L = Traders.Core.Ai.RobotLineupLayout;

namespace Traders.Tests.Ai;

/// <summary>
/// Differential tests of the lineup AI, segment 12C8 (finding docs/routines/12C8_000B_aufstellung_ki.md), on generated data
/// segments. The same data segment goes into the original (RefEmu, LoadOnly, direct far call) and the port; AX where returned,
/// the whole data segment and the random state DS:10D2 must match. The tables of T.EXE (corners, quadrants, neighbours, bit
/// masks) stay as loaded; everything the routines read from BSS is randomised with signed and unsigned edges. The search
/// 12C8:06D6 ends for every state with these tables (see RobotLineupAi.TargetSearchLimit); the calls still run with an
/// instruction budget. States from real games: RobotLineupAiGameStateDifferentialTests.
/// </summary>
public class RobotLineupAiDifferentialTests
{
    private const ushort Seg = 0x12C8;
    private const long Budget = 5_000_000;
    private const int Player = DgroupLayout.ActivePlayer;
    private const ushort OutX = 0x5880, OutY = 0x5884; // scratch words inside DGROUP for the far pointers of 12C8:00B5

    private static (OriginalProgram P, byte[] Initial) Load()
    {
        var p = OriginalProgram.LoadOnly(TestGameData.DosDir);
        return (p, p.DataSegment());
    }

    private static ushort Edge16(Random r) => (ushort)(r.Next(6) switch
    {
        0 => r.Next(0x10000),
        1 => 0x7FF0 + r.Next(0x20),   // around the signed edge
        2 => 0xFFF0 + r.Next(0x10),   // small negative
        _ => r.Next(0, 400),
    });

    /// <summary>
    /// Random state for the routines of 12C8. <paramref name="player"/> null: 0..3, rarely 4 (as left by the player loops).
    /// </summary>
    private static void Arrange(OriginalProgram p, byte[] initial, Random r, int? player = null)
    {
        p.Machine.Mem.WriteBlock(Traders.RefEmu.Cpu.Memory.Linear(p.Dgroup, 0), initial);
        p.WriteByte(Player, (byte)(player ?? (r.Next(16) == 0 ? 4 : r.Next(4))));
        int density = r.Next(1, 4); // share of used blocks
        for (int i = 0; i < 64; i++)
            p.WriteByte((ushort)(L.Blocks + i), (byte)(r.Next(4) < density ? r.Next(1, 256) : 0));
        for (int i = 0; i < 8; i++)
            p.WriteByte((ushort)(L.Marks + i), (byte)(r.Next(3) == 0 ? 0xC3 : r.Next(256)));
        for (int i = 0; i < 20; i++) // 4 x 4 plus the bytes reached by ring -1 or player 4
        {
            p.WriteByte((ushort)(L.RingUnits - 1 + i), (byte)(r.Next(8) == 0 ? r.Next(256) : r.Next(0, 7)));
            p.WriteByte((ushort)(L.RingTaken - 1 + i), (byte)r.Next(0, 2));
            p.WriteByte((ushort)(L.RingSpare - 1 + i), (byte)r.Next(0, 2));
        }
        for (int i = 0; i < 5; i++)
        {
            p.WriteByte((ushort)(L.Pass + i), (byte)(r.Next(8) == 0 ? r.Next(256) : r.Next(0, 4)));
            p.WriteByte((ushort)(L.Presses + i), (byte)(r.Next(3) == 0 ? 0 : r.Next(0, 7)));
            p.WriteByte((ushort)(L.Wanted + i), (byte)(r.Next(8) == 0 ? r.Next(256) : r.Next(0, 4)));
            p.WriteByte((ushort)(L.Mode + i), (byte)(r.Next(3) == 0 ? r.Next(256) : L.ModePlacing));
            p.WriteByte((ushort)(DgroupLayout.IsComputer + i), (byte)(r.Next(4) == 0 ? r.Next(0, 3) : 1));
            p.WriteByte((ushort)(ControlReader.Bits + i), (byte)r.Next(256));
            // pointer at the start position, inside the field or anywhere
            int pl = Math.Min(i, 3);
            bool atStart = r.Next(3) == 0;
            ushort px = atStart ? p.ReadByte((ushort)(L.StartX + pl)) : r.Next(6) == 0 ? Edge16(r) : (ushort)r.Next(0, 26);
            ushort py = atStart ? p.ReadByte((ushort)(L.StartY + pl)) : r.Next(6) == 0 ? Edge16(r) : (ushort)r.Next(0, 26);
            p.WriteWord((ushort)(L.PointerX + 2 * i), px);
            p.WriteWord((ushort)(L.PointerY + 2 * i), py);
            bool onTarget = r.Next(2) == 0;
            p.WriteWord((ushort)(L.TargetX + 2 * i), onTarget ? px : (ushort)(px + r.Next(-3, 4)));
            p.WriteWord((ushort)(L.TargetY + 2 * i), onTarget ? py : (ushort)(py + r.Next(-3, 4)));
            p.WriteWord((ushort)(DgroupLayout.Robot2EF8 + 2 * i), Edge16(r));
            p.WriteWord((ushort)(DgroupLayout.Robot3016 + 2 * i), r.Next(3) == 0 ? Edge16(r) : (ushort)r.Next(0, 12));
            p.WriteWord((ushort)(DgroupLayout.Robot40EC + 2 * i), Edge16(r));
            p.WriteWord((ushort)(DgroupLayout.Robot33DC + 2 * i), Edge16(r));
            p.WriteDword((ushort)(ControlReader.MoveDelay + 4 * i), Delay(r));
            p.WriteDword((ushort)(ControlReader.FireDelay + 4 * i), Delay(r));
        }
        for (int i = 0; i < 26 * 26; i++) // field values: kinds 0 and 2 (value / 8) qualify
            p.WriteByte((ushort)(0x48F4 + i), (byte)(r.Next(10) == 0 ? r.Next(256) : r.Next(0, 32)));
        p.WriteDword(OriginalProgram.RandomStateOffset, (uint)r.Next());
    }

    private static uint Delay(Random r) => r.Next(6) switch
    {
        0 => (uint)r.Next(1, 20),
        1 => 0x00010000u,           // only the high word above 0
        2 => 0xFFFFFFFFu,           // -1: not above 0
        3 => 0x80000000u,
        _ => 0,
    };

    private static void AssertSame(string label, OriginalProgram p, Dgroup port)
    {
        ControlHarness.AssertSameData(label, p.DataSegment(), port.Bytes);
        Assert.Equal(p.ReadDword(OriginalProgram.RandomStateOffset), port.U32(DgroupLayout.RandomSeed));
    }

    private static void Run(ushort offset, int seed, int cases, Action<Dgroup> port, Action<Random, OriginalProgram>? tweak = null)
    {
        var (p, initial) = Load();
        var r = new Random(seed);
        for (int n = 0; n < cases; n++)
        {
            Arrange(p, initial, r);
            tweak?.Invoke(r, p);
            var ds = new Dgroup(p.DataSegment());
            p.CallWithBudget(Budget, Seg, offset);
            port(ds);
            AssertSame($"{offset:X4} case {n} (player {ds.S8(Player)})", p, ds);
        }
    }

    // [DIFFERENTIAL] 12C8:000B
    [GameDataFact]
    public void ResetMarks_matches_the_original() => Run(0x000B, 0x000B, 50, RobotLineupAi.ResetMarks);

    // [DIFFERENTIAL] 12C8:003B
    [GameDataFact]
    public void Plan_matches_the_original() => Run(0x003B, 0x003B, 600, RobotLineupAi.Plan);

    // [DIFFERENTIAL] 12C8:005A
    [GameDataFact]
    public void MarkQuadrant_matches_the_original() => Run(0x005A, 0x005A, 600, RobotLineupAi.MarkQuadrant);

    // [DIFFERENTIAL] 12C8:030B
    [GameDataFact]
    public void PlanRings_matches_the_original() => Run(0x030B, 0x030B, 800, RobotLineupAi.PlanRings);

    /// <summary>
    /// Budget edges of 030B: Robot3016 below and above Robot2EF8 / 20 (unsigned), Robot40EC / 16 against that limit (signed),
    /// a quota just enough for a ring, all blocks marked so that every ring counts its full 2·r + 1 blocks.
    /// </summary>
    // [DIFFERENTIAL] 12C8:030B
    [GameDataFact]
    public void PlanRings_budget_edges_match_the_original() => Run(0x030B, 0x130B, 600, RobotLineupAi.PlanRings, (r, p) =>
    {
        int pl = (sbyte)p.ReadByte(Player);
        if (pl is < 0 or > 3) return;
        if (r.Next(2) == 0)
            for (int i = 0; i < 8; i++) p.WriteByte((ushort)(L.Marks + i), 0xFF);
        ushort lim = (ushort)r.Next(0, 12);
        p.WriteWord((ushort)(DgroupLayout.Robot2EF8 + 2 * pl), (ushort)(lim * 20 + r.Next(-1, 2)));
        p.WriteWord((ushort)(DgroupLayout.Robot3016 + 2 * pl), (ushort)(lim + r.Next(-1, 2)));
        p.WriteWord((ushort)(DgroupLayout.Robot40EC + 2 * pl), (ushort)(r.Next(0, 12) * 16 + r.Next(-1, 2)));
        p.WriteWord((ushort)(DgroupLayout.Robot33DC + 2 * pl), (ushort)(r.Next(0, 8) * 9 + r.Next(-1, 2)));
    });

    // [DIFFERENTIAL] 12C8:06D6
    [GameDataFact]
    public void ChooseTarget_matches_the_original() => Run(0x06D6, 0x06D6, 800, RobotLineupAi.ChooseTarget);

    /// <summary>06D6 at the start position and at the last cell of the quadrant (the next step ends the pass).</summary>
    // [DIFFERENTIAL] 12C8:06D6
    [GameDataFact]
    public void ChooseTarget_quadrant_edges_match_the_original() => Run(0x06D6, 0x16D6, 400, RobotLineupAi.ChooseTarget, (r, p) =>
    {
        int pl = (sbyte)p.ReadByte(Player);
        if (pl is < 0 or > 3) return;
        (ushort x, ushort y) = r.Next(3) switch
        {
            0 => (p.ReadByte((ushort)(L.StartX + pl)), p.ReadByte((ushort)(L.StartY + pl))),
            1 => (p.ReadByte((ushort)(L.QuadrantMaxX + pl)), p.ReadByte((ushort)(L.QuadrantMaxY + pl))),
            _ => ((ushort)(p.ReadByte((ushort)(L.QuadrantMaxX + pl)) - r.Next(0, 2)), p.ReadByte((ushort)(L.QuadrantMaxY + pl))),
        };
        p.WriteWord((ushort)(L.PointerX + 2 * pl), x);
        p.WriteWord((ushort)(L.PointerY + 2 * pl), y);
        if (r.Next(2) == 0) // no cell qualifies: the pass ends at the start position
            for (int i = 0; i < 26 * 26; i++) p.WriteByte((ushort)(0x48F4 + i), 8);
    });

    // [DIFFERENTIAL] 12C8:0539
    [GameDataFact]
    public void Steer_matches_the_original() => Run(0x0539, 0x0539, 800, RobotLineupAi.Steer);

    // [DIFFERENTIAL] 12C8:0621
    [GameDataFact]
    public void SteerToTarget_matches_the_original() => Run(0x0621, 0x0621, 400, RobotLineupAi.SteerToTarget);

    // [DIFFERENTIAL] 12C8:08EA
    [GameDataFact]
    public void SteerByPass_matches_the_original() => Run(0x08EA, 0x08EA, 400, RobotLineupAi.SteerByPass);

    private static short Coordinate(Random r) => (short)(r.Next(8) switch
    {
        0 => r.Next(-3, 0),
        1 => r.Next(8, 11),
        2 => r.Next(short.MinValue, short.MaxValue + 1),
        _ => r.Next(0, 8),
    });

    // [DIFFERENTIAL] 12C8:00B5
    [GameDataFact]
    public void RingBlock_matches_the_original()
    {
        var (p, initial) = Load();
        var r = new Random(0x00B5);
        for (int n = 0; n < 800; n++)
        {
            Arrange(p, initial, r);
            short ring = (short)(r.Next(10) == 0 ? r.Next(short.MinValue, short.MaxValue + 1) : r.Next(0, 4));
            short step = (short)(r.Next(10) == 0 ? r.Next(-2, 10) : r.Next(0, 2 * Math.Max((int)ring, 0) + 1));
            var ds = new Dgroup(p.DataSegment());
            p.CallWithBudget(Budget, Seg, 0x00B5, (ushort)ring, (ushort)step, OutX, p.Dgroup, OutY, p.Dgroup);
            var (x, y) = RobotLineupAi.RingBlock(ds, ring, step);
            ds.Set(OutX, x);
            ds.Set(OutY, y);
            AssertSame($"00B5({ring}, {step}) case {n}", p, ds);
        }
    }

    // [DIFFERENTIAL] 12C8:015C
    [GameDataFact]
    public void MarkBetween_matches_the_original()
    {
        var (p, initial) = Load();
        var r = new Random(0x015C);
        for (int n = 0; n < 800; n++)
        {
            Arrange(p, initial, r);
            short x = Coordinate(r), y = Coordinate(r);
            if (r.Next(3) > 0) { x = (short)r.Next(0, 8); y = (short)r.Next(0, 8); }
            var ds = new Dgroup(p.DataSegment());
            p.CallWithBudget(Budget, Seg, 0x015C, (ushort)x, (ushort)y);
            RobotLineupAi.MarkBetween(ds, x, y);
            AssertSame($"015C({x}, {y}) case {n}", p, ds);
        }
    }

    /// <summary>Grid edges -1/0 and 7/8 on both axes plus far values; distance from every corner.</summary>
    // [DIFFERENTIAL] 12C8:024E
    [GameDataFact]
    public void Distance_matches_the_original()
    {
        var (p, initial) = Load();
        var r = new Random(0x024E);
        for (int n = 0; n < 1200; n++)
        {
            Arrange(p, initial, r);
            short x = Coordinate(r), y = Coordinate(r);
            var ds = new Dgroup(p.DataSegment());
            short ax = (short)p.CallWithBudget(Budget, Seg, 0x024E, (ushort)x, (ushort)y).Ax;
            Assert.True(ax == RobotLineupAi.Distance(ds, x, y), $"024E({x}, {y}) case {n}: player {ds.S8(Player)} ax={ax}");
            AssertSame($"024E case {n}", p, ds);
        }
    }

    /// <summary>The flag is compared as a byte: words 1 and 101h set, 0, 2 and 100h clear.</summary>
    // [DIFFERENTIAL] 12C8:02BB
    [GameDataFact]
    public void SetMark_matches_the_original()
    {
        var (p, initial) = Load();
        var r = new Random(0x02BB);
        ushort[] flags = [0, 1, 2, 0x100, 0x101, 0xFF];
        for (int n = 0; n < 800; n++)
        {
            Arrange(p, initial, r);
            short x = (short)r.Next(-1, 9), y = (short)r.Next(-1, 9);
            ushort flag = flags[r.Next(flags.Length)];
            var ds = new Dgroup(p.DataSegment());
            p.CallWithBudget(Budget, Seg, 0x02BB, (ushort)x, (ushort)y, flag);
            RobotLineupAi.SetMark(ds, x, y, (byte)flag);
            AssertSame($"02BB({x}, {y}, {flag:X}) case {n}", p, ds);
        }
    }

    // [DIFFERENTIAL] 12C8:02EA
    [GameDataFact]
    public void IsMarked_matches_the_original()
    {
        var (p, initial) = Load();
        var r = new Random(0x02EA);
        for (int n = 0; n < 800; n++)
        {
            Arrange(p, initial, r);
            short x = (short)r.Next(-1, 9), y = (short)r.Next(-1, 9);
            var ds = new Dgroup(p.DataSegment());
            byte al = (byte)p.CallWithBudget(Budget, Seg, 0x02EA, (ushort)x, (ushort)y).Ax;
            Assert.True(al == (RobotLineupAi.IsMarked(ds, x, y) ? 1 : 0), $"02EA({x}, {y}) case {n}: al={al}");
            AssertSame($"02EA case {n}", p, ds);
        }
    }

    /// <summary>
    /// The whole lineup AI in order as the original runs it: 000B once, 003B for each computer (with the pointer at the
    /// start position as 0535:00B4 leaves it), then 0539 frame after frame with the pointer moved by the control bits as
    /// 0E9E would (one cell per bit, delays cleared) until every computer has pressed all its fire presses of three passes.
    /// Each step is compared; the pointer moves are written to both sides by the test.
    /// </summary>
    // [DIFFERENTIAL] 12C8:0539
    [GameDataFact]
    public void Lineup_sequence_matches_the_original()
    {
        var (p, initial) = Load();
        for (int seed = 0; seed < 12; seed++)
        {
            var r = new Random(0x0539 + seed);
            Arrange(p, initial, r, 0);
            for (int pl = 0; pl < 4; pl++)
            {
                p.WriteByte((ushort)(DgroupLayout.IsComputer + pl), (byte)(r.Next(4) == 0 ? 0 : 1));
                p.WriteWord((ushort)(L.PointerX + 2 * pl), p.ReadByte((ushort)(L.StartX + pl)));
                p.WriteWord((ushort)(L.PointerY + 2 * pl), p.ReadByte((ushort)(L.StartY + pl)));
                p.WriteByte((ushort)(L.Mode + pl), L.ModePlacing);
                p.WriteDword((ushort)(ControlReader.MoveDelay + 4 * pl), 0);
                p.WriteDword((ushort)(ControlReader.FireDelay + 4 * pl), 0);
            }
            var ds = new Dgroup(p.DataSegment());
            p.CallWithBudget(Budget, Seg, 0x000B);
            RobotLineupAi.ResetMarks(ds);
            AssertSame($"seed {seed} 000B", p, ds);
            for (int pl = 0; pl < 4; pl++)
            {
                if (ds.U8(DgroupLayout.IsComputer + pl) != 1) continue;
                p.WriteByte(Player, (byte)pl);
                ds.Set(Player, (sbyte)pl);
                p.CallWithBudget(Budget, Seg, 0x003B);
                RobotLineupAi.Plan(ds);
                AssertSame($"seed {seed} 003B player {pl}", p, ds);
            }
            for (int frame = 0; frame < 400; frame++)
            {
                p.CallWithBudget(Budget, Seg, 0x0539);
                RobotLineupAi.Steer(ds);
                AssertSame($"seed {seed} 0539 frame {frame}", p, ds);
                for (int pl = 0; pl < 4; pl++)
                {
                    byte bits = ds.U8(ControlReader.Bits + pl);
                    short x = ds.S16(L.PointerX + 2 * pl), y = ds.S16(L.PointerY + 2 * pl);
                    if ((bits & 1) != 0) x++;
                    if ((bits & 2) != 0) x--;
                    if ((bits & 8) != 0) y++;
                    if ((bits & 4) != 0) y--;
                    foreach (var (off, v) in new[] { (L.PointerX + 2 * pl, x), (L.PointerY + 2 * pl, y) })
                    {
                        p.WriteWord((ushort)off, (ushort)v);
                        ds.Set(off, v);
                    }
                }
            }
            Assert.Contains(Enumerable.Range(0, 4), pl => ds.U8(L.Pass + pl) > 0 || ds.U8(DgroupLayout.IsComputer + pl) != 1);
        }
    }
}
