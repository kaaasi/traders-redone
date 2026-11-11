using Traders.App;
using Traders.App.Rendering;
using Traders.Assets;
using Traders.Vga;

// Usage: traders [path-to-gamedata/dos] [--scale n] [--aspect] [--frames n]
// Without scenes (R3) the host shows the first original picture, or colour bars if no original directory is found.
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

var vram = new ModeXMemory();
byte[] dac;
if (dir is not null)
{
    var picture = AssetLibrary.Load(dir).Pictures[ResourceTable.Pictures[0].Id];
    DemoPicture.Write(vram, picture.Pixels);
    dac = (byte[])picture.Palette.Vga6.Clone();
}
else
{
    dac = DemoPicture.ColourBars(vram);
}

using var game = new HostGame(options, vram, dac);
game.Run();
return 0;
