using Traders.Core.FatMike;
using L = Traders.Core.State.DgroupLayout;
using M = Traders.Core.FatMike.FatMikePhase;
using T = Traders.Tests.FatMike.FatMikeDifferentialTests;

namespace Traders.Tests.FatMike;

/// <summary>
/// Differential tests of the contest 1F63:0B03 (finding docs/routines/1F63_0B03_wettbewerb.md) against the original in the
/// RefEmu (<see cref="FatMikeHarness"/>, as the tests of Fat Mike 1F63:000F): data segment, random state, number of random()
/// calls and the recorded calls. The second field round 0609:0080 is replaced here; it runs whole in the real games of
/// <see cref="FatMikeDifferentialTests.Fat_Mike_in_real_games_matches_the_original"/>.
/// </summary>
public class ContestDifferentialTests
{
    private const int Stock3 = L.Stock + 6;

    // [DIFFERENTIAL] 1F63:11AD
    [GameDataFact]
    public void Flashing_matches_the_original()
    {
        var h = new FatMikeHarness();
        var r = new Random(0x11AD);
        for (int n = 0; n < 40; n++)
        {
            T.Arrange(h, r);
            for (int i = 0; i < 4; i++)
            {
                h.Program.WriteByte((ushort)(M.Bidders + i), (byte)(r.Next(3) == 0 ? 1 : r.Next(6) == 0 ? r.Next(256) : 0));
                h.Program.WriteWord((ushort)(ContestPhase.PlayerColours + 2 * i), (ushort)r.Next(65536));
            }
            T.AssertSame(h, $"11AD case {n}", 0x11AD, (d, random, rec) => T.Drain(ContestPhase.FlashFrames(d, rec)));
        }
    }

    /// <summary>
    /// 1F63:0B03 from LoadOnly states: weeks with and without contest (week mod 3 = 2, also negative and large weeks), stocks
    /// around the field prize limit 70 and the 16-bit bounds, free fields 0, positive and negative (signed byte), players left
    /// out by DS:4BC2 = 1, ties; texts at the pointers DS:3B6C/3B70 or null.
    /// </summary>
    // [DIFFERENTIAL] 1F63:0B03
    [GameDataFact]
    public void Contest_matches_the_original()
    {
        var h = new FatMikeHarness();
        var r = new Random(0x0B03);
        int prizes = 0, fields = 0, none = 0, typed = 0, negative = 0;
        for (int n = 0; n < 150; n++)
        {
            T.Arrange(h, r);
            T.ArrangeField(h, r);
            var p = h.Program;
            int week = r.Next(6) switch
            {
                0 => r.Next(-40, 65536),
                1 or 2 => 3 * r.Next(219, 10900) + 2, // prize week·50 ≥ 8000h: negative as a word, added unsigned
                _ => 3 * r.Next(0, 30) + (r.Next(4) == 0 ? r.Next(3) : 2),
            };
            p.WriteWord(L.Week, (ushort)week);
            for (int i = 0; i < 4; i++)
            {
                int stock = r.Next(4) switch
                {
                    0 => r.Next(60, 80),
                    1 => r.Next(0, 70),
                    2 => r.Next(8) == 0 ? r.Next(-50, 65536) : r.Next(70, 900),
                    _ => 69 + r.Next(2),
                };
                p.WriteWord((ushort)(Stock3 + 8 * i), (ushort)stock);
                p.WriteDword((ushort)(L.Cash + 4 * i), (uint)(r.Next(6) == 0 ? r.Next(int.MinValue, int.MaxValue) : r.Next(0, 100000)));
            }
            if (r.Next(3) == 0) // a tie for the largest stock
                p.WriteWord((ushort)(Stock3 + 8 * r.Next(4)), p.ReadWord((ushort)(Stock3 + 8 * r.Next(4))));
            if (r.Next(2) == 0) p.WriteByte(Traders.Core.Field.FieldGrid.FreeFields, (byte)new[] { 0, 0, 0x80, 0xFF, 1 }[r.Next(5)]);
            h.Script = T.Script(r.Next(), r.Next(2) == 0);
            T.AssertSame(h, $"0B03 case {n} (week {week})", 0x0B03, (d, random, rec) => T.Drain(ContestPhase.Run(d, random, rec)));
            h.Script = null;
            if (h.Calls.Count == 0) none++;
            prizes += h.Calls.Count(c => c.StartsWith(nameof(FatMikeEventKind.Number), StringComparison.Ordinal));
            negative += h.Calls.Count(c => c.StartsWith("Number(41, ", StringComparison.Ordinal) && c.Split(", ")[2].StartsWith('-'));
            fields += h.Calls.Count(c => c == "SecondRound()");
            typed += h.Calls.Count(c => c.StartsWith(nameof(FatMikeEventKind.TypeCharacter), StringComparison.Ordinal));
        }
        Assert.True(prizes > 10 && negative > 2 && fields > 5 && none > 5 && typed > 0,
            $"prizes {prizes} (negative {negative}), fields {fields}, none {none}, typed {typed}");
    }
}
