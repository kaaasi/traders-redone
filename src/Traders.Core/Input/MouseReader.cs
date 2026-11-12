using Traders.Core.State;

namespace Traders.Core.Input;

/// <summary>Mouse direction 0D73:07E0 (finding docs/routines/0E9E_000E_steuerung.md).</summary>
public static class MouseReader
{
    /// <summary>
    /// 0D73:07E0 – 0 without driver (DS:2BE6 = 0). Otherwise the motion since the last read decides one axis: horizontal when it
    /// moved and at least as far as vertically (right 1, left 2 beyond ±1), vertical when it moved further (down 8, up 4 beyond
    /// ±1); the left button adds 10h, the right button 20h.
    /// </summary>
    // [ORIGINAL] 0D73:07E0
    public static ushort ReadDirection(Dgroup d, IControlDevices devices)
    {
        if (d.U16(ControlReader.MouseDriver) == 0) return 0;
        var motion = devices.ReadMouseMotion();
        short ax = Abs(motion.X), ay = Abs(motion.Y);
        ushort result = 0;
        if (ax != 0 && ax >= ay)
        {
            if (motion.X > 1) result |= 0x01;
            else if (motion.X < -1) result |= 0x02;
        }
        if (ay > ax)
        {
            if (motion.Y > 1) result |= 0x08;
            else if (motion.Y < -1) result |= 0x04;
        }
        ushort buttons = devices.ReadMouseButtons();
        if ((buttons & 1) != 0) result |= 0x10;
        if ((buttons & 2) != 0) result |= 0x20;
        return result;
    }

    // cwd / xor / sub: 16-bit absolute value, −32768 stays negative.
    private static short Abs(short v) => Int16Math.S(v < 0 ? -v : v);
}
