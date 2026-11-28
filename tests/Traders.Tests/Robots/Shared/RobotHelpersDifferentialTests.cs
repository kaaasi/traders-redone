using Traders.Core;
using Traders.Core.Robots.Shared;
using Traders.Core.State;
using Traders.RefEmu;
using Traders.Tests.Input;

namespace Traders.Tests.Robots.Shared;

/// <summary>
/// Differential tests of the shared robot helpers 1C68:0A28/0AB0/0B02 and 150F:1B56 (findings
/// docs/routines/1C68_0A28_roboterhilfen.md, docs/routines/150F_1B56_kampfstufe.md). The same data segment goes into the
/// original (RefEmu, LoadOnly, direct far call) and the port; AX (where returned), the whole data segment and the random state
/// DS:10D2 must match. Inputs cover signed and unsigned edges: negative items and players, owned byte 0xFF and other negative
/// bytes, part bytes up to 255, Robot376C around 5 and above 0x7FFF.
/// </summary>
public class RobotHelpersDifferentialTests
{
    private const ushort Seg1C68 = 0x1C68, Seg150F = 0x150F;
    private const int Parts = DgroupLayout.RobotParts, Player = DgroupLayout.ActivePlayer, Gate = DgroupLayout.Robot376C;

    private static (OriginalProgram P, byte[] Initial) Load()
    {
        var p = OriginalProgram.LoadOnly(TestGameData.DosDir);
        return (p, p.DataSegment());
    }

    private static byte PartByte(Random r) => r.Next(5) switch
    {
        0 => (byte)r.Next(256),
        1 => (byte)(r.Next(26) * 10),           // exact multiples of 10
        2 => (byte)(r.Next(26) * 10 + 9),       // just below the next multiple
        _ => (byte)r.Next(0, 100),
    };

    private static void Arrange(OriginalProgram p, byte[] initial, Random r)
    {
        p.Machine.Mem.WriteBlock(Traders.RefEmu.Cpu.Memory.Linear(p.Dgroup, 0), initial);
        // player 0..3, 4 as left behind by the player loops, rarely a negative byte
        p.WriteByte(Player, (byte)(r.Next(12) == 0 ? r.Next(-3, 0) : r.Next(0, 5)));
        for (int i = -12; i < 6 * 8; i++) // includes the bytes before RobotParts reached by negative slots
            p.WriteByte((ushort)(Parts + i), PartByte(r));
        if (r.Next(2) == 0) // half of the cases keep the original price table, the others randomise it
            for (int i = 0; i < 12; i++)
                p.WriteWord((ushort)(RobotSharedLayout.ItemPrice + 2 * i), (ushort)(r.Next(3) == 0 ? r.Next(0x10000) : r.Next(0, 2000)));
        for (int i = 0; i < 5; i++)
            p.WriteByte((ushort)(RobotSharedLayout.OwnedUpgrade + i),
                (byte)(r.Next(3) == 0 ? 0xFF : r.Next(8) == 0 ? r.Next(0x80, 0xFF) : r.Next(0, 4)));
        for (int i = 0; i < 5; i++)
            p.WriteWord((ushort)(Gate + 4 * i), (ushort)(r.Next(4) switch
            {
                0 => r.Next(0x10000),
                1 => 0x8000 + r.Next(16),
                _ => r.Next(0, 12),
            }));
        p.WriteDword(OriginalProgram.RandomStateOffset, (uint)r.Next());
    }

    private static void AssertSame(string label, OriginalProgram p, Dgroup port)
    {
        ControlHarness.AssertSameData(label, p.DataSegment(), port.Bytes);
        Assert.Equal(p.ReadDword(OriginalProgram.RandomStateOffset), port.U32(DgroupLayout.RandomSeed));
    }

    private static short Slot(Random r) => (short)(r.Next(10) == 0 ? r.Next(-2, 6) : r.Next(0, 3));

    // [DIFFERENTIAL] 1C68:0AB0
    [GameDataFact]
    public void PartPairValue_matches_the_original()
    {
        var (p, initial) = Load();
        var r = new Random(0x0AB0);
        for (int n = 0; n < 600; n++)
        {
            Arrange(p, initial, r);
            short slot = Slot(r);
            var ds = new Dgroup(p.DataSegment());
            short ax = (short)p.Call(Seg1C68, 0x0AB0, (ushort)slot).Ax;
            Assert.True(ax == RobotHelpers.PartPairValue(ds, slot), $"0AB0({slot}) case {n}: player {ds.S8(Player)} ax={ax}");
            AssertSame($"0AB0 case {n}", p, ds);
        }
    }

    // [DIFFERENTIAL] 1C68:0B02
    [GameDataFact]
    public void PartPairAverage_matches_the_original()
    {
        var (p, initial) = Load();
        var r = new Random(0x0B02);
        for (int n = 0; n < 600; n++)
        {
            Arrange(p, initial, r);
            short slot = Slot(r);
            var ds = new Dgroup(p.DataSegment());
            short ax = (short)p.Call(Seg1C68, 0x0B02, (ushort)slot).Ax;
            Assert.True(ax == RobotHelpers.PartPairAverage(ds, slot), $"0B02({slot}) case {n}: player {ds.S8(Player)} ax={ax}");
            AssertSame($"0B02 case {n}", p, ds);
        }
    }

    // Branch edges 7/8, 11/12, the modulo-7 edges 18/19 and 32/33, negative items and items whose low byte wraps.
    private static readonly short[] EdgeItems = [short.MinValue, -1, 0, 7, 8, 11, 12, 13, 18, 19, 25, 26, 32, 33, 139, 140, 267, 268, short.MaxValue];

    // [DIFFERENTIAL] 1C68:0A28
    [GameDataFact]
    public void ItemPrice_matches_the_original()
    {
        var (p, initial) = Load();
        var r = new Random(0x0A28);
        for (int n = 0; n < 1200; n++)
        {
            Arrange(p, initial, r);
            short item = n < EdgeItems.Length * 20 ? EdgeItems[n % EdgeItems.Length] : (short)(r.Next(8) == 0 ? r.Next(-40, 300) : r.Next(0, 33));
            var ds = new Dgroup(p.DataSegment());
            short ax = (short)p.Call(Seg1C68, 0x0A28, (ushort)item).Ax;
            Assert.True(ax == RobotHelpers.ItemPrice(ds, item),
                $"0A28({item}) case {n}: player {ds.S8(Player)} owned {ds.U8(RobotSharedLayout.OwnedUpgrade + Math.Max((int)ds.S8(Player), 0)):X2} ax={ax}");
            AssertSame($"0A28 case {n}", p, ds);
        }
    }

    // Gate edges: 5 keeps the level at 0, 6 computes it; 0x8000 and 0xFFFF are large only when compared unsigned.
    private static readonly ushort[] EdgeGates = [0, 5, 6, 0x7FFF, 0x8000, 0xFFFF];

    // [DIFFERENTIAL] 150F:1B56
    [GameDataFact]
    public void UpdateBattleLevel_matches_the_original()
    {
        var (p, initial) = Load();
        var r = new Random(0x1B56);
        for (int n = 0; n < 800; n++)
        {
            Arrange(p, initial, r);
            if (n < EdgeGates.Length * 20)
            {
                int pl = unchecked((sbyte)p.Machine.Mem.Read8(p.Dgroup, Player));
                p.WriteWord((ushort)(Gate + 4 * pl), EdgeGates[n % EdgeGates.Length]);
            }
            // a nonzero level beforehand shows that the gate branch writes 0 rather than leaving the byte alone
            for (int i = -3; i < 5; i++)
                p.WriteByte((ushort)(RobotSharedLayout.BattleLevel + i), (byte)r.Next(1, 256));
            var ds = new Dgroup(p.DataSegment());
            p.Call(Seg150F, 0x1B56);
            RobotHelpers.UpdateBattleLevel(ds);
            AssertSame($"1B56 case {n} (player {ds.S8(Player)})", p, ds);
        }
    }

    /// <summary>All parts at 255 give the largest sum (25 * 4 + 25 * 3) / 7 = 25; level byte must be exactly that.</summary>
    // [DIFFERENTIAL] 150F:1B56
    [GameDataFact]
    public void UpdateBattleLevel_full_parts_matches_the_original()
    {
        var (p, initial) = Load();
        for (int pl = 0; pl < 4; pl++)
        {
            p.Machine.Mem.WriteBlock(Traders.RefEmu.Cpu.Memory.Linear(p.Dgroup, 0), initial);
            p.WriteByte(Player, (byte)pl);
            for (int i = 0; i < 6; i++) p.WriteByte((ushort)(Parts + pl * 6 + i), 255);
            p.WriteWord((ushort)(Gate + 4 * pl), 6);
            var ds = new Dgroup(p.DataSegment());
            p.Call(Seg150F, 0x1B56);
            RobotHelpers.UpdateBattleLevel(ds);
            AssertSame($"1B56 full player {pl}", p, ds);
            Assert.Equal(25, p.Machine.Mem.Read8(p.Dgroup, (ushort)(RobotSharedLayout.BattleLevel + pl)));
        }
    }
}
