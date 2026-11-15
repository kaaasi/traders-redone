using Traders.Core.Events;
using Traders.Core.Menus;
using Traders.Core.State;
using Traders.Tests.Events;
using Traders.Tests.Menus;

namespace Traders.Tests.Presentation;

// Drawing side covered by the scenarios of Week_incidents_scene_matches_original (scene against the original with live graphics):
// [DIFFERENTIAL] 17CB:0986 (scene: window, through 17CB:0005)
// [DIFFERENTIAL] 17CB:09E0 (scene: typewriter text, through 17CB:0005)
// [DIFFERENTIAL] 0A69:0288 (scene and core frames, through 17CB:09E0)

/// <summary>
/// [DIFFERENTIAL] 17CB:0005 (screen of the week incidents) – the incidents of a running game drawn by
/// <see cref="Traders.Presentation.Events.IncidentScene"/> against the original with live graphics (<see cref="EventsSceneHarness"/>):
/// VRAM of all pages, DAC, CRTC start, retraces and frame at every fade, page flip and at the end – per character of the
/// typewriter and every frame of the wait loop – for the incident of week 2 of the default game and states set to reach each
/// kind of incident (scenarios of <see cref="IncidentsDifferentialTests"/> applied to the running game).
/// </summary>
public sealed class EventsSceneDifferentialTests
{
    private const byte Fire = 0x52, PauseKey = 0x19;

    private static Action<Dgroup> Scenario(string name) =>
        d => IncidentsDifferentialTests.Apply(d, IncidentsDifferentialTests.Get(name));

    /// <summary>Scenario, the kind of incident it must show (text index of the first line) and the script.</summary>
    private static readonly Dictionary<string, (int? Text, EventsSceneHarness.Script Script)> Table = new()
    {
        ["week 0 nothing"] = (null, new(0, new KeyScript())),
        ["week 2 default game"] = (-1, new(2, new KeyScript())),
        ["takeover"] = (IncidentLayout.TakeoverText, new(2, new KeyScript(), Scenario("takeover"))),
        ["takeover humans keypad"] = (IncidentLayout.TakeoverText, new(2,
            new KeyScript().Hold(150, 40, MenuKeys.KeypadPlus).Hold(200, 30, MenuKeys.KeypadMinus).Hold(700, 2, Fire)
                .Hold(820, 2, Fire).Hold(900, 3, Fire), Scenario("takeover humans keypad"))),
        ["loss capped 3ED"] = (-1, new(2, new KeyScript(), Scenario("loss capped 3ED"))),
        ["gain class 3 pause"] = (-1, new(2, new KeyScript().Hold(260, 3, PauseKey).Hold(300, 3, PauseKey),
            Scenario("gain class 3 pause and abort"))),
        ["blight and gain"] = (IncidentLayout.BlightText, new(2, new KeyScript(), Scenario("blight and gain"))),
    };

    public static TheoryData<string> Scenarios => new(Table.Keys);

    [GameDataTheory]
    [MemberData(nameof(Scenarios))]
    public void Week_incidents_scene_matches_original(string scenario)
    {
        var (text, script) = Table[scenario];
        var events = EventsSceneHarness.AssertSame(script, scenario);
        var texts = events.Where(e => e.Kind == IncidentEventKind.Text).Select(e => e.A).ToList();
        if (text is null) Assert.Empty(texts);
        else if (text == -1) Assert.NotEmpty(texts);
        else Assert.Contains(text.Value, texts);
        if (text is not null) Assert.Contains(events, e => e.Kind == IncidentEventKind.Character);
    }
}
