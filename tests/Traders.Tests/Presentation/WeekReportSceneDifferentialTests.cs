using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.State;
using Traders.Tests.Menus;

namespace Traders.Tests.Presentation;

// Drawing side covered by the scenarios of Week_report_scene_matches_original (scene against the original with live graphics):
// [DIFFERENTIAL] 0825:0072 (scene, through 0825:000A)
// [DIFFERENTIAL] 0825:0178 (scene, through 0825:000A)
// [DIFFERENTIAL] 0825:0364 (scene, through 0825:000A)
// [DIFFERENTIAL] 0825:047D (scene, through 0825:000A)
// [DIFFERENTIAL] 0825:0560 (scene, through 0825:000A)
// [DIFFERENTIAL] 0825:0686 (scene, through 0825:000A)
// [DIFFERENTIAL] 0825:09C9 (scene, through 0825:000A)
// [DIFFERENTIAL] 0609:1879 (scene, through 0825:0072)
// [DIFFERENTIAL] 21FD:0590 (scene, through 0825:09C9)

/// <summary>
/// [DIFFERENTIAL] 0825:000A (screen of the week report) – the report of a running game drawn by
/// <see cref="Traders.Presentation.Week.ReportScene"/> against the original with live graphics (<see cref="WeekReportSceneHarness"/>):
/// VRAM of all pages, DAC and CRTC start at every page flip, palette step, fade-in and at the end, for week 0, a later week
/// and states set to reach the branches of the drawing routines.
/// </summary>
public sealed class WeekReportSceneDifferentialTests
{
    private const byte Fire = 0x52;

    public static TheoryData<string> Scenarios => new(Table.Keys);

    private static (int, byte)[] Word(int at, int v) => [(at, (byte)v), (at + 1, (byte)(v >> 8))];

    private static (int, byte)[] Dword(int at, int v) => [.. Word(at, v), .. Word(at + 2, v >> 16)];

    /// <summary>
    /// Week index <paramref name="week"/> (45: ring buffer wrapped, week axis shifted) with a history that rises, falls below
    /// zero and clips. Week 41 puts a tens line at i = 26h, where 0825:047D draws no number (upper bound i ≤ 25h, R5B-2).
    /// </summary>
    private static (int, byte)[] LongHistory(int week = 45)
    {
        var set = new List<(int, byte)>(Word(DgroupLayout.Week, week));
        for (int p = 0; p < DgroupLayout.PlayerCount; p++)
            for (int i = 0; i < 40; i++)
                set.AddRange(Dword(DgroupLayout.WealthHistory + p * 0xA0 + i * 4, (i * 37 + p * 911) % 900 * 40 - 4000 * p));
        return [.. set];
    }

    /// <summary>Players 0 and 2 human (keyboard), confirming with fire at different frames.</summary>
    private static (int, byte)[] Humans() =>
    [
        (DgroupLayout.IsComputer + 0, 0), (DgroupLayout.IsComputer + 2, 0), (MenuLayout.AllComputers, 0),
        (ControlReader.ControlType + 0, 4), (ControlReader.ControlType + 2, 4),
    ];

    private static readonly Dictionary<string, WeekReportSceneHarness.Script> Table = new()
    {
        ["week 0"] = new(0, new KeyScript()),
        ["week 1"] = new(1, new KeyScript()),
        ["week 1 humans keypad"] = new(1, new KeyScript().Hold(5, 30, MenuKeys.KeypadPlus).Hold(12, 4, MenuKeys.KeypadMinus)
            .Hold(150, 2, Fire).Hold(260, 2, Fire), Humans()),
        ["long history"] = new(1, new KeyScript(), LongHistory()),
        ["long history week 41"] = new(1, new KeyScript(), LongHistory(41)),
        // 0825:0364 draws the target line only when DS:1584·120 reaches the target (unsigned)
        ["target line"] = new(0, new KeyScript(), Dword(DgroupLayout.Target, 500)),
        ["no target line"] = new(0, new KeyScript(), Dword(DgroupLayout.Target, 2_000_000_000)),
    };

    [GameDataTheory]
    [MemberData(nameof(Scenarios))]
    public void Week_report_scene_matches_original(string scenario) => WeekReportSceneHarness.AssertSame(Table[scenario], scenario);
}
