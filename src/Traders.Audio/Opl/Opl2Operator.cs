// Translated to C# from ymfm (https://github.com/aaronsgiles/ymfm, fm_operator in src/ymfm_fm.ipp and the OPL parts of
// src/ymfm_opl.cpp), Copyright (c) 2021, Aaron Giles, BSD 3-Clause License – see THIRD_PARTY.md.
// Specialised for the YM3812: no SSG-EG, no depress or reverb state, no detune, no envelope shift.

namespace Traders.Audio.Opl;

/// <summary>Envelope generator state of an operator.</summary>
public enum EnvelopeState
{
    Attack = 1,
    Decay = 2,
    Sustain = 3,
    Release = 4,
}

/// <summary>One FM operator ("slot"): phase generator, envelope generator and waveform lookup.</summary>
internal sealed class Opl2Operator
{
    /// <summary>Attenuation above which the operator is treated as silent.</summary>
    private const uint EgQuiet = 0x380;

    private const int KeyOnNormal = 0, KeyOnRhythm = 1, KeyOnCsm = 2;

    private readonly Opl2 _chip;
    private readonly int _opoffs;
    private int _choffs;

    private uint _phase;            // 10.10 phase
    private uint _envAttenuation;   // 4.6 attenuation, 10 bits
    private EnvelopeState _envState;
    private uint _keyState;
    private uint _keyOnLive;        // bit 0 direct, bit 1 rhythm, bit 2 CSM

    // cache filled by Prepare
    private ushort[] _waveform = OplTables.Waveforms[0];
    private uint _phaseStep;
    private bool _phaseStepDynamic;
    private uint _totalLevel;
    private uint _blockFreq;
    private uint _multiple;
    private uint _egSustain;
    private readonly uint[] _egRate = new uint[5];

    public Opl2Operator(Opl2 chip, int opoffs)
    {
        _chip = chip;
        _opoffs = opoffs;
        Reset();
    }

    public int OperatorOffset => _opoffs;

    public int ChannelOffset { get => _choffs; set => _choffs = value; }

    /// <summary>Upper 10 bits of the phase.</summary>
    public uint Phase => _phase >> 10;

    internal EnvelopeState EnvState => _envState;

    internal uint EnvAttenuation => _envAttenuation;

    public void Reset()
    {
        _phase = 0;
        _envAttenuation = 0x3ff;
        _envState = EnvelopeState.Release;
        _keyState = 0;
        _keyOnLive = 0;
    }

    public void KeyOnOff(uint on, int type) =>
        _keyOnLive = (_keyOnLive & ~(1u << type)) | ((on & 1) << type);

    public void KeyOnOff(uint on, bool rhythm) => KeyOnOff(on, rhythm ? KeyOnRhythm : KeyOnNormal);

    /// <summary>Caches the register data and applies key changes; returns true while the operator is audible.</summary>
    public bool Prepare()
    {
        CacheOperatorData();
        ClockKeyState(_keyOnLive != 0 ? 1u : 0u);
        _keyOnLive &= ~(1u << KeyOnCsm);
        return _envState != EnvelopeState.Release || _envAttenuation < EgQuiet;
    }

    public void Clock(uint envCounter, int lfoRawPm)
    {
        if (OplTables.Bits(envCounter, 0, 2) == 0) ClockEnvelope(envCounter >> 2);
        uint step = _phaseStepDynamic ? ComputePhaseStep(lfoRawPm) : _phaseStep;
        _phase += step;
    }

    /// <summary>14-bit signed output for a 10-bit phase (with modulation already added) and an AM offset.</summary>
    public int ComputeVolume(uint phase, uint amOffset)
    {
        if (_envAttenuation > EgQuiet) return 0;
        uint sinAttenuation = _waveform[phase & (OplTables.WaveformLength - 1)];
        uint envAttenuation = EnvelopeAttenuation(amOffset) << 2;
        int result = (int)OplTables.AttenuationToVolume((sinAttenuation & 0x7fff) + envAttenuation);
        return OplTables.Bits(sinAttenuation, 15) != 0 ? -result : result;
    }

    private void CacheOperatorData()
    {
        var r = _chip;
        _waveform = OplTables.Waveforms[r.OpWaveform(_opoffs)];

        uint blockFreq = _blockFreq = r.ChBlockFreq(_choffs);

        // 4-bit key code: block plus one FNUM bit chosen by the note select flag (reversed from the manual)
        uint keycode = OplTables.Bits(blockFreq, 10, 3) << 1;
        keycode |= OplTables.Bits(blockFreq, 9 - (int)r.NoteSelect, 1);

        // multiple as x.1: 0,1,2,3,4,5,6,7,8,9,10,10,12,12,15,15 with 0 meaning 0.5
        uint multiple = r.OpMultiple(_opoffs);
        _multiple = ((multiple & 0xe) | OplTables.Bits(0xc2aa, (int)multiple)) * 2;
        if (_multiple == 0) _multiple = 1;

        if (r.OpLfoPmEnable(_opoffs) == 0)
        {
            _phaseStepDynamic = false;
            _phaseStep = ComputePhaseStep(0);
        }
        else
        {
            _phaseStepDynamic = true;
        }

        _totalLevel = r.OpTotalLevel(_opoffs) << 3;
        uint ksl = r.OpKsl(_opoffs);
        if (ksl != 0)
            _totalLevel += OplTables.KeyScaleAttenuation(OplTables.Bits(blockFreq, 10, 3), OplTables.Bits(blockFreq, 6, 4)) << (int)ksl;

        // 4-bit sustain level, 15 means 31
        _egSustain = r.OpSustainLevel(_opoffs);
        _egSustain |= (_egSustain + 1) & 0x10;
        _egSustain <<= 5;

        uint ksrval = keycode >> (int)(2 * (r.OpKsr(_opoffs) ^ 1));
        _egRate[(int)EnvelopeState.Attack] = OplTables.EffectiveRate(r.OpAttackRate(_opoffs) * 4, ksrval);
        _egRate[(int)EnvelopeState.Decay] = OplTables.EffectiveRate(r.OpDecayRate(_opoffs) * 4, ksrval);
        _egRate[(int)EnvelopeState.Sustain] = r.OpEgSustain(_opoffs) != 0 ? 0 : OplTables.EffectiveRate(r.OpReleaseRate(_opoffs) * 4, ksrval);
        _egRate[(int)EnvelopeState.Release] = OplTables.EffectiveRate(r.OpReleaseRate(_opoffs) * 4, ksrval);
    }

    private uint ComputePhaseStep(int lfoRawPm)
    {
        if (_chip.OpLfoPmEnable(_opoffs) == 0) lfoRawPm = 0;
        // FNUM as a 12-bit fraction, PM from the upper three FNUM bits; low 12 bits are the same as ymfm's unsigned arithmetic
        int fnum = (int)OplTables.Bits(_blockFreq, 0, 10) << 2;
        fnum += (lfoRawPm * (int)OplTables.Bits(_blockFreq, 7, 3)) >> 1;
        fnum &= 0xfff;
        int block = (int)OplTables.Bits(_blockFreq, 10, 3);
        uint phaseStep = ((uint)fnum << block) >> 2;
        return (phaseStep * _multiple) >> 1;
    }

    private void ClockKeyState(uint keystate)
    {
        if ((keystate ^ _keyState) == 0) return;
        _keyState = keystate;
        if (keystate != 0) StartAttack();
        else StartRelease();
    }

    private void StartAttack()
    {
        if (_envState == EnvelopeState.Attack) return;
        _envState = EnvelopeState.Attack;
        _phase = 0;
        if (_egRate[(int)EnvelopeState.Attack] >= 62) _envAttenuation = 0;
    }

    private void StartRelease()
    {
        if (_envState >= EnvelopeState.Release) return;
        _envState = EnvelopeState.Release;
    }

    private void ClockEnvelope(uint envCounter)
    {
        if (_envState == EnvelopeState.Attack && _envAttenuation == 0) _envState = EnvelopeState.Decay;
        // directly after attack→decay, so sustain level 0 skips the decay
        if (_envState == EnvelopeState.Decay && _envAttenuation >= _egSustain) _envState = EnvelopeState.Sustain;

        uint rate = _egRate[(int)_envState];
        int rateShift = (int)(rate >> 2);
        envCounter <<= rateShift;
        if (OplTables.Bits(envCounter, 0, 11) != 0) return;

        uint relevantBits = OplTables.Bits(envCounter, rateShift <= 11 ? 11 : rateShift, 3);
        uint increment = OplTables.AttenuationIncrement(rate, relevantBits);

        if (_envState == EnvelopeState.Attack)
        {
            // rates 62/63 do not move when changed after the key on (handled in StartAttack)
            if (rate < 62)
            {
                int att = (int)_envAttenuation;
                att += (~att * (int)increment) >> 4;
                _envAttenuation = (uint)att & 0xffff;
            }
        }
        else
        {
            _envAttenuation += increment;
            if (_envAttenuation >= 0x400) _envAttenuation = 0x3ff;
        }
    }

    private uint EnvelopeAttenuation(uint amOffset)
    {
        uint result = _envAttenuation;
        if (_chip.OpLfoAmEnable(_opoffs) != 0) result += amOffset;
        result += _totalLevel;
        return Math.Min(result, 0x3ffu);
    }
}
