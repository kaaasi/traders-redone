using Traders.Core.Field;
using Traders.Core.State;

namespace Traders.Core.Harvest;

/// <summary>
/// Harvest of the week 22A3:02D3 (call 16 of the week loop; finding docs/routines/22A3_02D3_ernte.md): the field is shown
/// again, the weather notice is typed, then every player's cursor walks over his fields in reading order (column by column
/// within a row) and collects the yield of DS:48F4 (computed by 22A3:0003) into the stock DS:3CF4, capped at 999. A human
/// player advances with "right" (action 0) or, after "fire" (action 0Ah), automatically like the computer. Works on the
/// data segment like the original; drawing and sound are emitted as events in the original's order (<see cref="IHarvestHost"/>),
/// the random draws of the field border (0609:13C5) are done here. Every yielded <see cref="FieldWait"/> is one frame.
/// </summary>
public static class Harvest
{
    // DS offsets (docs/analyse/DATENLAYOUT.md, section of lane ernte).
    public const int Counter = 0x57E8;     // word[4], frames of the running collection (the bar grows by one per frame)
    public const int Automatic = 0x57F4;   // byte[4], 1 = advance without a key (computer, or after action 0Ah)
    public const int Yield = DgroupLayout.ProductionResult; // byte[x·1Ah + y], yield per field (22A3:0003)
    public const int Kind = DgroupLayout.LoopWare;          // word, kind of the field being collected (also the loop of 0340)
    public const int WeatherLines = 0x3BA4;                 // far pointer[6][3], weather notice (text entries F6h + 3·weather)
    public const int ColourOfPlayer = 0x00C8;               // word[4], colour of the player's bar

    /// <summary>Modes DS:43B2 of the harvest: 7 waiting for the next field, 8 collecting, 6 done.</summary>
    public const byte Waiting = 7, Collecting = 8, Done = 6;

    /// <summary>Upper bound of a stock (3E7h).</summary>
    public const short StockLimit = 999;

    private static int P(Dgroup d) => d.S8(FieldGrid.CurrentPlayer);
    private static int At(int offset) => Int16Math.U(offset);
    private static int Cell(int x, int y) => Int16Math.U(Int16Math.S(x * 6) + y);
    private static int YieldCell(int x, int y) => Int16Math.U(Int16Math.S(x * 0x1A) + y);
    private static int StockOf(int p, int kind) => At(DgroupLayout.Stock + Int16Math.S((p << 3) + Int16Math.S(kind << 1)));
    private static short PanelX(int p, int add) => Int16Math.S((p << 6) + add);

    private static void Emit(IHarvestHost h, FieldEventKind k, params short[] args) => h.Emit(new FieldEvent(k, args));
    private static void Emit(IHarvestHost h, HarvestEventKind k, params short[] args) => h.Emit(new HarvestEvent(k, args));

    private static void ForPlayers(Dgroup d, Action<int> body)
    {
        for (d.Set(FieldGrid.CurrentPlayer, (sbyte)0); d.S8(FieldGrid.CurrentPlayer) < 4;
             d.Set(FieldGrid.CurrentPlayer, unchecked((sbyte)(d.S8(FieldGrid.CurrentPlayer) + 1))))
            body(P(d));
    }

    private static IEnumerable<FieldWait> ForPlayersFrames(Dgroup d, Func<int, IEnumerable<FieldWait>> body)
    {
        for (d.Set(FieldGrid.CurrentPlayer, (sbyte)0); d.S8(FieldGrid.CurrentPlayer) < 4;
             d.Set(FieldGrid.CurrentPlayer, unchecked((sbyte)(d.S8(FieldGrid.CurrentPlayer) + 1))))
            foreach (var w in body(P(d))) yield return w;
    }

    /// <summary>
    /// The whole harvest 22A3:02D3 frame by frame: picture, cursor objects, fade-out to 007h, the board 0609:1BE5 with its
    /// border draws, four players on the field, palette, fade-in, the weather notice 22A3:08C1, the panels 22A3:0340, sound
    /// (0, 17h), the collection loop 22A3:0426, sound (0, 0).
    /// </summary>
    // [ORIGINAL] 22A3:02D3
    public static IEnumerable<FieldWait> Run(Dgroup d, OriginalRandom random, IHarvestHost host)
    {
        var field = new FieldOutput(host);
        Emit(host, FieldEventKind.LoadImage, d.S16(Planting.PictureNumber));
        Planting.CursorObjects(d, field);
        Emit(host, FieldEventKind.FadeOut, 7);
        foreach (var w in Planting.Fade(d, 4)) yield return w;
        foreach (var w in Planting.BoardFrames(d, random, field)) yield return w;
        d.Set(Planting.Players, (byte)4);
        Emit(host, FieldEventKind.BasePalette, 0xA8);
        Planting.WeatherColours(d, field);
        Emit(host, FieldEventKind.FadeIn);
        foreach (var w in Planting.Fade(d, 3)) yield return w;
        foreach (var w in NoticeFrames(d, host)) yield return w;
        foreach (var w in PanelsFrames(d, host)) yield return w;
        Emit(host, FieldEventKind.ClearList);
        Emit(host, FieldEventKind.Sound, 0, 0x17);
        foreach (var w in CollectFrames(d, host)) yield return w;
        Emit(host, FieldEventKind.Sound, 0, 0);
    }

    /// <summary>
    /// 22A3:08C1: font 1, the three lines of the weather notice typed at (0, 14h) centred in 140h (colour 9, five retraces
    /// per character), font 0, 50h retraces, then the notice area (10h, 6, 120h, 2Bh) tiled over and shown on both pages.
    /// </summary>
    // [ORIGINAL] 22A3:08C1
    public static IEnumerable<FieldWait> NoticeFrames(Dgroup d, IHarvestHost host)
    {
        Emit(host, HarvestEventKind.SetFont, 1);
        short lines = Int16Math.S(WeatherLines + Int16Math.S(Int16Math.S(d.S8(DgroupLayout.Weather) * 3) << 2));
        foreach (var w in TypeLines(d, host, 0, 0x14, 0x140, lines, 3, 9, 5)) yield return w;
        Emit(host, HarvestEventKind.SetFont, 0);
        Emit(host, FieldEventKind.Delay, 0x50, 0);
        foreach (var w in Planting.Retraces(0x50, 0)) yield return w;
        Emit(host, FieldEventKind.ClearList);
        Emit(host, HarvestEventKind.TileArea, 0x10, 6, 0x120, 0x2B);
        Emit(host, FieldEventKind.ListOnBothPages);
        foreach (var w in Planting.Flip(d)) yield return w;
    }

    /// <summary>
    /// 0A69:0288(x, y, w, lines, count, colour, wait) with the far pointer array in the data segment: for line i = 0 … count − 1
    /// (signed) 0A69:0243 types it centred (0A69:01AD): per character the list is emptied, the character drawn, 21FD:060D
    /// shows it on both pages, a key click (1, 16h) unless it is a blank, the pause check 21FD:0245 and 21FD:0185(wait). A
    /// null line is skipped; the final wait of 0243 is −1 (none).
    /// </summary>
    // [ORIGINAL] 0A69:0288 – frames and sound; positions (0A69:0243 centring with DS:3CA8, line step from DS:34A4) are the scene's
    // [ORIGINAL] 0A69:0243
    // [ORIGINAL] 0A69:01AD
    public static IEnumerable<FieldWait> TypeLines(Dgroup d, IHarvestHost host, short x, short y, short w, short lines, short count,
        short colour, short wait)
    {
        Emit(host, HarvestEventKind.TypeLines, x, y, w, lines, unchecked((short)DgroupLayout.Segment), count, colour, wait);
        for (short i = 0; i < count; i = Int16Math.S(i + 1))
        {
            int at = Int16Math.U(lines + Int16Math.S(i << 2));
            ushort offset = d.U16(at), segment = d.U16(At(at + 2));
            if (offset == 0 && segment == 0) continue;
            byte[] text = host.FarString(offset, segment);
            for (int k = 0; k < text.Length; k++)
            {
                Emit(host, FieldEventKind.ClearList);
                Emit(host, HarvestEventKind.TypeCharacter, text[k], i, (short)k);
                Emit(host, FieldEventKind.ListOnBothPages);
                foreach (var f in Planting.Flip(d)) yield return f;
                if (text[k] != (byte)' ') Emit(host, FieldEventKind.Sound, 1, 0x16);
                foreach (var f in host.PauseCheck(d)) yield return f;
                short high = (short)(wait < 0 ? -1 : 0); // cwd
                Emit(host, FieldEventKind.Delay, wait, high);
                foreach (var f in Planting.Retraces(wait, high)) yield return f;
            }
        }
    }

    /// <summary>
    /// 22A3:0340: per player the panel – emblem 0609:1854, time bar 0609:1AC0, cursor shown (0609:1FCC), move delay cleared,
    /// empty selection panel, the four goods with their stocks – the cursor put before the first field (x = −1, y = 0) and
    /// moved to the first own field with a yield (22A3:04F7), counter 0, automatic for computers; then 21FD:060D.
    /// </summary>
    // [ORIGINAL] 22A3:0340
    // [ORIGINAL] 21FD:060D – 0A96:06A8, 21FD:051F, 0A96:06A8
    public static IEnumerable<FieldWait> PanelsFrames(Dgroup d, IHarvestHost host)
    {
        var field = new FieldOutput(host);
        return ForPlayersFrames(d, p => Panel(d, host, field, p));
    }

    private static IEnumerable<FieldWait> Panel(Dgroup d, IHarvestHost host, FieldOutput field, int p)
    {
        Emit(host, FieldEventKind.ClearList);
        Emit(host, FieldEventKind.DrawRank, PanelX(p, 0x20), 6);
        Emit(host, FieldEventKind.DrawPlayer);
        Planting.ShowCursor(d, field);
        d.Set(At(FieldCursor.MoveDelay + 4 * P(d)), 0);
        Emit(host, HarvestEventKind.DrawEmptyPanel);
        for (d.Set(Kind, (short)0); d.S16(Kind) <= 3; d.Set(Kind, Int16Math.S(d.S16(Kind) + 1)))
        {
            short k = d.S16(Kind);
            Emit(host, HarvestEventKind.DrawGoods, k, PanelX(P(d), 0x20), Int16Math.S((k << 3) + 0x0D));
            ShowStock(d, host);
        }
        d.Set(At(FieldCursor.CursorX + P(d)), (sbyte)-1);
        d.Set(At(FieldCursor.CursorY + P(d)), (sbyte)0);
        NextField(d, host);
        d.Set(At(Counter + 2 * P(d)), (short)0);
        d.Set(At(Automatic + P(d)), (byte)(d.U8(At(DgroupLayout.IsComputer + P(d))) == 1 ? 1 : 0));
        Emit(host, FieldEventKind.ListOnBothPages);
        foreach (var w in Planting.Flip(d)) yield return w;
    }

    /// <summary>One pass of the collection loop 22A3:0426 with its frames drained (see <see cref="PassFrames"/>).</summary>
    public static void Pass(Dgroup d, IHarvestHost host)
    {
        foreach (var _ in PassFrames(d, host)) { }
    }

    /// <summary>
    /// 22A3:0426: passes while players are on the field (DS:3CE6 ≠ 0), then the last frame 21FD:0665.
    /// </summary>
    // [ORIGINAL] 22A3:0426
    // [ORIGINAL] 21FD:0665 – 21FD:051F, 23CD:071C, 0A96:06A8, 23CD:06E9
    public static IEnumerable<FieldWait> CollectFrames(Dgroup d, IHarvestHost host)
    {
        while (d.U8(Planting.Players) != 0)
            foreach (var w in PassFrames(d, host)) yield return w;
        Emit(host, FieldEventKind.LastFrame);
        foreach (var w in Planting.Flip(d)) yield return w;
    }

    /// <summary>
    /// One pass of 22A3:0426: flip, one retrace, restore, list, controls, pause, then per player mode 7 → 22A3:04B1, mode 8 →
    /// 22A3:062E (the mode as unsigned byte); list and objects.
    /// </summary>
    public static IEnumerable<FieldWait> PassFrames(Dgroup d, IHarvestHost host)
    {
        Emit(host, FieldEventKind.FlipPages);
        foreach (var w in Planting.Flip(d)) yield return w;
        Emit(host, FieldEventKind.Delay, 1, 0);
        foreach (var w in Planting.Retraces(1, 0)) yield return w;
        Emit(host, FieldEventKind.RestoreObjects);
        Emit(host, FieldEventKind.RunList);
        Emit(host, FieldEventKind.ClearList);
        host.ReadControls(d);
        foreach (var w in host.PauseCheck(d)) yield return w;
        ForPlayers(d, p =>
        {
            switch (d.U8(At(FieldCursor.Mode + p)))
            {
                case Waiting: Advance(d, host); break;
                case Collecting: Collect(d, host); break;
            }
        });
        Emit(host, FieldEventKind.RunList);
        Emit(host, FieldEventKind.DrawObjects);
    }

    /// <summary>22A3:04B1: waiting – with action 0 (right) or automatic the cursor goes on (22A3:04F7); then 22A3:04DA.</summary>
    // [ORIGINAL] 22A3:04B1
    public static void Advance(Dgroup d, IHarvestHost host)
    {
        int p = P(d);
        if (d.U8(At(FieldCursor.Action + p)) == 0 || d.U8(At(Automatic + p)) == 1) NextField(d, host);
        CheckAutomatic(d);
    }

    /// <summary>22A3:04DA: action 0Ah (fire) switches the player at turn to automatic.</summary>
    // [ORIGINAL] 22A3:04DA
    public static void CheckAutomatic(Dgroup d)
    {
        if (d.U8(At(FieldCursor.Action + P(d))) == 0x0A) d.Set(At(Automatic + P(d)), (byte)1);
    }

    /// <summary>
    /// 22A3:04F7: the bar area of the player is cleared once (22A3:0806), then the cursor steps on – x + 1, after the last column
    /// x = 0 and y + 1 (8-bit cursor, signed 16-bit compare with columns − 1 / rows − 1) – until it stands on an own field
    /// (owner byte = DS:37AA) with a yield ≠ 0: the cursor object is put there (x·20h + origin + 8, y·16h + origin + 0Ch), the
    /// move delay set to 7D0h and the mode to 8 (collecting). After the last row the cursor is hidden (0609:21BA), one
    /// player fewer is on the field and the mode is 6.
    /// </summary>
    // [ORIGINAL] 22A3:04F7
    public static void NextField(Dgroup d, IHarvestHost host)
    {
        ClearBar(d, host);
        while (true)
        {
            int p = P(d);
            sbyte x = unchecked((sbyte)(d.S8(At(FieldCursor.CursorX + p)) + 1));
            d.Set(At(FieldCursor.CursorX + p), x);
            if (x > Int16Math.S(d.S16(FieldGrid.Columns) - 1))
            {
                d.Set(At(FieldCursor.CursorX + p), (sbyte)0);
                sbyte y = unchecked((sbyte)(d.S8(At(FieldCursor.CursorY + p)) + 1));
                d.Set(At(FieldCursor.CursorY + p), y);
                if (y > Int16Math.S(d.S16(FieldGrid.Rows) - 1))
                {
                    FieldCursor.HideCursor(d);
                    d.Set(Planting.Players, unchecked((byte)(d.U8(Planting.Players) - 1)));
                    d.Set(At(FieldCursor.Mode + P(d)), Done);
                    return;
                }
            }
            d.Set(DgroupLayout.LoopX, d.S8(At(FieldCursor.CursorX + P(d))));
            d.Set(DgroupLayout.LoopY, d.S8(At(FieldCursor.CursorY + P(d))));
            int cx = d.S8(DgroupLayout.LoopX), cy = d.S8(DgroupLayout.LoopY);
            if (d.U8(At(FieldGrid.Owner + Cell(cx, cy))) != d.U8(FieldGrid.CurrentPlayer)) continue;
            if (d.U8(At(Yield + YieldCell(cx, cy))) == 0) continue;
            p = P(d);
            short ox = Int16Math.S((d.S8(At(FieldCursor.CursorX + p)) << 5) + d.S16(MenuOrigin.X) + 8);
            d.Set(At(Planting.ObjectX + Int16Math.S(p * Planting.ObjectSize)), ox);
            short oy = Int16Math.S(Int16Math.S(d.S8(At(FieldCursor.CursorY + p)) * 0x16) + d.S16(MenuOrigin.Y) + 0x0C);
            d.Set(At(Planting.ObjectY + Int16Math.S(p * Planting.ObjectSize)), oy);
            d.Set(At(FieldCursor.MoveDelay + 4 * p), 0x7D0);
            d.Set(At(FieldCursor.Mode + p), Collecting);
            return;
        }
    }

    /// <summary>
    /// 22A3:062E: collecting – 22A3:04DA, the field under the cursor gives the kind (DS:45D0 = kind byte, signed); the counter
    /// counts up; while it is ≤ yield / 2 the bar grows (22A3:07C2) and the stock is shown as stock + 2·counter (22A3:082F);
    /// then the whole yield is added to the stock (capped at 999 when above, signed), the stock shown (22A3:0874), the
    /// counter cleared and the mode is 7 (waiting).
    /// </summary>
    // [ORIGINAL] 22A3:062E
    public static void Collect(Dgroup d, IHarvestHost host)
    {
        CheckAutomatic(d);
        int p = P(d);
        d.Set(DgroupLayout.LoopX, d.S8(At(FieldCursor.CursorX + p)));
        d.Set(DgroupLayout.LoopY, d.S8(At(FieldCursor.CursorY + p)));
        int x = d.S8(DgroupLayout.LoopX), y = d.S8(DgroupLayout.LoopY);
        d.Set(Kind, (short)d.S8(At(FieldGrid.Kind + Cell(x, y))));
        int counterAt = At(Counter + 2 * p);
        short counter = Int16Math.S(d.S16(counterAt) + 1);
        d.Set(counterAt, counter);
        byte amount = d.U8(At(Yield + YieldCell(x, y)));
        if (counter <= amount / 2)
        {
            Bar(d, host);
            ShowRunningStock(d, host, Int16Math.S(d.S16(StockOf(P(d), d.S16(Kind))) + Int16Math.S(d.S16(At(Counter + 2 * P(d))) << 1)));
            return;
        }
        int stock = StockOf(p, d.S16(Kind));
        d.Set(stock, Int16Math.S(d.S16(stock) + amount));
        if (d.S16(stock) > StockLimit) d.Set(StockOf(P(d), d.S16(Kind)), StockLimit);
        ShowStock(d, host);
        d.Set(At(Counter + 2 * P(d)), (short)0);
        d.Set(At(FieldCursor.Mode + P(d)), Waiting);
    }

    /// <summary>22A3:07C2: one line of the player's bar at height 2Ah − counter (x = p·40h + 46h … + 3, colour DS:00C8[p]).</summary>
    // [ORIGINAL] 22A3:07C2
    public static void Bar(Dgroup d, IHarvestHost host)
    {
        int p = P(d);
        short x = PanelX(p, 0x46), y = Int16Math.S(0x2A - d.S16(At(Counter + 2 * p)));
        Emit(host, HarvestEventKind.Line, x, y, Int16Math.S(x + 3), y, d.S16(At(ColourOfPlayer + 2 * p)));
    }

    /// <summary>22A3:0806: the bar area (p·40h + 46h, 0Ch, 4, 1Eh) filled with colour 5.</summary>
    // [ORIGINAL] 22A3:0806
    public static void ClearBar(Dgroup d, IHarvestHost host) =>
        Emit(host, HarvestEventKind.FillRect, PanelX(P(d), 0x46), 0x0C, 4, 0x1E, 5);

    /// <summary>22A3:082F(v): the running stock min(v, 999) (signed) as three digits in colour 2 at the goods row DS:45D0.</summary>
    // [ORIGINAL] 22A3:082F
    public static void ShowRunningStock(Dgroup d, IHarvestHost host, short value)
    {
        if (value > StockLimit) value = StockLimit;
        Emit(host, HarvestEventKind.Number, PanelX(P(d), 0x32), Int16Math.S((d.S16(Kind) << 3) + 0x12), value,
            (short)(value < 0 ? -1 : 0), 3, 2, 5);
    }

    /// <summary>22A3:0874: the stock of the goods DS:45D0 as three digits in colour 0Dh.</summary>
    // [ORIGINAL] 22A3:0874
    public static void ShowStock(Dgroup d, IHarvestHost host)
    {
        short value = d.S16(StockOf(P(d), d.S16(Kind)));
        Emit(host, HarvestEventKind.Number, PanelX(P(d), 0x32), Int16Math.S((d.S16(Kind) << 3) + 0x12), value,
            (short)(value < 0 ? -1 : 0), 3, 0x0D, 5);
    }

    /// <summary>The field layer's outputs of the shared routines (board, cursor objects, palette) go to the harvest's host.</summary>
    private sealed class FieldOutput(IHarvestHost host) : IFieldOutput
    {
        public void ComputerTurn(Dgroup d, short x, short y) =>
            throw new InvalidOperationException("The harvest has no computer turn.");

        public void Emit(FieldEvent e) => host.Emit(e);
    }
}
