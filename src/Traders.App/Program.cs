using Traders.App;
using Traders.App.Rendering;
using Traders.Assets;
using Traders.Vga;

// Usage: traders [path-to-gamedata/dos] [--scale n] [--aspect] [--frames n] [--original-roboter] (HostArguments)
// With the original files the host runs the program (start, main menu, new game, week loop of the core); without them it
// shows colour bars. The host skips the robot phase (call 18) unless --original-roboter is given (decision F3 = A of R7).
var arguments = HostArguments.Parse(args);
string? dir = arguments.Directory ?? AssetLibrary.FindOriginalDos() ?? AssetLibrary.FindOriginalDos(Environment.CurrentDirectory);

if (dir is not null)
{
    using var program = new HostGame(arguments.Options, arguments.CreateProgram(dir, Traders.App.Timing.BorlandClock.Host));
    program.Run();
    return 0;
}

var vram = new ModeXMemory();
var dac = DemoPicture.ColourBars(vram);
using var game = new HostGame(arguments.Options, vram, dac);
game.Run();
return 0;
