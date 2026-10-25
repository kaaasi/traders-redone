using System.Buffers.Binary;

namespace Traders.Core.State;

/// <summary>
/// Byte image of the original data segment (little endian, 16-bit offsets). Used to exchange
/// state with the reference emulator (DS dumps) and to describe the save format exactly as the
/// original reads and writes it.
/// </summary>
public sealed class Dgroup
{
    public byte[] Bytes { get; }

    public Dgroup() : this(new byte[DgroupLayout.Size]) { }

    public Dgroup(byte[] bytes)
    {
        if (bytes.Length < DgroupLayout.Size)
            throw new ArgumentException($"DGROUP image needs at least {DgroupLayout.Size} bytes.", nameof(bytes));
        Bytes = bytes;
    }

    public byte U8(int offset) => Bytes[offset];
    public sbyte S8(int offset) => unchecked((sbyte)Bytes[offset]);
    public ushort U16(int offset) => BinaryPrimitives.ReadUInt16LittleEndian(Bytes.AsSpan(offset));
    public short S16(int offset) => BinaryPrimitives.ReadInt16LittleEndian(Bytes.AsSpan(offset));
    public int S32(int offset) => BinaryPrimitives.ReadInt32LittleEndian(Bytes.AsSpan(offset));
    public uint U32(int offset) => BinaryPrimitives.ReadUInt32LittleEndian(Bytes.AsSpan(offset));

    public void Set(int offset, byte value) => Bytes[offset] = value;
    public void Set(int offset, sbyte value) => Bytes[offset] = unchecked((byte)value);
    public void Set(int offset, ushort value) => BinaryPrimitives.WriteUInt16LittleEndian(Bytes.AsSpan(offset), value);
    public void Set(int offset, short value) => BinaryPrimitives.WriteInt16LittleEndian(Bytes.AsSpan(offset), value);
    public void Set(int offset, int value) => BinaryPrimitives.WriteInt32LittleEndian(Bytes.AsSpan(offset), value);
    public void Set(int offset, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(Bytes.AsSpan(offset), value);

    /// <summary>
    /// Initial DGROUP from an MZ executable (T.EXE): initialised data from the load image,
    /// BSS zero. The image starts after the header (paragraphs at 0x08).
    /// </summary>
    public static Dgroup FromExe(ReadOnlySpan<byte> exe)
    {
        if (exe.Length < 0x1C || exe[0] != (byte)'M' || exe[1] != (byte)'Z')
            throw new ArgumentException("Not an MZ executable.", nameof(exe));
        int header = BinaryPrimitives.ReadUInt16LittleEndian(exe[8..]) * 16;
        int start = header + DgroupLayout.Segment * 16;
        var bytes = new byte[DgroupLayout.Size];
        int available = Math.Clamp(exe.Length - start, 0, bytes.Length);
        exe.Slice(start, available).CopyTo(bytes);
        return new Dgroup(bytes);
    }
}
