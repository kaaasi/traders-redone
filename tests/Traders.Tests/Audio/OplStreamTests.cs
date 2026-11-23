using Traders.Audio.Opl;

namespace Traders.Tests.Audio;

/// <summary>Resampling to host rates, tick-to-sample mapping and offline rendering of write sequences.</summary>
public class OplStreamTests
{
    private static void A440(Action<byte, byte> write)
    {
        write(0x01, 0x20);
        write(0x20, 0x01); write(0x40, 0x3f); write(0x60, 0x00);
        write(0x23, 0x21); write(0x43, 0x00); write(0x63, 0xf0); write(0x83, 0x08);
        write(0xc0, 0x01);
        write(0xa0, 0x44);
        write(0xb0, 0x20 | (4 << 2) | 0x02);
    }

    private static double Frequency(ReadOnlySpan<short> s, int rate)
    {
        int crossings = 0;
        for (int i = 1; i < s.Length; i++) if (s[i - 1] < 0 && s[i] >= 0) crossings++;
        return crossings * (double)rate / s.Length;
    }

    [Theory]
    [InlineData(44_100)]
    [InlineData(48_000)]
    [InlineData(22_050)]
    public void Resampled_tone_keeps_its_pitch_and_level(int rate)
    {
        var s = new OplStream(rate);
        A440(s.Write);
        var a = s.Render(rate);
        Assert.Equal(rate, s.SamplesRendered);
        double expected = 0x244 * Math.Pow(2, 4 - 20) * Opl2.SampleRate;
        Assert.InRange(Frequency(a.AsSpan(100), rate), expected * 0.99, expected * 1.01);
        Assert.InRange(a.Max(v => Math.Abs((int)v)), 3900, 4100);
    }

    [Fact]
    public void Native_samples_consumed_follow_the_exact_rate_ratio()
    {
        var s = new OplStream(44_100);
        s.Render(44_100 * 10);
        // 10 s at 3 579 545 / 72 Hz = 497 159.0 native samples (plus the two primed ones)
        Assert.InRange(s.Chip.SamplesGenerated, 497_159, 497_161);
    }

    [Fact]
    public void Render_in_pieces_equals_render_at_once()
    {
        var whole = new OplStream(48_000);
        A440(whole.Write);
        var a = whole.Render(20_000);

        var pieces = new OplStream(48_000);
        A440(pieces.Write);
        var b = new List<short>();
        foreach (int n in new[] { 1, 7, 333, 4096, 15_563 }) b.AddRange(pieces.Render(n));
        Assert.Equal(a, b);
    }

    [Fact]
    public void Clock_maps_ticks_to_samples_without_drift()
    {
        Assert.Equal(44_100, OplClock.Pit.SampleAt(1_193_182, 44_100));
        Assert.Equal(0, OplClock.Pit.SampleAt(0, 44_100));
        Assert.Equal(1, OplClock.Pit.SampleAt(1, 44_100));
        // 72.82 Hz ticks: tick n starts at ceil(n · 16384 · 48000 / 1193182)
        Assert.Equal(660, OplClock.TimerTick.SampleAt(1, 48_000));
        Assert.Equal(48_000L * 0x4000 * 1_000_000 / 1_193_182 + 1, OplClock.TimerTick.SampleAt(1_000_000, 48_000));
    }

    [Fact]
    public void Renderer_applies_writes_at_their_tick()
    {
        var writes = new List<OplWrite>();
        A440((r, v) => writes.Add(new OplWrite(0, r, v)));
        // key off after half a second of PIT clocks
        writes.Add(new OplWrite(1_193_182 / 2, 0xb0, (4 << 2) | 0x02));
        var pcm = OplRenderer.Render(writes, OplClock.Pit, 44_100, tailSeconds: 1.0);
        Assert.Equal(22_050 + 44_100, pcm.Length);
        Assert.True(pcm.AsSpan(21_000, 1000).ToArray().Max(v => Math.Abs((int)v)) > 3900);
        Assert.Equal(0, pcm.AsSpan(pcm.Length - 1000).ToArray().Max(v => Math.Abs((int)v)));
    }

    [Fact]
    public void Renderer_rejects_unsorted_writes()
    {
        var writes = new[] { new OplWrite(10, 0x20, 1), new OplWrite(5, 0x20, 1) };
        Assert.Throws<ArgumentException>(() => OplRenderer.Render(writes, OplClock.Pit, 44_100));
    }

    [Fact]
    public void Trace_text_round_trips_into_writes()
    {
        var text = "# pit-clocks seconds register value\n1193182 1.000000 B0 2A\n\n2386364 2.000000 bd 3f\n";
        var w = OplRenderer.ReadTrace(new StringReader(text));
        Assert.Equal([new OplWrite(1_193_182, 0xb0, 0x2a), new OplWrite(2_386_364, 0xbd, 0x3f)], w);
        Assert.Throws<FormatException>(() => OplRenderer.ReadTrace(new StringReader("12 x\n")));
    }

    [Fact]
    public void Wav_header_describes_sixteen_bit_mono()
    {
        using var ms = new MemoryStream();
        OplRenderer.WriteWav(ms, new short[] { 1, -2, 3 }, 44_100);
        var b = ms.ToArray();
        Assert.Equal(44 + 6, b.Length);
        Assert.Equal("RIFF", System.Text.Encoding.ASCII.GetString(b, 0, 4));
        Assert.Equal(44 + 6 - 8, BitConverter.ToInt32(b, 4));
        Assert.Equal("WAVEfmt ", System.Text.Encoding.ASCII.GetString(b, 8, 8));
        Assert.Equal(1, BitConverter.ToInt16(b, 22));
        Assert.Equal(44_100, BitConverter.ToInt32(b, 24));
        Assert.Equal(16, BitConverter.ToInt16(b, 34));
        Assert.Equal(6, BitConverter.ToInt32(b, 40));
        Assert.Equal(-2, BitConverter.ToInt16(b, 46));
    }
}
