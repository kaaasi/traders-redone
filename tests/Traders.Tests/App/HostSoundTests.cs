using Traders.App.Sound;
using Traders.Core.Phases;
using Traders.Core.Sound;
using Traders.Core.State;

namespace Traders.Tests.App;

/// <summary>
/// The host's end of the sound control (<see cref="HostSound"/>) on the original files: the core's commands reach the music
/// driver and the effect machine, the driver's answer follows them, and the timer ticks drive both into OPL writes.
/// The OPL writes themselves are compared with the original in the tests of lanes effekte and musik.
/// </summary>
public class HostSoundTests
{
    private static (HostSound Host, ProgramSound Sound, Dgroup State) Make()
    {
        var state = ProgramSteps.InitialState(File.ReadAllBytes(TestGameData.Dos("T.EXE")));
        state.Set(SoundControl.SoundDevice, (ushort)1);
        var host = HostSound.ForDirectory(TestGameData.DosDir, (seg, off) =>
        {
            if (seg != DgroupLayout.Segment) return null;
            int end = Array.IndexOf(state.Bytes, (byte)0, off);
            return System.Text.Encoding.ASCII.GetString(state.Bytes, off, end - off);
        });
        return (host, new ProgramSound(state, host), state);
    }

    /// <summary>
    /// The intro started by the core plays in the driver: 218B:013A answers "playing" right after 0094(0, 0) and the music
    /// ticks (every fourth timer tick) write OPL registers; 0153 stops it (DS:10A5 = 0, answer 0).
    /// Counter-check: <see cref="HostSound.Execute"/> without the driver calls → red.
    /// </summary>
    [GameDataFact]
    public void The_core_starts_and_stops_the_intro_in_the_driver()
    {
        var (host, sound, state) = Make();
        Assert.NotNull(host.Music);
        Assert.Equal(1, sound.StartDriver());
        sound.Music(0, 0);
        Assert.NotEqual(0, sound.MusicState());
        int writes = 0;
        host.Written += _ => writes++;
        for (long t = 1; t <= 400; t++) host.Tick(t, t % 4 == 0);
        Assert.True(writes > 50, $"{writes} OPL writes of the intro");
        sound.Control.StopMusic(state);
        Assert.Equal(0, state.U16(SoundControl.MusicPlaying));
        Assert.Equal(0, sound.MusicState());
    }

    /// <summary>
    /// An effect plays only while the timer callback is the effect machine's tick (21A7:000D), stamped with the tick it is
    /// written in; with sound off (DS:10A7 = 0) the core sends none.
    /// </summary>
    [GameDataFact]
    public void Effects_tick_once_selected_and_not_with_sound_off()
    {
        var (host, sound, state) = Make();
        var ticks = new List<long>();
        host.Written += w => ticks.Add(w.Tick);
        state.Set(SoundControl.SoundOn, (ushort)1);
        sound.Effect(0, 1);
        sound.Flush();
        for (long t = 1; t <= 10; t++) host.Tick(t, false);
        Assert.Empty(ticks); // no callback selected yet
        sound.SelectEffects();
        sound.Effect(0, 1);
        sound.Flush();
        Assert.True(host.EffectsTicking);
        for (long t = 11; t <= 60; t++) host.Tick(t, false);
        Assert.NotEmpty(ticks);
        Assert.All(ticks, t => Assert.InRange(t, 10, 60)); // the reset 0000:06FE silences the voices at the current tick 10

        int before = sound.Commands.Count;
        state.Set(SoundControl.SoundOn, (ushort)0);
        sound.Effect(0, 2);
        Assert.DoesNotContain(sound.Commands.Skip(before), c => c.Kind == SoundCommandKind.Effect);
    }
}
