namespace Traders.Audio.Opl;

/// <summary>
/// An <see cref="Opl2"/> resampled from its native rate (3 579 545 / 72 Hz) to an output rate such as 44 100 or 48 000 Hz.
/// Linear interpolation with an exact integer step (no drift, deterministic). Register writes take effect from the next
/// native sample on; <see cref="SamplesRendered"/> is the position in output samples.
/// </summary>
public sealed class OplStream
{
    private readonly long _step;    // native clocks per output sample, in units of 1/(72·rate) native samples
    private readonly long _period;  // one native sample in the same units
    private long _acc;
    private int _prev, _next;
    private bool _primed;

    public OplStream(int sampleRate, Opl2? chip = null)
    {
        if (sampleRate is < 8_000 or > 192_000) throw new ArgumentOutOfRangeException(nameof(sampleRate));
        SampleRate = sampleRate;
        Chip = chip ?? new Opl2();
        _step = Opl2.ClockHz;
        _period = (long)Opl2.ClocksPerSample * sampleRate;
    }

    public Opl2 Chip { get; }

    /// <summary>Output sample rate in Hz.</summary>
    public int SampleRate { get; }

    /// <summary>Output samples rendered so far.</summary>
    public long SamplesRendered { get; private set; }

    public void Write(byte register, byte value) => Chip.WriteRegister(register, value);

    /// <summary>Renders the next <paramref name="output"/>.Length output samples.</summary>
    public void Render(Span<short> output)
    {
        if (!_primed)
        {
            _prev = Chip.GenerateSample();
            _next = Chip.GenerateSample();
            _primed = true;
        }
        for (int i = 0; i < output.Length; i++)
        {
            output[i] = (short)((_prev * (_period - _acc) + _next * _acc) / _period);
            _acc += _step;
            while (_acc >= _period)
            {
                _acc -= _period;
                _prev = _next;
                _next = Chip.GenerateSample();
            }
        }
        SamplesRendered += output.Length;
    }

    /// <summary>Renders <paramref name="count"/> samples into a new array.</summary>
    public short[] Render(int count)
    {
        var a = new short[count];
        Render(a);
        return a;
    }
}
