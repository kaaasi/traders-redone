using Traders.Assets;
using Traders.Viewer;

// Usage: traders-viewer [path-to-original/dos] [--screenshot <out-dir>]
string? dir = null, screenshotDir = null;
for (int i = 0; i < args.Length; i++)
{
    if (args[i] == "--screenshot" && i + 1 < args.Length) screenshotDir = args[++i];
    else dir = args[i];
}
dir ??= AssetLibrary.FindOriginalDos() ?? AssetLibrary.FindOriginalDos(Environment.CurrentDirectory);
if (dir is null)
{
    Console.Error.WriteLine("Original directory not found. Usage: traders-viewer [path-to-original/dos] [--screenshot <out-dir>]");
    return 1;
}

using var game = new ViewerGame(AssetLibrary.Load(dir), screenshotDir);
game.Run();
return screenshotDir is null || game.ScreenshotsWritten > 0 ? 0 : 2;
