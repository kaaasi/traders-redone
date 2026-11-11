namespace Traders.Vga;

/// <summary>Line routines of the VGA driver part (vectors 1C and 4C, dispatcher 23CD:06BE).</summary>
public sealed partial class ModeXDriver
{
    /// <summary>
    /// [ORIGINAL] 23CD:06BE: draw a line from (x1, y1) to (x2, y2) in the current colour; solid when the
    /// line pattern is FFFFh (vector 1C), otherwise patterned (vector 4C).
    /// </summary>
    public void DrawLine(ushort x1, ushort y1, ushort x2, ushort y2)
    {
        if (LinePattern == 0xFFFF) DrawLineSolid(x1, y1, x2, y2);
        else DrawLinePatterned(x1, y1, x2, y2);
    }

    /// <summary>[ORIGINAL] 23CD:378B (vector 1C): solid line, both end points included.</summary>
    public void DrawLineSolid(ushort x1, ushort y1, ushort x2, ushort y2) => Line(x1, y1, x2, y2, null);

    /// <summary>
    /// [ORIGINAL] 23CD:38F2 (vector 4C): line with the 16-bit pattern of CS:04F4; the pattern is rotated left
    /// once per pixel and the bit rotated out decides whether the pixel is drawn. It restarts with every call.
    /// </summary>
    public void DrawLinePatterned(ushort x1, ushort y1, ushort x2, ushort y2) => Line(x1, y1, x2, y2, LinePattern);

    /// <summary>Common body of 378B and 38F2; both have the same three branches and differ only in the pattern test.</summary>
    private void Line(ushort x1, ushort y1, ushort x2, ushort y2, ushort? pattern)
    {
        ushort bits = pattern ?? 0xFFFF;
        bool Next()
        {
            // rol reg,1; jnc skip – the solid routine has no test, a solid pattern draws every pixel as well
            bool cf = (bits & 0x8000) != 0;
            bits = (ushort)((bits << 1) | (bits >> 15));
            return pattern is null || cf;
        }
        byte color = (byte)Color;

        if (x1 == x2)
        {
            // vertical: unsigned compare of the y values (sub, jnc)
            ushort di = PageAddr(x1, y1);
            Vram.MapMask = (byte)(1 << (x1 & 3));
            ushort step = 80;
            ushort count = (ushort)(y2 - y1);
            if (y2 < y1)
            {
                count = (ushort)-count;
                step = unchecked((ushort)-80);
            }
            count++;
            do
            {
                if (Next()) Vram.Write(Page(di), color);
                di = (ushort)(di + step);
            } while (--count != 0);
        }
        else if (y1 == y2)
        {
            // horizontal: unsigned compare of the x values; drawing starts at the smaller x
            ushort start = x1;
            ushort count = (ushort)(x2 - x1);
            if (x2 < x1)
            {
                count = (ushort)-count;
                start = x2;
            }
            count++;
            ushort di = PageAddr(start, y1);
            byte planes = Rol8((byte)0x11, start & 3);
            do
            {
                if (Next())
                {
                    Vram.MapMask = (byte)(planes & 0x0F);
                    Vram.Write(Page(di), color);
                }
                planes = Rol8(planes, 1, out bool cf);
                if (cf) di++;
            } while (--count != 0);
        }
        else
        {
            // Bresenham with signed 16-bit differences (sub, jns); points are swapped so that x grows
            ushort ystep = 80;
            short dx = (short)(x2 - x1);
            if (dx < 0)
            {
                dx = (short)-dx;
                (x1, x2) = (x2, x1);
                (y1, y2) = (y2, y1);
            }
            short dy = (short)(y2 - y1);
            if (dy < 0)
            {
                dy = (short)-dy;
                ystep = unchecked((ushort)-80);
            }
            bool yMajor = dy > dx;
            short major = dx, minor = dy;
            if (yMajor) (major, minor) = (minor, major);
            short incMinorOnly = (short)(minor << 1);
            short err = (short)(incMinorOnly - major);
            short incBoth = (short)(err - major);
            ushort di = PageAddr(x1, y1);
            byte planes = Rol8((byte)0x11, x1 & 3);
            ushort count = (ushort)(major + 1);
            do
            {
                if (!yMajor)
                {
                    if (Next())
                    {
                        Vram.MapMask = (byte)(planes & 0x0F);
                        Vram.Write(Page(di), color);
                    }
                    planes = Rol8(planes, 1, out bool cf);
                    if (cf) di++;
                    if (err < 0) err = (short)(err + incMinorOnly);
                    else
                    {
                        err = (short)(err + incBoth);
                        di = (ushort)(di + ystep);
                    }
                }
                else
                {
                    Vram.MapMask = (byte)(planes & 0x0F);
                    if (Next()) Vram.Write(Page(di), color);
                    di = (ushort)(di + ystep);
                    if (err < 0) err = (short)(err + incMinorOnly);
                    else
                    {
                        err = (short)(err + incBoth);
                        planes = Rol8(planes, 1, out bool cf);
                        if (cf) di++;
                    }
                }
            } while (--count != 0);
        }
        Vram.MapMask = 0x0F;
    }

    private static byte Rol8(byte v, int n) => (byte)((v << (n & 7)) | (v >> ((8 - n) & 7)));

    private static byte Rol8(byte v, int n, out bool carry)
    {
        byte r = Rol8(v, n);
        carry = (r & 1) != 0;
        return r;
    }
}
