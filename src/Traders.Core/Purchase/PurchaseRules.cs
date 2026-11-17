using Traders.Core.Events;
using Traders.Core.Field;
using Traders.Core.Rules;
using Traders.Core.State;
using K = Traders.Core.Purchase.PurchaseEventKind;
using L = Traders.Core.State.DgroupLayout;
using P = Traders.Core.Purchase.PurchaseLayout;

namespace Traders.Core.Purchase;

/// <summary>
/// Rules of the purchase 08CF:0004 (finding docs/routines/08CF_0004_einkauf.md): the walk along the shops, the shops for
/// oil, rockets and gems, the lotto with its draw, the computer's choices and the set-up of the street with its random
/// decoration. Works on the data segment like the original; drawing and sound are emitted as <see cref="PurchaseEvent"/>s (and
/// <see cref="FieldEvent"/>s from the field routines it calls) in the original's order.
/// </summary>
public static class PurchaseRules
{
    /// <summary>Segment value the events carry for a near string of the data segment (the original pushes DS).</summary>
    public const short DataSegment = 0;

    private static int Pl(Dgroup d) => d.S8(P.Player);
    private static int At(int offset) => Int16Math.U(offset);
    private static int Cash(Dgroup d) => d.S32(At(L.Cash + 4 * Pl(d)));
    private static void SetCash(Dgroup d, int value) => d.Set(At(L.Cash + 4 * Pl(d)), value);
    private static short PanelX(Dgroup d, int add) => Int16Math.S((Pl(d) << 6) + add);

    private static void Emit(IPurchaseOutput o, K kind, params short[] args) => o.Emit(new PurchaseEvent(kind, args));
    private static void Sound(IPurchaseOutput o, short channel, short sound) => Emit(o, K.Sound, channel, sound);

    private static void DrawCash(Dgroup d, IPurchaseOutput o)
    {
        int cash = Cash(d);
        Emit(o, K.DrawCash, (short)Pl(d), (short)cash, (short)(cash >> 16));
    }

    private static void Emblem(Dgroup d, IPurchaseOutput o, short rank) => Emit(o, K.Emblem, PanelX(d, 0x20), 6, rank);

    private static void OwnEmblem(Dgroup d, IPurchaseOutput o) => Emblem(d, o, d.U8(At(L.Rank + Pl(d))));

    /// <summary>08CF:16F5 – clears the ticket of the player DS:37AA.</summary>
    // Drawn in part 3 (scene): 08CF:16F5 – 0A96:0267(F0h, B0h, 20h, 16h, p·40h + 1Ah, 35h, 2Ch, 0Ah); drawing only
    private static void ClearTicket(Dgroup d, IPurchaseOutput o) => Emit(o, K.ClearTicket, (short)Pl(d));

    /// <summary>08CF:16B4(slot, number) – one number of the ticket of the player DS:37AA.</summary>
    // Drawn in part 3 (scene): 08CF:16B4 – 0A96:0030(number·0Ah + F0h, CFh, 0Ah, 0Ah, p·40h + slot·0Bh + 1Ah, 35h); drawing only
    private static void DrawTicket(Dgroup d, IPurchaseOutput o, short slot, short number) =>
        Emit(o, K.DrawTicket, slot, number, (short)Pl(d));

    /// <summary>The value of all gems of one unit: DS:4436 · DS:3CF0 / 100 (32-bit, LXMUL and LDIV).</summary>
    public static int GemValue(Dgroup d) => LongMath.Div(LongMath.Mul(d.S32(L.GemPriceFactor), d.S32(L.BaseUnit)), 100);

    /// <summary>
    /// 08CF:0082: x positions of the shops – lotto at 60h − 20h · (oil + gems switched on), then oil (only with OIL
    /// CONSUMPTION), rockets and gems in steps of 40h.
    /// </summary>
    // [ORIGINAL] 08CF:0082
    public static void Layout(Dgroup d)
    {
        short x = Int16Math.S(0x60 - ((d.U8(L.OilConsumption) + d.U8(L.GemTrading)) << 5));
        d.Set(P.LottoX, x);
        x = Int16Math.S(x + 0x40);
        if (d.U8(L.OilConsumption) == 1)
        {
            d.Set(P.OilX, x);
            x = Int16Math.S(x + 0x40);
        }
        d.Set(P.RocketX, x);
        x = Int16Math.S(x + 0x40);
        d.Set(P.GemX, x);
    }

    /// <summary>
    /// 08CF:029A: one frame of the player DS:37AA in the street – a running move goes on (0609:1FE0); otherwise the computer
    /// chooses (08CF:03E5) and the action moves to the next shop right (0) or left (1), skipping the oil shop without OIL
    /// CONSUMPTION and the gem shop without GEM TRADING, or enters the shop (2, 08CF:039C).
    /// </summary>
    // [ORIGINAL] 08CF:029A
    public static void Street(Dgroup d, OriginalRandom random, IPurchaseOutput o)
    {
        int p = Pl(d);
        if (d.S16(At(P.MoveTimer + 2 * p)) != 0)
        {
            Planting.Move(d, o);
            return;
        }
        if (d.U8(At(L.IsComputer + p)) == 1) ComputerChoice(d);
        p = Pl(d);
        sbyte shop = d.S8(At(P.Shop + p));
        switch (d.S8(At(P.Action + p)))
        {
            case 1:
                shop--;
                if (d.U8(L.OilConsumption) == 0 && shop == 1) shop--;
                if (shop < 0) return;
                d.Set(At(P.Shop + p), shop);
                d.Set(At(P.Direction + 2 * p), (short)1);
                d.Set(At(P.MoveTimer + 2 * p), (short)0x21);
                return;
            case 0:
                shop++;
                if (d.U8(L.OilConsumption) == 0 && shop == 1) shop++;
                if (d.U8(L.GemTrading) == 0 && shop == 3) shop++;
                if (shop > 3) return;
                d.Set(At(P.Shop + p), shop);
                d.Set(At(P.Direction + 2 * p), (short)0);
                d.Set(At(P.MoveTimer + 2 * p), (short)0x21);
                return;
            case 2:
                Enter(d, o);
                return;
        }
    }

    /// <summary>
    /// 08CF:039C: the player DS:37AA enters the shop in front of it – lotto (only while the draw waits, DS:15A2 = FFh), oil,
    /// rockets or gems.
    /// </summary>
    // [ORIGINAL] 08CF:039C
    public static void Enter(Dgroup d, IPurchaseOutput o)
    {
        switch (d.S8(At(P.Shop + Pl(d))))
        {
            case 0:
                if (d.S8(P.DrawState) == P.DrawWaiting) EnterLotto(d, o);
                break;
            case 1: EnterOil(d, o); break;
            case 2: EnterRockets(d, o); break;
            case 3: EnterGems(d, o); break;
        }
    }

    /// <summary>
    /// 08CF:03E5: the computer DS:37AA heads for the lotto (if it may play and has fewer than four numbers), else for oil,
    /// rockets or gems as far as it wants them; with no wish its action is FFh.
    /// </summary>
    // [ORIGINAL] 08CF:03E5
    public static void ComputerChoice(Dgroup d)
    {
        if (CanPlayLotto(d) && d.S8(At(P.Picks + Pl(d))) < 4) Head(d, 0);
        else if (ComputerWantsOil(d)) Head(d, 1);
        else if (ComputerWantsRocket(d)) Head(d, 2);
        else if (ComputerGemChoice(d) != 0) Head(d, 3);
        else d.Set(At(P.Action + Pl(d)), (sbyte)-1);
    }

    /// <summary>08CF:0455(target): action towards the shop – 0 (right) if it lies right, 1 (left) if left, 2 (enter) if here.</summary>
    // [ORIGINAL] 08CF:0455
    public static void Head(Dgroup d, short target)
    {
        short shop = d.S8(At(P.Shop + Pl(d)));
        sbyte action = shop < target ? (sbyte)0 : shop > target ? (sbyte)1 : (sbyte)2;
        d.Set(At(P.Action + Pl(d)), action);
    }

    /// <summary>08CF:0492: into the oil shop (mode 0Ch) – empty panel, cash, sound (0, 50h).</summary>
    // [ORIGINAL] 08CF:0492
    public static void EnterOil(Dgroup d, IPurchaseOutput o)
    {
        d.Set(At(P.Mode + Pl(d)), P.OilShop);
        Emit(o, K.EmptyPanel, (short)Pl(d));
        DrawCash(d, o);
        Sound(o, 0, 0x50);
    }

    /// <summary>08CF:066D: into the gem shop (mode 0Eh) – empty panel, cash, gems, gem account, sound (0, 50h).</summary>
    // [ORIGINAL] 08CF:066D
    public static void EnterGems(Dgroup d, IPurchaseOutput o)
    {
        d.Set(At(P.Mode + Pl(d)), P.GemShop);
        Emit(o, K.EmptyPanel, (short)Pl(d));
        DrawCash(d, o);
        DrawGems(d, o);
        DrawGemCredit(d, o);
        Sound(o, 0, 0x50);
    }

    /// <summary>08CF:09C4: into the rocket shop (mode 0Bh) – empty panel, cash, rockets, sound (0, 50h).</summary>
    // [ORIGINAL] 08CF:09C4
    public static void EnterRockets(Dgroup d, IPurchaseOutput o)
    {
        d.Set(At(P.Mode + Pl(d)), P.RocketShop);
        Emit(o, K.EmptyPanel, (short)Pl(d));
        DrawCash(d, o);
        DrawRockets(d, o);
        Sound(o, 0, 0x50);
    }

    /// <summary>
    /// 08CF:0BE1: to the lotto (mode 0Dh) – empty panel, cash, sound (0, 50h), no numbers picked, ticket cleared; with cash of
    /// at least the ticket price DS:40CC text 18 and the four empty numbers, else texts 16 and 17.
    /// </summary>
    // [ORIGINAL] 08CF:0BE1
    public static void EnterLotto(Dgroup d, IPurchaseOutput o)
    {
        int p = Pl(d);
        d.Set(At(P.Mode + p), P.Lotto);
        Emit(o, K.EmptyPanel, (short)p);
        DrawCash(d, o);
        Sound(o, 0, 0x50);
        d.Set(At(P.Picks + p), (byte)0);
        ClearTicket(d, o);
        short x = PanelX(d, 0x26);
        if (Cash(d) >= d.S16(L.LottoPrice))
        {
            Text(d, o, x, 0x1A, P.TextLotto, 2, 5);
            for (short slot = 0; slot <= 3; slot++)
            {
                d.Set(At(P.Tickets + 4 * Pl(d) + slot), (byte)0);
                DrawTicket(d, o, slot, 0);
            }
        }
        else
        {
            Text(d, o, x, 0x1A, P.TextBroke, 8, 5);
            Text(d, o, x, 0x20, P.TextDown, 8, 5);
        }
    }

    private static void Text(Dgroup d, IPurchaseOutput o, short x, short y, int text, short colour, short shadow)
    {
        int at = P.TextTable + 4 * text;
        Emit(o, K.Text, x, y, d.S16(at), d.S16(at + 2), colour, shadow);
    }

    /// <summary>
    /// 08CF:04BA: one frame of the player DS:37AA in the oil shop – the computer leaves (3) when it wants no oil and buys (0)
    /// when its input delay DS:3786 has run out; 0 or fire buys one unit (cash at least DS:43F4, oil below 28h, shop not
    /// empty; 0609:1B72(1) adds it), 3 leaves.
    /// </summary>
    // [ORIGINAL] 08CF:04BA
    public static void OilShop(Dgroup d, IPurchaseOutput o)
    {
        int p = Pl(d);
        if (d.U8(At(L.IsComputer + p)) == 1)
        {
            if (!ComputerWantsOil(d)) d.Set(At(P.Action + Pl(d)), (sbyte)3);
            else if (d.S32(At(P.MoveDelay + 4 * Pl(d))) <= 0) d.Set(At(P.Action + Pl(d)), (sbyte)0);
        }
        switch (d.S8(At(P.Action + Pl(d))))
        {
            case 3:
                Leave(d, o);
                return;
            case 0:
            case 10:
                short price = d.S16(L.OilPrice);
                if (Cash(d) < price) return;
                if (d.S16(At(L.Oil + 2 * Pl(d))) >= P.MaxOil) return;
                if (d.S16(L.ShopOil) == 0) return;
                FieldCursor.TimeBar(d, 1, o);
                d.Set(L.ShopOil, Int16Math.S(d.S16(L.ShopOil) - 1));
                Emit(o, K.DrawShopOil, d.S16(L.ShopOil));
                SetCash(d, Cash(d) - d.S16(L.OilPrice));
                Sound(o, 0, 0x52);
                DrawCash(d, o);
                d.Set(At(P.MoveDelay + 4 * Pl(d)), 3);
                return;
        }
    }

    /// <summary>Leaving a shop (action 3 in 04BA, 069F, 09F1): emblem with the rank, mode 0Ah, sound (0, 51h).</summary>
    private static void Leave(Dgroup d, IPurchaseOutput o)
    {
        OwnEmblem(d, o);
        d.Set(At(P.Mode + Pl(d)), P.Street);
        Sound(o, 0, 0x51);
    }

    /// <summary>
    /// 08CF:05F1: the computer DS:37AA wants oil – OIL CONSUMPTION on, oil below 28h, the shop not empty and cash / (100 below
    /// 10 units, else 10 · oil + 200) at least the oil price (32-bit LDIV).
    /// </summary>
    // [ORIGINAL] 08CF:05F1
    public static bool ComputerWantsOil(Dgroup d)
    {
        short oil = d.S16(At(L.Oil + 2 * Pl(d)));
        if (d.U8(L.OilConsumption) == 0 || oil >= P.MaxOil) return false;
        if (d.S16(L.ShopOil) == 0) return false;
        short divisor = oil < 10 ? (short)100 : Int16Math.S(oil * 10 + 200);
        return LongMath.Div(Cash(d), divisor) >= d.S16(L.OilPrice);
    }

    /// <summary>
    /// 08CF:069F: one frame of the player DS:37AA in the gem shop – a computer acts when DS:15B4 = 0 (08CF:0914: 1 sell, 2 buy,
    /// 3 cash, else leave); 0 buys a gem from the account, 1 sells one into it (08CF:0886, counted in DS:159C), fire sells one
    /// for cash (DS:4436 · DS:3CF0 / 100), 3 leaves.
    /// </summary>
    // [ORIGINAL] 08CF:069F
    public static void GemShop(Dgroup d, IPurchaseOutput o)
    {
        if (d.U8(At(L.IsComputer + Pl(d))) == 1 && d.S16(P.ComputerDelay) == 0)
        {
            short action = ComputerGemChoice(d) switch { 1 => 1, 2 => 0, 3 => 10, _ => 3 };
            d.Set(At(P.Action + Pl(d)), (sbyte)action);
        }
        switch (d.S8(At(P.Action + Pl(d))))
        {
            case 3:
                Leave(d, o);
                return;
            case 1:
                if (!CanSellGem(d)) return;
                TradeGem(d, -1, o);
                d.Set(At(P.GemDeals + Pl(d)), unchecked((byte)(d.U8(At(P.GemDeals + Pl(d))) + 1)));
                return;
            case 0:
                if (!CanBuyGem(d)) return;
                TradeGem(d, 1, o);
                d.Set(At(P.GemDeals + Pl(d)), unchecked((byte)(d.U8(At(P.GemDeals + Pl(d))) + 1)));
                return;
            case 10:
                if (d.U8(At(L.Gems + Pl(d))) == 0) return;
                ChangeGems(d, -1, o);
                SetCash(d, Cash(d) + GemValue(d));
                DrawCash(d, o);
                Sound(o, 0, 0x52);
                return;
        }
    }

    /// <summary>08CF:0813: a gem may go into the account – the player has one and account + DS:4436 stays at most 2000.</summary>
    // [ORIGINAL] 08CF:0813
    public static bool CanSellGem(Dgroup d)
    {
        int sum = d.S16(At(L.GemCredit + 2 * Pl(d))) + d.S32(L.GemPriceFactor);
        return sum <= P.MaxGemCredit && d.U8(At(L.Gems + Pl(d))) != 0;
    }

    /// <summary>08CF:084F: a gem may be bought from the account – fewer than ten gems and an account of at least DS:4436.</summary>
    // [ORIGINAL] 08CF:084F
    public static bool CanBuyGem(Dgroup d) =>
        d.U8(At(L.Gems + Pl(d))) != P.MaxGems && d.S16(At(L.GemCredit + 2 * Pl(d))) >= d.S32(L.GemPriceFactor);

    /// <summary>
    /// 08CF:0886(direction): one gem bought (1) or sold (−1) against the account – gems (08CF:178D), account − direction ·
    /// DS:4436 (low word of the 32-bit product), account shown, sound (0, 2Bh) or (0, 2Ch); a human's input waits 64h.
    /// </summary>
    // [ORIGINAL] 08CF:0886
    public static void TradeGem(Dgroup d, sbyte direction, IPurchaseOutput o)
    {
        ChangeGems(d, direction, o);
        short product = (short)LongMath.Mul(direction, d.S32(L.GemPriceFactor));
        int at = At(L.GemCredit + 2 * Pl(d));
        d.Set(at, Int16Math.S(d.S16(at) - product));
        DrawGemCredit(d, o);
        Sound(o, 0, direction == 1 ? (short)0x2B : (short)0x2C);
        if (d.U8(At(L.IsComputer + Pl(d))) == 0) d.Set(At(P.MoveDelay + 4 * Pl(d)), 0x64);
    }

    /// <summary>
    /// 08CF:0914: the computer's wish in the gem shop from the drift s = DS:4436 − (week + 100) (16 bit): none without GEM
    /// TRADING; for s &gt; 0 cash (3) with more than five gems and 75 − 5 · gems ≤ s, else sell (1) while its deals are fewer
    /// than (s − 10) / 4; for s &lt; 0 buy (2) while its deals are fewer than −(s + 10) / 4; else 0.
    /// </summary>
    // [ORIGINAL] 08CF:0914
    public static short ComputerGemChoice(Dgroup d)
    {
        short s = Int16Math.S(d.S16(L.GemPriceFactor) - Int16Math.S(d.S16(L.Week) + 100));
        if (d.U8(L.GemTrading) == 0) return 0;
        int p = Pl(d);
        if (s > 0)
        {
            sbyte gems = d.S8(At(L.Gems + p));
            if (gems > 5 && Int16Math.S(0x4B - Int16Math.S(gems * 5)) <= s) return 3;
            if (CanSellGem(d) && d.S8(At(P.GemDeals + Pl(d))) < Int16Math.Div(Int16Math.S(s - 10), 4)) return 1;
            return 0;
        }
        if (s < 0 && CanBuyGem(d) && d.S8(At(P.GemDeals + Pl(d))) < Int16Math.Div(Int16Math.S(-Int16Math.S(s + 10)), 4))
            return 2;
        return 0;
    }

    /// <summary>
    /// 08CF:09F1: one frame of the player DS:37AA in the rocket shop – a computer chooses when DS:15B4 = 0 (fire if 08CF:0AF0,
    /// else leave); fire buys a rocket (cash at least DS:48A0, fewer than ten), 3 leaves.
    /// </summary>
    // [ORIGINAL] 08CF:09F1
    public static void RocketShop(Dgroup d, IPurchaseOutput o)
    {
        if (d.U8(At(L.IsComputer + Pl(d))) == 1 && d.S16(P.ComputerDelay) == 0)
            d.Set(At(P.Action + Pl(d)), ComputerWantsRocket(d) ? (sbyte)10 : (sbyte)3);
        switch (d.S8(At(P.Action + Pl(d))))
        {
            case 3:
                Leave(d, o);
                return;
            case 10:
                if (Cash(d) < d.S16(L.RocketPrice)) return;
                int at = At(L.Rockets + 2 * Pl(d));
                if (d.S16(at) == P.MaxRockets) return;
                d.Set(at, Int16Math.S(d.S16(at) + 1));
                DrawRockets(d, o);
                SetCash(d, Cash(d) - d.S16(L.RocketPrice));
                DrawCash(d, o);
                Sound(o, 0, 0x52);
                return;
        }
    }

    /// <summary>
    /// 08CF:0AF0: the computer DS:37AA wants a rocket – fewer rockets than it still needs (DS:3046) and the price at most a
    /// share of its cash (unsigned 32-bit compare): cash / 10 for rank ≥ 2, else cash / 30, when Fat Mike's flag DS:4BC2 is 1
    /// or rockets + rocket bases &lt; 3; otherwise cash / 50.
    /// </summary>
    // [ORIGINAL] 08CF:0AF0
    public static bool ComputerWantsRocket(Dgroup d)
    {
        int p = Pl(d);
        short rockets = d.S16(At(L.Rockets + 2 * p));
        if (rockets >= d.S16(At(P.RocketShortfall + 2 * p))) return false;
        int limit;
        if (d.U8(At(L.FatMikeFlag + p)) == 1 || Int16Math.S(rockets + d.S8(At(L.FieldsByKind + 4 * p + 3))) < 3)
            limit = LongMath.Div(Cash(d), d.U8(At(L.Rank + p)) >= 2 ? 10 : 30);
        else limit = LongMath.Div(Cash(d), 50);
        return (uint)(int)d.S16(L.RocketPrice) <= (uint)limit;
    }

    /// <summary>
    /// 08CF:0CD5: one frame of the player DS:37AA at the lotto – with four numbers it leaves; a computer picks when DS:15B4 =
    /// 0 (preset DS:08ED[k] or random(1); 1 → number 1, else number 2; leaves if it may not play); anybody leaves while the
    /// draw does not wait. Action 0 picks number 2, 1 picks number 1 (08CF:0E53), 3 leaves (ticket cleared).
    /// </summary>
    // [ORIGINAL] 08CF:0CD5
    public static void Lotto(Dgroup d, OriginalRandom random, IPurchaseOutput o)
    {
        sbyte picks = d.S8(At(P.Picks + Pl(d)));
        if (picks > 3) d.Set(At(P.Action + Pl(d)), (sbyte)3);
        if (d.U8(At(L.IsComputer + Pl(d))) == 1 && d.S16(P.ComputerDelay) == 0)
        {
            byte preset = d.U8(At(P.Preset + picks));
            byte pick = preset != 0xFF ? preset : unchecked((byte)random.Next(1));
            d.Set(At(P.Action + Pl(d)), pick == 1 ? (byte)1 : (byte)0);
            if (!CanPlayLotto(d)) d.Set(At(P.Action + Pl(d)), (sbyte)3);
        }
        if (d.S8(P.DrawState) != P.DrawWaiting) d.Set(At(P.Action + Pl(d)), (sbyte)3);
        switch (d.S8(At(P.Action + Pl(d))))
        {
            case 0:
                Pick(d, 2, o);
                return;
            case 1:
                Pick(d, 1, o);
                return;
            case 3:
                ClearTicket(d, o);
                OwnEmblem(d, o);
                d.Set(At(P.Mode + Pl(d)), P.Street);
                Sound(o, 0, 0x51);
                return;
        }
    }

    /// <summary>
    /// 08CF:0DEF: the player DS:37AA may play – cash at least the ticket price DS:40CC and, unless DS:08EE is preset, cash / 5
    /// at least the price too.
    /// </summary>
    // [ORIGINAL] 08CF:0DEF
    public static bool CanPlayLotto(Dgroup d)
    {
        short price = d.S16(L.LottoPrice);
        if (Cash(d) < price) return false;
        if (d.U8(P.Preset + 1) != 0xFF) return true;
        return LongMath.Div(Cash(d), 5) >= price;
    }

    /// <summary>
    /// 08CF:0E53(number): with cash of at least the ticket price the next number of the ticket is the given one (drawn, sound
    /// (0, 2Dh) for 1, (0, 2Eh) else; a human's input waits 12Ch); after the fourth the ticket price is paid, the emblem drawn
    /// and the player is back in the street.
    /// </summary>
    // [ORIGINAL] 08CF:0E53
    public static void Pick(Dgroup d, short number, IPurchaseOutput o)
    {
        short slot = d.S8(At(P.Picks + Pl(d)));
        if (Cash(d) < d.S16(L.LottoPrice)) return;
        d.Set(At(P.Tickets + 4 * Pl(d) + slot), unchecked((byte)number));
        DrawTicket(d, o, slot, number);
        Sound(o, 0, number == 1 ? (short)0x2D : (short)0x2E);
        if (d.U8(At(L.IsComputer + Pl(d))) == 0) d.Set(At(P.MoveDelay + 4 * Pl(d)), 0x12C);
        sbyte picks = unchecked((sbyte)(d.S8(At(P.Picks + Pl(d))) + 1));
        d.Set(At(P.Picks + Pl(d)), picks);
        if (picks <= 3) return;
        SetCash(d, Cash(d) - d.S16(L.LottoPrice));
        OwnEmblem(d, o);
        d.Set(At(P.Mode + Pl(d)), P.Street);
    }

    /// <summary>
    /// 08CF:0F4D: the draw, once per frame. While it waits the time to the next ball (DS:3780 − DS:159A) is shown when it
    /// changes. When the countdown reaches DS:159A the next ball (preset DS:08ED[k] once, else random(1) + 1) is drawn; after
    /// the fourth every player with four numbers counts its matches and gets the prize DS:34A2 for four, the ranks are
    /// computed (0825:06CB) and the next draw follows two steps later; after that a new draw is prepared (08CF:161F).
    /// </summary>
    // [ORIGINAL] 08CF:0F4D
    public static void Draw(Dgroup d, OriginalRandom random, IPurchaseOutput o)
    {
        if (d.S8(P.DrawState) == P.DrawWaiting)
        {
            short time = Int16Math.S(d.S16(P.Remaining) - d.S16(P.NextBall));
            if (time != d.S16(P.ShownTime))
            {
                d.Set(P.ShownTime, time);
                Emit(o, K.Number, Int16Math.S(d.S16(P.LottoX) + 0x29), 0x72, time, (short)(time < 0 ? -1 : 0), 1, 0x0E, 5);
            }
        }
        if (d.S16(P.Remaining) > d.S16(P.NextBall) || d.S8(P.DrawState) == P.DrawClosed) return;
        sbyte k = unchecked((sbyte)(d.S8(P.DrawState) + 1));
        d.Set(P.DrawState, k);
        if (k > 3)
        {
            NewDraw(d, o);
            for (d.Set(P.Player, (sbyte)0); d.S8(P.Player) < 4; d.Set(P.Player, unchecked((sbyte)(d.S8(P.Player) + 1))))
            {
                ClearTicket(d, o);
                d.Set(At(P.Picks + Pl(d)), (byte)0);
            }
            return;
        }
        byte preset = d.U8(At(P.Preset + k));
        byte ball;
        if (preset != 0xFF)
        {
            ball = preset;
            d.Set(At(P.Preset + k), (byte)0xFF);
        }
        else ball = unchecked((byte)(random.Next(1) + 1));
        d.Set(At(P.Balls + k), ball);
        Emit(o, K.Blit, 0xA0, Int16Math.S((sbyte)ball * 0x16), 0x20, 0x16, Int16Math.S((k << 5) + 0x20), 0xA8);
        Sound(o, 1, ball == 1 ? (short)0x2F : (short)0x30);
        d.Set(P.NextBall, Int16Math.S(d.S16(P.Remaining) - 1));
        if (k != 3) return;
        for (d.Set(P.Player, (sbyte)0); d.S8(P.Player) < 4; d.Set(P.Player, unchecked((sbyte)(d.S8(P.Player) + 1))))
        {
            int p = Pl(d);
            d.Set(At(P.Matches + p), (byte)0);
            if (d.S8(At(P.Picks + p)) <= 3) continue;
            for (int slot = 0; slot <= 3; slot++)
                if (d.U8(At(P.Tickets + 4 * p + slot)) == d.U8(P.Balls + slot))
                    d.Set(At(P.Matches + p), unchecked((byte)(d.U8(At(P.Matches + p)) + 1)));
            if (d.U8(At(P.Matches + p)) != 4) continue;
            SetCash(d, Cash(d) + d.S16(L.LottoPrize));
            if (d.U8(At(P.Mode + p)) != P.Street) DrawCash(d, o);
            else Emblem(d, o, 0);
        }
        Ranking.Apply(d);
        d.Set(P.BlinkStep, (byte)0);
        d.Set(P.NextBall, Int16Math.S(d.S16(P.Remaining) - 2));
    }

    /// <summary>
    /// 08CF:115C: every second call (DS:00EA) the winners' tickets blink – step 0 … 3 clears a number, step 4 … 7 shows the
    /// ball of that slot again (read as DS:15A3[step]).
    /// </summary>
    // [ORIGINAL] 08CF:115C
    public static void Blink(Dgroup d, IPurchaseOutput o)
    {
        short timer = Int16Math.S(d.S16(P.BlinkTimer) - 1);
        d.Set(P.BlinkTimer, timer);
        if (timer != 0) return;
        for (d.Set(P.Player, (sbyte)0); d.S8(P.Player) < 4; d.Set(P.Player, unchecked((sbyte)(d.S8(P.Player) + 1))))
        {
            if (d.U8(At(P.Matches + Pl(d))) != 4) continue;
            sbyte step = d.S8(P.BlinkStep);
            if (step <= 3) DrawTicket(d, o, step, 0);
            else DrawTicket(d, o, Int16Math.S(step - 4), d.S8(At(P.Matches + step)));
        }
        byte next = unchecked((byte)(d.U8(P.BlinkStep) + 1));
        d.Set(P.BlinkStep, next == 8 ? (byte)0 : next);
        d.Set(P.BlinkTimer, (short)2);
    }

    /// <summary>
    /// 08CF:161F: a new draw – balls cleared, the next ball at DS:3780 − 5; below 5 the draw is closed (DS:15A2 = F6h, the
    /// string DS:0136 instead of the prize), else it waits (FFh) with the prize DS:34A2 shown.
    /// </summary>
    // [ORIGINAL] 08CF:161F
    // Drawn in part 3 (scene): 08CF:1688 – 0A96:0267(A0h, 0, 20h, 16h, 20h, A8h, 80h, 16h); drawing only
    public static void NewDraw(Dgroup d, IPurchaseOutput o)
    {
        Emit(o, K.ClearBalls);
        short next = Int16Math.S(d.S16(P.Remaining) - 5);
        d.Set(P.NextBall, next);
        short x = d.S16(P.LottoX);
        if (next < 5)
        {
            Emit(o, K.Text, Int16Math.S(x + 0x16), 0x67, P.NoDrawText, DataSegment, 8, 5);
            d.Set(P.DrawState, P.DrawClosed);
        }
        else
        {
            short prize = d.S16(L.LottoPrize);
            Emit(o, K.Money, Int16Math.S(x + 0x15), 0x67, prize, (short)(prize < 0 ? -1 : 0), 4, 1, 5);
            d.Set(P.DrawState, P.DrawWaiting);
        }
    }

    /// <summary>
    /// 08CF:1348: the street – background, frame, the border 0609:13C5 (its random draws), shelves, the decoration of 2 × 8
    /// places (random(1) = 0 puts ball random(2) there), the shops, the gem factor, the clock, a new draw and the price line
    /// (08CF:1543) for lotto, oil, rockets and gems; then the list runs and the page is shown.
    /// </summary>
    // [ORIGINAL] 08CF:1348
    // Drawn in part 3 (scene): 08CF:15F5, 08CF:15C6 – 0A96:0030 from the atlas; drawing only
    public static void Setup(Dgroup d, OriginalRandom random, IPurchaseOutput o)
    {
        Emit(o, K.ClearList);
        Emit(o, K.DrawBackground);
        Emit(o, K.DrawFrame, 0, 0, 0x140, 0x4C);
        Planting.Border(0x20, 0x50, 0x100, 0x6E, random, o);
        Emit(o, K.Tiles, 0xA0, 0x42, 0x20, 0x16, 0x20, 0x50, 0x100, 0x6E);
        for (short row = 0; row <= 1; row++)
            for (short place = 0; place <= 7; place++)
            {
                if (random.Next(1) != 0) continue;
                short ball = random.Next(2);
                Emit(o, K.DrawDecoration, Int16Math.S((place << 5) + 0x20), Int16Math.S(row * 0x16 + 0x92), ball);
            }
        Emit(o, K.DrawShop, 0, d.S16(P.LottoX));
        if (d.U8(L.OilConsumption) == 1)
        {
            Emit(o, K.DrawShop, 1, d.S16(P.OilX));
            Emit(o, K.DrawShopOil, d.S16(L.ShopOil));
        }
        Emit(o, K.DrawShop, 2, d.S16(P.RocketX));
        if (d.U8(L.GemTrading) == 1)
        {
            Emit(o, K.DrawShop, 3, d.S16(P.GemX));
            Emit(o, K.Number, Int16Math.S(d.S16(P.GemX) + 0x14), 0x6A, d.S16(L.GemPriceFactor), d.S16(L.GemPriceFactor + 2), 3,
                2, 0x0C);
        }
        Emit(o, K.DrawClock, d.S16(P.Remaining));
        NewDraw(d, o);
        d.Set(P.TextX, (short)0x58);
        if (d.U8(L.OilConsumption) == 1) d.Set(P.TextX, Int16Math.S(d.S16(P.TextX) - 0x1D));
        if (d.U8(L.GemTrading) == 1) d.Set(P.TextX, Int16Math.S(d.S16(P.TextX) - 0x2C));
        PriceLine(d, o, P.TextLottoPrice, d.S16(L.LottoPrice), 3);
        if (d.U8(L.OilConsumption) == 1) PriceLine(d, o, P.TextOil, d.S16(L.OilPrice), 3);
        PriceLine(d, o, P.TextRockets, d.S16(L.RocketPrice), 3);
        if (d.U8(L.GemTrading) == 1) PriceLine(d, o, P.TextGems, GemValue(d), 4);
        Emit(o, K.RunList);
        Emit(o, K.ShowPage, d.S16(P.DrawPage));
    }

    /// <summary>
    /// 08CF:1543(text, value, digits): text k at DS:15B6, the x advanced by 6 per character (strlen 0000:42E5), the value with
    /// the digits after it, then the x advanced by 6 · (digits + 1) + 0Bh.
    /// </summary>
    // [ORIGINAL] 08CF:1543
    public static void PriceLine(Dgroup d, IPurchaseOutput o, int text, int value, short digits)
    {
        int at = P.TextTable + 4 * text;
        short offset = d.S16(at), segment = d.S16(at + 2);
        Emit(o, K.PriceText, d.S16(P.TextX), 0x45, offset, segment, 9, 0x0B, -1, 1);
        ushort length = IncidentText.Length(o.Texts, (ushort)offset, (ushort)segment);
        d.Set(P.TextX, Int16Math.S(Int16Math.S(length * 6) + d.S16(P.TextX)));
        Emit(o, K.PriceNumber, d.S16(P.TextX), 0x45, (short)value, (short)(value >> 16), digits, 9, 0x0B, -1, 1);
        d.Set(P.TextX, Int16Math.S(Int16Math.S(Int16Math.S(digits + 1) * 6) + d.S16(P.TextX) + 0x0B));
    }

    /// <summary>
    /// 08CF:178D(direction): gems of the player DS:37AA + direction (8 bit); one gem drawn for +1 (slot gems − 1), the slot of
    /// the new count cleared otherwise (five per row, 8 pixels apart).
    /// </summary>
    // [ORIGINAL] 08CF:178D
    public static void ChangeGems(Dgroup d, sbyte direction, IPurchaseOutput o)
    {
        int at = At(L.Gems + Pl(d));
        d.Set(at, unchecked((byte)(d.U8(at) + (byte)direction)));
        short gems = d.S8(At(L.Gems + Pl(d)));
        if (direction == 1)
        {
            short slot = Int16Math.S(gems - 1);
            Emit(o, K.Blit, 0x109, 0xC6, 7, 7, GemX(d, slot), GemY(slot));
        }
        else Emit(o, K.FillRect, GemX(d, gems), GemY(gems), 7, 7, 5);
    }

    private static short GemX(Dgroup d, short slot) => Int16Math.S((Pl(d) << 6) + (Int16Math.Mod(slot, 5) << 3) + 0x26);

    private static short GemY(short slot) => Int16Math.S((Int16Math.Div(slot, 5) << 3) + 0x14);

    /// <summary>08CF:172A – the gems of the player DS:37AA, one 7 × 7 picture per gem.</summary>
    // Drawn in part 3 (scene): 08CF:172A – 0A96:0030(109h, C6h, 7, 7, x, y) per gem; drawing only
    private static void DrawGems(Dgroup d, IPurchaseOutput o) => Emit(o, K.DrawGems, (short)Pl(d), d.S8(At(L.Gems + Pl(d))));

    /// <summary>08CF:1859 – the gem account of the player DS:37AA.</summary>
    // Drawn in part 3 (scene): 08CF:1859 – 0A96:0525(p·40h + 26h, 2Ah, DS:013C, 0Dh, 5), 0A96:057A(p·40h + 34h, 2Ah, account, 4, 0, 5)
    private static void DrawGemCredit(Dgroup d, IPurchaseOutput o) =>
        Emit(o, K.DrawGemCredit, (short)Pl(d), d.S16(At(L.GemCredit + 2 * Pl(d))));

    /// <summary>0609:1978 – the rockets of the player DS:37AA.</summary>
    private static void DrawRockets(Dgroup d, IPurchaseOutput o) =>
        Emit(o, K.DrawRockets, (short)Pl(d), d.S16(At(L.Rockets + 2 * Pl(d))));
}
