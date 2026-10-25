using Traders.RefEmu.Cpu;

namespace Traders.RefEmu.Machine;

/// <summary>Parsed DOS MZ header.</summary>
public sealed record MzHeader(
    int LastPageBytes, int Pages, int RelocationCount, int HeaderParagraphs, int MinAlloc, int MaxAlloc,
    ushort InitialSs, ushort InitialSp, ushort InitialIp, ushort InitialCs, int RelocationTableOffset)
{
    /// <summary>Size of the load module (file image without header) in bytes.</summary>
    public int ImageSize => (LastPageBytes == 0 ? Pages * 512 : (Pages - 1) * 512 + LastPageBytes) - HeaderParagraphs * 16;

    public int ImageParagraphs => (ImageSize + 15) / 16;
}

/// <summary>Loads an MZ executable into memory and applies its segment relocations (like DOS EXEC, function 4B00h).</summary>
public static class MzLoader
{
    public static MzHeader ParseHeader(ReadOnlySpan<byte> exe)
    {
        if (exe.Length < 0x1C || exe[0] != 'M' || exe[1] != 'Z') throw new InvalidDataException("Not an MZ executable.");
        var w = new ushort[13];
        for (int i = 0; i < w.Length; i++) w[i] = (ushort)(exe[2 * i] | (exe[2 * i + 1] << 8));
        return new MzHeader(w[1], w[2], w[3], w[4], w[5], w[6], w[7], w[8], w[10], w[11], w[12]);
    }

    /// <summary>Copies the load module to <paramref name="loadSegment"/>:0 and relocates it. Returns the header.</summary>
    public static MzHeader Load(Memory mem, ReadOnlySpan<byte> exe, ushort loadSegment)
    {
        var h = ParseHeader(exe);
        int start = h.HeaderParagraphs * 16;
        int size = Math.Min(h.ImageSize, exe.Length - start);
        mem.WriteBlock(Memory.Linear(loadSegment, 0), exe.Slice(start, size));
        for (int i = 0; i < h.RelocationCount; i++)
        {
            int e = h.RelocationTableOffset + i * 4;
            ushort off = (ushort)(exe[e] | (exe[e + 1] << 8));
            ushort seg = (ushort)(exe[e + 2] | (exe[e + 3] << 8));
            ushort s = (ushort)(loadSegment + seg);
            mem.Write16(s, off, (ushort)(mem.Read16(s, off) + loadSegment));
        }
        return h;
    }
}
