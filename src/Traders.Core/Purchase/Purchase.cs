using Traders.Core.Field;
using Traders.Core.Rules;
using Traders.Core.State;
using K = Traders.Core.Purchase.PurchaseEventKind;
using L = Traders.Core.State.DgroupLayout;
using P = Traders.Core.Purchase.PurchaseLayout;
using R = Traders.Core.Purchase.PurchaseRules;

namespace Traders.Core.Purchase;

/// <summary>
/// What the purchase needs from outside besides its output: the control reader 0E9E:037A (the human players' commands, written
/// as actions DS:3308) and the pause check 21FD:0245 of the countdown tick 21FD:04F9.
/// </summary>
public interface IPurchaseHost : IPurchaseOutput
{
    /// <summary>0E9E:037A – reads the controls and writes the actions DS:3308 (ControlReader.ReadActions).</summary>
    void ReadControls(Dgroup d);

    /// <summary>21FD:0245 – pause key check; the frames it waits (<see cref="Planting.Pause"/> in the game).</summary>
    IEnumerable<FieldWait> PauseCheck(Dgroup d);
}

/// <summary>
/// The purchase of the week 08CF:0004 (call 29 of the week loop 1777:01F8; finding docs/routines/08CF_0004_einkauf.md) frame by
/// frame: the street with its shops is set up, every player gets its figure and panel, then the shop loop 08CF:01BD runs one
/// frame per pass until the countdown DS:3780 has run out. The rules are <see cref="PurchaseRules"/>; drawing and sound are
/// emitted as <see cref="PurchaseEvent"/>s and <see cref="FieldEvent"/>s in the original's order. Every yielded
/// <see cref="FieldWait"/> is one frame (a retrace or a poll of the pause loop).
/// </summary>
public static class Purchase
{
    // Retrace waits per stage of the fades 21FD:037D (0185(4)) and 21FD:0329 (0185(3)), as in the planting phase.
    private const int FadeOutRetraces = 4, FadeInRetraces = 3;

    /// <summary>DS offset of the palette argument of 21FD:026C (as in the planting phase).</summary>
    private const short Palette = 0x00A8;

    private static int Pl(Dgroup d) => d.S8(P.Player);
    private static int At(int offset) => Int16Math.U(offset);

    private static void Emit(IFieldOutput o, FieldEventKind k, params short[] args) => o.Emit(new FieldEvent(k, args));
    private static void Emit(IPurchaseOutput o, K k, params short[] args) => o.Emit(new PurchaseEvent(k, args));

    /// <summary>Loop over the players with the global loop variable DS:37AA (sbyte, ends at 4) like the original.</summary>
    private static IEnumerable<FieldWait> ForPlayers(Dgroup d, Func<IEnumerable<FieldWait>> body)
    {
        for (d.Set(P.Player, (sbyte)0); d.S8(P.Player) < 4; d.Set(P.Player, unchecked((sbyte)(d.S8(P.Player) + 1))))
            foreach (var w in body()) yield return w;
    }

    private static void ForPlayers(Dgroup d, Action body)
    {
        for (d.Set(P.Player, (sbyte)0); d.S8(P.Player) < 4; d.Set(P.Player, unchecked((sbyte)(d.S8(P.Player) + 1))))
            body();
    }

    /// <summary>
    /// 08CF:0004: picture DS:08CE (186C:0002), the cursor objects 0609:1F2D, the shop positions 08CF:0082, the countdown
    /// 21FD:04E5(time limit DS:08F8, 28h), fade-out 21FD:037D(0), the street 08CF:1348, ranks 0825:06CB, the players 08CF:00CD,
    /// base palette 21FD:026C(DS:00A8), weather colours 0609:11E7, fade-in 21FD:0329, empty list, the shop loop 08CF:01BD; then
    /// every player's cursor off (0609:21BA) and the objects off (21AF:000D).
    /// </summary>
    // [ORIGINAL] 08CF:0004
    public static IEnumerable<FieldWait> Run(Dgroup d, OriginalRandom random, IPurchaseHost host)
    {
        Emit(host, FieldEventKind.LoadImage, d.S16(Planting.PictureNumber));
        Planting.CursorObjects(d, host);
        R.Layout(d);
        Countdown.Start(d, d.U8(L.TimeLimit), 0x28);
        Emit(host, FieldEventKind.FadeOut, 0);
        foreach (var w in Planting.Fade(d, FadeOutRetraces)) yield return w;
        R.Setup(d, random, host);
        foreach (var w in ShowPageFrames(d)) yield return w;
        Ranking.Apply(d);
        foreach (var w in Players(d, host)) yield return w;
        Emit(host, FieldEventKind.BasePalette, Palette);
        Planting.WeatherColours(d, host);
        Emit(host, FieldEventKind.FadeIn);
        foreach (var w in Planting.Fade(d, FadeInRetraces)) yield return w;
        Emit(host, FieldEventKind.ClearList);
        foreach (var w in Shops(d, random, host)) yield return w;
        ForPlayers(d, () => FieldCursor.HideCursor(d));
        Planting.InitObjects(d, host);
    }

    /// <summary>
    /// The page copy 0A96:0C3F at the end of 08CF:1348 (<see cref="PurchaseRules.Setup"/> emits it): on cards 2 and 63h a flip
    /// first, which takes frames; the copy itself is the output's.
    /// </summary>
    // [ORIGINAL] 0A96:0C3F – 21FD:051F when DS:090E = 2 or 63h, then the page copy (output)
    private static IEnumerable<FieldWait> ShowPageFrames(Dgroup d)
    {
        if (d.S16(Planting.CardMode) is not (2 or 0x63)) yield break;
        foreach (var w in Planting.Flip(d)) yield return w;
    }

    /// <summary>
    /// 08CF:00CD: per player an empty list, the figure at x = DS:15A0 + DS:00E4[p], y = 3p + 79h (object record 1Ah · p), the
    /// emblem 0609:1879(40h·p + 20h, 6, rank) and the oil bar 0609:1AC0; mode 0Ah, in front of the lotto, no move, no input
    /// delay, no picks and no gem deals; the cursor shown (0609:1FCC) and both pages drawn (21FD:060D).
    /// </summary>
    // [ORIGINAL] 08CF:00CD
    // [ORIGINAL] 21FD:060D – 0A96:06A8, 21FD:051F, 0A96:06A8
    public static IEnumerable<FieldWait> Players(Dgroup d, IPurchaseHost host) => ForPlayers(d, () => Player(d, host));

    private static IEnumerable<FieldWait> Player(Dgroup d, IPurchaseHost host)
    {
        Emit(host, FieldEventKind.ClearList);
        int p = Pl(d);
        int obj = Int16Math.S(p * Planting.ObjectSize);
        d.Set(At(P.ObjectX + obj), Int16Math.S(d.S16(P.LottoX) + d.U8(At(P.FigureX + p))));
        d.Set(At(P.ObjectY + obj), Int16Math.S(p * 3 + 0x79));
        Emit(host, K.Emblem, Int16Math.S((p << 6) + 0x20), 6, d.U8(At(L.Rank + p)));
        Emit(host, K.DrawOilBar, (short)p);
        d.Set(At(P.Mode + p), P.Street);
        d.Set(At(P.Shop + p), (byte)0);
        d.Set(At(P.MoveFrame + 2 * p), (short)0);
        d.Set(At(P.MoveTimer + 2 * p), (short)0);
        d.Set(At(P.MoveDelay + 4 * p), 0);
        d.Set(At(P.Picks + p), (byte)0);
        d.Set(At(P.GemDeals + p), (byte)0);
        Planting.ShowCursor(d, host);
        Emit(host, FieldEventKind.ListOnBothPages);
        foreach (var w in Planting.Flip(d)) yield return w;
    }

    /// <summary>
    /// 08CF:01BD: passes until the countdown DS:3780 is 0 (the test at the end, so at least one), then the last frame 21FD:0665.
    /// </summary>
    // [ORIGINAL] 08CF:01BD
    // [ORIGINAL] 21FD:0665 – 21FD:051F, 23CD:071C, 0A96:06A8, 23CD:06E9
    public static IEnumerable<FieldWait> Shops(Dgroup d, OriginalRandom random, IPurchaseHost host)
    {
        do
            foreach (var w in Pass(d, random, host)) yield return w;
        while (d.S16(P.Remaining) != 0);
        Emit(host, FieldEventKind.LastFrame);
        foreach (var w in Planting.Flip(d)) yield return w;
    }

    /// <summary>
    /// One pass of 08CF:01BD: flip, restore, list, empty list, 21FD:0185(2), controls, the computer delay DS:15B4 − 1 (below 0
    /// back to 5), every player's frame by its mode (0Ah street 08CF:029A, 0Bh rockets 09F1, 0Ch oil 04BA, 0Dh lotto 0CD5, 0Eh
    /// gems 069F; others nothing), the blinking 08CF:115C after the fourth ball, the draw 08CF:0F4D, the countdown tick
    /// 21FD:04F9 with its pause check (a step redraws the clock, 08CF:1935), list and objects.
    /// </summary>
    public static IEnumerable<FieldWait> Pass(Dgroup d, OriginalRandom random, IPurchaseHost host)
    {
        Emit(host, FieldEventKind.FlipPages);
        foreach (var w in Planting.Flip(d)) yield return w;
        Emit(host, FieldEventKind.RestoreObjects);
        Emit(host, FieldEventKind.RunList);
        Emit(host, FieldEventKind.ClearList);
        Emit(host, FieldEventKind.Delay, 2, 0);
        foreach (var w in Planting.Retraces(2, 0)) yield return w;
        host.ReadControls(d);
        short delay = Int16Math.S(d.S16(P.ComputerDelay) - 1);
        d.Set(P.ComputerDelay, delay < 0 ? (short)5 : delay);
        ForPlayers(d, () =>
        {
            switch (d.U8(At(P.Mode + Pl(d))))
            {
                case P.Street: R.Street(d, random, host); break;
                case P.RocketShop: R.RocketShop(d, host); break;
                case P.OilShop: R.OilShop(d, host); break;
                case P.Lotto: R.Lotto(d, random, host); break;
                case P.GemShop: R.GemShop(d, host); break;
            }
        });
        if (d.S8(P.DrawState) == 3) R.Blink(d, host);
        R.Draw(d, random, host);
        foreach (var w in host.PauseCheck(d)) yield return w;
        // 08CF:1935 – 08CF:18B5 (the clock), drawn by the scene (PurchaseScene)
        if (Countdown.Tick(d)) Emit(host, K.DrawClock, d.S16(P.Remaining));
        Emit(host, FieldEventKind.RunList);
        Emit(host, FieldEventKind.DrawObjects);
    }
}
