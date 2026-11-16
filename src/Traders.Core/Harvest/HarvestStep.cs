using Traders.Core.Field;
using Traders.Core.Phases;
using Traders.Core.Startup;
using Traders.Core.State;

namespace Traders.Core.Harvest;

/// <summary>
/// Call 16 of the week loop 1777:01F8, the harvest 22A3:02D3, as a step of the phase machine: <see cref="Begin"/> runs
/// <see cref="Harvest.Run"/> up to its first wait, every <see cref="Tick"/> one frame further (a retrace or a poll of the
/// pause loop); the step finishes in the frame in which 22A3:02D3 returns. The picture load 186C:0002 is recorded in the
/// picture table (<see cref="ImageTable.RecordLoad"/>) when it is emitted; all other events go to <paramref name="host"/>
/// unchanged. Bound into the machine by part 3 of lane ernte.
/// </summary>
/// <param name="host">Controls, pause, texts and the outputs (scene, sound).</param>
/// <param name="files">Lengths of the picture files (as for the main menu).</param>
public sealed class HarvestStep(IHarvestHost host, Func<int, ImageFileLengths> files) : IWeekStepHandler
{
    private IEnumerator<FieldWait>? _run;

    /// <summary>Frames waited so far, by kind (diagnostics).</summary>
    public int Retraces { get; private set; }

    public int Polls { get; private set; }

    public StepOutcome Begin(StepContext context)
    {
        _run = Harvest.Run(context.State, context.Random, new LoadRecorder(host, context.State, files)).GetEnumerator();
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
    private sealed class LoadRecorder(IHarvestHost host, Dgroup state, Func<int, ImageFileLengths> files) : IHarvestHost
    {
        public void Emit(FieldEvent e)
        {
            // [ORIGINAL] 186C:0002 – the picture table entry and DS:09C8 (ImageTable.RecordLoad); unpacking is the output's
            if (e.Kind == FieldEventKind.LoadImage) ImageTable.RecordLoad(state, e.Args[0], files);
            host.Emit(e);
        }

        public void Emit(HarvestEvent e) => host.Emit(e);
        public void ReadControls(Dgroup d) => host.ReadControls(d);
        public IEnumerable<FieldWait> PauseCheck(Dgroup d) => host.PauseCheck(d);
        public byte[] FarString(ushort offset, ushort segment) => host.FarString(offset, segment);
    }
}
