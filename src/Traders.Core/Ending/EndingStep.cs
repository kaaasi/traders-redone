using Traders.Core.Field;
using Traders.Core.Phases;
using Traders.Core.Pixels;
using Traders.Core.Startup;
using Traders.Core.State;

namespace Traders.Core.Ending;

/// <summary>
/// The whole end check 20B3:0005 as a step of the phase machine, for the states in which the game ends (the rest of
/// <see cref="EndCheckStep"/>, which hands it the whole call from the unchanged state): <see cref="Begin"/> makes the
/// decision (<see cref="EndCheck.Decide"/>) and runs <see cref="EndScreens.Run"/> up to its first wait, every
/// <see cref="Tick"/> one frame further; the step finishes with AL = 1 in the frame in which 20B3:0005 returns (AL = 0 at once
/// if the game goes on after all). The rule page of the evening starts on the draw page the data segment states
/// (<see cref="RulePage.DrawPageOf"/>); the picture loads 186C:0002 are recorded in the picture table
/// (<see cref="ImageTable.RecordLoad"/>) when they are emitted. Not bound into the machine yet (lane partie, R9).
/// </summary>
/// <param name="host">Outputs, the report 0825:000A(0) and (for tests) the pixel reads.</param>
/// <param name="files">Lengths of the picture files (as for the main menu).</param>
/// <param name="images">Pixels of picture n (186C:0002(n)) for the rule page.</param>
/// <param name="fonts">The driver's fonts by number (21FD:05DB) for the rule page.</param>
public sealed class EndingStep(IEndingHost host, Func<int, ImageFileLengths> files, Func<int, IndexedImage> images,
    IReadOnlyList<RuleFont> fonts) : IWeekStepHandler
{
    private IEnumerator<FieldWait>? _run;

    /// <summary>The rule page of the running end screens.</summary>
    public EndingPage? Page { get; private set; }

    public StepOutcome Begin(StepContext context)
    {
        if (!EndCheck.Decide(context.State)) return StepOutcome.Finished(0); // 20B3:004F
        Page = new EndingPage(new RulePage(RulePage.DrawPageOf(context.State)), fonts);
        _run = EndScreens.Run(context.State, context.Random, new LoadRecorder(host, context.State, files), Page, images)
            .GetEnumerator();
        return Next();
    }

    public StepOutcome Tick(StepContext context)
    {
        if (_run is null) throw new InvalidOperationException("Not started.");
        return Next();
    }

    private StepOutcome Next()
    {
        if (_run!.MoveNext()) return StepOutcome.Running;
        _run.Dispose();
        _run = null;
        return StepOutcome.Finished(1); // 20B3:0088
    }

    /// <summary>Passes everything to the host; a picture load is first recorded in the picture table.</summary>
    private sealed class LoadRecorder(IEndingHost host, Dgroup state, Func<int, ImageFileLengths> files) : IEndingHost
    {
        public void ComputerTurn(Dgroup d, short x, short y) => host.ComputerTurn(d, x, y);

        public void Emit(FieldEvent e)
        {
            // [ORIGINAL] 186C:0002 – the picture table entry and DS:09C8 (ImageTable.RecordLoad); unpacking is the output's
            if (e.Kind == FieldEventKind.LoadImage) ImageTable.RecordLoad(state, e.Args[0], files);
            host.Emit(e);
        }

        public void Emit(EndingEvent e) => host.Emit(e);
        public byte ReadPixel(RulePage page, short x, short y) => host.ReadPixel(page, x, y);
        public IEnumerable<FieldWait> Report(Dgroup d, OriginalRandom random) => host.Report(d, random);
    }
}
