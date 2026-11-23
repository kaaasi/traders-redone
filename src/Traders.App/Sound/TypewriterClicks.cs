using Traders.Core.Phases;
using Traders.Core.Sound;
using Traders.Core.State;
using Traders.Vga.Layers;

namespace Traders.App.Sound;

/// <summary>
/// The typewriter's effects (<see cref="ITypewriterEffects"/>) of the scenes (planting output, stations, 0A69:01AD) to the
/// host's sound: the key click 21A7:0054(1, 16h) gives the host the commands the original issues for it – with DS:10A5 ≠ 0
/// the driver's state (218B:0153: 12h, 03h if it plays) and the effect callback again (21A7:000D), then with sound on
/// (DS:10A7) the effect into the ring 0000:0750 (AdLib, DS:0910 = 1) or to the PC speaker 2850:00CC. They come from the
/// core's own sound control (<see cref="SoundControl.PlayEffect"/>) run on a copy of the data segment, after the core's
/// pending commands, so the host executes them in the original's order. Reads the core, books nothing (decision lane lead
/// 23.11.2026, #262).
/// </summary>
// [APPROX] Where the driver reports music playing at a click (FXDRIVER, the music of the menu still playing because no effect of
// the core came since), the music is stopped as in the original, but DS:10A5 stays 1 in the core (the original clears it):
// the core's next effect asks the driver once more. With the RefEmu's recording driver (always "nothing playing") the
// commands and the data segment are the original's. Moving the click into the core is #262.
// [APPROX] 21FD:0245 (wait for the timer flag DS:3D60) is not waited for: the host's frame clock replaces the timer flag.
public sealed class TypewriterClicks(Dgroup state, ProgramSound sound, ISoundHost host) : ITypewriterEffects
{
    /// <summary>Clicks passed to the host so far (diagnostics).</summary>
    public int Count { get; private set; }

    /// <summary>Raised before the commands of a click go to the host (diagnostics).</summary>
    public event Action? Clicking;

    public void PlaySound(ushort a, ushort b)
    {
        sound.Flush();
        Clicking?.Invoke();
        var answer = new Answer(host);
        var control = new SoundControl(answer);
        answer.Control = control;
        control.PlayEffect(new Dgroup((byte[])state.Bytes.Clone()), a, b);
        answer.Flush();
        if (control.Commands.Any(c => c.Kind is SoundCommandKind.Effect or SoundCommandKind.SpeakerEffect)) Count++;
    }

    public void WaitTimerFlag()
    {
    }

    /// <summary>The driver's answer: the click's commands up to function 12h are executed first (as <see cref="ProgramSound"/>).</summary>
    private sealed class Answer(ISoundHost host) : IMusicDriverState
    {
        private int _done;

        public SoundControl? Control { get; set; }

        public byte PlaybackState
        {
            get
            {
                Flush();
                return host.PlaybackState;
            }
        }

        public void Flush()
        {
            var all = Control!.Commands;
            if (_done == all.Count) return;
            var pending = all.GetRange(_done, all.Count - _done);
            _done = all.Count;
            host.Execute(pending);
        }
    }
}
