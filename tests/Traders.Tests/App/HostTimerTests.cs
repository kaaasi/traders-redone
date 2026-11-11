using Traders.App.Timing;
using Traders.RefEmu;
using Traders.RefEmu.Hardware;

namespace Traders.Tests.App;

/// <summary>Host clock and timer bookkeeping, against the RefEmu where the original decides.</summary>
public class HostTimerTests(OriginalInputProbeTests.MenuFixture menu) : IClassFixture<OriginalInputProbeTests.MenuFixture>
{
    // [DIFFERENTIAL] 27C6:037A
    [GameDataFact]
    public void Switching_the_timer_on_sets_the_divisor_and_clears_both_counters()
    {
        var p = OriginalProgram.LoadOnly(TestGameData.DosDir);
        p.WriteWord(TimerTicks.CounterAOffset, 0x1234);
        p.WriteWord(TimerTicks.CounterBOffset, 0x5678);
        p.Call(0x27C6, 0x037A);

        var port = new TimerTicks { CounterA = 0x1234, CounterB = 0x5678 };
        port.Reset();
        Assert.Equal(HostClock.TimerDivisor, p.Machine.Pit.Channel0Period);
        Assert.Equal(port.CounterA, p.ReadWord(TimerTicks.CounterAOffset));
        Assert.Equal(port.CounterB, p.ReadWord(TimerTicks.CounterBOffset));
        Assert.Equal((p.Segment(0x27C6), (ushort)0x03E4), (p.Machine.Mem.Read16(0, 0x22), p.Machine.Mem.Read16(0, 0x20)));
    }

    public static TheoryData<ushort, ushort, double> TickCases => new()
    {
        { 0, 0, 3.0 },
        { 0xFFF0, 0xFFFE, 2.0 },   // both counters wrap
        { 7, 1, 1.5 },
        { 0x8000, 3, 5.0 },
    };

    // [DIFFERENTIAL] 27C6:03E4
    [GameDataTheory]
    [MemberData(nameof(TickCases))]
    public void Ticks_count_both_words_and_pass_every_fourth_on_like_the_original(ushort a, ushort b, double seconds)
    {
        var p = menu.Program; // shared boot; every case only looks at the time after it set the counters
        // Set the counters between two ticks: wait for a tick, let its handler finish (1 ms, a tick is 13.7 ms).
        ushort before = p.ReadWord(TimerTicks.CounterBOffset);
        p.Machine.RunFor(1.0, _ => p.ReadWord(TimerTicks.CounterBOffset) != before);
        p.Machine.RunFor(0.001);
        p.WriteWord(TimerTicks.CounterAOffset, a);
        p.WriteWord(TimerTicks.CounterBOffset, b);
        ushort bios0 = p.Machine.Mem.Read16(0x40, 0x6C);
        p.Machine.RunFor(seconds);
        int n = (ushort)(p.ReadWord(TimerTicks.CounterAOffset) - a);

        var port = new TimerTicks { CounterA = a, CounterB = b };
        int callbacks = 0;
        port.Callback = () => callbacks++;
        for (int i = 0; i < n; i++) port.Tick();

        Assert.InRange(n, (int)(seconds * HostClock.TimerHz) - 1, (int)(seconds * HostClock.TimerHz) + 1);
        Assert.Equal(n, callbacks);
        Assert.Equal(port.CounterA, p.ReadWord(TimerTicks.CounterAOffset));
        Assert.Equal(port.CounterB, p.ReadWord(TimerTicks.CounterBOffset));
        Assert.Equal(port.Chained, (ushort)(p.Machine.Mem.Read16(0x40, 0x6C) - bios0));
    }

    [Fact]
    public void Ten_seconds_give_728_ticks_and_700_frames()
    {
        var c = new HostClock();
        Assert.Equal((728, 700), c.Advance(TimeSpan.FromSeconds(10)));
        Assert.Equal(11_931_820, c.Clocks);
    }

    public static TheoryData<double> FrameRates => new() { 30, 50, 59.94, 60, 70, 75, 120, 144, 240, 1000 };

    [Theory]
    [MemberData(nameof(FrameRates))]
    public void Tick_and_frame_count_do_not_depend_on_the_host_frame_rate(double hz)
    {
        var c = new HostClock();
        var step = TimeSpan.FromTicks((long)Math.Round(TimeSpan.TicksPerSecond / hz));
        long total = 0, ticks = 0, frames = 0;
        while (total + step.Ticks <= 20 * TimeSpan.TicksPerSecond)
        {
            var (t, f) = c.Advance(step);
            ticks += t;
            frames += f;
            total += step.Ticks;
        }
        c.Advance(TimeSpan.FromTicks(20 * TimeSpan.TicksPerSecond - total));
        var reference = new HostClock();
        reference.Advance(TimeSpan.FromSeconds(20));
        Assert.Equal(reference.Clocks, c.Clocks);
        Assert.Equal(reference.Ticks, c.Ticks);
        Assert.Equal(reference.Frames, c.Frames);
        Assert.True(ticks <= c.Ticks && frames <= c.Frames);
    }

    [Fact]
    public void Irregular_steps_add_up_exactly()
    {
        var rnd = new Random(99);
        var c = new HostClock();
        long sum = 0, ticks = 0, frames = 0;
        for (int i = 0; i < 5000; i++)
        {
            long s = rnd.Next(0, 400_000); // up to 40 ms
            sum += s;
            var (t, f) = c.Advance(TimeSpan.FromTicks(s));
            Assert.InRange(t, 0, 3);
            Assert.InRange(f, 0, 3);
            ticks += t;
            frames += f;
        }
        Assert.Equal(sum * HostClock.PitHz / TimeSpan.TicksPerSecond, c.Clocks);
        Assert.Equal(c.Ticks, ticks);
        Assert.Equal(c.Frames, frames);
        Assert.Equal(c.Clocks / 0x4000, c.Ticks);
        Assert.Equal(c.Clocks / (Pit.ClockHz / 70), c.Frames);
    }

    [Fact]
    public void Negative_time_is_rejected() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new HostClock().Advance(TimeSpan.FromTicks(-1)));

    [Fact]
    public void Frame_period_matches_the_refemu_display()
    {
        Assert.Equal(HostClock.FramePeriod, new VgaAdapter().FramePeriod);
        Assert.Equal(HostClock.PitHz, Pit.ClockHz);
    }
}
