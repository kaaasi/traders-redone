using Traders.Core.State;

namespace Traders.Core.Phases;

/// <summary>
/// Implementation of one call of main (<see cref="ProgramLoop"/>). Steps without a screen finish in <see cref="Begin"/>; steps
/// with a screen return <see cref="StepOutcome.Running"/> and are ticked once per frame until they finish. Handlers change state
/// only through <see cref="ProgramStepContext"/>.
/// </summary>
public interface IProgramStepHandler
{
    StepOutcome Begin(ProgramStepContext context);

    StepOutcome Tick(ProgramStepContext context);
}

/// <summary>What a program step handler gets from the machine for one call of Begin or Tick.</summary>
public sealed class ProgramStepContext
{
    private readonly List<IPlayerCommand> _commands;

    internal ProgramStepContext(Dgroup state, OriginalRandom random, ProgramStep step, long frame, List<IPlayerCommand> commands)
    {
        State = state;
        Random = random;
        Step = step;
        Frame = frame;
        _commands = commands;
    }

    /// <summary>Game state as byte image of the original data segment.</summary>
    public Dgroup State { get; }

    /// <summary>The game's random generator (state DS:10D2 is kept in sync by the machine).</summary>
    public OriginalRandom Random { get; }

    public ProgramStep Step { get; }

    /// <summary>Frame being processed.</summary>
    public long Frame { get; }

    /// <summary>Commands submitted since the last frame, oldest first; a handler removes what it consumes.</summary>
    public IReadOnlyList<IPlayerCommand> Commands => _commands;

    /// <summary>Takes the oldest submitted command (null if none).</summary>
    public IPlayerCommand? TakeCommand()
    {
        if (_commands.Count == 0) return null;
        var c = _commands[0];
        _commands.RemoveAt(0);
        return c;
    }
}

public enum ProgramStatus
{
    /// <summary>Between two steps; the next frame starts <see cref="ProgramMachine.NextStep"/>.</summary>
    Ready,

    /// <summary>A program step with a screen runs and is ticked once per frame.</summary>
    Waiting,

    /// <summary>The week loop runs (<see cref="ProgramMachine.WeekMachine"/>); it gets every frame.</summary>
    InWeek,

    /// <summary>The next program step has no handler (not ported yet); the machine does not skip it.</summary>
    Blocked,

    /// <summary>main reached the teardown; <see cref="ProgramMachine.ExitCode"/> holds the code of the closing message.</summary>
    Ended,
}

public enum ProgramEventKind
{
    /// <summary>A program step starts (Value = first argument or 0).</summary>
    StepStarted,

    /// <summary>A program step returned (Value = its result).</summary>
    StepFinished,

    /// <summary>The guard of the step was false and main went past it (Value = 0).</summary>
    StepSkipped,

    /// <summary>The step has no handler (not ported yet); the machine is blocked.</summary>
    StepPending,

    /// <summary>DS:08FA was set to 1 before the week loop (1777:017A).</summary>
    GameRunningSet,

    /// <summary>A frame was processed (Value = number of random() calls in it, inside the week loop included).</summary>
    Frame,

    /// <summary>main reached the teardown 1777:018C (Value = exit code; Step = the step whose result ended the program).</summary>
    Ended,
}

/// <summary>An event of the program machine; <see cref="Step"/> is the program step number 1..10.</summary>
public readonly record struct ProgramEvent(ProgramEventKind Kind, int Step, long Frame, int Value)
{
    public override string ToString() => $"frame {Frame} step {Step} {Kind} {Value}";
}

/// <summary>
/// Program machine of the core: runs main 1777:000C (<see cref="ProgramLoop"/>) step by step on the data segment, with the
/// week loop as a nested <see cref="PhaseMachine"/>. Time enters only through <see cref="Advance"/>; every frame is processed
/// exactly once and is passed to the running step or the week loop. Steps without a handler are pending: the machine stops
/// there (<see cref="ProgramStatus.Blocked"/>) instead of skipping them.
/// </summary>
// [ORIGINAL] 1777:000C – step order, checks at 1777:0120/0132/0142/0168/016E/0184, DS:08FA = 1 at 1777:017A, loop back at 1777:018A
// [APPROX] Command line (1777:0014..0116), teardown (1777:018C..01A4) and the closing message (1777:01A5..01EF) are the host's:
// the machine ends with the exit code that selects the message (0 normal end, 1/2 errors of the initialisation).
public sealed class ProgramMachine
{
    private readonly Dgroup _state;
    private readonly OriginalRandom _random;
    private readonly IReadOnlyDictionary<int, IProgramStepHandler> _handlers;
    private readonly IReadOnlyDictionary<int, IWeekStepHandler> _weekHandlers;
    private readonly List<IPlayerCommand> _commands = [];
    private readonly List<ProgramEvent> _events = [];
    private readonly List<PhaseEvent> _weekEvents = [];
    private long _currentFrame = -1;

    /// <param name="state">Data segment image; the machine and the handlers work on it directly.</param>
    /// <param name="random">The game's generator; its seed is synchronised with DS:10D2 around every handler call.</param>
    /// <param name="handlers">Handlers by program step number 1..9; missing call steps are pending.</param>
    /// <param name="weekHandlers">Handlers of the week loop by call number 1..30 (see <see cref="PhaseMachine"/>).</param>
    /// <param name="nextStep">Step at which the machine starts (1 = program start).</param>
    /// <param name="lastFrame">Last frame already processed.</param>
    public ProgramMachine(Dgroup state, OriginalRandom random, IReadOnlyDictionary<int, IProgramStepHandler> handlers,
        IReadOnlyDictionary<int, IWeekStepHandler> weekHandlers, int nextStep = 1, long lastFrame = -1)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(nextStep, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(nextStep, ProgramLoop.Steps.Count);
        _state = state;
        _random = random;
        _handlers = handlers;
        _weekHandlers = weekHandlers;
        NextStep = nextStep;
        LastFrame = lastFrame;
    }

    public ProgramStatus Status { get; private set; }

    /// <summary>Program step that starts next (Ready, Blocked) or runs (Waiting, InWeek).</summary>
    public int NextStep { get; private set; }

    /// <summary>The running week loop while <see cref="ProgramStatus.InWeek"/>, otherwise null.</summary>
    public PhaseMachine? WeekMachine { get; private set; }

    /// <summary>True when the machine or the running week loop waits at a step that is not ported yet.</summary>
    public bool IsBlocked => Status == ProgramStatus.Blocked || WeekMachine?.Status == PhaseStatus.Blocked;

    /// <summary>Result of the initialisation (main keeps it in DI); selects the closing message once <see cref="ProgramStatus.Ended"/>.</summary>
    public int ExitCode { get; private set; }

    /// <summary>Last frame processed.</summary>
    public long LastFrame { get; private set; }

    /// <summary>Events since the last <see cref="DrainEvents"/>.</summary>
    public IReadOnlyList<ProgramEvent> Events => _events;

    public List<ProgramEvent> DrainEvents()
    {
        var list = new List<ProgramEvent>(_events);
        _events.Clear();
        return list;
    }

    /// <summary>Events of the week loops run so far, since the last <see cref="DrainWeekEvents"/>.</summary>
    public IReadOnlyList<PhaseEvent> WeekEvents => _weekEvents;

    public List<PhaseEvent> DrainWeekEvents()
    {
        var list = new List<PhaseEvent>(_weekEvents);
        _weekEvents.Clear();
        return list;
    }

    /// <summary>Queues a player command; the running program step or week step receives it with the next frame.</summary>
    public void Submit(IPlayerCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (WeekMachine is { } week) week.Submit(command);
        else _commands.Add(command);
    }

    /// <summary>
    /// Processes every frame after <see cref="LastFrame"/> up to <paramref name="tick"/>.Frame, each exactly once (see
    /// <see cref="PhaseMachine.Advance"/>); inside the week loop the frame goes to the nested phase machine.
    /// </summary>
    public void Advance(PhaseTick tick)
    {
        if (tick.HeldScancodes is { } keys) PhaseMachine.WriteKeyTable(_state, keys);
        while (LastFrame < tick.Frame && Status != ProgramStatus.Ended)
        {
            _currentFrame = LastFrame + 1;
            long drawsBefore = _random.Calls;
            ProcessFrame();
            LastFrame = _currentFrame;
            _events.Add(new ProgramEvent(ProgramEventKind.Frame, NextStep, _currentFrame, (int)(_random.Calls - drawsBefore)));
        }
        _currentFrame = -1;
    }

    private void ProcessFrame()
    {
        switch (Status)
        {
            case ProgramStatus.Waiting:
            {
                var step = ProgramLoop.Step(NextStep);
                var outcome = Invoke(step, h => h.Tick(Context(step)));
                if (outcome.Done && Complete(step, outcome.Result)) RunSteps();
                return;
            }
            case ProgramStatus.InWeek:
                if (TickWeek()) RunSteps();
                return;
            case ProgramStatus.Ready:
                RunSteps();
                return;
            default:
                return; // Blocked: stays at the pending step
        }
    }

    /// <summary>
    /// Starts steps until one waits for frames, the week loop runs on, a step is pending or the program ends. At most one round
    /// of the ten steps per frame (only test handlers finish a whole round at once), so a frame always returns.
    /// </summary>
    private void RunSteps()
    {
        for (int started = 0; started < ProgramLoop.Steps.Count; started++)
        {
            var step = ProgramLoop.Step(NextStep);
            if (step.Guard == ProgramGuard.NewGameRequested && _state.U8(ProgramLoop.NewGameRequest) != 1)
            {
                _events.Add(new ProgramEvent(ProgramEventKind.StepSkipped, step.Number, _currentFrame, 0));
                NextStep++;
                continue;
            }
            if (step.Kind == ProgramStepKind.WeekLoop)
            {
                _state.Set(ProgramLoop.GameRunning, (byte)1); // [ORIGINAL] 1777:000C (site 017A) mov byte [08FA], 1
                _events.Add(new ProgramEvent(ProgramEventKind.GameRunningSet, step.Number, _currentFrame, 1));
                _events.Add(new ProgramEvent(ProgramEventKind.StepStarted, step.Number, _currentFrame, 0));
                WeekMachine = new PhaseMachine(_state, _random, _weekHandlers, nextStep: 1, lastFrame: _currentFrame - 1);
                Status = ProgramStatus.InWeek;
                if (!TickWeek()) return;
                continue;
            }
            if (!_handlers.ContainsKey(step.Number))
            {
                Status = ProgramStatus.Blocked;
                _events.Add(new ProgramEvent(ProgramEventKind.StepPending, step.Number, _currentFrame, 0));
                return;
            }
            _events.Add(new ProgramEvent(ProgramEventKind.StepStarted, step.Number, _currentFrame,
                step.Arguments.Count > 0 ? step.Arguments[0] : 0));
            var outcome = Invoke(step, h => h.Begin(Context(step)));
            if (!outcome.Done)
            {
                Status = ProgramStatus.Waiting;
                return;
            }
            if (!Complete(step, outcome.Result)) return;
        }
    }

    /// <summary>Gives the current frame to the week loop. True if the loop returned and main went on (not ended).</summary>
    private bool TickWeek()
    {
        var week = WeekMachine!;
        week.Advance(new PhaseTick(_currentFrame));
        _weekEvents.AddRange(week.DrainEvents());
        if (week.Status != PhaseStatus.Finished) return false;
        WeekMachine = null;
        return Complete(ProgramLoop.Step(ProgramLoop.WeekLoopStep), week.Result!.Value);
    }

    /// <summary>Step returned: checks of main, then move on. False if the program ended.</summary>
    private bool Complete(ProgramStep step, byte result)
    {
        _events.Add(new ProgramEvent(ProgramEventKind.StepFinished, step.Number, _currentFrame, result));
        Status = ProgramStatus.Ready;
        switch (step.After)
        {
            case ProgramResultCheck.EndIfNonZero when result != 0:
                ExitCode = result; // mov di, ax
                End(step);
                return false;
            case ProgramResultCheck.JumpIfNonZero when result != 0:
                NextStep = step.JumpTo!.Value;
                return true;
            case ProgramResultCheck.EndIfZero when result == 0:
                End(step);
                return false;
            case ProgramResultCheck.EndIfOneElseJump:
                if (result == 1)
                {
                    End(step);
                    return false;
                }
                NextStep = step.JumpTo!.Value;
                return true;
        }
        NextStep = step.Number + 1;
        return true;
    }

    private void End(ProgramStep step)
    {
        Status = ProgramStatus.Ended;
        _events.Add(new ProgramEvent(ProgramEventKind.Ended, step.Number, _currentFrame, ExitCode));
    }

    private ProgramStepContext Context(ProgramStep step) => new(_state, _random, step, _currentFrame, _commands);

    /// <summary>Calls the handler with the generator seeded from DS:10D2 and stores a changed seed back (as the phase machine).</summary>
    private StepOutcome Invoke(ProgramStep step, Func<IProgramStepHandler, StepOutcome> call)
    {
        uint seed = _state.U32(DgroupLayout.RandomSeed);
        _random.Seed = seed;
        long calls = _random.Calls;
        var outcome = call(_handlers[step.Number]);
        if (_random.Calls != calls || _random.Seed != seed) _state.Set(DgroupLayout.RandomSeed, _random.Seed);
        return outcome;
    }
}
