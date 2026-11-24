using Traders.App.Audio;
using Traders.Audio.Opl;

namespace Traders.Tests.Audio;

/// <summary>Host sound output (lane klang, part 2): scheduling by host ticks, gap-free queueing, running without a device.</summary>
public class OplHostAudioTests
{
    private const int Rate = 44_100;

    private sealed class FakeOutput : IPcmOutput
    {
        public List<short[]> Buffers { get; } = [];
        public int Played { get; set; }
        public int SampleRate => Rate;
        public int PendingBuffers => Buffers.Count - Played;
        public void Submit(ReadOnlySpan<short> samples) => Buffers.Add(samples.ToArray());
        public void Dispose() { }
        public short[] All() => Buffers.SelectMany(b => b).ToArray();
    }

    private static readonly (byte Reg, byte Value)[] ToneOn =
    [
        (0x20, 0x01), (0x40, 0x3f), (0x60, 0x00), (0x23, 0x21), (0x43, 0x00), (0x63, 0xf0), (0x83, 0x08),
        (0xc0, 0x01), (0xa0, 0x44), (0xb0, 0x32),
    ];

    private static int FirstLoud(short[] s, int from = 0)
    {
        for (int i = from; i < s.Length; i++) if (Math.Abs((int)s[i]) > 1000) return i;
        return -1;
    }

    [Fact]
    public void Create_never_throws_and_the_host_can_run_without_sound_device()
    {
        using var audio = HostAudio.Create(log: TextWriter.Null);
        audio.Write(0, 0xb0, 0x20);
        audio.Update();
        audio.Update();
        if (audio.IsSilent) Assert.Null(audio.Scheduler);
    }

    [Fact]
    public void Silent_instance_drops_writes()
    {
        using var audio = new HostAudio(null);
        Assert.True(audio.IsSilent);
        for (int i = 0; i < 100; i++) audio.Write(i, 0xb0, 0x20);
        audio.Update();
    }

    [Fact]
    public void Update_keeps_three_buffers_queued_without_gaps()
    {
        var output = new FakeOutput();
        using var audio = new HostAudio(output);
        audio.Update();
        Assert.Equal(HostAudio.QueuedBuffers, output.Buffers.Count);
        Assert.All(output.Buffers, b => Assert.Equal(Rate / 60, b.Length));
        output.Played += 2;
        audio.Update();
        Assert.Equal(HostAudio.QueuedBuffers + 2, output.Buffers.Count);
        audio.Update();
        Assert.Equal(HostAudio.QueuedBuffers + 2, output.Buffers.Count);
        Assert.Equal(output.Buffers.Count * (long)(Rate / 60), audio.Scheduler!.Rendered);
    }

    [Fact]
    public void Write_sounds_after_the_latency_and_follows_the_tick_spacing()
    {
        // host loop: 60 updates per second, the device plays one buffer per update, writes are made when their tick comes
        var output = new FakeOutput();
        using var audio = new HostAudio(output);
        var clock = new Traders.App.Timing.HostClock();
        var script = ToneOn.Select(w => (Tick: 10L, w.Reg, w.Value)).ToList();
        script.Add((10 + 73, 0xb0, 0x12));      // key off about one second later
        script.Add((10 + 2 * 73, 0xb0, 0x32));  // key on again
        int next = 0;
        long renderedAtFirstWrite = -1;
        for (int i = 0; i < 240; i++)
        {
            clock.Advance(TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 60));
            while (next < script.Count && script[next].Tick <= clock.Ticks)
            {
                if (renderedAtFirstWrite < 0) renderedAtFirstWrite = audio.Scheduler!.Rendered;
                audio.Write(script[next].Tick, script[next].Reg, script[next].Value);
                next++;
            }
            audio.Update();
            output.Played = Math.Max(output.Played, output.Buffers.Count - HostAudio.QueuedBuffers + 1);
        }
        var pcm = output.All();

        int latency = audio.Scheduler!.LatencySamples;
        int on = FirstLoud(pcm);
        // one latency after the render position at the time of the write (plus resampler delay and rise of the sine)
        Assert.InRange(on - renderedAtFirstWrite, latency, latency + 10);
        // second key on 146 ticks (146 · 16384 / 1 193 182 s) after the first
        int expected = on + (int)Math.Round(146 * 16384.0 * Rate / 1_193_182);
        int again = FirstLoud(pcm, on + Rate * 3 / 2);
        Assert.InRange(again, expected - Rate / 60 - 3, expected + Rate / 60 + 3); // writes arrive per host update
        Assert.Equal(1, audio.Scheduler.Anchorings);
    }

    [Fact]
    public void Late_writes_are_reanchored_in_order()
    {
        var s = new OplScheduler(Rate, latencySamples: 2205);
        s.Enqueue(new OplWrite(0, 0x20, 0x01));
        s.Render(new short[Rate * 2]);            // device ran on (stall of the game)
        s.Enqueue(new OplWrite(1, 0x23, 0x21));   // tick 1 is long past
        s.Enqueue(new OplWrite(1, 0x63, 0xf0));
        s.Enqueue(new OplWrite(1, 0xa0, 0x44));
        s.Enqueue(new OplWrite(1, 0xb0, 0x32));
        Assert.Equal(2, s.Anchorings);
        var pcm = new short[Rate];
        s.Render(pcm);
        Assert.InRange(FirstLoud(pcm), 2205, 2215);
        Assert.Equal(0, s.Pending);
    }

    [Fact]
    public void Writes_of_one_tick_keep_their_order()
    {
        var s = new OplScheduler(Rate, latencySamples: 100);
        // key on first, then mute with total level: the order decides between tone and silence
        foreach (var (r, v) in ToneOn) s.Enqueue(new OplWrite(5, r, v));
        s.Enqueue(new OplWrite(5, 0x43, 0x3f));
        var pcm = new short[Rate / 2];
        s.Render(pcm);
        Assert.True(pcm.Max(v => Math.Abs((int)v)) < 100);
    }

    /// <summary>
    /// R9K-3 (#256): a write for an earlier tick queued after a re-anchoring by a later tick (the effect and music streams
    /// share one OPL2, their writes of neighbouring ticks interleave) goes neither before that write's sample position nor
    /// before it in order: the tone switched on by the re-anchoring write is muted by the later-queued earlier one.
    /// Counter-check: <c>target = Math.Max(target, _lastTarget)</c> removed (mutation M10) → red.
    /// </summary>
    [Fact]
    public void A_write_after_a_reanchoring_by_a_later_tick_keeps_its_order()
    {
        // latency above one tick (about 605 samples): tick 39 lands after the render position but before tick 40's target
        var s = new OplScheduler(Rate, latencySamples: 2000);
        s.Enqueue(new OplWrite(1, 0x20, 0x01));
        s.Render(new short[Rate]); // stall: tick 40 would land in the past
        foreach (var (r, v) in ToneOn) s.Enqueue(new OplWrite(40, r, v));
        Assert.Equal(2, s.Anchorings);
        long reanchored = s.LastTarget;
        s.Enqueue(new OplWrite(39, 0x43, 0x3f)); // earlier tick, queued later
        Assert.True(s.LastTarget >= reanchored, $"target {s.LastTarget} before the re-anchored write at {reanchored}");
        Assert.Equal(2, s.Anchorings);
        var pcm = new short[Rate / 2];
        s.Render(pcm);
        Assert.Equal(0, s.Pending);
        Assert.True(pcm.Max(v => Math.Abs((int)v)) < 100, "the mute was applied before the key on");
    }

    [Fact]
    public void Slow_device_keeps_every_write_on_its_own_sample_position()
    {
        // R9K-2: the device plays 5 % slower than the host clock (about 73 timer ticks per second, one write per tick);
        // a forward correction must not pile writes up on one sample position while the render position catches up
        const int latency = 2205;
        var s = new OplScheduler(Rate, latency);
        int perTick = (int)(Rate * 16384.0 / 1_193_182 * 0.95);
        var block = new short[perTick];
        long previous = -1, maxLag = 0;
        for (int tick = 1; tick <= 120 * 73; tick++)
        {
            s.Enqueue(new OplWrite(tick, 0x20, 0x01));
            Assert.True(s.LastTarget > previous, $"tick {tick}: target {s.LastTarget} not after {previous}");
            previous = s.LastTarget;
            maxLag = Math.Max(maxLag, s.LastTarget - s.Rendered);
            s.Render(block);
        }
        Assert.Equal(1, s.Anchorings);
        Assert.True(s.Corrections > 0);
        Assert.InRange(maxLag, 1, 4L * latency + Rate / 10 + Rate / 10); // stays near the re-anchoring limit
    }

    [Fact]
    public void Write_after_a_pause_of_a_second_or_more_jumps_ahead_cleanly()
    {
        var s = new OplScheduler(Rate, latencySamples: 2205);
        s.Enqueue(new OplWrite(0, 0x20, 0x01));
        s.Render(new short[Rate / 4]);
        s.Enqueue(new OplWrite(300, 0x20, 0x01));     // about 4 s of host time later, device only played 0.25 s
        Assert.Equal(2, s.Anchorings);
        Assert.Equal(Rate / 4 + 2205, s.LastTarget);
        Assert.Equal(0, s.Corrections);
    }
}
