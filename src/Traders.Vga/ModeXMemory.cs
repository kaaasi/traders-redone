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

    /// <summary>CPU read at an offset inside the A000 window: loads all latches, returns the read-map plane.</summary>
    public byte Read(int offset)
    {
        offset &= 0xFFFF;
        for (int p = 0; p < 4; p++) _latch[p] = Planes[p][offset];
        return _latch[ReadMap & 3];
    }

    /// <summary>CPU write (write mode 0, bit mask FFh): value goes to every plane enabled in the map mask.</summary>
    public void Write(int offset, byte value)
    {
        offset &= 0xFFFF;
        for (int p = 0; p < 4; p++)
            if ((MapMask & (1 << p)) != 0) Planes[p][offset] = value;
    }

    /// <summary>CPU write with bit mask 00h: every enabled plane receives its latch (used for VRAM-to-VRAM copies).</summary>
    public void WriteLatches(int offset)
    {
        offset &= 0xFFFF;
        for (int p = 0; p < 4; p++)
            if ((MapMask & (1 << p)) != 0) Planes[p][offset] = _latch[p];
    }

    /// <summary>Memory behind the A000h window at an offset relative to A0000h (not modelled yet: always 0).</summary>
    public byte ReadOutside(int offset) => 0;

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
