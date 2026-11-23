// Translated to C# from ymfm (https://github.com/aaronsgiles/ymfm: ym3812 and opl_registers_base<2> in
// src/ymfm_opl.h/.cpp, fm_engine_base in src/ymfm_fm.ipp), Copyright (c) 2021, Aaron Giles, BSD 3-Clause License –
// see THIRD_PARTY.md. Not part of the original game, hence no original-code markers (plan R9, lane klang).

namespace Traders.Audio.Opl;

/// <summary>
/// Yamaha YM3812 (OPL2) as used on the AdLib card: register writes in, one 16-bit mono sample per 72 input clocks out
/// (about 49 716 Hz at 3.579545 MHz). Deterministic: the same writes at the same sample positions give the same samples.
/// Timers, status flags and CSM mode are not modelled (the port never reads the status port); writes to registers 02h–04h
/// are stored only.
/// </summary>
public sealed class Opl2
{
    /// <summary>Input clock of the AdLib card in Hz.</summary>
    public const int ClockHz = 3_579_545;

    /// <summary>Input clocks per output sample (prescale 4 · 18 operators).</summary>
    public const int ClocksPerSample = 72;

    /// <summary>Native sample rate rounded to whole Hz (3 579 545 / 72 ≈ 49 715.9).</summary>
    public const int NominalSampleRate = 49_716;

    /// <summary>Exact native sample rate.</summary>
    public static double SampleRate => (double)ClockHz / ClocksPerSample;

    private const int Channels = 9, Operators = 18;
    private const uint AllChannels = (1u << Channels) - 1;
    private const int RegMode = 0x04;

    private readonly byte[] _regs = new byte[0x100];
    private readonly Opl2Operator[] _operators = new Opl2Operator[Operators];
    private readonly Opl2Channel[] _channels = new Opl2Channel[Channels];

    private byte _address;
    private uint _envCounter;
    private uint _activeChannels = AllChannels;
    private uint _modifiedChannels = AllChannels;
    private uint _prepareCount;

    private ushort _lfoAmCounter, _lfoPmCounter;
    private uint _noiseLfsr = 1;
    private byte _lfoAm;

    public Opl2()
    {
        for (int i = 0; i < Operators; i++) _operators[i] = new Opl2Operator(this, i + 2 * (i / 6));
        // fixed OPL2 operator map: channel n uses operators (n, n+3) within each group of three channels
        int[,] map = { { 0, 3 }, { 1, 4 }, { 2, 5 }, { 6, 9 }, { 7, 10 }, { 8, 11 }, { 12, 15 }, { 13, 16 }, { 14, 17 } };
        for (int c = 0; c < Channels; c++) _channels[c] = new Opl2Channel(this, c, _operators[map[c, 0]], _operators[map[c, 1]]);
        Reset();
    }

    /// <summary>Number of samples generated so far.</summary>
    public long SamplesGenerated { get; private set; }

    /// <summary>Clears all registers and puts every operator into release at full attenuation (chip reset).</summary>
    public void Reset()
    {
        Array.Clear(_regs);
        WriteRegister(RegMode, 0);
        foreach (var c in _channels) c.Reset();
        foreach (var o in _operators) o.Reset();
    }

    /// <summary>Current contents of a register (as last written).</summary>
    public byte ReadRegister(int register) => _regs[register & 0xff];

    /// <summary>Port write as on the card: offset 0 = address port (388h), 1 = data port (389h).</summary>
    public void WritePort(int offset, byte data)
    {
        if ((offset & 1) == 0) _address = data;
        else WriteRegister(_address, data);
    }

    /// <summary>Writes <paramref name="data"/> to register <paramref name="register"/> (0–FFh).</summary>
    public void WriteRegister(int register, byte data)
    {
        int index = register & 0xff;
        _modifiedChannels = AllChannels;

        // writes to the mode register with the high bit set ignore the low bits
        if (index == RegMode && (data & 0x80) != 0) _regs[index] |= 0x80;
        else _regs[index] = data;

        if (index == 0xbd)
        {
            uint mask = OplTables.Bits(data, 5) != 0 ? OplTables.Bits(data, 0, 5) : 0;
            _channels[6].KeyOnOff(OplTables.Bits(mask, 4) != 0 ? 3u : 0u, rhythm: true);
            _channels[7].KeyOnOff(OplTables.Bits(mask, 0) | (OplTables.Bits(mask, 3) << 1), rhythm: true);
            _channels[8].KeyOnOff(OplTables.Bits(mask, 2) | (OplTables.Bits(mask, 1) << 1), rhythm: true);
        }
        else if ((index & 0xf0) == 0xb0)
        {
            int channel = index & 0x0f;
            if (channel < Channels) _channels[channel].KeyOnOff(OplTables.Bits(data, 5) != 0 ? 3u : 0u, rhythm: false);
        }
    }

    /// <summary>Generates <paramref name="output"/>.Length native samples.</summary>
    public void Generate(Span<short> output)
    {
        for (int i = 0; i < output.Length; i++) output[i] = GenerateSample();
    }

    /// <summary>Generates one native sample.</summary>
    public short GenerateSample()
    {
        Clock();
        int sum = Output(rshift: 1, clipmax: 32767);
        SamplesGenerated++;
        // the YM3812 feeds an external DAC (YM3014) in a floating-point format
        return OplTables.RoundTripFloatingPoint(sum);
    }

    private void Clock()
    {
        // prepare after any write, and every 4096 samples to catch ending notes
        if (_modifiedChannels != 0 || _prepareCount++ >= 4096)
        {
            _activeChannels = 0;
            for (int c = 0; c < Channels; c++)
                if (_channels[c].Prepare()) _activeChannels |= 1u << c;
            _modifiedChannels = 0;
            _prepareCount = 0;
        }

        _envCounter += 4; // envelope clock divider 1
        int lfoRawPm = ClockNoiseAndLfo();
        foreach (var c in _channels) c.Clock(_envCounter, lfoRawPm);
    }

    private int ClockNoiseAndLfo()
    {
        // 23-bit noise generator for the rhythm section
        _noiseLfsr <<= 1;
        _noiseLfsr |= OplTables.Bits(_noiseLfsr, 23) ^ OplTables.Bits(_noiseLfsr, 9) ^ OplTables.Bits(_noiseLfsr, 8) ^ OplTables.Bits(_noiseLfsr, 1);

        // AM LFO: 210·64 steps, triangle; depth 0 divides by 2, depth 1 multiplies by 2
        uint amCounter = _lfoAmCounter++;
        if (amCounter >= 210 * 64 - 1) _lfoAmCounter = 0;
        int shift = 9 - 2 * (int)LfoAmDepth;
        _lfoAm = (byte)((amCounter < 105 * 64 ? amCounter : 210 * 64 + 63 - amCounter) >> shift);

        // PM LFO: 8192 steps in 8 chunks, value is a signed 1.3 fraction of the upper FNUM bits
        uint pmCounter = _lfoPmCounter++;
        ReadOnlySpan<sbyte> pmScale = [8, 4, 0, -4, -8, -4, 0, 4];
        return pmScale[(int)OplTables.Bits(pmCounter, 10, 3)] >> (int)(LfoPmDepth ^ 1);
    }

    private int Output(int rshift, int clipmax)
    {
        uint mask = _activeChannels;
        int sum = 0;
        if (RhythmEnable != 0)
        {
            uint op13 = _operators[13].Phase, op17 = _operators[17].Phase;
            uint phaseSelect = (OplTables.Bits(op13, 2) ^ OplTables.Bits(op13, 7)) | OplTables.Bits(op13, 3)
                | (OplTables.Bits(op17, 5) ^ OplTables.Bits(op17, 3));
            for (int c = 0; c < Channels; c++)
            {
                if (OplTables.Bits(mask, c) == 0) continue;
                sum += c switch
                {
                    6 => _channels[c].OutputRhythmCh6(rshift, clipmax),
                    7 => _channels[c].OutputRhythmCh7(phaseSelect, rshift, clipmax),
                    8 => _channels[c].OutputRhythmCh8(phaseSelect, rshift, clipmax),
                    _ => _channels[c].Output2Op(rshift, clipmax),
                };
            }
        }
        else
        {
            for (int c = 0; c < Channels; c++)
                if (OplTables.Bits(mask, c) != 0) sum += _channels[c].Output2Op(rshift, clipmax);
        }
        return sum;
    }

    // ---- register fields (ymfm opl_registers_base<2>) ----

    private uint Byte(int offset, int start, int count, int extra = 0) => OplTables.Bits(_regs[offset + extra], start, count);

    internal byte LfoAm => _lfoAm;
    internal uint NoiseState => _noiseLfsr >> 23;
    internal uint WaveformEnable => Byte(0x01, 5, 1);
    internal uint NoteSelect => Byte(0x08, 6, 1);
    internal uint LfoAmDepth => Byte(0xbd, 7, 1);
    internal uint LfoPmDepth => Byte(0xbd, 6, 1);
    internal uint RhythmEnable => Byte(0xbd, 5, 1);

    internal uint ChBlockFreq(int ch) => (Byte(0xb0, 0, 5, ch) << 8) | Byte(0xa0, 0, 8, ch);
    internal uint ChFeedback(int ch) => Byte(0xc0, 1, 3, ch);
    internal uint ChAlgorithm(int ch) => Byte(0xc0, 0, 1, ch);

    internal uint OpLfoAmEnable(int op) => Byte(0x20, 7, 1, op);
    internal uint OpLfoPmEnable(int op) => Byte(0x20, 6, 1, op);
    internal uint OpEgSustain(int op) => Byte(0x20, 5, 1, op);
    internal uint OpKsr(int op) => Byte(0x20, 4, 1, op);
    internal uint OpMultiple(int op) => Byte(0x20, 0, 4, op);
    internal uint OpKsl(int op)
    {
        uint t = Byte(0x40, 6, 2, op);
        return OplTables.Bits(t, 1) | (OplTables.Bits(t, 0) << 1);
    }
    internal uint OpTotalLevel(int op) => Byte(0x40, 0, 6, op);
    internal uint OpAttackRate(int op) => Byte(0x60, 4, 4, op);
    internal uint OpDecayRate(int op) => Byte(0x60, 0, 4, op);
    internal uint OpSustainLevel(int op) => Byte(0x80, 4, 4, op);
    internal uint OpReleaseRate(int op) => Byte(0x80, 0, 4, op);
    internal int OpWaveform(int op) => WaveformEnable != 0 ? (int)Byte(0xe0, 0, 2, op) : 0;

    /// <summary>Envelope state and 10-bit attenuation (0 = loudest) of operator 0–17 (for tests and diagnostics).</summary>
    public (EnvelopeState State, uint Attenuation) Envelope(int opnum) =>
        (_operators[opnum].EnvState, _operators[opnum].EnvAttenuation);
}
