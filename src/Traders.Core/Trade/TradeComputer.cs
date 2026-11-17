using Traders.Core.Ai;
using Traders.Core.State;

namespace Traders.Core.Trade;

/// <summary>
/// The trading computer of the game: the seven entries of 141D the trade calls (<see cref="ITradeAi"/>) on the port of lane
/// handelski, <see cref="TradeAi"/> (finding docs/routines/141D_0008_handelski.md, "Vertrag"). Only 141D:0397 and 141D:09F3
/// draw random numbers; they get the caller's generator.
/// </summary>
public sealed class TradeComputer : ITradeAi
{
    public static TradeComputer Instance { get; } = new();

    // 0B9F:000A call 141D:0008
    public void Prepare(Dgroup d, OriginalRandom random) => TradeAi.Budgets(d);

    // 0C29:00D0 call 141D:021A
    public void EnterTrade(Dgroup d, OriginalRandom random) => TradeAi.Demand(d);

    // 0B9F:011B call 141D:0397
    public short ChooseRole(Dgroup d, OriginalRandom random, short stackWord) => TradeAi.ChooseSide(d, random, stackWord);

    // 0C29:0075 call 141D:0464
    public void StartBidding(Dgroup d, OriginalRandom random) => TradeAi.PlanBids(d);

    // 0C29:0284 call 141D:09F3
    public void Bid(Dgroup d, OriginalRandom random) => TradeAi.Act(d, random);

    // 0C29:0445 call 141D:0E67
    public void AfterSale(Dgroup d, OriginalRandom random) => TradeAi.Sold(d);

    // 0C29:04AA call 141D:0CCA(price)
    public void AfterPurchase(Dgroup d, OriginalRandom random, short price) => TradeAi.Bought(d, price);
}
