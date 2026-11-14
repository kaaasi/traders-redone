using Traders.Core.State;

namespace Traders.Core.Phases;

public enum PhaseStatus
{
    /// <summary>Between two steps; the next frame starts <see cref="PhaseMachine.NextStep"/>.</summary>
    Ready,

    /// <summary>A step with a screen runs and is ticked once per frame.</summary>
    Waiting,

    /// <summary>
    /// The next step is not ported yet: it has no handler, or its handler does not cover the current state
    /// (<see cref="IPartialWeekStepHandler"/>); the machine does not skip it.
    /// </summary>
    Blocked,

    /// <summary>The loop returned (exit A or B); <see cref="PhaseMachine.Result"/> holds AL.</summary>
    Finished,
}

public enum PhaseEventKind
{
    /// <summary>A call step starts (Value = argument or 0).</summary>
    StepStarted,

    /// <summary>A call step returned (Value = AL).</summary>
    StepFinished,

    /// <summary>A phase marker 21FD:0977(n) was passed (Value = n).</summary>
    MarkerPassed,

    /// <summary>The step has no handler (not ported yet); the machine is blocked.</summary>
    StepPending,

    /// <summary>The week counter DS:4B9A was incremented after call 30 (Value = new counter).</summary>
    WeekAdvanced,

    /// <summary>A frame was processed (Value = number of random() calls in it, the per-frame random consumption).</summary>
    Frame,

    /// <summary>The loop returned (Value = AL: 0 main menu, 1 game over).</summary>
    Exited,
}

/// <summary>An event of the phase machine. <see cref="Week"/> is DS:4B9A when it happens, <see cref="Step"/> the call number 1..30.</summary>
public readonly record struct PhaseEvent(PhaseEventKind Kind, int Week, int Step, long Frame, int Value)
{
    public override string ToString() => $"frame {Frame} week {Week} step {Step} {Kind} {Value}";
}

/// <summary>
/// Phase machine of the core: runs the week loop 1777:01F8 (<see cref="WeekLoop"/>) step by step on the data segment.
/// Time enters only through <see cref="Advance"/> (frame numbers from the host); every frame is processed exactly once, in
/// order, and the random draws a step makes in a frame are part of that frame (decision F3 = A, AGENTS.md §4). Player actions
/// arrive as commands (<see cref="Submit"/>) and are handed to the running step. Steps without a handler are pending: the
/// machine stops there (<see cref="PhaseStatus.Blocked"/>) instead of skipping them.
/// </summary>
// [ORIGINAL] 1777:01F8 – order of the calls, exit A after call 3 (DS:3D55), exit B after call 19 (AL = 1), counter at 1777:02D3
public sealed class PhaseMachine
{
    private readonly Dgroup _state;
    private readonly OriginalRandom _random;
    private readonly IReadOnlyDictionary<int, IWeekStepHandler> _handlers;
    private readonly List<IPlayerCommand> _commands = [];
    private readonly List<PhaseEvent> _events = [];
    private long _currentFrame = -1;

    /// <param name="state">Data segment image; the machine and the handlers work on it directly.</param>
    /// <param name="random">The game's generator; its seed is synchronised with DS:10D2 around every handler call.</param>
    /// <param name="handlers">Handlers by call number 1..30. Markers need none; missing call steps are pending.</param>
    /// <param name="nextStep">Call number at which the machine starts (1 = start of a week, as after the new game).</param>
    /// <param name="lastFrame">Last frame already processed (the first <see cref="Advance"/> continues after it).</param>
    public PhaseMachine(Dgroup state, OriginalRandom random, IReadOnlyDictionary<int, IWeekStepHandler> handlers, int nextStep = 1,
        long lastFrame = -1)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(nextStep, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(nextStep, WeekLoop.Steps.Count);
        _state = state;
        _random = random;
        _handlers = handlers;
        NextStep = nextStep;
        LastFrame = lastFrame;
    }

    public PhaseStatus Status { get; private set; }

    /// <summary>Call number of the step that starts next (Ready, Blocked) or runs (Waiting).</summary>
    public int NextStep { get; private set; }

    /// <summary>AL returned by the loop once <see cref="PhaseStatus.Finished"/>.</summary>
    public byte? Result { get; private set; }

    /// <summary>Last frame processed.</summary>
    public long LastFrame { get; private set; }

    /// <summary>Week counter DS:4B9A.</summary>
    public int Week => _state.S16(DgroupLayout.Week);

    /// <summary>Events since the last <see cref="DrainEvents"/>.</summary>
    public IReadOnlyList<PhaseEvent> Events => _events;

    public List<PhaseEvent> DrainEvents()
    {
        var list = new List<PhaseEvent>(_events);
        _events.Clear();
        return list;
    }

    /// <summary>Queues a player command; the running step receives it with the next frame.</summary>
    public void Submit(IPlayerCommand command) => _commands.Add(command ?? throw new ArgumentNullException(nameof(command)));

    /// <summary>
    /// Processes every frame after <see cref="LastFrame"/> up to <paramref name="tick"/>.Frame, each exactly once: the held keys
    /// are written to the key table, the running step is ticked (or the next steps start), and a <see cref="PhaseEventKind.Frame"/>
    /// event records the random draws of that frame. A frame number already processed changes nothing, so a scene that redraws,
    /// pauses or reloads cannot consume random numbers twice.
    /// </summary>
    public void Advance(PhaseTick tick)
    {
        if (tick.HeldScancodes is { } keys) WriteKeyTable(_state, keys);
        while (LastFrame < tick.Frame && Status != PhaseStatus.Finished)
        {
            _currentFrame = LastFrame + 1;
            long drawsBefore = _random.Calls;
            ProcessFrame();
            LastFrame = _currentFrame;
            _events.Add(new PhaseEvent(PhaseEventKind.Frame, Week, NextStep, _currentFrame, (int)(_random.Calls - drawsBefore)));
        }
        _currentFrame = -1;
    }

    // [APPROX] Key table DS:3D47 + scancode: 1 = held, 0 = released. The keyboard handler 27C6:037A is not ported yet (lane host);
    // the loop itself only tests DS:3D55 against zero.
    // Also used by the program machine (ProgramMachine.Advance), which passes frames to a nested phase machine without keys.
    internal static void WriteKeyTable(Dgroup state, IReadOnlyCollection<byte> held)
    {
        for (int sc = 0; sc < KeyTableLength; sc++) state.Set(DgroupLayout.KeyState + sc, (byte)0);
        foreach (byte sc in held)
            if (sc < KeyTableLength) state.Set(DgroupLayout.KeyState + sc, (byte)1);
    }

    private const int KeyTableLength = 0x80;

    private void ProcessFrame()
    {
        switch (Status)
        {
            case PhaseStatus.Waiting:
            {
                var step = WeekLoop.Step(NextStep);
                var outcome = Invoke(step, h => h.Tick(Context(step)));
                if (!outcome.Done) return;
                if (!Complete(step, outcome.Result)) return;
                RunSteps();
                return;
            }
            case PhaseStatus.Ready:
                RunSteps();
                return;
            default:
                return; // Blocked: stays at the pending step
        }
    }

    /// <summary>
    /// Starts steps until one waits for frames, is pending or the loop exits. A full cycle of 30 steps that all finish at once
    /// (only possible with test handlers; every real week has screen steps) ends the frame, so a frame always returns.
    /// </summary>
    private void RunSteps()
    {
        for (int started = 0; started < WeekLoop.Steps.Count; started++)
        {
            var step = WeekLoop.Step(NextStep);
            if (step.Kind == WeekStepKind.Marker)
            {
                // 21FD:0977 is an empty function: nothing but the marker itself.
                _events.Add(new PhaseEvent(PhaseEventKind.MarkerPassed, Week, step.Number, _currentFrame, step.Marker!.Value));
                Advanced();
                continue;
            }
            if (!_handlers.TryGetValue(step.Number, out var handler)
                || (handler is IPartialWeekStepHandler partial && partial.IsPending(_state)))
            {
                Status = PhaseStatus.Blocked;
                _events.Add(new PhaseEvent(PhaseEventKind.StepPending, Week, step.Number, _currentFrame, 0));
                return;
            }
            _events.Add(new PhaseEvent(PhaseEventKind.StepStarted, Week, step.Number, _currentFrame, step.Argument ?? 0));
            var outcome = Invoke(step, h => h.Begin(Context(step)));
            if (!outcome.Done)
            {
                Status = PhaseStatus.Waiting;
                return;
            }
            if (!Complete(step, outcome.Result)) return;
        }
    }

    /// <summary>Step returned: exit checks of the loop, then move on. False if the loop exited.</summary>
    private bool Complete(WeekStep step, byte al)
    {
        _events.Add(new PhaseEvent(PhaseEventKind.StepFinished, Week, step.Number, _currentFrame, al));
        switch (step.ExitAfter)
        {
            case WeekExitCheck.AbortToMenu when _state.U8(WeekLoop.AbortKeyState) != 0:
                Exit(step, WeekLoop.ResultMainMenu);
                return false;
            case WeekExitCheck.GameOver when al == 1:
                Exit(step, WeekLoop.ResultGameOver);
                return false;
        }
        Status = PhaseStatus.Ready;
        Advanced();
        return true;
    }

    /// <summary>Moves to the next call; after call 30 increments the week counter and starts again with call 1.</summary>
    private void Advanced()
    {
        if (NextStep < WeekLoop.Steps.Count)
        {
            NextStep++;
            return;
        }
        _state.Set(DgroupLayout.Week, Int16Math.S(_state.S16(DgroupLayout.Week) + 1)); // inc word [4B9A], 16-bit wrap
        NextStep = 1;
        _events.Add(new PhaseEvent(PhaseEventKind.WeekAdvanced, Week, WeekLoop.Steps.Count, _currentFrame, Week));
    }

    private void Exit(WeekStep step, byte al)
    {
        Status = PhaseStatus.Finished;
        Result = al;
        _events.Add(new PhaseEvent(PhaseEventKind.Exited, Week, step.Number, _currentFrame, al));
    }

    private StepContext Context(WeekStep step) => new(_state, _random, step, _currentFrame, _commands);

    /// <summary>
    /// Calls the handler with the generator seeded from DS:10D2 and stores a changed seed back. A handler that writes the data
    /// segment as a whole (for example from the RefEmu) and draws nothing keeps its DS:10D2.
    /// </summary>
    private StepOutcome Invoke(WeekStep step, Func<IWeekStepHandler, StepOutcome> call)
    {
        uint seed = _state.U32(DgroupLayout.RandomSeed);
        _random.Seed = seed;
        long calls = _random.Calls;
        var outcome = call(_handlers[step.Number]);
        if (_random.Calls != calls || _random.Seed != seed) _state.Set(DgroupLayout.RandomSeed, _random.Seed);
        return outcome;
    }
}
