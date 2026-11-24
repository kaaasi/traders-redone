using Traders.Audio.Effects;
using Traders.Audio.Opl;

namespace Traders.Tests.Audio;

/// <summary>
/// R9E-2 (#254): the effect machine takes the timer tick from its caller (<see cref="EffectMachine.Tick(long)"/>, as
/// <c>FxMusicDriver.Tick</c>), so effect and music writes lie on one time axis (the counter DS:1180 after its increment, plan
/// R9 "Einheit"). The writes of a tick carry the caller's tick, writes outside a tick (<see cref="EffectMachine.Stop"/>) the tick
/// the caller set last; <see cref="EffectMachine.WrittenAt"/> reports every write as <see cref="OplWrite"/>. Which writes a tick
/// makes is the differential tests' (<see cref="EffectsDifferentialTests"/>): the tick number does not change them.
/// </summary>
public class EffectsTimerTickTests
{
    private static EffectMachine Machine() => new(EffectImage.FromExe(File.ReadAllBytes(TestGameData.Dos("T.EXE"))));

    [GameDataFact]
    public void Writes_carry_the_callers_timer_tick_and_are_the_same_as_with_counted_ticks()
    {
        var counted = Machine();
        var given = Machine();
        foreach (var m in new[] { counted, given })
        {
            m.Reset(); // its writes carry tick 0 on both
            m.Enqueue(1);
            m.ClearWrites();
        }
        var reported = new List<OplWrite>();
        given.WrittenAt += reported.Add;
        const long first = 4711;
        for (int i = 0; i < 40; i++)
        {
            counted.Tick();
            given.Tick(first + 4 * i); // any numbering of the caller, here every fourth timer tick
        }
        Assert.Equal(40, given.TickCount);
        Assert.Equal(first + 4 * 39, given.CurrentTick);
        Assert.Equal(40, counted.CurrentTick);
        Assert.True(given.Writes.Count > 10, $"effect 1 wrote {given.Writes.Count} times");
        Assert.Equal(counted.Writes.Select(w => (w.Register, w.Value)), given.Writes.Select(w => (w.Register, w.Value)));
        Assert.Equal(counted.Writes.Select(w => first + 4 * (w.Tick - 1)), given.Writes.Select(w => w.Tick));
        Assert.Equal(given.Writes.Select(w => new OplWrite(w.Tick, w.Register, w.Value)), reported);

        // outside a tick: the tick the caller set last
        given.ClearWrites();
        given.CurrentTick = 9000;
        given.Stop();
        Assert.NotEmpty(given.Writes);
        Assert.All(given.Writes, w => Assert.Equal(9000, w.Tick));
    }
}
