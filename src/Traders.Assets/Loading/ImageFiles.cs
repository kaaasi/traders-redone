namespace Traders.Assets.Loading;

/// <summary>
/// File lengths of the pictures as the resource loader 186C:0997 reads them: the length of the packed file and the unpacked
/// length from the PowerPacker trailer (finding docs/routines/186C_ressourcenlader.md).
/// </summary>
public static class ImageFiles
{
    /// <summary>(file length, unpacked length) of one PP20 file: bytes −4..−2 as a 24-bit big-endian number.</summary>
    public static (uint FileLength, uint UnpackedLength) Lengths(ReadOnlySpan<byte> file)
    {
        if (file.Length < 4) throw new InvalidDataException("picture file shorter than the PowerPacker trailer");
        int t = file.Length - 4;
        return ((uint)file.Length, (uint)(file[t] << 16 | file[t + 1] << 8 | file[t + 2]));
    }

    /// <summary>Lengths of picture <paramref name="number"/> of <see cref="ResourceTable.Pictures"/> (VGA file) read through <paramref name="load"/>.</summary>
    public static (uint FileLength, uint UnpackedLength) Lengths(int number, Func<string, byte[]> load) =>
        Lengths(load(ResourceTable.Pictures[number].Vga));
}
