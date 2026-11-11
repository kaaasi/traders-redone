namespace Traders.Vga;

/// <summary>
/// The four 64 KB planes of VGA memory as T.EXE uses them in "Mode X" (unchained 256 colours,
/// 80 bytes per row and plane). Only the register effects the original driver relies on are
/// modelled: sequencer map mask (index 2), graphics-controller read map (index 4) and the
/// four latches that every read loads. All other graphics-controller registers stay at the
/// values the driver resets them to (23CD:30D1): plain write mode 0, bit mask FFh.
/// </summary>
public sealed class ModeXMemory
{
    public const int PlaneSize = 0x10000;
    public const int BytesPerRow = 80;

    public byte[][] Planes { get; } = [new byte[PlaneSize], new byte[PlaneSize], new byte[PlaneSize], new byte[PlaneSize]];

    /// <summary>Sequencer register 2: planes a CPU write goes to (low 4 bits).</summary>
    public byte MapMask { get; set; } = 0x0F;

    /// <summary>Graphics controller register 4: plane a CPU read returns.</summary>
    public byte ReadMap { get; set; }

    private readonly byte[] _latch = new byte[4];

    /// <summary>
    /// Bytes written outside the A000h window (offsets relative to A0000h, i.e. physical address − A0000h).
    /// [APPROX] Draw offsets that run past AFFFFh (or a page segment outside A000h) leave video memory and reach
    /// ordinary RAM in T.EXE's machine; the port keeps that RAM here, initially zero, as the RefEmu does for memory
    /// nothing is loaded into (B0000h and above). It is not shared with the atlas or any other buffer of the port.
    /// </summary>
    private readonly Dictionary<int, byte> _outside = [];

    private static bool InWindow(int offset) => (uint)offset < PlaneSize;

    /// <summary>
    /// Window offset of the far address <paramref name="segment"/>:<paramref name="offset"/>: physical address
    /// (wrapping at 1 MB, A20 off) minus A0000h; outside the window when not in [0, 10000h).
    /// </summary>
    public static int WindowOffset(ushort segment, ushort offset) => ((((segment << 4) + offset) & 0xFFFFF) - 0xA0000);

    /// <summary>
    /// CPU read at a window offset. Inside the window: loads all latches, returns the read-map plane. Outside: the
    /// RAM byte there (latches unchanged).
    /// </summary>
    public byte Read(int offset)
    {
        if (!InWindow(offset)) return ReadOutside(offset);
        for (int p = 0; p < 4; p++) _latch[p] = Planes[p][offset];
        return _latch[ReadMap & 3];
    }

    /// <summary>CPU write (write mode 0, bit mask FFh): value goes to every plane enabled in the map mask.</summary>
    public void Write(int offset, byte value)
    {
        if (!InWindow(offset)) { _outside[offset] = value; return; }
        for (int p = 0; p < 4; p++)
            if ((MapMask & (1 << p)) != 0) Planes[p][offset] = value;
    }

    /// <summary>
    /// CPU write of <paramref name="value"/> with bit mask 00h: every enabled plane receives its latch (used for
    /// VRAM-to-VRAM copies); outside the window the RAM byte receives the CPU value.
    /// </summary>
    public void WriteLatches(int offset, byte value)
    {
        if (!InWindow(offset)) { _outside[offset] = value; return; }
        for (int p = 0; p < 4; p++)
            if ((MapMask & (1 << p)) != 0) Planes[p][offset] = _latch[p];
    }

    /// <summary>RAM byte outside the A000h window at a window offset (0 where nothing was written).</summary>
    public byte ReadOutside(int offset) => _outside.GetValueOrDefault(offset);

    /// <summary>Pixel at (x, y) of a page whose plane offset is <paramref name="pageOffset"/>.</summary>
    public byte Pixel(int pageOffset, int x, int y) =>
        Planes[x & 3][(pageOffset + y * BytesPerRow + (x >> 2)) & 0xFFFF];

    /// <summary>Linear 320×200 copy of a page (index = y·320 + x).</summary>
    public byte[] PageToLinear(int pageOffset, int height = 200)
    {
        var result = new byte[320 * height];
        for (int y = 0; y < height; y++)
            for (int x = 0; x < 320; x++)
                result[y * 320 + x] = Pixel(pageOffset, x, y);
        return result;
    }
}

/// <summary>
/// Picture atlas in RAM as the driver addresses it: the buffer starts with the 4-byte size
/// header of the V-*.PP file, then 256 rows of 320 bytes, each row split by plane
/// (bytes p·80 … p·80+79 hold pixels x = 4j + p). Conversion as in 23CD:3079.
/// </summary>
public sealed class PlanarAtlas
{
    public const int Width = 320, Height = 256, HeaderSize = 4;
    public const int Length = HeaderSize + Width * Height;

    /// <summary>Raw buffer (header + plane-split rows). Driver address = paragraph·16 + offset into this array.</summary>
    public byte[] Data { get; }

    public PlanarAtlas(byte[] data)
    {
        if (data.Length < Length) throw new ArgumentException($"Atlas buffer needs {Length} bytes.", nameof(data));
        Data = data;
    }

    public PlanarAtlas() : this(new byte[Length]) { }

    /// <summary>
    /// Bytes behind the end of <see cref="Data"/> that the driver wrote. [APPROX] The driver addresses the atlas with a
    /// paragraph per row and a 16-bit offset, so runs of up to 64 KB (narrow widths, many rows) leave the buffer; in
    /// T.EXE's machine that is whatever memory follows it. The port models it as RAM that holds zero until written,
    /// as the RefEmu does where nothing is loaded behind the atlas.
    /// </summary>
    private readonly Dictionary<int, byte> _behind = [];

    /// <summary>Byte at driver address <paramref name="index"/> (paragraph·16 + offset), also behind the buffer.</summary>
    public byte Read(int index) => (uint)index < (uint)Data.Length ? Data[index] : _behind.GetValueOrDefault(index);

    /// <summary>Writes the byte at driver address <paramref name="index"/>, also behind the buffer.</summary>
    public void Write(int index, byte value)
    {
        if ((uint)index < (uint)Data.Length) Data[index] = value;
        else _behind[index] = value;
    }

    /// <summary>
    /// [ORIGINAL] 23CD:3079 – port of 3079(buf, rowBytes, rows): every row is copied to a scratch buffer and written
    /// back plane by plane (scratch[0], scratch[4], …, then scratch[1], scratch[5], …).
    /// </summary>
    public static void ConvertToPlanarRows(byte[] buffer, int start, int rowBytes, int rows)
    {
        var scratch = new byte[rowBytes];
        int row = start;
        do
        {
            Array.Copy(buffer, row, scratch, 0, rowBytes & ~1); // rep movsw, cx = rowBytes/2
            int si = row;
            for (int plane = 0; plane < 4; plane++)
                for (int i = 0, di = plane; i < rowBytes >> 2; i++, di += 4)
                    buffer[si++] = scratch[di];
            row += (rowBytes >> 4) << 4; // ds += rowBytes/16 paragraphs
        } while (--rows != 0);
    }

    /// <summary>Builds the driver atlas from a decoded V-*.PP picture (header 256/320 + converted rows).</summary>
    public static PlanarAtlas FromLinear(ReadOnlySpan<byte> pixels)
    {
        if (pixels.Length != Width * Height) throw new ArgumentException("Need 320×256 pixels.", nameof(pixels));
        var data = new byte[Length];
        data[0] = 0x00; data[1] = 0x01; data[2] = 0x40; data[3] = 0x01;
        pixels.CopyTo(data.AsSpan(HeaderSize));
        ConvertToPlanarRows(data, HeaderSize, Width, Height);
        return new PlanarAtlas(data);
    }

    /// <summary>Pixel (x, y) of the converted atlas.</summary>
    public byte Pixel(int x, int y) => Data[HeaderSize + y * Width + (x & 3) * 80 + (x >> 2)];
}

/// <summary>
/// One entry of the font table the driver reads at CS:057A (10 bytes each in the original:
/// far glyph pointer, height, advance, first character). Glyphs are 16 pixels wide, two bytes
/// (big-endian, MSB = left pixel) per row.
/// </summary>
public sealed record FontDescriptor(byte[] Data, int Offset, ushort Height, ushort Advance, ushort FirstChar);
