using Traders.Core.Field;
using Traders.Core.Harvest;
using Traders.Core.State;

namespace Traders.Core.Extra;

/// <summary>
/// The extra phase 1DEF:000C (call 24 of the week loop; finding docs/routines/1DEF_000C_zusatzphase.md): with a chance of
/// 1 in 21, or when fewer than half of the idle fields produced (2·DS:57F2 &lt; DS:57F0), a notice is typed and 25 goods are
/// scattered over a board of 6 × 7 cells. All four players walk over the board at the same time – humans with the controls,
/// computers to the nearest unclaimed goods – and pick up what lies on their cell: each piece adds 2·DS:2F26 units of its
/// kind to the player's gain DS:5482. When all 25 are taken, the common wait loop runs and the gains are added to the stocks
/// DS:3CF4 (without an upper bound). Works on the data segment like the original, the stack arrays in an
/// <see cref="ExtraBoard"/>; drawing and sound are emitted as events in the original's order, every yielded
/// <see cref="FieldWait"/> is one frame.
/// </summary>
public static class ExtraPhase
{
    // DS offsets (docs/analyse/DATENLAYOUT.md, section of lane zusatz).
    public const int GroundPointer = 0x54A6;  // far pointer → ExtraBoard.Ground
    public const int GoodsPointer = 0x547C;   // far pointer → ExtraBoard.Goods
    public const int ClaimsPointer = 0x54A2;  // far pointer → ExtraBoard.Claims
    public const int Collected = 0x5480;      // word, goods picked up so far (the phase ends at 25)
    public const int Gains = 0x5482;          // word[4][4], stride 8: units gained per player and kind
    public const int TileX = 0x0F22;          // word[4], atlas x of the ground tiles (initialised data)
    public const int TileY = 0x0F2A;          // word[4], atlas y of the ground tiles
    public const int NoticeLines = 0x3B5C;    // far pointer[2], the two lines of the notice (text table)
    public const int ColourOfPlayer = Traders.Core.Harvest.Harvest.ColourOfPlayer; // word[4], colour of the gain numbers
    public const int Palette = 0x00A8;        // the palette argument of 21FD:026C (not read by it)

    /// <summary>Goods on the board at the start; the phase ends when as many are picked up (19h).</summary>
    public const short Pieces = 25;

    /// <summary>Board size and origin: 6 columns of 20h, 7 rows of 16h pixels at (70h, 17h).</summary>
    public const int Columns = 6, Rows = 7;

    private static int P(Dgroup d) => d.S8(FieldGrid.CurrentPlayer);
    private static int At(int offset) => Int16Math.U(offset);
    private static int Obj(int p) => Int16Math.U(Int16Math.S(p * Planting.ObjectSize));

    private static void Emit(IExtraHost h, FieldEventKind k, params short[] args) => h.Emit(new FieldEvent(k, args));
    private static void Emit(IExtraHost h, ExtraEventKind k, params short[] args) => h.Emit(new ExtraEvent(k, args));

    private static void ForPlayers(Dgroup d, Action<int> body)
    {
        for (d.Set(FieldGrid.CurrentPlayer, (sbyte)0); d.S8(FieldGrid.CurrentPlayer) < 4;
             d.Set(FieldGrid.CurrentPlayer, unchecked((sbyte)(d.S8(FieldGrid.CurrentPlayer) + 1))))
            body(P(d));
    }

    private static void ForKinds(Dgroup d, Action<short> body)
    {
        for (d.Set(DgroupLayout.LoopWare, (short)0); d.S16(DgroupLayout.LoopWare) <= 3;
             d.Set(DgroupLayout.LoopWare, Int16Math.S(d.S16(DgroupLayout.LoopWare) + 1)))
            body(d.S16(DgroupLayout.LoopWare));
    }

    private static int GainOf(int p, int kind) => At(Gains + Int16Math.S((p << 3) + Int16Math.S(kind << 1)));

    /// <summary>
    /// The whole phase 1DEF:000C frame by frame: the far pointers to the frame's arrays, the decision 00DB; if it falls: picture,
    /// cursor objects, fade-out to 0, base palette, weather colours, the notice 0105, fade-out, the board set-up 0173, 01AF, 020E,
    /// 0227, 03CD, fade-in, 20 retraces, the walk 04A9, the wait loop 21FD:0004(0), cursors off (0609:21BA) and the gains added
    /// to the stocks (0751).
    /// </summary>
    // [ORIGINAL] 1DEF:000C
    public static IEnumerable<FieldWait> Run(Dgroup d, OriginalRandom random, IExtraHost host, ExtraBoard board, ExtraFrame frame)
    {
        d.Set(GroundPointer, frame.GroundOffset);
        d.Set(At(GroundPointer + 2), frame.Ss);
        d.Set(GoodsPointer, frame.GoodsOffset);
        d.Set(At(GoodsPointer + 2), frame.Ss);
        d.Set(ClaimsPointer, frame.ClaimsOffset);
        d.Set(At(ClaimsPointer + 2), frame.Ss);
        if (!Triggered(d, random)) yield break;
        var field = new FieldOutput(host);
        Emit(host, FieldEventKind.LoadImage, d.S16(Planting.PictureNumber));
        Planting.CursorObjects(d, field);
        Emit(host, FieldEventKind.FadeOut, 0);
        foreach (var w in Planting.Fade(d, 4)) yield return w;
        Emit(host, FieldEventKind.BasePalette, Palette);
        Planting.WeatherColours(d, field);
        foreach (var w in NoticeFrames(d, host)) yield return w;
        Emit(host, FieldEventKind.FadeOut, 0);
        foreach (var w in Planting.Fade(d, 4)) yield return w;
        Ground(random, board);
        Scatter(random, board);
        ClearClaims(board);
        foreach (var w in BoardFrames(d, random, host, board)) yield return w;
        Players(d, random, host);
        Emit(host, FieldEventKind.FadeIn);
        foreach (var w in Planting.Fade(d, 3)) yield return w;
        Emit(host, FieldEventKind.Delay, 0x14, 0);
        foreach (var w in Planting.Retraces(0x14, 0)) yield return w;
        d.Set(Collected, (short)0);
        Emit(host, FieldEventKind.ClearList);
        foreach (var w in WalkFrames(d, random, host, board)) yield return w;
        foreach (var w in host.WaitLoop(d, random, 0)) yield return w;
        ForPlayers(d, _ => FieldCursor.HideCursor(d));
        Credit(d);
    }

    /// <summary>
    /// 1DEF:00DB: random(20) = 0 (1 in 21) gives AL = 1; otherwise AL = 1 when 2·DS:57F2 &lt; DS:57F0 (16-bit shift, signed
    /// compare), else 0. The draw is made in any case.
    /// </summary>
    // [ORIGINAL] 1DEF:00DB
    public static bool Triggered(Dgroup d, OriginalRandom random)
    {
        if (random.Next(0x14) == 0) return true;
        return Int16Math.S(d.S16(DgroupLayout.ProductionActive) << 1) < d.S16(DgroupLayout.ProductionIdle);
    }

    /// <summary>
    /// 1DEF:0105: background, frame (28h, 4Bh, F0h, 32h), list, page shown (0A96:0C3F), fade-in, the two lines DS:3B5C typed at
    /// (28h, 64h) in F0h (colour 5, five retraces per character), then 20 retraces.
    /// </summary>
    // [ORIGINAL] 1DEF:0105
    public static IEnumerable<FieldWait> NoticeFrames(Dgroup d, IExtraHost host)
    {
        Emit(host, FieldEventKind.ClearList);
        Emit(host, FieldEventKind.DrawBackground);
        Emit(host, FieldEventKind.DrawFrame, 0x28, 0x4B, 0xF0, 0x32);
        Emit(host, FieldEventKind.RunList);
        foreach (var w in ShowPage(d, host)) yield return w;
        Emit(host, FieldEventKind.FadeIn);
        foreach (var w in Planting.Fade(d, 3)) yield return w;
        foreach (var w in Traders.Core.Harvest.Harvest.TypeLines(d, new Typewriter(host), 0x28, 0x64, 0xF0, NoticeLines, 2, 5, 5))
            yield return w;
        Emit(host, FieldEventKind.Delay, 0x14, 0);
        foreach (var w in Planting.Retraces(0x14, 0)) yield return w;
    }

    /// <summary>0A96:0C3F([082E]) – the hidden page is copied; in the modes 2 and 63h it flips the pages first (21FD:051F).</summary>
    private static IEnumerable<FieldWait> ShowPage(Dgroup d, IExtraHost host)
    {
        Emit(host, FieldEventKind.ShowPage);
        if (d.S16(Planting.CardMode) is 2 or 0x63)
            foreach (var w in Planting.Flip(d)) yield return w;
    }

    /// <summary>1DEF:0173: per cell random(6) as ground tile; values above 3 (signed) become 0.</summary>
    // [ORIGINAL] 1DEF:0173
    public static void Ground(OriginalRandom random, ExtraBoard board)
    {
        for (int si = 0; si <= 0x29; si++)
        {
            short v = random.Next(6);
            board.Ground[si] = v;
            if (v > 3) board.Ground[si] = 0;
        }
    }

    /// <summary>
    /// 1DEF:01AF: all cells empty (−1), then 25 times a cell random(41) is drawn until an empty one comes up, and it gets the
    /// goods kind random(3).
    /// </summary>
    // [ORIGINAL] 1DEF:01AF
    public static void Scatter(OriginalRandom random, ExtraBoard board)
    {
        for (int si = 0; si <= 0x29; si++) board.Goods[si] = -1;
        for (int si = 0; si <= 0x18; si++)
        {
            short di;
            do di = random.Next(0x29);
            while (board.Goods[Cell(di)] != -1);
            board.Goods[Cell(di)] = random.Next(3);
        }
    }

    /// <summary>1DEF:020E: no cell is claimed (FFh).</summary>
    // [ORIGINAL] 1DEF:020E
    public static void ClearClaims(ExtraBoard board)
    {
        for (int si = 0; si <= 0x29; si++) board.Claims[si] = 0xFF;
    }

    /// <summary>
    /// 1DEF:0227: background, frame (0, 4, 50h, C0h), the border 0609:13C5(70h, 17h, C0h, 9Ah) with its random draws, every cell
    /// (0308), list; per player the panel box (atlas C0h, B0h, 30h × 2Ch at (10h, 2Ch·p + 0Ch)) with the four goods icons
    /// (0609:1912), list and the page shown.
    /// </summary>
    // [ORIGINAL] 1DEF:0227
    public static IEnumerable<FieldWait> BoardFrames(Dgroup d, OriginalRandom random, IExtraHost host, ExtraBoard board)
    {
        var field = new FieldOutput(host);
        Emit(host, FieldEventKind.ClearList);
        Emit(host, FieldEventKind.DrawBackground);
        Emit(host, FieldEventKind.DrawFrame, 0, 4, 0x50, 0xC0);
        Planting.Border(0x70, 0x17, 0xC0, 0x9A, random, field);
        for (short si = 0; si <= 0x29; si++) DrawCell(d, host, board, si);
        Emit(host, FieldEventKind.RunList);
        Emit(host, FieldEventKind.ClearList);
        ForPlayers(d, p =>
        {
            short top = Int16Math.S(p * 0x2C);
            Emit(host, FieldEventKind.DrawCommand, 0xC0, 0xB0, 0x30, 0x2C, 0x10, Int16Math.S(top + 0x0C));
            ForKinds(d, k => Emit(host, ExtraEventKind.DrawGoods, k, 0x10,
                Int16Math.S(Int16Math.S(P(d) * 0x2C) + Int16Math.S(k << 3) + 0x13)));
        });
        Emit(host, FieldEventKind.RunList);
        foreach (var w in ShowPage(d, host)) yield return w;
    }

    /// <summary>
    /// 1DEF:0308(cell): the ground tile (atlas DS:0F22/0F2A[tile], 20h × 16h) at (column·20h + 70h, row·16h + 17h) and, if goods
    /// lie there, their icon masked (atlas 130h, kind·7 + B0h; mask 80h, kind·7 + 28h; 10h × 7) at (column·20h + 78h,
    /// row·16h + 26h). Column and row by signed division by 6.
    /// </summary>
    // [ORIGINAL] 1DEF:0308
    public static void DrawCell(Dgroup d, IExtraHost host, ExtraBoard board, short cell)
    {
        short tile = board.Ground[Cell(cell)];
        short goods = board.Goods[Cell(cell)];
        short column = Int16Math.Mod(cell, 6), row = Int16Math.Div(cell, 6);
        short sx = d.S16(At(TileX + Int16Math.S(tile << 1))), sy = d.S16(At(TileY + Int16Math.S(tile << 1)));
        Emit(host, FieldEventKind.DrawCommand, sx, sy, 0x20, 0x16, Int16Math.S((column << 5) + 0x70),
            Int16Math.S(Int16Math.S(row * 0x16) + 0x17));
        if (goods == -1) return;
        short g7 = Int16Math.S(goods * 7);
        Emit(host, ExtraEventKind.MaskedBlit, 0x130, Int16Math.S(g7 + 0xB0), 0x80, Int16Math.S(g7 + 0x28), 0x10, 7,
            Int16Math.S((column << 5) + 0x78), Int16Math.S(Int16Math.S(row * 0x16) + 0x26));
    }

    /// <summary>
    /// 1DEF:03CD: per player no move running (DS:4BD2, DS:3022 = 0), the cursor on column random(5) and row random(6), the cursor
    /// object there (column·20h + 78h, row·16h + 23h) and shown (0609:1FCC), the gains cleared.
    /// </summary>
    // [ORIGINAL] 1DEF:03CD
    public static void Players(Dgroup d, OriginalRandom random, IExtraHost host)
    {
        var field = new FieldOutput(host);
        ForPlayers(d, p =>
        {
            d.Set(At(FieldCursor.MoveFrame + 2 * p), (short)0);
            d.Set(At(FieldCursor.MoveTimer + 2 * p), (short)0);
            d.Set(At(FieldCursor.CursorX + p), unchecked((byte)random.Next(5)));
            d.Set(At(FieldCursor.CursorY + P(d)), unchecked((byte)random.Next(6)));
            p = P(d);
            d.Set(At(Planting.ObjectX + Obj(p)), Int16Math.S((d.S8(At(FieldCursor.CursorX + p)) << 5) + 0x78));
            d.Set(At(Planting.ObjectY + Obj(p)), Int16Math.S(Int16Math.S(d.S8(At(FieldCursor.CursorY + p)) * 0x16) + 0x23));
            Planting.ShowCursor(d, field);
            ForKinds(d, k => d.Set(GainOf(P(d), k), (short)0));
        });
    }

    /// <summary>One pass of the walk 1DEF:04A9 with its frames drained (see <see cref="PassFrames"/>).</summary>
    public static void Pass(Dgroup d, OriginalRandom random, IExtraHost host, ExtraBoard board)
    {
        foreach (var _ in PassFrames(d, random, host, board)) { }
    }

    /// <summary>1DEF:04A9: passes while fewer than 25 goods are picked up (DS:5480, signed), then the last frame 21FD:0665.</summary>
    // [ORIGINAL] 1DEF:04A9
    // [ORIGINAL] 21FD:0665 – 21FD:051F, 23CD:071C, 0A96:06A8, 23CD:06E9
    public static IEnumerable<FieldWait> WalkFrames(Dgroup d, OriginalRandom random, IExtraHost host, ExtraBoard board)
    {
        while (Running(d))
            foreach (var w in PassFrames(d, random, host, board)) yield return w;
        Emit(host, FieldEventKind.LastFrame);
        foreach (var w in Planting.Flip(d)) yield return w;
    }

    /// <summary>True while the walk 1DEF:04A9 runs another pass (DS:5480 &lt; 25, signed).</summary>
    public static bool Running(Dgroup d) => d.S16(Collected) < Pieces;

    /// <summary>
    /// One pass of 1DEF:04A9: flip, restore, list, 3 retraces, controls, pause; then per player a running move goes on
    /// (0609:1FE0), otherwise the goods on the cell are picked up (0651), a computer chooses its direction (07AD) and the action
    /// DS:3308 is taken: none (−1) → idle blink (0609:215E); right, left, up, down → a move of 11h (horizontal) or 0Ch
    /// (vertical) frames starts unless the board ends there; other actions do nothing. List and objects.
    /// </summary>
    public static IEnumerable<FieldWait> PassFrames(Dgroup d, OriginalRandom random, IExtraHost host, ExtraBoard board)
    {
        var field = new FieldOutput(host);
        Emit(host, FieldEventKind.FlipPages);
        foreach (var w in Planting.Flip(d)) yield return w;
        Emit(host, FieldEventKind.RestoreObjects);
        Emit(host, FieldEventKind.RunList);
        Emit(host, FieldEventKind.ClearList);
        Emit(host, FieldEventKind.Delay, 3, 0);
        foreach (var w in Planting.Retraces(3, 0)) yield return w;
        host.ReadControls(d);
        foreach (var w in host.PauseCheck(d)) yield return w;
        ForPlayers(d, p =>
        {
            if (d.S16(At(FieldCursor.MoveTimer + 2 * p)) != 0)
            {
                Planting.Move(d, field);
                return;
            }
            Collect(d, host, board);
            if (d.U8(At(DgroupLayout.IsComputer + P(d))) == 1) Aim(d, board);
            Act(d, random);
        });
        Emit(host, FieldEventKind.RunList);
        Emit(host, FieldEventKind.DrawObjects);
    }

    /// <summary>The action switch of 1DEF:04A9 (jump table 1DEF:0647 over action + 1, unsigned ≤ 4).</summary>
    private static void Act(Dgroup d, OriginalRandom random)
    {
        int p = P(d);
        ushort index = Int16Math.U(d.S8(At(FieldCursor.Action + p)) + 1);
        switch (index)
        {
            case 0: FieldCursor.Idle(d, random); break;
            case 1: Start(d, p, FieldCursor.CursorX, 5, 0, 0x11, +1); break;
            case 2: Start(d, p, FieldCursor.CursorX, 0, 1, 0x11, -1); break;
            case 3: Start(d, p, FieldCursor.CursorY, 0, 2, 0x0C, -1); break;
            case 4: Start(d, p, FieldCursor.CursorY, 6, 3, 0x0C, +1); break;
        }
    }

    /// <summary>A move in one direction: not at the edge <paramref name="edge"/> – direction, move timer, cursor ± 1 (bytes).</summary>
    private static void Start(Dgroup d, int p, int cursor, byte edge, short direction, short frames, int step)
    {
        if (d.U8(At(cursor + p)) == edge) return;
        d.Set(At(FieldCursor.Direction + 2 * p), direction);
        d.Set(At(FieldCursor.MoveTimer + 2 * p), frames);
        d.Set(At(cursor + p), unchecked((byte)(d.U8(At(cursor + p)) + step)));
    }

    /// <summary>
    /// 1DEF:0651: the goods on the player's cell (row·6 + column, signed bytes; loop variable DS:45D0 = kind): if any, one more
    /// is picked up (DS:5480), the gain of that kind grows by 2·DS:2F26 (signed byte, 16-bit sum) and is shown (06F8), the cell
    /// is emptied and redrawn (0308), sound (0, 31h).
    /// </summary>
    // [ORIGINAL] 1DEF:0651
    public static void Collect(Dgroup d, IExtraHost host, ExtraBoard board)
    {
        int p = P(d);
        short cell = Int16Math.S(Int16Math.S(d.S8(At(FieldCursor.CursorY + p)) * 6) + d.S8(At(FieldCursor.CursorX + p)));
        d.Set(DgroupLayout.LoopWare, board.Goods[Cell(cell)]);
        if (d.S16(DgroupLayout.LoopWare) == -1) return;
        d.Set(Collected, Int16Math.S(d.S16(Collected) + 1));
        int gain = GainOf(P(d), d.S16(DgroupLayout.LoopWare));
        d.Set(gain, Int16Math.S(d.S16(gain) + Int16Math.S(d.S8(DgroupLayout.LimitLow) << 1)));
        ShowGain(d, host);
        board.Goods[Cell(cell)] = -1;
        DrawCell(d, host, board, cell);
        Emit(host, FieldEventKind.Sound, 0, 0x31);
    }

    /// <summary>
    /// 1DEF:06F8: the gain of kind DS:45D0 as three digits at (22h, 2Ch·player + 8·kind + 18h) in the player's colour DS:00C8,
    /// background 5.
    /// </summary>
    // [ORIGINAL] 1DEF:06F8
    public static void ShowGain(Dgroup d, IExtraHost host)
    {
        int p = P(d);
        short kind = d.S16(DgroupLayout.LoopWare);
        short value = d.S16(GainOf(p, kind));
        Emit(host, ExtraEventKind.Number, 0x22, Int16Math.S(Int16Math.S(p * 0x2C) + Int16Math.S(kind << 3) + 0x18), value,
            (short)(value < 0 ? -1 : 0), 3, d.S16(At(ColourOfPlayer + 2 * p)), 5);
    }

    /// <summary>
    /// 1DEF:07AD – the computer's direction: among the cells with goods that nobody else claimed, the one with the smallest
    /// distance |column − own column| + |row − own row| (8-bit arithmetic, signed compare, start value 100; the last of equal
    /// distance wins, none found → cell 0); the action DS:3308 is left (1) or right (0) while the column differs, else up (2)
    /// while the target row is smaller, else down (3); the cell is claimed for the player.
    /// </summary>
    // [ORIGINAL] 1DEF:07AD
    public static void Aim(Dgroup d, ExtraBoard board)
    {
        int p = P(d);
        sbyte best = 100, target = 0;
        short column = d.S8(At(FieldCursor.CursorX + p)), row = d.S8(At(FieldCursor.CursorY + p));
        byte player = d.U8(FieldGrid.CurrentPlayer);
        for (short cx = 0; cx <= 0x29; cx++)
        {
            if (board.Goods[cx] == -1) continue;
            byte claim = board.Claims[cx];
            if (claim != 0xFF && claim != player) continue;
            byte c = unchecked((byte)Int16Math.Mod(cx, 6)), r = unchecked((byte)Int16Math.Div(cx, 6));
            byte dc = (sbyte)c < column ? unchecked((byte)(column - c)) : unchecked((byte)(c - column));
            byte dr = (sbyte)r < row ? unchecked((byte)(row - r)) : unchecked((byte)(r - row));
            sbyte distance = unchecked((sbyte)(dc + dr));
            if (distance > best) continue;
            best = distance;
            target = unchecked((sbyte)cx);
        }
        sbyte tc = (sbyte)Int16Math.Mod(target, 6), tr = (sbyte)Int16Math.Div(target, 6);
        sbyte action = tc < column ? (sbyte)1 : tc > column ? (sbyte)0 : tr < row ? (sbyte)2 : (sbyte)3;
        d.Set(At(FieldCursor.Action + p), action);
        board.Claims[Cell(target)] = player;
    }

    /// <summary>1DEF:0751: every player's gains of the four kinds are added to the stocks DS:3CF4 (16-bit, no upper bound).</summary>
    // [ORIGINAL] 1DEF:0751
    public static void Credit(Dgroup d)
    {
        ForPlayers(d, _ => ForKinds(d, k =>
        {
            int stock = At(DgroupLayout.Stock + Int16Math.S((P(d) << 3) + Int16Math.S(k << 1)));
            d.Set(stock, Int16Math.S(d.S16(stock) + d.S16(GainOf(P(d), k))));
        }));
    }

    /// <summary>A cell index of the stack arrays; the original would read outside its frame for others.</summary>
    private static int Cell(int index)
    {
        // [APPROX] Indices outside 0 … 29h address the stack around the frame in the original; they do not occur with the
        // cursors 0…5/0…6 that 03CD and the moves keep, so the port refuses them instead of modelling the stack.
        if ((uint)index >= ExtraBoard.Cells) throw new InvalidOperationException($"1DEF: cell {index} outside the board");
        return index;
    }

    /// <summary>The field layer's outputs of the shared routines (cursor objects, palette, border, moves) go to the host.</summary>
    private sealed class FieldOutput(IExtraHost host) : IFieldOutput
    {
        public void ComputerTurn(Dgroup d, short x, short y) =>
            throw new InvalidOperationException("The extra phase has no computer turn of the field.");

        public void Emit(FieldEvent e) => host.Emit(e);
    }

    /// <summary>
    /// The typewriter 0A69:0288 of lane ernte (<see cref="Traders.Core.Harvest.Harvest.TypeLines"/>) on the phase's host: its
    /// lines and characters become <see cref="ExtraEventKind.TypeLines"/>/<see cref="ExtraEventKind.TypeCharacter"/>.
    /// </summary>
    private sealed class Typewriter(IExtraHost host) : IHarvestHost
    {
        public void Emit(FieldEvent e) => host.Emit(e);

        public void Emit(HarvestEvent e) => host.Emit(e.Kind switch
        {
            HarvestEventKind.TypeLines => new ExtraEvent(ExtraEventKind.TypeLines, e.Args),
            HarvestEventKind.TypeCharacter => new ExtraEvent(ExtraEventKind.TypeCharacter, e.Args),
            _ => throw new InvalidOperationException($"typewriter event {e.Kind} is not part of the extra phase"),
        });

        public void ReadControls(Dgroup d) => throw new InvalidOperationException("The typewriter reads no controls.");

        public IEnumerable<FieldWait> PauseCheck(Dgroup d) => host.PauseCheck(d);

        public byte[] FarString(ushort offset, ushort segment) => host.FarString(offset, segment);
    }
}
