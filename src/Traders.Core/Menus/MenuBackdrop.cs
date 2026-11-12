namespace Traders.Core.Menus;

/// <summary>Kind of a draw operation of the menu backdrop (calls into the graphics driver 0A96/0A69).</summary>
public enum BackdropOpKind
{
    /// <summary>0A96:0030(sx, sy, w, h, dx, dy): copy from the picture page.</summary>
    Blit,

    /// <summary>0A96:013B(sx, sy, mx, my, w, h, dx, dy): masked copy.</summary>
    MaskedBlit,

    /// <summary>0A96:04D7(x, y, w, h, colour): filled rectangle.</summary>
    FilledRect,

    /// <summary>0A69:000B(x, y, w, colour): horizontal line.</summary>
    HorizontalLine,

    /// <summary>0A69:002C(x, y, h, colour): vertical line.</summary>
    VerticalLine,
}

/// <summary>
/// One draw call of the backdrop with the arguments as the original passes them (16-bit words). The core computes these
/// because their coordinates come from random(); the scene replays them through the graphics layers.
/// </summary>
public readonly record struct BackdropOp(BackdropOpKind Kind, ushort A, ushort B, ushort C, ushort D, ushort E = 0, ushort F = 0,
    ushort G = 0, ushort H = 0);

/// <summary>
/// The random parts of the menu pictures: the brick wall 192D:131B (bricks 138F with rivets 14B4) and the plaque 192D:14FB.
/// Every random() call of these routines is made here in the original order (through <c>random</c> = random(n) of the core);
/// the result is the list of draw calls.
/// </summary>
public static class MenuBackdrop
{
    /// <summary>[ORIGINAL] 192D:131B – rows of bricks every 40 lines from y = −20 to 180, random widths, until x = 320.</summary>
    public static List<BackdropOp> Wall(Func<short, short> random)
    {
        var ops = new List<BackdropOp>();
        for (short y = -20; y <= 0xC7; y = Int16Math.S(y + 0x28))
        {
            short x = 0;
            while (true)
            {
                short w = x == 0 ? Int16Math.S(random(0x78) + 0x0F) : Int16Math.S(random(0x3C) + 0x78);
                if (Int16Math.S(x + w) > 0x131) w = Int16Math.S(0x140 - x);
                Brick(random, ops, x, y, w);
                x = Int16Math.S(x + w);
                if (x == 0x140) break;
            }
        }
        return ops;
    }

    /// <summary>[ORIGINAL] 192D:138F(x, y, w) – one brick, clipped at the top and bottom, edges and rivets inside.</summary>
    private static void Brick(Func<short, short> random, List<BackdropOp> ops, short x, short y, short w)
    {
        short skip, h;
        if (y < 0)
        {
            skip = Int16Math.S(-y);
            h = Int16Math.S(0x28 - skip);
            y = 0;
        }
        else if (y > 0xA0)
        {
            skip = 0;
            h = Int16Math.S(0xC8 - y);
        }
        else
        {
            skip = 0;
            h = 0x28;
        }
        if (h <= 0) return;
        short sx = Int16Math.S(random(Int16Math.S(0x13C - w)) + 2);
        ops.Add(Op(BackdropOpKind.Blit, sx, skip, w, h, x, y));
        if (x > 0)
        {
            ops.Add(Op(BackdropOpKind.Blit, 0, skip, 2, h, x, y));
            if (skip == 0) Rivet(random, ops, Int16Math.S(x + 4), Int16Math.S(y + 4));
            if (skip != 0 || h == 0x28) Rivet(random, ops, Int16Math.S(x + 4), Int16Math.S(y + h - 10));
        }
        if (Int16Math.S(x + w) == 0x140) return;
        ops.Add(Op(BackdropOpKind.Blit, 0x13E, skip, 2, h, Int16Math.S(x + w - 2), y));
        if (skip == 0) Rivet(random, ops, Int16Math.S(x + w - 11), Int16Math.S(y + 3));
        if (skip != 0 || h == 0x28) Rivet(random, ops, Int16Math.S(x + w - 11), Int16Math.S(y + h - 10));
    }

    /// <summary>[ORIGINAL] 192D:14B4(x, y) – rivet 7×7 (mask at D0h/4Bh) moved by 0…2 in y first, then in x.</summary>
    private static void Rivet(Func<short, short> random, List<BackdropOp> ops, short x, short y)
    {
        short dy = Int16Math.S(y - 1 + random(2));
        short dx = Int16Math.S(x - 1 + random(2));
        ops.Add(Op(BackdropOpKind.MaskedBlit, 0x30, 0x28, 0xD0, 0x4B, 7, 7, dx, dy));
    }

    /// <summary>
    /// [ORIGINAL] 192D:14FB(x, y, w, h) – plaque: filled rectangle, four screws (the first one in one of five cases a
    /// different, round one) and a two-pixel shadow below and left.
    /// </summary>
    public static List<BackdropOp> Plaque(Func<short, short> random, short x, short y, short w, short h)
    {
        var ops = new List<BackdropOp> { Op(BackdropOpKind.FilledRect, x, y, w, h, 2) };
        if (random(5) == 0)
        {
            ops.Add(Op(BackdropOpKind.Blit, 0x30, 0x3A, 9, 9, Int16Math.S(x + 4), Int16Math.S(y + 4)));
        }
        else
        {
            short dy = Int16Math.S(y + 3 + random(3));
            short dx = Int16Math.S(x + 3 + random(2));
            ops.Add(Screw(dx, dy));
        }
        {
            short dy = Int16Math.S(y + 3 + random(3));
            short dx = Int16Math.S(x + w - 14 - random(2));
            ops.Add(Screw(dx, dy));
        }
        {
            short dy = Int16Math.S(y + h - 14 - random(3));
            short dx = Int16Math.S(x + w - 14 - random(2));
            ops.Add(Screw(dx, dy));
        }
        {
            short dy = Int16Math.S(y + h - 14 - random(3));
            short dx = Int16Math.S(x + 3 + random(2));
            ops.Add(Screw(dx, dy));
        }
        ops.Add(Op(BackdropOpKind.HorizontalLine, x, Int16Math.S(y + h), Int16Math.S(w - 1), 0));
        ops.Add(Op(BackdropOpKind.HorizontalLine, x, Int16Math.S(y + h + 1), Int16Math.S(w - 1), 0));
        ops.Add(Op(BackdropOpKind.VerticalLine, Int16Math.S(x - 1), Int16Math.S(y + 1), Int16Math.S(h + 1), 0));
        ops.Add(Op(BackdropOpKind.VerticalLine, Int16Math.S(x - 2), Int16Math.S(y + 1), Int16Math.S(h + 1), 0));
        return ops;
    }

    private static BackdropOp Screw(short dx, short dy) => Op(BackdropOpKind.Blit, 0x30, 0x2F, 0x0B, 0x0B, dx, dy);

    private static BackdropOp Op(BackdropOpKind kind, int a, int b, int c, int d, int e = 0, int f = 0, int g = 0, int h = 0) =>
        new(kind, Int16Math.U(a), Int16Math.U(b), Int16Math.U(c), Int16Math.U(d), Int16Math.U(e), Int16Math.U(f), Int16Math.U(g),
            Int16Math.U(h));
}
