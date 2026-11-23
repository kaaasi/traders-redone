using Traders.Audio.Effects;

namespace Traders.Tests.Audio;

/// <summary>
/// Differential tests of the sequence commands of the effect machine (jump table CS:151E) that the game's own effects do not use
/// (or use rarely): random but well-formed sequences are written into the same free place of the segment window on both sides
/// (CS:3000…, start table CS:3800 behind the start table pointer CS:15B2), started together on several voices and compared per
/// tick. The original is the oracle; the generator only keeps the sequences well-formed (command lengths, loop and call
/// structure, valid voice and instrument numbers), so that the original never runs into data.
/// </summary>
public class EffectsSequenceTests
{
    private const int SequenceArea = 0x3000, StartTable = 0x3800, StepTable = 0x3900, Sequences = 12;

    /// <summary>Builds one sequence (header word voice/priority, commands) with forward and backward labels.</summary>
    private sealed class Builder(int start, Random r, int[] voices, int voice)
    {
        public readonly List<byte> Bytes = [];
        private bool _velocity;
        public int Address => start + Bytes.Count;

        public void Command(int op, int param, params int[] extra)
        {
            Bytes.Add((byte)(op | 0x80));
            Bytes.Add((byte)param);
            foreach (int b in extra) Bytes.Add((byte)b);
        }

        /// <summary>One-byte commands (the handler steps SI back).</summary>
        public void Short(int op) => Bytes.Add((byte)(op | 0x80));

        /// <summary>Note with octave 0…7 and semitone 0…11 (from a random byte).</summary>
        public void Note(int note, int duration)
        {
            Bytes.Add(Pitch(note));
            Bytes.Add((byte)duration);
            if (_velocity) Bytes.Add((byte)r.Next(256));
        }

        /// <summary>
        /// Semitones 12…15 make the bend lookup (10B5/0CBD) read from CS:0100…, code bytes with relocated words whose values
        /// depend on the load segment ([APPROX] in the port); the generator keeps to 0…11.
        /// </summary>
        public static byte Pitch(int value) => (byte)(((value >> 4) & 0x07) << 4 | (value & 0x0F) % 12);

        public void Velocity(int a, int b)
        {
            Command(0x49, a, b);
            if (a != 0 || b != 0) _velocity = true;
        }

        public int Duration() => r.Next(8) == 0 ? 0 : r.Next(1, 24);

        /// <summary>A random command or note that does not change the control flow.</summary>
        public void Plain(int sequences)
        {
            int k = r.Next(48);
            switch (k)
            {
                case < 10: Note(r.Next(0x80), Duration()); break;
                case 10: Command(0x03, r.Next(8)); break;
                case 11: Command(0x07, r.Next(8) << 4); break;
                case 12: Command(0x09, r.Next(1, 16)); break;
                case 13: Command(0x0A, r.Next(0x20, 0xF6), r.Next(256)); break;
                case 14: Command(0x0B, Pitch(r.Next(0x80)), r.Next(1, 16)); break;
                case 15: Command(0x0C, r.Next(-14, 15)); break;
                case 16: Command(0x0D, r.Next(256), r.Next(16), r.Next(2) == 0 ? 0x40 : 0x60, StepTable & 0xFF, StepTable >> 8); break;
                case 17: Command(0x0E, r.Next(10)); break;
                case 18: Command(0x10, r.Next(200)); break;
                case 19: Command(0x11, r.Next(256), r.Next(256), r.Next(256)); break;
                case 20: Short(0x12); break;
                case 21: Command(0x13, r.Next(256)); break;
                case 22: Command(0x15, r.Next(256), r.Next(10), r.Next(1, 8), r.Next(8)); break;
                case 23: Command(0x1A, r.Next(-2, 6)); break;
                case 24: Command(0x1C, r.Next(256)); break;
                case 25: Command(0x1E, r.Next(64)); break;
                case 26: Command(0x20, r.Next(1, 16)); break;
                case 27: Command(0x21, r.Next(1, 16)); break;
                case 28: Command(0x24, r.Next(8)); break;
                case 29: Command(0x26, r.Next(256)); break;
                case 30: Short(0x27); break;
                case 31: Command(0x29, r.Next(256)); break;
                case 32: Command(0x2B, r.Next(64)); break;
                case 33: Command(r.Next(2) == 0 ? 0x2C : 0x2D, r.Next(10), r.Next(256)); break;
                case 34: Command(r.Next(2) == 0 ? 0x2E : 0x2F, r.Next(4)); break;
                case 35: Command(0x30, r.Next(256)); break;
                case 36: Command(0x33, r.Next(10)); break;
                case 37: Command(0x35, r.Next(4), r.Next(256)); break;
                case 38: Short(0x36); break;
                case 39: Command(0x39, r.Next(256)); break;
                case 40: Short(r.Next(2) == 0 ? 0x3A : 0x3B); break;
                case 41: Command(0x3C, r.Next(16)); break;
                case 42: Command(0x3D, r.Next(256)); break;
                case 43: Command(r.Next(2) == 0 ? 0x3F : 0x40, r.Next(256)); break;
                case 44: Command(0x41, r.Next(200), r.Next(200), r.Next(200), r.Next(256), r.Next(256), r.Next(256), r.Next(256), r.Next(256), r.Next(256)); break;
                case 45: if (r.Next(2) == 0) Command(0x42, r.Next(256)); else Short(0x43); break;
                case 46: Command(0x44 + r.Next(3), r.Next(32), r.Next(256)); break;
                default:
                    switch (r.Next(3))
                    {
                        case 0: Command(0x48, r.Next(2)); break;
                        case 1:
                            // starting a sequence on the own voice clears the block under the running stream (call depth
                            // and loop count), which can send the original into data; only other voices are started
                            int other = r.Next(sequences);
                            if (voices[other] != voice) Command(0x02, other);
                            break;
                        default: Command(0x1D, 1 << r.Next(3)); break;
                    }
                    break;
            }
        }
    }

    /// <summary>Writes <see cref="Sequences"/> random sequences, the start table and a step table into both windows.</summary>
    internal static void Arrange(EffectsHarness h, Random r)
    {
        int address = SequenceArea;
        var table = new List<int>();
        var voices = new int[Sequences];
        for (int s = 0; s < Sequences; s++) voices[s] = r.Next(3) == 0 ? 9 : r.Next(9);
        for (int s = 0; s < Sequences; s++)
        {
            int voice = voices[s];
            var b = new Builder(address, r, voices, voice);
            b.Bytes.Add((byte)voice);
            b.Bytes.Add((byte)r.Next(4));
            // velocity bytes follow every note once 49h is set, so it is set (or not) before any note
            if (r.Next(4) == 0) b.Velocity(r.Next(1, 4), r.Next(4));
            // optional subroutine placed first and jumped over
            int sub = -1;
            if (r.Next(2) == 0)
            {
                int jump = b.Bytes.Count;
                b.Command(0x04, 0, 0);
                sub = b.Address;
                for (int i = r.Next(1, 5); i > 0; i--) b.Plain(Sequences);
                b.Note(r.Next(0x80), r.Next(1, 8));
                b.Short(0x06);
                b.Bytes[jump + 1] = (byte)b.Address;
                b.Bytes[jump + 2] = (byte)(b.Address >> 8);
            }
            int body = b.Address;
            bool loop = r.Next(2) == 0;
            if (loop) { b.Command(0x00, r.Next(1, 5)); body = b.Address; }
            for (int i = r.Next(3, 25); i > 0; i--)
            {
                if (sub >= 0 && r.Next(6) == 0) b.Command(0x05, sub & 0xFF, sub >> 8);
                else b.Plain(Sequences);
            }
            b.Note(r.Next(0x80), r.Next(1, 8));
            if (loop) b.Command(0x01, body & 0xFF, body >> 8);
            if (r.Next(4) == 0) b.Command(0x04, body & 0xFF, body >> 8);
            else b.Command(0x08, 0);
            for (int i = 0; i < b.Bytes.Count; i++) h.WriteBoth(address + i, b.Bytes[i]);
            table.Add(address);
            address += b.Bytes.Count;
            if (address >= StartTable) throw new InvalidOperationException("sequence area full");
        }
        for (int s = 0; s < table.Count; s++)
        {
            h.WriteBoth(StartTable + 2 * s, (byte)table[s]);
            h.WriteBoth(StartTable + 2 * s + 1, (byte)(table[s] >> 8));
        }
        for (int i = 0; i < 256; i++) h.WriteBoth(StepTable + i, (byte)r.Next(256));
        h.WriteBoth(EffectMachine.StartTablePointer, StartTable & 0xFF);
        h.WriteBoth(EffectMachine.StartTablePointer + 1, StartTable >> 8);
    }

    // [DIFFERENTIAL] 0000:06FE, 0000:0732, 0000:0750, 0000:0770, 0000:0812, 0000:0866, 0000:0921, 0000:0946, 0000:094B, 0000:0953, 0000:09A2, 0000:09A8, 0000:09B4, 0000:09CC, 0000:09DF, 0000:09E5, 0000:09FB, 0000:0A18, 0000:0A1F, 0000:0A3A, 0000:0A40, 0000:0A5D, 0000:0A81, 0000:0A97, 0000:0AAD, 0000:0ABB, 0000:0AC1, 0000:0AE0, 0000:0AE6, 0000:0B02, 0000:0B32, 0000:0B3B, 0000:0B4E, 0000:0B64, 0000:0B6D, 0000:0B74, 0000:0B7D, 0000:0B83, 0000:0B89, 0000:0BAA, 0000:0BCD, 0000:0BEB, 0000:0C09, 0000:0C15, 0000:0C81, 0000:0CB4, 0000:0CBD, 0000:0D77, 0000:0D81, 0000:0D85, 0000:0D8B, 0000:0DA8, 0000:0DAB, 0000:0E8A, 0000:0EBA, 0000:0ECD, 0000:0F71, 0000:1015, 0000:10A5, 0000:10AB, 0000:10B5, 0000:116A, 0000:119A, 0000:11C2, 0000:11FA, 0000:120B, 0000:121C, 0000:127A, 0000:12A9, 0000:12E1, 0000:131F, 0000:1352, 0000:13BE, 0000:1408, 0000:1461, 0000:148A, 0000:150E
    [GameDataFact]
    public void Random_sequences_match_the_original() => RunCases(150, null);

    internal static void RunCases(int count, Action<int, EffectsHarness>? atCase)
    {
        var h = new EffectsHarness();
        var r = new Random(0x151E);
        for (int c = 0; c < count; c++)
        {
            h.Restart();
            Arrange(h, r);
            atCase?.Invoke(c, h);
            h.Reset($"case {c} reset");
            for (int t = 0; t < 400; t++)
            {
                if (t == 0 || r.Next(40) == 0)
                    for (int i = r.Next(1, 4); i > 0; i--) h.Enqueue($"case {c} tick {t} request", (ushort)r.Next(Sequences));
                try
                {
                    h.Tick($"case {c} tick {t}");
                }
                catch (Traders.RefEmu.Cpu.EmulationException e)
                {
                    throw new InvalidOperationException($"case {c} tick {t}: the original left the effect machine", e);
                }
            }
            h.AssertSameSegment($"case {c} end");
        }
    }
}
