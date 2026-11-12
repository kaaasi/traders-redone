namespace Traders.Core.Phases;

/// <summary>Kind of a step of the program flow in main 1777:000C.</summary>
public enum ProgramStepKind
{
    /// <summary>A far call of main into a routine; handled by an <see cref="IProgramStepHandler"/>.</summary>
    Call,

    /// <summary>The call of the week loop 1777:01F8; the machine runs it itself as a nested <see cref="PhaseMachine"/>.</summary>
    WeekLoop,
}

/// <summary>What main does with the result of a step (AL, for the initialisation the word in DI).</summary>
public enum ProgramResultCheck
{
    /// <summary>Always continue with the next step.</summary>
    None,

    /// <summary>Result not zero → teardown with that result as exit code (initialisation 1777:0120).</summary>
    EndIfNonZero,

    /// <summary>Result not zero → continue at <see cref="ProgramStep.JumpTo"/>, else with the next step (sound query 218B:013A).</summary>
    JumpIfNonZero,

    /// <summary>Result zero → teardown (main menu 1777:0168).</summary>
    EndIfZero,

    /// <summary>Result one → teardown, otherwise back to <see cref="ProgramStep.JumpTo"/> (week loop 1777:0184).</summary>
    EndIfOneElseJump,
}

/// <summary>Condition main tests before a step; a step whose condition is false is skipped.</summary>
public enum ProgramGuard
{
    None,

    /// <summary>Only if the menu result "new game" DS:3CCC is 1 (1777:016E); otherwise a loaded or continued game goes on.</summary>
    NewGameRequested,
}

/// <summary>
/// One of the ten far calls of main 1777:000C between the command line and the teardown, in program order.
/// <see cref="Arguments"/> are the C arguments in order (first argument first; pushed in reverse). The loop starts at step 5
/// (1777:013B); the teardown 1777:018C and the closing message are the host's.
/// </summary>
public sealed record ProgramStep(
    int Number,
    ushort Site,
    ushort TargetSegment,
    ushort TargetOffset,
    ProgramStepKind Kind,
    IReadOnlyList<int> Arguments,
    ProgramGuard Guard,
    ProgramResultCheck After,
    int? JumpTo,
    string Name)
{
    /// <summary>Return address in main (site + 5).</summary>
    public ushort ReturnSite => (ushort)(Site + 5);

    public override string ToString() =>
        $"{Number} 1777:{Site:X4} -> {TargetSegment:X4}:{TargetOffset:X4}{(Arguments.Count > 0 ? $"({string.Join(", ", Arguments)})" : "")} {Name}";
}

/// <summary>
/// The program flow of main 1777:000C as data (finding docs/routines/1777_000C_main.md): initialisation and program start
/// once, then the loop sound → main menu → new game (only on request) → DS:08FA = 1 → week loop. Week loop result 0 (exit A)
/// goes back to the loop start, result 1 (game over) and menu result 0 end the program.
/// </summary>
// [ORIGINAL] 1777:000C – order of the calls of main (sites 1777:0119..1777:018A), checks at 1777:0120, 0132, 0142, 0168, 016E, 0184
public static class ProgramLoop
{
    public const ushort Segment = 0x1777;
    public const ushort Entry = 0x000C;

    /// <summary>Site of <c>mov byte [DS:08FA], 1</c> before the week loop (executed after a new game and after a loaded one).</summary>
    public const ushort GameRunningSite = 0x017A;

    /// <summary>DS offset of the byte main sets to 1 before every week loop (meaning open; read by the menu 192D:11B8/12A6).</summary>
    public const int GameRunning = 0x08FA;

    /// <summary>DS offset of the menu result "new game" (set by 192D:12A6), tested at 1777:016E.</summary>
    public const int NewGameRequest = 0x3CCC;

    /// <summary>Teardown 1777:018C (music off, sound, system layer, video mode 3; then the closing message by exit code).</summary>
    public const ushort TeardownSite = 0x018C;

    /// <summary>Step number of the loop start 1777:013B.</summary>
    public const int LoopStart = 5;

    /// <summary>Step number of the main menu.</summary>
    public const int MainMenu = 8;

    /// <summary>Step number of the new game.</summary>
    public const int NewGame = 9;

    /// <summary>Step number of the week loop.</summary>
    public const int WeekLoopStep = 10;

    private const ProgramStepKind C = ProgramStepKind.Call;
    private const ProgramGuard G = ProgramGuard.None;
    private const ProgramResultCheck N = ProgramResultCheck.None;

    public static IReadOnlyList<ProgramStep> Steps { get; } =
    [
        new(1, 0x0119, 0x1777, 0x04E4, C, [], G, ProgramResultCheck.EndIfNonZero, null, "initialisation"),
        new(2, 0x0126, 0x0DFD, 0x000D, C, [], G, N, null, "program start"),
        new(3, 0x012B, 0x218B, 0x013A, C, [], G, ProgramResultCheck.JumpIfNonZero, 5, "music query"),
        new(4, 0x0136, 0x21A7, 0x000D, C, [], G, N, null, "select sound source"),
        new(5, 0x013B, 0x218B, 0x013A, C, [], G, ProgramResultCheck.JumpIfNonZero, MainMenu, "music query (loop start)"),
        new(6, 0x014D, 0x21A7, 0x0054, C, [0, 1], G, N, null, "sound effect"),
        new(7, 0x015C, 0x218B, 0x0094, C, [1, 2], G, N, null, "menu music"),
        new(8, 0x0163, 0x192D, 0x0007, C, [], G, ProgramResultCheck.EndIfZero, null, "main menu"),
        new(9, 0x0175, 0x0E31, 0x0003, C, [], ProgramGuard.NewGameRequested, N, null, "new game"),
        new(10, 0x017F, 0x1777, 0x01F8, ProgramStepKind.WeekLoop, [], G, ProgramResultCheck.EndIfOneElseJump, LoopStart, "week loop"),
    ];

    /// <summary>Step with number <paramref name="number"/> (1..10).</summary>
    public static ProgramStep Step(int number) => Steps[number - 1];
}
