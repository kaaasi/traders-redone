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
        new long[] { 0, 15, 30, 45, 60, 75, 90 }, // steady catch-up of 15 frames (a quarter second is about 17)
        new long[] { 5, 22, 39, 56, 73, 90 },     // jumps of 17, the upper bound of one update
    };

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
