using Traders.Core.State;

namespace Traders.Core.Trade;

/// <summary>
/// Drawing and sound requests of the trade (finding docs/routines/0B9F_0002_handel.md). Requests of the trade's own drawing
/// routines (segment 0C29) carry the data segment values the routine reads that the rules change later in the same frame; the
/// scene draws from them. None of these requests changes game state.
/// </summary>
public enum TradeEventKind
{
    /// <summary>21A7:0054(channel, sound) – sound request.</summary>
    Sound,

    /// <summary>0A96:04D7(x, y, w, h, colour) – filled rectangle (command type 11).</summary>
    FillRect,

    /// <summary>0C29:0F68(player, role, position) – clears the marker of the player at its old place on the scale.</summary>
    ClearMarker,

    /// <summary>0C29:09BB(player, role, cash low, cash high, stock) – cash and stock of the player in its panel.</summary>
    DrawPlayerNumbers,

    /// <summary>
    /// 0C29:1108(direction, player, role, stock, shown stock DS:0634) – stock display of the player after a deal (−1 sold, 1
    /// bought).
    /// </summary>
    DrawStockChange,
}

/// <summary>A request of the trade with its arguments in call order.</summary>
public sealed record TradeEvent(TradeEventKind Kind, params short[] Args)
{
    public override string ToString() => $"{Kind}({string.Join(", ", Args)})";
}

/// <summary>What the trade needs from outside the core: the outputs (scene, sound) and the controls 0E9E:037A.</summary>
public interface ITradeHost
{
    void Emit(TradeEvent e);

    /// <summary>0E9E:037A – reads the controls and writes the actions DS:3308 (ControlReader.ReadActions).</summary>
    void ReadControls(Dgroup d);
}

/// <summary>
/// The trading computer 141D (lane handelski, finding docs/routines/141D_0008_handelski.md): the entry points the trade calls,
/// each with the caller's random generator. All of them return nothing; their results are in the data segment. The player
/// concerned is DS:37AA where the trade calls them inside a player loop.
/// </summary>
public interface ITradeAi
{
    /// <summary>141D:0008 – once before the four trades (0B9F:0002).</summary>
    void Prepare(Dgroup d, OriginalRandom random);

    /// <summary>141D:021A – per player while the panels are drawn (0C29:0086).</summary>
    void EnterTrade(Dgroup d, OriginalRandom random);

    /// <summary>141D:0397 – per computer and frame of the role choice (0B9F:00FE).</summary>
    void ChooseRole(Dgroup d, OriginalRandom random);

    /// <summary>141D:0464 – once before the bidding if there is a seller (0C29:0046).</summary>
    void StartBidding(Dgroup d, OriginalRandom random);

    /// <summary>141D:09F3 – per computer and frame of the bidding (0C29:01B7).</summary>
    void Bid(Dgroup d, OriginalRandom random);

    /// <summary>141D:0E67 – after a sale, player DS:37AA = the seller (0C29:03B2).</summary>
    void AfterSale(Dgroup d, OriginalRandom random);

    /// <summary>141D:0CCA(price) – after a purchase, player DS:37AA = the buyer (0C29:03B2).</summary>
    void AfterPurchase(Dgroup d, OriginalRandom random, short price);
}
