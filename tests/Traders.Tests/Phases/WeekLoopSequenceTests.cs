using static Traders.Tests.Phases.WeekLoopSequence;

namespace Traders.Tests.Phases;

/// <summary>
/// The comparison used by <see cref="WeekLoopTests"/> rejects a swapped, missing or extra marker, a wrong call order and a
/// wrong week counter (counter-check of the acceptance criteria; runs without original data).
/// </summary>
public class WeekLoopSequenceTests
{
    private static List<WeekEvent> TwoWeeks() => Expected(0, 2);

    [Fact]
    public void Table_has_30_calls_and_markers_0_to_6_in_order()
    {
        Assert.Equal(30, Calls.Length);
        Assert.Equal(Enumerable.Range(0, 7), Calls.Where(c => c.Marker is not null).Select(c => c.Marker!.Value));
        Assert.All(Calls.Where(c => c.Marker is not null), c => Assert.Equal((0x21FD, 0x0977), (c.TargetSegment, c.TargetOffset)));
        Assert.Equal(Calls.OrderBy(c => c.Site).Select(c => c.Site), Calls.Select(c => c.Site));
    }

    [Fact]
    public void Expected_sequence_matches_itself()
    {
        var e = TwoWeeks();
        Assert.Equal(2 * (30 + 7 + 2) + 1, e.Count);
        Assert.Null(Difference(e, TwoWeeks()));
    }

    [Theory]
    [InlineData(2, 3)]
    [InlineData(0, 6)]
    [InlineData(4, 5)]
    public void Swapped_markers_are_rejected(int a, int b)
    {
        var e = TwoWeeks();
        var actual = TwoWeeks();
        int ia = actual.FindLastIndex(x => x.Kind == WeekEventKind.Marker && x.Value == a);
        int ib = actual.FindLastIndex(x => x.Kind == WeekEventKind.Marker && x.Value == b);
        actual[ia] = actual[ia] with { Value = b };
        actual[ib] = actual[ib] with { Value = a };
        Assert.NotNull(Difference(e, actual));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(6)]
    public void Missing_marker_is_rejected(int marker)
    {
        var actual = TwoWeeks();
        actual.RemoveAt(actual.FindLastIndex(x => x.Kind == WeekEventKind.Marker && x.Value == marker));
        Assert.NotNull(Difference(TwoWeeks(), actual));
    }

    [Fact]
    public void Extra_marker_is_rejected()
    {
        var actual = TwoWeeks();
        int i = actual.FindIndex(x => x.Kind == WeekEventKind.Marker && x.Value == 3);
        actual.Insert(i, actual[i]);
        Assert.NotNull(Difference(TwoWeeks(), actual));
    }

    [Fact]
    public void Swapped_calls_are_rejected()
    {
        var actual = TwoWeeks();
        (actual[3], actual[4]) = (actual[4], actual[3]);
        Assert.NotNull(Difference(TwoWeeks(), actual));
    }

    [Fact]
    public void Wrong_week_counter_is_rejected()
    {
        var actual = TwoWeeks();
        int i = actual.FindLastIndex(x => x.Kind == WeekEventKind.BackJump);
        actual[i] = actual[i] with { Week = 1, Value = 1 };
        Assert.NotNull(Difference(TwoWeeks(), actual));
    }

    [Fact]
    public void Run_stopping_early_is_rejected()
    {
        var actual = TwoWeeks();
        actual.RemoveRange(actual.Count - 10, 10);
        Assert.NotNull(Difference(TwoWeeks(), actual));
    }
}
