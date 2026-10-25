using Traders.Assets;
using Traders.Viewer;

var dir = args.Length > 0 ? args[0] : AssetLibrary.FindOriginalDos() ?? AssetLibrary.FindOriginalDos(Environment.CurrentDirectory);
if (dir is null)
{
    Console.Error.WriteLine("Original directory not found. Usage: traders-viewer <path-to-original/dos>");
    return 1;
}

using var game = new ViewerGame(AssetLibrary.Load(dir));
game.Run();
return 0;
