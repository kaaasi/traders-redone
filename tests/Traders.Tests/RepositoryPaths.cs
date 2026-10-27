namespace Traders.Tests;

internal static class RepositoryPaths
{
    private static readonly Lazy<string> RootPath = new(FindRoot);
    public static string Root => RootPath.Value;
    public static string File(params string[] parts) => Path.Combine(new[] { Root }.Concat(parts).ToArray());

    private static string FindRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (System.IO.File.Exists(Path.Combine(directory.FullName, "TradersRedone.slnx")))
                return directory.FullName;
            directory = directory.Parent;
        }
        throw new InvalidOperationException("Repository root (TradersRedone.slnx) not found.");
    }
}
