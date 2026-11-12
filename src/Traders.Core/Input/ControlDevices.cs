namespace Traders.Core.Input;

/// <summary>
/// One joystick measurement as the original takes it: the axis counts of the timing loop 27C6:01D5 (BX = x, CX = y: number
/// of reads of port 201h during which the axis bit was still set after the trigger write, at most 15 536) and the port byte
/// read right after the loop (buttons in bits 4–7, active low).
/// </summary>
public readonly record struct JoystickReading(ushort CountX, ushort CountY, byte Port);

/// <summary>Motion counters of the mouse driver since the last read (int 33h function 0Bh: CX horizontal, DX vertical).</summary>
public readonly record struct MouseMotion(short X, short Y);

/// <summary>
/// Device values the control reader needs besides the key table DS:3D47 (decision F3 = A: the host offers only the keyboard
/// for now, the other devices are explicit inputs). Calls arrive in the order of the original, once per human player that
/// uses the device, so a host or a test can reproduce counters that are consumed by reading.
/// </summary>
public interface IControlDevices
{
    /// <summary>27C6:01D5 and the port read after it for joystick <paramref name="stick"/> (1 or 2).</summary>
    JoystickReading ReadJoystick(int stick);

    /// <summary>int 33h function 0Bh (only called when the driver flag DS:2BE6 is set).</summary>
    MouseMotion ReadMouseMotion();

    /// <summary>int 33h function 03h: BX (bit 0 left, bit 1 right button).</summary>
    ushort ReadMouseButtons();

    /// <summary>
    /// Writes <paramref name="data"/> to the data port of LPT1 (base from 0040:0008) and returns the status port (base + 1).
    /// </summary>
    byte ReadParallelStatus(byte data);
}

/// <summary>
/// Devices of a PC with keyboard only: the joystick port floats high (both axes count to the loop limit, no button), the
/// parallel status reads FFh (no adapter), the mouse never moves.
/// </summary>
public sealed class KeyboardOnlyDevices : IControlDevices
{
    public static readonly KeyboardOnlyDevices Instance = new();

    /// <summary>Iterations of the timing loop 27C6:01D5 (SI from C350h up to the wrap at 10000h).</summary>
    public const ushort JoystickLoopLimit = 0x10000 - 0xC350;

    public JoystickReading ReadJoystick(int stick) => new(JoystickLoopLimit, JoystickLoopLimit, 0xFF);

    public MouseMotion ReadMouseMotion() => default;

    public ushort ReadMouseButtons() => 0;

    public byte ReadParallelStatus(byte data) => 0xFF;
}
