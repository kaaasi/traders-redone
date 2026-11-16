using Traders.App;
using Traders.App.Rendering;
using Traders.Core;
using Traders.Core.State;

namespace Traders.Tests.App;

/// <summary>
/// Catch-up of the host (finding R4G-1, issue #110): a <see cref="ProgramHost.Frame"/> call that jumps over several frames
/// (an update after a short stall, as <see cref="HostGame"/> makes it) gives the same result as calling it for every frame:
/// core and menu scene run in step frame by frame.
/// </summary>
public class ProgramHostCatchUpTests
{
    private const int Clock = 12345678;

    /// <summary>Frames reached by the host in each call; every later entry is compared with a host run frame by frame.</summary>
    public static TheoryData<long[]> Jumps => new()
    {
        new long[] { 20, 35 },                    // probe of the review: 21 single frames, then a jump of 15
        new long[] { 0, 15, 30, 45, 60, 75, 90 }, // steady catch-up of 15 frames
        new long[] { 5, 22, 39, 56, 73, 90 },     // jumps of 17 and
        new long[] { 4, 22, 40, 58, 76, 94 },     // of 18: one update of HostGame.MaxStep (0.25 s at 70 Hz) covers 17 or 18
    };

    /// <summary>
    /// The bound of the jumps above (#113): an update of <see cref="HostGame.MaxStep"/> completes 17 or 18 frames of the
    /// host clock, depending on where in a frame it starts – never more than 18.
    /// </summary>
    [Fact]
    public void One_update_of_the_longest_step_covers_17_or_18_frames()
    {
        var counts = new HashSet<int>();
        for (int phase = 0; phase < 70; phase++)
        {
            var clock = new Traders.App.Timing.HostClock();
            clock.Advance(TimeSpan.FromSeconds(phase / 4900.0)); // 70 starting points within one frame
            counts.Add(clock.Advance(HostGame.MaxStep).Frames);
        }
        Assert.Equal([17, 18], counts.Order());
    }

    /// <summary>
    /// Issue #158: the frame <see cref="HostGame.Update"/> hands to <see cref="ProgramHost.Frame"/> is the last completed one
    /// of the host clock – none before the first frame is complete, frame n − 1 after n frames, also over a jump of
    /// <see cref="HostGame.MaxStep"/>. Counter-check: <c>Frames</c> instead of <c>Frames - 1</c> (one frame ahead) → red.
    /// </summary>
    [Fact]
    public void The_target_frame_is_the_last_frame_the_host_clock_completed()
    {
        var clock = new Traders.App.Timing.HostClock();
        Assert.Equal(-1, HostGame.TargetFrame(clock));
        clock.Advance(TimeSpan.FromSeconds(0.5 / 70));
        Assert.Equal(0, clock.Frames);
        Assert.Equal(-1, HostGame.TargetFrame(clock));
        // 1/70 s takes 1193182 / 70 clocks; the period is FramePeriod = 17045 clocks, so one frame completes after 1/70 s
        clock.Advance(TimeSpan.FromSeconds(0.5 / 70));
        Assert.Equal(1, clock.Frames);
        Assert.Equal(0, HostGame.TargetFrame(clock));
        clock.Advance(HostGame.MaxStep);
        Assert.Equal(clock.Frames - 1, HostGame.TargetFrame(clock));
        Assert.InRange(HostGame.TargetFrame(clock), 17, 18);
    }

    /// <summary>
    /// Issue #158 with the program: after updates of the host clock (single frames, a frame split over two updates and a
    /// stall of <see cref="HostGame.MaxStep"/>) the program has processed exactly the completed frames 0 … n − 1, as
    /// <see cref="HostGame.Update"/> drives it – one frame more or less than the clock is red.
    /// </summary>
    [GameDataFact]
    public void The_program_processes_exactly_the_frames_of_the_host_clock()
    {
        var host = new ProgramHost(TestGameData.DosDir, new RecordedClock([Clock]));
        var clock = new Traders.App.Timing.HostClock();
        foreach (var step in new[] { 1.0 / 70, 1.0 / 70, 0.3 / 70, 0.9 / 70, 0.25, 1.0 / 60, 1.0 / 60, 0.25 })
        {
            var (_, frames) = clock.Advance(TimeSpan.FromSeconds(step));
            if (frames > 0) host.Frame(HostGame.TargetFrame(clock), []);
            Assert.Equal(clock.Frames - 1, host.LastFrame);
        }
        Assert.True(host.LastFrame > 30);
    }

    [GameDataTheory]
    [MemberData(nameof(Jumps))]
    public void A_jump_over_several_frames_matches_processing_them_one_by_one(long[] reached)
    {
        var single = new ProgramHost(TestGameData.DosDir, new RecordedClock([Clock]));
        var jumped = new ProgramHost(TestGameData.DosDir, new RecordedClock([Clock]));
        long next = 0;
        foreach (long frame in reached)
        {
            for (; next <= frame; next++) single.Frame(next, []);
            jumped.Frame(frame, []);
            AssertSame(single, jumped, frame);
        }
    }

    private static void AssertSame(ProgramHost expected, ProgramHost actual, long frame)
    {
        Assert.Equal(expected.LastFrame, actual.LastFrame);
        Assert.Equal(expected.Machine.Status, actual.Machine.Status);
        Assert.True(expected.Core.U32(DgroupLayout.RandomSeed) == actual.Core.U32(DgroupLayout.RandomSeed), $"DS:10D2 at frame {frame}");
        Assert.True(expected.Core.Bytes.AsSpan().SequenceEqual(actual.Core.Bytes), $"core state at frame {frame}");
        Assert.True(expected.Layers.Ds.AsSpan().SequenceEqual(actual.Layers.Ds), $"scene data segment at frame {frame}");
        Assert.True(expected.Layers.Dac.Registers.AsSpan().SequenceEqual(actual.Layers.Dac.Registers), $"palette at frame {frame}");
        Assert.Equal(expected.Driver.DisplayStart, actual.Driver.DisplayStart);
        var x = new uint[FramePresenter.Width * FramePresenter.Height];
        var y = new uint[FramePresenter.Width * FramePresenter.Height];
        FramePresenter.Present(expected.Driver.Vram, expected.Driver.DisplayStart, expected.Layers.Dac.Registers, x);
        FramePresenter.Present(actual.Driver.Vram, actual.Driver.DisplayStart, actual.Layers.Dac.Registers, y);
        int first = x.AsSpan().CommonPrefixLength(y);
        Assert.True(first == x.Length, $"picture at frame {frame}: first different pixel {first}");
    }
}
