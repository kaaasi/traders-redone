using Traders.Audio.Opl;

namespace Traders.Audio.Effects;

/// <summary>
/// One OPL2 register write of the effect machine with the tick it belongs to: the timer tick the caller gave the last
/// <see cref="EffectMachine.Tick(long)"/> (or set as <see cref="EffectMachine.CurrentTick"/>); with <see cref="EffectMachine.Tick()"/>
/// the number of calls of 0000:0732.
/// </summary>
public readonly record struct EffectWrite(long Tick, byte Register, byte Value)
{
    public override string ToString() => $"{Tick} {Register:X2} {Value:X2}";
}

/// <summary>
/// Port of the sound effect machine in code segment 0000 of T.EXE (finding docs/routines/0000_0750_effekte.md): entry points
/// 0000:06FE (reset with start table), 0000:0724 (stop), 0000:0750 (queue an effect) and the timer tick 0000:0732. It works on a
/// copy of the 64 KiB code segment window (<see cref="EffectImage"/>) like the original (DS = CS) and is transcribed instruction
/// by instruction on the 8086 registers, so that overlapping data (the ring CS:041B overlaps voice block 0 at CS:042B, #76 R3H-2),
/// register values left over between subroutines and the original's stack slip in 0000:148A are reproduced. Output is the
/// sequence of OPL2 register writes (<see cref="Writes"/>, <see cref="WrittenAt"/>) with the tick they belong to: the caller gives
/// the timer tick of each call of 0000:0732 (<see cref="Tick(long)"/>, the same unit as the music driver's, R9E-2); writes
/// outside a tick (<see cref="Reset"/>, <see cref="Stop"/>) carry <see cref="CurrentTick"/>, the tick the caller set last. No host
/// clock, no randomness besides the machine's own generator CS:03F8.
/// </summary>
public sealed partial class EffectMachine
{
    // Variables of the effect machine (offsets in its code segment).
    public const int NoteDuration = 0x03EC, CurrentVoice = 0x03EF, Active = 0x03F0, Rhythm = 0x03F1, SpeedShared = 0x03F2,
        Voice0Started = 0x03F3, RandomState = 0x03F8, RhythmRegister = 0x03FB, ChannelOffset = 0x03FC, StartTable = 0x0415,
        RingHead = 0x0417, RingTail = 0x0419, Ring = 0x041B, VoiceBlocks = 0x06C9, ChannelOffsets = 0x06DD,
        StartTablePointer = 0x15B2, JumpTable = 0x151E;

    /// <summary>Number of voices (0…9; voice 9 is the silent control voice).</summary>
    public const int Voices = 10;

    private readonly byte[] _m;
    private readonly List<EffectWrite> _writes = [];

    // 8086 registers of the transcription.
    private ushort _ax, _bx, _cx, _dx, _si, _di;

    /// <summary>Creates the machine on a copy of <paramref name="image"/> (the code segment window, <see cref="EffectImage"/>).</summary>
    public EffectMachine(ReadOnlySpan<byte> image)
    {
        if (image.Length != EffectImage.Size) throw new ArgumentException("The image must be one 64 KiB segment.", nameof(image));
        _m = image.ToArray();
    }

    /// <summary>The code segment window with the machine's current variables.</summary>
    public byte[] Memory => _m;

    /// <summary>All OPL2 register writes so far.</summary>
    public IReadOnlyList<EffectWrite> Writes => _writes;

    /// <summary>Number of ticks so far (calls of <see cref="Tick()"/> and <see cref="Tick(long)"/>).</summary>
    public long TickCount { get; private set; }

    /// <summary>
    /// Tick stamped on the writes: the value given to the last <see cref="Tick(long)"/>, or set by the caller before
    /// <see cref="Reset"/> or <see cref="Stop"/> (writes outside the timer interrupt carry the caller's current tick; 0 before the
    /// first).
    /// </summary>
    public long CurrentTick { get; set; }

    /// <summary>Every write as it is made, with its tick (common interface of the sound lanes, <see cref="OplWrite"/>).</summary>
    public event Action<OplWrite>? WrittenAt;

    public void ClearWrites() => _writes.Clear();

    // ---- registers --------------------------------------------------------------------------------------------------------

    private byte AL { get => (byte)_ax; set => _ax = (ushort)((_ax & 0xFF00) | value); }
    private byte AH { get => (byte)(_ax >> 8); set => _ax = (ushort)((_ax & 0x00FF) | (value << 8)); }
    private byte BL { get => (byte)_bx; set => _bx = (ushort)((_bx & 0xFF00) | value); }
    private byte BH { get => (byte)(_bx >> 8); set => _bx = (ushort)((_bx & 0x00FF) | (value << 8)); }
    private byte CL { get => (byte)_cx; set => _cx = (ushort)((_cx & 0xFF00) | value); }
    private byte CH { get => (byte)(_cx >> 8); set => _cx = (ushort)((_cx & 0x00FF) | (value << 8)); }
    private byte DL { get => (byte)_dx; set => _dx = (ushort)((_dx & 0xFF00) | value); }
    private byte DH { get => (byte)(_dx >> 8); set => _dx = (ushort)((_dx & 0x00FF) | (value << 8)); }

    // ---- memory (DS = CS, offsets wrap inside the segment) ----------------------------------------------------------------

    private byte B(int offset) => _m[offset & 0xFFFF];
    private void SetB(int offset, byte value) => _m[offset & 0xFFFF] = value;
    private ushort W(int offset) => (ushort)(_m[offset & 0xFFFF] | (_m[(offset + 1) & 0xFFFF] << 8));
    private void SetW(int offset, ushort value)
    {
        _m[offset & 0xFFFF] = (byte)value;
        _m[(offset + 1) & 0xFFFF] = (byte)(value >> 8);
    }

    private void Lodsb() { AL = B(_si); _si++; }
    private void Lodsw() { _ax = W(_si); _si += 2; }

    /// <summary>Reads byte <paramref name="field"/> of the current voice block (DI).</summary>
    private byte V(int field) => B(_di + field);
    private void SetV(int field, byte value) => SetB(_di + field, value);

    // ---- entry points ---------------------------------------------------------------------------------------------------

    // [ORIGINAL] 0000:06FE
    /// <summary>0000:06FE – takes the start table pointer CS:15B2 into CS:0415, resets the machine (0812), empties the ring.</summary>
    public void Reset()
    {
        SetW(StartTable, W(StartTablePointer));
        _bx = W(StartTablePointer);
        Silence();
        SetW(RingTail, 0);
        SetW(RingHead, 0);
    }

    // [ORIGINAL] 0000:0724
    /// <summary>0000:0724 – stops all sounds (0812) without touching the ring.</summary>
    public void Stop() => Silence();

    // [ORIGINAL] 0000:0750
    /// <summary>
    /// 0000:0750 – writes the word <paramref name="effect"/> into ring slot [CS:0417] and advances the head modulo 16. There is no
    /// overflow check: the 17th unprocessed request makes the ring look empty (#76 R3H-2).
    /// </summary>
    public void Enqueue(ushort effect)
    {
        _bx = W(RingHead);
        _ax = effect;
        SetW(_bx + Ring, _ax);
        _bx++;
        _bx &= 0x0F;
        SetW(RingHead, _bx);
    }

    /// <summary><see cref="Tick(long)"/> with the next tick number after <see cref="CurrentTick"/> (ticks counted by the machine).</summary>
    public void Tick() => Tick(CurrentTick + 1);

    // [ORIGINAL] 0000:0732
    /// <summary>
    /// 0000:0732 – one timer tick: start queued effects (0770), step all voices (0866), step the shared counter (121C).
    /// <paramref name="timerTick"/> is the number of the timer tick of this call; the timer handler 27C6:03E4 calls the effect
    /// callback first in its tick, before it increments DS:1180 (and before the music's tick in the same interrupt).
    /// </summary>
    public void Tick(long timerTick)
    {
        CurrentTick = timerTick;
        TickCount++;
        // push ax…es / pop: the caller's registers are restored; the machine's own register values do not survive a tick in
        // any way that matters, but they are kept as the transcription leaves them.
        StartQueued();
        StepVoices();
        StepSharedCounter();
    }

    // [ORIGINAL] 0000:11FA
    /// <summary>0000:11FA – writes AL to OPL register AH (ports 388h/389h); DX ends as 389h.</summary>
    private void Opl()
    {
        _dx = 0x0389;
        Add(AH, AL);
    }

    private void Add(byte register, byte value)
    {
        _writes.Add(new EffectWrite(CurrentTick, register, value));
        WrittenAt?.Invoke(new OplWrite(CurrentTick, register, value));
    }

    /// <summary>Plain data port write (out 389h) to the register last selected.</summary>
    private void OplData(byte register, byte value) => Add(register, value);

    // [ORIGINAL] 0000:0812
    /// <summary>0000:0812 – machine idle, random seed 1234h, OPL 01h = 20h, 08h = 0, BDh = 0, all voices silent and cleared.</summary>
    private void Silence()
    {
        SetB(Active, 0);
        SetW(RandomState, 0x1234);
        _ax = 0x0120; Opl();
        _ax = 0x0800; Opl();
        _ax = 0xBD00; Opl();
        SetB(Voice0Started, 0);
        _cx = 0x000A;
        BH = 0;
        do
        {
            BL = CL;
            BL--;
            if (BL != 9)
            {
                AH = (byte)(B(_bx + ChannelOffsets) + 0x40);
                AL = 0x3F;
                Opl();
                AH = (byte)(B(_bx + ChannelOffsets) + 0x43);
                AL = 0x3F;
                Opl();
            }
            BL = (byte)(BL << 1);
            _di = W(_bx + VoiceBlocks);
            ClearVoice();
            _cx--;
        } while (_cx != 0);
    }

    // [ORIGINAL] 0000:0770
    /// <summary>0000:0770 – starts every queued request whose priority is at least the priority of its voice.</summary>
    private void StartQueued()
    {
        while (true)
        {
            _bx = W(RingTail);
            if (_bx == W(RingHead)) return;
            _bx = W(_bx + Ring);
            BH = 0;
            _bx <<= 1;
            _bx += W(StartTable);
            _si = W(_bx);
            _bx = W(_si);
            if (BL == 0) SetB(Voice0Started, 0);
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
                if (BL != 0x12) MuteChannel();
            }
            SetW(RingTail, (ushort)((W(RingTail) + 1) & 0x0F));
        }
    }

    // [ORIGINAL] 0000:121C
    /// <summary>0000:121C – shared counter for the wait command (0B02): CS:040E += CS:03F2, on carry count down CS:040D.</summary>
    private void StepSharedCounter()
    {
        int sum = B(0x040E) + B(SpeedShared);
        AL = (byte)sum;
        SetB(0x040E, AL);
        if (sum < 0x100) return;
        byte d = (byte)(B(0x040D) - 1);
        SetB(0x040D, d);
        if (d != 0) return;
        AL = B(0x040C);
        SetB(0x040D, AL);
        SetB(0x040F, (byte)(B(0x040F) + 1));
    }
}
