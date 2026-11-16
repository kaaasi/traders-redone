using Traders.Core.Phases;

namespace Traders.App.Deviations;

/// <summary>
/// The host's stand-in for call 18 of the week loop, the robot phase 1C68:000B (robot purchase, line-up and fight; ported in
/// R10–R11): it returns in the frame it starts, without changing a byte of the data segment, without drawing a random number
/// and without drawing anything. With it the week runs on to the end check and the trade; the core itself stays faithful and
/// halts at call 18 without it (switch <c>--original-roboter</c>).
/// </summary>
// [APPROX] bewusste Abweichung, Nutzer 16.11.2026 (F3 = A): the robot phase is skipped, so robots are neither bought nor
// fought for and the week goes on with the state of the harvest; on by default, the host notes it in the window title.
public sealed class RobotPhaseSkip : IWeekStepHandler
{
    /// <summary>Calls skipped so far.</summary>
    public int Skipped { get; private set; }

    public StepOutcome Begin(StepContext context)
    {
        Skipped++;
        return StepOutcome.Finished();
    }

    public StepOutcome Tick(StepContext context) => throw new InvalidOperationException("the skipped robot phase finishes at once");
}
