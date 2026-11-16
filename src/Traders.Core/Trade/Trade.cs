using Traders.Core.Field;
using Traders.Core.Rules;
using Traders.Core.State;
using L = Traders.Core.State.DgroupLayout;
using R = Traders.Core.Trade.TradeRules;
using T = Traders.Core.Trade.TradeLayout;

namespace Traders.Core.Trade;

/// <summary>
/// The trade of the week 0B9F:0002 (call 23 of the week loop 1777:01F8; finding docs/routines/0B9F_0002_handel.md) frame by
/// frame: the four wares with their picture, the role choice and the bidding. Works on the data segment like the original;
/// the rules are <see cref="TradeRules"/>, the computer players' decisions the calls of <see cref="ITradeAi"/>. Drawing and
/// sound are emitted as <see cref="TradeEvent"/>s in the original's order; the data segment writes of the trade's drawing
/// routines that later code reads (the loop variable DS:37AA, DS:2BE4, DS:0634/0642, DS:1126) are done here. Every yielded
/// <see cref="FieldWait"/> is one frame (a retrace or a poll of the pause loop).
/// </summary>
public static class Trade
{
    /// <summary>DS words with the picture numbers of V-AHANDE (picture 0) and V-BHANDE (picture 1).</summary>
    public const int PictureA = 0x08D6, PictureB = 0x08D8;

    private static void Emit(ITradeHost h, TradeEventKind k, params short[] args) => h.Emit(new TradeEvent(k, args));

    private static int P(Dgroup d) => d.S8(T.Player);

    /// <summary>
    /// 0B9F:0002: minimum ware prices (0B9F:06A0), the computer's preparation (141D:0008), number fill '/', the four wares
    /// 0C29:0001 with (ware, picture) = (1, 0), (0, 1), (2, 0), (3, 1), number fill ' '.
    /// </summary>
    // [ORIGINAL] 0B9F:0002
    public static IEnumerable<FieldWait> Run(Dgroup d, OriginalRandom random, ITradeHost host, ITradeAi ai)
    {
        WarePrices.RaiseToMinimum(d);
        ai.Prepare(d, random);
        NumberFill(d, host, 0x2F);
        foreach (var (ware, picture) in new (short, short)[] { (1, 0), (0, 1), (2, 0), (3, 1) })
            foreach (var w in Ware(d, random, host, ai, ware, picture)) yield return w;
        NumberFill(d, host, 0x20);
    }

    /// <summary>2295:0062(c): DS:1126 = c (read by the number formatting 2295:0001 of the core and the output).</summary>
    // [ORIGINAL] 2295:0062
    private static void NumberFill(Dgroup d, ITradeHost host, byte c)
    {
        Emit(host, TradeEventKind.NumberFill, c);
        d.Set(Events.IncidentText.NumberFill, c);
    }

    /// <summary>
    /// 0C29:0001(ware, picture): DS:45D0 = ware, DS:2BC5 = picture (byte); picture 0 loads [08D6], otherwise [08D8]; font 2,
    /// 0C29:0046, font 0.
    /// </summary>
    // [ORIGINAL] 0C29:0001
    public static IEnumerable<FieldWait> Ware(Dgroup d, OriginalRandom random, ITradeHost host, ITradeAi ai, short ware, short picture)
    {
        d.Set(T.Ware, ware);
        d.Set(T.Variant, unchecked((byte)picture));
        Emit(host, TradeEventKind.LoadImage, d.S16(picture == 0 ? PictureA : PictureB));
        Emit(host, TradeEventKind.SetFont, 2);
        foreach (var w in WareFrames(d, random, host, ai)) yield return w;
        Emit(host, TradeEventKind.SetFont, 0);
    }

    /// <summary>
    /// 0C29:0046: deal delay Eh, 0B9F:005C, 0B9F:009E, the panels 0C29:0086, list emptied, the role choice 0C29:015C,
    /// 0B9F:01E1; with a seller 141D:0464, the bidding 0C29:01B7 and the new price 0B9F:078A.
    /// </summary>
    // [ORIGINAL] 0C29:0046
    public static IEnumerable<FieldWait> WareFrames(Dgroup d, OriginalRandom random, ITradeHost host, ITradeAi ai)
    {
        d.Set(T.DealDelay, (byte)0x0E);
        R.ResetWare(d);
        R.ComputeNeed(d);
        foreach (var w in Opening(d, random, host, ai)) yield return w;
        Emit(host, TradeEventKind.ClearList);
        foreach (var w in RoleChoice(d, random, host, ai)) yield return w;
        R.CountRoles(d);
        if (d.S16(T.Sellers) == 0) yield break;
        ai.StartBidding(d, random);
        foreach (var w in Bidding(d, random, host, ai)) yield return w;
        WarePrices.AfterAuction(d);
    }

    /// <summary>
    /// 0C29:0086: everybody seller for the layout 0C29:051A, the transition 0B9F:080C, the board 0C29:0635; per player stock,
    /// numbers, need, 141D:021A, then buyer at −3; markers, extremes 0 / Ah, countdown 19h − 5·IQ steps of 14h frames, its
    /// bar, the page copied, sound (2, 4Dh), the trade's colours, fade in.
    /// </summary>
    // [ORIGINAL] 0C29:0086
    public static IEnumerable<FieldWait> Opening(Dgroup d, OriginalRandom random, ITradeHost host, ITradeAi ai)
    {
        ForPlayers(d, p => d.Set(T.Byte(T.Role, p), T.Seller));
        R.Layout(d);
        foreach (var w in Transition(d, host)) yield return w;
        DrawBoard(d, host);
        ForPlayers(d, p =>
        {
            DrawStock(d, host);
            Emit(host, TradeEventKind.ClearList);
            R.DrawPlayerNumbers(d, host);
            Emit(host, TradeEventKind.DrawNeed, (short)P(d));
            ai.EnterTrade(d, random);
            p = P(d);
            d.Set(T.Byte(T.Role, p), T.Buyer);
            d.Set(T.Word(T.Position, p), T.BuyerFloor);
            Emit(host, TradeEventKind.RunList);
        });
        DrawMarkers(d, host);
        Emit(host, TradeEventKind.ClearList);
        d.Set(T.HighestBuyer, (short)0);
        d.Set(T.LowestSeller, (short)0x0A);
        Countdown.Start(d, Int16Math.S(0x19 - Int16Math.S(d.S16(L.ComputerIq) * 5)), 0x14);
        Emit(host, TradeEventKind.DrawCountdown);
        Emit(host, TradeEventKind.RunList);
        foreach (var w in ShowPage(d, host)) yield return w;
        Emit(host, TradeEventKind.Sound, 2, 0x4D);
        Emit(host, TradeEventKind.TradePalette);
        Emit(host, TradeEventKind.FadeIn);
        foreach (var w in Planting.Fade(d, 3)) yield return w;
    }

    /// <summary>
    /// 0C29:015C: per frame while the countdown runs: flip, list, list emptied, one frame of the role choice 0B9F:00FE, the
    /// countdown tick 21FD:04F9 (its step drawn by 0C29:0D6A), list, markers. Then the last frame, sound (2, 0), 19h retraces.
    /// </summary>
    // [ORIGINAL] 0C29:015C
    public static IEnumerable<FieldWait> RoleChoice(Dgroup d, OriginalRandom random, ITradeHost host, ITradeAi ai)
    {
        while (d.S16(Countdown.Remaining) != 0)
        {
            foreach (var w in Flip(d, host)) yield return w;
            Emit(host, TradeEventKind.RunList);
            Emit(host, TradeEventKind.ClearList);
            R.ChooseRoles(d, random, host, ai);
            foreach (var w in Tick(d, host)) yield return w;
            Emit(host, TradeEventKind.RunList);
            DrawMarkers(d, host);
        }
        foreach (var w in End(d, host)) yield return w;
        Emit(host, TradeEventKind.Sound, 2, 0);
        foreach (var w in Delay(host, 0x19)) yield return w;
    }

    /// <summary>
    /// 0C29:01B7: layout, fade out to black, board, panels, markers, countdown of 0B9F:024A steps of 1Eh frames and its bar, page
    /// copied, fast fade in, no deal parties, extremes 0 / B4h, sound (2, 4Dh). Then per frame while (the countdown runs and a
    /// seller is left) or a deal is shown: flip, list, controls, 0B9F:02BE, every player's step (the computer's 141D:09F3 first),
    /// markers, movement sound, the sellers leaving, the base price if it changed, countdown tick, 0C29:0323, list. Then the last
    /// frame, sound (2, 0) and (0, 0), 19h retraces, no deal shown.
    /// </summary>
    // [ORIGINAL] 0C29:01B7
    public static IEnumerable<FieldWait> Bidding(Dgroup d, OriginalRandom random, ITradeHost host, ITradeAi ai)
    {
        R.Layout(d);
        Emit(host, TradeEventKind.FadeOutFast, 0);
        foreach (var w in Planting.Fade(d, 2)) yield return w;
        DrawBoard(d, host);
        ForPlayers(d, _ =>
        {
            DrawStock(d, host);
            Emit(host, TradeEventKind.ClearList);
            R.DrawPlayerNumbers(d, host);
            Emit(host, TradeEventKind.DrawNeed, (short)P(d));
            Emit(host, TradeEventKind.RunList);
        });
        DrawMarkers(d, host);
        Emit(host, TradeEventKind.ClearList);
        Countdown.Start(d, R.BiddingLength(d), 0x1E);
        Emit(host, TradeEventKind.DrawCountdown);
        Emit(host, TradeEventKind.RunList);
        foreach (var w in ShowPage(d, host)) yield return w;
        Emit(host, TradeEventKind.FadeInFast);
        foreach (var w in Planting.Fade(d, 1)) yield return w;
        d.Set(T.DealBuyer, (short)-1);
        d.Set(T.DealSeller, (short)-1);
        d.Set(T.HighestBuyer, (short)0);
        d.Set(T.LowestSeller, (short)0xB4);
        Emit(host, TradeEventKind.Sound, 2, 0x4D);
        Emit(host, TradeEventKind.ClearList);
        while ((d.S16(Countdown.Remaining) != 0 && d.S16(T.Sellers) != 0) || d.U8(T.DealTimer) != 0)
        {
            foreach (var w in Flip(d, host)) yield return w;
            Emit(host, TradeEventKind.RunList);
            host.ReadControls(d);
            R.CheckTopBuyers(d);
            d.Set(T.Moved, (byte)0);
            ForPlayers(d, p =>
            {
                if (d.U8(T.Byte(L.IsComputer, p)) == 1) ai.Bid(d, random);
                R.Step(d);
            });
            DrawMarkers(d, host);
            R.MovementSound(d, host);
            Emit(host, TradeEventKind.ClearList);
            R.Leave(d, host);
            if (d.S16(T.BasePrice) != d.S16(T.ShownBasePrice)) DrawBasePrice(d, host);
            foreach (var w in Tick(d, host)) yield return w;
            R.Match(d, random, host, ai);
            Emit(host, TradeEventKind.RunList);
        }
        foreach (var w in End(d, host)) yield return w;
        Emit(host, TradeEventKind.Sound, 2, 0);
        Emit(host, TradeEventKind.Sound, 0, 0);
        foreach (var w in Delay(host, 0x19)) yield return w;
        d.Set(T.DealParties, (byte)0);
    }

    /// <summary>Loop over the players with the global loop variable DS:37AA (sbyte, ends at 4) like the original.</summary>
    private static void ForPlayers(Dgroup d, Action<int> body)
    {
        for (d.Set(T.Player, (sbyte)0); d.S8(T.Player) < 4; d.Set(T.Player, unchecked((sbyte)(d.S8(T.Player) + 1))))
            body(P(d));
    }

    /// <summary>
    /// 0C29:0635: board of the picture (0C29:065C picture A, 0739 picture B; both loop over the players with DS:37AA) and the
    /// base price 0C29:0B1F. Drawing is the scene's; the core keeps DS:37AA = 4 and DS:2BE4.
    /// </summary>
    // [ORIGINAL] 0C29:0635 – data segment writes only (065C/0739: DS:37AA loop; 0B1F: DS:2BE4); the drawing is the scene's
    private static void DrawBoard(Dgroup d, ITradeHost host)
    {
        Emit(host, TradeEventKind.DrawBoard);
        d.Set(T.Player, (sbyte)4);
        d.Set(T.ShownBasePrice, d.S16(T.BasePrice));
    }

    /// <summary>0C29:0B1F: the base price drawn, DS:2BE4 = DS:455A.</summary>
    // [ORIGINAL] 0C29:0B1F – data segment write DS:2BE4; the number is the scene's
    private static void DrawBasePrice(Dgroup d, ITradeHost host)
    {
        Emit(host, TradeEventKind.DrawBasePrice, d.S16(T.BasePrice));
        d.Set(T.ShownBasePrice, d.S16(T.BasePrice));
    }

    /// <summary>0C29:109F: stock of player DS:37AA; in picture B the bars 0C29:134D (<see cref="TradeRules.StockBars"/>).</summary>
    // [ORIGINAL] 0C29:109F – data segment writes of 134D in picture B only; the drawing is the scene's
    private static void DrawStock(Dgroup d, ITradeHost host)
    {
        Emit(host, TradeEventKind.DrawStock, (short)P(d));
        if (d.U8(T.Variant) != 0) R.StockBars(d);
    }

    /// <summary>0C29:0E01: markers of all players (loop over DS:37AA, ends at 4); the number buffer it fills is the scene's.</summary>
    // [ORIGINAL] 0C29:0E01 – data segment write DS:37AA (loop); the drawing and its number buffer are the scene's
    private static void DrawMarkers(Dgroup d, ITradeHost host)
    {
        Emit(host, TradeEventKind.DrawMarkers);
        d.Set(T.Player, (sbyte)4);
    }

    /// <summary>
    /// 0B9F:080C – transition: 21FD:0590 (two retraces), 100 lines from the top and bottom; on card 1 (DS:090E) every line waits
    /// one retrace and from line 49 every seventh a palette step (one retrace more); then blend colour 3 and a flip.
    /// </summary>
    // [ORIGINAL] 0B9F:080C – frames; the drawing (0A69:000B, 21FD:03EB, 21FD:04DA) is the scene's
    // [ORIGINAL] 21FD:0590 – 0B67:0238 (show, retrace), 0B67:028F (retrace)
    public static IEnumerable<FieldWait> Transition(Dgroup d, ITradeHost host)
    {
        Emit(host, TradeEventKind.Transition);
        yield return FieldWait.Retrace;
        yield return FieldWait.Retrace;
        bool vga = d.S16(Planting.CardMode) == 1;
        for (short si = 0; si < 0x64; si++)
        {
            if (!vga) continue;
            if (si >= 0x31 && si % 7 == 0) yield return FieldWait.Retrace;
            yield return FieldWait.Retrace;
        }
        foreach (var w in Planting.Flip(d)) yield return w;
    }

    /// <summary>
    /// 0A96:0C3F([082E]) – page copy; on cards 2 and 63h a flip first. The page number DS:082E belongs to the output (written by
    /// its flips), so the event carries none.
    /// </summary>
    // [ORIGINAL] 0A96:0C3F – 21FD:051F when DS:090E = 2 or 63h, then the page copy (output)
    private static IEnumerable<FieldWait> ShowPage(Dgroup d, ITradeHost host)
    {
        Emit(host, TradeEventKind.ShowPage);
        if (d.S16(Planting.CardMode) is not (2 or 0x63)) yield break;
        foreach (var w in Planting.Flip(d)) yield return w;
    }

    /// <summary>21FD:051F – page flip (<see cref="Planting.Flip"/>).</summary>
    private static IEnumerable<FieldWait> Flip(Dgroup d, ITradeHost host)
    {
        Emit(host, TradeEventKind.FlipPages);
        return Planting.Flip(d);
    }

    /// <summary>21FD:0665 – flip, restore, list, objects: the frames of the flip.</summary>
    // [ORIGINAL] 21FD:0665 – 21FD:051F, 23CD:071C, 0A96:06A8, 23CD:06E9
    private static IEnumerable<FieldWait> End(Dgroup d, ITradeHost host)
    {
        Emit(host, TradeEventKind.LastFrame);
        return Planting.Flip(d);
    }

    /// <summary>21FD:0185(n, 0).</summary>
    private static IEnumerable<FieldWait> Delay(ITradeHost host, short n)
    {
        Emit(host, TradeEventKind.Delay, n, 0);
        return Planting.Retraces(n, 0);
    }

    /// <summary>21FD:04F9: the pause check 21FD:0245, then the countdown tick; a step is drawn by 0C29:0D6A.</summary>
    private static IEnumerable<FieldWait> Tick(Dgroup d, ITradeHost host)
    {
        foreach (var w in host.PauseCheck(d)) yield return w;
        if (Countdown.Tick(d)) Emit(host, TradeEventKind.CountdownStep);
    }
}
