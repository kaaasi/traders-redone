using Traders.Core.Field;
using Traders.Core.State;

namespace Traders.Core.Purchase;

/// <summary>
/// Data segment addresses of the purchase 08CF:0004 (finding docs/routines/08CF_0004_einkauf.md, docs/analyse/DATENLAYOUT.md
/// section of lane einkauf). Every player walks along a row of shops on V-ANBAU (lotto, oil, rockets, gems; oil only with OIL
/// CONSUMPTION, gems only with GEM TRADING), enters one with "up" and buys or sells there until it leaves with "down".
/// </summary>
public static class PurchaseLayout
{
    /// <summary>sbyte, player loop variable.</summary>
    public const int Player = DgroupLayout.ActivePlayer;

    /// <summary>byte[4], mode of the player: <see cref="Street"/> or one of the shops (dispatch of 08CF:01BD).</summary>
    public const int Mode = FieldCursor.Mode;

    /// <summary>Mode: walking along the shops (08CF:029A).</summary>
    public const byte Street = 0x0A;

    /// <summary>Mode: in the rocket shop (08CF:09F1).</summary>
    public const byte RocketShop = 0x0B;

    /// <summary>Mode: in the oil shop (08CF:04BA).</summary>
    public const byte OilShop = 0x0C;

    /// <summary>Mode: at the lotto (08CF:0CD5).</summary>
    public const byte Lotto = 0x0D;

    /// <summary>Mode: in the gem shop (08CF:069F).</summary>
    public const byte GemShop = 0x0E;

    /// <summary>sbyte[4], action of the player from 0E9E:037A or the computer's choice (0 right, 1 left, 2 up, 3 down, 10 fire).</summary>
    public const int Action = FieldCursor.Action;

    /// <summary>sbyte[4], shop in front of the player: 0 lotto, 1 oil, 2 rockets, 3 gems.</summary>
    public const int Shop = 0x15AB;

    /// <summary>word[4], direction of the running move (0609:1FE0): 0 right, 1 left.</summary>
    public const int Direction = FieldCursor.Direction;

    /// <summary>word[4], frames left of the running move (21h after a step).</summary>
    public const int MoveTimer = FieldCursor.MoveTimer;

    /// <summary>word[4], start frame of the move (cleared in 08CF:00CD).</summary>
    public const int MoveFrame = FieldCursor.MoveFrame;

    /// <summary>dword[4], input repeat delay of 0E9E (3 after an oil unit, 64h after a gem, 12Ch after a lotto pick of a human).</summary>
    public const int MoveDelay = FieldCursor.MoveDelay;

    /// <summary>sbyte[4], lotto numbers picked by the player in this draw (0 … 4).</summary>
    public const int Picks = 0x1586;

    /// <summary>byte[4][4], picked numbers (1 or 2) of the player, stride 4.</summary>
    public const int Tickets = 0x158A;

    /// <summary>sbyte[4], gem deals of the player in this purchase (limits the computer).</summary>
    public const int GemDeals = 0x159C;

    /// <summary>word, x of the lotto shop; the other shops follow in steps of 40h (08CF:0082).</summary>
    public const int LottoX = 0x15A0;

    /// <summary>word, x of the oil shop (only written with OIL CONSUMPTION).</summary>
    public const int OilX = 0x15B0;

    /// <summary>word, x of the rocket shop.</summary>
    public const int RocketX = 0x15B8;

    /// <summary>word, x of the gem shop.</summary>
    public const int GemX = 0x15B2;

    /// <summary>sbyte, state of the draw: FFh waiting, 0 … 3 balls drawn, F6h no more draws.</summary>
    public const int DrawState = 0x15A2;

    /// <summary>Draw state: waiting for the next ball.</summary>
    public const sbyte DrawWaiting = -1;

    /// <summary>Draw state: too little time left, no more draws.</summary>
    public const sbyte DrawClosed = -10;

    /// <summary>byte[4], matching numbers of the player after the fourth ball (4 = prize).</summary>
    public const int Matches = 0x15A3;

    /// <summary>byte[4], drawn balls (1 or 2); index 4 … 7 of <see cref="Matches"/> reads them.</summary>
    public const int Balls = 0x15A7;

    /// <summary>sbyte, blink step of the winners' numbers 0 … 7 (08CF:115C).</summary>
    public const int BlinkStep = 0x15AF;

    /// <summary>word, countdown value at which the next ball is drawn.</summary>
    public const int NextBall = 0x159A;

    /// <summary>word, frames to the next computer decision: 5 … 0 (the computers act at 0).</summary>
    public const int ComputerDelay = 0x15B4;

    /// <summary>word, x of the next price line (08CF:1543).</summary>
    public const int TextX = 0x15B6;

    /// <summary>word, time to the next ball as last shown (initially −1).</summary>
    public const int ShownTime = 0x00E8;

    /// <summary>word, frames to the next blink step (initially 1).</summary>
    public const int BlinkTimer = 0x00EA;

    /// <summary>byte[4], x offset of the player's figure on the lotto shop (25h, 1Dh, 23h, 19h).</summary>
    public const int FigureX = 0x00E4;

    /// <summary>byte[…], preset lotto numbers (FFh = none; the computer's picks and the balls use them once).</summary>
    public const int Preset = 0x08ED;

    /// <summary>word[4], rockets the computer still needs (lane planung, 0825:104B).</summary>
    public const int RocketShortfall = 0x3046;

    /// <summary>word, countdown of the purchase (21FD:04E5 with the time limit DS:08F8, period 28h).</summary>
    public const int Remaining = Rules.Countdown.Remaining;

    /// <summary>Gem trade account DS:300E[p] may not exceed this.</summary>
    public const int MaxGemCredit = 2000;

    /// <summary>Most gems a player can hold.</summary>
    public const sbyte MaxGems = 10;

    /// <summary>Most rockets a player can buy up to.</summary>
    public const short MaxRockets = 10;

    /// <summary>Most oil a player can buy up to.</summary>
    public const short MaxOil = 0x28;

    /// <summary>Object x of the player's figure: word in the object record of 1Ah bytes (21AF).</summary>
    public const int ObjectX = Planting.ObjectX;

    /// <summary>Object y of the player's figure.</summary>
    public const int ObjectY = Planting.ObjectY;

    /// <summary>Table of far text pointers: text k at DS:37D0 + 4k.</summary>
    public const int TextTable = 0x37D0;

    /// <summary>Texts of the purchase: 16 broke, 17 down, 18 lotto, 19 gems, 20 rockets, 21 oil, 22 lotto (price line).</summary>
    public const int TextBroke = 16, TextDown = 17, TextLotto = 18, TextGems = 19, TextRockets = 20, TextOil = 21,
        TextLottoPrice = 22;

    /// <summary>word, index of the page being drawn (argument of 0A96:0C3F).</summary>
    public const int DrawPage = 0x082E;

    /// <summary>Near string in the data segment shown instead of the prize when no draw is left (08CF:161F).</summary>
    public const int NoDrawText = 0x0136;
}
