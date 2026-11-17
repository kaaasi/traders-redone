using Xunit.Abstractions;
using static Traders.Tests.Trade.TradeFrameHarness;
using Kind = Traders.Core.Trade.TradeEventKind;

namespace Traders.Tests.Trade;

/// <summary>
/// Lock step of the trade 0B9F:0002 with the original in a running game (<see cref="TradeFrameHarness"/>): data segment,
/// DS:10D2, random() calls, the frame count and the picture of the scene per frame (and the command lists of the flow), for week 0 and week 2, with the trading computer 141D of the original
/// against <see cref="Traders.Core.Trade.TradeComputer"/> (four computers; two humans pressing keys) and with 141D replaced on
/// both sides; counter-tests with a frame processed twice, a wrong stack word for 141D:0397 and requests the scene does not draw.
/// </summary>
public class TradeFrameDifferentialTests
{
    private readonly ITestOutputHelper _out;

    public TradeFrameDifferentialTests(ITestOutputHelper output) => _out = output;

    // scancodes: up 48h, down 50h, Ins 52h (control 4)
    private const byte Up = 0x48, Down = 0x50, Fire = 0x52;

    private static readonly Press[] Keys =
    [
        new(150, 20, Up), new(400, 15, Down), new(700, 30, Up), new(1100, 10, Down),
        // the bidding: buyers climb, sellers descend, one fire
        new(1400, 300, Up), new(1800, 200, Down), new(2300, 4, Fire), new(2600, 400, Up), new(3200, 300, Down),
    ];

    // [DIFFERENTIAL] 0B9F:0002, 0C29:0046, 0C29:0086, 0C29:015C, 0C29:01B7, 0B9F:00FE
    // Scene (picture of every frame against the original, both pictures V-AHANDE and V-BHANDE):
    // [DIFFERENTIAL] 0B9F:080C, 0C29:04BD, 0C29:0635, 0C29:065C, 0C29:0739, 0C29:09BB, 0C29:0B1F, 0C29:0B60, 0C29:0BD9, 0C29:0C8A
    // [DIFFERENTIAL] 0C29:0CC1, 0C29:0D6A, 0C29:0E01, 0C29:0F68, 0C29:1050, 0C29:109F, 0C29:1108, 0C29:1163, 0C29:127E, 0C29:134D, 0C29:146B
    [GameDataFact]
    public void Week_0_computers_in_lock_step()
    {
        var r = Run(new Script(0, [], []));
        Assert.True(r.Retraces > 0);
        Assert.True(r.Deals > 0 && r.Biddings > 0, $"deals {r.Deals}, biddings {r.Biddings}");
        Scene(r);
        // 141D:0397 stored the stack word as the action: left by an earlier 0397 and by 0C29:0F68 (main's SI)
        Assert.Contains(r.StackWordUses.Keys, k => k.Source == "0397" && k.Word is 2 or 3);
        Assert.Contains(r.StackWordUses.Keys, k => k == ("0F68", Traders.Core.Trade.TradeRules.MainArgumentCount));
    }

    // #205 R7H-6: weeks 1 and 3 with the real trading computer 141D and the picture, as weeks 0 and 2 above
    // [DIFFERENTIAL] 0B9F:0002, 0C29:0046, 0C29:0086, 0C29:015C, 0C29:01B7, 0B9F:00FE
    [GameDataFact]
    public void Week_1_computers_in_lock_step()
    {
        var r = Run(new Script(1, [], []));
        Assert.True(r.Retraces > 0);
        Assert.True(r.Deals > 0 && r.Biddings > 0, $"deals {r.Deals}, biddings {r.Biddings}");
        Scene(r);
    }

    // [DIFFERENTIAL] 0B9F:0002, 0C29:015C, 0C29:01B7, 0B9F:00FE
    [GameDataFact]
    public void Week_3_with_two_humans_in_lock_step()
    {
        var r = Run(new Script(3, [0, 3], Keys));
        Assert.True(r.Retraces > 0);
        Scene(r);
    }

    // [DIFFERENTIAL] 0B9F:0002, 0C29:015C, 0C29:01B7, 0B9F:00FE
    [GameDataFact]
    public void Week_2_with_two_humans_in_lock_step()
    {
        var r = Run(new Script(2, [0, 3], Keys));
        Assert.True(r.Retraces > 0);
        Scene(r);
        // player 3 is human: 0E9E:037A leaves his action word, and 0397 stored it for a computer
        Assert.Contains(r.StackWordUses.Keys, k => k.Source == "037A");
    }

    // [DIFFERENTIAL] 0B9F:00FE
    [GameDataFact]
    public void Week_2_with_a_human_holding_up_in_lock_step()
    {
        // both humans hold up from frame 100 on: 0E9E:037A leaves player 3's action 2, and 0397 of player 1 stores it
        Press[] keys = [new(100, 1200, Up), .. Keys.Skip(4)];
        var r = Run(new Script(2, [0, 3], keys));
        Assert.True(r.Retraces > 0);
        Assert.Contains(r.StackWordUses.Keys, k => k.Source == "037A" && k.Word != -1);
    }

    // [DIFFERENTIAL] 0B9F:0002, 0C29:015C, 0C29:01B7
    [GameDataFact]
    public void Week_2_with_two_humans_and_the_computer_replaced_in_lock_step()
    {
        var r = Run(new Script(2, [0, 2], Keys, Ai: false));
        Assert.True(r.Retraces > 0);
    }

    /// <summary>
    /// The scene was compared: a picture per frame (and at the return), many different ones, the command lists of the flow, and
    /// every drawing request of the trade occurred (both pictures: four wares).
    /// </summary>
    private static void Scene(Result r)
    {
        Assert.Equal(r.Retraces + 1, r.PicturesCompared);
        Assert.True(r.DistinctPictures > 500, $"{r.DistinctPictures} different pictures");
        Assert.True(r.ListsCompared > 1000 && r.NonEmptyLists > 20, $"lists {r.ListsCompared}, non-empty {r.NonEmptyLists}");
        Assert.Equal(4, r.Kinds.GetValueOrDefault(Kind.LoadImage));
        foreach (var kind in new[]
                 {
                     Kind.Transition, Kind.TradePalette, Kind.FadeIn, Kind.FadeInFast, Kind.FadeOutFast, Kind.DrawBoard, Kind.DrawStock,
                     Kind.DrawNeed, Kind.DrawPlayerNumbers, Kind.DrawMarkers, Kind.ClearMarker, Kind.DrawCountdown, Kind.CountdownStep,
                     Kind.DrawStockChange, Kind.FillRect, Kind.ShowPage, Kind.LastFrame, Kind.SetFont, Kind.NumberFill,
                 })
            Assert.True(r.Kinds.GetValueOrDefault(kind) > 0, $"no request {kind}");
    }

    /// <summary>
    /// Counter-tests (four computers, week 0), each with the first line of the failure it must cause: a frame processed
    /// twice, a lost stack word or a wrong SI in the role choice 141D:0397, requests the scene does not draw.
    /// </summary>
    [GameDataTheory]
    [InlineData(nameof(Mutation.DoubleFrame), "the port executes list 361 of the flow, the original has none there")]
    [InlineData(nameof(Mutation.StackWordLost), "DS:330B original 03, port FF")]
    [InlineData(nameof(Mutation.SiZero), "DS:330B original 01, port 00")]
    [InlineData(nameof(Mutation.SkipMarkers), "plane 0 offset 4415")]
    [InlineData(nameof(Mutation.SkipCountdownStep), "list 124 of the flow: original 1 entries, port 0")]
    [InlineData(nameof(Mutation.SkipNumbers), "list 853 of the flow: original 2 entries, port 0")]
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
