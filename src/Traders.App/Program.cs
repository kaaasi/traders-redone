using Traders.App;
using Traders.App.Rendering;
using Traders.Assets;
using Traders.Vga;

// Usage: traders [path-to-gamedata/dos] [--scale n] [--aspect] [--frames n]
// With the original files the host runs the program (start, main menu, new game, week loop of the core); without them it
// shows colour bars.
string? dir = null;
var options = new HostOptions();
for (int i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--scale" when i + 1 < args.Length: options = options with { Scale = Math.Clamp(int.Parse(args[++i]), 1, 8) }; break;
        case "--aspect": options = options with { AspectCorrection = true }; break;
        case "--frames" when i + 1 < args.Length: options = options with { MaxFrames = long.Parse(args[++i]) }; break;
        default: dir = args[i]; break;
    }
}
dir ??= AssetLibrary.FindOriginalDos() ?? AssetLibrary.FindOriginalDos(Environment.CurrentDirectory);

if (dir is not null)
{
    using var program = new HostGame(options, new ProgramHost(dir, Traders.App.Timing.BorlandClock.Host));
    program.Run();
    return 0;
}

var vram = new ModeXMemory();
var dac = DemoPicture.ColourBars(vram);
using var game = new HostGame(options, vram, dac);
game.Run();
return 0;
