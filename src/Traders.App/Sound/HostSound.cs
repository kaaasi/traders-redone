using Traders.Assets;
using Traders.Audio.Effects;
using Traders.Audio.Music;
using Traders.Audio.Opl;
using Traders.Core.Phases;
using Traders.Core.Sound;

namespace Traders.App.Sound;

/// <summary>
/// The host's end of the sound control (<see cref="ISoundHost"/>): executes the core's sound commands on the effect machine
/// (lane effekte, image from T.EXE) and on the music driver (lane musik, FXDRIVER.EXE), answers the driver's playback state
/// and drives both with the timer ticks of the host (<see cref="Tick"/>): the effect machine in every tick while the timer
/// callback is its tick 0000:0732, the music in the ticks the timer handler passes on to the previous handler (every fourth,
/// DS:1180). The OPL writes of both, stamped with the tick, go to <see cref="Written"/> (the host's sound output). Without
/// FXDRIVER.EXE there is no music driver (int 64h vector 0), as in the original started without TRADERS.BAT.
/// </summary>
// [APPROX] Host parts: the heap block of 218B:0002 is a fixed host address (the driver keeps its own buffers); the int 64h
// vector is a fixed non-zero value when the driver file is there. The PC speaker (2850) is not played (user 11.11.2026).
public sealed class HostSound : ISoundHost
{
    /// <summary>Far address of the block for the driver's buffers (host heap, behind the mask atlas).</summary>
    public const uint BufferBlock = 0xB000_0000;

    /// <summary>The int 64h vector the host reports when the driver is there (image address of the driver's entry).</summary>
    public const uint ResidentVector = 0x0000_195D;

    private readonly Func<ushort, ushort, string?> _fileName;
    private long _tick;

    /// <param name="exe">T.EXE (the effect machine's image).</param>
    /// <param name="fxDriver">FXDRIVER.EXE, or null without the music driver.</param>
    /// <param name="openFile">Reads a music file of the original directory by name (null if missing).</param>
    /// <param name="fileName">Name behind a far pointer of the game (segment, offset) for function 00h.</param>
    public HostSound(ReadOnlySpan<byte> exe, byte[]? fxDriver, Func<string, byte[]?> openFile, Func<ushort, ushort, string?> fileName)
    {
        _fileName = fileName;
        Effects = new EffectMachine(EffectImage.FromExe(exe));
        Effects.WrittenAt += w => Written?.Invoke(w);
        if (fxDriver is not null)
        {
            Music = new FxMusicDriver(FxDriverImage.Unpack(fxDriver), openFile);
            Music.WrittenAt += w => Written?.Invoke(w);
            Music.Install();
        }
    }

    /// <summary>The host for the original files in <paramref name="dosDirectory"/>; <paramref name="fileName"/> as above.</summary>
    public static HostSound ForDirectory(string dosDirectory, Func<ushort, ushort, string?> fileName)
    {
        byte[]? Read(string name)
        {
            string path = Path.Combine(dosDirectory, name);
            return File.Exists(path) ? File.ReadAllBytes(path) : null;
        }
        return new HostSound(File.ReadAllBytes(Path.Combine(dosDirectory, "T.EXE")), Read("FXDRIVER.EXE"), Read, fileName);
    }

    /// <summary>The effect machine (0000:06FE–15B1).</summary>
    public EffectMachine Effects { get; }

    /// <summary>The music driver, or null without FXDRIVER.EXE.</summary>
    public FxMusicDriver? Music { get; }

    /// <summary>OPL writes of the effect machine and the music driver, stamped with the timer tick.</summary>
    public event Action<OplWrite>? Written;

    /// <summary>True while the timer callback is the effect machine's tick 0000:0732 (27C6:041B).</summary>
    public bool EffectsTicking { get; private set; }

    /// <summary>Commands executed so far, oldest first.</summary>
    public List<SoundCommand> Executed { get; } = [];

    public uint DriverVector => Music is null ? 0 : ResidentVector;

    public uint Allocate(int size) => BufferBlock;

    public byte PlaybackState { get; private set; }

    public void Execute(IReadOnlyList<SoundCommand> commands)
    {
        foreach (var c in commands)
        {
            Executed.Add(c);
            switch (c.Kind)
            {
                case SoundCommandKind.Driver when Music is not null:
                {
                    Music.CurrentTick = _tick;
                    string? name = c.Function == SoundCommand.Load ? _fileName(c.Bx, c.Cx) : null;
                    var r = Music.Call(c.Function, c.Bx, c.Cx, c.Dx, name);
                    if (c.Function == SoundCommand.State) PlaybackState = (byte)r.Bx;
                    break;
                }
                case SoundCommandKind.EffectsReset:
                    Effects.CurrentTick = _tick;
                    Effects.Reset();
                    break;
                case SoundCommandKind.EffectsStop:
                    Effects.CurrentTick = _tick;
                    Effects.Stop();
                    break;
                case SoundCommandKind.Effect: Effects.Enqueue(c.Bx); break;
                case SoundCommandKind.TickCallback:
                    EffectsTicking = c.Bx == SoundControl.EffectsSegment && c.Cx == SoundControl.EffectsTick;
                    break;
            }
        }
    }

    /// <summary>
    /// One timer tick <paramref name="tick"/> (27C6:03E4): the effect callback first, then – when the handler passes the tick on
    /// (<paramref name="chained"/>, DS:1180 ≡ 0 mod 4 after its increment) – the music driver's timer.
    /// </summary>
    public void Tick(long tick, bool chained)
    {
        _tick = tick;
        if (EffectsTicking) Effects.Tick(tick);
        if (chained && Music is { TimerActive: true } music) music.Tick(tick);
    }
}
