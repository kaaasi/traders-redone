using Traders.Core.State;

namespace Traders.Core.Startup;

/// <summary>Lengths of one picture file as the resource loader 186C:0997 stores them in the picture table.</summary>
/// <param name="FileLength">Length of the packed file (DOS filelength).</param>
/// <param name="UnpackedLength">The 24-bit unpacked length from the PowerPacker trailer (the three bytes four bytes before the end).</param>
public readonly record struct ImageFileLengths(uint FileLength, uint UnpackedLength);

/// <summary>
/// Picture table of the resource loader 186C in the data segment (finding docs/routines/186C_ressourcenlader.md): ten entries
/// of 13h bytes at DS:09CE (far pointers to the VGA and EGA file names, disk number, file length, unpacked length) and the
/// number of the picture loaded last, DS:09C8 (also the palette number of the EGA colour map 0A96).
/// </summary>
public static class ImageTable
{
    /// <summary>DS:09C8 – number of the picture loaded last (-1 before the first load).</summary>
    public const int CurrentImage = 0x09C8;

    /// <summary>DS:09CE – first entry.</summary>
    public const int Start = 0x09CE;

    public const int EntrySize = 0x13;

    public const int Count = 10;

    /// <summary>Entry +0Bh: length of the packed file (long, written once by 186C:0997).</summary>
    public const int FileLengthField = 0x0B;

    /// <summary>Entry +0Fh: unpacked length (long; 0 = picture not opened yet).</summary>
    public const int UnpackedLengthField = 0x0F;

    /// <summary>DS:08CE..08DA – picture numbers the program start uses (six preloaded pictures, then the code request picture).</summary>
    public const int PreloadNumbers = 0x08CE, CodeRequestNumber = 0x08DA;

    /// <summary>Order in which 0DFD:0239 loads the preloaded pictures (DS words 08CE, 08D0, 08D6, 08D8, 08D4, 08D2).</summary>
    public static IReadOnlyList<int> PreloadOrder { get; } = [0x08CE, 0x08D0, 0x08D6, 0x08D8, 0x08D4, 0x08D2];

    public static int Entry(int number) => Start + EntrySize * number;

    /// <summary>
    /// What loading picture <paramref name="number"/> with 186C:0002 changes in the picture table: on the first load
    /// (unpacked length 0) the two lengths (186C:0997), then the number of the picture loaded last. Unpacking, cache
    /// (DS:4C00), atlas, palette and objects are the output layers' and the host's.
    /// </summary>
    // [ORIGINAL] 186C:0002 – entry = DS:09CE + 13h·n; [+0Fh] = 0 → 186C:0997; DS:09C8 = n
    // [ORIGINAL] 186C:0997 – [+0Bh] = filelength, [+0Fh] = bytes −4..−2 of the file (big endian)
    public static void RecordLoad(Dgroup d, int number, Func<int, ImageFileLengths> files)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(number);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(number, Count);
        int entry = Entry(number);
        if (d.U32(entry + UnpackedLengthField) == 0)
        {
            var lengths = files(number);
            d.Set(entry + FileLengthField, lengths.FileLength);
            d.Set(entry + UnpackedLengthField, lengths.UnpackedLength);
        }
        d.Set(CurrentImage, (short)number);
    }
}
