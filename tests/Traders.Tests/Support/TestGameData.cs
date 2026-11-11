namespace Traders.Tests;

/// <summary>
/// Location of the original DOS files. They are not part of this repository (private repo kaaasi/traders-referenz,
/// loaded with tools/claude/fetch-reference.sh into gamedata/ or pointed to by TRADERS_GAMEDATA).
/// </summary>
internal static class TestGameData
{
    /// <summary>Folder with T.EXE, *.PP, *.CMF: TRADERS_GAMEDATA/dos if set, otherwise gamedata/dos in the repository.</summary>
    public static string DosDir
    {
        get
        {
            var root = Environment.GetEnvironmentVariable("TRADERS_GAMEDATA");
            return string.IsNullOrEmpty(root) ? RepositoryPaths.File("gamedata", "dos") : Path.Combine(root, "dos");
        }
    }

    public static string Dos(string name) => Path.Combine(DosDir, name);

    public static bool Available => File.Exists(Dos("T.EXE"));

    /// <summary>
    /// Skip reason for tests that need the original files, or null when they must run:
    /// TR_REQUIRE_GAMEDATA=1 always runs them (missing data then fails the test), TR_SKIP_GAMEDATA=1 always skips them,
    /// otherwise they are skipped only when the data is missing.
    /// </summary>
    public static string? SkipReason
    {
        get
        {
            if (Environment.GetEnvironmentVariable("TR_REQUIRE_GAMEDATA") == "1") return null;
            if (Environment.GetEnvironmentVariable("TR_SKIP_GAMEDATA") == "1") return "Originaldaten übersprungen (TR_SKIP_GAMEDATA=1)";
            return Available ? null : $"Originaldaten fehlen ({DosDir}) – tools/claude/fetch-reference.sh original";
        }
    }
}

/// <summary>Fact that needs the original files; see <see cref="TestGameData.SkipReason"/>. Never "return early" without data.</summary>
public sealed class GameDataFactAttribute : FactAttribute
{
    public GameDataFactAttribute()
    {
        Skip = TestGameData.SkipReason;
    }
}

/// <summary>Theory that needs the original files; see <see cref="TestGameData.SkipReason"/>.</summary>
public sealed class GameDataTheoryAttribute : TheoryAttribute
{
    public GameDataTheoryAttribute()
    {
        Skip = TestGameData.SkipReason;
    }
}
