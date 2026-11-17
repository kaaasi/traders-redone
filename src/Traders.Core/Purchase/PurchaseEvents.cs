using Traders.Core.Field;
using Traders.Core.State;

namespace Traders.Core.Purchase;

/// <summary>
/// Drawing and sound requests of the purchase (finding docs/routines/08CF_0004_einkauf.md), in the original's order with its
/// arguments. Requests of the purchase's own drawing routines (segment 08CF) carry the player DS:37AA and the data segment values
/// the routine reads; the scene draws from them. None of these requests changes game state.
/// </summary>
public enum PurchaseEventKind
{
    /// <summary>21A7:0054(channel, sound) – sound request.</summary>
    Sound,

    /// <summary>0609:1879(x, y, rank) – emblem of the player DS:37AA (with the rank badge for rank &gt; 0).</summary>
    Emblem,

    /// <summary>0609:18E5 – empty panel of the player DS:37AA.</summary>
    EmptyPanel,

    /// <summary>0609:1978 – rockets of the player DS:37AA.</summary>
    DrawRockets,

    /// <summary>0609:1AC0 – oil bar of the player DS:37AA.</summary>
    DrawOilBar,

    /// <summary>0609:1ED9 – background tiles.</summary>
    DrawBackground,

    /// <summary>0609:1256(x, y, w, h) – frame.</summary>
    DrawFrame,

    /// <summary>0A96:001B – empties the drawing list.</summary>
    ClearList,

    /// <summary>0A96:06A8 – executes the drawing list.</summary>
    RunList,

    /// <summary>0A96:0C3F(page DS:082E) – copies the drawn page to the shown one.</summary>
    ShowPage,

    /// <summary>0A96:0030(sx, sy, w, h, x, y) – atlas blit.</summary>
    Blit,

    /// <summary>0A96:0267(sx, sy, w, h, x, y, area w, area h) – area tiled with an atlas tile.</summary>
    Tiles,

    /// <summary>0A96:0433(x, y, x2, y2, colour) – line.</summary>
    Line,

    /// <summary>0A96:04D7(x, y, w, h, colour) – filled rectangle.</summary>
    FillRect,

    /// <summary>0A96:0525(x, y, text offset, text segment, colour, shadow) – text.</summary>
    Text,

    /// <summary>0A96:057A(x, y, low, high, digits, colour, shadow) – number.</summary>
    Number,

    /// <summary>0A96:05FB(x, y, low, high, digits, colour, shadow) – amount of money.</summary>
    Money,

    /// <summary>0A69:004D(x, y, text offset, text segment, colour, shadow, −1, 1) – price line text (08CF:1543).</summary>
    PriceText,

    /// <summary>0A69:0164(x, y, low, high, digits, colour, shadow, −1, 1) – price line number (08CF:1543).</summary>
    PriceNumber,

    /// <summary>08CF:193E(player, cash low, cash high) – cash of the player.</summary>
    DrawCash,

    /// <summary>08CF:1981(shop oil) – oil stock of the shop.</summary>
    DrawShopOil,

    /// <summary>08CF:1859(player, gem credit) – gem account of the player.</summary>
    DrawGemCredit,

    /// <summary>08CF:172A(player, gems) – gems of the player.</summary>
    DrawGems,

    /// <summary>08CF:16B4(slot, number, player) – one lotto number of the player's ticket (number 0 = empty).</summary>
    DrawTicket,

    /// <summary>08CF:16F5(player) – clears the player's lotto ticket.</summary>
    ClearTicket,

    /// <summary>08CF:15C6(shop, x) – picture of a shop.</summary>
    DrawShop,

    /// <summary>08CF:15F5(x, y, ball) – one decoration ball of the lotto shop.</summary>
    DrawDecoration,

    /// <summary>08CF:1688 – clears the drawn balls.</summary>
    ClearBalls,

    /// <summary>08CF:18B5(time) – the clock with the countdown DS:3780.</summary>
    DrawClock,
}

/// <summary>A request of the purchase to scene or sound output, with the original's arguments in call order.</summary>
public sealed record PurchaseEvent(PurchaseEventKind Kind, params short[] Args)
{
    public override string ToString() => $"{Kind}({string.Join(", ", Args)})";
}

/// <summary>
/// What the purchase needs from outside its rules: the output of its requests and of the field routines it calls (0609:1FE0,
/// 0609:1B72, 0609:13C5) and the memory behind the far text pointers (strlen in 08CF:1543).
/// </summary>
public interface IPurchaseOutput : IFieldOutput
{
    void Emit(PurchaseEvent e);

    /// <summary>Memory behind the far text pointers DS:37D0 + 4k.</summary>
    Events.ITextMemory Texts { get; }
}
