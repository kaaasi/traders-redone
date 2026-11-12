using Traders.Core.State;

namespace Traders.Core.Input;

/// <summary>Direction and fire bits of DS:37C3+i.</summary>
[Flags]
public enum ControlBits : byte
{
    None = 0,
    Right = 0x01,
    Left = 0x02,
    Up = 0x04,
    Down = 0x08,
    Fire = 0x10,
}

/// <summary>Action code of DS:3308+i as written by 0E9E:037A.</summary>
public enum ControlAction : sbyte
{
    None = -1,
    Right = 0,
    Left = 1,
    Up = 2,
    Down = 3,
    Fire = 10,
}

/// <summary>
/// Control reader 0E9E (finding docs/routines/0E9E_000E_steuerung.md): turns the selected control type of every human player
/// into direction and fire bits, the single flags with their repeat delays, and an action code. Works on the data segment like
/// the original; the key table DS:3D47 is written by the host input, the other devices come from <see cref="IControlDevices"/>.
/// </summary>
public static class ControlReader
{
    // DS offsets (docs/analyse/DATENLAYOUT.md, section of lane spielfeld).
    public const int ControlType = 0x083C;   // byte[4] 0/1 joystick 1/2, 2 mouse, 3 letter keys, 4 cursor keys, 5 LPT adapter
    public const int IsComputer = 0x0840;    // byte[4]
    public const int KeyTable = 0x3D47;      // byte[128] per set-1 make code
    public const int Bits = 0x37C3;          // byte[4] ControlBits
    public const int MoveDelay = 0x3786;     // long[4]
    public const int FireDelay = 0x302E;     // long[4]
    public const int FlagRight = 0x44BE, FlagLeft = 0x43F0, FlagUp = 0x439D, FlagDown = 0x44BA, FlagFire = 0x4894; // byte[4]
    public const int Action = 0x3308;        // sbyte[4] ControlAction
    public const int PlayerIndex = 0x2C10;   // word, loop variable of 000E (stays 4)
    public const int MouseDriver = 0x2BE6;   // word, AX of int 33h function 0 (0D73:00F8)

    /// <summary>Delay armed by a reported fire action (0E9E:03EB): held fire is suppressed until released.</summary>
    public const int FireRepeatDelay = 100_000;

    /// <summary>
    /// 0E9E:037A – reads all controls (<see cref="ReadAll"/>), then sets the action of every player: −1 for computers and idle
    /// humans, otherwise fire before up, down, right, left. A fire action arms the fire delay.
    /// </summary>
    // [ORIGINAL] 0E9E:037A
    public static void ReadActions(Dgroup d, IControlDevices devices)
    {
        ReadAll(d, devices);
        for (int i = 0; i < DgroupLayout.PlayerCount; i++)
        {
            d.Set(Action + i, (sbyte)ControlAction.None);
            if (d.U8(IsComputer + i) == 1) continue;
            var action = ControlAction.None;
            if (d.U8(FlagFire + i) != 0) action = ControlAction.Fire;
            else if (d.U8(FlagUp + i) != 0) action = ControlAction.Up;
            else if (d.U8(FlagDown + i) != 0) action = ControlAction.Down;
            else if (d.U8(FlagRight + i) != 0) action = ControlAction.Right;
            else if (d.U8(FlagLeft + i) != 0) action = ControlAction.Left;
            if (action == ControlAction.Fire) d.Set(FireDelay + 4 * i, FireRepeatDelay);
            d.Set(Action + i, (sbyte)action);
        }
    }

    /// <summary>0E9E:000E – bits of all players (<see cref="ReadBits"/>), then the flags of players 0..3 via DS:2C10.</summary>
    // [ORIGINAL] 0E9E:000E
    public static void ReadAll(Dgroup d, IControlDevices devices)
    {
        ReadBits(d, devices);
        for (d.Set(PlayerIndex, (short)0); d.S16(PlayerIndex) < 4; d.Set(PlayerIndex, (short)(d.S16(PlayerIndex) + 1)))
        {
            UpdateMoveFlags(d);
            UpdateFireFlag(d);
        }
    }

    /// <summary>
    /// 0E9E:0035 – for every player whose DS:0840 is not 1, the bits of the selected control type (types above 5 or negative
    /// give 0). Computer bits are left as their routines wrote them.
    /// </summary>
    // [ORIGINAL] 0E9E:0035
    public static void ReadBits(Dgroup d, IControlDevices devices)
    {
        for (int i = 0; i < DgroupLayout.PlayerCount; i++)
        {
            if (d.U8(IsComputer + i) == 1) continue;
            byte bits = 0;
            // cbw, then an unsigned compare with 5: negative types fall through like 6..127.
            switch ((ushort)d.S8(ControlType + i))
            {
                case 0:
                case 1:
                    bits = JoystickBits(d, devices, d.U8(ControlType + i) == 0 ? 1 : 2);
                    break;
                case 2:
                    bits = (byte)(MouseReader.ReadDirection(d, devices) & 0x1F);
                    if (Key(d, 0x7E)) bits |= (byte)ControlBits.Fire;
                    break;
                case 3:
                    bits = KeyBits(d, right: 0x2D, left: 0x2C, up: 0x10, down: 0x1E, fire: 0x39);
                    break;
                case 4:
                    bits = KeyBits(d, right: 0x4D, left: 0x4B, up: 0x48, down: 0x50, fire: 0x52);
                    break;
                case 5:
                    bits = ParallelBits(devices, i);
                    break;
            }
            d.Set(Bits + i, bits);
        }
    }

    private static bool Key(Dgroup d, int scancode) => d.U8(KeyTable + scancode) != 0;

    // [ORIGINAL] 0E9E:0035 – cases at 006F (cursor keys) and 00C1 (letter keys): right before left, up before down.
    private static byte KeyBits(Dgroup d, int right, int left, int up, int down, int fire)
    {
        byte bits = 0;
        if (Key(d, right)) bits += 1;
        else if (Key(d, left)) bits += 2;
        if (Key(d, up)) bits += 4;
        else if (Key(d, down)) bits += 8;
        if (Key(d, fire)) bits += 0x10;
        return bits;
    }

    // [ORIGINAL] 0E9E:0035 – case at 0135: only the axis with the larger magnitude counts, on a tie the y axis; button 1 fires.
    private static byte JoystickBits(Dgroup d, IControlDevices devices, int stick)
    {
        var (x, y, buttons) = JoystickReader.Read(d, stick, devices.ReadJoystick(stick));
        byte bits = 0;
        if (Math.Abs((int)x) > Math.Abs((int)y))
        {
            if (x > 1) bits += 1;
            else if (x < -1) bits += 2;
        }
        else if (y < 0) bits += 4;
        else if (y > 1) bits += 8;
        if ((buttons & 1) != 0) bits += 0x10;
        return bits;
    }

    // [ORIGINAL] 0E9E:0035 – case at 01BF: select line ~(1 << player) on the data port, inputs of the status port
    // (active low except down).
    private static byte ParallelBits(IControlDevices devices, int player)
    {
        byte status = devices.ReadParallelStatus(unchecked((byte)~(1 << player)));
        byte bits = 0;
        if ((status & 0x80) != 0) bits = 8;
        if ((status & 0x40) == 0) bits += 4;
        if ((status & 0x20) == 0) bits += 2;
        if ((status & 0x10) == 0) bits += 1;
        if ((status & 0x08) == 0) bits += 0x10;
        return bits;
    }

    /// <summary>
    /// 0E9E:0264 – direction flags of player DS:2C10. Without a direction a human's move delay is cleared; otherwise a positive
    /// delay is counted down and suppresses the directions (computers count down even without bits).
    /// </summary>
    // [ORIGINAL] 0E9E:0264
    public static void UpdateMoveFlags(Dgroup d)
    {
        int p = d.U16(PlayerIndex);
        int dirs = d.U8(Bits + p) & 0x0F;
        int delay = MoveDelay + 4 * p;
        if (dirs == 0 && d.U8(IsComputer + p) == 0) d.Set(delay, 0);
        else if (d.S32(delay) > 0)
        {
            d.Set(delay, d.S32(delay) - 1);
            dirs = 0;
        }
        d.Set(FlagRight + p, (byte)(dirs & 1));
        d.Set(FlagLeft + p, (byte)((dirs >> 1) & 1));
        d.Set(FlagUp + p, (byte)((dirs >> 2) & 1));
        d.Set(FlagDown + p, (byte)((dirs >> 3) & 1));
    }

    /// <summary>0E9E:0312 – fire flag of player DS:2C10 with its delay (cleared when fire is released).</summary>
    // [ORIGINAL] 0E9E:0312
    public static void UpdateFireFlag(Dgroup d)
    {
        int p = d.U16(PlayerIndex);
        int delay = FireDelay + 4 * p;
        byte fire = (byte)((d.U8(Bits + p) & 0x10) != 0 ? 1 : 0);
        if (fire == 0) d.Set(delay, 0);
        else if (d.S32(delay) > 0)
        {
            d.Set(delay, d.S32(delay) - 1);
            fire = 0;
        }
        d.Set(FlagFire + p, fire);
    }

    /// <summary>State of one player after <see cref="ReadActions"/> (the result the phases consume).</summary>
    public static PlayerControl Result(Dgroup d, int player) => new(
        player,
        (ControlBits)d.U8(Bits + player),
        d.U8(FlagRight + player) != 0,
        d.U8(FlagLeft + player) != 0,
        d.U8(FlagUp + player) != 0,
        d.U8(FlagDown + player) != 0,
        d.U8(FlagFire + player) != 0,
        (ControlAction)d.S8(Action + player));
}

/// <summary>Bits, single flags and action code of one player as left in the data segment by 0E9E:037A.</summary>
public readonly record struct PlayerControl(
    int Player, ControlBits Bits, bool Right, bool Left, bool Up, bool Down, bool Fire, ControlAction Action)
    : Phases.IPlayerCommand;
