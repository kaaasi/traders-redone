using Traders.Core.State;

namespace Traders.Core.Sound;

/// <summary>
/// Sound control of the game (finding docs/routines/218B_21A7_ton.md): music module 218B (driver FXDRIVER over int 64h), effect
/// module 21A7 and the timer callback 27C6:041B. Works on the data segment like the original and issues
/// <see cref="SoundCommand"/>s in the original order instead of calling the driver or the effect machine; the driver's
/// playback state comes back as explicit input (<see cref="IMusicDriverState"/>). The callback pointer CS:0416/0418 of segment
/// 27C6 is not in DGROUP and is kept here (<see cref="TickCallbackSegment"/>, <see cref="TickCallbackOffset"/>).
/// </summary>
public sealed class SoundControl(IMusicDriverState driver)
{
    // DS offsets (docs/analyse/DATENLAYOUT.md, sections of lanes programm and effekte).
    public const int SoundDevice = 0x0910;   // word, 1 = AdLib
    public const int DriverReady = 0x10A3;   // word, set by 218B:0002
    public const int MusicPlaying = 0x10A5;  // word
    public const int SoundOn = DgroupLayout.Sound; // word DS:10A7, menu switch
    public const int CurrentPiece = 0x10A9;  // word, piece loaded in the driver (FFFFh from the EXE)
    public const int PieceDisk = 0x10A0;     // byte per piece, disk number (only for the dropped file check 186C:0A6C)
    public const int PieceNames = 0x1094;    // far pointer per piece (offset, segment)

    /// <summary>Size of the block for the driver's buffers, and of one buffer (int 64h function 15h answers 0C04h).</summary>
    public const int BufferBlockSize = 0x12060, BufferCount = 0x18;
    public const ushort BufferSize = 0x0C04;

    /// <summary>Image addresses of the effect machine and of the PC speaker module.</summary>
    public const ushort EffectsSegment = 0x0000, EffectsTick = 0x0732, SpeakerSegment = 0x2850, SpeakerTick = 0x0061;

    /// <summary>Initial callback: the retf at 27C6:041A (no callback).</summary>
    // [ORIGINAL] 27C6:0416
    public const ushort NoCallbackSegment = 0x27C6, NoCallbackOffset = 0x041A;

    public List<SoundCommand> Commands { get; } = [];

    /// <summary>Segment (image address) of the timer callback, CS:0418 of segment 27C6.</summary>
    public ushort TickCallbackSegment { get; private set; } = NoCallbackSegment;

    /// <summary>Offset of the timer callback, CS:0416 of segment 27C6.</summary>
    public ushort TickCallbackOffset { get; private set; } = NoCallbackOffset;

    /// <summary>True if the timer callback is the effect machine's tick 0000:0732.</summary>
    public bool EffectsTicking => TickCallbackSegment == EffectsSegment && TickCallbackOffset == EffectsTick;

    /// <summary>
    /// 218B:0002 – starts the music driver: only with AdLib (DS:0910 = 1) and an int 64h vector. Detect (06h) and reset (05h),
    /// a block of 12060h bytes from the heap, its 24 buffers of 0C04h bytes to the driver (10h, huge pointer steps), DS:10A3 = 1.
    /// <paramref name="driverVector"/> is the int 64h vector (getvect, 0000:3B91), <paramref name="allocate"/> the heap
    /// allocation 186C:02C6 (far pointer segment:offset, 0 = none) – both are the host's.
    /// </summary>
    // [ORIGINAL] 218B:0002
    public ushort Start(Dgroup d, uint driverVector, Func<int, uint> allocate)
    {
        if (d.U16(SoundDevice) != 1) return 0;
        if (driverVector == 0) return 0;
        Commands.Add(SoundCommand.Driver(SoundCommand.Detect, 0));
        Commands.Add(SoundCommand.Driver(SoundCommand.Reset, 0));
        uint block = allocate(BufferBlockSize);
        ushort off = (ushort)block, seg = (ushort)(block >> 16);
        if (IsNull(seg, off)) return 0;
        for (ushort i = 0; i < BufferCount; i++)
        {
            Commands.Add(SoundCommand.Driver(SoundCommand.Buffer, i, seg, off));
            (seg, off) = HugeAdd(seg, off, BufferSize);
        }
        d.Set(DriverReady, (ushort)1);
        return 1;
    }

    /// <summary>
    /// 218B:0094(i, flags) – plays piece <paramref name="piece"/>: only with sound on and a driver. Stops the music (0153), loads
    /// the piece if it is not the current one (00h with the name pointer, DS:10A9 = i), repeat (18h) per flag bit 1, fade step
    /// (1Bh) 3 per bit 0 else 7Fh, fade in (1Ah), play (01h), DS:10A5 = 1.
    /// </summary>
    // [ORIGINAL] 218B:0094
    // [APPROX] The file check 186C:0A6C before loading (disk request when the file is missing) is dropped (F2 = A,
    // docs/routines/186C_ressourcenlader.md); the port's files are always there.
    public void PlayMusic(Dgroup d, ushort piece, ushort flags)
    {
        if (d.U16(SoundOn) == 0) return;
        if (d.U16(DriverReady) == 0) return;
        StopMusic(d);
        if (d.U16(CurrentPiece) != piece)
        {
            int name = (ushort)(PieceNames + (ushort)(piece << 2));
            Commands.Add(SoundCommand.Driver(SoundCommand.Load, d.U16((ushort)(name + 2)), d.U16(name)));
            d.Set(CurrentPiece, piece);
        }
        Commands.Add(SoundCommand.Driver(SoundCommand.Repeat, (ushort)((flags & 2) != 0 ? 1 : 0)));
        SetFadeStep((ushort)((flags & 1) != 0 ? 3 : 0x7F));
        Commands.Add(SoundCommand.Driver(SoundCommand.FadeIn));
        Commands.Add(SoundCommand.Driver(SoundCommand.Play));
        d.Set(MusicPlaying, (ushort)1);
    }

    /// <summary>218B:013A – playback state: 0 without a driver (DS:10A3 = 0), else BL of function 12h.</summary>
    // [ORIGINAL] 218B:013A
    public byte MusicState(Dgroup d)
    {
        if (d.U16(DriverReady) == 0) return 0;
        Commands.Add(SoundCommand.Driver(SoundCommand.State));
        return driver.PlaybackState;
    }

    /// <summary>218B:0153 – stops playing music (03h) and clears DS:10A5; nothing if the driver reports nothing playing.</summary>
    // [ORIGINAL] 218B:0153
    public void StopMusic(Dgroup d)
    {
        if (MusicState(d) == 0) return;
        Commands.Add(SoundCommand.Driver(SoundCommand.Stop));
        d.Set(MusicPlaying, (ushort)0);
    }

    /// <summary>218B:01A4(x) – fade step of the driver (1Bh with BX = x).</summary>
    // [ORIGINAL] 218B:01A4
    public void SetFadeStep(ushort step) => Commands.Add(SoundCommand.Driver(SoundCommand.FadeStep, step));

    /// <summary>
    /// 21A7:000D – selects the timer callback: with AdLib reset the effect machine (0000:06FE) and tick 0000:0732, else the PC
    /// speaker tick 2850:0061. Returns 1.
    /// </summary>
    // [ORIGINAL] 21A7:000D
    public ushort SelectEffects(Dgroup d)
    {
        if (d.U16(SoundDevice) == 1)
        {
            Commands.Add(new SoundCommand(SoundCommandKind.EffectsReset));
            SetTickCallback(EffectsSegment, EffectsTick);
        }
        else SetTickCallback(SpeakerSegment, SpeakerTick);
        return 1;
    }

    /// <summary>21A7:0043 – with AdLib stops the effect voices (0000:0724).</summary>
    // [ORIGINAL] 21A7:0043
    public void StopEffects(Dgroup d)
    {
        if (d.U16(SoundDevice) == 1) Commands.Add(new SoundCommand(SoundCommandKind.EffectsStop));
    }

    /// <summary>
    /// 21A7:0054(a, b) – sound effect <paramref name="effect"/>; <paramref name="unused"/> (a) is not read. Playing music is
    /// stopped first and the effect callback selected again (000D); with sound on the effect goes to the ring 0000:0750
    /// (AdLib) or to 2850:00CC (PC speaker).
    /// </summary>
    // [ORIGINAL] 21A7:0054, 21AF:0002
    public void PlayEffect(Dgroup d, ushort unused, ushort effect)
    {
        _ = unused;
        if (d.U16(MusicPlaying) != 0)
        {
            StopMusic(d);
            SelectEffects(d);
        }
        if (d.U16(SoundOn) == 0) return;
        Commands.Add(new SoundCommand(d.U16(SoundDevice) == 1 ? SoundCommandKind.Effect : SoundCommandKind.SpeakerEffect,
            Bx: effect));
    }

    /// <summary>27C6:041B(far p) – sets the timer callback CS:0416/0418 of segment 27C6 (image address).</summary>
    // [ORIGINAL] 27C6:041B
    public void SetTickCallback(ushort segment, ushort offset)
    {
        TickCallbackSegment = segment;
        TickCallbackOffset = offset;
        Commands.Add(new SoundCommand(SoundCommandKind.TickCallback, Bx: segment, Cx: offset));
    }

    /// <summary>Far pointer compare with null of the runtime (0000:4F65): both normalised to segment + offset/16, offset & 0Fh.</summary>
    // [ORIGINAL] 0000:4F65
    public static bool IsNull(ushort segment, ushort offset) => (ushort)(segment + (offset >> 4)) == 0 && (offset & 0xF) == 0;

    /// <summary>Huge pointer addition of the runtime (0000:5079) for a non-negative 16-bit step: normalised result.</summary>
    // [ORIGINAL] 0000:5079
    public static (ushort Segment, ushort Offset) HugeAdd(ushort segment, ushort offset, ushort step)
    {
        int sum = offset + step;
        ushort seg = segment;
        if (sum > 0xFFFF) seg = (ushort)(seg + 0x1000);
        ushort off = (ushort)sum;
        seg = (ushort)(seg + (off >> 4));
        return (seg, (ushort)(off & 0xF));
    }
}
