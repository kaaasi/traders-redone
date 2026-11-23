using System.Buffers.Binary;

namespace Traders.Audio.Effects;

/// <summary>
/// Memory image of the effect machine: the 64 KiB window at code segment 0000 of T.EXE. The effect machine runs with
/// DS = CS = segment 0000, so its code, tables (jump table CS:151E, start table pointer CS:15B2, voice block pointers CS:06C9,
/// channel offsets CS:06DD, frequencies CS:06E6, pitch tables CS:020C, instruments CS:1B0D), the sound sequences and its
/// variables (CS:03EC–06FD) all live in this window. The tables are read from T.EXE at run time (R9PL-3), never copied into code.
/// </summary>
public static class EffectImage
{
    /// <summary>Size of the window: one real-mode segment.</summary>
    public const int Size = 0x10000;

    /// <summary>
    /// The window from an MZ executable (T.EXE): the first 64 KiB of the load image (segment 0000 starts at the image start),
    /// unrelocated. No relocation entry falls into the effect machine's code, tables or variables (CS:0208–2683).
    /// </summary>
    public static byte[] FromExe(ReadOnlySpan<byte> exe)
    {
        if (exe.Length < 0x1C || exe[0] != (byte)'M' || exe[1] != (byte)'Z')
            throw new ArgumentException("Not an MZ executable.", nameof(exe));
        int header = BinaryPrimitives.ReadUInt16LittleEndian(exe[8..]) * 16;
        var bytes = new byte[Size];
        int available = Math.Clamp(exe.Length - header, 0, Size);
        exe.Slice(header, available).CopyTo(bytes);
        return bytes;
    }
}
