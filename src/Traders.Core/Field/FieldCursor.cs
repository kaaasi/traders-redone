using Traders.Core.State;

namespace Traders.Core.Field;

/// <summary>What the cursor step hands to the outside: drawing and sound requests and the computer's turn.</summary>
public enum FieldEventKind
{
    /// <summary>0609:0C73 – redraw the selected kind of the player (no arguments; reads DS:2E94 and others).</summary>
    DrawSelection,
    /// <summary>0609:1854(x, y) – draw the player's rank icon (reads DS:3D42).</summary>
    DrawRank,
    /// <summary>0A96:0030 with six arguments from 0609:1B72 – one step of the time bar.</summary>
    DrawCommand,
    /// <summary>21A7:0054(a, b) – sound request to the sound driver.</summary>
    Sound,

    // Planting phase 0609:0004 (finding docs/routines/0609_0004_anbau.md). Drawing routines of segment 0609 and the
    // output layers called by the phase, in the original's order; none of them changes game state.

    /// <summary>186C:0002(n) – load picture n (the step handler records it with ImageTable.RecordLoad).</summary>
    LoadImage,
    /// <summary>21AF:000D – object tables (the core part, all objects inactive, is done by the core).</summary>
    InitObjects,
    /// <summary>21AF:04CD – restore records of all objects cleared.</summary>
    ResetObjectRestore,
    /// <summary>21AF:022B(object, x1, y1, x2, y2) – save buffers of an object.</summary>
    ObjectBuffers,
    /// <summary>21AF:00CA(object, frame, x, y, mx, my) – image and mask pointer of an object frame.</summary>
    ObjectImage,
    /// <summary>21AF:0412(object, frame) – object shown (frame and active flag are set by the core).</summary>
    ShowObject,
    /// <summary>21FD:037D(rgb12) – fade out to a colour.</summary>
    FadeOut,
    /// <summary>21FD:026C(offset) – base palette from the loaded palette (far pointer DS:offset).</summary>
    BasePalette,
    /// <summary>21FD:02C9(index, rgb12) – one colour of the base palette.</summary>
    PaletteColour,
    /// <summary>21FD:0329 – fade in.</summary>
    FadeIn,
    /// <summary>0A96:001B – empty the command list.</summary>
    ClearList,
    /// <summary>0A96:06A8 – execute the command list.</summary>
    RunList,
    /// <summary>0A96:0C3F(page) – copy the drawn page to the shown one (the page is the output layers').</summary>
    ShowPage,
    /// <summary>21FD:060D – execute the list, flip the pages, execute the list again.</summary>
    ListOnBothPages,
    /// <summary>0609:1ED9 – background tiles.</summary>
    DrawBackground,
    /// <summary>0609:1256(x, y, w, h) – frame.</summary>
    DrawFrame,
    /// <summary>0609:13C5(x, y, w, h, draws…) – border of the field; the core's random(1) results follow, top and bottom per column.</summary>
    DrawBorder,
    /// <summary>0609:1C83(x, y) – one field (nothing outside the field).</summary>
    DrawField,
    /// <summary>0609:1CB1(x, y) – a field and its four neighbours.</summary>
    DrawFieldAndNeighbours,
    /// <summary>0609:1A03 – terrain picture under the cursor of the player at turn and the purchase marker.</summary>
    DrawPanel,
    /// <summary>0609:1A97 – clears the purchase marker of the player at turn.</summary>
    DrawPurchaseUsed,
    /// <summary>0609:1AC0 – time bar of the player at turn.</summary>
    DrawPlayer,
    /// <summary>0609:0204 – status lines (weather and limits, or the free-field notice).</summary>
    DrawStatus,
    /// <summary>21FD:051F – flip the pages.</summary>
    FlipPages,
    /// <summary>23CD:071C(page) – restore the backgrounds of the objects (page of the output layers).</summary>
    RestoreObjects,
    /// <summary>23CD:06E9(page) – draw the objects (page of the output layers).</summary>
    DrawObjects,
    /// <summary>21FD:0185(count) – wait for count retraces (count as a long: low word, high word).</summary>
    Delay,
    /// <summary>21FD:0665 – last frame of the field: flip, restore, list, objects.</summary>
    LastFrame,
}

/// <summary>A request of the field layer to scene or sound output, with the original's arguments in call order.</summary>
public sealed record FieldEvent(FieldEventKind Kind, params short[] Args)
{
    public override string ToString() => $"{Kind}({string.Join(", ", Args)})";
}

/// <summary>Receiver of the cursor step's outputs.</summary>
public interface IFieldOutput
{
    /// <summary>
    /// 135D:000E(x, y) – the computer chooses its action for the cursor at (x, y). Not part of this port (computer player);
    /// called synchronously before the action is evaluated, so an implementation may set DS:3308.
    /// </summary>
    void ComputerTurn(Dgroup d, short x, short y);

    void Emit(FieldEvent e);
}

/// <summary>
/// Cursor logic of the field (finding docs/routines/0609_047B_cursor.md): one step of the player at turn DS:37AA – the
/// computer's choice, the action of DS:3308 (move the cursor or change the selected kind), the time unit used per move and
/// the idle blink. Works on the data segment; drawing and sound are emitted as <see cref="FieldEvent"/>s.
/// </summary>
public static class FieldCursor
{
    // DS offsets (docs/analyse/DATENLAYOUT.md, section of lane spielfeld).
    public const int CursorX = 0x304E;        // sbyte[4]
    public const int CursorY = 0x305A;        // sbyte[4]
    public const int Action = 0x3308;         // sbyte[4], 0 right, 1 left, 2 up, 3 down (from 0E9E)
    public const int Mode = 0x43B2;           // byte[4], 4 = choosing the kind, 6 = done
    public const int Direction = 0x4BCA;      // word[4], last move 0 right, 1 left, 2 up, 3 down
    public const int MoveTimer = 0x3022;      // word[4], 0x11 horizontal / 0x0C vertical after a move
    public const int MoveFrame = 0x4BD2;      // word[4]
    public const int Selection = 0x2E94;      // sbyte[4], selected kind -1..4
    public const int Terrain = 0x45DA;        // byte[9*6], 3 skips kind 3 in the selection
    public const int TimeLimit = 0x45BA;      // byte, 0 = no time limit
    public const int NoTime = 0x1582;         // byte, 1 = moves cost no time
    public const int TimeLeft = 0x4520;       // word[4], time units left
    public const int Finished = 0x3CE6;       // byte, counted down when a player runs out of time
    public const int Blink = 0x00E0;          // byte[4], blink countdown
    public const int Sprite = 0x330C;         // byte[26*n], sprite records of 26 bytes, +0 = visible
    public const int SpriteFrame = 0x3324;    // word in the same records
    public const int MoveDelay = 0x3786;      // dword[4] (0E9E)

    private static int P(Dgroup d) => d.S8(FieldGrid.CurrentPlayer);
    private static int At(int offset) => Int16Math.U(offset);

    /// <summary>0609:047B – one cursor step of the player at turn.</summary>
    // [ORIGINAL] 0609:047B
    public static void Step(Dgroup d, OriginalRandom random, IFieldOutput output)
    {
        int p = P(d);
        short x = d.S8(At(CursorX + p)), y = d.S8(At(CursorY + p));
        if (d.U8(At(DgroupLayout.IsComputer + p)) == 1) output.ComputerTurn(d, x, y);
        p = P(d);
        if (d.U8(At(Mode + p)) == 4)
        {
            ChangeSelection(d, x, y, output);
            return;
        }
        int field = Int16Math.U(Int16Math.S(x * 6) + y);
        switch ((ushort)d.S8(At(Action + p)))
        {
            case 0: // right
                if (d.S8(At(CursorX + p)) < Int16Math.S(d.S16(FieldGrid.Columns) - 1))
                    Move(d, p, CursorX, 0, 0x11, 8, +1);
                break;
            case 1: // left
                if (d.S8(At(CursorX + p)) > 0) Move(d, p, CursorX, 1, 0x11, 8, -1);
                break;
            case 2: // up; an HQ on the cursor's own field blocks
                if (d.S8(At(CursorY + p)) > 0 && d.U8(At(FieldGrid.Kind + field)) != 4)
                    Move(d, p, CursorY, 2, 0x0C, 0x0B, -1);
                break;
            case 3: // down; an HQ on the field below blocks
                if (d.S8(At(CursorY + p)) < Int16Math.S(d.S16(FieldGrid.Rows) - 1) && d.U8(At(FieldGrid.Kind + field + 1)) != 4)
                    Move(d, p, CursorY, 3, 0x0C, 1, +1);
                break;
        }
        if (d.S16(At(MoveTimer + 2 * p)) != 0) UseTime(d, output);
        else Idle(d, random);
    }

    private static void Move(Dgroup d, int p, int coordinate, short direction, short timer, short frame, int delta)
    {
        d.Set(At(Direction + 2 * p), direction);
        d.Set(At(MoveTimer + 2 * p), timer);
        d.Set(At(MoveFrame + 2 * p), frame);
        int at = At(coordinate + p);
        d.Set(at, unchecked((byte)(d.U8(at) + delta)));
    }

    /// <summary>0609:0B8E(x, y) – actions 2 and 3 step the selected kind down or up.</summary>
    // [ORIGINAL] 0609:0B8E
    public static void ChangeSelection(Dgroup d, short x, short y, IFieldOutput output)
    {
        int p = P(d);
        short action = d.S8(At(Action + p));
        if (action == 2) StepSelection(d, d.S8(At(Selection + p)), -1, x, y, output);
        else if (action == 3) StepSelection(d, d.S8(At(Selection + p)), 1, x, y, output);
    }

    /// <summary>
    /// 0609:0BEA(kind, step, x, y) – next kind: kind 3 is skipped on terrain 3, kind 4 (HQ) unless a neighbour field is
    /// foreign; outside −1..4 nothing changes. Otherwise store it, redraw, set the move delay to 4 and play sound (1, 14).
    /// </summary>
    // [ORIGINAL] 0609:0BEA
    public static void StepSelection(Dgroup d, short kind, short step, short x, short y, IFieldOutput output)
    {
        kind = Int16Math.S(kind + step);
        if (kind == 3 && d.U8(At(Terrain + Int16Math.S(x * 6) + y)) == 3) kind = Int16Math.S(kind + step);
        if (kind == 4 && !HasForeignNeighbour(d, x, y)) kind = Int16Math.S(kind + step);
        if (kind < -1 || kind > 4) return;
        int p = P(d);
        d.Set(At(Selection + p), unchecked((byte)kind));
        output.Emit(new FieldEvent(FieldEventKind.DrawSelection));
        p = P(d);
        d.Set(At(MoveDelay + 4 * p), (short)4);
        d.Set(At(MoveDelay + 4 * p + 2), (short)0);
        output.Emit(new FieldEvent(FieldEventKind.Sound, 1, 0x0E));
    }

    /// <summary>0609:0F0D(x, y) – 1 if one of the four neighbours (right, left, below, above) is foreign (0F70).</summary>
    // [ORIGINAL] 0609:0F0D
    public static bool HasForeignNeighbour(Dgroup d, short x, short y) =>
        IsForeign(d, Int16Math.S(x + 1), y) || IsForeign(d, Int16Math.S(x - 1), y) ||
        IsForeign(d, x, Int16Math.S(y + 1)) || IsForeign(d, x, Int16Math.S(y - 1));

    /// <summary>0609:0F70(x, y) – 1 if the field is inside and not owned by the player at turn.</summary>
    // [ORIGINAL] 0609:0F70
    public static bool IsForeign(Dgroup d, short x, short y) =>
        FieldGrid.Inside(d, x, y) && d.U8(At(FieldGrid.Owner + Int16Math.S(x * 6) + y)) != d.U8(FieldGrid.CurrentPlayer);

    /// <summary>
    /// 0609:0EA1 – a move costs one time unit if the time limit is on: none left ends the player's turn (0EDD, AL = 0),
    /// otherwise one unit is taken (1B72(−1)). AL = 1 in all other cases.
    /// </summary>
    // [ORIGINAL] 0609:0EA1
    public static bool UseTime(Dgroup d, IFieldOutput output)
    {
        if (d.U8(NoTime) == 1 || d.U8(TimeLimit) == 0) return true;
        if (d.S16(At(TimeLeft + 2 * P(d))) == 0)
        {
            OutOfTime(d, output);
            return false;
        }
        TimeBar(d, -1, output);
        return true;
    }

    /// <summary>0609:0EDD – out of time: one finished player more, mode 6, cursor sprite off (21BA), rank icon drawn.</summary>
    // [ORIGINAL] 0609:0EDD
    public static void OutOfTime(Dgroup d, IFieldOutput output)
    {
        d.Set(Finished, unchecked((byte)(d.U8(Finished) - 1)));
        d.Set(At(Mode + P(d)), (byte)6);
        HideCursor(d);
        output.Emit(new FieldEvent(FieldEventKind.DrawRank, Int16Math.S((P(d) << 6) + 0x20), 6));
    }

    /// <summary>0609:21BA – 21AF:045E(player): clears the visible byte of the player's cursor sprite record.</summary>
    // [ORIGINAL] 0609:21BA
    // [ORIGINAL] 21AF:045E
    public static void HideCursor(Dgroup d) => d.Set(At(Sprite + Int16Math.S(P(d) * 0x1A)), (byte)0);

    /// <summary>
    /// 0609:1B72(step) – draws one cell of the time bar (cell index = time left, +1 when growing; colour from the player when
    /// growing, F1h when shrinking) and adds the step to the time left.
    /// </summary>
    // [ORIGINAL] 0609:1B72
    public static void TimeBar(Dgroup d, sbyte step, IFieldOutput output)
    {
        int p = P(d);
        short cell = d.S16(At(TimeLeft + 2 * p));
        short colour;
        if (step == 1)
        {
            cell = Int16Math.S(cell + 1);
            colour = Int16Math.S((p << 3) + 0xD1);
        }
        else colour = 0xF1;
        output.Emit(new FieldEvent(FieldEventKind.DrawCommand, 0x110, Int16Math.S(colour + cell % 8), 0x10, 1,
            Int16Math.S((p << 6) + 0x10), Int16Math.S(0x2F - cell)));
        int at = At(TimeLeft + 2 * P(d));
        d.Set(at, Int16Math.S(d.S16(at) + step));
    }

    /// <summary>
    /// 0609:215E – idle: a running blink counts down and sets the sprite frame (counter mod 2) · 2; otherwise random(200) = 0
    /// starts a new blink of 19 steps.
    /// </summary>
    // [ORIGINAL] 0609:215E
    public static void Idle(Dgroup d, OriginalRandom random)
    {
        int p = P(d);
        if (d.U8(At(Blink + p)) != 0)
        {
            d.Set(At(Blink + p), (byte)(d.U8(At(Blink + p)) - 1));
            int frame = d.U8(At(Blink + P(d))) % 2 * 2;
            d.Set(At(SpriteFrame + Int16Math.S(P(d) * 0x1A)), (short)frame);
        }
        else if (random.Next(200) == 0) d.Set(At(Blink + P(d)), (byte)0x13);
    }
}
