using Traders.Core;
using Traders.Core.Ending;
using Traders.Core.Field;
using Traders.Core.Pixels;
using Traders.Core.State;
using Traders.RefEmu;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Tests.Ending;

/// <summary>
/// The end check 20B3:0005 with its rest from 20B3:0053 on (finding docs/routines/20B3_008E_ende.md) against the original,
/// with the drawing, pages, sound, report and key wait replaced (<see cref="EndScreensHarness"/>): the RefEmu runs the whole
/// routine, the port <see cref="EndCheck.Decide"/> and <see cref="EndScreens.Run"/> on the same data segment. Compared: AL,
/// the sequence of recorded calls with their arguments (pixel reads with the values the shared script answers), the number
/// and arguments of random() calls, DS:10D2 and the whole data segment outside the output's bytes. States: data segments of
/// a real game at the end check of weeks 0–3 (target set to 0, or F10 held), and set states – wealth with ties, negative
/// and at 16-bit borders, the counters DS:1080–1089 at their limits, display modes 1 and 2. Expected values only from the RefEmu.
/// </summary>
public class EndScreensDifferentialTests
{
    /// <summary>Data segments of one real game (four computer players) at the entry of 20B3:0005 in weeks 0–3.</summary>
    private static readonly Lazy<byte[][]> GameStates = new(() =>
    {
        var p = OriginalProgram.StartGame(TestGameData.DosDir);
        var states = new byte[4][];
        for (int week = 0; week < 4; week++)
        {
            Assert.True(p.RunToWeekStart(week, 900), $"week {week} not reached");
            Assert.True(p.RunToRoutine(EndScreensHarness.Seg, EndScreensHarness.Entry, 900), $"end check of week {week} not reached");
            states[week] = p.DataSegment();
        }
        return states;
    });

    public static TheoryData<int, bool, int> GameCases() => new()
    {
        { 0, false, 1 }, { 1, false, 2 }, { 2, false, 3 }, { 3, false, 4 }, { 0, true, 5 }, { 3, true, 6 },
    };

    // [DIFFERENTIAL] 20B3:0005, 20B3:008E, 015B, 0267, 0304, 034F, 03BF, 03E5, 0432, 047B, 04DE, 0525, 05B4, 069A, 07BA, 0863, 0986, 09D4, 0A9F, 0ADC, 0B40, 0B7D, 0BF0, 0C05, 0D0D, 0D65, 0D77, 21FD:0620, 0634
    // (the whole of 20B3:0005 with its rest 20B3:0053–0088, states of a real game)
    [GameDataTheory]
    [MemberData(nameof(GameCases))]
    public void End_screens_match_the_original_in_a_real_game(int week, bool f10, int seed)
    {
        var p = OriginalProgram.LoadOnly(TestGameData.DosDir);
        p.Machine.Mem.WriteBlock((uint)p.Dgroup << 4, GameStates.Value[week]);
        if (f10) p.WriteByte((ushort)(L.KeyState + EndCheck.EndKey), 1);
        else p.WriteDword((ushort)L.Target, 0);
        Check($"week {week}{(f10 ? " F10" : "")}", p, seed);
    }

    /// <summary>Wealth per player, display mode, counters DS:1080/1082/1084/1085/1087/1089 (null = as loaded), pixel seed.</summary>
    public static TheoryData<uint[], short, short[]?, int> SetCases() => new()
    {
        // a clear order; all at the target
        { new uint[] { 50_000, 40_000, 30_000, 20_000 }, 1, null, 11 },
        // a tie of two winners (two leave the field, two drinks), the others tied too
        { new uint[] { 70_000, 70_000, 5, 5 }, 1, null, 12 },
        // all tied: four winners
        { new uint[] { 30_000, 30_000, 30_000, 30_000 }, 1, null, 13 },
        // negative wealth and 16-bit borders; wealth with more than five digits
        { new uint[] { 0xFFFF_FFF0, 0x0000_FFFF, 0x0001_0000, 123_456 }, 1, null, 14 },
        { new uint[] { 0x7FFF_FFFF, 0x8000_0000, 99_999, 100_000 }, 1, null, 15 },
        // display mode 2 (target 6 in the reveal, the page copy flips)
        { new uint[] { 10, 20, 30, 40 }, 2, null, 16 },
        // the walk counters at their limits: walk already past its end (state 2 at once), walk at 0, short bows and drink
        { new uint[] { 1_000, 2_000, 3_000, 4_000 }, 1, new short[] { 0x1EC, 1, 3, 1, 100, 1 }, 17 },
        { new uint[] { 4_000, 3_000, 2_000, 1_000 }, 1, new short[] { 0, 6, 0, 11, 0, 11 }, 18 },
        { new uint[] { 4_000, 4_000, 2_000, 1_000 }, 1, new short[] { 0x43, 1, 2, 6, 99, 6 }, 19 },
    };

    // [DIFFERENTIAL] 20B3:0005 – whole, with the rest 20B3:0053–0088, set states
    [GameDataTheory]
    [MemberData(nameof(SetCases))]
    public void End_screens_match_the_original_in_set_states(uint[] wealth, short mode, short[]? counters, int seed)
    {
        var p = OriginalProgram.LoadOnly(TestGameData.DosDir);
        SetWealth(p, wealth);
        p.WriteDword((ushort)L.Target, 0);
        p.WriteWord((ushort)Planting.CardMode, (ushort)mode);
        p.WriteDword((ushort)L.RandomSeed, (uint)(0x1357_9BDF * seed));
        if (counters is { } c)
        {
            p.WriteWord(EndScreens.Walk, (ushort)c[0]);
            p.WriteWord(EndScreens.BlinkTimer, (ushort)c[1]);
            p.WriteByte(EndScreens.BlinkStep, (byte)c[2]);
            p.WriteWord(EndScreens.Bow1, (ushort)c[3]);
            p.WriteWord(EndScreens.Drink, (ushort)c[4]);
            p.WriteWord(EndScreens.Bow2, (ushort)c[5]);
        }
        Check($"set {seed}", p, seed);
    }

    /// <summary>
    /// Pixel script: with seed s, read n answers 2 or 6 (the targets) in about half of the reads, 1 in a fifth, else 9 or 0;
    /// after <see cref="EndScreensHarness.ReadLimit"/> reads only 9 (abort counter: the reveal then ends).
    /// </summary>
    private static Func<int, byte> Script(int seed) => n =>
    {
        if (n >= EndScreensHarness.ReadLimit) return 9;
        uint h = unchecked((uint)n * 0x9E37_79B9u + (uint)seed * 0x85EB_CA6Bu);
        h ^= h >> 15;
        h *= 0x2C1B_3C6D;
        h ^= h >> 12;
        return (h % 10) switch
        {
            < 3 => 2,
            < 5 => 6,
            < 7 => 1,
            8 => 0,
            _ => 9,
        };
    };

    private static void Check(string label, OriginalProgram p, int seed)
    {
        var h = new EndScreensHarness(p, Script(seed));
        var before = p.DataSegment().AsSpan(0, L.Size).ToArray();
        byte al = h.Run();
        var emu = p.DataSegment();

        var d = new Dgroup(before);
        var random = new OriginalRandom(new RecordedClock([]), d.U32(L.RandomSeed));
        var recorder = new EndScreensRecorder(Script(seed)) { State = d };
        bool ends = EndCheck.Decide(d);
        Assert.True(ends, $"{label}: the port's decision does not end the game");
        var page = new EndingPage(new RulePage());
        long frames = 0;
        foreach (var _ in EndScreens.Run(d, random, recorder, page)) frames++;
        d.Set(L.RandomSeed, random.Seed);

        Assert.Equal(1, al);
        Assert.True(h.PixelReads < EndScreensHarness.ReadLimit, $"{label}: the reveal needed the abort counter");
        Assert.Equal(h.PixelReads, recorder.PixelReads);
        Assert.True(h.RandomArgs.Count == random.Calls, $"{label}: original drew {h.RandomArgs.Count} times, port {random.Calls}");
        AssertSameCalls(label, h.Calls, recorder.Calls);
        EndScreensHarness.Compare(label, emu, d);
        Assert.True(frames > 0);
    }

    private static void AssertSameCalls(string label, List<string> original, List<string> port)
    {
        int n = Math.Min(original.Count, port.Count);
        for (int i = 0; i < n; i++)
            if (original[i] != port[i])
                Assert.Fail($"{label}: call {i} differs: original {original[i]}, port {port[i]} " +
                            $"(before: {string.Join("; ", original.Skip(Math.Max(0, i - 4)).Take(Math.Min(4, i)))})");
        Assert.True(original.Count == port.Count, $"{label}: original made {original.Count} calls, port {port.Count}");
    }

    /// <summary>Cash so that the original's 0825:06CB gives <paramref name="wealth"/> (as in EndCheckDifferentialTests).</summary>
    private static void SetWealth(OriginalProgram p, uint[] wealth)
    {
        for (int i = 0; i < 4; i++) p.WriteDword((ushort)(L.Cash + 4 * i), 0);
        p.Call(0x0825, 0x06CB);
        var rest = Enumerable.Range(0, 4).Select(i => p.ReadDword((ushort)(L.CurrentWealth + 4 * i))).ToArray();
        for (int i = 0; i < 4; i++) p.WriteDword((ushort)(L.Cash + 4 * i), unchecked(wealth[i] - rest[i]));
    }
}
