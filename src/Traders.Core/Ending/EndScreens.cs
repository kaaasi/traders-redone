using System.Globalization;
using Traders.Core.Field;
using Traders.Core.Pixels;
using Traders.Core.Rules;
using Traders.Core.State;
using E = Traders.Core.Ending.EndingEventKind;
using F = Traders.Core.Field.FieldEventKind;
using L = Traders.Core.State.DgroupLayout;

namespace Traders.Core.Ending;

/// <summary>
/// The rest of the end check 20B3:0005 from 20B3:0053 on (finding docs/routines/20B3_008E_ende.md): the closing picture of
/// the field 008E (the players' cursors march in, the winners leave the field), the end tune, the evening 0525 (every player
/// in the order of his rank, his place, name and wealth revealed in 5 × 5 cells), the ranking 0863 (the winner's walk, bow
/// and drink), DS:4B9A + 1, 0825:07C4, the report 0825:000A(0) and the any-key wait 27C6:00D2. Works on the data segment like
/// the original; drawing and sound are emitted as events in the original's order, every yielded <see cref="FieldWait"/> is
/// one frame. The evening reads pixels of its draw page; the core follows them on its rule page (<see cref="EndingPage"/>).
/// </summary>
public static class EndScreens
{
    // DS offsets (docs/analyse/DATENLAYOUT.md, section of lane ende R9).
    public const int FieldPictureNumber = 0x08CE;   // word, picture of the closing picture (V-ANBAU)
    public const int EveningPictureNumber = 0x08E0; // word, picture of the evening (V-ABEND)
    public const int RankPictureNumber = 0x08DC;    // word, picture of the ranking (V-ENDE)
    public const int FieldPalette = 0x00A8, EveningPalette = 0x1060, RankPalette = 0x1040;
    public const int Walk = 0x1080;      // word, position on the winner's walk (initialised data, kept between games)
    public const int BlinkTimer = 0x1082; // word, frames to the next step of the blinking 0986
    public const int BlinkStep = 0x1084;  // byte, 0…3, picture of the blinking
    public const int Bow1 = 0x1085, Bow2 = 0x1089; // words, frames of the two bows
    public const int Drink = 0x1087;      // word, frames of the drink (0…100)
    public const int ObjectFrames = 0x108B; // word[2], y of the object frames in the atlas of 0D0D(arg)
    public const int RankText = 0x108F;   // string in DS, the word before the place in the evening
    public const int CosTable = 0x0D17;   // word[91], 0B7D: cos(0…90°) scaled
    public const int RankState = 0x54D4;  // word, state of the ranking loop 0863 (0…7)
    public const int Winners = 0x54D6;    // byte, players with rank 0 (008E), the drinks left (0ADC)
    public const int MakeCount = 0x115D;  // byte, makes counted by the keyboard handler 27C6:0043

    /// <summary>Calls of the evening's reveal 069A per run and the size of its grid (random(23h), random(0Ch)).</summary>
    public const int RevealCells = 0x1D4, GridWidth = 0x23, GridHeight = 0x0C;

    private static int At(int offset) => Int16Math.U(offset);
    private static int P(Dgroup d) => d.S8(L.ActivePlayer);
    private static int Obj(int p) => Int16Math.U(Int16Math.S(p * Planting.ObjectSize));
    private static int Word(int table, int p) => At(table + Int16Math.S(p << 1));

    private static void Emit(IEndingHost h, F k, params short[] args) => h.Emit(new FieldEvent(k, args));
    private static void Emit(IEndingHost h, E k, params short[] args) => h.Emit(new EndingEvent(k, args));

    private static IEnumerable<FieldWait> ForPlayers(Dgroup d, Func<int, IEnumerable<FieldWait>> body)
    {
        for (d.Set(L.ActivePlayer, (sbyte)0); d.S8(L.ActivePlayer) < 4;
             d.Set(L.ActivePlayer, unchecked((sbyte)(d.S8(L.ActivePlayer) + 1))))
            foreach (var w in body(P(d))) yield return w;
    }

    /// <summary>
    /// 20B3:0053–0088 frame by frame: 008E, 218B:0094(2, 3), 0525, 0863, <c>inc word [4B9A]</c>, 0825:07C4, 0825:000A(0) and
    /// 27C6:00D2. <paramref name="images"/> gives the pixels of picture n (186C:0002(n)) for the evening's rule page.
    /// </summary>
    // [ORIGINAL] 20B3:0005 – rest 20B3:0053–0088
    public static IEnumerable<FieldWait> Run(Dgroup d, OriginalRandom random, IEndingHost host, EndingPage page,
        Func<int, IndexedImage>? images = null)
    {
        foreach (var w in Picture(d, random, host, page)) yield return w;
        Emit(host, E.Music, 2, 3);
        foreach (var w in Evening(d, random, host, page, images)) yield return w;
        foreach (var w in Ranking(d, host, page)) yield return w;
        d.Set(L.Week, Int16Math.S(d.S16(L.Week) + 1)); // 20B3:0071
        WealthHistoryRecord.Apply(d); // 0825:07C4
        foreach (var w in host.Report(d, random)) yield return w; // 0825:000A(0)
        foreach (var w in WaitKey(d, host)) yield return w;
    }

    /// <summary>
    /// 0825:000A(0) for a game host: the report of lane bilanz (<see cref="Week.Report"/> with argument 0 – no opening, no
    /// wait loop) frame by frame; after every frame its events go to <paramref name="events"/>.
    /// </summary>
    // 0825:000A – Week.Report (lane bilanz), called from 20B3:007D with argument 0
    public static IEnumerable<FieldWait> Report(Dgroup d, OriginalRandom random, Action<IReadOnlyList<Week.WeekEvent>> events)
    {
        var report = new Week.Report(d, random, Input.KeyboardOnlyDevices.Instance, 0);
        foreach (var w in report.Run())
        {
            events(report.DrainEvents());
            yield return w == Menus.MenuWait.Poll ? FieldWait.Poll : FieldWait.Retrace;
        }
        events(report.DrainEvents());
    }

    /// <summary>27C6:00D2 – DS:115D = 0, then a poll per frame until the keyboard handler has counted a make.</summary>
    // [ORIGINAL] 27C6:00D2
    public static IEnumerable<FieldWait> WaitKey(Dgroup d, IEndingHost host)
    {
        d.Set(MakeCount, (byte)0);
        Emit(host, E.WaitKey);
        while (d.U8(MakeCount) == 0) yield return FieldWait.Poll;
    }

    // ------------------------------------------------------------------ 008E closing picture

    /// <summary>
    /// 20B3:008E – the closing picture: V-ANBAU, the cursor objects 0609:1F2D, fade-out, palette and weather colours, the scene
    /// 015B, the marchers 0267, fade-in, the march 047B, 150 retraces; every player with rank 0 leaves the field (0304, 034F,
    /// 03BF, 03E5, 20 retraces, 0432) and is counted in DS:54D6; 50 retraces, fade-out, all cursors hidden (0609:21BA).
    /// </summary>
    // [ORIGINAL] 20B3:008E
    public static IEnumerable<FieldWait> Picture(Dgroup d, OriginalRandom random, IEndingHost host, EndingPage page)
    {
        Load(d, host, page, null, FieldPictureNumber);
        Planting.CursorObjects(d, host); // 0609:1F2D
        foreach (var w in FadeOut(d, host)) yield return w;
        Emit(host, F.BasePalette, FieldPalette);
        Planting.WeatherColours(d, host); // 0609:11E7
        foreach (var w in Scenery(d, random, host, page)) yield return w;
        foreach (var w in Marchers(d, host, page)) yield return w;
        foreach (var w in FadeIn(d, host)) yield return w;
        foreach (var w in March(d, host, page)) yield return w;
        foreach (var w in Delay(host, 0x96)) yield return w;
        d.Set(Winners, (byte)0);
        foreach (var w in ForPlayers(d, p => Winner(d, host, page, p))) yield return w;
        foreach (var w in Delay(host, 0x32)) yield return w;
        foreach (var w in FadeOut(d, host)) yield return w;
        foreach (var _ in ForPlayers(d, p =>
                 {
                     HideObject(d, host, (short)p); // 0609:21BA → 21AF:045E(37AA)
                     return [];
                 })) { }
    }

    private static IEnumerable<FieldWait> Winner(Dgroup d, IEndingHost host, EndingPage page, int p)
    {
        if (d.U8(At(L.Rank + p)) != 0) yield break;
        foreach (var w in Leave(d, host, page, p)) yield return w;
        foreach (var w in Return(d, host, page, p)) yield return w;
        Walk3(d, p);
        foreach (var w in MoveOut(d, host, page, p)) yield return w;
        foreach (var w in Stripes(d, host, page)) yield return w;
        foreach (var w in Delay(host, 0x14)) yield return w;
        foreach (var w in Vanish(d, host, page, p)) yield return w;
        d.Set(Winners, unchecked((byte)(d.U8(Winners) + 1)));
    }

    /// <summary>
    /// 20B3:015B – the scene of the closing picture: background tiles, frame (0, 0, 140h, 36h), the field border
    /// 0609:13C5(30h, 64h, E0h, 42h) with its random draws, three tiled areas, a masked blit and a blit; list executed, page copy.
    /// </summary>
    // [ORIGINAL] 20B3:015B
    public static IEnumerable<FieldWait> Scenery(Dgroup d, OriginalRandom random, IEndingHost host, EndingPage page)
    {
        Clear(host, page);
        Emit(host, F.DrawBackground);
        Emit(host, F.DrawFrame, 0, 0, 0x140, 0x36);
        Planting.Border(0x30, 0x64, 0xE0, 0x42, random, host);
        Emit(host, E.Tile, 0xA0, 0x42, 0x20, 0x16, 0x30, 0x64, 0xE0, 0x42);
        Emit(host, E.Tile, 0x120, 0xD0, 0x20, 0x16, 0x70, 0x64, 0xA1, 0x16);
        Emit(host, E.Tile, 0x120, 0xE6, 0x20, 0x16, 0x70, 0x90, 0xA1, 0x16);
        Emit(host, E.MaskedBlit, 0xD5, 0xE3, 0xD0, 0x1E, 0x3B, 0x1C, 0x4A, 0x54);
        Blit(host, page, 0x90, 0xEE, 0x3B, 0x12, 0x2E, 0x7B);
        Execute(host, page);
        // [ORIGINAL] 0A96:0C3F – 21FD:051F first when DS:090E = 2 or 63h, then the page copy (output)
        Emit(host, F.ShowPage);
        if (d.S16(Planting.CardMode) is not (2 or 0x63)) yield break;
        page.Page.Flip();
        foreach (var w in Planting.Flip(d)) yield return w;
    }

    /// <summary>
    /// 20B3:0267 – the marchers: list emptied; per player his emblem (x = 40h·p + 20h, y = 6, rank 1), the cursor object at
    /// (FAh, 6·p + 78h) moving left (DS:4BCA = 1) for 32h frames (DS:3022), start frame DS:4BD2 = 0, shown (0609:1FCC); then
    /// 21FD:060D.
    /// </summary>
    // [ORIGINAL] 20B3:0267
    public static IEnumerable<FieldWait> Marchers(Dgroup d, IEndingHost host, EndingPage page)
    {
        Clear(host, page);
        foreach (var _ in ForPlayers(d, p =>
                 {
                     Emblem(d, host, p, 1);
                     d.Set(Word(FieldCursor.MoveFrame, p), (short)0);
                     d.Set(At(Planting.ObjectX + Obj(p)), (short)0xFA);
                     d.Set(At(Planting.ObjectY + Obj(p)), Int16Math.S(p * 6 + 0x78));
                     d.Set(Word(FieldCursor.Direction, p), (short)1);
                     d.Set(Word(FieldCursor.MoveTimer, p), (short)0x32);
                     Planting.ShowCursor(d, host); // 0609:1FCC
                     return [];
                 })) { }
        return BothPages(d, host, page);
    }

    /// <summary>
    /// 20B3:047B – the march: per frame flip, restore, 3 retraces, every player with a running move (DS:3022 &gt; 0) one step
    /// (0609:1FE0), objects drawn; until no move runs; then 21FD:0665.
    /// </summary>
    // [ORIGINAL] 20B3:047B
    public static IEnumerable<FieldWait> March(Dgroup d, IEndingHost host, EndingPage page)
    {
        int running;
        do
        {
            foreach (var w in Flip(d, host, page)) yield return w;
            Emit(host, F.RestoreObjects);
            foreach (var w in Delay(host, 3)) yield return w;
            running = 4;
            foreach (var _ in ForPlayers(d, p =>
                     {
                         if (d.S16(Word(FieldCursor.MoveTimer, p)) > 0) Planting.Move(d, host);
                         else running--;
                         return [];
                     })) { }
            Emit(host, F.DrawObjects);
        }
        while (running != 0);
        foreach (var w in LastFrame(d, host, page)) yield return w;
    }

    /// <summary>
    /// 20B3:04DE – the move of player DS:37AA to its end: per frame flip, restore, a step (0609:1FE0), objects, 3 retraces;
    /// then 21FD:0665.
    /// </summary>
    // [ORIGINAL] 20B3:04DE
    public static IEnumerable<FieldWait> MoveOut(Dgroup d, IEndingHost host, EndingPage page, int p)
    {
        while (d.S16(Word(FieldCursor.MoveTimer, p)) > 0)
        {
            foreach (var w in Flip(d, host, page)) yield return w;
            Emit(host, F.RestoreObjects);
            Planting.Move(d, host);
            Emit(host, F.DrawObjects);
            foreach (var w in Delay(host, 3)) yield return w;
        }
        foreach (var w in LastFrame(d, host, page)) yield return w;
    }

    /// <summary>
    /// 20B3:0304 – a winner steps out: 10 frames left (DS:3022), the move 04DE; 21FD:0620, his emblem with rank 0, 21FD:0634,
    /// 100 retraces.
    /// </summary>
    // [ORIGINAL] 20B3:0304
    public static IEnumerable<FieldWait> Leave(Dgroup d, IEndingHost host, EndingPage page, int p)
    {
        d.Set(Word(FieldCursor.MoveTimer, p), (short)0x0A);
        foreach (var w in MoveOut(d, host, page, p)) yield return w;
        RestoreAndClear(host, page);
        Emblem(d, host, p, 0);
        foreach (var w in ListAndObjectsOnBothPages(d, host, page)) yield return w;
        foreach (var w in Delay(host, 0x64)) yield return w;
    }

    /// <summary>
    /// 20B3:034F – to row 78h: si = 78h − y; si ≥ 0: si/2 frames down (direction 3), else −si/2 frames up (2); the move 04DE.
    /// </summary>
    // [ORIGINAL] 20B3:034F
    public static IEnumerable<FieldWait> Return(Dgroup d, IEndingHost host, EndingPage page, int p)
    {
        short si = Int16Math.S(0x78 - d.S16(At(Planting.ObjectY + Obj(p))));
        if (si >= 0)
        {
            d.Set(Word(FieldCursor.MoveTimer, p), Int16Math.Div(si, 2));
            d.Set(Word(FieldCursor.Direction, p), (short)3);
        }
        else
        {
            d.Set(Word(FieldCursor.MoveTimer, p), Int16Math.Div(Int16Math.S(-si), 2));
            d.Set(Word(FieldCursor.Direction, p), (short)2);
        }
        return MoveOut(d, host, page, p);
    }

    /// <summary>20B3:03BF (without its move 04DE): 15h frames to the left.</summary>
    // [ORIGINAL] 20B3:03BF – DS:3022 = 15h, DS:4BCA = 1, then 20B3:04DE
    private static void Walk3(Dgroup d, int p)
    {
        d.Set(Word(FieldCursor.MoveTimer, p), (short)0x15);
        d.Set(Word(FieldCursor.Direction, p), (short)1);
    }

    /// <summary>20B3:03E5 – 20 stripes: per stripe 21FD:0620, blit (CDh, E3h + i, 8, 1) to (63h, 70h + i), 21FD:0634, 1 retrace.</summary>
    // [ORIGINAL] 20B3:03E5
    public static IEnumerable<FieldWait> Stripes(Dgroup d, IEndingHost host, EndingPage page)
    {
        for (short si = 0; si <= 0x13; si++)
        {
            RestoreAndClear(host, page);
            Blit(host, page, 0xCD, Int16Math.S(si + 0xE3), 8, 1, 0x63, Int16Math.S(si + 0x70));
            foreach (var w in ListAndObjectsOnBothPages(d, host, page)) yield return w;
            foreach (var w in Delay(host, 1)) yield return w;
        }
    }

    /// <summary>20B3:0432 – the winner's cursor hidden (21AF:045E), restore, objects, 51 flips, restore, objects.</summary>
    // [ORIGINAL] 20B3:0432
    public static IEnumerable<FieldWait> Vanish(Dgroup d, IEndingHost host, EndingPage page, int p)
    {
        HideObject(d, host, (short)p);
        Emit(host, F.RestoreObjects);
        Emit(host, F.DrawObjects);
        for (int i = 0; i < 0x33; i++)
            foreach (var w in Flip(d, host, page)) yield return w;
        Emit(host, F.RestoreObjects);
        Emit(host, F.DrawObjects);
    }

    // ------------------------------------------------------------------ 0525 evening

    /// <summary>
    /// 20B3:0525 – the evening: V-ABEND, object 0 with the frames of row 0 (0D0D(0)), the picture (0, 0, 140h, C8h) and a
    /// black rectangle (0, 8Fh, 5, 14h) on both pages, palette DS:1060, fade-in, 100 retraces, the places 05B4, the orbit 07BA,
    /// 50 retraces.
    /// </summary>
    // [ORIGINAL] 20B3:0525
    public static IEnumerable<FieldWait> Evening(Dgroup d, OriginalRandom random, IEndingHost host, EndingPage page,
        Func<int, IndexedImage>? images = null)
    {
        Load(d, host, page, images, EveningPictureNumber);
        ObjectFrameSet(d, host, 0);
        Clear(host, page);
        Blit(host, page, 0, 0, 0x140, 0xC8, 0, 0);
        FillRect(host, page, 0, 0x8F, 5, 0x14, 0);
        foreach (var w in BothPages(d, host, page)) yield return w;
        Emit(host, F.BasePalette, EveningPalette);
        foreach (var w in FadeIn(d, host)) yield return w;
        foreach (var w in Delay(host, 0x64)) yield return w;
        foreach (var w in Places(d, random, host, page)) yield return w;
        foreach (var w in Orbit(d, host, page)) yield return w;
        foreach (var w in Delay(host, 0x32)) yield return w;
    }

    /// <summary>
    /// 20B3:05B4 – for the places 0…3 every player with that rank (DS:3D42) in player order: the grid (0, 6, 24h, 0Dh) in colour
    /// 9, the text DS:108F at (0, 0Ah), the place + 1 at (1Eh, 0Ah), the wealth DS:2F06 as "$" and 5 digits at (0, 12h), all in
    /// colour 1, executed; the reveal 069A; 200 retraces. At the end one more reveal 069A (DS:37AA = 4).
    /// </summary>
    // [ORIGINAL] 20B3:05B4
    public static IEnumerable<FieldWait> Places(Dgroup d, OriginalRandom random, IEndingHost host, EndingPage page)
    {
        for (short si = 0; si <= 3; si++)
        {
            short place = si;
            foreach (var w in ForPlayers(d, p => Place(d, random, host, page, p, place))) yield return w;
        }
        Reveal(d, random, host, page);
    }

    private static IEnumerable<FieldWait> Place(Dgroup d, OriginalRandom random, IEndingHost host, EndingPage page, int p,
        short place)
    {
        if (d.U8(At(L.Rank + p)) != place) yield break;
        Clear(host, page);
        FillRect(host, page, 0, 6, 0x24, 0x0D, 9);
        Emit(host, E.Text, 0, 0x0A, RankText, unchecked((short)L.Segment), 1, -1);
        page.SelectFont(d);
        page.Text(0, 0x0A, d.Bytes.AsSpan(RankText, Strlen(d, RankText)).ToArray(), 1);
        short n = Int16Math.S(place + 1);
        Emit(host, E.Number, 0x1E, 0x0A, n, (short)(n < 0 ? -1 : 0), 1, 1, -1);
        page.Text(0x1E, 0x0A, NumberText(d, (uint)(int)n, 1, false), 1);
        uint wealth = d.U32(At(L.CurrentWealth + Int16Math.S(p << 2)));
        Emit(host, E.Money, 0, 0x12, unchecked((short)wealth), unchecked((short)(wealth >> 16)), 5, 1, -1);
        page.Text(0, 0x12, NumberText(d, wealth, 5, true), 1);
        Execute(host, page);
        Reveal(d, random, host, page);
        foreach (var w in Delay(host, 0xC8)) yield return w;
    }

    /// <summary>
    /// 20B3:069A – the reveal in 1D4h steps: per step a cell (x = random(23h) ∈ 0…35, y = random(0Ch) ∈ 0…12, 468 cells) whose pixel (x, y + 6) on the
    /// draw page (23CD:06DE) is not the target (2 on card 1 (DS:090E), else 6), drawn anew until it is not; a pixel 1 copies
    /// the black block (0, 5·DS:37AA + 8Fh), any other the picture's own cell (5x + 88h, 5y + 14h), 5 × 5 to (5x + 88h,
    /// 5y + 14h) on the page shown (0B67:01D4/0206); then the pixel (x, y + 6) in colour 2 on the draw page. At the end the
    /// grid in colour 9 again. No frame: the steps run within one frame. The number of draws depends on the pixels read.
    /// </summary>
    // [ORIGINAL] 20B3:069A
    public static void Reveal(Dgroup d, OriginalRandom random, IEndingHost host, EndingPage page)
    {
        short target = (short)(d.S16(Planting.CardMode) == 1 ? 2 : 6);
        for (short count = 0; count < RevealCells; count++)
        {
            short x, y, v;
            do
            {
                x = random.Next(GridWidth);
                y = random.Next(GridHeight);
                v = host.ReadPixel(page.Page, x, Int16Math.S(y + 6));
                Emit(host, E.ReadPixel, x, Int16Math.S(y + 6), v);
            }
            while (v == target);
            short sx, sy;
            if (v == 1)
            {
                sx = 0;
                sy = Int16Math.S(d.S8(L.ActivePlayer) * 5 + 0x8F);
            }
            else
            {
                sx = Int16Math.S(x * 5 + 0x88);
                sy = Int16Math.S(y * 5 + 0x14);
            }
            Emit(host, E.DrawShownPage);
            page.DrawOnShown();
            Clear(host, page);
            Blit(host, page, sx, sy, 5, 5, Int16Math.S(x * 5 + 0x88), Int16Math.S(y * 5 + 0x14));
            Execute(host, page);
            Emit(host, E.DrawHiddenPage);
            page.DrawOnHidden();
            Clear(host, page);
            Emit(host, E.Pixel, x, Int16Math.S(y + 6), 2);
            page.SetPixel(x, Int16Math.S(y + 6), 2);
            Execute(host, page);
        }
        Clear(host, page);
        FillRect(host, page, 0, 6, 0x24, 0x0D, 9);
        Execute(host, page);
    }

    /// <summary>
    /// 20B3:07BA – the orbit: object 0 shown with frame 5; for si = 120 down to 0 per frame flip, restore, 2 retraces, position
    /// (sin si / 8 + A0h, 64h − cos si / 11) and below 50 every tenth step (si mod 10 = 9) frame si / 10, objects; then object 0
    /// hidden, restore, flip.
    /// </summary>
    // [ORIGINAL] 20B3:07BA
    public static IEnumerable<FieldWait> Orbit(Dgroup d, IEndingHost host, EndingPage page)
    {
        ShowObject0(d, host);
        Frame(d, 5);
        for (short si = 0x78; si >= 0; si--)
        {
            foreach (var w in Flip(d, host, page)) yield return w;
            Emit(host, F.RestoreObjects);
            foreach (var w in Delay(host, 2)) yield return w;
            d.Set(At(Planting.ObjectX), Int16Math.S(Int16Math.Div(Sin(d, si), 8) + 0xA0));
            d.Set(At(Planting.ObjectY), Int16Math.S(0x64 - Int16Math.Div(Cos(d, si), 0x0B)));
            if (si < 0x32 && Int16Math.Mod(si, 0x0A) == 9) Frame(d, Int16Math.Div(si, 0x0A));
            Emit(host, F.DrawObjects);
        }
        HideObject(d, host, 0);
        Emit(host, F.RestoreObjects);
        foreach (var w in Flip(d, host, page)) yield return w;
    }

    // ------------------------------------------------------------------ 0863 ranking

    /// <summary>
    /// 20B3:0863 – the ranking: V-ENDE, fade-out, object 0 with the frames of row 1 (0D0D(1)), the picture (0, 14h, 140h, C8h)
    /// to (0, 0) on both pages, frame DS:1080 / 13, object shown, palette DS:1040, fade-in; then per frame flip, restore, list,
    /// the blinking 0986 and the state DS:54D4 (0 400 frames, 1 and 5 the walk 09D4, 2 and 4 the bows 0A9F/0B40, 3 the drink
    /// 0ADC, 6 750 frames), list and objects, until state 7; 21FD:0665 and the wipe 0C05.
    /// </summary>
    // [ORIGINAL] 20B3:0863
    public static IEnumerable<FieldWait> Ranking(Dgroup d, IEndingHost host, EndingPage page)
    {
        short di = 0x190, si = 0x2EE;
        Load(d, host, page, null, RankPictureNumber);
        foreach (var w in FadeOut(d, host)) yield return w;
        ObjectFrameSet(d, host, 1);
        Clear(host, page);
        Blit(host, page, 0, 0x14, 0x140, 0xC8, 0, 0);
        foreach (var w in BothPages(d, host, page)) yield return w;
        Frame(d, Int16Math.Div(d.S16(Walk), 0x0D));
        ShowObject0(d, host);
        Emit(host, F.BasePalette, RankPalette);
        foreach (var w in FadeIn(d, host)) yield return w;
        d.Set(RankState, (short)0);
        Clear(host, page);
        while (d.S16(RankState) != 7)
        {
            foreach (var w in Flip(d, host, page)) yield return w;
            Emit(host, F.RestoreObjects);
            Execute(host, page);
            Clear(host, page);
            Blink(d, host, page);
            switch (d.U16(RankState))
            {
                case 0:
                    di = Int16Math.S(di - 1);
                    if (di == 0) d.Set(RankState, (short)1);
                    break;
                case 1:
                case 5: WinnerWalk(d, host); break;
                case 2: Bow(d, Bow1, 7, 3); break;
                case 3: Toast(d, host, page); break;
                case 4: Bow(d, Bow2, 5, 5); break;
                case 6:
                    si = Int16Math.S(si - 1);
                    if (si == 0) d.Set(RankState, (short)7);
                    break;
            }
            Execute(host, page);
            Emit(host, F.DrawObjects);
        }
        foreach (var w in LastFrame(d, host, page)) yield return w;
        foreach (var w in Wipe(d, host, page)) yield return w;
    }

    /// <summary>20B3:0986 – the blinking: DS:1082 counts down; at 0 the next of four pictures (DS:1084) at (D6h, A1h), timer 6.</summary>
    // [ORIGINAL] 20B3:0986
    public static void Blink(Dgroup d, IEndingHost host, EndingPage page)
    {
        d.Set(BlinkTimer, Int16Math.S(d.S16(BlinkTimer) - 1));
        if (d.S16(BlinkTimer) > 0) return;
        d.Set(BlinkStep, unchecked((byte)(d.U8(BlinkStep) + 1)));
        if (d.S8(BlinkStep) > 3) d.Set(BlinkStep, (byte)0);
        Blit(host, page, Int16Math.S(d.S8(BlinkStep) << 4), 0xEB, 0x10, 0x15, 0xD6, 0xA1);
        d.Set(BlinkTimer, (short)6);
    }

    /// <summary>
    /// 20B3:09D4 – the winner's walk: DS:1080 one up in state 1, one down otherwise. Below 0: object 0 hidden, state 6. Below
    /// 44h: straight at (1080 + 11h, 73h), frame 1080 / 11 where 1080 mod 11 = 5. Below 1D9h: on a circle, angle
    /// s = B4h − 2·(1080 − 44h), at (sin s / 21h + 55h, 69h − cos s / 21h − 14h). Below 1ECh: straight at (1080 − 166h, 55h).
    /// Otherwise state 2.
    /// </summary>
    // [ORIGINAL] 20B3:09D4
    public static void WinnerWalk(Dgroup d, IEndingHost host)
    {
        d.Set(Walk, Int16Math.S(d.S16(Walk) + (d.S16(RankState) == 1 ? 1 : -1)));
        short w = d.S16(Walk);
        if (w < 0)
        {
            HideObject(d, host, 0);
            d.Set(RankState, (short)6);
        }
        else if (w < 0x44)
        {
            d.Set(At(Planting.ObjectX), Int16Math.S(w + 0x11));
            d.Set(At(Planting.ObjectY), (short)0x73);
            if (Int16Math.Mod(w, 0x0B) == 5) Frame(d, Int16Math.Div(w, 0x0B));
        }
        else if (w < 0x1D9)
        {
            short s = Int16Math.S(0xB4 - Int16Math.S(Int16Math.S(w - 0x44) << 1));
            d.Set(At(Planting.ObjectX), Int16Math.S(Int16Math.Div(Sin(d, s), 0x21) + 0x55));
            d.Set(At(Planting.ObjectY), Int16Math.S(0x69 - Int16Math.Div(Cos(d, s), 0x21) - 0x14));
        }
        else if (w < 0x1EC)
        {
            d.Set(At(Planting.ObjectX), Int16Math.S(w - 0x166));
            d.Set(At(Planting.ObjectY), (short)0x55);
        }
        else d.Set(RankState, (short)2);
    }

    /// <summary>
    /// 20B3:0A9F (counter DS:1085, frames 6 and 7, next state 3) and 20B3:0B40 (counter DS:1089, frames 6 and 5, next state 5):
    /// the counter one down; at 10 frame 6, at 5 the second frame, at 0 the next state.
    /// </summary>
    // [ORIGINAL] 20B3:0A9F, 20B3:0B40
    public static void Bow(Dgroup d, int counter, short second, short next)
    {
        d.Set(counter, Int16Math.S(d.S16(counter) - 1));
        short c = d.S16(counter);
        if (c == 0x0A) Frame(d, 6);
        else if (c == 5) Frame(d, second);
        else if (c == 0) d.Set(RankState, next);
    }

    /// <summary>
    /// 20B3:0ADC – the drink: pixels (DS:1087 / 20 + 91h, 5Bh) in colour 0 and (… + 90h, 5Bh) in colour 1; DS:1087 one up; above
    /// 100 one winner less (DS:54D6): while some are left DS:1087 = 0, else state 4.
    /// </summary>
    // [ORIGINAL] 20B3:0ADC
    public static void Toast(Dgroup d, IEndingHost host, EndingPage page)
    {
        short x = Int16Math.Div(d.S16(Drink), 0x14);
        Emit(host, E.Pixel, Int16Math.S(x + 0x91), 0x5B, 0);
        page.SetPixel(Int16Math.S(x + 0x91), 0x5B, 0);
        Emit(host, E.Pixel, Int16Math.S(x + 0x90), 0x5B, 1);
        page.SetPixel(Int16Math.S(x + 0x90), 0x5B, 1);
        d.Set(Drink, Int16Math.S(d.S16(Drink) + 1));
        if (d.S16(Drink) <= 0x64) return;
        d.Set(Winners, unchecked((byte)(d.U8(Winners) - 1)));
        if (d.S8(Winners) > 0) d.Set(Drink, (short)0);
        else d.Set(RankState, (short)4);
    }

    /// <summary>
    /// 20B3:0C05 – the wipe: drawing on the page shown (21FD:0590); 60 pairs of vertical lines from the sides (no frame); 200
    /// steps of four lines in colour 0, from step 80 on every 16th with a palette step 21FD:03EB(0, n); blend colour 0, flip,
    /// object restore records cleared (21AF:04CD).
    /// </summary>
    // [ORIGINAL] 20B3:0C05
    public static IEnumerable<FieldWait> Wipe(Dgroup d, IEndingHost host, EndingPage page)
    {
        short di = 0;
        Emit(host, E.DrawOnShownPage);
        page.Page.DrawOnShownPage();
        yield return FieldWait.Retrace; // 0B67:0238
        yield return FieldWait.Retrace; // 0B67:028F
        for (short si = 0; si <= 0x3B; si++)
        {
            Clear(host, page);
            Emit(host, E.VerticalLine, si, 0, 0xC8, 0);
            Emit(host, E.VerticalLine, Int16Math.S(0x13F - si), 0, 0xC8, 0);
            Execute(host, page);
        }
        for (short si = 0; si <= 0xC7; si++)
        {
            Clear(host, page);
            Emit(host, E.Line, 0x3C, 0, 0x103, si, 0);
            Emit(host, E.Line, 0x103, 0, Int16Math.S(0x103 - si), 0xC7, 0);
            Emit(host, E.Line, 0x103, 0xC7, 0x3C, Int16Math.S(0xC7 - si), 0);
            Emit(host, E.Line, 0x3C, 0xC7, Int16Math.S(si + 0x3C), 0, 0);
            if (si >= 0x50 && Int16Math.Mod(si, 0x10) == 0)
            {
                di = Int16Math.S(di + 1);
                Emit(host, E.PaletteStep, 0, di);
                // [ORIGINAL] 21FD:03EB – 2295:00C5 (one retrace) only when DS:090E = 1
                if (d.S16(Planting.CardMode) == 1) yield return FieldWait.Retrace;
            }
            Execute(host, page);
        }
        Emit(host, E.BlendColor, 0);
        foreach (var w in Flip(d, host, page)) yield return w;
        Emit(host, F.ResetObjectRestore);
    }

    // ------------------------------------------------------------------ objects and helpers

    /// <summary>
    /// 20B3:0D0D(arg) – object 0 on layer 7 (DS:330E), buffers (0, 120h, 0, 120h, 7), eight frames i at (10h·i, DS:108B[arg])
    /// with masks (10h·i, 37h).
    /// </summary>
    // [ORIGINAL] 20B3:0D0D
    public static void ObjectFrameSet(Dgroup d, IEndingHost host, short arg)
    {
        d.Set(At(Planting.ObjectLayer), (short)7);
        Emit(host, F.ObjectBuffers, 0, 0x120, 0, 0x120, 7);
        short y = d.S16(Word(ObjectFrames, arg));
        for (short si = 0; si <= 7; si++)
            Emit(host, F.ObjectImage, 0, si, Int16Math.S(si << 4), y, Int16Math.S(si << 4), 0x37);
    }

    /// <summary>20B3:0D65 – 21AF:0412(0, 0): object 0 shown with frame 0 (core part).</summary>
    // [ORIGINAL] 20B3:0D65
    public static void ShowObject0(Dgroup d, IEndingHost host) => Planting.ShowCursor(d, host, 0);

    /// <summary>20B3:0D77(n) – frame of object 0 (DS:3324).</summary>
    // [ORIGINAL] 20B3:0D77
    public static void Frame(Dgroup d, short n) => d.Set(At(Planting.ObjectFrame), n);

    /// <summary>20B3:0B7D(a) – cos a from the table DS:0D17 (a reduced to 0…359 by steps of 360; signs by quadrant).</summary>
    // [ORIGINAL] 20B3:0B7D
    public static short Cos(Dgroup d, short a)
    {
        short si = a;
        while (si >= 0x168) si = Int16Math.S(si - 0x168);
        while (si < 0) si = Int16Math.S(si + 0x168);
        if (si < 0x5A) return d.S16(Word(CosTable, si));
        if (si < 0xB4) return Int16Math.S(-d.S16(Word(CosTable, Int16Math.S(0x5A - (si - 0x5A)))));
        if (si < 0x10E) return Int16Math.S(-d.S16(At(0x0BAF + Int16Math.S(si << 1))));
        return d.S16(Word(CosTable, Int16Math.S(0x5A - (si - 0x10E))));
    }

    /// <summary>20B3:0BF0(a) – sin a = −cos(a + 90).</summary>
    // [ORIGINAL] 20B3:0BF0
    public static short Sin(Dgroup d, short a) => Int16Math.S(-Cos(d, Int16Math.S(a + 0x5A)));

    /// <summary>21AF:045E(object): the object's visible byte cleared (core part of the event).</summary>
    // [ORIGINAL] 21AF:045E
    private static void HideObject(Dgroup d, IEndingHost host, short obj)
    {
        Emit(host, E.HideObject, obj);
        d.Set(At(FieldCursor.Sprite + Obj(obj)), (byte)0);
    }

    /// <summary>0609:1879(40h·p + 20h, 6, rank) for player <paramref name="p"/> (= DS:37AA).</summary>
    private static void Emblem(Dgroup d, IEndingHost host, int p, short rank) =>
        Emit(host, E.Emblem, Int16Math.S((p << 6) + 0x20), 6, rank, (short)P(d));

    private static void Load(Dgroup d, IEndingHost host, EndingPage page, Func<int, IndexedImage>? images, int picture)
    {
        short n = d.S16(picture);
        Emit(host, F.LoadImage, n);
        page.Atlas = images?.Invoke(n);
    }

    private static void Clear(IEndingHost host, EndingPage page)
    {
        Emit(host, F.ClearList);
        page.Clear();
    }

    private static void Execute(IEndingHost host, EndingPage page)
    {
        Emit(host, F.RunList);
        page.Run();
    }

    private static void Blit(IEndingHost host, EndingPage page, short sx, short sy, short w, short h, short dx, short dy)
    {
        Emit(host, F.DrawCommand, sx, sy, w, h, dx, dy);
        page.Blit(sx, sy, w, h, dx, dy);
    }

    private static void FillRect(IEndingHost host, EndingPage page, short x, short y, short w, short h, short colour)
    {
        Emit(host, E.FillRect, x, y, w, h, colour);
        page.FillRect(x, y, w, h, colour);
    }

    /// <summary>21FD:0620 – 23CD:071C, 0A96:001B.</summary>
    // [ORIGINAL] 21FD:0620
    private static void RestoreAndClear(IEndingHost host, EndingPage page)
    {
        Emit(host, F.RestoreObjects);
        Clear(host, page);
    }

    /// <summary>21FD:0634 – list, objects, flip, restore, list, objects.</summary>
    // [ORIGINAL] 21FD:0634
    private static IEnumerable<FieldWait> ListAndObjectsOnBothPages(Dgroup d, IEndingHost host, EndingPage page)
    {
        Execute(host, page);
        Emit(host, F.DrawObjects);
        foreach (var w in Flip(d, host, page)) yield return w;
        Emit(host, F.RestoreObjects);
        Execute(host, page);
        Emit(host, F.DrawObjects);
    }

    // [ORIGINAL] 21FD:060D – 0A96:06A8, 21FD:051F, 0A96:06A8
    private static IEnumerable<FieldWait> BothPages(Dgroup d, IEndingHost host, EndingPage page)
    {
        Emit(host, F.ListOnBothPages);
        page.RunOnBothPages();
        return Planting.Flip(d);
    }

    private static IEnumerable<FieldWait> Flip(Dgroup d, IEndingHost host, EndingPage page)
    {
        Emit(host, F.FlipPages);
        page.Page.Flip();
        return Planting.Flip(d);
    }

    /// <summary>21FD:0665 – flip, restore, list, objects.</summary>
    // [ORIGINAL] 21FD:0665
    private static IEnumerable<FieldWait> LastFrame(Dgroup d, IEndingHost host, EndingPage page)
    {
        Emit(host, F.LastFrame);
        page.Page.Flip();
        page.Run();
        return Planting.Flip(d);
    }

    private static IEnumerable<FieldWait> Delay(IEndingHost host, short n)
    {
        Emit(host, F.Delay, n, 0);
        return Planting.Retraces(n, 0);
    }

    private static IEnumerable<FieldWait> FadeOut(Dgroup d, IEndingHost host)
    {
        Emit(host, F.FadeOut, 0);
        return Planting.Fade(d, 4);
    }

    private static IEnumerable<FieldWait> FadeIn(Dgroup d, IEndingHost host)
    {
        Emit(host, F.FadeIn);
        return Planting.Fade(d, 3);
    }

    private static int Strlen(Dgroup d, int offset)
    {
        int n = 0;
        while (d.U8(At(offset + n)) != 0) n++;
        return n;
    }

    /// <summary>
    /// The text of 0A96:057A/05FB: 2295:0001(number, digits) – "%*lu", leading blanks and zeros (at most digits − 1) replaced
    /// by DS:1126 – with a leading "$" for 05FB.
    /// </summary>
    // [ORIGINAL] 2295:0001 – as IncidentText.FormatNumber (widths 1 and 5 here)
    private static byte[] NumberText(Dgroup d, uint number, int width, bool dollar)
    {
        string s = number.ToString(CultureInfo.InvariantCulture).PadLeft(width);
        var digits = s.Select(c => (byte)c).ToArray();
        for (int i = 0; width - 1 > i; i++)
        {
            if (digits[i] is (byte)' ' or (byte)'0') digits[i] = d.U8(Events.IncidentText.NumberFill);
            else break;
        }
        var text = digits.AsSpan(0, width).ToArray();
        return dollar ? [(byte)'$', .. text] : text;
    }
}
