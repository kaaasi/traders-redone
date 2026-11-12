using Traders.Core.Menus;
using Traders.Core.State;
using Press = Traders.Tests.Presentation.MenuFrameHarness.Press;

namespace Traders.Tests.Presentation;

/// <summary>
/// [DIFFERENTIAL] 192D:0234 (frame and random sequence of the main menu, #86) – the main menu with human key input (real
/// keyboard interrupts with dwell and hold times) against the original retrace by retrace (<see cref="MenuFrameHarness"/>):
/// VRAM, DAC, CRTC start, DS:10D2 and the number of random() calls at every retrace equal; the scene's retrace waits in the
/// frames the core waits; menu outputs and AL equal at the end.
/// </summary>
// [DIFFERENTIAL] 192D:0495 (frames of the level loop)
// [DIFFERENTIAL] 192D:1A04 (frames of the control loop)
// [DIFFERENTIAL] 21FD:051F (two retraces per flip, keypad plus and minus)
// [DIFFERENTIAL] 21FD:0245 (pause)
public sealed class MenuFrameDifferentialTests
{
    /// <summary>Real frames until the main menu is on screen in the original (picture load and screen build at 3 MIPS take about 110).</summary>
    private const long Ready = 150;

    private static Press At(long from, long frames, byte key) => new(Ready + from, frames, key);

    /// <summary>Keys one after another: first at <paramref name="start"/>, <paramref name="gap"/> frames apart, each held <paramref name="hold"/> frames.</summary>
    private static Press[] Sequence(long start, long gap, long hold, params byte[] keys) =>
        keys.Select((k, i) => At(start + i * gap, hold, k)).ToArray();

    private static MenuFrameHarness.Script Keys(Press[] presses) => new(presses, [], [], []);

    public static TheoryData<string> Scenarios => new(Table.Keys);

    private static readonly Dictionary<string, MenuFrameHarness.Script> Table = new()
    {
        // dwell in the main menu (random(1) every pass), then RETURN
        ["idle-enter"] = Keys([At(150, 4, MenuKeys.Enter)]),
        // players with different holds and dwell times, Q
        ["players"] = Keys([At(80, 2, MenuKeys.F1), At(95, 9, MenuKeys.F2), At(140, 1, MenuKeys.F3),
            At(141 + 30, 5, MenuKeys.F4), At(230, 3, MenuKeys.F1), At(300, 3, MenuKeys.Q)]),
        ["level"] = Keys(Sequence(80, 70, 3, MenuKeys.F6, MenuKeys.F1, MenuKeys.F1, MenuKeys.F1, MenuKeys.F1, MenuKeys.F5,
            MenuKeys.F10, MenuKeys.F1, MenuKeys.Enter, MenuKeys.Q)),
        ["settings"] = Keys(Sequence(80, 70, 4, MenuKeys.F7, MenuKeys.F1, MenuKeys.F2, MenuKeys.F3, MenuKeys.F4, MenuKeys.F5,
            MenuKeys.F6, MenuKeys.Left, MenuKeys.Down, MenuKeys.S, MenuKeys.Enter, MenuKeys.Q)),
        // pause (P: release, press, release) between two passes; F1 makes player 1 human, so RETURN opens the control menu
        ["pause"] = Keys([At(80, 3, MenuKeys.P), At(140, 6, MenuKeys.P), At(200, 3, MenuKeys.F1),
            At(260, 3, MenuKeys.Enter), At(400, 3, MenuKeys.Enter)]),
        // keypad minus: four more retraces per flip; keypad plus: one fewer
        ["keypad"] = Keys([At(60, 120, MenuKeys.KeypadMinus), At(100, 3, MenuKeys.F1), At(250, 120, MenuKeys.KeypadPlus),
            At(290, 3, MenuKeys.F2), At(450, 3, MenuKeys.Enter), At(600, 3, MenuKeys.Enter)]),
        // disk menu: load succeeds, back, RETURN → continue dialog (slow fade-in 0329), A
        ["disk-continue"] = new(Sequence(80, 90, 3, MenuKeys.F8, MenuKeys.L, MenuKeys.Enter, MenuKeys.Enter, MenuKeys.A), [true], [], []),
        ["info"] = Keys(Sequence(80, 90, 3, MenuKeys.F9, MenuKeys.Enter, MenuKeys.Q)),
        // control menu (192D:1A04, random(1) per pass) with two joystick calibrations, the first one failing
        ["controls"] = new(Sequence(80, 150, 3, MenuKeys.F1, MenuKeys.F2, MenuKeys.Enter, MenuKeys.F1, MenuKeys.F2, MenuKeys.F2,
            MenuKeys.F3, MenuKeys.Enter, MenuKeys.Enter), [], [], [false, true, false, false, false, false, false, false],
            [(DgroupLayout.ControlDevice + 0, 5), (DgroupLayout.ControlDevice + 1, 5)]),
    };

    [GameDataTheory]
    [MemberData(nameof(Scenarios))]
    public void Menu_frames_match_original(string scenario) => MenuFrameHarness.AssertSame(Table[scenario], scenario);

    /// <summary>Counter-test: one host frame that runs two core frames is caught.</summary>
    [GameDataFact]
    public void Doubled_frame_is_detected()
    {
        var e = Assert.ThrowsAny<Xunit.Sdk.XunitException>(() => MenuFrameHarness.AssertSame(Table["idle-enter"], "doubled", doubleFrame: 30));
        Console.WriteLine(e.Message);
    }
}
