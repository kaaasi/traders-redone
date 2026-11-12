using Traders.Core.State;

namespace Traders.Core.Phases;

/// <summary>A player action for the core (planting a field, a bid, a purchase …). Concrete commands come with their phases.</summary>
public interface IPlayerCommand
{
    /// <summary>Player number 0..3 who issues the command.</summary>
    int Player { get; }
}

/// <summary>Result of starting or ticking a step: still running (waits for the next frame) or done with the AL result.</summary>
public readonly record struct StepOutcome(bool Done, byte Result)
{
    public static StepOutcome Running => new(false, 0);

    /// <summary>The step returned; <paramref name="al"/> is its AL value (only the end check 20B3:0005 uses it).</summary>
    public static StepOutcome Finished(byte al = 0) => new(true, al);
}

/// <summary>
/// Implementation of one call of the week loop. Rule steps without a screen finish in <see cref="Begin"/>; steps with a screen
/// return <see cref="StepOutcome.Running"/> and are ticked once per frame until they finish. Handlers change state only through
/// <see cref="StepContext"/> (data segment, random generator, commands).
/// </summary>
public interface IWeekStepHandler
{
    StepOutcome Begin(StepContext context);

    StepOutcome Tick(StepContext context);
}

/// <summary>What a step handler gets from the machine for one call of <see cref="IWeekStepHandler.Begin"/> or Tick.</summary>
public sealed class StepContext
{
    private readonly List<IPlayerCommand> _commands;

    internal StepContext(Dgroup state, OriginalRandom random, WeekStep step, long frame, List<IPlayerCommand> commands)
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

    public WeekStep Step { get; }

    /// <summary>Frame being processed, or -1 when the step starts outside a frame (machine started without a tick).</summary>
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

/// <summary>
/// Input of one call of <see cref="PhaseMachine.Advance"/>: the frame number the host has reached (70 Hz, counted from 0)
/// and, if known, the scancodes held now. The host derives the frame from its clock; the core never reads a clock.
/// </summary>
// [APPROX] One tick = one 70 Hz frame; how the original's wait loops relate to the frame change is to be confirmed with the
// scenes (docs/routines/1777_01F8_wochenschleife.md).
public readonly record struct PhaseTick(long Frame, IReadOnlyCollection<byte>? HeldScancodes = null);
