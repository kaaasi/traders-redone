using System.Runtime.CompilerServices;
using Traders.Core.Field;
using Traders.Core.Harvest;
using Traders.Core.Input;
using Traders.Core.State;
using Traders.Core.Week;

namespace Traders.Core.FatMike;

/// <summary>
/// Fat Mike 1F63:000F (call 26 of the week loop; finding docs/routines/1F63_000F_fatmike.md): Fat Mike offers his protection
/// in up to three rounds at a price each; a player pays with his stock of kind 3 (DS:3CFA) by pressing fire while the offer
/// runs, computers decide at a fixed frame by their own rule. Who has not paid when the offers end (or when nobody paid at
/// all) is punished on the field: crops destroyed or fields lost, by a count from his fields. Works on the data segment like
/// the original; drawing and sound are emitted as events in the original's order, every yielded <see cref="FieldWait"/> is
/// one frame.
/// </summary>
public static class FatMikePhase
{
    // DS offsets (docs/analyse/DATENLAYOUT.md, section of lane fatmike).
    public const int PictureNumber = 0x08D4;  // word, picture of the scene (V-FATMIK)
    public const int Lamps = 0x08ED;          // byte[4], indicator mask of the blink sequence 0305
    public const int Bidders = 0x4890;        // byte[4], 1 = has the largest stock 3 (0C82)
    public const int Contest = 0x54B1;        // byte, 0 = Fat Mike, 1 = contest 0B03 (players with DS:4BC2 = 1 left out)
    public const int Unit = 0x54B2;           // word, stock units per pixel of the scale (max/120 + 1)
    public const int Base = 0x54B4;           // word, base price: total stock 3 / 24, at most 30
    public const int Price = 0x54B8;          // word, price of the running offer
    public const int Paid = 0x54BA;           // word[4], 1 = has paid
    public const int Lots = 0x54C2;           // word[3], the three prices
    public const int NobodyPaid = 0x54C8;     // byte, 1 = nobody paid in any round
    public const int Slide = 0x54CA;          // word, x of the object row (moves toward the price)
    public const int Lot = 0x54CC;            // word, running round 0…2 (loop variable)
    public const int Verdict = 0x54CE;        // byte[4], 0 nothing, 1 crops destroyed, 2 fields lost
    public const int Cheapest = 0x54D2;       // word, round with the lowest price
    public const int PaidText = 0x103C;       // string in DS, shown when a player pays
    public const int Colour = 0x101C;         // byte[4], colour of the player's texts
    public const int Patience = 0x1020;       // byte[IQ], rounds of 31 frames an offer lasts (by DS:2E8E)
    public const int LampX = 0x1024, LampY = 0x1026, LampX2 = 0x1028, LampY2 = 0x102C; // atlas of 0FDF (byte, byte, word, byte)
    public const int RowX = 0x102E, RowY = 0x1035; // byte[7], object offsets of the row (143E)
    public const int NoticeLines = 0x3B64;    // far pointer[2], the typed notice of the scene
    public const int WarningLines = 0x3B74;   // far pointer[2], notice on the field when nobody paid
    public const int PunishLines = 0x3B94;    // far pointer[3], notice on the field when nobody paid
    public const int ExcludedText = 0x3BA0;   // far pointer, text instead of a stock in the contest
    public const int TextTable = 0x37CC;      // far pointer[305], text table (lines EC… and EF… for 08DA)
    public const int Palette = 0x0FFC;        // the palette argument of 21FD:026C
    public const int FieldPalette = 0x00A8;

    private const int Stock3 = DgroupLayout.Stock + 6;

    private static int P(Dgroup d) => d.S8(DgroupLayout.ActivePlayer);
    private static int At(int offset) => Int16Math.U(offset);
    private static int StockOf(int p) => At(Stock3 + Int16Math.S(p << 3));
    private static short Row(int p) => Int16Math.S(p * 0x21);

    private static void Emit(IFatMikeHost h, FieldEventKind k, params short[] args) => h.Emit(new FieldEvent(k, args));
    private static void Emit(IFatMikeHost h, FatMikeEventKind k, params short[] args) => h.Emit(new FatMikeEvent(k, args));

    private static void ForPlayers(Dgroup d, Action<int> body)
    {
        for (d.Set(DgroupLayout.ActivePlayer, (sbyte)0); d.S8(DgroupLayout.ActivePlayer) < 4;
             d.Set(DgroupLayout.ActivePlayer, unchecked((sbyte)(d.S8(DgroupLayout.ActivePlayer) + 1))))
            body(P(d));
    }

    /// <summary>
    /// The whole of 1F63:000F frame by frame: picture, objects, fade-out, the scene 0D24, palette, the object row 13FE, fade-in,
    /// the stock scale 01B2, the typed notice, 50 retraces, with random(6) = 0 the blinking 0305, the prices 0390, then up to
    /// three offers (147B, 04D8) while somebody is left to pay, the last indicator, 100 retraces, the verdicts 0711, objects off
    /// (21AF:0472) and the punishment on the field 079A.
    /// </summary>
    // [ORIGINAL] 1F63:000F
    public static IEnumerable<FieldWait> Run(Dgroup d, OriginalRandom random, IFatMikeHost host)
    {
        var h = new Listing(host);
        var indicator = new WaitLoop(d, random, KeyboardOnlyDevices.Instance, 4);
        Emit(h, FieldEventKind.LoadImage, d.S16(PictureNumber));
        Objects(d, h);
        Emit(h, FieldEventKind.FadeOut, 0);
        foreach (var w in Planting.Fade(d, 4)) yield return w;
        foreach (var w in SceneFrames(d, h)) yield return w;
        d.Set(Contest, (byte)0);
        Emit(h, FieldEventKind.BasePalette, Palette);
        foreach (var w in ShowRowFrames(d, h)) yield return w;
        Emit(h, FieldEventKind.FadeIn);
        foreach (var w in Planting.Fade(d, 3)) yield return w;
        foreach (var w in ScaleFrames(d, h, new StrongBox<short>())) yield return w;
        foreach (var w in Traders.Core.Harvest.Harvest.TypeLines(d, new Typewriter(h), 0xB1, 0x0A, 0x8F, NoticeLines, 2, 8, 3))
            yield return w;
        foreach (var w in Delay(h, 0x32)) yield return w;
        if (random.Next(6) == 0)
            foreach (var w in BlinkFrames(d, random, h)) yield return w;
        Emit(h, FieldEventKind.RestoreObjects);
        Prices(d, random);
        d.Set(Planting.Players, (byte)4);
        ForPlayers(d, p => d.Set(At(Paid + Int16Math.S(p << 1)), (short)0));
        for (d.Set(Lot, (short)0); d.S16(Lot) <= 2 && d.U8(Planting.Players) != 0; d.Set(Lot, Int16Math.S(d.S16(Lot) + 1)))
        {
            d.Set(Price, d.S16(At(Lots + Int16Math.S(d.S16(Lot) << 1))));
            foreach (var w in SlideFrames(d, h, d.S16(Price))) yield return w;
            if (d.S16(Lot) == 2)
                foreach (var w in IndicatorFrames(d, h, 1)) yield return w;
            switch (d.S16(Lot))
            {
                case 0: Emit(h, FieldEventKind.Sound, 2, 0x21); break;
                case 1: Emit(h, FieldEventKind.Sound, 2, 0x22); break;
                case 2: Emit(h, FieldEventKind.Sound, 2, 0x23); break;
            }
            foreach (var w in OfferFrames(d, random, h, indicator)) yield return w;
        }
        Emit(h, FieldEventKind.Sound, 2, 0);
        d.Set(NobodyPaid, (byte)(d.U8(Planting.Players) == 4 ? 1 : 0));
        foreach (var w in IndicatorFrames(d, h, (short)(d.U8(Planting.Players) == 0 ? 0 : 2))) yield return w;
        foreach (var w in Delay(h, 0x64)) yield return w;
        Verdicts(d);
        HideObjects(d, h);
        foreach (var w in PunishFrames(d, random, h)) yield return w;
    }

    /// <summary>
    /// 1F63:01B2 (result in <paramref name="max"/>): the largest stock 3 (0C82), the scale unit max/120 + 1, the scale with four
    /// marks (lines and numbers at 30, 60, 90, 120 pixels), per player his stock (0EC7) or, in the contest for players with
    /// DS:4BC2 = 1, the text DS:3BA0 (0F46), shown on both pages; sound (0, 24h); then the bars grow pixel by pixel (1 … 120)
    /// while the largest stock reaches the pixel, each step on both pages.
    /// </summary>
    // [ORIGINAL] 1F63:01B2
    public static IEnumerable<FieldWait> ScaleFrames(Dgroup d, IFatMikeHost host, StrongBox<short> max)
    {
        max.Value = MaxStock(d);
        d.Set(Unit, Int16Math.S(Int16Math.Div(max.Value, 0x78) + 1));
        Emit(host, FieldEventKind.ClearList);
        for (short si = 0x1E; si <= 0x78; si = Int16Math.S(si + 0x1E))
        {
            Emit(host, FatMikeEventKind.VerticalLine, Int16Math.S(si + 0x27), 0x0B, 5, 0);
            Emit(host, FatMikeEventKind.VerticalLine, Int16Math.S(si + 0x26), 0x0C, 5, 0x0B);
            short v = Int16Math.S(d.S16(Unit) * si); // imul, then cwd
            Emit(host, FatMikeEventKind.ShadowNumber, Int16Math.S(si + 0x0D), 0x0F, v, (short)(v < 0 ? -1 : 0), 4, 8, 0, -1, 1);
        }
        ForPlayers(d, p =>
        {
            if (LeftOut(d, p)) ShowExcluded(d, host);
            else ShowStock(d, host);
        });
        foreach (var w in BothPages(d, host)) yield return w;
        Emit(host, FieldEventKind.Sound, 0, 0x24);
        for (short di = 1; di <= 0x78; di++)
        {
            if (Int16Math.Div(max.Value, d.S16(Unit)) < di) break;
            Emit(host, FieldEventKind.ClearList);
            for (d.Set(DgroupLayout.ActivePlayer, (sbyte)0); d.S8(DgroupLayout.ActivePlayer) < 4;
                 d.Set(DgroupLayout.ActivePlayer, unchecked((sbyte)(d.S8(DgroupLayout.ActivePlayer) + 1))))
            {
                int p = P(d);
                if (LeftOut(d, p)) continue;
                if (Int16Math.Div(d.S16(StockOf(p)), d.S16(Unit)) >= di) Bar(host, (short)p, di);
            }
            foreach (var w in BothPages(d, host)) yield return w;
        }
    }

    /// <summary>In the contest (DS:54B1 = 1) players with DS:4BC2 = 1 take no part.</summary>
    private static bool LeftOut(Dgroup d, int p) =>
        d.U8(Contest) == 1 && d.U8(At(DgroupLayout.FatMikeFlag + p)) == 1;

    /// <summary>
    /// 1F63:0305: random(3) + 1 times (0 … 3 inclusive) a mask random(1) + 1 (1 left, 2 right) stored in DS:08ED[i]; the indicator
    /// in state 0, flip, 10 retraces, flip, the indicator in state 1, 20 retraces.
    /// </summary>
    // [ORIGINAL] 1F63:0305
    public static IEnumerable<FieldWait> BlinkFrames(Dgroup d, OriginalRandom random, IFatMikeHost host)
    {
        short n = random.Next(3);
        for (short si = 0; si <= n; si++)
        {
            short v = Int16Math.S(random.Next(1) + 1);
            d.Set(At(Lamps + si), unchecked((byte)v));
            Emit(host, FieldEventKind.ClearList);
            Emit(host, FatMikeEventKind.Indicator, v, 0);
            Emit(host, FieldEventKind.RunList);
            foreach (var w in Flip(d, host)) yield return w;
            foreach (var w in Delay(host, 0x0A)) yield return w;
            foreach (var w in Flip(d, host)) yield return w;
            Emit(host, FieldEventKind.ClearList);
            Emit(host, FatMikeEventKind.Indicator, v, 1);
            Emit(host, FieldEventKind.RunList);
            foreach (var w in Delay(host, 0x14)) yield return w;
        }
    }

    /// <summary>
    /// 1F63:0390: base price = total stock 3 of all players / 24 (16-bit sum), at most 30. Per round (DS:54CC = 0 … 2) from week 4
    /// on a kind is drawn – random(6) = 0 → 0, random(15) = 0 → 1, random(20) = 0, or fewer than 4 free fields and random(5) = 0
    /// → 2; it stays from round to round – and the price is (random(5) + 1)·base, kind 1 (random(5) + 3)·base, kind 2 999; at
    /// least 10, rounded down to a multiple of the scale unit (remainder of the signed division). DS:54D2 = the cheapest round
    /// (the first of equal prices).
    /// </summary>
    // [ORIGINAL] 1F63:0390
    public static void Prices(Dgroup d, OriginalRandom random)
    {
        short kind = 0;
        d.Set(Base, (short)0);
        ForPlayers(d, p => d.Set(Base, Int16Math.S(d.S16(Base) + d.S16(StockOf(p)))));
        d.Set(Base, Int16Math.Div(d.S16(Base), 0x18));
        if (d.S16(Base) > 0x1E) d.Set(Base, (short)0x1E);
        for (d.Set(Lot, (short)0); d.S16(Lot) <= 2; d.Set(Lot, Int16Math.S(d.S16(Lot) + 1)))
        {
            if (d.S16(DgroupLayout.Week) >= 4)
            {
                if (random.Next(6) == 0) kind = 0;
                if (random.Next(0x0F) == 0) kind = 1;
                if (random.Next(0x14) == 0) kind = 2;
                else if (d.S8(FieldGrid.FreeFields) < 4 && random.Next(5) == 0) kind = 2;
            }
            d.Set(Price, Int16Math.S(Int16Math.S(random.Next(5) + 1) * d.S16(Base)));
            if (kind == 1) d.Set(Price, Int16Math.S(Int16Math.S(random.Next(5) + 3) * d.S16(Base)));
            if (kind == 2) d.Set(Price, (short)999);
            if (d.S16(Price) < 0x0A) d.Set(Price, (short)0x0A);
            d.Set(Price, Int16Math.S(d.S16(Price) - Int16Math.Mod(d.S16(Price), d.S16(Unit))));
            d.Set(At(Lots + Int16Math.S(d.S16(Lot) << 1)), d.S16(Price));
        }
        d.Set(Cheapest, (short)0);
        for (d.Set(Lot, (short)1); d.S16(Lot) <= 2; d.Set(Lot, Int16Math.S(d.S16(Lot) + 1)))
            if (d.S16(At(Lots + Int16Math.S(d.S16(Lot) << 1))) < d.S16(At(Lots + Int16Math.S(d.S16(Cheapest) << 1))))
                d.Set(Cheapest, d.S16(Lot));
    }

    /// <summary>
    /// 1F63:04D8 – one offer at the price DS:54B8, frame counter i = 0 … 31·DS:1020[IQ] while somebody is left to pay (DS:3CE6):
    /// list emptied; in rounds 0 and 1 the indicator 1F63:113E (random(200)); controls, pause; per player who has not paid: a
    /// computer decides (064A) in the frame 31·DS:1020[IQ] − 20·player; with fire (0Ah) and stock 3 ≥ price he pays – the price
    /// is booked off, text DS:103C, sound (0, 25h), his stock (0EC7) and bar (0E8F) redrawn, DS:54BA = 1, DS:3CE6 − 1. Then the
    /// list goes to the screen with the objects and a flip (1185) when it has entries, else 2 retraces.
    /// </summary>
    // [ORIGINAL] 1F63:04D8
    // 1F63:060E – DS:15BE > 0: the core counts the list entries it requested since 0A96:001B (Listing); DS:15BE
    // itself is the Vga command list's
    public static IEnumerable<FieldWait> OfferFrames(Dgroup d, OriginalRandom random, IFatMikeHost host, WaitLoop indicator)
    {
        var h = host as Listing ?? new Listing(host);
        for (short si = 0; Limit(d) >= si && d.U8(Planting.Players) != 0; si++)
        {
            Emit(h, FieldEventKind.ClearList);
            if (d.S16(Lot) <= 1) Indicator(h, indicator);
            h.ReadControls(d);
            foreach (var w in h.PauseCheck(d)) yield return w;
            for (d.Set(DgroupLayout.ActivePlayer, (sbyte)0); d.S8(DgroupLayout.ActivePlayer) < 4;
                 d.Set(DgroupLayout.ActivePlayer, unchecked((sbyte)(d.S8(DgroupLayout.ActivePlayer) + 1))))
            {
                int p = P(d);
                if (d.S16(At(Paid + Int16Math.S(p << 1))) == 1) continue;
                if (d.U8(At(DgroupLayout.IsComputer + p)) == 1 && Int16Math.S(Limit(d) - Int16Math.S(p * 0x14)) == si) Decide(d, random);
                if (d.U8(At(ControlReader.Action + p)) != (byte)ControlAction.Fire) continue;
                if (d.S16(StockOf(p)) < d.S16(Price)) continue;
                d.Set(StockOf(p), Int16Math.S(d.S16(StockOf(p)) - d.S16(Price)));
                Emit(h, FatMikeEventKind.ShadowText, 0x93, Int16Math.S(Row(p) + 0x2D), PaidText, unchecked((short)DgroupLayout.Segment),
                    d.U8(At(Colour + p)), 0, -1, 1);
                Emit(h, FieldEventKind.Sound, 0, 0x25);
                ShowStock(d, h);
                ShowBar(d, h, Int16Math.S(0x78 - Int16Math.Div(d.S16(StockOf(P(d))), d.S16(Unit))));
                d.Set(At(Paid + Int16Math.S(P(d) << 1)), (short)1);
                d.Set(Planting.Players, unchecked((byte)(d.U8(Planting.Players) - 1)));
            }
            if (h.Entries > 0)
                foreach (var w in ShowFrames(d, h)) yield return w;
            else
                foreach (var w in Delay(h, 2)) yield return w;
        }
    }

    /// <summary>31·DS:1020[DS:2E8E] (signed byte, 16-bit product): the last frame of an offer.</summary>
    private static short Limit(Dgroup d) =>
        Int16Math.S(d.S8(At(Patience + d.S16(DgroupLayout.ComputerIq))) * 0x1F);

    /// <summary>1F63:113E through the wait loop's indicator (lane bilanz, <see cref="WaitLoop.Indicator"/>); its event as ours.</summary>
    // [ORIGINAL] 1F63:113E – called, Week.WaitLoop.Indicator
    private static void Indicator(IFatMikeHost host, WaitLoop indicator)
    {
        indicator.Indicator();
        foreach (var e in indicator.DrainEvents())
        {
            if (e.Kind != WeekEventKind.Indicator) throw new InvalidOperationException($"indicator event {e.Kind}");
            Emit(host, FatMikeEventKind.Indicator, (short)e.A, (short)e.B);
        }
    }

    /// <summary>
    /// 1F63:064A – a computer's choice (DS:3308 = 0Ah fire or FFh): the player of the week (week mod 7) pays only in the cheapest
    /// round; otherwise he pays with DS:4BC2 = 1, with stock 3 ≥ 4·price (16-bit shift), with random(5) = 0, in round 0 when
    /// 2·base ≥ price, in round 1 when 4·base ≥ price and always in round 2.
    /// </summary>
    // [ORIGINAL] 1F63:064A
    public static void Decide(Dgroup d, OriginalRandom random)
    {
        int p = P(d);
        short action = -1;
        if (Int16Math.Mod(d.S16(DgroupLayout.Week), 7) == p)
        {
            if (d.S16(Lot) == d.S16(Cheapest)) action = 0x0A;
        }
        else if (d.U8(At(DgroupLayout.FatMikeFlag + p)) == 1) action = 0x0A;
        else if (d.S16(StockOf(p)) >= Int16Math.S(d.S16(Price) << 2)) action = 0x0A;
        else if (random.Next(5) == 0) action = 0x0A;
        else
        {
            switch (d.S16(Lot))
            {
                case 0: if (Int16Math.S(d.S16(Base) << 1) >= d.S16(Price)) action = 0x0A; break;
                case 1: if (Int16Math.S(d.S16(Base) << 2) >= d.S16(Price)) action = 0x0A; break;
                case 2: action = 0x0A; break;
            }
        }
        d.Set(At(ControlReader.Action + p), unchecked((byte)action));
    }

    /// <summary>
    /// 1F63:0711 – the verdicts: who paid gets 0 and DS:4BC2 = 0; who did not gets 2 (fields lost) when DS:4BC2 was already 1 or
    /// nobody paid, else 1 (crops destroyed), DS:4BC2 = 1, and 0 again when the count 0A8A is not positive.
    /// </summary>
    // [ORIGINAL] 1F63:0711
    public static void Verdicts(Dgroup d)
    {
        ForPlayers(d, p =>
        {
            if (d.S16(At(Paid + Int16Math.S(p << 1))) == 1)
            {
                d.Set(At(Verdict + p), (byte)0);
                d.Set(At(DgroupLayout.FatMikeFlag + p), (byte)0);
                return;
            }
            bool again = d.U8(At(DgroupLayout.FatMikeFlag + p)) == 1 || d.U8(NobodyPaid) == 1;
            d.Set(At(Verdict + p), (byte)(again ? 2 : 1));
            d.Set(At(DgroupLayout.FatMikeFlag + p), (byte)1);
            if (Count(d) <= 0) d.Set(At(Verdict + P(d)), (byte)0);
        });
    }

    /// <summary>
    /// 1F63:0A8A – how many fields the punishment hits: with nobody paid (fields − 1)/4; for crops destroyed (verdict 1) the
    /// fields of kinds 0…3 (loop variable DS:45D0) / 4; else fields / 5 (signed divisions, fields DS:4BA8 unsigned).
    /// </summary>
    // [ORIGINAL] 1F63:0A8A
    public static short Count(Dgroup d)
    {
        int p = P(d);
        short di = Int16Math.S(d.U8(At(DgroupLayout.FieldCount + p)) - 1);
        if (d.U8(NobodyPaid) == 1) return Int16Math.Div(di, 4);
        if (d.U8(At(Verdict + p)) == 1)
        {
            short si = 0;
            for (d.Set(DgroupLayout.LoopWare, (short)0); d.S16(DgroupLayout.LoopWare) <= 3;
                 d.Set(DgroupLayout.LoopWare, Int16Math.S(d.S16(DgroupLayout.LoopWare) + 1)))
                si = Int16Math.S(si + d.S8(At(FieldGrid.OwnedKinds + Int16Math.S(Int16Math.S(p << 2) + d.S16(DgroupLayout.LoopWare)))));
            return Int16Math.Div(si, 4);
        }
        return Int16Math.Div(Int16Math.S(di + 1), 5);
    }

    /// <summary>
    /// 1F63:079A – the field part: nothing when every verdict is 0. Else the field picture (DS:08CE), fade-out to 5, objects
    /// off (21AF:000D), palette, weather colours; when nobody paid a notice (DS:3B74) on its own screen and a fade-out; the
    /// board 0609:1BE5 with its border draws, fade-in, font 1, when nobody paid the notice DS:3B94; per punished player 08DA;
    /// font 0 and the wait loop 21FD:0004(0).
    /// </summary>
    // [ORIGINAL] 1F63:079A
    public static IEnumerable<FieldWait> PunishFrames(Dgroup d, OriginalRandom random, IFatMikeHost host)
    {
        var field = new FieldOutput(host);
        short punished = 0;
        ForPlayers(d, p => { if (d.U8(At(Verdict + p)) != 0) punished++; });
        if (punished == 0) yield break;
        Emit(host, FieldEventKind.LoadImage, d.S16(Planting.PictureNumber));
        Emit(host, FieldEventKind.FadeOut, 5);
        foreach (var w in Planting.Fade(d, 4)) yield return w;
        Planting.InitObjects(d, field);
        Emit(host, FieldEventKind.BasePalette, FieldPalette);
        Planting.WeatherColours(d, field);
        var typewriter = new Typewriter(host);
        if (d.U8(NobodyPaid) == 1)
        {
            Emit(host, FieldEventKind.ClearList);
            Emit(host, FieldEventKind.DrawBackground);
            Emit(host, FieldEventKind.DrawFrame, 0x28, 0x4B, 0xF0, 0x32);
            foreach (var w in BothPages(d, host)) yield return w;
            Emit(host, FieldEventKind.FadeIn);
            foreach (var w in Planting.Fade(d, 3)) yield return w;
            foreach (var w in Traders.Core.Harvest.Harvest.TypeLines(d, typewriter, 0x28, 0x63, 0xF0, WarningLines, 2, 5, 5))
                yield return w;
            foreach (var w in Delay(host, 0x64)) yield return w;
            Emit(host, FieldEventKind.FadeOut, 5);
            foreach (var w in Planting.Fade(d, 4)) yield return w;
        }
        foreach (var w in Planting.BoardFrames(d, random, field)) yield return w;
        Emit(host, FieldEventKind.FadeIn);
        foreach (var w in Planting.Fade(d, 3)) yield return w;
        Emit(host, FatMikeEventKind.Font, 1);
        if (d.U8(NobodyPaid) == 1)
            foreach (var w in Traders.Core.Harvest.Harvest.TypeLines(d, typewriter, 0x10, 0x13, 0xE0, PunishLines, 3, 5, 5))
                yield return w;
        for (d.Set(DgroupLayout.ActivePlayer, (sbyte)0); d.S8(DgroupLayout.ActivePlayer) < 4;
             d.Set(DgroupLayout.ActivePlayer, unchecked((sbyte)(d.S8(DgroupLayout.ActivePlayer) + 1))))
        {
            if (d.U8(At(Verdict + P(d))) == 0) continue;
            foreach (var w in PunishPlayerFrames(d, random, host)) yield return w;
        }
        Emit(host, FatMikeEventKind.Font, 0);
        foreach (var w in host.WaitLoop(d, random, 0)) yield return w;
    }

    /// <summary>
    /// 1F63:08DA – the punishment of player DS:37AA: n = 0A8A; nothing when n ≤ 0. His emblem (F0h, 6, rank 3) on both pages;
    /// unless nobody paid, a panel and the typed lines EC… (fields lost) or EF… (crops destroyed) of the text table. Then until
    /// n fields are hit a field is drawn (column random(columns − 1), row random(rows − 1) into DS:2EB0/2F00); his own fields
    /// except the HQ count: fields lost → nobody's and empty (0609:10D2(x, y, −1, −2)), sound (0, 27h); crops destroyed → empty
    /// when not empty yet (0609:10D2(x, y, player, −2)), sound (0, 26h); the field and its neighbours on both pages and 25
    /// retraces.
    /// </summary>
    // [ORIGINAL] 1F63:08DA
    public static IEnumerable<FieldWait> PunishPlayerFrames(Dgroup d, OriginalRandom random, IFatMikeHost host)
    {
        short si = Count(d);
        if (si <= 0) yield break;
        Emit(host, FieldEventKind.ClearList);
        Emit(host, FatMikeEventKind.Emblem, 0xF0, 6, 3, d.S8(DgroupLayout.ActivePlayer));
        foreach (var w in BothPages(d, host)) yield return w;
        if (d.U8(NobodyPaid) == 0)
        {
            Emit(host, FieldEventKind.ClearList);
            Emit(host, FatMikeEventKind.Tile, 0xF0, 0xB0, 0x20, 0x16, 0x10, 6, 0xE0, 0x2C);
            foreach (var w in BothPages(d, host)) yield return w;
            short line = d.U8(At(Verdict + P(d))) == 2 ? (short)0xEC : (short)0xEF;
            foreach (var w in Traders.Core.Harvest.Harvest.TypeLines(d, new Typewriter(host), 0x10, 0x13, 0xE0,
                         Int16Math.S((line << 2) + TextTable), 3, 5, 5))
                yield return w;
        }
        while (si > 0)
        {
            d.Set(DgroupLayout.LoopX, unchecked((byte)random.Next(Int16Math.S(d.S16(FieldGrid.Columns) - 1))));
            d.Set(DgroupLayout.LoopY, unchecked((byte)random.Next(Int16Math.S(d.S16(FieldGrid.Rows) - 1))));
            short x = d.S8(DgroupLayout.LoopX), y = d.S8(DgroupLayout.LoopY);
            int cell = Int16Math.U(Int16Math.S(x * 6) + y);
            if (d.U8(At(FieldGrid.Owner + cell)) != d.U8(DgroupLayout.ActivePlayer)) continue;
            if (d.S8(At(FieldGrid.Kind + cell)) == 4) continue;
            if (d.U8(At(Verdict + P(d))) == 2)
            {
                FieldGrid.Set(d, x, y, -1, -2);
                Emit(host, FieldEventKind.Sound, 0, 0x27);
            }
            else
            {
                if (d.S8(At(FieldGrid.Kind + cell)) == -2) continue;
                FieldGrid.Set(d, x, y, d.S8(DgroupLayout.ActivePlayer), -2);
                Emit(host, FieldEventKind.Sound, 0, 0x26);
            }
            si--;
            Emit(host, FieldEventKind.ClearList);
            Emit(host, FieldEventKind.DrawFieldAndNeighbours, d.S8(DgroupLayout.LoopX), d.S8(DgroupLayout.LoopY));
            foreach (var w in BothPages(d, host)) yield return w;
            foreach (var w in Delay(host, 0x19)) yield return w;
        }
    }

    /// <summary>
    /// 1F63:0C82 – the largest stock 3 (start 0, signed) among the players taking part (<see cref="LeftOut"/>); DS:4890[p] = 1 for
    /// those with exactly that stock and DS:4BC2 = 0, when it is positive (all others 0).
    /// </summary>
    // [ORIGINAL] 1F63:0C82
    public static short MaxStock(Dgroup d)
    {
        short si = 0;
        ForPlayers(d, p =>
        {
            d.Set(At(Bidders + p), (byte)0);
            if (LeftOut(d, p)) return;
            if (d.S16(StockOf(p)) > si) si = d.S16(StockOf(p));
        });
        if (si > 0)
            ForPlayers(d, p =>
            {
                if (d.S16(StockOf(p)) == si && d.U8(At(DgroupLayout.FatMikeFlag + p)) == 0) d.Set(At(Bidders + p), (byte)1);
            });
        return si;
    }

    /// <summary>
    /// 1F63:0D24 – the scene: 32 columns copied inside the atlas, the background (atlas 0, 38h, 140h × C8h), the panels and
    /// scale, five and three copies of page parts, list and page shown (0A96:0C3F).
    /// </summary>
    // [ORIGINAL] 1F63:0D24
    public static IEnumerable<FieldWait> SceneFrames(Dgroup d, IFatMikeHost host)
    {
        Emit(host, FieldEventKind.ClearList);
        for (short si = 0; si <= 0x1F; si++)
            Emit(host, FatMikeEventKind.CopyAtlas, Int16Math.S(0x57 - si), 0x43, 1, 0x6D, Int16Math.S(si + 0x58), 0x43);
        Emit(host, FieldEventKind.DrawCommand, 0, 0x38, 0x140, 0xC8, 0, 0);
        Emit(host, FatMikeEventKind.CopyPage, 0x2C, 0x0B, 0x0C, 0x21, 0x95, 0x0B);
        Emit(host, FatMikeEventKind.Tile, 0x2C, 0x49, 0x0A, 0x16, 0x28, 0x11, 0x78, 0x16);
        Emit(host, FatMikeEventKind.Tile, 0x23, 0x5F, 0x0A, 5, 0x28, 0x27, 0x6D, 5);
        for (short si = 0; si <= 4; si++)
            Emit(host, FatMikeEventKind.CopyPage, 0x22, 0x0B, 0x16, 6, Int16Math.S(Int16Math.S(si * 0x16) + 0x38), 0x0B);
        for (short si = 0; si <= 2; si++)
            Emit(host, FatMikeEventKind.CopyPage, 0x22, 0x0B, 0x7F, 0x21, 0x22, Int16Math.S(Int16Math.S(si * 0x21) + 0x2C));
        Emit(host, FieldEventKind.RunList);
        foreach (var w in ShowPage(d, host)) yield return w;
    }

    /// <summary>1F63:0E5C(player, i) – pixel i of the player's bar (atlas player + 28h, 49h, 1 × 14h at (i + 27h, 21h·p + 11h)).</summary>
    // [ORIGINAL] 1F63:0E5C
    public static void Bar(IFatMikeHost host, short player, short i) =>
        Emit(host, FieldEventKind.DrawCommand, Int16Math.S(player + 0x28), 0x49, 1, 0x14, Int16Math.S(i + 0x27),
            Int16Math.S(Int16Math.S(player * 0x21) + 0x11));

    /// <summary>1F63:0E8F(w) – the right part of the bar of DS:37AA cleared: w columns from A0h − w (tile 2Ch, 4Ch, 0Ah × 10h).</summary>
    // [ORIGINAL] 1F63:0E8F
    public static void ShowBar(Dgroup d, IFatMikeHost host, short w) =>
        Emit(host, FatMikeEventKind.Tile, 0x2C, 0x4C, 0x0A, 0x10, Int16Math.S(0xA0 - w), Int16Math.S(Row(P(d)) + 0x14), w, 0x10);

    /// <summary>1F63:0EC7 – stock 3 of DS:37AA: the box restored (page copy) and the number, three digits, in the player's colour.</summary>
    // [ORIGINAL] 1F63:0EC7
    public static void ShowStock(Dgroup d, IFatMikeHost host)
    {
        int p = P(d);
        short y = Int16Math.S(Row(p) + 0x28);
        Emit(host, FatMikeEventKind.CopyPage, 0x46, y, 0x14, 7, 0x28, y);
        short v = d.S16(StockOf(p));
        Emit(host, FatMikeEventKind.ShadowNumber, 0x29, Int16Math.S(Row(p) + 0x2D), v, (short)(v < 0 ? -1 : 0), 3,
            d.U8(At(Colour + p)), 0, -1, 1);
    }

    /// <summary>1F63:0F46 – the text DS:3BA0 (far) instead of the stock of DS:37AA, in the player's colour.</summary>
    // [ORIGINAL] 1F63:0F46
    public static void ShowExcluded(Dgroup d, IFatMikeHost host)
    {
        int p = P(d);
        Emit(host, FatMikeEventKind.ShadowText, 0x29, Int16Math.S(Row(p) + 0x2C), d.S16(ExcludedText), d.S16(ExcludedText + 2),
            d.U8(At(Colour + p)), 0, -1, 1);
    }

    /// <summary>
    /// 1F63:0F84(n) – the indicator: n = 0 both in state 1, n = 1 both in state 2, n = 2 both in state 2 and the lamps 0FDF(1);
    /// other n nothing; then the list to the screen (1185).
    /// </summary>
    // [ORIGINAL] 1F63:0F84
    public static IEnumerable<FieldWait> IndicatorFrames(Dgroup d, IFatMikeHost host, short n)
    {
        Emit(host, FieldEventKind.ClearList);
        switch (n)
        {
            case 0: Emit(host, FatMikeEventKind.Indicator, 3, 1); break;
            case 1: Emit(host, FatMikeEventKind.Indicator, 3, 2); break;
            case 2:
                Emit(host, FatMikeEventKind.Indicator, 3, 2);
                Lights(d, host, 1);
                break;
        }
        foreach (var w in ShowFrames(d, host)) yield return w;
    }

    /// <summary>1F63:0FDF(i) – two pictures from the atlas positions DS:1024/1026[i] and DS:1028[2i]/102C[i] at (C6h, 46h) and (104h, 46h).</summary>
    // [ORIGINAL] 1F63:0FDF
    public static void Lights(Dgroup d, IFatMikeHost host, byte i)
    {
        Emit(host, FieldEventKind.DrawCommand, d.U8(LampX + i), d.U8(LampY + i), 0x1A, 0x26, 0xC6, 0x46);
        Emit(host, FieldEventKind.DrawCommand, d.S16(LampX2 + 2 * i), d.U8(LampY2 + i), 0x1A, 0x26, 0x104, 0x46);
    }

    /// <summary>1F63:1185 – list, objects drawn, flip, objects restored, list.</summary>
    // [ORIGINAL] 1F63:1185
    public static IEnumerable<FieldWait> ShowFrames(Dgroup d, IFatMikeHost host)
    {
        Emit(host, FieldEventKind.RunList);
        Emit(host, FieldEventKind.DrawObjects);
        foreach (var w in Flip(d, host)) yield return w;
        Emit(host, FieldEventKind.RestoreObjects);
        Emit(host, FieldEventKind.RunList);
    }

    /// <summary>
    /// 1F63:1247 – the seven objects of the scene: layer (+2) 82h for object 0, 1Eh for 1…6, save buffers (21AF:022B) and the
    /// picture of frame 0 (21AF:00CA).
    /// </summary>
    // [ORIGINAL] 1F63:1247
    public static void Objects(Dgroup d, IFatMikeHost host)
    {
        short[][] buffers =
        [
            [0, 0, 0x46, 0x120, 0x46], [1, 0xE0, 0xC4, 0xE0, 0xE2], [2, 0x100, 0xC4, 0x100, 0xE2], [3, 0xC0, 0, 0xC0, 0x1E],
            [4, 0xE0, 0, 0xE0, 0x1E], [5, 0x100, 0, 0x100, 0x1E], [6, 0x120, 0, 0x120, 0x1E],
        ];
        short[][] images =
        [
            [0, 0, 0x90, 0x43, 0x130, 0], [1, 0, 0, 0, 0xD0, 0], [2, 0, 0x10, 0, 0xE0, 0], [3, 0, 0x20, 0, 0xF0, 0],
            [4, 0, 0x30, 0, 0x100, 0], [5, 0, 0x40, 0, 0x110, 0], [6, 0, 0x50, 0, 0x120, 0],
        ];
        for (int i = 0; i < 7; i++)
        {
            d.Set(Planting.ObjectLayer + i * Planting.ObjectSize, (short)(i == 0 ? 0x82 : 0x1E));
            Emit(host, FieldEventKind.ObjectBuffers, buffers[i]);
            Emit(host, FieldEventKind.ObjectImage, images[i]);
        }
    }

    /// <summary>
    /// 1F63:13FE – the row at x 94h (DS:54CA, 143E), objects 0…6 shown in frame 0 (21AF:0412), drawn, flip, drawn.
    /// </summary>
    // [ORIGINAL] 1F63:13FE
    public static IEnumerable<FieldWait> ShowRowFrames(Dgroup d, IFatMikeHost host)
    {
        var field = new FieldOutput(host);
        d.Set(Slide, (short)0x94);
        PlaceRow(d);
        for (int si = 0; si < 7; si++) Planting.ShowCursor(d, field, si);
        Emit(host, FieldEventKind.DrawObjects);
        foreach (var w in Flip(d, host)) yield return w;
        Emit(host, FieldEventKind.DrawObjects);
    }

    /// <summary>1F63:143E – objects 0…6 at (DS:54CA + DS:102E[i], DS:1035[i]) (bytes unsigned).</summary>
    // [ORIGINAL] 1F63:143E
    public static void PlaceRow(Dgroup d)
    {
        for (int si = 0; si < 7; si++)
        {
            d.Set(Planting.ObjectX + si * Planting.ObjectSize, Int16Math.S(d.S16(Slide) + d.U8(RowX + si)));
            d.Set(Planting.ObjectY + si * Planting.ObjectSize, (short)d.U8(RowY + si));
        }
    }

    /// <summary>
    /// 1F63:147B(price) – the row moves to x = price/unit + 20 (at most 100h): per frame by a step of distance/8, at least 1 and
    /// at most 4 pixels (signed), placed (143E), drawn, flip, restored.
    /// </summary>
    // [ORIGINAL] 1F63:147B
    public static IEnumerable<FieldWait> SlideFrames(Dgroup d, IFatMikeHost host, short price)
    {
        short di = Int16Math.S(Int16Math.Div(price, d.S16(Unit)) + 0x14);
        if (di > 0x100) di = 0x100;
        while (true)
        {
            short delta = Int16Math.S(di - d.S16(Slide));
            if (delta == 0) yield break;
            short si = Int16Math.Div(delta, 8);
            if (si == 0) si = delta < 0 ? (short)-1 : (short)1;
            else if (si > 4) si = 4;
            else if (si < -4) si = -4;
            d.Set(Slide, Int16Math.S(d.S16(Slide) + si));
            PlaceRow(d);
            Emit(host, FieldEventKind.DrawObjects);
            foreach (var w in Flip(d, host)) yield return w;
            Emit(host, FieldEventKind.RestoreObjects);
        }
    }

    /// <summary>21AF:0472 – 21AF:045E for the objects 0…7, core part: their visible bytes (+0) cleared.</summary>
    // [ORIGINAL] 21AF:0472 – core part only (object records +0)
    public static void HideObjects(Dgroup d, IFatMikeHost host)
    {
        Emit(host, FatMikeEventKind.HideObjects);
        for (int i = 0; i < 8; i++) d.Set(Planting.Objects + i * Planting.ObjectSize, (byte)0);
    }

    // 21FD:051F – the flip with its frames (Planting.Flip).
    private static IEnumerable<FieldWait> Flip(Dgroup d, IFatMikeHost host)
    {
        Emit(host, FieldEventKind.FlipPages);
        foreach (var w in Planting.Flip(d)) yield return w;
    }

    // [ORIGINAL] 21FD:060D – 0A96:06A8, 21FD:051F, 0A96:06A8 (one event, the flip's frames)
    private static IEnumerable<FieldWait> BothPages(Dgroup d, IFatMikeHost host)
    {
        Emit(host, FieldEventKind.ListOnBothPages);
        foreach (var w in Planting.Flip(d)) yield return w;
    }

    // 21FD:0185(n) – the retraces (Planting.Retraces).
    private static IEnumerable<FieldWait> Delay(IFatMikeHost host, short n)
    {
        Emit(host, FieldEventKind.Delay, n, 0);
        return Planting.Retraces(n, 0);
    }

    /// <summary>0A96:0C3F([082E]) – the hidden page is copied; in the modes 2 and 63h it flips the pages first (21FD:051F).</summary>
    // [ORIGINAL] 0A96:0C3F – 21FD:051F when DS:090E = 2 or 63h, then the page copy (output)
    private static IEnumerable<FieldWait> ShowPage(Dgroup d, IFatMikeHost host)
    {
        Emit(host, FieldEventKind.ShowPage);
        if (d.S16(Planting.CardMode) is 2 or 0x63)
            foreach (var w in Planting.Flip(d)) yield return w;
    }

    /// <summary>
    /// The host with a count of the command list's entries requested since the last 0A96:001B: every drawing request except
    /// list execution, pages, palette, objects and sound adds at least one entry in the original (0A96 command types 0–12).
    /// </summary>
    internal sealed class Listing(IFatMikeHost host) : IFatMikeHost
    {
        public int Entries { get; private set; }

        public void Emit(FieldEvent e)
        {
            switch (e.Kind)
            {
                case FieldEventKind.ClearList: Entries = 0; break;
                case FieldEventKind.DrawCommand or FieldEventKind.DrawField or FieldEventKind.DrawFieldAndNeighbours
                    or FieldEventKind.DrawBackground or FieldEventKind.DrawFrame or FieldEventKind.DrawBorder:
                    Entries++;
                    break;
            }
            host.Emit(e);
        }

        public void Emit(FatMikeEvent e)
        {
            if (e.Kind is not (FatMikeEventKind.Font or FatMikeEventKind.HideObjects or FatMikeEventKind.TypeLines)) Entries++;
            host.Emit(e);
        }

        public void ReadControls(Dgroup d) => host.ReadControls(d);
        public IEnumerable<FieldWait> PauseCheck(Dgroup d) => host.PauseCheck(d);
        public IEnumerable<FieldWait> WaitLoop(Dgroup d, OriginalRandom random, short mode) => host.WaitLoop(d, random, mode);
        public byte[] FarString(ushort offset, ushort segment) => host.FarString(offset, segment);
    }

    /// <summary>The field layer's outputs of the shared routines (objects, palette, board) go to the host.</summary>
    private sealed class FieldOutput(IFatMikeHost host) : IFieldOutput
    {
        public void ComputerTurn(Dgroup d, short x, short y) =>
            throw new InvalidOperationException("Fat Mike has no computer turn of the field.");

        public void Emit(FieldEvent e) => host.Emit(e);
    }

    /// <summary>The typewriter 0A69:0288 of lane ernte on Fat Mike's host (<see cref="FatMikeEventKind.TypeLines"/>/<c>TypeCharacter</c>).</summary>
    private sealed class Typewriter(IFatMikeHost host) : IHarvestHost
    {
        public void Emit(FieldEvent e) => host.Emit(e);

        public void Emit(HarvestEvent e) => host.Emit(e.Kind switch
        {
            HarvestEventKind.TypeLines => new FatMikeEvent(FatMikeEventKind.TypeLines, e.Args),
            HarvestEventKind.TypeCharacter => new FatMikeEvent(FatMikeEventKind.TypeCharacter, e.Args),
            _ => throw new InvalidOperationException($"typewriter event {e.Kind} is not part of Fat Mike"),
        });

        public void ReadControls(Dgroup d) => throw new InvalidOperationException("The typewriter reads no controls.");

        public IEnumerable<FieldWait> PauseCheck(Dgroup d) => host.PauseCheck(d);

        public byte[] FarString(ushort offset, ushort segment) => host.FarString(offset, segment);
    }
}
