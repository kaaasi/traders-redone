using static Traders.Tests.Trade.TradeFrameHarness;

namespace Traders.Tests.Trade;

/// <summary>
/// Lock step of the trade 0B9F:0002 with the original in a running game (<see cref="TradeFrameHarness"/>): data segment,
/// DS:10D2, random() calls and the frame count per frame, for week 0 and week 2, four computers (141D replaced) and with two
/// humans pressing keys; counter-test with a frame processed twice.
/// </summary>
public class TradeFrameDifferentialTests
{
    // scancodes: up 48h, down 50h, Ins 52h (control 4)
    private const byte Up = 0x48, Down = 0x50, Fire = 0x52;

    private static readonly Press[] Keys =
    [
        new(150, 20, Up), new(400, 15, Down), new(700, 30, Up), new(1100, 10, Down),
        // the bidding: buyers climb, sellers descend, one fire
        new(1400, 300, Up), new(1800, 200, Down), new(2300, 4, Fire), new(2600, 400, Up), new(3200, 300, Down),
    ];

    // [DIFFERENTIAL] 0B9F:0002, 0C29:0046, 0C29:0086, 0C29:015C, 0C29:01B7
    [GameDataFact]
    public void Week_0_computers_in_lock_step()
    {
        var r = Run(new Script(0, [], []));
        Assert.True(r.Retraces > 0);
    }

    // [DIFFERENTIAL] 0B9F:0002, 0C29:015C, 0C29:01B7
    [GameDataFact]
    public void Week_2_with_two_humans_in_lock_step()
    {
        var r = Run(new Script(2, [0, 2], Keys));
        Assert.True(r.Retraces > 0);
    }

    [GameDataFact]
    public void A_frame_processed_twice_is_found()
    {
        var e = Record.Exception(() => Run(new Script(0, [], []), Mutation.DoubleFrame));
        Assert.NotNull(e);
    }
}
