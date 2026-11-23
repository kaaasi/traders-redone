namespace Traders.Audio.Effects;

public sealed partial class EffectMachine
{
    // [ORIGINAL] 0000:0921, 0000:0946, 0000:09A2, 0000:09DF, 0000:0A18, 0000:0A3A, 0000:0AAD, 0000:0ABB, 0000:0AE0, 0000:0B32, 0000:0B64, 0000:0B6D, 0000:0B74, 0000:0B7D, 0000:0B83, 0000:0C09, 0000:0CB4, 0000:0D77, 0000:0D81, 0000:0D85, 0000:0DA8, 0000:10A5, 0000:10AB
    /// <summary>
    /// Runs the sequence command whose handler address <paramref name="handler"/> came from the jump table CS:151E (AL = command
    /// number, AH = parameter byte, SI after the command word).
    /// </summary>
    private Next Command(ushort handler)
    {
        switch (handler)
        {
            case 0x0946: SetV(0x06, AH); return Next.Read;                              // 00: loop count
            case 0x094B: return LoopBack();                                              // 01: loop
            case 0x0953: return StartNested();                                           // 02: start effect
            case 0x09A2: SetV(0x35, AH); return Next.Read;                              // 03: key-off point
            case 0x09A8: return Jump();                                                  // 04: jump
            case 0x09B4: return CallSequence();                                          // 05: call
            case 0x09CC: return ReturnSequence();                                        // 06: return
            case 0x09DF: SetV(0x07, AH); return Next.Read;                              // 07: octave
            case 0x09E5: return EndSequence();                                           // 08 and unused numbers: end
            case 0x09FB: return Rest(keyOff: true);                                      // 09: rest with key off
            case 0x0A18: Lodsb(); Opl(); return Next.Read;                              // 0A: OPL register AH = byte
            case 0x0A1F: return Legato();                                                // 0B: note without key on
            case 0x0A3A: SetV(0x12, AH); return Next.Read;                              // 0C: transpose
            case 0x0A40: return StartTableStep();                                        // 0D: table step
            case 0x0A5D: return HaltVoice();                                             // 0E: halt voice AH
            case 0x0A81: return Instrument();                                            // 10: instrument
            case 0x0A97: return StartSlide();                                            // 11: slide
            case 0x0AAD: _si--; SetW(_di + 0x29, 0x1407); SetW(_di + 0x15, 0); return Next.Read; // 12: slide off
            case 0x0ABB: SetV(0x24, AH); return Next.Read;                              // 13: fine tune
            case 0x0AC1: return StartVibrato();                                          // 15: vibrato
            case 0x0AE0: SetV(0x08, AH); return Next.Read;                              // 1A: priority
            case 0x0AE6: return SetSharedCounter();                                      // 1C: shared counter
            case 0x0B02: return WaitShared();                                            // 1D: wait for counter
            case 0x0B32: SetV(0x20, AH); WriteLevels(); return Next.Read;              // 1E: volume
            case 0x0B3B: return Rest(keyOff: false);                                     // 20: rest
            case 0x0B4E: return Retrigger();                                             // 21: key on again
            case 0x0B64: SetV(0x2D, (byte)(AH & 0x07)); return Next.Read;              // 24: key-off fraction
            case 0x0B6D: SetB(SpeedShared, AH); return Next.Read;                       // 26: shared speed
            case 0x0B74: _si--; SetW(_di + 0x2B, 0x1407); return Next.Read;            // 27: table step off
            case 0x0B7D: SetV(0x25, AH); return Next.Read;                              // 29: speed
            case 0x0B83: SetV(0x30, AH); return Next.Read;                              // 2B: level offset
            case 0x0B89: return SetOtherVolume(add: false);                              // 2C: volume of voice AH
            case 0x0BAA: return SetOtherVolume(add: true);                               // 2D: add volume of voice AH
            case 0x0BCD: return RhythmBit(0x07, 0x7F);                                   // 2E: AM depth bit
            case 0x0BEB: return RhythmBit(0x06, 0xBF);                                   // 2F: vibrato depth bit
            case 0x0C09: SetV(0x20, (byte)(AH + V(0x20))); WriteLevels(); return Next.Read; // 30: add volume
            case 0x0C15: return SilenceVoice();                                          // 33: silence voice AH
            case 0x0C81: return RandomFrequency();                                       // 35: random frequency
            case 0x0CB4: _si--; SetW(_di + 0x29, 0x1407); return Next.Read;            // 36: routine off
            case 0x0CBD: return Bend();                                                  // 39: bend
            case 0x0D77: _si--; SetV(0x25, B(SpeedShared)); return Next.Read;          // 3A: speed = shared
            case 0x0D81: _si--; return Next.Read;                                        // 3B: no operation (one byte)
            case 0x0D85: SetV(0x36, AH); return Next.Read;                              // 3C: random length
            case 0x0D8B: return ChangeSpeed();                                           // 3D: change speed
            case 0x0DA8: return Next.Read;                                               // 3F, 40: no operation
            case 0x0DAB: return RhythmInstruments();                                     // 41: rhythm instruments
            case 0x0E8A: return RhythmHit();                                             // 42: rhythm hit
            case 0x0EBA: return RhythmOff();                                             // 43: rhythm off
            case 0x0ECD: return RhythmLevels(RhythmLevelMode.Offset);                    // 44
            case 0x0F71: return RhythmLevels(RhythmLevelMode.Accumulate);                // 45
            case 0x1015: return RhythmLevels(RhythmLevelMode.Base);                      // 46
            case 0x10A5: SetV(0x40, AH); return Next.Read;                              // 48: follow shared speed
            case 0x10AB: SetV(0x32, AH); Lodsb(); SetV(0x33, AL); return Next.Read;    // 49: velocity bends
            default:
                // [APPROX] Command numbers 4Ah…70h index past the jump table into data (handler addresses 15B6h and up); the
                // original would execute data as code. No observed effect uses them.
                throw new NotSupportedException($"Effect machine: command {AL:X2} has no handler (address {handler:X4}).");
        }
    }

    // [ORIGINAL] 0000:09E5
    /// <summary>0000:09E5 – end of sequence: priority 0, pointer 0, key off (not voice 9); the voice counter stays.</summary>
    private Next EndSequence()
    {
        SetV(0x08, 0);
        SetW(_di + 0x03, 0);
        if (B(CurrentVoice) != 9) KeyOff();
        return Next.NextVoice;
    }

    // [ORIGINAL] 0000:094B
    private Next LoopBack()
    {
        byte count = (byte)(V(0x06) - 1);
        SetV(0x06, count);
        if (count != 0) return Jump();
        _si++;
        return Next.Read;
    }

    // [ORIGINAL] 0000:09A8
    /// <summary>0000:09A8 – SI = parameter byte (low) and the next byte (high).</summary>
    private Next Jump()
    {
        CL = AH;
        Lodsb();
        (AL, AH) = (AH, AL);
        AL = CL;
        _si = _ax;
        return Next.Read;
    }

    // [ORIGINAL] 0000:0953
    /// <summary>0000:0953 – starts effect AH (below FFh) like 0770 (priority test, clear, start, mute), without the 03F3 rule.</summary>
    private Next StartNested()
    {
        ushort si = _si, di = _di, bx = _bx;
        BL = AH;
        if (BL < 0xFF)
        {
            BH = 0;
            _bx <<= 1;
            _bx += W(StartTable);
            _si = W(_bx);
            _bx = W(_si);
            AL = BH;
            CL = BL;
            BH = 0;
            _bx <<= 1;
            _di = W(_bx + VoiceBlocks);
            if ((sbyte)AL >= (sbyte)V(0x08))
            {
                ClearVoice();
                SetV(0x08, AL);
                _si += 2;
                SetW(_di + 0x03, _si);
                SetV(0x25, 0xFF);
                SetV(0x26, 0xFF);
                SetV(0x05, 0x01);
                SetB(Active, 1);
                MuteChannel();
            }
        }
        _bx = bx;
        _di = di;
        _si = si;
        return Next.Read;
    }

    // [ORIGINAL] 0000:09B4
    /// <summary>0000:09B4 – calls the sequence at the next word; the return position goes onto the voice's stack [DI+0Ah+2·[9]].</summary>
    private Next CallSequence()
    {
        _si--;
        BL = V(0x09);
        BH = 0;
        BL = (byte)(BL << 1);
        _ax = (ushort)(_si + 2);
        SetW(_bx + _di + 0x0A, _ax);
        SetV(0x09, (byte)(V(0x09) + 1));
        Lodsw();
        _si = _ax;
        return Next.Read;
    }

    // [ORIGINAL] 0000:09CC
    private Next ReturnSequence()
    {
        _si--;
        SetV(0x09, (byte)(V(0x09) - 1));
        BL = V(0x09);
        BH = 0;
        BL = (byte)(BL << 1);
        _ax = W(_bx + _di + 0x0A);
        _si = _ax;
        return Next.Read;
    }

    // [ORIGINAL] 0000:09FB, 0000:0B3B
    /// <summary>0000:09FB (with key off) / 0B3B (without) – rest of duration AH.</summary>
    private Next Rest(bool keyOff)
    {
        SetDuration();
        SetW(_di + 0x03, _si);
        if (keyOff && B(CurrentVoice) != 9) KeyOff();
        AL = B(NoteDuration);
        return AL != 0 ? Next.Routines : Next.Read;
    }

    // [ORIGINAL] 0000:0B4E
    private Next Retrigger()
    {
        SetDuration();
        KeyOn();
        SetW(_di + 0x03, _si);
        AL = B(NoteDuration);
        return AL != 0 ? Next.Routines : Next.Read;
    }

    // [ORIGINAL] 0000:0A1F
    /// <summary>0000:0A1F – note AH without key on, duration from the next byte.</summary>
    private Next Legato()
    {
        (AL, AH) = (AH, AL);
        SetNote();
        Lodsb();
        (AL, AH) = (AH, AL);
        SetDuration();
        SetW(_di + 0x03, _si);
        AL = B(NoteDuration);
        return AL != 0 ? Next.Routines : Next.Read;
    }

    // [ORIGINAL] 0000:0A40
    private Next StartTableStep()
    {
        SetV(0x3A, AH);
        SetV(0x39, AH);
        Lodsb();
        SetV(0x3B, AL);
        SetV(0x3C, AL);
        Lodsb();
        SetV(0x3D, AL);
        Lodsw();
        SetW(_di + 0x3E, _ax);
        SetW(_di + 0x2B, 0x1461);
        return Next.Read;
    }

    // [ORIGINAL] 0000:0A5D
    private Next HaltVoice()
    {
        AL = B(CurrentVoice);
        ushort ax = _ax, di = _di;
        BL = AH;
        BH = 0;
        BL = (byte)(BL << 1);
        _di = W(_bx + VoiceBlocks);
        SetV(0x05, 0);
        SetV(0x08, 0);
        SetW(_di + 0x03, 0);
        _di = di;
        _ax = ax;
        SetB(CurrentVoice, AL);
        return Next.Read;
    }

    // [ORIGINAL] 0000:0A81
    private Next Instrument()
    {
        ushort si = _si;
        CL = B(ChannelOffset);
        _bx = 0;
        BL = AH;
        _bx <<= 1;
        _si = W(_bx + 0x1B0D);
        LoadInstrument();
        _si = si;
        return Next.Read;
    }

    // [ORIGINAL] 0000:0A97
    /// <summary>0000:0A97 – slide speed AH, step = next two bytes (high, low), routine 148A.</summary>
    private Next StartSlide()
    {
        SetV(0x13, AH);
        Lodsb();
        (AL, AH) = (AH, AL);
        Lodsb();
        SetW(_di + 0x15, _ax);
        SetW(_di + 0x29, 0x148A);
        SetV(0x14, 0xFF);
        return Next.Read;
    }

    // [ORIGINAL] 0000:0AC1
    private Next StartVibrato()
    {
        SetV(0x1D, AH);
        Lodsb();
        SetV(0x19, AL);
        Lodsb();
        SetV(0x1A, AL);
        SetV(0x1A, (byte)(V(0x1A) + 1));
        AL = (byte)(AL << 1);
        SetV(0x1B, AL);
        Lodsb();
        SetV(0x1C, AL);
        SetW(_di + 0x29, 0x1408);
        return Next.Read;
    }

    // [ORIGINAL] 0000:0AE6
    private Next SetSharedCounter()
    {
        AH = (byte)(AH >> 1);
        SetB(0x040C, AH);
        SetB(0x040D, AH);
        SetB(0x040E, 0xFF);
        AH = 0;
        SetB(0x040F, AH);
        SetB(0x0410, AH);
        return Next.Read;
    }

    // [ORIGINAL] 0000:0B02
    /// <summary>0000:0B02 – waits (repeats the command next tick) until bit mask AH of counter CS:040F changes from 0 to set.</summary>
    private Next WaitShared()
    {
        AL = B(0x0410);
        if (AL == 0)
        {
            AH &= B(0x040F);
            if (AH == 0) SetB(0x0410, (byte)(B(0x0410) + 1));
        }
        else
        {
            AH &= B(0x040F);
            if (AH != 0)
            {
                SetB(0x0410, 0);
                return Next.Read;
            }
        }
        // 0B23
        _si -= 2;
        SetV(0x05, 1);
        SetW(_di + 0x03, _si);
        return Next.NextVoice;
    }

    // [ORIGINAL] 0000:0B89, 0000:0BAA
    /// <summary>0000:0B89 (set) / 0BAA (add) – volume byte [0] of voice AH from the next byte, levels rewritten for that voice.</summary>
    private Next SetOtherVolume(bool add)
    {
        AL = B(CurrentVoice);
        ushort ax = _ax, di = _di;
        if (!add)
        {
            Lodsb();
            SetB(CurrentVoice, AH);
            BL = AH;
            BH = 0;
            BL = (byte)(BL << 1);
            _di = W(_bx + VoiceBlocks);
        }
        else
        {
            SetB(CurrentVoice, AH);
            _bx = 0;
            BL = AH;
            Lodsb();
            BL = (byte)(BL << 1);
            _di = W(_bx + VoiceBlocks);
            AL = (byte)(AL + V(0x00));
        }
        SetV(0x00, AL);
        WriteLevels();
        _di = di;
        _ax = ax;
        SetB(CurrentVoice, AL);
        return Next.Read;
    }

    // [ORIGINAL] 0000:0BCD, 0000:0BEB
    private Next RhythmBit(int shift, byte keep)
    {
        AH &= 1;
        CL = (byte)shift;
        AH = (byte)(AH << shift);
        BL = (byte)((B(RhythmRegister) & keep) | AH);
        SetB(RhythmRegister, BL);
        AH = 0xBD;
        AL = BL;
        Opl();
        return Next.Read;
    }

    // [ORIGINAL] 0000:0C15
    private Next SilenceVoice()
    {
        AL = B(CurrentVoice);
        ushort ax = _ax, di = _di;
        if (AH == 0) SetB(Voice0Started, 0);
        SetB(CurrentVoice, AH);
        BL = AH;
        BH = 0;
        BL = (byte)(BL << 1);
        _di = W(_bx + VoiceBlocks);
        SetV(0x05, 0);
        SetV(0x08, 0);
        SetW(_di + 0x03, 0);
        SetV(0x00, 0);
        if (B(CurrentVoice) != 9)
        {
            BL = AH;
            BH = 0;
            CL = B(_bx + ChannelOffsets);
            AH = (byte)(0xC0 + B(CurrentVoice)); AL = 0x00; Opl();
            AH = (byte)(0x43 + CL); AL = 0x3F; Opl();
            AH = (byte)(0x83 + CL); AL = 0xFF; Opl();
            AH = (byte)(0xB0 + B(CurrentVoice)); AL = 0x00; Opl();
        }
        _di = di;
        _ax = ax;
        SetB(CurrentVoice, AL);
        return Next.Read;
    }

    // [ORIGINAL] 0000:0C81
    /// <summary>0000:0C81 – frequency [27h]/[28h] plus (random AND mask (AH:next byte)), written but not stored.</summary>
    private Next RandomFrequency()
    {
        Lodsb();
        _bx = _ax;
        Random();
        _ax &= _bx;
        CL = V(0x28);
        BH = CL;
        CL &= 0x20;
        BL = V(0x27);
        BH &= 0x1F;
        _bx += _ax;
        BH |= CL;
        AH = (byte)(0xA0 + B(CurrentVoice));
        AL = BL;
        Opl();
        AH = (byte)(0xB0 + B(CurrentVoice));
        AL = BH;
        Opl();
        return Next.Read;
    }

    // [ORIGINAL] 0000:0CBD
    /// <summary>0000:0CBD – bend AH for the current note [41h] (no zero test, unlike 10B5).</summary>
    private Next Bend()
    {
        SetV(0x42, AH);
        AL = V(0x41);
        ushort saved = _ax;
        NoteFrequency(0x0CBD);
        _ax = saved;
        return Next.Read;
    }

    // [ORIGINAL] 0000:0D8B
    /// <summary>0000:0D8B – speed [25h] += signed AH, saturating at FFh (up) and 1 (down).</summary>
    private Next ChangeSpeed()
    {
        byte old = V(0x25);
        if ((sbyte)AH >= 0)
        {
            int sum = AH + old;
            AH = sum > 0xFF ? (byte)0xFF : (byte)sum;
        }
        else
        {
            AH = (byte)(AH + old);
            if (AH >= old) AH = 1;
        }
        SetV(0x25, AH);
        return Next.Read;
    }

    // [ORIGINAL] 0000:0DAB
    /// <summary>
    /// 0000:0DAB – rhythm mode: instruments AH, next, next for channels 6, 7, 8 (operator levels into CS:03FD–0401), then three
    /// frequency pairs for B6h/A6h…B8h/A8h (block bytes masked with 2Fh into the blocks of voices 6–8), rhythm flag CS:03F1 = 20h.
    /// The instrument loader works on the current voice block DI, as in the original.
    /// </summary>
    private Next RhythmInstruments()
    {
        AL = B(CurrentVoice);
        ushort savedVoice = _ax;
        AL = B(ChannelOffset);
        ushort savedChannel = _ax;
        ushort si = _si;

        SetB(CurrentVoice, 6);
        _bx = ChannelOffsets;
        BL = (byte)(BL + 6);
        AL = B(_bx);
        SetB(ChannelOffset, AL);
        _dx = 0x0388;
        CL = B(ChannelOffset);
        _bx = 0;
        BL = AH;
        _bx <<= 1;
        _si = W(_bx + 0x1B0D);
        AL = B(_si + 6);
        SetB(0x03FD, AL);
        LoadInstrument();
        _si = si;

        Lodsb();
        si = _si;
        RhythmInstrument(7, 0x03FF, 0x03FE);
        _si = si;
        Lodsb();
        si = _si;
        RhythmInstrument(8, 0x0400, 0x0401);
        _si = si;

        Lodsb(); AL &= 0x2F; SetB(0x05E5, AL); AH = 0xB6; Opl();
        Lodsb(); AH = 0xA6; Opl();
        Lodsb(); AL &= 0x2F; SetB(0x0628, AL); AH = 0xB7; Opl();
        Lodsb(); AH = 0xA7; Opl();
        Lodsb(); AL &= 0x2F; SetB(0x066B, AL); AH = 0xB8; Opl();
        Lodsb(); AH = 0xA8; Opl();
        SetB(Rhythm, 0x20);
        _ax = savedChannel;
        SetB(ChannelOffset, AL);
        _ax = savedVoice;
        SetB(CurrentVoice, AL);
        return Next.Read;
    }

    /// <summary>0DE1–0E12 / 0E15–0E46: instrument AL for channel of voice <paramref name="voice"/>, levels [5] and [6] saved.</summary>
    private void RhythmInstrument(byte voice, int level5, int level6)
    {
        SetB(CurrentVoice, voice);
        _bx = ChannelOffsets;
        BL = (byte)(BL + voice);
        AH = B(_bx);
        SetB(ChannelOffset, AH);
        _dx = 0x0388;
        CL = B(ChannelOffset);
        _bx = 0;
        BL = AL;
        _bx <<= 1;
        _si = W(_bx + 0x1B0D);
        AL = B(_si + 5);
        SetB(level5, AL);
        AL = B(_si + 6);
        SetB(level6, AL);
        LoadInstrument();
    }

    // [ORIGINAL] 0000:0E8A
    /// <summary>
    /// 0000:0E8A – rhythm hit AH: BDh = ((¬(AH &amp; 1Fh)) &amp; [03F1]) | 20h, then a second data write without a new register
    /// select: [03F1] |= AH, BDh = [03F1] | [03FB] | 20h.
    /// </summary>
    private Next RhythmHit()
    {
        _dx = 0x0388;
        AL = 0xBD;
        _dx++;
        (AL, AH) = (AH, AL);
        ushort saved = _ax;
        AL &= 0x1F;
        AL ^= 0xFF;
        AL &= B(Rhythm);
        AL |= 0x20;
        OplData(0xBD, AL);
        _ax = saved;
        AL |= B(Rhythm);
        SetB(Rhythm, AL);
        AL |= B(RhythmRegister);
        AL |= 0x20;
        OplData(0xBD, AL);
        return Next.Read;
    }

    // [ORIGINAL] 0000:0EBA
    private Next RhythmOff()
    {
        _si--;
        AL = B(RhythmRegister);
        SetB(Rhythm, 0);
        AL &= 0xC0;
        AH = 0xBD;
        Opl();
        return Next.Read;
    }

    private enum RhythmLevelMode
    {
        /// <summary>0ECD: offset byte per instrument stored (0407…040B), level = base + accumulated + offset.</summary>
        Offset,

        /// <summary>0F71: accumulated value (0402…0406) = clamped base + accumulated + offset.</summary>
        Accumulate,

        /// <summary>1015: accumulated value set to the byte, level = base + byte + offset.</summary>
        Base,
    }

    // [ORIGINAL] 0000:0ECD, 0000:0F71, 0000:1015
    /// <summary>Rhythm instrument levels: AH selects the instruments (bits 0…4), the next byte is the value.</summary>
    private Next RhythmLevels(RhythmLevelMode mode)
    {
        Lodsb();
        _bx = _ax;
        // bit, base level, accumulated, offset, register
        RhythmLevel(mode, 0x01, 0x03FF, 0x0404, 0x0409, 0x51);
        _ax = _bx;
        RhythmLevel(mode, 0x02, 0x0401, 0x0406, 0x040B, 0x55);
        _ax = _bx;
        RhythmLevel(mode, 0x04, 0x0400, 0x0405, 0x040A, 0x52);
        _ax = _bx;
        RhythmLevel(mode, 0x08, 0x03FE, 0x0403, 0x0408, 0x54);
        _ax = _bx;
        RhythmLevel(mode, 0x10, 0x03FD, 0x0402, 0x0407, 0x53);
        return Next.Read;
    }

    private void RhythmLevel(RhythmLevelMode mode, byte bit, int baseLevel, int accumulated, int offset, byte register)
    {
        if ((AH & bit) == 0) return;
        switch (mode)
        {
            case RhythmLevelMode.Offset:
                SetB(offset, AL);
                AH = 0;
                AL = (byte)(AL + B(baseLevel) + B(accumulated) + B(offset));
                ClampLevel();
                break;
            case RhythmLevelMode.Accumulate:
                AH = 0;
                AL = (byte)(AL + B(baseLevel) + B(accumulated) + B(offset));
                ClampLevel();
                SetB(accumulated, AL);
                break;
            default:
                SetB(accumulated, AL);
                AH = 0;
                AL = (byte)(AL + B(baseLevel) + B(offset));
                ClampLevel();
                break;
        }
        AH = register;
        Opl();
    }
}
