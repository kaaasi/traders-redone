namespace Traders.Assets;

/// <summary>All original DOS resources loaded from one directory.</summary>
public sealed class AssetLibrary
{
    public string Directory { get; }
    public IReadOnlyDictionary<PictureId, VgaImage> Pictures { get; }
    public FontSet Fonts { get; }
    public MaskImage Masks { get; }
    public TextTable Texts { get; }
    public IReadOnlyDictionary<string, MusicModule> Music { get; }

    private AssetLibrary(string dir, Dictionary<PictureId, VgaImage> pictures, FontSet fonts, MaskImage masks,
        TextTable texts, Dictionary<string, MusicModule> music)
    {
        Directory = dir;
        Pictures = pictures;
        Fonts = fonts;
        Masks = masks;
        Texts = texts;
        Music = music;
    }

    public static AssetLibrary Load(string dir)
    {
        var pictures = ResourceTable.Pictures.ToDictionary(p => p.Id, p => VgaImage.Load(Path.Combine(dir, p.Vga)));
        var music = ResourceTable.Music.ToDictionary(m => m, m => MusicModule.Load(Path.Combine(dir, m)));
        return new AssetLibrary(dir, pictures,
            FontSet.Load(Path.Combine(dir, ResourceTable.Fonts)),
            MaskImage.Load(Path.Combine(dir, ResourceTable.Masks)),
            TextTable.Load(Path.Combine(dir, ResourceTable.EnglishText)),
            music);
    }

    /// <summary>Finds original/dos by walking up from the given start directory (default: app base).</summary>
    public static string? FindOriginalDos(string? start = null)
    {
        var dir = new DirectoryInfo(start ?? AppContext.BaseDirectory);
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, "original", "dos");
            if (File.Exists(Path.Combine(candidate, ResourceTable.Executable))) return candidate;
            dir = dir.Parent;
        }
        return null;
    }
}
