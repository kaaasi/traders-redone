namespace Traders.Audio.Effects;

public sealed partial class EffectMachine
{
    /// <summary>How a sequence command continues in the voice loop of 0000:0866.</summary>
    private enum Next
    {
        /// <summary>jmp 08D3: read the next event of the sequence.</summary>
        Read,

        /// <summary>jmp 0936: run the voice's per-tick routines [DI+29h] and [DI+2Bh].</summary>
        Routines,

        /// <summary>jmp 093C: next voice.</summary>
        NextVoice,
    }

    /// <summary>
    /// Set when the slide routine 0000:148A pops the return address of its caller (stack slip at 14C2/14DE): its final ret then
    /// returns from 0866 itself, so the remaining voices are skipped in this tick.
    /// </summary>
    private bool _voiceLoopAborted;

    // [ORIGINAL] 0000:0866
    /// <summary>0000:0866 – steps the voices 9…0 (CS:03EF) while the machine is active.</summary>
    private void StepVoices()
    {
        if (B(Active) == 0) return;
        SetB(CurrentVoice, 9);
        while (true)
        {
            // 0873
            BL = B(CurrentVoice);
            BH = 0;
            BL = (byte)(BL << 1);
            _di = W(_bx + VoiceBlocks);
            AL = V(0x05);
            if (AL != 0)
            {
                StepVoice();
                if (_voiceLoopAborted)
                {
                    _voiceLoopAborted = false;
                    return;
                }
            }
            // 093C
            byte v = (byte)(B(CurrentVoice) - 1);
            SetB(CurrentVoice, v);
            if ((sbyte)v < 0) return;
        }
    }

    /// <summary>0000:0889–093B for one active voice (DI).</summary>
    private void StepVoice()
    {
        // 0889
        _bx = ChannelOffsets;
        BL = (byte)(BL + B(CurrentVoice));
        AL = B(_bx);
        SetB(ChannelOffset, AL);
        if (V(0x40) != 0)
        {
            AL = B(SpeedShared);
            SetV(0x25, AL);
        }
        int sum = V(0x26) + V(0x25);
        AL = (byte)sum;
        SetV(0x26, AL);
        Next next;
        if (sum < 0x100) next = Next.Routines;
        else
        {
            // 08AF
            byte count = (byte)(V(0x05) - 1);
            SetV(0x05, count);
            if (count != 0)
            {
                AL = V(0x05);
                if (AL == V(0x21)) KeyOff();
                if (AL == V(0x35) && B(CurrentVoice) != 9) KeyOff();
                next = Next.Routines;
            }
            else
            {
                // 08D0
                _si = W(_di + 0x03);
                next = Next.Read;
            }
        }
        while (next == Next.Read) next = ReadEvent();
        if (next == Next.NextVoice) return;
        // 0936
        CallRoutine(W(_di + 0x29), 0x0939);
        if (_voiceLoopAborted) return;
        CallRoutine(W(_di + 0x2B), 0x093C);
    }

    /// <summary>0000:08D3 – reads one event: a note (AL &lt; 80h, AH = duration) or a command over the jump table CS:151E.</summary>
    private Next ReadEvent()
    {
        Lodsw();
        if ((AL & 0x80) == 0)
        {
            // 08DA: note
            SetNote();
            KeyOn();
            SetDuration();
            if (W(_di + 0x32) != 0 || W(_di + 0x33) != 0)
            {
                // 08F1
                Lodsb();
                SetV(0x34, AL);
                CarrierLevel();
                _dx = 0x0388;
                AH = (byte)(B(ChannelOffset) + 0x43);
                Opl();
                ModulatorLevel();
                _dx = 0x0388;
                AH = (byte)(B(ChannelOffset) + 0x40);
                Opl();
            }
            // 0915
            AL = B(NoteDuration);
            if (AL == 0) return Next.Read;
            SetW(_di + 0x03, _si);
            return Next.Routines;
        }
        // 0921
        AL &= 0x7F;
        if ((sbyte)AL > 0x70) return EndSequence();
        BL = (byte)(AL << 1);
        BH = 0;
        _bx = W(_bx + JumpTable);
        return Command(_bx);
    }

    /// <summary>Calls the per-tick routine at <paramref name="address"/> (call [DI+29h] / [DI+2Bh]).</summary>
    private void CallRoutine(ushort address, ushort returnAddress)
    {
        switch (address)
        {
            case 0x1407: return;
            case 0x1408: Vibrato(); return;
            case 0x1461: TableStep(); return;
            case 0x148A: Slide(returnAddress); return;
            default:
                // [APPROX] Only the four routine addresses the commands set are ported; another value means corrupted voice
                // data, on which the original would run arbitrary code.
                throw new NotSupportedException($"Effect machine: routine address {address:X4} is not a routine.");
        }
    }

    // [ORIGINAL] 0000:131F
    /// <summary>0000:131F – clears bytes 03h…42h of the voice block DI, sets speed FFh, priority 0, routines 1407h, 35h = 1.</summary>
    private void ClearVoice()
    {
        for (int i = 0; i < 0x40; i++) SetB(_di + 0x03 + i, 0);
        SetV(0x25, 0xFF);
        SetV(0x08, 0);
        SetW(_di + 0x29, 0x1407);
        SetW(_di + 0x2B, 0x1407);
        SetV(0x35, 1);
    }

    // [ORIGINAL] 0000:13BE
    /// <summary>0000:13BE – mutes the channel of voice CL (release FFh, key off, then B0h+v = 20h) unless rhythm mode owns it.</summary>
    private void MuteChannel()
    {
        if (B(Rhythm) != 0 && (sbyte)CL >= 6) return;
        _bx = ChannelOffsets;
        CH = CL;
        BL = (byte)(BL + CL);
        CL = B(_bx);
        AH = (byte)(0x60 + CL); AL = 0xFF; Opl();
        AH = (byte)(0x63 + CL); AL = 0xFF; Opl();
        AH = (byte)(0x80 + CL); AL = 0xFF; Opl();
        AH = (byte)(0x83 + CL); AL = 0xFF; Opl();
        AH = (byte)(0xB0 + CH); AL = 0x00; Opl();
        AL = 0x20; Opl();
    }

    // [ORIGINAL] 0000:10B5
    /// <summary>0000:10B5 – note AL (high nibble octave, low nibble semitone) with transpose, fine tune and bend to A0h/B0h.</summary>
    private void SetNote()
    {
        ushort saved = _ax;
        SetV(0x41, AL);
        NoteFrequency(0x10B5);
        _ax = saved;
    }

    /// <summary>
    /// Shared body of 0000:10B5 and 0000:0CBD: frequency of note AL into CX (CL = A0h value, CH = B0h value), bend [DI+42h] from
    /// the tables over CS:020C, stored in [DI+27h]/[DI+28h] and written. <paramref name="entry"/> selects the variant: 10B5 skips
    /// the bend when [DI+42h] = 0, 0CBD does not test for zero.
    /// </summary>
    private void NoteFrequency(int entry)
    {
        CH = AL;
        CH &= 0xF0;
        BL = V(0x07);
        CH = (byte)(CH + BL);
        AL &= 0x0F;
        BL = V(0x12);
        AL = (byte)(AL + BL);
        if ((sbyte)AL >= 0x0C)
        {
            AL = (byte)(AL - 0x0C);
            CH = (byte)(CH + 0x10);
        }
        else if ((sbyte)AL < 0)
        {
            AL = (byte)(AL + 0x0C);
            CH = (byte)(CH - 0x10);
        }
        BL = AL;
        BH = 0;
        BL = (byte)(BL << 1);
        _ax = W(_bx + 0x06E6);
        _dx = 0;
        DL = V(0x24);
        _ax += _dx;
        CH = (byte)(CH >> 2);
        CH &= 0x1C;
        CH |= AH;
        CL = AL;
        sbyte bend = (sbyte)V(0x42);
        if (entry == 0x10B5 && bend == 0)
        {
            // 1143
        }
        else if (bend >= 0)
        {
            BH = 0;
            BL = V(0x41);
            BL &= 0x0F;
            BL += 2;
            _bx <<= 1;
            ushort table = W(_bx + 0x020C);
            BH = 0;
            BL = V(0x42);
            AL = B(_bx + table);
            AH = 0;
            _cx += _ax;
        }
        else
        {
            BH = 0;
            BL = V(0x41);
            BL &= 0x0F;
            _bx <<= 1;
            ushort table = W(_bx + 0x020C);
            BH = 0;
            BL = V(0x42);
            BL ^= 0xFF;
            BL++;
            AL = B(_bx + table);
            AH = 0;
            _cx -= _ax;
        }
        // 1143 / 0D4E
        AH = 0xA0;
        AL = V(0x28);
        AL &= 0x20;
        CH |= AL;
        SetV(0x28, CH);
        SetV(0x27, CL);
        AH = (byte)(0xA0 + B(CurrentVoice));
        AL = CL;
        Opl();
        AH = (byte)(0xB0 + B(CurrentVoice));
        AL = CH;
        Opl();
    }

    // [ORIGINAL] 0000:116A
    /// <summary>0000:116A – key on (B0h+v with bit 20h) and vibrato start values from the frequency.</summary>
    private void KeyOn()
    {
        ushort saved = _ax;
        AH = (byte)(0xB0 + B(CurrentVoice));
        AL = (byte)(V(0x28) | 0x20);
        SetV(0x28, AL);
        Opl();
        CL = V(0x19);
        AL = (byte)(9 - CL);
        CL = AL;
        _ax = W(_di + 0x27);
        _ax &= 0x03FF;
        _ax = (ushort)(_ax >> (CL & 0x1F));
        AH = 0;
        SetW(_di + 0x17, _ax);
        CL = V(0x1C);
        SetV(0x1F, CL);
        _ax = saved;
    }

    // [ORIGINAL] 0000:119A
    /// <summary>0000:119A – key off of the current voice (B0h+v without bit 20h), not for voice 9 and not in rhythm mode for 6…8.</summary>
    private void KeyOff()
    {
        if (B(Rhythm) != 0 && (sbyte)B(CurrentVoice) >= 6) return;
        if (B(CurrentVoice) == 9) return;
        AH = (byte)(0xB0 + B(CurrentVoice));
        AL = (byte)(V(0x28) & 0xDF);
        SetV(0x28, AL);
        Opl();
    }

    // [ORIGINAL] 0000:11C2
    /// <summary>0000:11C2 – duration AH into CS:03EC and [DI+5]; random length ([DI+36h]) or key-off point ([DI+2Dh]).</summary>
    private void SetDuration()
    {
        SetB(NoteDuration, AH);
        BL = AH;
        if (V(0x36) != 0)
        {
            Random();
            AL &= V(0x36);
            BL = (byte)(BL + AL);
            SetV(0x05, BL);
            return;
        }
        if (V(0x2D) != 0)
        {
            BL = AH;
            AH = (byte)(AH >> 3);
            CL = V(0x2D);
            CH = 0;
            AL = 0;
            do
            {
                AL = (byte)(AL + AH);
                _cx--;
            } while (_cx != 0);
            SetV(0x21, AL);
        }
        SetV(0x05, BL);
    }

    // [ORIGINAL] 0000:120B
    /// <summary>0000:120B – clamps AX to 0…3Fh (signed) into AL.</summary>
    private void ClampLevel()
    {
        if ((short)_ax < 0) AL = 0;
        else if ((short)_ax > 0x3F) AL = 0x3F;
    }

    // [ORIGINAL] 0000:127A
    /// <summary>0000:127A – rewrites the carrier level (43h+ch) and, for additive voices, the modulator level (40h+ch).</summary>
    private void WriteLevels()
    {
        CarrierLevel();
        _bx = ChannelOffsets;
        BL = (byte)(BL + B(CurrentVoice));
        CL = B(_bx);
        AH = (byte)(CL + 0x43);
        Opl();
        if (V(0x31) == 0) return;
        ModulatorLevel();
        _bx = ChannelOffsets;
        BL = (byte)(BL + B(CurrentVoice));
        CL = B(_bx);
        AH = (byte)(CL + 0x40);
        Opl();
    }

    // [ORIGINAL] 0000:12A9
    /// <summary>0000:12A9 – carrier level: [2Fh] &amp; 3Fh + [20h] + [0] + [30h] − ([34h] &lt;&lt; ([33h]+1)).high, clamped, with KSL bits.</summary>
    private void CarrierLevel()
    {
        AL = (byte)((V(0x2F) & 0x3F) + V(0x20) + V(0x00) + V(0x30));
        if (V(0x33) != 0)
        {
            CL = (byte)(V(0x33) + 1);
            BH = 0;
            BL = V(0x34);
            _bx = (ushort)(_bx << (CL & 0x1F));
            AL = (byte)(AL - BH);
        }
        ClampLevelByte();
        BL = (byte)(V(0x2F) & 0xC0);
        AL |= BL;
    }

    // [ORIGINAL] 0000:12E1
    /// <summary>0000:12E1 – modulator level: like the carrier for additive voices ([31h] ≠ 0), else only [2Eh] − bend of [32h].</summary>
    private void ModulatorLevel()
    {
        AL = (byte)(V(0x2E) & 0x3F);
        if (V(0x31) != 0) AL = (byte)(AL + V(0x20) + V(0x00) + V(0x30));
        if (V(0x32) != 0)
        {
            CL = (byte)(V(0x32) + 1);
            BH = 0;
            BL = V(0x34);
            _bx = (ushort)(_bx << (CL & 0x1F));
            AL = (byte)(AL - BH);
        }
        ClampLevelByte();
        BL = (byte)(V(0x2E) & 0xC0);
        AL |= BL;
    }

    /// <summary>12CA/1308: above 3Fh (unsigned) becomes 0 when negative (signed), else 3Fh.</summary>
    private void ClampLevelByte()
    {
        if (AL <= 0x3F) return;
        AL = (sbyte)AL < 0 ? (byte)0 : (byte)0x3F;
    }

    // [ORIGINAL] 0000:1352
    /// <summary>0000:1352 – loads the instrument at SI into operator slots CL (and CL+3) and feedback register C0h+voice.</summary>
    private void LoadInstrument()
    {
        AH = (byte)(0x20 + CL); Lodsb(); Opl();
        AH = (byte)(0x23 + CL); Lodsb(); Opl();
        AH = (byte)(0xC0 + B(CurrentVoice)); Lodsb(); Opl();
        AL &= 1;
        SetV(0x31, AL);
        AH = (byte)(0xE0 + CL); Lodsb(); Opl();
        AH = (byte)(0xE3 + CL); Lodsb(); Opl();
        AH = (byte)(0x40 + CL); Lodsb();
        SetV(0x2E, AL);
        ModulatorLevel();
        Opl();
        AH = (byte)(0x43 + CL); Lodsb();
        SetV(0x2F, AL);
        CarrierLevel();
        Opl();
        AH = (byte)(0x60 + CL); Lodsb(); Opl();
        AH = (byte)(0x63 + CL); Lodsb(); Opl();
        AH = (byte)(0x80 + CL); Lodsb(); Opl();
        AH = (byte)(0x83 + CL); Lodsb(); Opl();
    }

    // [ORIGINAL] 0000:150E
    /// <summary>0000:150E – generator of the effect machine: CS:03F8 = ror3(CS:03F8 + 9248h), result in AX.</summary>
    private void Random()
    {
        _ax = (ushort)(W(RandomState) + 0x9248);
        _ax = (ushort)((_ax >> 3) | (_ax << 13));
        SetW(RandomState, _ax);
    }

    // [ORIGINAL] 0000:1408
    /// <summary>0000:1408 – vibrato: after the delay [1Fh], add [1Dh] to [1Eh]; on carry move the frequency by ±[17h].</summary>
    private void Vibrato()
    {
        if (V(0x1F) != 0)
        {
            SetV(0x1F, (byte)(V(0x1F) - 1));
            return;
        }
        int sum = V(0x1E) + V(0x1D);
        CL = (byte)sum;
        SetV(0x1E, CL);
        if (sum < 0x100) return;
        _bx = W(_di + 0x17);
        byte count = (byte)(V(0x1A) - 1);
        SetV(0x1A, count);
        if (count == 0)
        {
            _bx ^= 0xFFFF;
            _bx++;
            SetW(_di + 0x17, _bx);
            AL = V(0x1B);
            SetV(0x1A, AL);
        }
        _ax = W(_di + 0x27);
        _ax &= 0x03FF;
        _ax += _bx;
        SetV(0x27, AL);
        AL = V(0x28);
        AL &= 0xFC;
        AL |= AH;
        SetV(0x28, AL);
        AH = (byte)(0xA0 + B(CurrentVoice));
        AL = V(0x27);
        Opl();
        AH = (byte)(0xB0 + B(CurrentVoice));
        AL = V(0x28);
        Opl();
    }

    // [ORIGINAL] 0000:1461
    /// <summary>0000:1461 – table step: on carry of [3Ah] += [39h] write table entry [3Eh][[3Ch]] to register [3Dh] + channel.</summary>
    private void TableStep()
    {
        int sum = V(0x3A) + V(0x39);
        AL = (byte)sum;
        SetV(0x3A, AL);
        if (sum < 0x100) return;
        byte index = (byte)(V(0x3C) - 1);
        SetV(0x3C, index);
        if ((sbyte)index < 0)
        {
            AL = V(0x3B);
            SetV(0x3C, AL);
        }
        AH = (byte)(V(0x3D) + B(ChannelOffset));
        AL = V(0x3C);
        _bx = W(_di + 0x3E);
        AL = B(_bx + AL);
        Opl();
    }

    // [ORIGINAL] 0000:148A
    /// <summary>
    /// 0000:148A – frequency slide by the signed word [15h] on carry of [14h] += [13h], with octave change above 2DEh / below 185h.
    /// When the halved/doubled frequency has low ten bits 0, the original pops its own return address (14C2/14DE): BX becomes that
    /// address (0939h) and the final ret leaves 0866, ending the voice loop for this tick.
    /// </summary>
    private void Slide(ushort returnAddress)
    {
        int sum = V(0x14) + V(0x13);
        CL = (byte)sum;
        SetV(0x14, CL);
        if (sum < 0x100) return;
        BL = V(0x27);
        BH = V(0x28);
        BH &= 0x03;
        DL = V(0x28);
        DH = DL;
        DL &= 0x1C;
        DH &= 0x20;
        _cx = W(_di + 0x15);
        if ((short)_cx >= 0)
        {
            _bx += _cx;
            if ((short)_bx >= 0x02DE)
            {
                _bx >>= 1;
                if ((_bx & 0x03FF) == 0) PopReturnAddress(returnAddress);
                DL = (byte)((DL + 4) & 0x1C);
            }
        }
        else
        {
            _bx += _cx;
            if ((short)_bx <= 0x0184)
            {
                _bx <<= 1;
                if ((_bx & 0x03FF) == 0) PopReturnAddress(returnAddress);
                DL = (byte)((DL - 4) & 0x1C);
            }
        }
        // 14E7
        _bx &= 0x03FF;
        _cx = _dx;
        AH = (byte)(0xA0 + B(CurrentVoice));
        AL = BL;
        Opl();
        SetV(0x27, AL);
        AH = (byte)(0xB0 + B(CurrentVoice));
        BH |= CL;
        BH |= CH;
        AL = BH;
        Opl();
        SetV(0x28, AL);
    }

    /// <summary>14C2/14DE: pop bx takes the return address of the call (0939h for [DI+29h]); the routine's ret then returns from 0866.</summary>
    private void PopReturnAddress(ushort returnAddress)
    {
        _bx = returnAddress;
        _voiceLoopAborted = true;
    }
}
