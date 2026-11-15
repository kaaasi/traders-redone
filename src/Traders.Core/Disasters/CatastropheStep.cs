using Traders.Core.Field;
using Traders.Core.Phases;
using Traders.Core.Pixels;
using Traders.Core.Startup;
using Traders.Core.State;

namespace Traders.Core.Disasters;

/// <summary>
/// The rest of call 4 of the week loop, 111D:0009 after its guard, as the handler behind
/// <see cref="CatastropheGuardStep"/> (contract <see cref="IWeekStepHandler"/>, the guard calls it only when 111D:0009 goes
/// on to 001E): <see cref="Begin"/> runs <see cref="Catastrophes.Run"/> up to its first wait, every <see cref="Tick"/> one
/// frame further; the step finishes in the frame in which 111D:0009 returns (at once when no catastrophe strikes). Picture
/// loads 186C:0002 are recorded in the picture table (<see cref="ImageTable.RecordLoad"/>) when they are emitted; all
/// other events go to <paramref name="host"/> unchanged. <paramref name="page"/> is the core's rule page (its draw page
/// must be the original's when the step begins, <see cref="RulePage.DrawPageOf"/>); <paramref name="images"/> gives the
/// pixels of a picture by number (host and tests from Traders.Assets), <paramref name="texts"/> the text buffer for the typed
/// lines. Bound into the phase machine by lane ernte (part 3).
/// </summary>
public sealed class CatastropheStep(
    IDisasterHost host, Func<int, ImageFileLengths> files, RulePage page, Func<int, IndexedImage> images, DisasterTexts texts)
    : IWeekStepHandler
{
    private IEnumerator<FieldWait>? _run;

    /// <summary>Retrace waits of the last run (frames of the original).</summary>
    public int Retraces { get; private set; }

    /// <summary>Busy polls of the pause key of the last run.</summary>
    public int Polls { get; private set; }

    public StepOutcome Begin(StepContext context)
    {
        Retraces = Polls = 0;
        _run = Catastrophes.Run(
            context.State, context.Random, new LoadRecorder(host, context.State, files), page, images, texts).GetEnumerator();
        return Next();
    }

    public StepOutcome Tick(StepContext context)
    {
        if (_run is null) throw new InvalidOperationException("Not started.");
        return Next();
    }

    private StepOutcome Next()
    {
        if (_run!.MoveNext())
        {
            if (_run.Current == FieldWait.Retrace) Retraces++;
            else Polls++;
            return StepOutcome.Running;
        }
        _run.Dispose();
        _run = null;
        return StepOutcome.Finished();
    }

    private sealed class LoadRecorder(IDisasterHost host, Dgroup state, Func<int, ImageFileLengths> files) : IDisasterHost
    {
        public void Emit(DisasterEvent e)
        {
            // [ORIGINAL] 186C:0002 – the picture table entry and DS:09C8 (ImageTable.RecordLoad); unpacking is the output's
            if (e.Kind == DisasterEventKind.LoadImage) ImageTable.RecordLoad(state, e.Args[0], files);
            host.Emit(e);
        }

        public IEnumerable<FieldWait> PauseCheck(Dgroup d) => host.PauseCheck(d);

        public IEnumerable<FieldWait> WaitLoop(Dgroup d, OriginalRandom random, short mode) => host.WaitLoop(d, random, mode);
    }
}
