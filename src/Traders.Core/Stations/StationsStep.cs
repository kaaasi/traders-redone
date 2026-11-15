using Traders.Core.Field;
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
/// wired into the week loop by lane ernte): <see cref="Begin"/> runs <see cref="StationPhase.Run"/> up to its first wait,
/// every <see cref="Tick"/> one frame further (a retrace wait of the original or a busy poll of its pause 21FD:0245); the
/// step finishes in the frame in which 1B03:0009 returns. The machine writes the key table before each frame and keeps
/// DS:10D2 in sync. Every output call goes to <paramref name="output"/> when the core reports it, so each frame gets only
/// the calls of that frame (the scene, <c>Traders.Presentation.Stations.StationScene</c>, plays it with the data segment of
/// that moment and runs its retrace waits one per frame, <c>NextFrame</c> before the tick); the
/// picture load 186C:0002 is recorded in the picture table first. The rule page starts empty: the star field 1B03:1178 draws
/// the whole picture onto the draw page and copies it before the first read.
/// </summary>
/// <param name="inputs">Pixel inputs of the rule page.</param>
/// <param name="devices">Control devices for the human players' choice (0E9E:000E).</param>
/// <param name="output">Receiver of the output calls (scene, sound).</param>
/// <param name="files">Lengths of the picture files (as for the planting phase).</param>
// The data segment's page indices DS:082E/0830 and numbers DS:40DE must be the output's at the start (the host keeps them;
// lane ernte).
public sealed class StationsStep(StationsInputs inputs, IControlDevices devices, Action<StationEvent> output,
    Func<int, ImageFileLengths> files) : IWeekStepHandler
{
    private IEnumerator<FieldWait>? _run;

    /// <summary>The phase of the current call (diagnostics, tests).</summary>
    public StationPhase? Phase { get; private set; }

    /// <summary>Retrace waits of the current call (frames of the original).</summary>
    public int Retraces { get; private set; }

    /// <summary>Busy polls of the pause key of the current call.</summary>
    public int Polls { get; private set; }

    public StepOutcome Begin(StepContext context)
    {
        Retraces = Polls = 0;
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
        if (_run.Current == FieldWait.Retrace) Retraces++;
        else Polls++;
        return StepOutcome.Running;
    }
}
