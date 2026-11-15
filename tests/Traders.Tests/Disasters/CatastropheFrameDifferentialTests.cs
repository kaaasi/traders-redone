using Xunit.Abstractions;
using static Traders.Tests.Disasters.CatastropheFrameHarness;

namespace Traders.Tests.Disasters;

/// <summary>
/// The catastrophes 111D:0009 as call 4 of the phase machine in lockstep with the original in a running game
/// (<see cref="CatastropheFrameHarness"/>): every retrace of the original is a frame of the port; data segment, DS:10D2 and
/// the number of random() calls are equal at each of them and at the return. Weeks 3–5 of a game reached by four computers,
/// with computers only and with a human (control 4, Ins confirms the wait loop 21FD:0004(3)), every kind and "none".
/// </summary>
public class CatastropheFrameDifferentialTests(ITestOutputHelper output)
{
    private Result Check(Script script, int kind)
    {
        var r = Run(script);
        output.WriteLine($"{script}: kind {r.Kind}, retraces {r.Retraces} (wait loop {r.WaitLoopFrames}), polls {r.Polls}, " +
            $"random {r.RandomCalls}, typed {r.Typed}");
        Assert.Equal(kind, r.Kind);
        if (kind >= 0)
        {
            Assert.True(r.Typed > 0, "no character typed");
            Assert.True(r.WaitLoopFrames > 0, "the wait loop 21FD:0004 ran no frame");
        }
        else Assert.Equal(0, r.Retraces);
        return r;
    }

    /// <summary>Ins (fire of control 4) every 40 frames for 4 frames from frame <paramref name="from"/> to <paramref name="to"/>.</summary>
    private static Press[] Fire(long from, long to) =>
        [.. Enumerable.Range(0, (int)((to - from) / 40)).Select(i => new Press(from + 40L * i, 4, Ins))];

    public static TheoryData<int, int> Computers() => new()
    {
        { 3, -1 }, { 3, 0 }, { 3, 1 }, { 3, 2 },
        { 4, 0 }, { 4, 2 },
        { 5, 1 }, { 5, -1 },
    };

    // [DIFFERENTIAL] 111D:0009 – whole routine frame by frame in weeks 3–5 of a running game, four computers, every kind
    // [DIFFERENTIAL] 111D:0757 – message window frames (fades, flips)
    // [DIFFERENTIAL] 0A69:0288 – typed lines: one flip, pause check and 21FD:0185(3) per character
    // [DIFFERENTIAL] 0A69:0243 – centred typewriter line (frames)
    // [DIFFERENTIAL] 0A69:01AD – typewriter frames per character
    // [DIFFERENTIAL] 21FD:0590 – draw on the page shown: two retraces (in 111D:0519)
    [GameDataTheory]
    [MemberData(nameof(Computers))]
    public void Catastrophe_with_four_computers_runs_in_lockstep(int week, int kind) =>
        Check(new Script(week, [], RandomFor(week, kind), []), kind);

    public static TheoryData<int, int> Humans() => new()
    {
        { 3, 0 }, { 4, 1 }, { 5, 2 },
    };

    /// <summary>A human (player 0 or 2, control 4) confirms the wait loop with Ins; presses before it change nothing.</summary>
    [GameDataTheory]
    [MemberData(nameof(Humans))]
    public void Catastrophe_with_a_human_runs_in_lockstep(int week, int kind) =>
        Check(new Script(week, [week % 2 == 0 ? 2 : 0], RandomFor(week, kind), Fire(20, 6000)), kind);
}
