namespace Traders.Tests.RefEmu;

internal static class RefEmuPaths
{
    private static readonly Lazy<string> Root = new(FindRoot);

    public static string RepoRoot => Root.Value;

    public static string Dos(string name) => Path.Combine(RepoRoot, "original", "dos", name);

    /// <summary>Output folder for diagnostic files (CI experiment sets REFEMU_OUT); null when not set.</summary>
    public static string? OutputDir
    {
        get
        {
            var dir = Environment.GetEnvironmentVariable("REFEMU_OUT");
            if (string.IsNullOrEmpty(dir)) return null;
            Directory.CreateDirectory(dir);
            return dir;
        }
    }

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
