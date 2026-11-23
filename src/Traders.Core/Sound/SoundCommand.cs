namespace Traders.Core.Sound;

/// <summary>Kinds of sound commands the core gives to the audio layer (finding docs/routines/218B_21A7_ton.md).</summary>
public enum SoundCommandKind
{
    /// <summary>
    /// int 64h of the music driver FXDRIVER: <see cref="SoundCommand.Function"/> = AX, parameters BX, CX, DX as far as the
    /// function reads them (<see cref="SoundCommand.Defined"/>).
    /// </summary>
    Driver,

    /// <summary>0000:06FE – reset the effect machine (start table, ring head and tail).</summary>
    EffectsReset,

    /// <summary>0000:0724 – stop the effect voices.</summary>
    EffectsStop,

    /// <summary>0000:0750(b) – queue effect <see cref="SoundCommand.Bx"/> in the effect ring.</summary>
    Effect,

    /// <summary>2850:00CC(b) – PC speaker effect <see cref="SoundCommand.Bx"/> (DS:0910 ≠ 1; not played by the port).</summary>
    SpeakerEffect,

    /// <summary>27C6:041B – timer callback set to image address <see cref="SoundCommand.Bx"/>:<see cref="SoundCommand.Cx"/>.</summary>
    TickCallback,
}

/// <summary>One sound command in the order the original issues it; unused parameters are 0.</summary>
public readonly record struct SoundCommand(SoundCommandKind Kind, ushort Function = 0, ushort Bx = 0, ushort Cx = 0, ushort Dx = 0)
{
    /// <summary>Driver functions the game uses.</summary>
    public const ushort Load = 0x00, Play = 0x01, Stop = 0x03, Reset = 0x05, Detect = 0x06, Buffer = 0x10, State = 0x12,
        Repeat = 0x18, FadeIn = 0x1A, FadeStep = 0x1B;

    /// <summary>
    /// Registers of a driver call that the game sets on purpose (1 = BX, 2 = CX, 4 = DX). The others hold whatever was left
    /// in them and are not part of the command.
    /// </summary>
    public static int Defined(ushort function) => function switch
    {
        Load => 1 | 2,               // BX:CX = file name (segment:offset)
        Buffer => 1 | 2 | 4,         // BX = buffer number, CX = segment, DX = offset
        Reset or Detect or Repeat or FadeStep => 1,
        _ => 0,                      // 01h, 03h, 12h, 1Ah
    };

    public static SoundCommand Driver(ushort function, ushort bx = 0, ushort cx = 0, ushort dx = 0)
    {
        int m = Defined(function);
        return new(SoundCommandKind.Driver, function, (m & 1) != 0 ? bx : (ushort)0, (m & 2) != 0 ? cx : (ushort)0,
            (m & 4) != 0 ? dx : (ushort)0);
    }

    public override string ToString() => Kind switch
    {
        SoundCommandKind.Driver => $"int 64h {Function:X2}h BX={Bx:X4} CX={Cx:X4} DX={Dx:X4}",
        SoundCommandKind.TickCallback => $"callback {Bx:X4}:{Cx:X4}",
        SoundCommandKind.Effect or SoundCommandKind.SpeakerEffect => $"{Kind} {Bx:X4}",
        _ => Kind.ToString(),
    };
}

/// <summary>Answer of the music driver that the core reads (explicit input, the driver itself is the audio layer's).</summary>
public interface IMusicDriverState
{
    /// <summary>BL of int 64h function 12h: 0 = nothing playing.</summary>
    byte PlaybackState { get; }
}
