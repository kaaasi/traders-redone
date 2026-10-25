using System.IO.Compression;

namespace Traders.Assets;

/// <summary>Minimal dependency-free PNG writer (8-bit indexed with palette) for asset export and image tests.</summary>
public static class PngWriter
{
    public static void WriteIndexed(Stream output, int width, int height, ReadOnlySpan<byte> pixels, Palette palette)
    {
        if (pixels.Length < width * height) throw new ArgumentException("Pixel buffer too small.", nameof(pixels));

        output.Write([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]);

        var ihdr = new byte[13];
        WriteBe(ihdr, 0, (uint)width);
        WriteBe(ihdr, 4, (uint)height);
        ihdr[8] = 8;  // bit depth
        ihdr[9] = 3;  // colour type: indexed
        WriteChunk(output, "IHDR", ihdr);

        var plte = new byte[768];
        for (int i = 0; i < 256; i++)
        {
            var (r, g, b) = palette.Rgb(i);
            plte[i * 3] = r; plte[i * 3 + 1] = g; plte[i * 3 + 2] = b;
        }
        WriteChunk(output, "PLTE", plte);

        using var raw = new MemoryStream();
        using (var z = new ZLibStream(raw, CompressionLevel.SmallestSize, leaveOpen: true))
        {
            for (int y = 0; y < height; y++)
            {
                z.WriteByte(0); // filter: none
                z.Write(pixels.Slice(y * width, width));
            }
        }
        WriteChunk(output, "IDAT", raw.ToArray());
        WriteChunk(output, "IEND", []);
    }

    public static void WriteIndexed(string path, int width, int height, ReadOnlySpan<byte> pixels, Palette palette)
    {
        using var fs = File.Create(path);
        WriteIndexed(fs, width, height, pixels, palette);
    }

    private static void WriteChunk(Stream s, string type, byte[] data)
    {
        var header = new byte[8];
        WriteBe(header, 0, (uint)data.Length);
        for (int i = 0; i < 4; i++) header[4 + i] = (byte)type[i];
        s.Write(header);
        s.Write(data);
        uint crc = Crc32(header.AsSpan(4, 4), 0xFFFFFFFFu);
        crc = Crc32(data, crc) ^ 0xFFFFFFFFu;
        var c = new byte[4];
        WriteBe(c, 0, crc);
        s.Write(c);
    }

    private static readonly uint[] CrcTable = BuildCrcTable();

    private static uint[] BuildCrcTable()
    {
        var t = new uint[256];
        for (uint n = 0; n < 256; n++)
        {
            uint c = n;
            for (int k = 0; k < 8; k++) c = (c & 1) != 0 ? 0xEDB88320u ^ (c >> 1) : c >> 1;
            t[n] = c;
        }
        return t;
    }

    private static uint Crc32(ReadOnlySpan<byte> data, uint crc)
    {
        foreach (var b in data) crc = CrcTable[(crc ^ b) & 0xFF] ^ (crc >> 8);
        return crc;
    }

    private static void WriteBe(byte[] buf, int offset, uint value)
    {
        buf[offset] = (byte)(value >> 24);
        buf[offset + 1] = (byte)(value >> 16);
        buf[offset + 2] = (byte)(value >> 8);
        buf[offset + 3] = (byte)value;
    }
}
