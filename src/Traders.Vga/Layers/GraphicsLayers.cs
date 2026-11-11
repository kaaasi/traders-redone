using System.Buffers.Binary;

namespace Traders.Vga.Layers;

/// <summary>
/// Waiting for the vertical retrace (0D73:00D3: wait while bit 3 of port 3DAh is set, then until it is set again).
/// The layers never measure time themselves; every wait of the original is handed to this interface exactly once,
/// so a host can pace frames and a test can count the waits (AGENTS.md §4: no host clock in the layers).
/// </summary>
public interface IRetraceWait
{
    void WaitForRetrace();
}

/// <summary>Retrace wait that only counts (tests, headless use).</summary>
public sealed class RetraceCounter : IRetraceWait
{
    public int Count { get; private set; }

    public void WaitForRetrace() => Count++;
}

/// <summary>
/// The 256 colour registers of the VGA DAC as written through ports 3C8h/3C9h. Only the low six bits of each written
/// component are kept (VGA hardware; the RefEmu VgaAdapter does the same).
/// </summary>
public sealed class VgaDac
{
    public byte[] Registers { get; } = new byte[768];

    public void Write(byte index, byte r, byte g, byte b)
    {
        Registers[index * 3] = (byte)(r & 0x3F);
        Registers[index * 3 + 1] = (byte)(g & 0x3F);
        Registers[index * 3 + 2] = (byte)(b & 0x3F);
    }
}

/// <summary>
/// Graphics layers above the 23CD driver: page management (0B67), palettes and fades (21FD:026C…04DA), page flip
/// (21FD:051F) and colour registers (2295). The layers work on the byte image of the data segment (DGROUP) at the
/// original offsets, so their state can be compared with the RefEmu byte for byte; they touch no game state and draw
/// no random numbers. Card mode: only VGA (<c>DS:090E</c> = 1) is ported, like the driver.
/// </summary>
public sealed partial class GraphicsLayers
{
    /// <summary>DS:090E – display mode chosen at start (0 EGA, 1 VGA, 2/63h Tandy).</summary>
    public const int CardMode = 0x090E;

    public ModeXDriver Driver { get; }

    /// <summary>Data segment image (64 KB), shared with the caller.</summary>
    public byte[] Ds { get; }

    public VgaDac Dac { get; }

    public IRetraceWait Retrace { get; }

    public GraphicsLayers(ModeXDriver driver, byte[] ds, VgaDac dac, IRetraceWait retrace)
    {
        if (ds.Length < 0x10000) throw new ArgumentException("DGROUP image needs 64 KB.", nameof(ds));
        Driver = driver;
        Ds = ds;
        Dac = dac;
        Retrace = retrace;
    }

    private bool IsVga => U16(CardMode) == 1;

    private ushort U16(int offset) => BinaryPrimitives.ReadUInt16LittleEndian(Ds.AsSpan(offset & 0xFFFF));

    private void Set16(int offset, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(Ds.AsSpan(offset & 0xFFFF), value);

    private uint U32(int offset) => (uint)(U16(offset) | (U16(offset + 2) << 16));

    /// <summary>[ORIGINAL] 0D73:00D3: wait for the start of the next vertical retrace.</summary>
    public void WaitRetrace() => Retrace.WaitForRetrace();

    /// <summary>
    /// [ORIGINAL] 21FD:0185(count as long): <paramref name="count"/> retrace waits (0D73:00D3); the loop counter is
    /// a 32-bit value compared unsigned in its low word and signed in its high word, so counts ≤ 0 wait not at all.
    /// </summary>
    public void WaitRetraces(int count)
    {
        for (int i = 0; i < count; i++) WaitRetrace();
    }
}
