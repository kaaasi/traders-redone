namespace Traders.Core.Phases;

/// <summary>
/// A call step whose handler is made anew at every call, from the state at the call site: for steps whose inputs are only
/// known when the call starts (the rule page on the draw page of that moment, the text buffer after the program start, the
/// driver's current font). <see cref="Begin"/> creates the handler and begins it; every <see cref="Tick"/> goes to it.
/// </summary>
/// <param name="create">Makes the handler of one call (the context is the one of <see cref="Begin"/>).</param>
public sealed class DeferredStep(Func<StepContext, IWeekStepHandler> create) : IWeekStepHandler
{
    /// <summary>Handler of the current or last call (null before the first).</summary>
    public IWeekStepHandler? Current { get; private set; }

    public StepOutcome Begin(StepContext context)
    {
        Current = create(context) ?? throw new InvalidOperationException("no handler made for the call");
        return Current.Begin(context);
    }

    public StepOutcome Tick(StepContext context) =>
        (Current ?? throw new InvalidOperationException("Not started.")).Tick(context);
}
