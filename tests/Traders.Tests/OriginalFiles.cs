namespace Traders.Tests;

internal static class OriginalFiles
{
    private static readonly Lazy<string> Root = new(FindRoot);

    public static string DosDir => Path.Combine(Root.Value, "original", "dos");

    public static string Dos(string name) => Path.Combine(DosDir, name);

    private static string FindRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "TradersRedone.slnx"))) return dir.FullName;
            dir = dir.Parent;
        }
        throw new InvalidOperationException("Repository root (TradersRedone.slnx) not found.");
    }
}
