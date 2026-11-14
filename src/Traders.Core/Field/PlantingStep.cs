using Traders.Core.Phases;
using Traders.Core.Startup;
using Traders.Core.State;

namespace Traders.Core.Field;

/// <summary>
/// Call 8 of the week loop 1777:01F8, the planting phase 0609:0004, as a step of the phase machine: <see cref="Begin"/> runs
/// <see cref="Planting.Run"/> up to its first wait, every <see cref="Tick"/> one frame further (a retrace or a poll of the
/// pause loop); the step finishes in the frame in which 0609:0004 returns. The machine writes the key table before each frame
/// and keeps DS:10D2 in sync. The picture load 186C:0002 is recorded in the picture table (<see cref="ImageTable.RecordLoad"/>)
/// when it is emitted; all other events go to <paramref name="host"/> unchanged.
/// </summary>
/// <param name="host">Helpers and hooks of the other lanes, controls and the outputs (scene, sound).</param>
/// <param name="files">Lengths of the picture files (as for the main menu).</param>
public sealed class PlantingStep(IPlantingHost host, Func<int, ImageFileLengths> files) : IWeekStepHandler
{
    private IEnumerator<FieldWait>? _run;

    /// <summary>Frames waited so far, by kind (diagnostics).</summary>
    public int Retraces { get; private set; }

    public int Polls { get; private set; }

    public StepOutcome Begin(StepContext context)
    {
        _run = Planting.Run(context.State, context.Random, new LoadRecorder(host, context.State, files)).GetEnumerator();
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

    /// <summary>Passes everything to the host; a picture load is first recorded in the picture table.</summary>
    private sealed class LoadRecorder(IPlantingHost host, Dgroup state, Func<int, ImageFileLengths> files) : IPlantingHost
    {
        public void Emit(FieldEvent e)
        {
            // [ORIGINAL] 186C:0002 – the picture table entry and DS:09C8 (ImageTable.RecordLoad); unpacking is the output's
            if (e.Kind == FieldEventKind.LoadImage) ImageTable.RecordLoad(state, e.Args[0], files);
            host.Emit(e);
        }

        public void ComputerTurn(Dgroup d, short x, short y) => host.ComputerTurn(d, x, y);
        public bool CanPlaceHq(Dgroup d) => host.CanPlaceHq(d);
        public bool HasOwnNeighbour(Dgroup d, short x, short y) => host.HasOwnNeighbour(d, x, y);
        public void ComputerPlacedHq(Dgroup d) => host.ComputerPlacedHq(d);
        public void ComputerBoughtField(Dgroup d) => host.ComputerBoughtField(d);
        public void ReadControls(Dgroup d) => host.ReadControls(d);
        public IEnumerable<FieldWait> PauseCheck(Dgroup d) => host.PauseCheck(d);
        public IEnumerable<FieldWait> WaitLoop(Dgroup d, OriginalRandom random, short mode) => host.WaitLoop(d, random, mode);
    }
}
