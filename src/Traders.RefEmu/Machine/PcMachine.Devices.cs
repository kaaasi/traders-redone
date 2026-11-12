namespace Traders.RefEmu.Machine;

/// <summary>
/// Optional game devices for differential tests of the control reader (lane spielfeld, R4P-6): an analogue joystick at
/// port 201h, a mouse driver at int 33h and an adapter at the parallel port. Each device is off by default; the machine then
/// behaves as without it (201h and the LPT ports read FFh, int 33h returns AX = 0). Values are test inputs, not modelled timing.
/// </summary>
public sealed class GameDevices
{
    /// <summary>
    /// Joystick port model: after a write to 201h, axis bit i (0 = stick 1 x, 1 = stick 1 y, 2 = stick 2 x, 3 = stick 2 y)
    /// reads 1 for the first <c>AxisReads[i]</c> reads and 0 afterwards. Null = no joystick (201h reads FFh).
    /// </summary>
    public ushort[]? AxisReads { get; set; }

    /// <summary>Bits 4–7 of port 201h (buttons, active low) while a joystick is attached.</summary>
    public byte JoystickButtons { get; set; } = 0xF0;

    /// <summary>A mouse driver answers int 33h (function 0: installed, two buttons).</summary>
    public bool Mouse { get; set; }

    /// <summary>Motion counters returned by successive calls of function 0Bh; when empty the counters read 0.</summary>
    public Queue<(short X, short Y)> MouseMotions { get; } = new();

    /// <summary>BX of function 03h (bit 0 left, bit 1 right).</summary>
    public ushort MouseButtons { get; set; }

    /// <summary>Port base of the parallel adapter (data port; status port = base + 1).</summary>
    public ushort ParallelBase { get; set; } = 0x378;

    /// <summary>Status port as a function of the last byte written to the data port. Null = no adapter (unmapped).</summary>
    public Func<byte, byte>? ParallelStatus { get; set; }

    /// <summary>Bytes written to the data port, in order.</summary>
    public List<byte> ParallelWrites { get; } = [];

    /// <summary>Number of int 33h calls by function number.</summary>
    public Dictionary<ushort, int> MouseCalls { get; } = [];

    internal byte ParallelData;
    internal int ReadsSinceTrigger;
}

public sealed partial class PcMachine
{
    /// <summary>The game devices of <see cref="MachineOptions.Devices"/>.</summary>
    public GameDevices Devices => Options.Devices;

    private bool TryDeviceIn(ushort port, out byte value)
    {
        var d = Devices;
        if (port == 0x201 && d.AxisReads is { } axes)
        {
            int n = d.ReadsSinceTrigger;
            if (n < int.MaxValue) d.ReadsSinceTrigger = n + 1;
            int bits = 0;
            for (int i = 0; i < 4 && i < axes.Length; i++)
                if (n < axes[i]) bits |= 1 << i;
            value = (byte)((d.JoystickButtons & 0xF0) | bits);
            return true;
        }
        if (d.ParallelStatus is { } status && port == (ushort)(d.ParallelBase + 1))
        {
            value = status(d.ParallelData);
            return true;
        }
        value = 0;
        return false;
    }

    private bool TryDeviceOut(ushort port, byte value)
    {
        var d = Devices;
        if (port == 0x201 && d.AxisReads is not null)
        {
            d.ReadsSinceTrigger = 0;
            return true;
        }
        if (d.ParallelStatus is not null && port == d.ParallelBase)
        {
            d.ParallelData = value;
            d.ParallelWrites.Add(value);
            return true;
        }
        return false;
    }

    /// <summary>int 33h with <see cref="GameDevices.Mouse"/>: functions 0, 3 and 0Bh; others leave the registers unchanged.</summary>
    private void MouseDriver()
    {
        var d = Devices;
        var c = Cpu;
        d.MouseCalls[c.Ax] = d.MouseCalls.GetValueOrDefault(c.Ax) + 1;
        switch (c.Ax)
        {
            case 0x00:
                c.Ax = 0xFFFF;
                c.Bx = 2;
                return;
            case 0x03:
                c.Bx = d.MouseButtons;
                c.Cx = 0;
                c.Dx = 0;
                return;
            case 0x0B:
                var (x, y) = d.MouseMotions.Count > 0 ? d.MouseMotions.Dequeue() : ((short)0, (short)0);
                c.Cx = unchecked((ushort)x);
                c.Dx = unchecked((ushort)y);
                return;
            default:
                LogEvent(EventKind.Bios, $"int 33 AX={c.Ax:X4} ignored");
                return;
        }
    }
}
