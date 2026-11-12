using Traders.Core.State;

namespace Traders.Core.Input;

/// <summary>
/// Joystick scaling 27C6:0268 (finding docs/routines/0E9E_000E_steuerung.md): the axis counts of the timing loop become signed
/// steps from the calibrated centre, the port byte becomes the two buttons. The calibration 27C6:00E6 writes the words used here.
/// </summary>
public static class JoystickReader
{
    // DS offsets of the calibration words per stick: centre x, centre y, step x below/above centre, step y below/above centre.
    public const int Stick1 = 0x1166, Stick2 = 0x1172;

    /// <summary>
    /// 27C6:0268(stick, &amp;x) after the measurement: x, y as signed bytes and AX with bit 0 = button 1, bit 1 = button 2
    /// (port bits 4/5 for stick 1, 6/7 for stick 2, active low). Every stick other than 1 reads the second set.
    /// </summary>
    // [ORIGINAL] 27C6:0268
    public static (sbyte X, sbyte Y, ushort Buttons) Read(Dgroup d, int stick, JoystickReading reading)
    {
        bool first = stick == 1;
        int cal = first ? Stick1 : Stick2;
        sbyte x = Axis(reading.CountX, d.U16(cal), below: d.U16(cal + 4), above: d.U16(cal + 6));
        sbyte y = Axis(reading.CountY, d.U16(cal + 2), below: d.U16(cal + 8), above: d.U16(cal + 10));
        int b1 = first ? 0x10 : 0x40, b2 = first ? 0x20 : 0x80;
        ushort buttons = 0;
        if ((reading.Port & b1) == 0) buttons |= 1;
        if ((reading.Port & b2) == 0) buttons |= 2;
        return (x, y, buttons);
    }

    /// <summary>
    /// Counts how often the step fits into the distance from the centre (8-bit counter, wraps), negated below the centre.
    /// </summary>
    private static sbyte Axis(ushort count, ushort centre, ushort below, ushort above)
    {
        if (count >= centre) return unchecked((sbyte)Steps((ushort)(count - centre), above));
        return unchecked((sbyte)-Steps((ushort)(centre - count), below));
    }

    // [APPROX] A step of 0 never ends the subtraction loop of the original (the game hangs with an uncalibrated joystick);
    // the port returns 0 steps instead.
    private static byte Steps(ushort distance, ushort step) => step == 0 ? (byte)0 : unchecked((byte)(distance / step));
}
