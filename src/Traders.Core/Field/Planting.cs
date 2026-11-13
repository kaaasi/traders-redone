using Traders.Core.Rules;
using Traders.Core.State;

namespace Traders.Core.Field;

/// <summary>
/// What the planting phase needs from outside the core's field layer: the helpers and computer hooks of lane anbau-ki
/// (0609:074D, 0609:100F, 135D:0AB3, 135D:0935), the controls (0E9E:037A), the pause check (21FD:0245) and the wait loop of
/// lane bilanz (21FD:0004). Until those lanes are merged the tests answer them from the RefEmu.
/// </summary>
public interface IPlantingHost : IFieldOutput
{
    /// <summary>0609:074D – AL ≠ 0 if an HQ may be built on the field (DS:2EB0, DS:2F00).</summary>
    bool CanPlaceHq(Dgroup d);

    /// <summary>0609:100F(x, y) – AL = 1 if a neighbour field belongs to the player at turn.</summary>
    bool HasOwnNeighbour(Dgroup d, short x, short y);

    /// <summary>135D:0AB3 – the computer at turn has built its HQ.</summary>
    void ComputerPlacedHq(Dgroup d);

    /// <summary>135D:0935 – the computer at turn has bought a free field.</summary>
    void ComputerBoughtField(Dgroup d);

    /// <summary>0E9E:037A – reads the controls and writes the actions DS:3308 (ControlReader.ReadActions).</summary>
    void ReadControls(Dgroup d);

    /// <summary>21FD:0245 – pause key check (waits while paused; no game state).</summary>
    void PauseCheck(Dgroup d);

    /// <summary>21FD:0004(mode) – wait loop of lane bilanz (frames, random draws).</summary>
    void WaitLoop(Dgroup d, OriginalRandom random, short mode);
}

/// <summary>
/// Planting phase 0609:0004 (finding docs/routines/0609_0004_anbau.md): preparation of the players, the field set-up
/// 0609:00D6, the field loop 0609:03E0 one pass at a time, the field actions 0609:064D with HQ, selling and planting, the cursor
/// movement 0609:1FE0 and the settlement of sold fields 0609:0FA9. Works on the data segment like the original; drawing and
/// sound are emitted as <see cref="FieldEvent"/>s in the original's order, the random draws of the border (0609:13C5) are
/// done here.
/// </summary>
public static class Planting
{
    // DS offsets (docs/analyse/DATENLAYOUT.md, section of lane anbau).
    public const int NoTime = FieldCursor.NoTime;        // byte, 1 = second field round of 0609:0080 (moves cost no time)
    public const int Mode = FieldCursor.Mode;            // byte[4]: 2 build HQ, 3 move, 4 choose kind, 5 free field, 6 done
    public const int Purchase = 0x43FA;                  // byte[4], 1 = may take one free field this round
    public const int SoldX = 0x3CAF;                     // sbyte[4], field marked for sale (−1 = none)
    public const int SoldY = 0x3CB3;                     // sbyte[4]
    public const int PlantCost = DgroupLayout.LimitHigh; // sbyte, stock units per planted field (0609:1178)
    public const int Players = FieldCursor.Finished;     // byte, players still on the field
    public const int Objects = FieldCursor.Sprite;       // object records of 1Ah bytes (core part: +0, +2, +14h, +16h, +18h)
    public const int ObjectSize = 0x1A;
    public const int ObjectX = Objects + 0x14, ObjectY = Objects + 0x16, ObjectFrame = Objects + 0x18, ObjectLayer = Objects + 2;
    public const int PictureNumber = 0x08CE;             // word, picture of the field (V-ANBAU)

    private static int P(Dgroup d) => d.S8(FieldGrid.CurrentPlayer);
    private static int At(int offset) => Int16Math.U(offset);
    private static int Cell(int x, int y) => Int16Math.U(Int16Math.S(x * 6) + y);
    private static int Obj(int p) => Int16Math.U(Int16Math.S(p * ObjectSize));

    private static void Emit(IFieldOutput o, FieldEventKind k, params short[] args) => o.Emit(new FieldEvent(k, args));
    private static void Sound(IFieldOutput o, short a, short b) => Emit(o, FieldEventKind.Sound, a, b);

    private static void ForPlayers(Dgroup d, Action<int> body)
    {
        for (d.Set(FieldGrid.CurrentPlayer, (sbyte)0); d.S8(FieldGrid.CurrentPlayer) < 4;
             d.Set(FieldGrid.CurrentPlayer, unchecked((sbyte)(d.S8(FieldGrid.CurrentPlayer) + 1))))
            body(P(d));
    }

    /// <summary>0609:0004 up to the call of 0609:00D6: objects off, mode 2 (no HQ yet) or 3, purchase flag, DS:1582 = 0.</summary>
    // [ORIGINAL] 0609:0004
    public static void Prepare(Dgroup d, IFieldOutput output)
    {
        InitObjects(d, output);
        ForPlayers(d, p =>
        {
            d.Set(At(Mode + p), (byte)(d.S8(At(DgroupLayout.HqX + p)) == -1 ? 2 : 3));
            bool free = d.U8(At(DgroupLayout.FatMikeFlag + p)) == 0 && d.S8(FieldGrid.FreeFields) > 0;
            d.Set(At(Purchase + p), (byte)(free ? 1 : 0));
        });
        d.Set(NoTime, (byte)0);
    }

    /// <summary>21AF:000D, core part: all eight objects inactive (+0 = 0); tables and restore records are the output's.</summary>
    // [ORIGINAL] 21AF:000D – core part only (object records +0)
    public static void InitObjects(Dgroup d, IFieldOutput output)
    {
        Emit(output, FieldEventKind.InitObjects);
        for (int i = 0; i < 8; i++) d.Set(At(Objects + i * ObjectSize), (byte)0);
    }

    /// <summary>
    /// 0609:00D6 up to the field loop: picture, objects, palette, ranks, start positions, the board with its border draws,
    /// status and panels, cursor objects, the wait loop when DS:1582 = 0, an empty list.
    /// </summary>
    // [ORIGINAL] 0609:00D6
    public static void Setup(Dgroup d, OriginalRandom random, IPlantingHost host)
    {
        Emit(host, FieldEventKind.LoadImage, d.S16(PictureNumber));
        CursorObjects(d, host);
        Emit(host, FieldEventKind.FadeOut, 0x666);
        Emit(host, FieldEventKind.BasePalette, 0xA8);
        WeatherColours(d, host);
        Ranking.Apply(d);
        ForPlayers(d, p => d.Set(At(SoldX + p), (sbyte)-1));
        StartPositions(d);
        Board(d, random, host);
        Emit(host, FieldEventKind.FadeIn);
        Emit(host, FieldEventKind.DrawStatus);
        Panels(d, host);
        PlaceCursors(d, host);
        if (d.U8(NoTime) == 0) host.WaitLoop(d, random, 1);
        Emit(host, FieldEventKind.ClearList);
    }

    /// <summary>0609:1F2D: per player object layer +2 = 0Ah, restore records, save buffers and the nine cursor frames.</summary>
    // [ORIGINAL] 0609:1F2D
    public static void CursorObjects(Dgroup d, IFieldOutput output)
    {
        ForPlayers(d, p =>
        {
            d.Set(At(ObjectLayer + Obj(p)), (short)0x0A);
            Emit(output, FieldEventKind.ResetObjectRestore);
            short row = Int16Math.S(p * 10);
            Emit(output, FieldEventKind.ObjectBuffers, (short)p, 0x120, row, 0x120, Int16Math.S(row + 0x28));
            for (short f = 0; f < 9; f++)
                Emit(output, FieldEventKind.ObjectImage, (short)p, f, Int16Math.S((f << 4) + 0x30), Int16Math.S(row + 0xC6),
                    (short)(f << 4), row);
        });
    }

    /// <summary>0609:11E7: three colours of the base palette for weather 3 (764h/642h/420h) and 5 (666h/555h/333h).</summary>
    // [ORIGINAL] 0609:11E7
    public static void WeatherColours(Dgroup d, IFieldOutput output)
    {
        short[]? colours = d.U8(DgroupLayout.Weather) switch
        {
            3 => [0x764, 0x642, 0x420],
            5 => [0x666, 0x555, 0x333],
            _ => null,
        };
        if (colours is null) return;
        Emit(output, FieldEventKind.PaletteColour, 1, colours[0]);
        Emit(output, FieldEventKind.PaletteColour, 3, colours[1]);
        Emit(output, FieldEventKind.PaletteColour, 4, colours[2]);
    }

    /// <summary>
    /// 0609:0160: cursors of players still playing start on their HQ, without an HQ in the corner of the player
    /// (x = 0 for even players, columns − 1 for odd; y = 0 for players 0 and 1, rows − 1 for 2 and 3; 8-bit arithmetic).
    /// </summary>
    // [ORIGINAL] 0609:0160
    public static void StartPositions(Dgroup d)
    {
        ForPlayers(d, p =>
        {
            if (d.U8(At(Mode + p)) == 6) return;
            if (d.S8(At(DgroupLayout.HqX + p)) == -1)
            {
                byte x = p % 2 == 0 ? (byte)0 : unchecked((byte)(d.U8(FieldGrid.Columns) - 1));
                byte y = p > 1 ? unchecked((byte)(d.U8(FieldGrid.Rows) - 1)) : (byte)0;
                d.Set(At(FieldCursor.CursorX + p), x);
                d.Set(At(FieldCursor.CursorY + p), y);
            }
            else
            {
                d.Set(At(FieldCursor.CursorX + p), d.U8(At(DgroupLayout.HqX + p)));
                d.Set(At(FieldCursor.CursorY + p), d.U8(At(DgroupLayout.HqY + p)));
            }
        });
    }

    /// <summary>
    /// 0609:1BE5: background, frame, the field border 0609:13C5 with its random draws, then every field with the loop
    /// variables DS:2F00 (row, outer) and DS:2EB0 (column, inner), then the page is shown.
    /// </summary>
    // [ORIGINAL] 0609:1BE5
    public static void Board(Dgroup d, OriginalRandom random, IFieldOutput output)
    {
        Emit(output, FieldEventKind.ClearList);
        Emit(output, FieldEventKind.DrawBackground);
        Emit(output, FieldEventKind.DrawFrame, 0, 0, 0x140, 0x36);
        Border(d.S16(MenuOrigin.X), d.S16(MenuOrigin.Y), Int16Math.S(d.S16(FieldGrid.Columns) << 5),
            Int16Math.S(d.S16(FieldGrid.Rows) * 0x16), random, output);
        Emit(output, FieldEventKind.RunList);
        for (d.Set(DgroupLayout.LoopY, (sbyte)0); d.S8(DgroupLayout.LoopY) <= Int16Math.S(d.S16(FieldGrid.Rows) - 1);
             d.Set(DgroupLayout.LoopY, unchecked((sbyte)(d.S8(DgroupLayout.LoopY) + 1))))
            for (d.Set(DgroupLayout.LoopX, (sbyte)0); d.S8(DgroupLayout.LoopX) <= Int16Math.S(d.S16(FieldGrid.Columns) - 1);
                 d.Set(DgroupLayout.LoopX, unchecked((sbyte)(d.S8(DgroupLayout.LoopX) + 1))))
            {
                Emit(output, FieldEventKind.ClearList);
                Emit(output, FieldEventKind.DrawField, d.S8(DgroupLayout.LoopX), d.S8(DgroupLayout.LoopY));
                Emit(output, FieldEventKind.RunList);
            }
        Emit(output, FieldEventKind.ShowPage);
    }

    /// <summary>
    /// 0609:13C5(x, y, w, h), core part: per 20h-wide column of the border (i = 0 … w/20h − 1, signed) one random(1) for the
    /// upper and one for the lower edge. random(1) is always 0, so the drawing does not depend on the draws (R5P-3).
    /// </summary>
    // [ORIGINAL] 0609:13C5 – core part (random draws); the drawing part is the scene's
    public static void Border(short x, short y, short w, short h, OriginalRandom random, IFieldOutput output)
    {
        Emit(output, FieldEventKind.DrawBorder, x, y, w, h);
        for (short i = 0; Int16Math.S(Int16Math.Div(w, 0x20) - 1) >= i; i++)
        {
            random.Next(1);
            random.Next(1);
        }
    }

    /// <summary>0609:1EA3: emblem and panel of every player still playing, then shown on both pages.</summary>
    // [ORIGINAL] 0609:1EA3
    public static void Panels(Dgroup d, IFieldOutput output)
    {
        Emit(output, FieldEventKind.ClearList);
        ForPlayers(d, p =>
        {
            if (d.U8(At(Mode + p)) == 6) return;
            Emit(output, FieldEventKind.DrawPlayer);
            Emit(output, FieldEventKind.DrawPanel);
        });
        Emit(output, FieldEventKind.ListOnBothPages);
    }

    /// <summary>
    /// 0609:032A: counts the players still playing (DS:3CE6), clears the move delay of all and, for those playing, the move
    /// timer and places the cursor object on the cursor field (x·20h + origin + 8, y·16h + origin + 0Ch); in the second
    /// round (DS:1582 = 1) the cursor object is shown at once (0609:1FCC).
    /// </summary>
    // [ORIGINAL] 0609:032A
    public static void PlaceCursors(Dgroup d, IFieldOutput output)
    {
        d.Set(Players, (byte)0);
        ForPlayers(d, p =>
        {
            d.Set(At(FieldCursor.MoveDelay + 4 * p), 0);
            if (d.U8(At(Mode + p)) == 6) return;
            d.Set(At(FieldCursor.MoveTimer + 2 * p), (short)0);
            short x = Int16Math.S((d.S8(At(FieldCursor.CursorX + p)) << 5) + d.S16(MenuOrigin.X) + 8);
            d.Set(At(ObjectX + Obj(p)), x);
            short y = Int16Math.S(Int16Math.S(d.S8(At(FieldCursor.CursorY + p)) * 0x16) + d.S16(MenuOrigin.Y) + 0x0C);
            d.Set(At(ObjectY + Obj(p)), y);
            d.Set(Players, unchecked((byte)(d.U8(Players) + 1)));
            if (d.U8(NoTime) == 1) ShowCursor(d, output);
        });
    }

    /// <summary>0609:1FCC: 21AF:0412(player, 0) – core part: frame 0 and active; the restore records are the output's.</summary>
    // [ORIGINAL] 0609:1FCC
    // [ORIGINAL] 21AF:0412 – core part only (object records +0, +18h)
    public static void ShowCursor(Dgroup d, IFieldOutput output)
    {
        int p = P(d);
        Emit(output, FieldEventKind.ShowObject, (short)p, 0);
        d.Set(At(ObjectFrame + Obj(p)), (short)0);
        d.Set(At(Objects + Obj(p)), (byte)1);
    }

    /// <summary>
    /// One pass of the field loop 0609:03E0 (the loop runs while DS:3CE6 ≠ 0): flip, restore, list, the delay of
    /// 5 − DS:2E8E retraces, controls, pause, then every player still playing moves (0609:1FE0) or takes a cursor step
    /// (0609:047B) and its action (0609:064D); list and objects.
    /// </summary>
    // [ORIGINAL] 0609:03E0
    public static void Pass(Dgroup d, OriginalRandom random, IPlantingHost host)
    {
        Emit(host, FieldEventKind.FlipPages);
        Emit(host, FieldEventKind.RestoreObjects);
        Emit(host, FieldEventKind.RunList);
        Emit(host, FieldEventKind.ClearList);
        int delay = 5 - d.S16(DgroupLayout.ComputerIq);
        Emit(host, FieldEventKind.Delay, Int16Math.S(delay), Int16Math.S(delay >> 16));
        host.ReadControls(d);
        host.PauseCheck(d);
        ForPlayers(d, p =>
        {
            if (d.U8(At(Mode + p)) == 6) return;
            if (d.S16(At(FieldCursor.MoveTimer + 2 * p)) != 0) Move(d, host);
            else
            {
                FieldCursor.Step(d, random, host);
                Action(d, host);
            }
        });
        Emit(host, FieldEventKind.RunList);
        Emit(host, FieldEventKind.DrawObjects);
    }

    /// <summary>True while the field loop 0609:03E0 runs another pass (DS:3CE6 ≠ 0).</summary>
    public static bool Running(Dgroup d) => d.U8(Players) != 0;

    /// <summary>
    /// 0609:1FE0: one frame of a running move – the timer counts down; while it runs the object moves 2 pixels in the
    /// direction DS:4BCA with the two frames of that direction alternating, the start frame DS:4BD2 counts down and redraws
    /// the panel when it reaches 0; at the end the frame is 0. Each frame plays sound (2, 0Fh).
    /// </summary>
    // [ORIGINAL] 0609:1FE0
    public static void Move(Dgroup d, IFieldOutput output)
    {
        int p = P(d);
        int obj = Obj(p);
        short frame = d.S16(At(ObjectFrame + obj));
        int timer = At(FieldCursor.MoveTimer + 2 * p);
        d.Set(timer, Int16Math.S(d.S16(timer) - 1));
        short next;
        if (d.S16(timer) == 0) next = 0;
        else
        {
            switch (d.U16(At(FieldCursor.Direction + 2 * p)))
            {
                case 0: next = Step(d, ObjectX + obj, frame, 7, +2); break;
                case 1: next = Step(d, ObjectX + obj, frame, 5, -2); break;
                case 2: next = Step(d, ObjectY + obj, frame, 3, -2); break;
                case 3: next = Step(d, ObjectY + obj, frame, 1, +2); break;
                default:
                    // [APPROX] direction > 3 is never written (0609:047B sets 0..3); the original then stores SI as it came
                    // from the caller, which the port cannot know.
                    throw new InvalidOperationException($"0609:1FE0: direction {d.U16(At(FieldCursor.Direction + 2 * p))}");
            }
            int start = At(FieldCursor.MoveFrame + 2 * p);
            if (d.S16(start) > 0)
            {
                d.Set(start, Int16Math.S(d.S16(start) - 1));
                if (d.S16(start) == 0) Emit(output, FieldEventKind.DrawPanel);
            }
        }
        d.Set(At(ObjectFrame + Obj(P(d))), next);
        Sound(output, 2, 0x0F);
    }

    private static short Step(Dgroup d, int coordinate, short frame, short first, int delta)
    {
        int at = At(coordinate);
        d.Set(at, Int16Math.S(d.S16(at) + delta));
        return frame == first ? (short)(first + 1) : first;
    }

    /// <summary>0609:064D: with action 0Ah (fire) the handler of the mode: 2 → 069F, 3 → 07D6, 4 → 096A, 5 → 0AEF.</summary>
    // [ORIGINAL] 0609:064D
    public static void Action(Dgroup d, IPlantingHost host)
    {
        int p = P(d);
        if (d.S8(At(FieldCursor.Action + p)) != 0x0A) return;
        switch (d.U8(At(Mode + p)))
        {
            case 2: BuildHq(d, host); break;
            case 3: FieldAction(d, host); break;
            case 4: Plant(d, host); break;
            case 5: TakeFreeField(d, host); break;
        }
    }

    /// <summary>
    /// 0609:069F: HQ on the cursor field if 0609:074D allows it (DS:2EB0/2F00 = cursor): field kind 4, HQ position, field
    /// redrawn, sound (0, 7), mode 3; a computer is told (135D:0AB3).
    /// </summary>
    // [ORIGINAL] 0609:069F
    public static void BuildHq(Dgroup d, IPlantingHost host)
    {
        int p = P(d);
        d.Set(DgroupLayout.LoopX, d.U8(At(FieldCursor.CursorX + p)));
        d.Set(DgroupLayout.LoopY, d.U8(At(FieldCursor.CursorY + p)));
        if (!host.CanPlaceHq(d)) return;
        p = P(d);
        FieldGrid.Set(d, d.S8(DgroupLayout.LoopX), d.S8(DgroupLayout.LoopY), (short)p, 4);
        p = P(d);
        d.Set(At(DgroupLayout.HqX + p), d.U8(At(FieldCursor.CursorX + p)));
        d.Set(At(DgroupLayout.HqY + p), d.U8(At(FieldCursor.CursorY + p)));
        Emit(host, FieldEventKind.DrawField, d.S8(DgroupLayout.LoopX), d.S8(DgroupLayout.LoopY));
        Sound(host, 0, 7);
        p = P(d);
        d.Set(At(Mode + p), (byte)3);
        if (d.U8(At(DgroupLayout.IsComputer + p)) == 1) host.ComputerPlacedHq(d);
    }

    /// <summary>
    /// 0609:07D6 on the cursor field: own field – on the HQ the player ends (0EDD), sound (0, 8), rank, and cash grows by
    /// cash / 50 (LDIV); elsewhere mode 4 with no kind chosen (a sale mark on this field is taken back), sound (0, 9),
    /// selection drawn. Foreign field – a free field next to an own one is taken with its kind if the purchase flag is set:
    /// sound (0, 0Ah), field and neighbours drawn, flag cleared, marker drawn, a computer is told (135D:0935).
    /// </summary>
    // [ORIGINAL] 0609:07D6
    public static void FieldAction(Dgroup d, IPlantingHost host)
    {
        int p = P(d);
        short x = d.S8(At(FieldCursor.CursorX + p)), y = d.S8(At(FieldCursor.CursorY + p));
        int cell = Cell(x, y);
        if (d.U8(At(FieldGrid.Owner + cell)) == d.U8(FieldGrid.CurrentPlayer))
        {
            if (d.S8(At(DgroupLayout.HqX + p)) == x && d.S8(At(DgroupLayout.HqY + p)) == y)
            {
                FieldCursor.OutOfTime(d, host);
                Sound(host, 0, 8);
                p = P(d);
                Emit(host, FieldEventKind.DrawRank, Int16Math.S((p << 6) + 0x20), 6);
                int cash = At(DgroupLayout.Cash + 4 * P(d));
                d.Set(cash, d.S32(cash) + LongMath.Div(d.S32(cash), 50));
                return;
            }
            d.Set(At(Mode + p), (byte)4);
            d.Set(At(FieldCursor.Selection + P(d)), (sbyte)-1);
            p = P(d);
            if (d.S8(At(SoldX + p)) == x && d.S8(At(SoldY + p)) == y)
            {
                d.Set(At(SoldX + p), (sbyte)-1);
                Emit(host, FieldEventKind.DrawField, x, y);
            }
            Sound(host, 0, 9);
            Emit(host, FieldEventKind.DrawSelection);
            return;
        }
        if (!host.HasOwnNeighbour(d, x, y)) return;
        if (d.S8(At(FieldGrid.Owner + cell)) != -1) return;
        if (d.U8(At(Purchase + P(d))) != 1) return;
        FieldGrid.Set(d, x, y, d.S8(FieldGrid.CurrentPlayer), d.S8(At(FieldGrid.Kind + cell)));
        Sound(host, 0, 0x0A);
        Emit(host, FieldEventKind.DrawFieldAndNeighbours, x, y);
        d.Set(At(Purchase + P(d)), (byte)0);
        Emit(host, FieldEventKind.DrawPurchaseUsed);
        if (d.U8(At(DgroupLayout.IsComputer + P(d))) == 1) host.ComputerBoughtField(d);
    }

    /// <summary>
    /// 0609:096A with the chosen kind DS:2E94: none (−1) – sound (0, 0Bh); 4 – the cursor field is marked for sale (the old
    /// mark and the new field are redrawn); 3 (rocket base) – costs one rocket, not on terrain 3; 0–2 – costs DS:48A2 units
    /// of ware (kind + 1) mod 3, nothing without enough stock. A planted field gets the kind, is drawn, sound (0, 0Ch). Except
    /// when the cost cannot be paid: panel drawn and mode 3.
    /// </summary>
    // [ORIGINAL] 0609:096A
    public static void Plant(Dgroup d, IPlantingHost host)
    {
        int p = P(d);
        short x = d.S8(At(FieldCursor.CursorX + p)), y = d.S8(At(FieldCursor.CursorY + p));
        short kind = d.S8(At(FieldCursor.Selection + p));
        if (kind == -1) Sound(host, 0, 0x0B);
        else if (kind == 4)
        {
            short oldX = d.S8(At(SoldX + p)), oldY = d.S8(At(SoldY + p));
            d.Set(At(SoldX + p), unchecked((byte)x));
            d.Set(At(SoldY + P(d)), unchecked((byte)y));
            Emit(host, FieldEventKind.DrawField, x, y);
            if (oldX != -1) Emit(host, FieldEventKind.DrawField, oldX, oldY);
        }
        else
        {
            if (kind == 3)
            {
                int rockets = At(DgroupLayout.Rockets + 2 * p);
                if (d.S16(rockets) == 0) return;
                if (d.U8(At(FieldCursor.Terrain + Cell(x, y))) == 3) return;
                d.Set(rockets, Int16Math.S(d.S16(rockets) - 1));
            }
            else
            {
                int ware = Int16Math.S(kind + 1);
                if (ware == 3) ware = 0;
                int stock = At(Int16Math.S((p << 3) + (ware << 1)) + DgroupLayout.Stock);
                short cost = d.S8(PlantCost);
                if (d.S16(stock) < cost) return;
                d.Set(stock, Int16Math.S(d.S16(stock) - cost));
            }
            p = P(d);
            FieldGrid.Set(d, x, y, (short)p, d.S8(At(FieldCursor.Selection + p)));
            Emit(host, FieldEventKind.DrawField, x, y);
            Sound(host, 0, 0x0C);
        }
        Emit(host, FieldEventKind.DrawPanel);
        d.Set(At(Mode + P(d)), (byte)3);
    }

    /// <summary>
    /// 0609:0AEF (second round 0609:0080): on the own HQ the player ends; a free field is taken with its kind (sound (0, 0Dh),
    /// field and neighbours drawn, purchase flag cleared) and the player ends; on any other field nothing happens.
    /// </summary>
    // [ORIGINAL] 0609:0AEF
    public static void TakeFreeField(Dgroup d, IFieldOutput output)
    {
        int p = P(d);
        short x = d.S8(At(FieldCursor.CursorX + p)), y = d.S8(At(FieldCursor.CursorY + p));
        if (d.S8(At(DgroupLayout.HqX + p)) == x && d.S8(At(DgroupLayout.HqY + p)) == y)
        {
            FieldCursor.OutOfTime(d, output);
            return;
        }
        int cell = Cell(x, y);
        if (d.S8(At(FieldGrid.Owner + cell)) != -1) return;
        Sound(output, 0, 0x0D);
        FieldGrid.Set(d, x, y, d.S8(FieldGrid.CurrentPlayer), d.S8(At(FieldGrid.Kind + cell)));
        Emit(output, FieldEventKind.DrawFieldAndNeighbours, x, y);
        d.Set(At(Purchase + P(d)), (byte)0);
        FieldCursor.OutOfTime(d, output);
    }

    /// <summary>0609:0FA9: every field marked for sale becomes free and empty (−1, −2) and pays the land price DS:3746.</summary>
    // [ORIGINAL] 0609:0FA9
    public static void Settle(Dgroup d)
    {
        ForPlayers(d, p =>
        {
            if (d.S8(At(SoldX + p)) == -1) return;
            FieldGrid.Set(d, d.S8(At(SoldX + p)), d.S8(At(SoldY + p)), -1, -2);
            int cash = At(DgroupLayout.Cash + 4 * P(d));
            d.Set(cash, d.S32(cash) + d.S16(DgroupLayout.LandPrice));
        });
    }
}

/// <summary>Field origin DS:451E/4528 (set by the new game 192D:0058).</summary>
internal static class MenuOrigin
{
    public const int X = 0x451E, Y = 0x4528;
}
