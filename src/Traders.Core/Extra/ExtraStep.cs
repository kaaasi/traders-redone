using Traders.Core.Field;
using Traders.Core.Phases;
using Traders.Core.Startup;
using Traders.Core.State;

namespace Traders.Core.Extra;

/// <summary>
/// Call 24 of the week loop 1777:01F8, the extra phase 1DEF:000C, as a step of the phase machine: <see cref="Begin"/> runs
/// <see cref="ExtraPhase.Run"/> up to its first wait, every <see cref="Tick"/> one frame further (a retrace or a poll of the
/// pause loop); the step finishes in the frame in which 1DEF:000C returns (at once when the phase does not fall). The picture
/// load 186C:0002 is recorded in the picture table (<see cref="ImageTable.RecordLoad"/>) when it is emitted; all other events
/// go to <paramref name="host"/> unchanged. Bound into the machine by lane ende (docs/arbeitspakete.md R7).
/// </summary>
/// <param name="host">Controls, pause, wait loop, texts and the outputs (scene, sound).</param>
/// <param name="files">Lengths of the picture files (as for the main menu).</param>
/// <param name="frame">Stack frame of 1DEF:000C for the far pointers it stores (default: as in the game, <see cref="GameFrame"/>).</param>
public sealed class ExtraStep(IExtraHost host, Func<int, ImageFileLengths> files, ExtraFrame? frame = null) : IWeekStepHandler
{
    /// <summary>
    /// The frame of 1DEF:000C when the week loop calls it: BP = 0FD8h in the stack segment, which lies right behind DGROUP
    /// (SS = DGROUP + 58Ah, written relative to <see cref="DgroupLayout.Segment"/>). Measured in the original in weeks 0–3 of a
    /// running game (test Frame_in_real_games_is_the_game_frame, docs/routines/1DEF_000C_zusatzphase.md).
    /// </summary>
    // [ORIGINAL] 1DEF:000C – SS:SP = DGROUP + 58Ah : 0FDAh at the entry from the week loop (RefEmu probe)
    // [APPROX] The port has no load address: SS is given relative to the image's DGROUP (DgroupLayout.Segment), as the
    // host numbers segments differently (its text buffer, for example); a host that mirrors the original's memory passes its frame.
    public static readonly ExtraFrame GameFrame = new(0x0FD8, (ushort)(DgroupLayout.Segment + 0x058A));

    private IEnumerator<FieldWait>? _run;

    /// <summary>The board of the running phase (the original's stack arrays).</summary>
    public ExtraBoard Board { get; } = new();

    /// <summary>Frames waited so far, by kind (diagnostics).</summary>
    public int Retraces { get; private set; }

    public int Polls { get; private set; }

    public StepOutcome Begin(StepContext context)
    {
        _run = ExtraPhase.Run(context.State, context.Random, new LoadRecorder(host, context.State, files), Board,
            frame ?? GameFrame).GetEnumerator();
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
    private sealed class LoadRecorder(IExtraHost host, Dgroup state, Func<int, ImageFileLengths> files) : IExtraHost
    {
        public void Emit(FieldEvent e)
        {
            // [ORIGINAL] 186C:0002 – the picture table entry and DS:09C8 (ImageTable.RecordLoad); unpacking is the output's
            if (e.Kind == FieldEventKind.LoadImage) ImageTable.RecordLoad(state, e.Args[0], files);
            host.Emit(e);
        }

        public void Emit(ExtraEvent e) => host.Emit(e);
        public void ReadControls(Dgroup d) => host.ReadControls(d);
        public IEnumerable<FieldWait> PauseCheck(Dgroup d) => host.PauseCheck(d);
        public IEnumerable<FieldWait> WaitLoop(Dgroup d, OriginalRandom random, short mode) => host.WaitLoop(d, random, mode);
        public byte[] FarString(ushort offset, ushort segment) => host.FarString(offset, segment);
    }
}
