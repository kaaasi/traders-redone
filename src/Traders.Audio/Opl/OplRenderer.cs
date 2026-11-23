using System.Globalization;

namespace Traders.Audio.Opl;

/// <summary>Offline rendering of a timed write sequence (e.g. a RefEmu <c>OplTrace</c>) to PCM, and WAV output.</summary>
public static class OplRenderer
{
    /// <summary>
    /// Renders <paramref name="writes"/> (sorted by tick) at <paramref name="sampleRate"/>. Sample 0 corresponds to tick
    /// <paramref name="origin"/> (default: the first write's tick); every write is applied before the first output sample
    /// at or after its tick. After the last write <paramref name="tailSeconds"/> of output follow.
    /// </summary>
    public static short[] Render(IReadOnlyList<OplWrite> writes, OplClock clock, int sampleRate,
        long? origin = null, double tailSeconds = 1.0)
    {
        var stream = new OplStream(sampleRate);
        var output = new List<short>();
        long t0 = origin ?? (writes.Count > 0 ? writes[0].Tick : 0);
        var buffer = new short[4096];
        long previousTick = long.MinValue;
        foreach (var w in writes)
        {
            if (w.Tick < previousTick) throw new ArgumentException("writes must be sorted by tick", nameof(writes));
            previousTick = w.Tick;
            long target = clock.SampleAt(Math.Max(0, w.Tick - t0), sampleRate);
            Advance(stream, output, buffer, target);
            stream.Write(w.Register, w.Value);
        }
        Advance(stream, output, buffer, stream.SamplesRendered + (long)Math.Round(tailSeconds * sampleRate));
        return output.ToArray();
    }

    private static void Advance(OplStream stream, List<short> output, short[] buffer, long target)
    {
        while (stream.SamplesRendered < target)
        {
            int n = (int)Math.Min(buffer.Length, target - stream.SamplesRendered);
            stream.Render(buffer.AsSpan(0, n));
            output.AddRange(buffer.AsSpan(0, n));
        }
    }

    /// <summary>
    /// Reads the text format of <c>OplTrace.WriteText</c>: per line PIT clock time, seconds, register and value (hex);
    /// lines starting with '#' are comments. Ticks are PIT clocks (<see cref="OplClock.Pit"/>).
    /// </summary>
    public static List<OplWrite> ReadTrace(TextReader reader)
    {
        var list = new List<OplWrite>();
        string? line;
        int number = 0;
        while ((line = reader.ReadLine()) is not null)
        {
            number++;
            line = line.Trim();
            if (line.Length == 0 || line[0] == '#') continue;
            var parts = line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 4
                || !long.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out long time)
                || !byte.TryParse(parts[2], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte reg)
                || !byte.TryParse(parts[3], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte value))
                throw new FormatException($"line {number}: expected '<pit-clocks> <seconds> <register> <value>'");
            list.Add(new OplWrite(time, reg, value));
        }
        return list;
    }

    /// <summary>Writes 16-bit mono PCM as a RIFF/WAVE file.</summary>
    public static void WriteWav(Stream stream, ReadOnlySpan<short> samples, int sampleRate)
    {
        using var w = new BinaryWriter(stream, System.Text.Encoding.ASCII, leaveOpen: true);
        int dataBytes = checked(samples.Length * 2);
        w.Write("RIFF"u8);
        w.Write(36 + dataBytes);
        w.Write("WAVE"u8);
        w.Write("fmt "u8);
        w.Write(16);
        w.Write((short)1);              // PCM
        w.Write((short)1);              // mono
        w.Write(sampleRate);
        w.Write(sampleRate * 2);        // byte rate
        w.Write((short)2);              // block align
        w.Write((short)16);             // bits per sample
        w.Write("data"u8);
        w.Write(dataBytes);
        foreach (short s in samples) w.Write(s);
    }
}
