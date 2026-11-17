using Traders.Core.Field;
using Traders.Core.Phases;
using Traders.Core.Startup;
using Traders.Core.State;

namespace Traders.Core.FatMike;

/// <summary>
/// Call 26 of the week loop 1777:01F8, Fat Mike 1F63:000F, as a step of the phase machine: <see cref="Begin"/> runs
/// <see cref="FatMikePhase.Run"/> up to its first wait, every <see cref="Tick"/> one frame further (a retrace or a poll of the
/// pause loop); the step finishes in the frame in which 1F63:000F returns. The picture loads 186C:0002 are recorded in the
/// picture table (<see cref="ImageTable.RecordLoad"/>) when they are emitted; all other events go to <paramref name="host"/>
/// unchanged. Bound into the machine by lane woche (docs/arbeitspakete.md R8).
/// </summary>
/// <param name="host">Controls, pause, wait loop, texts and the outputs (scene, sound).</param>
/// <param name="files">Lengths of the picture files (as for the main menu).</param>
public sealed class FatMikeStep(IFatMikeHost host, Func<int, ImageFileLengths> files) : IWeekStepHandler
{
    private IEnumerator<FieldWait>? _run;

    /// <summary>Frames waited so far, by kind (diagnostics).</summary>
    public int Retraces { get; private set; }

    public int Polls { get; private set; }

    public StepOutcome Begin(StepContext context)
    {
        _run = FatMikePhase.Run(context.State, context.Random, new LoadRecorder(host, context.State, files)).GetEnumerator();
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
    private sealed class LoadRecorder(IFatMikeHost host, Dgroup state, Func<int, ImageFileLengths> files) : IFatMikeHost
    {
        public void Emit(FieldEvent e)
        {
            // [ORIGINAL] 186C:0002 – the picture table entry and DS:09C8 (ImageTable.RecordLoad); unpacking is the output's
            if (e.Kind == FieldEventKind.LoadImage) ImageTable.RecordLoad(state, e.Args[0], files);
            host.Emit(e);
        }

        public void Emit(FatMikeEvent e) => host.Emit(e);
        public void ReadControls(Dgroup d) => host.ReadControls(d);
        public IEnumerable<FieldWait> PauseCheck(Dgroup d) => host.PauseCheck(d);
        public IEnumerable<FieldWait> WaitLoop(Dgroup d, OriginalRandom random, short mode) => host.WaitLoop(d, random, mode);
        public byte[] FarString(ushort offset, ushort segment) => host.FarString(offset, segment);
    }
}
