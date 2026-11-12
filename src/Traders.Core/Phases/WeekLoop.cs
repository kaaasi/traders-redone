namespace Traders.Core.Phases;

/// <summary>Kind of a step of the week loop.</summary>
public enum WeekStepKind
{
    /// <summary>A far call of the loop into a phase or rule routine; handled by an <see cref="IWeekStepHandler"/>.</summary>
    Call,

    /// <summary>A call of the empty phase marker 21FD:0977(n); the machine passes it itself (no side effects in the original).</summary>
    Marker,
}

/// <summary>Exit check the loop performs right after a step.</summary>
public enum WeekExitCheck
{
    None,

    /// <summary>Exit A after the weekly balance: key state of scancode 0Eh (DS:3D55) not zero → return AL = 0 (main menu).</summary>
    AbortToMenu,

    /// <summary>Exit B after the end check 20B3:0005: its result AL = 1 → return AL = 1 (game over).</summary>
    GameOver,
}

/// <summary>
/// One of the 30 far calls of the week loop 1777:01F8, in loop order. <see cref="Number"/> is the call number 1..30 of
/// docs/routines/1777_01F8_wochenschleife.md, <see cref="PlanStep"/> the step number 1..23 of docs/analyse/BESTAND.md §1.4
/// (markers have none). <see cref="Argument"/> is the byte or word pushed before the call (balance 1, computer players
/// 0/1, marker number); <see cref="Screen"/> and <see cref="UsesRandom"/> follow BESTAND.md §1.4 (read statically;
/// null = open there).
/// </summary>
public sealed record WeekStep(
    int Number,
    ushort Site,
    ushort TargetSegment,
    ushort TargetOffset,
    WeekStepKind Kind,
    int? PlanStep,
    int? Argument,
    bool Screen,
    bool? UsesRandom,
    WeekExitCheck ExitAfter,
    string Name)
{
    /// <summary>Marker number 0..6 for <see cref="WeekStepKind.Marker"/>, otherwise null.</summary>
    public int? Marker => Kind == WeekStepKind.Marker ? Argument : null;

    public override string ToString() =>
        $"{Number} 1777:{Site:X4} -> {TargetSegment:X4}:{TargetOffset:X4}{(Argument is int a ? $"({a})" : "")} {Name}";
}

/// <summary>
/// The week loop of the original as data. Each week runs the 30 calls in this order; exit A is checked after call 3, exit B
/// after call 19; after call 30 the week counter DS:4B9A is incremented (16 bit) and the loop starts again with call 1.
/// The loop has no week limit, no own random draws, no drawing and no input except reading DS:3D55.
/// </summary>
// [ORIGINAL] 1777:01F8 – week loop play_game, finding docs/routines/1777_01F8_wochenschleife.md
public static class WeekLoop
{
    public const ushort Segment = 0x1777;
    public const ushort Entry = 0x01F8;

    /// <summary>Site of <c>inc word [DS:4B9A]</c> after call 30.</summary>
    public const ushort CounterIncrementSite = 0x02D3;

    /// <summary>DS offset of the key state the loop reads for exit A (key table DS:3D47 + scancode 0Eh).</summary>
    public const int AbortKeyState = 0x3D55;

    /// <summary>Return value of the loop in AL: exit A (back to the main menu).</summary>
    public const byte ResultMainMenu = 0;

    /// <summary>Return value of the loop in AL: exit B (game over).</summary>
    public const byte ResultGameOver = 1;

    private const WeekStepKind C = WeekStepKind.Call, M = WeekStepKind.Marker;
    private const WeekExitCheck No = WeekExitCheck.None;

    public static IReadOnlyList<WeekStep> Steps { get; } =
    [
        new(1, 0x01FB, 0x1777, 0x02DC, C, 1, null, false, false, No, "weekly prices"),
        new(2, 0x0200, 0x0825, 0x07C4, C, 2, null, false, false, No, "wealth history"),
        new(3, 0x0208, 0x0825, 0x000A, C, 3, 1, true, false, WeekExitCheck.AbortToMenu, "weekly balance"),
        new(4, 0x021A, 0x111D, 0x0009, C, 4, null, true, true, No, "disasters"),
        new(5, 0x021F, 0x0609, 0x1178, C, 5, null, false, false, No, "limits from production fields"),
        new(6, 0x0227, 0x11A3, 0x0000, C, 6, 0, false, true, No, "computer players before planting"),
        new(7, 0x0230, 0x21FD, 0x0977, M, null, 0, false, false, No, "marker 0"),
        new(8, 0x0236, 0x0609, 0x0004, C, 7, null, true, true, No, "planting"),
        new(9, 0x023B, 0x17CB, 0x0005, C, 8, null, true, true, No, "weekly events"),
        new(10, 0x0244, 0x21FD, 0x0977, M, null, 1, false, false, No, "marker 1"),
        new(11, 0x024A, 0x1B03, 0x0009, C, 9, null, true, true, No, "stations, rocket, pirates"),
        new(12, 0x024F, 0x21FD, 0x021F, C, 10, null, false, true, No, "weather"),
        new(13, 0x0254, 0x22A3, 0x0003, C, 11, null, false, false, No, "production"),
        new(14, 0x0259, 0x22A3, 0x026B, C, 12, null, false, false, No, "stores to 8/10"),
        new(15, 0x0262, 0x21FD, 0x0977, M, null, 2, false, false, No, "marker 2"),
        new(16, 0x0268, 0x22A3, 0x02D3, C, 13, null, true, true, No, "harvest"),
        new(17, 0x0271, 0x21FD, 0x0977, M, null, 3, false, false, No, "marker 3"),
        new(18, 0x0277, 0x1C68, 0x000B, C, 14, null, true, true, No, "robot phase"),
        new(19, 0x027C, 0x20B3, 0x0005, C, 15, null, true, true, WeekExitCheck.GameOver, "end check"),
        new(20, 0x0289, 0x0609, 0x1178, C, 16, null, false, false, No, "limits from production fields"),
        new(21, 0x0291, 0x11A3, 0x0000, C, 17, 1, false, true, No, "computer players before trade"),
        new(22, 0x029B, 0x21FD, 0x0977, M, null, 4, false, false, No, "marker 4"),
        new(23, 0x02A1, 0x0B9F, 0x0002, C, 18, null, true, true, No, "trade"),
        new(24, 0x02A6, 0x1DEF, 0x000C, C, 19, null, true, true, No, "extra phase"),
        new(25, 0x02AF, 0x21FD, 0x0977, M, null, 5, false, false, No, "marker 5"),
        new(26, 0x02B5, 0x1F63, 0x000F, C, 20, null, true, true, No, "Fat Mike"),
        new(27, 0x02BA, 0x08CF, 0x11D7, C, 21, null, false, true, No, "gem price of the week"),
        new(28, 0x02C3, 0x21FD, 0x0977, M, null, 6, false, false, No, "marker 6"),
        new(29, 0x02C9, 0x08CF, 0x0004, C, 22, null, true, true, No, "purchase"),
        new(30, 0x02CE, 0x1F63, 0x0B03, C, 23, null, true, null, No, "contest after the purchase"),
    ];

    /// <summary>Step with call number <paramref name="number"/> (1..30).</summary>
    public static WeekStep Step(int number) => Steps[number - 1];
}
