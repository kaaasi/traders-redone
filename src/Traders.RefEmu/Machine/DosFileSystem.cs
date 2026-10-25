namespace Traders.RefEmu.Machine;

/// <summary>
/// Drive C: of the emulated DOS. Reads come from a host directory (never modified); files the program
/// creates or writes live in memory only, so runs are repeatable and the originals stay untouched.
/// Names are matched case-insensitively in 8.3 form; paths are reduced to their file name.
/// </summary>
public sealed class DosFileSystem
{
    private readonly string _hostDir;
    private readonly Dictionary<string, byte[]> _overlay = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _deleted = new(StringComparer.OrdinalIgnoreCase);

    public DosFileSystem(string hostDir) => _hostDir = hostDir;

    /// <summary>Files written by the program during the run.</summary>
    public IReadOnlyDictionary<string, byte[]> WrittenFiles => _overlay;

    public static string Normalize(string dosPath)
    {
        string p = dosPath.Replace('/', '\\');
        int i = p.LastIndexOf('\\');
        if (i >= 0) p = p[(i + 1)..];
        if (p.Length >= 2 && p[1] == ':') p = p[2..];
        return p.Trim().ToUpperInvariant();
    }

    private string? HostPath(string name)
    {
        if (!Directory.Exists(_hostDir)) return null;
        foreach (var f in Directory.EnumerateFiles(_hostDir))
            if (string.Equals(Path.GetFileName(f), name, StringComparison.OrdinalIgnoreCase)) return f;
        return null;
    }

    public bool Exists(string dosPath)
    {
        string n = Normalize(dosPath);
        if (_overlay.ContainsKey(n)) return true;
        return !_deleted.Contains(n) && HostPath(n) is not null;
    }

    public byte[]? Load(string dosPath)
    {
        string n = Normalize(dosPath);
        if (_overlay.TryGetValue(n, out var d)) return d;
        if (_deleted.Contains(n)) return null;
        var hp = HostPath(n);
        return hp is null ? null : File.ReadAllBytes(hp);
    }

    public void Store(string dosPath, byte[] data)
    {
        string n = Normalize(dosPath);
        _overlay[n] = data;
        _deleted.Remove(n);
    }

    public bool Delete(string dosPath)
    {
        string n = Normalize(dosPath);
        bool existed = Exists(n);
        _overlay.Remove(n);
        _deleted.Add(n);
        return existed;
    }

    /// <summary>All visible file names (for find first/next).</summary>
    public IEnumerable<string> List()
    {
        var names = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
        if (Directory.Exists(_hostDir))
            foreach (var f in Directory.EnumerateFiles(_hostDir))
            {
                string n = Path.GetFileName(f).ToUpperInvariant();
                if (!_deleted.Contains(n)) names.Add(n);
            }
        foreach (var n in _overlay.Keys) names.Add(n.ToUpperInvariant());
        return names;
    }

    /// <summary>DOS wildcard match (* and ?) on 8.3 names.</summary>
    public static bool Matches(string pattern, string name)
    {
        static (string, string) Split(string s)
        {
            int dot = s.LastIndexOf('.');
            return dot < 0 ? (s, "") : (s[..dot], s[(dot + 1)..]);
        }

        static bool Part(string p, string n)
        {
            int i = 0;
            for (; i < p.Length; i++)
            {
                if (p[i] == '*') return true;
                if (i >= n.Length) return p[i..].All(c => c is '?' or '*');
                if (p[i] != '?' && char.ToUpperInvariant(p[i]) != char.ToUpperInvariant(n[i])) return false;
            }
            return i == n.Length;
        }

        var (pb, pe) = Split(Normalize(pattern));
        var (nb, ne) = Split(name);
        if (pattern.EndsWith("*.*", StringComparison.Ordinal) || pb == "*" && pe == "*") return true;
        return Part(pb, nb) && Part(pe, ne);
    }
}
