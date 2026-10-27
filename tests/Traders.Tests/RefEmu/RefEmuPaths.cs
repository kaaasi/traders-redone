namespace Traders.Tests.RefEmu;

internal static class RefEmuPaths
{
    public static string RepoRoot => RepositoryPaths.Root;

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

}
