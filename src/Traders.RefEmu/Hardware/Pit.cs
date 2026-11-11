namespace Traders.RefEmu.Hardware;

/// <summary>
/// 8253/8254 timer. Time is measured in PIT input clocks (1.193182 MHz) supplied by the machine,
/// which derives them from the executed instruction count, so all timing is deterministic.
/// </summary>
public sealed class Pit
{
    public const long ClockHz = 1193182;

    private sealed class Channel
    {
        public int Reload = 0x10000;
        public long LoadTime;
        public int AccessMode = 3;   // 1 lo, 2 hi, 3 lo/hi
        public int Mode = 3;
        public bool WriteHigh;       // next write is the high byte (lo/hi mode)
        public int PendingLow;
        public bool ReadHigh;
        public int? Latched;
    }

    private readonly Channel[] _ch = [new(), new(), new()];

    /// <summary>Called with the channel number whenever a new reload value takes effect.</summary>
    public event Action<int, int>? ReloadChanged;

    /// <summary>Current period of channel 0 in PIT clocks (65536 for a written 0).</summary>
    public int Channel0Period => _ch[0].Reload;

    /// <summary>Snapshot of all three channels; restoring does not raise <see cref="ReloadChanged"/>.</summary>
    internal void SaveState(BinaryWriter w)
    {
        foreach (var c in _ch)
        {
            w.Write(c.Reload);
            w.Write(c.LoadTime);
            w.Write(c.AccessMode);
            w.Write(c.Mode);
            w.Write(c.WriteHigh);
            w.Write(c.PendingLow);
            w.Write(c.ReadHigh);
            w.Write(c.Latched.HasValue);
            w.Write(c.Latched ?? 0);
        }
    }

    internal void LoadState(BinaryReader r)
    {
        foreach (var c in _ch)
        {
            c.Reload = r.ReadInt32();
            c.LoadTime = r.ReadInt64();
            c.AccessMode = r.ReadInt32();
            c.Mode = r.ReadInt32();
            c.WriteHigh = r.ReadBoolean();
            c.PendingLow = r.ReadInt32();
            c.ReadHigh = r.ReadBoolean();
            bool latched = r.ReadBoolean();
            int value = r.ReadInt32();
            c.Latched = latched ? value : null;
        }
    }

    public int Period(int channel) => _ch[channel].Reload;

    private int Count(Channel c, long now)
    {
        long elapsed = now - c.LoadTime;
        int period = c.Reload;
        int v = (int)(period - elapsed % period);
        if (c.Mode == 3) v = (v * 2) % 0x10000; // square wave counts down by two
        return v & 0xFFFF;
    }

    public byte Read(ushort port, long now)
    {
        if (port == 0x43) return 0xFF;
        var c = _ch[port - 0x40];
        int v = c.Latched ?? Count(c, now);
        byte result;
        if (c.AccessMode == 1) result = (byte)v;
        else if (c.AccessMode == 2) result = (byte)(v >> 8);
        else
        {
            result = c.ReadHigh ? (byte)(v >> 8) : (byte)v;
            c.ReadHigh = !c.ReadHigh;
            if (c.ReadHigh) return result; // latch held until the high byte was read
        }
        c.Latched = null;
        return result;
    }

    public void Write(ushort port, byte value, long now)
    {
        if (port == 0x43)
        {
            int sel = value >> 6;
            if (sel == 3) return; // read-back (8254) not used
            var c = _ch[sel];
            int access = (value >> 4) & 3;
            if (access == 0) { c.Latched = Count(c, now); c.ReadHigh = false; return; }
            c.AccessMode = access;
            c.Mode = (value >> 1) & 7;
            c.WriteHigh = false;
            c.ReadHigh = false;
            return;
        }
        int n = port - 0x40;
        var ch = _ch[n];
        int reload;
        switch (ch.AccessMode)
        {
            case 1: reload = value; break;
            case 2: reload = value << 8; break;
            default:
                if (!ch.WriteHigh) { ch.PendingLow = value; ch.WriteHigh = true; return; }
                ch.WriteHigh = false;
                reload = ch.PendingLow | (value << 8);
                break;
        }
        if (reload == 0) reload = 0x10000;
        ch.Reload = reload;
        ch.LoadTime = now;
        ReloadChanged?.Invoke(n, reload);
    }
}
