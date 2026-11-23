using Traders.Core;
using Traders.Core.Phases;
using Traders.Core.Sound;
using Traders.Core.State;

namespace Traders.Tests.Phases;

/// <summary>
/// The sound calls of main on the program's sound control (<see cref="ProgramSound"/>, lane partie part 2) without original
/// data: order of the commands from the initialisation to the menu and at the teardown, the jumps of main on the driver's
/// answer (1777:0132/0142), and the execution of pending commands before every answer.
/// </summary>
public class ProgramSoundTests
{
    private sealed class Instant(Func<Dgroup, byte>? result = null) : IProgramStepHandler
    {
        public StepOutcome Begin(ProgramStepContext c) => StepOutcome.Finished(result?.Invoke(c.State) ?? 0);

        public StepOutcome Tick(ProgramStepContext c) => throw new InvalidOperationException("instant step ticked");
    }

    /// <summary>State as T.EXE starts it for the sound: sound on (DS:10A7 = 1), no piece loaded (DS:10A9 = FFFFh), detect all.</summary>
    private static Dgroup NewState()
    {
        var d = new Dgroup();
        d.Set(ProgramInitialisation.GraphicsCard, (short)-1);
        d.Set(ProgramInitialisation.SoundDevice, (short)-1);
        d.Set(SoundControl.SoundOn, (ushort)1);
        d.Set(SoundControl.CurrentPiece, (ushort)0xFFFF);
        return d;
    }

    /// <summary>Main with the sound control, a program start without frames and a menu that quits at once (AL = 0).</summary>
    private static (ProgramMachine Machine, ProgramSound Sound, List<ProgramEvent> Events, Dgroup State) Run(StubSoundHost host)
    {
        var state = NewState();
        var sound = new ProgramSound(state, host);
        var steps = ProgramSteps.Defaults(ProgramInitialisation.AdLib, sound);
        steps[2] = new ProgramStartWithMusic(new Instant(), sound);
        steps[ProgramLoop.MainMenu] = new Instant(_ => 0);
        var machine = new ProgramMachine(state, new OriginalRandom(new RecordedClock([]), 1), steps,
            new Dictionary<int, IWeekStepHandler>(), sound: sound);
        var events = new List<ProgramEvent>();
        for (int f = 0; f < 10 && machine.Status != ProgramStatus.Ended; f++)
        {
            machine.Advance(new PhaseTick(f));
            events.AddRange(machine.DrainEvents());
            sound.Flush();
        }
        return (machine, sound, events, state);
    }

    private static SoundCommand D(ushort function, ushort bx = 0, ushort cx = 0, ushort dx = 0) => SoundCommand.Driver(function, bx, cx, dx);

    private static List<int> Started(IEnumerable<ProgramEvent> events) =>
        events.Where(e => e.Kind == ProgramEventKind.StepStarted).Select(e => e.Step).ToList();

    /// <summary>
    /// Nothing playing (the RefEmu's recording stub): 218B:0002 starts the driver (06h, 05h, 24 × 10h, DS:10A3 = 1), the program
    /// start plays the intro (0094(0, 0): load piece 0, no repeat, fade step 7Fh, fade in, play), main calls 013A (AL = 0), 000D
    /// (effects reset, callback 0000:0732), 013A, 0054(0, 1) (music flag set: 013A, 000D again, effect 1) and 0094(1, 2), then the
    /// menu; its AL = 0 takes main to the teardown: 0153 (013A, nothing to stop), 0043 (effect voices off).
    /// Counter-check: <see cref="ProgramSound.Teardown"/> left out or 218B:0002 without buffers → red.
    /// </summary>
    [Fact]
    public void Main_issues_the_commands_of_the_original_order_when_nothing_plays()
    {
        var host = new StubSoundHost();
        var (machine, sound, events, d) = Run(host);
        Assert.Equal(ProgramStatus.Ended, machine.Status);
        Assert.Equal([1, 2, 3, 4, 5, 6, 7, 8], Started(events));

        var expected = new List<SoundCommand> { D(SoundCommand.Detect), D(SoundCommand.Reset) };
        var (seg, off) = ((ushort)(StubSoundHost.Block >> 16), (ushort)(StubSoundHost.Block & 0xFFFF));
        for (ushort i = 0; i < SoundControl.BufferCount; i++)
        {
            expected.Add(D(SoundCommand.Buffer, i, seg, off));
            (seg, off) = SoundControl.HugeAdd(seg, off, SoundControl.BufferSize);
        }
        var state = NewState();
        void Music(int piece, int flags)
        {
            int name = SoundControl.PieceNames + 4 * piece;
            expected.Add(D(SoundCommand.State));
            expected.Add(D(SoundCommand.Load, state.U16(name + 2), state.U16(name)));
            expected.Add(D(SoundCommand.Repeat, (ushort)((flags & 2) >> 1)));
            expected.Add(D(SoundCommand.FadeStep, (ushort)((flags & 1) != 0 ? 3 : 0x7F)));
            expected.Add(D(SoundCommand.FadeIn));
            expected.Add(D(SoundCommand.Play));
        }
        SoundCommand[] select = [new(SoundCommandKind.EffectsReset), new(SoundCommandKind.TickCallback, Bx: 0x0000, Cx: 0x0732)];
        Music(0, 0);
        expected.Add(D(SoundCommand.State));
        expected.AddRange(select);
        expected.Add(D(SoundCommand.State));
        expected.Add(D(SoundCommand.State));
        expected.AddRange(select);
        expected.Add(new SoundCommand(SoundCommandKind.Effect, Bx: 1));
        Music(1, 2);
        expected.Add(D(SoundCommand.State));
        expected.Add(new SoundCommand(SoundCommandKind.EffectsStop));
        Assert.Equal(expected, sound.Commands);
        Assert.Equal(expected, host.Executed);
        Assert.Equal(1, d.U16(SoundControl.DriverReady));
        Assert.Equal(1, d.U16(SoundControl.MusicPlaying)); // 0153 with nothing playing leaves it
        Assert.Equal(1, d.U16(SoundControl.CurrentPiece));
        Assert.True(sound.Control.EffectsTicking);
    }

    /// <summary>
    /// Every answer of the driver is read after the host has executed all commands before it, function 12h of 218B:013A last.
    /// Counter-check: the answer read without <see cref="ProgramSound.Flush"/> → red.
    /// </summary>
    [Fact]
    public void The_host_executes_the_pending_commands_before_each_answer()
    {
        var host = new StubSoundHost();
        Run(host);
        Assert.Equal(6, host.ReadsAt.Count); // 0094(0, 0), steps 3, 5, 6, 7, teardown
        Assert.All(host.ReadsAt, n => Assert.Equal(SoundCommand.State, host.Executed[n - 1].Function));
        Assert.All(host.ReadsAt, n => Assert.Equal(SoundCommandKind.Driver, host.Executed[n - 1].Kind));
    }

    /// <summary>
    /// The intro still playing (AL ≠ 0): 1777:0132 jumps over 21A7:000D to the loop start, 0142 over the effect and the menu
    /// music to the menu (steps 4, 6 and 7 not run); the teardown stops the music (03h, DS:10A5 = 0).
    /// </summary>
    [Fact]
    public void A_playing_intro_takes_main_past_the_effect_and_the_menu_music()
    {
        var host = new StubSoundHost { Playing = 1 };
        var (machine, sound, events, d) = Run(host);
        Assert.Equal([1, 2, 3, 5, 8], Started(events));
        Assert.Equal(1, events.Single(e => e.Kind == ProgramEventKind.StepFinished && e.Step == 3).Value);
        Assert.DoesNotContain(sound.Commands, c => c.Kind == SoundCommandKind.TickCallback);
        Assert.Equal([D(SoundCommand.State), D(SoundCommand.Stop), new SoundCommand(SoundCommandKind.EffectsStop)], sound.Commands.TakeLast(3));
        Assert.Equal(0, d.U16(SoundControl.MusicPlaying));
    }

    /// <summary>Without an int 64h vector 218B:0002 does nothing (DS:10A3 = 0) and the music calls issue no driver command.</summary>
    [Fact]
    public void Without_a_driver_only_the_effects_are_commanded()
    {
        var host = new StubSoundHost { DriverVector = 0 };
        var (machine, sound, events, d) = Run(host);
        Assert.Equal([1, 2, 3, 4, 5, 6, 7, 8], Started(events));
        Assert.DoesNotContain(sound.Commands, c => c.Kind == SoundCommandKind.Driver);
        Assert.Equal(0, d.U16(SoundControl.DriverReady));
        Assert.Equal([new SoundCommand(SoundCommandKind.EffectsReset), new SoundCommand(SoundCommandKind.TickCallback, Bx: 0, Cx: 0x0732),
            new SoundCommand(SoundCommandKind.Effect, Bx: 1), new SoundCommand(SoundCommandKind.EffectsStop)], sound.Commands);
    }
}
