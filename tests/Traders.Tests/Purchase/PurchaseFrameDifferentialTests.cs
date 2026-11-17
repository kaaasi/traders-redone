using Traders.Core.State;
using Xunit.Abstractions;
using static Traders.Tests.Purchase.PurchaseFrameHarness;

namespace Traders.Tests.Purchase;

/// <summary>
/// Lock step of the purchase 08CF:0004 with the original in a running game (<see cref="PurchaseFrameHarness"/>): data segment,
/// DS:10D2, random() calls, the frame count, the picture of the scene and the command lists per frame, for week 0 (four
/// computers; two humans in the gem shop) and week 2 (two humans in the lotto, oil and rocket shops); counter-tests with a
/// frame processed twice, a countdown tick lost and drawing requests of the scene left out.
/// </summary>
public class PurchaseFrameDifferentialTests
{
    private readonly ITestOutputHelper _out;

    public PurchaseFrameDifferentialTests(ITestOutputHelper output) => _out = output;

    private void Report(Result r)
    {
        _out.WriteLine($"retraces {r.Retraces}, passes {r.Passes}, random {r.RandomCalls}, pictures {r.PicturesCompared} " +
                       $"({r.DistinctPictures} distinct), lists {r.ListsCompared} ({r.NonEmptyLists} with entries)");
        _out.WriteLine(string.Join(" ", r.Sounds.OrderBy(s => s.Key).Select(s => $"({s.Key.Channel},{s.Key.Sound:X2})x{s.Value}")));
        _out.WriteLine(string.Join(" ", r.Log));
        _out.WriteLine(string.Join(" ", r.Kinds.OrderBy(k => k.Key).Select(k => $"{k.Key}x{k.Value}")));
    }

    // [DIFFERENTIAL] 08CF:0004, 08CF:00CD, 08CF:01BD
    // [DIFFERENTIAL] 08CF:15C6, 08CF:15F5, 08CF:1688, 08CF:16B4, 08CF:16F5, 08CF:172A, 08CF:1859, 08CF:18B5, 08CF:1935
    // [DIFFERENTIAL] 08CF:193E, 08CF:1981 (scene: picture and command lists per frame)
    [GameDataFact]
    public void Week_0_computers_in_lock_step()
    {
        var r = Run(new Script(0, [], []));
        Report(r);
        Common(r);
        // the computers enter the lotto and pick four numbers each, sell gems into the account and leave
        Assert.Contains(r.Log, e => e.EndsWith(":p1:2D") || e.EndsWith(":p1:2E"));
        Assert.True(r.Sounds.GetValueOrDefault(((short)0, (short)0x2C)) > 0, "no gem sold into the account");
        Assert.True(r.Sounds.GetValueOrDefault(((short)0, (short)0x51)) > 0, "nobody left a shop");
    }

    // scancodes (control 4): right 4Dh, left 4Bh, up 48h, down 50h, Ins 52h
    private const byte Right = 0x4D, Left = 0x4B, Up = 0x48, Down = 0x50, Fire = 0x52;

    private static Press Tap(long from, byte key) => new(from, 6, key);

    /// <summary>
    /// Both humans: lotto (enter, taps for picks), then right along the street into the shops with purchases (oil, rockets) and
    /// out again, back to the lotto at the end.
    /// </summary>
    private static readonly Press[] Walk =
    [
        Tap(10, Up), Tap(40, Left), Tap(70, Right), Tap(100, Left), Tap(130, Right),
        Tap(170, Right), Tap(330, Up), Tap(360, Fire), Tap(390, Right), Tap(420, Fire), Tap(450, Down),
        Tap(480, Right), Tap(640, Up), Tap(670, Fire), Tap(700, Down),
        Tap(730, Right), Tap(890, Up), Tap(920, Left), Tap(950, Right), Tap(980, Fire), Tap(1010, Down),
        Tap(1040, Left), Tap(1200, Left), Tap(1350, Left), Tap(1510, Up),
    ];

    // [DIFFERENTIAL] 08CF:0004, 08CF:00CD, 08CF:01BD
    [GameDataFact]
    public void Week_2_with_two_humans_in_lock_step()
    {
        var r = Run(new Script(2, [0, 2], Walk));
        Report(r);
        Common(r);
        // the humans enter the lotto and pick, buy in two shops and leave them
        Assert.Contains(r.Log, e => e.EndsWith(":p0:2E"));
        Assert.True(r.Log.Count(e => e.EndsWith(":p0:52")) >= 2, "player 0 bought less than twice");
        Assert.True(r.Log.Count(e => e.EndsWith(":p2:51")) >= 2, "player 2 left less than two shops");
    }

    /// <summary>
    /// Both humans hold right to the gem shop (a held key starts the next walk), sell a gem into the account, buy one back,
    /// sell one for cash and leave (the account of 500 lets them buy).
    /// </summary>
    private static readonly Press[] Gems =
    [
        new(120, 640, Right), Tap(800, Up), new(860, 12, Left), new(930, 12, Right), Tap(1000, Fire),
        Tap(1060, Down), Tap(1120, Left),
    ];

    // [DIFFERENTIAL] 08CF:0004, 08CF:01BD
    [GameDataFact]
    public void Week_0_with_two_humans_in_the_gem_shop_in_lock_step()
    {
        var r = Run(new Script(0, [0, 2], Gems, [(Traders.Core.State.DgroupLayout.GemCredit, [0xF4, 0x01, 0, 0, 0xF4, 0x01])]));
        Report(r);
        Common(r);
        // both humans sell a gem into the account, buy one back and sell one for cash (one gem less)
        Assert.Contains(r.Log, e => e.EndsWith(":p0:2C"));
        Assert.Contains(r.Log, e => e.EndsWith(":p2:2B"));
        Assert.Contains(r.Log, e => e.EndsWith(":p0:52"));
        Assert.True((sbyte)r.DataSegment[DgroupLayout.Gems] < (sbyte)r.Entry[DgroupLayout.Gems], "player 0 has not fewer gems");
    }

    /// <summary>
    /// Every run: the shop loop 08CF:01BD runs DS:08F8 · 28h passes (the countdown), the clock is redrawn at each of its steps
    /// (and once in the set-up), all four balls of a draw fall; the picture was compared in every frame and at the return, and
    /// the original showed many different pictures.
    /// </summary>
    private static void Common(Result r)
    {
        Assert.Equal(r.Retraces + 1, r.PicturesCompared);
        Assert.True(r.DistinctPictures > r.Passes / 2, $"only {r.DistinctPictures} distinct pictures");
        Assert.True(r.NonEmptyLists >= r.Kinds.GetValueOrDefault("DrawClock"), $"only {r.NonEmptyLists} lists with entries");
        int limit = r.Entry[DgroupLayout.TimeLimit];
        Assert.Equal(limit * 0x28, r.Passes);
        Assert.Equal(limit + 1, r.Kinds.GetValueOrDefault("DrawClock"));
        Assert.True(r.Sounds.GetValueOrDefault(((short)1, (short)0x2F)) + r.Sounds.GetValueOrDefault(((short)1, (short)0x30)) >= 4,
            "fewer than four balls drawn");
        Assert.True(r.Retraces > r.Passes * 3, $"{r.Retraces} frames for {r.Passes} passes");
    }

    /// <summary>
    /// Counter-tests (four computers, week 0), each with the first line of the failure it must cause: a frame processed twice,
    /// a countdown tick that does not count; scene: a clock, a cash display and a lotto number left out.
    /// </summary>
    [GameDataTheory]
    [InlineData(nameof(Mutation.DoubleFrame), "after retrace 300 (port frame 301, pass 56): the port executes list 111 of the flow, the original has none there")]
    [InlineData(nameof(Mutation.CountdownHeld), "after retrace 879 (port frame 880, pass 200): list 400 of the flow: original 3 entries, port 0")]
    [InlineData(nameof(Mutation.SkipClock), "after retrace 399 (port frame 400, pass 80): list 160 of the flow: original 3 entries, port 0")]
    [InlineData(nameof(Mutation.SkipCash), "after retrace 83 (port frame 84, pass 1): list 2 of the flow: original 33 entries, port 32")]
    [InlineData(nameof(Mutation.SkipTicket), "after retrace 83 (port frame 84, pass 1): list 2 of the flow: original 33 entries, port 32")]
    public void Mutations_are_detected(string mutation, string expected)
    {
        // the original files must be there: a run that fails for want of them proves nothing (pflichtlauf without data)
        Assert.True(TestGameData.Available, $"original files missing ({TestGameData.DosDir})");
        var e = Record.Exception(() => Run(new Script(0, [], []), Enum.Parse<Mutation>(mutation)));
        Assert.NotNull(e);
        string first = e.Message.Split('\n')[0];
        _out.WriteLine(first);
        Assert.Contains(expected, first);
    }
}
