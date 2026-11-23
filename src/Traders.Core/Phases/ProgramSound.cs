using Traders.Core.Sound;
using Traders.Core.State;

namespace Traders.Core.Phases;

/// <summary>
/// What the sound control needs from the host: the music driver's answer (<see cref="IMusicDriverState.PlaybackState"/>: BL of
/// the last int 64h function 12h it executed), the int 64h vector and the heap block for the driver's buffers (218B:0002), and
/// the execution of the sound commands issued so far. The core has the host execute the pending commands, function 12h of
/// 218B:013A last, before it reads the answer, so the answer follows every command before it.
/// </summary>
public interface ISoundHost : IMusicDriverState
{
    /// <summary>The int 64h vector (getvect 0000:3B91; 0 = no music driver).</summary>
    uint DriverVector { get; }

    /// <summary>Heap block of <paramref name="size"/> bytes (186C:02C6; far pointer segment:offset, 0 = none).</summary>
    uint Allocate(int size);

    /// <summary>Executes <paramref name="commands"/> in order (effect machine, music driver, timer callback).</summary>
    void Execute(IReadOnlyList<SoundCommand> commands);
}

/// <summary>
/// The sound control of the program run (<see cref="SoundControl"/>, lane effekte) bound to the data segment and the host:
/// the sound calls of main (initialisation 218B:0002, program start 218B:0094(0, 0), steps 3–7, teardown 218B:0153 and
/// 21A7:0043) and the sound requests 21A7:0054 / 218B:0094 of the menus and the week's steps go through it. It books the
/// DGROUP flags (DS:10A3, 10A5, 10A9) at the place of the original call and passes the commands to the host
/// (<see cref="ISoundHost.Execute"/>): at the latest when the driver's answer is read, otherwise when the host drains them
/// after the frame (<see cref="Flush"/>). No output, no clock.
/// </summary>
public sealed class ProgramSound
{
    private readonly Dgroup _state;
    private readonly ISoundHost _host;
    private int _executed;

    public ProgramSound(Dgroup state, ISoundHost host)
    {
        _state = state;
        _host = host;
        Control = new SoundControl(new Answer(this));
    }

    /// <summary>The sound control (its commands, timer callback).</summary>
    public SoundControl Control { get; }

    /// <summary>All commands issued so far, oldest first (diagnostics and tests).</summary>
    public IReadOnlyList<SoundCommand> Commands => Control.Commands;

    /// <summary>Number of commands passed to the host so far.</summary>
    public int Executed => _executed;

    /// <summary>[ORIGINAL] 218B:0002 – starts the music driver (part of the initialisation 1777:04E4).</summary>
    public ushort StartDriver() => Control.Start(_state, _host.DriverVector, _host.Allocate);

    /// <summary>[ORIGINAL] 218B:0094(i, flags) – plays piece <paramref name="piece"/>.</summary>
    public void Music(int piece, int flags) => Control.PlayMusic(_state, (ushort)piece, (ushort)flags);

    /// <summary>[ORIGINAL] 218B:013A – playback state of the driver (AL).</summary>
    public byte MusicState() => Control.MusicState(_state);

    /// <summary>[ORIGINAL] 21A7:000D – selects the effect callback; AX = 1.</summary>
    public ushort SelectEffects() => Control.SelectEffects(_state);

    /// <summary>[ORIGINAL] 21A7:0054(a, b) – sound effect <paramref name="effect"/>.</summary>
    public void Effect(int unused, int effect) => Control.PlayEffect(_state, (ushort)unused, (ushort)effect);

    /// <summary>
    /// Sound part of the teardown 1777:018C: 218B:0153 (music off), then 21A7:0043 (effect voices off).
    /// </summary>
    // [ORIGINAL] 1777:018C – calls of 218B:0153 and 21A7:0043 (the system layer 0D73:04A9 and the video mode 0D73:07B6(3)
    // after them are the host's)
    public void Teardown()
    {
        Control.StopMusic(_state);
        Control.StopEffects(_state);
    }

    /// <summary>Passes the commands issued since the last call to the host.</summary>
    public void Flush()
    {
        var all = Control.Commands;
        if (_executed == all.Count) return;
        var pending = all.GetRange(_executed, all.Count - _executed);
        _executed = all.Count;
        _host.Execute(pending);
    }

    /// <summary>
    /// The driver's answer as the sound control reads it: the pending commands up to function 12h, which 218B:013A has just
    /// issued, are executed first; the host answers with BL of that call.
    /// </summary>
    private sealed class Answer(ProgramSound owner) : IMusicDriverState
    {
        public byte PlaybackState
        {
            get
            {
                owner.Flush();
                return owner._host.PlaybackState;
            }
        }
    }
}
