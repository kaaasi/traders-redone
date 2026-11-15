using Traders.Core.Input;
using Traders.Core.Phases;
using Traders.Core.Pixels;
using Traders.Core.Startup;
using Traders.Core.State;

namespace Traders.Core.Stations;

/// <summary>
/// Pixel inputs of the stations phase for its rule page (R6P-1), filled by the host and the tests from Traders.Assets: the
/// picture V-RAKETE (DS:08D0) as the RAM atlas, MASKS.PP decrunched, the driver's current font and the text buffer DS:4884.
/// </summary>
public sealed record StationsInputs(IndexedImage Atlas, byte[] Masks, TextFont Font, TextFile Texts);

/// <summary>
/// Call 11 of the week loop 1777:01F8, the stations phase 1B03:0009, as a step of the phase machine (lane stationen part 3;
/// wired into the week loop by lane ernte): <see cref="Begin"/> runs <see cref="StationPhase.Run"/> up to its first page
/// flip, every <see cref="Tick"/> one flip further; the step finishes when 1B03:0009 returns. The machine writes the key table
/// before each tick and keeps DS:10D2 in sync. Every output call goes to <paramref name="output"/> when the core reports it
/// (the scene, <c>Traders.Presentation.Stations.StationScene</c>, plays it with the data segment of that moment); the
/// picture load 186C:0002 is recorded in the picture table first. The rule page starts empty: the star field 1B03:1178 draws
/// the whole picture onto the draw page and copies it before the first read.
/// </summary>
/// <param name="inputs">Pixel inputs of the rule page.</param>
/// <param name="devices">Control devices for the human players' choice (0E9E:000E).</param>
/// <param name="output">Receiver of the output calls (scene, sound).</param>
/// <param name="files">Lengths of the picture files (as for the planting phase).</param>
// [APPROX] One tick per page flip of 1B03 (choice loop 02C0, flight loop 08ED); the retrace waits of the output (21FD:0185,
// fades, the flips inside 21FD:060D/0665 and the typed texts) are not frames of the phase machine. The data segment's page
// indices DS:082E/0830 and numbers DS:40DE must be the output's at the start (the host keeps them; lane ernte).
public sealed class StationsStep(StationsInputs inputs, IControlDevices devices, Action<StationEvent> output,
    Func<int, ImageFileLengths> files) : IWeekStepHandler
{
    private IEnumerator<StationWait>? _run;

    /// <summary>The phase of the current call (diagnostics, tests).</summary>
    public StationPhase? Phase { get; private set; }

    /// <summary>Page flips so far.</summary>
    public int Flips { get; private set; }

    public StepOutcome Begin(StepContext context)
    {
        var d = context.State;
        var page = new RulePage(RulePage.DrawPageOf(d));
        Phase = new StationPhase(d, context.Random, devices)
        {
            Pixels = new StationsPage(page, inputs.Atlas, inputs.Masks, inputs.Font, inputs.Texts, d),
            Output = e =>
            {
                // [ORIGINAL] 186C:0002 – the picture table entry and DS:09C8 (ImageTable.RecordLoad); unpacking is the output's
                if (e.Call == StationCall.LoadPicture) ImageTable.RecordLoad(d, e.Args[0], files);
                output(e);
            },
        };
        _run = Phase.Run().GetEnumerator();
        return Next();
    }

    public StepOutcome Tick(StepContext context)
    {
        if (_run is null) throw new InvalidOperationException("Not started.");
        return Next();
    }

    private StepOutcome Next()
    {
        if (!_run!.MoveNext())
        {
            _run.Dispose();
            _run = null;
            return StepOutcome.Finished();
        }
        Flips++;
        return StepOutcome.Running;
    }
}
