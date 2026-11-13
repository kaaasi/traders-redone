using Traders.Core.Field;
using Traders.Core.State;

namespace Traders.Core.Ai;

/// <summary>
/// Computer turn in the planting phase, segment 135D (finding docs/routines/135D_000E_computerzug.md). The computer chooses its
/// next cursor action (DS:3308), a target field and a crop kind, evaluating fields on the data segment. 135D draws no random
/// (R5P-5); it only reads and writes DGROUP and calls the field helpers 0609:074D/100F/10A7/1072.
///
/// Public entry points match the original's three call sites: <see cref="ComputerTurn"/> = 135D:000E (from 0609:047B),
/// <see cref="CommitCandidate"/> = 135D:0935 (from 0609:096A), <see cref="ChooseWantKind"/> = 135D:0AB3 (from 0609:069F).
/// All 16-bit arithmetic goes through <see cref="Int16Math"/>; byte intermediates wrap as sbyte/byte like the original.
/// </summary>
public static class PlantingAi
{
    // DS offsets (docs/analyse/DATENLAYOUT.md, lane anbau-ki).
    private const int CurrentPlayer = 0x37AA; // sbyte
    private const int Mode = 0x43B2;          // byte[4]
    private const int MoveDelayLo = 0x3786;   // dword[4] (stride 4), guard in 000E
    private const int MoveDelayHi = 0x3788;
    private const int Action = 0x3308;        // sbyte[4]
    private const int RefX = 0x43F6, RefY = 0x43F8;     // word
    private const int TargetX = 0x40CE, TargetY = 0x40D6; // word[4]
    private const int PickX = 0x410A, PickY = 0x4112;     // word[4]
    private const int NoneFound = 0x4BBE;     // byte[4]
    private const int CandIndex = 0x3C90;     // word[4]
    private const int CandX = 0x4654, CandY = 0x472C, CandKind = 0x2F31; // byte[4*54], stride 54
    private const int CandCount = 0x48EC;     // word[4]
    private const int WantKind = 0x40F6;      // word[4]
    private const int HqX = 0x40FE, HqY = 0x4102; // sbyte[4]
    private const int LoopX = 0x2EB0, LoopY = 0x2F00; // sbyte (global loop vars)
    private const int LoopW = 0x45D0;         // word (loop var in 0AB3)
    private const int Selection = 0x2E94;     // sbyte[4]
    private const int FieldKind = 0x2EB1;     // sbyte[9*6]
    private const int FieldQuality = 0x45DA;  // sbyte[9*6]
    private const int Owner = 0x4442;         // sbyte[9*6]
    private const int OwnedFields = 0x4BA8;   // byte[4]
    private const int Rockets = 0x4BAC;       // word[4]
    private const int Stock = 0x3CF4;         // word[4][4], stride 8
    private const int LimitHigh = 0x48A2;     // sbyte
    private const int TimeLeft = 0x4520;      // word[4]
    private const int TimeLimit = 0x45BA;     // byte
    private const int FreeFields = 0x2F05;    // byte
    private const int IsComputer = 0x0840;    // byte[4]
    private const int ComputerIq = 0x2E8E;    // word
    private const int Marker4580 = 0x4580;    // sbyte
    private const int FatMike = 0x4BC2;       // byte[4]
    private const int T2EEA = 0x2EEA;         // byte[4][?], stride 4
    private const int KindWare2 = 0x08E7;     // sbyte[kind] -> ware index
    private const int QualityTable = 0x0902;  // sbyte[3][4], stride 4
    private const int Marker43FA = 0x43FA;    // byte[4]
    private const int Columns = 0x08FC, Rows = 0x08FE; // word

    private static int P(Dgroup d) => d.S8(CurrentPlayer);
    private static int Cell(short x, short y) => Int16Math.U(Int16Math.S(x * 6) + y);

    // ---- 135D:000E -------------------------------------------------------------------------------------------------

    /// <summary>135D:000E(x, y) – the computer picks its action for the cursor at (x, y) and stores it in DS:3308[p].</summary>
    // [ORIGINAL] 135D:000E
    public static void ComputerTurn(Dgroup d, short x, short y)
    {
        int p = P(d);
        short hi = d.S16(Int16Math.U(MoveDelayHi + 4 * p));
        ushort lo = d.U16(Int16Math.U(MoveDelayLo + 4 * p));
        if (!(hi < 0 || (hi == 0 && lo == 0))) return; // move delay still positive -> do nothing

        d.Set(RefX, x);
        d.Set(RefY, y);
        short action = -1;
        int idx = (d.U8(Int16Math.U(Mode + p)) - 2) & 0xFFFF;
        switch (idx)
        {
            case 0: ChooseFreeField(d); action = ActionToTarget(d); break; // 00AC, 04F5
            case 1: ChooseExpansion(d); action = ActionToTarget(d); break; // 02BD, 04F5
            case 2: action = ActionSelectKind(d); break;                   // 0A59
            case 3: ChooseStart(d); action = ActionToTarget(d); break;     // 0480, 04F5
            default: break;                                                // other modes: action stays -1
        }
        d.Set(Int16Math.U(Action + p), unchecked((byte)action));
    }

    // ---- 135D:0240 / 0263 / 0284 ----------------------------------------------------------------------------------

    /// <summary>135D:0240(x, y) – quadrant index (2x/cols) + 2*(2y/rows), signed 16-bit idiv.</summary>
    // [ORIGINAL] 135D:0240
    private static short Quadrant(Dgroup d, short x, short y)
    {
        short xq = Int16Math.Div(Int16Math.S(x * 2), d.S16(Columns));
        short yq = Int16Math.Div(Int16Math.S(y * 2), d.S16(Rows));
        return Int16Math.S(xq + Int16Math.S(yq * 2));
    }

    /// <summary>135D:0263/0284 – Manhattan distance of the loop field (DS:2EB0/2F00) to the reference (DS:43F6/43F8).</summary>
    // [ORIGINAL] 135D:0263
    // [ORIGINAL] 135D:0284
    private static short Distance(Dgroup d)
    {
        short cx = d.S8(LoopX), cy = d.S8(LoopY), tx = d.S16(RefX), ty = d.S16(RefY);
        short dx = cx <= tx ? Int16Math.S(tx - cx) : Int16Math.S(cx - tx);
        short dy = cy <= ty ? Int16Math.S(ty - cy) : Int16Math.S(cy - ty);
        return Int16Math.S(dx + dy);
    }

    // ---- 135D:00AC – free-field choice (mode 2) -------------------------------------------------------------------

    // [ORIGINAL] 135D:00AC
    private static void ChooseFreeField(Dgroup d)
    {
        int p = P(d);
        Span<byte> quadCount = stackalloc byte[4];
        quadCount.Clear();
        for (int pl = 0; pl < 4; pl++)
        {
            if (pl == p) continue;
            if (d.U8(Int16Math.U(HqX + pl)) == 0xFF) continue;
            short q = Quadrant(d, d.S8(Int16Math.U(HqX + pl)), d.S8(Int16Math.U(HqY + pl)));
            quadCount[q & 3]++; // HQ coordinates are always in-grid -> quadrant 0..3 (see finding)
        }

        short cols = d.S16(Columns), rows = d.S16(Rows);
        sbyte bestScore = unchecked((sbyte)0x9C); // -100
        sbyte bestDist = 0x64;                     // 100
        for (short y = 0; y <= Int16Math.S(rows - 1); y++)
        {
            d.Set(LoopY, (sbyte)y);
            for (short x = 0; x <= Int16Math.S(cols - 1); x++)
            {
                d.Set(LoopX, (sbyte)x);
                if (!PlantingAiHelpers.CanPlant(d)) continue;
                byte edgeX = (byte)(Int16Math.Div(cols, 2) > x ? (byte)x : (byte)(d.U8(Columns) - 1 - x));
                byte edgeY = (byte)(Int16Math.Div(rows, 2) > y ? (byte)y : (byte)(d.U8(Rows) - 1 - y));
                sbyte score = unchecked((sbyte)(edgeX + edgeY));
                short q = Quadrant(d, x, y);
                score = unchecked((sbyte)(score - (byte)(quadCount[q & 3] * 2)));
                if (bestScore > score) continue;                 // keep higher score
                sbyte dist = unchecked((sbyte)Distance(d));
                if (bestScore == score && dist > bestDist) continue; // tie: keep smaller distance
                bestScore = score;
                bestDist = dist;
                d.Set(Int16Math.U(TargetX + 2 * p), x);
                d.Set(Int16Math.U(TargetY + 2 * p), y);
            }
        }
        d.Set(LoopX, (sbyte)cols); // loop variables left behind at their past-end values
        d.Set(LoopY, (sbyte)rows);
    }

    // ---- 135D:04F5 – action toward the chosen target --------------------------------------------------------------

    // [ORIGINAL] 135D:04F5
    private static short ActionToTarget(Dgroup d)
    {
        int p = P(d);
        short tx = d.S16(Int16Math.U(TargetX + 2 * p)), ty = d.S16(Int16Math.U(TargetY + 2 * p));
        short rx = d.S16(RefX), ry = d.S16(RefY);
        int cell = Cell(rx, ry);
        if (ty > ry && d.U8(Int16Math.U(FieldKind + cell + 1)) != 4) return 3;   // down, unless field (x, y+1) is HQ
        if (ty < ry && d.U8(Int16Math.U(FieldKind + cell)) != 4) return 2;       // up, unless field (x, y) is HQ
        if (tx > rx) return 0;                                                   // right
        if (tx < rx) return 1;                                                   // left
        if (ty == ry) return 0x0A;                                               // at target -> key
        return Int16Math.Div(d.S16(Columns), 2) > rx ? (short)0 : (short)1;
    }

    // ---- 135D:0A59 – action in mode 4 (step the selected kind) ----------------------------------------------------

    // [ORIGINAL] 135D:0A59
    private static short ActionSelectKind(Dgroup d)
    {
        int p = P(d);
        int at = Int16Math.U(Int16Math.S(p * 54) + d.S16(Int16Math.U(CandIndex + 2 * p)));
        sbyte cl = d.S8(Int16Math.U(CandKind + at));
        if (d.S8(Int16Math.U(Selection + p)) < cl) return 3; // selection below the wanted kind -> step up
        d.Set(Int16Math.U(CandX + at), (byte)0xFF);          // candidate consumed
        return 0x0A;
    }

    // ---- 135D:02BD – expansion choice (mode 3) --------------------------------------------------------------------

    // [ORIGINAL] 135D:02BD
    private static void ChooseExpansion(Dgroup d)
    {
        int p = P(d);
        sbyte bestDist = 0x64; // 100
        d.Set(Int16Math.U(CandIndex + 2 * p), (short)0);

        if (d.U8(Int16Math.U(Marker43FA + p)) == 1)
        {
            ChooseFreeAdjacent(d);
            if (d.U8(Int16Math.U(NoneFound + p)) == 0)
            {
                d.Set(LoopX, unchecked((sbyte)d.U16(Int16Math.U(PickX + 2 * p))));
                d.Set(LoopY, unchecked((sbyte)d.U16(Int16Math.U(PickY + 2 * p))));
                short dist = Distance(d);
                bool take = d.U8(TimeLimit) == 0 || d.S16(Int16Math.U(TimeLeft + 2 * p)) >= dist;
                if (take)
                {
                    d.Set(Int16Math.U(TargetX + 2 * p), (short)d.S8(LoopX));
                    d.Set(Int16Math.U(TargetY + 2 * p), (short)d.S8(LoopY));
                    return;
                }
            }
        }

        short count = d.S16(Int16Math.U(CandCount + 2 * p));
        for (short si = 0; si <= Int16Math.S(count - 1); si++)
        {
            int baseAt = Int16Math.U(Int16Math.S(p * 54) + si);
            sbyte lx = d.S8(Int16Math.U(CandX + baseAt));
            d.Set(LoopX, lx);
            if ((byte)lx == 0xFF) continue;
            d.Set(LoopY, d.S8(Int16Math.U(CandY + baseAt)));
            short dist = Distance(d);
            if (dist > bestDist) continue; // keep nearest
            sbyte kind = d.S8(Int16Math.U(CandKind + baseAt));
            bool ok;
            if (kind == 3)
                ok = d.S16(Int16Math.U(Rockets + 2 * p)) != 0;
            else
            {
                int wareAt = Int16Math.U(Int16Math.S(p * 8) + Int16Math.S(d.S8(Int16Math.U(KindWare2 + kind)) * 2));
                ok = d.S16(Int16Math.U(Stock + wareAt)) >= d.S8(LimitHigh);
            }
            if (!ok) continue;
            if (d.U8(TimeLimit) == 1 && d.S16(Int16Math.U(TimeLeft + 2 * p)) < dist) continue;
            bestDist = unchecked((sbyte)dist);
            d.Set(Int16Math.U(TargetX + 2 * p), (short)d.S8(LoopX));
            d.Set(Int16Math.U(TargetY + 2 * p), (short)d.S8(LoopY));
            d.Set(Int16Math.U(CandIndex + 2 * p), si);
        }

        if (bestDist == 0x64)
        {
            d.Set(Int16Math.U(TargetX + 2 * p), (short)d.S8(Int16Math.U(HqX + p)));
            d.Set(Int16Math.U(TargetY + 2 * p), (short)d.S8(Int16Math.U(HqY + p)));
        }
    }

    // ---- 135D:0480 – start choice (mode 5) ------------------------------------------------------------------------

    // [ORIGINAL] 135D:0480
    private static void ChooseStart(Dgroup d)
    {
        int p = P(d);
        if (d.U8(FreeFields) == 0)
        {
            d.Set(Int16Math.U(TargetX + 2 * p), (short)d.S8(Int16Math.U(HqX + p)));
            d.Set(Int16Math.U(TargetY + 2 * p), (short)d.S8(Int16Math.U(HqY + p)));
            return;
        }
        ChooseFreeAdjacent(d);
        d.Set(Int16Math.U(TargetX + 2 * p), d.S16(Int16Math.U(PickX + 2 * p)));
        d.Set(Int16Math.U(TargetY + 2 * p), d.S16(Int16Math.U(PickY + 2 * p)));
    }

    // ---- 135D:05AF – best free (adjacent) field -------------------------------------------------------------------

    // [ORIGINAL] 135D:05AF
    private static void ChooseFreeAdjacent(Dgroup d)
    {
        int p = P(d);
        sbyte bestDist = 0x64;
        short bestScore = -100;
        short cols = d.S16(Columns), rows = d.S16(Rows);
        for (short y = 0; y <= Int16Math.S(rows - 1); y++)
        {
            d.Set(LoopY, (sbyte)y);
            for (short x = 0; x <= Int16Math.S(cols - 1); x++)
            {
                d.Set(LoopX, (sbyte)x);
                if (d.U8(Int16Math.U(Owner + Cell(x, y))) != 0xFF) continue;
                if (d.U8(Int16Math.U(Mode + p)) != 5 && !PlantingAiHelpers.HasOwnNeighbour(d, x, y)) continue;
                sbyte score = unchecked((sbyte)ScoreField(d));
                if (score < bestScore) continue;
                sbyte dist = unchecked((sbyte)Distance(d));
                if (score == bestScore && dist > bestDist) continue;
                d.Set(Int16Math.U(PickX + 2 * p), x);
                d.Set(Int16Math.U(PickY + 2 * p), y);
                bestScore = score;
                bestDist = dist;
            }
        }
        d.Set(LoopX, (sbyte)cols);
        d.Set(LoopY, (sbyte)rows);
        d.Set(Int16Math.U(NoneFound + p), (byte)(bestScore == -100 ? 1 : 0));
    }

    // ---- 135D:06A1 – field score -----------------------------------------------------------------------------------

    // [ORIGINAL] 135D:06A1
    private static int ScoreField(Dgroup d)
    {
        int p = P(d);
        short x = d.S8(LoopX), y = d.S8(LoopY);
        int cell = Cell(x, y);
        sbyte baseV;
        if (d.U8(Int16Math.U(OwnedFields + p)) <= 3 && d.S8(Int16Math.U(FieldQuality + cell)) == 3)
            baseV = 0;
        else
        {
            short q = d.S8(Int16Math.U(FieldQuality + cell));
            int qt = Int16Math.U(Int16Math.S(d.S16(Int16Math.U(WantKind + 2 * p)) * 4) + q);
            baseV = unchecked((sbyte)(d.S8(Int16Math.U(QualityTable + qt)) * 3));
        }
        byte sum = unchecked((byte)Neighbour(d, Int16Math.S(x + 1), y));            // right
        sum = unchecked((byte)(sum + (byte)Neighbour(d, Int16Math.S(x - 1), y)));   // left
        sum = unchecked((byte)(sum + (byte)Neighbour(d, x, Int16Math.S(y + 1))));   // down
        sum = unchecked((byte)(sum + (byte)Neighbour(d, x, Int16Math.S(y - 1))));   // up
        sum = unchecked((byte)(sum + (byte)Int16Math.Div(Neighbour(d, Int16Math.S(x + 1), Int16Math.S(y + 1)), 2)));
        sum = unchecked((byte)(sum + (byte)Int16Math.Div(Neighbour(d, Int16Math.S(x + 1), Int16Math.S(y - 1)), 2)));
        sum = unchecked((byte)(sum + (byte)Int16Math.Div(Neighbour(d, Int16Math.S(x - 1), Int16Math.S(y + 1)), 2)));
        sum = unchecked((byte)(sum + (byte)Int16Math.Div(Neighbour(d, Int16Math.S(x - 1), Int16Math.S(y - 1)), 2)));
        return Int16Math.S(baseV + unchecked((sbyte)sum));
    }

    // ---- 135D:0851 – neighbour contribution ------------------------------------------------------------------------

    // [ORIGINAL] 135D:0851
    private static short Neighbour(Dgroup d, short x, short y)
    {
        sbyte s = d.S8(Int16Math.U(Owner + Cell(x, y)));
        if (!FieldGrid.Inside(d, x, y)) return 0;
        if (s == -1) return 0;
        if (d.S8(CurrentPlayer) == s) return 0;
        if (d.U8(Int16Math.U(Mode + P(d))) == 5) return -1;
        if (d.U8(Int16Math.U(IsComputer + (s & 0xFFFF))) == 0) return Int16Math.S(d.S16(ComputerIq) + 3);
        return 0;
    }

    // ---- 135D:0935 – append a candidate (from 0609:096A, action 0Ah mode 4) ---------------------------------------

    /// <summary>135D:0935 – decides the crop for the picked field and appends it to the player's candidate list.</summary>
    // [ORIGINAL] 135D:0935
    public static void CommitCandidate(Dgroup d)
    {
        int p = P(d);
        short si = d.S16(Int16Math.U(CandCount + 2 * p));
        sbyte want = unchecked((sbyte)d.U16(Int16Math.U(WantKind + 2 * p))); // low byte of WantKind
        d.Set(LoopX, unchecked((sbyte)d.U16(Int16Math.U(PickX + 2 * p))));
        d.Set(LoopY, unchecked((sbyte)d.U16(Int16Math.U(PickY + 2 * p))));
        short lx = d.S8(LoopX), ly = d.S8(LoopY);
        int cell = Cell(lx, ly);

        sbyte cl;
        // Rocket path: current player is the marked one, Fat Mike flag set, or DS:2EEA[p*4] < 2.
        bool rocketPath = d.S8(CurrentPlayer) == d.S8(Marker4580)
                          || d.U8(Int16Math.U(FatMike + p)) == 1
                          || d.S8(Int16Math.U(T2EEA + Int16Math.U(p << 2))) < 2;
        if (rocketPath && d.S8(Int16Math.U(FieldQuality + cell)) != 3)
            cl = 3; // rocket
        else
        {
            short q = d.S8(Int16Math.U(FieldQuality + cell));
            int qt = Int16Math.U(Int16Math.S(want * 4) + q);
            if (d.S8(Int16Math.U(QualityTable + qt)) == 0) return; // kind yields nothing here
            cl = want;
        }

        if (d.S8(Int16Math.U(FieldKind + cell)) == cl) return; // already that kind
        int listBase = Int16Math.S(p * 54);
        d.Set(Int16Math.U(CandKind + listBase + si), unchecked((byte)cl));
        d.Set(Int16Math.U(CandX + listBase + si), d.U8(LoopX));
        d.Set(Int16Math.U(CandY + listBase + si), d.U8(LoopY));
        d.Set(Int16Math.U(CandCount + 2 * p), Int16Math.S(si + 1));
    }

    // ---- 135D:0AB3 – choose the wanted crop kind (from 0609:069F, action 0Ah mode 2) ------------------------------

    /// <summary>135D:0AB3 – recomputes the wanted kind DS:40F6[p] from the 3x3 field around the HQ (best quality sum).</summary>
    // [ORIGINAL] 135D:0AB3
    public static void ChooseWantKind(Dgroup d)
    {
        int p = P(d);
        Span<byte> sums = stackalloc byte[4];
        sums.Clear();
        d.Set(LoopW, (short)4); // zero loop leaves 45D0 at 4

        short hx = d.S8(Int16Math.U(HqX + p)), hy = d.S8(Int16Math.U(HqY + p));
        for (short y = Int16Math.S(hy - 1); y <= Int16Math.S(hy + 1); y++)
        {
            d.Set(LoopY, (sbyte)y);
            for (short x = Int16Math.S(hx - 1); x <= Int16Math.S(hx + 1); x++)
            {
                d.Set(LoopX, (sbyte)x);
                if (!FieldGrid.Inside(d, x, y)) continue;
                int cell = Cell(x, y);
                if (d.U8(Int16Math.U(Owner + cell)) != 0xFF) continue;
                for (short j = 0; j <= 2; j++)
                {
                    d.Set(LoopW, j);
                    short q = d.S8(Int16Math.U(FieldQuality + cell));
                    int qt = Int16Math.U(Int16Math.S(j * 4) + q);
                    sums[j] = unchecked((byte)(sums[j] + (byte)d.S8(Int16Math.U(QualityTable + qt))));
                }
                d.Set(LoopW, (short)3);
            }
        }
        d.Set(LoopX, (sbyte)Int16Math.S(hx + 2)); // loop variables left behind at their past-end values
        d.Set(LoopY, (sbyte)Int16Math.S(hy + 2));

        byte best = sums[d.S16(Int16Math.U(WantKind + 2 * p)) & 3];
        for (short j = 0; j <= 2; j++)
        {
            d.Set(LoopW, j);
            if (unchecked((sbyte)sums[j]) > unchecked((sbyte)best))
            {
                best = sums[j];
                d.Set(Int16Math.U(WantKind + 2 * p), j);
            }
        }
        d.Set(LoopW, (short)3);
    }
}
