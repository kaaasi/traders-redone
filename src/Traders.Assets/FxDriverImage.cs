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
        if (lz + 0x158 > exe.Length || ip != 0x000E || !exe.Slice(lz + ip, Lz91Start.Length).SequenceEqual(Lz91Start))
            throw new InvalidDataException("Decompressor not recognised as LZEXE 0.91.");
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

    // push es; push cs; pop ds; mov cx,[000C]; mov si,cx; dec si; mov di,si; mov bx,ds; add bx,[000A]; mov es,bx;
    // std; rep movsb – the self-relocation at the start of the LZEXE 0.91 decompressor.
    private static ReadOnlySpan<byte> Lz91Start =>
        [0x06, 0x0E, 0x1F, 0x8B, 0x0E, 0x0C, 0x00, 0x8B, 0xF1, 0x4E, 0x89, 0xF7, 0x8C, 0xDB, 0x03, 0x1E, 0x0A, 0x00, 0x8E, 0xC3, 0xFD, 0xF3, 0xA4];

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
