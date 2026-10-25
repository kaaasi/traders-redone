namespace Traders.Assets;

/// <summary>
/// Decruncher for PowerPacker "PP20" data as used by all *.PP resources of Traders.
/// The compressed stream is read backwards in big-endian 32-bit words; output is
/// produced from the end towards the start.
/// </summary>
public static class PowerPacker
{
    public static bool IsPp20(ReadOnlySpan<byte> data) =>
        data.Length >= 12 && data[0] == (byte)'P' && data[1] == (byte)'P' && data[2] == (byte)'2' && data[3] == (byte)'0';

    public static byte[] Decrunch(ReadOnlySpan<byte> data)
    {
        if (!IsPp20(data))
            throw new InvalidDataException("Not a PP20 stream.");

        ReadOnlySpan<byte> efficiency = data.Slice(4, 4);
        int outLength = (data[^4] << 16) | (data[^3] << 8) | data[^2];
        int skipBits = data[^1];
        var reader = new BackwardBitReader(data.Slice(8, data.Length - 12));
        var output = new byte[outLength];

        reader.ReadBits(skipBits);
        int o = outLength;
        while (o > 0)
        {
            if (reader.ReadBits(1) == 0)
            {
                int literals = 1;
                int x;
                do
                {
                    x = reader.ReadBits(2);
                    literals += x;
                } while (x == 3);

                for (int i = 0; i < literals; i++)
                {
                    if (o <= 0) throw new InvalidDataException("PP20 literal run overflows output.");
                    output[--o] = (byte)reader.ReadBits(8);
                }
                if (o <= 0) break;
            }

            int idx = reader.ReadBits(2);
            int offsetBits = efficiency[idx];
            int length = idx + 2;
            int offset;
            if (idx == 3)
            {
                if (reader.ReadBits(1) == 0) offsetBits = 7;
                offset = reader.ReadBits(offsetBits);
                int y;
                do
                {
                    y = reader.ReadBits(3);
                    length += y;
                } while (y == 7);
            }
            else
            {
                offset = reader.ReadBits(offsetBits);
            }

            for (int i = 0; i < length; i++)
            {
                if (o <= 0) throw new InvalidDataException("PP20 match overflows output.");
                o--;
                output[o] = output[o + offset + 1];
            }
        }

        return output;
    }

    private ref struct BackwardBitReader
    {
        private readonly ReadOnlySpan<byte> _src;
        private int _pos;
        private uint _buffer;
        private int _bits;

        public BackwardBitReader(ReadOnlySpan<byte> src)
        {
            _src = src;
            _pos = src.Length;
            _buffer = ReadWord();
            _bits = 32;
        }

        private uint ReadWord()
        {
            _pos -= 4;
            if (_pos < 0) throw new InvalidDataException("PP20 stream underrun.");
            return (uint)(_src[_pos] << 24 | _src[_pos + 1] << 16 | _src[_pos + 2] << 8 | _src[_pos + 3]);
        }

        public int ReadBits(int count)
        {
            int value = 0;
            for (int i = 0; i < count; i++)
            {
                if (_bits == 0)
                {
                    _buffer = ReadWord();
                    _bits = 32;
                }
                value = (value << 1) | (int)(_buffer & 1);
                _buffer >>= 1;
                _bits--;
            }
            return value;
        }
    }
}
