using Traders.Core;
using Traders.Core.Menus;

namespace Traders.App;

/// <summary>
/// The host's command line: <c>traders [path-to-gamedata/dos] [--scale n] [--aspect] [--frames n] [--original-roboter]</c>.
/// The host skips the robot phase (call 18) unless <c>--original-roboter</c> is given (decision F3 = A of R7).
/// </summary>
/// <param name="Directory">Directory of the original files as given (null: searched by the caller).</param>
/// <param name="Options">Window options.</param>
/// <param name="SkipRobotPhase">Robot switch: true unless <c>--original-roboter</c> was given.</param>
public sealed record HostArguments(string? Directory, HostOptions Options, bool SkipRobotPhase)
{
    /// <summary>Option that turns the robot switch off: the week halts at call 18 as the core does.</summary>
    public const string OriginalRobots = "--original-roboter";

    /// <summary>Reads the arguments in order; a later directory replaces an earlier one, an unknown option counts as one.</summary>
    public static HostArguments Parse(IReadOnlyList<string> args)
    {
        string? dir = null;
        bool skipRobotPhase = true;
        var options = new HostOptions();
        for (int i = 0; i < args.Count; i++)
        {
            switch (args[i])
            {
                case "--scale" when i + 1 < args.Count: options = options with { Scale = Math.Clamp(int.Parse(args[++i]), 1, 8) }; break;
                case "--aspect": options = options with { AspectCorrection = true }; break;
                case OriginalRobots: skipRobotPhase = false; break;
                case "--frames" when i + 1 < args.Count: options = options with { MaxFrames = long.Parse(args[++i]) }; break;
                default: dir = args[i]; break;
            }
        }
        return new HostArguments(dir, options, skipRobotPhase);
    }

    /// <summary>The program run for the original files in <paramref name="directory"/> with the robot switch of these arguments.</summary>
    public ProgramHost CreateProgram(string directory, IClockInput clock, IMenuHost? menuHost = null) =>
        new(directory, clock, menuHost, SkipRobotPhase);
}
