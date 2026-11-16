using Traders.Core.State;

namespace Traders.Core.Trade;

/// <summary>
/// Data segment addresses of the trade 0B9F:0002 / 0C29 (finding docs/routines/0B9F_0002_handel.md, docs/analyse/DATENLAYOUT.md
/// section of lane handel). The trade runs once per ware: every player is a buyer or a seller on a vertical price scale; a
/// position p stands for the price p / 2 + base price. Buyers climb, sellers descend; when the highest buyer meets the lowest
/// seller, ten units change hands.
/// </summary>
public static class TradeLayout
{
    /// <summary>word, ware being traded (first argument of 0C29:0001; the global loop variable).</summary>
    public const int Ware = DgroupLayout.LoopWare;

    /// <summary>byte, picture variant: 0 = V-AHANDE, 1 = V-BHANDE (second argument of 0C29:0001).</summary>
    public const int Variant = 0x2BC5;

    /// <summary>sbyte, player loop variable.</summary>
    public const int Player = DgroupLayout.ActivePlayer;

    /// <summary>byte[4], role of the player: <see cref="Buyer"/>, <see cref="Seller"/>, <see cref="Withdrawn"/>.</summary>
    public const int Role = 0x43B2;

    /// <summary>word[4], position of the player on the scale, −3 (buyer's floor) … 67h (seller's ceiling).</summary>
    public const int Position = 0x43A2;

    /// <summary>dword[4], need of the player for the ware (0B9F:009E; drawn as a bar, not used by the rules of the trade).</summary>
    public const int Need = 0x43B6;

    /// <summary>sbyte[4], action of the player from 0E9E:037A (<see cref="Input.ControlAction"/>).</summary>
    public const int Action = 0x3308;

    /// <summary>word, base price: position 0 costs this much.</summary>
    public const int BasePrice = 0x455A;

    /// <summary>word, base price at the start (ware price DS:4898).</summary>
    public const int StartPrice = DgroupLayout.AuctionStartPrice;

    /// <summary>word, highest price paid by a buyer (0 = none).</summary>
    public const int BestPrice = DgroupLayout.AuctionBestBid;

    /// <summary>word, number of sellers (0C29:051A, 0B9F:01E1, 0C29:0FB9); the bidding ends when it is 0.</summary>
    public const int Sellers = 0x2BBC;

    /// <summary>byte, 1 = no human buyer (0B9F:01E1).</summary>
    public const int NoHumanBuyer = 0x439C;

    /// <summary>byte, 1 = no human seller (0B9F:01E1).</summary>
    public const int NoHumanSeller = 0x450C;

    /// <summary>byte, number of buyers at the highest buyer position (0B9F:02BE).</summary>
    public const int TopBuyers = 0x2BB8;

    /// <summary>byte, 1 = no human among the buyers at the highest buyer position (0B9F:02BE).</summary>
    public const int NoHumanTopBuyer = 0x2BC0;

    /// <summary>byte, 1 = a player moved in this frame (0B9F:0321).</summary>
    public const int Moved = 0x2BC4;

    /// <summary>byte, <see cref="Moved"/> of the previous frame (0B9F:0549 sound).</summary>
    public const int MovedBefore = 0x023E;

    /// <summary>byte[4], 1 = the seller pressed fire or ran out of goods and leaves (0B9F:0321, 0C29:0FB9).</summary>
    public const int Leaving = 0x2BC6;

    /// <summary>word, highest buyer position (at least 0).</summary>
    public const int HighestBuyer = 0x3D1C;

    /// <summary>word, lowest seller position (at most 66h).</summary>
    public const int LowestSeller = 0x43EE;

    /// <summary>byte, frames without a deal after the extremes changed (3).</summary>
    public const int Settling = 0x3D18;

    /// <summary>byte, frames until the next deal at the same price.</summary>
    public const int DealTimer = 0x3CAE;

    /// <summary>byte, delay between two deals at the same price (Eh, shrinking to 5).</summary>
    public const int DealDelay = 0x3D46;

    /// <summary>byte, player bits DS:08C6 of the two parties of the last deal (drawn highlighted).</summary>
    public const int DealParties = 0x2BCC;

    /// <summary>word, seller of the deal (0B9F:060A).</summary>
    public const int DealSeller = 0x44F2;

    /// <summary>word, buyer of the deal, <see cref="NoBuyer"/> = none (0B9F:060A).</summary>
    public const int DealBuyer = 0x4106;

    /// <summary>word, round-robin cursors of the seller and the buyer search (0B9F:060A).</summary>
    public const int SellerCursor = 0x2BB4, BuyerCursor = 0x2BB6;

    /// <summary>word, buyer pushes until the base price rises (0B9F:04FC; init 2).</summary>
    public const int PushCounter = 0x023C;

    /// <summary>word, countdown value at the start or after the last deal (0B9F:024A, 0B9F:0695).</summary>
    public const int CountdownMark = 0x2BBA;

    /// <summary>word[4] x and y of the player's panel, word x of the scale, x of the countdown, x and y of the label (0C29:051A).</summary>
    public const int PanelX = 0x2BD0, PanelY = 0x2BDA, ScaleX = 0x2BCE, CountdownX = 0x2BCA, LabelX = 0x2BD8, LabelY = 0x2BE2;

    /// <summary>word, base price last drawn (0C29:0B1F).</summary>
    public const int ShownBasePrice = 0x2BE4;

    /// <summary>word[4], stock / 10 last drawn as a bar in picture B (0C29:134D).</summary>
    public const int ShownStock = 0x0634;

    /// <summary>byte, colour of the bars in picture B, DAh on graphics card 1 (0C29:134D).</summary>
    public const int BarColour = 0x0642;

    /// <summary>Byte tables: panel y per variant (DS:0258 buyers, DS:025A sellers).</summary>
    public const int BuyerPanelY = 0x0258, SellerPanelY = 0x025A;

    /// <summary>Byte[8] player bits (01h, 02h, …) of DS:08C6.</summary>
    public const int PlayerBits = 0x08C6;

    /// <summary>Roles DS:43B2.</summary>
    public const byte Buyer = 0, Seller = 1, Withdrawn = 6;

    /// <summary>Buyer index of a deal without buyer.</summary>
    public const short NoBuyer = 10;

    /// <summary>Lot size: units per deal; a seller needs at least this many.</summary>
    public const short Lot = 10;

    /// <summary>Buyer's floor and start position, seller's start position and ceiling.</summary>
    public const short BuyerFloor = -3, SellerStart = 0x67;

    /// <summary>Address of a word or dword of player <paramref name="p"/> (16-bit offset arithmetic as in the original).</summary>
    internal static int Word(int table, int p) => Int16Math.U(table + Int16Math.S(p << 1));

    internal static int Dword(int table, int p) => Int16Math.U(table + Int16Math.S(p << 2));

    internal static int Byte(int table, int p) => Int16Math.U(table + p);

    /// <summary>Stock DS:3CF4 of player <paramref name="p"/> for the ware DS:45D0 (p·8 + ware·2).</summary>
    internal static int Stock(Dgroup d, int p) =>
        Int16Math.U(DgroupLayout.Stock + Int16Math.S(Int16Math.S(p << 3) + Int16Math.S(d.S16(Ware) << 1)));
}
