using Traders.Core.Robots.Purchase;
using Traders.Core.State;
using Traders.Tests.Rules;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Tests.Robots.Purchase;

/// <summary>
/// Differential tests of the data steps of the robot purchase (track "kauf", part 1; docs/routines/1C68_000B_roboterphase.md):
/// the original routine runs in the RefEmu on a prepared data segment, the port on the same bytes; the whole data segment
/// (including the random state DS:10D2) and the result in AL must match. Expected values come only from the original.
/// </summary>
public class RobotPurchaseRulesDifferentialTests
{
    private static readonly RulesHarness Harness = new();

    // ------------------------------------------------------------------ inputs

    /// <summary>Random robot fields 18..25, shopping copy and purchase variables, with extreme values when asked.</summary>
    private static void RobotFields(Dgroup d, Inputs r, bool extreme)
    {
        for (int i = 0; i < 4 * 6; i++)
            d.Set(L.RobotParts + i, (byte)(extreme && r.Chance(30) ? r.Range(0, 255) : r.Chance(50) ? 9 : 10 * r.Range(0, 6) + 9));
        for (int p = 0; p < 4; p++)
        {
            d.Set(L.Robot376C + 4 * p, extreme && r.Chance(30) ? r.Word() : (short)r.Range(0, 400));
            d.Set(L.Robot376E + 4 * p, extreme && r.Chance(30) ? r.Word() : (short)r.Range(0, 4));
            d.Set(L.Robot2EF8 + 2 * p, extreme && r.Chance(30) ? r.Word() : (short)r.Range(0, 900));
            d.Set(L.Robot377C + p, (byte)(extreme && r.Chance(30) ? r.Range(0, 255) : r.Range(0, 2)));
            d.Set(L.Robot40EC + 2 * p, extreme && r.Chance(30) ? r.Word() : (short)r.Range(0, 360));
            d.Set(L.Robot33DC + 2 * p, extreme && r.Chance(30) ? r.Word() : (short)r.Range(0, 108));
            d.Set(L.Robot3016 + 2 * p, extreme && r.Chance(30) ? r.Word() : (short)r.Range(0, 7));
            for (int k = 0; k < 8; k++) d.Set(L.RobotStock + 16 * p + 2 * k, r.Word());
            d.Set(L.RobotFights + p, (byte)r.Range(0, 255));
            d.Set(L.RobotBudget + 4 * p, (int)r.Next());
            d.Set(L.RobotShop + p, (byte)r.Range(0, 255));
            d.Set(L.RobotItem + p, (byte)r.Range(0, 255));
            d.Set(L.RobotType + p, (byte)r.Range(0, 255));
            d.Set(L.RobotStep + p, (byte)r.Range(0, 255));
            d.Set(L.RobotPhaseMode + p, (byte)r.Range(0, 255));
            d.Set(L.MoveDelay + 4 * p, (int)r.Next());
            d.Set(L.ObjectX + 26 * p, r.Word());
            d.Set(L.ObjectY + 26 * p, r.Word());
            d.Set(L.IsComputer + p, (byte)(r.Chance(45) ? 0 : r.Chance(85) ? 1 : r.Range(2, 255)));
        }
        d.Set(L.RobotPlayersDone, r.Word());
        d.Set(L.ComputerIq, extreme && r.Chance(40) ? r.Word() : (short)r.Range(0, 3));
    }

    /// <summary>A state whose total wealth of every player is just its cash (no land, wares, gems, rockets, oil).</summary>
    private static Dgroup CashOnly(Inputs r, int[] cash)
    {
        var d = Harness.Base(r);
        RobotFields(d, r, extreme: false);
        d.Set(L.GemTrading, (byte)0);
        d.Set(L.OilConsumption, (byte)0);
        for (int p = 0; p < 4; p++)
        {
            d.Set(L.Cash + 4 * p, cash[p]);
            d.Set(L.FieldCount + p, (byte)0);
            d.Set(L.Rockets + 2 * p, (short)0);
            for (int k = 0; k < 4; k++) d.Set(L.FieldsByKind + 4 * p + k, (sbyte)0);
            for (int w = 0; w < 4; w++) d.Set(L.Stock + 8 * p + 2 * w, (short)0);
        }
        return d;
    }

    /// <summary>A plausible game state (as for 0825:0813) with random robot fields.</summary>
    private static Dgroup GameState(Inputs r, bool extreme)
    {
        var d = Harness.Base(r);
        RobotFields(d, r, extreme);
        d.Set(L.LandPrice, (short)r.Range(100, 550));
        d.Set(L.RocketPrice, (short)r.Range(1, 900));
        d.Set(L.OilPrice, (short)r.Range(1, 200));
        d.Set(L.GemTrading, (byte)r.Range(0, 1));
        d.Set(L.OilConsumption, (byte)r.Range(0, 1));
        d.Set(L.BaseUnit, r.Range(12, 50));
        d.Set(L.GemPriceFactor, r.Range(40, 160));
        for (int w = 0; w < 4; w++) d.Set(L.WarePrice + 2 * w, (short)r.Range(1, 60));
        for (int p = 0; p < 4; p++)
        {
            d.Set(L.Cash + 4 * p, extreme && r.Chance(30) ? (int)r.Next() : r.Range(-5_000, 90_000));
            d.Set(L.FieldCount + p, (byte)r.Range(0, 20));
            d.Set(L.Rockets + 2 * p, (short)r.Range(0, 10));
            d.Set(L.Oil + 2 * p, (short)r.Range(0, 40));
            d.Set(L.GemCredit + 2 * p, (short)r.Range(0, 2000));
            d.Set(L.Gems + p, (sbyte)r.Range(0, 10));
            for (int k = 0; k < 4; k++) d.Set(L.FieldsByKind + 4 * p + k, (sbyte)r.Range(0, 8));
            for (int w = 0; w < 4; w++) d.Set(L.Stock + 8 * p + 2 * w, (short)r.Range(0, 999));
        }
        return d;
    }

    private static Dgroup WithPlayer(Dgroup d, int p)
    {
        d.Set(L.ActivePlayer, (sbyte)p);
        return d;
    }

    // ------------------------------------------------------------------ 1C68:01A8 / 02ED

    // [DIFFERENTIAL] 1C68:01A8
    // [DIFFERENTIAL] 1C68:02ED
    [GameDataFact]
    public void Setup_matches_for_many_states()
    {
        for (int i = 0; i < 200; i++)
        {
            var r = new Inputs(0x01A8u + (uint)i);
            Harness.AssertSame(GameState(r, extreme: i >= 100), 0x1C68, 0x01A8, RobotPurchaseRules.Setup, $"01A8 state {i}");
        }
    }

    /// <summary>
    /// Budget bounds: wealth 4999 (→ 499 → 500), 5000, 20009 (→ 2000), 20010 (→ 2001 → 2000), negative and extreme wealth;
    /// computer bonus with IQ 0..3, an IQ whose product with 12 wraps in 16 bit, and DS:0840 = 2 (no bonus).
    /// </summary>
    // [DIFFERENTIAL] 1C68:01A8
    // [DIFFERENTIAL] 1C68:02ED
    [GameDataTheory]
    [InlineData(4999, 5000, 20009, 20010, (short)0)]
    [InlineData(-1, -9, -10, int.MinValue, (short)1)]
    [InlineData(int.MaxValue, 5009, 19999, 0, (short)2)]
    [InlineData(10_000, 15_555, 20_000, 7, (short)3)]
    [InlineData(20_000, 20_000, 20_000, 20_000, (short)0x0AAB)] // 0AABh * 12 = 8004h: bonus factor with sign
    [InlineData(20_000, 5_000, 9_999, 12_345, (short)-1)]
    public void Setup_budget_bounds_and_computer_bonus(int c0, int c1, int c2, int c3, short iq)
    {
        int[] cash = [c0, c1, c2, c3];
        foreach (byte computer in new byte[] { 0, 1, 2 })
        {
            var d = CashOnly(new Inputs((uint)(c0 ^ iq ^ computer)), cash);
            for (int p = 0; p < 4; p++) d.Set(L.IsComputer + p, (byte)(p % 2 == 0 ? computer : 1 - computer % 2));
            d.Set(L.ComputerIq, iq);
            Harness.AssertSame(d, 0x1C68, 0x01A8, RobotPurchaseRules.Setup, $"01A8 cash {string.Join('/', cash)} IQ {iq} computer {computer}");
        }
    }

    // [DIFFERENTIAL] 1C68:02ED
    [GameDataFact]
    public void Save_stock_matches_per_player()
    {
        for (int i = 0; i < 80; i++)
        {
            var r = new Inputs(0x02EDu + (uint)i);
            var d = GameState(r, extreme: i % 2 == 1);
            Harness.AssertSame(WithPlayer(d, i % 4), 0x1C68, 0x02ED, RobotPurchaseRules.SaveStock, $"02ED state {i}");
        }
    }

    // ------------------------------------------------------------------ 1C68:03C8

    // [DIFFERENTIAL] 1C68:03C8
    [GameDataFact]
    public void Commit_matches_for_many_states()
    {
        for (int i = 0; i < 80; i++)
        {
            var r = new Inputs(0x03C8u + (uint)i);
            Harness.AssertSame(GameState(r, extreme: true), 0x1C68, 0x03C8, RobotPurchaseRules.Commit, $"03C8 state {i}");
        }
    }

    /// <summary>Byte field 377Ch takes only the low byte of its shopping word; item 0 is not stored anywhere.</summary>
    // [DIFFERENTIAL] 1C68:03C8
    [GameDataFact]
    public void Commit_keeps_only_the_low_byte_of_item_4()
    {
        var d = GameState(new Inputs(4), extreme: false);
        for (int p = 0; p < 4; p++)
        {
            d.Set(L.RobotStock + 16 * p + 8, (short)(0x1234 + 0x0101 * p));
            d.Set(L.RobotStock + 16 * p, (short)-1);
        }
        var ported = Harness.AssertSame(d, 0x1C68, 0x03C8, RobotPurchaseRules.Commit, "03C8 low byte");
        Assert.Equal(0x34, ported.U8(L.Robot377C));
    }

    // ------------------------------------------------------------------ 1C68:0641

    /// <summary>Pointer x near both bounds (0 and 12Ch), negative x, extreme steps with sign.</summary>
    // [DIFFERENTIAL] 1C68:0641
    [GameDataTheory]
    [InlineData((short)0, (sbyte)-2)]
    [InlineData((short)1, (sbyte)-2)]
    [InlineData((short)2, (sbyte)-2)]
    [InlineData((short)0x12A, (sbyte)2)]
    [InlineData((short)0x12B, (sbyte)2)]
    [InlineData((short)0x12C, (sbyte)0)]
    [InlineData((short)-1, (sbyte)2)]
    [InlineData((short)-3, (sbyte)2)]
    [InlineData((short)0x7FFF, (sbyte)1)]
    [InlineData((short)100, (sbyte)-128)]
    [InlineData((short)200, (sbyte)127)]
    [InlineData((short)0x12D, (sbyte)-1)]
    public void Move_pointer_bounds(short x, sbyte step)
    {
        for (int p = 0; p < 4; p++)
        {
            var d = GameState(new Inputs((uint)(x * 7 + step + p)), extreme: false);
            d.Set(L.ObjectX + 26 * p, x);
            d.Set(L.RobotStep + p, step);
            Harness.AssertSame(WithPlayer(d, p), 0x1C68, 0x0641, RobotPurchaseRules.MovePointer, $"0641 x {x} step {step} p {p}");
        }
    }

    // [DIFFERENTIAL] 1C68:0641
    [GameDataFact]
    public void Move_pointer_matches_for_many_states()
    {
        for (int i = 0; i < 100; i++)
        {
            var r = new Inputs(0x0641u + (uint)i);
            Harness.AssertSame(WithPlayer(GameState(r, extreme: true), i % 4), 0x1C68, 0x0641, RobotPurchaseRules.MovePointer,
                $"0641 state {i}");
        }
    }

    // ------------------------------------------------------------------ 1C68:006B, 0EDE:0322, 0EDE:1826

    /// <summary>Data segment equal and AL equal (the routines return a byte; AH/DX are left over and not part of it).</summary>
    private static void AssertSameByte(Dgroup input, ushort segment, ushort offset, Func<Dgroup, bool> port, string label)
    {
        bool result = false;
        Harness.AssertSame(input, segment, offset, d => { result = port(d); }, label);
        var original = Harness.RunOriginal(input, segment, offset);
        Assert.True((original.Result & 0xFF) == (result ? 1 : 0), $"{label}: AL original {original.Result & 0xFF}, port {result}");
    }

    // [DIFFERENTIAL] 1C68:006B
    // [DIFFERENTIAL] 0EDE:0322
    // [DIFFERENTIAL] 0EDE:1826
    [GameDataFact]
    public void Readiness_matches_for_many_states()
    {
        for (int i = 0; i < 120; i++)
        {
            var r = new Inputs(0x006Bu + (uint)i);
            var d = GameState(r, extreme: i >= 60);
            AssertSameByte(d, 0x1C68, 0x006B, RobotReadiness.AnyoneFights, $"006B state {i}");
            Harness.AssertSame(WithPlayer(d, i % 4), 0x0EDE, 0x0322, RobotReadiness.UpdateFights, $"0322 state {i}");
            AssertSameByte(WithPlayer(d, i % 4), 0x0EDE, 0x1826, RobotReadiness.HasNoParts, $"1826 state {i}");
        }
    }

    /// <summary>
    /// Bounds of the readiness: parts 9 (not bought) against 10 and FFh in every slot, the word DS:376C = 0 with DS:376E
    /// set (no fight), 376C = 8000h and 1, and no robot at all (006B returns 0).
    /// </summary>
    // [DIFFERENTIAL] 1C68:006B
    // [DIFFERENTIAL] 0EDE:0322
    // [DIFFERENTIAL] 0EDE:1826
    [GameDataTheory]
    [InlineData(-1, (byte)0, (short)1)]
    [InlineData(-1, (byte)0, (short)0)]
    [InlineData(0, (byte)10, (short)0)]
    [InlineData(0, (byte)10, (short)1)]
    [InlineData(5, (byte)10, unchecked((short)0x8000))]
    [InlineData(3, (byte)0xFF, (short)400)]
    [InlineData(2, (byte)9, (short)400)]
    public void Readiness_bounds(int slot, byte value, short energy)
    {
        for (int p = 0; p < 4; p++)
        {
            var d = GameState(new Inputs((uint)(slot * 31 + value + p)), extreme: false);
            for (int q = 0; q < 4; q++)
            {
                for (int i = 0; i < 6; i++) d.Set(L.RobotParts + 6 * q + i, (byte)9);
                d.Set(L.Robot376C + 4 * q, (short)0);
                d.Set(L.Robot376E + 4 * q, (short)3);
            }
            if (slot >= 0) d.Set(L.RobotParts + 6 * p + slot, value);
            d.Set(L.Robot376C + 4 * p, energy);
            AssertSameByte(d, 0x1C68, 0x006B, RobotReadiness.AnyoneFights, $"006B slot {slot} value {value} 376C {energy} p {p}");
            Harness.AssertSame(WithPlayer(d, p), 0x0EDE, 0x0322, RobotReadiness.UpdateFights, $"0322 slot {slot} p {p}");
            AssertSameByte(WithPlayer(d, p), 0x0EDE, 0x1826, RobotReadiness.HasNoParts, $"1826 slot {slot} p {p}");
        }
    }
}
