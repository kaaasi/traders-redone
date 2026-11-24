using Traders.App.Sound;
using Traders.Core.Phases;
using Traders.Core.Sound;
using Traders.Core.State;
using Traders.Tests.Phases;

namespace Traders.Tests.App;

/// <summary>
/// The key clicks of the scenes' typewriter text to the host's sound (<see cref="TypewriterClicks"/>, #262) without original
/// data: with sound on (DS:10A7) an effect for AdLib (DS:0910 = 1) or the PC speaker, nothing with sound off, the core's
/// pending commands first, the driver asked and the effect callback selected again while DS:10A5 is set, and no booking in the
/// data segment.
/// </summary>
public class TypewriterClicksTests
{
    private static Dgroup State(ushort soundOn, ushort device)
    {
        var d = new Dgroup();
        d.Set(SoundControl.SoundOn, soundOn);
        d.Set(SoundControl.SoundDevice, device);
        return d;
    }

    [Fact]
    public void Click_with_sound_on_and_AdLib_queues_effect_16h_after_the_pending_core_commands()
    {
        var state = State(1, 1);
        var host = new StubSoundHost();
        var sound = new ProgramSound(state, host);
        sound.Effect(1, 0x0B); // pending core command
        var before = (byte[])state.Bytes.Clone();
        var clicks = new TypewriterClicks(state, sound, host);

        clicks.PlaySound(1, 0x16);

        Assert.Equal([new SoundCommand(SoundCommandKind.Effect, Bx: 0x0B), new SoundCommand(SoundCommandKind.Effect, Bx: 0x16)],
            host.Executed);
        Assert.Equal(1, clicks.Count);
        Assert.Equal(before, state.Bytes);
    }

    [Fact]
    public void Click_with_sound_off_is_silent()
    {
        var state = State(0, 1);
        var host = new StubSoundHost();
        var clicks = new TypewriterClicks(state, new ProgramSound(state, host), host);

        clicks.PlaySound(1, 0x16);

        Assert.Empty(host.Executed);
        Assert.Equal(0, clicks.Count);
    }

    [Fact]
    public void Click_without_AdLib_goes_to_the_PC_speaker()
    {
        var state = State(1, 2);
        var host = new StubSoundHost();
        var clicks = new TypewriterClicks(state, new ProgramSound(state, host), host);

        clicks.PlaySound(1, 0x16);

        Assert.Equal([new SoundCommand(SoundCommandKind.SpeakerEffect, Bx: 0x16)], host.Executed);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void Click_while_the_music_flag_is_set_asks_the_driver_and_selects_the_effects_again_without_booking(byte playing)
    {
        var state = State(1, 1);
        state.Set(SoundControl.DriverReady, (ushort)1);
        state.Set(SoundControl.MusicPlaying, (ushort)1);
        var host = new StubSoundHost { Playing = playing };
        var before = (byte[])state.Bytes.Clone();
        var clicks = new TypewriterClicks(state, new ProgramSound(state, host), host);

        clicks.PlaySound(1, 0x16);

        List<SoundCommand> expected = [SoundCommand.Driver(SoundCommand.State)];
        if (playing != 0) expected.Add(SoundCommand.Driver(SoundCommand.Stop));
        expected.AddRange([
            new SoundCommand(SoundCommandKind.EffectsReset),
            new SoundCommand(SoundCommandKind.TickCallback, Bx: SoundControl.EffectsSegment, Cx: SoundControl.EffectsTick),
            new SoundCommand(SoundCommandKind.Effect, Bx: 0x16),
        ]);
        Assert.Equal(expected, host.Executed);
        Assert.Equal([1], host.ReadsAt); // the answer read after function 12h was executed
        Assert.Equal(before, state.Bytes);
    }
}
