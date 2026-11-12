using Traders.Core.Menus;
using Traders.Core.State;
using Traders.Tests.Menus;

namespace Traders.Tests.Presentation;

// Drawing side covered by the scenarios of Menu_scene_matches_original (scene against the original with live graphics):
// [DIFFERENTIAL] 192D:03AB (scene, through 192D:0007)
// [DIFFERENTIAL] 192D:042F (scene, through 192D:0007)
// [DIFFERENTIAL] 192D:0653 (scene, through 192D:0007)
// [DIFFERENTIAL] 192D:076C (scene, through 192D:0007)
// [DIFFERENTIAL] 192D:07AC (scene, through 192D:0007)
// [DIFFERENTIAL] 192D:07FC (scene, through 192D:0007)
// [DIFFERENTIAL] 192D:084A (scene, through 192D:0007)
// [DIFFERENTIAL] 192D:0899 (scene, through 192D:0007)
// [DIFFERENTIAL] 192D:08E7 (scene, through 192D:0007)
// [DIFFERENTIAL] 192D:0935 (scene, through 192D:0007)
// [DIFFERENTIAL] 192D:09B0 (scene, through 192D:0007)
// [DIFFERENTIAL] 192D:0A26 (scene, through 192D:0007)
// [DIFFERENTIAL] 192D:0DC0 (scene, through 192D:0007)
// [DIFFERENTIAL] 192D:0E11 (scene, through 192D:0007)
// [DIFFERENTIAL] 192D:0E64 (scene, through 192D:0007)
// [DIFFERENTIAL] 192D:0E8E (scene, through 192D:0007)
// [DIFFERENTIAL] 192D:0EAE (scene, through 192D:0007)
// [DIFFERENTIAL] 192D:0ED9 (scene, through 192D:0007)
// [DIFFERENTIAL] 192D:0EF8 (scene, through 192D:0007)
// [DIFFERENTIAL] 192D:0F16 (scene, through 192D:0007)
// [DIFFERENTIAL] 192D:0F6E (scene, through 192D:0007)
// [DIFFERENTIAL] 192D:0FB7 (scene, through 192D:0007)
// [DIFFERENTIAL] 192D:120C (scene, through 192D:0007)
// [DIFFERENTIAL] 192D:16AF (scene, through 192D:0007)
// [DIFFERENTIAL] 192D:17F9 (scene, through 192D:0007)
// [DIFFERENTIAL] 192D:1855 (scene, through 192D:0007)
// [DIFFERENTIAL] 192D:19A4 (scene, through 192D:0007)
// [DIFFERENTIAL] 192D:1B06 (scene, through 192D:0007)
// [DIFFERENTIAL] 192D:1B90 (scene, through 192D:0007)
// [DIFFERENTIAL] 21FD:05DB (scene, through 192D:0007)

/// <summary>
/// [DIFFERENTIAL] 192D:00C5 (screens and redraws of the menu scene) – every menu of 192D:0007 drawn by <see cref="Traders.Presentation.Menus.MenuScene"/>
/// against the original with live graphics (<see cref="MenuSceneHarness"/>): VRAM of all pages, DAC and CRTC start at every
/// flip, fade and at the end.
/// </summary>
public sealed class MenuSceneDifferentialTests
{
    private const long Start = 60, Gap = 90, Hold = 3;

    private static MenuSceneHarness.Script Keys(params byte[] keys) => new(KeyScript.Sequence(Start, Gap, Hold, keys), [], [], []);

    public static TheoryData<string> Scenarios => new(Table.Keys);

    private static readonly Dictionary<string, MenuSceneHarness.Script> Table = new()
    {
        ["main-enter"] = Keys(MenuKeys.Enter),
        ["players"] = Keys(MenuKeys.F1, MenuKeys.F2, MenuKeys.F3, MenuKeys.F4, MenuKeys.F1, MenuKeys.Q),
        ["level"] = Keys(MenuKeys.F6, MenuKeys.F1, MenuKeys.F1, MenuKeys.F1, MenuKeys.F1, MenuKeys.F5, MenuKeys.F6, MenuKeys.F7,
            MenuKeys.F8, MenuKeys.F9, MenuKeys.F10, MenuKeys.F10, MenuKeys.F5, MenuKeys.F1, MenuKeys.F1, MenuKeys.Enter, MenuKeys.Q),
        ["settings"] = Keys(MenuKeys.F7, MenuKeys.F1, MenuKeys.F2, MenuKeys.F3, MenuKeys.F4, MenuKeys.F5, MenuKeys.F6, MenuKeys.Left,
            MenuKeys.Right, MenuKeys.Right, MenuKeys.Up, MenuKeys.Down, MenuKeys.S, MenuKeys.Enter, MenuKeys.Q),
        ["disk"] = new(KeyScript.Sequence(Start, Gap, Hold, MenuKeys.F8, MenuKeys.S, MenuKeys.L, MenuKeys.S, MenuKeys.L, MenuKeys.S,
            MenuKeys.S, MenuKeys.Enter, MenuKeys.Enter, MenuKeys.N), [false, true], [true, false], []),
        ["info"] = Keys(MenuKeys.F9, MenuKeys.Enter, MenuKeys.Q),
        ["controls-calibration"] = new(KeyScript.Sequence(Start, 2 * Gap, Hold, MenuKeys.F1, MenuKeys.F2, MenuKeys.Enter,
            MenuKeys.F1, MenuKeys.F2, MenuKeys.F2, MenuKeys.F3, MenuKeys.Enter, MenuKeys.Enter), [], [],
            [false, true, false, false, false, false, false, false],
            // player 1 joystick 1 after one F1 (device 5 → 0), player 2 joystick 2 after two F2 (5 → 0 → 1)
            [(DgroupLayout.ControlDevice + 0, 5), (DgroupLayout.ControlDevice + 1, 5)]),
        ["continue"] = new(KeyScript.Sequence(Start, Gap, Hold, MenuKeys.F8, MenuKeys.L, MenuKeys.Enter, MenuKeys.Enter, MenuKeys.A),
            [true], [], []),
        ["keypad-minus"] = new(new KeyScript().Hold(Start, 40, MenuKeys.KeypadMinus).Hold(Start + 10, 3, MenuKeys.F1)
            .Hold(Start + 100, 3, MenuKeys.Enter).Hold(Start + 200, 3, MenuKeys.Enter), [], [], []),
    };

    [GameDataTheory]
    [MemberData(nameof(Scenarios))]
    public void Menu_scene_matches_original(string scenario) => MenuSceneHarness.AssertSame(Table[scenario], scenario);
}
