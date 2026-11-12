using Traders.Core.State;

namespace Traders.Core.Field;

/// <summary>
/// Shared field layer of segment 0609 (finding docs/routines/0609_10D2_feldart.md): bounds and ownership tests and setting the
/// owner and kind of a field with its counters. Works on the data segment with the original's 16-bit index arithmetic.
/// </summary>
public static class FieldGrid
{
    // DS offsets (docs/analyse/DATENLAYOUT.md, section of lane spielfeld).
    public const int Owner = 0x4442;          // sbyte[x*6+y], -1 = nobody
    public const int Kind = 0x2EB1;           // sbyte[x*6+y], -2 = empty, 0..2 production, 3 rocket, 4 HQ
    public const int FreeFields = 0x2F05;     // byte, fields without owner
    public const int OwnedFields = 0x4BA8;    // byte[4], fields per player
    public const int OwnedKinds = 0x2EE7;     // byte[4][4], fields per player and kind 0..3
    public const int Columns = 0x08FC;        // word
    public const int Rows = 0x08FE;           // word
    public const int CurrentPlayer = 0x37AA;  // sbyte, player whose turn it is

    private static int Cell(int x, int y) => Int16Math.U(x * 6 + y); // imul 16 bit, add

    /// <summary>0609:10A7(x, y) – 1 if 0 ≤ x &lt; columns and 0 ≤ y &lt; rows (signed compares), else 0.</summary>
    // [ORIGINAL] 0609:10A7
    public static bool Inside(Dgroup d, short x, short y) =>
        x >= 0 && Int16Math.S(d.S16(Columns) - 1) >= x && y >= 0 && Int16Math.S(d.S16(Rows) - 1) >= y;

    /// <summary>0609:1072(x, y) – 1 if the field is inside and its owner byte equals DS:37AA.</summary>
    // [ORIGINAL] 0609:1072
    public static bool OwnedByCurrent(Dgroup d, short x, short y) =>
        Inside(d, x, y) && d.U8(Int16Math.U(Owner + Cell(x, y))) == d.U8(CurrentPlayer);

    /// <summary>
    /// 0609:10D2(x, y, owner, kind) – moves the field from its old owner to the new one: the free-field count or the old owner's
    /// field count goes down (and his count of the old kind, if 0..3), the new side's counts go up, then owner and kind are
    /// stored as bytes.
    /// </summary>
    // [ORIGINAL] 0609:10D2
    public static void Set(Dgroup d, short x, short y, short owner, short kind)
    {
        int cell = Cell(x, y);
        short oldOwner = d.S8(Int16Math.U(Owner + cell));
        short oldKind = d.S8(Int16Math.U(Kind + cell));
        if (oldOwner == -1) Add(d, FreeFields, -1);
        else
        {
            Add(d, OwnedFields + oldOwner, -1);
            if (oldKind >= 0 && oldKind <= 3) Add(d, OwnedKinds + Int16Math.U(oldOwner << 2) + oldKind, -1);
        }
        if (owner == -1) Add(d, FreeFields, 1);
        else
        {
            Add(d, OwnedFields + owner, 1);
            if (kind >= 0 && kind <= 3) Add(d, OwnedKinds + Int16Math.U(owner << 2) + kind, 1);
        }
        d.Set(Int16Math.U(Owner + cell), unchecked((byte)owner));
        d.Set(Int16Math.U(Kind + cell), unchecked((byte)kind));
    }

    // inc/dec byte [offset] with the 16-bit wrap of the effective address.
    private static void Add(Dgroup d, int offset, int delta)
    {
        int at = Int16Math.U(offset);
        d.Set(at, unchecked((byte)(d.U8(at) + delta)));
    }
}
