namespace Traders.RefEmu.Cpu;

/// <summary>A device that claims a window of the physical address space (e.g. VGA at A0000-BFFFF).</summary>
public interface IMemoryDevice
{
    byte Read(uint address);
    void Write(uint address, byte value);
}

/// <summary>
/// Physical memory of the emulated PC: 1 MB plus the 64 KB high memory area that a 286 can reach in
/// real mode. With <see cref="A20"/> off (DOS default) addresses wrap at 1 MB like on an 8086.
/// </summary>
public sealed class Memory
{
    public const int Size = 0x110000;

    public byte[] Ram { get; } = new byte[Size];

    /// <summary>A20 gate. False: addresses wrap at 1 MB.</summary>
    public bool A20 { get; set; }

    private IMemoryDevice? _device;
    private uint _devStart, _devEnd;

    /// <summary>Maps <paramref name="device"/> over [start, end).</summary>
    public void Map(IMemoryDevice device, uint start, uint end)
    {
        _device = device;
        _devStart = start;
        _devEnd = end;
    }

    private uint Wrap(uint a) => A20 ? (a < Size ? a : a % Size) : a & 0xFFFFF;

    public byte Read8(uint linear)
    {
        uint a = Wrap(linear);
        if (_device is not null && a >= _devStart && a < _devEnd) return _device.Read(a);
        return Ram[a];
    }

    public void Write8(uint linear, byte value)
    {
        uint a = Wrap(linear);
        if (_device is not null && a >= _devStart && a < _devEnd) { _device.Write(a, value); return; }
        Ram[a] = value;
    }

    public static uint Linear(ushort seg, ushort off) => ((uint)seg << 4) + off;

    public byte Read8(ushort seg, ushort off) => Read8(Linear(seg, off));
    public void Write8(ushort seg, ushort off, byte v) => Write8(Linear(seg, off), v);

    /// <summary>Word access helper for host code (wraps the offset at 64 KB, no fault).</summary>
    public ushort Read16(ushort seg, ushort off) =>
        (ushort)(Read8(seg, off) | (Read8(seg, (ushort)(off + 1)) << 8));

    public void Write16(ushort seg, ushort off, ushort v)
    {
        Write8(seg, off, (byte)v);
        Write8(seg, (ushort)(off + 1), (byte)(v >> 8));
    }

    public byte[] ReadBlock(ushort seg, ushort off, int count)
    {
        var b = new byte[count];
        uint lin = Linear(seg, off);
        for (int i = 0; i < count; i++) b[i] = Read8((uint)(lin + i));
        return b;
    }

    public void WriteBlock(uint linear, ReadOnlySpan<byte> data)
    {
        for (int i = 0; i < data.Length; i++) Write8((uint)(linear + i), data[i]);
    }

    /// <summary>Reads a zero-terminated string (max 256 chars).</summary>
    public string ReadAsciiz(ushort seg, ushort off)
    {
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < 256; i++)
        {
            byte c = Read8(seg, (ushort)(off + i));
            if (c == 0) break;
            sb.Append((char)c);
        }
        return sb.ToString();
    }
}

/// <summary>Port I/O as seen by the CPU.</summary>
public interface IIoBus
{
    byte In8(ushort port);
    void Out8(ushort port, byte value);
}

/// <summary>I/O bus without devices: reads return 0xFF (open bus), writes are ignored.</summary>
public sealed class OpenIoBus : IIoBus
{
    public byte In8(ushort port) => 0xFF;
    public void Out8(ushort port, byte value) { }
}
