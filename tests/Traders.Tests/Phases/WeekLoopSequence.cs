namespace Traders.Tests.Phases;

/// <summary>Kind of an event recorded while the week loop 1777:01F8 runs.</summary>
public enum WeekEventKind
{
    /// <summary>A far call site of the week loop is reached (Value = site offset in segment 1777).</summary>
    Call,

    /// <summary>The phase marker routine 21FD:0977 is entered (Value = its word argument).</summary>
    Marker,

    /// <summary>The week counter increment at 1777:02D3 is reached (Value = counter before the increment).</summary>
    Increment,

    /// <summary>The jump back to the loop start at 1777:02D7 is reached (Value = counter after the increment).</summary>
    BackJump,
}

/// <summary>
/// One event of the week loop. <paramref name="Week"/> is DS:4B9A when the event happens; for <see cref="WeekEventKind.Call"/>
/// <paramref name="TargetSegment"/>:<paramref name="TargetOffset"/> is the far call target as an image address.
/// </summary>
public readonly record struct WeekEvent(WeekEventKind Kind, int Week, int Value, ushort TargetSegment = 0, ushort TargetOffset = 0)
{
    public override string ToString() => Kind == WeekEventKind.Call
        ? $"week {Week} call 1777:{Value:X4} -> {TargetSegment:X4}:{TargetOffset:X4}"
        : $"week {Week} {Kind} {Value}";
}

/// <summary>
/// Expected order of the week loop 1777:01F8 as read statically in docs/routines/1777_01F8_wochenschleife.md (table
/// "Aufruffolge einer Woche"): 30 far calls, the phase markers 0..6 after calls 6, 9, 14, 16, 20, 24 and 27, then the week
/// counter increment. Nothing here is taken from a RefEmu run.
/// </summary>
public static class WeekLoopSequence
{
    public const ushort Segment = 0x1777;
    public const ushort Entry = 0x01F8;
    public const ushort CounterIncrement = 0x02D3;
    public const ushort BackJump = 0x02D7;
    public const ushort Exit = 0x02DA;

    /// <summary>Return address in main 1777:000C after the call of the week loop at 1777:017F.</summary>
    public const ushort ReturnInMain = 0x0184;

    /// <summary>Image address of the main menu, called by main at 1777:0163.</summary>
    public const ushort MainMenuSegment = 0x192D, MainMenuOffset = 0x0007;

    /// <summary>One far call of the loop; <paramref name="Marker"/> is set for calls of the phase marker routine.</summary>
    public readonly record struct LoopCall(ushort Site, ushort TargetSegment, ushort TargetOffset, int? Marker = null);

    public static readonly LoopCall[] Calls =
    [
        new(0x01FB, 0x1777, 0x02DC),     // 1 weekly prices, loop start
        new(0x0200, 0x0825, 0x07C4),     // 2 wealth history
        new(0x0208, 0x0825, 0x000A),     // 3 weekly balance; exit A is checked after it
        new(0x021A, 0x111D, 0x0009),     // 4 disasters
        new(0x021F, 0x0609, 0x1178),     // 5 limits from production fields
        new(0x0227, 0x11A3, 0x0000),     // 6 computer players, pass 0
        new(0x0230, 0x21FD, 0x0977, 0),  // 7 marker 0
        new(0x0236, 0x0609, 0x0004),     // 8 planting
        new(0x023B, 0x17CB, 0x0005),     // 9 weekly events
        new(0x0244, 0x21FD, 0x0977, 1),  // 10 marker 1
        new(0x024A, 0x1B03, 0x0009),     // 11 stations, rocket, pirates
        new(0x024F, 0x21FD, 0x021F),     // 12 weather
        new(0x0254, 0x22A3, 0x0003),     // 13 production
        new(0x0259, 0x22A3, 0x026B),     // 14 stores to 8/10
        new(0x0262, 0x21FD, 0x0977, 2),  // 15 marker 2
        new(0x0268, 0x22A3, 0x02D3),     // 16 harvest
        new(0x0271, 0x21FD, 0x0977, 3),  // 17 marker 3
        new(0x0277, 0x1C68, 0x000B),     // 18 robot phase
        new(0x027C, 0x20B3, 0x0005),     // 19 end check; exit B is checked after it
        new(0x0289, 0x0609, 0x1178),     // 20 limits again
        new(0x0291, 0x11A3, 0x0000),     // 21 computer players, pass 1
        new(0x029B, 0x21FD, 0x0977, 4),  // 22 marker 4
        new(0x02A1, 0x0B9F, 0x0002),     // 23 trade
        new(0x02A6, 0x1DEF, 0x000C),     // 24 extra phase
        new(0x02AF, 0x21FD, 0x0977, 5),  // 25 marker 5
        new(0x02B5, 0x1F63, 0x000F),     // 26 Fat Mike
        new(0x02BA, 0x08CF, 0x11D7),     // 27 gem price of the week
        new(0x02C3, 0x21FD, 0x0977, 6),  // 28 marker 6
        new(0x02C9, 0x08CF, 0x0004),     // 29 purchase
        new(0x02CE, 0x1F63, 0x0B03),     // 30 contest after the purchase
    ];

    /// <summary>
    /// Expected events of the complete weeks <paramref name="firstWeek"/> .. <paramref name="firstWeek"/> + <paramref name="weeks"/> - 1,
    /// followed by the first call of the next week (the loop start, where a run stopped by RunToWeekStart ends).
    /// </summary>
    public static List<WeekEvent> Expected(int firstWeek, int weeks)
    {
        var list = new List<WeekEvent>();
        for (int w = firstWeek; w < firstWeek + weeks; w++)
        {
            foreach (var c in Calls)
            {
                list.Add(new WeekEvent(WeekEventKind.Call, w, c.Site, c.TargetSegment, c.TargetOffset));
                if (c.Marker is int marker) list.Add(new WeekEvent(WeekEventKind.Marker, w, marker));
            }
            list.Add(new WeekEvent(WeekEventKind.Increment, w, w));
            list.Add(new WeekEvent(WeekEventKind.BackJump, w + 1, w + 1));
        }
        var first = Calls[0];
        list.Add(new WeekEvent(WeekEventKind.Call, firstWeek + weeks, first.Site, first.TargetSegment, first.TargetOffset));
        return list;
    }

    /// <summary>Null if <paramref name="actual"/> equals <paramref name="expected"/>, otherwise the first difference with context.</summary>
    public static string? Difference(IReadOnlyList<WeekEvent> expected, IReadOnlyList<WeekEvent> actual)
    {
        int n = Math.Min(expected.Count, actual.Count);
        for (int i = 0; i < n; i++)
            if (expected[i] != actual[i])
                return $"event {i}: expected <{expected[i]}>, actual <{actual[i]}>\n{Context(actual, i)}";
        if (expected.Count == actual.Count) return null;
        return expected.Count > actual.Count
            ? $"missing from event {n}: <{expected[n]}> ({expected.Count - n} expected events not recorded)\n{Context(actual, n)}"
            : $"extra from event {n}: <{actual[n]}> ({actual.Count - n} events more than expected)\n{Context(actual, n)}";
    }

    private static string Context(IReadOnlyList<WeekEvent> actual, int at) =>
        "recorded around it:\n" + string.Join('\n',
            Enumerable.Range(Math.Max(0, at - 3), Math.Min(actual.Count, at + 3) - Math.Max(0, at - 3)).Select(i => $"  {i}: {actual[i]}"));
}
