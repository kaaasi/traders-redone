using System.Security.Cryptography;

namespace Traders.Assets;

/// <summary>
/// Unpacked load image of <c>FXDRIVER.EXE</c> V1.06 (Linel's resident music driver, LZEXE 0.91 packed). The music player
/// reads the driver's tables (note frequencies, operator offsets, default voice parameters, ...) from this image at run
/// time instead of keeping copies in the code (R9PL-3). Offsets are byte offsets in the load image: offset 0 is the first
/// byte after the MZ header of the unpacked program, i.e. the load segment (PSP + 10h). The FX code markers (tag
/// ORIGINAL-FX with a four-digit offset) use the same offsets (docs/analyse/FXDRIVER.md §1).
/// </summary>
public sealed class FxDriverImage
{
    /// <summary>Paragraph of the resident driver's code segment (int 64h entry, timer handler, AdLib layer).</summary>
    /// <remarks>[ORIGINAL-FX] 427D – the int 64h vector points to CS:195D of this segment.</remarks>
    public const ushort CodeParagraph = 0x0292;

    /// <summary>Image offset of the relocated word that loads the driver's data segment (<c>mov bp,0481h</c> at the int 64h entry).</summary>
    /// <remarks>[ORIGINAL-FX] 4287 – operand of <c>mov bp,0481h</c> at CS:1966; every routine of the driver loads DS this way.</remarks>
    public const int DataSegmentOperand = CodeParagraph * 16 + 0x1967;

    private readonly byte[] _image;

    private FxDriverImage(byte[] image, IReadOnlyList<(ushort Segment, ushort Offset)> relocations,
        ushort cs, ushort ip, ushort ss, ushort sp)
    {
        _image = image;
        Relocations = relocations;
        (EntryCs, EntryIp, StackSs, StackSp) = (cs, ip, ss, sp);
    }

    /// <summary>Load image without relocation (segment words relative to the load segment).</summary>
    public ReadOnlySpan<byte> Image => _image;

    public int Length => _image.Length;

    /// <summary>Relocation sites (segment:offset relative to the load segment) of the unpacked program.</summary>
    public IReadOnlyList<(ushort Segment, ushort Offset)> Relocations { get; }

    public ushort EntryCs { get; }
    public ushort EntryIp { get; }
    public ushort StackSs { get; }
    public ushort StackSp { get; }

    /// <summary>Paragraph (relative to the load segment) of the data segment the resident driver works with.</summary>
    public ushort DataParagraph => ReadWord(DataSegmentOperand);

    public int CodeBase => CodeParagraph * 16;

    public int DataBase => DataParagraph * 16;

    public byte ReadByte(int offset) => _image[offset];

    public ushort ReadWord(int offset) => (ushort)(_image[offset] | _image[offset + 1] << 8);

    /// <summary>Byte at an offset of the driver's data segment (the <c>[xxxx]</c> operands of the driver code).</summary>
    public byte DataByte(int offset) => _image[DataBase + offset];

    public ushort DataWord(int offset) => ReadWord(DataBase + offset);

    public static FxDriverImage Load(string path) => Unpack(File.ReadAllBytes(path));

    /// <summary>Unpacks an LZEXE 0.91 program (format of the packer's decompressor, not code of the driver).</summary>
    public static FxDriverImage Unpack(ReadOnlySpan<byte> exe)
    {
        if (exe.Length < 0x20 || exe[0] != 'M' || exe[1] != 'Z')
            throw new InvalidDataException("Not an MZ program.");
        int headerBytes = Word(exe, 8) * 16;
        ushort ip = Word(exe, 0x14), cs = Word(exe, 0x16);
        int lz = headerBytes + cs * 16; // LZEXE header at CS:0 of the packed program
        // LZEXE 0.91 format check by header fields: no relocations in the packed program's own table, the table offset at 1Ch,
        // entry IP 000Eh, a file length that matches the page fields and the packer header with its relocation table inside
        // the file. Then the checksum of the whole file (V1.06 only).
        int pages = Word(exe, 4), lastPage = Word(exe, 2);
        long declaredLength = lastPage == 0 ? pages * 512L : (pages - 1) * 512L + lastPage;
        if (Word(exe, 6) != 0 || Word(exe, 0x18) != 0x1C || ip != 0x000E || exe.Length < 0x20
            || declaredLength != exe.Length || lz + 0x158 > exe.Length)
            throw new InvalidDataException("Not an LZEXE 0.91 program (header fields).");
        if (!Convert.ToHexString(SHA256.HashData(exe)).Equals(Sha256, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("FXDRIVER.EXE is not V1.06 (checksum).");
        ushort ip0 = Word(exe, lz), cs0 = Word(exe, lz + 2), sp0 = Word(exe, lz + 4), ss0 = Word(exe, lz + 6);
        int packedParagraphs = Word(exe, lz + 8);

        var relocations = new List<(ushort, ushort)>();
        int pos = lz + 0x158, seg = 0, off = 0;
        while (true)
        {
            int span = exe[pos++];
            if (span == 0)
            {
                span = Word(exe, pos);
                pos += 2;
                if (span == 0) { seg += 0x0FFF; continue; }
                if (span == 1) break;
            }
            off += span;
            seg += (off & ~0x0F) >> 4;
            off &= 0x0F;
            relocations.Add(((ushort)seg, (ushort)off));
        }

        var output = new List<byte>(0x8000);
        var bits = new BitReader(exe, headerBytes + (cs - packedParagraphs) * 16);
        while (true)
        {
            if (bits.Bit() == 1)
            {
                output.Add(bits.Byte());
                continue;
            }
            int length, distance;
            if (bits.Bit() == 0)
            {
                length = (bits.Bit() << 1 | bits.Bit()) + 2;
                distance = bits.Byte() - 0x100;
            }
            else
            {
                int lo = bits.Byte(), hi = bits.Byte();
                distance = (lo | (hi & 0xF8) << 5 | 0xE000) - 0x10000;
                length = (hi & 7) + 2;
                if (length == 2)
                {
                    length = bits.Byte();
                    if (length == 0) break; // end of data
                    if (length == 1) continue; // segment step of the decompressor, no output
                    length++;
                }
            }
            if (-distance > output.Count) throw new InvalidDataException("LZEXE back reference before the start.");
            for (int i = 0; i < length; i++) output.Add(output[output.Count + distance]);
        }
        return new FxDriverImage([.. output], relocations, cs0, ip0, ss0, sp0);
    }

    /// <summary>SHA-256 of <c>FXDRIVER.EXE</c> V1.06 (<c>tools/originale/SHA256SUMS.dos</c>).</summary>
    public const string Sha256 = "fcbab0643d2cb4b5bf3713f663d8c1717b4b8ba8d35d67ad32ccd8c9ddcea2e2";

    private static ushort Word(ReadOnlySpan<byte> d, int o) => (ushort)(d[o] | d[o + 1] << 8);

    private ref struct BitReader
    {
        private readonly ReadOnlySpan<byte> _data;
        private int _pos;
        private int _buffer;
        private int _count;

        public BitReader(ReadOnlySpan<byte> data, int start)
        {
            _data = data;
            _pos = start;
            _buffer = NextWord();
            _count = 16;
        }

        private int NextWord()
        {
            int v = _data[_pos] | _data[_pos + 1] << 8;
            _pos += 2;
            return v;
        }

        public byte Byte() => _data[_pos++];

        public int Bit()
        {
            int b = _buffer & 1;
            _buffer >>= 1;
            if (--_count == 0)
            {
                _buffer = NextWord();
                _count = 16;
            }
            return b;
        }
    }
}
