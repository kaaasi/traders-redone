namespace Traders.Tests;

internal static class OriginalFiles
{
    public static string DosDir => RepositoryPaths.File("original", "dos");
    public static string Dos(string name) => Path.Combine(DosDir, name);
}
