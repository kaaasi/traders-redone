namespace Traders.Core.Ai;

/// <summary>
/// DGROUP offsets of the trade AI 141D (docs/routines/141D_0008_handelski.md). Names are tentative; the formulas are in the
/// finding. Offsets shared with other modules (cash, stock, prices, loop variables) stay in <see cref="State.DgroupLayout"/>.
/// "[4]" arrays are indexed by player, "[4][4]" by player and price level or ware.
/// </summary>
public static class TradeAiLayout
{
    // Written by 141D
    public const int Budget = 0x2D3E;          // dword[4]       cash / (150 - (10000 - cash) / 83) (0029); low word = base bid
    public const int Need = 0x2D78;            // word[4]        wanted units of the current ware, at least 0 (021A)
    public const int Reserve = 0x2D64;         // word[4]        price limit (computer) or 30 (human); 0 when Surplus >= 0
    public const int Surplus = 0x2D80;         // word[4]        -(Need + Reserve) before the clamp, at least 0 (021A)
    public const int WareBudget = 0x2D88;      // dword[4][4]    stride 16: cash share per ware (008C)
    public const int BidLevel = 0x2C7E;        // dword[4][4]    stride 16: bids base+step, base, base-step, ware price (04A5)
    public const int BidUnits = 0x2CFE;        // word[4][4]     stride 8: units planned per bid level (05A3)
    public const int CurrentLevel = 0x2D6C;    // word[4]        first level with units, 4 = none (05A3, 0CCA)
    public const int CurrentBid = 0x2D50;      // dword[4]       bid of the current level, 0 = none (05A3, 0CCA)
    public const int SortedBid = 0x2CBE;       // dword[16]      bids of all buyers sorted by rank (0806)
    public const int SortedUnits = 0x2D1E;     // word[16]       units of SortedBid (0806, 0E67)
    public const int SortedIndex = 0x2D74;     // word           first sorted entry with units, 16 = none (0806, 0BFD, 0E67)
    public const int AskFloor = 0x2D60;        // dword          SortedBid[SortedIndex], 1 when SortedIndex > 15 (0ED7)
    public const int SellerSurplus = 0x2D76;   // word           sum of Surplus over the sellers (0F01)
    public const int HighestBid = 0x2DC8;      // dword          highest CurrentBid of a computer buyer (0E09)

    // Read by 141D, written by the trade 0B9F/0C29 or the countdown 21FD
    public const int Side = 0x43B2;            // byte[4]        0 = buyer, 1 = seller (0B9F:00FE, 0C29:0086)
    public const int Position = 0x43A2;        // word[4]        position on the trade screen (0B9F:00FE: -3 buyer, 103 seller)
    public const int MarketPrice = 0x455A;     // word           starting price of the round (0B9F:005C: ware price)
    public const int Spread = 0x3D1C;          // word           read by 0A1D (set in 0C29)
    public const int Sellers = 0x2BB8;         // sbyte          divisor in 0A1D (set in 0C29)
    public const int BuyerFlag = 0x2BC0;       // byte           1 enables the HighestBid rule in 0A1D (set in 0C29)
    public const int LastSale = 0x2BBA;        // word           countdown value at the last forced step (0BFD)
    public const int Countdown = 0x3780;       // word           trade countdown (21FD:04E5/04F9)
    public const int CountdownA = 0x2F16;      // word           countdown companion; 0397 acts when equal to CountdownB
    public const int CountdownB = 0x2F18;      // word
    public const int Buyer = 0x4106;           // word           buyer of a deal, 10 = shop (0C29:03B2)
    public const int Seller = 0x44F2;          // word           seller of a deal (0C29:03B2)
    public const int NoFieldFound = 0x4BBE;    // byte[4]        1 = 135D:05AF found no field (WeekPlanning reads it too)
    public const int Ware3Rank = 0x2C74;       // word[4]        rank by ware-3 stock (11A3:007D)
    public const int TargetPlayer = 0x4580;    // sbyte          target player of the computers (11A3:0180)
    public const int WareNeed = 0x4560;        // word[4][4]     stride 8 (11A3:104B)
    public const int PriceLimit = 0x452A;      // word[4][4]     stride 8 (11A3:104B)
}
