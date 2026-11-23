using Traders.Audio.Opl;

namespace Traders.Tests.Audio;

/// <summary>
/// OPL2 synthesis (lane klang, plan R9 F1 = B). Expected values come from the YM3812 data sheet (frequency formula,
/// multiple table, 0.75 dB total level steps, 3 dB sustain level steps, waveform select, release times), never from the
/// port itself.
/// </summary>
public class OplSynthesisTests
{
    private const int Rate = Opl2.NominalSampleRate;

    /// <summary>Sets channel 0 to a single audible carrier (operator 2, offset 3); operator 1 never attacks (AR 0).</summary>
    private static Opl2 Tone(int fnum, int block, int multiple = 1, int totalLevel = 0, int attack = 15, int decay = 0,
        int sustainLevel = 0, int release = 0, bool sustain = true, int waveform = 0, bool waveformEnable = true,
        int feedback = 0, bool keyOn = true)
    {
        var c = new Opl2();
        c.WriteRegister(0x01, (byte)(waveformEnable ? 0x20 : 0x00));
        c.WriteRegister(0x20, 0x01);
        c.WriteRegister(0x40, 0x3f);
        c.WriteRegister(0x60, 0x00);
        c.WriteRegister(0x80, 0x00);
        c.WriteRegister(0x23, (byte)((sustain ? 0x20 : 0) | multiple));
        c.WriteRegister(0x43, (byte)totalLevel);
        c.WriteRegister(0x63, (byte)((attack << 4) | decay));
        c.WriteRegister(0x83, (byte)((sustainLevel << 4) | release));
        c.WriteRegister(0xe3, (byte)waveform);
        c.WriteRegister(0xc0, (byte)(0x01 | (feedback << 1)));
        c.WriteRegister(0xa0, (byte)fnum);
        c.WriteRegister(0xb0, (byte)((keyOn ? 0x20 : 0) | (block << 2) | (fnum >> 8)));
        return c;
    }

    private static short[] Run(Opl2 c, double seconds)
    {
        var a = new short[(int)(seconds * Rate)];
        c.Generate(a);
        return a;
    }

    private static double Frequency(ReadOnlySpan<short> s, int rate)
    {
        int crossings = 0;
        for (int i = 1; i < s.Length; i++) if (s[i - 1] < 0 && s[i] >= 0) crossings++;
        return crossings * (double)rate / s.Length;
    }

    private static int Peak(ReadOnlySpan<short> s)
    {
        int p = 0;
        foreach (short v in s) p = Math.Max(p, Math.Abs((int)v));
        return p;
    }

    /// <summary>Data sheet: f = FNUM · 2^(BLOCK−20) · fs · MULT with fs = 3.579545 MHz / 72.</summary>
    private static double Expected(int fnum, int block, double mult) => fnum * Math.Pow(2, block - 20) * Opl2.SampleRate * mult;

    [Theory]
    [InlineData(0x244, 4)] // a' 440 Hz
    [InlineData(0x244, 2)]
    [InlineData(0x244, 6)]
    [InlineData(0x16b, 5)]
    [InlineData(0x3ff, 3)]
    [InlineData(0x101, 1)]
    public void Frequency_follows_fnum_and_block(int fnum, int block)
    {
        var s = Run(Tone(fnum, block), 1.0);
        double f = Frequency(s.AsSpan(1000), Rate);
        double e = Expected(fnum, block, 1);
        Assert.InRange(f, e * 0.99 - 1, e * 1.01 + 1);
    }

    [Theory]
    [InlineData(0, 0.5)]
    [InlineData(2, 2)]
    [InlineData(5, 5)]
    [InlineData(10, 10)]
    [InlineData(11, 10)] // data sheet: 11 → 10
    [InlineData(13, 12)] // 13 → 12
    [InlineData(14, 15)] // 14 → 15
    [InlineData(15, 15)]
    public void Multiple_follows_the_data_sheet_table(int multiple, double factor)
    {
        var s = Run(Tone(0x244, 2, multiple), 1.0);
        double f = Frequency(s.AsSpan(1000), Rate);
        double e = Expected(0x244, 2, factor);
        Assert.InRange(f, e * 0.99 - 1, e * 1.01 + 1);
    }

    [Fact]
    public void Full_level_sine_reaches_the_dac_maximum_of_a_single_channel()
    {
        // 13-bit volume (power table maximum 0x7FA0 >> 2 = 8168), output halved per channel
        int peak = Peak(Run(Tone(0x244, 4), 0.2));
        Assert.InRange(peak, 4000, 4100);
    }

    [Theory]
    [InlineData(8, 6.0)]
    [InlineData(16, 12.0)]
    [InlineData(4, 3.0)]
    public void Total_level_attenuates_in_three_quarter_decibel_steps(int tl, double decibels)
    {
        double full = Peak(Run(Tone(0x244, 4), 0.2).AsSpan(2000));
        double att = Peak(Run(Tone(0x244, 4, totalLevel: tl), 0.2).AsSpan(2000));
        Assert.InRange(20 * Math.Log10(full / att), decibels - 0.3, decibels + 0.3);
    }

    [Theory]
    [InlineData(2, 6.0)]
    [InlineData(4, 12.0)]
    public void Decay_stops_at_the_sustain_level_in_three_decibel_steps(int sl, double decibels)
    {
        double full = Peak(Run(Tone(0x244, 4), 0.2).AsSpan(2000));
        var s = Run(Tone(0x244, 4, decay: 10, sustainLevel: sl), 1.5);
        double held = Peak(s.AsSpan(s.Length - 5000));
        Assert.InRange(20 * Math.Log10(full / held), decibels - 0.5, decibels + 0.5);
        // still held at the same level half a second later (EG type = sustained)
        Assert.Equal(held, Peak(s.AsSpan(s.Length - 30000, 5000)), 0.02 * held);
    }

    [Fact]
    public void Attack_rate_zero_never_sounds()
    {
        Assert.Equal(0, Peak(Run(Tone(0x244, 4, attack: 0), 0.5)));
    }

    [Fact]
    public void Slow_attack_rises_over_time()
    {
        var s = Run(Tone(0x244, 4, attack: 4), 1.0);
        int early = Peak(s.AsSpan(0, Rate / 50));
        int late = Peak(s.AsSpan(Rate * 8 / 10));
        Assert.True(early < late / 4, $"early {early}, late {late}");
        Assert.InRange(late, 4000, 4100);
    }

    /// <summary>
    /// Data sheet: release from 0 dB to −96 dB takes 307 ms at RR = 8; the output reaches 0 once below the DAC's last
    /// step (about −72 dB, ~230 ms). The tone must neither stop at once nor keep sounding.
    /// </summary>
    [Fact]
    public void Release_after_key_off_fades_to_silence()
    {
        var c = Tone(0x244, 4, release: 8);
        Run(c, 0.2);
        c.WriteRegister(0xb0, (byte)((4 << 2) | (0x244 >> 8))); // key off
        var s = Run(c, 1.0);
        int silentFrom = s.Length;
        while (silentFrom > 0 && s[silentFrom - 1] == 0) silentFrom--;
        double ms = silentFrom * 1000.0 / Rate;
        Assert.InRange(ms, 150, 400);
        Assert.True(Peak(s.AsSpan(0, Rate / 50)) > 1000, "release must not cut the tone at once");
        Assert.Equal((EnvelopeState.Release, 0x3ffu), c.Envelope(3));
    }

    [Fact]
    public void Release_rate_zero_keeps_the_tone_after_key_off()
    {
        var c = Tone(0x244, 4, release: 0);
        Run(c, 0.1);
        c.WriteRegister(0xb0, (byte)((4 << 2) | (0x244 >> 8)));
        var s = Run(c, 1.0);
        Assert.InRange(Peak(s.AsSpan(s.Length - 5000)), 4000, 4100);
    }

    [Fact]
    public void Percussive_envelope_decays_with_release_rate_while_key_is_held()
    {
        var s = Run(Tone(0x244, 4, release: 8, sustain: false), 1.0);
        Assert.Equal(0, Peak(s.AsSpan(s.Length - 5000)));
        Assert.True(Peak(s.AsSpan(0, 2000)) > 3000);
    }

    [Fact]
    public void Key_on_again_restarts_the_attack()
    {
        var c = Tone(0x244, 4, release: 8);
        Run(c, 0.1);
        c.WriteRegister(0xb0, (byte)((4 << 2) | (0x244 >> 8)));
        Run(c, 1.0);
        c.WriteRegister(0xb0, (byte)(0x20 | (4 << 2) | (0x244 >> 8)));
        Assert.InRange(Peak(Run(c, 0.1)), 4000, 4100);
    }

    [Fact]
    public void Half_sine_waveform_needs_the_waveform_select_enable()
    {
        var on = Run(Tone(0x244, 4, waveform: 1, waveformEnable: true), 0.2);
        var off = Run(Tone(0x244, 4, waveform: 1, waveformEnable: false), 0.2);
        Assert.DoesNotContain(on, v => v < 0);
        Assert.Contains(off, v => v < -3000);
        var abs = Run(Tone(0x244, 4, waveform: 2), 0.2);
        Assert.DoesNotContain(abs, v => v < 0);
        Assert.Equal(2 * Expected(0x244, 4, 1), AbsFrequency(abs), 0.02 * Expected(0x244, 4, 1));
    }

    private static double AbsFrequency(short[] s)
    {
        // rectified sine: fundamental at twice the tone, counted as upward crossings of the mean
        var span = s.AsSpan(1000);
        double mean = 0;
        foreach (short v in span) mean += v;
        mean /= span.Length;
        int n = 0;
        for (int i = 1; i < span.Length; i++) if (span[i - 1] < mean && span[i] >= mean) n++;
        return n * (double)Rate / span.Length;
    }

    /// <summary>Self-feedback of operator 1 adds harmonics; the fundamental stays.</summary>
    [Fact]
    public void Feedback_adds_harmonics_to_operator_one()
    {
        static short[] Op1(int fb)
        {
            var c = new Opl2();
            c.WriteRegister(0x20, 0x21);
            c.WriteRegister(0x40, 0x00);
            c.WriteRegister(0x60, 0xf0);
            c.WriteRegister(0x80, 0x00);
            c.WriteRegister(0x63, 0x00); // operator 2 silent
            c.WriteRegister(0xc0, (byte)(0x01 | (fb << 1)));
            c.WriteRegister(0xa0, 0x44);
            c.WriteRegister(0xb0, 0x20 | (4 << 2) | 0x02);
            return Run(c, 0.5);
        }

        static double Roughness(short[] s)
        {
            double d = 0, e = 0;
            for (int i = 1001; i < s.Length; i++) { d += Math.Pow(s[i] - s[i - 1], 2); e += (double)s[i] * s[i]; }
            return d / e;
        }

        var plain = Op1(0);
        var fed = Op1(4); // data sheet: modulation index π/2 (6 and 7 turn noise-like)
        Assert.Equal(Expected(0x244, 4, 1), Frequency(plain.AsSpan(1000), Rate), 5.0);
        Assert.Equal(Expected(0x244, 4, 1), Frequency(fed.AsSpan(1000), Rate), 5.0);
        Assert.True(Roughness(fed) > 2 * Roughness(plain), $"{Roughness(fed)} vs {Roughness(plain)}");
        Assert.NotEqual(plain, fed);
    }

    [Fact]
    public void Frequency_modulation_changes_the_waveform_but_not_the_pitch()
    {
        var plain = Run(Tone(0x244, 4), 0.5);
        var c = Tone(0x244, 4, keyOn: false);
        c.WriteRegister(0xc0, 0x00);  // algorithm 0: operator 1 modulates operator 2
        c.WriteRegister(0x40, 0x30);
        c.WriteRegister(0x60, 0xf0);
        c.WriteRegister(0xb0, 0x20 | (4 << 2) | 0x02);
        var fm = Run(c, 0.5);
        Assert.NotEqual(plain, fm);
        Assert.Equal(Expected(0x244, 4, 1), Frequency(fm.AsSpan(1000), Rate), 5.0);
    }

    [Fact]
    public void Rhythm_instruments_sound_and_normal_key_on_of_channel_six_is_not_needed()
    {
        foreach (int bit in new[] { 0x10, 0x08, 0x04, 0x02, 0x01 })
        {
            var c = new Opl2();
            for (int op = 0; op < 0x16; op++)
            {
                c.WriteRegister(0x20 + op, 0x01);
                c.WriteRegister(0x40 + op, 0x00);
                c.WriteRegister(0x60 + op, 0xf4);
                c.WriteRegister(0x80 + op, 0x04);
            }
            for (int ch = 6; ch <= 8; ch++)
            {
                c.WriteRegister(0xa0 + ch, 0x44);
                c.WriteRegister(0xb0 + ch, (4 << 2) | 0x02);
            }
            c.WriteRegister(0xbd, (byte)(0x20 | bit));
            Assert.True(Peak(Run(c, 0.1)) > 500, $"rhythm bit {bit:X2} silent");
        }
    }

    [Fact]
    public void Same_writes_give_bit_identical_output()
    {
        static short[] Song()
        {
            var c = Tone(0x244, 4, release: 6, feedback: 3);
            var a = new List<short>();
            for (int n = 0; n < 20; n++)
            {
                c.WriteRegister(0xa0, (byte)(0x40 + 7 * n));
                c.WriteRegister(0xb0, (byte)(0x20 | (3 << 2) | 1));
                c.WriteRegister(0xbd, (byte)(n % 2 == 0 ? 0xc0 : 0x00));
                a.AddRange(Run(c, 0.03));
                c.WriteRegister(0xb0, (3 << 2) | 1);
                a.AddRange(Run(c, 0.02));
            }
            return a.ToArray();
        }

        var first = Song();
        Assert.Equal(first, Song());
        Assert.Contains(first, v => v != 0);
    }

    [Fact]
    public void Reset_silences_and_clears_registers()
    {
        var c = Tone(0x244, 4);
        Run(c, 0.05);
        c.Reset();
        Assert.Equal(0, c.ReadRegister(0xb0));
        var s = Run(c, 0.05);
        Assert.Equal(0, Peak(s.AsSpan(10)));
    }

    [Fact]
    public void Port_interface_selects_register_then_writes_data()
    {
        var c = new Opl2();
        c.WritePort(0, 0xa3);
        c.WritePort(1, 0x5a);
        Assert.Equal(0x5a, c.ReadRegister(0xa3));
    }
}
