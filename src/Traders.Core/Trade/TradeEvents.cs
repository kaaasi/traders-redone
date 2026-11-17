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

    /// <summary>186C:0002(picture) – loads the picture of the trade ([08D6] V-AHANDE, [08D8] V-BHANDE).</summary>
    LoadImage,

    /// <summary>21FD:05DB(font) – text font of the driver.</summary>
    SetFont,

    /// <summary>2295:0062(c) – fill character of numbers DS:1126 (the core writes it too).</summary>
    NumberFill,

    /// <summary>0A96:001B – empties the drawing list.</summary>
    ClearList,

    /// <summary>0A96:06A8 – executes the drawing list.</summary>
    RunList,

    /// <summary>0A96:0C3F(page DS:082E) – copies the page; flips first on cards 2 and 63h (the core yields those frames).</summary>
    ShowPage,

    /// <summary>21FD:051F – page flip (frames in the core).</summary>
    FlipPages,

    /// <summary>21FD:0665 – last frame: flip, restore, list, objects (frames in the core).</summary>
    LastFrame,

    /// <summary>21FD:0185(low, high) – retraces (frames in the core).</summary>
    Delay,

    /// <summary>21FD:0329 – fade in, three retraces per stage (frames in the core).</summary>
    FadeIn,

    /// <summary>21FD:0353 – fade in, one retrace per stage (frames in the core).</summary>
    FadeInFast,

    /// <summary>21FD:03B4(rgb12) – fade out to the colour, two retraces per stage (frames in the core).</summary>
    FadeOutFast,

    /// <summary>0B9F:080C – opening transition of a ware (frames in the core).</summary>
    Transition,

    /// <summary>0C29:0635 – scale and frame of the picture with the base price (0C29:065C or 0739, 0B1F).</summary>
    DrawBoard,

    /// <summary>0C29:109F(player DS:37AA) – stock of the player (columns in picture A, bars 0C29:134D in picture B).</summary>
    DrawStock,

    /// <summary>0C29:0B60(player DS:37AA) – need bar of the player.</summary>
    DrawNeed,

    /// <summary>0C29:0E01 – markers and prices of all players on the scale.</summary>
    DrawMarkers,

    /// <summary>0C29:0B1F(base price) – base price on the scale (only when DS:455A ≠ DS:2BE4 in the bidding).</summary>
    DrawBasePrice,

    /// <summary>0C29:0CC1 – countdown bar.</summary>
    DrawCountdown,

    /// <summary>0C29:0D6A – one step of the countdown bar.</summary>
    CountdownStep,

    /// <summary>0C29:04BD – colours of the trade in the base palette.</summary>
    TradePalette,
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

    /// <summary>21FD:0245 – pause check, frame by frame (<see cref="Field.Planting.Pause"/> in the game).</summary>
    IEnumerable<Field.FieldWait> PauseCheck(Dgroup d);
}

/// <summary>
/// The trading computer 141D (lane handelski, finding docs/routines/141D_0008_handelski.md): the entry points the trade calls,
/// each with the caller's random generator. Their results are in the data segment; only 141D:0397 also leaves a word on the
/// stack that its next call may read (<see cref="ChooseRole"/>). The player concerned is DS:37AA where the trade calls them
/// inside a player loop. In the game: <see cref="TradeComputer"/>.
/// </summary>
public interface ITradeAi
{
    /// <summary>141D:0008 – once before the four trades (0B9F:0002).</summary>
    void Prepare(Dgroup d, OriginalRandom random);

    /// <summary>141D:021A – per player while the panels are drawn (0C29:0086).</summary>
    void EnterTrade(Dgroup d, OriginalRandom random);

    /// <summary>
    /// 141D:0397 – per computer and frame of the role choice (0B9F:00FE). <paramref name="stackWord"/> is the word the routine
    /// finds under its uninitialised local (<see cref="TradeRules.ChooseRoles"/>); returns the word it leaves there (the same
    /// word if it does not write the local).
    /// </summary>
    short ChooseRole(Dgroup d, OriginalRandom random, short stackWord);

    /// <summary>141D:0464 – once before the bidding if there is a seller (0C29:0046).</summary>
    void StartBidding(Dgroup d, OriginalRandom random);

    /// <summary>141D:09F3 – per computer and frame of the bidding (0C29:01B7).</summary>
    void Bid(Dgroup d, OriginalRandom random);

    /// <summary>141D:0E67 – after a sale, player DS:37AA = the seller (0C29:03B2).</summary>
    void AfterSale(Dgroup d, OriginalRandom random);

    /// <summary>141D:0CCA(price) – after a purchase, player DS:37AA = the buyer (0C29:03B2).</summary>
    void AfterPurchase(Dgroup d, OriginalRandom random, short price);
}
