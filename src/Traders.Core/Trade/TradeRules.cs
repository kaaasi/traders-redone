using Traders.Core.Input;
using Traders.Core.Rules;
using Traders.Core.State;
using L = Traders.Core.State.DgroupLayout;
using T = Traders.Core.Trade.TradeLayout;

namespace Traders.Core.Trade;

/// <summary>
/// Rules of the trade 0B9F / 0C29 (finding docs/routines/0B9F_0002_handel.md): set-up of a ware, role choice, movement on the
/// price scale, matching of the highest buyer with the lowest seller and the booking of the deal. Works on the data segment
/// like the original. The sound and the trade's own drawing routines are emitted as <see cref="TradeEvent"/>s in the original's
/// order; the computer players' decisions are the calls of <see cref="ITradeAi"/> (lane handelski). The frame-by-frame flow
/// (0C29:0046, 0086, 015C, 01B7) uses these rules.
/// </summary>
public static class TradeRules
{
    private static int P(Dgroup d) => d.S8(T.Player);

    private static void Emit(ITradeHost h, TradeEventKind k, params short[] args) => h.Emit(new TradeEvent(k, args));

    /// <summary>Loop over the players with the global loop variable DS:37AA (sbyte, ends at 4) like the original.</summary>
    private static void ForPlayers(Dgroup d, Action<Dgroup> body)
    {
        for (d.Set(T.Player, (sbyte)0); d.S8(T.Player) < 4; d.Set(T.Player, unchecked((sbyte)(d.S8(T.Player) + 1))))
            body(d);
    }

    private static byte Role(Dgroup d, int p) => d.U8(T.Byte(T.Role, p));

    private static short Position(Dgroup d, int p) => d.S16(T.Word(T.Position, p));

    private static short Stock(Dgroup d, int p) => d.S16(T.Stock(d, p));

    private static bool IsComputer(Dgroup d, int p) => d.U8(T.Byte(L.IsComputer, p)) == 1;

    /// <summary>Price of a position: position / 2 (truncated) + base price, 16 bit.</summary>
    private static short Price(Dgroup d, short position) => Int16Math.S(Int16Math.Div(position, 2) + d.S16(T.BasePrice));

    /// <summary>
    /// 0B9F:005C – start of a ware: nobody leaving, no deal pending, base price and start price = the ware's price DS:4898,
    /// highest price paid 0.
    /// </summary>
    // [ORIGINAL] 0B9F:005C
    public static void ResetWare(Dgroup d)
    {
        ForPlayers(d, d => d.Set(T.Byte(T.Leaving, P(d)), (byte)0));
        d.Set(T.DealTimer, (byte)0);
        d.Set(T.Settling, (byte)0);
        short price = d.S16(T.Word(L.WarePrice, d.S16(T.Ware)));
        d.Set(T.BasePrice, price);
        d.Set(T.StartPrice, price);
        d.Set(T.BestPrice, (short)0);
    }

    /// <summary>
    /// 0B9F:009E – need of every player for the ware: ware 3: 100; otherwise the fields of the next kind (ware + 1, ware 2 → kind
    /// 0) DS:2EE7 times DS:2F26, the low word sign-extended (the original's cwd after the 16-bit product).
    /// </summary>
    // [ORIGINAL] 0B9F:009E
    public static void ComputeNeed(Dgroup d)
    {
        short kind = Int16Math.S(d.S16(T.Ware) + 1);
        if (kind == 3) kind = 0;
        ForPlayers(d, d =>
        {
            int p = P(d);
            short need = d.S16(T.Ware) == 3
                ? (short)100
                : Int16Math.S(d.S8(Int16Math.U(L.FieldsByKind + Int16Math.S(p << 2) + kind)) * d.S8(L.LimitLow));
            d.Set(T.Dword(T.Need, p), (int)need);
        });
    }

    /// <summary>
    /// SI of main 1777:000C while the week loop runs: its argument counter after the scan of the command line, argc (1 for a
    /// start without arguments; no routine between main and 0B9F:00FE changes SI). 0C29:0F68 saves it on the stack where
    /// 141D:0397 later finds its uninitialised local (<see cref="ChooseRoles"/>).
    /// </summary>
    // [ORIGINAL] 1777:000C – mov si,1 at 1777:0014, inc si per argument (1777:0110); the port starts without arguments
    public const short MainArgumentCount = 1;

    /// <summary>
    /// 0B9F:00FE – one frame of the role choice: controls, the computer's choice (141D:0397), then per player: up makes a
    /// player with at least ten units a seller at 67h (sound 1, 18h), down makes him a buyer at −3 (sound 0, 19h); the old
    /// marker is cleared first.
    /// <para>
    /// 141D:0397 has an uninitialised local at SP−8 of this routine's stack (finding 141D_0008_handelski.md, "Uninitialised
    /// local") and in one branch stores what it finds there as the computer's action. The routines called here leave in that
    /// word: 0E9E:037A its local [bp-2], which ends as the action word of player 3 (−1 for a computer); 141D:0397 the final
    /// value of its local; 0C29:0F68 the saved SI (<paramref name="si"/>; the return address 0154/01AF that 21A7:0054 leaves
    /// there before is always overwritten by it). The word is carried along in that order.
    /// </para>
    /// </summary>
    /// <param name="si">Register SI of the caller chain up to main (<see cref="MainArgumentCount"/> in the game).</param>
    // [ORIGINAL] 0B9F:00FE
    // [APPROX] 0B9F:00FE – stack word under 141D:0397's local: an interrupt (timer, keyboard) between 0E9E:037A and 141D:039C
    // can overwrite it with its flags, CS or the handler's saved AX (measured in the RefEmu: 8 of 19 200 calls in weeks 0–3,
    // none of them in the branch that uses the word); this depends on interrupt timing and is not reproduced.
    public static void ChooseRoles(Dgroup d, OriginalRandom random, ITradeHost host, ITradeAi ai, short si = MainArgumentCount)
    {
        host.ReadControls(d);
        // 0E9E:037A – mov al,[bp-2]; mov [si+3308],al for the last player: the local is the action byte sign-extended
        // (FFFFh, 0, 1, 2, 3 or Ah).
        short stackWord = d.S8(T.Byte(T.Action, 3));
        ForPlayers(d, d =>
        {
            if (IsComputer(d, P(d))) stackWord = ai.ChooseRole(d, random, stackWord);
            int p = P(d);
            switch ((ControlAction)d.S8(T.Byte(T.Action, p)))
            {
                case ControlAction.Down:
                    if (Role(d, p) == T.Buyer) break;
                    Emit(host, TradeEventKind.Sound, 0, 0x19);
                    ClearMarker(d, host);
                    stackWord = si; // 0C29:0F68 – push si at SP−8
                    p = P(d);
                    d.Set(T.Byte(T.Role, p), T.Buyer);
                    d.Set(T.Word(T.Position, p), T.BuyerFloor);
                    break;
                case ControlAction.Up:
                    if (Role(d, p) == T.Seller) break;
                    if (Stock(d, p) < T.Lot) break;
                    Emit(host, TradeEventKind.Sound, 1, 0x18);
                    ClearMarker(d, host);
                    stackWord = si; // 0C29:0F68 – push si at SP−8
                    p = P(d);
                    d.Set(T.Byte(T.Role, p), T.Seller);
                    d.Set(T.Word(T.Position, p), T.SellerStart);
                    break;
            }
        });
    }

    /// <summary>0C29:0F68 as an event: the marker of player DS:37AA at its current role and position.</summary>
    private static void ClearMarker(Dgroup d, ITradeHost host)
    {
        int p = P(d);
        Emit(host, TradeEventKind.ClearMarker, (short)p, Role(d, p), Position(d, p));
    }

    /// <summary>0C29:09BB as an event: cash and stock of player DS:37AA.</summary>
    public static void DrawPlayerNumbers(Dgroup d, ITradeHost host)
    {
        int p = P(d);
        int cash = d.S32(T.Dword(L.Cash, p));
        Emit(host, TradeEventKind.DrawPlayerNumbers, (short)p, Role(d, p), unchecked((short)cash), unchecked((short)(cash >> 16)), Stock(d, p));
    }

    /// <summary>
    /// 0C29:1108(direction) as an event: the stock display of player DS:37AA after a deal (direction −1 sold, 1 bought). Picture
    /// B draws it with 0C29:134D (<see cref="StockBars"/>), whose data segment writes the core does.
    /// </summary>
    public static void DrawStockChange(Dgroup d, ITradeHost host, short direction)
    {
        int p = P(d);
        Emit(host, TradeEventKind.DrawStockChange, direction, (short)p, Role(d, p), Stock(d, p), d.S16(T.Word(T.ShownStock, p)));
        if (d.U8(T.Variant) != 0) StockBars(d);
    }

    /// <summary>
    /// 0C29:134D, its data segment writes (the bars are the scene's): on graphics card 1 (DS:090E) the colour byte DS:0642 = DAh;
    /// at the end the shown stock DS:0634 of player DS:37AA = stock / 10.
    /// </summary>
    // [ORIGINAL] 0C29:134D – only the writes of DS:0642 and DS:0634; the drawing is the scene's
    public static void StockBars(Dgroup d)
    {
        if (d.S16(Field.Planting.CardMode) == 1) d.Set(T.BarColour, (byte)0xDA);
        int p = P(d);
        d.Set(T.Word(T.ShownStock, p), Int16Math.Div(Stock(d, p), 10));
    }

    /// <summary>
    /// 0B9F:01E1 – counts the sellers into DS:2BBC and notes whether a human is among the buyers (DS:439C = 0) or the
    /// sellers (DS:450C = 0).
    /// </summary>
    // [ORIGINAL] 0B9F:01E1
    public static void CountRoles(Dgroup d)
    {
        d.Set(T.Sellers, (short)0);
        d.Set(T.NoHumanBuyer, (byte)1);
        d.Set(T.NoHumanSeller, (byte)1);
        ForPlayers(d, d =>
        {
            int p = P(d);
            bool human = d.U8(T.Byte(L.IsComputer, p)) == 0;
            if (Role(d, p) == T.Seller)
            {
                d.Set(T.Sellers, Int16Math.S(d.S16(T.Sellers) + 1));
                if (human) d.Set(T.NoHumanSeller, (byte)0);
            }
            if (Role(d, p) == T.Buyer && human) d.Set(T.NoHumanBuyer, (byte)0);
        });
    }

    /// <summary>
    /// 0B9F:024A – length of the bidding: the sellers' stock of the ware, each divided by 5·IQ + 15, summed and clamped to
    /// 15 … 120; DS:2BBA = result − 5.
    /// </summary>
    // [ORIGINAL] 0B9F:024A
    public static short BiddingLength(Dgroup d)
    {
        short sum = 0;
        ForPlayers(d, d =>
        {
            int p = P(d);
            if (Role(d, p) != T.Seller) return;
            short divisor = Int16Math.S(Int16Math.S(d.S16(L.ComputerIq) * 5) + 15);
            sum = Int16Math.S(sum + Int16Math.Div(Stock(d, p), divisor));
        });
        if (sum < 15) sum = 15;
        if (sum > 120) sum = 120;
        d.Set(T.CountdownMark, Int16Math.S(sum - 5));
        return sum;
    }

    /// <summary>
    /// 0B9F:02BE – once per frame of the bidding: counts the buyers at the highest buyer position (DS:2BB8) and whether all of
    /// them are computers (DS:2BC0); without such a buyer the base price falls back to the start price.
    /// </summary>
    // [ORIGINAL] 0B9F:02BE
    public static void CheckTopBuyers(Dgroup d)
    {
        d.Set(T.NoHumanTopBuyer, (byte)1);
        d.Set(T.TopBuyers, (byte)0);
        ForPlayers(d, d =>
        {
            int p = P(d);
            if (Role(d, p) != T.Buyer || Position(d, p) != d.S16(T.HighestBuyer)) return;
            d.Set(T.TopBuyers, unchecked((byte)(d.U8(T.TopBuyers) + 1)));
            if (d.U8(T.Byte(L.IsComputer, p)) == 0) d.Set(T.NoHumanTopBuyer, (byte)0);
        });
        if (d.U8(T.TopBuyers) == 0) d.Set(T.BasePrice, d.S16(T.StartPrice));
    }

    /// <summary>
    /// 0B9F:0321 – one step of player DS:37AA on the scale. A buyer who cannot pay the price of his position or holds at least
    /// 990 units is forced down; up climbs unless he meets the lowest seller, his cash equals the price or the price is 999
    /// (above the top the scale moves: 0B9F:04FC); down descends to −3; idle snaps even positions up. A seller below ten units is
    /// forced to leave (fire); up climbs to 67h, down descends to 1 unless he meets the highest buyer, idle snaps odd positions
    /// down; fire marks him leaving. Then the extremes 0B9F:0586.
    /// </summary>
    // [ORIGINAL] 0B9F:0321
    public static void Step(Dgroup d)
    {
        int p = P(d);
        short si = Position(d, p);
        int position = T.Word(T.Position, p), action = T.Byte(T.Action, p);
        byte role = Role(d, p);
        if (role == T.Buyer)
        {
            int cash = d.S32(T.Dword(L.Cash, p));
            if (Price(d, si) > cash || Stock(d, p) >= 0x3DE) d.Set(action, (sbyte)ControlAction.Down);
            switch (d.S8(action))
            {
                case -1:
                    if ((si & 1) != 0) break;
                    goto case 2;
                case 2:
                    if (si == d.S16(T.LowestSeller)) break;
                    if (Price(d, si) == d.S32(T.Dword(L.Cash, p))) break;
                    if (Price(d, si) == 999) break;
                    short up = Int16Math.S(d.S16(position) + 1);
                    d.Set(position, up);
                    if (up > 0x63) PushScale(d);
                    d.Set(T.Moved, (byte)1);
                    break;
                case 3:
                    if (si <= T.BuyerFloor) break;
                    d.Set(position, Int16Math.S(d.S16(position) - 1));
                    d.Set(T.Moved, (byte)1);
                    break;
            }
        }
        else if (role == T.Seller)
        {
            if (Stock(d, p) < T.Lot) d.Set(action, (sbyte)ControlAction.Fire);
            switch (d.S8(action))
            {
                case -1:
                    if ((si & 1) == 0) break;
                    goto case 3;
                case 3:
                    if (si <= 0 || si == d.S16(T.HighestBuyer)) break;
                    d.Set(position, Int16Math.S(d.S16(position) - 1));
                    d.Set(T.Moved, (byte)1);
                    break;
                case 2:
                    if (si >= T.SellerStart) break;
                    d.Set(position, Int16Math.S(d.S16(position) + 1));
                    d.Set(T.Moved, (byte)1);
                    break;
                case 10:
                    d.Set(T.Byte(T.Leaving, p), (byte)1);
                    break;
            }
        }
        UpdateExtremes(d);
    }

    /// <summary>
    /// 0B9F:04FC – a buyer climbed above 63h: every buyer moves down one (not below −3); every second push raises the base price
    /// by one (counter DS:023C).
    /// </summary>
    // [ORIGINAL] 0B9F:04FC
    public static void PushScale(Dgroup d)
    {
        for (short p = 0; p < 4; p++)
        {
            if (d.U8(T.Byte(T.Role, p)) != T.Buyer) continue;
            short v = Int16Math.S(Position(d, p) - 1);
            d.Set(T.Word(T.Position, p), v);
            if (v < T.BuyerFloor) d.Set(T.Word(T.Position, p), T.BuyerFloor);
        }
        short counter = Int16Math.S(d.S16(T.PushCounter) - 1);
        d.Set(T.PushCounter, counter);
        if (counter != 0) return;
        d.Set(T.BasePrice, Int16Math.S(d.S16(T.BasePrice) + 1));
        d.Set(T.PushCounter, (short)2);
    }

    /// <summary>
    /// 0B9F:0549 – movement sound: starts (0, 1Ah) when a player moved after a still frame, stops (0, 0) when nobody moved after
    /// a moving frame.
    /// </summary>
    // [ORIGINAL] 0B9F:0549
    public static void MovementSound(Dgroup d, ITradeHost host)
    {
        byte moved = d.U8(T.Moved);
        if (moved == 0)
        {
            if (d.U8(T.MovedBefore) == 1) Emit(host, TradeEventKind.Sound, 0, 0);
        }
        else if (d.U8(T.MovedBefore) == 0) Emit(host, TradeEventKind.Sound, 0, 0x1A);
        d.Set(T.MovedBefore, d.U8(T.Moved));
    }

    /// <summary>
    /// 0B9F:0586 – highest buyer position (from 0) and lowest position of a seller who is not leaving (from 66h); a change
    /// holds the deals for three frames (DS:3D18).
    /// </summary>
    // [ORIGINAL] 0B9F:0586
    public static void UpdateExtremes(Dgroup d)
    {
        short highest = d.S16(T.HighestBuyer), lowest = d.S16(T.LowestSeller);
        d.Set(T.HighestBuyer, (short)0);
        d.Set(T.LowestSeller, (short)0x66);
        for (short p = 0; p <= 3; p++)
        {
            byte role = d.U8(T.Byte(T.Role, p));
            short pos = Position(d, p);
            if (role == T.Buyer)
            {
                if (pos > d.S16(T.HighestBuyer)) d.Set(T.HighestBuyer, pos);
            }
            else if (role == T.Seller && d.U8(T.Byte(T.Leaving, p)) == 0 && pos < d.S16(T.LowestSeller))
                d.Set(T.LowestSeller, pos);
        }
        if (highest != d.S16(T.HighestBuyer) || lowest != d.S16(T.LowestSeller)) d.Set(T.Settling, (byte)3);
    }

    /// <summary>
    /// 0B9F:060A – the parties of a deal, round robin from the last ones: the next seller at the lowest seller position
    /// (DS:44F2 unchanged if none) and the next buyer at the highest buyer position (DS:4106, 10 if none).
    /// </summary>
    // [ORIGINAL] 0B9F:060A
    public static void FindParties(Dgroup d)
    {
        for (int i = 0; i < 4; i++)
        {
            short c = Int16Math.S(d.S16(T.SellerCursor) + 1);
            d.Set(T.SellerCursor, c);
            if (c > 3) d.Set(T.SellerCursor, (short)0);
            c = d.S16(T.SellerCursor);
            if (d.U8(T.Byte(T.Role, c)) == T.Seller && Position(d, c) == d.S16(T.LowestSeller))
            {
                d.Set(T.DealSeller, c);
                break;
            }
        }
        for (int i = 0; i < 4; i++)
        {
            short c = Int16Math.S(d.S16(T.BuyerCursor) + 1);
            d.Set(T.BuyerCursor, c);
            if (c > 3) d.Set(T.BuyerCursor, (short)0);
            c = d.S16(T.BuyerCursor);
            if (d.U8(T.Byte(T.Role, c)) == T.Buyer && Position(d, c) == d.S16(T.HighestBuyer))
            {
                d.Set(T.DealBuyer, c);
                return;
            }
        }
        d.Set(T.DealBuyer, T.NoBuyer);
    }

    /// <summary>0B9F:0695 – marks the countdown value of the last deal (DS:2BBA = DS:3780).</summary>
    // [ORIGINAL] 0B9F:0695
    public static void MarkCountdown(Dgroup d) => d.Set(T.CountdownMark, d.S16(Countdown.Remaining));

    /// <summary>
    /// 0C29:0323 – once per frame of the bidding: a pending deal counts down (the seller's numbers are drawn two frames after
    /// the deal, the highlight ends at 2); after a change of the extremes nothing happens for three frames and the delay
    /// returns to Eh; otherwise, if the highest buyer meets the lowest seller, the parties are found and the deal is booked, and
    /// the delay to the next deal shrinks by one down to 5.
    /// </summary>
    // [ORIGINAL] 0C29:0323
    public static void Match(Dgroup d, OriginalRandom random, ITradeHost host, ITradeAi ai)
    {
        if (d.S8(T.DealTimer) > 0)
        {
            sbyte timer = unchecked((sbyte)(d.S8(T.DealTimer) - 1));
            d.Set(T.DealTimer, timer);
            if (timer == 2) d.Set(T.DealParties, (byte)0);
            if (d.S8(T.DealTimer) == d.S8(T.DealDelay) - 2)
            {
                d.Set(T.Player, unchecked((sbyte)d.U8(T.DealSeller)));
                DrawPlayerNumbers(d, host);
            }
            return;
        }
        if (d.S8(T.Settling) > 0)
        {
            d.Set(T.Settling, unchecked((byte)(d.U8(T.Settling) - 1)));
            d.Set(T.DealDelay, (byte)0x0E);
            return;
        }
        if (d.S16(T.HighestBuyer) != d.S16(T.LowestSeller)) return;
        FindParties(d);
        d.Set(T.DealParties, d.U8(T.Byte(T.PlayerBits, d.S16(T.DealSeller))));
        if (d.S16(T.DealBuyer) != T.NoBuyer)
            d.Set(T.DealParties, unchecked((byte)(d.U8(T.DealParties) + d.U8(T.Byte(T.PlayerBits, d.S16(T.DealBuyer))))));
        Settle(d, random, host, ai);
        if (d.S8(T.DealDelay) > 5) d.Set(T.DealDelay, unchecked((byte)(d.U8(T.DealDelay) - 1)));
        d.Set(T.DealTimer, d.U8(T.DealDelay));
    }

    /// <summary>
    /// 0C29:03B2 – books a deal at the price of the highest buyer position: sound (1, 1Bh + buyer; 1Fh without buyer), countdown
    /// mark, the seller gets the price and gives ten units (141D:0E67 after), the buyer – if any – pays the price and gets ten
    /// units (141D:0CCA(price) after); the highest price paid is kept in DS:2BBE.
    /// </summary>
    // [ORIGINAL] 0C29:03B2
    public static void Settle(Dgroup d, OriginalRandom random, ITradeHost host, ITradeAi ai)
    {
        short price = Price(d, d.S16(T.HighestBuyer));
        short buyer = d.S16(T.DealBuyer);
        Emit(host, TradeEventKind.Sound, 1, buyer != T.NoBuyer ? Int16Math.S(buyer + 0x1B) : (short)0x1F);
        MarkCountdown(d);
        short seller = d.S16(T.DealSeller);
        Book(d, seller, price, -T.Lot);
        d.Set(T.Player, unchecked((sbyte)d.U8(T.DealSeller)));
        DrawStockChange(d, host, -1);
        ai.AfterSale(d, random);
        if (d.S16(T.DealBuyer) == T.NoBuyer) return;
        Book(d, d.S16(T.DealBuyer), -(int)price, T.Lot);
        d.Set(T.Player, unchecked((sbyte)d.U8(T.DealBuyer)));
        DrawPlayerNumbers(d, host);
        DrawStockChange(d, host, 1);
        ai.AfterPurchase(d, random, price);
        if (price > d.S16(T.BestPrice)) d.Set(T.BestPrice, price);
    }

    /// <summary>Cash += amount (32 bit; the price sign-extended), stock of the ware += units (16 bit).</summary>
    private static void Book(Dgroup d, short p, int amount, short units)
    {
        int cash = T.Dword(L.Cash, p);
        d.Set(cash, unchecked(d.S32(cash) + amount));
        int stock = T.Stock(d, p);
        d.Set(stock, Int16Math.S(d.S16(stock) + units));
    }

    /// <summary>
    /// 0C29:0FB9 – the sellers marked leaving go, unless they are a party of the deal being shown: sound (1, 20h), role 6 at
    /// 67h, one seller less, their column of the scale cleared (0A96:04D7).
    /// </summary>
    // [ORIGINAL] 0C29:0FB9
    public static void Leave(Dgroup d, ITradeHost host)
    {
        ForPlayers(d, d =>
        {
            int p = P(d);
            if (d.U8(T.Byte(T.Leaving, p)) != 1) return;
            if ((d.U8(T.Byte(T.PlayerBits, p)) & d.U8(T.DealParties)) != 0) return;
            Emit(host, TradeEventKind.Sound, 1, 0x20);
            d.Set(T.Byte(T.Role, p), T.Withdrawn);
            d.Set(T.Word(T.Position, p), T.SellerStart);
            d.Set(T.Sellers, Int16Math.S(d.S16(T.Sellers) - 1));
            Emit(host, TradeEventKind.FillRect, Int16Math.S(d.S16(T.ScaleX) + Int16Math.S(p << 4)), 0x0D, 0x10, 0x8D, 0);
            d.Set(T.Byte(T.Leaving, p), (byte)0);
        });
    }

    /// <summary>
    /// 0C29:051A – layout of the panels: buyers from x = 0 in steps of 40h, then the scale, then the sellers (DS:2BBC counted
    /// anew); y of the panels from DS:0258/025A per picture; position of the label and of the countdown by picture and number of
    /// sellers.
    /// </summary>
    // [ORIGINAL] 0C29:051A
    public static void Layout(Dgroup d)
    {
        short x = 0;
        ForPlayers(d, d =>
        {
            int p = P(d);
            if (Role(d, p) != T.Buyer) return;
            d.Set(T.Word(T.PanelX, p), x);
            d.Set(T.Word(T.PanelY, p), (short)d.U8(T.Byte(T.BuyerPanelY, d.S8(T.Variant))));
            x = Int16Math.S(x + 0x40);
        });
        d.Set(T.ScaleX, x);
        x = Int16Math.S(x + 0x40);
        d.Set(T.Sellers, (short)0);
        ForPlayers(d, d =>
        {
            int p = P(d);
            if (Role(d, p) != T.Seller) return;
            d.Set(T.Sellers, Int16Math.S(d.S16(T.Sellers) + 1));
            d.Set(T.Word(T.PanelX, p), x);
            d.Set(T.Word(T.PanelY, p), (short)d.U8(T.Byte(T.SellerPanelY, d.S8(T.Variant))));
            x = Int16Math.S(x + 0x40);
        });
        if (d.U8(T.Variant) != 0)
        {
            d.Set(T.LabelX, (short)0x100);
            d.Set(T.LabelY, (short)0);
            d.Set(T.CountdownX, d.S16(T.ScaleX));
            return;
        }
        short sellers = d.S16(T.Sellers);
        (short lx, short ly) = sellers <= 2 ? ((short)0x40, (short)0) : sellers == 3 ? ((short)0, (short)0) : ((short)0xC0, (short)0x96);
        d.Set(T.LabelX, lx);
        d.Set(T.LabelY, ly);
        d.Set(T.CountdownX, sellers == 3 ? (short)0xC0 : Int16Math.S(d.S16(T.ScaleX) + 0x40));
    }
}
