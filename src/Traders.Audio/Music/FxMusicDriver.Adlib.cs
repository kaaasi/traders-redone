namespace Traders.Audio.Music;

/// <summary>AdLib layer of FXDRIVER (FXDRIVER.md §5): voices, operators and the register writes.</summary>
public sealed partial class FxMusicDriver
{
    /// <summary>Register write (CS:0008): register to the port at DS:01D6, value to the port + 1; the read delays carry no data.</summary>
    /// <remarks>[ORIGINAL-FX] 2928</remarks>
    private void Out(int register, int value)
    {
        Written?.Invoke((byte)register, (byte)value);
        WrittenAt?.Invoke(new Opl.OplWrite(CurrentTick, (byte)register, (byte)value));
    }

    /// <summary>Melodic (9 voices) or percussion mode (6 melodic + 5 drums).</summary>
    /// <remarks>[ORIGINAL-FX] 2A3F</remarks>
    private void SetMode(int mode)
    {
        if (mode != 0)
        {
            SetB(Note + 8, 0x18);
            SetW(Bend + 2 * 8, 0x2000);
            SetFrequency(8);
            SetB(Note + 7, 0x1F);
            SetW(Bend + 2 * 7, 0x2000);
            SetFrequency(7);
        }
        SetB(Percussion, mode);
        SetW(VoiceCount, mode != 0 ? 0x0B : 0x09);
        SetB(Rhythm, 0);
        DefaultOperators();
        WriteRhythm();
    }

    /// <summary>Wave select enable (register 01h bit 5) and all wave forms 0.</summary>
    /// <remarks>[ORIGINAL-FX] 2AA1</remarks>
    private void SetWaveSelect(int on)
    {
        SetW(WaveSelect, on != 0 ? 0x20 : 0);
        for (int slot = 0; slot < 0x12; slot++) Out(0xE0 + B(SlotOffset + slot), 0);
        Out(1, W(WaveSelect));
    }

    /// <remarks>[ORIGINAL-FX] 2AF5</remarks>
    private void SetPitchRange(int range) => SetW(PitchRange, Math.Clamp(range, 1, 0x0C));

    /// <summary>AM depth, vibrato depth and note select (registers BDh and 08h).</summary>
    /// <remarks>[ORIGINAL-FX] 2B1D</remarks>
    private void SetGlobalParameters(int am, int vib, int noteSelect)
    {
        SetB(AmDepth, am);
        SetB(VibDepth, vib);
        SetB(NoteSelect, noteSelect);
        WriteRhythm();
        WriteNoteSelect();
    }

    /// <summary>Instrument at DS:<paramref name="instrument"/> (69-byte record) for the one or two operators of a voice.</summary>
    /// <remarks>[ORIGINAL-FX] 2B45</remarks>
    private void SetVoiceTimbre(int voice, int instrument)
    {
        if (voice >= W(VoiceCount)) return;
        int wave1 = W(instrument + 0x34), wave2 = W(instrument + 0x36);
        int slots = (Perc ? PercussionSlots : MelodicSlots) + 2 * voice;
        SetSlotParameters(B(slots), instrument, wave1);
        if (B(slots + 1) != 0xFF) SetSlotParameters(B(slots + 1), instrument + 0x1A, wave2);
    }

    /// <remarks>[ORIGINAL-FX] 2BFA</remarks>
    private void SetVoiceVolume(int voice, int volume)
    {
        if (voice >= W(VoiceCount)) return;
        if ((ushort)volume > 0x7F) volume = 0x7F;
        SetB(Volume + voice, volume);
        int slots = (Perc ? PercussionSlots : MelodicSlots) + 2 * voice;
        WriteLevel(B(slots));
        if (B(slots + 1) != 0xFF) WriteLevel(B(slots + 1));
    }

    /// <remarks>[ORIGINAL-FX] 2C79</remarks>
    private void SetVoicePitch(int voice, int bend)
    {
        if (!(!Perc && voice < 9 || voice <= 6)) return;
        if ((ushort)bend > 0x3FFF) bend = 0x3FFF;
        SetW(Bend + 2 * voice, bend);
        SetFrequency(voice);
    }

    /// <summary>Note on; in percussion mode the drums share the pitch of voices 7/8 and set their rhythm bit.</summary>
    /// <remarks>[ORIGINAL-FX] 2CBB</remarks>
    private void NoteOn(int voice, int note)
    {
        note -= 12;
        if (note < 0) note = 0;
        if (!Perc && voice < 9 || voice < 6)
        {
            SetB(Note + voice, note);
            SetB(KeyOn + voice, 0x20);
            SetFrequency(voice);
            return;
        }
        if (!Perc || voice > 10) return;
        if (voice == 6)
        {
            SetB(Note + 6, note);
            SetFrequency(6);
        }
        else if ((voice == 8 || voice == 7) && B(Note + 8) != note)
        {
            SetB(Note + 8, note);
            SetFrequency(8);
        }
        SetB(Note + 7, note);
        SetFrequency(7);
        SetB(Rhythm, B(Rhythm) | B(PercussionMask + voice - 6));
        WriteRhythm();
    }

    /// <remarks>[ORIGINAL-FX] 2D75</remarks>
    private void NoteOff(int voice)
    {
        if (!Perc && voice < 9 || voice < 6)
        {
            SetB(KeyOn + voice, 0);
            SetB(BlockShadow + voice, B(BlockShadow + voice) & 0xDF);
            Out(0xB0 + voice, B(BlockShadow + voice));
            return;
        }
        if (!Perc || voice > 10) return;
        SetB(Rhythm, B(Rhythm) & ~B(PercussionMask + voice - 6));
        WriteRhythm();
    }

    /// <summary>Default parameters for all 18 operators (modulator/carrier), in percussion mode the drum defaults.</summary>
    /// <remarks>[ORIGINAL-FX] 2DE1 – tables DS:1536/1544 (5D46/5D54) and DS:1552…1598 (5D62…5DA8).</remarks>
    private void DefaultOperators()
    {
        for (int slot = 0; slot < 0x12; slot++)
            SetSlotFromBytes(slot, B(Carrier + slot) != 0 ? DefaultCarrier : DefaultModulator);
        if (!Perc) return;
        SetSlotFromBytes(0x0C, 0x1552);
        SetSlotFromBytes(0x0F, 0x1560);
        SetSlotFromBytes(0x10, 0x156E);
        SetSlotFromBytes(0x0E, 0x157C);
        SetSlotFromBytes(0x11, 0x158A);
        SetSlotFromBytes(0x0D, 0x1598);
    }

    /// <summary>13 byte parameters at DS:<paramref name="table"/>, wave form 0.</summary>
    /// <remarks>[ORIGINAL-FX] 2F55</remarks>
    private void SetSlotFromBytes(int slot, int table)
    {
        Span<byte> p = stackalloc byte[13];
        for (int i = 0; i < 13; i++) p[i] = B(table + i);
        StoreSlot(slot, p, 0);
    }

    /// <summary>13 word parameters at DS:<paramref name="words"/> (low bytes used).</summary>
    private void SetSlotParameters(int slot, int words, int wave)
    {
        Span<byte> p = stackalloc byte[13];
        for (int i = 0; i < 13; i++) p[i] = B(words + 2 * i);
        StoreSlot(slot, p, wave);
    }

    /// <summary>Parameters into the operator store DS:1332, then all registers of the operator.</summary>
    /// <remarks>[ORIGINAL-FX] 2EFA, 3031</remarks>
    private void StoreSlot(int slot, ReadOnlySpan<byte> p, int wave)
    {
        int at = SlotParams + slot * SlotSize;
        for (int i = 0; i < 13; i++) SetB(at + i, p[i]);
        SetB(at + 13, wave & 3);
        WriteRhythm();
        WriteNoteSelect();
        WriteLevel(slot);
        WriteFeedback(slot);
        WriteAttackDecay(slot);
        WriteSustainRelease(slot);
        WriteCharacter(slot);
        WriteWave(slot);
    }

    /// <summary>Register 40h: key scale level and the level scaled with the voice volume.</summary>
    /// <remarks>[ORIGINAL-FX] 307D</remarks>
    private void WriteLevel(int slot)
    {
        int voice = B((Perc ? PercussionVoice : MelodicVoice) + slot);
        int t = 0x3F - (P(slot, 8) & 0x3F);
        bool single = Perc && voice > 6;
        if (B(Carrier + slot) != 0 || P(slot, 12) == 0 || single)
            t = (ushort)(B(Volume + voice) * t + 0x40) >> 7;
        t = 0x3F - t;
        t |= P(slot, 0) << 6;
        Out(0x40 + B(SlotOffset + slot), t);
    }

    /// <remarks>[ORIGINAL-FX] 3151</remarks>
    private void WriteNoteSelect() => Out(8, B(NoteSelect) != 0 ? 0x40 : 0);

    /// <summary>Register C0h of the voice (modulator only): feedback and connection.</summary>
    /// <remarks>[ORIGINAL-FX] 3176</remarks>
    private void WriteFeedback(int slot)
    {
        if (B(Carrier + slot) != 0) return;
        Out(0xC0 + B(MelodicVoice + slot), P(slot, 2) << 1 | (P(slot, 12) == 0 ? 1 : 0));
    }

    /// <remarks>[ORIGINAL-FX] 31D8</remarks>
    private void WriteAttackDecay(int slot) => Out(0x60 + B(SlotOffset + slot), P(slot, 3) << 4 | P(slot, 6) & 0x0F);

    /// <remarks>[ORIGINAL-FX] 322B</remarks>
    private void WriteSustainRelease(int slot) => Out(0x80 + B(SlotOffset + slot), P(slot, 4) << 4 | P(slot, 7) & 0x0F);

    /// <summary>Register 20h: AM, vibrato, sustain, KSR, multiplier.</summary>
    /// <remarks>[ORIGINAL-FX] 327E</remarks>
    private void WriteCharacter(int slot)
    {
        int v = (P(slot, 9) != 0 ? 0x80 : 0) + (P(slot, 10) != 0 ? 0x40 : 0) + (P(slot, 5) != 0 ? 0x20 : 0)
            + (P(slot, 11) != 0 ? 0x10 : 0) + (P(slot, 1) & 0x0F);
        Out(0x20 + B(SlotOffset + slot), v);
    }

    /// <remarks>[ORIGINAL-FX] 3326</remarks>
    private void WriteRhythm() =>
        Out(0xBD, (B(AmDepth) != 0 ? 0x80 : 0) | (B(VibDepth) != 0 ? 0x40 : 0) | (Perc ? 0x20 : 0) | B(Rhythm));

    /// <remarks>[ORIGINAL-FX] 337E</remarks>
    private void WriteWave(int slot) => Out(0xE0 + B(SlotOffset + slot), W(WaveSelect) != 0 ? P(slot, 13) & 3 : 0);

    /// <summary>Frequency of a voice from its note, bend and key bit; remembers register B0h.</summary>
    /// <remarks>[ORIGINAL-FX] 33C8</remarks>
    private void SetFrequency(int voice) =>
        SetB(BlockShadow + voice, Frequency(voice, B(Note + voice), (short)W(Bend + 2 * voice), B(KeyOn + voice)));

    /// <summary>F-number and block (CS:0B8D) with the linear bend towards the neighbour note at the pitch range.</summary>
    /// <remarks>[ORIGINAL-FX] 34AD – F-numbers from DS:0028 (4838), indexed like the original also below the table for
    /// notes under the range.</remarks>
    private int Frequency(int voice, int note, short bend, int keyOn)
    {
        int block = note / 12 - 1;
        ushort f = W(FNumbers + 2 * (note % 12));
        if (bend != 0x2000)
        {
            if (bend > 0x2000)
            {
                ushort b = (ushort)(bend - 0x2000);
                ushort g = W(FNumbers + 2 * ((note + W(PitchRange)) % 12));
                if (g < f) g = (ushort)(g << 1);
                g = (ushort)(g - f);
                f = (ushort)(f + (ushort)((uint)(g * b) >> 13));
                while (f > 0x3FF)
                {
                    f >>= 1;
                    block++;
                }
            }
            else
            {
                ushort b = (ushort)(0x2000 - bend);
                ushort g = W(FNumbers + 2 * ((note - W(PitchRange)) % 12));
                if (g > f) g >>= 1;
                g = (ushort)(f - g);
                f = (ushort)(f - (ushort)((uint)(g * b) >> 13));
                while (f < W(FNumbers))
                {
                    f = (ushort)(f << 1);
                    block--;
                }
            }
        }
        Out(0xA0 + voice, f);
        int b0 = keyOn | block << 2 | f >> 8;
        Out(0xB0 + voice, b0);
        return b0 & 0xFF;
    }
}
