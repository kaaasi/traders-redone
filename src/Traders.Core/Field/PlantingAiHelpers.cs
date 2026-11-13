using Traders.Core.State;

namespace Traders.Core.Field;

/// <summary>
/// Two field-layer helpers of segment 0609 that the computer turn 135D needs (findings
/// docs/routines/0609_074D_pflanzbar.md and 0609_100F_nachbar.md): whether a plot may be planted and whether a field borders
/// the current player's land. Owned by lane anbau-ki (R5P-2). Pure predicates on the data segment, no random, no state change.
/// </summary>
public static class PlantingAiHelpers
{
    private const int LoopX = 0x2EB0;   // global loop column (sbyte)
    private const int LoopY = 0x2F00;   // global loop row (sbyte)
    private const int KindLeft = 0x2EAB;  // FieldGrid.Kind - 6, i.e. field (x-1, y)
    private const int KindRight = 0x2EB7; // FieldGrid.Kind + 6, i.e. field (x+1, y)

    private static int Cell(short x, short y) => Int16Math.U(Int16Math.S(x * 6) + y);

    /// <summary>
    /// 0609:074D – 1 if the field at the global loop coordinates DS:2EB0/2F00 may be planted: a quality-3 field only while free,
    /// and no horizontally adjacent HQ (left when x &gt; 0, right when x &lt; columns-1).
    /// </summary>
    // [ORIGINAL] 0609:074D
    public static bool CanPlant(Dgroup d)
    {
        short x = d.S8(LoopX), y = d.S8(LoopY);
        int cell = Cell(x, y);
        if (d.S8(Int16Math.U(DgroupLayout.FieldQuality + cell)) != 3) return false; // only quality-3 terrain
        if (d.U8(Int16Math.U(FieldGrid.Owner + cell)) != 0xFF) return false;         // and only while free
        if (x > 0 && d.U8(Int16Math.U(KindLeft + cell)) == 4) return false;
        if (x < Int16Math.S(d.S16(FieldGrid.Columns) - 1) && d.U8(Int16Math.U(KindRight + cell)) == 4) return false;
        return true;
    }

    /// <summary>0609:100F(x, y) – 1 if one of the four orthogonal neighbours (left, right, up, down) is owned by DS:37AA.</summary>
    // [ORIGINAL] 0609:100F
    public static bool HasOwnNeighbour(Dgroup d, short x, short y) =>
        FieldGrid.OwnedByCurrent(d, Int16Math.S(x - 1), y) || FieldGrid.OwnedByCurrent(d, Int16Math.S(x + 1), y) ||
        FieldGrid.OwnedByCurrent(d, x, Int16Math.S(y - 1)) || FieldGrid.OwnedByCurrent(d, x, Int16Math.S(y + 1));
}
