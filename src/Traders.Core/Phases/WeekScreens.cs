using Traders.Core.Disasters;
using Traders.Core.Events;
using Traders.Core.Field;
using Traders.Core.Harvest;
using Traders.Core.Input;
using Traders.Core.Menus;
using Traders.Core.Pixels;
using Traders.Core.Startup;
using Traders.Core.State;
using Traders.Core.Stations;

namespace Traders.Core.Phases;

/// <summary>
/// Explicit inputs of the screen steps of round 6 (calls 4, 9, 11, 16), filled by the host and the tests from Traders.Assets
/// and the program start: the core reads no file and no output state itself (AGENTS.md §4, R6P-1).
/// </summary>
/// <param name="Files">Lengths of the picture files (picture table of 186C:0002).</param>
/// <param name="Pictures">Pixels of a picture by its number (V-BERGE, V-ANBAU, V-RAKETE …) for the rule pages.</param>
/// <param name="Masks">MASKS.PP decrunched (objects of the stations phase on its rule page).</param>
/// <param name="Font">The graphics driver's current font when call 11 starts (the stations phase types into its panel).</param>
/// <param name="TextBuffer">
/// The text buffer behind DS:4884 after the program start (null before); 2295:0001 of the incidents writes amounts into it,
/// so all steps share this one buffer.
/// </param>
/// <param name="Devices">Control devices besides the key table; keyboard only if null.</param>
/// <param name="OutputPages">
/// The output's data segment image (the graphics layers' own) when a call starts, or null when the core's data segment already
/// holds the output's page state (a test that takes over the original's data segment at the call site). The page state – page
/// indices DS:082E/0830, page numbers DS:40DE…40E5 and the draw page number DS:4516 – belongs to the output layers (21FD:051F,
/// 0590); the rule pages of calls 4 and 11 start on the draw page it names (R6P-1), so it is an input like the key table and
/// the core mirrors it into its data segment at the start of calls 4, 9 and 11 (the harvest, call 16, reads none of it).
/// </param>
public sealed record WeekScreenInputs(
    Func<int, ImageFileLengths> Files,
    Func<int, IndexedImage> Pictures,
    byte[] Masks,
    Func<TextFont> Font,
    Func<byte[]?> TextBuffer,
    IControlDevices? Devices = null,
    Func<ReadOnlyMemory<byte>>? OutputPages = null);

/// <summary>Receivers of the output events of the screen steps of round 6 (scenes, sound); the incidents are drained.</summary>
public sealed record WeekScreenOutputs(
    Action<DisasterEvent> Disasters,
    Action<StationEvent> Stations,
    Action<FieldEvent> HarvestField,
    Action<HarvestEvent> Harvest);

/// <summary>
/// The handlers of the screen steps of round 6 for <see cref="WeekSteps"/> (lane ernte part 3): call 4 beyond its guard
/// (<see cref="CatastropheStep"/> of lane katastrophen behind <see cref="CatastropheGuardStep"/>), call 9
/// (<see cref="WeekIncidentsStep"/> of lane ereignisse), call 11 (<see cref="StationsStep"/> of lane stationen) and call 16
/// (<see cref="HarvestStep"/>). The steps whose inputs depend on the moment of the call – the rule page of 111D:0009 on the
/// draw page DS:40DE[DS:082E] of that moment, the stations' picture DS:08D0, font and text buffer – are made anew at every
/// call (<see cref="DeferredStep"/>).
/// </summary>
public sealed class WeekScreens
{
    public WeekScreens(WeekScreenInputs inputs, WeekScreenOutputs outputs)
    {
        var devices = inputs.Devices ?? KeyboardOnlyDevices.Instance;
        byte[] Texts() => inputs.TextBuffer() ?? throw new InvalidOperationException("Text buffer used before the program start.");
        StepContext Pages(StepContext c)
        {
            if (inputs.OutputPages?.Invoke() is { } output) MirrorPages(c.State, output.Span);
            return c;
        }

        Catastrophes = new DeferredStep(c => new CatastropheStep(new DisasterHost(outputs.Disasters, devices), inputs.Files,
            new RulePage(RulePage.DrawPageOf(Pages(c).State)), inputs.Pictures, new DisasterTexts(Texts())));
        Incidents = new DeferredStep(c => new WeekIncidentsStep(new TextBufferMemory(Pages(c).State, inputs.TextBuffer), devices));
        Stations = new DeferredStep(c => new StationsStep(
            new StationsInputs(inputs.Pictures(Pages(c).State.S16(StationsLayout.Picture)), inputs.Masks, inputs.Font(),
                new TextFile(Texts(), c.State.U16(MenuLayout.TextBuffer), c.State.U16(MenuLayout.TextBuffer + 2))),
            devices, outputs.Stations, inputs.Files));
        Harvest = new DeferredStep(c => new HarvestStep(new HarvestHost(outputs.HarvestField, outputs.Harvest,
            (offset, segment) => FarString(c.State, Texts(), offset, segment), devices), inputs.Files));
    }

    /// <summary>Rest of call 4 behind its guard (a <see cref="CatastropheStep"/> per call).</summary>
    public DeferredStep Catastrophes { get; }

    /// <summary>Call 9 (a <see cref="WeekIncidentsStep"/> per call); its events: <see cref="DrainIncidentEvents"/>.</summary>
    public DeferredStep Incidents { get; }

    /// <summary>Call 11 (a <see cref="StationsStep"/> per call).</summary>
    public DeferredStep Stations { get; }

    /// <summary>Call 16 (a <see cref="HarvestStep"/> per call).</summary>
    public DeferredStep Harvest { get; }

    /// <summary>Events of the running or last week incidents since the last drain (empty before the first call).</summary>
    public List<IncidentEvent> DrainIncidentEvents() =>
        (Incidents.Current as WeekIncidentsStep)?.Incidents?.DrainEvents() ?? [];

    /// <summary>
    /// The week steps with these handlers bound: everything up to call 16 runs in the core, call 18 (robot phase) is the
    /// first pending call unless <paramref name="robots"/> stands in for it; then the week runs on through the end check
    /// (call 19) to the trade (call 23).
    /// </summary>
    public WeekSteps Week(Action<FieldEvent> planting, Func<int, ImageFileLengths> files, IControlDevices? devices = null,
        IWeekStepHandler? robots = null) =>
        new(planting, files, devices, Catastrophes, Incidents, Stations, Harvest, robots);

    /// <summary>Bytes of the output's page state: indices DS:082E/0830, numbers DS:40DE…40E5, draw page number DS:4516.</summary>
    public static IReadOnlyList<(int Offset, int Length)> PageState { get; } =
        [(RulePage.DrawPageIndex, 4), (RulePage.PageNumbers, 8), (0x4516, 2)];

    /// <summary>Copies the output's page state (<see cref="PageState"/>) into the core's data segment.</summary>
    // [APPROX] Host input: the page state is the output layers' (21FD:051F/0590 run in Traders.Vga); the core takes it at the
    // start of calls 4, 9 and 11 instead of following every flip of the earlier steps itself.
    public static void MirrorPages(Dgroup d, ReadOnlySpan<byte> output)
    {
        foreach (var (offset, length) in PageState)
            for (int i = 0; i < length; i++) d.Set(offset + i, output[offset + i]);
    }

    /// <summary>The zero-terminated far text at <paramref name="segment"/>:<paramref name="offset"/> in the text buffer.</summary>
    private static byte[] FarString(Dgroup d, byte[] buffer, ushort offset, ushort segment)
    {
        int start = (segment - d.U16(MenuLayout.TextBuffer + 2)) * 16 + offset - d.U16(MenuLayout.TextBuffer);
        if (start < 0 || start >= buffer.Length)
            throw new InvalidOperationException($"text {segment:X4}:{offset:X4} lies outside the text buffer");
        int end = Array.IndexOf(buffer, (byte)0, start);
        if (end < 0) throw new InvalidOperationException($"text {segment:X4}:{offset:X4} is not terminated in the buffer");
        return buffer[start..end];
    }
}
