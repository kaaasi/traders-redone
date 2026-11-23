// Translated to C# from ymfm (https://github.com/aaronsgiles/ymfm, fm_channel in src/ymfm_fm.ipp),
// Copyright (c) 2021, Aaron Giles, BSD 3-Clause License – see THIRD_PARTY.md.
// Specialised for the YM3812: two operators, one output, modulator delayed by one sample.

namespace Traders.Audio.Opl;

/// <summary>One two-operator channel, including the special rhythm outputs of channels 6–8.</summary>
internal sealed class Opl2Channel
{
    private readonly Opl2 _chip;
    private readonly int _choffs;
    private readonly Opl2Operator _op1, _op2;
    private short _feedback0, _feedback1; // last two outputs of operator 1
    private short _feedbackIn;            // next feedback input, set during output

    public Opl2Channel(Opl2 chip, int choffs, Opl2Operator op1, Opl2Operator op2)
    {
        _chip = chip;
        _choffs = choffs;
        _op1 = op1;
        _op2 = op2;
        op1.ChannelOffset = choffs;
        op2.ChannelOffset = choffs;
    }

    public void Reset()
    {
        _feedback0 = _feedback1 = 0;
        _feedbackIn = 0;
    }

    /// <summary>Key on/off: bit 0 = operator 1, bit 1 = operator 2.</summary>
    public void KeyOnOff(uint states, bool rhythm)
    {
        _op1.KeyOnOff(states & 1, rhythm);
        _op2.KeyOnOff((states >> 1) & 1, rhythm);
    }

    public bool Prepare()
    {
        bool a = _op1.Prepare();
        bool b = _op2.Prepare();
        return a | b;
    }

    public void Clock(uint envCounter, int lfoRawPm)
    {
        _feedback0 = _feedback1;
        _feedback1 = _feedbackIn;
        _op1.Clock(envCounter, lfoRawPm);
        _op2.Clock(envCounter, lfoRawPm);
    }

    private int Operator1WithFeedback(uint amOffset)
    {
        int opmod = 0;
        uint feedback = _chip.ChFeedback(_choffs);
        if (feedback != 0) opmod = (_feedback0 + _feedback1) >> (int)(10 - feedback);
        int value = _op1.ComputeVolume((uint)((int)_op1.Phase + opmod), amOffset);
        _feedbackIn = (short)value;
        return value;
    }

    /// <summary>Normal two-operator output (algorithm 0: FM, 1: additive).</summary>
    public int Output2Op(int rshift, int clipmax)
    {
        uint am = _chip.LfoAm;
        Operator1WithFeedback(am);
        int result;
        if ((_chip.ChAlgorithm(_choffs) & 1) == 0)
        {
            // OPL2 modulates with the previous sample of operator 1
            int opmod = _feedback1 >> 1;
            result = _op2.ComputeVolume((uint)((int)_op2.Phase + opmod), am) >> rshift;
        }
        else
        {
            result = _feedback1 >> rshift;
            result += _op2.ComputeVolume(_op2.Phase, am) >> rshift;
            result = Math.Clamp(result, -clipmax - 1, clipmax);
        }
        return result;
    }

    /// <summary>Channel 6 in rhythm mode: bass drum (operator 1 ignored instead of added for algorithm 1).</summary>
    public int OutputRhythmCh6(int rshift, int clipmax)
    {
        uint am = _chip.LfoAm;
        int opout1 = Operator1WithFeedback(am);
        int opmod = (_chip.ChAlgorithm(_choffs) & 1) != 0 ? 0 : (opout1 >> 1);
        int result = _op2.ComputeVolume((uint)((int)_op2.Phase + opmod), am) >> rshift;
        return result * 2;
    }

    /// <summary>Channel 7 in rhythm mode: high hat (operator 1) and snare drum (operator 2).</summary>
    public int OutputRhythmCh7(uint phaseSelect, int rshift, int clipmax)
    {
        uint am = _chip.LfoAm;
        uint noise = _chip.NoiseState & 1;
        uint phase = (phaseSelect << 9) | (0xd0u >> (int)(2 * (noise ^ phaseSelect)));
        int result = _op1.ComputeVolume(phase, am) >> rshift;
        uint op13Phase = _op1.Phase;
        phase = (0x100u << (int)OplTables.Bits(op13Phase, 8)) ^ (noise << 8);
        result += _op2.ComputeVolume(phase, am) >> rshift;
        result = Math.Clamp(result, -clipmax - 1, clipmax);
        return result * 2;
    }

    /// <summary>Channel 8 in rhythm mode: tom tom (operator 1) and top cymbal (operator 2).</summary>
    public int OutputRhythmCh8(uint phaseSelect, int rshift, int clipmax)
    {
        uint am = _chip.LfoAm;
        int result = _op1.ComputeVolume(_op1.Phase, am) >> rshift;
        uint phase = 0x100u | (phaseSelect << 9);
        result += _op2.ComputeVolume(phase, am) >> rshift;
        result = Math.Clamp(result, -clipmax - 1, clipmax);
        return result * 2;
    }
}
